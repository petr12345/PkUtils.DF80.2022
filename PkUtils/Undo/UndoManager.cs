// Ignore Spelling: Utils
//
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.Undo;

/// <summary>
/// Manages the undo and redo buffer as the root <see cref="UndoableCompoundEdit"/>.
/// Provides thread-safe operations for adding, undoing, redoing, and trimming edits.
/// </summary>
[CLSCompliant(true)]
public class UndoManager : UndoableCompoundEdit
{
    #region Typedefs

    /// <summary>
    /// Represents the current status of the <see cref="UndoManager"/>.
    /// </summary>
    public enum MgrStatus
    {
        /// <summary>
        /// Unknown ( undefined ) status.
        /// </summary>
        Unknown,

        /// <summary>
        /// This enum value represents a status when undo manager is "listening", i.e. recording the editing steps.
        /// </summary>
        Listening,

        /// <summary>
        /// This enum value represents a status when the Undo operation is in progress.
        /// </summary>
        Undoing,

        /// <summary>
        /// This enum value represents a status when the Redo operation is in progress.
        /// </summary>
        Redoing,
    }
    #endregion // Typedefs

    #region Fields

    /// <summary>
    /// The default maximum number of <see cref="IUndoableEdit"/> items kept in the buffer.
    /// </summary>
    public const int _defaultLimit = 256;

    /// <summary>
    /// The current status of the <see cref="UndoManager"/>.
    /// </summary>
    protected MgrStatus _status = MgrStatus.Listening;

    private int _nIndexOfNextAdd;

    private int _limit = _defaultLimit;
    private readonly object _syncRoot = new();
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Initializes a new instance of the <see cref="UndoManager"/> class with the default limit.
    /// </summary>
    /// <remarks>
    /// Calls the constructor with a single argument, passing <see cref="_defaultLimit"/> as the value.
    /// </remarks>
    public UndoManager()
      : this(_defaultLimit)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UndoManager"/> class with a specified limit.
    /// </summary>
    /// <param name="nLimit">
    /// The maximum number of <see cref="IUndoableEdit"/> items to keep in the buffer. Must be positive.
    /// </param>
    public UndoManager(int nLimit)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(nLimit);
        _limit = nLimit;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Gets or sets the maximum number of <see cref="IUndoableEdit"/> items kept in the buffer.
    /// </summary>
    public int Limit
    {
        get
        {
            return _limit;
        }
        set
        {
            lock (_syncRoot)
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
                _limit = value;
                TrimForLimit();
            }
        }
    }

    /// <summary>
    /// Gets the current number of <see cref="IUndoableEdit"/> items held in the undo/redo buffer.
    /// </summary>
    public int EditsCount { get => _edits.Count; }

    /// <summary>
    /// Gets the current status of the undo manager.
    /// </summary>
    public MgrStatus Status { get => _status; }

    /// <summary>
    /// Gets the index in the internal buffer where the next <see cref="IUndoableEdit"/> will be added.
    /// </summary>
    protected int NextAdd { get => _nIndexOfNextAdd; }
    #endregion // Properties

    #region Methods

    #region Protected Methods

    /// <summary>
    /// Trims the undo buffer if the number of edits exceeds the <see cref="Limit"/>.
    /// Removes the oldest edits to maintain the buffer size.
    /// </summary>
    protected void TrimForLimit()
    {
        int nDelete;
        if (0 < (nDelete = EditsCount - Limit))
        {
            TrimEdits(0, nDelete - 1);
        }
    }

    /// <summary>
    /// Removes edits from the buffer between the specified indices, inclusive.
    /// </summary>
    /// <param name="nFrom">The index of the first edit to remove.</param>
    /// <param name="nTo">The index of the last edit to remove.</param>
    protected void TrimEdits(int nFrom, int nTo)
    {
        if (nFrom <= nTo)
        {   // loop backwards to prevent automatic adjustment to mess us up
            for (int ii = nTo; ii >= nFrom; ii--)
            {
                _edits[ii].Dispose();
                _edits.RemoveAt(ii);
            }
            if (NextAdd > nTo)
            {
                _nIndexOfNextAdd -= (nTo - nFrom + 1);
            }
            else if (NextAdd >= nFrom)
            {
                _nIndexOfNextAdd = nFrom;
            }
        }
    }

    /// <summary>
    /// Returns the most recent significant <see cref="IUndoableEdit"/> to which an Undo should be performed.
    /// If a compound edit is active, returns the active edit if it can be undone.
    /// </summary>
    /// <returns>
    /// The <see cref="IUndoableEdit"/> to be undone, or <c>null</c> if none is available.
    /// </returns>
    protected IUndoableEdit EditToBeUndone()
    {
        IUndoableEdit tempEdit;

        if (IsActive)
        {
            if ((tempEdit = GetActiveEdit()).CanUndo)
            {
                return tempEdit;
            }
        }
        return _edits.Take(NextAdd).LastOrDefault(ed => ed.Significant);
    }

    /// <summary>
    /// Returns the next significant <see cref="IUndoableEdit"/> to which a Redo should be performed.
    /// If a compound edit is active, returns the active edit if it can be undone.
    /// </summary>
    /// <returns>
    /// The <see cref="IUndoableEdit"/> to be redone, or <c>null</c> if none is available.
    /// </returns>
    protected IUndoableEdit EditToBeRedone()
    {
        IUndoableEdit tempEdit;

        if (IsActive)
        {
            if ((tempEdit = GetActiveEdit()).CanUndo)
            {
                return tempEdit;
            }
        }

        if (NextAdd < EditsCount)
        {
            return _edits.Skip(NextAdd).Take(EditsCount - NextAdd).FirstOrDefault(ed => ed.Significant);
        }
        return null;
    }

    /// <summary>
    /// Performs Undo operations up to and including the specified <see cref="IUndoableEdit"/>.
    /// </summary>
    /// <param name="editTo">
    /// The <see cref="IUndoableEdit"/> to which Undo should be performed (inclusive).
    /// </param>
    protected void UndoTo(IUndoableEdit editTo)
    {
        editTo.CheckNotDisposed();
        Debug.Assert(
            0 <= _edits.IndexOf(editTo),
            $"The buffer {nameof(_edits)} must contain this IUndoableEdit");

        for (int nDex = NextAdd - 1; nDex >= 0; nDex--)
        {
            IUndoableEdit edit = _edits[_nIndexOfNextAdd = nDex];
            edit.Undo();
            if (edit == editTo) break;
        }
    }

    /// <summary>
    /// Performs Redo operations up to and including the specified <see cref="IUndoableEdit"/>.
    /// </summary>
    /// <param name="editTo">
    /// The <see cref="IUndoableEdit"/> to which Redo should be performed (inclusive).
    /// </param>
    protected void RedoTo(IUndoableEdit editTo)
    {
        editTo.CheckNotDisposed();
        if (!_edits.Contains(editTo)) throw new ArgumentException("The buffer _edits must contain this IUndoableEdit", nameof(editTo));

        for (; NextAdd < EditsCount;)
        {
            IUndoableEdit edit = _edits[_nIndexOfNextAdd++];
            edit.Redo();
            if (edit == editTo) break;
        }
    }
    #endregion // Protected Methods
    #endregion // Methods

    #region ICompoundEdit Members
    #region IUndoableEdit Members
    #region IUndoable Members

    /// <inheritdoc/>
    public override bool CanUndo
    {
        get
        {
            IUndoableEdit e;
            bool result = false;

            lock (_syncRoot)
            {
                if (this.Status == MgrStatus.Listening)
                {
                    if (IsOpenMultiMode)
                    {
                        e = EditToBeUndone();
                        result = (e != null && e.CanUndo);
                    }
                    else
                    {
                        result = base.CanUndo;
                    }
                }
            }
            return result;
        }
    }

    /// <inheritdoc/>
    public override bool CanRedo
    {
        get
        {
            IUndoableEdit e;
            bool result = false;

            lock (_syncRoot)
            {
                if (this.Status == MgrStatus.Listening)
                {
                    if (IsOpenMultiMode)
                    {
                        e = EditToBeRedone();
                        result = (e != null && e.CanRedo);
                    }
                    else
                    {
                        result = base.CanRedo;
                    }
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Performs the Undo operation.
    /// Overrides the base class implementation to provide specific behavior for <see cref="UndoManager"/>.
    /// </summary>
    public override void Undo()
    {
        lock (_syncRoot)
        {
            Debug.Assert(this.Status == MgrStatus.Listening);
            _status = MgrStatus.Undoing;
            if (IsOpenMultiMode)
            {
                this.UndoTo(EditToBeUndone());
            }
            else
            {
                base.Undo();
            }
            _status = MgrStatus.Listening;
        }
    }

    /// <summary>
    /// Performs the Redo operation.
    /// Overrides the base class implementation to provide specific behavior for <see cref="UndoManager"/>.
    /// </summary>
    public override void Redo()
    {
        lock (_syncRoot)
        {
            Debug.Assert(this.Status == MgrStatus.Listening);
            _status = MgrStatus.Redoing;
            if (IsOpenMultiMode)
            {
                this.RedoTo(EditToBeRedone());
            }
            else
            {
                base.Redo();
            }
            _status = MgrStatus.Listening;
        }
    }

    /// <summary>
    /// Clears all Undo and Redo information currently held by this object.
    /// </summary>
    public override void EmptyUndoBuffer()
    {
        lock (_syncRoot)
        {
            base.EmptyUndoBuffer();
            _nIndexOfNextAdd = 0;
        }
    }
    #endregion // IUndoable Members

    /// <inheritdoc/>
    public override string UndoPresentationName
    {
        get
        {
            lock (_syncRoot)
            {
                string strRes = string.Empty;
                if (IsOpenMultiMode)
                {
                    if (CanUndo)
                    {
                        IUndoableEdit edit = EditToBeUndone();
                        strRes = (edit != null) ? edit.UndoPresentationName : string.Empty;
                    }
                    else
                    {
                        strRes = UndoableAbstractEdit.BaseUndoName;
                    }
                }
                else
                {
                    strRes = base.UndoPresentationName;
                }
                return strRes;
            }
        }
    }

    /// <inheritdoc/>
    public override string RedoPresentationName
    {
        get
        {
            lock (_syncRoot)
            {
                string strRes = string.Empty;
                if (IsOpenMultiMode)
                {
                    if (CanRedo)
                    {
                        IUndoableEdit edit = EditToBeRedone();
                        strRes = (edit != null) ? edit.RedoPresentationName : string.Empty;
                    }
                    else
                    {
                        strRes = UndoableAbstractEdit.BaseRedoName;
                    }
                }
                else
                {
                    strRes = base.RedoPresentationName;
                }
                return strRes;
            }
        }
    }

    /// <summary>
    /// Returns a string describing all field values of this instance, including the status, buffer index, limit, and contained edits.
    /// </summary>
    public override string Say
    {
        get
        {
            StringBuilder sbEdits = new();

            for (int ii = 0; ii < NextAdd; ii++)
            {
                if (ii == 0)
                    sbEdits.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", Environment.NewLine, _edits[ii].Say);
                else
                    sbEdits.AppendFormat(CultureInfo.InvariantCulture, ",{0}{1}", Environment.NewLine, _edits[ii].Say);
            }

            string strRes = string.Format(
                CultureInfo.InvariantCulture,
                "UndoManager: (_status={0}, _nIndexOfNextAdd={1}, _limit={2}, _Edits={3})",
                _status,
                _nIndexOfNextAdd,
                _limit,
                sbEdits.ToString());

            return strRes;
        }
    }
    #endregion // IUndoableEdit Members

    /// <summary>
    /// Adds a single <see cref="IUndoableEdit"/> item into the current undo recording action.
    /// Trims the redo buffer and ensures the buffer does not exceed the set limit.
    /// </summary>
    /// <param name="e">The <see cref="IUndoableEdit"/> item to add. Cannot be <c>null</c>.</param>
    /// <returns><c>true</c> on success; otherwise, <c>false</c>.</returns>
    public override bool AddEdit(IUndoableEdit e)
    {
        ArgumentNullException.ThrowIfNull(e);

        bool retVal = false;

        lock (_syncRoot)
        {
            if (this.Status == MgrStatus.Listening)
            {
                // Remove edits from here to end of undo queue.
                this.TrimEdits(NextAdd, EditsCount - 1);
                // Now we can't redo
                retVal = base.AddEdit(e);
                if (IsOpenMultiMode)
                {
                    retVal = true;
                }
                // Update position
                this._nIndexOfNextAdd = EditsCount;
                // Cut beginning to make sure undo queue is not too big
                this.TrimForLimit();
            }
            else
            {
                Debug.Fail("AddEdit has been called with incompatible status" + this.Status.ToString());
            }
        }
        return retVal;
    }

    /// <inheritdoc/>
    public override void EndMultiMode()
    {
        lock (_syncRoot)
        {
            base.EndMultiMode();
            TrimEdits(NextAdd, EditsCount - 1);
        }
    }
    #endregion // ICompoundEdit Members
}

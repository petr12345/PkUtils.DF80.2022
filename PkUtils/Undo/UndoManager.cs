/***************************************************************************************************************
*
* FILE NAME:   .\Undo\UndoManager.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class UndoableCompoundEdit
*
**************************************************************************************************************/

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
/// UndoManager is the "root" UndoableCompoundEdit - the holder of the complete undo/redo buffer.
/// </summary>
[CLSCompliant(true)]
public class UndoManager : UndoableCompoundEdit
{
    #region Typedefs

    /// <summary>
    /// This enum defines current UndoManager status
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
    /// The default value that will be used by argument-less constructor as the initial value for the property
    /// <see cref="Limit"/>.
    /// </summary>
    public const int _defaultLimit = 256;

    /// <summary>
    /// The backing field for the <see cref="Status"/> property.
    /// </summary>
    protected MgrStatus _status = MgrStatus.Listening;

    private int _nIndexOfNextAdd;

    private int _limit = _defaultLimit;
    private readonly object _syncRoot = new();
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default argument-less constructor.
    /// </summary>
    /// <remarks>Calls overloaded single-argument constructor <see cref="UndoManager(int)"/>,
    /// passing as the actual argument value the constant <see cref="_defaultLimit"/> </remarks>
    public UndoManager()
      : this(_defaultLimit)
    { }

    /// <summary>
    /// A single-argument constructor, providing the initial value for the property <see cref="Limit"/>.
    /// </summary>
    /// <param name="nLimit">This limit is a maximal count of IUndoableEdit items kept in the buffer.
    /// The value must be positive; otherwise throws <see cref="ArgumentException"/>.
    /// </param>
    public UndoManager(int nLimit)
    {
        CheckPositive(nLimit, "nLimit");
        _limit = nLimit;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Maximal count of IUndoableEdit items kept in the buffer.
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
                CheckPositive(value, "value");
                _limit = value;
                TrimForLimit();
            }
        }
    }

    /// <summary>
    /// Count of edits of <see cref="IUndoableEdit"/> type, that are currently held in the undo/redo buffer.
    /// </summary>
    public int EditsCount
    {
        get { return _edits.Count; }
    }

    /// <summary>
    /// Returns the current undo manager status, as represented by MgrStatus enum.
    /// </summary>
    public MgrStatus Status
    {
        get { return _status; }
    }

    /// <summary>
    /// Returns the index for the _edits internal buffer,
    /// - position where the next IUndoableEdit item will be added by <see cref="AddEdit(IUndoableEdit)"/>.
    /// </summary>
    protected int NextAdd
    {
        get { return _nIndexOfNextAdd; }
    }
    #endregion // Properties

    #region Methods

    #region Public Methods
    #endregion // Public Methods

    #region Protected Methods

    /// <summary>
    /// If the current count of edits in the buffer, represented by <see cref="EditsCount"/>
    /// is bigger than <see cref="Limit"/>, removes from the undo buffer all superfluous edit items.
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
    /// Get rid of edits between nFrom to nTo ( including )
    /// </summary>
    /// <param name="nFrom">The index of first deleted edit item.</param>
    /// <param name="nTo">The index of last deleted edit item.</param>
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
    /// Returns the first 'significant' <see cref="IUndoableEdit"/> edit item
    /// (counting in the backward order), to which Undo should be performed
    /// if <see cref="ICompoundEdit.IsOpenMultiMode"/> is still set to true; i.e. if <see cref="ICompoundEdit.EndMultiMode"/> 
    /// has not been called yet.<br/>
    /// For better understanding, see the implementation of <see cref="Undo"/>.
    /// </summary>
    /// <returns>Returns found <see cref="IUndoableEdit"/> or null if no such item can be found.</returns>
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
    /// Returns the first 'significant' <see cref="IUndoableEdit"/> edit item
    /// to which Redo operation should be performed
    /// if <see cref="ICompoundEdit.IsOpenMultiMode"/> is still set to true; i.e. if <see cref="ICompoundEdit.EndMultiMode"/> 
    /// has not been called yet.<br/>
    /// For better understanding, see the implementation of <see cref="Redo"/>.
    /// </summary>
    /// <returns>Returns found <see cref="IUndoableEdit"/> or null if no such item can be found.</returns>
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

    /// <summary> Undo to the given IUndoableEdit edit (including) </summary>
    ///
    /// <param name="editTo"> The <see cref="IUndoableEdit"/> item to which the undoing engine should perform Undo
    /// ( in backward order). </param>
    protected void UndoTo(IUndoableEdit editTo)
    {
        editTo.CheckNotDisposed(nameof(editTo));
        Debug.Assert(0 <= _edits.IndexOf(editTo), "The buffer _edits must contain this IUndoableEdit");

        for (int nDex = NextAdd - 1; nDex >= 0; nDex--)
        {
            IUndoableEdit edit = _edits[_nIndexOfNextAdd = nDex];
            edit.Undo();
            if (edit == editTo) break;
        }
    }

    /// <summary>
    /// Redo to given IUndoableEdit edit (including)
    /// </summary>
    /// <param name="editTo"> The <see cref="IUndoableEdit"/> item to which the redoing engine should perform Redo </param>
    protected void RedoTo(IUndoableEdit editTo)
    {
        editTo.CheckNotDisposed(nameof(editTo));
        if (!_edits.Contains(editTo)) throw new ArgumentException("The buffer _edits must contain this IUndoableEdit", nameof(editTo));

        for (; NextAdd < EditsCount;)
        {
            IUndoableEdit edit = _edits[_nIndexOfNextAdd++];
            edit.Redo();
            if (edit == editTo) break;
        }
    }
    #endregion // Protected Methods

    #region Private Methods

    #endregion // Private Methods

    private static void CheckPositive(int nLimitVal, string argName)
    {
        if (nLimitVal <= 0)
        {
            string strErr = string.Format(CultureInfo.InvariantCulture,
              "Invalid value of {0} = {1}. The value has to be positive.", argName, nLimitVal);
            throw new ArgumentException(strErr, argName);
        }
    }
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
    /// Performs the Undo operation.<br/>
    /// Overrides the base class implementation, in order specific behaviour for UndoManager.
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
    /// Performs the Redo operation.<br/>
    /// Overrides the base class implementation, in order specific behaviour for UndoManager.
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
    /// Overwrites the implementation of the base class, which cleans all the Undo and Redo information that is
    /// currently held by this object.
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

#if DEBUG
    /// <summary>
    /// Returns a string describing all values of fields of this instance.
    /// </summary>
    public override string Say
    {
        get
        {

            string strRes;
            StringBuilder sbEdits = new();

            for (int ii = 0; ii < NextAdd; ii++)
            {
                if (ii == 0)
                    sbEdits.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", Environment.NewLine, _edits[ii].Say);
                else
                    sbEdits.AppendFormat(CultureInfo.InvariantCulture, ",{0}{1}", Environment.NewLine, _edits[ii].Say);
            }

            strRes = string.Format(
                CultureInfo.InvariantCulture,
                "UndoManager: (_status={0}, _nIndexOfNextAdd={1}, _limit={2}, _Edits={3})",
                _status,
                _nIndexOfNextAdd,
                _limit,
                sbEdits.ToString());

            return strRes;
        }
    }
#endif
    #endregion // IUndoableEdit Members

    /// <summary>
    /// Adds a single IUndoableEdit item into the current undo recording action.
    /// </summary>
    /// <param name="e">The <see cref="IUndoableEdit "/> item being added.</param>
    /// <returns>True on success, false on failure.</returns>
    public override bool AddEdit(IUndoableEdit e)
    {
        bool retVal = false;

        lock (_syncRoot)
        {
            if (this.Status == MgrStatus.Listening)
            {
                // remove edits from here to end of undo queue.
                this.TrimEdits(NextAdd, EditsCount - 1);
                // now we can't redo
                retVal = base.AddEdit(e);
                if (IsOpenMultiMode)
                {
                    retVal = true;
                }
                //update position
                this._nIndexOfNextAdd = EditsCount;
                // cut beginning to make sure undo queue is not too big
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

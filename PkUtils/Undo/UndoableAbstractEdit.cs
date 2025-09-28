
// Define UNDO_ACTIVATION_SUPPORT if you want to support activation/deactivation of compound undo operations 
// (e.g., tracking whether an edit is still in progress).
#define UNDO_ACTIVATION_SUPPORT

// Ignore Spelling: Utils, Undoable
//
using System;
using System.Globalization;

namespace PK.PkUtils.Undo;

/// <summary>
/// The abstract class implementing most of the functionality of <see cref="IUndoableEdit"/>.
/// Provides base logic for undo/redo operations, disposal, and presentation naming.
/// </summary>
[CLSCompliant(true)]
public abstract class UndoableAbstractEdit : IUndoableEdit
{
    #region Fields
    /// <summary>
    /// Indicates whether this object has been disposed.
    /// </summary>
    protected bool _bDisposed;

    /// <summary>
    /// Indicates whether the edit operation has been performed.
    /// </summary>
    protected bool _bHasBeenDone = true;

    /// <summary>
    /// The base name for the Undo operation.
    /// </summary>
    protected const string _strUndoName = "Undo";

    /// <summary>
    /// The base name for the Redo operation.
    /// </summary>
    protected const string _strRedoName = "Redo";

    /// <summary>
    /// The default presentation name for undo/redo operations.
    /// </summary>
    private static readonly string _strDefaultPresentationName = string.Empty;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Initializes a new instance of the <see cref="UndoableAbstractEdit"/> class.
    /// </summary>
    protected UndoableAbstractEdit()
    {
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Gets the base name of the Undo operation, used as a leading prefix in <see cref="UndoPresentationName"/>.
    /// </summary>
    protected static string BaseUndoName
    {
        get { return _strUndoName; }
    }

    /// <summary>
    /// Gets the base name of the Redo operation, used as a leading prefix in <see cref="RedoPresentationName"/>.
    /// </summary>
    protected static string BaseRedoName
    {
        get { return _strRedoName; }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="UndoableAbstractEdit"/> is still usable.
    /// </summary>
    /// <value>True if this object has not been disposed; otherwise, false.</value>
    protected bool IsAlive
    {
        get { return !IsDisposed; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the editing step recorded by this object has been performed.
    /// Becomes false after <see cref="Undo"/> is called, and true after construction or <see cref="Redo"/>.
    /// </summary>
    /// <value>True if the operation has been performed; otherwise, false.</value>
    protected bool HasBeenDone
    {
        get { return _bHasBeenDone; }
        set { _bHasBeenDone = value; }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="UndoableAbstractEdit"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// True to release both managed and unmanaged resources; false to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.IsDisposed)
        {
            // If disposing equals true, dispose both managed and unmanaged resources.
            if (disposing)
            {
                // No managed resources to dispose in this class.
            }
            // No unmanaged resources to release in this class.

            // Note: Not thread safe. Thread safety must be implemented by the client if needed.
            _bDisposed = true;
        }
    }
    #endregion // Methods

    #region IUndoableEdit Members
    #region IUndoable Members

    /// <inheritdoc/>
    public virtual bool CanUndo
    {
        get { return (IsAlive && HasBeenDone); }
    }

    /// <inheritdoc/>
    public virtual bool CanRedo
    {
        get { return (IsAlive && !HasBeenDone); }
    }

    /// <inheritdoc/>
    public virtual void Undo()
    {
        if (!CanUndo)
        {
            throw new InvalidOperationException("Cannot undo: either the edit is disposed or has not been done.");
        }
        if (!IsActive)
        {
            HasBeenDone = false;
        }
    }

    /// <inheritdoc/>
    public virtual void Redo()
    {
        if (!CanRedo)
        {
            throw new InvalidOperationException("Cannot redo: either the edit is disposed or has already been done.");
        }
        if (!IsActive)
        {
            HasBeenDone = true;
        }
    }

    /// <summary>
    /// Removes all undo and redo information currently held by this object.
    /// </summary>
    /// <remarks>
    /// This method must be implemented by derived classes to clear their undo/redo state.
    /// </remarks>
    public abstract void EmptyUndoBuffer();
    #endregion // IUndoable Members

    #region IDisposableEx Members
    #region IDisposable Members

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        // Prevent finalization code from executing a second time.
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposable Members

    /// <inheritdoc/>
    public bool IsDisposed
    {
        get { return _bDisposed; }
    }
    #endregion // IDisposableEx Members

    /// <inheritdoc/>
    public virtual bool Significant
    {
        get { return true; }
    }

    /// <inheritdoc/>
    public virtual string PresentationName
    {
        get { return _strDefaultPresentationName; }
    }

    /// <inheritdoc/>
    public virtual string UndoPresentationName
    {
        get
        {
            string result = BaseUndoName;

            if (PresentationName != _strDefaultPresentationName)
            {
                result += (" " + PresentationName);
            }
            return result;
        }
    }

    /// <inheritdoc/>
    public virtual string RedoPresentationName
    {
        get
        {
            string result = BaseRedoName;

            if (PresentationName != _strDefaultPresentationName)
            {
                result += (" " + PresentationName);
            }
            return result;
        }
    }

    /// <summary>
    /// Gets a string describing the values of all fields of this instance.
    /// </summary>
    public virtual string Say
    {
        get
        {
            string strRes = string.Format(
                CultureInfo.InvariantCulture,
                "AbstractUndoableEdit: (_bDisposed={0}, _bHasBeenDone={1})",
                _bDisposed,
                _bHasBeenDone);

            return strRes;
        }
    }

#if UNDO_ACTIVATION_SUPPORT
    /// <inheritdoc/>
    public virtual bool IsActive
    {
        get { return false; }
    }

    /// <inheritdoc/>
    public virtual bool CanUndoWhileActive
    {
        get { return false; }
    }

    /// <inheritdoc/>
    public virtual bool CanRedoWhileActive
    {
        get { return false; }
    }

    /// <inheritdoc/>
    public virtual bool Deactivate()
    {
        return true;
    }
#endif // UNDO_ACTIVATION_SUPPORT
    #endregion // IUndoableEdit Members
}

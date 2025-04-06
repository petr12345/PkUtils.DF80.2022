/***************************************************************************************************************
*
* FILE NAME:   .\Undo\IUndoableEdit.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class UndoableAbstractEdit
*
**************************************************************************************************************/

#define UNDO_ACTIVATION_SUPPORT

// Ignore Spelling: Utils, Undoable
//
using System;
using System.Globalization;

namespace PK.PkUtils.Undo;

/// <summary>
/// The abstract class implementing some (actually most) of the functionality
/// of IUndoableEdit.
/// </summary>
[CLSCompliant(true)]
public abstract class UndoableAbstractEdit : IUndoableEdit
{
    #region Fields
    /// <summary>
    /// Backing field for the property <see cref="IsDisposed"/>
    /// </summary>
    protected bool _bDisposed;

    /// <summary>
    /// Backing field for the property <see cref="HasBeenDone"/>
    /// </summary>
    protected bool _bHasBeenDone = true;

    /// <summary>
    /// Backing field for the property <see cref="BaseUndoName"/>
    /// </summary>
    protected const string _strUndoName = "Undo";

    /// <summary>
    /// Backing field for the property <see cref="BaseRedoName"/>
    /// </summary>
    protected const string _strRedoName = "Redo";

    private static readonly string _strDefaultPresentationName = string.Empty;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// The default argument-less constructor. Should be protected in an abstract class.
    /// </summary>
    protected UndoableAbstractEdit()
    {
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// A base name of the Undo operation, used as a leading prefix
    /// in <see cref="UndoPresentationName"/> property implementation.
    /// </summary>
    protected static string BaseUndoName
    {
        get { return _strUndoName; }
    }

    /// <summary>
    /// A base name of the Redo operation, used as a leading prefix
    /// in <see cref="RedoPresentationName"/> property implementation.
    /// </summary>
    protected static string BaseRedoName
    {
        get { return _strRedoName; }
    }

    /// <summary> The indicator whether this UndoableAbstractEdit is still usable. </summary>
    ///
    /// <value> True if this has not been disposed, false otherwise. </value>
    protected bool IsAlive
    {
        get { return !IsDisposed; }
    }

    /// <summary>
    /// The indicator whether the editing step that is recorder by this object has been actually done or not.<br/>
    /// Becomes false after the <see cref="Undo"/> operation is completed. <br/>
    /// Becomes true after constructing this UndoableAbstractEdit, and after the <see cref="Redo"/> operation is
    /// completed. <br/>
    /// </summary>
    ///
    /// <value> true if this object has been done, false if not. </value>
    protected bool HasBeenDone
    {
        get { return _bHasBeenDone; }
        set { _bHasBeenDone = value; }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios.
    /// If disposing equals true, the method has been called directly
    /// or indirectly by a user's code. Managed and unmanaged resources
    /// can be disposed.
    /// If disposing equals false, the method has been called by the 
    /// runtime from inside the finalizer and you should not reference 
    /// other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. 
    /// Otherwise it is called by finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (!this.IsDisposed)
        {
            // If disposing equals true, dispose both managed and unmanaged resources.
            if (disposing)
            {
            }
            // Now release unmanaged resources. If disposing is false, 
            // only that code is executed.
            // Actually nothing to do here for this particular class

            ////////////////////////////////////////////////////////////////////
            // Note that this is not thread safe.
            // Another thread could start disposing the object after the managed resources are disposed,
            // but before the _disposed flag is set to true.
            // If thread safety is necessary, it must be implemented by the client ( calling code ).

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
            throw new InvalidOperationException();
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
            throw new InvalidOperationException();
        }
        if (!IsActive)
        {
            HasBeenDone = true;
        }
    }

    /// <summary>
    ///  Cleans all the Undo and Redo information that is currently held by this object.
    /// </summary>
    /// <remarks> An abstract method inherited from <see cref="IUndoableEdit"/></remarks>
    public abstract void EmptyUndoBuffer();
    #endregion // IUndoable Members

    #region IDisposableEx Members
    #region IDisposable Members

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        // Take yourself off the Finalization queue to prevent finalization code 
        // for this object from executing a second time.
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposable Members

    /// <inheritdoc/>
    public bool IsDisposed
    {
        get { return _bDisposed; }
    }
    #endregion // IDisposableEx Members

    /// <summary>Is this edit item significant enough for shaping individual Undo operation, 
    /// with related menu text.</summary>
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

#if UNDO_ACTIVATION_SUPPORT
    /// <summary>
    /// IsActive return true if the user has not finished editing yet
    /// </summary>
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

    /// <summary>
    /// Should return true if successfully deactivated, or if no deactivation was needed.
    /// Return false for unsuccessfully deactivated (if the edit had to stay active ).
    /// </summary>
    /// <returns>True on success, false on failure.</returns>
    public virtual bool Deactivate()
    {
        return true;
    }
#endif // UNDO_ACTIVATION_SUPPORT

#if DEBUG
    /// <summary>
    /// Returns a string describing all values of fields of this instance.
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
#endif
    #endregion // IUndoableEdit Members
}

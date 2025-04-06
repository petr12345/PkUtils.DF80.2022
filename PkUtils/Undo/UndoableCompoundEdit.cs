/***************************************************************************************************************
*
* FILE NAME:   .\Undo\UndoableCompoundEdit.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class UndoableCompoundEdit
*
**************************************************************************************************************/

#define UNDO_ACTIVATION_SUPPORT

// Ignore Spelling: Utils, Undoable
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PK.PkUtils.Undo;

/// <summary>
/// UndoableCompoundEdit is UndoableAbstractEdit consisting of more child IUndoableEdit items.
/// </summary>
[CLSCompliant(true)]
public class UndoableCompoundEdit : UndoableAbstractEdit, ICompoundEdit
{
    #region Fields
    /// <summary>
    /// A backing filed for the <see cref="IsOpenMultiMode"/> property.
    /// </summary>
    protected bool _bMultiMode = true;

    /// <summary>
    /// A buffer of child <see cref="IUndoableEdit"/> items, new child edits are added by <see cref="AddEdit"/>
    /// </summary>
    protected List<IUndoableEdit> _edits = [];
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// The default argument-less constructor.
    /// </summary>
    public UndoableCompoundEdit()
    {
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Nomen est omen.
    /// </summary>
    public IUndoableEdit LastEdit
    {
        get
        {
            return _edits.LastOrDefault();
        }
    }
    #endregion // Properties

    #region Methods

    /// <summary> Returns the last edit if that one is active. </summary>
    ///
    /// <returns>
    /// The last <see cref="IUndoableEdit"/> if its property
    /// <see cref="PK.PkUtils.Undo.IUndoableEdit.IsActive"/>
    /// returns true; otherwise null.
    /// </returns>
    protected virtual IUndoableEdit GetActiveEdit()
    {
        IUndoableEdit temp;
        IUndoableEdit result = null;

        if (null != (temp = LastEdit))
        {
            if (temp.IsActive)
            {
                result = temp;
            }
        }
        return result;
    }

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
    protected override void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (!this.IsDisposed)
        {
            // If disposing equals true, dispose both managed and unmanaged resources.
            if (disposing)
            {
                EmptyUndoBuffer();
            }
            // Now release unmanaged resources. Actually nothing to do here.

            // Call the base class implementation
            base.Dispose(disposing);
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
            bool bRes;

            if (IsActive)
            {
                bRes = CanUndoWhileActive;
            }
            else if (IsOpenMultiMode)
            {
                bRes = false;
            }
            else
            {
                bRes = base.CanUndo;
            }

            return bRes;
        }
    }

    /// <inheritdoc/>
    public override bool CanRedo
    {
        get
        {
            bool bRes;

            if (IsActive)
            {
                bRes = CanRedoWhileActive;
            }
            else if (IsOpenMultiMode)
            {
                bRes = false;
            }
            else
            {
                bRes = base.CanRedo;
            }

            return bRes;
        }
    }

    /// <summary>
    /// Nomen est omen
    /// </summary>
    public override void Undo()
    {
        base.Undo();

        if (IsActive)
        {
            IUndoableEdit temp = GetActiveEdit();
            Debug.Assert(temp != null);
            temp.Undo();
        }
        else
        {
            //send undo to all edits in order reversed from the way they were added
            for (int ii = _edits.Count - 1; ii >= 0; ii--)
            {
                _edits[ii].Undo();
            }
        }
    }

    /// <summary>
    /// Nomen est omen
    /// </summary>
    public override void Redo()
    {
        base.Redo();

        if (IsActive)
        {
            IUndoableEdit temp = GetActiveEdit();
            Debug.Assert(temp != null);
            temp.Redo();
        }
        else
        {
            // send redo to all edits in the same order as they were added 
            // ( which is reversed order from the way they were undone )
            for (int ii = 0; ii < _edits.Count; ii++)
            {
                _edits[ii].Redo();
            }
        }
    }

    /// <summary>
    /// Overwrites the implementation of the base class, which cleans all the Undo and Redo information that is currently held by this object.<br/>
    /// This implementation Disposes all child edits in order reversed from the way they were added
    /// </summary>
    public override void EmptyUndoBuffer()
    {
        Deactivate();
        for (int ii = _edits.Count - 1; ii >= 0; ii--)
        {
            _edits[ii].Dispose();
        }
        _edits.Clear();
    }
    #endregion // IUndoable Members

    /// <inheritdoc/>
    public override bool Significant
    {
        get
        {
            return (null != _edits.First(edit => edit.Significant));
        }
    }

    /// <inheritdoc/>
    public override string PresentationName
    {
        get
        {
            IUndoableEdit last;
            string result;

            if (null != (last = LastEdit))
                result = last.PresentationName;
            else
                result = base.PresentationName;

            return result;
        }
    }

    /// <inheritdoc/>
    public override string UndoPresentationName
    {
        get
        {
            IUndoableEdit last;
            string result;

            if (null != (last = LastEdit))
                result = last.UndoPresentationName;
            else
                result = base.UndoPresentationName;

            return result;
        }
    }

    /// <inheritdoc/>
    public override string RedoPresentationName
    {
        get
        {
            IUndoableEdit last;
            string result;

            if (null != (last = LastEdit))
                result = last.RedoPresentationName;
            else
                result = base.RedoPresentationName;

            return result;
        }
    }

#if UNDO_ACTIVATION_SUPPORT

    /// <summary>
    /// Nomen est omen
    /// </summary>
    public override bool IsActive
    {
        get
        {
            IUndoableEdit temp;
            bool bRes = false;

            if (null != (temp = GetActiveEdit()))
            {
                Debug.Assert(temp.IsActive);
                bRes = temp.IsActive;
            }
            return bRes;
        }
    }

    /// <summary>
    /// Nomen est omen
    /// </summary>
    public override bool CanUndoWhileActive
    {
        get
        {
            IUndoableEdit temp;
            bool bRes = false;

            if (IsActive)
            {
                if (null != (temp = GetActiveEdit()))
                    bRes = temp.CanUndoWhileActive;
                else
                    Debug.Fail("Unable to find an active IUndoableEdit");
            }
            return bRes;
        }
    }

    /// <summary>
    /// Nomen est omen
    /// </summary>
    public override bool CanRedoWhileActive
    {
        get
        {
            IUndoableEdit temp;
            bool bRes = false;

            if (IsActive)
            {
                if (null != (temp = GetActiveEdit()))
                    bRes = temp.CanRedoWhileActive;
                else
                    Debug.Fail("Unable to find an active IUndoableEdit");
            }
            return bRes;
        }
    }

    /// <summary>
    /// Returns true if successfully deactivated, or if no deactivation was needed.<br/>
    /// Return false for unsuccessfully deactivation (if the edit had to stay active ).
    /// </summary>
    /// <returns>True on success, false on failure.</returns>
    public override bool Deactivate()
    {
        IUndoableEdit temp;
        bool bOk = true;

        if (IsActive)
        {
            if (null != (temp = GetActiveEdit()))
            {
                bOk = temp.Deactivate();
                /* no !! EndMultiMode(); */
            }
            else
            {
                Debug.Fail("Unable to find an active IUndoableEdit");
            }
        }
        return bOk;
    }
#endif // UNDO_ACTIVATION_SUPPORT

    #endregion // IUndoableEdit Members

    /// <summary>
    /// Implements <see cref="ICompoundEdit.IsOpenMultiMode"/> which indicates whether the multi-mode still in progress.<br/>
    /// Being "in progress" for UndoableCompoundEdit means that undo recording action is not closed yet.<br/>
    /// For better understanding see code of AddEdit method, or the 'EndMultiMode' method. <br/>
    /// </summary>
    /// <seealso cref="ICompoundEdit.IsOpenMultiMode"/>
    public virtual bool IsOpenMultiMode
    {
        get { return _bMultiMode; }
    }

    /// <summary>
    /// Adds a single IUndoableEdit item into the current undo recording action.
    /// </summary>
    /// <param name="e">The <see cref="IUndoableEdit "/> item being added.</param>
    /// <returns>True on success, false on failure.</returns>
    public virtual bool AddEdit(IUndoableEdit e)
    {
        bool bRes = false;

        if (IsOpenMultiMode)
        {
            ICompoundEdit iCompound;
            IUndoableEdit last;

            if ((null != (last = LastEdit)) && (last.IsActive))
            {
                // This should not happen, or at least is not resolved yet.
                // When the last edit is active, either the new edit should not be added at all,
                // or the last edit should be deactivated first.
                Debug.Fail("Unable to perform AddEdit with the last edit still active");
                throw new InvalidOperationException();
            }
            if (null != (iCompound = last as ICompoundEdit))
            {
                bRes = iCompound.AddEdit(e);
            }
            else
            {
                _edits.Add(e);
                bRes = true;
            }
        }
        return bRes;
    }

    /// <inheritdoc/>
    public virtual void EndMultiMode()
    {
        _bMultiMode = false;
    }
    #endregion // ICompoundEdit Members
}

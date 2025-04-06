/***************************************************************************************************************
*
* FILE NAME:   .\UI\Utils\WaitCursor.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class WaitCursor
*
**************************************************************************************************************/

// Ignore Spelling: Utils
// 
using System;
using System.Windows.Forms;
using PK.PkUtils.DataStructures;

namespace PK.PkUtils.UI.Utils;

/// <summary> The purpose of the class WaitCursor is more easy setting the wait cursor, and later restoring
/// the original cursor along the long operation. <br/>
/// The method Dispose will restore the original cursor;
/// you can use the 'using' statement to ensure that the cursor is restored regardless on any
/// exception thrown etc. <br/>
///  Your code may look like this:
/// <code>
///   using (new WaitCursor(this))
///   {
///     DoLongOperation();
///   }
///   </code> </summary>
public class WaitCursor : CountableGeneric<WaitCursor>
{
    #region Fields

    /// <summary> The control to which the wait cursor is applied. </summary>
    protected Control _targetControl;

    /// <summary> The previous cursor that should be restored after wait cursor disappears. </summary>
    protected Cursor _oldCursor;
    #endregion // Fields

    #region Constructor(s)
    /// <summary>
    /// Public constructor. Applies the wait cursor
    /// </summary>
    public WaitCursor()
      : this(null)
    { }

    /// <summary>
    /// Public constructor. Applies the wait cursor to given control
    /// </summary>
    /// <param name="control">The destination control for which the  wait cursor is applied. May be null.</param>
    public WaitCursor(Control control)
    {
        ApplyCursor(control);
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Get the amount of active wait cursors ( if there is more of them in the call stack )
    /// </summary>
    public static int ActiveWaitCursors
    {
        get { return typeof(WaitCursor).GetCountIncludingDescendants(); }
    }

    /// <summary> Gets the target control. </summary>
    protected Control TargetControl { get => _targetControl; }

    /// <summary> Gets the old cursor ( before modification to wait). </summary>
    protected Cursor OldCursor { get => _oldCursor; }
    #endregion // Properties

    #region Methods

    #region Public Methods

    // PetrK Sept 05, 2013: 
    // Following is not needed any more, as there is similar generic method in static class Disposer.
    /*
    /// <summary> Calls dispose and assigns null. </summary>
    ///
    /// <param name="wc"> A disposed object variable. Its value can be null, in that case nothing is
    /// disposed; otherwise the object is disposed and the variable value is assigned to null. </param>
    public static void Dispose(ref WaitCursor wc)
    {
      if (null != wc)
      {
        wc.Dispose();
        wc = null;
      }
    }
    */
    #endregion // Public Methods

    #region Protected Methods

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
        if (!IsDisposed)
        {
            // If disposing equals true, dispose both managed and unmanaged resources.
            if (disposing)
            {   // Dispose managed resources here
                RestoreCursor();
            }
            // Now release unmanaged resources. For this class, actually nothing to do here ..
            // 
            // And call the base...
            base.Dispose(disposing);
        }
    }
    #endregion // Protected Methods

    #region Private Methods

    /// <summary>
    /// Assignes control cursor in thread-safe manner, preventing System.InvalidOperationException "Cross-thread
    /// operation not valid".
    /// </summary>
    /// <param name="targetControl"> The control to which the wait cursor is applied. </param>
    /// <param name="cursor"> The cursor to be assigned. </param>
    /// <returns> The previous cursor in control. </returns>
    private static Cursor SetControlCursor(Control targetControl, Cursor cursor)
    {
        Cursor previous = targetControl.Cursor;

        if (targetControl.InvokeRequired)
        {
            targetControl.Invoke(new Action(() => targetControl.Cursor = cursor));
        }
        else
        {
            targetControl.Cursor = cursor;
        }
        return previous;
    }

    /// <summary>
    /// Called by the constructor. 
    /// Applies the cursor on a given control
    /// ( changes the cursor to wait cursor) if the usage counter is exactly one.
    /// </summary>
    /// <param name="destControl"></param>
    private void ApplyCursor(Control destControl)
    {
        if (1 == CountIncludingDescendants)
        {   // it is the first one
            if (OldCursor == null)
            {
                _targetControl = destControl;

                if (TargetControl != null)
                {
                    _oldCursor = SetControlCursor(TargetControl, Cursors.WaitCursor);
                }
                else
                {
                    _oldCursor = Cursor.Current;
                    Cursor.Current = Cursors.WaitCursor;
                }
            }
        }
        else
        {   // should I do anything at all ? Just keep displaying wait cursor...
        }
    }

    /// <summary>
    /// Called by the Dispose method. 
    /// Restores the previous cursor if usage counter becomes zero.
    /// </summary>
    private void RestoreCursor()
    {
        if (1 == CountIncludingDescendants)
        {
            if (OldCursor != null)
            {
                if (TargetControl != null)
                {
                    SetControlCursor(TargetControl, OldCursor);
                    _targetControl = null;
                }
                else
                {
                    // PetrK 12/17/2013: Assigning the old cursor might be quite inappropriate at this moment.
                    // The code should assign Cursor.Current = Cursors.Default;
                    // For more info, see for instance 
                    // http://stackoverflow.com/questions/1568557/how-can-i-make-the-cursor-turn-to-the-wait-cursor
                    // http://jktarun.blogspot.com/2012/10/change-cursor-to-wait-cursor.html

                    /* Cursor.Current = _oldCursor; */
                    Cursor.Current = Cursors.Default;   // Reset the cursor to the default for all controls.
                }
                _oldCursor = null;
            }
        }
    }
    #endregion // Private Methods
    #endregion // Methods
}
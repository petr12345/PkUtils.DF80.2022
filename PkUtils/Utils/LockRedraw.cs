/***************************************************************************************************************
*
* FILE NAME:   .\Utils\LockRedraw.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains the class LockRedraw.
*
**************************************************************************************************************/


// Ignore Spelling: Utils

using System;
using System.Windows.Forms;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.Utils;

/// <summary>
/// LockRedraw locks ( prevents ) a window from redrawing. <br/>
/// 
/// Usage example.  <br/>
/// Assuming you have declared a field of type LockRedraw in your class, 
/// you can use it in following ways:
/// <code>
/// class CAnyClass
/// {
///   protected LockRedraw _rlock = new LockRedraw();
///
///   void SomeMyMethod()
///   {
///     try
///     {
///       _rlock.Lock();
///       //  Do something.
///     }
///     finally
///     { //  should unlock redrawing regardless any exception thrown
///       _rlock.Unlock();
///     }
///   }
///
///   void SomeOtherMethod()
///   {
///     using (var lockUser = new UsageCounterWrapper(_rlock))
///     {
///       //  Do something.
///       //  The 'using' statement makes sure that dispose of lockUser is called;
///       //  which in turn will unlock the _rlock instance.
///     }
///   }
/// }
/// </code>
/// </summary>
/// 
/// <seealso href="http://stackoverflow.com/questions/3590773/c-sharp-lock-drawing">
/// Stackoverflow:  C# - Lock drawing</seealso>
/// 
/// <seealso href="http://stackoverflow.com/questions/192413/how-do-you-prevent-a-richtextbox-from-refreshing-its-display">
/// Stackoverflow:  How do you prevent a RichTextBox from refreshing its display?</seealso>
///
/// <seealso cref="UsageCounterWrapper"/>
[CLSCompliant(true)]
public class LockRedraw : UsageCounter
{
    #region Fields

    /// <summary>
    /// Handle of the window that is temporarily prevented from redrawing.
    /// </summary>
    protected readonly IntPtr _handle;
    private IntPtr _eventMask;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Initializes a new instance of the <see cref="LockRedraw"/> class for a given window handle.
    /// </summary>
    /// <remarks>The class does NOT become an owner of the provided window handle.</remarks>
    /// <param name="handle">Handle of the window to lock redrawing for.</param>
    public LockRedraw(IntPtr handle)
        : this(handle, shouldLock: false)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LockRedraw"/> class for a given window handle,
    /// with an option to lock immediately.
    /// </summary>
    /// <remarks>The class does NOT become an owner of the provided window handle.</remarks>
    /// <param name="handle">Handle of the window to lock redrawing for.</param>
    /// <param name="shouldLock">If true, locking will be performed during construction.</param>
    public LockRedraw(IntPtr handle, bool shouldLock)
        : base()
    {
        _handle = handle;
        if (shouldLock)
        {
            this.AddReference();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LockRedraw"/> class for a given WinForms control,
    /// with an option to lock immediately.
    /// </summary>
    /// <param name="control">The WinForms control to lock redrawing for.</param>
    /// <param name="shouldLock">If true, locking will be performed during construction.</param>
    public LockRedraw(Control control, bool shouldLock)
        : this(control?.Handle ?? throw new ArgumentNullException(nameof(control)), shouldLock)
    { }

    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// The window handle provided to the constructor.
    /// </summary>
    protected IntPtr Handle => _handle;

    #endregion // Properties

    #region Methods

    #region Protected Methods

    /// <summary>
    /// Overrides the method of the base class. Calls StopRepaint.
    /// </summary>
    protected override void OnFirstAddReference()
    {
        StopRepaint();
    }

    /// <summary>
    /// Overrides the method of the base class. Calls StartRepaint.
    /// </summary>
    protected override void OnLastRelease()
    {
        StartRepaint();
    }

    /// <summary>
    /// Prevents window redrawing by sending WM_SETREDRAW with wParam = IntPtr.Zero.
    /// </summary>
    protected virtual void StopRepaint()
    {
        // Stop redrawing
        User32.SendMessage(this.Handle, (int)Win32.WM.WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        // Stop sending of events
        _eventMask = User32.SendMessage(this.Handle, (int)Win32.RichEm.EM_GETEVENTMASK, IntPtr.Zero, IntPtr.Zero);
    }

    /// <summary>
    /// Enables window redrawing by sending WM_SETREDRAW with wParam = IntPtr(1).
    /// </summary>
    protected virtual void StartRepaint()
    {
        // Restore event mask
        User32.SendMessage(this.Handle, (int)Win32.RichEm.EM_SETEVENTMASK, IntPtr.Zero, _eventMask);
        // Resume redrawing
        User32.SendMessage(this.Handle, (int)Win32.WM.WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
    }
    #endregion // Protected Methods
    #endregion // Methods
}

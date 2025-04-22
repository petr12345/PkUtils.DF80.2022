// Ignore Spelling: Utils, Stackoverflow
//

using System;
using System.Windows.Forms;
using PK.PkUtils.Utils;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.UI.Utils;

/// <summary>
/// LockRedraw locks ( prevents ) a window from redrawing. <br/>
/// 
/// Usage example.  <br/>
/// Assuming you have declared a field of type LockRedraw in your class, 
/// you can use it in following ways:
/// <code>
/// class CAnyForm : Form
/// {
///    private LockRedraw _lockRedraw;
///    private TreeView _treeView;
///
///    protected override void OnLoad(EventArgs e)
///    {
///        // LockRedraw should only be created after the associated control handle is guaranteed to be created
///        _lockRedraw = new LockRedraw(_treeView, shouldLock: false);
///        base.OnLoad(e);
///    }
///    protected void SomeMyMethod()
///    {
///        // Constructor of UsageCounterWrapper causes the call of _lockRedraw.StopRedrawing();
///        // unless it has already been called somewhere higher in the call stack.
///        using (IDisposable _ = new UsageCounterWrapper(_lockRedraw))
///        {
///            // Do some long operation here involving many changes to _treeView,
///            // while _lockRedraw prevents redrawing.
///            //
///            // The following Dispose in turn calls _lockRedraw.StartRedrawing, if no one else locked it.
///        }
///    }
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
    /// Overrides the method of the base class. Calls StopRedrawing.
    /// </summary>
    protected override void OnFirstAddReference()
    {
        StopRedrawing();
    }

    /// <summary>
    /// Overrides the method of the base class. Calls StartRedrawing.
    /// </summary>
    protected override void OnLastRelease()
    {
        StartRedrawing();
    }

    /// <summary>
    /// Prevents window redrawing by sending WM_SETREDRAW with wParam = IntPtr.Zero.
    /// </summary>
    protected virtual void StopRedrawing()
    {
        // Stop redrawing
        User32.SendMessage(this.Handle, (int)Win32.WM.WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        // Stop sending of events
        _eventMask = User32.SendMessage(this.Handle, (int)Win32.RichEm.EM_GETEVENTMASK, IntPtr.Zero, IntPtr.Zero);
    }

    /// <summary>
    /// Enables window redrawing by sending WM_SETREDRAW with wParam = IntPtr(1).
    /// </summary>
    protected virtual void StartRedrawing()
    {
        // Restore event mask
        User32.SendMessage(this.Handle, (int)Win32.RichEm.EM_SETEVENTMASK, IntPtr.Zero, _eventMask);
        // Resume redrawing
        User32.SendMessage(this.Handle, (int)Win32.WM.WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
    }
    #endregion // Protected Methods
    #endregion // Methods
}

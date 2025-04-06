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
    /// The handle of window that we temporarily prevent redrawing.
    /// </summary>
    protected readonly IntPtr _hwnd;
    private IntPtr _eventMask;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Constructor providing the window locked from redrawing.
    /// </summary>
    /// <remarks>The class does NOT become an owner of the provided window</remarks>
    /// <param name="hwnd">The handle of the window to be locked from redrawing.</param>
    public LockRedraw(IntPtr hwnd)
      : this(hwnd, false)
    {
    }

    /// <summary>
    /// Constructor providing the window locked from redrawing, 
    /// and the boolean that specifies whether the lock should be called by constructor.
    /// </summary>
    /// <remarks>The class does NOT become an owner of the provided window</remarks>
    /// <param name="hwnd">The handle of the window to be locked from redrawing.</param>
    /// <param name="bLock">Should the contructor perform the lock.</param>
    public LockRedraw(IntPtr hwnd, bool bLock)
      : base()
    {
        _hwnd = hwnd;
        if (bLock)
        {
            this.AddReference();
        }
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// The window handle provided to constructor
    /// </summary>
    protected IntPtr Handle
    {
        get { return _hwnd; }
    }
    #endregion // Properties

    #region Methods

    #region Public Methods
    #endregion // Public Methods

    #region Protected Methods

    /// <summary>
    /// Overrides ( implements) the method of the base class. Calls StopRepaint.
    /// </summary>
    protected override void OnFirstAddReference()
    {
        StopRepaint();
    }

    /// <summary>
    /// Overrides ( implements) the method of the base class. Calls StartRepaint.
    /// </summary>
    protected override void OnLastRelease()
    {
        StartRepaint();
    }

    /// <summary>
    /// Stops control redrawing by sending WM_SETREDRAW with wParam = IntPtr.Zero
    /// </summary>
    protected virtual void StopRepaint()
    {
        // Stop redrawing:   
        User32.SendMessage(this.Handle, (int)Win32.WM.WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        // Stop sending of events:         
        _eventMask = User32.SendMessage(this.Handle, (int)Win32.RichEm.EM_GETEVENTMASK, IntPtr.Zero, IntPtr.Zero);
    }

    /// <summary>
    /// Starts control redrawing by sending WM_SETREDRAW with wParam = IntPtr(1)
    /// </summary>
    protected virtual void StartRepaint()
    {
        // turn on events         
        User32.SendMessage(this.Handle, (int)Win32.RichEm.EM_SETEVENTMASK, IntPtr.Zero, _eventMask);
        // turn on redrawing         
        User32.SendMessage(this.Handle, (int)Win32.WM.WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
    }
    #endregion // Protected Methods
    #endregion // Methods
}

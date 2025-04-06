/***************************************************************************************************************
*
* FILE NAME:   .\SystemEx\WindowsSystemHookBase.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class WindowsSystemHookBase
*
**************************************************************************************************************/


// Ignore Spelling: Utils, Uninstall, Toub
//
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PK.PkUtils.WinApi;
using static PK.PkUtils.WinApi.Win32;


namespace PK.PkUtils.SystemEx;

/// <summary>
/// WindowsSystemHookBase works as a .NET wrapper around SetWindowsHookEx.<br/>
/// In your code you will instantiate the class, providing the hook type and a delegate callback, 
/// and call <see cref="Install()"/>.
/// Later you should call <see cref="Uninstall()"/> or <see cref="Dispose()"/> when the hook is no longer needed.<br/>
/// An example:
/// <code>
/// private bool SetMouseHook()
/// {
///   if (_mouseHook == null)
///     _mouseHook = new WindowsSystemHookBase(HookType.WH_MOUSE, new Win32.HookProc( MouseHookMessageProc ));
///   if (!_mouseHook.Installed)
///     _mouseHook.Install();
///   return _mouseHook.Installed;
/// }
/// </code>
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/windows/win32/winmsg/about-hooks">
/// Windows Hooks Overview
/// </seealso>
public class WindowsSystemHookBase : IDisposable
{
    #region Typedefs

    /// <summary>
    /// A safe windows hook handle, encapsulating a native resource - a result of SetWindowshookEx.
    /// </summary>
    ///
    /// <remarks>
    /// The purpose of SafeHandle usage here is to guarantee that related system object is released properly.
    /// At its core, SafeHandle is simply a managed wrapper around an IntPtr with a finalizer
    /// that knows how to release the underlying resource referenced by that IntPtr.
    /// Since SafeHandle derives from CriticalFinalizerObject, this finalizer is prepared when
    /// SafeHandle is instantiated, and will be called from within a CER ( Constrained Execution
    /// Region ) to ensure that asynchronous thread aborts do not interrupt the finalizer.
    /// For more details see
    /// <a href="https://docs.microsoft.com/en-us/archive/msdn-magazine/2005/october/using-the-reliability-features-of-the-net-framework">
    /// Keep Your Code Running with the Reliability Features of the .NET Framework</a>
    /// by Stephen Toub, MSDN Magazine Oct 2005.
    /// </remarks>
    protected class SafeWindowsHookHandle : SafeHandle
    {

        /// <summary> Public constructor. </summary>
        /// <param name="hookHandle" type="IntPtr"> Handle of the hook. </param>
        public SafeWindowsHookHandle(IntPtr hookHandle)
          : base(IntPtr.Zero, true)
        {
            SetHandle(hookHandle);
        }

        /// <summary>
        /// Gets a value indicating whether the handle value is invalid.
        /// </summary>
        /// <value> true if the handle value is invalid; otherwise, false. </value>
        public override bool IsInvalid
        {
            get { return (this.handle == IntPtr.Zero); }
        }

        /// <summary> Executes the code required to free the handle. </summary>
        /// <returns>
        /// true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. 
        /// In this case, it generates a releaseHandleFailed MDA Managed Debugging Assistant.
        /// </returns>
        protected override bool ReleaseHandle()
        {
            bool bRes;

            if (!IsInvalid)
            {
                bRes = (0 != User32.UnhookWindowsHookEx(handle));
                this.handle = IntPtr.Zero;
            }
            else
            {
                bRes = true;
            }
            return bRes;
        }
    }
    #endregion // Typedefs

    #region Fields

    /// <summary>
    /// The backing field of the property <see cref="HHook"/>
    /// </summary>
    protected SafeWindowsHookHandle _hookHandle;

    /// <summary>
    /// The system filter function delegate, initialized by constructor
    /// </summary>
    protected User32.HookProc _filterFunc;

    /// <summary>
    ///The type of the system hook 
    /// </summary>
    protected readonly Win32.HookType _hookType;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// The constructor accepting a hook type argument
    /// </summary>
    /// <param name="hookType">The type of the system hook.</param>
    public WindowsSystemHookBase(HookType hookType)
    {
        _hookType = hookType;
        _filterFunc = new User32.HookProc(this.CoreHookProc);
    }

    /// <summary>
    /// The constructor accepting a hook type and hook method as input arguments
    /// </summary>
    /// <param name="hookType">The type of the system hook.</param>
    /// <param name="func">A delegate that will be used  for system hooking, by User32.SetWindowsHookEx</param>
    public WindowsSystemHookBase(HookType hookType, User32.HookProc func)
    {
        _hookType = hookType;
        _filterFunc = func;
    }
    #endregion // Constructor(s)

    #region Finalizer

    // Use C# destructor syntax for generation of finalizer method code.
    // The actually generated method (finalizer) will run only if the Dispose method
    // does not get called (  therefore GC.SuppressFinalize(this); has not been called ).

    /// <summary>
    /// Finalizer
    /// </summary>
    /// <remarks>
    /// Note: the compiler actually expands the pseudo-destructor syntax here to following code:
    /// <code>
    /// protected override void Finalize()
    /// {
    ///   try
    ///   { // Your cleanup code here
    ///     Dispose(false);
    ///   }
    ///   finally
    ///   {
    ///     base.Finalize();
    ///   }
    /// }
    /// </code>
    /// </remarks>
    /// <seealso href="http://www.devx.com/dotnet/Article/33167/1954">
    /// When and How to Use Dispose and Finalize in C#</seealso>
    ~WindowsSystemHookBase()
    {
        Dispose(false);
    }
    #endregion // Finalizer

    #region Properties

    /// <summary>
    /// Return the initialized hook type
    /// </summary>
    public HookType HookType
    {
        get { return _hookType; }
    }

    /// <summary>
    /// Has been the hook installed?
    /// </summary>
    public bool IsInstalled
    {
        get { return (_hookHandle != null && !_hookHandle.IsInvalid); }
    }

    /// <summary>
    /// Returns the handle of installed system hook ( if there is any )
    /// </summary>
    protected IntPtr HHook
    {
        get { return _hookHandle.DangerousGetHandle(); }
    }
    #endregion // Properties

    #region Methods

    /// <summary> Install the hook on the current thread. </summary>
    ///
    /// <remarks>
    /// You may overwrite this method in a derived class. For instance for the WH_KEYBOARD_LL, which unlike the
    /// other hooks must be global;
    /// that is, the dwThreadId parameter must be 0.  <br/>
    /// Otherwise, the hook does not install, with the SetWindowsHookEx API setting the GetLastError value to: <br/>
    /// error code : 1429 ( ERROR_GLOBAL_ONLY_HOOK ) <br/>
    /// error description : This hook procedure can only be set globally )<br/>
    /// </remarks>
    public virtual void Install()
    {
        if (!IsInstalled)
        {
            uint nThreadId = Kernel32.GetCurrentThreadId();
            Install(nThreadId);
        }
    }

    /// <summary> Install the hook on thread specified by thread Id. </summary>
    /// <param name="threadID"> The Id of the system thread on which  the delegate <see cref="_filterFunc"/>
    ///  should be installed. </param>
    public virtual void Install(uint threadID)
    {
        if (!IsInstalled)
        {
            _hookHandle = new SafeWindowsHookHandle(User32.SetWindowsHookEx(_hookType, _filterFunc, IntPtr.Zero, threadID));
            int lastErr = Marshal.GetLastWin32Error();

            if (_hookHandle.IsInvalid)
            {
                string win32Error = Marshal.GetPInvokeErrorMessage(lastErr);
                string errorMessage = $"SetWindowsHookEx failed, GetLastError == {lastErr} ( {win32Error} ).";
                ReportError(errorMessage);

                _hookHandle = null;
            }
        }
    }

    /// <summary> Uninstall the hook. </summary>
    public void Uninstall()
    {
        if (IsInstalled)
        {
            _hookHandle.Dispose();
            _hookHandle = null;
        }
    }

    /// <summary> Auxiliary method used to call the next hook in the hook chain. </summary>
    ///
    /// <param name="code">   The hook code passed to the current hook procedure. The next hook procedure uses
    ///   this code to determine how to process the hook information. </param>
    /// <param name="wParam"> The wParam value passed to the current hook procedure. The meaning of this
    ///   parameter depends on the type of hook associated with the current hook chain. </param>
    /// <param name="lParam"> The lParam value passed to the current hook procedure. The meaning of this
    ///   parameter depends on the type of hook associated with the current hook chain. </param>
    ///
    /// <returns>
    /// This value is returned by the next hook procedure in the chain. The current hook procedure must also
    /// return this value. The meaning of the return value depends on the hook type. For more information, see
    /// the descriptions of the individual hook procedures. </returns>
    public IntPtr CallNextHook(int code, IntPtr wParam, IntPtr lParam)
    {
        return User32.CallNextHookEx(HHook, code, wParam, lParam);
    }

    /// <summary> Reports an error. Here just writes to trace, in derived classes logging may be involved. </summary>
    /// <param name="errorMessage"> Message describing the error. </param>
    protected internal virtual void ReportError(string errorMessage)
    {
        Trace.WriteLine(errorMessage);
    }

    /// <summary>
    /// Default filter function ( if none specified ).
    /// </summary>
    /// <param name="code">   The hook code passed to the current hook procedure. The next hook procedure uses
    ///   this code to determine how to process the hook information. </param>
    /// <param name="wParam"> The wParam value passed to the current hook procedure. The meaning of this
    ///   parameter depends on the type of hook associated with the current hook chain. </param>
    /// <param name="lParam"> The lParam value passed to the current hook procedure. The meaning of this
    ///   parameter depends on the type of hook associated with the current hook chain. </param>
    ///
    /// <returns>
    /// This value is returned by the next hook procedure in the chain. The current hook procedure must also
    /// return this value. The meaning of the return value depends on the hook type. For more information, see
    /// the descriptions of the individual hook procedures. </returns>
    /// <remarks> Do NOT make this method virtual. Derived classes should override by 'new' keyword</remarks>
    protected IntPtr CoreHookProc(int code, IntPtr wParam, IntPtr lParam)
    {
        // Yield to the next hook in the chain
        return CallNextHook(code, wParam, lParam);
    }

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the method has been
    /// called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed. If
    /// disposing equals false, the method has been called by the runtime from inside the finalizer and you
    /// should not reference other objects. Only unmanaged resources can be disposed.
    /// </summary>
    ///
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
    protected virtual void Dispose(bool disposing)
    {
        Uninstall(); // in both cases this will release just unmanaged resources
    }
    #endregion // Methods

    #region IDisposable members

    /// This single method is needed to implement IDisposable.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposable members
}

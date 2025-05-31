///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// MSJ Copyright notice
// 
// This software is a Derivative Work based upon a MSJ article
// "More Fun With MFC: DIBs, Palettes, Subclassing and a Gamut of Goodies, Part II"
// from the March 1997 issue of Microsoft Systems Journal
// https://web.archive.org/web/20040614000754/http://www.microsoft.com/msj/0397/mfcp2/mfcp2.aspx
// by Paul DiLascia
// https://en.wikipedia.org/wiki/Paul_DiLascia
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Ignore Spelling: Coord, Subclassing, Utils
//
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.Interfaces;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.MessageHooking;

/// <summary>
/// The <c>WindowMessageHook</c> class enables hooking (subclassing) to any Win32 window. Subclassing in the Windows sense
/// refers to inserting this class’s window procedure ahead of the existing one in the Windows message chain.
///
/// This functionality is especially useful when you either do not have the source code of the control you need to hook, 
/// or you cannot (or do not want to) extend the class hierarchy (for example, when multiple inheritance would be required 
/// but is not supported by the language).
///
/// The original idea for this design was inspired by the article 
/// “More Fun With MFC: DIBs, Palettes, Subclassing and a Gamut of Goodies Part II” by Paul DiLascia, 
/// published in Microsoft Systems Journal, March 1997, particularly the section “CMsgHook – Windows Subclassing.”
///
/// The implementation in C#/.NET uses 
/// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.nativewindow">NativeWindow</see> 
/// from the .NET Framework Class Library.
/// However, <c>NativeWindow</c> cannot be used directly because when multiple instances are hooked to the same window, 
/// only one remains functional.
/// Therefore, this implementation uses an internal <see cref="ChainedNativeWindow"/> (derived from <c>NativeWindow</c>), 
/// and multiple hooks attached to the same Win32 <c>HWND</c> are linked in a chain.
///
/// To initialize the hook, call the public method <c>HookWindow(IntPtr hwnd)</c>, 
/// where <c>hwnd</c> is the window handle of the control or window to hook.
/// In your derived class, override:
/// <code>
/// protected virtual void HookWindowProc(ref Message m)
/// </code>
/// to modify message processing as needed.
/// </summary>
public class WindowMessageHook : IDisposableEx
{
    #region Fields
    /// <summary> the window hooked </summary>
    private IntPtr _hookedHwnd;
    /// <summary> the next hook in the chain </summary>
    private WindowMessageHook _nextHook;
    /// <summary> the actual native hook doing the job </summary>
    private ChainedNativeWindow _realHook;
    /// <summary> Track whether Dispose has been called. </summary>
    private bool _disposed;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Argument-less constructor. </summary>
    public WindowMessageHook()
    { }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Returns the hooked window (if any), or null. </summary>
    public IntPtr HookedHWND { get => _hookedHwnd; }

    /// <summary> Returns true if hooked to window, false otherwise. </summary>
    public bool IsHooked { get => (IntPtr.Zero != HookedHWND); }

    /// <summary> Get the next hook in the chain </summary>
    protected internal WindowMessageHook NextHook { get => _nextHook; }
    #endregion // Properties

    #region Public Methods

    /// <summary> Subclass a window. To unhook once hooked instance, call Hook(IntPtr.Zero). </summary>
    ///
    /// <remarks> The other option for unhooking is to leave it up to the default processing (the hook unhooks
    /// itself automatically when processing WM_NCDESTROY) </remarks>
    ///
    /// <param name="hwnd"> The handle of the hooked control. </param>
    ///
    /// <returns> True on success, false on failure.  <br/>
    /// Returns false if the given handle is not valid window handle, or if this hook is hooked-up
    /// already to something. </returns>
    public virtual bool HookWindow(IntPtr hwnd)
    {
        bool result = false;

        if (hwnd != IntPtr.Zero)
        {
            if ((User32.IsWindow(hwnd)) && !IsHooked)
            {  // Hook the window - add to map of hooks, etc.
                MsgHookMap.Instance.Add(hwnd, this);
                result = true;
                OnHookup(null);
            }
            else
            {   // wrong argument
                Debug.Assert(User32.IsWindow(hwnd)); // argument must be window
                Debug.Assert(!IsHooked); // must not be hooked already
            }
        }
        else if (IsHooked)
        {   // Unhook the window ( remove from map etc.)
            MsgHookMap.Instance.Remove(this);
            OnUnhook(null);
        }
        return result;
    }

    /// <summary>
    /// Convert the client-area coordinates of a specified point to screen coordinates. <br/>
    /// The client-area in this context is understood as the client part of <see cref="HookedHWND"/>.
    /// </summary>
    /// <param name="pt">Input/output argument  containing in-out coordinates</param>
    /// <remarks>Auxiliary helper; can be used by the derived class.</remarks>
    public void ClientToScreen(ref Point pt)
    {
        ClientToScreen(this.HookedHWND, ref pt);
    }

    /// <summary>
    /// Convert the client-area coordinates of a specified point to screen coordinates. <br/>
    /// The client-area in this context is understood as the client part of <see cref="HookedHWND"/>.
    /// </summary>
    /// <param name="hwnd">The control whose client area is being referenced.</param>
    /// <param name="pt">Input/output argument  containing in-out coordinates</param>
    /// <remarks>Auxiliary helper; can be used by the derived class.</remarks>
    public static void ClientToScreen(IntPtr hwnd, ref Point pt)
    {
        Debug.Assert(hwnd != IntPtr.Zero);
        User32.POINT ptapi = new(pt.X, pt.Y);

        User32.ClientToScreen(hwnd, ref ptapi);
        pt.X = ptapi.x;
        pt.Y = ptapi.y;
    }

    /// <summary>
    /// Convert the screen coordinates of a specified point to client-area coordinates. <br/>
    /// The client-area in this context is understood as the client part of <see cref="HookedHWND"/>.
    /// </summary>
    /// <param name="hwnd">The control whose client area is being referenced.</param>
    /// <param name="pt">Input/output argument  containing in-out coordinates</param>
    public static void ScreenToClient(IntPtr hwnd, ref Point pt)
    {
        Debug.Assert(hwnd != IntPtr.Zero);
        User32.POINT ptapi = new(pt.X, pt.Y);

        User32.ScreenToClient(hwnd, ref ptapi);
        pt.X = ptapi.x;
        pt.Y = ptapi.y;
    }

    /// <summary> Retrieves a handle to the window that contains the specified point. </summary>
    /// <remarks> Auxiliary helper; can be used by the derived class. </remarks>
    /// <param name="screenCoord"> Screen coordinates of the point. </param>
    /// <returns> If the function succeeds, the return value is the handle of the window that contains
    /// the point. If no window exists at the given point, the return value is IntPtr.Zero. </returns>
    public static IntPtr WndFromPoint(System.Drawing.Point screenCoord)
    {
        return User32.WndFromPoint(screenCoord);
    }

    /// <summary> Returns the control that is currently associated with the handle of window  that contains the
    /// specified point. </summary>
    /// <param name="screenCoord"> Screen coordinates of the point. </param>
    /// <returns>WinForms control on success,  or null on failure.</returns>
    public static Control ControlFromPoint(Point screenCoord)
    {
        IntPtr hwnd;
        Control result = null;

        if (IntPtr.Zero != (hwnd = WndFromPoint(screenCoord)))
        {
            result = Control.FromHandle(hwnd);
        }
        return result;
    }
    #endregion // Public Methods

    #region Protected methods

    /// <summary>
    /// Calls DefWndProc on the originally hooked control.
    /// Delegates the functionality to the overloaded method.
    /// </summary>
    /// <param name="msg">The ID number for the Win32 message. </param>
    /// <param name="wParam">The WParam argument of the message. </param>
    /// <param name="lParam">The lParam argument of the message.</param>
    /// <returns>Specifies the value that is returned to Windows in response to handling the message. </returns>
    protected virtual IntPtr CallOrigProc(int msg, IntPtr wParam, IntPtr lParam)
    {
        Message m = new()
        {
            HWnd = this.HookedHWND,
            Msg = msg,
            WParam = wParam,
            LParam = lParam
        };
        return this.CallOrigProc(ref m);
    }

    /// <summary>
    /// Calls DefWndProc on the originally hooked control
    /// </summary>
    /// <param name="m"> The Message structure which wraps Win32 messages that Windows sends. </param>
    /// <returns>Specifies the value that is returned to Windows in response to handling the message. </returns>
    protected virtual IntPtr CallOrigProc(ref Message m)
    {
        this._realHook.DefWndProc(ref m);
        return m.Result;
    }

    /// <summary>
    /// This is the setter of _nextHook. Should be rather a method than a property.
    /// </summary>
    /// <param name="hook">The value being assigned as a next hook in the hook chain.</param>
    protected internal void SetNextHook(WindowMessageHook hook)
    {
        _nextHook = hook;
    }

    /// <summary> Auxiliary helper called from HookWindow. Nothing really done here ( the derived class can
    /// override it if convenient ). </summary>
    ///
    /// <param name="pExtraInfo"> The object providing additional information; possibly provided by the
    /// derived class in case it overrides the caller <see cref="HookWindow(IntPtr)"/> </param>
    protected virtual void OnHookup(object pExtraInfo)
    { }

    /// <summary>
    /// Auxiliary helper called from HookWindow. 
    /// Nothing really done here ( but the derived class can override it if convenient ).
    /// </summary>
    /// <param name="pExtraInfo"> The object providing additional information; possibly provided by the
    /// derived class in case it overrides the caller <see cref="HookWindow(IntPtr)"/> </param>
    protected virtual void OnUnhook(object pExtraInfo)
    { }

    /// <summary>
    /// The virtual method, usually overwritten by the derived class 
    /// Delegates the processing to the next hook ( if there is any),
    /// otherwise calls the 'real hook' DefWndProc.
    /// 
    /// In case the derived class overwrites this method,
    /// it MUST call the base class processing as well;
    /// ( otherwise the messages don't get processed by the 'real hook'
    /// </summary>
    /// <param name="m"> The Message structure which wraps Win32 messages that Windows sends. </param>
    protected virtual void HookWindowProc(ref Message m)
    {
        if (_nextHook != null)
            _nextHook.HookWindowProc(ref m);
        else
            this._realHook.DefWndProc(ref m);
    }
    #endregion // Protected methods

    #region internal methods

    /// <summary>
    /// Auxiliary implementation helper, created only for purpose of ChainedNativeWindow implementation
    /// ( so it is able to call protected ( but not internal ) WindowMessageHook.HookWindowProc
    /// </summary>
    /// <param name = "m"> The Windows message structure being processed.</param>
    internal void CallHookWindowProc(ref Message m)
    {
        HookWindowProc(ref m);
    }

    /// <summary>
    /// Auxiliary implementation helper, called by MsgHookMap
    /// </summary>
    /// <param name = "hwnd" > The handle of the window being subclassed.</param>
    /// <param name = "nativeHook" > The ChainedNativeWindow object handling the native hook logic.</param>
    /// <returns>true on success, false on failure </returns>
    internal bool SubclassWindow(IntPtr hwnd, ChainedNativeWindow nativeHook)
    {
        bool result = false;

        Debug.Assert(hwnd != IntPtr.Zero);
        Debug.Assert(nativeHook != null);
        Debug.Assert(nativeHook.FirstMsgHook == null);
        Debug.Assert(!IsHooked);
        if (!IsHooked)
        {   // do the hookup, setup hookedWnd and _nextHook
            this._hookedHwnd = hwnd;
            this._realHook = nativeHook;
            Debug.Assert(this._nextHook == null);
            nativeHook.AssignHandle(hwnd);
            nativeHook.FirstMsgHook = this;
            result = true;
        }
        return result;
    }

    /// <summary>
    /// Auxiliary helper method. Adds this instance to the chain of message hooks
    /// represented by the specified <paramref name="nativeHook"/>.
    /// </summary>
    /// <param name="hwnd">The window handle (HWND) to associate with this hook.</param>
    /// <param name="nativeHook">The existing chained native window object to which this will be added.</param>
    /// <returns><c>true</c> if the hook was successfully added; otherwise, <c>false</c>.</returns>
    internal bool ChainHook(IntPtr hwnd, ChainedNativeWindow nativeHook)
    {
        bool result = false;

        Debug.Assert(hwnd != IntPtr.Zero);
        Debug.Assert(!IsHooked);
        Debug.Assert(nativeHook != null);
        Debug.Assert(nativeHook.FirstMsgHook != null);
        if (!IsHooked)
        {
            this._hookedHwnd = hwnd;
            this._realHook = nativeHook;
            this._nextHook = nativeHook.FirstMsgHook;
            nativeHook.FirstMsgHook = this;
            result = true;
        }
        return result;
    }

    /// <summary>
    /// Auxiliary implementation helper. Remove 'this object' from chain of message hooks.
    /// </summary>
    internal bool UnChainHook()
    {
        bool result = false;

        Debug.Assert(IsHooked);
        if (IsHooked)
        {
            this._hookedHwnd = IntPtr.Zero;
            this._realHook = null;
            this._nextHook = null;
            result = true;
        }
        return result;
    }

    /// <summary>
    /// Auxiliary implementation helper. Releases the hooked handle and calls UnChainHook().
    /// </summary>
    /// <returns>true on success, false on failure </returns>
    internal bool UnSubclassWindow()
    {
        bool result = false;

        Debug.Assert(IsHooked);
        if (IsHooked)
        {
            if (!_realHook.PreserveHandle)
            {
                _realHook.ReleaseHandle();
            }
            UnChainHook();
            result = true;
        }
        return result;
    }
    #endregion // internal methods

    #region IDisposableEx members
    #region IDisposable Members
    /// <summary>
    /// Implement IDisposable.
    /// Do not make this method virtual.
    /// A derived class should not be able to override this method.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        // Take yourself off the Finalization queue to prevent finalization code 
        // for this object from executing a second time.
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the method has been
    /// called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed. If
    /// disposing equals false, the method has been called by the runtime from inside the finalizer and you
    /// should not reference other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (!this.IsDisposed)
        {
            // If disposing equals true, dispose both managed and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources here ( if there are any )
                if (IsHooked)
                {   // unhook window
                    HookWindow(IntPtr.Zero);
                }
            }
            // Now release unmanaged resources. If disposing is false, only that code is executed.
            // Actually, nothing to do here for this particular class

            ////////////////////////////////////////////////////////////////////
            // Note that this is not thread safe.
            // Another thread could start disposing the object
            // after the managed resources are disposed,
            // but before the _disposed flag is set to true.
            // If thread safety is necessary, it must be
            // implemented by the client ( calling code ).
        }
        _disposed = true;
    }
    #endregion // IDisposable Members

    /// <summary>
    /// Implements <see cref="IDisposableEx.IsDisposed"/>
    /// Returns true in case the object has been disposed and no longer should be used.
    /// </summary>
    public bool IsDisposed { get => _disposed; }

    #endregion // IDisposableEx members
}

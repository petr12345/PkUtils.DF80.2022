///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// MSJ Copyright notice
// 
// This software is a Derivative Work based upon a MSJ article
// "More Fun With MFC: DIBs, Palettes, Subclassing and a Gamut of Goodies, Part II"
// from the March 1997 issue of Microsoft Systems Journal, by Paul DiLascia
// http://www.microsoft.com/msj/0397/mfcp2/mfcp2.aspx
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Ignore Spelling: Utils
//
using System;
using System.Windows.Forms;
using PK.PkUtils.WinApi;

#pragma warning disable IDE0060	// Avoid unused parameters
#pragma warning disable IDE1006	// Naming violation


namespace PK.PkUtils.MessageHooking;

/// <summary>
/// The <c>Win32WindowHook</c> class is a specialized <see cref="WindowMessageHook"/> designed for hooking up
/// objects that support <see cref="IWin32Window"/>. It shares the core functionality of the base class.
/// For more details, refer to the documentation of <see cref="WindowMessageHook"/>.
/// </summary>
/// <seealso cref="ControlMessageHook"/>
[CLSCompliant(false)]
public class Win32WindowHook : WindowMessageHook
{
    #region Fields
    /// <summary>
    /// The interface of IWin32Window window hooked
    /// </summary>
    private IWin32Window _hookedIWin32;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Argument-less constructor.
    /// You can create Win32WindowHook first without providing any argument and hook it only later.
    /// </summary>
    public Win32WindowHook()
    { }

    /// <summary> Constructor which directly hooks the given IWin32Window. </summary>
    /// <param name="iWnd"> An object that exposes Win32 HWND handle. Must NOT be null. </param>
    public Win32WindowHook(IWin32Window iWnd)
    {
        ArgumentNullException.ThrowIfNull(iWnd, nameof(iWnd));
        HookWindow(iWnd);
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Returns the hooked window interface (if any), or null.
    /// </summary>
    public IWin32Window HookedWindow
    {
        get { return _hookedIWin32; }
    }
    #endregion // Properties

    #region Public Methods

    /// <summary>
    /// Hooks the given IWin32Window-derived object, by calling base.HookWindow(iWnd.Handle). 
    /// To unhook already created hook, you should call HookWindow(null).
    /// </summary>
    /// <param name="iWnd"> An object that exposes Win32 HWND handle.
    /// Could be null; in that case the hook unhooks itself. 
    /// </param>
    /// <returns>   True on success, false on failure. </returns>
    public virtual bool HookWindow(IWin32Window iWnd)
    {
        bool bWasHooked = IsHooked;
        bool bRes;

        if (iWnd != null)
        {   // the caller wants to hook something
            bRes = base.HookWindow(iWnd.Handle);
            if (IsHooked && !bWasHooked)
            {   // update the member variable
                _hookedIWin32 = iWnd;
                OnHookup(HookedWindow);
            }
        }
        else
        {   // the caller wants to unhook 
            bRes = base.HookWindow(IntPtr.Zero);
            if ((!IsHooked) && bWasHooked)
            {   // update the member variable
                OnUnhook(HookedWindow);
                _hookedIWin32 = null;
            }
        }
        return bRes;
    }

    /// <summary> Returns the dialog-control id for the specific message command ( WM_COMMAND ).  <br/>
    /// For more information, see <a href="http://msdn.microsoft.com/en-us/library/ms647591.aspx">
    /// WM_COMMAND message help.</a> 
    /// </summary>
    /// 
    /// <remarks> Auxiliary helper, that may be used by derived classes. </remarks>
    ///
    /// <param name="wParam"> The wParam parameter of windows message. </param>
    /// <param name="lParam"> The lParam parameter of windows message. </param>
    ///
    /// <returns> The control Id. </returns>
    public static ushort xWMCMD_ID(uint wParam, IntPtr lParam)
    {
        return (Win32.LOWORD((int)wParam));
    }

    /// <summary>
    /// Returns the dialog-control notification code for the specific message command ( WM_COMMAND )
    /// If the message is from an accelerator, this value is 1. If the message is from a menu, this value is 0.<br/>
    /// For more information, see <a href="http://msdn.microsoft.com/en-us/library/ms647591.aspx">
    /// WM_COMMAND message help.</a> 
    /// </summary>
    ///
    /// <remarks> Auxiliary helper, that may be used by derived classes. </remarks>
    ///
    /// <param name="wParam"> The wParam parameter of windows message. </param>
    /// <param name="lParam"> The lParam parameter of windows message. </param>
    ///
    /// <returns> The control-specific notification code. </returns>
    public static ushort xWMCMD_CMD(uint wParam, IntPtr lParam)
    {
        return (Win32.HIWORD((int)wParam));
    }

    /// <summary>
    /// Returns the dialog-control handle (HWND) for the specific message command ( WM_COMMAND )  <br/>
    /// For more information, see <a href="http://msdn.microsoft.com/en-us/library/ms647591.aspx">
    /// WM_COMMAND message help.</a> </summary>
    ///
    /// <remarks> Auxiliary helper, that may be used by derived classes. </remarks>
    ///
    /// <param name="wParam"> The wParam parameter of windows message. </param>
    /// <param name="lParam"> The lParam parameter of windows message. </param>
    ///
    /// <returns> The window control handle . </returns>
    public static IntPtr xWMCMD_WND(uint wParam, IntPtr lParam)
    {
        return lParam;
    }
    #endregion // Public Methods
}

#pragma warning restore IDE1006
#pragma warning restore IDE0060
// Ignore Spelling: PK, Utils
//
using System;
using PK.PkUtils.WinApi;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace PK.PkUtils.UI.Utils;

/// <summary> A wrapper for window handle, implementing IWin32Window. </summary>
public class Win32Window(IntPtr handle) : IWin32Window, System.Windows.Interop.IWin32Window
{
    private readonly IntPtr _handle = handle;

    ///// <summary> Constructor. </summary>
    ///// <param name="handle"> The handle to the represented window. </param>
    //public Win32Window(IntPtr handle)
    //{
    //    _handle = handle;
    //}

    /// <summary> Constructor. </summary>
    /// <exception cref="ArgumentNullException"> Thrown <paramref name="window"/> is null. </exception>
    /// <param name="window"> The window. </param>
    public Win32Window(IWin32Window window)
        : this((window is not null) ? window.Handle : throw new ArgumentNullException(nameof(window)))
    { }


    /// <summary> Constructor. </summary>
    /// <exception cref="ArgumentNullException"> Thrown <paramref name="window"/> is null. </exception>
    /// <param name="window"> The window. </param>
    public Win32Window(System.Windows.Interop.IWin32Window window)
        : this((window is not null) ? window.Handle : throw new ArgumentNullException(nameof(window)))
    { }


    /// <summary> Gets the handle to the represented window. </summary>
    public IntPtr Handle => _handle;

    /// <summary> Gets the handle to the represented window. </summary>
    nint System.Windows.Interop.IWin32Window.Handle => _handle;

    /// <summary> Gets focused window, if there is any. </summary>
    /// <returns>  The IWin32Window representing focused window handle. </returns>
    public static IWin32Window GetFocusedWindow()
    {
        return (User32.GetFocusedControl() as IWin32Window) ?? new Win32Window(User32.GetFocus());
    }
}

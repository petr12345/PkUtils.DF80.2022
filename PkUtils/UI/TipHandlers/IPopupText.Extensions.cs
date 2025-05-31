// Ignore Spelling: Utils, Popup
//
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PK.PkUtils.UI.TipHandlers;

/// <summary> 
/// Static class containing extensions methods of <see cref="IPopupText"/>.
/// </summary>
[CLSCompliant(true)]
public static class PopupTextExtensions
{
    #region Public Methods

    /// <summary> An extension method changing the position and size of tooltip window <paramref name="this"/>. </summary>
    ///
    /// <param name="this">         Tooltip window being moved. </param>
    /// <param name="win32Window">         An object that exposes Win32 HWND handle. Could be null. </param>
    /// <param name="ptLocation">   The new location, relative to top-left corner of <paramref name="win32Window"/>. </param>
    /// <param name="size">         The new size. </param>
    public static void MoveToWindow(this IPopupText @this, IWin32Window win32Window, Point ptLocation, Size size)
    {
        ArgumentNullException.ThrowIfNull(@this);

        @this.Size = size;
        @this.MoveToWindow(win32Window, ptLocation);
    }

    /// <summary>
    /// An extension method changing the position and size of tooltip window <paramref name="this"/>.
    /// </summary>
    ///
    /// <param name="this">         Tooltip window being moved. </param>
    /// <param name="ptLocation">   The new location, relative to top-left corner of desktop. </param>
    /// <param name="size">         The new size. </param>
    /// <param name="topMost">      True to make tip window topmost, false otherwise. </param>
    public static void MoveToScreen(this IPopupText @this, Point ptLocation, Size size, bool topMost)
    {
        ArgumentNullException.ThrowIfNull(@this);

        @this.Size = size;
        if (!topMost)
            @this.MoveToWindow(null, ptLocation);
        else
            @this.MoveAsTopmost(ptLocation);
    }

    /// <summary> An extension method changing the position of tooltip window <paramref name="this"/>. </summary>
    ///
    /// <param name="this">         Tooltip window being moved. </param>
    /// <param name="ptLocation">   The new location, relative to top-left corner of desktop. </param>
    public static void MoveToScreen(this IPopupText @this, Point ptLocation)
    {
        ArgumentNullException.ThrowIfNull(@this);

        @this.MoveToWindow(null, ptLocation);
    }
    #endregion // Public Methods
}
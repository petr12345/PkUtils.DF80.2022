// Ignore Spelling: Utils, Popup
//
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PK.PkUtils.UI.TipHandlers;

/// <summary>
/// IPopupText interface should be supported by any tooltip window
/// displayed by class <see cref="TipHandler"/> or derived classes.<br/>
/// Classes derived from TipHandler have freedom to create (implement) their tooltip 
/// the way they wish; the control just have to support that interface.
/// </summary>
[CLSCompliant(true)]
public interface IPopupText : IDisposable
{
    /// <summary> Is the tooltip window visible ? </summary>
    bool IsVisible { get; }

    /// <summary> Gets or sets the text of tooltip window. </summary>
    string Text { get; set; }

    /// <summary> Gets or sets the font of tooltip window. </summary>
    Font Font { get; set; }

    /// <summary> Gets or sets the size of tooltip window. </summary>
    Size Size { get; set; }

    /// <summary> Gets or sets the margins of tooltip window. </summary>
    Size Margins { get; set; }

    /// <summary> Gets or sets a value indicating whether the contents should be drawn as highlighted. </summary>
    bool DrawHighlighted { get; set; }

    /// <summary>
    /// Show tooltip window with delay. No (zero) delay means show now.
    /// </summary>
    /// <param name="msec">The delay in milliseconds.</param>
    void ShowDelayed(int msec);

    /// <summary>
    /// Show tooltip window with delay, when showing the position should be changed to mouse position + offset. 
    /// No (zero) delay means show now.
    /// </summary>
    /// <param name="msec">The delay in milliseconds.</param>
    /// <param name="mousePosOffset"> The desired offset from mouse position.</param>
    void ShowDelayed(int msec, Size mousePosOffset);

    /// <summary>
    /// Cancel the tooltip, i.e. hide window.
    /// </summary>
    void Cancel();

    /// <summary>
    /// Changes the position the tooltip window. <br/>
    /// For a non-null argument <paramref name="win32Window"/>, the position is relative to the upper-left
    /// corner of the <paramref name="win32Window"/>.<br/>
    /// For a null argument <paramref name="win32Window"/>, the position is relative to the upper-left
    /// corner of the screen.
    /// </summary>
    ///
    /// <param name="win32Window">         An object that exposes Win32 HWND handle. If not null, position
    /// will be relative to its top left corner. If null, position will be relative to the desktop. </param>
    /// <param name="ptLocation">   A new window position, relative to the upper-left corner of that
    /// window. </param>
    void MoveToWindow(IWin32Window win32Window, Point ptLocation);

    /// <summary>
    /// Places the tooltip window above all non-topmost windows, 
    /// and changes the position. The position is relative to the upper-left corner of the screen.
    /// </summary>
    /// <param name="ptLocation">A new window position, relative to the upper-left corner of that window.</param>
    /// <remarks>The efficient implementation delegates window moving part to <see cref="MoveToWindow"/>.</remarks>
    void MoveAsTopmost(Point ptLocation);
}

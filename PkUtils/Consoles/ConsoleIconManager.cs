// Ignore Spelling: Utils, Taskbar
//
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.Consoles;


/// <summary>
/// Provides methods to manage icons for console applications, including setting the console window and taskbar icons.
/// </summary>
public static class ConsoleIconManager
{
    /// <summary>
    /// Creates an <see cref="Icon"/> from a byte array representing icon data.
    /// </summary>
    /// <param name="resourceData">The byte array containing the icon data.</param>
    /// <returns>An <see cref="Icon"/> instance created from the provided byte array.</returns>
    public static Icon CreateIcon(byte[] resourceData)
    {
        using MemoryStream ms = new(resourceData);
        return new Icon(ms);
    }

    /// <summary>
    /// Sets the console window's icon to the specified <see cref="Icon"/>.
    /// </summary>
    /// <param name="icon">The icon to be set for the console window.</param>
    public static void SetConsoleWindowIcon(Icon icon)
    {
        const int ICON_SMALL = 0;
        const int ICON_BIG = 1;
        IntPtr hWnd = Kernel32.GetConsoleWindow();

        if (hWnd != IntPtr.Zero)
        {
            IntPtr hIcon = icon.Handle;
            User32.SendMessage(hWnd, (int)Win32.WM.WM_SETICON, ICON_SMALL, hIcon);
            User32.SendMessage(hWnd, (int)Win32.WM.WM_SETICON, ICON_BIG, hIcon);
        }
    }

    /// <summary>   Sets the Taskbar icon using a <see cref="NotifyIcon"/>. </summary>
    /// <param name="icon"> The icon to be displayed in the taskbar. </param>
    /// <param name="text"> (Optional) The text. </param>
    public static void SetTaskbarIcon(Icon icon, string text = null)
    {
        NotifyIcon notifyIcon = new()
        {
            Icon = icon,
            Visible = true, // Required for the taskbar
            Text = text ?? "Console Application"
        };

        // Hide and reshow to force Windows to update the taskbar icon
        notifyIcon.Visible = false;
        notifyIcon.Visible = true;
    }
}

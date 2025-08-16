// Ignore Spelling: PK, Utils
//
using System;
using System.Windows.Interop;


namespace PK.PkUtils.UI.Utils;

/// <summary> A WPF-related utilities. </summary>
public static class WpfUtils
{
    #region Methods

    /// <summary> Gets main window handle. </summary>
    /// <returns>   The main window handle, or IntPtr.Zero if failed. </returns>
    public static IntPtr GetMainWindowHandle()
    {
        // Ensure the application has a main window and it's loaded
        System.Windows.Window mainWindow = System.Windows.Application.Current?.MainWindow;
        IntPtr handle = IntPtr.Zero;

        if (mainWindow != null)
        {
            // Get the handle of the main window
            handle = new WindowInteropHelper(mainWindow).Handle;
        }

        return handle;
    }

    /// <summary> Gets main window. </summary>
    /// <returns>   The main window, or null if none found. </returns>
    public static Win32Window GetMainWindow()
    {
        IntPtr handle = GetMainWindowHandle();
        Win32Window result = (handle == IntPtr.Zero) ? null : new Win32Window(handle);

        return result;
    }

    #endregion // Methods
}

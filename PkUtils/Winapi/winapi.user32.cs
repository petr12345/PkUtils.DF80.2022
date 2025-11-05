using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable IDE0130 // Namespace "..." does not match folder structure


namespace PK.PkUtils.WinApi;

#pragma warning disable IDE0079  // Remove unnecessary suppression
#pragma warning disable VSSpell001
#pragma warning disable 1591        // Missing XML comment for publicly visible type or member...
#pragma warning disable IDE0251     // Member can be made 'readonly'	
#pragma warning disable IDE0290     // Use primary constructor
#pragma warning disable CA1069      // The enum member ... has the same constant value as member ...
#pragma warning disable CA1401      // P/Invoke method should not be visible
#pragma warning disable CA1806      // The HRESULT of some API is not used
#pragma warning disable SYSLIB1054  // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

/// <summary> Helper class containing User32 API functions. </summary>
[CLSCompliant(false)]
public static class User32
{
    #region Nested types

    /// <summary> A class defining specific data that will be used for windows enumeration and searching 
    ///  by <see cref="EnumWindows(EnumWindowsProcEx, ref WindowSearchData)"/>. </summary>
    /// <remarks> On purpose instead of structure a class is used here, for the possibility to derive
    /// other class with more custom data from it.</remarks>
    public class WindowSearchData
    {
        /// <summary> Public Constructor. </summary>
        /// <param name="windowText"> Text of the window. </param>
        /// <param name="processId"> Identifier for the process. </param>
        public WindowSearchData(string windowText, uint processId)
        {
            WindowText = windowText;
            ProcessId = processId;
            FoundWindows = [];
        }

        /// <summary> Gets or sets the text (title) of searched window. </summary>
        /// <remarks>Value null could be used to indicate to the enumeration that text of window is irrelevant.</remarks>
        public string WindowText { get; private set; }

        /// <summary> Gets or sets the process id of searched window. </summary>
        /// <remarks>Value 0xffffffff could be used to indicate to the enumeration ProcessId is irrelevant.</remarks>
        public uint ProcessId { get; private set; }

        /// <summary> The list of found windows. </summary>
        public IList<IntPtr> FoundWindows { get; protected set; }

        /// <summary> Identifier could not belong to any actual process. </summary>
        public const uint NoProcessId = 0xffffffff;
    }

    /// <summary>
    /// Contains message information from a thread's message queue.
    /// </summary>
    ///
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-msg">
    /// MSDN documentation of Win32 MSG structure
    /// </seealso>
    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        /// <summary> A handle to the window whose window procedure receives the message. </summary>
        public IntPtr hwnd;

        /// <summary> The message identifier. Applications can only use the low word; 
        ///  the high word is reserved by the system. </summary>
        public int message;

        /// <summary> Additional information about the message. 
        /// The exact meaning depends on the value of the message member. </summary>
        public IntPtr wParam;

        /// <summary> Additional information about the message. 
        /// The exact meaning depends on the value of the message member. </summary>
        public IntPtr lParam;

        /// <summary>The time at which the message was posted.</summary>
        public int time;

        /// <summary> 
        /// The x-coordinate of the cursor position, in screen coordinates, when the message was posted.
        /// </summary>
        public int pt_x;

        /// <summary> 
        /// The y-coordinate of the cursor position, in screen coordinates, when the message was posted.
        /// </summary>
        public int pt_y;
    }

    /// <summary>
    /// The Win32 POINT structure defines the x- and y- coordinates of a point.
    /// </summary>
    ///
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd162805(v=vs.85).aspx">
    /// MSDN documentation of Win32 POINT structure
    /// </seealso>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        /// <summary> The x-coordinate of this POINT. </summary>
        public int x;
        /// <summary> The y-coordinate of this POINT. </summary>
        public int y;

        /// <summary>
        /// Constructor. Initializes a new instance of the Point class with the specified coordinates.
        /// </summary>
        /// <param name="xx"> The x-coordinate of this POINT. </param>
        /// <param name="yy"> The y-coordinate of this POINT. </param>
        public POINT(int xx, int yy)
        {
            x = xx;
            y = yy;
        }

        /// <summary>
        /// Constructor. Initializes a new instance of the Point class with the specified coordinates.
        /// </summary>
        /// <param name="pt"> The point whose coordinates will be used for initialization. </param>
        public POINT(Point pt)
        {
            x = pt.X;
            y = pt.Y;
        }

        /// <summary>
        /// Constructor. Initializes a new instance of the Point class with the specified coordinates.
        /// </summary>
        /// <param name="pt"> The point whose coordinates will be used for initialization. </param>
        public POINT(POINT pt)
        {
            x = pt.x;
            y = pt.y;
        }

        /// <summary> Returns the hash code for this instance. </summary>
        /// <returns> A 32-bit signed integer that is the hash code for this instance. </returns>
        public override int GetHashCode()
        {
            return x ^ y;
        }

        /// <summary> Indicates whether this instance and a specified object are equal. </summary>
        /// <param name="obj"> Another object to compare to. </param>
        /// <returns>
        /// true if <paramref name="obj" /> and this instance are the same type and represent the same value;
        /// otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return (obj is POINT pt) && Equals(pt);
        }

        /// <summary> Indicates whether this instance and a specified other POINT are equal. </summary>
        /// <param name="other"> The point to compare to this object. </param>
        /// <returns>
        /// true if <paramref name="other" /> and this instance represent the same value;
        /// otherwise, false.
        /// </returns>
        public bool Equals(POINT other)
        {
            return (x == other.x) && (y == other.y);
        }

        /// <summary> Equality operator. </summary>
        /// <param name="pt1"> The first point. </param>
        /// <param name="pt2"> The second point. </param>
        /// <returns> The result of the comparison. </returns>
        public static bool operator ==(POINT pt1, POINT pt2)
        {
            return pt1.Equals(pt2);
        }

        /// <summary> Inequality operator. </summary>
        /// <param name="pt1"> The first point. </param>
        /// <param name="pt2"> The second point. </param>
        /// <returns> The result of the operation. </returns>
        public static bool operator !=(POINT pt1, POINT pt2)
        {
            return !pt1.Equals(pt2);
        }
    }

    /// <summary>
    /// The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
    /// </summary>
    ///
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd162897(v=vs.85).aspx">
    /// MSDN documentation of Win32 RECT structure
    /// </seealso>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        /// <summary> Specifies the x-coordinate of the upper-left corner of a rectangle. </summary>
        public int left;
        /// <summary> Specifies the y-coordinate of the upper-left corner of a rectangle. </summary>
        public int top;
        /// <summary> Specifies the x-coordinate of the lower-right corner of a rectangle. </summary>
        public int right;
        /// <summary> Specifies the y-coordinate of the lower-right corner of a rectangle. </summary>
        public int bottom;

        /// <summary>
        /// A constructor, initializing a new instance of the RECT structure.
        /// </summary>
        /// <param name="l"> Specifies the x-coordinate of the upper-left corner of a rectangle. </param>
        /// <param name="t"> Specifies the y-coordinate of the upper-left corner of a rectangle. </param>
        /// <param name="r"> Specifies the x-coordinate of the lower-right corner of a rectangle. </param>
        /// <param name="b"> Specifies the y-coordinate of the lower-right corner of a rectangle. </param>
        public RECT(int l, int t, int r, int b)
        { left = l; top = t; right = r; bottom = b; }

        /// <summary> A constructor, initializing a new instance of the RECT structure, 
        /// copying all the values from <paramref name="r"/> argument. </summary>
        /// <param name="r"> An existing Rectangle whose position and dimension will be used. </param>
        public RECT(Rectangle r)
          : this(r.Left, r.Top, r.Right, r.Bottom)
        {
        }

        /// <summary> A copy-constructor, initializing a new instance of the RECT structure, 
        /// copying all the values from <paramref name="r"/> argument. </summary>
        /// <param name="r"> An existing RECT structure that will be copied. </param>
        public RECT(RECT r)
          : this(r.left, r.top, r.right, r.bottom)
        {
        }

        /// <summary> Gets the width of the rectangle. </summary>
        /// <returns> Width of the rectangle. </returns>
        public int Width
        { get { return right - left; } }

        /// <summary> Gets the height of the rectangle. </summary>
        /// <returns> Height of the rectangle. </returns>
        public int Height
        { get { return bottom - top; } }

        /// <summary> Inflates the rectangle by moving its sides away from its center. </summary>
        /// <param name="dx"> Specifies the number of units to inflate the left and right sides of rectangle.
        /// </param>
        /// <param name="dy"> Specifies the number of units to inflate the top and bottom sides of rectangle.
        /// </param>
        public void Inflate(int dx, int dy)
        {
            if ((dx < 0) && (Width + 2 * dx < 0))
            {
                dx = (-Width / 2);
            }
            left -= dx;
            right += dx;

            if ((dy < 0) && (Height + 2 * dy < 0))
            {
                dy = (-Height / 2);
            }
            top -= dy;
            bottom += dy;
        }

        /// <summary> Inflates the rectangle by moving its sides toward its center.</summary>
        /// <param name="dx"> Specifies the number of units to deflate the left and right sides of rectangle.
        /// </param>
        /// <param name="dy"> Specifies the number of units to deflate the top and bottom sides of rectangle.
        /// </param>
        public void Deflate(int dx, int dy)
        {
            Inflate(-dx, -dy);
        }
    }

    /// <summary>
    /// The SIZE structure specifies the width and height of a rectangle.
    /// </summary>
    ///
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd145106(v=vs.85).aspx">
    /// MSDN documentation of Win32 SIZE structure
    /// </seealso>
    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        /// <summary> The Width of this instance of Size. </summary>
        public int cx;
        /// <summary> The Height of this instance of Size. </summary>
        public int cy;

        /// <summary> Returns the hash code for this instance. </summary>
        /// <returns> A 32-bit signed integer that is the hash code for this instance. </returns>
        public override int GetHashCode()
        {
            return cx ^ cy;
        }

        /// <summary> Indicates whether this instance and a specified object are equal. </summary>
        /// <param name="obj"> Another object to compare to. </param>
        /// <returns>
        /// true if <paramref name="obj" /> and this instance are the same type and represent the same value;
        /// otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return (obj is SIZE sz) && Equals(sz);
        }

        /// <summary> Indicates whether this instance and a specified SIZE <paramref name="other"/>
        /// are equal. </summary>
        /// <param name="other"> The size to compare to this object. </param>
        /// <returns> true if <paramref name="other" /> and this instance represent the same value;
        /// otherwise, false.
        /// </returns>
        public bool Equals(SIZE other)
        {
            return (cx == other.cx) && (cy == other.cy);
        }

        /// <summary> Equality operator. </summary>
        /// <param name="size1"> The first size. </param>
        /// <param name="size2"> The second size. </param>
        /// <returns> The result of the operation. </returns>
        public static bool operator ==(SIZE size1, SIZE size2)
        {
            return size1.Equals(size2);
        }

        /// <summary> Inequality operator. </summary>
        /// <param name="size1"> The first size. </param>
        /// <param name="size2"> The second size. </param>
        /// <returns> The result of the operation. </returns>
        public static bool operator !=(SIZE size1, SIZE size2)
        {
            return !size1.Equals(size2);
        }
    };

    /// <summary>
    /// The SCROLLINFO structure contains scroll bar parameters to be set by the SetScrollInfo function (or
    /// SBM_SETSCROLLINFO message), or retrieved by the GetScrollInfo function (or SBM_GETSCROLLINFO
    /// message).
    /// </summary>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb787537(v=vs.85).aspx">
    /// MSDN documentation of SCROLLINFO structure
    /// </seealso>
    [StructLayout(LayoutKind.Sequential)]

    public struct SCROLLINFO
    {
        /// <summary> Specifies the size, in bytes, of this structure </summary>
        public uint cbSize;
        /// <summary> Specifies the scroll bar parameters to set or retrieve. </summary>
        public uint fMask;
        /// <summary> Specifies the minimum scrolling position.</summary>
        public int nMin;
        /// <summary> Specifies the maximum scrolling position.</summary>
        public int nMax;
        /// <summary> Specifies the page size, in device units. </summary>
        public uint nPage;
        /// <summary> Specifies the position of the scroll box. </summary>
        public int nPos;
        /// <summary> Specifies the immediate position of a scroll box that the user is dragging. </summary>
        public int nTrackPos;

        /// <summary> Gets the default (empty) value. </summary>
        public static SCROLLINFO Default { get => new(); }

        /// <summary> Initializes a new instance of the <see cref="SCROLLINFO"/> struct. </summary>
        public SCROLLINFO()
        {
            cbSize = (uint)Marshal.SizeOf(this);
        }
    }

    /// <summary>
    /// Contains information about the size and position of a window.
    /// </summary>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms632612(v=vs.85).aspx">
    /// MSDN documentation of WINDOWPOS structure
    /// </seealso>
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPOS
    {
        /// <summary> A handle to the window.</summary>
        public IntPtr hwnd;

        /// <summary>
        /// The position of the window in Z order (front-to-back position). This member can be a handle to
        /// the window behind which this window is placed, or can be one of the special values of enum
        /// <see cref="Win32.SWP_Vals"/>.
        /// </summary>
        public IntPtr hwndInsertAfter;

        /// <summary> The position of the left edge of the window. </summary>
        public int x;

        /// <summary> The position of the top edge of the window. </summary>
        public int y;

        /// <summary> The window width, in pixels. </summary>
        public int cx;

        /// <summary> The window height, in pixels.</summary>
        public int cy;

        /// <summary> The window position flags. This could be a combination of one or more values of 
        ///           <see cref="Win32.SWP_Flags"/> enum.</summary>
        public uint flags;
    };

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        /// <summary>
        /// The length of the structure, in bytes. Before calling the GetWindowPlacement or SetWindowPlacement functions, set this member to sizeof(WINDOWPLACEMENT).
        /// <para>
        /// GetWindowPlacement and SetWindowPlacement fail if this member is not set correctly.
        /// </para>
        /// </summary>
        public int Length;

        /// <summary>
        /// Specifies flags that control the position of the minimized window and the method by which the window is restored.
        /// </summary>
        public int Flags;

        /// <summary>
        /// The current show state of the window.
        /// </summary>
        public SW ShowCmd;

        /// <summary>
        /// The coordinates of the window's upper-left corner when the window is minimized.
        /// </summary>
        public POINT MinPosition;

        /// <summary>
        /// The coordinates of the window's upper-left corner when the window is maximized.
        /// </summary>
        public POINT MaxPosition;

        /// <summary>
        /// The window's coordinates when the window is in the restored position.
        /// </summary>
        public RECT NormalPosition;

        /// <summary> Gets the default (empty) value. </summary>
        public static WINDOWPLACEMENT Default { get => new(); }

        /// <summary> Initializes a new instance of the <see cref="WINDOWPLACEMENT"/> struct. </summary>
        public WINDOWPLACEMENT()
        {
            Length = Marshal.SizeOf(this);
        }
    }

    /// <summary>
    /// The PAINTSTRUCT structure contains information that can be used to paint the client area 
    /// of a window owned by that application.
    /// </summary>
    ///
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/dd162768(v=vs.85).aspx">
    /// MSDN documentation of PAINTSTRUCT structure
    /// </seealso>
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
    public struct PAINTSTRUCT
    {
        /// <summary> A handle to the display DC to be used for painting. </summary>
        public IntPtr hdc;

        /// <summary> Indicates whether the background must be erased. 
        /// This value is nonzero if the application should erase the background. 
        /// The application is responsible for erasing the background if a window class is created 
        /// without a background brush. 
        /// For more information, see the description of the hbrBackground member of the WNDCLASS structure.
        /// </summary>
        public int fErase;

        /// <summary> RECT structure that specifies the upper left and lower right corners of the rectangle 
        /// in which the painting is requested, 
        /// in device units relative to the upper-left corner of the client area.
        /// </summary>
        public RECT rcPaint;

        /// <summary> Reserved; used internally by the system. </summary>
        public int fRestore;

        /// <summary> Reserved; used internally by the system. </summary>
        public int fIncUpdate;

        /// <summary> Reserved; used internally by the system. </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] rgbReserved;
    };

    /// <summary>
    /// Contains information about an icon or a cursor.
    /// </summary>
    ///
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms648052(v=vs.85).aspx">
    /// MSDN documentation of ICONINFO structure
    /// </seealso>
    [StructLayout(LayoutKind.Sequential)]
    public struct ICONINFO
    {
        /// <summary> Specifies whether this structure defines an icon or a cursor. 
        ///           A value of TRUE specifies an icon; FALSE specifies a cursor.
        /// </summary>
        public bool fIcon;

        /// <summary> The x-coordinate of a cursor's hot spot. If this structure defines an icon, 
        ///  the hot spot is always in the center of the icon, and this member is ignored.
        /// </summary>
        public int xHotspot;

        /// <summary> The y-coordinate of the cursor's hot spot. If this structure defines an icon, 
        /// the hot spot is always in the center of the icon, and this member is ignored.
        /// </summary>
        public int yHotspot;

        /// <summary> The icon bitmask bitmap. 
        /// If this structure defines a black and white icon, 
        /// this bitmask is formatted so that the upper half is the icon AND bitmask 
        /// and the lower half is the icon XOR bitmask. 
        /// Under this condition, the height should be an even multiple of two. <br/>
        /// 
        /// If this structure defines a color icon, this mask only defines the AND bitmask of the icon.
        /// </summary>
        public IntPtr hbmMask;

        /// <summary> A handle to the icon color bitmap. 
        /// This member can be optional if this structure defines a black and white icon. </summary>
        public IntPtr hbmColor;
    }

    /// <summary>
    /// Contains information about a menu item.
    /// This structure is used with functions that operate on menus.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MENUITEMINFO
    {
        /// <summary> Default constructor. </summary>
        public MENUITEMINFO()
        {
            cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO));
            fMask = 0;
            fType = 0;
            fState = 0;
            wID = 0;
            hSubMenu = IntPtr.Zero;
            hbmpChecked = IntPtr.Zero;
            hbmpUnchecked = IntPtr.Zero;
            dwItemData = IntPtr.Zero;
            dwTypeData = null;
            cch = 0;
            hbmpItem = IntPtr.Zero;
        }

        /// <summary>
        /// Specifies the size of the structure, in bytes.
        /// Before calling any function that uses this structure, set this member to <c>Marshal.SizeOf(typeof(MENUITEMINFO))</c>.
        /// </summary>
        public uint cbSize;

        /// <summary>
        /// Indicates which members contain valid data or are to be retrieved.
        /// This member can be a combination of <c>MIIM_*</c> values.
        /// </summary>
        public uint fMask;

        /// <summary>
        /// Specifies the type of the menu item.
        /// This member can be a combination of <c>MFT_*</c> values.
        /// </summary>
        public uint fType;

        /// <summary>
        /// Specifies the state of the menu item.
        /// This member can be a combination of <c>MFS_*</c> values.
        /// </summary>
        public uint fState;

        /// <summary>
        /// Specifies the identifier of the menu item.
        /// </summary>
        public uint wID;

        /// <summary>
        /// Handle to the drop-down menu or submenu associated with the menu item, if applicable.
        /// </summary>
        public IntPtr hSubMenu;

        /// <summary>
        /// Handle to the bitmap displayed when the menu item is checked.
        /// </summary>
        public IntPtr hbmpChecked;

        /// <summary>
        /// Handle to the bitmap displayed when the menu item is unchecked.
        /// </summary>
        public IntPtr hbmpUnchecked;

        /// <summary>
        /// Application-defined value associated with the menu item.
        /// </summary>
        public IntPtr dwItemData;

        /// <summary>
        /// Pointer to a null-terminated string that contains the text for the menu item.
        /// </summary>
        public string dwTypeData;

        /// <summary>
        /// Specifies the length of the menu item text, in characters, excluding the null terminator.
        /// Used when retrieving the item text.
        /// </summary>
        public uint cch;

        /// <summary>
        /// Handle to the bitmap displayed for the menu item.
        /// </summary>
        public IntPtr hbmpItem;
    }

    /// <summary>
    /// Defines constants that could be used as an argument for API <see cref="User32.ShowWindow"/>
    /// </summary>
    public enum SW
    {
        /// <summary>
        /// Hides the window and activates another window
        /// </summary>
        HIDE = 0,
        /// <summary>
        /// Activates and displays a window. If the window is minimized or maximized, 
        /// the system restores it to its original size and position. 
        /// An application should specify this flag when displaying the window for the first time.
        /// </summary>
        SHOWNORMAL = 1,
        /// <summary>
        /// Displays a window in its most recent size and position. 
        /// This value is similar to SW_SHOWNORMAL, except the window is not activated.
        /// </summary>
        SHOWNOACTIVATE = 4,
        /// <summary>
        /// Activates the window and displays it in its current size and position.
        /// </summary>
        SHOW = 5,
        /// <summary>
        /// Minimizes the specified window and activates the next top-level window in the Z order.
        /// </summary>
        MINIMIZE = 6,
        /// <summary>
        /// Displays the window in its current size and position. 
        /// This value is similar to SW_SHOW, except the window is not activated.
        /// </summary>
        SHOWNA = 8,
        /// <summary>
        /// Activates the window and displays it as a maximized window.
        /// </summary>
        SHOWMAXIMIZED = 11,
        /// <summary>
        /// Maximizes the specified window.
        /// </summary>
        MAXIMIZE = 12,
        /// <summary>
        /// Activates and displays the window. If the window is minimized or maximized, 
        /// the system restores it to its original size and position. 
        /// An application should specify this flag when restoring a minimized window.
        /// </summary>
        RESTORE = 13
    }

    /// <summary>
    /// Defines constants to be used for the <see cref="GetWindow"/> API.
    /// </summary>
    /// <seealso href="http://stackoverflow.com/questions/798295/how-can-i-use-getnextwindow-in-c">
    /// SatckOverflow: How can I use GetNextWindow() in C#?</seealso>
    public enum GetWindow_Cmd : uint
    {
        /// <summary> 
        /// The retrieved handle identifies the window of the same type that is highest in the Z order. <br/>
        ///
        /// If the specified window is a topmost window, the handle identifies a topmost window. 
        /// If the specified window is a top-level window, the handle identifies a top-level window. 
        /// If the specified window is a child window, the handle identifies a sibling window.
        /// </summary>
        GW_HWNDFIRST = 0,

        /// <summary> 
        /// The retrieved handle identifies the window of the same type that is lowest in the Z order.<br/>
        ///
        /// If the specified window is a topmost window, the handle identifies a topmost window. 
        /// If the specified window is a top-level window, the handle identifies a top-level window.
        /// If the specified window is a child window, the handle identifies a sibling window.
        /// </summary>
        GW_HWNDLAST = 1,

        /// <summary> 
        /// The retrieved handle identifies the window below the specified window in the Z order.<br/>
        ///
        /// If the specified window is a topmost window, the handle identifies a topmost window. 
        /// If the specified window is a top-level window, the handle identifies a top-level window.
        /// If the specified window is a child window, the handle identifies a sibling window.
        /// </summary>
        GW_HWNDNEXT = 2,

        /// <summary> 
        /// The retrieved handle identifies the window above the specified window in the Z order.<br/>
        ///
        /// If the specified window is a topmost window, the handle identifies a topmost window. 
        /// If the specified window is a top-level window, the handle identifies a top-level window.
        /// If the specified window is a child window, the handle identifies a sibling window.
        /// </summary>
        GW_HWNDPREV = 3,

        /// <summary> The retrieved handle identifies the specified window's owner window, if any. 
        /// For more information, see
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms632599(v=vs.85).aspx#owned_windows"> 
        /// Owned Windows</see>.<br/>
        /// </summary>
        GW_OWNER = 4,

        /// <summary> The retrieved handle identifies the child window at the top of the Z order, 
        /// if the specified window is a parent window; otherwise, the retrieved handle is NULL. 
        /// The function examines only child windows of the specified window. 
        /// It does not examine descendant windows.
        /// </summary>
        GW_CHILD = 5,

        /// <summary> The retrieved handle identifies the enabled popup window owned by the specified window 
        /// (the search uses the first such window found using GW_HWNDNEXT); 
        /// otherwise, if there are no enabled popup windows, 
        /// the retrieved handle is that of the specified window. </summary>
        GW_ENABLEDPOPUP = 6
    };

    /// <summary>
    /// Defines constants to be used as an argument for the <see cref="GetWindowLong"/> and
    /// <see cref="SetWindowLong"/> API.
    /// </summary>
    public enum GwlIndex
    {
        /// <summary> Retrieves the extended window styles. </summary>
        GWL_EXSTYLE = -20,

        /// <summary> Retrieves a handle to the application instance. </summary>
        GWL_HINSTANCE = -6,

        /// <summary> Retrieves a handle to the parent window, if any. </summary>
        GWL_HWNDPARENT = -8,

        /// <summary> Retrieves the identifier of the window.  </summary>
        GWL_ID = -12,

        /// <summary> Retrieves the window styles.  </summary>
        GWL_STYLE = -16,

        /// <summary>
        /// Retrieves the user data associated with the window. This data is intended for use by the
        /// application that created the window. Its value is initially zero.
        /// </summary>
        /// <value> The. </value>
        GWL_USERDATA = -21,

        /// <summary> Retrieves the address of the window procedure, or a handle representing the address 
        ///  of the window procedure. You must use the CallWindowProc function to call the window procedure.</summary>
        GWL_WNDPROC = -4,
    };

    /// <summary> Constants for <see cref="DrawText"/>. </summary>
    [Flags]
    public enum DT_Flags
    {
        /// <summary> Justifies the text to the top of the rectangle. </summary>
        DT_TOP = 0x00000000,

        /// <summary> Aligns text to the left. </summary>
        DT_LEFT = 0x00000000,

        /// <summary> Centers text horizontally in the rectangle. </summary>
        DT_CENTER = 0x00000001,

        /// <summary> Aligns text to the right. </summary>
        DT_RIGHT = 0x00000002,

        /// <summary> Centers text vertically. This value is used only with the DT_SINGLELINE value. </summary>
        DT_VCENTER = 0x00000004,

        /// <summary> Justifies the text to the bottom of the rectangle. 
        ///  This value is used only with the DT_SINGLELINE value. </summary>
        DT_BOTTOM = 0x00000008,

        /// <summary> Breaks words. Lines are automatically broken between words if a word would extend 
        /// past the edge of the rectangle specified by the lpRect parameter. 
        /// A carriage return-line feed sequence also breaks the line.
        /// If this is not specified, output is on one line.
        /// </summary>
        DT_WORDBREAK = 0x00000010,

        /// <summary> Displays text on a single line only. Carriage returns and linefeeds do not break the line. </summary>
        DT_SINGLELINE = 0x00000020,

        /// <summary> Expands tab characters. The default number of characters per tab is eight. </summary>
        DT_EXPANDTABS = 0x00000040,

        /// <summary> Sets tab stops. Bits 8–15, which form the high-order byte of the low-order word, 
        /// of the uFormat parameter specify the number of characters for each tab. 
        /// The default number of characters per tab is eight. 
        /// You cannot use the DT_CALCRECT, DT_EXTERNALLEADING, DT_INTERNAL, DT_NOCLIP, and DT_NOPREFIX values 
        /// with the DT_TABSTOP value. </summary>
        DT_TABSTOP = 0x00000080,

        /// <summary> Draws without clipping. DrawText is somewhat faster when DT_NOCLIP is used. </summary>
        DT_NOCLIP = 0x00000100,

        DT_EXTERNALLEADING = 0x00000200,

        /// <summary> Determines the width and height of the rectangle. If the rectangle includes multiple lines 
        /// of text, DrawText uses the width of the rectangle pointed to by the lpRect parameter and extends 
        /// the base of the rectangle to bound the last line of text. 
        /// If the rectangle includes only one line of text, DrawText modifies the right side of the rectangle 
        /// so that it bounds the last character in the line. In either case, DrawText returns the height 
        /// of the formatted text but does not draw the text. <br/>
        /// 
        /// Before calling DrawText, an application must set the right and bottom members of the RECT structure 
        /// pointed to by lpRect. These members are updated with the call to DrawText. </summary>
        DT_CALCRECT = 0x00000400,

        /// <summary> Turns off processing of prefix characters. Normally, DrawText interprets 
        /// the mnemonic-prefix character &amp; as a directive to underscore the character that follows,
        /// and the mnemonic-prefix characters &amp;&amp; as a directive to print a single &amp;.
        /// By specifying DT_NOPREFIX, this processing is turned off. </summary>
        DT_NOPREFIX = 0x00000800,

        /// <summary> Uses the system font to calculate text metrics. </summary>
        DT_INTERNAL = 0x00001000,

        /// <summary> Duplicates the text-displaying characteristics of a multiline edit control. 
        ///  Specifically, the average character width is calculated in the same manner as for an edit control, 
        ///  and the function does not display a partially visible last line. </summary>
        DT_EDITCONTROL = 0x00002000,
    }

    /// <summary>
    /// Filter function delegate, that can be used as an argument for <see cref="User32.SetWindowsHookEx"/>
    /// </summary>
    ///
    /// <remarks>
    /// The SetWindowsHookEx function always installs a hook procedure at the beginning of a hook chain. 
    /// When an event occurs that is monitored by a particular type of hook, the system calls the procedure 
    /// that is monitored by a particular type of hook, beginning of the hook chain associated with the hook.
    /// Each hook procedure in the chain determines whether to pass the event to the next procedure.
    /// A hook procedure passes an event to the next procedure by calling the CallNextHookEx function.
    /// </remarks>
    ///
    /// <param name="code">   The nCode parameter is a hook code that the hook procedure uses to determine the
    /// action to perform. The value of the hook code depends on the type of the hook; each type has its own
    /// characteristic set of hook codes. <br/>
    /// The values of the wParam and lParam parameters depend on the hook code, but they typically contain
    /// information about a message that was sent or posted. </param>
    /// 
    /// <param name="wParam"> The wParam value passed to the current hook procedure.  
    /// The meaning of the value depends on an actual type of windows hook, 
    /// for which this HookProc has been used. </param>
    /// 
    /// <param name="lParam"> The lParam value passed to the current hook procedure.  
    /// The meaning of the value depends on an actual type of windows hook, 
    /// for which this HookProc has been used. </param>
    ///
    /// <returns>
    /// Implementing code should return to he system the value returned by CallNextHookEx API, that will be
    /// eventually called.
    /// </returns>
    ///
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644959(v=vs.85).aspx">
    /// Hooks overview on MSDN
    /// </seealso>
    public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Defines a delegate that can be used as an argument either when when invoking Win32 CallWindowProc from
    /// managed code, and/or subclassing an unmanaged window.
    /// </summary>
    ///
    /// <param name="hWnd">   A handle to the window procedure to receive the message. </param>
    /// <param name="Msg">    The message. </param>
    /// <param name="wParam"> Additional message-specific information. 
    /// The contents of this parameter depend on the value of the <paramref name="Msg"/>parameter. </param>
    /// <param name="lParam"> Additional message-specific information. 
    /// The contents of this parameter depend on the value of the <paramref name="Msg"/>parameter. </param>
    ///
    /// <returns> The return value specifies the result of the message processing and depends on the message.
    /// </returns>
    /// <seealso cref="CallWindowProc(System.IntPtr, System.IntPtr, uint, System.IntPtr, System.IntPtr)"/>
    public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Defines a delegate that can be used when invoking an
    /// <see cref="EnumWindows(EnumWindowsProc, IntPtr)"/> API.
    /// </summary>
    /// <param name="hWnd"> A handle to an enumerated top-level window. </param>
    /// <param name="lParam"> The application-defined value given in EnumWindows or EnumDesktopWindows. </param>
    /// <returns>
    /// To continue enumeration, the callback function must return true;
    /// to stop enumeration, it must return false.
    /// </returns>
    public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

    /// <summary>
    /// Defines a delegate that can be used when invoking an
    /// <see cref="EnumWindows(EnumWindowsProcEx, ref WindowSearchData)"/> API.
    /// </summary>
    /// <param name="hWnd"> A handle to an enumerated top-level window. </param>
    /// <param name="matchData"> [in,out] The application-defined data that will be used for the windows
    /// properties comparison inside the delegate code. </param>
    /// <returns>
    /// To continue enumeration, the callback function must return true;
    /// to stop enumeration, it must return false.
    /// </returns>
    public delegate bool EnumWindowsProcEx(IntPtr hWnd, ref WindowSearchData matchData);

    /// <summary>
    /// Defines a delegate that can be used when invoking an <see cref="SetTimer"/> API.
    /// </summary>
    /// <param name="hWnd"> The window handle. </param>
    /// <param name="uMsg"> The WM_TIMER message. </param>
    /// <param name="nIDEvent"> timer identifier. </param>
    /// <param name="dwTime"> The time-out value; i.e. the number of milliseconds that have elapsed since
    ///  the system was started. This is the value returned ed by the GetTickCount function. </param>
    public delegate void TimerProc(IntPtr hWnd, uint uMsg, IntPtr nIDEvent, uint dwTime);

    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool EnumChildProc([In] IntPtr hWnd, [In] IntPtr lParam);

    /// Delegate for invoking SetWindowLongPtr dynamically
    internal delegate IntPtr SetWindowLongPtrDelegate(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    /// Delegate for invoking SetWindowLongPtr dynamically
    internal delegate IntPtr SetWindowLongPtrWndProcDelegate(IntPtr hWnd, int nIndex, WndProcDelegate newProc);

    /// Delegate for invoking GetWindowLongPtr dynamically
    internal delegate IntPtr GetWindowLongPtrDelegate(IntPtr hWnd, int nIndex);
    #endregion // Nested types

    #region Constants

    // Constants for DrawEdge:
    //    3D border styles
    public const int BDR_RAISEDOUTER = 0x0001;
    public const int BDR_SUNKENOUTER = 0x0002;
    public const int BDR_RAISEDINNER = 0x0004;
    public const int BDR_SUNKENINNER = 0x0008;

    public const int BDR_OUTER = (BDR_RAISEDOUTER | BDR_SUNKENOUTER);
    public const int BDR_INNER = (BDR_RAISEDINNER | BDR_SUNKENINNER);
    public const int BDR_RAISED = (BDR_RAISEDOUTER | BDR_RAISEDINNER);
    public const int BDR_SUNKEN = (BDR_SUNKENOUTER | BDR_SUNKENINNER);

    public const int EDGE_RAISED = (BDR_RAISEDOUTER | BDR_RAISEDINNER);
    public const int EDGE_SUNKEN = (BDR_SUNKENOUTER | BDR_SUNKENINNER);
    public const int EDGE_ETCHED = (BDR_SUNKENOUTER | BDR_RAISEDINNER);
    public const int EDGE_BUMP = (BDR_RAISEDOUTER | BDR_SUNKENINNER);

    //  and Border flags for for DrawEdge:
    public const int BF_LEFT = 0x0001;
    public const int BF_TOP = 0x0002;
    public const int BF_RIGHT = 0x0004;
    public const int BF_BOTTOM = 0x0008;

    public const int BF_TOPLEFT = (BF_TOP | BF_LEFT);
    public const int BF_TOPRIGHT = (BF_TOP | BF_RIGHT);
    public const int BF_BOTTOMLEFT = (BF_BOTTOM | BF_LEFT);
    public const int BF_BOTTOMRIGHT = (BF_BOTTOM | BF_RIGHT);
    public const int BF_RECT = (BF_LEFT | BF_TOP | BF_RIGHT | BF_BOTTOM);

    public const int BF_DIAGONAL = 0x0010;

    // For diagonal lines, the BF_RECT flags specify the end point of the
    // vector bounded by the rectangle parameter.
    public const int BF_DIAGONAL_ENDTOPRIGHT = (BF_DIAGONAL | BF_TOP | BF_RIGHT);
    public const int BF_DIAGONAL_ENDTOPLEFT = (BF_DIAGONAL | BF_TOP | BF_LEFT);
    public const int BF_DIAGONAL_ENDBOTTOMLEFT = (BF_DIAGONAL | BF_BOTTOM | BF_LEFT);
    public const int BF_DIAGONAL_ENDBOTTOMRIGHT = (BF_DIAGONAL | BF_BOTTOM | BF_RIGHT);

    public const int BF_MIDDLE = 0x0800;  // Fill in the middle
    public const int BF_SOFT = 0x1000;  // For softer buttons
    public const int BF_ADJUST = 0x2000;  // Calculate the space left over
    public const int BF_FLAT = 0x4000;  // For flat rather than 3D borders
    public const int BF_MONO = 0x8000;  // For monochrome borders

    // constants for DrawAnimatedRects
    public const int IDANI_OPEN = 1;
    public const int IDANI_CLOSE = 2;
    public const int IDANI_CAPTION = 3;

    // Menu flags for Add/Check/EnableMenuItem()
    public const int MF_INSERT = 0x00000000;
    public const int MF_CHANGE = 0x00000080;
    public const int MF_APPEND = 0x00000100;
    public const int MF_DELETE = 0x00000200;
    public const int MF_REMOVE = 0x00001000;

    public const int MF_BYCOMMAND = 0x00000000;
    public const int MF_BYPOSITION = 0x00000400;

    public const int MF_SEPARATOR = 0x00000800;

    public const int MF_ENABLED = 0x00000000;
    public const int MF_GRAYED = 0x00000001;
    public const int MF_DISABLED = 0x00000002;

    public const int MF_UNCHECKED = 0x00000000;
    public const int MF_CHECKED = 0x00000008;
    public const int MF_USECHECKBITMAPS = 0x00000200;

    public const int MF_STRING = 0x00000000;
    public const int MF_BITMAP = 0x00000004;
    public const int MF_OWNERDRAW = 0x00000100;

    public const int MF_POPUP = 0x00000010;
    public const int MF_MENUBARBREAK = 0x00000020;
    public const int MF_MENUBREAK = 0x00000040;

    public const int MF_UNHILITE = 0x00000000;
    public const int MF_HILITE = 0x00000080;

    private const string _strGetWindowLongPtrW = "GetWindowLongPtrW";
    private const string _strSetWindowLongPtrW = "SetWindowLongPtrW";
    #endregion // Constants

    #region External functions

    /// <summary> 
    /// Determines whether the specified window handle <paramref name="hWnd"/>identifies an existing window.
    /// </summary>
    /// <param name="hWnd"> A handle to the window to be tested. </param>
    /// <returns> true if window, false if not. </returns>
    /// <remarks> A thread should not use IsWindow for a window that it did not create because the window 
    /// could be destroyed after this function was called. Further, because window handles are recycled 
    /// the handle could even point to a different window.
    /// </remarks>
    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindow(IntPtr hWnd);

    /// <summary> Determines the visibility state of the specified window. </summary>
    /// <param name="hWnd"> A handle to the window to be tested. </param>
    /// <returns> true if window visible, false if not. </returns>
    /// <remarks>
    /// The visibility state of a window is indicated by the WS_VISIBLE style bit. 
    /// When WS_VISIBLE is set, the window is displayed and subsequent drawing into it is displayed 
    /// as long as the window has the WS_VISIBLE style.
    /// Any drawing to a window with the WS_VISIBLE style will not be displayed if the window is obscured 
    /// by other windows or is clipped by its parent window.
    /// </remarks>
    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    /// <summary> Determines whether the specified window is enabled for mouse and keyboard input.
    /// </summary>
    /// <param name="hWnd"> A handle to the window to be tested. </param>
    /// <returns> true if a window is enabled, false if not. </returns>
    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowEnabled(IntPtr hWnd);

    /// <summary> Determines whether the specified window is minimized (iconic). </summary>
    /// <param name="hWnd"> A handle to the window to be tested. </param>
    /// <returns> true if iconic, false if not. </returns>
    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsIconic(IntPtr hWnd);

    /// <summary> Sets the specified window's show state. </summary>
    /// <param name="hWnd"> A handle to the window. </param>
    /// <param name="cmdShow"> Controls how the window is to be shown. The argument value should be
    /// one of ShowWindowCommands enum values.
    /// </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

    /// <summary>
    /// Creates an overlapped, pop-up, or child window with an extended window style; otherwise, this
    /// function is identical to the CreateWindow function. For more information about creating a window
    /// and for full descriptions of the other parameters of CreateWindowEx, see CreateWindow.
    /// </summary>
    /// <param name="dwExStyle"> The extended window style of the window being created. For a list of
    ///  possible values, see
    ///  <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/ff700543(v=vs.85).aspx">
    ///  Extended Window Styles</see> </param>
    /// <param name="lpClassName"> A null-terminated string or a class atom created by a previous call to
    ///  the RegisterClass or RegisterClassEx function. </param>
    /// <param name="lpWindowName"> The window name. If the window style specifies a title bar, the window
    ///  title pointed to by lpWindowName is displayed in the title bar. </param>
    /// <param name="dwStyle"> The style of the window being created. This parameter can be a combination
    ///  of the
    ///  <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms632600(v=vs.85).aspx">
    ///  window style </see> values, plus the control styles indicated in the Remarks section. </param>
    /// <param name="x"> The initial horizontal position of the window. </param>
    /// <param name="y"> The initial vertical position of the window. </param>
    /// <param name="nWidth"> The width, in device units, of the window. </param>
    /// <param name="nHeight"> The height, in device units, of the window. </param>
    /// <param name="hWndParent"> A handle to the parent or owner window of the window being created. </param>
    /// <param name="hMenu"> [in, optional] A handle to a menu, or specifies a child-window identifier,
    ///  depending on the window style. </param>
    /// <param name="hInstance"> [in, optional]A handle to the instance of the module to be associated
    ///  with the window. </param>
    /// <param name="lpParam"> The parameter. </param>
    /// <returns>
    /// If the function succeeds, the return value is a handle to the new window. If the function fails,
    /// the return value is IntPtr.Zero. To get extended error information, call
    /// Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr CreateWindowEx(
      uint dwExStyle,
      [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
      [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
      uint dwStyle,
      int x,
      int y,
      int nWidth,
      int nHeight,
      IntPtr hWndParent,
      IntPtr hMenu,
      IntPtr hInstance,
      IntPtr lpParam);

    /// <summary> Destroys the specified window. The function sends WM_DESTROY and WM_NCDESTROY messages 
    /// to the window to deactivate it and remove the keyboard focus from it. 
    /// The function also destroys the window's menu, flushes the thread message queue, destroys timers, 
    /// removes clipboard ownership, and breaks the clipboard viewer chain (if the window is at the top 
    /// of the viewer chain). <br/>
    /// If the specified window is a parent or owner window, DestroyWindow automatically destroys 
    /// the associated child or owned windows when it destroys the parent or owner window. 
    /// The function first destroys child or owned windows, and then it destroys the parent or owner window.
    /// </summary>
    /// <param name="hWnd"> A handle to the window to be destroyed. </param>
    /// <returns> true if it succeeds, false if it fails. 
    /// To get extended error information, call Marshal.GetLastWin32Error().</returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyWindow(IntPtr hWnd);

    /// <summary> Retrieves a handle to a control in the specified dialog box. </summary>
    /// <param name="hDlg"> A handle to the dialog box that contains the control. </param>
    /// <param name="nIDDlgItem"> The identifier of the control to be retrieved. </param>
    /// <returns> If the function succeeds, the return value is the window handle of the specified control.
    /// If the function fails, the return value is NULL, indicating an invalid dialog box handle 
    /// or a nonexistent control. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

    /// <summary> Retrieves the identifier of the specified control. </summary>
    /// <param name="hCtrl"> A handle to the control. </param>
    /// <returns> If the function succeeds, the return value is the identifier of the control.
    /// If the function fails, the return value is zero. An invalid value for the hwndCtl parameter, 
    /// for example, will cause the function to fail.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern int GetDlgCtrlID(IntPtr hCtrl);

    /// <summary> Retrieves a message from the calling thread's message queue. 
    /// The function dispatches incoming sent messages until a posted message is available for retrieval.
    /// </summary>
    /// <param name="lpMsg"> [out] The message structure that receives message information. </param>
    /// <param name="hWnd"> A handle to the window whose messages are to be retrieved. 
    /// The window must belong to the current thread.
    /// If hWnd is NULL, GetMessage retrieves messages for any window that belongs to the current thread, 
    ///  and any messages on the current thread's message queue whose hwnd value is NULL (see the MSG 
    ///  structure). Therefore if hWnd is NULL, both window messages and thread messages are processed.
    /// </param>
    /// <param name="wMsgFilterMin"> The message filter minimum. 
    ///  The integer value of the lowest message value to be retrieved. </param>
    /// <param name="wMsgFilterMax"> The message filter maximum. 
    ///  The integer value of the highest message value to be retrieved</param>
    /// <returns> If the function retrieves a message other than WM_QUIT, the return value is nonzero.
    /// If the function retrieves the WM_QUIT message, the return value is zero.
    /// If there is an error, the return value is -1. For example, the function fails if hWnd is 
    /// an invalid window handle or lpMsg is an invalid pointer. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", EntryPoint = "GetMessageW", SetLastError = true)]
    public static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    /// <summary> Dispatches incoming sent messages, checks the thread message queue for a posted message, 
    /// and retrieves the message (if any exist). </summary>
    /// 
    /// <param name="lpMsg"> [out] The message structure that receives message information. </param>
    /// <param name="hWnd"> A handle to the window whose messages are to be retrieved. 
    /// The window must belong to the current thread.
    /// If hWnd is IntPtr.Zero, PeekMessage retrieves messages for any window that belongs to the current 
    /// thread, and any messages on the current thread's message queue whose hwnd value is IntPtr.Zero
    /// (see the MSG structure). Therefore if hWnd is IntPtr.Zero, both window messages and thread messages 
    /// are processed.
    /// If hWnd is -1, PeekMessage retrieves only messages on the current thread's message queue whose 
    /// hwnd value is IntPtr.Zero, that is, thread messages as posted by PostMessage (when the hWnd 
    /// parameter is IntPtr.Zero) or PostThreadMessage. </param>
    /// <param name="wMsgFilterMin"> The message filter minimum. 
    ///  The integer value of the lowest message value to be retrieved. </param>
    /// <param name="wMsgFilterMax"> The message filter maximum. 
    ///  The integer value of the highest message value to be retrieved</param>
    /// <param name="wRemoveMsg"> Specifies how messages are to be handled. </param>
    /// <returns> true if it succeeds, false if it fails. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", EntryPoint = "PeekMessage", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PeekMessage(
      out MSG lpMsg,
      IntPtr hWnd,
      uint wMsgFilterMin,
      uint wMsgFilterMax,
      uint wRemoveMsg);

    /// <summary> Sends the specified message to a window or windows. The SendMessage function calls 
    /// the window procedure for the specified window and does not return until the window procedure 
    /// has processed the message.<br/>
    /// To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. 
    /// To post a message to a thread's message queue and return immediately, use the PostMessage 
    /// or PostThreadMessage function.
    /// </summary>
    /// <param name="hWnd"> A handle to the window whose window procedure will receive the message. 
    ///  If this parameter is HWND_BROADCAST ((HWND)0xffff), the message is sent to all top-level windows 
    ///  in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up 
    ///  windows; but the message is not sent to child windows.
    /// </param>
    /// <param name="Msg"> The message to be sent.</param>
    /// <param name="wParam"> Additional message-specific information.</param>
    /// <param name="lParam"> Additional message-specific information.</param>
    /// <returns> The return value specifies the result of the message processing; 
    ///  it depends on the message sent.</returns>
    [DllImport("user32")]
    public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    /// <summary> Places (posts) a message in the message queue associated with the thread that created 
    ///  the specified window and returns without waiting for the thread to process the message.
    /// </summary>
    /// <remarks>
    /// To post a message in the message queue associated with a thread, use the PostThreadMessage function.
    /// </remarks>
    /// 
    /// <param name="hWnd"> A handle to the window whose window procedure is to receive the message. 
    /// The following values have special meanings.
    /// <list type="bullet">
    /// <item><b>HWND_BROADCAST</b>((HWND)0xffff)<br/> The message is posted to all top-level windows 
    ///    in the system, including disabled or invisible unowned windows, overlapped windows, 
    ///    and pop-up windows. The message is not posted to child windows.</item>
    /// <item><b>IntPtr.Zero</b><br/> The function behaves like a call to PostThreadMessage with 
    ///    the dwThreadId parameter set to the identifier of the current thread.</item>
    /// </list>
    /// </param>
    /// <param name="Msg"> The message to be posted. </param>
    /// <param name="wParam"> Additional message-specific information. </param>
    /// <param name="lParam"> Additional message-specific information. </param>
    /// <returns> true if it succeeds, false if it fails. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    /// <summary> Defines a new window message that is guaranteed to be unique throughout the system. 
    ///  The message value can be used when sending or posting messages. </summary>
    /// <param name="lpStr"> The message to be registered. </param>
    /// <returns> If the message is successfully registered, the return value is a message identifier 
    ///  in the range 0xC000 through 0xFFFF. If the function fails, the return value is zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    ///  </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern uint RegisterWindowMessage(
        [MarshalAs(UnmanagedType.LPWStr)] string lpStr);

    /// <summary>
    /// Converts the client-area coordinates of a specified point to screen coordinates.
    /// </summary>
    /// <param name="hWnd"> A handle to the window whose client area will be used for the conversion. </param>
    /// <param name="lpPoint"> [in,out] A reference to a POINT structure that specifies the client
    ///  coordinates to be converted, and receives the screen coordinates. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

    /// <summary>
    /// Converts the screen coordinates of a specified point on the screen to client-area coordinates.
    /// </summary>
    /// <param name="hWnd"> A handle to the window whose client area will be used for the conversion. </param>
    /// <param name="lpPoint"> [in,out] A reference to a POINT structure that specifies the screen
    ///  coordinates to be converted, and receives the client coordinates. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

    /// <summary> Retrieves the dimensions of the bounding rectangle of the specified window. 
    /// The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
    /// </summary>
    /// <param name="hWnd"> A handle to the investigated window. </param>
    /// <param name="lpRect"> [in,out] A RECT structure that receives the screen coordinates 
    /// of the upper-left and lower-right corners of the window. </param>
    /// <returns> true if it succeeds, false if it fails. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

    /// <summary> Retrieves the coordinates of a window's client area. The client coordinates specify 
    /// the upper-left and lower-right corners of the client area. 
    /// Because client coordinates are relative to the upper-left corner of a window's client area, 
    /// the coordinates of the upper-left corner are (0,0).
    /// </summary>
    /// <param name="hWnd"> A handle to the window. </param>
    /// <param name="lpRect"> [in,out] A RECT structure that receives the client coordinates. 
    /// The left and top members are zero. The right and bottom members contain the width and height 
    /// of the window. </param>
    /// <returns> true if it succeeds, false if it fails.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);

    /// <summary>
    /// The WindowFromPoint function retrieves a handle to the window that contains the specified point.
    /// </summary>
    /// <remarks>
    /// Do NOT use this overload on x64-based system. The overload IntPtr WindowFromPoint(POINT Point)
    /// always works in both 32/64 bit process. The overload IntPtr WindowFromPoint(int x, int y) works in
    /// 32-bit but fails in 64-bit process. See also <see cref="User32.WndFromPoint"/> method.
    /// </remarks>
    /// <param name="xPoint"> The x-coordinate of the point to be checked. </param>
    /// <param name="yPoint"> The y-coordinate of the point to be checked. </param>
    /// <returns>
    /// The return value is a handle to the window that contains the point. If no window exists at the
    /// given point, the return value is IntPtr.Zero.
    /// </returns>
    [DllImport("user32")]
    private static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

    /// <summary>
    /// The WindowFromPoint function retrieves a handle to the window that contains the specified point.
    /// </summary>
    /// <remarks>
    /// This WindowFromPoint overload works both on on X86 / X64 systems. See also
    /// <see cref="User32.WndFromPoint"/> method.
    /// </remarks>
    /// <param name="point"> The point to be checked. </param>
    /// <returns>
    /// The return value is a handle to the window that contains the point. If no window exists at the
    /// given point, the return value is IntPtr.Zero.
    /// </returns>
    [DllImport("user32")]
    public static extern IntPtr WindowFromPoint(POINT point);

    /// <summary> Retrieves a handle to the specified window's parent or owner. </summary>
    /// <param name="hWnd"> A handle to the window whose parent window handle is to be retrieved. </param>
    /// <returns>
    /// A handle to the parent window. If the function fails, the return value is IntPtr.Zero. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr GetParent(IntPtr hWnd);

    /// <summary> Changes the parent window of the specified child window. </summary>
    /// <param name="hWndChild"> A handle to the child window. </param>
    /// <param name="hWndNewParent"> A handle to the new parent window. If this parameter is IntPtr.Zero, 
    ///  the desktop window becomes the new parent window. 
    ///  If this parameter is HWND_MESSAGE, the child window becomes a message-only window. </param>
    /// <returns> If the function succeeds, the return value is a handle to the previous parent window.
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr SetParent(
        IntPtr hWndChild,
        IntPtr hWndNewParent);

    /// <summary>
    /// The PtInRect function determines whether the specified point lies within the specified rectangle. A
    /// point is within a rectangle if it lies on the left or top side or is within all four sides. A point
    /// on the right or bottom side is considered outside the rectangle.
    /// </summary>
    /// <remarks>
    /// The rectangle must be normalized before PtInRect is called. That is, lprc.right must be greater 
    /// than lprc.left and lprc.bottom must be greater than lprc.top. 
    /// If the rectangle is not normalized, a point is never considered inside of the rectangle.
    /// </remarks>
    /// <param name="r"> [in,out] A reference to a RECT structure that contains the specified rectangle. </param>
    /// <param name="p"> A POINT structure that contains the specified point. </param>
    /// <returns>
    /// If the specified point lies within the rectangle, the return value is true, otherwise false.
    /// </returns>
    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PtInRect(ref RECT r, POINT p);

    /// <summary>
    /// The IntersectRect function calculates the intersection of two source rectangles and places the
    /// coordinates of the intersection rectangle into the destination rectangle. If the source rectangles
    /// do not intersect, an empty rectangle (in which all coordinates are set to zero) is placed into the
    /// destination rectangle.
    /// </summary>
    /// <param name="lpDestRect"> [out] A reference to the RECT structure that is to receive the
    ///  intersection of the <paramref name="lpSrc1Rect"/> and <paramref name="lpSrc2Rect"/> rectangles. </param>
    /// <param name="lpSrc1Rect"> [in] Source 1 rectangle. </param>
    /// <param name="lpSrc2Rect"> [in] Source 2 rectangle. </param>
    /// <returns> If the rectangles intersect, the return value is true, otherwise false. </returns>
    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IntersectRect(
        ref RECT lpDestRect,
        ref RECT lpSrc1Rect,
        ref RECT lpSrc2Rect);

    /// <summary> Sets window position. </summary>
    /// <param name="hWnd"> A handle to the window to be positioned. </param>
    /// <param name="hWndInsertAfter"> A handle to the window to precede the positioned window in the Z
    ///  order. This parameter must be a window handle, or one of the specific values of the enum
    ///  <see cref="Win32.SWP_Vals"/> </param>
    /// <param name="X"> The new position of the left side of the window, in client coordinates. </param>
    /// <param name="Y"> The new position of the top of the window, in client coordinates. </param>
    /// <param name="cx"> The new width of the window, in pixels. </param>
    /// <param name="cy"> The new height of the window, in pixels. </param>
    /// <param name="uFlags"> The window sizing and positioning flags. This parameter can be a combination
    ///  of the of the values of <see cref="Win32.SWP_Flags"/> enum. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags);

    /// <summary>
    /// Changes the position and dimensions of the specified window. For a top-level window, the position
    /// and dimensions are relative to the upper-left corner of the screen. For a child window, they are
    /// relative to the upper-left corner of the parent window's client area.
    /// </summary>
    /// <param name="hWnd"> A handle to the window to be moved. </param>
    /// <param name="x"> The new position of the left side of the window. </param>
    /// <param name="y"> The new position of the top of the window. </param>
    /// <param name="cx"> The new width of the window. </param>
    /// <param name="cy"> The new height of the window. </param>
    /// <param name="repaint"> The repaint. </param>
    /// <returns> true if it succeeds, false if it fails. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633534(v=vs.85).aspx">
    /// MoveWindow documentation on MSDN</seealso>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool MoveWindow(
      IntPtr hWnd,
      int x,
      int y,
      int cx,
      int cy,
      [MarshalAs(UnmanagedType.Bool)] bool repaint);

    /// <summary>
    /// Retrieves information about the specified window. The function also retrieves the 32-bit (DWORD)
    /// value at the specified offset into the extra window memory.
    /// </summary>
    /// <remarks>
    /// Note  If you are retrieving a pointer or a handle, this function has been superseded by the
    /// <see cref="GetWindowLongPtr"/> function. (Pointers and handles are 32 bits on 32-bit Windows and 64
    /// bits on 64-bit Windows.)
    /// To write code that is compatible with both 32-bit and 64-bit versions of Windows, use
    /// GetWindowLongPtr.
    /// </remarks>
    /// <param name="hWnd"> A handle to the window. </param>
    /// <param name="nIndex"> The zero-based offset to the value to be retrieved. Valid values are in the
    ///  range zero through the number of bytes of extra window memory, minus four;
    ///  for example, if you specified 12 or more bytes of extra memory, a value of 8 would be an index to
    ///  the third 32-bit integer. <br/>
    ///  To retrieve any other value, specify one of the negative values of <see cref="GwlIndex"/> enum. 
    ///  </param>
    /// <returns>
    /// If the function succeeds, the return value is the requested value. If the function fails, the
    /// return value is zero. To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633584(v=vs.85).aspx">
    /// GetWindowLong documentation on MSDN</seealso>
    /// <seealso cref="SetWindowLong"/>
    [DllImport("user32", SetLastError = true)]
    public static extern int GetWindowLong(
        IntPtr hWnd,
        int nIndex);

    /// <summary>
    /// Changes an attribute of the specified window. The function also sets the 32-bit (long) value at the
    /// specified offset into the extra window memory.
    /// </summary>
    /// <param name="hWnd"> A handle to the window. </param>
    /// <param name="nIndex"> The zero-based offset to the value to be set. Valid values are in the range 
    /// zero through the number of bytes of extra window memory, minus the size of an integer. 
    ///  To retrieve any other value, specify one of the negative values of <see cref="GwlIndex"/> enum. 
    /// </param>
    /// <param name="dwNewLong"> The replacement value. </param>
    /// <returns> If the function succeeds, the return value is the previous value of the specified 32-bit integer. 
    /// If the function fails, the return value is zero. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// 
    /// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms633591(v=vs.85).aspx">
    /// SetWindowLong documentation on MSDN
    /// </seealso>
    /// <seealso cref="GetWindowLong"/>
    [DllImport("user32", SetLastError = true)]
    public static extern int SetWindowLong(
        IntPtr hWnd,
        int nIndex,
        int dwNewLong);

    /// <summary>
    /// The helper for the actual public method SetWindowLongPtr.
    /// It should be called only in 32-bit process, when the size of 
    /// the last argument declared here ( WndProcDelegate ) is matching
    /// the 4-byte size of declaration of the native Win32 API
    /// <code> LONG SetWindowLong(int nIndex, LONG dwNewLong) </code>
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="nIndex"></param>
    /// <param name="newProc"></param>
    /// <returns></returns>
    [DllImport("user32", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLongPtr32(
        IntPtr hWnd,
        int nIndex,
        WndProcDelegate newProc);

    /// <summary> Retrieves the name of the Win32 class to which the specified window belongs. </summary>
    /// <remarks>
    /// Declaration on purpose should use CharSet.Ansi to make it work on x64. see also
    /// http://www.pinvoke.net/default.aspx/user32.getclassname.
    /// </remarks>
    /// 
    /// <param name="hWnd"> A handle to the examined window. </param>
    /// <param name="lpClassName"> The buffer that will receive the class name. </param>
    /// <param name="nMaxCount"> The maximum number of characters to copy to the buffer, including the
    ///  null character. If the text exceeds this limit, it is truncated. </param>
    /// <returns>
    /// If the function succeeds, the return value is the number of characters copied to the buffer, not
    /// including the terminating null character. If the function fails, the return value is zero. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", CharSet = CharSet.Ansi, EntryPoint = "GetClassNameA",
      SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    public static extern int GetClassName(
        IntPtr hWnd,
        [MarshalAs(UnmanagedType.LPStr)] System.Text.StringBuilder lpClassName,
        int nMaxCount);

    /// <summary>
    /// The GetScrollRange function retrieves the current minimum and maximum scroll box (thumb) positions
    /// for the specified scroll bar.
    /// </summary>
    /// <param name="hWnd"> Handle to a scroll bar control or a window with a standard scroll bar,
    ///  depending on the value of the <paramref name="nBar"/> parameter. </param>
    /// <param name="nBar"> Specifies the scroll bar to be set. </param>
    /// <param name="lpMinPos"> [out] Reference to the integer variable that receives the minimum position. </param>
    /// <param name="lpMaxPos"> [out] Reference to the integer variable that receives the maximum
    ///  position. </param>
    /// <returns> If the function succeeds, the return value is nonzero.
    /// If the function fails, the return value is zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern int GetScrollRange(IntPtr hWnd, int nBar, ref int lpMinPos, ref int lpMaxPos);

    /// <summary>
    /// The GetScrollInfo function retrieves the parameters of a scroll bar, including the minimum and
    /// maximum scrolling positions, the page size, and the position of the scroll box (thumb).
    /// </summary>
    /// <param name="hWnd"> Handle to a scroll bar control or a window with a standard scroll bar,
    ///  depending on the value of the fnBar parameter. </param>
    /// 
    /// <param name="nBar"> Specifies the type of scroll bar for which to retrieve parameters. </param>
    /// <param name="lpScrollInfo"> [in,out]. A reference to a SCROLLINFO structure. 
    /// Before calling GetScrollInfo, set the cbSize member to sizeof(SCROLLINFO), 
    /// and set the fMask member to specify the scroll bar parameters to retrieve. 
    /// Before returning, the function copies the specified parameters to the appropriate members 
    /// of the structure. </param>
    /// <returns> If the function retrieved any values, the return value is nonzero. 
    /// If the function does not retrieve any values, the return value is zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// <seealso cref="GetWndScrollInfo"/>
    [DllImport("user32", SetLastError = true)]
    public static extern int GetScrollInfo(IntPtr hWnd, int nBar, ref SCROLLINFO lpScrollInfo);

    /// <summary>
    /// Retrieves a handle to the desktop window. The desktop window covers the entire screen. The desktop
    /// window is the area on top of which other windows are painted.
    /// </summary>
    /// <returns> The desktop window handle. </returns>
    [DllImport("user32", EntryPoint = "GetDesktopWindow")]
    public static extern IntPtr GetDesktopWindow();

    /// <summary> The GetDC function retrieves a handle to a device context (DC) for the client area 
    /// of a specified window or for the entire screen. 
    /// You can use the returned handle in subsequent GDI functions to draw in the DC. 
    /// The device context is an opaque data structure, whose values are used internally by GDI.
    /// </summary>
    /// <param name="hwnd"> A handle to the window whose DC is to be retrieved. If this value is IntPtr.Zero,
    /// GetDC retrieves the DC for the entire screen. </param>
    /// <returns> If the function succeeds, the return value is a handle to the DC for the specified 
    ///  window's client area. If the function fails, the return value is IntPtr.Zero.
    /// </returns>
    /// <seealso cref="GetWindowDC"/>
    [DllImport("user32", EntryPoint = "GetDC")]
    public static extern IntPtr GetDC(IntPtr hwnd);

    /// <summary> Gets window device context. </summary>
    /// <remarks>
    /// The GetWindowDC function retrieves the device context (DC) for the entire window, including 
    /// title bar, menus, and scroll bars. A window device context permits painting anywhere in a window, 
    /// because the origin of the device context is the upper-left corner of the window 
    /// instead of the client area.
    /// </remarks>
    /// <param name="hWnd"> A handle to the window with a device context that is to be retrieved. 
    /// If this value is IntPtr.Zero, GetWindowDC retrieves the device context for the entire screen.
    /// </param>
    /// <returns> The window device context. </returns>
    /// <seealso cref="GetDC"/>
    [DllImport("user32", EntryPoint = "GetWindowDC")]
    public static extern IntPtr GetWindowDC(IntPtr hWnd);

    /// <summary> The ReleaseDC function releases a device context (DC), freeing it for use by other 
    /// applications. The effect of the ReleaseDC function depends on the type of DC.
    /// It frees only common and window DCs. It has no effect on class or private DCs. </summary>
    /// <param name="hWnd"> A handle to the window whose DC is to be released. </param>
    /// <param name="hDc"> The device-context to be released. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("user32", EntryPoint = "ReleaseDC")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

    /// <summary> Retrieves the specified system metric or system configuration setting. </summary>
    /// <param name="nIndex"> The system metric or configuration setting to be retrieved. </param>
    /// <returns>
    /// If the function succeeds, the return value is the requested system metric or configuration setting.
    /// If the function fails, the return value is 0.
    /// </returns>
    [DllImport("user32", EntryPoint = "GetSystemMetrics")]
    public static extern int GetSystemMetrics(Win32.SM nIndex);

    /// <summary>
    /// The BeginPaint function prepares the specified window for painting and fills a
    /// <see cref="PAINTSTRUCT"/> structure with information about the painting.
    /// </summary>
    /// <param name="hwnd"> A Handle to the window to be repainted. </param>
    /// <param name="lpPaint"> [in,out] A reference to <see cref="PAINTSTRUCT"/> structure that will
    ///  receive the painting information. </param>
    /// <returns>
    /// If the function succeeds, the return value is the handle to a display device context for the
    /// specified window. If the function fails, the return value is IntPtr.Zero, indicating that no
    /// display device context is available.
    /// </returns>
    [DllImport("user32")]
    public static extern IntPtr BeginPaint(IntPtr hwnd, ref PAINTSTRUCT lpPaint);

    /// <summary>
    /// The EndPaint function marks the end of painting in the specified window. This function is required
    /// for each call to the <see cref="BeginPaint "/> function, but only after painting is complete.
    /// </summary>
    /// <param name="hwnd"> Handle to the window that has been repainted. </param>
    /// <param name="lpPaint"> [in,out] A reference to <see cref="PAINTSTRUCT"/> structure that contains 
    ///  the painting information retrieved by <see cref="BeginPaint"/>. </param>
    /// <returns> The return value is always nonzero. </returns>
    [DllImport("user32")]
    public static extern int EndPaint(IntPtr hwnd, ref PAINTSTRUCT lpPaint);

    /// <summary> Defines a system-wide hot key. </summary>
    /// 
    /// <param name="hWnd"> A handle to the window that will receive WM_HOTKEY messages generated by the
    ///  hot key. If this parameter is IntPtr.Zero, WM_HOTKEY messages are posted to the message queue of
    ///  the calling thread and must be processed in the message loop. </param>
    /// 
    /// <param name="id"> The identifier of the hot key. If the hWnd parameter is IntPtr.Zero, then the hot
    ///  key is associated with the current thread rather than with a particular window. If a hot key
    ///  already exists with the same hWnd and id parameters, see Remarks for the action taken. </param>
    /// 
    /// <param name="fsModifiers"> The keys that must be pressed in combination with the key specified by
    ///  the <paramref name="vk"/> parameter in order to generate the WM_HOTKEY message. The fsModifiers
    ///  parameter can be a combination of the following values. </param>
    /// <param name="vk"> The virtual-key code of the hot key. </param>
    /// 
    /// <returns>
    /// true if it succeeds, false if it fails. To get extended error information, call
    /// Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    /// <summary> Frees a hot key previously registered by the calling thread. </summary>
    /// <param name="hWnd"> A handle to the window associated with the hot key to be freed. This parameter 
    /// should be IntPtr.Zero if the hot key is not associated with a window. </param>
    /// <param name="id"> The identifier of the hot key to be freed. </param>
    /// <returns> true if it succeeds, false if it fails. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// <see cref="RegisterHotKey"/>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    /// <summary>
    /// Retrieves the status of the specified virtual key. The status specifies whether the key is up, down,
    /// or toggled (on, off—alternating each time the key is pressed).
    /// </summary>
    /// <param name="keyCode"> The key code, i.e. a virtual key. If the desired virtual key is a letter or
    ///  digit (A through Z, a through z, or 0 through 9), nVirtKey must be set to the ASCII value of that
    ///  character. For other keys, it must be a virtual-key code.
    ///  
    ///  If a non-English keyboard layout is used, virtual keys with values in the range ASCII A through Z
    ///  and 0 through 9 are used to specify most of the character keys. For example, for the German
    ///  keyboard layout, the virtual key of value ASCII O (0x4F) refers to the "o" key, whereas VK_OEM_1
    ///  refers to the "o with umlaut" key. </param>
    /// <returns>
    /// The return value specifies the status of the specified virtual key, as follows:
    /// <list type="bullet">
    /// <item>If the high-order bit is 1, the key is down; otherwise, it is up. </item>
    /// <item> If the low-order bit is 1, the key is toggled. A key, such as the CAPS LOCK key, is toggled
    /// if it is turned on. The key is off and untoggled if the low-order bit is 0. A toggle key's
    /// indicator light (if any) on the keyboard will be on when the key is toggled, and off when the key
    /// is untoggled. </item>
    /// </list>
    /// </returns>
    [DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true,
      CallingConvention = CallingConvention.Winapi)]
    public static extern short GetKeyState(int keyCode);

    /// <summary>
    /// Installs an application-defined hook procedure into a hook chain. You would install a hook procedure 
    /// to monitor the system for certain types of events. These events are associated either
    /// with a specific thread or with all threads in the same desktop as the calling thread.
    /// or with all threads in the same desktop as the calling thread.
    /// </summary>
    ///
    /// <param name="code"> The type of hook procedure to be installed. 
    ///                     For more info, see <see cref="Win32.HookType"/> enum. </param>
    /// 
    /// <param name="func">       A delegate encapsulating the hook procedure. </param>
    /// 
    /// <param name="hInstance"> A Win32 handle to the DLL containing the hook procedure encapsulated by the
    /// <paramref name="func"/> argument. <br/>
    /// The hInstance parameter must be set to null if the threadID parameter specifies a thread created
    /// by the current process and if the hook procedure is within the code associated with the current process. 
    /// </param>
    /// 
    /// <param name="threadID">  The identifier of the thread with which the hook procedure is to be associated.
    /// For desktop apps, if this parameter is zero, the hook procedure is associated with all existing threads
    /// running in the same desktop as the calling thread. </param>
    ///
    /// <returns> If the function succeeds, the return value is the handle to the hook procedure.
    /// If the function fails, the return value is IntPtr.Zero.
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(
        Win32.HookType code,
        HookProc func,
        IntPtr hInstance,
        uint threadID);

    /// <summary>
    /// Removes a hook procedure installed in a hook chain by the <see cref="SetWindowsHookEx"/>SetWindowsHookEx
    /// function.
    /// </summary>
    /// <param name="hhook"> A handle to the hook to be removed. This parameter is a hook handle obtained by a
    ///  previous call to <see cref="SetWindowsHookEx"/>SetWindowsHookEx. </param>
    /// <returns>
    /// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern int UnhookWindowsHookEx(IntPtr hhook);

    /// <summary>
    /// Passes the hook information to the next hook procedure in the current hook chain. A hook procedure can call
    /// this function either before or after processing the hook information.
    /// </summary>
    /// <param name="hhook"> A handle to the hook. This parameter is a hook handle obtained by a previous call to
    ///  <see cref="SetWindowsHookEx"/>SetWindowsHookEx. </param>
    /// <param name="code"> The hook code passed to the current hook procedure. The next hook procedure uses this
    ///  code to determine how to process the hook information. </param>
    /// <param name="wParam"> The wParam value passed to the current hook procedure. The meaning of this parameter
    ///  depends on the type of hook associated with the current hook chain. </param>
    /// <param name="lParam"> The lParam value passed to the current hook procedure. The meaning of this parameter
    ///  depends on the type of hook associated with the current hook chain. </param>
    /// <returns>
    /// This value is returned by the next hook procedure in the chain. The current hook procedure must also return
    /// this value. The meaning of the return value depends on the hook type. For more information, see the
    /// descriptions of the individual hook procedures.
    /// </returns>
    [DllImport("user32")]
    public static extern IntPtr CallNextHookEx(IntPtr hhook,
        int code, IntPtr wParam, IntPtr lParam);

    /// <summary> The ShowScrollBar function shows or hides the specified scroll bar. </summary>
    /// <param name="hWnd"> Handle to a scroll bar control or a window with a standard scroll bar, 
    /// depending on the value of the wBar parameter. </param>
    /// <param name="wBar"> Specifies the scroll bar(s) to be shown or hidden. </param>
    /// <param name="bShow"> Specifies whether the scroll bar is shown or hidden. 
    /// If this parameter is nonzero, the scroll bar is shown; otherwise, it is hidden. </param>
    /// <returns> If the function succeeds, the return value is nonzero.
    /// To get extended error information, call Marshal.GetLastWin32Error(). </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern int ShowScrollBar(
        IntPtr hWnd, int wBar, int bShow);

    /// <summary> The EnableScrollBar function enables or disables one or both scroll bar arrows.
    /// </summary>
    /// <param name="hWnd"> Handle to a scroll bar control or a window with a standard scroll bar, 
    /// depending on the value of the wBar parameter. </param>
    /// <param name="wSBflags"> Specifies the scroll bar type. </param>
    /// <param name="wArrows"> Specifies whether the scroll bar arrows are enabled or disabled 
    /// and indicates which arrows are enabled or disabled.</param>
    /// <returns> If the arrows are enabled or disabled as specified, the return value is nonzero.
    /// If the arrows are already in the requested state or an error occurs, the return value is zero.
    /// To get extended error information, call Marshal.GetLastWin32Error(). </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern int EnableScrollBar(
        IntPtr hWnd, uint wSBflags, uint wArrows);

    /// <summary> Sets the minimum and maximum scroll box positions for the specified scroll bar. </summary>
    /// <remarks>
    /// The SetScrollRange function is provided for backward compatibility. New applications should use the
    /// SetScrollInfo function.
    /// </remarks>
    /// <param name="hWnd"> Handle to a scroll bar control or a window with a standard scroll bar,
    ///  depending on the value of the nBar parameter. </param>
    /// <param name="nBar"> Specifies the scroll bar to be set. </param>
    /// <param name="nMinPos"> Specifies the minimum scrolling position. </param>
    /// <param name="nMaxPos"> Specifies the maximum scrolling position. </param>
    /// <param name="bRedraw"> Specifies whether the scroll bar should be redrawn to reflect the change. 
    /// </param>
    /// <returns> If the function succeeds, the return value is nonzero.
    /// To get extended error information, call Marshal.GetLastWin32Error(). </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern int SetScrollRange(
        IntPtr hWnd, int nBar, int nMinPos, int nMaxPos, int bRedraw);

    /// <summary> The SetScrollPos function sets the position of the scroll box (thumb) in the specified 
    /// scroll bar and, if requested, redraws the scroll bar to reflect the new position of the scroll box.
    /// </summary>
    /// <param name="hWnd"> Handle to a scroll bar control or a window with a standard scroll bar,
    /// depending on the value of the nBar parameter. </param>
    /// <param name="nBar"> Specifies the scroll bar to be set. </param>
    /// <param name="nPos"> Specifies the new position of the scroll box. 
    /// The position must be within the scrolling range. </param>
    /// <param name="bRedraw"> Specifies whether the scroll bar should be redrawn to reflect the change. 
    /// </param>
    /// <returns> If the function succeeds, the return value is the previous position of the scroll box.
    /// </returns>
    [DllImport("user32")]
    public static extern int SetScrollPos(
        IntPtr hWnd, int nBar, int nPos, int bRedraw);

    /// <summary>
    /// The InvalidateRect function adds a rectangle to the specified window's update region. The update
    /// region represents the portion of the window's client area that must be redrawn.
    /// </summary>
    /// <param name="hWnd"> A handle to the window whose update region has changed. If this parameter is
    ///  IntPtr.Zero, the system invalidates and redraws all windows, not just the windows for this
    ///  application, and sends the WM_ERASEBKGND and WM_NCPAINT messages before the function returns.
    ///  Setting this parameter to IntPtr.Zero is not recommended. </param>
    /// <param name="lpRect"> [in] A pointer to a RECT structure that that contains the client
    ///  coordinates of the rectangle to be added to the update region. <br/>
    ///  To added the entire client area to the update region, one should call InvalidateRect API with
    ///  this parameter se to NULL, which is impossible in C#. Instead, you should use the declaration of
    ///  <see cref="InvalidateRect2"/> for that purpose. </param>
    /// <param name="bErase"> Specifies whether the background within the update region is to be erased
    ///  when the update region is processed. </param>
    /// <returns> If the function succeeds, the return value is nonzero; otherwise it is zero. </returns>
    /// 
    /// <seealso cref="InvalidateRect2"/>
    [DllImport("user32")]
    public static extern int InvalidateRect(
      IntPtr hWnd,
      ref RECT lpRect,
      [MarshalAs(UnmanagedType.Bool)] bool bErase);

    /// <summary>
    /// Should be used to add the entire client area a client area to the specified window's update region.
    /// The update region represents the portion of the window's client area that must be redrawn.
    /// </summary>
    /// <remarks>
    /// This declaration is work-around that allows to call InvalidateRect API with the NULL as a second
    /// argument. Otherwise, I cannot call InvalidateRect(hWnd, null, ... because of the compiler error
    /// CS0037: <br/>
    /// error CS0037: Cannot convert null to '...Win32.RECT' because it is a value type.
    /// </remarks>
    /// <param name="hWnd"> A handle to the window whose update region has changed. </param>
    /// <param name="thatArgumentShouldHaveZeroValue"> Should have zero value. </param>
    /// <param name="bErase"> Specifies whether the background within the update region is to be erased
    ///  when the update region is processed. </param>
    /// <returns> If the function succeeds, the return value is nonzero; otherwise it is zero. </returns>
    /// 
    /// <seealso cref="InvalidateRect"/>
    [DllImport("user32", EntryPoint = "InvalidateRect")]
    public static extern int InvalidateRect2(
      IntPtr hWnd,
      int thatArgumentShouldHaveZeroValue,
      [MarshalAs(UnmanagedType.Bool)] bool bErase);

    /// <summary> The FillRect function fills a rectangle by using the specified brush. This function 
    /// includes the left and top borders, but excludes the right and bottom borders of the rectangle.
    /// </summary>
    /// <param name="hdc"> A handle to the device context in which the rectangle is drawn. </param>
    /// <param name="lpRect"> [in] A pointer to a RECT structure that contains the logical coordinates
    ///  of the upper-left and lower-right corners of the rectangle to be filled. </param>
    /// <param name="hBrush"> A handle to the brush used to fill the rectangle. </param>
    /// <returns> If the function succeeds, the return value is nonzero; otherwise it is zero.
    /// </returns>
    [DllImport("user32")]
    public static extern int FillRect(IntPtr hdc, ref RECT lpRect, IntPtr hBrush);

    /// <summary> The UpdateWindow function updates the client area of the specified window by sending 
    ///  a WM_PAINT message to the window if the window's update region is not empty. 
    ///  The function sends a WM_PAINT message directly to the window procedure of the specified window, 
    ///  bypassing the application queue. If the update region is empty, no message is sent.
    ///  </summary>
    /// <param name="hwnd"> A Handle to the window to be updated.</param>
    /// <returns> If the function succeeds, the return value is nonzero; otherwise it is zero.
    /// </returns>
    [DllImport("user32")]
    public static extern int UpdateWindow(IntPtr hwnd);

    /// <summary>
    /// Creates a new shape for the system caret and assigns ownership of the caret to the specified
    /// window. The caret shape can be a line, a block, or a bitmap.
    /// </summary>
    /// <param name="hwnd"> Handle to the window that owns the caret. </param>
    /// <param name="hBitmap"> A handle to the bitmap that defines the caret shape. 
    /// If this parameter is IntPtr.Zero, the caret is solid. If this parameter is (HBITMAP)1, 
    /// the caret is gray. </param>
    /// <param name="nWidth"> The width of the caret, in logical units. 
    /// If this parameter is zero, the width is set to the system-defined window border width. </param>
    /// <param name="nHeight"> The height of the caret, in logical units. If this parameter is zero, 
    /// the height is set to the system-defined window border height. </param>
    /// <returns> If the function succeeds, the return value is true; otherwise it is false.
    /// To get extended error information, call Marshal.GetLastWin32Error(). </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CreateCaret(IntPtr hwnd, IntPtr hBitmap, int nWidth, int nHeight);

    /// <summary> Makes the caret visible on the screen at the caret's current position. 
    ///           When the caret becomes visible, it begins flashing automatically. </summary>
    /// <param name="hwnd"> A handle to the window that owns the caret. If this parameter is IntPtr.Zero, 
    ///  ShowCaret searches the current task for the window that owns the caret. </param>
    /// <returns> If the function succeeds, the return value is nonzero; otherwise it is zero.
    /// To get extended error information, call Marshal.GetLastWin32Error(). </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern int ShowCaret(IntPtr hwnd);

    /// <summary>
    /// Removes the caret from the screen. Hiding a caret does not destroy its current shape or invalidate
    /// the insertion point.
    /// </summary>
    /// <param name="hwnd"> A handle to the window that owns the caret. If this parameter is IntPtr.Zero, 
    /// HideCaret searches the current task for the window that owns the caret. </param>
    /// <returns> If the function succeeds, the return value is nonzero; otherwise it is zero.
    /// To get extended error information, call Marshal.GetLastWin32Error(). </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern int HideCaret(IntPtr hwnd);

    /// <summary> Moves the caret to the specified coordinates. If the window that owns the caret was
    /// created with the CS_OWNDC class style, then the specified coordinates are subject to the mapping 
    /// mode of the device context associated with that window.
    /// </summary>
    /// <param name="x"> The new x-coordinate of the caret.</param>
    /// <param name="y"> The new y-coordinate of the caret. </param>
    /// <returns> If the function succeeds, the return value is nonzero; otherwise it is zero.
    /// To get extended error information, call Marshal.GetLastWin32Error(). </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern int SetCaretPos(int x, int y);

    /// <summary> Copies the caret's position to the specified POINT structure. </summary>
    /// <param name="lpPoint"> [out] A reference to a POINT structure that receives the client coordinates
    ///  of the caret. </param>
    /// <returns> If the function succeeds, the return value is true; otherwise it is false.
    /// To get extended error information, call Marshal.GetLastWin32Error(). </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCaretPos(ref Point lpPoint);

    /// <summary> Creates a timer with the specified time-out value. </summary>
    /// <param name="hwnd"> An optional handle to the window to be associated with the timer. This window
    ///  must be owned by the calling thread. If an IntPtr.Zero value for hWnd is passed in along with an
    ///  nIDEvent of an existing timer, that timer will be replaced in the same way that an existing non-
    ///  NULL hWnd timer will be. </param>
    /// <param name="nIDEvent"> A nonzero timer identifier. If the hWnd parameter is NULL, and the nIDEvent
    ///  does not match an existing timer then it is ignored and a new timer ID is generated. If the hWnd
    ///  parameter is not NULL and the window specified by hWnd already has a timer with the value
    ///  nIDEvent, then the existing timer is replaced by the new timer. When SetTimer replaces a timer,
    ///  the timer is reset. Therefore, a message will be sent after the current time-out value elapses,
    ///  but the previously set time-out value is ignored. If the call is not intended to replace an
    ///  existing timer, nIDEvent should be 0 if the hWnd is NULL. </param>
    /// <param name="uElapse"> The time-out value, in milliseconds. </param>
    /// <param name="lpTimerFunc"> A pointer to the function to be notified when the time-out value
    ///  elapses. For more information about the function, see TimerProc. If lpTimerFunc is NULL, the
    ///  system posts a WM_TIMER message to the application queue. The hwnd member of the message's MSG
    ///  structure contains the value of the hWnd parameter. </param>
    /// 
    /// <returns>
    /// If the function succeeds and the hWnd parameter is IntPtr.Zero, the return value is an integer
    /// identifying the new timer. An application can pass this value to the KillTimer function to destroy
    /// the timer.
    /// 
    /// If the function succeeds and the hWnd parameter is not IntPtr.Zero, then the return value is a
    /// nonzero integer. An application can pass the value of the nIDEvent parameter to the KillTimer
    /// function to destroy the timer.
    /// 
    /// If the function fails to create a timer, the return value is zero. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// <seealso cref="KillTimer"/>
    [DllImport("user32", SetLastError = true)]
    public static extern int SetTimer(IntPtr hwnd, int nIDEvent, int uElapse, TimerProc lpTimerFunc);

    /// <summary> Kill timer. </summary>
    /// <param name="hwnd"> A handle to the window associated with the specified timer. 
    /// This value must be the same as the hWnd value passed to the SetTimer function that created the timer.
    /// </param>
    /// <param name="nIDEvent"> The timer to be destroyed. 
    /// If the window handle passed to SetTimer is valid, 
    /// this parameter must be the same as the nIDEvent value passed to SetTimer. 
    /// If the application calls SetTimer with hWnd set to NULL, 
    /// this parameter must be the timer identifier returned by SetTimer.
    /// </param>
    /// <returns> If the function succeeds, the return value is nonzero.
    /// If the function fails, the return value is zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// <seealso cref="SetTimer"/>
    [DllImport("user32", SetLastError = true)]
    public static extern int KillTimer(IntPtr hwnd, int nIDEvent);

    /// <summary> This function scrolls the contents of the specified window's client area. </summary>
    /// <param name="hwnd"> Handle to the window where the client area is to be scrolled. </param>
    /// <param name="dx"> Specifies the amount, in device units, of horizontal scrolling. This parameter
    ///  must be a negative value to scroll to the left. </param>
    /// <param name="dy"> Specifies the amount, in device units, of vertical scrolling. This parameter must
    ///  be a negative value to scroll up. </param>
    /// <param name="lprcScroll"> [in] Reference to a RECT structure that specifies the portion of the
    ///  client area to be scrolled. If this parameter is NULL, the entire client area is scrolled. </param>
    /// <param name="lprcClip"> [in] Reference to a RECT structure that contains the coordinates of the
    ///  clipping rectangle. Only device bits within the clipping rectangle are affected. Bits scrolled
    ///  from the outside of the rectangle to the inside are painted; bits scrolled from the inside of the
    ///  rectangle to the outside are not painted. This parameter may be NULL. </param>
    /// <param name="hrgnUpdate"> [in] Handle to the region that is modified to hold the region invalidated
    ///  by scrolling. This parameter may be IntPtr.Zero. </param>
    /// <param name="lprcUpdate"> [in] Pointer to a RECT structure that receives the boundaries of the
    ///  rectangle invalidated by scrolling. This parameter may be NULL. </param>
    /// <param name="fuScroll"> Specifies flags that control scrolling. This parameter can be a combination 
    /// of the values SW_ERASE, SW_INVALIDATE, SW_SCROLLCHILDREN, SW_SMOOTHSCROLL. </param>
    /// <returns> If the function succeeds, the return value is SIMPLEREGION (rectangular invalidated region), 
    /// COMPLEXREGION (nonrectangular invalidated region; overlapping rectangles), 
    /// or NULLREGION (no invalidated region).
    /// If the function fails, the return value is ERROR.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern int ScrollWindowEx(
      IntPtr hwnd,
      int dx,
      int dy,
      ref RECT lprcScroll,
      ref RECT lprcClip,
      IntPtr hrgnUpdate,
      ref RECT lprcUpdate,
      int fuScroll);

    /// <summary> Sets the mouse capture to the specified window belonging to the current thread. </summary>
    /// <param name="hwnd"> A handle to the window in the current thread that is to capture the mouse. </param>
    /// <returns>
    /// The return value is a handle to the window that had previously captured the mouse. If there is no
    /// such window, the return value is IntPtr.Zero.
    /// </returns>
    [DllImport("user32")]
    public static extern IntPtr SetCapture(IntPtr hwnd);

    /// <summary> Releases the mouse capture from a window in the current thread and restores 
    /// normal mouse input processing. . </summary>
    /// <returns> If the function succeeds, the return value is nonzero. 
    /// If the function fails, the return value is zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern int ReleaseCapture();

    /// <summary> Retrieves a handle to the window (if any) that has captured the mouse. </summary>
    /// <returns> The return value is a handle to the capture window associated with the current thread. 
    ///  If no window in the thread has captured the mouse, the return value is IntPtr.Zero.
    /// </returns>
    [DllImport("user32")]
    public static extern IntPtr GetCapture();

    /// <summary> Retrieves the handle to the window that has the keyboard focus, 
    /// if the window is attached to the calling thread's message queue.
    /// </summary>
    /// <returns> The return value is the handle to the window with the keyboard focus. 
    /// If the calling thread's message queue does not have an associated window with the keyboard focus, 
    /// the return value is IntPtr.Zero.
    /// </returns>
    [DllImport("user32")]
    public static extern IntPtr GetFocus();

    /// <summary> Sets the keyboard focus to the specified window. 
    /// The window must be attached to the calling thread's message queue. </summary>
    /// <param name="hWnd"> A handle to the window that will receive the keyboard input. 
    /// If this parameter is IntPtr.Zero, keystrokes are ignored.
    /// </param>
    /// <returns> If the function succeeds, the return value is the handle to the window that previously 
    /// had the keyboard focus. If the hWnd parameter is invalid or the window is not attached 
    /// to the calling thread's message queue, the return value is IntPtr.Zero. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    ///  </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr SetFocus(IntPtr hWnd);

    /// <summary> Sets the cursor shape. </summary>
    /// <param name="cursorHandle"> Handle of the cursor. The cursor must have been created by the 
    /// CreateCursor function or loaded by the <see cref="LoadCursor(IntPtr, string)"/> or 
    /// <see cref="LoadImage"/> function.
    /// If this parameter is IntPtr.Zero, the cursor is removed from the screen.
    /// </param>
    /// <returns> The return value is the handle to the previous cursor, if there was one. </returns>
    [DllImport("user32")]
    public static extern IntPtr SetCursor(IntPtr cursorHandle);

    /// <summary> Retrieves the position of the mouse cursor, in screen coordinates. </summary>
    /// <param name="lpPoint"> [out] A reference to a POINT structure that receives the screen coordinates 
    /// of the cursor. </param>
    /// <returns> Returns nonzero if successful or zero otherwise. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    ///  </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern int GetCursorPos(ref POINT lpPoint);

    /// <summary> Loads a cursor. </summary>
    /// <param name="hInstance"> A Win32 handle to the module of either a DLL or executable (.exe) 
    ///  that contains the cursor to be loaded. </param>
    /// <param name="cursorName"> Name of the cursor. </param>
    /// <returns> If the function succeeds, the return value is the handle of the newly loaded cursor.
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", EntryPoint = "LoadCursor", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr LoadCursor(
        IntPtr hInstance,
        [MarshalAs(UnmanagedType.LPWStr)] string cursorName);

    /// <summary> Loads a cursor. </summary>
    /// <param name="hInstance"> A Win32 handle to the module of either a DLL or executable (.exe) 
    ///  that contains the cursor to be loaded. </param>
    /// <param name="nResource"> The resource. </param>
    /// <returns> If the function succeeds, the return value is the handle of the newly loaded cursor.
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", EntryPoint = "LoadCursor", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr LoadCursor(
        IntPtr hInstance,
        IntPtr nResource);

    /// <summary> Loads an icon, cursor, animated cursor, or bitmap.</summary>
    /// <param name="hInstance"> A Win32 handle to the module of either a DLL or executable (.exe) 
    ///  that contains the image to be loaded. </param>
    /// 
    /// <param name="imageName"> Name of the image. 
    /// If the <paramref name="hInstance"/>hInstance parameter is non-null 
    /// and the  <paramref name="fuLoad"/> parameter omits LR_LOADFROMFILE, then lpszName specifies 
    /// the image resource in the module. 
    /// In case the imageName represents the name of resource and image resource is to be loaded by name 
    /// from the module, the lpszName parameter is a string that contains the name of the image resource. 
    /// If the image resource is to be loaded by ordinal from the module, 
    /// use the overload method with imageName argument of type IntPtr.
    /// </param>
    /// <param name="uType"> . The type of image to be copied. This parameter can be one of the following
    /// values
    /// <list type="bullet">
    /// <item><b>IMAGE_BITMAP = 0</b><br/>Loads a bitmap.</item>
    /// <item><b>IMAGE_CURSOR = 2</b><br/>Loads a cursor.</item>
    /// <item><b>IMAGE_ICON = 1</b><br/>Loads an icon.</item>
    /// </list> </param>
    ///
    /// <param name="cxDesired"> The desired width, in pixels, of the image. If this parameter is zero 
    ///  and the fuLoad parameter is LR_DEFAULTSIZE, the function uses the SM_CXICON or SM_CXCURSOR 
    ///  system metric value to set the width. If this parameter is zero and LR_DEFAULTSIZE is not used, 
    ///  the function uses the actual resource width.
    /// </param>
    /// <param name="cyDesired"> The desired height, in pixels, of the image. If this parameter is zero
    ///  and the fuLoad parameter is LR_DEFAULTSIZE, the function uses the SM_CYICON or SM_CYCURSOR
    ///  system metric value to set the width. If this parameter is zero and LR_DEFAULTSIZE is not used,
    ///  the function uses the actual resource width.
    /// </param>
    /// 
    /// <param name="fuLoad"> The flags specifying details of loading. </param>
    /// <returns> If the function succeeds, the return value is the handle of the newly loaded image.
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", EntryPoint = "LoadImage", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr LoadImage(
        IntPtr hInstance,
        [MarshalAs(UnmanagedType.LPWStr)] string imageName,
        uint uType,
        int cxDesired,
        int cyDesired,
        uint fuLoad);

    /// <summary>
    /// Retrieves the current color of the specified display element. Display elements are the parts of a
    /// window and the display that appear on the system display screen.
    /// </summary>
    /// <remarks>
    /// To get the individual components of the RGB value, use the explicit cast to <see cref="COLORREF"/>
    /// structure, and afterwards use the <see cref="COLORREF.R"/>, <see cref="COLORREF.G"/> and 
    /// <see cref="COLORREF.B"/> properties.
    /// </remarks>
    /// 
    /// <param name="nIndex"> The display element whose color is to be retrieved. This parameter can be one
    ///  of <see cref="Win32.SysColor"/> enum values. </param>
    /// <returns>
    /// The function returns the red, green, blue (RGB) color value of the given element. If the nIndex
    /// parameter is out of range, the return value is zero. Because zero is also a valid RGB value, you
    /// cannot use GetSysColor to determine whether a system color is supported by the current platform.
    /// Instead, use the GetSysColorBrush function, which returns NULL if the color is not supported.
    /// </returns>
    [DllImport("user32")]
    public static extern uint GetSysColor(Win32.SysColor nIndex);

    /// <summary>
    /// The GetDoubleClickTime function retrieves the current double-click time for the mouse. 
    /// A double-click is a series of two clicks of the mouse button, 
    /// the second occurring within a specified time after the first. 
    /// The double-click time is the maximum number of milliseconds that may occur 
    /// between the first and second click of a double-click. 
    /// </summary>
    /// <returns>The return value specifies the current double-click time, in milliseconds. </returns>
    [DllImport("user32")]
    public static extern uint GetDoubleClickTime();

    /// <summary>
    /// The SetDoubleClickTime function sets the double-click time for the mouse. 
    /// A double-click is a series of two clicks of a mouse button, 
    /// the second occurring within a specified time after the first. 
    /// The double-click time is the maximum number of milliseconds that may occur 
    /// between the first and second clicks of a double-click. 
    /// </summary>
    /// <param name="uInterval">The number of milliseconds that may occur between the first and second 
    /// clicks of a double-click. If this parameter is set to 0, the system uses the default double-click 
    /// time of 500 milliseconds. If this parameter value is greater than 5000 milliseconds, 
    /// the system sets the value to 5000 milliseconds.
    /// </param>
    /// <returns> If the function succeeds, the return value is true.
    /// If the function fails, the return value is false.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetDoubleClickTime(uint uInterval);

    /// <summary> Calls the default window procedure to provide default processing for any window messages 
    /// that an application does not process. 
    /// This function ensures that every message is processed. DefWindowProc is called with the same 
    /// parameters received by the window procedure.
    /// </summary>
    /// <param name="hwnd">  handle to the window procedure that received the message.</param>
    /// <param name="wMsg"> The message. </param>
    /// <param name="wParam"> Additional message information. 
    /// The content of this parameter depends on the value of the Msg parameter.
    /// </param>
    /// <param name="lParam"> Additional message information.
    /// The content of this parameter depends on the value of the Msg parameter.
    /// </param>
    /// <returns> The return value is the result of the message processing and depends on the message.
    /// </returns>
    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr DefWindowProc(IntPtr hwnd, uint wMsg, IntPtr wParam, IntPtr lParam);

    /// <summary> Passes message information to the specified window procedure. </summary>
    ///
    /// <param name="lpPrevWndFunc"> The previous window procedure. 
    /// If this value is obtained by calling the GetWindowLong function
    /// with the nIndex parameter set to GWL_WNDPROC or DWL_DLGPROC, 
    /// it is actually either the address of a window or dialog box procedure, 
    /// or a special internal value meaningful only to CallWindowProc. 
    /// </param>
    /// <param name="hWnd">           A handle to the window whose procedure will receive the message. </param>
    /// <param name="Msg">            The message. </param>
    /// <param name="wParam">        Additional message-specific information. The contents of this parameter
    /// depend on the value of the <paramref name="Msg"/> parameter. </param>
    /// <param name="lParam">        Additional message-specific information. The contents of this parameter
    /// depend on the value of the <paramref name="Msg"/> parameter. </param>
    ///
    /// <returns>
    /// The return value specifies the result of the message processing and depends on the message sent.
    /// </returns>
    [DllImport("user32")]
    public static extern IntPtr CallWindowProc(
      IntPtr lpPrevWndFunc,
      IntPtr hWnd,
      uint Msg,
      IntPtr wParam,
      IntPtr lParam);

    /// <summary> Passes message information to the specified window procedure. </summary>
    ///
    /// <param name="lpPrevWndFunc"> The previous window procedure. 
    /// If this value is obtained by calling the GetWindowLong function
    /// with the nIndex parameter set to GWL_WNDPROC or DWL_DLGPROC, 
    /// it is actually either the address of a window or dialog box procedure, 
    /// or a special internal value meaningful only to CallWindowProc. 
    /// </param>
    /// <param name="hWnd">           A handle to the window whose procedure will receive the message. </param>
    /// <param name="Msg">            The message. </param>
    /// <param name="wParam">        Additional message-specific information. The contents of this parameter
    /// depend on the value of the <paramref name="Msg"/> parameter. </param>
    /// <param name="lParam">        Additional message-specific information. The contents of this parameter
    /// depend on the value of the <paramref name="Msg"/> parameter. </param>
    ///
    /// <returns>
    /// The return value specifies the result of the message processing and depends on the message sent.
    /// </returns>
    [DllImport("user32")]
    public static extern IntPtr CallWindowProc(
      WndProcDelegate lpPrevWndFunc,
      IntPtr hWnd,
      uint Msg,
      IntPtr wParam,
      IntPtr lParam);

    /// <summary> The FrameRect function draws a border around the specified rectangle by using 
    /// the specified brush. The width and height of the border are always one logical unit.
    /// </summary>
    /// <param name="hdc"> A handle to the device context in which the border is drawn. </param>
    /// <param name="lpRect"> A pointer to a RECT structure that contains the logical coordinates
    /// of the upper-left and lower-right corners of the rectangle. </param>
    /// <param name="hBrush"> A handle to the brush used to draw the border.</param>
    /// <returns> If the function succeeds, the return value is nonzero; otherwise it is zero.</returns>
    [DllImport("user32")]
    public static extern int FrameRect(IntPtr hdc, ref RECT lpRect, IntPtr hBrush);

    /// <summary> The DrawEdge function draws one or more edges of rectangle. </summary>
    /// <param name="hdc"> A handle to the device context in which the edge is drawn. </param>
    /// <param name="lpRect"> [in,out] A RECT structure that contains the logical coordinates of the
    ///  rectangle. </param>
    /// <param name="edge"> The type of inner and outer edges to draw. This parameter must be a combination 
    ///  of one inner-border flag and one outer-border flag.
    ///  </param>
    /// <param name="grfFlags"> The type of border. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DrawEdge(IntPtr hdc, ref RECT lpRect, int edge, int grfFlags);

    /// <summary> Animates the caption of a window to indicate the opening of an icon or the minimizing 
    /// or maximizing of a window. </summary>
    /// <param name="hwnd"> A handle to the window whose caption should be animated on the screen. 
    /// The animation will be clipped to the parent of this window. </param>
    /// <param name="idAni"> The type of animation. This must be IDANI_CAPTION. 
    /// With the IDANI_CAPTION animation type, the window caption will animate from the position 
    /// specified by lprcFrom to the position specified by lprcTo. 
    /// The effect is similar to minimizing or maximizing a window. </param>
    /// <param name="lprcFrom"> A RECT structure specifying the location and size of the icon 
    ///  or minimized window. Coordinates are relative to the clipping window <paramref name="hwnd"/>.</param>
    /// <param name="lprcTo"> A RECT structure specifying the location and size of the restored window. 
    /// Coordinates are relative to the clipping window <paramref name="hwnd"/>.</param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("user32", EntryPoint = "DrawAnimatedRects", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DrawAnimatedRects(
      IntPtr hwnd,
      int idAni,
      ref RECT lprcFrom,
      ref RECT lprcTo);

    /// <summary>
    /// Enumerates all top-level windows on the screen by passing the handle to each window, in turn, to an
    /// application-defined callback function. EnumWindows continues until the last top-level window is
    /// enumerated or the callback function returns false.
    /// </summary>
    /// <param name="x"> An application-defined callback function. For more information, see
    ///  <see cref="EnumWindowsProc"/> </param>
    /// <param name="lParam"> An application-defined value to be passed to the callback function. </param>
    /// <returns>
    /// If the function succeeds, the return value is nonzero. If the function fails, the return value is
    /// zero.
    /// </returns>
    [DllImport("user32")]
    public static extern int EnumWindows(EnumWindowsProc x, IntPtr lParam);

    /// <summary>
    /// Enumerates all top-level windows on the screen by passing the handle to each window, in turn, to an
    /// application-defined callback function. EnumWindows continues until the last top-level window is
    /// enumerated or the callback function returns false.
    /// </summary>
    /// <param name="x"> An application-defined callback function. For more information, see
    ///  <see cref="EnumWindowsProc"/> </param>
    /// <param name="lParam"> An application-defined data structure to be passed to the callback function. </param>
    /// <returns>
    /// If the function succeeds, the return value is nonzero. If the function fails, the return is zero.
    /// Note: If EnumWindowsProcEx returns zero, the return value is also zero. In this case, the callback 
    /// should call SetLastError API to obtain a meaningful error code to be returned to the caller of EnumWindows.
    /// </returns>
    [DllImport("User32")]
    public static extern int EnumWindows(EnumWindowsProcEx x, ref WindowSearchData lParam);

    /// <summary>
    /// Retrieves a handle to the top-level window whose class name and window name match the specified 
    /// strings. This function does not search child windows, and does not perform a case-sensitive search.
    /// </summary>
    /// <param name="lpszClassName"> The class name. Optional argument that may be null.
    /// If that string is null, it finds any window whose title matches the lpszWindowName parameter.
    /// </param>
    /// <param name="lpszWindowName"> The window name (the window's title). If this parameter is null, 
    /// all window names match. </param>
    /// <remarks>
    /// Do NOT specify string argument lpszClassName as [MarshalAs(UnmanagedType.LPWStr)].
    /// Since the class name is stored internally still as an ANSI string, usage of different marshalling
    /// would not work.
    /// </remarks>
    /// <returns> If the function succeeds, the return value is a handle to the found window.
    /// If the function fails, the return value is IntPtr.Zero. 
    /// To get extended error information, call Marshal.GetLastWin32Error().</returns>
    [DllImport("user32", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    public static extern IntPtr FindWindow(
      [MarshalAs(UnmanagedType.LPStr)] string lpszClassName,
      string lpszWindowName);

    /// <summary>
    /// Retrieves a handle to the top-level window whose class name and window name match the specified
    /// strings. The function searches child windows, beginning with the one following the specified child
    /// window. This function does not perform a case-sensitive search.
    /// </summary>
    /// <remarks>
    /// Do NOT specify string argument lpszClassName as [MarshalAs(UnmanagedType.LPWStr)]. Since the class
    /// name is stored internally still as an ANSI string, usage of different marshaling would not work.
    /// </remarks>
    /// <param name="hwndParent"> A handle to the parent window whose child windows are to be searched. If
    ///  hwndParent is NULL, the function uses the desktop window as the parent window. The function
    ///  searches among windows that are child windows of the desktop. If hwndParent is HWND_MESSAGE, the
    ///  function searches all message-only windows. </param>
    /// <param name="hwndChildAfter"> A handle to a child window. The search begins with the next child
    ///  window in the Z order. The child window must be a direct child window of hwndParent, not just a
    ///  descendant window.
    ///  If hwndChildAfter is NULL, the search begins with the first child window of hwndParent.
    ///  If both hwndParent and hwndChildAfter are NULL, the function searches all top-level and
    ///  message-only windows.
    ///  </param>
    /// <param name="lpszClassName"> The class name. Optional argument that may be null. If that string is
    ///  null, it finds any window whose title matches the lpszWindowName parameter. </param>
    /// <param name="lpszWindowName"> The window name (the window's title). If this parameter is null, all
    ///  window names match. </param>
    /// <returns>
    /// If the function succeeds, the return value is a handle to the found window. If the function fails,
    /// the return value is IntPtr.Zero. To get extended error information, call
    /// Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    public static extern IntPtr FindWindowEx(
        IntPtr hwndParent,
        IntPtr hwndChildAfter,
        [MarshalAs(UnmanagedType.LPStr)] string lpszClassName,
        string lpszWindowName);

    /// <summary>
    /// Retrieves a handle to the foreground window (the window with which the user is currently working).
    /// The system assigns a slightly higher priority to the thread that creates the foreground window than
    /// it does to other threads.
    /// </summary>
    /// <returns>
    /// A handle to the foreground window. The foreground window can be IntPtr.Zero in certain circumstances,
    /// such as when a window is losing activation.
    /// </returns>
    [DllImport("user32")]
    public static extern IntPtr GetForegroundWindow();

    /// <summary>
    /// Brings the thread that created the specified window into the foreground and activates the window.
    /// Keyboard input is directed to the window, and various visual cues are changed for the user. The
    /// system assigns a slightly higher priority to the thread that created the foreground window than it
    /// does to other threads.
    /// </summary>
    /// <param name="hWnd"> A handle to the window that should be activated and brought to the foreground. 
    /// </param>
    /// <returns> If the window was brought to the foreground, the return value is nonzero.
    /// If the window was not brought to the foreground, the return value is zero.
    /// </returns>
    [DllImport("user32")]
    public static extern Int32 SetForegroundWindow(IntPtr hWnd);

    /// <summary>
    /// Activates a window. The window must be attached to the calling thread's message queue.
    /// </summary>
    /// <param name="hWnd"> A handle to the top-level window to be activated. </param>
    /// <returns>
    /// If the function succeeds, the return value is the handle to the window that was previously active.
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr SetActiveWindow(IntPtr hWnd);

    /// <summary> Copies the text of the specified window's title bar (if it has one) into a buffer. 
    /// If the specified window is a control, the text of the control is copied. 
    /// However, GetWindowText cannot retrieve the text of a control in another process.
    /// To retrieve the text of a control in another process, send a WM_GETTEXT message directly 
    /// instead of calling GetWindowText.
    /// </summary>
    /// <remarks>
    /// The output buffer <paramref name="s"/>needs to be pre-allocated to consume the text completely;
    /// needed size can be figured-out through <see cref="GetWindowTextLength"/>.
    /// To achieve that in one convenient method, you can call an overloaded method 
    /// <see cref="GetWindowText(IntPtr)"/>.
    /// </remarks>
    ///
    /// <param name="hWnd"> A handle to the window whose text should be retrieved. </param>
    /// <param name="s"> The buffer that will receive the text. </param>
    /// <param name="nMaxCount"> The maximum number of characters to copy to the buffer, 
    /// including the null character. If the text exceeds this limit, it is truncated.
    /// </param>
    /// <returns> If the function succeeds, the return value is the length, in characters, 
    /// of the copied string, not including the terminating null character. 
    /// If the window has no title bar or text, if the title bar is empty, 
    /// or if the window or control handle is invalid, the return value is zero. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// <seealso cref="GetWindowTextLength"/>
    [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern Int32 GetWindowText(
        IntPtr hWnd,
        [MarshalAs(UnmanagedType.LPWStr)] StringBuilder s,
        int nMaxCount);

    /// <summary>
    /// Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
    /// </summary>
    /// <param name="hWnd">
    /// A handle to the window whose placement information is to be retrieved.
    /// </param>
    /// <param name="lpwndpl">
    /// A reference to a <see cref="WINDOWPLACEMENT"/> structure that receives the show state and position information.
    /// Before calling <c>GetWindowPlacement</c>, set the <c>Length</c> member of the structure to <c>sizeof(WINDOWPLACEMENT)</c>.
    /// The function fails if this member is not set correctly.
    /// </param>
    /// <returns>
    /// <c>true</c> if the function succeeds; otherwise, <c>false</c>.
    /// To get extended error information, call <c>Marshal.GetLastWin32Error()</c>.
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

    /// <summary>
    /// Sets the show state and the restored, minimized, and maximized positions of the specified window.
    /// </summary>
    /// <param name="hWnd">
    /// A handle to the window whose placement information is to be set.
    /// </param>
    /// <param name="lpwndpl">
    /// A reference to a <see cref="WINDOWPLACEMENT"/> structure that specifies the new show state and position information.
    /// Before calling <c>SetWindowPlacement</c>, set the <c>Length</c> member of the structure to <c>sizeof(WINDOWPLACEMENT)</c>.
    /// The function fails if this member is not set correctly.
    /// </param>
    /// <returns> <c>true</c> if the function succeeds; otherwise, <c>false</c>. </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

    /// <summary>
    /// Retrieves the identifier of the thread that created the specified window, and optionally the
    /// identifier of the process that created the window.
    /// </summary>
    /// <param name="hWnd"> A handle to the window.</param>
    /// <param name="lpdwProcessId"> [out] A variable that receives the process identifier. </param>
    /// <returns> The window thread process identifier. </returns>
    [DllImport("user32")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    /// <summary> Retrieves the length, in characters, of the specified window's title bar text 
    /// (if the window has a title bar). If the specified window is a control, the function retrieves 
    /// the length of the text within the control. 
    /// However, GetWindowTextLength cannot retrieve the length of the text of an edit control 
    /// in another application. To achieve that, send a WM_GETTEXTLENGTH message directly.
    /// </summary>
    /// <param name="hwnd"> A handle to the window or control. </param>
    /// <returns> If the function succeeds, the return value is the length, in characters, of the text. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// <seealso cref="GetWindowText(IntPtr, StringBuilder, int)"/>
    [DllImport("user32", SetLastError = true)]
    public static extern Int32 GetWindowTextLength(IntPtr hwnd);

    /// <summary>
    /// Synthesizes a keystroke. The system can use such a synthesized keystroke to generate a WM_KEYUP
    /// or WM_KEYDOWN message.
    /// </summary>
    /// <remarks>
    /// This function has been superseded. One should use
    /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms646310(v=vs.85).aspx">
    ///  SendInput</see> instead.
    /// </remarks>
    /// <param name="bVk"> A virtual-key code. The code must be a value in the range 1 to 254. For a
    ///  complete list, see
    ///  <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx">
    ///  Virtual Key Codes</see>. </param>
    /// <param name="bScan"> A hardware scan code for the key. </param>
    /// <param name="dwFlags"> Controls various aspects of function operation. This parameter can be one
    ///  or more of the following values.  
    ///  <list type="bullet">
    ///  <item><b>KEYEVENTF_EXTENDEDKEY</b>0x0001<br/>If specified, the scan code was preceded by a prefix
    ///  byte having the value 0xE0 (224).</item>
    ///  <item><b>KEYEVENTF_KEYUP</b>0x0002<br/>If specified, the key is being released. If not specified, 
    ///  the key is being depressed.</item>
    ///  </list> </param>
    /// <param name="dwExtraInfo"> An additional value associated with the key stroke. </param>
    [DllImport("user32")]
    public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, IntPtr dwExtraInfo);

    /// <summary> Copies the status of the 256 virtual keys to the specified buffer.
    /// </summary>
    /// <param name="lpKeyState"> The 256-byte array that receives the status data for each virtual key.
    /// </param>
    /// <returns> True if it succeeds, false if it fails. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetKeyboardState(byte[] lpKeyState);

    /// <summary>
    /// Copies an array of keyboard key states into the calling thread's keyboard input-state table. This
    /// is the same table accessed by the GetKeyboardState and GetKeyState functions. Changes made to this
    /// table do not affect keyboard input to any other thread.
    /// </summary>
    /// <param name="lpKeyState"> A reference to a 256-byte array that contains keyboard key states.
    /// </param>
    /// <returns> True if it succeeds, false if it fails. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetKeyboardState(byte[] lpKeyState);

    /// <summary>Loads the specified icon resource from the executable (.exe) file associated with 
    /// an application instance.
    /// </summary>
    /// <param name="hInst"> A handle to an instance of the module whose executable file contains 
    /// the icon to be loaded. This parameter must be NULL when a standard icon is being loaded.
    /// </param>
    /// <param name="iconId"> Identifier for the icon, i.e. the name of the icon resource to be loaded. 
    /// Alternatively, this parameter can contain the resource identifier in the low-order word 
    /// and zero in the high-order word </param>
    /// <returns> If the function succeeds, the return value is a handle to the newly loaded icon.
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr LoadIcon(
        IntPtr hInst,
        [MarshalAs(UnmanagedType.LPWStr)] string iconId);

    /// <summary> Retrieves information about the specified icon or cursor. </summary>
    /// <param name="hIcon"> A handle to the icon or cursor. </param>
    /// <param name="piconinfo"> [out] A reference to an <see cref="ICONINFO"/>ICONINFO structure. 
    /// The function fills in the structure's members. </param>
    /// <returns> true if it succeeds, false if it fails.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

    // Note: here is used wrapper of API SendMessage with different NET arguments,
    // therefore I need to give it different name.
    [DllImport("user32", CharSet = CharSet.Ansi, EntryPoint = "SendMessageA")]
    private static extern IntPtr SendMsgToGetText(
        IntPtr hWnd,
        int Msg,
        int wParam,
        [Out][MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder lParam);

    /// <summary>
    /// The DrawText function draws formatted text in the specified rectangle. It formats the text
    /// according to the specified method (expanding tabs, justifying characters, breaking lines, and so
    /// forth).
    /// </summary>
    /// <param name="hDC"> A handle to the device context. </param>
    /// <param name="lpString"> A pointer to the string that specifies the text to be drawn. If the nCount
    ///  parameter is -1, the string must be null-terminated. </param>
    /// <param name="nCount"> The length, in characters, of the string. If nCount is -1, then the lpchText
    ///  parameter is assumed to be a pointer to a null-terminated string and DrawText computes the
    ///  character count automatically. </param>
    /// <param name="lpRect"> [in,out]. A pointer to a RECT structure that contains the rectangle (in
    ///  logical coordinates) in which the text is to be formatted. </param>
    /// <param name="uFormat"> The method of formatting the text. This parameter can be one or more of the
    ///  following values. </param>
    /// <returns>
    /// If the function succeeds, the return value is the height of the text in logical units. If
    /// DT_VCENTER or DT_BOTTOM is specified, the return value is the offset from lpRect-&gt;top to the
    /// bottom of the drawn text. <br/>
    /// If the function fails, the return value is zero. <br/>
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern int DrawText(
        IntPtr hDC,
        [MarshalAs(UnmanagedType.LPWStr)] string lpString,
        int nCount,
        ref RECT lpRect,
        uint uFormat);

    /// <summary>
    /// Opens the clipboard for examination and prevents other applications from modifying the clipboard content.
    /// </summary>
    /// <param name="hWndNewOwner">A handle to the window to be associated with the open clipboard. 
    ///  If this parameter is IntPtr.Zero, the open clipboard is associated with the current task.
    /// </param>
    /// <returns>If the function succeeds, the return value is true; otherwise false.
    /// To get extended error information, call Marshal.GetLastWin32Error()..
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool OpenClipboard(IntPtr hWndNewOwner);

    /// <summary>
    /// Empties the clipboard and frees handles to data in the clipboard. 
    /// The function then assigns ownership of the clipboard to the window that currently has the clipboard open.
    /// </summary>
    /// <returns>If the function succeeds, the return value is true; otherwise false.
    /// To get extended error information, call Marshal.GetLastWin32Error()..
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EmptyClipboard();

    /// <summary>
    /// Closes the clipboard.
    /// </summary>
    /// <returns>If the function succeeds, the return value is true; otherwise false.
    /// To get extended error information, call Marshal.GetLastWin32Error()..
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseClipboard();

    /// <summary>
    /// Determines whether the clipboard contains data in the specified format.
    /// </summary>
    /// <param name="format">A standard or registered clipboard format.</param>
    /// <returns>If the clipboard format is available, the return value is true, otherwise false.
    /// To get extended error information, call Marshal.GetLastWin32Error()..
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsClipboardFormatAvailable(uint format);

    /// <summary>
    /// Retrieves data from the clipboard in a specified format. The clipboard must have been opened previously.
    /// </summary>
    /// <param name="format">A standard or registered clipboard format.</param>
    /// <returns>If the function succeeds, the return value is the handle to a clipboard object 
    /// in the specified format. If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error()..
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr GetClipboardData(uint format);

    /// <summary>
    /// Places data on the clipboard in a specified clipboard format.
    /// </summary>
    /// <param name="uFormat">A standard or registered clipboard format.</param>
    /// <param name="hMem">A handle to the data in the specified format. 
    /// This parameter can be IntPtr.Zero, indicating that the window provides data in the specified 
    /// clipboard format (renders the format) upon request.</param>
    /// <returns>
    /// If the function succeeds, the return value is the handle to the data.
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error()..
    /// </returns>
    [DllImport("User32", SetLastError = true)]
    public static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

    /// <summary>
    /// Adds the specified window to the chain of clipboard viewers. 
    /// Clipboard viewer windows receive a WM_DRAWCLIPBOARD message whenever the content of the clipboard changes.
    /// </summary>
    /// <param name="hWnd">A handle to the window to be added to the clipboard chain.</param>
    /// <returns>
    /// If the function succeeds, the return value identifies the next window in the clipboard viewer chain. 
    /// If an error occurs or there are no other windows in the clipboard viewer chain, 
    /// the return value is IntPtr.Zero. 
    /// To get extended error information, call Marshal.GetLastWin32Error()..
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr SetClipboardViewer(IntPtr hWnd);

    /// <summary>
    /// Removes a specified window from the chain of clipboard viewers.
    /// </summary>
    /// <param name="hWndRemove">
    /// A handle to the window to be removed from the chain. 
    /// The handle must have been passed to the <see cref="SetClipboardViewer"/> function.
    /// </param>
    /// <param name="hWndNewNext">
    /// A handle to the window that follows the hWndRemove window in the clipboard viewer chain. 
    /// (This is the handle returned by SetClipboardViewer, unless the sequence was changed 
    /// in response to a WM_CHANGECBCHAIN message.)
    /// </param>
    /// <returns>
    /// The return value indicates the result of passing the WM_CHANGECBCHAIN message to the windows 
    /// in the clipboard viewer chain. 
    /// Because a window in the chain typically returns false when it processes WM_CHANGECBCHAIN, 
    /// the return value from ChangeClipboardChain is typically false. 
    /// If there is only one window in the chain, the return value is typically true.
    /// </returns>
    [DllImport("user32", SetLastError = false)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ChangeClipboardChain(
        IntPtr hWndRemove,  // handle to window to remove
        IntPtr hWndNewNext);  // handle to next window

    /// <summary>
    /// Creates a new image (icon, cursor, or bitmap) and copies the attributes of the specified image 
    /// to the new one. If necessary, the function stretches the bits to fit the desired size of the new image.
    /// </summary>
    /// <param name="hImage">A handle to the image to be copied. </param>
    /// <param name="uType">. The type of image to be copied. This parameter can be one of the following values
    /// <list type="bullet">
    /// <item><b>IMAGE_BITMAP = 0</b><br/>Copies a bitmap.</item>
    /// <item><b>IMAGE_CURSOR = 2</b><br/>Copies a cursor.</item>
    /// <item><b>IMAGE_ICON = 1</b><br/>Copies an icon.</item>
    /// </list> </param>
    /// <param name="cxDesired">The desired width, in pixels, of the image. 
    /// If this is zero, then the returned image will have the same width as the original hImage. </param>
    /// <param name="cyDesired">The desired height, in pixels, of the image. 
    /// If this is zero, then the returned image will have the same height as the original hImage. </param>
    /// <param name="fuFlags"> Represents a bitwise flag, specifying other details of behavior.
    /// For more information, see <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms648031(v=vs.85).aspx">
    /// CopyImage function</a> MSDN documentation.
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is handle to the newly created image; 
    /// otherwise it is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr CopyImage(
      IntPtr hImage, uint uType, int cxDesired, int cyDesired, uint fuFlags);

    /// <summary>
    /// Gets the handle of the window menu (also known as the system menu or the control menu), thus
    /// enabling the application to access it for copying and modifying.
    /// </summary>
    /// <param name="hWnd"> A handle to the window that will own a copy of the window menu. </param>
    /// <param name="bRevert"> The action to be taken. If this parameter is false, GetSystemMenu returns a
    ///  handle to the copy of the window menu currently in use. The copy is initially identical to the
    ///  window menu, but it can be modified. If this parameter is true, GetSystemMenu resets the window
    ///  menu back to the default state. The previous window menu, if any, is destroyed. </param>
    /// <returns> If the bRevert parameter is false, the return value is a handle to a copy 
    /// of the window menu. If the bRevert parameter is true, the return value is IntPtr.Zero. </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr GetSystemMenu(
      IntPtr hWnd,
      [MarshalAs(UnmanagedType.Bool)] bool bRevert);

    /// <summary> Determines the number of items in the specified menu.</summary>
    /// <param name="hWnd"> A handle to the menu to be examined. </param>
    /// <returns> If the function succeeds, the return value specifies the number of items in the menu.
    /// If the function fails, the return value is -1.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern int GetMenuItemCount(
      IntPtr hWnd);

    /// <summary>
    /// Deletes an item from the specified menu. If the menu item opens a menu or submenu, this function
    /// destroys the handle to the menu or submenu and frees the memory used by the menu or submenu.
    /// </summary>
    /// <param name="hMenu"> A handle to the menu to be changed. </param>
    /// <param name="nPosition"> The menu item to be deleted, as determined by the uFlags parameter. </param>
    /// <param name="wFlags"> Indicates how the uPosition parameter is interpreted. This parameter must be
    ///  one of the following values.
    ///  <list type="bullet"><item><b>MF_BYCOMMAND</b> 0x00000000<br/>
    ///  Indicates that uPosition gives the identifier of the menu item.
    ///  </item>
    ///  <item><b>MF_BYPOSITION</b> 0x00000400L<br/>
    ///  Indicates that uPosition gives the zero-based relative position of the menu item.
    ///  </item>
    ///  </list> </param>
    /// <returns> true if it succeeds, false if it fails. 
    /// To get extended error information, call Marshal.GetLastWin32Error().</returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteMenu(
      IntPtr hMenu,
      int nPosition,
      int wFlags);

    /// <summary> Deletes a menu item or detaches a submenu from the specified menu. 
    /// If the menu item opens a drop-down menu or submenu, RemoveMenu does not destroy the menu 
    /// or its handle, allowing the menu to be reused. Before this function is called, 
    /// the GetSubMenu function should retrieve a handle to the drop-down menu or submenu.
    /// </summary>
    /// <param name="hMenu"> A handle to the menu to be changed. </param>
    /// <param name="nPosition"> The menu item to be deleted, as determined by the uFlags parameter.
    /// </param>
    /// <param name="wFlags"> Indicates how the uPosition parameter is interpreted. 
    ///  This parameter must be one of the following values.
    /// <list type="bullet">
    /// <item><b>MF_BYCOMMAND</b> 0x00000000<br/>
    /// Indicates that uPosition gives the identifier of the menu item.
    /// </item>
    /// <item><b>MF_BYPOSITION</b> 0x00000400L<br/>
    /// Indicates that uPosition gives the zero-based relative position of the menu item.
    /// </item>
    /// </list>
    /// </param>
    /// 
    /// <returns> true if it succeeds, false if it fails. 
    /// To get extended error information, call Marshal.GetLastWin32Error().</returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool RemoveMenu(
      IntPtr hMenu,
      int nPosition,
      int wFlags);

    /// <summary> Enables, disables, or grays the specified menu item.</summary>
    /// <param name="hMenu"> A handle to the menu. </param>
    /// <param name="uIDEnableItem"> The menu item to be enabled, disabled, or grayed, as determined
    ///  by the uEnable parameter. This parameter specifies an item in a menu bar, menu, or submenu.
    /// </param>
    /// <param name="uEnable"> Controls the interpretation of the uIDEnableItem parameter and indicate 
    /// whether the menu item is enabled, disabled, or grayed. 
    /// This parameter must be a combination of the several flag values.
    /// </param>
    /// <returns> The return value specifies the previous state of the menu item (it is either MF_DISABLED, 
    /// MF_ENABLED, or MF_GRAYED). If the menu item does not exist, the return value is -1.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnableMenuItem(
      IntPtr hMenu,
      uint uIDEnableItem,
      uint uEnable);

    /// <summary>
    /// Inserts a new menu item at the specified position in a menu.
    /// </summary>
    /// <param name="hMenu">A handle to the menu to be modified.</param>
    /// <param name="uItem">The identifier or position where the new item is inserted, depending on <paramref name="fByPosition"/>.</param>
    /// <param name="fByPosition">
    /// If <c>true</c>, <paramref name="uItem"/> specifies the position; 
    /// if <c>false</c>, <paramref name="uItem"/> specifies the identifier of the menu item.
    /// </param>
    /// <param name="lpmii">Reference to a <see cref="MENUITEMINFO"/> structure containing information about the new menu item.</param>
    /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
    [DllImport("user32", SetLastError = true)]
    public static extern bool InsertMenuItem(
        IntPtr hMenu,
        uint uItem,
        bool fByPosition,
        [In] ref MENUITEMINFO lpmii);

    /// <summary> Updates the menu bar of the specified window. </summary>
    /// <param name="hWnd"> A handle to the window whose menu bar is to be redrawn. </param>
    /// <returns> <c>true</c> if successful; otherwise, <c>false</c>. </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern bool DrawMenuBar(IntPtr hWnd);

    /// <summary>
    /// Retrieves a handle to a window that has the specified relationship 
    /// (Z-Order or owner) to the specified window.
    /// </summary>
    /// <param name="hWnd">A handle to a window. The window handle retrieved 
    /// is relative to this window, based on the value of the uCmd </param>
    /// <param name="uCmd">The relationship between the specified window 
    ///  and the window whose handle is to be retrieved. <br/>
    ///  Use GetWindow_Cmd enum values here.</param>
    /// <returns>If the function succeeds, the return value is a window handle. 
    ///  If it fails, the return is IntPtr.Zero. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    ///  </returns>
    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr GetWindow(
      IntPtr hWnd,
      uint uCmd);

    /// <summary> Enumerates the child windows that belong to the specified parent window 
    ///            by passing the handle to each child window, in turn, to an application-defined callback function. 
    ///            EnumChildWindows continues until the last child window is enumerated or the callback function returns FALSE. </summary>
    /// <param name="hWndParent"> A handle to the parent window whose child windows are to be enumerated. 
    ///                           If this parameter is NULL, this function is equivalent to EnumWindows.. </param>
    /// <param name="lpEnumFunc"> A pointer to an application-defined callback function. </param>
    /// <param name="lParam"> An application-defined value to be passed to the callback function.. </param>
    /// <returns>   True if it succeeds, false if it fails. </returns>
    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumChildWindows(
        [Optional][In] IntPtr hWndParent,
        [In] EnumChildProc lpEnumFunc,
        [In] IntPtr lParam);

    #endregion // External functions

    #region Methods
    #region Public Methods

    /// <summary>
    /// Determines whether currently running process is 32-bit application.
    /// </summary>
    /// <returns>
    /// Returns true if the currently running process is running as 32-bit process, false otherwise.
    /// </returns>
    public static bool Is32BitProcess()
    {
        return (IntPtr.Size == 4);
    }

    /// <summary>
    /// Safe wrapper around native API
    /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644898(v=vs.85).aspx">
    /// SetWindowLongPtr</see>, which changes an attribute of the specified window;
    /// it may also set a value at the specified offset in the extra window memory.
    /// </summary>
    /// <remarks>
    /// This static method is required because legacy OSes do not support SetWindowLongPtr, hence it may
    /// require involving <see cref="SetWindowLong"/> instead.
    /// </remarks>
    /// <exception cref="EntryPointNotFoundException"> Thrown when an Entry Point Not Found error
    ///  condition occurs. </exception>
    /// <param name="hWnd"> A handle to the window. </param>
    /// <param name="nIndex"> The zero-based offset to the value to be set. Valid values are in the range
    ///  zero through the number of bytes of extra window memory, minus the size of an integer. To set any
    ///  other value, specify one of the <see cref="Win32.GWL"/>
    ///  enum values. </param>
    /// <param name="dwNewLong"> The replacement value. </param>
    /// <returns>
    /// If the function succeeds, the return value is the previous value of the specified offset. 
    /// If the function fails, the return value is zero.
    /// </returns>
    public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        IntPtr result = IntPtr.Zero;

        if (Is32BitProcess())
        {
            result = new IntPtr(SetWindowLong(hWnd, nIndex, dwNewLong.ToInt32()));
        }
        else
        {
            SetWindowLongPtrDelegate deleg;
            IntPtr hModule = Kernel32.GetModuleHandle("user32");
            IntPtr fnAddr = Kernel32.GetProcAddress(hModule, _strSetWindowLongPtrW);

            // call via a function pointer
            if (fnAddr != IntPtr.Zero)
            {   // create delegate
                deleg = (SetWindowLongPtrDelegate)Marshal.GetDelegateForFunctionPointer(
                  fnAddr, typeof(SetWindowLongPtrDelegate));
                // call via a function delegate
                result = deleg(hWnd, nIndex, dwNewLong);
            }
            else
            {
                throw new EntryPointNotFoundException(_strSetWindowLongPtrW);
            }
        }
        return result;
    }

    /// <summary>
    /// Safe wrapper around native API
    /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644898(v=vs.85).aspx">SetWindowLongPtr</see>,
    /// which changes an attribute of the specified window;
    /// it may also set a value at the specified offset in the extra window memory.<br/>
    /// This overload of SetWindowLongPtr should be used for case the method is called with 
    /// <see cref="Win32.GWL.GWL_WNDPROC"/> value; 
    /// i.e. it is used to set a new address for the window procedure.
    ///  </summary>
    /// 
    /// <exception cref="EntryPointNotFoundException"> Thrown when an Entry Point Not Found error
    ///  condition occurs. </exception>
    /// <param name="hWnd"> A handle to the window. </param>
    /// <param name="nIndex"> The zero-based offset to the value to be set. Valid values are in the range
    ///  zero through the number of bytes of extra window memory, minus the size of an integer. To set any
    ///  other value, specify one of the <see cref="Win32.GWL"/> enum values. </param>
    /// <param name="newProc"> A new address for the window procedure. </param>
    /// <returns>
    /// If the function succeeds, the return value is the previous value of the specified offset. If the
    /// function fails, the return value is zero.
    /// </returns>
    public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, WndProcDelegate newProc)
    {
        IntPtr result = IntPtr.Zero;

        if (Is32BitProcess())
        {
            result = new IntPtr(SetWindowLongPtr32(hWnd, nIndex, newProc));
        }
        else
        {
            SetWindowLongPtrWndProcDelegate deleg;
            IntPtr hModule = Kernel32.GetModuleHandle("user32");
            IntPtr fnAddr = Kernel32.GetProcAddress(hModule, _strSetWindowLongPtrW);

            // call via a function pointer
            if (fnAddr != IntPtr.Zero)
            {   // create delegate
                deleg = (SetWindowLongPtrWndProcDelegate)Marshal.GetDelegateForFunctionPointer(
                  fnAddr, typeof(SetWindowLongPtrWndProcDelegate));
                // call via a function delegate
                deleg(hWnd, nIndex, newProc);
            }
            else
            {
                throw new EntryPointNotFoundException(_strSetWindowLongPtrW);
            }
        }

        return result;
    }

    /// <summary>
    /// Safe wrapper around native API
    /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms633585(v=vs.85).aspx">
    /// GetWindowLongPtr</see>, which retrieves an attribute of the specified window;
    /// it may also retrieve a value at the specified offset in the extra window memory.
    /// </summary>
    /// <remarks>
    /// This static method is required because legacy OSes do not support SetWindowLongPtr, hence it may
    /// require involving <see cref="SetWindowLong"/> instead.
    /// </remarks>
    /// <exception cref="EntryPointNotFoundException"> Thrown when an Entry Point Not Found error
    ///  condition occurs. </exception>
    /// <param name="hWnd"> A handle to the window. </param>
    /// <param name="nIndex"> The zero-based offset to the value to be retrieved. Valid values are in the
    ///  range zero through the number of bytes of extra window memory, minus the size of an integer. 
    ///  To set any other value, specify one of the <see cref="Win32.GWL"/> enum values. </param>
    /// <returns>  If the function succeeds, the return value is the requested value.
    /// If the function fails, the return value is zero.
    /// </returns>
    public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
    {
        IntPtr result = IntPtr.Zero;

        if (Is32BitProcess())
        {
            result = new IntPtr(GetWindowLong(hWnd, nIndex));
        }
        else
        {
            GetWindowLongPtrDelegate deleg;
            IntPtr hModule = Kernel32.GetModuleHandle("user32");
            IntPtr fnAddr = Kernel32.GetProcAddress(hModule, _strGetWindowLongPtrW);
            // call via a function pointer
            if (fnAddr != IntPtr.Zero)
            {   // create delegate
                deleg = (GetWindowLongPtrDelegate)Marshal.GetDelegateForFunctionPointer(
                  fnAddr, typeof(GetWindowLongPtrDelegate));
                // call via a function delegate
                result = deleg(hWnd, nIndex);
            }
            else
            {
                throw new EntryPointNotFoundException(_strGetWindowLongPtrW);
            }
        }
        return result;
    }

    /// <summary> Gets window text. </summary>
    /// <param name="hWnd"> A handle to the window whose text should be retrieved. </param>
    /// <returns> The window text on success, and empty string on error. </returns>
    public static string GetWindowText(IntPtr hWnd)
    {
        string str = string.Empty;

        if (IsWindow(hWnd))
        {
            int cap = (int)SendMessage(hWnd, (int)Win32.WM.WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);
            if (cap > 0)
            {
                StringBuilder buffer = new(cap + 1);
                GetWindowText(hWnd, buffer, cap + 1);
                str = buffer.ToString();
            }
        }

        return str;
    }

    /// <summary>
    /// Get window (HWND) from point.
    /// Auxiliary helper; used as a wrapper to overshadow the difficulties with
    /// WindowFromPoint on X86 / X64 systems. See more for instance in 
    /// http://sites.google.com/site/x64lab/home/notes-on-x64-windows-gui-programming/windowfrompoint-on-x64-vista
    /// </summary>
    /// <param name="screenCoord"> Screen coordinates of the point. </param>
    /// <returns> If the function succeeds, the return value is the handle of the window that contains
    /// the point. If no window exists at the given point, the return value is IntPtr.Zero. </returns>
    public static IntPtr WndFromPoint(System.Drawing.Point screenCoord)
    {
        IntPtr result;

        if (Is32BitProcess())
        {
            result = WindowFromPoint(screenCoord.X, screenCoord.Y);
        }
        else
        { // Must construct POINT structure to provide it to WindowFromPoint. 
          // The overload with two arguments does not work on x64
            User32.POINT pt = new(screenCoord.X, screenCoord.Y);
            result = User32.WindowFromPoint(pt);
        }
        return result;
    }

    /// <summary>
    /// A wrapper around DllImport(et) <see cref="GetClassName(IntPtr, System.Text.StringBuilder, int)"/>
    /// </summary>
    /// <param name="hWnd"> A handle to the examined window. </param>
    /// <returns>Retrieves the name of the Win32 class to which the specified window belongs.
    /// In case of failure returns an empty string.
    /// </returns>
    public static string GetClassName(IntPtr hWnd)
    {
        // According to the documentation for the WNDCLASS structure, 
        // the maximum length for the class name is 256 characters. See also
        // http://social.msdn.microsoft.com/forums/en-US/vcgeneral/thread/c1087f6b-dc02-4082-979e-66c70479bfc7
        StringBuilder sbClass = new(260);

        User32.GetClassName(hWnd, sbClass, sbClass.Capacity);
#if DEBUG
        int lastErr = Marshal.GetLastWin32Error();
        string strClass = sbClass.ToString();

        if (string.IsNullOrEmpty(strClass) && (lastErr != 0))
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture,
              "User32.GetClassName returned an empty string, GetLastWin32Error = {0};", lastErr));
        }
        return strClass;
#else
        return sbClass.ToString();
#endif //DEBUG
    }

    /// <summary>
    /// Get the control which currently has the keyboard focus, if the control is attached to the calling 
    /// thread's message queue.
    /// </summary>
    /// <remarks>
    /// If the window that has the keyboard focus is not a .NET control, 
    /// but there is parent or grand-parent window that is Control, that Control is returned instead.
    /// </remarks>
    /// <returns>Returns resulting Control, or null.</returns>
    public static Control GetFocusedControl()
    {
        Control pFocused = null;

        for (IntPtr hFocused = User32.GetFocus(); hFocused != IntPtr.Zero;)
        {
            // If the window with the focus is not a control, it may be a child window of Control...
            if ((pFocused = Control.FromHandle(hFocused)) != null)
                break;
            hFocused = User32.GetParent(hFocused);
        }

        return pFocused;
    }

    /// <summary>
    /// Activates the top-level window, given its
    /// <see href="https://msdn.microsoft.com/en-us/library/system.windows.forms.iwin32window(v=vs.110).aspx">
    /// IWin32Window </see> interface.
    /// </summary>
    /// <param name="iWnd"> An interface exposing Win32 HWND handles. </param>
    /// <returns> True on success, false on failure. </returns>
    public static bool ActivateWnd(IWin32Window iWnd)
    {
        ArgumentNullException.ThrowIfNull(iWnd);
        return ActivateWnd(iWnd.Handle);
    }

    /// <summary> Activates the top-level window, given the window handle. </summary>
    /// <remarks>Delegates the functionality to <see cref="ActivateWindow"/>, 
    /// with the second argument User32.SW.RESTORE. </remarks>
    /// <param name="hwnd"> A handle to the window which should be activated. </param>
    /// <returns> True on success, false on failure. </returns>
    /// <seealso cref="ActivateWindow"/>
    public static bool ActivateWnd(IntPtr hwnd)
    {
        return ActivateWindow(hwnd, User32.SW.RESTORE);
    }

    /// <summary> Attempts to activate the top-level window, given the window handle. </summary>
    /// <param name="hWnd"> The window handle. </param>
    /// <param name="command"> The command being send to a window. 
    ///  Typically, SW.SHOWNORMAL or SW.SHOW will be used.</param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    public static bool ActivateWindow(IntPtr hWnd, User32.SW command)
    {
        bool result;

        if (result = User32.IsWindow(hWnd))
        {
            if (User32.IsIconic(hWnd))
            {
                User32.SendMessage(hWnd, (int)Win32.WM.WM_SYSCOMMAND, (IntPtr)Win32.SC_RESTORE, IntPtr.Zero);
            }
            User32.ShowWindow(hWnd, (int)command);
            User32.SetForegroundWindow(hWnd);
        }
        return result;
    }

    /// <summary>
    /// Retrieves the parameters of a scroll bar, including the minimum and maximum scrolling positions,
    /// the page size, and the position of the scroll box (thumb).
    /// </summary>
    /// <remarks>
    /// This method works as a wrapper around <see cref="GetScrollInfo"/> API. Unlike with
    /// <see cref="GetScrollInfo"/>, one does not have to set the lpScrollInfo.cbSize member
    ///  to sizeof(SCROLLINFO), and set the fMask member.
    /// </remarks>
    /// <param name="hWnd"> Handle to a scroll bar control or a window with a standard scroll bar,
    ///  depending on the value of the fnBar parameter. </param>
    /// <param name="nBar"> Specifies the type of scroll bar for which to retrieve parameters. </param>
    /// <param name="lpScrollInfo"> [in,out]. A reference to a SCROLLINFO structure. Before returning, the
    ///  function copies the specified parameters to the appropriate members of the structure. </param>
    /// <param name="nMask">. </param>
    /// <returns>
    /// If the function retrieved any values, the return value is nonzero. If the function does not
    /// retrieve any values, the return value is zero. To get extended error information, call
    /// Marshal.GetLastWin32Error().
    /// </returns>
    /// <seealso cref="GetScrollInfo"/>
    public static int GetWndScrollInfo(IntPtr hWnd,
        int nBar, ref SCROLLINFO lpScrollInfo, uint nMask)
    {
        lpScrollInfo.cbSize = (uint)Marshal.SizeOf(lpScrollInfo);
        lpScrollInfo.fMask = nMask;
        return GetScrollInfo(hWnd, nBar, ref lpScrollInfo);
    }

    /// <summary>
    /// utility method - conversion of Win32 API RECT to rectangle
    /// </summary>
    /// <param name="rc"> The RECT being converted. </param>
    /// <returns>a newly created Rectangle object</returns>
    public static Rectangle RectAPI2Rectangle(RECT rc)
    {
        return new Rectangle(rc.left, rc.top, rc.Width, rc.Height);
    }

    /// <summary>
    /// utility method - conversion of Rectangle to Win32 API RECT.
    /// </summary>
    /// <param name="rc"> The Rectangle being converted. </param>
    /// <returns> A RECT structure. </returns>
    public static RECT Rectangle2RectAPI(Rectangle rc)
    {
        return new RECT(rc.Left, rc.Top, rc.Right, rc.Bottom);
    }

    /// <summary> utility method - Win32 API GetWindowRect encapsulation. </summary>
    /// <param name="hWnd"> A handle to the investigated window. </param>
    /// <returns> The window rectangle on success, Rectangle.Empty on failure. </returns>
    public static Rectangle GetWindowRect(IntPtr hWnd)
    {
        Rectangle result = Rectangle.Empty;

        if (IsWindow(hWnd))
        {
            RECT rc = new();
            GetWindowRect(hWnd, ref rc);
            result = RectAPI2Rectangle(rc);
        }

        return result;
    }

    /// <summary> utility method - encapsulates Win32 API GetClientRect. </summary>
    /// <param name="hWnd"> A handle to the investigated window. </param>
    /// <returns> The client rectangle on success, Rectangle.Empty on failure. </returns>
    public static Rectangle GetClientRect(IntPtr hWnd)
    {
        Rectangle result = Rectangle.Empty;

        if (IsWindow(hWnd))
        {
            RECT rc = new();
            GetClientRect(hWnd, ref rc);
            result = RectAPI2Rectangle(rc);
        }

        return result;
    }

    /// <summary>
    /// Disables the Close(X) button.
    /// Note that this does not disable the Alt+F4 functionality.
    /// </summary>
    /// <param name="form">The Form whose system Close(x) button should be disabled.</param>
    /// <param name="removeMenuItem">Determines the way of implementation: <br/>
    /// If the value is true, the system menu item "Close" is removed.<br/>
    /// If the value is false, just disables that menu item.<br/>
    /// Both of this disables the related system Close(x) button.
    /// </param>
    /// <returns>True on success, false on failure</returns>
    /// <seealso cref="EnableCloseXButton"/>
    public static bool DisableCloseXButton(
        Form form,
        bool removeMenuItem)
    {
        ArgumentNullException.ThrowIfNull(form);

        IntPtr hMenu = User32.GetSystemMenu(form.Handle, false);
        bool bOk = true;

        if (hMenu == IntPtr.Zero)
        {
            bOk = false;
        }
        else
        {
            if (removeMenuItem)
            {
                // See also 
                // http://blogs.msdn.com/b/atosah/archive/2007/05/18/disable-close-x-button-in-winforms-using-c.aspx
                /* Instead of following, it is better to use SC_CLOSE value with MF_BYCOMMAND flag
                int menuItemCount = User32.GetMenuItemCount(hMenu);
                User32.RemoveMenu(hMenu, menuItemCount - 1, User32.MF_BYPOSITION);
                */
                User32.RemoveMenu(hMenu, (int)Win32.SC_CLOSE, User32.MF_BYCOMMAND);
            }
            else
            {
                // See also http://code.google.com/p/wyupdate/source/browse/trunk/SystemMenu.cs?r=412
                User32.EnableMenuItem(hMenu, Win32.SC_CLOSE, User32.MF_GRAYED);
            }
        }

        return bOk;
    }

    /// <summary>
    /// Reverts the effect of <see cref="DisableCloseXButton"/>, if that has been called with
    /// bRemoveMenuItem = false.
    /// </summary>
    /// <param name="form"> The Form whose system Close(x) button should be enabled. </param>
    /// <returns> True on success, false on failure. </returns>
    public static bool EnableCloseXButton(
        Form form)
    {
        ArgumentNullException.ThrowIfNull(form);

        IntPtr hMenu = User32.GetSystemMenu(form.Handle, false);
        bool bOk = true;

        if (hMenu == IntPtr.Zero)
        {
            bOk = false;
        }
        else
        {
            User32.EnableMenuItem(hMenu, Win32.SC_CLOSE, User32.MF_ENABLED);
        }
        return bOk;
    }

    /// <summary>
    /// Enumerates a window and child windows of a window; using given boolean function. Enumeration stops
    /// once the <paramref name="func"/>returns true for some child.
    /// </summary>
    /// <param name="hWnd"> The enumeration root. </param>
    /// <param name="func"> The criteria function. May be null. </param>
    /// <returns> A handle to found window, or IntPtr.Zero if none window is found. </returns>
    public static IntPtr RecursiveFind(IntPtr hWnd, Func<IntPtr, bool> func)
    {
        IntPtr hFirstChild;
        IntPtr result = IntPtr.Zero;

        if ((func != null) && func(hWnd))
        {
            result = hWnd;
        }
        else
        {
            // go through children
            hFirstChild = User32.GetWindow(hWnd, (uint)User32.GetWindow_Cmd.GW_CHILD);
            for (IntPtr hChild = hFirstChild; hChild != IntPtr.Zero;)
            {
                if (IntPtr.Zero != (result = RecursiveFind(hChild, func)))
                {
                    break;
                }
                hChild = User32.GetWindow(hChild, (uint)User32.GetWindow_Cmd.GW_HWNDNEXT);
            }
        }
        return result;
    }

    /// <summary> Attempts to find the top-level window, given the window name (the window's title),
    /// and (optional) process id. </summary>
    /// <param name="windowText"> Text of the window. </param>
    /// <param name="processId"> Optional identifier for the process. </param>
    /// <returns> Handle of the window if it succeeds, IntPtr.Zero if it fails. </returns>
    public static IntPtr FindWindow(string windowText, Nullable<int> processId)
    {
        IntPtr hWnd = IntPtr.Zero;

        if (!processId.HasValue)
        {
            hWnd = FindWindow(null, windowText);
        }
        else
        {
            var data = new WindowSearchData(windowText, (uint)processId.Value);

            EnumWindows(WindowFirstMatchCallback, ref data);
            if (data.FoundWindows.Count > 0)
                hWnd = data.FoundWindows[0];
        }
        return hWnd;
    }

    /// <summary> Searches for the message box, given the caption. </summary>
    /// <param name="caption"> The caption. </param>
    /// <returns>   The found message box. </returns>
    public static IntPtr FindMessageBox(string caption)
    {
        return FindWindow(Win32.MessageBoxClass, caption);
    }

    /// <summary> Sends a command to dialog button. </summary>
    /// <param name="hWnd"> A handle to the window ( dialog window ). </param>
    /// <param name="dlgButtonId"> Identifier for the dialog button. </param>
    public static void SendCommandToDlgButton(IntPtr hWnd, int dlgButtonId)
    {
        if (hWnd == IntPtr.Zero)
        {
            return;
        }

        EnumChildWindows(hWnd, delegate (IntPtr childHandle, IntPtr param)
        {
            int dlgCtrlID = GetDlgCtrlID(childHandle);
            bool buttonFound = (dlgCtrlID == dlgButtonId);

            if (buttonFound)
            {
                SendMessage(hWnd, (int)Win32.WM.WM_COMMAND, new IntPtr(dlgCtrlID), childHandle);
            }

            return !buttonFound;
        }, IntPtr.Zero);
    }
    #endregion // Public Methods

    #region Private Methods

    /// <summary>
    /// An application-defined callback function used with top-level windows enumeration. 
    /// It receives top-level window handles.
    /// </summary>
    /// <param name="hWnd"> A handle to a top-level window. </param>
    /// <param name="matchData"> [in,out] The data describing the search criteria. </param>
    /// <returns>
    /// To continue enumeration, the callback function must return true; to stop enumeration, it must return false.
    /// This particular method stops if both following conditions are true: <br/>
    /// i/ matchData.ProcessId is WindowSearchData.NoProcessid, or process Id of window match matchData.ProcessId
    /// ii/ matchData.WindowText is null, or the text of window match matchData.WindowText
    /// </returns>
    private static bool WindowFirstMatchCallback(IntPtr hWnd, ref WindowSearchData matchData)
    {
        string strText;
        bool bProcessMatch, bTextMatch;

#pragma warning disable IDE0059 // Unnecessary assignment of a value
        // Initialize ProcessId. Whatever GetWindowThreadProcessId does with output, better safe than sorry
        uint dwProcessId = WindowSearchData.NoProcessId;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        bool bContinueEnumeration = true;

        if (!(bProcessMatch = (matchData.ProcessId == WindowSearchData.NoProcessId)))
        {
            GetWindowThreadProcessId(hWnd, out dwProcessId);
            bProcessMatch = (matchData.ProcessId == dwProcessId);
        }
        if (bProcessMatch)
        {
            if (!(bTextMatch = (matchData.WindowText == null)))
            {
                strText = GetWindowText(hWnd);
                bTextMatch = string.Equals(strText, matchData.WindowText, StringComparison.Ordinal);
            }

            if (bTextMatch)
            {
                // match has been found
                matchData.FoundWindows.Add(hWnd);
                bContinueEnumeration = false;  // indicate that one does not need to enumerate longer
            }
        }

        return bContinueEnumeration;
    }
    #endregion // Private Methods
    #endregion // Methods

    #region Constructor(s)

    /// <summary>
    /// Static constructor, performs PrelinkAll check
    /// </summary>
    static User32()
    {
        try
        {
            Marshal.PrelinkAll(typeof(User32));
        }
#if DEBUG
        catch (Exception ex)
        {
            string strMsg = $"PrelinkAll failed for '{typeof(User32)}', with exception: '{ex.Message}', stack trace '{ex.StackTrace}'";
            Debug.Fail(strMsg);
            throw;
        }
#else
        catch (Exception)
        {
            throw;
        }
#endif // DEBUG
    }

    #endregion // Constructor(s)
}

#pragma warning restore SYSLIB1054
#pragma warning restore CA1806
#pragma warning restore CA1401
#pragma warning restore CA1069
#pragma warning restore IDE0290
#pragma warning restore IDE0251   
#pragma warning restore 1591
#pragma warning restore VSSpell001

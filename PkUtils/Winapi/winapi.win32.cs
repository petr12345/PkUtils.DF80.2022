using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;


namespace PK.PkUtils.WinApi;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable VSSpell001  // Spell Check
#pragma warning disable 1591        // Missing XML comment for publicly visible type or member...
#pragma warning disable CA1069      // The enum member ... has the same constant value as member ...
#pragma warning disable CA1401      // P/Invoke method should not be visible
#pragma warning disable IDE0130     // Namespace "..." does not match folder structure
#pragma warning disable SYSLIB1054  // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

/// <summary> Basic Win32 definitions. </summary>
[CLSCompliant(false)]
public static class Win32
{
    #region Constants

    #region WM_ACTIVATE state values

    /// <summary> One of possible low-order word codes of wParam argument of WM.WM_ACTIVATE message. 
    /// This concrete WA_INACTIVE value means the window is being deactivated.
    /// </summary>
    public const uint WA_INACTIVE = 0;

    /// <summary> One of possible low-order word codes of wParam argument of WM.WM_ACTIVATE message. 
    /// This concrete WA_ACTIVE value means the window is activated by some method other than a mouse click
    /// (for example, by a call to the SetActiveWindow function or by use of the keyboard interface to select the window).
    /// </summary>
    public const uint WA_ACTIVE = 1;

    /// <summary> One of possible low-order word codes of wParam argument of WM.WM_ACTIVATE message. 
    /// This concrete WA_CLICKACTIVE value means the window is activated by a mouse click.
    /// </summary>
    public const uint WA_CLICKACTIVE = 2;
    #endregion // WM_ACTIVATE state values

    /*
     * Dialog Codes
     */
    public const uint DLGC_WANTARROWS = 0x0001;     /* Control wants arrow keys         */
    public const uint DLGC_WANTTAB = 0x0002;     /* Control wants tab keys           */
    public const uint DLGC_WANTALLKEYS = 0x0004;     /* Control wants all keys           */
    public const uint DLGC_WANTMESSAGE = 0x0004;     /* Pass message to control          */
    public const uint DLGC_HASSETSEL = 0x0008;     /* Understands EM_SETSEL message    */
    public const uint DLGC_DEFPUSHBUTTON = 0x0010;     /* Default pushbutton               */
    public const uint DLGC_UNDEFPUSHBUTTON = 0x0020;    /* Non-default pushbutton           */
    public const uint DLGC_RADIOBUTTON = 0x0040;     /* Radio button                     */
    public const uint DLGC_WANTCHARS = 0x0080;     /* Want WM_CHAR messages            */
    public const uint DLGC_STATIC = 0x0100;     /* Static item: don't include       */
    public const uint DLGC_BUTTON = 0x2000;     /* Button item: can be checked      */


    /*
     * System Menu Command Values
     */
    public const uint SC_SIZE = 0xF000;
    public const uint SC_MOVE = 0xF010;
    public const uint SC_MINIMIZE = 0xF020;
    public const uint SC_MAXIMIZE = 0xF030;
    public const uint SC_NEXTWINDOW = 0xF040;
    public const uint SC_PREVWINDOW = 0xF050;
    public const uint SC_CLOSE = 0xF060;
    public const uint SC_VSCROLL = 0xF070;
    public const uint SC_HSCROLL = 0xF080;
    public const uint SC_MOUSEMENU = 0xF090;
    public const uint SC_KEYMENU = 0xF100;
    public const uint SC_ARRANGE = 0xF110;
    public const uint SC_RESTORE = 0xF120;
    public const uint SC_TASKLIST = 0xF130;
    public const uint SC_SCREENSAVE = 0xF140;
    public const uint SC_HOTKEY = 0xF150;
    public const uint SC_DEFAULT = 0xF160;
    public const uint SC_MONITORPOWER = 0xF170;
    public const uint SC_CONTEXTHELP = 0xF180;
    public const uint SC_SEPARATOR = 0xF00F;

    /*
    * Key State Masks for Mouse Messages
    */
    public const int MK_LBUTTON = 0x0001;
    public const int MK_RBUTTON = 0x0002;
    public const int MK_SHIFT = 0x0004;
    public const int MK_CONTROL = 0x0008;
    public const int MK_MBUTTON = 0x0010;

    /// Combo Box Notification Codes
    public const int CBN_ERRSPACE = (-1);
    public const int CBN_SELCHANGE = 1;
    public const int CBN_DBLCLK = 2;
    public const int CBN_SETFOCUS = 3;
    public const int CBN_KILLFOCUS = 4;
    public const int CBN_EDITCHANGE = 5;
    public const int CBN_EDITUPDATE = 6;
    public const int CBN_DROPDOWN = 7;
    public const int CBN_CLOSEUP = 8;
    public const int CBN_SELENDOK = 9;
    public const int CBN_SELENDCANCEL = 10;

    // listbox specific messages
    public const int LB_SETCURSEL = 0x0186;
    public const int LB_GETCURSEL = 0x0188;

    // ComboBox specific messages
    public const int CB_GETDROPPEDCONTROLRECT = 0x0152;
    public const int CB_GETITEMHEIGHT = 0x0154;
    public const int CB_GETTOPINDEX = 0x015b;

    /// <summary> Constant for invalid handle value, returned by FindFirstFile and so on. </summary>
    public const uint INVALID_HANDLE_VALUE = unchecked((uint)-1);

    // the mask for detecting Alt key pressed ( use with KBDLLHOOKSTRUCT.flags )
    public const int LLKHF_ALTDOWN = 0x20;

    /*
     * System Hook Codes
     */
    /// <summary> If nCode is HC_ACTION, the hook procedure must process the message. </summary>
    public const int HC_ACTION = 0;

    /// <summary>
    /// The wParam and lParam parameters contain information about a specific message, 
    /// and the message has not been removed from the message queue. 
    /// (An application called the PeekMessage function, specifying the PM_NOREMOVE flag.)
    /// </summary>
    public const int HC_NOREMOVE = 3;

    /// Scroll Bar Constants
    public const int SB_HORZ = 0;
    public const int SB_VERT = 1;
    public const int SB_CTL = 2;
    public const int SB_BOTH = 3;

    /// SCROLLINFO Constants
    public const int SIF_RANGE = 0x0001;
    public const int SIF_PAGE = 0x0002;
    public const int SIF_POS = 0x0004;
    public const int SIF_DISABLENOSCROLL = 0x0008;
    public const int SIF_TRACKPOS = 0x0010;
    public const int SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS);

    /// <summary> Windows Dialog Class Name. </summary>
    public const string MessageBoxClass = "#32770";
    public const string ButtonClass = "Button";
    public const string ListViewClassName = "SysListView32";
    #endregion // Constants

    #region Nested types

    /// <summary>
    /// Declares various Win32 windows styles.
    /// </summary>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms632600(v=vs.85).aspx">
    /// MSDN about windows styles</seealso>
    [Flags]
    public enum WindowStyles : uint
    {
        /// <summary>
        /// The window is an overlapped window. An overlapped window has a title bar and a border.
        /// </summary>
        WS_OVERLAPPED = 0x00000000,
        /// <summary>
        /// The windows is a pop-up window. This style cannot be used with the WS_CHILD style.
        /// </summary>
        WS_POPUP = 0x80000000,
        /// <summary>
        /// The window is a child window. A window with this style cannot have a menu bar. 
        /// This style cannot be used with the WS_POPUP style.
        /// </summary>
        WS_CHILD = 0x40000000,
        /// <summary>
        /// The window is initially minimized. Same as the WS_ICONIC style.
        /// </summary>
        WS_MINIMIZE = 0x20000000,
        /// <summary>
        /// The window is initially visible.
        /// This style can be turned on and off by using the ShowWindow or SetWindowPos function.
        /// </summary>
        WS_VISIBLE = 0x10000000,
        /// <summary>
        /// The window is disabled. A disabled window cannot receive input from the user.
        /// </summary>
        WS_DISABLED = 0x08000000,
        /// <summary>
        /// Clips child windows relative to each other; that is, when a particular child window receives 
        /// a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region 
        /// of the child window to be updated. If WS_CLIPSIBLINGS is not specified and child windows overlap, 
        /// it is possible, when drawing within the client area of a child window, to draw within the client area 
        /// of a neighboring child window.
        /// </summary>
        WS_CLIPSIBLINGS = 0x04000000,
        /// <summary>
        /// Excludes the area occupied by child windows when drawing occurs within the parent window. 
        /// This style is used when creating the parent window.
        /// </summary>
        WS_CLIPCHILDREN = 0x02000000,
        /// <summary>
        /// The window is maximized.
        /// </summary>
        WS_MAXIMIZE = 0x01000000,
        /// <summary>
        /// The window has a title bar (includes the WS_BORDER style).
        /// </summary>
        WS_CAPTION = 0x00C00000,
        /// <summary> 
        /// The window has a thin-line border. 
        /// </summary>
        WS_BORDER = 0x00800000,
        /// <summary>
        /// The window has a border of a style typically used with dialog boxes. 
        /// A window with this style cannot have a title bar.
        /// </summary>
        WS_DLGFRAME = 0x00400000,
        /// <summary>
        /// The window has a vertical scroll bar.
        /// </summary>
        WS_VSCROLL = 0x00200000,
        /// <summary>
        /// The window has a horizontal scroll bar.
        /// </summary>
        WS_HSCROLL = 0x00100000,
        /// <summary>
        /// The window has a window menu on its title bar. The WS_CAPTION style must also be specified.
        /// </summary>
        WS_SYSMENU = 0x00080000,
        /// <summary>
        /// The window has a sizing border. Same as the WS_SIZEBOX style.
        /// </summary>
        WS_THICKFRAME = 0x00040000,
        /// <summary>
        /// The window is the first control of a group of controls.
        /// </summary>
        WS_GROUP = 0x00020000,
        /// <summary>
        /// The window is a control that can receive the keyboard focus when the user presses the TAB key.
        /// </summary>
        WS_TABSTOP = 0x00010000,
        /// <summary>
        /// The window has a maximize button.
        /// </summary>
        WS_MINIMIZEBOX = 0x00020000,
        /// <summary>
        /// The window has a maximize button.
        /// </summary>
        WS_MAXIMIZEBOX = 0x00010000,
        /// <summary>
        /// The window is an overlapped window. An overlapped window has a title bar and a border. 
        /// Same as the WS_OVERLAPPED style.
        /// </summary>
        WS_TILED = WS_OVERLAPPED,
        /// <summary>
        /// The window is initially minimized. Same as the WS_MINIMIZE style.
        /// </summary>
        WS_ICONIC = WS_MINIMIZE,
        /// <summary>
        /// The window has a sizing border. Same as the WS_THICKFRAME style.
        /// </summary>
        WS_SIZEBOX = WS_THICKFRAME,
        /// <summary>
        /// The window is an overlapped window. Same as the WS_OVERLAPPEDWINDOW style.
        /// </summary>
        WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
        /// <summary>
        /// The window is an overlapped window. Same as the WS_TILEDWINDOW style.
        /// </summary>
        WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED |
            WS_CAPTION |
            WS_SYSMENU |
            WS_THICKFRAME |
            WS_MINIMIZEBOX |
            WS_MAXIMIZEBOX),
        /// <summary>
        /// The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make 
        /// the window menu visible.
        /// </summary>
        WS_POPUPWINDOW = 0x80880000, //(WS_POPUP | WS_BORDER | WS_SYSMENU),  
        /// <summary>
        /// Same as the WS_CHILD style.
        /// </summary>
        WS_CHILDWINDOW = WS_CHILD,
    }

    /// <summary>
    /// Constants defining the extended Win32 windows styles.
    /// </summary>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/ff700543(v=vs.85).aspx">
    /// MSDN documentation of extended styles
    /// </seealso>
    [Flags]
    public enum WindowExStyles : uint
    {
        WS_EX_DLGMODALFRAME = 0x00000001,
        WS_EX_NOPARENTNOTIFY = 0x00000004,
        WS_EX_TOPMOST = 0x00000008,
        WS_EX_ACCEPTFILES = 0x00000010,
        WS_EX_TRANSPARENT = 0x00000020,
        WS_EX_MDICHILD = 0x00000040,
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_WINDOWEDGE = 0x00000100,
        WS_EX_CLIENTEDGE = 0x00000200,
        WS_EX_CONTEXTHELP = 0x00000400,
        WS_EX_RIGHT = 0x00001000,
        WS_EX_LEFT = 0x00000000,
        WS_EX_RTLREADING = 0x00002000,
        WS_EX_LTRREADING = 0x00000000,
        WS_EX_LEFTSCROLLBAR = 0x00004000,
        WS_EX_RIGHTSCROLLBAR = 0x00000000,
        WS_EX_CONTROLPARENT = 0x00010000,
        WS_EX_STATICEDGE = 0x00020000,
        WS_EX_APPWINDOW = 0x00040000,
        WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
        WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),

        // for (_WIN32_WINNT >= 0x0500)
        WS_EX_LAYERED = 0x00080000,
        // for (WINVER >= 0x0500)
        WS_EX_NOINHERITLAYOUT = 0x00100000,
        WS_EX_LAYOUTRTL = 0x00400000,
        // for (_WIN32_WINNT >= 0x0501)
        WS_EX_COMPOSITED = 0x02000000,
        // for (_WIN32_WINNT >= 0x0500)
        WS_EX_NOACTIVATE = 0x08000000,
    }

    /// <summary>
    /// Win32 window class styles.
    /// </summary>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/ff729176(v=vs.85).aspx">
    /// Window Class Styles
    /// </seealso>
    [Flags]
    public enum ClassStyles : uint
    {
        /// <summary> Redraws the entire window if a movement or size adjustment changes the height 
        ///  of the client area. </summary>
        CS_VREDRAW = 0x0001,

        /// <summary> Redraws the entire window if a movement or size adjustment changes the width 
        /// of the client area. </summary>
        CS_HREDRAW = 0x0002,

        /// <summary> Sends double-click messages to the window procedure when the user double-clicks 
        ///  the mouse while the cursor is within a window belonging to the class. </summary>
        CS_DBLCLKS = 0x0008,

        /// <summary> Allocates a unique device context for each window in the class.</summary>
        CS_OWNDC = 0x0020,

        /// <summary> Allocates one device context to be shared by all windows in the class. 
        /// Because window classes are process specific, it is possible for multiple threads of an application 
        /// to create a window of the same class. 
        /// It is also possible for the threads to attempt to use the device context simultaneously. </summary>
        CS_CLASSDC = 0x0040,

        /// <summary> Sets the clipping rectangle of the child window to that of the parent window 
        ///  so that the child can draw on the parent. A window with the CS_PARENTDC style bit receives 
        ///  a regular device context from the system's cache of device contexts. It does not give the child 
        ///  the parent's device context or device context settings. 
        ///  Specifying CS_PARENTDC enhances an application's performance.  </summary>
        CS_PARENTDC = 0x0080,

        /// <summary> Disables Close on the window menu. </summary>
        CS_NOCLOSE = 0x0200,

        /// <summary> Saves, as a bitmap, the portion of the screen image obscured by a window of this class. 
        ///  When the window is removed, the system uses the saved bitmap to restore the screen image, 
        ///  including other windows that were obscured. Therefore, the system does not send WM_PAINT messages 
        ///  to windows that were obscured if the memory used by the bitmap has not been discarded 
        ///  and if other screen actions have not invalidated the stored image.
        ///
        /// This style is useful for small windows (for example, menus or dialog boxes) that are displayed 
        /// briefly and then removed before other screen activity takes place. 
        /// This style increases the time required to display the window, 
        /// because the system must first allocate memory to store the bitmap. </summary>
        CS_SAVEBITS = 0x0800,

        /// <summary> Aligns the window's client area on a byte boundary (in the x direction). 
        ///  This style affects the width of the window and its horizontal placement on the display.</summary>
        CS_BYTEALIGNCLIENT = 0x1000,

        /// <summary> Aligns the window on a byte boundary (in the x direction). 
        ///  This style affects the width of the window and its horizontal placement on the display.</summary>
        CS_BYTEALIGNWINDOW = 0x2000,

        /// <summary> Indicates that the window class is an application global class. 
        /// For more information, see the "Application Global Classes" section of About Window Classes.
        /// </summary>
        CS_GLOBALCLASS = 0x4000,
    }

    /// <summary>
    /// Bit-field of flags for specifying edit control styles. <br/>
    /// To create an edit control using the CreateWindow or CreateWindowEx function, specify the EDIT class,
    /// appropriate window style constants, and a combination of the following edit control styles. After the
    /// control has been created, these styles cannot be modified, except as noted.
    /// </summary>
    [Flags]
    public enum EditControlStyles : uint
    {
        /// <summary> Aligns text with the left margin. </summary>
        ES_LEFT = 0x00000000,

        /// <summary> Centers text in a single-line or multiline edit control. </summary>
        ES_CENTER = 0x00000001,

        /// <summary> Right-aligns text in a single-line or multiline edit control. </summary>
        ES_RIGHT = 0x00000002,

        /// <summary> Designates a multiline edit control. The default is single-line edit control. </summary>
        ES_MULTILINE = 0x00000004,

        /// <summary> Converts all characters to uppercase as they are typed into the edit control. </summary>
        ES_UPPERCASE = 0x00000008,

        /// <summary> Converts all characters to lowercase as they are typed into the edit control.
        /// To change this style after the control has been created, use SetWindowLong.
        /// </summary>
        ES_LOWERCASE = 0x00000010,

        /// <summary>
        /// Displays an asterisk (*) for each character typed into the edit control. This style is valid only for
        /// single-line edit controls.
        /// </summary>
        ES_PASSWORD = 0x00000020,

        /// <summary>
        /// Automatically scrolls text up one page when the user presses the ENTER key on the last line.
        /// </summary>
        ES_AUTOVSCROLL = 0x00000040,

        /// <summary>
        /// Automatically scrolls text to the right by 10 characters when the user types a character at the end
        /// of the line. When the user presses the ENTER key, the control scrolls all text back to position zero.
        /// </summary>
        ES_AUTOHSCROLL = 0x00000080,

        /// <summary>
        /// Negates the default behavior for an edit control. The default behavior hides the selection when the
        /// control loses the input focus and inverts the selection when the control receives the input focus. If
        /// you specify ES_NOHIDESEL, the selected text is inverted, even if the control does not have the focus.
        /// </summary>
        ES_NOHIDESEL = 0x00000100,

        /// <summary>
        /// Converts text entered in the edit control. The text is converted from the Windows character set to
        /// the OEM character set and then back to the Windows character set. This ensures proper character
        /// conversion when the application calls the CharToOem function to convert a Windows string in the edit
        /// control to OEM characters. This style is most useful for edit controls that contain file names that
        /// will be used on file systems that do not support Unicode.<br/>
        /// 
        /// To change this style after the control has been created, use SetWindowLong.
        /// </summary>
        ES_OEMCONVERT = 0x00000400,

        /// <summary> Prevents the user from typing or editing text in the edit control. </summary>
        ES_READONLY = 0x00000800,

        /// <summary>
        /// Specifies that a carriage return be inserted when the user presses the ENTER key while entering text
        /// into a multiline edit control in a dialog box. If you do not specify this style, pressing the ENTER
        /// key has the same effect as pressing the dialog box's default push button. This style has no effect on
        /// a single-line edit control.
        /// 
        /// To change this style after the control has been created, use SetWindowLong.
        /// </summary>
        ES_WANTRETURN = 0x00001000,

        /// <summary>
        /// Allows only digits to be entered into the edit control. Note that, even with this set, it is still
        /// possible to paste non-digits into the edit control.
        /// </summary>
        ES_NUMBER = 0x00002000,
    };

    /// <summary> Bit-field of flags for specifying Rich Edit Control Styles. 
    /// Following window styles are unique to rich edit controls.
    /// </summary>
    [Flags]
    public enum RichEditControlStyles : uint
    {
        /// <summary>
        /// Displays the control with a sunken border style so that the rich edit control appears recessed
        /// into its parent window.
        /// </summary>
        ES_SUNKEN = 0x00004000,

        /// <summary>
        /// Preserves the selection when the control loses the focus. By default, the entire contents of the
        /// control are selected when it regains the focus.
        /// </summary>
        ES_SAVESEL = 0x00008000,

        /// <summary> Disables scroll bars instead of hiding them when they are not needed. </summary>
        ES_DISABLENOSCROLL = 0x00002000,

        /// <summary>
        /// Adds space to the left margin where the cursor changes to a right-up arrow, allowing the user to
        /// select full lines of text.
        /// </summary>
        ES_SELECTIONBAR = 0x01000000,

        /// <summary> Same as ES_UPPERCASE, but re-used to completely disable OLE drag'n'drop. </summary>
        ES_NOOLEDRAGDROP = 0x00000008,

        /// <summary>
        /// Draws text and objects in a vertical direction. This style is available for Asian-language support
        /// only.
        /// </summary>
        ES_VERTICAL = 0x00400000,

        /// <summary>
        /// Disables the IME operation. This style is available for Asian language support only.
        /// </summary>
        ES_NOIME = 0x00080000,

        /// <summary>
        /// Directs the rich edit control to allow the application to handle all IME operations. This style is
        /// available for Asian language support only.
        /// </summary>
        ES_SELFIME = 0x00040000,
    };

    /// <summary>
    /// System hook types, used as an argument of <see cref="User32.SetWindowsHookEx"/>.
    /// </summary>
    /// <seealso cref="User32.SetWindowsHookEx"/>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644990(v=vs.85).aspx">
    /// MSDN documentation of SetWindowsHookEx API</seealso>
    public enum HookType : int
    {
        /// <summary> An enum constant representing the WH_JOURNALRECORD hook, which enables you to monitor 
        /// and record input events.
        /// Typically, you use this hook to record a sequence of mouse and keyboard events 
        /// to play back later by using WH_JOURNALPLAYBACK. 
        /// The WH_JOURNALRECORD hook is a global hook—it cannot be used as a thread-specific hook.
        /// </summary>
        WH_JOURNALRECORD = 0,

        /// <summary> An enum constant representing The WH_JOURNALPLAYBACK hook enables an application to insert 
        /// messages into the system message queue. You can use this hook to play back a series of mouse 
        /// and keyboard events recorded earlier by using WH_JOURNALRECORD. 
        /// Regular mouse and keyboard input is disabled as long as a WH_JOURNALPLAYBACK hook is installed. 
        /// A WH_JOURNALPLAYBACK hook is a global hook—it cannot be used as a thread-specific hook.
        /// </summary>
        WH_JOURNALPLAYBACK = 1,

        /// <summary> The WH_KEYBOARD hook enables an application to monitor message traffic for WM_KEYDOWN 
        /// and WM_KEYUP messages about to be returned by the GetMessage or PeekMessage function. 
        /// You can use the WH_KEYBOARD hook to monitor keyboard input posted to a message queue. </summary>
        WH_KEYBOARD = 2,

        /// <summary> The WH_GETMESSAGE hook enables an application to monitor messages about to be returned 
        /// by the GetMessage or PeekMessage function. You can use the WH_GETMESSAGE hook to monitor 
        /// mouse and keyboard input and other messages posted to the message queue.
        /// </summary>
        WH_GETMESSAGE = 3,

        /// <summary> The WH_CALLWNDPROC and WH_CALLWNDPROCRET hooks enable you to monitor messages 
        /// sent to window procedures. 
        /// The system calls a WH_CALLWNDPROC hook procedure before passing the message 
        /// to the receiving window procedure, and calls the WH_CALLWNDPROCRET hook procedure 
        /// after the window procedure has processed the message.
        /// </summary>
        /// <seealso cref="WH_CALLWNDPROCRET"/>
        WH_CALLWNDPROC = 4,

        /// <summary> The system calls a WH_CBT hook procedure before activating, creating, destroying, minimizing, 
        /// maximizing, moving, or sizing a window; before completing a system command; 
        /// before removing a mouse or keyboard event from the system message queue; 
        /// before setting the input focus; or before synchronizing with the system message queue. 
        /// The value the hook procedure returns determines whether the system allows or prevents 
        /// one of these operations. 
        /// The WH_CBT hook is intended primarily for computer-based training (CBT) applications.
        /// </summary>
        WH_CBT = 5,

        /// <summary> The WH_MSGFILTER and WH_SYSMSGFILTER hooks enable you to perform message filtering 
        /// during modal loops that is equivalent to the filtering done in the main message loop. 
        /// For example, an application often examines a new message in the main loop between the time it retrieves 
        /// the message from the queue and the time it dispatches the message, 
        /// performing special processing as appropriate. However, during a modal loop, 
        /// the system retrieves and dispatches messages without allowing an application the chance to filter 
        /// the messages in its main message loop. 
        /// If an application installs a WH_MSGFILTER or WH_SYSMSGFILTER hook procedure, 
        /// the system calls the procedure during the modal loop.
        /// </summary>
        WH_SYSMSGFILTER = 6,

        /// <summary> The WH_MOUSE hook enables you to monitor mouse messages about to be returned 
        /// by the GetMessage or PeekMessage function. 
        /// You can use the WH_MOUSE hook to monitor mouse input posted to a message queue.
        /// </summary>
        WH_MOUSE = 7,

        /// <summary> This hook is currently not implemented. </summary>
        WH_HARDWARE = 8,

        /// <summary> The system calls a WH_DEBUG hook procedure before calling hook procedures associated 
        /// with any other hook in the system. You can use this hook to determine whether to allow the system 
        /// to call hook procedures associated with other types of hooks.
        /// </summary>
        WH_DEBUG = 9,

        /// <summary> A shell application can use the WH_SHELL hook to receive important notifications. 
        /// The system calls a WH_SHELL hook procedure when the shell application is about to be activated 
        /// and when a top-level window is created or destroyed.
        /// Note that custom shell applications do not receive WH_SHELL messages. 
        /// Therefore, any application that registers itself as the default shell must call 
        /// the SystemParametersInfo function before it (or any other application) can receive WH_SHELL messages. 
        /// This function must be called with SPI_SETMINIMIZEDMETRICS and a MINIMIZEDMETRICS structure. 
        /// Set the iArrange member of this structure to ARW_HIDE.
        /// </summary>
        WH_SHELL = 10,

        /// <summary> The WH_FOREGROUNDIDLE hook enables you to perform low priority tasks during times when 
        /// its foreground thread is idle. The system calls a WH_FOREGROUNDIDLE hook procedure when 
        /// the application's foreground thread is about to become idle.
        /// </summary>
        WH_FOREGROUNDIDLE = 11,

        /// <summary> The WH_CALLWNDPROC and WH_CALLWNDPROCRET hooks enable you to monitor messages 
        /// sent to window procedures. 
        /// The WH_CALLWNDPROCRET hook passes a pointer to a CWPRETSTRUCT structure to the hook procedure. 
        /// The structure contains the return value from the window procedure that processed the message, 
        /// as well as the message parameters associated with the message. 
        /// Subclassing the window does not work for messages set between processes.
        /// </summary>
        /// <seealso cref="WH_CALLWNDPROC"/>
        WH_CALLWNDPROCRET = 12,

        /// <summary> The WH_KEYBOARD_LL hook enables you to monitor keyboard input events about to be posted 
        /// in a thread input queue. </summary>
        WH_KEYBOARD_LL = 13,

        /// <summary> The WH_MOUSE_LL hook enables you to monitor mouse input events about to be posted 
        /// in a thread input queue.
        /// </summary>
        WH_MOUSE_LL = 14
    }

    /// <summary> Enum whose values represent windows messages. </summary>
    public enum WM
    {
        /// <summary>
        /// Sent when an application requests that a window be created by calling the CreateWindowEx or
        /// CreateWindow function. (The message is sent before the function returns.) .
        /// </summary>
        WM_CREATE = 0x0001,

        /// <summary>
        /// Sent when a window is being destroyed. It is sent to the window procedure of the window being
        /// destroyed after the window is removed from the screen.
        /// </summary>
        WM_DESTROY = 0x0002,

        /// <summary> Sent after a window has been moved.</summary>
        WM_MOVE = 0x0003,

        /// <summary> Sent to a window after its size has changed.</summary>
        WM_SIZE = 0x0005,

        /// <summary> Sent to both the window being activated and the window being deactivated. 
        /// If the windows use the same input queue, the message is sent synchronously, 
        /// first to the window procedure of the top-level window being deactivated, 
        /// then to the window procedure of the top-level window being activated. 
        /// If the windows use different input queues, the message is sent asynchronously, 
        /// so the window is activated immediately.
        /// </summary>
        WM_ACTIVATE = 0x0006,

        /// <summary> Sent to a window after it has gained the keyboard focus.</summary>
        WM_SETFOCUS = 0x0007,

        /// <summary> Sent to a window immediately before it loses the keyboard focus.</summary>
        WM_KILLFOCUS = 0x0008,

        /// <summary>
        /// Sent when an application changes the enabled state of a window. It is sent to the window whose
        /// enabled state is changing. This message is sent before the EnableWindow function returns, but after
        /// the enabled state (WS_DISABLED style bit) of the window has changed.
        /// </summary>
        WM_ENABLE = 0x000A,

        /// <summary>
        /// An application sends the WM_SETREDRAW message to a window to allow changes in that window to be
        /// redrawn or to prevent changes in that window from being redrawn.
        /// </summary>
        WM_SETREDRAW = 0x000B,

        /// <summary> Sets the text of a window.</summary>
        WM_SETTEXT = 0x000C,

        /// <summary> Copies the text that corresponds to a window into a buffer provided by the caller.
        /// </summary>
        WM_GETTEXT = 0x000D,

        /// <summary> Determines the length, in characters, of the text associated with a window.</summary>
        WM_GETTEXTLENGTH = 0x000E,

        /// <summary>
        /// The WM_PAINT message is sent when the system or another application makes a request to paint a
        /// portion of an application's window. The message is sent when the UpdateWindow or RedrawWindow
        /// function is called, or by the DispatchMessage function when the application obtains a WM_PAINT
        /// message by using the GetMessage or PeekMessage function.<br/>
        /// 
        /// A window receives this message through its WindowProc function.
        /// </summary>
        WM_PAINT = 0x000F,

        /// <summary>
        /// Sent as a signal that a window or an application should terminate.
        /// </summary>
        WM_CLOSE = 0x0010,

        /// <summary>
        /// The WM_QUERYENDSESSION message is sent when the user chooses to end the session or when an
        /// application calls one of the system shutdown functions (like ExitWindows). If any application returns
        /// zero, the session is not ended. The system stops sending WM_QUERYENDSESSION messages as soon as one
        /// application returns zero.<br/>
        /// After processing this message, the system sends the WM_ENDSESSION message with the wParam parameter
        /// set to the results of the WM_QUERYENDSESSION message.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is reserved for future use.
        /// <br/>
        /// lParam <br/>
        /// This parameter can be one or more of the following values.
        /// Note that this parameter is a bit mask, to test for this value, use a bit-wise operations.
        /// If this parameter is 0, the system is shutting down or restarting 
        /// (it is not possible to determine which event is occurring).
        /// <list type="bullet">
        /// <item><b>ENDSESSION_CLOSEAPP</b> 0x00000001<br/>
        /// The application is using a file that must be replaced, the system is being serviced, 
        /// or system resources are exhausted. For more information, see Guidelines for Applications.
        /// </item>
        /// <item><b>ENDSESSION_CRITICAL</b> 0x40000000<br/>
        /// The application is forced to shut down.
        /// </item>
        /// <item><b>ENDSESSION_LOGOFF</b> 0x80000000<br/>
        /// The user is logging off. For more information, see Logging Off.
        /// </item>
        /// </list>
        /// </summary>
        WM_QUERYENDSESSION = 0x0011,

        /// <summary> Indicates a request to terminate an application, and is generated when the application 
        /// calls the PostQuitMessage function. This message causes the GetMessage function to return zero.
        /// </summary>
        WM_QUIT = 0x0012,

        /// <summary>
        /// Sent to an multiple document interface (MDI) child window in minimized ( 'iconized'  ) state
        /// when the user requests that the window be restored to its previous size and position.
        /// </summary>
        WM_QUERYOPEN = 0x0013,

        /// <summary>
        /// The WM_ENDSESSION message is sent to an application always after the system processes the results 
        /// of the WM_QUERYENDSESSION message, regardless whether the session is ending or not.
        /// Thus, The WM_ENDSESSION message informs the application whether the session is ending.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// If the session is being ended, this parameter is nonzero, the session can end any time
        /// after all applications have returned from processing this message. Otherwise, it is zero.
        /// <br/>
        /// lParam <br/>
        /// This parameter can be one or more of the following values.
        /// Note that this parameter is a bit mask, to test for this value, use a bit-wise operations.
        /// If this parameter is 0, the system is shutting down or restarting 
        /// (it is not possible to determine which event is occurring).
        /// <list type="bullet">
        /// <item><b>ENDSESSION_CLOSEAPP</b> 0x00000001<br/>
        /// The application is using a file that must be replaced, the system is being serviced, 
        /// or system resources are exhausted. For more information, see Guidelines for Applications.
        /// </item>
        /// <item><b>ENDSESSION_CRITICAL</b> 0x40000000<br/>
        /// The application is forced to shut down.
        /// </item>
        /// <item><b>ENDSESSION_LOGOFF</b> 0x80000000<br/>
        /// The user is logging off. For more information, see Logging Off.
        /// </item>
        /// </list>
        /// </summary>
        WM_ENDSESSION = 0x0016,

        /// <summary>
        /// The WM_ERASEBKGND message is sent when the window background must be erased (for example, when a window is resized). The message is sent to prepare 
        /// an invalidated portion of a window for painting. 
        /// </summary>
        WM_ERASEBKGND = 0x0014,

        /// <summary>
        /// This message is sent to all top-level windows when a change is made to a system color setting. 
        /// </summary>
        WM_SYSCOLORCHANGE = 0x0015,


        /// <summary>
        /// The WM_SHOWWINDOW message is sent to a window when the window is about to be hidden or shown.
        /// </summary>
        WM_SHOWWINDOW = 0x0018,


        /// <summary>
        /// An application sends the WM_WININICHANGE message to all top-level windows after making a change to the WIN.INI file. The SystemParametersInfo 
        /// function sends this message after an application uses the function to change a setting in WIN.INI. The WM_WININICHANGE message is provided only for 
        /// compatibility with earlier versions of the system. Applications should use the WM_SETTINGCHANGE message.
        /// </summary>
        WM_WININICHANGE = 0x001A,
        WM_SETTINGCHANGE = WM_WININICHANGE,

        /// <summary>
        /// The WM_DEVMODECHANGE message is sent to all top-level windows whenever the user changes device-mode settings. 
        /// </summary>
        WM_DEVMODECHANGE = 0x001B,

        /// <summary>
        /// The WM_ACTIVATEAPP message is sent when a window belonging to a different application than the active window is about to be activated. The message 
        /// is sent to the application whose window is being activated and to the application whose window is being deactivated.
        /// </summary>
        WM_ACTIVATEAPP = 0x001C,

        /// <summary>
        /// An application sends the WM_FONTCHANGE message to all top-level windows in the system after changing the pool of font resources. 
        /// </summary>
        WM_FONTCHANGE = 0x001D,


        /// <summary>
        /// A message that is sent whenever there is a change in the system time.
        /// </summary>
        WM_TIMECHANGE = 0x001E,

        /// <summary>
        /// The WM_CANCELMODE message is sent to cancel certain modes, such as mouse capture. For example, the system sends this message to the active window 
        /// when a dialog box or message box is displayed. Certain functions also send this message explicitly to the specified window regardless of whether it 
        /// is the active window. For example, the EnableWindow function sends this message when disabling the specified window.
        /// </summary>
        WM_CANCELMODE = 0x001F,

        /// <summary>
        /// The WM_SETCURSOR message is sent to a window if the mouse causes the cursor to move within a window and mouse input is not captured.
        /// </summary>
        WM_SETCURSOR = 0x0020,

        /// <summary>
        /// The WM_MOUSEACTIVATE message is sent when the cursor is in an inactive window and the user presses a mouse button. The parent window receives this 
        /// message only if the child window passes it to the DefWindowProc function.
        /// </summary>
        WM_MOUSEACTIVATE = 0x0021,

        /// <summary>
        /// The WM_CHILDACTIVATE message is sent to a child window when the user clicks the window's title bar or when the window is activated, moved, or 
        /// sized.
        /// </summary>
        WM_CHILDACTIVATE = 0x0022,

        /// <summary>
        /// The WM_QUEUESYNC message is sent by a computer-based training (CBT) application to separate user-input messages from other messages sent through 
        /// the WH_JOURNALPLAYBACK Hook procedure. 
        /// </summary>
        WM_QUEUESYNC = 0x0023,

        /// <summary>
        /// The WM_GETMINMAXINFO message is sent to a window when the size or position of the window is about to change. An application can use this message to 
        /// override the window's default maximized size and position, or its default minimum or maximum tracking size. 
        /// </summary>
        WM_GETMINMAXINFO = 0x0024,

        /// <summary>
        /// Windows NT 3.51 and earlier: The WM_PAINTICON message is sent to a minimized window when the icon is to be painted. This message is not sent by 
        /// newer versions of Microsoft Windows, except in unusual circumstances explained in the Remarks.
        /// </summary>
        WM_PAINTICON = 0x0026,

        /// <summary>
        /// Windows NT 3.51 and earlier: The WM_ICONERASEBKGND message is sent to a minimized window when the background of the icon must be filled before 
        /// painting the icon. A window receives this message only if a class icon is defined for the window; otherwise, WM_ERASEBKGND is sent. This message is 
        /// not sent by newer versions of Windows.
        /// </summary>
        WM_ICONERASEBKGND = 0x0027,

        /// <summary>
        /// The WM_NEXTDLGCTL message is sent to a dialog box procedure to set the keyboard focus to a different control in the dialog box. 
        /// </summary>
        WM_NEXTDLGCTL = 0x0028,

        /// <summary>
        /// The WM_SPOOLERSTATUS message is sent from Print Manager whenever a job is added to or removed from the Print Manager queue. 
        /// </summary>
        WM_SPOOLERSTATUS = 0x002A,

        /// <summary>
        /// The WM_DRAWITEM message is sent to the parent window of an owner-drawn button, combo box, list box, or menu when a visual aspect of the button, 
        /// combo box, list box, or menu has changed.
        /// </summary>
        WM_DRAWITEM = 0x002B,

        /// <summary>
        /// The WM_MEASUREITEM message is sent to the owner window of a combo box, list box, list view control, or menu item when the control or menu is 
        /// created.
        /// </summary>
        WM_MEASUREITEM = 0x002C,

        /// <summary>
        /// Sent to the owner of a list box or combo box when the list box or combo box is destroyed or when items are removed by the LB_DELETESTRING, 
        /// LB_RESETCONTENT, CB_DELETESTRING, or CB_RESETCONTENT message. The system sends a WM_DELETEITEM message for each deleted item. The system sends the 
        /// WM_DELETEITEM message for any deleted list box or combo box item with nonzero item data.
        /// </summary>
        WM_DELETEITEM = 0x002D,

        /// <summary>
        /// Sent by a list box with the LBS_WANTKEYBOARDINPUT style to its owner in response to a WM_KEYDOWN message. 
        /// </summary>
        WM_VKEYTOITEM = 0x002E,

        /// <summary>
        /// Sent by a list box with the LBS_WANTKEYBOARDINPUT style to its owner in response to a WM_CHAR message. 
        /// </summary>
        WM_CHARTOITEM = 0x002F,

        /// <summary>
        /// An application sends a WM_SETFONT message to specify the font that a control is to use when drawing text. 
        /// </summary>
        WM_SETFONT = 0x0030,

        /// <summary>
        /// An application sends a WM_GETFONT message to a control to retrieve the font with which the control is currently drawing its text. 
        /// </summary>
        WM_GETFONT = 0x0031,

        /// <summary>
        /// An application sends a WM_SETHOTKEY message to a window to associate a hot key with the window. When the user presses the hot key, the system 
        /// activates the window. 
        /// </summary>
        WM_SETHOTKEY = 0x0032,

        /// <summary>
        /// An application sends a WM_GETHOTKEY message to determine the hot key associated with a window. 
        /// </summary>
        WM_GETHOTKEY = 0x0033,

        /// <summary>
        /// The WM_QUERYDRAGICON message is sent to a minimized (iconic) window. The window is about to be dragged by the user but does not have an icon 
        /// defined for its class. An application can return a handle to an icon or cursor. The system displays this cursor or icon while the user drags the 
        /// icon.
        /// </summary>
        WM_QUERYDRAGICON = 0x0037,

        /// <summary>
        /// The system sends the WM_COMPAREITEM message to determine the relative position of a new item in the sorted list of an owner-drawn combo box or list 
        /// box. Whenever the application adds a new item, the system sends this message to the owner of a combo box or list box created with the CBS_SORT or 
        /// LBS_SORT style. 
        /// </summary>
        WM_COMPAREITEM = 0x0039,

        /// <summary>
        /// Active Accessibility sends the WM_GETOBJECT message to obtain information about an accessible object contained in a server application. 
        /// Applications never send this message directly. It is sent only by Active Accessibility in response to calls to AccessibleObjectFromPoint, 
        /// AccessibleObjectFromEvent, or AccessibleObjectFromWindow. However, server applications handle this message. 
        /// </summary>
        WM_GETOBJECT = 0x003D,

        /// <summary>
        /// The WM_COMPACTING message is sent to all top-level windows when the system detects more than 12.5 percent of system time over a 30- to 60-second 
        /// interval is being spent compacting memory. This indicates that system memory is low.
        /// </summary>
        WM_COMPACTING = 0x0041,

        /// <summary>
        /// The WM_WINDOWPOSCHANGING message is sent to a window whose size, position, or place in the Z order is about to change as a result of a call to the 
        /// SetWindowPos function or another window-management function.
        /// </summary>
        WM_WINDOWPOSCHANGING = 0x0046,

        /// <summary> Sent to a window whose size, position, or place in the Z order has changed 
        /// as a result of a call to the SetWindowPos function or another window-management function.
        /// </summary>
        WM_WINDOWPOSCHANGED = 0x0047,

        /// <summary>
        /// Notifies applications that the system, typically a battery-powered personal computer, is about to enter a suspended mode.
        /// Use: POWERBROADCAST
        /// </summary>
        WM_POWER = 0x0048,

        /// <summary>
        /// An application sends the WM_COPYDATA message to pass data to another application. 
        /// </summary>
        WM_COPYDATA = 0x004A,

        /// <summary>
        /// The WM_CANCELJOURNAL message is posted to an application when a user cancels the application's journaling activities. The message is posted with a 
        /// NULL window handle. 
        /// </summary>
        WM_CANCELJOURNAL = 0x004B,

        /// <summary>
        /// Sent by a common control to its parent window when an event has occurred or the control requires some information. 
        /// </summary>
        WM_NOTIFY = 0x004E,

        /// <summary>
        /// The WM_INPUTLANGCHANGEREQUEST message is posted to the window with the focus when the user chooses a new input language, either with the hotkey 
        /// (specified in the Keyboard control panel application) or from the indicator on the system taskbar. An application can accept the change by passing 
        /// the message to the DefWindowProc function or reject the change (and prevent it from taking place) by returning immediately. 
        /// </summary>
        WM_INPUTLANGCHANGEREQUEST = 0x0050,

        /// <summary>
        /// The WM_INPUTLANGCHANGE message is sent to the topmost affected window after an application's input language has been changed. You should make any 
        /// application-specific settings and pass the message to the DefWindowProc function, which passes the message to all first-level child windows. These 
        /// child windows can pass the message to DefWindowProc to have it pass the message to their child windows, and so on. 
        /// </summary>
        WM_INPUTLANGCHANGE = 0x0051,

        /// <summary>
        /// Sent to an application that has initiated a training card with Microsoft Windows Help. The message informs the application when the user clicks an 
        /// authorable button. An application initiates a training card by specifying the HELP_TCARD command in a call to the WinHelp function.
        /// </summary>
        WM_TCARD = 0x0052,

        /// <summary>
        /// Indicates that the user pressed the F1 key. If a menu is active when F1 is pressed, WM_HELP is sent to the window associated with the menu; 
        /// otherwise, WM_HELP is sent to the window that has the keyboard focus. If no window has the keyboard focus, WM_HELP is sent to the currently active 
        /// window. 
        /// </summary>
        WM_HELP = 0x0053,

        /// <summary>
        /// The WM_USERCHANGED message is sent to all windows after the user has logged on or off. When the user logs on or off, the system updates the user-
        /// specific settings. The system sends this message immediately after updating the settings.
        /// </summary>
        WM_USERCHANGED = 0x0054,

        /// <summary>
        /// Determines if a window accepts ANSI or Unicode structures in the WM_NOTIFY notification message. WM_NOTIFYFORMAT messages are sent from a common 
        /// control to its parent window and from the parent window to the common control.
        /// </summary>
        WM_NOTIFYFORMAT = 0x0055,

        /// <summary> Sent to a window when the SetWindowLong function is about to change one or more of the window's styles. </summary>
        WM_STYLECHANGING = 0x007C,

        /// <summary> Sent to a window after the SetWindowLong function has changed one or more of the window's styles. </summary>
        WM_STYLECHANGED = 0x007D,

        /// <summary>
        /// The WM_DISPLAYCHANGE message is sent to all windows when the display resolution has changed.
        /// </summary>
        WM_DISPLAYCHANGE = 0x007E,

        /// <summary>
        /// The WM_GETICON message is sent to a window to retrieve a handle to the large or small icon associated with a window. The system displays the large 
        /// icon in the ALT+TAB dialog, and the small icon in the window caption. 
        /// </summary>
        WM_GETICON = 0x007F,

        /// <summary>
        /// An application sends the WM_SETICON message to associate a new large or small icon with a window. The system displays the large icon in the ALT+TAB 
        /// dialog box, and the small icon in the window caption. 
        /// </summary>
        WM_SETICON = 0x0080,

        /// <summary>
        /// The WM_NCCREATE message is sent prior to the WM_CREATE message when a window is first created.
        /// </summary>
        WM_NCCREATE = 0x0081,

        /// <summary>
        /// The WM_NCDESTROY message informs a window that its nonclient area is being destroyed. The DestroyWindow function sends the WM_NCDESTROY message to 
        /// the window following the WM_DESTROY message. WM_DESTROY is used to free the allocated memory object associated with the window.  The WM_NCDESTROY 
        /// message is sent after the child windows have been destroyed. In contrast, WM_DESTROY is sent before the child windows are destroyed.
        /// </summary>
        WM_NCDESTROY = 0x0082,

        /// <summary>
        /// The WM_NCCALCSIZE message is sent when the size and position of a window's client area must be calculated. By processing this message, an 
        /// application can control the content of the window's client area when the size or position of the window changes.
        /// </summary>
        WM_NCCALCSIZE = 0x0083,

        /// <summary> Sent to a window in order to determine what part of the window corresponds 
        /// to a particular screen coordinate. This can happen, for example, when the cursor moves, 
        /// when a mouse button is pressed or released, 
        /// or in response to a call to a function such as WindowFromPoin. </summary>
        WM_NCHITTEST = 0x0084,

        /// <summary>
        /// The WM_NCPAINT message is sent to a window when its frame must be painted. 
        /// </summary>
        WM_NCPAINT = 0x0085,

        /// <summary>
        /// The WM_NCACTIVATE message is sent to a window when its nonclient area needs to be changed to indicate an active or inactive state.
        /// </summary>
        WM_NCACTIVATE = 0x0086,

        /// <summary>
        /// The WM_GETDLGCODE message is sent to the window procedure associated with a control. By default, the system handles all keyboard input to the 
        /// control; the system interprets certain types of keyboard input as dialog box navigation keys. To override this default behavior, the control can 
        /// respond to the WM_GETDLGCODE message to indicate the types of input it wants to process itself.
        /// </summary>
        WM_GETDLGCODE = 0x0087,

        /// <summary>
        /// The WM_SYNCPAINT message is used to synchronize painting while avoiding linking independent GUI threads.
        /// </summary>
        WM_SYNCPAINT = 0x0088,

        // For definitions of 0x0090 - 0x0095 see
        // https://social.msdn.microsoft.com/Forums/windowsapps/en-US/f677f319-9f02-4438-92fb-6e776924425d/windowproc-and-messages-0x90-0x91-0x92-0x93
        WM_UAHDESTROYWINDOW = 0x0090,

        WM_UAHDRAWMENU = 0x0091,
        WM_UAHDRAWMENUITEM = 0x0092,
        WM_UAHINITMENU = 0x0093,
        WM_UAHMEASUREMENUITEM = 0x0094,
        WM_UAHNCPAINTMENUPOPUP = 0x0095,

        /// <summary>
        /// The WM_NCMOUSEMOVE message is posted to a window when the cursor is moved within the nonclient area of the window. This message is posted to the 
        /// window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCMOUSEMOVE = 0x00A0,

        /// <summary>
        /// The WM_NCLBUTTONDOWN message is posted when the user presses the left mouse button while the cursor is within the nonclient area of a window. This 
        /// message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCLBUTTONDOWN = 0x00A1,

        /// <summary>
        /// The WM_NCLBUTTONUP message is posted when the user releases the left mouse button while the cursor is within the nonclient area of a window. This 
        /// message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCLBUTTONUP = 0x00A2,

        /// <summary>
        /// The WM_NCLBUTTONDBLCLK message is posted when the user double-clicks the left mouse button while the cursor is within the nonclient area of a 
        /// window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCLBUTTONDBLCLK = 0x00A3,

        /// <summary>
        /// The WM_NCRBUTTONDOWN message is posted when the user presses the right mouse button while the cursor is within the nonclient area of a window. This 
        /// message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCRBUTTONDOWN = 0x00A4,

        /// <summary>
        /// The WM_NCRBUTTONUP message is posted when the user releases the right mouse button while the cursor is within the nonclient area of a window. This 
        /// message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCRBUTTONUP = 0x00A5,

        /// <summary>
        /// The WM_NCRBUTTONDBLCLK message is posted when the user double-clicks the right mouse button while the cursor is within the nonclient area of a 
        /// window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCRBUTTONDBLCLK = 0x00A6,

        /// <summary>
        /// The WM_NCMBUTTONDOWN message is posted when the user presses the middle mouse button while the cursor is within the nonclient area of a window. 
        /// This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCMBUTTONDOWN = 0x00A7,

        /// <summary>
        /// The WM_NCMBUTTONUP message is posted when the user releases the middle mouse button while the cursor is within the nonclient area of a window. 
        /// This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCMBUTTONUP = 0x00A8,

        /// <summary>
        /// The WM_NCMBUTTONDBLCLK message is posted when the user double-clicks the middle mouse button while the cursor is within the nonclient area of a 
        /// window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCMBUTTONDBLCLK = 0x00A9,

        /// <summary>
        /// The WM_NCXBUTTONDOWN message is posted when the user presses the first or second X button while the cursor is in the nonclient area of a window. 
        /// This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCXBUTTONDOWN = 0x00AB,

        /// <summary>
        /// The WM_NCXBUTTONUP message is posted when the user releases the first or second X button while the cursor is in the nonclient area of a window. 
        /// This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCXBUTTONUP = 0x00AC,

        /// <summary>
        /// The WM_NCXBUTTONDBLCLK message is posted when the user double-clicks the first or second X button while the cursor is in the nonclient area of a 
        /// window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        WM_NCXBUTTONDBLCLK = 0x00AD,

        /// <summary>
        /// The WM_INPUT_DEVICE_CHANGE message is sent to the window that registered to receive raw input. A window receives this message through its 
        /// WindowProc function.
        /// </summary>
        WM_INPUT_DEVICE_CHANGE = 0x00FE,

        /// <summary>
        /// The WM_INPUT message is sent to the window that is getting raw input. 
        /// </summary>
        WM_INPUT = 0x00FF,

        /// <summary>
        /// The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem key is pressed. A nonsystem key is a key that is pressed 
        /// when the ALT key is not pressed. 
        /// </summary>
        WM_KEYDOWN = 0x0100,

        /// <summary> Is the same as <see cref="WM_KEYDOWN"/>. </summary>
        WM_KEYFIRST = WM_KEYDOWN,

        /// <summary>
        /// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem key is released. A nonsystem key is a key that is pressed 
        /// when the ALT key is not pressed, or a keyboard key that is pressed when a window has the keyboard focus. 
        /// </summary>
        WM_KEYUP = 0x0101,

        /// <summary>
        /// The WM_CHAR message is posted to the window with the keyboard focus when a WM_KEYDOWN message is translated by the TranslateMessage function. The 
        /// WM_CHAR message contains the character code of the key that was pressed. 
        /// </summary>
        WM_CHAR = 0x0102,

        /// <summary>
        /// The WM_DEADCHAR message is posted to the window with the keyboard focus when a WM_KEYUP message is translated by the TranslateMessage function. 
        /// WM_DEADCHAR specifies a character code generated by a dead key. A dead key is a key that generates a character, such as the umlaut (double-dot), 
        /// that is combined with another character to form a composite character. For example, the umlaut-O character (Ö) is generated by typing the dead key 
        /// for the umlaut character, and then typing the O key. 
        /// </summary>
        WM_DEADCHAR = 0x0103,

        /// <summary>
        /// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user presses the F10 key (which activates the menu bar) or holds 
        /// down the ALT key and then presses another key. It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYDOWN 
        /// message is sent to the active window. The window that receives the message can distinguish between these two contexts by checking the context code 
        /// in the lParam parameter. 
        /// </summary>
        WM_SYSKEYDOWN = 0x0104,

        /// <summary>
        /// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user releases a key that was pressed while the ALT key was held 
        /// down. It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent to the active window. The 
        /// window that receives the message can distinguish between these two contexts by checking the context code in the lParam parameter. 
        /// </summary>
        WM_SYSKEYUP = 0x0105,

        /// <summary>
        /// The WM_SYSCHAR message is posted to the window with the keyboard focus when a WM_SYSKEYDOWN message is translated by the TranslateMessage function. 
        /// It specifies the character code of a system character key — that is, a character key that is pressed while the ALT key is down. 
        /// </summary>
        WM_SYSCHAR = 0x0106,

        /// <summary>
        /// The WM_SYSDEADCHAR message is sent to the window with the keyboard focus when a WM_SYSKEYDOWN message is translated by the TranslateMessage 
        /// function. WM_SYSDEADCHAR specifies the character code of a system dead key — that is, a dead key that is pressed while holding down the ALT key. 
        /// </summary>
        WM_SYSDEADCHAR = 0x0107,

        /// <summary> Previously indicated last key message. </summary>
        WM_KEYLAST = 0x0108,

        /// <summary>
        /// The WM_UNICHAR message is posted to the window with the keyboard focus when a WM_KEYDOWN message is translated by the TranslateMessage function. 
        /// The WM_UNICHAR message contains the character code of the key that was pressed. The WM_UNICHAR message is equivalent to WM_CHAR, but it uses 
        /// Unicode Transformation Format (UTF)-32, whereas WM_CHAR uses UTF-16. It is designed to send or post Unicode characters to ANSI windows and it can 
        /// can handle Unicode Supplementary Plane characters.
        /// </summary>
        WM_UNICHAR = 0x0109,

        /// <summary>
        /// Sent immediately before the IME generates the composition string as a result of a keystroke. A window receives this message through its WindowProc 
        /// function. 
        /// </summary>
        WM_IME_STARTCOMPOSITION = 0x010D,

        /// <summary>
        /// Sent to an application when the IME ends composition. A window receives this message through its WindowProc function. 
        /// </summary>
        WM_IME_ENDCOMPOSITION = 0x010E,

        /// <summary>
        /// Sent to an application when the IME changes composition status as a result of a keystroke. A window receives this message through its WindowProc 
        /// function. 
        /// </summary>
        WM_IME_COMPOSITION = 0x010F,

        /// <summary>
        /// The WM_INITDIALOG message is sent to the dialog box procedure immediately before a dialog box is displayed. Dialog box procedures typically use 
        /// this message to initialize controls and carry out any other initialization tasks that affect the appearance of the dialog box. 
        /// </summary>
        WM_INITDIALOG = 0x0110,

        /// <summary>
        /// The WM_COMMAND message is sent when the user selects a command item from a menu, when a control sends a notification message to its parent window, 
        /// or when an accelerator keystroke is translated. 
        /// </summary>
        WM_COMMAND = 0x0111,

        /// <summary> A window receives this message when the user chooses a command from the Window menu 
        /// (formerly known as the system or control menu) or when the user chooses the maximize button, 
        /// minimize button, restore button, or close button. 
        /// </summary>
        WM_SYSCOMMAND = 0x0112,

        /// <summary>
        /// The WM_TIMER message is posted to the installing thread's message queue when a timer expires. The message is posted by the GetMessage or 
        /// PeekMessage function. 
        /// </summary>
        WM_TIMER = 0x0113,

        /// <summary>
        /// The WM_HSCROLL message is sent to a window when a scroll event occurs in the window's standard
        /// horizontal scroll bar. This message is also sent to the owner of a horizontal scroll bar control when
        /// a scroll event occurs in the control.
        /// </summary>
        WM_HSCROLL = 0x0114,

        /// <summary> The WM_VSCROLL message is sent to a window when a scroll event occurs in the window's 
        /// standard vertical scroll bar. 
        /// This message is also sent to the owner of a vertical scroll bar control when a scroll event occurs 
        /// in the control. </summary>
        WM_VSCROLL = 0x0115,

        /// <summary>
        /// The WM_INITMENU message is sent when a menu is about to become active. It occurs when the user clicks an item on the menu bar or presses a menu 
        /// key. This allows the application to modify the menu before it is displayed. 
        /// </summary>
        WM_INITMENU = 0x0116,

        /// <summary>
        /// The WM_INITMENUPOPUP message is sent when a drop-down menu or submenu is about to become active. This allows an application to modify the menu 
        /// before it is displayed, without changing the entire menu. 
        /// </summary>
        WM_INITMENUPOPUP = 0x0117,

        /// <summary>
        /// WM_SYSTIMER is a well-known yet still undocumented message. Windows uses WM_SYSTIMER for internal actions like scrolling.
        /// </summary>
        WM_SYSTIMER = 0x118,

        /// <summary>
        /// The WM_MENUSELECT message is sent to a menu's owner window when the user selects a menu item. 
        /// </summary>
        WM_MENUSELECT = 0x011F,

        /// <summary>
        /// The WM_MENUCHAR message is sent when a menu is active and the user presses a key that does not correspond to any mnemonic or accelerator key. This 
        /// message is sent to the window that owns the menu. 
        /// </summary>
        WM_MENUCHAR = 0x0120,

        /// <summary>
        /// The WM_ENTERIDLE message is sent to the owner window of a modal dialog box or menu that is entering an idle state. A modal dialog box or menu 
        /// enters an idle state when no messages are waiting in its queue after it has processed one or more previous messages. 
        /// </summary>
        WM_ENTERIDLE = 0x0121,

        /// <summary>
        /// The WM_MENURBUTTONUP message is sent when the user releases the right mouse button while the cursor is on a menu item. 
        /// </summary>
        WM_MENURBUTTONUP = 0x0122,

        /// <summary>
        /// The WM_MENUDRAG message is sent to the owner of a drag-and-drop menu when the user drags a menu item. 
        /// </summary>
        WM_MENUDRAG = 0x0123,

        /// <summary>
        /// The WM_MENUGETOBJECT message is sent to the owner of a drag-and-drop menu when the mouse cursor enters a menu item or moves from the center of the 
        /// item to the top or bottom of the item. 
        /// </summary>
        WM_MENUGETOBJECT = 0x0124,

        /// <summary>
        /// The WM_UNINITMENUPOPUP message is sent when a drop-down menu or submenu has been destroyed. 
        /// </summary>
        WM_UNINITMENUPOPUP = 0x0125,

        /// <summary>
        /// The WM_MENUCOMMAND message is sent when the user makes a selection from a menu. 
        /// </summary>
        WM_MENUCOMMAND = 0x0126,

        /// <summary>
        /// An application sends the WM_CHANGEUISTATE message to indicate that the user interface (UI) state should be changed.
        /// </summary>
        WM_CHANGEUISTATE = 0x0127,

        /// <summary>
        /// An application sends the WM_UPDATEUISTATE message to change the user interface (UI) state for the specified window and all its child windows.
        /// </summary>
        WM_UPDATEUISTATE = 0x0128,

        /// <summary>
        /// An application sends the WM_QUERYUISTATE message to retrieve the user interface (UI) state for a window.
        /// </summary>
        WM_QUERYUISTATE = 0x0129,

        /// <summary>
        /// The WM_CTLCOLORMSGBOX message is sent to the owner window of a message box before Windows draws the message box. By responding to this message, the 
        /// owner window can set the text and background colors of the message box by using the given display device context handle. 
        /// </summary>
        WM_CTLCOLORMSGBOX = 0x0132,

        /// <summary>
        /// An edit control that is not read-only or disabled sends the WM_CTLCOLOREDIT message to its parent window when the control is about to be drawn. By 
        /// responding to this message, the parent window can use the specified device context handle to set the text and background colors of the edit control.
        /// </summary>
        WM_CTLCOLOREDIT = 0x0133,

        /// <summary>
        /// Sent to the parent window of a list box before the system draws the list box. By responding to this message, the parent window can set the text and 
        /// background colors of the list box by using the specified display device context handle. 
        /// </summary>
        WM_CTLCOLORLISTBOX = 0x0134,

        /// <summary>
        /// The WM_CTLCOLORBTN message is sent to the parent window of a button before drawing the button. The parent window can change the button's text and 
        /// background colors. However, only owner-drawn buttons respond to the parent window processing this message. 
        /// </summary>
        WM_CTLCOLORBTN = 0x0135,

        /// <summary>
        /// The WM_CTLCOLORDLG message is sent to a dialog box before the system draws the dialog box. By responding to this message, the dialog box can set 
        /// its text and background colors using the specified display device context handle. 
        /// </summary>
        WM_CTLCOLORDLG = 0x0136,

        /// <summary>
        /// The WM_CTLCOLORSCROLLBAR message is sent to the parent window of a scroll bar control when the control is about to be drawn. By responding to this 
        /// message, the parent window can use the display context handle to set the background color of the scroll bar control. 
        /// </summary>
        WM_CTLCOLORSCROLLBAR = 0x0137,

        /// <summary>
        /// A static control, or an edit control that is read-only or disabled, sends the WM_CTLCOLORSTATIC message to its parent window when the control is 
        /// about to be drawn. By responding to this message, the parent window can use the specified device context handle to set the text and background 
        /// colors of the static control. 
        /// </summary>
        WM_CTLCOLORSTATIC = 0x0138,

        /// <summary>
        /// Use WM_MOUSEFIRST to specify the first mouse message. Use the PeekMessage() Function.
        /// </summary>
        WM_MOUSEFIRST = 0x0200,

        /// <summary>
        /// The WM_MOUSEMOVE message is posted to a window when the cursor moves. If the mouse is not captured, the message is posted to the window that 
        /// contains the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_MOUSEMOVE = 0x0200,

        /// <summary> Posted when the user presses the left mouse button while the cursor is in the client area
        /// of a window.</summary>
        WM_LBUTTONDOWN = 0x0201,

        /// <summary> Posted when the user releases the left mouse button while the cursor is in the client area 
        /// of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. 
        /// Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_LBUTTONUP = 0x0202,

        /// <summary> Posted when the user double-clicks the left mouse button while the cursor is 
        /// in the client area of a window. If the mouse is not captured, the message is posted to the window 
        /// beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_LBUTTONDBLCLK = 0x0203,

        /// <summary> Posted when the user presses the right mouse button 
        ///           while the cursor is in the client area of a window. </summary>
        WM_RBUTTONDOWN = 0x0204,

        /// <summary> Posted when the user releases the right mouse button while the cursor is 
        /// in the client area of a window. If the mouse is not captured, the message is posted to the window 
        /// beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_RBUTTONUP = 0x0205,

        /// <summary> Posted when the user double-clicks the right mouse button while the cursor is 
        /// in the client area of a window. If the mouse is not captured, the message is posted to the window 
        /// beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_RBUTTONDBLCLK = 0x0206,

        /// <summary> Posted when the user presses the middle mouse button 
        ///           while the cursor is in the client area of a window.  </summary>
        WM_MBUTTONDOWN = 0x0207,

        /// <summary> Posted when the user releases the middle mouse button while the cursor is 
        /// in the client area of a window. If the mouse is not captured, the message is posted to the window 
        /// beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_MBUTTONUP = 0x0208,

        /// <summary> Posted when the user double-clicks the middle mouse button while the cursor is 
        /// in the client area of a window. If the mouse is not captured, the message is posted to the window
        /// beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        WM_MBUTTONDBLCLK = 0x0209,

        /// <summary>
        /// The WM_MOUSEWHEEL message is sent to the focus window when the mouse wheel is rotated. The DefWindowProc function propagates the message to the 
        /// window's parent. There should be no internal forwarding of the message, since DefWindowProc propagates it up the parent chain until it finds a 
        /// window that processes it.
        /// </summary>
        WM_MOUSEWHEEL = 0x020A,

        /// <summary>
        /// The WM_XBUTTONDOWN message is posted when the user presses the first or second X button while the cursor is in the client area of a window. If the 
        /// mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the 
        /// mouse. 
        /// </summary>
        WM_XBUTTONDOWN = 0x020B,

        /// <summary>
        /// The WM_XBUTTONUP message is posted when the user releases the first or second X button while the cursor is in the client area of a window. If the 
        /// mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the 
        /// mouse.
        /// </summary>
        WM_XBUTTONUP = 0x020C,

        /// <summary>
        /// The WM_XBUTTONDBLCLK message is posted when the user double-clicks the first or second X button while the cursor is in the client area of a window. 
        /// If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has 
        /// captured the mouse.
        /// </summary>
        WM_XBUTTONDBLCLK = 0x020D,

        /// <summary>
        /// The WM_MOUSEHWHEEL message is sent to the focus window when the mouse's horizontal scroll wheel is tilted or rotated. The DefWindowProc function 
        /// propagates the message to the window's parent. There should be no internal forwarding of the message, since DefWindowProc propagates it up the 
        /// parent chain until it finds a window that processes it.
        /// </summary>
        WM_MOUSEHWHEEL = 0x020E,

        /// <summary>
        /// Use WM_MOUSELAST to specify the last mouse message. Used with PeekMessage() Function.
        /// </summary>
        WM_MOUSELAST = 0x020E,

        /// <summary>
        /// The WM_PARENTNOTIFY message is sent to the parent of a child window when the child window is created or destroyed, or when the user clicks a mouse 
        /// button while the cursor is over the child window. When the child window is being created, the system sends WM_PARENTNOTIFY just before the 
        /// CreateWindow or CreateWindowEx function that creates the window returns. When the child window is being destroyed, the system sends the message 
        /// before any processing to destroy the window takes place.
        /// </summary>
        WM_PARENTNOTIFY = 0x0210,

        /// <summary>
        /// The WM_ENTERMENULOOP message informs an application's main window procedure that a menu modal loop has been entered. 
        /// </summary>
        WM_ENTERMENULOOP = 0x0211,

        /// <summary>
        /// The WM_EXITMENULOOP message informs an application's main window procedure that a menu modal loop has been exited. 
        /// </summary>
        WM_EXITMENULOOP = 0x0212,

        /// <summary>
        /// The WM_NEXTMENU message is sent to an application when the right or left arrow key is used to switch between the menu bar and the system menu. 
        /// </summary>
        WM_NEXTMENU = 0x0213,

        /// <summary>
        /// The WM_SIZING message is sent to a window that the user is resizing. By processing this message, an application can monitor the size and position 
        /// of the drag rectangle and, if needed, change its size or position. 
        /// </summary>
        WM_SIZING = 0x0214,

        /// <summary>
        /// The WM_CAPTURECHANGED message is sent to the window that is losing the mouse capture.
        /// </summary>
        WM_CAPTURECHANGED = 0x0215,

        /// <summary>
        /// The WM_MOVING message is sent to a window that the user is moving. By processing this message, an application can monitor the position of the drag 
        /// rectangle and, if needed, change its position.
        /// </summary>
        WM_MOVING = 0x0216,

        /// <summary>
        /// Notifies applications that a power-management event has occurred.
        /// </summary>
        WM_POWERBROADCAST = 0x0218,

        /// <summary>
        /// Notifies an application of a change to the hardware configuration of a device or the computer.
        /// </summary>
        WM_DEVICECHANGE = 0x0219,

        /// <summary>
        /// An application sends the WM_MDICREATE message to a multiple-document interface (MDI) client window to create an MDI child window. 
        /// </summary>
        WM_MDICREATE = 0x0220,

        /// <summary>
        /// An application sends the WM_MDIDESTROY message to a multiple-document interface (MDI) client window to close an MDI child window. 
        /// </summary>
        WM_MDIDESTROY = 0x0221,

        /// <summary>
        /// An application sends the WM_MDIACTIVATE message to a multiple-document interface (MDI) client window to instruct the client window to activate a 
        /// different MDI child window. 
        /// </summary>
        WM_MDIACTIVATE = 0x0222,

        /// <summary>
        /// An application sends the WM_MDIRESTORE message to a multiple-document interface (MDI) client window to restore an MDI child window from maximized 
        /// or minimized size. 
        /// </summary>
        WM_MDIRESTORE = 0x0223,

        /// <summary>
        /// An application sends the WM_MDINEXT message to a multiple-document interface (MDI) client window to activate the next or previous child window. 
        /// </summary>
        WM_MDINEXT = 0x0224,

        /// <summary>
        /// An application sends the WM_MDIMAXIMIZE message to a multiple-document interface (MDI) client window to maximize an MDI child window. The system 
        /// resizes the child window to make its client area fill the client window. The system places the child window's window menu icon in the rightmost 
        /// position of the frame window's menu bar, and places the child window's restore icon in the leftmost position. The system also appends the title bar 
        /// text of the child window to that of the frame window. 
        /// </summary>
        WM_MDIMAXIMIZE = 0x0225,

        /// <summary>
        /// An application sends the WM_MDITILE message to a multiple-document interface (MDI) client window to arrange all of its MDI child windows in a tile 
        /// format. 
        /// </summary>
        WM_MDITILE = 0x0226,

        /// <summary>
        /// An application sends the WM_MDICASCADE message to a multiple-document interface (MDI) client window to arrange all its child windows in a cascade 
        /// format. 
        /// </summary>
        WM_MDICASCADE = 0x0227,

        /// <summary>
        /// An application sends the WM_MDIICONARRANGE message to a multiple-document interface (MDI) client window to arrange all minimized MDI child windows. 
        /// It does not affect child windows that are not minimized. 
        /// </summary>
        WM_MDIICONARRANGE = 0x0228,

        /// <summary>
        /// An application sends the WM_MDIGETACTIVE message to a multiple-document interface (MDI) client window to retrieve the handle to the active MDI 
        /// child window. 
        /// </summary>
        WM_MDIGETACTIVE = 0x0229,

        /// <summary>
        /// An application sends the WM_MDISETMENU message to a multiple-document interface (MDI) client window to replace the entire menu of an MDI frame 
        /// window, to replace the window menu of the frame window, or both. 
        /// </summary>
        WM_MDISETMENU = 0x0230,

        /// <summary>
        /// The WM_ENTERSIZEMOVE message is sent one time to a window after it enters the moving or sizing modal loop. The window enters the moving or sizing 
        /// modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc 
        /// function and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns. 
        /// The system sends the WM_ENTERSIZEMOVE message regardless of whether the dragging of full windows is enabled.
        /// </summary>
        WM_ENTERSIZEMOVE = 0x0231,

        /// <summary>
        /// The WM_EXITSIZEMOVE message is sent one time to a window, after it has exited the moving or sizing modal loop. The window enters the moving or 
        /// sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the 
        /// DefWindowProc function and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc 
        /// returns. 
        /// </summary>
        WM_EXITSIZEMOVE = 0x0232,

        /// <summary>
        /// Sent when the user drops a file on the window of an application that has registered itself as a recipient of dropped files.
        /// </summary>
        WM_DROPFILES = 0x0233,

        /// <summary>
        /// An application sends the WM_MDIREFRESHMENU message to a multiple-document interface (MDI) client window to refresh the window menu of the MDI frame window.
        /// </summary>
        WM_MDIREFRESHMENU = 0x0234,

        /// <summary> Sent to a window when there is a change in the settings of a monitor that has a digitizer attached to it. 
        /// This message contains information regarding the scaling of the display mode. 
        /// </summary>
        WM_POINTERDEVICECHANGE = 0x238,

        /// <summary> Sent to a window when a pointer device is detected within range of an input digitizer. 
        ///           This message contains information regarding the device and its proximity. </summary>
        WM_POINTERDEVICEINRANGE = 0x239,

        /// <summary> Sent to a window when a pointer device has departed the range of an input digitizer. 
        ///           This message contains information regarding the device and its proximity. </summary>
        WM_POINTERDEVICEOUTOFRANGE = 0x23A,

        /// <summary> Notifies the window when one or more touch points, such as a finger or pen, touches a touch-sensitive digitizer surface. </summary>
        WM_TOUCH = 0x0240,

        /// <summary> Posted to provide an update on a pointer that made contact over the non-client area of a window 
        ///           or when a hovering uncaptured contact moves over the non-client area of a window. </summary>
        WM_NCPOINTERUPDATE = 0x0241,

        /// <summary> Posted when a pointer makes contact over the non-client area of a window. 
        ///           The message targets the window over which the pointer makes contact. 
        ///           The pointer is implicitly captured to the window so that the window continues to receive input for the pointer until it breaks contact. </summary>
        WM_NCPOINTERDOWN = 0x0242,

        /// <summary> Posted when a pointer that made contact over the non-client area of a window breaks contact. 
        ///           The message targets the window over which the pointer makes contact and the pointer is, 
        ///           at that point, implicitly captured to the window so that the window continues to receive input for the pointer 
        ///           until it breaks contact, including the WM_NCPOINTERUP notification.. </summary>
        WM_NCPOINTERUP = 0x0243,

        /// <summary> Posted to provide an update on a pointer that made contact over the client area of a window or on a hovering uncaptured pointer over the client area of a window. </summary>
        WM_POINTERUPDATE = 0x0245,

        /// <summary> Posted when a pointer makes contact over the client area of a window. 
        ///           This input message targets the window over which the pointer makes contact, 
        ///           and the pointer is implicitly captured to the window so that the window continues to receive input for the pointer until it breaks contact. </summary>
        WM_POINTERDOWN = 0x0246,

        /// <summary> Posted when a pointer that made contact over the client area of a window breaks contact. </summary>
        WM_POINTERUP = 0x0247,

        /// <summary> Sent to a window when a new pointer enters detection range over the window (hover) or when an existing pointer moves within the boundaries of the window. </summary>
        WM_POINTERENTER = 0x0249,

        /// <summary> Sent to a window when a pointer leaves detection range over the window (hover) or when a pointer moves outside the boundaries of the window. </summary>
        WM_POINTERLEAVE = 0x024A,

        /// <summary> Sent to an inactive window when a primary pointer generates a WM_POINTERDOWN over the window. </summary>
        WM_POINTERACTIVATE = 0x024B,

        /// <summary> Sent to a window that is losing capture of an input pointer. </summary>
        WM_POINTERCAPTURECHANGED = 0x024C,

        /// <summary> Sent to a window on a touch down in order to determine the most probable touch target. </summary>
        WM_TOUCHHITTESTING = 0x024D,

        /// <summary> Posted to the window with foreground keyboard focus when a scroll wheel is rotated. </summary>
        WM_POINTERWHEEL = 0x024E,

        /// <summary> Posted to the window with foreground keyboard focus when a horizontal scroll wheel is rotated. </summary>
        WM_POINTERHWHEEL = 0x024F,

        /// <summary> Sent to a window, when pointer input is first detected, in order to determine the most probable input target for Direct Manipulation. </summary>
        DM_POINTERHITTEST = 0x0250,

        /// <summary> Sent when ongoing pointer input, for an existing pointer ID, transitions from one process to another across content configured for cross-process chaining (AddContentWithCrossProcessChaining).
        ///           This message is sent to the process not currently receiving pointer input. </summary>
        WM_POINTERROUTEDTO = 0x0251,

        /// <summary> Occurs on the process receiving input when the pointer input is routed to another process. </summary>
        WM_POINTERROUTEDAWAY = 0x0252,

        /// <summary> Sent to all processes (configured for cross-process chaining through AddContentWithCrossProcessChaining and not currently handling pointer input) 
        ///           ever associated with a specific pointer ID, when a WM_POINTERUP message is received on the current process. </summary>
        WM_POINTERROUTEDRELEASED = 0x0253,

        WM_VISIBILITYCHANGED = 0x0270,
        WM_VIEWSTATECHANGED = 0x0271,
        WM_UNREGISTER_WINDOW_SERVICES = 0x0272,
        WM_CONSOLIDATED = 0x0273,

        // See
        // https://github.com/akinomyoga/contra/blob/master/src/twin/win_messages.cpp

        WM_IME_REPORT = 0x0280,

        /// <summary>
        /// Sent to an application when a window is activated. A window receives this message through its WindowProc function. 
        /// </summary>
        WM_IME_SETCONTEXT = 0x0281,

        /// <summary>
        /// Sent to an application to notify it of changes to the IME window. A window receives this message through its WindowProc function. 
        /// </summary>
        WM_IME_NOTIFY = 0x0282,

        /// <summary>
        /// Sent by an application to direct the IME window to carry out the requested command. The application uses this message to control the IME window 
        /// that it has created. To send this message, the application calls the SendMessage function with the following parameters.
        /// </summary>
        WM_IME_CONTROL = 0x0283,

        /// <summary>
        /// Sent to an application when the IME window finds no space to extend the area for the composition window. A window receives this message through its 
        /// WindowProc function. 
        /// </summary>
        WM_IME_COMPOSITIONFULL = 0x0284,

        /// <summary>
        /// Sent to an application when the operating system is about to change the current IME. A window receives this message through its WindowProc 
        /// function. 
        /// </summary>
        WM_IME_SELECT = 0x0285,

        /// <summary>
        /// Sent to an application when the IME gets a character of the conversion result. A window receives this message through its WindowProc function. 
        /// </summary>
        WM_IME_CHAR = 0x0286,

        /// <summary> WM_IME_SYSTEM is an internal undocumented message. It is not intended to be used in users’ applications. . </summary>
        WM_IME_SYSTEM = 0x0287,

        /// <summary>
        /// Sent to an application to provide commands and request information. A window receives this message through its WindowProc function. 
        /// </summary>
        WM_IME_REQUEST = 0x0288,

        /// <summary>
        /// Sent to an application by the IME to notify the application of a key press and to keep message order. A window receives this message through its 
        /// WindowProc function. 
        /// </summary>
        WM_IME_KEYDOWN = 0x0290,

        /// <summary>
        /// Sent to an application by the IME to notify the application of a key release and to keep message order. A window receives this message through its 
        /// WindowProc function. 
        /// </summary>
        WM_IME_KEYUP = 0x0291,

        /// <summary>
        /// The WM_NCMOUSEHOVER message is posted to a window when the cursor hovers over the nonclient area of the window for the period of time specified in 
        /// a prior call to TrackMouseEvent.
        /// </summary>
        WM_NCMOUSEHOVER = 0x02A0,

        /// <summary>
        /// The WM_NCMOUSELEAVE message is posted to a window when the cursor leaves the nonclient area of the window specified in a prior call to 
        /// TrackMouseEvent.
        /// </summary>
        WM_NCMOUSELEAVE = 0x02A2,

        /// <summary>
        /// The WM_MOUSEHOVER message is posted to a window when the cursor hovers over the client area of the window for the period of time specified in a 
        /// prior call to TrackMouseEvent.
        /// </summary>
        WM_MOUSEHOVER = 0x02A1,

        /// <summary> Posted to a window when the cursor leaves the client area of the window specified 
        /// in a prior call to TrackMouseEvent.
        /// </summary>
        WM_MOUSELEAVE = 0x02A3,

        /// <summary>
        /// The WM_WTSSESSION_CHANGE message notifies applications of changes in session state.
        /// </summary>
        WM_WTSSESSION_CHANGE = 0x02B1,
        WM_TABLET_FIRST = 0x02c0,
        WM_TABLET_LAST = 0x02df,

        /// <summary> An application sends a WM_CUT message to an edit control or combo box to delete (cut) 
        /// the current selection, if any, in the edit control and copy the deleted text to the clipboard . </summary>
        WM_CUT = 0x0300,

        /// <summary> An application sends the WM_COPY message to an edit control or combo box to copy 
        /// the current selection to the clipboard. </summary>
        WM_COPY = 0x0301,

        /// <summary> An application sends a WM_PASTE message to an edit control or combo box to copy 
        /// the current content of the clipboard to the edit control at the current caret position. 
        /// Data is inserted only if the clipboard contains data in CF_TEXT format. </summary>
        WM_PASTE = 0x0302,

        /// <summary> An application sends a WM_CLEAR message to an edit control or combo box to delete 
        /// (clear) the current selection, if any, from the edit control. </summary>
        WM_CLEAR = 0x0303,

        /// <summary> An application sends a WM_UNDO message to an edit control to undo the last operation. 
        /// When this message is sent to an edit control, the previously deleted text is restored 
        /// or the previously added text is deleted. </summary>
        WM_UNDO = 0x0304,

        /// <summary> Sent to the clipboard owner if it has delayed rendering a specific clipboard format 
        /// and if an application has requested data in that format. The clipboard owner must render data 
        /// in the specified format and place it on the clipboard by calling the SetClipboardData function. 
        /// </summary>
        WM_RENDERFORMAT = 0x0305,

        /// <summary> Sent to the clipboard owner before it is destroyed, if the clipboard owner has delayed 
        /// rendering one or more clipboard formats. For the content of the clipboard to remain available 
        /// to other applications, the clipboard owner must render data in all the formats it is capable 
        /// of generating, and place the data on the clipboard by calling the SetClipboardData function.
        /// </summary>
        WM_RENDERALLFORMATS = 0x0306,

        /// <summary> 
        /// Sent to the clipboard owner when a call to the EmptyClipboard function empties the clipboard.  
        /// </summary>
        WM_DESTROYCLIPBOARD = 0x0307,

        /// <summary> Sent to the first window in the clipboard viewer chain when the content of the clipboard 
        /// changes. This enables a clipboard viewer window to display the new content of the clipboard. 
        /// </summary>
        WM_DRAWCLIPBOARD = 0x0308,

        /// <summary> Sent to the clipboard owner by a clipboard viewer window when the clipboard contains 
        /// data in the CF_OWNERDISPLAY format and the clipboard viewer's client area needs repainting. </summary>
        WM_PAINTCLIPBOARD = 0x0309,

        /// <summary> Sent to the clipboard owner by a clipboard viewer window when the clipboard contains 
        /// data in the CF_OWNERDISPLAY format and an event occurs in the clipboard viewer's vertical scroll bar. 
        /// The owner should scroll the clipboard image and update the scroll bar values. </summary>
        WM_VSCROLLCLIPBOARD = 0x030A,

        /// <summary> Sent to the clipboard owner by a clipboard viewer window when the clipboard 
        /// contains data in the CF_OWNERDISPLAY format and the clipboard viewer's client area has changed size. </summary>
        WM_SIZECLIPBOARD = 0x030B,

        /// <summary> Sent to the clipboard owner by a clipboard viewer window to request the name of a 
        ///CF_OWNERDISPLAY clipboard format. </summary>
        WM_ASKCBFORMATNAME = 0x030C,

        /// <summary> Sent to the first window in the clipboard viewer chain when a window is being removed 
        /// from the chain.</summary>
        WM_CHANGECBCHAIN = 0x030D,

        /// <summary> Sent to the clipboard owner by a clipboard viewer window. This occurs when the clipboard 
        /// contains data in the CF_OWNERDISPLAY format and an event occurs in the clipboard viewer's 
        /// horizontal scroll bar. The owner should scroll the clipboard image and update the scroll bar values. 
        /// </summary>
        WM_HSCROLLCLIPBOARD = 0x030E,

        /// <summary> The WM_QUERYNEWPALETTE message informs a window that it is about to receive the keyboard 
        /// focus, giving the window the opportunity to realize its logical palette when it receives the focus. 
        /// </summary>
        WM_QUERYNEWPALETTE = 0x030F,

        /// <summary> The WM_PALETTEISCHANGING message informs applications that an application is going to 
        /// realize its logical palette. </summary>
        WM_PALETTEISCHANGING = 0x0310,

        /// <summary> The WM_PALETTECHANGED message is sent to all top-level and overlapped windows after
        ///the window with the keyboard focus has realized its logical palette, thereby changing 
        /// the system palette. This message enables a window that uses a color palette but does not have 
        /// the keyboard focus to realize its logical palette and update its client area. </summary>
        WM_PALETTECHANGED = 0x0311,

        /// <summary> Posted when the user presses a hot key registered by the RegisterHotKey function. 
        /// The message is placed at the top of the message queue associated with the thread 
        /// that registered the hot key. </summary>
        WM_HOTKEY = 0x0312,

        /// <summary> The WM_PRINT message is sent to a window to request that it draw itself in the specified 
        /// device context, most commonly in a printer device context. </summary>
        WM_PRINT = 0x0317,

        /// <summary>
        /// The WM_PRINTCLIENT message is sent to a window to request that it draw its client area 
        /// in the specified device context, most commonly in a printer device context.
        ///
        /// Unlike WM_PRINT, WM_PRINTCLIENT is not processed by DefWindowProc. A window should process 
        /// the WM_PRINTCLIENT message through an application-defined WindowProc function for it 
        /// to be used properly.
        /// </summary>
        WM_PRINTCLIENT = 0x0318,

        /// <summary>
        /// The WM_APPCOMMAND message notifies a window that the user generated an application command event, for example, by clicking an application command 
        /// button using the mouse or typing an application command key on the keyboard.
        /// </summary>
        WM_APPCOMMAND = 0x0319,

        /// <summary>
        /// The WM_THEMECHANGED message is broadcast to every window following a theme change event. Examples of theme change events are the activation of a 
        /// theme, the deactivation of a theme, or a transition from one theme to another.
        /// </summary>
        WM_THEMECHANGED = 0x031A,

        /// <summary>
        /// Sent when the contents of the clipboard have changed.
        /// </summary>
        WM_CLIPBOARDUPDATE = 0x031D,

        /// <summary>
        /// The system will send a window the WM_DWMCOMPOSITIONCHANGED message to indicate that the availability of desktop composition has changed.
        /// </summary>
        WM_DWMCOMPOSITIONCHANGED = 0x031E,

        /// <summary>
        /// WM_DWMNCRENDERINGCHANGED is called when the non-client area rendering status of a window has changed. Only windows that have set the flag 
        /// DWM_BLURBEHIND.fTransitionOnMaximized to true will get this message. 
        /// </summary>
        WM_DWMNCRENDERINGCHANGED = 0x031F,

        /// <summary>
        /// Sent to all top-level windows when the colorization color has changed. 
        /// </summary>
        WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320,

        /// <summary>
        /// WM_DWMWINDOWMAXIMIZEDCHANGE will let you know when a DWM composed window is maximized. You also have to register for this message as well. You'd 
        /// have other windowd go opaque when this message is sent.
        /// </summary>
        WM_DWMWINDOWMAXIMIZEDCHANGE = 0x0321,

        /// <summary>
        /// Sent to request extended title bar information. A window receives this message through its WindowProc function.
        /// </summary>
        WM_GETTITLEBARINFOEX = 0x033F,
        WM_HANDHELDFIRST = 0x0358,
        WM_HANDHELDLAST = 0x035F,
        WM_AFXFIRST = 0x0360,
        WM_AFXLAST = 0x037F,
        WM_PENWINFIRST = 0x0380,
        WM_PENWINLAST = 0x038F,

        /// <summary> Used to define private messages for use by private window classes, 
        /// usually of the form WM_USER+x, where x is an integer value. </summary>
        WM_USER = 0x0400,

        /// <summary> Retrieves the identifier of the default push button control for a dialog box. </summary>
        DM_GETDEFID = (WM_USER + 0),

        /// <summary> Changes the identifier of the default push button for a dialog box. </summary>
        DM_SETDEFID = (WM_USER + 1),

        /// <summary> Repositions a top-level dialog box so that it fits within the desktop area. 
        /// An application can send this message to a dialog box after resizing it to ensure 
        /// that the entire dialog box remains visible. </summary>
        DM_REPOSITION = (WM_USER + 2),

        /// <summary>
        /// The WM_APP constant is used by applications to help define private messages, usually of the form WM_APP+X, where X is an integer value. 
        /// </summary>
        WM_APP = 0x8000,
    }

    /// <summary>
    /// Edit control message IDs.
    /// </summary>
    /// <remarks>
    /// Based on EM_* constants defined in winuser.h in the Windows API.
    /// The declared type of the enum is int, because the WM enumeration is int also.
    /// </remarks>  
    public enum EM : int
    {
        /// <summary>
        /// Gets the starting and ending character positions (in TCHARs) of the current selection in an edit control. 
        /// You can send this message to either an edit control or a rich edit control. 
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// A pointer to a DWORD value that receives the starting position of the selection. This parameter can be NULL.
        /// <br/>
        /// lParam <br/>
        /// A pointer to a DWORD value that receives the position of the first unselected character after the end of the selection. This parameter can be NULL.
        /// </summary>
        EM_GETSEL = 0x00B0,

        /// <summary> Selects a range of characters in an edit control. 
        /// You can send this message to either an edit control or a rich edit control. 
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// The starting character position of the selection.
        /// <br/>
        /// lParam <br/>
        /// The ending character position of the selection.
        /// </summary>
        EM_SETSEL = 0x00B1,

        /// <summary> Gets the formatting rectangle of an edit control. 
        /// The formatting rectangle is the limiting rectangle into which the control draws the text. 
        /// The limiting rectangle is independent of the size of the edit-control window. 
        /// You can send this message to either an edit control or a rich edit control.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used.
        /// <br/>
        /// lParam <br/>
        /// A pointer to a RECT structure that receives the formatting rectangle.
        /// </summary>
        EM_GETRECT = 0x00B2,

        /// <summary> Sets the formatting rectangle of a multiline edit control. 
        /// The formatting rectangle is the limiting rectangle into which the control draws the text. 
        /// The limiting rectangle is independent of the size of the edit control window.<br/>
        /// This message is processed only by multiline edit controls. 
        /// You can send this message to either an edit control or a rich edit control.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// Indicates whether lParam specifies absolute or relative coordinates. 
        /// A value of zero indicates absolute coordinates. 
        /// A value of 1 indicates offsets relative to the current formatting rectangle. 
        /// (The offsets can be positive or negative.)
        /// <br/>
        /// lParam <br/>
        /// A pointer to a RECT structure that specifies the new dimensions of the rectangle. 
        /// If this parameter is NULL, the formatting rectangle is set to its default values.
        /// </summary>
        EM_SETRECT = 0x00B3,

        /// <summary> Sets the formatting rectangle of a multiline edit control. 
        /// The EM_SETRECTNP message is identical to the EM_SETRECT message, 
        /// except that EM_SETRECTNP does not redraw the edit control window.
        ///
        /// The formatting rectangle is the limiting rectangle into which the control draws the text. 
        /// The limiting rectangle is independent of the size of the edit control window.
        /// This message is processed only by multiline edit controls. 
        /// You can send this message to either an edit control or a rich edit control.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// Indicates whether lParam specifies absolute or relative coordinates. 
        /// A value of zero indicates absolute coordinates. 
        /// A value of 1 indicates offsets relative to the current formatting rectangle. 
        /// (The offsets can be positive or negative.)
        /// <br/>
        /// lParam <br/>
        /// A pointer to a RECT structure that specifies the new dimensions of the rectangle. 
        /// If this parameter is NULL, the formatting rectangle is set to its default values.
        /// </summary>
        EM_SETRECTNP = 0x00B4,

        /// <summary> Scrolls the text vertically in a multiline edit control. 
        /// This message is equivalent to sending a WM_VSCROLL message to the edit control. 
        /// You can send this message to either an edit control or a rich edit control.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// The action the scroll bar is to take. This parameter can be one of the following values.
        /// SB_LINEDOWN, SB_LINEUP, SB_PAGEDOWN, SB_PAGEUP
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_SCROLL = 0x00B5,

        /// <summary> Scrolls the text in a multiline edit control.
        /// You can send this message to either an edit control or a rich edit control.
        ///           
        /// Arguments: <br/>
        /// wParam <br/>
        /// Edit controls: The number of characters to scroll horizontally.
        /// Rich edit controls: This parameter is not used; it must be zero.
        /// <br/>
        /// lParam <br/>
        /// The number of lines to scroll vertically.
        /// </summary>
        EM_LINESCROLL = 0x00B6,

        /// <summary> Scrolls the caret into view in an edit control. 
        /// You can send this message to either an edit control or a rich edit control.
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is reserved. It should be set to zero.
        /// <br/>
        /// lParam <br/>
        /// This parameter is reserved. It should be set to zero.
        /// </summary>
        EM_SCROLLCARET = 0x00B7,

        /// <summary> Gets the state of an edit control's modification flag. 
        /// The flag indicates whether the contents of the edit control have been modified. 
        /// You can send this message to either an edit control or a rich edit control.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used. It must be set to zero.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used. It must be set to zero.
        /// </summary>
        EM_GETMODIFY = 0x00B8,

        /// <summary> Sets or clears the modification flag for an edit control. 
        /// The modification flag indicates whether the text within the edit control has been modified. 
        /// You can send this message to either an edit control or a rich edit control.
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// The new value for the modification flag. A value of TRUE indicates the text has been modified, 
        /// and a value of FALSE indicates it has not been modified.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_SETMODIFY = 0x00B9,

        /// <summary> Gets the number of lines in a multiline edit control. 
        /// You can send this message to either an edit control or a rich edit control.
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_GETLINECOUNT = 0x00BA,

        /// <summary> Gets the character index of the first character of a specified line in a multiline 
        /// edit control. A character index is the zero-based index of the character from the beginning 
        /// of the edit control. You can send this message to either an edit control or a rich edit control.
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// The zero-based line number. A value of –1 specifies the current line number (the line that contains the caret). <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_LINEINDEX = 0x00BB,

        /// <summary> Sets the handle of the memory that will be used by a multiline edit control.
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// A handle to the memory buffer the edit control uses to store the currently displayed text 
        /// instead of allocating its own memory. If necessary, the control reallocates this memory.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_SETHANDLE = 0x00BC,

        /// <summary> Gets a handle of the memory currently allocated for a multiline edit control's text.
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_GETHANDLE = 0x00BD,

        /// <summary> Gets the position of the scroll box (thumb) in the vertical scroll bar of a multiline 
        /// edit control. You can send this message to either an edit control or a rich edit control.
        /// </summary>
        EM_GETTHUMB = 0x00BE,

        /// <summary> Retrieves the length, in characters, of a line in an edit control. 
        ///  You can send this message to either an edit control or a rich edit control.
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// The character index of a character in the line whose length is to be retrieved. 
        /// If this parameter is greater than the number of characters in the control, the return value is zero.
        ///
        /// This parameter can be –1. In this case, the message returns the number of unselected characters 
        /// on lines containing selected characters. For example, if the selection extended from the fourth 
        /// character of one line through the eighth character from the end of the next line, 
        /// the return value would be 10 (three characters on the first line and seven on the next).
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_LINELENGTH = 0x00C1,

        /// <summary> Replaces the selected text in an edit control or a rich edit control with the specified text.
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// Specifies whether the replacement operation can be undone. 
        /// If this is TRUE, the operation can be undone. If this is FALSE , the operation cannot be undone.
        /// <br/>
        /// lParam <br/>
        /// A pointer to a null-terminated string containing the replacement text.
        /// </summary>
        EM_REPLACESEL = 0x00C2,

        /// <summary> Copies a line of text from an edit control and places it in a specified buffer. 
        ///  You can send this message to either an edit control or a rich edit control.
        ///  
        /// Arguments: <br/>
        /// wParam <br/>
        /// The zero-based index of the line to retrieve from a multiline edit control. 
        /// A value of zero specifies the topmost line. This parameter is ignored by a single-line edit control.
        /// <br/>
        /// lParam <br/>
        /// A pointer to the buffer that receives a copy of the line. 
        /// Before sending the message, set the first word of this buffer to the size, in TCHARs, of the buffer. 
        /// For ANSI text, this is the number of bytes; for Unicode text, this is the number of characters. 
        /// The size in the first word is overwritten by the copied line.
        /// </summary>
        EM_GETLINE = 0x00C4,

        /// <summary> Sets the text limit of an edit control. The text limit is the maximum amount of text, 
        /// in TCHARs, that the user can type into the edit control. 
        /// You can send this message to either an edit control or a rich edit control. 
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// The maximum number of TCHARs the user can enter. For ANSI text, this is the number of bytes; 
        /// for Unicode text, this is the number of characters. 
        /// This number does not include the terminating null character.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_LIMITTEXT = 0x00C5,

        /// <summary> Determines whether there are any actions in an edit control's undo queue. 
        ///  You can send this message to either an edit control or a rich edit control.
        ///  
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_CANUNDO = 0x00C6,

        /// <summary> This message undoes the last edit control operation in the control's undo queue. 
        ///  You can send this message to either an edit control or a rich edit control.
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// Not used; must be zero.
        /// <br/>
        /// lParam <br/>
        /// Not used; must be zero.
        /// </summary>
        EM_UNDO = 0x00C7,

        /// <summary> Sets a flag that determines whether a multiline edit control includes soft line-break 
        /// characters. A soft line break consists of two carriage returns and a line feed and is inserted 
        /// at the end of a line that is broken because of word-wrapping.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// Not used; must be zero.
        /// <br/>
        /// lParam <br/>
        /// Not used; must be zero.
        /// </summary>
        EM_FMTLINES = 0x00C8,

        /// <summary> Gets the index of the line that contains the specified character index 
        /// in a multiline edit control. A character index is the zero-based index of the character 
        /// from the beginning of the edit control. 
        /// You can send this message to either an edit control or a rich edit control.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// The character index of the character contained in the line whose number is to be retrieved. 
        /// If this parameter is –1, EM_LINEFROMCHAR retrieves either the line number of the current line 
        /// (the line containing the caret) or, if there is a selection, the line number of the line 
        /// containing the beginning of the selection.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_LINEFROMCHAR = 0x00C9,

        /// <summary> The EM_SETTABSTOPS message sets the tab stops in a multiline edit control. 
        /// When text is copied to the control, any tab character in the text causes space to be generated 
        /// up to the next tab stop.
        ///
        /// This message is processed only by multiline edit controls. 
        /// You can send this message to either an edit control or a rich edit control.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// The number of tab stops contained in the array. If this parameter is zero, 
        /// the lParam parameter is ignored and default tab stops are set at every 32 dialog template units. 
        /// If this parameter is 1, tab stops are set at every n dialog template units, 
        /// where n is the distance pointed to by the lParam parameter. 
        /// If this parameter is greater than 1, lParam is a pointer to an array of tab stops.
        /// <br/>
        /// lParam <br/>
        /// A pointer to an array of unsigned integers specifying the tab stops, in dialog template units. 
        /// If the wParam parameter is 1, this parameter is a pointer to an unsigned integer 
        /// containing the distance between all tab stops, in dialog template units.
        /// </summary>
        EM_SETTABSTOPS = 0x00CB,

        /// <summary> Sets or removes the password character for an edit control. 
        /// When a password character is set, that character is displayed in place of the characters 
        /// typed by the user. 
        /// You can send this message to either an edit control or a rich edit control.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// Not used; must be zero.
        /// <br/>
        /// lParam <br/>
        /// Not used; must be zero.
        /// </summary>
        EM_SETPASSWORDCHAR = 0x00CC,

        /// <summary> Resets the undo flag of an edit control. 
        /// The undo flag is set whenever an operation within the edit control can be undone. 
        /// You can send this message to either an edit control or a rich edit control.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// The character to be displayed in place of the characters typed by the user. 
        /// If this parameter is zero, the control removes the current password character 
        /// and displays the characters typed by the user.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_EMPTYUNDOBUFFER = 0x00CD,

        /// <summary> 
        ///  Gets the zero-based index of the uppermost visible line in a multi-line edit control. 
        ///  You can send this message to either an edit control or a rich edit control.
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_GETFIRSTVISIBLELINE = 0x00CE,

        /// <summary> 
        ///  Sets or removes the read-only style (ES_READONLY) of an edit control. 
        ///  You can send this message to either an edit control or a rich edit control.
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// Specifies whether to set or remove the ES_READONLY style. 
        /// A value of TRUE sets the ES_READONLY style; a value of FALSE removes the ES_READONLY style.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_SETREADONLY = 0x00CF,

        /// <summary> 
        /// Replaces an edit control's default Wordwrap function with an application-defined function.
        /// You can send this message to either an edit control or a rich edit control.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used.
        /// <br/>
        /// lParam <br/>
        /// The address of the application-defined Wordwrap function. 
        /// For more information about breaking lines, see the description of the EditWordBreakProc 
        /// callback function.
        /// </summary>
        EM_SETWORDBREAKPROC = 0x00D0,

        /// <summary> Gets the address of the current Wordwrap function. 
        ///  You can send this message to either an edit control or a rich edit control.
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_GETWORDBREAKPROC = 0x00D1,

        /// <summary> Gets the password character that an edit control displays when the user enters text. 
        /// You can send this message to either an edit control or a rich edit control.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_GETPASSWORDCHAR = 0x00D2,

        /// <summary> Sets the widths of the left and right margins for an edit control. 
        /// The message redraws the control to reflect the new margins. 
        /// You can send this message to either an edit control or a rich edit control.
        ///  
        /// Arguments: <br/>
        /// wParam <br/>
        /// The margins to set. This parameter can be one or more of the following values:<br/>
        /// <list type="bullet">
        /// <item><b>EC_LEFTMARGIN</b><br/>Sets the left margin.</item>
        /// <item><b>EC_RIGHTMARGIN</b><br/>Sets the right margin.</item>
        /// <item><b>EC_USEFONTINFO</b><br/>
        ///  Rich edit controls:<br/>
        ///  Sets the left and right margins to a narrow width calculated using the text metrics 
        ///  of the control's current font. If no font has been set for the control, 
        ///  the margins are set to zero. The lParam parameter is ignored.<br/>
        ///  Edit controls:<br/>
        ///  The EC_USEFONTINFO value cannot be used in the wParam parameter. 
        ///  It can only be used in the lParam parameter.
        /// </item>
        /// </list>
        /// <br/>
        /// lParam <br/>
        /// The LOWORD specifies the new width of the left margin, in pixels. 
        /// This value is ignored if wParam does not include EC_LEFTMARGIN.<br/>
        /// The HIWORD can specify the EC_USEFONTINFO value to set the right margin 
        /// to a narrow width calculated using the text metrics of the control's current font. 
        /// If no font has been set for the control, the margin is set to zero.
        /// </summary>
        EM_SETMARGINS = 0x00D3,

        /// <summary> Gets the widths of the left and right margins for an edit control.
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used, must be zero.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used, must be zero.
        /// </summary>
        EM_GETMARGINS = 0x00D4,

        /// <summary> The EM_SETLIMITTEXT message is identical to the <see cref="EM_LIMITTEXT"/>message.
        /// </summary>
        EM_SETLIMITTEXT = EM_LIMITTEXT,

        /// <summary> Gets the current text limit for an edit control. 
        /// You can send this message to either an edit control or a rich edit control.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used, must be zero.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used, must be zero.
        /// </summary>
        EM_GETLIMITTEXT = 0x00D5,

        /// <summary> Retrieves the client area coordinates of a specified character in an edit control. 
        ///  You can send this message to either an edit control or a rich edit control.
        ///  
        /// Arguments: <br/>
        /// wParam <br/>
        /// The zero-based index of the character.
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_POSFROMCHAR = 0x00D6,

        /// <summary> Gets information about the character closest to a specified point in the client area 
        /// of an edit control. 
        /// You can send this message to either an edit control or a rich edit control.
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used
        /// <br/>
        /// lParam <br/>
        /// The coordinates of a point in the control's client area. The coordinates are in screen units
        /// and are relative to the upper-left corner of the control's client area. <br/>
        /// <br/>
        /// Rich edit controls: <br/>
        /// A pointer to a POINTL structure that contains the horizontal and vertical coordinates.<br/>
        /// Edit controls: <br/>
        /// The LOWORD contains the horizontal coordinate. The HIWORD contains the vertical coordinate.
        /// </summary>
        EM_CHARFROMPOS = 0x00D7,

        /// <summary> Sets the status flags that determine how an edit control interacts 
        ///  with the Input Method Editor (IME).
        ///  For more info see
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb761645(v=vs.85).aspx">
        /// EM_SETIMESTATUS message details </see>
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// The type of status to set. 
        /// <br/>
        /// lParam <br/>
        /// Data specific to the status type.
        /// </summary>
        EM_SETIMESTATUS = 0x00D8,

        /// <summary> Gets a set of status flags that indicate how the edit control interacts 
        /// with the Input Method Editor (IME).
        ///  For more info see
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb761580(v=vs.85).aspx">
        /// EM_GETIMESTATUS message details </see>
        /// 
        /// Arguments: <br/>
        /// wParam <br/>
        /// The type of status to retrieve. 
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used.
        /// </summary>
        EM_GETIMESTATUS = 0x00D9,
    }

    /// <summary>
    /// RichEdit control message IDs.
    /// </summary>
    /// <remarks>
    /// Based on EM_* constants defined in Richedit.h.
    /// </remarks>  
    public enum RichEm : int
    {
        /// <summary> Determines whether a rich edit control can paste a specified clipboard format.<br/>
        ///           
        /// Arguments: <br/>
        /// wParam <br/>
        /// Specifies the Clipboard Formats to try. To try any format currently on the clipboard, 
        /// set this parameter to zero.<br/>
        /// lParam <br/>
        /// This parameter is not used; it must be zero.<br/>
        /// </summary>
        EM_CANPASTE = (WM.WM_USER + 50),

        /// <summary> Displays a portion of the contents of a rich edit control, as previously formatted 
        /// for a device using the EM_FORMATRANGE message.<br/>
        ///           
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used; it must be zero.<br/>
        /// lParam <br/>
        /// A RECT structure specifying the display area of the device.<br/>
        /// </summary>
        EM_DISPLAYBAND = (WM.WM_USER + 51),

        /// <summary> Retrieves the starting and ending character positions of the selection 
        /// in a rich edit control.<br/>
        ///           
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used; it must be zero.<br/>
        /// lParam <br/>
        /// A CHARRANGE structure that receives the selection range.<br/>
        /// </summary>
        EM_EXGETSEL = (WM.WM_USER + 52),

        /// <summary> Sets an upper limit to the amount of text the user can type or paste 
        /// into a rich edit control.<br/>
        ///           
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used; it must be zero.<br/>
        /// lParam <br/>
        /// Specifies the maximum amount of text that can be entered. If this parameter is zero, the default 
        /// maximum is used, which is 64K characters. A COM object counts as a single character.<br/>
        /// </summary>
        EM_EXLIMITTEXT = (WM.WM_USER + 53),

        /// <summary> Determines which line contains the specified character in a rich edit control.<br/>
        ///           
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used; it must be zero.<br/>
        /// lParam <br/>
        /// Zero-based index of the character.<br/>
        /// </summary>
        EM_EXLINEFROMCHAR = (WM.WM_USER + 54),

        /// <summary> Selects a range of characters or Component Object Model (COM) objects 
        /// in a Microsoft Rich Edit control.<br/>
        ///           
        /// Arguments: <br/>
        /// wParam <br/>
        /// This parameter is not used; it must be zero.<br/>
        /// lParam <br/>
        /// A CHARRANGE structure that specifies the selection range.<br/>
        /// </summary>
        EM_EXSETSEL = (WM.WM_USER + 55),

        /// <summary> Finds text within a rich edit control.<br/>
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// Specify the parameters of the search operation.<br/>
        /// lParam <br/>
        /// A reference to FINDTEXT structure containing information about the find operation.<br/>
        /// For more information, see for instance
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb788009(v=vs.85).aspx">
        /// MSDN about EM_FINDTEXT</see>.<br/>
        /// </summary>
        EM_FINDTEXT = (WM.WM_USER + 56),

        /// <summary> Formats a range of text in a rich edit control for a specific device.<br/>
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// Specifies whether to render the text. If this parameter is not zero, the text is rendered. 
        /// Otherwise, the text is just measured.<br/>
        /// lParam <br/>
        /// A FORMATRANGE structure containing information about the output device, 
        /// or NULL to free information cached by the control.<br/>
        /// </summary>
        EM_FORMATRANGE = (WM.WM_USER + 57),

        /// <summary> Determines the character formatting in a rich edit control.<br/>
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// Specifies the range of text from which to retrieve formatting. 
        /// It can be one of the following values.
        /// <list type="bullet">
        /// <item><b>SCF_DEFAULT</b><br/>The default character formatting.</item>
        /// <item><b>SCF_SELECTION</b><br/>The current selection's character formatting.</item>
        /// </list>
        /// <br/>
        /// lParam <br/>
        /// A CHARFORMAT structure that receives the attributes of the first character.
        /// <br/>
        /// </summary>
        EM_GETCHARFORMAT = (WM.WM_USER + 58),

        EM_GETEVENTMASK = (WM.WM_USER + 59),
        EM_GETOLEINTERFACE = (WM.WM_USER + 60),
        EM_GETPARAFORMAT = (WM.WM_USER + 61),
        EM_GETSELTEXT = (WM.WM_USER + 62),
        EM_HIDESELECTION = (WM.WM_USER + 63),
        EM_PASTESPECIAL = (WM.WM_USER + 64),
        EM_REQUESTRESIZE = (WM.WM_USER + 65),
        EM_SELECTIONTYPE = (WM.WM_USER + 66),
        EM_SETBKGNDCOLOR = (WM.WM_USER + 67),
        EM_SETCHARFORMAT = (WM.WM_USER + 68),
        EM_SETEVENTMASK = (WM.WM_USER + 69),
        EM_SETOLECALLBACK = (WM.WM_USER + 70),
        EM_SETPARAFORMAT = (WM.WM_USER + 71),
        EM_SETTARGETDEVICE = (WM.WM_USER + 72),
        EM_STREAMIN = (WM.WM_USER + 73),
        EM_STREAMOUT = (WM.WM_USER + 74),
        EM_GETTEXTRANGE = (WM.WM_USER + 75),
        EM_FINDWORDBREAK = (WM.WM_USER + 76),
        EM_SETOPTIONS = (WM.WM_USER + 77),
        EM_GETOPTIONS = (WM.WM_USER + 78),
        EM_FINDTEXTEX = (WM.WM_USER + 79),
        EM_GETWORDBREAKPROCEX = (WM.WM_USER + 80),
        EM_SETWORDBREAKPROCEX = (WM.WM_USER + 81),

        // RichEdit 2.0 messages 
        EM_SETUNDOLIMIT = (WM.WM_USER + 82),
        EM_REDO = (WM.WM_USER + 84),
        EM_CANREDO = (WM.WM_USER + 85),
        EM_GETUNDONAME = (WM.WM_USER + 86),
        EM_GETREDONAME = (WM.WM_USER + 87),
        EM_STOPGROUPTYPING = (WM.WM_USER + 88),

        EM_SETTEXTMODE = (WM.WM_USER + 89),
        EM_GETTEXTMODE = (WM.WM_USER + 90),

        // Outline mode message
        EM_OUTLINE = (WM.WM_USER + 220),
        // Message for getting and restoring scroll pos
        EM_GETSCROLLPOS = (WM.WM_USER + 221),
        EM_SETSCROLLPOS = (WM.WM_USER + 222),

        /// <summary> Sets the font size for the selected text in a rich edit control.<br/>
        ///
        /// Arguments: <br/>
        /// wParam <br/>
        /// Change in point size of the selected text. 
        /// The result will be rounded according to values shown in the table in MSDN 
        /// documentation.
        /// This parameter should be in the range of –1637 to 1638. 
        /// The resulting font size will be within the range of 1 to 1638.
        /// It can be one of the following values.
        /// <list type="bullet">
        /// <item><b>SCF_DEFAULT</b><br/>The default character formatting.</item>
        /// <item><b>SCF_SELECTION</b><br/>The current selection's character formatting.</item>
        /// </list>
        /// <br/>
        /// lParam <br/>
        /// This parameter is not used; it must be zero.<br/>
        /// </summary>
        EM_SETFONTSIZE = (WM.WM_USER + 223),

        EM_GETZOOM = (WM.WM_USER + 224),
        EM_SETZOOM = (WM.WM_USER + 225),
        EM_GETVIEWKIND = (WM.WM_USER + 226),
        EM_SETVIEWKIND = (WM.WM_USER + 227),

        // RichEdit 4.0 messages
        EM_GETPAGE = (WM.WM_USER + 228),
        EM_SETPAGE = (WM.WM_USER + 229),
        EM_GETHYPHENATEINFO = (WM.WM_USER + 230),
        EM_SETHYPHENATEINFO = (WM.WM_USER + 231),
        EM_GETPAGEROTATE = (WM.WM_USER + 235),
        EM_SETPAGEROTATE = (WM.WM_USER + 236),
        EM_GETCTFMODEBIAS = (WM.WM_USER + 237),
        EM_SETCTFMODEBIAS = (WM.WM_USER + 238),
        EM_GETCTFOPENSTATUS = (WM.WM_USER + 240),
        EM_SETCTFOPENSTATUS = (WM.WM_USER + 241),
        EM_GETIMECOMPTEXT = (WM.WM_USER + 242),
        EM_ISIME = (WM.WM_USER + 243),
        EM_GETIMEPROPERTY = (WM.WM_USER + 244),

        // These messages control what rich edit does when it comes accross
        // OLE objects during RTF stream in.  Normally rich edit queries the client
        // application only after OleLoad has been called.  With these messages it is possible to
        // set the rich edit control to a mode where it will query the client application before
        // OleLoad is called
        EM_GETQUERYRTFOBJ = (WM.WM_USER + 269),
        EM_SETQUERYRTFOBJ = (WM.WM_USER + 270),
    }

    /// <summary>
    /// Mouse Position Codes, used as possible return values for
    /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms645618(v=vs.85).aspx">
    /// WM_NCHITTEST</see> window message, and for 
    /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms644968(v=vs.85).aspx">
    /// MOUSEHOOKSTRUCT.wHitTestCode</see> structure field.
    /// </summary>
    public enum MousePositionCode
    {
        /// <summary> An enum constant representing the position on the screen background or on a dividing line 
        /// between windows (same as HTNOWHERE, except that the DefWindowProc function produces a system beep 
        /// to indicate an error). </summary>
        HTERROR = (-2),

        /// <summary> An enum constant representing the position in a window currently covered by another window 
        /// in the same thread (the message will be sent to underlying windows in the same thread until 
        /// one of them returns a code that is not HTTRANSPARENT).</summary>
        HTTRANSPARENT = (-1),

        /// <summary> An enum constant representing the position on the screen background or on a dividing line 
        /// between windows. </summary>
        HTNOWHERE = 0,

        /// <summary> An enum constant representing the position in a client area. </summary>
        HTCLIENT = 1,

        /// <summary> An enum constant representing the position in a title bar. </summary>
        HTCAPTION = 2,

        /// <summary> An enum constant representing the position in a window menu or in a Close button in a child window.
        /// </summary>
        HTSYSMENU = 3,

        /// <summary> An enum constant representing the position in a size box (same as HTSIZE). </summary>
        HTGROWBOX = 4,

        /// <summary> An enum constant representing the position in a size box (same as HTGROWBOX). </summary>
        HTSIZE = HTGROWBOX,

        /// <summary> An enum constant representing the position in a menu. </summary>
        HTMENU = 5,

        /// <summary> An enum constant representing the position in a horizontal scroll bar. </summary>
        HTHSCROLL = 6,

        /// <summary> An enum constant representing the position in a vertical scroll bar. </summary>
        HTVSCROLL = 7,

        /// <summary> An enum constant representing the position in the Minimize button. </summary>
        HTMINBUTTON = 8,

        /// <summary> An enum constant representing the position in the Maximize button. </summary>
        HTMAXBUTTON = 9,

        /// <summary> An enum constant representing the position in the left border of a resizable window 
        /// (the user can click the mouse to resize the window horizontally. </summary>
        HTLEFT = 10,

        /// <summary> An enum constant representing the position in the right border of a resizable window 
        /// (the user can click the mouse to resize the window horizontally. </summary>
        HTRIGHT = 11,

        /// <summary> An enum constant representing the position in the upper-horizontal border of a window.
        /// </summary>
        HTTOP = 12,

        /// <summary> An enum constant representing the position in the upper-left corner of a window border.
        /// </summary>
        HTTOPLEFT = 13,

        /// <summary> An enum constant representing the position in the upper-right corner of a window border.
        /// </summary>
        HTTOPRIGHT = 14,

        /// <summary> An enum constant representing the position in the lower-horizontal border of a resizable 
        /// window (the user can click the mouse to resize the window vertically).  </summary>
        HTBOTTOM = 15,

        /// <summary> An enum constant representing the position in the lower-left corner of a border 
        /// of a resizable window (the user can click the mouse to resize the window diagonally).
        /// </summary>
        HTBOTTOMLEFT = 16,

        /// <summary> An enum constant representing the position in the lower-right corner of a border 
        /// of a resizable window (the user can click the mouse to resize the window diagonally).
        /// </summary>
        HTBOTTOMRIGHT = 17,

        /// <summary> An enum constant representing the position in the border of a window 
        /// that does not have a sizing border. </summary>
        HTBORDER = 18,

        /// <summary> An enum constant representing the position in the Minimize button (same as HTMINBUTTON).
        /// </summary>
        HTREDUCE = HTMINBUTTON,

        /// <summary> An enum constant representing the position in the Maximize button (same as HTMAXBUTTON).
        /// </summary>
        HTZOOM = HTMAXBUTTON,

        /// <summary> An enum constant representing not-implemented value. For more info see
        /// <see href="http://blogs.msdn.com/b/oldnewthing/archive/2012/07/11/10328580.aspx">
        /// What does the HTOBJECT hit-test code do?</see> </summary>
        HTOBJECT = 19,

        /// <summary> An enum constant representing the position in the Close button.</summary>
        HTCLOSE = 20,

        /// <summary> An enum constant representing the position in the Help button.</summary>
        HTHELP = 21,

        /// <summary> An enum constant of the same value as HTLEFT, but representing a minimal value 
        /// for resizing codes. <br/>
        /// Code example:
        /// <code>
        /// <![CDATA[
        /// if (nHitTest >= HTSIZEFIRST && nHitTest <= HTSIZELAST) { ... }
        /// ]]>
        /// </code>
        /// </summary>
        HTSIZEFIRST = HTLEFT,

        /// <summary> An enum constant of the same value as HTBOTTOMRIGHT, but representing a maximal value
        /// for resizing codes. <br/>
        /// Code example:
        /// <code>
        /// <![CDATA[
        /// if (nHitTest >= HTSIZEFIRST && nHitTest <= HTSIZELAST) { ... }
        /// ]]>
        /// </code>
        /// </summary>
        HTSIZELAST = HTBOTTOMRIGHT,
    };

    /// <summary>
    /// Virtual-Key Codes, used by the system. For more information, see
    /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx">
    /// Virtual-Key Codes</see> in MSDN.
    /// </summary>
    public enum VK : int
    {
        /// <summary> Virtual key for left mouse button. </summary>
        VK_LBUTTON = 0x01,
        /// <summary> Virtual key for right mouse button. </summary>
        VK_RBUTTON = 0x02,
        /// <summary> Virtual key for Control-break processing. </summary>
        VK_CANCEL = 0x03,
        /// <summary> Virtual key for Middle mouse button on a three-button mouse. </summary>
        VK_MBUTTON = 0x04,

        /// <summary> Virtual key for BACKSPACE key. </summary>
        VK_BACK = 0x08,
        /// <summary> Virtual key for Tab key. </summary>
        VK_TAB = 0x09,

        /// <summary> Virtual key for CLEAR key. </summary>
        VK_CLEAR = 0x0C,
        /// <summary> Virtual key for Enter key. </summary>
        VK_RETURN = 0x0D,

        /// <summary> Virtual key for SHIFT key. </summary>
        VK_SHIFT = 0x10,
        /// <summary> Virtual key for CTRL key. </summary>
        VK_CONTROL = 0x11,
        /// <summary> Virtual key for ALT key. </summary>
        VK_MENU = 0x12,
        /// <summary> Virtual key for PAUSE key. </summary>
        VK_PAUSE = 0x13,
        /// <summary> Virtual key for CAPS LOCK key. </summary>
        VK_CAPITAL = 0x14,

        /// <summary> Virtual key for IME Kana mode. </summary>
        VK_KANA = 0x15,

        /// <summary> Virtual key for IME Hanguel mode (maintained for compatibility; use VK_HANGUL). </summary>
        VK_HANGEUL = 0x15,

        /// <summary> Virtual key for IME Hangul mode. </summary>/// 
        VK_HANGUL = 0x15,

        /// <summary> Virtual key for IME Junja mode. </summary>
        VK_JUNJA = 0x17,

        /// <summary> Virtual key for IME final mode. </summary>
        VK_FINAL = 0x18,

        /// <summary> Virtual key for IME Kanji mode. </summary>
        VK_HANJA = 0x19,

        /// <summary> Virtual key for IME Hanja mode. </summary>
        VK_KANJI = 0x19,

        /// <summary> Virtual key for Esc key. </summary>
        VK_ESCAPE = 0x1B,

        /// <summary> Virtual key for IME convert. </summary>
        VK_CONVERT = 0x1C,

        /// <summary> Virtual key for IME nonconvert. </summary>
        VK_NONCONVERT = 0x1D,

        /// <summary> Virtual key for IME mode change request. </summary>
        VK_ACCEPT = 0x1E,

        /// <summary> Virtual key for IME accept. </summary>
        VK_MODECHANGE = 0x1F,

        /// <summary> Virtual key for SPACEBAR. </summary>
        VK_SPACE = 0x20,
        /// <summary> Virtual key for PAGE UP key. </summary>
        VK_PRIOR = 0x21,
        /// <summary> Virtual key for PAGE DOWN key. </summary>
        VK_NEXT = 0x22,
        /// <summary> Virtual key for END key. </summary>
        VK_END = 0x23,
        /// <summary> Virtual key for HOME key. </summary>
        VK_HOME = 0x24,
        /// <summary> Virtual key for LEFT ARROW key. </summary>
        VK_LEFT = 0x25,
        /// <summary> Virtual key for UP ARROW key. </summary>
        VK_UP = 0x26,
        /// <summary> Virtual key for RIGHT ARROW key. </summary>
        VK_RIGHT = 0x27,
        /// <summary> Virtual key for DOWN ARROW key. </summary>
        VK_DOWN = 0x28,

        /// <summary> Virtual key for SELECT key. </summary>
        /// <remarks>
        /// VK_SELECT is the key code for a Select key that doesn't exist on most keyboards. I'm pretty sure that
        /// I haven't seen one.
        /// You can check to see if your keyboard supports it by calling the MapVirtualKey function, which can
        /// map the virtual key code to a keyboard scan code. If the function returns 0, then there is no mapping.
        /// For more info see
        /// <see href="http://stackoverflow.com/questions/23995537/what-is-the-select-key">
        /// What is the SELECT key?</see>
        /// </remarks>
        VK_SELECT = 0x29,

        /// <summary>
        /// VK_PRINT is NOT a PrintScreen key ( which is represented by <see cref="VK_SNAPSHOT"/>.
        /// The VK_PRINT keycode is from back in the days of the 83/84 key keyboard (think IBM XT and the IBM AT
        /// machines). The 'Print' key on this keyboards was shared with the numeric keypad's '*' key (instead of
        /// the PrtSc/SysRq key as is usual today).
        /// </summary>
        VK_PRINT = 0x2A,

        /// <summary> Virtual key for EXECUTE key. </summary>
        VK_EXECUTE = 0x2B,

        /// <summary> Virtual key for PRINTSCREEN key. </summary>
        VK_SNAPSHOT = 0x2C,
        /// <summary> Virtual key for Insert key. </summary>
        VK_INSERT = 0x2D,
        /// <summary> Virtual key for Delete key. </summary>
        VK_DELETE = 0x2E,

        /// <summary> VK_HELP key is NOT identical with a <see cref="VK_F1"/> key which is 0x70.
        /// The VK_HELP is for extended keyboards supporting MS Windows. 
        /// You can have a bunch of MS specific keys like: "New", "Open" "Save" "Replace", "Undo", "Redo", etc.
        /// </summary>
        VK_HELP = 0x2F,

        /// <summary> The 0 key. </summary>
        VK_0 = 0x30,
        /// <summary> The 1 key. </summary>
        VK_1 = 0x31,
        /// <summary> The 2 key. </summary>
        VK_2 = 0x32,
        /// <summary> The 3 key. </summary>
        VK_3 = 0x33,
        /// <summary> The 4 key. </summary>
        VK_4 = 0x34,
        /// <summary> The 5 key. </summary>
        VK_5 = 0x35,
        /// <summary> The 6 key. </summary>
        VK_6 = 0x36,
        /// <summary> The 7 key. </summary>
        VK_7 = 0x37,
        /// <summary> The 8 key. </summary>
        VK_8 = 0x38,
        /// <summary> The 9 key. </summary>
        VK_9 = 0x39,

        /// <summary> The F1 key. </summary>
        VK_F1 = 0x70,
        /// <summary> The F2 key. </summary>
        VK_F2 = 0x71,
        /// <summary> The F3 key. </summary>
        VK_F3 = 0x72,
        /// <summary> The F4 key. </summary>
        VK_F4 = 0x73,
        /// <summary> The F5 key. </summary>
        VK_F5 = 0x74,
        /// <summary> The F6 key. </summary>
        VK_F6 = 0x75,
        /// <summary> The F7 key. </summary>
        VK_F7 = 0x76,
        /// <summary> The F8 key. </summary>
        VK_F8 = 0x77,
        /// <summary> The F9 key. </summary>
        VK_F9 = 0x78,
        /// <summary> The F10 key. </summary>
        VK_F10 = 0x79,
        /// <summary> The F11 key. </summary>
        VK_F11 = 0x7A,
        /// <summary> The F12 key. </summary>
        VK_F12 = 0x7B,
        /// <summary> The F13 key. </summary>
        VK_F13 = 0x7C,
        /// <summary> The F14 key. </summary>
        VK_F14 = 0x7D,
        /// <summary> The F15 key. </summary>
        VK_F15 = 0x7E,
        /// <summary> The F16 key. </summary>
        VK_F16 = 0x7F,
        /// <summary> The F17 key. </summary>
        VK_F17 = 0x80,
        /// <summary> The F18 key. </summary>
        VK_F18 = 0x81,
        /// <summary> The F19 key. </summary>
        VK_F19 = 0x82,
        /// <summary> The F20 key. </summary>
        VK_F20 = 0x83,
        /// <summary> The F21 key. </summary>
        VK_F21 = 0x84,
        /// <summary> The F22 key. </summary>
        VK_F22 = 0x85,
        /// <summary> The F23 key. </summary>
        VK_F23 = 0x86,
        /// <summary> The F24 key. </summary>
        VK_F24 = 0x87,

        /// <summary> The NUM LOCK key. </summary>
        VK_NUMLOCK = 0x90,
        /// <summary> The SCROLL LOCK key. </summary>
        VK_SCROLL = 0x91,
    };

    /// <summary> Values that represent various combo-box types. </summary>
    public enum ComboType : uint
    {
        /// <summary> An enum constant representing a combo-box which displays the list box at all times. 
        ///  The current selection in the list box is displayed in the edit control. </summary>
        CBS_SIMPLE = (uint)0x0001L,

        /// <summary> An enum constant representing the type of combo-box similar to CBS_SIMPLE,
        /// except that the list box is not displayed unless the user selects an icon next to the edit control.
        /// </summary>
        CBS_DROPDOWN = (uint)0x0002L,

        /// <summary> An enum constant representing the type of combo-box similar to CBS_DROPDOWN, 
        /// except that the edit control is replaced by a static text item that displays the current selection 
        /// in the list box.
        ///  </summary>
        CBS_DROPDOWNLIST = (uint)0x0003L,
    }

    /// <summary>
    /// The structure used by <see cref="HookType.WH_MOUSE"/> system hook.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEHOOKSTRUCT
    {
        /// <summary> The x- and y-coordinates of the cursor, in screen coordinates. </summary>
        public User32.POINT pt;

        /// <summary>
        /// A handle to the window that will receive the mouse message corresponding to the mouse event.
        /// </summary>
        public IntPtr hwnd;

        /// <summary> The hit-test value. For a list of hit-test values, 
        ///           see the description of the WM_NCHITTEST message. </summary>
        public uint wHitTestCode;

        /// <summary> Additional information associated with the message. </summary>
        public IntPtr dwExtraInfo;
    };

    /// <summary>
    /// The structure used by <see cref="HookType.WH_MOUSE_LL"/> system hook.
    /// Contains information about a low-level mouse input event
    /// </summary>
    /// <seealso href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-msllhookstruct?redirectedfrom=MSDN">
    /// Win32 MSLLHOOKSTRUCT structure</seealso>
    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        /// <summary> The x- and y-coordinates of the cursor, in per-monitor-aware screen coordinates. </summary>
        public User32.POINT pt;

        /// <summary>
        /// If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP, or WM_NCXBUTTONDBLCLK, 
        /// the high-order word specifies which X button was pressed or released, and the low-order word is reserved. 
        /// For more details, see MSDN documentation.
        /// </summary>
        public int mouseData;

        /// <summary> The event-injected flags. </summary>
        public int flags;

        /// <summary> The time stamp for this message.. </summary>
        public int time;

        /// <summary> Additional information associated with the message.. </summary>
        public UIntPtr dwExtraInfo;
    };

    /// <summary>
    /// The structure used by <see cref="HookType.WH_KEYBOARD_LL"/> system hook.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        /// <summary> A virtual-key code. The code must be a value in the range 1 to 254.</summary>
        public uint vkCode;

        /// <summary> A hardware scan code for the key.</summary>
        public uint scanCode;

        /// <summary>
        /// The extended-key flag, event-injected flags, context code, and transition-state flag.
        /// </summary>
        public uint flags;

        /// <summary>
        /// The time stamp for this message, equivalent to what GetMessageTime would return for this message.
        /// </summary>
        public uint time;

        /// <summary> Additional information associated with the message. </summary>
        public uint dwExtraInfo;
    }

    /// <summary>
    /// The structure used by WH_CALLWNDPROCRET system hook.
    /// </summary>
    /// <remarks>DON"T apply StructLayout with Pack = 4, the code would not work on x64 systems.</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct CWPRETSTRUCT
    {
        /// <summary>
        /// The return value of the window procedure that processed the message specified by the message value.
        /// </summary>
        public IntPtr lResult;

        /// <summary>
        /// Additional information about the message. The exact meaning depends on the message value.
        /// </summary>
        public IntPtr lParam;

        /// <summary>
        /// Additional information about the message. The exact meaning depends on the message value.
        /// </summary>
        public IntPtr wParam;

        /// <summary> The message. </summary>
        public uint message;

        /// <summary>
        /// A handle to the window that processed the message specified by the message value.
        /// </summary>
        public IntPtr hwnd;
    };

    /// <summary> Lists specific constants that could be used as a second argument of 
    ///           <see cref="User32.SetWindowPos"/> API. </summary>
    /// <seealso cref="SWP_Flags"/>
    public enum SWP_Vals : int
    {
        /// <summary> When used as argument in SetWindowPos, places the window at the bottom of the Z order.
        /// If the hWnd parameter identifies a topmost window, the window loses its topmost status 
        /// and is placed at the bottom of all other windows.
        /// </summary>
        HWND_BOTTOM = 1,

        /// <summary> Places the window at the top of the Z order. </summary>
        HWND_TOP = 0,

        /// <summary> Places the window above all non-topmost windows. 
        /// The window maintains its topmost position even when it is deactivated.
        /// </summary>
        HWND_TOPMOST = (-1),

        /// <summary> When used as argument in SetWindowPos, places the window above all non-topmost windows 
        /// (that is, behind all topmost windows). 
        /// This flag has no effect if the window is already a non-topmost window.
        /// </summary>
        HWND_NOTOPMOST = (-2),
    }

    /// <summary> This enum is a bit-field of flags for specifying the last argument of 
    ///           <see cref="User32.SetWindowPos"/> API. </summary>
    /// <seealso cref="SWP_Vals"/>
    [Flags]
    public enum SWP_Flags : uint
    {
        /// <summary> Retains the current size (ignores the cx and cy parameters).</summary>
        SWP_NOSIZE = 0x0001,

        /// <summary> Retains the current position (ignores X and Y parameters). </summary>
        SWP_NOMOVE = 0x0002,

        /// <summary> Retains the current Z order (ignores the hWndInsertAfter parameter). </summary>
        SWP_NOZORDER = 0x0004,

        /// <summary> Does not redraw changes. If this flag is set, no repainting of any kind occurs. </summary>
        SWP_NOREDRAW = 0x0008,

        /// <summary>
        /// Does not activate the window. If this flag is not set, the window is activated and moved to the top
        /// of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
        /// parameter).
        /// </summary>
        SWP_NOACTIVATE = 0x0010,

        /// <summary>
        /// Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the
        /// window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE
        /// is sent only when the window's size is being changed.
        /// </summary>
        SWP_FRAMECHANGED = 0x0020,

        /// <summary> Displays the window. </summary>
        SWP_SHOWWINDOW = 0x0040,

        /// <summary> Hides the window. </summary>
        SWP_HIDEWINDOW = 0x0080,

        /// <summary>
        /// Discards the entire contents of the client area. If this flag is not specified, the valid contents of
        /// the client area are saved and copied back into the client area after the window is sized or
        /// repositioned.
        /// </summary>
        SWP_NOCOPYBITS = 0x0100,

        /// <summary> Does not change the owner window's position in the Z order. </summary>
        SWP_NOOWNERZORDER = 0x0200,

        /// <summary> Prevents the window from receiving the WM_WINDOWPOSCHANGING message. </summary>
        SWP_NOSENDCHANGING = 0x0400,

        /// <summary> Draws a frame (defined in the window's class description) around the window. </summary>
        SWP_DRAWFRAME = SWP_FRAMECHANGED,

        /// <summary> Same as the SWP_NOOWNERZORDER flag. </summary>
        SWP_NOREPOSITION = SWP_NOOWNERZORDER,
    }

    /// <summary> Values that represent possible negative Window field offsets that could be used
    /// as arguments when calling <see cref="User32.GetWindowLong"/> or <see cref="User32.SetWindowLong"/>.
    /// </summary>
    public enum GWL
    {
        /// <summary> 
        /// An enum constant representing the option that sets a new address for the window procedure. 
        /// </summary>
        GWL_WNDPROC = (-4),

        /// <summary> 
        /// An enum constant representing the option that Sets a new application instance handle.
        /// </summary>
        GWL_HINSTANCE = (-6),

        /// <summary> An enum constant representing the option that change the owner the window
        ///           ( NOT the parent of the window ).
        /// </summary>
        GWL_HWNDPARENT = (-8),

        /// <summary> An enum constant representing the option that sets a new identifier of the child window. 
        /// The window cannot be a top-level window.
        /// </summary>
        GWL_ID = (-12),

        /// <summary> An enum constant representing the option that sets a new window style.
        /// </summary>
        GWL_STYLE = (-16),

        /// <summary> An enum constant representing the option that sets a new extended window style.
        /// </summary>
        GWL_EXSTYLE = (-20),

        /// <summary> An enum constant representing the option that sets the user data associated 
        /// with the window. This data is intended for use by the application that created the window. 
        /// Its value is initially zero.
        /// </summary>
        GWL_USERDATA = (-21),
    }

    /// <summary> Values that represent ScrollBar commands. <br/>
    /// When the <see cref="Win32.WM.WM_HSCROLL"/> or <see cref="Win32.WM.WM_VSCROLL"/> message is received,
    /// the LOWORD of wParam argument specifies a scroll bar value that indicates the user's scrolling request. 
    /// This word can be one of the following enum values.
    /// </summary>
    public enum ScrollbarCommand
    {
        /// <summary> Scrolls one line up. </summary>
        SB_LINEUP = 0,

        /// <summary> Scrolls left by one unit. </summary>
        SB_LINELEFT = 0,

        /// <summary> Scrolls one line down.</summary>
        SB_LINEDOWN = 1,

        /// <summary> Scrolls right by one unit.</summary>
        SB_LINERIGHT = 1,

        /// <summary> Scrolls one page up. </summary>
        SB_PAGEUP = 2,

        /// <summary> Scrolls left by the width of the window. </summary>
        SB_PAGELEFT = 2,

        /// <summary> Scrolls one page down. </summary>
        SB_PAGEDOWN = 3,

        /// <summary> Scrolls right by the width of the window. </summary>
        SB_PAGERIGHT = 3,

        /// <summary> The user has dragged the scroll box (thumb) and released the mouse button. 
        /// The HIWORD indicates the position of the scroll box at the end of the drag operation.
        /// </summary>
        SB_THUMBPOSITION = 4,

        /// <summary> The user is dragging the scroll box. 
        /// This message is sent repeatedly until the user releases the mouse button. 
        /// The HIWORD indicates the position that the scroll box has been dragged to.
        /// </summary>
        SB_THUMBTRACK = 5,

        /// <summary> Scrolls to the upper left. </summary>
        SB_TOP = 6,

        /// <summary> Scrolls to the upper left.</summary>
        SB_LEFT = 6,

        /// <summary> Scrolls to the lower right. </summary>
        SB_BOTTOM = 7,

        /// <summary> Scrolls to the lower right.</summary>
        SB_RIGHT = 7,

        /// <summary> Ends scroll.</summary>
        SB_ENDSCROLL = 8,
    };

    /// <summary>
    /// Enum values that represent system colors, which can be  used as an argument of
    /// <see cref="User32.GetSysColor"/>.
    /// </summary>
    public enum SysColor
    {
        /// <summary> An enum constant representing the color of scroll bar gray area. </summary>
        COLOR_SCROLLBAR = 0,

        /// <summary> An enum constant representing the color of desktop. </summary>
        COLOR_BACKGROUND = 1,

        /// <summary> An enum constant representing the color of active window title bar. 
        /// If the gradient effect is enabled, specifies the left side color in the color gradient of 
        /// an active window's title bar.
        /// </summary>
        COLOR_ACTIVECAPTION = 2,

        /// <summary> An enum constant representing the color of inactive window caption.
        /// If the gradient effect is enabled, specifies the left side color in the color gradient of 
        /// an inactive window's title bar.
        /// </summary>
        COLOR_INACTIVECAPTION = 3,

        /// <summary> An enum constant representing the color of menu background. </summary>
        COLOR_MENU = 4,

        /// <summary> An enum constant representing the color of window background. </summary>
        COLOR_WINDOW = 5,

        /// <summary> An enum constant representing the color of window frame. </summary>
        COLOR_WINDOWFRAME = 6,

        /// <summary> An enum constant representing the color of text in menus. </summary>
        COLOR_MENUTEXT = 7,

        /// <summary> An enum constant representing the color of text in windows. </summary>
        COLOR_WINDOWTEXT = 8,

        /// <summary> An enum constant representing the color of text in caption, size box, 
        ///  and scroll bar arrow box. </summary>
        COLOR_CAPTIONTEXT = 9,

        /// <summary> An enum constant representing the color of active window border. </summary>
        COLOR_ACTIVEBORDER = 10,

        /// <summary> An enum constant representing the color of inactive window border. </summary>
        COLOR_INACTIVEBORDER = 11,

        /// <summary> An enum constant representing the background color of multiple document interface 
        /// (MDI) applications. </summary>
        COLOR_APPWORKSPACE = 12,

        /// <summary> An enum constant representing the color of item(s) selected in a control. </summary>
        COLOR_HIGHLIGHT = 13,

        /// <summary> An enum constant representing the color of text of item(s) selected in a control. </summary>
        COLOR_HIGHLIGHTTEXT = 14,

        /// <summary> An enum constant representing the face color for three-dimensional display elements 
        /// and for dialog box backgrounds.. </summary>
        COLOR_BTNFACE = 15,

        /// <summary> An enum constant representing the shadow color for three-dimensional display elements 
        /// (for edges facing away from the light source). </summary>
        COLOR_BTNSHADOW = 16,

        /// <summary> An enum constant representing the color of grayed (disabled) text. 
        /// This color is set to 0 if the current display driver does not support a solid gray color. </summary>
        COLOR_GRAYTEXT = 17,

        /// <summary> An enum constant representing the color of text on push buttons. </summary>
        COLOR_BTNTEXT = 18,

        /// <summary> An enum constant representing the color of text in an inactive caption. </summary>
        COLOR_INACTIVECAPTIONTEXT = 19,

        /// <summary> An enum constant representing the highlight color for three-dimensional 
        /// display elements (for edges facing the light source). </summary>
        COLOR_BTNHIGHLIGHT = 20,

        /// <summary> An enum constant representing the color of dark shadow for three-dimensional 
        /// display elements. </summary>
        COLOR_3DDKSHADOW = 21,

        /// <summary> An enum constant representing the light color for three-dimensional display elements 
        /// (for edges facing the light source). </summary>
        COLOR_3DLIGHT = 22,

        /// <summary> An enum constant representing the color of text for tooltip controls. </summary>
        COLOR_INFOTEXT = 23,

        /// <summary> An enum constant representing the background color for tooltip controls. </summary>
        COLOR_INFOBK = 24,

        /// <summary> An enum constant representing the color for a hyperlink or hot-tracked item. </summary>
        COLOR_HOTLIGHT = 26,

        /// <summary> An enum constant representing the right side color in the color gradient 
        /// of an active window's title bar. COLOR_ACTIVECAPTION specifies the left side color. 
        /// Use SPI_GETGRADIENTCAPTIONS with the SystemParametersInfo function to determine 
        /// whether the gradient effect is enabled. </summary>
        COLOR_GRADIENTACTIVECAPTION = 27,

        /// <summary> An enum constant representing the right side color in the color gradient 
        /// of an inactive window's title bar. COLOR_INACTIVECAPTION specifies the left side color. </summary>
        COLOR_GRADIENTINACTIVECAPTION = 28,

        /// <summary> An enum constant representing the color used to highlight menu items when the menu 
        /// appears as a flat menu (see SystemParametersInfo). 
        /// The highlighted menu item is outlined with COLOR_HIGHLIGHT. </summary>
        COLOR_MENUHILIGHT = 29,

        /// <summary> An enum constant representing the background color for the menu bar when menus 
        /// appear as flat menus (see SystemParametersInfo). 
        /// However, COLOR_MENU continues to specify the background color of the menu popup. </summary>
        COLOR_MENUBAR = 30,
    };

    // constants for GetSystemMetrics

    /// <summary>
    /// Enum values that represent possible nIndex input argument of
    /// <see cref="User32.GetSystemMetrics"/>.
    /// </summary>
    public enum SM
    {
        /// <summary> Used to retrieve the width of the screen of the primary display monitor, in pixels. </summary>
        SM_CXSCREEN = 0,
        /// <summary> Used to retrieve the height of the screen of the primary display monitor, in pixels. </summary>
        SM_CYSCREEN = 1,
        /// <summary> Used to retrieve the width of a vertical scroll bar, in pixels. </summary>
        SM_CXVSCROLL = 2,
        /// <summary> Used to retrieve the height of a vertical scroll bar, in pixels. </summary>
        SM_CYHSCROLL = 3,
        /// <summary> Used to retrieve the height of a caption area, in pixels. </summary>
        SM_CYCAPTION = 4,
        /// <summary> Used to retrieve the width of a window border, in pixels. </summary>
        SM_CXBORDER = 5,
        /// <summary> Used to retrieve the height of a window border, in pixels. </summary>
        SM_CYBORDER = 6,
        /// <summary> This value is the same as <see cref="SM_CXFIXEDFRAME"/>. </summary>
        SM_CXDLGFRAME = 7,
        /// <summary> This value is the same as <see cref="SM_CYFIXEDFRAME"/>. </summary>
        SM_CYDLGFRAME = 8,
        /// <summary> Used to retrieve the height of the thumb box in a vertical scroll bar, in pixels.. </summary>
        SM_CYVTHUMB = 9,
        /// <summary> Used to retrieve the width of the thumb box in a vertical scroll bar, in pixels.. </summary>
        SM_CXHTHUMB = 10,
        /// <summary> Used to retrieve the default width of an icon, in pixels. </summary>
        SM_CXICON = 11,
        /// <summary> Used to retrieve the default height of an icon, in pixels. </summary>
        SM_CYICON = 12,
        /// <summary> Used to retrieve the width of a cursor, in pixels. The system cannot create cursors of other sizes. </summary>
        SM_CXCURSOR = 13,
        /// <summary> Used to retrieve the height of a cursor, in pixels. The system cannot create cursors of other sizes. </summary>
        SM_CYCURSOR = 14,
        /// <summary> The height of a single-line menu bar, in pixels.. </summary>
        SM_CYMENU = 15,
        /// <summary> Used to retrieve the width of the client area for a full-screen window on the primary display 
        /// monitor, in pixels. To get the coordinates of the portion of the screen that is not obscured by the system taskbar 
        /// or by application desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value. </summary>
        SM_CXFULLSCREEN = 16,
        /// <summary> Used to retrieve the height of the client area for a full-screen window on the primary display 
        /// monitor, in pixels. To get the coordinates of the portion of the screen not obscured by the system taskbar 
        /// or by application desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value. </summary>
        SM_CYFULLSCREEN = 17,
        /// <summary> For double byte character set versions of the system, this is the height of the Kanji window 
        /// at the bottom of the screen, in pixels. </summary>
        SM_CYKANJIWINDOW = 18,
        /// <summary> Used to retrieve the value that is nonzero if a mouse is installed; otherwise, 0. 
        /// This value is rarely zero, because of support for virtual mice and because some systems detect
        /// the presence of the port instead of the presence of a mouse. 
        /// </summary>
        SM_MOUSEPRESENT = 19,
        /// <summary> Used to retrieve the height of the arrow bitmap on a vertical scroll bar, in pixels. </summary>
        SM_CYVSCROLL = 20,
        /// <summary> Used to retrieve the width of the arrow bitmap on a horizontal scroll bar, in pixels. </summary>
        SM_CXHSCROLL = 21,
        /// <summary> Used to retrieve the value that is nonzero if the debug version of User.exe is installed; otherwise, 0.</summary>
        SM_DEBUG = 22,
        /// <summary> Used to retrieve the value that is nonzero if the meanings of the left and right mouse buttons are swapped; otherwise, 0.</summary>
        SM_SWAPBUTTON = 23,

        /*
        SM_RESERVED1 = 24,
        SM_RESERVED2 = 25,
        SM_RESERVED3 = 26,
        SM_RESERVED4 = 27,
         */
        /// <summary> Used to retrieve the value of the minimum width of a window, in pixels.</summary>
        SM_CXMIN = 28,
        /// <summary> Used to retrieve the value of the minimum height of a window, in pixels.</summary>
        SM_CYMIN = 29,
        /// <summary> Used to retrieve the width of a button in a window caption or title bar, in pixels.</summary>
        SM_CXSIZE = 30,
        /// <summary> Used to retrieve the height of a button in a window caption or title bar, in pixels.</summary>
        SM_CYSIZE = 31,
        /// <summary> This value is the same as <see cref="SM_CXSIZEFRAME"/>. </summary>
        SM_CXFRAME = 32,
        /// <summary> This value is the same as <see cref="SM_CYSIZEFRAME"/>. </summary>
        SM_CYFRAME = 33,
        /// <summary> The minimum tracking width of a window, in pixels. The user cannot drag the window frame 
        /// to a size smaller than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message. </summary>
        SM_CXMINTRACK = 34,
        /// <summary> The minimum tracking height of a window, in pixels. The user cannot drag the window frame 
        /// to a size smaller than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message. </summary>
        SM_CYMINTRACK = 35,
        /// <summary> Used to retrieve the width of the rectangle around the location of a first click in a double-click sequence, 
        /// in pixels. The second click must occur within the rectangle that is defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK 
        /// for the system to consider the two clicks a double-click. The two clicks must also occur within a specified time. </summary>
        SM_CXDOUBLECLK = 36,
        /// <summary> Used to retrieve the height of the rectangle around the location of a first click in a double-click sequence, 
        /// in pixels. The second click must occur within the rectangle that is defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK 
        /// for the system to consider the two clicks a double-click. The two clicks must also occur within a specified time. </summary>
        SM_CYDOUBLECLK = 37,
        /// <summary> Used to retrieve the width of a grid cell for items in large icon view, in pixels. </summary>
        SM_CXICONSPACING = 38,
        /// <summary> Used to retrieve the height of a grid cell for items in large icon view, in pixels. </summary>
        SM_CYICONSPACING = 39,
        /// <summary> Used to retrieve the value that is nonzero if drop-down menus are right-aligned 
        /// with the corresponding menu-bar item; 0 if the menus are left-aligned. </summary>
        SM_MENUDROPALIGNMENT = 40,
        /// <summary> Used to retrieve the value that is nonzero if the Microsoft Windows for Pen computing extensions 
        /// are installed; zero otherwise.. </summary>
        SM_PENWINDOWS = 41,
        /// <summary> Used to retrieve the value that is nonzero if User32.dll supports DBCS; otherwise, 0. </summary>
        SM_DBCSENABLED = 42,
        /// <summary> Used to retrieve the number of buttons on a mouse, or zero if no mouse is installed. </summary>
        SM_CMOUSEBUTTONS = 43,
        /// <summary> Used to retrieve the thickness of the frame around the perimeter of a window that has a caption but is not sizable, 
        /// in pixels. SM_CXFIXEDFRAME is the height of the horizontal border, and SM_CYFIXEDFRAME is the width of the vertical border. 
        /// This value is the same as <see cref="SM_CXDLGFRAME"/>.
        /// </summary>
        SM_CXFIXEDFRAME = SM_CXDLGFRAME,
        /// <summary> Used to retrieve the thickness of the frame around the perimeter of a window that has a caption but is not sizable, 
        /// in pixels. SM_CXFIXEDFRAME is the height of the horizontal border, and SM_CYFIXEDFRAME is the width of the vertical border. 
        /// This value is the same as <see cref="SM_CYDLGFRAME"/>.
        /// </summary>
        SM_CYFIXEDFRAME = SM_CYDLGFRAME,
        /// <summary> Used to retrieve the thickness of the sizing border around the perimeter of a window 
        /// that can be resized, in pixels. SM_CXSIZEFRAME is the width of the horizontal border, 
        /// and SM_CYSIZEFRAME is the height of the vertical border.
        /// This value is the same as <see cref="SM_CXFRAME"/>.
        /// </summary>
        SM_CXSIZEFRAME = SM_CXFRAME,
        /// <summary> Used to retrieve the thickness of the sizing border around the perimeter of a window 
        /// that can be resized, in pixels. SM_CXSIZEFRAME is the width of the horizontal border, 
        /// and SM_CYSIZEFRAME is the height of the vertical border. 
        /// This value is the same as <see cref="SM_CYFRAME"/>.
        /// </summary>
        SM_CYSIZEFRAME = SM_CYFRAME,

        SM_SECURE = 44,
        SM_CXEDGE = 45,
        SM_CYEDGE = 46,
        SM_CXMINSPACING = 47,
        SM_CYMINSPACING = 48,
        SM_CXSMICON = 49,
        SM_CYSMICON = 50,
        SM_CYSMCAPTION = 51,
        SM_CXSMSIZE = 52,
        SM_CYSMSIZE = 53,
        SM_CXMENUSIZE = 54,
        SM_CYMENUSIZE = 55,
        SM_ARRANGE = 56,
        SM_CXMINIMIZED = 57,
        SM_CYMINIMIZED = 58,
        SM_CXMAXTRACK = 59,
        SM_CYMAXTRACK = 60,
        SM_CXMAXIMIZED = 61,
        SM_CYMAXIMIZED = 62,
        SM_NETWORK = 63,
        SM_CLEANBOOT = 67,
        SM_CXDRAG = 68,
        SM_CYDRAG = 69,
        SM_SHOWSOUNDS = 70,
        SM_CXMENUCHECK = 71,
        SM_CYMENUCHECK = 72,
        SM_SLOWMACHINE = 73,
        SM_MIDEASTENABLED = 74,
        SM_MOUSEWHEELPRESENT = 75,
        SM_XVIRTUALSCREEN = 76,
        SM_YVIRTUALSCREEN = 77,
        SM_CXVIRTUALSCREEN = 78,
        SM_CYVIRTUALSCREEN = 79,
        SM_CMONITORS = 80,
        SM_SAMEDISPLAYFORMAT = 81,
        SM_IMMENABLED = 82,
        SM_CXFOCUSBORDER = 83,
        SM_CYFOCUSBORDER = 84,
        SM_TABLETPC = 86,
        SM_MEDIACENTER = 87,
        SM_STARTER = 88,
        SM_SERVERR2 = 89,
        SM_REMOTESESSION = 0x1000,
        SM_SHUTTINGDOWN = 0x2000,
    };


    /*
     * Standard Cursor IDs
     */
    public const int IDC_ARROW = 32512;
    public const int IDC_IBEAM = 32513;
    public const int IDC_WAIT = 32514;
    public const int IDC_CROSS = 32515;
    public const int IDC_UPARROW = 32516;
    public const int IDC_SIZE = 32640;  /* OBSOLETE: use IDC_SIZEALL */
    public const int IDC_ICON = 32641;  /* OBSOLETE: use IDC_ARROW */
    public const int IDC_SIZENWSE = 32642;
    public const int IDC_SIZENESW = 32643;
    public const int IDC_SIZEWE = 32644;
    public const int IDC_SIZENS = 32645;
    public const int IDC_SIZEALL = 32646;
    public const int IDC_NO = 32648; /*not in win3.1 */

    #endregion // Nested types

    #region External functions

    // imm32 -------------------
    #region imm32
    /// <summary>	Returns the input context associated with the specified window. </summary>
    /// <remarks>
    /// An application should routinely use this function to retrieve the current input context before attempting
    /// to access information in the context.<br/>
    /// The application must call <see cref="ImmReleaseContext"/> when it is finished with the input context.
    /// </remarks>
    /// <param name="hwnd">	Handle to the window for which to retrieve the input context. </param>
    /// <returns>	Returns the handle to the input context. </returns>
    /// <seealso cref="ImmReleaseContext"/>
    [DllImport("imm32.dll")]
    public static extern IntPtr ImmGetContext(IntPtr hwnd);

    /// <summary> Releases the input context and unlocks the memory associated in the input context. 
    /// An application must call this function for each call to the <see cref="ImmGetContext"/> function. </summary>
    /// <param name="hwnd">	Handle to the window for which the input context was previously retrieved. </param>
    /// <param name="himc">	Handle to the input context. </param>
    /// <returns> Returns a nonzero value if successful, or 0 otherwise.. </returns>
    /// <seealso cref="ImmGetContext"/>
    [DllImport("imm32.dll")]
    public static extern int ImmReleaseContext(IntPtr hwnd, IntPtr himc);

    /// <summary>
    /// Truncates a path to fit within a certain number of characters by replacing path components with ellipses.
    /// </summary>
    ///
    /// <param name="pszOut">   The resulting string that has been altered.</param>
    /// <param name="szPath">   The string that contains the path to be altered.</param>
    /// <param name="cchMax">   The maximum number of characters to be contained in the new string, 
    ///                         including the terminating null character. </param>
    /// <param name="dwFlags">  The flags. Reserved argument currently not used.</param>
    ///
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PathCompactPathEx(
      [MarshalAs(UnmanagedType.LPWStr)][Out] StringBuilder pszOut,
      string szPath,
      int cchMax,
      int dwFlags);

    #endregion // imm32
    #endregion // External functions

    #region Member functions

    /// <summary> Retrieves the low-order word from the specified value. </summary>
    /// <param name="l" type="int"> The value to be converted. </param>
    /// <returns> The return value is the low-order word of the specified value. </returns>
    public static ushort LOWORD(uint l)
    {
        return unchecked((ushort)(l));
    }

    /// <summary> Retrieves the high-order word from the specified value. </summary>
    /// <param name="l" type="int"> The value to be converted. </param>
    /// <returns> The return value is the high-order word of the specified value. </returns>
    public static ushort HIWORD(uint l)
    {
        return unchecked((ushort)(l >> 16));
    }

    /// <summary> Retrieves the low-order word from the specified value. </summary>
    /// <param name="l">  The value to be converted. </param>
    /// <returns> The return value is the low-order word of the specified value. </returns>
    public static ushort LOWORD(int l) { return LOWORD((uint)(l)); }

    /// <summary> Retrieves the high-order word from the specified value. </summary>
    /// <param name="l">  The value to be converted. </param>
    /// <returns> The return value is the high-order word of the specified value. </returns>
    public static ushort HIWORD(int l) { return HIWORD((uint)(l)); }


    /// <summary> Retrieves the x-coordinate from the specified LPARAM value. </summary>
    /// <remarks>
    /// Do NOT use the LOWORD or HIWORD macros to extract the x- and y- coordinates of the cursor position
    /// because these macros return incorrect results on systems with multiple monitors.
    /// Systems with multiple monitors can have negative x- and y- coordinates,
    /// and LOWORD and HIWORD treat the coordinates as unsigned quantities.
    /// </remarks>
    /// 
    /// <param name="lParam"> The value to be converted, usually Message.LParam. </param>
    /// <returns>   The return value is the low-order ushort of the specified value. </returns>
    public static short GET_X_LPARAM(int lParam) { return (short)((lParam) & 0xFFFF); }

    /// <summary> Retrieves the y-coordinate from the specified LPARAM value.</summary>
    /// <remarks> 
    /// Do NOT use the LOWORD or HIWORD macros to extract the x- and y- coordinates of the cursor position. 
    /// </remarks>
    /// 
    /// <param name="lParam"> The value to be converted, usually Message.LParam. </param>
    /// <returns> The return value is the high-order ushort of the specified value.</returns>
    public static short GET_Y_LPARAM(int lParam) { return (short)(((lParam) >> 16) & 0xFFFF); }

    /// <summary>  Get Point coordinates from l-parameter. </summary>
    /// <param name="lParam"> The value to be converted, usually Message.LParam. </param>
    /// <returns>   A POINT. </returns>
    public static User32.POINT PointFromLParam(int lParam)
    {
        return new User32.POINT(GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));
    }

    /// <summary>   Gets point from l-parameter. </summary>
    /// <param name="lParam"> The value to be converted, usually Message.LParam. </param>
    /// <returns>   The point from l-parameter. </returns>
    public static System.Drawing.Point GetPointFromLParam(int lParam)
    {
        return new System.Drawing.Point(GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));
    }

    /// <summary> Creates an unsigned 32-bit value by concatenating two given 16-bit values. </summary>
    /// <param name="l" type="ushort"> Specifies the low-order word of the new value. </param>
    /// <param name="h" type="ushort"> Specifies the high-order word of the new long value. </param>
    /// <returns> The return value is an unsigned 32-bit value. </returns>
    public static uint MAKELONG(ushort l, ushort h)
    {
        uint ul = l;
        uint uh = (((uint)h) << 16);
        uint result = ul | uh;
        return result;
    }

    /// <summary> Creates a value for use as an lParam parameter in a message. 
    /// The method concatenates the specified values, analogical way like <see cref="MAKELONG"/>
    /// </summary>
    /// <param name="l" type="ushort"> Specifies the low-order word of the new value. </param>
    /// <param name="h" type="ushort"> Specifies the high-order word of the new long value. </param>
    /// <returns> A new IntPtr value, encapsulating an unsigned 32-bit value. </returns>
    public static IntPtr MAKELPARAM(ushort l, ushort h)
    {
        return (int)MAKELONG(l, h);
    }

    /// <summary> Converts a Win32 error code to an HRESULT using the pattern 0x8007XXXX, 
    /// where XXXX is the first two bytes of the Win32 hex value 0x0000XXXX. </summary>
    /// 
    /// <param name="errorCode"> Win32 error code. </param>
    /// <returns> A HRESULT. </returns>
    /// <seealso href="https://msdn.microsoft.com/en-us/library/gg567305.aspx">
    /// C++ HRESULT From WIN32 Error Code Macro</seealso>
    internal static int MakeHRFromErrorCode(int errorCode)
    {
        return (errorCode <= 0) ? errorCode : unchecked((int)0x80070000 | (errorCode & 0x0000FFFF));
    }
    #endregion // Member functions

    #region Constructor(s)

    /// <summary>
    /// Static constructor, performs PrelinkAll check
    /// </summary>
    static Win32()
    {
        try
        {
            Marshal.PrelinkAll(typeof(Win32));
        }
#if DEBUG
        catch (Exception ex)
        {
            string strMsg = $"PrelinkAll failed for '{typeof(Win32)}', with exception: '{ex.Message}', stack trace '{ex.StackTrace}'";
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
#pragma warning restore IDE0130
#pragma warning restore CA1401
#pragma warning restore CA1069
#pragma warning restore 1591
#pragma warning restore VSSpell001
#pragma warning restore IDE0079
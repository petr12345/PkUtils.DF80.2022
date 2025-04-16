/***************************************************************************************************************
*
* FILE NAME:   .\ShellLib\ShellGUIDs.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION:  The file contains definition of class ShellGUIDs
*
**************************************************************************************************************/

///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Microsoft Public License (MS-PL) notice
// 
// This software is a Derivative Work based upon a Code Project article series
// C# does Shell, Part 1 – 3	
// http://www.codeproject.com/Articles/3551/C-does-Shell-Part-1
// http://www.codeproject.com/Articles/3590/C-does-Shell-Part-2	
// http://www.codeproject.com/Articles/3728/C-does-Shell-Part-3
// published under Microsoft Public License.
// 
// The related Microsoft Public License (MS-PL) text is available at
// http://www.opensource.org/licenses/ms-pl.html
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Ignore Spelling: Utils, Toolbar, App, Taskbar, Toolbars, Appbars, appbar
//
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.ShellLib;

/// <summary>
/// A base class for creating custom Application Desktop Toolbars, shortly called Appbars.<br/>
/// 'Appbars' are application that can be aligned to one of the screen edges. For example ICQ is an appbar.<br/>
/// 
/// For more info, see 
/// <see href="http://www.codeproject.com/Articles/3728/C-does-Shell-Part-3"> C# does Shell, Part 3
/// </see> article on CodeProject.<br/>
/// 
/// For still more information and native code example, see also 
/// <see href="http://www.microsoft.com/msj/archive/S274.aspx"> 
/// Extend the Windows 95 Shell with Application Desktop Toolbars
/// </see> from the March 1996 issue of Microsoft Systems Journal.<br/>
/// 
/// </summary>
[CLSCompliant(true)]
public class ApplicationDesktopToolbar : Form
{
    #region Typedefs

    /// <summary>
    /// Codes of messages being sent by <see cref="ShellApi.SHAppBarMessage"/>.
    /// </summary>
    public enum AppBarMessages
    {
        /// <summary>
        /// Registers a new appbar and specifies the message identifier
        /// that the system should use to send notification messages to 
        /// the appbar. 
        /// </summary>
        New = 0x00000000,
        /// <summary>
        /// Unregisters an appbar, removing the bar from the system's 
        /// internal list.
        /// </summary>
        Remove = 0x00000001,
        /// <summary>
        /// Requests a size and screen position for an appbar.
        /// </summary>
        QueryPos = 0x00000002,
        /// <summary>
        /// Sets the size and screen position of an appbar. 
        /// </summary>
        SetPos = 0x00000003,
        /// <summary>
        /// Retrieves the autohide and always-on-top states of the 
        /// Microsoft® Windows® taskbar. 
        /// </summary>
        GetState = 0x00000004,
        /// <summary>
        /// Retrieves the bounding rectangle of the Windows taskbar. 
        /// </summary>
        GetTaskBarPos = 0x00000005,
        /// <summary>
        /// Notifies the system that an appbar has been activated. 
        /// </summary>
        Activate = 0x00000006,
        /// <summary>
        /// Retrieves the handle to the autohide appbar associated with
        /// a particular edge of the screen. 
        /// </summary>
        GetAutoHideBar = 0x00000007,
        /// <summary>
        /// Registers or unregisters an autohide appbar for an edge of 
        /// the screen. 
        /// </summary>
        SetAutoHideBar = 0x00000008,
        /// <summary>
        /// Notifies the system when an appbar's position has changed. 
        /// </summary>
        WindowPosChanged = 0x00000009,
        /// <summary>
        /// Sets the state of the appbar's autohide and always-on-top 
        /// attributes.
        /// </summary>
        SetState = 0x0000000a
    }

    /// <summary>
    /// The notification codes, being sent to <see cref="OnAppbarNotification"/>
    /// </summary>
    public enum AppBarNotifications
    {
        /// <summary>
        /// Notifies an appbar that the taskbar's autohide or 
        /// always-on-top state has changed—that is, the user has selected 
        /// or cleared the "Always on top" or "Auto hide" check box on the
        /// taskbar's property sheet. 
        /// </summary>
        StateChange = 0x00000000,
        /// <summary>
        /// Notifies an appbar when an event has occurred that may affect 
        /// the appbar's size and position. Events include changes in the
        /// taskbar's size, position, and visibility state, as well as the
        /// addition, removal, or resizing of another appbar on the same 
        /// side of the screen.
        /// </summary>
        PosChanged = 0x00000001,
        /// <summary>
        /// Notifies an appbar when a full-screen application is opening or
        /// closing. This notification is sent in the form of an 
        /// application-defined message that is set by the ABM_NEW message. 
        /// </summary>
        FullScreenApp = 0x00000002,
        /// <summary>
        /// Notifies an appbar that the user has selected the Cascade, 
        /// Tile Horizontally, or Tile Vertically command from the 
        /// taskbar's shortcut menu.
        /// </summary>
        WindowArrange = 0x00000003
    }

    /// <summary>
    /// Flags describing various appbar states
    /// </summary>
    [Flags]
    public enum AppBarStates
    {
        /// <summary>
        /// If set, appbar is auto-hide
        /// </summary>
        AutoHide = 0x00000001,
        /// <summary>
        /// If set, appbar is always on top
        /// </summary>
        AlwaysOnTop = 0x00000002
    }

    /* has been moved to ShellApi.cs
    /// <summary>
    /// The enum representing the current Appbar position (which screen edge it is aligned to).
    /// </summary>
    public enum AppBarEdges
    {
      Left = 0,
      Top = 1,
      Right = 2,
      Bottom = 3,
      Float = 4
    }
    */

    /// <summary>
    /// Window Messages used with ApplicationDesktopToolbar
    /// </summary>
    public enum WM
    {
        /// <summary> Sent to both the window being activated and the window being deactivated. 
        /// If the windows use the same input queue, the message is sent synchronously, 
        /// first to the window procedure of the top-level window being deactivated, 
        /// then to the window procedure of the top-level window being activated. 
        /// If the windows use different input queues, the message is sent asynchronously, 
        /// so the window is activated immediately.
        /// </summary>
        WM_ACTIVATE = 0x0006,

        /// <summary> Sent to a window whose size, position, or place in the Z order has changed 
        /// as a result of a call to the SetWindowPos function or another window-management function.
        /// </summary>
        WM_WINDOWPOSCHANGED = 0x0047,

        /// <summary> Sent to a window in order to determine what part of the window corresponds 
        /// to a particular screen coordinate. This can happen, for example, when the cursor moves, 
        /// when a mouse button is pressed or released, 
        /// or in response to a call to a function such as WindowFromPoin. </summary>
        WM_NCHITTEST = 0x0084
    }
    #endregion // Typedefs

    #region Fields
    #region Private Fields

    // saves the current edge
    private ShellApi.AppBarEdges _Edge = ShellApi.AppBarEdges.Float;

    // saves the callback message id
    private readonly uint CallbackMessageID = 0;

    // are we in dock mode?
    private bool IsAppbarMode = false;

    // save the floating size and location
    private Size _PrevSize;
    private Point _PrevLocation;

    #endregion // Private Fields
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public ApplicationDesktopToolbar()
    {
        FormBorderStyle = FormBorderStyle.SizableToolWindow;

        // Register a unique message as our callback message
        CallbackMessageID = RegisterCallbackMessage();
        if (CallbackMessageID == 0)
        {
            throw new InvalidOperationException("RegisterCallbackMessage failed");
        }
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Represents the current edge of AppBar.
    /// </summary>
    public ShellApi.AppBarEdges Edge
    {
        get
        {
            return _Edge;
        }
        set
        {
            _Edge = value;

            if (value == ShellApi.AppBarEdges.Float)
            {
                AppbarRemove();
            }
            else
            {
                AppbarNew();
            }

            if (IsAppbarMode)
            {
                SizeAppBar();
            }
        }
    }
    #endregion // Properties

    #region Methods

    #region Protected Methods

    #region implementation helpers

    /// <summary>
    /// Creates a new ShellApi.APPBARDATA structure, filling-in some of its fields
    /// </summary>
    /// <returns>An instance of <see cref="ShellApi.APPBARDATA"/></returns>
    protected ShellApi.APPBARDATA PrepareAppbarData()
    {
        ShellApi.APPBARDATA msgData = new(this.Handle)
        {
            uCallbackMessage = CallbackMessageID,
            Edge = this.Edge
        };

        return msgData;
    }

    /// <summary>
    /// Registers or un-registers an autohide appbar for its current edge of the screen.
    /// </summary>
    /// <param name="hideValue">True for autohide, false for turning autohide off. </param>
    /// <returns>True for success, false for failure.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb787957(v=vs.85).aspx">
    /// MSDN help about ABM_SETAUTOHIDEBAR message
    /// </seealso>
    protected bool AppbarSetAutoHideBar(bool hideValue)
    {
        // prepare data structure of message
        ShellApi.APPBARDATA msgData = PrepareAppbarData();
        msgData.lParam = (hideValue) ? 1 : 0;

        // set auto hide
        IntPtr retVal = ShellApi.SHAppBarMessage((uint)AppBarMessages.SetAutoHideBar, ref msgData);
        return (retVal != IntPtr.Zero);
    }

    /// <summary>
    /// Returns the Win32 handle to the autohide appbar. 
    /// </summary>
    /// <param name="edge">The enum representing the particular Appbar position we are interested in.</param>
    /// <returns>The return value is IntPtr.Zero if an error occurs or if no autohide appbar is associated with the given edge.</returns>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb787945(v=vs.85).aspx">
    /// MSDN help about ABM_GETAUTOHIDEBAR message
    /// </seealso>
    protected IntPtr AppbarGetAutoHideBar(ShellApi.AppBarEdges edge)
    {
        // prepare data structure of message
        ShellApi.APPBARDATA msgData = PrepareAppbarData();

        // get auto hide
        IntPtr retVal = ShellApi.SHAppBarMessage((uint)AppBarMessages.GetAutoHideBar, ref msgData);
        return retVal;
    }

    /// <summary>
    /// Getting current Appbar state value
    /// </summary>
    /// <returns>The current <see cref="AppBarStates"/> value</returns>
    protected AppBarStates AppbarGetTaskbarState()
    {
        // prepare data structure of message
        ShellApi.APPBARDATA msgData = PrepareAppbarData();

        // get taskbar state
        IntPtr retVal = ShellApi.SHAppBarMessage((uint)AppBarMessages.GetState, ref msgData);
        return (AppBarStates)retVal;
    }

    /// <summary>
    /// Setting new Appbar state
    /// </summary>
    /// <param name="state">new AppBarStates value</param>
    protected void AppbarSetTaskbarState(AppBarStates state)
    {
        // prepare data structure of message
        ShellApi.APPBARDATA msgData = PrepareAppbarData();
        msgData.lParam = (int)state;

        // set taskbar state.
        ShellApi.SHAppBarMessage((uint)AppBarMessages.SetState, ref msgData);

        DoChangeTaskbarState(state);
    }
    #endregion // implementation helpers

    #region Window Procedure

    /// <summary>
    /// Overwrites the virtual method of the predecessor, to provide custom processing for WM_ACTIVATE,
    /// WM_WINDOWPOSCHANGED and WM_NCHITTEST.
    /// </summary>
    ///
    /// <param name="m">  [in,out] The Message structure which wraps Win32 messages that Windows sends. </param>
    protected override void WndProc(ref Message m)
    {
        if (IsAppbarMode)
        {
            if (m.Msg == CallbackMessageID)
            {
                OnAppbarNotification(ref m);
            }
            else if (m.Msg == (int)WM.WM_ACTIVATE)
            {
                AppbarActivate();
            }
            else if (m.Msg == (int)WM.WM_WINDOWPOSCHANGED)
            {
                AppbarWindowPosChanged();
            }
            else if (m.Msg == (int)WM.WM_NCHITTEST)
            {
                OnNcHitTest(ref m);
                return;
            }
        }

        base.WndProc(ref m);
    }
    #endregion Window Procedure

    #region Overrides

    /// <summary>
    /// Overwrites the virtual method of the predecessor, to provide additional initialization.
    /// </summary>
    /// <param name="args">An EventArgs that contains the event data.</param>
    protected override void OnLoad(EventArgs args)
    {
        _PrevSize = Size;
        _PrevLocation = Location;
        base.OnLoad(args);
    }

    /// <summary>
    /// Overwrites the virtual method of the predecessor, to provide custom processing.
    /// </summary>
    /// <param name="args">Provides data for a cancel able event. </param>
    protected override void OnClosing(CancelEventArgs args)
    {
        AppbarRemove();
        base.OnClosing(args);
    }

    /// <summary>
    /// Overwrites the virtual method of the predecessor, to provide additional processing (custom resizing).
    /// </summary>
    /// <param name="args">An EventArgs that contains the event data.</param>
    protected override void OnSizeChanged(EventArgs args)
    {
        if (IsAppbarMode)
        {
            if (this.Edge == ShellApi.AppBarEdges.Top || this.Edge == ShellApi.AppBarEdges.Bottom)
                _PrevSize.Height = Size.Height;
            else
                _PrevSize.Width = Size.Width;

            SizeAppBar();
        }

        base.OnSizeChanged(args);
    }
    #endregion // Overrides

    #region Message Handlers

    /// <summary>
    /// Auxiliary method called by <see cref="WndProc"/>
    /// </summary>
    /// <param name="msg">  [in,out] The Message structure which wraps Win32 messages that Windows sends. </param>
    protected void OnAppbarNotification(ref Message msg)
    {
        AppBarStates state;
        AppBarNotifications msgType = (AppBarNotifications)(int)msg.WParam;

        switch (msgType)
        {
            case AppBarNotifications.PosChanged:
                SizeAppBar();
                break;

            case AppBarNotifications.StateChange:
                state = AppbarGetTaskbarState();
                DoChangeTaskbarState(state);
                break;

            case AppBarNotifications.FullScreenApp:
                if ((int)msg.LParam != 0)
                {
                    TopMost = false;
                    SendToBack();
                }
                else
                {
                    state = AppbarGetTaskbarState();
                    if ((state & AppBarStates.AlwaysOnTop) != 0)
                    {
                        TopMost = true;
                        BringToFront();
                    }
                    else
                    {
                        TopMost = false;
                        SendToBack();
                    }
                }

                break;

            case AppBarNotifications.WindowArrange:
                if ((int)msg.LParam != 0)   // before
                    Visible = false;
                else                        // after
                    Visible = true;

                break;
        }
    }

    /// <summary>
    /// Implementation helper being called from <see cref="WndProc"/> for case WM_NCHITTEST,
    /// to provide custom processing.
    /// </summary>
    /// <param name="msg">  [in,out] The Message structure which wraps Win32 messages that Windows sends. </param>
    protected void OnNcHitTest(ref Message msg)
    {
        DefWndProc(ref msg);

        if ((this.Edge == ShellApi.AppBarEdges.Top) && ((int)msg.Result == (int)Win32.MousePositionCode.HTBOTTOM))
            0.ToString(CultureInfo.InvariantCulture);
        else if ((this.Edge == ShellApi.AppBarEdges.Bottom) && ((int)msg.Result == (int)Win32.MousePositionCode.HTTOP))
            0.ToString(CultureInfo.InvariantCulture);
        else if ((this.Edge == ShellApi.AppBarEdges.Left) && ((int)msg.Result == (int)Win32.MousePositionCode.HTRIGHT))
            0.ToString(CultureInfo.InvariantCulture);
        else if ((this.Edge == ShellApi.AppBarEdges.Right) && ((int)msg.Result == (int)Win32.MousePositionCode.HTLEFT))
            0.ToString(CultureInfo.InvariantCulture);
        else if ((int)msg.Result == (int)Win32.MousePositionCode.HTCLOSE)
            0.ToString(CultureInfo.InvariantCulture);
        else
        {
            msg.Result = (IntPtr)Win32.MousePositionCode.HTCLIENT;
            return;
        }
        base.WndProc(ref msg);
    }
    #endregion // Message Handlers
    #endregion // Protected Methods

    #region Private_methods

    #region General_private

    private static uint RegisterCallbackMessage()
    {
        string uniqueMessageString = Guid.NewGuid().ToString();
        return ShellApi.RegisterWindowMessage(uniqueMessageString);
    }

    private void SizeAppBar()
    {
        ShellApi.RECT rt = new();

        if ((this.Edge == ShellApi.AppBarEdges.Left) || (this.Edge == ShellApi.AppBarEdges.Right))
        {
            rt.top = 0;
            rt.bottom = SystemInformation.PrimaryMonitorSize.Height;
            if (this.Edge == ShellApi.AppBarEdges.Left)
            {
                rt.right = _PrevSize.Width;
            }
            else
            {
                rt.right = SystemInformation.PrimaryMonitorSize.Width;
                rt.left = rt.right - _PrevSize.Width;
            }
        }
        else
        {
            rt.left = 0;
            rt.right = SystemInformation.PrimaryMonitorSize.Width;
            if (_Edge == ShellApi.AppBarEdges.Top)
            {
                rt.bottom = _PrevSize.Height;
            }
            else
            {
                rt.bottom = SystemInformation.PrimaryMonitorSize.Height;
                rt.top = rt.bottom - _PrevSize.Height;
            }
        }

        AppbarQueryPos(ref rt);

        switch (this.Edge)
        {
            case ShellApi.AppBarEdges.Left:
                rt.right = rt.left + _PrevSize.Width;
                break;
            case ShellApi.AppBarEdges.Right:
                rt.left = rt.right - _PrevSize.Width;
                break;
            case ShellApi.AppBarEdges.Top:
                rt.bottom = rt.top + _PrevSize.Height;
                break;
            case ShellApi.AppBarEdges.Bottom:
                rt.top = rt.bottom - _PrevSize.Height;
                break;
        }

        AppbarSetPos(ref rt);

        Location = new Point(rt.left, rt.top);
        Size = new Size(rt.right - rt.left, rt.bottom - rt.top);
    }
    #endregion // General_private

    #region AppBar Functions

    private void DoChangeTaskbarState(AppBarStates state)
    {
        if ((state & AppBarStates.AlwaysOnTop) != 0)
        {
            TopMost = true;
            BringToFront();
        }
        else
        {
            TopMost = false;
            SendToBack();
        }
    }

    private bool AppbarNew()
    {
        if (CallbackMessageID == 0)
        {
            throw new InvalidOperationException("CallbackMessageID is 0");
        }

        if (IsAppbarMode)
        {
            return true;
        }

        _PrevSize = Size;
        _PrevLocation = Location;

        // prepare data structure of message
        ShellApi.APPBARDATA msgData = PrepareAppbarData();
        // install new appbar
        IntPtr retVal = ShellApi.SHAppBarMessage((uint)AppBarMessages.New, ref msgData);
        IsAppbarMode = (retVal != IntPtr.Zero);

        SizeAppBar();

        return IsAppbarMode;
    }

    private bool AppbarRemove()
    {
        // prepare data structure of message
        ShellApi.APPBARDATA msgData = PrepareAppbarData();
        // remove appbar
        IntPtr retVal = ShellApi.SHAppBarMessage((uint)AppBarMessages.Remove, ref msgData);

        IsAppbarMode = false;

        Size = _PrevSize;
        Location = _PrevLocation;

        return (retVal != IntPtr.Zero);
    }

    private void AppbarQueryPos(ref ShellApi.RECT appRect)
    {
        // prepare data structure of message, including the rectangle
        ShellApi.APPBARDATA msgData = PrepareAppbarData();
        msgData.rc = appRect;

        // query the position for the appbar
        ShellApi.SHAppBarMessage((uint)AppBarMessages.QueryPos, ref msgData);
        appRect = msgData.rc;
    }

    private void AppbarSetPos(ref ShellApi.RECT appRect)
    {
        // prepare data structure of message, including the rectangle
        ShellApi.APPBARDATA msgData = PrepareAppbarData();
        msgData.rc = appRect;

        // set the position for the appbar
        ShellApi.SHAppBarMessage((uint)AppBarMessages.SetPos, ref msgData);
        appRect = msgData.rc;
    }

    private void AppbarActivate()
    {
        // prepare data structure of message
        ShellApi.APPBARDATA msgData = PrepareAppbarData();

        // send activate to the system
        ShellApi.SHAppBarMessage((uint)AppBarMessages.Activate, ref msgData);
    }

    private void AppbarWindowPosChanged()
    {
        // prepare data structure of message
        ShellApi.APPBARDATA msgData = PrepareAppbarData();

        // send message WindowPosChangedto to the system
        ShellApi.SHAppBarMessage((uint)AppBarMessages.WindowPosChanged, ref msgData);
    }

#if NOTDEF
// not needed so far
private static void AppbarGetTaskbarPos(out ShellApi.RECT taskRect)
{
  // prepare data structure of message
  ShellApi.APPBARDATA msgData = new ShellApi.APPBARDATA();
  msgData.cbSize = (UInt32)Marshal.SizeOf(msgData);

  // get taskbar position
  ShellApi.SHAppBarMessage((UInt32)AppBarMessages.GetTaskBarPos, ref msgData);
  taskRect = msgData.rc;
}
#endif
    #endregion // AppBar Functions
    #endregion // Private_methods
    #endregion // Methods
}
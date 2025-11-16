// Ignore Spelling: Utils, Sel, Msec, Meth, kbs, ctrl, Coord, tooltip
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.MessageHooking;
using PK.PkUtils.SystemEx;
using PK.PkUtils.Utils;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.UI.TipHandlers;

#pragma warning disable IDE0079  // Remove unnecessary suppression
#pragma warning disable CA1859    // Change type of variable ...
#pragma warning disable IDE0290   // Use primary constructor

/// <summary>
/// TipHandler is base class of "tooltip-supporter engine." 
/// See derived classes like ListBoxTipHandler.
/// </summary>
[CLSCompliant(false)]
public abstract class TipHandler : WindowMessageHook
{
    #region Typedefs

    /// <summary>
    /// Auxiliary enum; its purpose is to declare ItemNumber.cNoItem constant.
    /// </summary>
    public enum ItemNumber : uint
    {
        /// <summary> An enum constant representing the 'no item' subject. </summary>
        cNoItem = uint.MaxValue
    };

    /// <summary>
    /// Auxiliary enum; its purpose is to declare SubItemNumber.cNoSubItem constant.
    /// </summary>
    public enum SubItemNumber : uint
    {
        /// <summary> An enum constant representing the 'no sub item' subject. </summary>
        cNoSubItem = uint.MaxValue
    };

    /// <summary>
    /// TipHookMouse is specialized WindowsSystemHookMouse, overriding its MouseHookMeth
    /// </summary>
    protected class TipHookMouse : WindowsSystemHookMouse
    {
        /// <summary> Default constructor. </summary>
        protected internal TipHookMouse()
          : base()
        { }

        /// <summary>
        /// Overrides a method of the base class to specialize the behaviour
        /// </summary>
        /// <param name="m">The structure used by <see cref="Win32.HookType.WH_MOUSE"/> system hook.</param>
        /// <returns>  True if the message has been completely handled by this hook instance, 
        ///            and the caller should NOT proceed passing it to other hooks in the chain; 
        ///            false otherwise. Use 'true' value with care! </returns>
        protected override bool MouseHookMeth(Win32.MOUSEHOOKSTRUCT m)
        {
            if (TipHandler.ListAllHandlers != null)
            {
                Point ptScreen = new(m.pt.x, m.pt.y);

                foreach (TipHandler pTip in TipHandler.ListAllHandlers)
                {
                    if (pTip.RequiresSystemMouseHookCall &&
                        (pTip.HookedHWND != IntPtr.Zero) &&
                        User32.IsWindowVisible(pTip.HookedHWND))
                    {
                        pTip.OnMouseMove(ptScreen);
                    }
                }
            }

            return false;
        }
    }

    /// <summary>
    /// TipHookKbLL is specialized WindowsSystemHookKbLL, overriding its KeyboardLLHookMeth
    /// </summary>
    protected class TipHookKbLL : WindowsSystemHookKbLL
    {
        /// <summary> Default constructor. </summary>
        protected internal TipHookKbLL()
          : base()
        { }

        /// <summary>
        /// A virtual method called by a delegate CoreKeyboardLLProc. 
        /// In a derived class you will overwrite this method.
        /// </summary>
        /// <param name="kbs">
        /// The structure used by <see cref="Win32.HookType.WH_KEYBOARD_LL"/> system hook.
        /// </param>
        /// <returns>  True if the message has been completely handled by this hook instance, 
        ///            and the caller should NOT proceed passing it to other hooks in the chain; 
        ///            false otherwise. Use 'true' value with care! </returns>
        protected override bool KeyboardLLHookMeth(Win32.KBDLLHOOKSTRUCT kbs)
        {
            // if the user pressed Alt+Tab, cancel any tooltip
            if (kbs.vkCode == (uint)Win32.VK.VK_TAB)
            {
                if ((kbs.flags & Win32.LLKHF_ALTDOWN) != 0)
                {
                    TipHandler.ListAllHandlers.SafeForEach(pTip =>
                    {
                        if (pTip.IsTipWindowVisible())
                        {
                            pTip.CancelTipWindow();
                        }
                    });
                }
            }

            return false;
        }
    }
    #endregion // Typedefs

    #region Fields

    /// <summary>
    /// global variable; draw highlighted text by different color when true
    /// </summary>
    private static bool g_bDrawSelHighlighted = true;

    /// <summary>
    /// global variable: how many milliseconds wait before showing tip
    /// </summary>
    private static uint g_nTipTimeMsec = 200; // .2 sec

    /// <summary>
    /// global variable; when nonzero, indicates how soon should be tooltip re-displayed
    /// after it is canceled by mouse click
    /// </summary>
    private static uint g_nTipTimeAfterMouse = 1000; // 1 sec

    /// <summary>
    /// the tip window
    /// </summary>
    protected IPopupText _wndTip;

    /// <summary>
    /// index of current item
    /// </summary>
    protected uint _nCurItem = (uint)ItemNumber.cNoItem;

    /// <summary>
    /// Says whether the member requires the call of OnMouseMove 
    /// from the system hook function.
    /// By default is false, the derived class can change it in its constructor.
    /// </summary>
    protected readonly bool _requiresSystemMouseHookCall;

    /// <summary> A backing field of <see cref="TipTimeDelayMsec"/> </summary>
    private readonly uint _tipTimeDelayMsec;

    /// <summary>
    /// all instances (used for enumeration in system-hook MouseProc ).
    /// </summary>
    private static List<TipHandler> _listAllHandlers;

    /// <summary>
    ///  The system hook of type WH_MOUSE
    /// </summary>
    private static TipHookMouse _mouseHook;

    /// <summary>
    /// The system hook of type WH_KEYBOARD_LL
    /// </summary>
    private static TipHookKbLL _keyboardHook;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// The default constructor ( should be protected in abstract class )
    /// </summary>
    protected TipHandler()
        : this(false)
    { }

    /// <summary> Single-argument constructor, should be protected in abstract class. </summary>
    /// <param name="requiresSystemMouseHookCall">  Provides initial value of <see cref="RequiresSystemMouseHookCall"/>. </param>
    protected TipHandler(bool requiresSystemMouseHookCall)
        : this(requiresSystemMouseHookCall, g_nTipTimeMsec)
    { }

    /// <summary> Single-argument constructor, should be protected in abstract class. </summary>
    /// <param name="tipTimeDelayMsec">  Provides initial value of <see cref="TipTimeDelayMsec"/>. </param>
    protected TipHandler(uint tipTimeDelayMsec)
        : this(false, tipTimeDelayMsec)
    { }

    /// <summary> Two-arguments constructor, should be protected in abstract class. </summary>
    /// <param name="requiresSystemMouseHookCall">  Provides initial value of <see cref="RequiresSystemMouseHookCall"/>. </param>
    /// <param name="tipTimeDelayMsec">  Provides initial value of <see cref="TipTimeDelayMsec"/>. </param>
    protected TipHandler(bool requiresSystemMouseHookCall, uint tipTimeDelayMsec)
    {
        _requiresSystemMouseHookCall = requiresSystemMouseHookCall;
        _tipTimeDelayMsec = tipTimeDelayMsec;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Draw highlighted text by different color when true
    /// </summary>
    public static bool DrawSelHighlighted
    {
        get { return g_bDrawSelHighlighted; }
        set { g_bDrawSelHighlighted = value; }
    }

    /// <summary>
    /// how many milliseconds wait before showing tip
    /// </summary>
    public static uint TipTimeMsec
    {
        get { return g_nTipTimeMsec; }
        set { g_nTipTimeMsec = value; }
    }

    /// <summary>
    /// Property affecting a time period how soon is tooltip re-displayed after canceling.<br/>
    /// If the tooltip is displayed and cancelled by mouse-clicking on the particular item, and if the mouse does
    /// not move afterwards, the tooltip will be re-displayed after TipTimeAfterMouse milliseconds.
    /// </summary>
    public virtual uint TipTimeAfterMouse
    {
        get { return g_nTipTimeAfterMouse; }
        set { g_nTipTimeAfterMouse = value; }
    }

    /// <summary>
    /// Get the tip window
    /// </summary>
    protected IPopupText TipWindow
    {
        get => _wndTip;
    }

    /// <summary>
    /// The delay time in milliseconds that is used when showing the tooltip by calling
    /// <see cref="IPopupText.ShowDelayed(int)"/>.
    /// </summary>
    ///
    /// <returns> An amount of milliseconds before the tooltip window is shown. </returns>
    /// <seealso cref="SetTipTimeMsec"/>
    protected virtual uint TipTimeDelayMsec
    {
        get => _tipTimeDelayMsec;
    }

    /// <summary>
    /// Get the value of _bRequiresSystemMouseHookCall, as initialized by constructor.
    /// </summary>
    protected bool RequiresSystemMouseHookCall
    {
        get { return _requiresSystemMouseHookCall; }
    }

    private static IReadOnlyList<TipHandler> ListAllHandlers { get => _listAllHandlers; }

    #endregion // Properties

    #region Methods

    #region Public Methods

    /// <summary> Overwrites the implementation of the base class to subclass a window. </summary>
    /// <param name="hwnd"> The handle of the hooked control. </param>
    /// <returns>
    /// True on success, false on failure.  <br/>
    /// Returns false if the given handle is not valid window handle, or if this hook is hooked-up already to something.
    /// </returns>
    public override bool HookWindow(IntPtr hwnd)
    {
        bool bWasHooked = IsHooked;
        bool result;

        if (hwnd != IntPtr.Zero)
        {   // the caller wants to hook something
            result = base.HookWindow(hwnd);
            if (IsHooked && !bWasHooked)
            {   // update the member variable
                OnHookup(hwnd);
            }
        }
        else
        {   // the caller wants to unhook 
            result = base.HookWindow(IntPtr.Zero);
            if ((!IsHooked) && bWasHooked)
            {   // update the member variable
                OnUnhook(this.HookedHWND);
            }
        }
        return result;
    }

    /// <summary>
    /// Hooks the given IWin32Window0derived object, by calling base.HookWindow(win32Window.Handle).
    /// To unhook already created hook, you should call HookWindow(null).
    /// </summary>
    /// <param name="win32Window"> An object that exposes Win32 HWND handle.</param>
    /// <returns>True on success, false on failure.</returns>
    public virtual bool HookControl(IWin32Window win32Window)
    {
        IntPtr hwnd = (win32Window != null) ? win32Window.Handle : IntPtr.Zero;
        bool result = HookWindow(hwnd);

        return result;
    }

    /// <summary> Initializes the TipHandler - installs the hook, and creates the tip window. <br/>
    /// For reverting this action, call <see cref="Done"/>.
    /// </summary>
    ///
    /// <param name="ctrl">  The WinForms control for which tooltips are  displayed- listbox, or ComboBox etc.
    /// Must NOT equal to null. </param>
    /// <param name="pFont"> The font of new tooltip window. May be null; in that case the font of its parent
    /// window ( the hooked control ) will be used. </param>
    public void Init(Control ctrl, Font pFont)
    {
        Debug.Assert(ctrl != null);
        IWin32Window win32Window = ctrl;

        HookControl(win32Window);
        if (IsHooked)
        {   // create tip window ( invisible for now )
            CreateTipWindow(pFont);
        }
    }

    /// <summary> Uninstall the hook. <br/> Reverting action is <see cref="Init"/>. </summary>
    public void Done()
    {
        HookControl(null);
    }

    /// <summary>
    /// Set the font of the tip window. If the argument is null, 
    /// tries to retrieve the font from hooked control and use that one.
    /// </summary>
    /// <param name="pFont">The System.Drawing.Fond object, or null.</param>
    public void SetTipWindowFont(Font pFont)
    {
        Debug.Assert(IsHooked); // needs to be already initialized

        if (null == pFont)
        {
            Control ctrl;
            IntPtr hFont = User32.SendMessage(this.HookedHWND, (int)Win32.WM.WM_GETFONT, IntPtr.Zero, IntPtr.Zero);

            if (hFont != IntPtr.Zero)
            {
                pFont = Font.FromHfont(hFont);
            }
            else if (null != (ctrl = Control.FromHandle(this.HookedHWND)))
            {
                pFont = ctrl.Font;
            }
        }
        if (pFont != null)
        {
            TipWindow.Font = pFont;
        }
    }

    /// <summary>
    /// Get the font of tooltip window ( if there is any )
    /// </summary>
    /// <returns>The font of current IPopupText tooltip window, if there is any, or null.</returns>
    public Font GetTipWindowFont()
    {
        Font result = null;

        if (TipWindow != null)
        {
            result = TipWindow.Font;
        }
        return result;
    }

    /// <summary>
    /// Give me the hooked control
    /// </summary>
    /// <returns>The currently hooked window, casted to WinForms control.</returns>
    public Control HookedControl()
    {
        return Control.FromHandle(this.HookedHWND);
    }

    /// <summary> Returns the index of current item. </summary>
    /// <returns>   The index of currently selected item, or ItemNumber.cNoItem if no item is selected. </returns>
    public uint GetCurItem()
    {
        return _nCurItem;
    }

    /// <summary>
    /// Is the tip window visible ?
    /// </summary>
    /// <returns>True if visible, false if not.</returns>
    public bool IsTipWindowVisible()
    {
        return (TipWindow != null) && (TipWindow.IsVisible);
    }

    /// <summary>
    /// Cancel the tip window if there is any, and set the current item to ItemNumber.cNoItem
    /// </summary>
    public virtual void CancelTipWindow()
    {
        TipWindow?.Cancel();
        _nCurItem = (uint)ItemNumber.cNoItem;
    }

    /// <summary>
    /// Abstract method you have to overwrite. 
    /// Is called by the "tooltip engine" when it needs to determine current item and sub-item under the mouse.<br/>
    /// Point <paramref name="ptClient"/> is in client coordinates.
    /// </summary>
    /// <param name="ptClient"> The input point, representing the position of the mouse. (Is in client coordinates).</param>
    /// <param name="nSubItem">The output argument returns the index of found sub-item, or SubItemNumber.cNoSubItem if no sub-item found.</param>
    /// <returns>The index of found item for the particular point, or ItemNumber.cNoItem if no item is found.</returns>
    public abstract uint ItemFromPoint(Point ptClient, out uint nSubItem);

    /// <summary> Gets specific information ( rectangle, text ) for given tooltip item. . </summary>
    ///
    /// <param name="nItem"> The item about which the information is required. </param>
    /// <param name="nSubItem"> The sub-item of the particular item, about which the information is required.  </param>
    /// <param name="bFontInControl"> Either we are computing the item's rectangle in the control (so bFontInControl == true), or we are
    /// computing the size of rectangle of the final tooltip (so bFontInControl == false). </param>
    /// <param name="rc"> [out] The resulting rectangle of the item, respective of the tooltip ( see <paramref name="bFontInControl"/>). </param>
    /// <param name="s">  [out] The text being displayed for the particular item. </param>
    public abstract void GetItemInfo(
        uint nItem,
        uint nSubItem,
        bool bFontInControl,
        out Rectangle rc,
        out string s);

    #endregion // Public Methods

    #region Protected Methods

    #region Protected Overrides

    /// <summary>
    /// Overrides of the base class method to install two needed system hooks
    /// </summary>
    /// <param name="pExtraInfo"> The object providing additional information; 
    /// in this case the caller provides in this argument the hooked window itself. </param>
    protected override void OnHookup(object pExtraInfo)
    {
        if (pExtraInfo != null)
        {  // having the extra information about the control, ad me to the global array
            AddToList();
            if (ListAllHandlers.Count == 1)
            {
                SetMouseHook();
                SetLLKeyboardHookHook();
            }
        }
    }

    /// <summary>
    /// Override of the base class method to get rid of system hooks if they are no longer needed
    /// </summary>
    /// <param name="pExtraInfo">  The object providing additional information;
    /// in this case the caller <see cref="WindowMessageHook"/>
    /// provides in this argument the hooked window itself. </param>
    protected override void OnUnhook(object pExtraInfo)
    {
        if (pExtraInfo != null)
        {
            if (ListAllHandlers != null)
            {
                if (ListAllHandlers.Count == 1)
                {
                    UnHookMouseHook();
                    UnHookLLKeyboardHookHook();
                }
                RemoveFromList();
            }
        }
    }

    /// <summary>
    /// Subclasses window procedure; overwrites the base class behavior.
    /// </summary>
    /// <param name="m">Processed Windows Message object.</param>
    protected override void HookWindowProc(ref Message m)
    {
        bool bWasCanceled = false;

        // preprocess the message
        switch (m.Msg)
        {
            case (int)Win32.WM.WM_LBUTTONDOWN:
                if (TipWindow != null)
                {
                    // Cancel tooltip text if any.  
                    // Should be regardless on TipWindow.IsVisible property value - needs to cancel also if displayed with delay
                    CancelTipWindow();
                    bWasCanceled = true;
                }
                break;

            case (int)Win32.WM.WM_MOUSELEAVE:
                CancelTipWindow(); // cancel popup text if any
                break;


            case (int)Win32.WM.WM_GETDLGCODE:  // this is some of the messages which we get when property page is switched
                if (User32.IsWindowVisible(this.HookedHWND))
                {
                    // Cancel that window too; to prevent displaying the tooltip for window 
                    // that belongs to property page currently not displayed.
                    CancelTipWindow();
                }
                break;
        }

        // process the message
        base.HookWindowProc(ref m);

        // post-process the message
        switch (m.Msg)
        {
            case (int)Win32.WM.WM_LBUTTONDOWN:
                if (bWasCanceled && (0 != TipTimeAfterMouse))
                {   // Show the tooltip again with timeout TipTimeAfterMouse milliseconds 
                    // Must do AFTER base.HookWindowProc has been called,
                    // as it allows the control to change its selection status
                    //
                    // Client-relative mouse coordinates are in LParam
                    Point pt = new(Win32.GET_X_LPARAM((int)m.LParam), Win32.GET_Y_LPARAM((int)m.LParam));

                    this.InvokeToShowTooltip(pt, TipTimeAfterMouse);
                }
                break;

            case (int)Win32.WM.WM_MOUSEMOVE:
                {
                    Control ctrl;
                    Point ptScreen;
                    // (client relative) mouse coordinates are in LParam
                    Point pt = new(Win32.GET_X_LPARAM((int)m.LParam), Win32.GET_Y_LPARAM((int)m.LParam));

                    // compute the screen coordinates
                    if (null != (ctrl = HookedControl()))
                    {
                        ptScreen = ctrl.PointToScreen(pt);
                    }
                    else
                    {
                        ptScreen = pt;
                        ClientToScreen(ref ptScreen);
                    }
                    OnMouseMove(ptScreen);
                }
                break;
        }
    }
    #endregion // Protected Overrides

    #region Protected Virtual Methods

    /// <summary> 
    /// Virtual method getting the parent window for TipWindow ( which will be created as its child).<br/>
    /// Derived class can overwrite.
    /// </summary>
    ///
    /// <returns> The parent window for child tip control creation. </returns>
    protected virtual Control GetParentForTipCreation()
    {
        Control pParent = null;

        if (IsHooked)
        {
            pParent = HookedControl();
        }
        return pParent;
    }

    /// <summary>
    /// Virtual method creating the (tool)tip window. <br/>
    /// Override that method when you want to use tip window different from CPopupText.
    /// </summary>
    /// <param name="pFont">The font of new tooltip window.
    /// May be null; in that case the font of its parent window ( the hooked control ) will be used.
    /// </param>
    /// <returns>True on success, false on failure.</returns>
    protected virtual bool CreateTipWindow(Font pFont)
    {
        bool result = true;

        if (null == TipWindow)
        {
            Control pParent = GetParentForTipCreation();
            if (pParent != null)
            {
                _wndTip = new PopupTooltip();
                SetTipWindowFont(pFont);
            }
            else
            {
                /* throw new ApplicationException("TipHandler error - cannot hook IWin32Window which is not a Control"); */
                result = false;
            }
        }

        return result;
    }

    /// <summary>
    /// virtual method destroying the tip window. 
    /// </summary>
    protected virtual void DestroyTipWindow()
    {
        Disposer.SafeDispose(ref _wndTip);
    }

    /// <summary>
    /// Determines whether given rectangle is completely visible within the hooked control ( listbox etc.).
    /// Is a virtual method you can overwrite.
    /// </summary>
    /// <param name="rc">A checked rectangle (might be previously retrieved by calling <see cref="OnGetItemInfo"/>).</param>
    /// <returns>True if the rectangle is completely visible; false otherwise.</returns>
    protected virtual bool IsRectCompletelyVisible(Rectangle rc)
    {
        Control ctrl = HookedControl();
        Rectangle rcClient = ctrl.ClientRectangle;

        // Need to compare rc.Right, not rc.Width(),
        // because of the cases like CCheckListBox, where the rectangle left is nonzero, 
        // and therefore the rc.right != rc.Width();
        return rcClient.Width > rc.Right;
    }

    /// <summary>
    /// Sets the delay time in milliseconds value that can be later retrieved by <see cref="TipTimeDelayMsec"/>.
    /// </summary>
    /// <param name="ms">The delay time in milliseconds</param>
    protected virtual void SetTipTimeMsec(uint ms)
    {
        g_nTipTimeMsec = ms;
    }
    #endregion // Protected Virtual Methods

    #region Protected Abstract Methods

    /// <summary> Gets item text. </summary>
    ///
    /// <param name="nItem"> The item about which the information is required. </param>
    /// <param name="nSubItem"> The sub-item of the particular item, about which the information is required.  </param>
    /// <returns>   The item text. </returns>
    protected abstract string GetItemText(uint nItem, uint nSubItem);

    /// <summary>
    /// Abstract method you need to overwrite. 
    /// Is called by the "tooltip engine" when mouse moves over the hooked control.<br/>
    /// Point ptInScreen is in screen coordinates.
    /// </summary>
    /// <param name="ptInScreen">Screen coordinates of mouse position.</param>
    protected abstract void OnMouseMove(Point ptInScreen);

    #endregion // Protected Abstract Methods

    #region Protected Helpers

    /// <summary> Determine if the system has left and right mouse buttons swapped. </summary>
    /// <returns>  True if mouse buttons swapped, false if not. </returns>
    protected static bool AreMouseButtonsSwapped()
    {
        int metrics = User32.GetSystemMetrics(Win32.SM.SM_SWAPBUTTON);
        return (metrics != 0);
    }

    /// <summary> Query the (logical) left mouse button is down. </summary>
    /// <returns> True if left mouse button down, false if not. </returns>
    protected static bool IsLeftMouseButtonDown()
    {
        Win32.VK buttonKey = (!AreMouseButtonsSwapped()) ? Win32.VK.VK_LBUTTON : Win32.VK.VK_RBUTTON;
        int state = User32.GetKeyState((int)buttonKey);

        return (state < 0);
    }

    /// <summary> Query the (logical) right mouse button is down. </summary>
    /// <returns> True if right mouse button down, false if not. </returns>
    protected static bool IsRightMouseButtonDown()
    {
        Win32.VK buttonKey = (!AreMouseButtonsSwapped()) ? Win32.VK.VK_RBUTTON : Win32.VK.VK_LBUTTON;
        int state = User32.GetKeyState((int)buttonKey);

        return (state < 0);
    }

    /// <summary>
    /// Given the point in client coordinates it returns related item, sub-item, rectangle and text.
    /// Virtual method you may need to override in derived classes.
    /// </summary>
    /// 
    /// <param name="ptClient"> Input argument. Position of mouse (in client's coordinate)
    /// </param>
    /// 
    /// <param name="bFontInControl"> Input argument. 
    /// Either we are computing the item's rectangle in the control (so bFontInControl == true),
    /// or we are computing the size of rectangle of the final tooltip (so bFontInControl == false). <br/>
    /// ( As the font in the control and font in the tooltip may be different,
    /// the resulting rectangle of the tooltip might have different size, 
    /// so the method needs to know about what rectangle we are asking.)
    /// </param>
    /// 
    /// <param name="nSubItem">Subitem of the item (will be used for the case of ListCtrl tooltips, where subitem == column.
    /// But in most cases (ListBox, ComboBox) it will be always simply zero. 
    /// </param>
    /// 
    /// <param name="rc">
    /// The resulting rectangle of the item, respective of the tooltip ( see bFontInControl )
    /// </param>
    /// 
    /// <param name="s"> 
    /// The text of the item, respective of the tooltip ( see bFontInControl )
    /// </param>
    ///
    /// <returns>The index of found item, or ItemNumber.cNoItem if no item is found.</returns>
    protected virtual uint OnGetItemInfo(
        Point ptClient,
        bool bFontInControl,
        out uint nSubItem,
        out Rectangle rc,
        out string s)
    {
        uint nItem;

        // initialize empty values
        rc = Rectangle.Empty;
        s = string.Empty;

        // compute
        nItem = ItemFromPoint(ptClient, out nSubItem);
        if ((nItem != (uint)ItemNumber.cNoItem) && (nSubItem != (uint)SubItemNumber.cNoSubItem))
        {
            GetItemInfo(nItem, nSubItem, bFontInControl, out rc, out s);
            return nItem;
        }

        return (uint)ItemNumber.cNoItem;
    }

    /// <summary> Call this method to cause showing tooltip at given point with given delay. </summary>
    ///
    /// <param name="ptClientCoord"> Coordinates related to client area of hooked control. <br/>
    /// ( The origin for client coordinates is the upper-left corner of the client area.)
    /// </param>
    /// <param name="ms"> The delay in milliseconds for showing tooltip. No (zero) delay means show now. </param>
    protected void InvokeToShowTooltip(Point ptClientCoord, uint ms)
    {
        // store the old delay value
        uint nPrevDelay = this.TipTimeDelayMsec;
        Point ptScreen;
        Control ctrl;

        // compute the screen coordinates
        if (null != (ctrl = HookedControl()))
        {
            ptScreen = ctrl.PointToScreen(ptClientCoord);
        }
        else
        {
            ptScreen = ptClientCoord;
            ClientToScreen(ref ptScreen);
        }

        // set temporary the new delay 
        SetTipTimeMsec(ms);
        // set the _nCurItem to invalid, so the OnMouseMove will show the computed new tip
        _nCurItem = (uint)ItemNumber.cNoItem;
        // now pretend the mouse has been moved
        OnMouseMove(ptScreen);

        // restore the original delay
        this.SetTipTimeMsec(nPrevDelay);
    }
    #endregion // Protected Helpers
    #endregion // Protected Methods

    #region Private Methods

    /// <summary>
    /// implementation helper, adds me to global list of all instances
    /// </summary>
    private void AddToList()
    {
        if (ListAllHandlers == null)
        {
            _listAllHandlers = [];
        }
        if (ListAllHandlers.IndexOf(this) < 0)
        {
            _listAllHandlers.Add(this);
        }
    }

    /// <summary>
    /// implementation helper, removes me from global list of all instances
    /// </summary>
    private void RemoveFromList()
    {
        if (ListAllHandlers != null)
        {
            if (ListAllHandlers.IndexOf(this) >= 0)
            {
                _listAllHandlers.Remove(this);
                if (ListAllHandlers.Count == 0)
                {
                    _listAllHandlers = null;
                }
            }
        }
    }

    /// <summary>
    /// implementation helper, set the mouse hook
    /// </summary>
    /// <returns></returns>
    private static bool SetMouseHook()
    {
        bool result;

        _mouseHook ??= new TipHookMouse();
        if (!(result = _mouseHook.IsInstalled))
        {
            _mouseHook.Install();
            result = _mouseHook.IsInstalled;
        }

        return result;
    }

    /// <summary>
    /// implementation helper, uninstall the mouse hook
    /// </summary>
    private static void UnHookMouseHook()
    {
        if (_mouseHook != null)
        {
            _mouseHook.Uninstall();
            _mouseHook = null;
        }
    }

    /// <summary>
    /// implementation helper, installs the low level keyboard hook
    /// </summary>
    /// <returns></returns>
    private static bool SetLLKeyboardHookHook()
    {
        bool result;

        _keyboardHook ??= new TipHookKbLL();
        if (!(result = _keyboardHook.IsInstalled))
        {
            _keyboardHook.Install();
            result = _keyboardHook.IsInstalled;
        }

        return result;
    }

    /// <summary>
    /// implementation helper, will uninstall the low-level keyboard hook
    /// </summary>
    private static void UnHookLLKeyboardHookHook()
    {
        if (_keyboardHook != null)
        {
            _keyboardHook.Uninstall();
            _keyboardHook = null;
        }
    }
    #endregion // Private Methods
    #endregion // Methods
}

#pragma warning restore IDE0290 // Use primary constructor
#pragma warning restore CA1859    // Change type of variable ...
#pragma warning restore IDE0079
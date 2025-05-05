// Ignore Spelling: listbox, Msec, popup, preprocess, tooltip, tooltips, Utils
//
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PK.PkUtils.MessageHooking;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.UI.TipHandlers;

/// <summary>   Supports tooltips for ComboBoxes. </summary>
[CLSCompliant(false)]
public class ComboBoxTipHandler : TipHandler
{
    #region Typedefs

    /// <summary>
    /// the internal hook of the listbox-part of the ComboBox
    /// </summary>
    internal class CInternalListHook : Win32WindowHook
    {
        protected ComboBoxTipHandler _comboTipHandler;
        private bool _isScrolling;
        private uint _nLastScrollItem;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="comboTipHandler"></param>
        internal CInternalListHook(ComboBoxTipHandler comboTipHandler)
        {
            _comboTipHandler = comboTipHandler;
            _isScrolling = false;
            _nLastScrollItem = (uint)ItemNumber.cNoItem;
        }

        /// <summary>
        /// Get the hooked window listbox 
        /// </summary>
        /// <returns>Win32 window handle.</returns>
        internal IntPtr GetListBox()
        {
            IntPtr result = this.HookedHWND;
            return result;
        }

        /// <summary>
        /// is the ListBox scrolling ?
        /// </summary>
        /// <returns>True if scrolling, false if not.</returns>
        internal bool IsScrolling
        {
            get => _isScrolling;
        }

        /// <summary>
        /// implementation helper, called from <see cref="HookWindowProc"/>
        /// </summary>
        protected internal void StartRecordScrolling()
        {
            if (!IsScrolling)
            {
                _isScrolling = true;
                _nLastScrollItem = (uint)ItemNumber.cNoItem;
            }
        }

        /// <summary>
        /// implementation helper, called from <see cref="HookWindowProc"/>
        /// </summary>
        protected internal void EndRecordScrolling()
        {
            if (IsScrolling)
            {
                _isScrolling = false;
            }
        }

        protected internal uint GetLastScrollItem()
        {
            return _nLastScrollItem;
        }

        protected internal void SetLastScrollItem(uint nVal)
        {
            _nLastScrollItem = nVal;
        }

        [DllImport("user32", EntryPoint = "SendMessage")]
        public static extern IntPtr ListBox_GetItemRectAlias(IntPtr hWnd, int Msg, IntPtr wParam, ref User32.RECT lpRect);


        public static void ListBox_GetItemRect(IntPtr hListBx, int nItem, out User32.RECT lpRect)
        {
            const int LB_GETITEMRECT = 0x0198;
            lpRect.top = lpRect.bottom = lpRect.left = lpRect.right = 0;
            ListBox_GetItemRectAlias(hListBx,
                LB_GETITEMRECT, nItem, ref lpRect);
        }

        /// <summary>
        /// Overwrites the implementation of the base class in order to perform different message processing.
        /// </summary>
        /// <param name="m"> The Message structure which wraps Win32 messages that Windows sends. </param>
        protected override void HookWindowProc(ref Message m)
        {
            int nScrollCode = -1; // initialize invalid value
            bool bWasCanceled = false;
            IPopupText pPopWnd = null;

            // preprocess message
            switch (m.Msg)
            {
                case (int)Win32.WM.WM_LBUTTONDOWN:
                    Debug.Assert(_comboTipHandler != null);
                    if (_comboTipHandler.IsTipWindowVisible())
                    {
                        _comboTipHandler.CancelTipWindow();  // cancel popup text
                        bWasCanceled = true;
                    }
                    break;

                case (int)Win32.WM.WM_VSCROLL:
                    {
                        // In case of CBS_SIMPLE, we don't receive any WM_LBUTTONDOWN (above),
                        // but directly just the scrolling message.
                        // So the popup text has to be cancelled only here.
                        if (_comboTipHandler.GetComboType() == (uint)Win32.ComboType.CBS_SIMPLE)
                            pPopWnd = _comboTipHandler.TipWindow;

                        switch (nScrollCode = Win32.LOWORD((int)m.WParam))
                        {
                            //  Ends scroll 
                            case (int)Win32.ScrollbarCommand.SB_ENDSCROLL:
                            //  The user has dragged the scroll box (thumb) and released the mouse button. 
                            //  The nPos parameter indicates the position of the scroll box at the end of the drag operation. 
                            case (int)Win32.ScrollbarCommand.SB_THUMBPOSITION:
                                EndRecordScrolling();
                                break;

                            case (int)Win32.ScrollbarCommand.SB_BOTTOM: // Scrolls to the lower right
                            case (int)Win32.ScrollbarCommand.SB_TOP: //  Scrolls to the upper left 
                            case (int)Win32.ScrollbarCommand.SB_LINEDOWN: //  Scrolls one line down 
                            case (int)Win32.ScrollbarCommand.SB_LINEUP: //  Scrolls one line up 
                            case (int)Win32.ScrollbarCommand.SB_PAGEDOWN: //  Scrolls one page down 
                            case (int)Win32.ScrollbarCommand.SB_PAGEUP: //  Scrolls one page up 
                            case (int)Win32.ScrollbarCommand.SB_THUMBTRACK: //  The user is dragging the scroll box. This message is sent repeatedly until the user releases the mouse button. The nPos parameter indicates the position that the scroll box has been dragged to. 
                                StartRecordScrolling();
                                pPopWnd?.Cancel();  // cancel popup text
                                break;
                        }
                    }
                    break;
            }

            // process the message 
            base.HookWindowProc(ref m);

            // post-process the message

            switch (m.Msg)
            {
                case (int)Win32.WM.WM_VSCROLL:
                    {
                        if (nScrollCode == (int)Win32.ScrollbarCommand.SB_ENDSCROLL)
                        {
                            uint nItem = GetLastScrollItem();
                            // first we found out the item, where was the mouse immediately before the scrolling stopped
                            if (nItem != (uint)ItemNumber.cNoItem)
                            {
                                IntPtr hList = GetListBox();

                                // Get the item rectangle
                                ListBox_GetItemRect(hList, (int)nItem, out User32.RECT rect);
                                // And by posting WM_MOUSEMOVE ensures that the item will be immediately selected;
                                // even when the mouse is still over the scroolbar...
                                User32.PostMessage(hList, (int)Win32.WM.WM_MOUSEMOVE, 0,
                                    (IntPtr)Win32.MAKELONG((ushort)(rect.left + 1), (ushort)(rect.top + 1)));
                            }
                        }

                    }
                    break;

                case (int)Win32.WM.WM_LBUTTONDOWN:
                    {
                        if (_comboTipHandler.GetComboType() == (uint)Win32.ComboType.CBS_SIMPLE)
                        {
                            if ((null != pPopWnd) && bWasCanceled)
                            {
                                if (0 != _comboTipHandler.TipTimeAfterMouse)
                                {
                                    User32.POINT ptComboPoint;
                                    User32.POINT ptListPoint = new()
                                    {
                                        // List Client-relative mouse coordinates are in LParam
                                        x = Win32.GET_X_LPARAM((int)m.LParam),
                                        y = Win32.GET_Y_LPARAM((int)m.LParam)
                                    };
                                    User32.ClientToScreen(this.HookedHWND, ref ptListPoint);
                                    ptComboPoint = ptListPoint;
                                    User32.ScreenToClient(_comboTipHandler.HookedHWND, ref ptComboPoint);

                                    uint nms = 2 * _comboTipHandler.TipTimeAfterMouse;
                                    Point pt = new(ptComboPoint.x, ptComboPoint.y);

                                    _comboTipHandler.InvokeToShowTooltip(pt, nms);
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
    #endregion // Typedefs

    #region Fields

    /// <summary>
    /// The variable for hook of the listbox-part of the ComboBox window
    /// </summary>
    private readonly CInternalListHook _lbHook;

    /// <summary>
    /// The hook of the edit control.
    /// We could use the simple HWND (aka IntPtr) here to keep the window handle, 
    /// however it is convenient to use Win32WindowHook-derived, as it is automatically sets 
    /// its hooked window handle to null on WM_NCDESTROY.
    /// </summary>
    private readonly WindowMessageHook _editHook;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public ComboBoxTipHandler()
        : this(g_nTipTimeMsec)
    { }

    /// <summary> Single-argument constructor. </summary>
    /// <param name="tipTimeDelayMsec">  Provides initial value of <see cref="TipTimeDelayMsec"/>. </param>
    public ComboBoxTipHandler(uint tipTimeDelayMsec)
        : base(true, tipTimeDelayMsec)
    {
        _lbHook = new CInternalListHook(this);
        _editHook = new WindowMessageHook();
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Overriding the property of the base class.
    /// For the ComboBox we want to show the tooltips faster...
    /// </summary>
    /// <returns> An amount of milliseconds before the tooltip window is shown. </returns>
    /// <seealso cref="TipHandler.SetTipTimeMsec(uint)"/>
    protected override uint TipTimeDelayMsec
    {
        get => (base.TipTimeDelayMsec / 2);
    }
    #endregion // Properties

    #region Methods

    #region Public_Methods

    /// <summary>
    /// Get the subclassed ComboBox window (if there is any)
    /// </summary>
    /// <returns>The <see cref="ComboBox"/> control, or null. </returns>
    public ComboBox GetComboBox()
    {
        return HookedControl() as System.Windows.Forms.ComboBox;
    }

    /// <summary>
    /// Returns CBS_SIMPLE, CBS_DROPDOWN or CBS_DROPDOWNLIST. Returns zero in case of error.
    /// </summary>
    /// <returns>The type of ComboBox</returns>
    public uint GetComboType()
    {
        ComboBox pComboBox;
        uint nStyle;
        uint nResult = 0;

        if (null != (pComboBox = GetComboBox()))
        {
            nStyle = (uint)User32.GetWindowLong(pComboBox.Handle, (int)Win32.GWL.GWL_STYLE);
            nResult = (nStyle & 3);
        }
        else
        {
            Debug.Fail("Unable to retrieve the ComboBox control");
        }
        return nResult;
    }

    /// <summary>
    /// Is visible the vertical scrollbar on the window listbox part of the ComboBox ?
    /// </summary>
    /// <returns>True if visible, false if not.</returns>
    public bool IsVerticalScrollbarVisible()
    {
        IntPtr hList;
        bool bScrollBar = false;

        if (IntPtr.Zero != (hList = this.GetListBoxPartHandle()))
        {
            int minScroll = 0;
            int maxScroll = 0;
            User32.SCROLLINFO scrollInfo = User32.SCROLLINFO.Default;

            User32.GetScrollRange(hList, Win32.SB_VERT, ref minScroll, ref maxScroll);
            if (minScroll < maxScroll)
            {
                User32.GetWndScrollInfo(hList, Win32.SB_VERT, ref scrollInfo, Win32.SIF_ALL);
                bScrollBar = (scrollInfo.nPage < (uint)(maxScroll - minScroll + 1));
            }
        }

        return bScrollBar;
    }

    /// <summary>
    /// Overriding the method of the base class, to perform specific processing.
    /// </summary>
    /// <param name="iWnd"> An object that exposes Win32 HWND handle.</param>
    /// <returns>True on success, false on failure.</returns>
    public override bool HookControl(IWin32Window iWnd)
    {
        bool bRes = base.HookControl(iWnd);

        // Must test IsHooked, 
        // because unhooking is called simply by HookWindow(NULL)
        if (IsHooked)
        {
            // The other hook _lbHook must be hooked later,
            // when handling WM_CTLCOLORLISTBOX.
        }
        else
        {
            if (_lbHook.IsHooked)
            {
                _lbHook.HookWindow(null);  // unhook the _lbHook
            }
        }

        return bRes;
    }

    /// <inheritdoc/>
    public override uint ItemFromPoint(Point ptClient, out uint nSubItem)
    {
        return ItemFromPoint(ptClient, out nSubItem, out _);
    }

    /// <inheritdoc/>
    public override void GetItemInfo(
        uint nItem,
        uint nSubItem,
        bool bFontInControl,
        out Rectangle rc,
        out string s)
    {
        rc = Rectangle.Empty;
        s = string.Empty;

        if ((nItem != (uint)ItemNumber.cNoItem) &&
            (nSubItem != (uint)SubItemNumber.cNoSubItem))
        {
            Size size;
            Font tmpFont;
            ComboBox pComboBox = GetComboBox();

            s = GetItemText(nItem, nSubItem);
            if (bFontInControl)
                tmpFont = pComboBox.Font;
            else
                tmpFont = this.TipWindow.Font;

            using (Graphics g = pComboBox.CreateGraphics())
            {
                size = TextRenderer.MeasureText(g, s, tmpFont);
                rc = new Rectangle(rc.Location, size);
            }

            // I don't know why the CBS_SIMPLE needs that offset; 
            // anyway to work well the rectangle has to be adjusted that way
            if (GetComboType() == (uint)Win32.ComboType.CBS_SIMPLE)
            {
                rc.Offset(0, 1);
                rc.Size = new Size(rc.Width + SystemInformation.Border3DSize.Width, rc.Height);
            }
        }
    }
    #endregion // Public_Methods

    #region Protected_Methods

    /// <summary>
    /// The alias for window procedure, declaring as a last argument a ref User32.RECT instead of IntPtr lParam.
    /// This is needed if I want to get the dropped control rectangle, by sending Win32.CB_GETDROPPEDCONTROLRECT.
    /// </summary>
    ///
    /// <param name="hWnd">   The window handle of the. </param>
    /// <param name="Msg">    The message being passed. </param>
    /// <param name="wParam"> wParam which is interpreted differently depending on the message. </param>
    /// <param name="lpRect"> [in,out] A rectangle structure that receives the coordinates of the combo box in its
    /// dropped-down state. </param>
    ///
    /// <returns> If the message succeeds, the return value is nonzero. Otherwise, zero is returned. </returns>
    ///
    /// <seealso href="https://docs.microsoft.com/en-us/windows/win32/controls/cb-getdroppedcontrolrect">
    /// MSDN documentation of CB_GETDROPPEDCONTROLRECT
    /// </seealso>
    [DllImport("user32", EntryPoint = "SendMessage")]
    private static extern IntPtr ComboBox_GetDroppedControlRectAlias(IntPtr hWnd, int Msg, IntPtr wParam, ref User32.RECT lpRect);

    /// <summary>
    /// Auxiliary method returning rectangle of dropped ComboBox control.
    /// </summary>
    /// <param name="hComboBx">The Win32 handle of ComboBox.</param>
    /// <returns>A rectangle structure that contains the coordinates of the combo box in its
    /// dropped-down state. </returns>
    protected static Rectangle ComboBox_GetDroppedControlRect(IntPtr hComboBx)
    {
        Rectangle result;
        User32.RECT rc = new();

        ComboBox_GetDroppedControlRectAlias(hComboBx,
            Win32.CB_GETDROPPEDCONTROLRECT, IntPtr.Zero, ref rc);
        result = User32.RectAPI2Rectangle(rc);
        return result;
    }

    /// <summary>
    /// Get the hooked window listbox part of the ComboBox (if there is any).
    /// This method works only after the listbox part is actually hooked 
    /// (see the code ComboBoxTipHandler::HookWindowProc and GetListBoxOfCombo for details).
    /// </summary>
    /// <returns>Window handle</returns>
    protected IntPtr GetListBoxPartHandle()
    {
        return _lbHook.GetListBox();
    }

    /// <inheritdoc/>
    protected override string GetItemText(uint nItem, uint nSubItem)
    {
        string result = string.Empty;

        if ((nItem != (uint)ItemNumber.cNoItem) && (nSubItem != (uint)SubItemNumber.cNoSubItem))
        {
            ComboBox pComboBox = GetComboBox();

            Debug.Assert(pComboBox != null);
            Debug.Assert(nItem < pComboBox.Items.Count);
            Debug.Assert(nSubItem == 0);

            result = pComboBox.Items[(int)nItem].ToString();
        }

        return result;
    }

    /// <summary>
    /// Overriding the method of the base class to provide specific functionality for tooltips 
    /// </summary>
    /// <param name="ptInScreen">Screen coordinates of mouse position.</param>
    protected override void OnMouseMove(Point ptInScreen)
    {
        if (User32.IsWindowVisible(this.HookedHWND))
        {
            ComboBox pComboBox = GetComboBox();
            Point ptClient = pComboBox.PointToClient(ptInScreen);

            // Get text and text rectangle for item under mouse item text
            Rectangle rcText = Rectangle.Empty; // item text rectangle in combo
            uint nItem = (uint)ItemNumber.cNoItem;
            IntPtr hListBx = GetListBoxPartHandle();
            bool bTipWasVisible = TipWindow.IsVisible;

            // Must avoid displaying any tooltip, if there is another
            // modal MessageBox or modal dialog overlapping the ComboBox.
            // That's why there is a test regarding WndFromPoint
            IntPtr hFromPoint = WndFromPoint(ptInScreen);
            IntPtr hHooked = pComboBox.Handle;
            int nCxOffset = 0;

            /*
            if ((hFromPoint == hListBx) || (hFromPoint == hHooked)|| (hFromPoint == GetParent(hHooked)))
               Must be without the last 'or':
            */
            if ((hFromPoint == hListBx) || (hFromPoint == hHooked))
            {   // Get text and text rectangle for item under mouse
                nItem = OnGetItemInfo(ptClient, true, out _, out rcText, out _);
            }
            // and for another WndFromPoint don't display anything ...

            if (nItem == (uint)ItemNumber.cNoItem)
            {
                TipWindow.Cancel(); // no item: cancel popup text
                if (_lbHook.IsScrolling)
                    _lbHook.SetLastScrollItem((uint)ItemNumber.cNoItem);
            }
            else if (nItem != GetCurItem())
            {
                bool bHighlited;

                TipWindow.Cancel(); // new item, or no item: cancel popup text

                // Change the selection in the list ( to follow the mouse ).
                //
                // But don't do that for CBS_SIMPLE 
                // ( since in WinXp combo CBS_SIMPLE, the list selection and 
                // the edited text in combo cooperate their own built-in way ... ).
                if (GetComboType() != (uint)Win32.ComboType.CBS_SIMPLE)
                {
                    bHighlited = true;
                    if ((hListBx != IntPtr.Zero) && bTipWasVisible)
                    {
                        User32.SendMessage(hListBx, Win32.LB_SETCURSEL, (IntPtr)nItem, IntPtr.Zero);
                    }
                }
                else
                {
                    int nLbSelItem = (int)User32.SendMessage(hListBx, Win32.LB_GETCURSEL, IntPtr.Zero, IntPtr.Zero);
                    bHighlited = (nLbSelItem == nItem);
                    nCxOffset = 1;
                }

                // please don't show the tooltip when the list is scrolling
                if (_lbHook.IsScrolling)
                {   // Remember the last item in the member variable.
                    // Will be used when posting a message in CInternalListHook::HookWindowProc
                    _lbHook.SetLastScrollItem(nItem);
                    // and process it here as if no item
                    nItem = (uint)ItemNumber.cNoItem;
                }
                else if (!IsRectCompletelyVisible(rcText))
                {
                    Rectangle rcBox;
                    Size sz;
                    Point ptLocation;

                    // new item, and not entirely visible: prepare popup tip
                    OnGetItemInfo(ptClient, false, out _, out Rectangle rcTipText, out string sText);
                    // set tip text to that of item
                    TipWindow.Text = sText;

                    // need text rectangle in screen coordinates
                    rcBox = User32.GetWindowRect(pComboBox.Handle);
                    rcTipText.Offset(rcBox.X, rcBox.Y);
                    rcTipText.Offset(nCxOffset, 0);

                    // set highlighted status
                    TipWindow.DrawHighlighted = g_bDrawSelHighlighted && bHighlited;

                    // move tip window over list text
                    sz = new Size(rcTipText.Width + 8, rcTipText.Height);
                    ptLocation = new Point(rcTipText.Left + 1, rcTipText.Top);
                    TipWindow.Size = sz;
                    TipWindow.MoveAsTopmost(ptLocation);
                    TipWindow.ShowDelayed((int)TipTimeDelayMsec); // show popup text delayed
                }
            }
            this._nCurItem = nItem;
        }
        else
        {
            CancelTipWindow();
        }
    }

    /// <summary> Implementation helper, called from the other override ItemFromPoint. </summary>
    ///
    /// <param name="ptClient">The input point, representing the position of the mouse. (Is in client coordinates). </param>
    /// <param name="nSubItem"> [out] The output argument returns the index of found sub-item, or
    /// SubItemNumber.cNoSubItem if no sub-item found. </param>
    /// <param name="rc">       [out] The rectangle of found sub-item in the found item, or Rectangle.Empty if no
    /// matching item and item are found. </param>
    ///
    /// <returns>
    /// The index of found item for the particular point, or ItemNumber.cNoItem if no item is found.
    /// </returns>
    protected uint ItemFromPoint(
        Point ptClient,
        out uint nSubItem,
        out Rectangle rc)
    {
        uint nItem = (uint)ItemNumber.cNoItem;
        ComboBox pComboBox = GetComboBox();

        Debug.Assert(pComboBox != null);
        rc = Rectangle.Empty;
        nSubItem = (uint)SubItemNumber.cNoSubItem;

        if (pComboBox.DroppedDown)
        {
            IntPtr hListBx = GetListBoxPartHandle();
            Rectangle wndRect, dropRect, listPartClient, rcListBx;
            bool bDroppedAbove = false;
            bool bInEdit = false;
            bool bInEditOrDummySpace = false;

            dropRect = ComboBox_GetDroppedControlRect(pComboBox.Handle);
            wndRect = User32.GetWindowRect(pComboBox.Handle);
            rcListBx = User32.GetWindowRect(hListBx);

            // When dropped list is actually above the combo
            // (it is the case when there is not enough space on the screen to drop it below )
            // the method GetDroppedControlRect returned the wrong value,
            // so the variable dropRect needs to be fixed
            if (GetComboType() != (uint)Win32.ComboType.CBS_SIMPLE)
            {
                if (rcListBx.Top < wndRect.Top)
                {
                    bDroppedAbove = true;
                    dropRect = rcListBx;
                    listPartClient = pComboBox.RectangleToClient(dropRect);
                }
                else
                {
                    dropRect = rcListBx;
                    listPartClient = pComboBox.RectangleToClient(dropRect);
                }
            }
            else
            {
                listPartClient = pComboBox.RectangleToClient(dropRect);
            }

            if (listPartClient.Contains(ptClient))  // now if the point is in that part
            {
                Rectangle rcClientEdit = Rectangle.Empty;
                Rectangle rcCurrentLine = listPartClient;
                int nEditHeight = (int)User32.SendMessage(pComboBox.Handle,
                    Win32.CB_GETITEMHEIGHT, -1, IntPtr.Zero);

                if (_editHook.IsHooked)
                {
                    rcClientEdit = User32.GetWindowRect(_editHook.HookedHWND);
                    rcClientEdit = pComboBox.RectangleToClient(rcClientEdit);
                    Debug.Assert(nEditHeight == rcClientEdit.Height);
                }
                if (!bDroppedAbove)
                {   // rcCurrentLine will be set to the rectangle of edit control,
                    // with added magic value 7
                    rcCurrentLine.Location = new Point(0, 0);
                    rcCurrentLine.Height = (nEditHeight + 7); // magic value 7
                    bInEditOrDummySpace = rcCurrentLine.Contains(ptClient);
                }
                else
                {   // set the rcCurrentLine at the very top, with zero height:
                    rcCurrentLine.Size = new Size(rcCurrentLine.Width, 0); // bottom = top
                    rcCurrentLine.Offset(0, 1);
                    // In edit control ?
                    bInEdit = rcClientEdit.Contains(ptClient);
                    // It may happen that we have no edit control ( the case CBS_DROPDOWN ),
                    // so the rcClientEdit is empty, but we still want to return ItemNumber.cNoItem;
                    // since the mouse is over the 'fixed' ( not dropped-up ) part of combo.
                    // That's why the following code checks for pComboBox.MaxDropDownItems value too...
                }

                if (!(bInEdit || bInEditOrDummySpace))
                {
                    int ii, nMaxVisible;
                    int nDexVisible = 0;
                    int nTopIndex = (int)User32.SendMessage(pComboBox.Handle, Win32.CB_GETTOPINDEX, IntPtr.Zero, IntPtr.Zero);
                    int isz = pComboBox.Items.Count;

                    // Compute nMaxVisible based on properties MaxDropDownItems and IntegralHeight.
                    //
                    // When the property IntegralHeight is set to true, the control automatically resizes 
                    // to ensure that an item is not partially displayed. 
                    // If you want to maintain the original size of the ComboBox based on the space 
                    // requirements of your form, that property should be set to false. 
                    // For more info, see
                    // http://stackoverflow.com/questions/3868907/ComboBox-maxdopdownitems-is-not-working-when-adding-items-using-the-click-event
                    if (!pComboBox.IntegralHeight)
                        nMaxVisible = pComboBox.MaxDropDownItems;
                    else
                        nMaxVisible = pComboBox.Items.Count; // in that case MaxDropDownItems is not relevant value

                    for (ii = nTopIndex; ii < isz; ii++)
                    {
                        // compute next item rectangle from the previous one
                        rcCurrentLine.Offset(0, rcCurrentLine.Height);
                        rcCurrentLine.Size = new Size(rcCurrentLine.Width, pComboBox.GetItemHeight(ii));

                        if ((++nDexVisible > nMaxVisible) || (rcCurrentLine.Top >= dropRect.Bottom - 1))
                        {
                            break; // already out of dropped rectangle
                        }
                        else if (rcCurrentLine.Contains(ptClient))
                        {
                            nItem = (uint)ii;  // that's what we look for !
                            rc = rcCurrentLine;
                            nSubItem = 0;
                            break;
                        }
                    }
                }
            }
        }

        return nItem;
    }

    /// <summary>
    /// Overriding the method of the base class
    /// </summary>
    /// <param name="ptClient"> Input argument. Position of mouse (in client's coordinate) </param>
    /// 
    /// <param name="bFontInControl"> Input argument. 
    /// Either we are computing the item's rectangle in the control (so bFontInControl == true),
    /// or we are computing the size of rectangle of the final tooltip (so bFontInControl == false),
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
    /// <returns>The index of found item, or ItemNumber.cNoItem if no item is found.</returns>
    protected override uint OnGetItemInfo(
        Point ptClient,
        bool bFontInControl,
        out uint nSubItem,
        out Rectangle rc,
        out string s)
    {
        uint nItem = ItemFromPoint(ptClient, out nSubItem, out Rectangle rcFromPoint);

        if ((nItem != (uint)ItemNumber.cNoItem) &&
            (nSubItem != (uint)SubItemNumber.cNoSubItem))
        {
            GetItemInfo(nItem, nSubItem, bFontInControl, out rc, out s);
            rc.Offset(0, rcFromPoint.Y);
        }
        else
        {
            s = string.Empty;
            rc = Rectangle.Empty;
            nItem = (uint)ItemNumber.cNoItem;
        }

        return nItem;
    }

    /// <summary>
    /// Determines whether given rectangle is completely visible within the hooked control ( listbox etc.).
    /// Overwriting the implementation of the base class.
    /// </summary>
    /// <param name="rc">A checked rectangle (might be previously retrieved by calling <see cref="GetItemInfo"/>).</param>
    /// <returns>True if the rectangle is completely visible; false otherwise.</returns>
    protected override bool IsRectCompletelyVisible(Rectangle rc)
    {
        ComboBox pComboBox = GetComboBox();
        Rectangle rcVisible = pComboBox.ClientRectangle;
        int cxDelta = SystemInformation.Border3DSize.Width;

        if (IsVerticalScrollbarVisible())
        {
            cxDelta += SystemInformation.VerticalScrollBarWidth;
        }
        rcVisible.Size = new Size(rcVisible.Width - cxDelta, rcVisible.Height);

        return rcVisible.Width > rc.Right;
    }

    /// <summary>
    /// Overwritten method of the base class; modifies the subclasses window procedure behaviour
    /// </summary>
    /// <param name="m">Processed Windows Message object.</param>
    protected override void HookWindowProc(ref Message m)
    {
        // preprocess message
        switch (m.Msg)
        {
            case (int)Win32.WM.WM_CTLCOLORLISTBOX:
                if (!_lbHook.IsHooked)
                    _lbHook.HookWindow(m.LParam);  // make the hookup to have the listbox HWND available
                else
                    Debug.Assert(_lbHook.HookedHWND == m.LParam);
                break;

            case (int)Win32.WM.WM_CTLCOLOREDIT:
                if (!_editHook.IsHooked)
                    _editHook.HookWindow(m.LParam);  // make the hookup to have the listbox HWND available
                else
                    Debug.Assert(_editHook.HookedHWND == m.LParam);
                break;
        }

        base.HookWindowProc(ref m);
    }
    #endregion // Protected_Methods
    #endregion // Methods
}
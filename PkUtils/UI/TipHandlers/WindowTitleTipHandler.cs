// Ignore Spelling: Meth, Msec, mss, Utils, tooltip
//
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.SystemEx;
using PK.PkUtils.Utils;
using PK.PkUtils.WinApi;

#pragma warning disable IDE0290     // Use primary constructor

namespace PK.PkUtils.UI.TipHandlers
{
    /// <summary> Supports tooltips for window title. </summary>
    [CLSCompliant(false)]
    public class WindowTitleTipHandler : TipHandler
    {
        #region Typedefs

        /// <summary> A specialized low-level mouse hook, used to trap WM_LBUTTONDOWN targeting window TitleBar. 
        ///           There seem no easier way to do this. 
        /// </summary>
        protected class LowLevelSystemMouseHook4Title : WindowsSystemHookMouseLL
        {
            private readonly WindowTitleTipHandler _handler;

            /// <summary> the only one constructor. </summary>
            /// <param name="handler">  The handler who created this. </param>
            protected internal LowLevelSystemMouseHook4Title(WindowTitleTipHandler handler)
            {
                _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            }

            /// <summary>
            /// Overrides the method of base hook, to perform specific mouse functionality.
            /// </summary>
            ///
            /// <param name="wParam">   The identifier of the mouse message. </param>
            /// <param name="mss">      A structure containing information about a low-level mouse input event. </param>
            /// <returns>  True if the message has been completely handled by this hook instance, 
            ///            and the caller should NOT proceed passing it to other hooks in the chain; 
            ///            false otherwise. Use 'true' value with care! </returns>
            protected override bool MouseLLHookMeth(IntPtr wParam, Win32.MSLLHOOKSTRUCT mss)
            {
                switch ((int)wParam)
                {
                    case (int)Win32.WM.WM_LBUTTONDOWN:
                    case (int)Win32.WM.WM_LBUTTONDBLCLK:
                    case (int)Win32.WM.WM_RBUTTONDOWN:
                        _handler.CancelTipWindow();
                        break;

                    case (int)Win32.WM.WM_MOUSEMOVE:
                        if (!IsLeftMouseButtonDown())
                        {
                            _handler.DelegateMouseMove(new Point(mss.pt.x, mss.pt.y));
                        }
                        break;
                }

                return false;
            }
        }
        #endregion // Typedefs

        #region Fields

        /// <summary> Constant representing the 'title bar item' subject. </summary>
        public const int TitleBarItem = 0;

        /// <summary> Null if tooltip not displayed; otherwise the timing when has been displayed. </summary>
        private Nullable<DateTime> _sinceDisplayed;

        /// <summary> The tooltip offset used when <see cref="_sinceDisplayed"/> was assigned. </summary>
        private Nullable<Size> _offsetDisplayed;

        /// <summary> True to keep the tooltip window topmost, false if not. </summary>
        private readonly bool _keepTopMost;

        /// <summary> Backing field of <see cref="MousePosOffset"/>. </summary>
        private readonly Size _mousePosOffset;

        /// <summary> The low level mouse hook. </summary>
        private LowLevelSystemMouseHook4Title _llMouseHook;

        /// <summary> Backing field of <see cref="IsInSizeMove"/>. </summary>
        private bool _isInSizeMove;

        /// <summary>
        /// The delay time in milliseconds that is used when showing the tooltip by calling
        /// <see cref="IPopupText.ShowDelayed(int)"/>.  
        /// Should be bigger for title tip window than for instance for ComboBox.
        /// </summary>
        private const int _biggerTipTimeMs = 960;

        private const int _hideIfMouseMovingAndDisplyedFor2200ms = 2200;
        private const int _moveIfMouseMovingAndDisplyedFor500ms = 500;
        #endregion // Fields

        #region Constructor(s)

        /// <summary> Default argument-less constructor. </summary>
        public WindowTitleTipHandler()
            : this(false)
        { }

        /// <summary> Constructor with provided topmost argument. </summary>
        ///
        /// <param name="keepTopMost">  True to keep the tooltip window topmost, false if not. </param>
        public WindowTitleTipHandler(bool keepTopMost)
            : this(keepTopMost, _biggerTipTimeMs)
        { }

        /// <summary> Constructor with provided topmost argument. </summary>
        ///
        /// <param name="keepTopMost">  True to keep the tooltip window topmost, false if not. </param>
        /// <param name="tipTimeDelayMsec">  Provides initial value of <see cref="TipHandler.TipTimeDelayMsec"/>. </param>
        public WindowTitleTipHandler(bool keepTopMost, uint tipTimeDelayMsec)
            : this(keepTopMost, tipTimeDelayMsec, new Size(8, -4))
        { }

        /// <summary> Constructor with provided topmost and mouse position offset arguments. </summary>
        ///
        /// <param name="keepTopMost">   True to keep the tooltip window topmost, false if not. </param>
        /// <param name="tipTimeDelayMsec">  Provides initial value of <see cref="TipHandler.TipTimeDelayMsec"/>. </param>
        /// <param name="mousePosOffset">   The mouse position offset from tooltip ( or rather vice versa ). </param>
        public WindowTitleTipHandler(bool keepTopMost, uint tipTimeDelayMsec, Size mousePosOffset)
            : base(true, tipTimeDelayMsec)
        {
            _keepTopMost = keepTopMost;
            _mousePosOffset = mousePosOffset;
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary> Gets a value indicating whether to keep the tooltip window topmost. </summary>
        protected bool KeepTopmost
        {
            get => _keepTopMost;
        }

        /// <summary> Gets the mouse position offset, as initialized by constructor. </summary>
        protected Size MousePosOffset
        {
            get => _mousePosOffset;
        }

        /// <summary> Gets for how long is tooltip displayed, in milliseconds. </summary>
        protected double ForHowLongDisplayed
        {
            get
            {
                double result;

                if (_sinceDisplayed.HasValue)
                    result = (DateTime.Now - _sinceDisplayed.Value).TotalMilliseconds;
                else
                    result = 0;

                return result;
            }
        }

        /// <summary> Returns true if the hooked window is in moving or sizing modal loop. </summary>
        protected bool IsInSizeMove
        {
            get => _isInSizeMove;
        }
        #endregion // Properties

        #region Methods

        #region Public_Methods

        /// <inheritdoc/>
        public override uint ItemFromPoint(Point ptClient, out uint nSubItem)
        {
            uint nItem = (uint)ItemNumber.cNoItem;
            nSubItem = (uint)SubItemNumber.cNoSubItem;

            Size szTitle = TitleBarSize();
            Point ptWnd = new(ptClient.X, szTitle.Height + ptClient.Y);
            Rectangle rcTitle = TitleBarRectangle();

            if (rcTitle.Contains(ptWnd))
            {
                nItem = TitleBarItem;
                nSubItem = 0;
            }

            return nItem;
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

            if (nItem != (uint)ItemNumber.cNoItem)
            {
                if (nItem == TitleBarItem)
                {
                    Font pFont = (bFontInControl) ? null : TipWindow.Font;

                    s = GetItemText(nItem, nSubItem);
                    rc = TitleBarRectangle();
                    using Graphics g = Graphics.FromHwnd(this.HookedHWND);
                    Size size = TextRenderer.MeasureText(g, s, pFont);
                    rc = new Rectangle(rc.Location, size);
                }
                else
                {
                    Debug.Fail("not expected value");
                }
            }
        }

        /// <inheritdoc/>
        public override void CancelTipWindow()
        {
            base.CancelTipWindow();
            _sinceDisplayed = null;
            _offsetDisplayed = null;
        }
        #endregion // Public_Methods

        #region Protected_Internal_Methods

        /// <summary>
        /// Executes the mouse move operation. 
        /// Implementation helper, to make protected method accessible from <see cref="WindowTitleTipHandler"/>
        /// </summary>
        ///
        /// <param name="ptInScreen">   Screen coordinates of mouse position. </param>
        protected internal void DelegateMouseMove(Point ptInScreen)
        {
            OnMouseMove(ptInScreen);
        }
        #endregion // Protected_Internal_Methods

        #region Protected_Methods

        /// <summary> Begins size move loop. Handling <see cref="Win32.WM.WM_ENTERSIZEMOVE"/> message. </summary>
        protected virtual void BeginSizeMoveLoop()
        {
            if (!IsInSizeMove)
            {
                _isInSizeMove = true;
                CancelTipWindow();
            }
        }

        /// <summary> Ends size move loop. Handling <see cref="Win32.WM.WM_EXITSIZEMOVE"/> message. </summary>
        protected virtual void EndSizeMoveLoop()
        {
            if (IsInSizeMove)
            {
                _isInSizeMove = false;
            }
        }

        /// <summary> Subclasses window procedure; overwrites the base class behavior. </summary>
        /// <param name="m">Processed Windows Message object.</param>
        protected override void HookWindowProc(ref Message m)
        {
            // preprocess the message
            switch (m.Msg)
            {
                case (int)Win32.WM.WM_MOVE:
                    // Don't check IsInSizeMove - apparently WM_MOVE may occur without preceding WM_ENTERSIZEMOVE
                    CancelTipWindow();
                    break;

                case (int)Win32.WM.WM_ENTERSIZEMOVE:
                    BeginSizeMoveLoop();
                    break;
            }

            // process the message
            base.HookWindowProc(ref m);

            // post-process the message
            switch (m.Msg)
            {
                case (int)Win32.WM.WM_EXITSIZEMOVE:
                    EndSizeMoveLoop();
                    break;
            }
        }

        /// <inheritdoc/>
        protected override void OnHookup(object pExtraInfo)
        {
            base.OnHookup(pExtraInfo);
            // lazy installing low level mouse hook
            if (null == _llMouseHook)
            {
                _llMouseHook = new LowLevelSystemMouseHook4Title(this);
            }
            if (!_llMouseHook.IsInstalled)
            {
                _llMouseHook.Install();
            }

            // make hidden initially
            CancelTipWindow();
        }

        /// <inheritdoc/>
        protected override void OnUnhook(object pExtraInfo)
        {
            Disposer.SafeDispose(ref _llMouseHook);
            base.OnUnhook(pExtraInfo);
        }

        /// <inheritdoc/>
        protected override bool IsRectCompletelyVisible(Rectangle rc)
        {
            // just return always false, to make it display tooltip always when mouse over TitleBar
            return false;
        }

        /// <inheritdoc/>
        protected override string GetItemText(uint nItem, uint nSubItem)
        {
            string result = string.Empty;

            if ((nItem != (uint)ItemNumber.cNoItem) && (nSubItem != (uint)SubItemNumber.cNoSubItem))
            {
                Debug.Assert(nItem == TitleBarItem);
                Debug.Assert(nSubItem == 0);
                result = User32.GetWindowText(HookedHWND);
            }

            return result;
        }

        /// <summary> Overriding the method of the base class </summary>
        /// <param name="ptInScreen">Screen coordinates of mouse position.</param>
        protected override void OnMouseMove(Point ptInScreen)
        {
            if (IsInSizeMove || !User32.IsWindowVisible(HookedHWND))
            {
                return;
            }

            // Get text and text rectangle for item under mouse
            string sText;
            Rectangle rcText = Rectangle.Empty; // item text rectangle
            uint nSubItem = (uint)SubItemNumber.cNoSubItem;
            uint nItem = (uint)ItemNumber.cNoItem;
            Point ptClient = ptInScreen;
            ScreenToClient(this.HookedHWND, ref ptClient);

            Debug.Assert(TipWindow != null);
            Size fixedOffset = MousePosOffset;

            // determine hit test
            IntPtr lParam = Win32.MAKELPARAM((ushort)ptInScreen.X, (ushort)ptInScreen.Y);
            IntPtr hitTest = User32.SendMessage(HookedHWND, (int)Win32.WM.WM_NCHITTEST, IntPtr.Zero, lParam);

            if ((int)hitTest == (int)Win32.MousePositionCode.HTCAPTION)
            {
                // Must avoid displaying any tooltip, if there is another modal MessageBox or modal dialog overlapping the window.
                // Checks that by involving WndFromPoint
                IntPtr hControl = WndFromPoint(ptInScreen);

                if (hControl != IntPtr.Zero)
                {
                    if ((hControl == this.HookedHWND) ||
                        (hControl == User32.GetParent(this.HookedHWND)))
                    {
                        // Get text and text rectangle for item under mouse, font in control = true
                        nItem = OnGetItemInfo(ptClient, true, out nSubItem, out rcText, out sText);
                    }
                }
            }

            if (nItem == (uint)ItemNumber.cNoItem)
            {
                CancelTipWindow();
            }
            else if (nItem != GetCurItem())
            {
                // new item, or no item: cancel popup text
                CancelTipWindow();
                _nCurItem = nItem; // should set the _nCurItem just now, so the GetCurItem(void) return the proper value

                if (!IsRectCompletelyVisible(rcText))
                {
                    // It's new item, and not wholly visible: prepare popup tip
                    // 
                    OnGetItemInfo(ptClient, false, out nSubItem, out Rectangle rc, out sText);  // item text rectangle in popup tip
                    int msDelay = (int)TipTimeDelayMsec;
                    Size sz = rc.Size + TipWindow.Margins;
                    Size offset = fixedOffset + new Size(0, -sz.Height);

                    // set tip text and highlighted status
                    TipWindow.Text = sText;
                    TipWindow.DrawHighlighted = false;

                    TipWindow.MoveToScreen(ptInScreen, sz, KeepTopmost);

                    // show popup text delayed
                    TipWindow.ShowDelayed(msDelay, offset);
                    //  actually not displayed yet, just records the request when should be displayed
                    _sinceDisplayed = DateTime.Now + TimeSpan.FromMilliseconds(msDelay);
                    _offsetDisplayed = offset;
                }
            }
            else if (IsTipWindowVisible() || _sinceDisplayed.HasValue)
            {
                if (ForHowLongDisplayed >= _hideIfMouseMovingAndDisplyedFor2200ms)
                {
                    CancelTipWindow();
                }
                else if (ForHowLongDisplayed >= _moveIfMouseMovingAndDisplyedFor500ms)
                {
                    TipWindow.MoveToScreen(ptInScreen + _offsetDisplayed.Value);
                }
            }
        }

        /// <summary>
        /// Overriding the method of the base class
        /// </summary>
        /// <param name="ptClient"> Input argument. Position of mouse (in client's coordinate). </param>
        /// 
        /// <param name="bFontInControl"> Input argument. 
        /// Either we are computing the item's rectangle in the control (so bFontInControl == true),
        /// or we are computing the size of rectangle of the final tooltip (so bFontInControl == false),
        /// ( As the font in the control and font in the tooltip may be different,
        /// the resulting rectangle of the tooltip might have different size, 
        /// so the method needs to know about what rectangle we are asking.)
        /// </param>
        /// 
        /// <param name="nSubItem">Sub-item of the item (will be used for the case of ListCtrl tooltips, where nSubItem == column.
        /// But in most cases (ListBox, ComboBox) it will be always simply zero). 
        /// </param>
        /// 
        /// <param name="rc"> The resulting rectangle of the item, respective of the tooltip ( see bFontInControl )
        /// </param>
        /// 
        /// <param name="s"> The text of the item, respective of the tooltip ( see bFontInControl )
        /// </param>
        /// 
        /// <returns>The index of found item, or ItemNumber.cNoItem if no item is found.</returns>
        protected override uint OnGetItemInfo(
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

        /// <summary> Cleanup any resources being used. </summary>
        ///
        /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposer.SafeDispose(ref _llMouseHook);
            }
            base.Dispose(disposing);
        }
        #endregion // Protected_Methods

        #region Private_Methods

        private Size TitleBarSize()
        {
            IntPtr hWnd;
            Size result = new(0, 0);

            if (User32.IsWindow(hWnd = this.HookedHWND))
            {
                Rectangle rcWindow = User32.GetWindowRect(hWnd);
                Rectangle rcClient = User32.GetClientRect(hWnd);
                int heightOfTheTitleBar_ofThis = rcWindow.Height - rcClient.Height;

                result = new Size(rcWindow.Width, heightOfTheTitleBar_ofThis);
            }

            return result;
        }

        private Rectangle TitleBarRectangle()
        {
            Size szTitle = TitleBarSize();
            Rectangle rcTitle = new(0, 0, szTitle.Width, szTitle.Height);
            return rcTitle;
        }
        #endregion // Private_Methods
        #endregion // Methods
    }
}
#pragma warning restore IDE0290 // Use primary constructor
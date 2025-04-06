/***************************************************************************************************************
*
* FILE NAME:   .\UI\TipHandlers\PopupTooltip.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class PopupTooltip
*
**************************************************************************************************************/

// Ignore Spelling: Utils, Popup, Tooltip, msec
//
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using PK.PkUtils.MessageHooking;
using PK.PkUtils.Utils;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.UI.TipHandlers;

/// <summary>
/// Implements the tooltip window ( the interface IPopupText ) for tip handlers
/// like <see cref="ListBoxTipHandler"/> and <see cref="ComboBoxTipHandler"/>.
/// </summary>
[CLSCompliant(true)]
public class PopupTooltip : Control, IPopupText
{
    #region Typedefs

    /// <summary>
    /// MyBoxText will be a child control ( the only visible window )
    /// </summary>
    internal class MyBoxText : TextBox
    {
        private bool _bDrawHighlited;
        private readonly PopupTooltip _owner;

        /// <summary> Creating new instance with given owner <paramref name="tooltip"/>. </summary>
        ///
        /// <param name="tooltip"> The tooltip. </param>
        internal MyBoxText(PopupTooltip tooltip)
        {
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Location = new Point(0, 0);
            this.Multiline = true;
            this.Name = "textBoxTooltip";
            this.ReadOnly = true;
            this.TabIndex = int.MaxValue;
            this.Visible = true;
            this.AcceptsReturn = false;
            this.SetStyle(ControlStyles.Selectable, false);
            this.Size = new System.Drawing.Size(100, 20);
            this.Text = string.Empty;

            this._owner = tooltip;
            RefreshColors();
        }

        public bool DrawHighlighted
        {
            get { return _bDrawHighlited; }

            set
            {
                if (DrawHighlighted != value)
                {
                    _bDrawHighlited = value;
                    RefreshColors();
                }
            }
        }

        /// <summary> Gets the owning tooltip, as initialized by constructor. </summary>
        protected PopupTooltip Owner
        {
            get => _owner;
        }

        /// <summary> Refresh foreground and background colors. </summary>
        protected void RefreshColors()
        {

            if ((Owner == null) || !Owner.GetTextColors(DrawHighlighted, out Color foreColor, out Color backColor))
            {
                PopupTooltip.GetDefaultTextColors(DrawHighlighted, out foreColor, out backColor);
            }

            this.ForeColor = foreColor;
            this.BackColor = backColor;
        }

        /// <summary>
        /// Overwrites the implementation of the base class, in order to provide custom processing
        /// </summary>
        /// <param name="m">A Windows Message object.
        /// </param>
        protected override void WndProc(ref Message m)
        {
            bool bHandled = false;

            // pre-processing the message
            switch (m.Msg)
            {
                // To make it transparent and prevent mouse clicks being sent to this window.
                // If the window returns HTTRANSPARENT, 
                // Windows doesn't send a WM_LBUTTONDOWN at all, but instead attempts 
                // to pass the mouse event to the first nontransparent window 
                // under the transparent one. 
                case (int)Win32.WM.WM_NCHITTEST:
                    m.Result = new IntPtr((int)Win32.MousePositionCode.HTTRANSPARENT);
                    bHandled = true;
                    break;
            }
            // processing the message
            if (!bHandled)
            {
                base.WndProc(ref m);
            }

            // post-processing the message
            switch (m.Msg)
            {
                // Following code draws the border once again, "manually".
                // This is needed if the application called Application.SetCompatibleTextRenderingDefault(false)
                // in the very beginning, as usually does in auto-generated Program.cs.
                // In that case, part of default-painted border of MyBoxText is missing for some weird reason.
                // 
                // What's more peculiar, there seems no way to figure-out whether the application actually called 
                // SetCompatibleTextRenderingDefault(false) at all, at least I don't know any.
                // Because of this, the code below just always repaints the border.
                // For more info about SetCompatibleTextRenderingDefault see
                // http://blogs.msdn.com/b/jfoscoding/archive/2005/10/13/480632.aspx
                case (int)Win32.WM.WM_PAINT:
                    {
                        Rectangle rect = this.ClientRectangle;
                        Color clr = SystemColors.WindowFrame;
                        using Graphics g = this.CreateGraphics();
                        using (Pen penTmp = new(clr, 1))
                        {
                            rect.Height--;
                            g.DrawRectangle(penTmp, rect);
                        }
                    }
                    break;
            }
        }
    }
    #endregion // Typedefs

    #region Fields

    /// <summary>
    /// The timer which is used for implementation <see cref="ShowDelayed(int)"/> method.
    /// </summary>
    private System.Timers.Timer _timer;

    /// <summary>
    /// Backing field for the property <see cref="Margins"/>
    /// </summary>
    private Size _margins = new(4, 4);

    private readonly MyBoxText _textBox;
    private Nullable<Size> _mousePosOffsets;
    private int _reentrancyCheck;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default argument-less constructor.
    /// </summary>
    public PopupTooltip()
    {
        _textBox = new MyBoxText(this);
        Controls.Add(_textBox);
        this.Parent = null;
        this.CreateControl();
        this.InitializeComponent();
    }
    #endregion // Constructor(s)

    #region Methods

    /// <summary> Gets default text colors. </summary>
    ///
    /// <param name="drawHighlighted">  True if draw highlighted. </param>
    /// <param name="foreColor">        [out] The foreground color. </param>
    /// <param name="backColor">        [out] The back color. </param>
    protected internal static void GetDefaultTextColors(bool drawHighlighted, out Color foreColor, out Color backColor)
    {
        if (drawHighlighted)
        {
            foreColor = Color.FromKnownColor(KnownColor.HighlightText);
            backColor = Color.FromKnownColor(KnownColor.Highlight);
        }
        else
        {
            /* could utilize ToolTip class - but the results are quite the same
               using (var tooltip = new System.Windows.Forms.ToolTip())
               {
                   this.ForeColor = tooltip.ForeColor;
                   this.BackColor = tooltip.BackColor;
               }
            */
            foreColor = Color.FromKnownColor(KnownColor.InfoText);
            backColor = Color.FromKnownColor(KnownColor.Info);
        }
    }

    /// <summary> Gets text colors. Derived class may override this method. </summary>
    ///
    /// <param name="drawHighlighted">  True if draw highlighted. </param>
    /// <param name="foreColor">        [out] The foreground color. </param>
    /// <param name="backColor">        [out] The back color. </param>
    ///
    /// <returns>   True if it succeeds, false if it fails. </returns>
    protected virtual bool GetTextColors(bool drawHighlighted, out Color foreColor, out Color backColor)
    {
        GetDefaultTextColors(drawHighlighted, out foreColor, out backColor);
        return true;
    }

    /// <summary>
    /// Overwrites the implementation of the base class, in order to provide custom processing
    /// </summary>
    /// <param name="m">A Windows Message object.</param>
    protected override void WndProc(ref Message m)
    {
        bool bHandled = false;

        // preprocess the message
        switch (m.Msg)
        {
            // To make it transparent and prevent mouse clicks being sent to this window.
            // If the window returns HTTRANSPARENT, 
            // Windows doesn't send a WM_LBUTTONDOWN at all, but instead attempts 
            // to pass the mouse event to the first nontransparent window 
            // under the transparent one. 
            case (int)Win32.WM.WM_NCHITTEST:
                m.Result = new IntPtr((int)Win32.MousePositionCode.HTTRANSPARENT);
                bHandled = true;
                break;
        }

        // process the message
        if (!bHandled)
            base.WndProc(ref m);

        // post-process the message - nothing to do
    }

    /// <summary>
    /// Overwrite CreateParams to define the style, ExStyle etc.
    /// </summary>
    protected override CreateParams CreateParams
    {
        get
        {
            var createParams = new CreateParams
            {
                // Define the screen position and size
                X = Location.X,
                Y = Location.Y,
                Height = Size.Height,
                Width = Size.Width,

                // As a top-level window it has no parent
                Parent = IntPtr.Zero,

                // Appear as a pop-up window. And WS_EX_TOOLWINDOW style will hide it from the TaskBar.
                Style = unchecked((int)Win32.WindowStyles.WS_POPUP),
                ExStyle = (int)Win32.WindowExStyles.WS_EX_TOPMOST +
                (int)Win32.WindowExStyles.WS_EX_TOOLWINDOW
            };

            return createParams;
        }
    }

    /// <summary>
    /// Creates a timer <see cref="_timer "/> if has not been created yet, and sets its interval to
    /// <paramref name="msec"/>.
    /// </summary>
    ///
    /// <param name="msec"> The interval in milliseconds at which the timer will raise the Elapsed event. </param>
    /// <param name="mousePosOffsets"> The desired offset from mouse when eventually showing. </param>
    protected void SetTimer(int msec, Nullable<Size> mousePosOffsets)
    {
        if (_timer == null)
        {
            _timer = new System.Timers.Timer();
            _timer.Elapsed += new ElapsedEventHandler(OnTimer);
            _timer.Enabled = true;
        }
        _timer.Interval = msec;
        _mousePosOffsets = mousePosOffsets;
    }

    /// <summary>
    /// Disposes the <see cref="_timer"/> timer, if there is any.
    /// </summary>
    protected void KillTimer()
    {
        _mousePosOffsets = null;
        Disposer.SafeDispose(ref _timer);
    }

    /// <summary> Show the tooltip window, if it is not displayed already. </summary>
    ///
    /// <remarks>
    /// The implementation handles the case when this is not called from the UI thread but form a different one.
    /// This is needed since the method might be called from IPopupText.ShowDelayed; which could be called from system hook.
    /// </remarks>
    protected void ShowMe()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new MethodInvoker(ShowMe));
        }
        else
        {
            this.Visible = true;
            this.BringToFront();
            this.Refresh();
            this.Update();
            KillTimer();
        }
    }

    /// <summary> Raises the system. timers. elapsed event. 
    /// The event handler called when the timer <see cref="_timer "/> popped. 
    /// Will display myself and kill _timer.
    /// </summary>
    ///
    /// <param name="source"> Source for the event. </param>
    /// <param name="e">      Event information to send to registered event handlers. </param>
    protected void OnTimer(object source, ElapsedEventArgs e)
    {
        try
        {
            if (1 == Interlocked.Increment(ref _reentrancyCheck))
            {
                MoveByOffsetBeforeShowing();
                ShowMe();
                KillTimer();
            }
        }
        finally
        {
            Interlocked.Decrement(ref _reentrancyCheck);
        }
    }

    /// <summary>
    /// Overwrites the method of the base class, which raises the
    /// <see cref="System.Windows.Forms.Control.SizeChanged" /> event.
    /// This implementation performs some additional updates.
    /// </summary>
    ///
    /// <param name="e">  An <see cref="System.EventArgs" /> that contains the event data. </param>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        // let's set bounds, imho it is better 
        /* _textBox.Size = this.Size; */
        _textBox.Bounds = new Rectangle(0, 0, this.Size.Width, this.Size.Height);
    }

    /// <summary> Clean up any resources being used. </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            KillTimer();
        }
        base.Dispose(disposing);
    }

    private void MoveByOffsetBeforeShowing()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new MethodInvoker(MoveByOffsetBeforeShowing));
        }
        else
        {
            if (_mousePosOffsets.HasValue)
            {
                Point newLocation = Control.MousePosition + _mousePosOffsets.Value;
                this.Location = newLocation;
                _mousePosOffsets = null;
            }
        }
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.ResumeLayout(false);
    }
    #endregion // Methods

    #region IPopupText members

    /// <inheritdoc/>
    public bool IsVisible
    {
        get { return base.Visible; }
    }

    /// <inheritdoc/>
    public override string Text
    {
        get { return _textBox.Text; }
        set { _textBox.Text = value; }
    }

    /* property Font - not needed to override 
    public override Font Font 
    { 
      get { return base.Font;} 
      set { base.Font = value;} 
    }
    */

    /* property Size - not needed to override 
    public new Size Size
    {
        get { return base.Size;} 
        set { base.Size = value;} 
    }
    */

    /// <summary>
    /// Property margins ( actually are not supported yet ... )
    /// </summary>
    public Size Margins
    {
        get { return _margins; }
        set { _margins = value; }
    }

    /// <inheritdoc/>
    public bool DrawHighlighted
    {
        get { return _textBox.DrawHighlighted; }
        set { _textBox.DrawHighlighted = value; }
    }

    /// <inheritdoc/>
    public void ShowDelayed(int msec)
    {
        Debug.Assert(msec >= 0);
        if (msec == 0)
        {   // no delay: show it now
            ShowMe();
        }
        else
        {   // delay: set time
            SetTimer(msec, null);
        }
    }

    /// <inheritdoc/>
    public void ShowDelayed(int msec, Size mousePosOffset)
    {
        Debug.Assert(msec >= 0);
        if (msec == 0)
        {   // no delay: move and show it now
            MoveToWindow(null, Control.MousePosition + mousePosOffset);
            ShowMe();
        }
        else
        {   // delay: set time
            SetTimer(msec, mousePosOffset);
        }
    }

    /// <summary>
    /// Cancel the tooltip - kill _timer and hide window.
    /// </summary>
    public void Cancel()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new MethodInvoker(Cancel));
        }
        else
        {
            KillTimer();
            this.Visible = false;
        }
    }

    /// <inheritdoc/>
    public void MoveToWindow(IWin32Window iWnd, Point ptLocation)
    {
        if (iWnd == null)
        {
            this.Location = ptLocation;
        }
        else
        {
            Control ctrlParent = this.Parent;

            if (iWnd.Equals(ctrlParent))
            {  // the case when iWnd is my parent
                this.Location = ptLocation;
            }
            else if ((ctrlParent == null) || (ctrlParent.Handle == User32.GetParent(Handle)))
            { // The case of no parent, 
              // or when iWnd and me have the same parent
                Win32WindowHook.ClientToScreen(iWnd.Handle, ref ptLocation);

                /* DO NOT do the following, when the style is WindowStyles.WS_POPUP.
                * ( is set by override CreateParams ).
                * In that case, the location must stay in screen coordinates.
                * 
                * ptLocation = this.Parent.PointToClient(ptLocation); * 
                * */
                this.Location = ptLocation;
            }
            else
            {  // other cases are invalid
                Debug.Fail("Has been called with incorrect argument");
            }
        }
    }

    /// <inheritdoc/>
    public void MoveAsTopmost(Point ptLocation)
    {
        User32.SetWindowPos(this.Handle,
          (IntPtr)Win32.SWP_Vals.HWND_TOPMOST,
          0, 0, 0, 0,
          (uint)(Win32.SWP_Flags.SWP_NOACTIVATE |
                 Win32.SWP_Flags.SWP_NOMOVE |
                 Win32.SWP_Flags.SWP_NOSIZE |
                 Win32.SWP_Flags.SWP_NOREDRAW |
                 Win32.SWP_Flags.SWP_NOSENDCHANGING));

        MoveToWindow(null, ptLocation);
    }
    #endregion // IPopupText members
}

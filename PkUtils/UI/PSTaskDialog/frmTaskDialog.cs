///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// The Code Project Open License Notice
// 
// This software is a Derivative Work based upon a Code Project article
// Vista TaskDialog Wrapper and Emulator
// http://www.codeproject.com/Articles/21276/Vista-TaskDialog-Wrapper-and-Emulator
//
// The Code Project Open License (CPOL) text is available at
// http://www.codeproject.com/info/cpol10.aspx
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Ignore Spelling: Utils, frm, Img
//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PK.PkUtils.UI.PSTaskDialog;

/// <summary>
/// The form emulating the TaskDialog (on pre-Vista systems, or if emulation is enforced ). 
/// </summary>
public partial class frmTaskDialog : Form
{
    #region Private fields

    private eSysIcons _mainIcon = eSysIcons.Question;
    private eSysIcons _footerIcon = eSysIcons.Warning;

    private string _mainInstruction = "Main Instruction Text";
    private int _mainInstructionHeight;
    private Font _mainInstructionFont = new("Arial", 11.75F, FontStyle.Regular, GraphicsUnit.Point, 0);

    private readonly List<RadioButton> _radioButtonCtrls = [];
    private string _radioButtons = string.Empty;

    /* PetrK 07/08/2013: commented-out as not used for anything
    private int _initialRadioButtonIndex; */

    /* PetrK 04/24/2012: commented-out as not used for anything
    private List<Button> _cmdButtons = new List<Button>(); */

    private string _commandButtons = string.Empty;
    private int _commandButtonClicked = -1;

    private int _defaultButtonIndex;
    private Control _focusControl;

    private eTaskDialogButtons _Buttons = eTaskDialogButtons.YesNoCancel;

    private bool _Expanded;
    private readonly bool _isVista;
    private bool _formBuilt;

    private const int MAIN_INSTRUCTION_LEFT_MARGIN = 46;
    private const int MAIN_INSTRUCTION_RIGHT_MARGIN = 8;
    #endregion // Private fields

    #region Constructor(s)

    /// <summary>
    /// The default argument-less constructor.
    /// </summary>
    public frmTaskDialog()
    {
        InitializeComponent();

        _isVista = VistaTaskDialog.IsAvailableOnThisOS;
        if (!_isVista && cTaskDialog.UseToolWindowOnXP) // <- shall we use the smaller toolbar?
        {
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        MainInstruction = "Main Instruction";
        Content = "";
        ExpandedInfo = "";
        Footer = "";
        VerificationText = "";
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// The kind of main icon being used for the Task Dialog.
    /// </summary>
    public eSysIcons MainIcon
    {
        get { return _mainIcon; }
        set { _mainIcon = value; }
    }

    /// <summary>
    /// The kind of icon displayed in the footer of the Task Dialog.
    /// </summary>
    public eSysIcons FooterIcon
    {
        get { return _footerIcon; }
        set { _footerIcon = value; }
    }

    /// <summary>
    /// The dialog Title ( the text in the Title bar ).
    /// </summary>
    public string Title
    {
        get { return this.Text; }
        set { this.Text = value; }
    }

    /// <summary>
    /// The main instruction being displayed in the dialog.
    /// </summary>
    public string MainInstruction
    {
        get { return _mainInstruction; }
        set { _mainInstruction = value; this.Invalidate(); }
    }

    /// <summary>
    /// The content text for the Task Dialog.
    /// </summary>
    public string Content
    {
        get { return lbContent.Text; }
        set { lbContent.Text = value; }
    }

    /// <summary>
    /// The Task Dialog expanded content text (it will be shown in the dialog after clicking on the arrow expand).
    /// </summary>
    public string ExpandedInfo
    {
        get { return lbExpandedInfo.Text; }
        set { lbExpandedInfo.Text = value; }
    }

    /// <summary>
    /// The footer text, displayed in the bottom of Task Dialog.
    /// </summary>
    public string Footer
    {
        get { return lbFooter.Text; }
        set { lbFooter.Text = value; }
    }

    /// <summary>
    /// Indicates the default (initial) radio button being selected in the Task Dialog.
    /// </summary>
    public int DefaultButtonIndex
    {
        get { return _defaultButtonIndex; }
        set { _defaultButtonIndex = value; }
    }

    /// <summary>
    /// Texts of individual command buttons, separated by '|' character. <br/>
    /// An example of string consisting of texts of three command buttons:<br/>
    /// <i>"Radio Option 1|Radio Option 2|Radio Option 3"</i>
    /// </summary>
    public string RadioButtons
    {
        get { return _radioButtons; }
        set { _radioButtons = value; }
    }

    // PetrK 07/08/2013: commented-out as not used for anything
    /* 
    public int InitialRadioButtonIndex
    {
      get { return _initialRadioButtonIndex; } 
      set { _initialRadioButtonIndex = value; } 
    }
    */

    /// <summary>
    /// Returns the index of checked radio button ( if there is any ).
    /// </summary>
    public int RadioButtonIndex
    {
        get
        {
            foreach (RadioButton rb in _radioButtonCtrls)
            {
                if (rb.Checked)
                {
                    return (int)rb.Tag;
                }
            }
            return -1;
        }
    }

    /// <summary>
    /// Texts of individual command buttons, separated by '|' character. <br/>
    /// An example of string consisting of texts of three command buttons:<br/>
    /// <i>"Command &amp;Button 1|Command Button 2\nLine 2\nLine 3|Command Button 3"</i>
    /// </summary>
    public string CommandButtons
    {
        get { return _commandButtons; }
        set { _commandButtons = value; }
    }

    /// <summary>
    /// The index of command button clicked that caused closing the dialog (if any).
    /// </summary>
    public int CommandButtonClickedIndex
    {
        get { return _commandButtonClicked; }
    }

    /// <summary>
    /// The set of displayed <see cref="eTaskDialogButtons"/> buttons, combined into bitmask.
    /// </summary>
    public eTaskDialogButtons Buttons
    {
        get { return _Buttons; }
        set { _Buttons = value; }
    }

    /// <summary>
    /// The string to be used to label the verification checkbox. If this member is null, the
    /// verification checkbox is not displayed in the dialog box.
    /// </summary>
    public string VerificationText
    {
        get { return cbVerify.Text; }
        set { cbVerify.Text = value; }
    }

    /// <summary>
    /// Is the verification checkbox checked?
    /// </summary>
    public bool VerificationCheckBoxChecked
    {
        get { return cbVerify.Checked; }
        set { cbVerify.Checked = value; }
    }

    /// <summary>
    /// Is the dialog in expanded state.
    /// </summary>
    public bool Expanded
    {
        get { return _Expanded; }
        set { _Expanded = value; }
    }
    #endregion // Properties

    #region Methods

    #region Public Methods

    /// <summary>
    /// This is the main routine that should be called before .ShowDialog()
    /// </summary>
    public void BuildForm()
    {
        int for_height = 0;

        // Setup Main Instruction
        switch (MainIcon)
        {
            case eSysIcons.Information:
                imgMain.Image = SystemIcons.Information.ToBitmap();
                break;
            case eSysIcons.Question:
                imgMain.Image = SystemIcons.Question.ToBitmap();
                break;
            case eSysIcons.Warning:
                imgMain.Image = SystemIcons.Warning.ToBitmap();
                break;
            case eSysIcons.Error:
                imgMain.Image = SystemIcons.Error.ToBitmap();
                break;
        }

        //AdjustLabelHeight(lbMainInstruction);
        //pnlMainInstruction.Height = Math.Max(41, lbMainInstruction.Height + 16);
        if (_mainInstructionHeight == 0)
        {
            GetMainInstructionTextSizeF();
        }
        pnlMainInstruction.Height = Math.Max(41, _mainInstructionHeight + 16);

        for_height += pnlMainInstruction.Height;

        // Setup Content
        if (pnlContent.Visible = !string.IsNullOrEmpty(Content))
        {
            AdjustLabelHeight(lbContent);
            pnlContent.Height = lbContent.Height + 4;
            for_height += pnlContent.Height;
        }

        bool show_verify_checkbox = !string.IsNullOrEmpty(cbVerify.Text);
        cbVerify.Visible = show_verify_checkbox;

        // Setup Expanded Info and Buttons panels
        if (string.IsNullOrEmpty(ExpandedInfo))
        {
            pnlExpandedInfo.Visible = false;
            lbShowHideDetails.Visible = false;
            cbVerify.Top = 12;
            pnlButtons.Height = 40;
        }
        else
        {
            AdjustLabelHeight(lbExpandedInfo);
            pnlExpandedInfo.Height = lbExpandedInfo.Height + 4;
            pnlExpandedInfo.Visible = _Expanded;
            lbShowHideDetails.Text = (_Expanded ? "        Hide details" : "        Show details");
            lbShowHideDetails.ImageIndex = (_Expanded ? 0 : 3);
            if (!show_verify_checkbox)
                pnlButtons.Height = 40;
            if (_Expanded)
                for_height += pnlExpandedInfo.Height;
        }

        // Setup RadioButtons
        if (pnlRadioButtons.Visible = !string.IsNullOrEmpty(_radioButtons))
        {
            string[] arr = _radioButtons.Split(['|']);
            int pnl_height = 12;
            for (int i = 0; i < arr.Length; i++)
            {
                RadioButton rb = new()
                {
                    Parent = pnlRadioButtons
                };
                rb.Location = new Point(60, 4 + (i * rb.Height));
                rb.Text = arr[i];
                rb.Tag = i;
                rb.Checked = (_defaultButtonIndex == i);
                rb.Width = this.Width - rb.Left - 15;
                pnl_height += rb.Height;
                _radioButtonCtrls.Add(rb);
            }
            pnlRadioButtons.Height = pnl_height;
            for_height += pnlRadioButtons.Height;
        }

        // Setup CommandButtons
        if (pnlCommandButtons.Visible = !string.IsNullOrEmpty(_commandButtons))
        {
            string[] arr = _commandButtons.Split(['|']);
            int t = 8;
            int pnl_height = 16;
            for (int i = 0; i < arr.Length; i++)
            {
                CommandButton btn = new()
                {
                    Parent = pnlCommandButtons,
                    Location = new Point(50, t)
                };
                if (_isVista)  // <- tweak font if vista
                    btn.Font = new Font(btn.Font, FontStyle.Regular);
                btn.Text = arr[i];
                btn.Size = new Size(this.Width - btn.Left - 15, btn.GetBestHeight());
                t += btn.Height;
                pnl_height += btn.Height;
                btn.Tag = i;
                btn.Click += new EventHandler(CommandButton_Click);
                if (i == _defaultButtonIndex)
                    _focusControl = btn;
            }
            pnlCommandButtons.Height = pnl_height;
            for_height += pnlCommandButtons.Height;
        }

        // Setup Buttons
        switch (_Buttons)
        {
            case eTaskDialogButtons.YesNo:
                bt1.Visible = false;
                bt2.Text = "&Yes";
                bt2.DialogResult = DialogResult.Yes;
                bt3.Text = "&No";
                bt3.DialogResult = DialogResult.No;
                this.AcceptButton = bt2;
                this.CancelButton = bt3;
                break;
            case eTaskDialogButtons.YesNoCancel:
                bt1.Text = "&Yes";
                bt1.DialogResult = DialogResult.Yes;
                bt2.Text = "&No";
                bt2.DialogResult = DialogResult.No;
                bt3.Text = "&Cancel";
                bt3.DialogResult = DialogResult.Cancel;
                this.AcceptButton = bt1;
                this.CancelButton = bt3;
                break;
            case eTaskDialogButtons.OKCancel:
                bt1.Visible = false;
                bt2.Text = "&OK";
                bt2.DialogResult = DialogResult.OK;
                bt3.Text = "&Cancel";
                bt3.DialogResult = DialogResult.Cancel;
                this.AcceptButton = bt2;
                this.CancelButton = bt3;
                break;
            case eTaskDialogButtons.OK:
                bt1.Visible = false;
                bt2.Visible = false;
                bt3.Text = "&OK";
                bt3.DialogResult = DialogResult.OK;
                this.AcceptButton = bt3;
                this.CancelButton = bt3;
                break;
            case eTaskDialogButtons.Close:
                bt1.Visible = false;
                bt2.Visible = false;
                bt3.Text = "&Close";
                bt3.DialogResult = DialogResult.Cancel;
                this.CancelButton = bt3;
                break;
            case eTaskDialogButtons.Cancel:
                bt1.Visible = false;
                bt2.Visible = false;
                bt3.Text = "&Cancel";
                bt3.DialogResult = DialogResult.Cancel;
                this.CancelButton = bt3;
                break;
            case eTaskDialogButtons.None:
                bt1.Visible = false;
                bt2.Visible = false;
                bt3.Visible = false;
                break;
        }

        this.ControlBox = (Buttons == eTaskDialogButtons.Cancel ||
                           Buttons == eTaskDialogButtons.Close ||
                           Buttons == eTaskDialogButtons.OKCancel ||
                           Buttons == eTaskDialogButtons.YesNoCancel);

        if (!show_verify_checkbox && string.IsNullOrEmpty(ExpandedInfo) && _Buttons == eTaskDialogButtons.None)
            pnlButtons.Visible = false;
        else
            for_height += pnlButtons.Height;

        if (pnlFooter.Visible = !string.IsNullOrEmpty(Footer))
        {
            AdjustLabelHeight(lbFooter);
            pnlFooter.Height = Math.Max(28, lbFooter.Height + 16);

            switch (FooterIcon)
            {
                case eSysIcons.Information:
                    // SystemIcons.Information.ToBitmap().GetThumbnailImage(16, 16, null, IntPtr.Zero);
                    imgFooter.Image = ResizeBitmap(SystemIcons.Information.ToBitmap(), 16, 16);
                    break;
                case eSysIcons.Question:
                    // SystemIcons.Question.ToBitmap().GetThumbnailImage(16, 16, null, IntPtr.Zero);
                    imgFooter.Image = ResizeBitmap(SystemIcons.Question.ToBitmap(), 16, 16);
                    break;
                case eSysIcons.Warning:
                    // SystemIcons.Warning.ToBitmap().GetThumbnailImage(16, 16, null, IntPtr.Zero);
                    imgFooter.Image = ResizeBitmap(SystemIcons.Warning.ToBitmap(), 16, 16);
                    break;
                case eSysIcons.Error:
                    // SystemIcons.Error.ToBitmap().GetThumbnailImage(16, 16, null, IntPtr.Zero);
                    imgFooter.Image = ResizeBitmap(SystemIcons.Error.ToBitmap(), 16, 16);
                    break;
            }
            for_height += pnlFooter.Height;
        }

        this.ClientSize = new Size(ClientSize.Width, for_height);

        _formBuilt = true;
    }
    #endregion // Public Methods

    #region Protected Overrides

    /// <summary> Overwrites the implementation of the base class, to provide additional processing.
    /// In more detail, before the base class implementation is called, checks whether <see cref="BuildForm"/>
    /// has been called at all.<br/>
    /// The implementation of the base class raises the <see cref="E:System.Windows.Forms.Form.Shown" /> event. </summary>
    ///
    /// <exception cref="InvalidOperationException">  Thrown when the requested operation is invalid. </exception>
    ///
    /// <param name="args">  A <see cref="T:System.EventArgs" /> that contains the event data. </param>
    protected override void OnShown(EventArgs args)
    {
        if (!_formBuilt)
        {
            throw new InvalidOperationException("frmTaskDialog : Please call .BuildForm() before showing the TaskDialog");
        }
        base.OnShown(args);
    }

    /* PetrK 04/24/2012: commented-out as not needed
    protected override void OnLoad(EventArgs args)
    {
      base.OnLoad(e);
    }
    */

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (components != null)
            {
                components.Dispose();
                components = null;
            }
            if (_mainInstructionFont != null)
            {
                _mainInstructionFont.Dispose();
                _mainInstructionFont = null;
            }
        }
        base.Dispose(disposing);
    }
    #endregion // Protected Overrides

    #region Protected Utilities

    /// <summary>
    /// Resizes the input image and returns the image with a new size.
    /// </summary>
    /// <param name="SrcImg">The input image</param>
    /// <param name="NewWidth">A new width of image</param>
    /// <param name="NewHeight">A new height of image</param>
    /// <returns>A new image with required width and height.</returns>
    protected static Image ResizeBitmap(Image SrcImg, int NewWidth, int NewHeight)
    {
        float percent_width = (NewWidth / (float)SrcImg.Width);
        float percent_height = (NewHeight / (float)SrcImg.Height);

        float resize_percent = (percent_height < percent_width ? percent_height : percent_width);

        int w = (int)(SrcImg.Width * resize_percent);
        int h = (int)(SrcImg.Height * resize_percent);
        Bitmap b = new(w, h);

        using (Graphics g = Graphics.FromImage(b))
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(SrcImg, 0, 0, w, h);
        }
        return b;
    }

    /// <summary>
    /// utility function for setting a Label's height
    /// </summary>
    /// <param name="lb">The control being adjusted.</param>
    protected static void AdjustLabelHeight(Control lb)
    {
        string text = lb.Text;
        Font textFont = lb.Font;
        SizeF layoutSize = new(lb.ClientSize.Width, 5000.0F);

        Graphics g = Graphics.FromHwnd(lb.Handle);
        SizeF stringSize = g.MeasureString(text, textFont, layoutSize);
        lb.Height = (int)stringSize.Height + 4;
    }

    /// <summary>
    /// Auxiliary method getting the text size of the main instruction text 
    /// ( which is represented by property <see cref="MainInstruction"/> ).
    /// </summary>
    /// <returns>This method returns a SizeF structure that represents the size, 
    /// (in the units specified by the PageUnit property), of the main instruction text string.<br/>
    /// For more details, see the
    /// <see href="http://msdn.microsoft.com/en-us/library/6xe5hazb(v=vs.90).aspx"> Graphics.MeasureString </see>
    /// method description.
    /// </returns>
    protected SizeF GetMainInstructionTextSizeF()
    {
        SizeF mzSize = new(pnlMainInstruction.Width - MAIN_INSTRUCTION_LEFT_MARGIN - MAIN_INSTRUCTION_RIGHT_MARGIN, 5000.0F);
        Graphics g = Graphics.FromHwnd(this.Handle);
        SizeF textSize = g.MeasureString(_mainInstruction, _mainInstructionFont, mzSize);
        _mainInstructionHeight = (int)textSize.Height;
        return textSize;
    }
    #endregion // Protected Utilities
    #endregion // Methods

    #region Event Handlers

    private void CommandButton_Click(object sender, EventArgs args)
    {
        _commandButtonClicked = (int)((CommandButton)sender).Tag;
        this.DialogResult = DialogResult.OK;
    }

    private void LbDetails_MouseEnter(object sender, EventArgs args)
    {
        lbShowHideDetails.ImageIndex = (_Expanded ? 1 : 4);
    }

    private void LbDetails_MouseLeave(object sender, EventArgs args)
    {
        lbShowHideDetails.ImageIndex = (_Expanded ? 0 : 3);
    }

    private void LbDetails_MouseUp(object sender, MouseEventArgs args)
    {
        lbShowHideDetails.ImageIndex = (_Expanded ? 1 : 4);
    }

    private void LbDetails_MouseDown(object sender, MouseEventArgs args)
    {
        lbShowHideDetails.ImageIndex = (_Expanded ? 2 : 5);
    }

    private void LbDetails_Click(object sender, EventArgs args)
    {
        _Expanded = !_Expanded;
        pnlExpandedInfo.Visible = _Expanded;
        lbShowHideDetails.Text = (_Expanded ? "        Hide details" : "        Show details");
        if (_Expanded)
            this.Height += pnlExpandedInfo.Height;
        else
            this.Height -= pnlExpandedInfo.Height;
    }

    private void PnlMainInstruction_Paint(object sender, PaintEventArgs args)
    {
        SizeF szL = GetMainInstructionTextSizeF();
        args.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        args.Graphics.DrawString(
          _mainInstruction,
          _mainInstructionFont,
          new SolidBrush(Color.DarkBlue),
          new RectangleF(new PointF(MAIN_INSTRUCTION_LEFT_MARGIN, 10), szL));
    }

    private void FrmTaskDialog_Shown(object sender, EventArgs args)
    {
        if (cTaskDialog.PlaySystemSounds)
        {
            switch (MainIcon)
            {
                case eSysIcons.Error:
                    System.Media.SystemSounds.Hand.Play();
                    break;
                case eSysIcons.Information:
                    System.Media.SystemSounds.Asterisk.Play();
                    break;
                case eSysIcons.Question:
                    System.Media.SystemSounds.Asterisk.Play();
                    break;
                case eSysIcons.Warning:
                    System.Media.SystemSounds.Exclamation.Play();
                    break;
            }
        }
        _focusControl?.Focus();
    }
    #endregion // Event Handlers
}

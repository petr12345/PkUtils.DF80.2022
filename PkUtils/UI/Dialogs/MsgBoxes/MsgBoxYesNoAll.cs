// Ignore Spelling: Utils, endregion
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PK.PkUtils.UI.General;

/// <summary> 
/// A support for a message box with buttons / yes / no / yes to all / no to all / cancel. 
/// The implementation of layout uses a custom resizing ( without usage of TablelayoutPanel ).
/// </summary>
public partial class MsgBoxYesNoAll : Form
{
    #region Typedefs

    /// <summary>
    /// The result of this dialog
    /// </summary>
    public enum MsgBoxResult
    {
        /// <summary> An enum constant representing the yes option. </summary>
        Yes,
        /// <summary> An enum constant representing the yes to all option. </summary>
        YesToAll,
        /// <summary> An enum constant representing the no option. </summary>
        No,
        /// <summary> An enum constant representing the no to all option. </summary>
        NoToAll,
        /// <summary> An enum constant representing the cancel option. </summary>
        Cancel
    }
    #endregion // Typedefs

    #region Private Fields

    /// <summary> The minimum width ( cached value retrieved in constructor). </summary>
    private readonly int _minWidth;

    /// <summary> The minimum height ( cached value retrieved in constructor). </summary>
    private readonly int _minHeight;

    /// <summary> The initial height ( cached value retrieved in constructor). </summary>
    private readonly int _initHeight;

    /// <summary> Initial height of _lblMainInstruction </summary>
    private readonly int _initMainHeight;

    /// <summary> Initial height of _lblContent</summary>
    private readonly int _initContentHeight;

    /// <summary> The last location of form. </summary>
    private Nullable<Point> _lastLocation;

    /// <summary> A backing field of property <see cref="ButtonTextsSpecified"/>.</summary>
    private readonly IEnumerable<KeyValuePair<MsgBoxResult, string>> _buttonTexts;

    /// <summary> The backing field of property <see cref="Result"/>. </summary> 
    private MsgBoxResult _result = MsgBoxResult.Cancel;

    /// <summary> The backing field for property <see cref="StandardIcon"/>. </summary>
    private MessageBoxIcon _standardIcon = MessageBoxIcon.None;

    /// <summary> A backing field for property <see cref="ResultToButtonsDict"/>. </summary>
    private IReadOnlyDictionary<MsgBoxResult, Button> _resultToButtonsDictionary;
    #endregion // Private Fields

    #region Constructor(s)

    /// <summary> Default argument-less constructor. </summary>
    public MsgBoxYesNoAll()
    {
        InitializeComponent();

        _minWidth = MinimumSize.Width;
        _minHeight = MinimumSize.Height;
        _initHeight = this.Height;
        _initMainHeight = _lblMainInstruction.Height;
        _initContentHeight = _lblContent.Height;
    }

    /// <summary> Public constructor. </summary>
    ///
    /// <param name="buttonTexts">  Button texts provided by caller; may be null.
    ///  If this argument is not null, text of individual buttons matching the given key
    ///  will be changed to provided value.</param>
    public MsgBoxYesNoAll(IReadOnlyDictionary<MsgBoxResult, string> buttonTexts)
      : this()
    {
        _buttonTexts = buttonTexts;
        UpdateButtonsTexts();
        UpdateButtonsVisibility();
    }
    #endregion // Constructor(s)

    #region Properties
    #region Public Properties

    /// <summary>
    /// A main instruction text ( text of label _lblMainInstruction, which is the form child control )
    /// </summary>
    public string MainInstructionText
    {
        get { return this._lblMainInstruction.Text; }
        set { this._lblMainInstruction.Text = value; }
    }

    /// <summary>
    /// A content text ( text of label _lblContent, which is the form child control )
    /// </summary>
    public string ContentText
    {
        get { return this._lblContent.Text; }
        set { this._lblContent.Text = value; }
    }

    /// <summary> Gets or sets the standard icon displayed in the PictureBox. </summary>
    public MessageBoxIcon StandardIcon
    {
        get { return _standardIcon; }
        set
        {
            if (StandardIcon != value)
            {
                _standardIcon = value;
                UpdateIconImage();
                UpdateSize();
            }
        }
    }

    /// <summary>
    /// The result of most recent ShowDialog or ShowDialogEx call.
    /// </summary>
    public MsgBoxResult Result { get => this._result; }
    #endregion // Public Properties

    #region Private Properties

    /// <summary> Button texts provided by constructor (if any). May be null.</summary>
    private IEnumerable<KeyValuePair<MsgBoxResult, string>> ButtonTextsSpecified
    {
        get { return _buttonTexts; }
    }

    /// <summary>
    /// Returns a dictionary, representing the relationship (MsgBoxResult --- > Button related).
    /// </summary>
    /// <remarks>
    /// Instead of usage of field initializer of backing field, the code uses the lazy initialization.
    /// The reason is the former approach produces a compilation error
    /// ( "A field initializer cannot reference the non-static field, method, or property ").
    /// </remarks>
    private IReadOnlyDictionary<MsgBoxResult, Button> ResultToButtonsDict
    {
        get
        {
            _resultToButtonsDictionary ??= new Dictionary<MsgBoxResult, Button>
                {
                    { MsgBoxResult.Yes, this._btnYes },
                    { MsgBoxResult.YesToAll, this._btnYesToAll },
                    { MsgBoxResult.No, this._btnNo },
                    { MsgBoxResult.NoToAll, this._btnNoToAll},
                    { MsgBoxResult.Cancel, this._btnCancel},
                };
            return _resultToButtonsDictionary;
        }
    }
    #endregion // Private Properties
    #endregion // Properties

    #region Methods
    #region Public Methods

    /// <summary>
    /// A helper method calling ShowDialog internally.  You should call this method instead of ShowDialog, 
    /// to get <see cref="MsgBoxResult "/> instead of DialogResult.
    /// </summary>
    ///
    /// <param name="owner"> Any object that implements IWin32Window that represents the top-level window 
    ///                       that will own the modal dialog box. May be null. </param>
    /// <returns> A MsgBoxResult reflecting the button clicked. </returns>
    public MsgBoxResult ShowDialogEx(IWin32Window owner)
    {
        _result = MsgBoxResult.Cancel;

        ShowDialog(owner);
        return Result;
    }
    /// <summary>
    /// A helper method calling ShowDialog internally.  You should call this method instead of ShowDialog, to get
    /// <see cref="MsgBoxResult"/> instead of   <see cref="DialogResult"/>
    /// </summary>
    /// 
    /// <param name="owner"> Any object that implements IWin32Window that represents the top-level window 
    ///                       that will own the modal dialog box. May be null. </param>
    /// <param name="mainInstruction">  The main instruction. </param>
    /// <param name="content">         A string that specifies the content to display ( below the main
    /// instruction).  
    /// The value will be assigned to  <see cref="ContentText"/> property. </param>
    /// <param name="caption">          A string that specifies the title bar caption to display. </param>
    /// <param name="icon">             The icon. </param>
    ///
    /// <returns> A MsgBoxResult reflecting the used button clicked. </returns>
    public MsgBoxResult ShowDialogEx(
        IWin32Window owner,
        string mainInstruction,
        string content,
        string caption,
        MessageBoxIcon icon)
    {
        this.StandardIcon = icon;
        this.MainInstructionText = mainInstruction;
        this.ContentText = content;
        this.Text = caption;

        return ShowDialogEx(owner);
    }
    #endregion // Public Methods

    #region Protected Methods

    /// <summary> Overrides the virtual method of the base class, 
    /// which raises the <see cref="Form.Load" /> event. 
    /// Performs additional initialization for which calling int from constructor is either too early,
    /// or if we need to guarantee it is called each time the ShowDialog is called ( if reusing the instance 
    /// of Form for multiple ShowDialog calls).
    /// </summary>
    ///
    /// <param name="args">  An <see cref="EventArgs" /> that contains the event data. </param>
    protected override void OnLoad(EventArgs args)
    {
        base.OnLoad(args);

        DialogResult = DialogResult.None;
        _result = MsgBoxResult.Cancel;

        if (_lastLocation != null)
            this.Location = _lastLocation.Value;
        UpdateSize();
    }

    /// <summary> Raises the <see cref="Form.Closing" /> event. </summary>
    /// <param name="args"> A <see cref="CancelEventArgs" /> that contains the event data. </param>
    protected override void OnClosing(CancelEventArgs args)
    {
        base.OnClosing(args);
        _lastLocation = this.Location;
    }
    #endregion // Protected Methods

    #region Private Methods

    /// <summary> Updates texts of individual buttons, based on data provided to constructor (if any). </summary>
    private void UpdateButtonsTexts()
    {
        if (null != ButtonTextsSpecified)
        {
            foreach (KeyValuePair<MsgBoxResult, string> pair in ButtonTextsSpecified)
            {
                Debug.Assert(ResultToButtonsDict.ContainsKey(pair.Key));
                ResultToButtonsDict[pair.Key].Text = pair.Value;
            }
        }
    }

    /// <summary>	Updates the buttons visibility. </summary>
    private void UpdateButtonsVisibility()
    {
        foreach (Button button in ResultToButtonsDict.Values)
        {
            bool bVisible = !string.IsNullOrEmpty(button.Text);
            button.Visible = bVisible;
        }
    }

    /// <summary>
    /// This call updates the size of the window based on certain factors,
    /// such as if an icon is present, and the size of label.
    /// </summary>
    private void UpdateSize()
    {
        int nPictureWidth, newWidth, newHeight;
        int nDeltaMainheight, nDeltaContentHeight;
        int nOffsetX = 0;

        // compute basis for new width
        newWidth = _lblContent.Size.Width + 40;

        // add the width of the icon, and some padding.
        if (_pbxIcon.Image != null)
        {
            nPictureWidth = Math.Min(_pbxIcon.Width, _pbxIcon.Image.Width);
            newWidth += (nOffsetX = nPictureWidth + 20);
        }
        _lblContent.Location = new Point(_pbxIcon.Location.X + nOffsetX, _lblContent.Location.Y);
        _lblMainInstruction.Location = new Point(_lblContent.Location.X, _lblMainInstruction.Location.Y);

        // increase new width if computed is too small
        newWidth = Math.Max(newWidth, this._minWidth);

        // compute new height
        nDeltaMainheight = Math.Max(0, _lblMainInstruction.Size.Height - _initMainHeight);
        nDeltaContentHeight = Math.Max(0, _lblContent.Size.Height - 4 * _initContentHeight);
        newHeight = Math.Max(_initHeight + nDeltaMainheight + nDeltaContentHeight, _minHeight);
        // apply all that
        this.Size = new Size(newWidth, newHeight);
    }

    /// <summary>
    /// Updates the _pbxIcon .Imag, based on current <see cref="StandardIcon"/> property value.
    /// </summary>
    private void UpdateIconImage()
    {
        Icon iconImage = null;

        switch (StandardIcon)
        {
            /*
            case MessageBoxIcon.None:
              iconImage = null;  already is
              break;
             */
            case MessageBoxIcon.Error:
                /* case MessageBoxIcon.Hand:  is the same  */
                iconImage = SystemIcons.Error;
                break;
            case MessageBoxIcon.Question:
                iconImage = SystemIcons.Question;
                break;
            case MessageBoxIcon.Exclamation:
                iconImage = SystemIcons.Exclamation;
                break;
            case MessageBoxIcon.Information:
                /* case MessageBoxIcon.Asterisk:  is the same  */
                iconImage = SystemIcons.Asterisk;
                break;
        }
        _pbxIcon.Image = iconImage?.ToBitmap();
    }
    #endregion // Private Methods
    #endregion // Methods

    #region Event Handlers

    /// <summary> The event handler called by _lblMainInstruction.TextChanged 
    ///           or _lblContent.TextChanged event. </summary>
    /// <param name="sender"> The sender. </param>
    /// <param name="args"> Event information. </param>
    private void AnyLabel_TextChanged(object sender, EventArgs args)
    {
        UpdateSize();
    }

    /// <summary> The Event handler for any button being clicked.</summary>
    /// <param name="sender"> The sender. </param>
    /// <param name="args"> Event information. </param>
    private void OnBtn_Click(object sender, EventArgs args)
    {
        Button btn = (Button)sender; // raise an exception if the sender is not a button

        Debug.Assert(btn.DialogResult == DialogResult.Yes || btn.DialogResult == DialogResult.No
          || btn.DialogResult == DialogResult.Cancel);
        this.DialogResult = btn.DialogResult;
        this._result = ResultToButtonsDict.First(pair => pair.Value == btn).Key;
    }
    #endregion // Event Handlers
}
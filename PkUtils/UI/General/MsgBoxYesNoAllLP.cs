/***************************************************************************************************************
*
* FILE NAME:   .\UI\General\MsgBoxYesNoAllLP.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class MsgBoxYesNoAllLP 
*
**************************************************************************************************************/

// Ignore Spelling: Utils
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
/// The implementation of layout uses a TablelayoutPanel.
/// </summary>
/// 
/// <remarks>
/// Important designer code to allow the panel grow/shrink properly
/// <code language="xml" title="a code fragments">
/// <![CDATA[
/// 
/// this._lblContent.AutoSize = true;
/// this._lblContent.Dock = System.Windows.Forms.DockStyle.Fill;
/// this._lblContent.Size = new System.Drawing.Size(350, 90);
/// this._lblContent.MinimumSize = new System.Drawing.Size(350, 0);
/// 
/// this._lblMainInstruction.AutoSize = true;
/// this._lblMainInstruction.Dock = System.Windows.Forms.DockStyle.Fill;
/// this._lblMainInstruction.Size = new System.Drawing.Size(350, 20);
/// this._lblMainInstruction.MinimumSize = new System.Drawing.Size(350, 0);
/// 
///  this._TableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
///     | System.Windows.Forms.AnchorStyles.Right)));
/// this._TableLayoutPanel.AutoSize = true;
/// this._TableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
/// this._TableLayoutPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
/// 
/// ]]>
/// </code>
/// </remarks>
/// 
/// <seealso href="http://www.c-sharpcorner.com/uploadfile/mahesh/tablelayoutpanel-in-C-Sharp1/">
/// TableLayoutPanel in C#</seealso>
/// 
/// <seealso href="http://stackoverflow.com/questions/3804610/making-tablelayoutpanel-act-like-an-html-table-cells-that-resize-automatically">
/// Making TableLayoutPanel Act Like An HTML Table (Cells That Resize Automatically Around Text)</seealso>
/// 
/// <seealso href="http://stackoverflow.com/questions/20797446/table-layout-panel-not-resizing">
/// Table layout panel not resizing -  > Check out the Dock property. Setting it to Fill will make the child control to fill the parent.</seealso>
/// 
/// <seealso href="http://stackoverflow.com/questions/3290414/hide-and-show-a-cell-of-the-tablelayoutpanel">
/// Hide and show a cell of the TableLayoutPanel</seealso>
/// 
public partial class MsgBoxYesNoAllLP : Form
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

    #region Public Interface

    #region Constructor(s)

    /// <summary>
    /// Default argument-less constructor
    /// </summary>
    public MsgBoxYesNoAllLP()
    {
        InitializeComponent();

        _initSize = this.Size;
        _initPanelSize = this._TableLayoutPanel.Size;
    }

    /// <summary> Public constructor. </summary>
    ///
    /// <param name="buttonTexts">  Button texts provided by caller; may be null. 
    ///  If this argument is not null, text of individual buttons matching the given key
    ///  will be changed to provided value.</param>
    public MsgBoxYesNoAllLP(IReadOnlyDictionary<MsgBoxResult, string> buttonTexts)
      : this()
    {
        _buttonTexts = buttonTexts;
        UpdateButtonsTexts();
        UpdateButtonsVisibility();
    }
    #endregion // Constructor(s)

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
        get { return _StandardIcon; }
        set
        {
            if (StandardIcon != value)
            {
                _StandardIcon = value;
                UpdateIconImage();
                UpdateSize(true);
            }
        }
    }

    /// <summary>
    /// The result of most recent ShowDialog or ShowDialogEx call.
    /// </summary>
    public MsgBoxResult Result
    {
        get { return this._Result; }
    }
    #endregion // Public Properties

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
        _Result = MsgBoxResult.Cancel;

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
    #endregion // Public Interface

    #region Protected Interface

    /// <summary>
    /// Should be table cell emphasized
    /// </summary>
    protected bool EmphasizeCells
    {
        get { return _EmphasizeCells; }
    }

    /// <summary> Updates the cell emphasize properties. </summary>
    ///
    /// <param name="updateSize"> true to consequently update size. </param>
    protected void UpdateCellEmphasizeProperties(bool updateSize)
    {
        if (EmphasizeCells)
        {
            this._lblContent.BackColor = System.Drawing.Color.PaleTurquoise;
            this._lblMainInstruction.BackColor = System.Drawing.Color.Ivory;
            this._TableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Outset;
        }
        else
        {
            this._lblContent.BackColor = System.Drawing.SystemColors.Control;
            this._lblMainInstruction.BackColor = System.Drawing.SystemColors.Control;
            this._TableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.None;
        }
        if (updateSize)
        {
            UpdateSize(false);
        }
    }

    /// <summary> Overrides the virtual method of the base class, 
    /// which raises the <see cref="E:System.Windows.Forms.Form.Load" /> event. 
    /// Performs additional initialization for which calling int from constructor is either too early,
    /// or if we need to guarantee it is called each time the ShowDialog is called ( if reusing the instance 
    /// of Form for multiple ShowDialog calls).
    /// </summary>
    ///
    /// <param name="args"> An <see cref="EventArgs" /> that contains the event data. </param>
    protected override void OnLoad(EventArgs args)
    {
        base.OnLoad(args);

        DialogResult = DialogResult.None;
        _Result = MsgBoxResult.Cancel;

        if (_lastLocation != null)
            this.Location = _lastLocation.Value;
        UpdateCellEmphasizeProperties(false);
        UpdateSize(true);
    }

    /// <summary> Raises the <see cref="Form.Closing" /> event. </summary>
    /// <param name="args"> A <see cref="System.ComponentModel.CancelEventArgs" /> that contains the event data. </param>
    protected override void OnClosing(CancelEventArgs args)
    {
        base.OnClosing(args);
        _lastLocation = this.Location;
    }
    #endregion // Protected Interface

    #region Private Members

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
    /// Instead of usage of field initializer of backing field _Dict, the code uses the lazy initialization.
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
        foreach (var bttn in ResultToButtonsDict.Values)
        {
            bool bVisible = !string.IsNullOrEmpty(bttn.Text);
            bttn.Visible = bVisible;
        }
    }

    /// <summary>
    /// This call updates the size of the window based on certain factors, such as if an icon is present, 
    /// and the size of label.
    /// </summary>
    /// <param name="updateCols"> If true, a method <see cref="UpdateIconColumn"/> is called first. </param>
    private void UpdateSize(bool updateCols)
    {
        if (updateCols)
        {
            UpdateIconColumn();
        }

        Size pSize = this._TableLayoutPanel.Size;
        Size delta = pSize - _initPanelSize;
        if ((delta.Width != 0) || (delta.Height != 0))
        {
            this.Size = this._initSize + delta;
        }
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

    /// <summary>
    /// Shows or hides the column containing _pbxIcon control, depending whether that control
    /// has its image set to 'something' not null or not.
    /// </summary>
    private void UpdateIconColumn()
    {
        int nColumn = _TableLayoutPanel.GetColumn(_pbxIcon);
        var cs = _TableLayoutPanel.ColumnStyles[nColumn];

        if (!_prevSizeType.HasValue)
        {
            _prevSizeType = cs.SizeType;
            _prevWidth = cs.Width;
        }

        if (StandardIcon == MessageBoxIcon.None)
        {
            cs.SizeType = SizeType.Absolute;
            cs.Width = 0;
        }
        else
        {
            cs.SizeType = _prevSizeType.Value;
            cs.Width = _prevWidth;
        }
    }
    #endregion //   Private Methods

    #region Event Handlers

    /// <summary> The event handler called by _lblMainInstruction.TextChanged 
    ///           or _lblContent.TextChanged event. </summary>
    /// <param name="sender"> The sender. </param>
    /// <param name="args"> Event information. </param>
    private void AnyLabel_TextChanged(object sender, EventArgs args)
    {
        UpdateSize(false);
    }

    private void MsgBoxYesNoAllLP_DoubleClick(object sender, EventArgs args)
    {
        _EmphasizeCells = !EmphasizeCells;
        UpdateCellEmphasizeProperties(true);
    }

    /// <summary> The Event handler for any button being clicked.</summary>
    ///
    /// <param name="sender"></param>
    /// <param name="args"> </param>
    private void OnBtn_Click(object sender, EventArgs args)
    {
        Button btn = (Button)sender; // raise an exception if the sender is not a button

        Debug.Assert(btn.DialogResult == DialogResult.Yes || btn.DialogResult == DialogResult.No
          || btn.DialogResult == DialogResult.Cancel);
        this.DialogResult = btn.DialogResult;
        this._Result = ResultToButtonsDict.First(pair => pair.Value == btn).Key;
    }
    #endregion // Event Handlers

    #region Private Fields

    /// <summary> The initial size ( cached value retrieved in constructor). </summary>
    private readonly Size _initSize;

    /// <summary> The initial size of panel ( cached value retrieved in constructor). </summary>
    private readonly Size _initPanelSize;

    /// <summary> The last location of form. </summary>
    private Nullable<Point> _lastLocation;

    /// <summary> The backup field for size type of column containing the icon. </summary>
    private Nullable<SizeType> _prevSizeType;

    /// <summary> The backup field for width of column containing the icon. </summary>
    private float _prevWidth;

    /// <summary> A backing field of property <see cref="ButtonTextsSpecified"/>.</summary>
    private readonly IEnumerable<KeyValuePair<MsgBoxResult, string>> _buttonTexts;

    /// <summary> The backing field of property <see cref="Result"/>. </summary> 
    private MsgBoxResult _Result = MsgBoxResult.Cancel;

    /// <summary> The backing field for property <see cref="StandardIcon"/>. </summary>
    private MessageBoxIcon _StandardIcon = MessageBoxIcon.None;

    /// <summary> A backing field for property <see cref="ResultToButtonsDict"/>. </summary>
    private IReadOnlyDictionary<MsgBoxResult, Button> _resultToButtonsDictionary;

    /// <summary> A backing field for property <see cref="EmphasizeCells"/>. </summary>
    private bool _EmphasizeCells = true;

    #endregion // Private Fields
    #endregion // Private Members
}
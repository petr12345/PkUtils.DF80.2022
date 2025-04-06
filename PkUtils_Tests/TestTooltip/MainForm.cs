using System;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.UI.General;
using PK.PkUtils.UI.TipHandlers;

namespace PK.TestTooltip;

/// <summary>
/// Summary description for MainForm.
/// </summary>
public partial class MainForm : FormWithLayoutPersistence
{
    #region Fields
    public static readonly string[] STRINGS_A = [
        "short",
        "this is longer",
        "this is more longer",
        "this is even longer text",
        "this is quite fine and yet longer still",
        "this is believe it or not yet even longer than all those before",
        "but this string is against all possible odds the longest one in the bunch",
    ];

    public static readonly string[] STRINGS_B = [
        "0 short abcdefgh #0",
        "1 this is longer abcdefgh #1",
        "2 this is longer, too abcdefgh #2",
        "3 this is even longer text abcdefgh #3",
        "4 this is even more longer text abcdefgh #4",
        "5 this is quite fine and yet longer still abcdefgh #5",
        "6 this is believe it or not yet even longer than all those before abcdefgh #6",
        "7 this is trying to be longer too but still has space for improvement abcdefgh #7",
        "8 this is almost the longest one but is still has space for improvement, too abcdefgh #8",
        "9 but this is against all possible odds the longest one in the bunch--------really!! abcdefgh #9",
    ];

    protected ListBoxTipHandler _lbTipHandler1 = new();
    protected ListBoxTipHandler _lbTipHandler2 = new();
    protected ComboBoxTipHandler _cbTipHandlerA = new();
    protected ComboBoxTipHandler _cbTipHandlerB = new();
    protected ComboBoxTipHandler _cbTipHandlerC = new();
    protected WindowTitleTipHandler _titleTipHandler = new();
    #endregion // Fields

    #region Constructor(s)

    public MainForm()
    {
        InitializeComponent();
        PopulateControls();
    }
    #endregion // Constructor(s)

    #region Methods

    #region General initialization

    /// <summary>
    /// Fills comboboxes and listboxes, this is initialization called after InitializeComponent
    /// </summary>
    protected void PopulateControls()
    {
        // populate listboxes and comboboxes
        _listBox1.Items.AddRange(STRINGS_A);
        _listBox2.Items.AddRange(STRINGS_B);
        _comboSimple.Items.AddRange(STRINGS_B);
        _comboDropDown.Items.AddRange(STRINGS_B);
        _comboDropDownList.Items.AddRange(STRINGS_B);

        // init tip handlers
        _lbTipHandler1.Init(this._listBox1, null);
        _lbTipHandler2.Init(this._listBox2, Program.Settings.ListBoxTooltipFont);
        _cbTipHandlerA.Init(this._comboSimple, Program.Settings.ComboBoxTooltipFont);
        _cbTipHandlerB.Init(this._comboDropDown, Program.Settings.ComboBoxTooltipFont);
        _cbTipHandlerC.Init(this._comboDropDownList, Program.Settings.ComboBoxTooltipFont);
        _titleTipHandler.Init(this, Program.Settings.ComboBoxTooltipFont);
        // init the rest
        /* _labelLbx2nd.Font = Program.Settings.ListBoxTooltipFont; */
    }

    private void SetListBxTooltipFont(Font font)
    {
        _lbTipHandler2.SetTipWindowFont(font);
        /* _labelLbx2nd.Font = font; */
        Program.Settings.ListBoxTooltipFont = font;
    }

    private void SetComboBxTooltipFont(Font font)
    {
        _cbTipHandlerA.SetTipWindowFont(font);
        _cbTipHandlerB.SetTipWindowFont(font);
        _cbTipHandlerC.SetTipWindowFont(font);
        Program.Settings.ComboBoxTooltipFont = font;
    }

    private void SetTitleHandlerTooltipFont(Font font)
    {
        _titleTipHandler.SetTipWindowFont(font);
    }
    #endregion // General initialization

    #region Overrides

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
    #endregion // Overrides
    #endregion // Methods

    #region Event_handlers

    private void MainFor_Load(object sender, EventArgs e)
    {
        this._tabControl.SelectedIndex = Program.Settings.TabPageIndex;
        this._checkBxUseVisualStyles.Checked = Program.Settings.UseVisualStyles;
        LoadLocation();
    }

    private void MainFor_Closed(object sender, System.EventArgs e)
    {
        _lbTipHandler1.Done();
        _lbTipHandler2.Done();
        _cbTipHandlerA.Done();
        _cbTipHandlerB.Done();
        _cbTipHandlerC.Done();
        _titleTipHandler.Done();
    }

    private void MainFor_FormClosing(object sender, FormClosingEventArgs e)
    {
        Program.Settings.TabPageIndex = this._tabControl.SelectedIndex;
        Program.Settings.UseVisualStyles = this._checkBxUseVisualStyles.Checked;
    }

    private void ButtonFont_Click(object sender, System.EventArgs e)
    {
        Font oldFont = _lbTipHandler2.GetTipWindowFont();
        FontDialog dlg = new()
        {
            Font = oldFont
        };
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            SetListBxTooltipFont(dlg.Font);
        }
    }

    private void BtnResetListBxTooltipFont_Click(object sender, EventArgs e)
    {
        SetListBxTooltipFont(null);
    }

    private void BtnCbTooltipFontA_Click(object sender, System.EventArgs e)
    {
        Font oldFont = _cbTipHandlerA.GetTipWindowFont();
        System.Windows.Forms.FontDialog dlg = new()
        {
            Font = oldFont
        };
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            SetComboBxTooltipFont(dlg.Font);
            SetTitleHandlerTooltipFont(dlg.Font);
        }
    }

    private void BtnResetComboBxTooltipFont_Click(object sender, EventArgs e)
    {
        SetComboBxTooltipFont(null);
        SetTitleHandlerTooltipFont(null);
    }

    private void BnRestart_Click(object sender, EventArgs e)
    {
        Application.Restart();
    }

    private void BtnClose_Click(object sender, EventArgs e)
    {
        this.Close();
    }
    #endregion // Event_handlers
}

using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using PK.PkUtils.Interfaces;
using PK.PkUtils.MessageHooking;
using PK.PkUtils.Undo;
using PK.SubstEditLib.Subst;
using PK.TestTgSchema.Properties;
using PK.TestTgSchema.TextBoxCtrls;
using PK.TestTgSchema.UserCtrls;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable CA1859    // Change type of variable ...

namespace PK.TestTgSchema;

public partial class MainForm : Form
{
    #region Typedefs
    public delegate void KeyboardEventHandler(object sender, KeyEventArgs e);
    #endregion // Typedefs

    #region Fields
    protected SubstEditTextBoxCtrl<FieldTypeId> _oldFocObj;
    protected SubstEditTextBoxCtrl<EFieldLineType> _oldFocEn;
    protected SaveFileDialog _saveDialogComponents;
    protected OpenFileDialog _openDialogComponents;
    protected SaveFileDialog _saveDialogLines;
    protected OpenFileDialog _openDialogLines;
    protected ClipMonitorHook _clipMonitor;

    // Argument for file save. May be changed for testing purpose
    /*
    private readonly Encoding _fileSaveEncoding = Encoding.UTF7;
    private readonly Encoding _fileSaveEncoding = Encoding.Unicode;
    */
    private readonly Encoding _fileSaveEncoding = Encoding.UTF8;

    private const string TabIndexFileName = "last_active_tab.index";
    // open file dialog filters
    private const string _strOpenFileFilterComponents = "XML files|*.xml|Components Schema Binary files (*.csb)|*.csb|Plain text (*.txt)|*.txt";
    private const string _strOpenFileFilterLines = "XML files|*.xml|Lines Schema Binary files (*.lsb)|*.lsb|Plain text (*.txt)|*.txt";
    #endregion // Fields

    #region Constructor(s)
    public MainForm()
    {
        InitializeComponent();
        this.Icon = Resources.C_sharp;

        this._tgSchemaComponentslUserCtrl.SetSubstitutionMap(TaggingFieldTables._myCompFieldsDescr);
        this._tgSchemaComponentslUserCtrl.SchemaTextBxCtrl.LostFocus += new EventHandler(OnSchemaTextBxComponentsCtrl_LostFocus);
        this._tgSchemaComponentslUserCtrl.SchemaTextBxCtrl.TextChanged += new EventHandler(OnSchemaTextBxCtrl_TextChanged);
        this._tgSchemaComponentslUserCtrl.SchemaTextBxCtrl.EventSubstEditContentsChanged += new ModifiedEventHandler(OnSchemaTextBxCtrl_SubstContentsChanged);
        this._tgSchemaComponentslUserCtrl.SchemaTextBxCtrl.EventSelChanged += new EventHandler<SelChangedEventArgs>(OnSelChanged);

        this._tgSchemaLinesUserCtrl.SetSubstitutionMap(TaggingFieldTables._myLinesFieldsDescr);
        this._tgSchemaLinesUserCtrl.SchemaTextBxCtrl.LostFocus += new EventHandler(OnSchemaTextBxLinesCtrl_LostFocus);

        this.Activated += new EventHandler(MainFor_Activated);

        _clipMonitor = new ClipMonitorHook(this);
        _clipMonitor.EventClipboardChanged += new EventHandler(OnEventClipboardChanged);

        PopulateInsertLineContextMenu();
        UpdateToolstripButtons();
    }
    #endregion // Constructor(s)

    #region Properties

    protected OpenFileDialog OpenDialogComponents
    {
        get
        {
            if (null == _openDialogComponents)
            {
                _openDialogComponents = new OpenFileDialog
                {
                    Filter = _strOpenFileFilterComponents,
                    CheckFileExists = true
                };
            }
            return _openDialogComponents;
        }
    }

    protected SaveFileDialog SaveDialogComponents
    {
        get
        {
            if (null == _saveDialogComponents)
            {
                _saveDialogComponents = new SaveFileDialog
                {
                    Filter = _strOpenFileFilterComponents
                };
            }
            return _saveDialogComponents;
        }
    }

    protected OpenFileDialog OpenDialogLines
    {
        get
        {
            if (null == _openDialogLines)
            {
                _openDialogLines = new OpenFileDialog
                {
                    Filter = _strOpenFileFilterLines,
                    CheckFileExists = true
                };
            }
            return _openDialogLines;
        }
    }

    protected SaveFileDialog SaveDialogLines
    {
        get
        {
            if (null == _saveDialogLines)
            {
                _saveDialogLines = new SaveFileDialog
                {
                    Filter = _strOpenFileFilterLines
                };
            }
            return _saveDialogLines;
        }
    }
    #endregion // Properties

    #region Methods
    #region Protected methods

    protected TaggingSchemaGeneralClassBasedUserCtrl ActiveClassBasedUseCtrl()
    {
        TaggingSchemaGeneralClassBasedUserCtrl result = null;

        if (_tabControl.SelectedTab == this._tabPage1)
        {
            result = _tgSchemaComponentslUserCtrl;
        }
        return result;
    }

    protected TaggingSchemaGeneralEnumBaseUserCtrl ActiveEnumBasedUseCtrl()
    {
        TaggingSchemaGeneralEnumBaseUserCtrl result = null;

        if (_tabControl.SelectedTab == this._tabPage2)
        {
            result = _tgSchemaLinesUserCtrl;
        }
        return result;
    }

    protected void InsertFieldInActiveBx(FieldTypeId id)
    {
        this.ActiveClassBasedUseCtrl().SchemaTextBxCtrl.InsertNewInfo(id);
        RestoreFocus();
    }

    protected void InsertFieldInActiveBx(EFieldLineType id)
    {
        this.ActiveEnumBasedUseCtrl().SchemaTextBxCtrl.InsertNewInfo(id);
        RestoreFocus();
    }

    protected void InitializeFocus()
    {
        switch (this._tabControl.SelectedIndex)
        {
            case 0:
                this._tgSchemaComponentslUserCtrl.FocusEditCtrl();
                break;
            case 1:
                this._tgSchemaLinesUserCtrl.FocusEditCtrl();
                break;
        }
    }

    protected void RestoreFocus()
    {
        if (null != _oldFocObj)
        {
            _oldFocObj.RestoreFocus();
        }
        else
        {
            _oldFocEn?.RestoreFocus();
        }
        _oldFocObj = null;
        _oldFocEn = null;
    }

    protected void UpdateToolstripButtons()
    {
        TaggingSchemaGeneralClassBasedUserCtrl useCtrl;
        TaggingSchemaTextBoxClassBasedCtrl tbxCtrl;
        bool bCut = false;
        bool bCopy = false;
        bool bPaste = false;
        bool bUndo = false;
        bool bRedo = false;

        if ((null != (useCtrl = ActiveClassBasedUseCtrl())) &&
            (null != (tbxCtrl = useCtrl.SchemaTextBxCtrl)))
        {
            IClipboardable iClip = tbxCtrl;
            IUndoable iUnd = tbxCtrl.TheHook;

            bCut = iClip.CanCut;
            bCopy = iClip.CanCopy;
            bPaste = iClip.CanPaste;
            bUndo = iUnd.CanUndo;
            bRedo = iUnd.CanRedo;
        }

        this._btnCut.Enabled = bCut;
        this._btnCopy.Enabled = bCopy;
        this._btnPaste.Enabled = bPaste;
        this._btnUndo.Enabled = bUndo;
        this._btnRedo.Enabled = bRedo;
    }

    protected void DoOpen<FieldTypeId>(
        string strOpenFile,
        FileFormatType fmt,
        TaggingSchemaTextBoxGenericCtrl<FieldTypeId> textBx)
    {
        if (textBx.DoOpen(strOpenFile, fmt))
        {
            textBx.SetModified(false);
            UpdatePreview();
        }
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (_clipMonitor != null))
        {
            _clipMonitor.Dispose();
            _clipMonitor = null;
        }
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }
    #endregion // Protected methods

    #region Private methods

    private static string FieldPreviewValFn(ISubstitutionDescriptor<FieldTypeId> lpDesc)
    {
        DateTime now = DateTime.Now;
        Type tField = lpDesc.FieldId;
        string strNewPart = lpDesc.DisplayText;

        if (tField == typeof(Field_PRJ_Title))
        {
            strNewPart = "Library of Congress";
        }
        else if (tField == typeof(Field_DOC_Author))
        {
            strNewPart = System.Environment.UserName;
        }
        else if (tField == typeof(Field_DOC_Title))
        {
            strNewPart = "West_Wall";
        }
        else if (tField == typeof(Field_DOC_Copyright))
        {
            strNewPart = "© " + now.Year.ToString(CultureInfo.CurrentUICulture);
        }
        else if (tField == typeof(Field_Year))
        {
            strNewPart = now.Year.ToString(CultureInfo.CurrentUICulture);
        }
        else if (tField == typeof(Field_Month))
        {
            strNewPart = now.ToString("MMMM", CultureInfo.CurrentUICulture);
        }
        else if (tField == typeof(Field_DayOfWeek))
        {
            strNewPart = now.ToString("dddd", CultureInfo.CurrentUICulture);
        }
        else if (tField == typeof(Field_Dog))
        {
            strNewPart = "The Hound of the Baskervilles";
        }
        return strNewPart;
    }

    private static string GetPreviewText(SubstLogData<FieldTypeId> logData)
    {
        int nDex;
        string strTmp = SubstLogData<FieldTypeId>.LogStrToPhysStr(logData, FieldPreviewValFn);

        if (0 < (nDex = strTmp.IndexOf('\n')))
        {
            strTmp = strTmp[..nDex];
        }
        if (0 < (nDex = strTmp.IndexOf('\r')))
        {
            strTmp = strTmp[..nDex];
        }
        return strTmp;
    }

    private string GetPreviewText()
    {
        return GetPreviewText(this._tgSchemaComponentslUserCtrl.SchemaTextBxCtrl.LogData);
    }

    private void UpdatePreview()
    {
        this._textBxPreview.Text = GetPreviewText();
    }

    private void PopulateInsertLineContextMenu()
    {
        ToolStripItemCollection items = this._ctxMenuInsertLine.Items;
        items.Clear();

        ToolStripMenuItem itemProjName = new("Project Name");
        itemProjName.Click += new EventHandler(OnBtnProjName_Click);
        items.Add(itemProjName);

        ToolStripMenuItem itemResistance = new("Resistance");
        itemResistance.Click += new EventHandler(OnBtnResistance_Click);
        items.Add(itemResistance);

        ToolStripMenuItem itemDiameter = new("Diameter");
        itemDiameter.Click += new EventHandler(OnBtnDiameter_Click);
        items.Add(itemDiameter);
    }
    #endregion // Private methods

    #region [0] Tab Restore

    /// <summary>
    /// Returns the full path to the file used to persist the last active tab index.
    /// The path is built from the per-user application data folder and a fixed file name.
    /// </summary>
    /// <returns>The full file path where the last active tab index is stored.</returns>
    protected static string TabIndexFilePath()
    {
        // Application.UserAppDataPath returns a per-user, per-application path.
        string dir = Application.UserAppDataPath;
        return Path.Combine(dir, TabIndexFileName);
    }

    /// <summary>
    /// Called when the form is loaded. Restores the last active tab index after base load logic.
    /// </summary>
    /// <param name="e">Event arguments for the load event.</param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RestoreLastActiveTab();
    }

    /// <summary>
    /// Attempts to read the persisted last active tab index from disk and select that tab.
    /// Any errors are ignored to avoid preventing the form from loading.
    /// </summary>
    protected void RestoreLastActiveTab()
    {
        try
        {
            string path = TabIndexFilePath();
            if (File.Exists(path))
            {
                var text = File.ReadAllText(path).Trim();
                if (int.TryParse(text, out var idx))
                {
                    if (_tabControl != null && idx >= 0 && idx < _tabControl.TabCount)
                    {
                        _tabControl.SelectedIndex = idx;
                    }
                }
            }
        }
        catch
        {
            // Ignore errors - do not prevent the form from loading.
        }
    }

    /// <summary>
    /// Persists the currently selected tab index to disk. IO errors are ignored.
    /// The method ensures the target directory exists before writing the file.
    /// </summary>
    protected void SaveLastActiveTab()
    {
        try
        {
            string path = TabIndexFilePath();
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (_tabControl != null)
            {
                File.WriteAllText(path, _tabControl.SelectedIndex.ToString());
            }
        }
        catch
        {
            // Ignore IO errors on exit.
        }
    }
    #endregion [0] Tab Restore
    #endregion // Methods

    #region Event handlers

    #region Focus-related functionality

    /// <summary>
    /// The handler for proper initialization of focus.
    /// There seems no better way to do this... ;-(
    /// see for instance http://www.telerik.com/community/forums/aspnet/tabstrip/set-focus-to-textbox-after-tab-clickec.aspx
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainFor_Activated(object sender, EventArgs e)
    {
        InitializeFocus();
    }

    private void OnTabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
        InitializeFocus();
    }

    private void OnSchemaTextBxComponentsCtrl_LostFocus(object sender, EventArgs e)
    {   // save od focus
        Control ctrlOldFoc = sender as Control;
        // assigns SubstEditTextBoxCtrl or null if it was something else than SubstEditTextBoxCtrl
        _oldFocObj = ctrlOldFoc as SubstEditTextBoxCtrl<FieldTypeId>;
        _oldFocEn = null;
    }

    private void OnSchemaTextBxLinesCtrl_LostFocus(object sender, EventArgs e)
    {   // save od focus
        Control ctrlOldFoc = sender as Control;
        // assigns SubstEditTextBoxCtrl or null if it was something else than SubstEditTextBoxCtrl
        _oldFocObj = null;
        _oldFocEn = ctrlOldFoc as SubstEditTextBoxCtrl<EFieldLineType>;
    }
    #endregion // Focus-related functionality

    #region Preview-related

    private void OnSchemaTextBxCtrl_SubstContentsChanged(IModified sender, EventArgs e)
    {
        if (sender.IsModified)
        {
            UpdatePreview();
        }
        this.UpdateToolstripButtons();
    }

    private void OnSchemaTextBxCtrl_TextChanged(object sender, EventArgs e)
    {
        this.UpdateToolstripButtons();
    }
    #endregion // Preview-related

    #region Insertions for components

    private void OnBtnProjTitle_Click(object sender, EventArgs e)
    {
        InsertFieldInActiveBx(new FieldTypeId(typeof(Field_PRJ_Title)));
    }

    private void OnBtnDocAuthor_Click(object sender, EventArgs e)
    {
        InsertFieldInActiveBx(new FieldTypeId(typeof(Field_DOC_Author)));
    }

    private void OnBtnDocTitle_Click(object sender, EventArgs e)
    {
        InsertFieldInActiveBx(new FieldTypeId(typeof(Field_DOC_Title)));
    }

    private void OnBtnDocCopyright_Click(object sender, EventArgs e)
    {
        InsertFieldInActiveBx(new FieldTypeId(typeof(Field_DOC_Copyright)));
    }

    private void OnBtnYear_Click(object sender, EventArgs e)
    {
        InsertFieldInActiveBx(new FieldTypeId(typeof(Field_Year)));
    }

    private void OnBtnMonth_Click(object sender, EventArgs e)
    {
        InsertFieldInActiveBx(new FieldTypeId(typeof(Field_Month)));
    }

    private void OnBtnDayOfWeek_Click(object sender, EventArgs e)
    {
        InsertFieldInActiveBx(new FieldTypeId(typeof(Field_DayOfWeek)));
    }

    private void OnBtnDog_Click(object sender, EventArgs e)
    {
        InsertFieldInActiveBx(new FieldTypeId(typeof(Field_Dog)));
    }
    #endregion // Insertions for components

    #region Insertions for lines

    private void OnBtnInsertLine_Click(object sender, EventArgs e)
    {
        Point pt = new(0, _btnInsertLine.Height);
        this._ctxMenuInsertLine.Show(_btnInsertLine, pt);
    }

    private void OnBtnProjName_Click(object sender, EventArgs e)
    {
        InsertFieldInActiveBx(EFieldLineType.IdField_ProjName);
    }
    private void OnBtnResistance_Click(object sender, EventArgs e)
    {
        InsertFieldInActiveBx(EFieldLineType.IdField_Resistance);
    }

    private void OnBtnDiameter_Click(object sender, EventArgs e)
    {
        InsertFieldInActiveBx(EFieldLineType.IdField_Diameter);
    }
    #endregion // Insertions for lines

    #region Open & Save

    private void OnBtnNew_Click(object sender, EventArgs e)
    {
        TaggingSchemaTextBoxClassBasedCtrl ctrl = ActiveClassBasedUseCtrl().SchemaTextBxCtrl;

        if (ctrl.SaveModified(SaveDialogComponents, _fileSaveEncoding))
        {
            ctrl.TheHook.DeleteContents();
            ctrl.TheHook.SetModified(false);
        }
    }

    private void OnBtnOpen_Click(object sender, EventArgs e)
    {
        string strOpenFile = string.Empty;
        int nFilterIndex = -1;
        OpenFileDialog od = OpenDialogComponents;
        TaggingSchemaTextBoxClassBasedCtrl textBx = ActiveClassBasedUseCtrl().SchemaTextBxCtrl;

        if (!textBx.SaveModified(SaveDialogComponents, _fileSaveEncoding))
        {
            return;
        }

        if (od.ShowDialog() == DialogResult.OK)
        {
            strOpenFile = od.FileName;
            nFilterIndex = od.FilterIndex;
        }

        if (!string.IsNullOrEmpty(strOpenFile))
        {
            FileFormatType fmt = (FileFormatType)nFilterIndex;
            DoOpen<FieldTypeId>(strOpenFile, fmt, textBx);
        }
    }

    private void OnBtnSaveAs_Click(object sender, EventArgs e)
    {
        string strSaveFile = string.Empty;
        int nFilterIndex = -1;
        SaveFileDialog sd = SaveDialogComponents;

        if (sd.ShowDialog() == DialogResult.OK)
        {
            strSaveFile = sd.FileName;
            nFilterIndex = sd.FilterIndex;
        }

        if (!string.IsNullOrEmpty(strSaveFile))
        {
            FileFormatType fmt = (FileFormatType)nFilterIndex;
            TaggingSchemaTextBoxClassBasedCtrl textBx = ActiveClassBasedUseCtrl().SchemaTextBxCtrl;
            if (textBx.DoSaveAs(strSaveFile, fmt, _fileSaveEncoding))
            {
                textBx.SetModified(false);
            }
        }
    }

    private void OnBtnOpenLines_Click(object sender, EventArgs e)
    {
        string strOpenFile = string.Empty;
        int nFilterIndex = -1;
        OpenFileDialog od = OpenDialogLines;
        TaggingSchemaTextBoxEnumBasedCtrl textBx = ActiveEnumBasedUseCtrl().SchemaTextBxCtrl;

        if (!textBx.SaveModified(SaveDialogLines, _fileSaveEncoding))
        {
            return;
        }

        if (od.ShowDialog() == DialogResult.OK)
        {
            strOpenFile = od.FileName;
            nFilterIndex = od.FilterIndex;
        }

        if (!string.IsNullOrEmpty(strOpenFile))
        {
            FileFormatType fmt = (FileFormatType)nFilterIndex;
            DoOpen<EFieldLineType>(strOpenFile, fmt, textBx);
        }
    }

    private void OnBtnSaveLines_Click(object sender, EventArgs e)
    {
        string strSaveFile = string.Empty;
        int nFilterIndex = -1;
        SaveFileDialog sd = SaveDialogLines;

        if (sd.ShowDialog() == DialogResult.OK)
        {
            strSaveFile = sd.FileName;
            nFilterIndex = sd.FilterIndex;
        }

        if (!string.IsNullOrEmpty(strSaveFile))
        {
            FileFormatType fmt = (FileFormatType)nFilterIndex;
            TaggingSchemaTextBoxEnumBasedCtrl textBx = ActiveEnumBasedUseCtrl().SchemaTextBxCtrl;
            if (textBx.DoSaveAs(strSaveFile, fmt, _fileSaveEncoding))
            {
                textBx.SetModified(false);
            }
        }
    }

    private void MainFor_FormClosing(object sender, FormClosingEventArgs e)
    {
        if ((null != ActiveClassBasedUseCtrl()) &&
            !ActiveClassBasedUseCtrl().SchemaTextBxCtrl.SaveModified(SaveDialogComponents, _fileSaveEncoding))
        {
            e.Cancel = true;
        }
        if ((null != ActiveEnumBasedUseCtrl()) &&
            !ActiveEnumBasedUseCtrl().SchemaTextBxCtrl.SaveModified(SaveDialogLines, _fileSaveEncoding))
        {
            e.Cancel = true;
        }

        if (!e.Cancel)
        {
            // Persist last active tab index (errors ignored inside SaveLastActiveTab)
            SaveLastActiveTab();
        }
    }
    #endregion // Open & Save

    #region Clipboard

    private void OnBtnCut_Click(object sender, EventArgs e)
    {
        ActiveClassBasedUseCtrl().SchemaTextBxCtrl.Cut();
        this.UpdateToolstripButtons();
    }
    private void OnBtnCopy_Click(object sender, EventArgs e)
    {
        ActiveClassBasedUseCtrl().SchemaTextBxCtrl.Copy();
        this.UpdateToolstripButtons();
    }

    private void OnBtnPaste_Click(object sender, EventArgs e)
    {
        ActiveClassBasedUseCtrl().SchemaTextBxCtrl.Paste();
        this.UpdateToolstripButtons();
    }
    #endregion // Clipboard

    #region Undo_redo

    private void OnBtnUndo_Click(object sender, EventArgs e)
    {
        if (this._tgSchemaComponentslUserCtrl._SchemaTextBxCtrl.TheHook.CanUndo)
        {
            this._tgSchemaComponentslUserCtrl._SchemaTextBxCtrl.TheHook.Undo();
            this.UpdateToolstripButtons();
        }
        else
        {
            MessageBox.Show("Nothing to undo");
        }
    }

    private void OnBtnRedo_Click(object sender, EventArgs e)
    {
        if (this._tgSchemaComponentslUserCtrl._SchemaTextBxCtrl.TheHook.CanRedo)
        {
            this._tgSchemaComponentslUserCtrl._SchemaTextBxCtrl.TheHook.Redo();
            this.UpdateToolstripButtons();

        }
        else
        {
            MessageBox.Show("Nothing to redo");
        }
    }
    #endregion // Undo_redo

    #region Various_other_handlers

    private void OnEventClipboardChanged(object sender, EventArgs e)
    {
        this.UpdateToolstripButtons();
    }

    private void OnSelChanged(object sender, SelChangedEventArgs e)
    {
        this.UpdateToolstripButtons();
    }
    #endregion // Various_other_handlers
    #endregion // Event handlers
}

#pragma warning restore CA1859
#pragma warning restore IDE0079
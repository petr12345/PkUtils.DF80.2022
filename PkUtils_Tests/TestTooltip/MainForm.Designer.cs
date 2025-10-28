namespace PK.TestTooltip;

public partial class MainForm
{
    private System.Windows.Forms.ListBox _listBox1;
    private System.Windows.Forms.ListBox _listBox2;
    private System.Windows.Forms.Button _buttonFont;
    private System.Windows.Forms.TabControl _tabControl;
    private System.Windows.Forms.TabPage _tabListBox;
    private System.Windows.Forms.ComboBox _comboSimple;
    private System.Windows.Forms.Label _label_CBS_SIMPLE;
    private System.Windows.Forms.ComboBox _comboDropDown;
    private System.Windows.Forms.Label _label_CBS_DROPDOWN;
    private System.Windows.Forms.Label _label_CBS_DROPDOWNLIST;
    private System.Windows.Forms.ComboBox _comboDropDownList;
    private System.Windows.Forms.PictureBox _pictureApe;
    private System.Windows.Forms.Button _buttonCbTooltipFontA;
    private System.Windows.Forms.TabPage _tabComboBox;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        _listBox1 = new System.Windows.Forms.ListBox();
        _listBox2 = new System.Windows.Forms.ListBox();
        _buttonFont = new System.Windows.Forms.Button();
        _tabControl = new System.Windows.Forms.TabControl();
        _tabListBox = new System.Windows.Forms.TabPage();
        _labelLbx2nd = new System.Windows.Forms.Label();
        _btnResetListBxTooltipFont = new System.Windows.Forms.Button();
        _pictureApe = new System.Windows.Forms.PictureBox();
        _tabComboBox = new System.Windows.Forms.TabPage();
        _btnResetComboBxTooltipFont = new System.Windows.Forms.Button();
        _btnClose = new System.Windows.Forms.Button();
        _btnRestart = new System.Windows.Forms.Button();
        _checkBxUseVisualStyles = new System.Windows.Forms.CheckBox();
        _buttonCbTooltipFontA = new System.Windows.Forms.Button();
        _label_CBS_DROPDOWNLIST = new System.Windows.Forms.Label();
        _comboDropDownList = new System.Windows.Forms.ComboBox();
        _label_CBS_DROPDOWN = new System.Windows.Forms.Label();
        _comboDropDown = new System.Windows.Forms.ComboBox();
        _label_CBS_SIMPLE = new System.Windows.Forms.Label();
        _comboSimple = new System.Windows.Forms.ComboBox();
        _tabControl.SuspendLayout();
        _tabListBox.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_pictureApe).BeginInit();
        _tabComboBox.SuspendLayout();
        SuspendLayout();
        // 
        // _listBox1
        // 
        _listBox1.ItemHeight = 15;
        _listBox1.Location = new System.Drawing.Point(10, 12);
        _listBox1.Name = "_listBox1";
        _listBox1.Size = new System.Drawing.Size(199, 109);
        _listBox1.TabIndex = 0;
        // 
        // _listBox2
        // 
        _listBox2.ItemHeight = 15;
        _listBox2.Location = new System.Drawing.Point(10, 164);
        _listBox2.Name = "_listBox2";
        _listBox2.Size = new System.Drawing.Size(199, 109);
        _listBox2.TabIndex = 2;
        // 
        // _buttonFont
        // 
        _buttonFont.Location = new System.Drawing.Point(227, 268);
        _buttonFont.Name = "_buttonFont";
        _buttonFont.Size = new System.Drawing.Size(96, 29);
        _buttonFont.TabIndex = 3;
        _buttonFont.Text = "Tooltip font...";
        _buttonFont.Click += ButtonFont_Click;
        // 
        // _tabControl
        // 
        _tabControl.Controls.Add(_tabListBox);
        _tabControl.Controls.Add(_tabComboBox);
        _tabControl.Location = new System.Drawing.Point(0, 0);
        _tabControl.Name = "_tabControl";
        _tabControl.SelectedIndex = 0;
        _tabControl.Size = new System.Drawing.Size(600, 348);
        _tabControl.TabIndex = 3;
        // 
        // _tabListBox
        // 
        _tabListBox.Controls.Add(_labelLbx2nd);
        _tabListBox.Controls.Add(_btnResetListBxTooltipFont);
        _tabListBox.Controls.Add(_pictureApe);
        _tabListBox.Controls.Add(_listBox1);
        _tabListBox.Controls.Add(_listBox2);
        _tabListBox.Controls.Add(_buttonFont);
        _tabListBox.Location = new System.Drawing.Point(4, 24);
        _tabListBox.Name = "_tabListBox";
        _tabListBox.Size = new System.Drawing.Size(592, 320);
        _tabListBox.TabIndex = 0;
        _tabListBox.Text = "ListBox";
        // 
        // _labelLbx2nd
        // 
        _labelLbx2nd.Location = new System.Drawing.Point(11, 133);
        _labelLbx2nd.Name = "_labelLbx2nd";
        _labelLbx2nd.Size = new System.Drawing.Size(198, 25);
        _labelLbx2nd.TabIndex = 1;
        _labelLbx2nd.Text = "Tooltip with variable font:";
        _labelLbx2nd.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
        // 
        // _btnResetListBxTooltipFont
        // 
        _btnResetListBxTooltipFont.Location = new System.Drawing.Point(341, 268);
        _btnResetListBxTooltipFont.Name = "_btnResetListBxTooltipFont";
        _btnResetListBxTooltipFont.Size = new System.Drawing.Size(96, 29);
        _btnResetListBxTooltipFont.TabIndex = 4;
        _btnResetListBxTooltipFont.Text = "Reset font";
        _btnResetListBxTooltipFont.Click += BtnResetListBxTooltipFont_Click;
        // 
        // _pictureApe
        // 
        _pictureApe.Image = Properties.Resources.MyAppe;
        _pictureApe.Location = new System.Drawing.Point(224, 12);
        _pictureApe.Name = "_pictureApe";
        _pictureApe.Size = new System.Drawing.Size(371, 244);
        _pictureApe.TabIndex = 3;
        _pictureApe.TabStop = false;
        // 
        // _tabComboBox
        // 
        _tabComboBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        _tabComboBox.Controls.Add(_btnResetComboBxTooltipFont);
        _tabComboBox.Controls.Add(_btnClose);
        _tabComboBox.Controls.Add(_btnRestart);
        _tabComboBox.Controls.Add(_checkBxUseVisualStyles);
        _tabComboBox.Controls.Add(_buttonCbTooltipFontA);
        _tabComboBox.Controls.Add(_label_CBS_DROPDOWNLIST);
        _tabComboBox.Controls.Add(_comboDropDownList);
        _tabComboBox.Controls.Add(_label_CBS_DROPDOWN);
        _tabComboBox.Controls.Add(_comboDropDown);
        _tabComboBox.Controls.Add(_label_CBS_SIMPLE);
        _tabComboBox.Controls.Add(_comboSimple);
        _tabComboBox.Location = new System.Drawing.Point(4, 24);
        _tabComboBox.Name = "_tabComboBox";
        _tabComboBox.Size = new System.Drawing.Size(592, 320);
        _tabComboBox.TabIndex = 1;
        _tabComboBox.Text = "ComboBox";
        // 
        // _btnResetComboBxTooltipFont
        // 
        _btnResetComboBxTooltipFont.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        _btnResetComboBxTooltipFont.Location = new System.Drawing.Point(118, 268);
        _btnResetComboBxTooltipFont.Name = "_btnResetComboBxTooltipFont";
        _btnResetComboBxTooltipFont.Size = new System.Drawing.Size(96, 29);
        _btnResetComboBxTooltipFont.TabIndex = 8;
        _btnResetComboBxTooltipFont.Text = "Reset font";
        _btnResetComboBxTooltipFont.Click += BtnResetComboBxTooltipFont_Click;
        // 
        // _btnClose
        // 
        _btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        _btnClose.Location = new System.Drawing.Point(462, 268);
        _btnClose.Name = "_btnClose";
        _btnClose.Size = new System.Drawing.Size(96, 29);
        _btnClose.TabIndex = 10;
        _btnClose.Text = "Close";
        _btnClose.UseVisualStyleBackColor = true;
        _btnClose.Click += BtnClose_Click;
        // 
        // _btnRestart
        // 
        _btnRestart.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        _btnRestart.Location = new System.Drawing.Point(462, 234);
        _btnRestart.Name = "_btnRestart";
        _btnRestart.Size = new System.Drawing.Size(96, 28);
        _btnRestart.TabIndex = 9;
        _btnRestart.Text = "Restart";
        _btnRestart.UseVisualStyleBackColor = true;
        _btnRestart.Click += BnRestart_Click;
        // 
        // _checkBxUseVisualStyles
        // 
        _checkBxUseVisualStyles.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        _checkBxUseVisualStyles.AutoSize = true;
        _checkBxUseVisualStyles.Location = new System.Drawing.Point(14, 237);
        _checkBxUseVisualStyles.Name = "_checkBxUseVisualStyles";
        _checkBxUseVisualStyles.Size = new System.Drawing.Size(332, 19);
        _checkBxUseVisualStyles.TabIndex = 6;
        _checkBxUseVisualStyles.Text = "Use visual styles ( must restart the application for change )";
        _checkBxUseVisualStyles.UseVisualStyleBackColor = true;
        // 
        // _buttonCbTooltipFontA
        // 
        _buttonCbTooltipFontA.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        _buttonCbTooltipFontA.Location = new System.Drawing.Point(10, 268);
        _buttonCbTooltipFontA.Name = "_buttonCbTooltipFontA";
        _buttonCbTooltipFontA.Size = new System.Drawing.Size(96, 29);
        _buttonCbTooltipFontA.TabIndex = 7;
        _buttonCbTooltipFontA.Text = "Tooltip font...";
        _buttonCbTooltipFontA.Click += BtnCbTooltipFontA_Click;
        // 
        // _label_CBS_DROPDOWNLIST
        // 
        _label_CBS_DROPDOWNLIST.Location = new System.Drawing.Point(404, 15);
        _label_CBS_DROPDOWNLIST.Name = "_label_CBS_DROPDOWNLIST";
        _label_CBS_DROPDOWNLIST.Size = new System.Drawing.Size(154, 15);
        _label_CBS_DROPDOWNLIST.TabIndex = 4;
        _label_CBS_DROPDOWNLIST.Text = "CBS_DROPDOWNLIST";
        // 
        // _comboDropDownList
        // 
        _comboDropDownList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        _comboDropDownList.Location = new System.Drawing.Point(403, 37);
        _comboDropDownList.Name = "_comboDropDownList";
        _comboDropDownList.Size = new System.Drawing.Size(154, 23);
        _comboDropDownList.TabIndex = 5;
        // 
        // _label_CBS_DROPDOWN
        // 
        _label_CBS_DROPDOWN.Location = new System.Drawing.Point(206, 15);
        _label_CBS_DROPDOWN.Name = "_label_CBS_DROPDOWN";
        _label_CBS_DROPDOWN.Size = new System.Drawing.Size(154, 15);
        _label_CBS_DROPDOWN.TabIndex = 2;
        _label_CBS_DROPDOWN.Text = "CBS_DROPDOWN";
        // 
        // _comboDropDown
        // 
        _comboDropDown.Location = new System.Drawing.Point(206, 37);
        _comboDropDown.Name = "_comboDropDown";
        _comboDropDown.Size = new System.Drawing.Size(154, 23);
        _comboDropDown.TabIndex = 3;
        // 
        // _label_CBS_SIMPLE
        // 
        _label_CBS_SIMPLE.Location = new System.Drawing.Point(10, 15);
        _label_CBS_SIMPLE.Name = "_label_CBS_SIMPLE";
        _label_CBS_SIMPLE.Size = new System.Drawing.Size(153, 15);
        _label_CBS_SIMPLE.TabIndex = 0;
        _label_CBS_SIMPLE.Text = "CBS_SIMPLE";
        // 
        // _comboSimple
        // 
        _comboSimple.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
        _comboSimple.Location = new System.Drawing.Point(10, 37);
        _comboSimple.Name = "_comboSimple";
        _comboSimple.Size = new System.Drawing.Size(153, 148);
        _comboSimple.TabIndex = 1;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        ClientSize = new System.Drawing.Size(570, 345);
        Controls.Add(_tabControl);
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MaximumSize = new System.Drawing.Size(586, 384);
        MinimizeBox = false;
        Name = "MainForm";
        Text = "TestTooltip";
        FormClosed += MainForm_Closed;
        FormClosing += MainFor_FormClosing;
        Load += MainFor_Load;
        _tabControl.ResumeLayout(false);
        _tabListBox.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_pictureApe).EndInit();
        _tabComboBox.ResumeLayout(false);
        _tabComboBox.PerformLayout();
        ResumeLayout(false);

    }
    #endregion

    private System.Windows.Forms.CheckBox _checkBxUseVisualStyles;
    private System.Windows.Forms.Button _btnRestart;
    private System.Windows.Forms.Button _btnClose;
    private System.Windows.Forms.Button _btnResetListBxTooltipFont;
    private System.Windows.Forms.Button _btnResetComboBxTooltipFont;
    private System.Windows.Forms.Label _labelLbx2nd;
}
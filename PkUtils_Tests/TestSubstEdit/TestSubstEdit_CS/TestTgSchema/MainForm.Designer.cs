using PK.TestTgSchema.UserCtrls;

namespace PK.TestTgSchema;

partial class MainForm
{
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
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        _splitContainer1 = new System.Windows.Forms.SplitContainer();
        pictureBox1 = new System.Windows.Forms.PictureBox();
        _tabControl = new System.Windows.Forms.TabControl();
        _tabPage1 = new System.Windows.Forms.TabPage();
        _btnDayOfWeek = new System.Windows.Forms.Button();
        _btnMonth = new System.Windows.Forms.Button();
        _btnYear = new System.Windows.Forms.Button();
        _btnDocCopyright = new System.Windows.Forms.Button();
        _toolBar = new System.Windows.Forms.ToolStrip();
        btnNew = new System.Windows.Forms.ToolStripButton();
        btnOpen = new System.Windows.Forms.ToolStripButton();
        btnSave = new System.Windows.Forms.ToolStripButton();
        toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
        _btnCut = new System.Windows.Forms.ToolStripButton();
        _btnCopy = new System.Windows.Forms.ToolStripButton();
        _btnPaste = new System.Windows.Forms.ToolStripButton();
        toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
        _btnUndo = new System.Windows.Forms.ToolStripButton();
        _btnRedo = new System.Windows.Forms.ToolStripButton();
        _textBxPreview = new System.Windows.Forms.TextBox();
        _lblPreview = new System.Windows.Forms.Label();
        _btnDog = new System.Windows.Forms.Button();
        _btnDocTitle = new System.Windows.Forms.Button();
        _btnDocAuthor = new System.Windows.Forms.Button();
        _btnProjTitle = new System.Windows.Forms.Button();
        _tgSchemaComponentslUserCtrl = new TaggingSchema_ComponentslUserCtrl();
        _tabPage2 = new System.Windows.Forms.TabPage();
        _btnInsertLine = new System.Windows.Forms.Button();
        _btnSaveLines = new System.Windows.Forms.Button();
        _btnOpenLines = new System.Windows.Forms.Button();
        _btnProjName = new System.Windows.Forms.Button();
        _btnDiameter = new System.Windows.Forms.Button();
        _btnResistance = new System.Windows.Forms.Button();
        _tgSchemaLinesUserCtrl = new TaggingSchema_LinesEnumBasedUserCtrl();
        _ctxMenuInsertLine = new System.Windows.Forms.ContextMenuStrip(components);
        ((System.ComponentModel.ISupportInitialize)_splitContainer1).BeginInit();
        _splitContainer1.Panel1.SuspendLayout();
        _splitContainer1.Panel2.SuspendLayout();
        _splitContainer1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        _tabControl.SuspendLayout();
        _tabPage1.SuspendLayout();
        _toolBar.SuspendLayout();
        _tabPage2.SuspendLayout();
        SuspendLayout();
        // 
        // splitContainer1
        // 
        _splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
        _splitContainer1.Location = new System.Drawing.Point(0, 0);
        _splitContainer1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _splitContainer1.Name = "splitContainer1";
        // 
        // splitContainer1.Panel1
        // 
        _splitContainer1.Panel1.Controls.Add(pictureBox1);
        _splitContainer1.Panel1MinSize = 50;
        // 
        // splitContainer1.Panel2
        // 
        _splitContainer1.Panel2.Controls.Add(_tabControl);
        _splitContainer1.Panel2MinSize = 50;
        _splitContainer1.Size = new System.Drawing.Size(728, 348);
        _splitContainer1.SplitterDistance = 171;
        _splitContainer1.SplitterWidth = 5;
        _splitContainer1.TabIndex = 0;
        // 
        // pictureBox1
        // 
        pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
        pictureBox1.Image = Properties.Resources.trespassers;
        pictureBox1.Location = new System.Drawing.Point(0, 0);
        pictureBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new System.Drawing.Size(171, 348);
        pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
        pictureBox1.TabIndex = 0;
        pictureBox1.TabStop = false;
        // 
        // _tabControl
        // 
        _tabControl.Controls.Add(_tabPage1);
        _tabControl.Controls.Add(_tabPage2);
        _tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
        _tabControl.Location = new System.Drawing.Point(0, 0);
        _tabControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _tabControl.Name = "_tabControl";
        _tabControl.SelectedIndex = 0;
        _tabControl.Size = new System.Drawing.Size(552, 348);
        _tabControl.TabIndex = 0;
        _tabControl.SelectedIndexChanged += OnTabControl_SelectedIndexChanged;
        // 
        // _tabPage1
        // 
        _tabPage1.Controls.Add(_btnDayOfWeek);
        _tabPage1.Controls.Add(_btnMonth);
        _tabPage1.Controls.Add(_btnYear);
        _tabPage1.Controls.Add(_btnDocCopyright);
        _tabPage1.Controls.Add(_toolBar);
        _tabPage1.Controls.Add(_textBxPreview);
        _tabPage1.Controls.Add(_lblPreview);
        _tabPage1.Controls.Add(_btnDog);
        _tabPage1.Controls.Add(_btnDocTitle);
        _tabPage1.Controls.Add(_btnDocAuthor);
        _tabPage1.Controls.Add(_btnProjTitle);
        _tabPage1.Controls.Add(_tgSchemaComponentslUserCtrl);
        _tabPage1.Location = new System.Drawing.Point(4, 24);
        _tabPage1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _tabPage1.Name = "_tabPage1";
        _tabPage1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _tabPage1.Size = new System.Drawing.Size(544, 320);
        _tabPage1.TabIndex = 0;
        _tabPage1.Text = "Components";
        _tabPage1.UseVisualStyleBackColor = true;
        // 
        // _btnDayOfWeek
        // 
        _btnDayOfWeek.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        _btnDayOfWeek.Location = new System.Drawing.Point(427, 239);
        _btnDayOfWeek.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _btnDayOfWeek.Name = "_btnDayOfWeek";
        _btnDayOfWeek.Size = new System.Drawing.Size(90, 27);
        _btnDayOfWeek.TabIndex = 9;
        _btnDayOfWeek.Text = "DayOfWeek";
        _btnDayOfWeek.UseVisualStyleBackColor = true;
        _btnDayOfWeek.Click += OnBtnDayOfWeek_Click;
        // 
        // _btnMonth
        // 
        _btnMonth.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        _btnMonth.Location = new System.Drawing.Point(426, 204);
        _btnMonth.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _btnMonth.Name = "_btnMonth";
        _btnMonth.Size = new System.Drawing.Size(90, 27);
        _btnMonth.TabIndex = 8;
        _btnMonth.Text = "Month";
        _btnMonth.UseVisualStyleBackColor = true;
        _btnMonth.Click += OnBtnMonth_Click;
        // 
        // _btnYear
        // 
        _btnYear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        _btnYear.Location = new System.Drawing.Point(426, 170);
        _btnYear.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _btnYear.Name = "_btnYear";
        _btnYear.Size = new System.Drawing.Size(90, 27);
        _btnYear.TabIndex = 7;
        _btnYear.Text = "Year";
        _btnYear.UseVisualStyleBackColor = true;
        _btnYear.Click += OnBtnYear_Click;
        // 
        // _btnDocCopyright
        // 
        _btnDocCopyright.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        _btnDocCopyright.Location = new System.Drawing.Point(308, 273);
        _btnDocCopyright.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _btnDocCopyright.Name = "_btnDocCopyright";
        _btnDocCopyright.Size = new System.Drawing.Size(90, 27);
        _btnDocCopyright.TabIndex = 6;
        _btnDocCopyright.Text = "Copyright";
        _btnDocCopyright.UseVisualStyleBackColor = true;
        _btnDocCopyright.Click += OnBtnDocCopyright_Click;
        // 
        // _toolBar
        // 
        _toolBar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        _toolBar.AutoSize = false;
        _toolBar.Dock = System.Windows.Forms.DockStyle.None;
        _toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { btnNew, btnOpen, btnSave, toolStripSeparator1, _btnCut, _btnCopy, _btnPaste, toolStripSeparator2, _btnUndo, _btnRedo });
        _toolBar.Location = new System.Drawing.Point(4, 3);
        _toolBar.Name = "_toolBar";
        _toolBar.Size = new System.Drawing.Size(513, 29);
        _toolBar.TabIndex = 0;
        _toolBar.Text = "toolStrip1";
        // 
        // btnNew
        // 
        btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        btnNew.Image = (System.Drawing.Image)resources.GetObject("btnNew.Image");
        btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
        btnNew.Name = "btnNew";
        btnNew.Size = new System.Drawing.Size(23, 26);
        btnNew.Text = "toolStripButton1";
        btnNew.ToolTipText = "New";
        btnNew.Click += OnBtnNew_Click;
        // 
        // btnOpen
        // 
        btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        btnOpen.Image = (System.Drawing.Image)resources.GetObject("btnOpen.Image");
        btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
        btnOpen.Name = "btnOpen";
        btnOpen.Size = new System.Drawing.Size(23, 26);
        btnOpen.Text = "toolStripButton2";
        btnOpen.ToolTipText = "Open";
        btnOpen.Click += OnBtnOpen_Click;
        // 
        // btnSave
        // 
        btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        btnSave.Image = (System.Drawing.Image)resources.GetObject("btnSave.Image");
        btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
        btnSave.Name = "btnSave";
        btnSave.Size = new System.Drawing.Size(23, 26);
        btnSave.Text = "toolStripButton3";
        btnSave.ToolTipText = "Save";
        btnSave.Click += OnBtnSaveAs_Click;
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new System.Drawing.Size(6, 29);
        // 
        // _btnCut
        // 
        _btnCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        _btnCut.Image = (System.Drawing.Image)resources.GetObject("_btnCut.Image");
        _btnCut.ImageTransparentColor = System.Drawing.Color.Magenta;
        _btnCut.Name = "_btnCut";
        _btnCut.Size = new System.Drawing.Size(23, 26);
        _btnCut.Text = "toolStripButton4";
        _btnCut.ToolTipText = "Cut";
        _btnCut.Click += OnBtnCut_Click;
        // 
        // _btnCopy
        // 
        _btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        _btnCopy.Image = (System.Drawing.Image)resources.GetObject("_btnCopy.Image");
        _btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
        _btnCopy.Name = "_btnCopy";
        _btnCopy.Size = new System.Drawing.Size(23, 26);
        _btnCopy.Text = "toolStripButton5";
        _btnCopy.ToolTipText = "Copy";
        _btnCopy.Click += OnBtnCopy_Click;
        // 
        // _btnPaste
        // 
        _btnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        _btnPaste.Image = (System.Drawing.Image)resources.GetObject("_btnPaste.Image");
        _btnPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
        _btnPaste.Name = "_btnPaste";
        _btnPaste.Size = new System.Drawing.Size(23, 26);
        _btnPaste.Text = "toolStripButton6";
        _btnPaste.ToolTipText = "Paste";
        _btnPaste.Click += OnBtnPaste_Click;
        // 
        // toolStripSeparator2
        // 
        toolStripSeparator2.Name = "toolStripSeparator2";
        toolStripSeparator2.Size = new System.Drawing.Size(6, 29);
        // 
        // _btnUndo
        // 
        _btnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        _btnUndo.Image = (System.Drawing.Image)resources.GetObject("_btnUndo.Image");
        _btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
        _btnUndo.Name = "_btnUndo";
        _btnUndo.Size = new System.Drawing.Size(23, 26);
        _btnUndo.Text = "toolStripButton7";
        _btnUndo.ToolTipText = "Undo Ctrl+Z, Alt+Backspace";
        _btnUndo.Click += OnBtnUndo_Click;
        // 
        // _btnRedo
        // 
        _btnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        _btnRedo.Image = (System.Drawing.Image)resources.GetObject("_btnRedo.Image");
        _btnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
        _btnRedo.Name = "_btnRedo";
        _btnRedo.Size = new System.Drawing.Size(23, 26);
        _btnRedo.Text = "toolStripButton8";
        _btnRedo.ToolTipText = "Redo Ctrl+Y";
        _btnRedo.Click += OnBtnRedo_Click;
        // 
        // _textBxPreview
        // 
        _textBxPreview.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        _textBxPreview.Location = new System.Drawing.Point(28, 126);
        _textBxPreview.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _textBxPreview.Name = "_textBxPreview";
        _textBxPreview.ReadOnly = true;
        _textBxPreview.Size = new System.Drawing.Size(487, 23);
        _textBxPreview.TabIndex = 2;
        // 
        // _lblPreview
        // 
        _lblPreview.AutoSize = true;
        _lblPreview.Location = new System.Drawing.Point(28, 106);
        _lblPreview.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        _lblPreview.Name = "_lblPreview";
        _lblPreview.Size = new System.Drawing.Size(260, 15);
        _lblPreview.TabIndex = 1;
        _lblPreview.Text = "Result preview, fields replaced by their contents:";
        // 
        // _btnDog
        // 
        _btnDog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        _btnDog.Location = new System.Drawing.Point(426, 273);
        _btnDog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _btnDog.Name = "_btnDog";
        _btnDog.Size = new System.Drawing.Size(90, 27);
        _btnDog.TabIndex = 10;
        _btnDog.Text = "Dog";
        _btnDog.UseVisualStyleBackColor = true;
        _btnDog.Click += OnBtnDog_Click;
        // 
        // _btnDocTitle
        // 
        _btnDocTitle.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        _btnDocTitle.Location = new System.Drawing.Point(308, 239);
        _btnDocTitle.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _btnDocTitle.Name = "_btnDocTitle";
        _btnDocTitle.Size = new System.Drawing.Size(90, 27);
        _btnDocTitle.TabIndex = 5;
        _btnDocTitle.Text = "Doc.Title";
        _btnDocTitle.UseVisualStyleBackColor = true;
        _btnDocTitle.Click += OnBtnDocTitle_Click;
        // 
        // _btnDocAuthor
        // 
        _btnDocAuthor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        _btnDocAuthor.Location = new System.Drawing.Point(308, 204);
        _btnDocAuthor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _btnDocAuthor.Name = "_btnDocAuthor";
        _btnDocAuthor.Size = new System.Drawing.Size(90, 27);
        _btnDocAuthor.TabIndex = 4;
        _btnDocAuthor.Text = "Doc.Author";
        _btnDocAuthor.UseVisualStyleBackColor = true;
        _btnDocAuthor.Click += OnBtnDocAuthor_Click;
        // 
        // _btnProjTitle
        // 
        _btnProjTitle.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        _btnProjTitle.Location = new System.Drawing.Point(308, 170);
        _btnProjTitle.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _btnProjTitle.Name = "_btnProjTitle";
        _btnProjTitle.Size = new System.Drawing.Size(90, 27);
        _btnProjTitle.TabIndex = 3;
        _btnProjTitle.Text = "Prj.Title";
        _btnProjTitle.UseVisualStyleBackColor = true;
        _btnProjTitle.Click += OnBtnProjTitle_Click;
        // 
        // _tgSchemaComponentslUserCtrl
        // 
        _tgSchemaComponentslUserCtrl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        _tgSchemaComponentslUserCtrl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        _tgSchemaComponentslUserCtrl.Location = new System.Drawing.Point(0, 0);
        _tgSchemaComponentslUserCtrl.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
        _tgSchemaComponentslUserCtrl.Name = "_tgSchemaComponentslUserCtrl";
        _tgSchemaComponentslUserCtrl.Size = new System.Drawing.Size(542, 100);
        _tgSchemaComponentslUserCtrl.TabIndex = 0;
        // 
        // _tabPage2
        // 
        _tabPage2.Controls.Add(_btnInsertLine);
        _tabPage2.Controls.Add(_btnSaveLines);
        _tabPage2.Controls.Add(_btnOpenLines);
        _tabPage2.Controls.Add(_btnProjName);
        _tabPage2.Controls.Add(_btnDiameter);
        _tabPage2.Controls.Add(_btnResistance);
        _tabPage2.Controls.Add(_tgSchemaLinesUserCtrl);
        _tabPage2.Location = new System.Drawing.Point(4, 24);
        _tabPage2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _tabPage2.Name = "_tabPage2";
        _tabPage2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _tabPage2.Size = new System.Drawing.Size(544, 320);
        _tabPage2.TabIndex = 1;
        _tabPage2.Text = "Lines";
        _tabPage2.UseVisualStyleBackColor = true;
        // 
        // _btnInsertLine
        // 
        _btnInsertLine.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        _btnInsertLine.Location = new System.Drawing.Point(404, 156);
        _btnInsertLine.Name = "_btnInsertLine";
        _btnInsertLine.Size = new System.Drawing.Size(124, 27);
        _btnInsertLine.TabIndex = 9;
        _btnInsertLine.Text = "Insert ...";
        _btnInsertLine.UseVisualStyleBackColor = true;
        _btnInsertLine.Click += OnBtnInsertLine_Click;
        // 
        // _btnSaveLines
        // 
        _btnSaveLines.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        _btnSaveLines.Location = new System.Drawing.Point(21, 266);
        _btnSaveLines.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _btnSaveLines.Name = "_btnSaveLines";
        _btnSaveLines.Size = new System.Drawing.Size(97, 27);
        _btnSaveLines.TabIndex = 8;
        _btnSaveLines.Text = "Save As...";
        _btnSaveLines.UseVisualStyleBackColor = true;
        _btnSaveLines.Click += OnBtnSaveLines_Click;
        // 
        // _btnOpenLines
        // 
        _btnOpenLines.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        _btnOpenLines.Location = new System.Drawing.Point(21, 230);
        _btnOpenLines.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _btnOpenLines.Name = "_btnOpenLines";
        _btnOpenLines.Size = new System.Drawing.Size(97, 27);
        _btnOpenLines.TabIndex = 7;
        _btnOpenLines.Text = "Open...";
        _btnOpenLines.UseVisualStyleBackColor = true;
        _btnOpenLines.Click += OnBtnOpenLines_Click;
        // 
        // _btnProjName
        // 
        _btnProjName.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        _btnProjName.Location = new System.Drawing.Point(404, 266);
        _btnProjName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _btnProjName.Name = "_btnProjName";
        _btnProjName.Size = new System.Drawing.Size(124, 27);
        _btnProjName.TabIndex = 6;
        _btnProjName.Text = "Project name";
        _btnProjName.UseVisualStyleBackColor = true;
        _btnProjName.Click += OnBtnProjName_Click;
        // 
        // _btnDiameter
        // 
        _btnDiameter.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        _btnDiameter.Location = new System.Drawing.Point(404, 230);
        _btnDiameter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _btnDiameter.Name = "_btnDiameter";
        _btnDiameter.Size = new System.Drawing.Size(124, 27);
        _btnDiameter.TabIndex = 5;
        _btnDiameter.Text = "Wire diameter";
        _btnDiameter.UseVisualStyleBackColor = true;
        _btnDiameter.Click += OnBtnDiameter_Click;
        // 
        // _btnResistance
        // 
        _btnResistance.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        _btnResistance.Location = new System.Drawing.Point(404, 194);
        _btnResistance.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        _btnResistance.Name = "_btnResistance";
        _btnResistance.Size = new System.Drawing.Size(124, 27);
        _btnResistance.TabIndex = 4;
        _btnResistance.Text = "Wire resistance";
        _btnResistance.UseVisualStyleBackColor = true;
        _btnResistance.Click += OnBtnResistance_Click;
        // 
        // _tgSchemaLinesUserCtrl
        // 
        _tgSchemaLinesUserCtrl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        _tgSchemaLinesUserCtrl.Location = new System.Drawing.Point(0, 0);
        _tgSchemaLinesUserCtrl.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
        _tgSchemaLinesUserCtrl.Name = "_tgSchemaLinesUserCtrl";
        _tgSchemaLinesUserCtrl.Size = new System.Drawing.Size(527, 159);
        _tgSchemaLinesUserCtrl.TabIndex = 1;
        // 
        // _ctxMenuInsertLine
        // 
        _ctxMenuInsertLine.Name = "_ctxMenuInsertLine";
        _ctxMenuInsertLine.Size = new System.Drawing.Size(181, 26);
        // 
        // MainForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(728, 348);
        Controls.Add(_splitContainer1);
        Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        MinimumSize = new System.Drawing.Size(744, 386);
        Name = "MainForm";
        Text = "MainForm";
        FormClosing += MainFor_FormClosing;
        _splitContainer1.Panel1.ResumeLayout(false);
        _splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)_splitContainer1).EndInit();
        _splitContainer1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        _tabControl.ResumeLayout(false);
        _tabPage1.ResumeLayout(false);
        _tabPage1.PerformLayout();
        _toolBar.ResumeLayout(false);
        _toolBar.PerformLayout();
        _tabPage2.ResumeLayout(false);
        ResumeLayout(false);

    }
    #endregion

    private System.Windows.Forms.SplitContainer _splitContainer1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.TabControl _tabControl;
    private System.Windows.Forms.TabPage _tabPage1;
    private System.Windows.Forms.TabPage _tabPage2;
    private TaggingSchema_ComponentslUserCtrl _tgSchemaComponentslUserCtrl;
    private TaggingSchema_LinesEnumBasedUserCtrl _tgSchemaLinesUserCtrl;
    private System.Windows.Forms.Button _btnDog;
    private System.Windows.Forms.Button _btnDocTitle;
    private System.Windows.Forms.Button _btnDocAuthor;
    private System.Windows.Forms.Button _btnProjTitle;
    private System.Windows.Forms.Button _btnProjName;
    private System.Windows.Forms.Button _btnDiameter;
    private System.Windows.Forms.Button _btnResistance;
    private System.Windows.Forms.TextBox _textBxPreview;
    private System.Windows.Forms.Label _lblPreview;
    private System.Windows.Forms.Button _btnSaveLines;
    private System.Windows.Forms.Button _btnOpenLines;
    private System.Windows.Forms.ToolStrip _toolBar;
    private System.Windows.Forms.ToolStripButton btnNew;
    private System.Windows.Forms.ToolStripButton btnOpen;
    private System.Windows.Forms.ToolStripButton btnSave;
    private System.Windows.Forms.ToolStripButton _btnCut;
    private System.Windows.Forms.ToolStripButton _btnCopy;
    private System.Windows.Forms.ToolStripButton _btnPaste;
    private System.Windows.Forms.ToolStripButton _btnUndo;
    private System.Windows.Forms.ToolStripButton _btnRedo;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.Button _btnDocCopyright;
    private System.Windows.Forms.Button _btnDayOfWeek;
    private System.Windows.Forms.Button _btnMonth;
    private System.Windows.Forms.Button _btnYear;
    private System.Windows.Forms.Button _btnInsertLine;
    private System.Windows.Forms.ContextMenuStrip _ctxMenuInsertLine;
}


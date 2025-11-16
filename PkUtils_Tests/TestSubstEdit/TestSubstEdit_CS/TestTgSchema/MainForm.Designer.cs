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
  System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
  this.splitContainer1 = new System.Windows.Forms.SplitContainer();
  this.pictureBox1 = new System.Windows.Forms.PictureBox();
  this._tabControl = new System.Windows.Forms.TabControl();
  this._tabPage1 = new System.Windows.Forms.TabPage();
  this._btnDayOfWeek = new System.Windows.Forms.Button();
  this._btnMonth = new System.Windows.Forms.Button();
  this._btnYear = new System.Windows.Forms.Button();
  this._btnDocCopyright = new System.Windows.Forms.Button();
  this._toolBar = new System.Windows.Forms.ToolStrip();
  this.btnNew = new System.Windows.Forms.ToolStripButton();
  this.btnOpen = new System.Windows.Forms.ToolStripButton();
  this.btnSave = new System.Windows.Forms.ToolStripButton();
  this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
  this._btnCut = new System.Windows.Forms.ToolStripButton();
  this._btnCopy = new System.Windows.Forms.ToolStripButton();
  this._btnPaste = new System.Windows.Forms.ToolStripButton();
  this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
  this._btnUndo = new System.Windows.Forms.ToolStripButton();
  this._btnRedo = new System.Windows.Forms.ToolStripButton();
  this._textBxPreview = new System.Windows.Forms.TextBox();
  this._lblPreview = new System.Windows.Forms.Label();
  this._btnDog = new System.Windows.Forms.Button();
  this._btnDocTitle = new System.Windows.Forms.Button();
  this._btnDocAuthor = new System.Windows.Forms.Button();
  this._btnProjTitle = new System.Windows.Forms.Button();
  this._tgSchemaComponentslUserCtrl = new TaggingSchema_ComponentslUserCtrl();
  this._tabPage2 = new System.Windows.Forms.TabPage();
  this._btnSaveLines = new System.Windows.Forms.Button();
  this._btnOpenLines = new System.Windows.Forms.Button();
  this._btnProjName = new System.Windows.Forms.Button();
  this._btnDiameter = new System.Windows.Forms.Button();
  this._btnResistance = new System.Windows.Forms.Button();
  this._tgSchemaLinesUserCtrl = new TaggingSchema_LinesEnumBasedUserCtrl();
  this.splitContainer1.Panel1.SuspendLayout();
  this.splitContainer1.Panel2.SuspendLayout();
  this.splitContainer1.SuspendLayout();
  ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
  this._tabControl.SuspendLayout();
  this._tabPage1.SuspendLayout();
  this._toolBar.SuspendLayout();
  this._tabPage2.SuspendLayout();
  this.SuspendLayout();
  // 
  // splitContainer1
  // 
  this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
  this.splitContainer1.Location = new System.Drawing.Point(0, 0);
  this.splitContainer1.Name = "splitContainer1";
  // 
  // splitContainer1.Panel1
  // 
  this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
  this.splitContainer1.Panel1MinSize = 50;
  // 
  // splitContainer1.Panel2
  // 
  this.splitContainer1.Panel2.Controls.Add(this._tabControl);
  this.splitContainer1.Panel2MinSize = 50;
  this.splitContainer1.Size = new System.Drawing.Size(624, 302);
  this.splitContainer1.SplitterDistance = 147;
  this.splitContainer1.TabIndex = 0;
  // 
  // pictureBox1
  // 
  this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
  this.pictureBox1.Image = global::PK.TestTgSchema.Properties.Resources.trespassers;
  this.pictureBox1.Location = new System.Drawing.Point(0, 0);
  this.pictureBox1.Name = "pictureBox1";
  this.pictureBox1.Size = new System.Drawing.Size(147, 302);
  this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
  this.pictureBox1.TabIndex = 0;
  this.pictureBox1.TabStop = false;
  // 
  // _tabControl
  // 
  this._tabControl.Controls.Add(this._tabPage1);
  this._tabControl.Controls.Add(this._tabPage2);
  this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
  this._tabControl.Location = new System.Drawing.Point(0, 0);
  this._tabControl.Name = "_tabControl";
  this._tabControl.SelectedIndex = 0;
  this._tabControl.Size = new System.Drawing.Size(473, 302);
  this._tabControl.TabIndex = 0;
  this._tabControl.SelectedIndexChanged += new System.EventHandler(this.OnTabControl_SelectedIndexChanged);
  // 
  // _tabPage1
  // 
  this._tabPage1.Controls.Add(this._btnDayOfWeek);
  this._tabPage1.Controls.Add(this._btnMonth);
  this._tabPage1.Controls.Add(this._btnYear);
  this._tabPage1.Controls.Add(this._btnDocCopyright);
  this._tabPage1.Controls.Add(this._toolBar);
  this._tabPage1.Controls.Add(this._textBxPreview);
  this._tabPage1.Controls.Add(this._lblPreview);
  this._tabPage1.Controls.Add(this._btnDog);
  this._tabPage1.Controls.Add(this._btnDocTitle);
  this._tabPage1.Controls.Add(this._btnDocAuthor);
  this._tabPage1.Controls.Add(this._btnProjTitle);
  this._tabPage1.Controls.Add(this._tgSchemaComponentslUserCtrl);
  this._tabPage1.Location = new System.Drawing.Point(4, 22);
  this._tabPage1.Name = "_tabPage1";
  this._tabPage1.Padding = new System.Windows.Forms.Padding(3);
  this._tabPage1.Size = new System.Drawing.Size(465, 276);
  this._tabPage1.TabIndex = 0;
  this._tabPage1.Text = "Components";
  this._tabPage1.UseVisualStyleBackColor = true;
  // 
  // _btnDayOfWeek
  // 
  this._btnDayOfWeek.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
  this._btnDayOfWeek.Location = new System.Drawing.Point(366, 207);
  this._btnDayOfWeek.Name = "_btnDayOfWeek";
  this._btnDayOfWeek.Size = new System.Drawing.Size(77, 23);
  this._btnDayOfWeek.TabIndex = 9;
  this._btnDayOfWeek.Text = "DayOfWeek";
  this._btnDayOfWeek.UseVisualStyleBackColor = true;
  this._btnDayOfWeek.Click += new System.EventHandler(this.OnBtnDayOfWeek_Click);
  // 
  // _btnMonth
  // 
  this._btnMonth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
  this._btnMonth.Location = new System.Drawing.Point(365, 177);
  this._btnMonth.Name = "_btnMonth";
  this._btnMonth.Size = new System.Drawing.Size(77, 23);
  this._btnMonth.TabIndex = 8;
  this._btnMonth.Text = "Month";
  this._btnMonth.UseVisualStyleBackColor = true;
  this._btnMonth.Click += new System.EventHandler(this.OnBtnMonth_Click);
  // 
  // _btnYear
  // 
  this._btnYear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
  this._btnYear.Location = new System.Drawing.Point(365, 147);
  this._btnYear.Name = "_btnYear";
  this._btnYear.Size = new System.Drawing.Size(77, 23);
  this._btnYear.TabIndex = 7;
  this._btnYear.Text = "Year";
  this._btnYear.UseVisualStyleBackColor = true;
  this._btnYear.Click += new System.EventHandler(this.OnBtnYear_Click);
  // 
  // _btnDocCopyright
  // 
  this._btnDocCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
  this._btnDocCopyright.Location = new System.Drawing.Point(264, 237);
  this._btnDocCopyright.Name = "_btnDocCopyright";
  this._btnDocCopyright.Size = new System.Drawing.Size(77, 23);
  this._btnDocCopyright.TabIndex = 6;
  this._btnDocCopyright.Text = "Copyright";
  this._btnDocCopyright.UseVisualStyleBackColor = true;
  this._btnDocCopyright.Click += new System.EventHandler(this.OnBtnDocCopyright_Click);
  // 
  // _toolBar
  // 
  this._toolBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
  this._toolBar.AutoSize = false;
  this._toolBar.Dock = System.Windows.Forms.DockStyle.None;
  this._toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.btnNew,
        this.btnOpen,
        this.btnSave,
        this.toolStripSeparator1,
        this._btnCut,
        this._btnCopy,
        this._btnPaste,
        this.toolStripSeparator2,
        this._btnUndo,
        this._btnRedo});
  this._toolBar.Location = new System.Drawing.Point(3, 3);
  this._toolBar.Name = "_toolBar";
  this._toolBar.Size = new System.Drawing.Size(440, 25);
  this._toolBar.TabIndex = 0;
  this._toolBar.Text = "toolStrip1";
  // 
  // btnNew
  // 
  this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
  this.btnNew.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.Image")));
  this.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
  this.btnNew.Name = "btnNew";
  this.btnNew.Size = new System.Drawing.Size(23, 22);
  this.btnNew.Text = "toolStripButton1";
  this.btnNew.ToolTipText = "New";
  this.btnNew.Click += new System.EventHandler(this.OnBtnNew_Click);
  // 
  // btnOpen
  // 
  this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
  this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
  this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
  this.btnOpen.Name = "btnOpen";
  this.btnOpen.Size = new System.Drawing.Size(23, 22);
  this.btnOpen.Text = "toolStripButton2";
  this.btnOpen.ToolTipText = "Open";
  this.btnOpen.Click += new System.EventHandler(this.OnBtnOpen_Click);
  // 
  // btnSave
  // 
  this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
  this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
  this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
  this.btnSave.Name = "btnSave";
  this.btnSave.Size = new System.Drawing.Size(23, 22);
  this.btnSave.Text = "toolStripButton3";
  this.btnSave.ToolTipText = "Save";
  this.btnSave.Click += new System.EventHandler(this.OnBtnSaveAs_Click);
  // 
  // toolStripSeparator1
  // 
  this.toolStripSeparator1.Name = "toolStripSeparator1";
  this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
  // 
  // _btnCut
  // 
  this._btnCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
  this._btnCut.Image = ((System.Drawing.Image)(resources.GetObject("_btnCut.Image")));
  this._btnCut.ImageTransparentColor = System.Drawing.Color.Magenta;
  this._btnCut.Name = "_btnCut";
  this._btnCut.Size = new System.Drawing.Size(23, 22);
  this._btnCut.Text = "toolStripButton4";
  this._btnCut.ToolTipText = "Cut";
  this._btnCut.Click += new System.EventHandler(this.OnBtnCut_Click);
  // 
  // _btnCopy
  // 
  this._btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
  this._btnCopy.Image = ((System.Drawing.Image)(resources.GetObject("_btnCopy.Image")));
  this._btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
  this._btnCopy.Name = "_btnCopy";
  this._btnCopy.Size = new System.Drawing.Size(23, 22);
  this._btnCopy.Text = "toolStripButton5";
  this._btnCopy.ToolTipText = "Copy";
  this._btnCopy.Click += new System.EventHandler(this.OnBtnCopy_Click);
  // 
  // _btnPaste
  // 
  this._btnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
  this._btnPaste.Image = ((System.Drawing.Image)(resources.GetObject("_btnPaste.Image")));
  this._btnPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
  this._btnPaste.Name = "_btnPaste";
  this._btnPaste.Size = new System.Drawing.Size(23, 22);
  this._btnPaste.Text = "toolStripButton6";
  this._btnPaste.ToolTipText = "Paste";
  this._btnPaste.Click += new System.EventHandler(this.OnBtnPaste_Click);
  // 
  // toolStripSeparator2
  // 
  this.toolStripSeparator2.Name = "toolStripSeparator2";
  this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
  // 
  // _btnUndo
  // 
  this._btnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
  this._btnUndo.Image = ((System.Drawing.Image)(resources.GetObject("_btnUndo.Image")));
  this._btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
  this._btnUndo.Name = "_btnUndo";
  this._btnUndo.Size = new System.Drawing.Size(23, 22);
  this._btnUndo.Text = "toolStripButton7";
  this._btnUndo.ToolTipText = "Undo Ctrl+Z, Alt+Backspace";
  this._btnUndo.Click += new System.EventHandler(this.OnBtnUndo_Click);
  // 
  // _btnRedo
  // 
  this._btnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
  this._btnRedo.Image = ((System.Drawing.Image)(resources.GetObject("_btnRedo.Image")));
  this._btnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
  this._btnRedo.Name = "_btnRedo";
  this._btnRedo.Size = new System.Drawing.Size(23, 22);
  this._btnRedo.Text = "toolStripButton8";
  this._btnRedo.ToolTipText = "Redo Ctrl+Y";
  this._btnRedo.Click += new System.EventHandler(this.OnBtnRedo_Click);
  // 
  // _textBxPreview
  // 
  this._textBxPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
  this._textBxPreview.Location = new System.Drawing.Point(24, 109);
  this._textBxPreview.Name = "_textBxPreview";
  this._textBxPreview.ReadOnly = true;
  this._textBxPreview.Size = new System.Drawing.Size(418, 20);
  this._textBxPreview.TabIndex = 2;
  // 
  // _lblPreview
  // 
  this._lblPreview.AutoSize = true;
  this._lblPreview.Location = new System.Drawing.Point(24, 92);
  this._lblPreview.Name = "_lblPreview";
  this._lblPreview.Size = new System.Drawing.Size(235, 13);
  this._lblPreview.TabIndex = 1;
  this._lblPreview.Text = "Result preview, fields replaced by their contents:";
  // 
  // _btnDog
  // 
  this._btnDog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
  this._btnDog.Location = new System.Drawing.Point(365, 237);
  this._btnDog.Name = "_btnDog";
  this._btnDog.Size = new System.Drawing.Size(77, 23);
  this._btnDog.TabIndex = 10;
  this._btnDog.Text = "Dog";
  this._btnDog.UseVisualStyleBackColor = true;
  this._btnDog.Click += new System.EventHandler(this.OnBtnDog_Click);
  // 
  // _btnDocTitle
  // 
  this._btnDocTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
  this._btnDocTitle.Location = new System.Drawing.Point(264, 207);
  this._btnDocTitle.Name = "_btnDocTitle";
  this._btnDocTitle.Size = new System.Drawing.Size(77, 23);
  this._btnDocTitle.TabIndex = 5;
  this._btnDocTitle.Text = "Doc.Title";
  this._btnDocTitle.UseVisualStyleBackColor = true;
  this._btnDocTitle.Click += new System.EventHandler(this.OnBtnDocTitle_Click);
  // 
  // _btnDocAuthor
  // 
  this._btnDocAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
  this._btnDocAuthor.Location = new System.Drawing.Point(264, 177);
  this._btnDocAuthor.Name = "_btnDocAuthor";
  this._btnDocAuthor.Size = new System.Drawing.Size(77, 23);
  this._btnDocAuthor.TabIndex = 4;
  this._btnDocAuthor.Text = "Doc.Author";
  this._btnDocAuthor.UseVisualStyleBackColor = true;
  this._btnDocAuthor.Click += new System.EventHandler(this.OnBtnDocAuthor_Click);
  // 
  // _btnProjTitle
  // 
  this._btnProjTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
  this._btnProjTitle.Location = new System.Drawing.Point(264, 147);
  this._btnProjTitle.Name = "_btnProjTitle";
  this._btnProjTitle.Size = new System.Drawing.Size(77, 23);
  this._btnProjTitle.TabIndex = 3;
  this._btnProjTitle.Text = "Prj.Title";
  this._btnProjTitle.UseVisualStyleBackColor = true;
  this._btnProjTitle.Click += new System.EventHandler(this.OnBtnProjTitle_Click);
  // 
  // _tgSchemaComponentslUserCtrl
  // 
  this._tgSchemaComponentslUserCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
  this._tgSchemaComponentslUserCtrl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
  this._tgSchemaComponentslUserCtrl.Location = new System.Drawing.Point(0, 0);
  this._tgSchemaComponentslUserCtrl.Name = "_tgSchemaComponentslUserCtrl";
  this._tgSchemaComponentslUserCtrl.Size = new System.Drawing.Size(465, 87);
  this._tgSchemaComponentslUserCtrl.TabIndex = 0;
  // 
  // _tabPage2
  // 
  this._tabPage2.Controls.Add(this._btnSaveLines);
  this._tabPage2.Controls.Add(this._btnOpenLines);
  this._tabPage2.Controls.Add(this._btnProjName);
  this._tabPage2.Controls.Add(this._btnDiameter);
  this._tabPage2.Controls.Add(this._btnResistance);
  this._tabPage2.Controls.Add(this._tgSchemaLinesUserCtrl);
  this._tabPage2.Location = new System.Drawing.Point(4, 22);
  this._tabPage2.Name = "_tabPage2";
  this._tabPage2.Padding = new System.Windows.Forms.Padding(3);
  this._tabPage2.Size = new System.Drawing.Size(465, 276);
  this._tabPage2.TabIndex = 1;
  this._tabPage2.Text = "Lines";
  this._tabPage2.UseVisualStyleBackColor = true;
  // 
  // _btnSaveLines
  // 
  this._btnSaveLines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
  this._btnSaveLines.Location = new System.Drawing.Point(18, 225);
  this._btnSaveLines.Name = "_btnSaveLines";
  this._btnSaveLines.Size = new System.Drawing.Size(83, 23);
  this._btnSaveLines.TabIndex = 8;
  this._btnSaveLines.Text = "Save As...";
  this._btnSaveLines.UseVisualStyleBackColor = true;
  this._btnSaveLines.Click += new System.EventHandler(this.OnBtnSaveLines_Click);
  // 
  // _btnOpenLines
  // 
  this._btnOpenLines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
  this._btnOpenLines.Location = new System.Drawing.Point(18, 194);
  this._btnOpenLines.Name = "_btnOpenLines";
  this._btnOpenLines.Size = new System.Drawing.Size(83, 23);
  this._btnOpenLines.TabIndex = 7;
  this._btnOpenLines.Text = "Open...";
  this._btnOpenLines.UseVisualStyleBackColor = true;
  this._btnOpenLines.Click += new System.EventHandler(this.OnBtnOpenLines_Click);
  // 
  // _btnProjName
  // 
  this._btnProjName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
  this._btnProjName.Location = new System.Drawing.Point(346, 225);
  this._btnProjName.Name = "_btnProjName";
  this._btnProjName.Size = new System.Drawing.Size(106, 23);
  this._btnProjName.TabIndex = 6;
  this._btnProjName.Text = "Project name";
  this._btnProjName.UseVisualStyleBackColor = true;
  this._btnProjName.Click += new System.EventHandler(this.OnBtnProjName_Click);
  // 
  // _btnDiameter
  // 
  this._btnDiameter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
  this._btnDiameter.Location = new System.Drawing.Point(346, 194);
  this._btnDiameter.Name = "_btnDiameter";
  this._btnDiameter.Size = new System.Drawing.Size(106, 23);
  this._btnDiameter.TabIndex = 5;
  this._btnDiameter.Text = "Wire diameter";
  this._btnDiameter.UseVisualStyleBackColor = true;
  this._btnDiameter.Click += new System.EventHandler(this.OnBtnDiameter_Click);
  // 
  // _btnResistance
  // 
  this._btnResistance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
  this._btnResistance.Location = new System.Drawing.Point(346, 163);
  this._btnResistance.Name = "_btnResistance";
  this._btnResistance.Size = new System.Drawing.Size(106, 23);
  this._btnResistance.TabIndex = 4;
  this._btnResistance.Text = "Wire resistance";
  this._btnResistance.UseVisualStyleBackColor = true;
  this._btnResistance.Click += new System.EventHandler(this.OnBtnResistance_Click);
  // 
  // _tgSchemaLinesUserCtrl
  // 
  this._tgSchemaLinesUserCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
  this._tgSchemaLinesUserCtrl.Location = new System.Drawing.Point(0, 0);
  this._tgSchemaLinesUserCtrl.Name = "_tgSchemaLinesUserCtrl";
  this._tgSchemaLinesUserCtrl.Size = new System.Drawing.Size(452, 138);
  this._tgSchemaLinesUserCtrl.TabIndex = 1;
  // 
  // MainForm
  // 
  this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
  this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
  this.ClientSize = new System.Drawing.Size(624, 302);
  this.Controls.Add(this.splitContainer1);
  this.MinimumSize = new System.Drawing.Size(640, 340);
  this.Name = "MainForm";
  this.Text = "MainForm";
  this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFor_FormClosing);
  this.splitContainer1.Panel1.ResumeLayout(false);
  this.splitContainer1.Panel2.ResumeLayout(false);
  this.splitContainer1.ResumeLayout(false);
  ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
  this._tabControl.ResumeLayout(false);
  this._tabPage1.ResumeLayout(false);
  this._tabPage1.PerformLayout();
  this._toolBar.ResumeLayout(false);
  this._toolBar.PerformLayout();
  this._tabPage2.ResumeLayout(false);
  this.ResumeLayout(false);

}

#endregion

private System.Windows.Forms.SplitContainer splitContainer1;
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
}


using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.UI.General;

namespace TestMultiSelectTreeView;

partial class TestForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        TreeNode treeNode1 = new TreeNode("Node0");
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestForm));
        _treeView = new MultiSelectTreeView();
        _treeImageList = new ImageList(components);
        _btnSelectNodes = new Button();
        _btnClearSelection = new Button();
        _btnExit = new Button();
        _checkBoxCustomColors = new CheckBox();
        _checkBoxUseImages = new CheckBox();
        _splitContainer = new SplitContainer();
        _dumpTextBox = new TextBox();
        _checkBoxShowLog = new CheckBox();
        _btnClearLog = new Button();
        _btnDeleteNodes = new Button();
        _btnInsertNodes = new Button();
        ((System.ComponentModel.ISupportInitialize)_splitContainer).BeginInit();
        _splitContainer.Panel1.SuspendLayout();
        _splitContainer.Panel2.SuspendLayout();
        _splitContainer.SuspendLayout();
        SuspendLayout();
        // 
        // _treeView
        // 
        _treeView.Dock = DockStyle.Fill;
        _treeView.ImageIndex = 0;
        _treeView.ImageList = _treeImageList;
        _treeView.Location = new Point(0, 0);
        _treeView.Margin = new Padding(4);
        _treeView.Name = "_treeView";
        treeNode1.BackColor = SystemColors.Highlight;
        treeNode1.ForeColor = SystemColors.HighlightText;
        treeNode1.Name = "Node0";
        treeNode1.Text = "Node0";
        _treeView.Nodes.AddRange(new TreeNode[] { treeNode1 });
        _treeView.SelectedImageIndex = 0;
        _treeView.SelectedNodes = (IReadOnlyCollection<TreeNode>)resources.GetObject("_treeView.SelectedNodes");
        _treeView.Size = new Size(374, 236);
        _treeView.TabIndex = 0;
        _treeView.SelectionChanged += OnMultiSelectTreeView_SelectionChanged;
        // 
        // _treeImageList
        // 
        _treeImageList.ColorDepth = ColorDepth.Depth32Bit;
        _treeImageList.ImageStream = (ImageListStreamer)resources.GetObject("_treeImageList.ImageStream");
        _treeImageList.TransparentColor = Color.Transparent;
        _treeImageList.Images.SetKeyName(0, "IL_0_icoBook.ico");
        _treeImageList.Images.SetKeyName(1, "IL_1_selBook.ico");
        _treeImageList.Images.SetKeyName(2, "IL_2_icoLibrary.ico");
        _treeImageList.Images.SetKeyName(3, "IL_3_selLibrary.ico");
        _treeImageList.Images.SetKeyName(4, "IL_4_icoNote.ico");
        _treeImageList.Images.SetKeyName(5, "selLibrary.ico");
        // 
        // _btnSelectNodes
        // 
        _btnSelectNodes.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _btnSelectNodes.Location = new Point(278, 437);
        _btnSelectNodes.Margin = new Padding(4);
        _btnSelectNodes.Name = "_btnSelectNodes";
        _btnSelectNodes.Size = new Size(110, 26);
        _btnSelectNodes.TabIndex = 7;
        _btnSelectNodes.Text = "Select 4 Nodes";
        _btnSelectNodes.UseVisualStyleBackColor = true;
        _btnSelectNodes.Click += OnButton_SelectNodes;
        // 
        // _btnClearSelection
        // 
        _btnClearSelection.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _btnClearSelection.Location = new Point(278, 405);
        _btnClearSelection.Margin = new Padding(4);
        _btnClearSelection.Name = "_btnClearSelection";
        _btnClearSelection.Size = new Size(110, 26);
        _btnClearSelection.TabIndex = 6;
        _btnClearSelection.Text = "Clear Selection";
        _btnClearSelection.UseVisualStyleBackColor = true;
        _btnClearSelection.Click += OnButton_ClearSelection_Click;
        // 
        // _btnExit
        // 
        _btnExit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _btnExit.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 238);
        _btnExit.Location = new Point(278, 469);
        _btnExit.Margin = new Padding(4);
        _btnExit.Name = "_btnExit";
        _btnExit.Size = new Size(110, 26);
        _btnExit.TabIndex = 8;
        _btnExit.Text = "Exit";
        _btnExit.UseVisualStyleBackColor = true;
        _btnExit.Click += OnButton_Exit_Click;
        // 
        // _checkBoxCustomColors
        // 
        _checkBoxCustomColors.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        _checkBoxCustomColors.AutoSize = true;
        _checkBoxCustomColors.Location = new Point(18, 443);
        _checkBoxCustomColors.Margin = new Padding(3, 2, 3, 2);
        _checkBoxCustomColors.Name = "_checkBoxCustomColors";
        _checkBoxCustomColors.Size = new Size(123, 19);
        _checkBoxCustomColors.TabIndex = 1;
        _checkBoxCustomColors.Text = "Use custom colors";
        _checkBoxCustomColors.UseVisualStyleBackColor = true;
        _checkBoxCustomColors.CheckedChanged += OnCheckBoxBackgroundImage_CheckedChanged;
        // 
        // _checkBoxUseImages
        // 
        _checkBoxUseImages.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        _checkBoxUseImages.AutoSize = true;
        _checkBoxUseImages.Location = new Point(18, 474);
        _checkBoxUseImages.Margin = new Padding(3, 2, 3, 2);
        _checkBoxUseImages.Name = "_checkBoxUseImages";
        _checkBoxUseImages.Size = new Size(86, 19);
        _checkBoxUseImages.TabIndex = 2;
        _checkBoxUseImages.Text = "Use images";
        _checkBoxUseImages.UseVisualStyleBackColor = true;
        _checkBoxUseImages.CheckedChanged += OnCheckBoxUseImages_CheckedChanged;
        // 
        // _splitContainer
        // 
        _splitContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _splitContainer.Location = new Point(14, 18);
        _splitContainer.Margin = new Padding(2);
        _splitContainer.MinimumSize = new Size(128, 128);
        _splitContainer.Name = "_splitContainer";
        _splitContainer.Orientation = Orientation.Horizontal;
        // 
        // _splitContainer.Panel1
        // 
        _splitContainer.Panel1.Controls.Add(_treeView);
        _splitContainer.Panel1MinSize = 100;
        // 
        // _splitContainer.Panel2
        // 
        _splitContainer.Panel2.Controls.Add(_dumpTextBox);
        _splitContainer.Panel2MinSize = 64;
        _splitContainer.Size = new Size(374, 380);
        _splitContainer.SplitterDistance = 236;
        _splitContainer.TabIndex = 0;
        // 
        // _dumpTextBox
        // 
        _dumpTextBox.Dock = DockStyle.Fill;
        _dumpTextBox.Location = new Point(0, 0);
        _dumpTextBox.MaxLength = 65535;
        _dumpTextBox.Multiline = true;
        _dumpTextBox.Name = "_dumpTextBox";
        _dumpTextBox.ReadOnly = true;
        _dumpTextBox.ScrollBars = ScrollBars.Both;
        _dumpTextBox.Size = new Size(374, 140);
        _dumpTextBox.TabIndex = 0;
        // 
        // _checkBoxShowLog
        // 
        _checkBoxShowLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        _checkBoxShowLog.AutoSize = true;
        _checkBoxShowLog.Location = new Point(18, 411);
        _checkBoxShowLog.Margin = new Padding(3, 2, 3, 2);
        _checkBoxShowLog.Name = "_checkBoxShowLog";
        _checkBoxShowLog.Size = new Size(78, 19);
        _checkBoxShowLog.TabIndex = 0;
        _checkBoxShowLog.Text = "Show Log";
        _checkBoxShowLog.UseVisualStyleBackColor = true;
        _checkBoxShowLog.CheckedChanged += OnCheckBoxShowLog_CheckedChanged;
        // 
        // _btnClearLog
        // 
        _btnClearLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _btnClearLog.Location = new Point(155, 470);
        _btnClearLog.Margin = new Padding(4);
        _btnClearLog.Name = "_btnClearLog";
        _btnClearLog.Size = new Size(110, 26);
        _btnClearLog.TabIndex = 5;
        _btnClearLog.Text = "Clear Log Text";
        _btnClearLog.UseVisualStyleBackColor = true;
        _btnClearLog.Click += OnButtonClearLog_Click;
        // 
        // _btnDeleteNodes
        // 
        _btnDeleteNodes.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _btnDeleteNodes.Location = new Point(155, 406);
        _btnDeleteNodes.Margin = new Padding(4);
        _btnDeleteNodes.Name = "_btnDeleteNodes";
        _btnDeleteNodes.Size = new Size(110, 26);
        _btnDeleteNodes.TabIndex = 3;
        _btnDeleteNodes.Text = "Delete Nodes";
        _btnDeleteNodes.UseVisualStyleBackColor = true;
        _btnDeleteNodes.Click += OnButtonDeleteNodes_Click;
        // 
        // _btnInsertNodes
        // 
        _btnInsertNodes.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _btnInsertNodes.Location = new Point(155, 438);
        _btnInsertNodes.Margin = new Padding(4);
        _btnInsertNodes.Name = "_btnInsertNodes";
        _btnInsertNodes.Size = new Size(110, 26);
        _btnInsertNodes.TabIndex = 4;
        _btnInsertNodes.Text = "Insert Nodes";
        _btnInsertNodes.UseVisualStyleBackColor = true;
        _btnInsertNodes.Click += OnBtnInsertNodes_Click;
        // 
        // TestForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(406, 511);
        Controls.Add(_btnInsertNodes);
        Controls.Add(_btnDeleteNodes);
        Controls.Add(_btnClearLog);
        Controls.Add(_checkBoxShowLog);
        Controls.Add(_splitContainer);
        Controls.Add(_checkBoxUseImages);
        Controls.Add(_checkBoxCustomColors);
        Controls.Add(_btnExit);
        Controls.Add(_btnClearSelection);
        Controls.Add(_btnSelectNodes);
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(4);
        MinimumSize = new Size(422, 550);
        Name = "TestForm";
        Text = "Test MultiSelect Tree View";
        FormClosing += TestForm_FormClosing;
        Load += TestForm_Load;
        KeyDown += TestForm_KeyDown;
        _splitContainer.Panel1.ResumeLayout(false);
        _splitContainer.Panel2.ResumeLayout(false);
        _splitContainer.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_splitContainer).EndInit();
        _splitContainer.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MultiSelectTreeView _treeView;
    private System.Windows.Forms.Button _btnSelectNodes;
    private System.Windows.Forms.Button _btnClearSelection;
    private System.Windows.Forms.Button _btnExit;
    private System.Windows.Forms.CheckBox _checkBoxShowLog;
    private System.Windows.Forms.CheckBox _checkBoxCustomColors;
    private System.Windows.Forms.CheckBox _checkBoxUseImages;
    private System.Windows.Forms.ImageList _treeImageList;
    private System.Windows.Forms.SplitContainer _splitContainer;
    private System.Windows.Forms.TextBox _dumpTextBox;
    private Button _btnClearLog;
    private Button _btnDeleteNodes;
    private Button _btnInsertNodes;
}


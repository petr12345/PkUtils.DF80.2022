using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.UI.General;

namespace TestMultiSelectTreeview;

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
        TreeNode treeNode41 = new TreeNode("Node16");
        TreeNode treeNode42 = new TreeNode("Node17");
        TreeNode treeNode43 = new TreeNode("Node4", new TreeNode[] { treeNode41, treeNode42 });
        TreeNode treeNode44 = new TreeNode("Node18");
        TreeNode treeNode45 = new TreeNode("Node19");
        TreeNode treeNode46 = new TreeNode("Node5", new TreeNode[] { treeNode44, treeNode45 });
        TreeNode treeNode47 = new TreeNode("Node20");
        TreeNode treeNode48 = new TreeNode("Node21");
        TreeNode treeNode49 = new TreeNode("Node6", new TreeNode[] { treeNode47, treeNode48 });
        TreeNode treeNode50 = new TreeNode("Node0", new TreeNode[] { treeNode43, treeNode46, treeNode49 });
        TreeNode treeNode51 = new TreeNode("Node22");
        TreeNode treeNode52 = new TreeNode("Node23");
        TreeNode treeNode53 = new TreeNode("Node7", new TreeNode[] { treeNode51, treeNode52 });
        TreeNode treeNode54 = new TreeNode("Node24");
        TreeNode treeNode55 = new TreeNode("Node25");
        TreeNode treeNode56 = new TreeNode("Node8", new TreeNode[] { treeNode54, treeNode55 });
        TreeNode treeNode57 = new TreeNode("Node26");
        TreeNode treeNode58 = new TreeNode("Node27");
        TreeNode treeNode59 = new TreeNode("Node9", new TreeNode[] { treeNode57, treeNode58 });
        TreeNode treeNode60 = new TreeNode("Node1", new TreeNode[] { treeNode53, treeNode56, treeNode59 });
        TreeNode treeNode61 = new TreeNode("Node38");
        TreeNode treeNode62 = new TreeNode("Node39");
        TreeNode treeNode63 = new TreeNode("Node10", new TreeNode[] { treeNode61, treeNode62 });
        TreeNode treeNode64 = new TreeNode("Node36");
        TreeNode treeNode65 = new TreeNode("Node37");
        TreeNode treeNode66 = new TreeNode("Node11", new TreeNode[] { treeNode64, treeNode65 });
        TreeNode treeNode67 = new TreeNode("Node34");
        TreeNode treeNode68 = new TreeNode("Node35");
        TreeNode treeNode69 = new TreeNode("Node12", new TreeNode[] { treeNode67, treeNode68 });
        TreeNode treeNode70 = new TreeNode("Node2", new TreeNode[] { treeNode63, treeNode66, treeNode69 });
        TreeNode treeNode71 = new TreeNode("Node32");
        TreeNode treeNode72 = new TreeNode("Node33");
        TreeNode treeNode73 = new TreeNode("Node13", new TreeNode[] { treeNode71, treeNode72 });
        TreeNode treeNode74 = new TreeNode("Node30");
        TreeNode treeNode75 = new TreeNode("Node31");
        TreeNode treeNode76 = new TreeNode("Node14", new TreeNode[] { treeNode74, treeNode75 });
        TreeNode treeNode77 = new TreeNode("Node28");
        TreeNode treeNode78 = new TreeNode("Node29");
        TreeNode treeNode79 = new TreeNode("Node15", new TreeNode[] { treeNode77, treeNode78 });
        TreeNode treeNode80 = new TreeNode("Node3", new TreeNode[] { treeNode73, treeNode76, treeNode79 });
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestForm));
        _multiSelectTreeview = new MultiSelectTreeview();
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
        ((System.ComponentModel.ISupportInitialize)_splitContainer).BeginInit();
        _splitContainer.Panel1.SuspendLayout();
        _splitContainer.Panel2.SuspendLayout();
        _splitContainer.SuspendLayout();
        SuspendLayout();
        // 
        // _multiSelectTreeview
        // 
        _multiSelectTreeview.Dock = DockStyle.Fill;
        _multiSelectTreeview.ImageIndex = 0;
        _multiSelectTreeview.ImageList = _treeImageList;
        _multiSelectTreeview.Location = new Point(0, 0);
        _multiSelectTreeview.Margin = new Padding(4);
        _multiSelectTreeview.Name = "_multiSelectTreeview";
        treeNode41.Name = "Node16";
        treeNode41.Text = "Node16";
        treeNode42.Name = "Node17";
        treeNode42.Text = "Node17";
        treeNode43.Name = "Node4";
        treeNode43.Text = "Node4";
        treeNode44.Name = "Node18";
        treeNode44.Text = "Node18";
        treeNode45.Name = "Node19";
        treeNode45.Text = "Node19";
        treeNode46.Name = "Node5";
        treeNode46.Text = "Node5";
        treeNode47.Name = "Node20";
        treeNode47.Text = "Node20";
        treeNode48.Name = "Node21";
        treeNode48.Text = "Node21";
        treeNode49.Name = "Node6";
        treeNode49.Text = "Node6";
        treeNode50.BackColor = SystemColors.Highlight;
        treeNode50.ForeColor = SystemColors.HighlightText;
        treeNode50.Name = "Node0";
        treeNode50.Text = "Node0";
        treeNode51.Name = "Node22";
        treeNode51.Text = "Node22";
        treeNode52.Name = "Node23";
        treeNode52.Text = "Node23";
        treeNode53.Name = "Node7";
        treeNode53.Text = "Node7";
        treeNode54.Name = "Node24";
        treeNode54.Text = "Node24";
        treeNode55.Name = "Node25";
        treeNode55.Text = "Node25";
        treeNode56.Name = "Node8";
        treeNode56.Text = "Node8";
        treeNode57.Name = "Node26";
        treeNode57.Text = "Node26";
        treeNode58.Name = "Node27";
        treeNode58.Text = "Node27";
        treeNode59.Name = "Node9";
        treeNode59.Text = "Node9";
        treeNode60.Name = "Node1";
        treeNode60.Text = "Node1";
        treeNode61.Name = "Node38";
        treeNode61.Text = "Node38";
        treeNode62.Name = "Node39";
        treeNode62.Text = "Node39";
        treeNode63.Name = "Node10";
        treeNode63.Text = "Node10";
        treeNode64.Name = "Node36";
        treeNode64.Text = "Node36";
        treeNode65.Name = "Node37";
        treeNode65.Text = "Node37";
        treeNode66.Name = "Node11";
        treeNode66.Text = "Node11";
        treeNode67.Name = "Node34";
        treeNode67.Text = "Node34";
        treeNode68.Name = "Node35";
        treeNode68.Text = "Node35";
        treeNode69.Name = "Node12";
        treeNode69.Text = "Node12";
        treeNode70.Name = "Node2";
        treeNode70.Text = "Node2";
        treeNode71.Name = "Node32";
        treeNode71.Text = "Node32";
        treeNode72.Name = "Node33";
        treeNode72.Text = "Node33";
        treeNode73.Name = "Node13";
        treeNode73.Text = "Node13";
        treeNode74.Name = "Node30";
        treeNode74.Text = "Node30";
        treeNode75.Name = "Node31";
        treeNode75.Text = "Node31";
        treeNode76.Name = "Node14";
        treeNode76.Text = "Node14";
        treeNode77.Name = "Node28";
        treeNode77.Text = "Node28";
        treeNode78.Name = "Node29";
        treeNode78.Text = "Node29";
        treeNode79.Name = "Node15";
        treeNode79.Text = "Node15";
        treeNode80.Name = "Node3";
        treeNode80.Text = "Node3";
        _multiSelectTreeview.Nodes.AddRange(new TreeNode[] { treeNode50, treeNode60, treeNode70, treeNode80 });
        _multiSelectTreeview.SelectedImageIndex = 0;
        _multiSelectTreeview.SelectedNodes = (IReadOnlyCollection<TreeNode>)resources.GetObject("_multiSelectTreeview.SelectedNodes");
        _multiSelectTreeview.Size = new Size(374, 236);
        _multiSelectTreeview.TabIndex = 0;
        _multiSelectTreeview.SelectionChanged += OnMultiSelectTreeview_SelectionChanged;
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
        _btnSelectNodes.Location = new Point(278, 436);
        _btnSelectNodes.Margin = new Padding(4);
        _btnSelectNodes.Name = "_btnSelectNodes";
        _btnSelectNodes.Size = new Size(110, 26);
        _btnSelectNodes.TabIndex = 4;
        _btnSelectNodes.Text = "Select Nodes";
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
        _btnClearSelection.TabIndex = 3;
        _btnClearSelection.Text = "Clear Selection";
        _btnClearSelection.UseVisualStyleBackColor = true;
        _btnClearSelection.Click += OnButton_ClearSelection_Click;
        // 
        // _btnExit
        // 
        _btnExit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _btnExit.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 238);
        _btnExit.Location = new Point(278, 467);
        _btnExit.Margin = new Padding(4);
        _btnExit.Name = "_btnExit";
        _btnExit.Size = new Size(110, 26);
        _btnExit.TabIndex = 5;
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
        _splitContainer.Panel1.Controls.Add(_multiSelectTreeview);
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
        _btnClearLog.Location = new Point(155, 467);
        _btnClearLog.Margin = new Padding(4);
        _btnClearLog.Name = "_btnClearLog";
        _btnClearLog.Size = new Size(110, 26);
        _btnClearLog.TabIndex = 6;
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
        _btnDeleteNodes.TabIndex = 7;
        _btnDeleteNodes.Text = "Delete Nodes";
        _btnDeleteNodes.UseVisualStyleBackColor = true;
        _btnDeleteNodes.Click += OnButtonDeleteNodes_Click;
        // 
        // TestForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(406, 511);
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
        Text = "Test Multiselect Tree View";
        Load += TestForm_Load;
        _splitContainer.Panel1.ResumeLayout(false);
        _splitContainer.Panel2.ResumeLayout(false);
        _splitContainer.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_splitContainer).EndInit();
        _splitContainer.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MultiSelectTreeview _multiSelectTreeview;
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
}


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
    protected override void Dispose( bool disposing )
    {
        if( disposing && ( components != null ) )
        {
            components.Dispose();
        }
        base.Dispose( disposing );
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node16");
        System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Node17");
        System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Node4", new System.Windows.Forms.TreeNode[] { treeNode1, treeNode2 });
        System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Node18");
        System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Node19");
        System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Node5", new System.Windows.Forms.TreeNode[] { treeNode4, treeNode5 });
        System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Node20");
        System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Node21");
        System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Node6", new System.Windows.Forms.TreeNode[] { treeNode7, treeNode8 });
        System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Node0", new System.Windows.Forms.TreeNode[] { treeNode3, treeNode6, treeNode9 });
        System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Node22");
        System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Node23");
        System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Node7", new System.Windows.Forms.TreeNode[] { treeNode11, treeNode12 });
        System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Node24");
        System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Node25");
        System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Node8", new System.Windows.Forms.TreeNode[] { treeNode14, treeNode15 });
        System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Node26");
        System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Node27");
        System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Node9", new System.Windows.Forms.TreeNode[] { treeNode17, treeNode18 });
        System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Node1", new System.Windows.Forms.TreeNode[] { treeNode13, treeNode16, treeNode19 });
        System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("Node38");
        System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("Node39");
        System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("Node10", new System.Windows.Forms.TreeNode[] { treeNode21, treeNode22 });
        System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("Node36");
        System.Windows.Forms.TreeNode treeNode25 = new System.Windows.Forms.TreeNode("Node37");
        System.Windows.Forms.TreeNode treeNode26 = new System.Windows.Forms.TreeNode("Node11", new System.Windows.Forms.TreeNode[] { treeNode24, treeNode25 });
        System.Windows.Forms.TreeNode treeNode27 = new System.Windows.Forms.TreeNode("Node34");
        System.Windows.Forms.TreeNode treeNode28 = new System.Windows.Forms.TreeNode("Node35");
        System.Windows.Forms.TreeNode treeNode29 = new System.Windows.Forms.TreeNode("Node12", new System.Windows.Forms.TreeNode[] { treeNode27, treeNode28 });
        System.Windows.Forms.TreeNode treeNode30 = new System.Windows.Forms.TreeNode("Node2", new System.Windows.Forms.TreeNode[] { treeNode23, treeNode26, treeNode29 });
        System.Windows.Forms.TreeNode treeNode31 = new System.Windows.Forms.TreeNode("Node32");
        System.Windows.Forms.TreeNode treeNode32 = new System.Windows.Forms.TreeNode("Node33");
        System.Windows.Forms.TreeNode treeNode33 = new System.Windows.Forms.TreeNode("Node13", new System.Windows.Forms.TreeNode[] { treeNode31, treeNode32 });
        System.Windows.Forms.TreeNode treeNode34 = new System.Windows.Forms.TreeNode("Node30");
        System.Windows.Forms.TreeNode treeNode35 = new System.Windows.Forms.TreeNode("Node31");
        System.Windows.Forms.TreeNode treeNode36 = new System.Windows.Forms.TreeNode("Node14", new System.Windows.Forms.TreeNode[] { treeNode34, treeNode35 });
        System.Windows.Forms.TreeNode treeNode37 = new System.Windows.Forms.TreeNode("Node28");
        System.Windows.Forms.TreeNode treeNode38 = new System.Windows.Forms.TreeNode("Node29");
        System.Windows.Forms.TreeNode treeNode39 = new System.Windows.Forms.TreeNode("Node15", new System.Windows.Forms.TreeNode[] { treeNode37, treeNode38 });
        System.Windows.Forms.TreeNode treeNode40 = new System.Windows.Forms.TreeNode("Node3", new System.Windows.Forms.TreeNode[] { treeNode33, treeNode36, treeNode39 });
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestForm));
        _multiSelectTreeview = new MultiSelectTreeview();
        _treeImageList = new System.Windows.Forms.ImageList(components);
        _btnSelectNodes = new System.Windows.Forms.Button();
        _btnClearNodes = new System.Windows.Forms.Button();
        _btnExit = new System.Windows.Forms.Button();
        _checkBoxCustomColors = new System.Windows.Forms.CheckBox();
        _checkBoxUseImages = new System.Windows.Forms.CheckBox();
        _splitContainer = new System.Windows.Forms.SplitContainer();
        _dumpTextBox = new System.Windows.Forms.TextBox();
        _checkBoxShowLog = new System.Windows.Forms.CheckBox();
        ((System.ComponentModel.ISupportInitialize)_splitContainer).BeginInit();
        _splitContainer.Panel1.SuspendLayout();
        _splitContainer.Panel2.SuspendLayout();
        _splitContainer.SuspendLayout();
        SuspendLayout();
        // 
        // _multiSelectTreeview
        // 
        _multiSelectTreeview.Dock = System.Windows.Forms.DockStyle.Fill;
        _multiSelectTreeview.ImageIndex = 0;
        _multiSelectTreeview.ImageList = _treeImageList;
        _multiSelectTreeview.Location = new System.Drawing.Point(0, 0);
        _multiSelectTreeview.Margin = new System.Windows.Forms.Padding(4);
        _multiSelectTreeview.Name = "_multiSelectTreeview";
        treeNode1.Name = "Node16";
        treeNode1.Text = "Node16";
        treeNode2.Name = "Node17";
        treeNode2.Text = "Node17";
        treeNode3.Name = "Node4";
        treeNode3.Text = "Node4";
        treeNode4.Name = "Node18";
        treeNode4.Text = "Node18";
        treeNode5.Name = "Node19";
        treeNode5.Text = "Node19";
        treeNode6.Name = "Node5";
        treeNode6.Text = "Node5";
        treeNode7.Name = "Node20";
        treeNode7.Text = "Node20";
        treeNode8.Name = "Node21";
        treeNode8.Text = "Node21";
        treeNode9.Name = "Node6";
        treeNode9.Text = "Node6";
        treeNode10.BackColor = System.Drawing.SystemColors.Highlight;
        treeNode10.ForeColor = System.Drawing.SystemColors.HighlightText;
        treeNode10.Name = "Node0";
        treeNode10.Text = "Node0";
        treeNode11.Name = "Node22";
        treeNode11.Text = "Node22";
        treeNode12.Name = "Node23";
        treeNode12.Text = "Node23";
        treeNode13.Name = "Node7";
        treeNode13.Text = "Node7";
        treeNode14.Name = "Node24";
        treeNode14.Text = "Node24";
        treeNode15.Name = "Node25";
        treeNode15.Text = "Node25";
        treeNode16.Name = "Node8";
        treeNode16.Text = "Node8";
        treeNode17.Name = "Node26";
        treeNode17.Text = "Node26";
        treeNode18.Name = "Node27";
        treeNode18.Text = "Node27";
        treeNode19.Name = "Node9";
        treeNode19.Text = "Node9";
        treeNode20.Name = "Node1";
        treeNode20.Text = "Node1";
        treeNode21.Name = "Node38";
        treeNode21.Text = "Node38";
        treeNode22.Name = "Node39";
        treeNode22.Text = "Node39";
        treeNode23.Name = "Node10";
        treeNode23.Text = "Node10";
        treeNode24.Name = "Node36";
        treeNode24.Text = "Node36";
        treeNode25.Name = "Node37";
        treeNode25.Text = "Node37";
        treeNode26.Name = "Node11";
        treeNode26.Text = "Node11";
        treeNode27.Name = "Node34";
        treeNode27.Text = "Node34";
        treeNode28.Name = "Node35";
        treeNode28.Text = "Node35";
        treeNode29.Name = "Node12";
        treeNode29.Text = "Node12";
        treeNode30.Name = "Node2";
        treeNode30.Text = "Node2";
        treeNode31.Name = "Node32";
        treeNode31.Text = "Node32";
        treeNode32.Name = "Node33";
        treeNode32.Text = "Node33";
        treeNode33.Name = "Node13";
        treeNode33.Text = "Node13";
        treeNode34.Name = "Node30";
        treeNode34.Text = "Node30";
        treeNode35.Name = "Node31";
        treeNode35.Text = "Node31";
        treeNode36.Name = "Node14";
        treeNode36.Text = "Node14";
        treeNode37.Name = "Node28";
        treeNode37.Text = "Node28";
        treeNode38.Name = "Node29";
        treeNode38.Text = "Node29";
        treeNode39.Name = "Node15";
        treeNode39.Text = "Node15";
        treeNode40.Name = "Node3";
        treeNode40.Text = "Node3";
        _multiSelectTreeview.Nodes.AddRange(new System.Windows.Forms.TreeNode[] { treeNode10, treeNode20, treeNode30, treeNode40 });
        _multiSelectTreeview.SelectedImageIndex = 0;
        _multiSelectTreeview.SelectedNodes = (System.Collections.Generic.IReadOnlyCollection<System.Windows.Forms.TreeNode>)resources.GetObject("_multiSelectTreeview.SelectedNodes");
        _multiSelectTreeview.Size = new System.Drawing.Size(306, 240);
        _multiSelectTreeview.TabIndex = 0;
        // 
        // _treeImageList
        // 
        _treeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
        _treeImageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("_treeImageList.ImageStream");
        _treeImageList.TransparentColor = System.Drawing.Color.Transparent;
        _treeImageList.Images.SetKeyName(0, "IL_0_icoBook.ico");
        _treeImageList.Images.SetKeyName(1, "IL_1_selBook.ico");
        _treeImageList.Images.SetKeyName(2, "IL_2_icoLibrary.ico");
        _treeImageList.Images.SetKeyName(3, "IL_3_selLibrary.ico");
        _treeImageList.Images.SetKeyName(4, "IL_4_icoNote.ico");
        _treeImageList.Images.SetKeyName(5, "selLibrary.ico");
        // 
        // _btnSelectNodes
        // 
        _btnSelectNodes.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        _btnSelectNodes.Location = new System.Drawing.Point(197, 442);
        _btnSelectNodes.Margin = new System.Windows.Forms.Padding(4);
        _btnSelectNodes.Name = "_btnSelectNodes";
        _btnSelectNodes.Size = new System.Drawing.Size(122, 26);
        _btnSelectNodes.TabIndex = 4;
        _btnSelectNodes.Text = "Select Nodes";
        _btnSelectNodes.UseVisualStyleBackColor = true;
        _btnSelectNodes.Click += OnButton_SelectNodes;
        // 
        // _btnClearNodes
        // 
        _btnClearNodes.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        _btnClearNodes.Location = new System.Drawing.Point(197, 411);
        _btnClearNodes.Margin = new System.Windows.Forms.Padding(4);
        _btnClearNodes.Name = "_btnClearNodes";
        _btnClearNodes.Size = new System.Drawing.Size(122, 26);
        _btnClearNodes.TabIndex = 3;
        _btnClearNodes.Text = "Clear Nodes";
        _btnClearNodes.UseVisualStyleBackColor = true;
        _btnClearNodes.Click += OnButton_ClearNodes_Click;
        // 
        // _btnExit
        // 
        _btnExit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        _btnExit.Location = new System.Drawing.Point(197, 473);
        _btnExit.Margin = new System.Windows.Forms.Padding(4);
        _btnExit.Name = "_btnExit";
        _btnExit.Size = new System.Drawing.Size(122, 26);
        _btnExit.TabIndex = 5;
        _btnExit.Text = "Exit";
        _btnExit.UseVisualStyleBackColor = true;
        _btnExit.Click += OnButton_Exit_Click;
        // 
        // _checkBoxCustomColors
        // 
        _checkBoxCustomColors.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        _checkBoxCustomColors.AutoSize = true;
        _checkBoxCustomColors.Location = new System.Drawing.Point(18, 449);
        _checkBoxCustomColors.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        _checkBoxCustomColors.Name = "_checkBoxCustomColors";
        _checkBoxCustomColors.Size = new System.Drawing.Size(123, 19);
        _checkBoxCustomColors.TabIndex = 1;
        _checkBoxCustomColors.Text = "Use custom colors";
        _checkBoxCustomColors.UseVisualStyleBackColor = true;
        _checkBoxCustomColors.CheckedChanged += OnCheckBoxBackgroundImage_CheckedChanged;
        // 
        // _checkBoxUseImages
        // 
        _checkBoxUseImages.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        _checkBoxUseImages.AutoSize = true;
        _checkBoxUseImages.Location = new System.Drawing.Point(18, 480);
        _checkBoxUseImages.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        _checkBoxUseImages.Name = "_checkBoxUseImages";
        _checkBoxUseImages.Size = new System.Drawing.Size(86, 19);
        _checkBoxUseImages.TabIndex = 2;
        _checkBoxUseImages.Text = "Use images";
        _checkBoxUseImages.UseVisualStyleBackColor = true;
        _checkBoxUseImages.CheckedChanged += OnCheckBoxUseImages_CheckedChanged;
        // 
        // _splitContainer
        // 
        _splitContainer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        _splitContainer.Location = new System.Drawing.Point(14, 18);
        _splitContainer.Margin = new System.Windows.Forms.Padding(2);
        _splitContainer.MinimumSize = new System.Drawing.Size(128, 128);
        _splitContainer.Name = "_splitContainer";
        _splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
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
        _splitContainer.Size = new System.Drawing.Size(306, 386);
        _splitContainer.SplitterDistance = 240;
        _splitContainer.TabIndex = 0;
        // 
        // _dumpTextBox
        // 
        _dumpTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
        _dumpTextBox.Location = new System.Drawing.Point(0, 0);
        _dumpTextBox.MaxLength = 65535;
        _dumpTextBox.Multiline = true;
        _dumpTextBox.Name = "_dumpTextBox";
        _dumpTextBox.ReadOnly = true;
        _dumpTextBox.Size = new System.Drawing.Size(306, 142);
        _dumpTextBox.TabIndex = 0;
        // 
        // _checkBoxShowLog
        // 
        _checkBoxShowLog.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        _checkBoxShowLog.AutoSize = true;
        _checkBoxShowLog.Location = new System.Drawing.Point(18, 418);
        _checkBoxShowLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        _checkBoxShowLog.Name = "_checkBoxShowLog";
        _checkBoxShowLog.Size = new System.Drawing.Size(127, 19);
        _checkBoxShowLog.TabIndex = 0;
        _checkBoxShowLog.Text = "Show TreeView Log";
        _checkBoxShowLog.UseVisualStyleBackColor = true;
        _checkBoxShowLog.CheckedChanged += OnCheckBoxShowLog_CheckedChanged;
        // 
        // TestForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(336, 511);
        Controls.Add(_checkBoxShowLog);
        Controls.Add(_splitContainer);
        Controls.Add(_checkBoxUseImages);
        Controls.Add(_checkBoxCustomColors);
        Controls.Add(_btnExit);
        Controls.Add(_btnClearNodes);
        Controls.Add(_btnSelectNodes);
        Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
        Margin = new System.Windows.Forms.Padding(4);
        MinimumSize = new System.Drawing.Size(352, 550);
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

    private PK.PkUtils.UI.General.MultiSelectTreeview _multiSelectTreeview;
    private System.Windows.Forms.Button _btnSelectNodes;
    private System.Windows.Forms.Button _btnClearNodes;
    private System.Windows.Forms.Button _btnExit;
    private System.Windows.Forms.CheckBox _checkBoxShowLog;
    private System.Windows.Forms.CheckBox _checkBoxCustomColors;
    private System.Windows.Forms.CheckBox _checkBoxUseImages;
    private System.Windows.Forms.ImageList _treeImageList;
    private System.Windows.Forms.SplitContainer _splitContainer;
    private System.Windows.Forms.TextBox _dumpTextBox;
}


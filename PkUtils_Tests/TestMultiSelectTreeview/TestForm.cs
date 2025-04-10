// Ignore Spelling: Treeview
//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.Consoles;
using PK.PkUtils.Extensions;

namespace TestMultiSelectTreeview;

public partial class TestForm : Form
{
    #region Fields

    private readonly Color _treeviewBackColor;
    private readonly Color _treeviewForeColor;
    private const int IL_0_icoRoot = 0;
    private const int IL_1_selRoot = 1;
    private const int IL_2_icoFolder = 2;
    private const int IL_3_selFolder = 3;
    private const int IL_4_icoLeaf = 4;
    private const int IL_5_selLeaf = 5;
    #endregion // Fields

    #region Constructor(s)

    public TestForm()
    {
        InitializeComponent();

        this.Icon = ConsoleIconManager.CreateIcon(Resources.tree);
        _treeviewBackColor = this._multiSelectTreeview.BackColor;
        _treeviewForeColor = this._multiSelectTreeview.ForeColor;

        _multiSelectTreeview.AfterExpand += (sender, e) => UpdateSingleNodeImages(e.Node);
        _multiSelectTreeview.AfterCollapse += (sender, e) => UpdateSingleNodeImages(e.Node);
    }
    #endregion // Constructor(s)

    #region Methods

    protected static void TurnOffSingleNodeImages(TreeNode node)
    {
        node.ImageIndex = node.SelectedImageIndex = -1;
    }

    protected static void UpdateSingleNodeImages(TreeNode node)
    {
        if (node.Level == 0)
        {
            node.ImageIndex = node.IsExpanded ? IL_1_selRoot : IL_0_icoRoot;
            node.SelectedImageIndex = node.ImageIndex;
        }
        else if (node.Nodes.Count > 0)
        {
            node.ImageIndex = node.IsExpanded ? IL_3_selFolder : IL_2_icoFolder;
            node.SelectedImageIndex = node.ImageIndex;
        }
        else
        {
            node.ImageIndex = node.IsExpanded ? IL_5_selLeaf : IL_4_icoLeaf;
            node.SelectedImageIndex = node.ImageIndex;
        }
    }


    protected void TurnOffImagesOnAllNodes()
    {
        _multiSelectTreeview.ImageList = null;
        foreach (TreeNode node in _multiSelectTreeview.GetAllNodes())
        {
            TurnOffSingleNodeImages(node);
        }
    }

    protected void TurnOnImagesOnAllNodes()
    {
        _multiSelectTreeview.ImageList = _treeImageList;
        foreach (TreeNode node in _multiSelectTreeview.GetAllNodes())
        {
            UpdateSingleNodeImages(node);
        }
    }

    protected void AdjustAllNodesImages()
    {
        if (_checkBoxUseImages.Checked)
            TurnOnImagesOnAllNodes();
        else
            TurnOffImagesOnAllNodes();
    }

    #endregion // Methods

    #region Event_handlers

    private void OnButton_SelectNodes(object sender, EventArgs e)
    {
        List<TreeNode> list = [
            _multiSelectTreeview.Nodes[0],
            _multiSelectTreeview.Nodes[2],
            _multiSelectTreeview.Nodes[3]];

        _multiSelectTreeview.SelectedNodes = list;
    }

    private void OnButton_ClearNodes_Click(object sender, EventArgs e)
    {
        _multiSelectTreeview.SelectedNodes = [];
    }

    private void OnCheckBoxBackgroundImage_CheckedChanged(object sender, EventArgs e)
    {

        if (_checkBoxCustomColors.Checked)
        {
            _multiSelectTreeview.BackColor = Color.Yellow;
            _multiSelectTreeview.ForeColor = Color.DarkGreen;
        }
        else
        {
            _multiSelectTreeview.BackColor = _treeviewBackColor;
            _multiSelectTreeview.ForeColor = _treeviewForeColor;
        }
    }

    private void TestForm_Load(object sender, EventArgs e)
    {
        AdjustAllNodesImages();
    }

    private void OnCheckBoxUseImages_CheckedChanged(object sender, EventArgs e)
    {
        AdjustAllNodesImages();
    }

    private void OnButton_Exit_Click(object sender, EventArgs e)
    {
        this.Close();
    }
    #endregion // Event_handlers
}


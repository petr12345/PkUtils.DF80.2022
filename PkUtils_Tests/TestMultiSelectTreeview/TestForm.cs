// Ignore Spelling: Treeview
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.Consoles;
using PK.PkUtils.Dump;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.UI.General;

namespace TestMultiSelectTreeview;

public partial class TestForm : FormWithLayoutPersistence, IDumper
{
    #region Fields

    private Color _treeviewBackColor;
    private Color _treeviewForeColor;
    private DumperCtrlTextBoxWrapper _wrapper;
    private const bool _shoCallStack = false;
    private const int _maxMsgHistoryItems = 2048;
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

        Icon = ConsoleIconManager.CreateIcon(Resources.tree);
        InitializeTreeview();
        InitializeTreeViewTextLogger(true);
        LoadLayout();
    }
    #endregion // Constructor(s)

    #region Properties

    protected bool ShouldShowLog { get => _checkBoxShowLog.Checked; }

    protected override IDumper Dumper
    {
        get { return (_wrapper ??= new DumperCtrlTextBoxWrapper(_dumpTextBox, _maxMsgHistoryItems)); }
    }
    #endregion // Properties

    #region IDumper Members

    public bool DumpText(string text) => Dumper.DumpText(text);

    public bool DumpError(string text) => Dumper.DumpError(text);

    public bool Reset() => Dumper.Reset();

    #endregion // IDumper Members

    #region Methods

    #region Initialize
    protected void InitializeTreeview()
    {
        _treeviewBackColor = this._multiSelectTreeview.BackColor;
        _treeviewForeColor = this._multiSelectTreeview.ForeColor;

        _multiSelectTreeview.AfterExpand += (sender, e) => UpdateSingleNodeImages(e.Node);
        _multiSelectTreeview.AfterCollapse += (sender, e) => UpdateSingleNodeImages(e.Node);
        _multiSelectTreeview.GetAllNodes().ForEach(node => node.Expand());
    }

    protected void InitializeTreeViewTextLogger(bool showLog)
    {
        _checkBoxShowLog.Checked = showLog;
        _splitContainer.Panel2Collapsed = !showLog;
    }
    #endregion // Initialize

    #region Updating_tree_nodes
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
        static void TurnOffSingleNodeImages(TreeNode node)
        {
            node.ImageIndex = node.SelectedImageIndex = -1;
        }
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
    #endregion // Updating_tree_nodes

    #region Updating_tree_selection_info

    protected void DumpTreeSelectionInfo(
        IReadOnlyCollection<TreeNode> selectedNodes,
        string timingSpec,
        StackTrace stackTrace)
    {
        int count = selectedNodes.Count;
        string timingFinal = timingSpec ?? "Now";
        string info1 = $"{timingFinal} {count} tree node(s) selected";
        string info2 = (count == 0) ? string.Empty : $" ({selectedNodes.Join(", ", x => x.Name)})";
        string info3 = _shoCallStack && (stackTrace != null) ? $" [ {stackTrace.AsNameValue()} ]" : string.Empty;

        Dumper.DumpLine(info1 + info2 + info3);
    }
    #endregion // Updating_tree_selection_info

    #endregion // Methods

    #region Event_handlers

    private void OnButton_SelectNodes(object sender, EventArgs args)
    {
        List<TreeNode> list = [
            _multiSelectTreeview.Nodes[0],
            _multiSelectTreeview.Nodes[2],
            _multiSelectTreeview.Nodes[3]];

        _multiSelectTreeview.SelectedNodes = list;
    }

    private void OnButton_ClearSelection_Click(object sender, EventArgs args)
    {
        _multiSelectTreeview.SelectedNodes = [];
    }

    private void OnButtonClearLog_Click(object sender, EventArgs e)
    {
        Dumper.Reset();
    }

    private void OnCheckBoxBackgroundImage_CheckedChanged(object sender, EventArgs args)
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

    private void OnCheckBoxShowLog_CheckedChanged(object sender, EventArgs args)
    {
        InitializeTreeViewTextLogger(_checkBoxShowLog.Checked);
    }

    private void TestForm_Load(object sender, EventArgs args)
    {
        AdjustAllNodesImages();
        DumpTreeSelectionInfo(_multiSelectTreeview.SelectedNodes, "Initially", null);
    }

    private void OnCheckBoxUseImages_CheckedChanged(object sender, EventArgs args)
    {
        AdjustAllNodesImages();
    }

    private void OnMultiSelectTreeview_SelectionChanged(object sender, MultiSelectTreeview.TreeviewSelChangeArgs args)
    {
        DumpTreeSelectionInfo(args.SelectedNodes, null, args.StackTrace);
    }

    private void OnButton_Exit_Click(object sender, EventArgs args)
    {
        Close();
    }
    #endregion // Event_handlers
}

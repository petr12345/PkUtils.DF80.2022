// Ignore Spelling: TreeView
//
using System.Diagnostics;
using System.Runtime.InteropServices;
using PK.PkUtils.Consoles;
using PK.PkUtils.Dump;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.UI.General;
using PK.PkUtils.UI.Utils;
using PK.PkUtils.Utils;
using PK.PkUtils.WinApi;
using TestMultiSelectOwnerDrawTreeview.Properties;
using static PK.PkUtils.UI.Controls.MultiSelectTreeView;
using static PK.PkUtils.WinApi.User32;

#pragma warning disable IDE0090 // Use 'new(...)'
#pragma warning disable IDE0305 // Collection initialization can be simplified


namespace TestMultiSelectOwnerDrawTreeview;

public partial class TestForm : FormWithLayoutPersistence, IDumper
{
    #region Fields

    private Color _treeviewBackColor;
    private Color _treeviewForeColor;
    private LockRedraw _treeLockRedraw;
    private DumperCtrlTextBoxWrapper _wrapper;
    private Font _italicFont;

    private readonly string[] _specialNodestNames = ["Node0", "Node1", "Node2", "Node3"];
    private const bool _showCallStack = false;
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
        InitializeAdditionalTreeNodes();
        InitializeNodesColors();
        InitializeTreeViewHandlers();
        InitializeFromSettings();
        LoadLayout();
        this.Icon = ConsoleIconManager.CreateIcon(Resources.text_tree);
    }
    #endregion // Constructor(s)

    #region Properties

    protected bool ShouldShowLog { get => this._checkBoxShowLog.Checked; }

    protected override IDumper Dumper
    {
        get { return (_wrapper ??= new DumperCtrlTextBoxWrapper(_dumpTextBox, _maxMsgHistoryItems)); }
    }

    protected Font ItalicFont
    {
        get { return (_italicFont ??= new Font(_treeView.Font, FontStyle.Italic)); }
    }
    #endregion // Properties

    #region IDumper Members

    public bool DumpText(string text) => Dumper.DumpText(text);

    public bool DumpWarning(string text) => Dumper.DumpWarning(text);

    public bool DumpError(string text) => Dumper.DumpError(text);

    public bool Reset() => Dumper.Reset();
    #endregion // IDumper Members

    #region Methods

    #region Initialize

    protected void InitializeTreeViewHandlers()
    {
        _treeviewBackColor = this._treeView.BackColor;
        _treeviewForeColor = this._treeView.ForeColor;

        _treeView.AfterExpand += (sender, e) => UpdateSingleNodeImages(e.Node);
        _treeView.AfterCollapse += (sender, e) => UpdateSingleNodeImages(e.Node);
        _treeView.GetAllNodes().ForEach(node => node.Expand());
    }

    protected void InitializeTreeViewTextLogger(bool showLog)
    {
        _checkBoxShowLog.Checked = showLog;
        _splitContainer.Panel2Collapsed = !showLog;
    }

    protected void InitializeFromSettings()
    {
        _checkBxIndicateProjectedSelection.Checked = Settings.Default.DisplayProjectedSelection;
        _checkBoxShowLog.Checked = Settings.Default.ShowLog;
        _checkBoxUseImages.Checked = Settings.Default.UseImages;
        _checkBoxCustomColors.Checked = Settings.Default.UseCustomColors;

        OnCheckBxIndicateProjectedSelection_CheckedChanged(this, EventArgs.Empty);
        InitializeTreeViewTextLogger(_checkBoxShowLog.Checked);
        AdjustAllNodesImages();
        AdjustCustomColors();
    }

    protected void SaveSettings()
    {
        Settings.Default.DisplayProjectedSelection = _checkBxIndicateProjectedSelection.Checked;
        Settings.Default.ShowLog = _checkBoxShowLog.Checked;
        Settings.Default.UseImages = _checkBoxUseImages.Checked;
        Settings.Default.UseCustomColors = _checkBoxCustomColors.Checked;
        Settings.Default.Save();
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
        _treeView.ImageList = null;
        foreach (TreeNode node in _treeView.GetAllNodes())
        {
            TurnOffSingleNodeImages(node);
        }
    }

    protected void TurnOnImagesOnAllNodes()
    {
        _treeView.ImageList = _treeImageList;
        foreach (TreeNode node in _treeView.GetAllNodes())
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

    protected void AdjustCustomColors()
    {
        if (_checkBoxCustomColors.Checked)
        {
            _treeView.BackColor = Color.Yellow;
            _treeView.ForeColor = Color.DarkGreen;
        }
        else
        {
            _treeView.BackColor = _treeviewBackColor;
            _treeView.ForeColor = _treeviewForeColor;
        }
    }

    private void InitializeAdditionalTreeNodes()
    {
        TreeNode treeNode16 = new TreeNode("Node16");
        TreeNode treeNode17 = new TreeNode("Node17");
        TreeNode treeNode4 = new TreeNode("Node4", [treeNode16, treeNode17]);

        TreeNode treeNode18 = new TreeNode("Node18");
        TreeNode treeNode19 = new TreeNode("Node19");
        TreeNode treeNode5 = new TreeNode("Node5", [treeNode18, treeNode19]);

        TreeNode treeNode20 = new TreeNode("Node20");
        TreeNode treeNode21 = new TreeNode("Node21");
        TreeNode treeNode6 = new TreeNode("Node6", [treeNode20, treeNode21]);

        _treeView.Nodes[0].Nodes.AddRange([treeNode4, treeNode5, treeNode6]);

        TreeNode treeNode22 = new TreeNode("Node22");
        TreeNode treeNode23 = new TreeNode("Node23");
        TreeNode treeNode7 = new TreeNode("Node7", [treeNode22, treeNode23]);

        TreeNode treeNode24 = new TreeNode("Node24");
        TreeNode treeNode25 = new TreeNode("Node25");
        TreeNode treeNode8 = new TreeNode("Node8", [treeNode24, treeNode25]);

        TreeNode treeNode26 = new TreeNode("Node26");
        TreeNode treeNode27 = new TreeNode("Node27");
        TreeNode treeNode9 = new TreeNode("Node9", [treeNode26, treeNode27]);

        TreeNode node1 = new TreeNode("Node1", [treeNode7, treeNode8, treeNode9]);

        TreeNode treeNode38 = new TreeNode("Node38");
        TreeNode treeNode39 = new TreeNode("Node39");
        TreeNode treeNode10 = new TreeNode("Node10", [treeNode38, treeNode39]);

        TreeNode treeNode36 = new TreeNode("Node36");
        TreeNode treeNode37 = new TreeNode("Node37");
        TreeNode treeNode11 = new TreeNode("Node11", [treeNode36, treeNode37]);

        TreeNode treeNode34 = new TreeNode("Node34");
        TreeNode treeNode35 = new TreeNode("Node35");
        TreeNode treeNode12 = new TreeNode("Node12", [treeNode34, treeNode35]);

        TreeNode node2 = new TreeNode("Node2", [treeNode10, treeNode11, treeNode12]);

        TreeNode treeNode32 = new TreeNode("Node32");
        TreeNode treeNode33 = new TreeNode("Node33");
        TreeNode treeNode13 = new TreeNode("Node13", [treeNode32, treeNode33]);

        TreeNode treeNode30 = new TreeNode("Node30");
        TreeNode treeNode31 = new TreeNode("Node31");
        TreeNode treeNode14 = new TreeNode("Node14", [treeNode30, treeNode31]);

        TreeNode treeNode28 = new TreeNode("Node28");
        TreeNode treeNode29 = new TreeNode("Node29");
        TreeNode treeNode15 = new TreeNode("Node15", [treeNode28, treeNode29]);

        TreeNode node3 = new TreeNode("Node3", [treeNode13, treeNode14, treeNode15]);

        _treeView.Nodes.AddRange([node1, node2, node3]);
        _treeView.GetAllNodes().ForEach(node => node.Name = node.Text);
    }

    private void InitializeNodesColors()
    {
        List<TreeNode> redNodes = _treeView.GetAllNodes()
            .Where(node => node.Name.Contains("1"))
            .ToList();
        List<TreeNode> blueNodes = _treeView.GetAllNodes()
            .Where(node => node.Name.Contains("2"))
            .ToList();

        redNodes.ForEach(node => _treeView.SetNodeForeColor(node, Color.Red));
        foreach (TreeNode node in blueNodes)
        {
            _treeView.SetNodeForeColor(node, Color.Blue);
            node.NodeFont = ItalicFont;
        }
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
        string info2 = (count == 0) ? string.Empty : $" ({selectedNodes.Join(", ", x => x.Name.NullIfEmpty() ?? x.Text)})";
        string info3 = _showCallStack && (stackTrace != null) ? $" [ {stackTrace.AsNameValue()} ]" : string.Empty;

        Dumper.DumpLine(info1 + info2 + info3);
    }
    #endregion // Updatin_Buttons

    #region Updating_Buttons

    protected void UpdateInsertDeleteButtons()
    {
        int count = _treeView.Nodes.Count;

        _btnInsertNodes.Enabled = (count <= 1);
        _btnDeleteNodes.Enabled = (count > 1);
    }

    protected void UpdateClearSelectionButton()
    {
        _btnClearSelection.Enabled = (_treeView.SelectedNodes.Count > 0);
    }
    protected void UpdateButtons()
    {
        UpdateInsertDeleteButtons();
        UpdateClearSelectionButton();
    }
    #endregion // Updating_Buttons
    #endregion // Methods

    #region Event_handlers
    #region Form_Events

    private void TestForm_Load(object sender, EventArgs args)
    {
        // This object should only be created after it is guaranteed that the tree control handle has been created
        _treeLockRedraw = new LockRedraw(_treeView, false);
        AdjustAllNodesImages();
        UpdateButtons();
        DumpTreeSelectionInfo(_treeView.SelectedNodes, "Initially", null);
    }

    private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        SaveSettings();
    }
    #endregion // Form_Events

    #region Tree_Events
    private void OnMultiSelectTreeView_SelectionChanged(object sender, TreeViewSelChangeArgs args)
    {
        DumpTreeSelectionInfo(args.SelectedNodes, null, args.StackTrace);
        UpdateClearSelectionButton();
    }
    #endregion // Tree_Events

    #region Context_Menu_Events

    private void OnMenuItemExpandAllNodes_Click(object sender, EventArgs e)
    {
        _treeView.ExpandAllNodes();
    }

    private void OnMenuItemCollapseNodes_Click(object sender, EventArgs e)
    {
        _treeView.CollapseAllNodes();
    }

    private void OnMenuItemOpenTheNode_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Implement 'Open the node'", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void OnMenuItemUnselectAllNodes_Click(object sender, EventArgs e)
    {
        _treeView.SelectedNodes = [];
    }
    #endregion // Context_Menu_Events

    #region Buttons_Events

    private void OnButton_SelectNodes(object sender, EventArgs args)
    {
        List<TreeNode> list = _treeView.Nodes.Cast<TreeNode>()
            .Where(node => _specialNodestNames.Contains(node.Name))
            .ToList();

        _treeView.SelectedNodes = list;
    }

    private void OnButton_ClearSelection_Click(object sender, EventArgs args)
    {
        _treeView.SelectedNodes = [];
    }

    private void OnButtonDeleteNodes_Click(object sender, EventArgs e)
    {
        using (IDisposable _ = new UsageMonitor(_treeLockRedraw))
        {
            TreeNode root = _treeView.RootNode;
            List<TreeNode> notRoot = _treeView.GetAllNodes().Except(root.FromSingle()).ToList();
            notRoot.ForEach(_treeView.RemoveNode);
        }

        _treeView.Refresh();
        DumpTreeSelectionInfo(_treeView.SelectedNodes, null, null);
        UpdateButtons();
    }

    private void OnBtnInsertNodes_Click(object sender, EventArgs e)
    {
        using (IDisposable _ = new UsageMonitor(_treeLockRedraw))
        {
            InitializeAdditionalTreeNodes();
            InitializeNodesColors();
            AdjustAllNodesImages();
            _treeView.Refresh();
        }
        DumpTreeSelectionInfo(_treeView.SelectedNodes, null, null);
        UpdateButtons();
    }

    private void OnButtonClearLog_Click(object sender, EventArgs e)
    {
        Dumper.Reset();
    }

    private void OnCheckBoxBackgroundImage_CheckedChanged(object sender, EventArgs args)
    {
        AdjustCustomColors();
    }

    private void OnCheckBoxShowLog_CheckedChanged(object sender, EventArgs args)
    {
        InitializeTreeViewTextLogger(_checkBoxShowLog.Checked);
    }

    private void OnCheckBoxUseImages_CheckedChanged(object sender, EventArgs args)
    {
        AdjustAllNodesImages();
    }

    private void OnCheckBxIndicateProjectedSelection_CheckedChanged(object sender, EventArgs e)
    {
        _treeView.ProjectedSelectionFrameThickness = _checkBxIndicateProjectedSelection.Checked ? 1 : 0;
    }

    private void OnButton_Exit_Click(object sender, EventArgs args)
    {
        Close();
    }
    #endregion // Buttons_Events
    #endregion // Event_handlers
}

#pragma warning restore IDE0305
#pragma warning restore IDE0090 // Use 'new(...)'
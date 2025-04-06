// Ignore Spelling: Ctrl, Treeview, treeview, Multiselect, unselects
//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace PK.PkUtils.UI.General;

/// <summary> Represents a TreeView control that supports multiple selection of nodes. </summary>
/// <seealso href="https://www.codeproject.com/Articles/20581/Multiselect-Treeview-Implementation/">
/// Multiselect Treeview Implementation.</seealso>
public partial class MultiSelectTreeview : TreeView
{
    #region Fields

    private readonly HashSet<TreeNode> _selectedNodes = [];
    private TreeNode _selectedNode;
    private ColorsCache _colorsCache;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public MultiSelectTreeview()
    {
        base.SelectedNode = null;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>   Gets or sets the collection of selected nodes. </summary>
    public IReadOnlyCollection<TreeNode> SelectedNodes
    {
        get => _selectedNodes;
        set
        {
            ClearSelectedNodes();
            if (value != null)
            {
                foreach (TreeNode node in value)
                {
                    ToggleNode(node, true);
                }
            }
        }
    }

    /// <summary> Gets or sets the selected node. </summary>
    /// <remarks> Note: This property hides the native TreeView's SelectedNode property. </remarks>
    /// <value> The selected node. </value>
    public new TreeNode SelectedNode
    {
        get => _selectedNode;
        set
        {
            if (value == SelectedNode) return;
            ClearSelectedNodes();
            if (value != null)
            {
                SelectNode(value);
            }
        }
    }

    /// <summary>   Gets a value indicating whether this object is using visual styles. </summary>
    /// <remarks>
    /// This property is needed to determine if visual styles are used, in order to assign proper tree nodes colors.
    /// In a WinForms application, the reason your TreeView background is white despite setting it to different
    /// value ( like Control ) in the designer is, that the TreeView control in WinForms does not respect
    /// the BackColor property unless you explicitly disable visual styles.
    /// </remarks>
    protected internal static bool IsUsingVisualStyles
    {
        get => Application.RenderWithVisualStyles;
    }

    /// <summary> Gets the visual styles colors cache (if any has been initialized). </summary>
    protected ColorsCache VisualStylesColorsCache { get => _colorsCache; }
    #endregion // Properties

    #region Methods

    #region Public Methods

    /// <summary> 
    /// Clears the cache retrieved by <see cref="InitializeColorsCache()"/>. 
    /// Should be called if the system color settings or visual styles have changed.
    /// </summary>
    /// <seealso cref="InitializeColorsCache"/>
    public void ResetColorsCache()
    {
        _colorsCache = null;
    }
    #endregion // Public Methods

    #region Protected Methods
    /// <summary>
    /// Initializes and retrieves the visual styles colors cache. If the cache has not been initialized, it is
    /// created and populated with the appropriate colors.
    /// </summary>
    /// <remarks>
    /// This method should be called only if visual styles are enabled. 
    /// It initializes a new instance of <see cref="ColorsCache"/>
    /// and attempts to populate it with colors from existing tree node; 
    /// if that fails, it falls back to default initialization.
    /// </remarks>
    /// <returns>   The initialized <see cref="ColorsCache"/> instance. </returns>
    /// <seealso cref="ResetColorsCache"/>
    protected virtual ColorsCache InitializeColorsCache()
    {
        if (VisualStylesColorsCache is null)
        {
            _colorsCache = new ColorsCache();
            if (!_colorsCache.InitializeFromNodes(this))
            {
                _colorsCache.Initialize();
            }
        }

        return VisualStylesColorsCache;
    }

    /// <summary> Handles exceptions that occur in the control. </summary>
    /// <remarks>
    /// The default implementation simply displays a message box with the exception message.  
    /// Derived classes should override this method to implement more appropriate error handling,  
    /// such as logging the exception or displaying a non-blocking notification instead of using MessageBox.
    /// </remarks>
    /// <param name="ex"> The exception to handle. </param>
    protected virtual void HandleException(Exception ex)
    {
        MessageBox.Show($"{ex.GetType().Name} has occurred: {ex.Message}.");
    }
    #endregion // Protected Methods

    #region Overridden Events

    /// <inheritdoc/>
    protected override void OnGotFocus(EventArgs e)
    {
        // Make sure at least one node has a selection
        // this way we can tab to the ctrl and use the 
        // keyboard to select nodes
        try
        {
            if (SelectedNode == null && this.TopNode != null)
            {
                ToggleNode(this.TopNode, true);
            }

            base.OnGotFocus(e);
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    /// <inheritdoc/>
    protected override void OnMouseDown(MouseEventArgs e)
    {
        try
        {
            base.SelectedNode = null;
            TreeNode node = GetNodeAt(e.Location);
            if (node == null) return;

            int leftBound = node.Bounds.X;
            int rightBound = node.Bounds.Right + 10;
            if (e.Location.X < leftBound || e.Location.X > rightBound) return;

            if (ModifierKeys == Keys.None && SelectedNodes.Contains(node))
            {
                // Potential drag operation, selection on MouseUp
            }
            else
            {
                SelectNode(node);
            }

            base.OnMouseDown(e);
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    /// <inheritdoc/>
    protected override void OnMouseUp(MouseEventArgs e)
    {
        // If the clicked on a node that WAS previously
        // selected then, reselect it now. This will clear
        // any other selected nodes. e.g. A B C D are selected
        // the user clicks on B, now A C & D are no longer selected.
        try
        {
            // Check to see if a node was clicked on 
            TreeNode node = this.GetNodeAt(e.Location);
            if (node != null)
            {
                if (ModifierKeys == Keys.None && SelectedNodes.Contains(node))
                {
                    int leftBound = node.Bounds.X; // -20; // Allow user to click on image
                    int rightBound = node.Bounds.Right + 10; // Give a little extra room
                    if (e.Location.X > leftBound && e.Location.X < rightBound)
                    {
                        SelectNode(node);
                    }
                }
            }

            base.OnMouseUp(e);
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    /// <inheritdoc/>
    protected override void OnItemDrag(ItemDragEventArgs e)
    {
        // If the user drags a node and the node being dragged is NOT selected,
        // then clear the active selection, select the node being dragged and drag it.
        // Otherwise if the node being dragged is selected, drag the entire selection.
        // 
        try
        {
            if (e.Item is TreeNode node)
            {
                if (!SelectedNodes.Contains(node))
                {
                    SelectSingleNode(node);
                    ToggleNode(node, true);
                }
            }

            base.OnItemDrag(e);
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    /// <inheritdoc/>
    protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
    {
        // Never allow base.SelectedNode to be set!
        try
        {
            base.SelectedNode = null;
            e.Cancel = true;

            base.OnBeforeSelect(e);
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    /// <inheritdoc/>
    protected override void OnAfterSelect(TreeViewEventArgs e)
    {
        // Never allow base.SelectedNode to be set!
        try
        {
            base.OnAfterSelect(e);
            base.SelectedNode = null;
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    /// <inheritdoc/>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        // Handle keyboard navigation and selection for the control.
        base.OnKeyDown(e);

        // PetrK 2025/04/02: added missed return for Keys.ControlKey
        if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey) return;

        // Start updating the tree view to optimize UI performance
        BeginUpdate();
        bool isShiftPressed = (ModifierKeys == Keys.Shift);

        try
        {
            // Ensure a node is selected
            if (SelectedNode == null && TopNode != null)
            {
                ToggleNode(TopNode, true);
            }

            if (SelectedNode == null) return;

            switch (e.KeyCode)
            {
                case Keys.Left:
                    HandleLeftArrowKey();
                    break;

                case Keys.Right:
                    HandleRightArrowKey();
                    break;

                case Keys.Up:
                    SelectVisibleNode(SelectedNode.PrevVisibleNode);
                    break;

                case Keys.Down:
                    SelectVisibleNode(SelectedNode.NextVisibleNode);
                    break;

                case Keys.Home:
                    HandleHomeKey(isShiftPressed);
                    break;

                case Keys.End:
                    HandleEndKey(isShiftPressed);
                    break;

                case Keys.PageUp:
                    SelectPageVisibleNode(false);
                    break;

                case Keys.PageDown:
                    SelectPageVisibleNode(true);
                    break;

                default:
                    HandleCharacterSearch(e);
                    break;
            }
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
        finally
        {
            // End updating to refresh UI
            EndUpdate();
        }
    }
    #endregion // Overridden Events

    #region Private_utilities_called_by_OnKeyDown

    private void HandleLeftArrowKey()
    {
        if (SelectedNode.IsExpanded && SelectedNode.Nodes.Count > 0)
        {
            SelectedNode.Collapse();
        }
        else if (SelectedNode.Parent != null)
        {
            SelectSingleNode(SelectedNode.Parent);
        }
    }

    private void HandleRightArrowKey()
    {
        if (!SelectedNode.IsExpanded)
        {
            SelectedNode.Expand();
        }
        else if (SelectedNode.FirstNode != null)
        {
            SelectSingleNode(SelectedNode.FirstNode);
        }
    }

    private void HandleHomeKey(bool isShiftPressed)
    {
        if (Nodes.Count == 0) return;

        if (isShiftPressed && SelectedNode?.Parent != null)
        {
            SelectNode(SelectedNode.Parent.FirstNode);
        }
        else
        {
            SelectSingleNode(Nodes[0]);
        }
    }

    private void HandleEndKey(bool isShiftPressed)
    {
        if (Nodes.Count == 0) return;

        if (isShiftPressed && SelectedNode?.Parent != null)
        {
            SelectNode(SelectedNode.Parent.LastNode);
        }
        else
        {
            TreeNode lastNode = Nodes[0].LastNode;
            while (lastNode.IsExpanded && lastNode.LastNode != null)
            {
                lastNode = lastNode.LastNode;
            }
            SelectSingleNode(lastNode);
        }
    }

    private void SelectPageVisibleNode(bool isPageDown)
    {
        TreeNode node = SelectedNode;

        if (node is null) return;
        for (int count = VisibleCount; count > 0; count--)
        {
            TreeNode nextNode = isPageDown ? node.NextVisibleNode : node.PrevVisibleNode;
            if (nextNode == null)
                break;  // preserve current node value for selection
            node = nextNode;
        }

        SelectSingleNode(node);
    }

    private void HandleCharacterSearch(KeyEventArgs e)
    {
        if (char.IsLetterOrDigit((char)e.KeyValue))
        {
            string searchChar = char.ToUpperInvariant((char)e.KeyValue).ToString();
            TreeNode nextNode, node = SelectedNode;

            while ((nextNode = node.NextVisibleNode) != null)
            {
                node = nextNode;
                if (node.Text.StartsWith(searchChar, StringComparison.InvariantCultureIgnoreCase))
                {
                    SelectSingleNode(node);
                    break;
                }
            }
        }
    }

    private void SelectVisibleNode(TreeNode node)
    {
        if (node != null)
        {
            SelectNode(node);
        }
    }
    #endregion // Private_utilities_called_by_OnKeyDown

    #region Other_Private_Helper_Methods

    private void SelectNode(TreeNode node)
    {
        try
        {
            this.BeginUpdate();

            if (SelectedNode == null || ModifierKeys == Keys.Control)
            {
                // Ctrl + Click selects an unselected node, or unselects a selected node.
                bool bIsSelected = SelectedNodes.Contains(node);
                ToggleNode(node, !bIsSelected);
            }
            else if (ModifierKeys == Keys.Shift)
            {
                // Shift+Click selects nodes between the selected node and here.
                TreeNode ndStart = SelectedNode;
                TreeNode ndEnd = node;

                if (ndStart.Parent == ndEnd.Parent)
                {
                    // Selected node and clicked node have same parent, easy case.
                    if (ndStart.Index < ndEnd.Index)
                    {
                        // If the selected node is beneath the clicked node, walk down
                        // selecting each Visible node until we reach the end.
                        while (ndStart != ndEnd)
                        {
                            ndStart = ndStart.NextVisibleNode;
                            if (ndStart == null) break;
                            ToggleNode(ndStart, true);
                        }
                    }
                    else if (ndStart.Index == ndEnd.Index)
                    {
                        // Clicked same node, do nothing
                    }
                    else
                    {
                        // If the selected node is above the clicked node, walk up
                        // selecting each Visible node until we reach the end.
                        while (ndStart != ndEnd)
                        {
                            ndStart = ndStart.PrevVisibleNode;
                            if (ndStart == null) break;
                            ToggleNode(ndStart, true);
                        }
                    }
                }
                else
                {
                    // Selected node and clicked node have same parent, hard case.
                    // We need to find a common parent to determine if we need
                    // to walk down selecting, or walk up selecting.

                    TreeNode ndStartP = ndStart;
                    TreeNode ndEndP = ndEnd;
                    int startDepth = Math.Min(ndStartP.Level, ndEndP.Level);

                    // Bring lower node up to common depth
                    while (ndStartP.Level > startDepth)
                    {
                        ndStartP = ndStartP.Parent;
                    }

                    // Bring lower node up to common depth
                    while (ndEndP.Level > startDepth)
                    {
                        ndEndP = ndEndP.Parent;
                    }

                    // Walk up the tree until we find the common parent
                    while (ndStartP.Parent != ndEndP.Parent)
                    {
                        ndStartP = ndStartP.Parent;
                        ndEndP = ndEndP.Parent;
                    }

                    // Select the node
                    if (ndStartP.Index < ndEndP.Index)
                    {
                        // If the selected node is beneath the clicked node walk down
                        // selecting each Visible node until we reach the end.
                        while (ndStart != ndEnd)
                        {
                            ndStart = ndStart.NextVisibleNode;
                            if (ndStart == null) break;
                            ToggleNode(ndStart, true);
                        }
                    }
                    else if (ndStartP.Index == ndEndP.Index)
                    {
                        if (ndStart.Level < ndEnd.Level)
                        {
                            while (ndStart != ndEnd)
                            {
                                ndStart = ndStart.NextVisibleNode;
                                if (ndStart == null) break;
                                ToggleNode(ndStart, true);
                            }
                        }
                        else
                        {
                            while (ndStart != ndEnd)
                            {
                                ndStart = ndStart.PrevVisibleNode;
                                if (ndStart == null) break;
                                ToggleNode(ndStart, true);
                            }
                        }
                    }
                    else
                    {
                        // If the selected node is above the clicked node walk up
                        // selecting each Visible node until we reach the end.
                        while (ndStart != ndEnd)
                        {
                            ndStart = ndStart.PrevVisibleNode;
                            if (ndStart == null) break;
                            ToggleNode(ndStart, true);
                        }
                    }
                }
            }
            else
            {
                // Just clicked a node, select it
                SelectSingleNode(node);
            }

            OnAfterSelect(new TreeViewEventArgs(SelectedNode));
        }
        finally
        {
            this.EndUpdate();
        }
    }

    private void ClearSelectedNodes()
    {
        try
        {
            DetermineUnselectedNodeColor(out Color bgColor, out Color fgColor);
            foreach (TreeNode node in SelectedNodes)
            {
                node.BackColor = bgColor;
                node.ForeColor = fgColor;
            }
        }
        finally
        {
            _selectedNodes.Clear();
            _selectedNode = null;
        }
    }

    private void SelectSingleNode(TreeNode node)
    {
        if (node == null)
        {
            return;
        }

        ClearSelectedNodes();
        ToggleNode(node, true);
        node.EnsureVisible();
    }

    private void ToggleNode(TreeNode node, bool bSelectNode)
    {
        Color bgColor, fgColor;

        if (bSelectNode)
        {
            _selectedNode = node;
            _selectedNodes.Add(node);

            bgColor = SystemColors.Highlight;
            fgColor = SystemColors.HighlightText;
        }
        else
        {
            _selectedNodes.Remove(node);

            DetermineUnselectedNodeColor(out bgColor, out fgColor);
        }
        node.BackColor = bgColor;
        node.ForeColor = fgColor;
    }

    private void DetermineUnselectedNodeColor(out Color bgColor, out Color fgColor)
    {
        if (IsUsingVisualStyles)
        {
            ColorsCache cache = InitializeColorsCache();
            bgColor = cache.BgColor;
            fgColor = cache.FgColor;
        }
        else
        {
            // Visual styles disabled - inherit from TreeView
            bgColor = this.BackColor;
            fgColor = this.ForeColor;
        }
    }
    #endregion // Other_Private_Helper_Methods
    #endregion // Methods
}

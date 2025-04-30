// Ignore Spelling: Ctrl, TreeView, treeview, Multiselect, Sel, unselects
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;
using PK.PkUtils.WinApi;


namespace PK.PkUtils.UI.General;


/// <summary> Represents a TreeView control that supports multiple selection of nodes. </summary>
/// <seealso href="https://www.codeproject.com/Articles/20581/Multiselect-TreeView-Implementation/">
/// Multiselect TreeView Implementation.</seealso>
public partial class MultiSelectTreeView : TreeView
{
    #region Typedefs

    /// <summary>
    /// Additional information for TreeView selection change events.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TreeViewSelChangeArgs"/> struct with custom selection.
    /// </remarks>
    /// <param name="treeview">The TreeView control. Must not be null.</param>
    /// <param name="selectedNodes">The selected nodes. Must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public readonly struct TreeViewSelChangeArgs(MultiSelectTreeView treeview, IReadOnlyCollection<TreeNode> selectedNodes)
    {
        /// <summary> The TreeView instance. </summary>
        public MultiSelectTreeView TreeView { get; } = treeview ?? throw new ArgumentNullException(nameof(treeview));

        /// <summary> The collection of selected nodes. </summary>
        public IReadOnlyCollection<TreeNode> SelectedNodes { get; } = selectedNodes ?? throw new ArgumentNullException(nameof(selectedNodes));

        /// <summary> The stack trace when this object was created. </summary>
        public StackTrace StackTrace { get; } = new StackTrace(skipFrames: 2);

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewSelChangeArgs"/> struct.
        /// </summary>
        /// <param name="treeview">The TreeView control. Must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="treeview"/> is null.</exception>
        public TreeViewSelChangeArgs(MultiSelectTreeView treeview)
            : this(treeview, treeview?.SelectedNodes ?? throw new ArgumentNullException(nameof(treeview))) { }
    }
    #endregion // Typedefs

    #region Fields

    private ColorsCache _colorsCache;
    private int _selectionChangeDepth;
    private bool _selectionChangedPending;
    private TreeNode _selectedNode;
    private bool _selectionInitialized;
    private readonly HashSet<TreeNode> _selectedNodes = [];
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public MultiSelectTreeView()
    {
        base.SelectedNode = null;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Gets the root node, if there is any. </summary>
    public TreeNode RootNode => Nodes.Count > 0 ? Nodes[0] : null;

    /// <summary> Gets or sets the collection of selected nodes. </summary>
    [Browsable(false)]
    public IReadOnlyCollection<TreeNode> SelectedNodes
    {
        get => _selectedNodes;
        set
        {
            if (ReferenceEquals(value, _selectedNodes)) return;
            if ((value is not null) && _selectedNodes.SetEquals(value)) return;

            using IDisposable deferred = DeferSelectionChange();
            ClearSelectedNodes();
            if (value != null)
            {
                foreach (TreeNode node in value)
                {
                    ToggleNode(node, true);  // MarkSelectionChanged() is called from inside of ToggleNode
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

            using IDisposable deferred = DeferSelectionChange();
            ClearSelectedNodes();  // MarkSelectionChanged() is called from inside of ToggleNode
            if (value != null)
            {
                SelectNode(value);
            }
        }
    }

    /// <summary>  Event queue for all listeners interested in SelectionChanged events. </summary>
    public event EventHandler<TreeViewSelChangeArgs> SelectionChanged;

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

    /// <summary> Gets a value indicating whether a selection changed is pending. </summary>
    protected bool IsSelectionChangedPending { get => _selectionChangedPending; }

    /// <summary>   Gets a value indicating whether the selection initialized. </summary>
    protected bool SelectionInitialized { get => _selectionInitialized; }
    #endregion // Properties

    #region Methods

    #region Public Methods

    /// <summary> Query if <paramref name="node"/> is one of selected nodes. </summary>
    /// <param name="node"> The <see cref="TreeNode"/> to be examined. It can be null. </param>
    /// <returns> True if <paramref name="node"/> is part of selection, false if not. </returns>
    public bool IsSelected(TreeNode node)
    {
        return (node is not null) && SelectedNodes.Contains(node);
    }

    /// <summary>
    /// Removes the specified <see cref="TreeNode"/> from the <see cref="TreeView"/>,
    /// and ensures that the internal selection set remains consistent by removing the node
    /// and all of its descendant nodes from <see cref="_selectedNodes"/> collection.
    /// </summary>
    /// <param name="node">The <see cref="TreeNode"/> to remove from the control.</param>
    /// <remarks> The standard <see cref="TreeNode.Remove"/> method does not raise any event or callback
    /// that would allow the control to react to the removal of a node. If a selected node is removed directly,
    /// it would remain in the internal <see cref="_selectedNodes"/> set, leading to inconsistencies
    /// such as invalid selection states, rendering errors, or exceptions.
    ///
    /// To avoid this, the <c>RemoveNode</c> method provides a safe and consistent way to remove a node.
    /// Always use this method instead of calling TreeNode.Remove() directly, to preserve selection integrity.
    /// </remarks>
    public void RemoveNode(TreeNode node)
    {
        if (node is not null)
        {
            using IDisposable deferred = DeferSelectionChange();

            RemoveDescendantsFromSelection(node, includeNode: true);
            node.Remove();
            Invalidate();
        }
    }

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
    #region Colors_cache_related

    /// <summary>
    /// Initializes and retrieves the visual styles colors cache. If the cache has not been initialized, it is
    /// created and populated with the appropriate colors.
    /// </summary>
    /// <remarks>
    /// This method should be called only if visual styles are enabled. It initializes a new instance of <see cref="ColorsCache"/>
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
    #endregion // Colors_cache_related

    #region Selection_change_related

    /// <summary> Begins selection change, incrementing internal depth counter. </summary>
    protected void BeginSelectionChange()
    {
        _selectionChangeDepth++;
    }

    /// <summary>
    /// Ends selection change, decrementing selection change depth. If that depth becomes zero, and selection
    /// change is pending, calls <see cref="OnSelectionChanged"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
    protected void EndSelectionChange()
    {
        if (_selectionChangeDepth <= 0)
        {
            throw new InvalidOperationException($"Unbalanced {nameof(EndSelectionChange)} call, {_selectionChangeDepth.AsNameValue()}.");
        }
        if ((--_selectionChangeDepth == 0) && IsSelectionChangedPending)
        {
            _selectionChangedPending = false;
            OnSelectionChanged();
        }
    }

    /// <summary> Begins selection change, and defers <see cref="EndSelectionChange"/>. </summary>
    /// <returns> An IDisposable object that will call <see cref="EndSelectionChange"/>. </returns>
    protected IDisposable DeferSelectionChange()
    {
        BeginSelectionChange();
        return new DisposableAction(EndSelectionChange);
    }

    /// <summary>
    /// To be called when individual selection part is being changed.
    /// If selection changed depth is nonzero, marks the change as pending;
    /// otherwise calls <see cref="OnSelectionChanged"/> directly.
    /// </summary>
    protected void MarkSelectionChanged()
    {
        if (_selectionChangeDepth > 0)
        {
            _selectionChangedPending = true;
        }
        else
        {
            OnSelectionChanged();
        }
    }

    /// <summary> Invokes the 'selection changed' event <see cref="SelectionChanged"/>. </summary>
    protected virtual void OnSelectionChanged()
    {
        if (_selectionChangeDepth > 0)
        {
            throw new InvalidOperationException($"Event should not be raised with this value being positive, {_selectionChangeDepth.AsNameValue()}.");
        }
        SelectionChanged?.Invoke(this, new TreeViewSelChangeArgs(this));
    }

    /// <summary> Initialize the selection if not initialized already. </summary>
    /// <remarks>
    /// This is an attempt to address the odd behavior of the tree view, namely that it selects some of the nodes 
    /// you added and draws them that way, despite the SelectedNode property still returns null.
    /// So, this method is the attempt to unify the initial displayed selection
    /// and SelectedNodes property. See also
    /// https://stackoverflow.com/questions/11310912/how-to-disable-treeview-auto-first-node-select.
    /// </remarks>
    /// <param name="selectedNodes"> (Optional) The collection of selected nodes. </param>
    /// <seealso cref="SelectedNodes"/>
    protected virtual void InitSelection(IEnumerable<TreeNode> selectedNodes = null)
    {
        if (!SelectionInitialized)
        {
            _selectedNodes.Clear();
            _selectedNodes.UnionWith(selectedNodes ?? []);
            _selectedNode = selectedNodes?.FirstOrDefault();

            // Force a change in the behavior of the standard tree control, which, when there is no selection, 
            // still wants to draw the root as selected
            if ((SelectedNode is null) && (RootNode is not null))
            {
                EnforceNodeColor(RootNode, selectNode: false);
            }

            _selectionInitialized = true;
            OnSelectionChanged(); // Notify things has changed
        }
    }

    /// <summary>
    /// Removes all selected nodes that are descendants (children, grandchildren, etc.)
    /// of the specified <paramref name="node"/> from the selection set.
    /// </summary>
    /// <param name="node">The node whose descendants should be removed from selection. Can be null. </param>
    /// <param name="includeNode">
    /// If <c>true</c>, the <paramref name="node"/> itself will also be removed from selection if it is present.
    /// </param>
    /// <remarks>
    /// This method is useful when a node is being removed or collapsed, and its children
    /// (and optionally the node itself) should no longer be considered part of the current selection.
    /// It ensures the internal selection set remains valid and free of dangling references to disposed nodes.
    /// </remarks>
    /// <returns>The number of nodes removed from selection.</returns>
    protected virtual int RemoveDescendantsFromSelection(TreeNode node, bool includeNode)
    {
        using IDisposable deferred = DeferSelectionChange();
        int removedCount = 0;

        if (node is not null && SelectedNodes.Count > 0)
        {
            // Use a stack for depth-first traversal to avoid recursion; start either from the root or child nodes
            for (var stack = new Stack<TreeNode>(includeNode ? [node] : node.Nodes.Cast<TreeNode>()); stack.Count > 0;)
            {
                TreeNode current = stack.Pop();

                if (_selectedNodes.Remove(current))
                {
                    removedCount++;
                    MarkSelectionChanged();
                }

                stack.PushRange(current.Nodes.Cast<TreeNode>());
            }
        }

        return removedCount;
    }
    #endregion // Selection_change_related

    #region Error_handling_related

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
    #endregion // Error_handling_related
    #endregion // Protected Methods

    #region Protected Overridden Events

    /// <summary>
    /// Overrides <see cref="M:System.Windows.Forms.Control.WndProc(System.Windows.Forms.Message@)" />.
    /// </summary>
    /// <param name="m"> [in,out] The Windows <see cref="T:System.Windows.Forms.Message" /> to process. </param>
    protected override void WndProc(ref Message m)
    {
        switch (m.Msg)
        {
            case (int)Win32.WM.WM_PAINT:
                if (!SelectionInitialized)
                {
                    InitSelection();
                }
                break;

            // System colors or visual styles have changed. Reset any cached colors, and request a repaint
            case (int)Win32.WM.WM_SYSCOLORCHANGE:
            case (int)Win32.WM.WM_THEMECHANGED:
                ResetColorsCache();
                Invalidate();
                break;
        }

        base.WndProc(ref m);
    }

    /// <inheritdoc/>
    protected override void OnGotFocus(EventArgs e)
    {
        InitSelection();
        base.OnGotFocus(e);
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

            using IDisposable deferred = DeferSelectionChange();
            if (ModifierKeys == Keys.None && IsSelected(node))
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
            BeginSelectionChange();

            // Check to see if a node was clicked on 
            TreeNode node = this.GetNodeAt(e.Location);
            if (node != null)
            {
                if (ModifierKeys == Keys.None && IsSelected(node))
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
        finally
        {
            EndSelectionChange();
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
            BeginSelectionChange();

            if (e.Item is TreeNode node)
            {
                if (!IsSelected(node))
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
        finally
        {
            EndSelectionChange();
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

        // Begin potential selection change. 
        // Related MarkSelectionChanged are called from individual handlers.
        using IDisposable deferred = DeferSelectionChange();

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
    #endregion // Protected Overridden Events

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
            TreeNode nextLastNode, lastNode = Nodes[0].LastNode;
            while (lastNode.IsExpanded && (nextLastNode = lastNode.LastNode) != null)
            {
                lastNode = nextLastNode;
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
            BeginSelectionChange();
            BeginUpdate();

            if (SelectedNode == null || ModifierKeys == Keys.Control)
            {
                // Ctrl + Click selects an unselected node, or unselects a selected node.
                bool bIsSelected = IsSelected(node);
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
            EndUpdate();
            EndSelectionChange();
        }
    }

    private void ClearSelectedNodes()
    {
        // early return to avoid unnecessary call of MarkSelectionChanged
        if ((_selectedNodes.Count == 0) && (_selectedNode is null)) return;

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
            MarkSelectionChanged();
        }
    }

    private void SelectSingleNode(TreeNode node)
    {
        if (node == null)
            return;

        if ((SelectedNodes.Count == 1) && (node == SelectedNodes.First()) && (node == SelectedNode))
        {
            // Nothing more is needed, so avoid doing any calls that would trigger MarkSelectionChanged()
            node.EnsureVisible();
        }
        else
        {
            using IDisposable deferred = DeferSelectionChange();
            ClearSelectedNodes();
            ToggleNode(node, true);
            node.EnsureVisible();
        }
    }

    private bool ToggleNode(TreeNode node, bool selectNode)
    {
        // Should not be called for any null node; only to be on the safe side
        if (node is null) return false;

        bool result;

        if (selectNode)
        {
            if (result = ((SelectedNode != node) || !IsSelected(node)))
            {
                _selectedNode = node;
                _selectedNodes.Add(node);
            }
        }
        else if (result = _selectedNodes.Remove(node))
        {
            if (ReferenceEquals(SelectedNode, node))
            {
                _selectedNode = null;
            }
        }

        if (result)
        {
            EnforceNodeColor(node, selectNode);
            MarkSelectionChanged();
        }

        return result;
    }

    /// <summary>  Enforce node color to be selected or unselected; depending <paramref name="selectNode"/>. </summary>
    /// <param name="node"> The <see cref="TreeNode"/> to be modified. Can't be null. </param>
    /// <param name="selectNode"> True to use select, false unselected node color. </param>
    private void EnforceNodeColor(TreeNode node, bool selectNode)
    {
        if (selectNode)
        {
            node.BackColor = SystemColors.Highlight;
            node.ForeColor = SystemColors.HighlightText;
        }
        else
        {
            DetermineUnselectedNodeColor(out Color bgColor, out Color fgColor);
            node.BackColor = bgColor;
            node.ForeColor = fgColor;
        }
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.UI.Controls;

/// <summary>
/// A specialized TreeView control that supports owner-drawn rendering and multiple node selection.
/// In addition to standard selection, this control visually distinguishes nodes that are not directly selected
/// but have a selected ancestor (i.e., their parent or higher ancestor is selected). Such nodes are treated as
/// "projected selections" and are rendered with a distinct indicator (see <see cref="OnDrawNode(DrawTreeNodeEventArgs)"/>).
/// </summary>
public class MultiSelectOwnerDrawTreeView : MultiSelectTreeView
{
    #region Fields

    private readonly SizeLimitedCache<Color, SolidBrush> _cacheBrushes;

    private int _nodeFrameThickness = _defaultNodeFrameThickness;
    private const int _defaultNodeFrameThickness = 0;

    // <summary> Horizontal padding added when invalidating a node's bounds. </summary>
    private const int _nodeInvalidatePaddingX = 5;

    /// <summary> Vertical padding added when invalidating a node's bounds. </summary>
    private const int _nodeInvalidatePaddingY = 2;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public MultiSelectOwnerDrawTreeView()
    {
        DrawMode = TreeViewDrawMode.OwnerDrawText;
        this.DoubleBuffered = true; // Enable double buffering for smoother rendering
        _cacheBrushes = new SizeLimitedCache<Color, SolidBrush>(maxSize: 8);
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Gets or sets the thickness (in pixels) of the rectangle drawn around nodes that have a selected ancestor.
    /// Set to 0 to disable the projected selection frame. Value is constrained to be non-negative.
    /// </summary>
    [Browsable(true)]
    [Description("Specifies the thickness (in pixels) of the rectangle drawn around nodes that have a selected ancestor. Set to 0 to disable the frame.")]
    [DefaultValue(_defaultNodeFrameThickness)]
    public int ProjectedSelectionFrameThickness
    {
        get => _nodeFrameThickness;
        set
        {
            int newValue = Math.Max(0, value);
            if (ProjectedSelectionFrameThickness != newValue)
            {
                _nodeFrameThickness = newValue;
                if (SelectedNodes.Count > 0) Invalidate();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether projected selections should be painted,
    /// based on whether the projected selection frame thickness is greater than zero.
    /// </summary>
    protected bool PaintProjectedSelections => ProjectedSelectionFrameThickness > 0;
    #endregion // Properties

    #region Methods
    #region Protected Methods

    /// <summary> Determines whether any ancestor of the specified node is selected. </summary>
    /// <param name="node">The <see cref="TreeNode"/> to examine.</param>
    /// <returns>true if any ancestor is selected; otherwise, false.</returns>
    protected bool IsAncestorSelected(TreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        for (TreeNode parent = node.Parent; (parent != null); parent = parent.Parent)
        {
            if (IsSelected(parent))
                return true;
        }
        return false;
    }

    // For now, this method is not used, but it could be useful in the future.
    ///// <summary> Gets the bounds of the text area of the specified node. </summary>
    ///// <param name="node">The <see cref="TreeNode"/> whose text bounds are retrieved.</param>
    ///// <returns>A <see cref="Rectangle"/> representing the text area of the node.</returns>
    //protected Rectangle GetNodeTextBounds(TreeNode node)
    //{
    //    ArgumentNullException.ThrowIfNull(node);

    //    Size textSize = TextRenderer.MeasureText(node.Text, node.NodeFont ?? this.Font);
    //    Point location = new(node.Bounds.X, node.Bounds.Y);
    //    return new Rectangle(location, new Size(textSize.Width, node.Bounds.Height));
    //}

    /// <summary> Invalidates the drawing area of the specified node. </summary>
    /// <param name="node">The <see cref="TreeNode"/> to invalidate.</param>
    protected void InvalidateNode(TreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        Rectangle bounds = node.Bounds;
        bounds.Inflate(_nodeInvalidatePaddingX, _nodeInvalidatePaddingY); // Could be replaced with dynamic sizing if needed

        this.Invalidate(bounds);
    }

    /// <summary> Invalidates the specified node and all its descendant nodes. </summary>
    /// <param name="node">The root <see cref="TreeNode"/> to invalidate.</param>
    protected void InvalidateNodeAndDescendants(TreeNode node)
    {
        if (node is null || !node.IsVisible)
            return;

        // Instead of invalidating each node separately (which can trigger many paint requests),
        // we collect a single bounding rectangle that covers the node and all visible descendants.
        Rectangle union = Rectangle.Empty;

        // Use an explicit stack instead of recursion to avoid deep call chains for large trees.
        for (var stack = new Stack<TreeNode>([node]); stack.Count > 0;)
        {
            TreeNode current = stack.Pop();
            if (!current.IsVisible)
                continue;

            union = union.IsEmpty ? current.Bounds : Rectangle.Union(union, current.Bounds);

            // Only descend into children if the node is expanded.
            // This way we skip collapsed branches, which don’t need repainting.
            if (current.IsExpanded)
            {
                stack.PushRange(current.GetChildren());
            }
        }

        // Invalidate once using the combined rectangle (with same padding as before).
        // This reduces the number of invalidation calls and therefore the number of paint operations.
        if (!union.IsEmpty)
        {
            union.Inflate(_nodeInvalidatePaddingX, _nodeInvalidatePaddingY);
            Invalidate(union);
        }
    }
    #endregion // Protected Methods

    #region Protected_Overrides

    /// <summary>
    /// Overwrites implementation of the base class <see cref="MultiSelectTreeView.ToggleNode(TreeNode, bool)"/>
    /// method to ensure proper rendering.
    /// </summary>
    /// <param name="node"> The node to select or unselect. If null, the method returns false and does nothing. </param>
    /// <param name="selectNode"> If true, the node is added to the selection set. If false, the node is removed
    /// from the selection set. </param>
    /// <returns>   true if the selection state of the node was changed; otherwise, false&lt;. </returns>
    protected override bool ToggleNode(TreeNode node, bool selectNode)
    {
        bool result;
        if (result = (node is not null) && base.ToggleNode(node, selectNode))
        {
            if (PaintProjectedSelections)
                InvalidateNodeAndDescendants(node);
            else
                InvalidateNode(node);
        }

        return result;
    }

    /// <summary>
    /// Customizes the drawing of tree nodes to provide enhanced visual feedback for node states.
    /// <para>
    /// - Directly selected nodes are rendered with a highlighted background and foreground color.
    /// - Nodes that are not directly selected, but have a selected ancestor (i.e., their parent or higher ancestor is selected),
    ///   are considered "projected selections" and are visually indicated by drawing a colored rectangle around their text area.
    /// </para>
    /// </summary>
    /// <param name="args"> A <see cref="DrawTreeNodeEventArgs"/> containing the event data for the node to be drawn. </param>
    protected override void OnDrawNode(DrawTreeNodeEventArgs args)
    {
        // For some reason, this method is called for invisible nodes as well,
        // so we need to check if the node is visible not to waste time on rendering.
        // Checking the bound is the most effective way to do this,
        // as they are prepared already in the input args.
        Rectangle textBounds = args.Bounds;

        if ((textBounds.Width > 0) && (textBounds.Height > 0))
        {
            TreeNode currentNode = args.Node;
            Font nodeFont = currentNode.NodeFont;
            bool isNodeSelected = IsSelected(currentNode);
            Color highlightBackgroundColor = SystemColors.Highlight;
            Color highlightForegroundColor = SystemColors.HighlightText;
            Color foregroundColor = highlightForegroundColor;

            // If the node has a custom font, adjust the bounds to fit the rendered text.
            if (nodeFont is not null)
            {
                textBounds.Size = TextRenderer.MeasureText(currentNode.Text, nodeFont);
            }

            if (isNodeSelected)
            {
                // Fill the background for selected nodes using the system highlight color.
                args.Graphics.FillRectangle(SystemBrushes.Highlight, textBounds);
            }
            else
            {
                // Determine the background and foreground colors for unselected nodes.
                DetermineUnselectedNodeColor(currentNode, out Color backgroundColor, out foregroundColor);

                // Retrieve or create a cached SolidBrush for the background color.
                if (_cacheBrushes.TryGetValue(backgroundColor, out SolidBrush backgroundBrush) is false)
                    _cacheBrushes[backgroundColor] = backgroundBrush = new SolidBrush(backgroundColor);
                args.Graphics.FillRectangle(backgroundBrush, textBounds);
            }

            // Draw the node's text using the appropriate font and foreground color.
            TextRenderer.DrawText(
                args.Graphics,
                currentNode.Text,
                nodeFont ?? this.Font,
                textBounds,
                foregroundColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            // Draw a projected selection frame if enabled, the node is not selected, and it has a selected ancestor.
            if (PaintProjectedSelections && !isNodeSelected && IsAncestorSelected(currentNode))
            {
                int nodeFrameThickness = ProjectedSelectionFrameThickness;
                Rectangle textRectangle = textBounds; // Optionally, use GetNodeTextBounds(currentNode) for more precise bounds.

                // Shrink the rectangle by the frame thickness to ensure the frame is drawn inside the text area.
                textRectangle.Inflate(-nodeFrameThickness, -nodeFrameThickness);

                // Note: Pens are lightweight in GDI+ and do not require caching like SolidBrushes.
                // Creating a new Pen for each draw is acceptable and avoids cache invalidation issues 
                // in case of ProjectedSelectionFrameThickness value changes.
                // 
                using Pen framePen = new(highlightBackgroundColor, nodeFrameThickness);
                args.Graphics.DrawRectangle(framePen, textRectangle);
            }
        }

        // Call the base implementation to ensure any additional drawing logic is executed.
        base.OnDrawNode(args);
    }

    /// <summary> Releases the unmanaged resources and optionally releases the managed resources. </summary>
    /// <param name="disposing"> true to release both managed and unmanaged resources; false to release only
    /// unmanaged resources. </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cacheBrushes.Dispose();
        }
        base.Dispose(disposing);
    }
    #endregion // Protected_Overrides
    #endregion // Methods
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.UI.Controls;

/// <summary>
/// A specialized TreeView control that supports owner-drawn rendering and multiple node selection.
/// In addition to standard selection, this control visually distinguishes nodes that are not directly selected
/// but have a selected ancestor (i.e., their parent or higher ancestor is selected). Such nodes are treated as
/// "projected selections" and are rendered with a distinct indicator (see <see cref="OnDrawNode(DrawTreeNodeEventArgs)"/>).
/// </summary>
public partial class MultiSelectOwnerDrawTreeView : MultiSelectTreeView
{
    #region Fields

    private const int _defaultNodeFrameThickness = 0;
    private int _nodeFrameThickness = _defaultNodeFrameThickness;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public MultiSelectOwnerDrawTreeView()
    {
        DrawMode = TreeViewDrawMode.OwnerDrawText;
        this.DoubleBuffered = true; // Enable double buffering for smoother rendering
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

    /// <summary> Gets the bounds of the text area of the specified node. </summary>
    /// <param name="node">The <see cref="TreeNode"/> whose text bounds are retrieved.</param>
    /// <returns>A <see cref="Rectangle"/> representing the text area of the node.</returns>
    protected Rectangle GetNodeTextBounds(TreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        Size textSize = TextRenderer.MeasureText(node.Text, node.NodeFont ?? this.Font);
        Point location = new(node.Bounds.X, node.Bounds.Y);
        return new Rectangle(location, new Size(textSize.Width, node.Bounds.Height));
    }

    /// <summary> Invalidates the drawing area of the specified node. </summary>
    /// <param name="node">The <see cref="TreeNode"/> to invalidate.</param>
    protected void InvalidateNode(TreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        Rectangle bounds = node.Bounds;
        bounds.Inflate(5, 2); // Could be replaced with dynamic sizing if needed

        this.Invalidate(bounds);
    }

    /// <summary> Invalidates the specified node and all its descendant nodes. </summary>
    /// <param name="node">The root <see cref="TreeNode"/> to invalidate.</param>
    protected void InvalidateNodeAndDescendants(TreeNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        node.EnumerateSelfAndDescendants().ForEach(InvalidateNode);
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
            InvalidateNodeAndDescendants(node);
        }

        return result;
    }

    /// <summary>
    /// Overrides the default drawing behavior of tree nodes to provide custom visuals
    /// for selection and ancestor relationships.
    /// <para>
    /// Selected nodes are rendered with a highlighted background. Nodes that are not directly selected,
    /// but have a selected ancestor (i.e., their parent or higher ancestor is selected), are considered
    /// "projected selections" and are visually indicated by drawing a rectangle around their text area.
    /// </para>
    /// </summary>
    /// <param name="args">A <see cref="DrawTreeNodeEventArgs"/> that contains the event data for the node to be drawn.</param>
    protected override void OnDrawNode(DrawTreeNodeEventArgs args)
    {
        // int nodeFrameThickness = NodeFrameThickness;
        Rectangle textBounds = args.Bounds;
        TreeNode currentNode = args.Node;
        Font nodeFont = currentNode.NodeFont;
        bool isNodeSelected = IsSelected(currentNode);
        Color highlightBackgroundColor = SystemColors.Highlight;
        Color highlightForegroundColor = SystemColors.HighlightText;
        Color backgroundColor = highlightBackgroundColor;
        Color foregroundColor = highlightForegroundColor;

        // If node has custom font, must adjust the bounds from arguments to fit the text currentNode.Text
        if (nodeFont is not null)
        {
            textBounds.Size = TextRenderer.MeasureText(currentNode.Text, nodeFont);
        }

        if (isNodeSelected)
        {
            args.Graphics.FillRectangle(SystemBrushes.Highlight, textBounds);
        }
        else
        {
            DetermineUnselectedNodeColor(currentNode, out backgroundColor, out foregroundColor);
            using (var backgroundBrush = new SolidBrush(backgroundColor)) // ##FIX## cache brush
            {
                args.Graphics.FillRectangle(backgroundBrush, textBounds);
            }
        }

        TextRenderer.DrawText(
            args.Graphics,
            currentNode.Text,
            nodeFont ?? this.Font,
            textBounds,
            foregroundColor,
            TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

        if (PaintProjectedSelections && !isNodeSelected && IsAncestorSelected(currentNode))
        {
            int nodeFrameThickness = ProjectedSelectionFrameThickness;
            Rectangle textRectangle = textBounds; /* GetNodeTextBounds(currentNode); */

            textRectangle.Inflate(-nodeFrameThickness, -nodeFrameThickness);
            using Pen framePen = new(highlightBackgroundColor, nodeFrameThickness);
            args.Graphics.DrawRectangle(framePen, textRectangle);
        }

        base.OnDrawNode(args);
    }
    #endregion // Protected_Overrides
    #endregion // Methods
}

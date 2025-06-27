// Ignore Spelling: Ctrl, TreeView, treeview, Multiselect, Sel, unselects
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PK.PkUtils.Extensions;

#pragma warning disable IDE0290     // Use primary constructor

namespace PK.PkUtils.UI.Controls;


public partial class MultiSelectTreeView : TreeView
{
    #region Typedefs

    /// <summary> 
    /// Contains information for TreeView selection change events. 
    /// </summary>
    public sealed class TreeViewSelChangeArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewSelChangeArgs"/> class.
        /// </summary>
        /// <param name="treeview">The TreeView control. Must not be null.</param>
        /// <param name="selectedNodes">The selected nodes. Must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
        public TreeViewSelChangeArgs(MultiSelectTreeView treeview, IReadOnlyCollection<TreeNode> selectedNodes)
        {
            TreeView = treeview ?? throw new ArgumentNullException(nameof(treeview));
            SelectedNodes = selectedNodes ?? throw new ArgumentNullException(nameof(selectedNodes));
            StackTrace = new StackTrace(skipFrames: 2);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewSelChangeArgs"/> class.
        /// </summary>
        /// <param name="treeview">The TreeView control. Must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="treeview"/> is null.</exception>
        public TreeViewSelChangeArgs(MultiSelectTreeView treeview)
            : this(treeview, treeview?.SelectedNodes ?? throw new ArgumentNullException(nameof(treeview)))
        { }

        /// <summary> The TreeView instance. </summary>
        public MultiSelectTreeView TreeView { get; }

        /// <summary> The collection of selected nodes. </summary>
        public IReadOnlyCollection<TreeNode> SelectedNodes { get; }

        /// <summary> Stack trace at the time of creation (skips constructor frames). </summary>
        public StackTrace StackTrace { get; }

        /// <summary> Gets selected nodes information, as output string. </summary>
        /// <returns>  The selected nodes information. </returns>
        public string GetSelectedNodesInfo() => MultiSelectTreeView.GetSelectedNodesInfo(SelectedNodes);

        /// <summary>   Returns a string that represents the current object. </summary>
        /// <returns>   A string that represents the current object. </returns>
        public override string ToString()
        {
            string[] items = [$"TreeView.Name: {TreeView.Name}", $"{GetSelectedNodesInfo()}"];
            return $"{GetType().Name}({items.Join()})";
        }
    }

    /// <summary>
    /// Provides data for a TreeView right-click event.
    /// </summary>
    public sealed class TreeViewRightClickArgs : EventArgs
    {
        /// <summary> Initializes a new instance of the <see cref="TreeViewRightClickArgs"/> class. </summary>
        /// <param name="treeView">The TreeView that was clicked. Must not be null.</param>
        /// <param name="clickedNode">The node that was clicked, or null if none.</param>
        /// <param name="location">The location of the click.</param>
        public TreeViewRightClickArgs(MultiSelectTreeView treeView, TreeNode clickedNode, Point location)
        {
            TreeView = treeView ?? throw new ArgumentNullException(nameof(treeView));
            ClickedNode = clickedNode;
            Location = location;
        }

        /// <summary> The TreeView control where the click occurred. </summary>
        public MultiSelectTreeView TreeView { get; }

        /// <summary> The node that was right-clicked, or <c>null</c> if none. </summary>
        public TreeNode ClickedNode { get; }

        /// <summary> The location of the mouse click in client coordinates. </summary>
        public Point Location { get; }

        /// <summary>   Returns a string that represents the current object. </summary>
        /// <returns>   A string that represents the current object. </returns>
        public override string ToString()
        {
            string clickedNodeInfo = (ClickedNode is null)
                ? "ClickedNode is null"
                : $"{ClickedNode.Text.AsNameValue()}, {ClickedNode.Name.AsNameValue()}";

            string[] items = [
                $"{TreeView.Name.AsNameValue()}",
                clickedNodeInfo,
                $"{Location.AsNameValue()}"
            ];

            return $"{GetType().Name}({items.Join()})";
        }
    }
    #endregion // Typedefs
}
#pragma warning restore IDE0290     // Use primary constructor
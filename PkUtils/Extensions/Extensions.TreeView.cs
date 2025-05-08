// Ignore Spelling: Utils, ctrl
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PK.PkUtils.Extensions;


/// <summary>
/// Extension methods for TreeView, TreeNodeCollection and TreeNode
/// </summary>
/// <seealso href="http://msdn.microsoft.com/en-us/library/system.windows.forms.treeview(v=vs.110).aspx">
/// System.Windows.Forms.TreeView class</seealso>
public static class TreeViewExtensions
{
    #region Extensions_of_TreeView

    /// <summary> Gets recursively all nodes in the tree view. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="tv"/> is null. </exception>
    /// <param name="tv"> A TreeView control whose nodes are retrieved. Can't be null. </param>
    /// <returns>   Enumerable collection of all nodes in the tree. </returns>
    public static IEnumerable<TreeNode> GetAllNodes(this TreeView tv)
    {
        ArgumentNullException.ThrowIfNull(tv);
        return GetAllNodes(tv.Nodes);
    }

    /// <summary>
    /// Retrieves all the TreeNode(s) that match the conditions defined by the specified predicate <paramref name="match"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when any of input arguments is null. </exception>
    /// <param name="tv"> A TreeView control whose nodes are examined. Must not be null. </param>
    /// <param name="match"> A predicate that specifies matching condition for nodes. Must not be null. </param>
    /// <returns> Resulting sequence of nodes that can be iterated. </returns>
    public static IEnumerable<TreeNode> FindMatchingNodes(
        this TreeView tv,
        Predicate<TreeNode> match)
    {
        ArgumentNullException.ThrowIfNull(tv);
        ArgumentNullException.ThrowIfNull(match);
        return FindMatchingNodes(tv.Nodes, match);
    }

    /// <summary>
    /// Finds recursively the first node, that match the conditions defined by the specified predicate; returns null if there
    /// is no such node.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when any of input arguments is null. </exception>
    /// <param name="tv"> A TreeView control whose nodes are hunted-down. Must not be equal to null. </param>
    /// <param name="match"> A predicate that matching node must fulfill. Must not be equal to null. </param>
    /// <returns>   Found TreeNode, or null if none matching node is found. </returns>
    public static TreeNode FindNode(this TreeView tv, Predicate<TreeNode> match)
    {
        ArgumentNullException.ThrowIfNull(tv);
        ArgumentNullException.ThrowIfNull(match);

        return FindMatchingNodes(tv.Nodes, match).FirstOrDefault();
    }

    /// <summary>
    /// Finds a node, given the node full path. Returns <c>null</c> if there is no such node,
    /// or the closest existing parent node if <paramref name="allowClosestParent"/> is <c>true</c>.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the <paramref name="tv"/> argument is <c>null</c>.
    /// </exception>
    /// <param name="tv">A <see cref="TreeView"/> control whose nodes are examined. Must not be <c>null</c>.</param>
    /// <param name="fullPath">
    /// A sequence of tree node texts (node names) that form a hierarchy,
    /// separated by the <see cref="TreeView.PathSeparator"/> character.
    /// </param>
    /// <param name="allowClosestParent">
    /// If true, and if the exact match is not found, returns the closest existing parent node.
    /// Otherwise returns null on failure to match exactly. Default is false.
    /// </param>
    /// <returns>
    /// A matching <see cref="TreeNode"/> if found; the closest parent node if
    /// <paramref name="allowClosestParent"/> is <c>true</c>; otherwise <c>null</c>.
    /// </returns>
    public static TreeNode FindNode(this TreeView tv, string fullPath, bool allowClosestParent = false)
    {
        ArgumentNullException.ThrowIfNull(tv);
        if (string.IsNullOrEmpty(fullPath))
            return null;

        string[] pathSegments = fullPath.Split(tv.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
        TreeNodeCollection currentNodes = tv.Nodes;
        TreeNode currentNode = null;

        foreach (string segment in pathSegments)
        {
            TreeNode nextNode = currentNodes?.OfType<TreeNode>().FirstOrDefault(node => node.Text == segment);
            if (nextNode == null)
                return allowClosestParent ? currentNode : null;

            currentNode = nextNode;
            currentNodes = currentNode.Nodes;
        }

        return currentNode;
    }


    /// <summary>   A TreeView extension method that expands all nodes. </summary>
    /// <param name="tv"> A TreeView control whose nodes are retrieved. Can't be null. </param>
    public static void ExpandAllNodes(this TreeView tv)
    {
        ArgumentNullException.ThrowIfNull(tv);
        foreach (TreeNode node in tv.GetAllNodes()) { node.Expand(); }
    }

    /// <summary>   A TreeView extension method that collapses all nodes. </summary>
    /// <param name="tv"> A TreeView control whose nodes are retrieved. Can't be null. </param>
    public static void CollapseAllNodes(this TreeView tv)
    {
        ArgumentNullException.ThrowIfNull(tv);
        foreach (TreeNode node in tv.GetAllNodes()) { node.Collapse(); }
    }

    /// <summary>   Gets the full path of the top node in the <see cref="TreeView"/>, if one exists. </summary>
    /// <param name="tv"> The TreeView control from which to retrieve the top node path. </param>
    /// <returns>   The full path of the top node if it exists; otherwise, <c>null</c>. </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name="tv"/> is <c>null</c>. </exception>/// 
    /// <seealso cref="RestoreTopNode"/>
    public static string GetTopNodePath(this TreeView tv)
    {
        ArgumentNullException.ThrowIfNull(tv);
        return tv.TopNode?.FullPath;
    }

    /// <summary>
    /// Attempts to restore the top node of the <see cref="TreeView"/> by using the specified full path.
    /// </summary>
    /// <param name="tv">The <see cref="TreeView"/> control on which to restore the top node.</param>
    /// <param name="topNodePath">
    /// The full path of the node to restore as the top node. The path is a sequence of node texts separated
    /// by the <see cref="TreeView.PathSeparator"/> character.
    /// </param>
    /// <param name="allowClosestParent">
    /// If true and an exact match for <paramref name="topNodePath"/> is not found, the method will
    /// attempt to restore the closest existing parent node instead. If false, only an exact match is accepted.
    /// </param>
    /// <returns>
    /// true if a node matching <paramref name="topNodePath"/> (or its closest parent, if allowed)
    /// was found and restored as the top node; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="tv"/> is null.</exception>
    /// <seealso cref="GetTopNodePath"/>
    /// <seealso cref="FindNode(TreeView, string, bool)"/>
    public static bool RestoreTopNode(this TreeView tv, string topNodePath, bool allowClosestParent = false)
    {
        ArgumentNullException.ThrowIfNull(tv);
        bool result = false;

        if (!string.IsNullOrEmpty(topNodePath))
        {
            TreeNode node = tv.FindNode(topNodePath, allowClosestParent);
            if (result = (node is not null))
                tv.TopNode = node;
        }
        return result;
    }
    #endregion // Extensions_of_TreeView

    #region Extensions_of_TreeNodeCollection

    /// <summary>
    /// Retrieves recursively all TreeNode(s) in the provided TreeNodeCollection, including their child nodes.
    /// </summary>
    /// <param name="tc">A collection of tree nodes. Could be null. </param>
    /// <returns>Enumerable collection of all nodes in the tree.</returns>
    public static IEnumerable<TreeNode> GetAllNodes(this TreeNodeCollection tc)
    {
        if (tc == null)
            yield break;

        foreach (TreeNode tn in tc)
        {
            yield return tn;

            foreach (TreeNode child in GetAllNodes(tn.Nodes))
            {
                yield return child;
            }
        }
    }

    /// <summary>
    /// Retrieves all the TreeNode(s) that match the conditions defined by the specified predicate <paramref name="match"/>,
    /// including their child nodes.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when the input argument <paramref name="match"/> is null. </exception>
    /// <param name="tc"> A collection of tree nodes, usually retrieved through TreeView.Nodes property. Could be null.  </param>
    /// <param name="match"> A predicate that specifies matching condition for nodes. Must not be null. </param>
    /// <returns> Resulting sequence of nodes that can be iterated. </returns>
    public static IEnumerable<TreeNode> FindMatchingNodes(
        TreeNodeCollection tc,
        Predicate<TreeNode> match)
    {
        ArgumentNullException.ThrowIfNull(match);

        if (tc == null)
            yield break;

        foreach (TreeNode tn in tc)
        {
            if (match(tn))
            {
                yield return tn;
            }

            foreach (TreeNode tcn in FindMatchingNodes(tn.Nodes, match))
            {
                yield return tcn;
            }
        }
    }
    #endregion // Extensions_of_TreeNodeCollection

    #region Extensions_of_TreeNode

    /// <summary> A TreeNode extension method that query if 'tn' is root. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="tn"/> is null. </exception>
    /// <param name="tn"> A tree node examined. Can't be null. </param>
    /// <returns>   True if root, false if not. </returns>
    public static bool IsRoot(this TreeNode tn)
    {
        ArgumentNullException.ThrowIfNull(tn);
        return (tn.Parent is null);
    }

    /// <summary> A TreeNode extension method that query if 'tn' is root. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="tn"/> is null. </exception>
    /// <param name="tn"> A tree node examined. Can't be null. </param>
    /// <returns>   True if root, false if not. </returns>
    public static bool IsLeaf(this TreeNode tn)
    {
        ArgumentNullException.ThrowIfNull(tn);
        return (tn.Nodes.Count == 0);
    }

    /// <summary>
    /// Finds all children, grand-children etc. that match the conditions defined by the specified predicate <paramref name="match"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when the <paramref name="tn"/> argument is null. </exception>
    /// <param name="tn"> A tree node whose child nodes are examined. Can't be null. </param>
    /// <param name="match"> A predicate that specifies matching condition for nodes.  
    /// Must not be equal to null. </param>
    /// <returns>   Resulting sequence of nodes that can be iterated. </returns>
    /// <seealso cref="FindChildNode"/>
    public static IEnumerable<TreeNode> FindAllChildren(
        this TreeNode tn,
        Predicate<TreeNode> match)
    {
        ArgumentNullException.ThrowIfNull(tn);
        return FindMatchingNodes(tn.Nodes, match);
    }

    /// <summary>
    /// Finds the first child node, that match the conditions defined by the specified predicate.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when the <paramref name="tn"/> argument is null. </exception>
    /// <param name="tn"> A tree node whose child nodes are examined. Can't be null. </param>
    /// <param name="match"> A predicate that specifies matching condition for nodes. Must not be equal to null. </param>
    /// <returns> Found TreeNode, or null if none matching node is found. Otherwise null. </returns>
    /// <seealso cref="FindAllChildren"/>
    public static TreeNode FindChildNode(
        this TreeNode tn,
        Predicate<TreeNode> match)
    {
        ArgumentNullException.ThrowIfNull(tn);
        return tn.FindAllChildren(match).FirstOrDefault();
    }

    /// <summary> For a given TreeNode returns a path from the root node to the given node. </summary>
    /// <param name="tnChild"> A child tree node. Must NOT equal null. </param>
    /// <returns>
    /// Returns a path from the root node to the given node, with nodes separated by TreeView.PathSeparator
    /// character ( which is usually backslash by default).
    /// <exception cref="ArgumentNullException"> Thrown when the <paramref name="tnChild"/> argument is null. </exception>
    /// </returns>
    /// <seealso cref="FindNode(TreeView, string, bool)"/>
    public static IReadOnlyList<TreeNode> NodePath(this TreeNode tnChild)
    {
        return tnChild.NodePath(null);
    }

    /// <summary>
    /// For a given TreeNode <paramref name="tnChild"/> returns a path to the node tnAbove, 
    /// where tnAbove is either a parent or grand-parent or grand-grand-parent etc.
    /// </summary>
    /// <param name="tnChild">A child tree node. Must not equal null.</param>
    /// <param name="tnAbove">A direct or indirect parent of starting tree node. Could be null. </param>
    /// <returns>
    /// The resulting path. Contains tnAbove as the first item and tnChild as the last item.
    /// If tnAbove is null, it will contain as the first item the node at the very top (root) level.
    /// If arguments tnChild and tnAbove are equal, the resulting list contains just one item.
    /// </returns>
    /// <exception cref="ArgumentNullException"> Thrown when the <paramref name="tnChild"/> argument is null. </exception>
    /// <remarks> 
    /// Returns null on failure, i.e. if a given argument <paramref name="tnAbove"/> is not null, 
    /// but it is NOT direct or indirect parent of the node <paramref name="tnChild"/>.
    /// </remarks>
    public static IReadOnlyList<TreeNode> NodePath(this TreeNode tnChild, TreeNode tnAbove)
    {
        ArgumentNullException.ThrowIfNull(tnChild);

        List<TreeNode> result = [];
        bool bOk = true;

        for (TreeNode lastParsed = tnChild; ;)
        {
            result.Add(lastParsed);
            if (lastParsed == tnAbove)
            {
                break;
            }
            if (null == (lastParsed = lastParsed.Parent))
            {
                bOk = (null == tnAbove);
                break;
            }
        }

        if (!bOk)
            result = null;
        else
            result.Reverse();

        return result;
    }
    #endregion // Extensions_of_TreeNode
}

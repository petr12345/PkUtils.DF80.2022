using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PK.PkUtils.UI.General;

#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.PkUtils.Extensions;


/// <summary>
/// Contains reusable extensions of <see cref="MultiSelectTreeView "/>.
/// </summary>
public static class MultiSelectTreeViewExtensions
{
    #region Methods

    /// <summary>  Returns all selected nodes paths. </summary>
    /// <param name="treeView"> The treeView to act on. Can't be null. </param>
    /// <returns> The array of selected nodes paths, sorted alphabetically. </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="treeView"/> is null. </exception>/// 
    public static string[] FindSelectedNodesPaths(this MultiSelectTreeView treeView)
    {
        ArgumentNullException.ThrowIfNull(treeView);
        return treeView.SelectedNodes.Select(node => node.FullPath).OrderBy(s => s).ToArray();
    }

    /// <summary>   A MultiSelectTreeView extension method that restore selected nodes state. </summary>
    /// <param name="treeView"> The treeView to act on. Can't be null. </param>
    /// <param name="arraySelectedNodesPaths"> The selected nodes paths. May be null or empty. If null,
    /// selection is NOT changed at all. </param>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="treeView"/> is null. </exception>
    public static void RestoreSelectedNodesState(this MultiSelectTreeView treeView, string[] arraySelectedNodesPaths)
    {
        ArgumentNullException.ThrowIfNull(treeView);

        if (arraySelectedNodesPaths is null) return;
        List<TreeNode> nodes = arraySelectedNodesPaths.Select(s => treeView.FindNode(s)).Where(x => x is not null).ToList();
        treeView.SelectedNodes = nodes;
    }
    #endregion // Methods
}

#pragma warning restore IDE0305
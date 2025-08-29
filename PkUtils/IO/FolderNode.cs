// Ignore Spelling: Utils, Subfolders
//
using System;
using System.Collections.Generic;
using System.IO;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.IO;

/// <summary> Represents a folder node. </summary>
public class FolderNode : IFolderNode
{
    #region Constructor(s)

    /// <summary> The only constructor. </summary>
    /// <param name="name"> The name. Can't be null. </param>
    public FolderNode(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        Name = name;
        SubfoldersDictionary = new(StringComparer.OrdinalIgnoreCase);
        FilesList = [];
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Gets the collection of subfolders. </summary>
    protected internal Dictionary<string, FolderNode> SubfoldersDictionary { get; }

    /// <summary>
    /// Gets the collection of files contained in this folder.
    /// </summary>
    protected internal List<FileInfo> FilesList { get; }
    #endregion // Properties

    #region IFolderNode Members

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IEnumerable<IFolderNode> Subfolders { get => SubfoldersDictionary.Values; }

    /// <inheritdoc/>
    public IEnumerable<FileInfo> Files { get => this.FilesList; }

    #endregion // IFolderNode Members

    #region Methods

    /// <summary> Normalize root path. </summary>
    /// <param name="rootPath"> Full pathname of the root file. Can't be null. </param>
    /// <returns>   A normalized root path string. </returns>
    public static string NormalizeRootPath(string rootPath)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(rootPath);

        rootPath = Path.GetFullPath(rootPath);

        // Remove any trailing separator unless it is the root itself, for example @"e:\"
        rootPath = Path.TrimEndingDirectorySeparator(rootPath);

        // Normalize slashes in rootPath (ensure consistent directory separator)
        rootPath = rootPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

        return rootPath;
    }

    /// <summary> Builds file tree. </summary>
    /// <param name="files"> The sequence of all files in the tree. Can't be null. </param>
    /// <param name="rootPath"> Full pathname of the root file. Can't be null. </param>
    /// <returns>   A root FolderNode. </returns>
    public static FolderNode BuildFileTree(IEnumerable<FileInfo> files, string rootPath)
    {
        ArgumentNullException.ThrowIfNull(files);
        ArgumentNullException.ThrowIfNull(rootPath);

        rootPath = NormalizeRootPath(rootPath);
        FolderNode root = new(rootPath);

        foreach (FileInfo file in files)
        {
            string relativePath = Path.GetRelativePath(rootPath, file.FullName);

            // Validate that the file is under the root path
            if (relativePath.StartsWith("..", StringComparison.OrdinalIgnoreCase) || Path.IsPathRooted(relativePath))
            {
                throw new ArgumentException($"File '{file.FullName}' is not under the root path '{rootPath}'.", nameof(files));
            }

            string[] parts = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            int partsLength = parts.Length;
            FolderNode current = root;

            for (int ii = 0; ii < partsLength - 1; ii++)
            {
                string part = parts[ii];
                if (!current.SubfoldersDictionary.TryGetValue(part, out FolderNode next))
                {
                    current.SubfoldersDictionary[part] = next = new FolderNode(part);
                }
                current = next;
            }
            current.FilesList.Add(file);
        }

        return root;
    }
    #endregion // Methods
}

// Ignore Spelling: CCA, Utils, subfolder, subfolders, SubNodes
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PK.PkUtils.Interfaces;


#pragma warning disable IDE0057 // Use range operator

namespace PK.PkUtils.IO;


/// <summary>
/// Represents a lazily evaluated folder structure.
/// </summary>
/// <remarks>
/// This design is useful when working with large or slow <see cref="IEnumerable{FileInfo}"/> sources 
/// (e.g., a network drive or database query). It leverages lazy evaluation to improve efficiency 
/// and reduce memory usage. Key concepts:
/// 
/// 1. **Lazy Evaluation** – Both files and subfolders are generated on demand using <see cref="IEnumerable{T}"/>, 
///    reducing memory usage and improving efficiency for large file collections.
/// 2. **LINQ Lookup (<see cref="ILookup{TKey, TElement}"/>)** – Prevents multiple enumerations of the original 
///    <see cref="IEnumerable{FileInfo}"/>, making the process more efficient.
/// 3. **Recursive Generation** – The tree structure is built recursively while maintaining lazy evaluation.
/// </remarks>
public class FolderNodeDeferred : IFolderNode
{
    #region Constructor(s)

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderNodeDeferred"/> class.
    /// </summary>
    /// <param name="name">The name of the folder. Cannot be null.</param>
    public FolderNodeDeferred(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        Name = name;
    }

    private FolderNodeDeferred(string name, IEnumerable<FileInfo> files, IEnumerable<IFolderNode> subfolders)
    {
        Name = name;
        Files = files;
        Subfolders = subfolders;
    }
    #endregion // Constructor(s)

    #region IFolderNode Members

    /// <summary> Gets the name of the folder. </summary>
    public string Name { get; }

    /// <inheritdoc/>
    public IEnumerable<IFolderNode> Subfolders { get; init; } = [];

    /// <summary>
    /// Gets the lazily evaluated collection of files contained in this folder.
    /// </summary>
    public IEnumerable<FileInfo> Files { get; init; } = [];

    #endregion // IFolderNode Members

    #region Methods

    /// <summary>   Normalize root path. </summary>
    /// <param name="rootPath"> Full pathname of the root file. Can't be null. </param>
    /// <returns>   A normalized root path string. </returns>
    public static string NormalizeRootPath(string rootPath)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(rootPath);

        rootPath = Path.GetFullPath(rootPath);

        // Remove any trailing separator unless it is the root itself, for example @"e:\";
        rootPath = Path.TrimEndingDirectorySeparator(rootPath);
        // Normalize slashes in rootPath (ensure consistent directory separator)
        rootPath = rootPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

        return rootPath;
    }

    /// <summary>
    /// Builds a lazy file tree from a given sequence of files and a root directory.
    /// </summary>
    /// <param name="files">The sequence of files. Cannot be null.</param>
    /// <param name="rootPath">The full path of the root directory. Cannot be null.</param>
    /// <returns>The root <see cref="FolderNodeDeferred"/> representing the file tree.</returns>
    public static FolderNodeDeferred BuildFileTree(IEnumerable<FileInfo> files, string rootPath)
    {
        ArgumentNullException.ThrowIfNull(files);
        ArgumentNullException.ThrowIfNull(rootPath);

        rootPath = NormalizeRootPath(rootPath);

        // Construct main lookup
        ILookup<string, FileInfo> lookup = files
            .Where(file =>
            {
                string relativePath = Path.GetRelativePath(rootPath, file.FullName);
                if (relativePath.StartsWith("..", StringComparison.OrdinalIgnoreCase) || Path.IsPathRooted(relativePath))
                {
                    throw new ArgumentException($"File '{file.FullName}' is not under the root path '{rootPath}'.", nameof(files));
                }
                return true;
            })
            .ToLookup(file => Path.GetDirectoryName(file.FullName));

        // Root folder node, with lazy loading of files and subfolders
        return new FolderNodeDeferred(rootPath,
            BuildFiles(lookup, rootPath),
            BuildSubfolders(lookup, rootPath, rootPath));
    }

    /// <summary>
    /// Builds the collection of files that belong to a specific folder.
    /// </summary>
    /// <param name="lookup">The lookup table of file paths.</param>
    /// <param name="folderPath">The path of the current folder.</param>
    /// <returns>A lazily evaluated sequence of files belonging to the specified folder.</returns>
    private static IEnumerable<FileInfo> BuildFiles(
        ILookup<string, FileInfo> lookup,
        string folderPath)
    {
        // Normalize the folderPath to ensure consistent format
        string normalizedFolderPath = folderPath
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar) // Replace alternate separator with the native one
            .TrimEnd(Path.DirectorySeparatorChar); // Remove trailing separator to avoid mismatch
        int folderSeparatorCount = normalizedFolderPath.Count(c => c == Path.DirectorySeparatorChar);

        return lookup
            .Where(group =>
            {
                // Normalize the group key once, cache the count of directory separators
                string normalizedGroupKey = group.Key
                    .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar) // Normalize group key
                    .TrimEnd(Path.DirectorySeparatorChar); // Remove trailing separator
                int groupSeparatorCount = normalizedGroupKey.Count(c => c == Path.DirectorySeparatorChar);

                return normalizedGroupKey.StartsWith(normalizedFolderPath, StringComparison.OrdinalIgnoreCase)
                    && groupSeparatorCount == folderSeparatorCount;
            })
            .SelectMany(group => group);
    }

    /// <summary>
    /// Builds the collection of subfolder nodes for a given folder.
    /// </summary>
    /// <param name="lookup">The lookup table of file paths.</param>
    /// <param name="rootPath">The root directory path.</param>
    /// <param name="folderPath">The path of the current folder.</param>
    /// <returns>A lazily evaluated sequence of subfolder nodes.</returns>
    private static IEnumerable<IFolderNode> BuildSubfolders(
        ILookup<string, FileInfo> lookup,
        string rootPath,
        string folderPath)
    {
        // Normalize the folderPath to ensure it has the appropriate directory separator, and no trailing separator
        folderPath = NormalizeRootPath(folderPath);

        IEnumerable<string> subfolderNames = lookup
            .Where(group =>
            {
                return group.Key.StartsWith(folderPath, StringComparison.OrdinalIgnoreCase);
            })
            .Select(group =>
            {
                // Get the first subfolder
                return group.Key.Substring(folderPath.Length)
                    .TrimStart(Path.DirectorySeparatorChar)
                    .Split(Path.DirectorySeparatorChar)[0];
            })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(subfolder => !string.IsNullOrEmpty(subfolder));  // Exclude empty subfolder names

        foreach (string subfolderName in subfolderNames)
        {
            string subfolderPath = Path.Combine(folderPath, subfolderName);
            FolderNodeDeferred subfolder = new(subfolderName)
            {
                Files = BuildFiles(lookup, subfolderPath),
                Subfolders = BuildSubfolders(lookup, rootPath, subfolderPath)
            };
            yield return subfolder;
        }
    }
    #endregion // Methods
}
#pragma warning restore IDE0057

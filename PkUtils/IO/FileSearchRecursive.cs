// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.PkUtils.IO;

/// <summary>
/// Derives from (implements) the abstract class FileSearchBase,
/// (using internally a recursive-call algorithm).
/// </summary>
/// <remarks>
/// Unless the property <see cref="FoldersComparer"/> is initialized with non-null comparer, 
/// the order of directories returned with current implementation is so-called "pre-order"
/// (root, left, right). For more information, see for instance 
/// <see href="http://en.wikipedia.org/wiki/Tree_traversal#Pre-order">
/// Wikipedia about Tree traversal</see> or
/// <see href="http://www.math.ucla.edu/~wittman/10b.1.10w/Lectures/Lec18.pdf">
/// Todd Wittman's Lecture 18 Tree Traversals</see><br/>
///
/// Note that parsing avoids folders with attribute FileAttributes.ReparsePoint, which generally
/// indicates a block of user-defined data associated with a file or a directory.
/// In case of directory, this means a reparse points, in other words a junction points,
/// i.e. a disk is mounted within folder. Junction points generate a problem when recursively scanning
/// folders; drives can be mounted in such a way as to create infinite directory structures.
/// For more info, see for instance
/// <see href="http://www.blackwasp.co.uk/FolderRecursion.aspx"> Folder Recursion with C# </see>
/// </remarks>
[CLSCompliant(true)]
public class FileSearchRecursive : FileSearchBase
{
    #region Fields

    /// <summary> A backing field of property <see cref="FoldersComparer"/>. </summary>
    private IComparer<DirectoryInfo> _foldersComparer;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default argument-less constructor
    /// </summary>
    public FileSearchRecursive()
    {
    }

    /// <summary> Public constructor initializing <see cref="FoldersComparer"/> property. </summary>
    /// <param name="fComparer"> The comparer. </param>
    public FileSearchRecursive(IComparer<DirectoryInfo> fComparer)
    {
        FoldersComparer = fComparer;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Gets or sets the comparer, that is used to sort-out folders on the same level 
    /// during the parsing process. 
    /// The default value of property is null; if a non-null value is assigned, then the order
    /// in which sub-directories on the same level are parsed is determined by this comparer.
    /// </summary>
    public IComparer<DirectoryInfo> FoldersComparer
    {
        get { return _foldersComparer; }
        set { _foldersComparer = value; }
    }
    #endregion // Properties

    #region Methods
    #region Public Methods

    /// <summary> Implementation of predecessor's abstract method SearchFiles. </summary>
    /// <remarks>
    /// Delegates the functionality to protected method <see cref="DoSearchFiles"/>.
    /// </remarks>
    ///
    /// <param name="dirRoot"> The folder to begin the search with. </param>
    /// <param name="pattern"> A filename which can include wildcard characters. For instance "*.dll". 
    /// If this argument is null or empty, pattern "*.*" will be used instead. </param>
    /// <param name="searchOption"> A value which specifies how the search should be performed. </param>
    /// 
    /// <returns> Resulting sequence of files that can be iterated. </returns>
    public override IEnumerable<FileInfo> SearchFiles(DirectoryInfo dirRoot, string pattern,
      SearchOption searchOption)
    {
        if (string.IsNullOrEmpty(pattern))
            pattern = "*.*";
        ClearCanceled();
        return DoSearchFiles(0, dirRoot, pattern, searchOption);
    }

    /// <summary>
    /// Search for given file pattern all directories under the given folder (including that root). Will
    /// search recursively if searchOption is FileSearchOption.AllDirectories.
    /// </summary>
    /// <remarks>
    /// Delegates the functionality to protected method <see cref="DoSearchDirectories"/>.
    /// </remarks>
    /// 
    /// <param name="dirRoot"> The directory to begin the search with. </param>
    /// <param name="searchOption"> A value which specifies how the search should be performed. 
    /// In more detail ( to avoid confusion), it relates to the fact 'sub-directories of what' 
    /// will be returned. For instance, if dirRoot is 'c:\Tmp3', and searchOption has a value 
    /// SearchOption.TopDirectoryOnly, the method will return all direct sub-directories of c:\Tmp3' 
    /// ( and 'c:\Tmp3' itself if includeTop is true ).
    /// </param>
    /// <param name="includeTop"> true to include top folder in returned enumeration. </param>
    /// 
    /// <returns> Resulting sequence of directories that can be iterated. </returns>
    public override IEnumerable<DirectoryInfo> SearchDirectories(DirectoryInfo dirRoot,
      SearchOption searchOption, bool includeTop)
    {
        ClearCanceled();
        return DoSearchDirectories(0, dirRoot, searchOption, includeTop);
    }
    #endregion // Public Methods

    #region Protected Methods

    /// <summary> Auxiliary method called by method SearchFiles, performing its actual functionality. 
    /// </summary>
    /// <remarks> Regarding the order of directories parsing during enumeration, and handling of a reparse 
    /// points, see a general remark for this whole class. </remarks>
    /// 
    /// <param name="nParseLevel"> The parsed directory level. Zero means top folder,
    ///  one its immediate sub-folder etc. </param>
    /// <param name="dirRoot"> The folder to begin the search with. </param>
    /// <param name="pattern"> A filename which can include wildcard characters. For instance "*.dll". </param>
    /// <param name="searchOption"> A value which specifies how the search should be performed. </param>
    /// 
    /// <returns> Resulting sequence of files that can be iterated. </returns>
    protected virtual IEnumerable<FileInfo> DoSearchFiles(
        int nParseLevel,
        DirectoryInfo dirRoot,
        string pattern,
        SearchOption searchOption)
    {
        ArgumentNullException.ThrowIfNull(dirRoot);
        ArgumentNullException.ThrowIfNull(pattern);

        // 1. -- Entering the folder itself

        // Following 'yield break' essentially informs the iterator that there are no more values.
        // If it's the first item hit in the method, it will be like returning an empty list. 
        // The method itself still returns an IEnumerable, but if you try to iterate 
        // it will be like iterating an empty list (that is, no iterations will occur). 
        // If you try to re-iterate the same IEnumerable result, it will call the method a second time. 
        // Depending on related code logic and other circumstances, that might mean it will hit
        // yield break the same way (with an empty list) or perhaps this time you'll have it yield values.
        // 
        // Remark: In this concrete case, since the algorithm involves recursion,
        // with individual 'foreach ... { yield return .. } ' blocks involved,
        // just one 'yield break' is not sufficient enough to stop parsing completely;
        // hence additional property IsCanceled has to be involved.
        //
        // For more info, see for instance
        // http://stackoverflow.com/questions/231893/what-does-yield-break-do-in-c
        //
        if (!OnFolderEnter(dirRoot, nParseLevel))
            yield break; // specifies that an iterator has come to an end

        // 2. -- Process all files in	that folder, possibly with sub-folders
        //
        bool getDirs = (searchOption == SearchOption.AllDirectories);
        bool keepRetry = true;
        FileInfo[] files = null;
        DirectoryInfo[] dirs = null;

        while (!IsCanceled && keepRetry && ((files == null) || (getDirs && dirs == null)))
        {
            keepRetry = false;
            try
            {
                files = dirRoot.GetFiles(pattern);
                if (getDirs)
                {
                    var enDirs = dirRoot.GetDirectories().Where(
                      d => (d.Attributes & FileAttributes.ReparsePoint) == 0);
                    if (FoldersComparer != null)
                        enDirs = enDirs.OrderBy(di => di, FoldersComparer);
                    dirs = enDirs.ToArray();
                }
            }
            catch (SystemException ex)
            {
                // there is no access to that folder
                OnParseError(dirRoot, nParseLevel, ex, out keepRetry);
            }
        }
        if (IsCanceled)
            yield break; // specifies that an iterator has come to an end

        if (files != null)
        {
            foreach (FileInfo fi in files)
            {
                Debug.Assert(((fi.Attributes & FileAttributes.Directory) == 0));
                yield return fi;
            }
        }

        // 3. -- Add files from all sub-folders if that's what	the	user wants
        //
        if (dirs != null)
        {
            foreach (var childDir in dirs)
            {
                if (IsCanceled)
                    yield break; // specifies that an iterator has come to an end
                foreach (FileInfo info in DoSearchFiles(nParseLevel + 1, childDir, pattern, searchOption))
                {
                    yield return info;
                }
            }
        }

        // 4. -- Leaving the folder
        //
        if (!OnFolderExit(dirRoot, nParseLevel))
            yield break; // specifies that an iterator has come to an end
    }

    /// <summary> Method called by method SearchDirectories, performing its actual functionality. </summary>
    /// <remarks> Regarding the order of directories parsing during enumeration, and handling of a reparse 
    /// points, see a general remark for this whole class. </remarks>
    /// 
    /// <param name="nParseLevel"> The parsed directory level. Zero means top folder,
    ///  one its immediate sub-folder etc. </param>
    /// <param name="dirRoot"> The directory to begin the search with. </param>
    /// <param name="searchOption"> A value which specifies how the search should be performed. </param>
    /// <param name="includeTop"> true to include top folder in returned enumeration. </param>
    /// 
    /// <returns> Resulting sequence of directories that can be iterated. </returns>
    protected virtual IEnumerable<DirectoryInfo> DoSearchDirectories(
        int nParseLevel,
        DirectoryInfo dirRoot,
        SearchOption searchOption,
        bool includeTop)
    {
        // Possible variant of declaration:
        // DoSearchDirectories(int nParseLevel, DirectoryInfo dirRoot, bool includeTop, int nMaxLevel = int.MaxValue)
        // where for nMaxLevel = 0 it returns only top or nothing ( depending on includeTop value),
        // where for nMaxLevel = 1 it returns the same as now for SearchOption.TopDirectoryOnly,
        // etc.
        ArgumentNullException.ThrowIfNull(dirRoot);

        string dirPath;

        // Process this folder
        // Get the name of the next folder and the corresponding search pattern
        if (!string.IsNullOrEmpty(dirPath = FilePathHelper.SafeGetFullName(dirRoot).Trim()))
        {
            // 1. -- Entering the folder itself
            //
            if (!OnFolderEnter(dirRoot, nParseLevel))
                yield break; // specifies that an iterator has come to an end

            // got a new one. skip if excluding root is needed
            if (includeTop)
            {
                yield return new DirectoryInfo(dirPath);
            }

            // 2. -- Process sub-folders in that folder
            //
            DirectoryInfo[] dirs = null;
            bool keepRetry = true;

            if ((nParseLevel == 0) || (searchOption == SearchOption.AllDirectories))
            {
                while (!IsCanceled && keepRetry && (dirs == null))
                {
                    keepRetry = false;
                    try
                    {
                        var enDirs = dirRoot.GetDirectories().Where(
                          d => (d.Attributes & FileAttributes.ReparsePoint) == 0);
                        if (FoldersComparer != null)
                            enDirs = enDirs.OrderBy(di => di, FoldersComparer);
                        dirs = enDirs.ToArray();
                    }
                    catch (SystemException ex)
                    {
                        OnParseError(dirRoot, nParseLevel, ex, out keepRetry);
                    }
                }
            }
            if (IsCanceled)
                yield break; // specifies that an iterator has come to an end

            // 3. -- Add all sub-folders if	that's what	the	user wants
            //
            if (dirs != null)
            {
                foreach (var childDir in dirs)
                {
                    if (IsCanceled)
                        yield break; // specifies that an iterator has come to an end

                    // Uncomment following in case childDir should be returned here now.
                    // In that case, DoSearchDirectories should be called with last argument includeTop = false.
                    /* yield return new DirectoryInfo(FilePathHelper.SafeGetFullName(childDir));
                    */
                    foreach (var info in DoSearchDirectories(nParseLevel + 1, childDir, searchOption, true))
                    {
                        yield return info;
                    }
                }
            }

            // 4. -- Leaving the folder
            //
            if (!OnFolderExit(dirRoot, nParseLevel))
                yield break; // specifies that an iterator has come to an end
        }
    }
    #endregion // Protected Methods
    #endregion // Methods
}
#pragma warning restore IDE0305
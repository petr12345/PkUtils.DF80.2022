// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using PK.PkUtils.Extensions;

#pragma warning disable IDE0305 // Collection initialization can be simplified


namespace PK.PkUtils.IO;

/// <summary>
/// Derives from (implements) the abstract class FileSearchBase,
/// (using internally a non-recursive-call algorithm).
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
public class FileSearchNonRecursive : FileSearchBase
{
    #region Fields

    /// <summary> A backing field of property <see cref="FoldersComparer"/>. </summary>
    private IComparer<DirectoryInfo> _foldersComparer;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Public argument-less constructor
    /// </summary>
    public FileSearchNonRecursive()
    {
    }

    /// <summary> Public constructor initializing <see cref="FoldersComparer"/> property. </summary>
    /// <param name="fComparer"> The comparer. </param>
    public FileSearchNonRecursive(IComparer<DirectoryInfo> fComparer)
    {
        FoldersComparer = fComparer;
    }
    #endregion // Constructor

    #region Public Properties

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
    #endregion // Public Properties

    #region Methods
    #region Public Methods

    /// <summary> Implementation of predecessor's abstract method SearchFiles. </summary>
    /// <remarks> Regarding the order of directories parsing during enumeration, and handling of a reparse 
    /// points, see a general remark for this whole class. </remarks>
    ///
    /// <param name="dirRoot"> The directory to begin the search with. </param>
    /// <param name="pattern"> A filename which can include wildcard characters. For instance "*.dll". 
    /// If this argument is null or empty, pattern "*.*" will be used instead. </param>
    /// <param name="searchOption"> A value which specifies how the search should be performed. </param>
    /// 
    /// <returns> Resulting sequence of files that can be iterated. </returns>
    public override IEnumerable<FileInfo> SearchFiles(
        DirectoryInfo dirRoot,
        string pattern,
        SearchOption searchOption)
    {
        ArgumentNullException.ThrowIfNull(dirRoot);

        if (string.IsNullOrEmpty(pattern))
            pattern = "*.*";
        this.ClearCanceled();

        // Initialize the stack of folders
        Stack<(DirectoryInfo DirInfo, int Level)> dirStack = new();
        dirStack.Push((dirRoot, 0));

        // Process each folder until the stack is empty
        while (dirStack.Count > 0)
        {
            // Get the name	of the next	folder and the corresponding	search pattern
            (DirectoryInfo DirInfo, int Level) = dirStack.Pop();
            DirectoryInfo dirParsed = DirInfo;
            int nParseLevel = Level;

            // 1. -- Entering the folder itself
            //
            // Following 'yield break' essentially informs the iterator that there are no more values.
            // If it's the first item hit in the method, it will be like returning an empty list. 
            // The method itself still returns an IEnumerable, but if you try to iterate 
            // it will be like iterating an empty list (that is, no iterations will occur). 
            // If you try to re-iterate the same IEnumerable result, it will call the method a second time. 
            // Depending on related code logic and other circumstances, that might mean it will hit
            // yield break the same way (with an empty list) or perhaps this time you'll have it yield values.
            // 
            // Remark: In this concrete case, since the algorithm does not involve recursion, everything
            // is done in a single method, hence just one 'yield break' is sufficient enough to stop
            // all parsing.
            //
            // For more info, see for instance
            // http://stackoverflow.com/questions/231893/what-does-yield-break-do-in-c
            //
            if (!OnFolderEnter(dirParsed, nParseLevel))
                yield break;

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
                    files = dirParsed.GetFiles(pattern);
                    if (getDirs)
                    {
                        var enDirs = dirParsed.GetDirectories().Where(
                          d => (d.Attributes & FileAttributes.ReparsePoint) == 0);
                        if (FoldersComparer != null)
                            enDirs = enDirs.OrderBy(di => di, FoldersComparer);
                        dirs = enDirs.ToArray();
                    }
                }
                catch (SystemException ex)
                { // there is no access to that folder
                    OnParseError(dirParsed, nParseLevel, ex, out keepRetry);
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

            // 3. -- Add all sub-folders if	that's what	the	user wants
            //
            if (dirs != null)
            {
                // Reverse the order of pushed items to make the left items popped first. 
                // That way, tree traversal will keep "pre-order" (root, left, right) order.
                dirStack.PushRange(dirs.Reverse().Select(
                    childDir => (DirInfo: childDir, Level: nParseLevel + 1)));
            }

            // 4. -- Leaving the folder
            //
            if (!OnFolderExit(dirParsed, nParseLevel))
                yield break; // specifies that an iterator has come to an end
        }
    }

    /// <summary> Implementation of predecessor's abstract method SearchDirectories. </summary>
    /// <remarks> Regarding the order of directories parsing during enumeration, and handling of a reparse 
    /// points, see a general remark for this whole class. </remarks>
    ///
    /// <param name="dirRoot"> The directory to begin the search with. </param>
    /// <param name="searchOption"> A value which specifies how the search should be performed. 
    /// In more detail ( to avoid confusion), it relates to the fact 'sub-directories of what' 
    /// will be returned. For instance, if dirRoot is 'c:\Tmp3', and searchOption has a value 
    /// SearchOption.TopDirectoryOnly, the method will return all direct sub-directories of c:\Tmp3' 
    /// ( and 'c:\Tmp3' itself if includeTop is true ).
    /// </param>
    /// <param name="includeTop"> true to include top directory in returned enumeration. </param>
    /// 
    /// <returns> Resulting sequence of directories that can be iterated. </returns>
    public override IEnumerable<DirectoryInfo> SearchDirectories(
        DirectoryInfo dirRoot,
        SearchOption searchOption,
        bool includeTop)
    {
        ArgumentNullException.ThrowIfNull(dirRoot);
        this.ClearCanceled();

        // Initialize the stack
        bool passedRoot = false;
        Stack<(DirectoryInfo DirInfo, int Level)> dirStack = new();
        dirStack.Push((DirInfo: dirRoot, Level: 0));

        // Process each folder until the stack is empty
        while (dirStack.Count > 0)
        {
            // Get the name of the next folder and the corresponding search pattern
            (DirectoryInfo DirInfo, int Level) = dirStack.Pop();
            DirectoryInfo dirParsed = DirInfo;
            int nParseLevel = Level;
            string parsedPath = dirParsed.FullName.Trim();

            // 1. -- Entering the folder itself
            //
            if (string.IsNullOrEmpty(parsedPath))
                continue;
            if (!OnFolderEnter(dirParsed, nParseLevel))
                yield break; // specifies that an iterator has come to an end

            // Got a new folder
            // because of yield, must return a new instance of DirectoryInfo, instead of popped value
            if (passedRoot || includeTop)
            {
                yield return new DirectoryInfo(parsedPath);
            }
            passedRoot = true;

            // 2. -- Process sub-folders in that folder
            //
            DirectoryInfo[] dirs = null;

            if ((nParseLevel == 0) || (searchOption == SearchOption.AllDirectories))
            {
                bool keepRetry = true;

                while (!IsCanceled && keepRetry && (dirs == null))
                {
                    keepRetry = false;
                    try
                    {
                        IEnumerable<DirectoryInfo> enDirs = dirParsed.GetDirectories().Where(
                          d => (d.Attributes & FileAttributes.ReparsePoint) == 0);
                        if (FoldersComparer != null)
                            enDirs = enDirs.OrderBy(di => di, FoldersComparer);
                        dirs = enDirs.ToArray();
                    }
                    catch (SystemException ex)
                    {
                        OnParseError(dirParsed, nParseLevel, ex, out keepRetry);
                    }
                }
            }
            if (IsCanceled)
                yield break; // specifies that an iterator has come to an end

            // 3. Add found sub-folders
            //
            if (dirs != null)
            {
                // Reverse the order of pushed items to make the left items popped first. 
                // That way, tree traversal will keep "pre-order" (root, left, right) order.
                dirStack.PushRange(dirs.Reverse().Select(
                  childDir => (DirInfo: childDir, Level: nParseLevel + 1)));
            }

            // 4. -- Leaving the folder
            //
            if (!OnFolderExit(dirParsed, nParseLevel))
                yield break; // specifies that an iterator has come to an end
        }
    }
    #endregion // Public Methods
    #endregion // Methods
}
#pragma warning restore IDE0305
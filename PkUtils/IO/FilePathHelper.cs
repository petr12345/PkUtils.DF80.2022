/***************************************************************************************************************
*
* FILE NAME:   .\IO\FilePathHelper.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The class FilePathHelper, contains file-system related utilities.
*
**************************************************************************************************************/

// Ignore Spelling: Utils, Subfolder, Stackoverflow
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using PK.PkUtils.Comparers;
using PK.PkUtils.Extensions;
using PK.PkUtils.Reflection;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.IO;

#pragma warning disable IDE0090 // Use 'new(...)'
#pragma warning disable IDE0057 // Use range operator
#pragma warning disable IDE0305 // Collection initialization can be simplified

/// <summary> <para>
/// Provides some helper methods related to file-system objects paths, or IO-related functionality.
/// </para>
/// 
/// <para>
/// A <b>clarification of terminology</b> will be beneficial here.   In general, a path is a way to
/// something. In this context, a path can point to any object on the file system, including files
/// but not limited to files. <br/>
/// Following are special cases of path:
/// <list type="number">
/// <item><b>/foo/bar/file.txt </b><br/>Absolute path</item>
/// <item><b>/foo/bar</b><br/>An absolute path to a directory</item>
/// <item><b>../foo </b><br/>A relative path to a directory, from current directory</item>
/// <item><b>./file.txt </b><br/>A relative path to a file, from current directory</item>
/// <item><b>file.txt </b><br/>A relative path too</item>
/// </list>
/// </para> </summary>
///
/// <remarks> The source of this used terminology is Stackoverflow
/// <see href="http://stackoverflow.com/questions/2119156/name-of-a-path-containing-the-complete-file-name">
/// Name of a path containing the complete file name?</see> </remarks>
[CLSCompliant(true)]
public static class FilePathHelper
{
    #region Fields
    /// <summary> A backing field for <see cref="FileSystemItemEqualityComparer"/> property. </summary>
    private static IEqualityComparer<FileSystemInfo> _fileSystemItemEqualityComparer;

    /// <summary> A backing field for <see cref="FileSystemItemComparer"/> property. </summary>
    private static IComparer<FileSystemInfo> _fileSystemItemComparer;

    /// <summary>
    /// A backing field of property <see cref="DirectorySeparators"/>
    /// </summary>
    private static readonly char[] _directorySeparators = [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar];

    /// <summary> 'Wild' characters. </summary>
    private static readonly char[] _WildChars = "*?".ToCharArray();
    private static readonly char[] _ExtraInvalidChars = "<>".ToCharArray();

    private static readonly char[] _PathInvalidCharacters =
        Path.GetInvalidPathChars().Concat(_ExtraInvalidChars).Distinct().ToArray();

    /// <summary>
    /// An union of Path.GetInvalidPathChars() and _WillChars ('*' and '?').
    /// </summary>
    private static readonly char[] _PathInvalidOrWildCharacters =
        _PathInvalidCharacters.Concat(_WildChars).Distinct().ToArray();

    private const string _PrefixCurDirBackSlash = @".\";
    private const string _PrefixCurDirForwSlash = @"./";
    private const string _PrefixUpperDirBackSlash = @"..\";
    private const string _PrefixUpperDirForwSlash = @"../";
    #endregion // Fields

    #region Properties

    /// <summary>
    ///  Returns a collection of characters { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
    /// </summary>
    public static IEnumerable<char> DirectorySeparators
    {
        get { return _directorySeparators; }
    }

    /// <summary>
    /// Returns an array containing characters that are invalid for use in path names, including those returned
    /// by Path.GetInvalidPathChars(), as well as "&lt;&gt;" (which are also invalid directory characters).
    /// </summary>
    /// <value> The path invalid characters. </value>
    public static IEnumerable<char> PathInvalidCharacters
    {
        get { return _PathInvalidCharacters; }
    }

    /// <summary>
    /// Returns an union of Path.GetInvalidPathChars() and _WillChars ('*' and '?').
    /// </summary>
    public static IEnumerable<char> PathInvalidOrWildCharacters
    {
        get { return _PathInvalidOrWildCharacters; }
    }

    /// <summary>
    /// Returns instance of <![CDATA[IEqualityComparer<string>]]>, which can be used to compare two paths
    /// by delegating to StringComparer.OrdinalIgnoreCase
    /// </summary>
    public static IEqualityComparer<string> FileSystemItemPathEqualityComparer
    {
        get { return StringComparer.OrdinalIgnoreCase; }
    }

    /// <summary>
    /// Returns instance of  <![CDATA[IEqualityComparer<FileSystemInfo>]]>, which compares two FileSystemInfo 
    /// items by comparison of their path with StringComparison.OrdinalIgnoreCase.
    /// </summary>
    public static IEqualityComparer<FileSystemInfo> FileSystemItemEqualityComparer
    {
        get
        {
            if (_fileSystemItemEqualityComparer == null)
            {
                bool FsComparer(FileSystemInfo x, FileSystemInfo y)
                    => string.Equals(SafeGetFullName(x), SafeGetFullName(y), StringComparison.OrdinalIgnoreCase);

                int FsHash(FileSystemInfo x) => SafeGetFullName(x).ToUpperInvariant().GetHashCode();

                _fileSystemItemEqualityComparer = FunctionalEqualityComparer.CreateNullSafeComparer<FileSystemInfo>(
                  FsComparer, FsHash);
            }
            return _fileSystemItemEqualityComparer;
        }
    }

    /// <summary>
    /// Returns instance of <![CDATA[IComparer<string>]]>, which can be used to compare two paths
    /// by delegating to StringComparer.OrdinalIgnoreCase
    /// </summary>
    public static IComparer<string> FileSystemItemPathComparer
    {
        get { return StringComparer.OrdinalIgnoreCase; }
    }

    /// <summary>
    /// Returns instance of <![CDATA[IComparer<FileSystemInfo>]]>, which compares two FileSystemInfo
    /// items by comparison of their path with StringComparison.OrdinalIgnoreCase.
    /// </summary>
    public static IComparer<FileSystemInfo> FileSystemItemComparer
    {
        get
        {
            static int fsComparer(FileSystemInfo x, FileSystemInfo y)
            {
                return string.Compare(SafeGetFullName(x), SafeGetFullName(y), StringComparison.OrdinalIgnoreCase);
            }

            return _fileSystemItemComparer ??= FunctionalComparer.CreateNullSafeComparer<FileSystemInfo>(fsComparer);
        }
    }
    #endregion // Properties

    #region Methods

    #region Public Methods

    /// <summary>
    /// Determines if given path contains wildcards.
    /// </summary>
    /// 
    /// <remarks>The method is intended for file-system paths, and has no specific handling for UNC paths, 
    /// like "\\?\C:\user\docs\Letter.txt". Hence you should not call it for such paths. <br/>
    /// 
    /// The method accepts input path with length longer than or equal to Win32.MAX_PATH.
    /// </remarks>
    /// 
    /// <param name="path">Absolute or relative path</param>
    /// <returns>true if the argument <paramref name="path"/>contains any of '*' '?' characters returns true; 
    /// otherwise false.</returns>
    public static bool HasWildCard(string path)
    {
        return ((!string.IsNullOrEmpty(path)) && (0 <= path.IndexOfAny(_WildChars)));
    }

    /// <summary>
    /// Appends path separator to the path, if the path is not terminated by that character. <br/>
    /// It does NOT append separator in case the path is actually just drive specification, like "D:" 
    /// ( in this case the semantics of the path is 'current directory for D:', not the root D:\ ).
    /// </summary>
    /// <remarks>
    /// The method accepts input path with length longer than or equal to Win32.MAX_PATH.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> Passed when the supplied argument <paramref name="path"/> 
    /// is null. </exception>
    /// <param name="path">Given path</param>
    /// <returns>Modified path</returns>
    /// <seealso cref="TrimEndingDirectorySeparator"/>
    public static string AppendPathSeparator(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        int nLen;
        string result = path;

        if ((nLen = path.Length) > 0)
        {
            char ch = path[nLen - 1];

            if ((ch != Path.DirectorySeparatorChar) &&
              (ch != Path.AltDirectorySeparatorChar) &&
              (ch != Path.VolumeSeparatorChar))
            {
                result = path + Path.DirectorySeparatorChar;
            }
        }

        return result;
    }

    /// <summary>
    /// Removes trailing path separator from the given path ( the input argument ), 
    /// if it is terminated by that character, and if the path is not a root directory ( like "D:\" ) <br/>
    /// Examples: <br/>
    /// string "D:\Tools\FAR\" is changed to "D:\Tools\FAR" <br/>
    /// string "D:\Tools" in not changed  <br/>
    /// string "D:\" is not changed  <br/>
    /// string "\" is not changed  <br/>
    /// </summary>
    /// <remarks>
    /// The method accepts input path with length longer than or equal to Win32.MAX_PATH.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> Passed when the supplied argument <paramref name="path"/> 
    /// is null. </exception>
    /// <param name="path">Given path</param>
    /// <returns>Modified path</returns>
    /// <seealso cref="AppendPathSeparator"/>
    public static string TrimEndingDirectorySeparator(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        return Path.TrimEndingDirectorySeparator(path);
    }

    /// <summary> Trims-out single double-quotes character '"' from the beginning and from end of the path,
    /// if the path contains more than one character, and its first and last character equal to it. <br/>
    /// 
    /// If the argument <paramref name="trimSpaces"/> is true, the method trims from the input argument
    /// all leading and trailing occurrences of the space character ' ', before trying to remove double quotes.
    /// Anyway, the eventual result will modify the input argument ONLY if double-quotes characters were found 
    /// after space character trimming.
    /// </summary>
    /// <remarks>
    /// The input path may be an absolute or relative path. Actually no path existence or validity is checked,
    /// the path is treated just as a string. <br/>
    /// 
    /// The method accepts input path with length longer than or equal to Win32.MAX_PATH.
    /// </remarks>
    /// 
    /// <param name="path"> The input path. If it is null or empty, the result is exactly the same.</param>
    /// <param name="trimSpaces"> (Optional) true to trim leading and trailing spaces. </param>
    /// <returns> A string. </returns>
    public static string TrimPathDoubleQuotes(string path, bool trimSpaces = true)
    {
        string strRes = path;

        if (!string.IsNullOrEmpty(path))
        {
            string strTemp = trimSpaces ? path.Trim(' ') : path;
            int nLastIndex = strTemp.Length - 1;

            if ((nLastIndex > 0) && (strTemp[0] == '"') && (strTemp[nLastIndex] == '"'))
                strRes = strTemp.Substring(1, nLastIndex - 1);
        }
        return strRes;
    }

    /// <summary> Query if 'path' is rooted, and is not a subdirectory of the root. </summary>
    /// <remarks> For consistency with Path.IsPathRooted, for case of null <paramref name="path"/> argument 
    /// the method just returns false and does not throw an exception. <br/>
    /// 
    /// If the path consists of just a drive letter and colon (:), like @"d:", it does NOT return true,
    /// unless the actual value of argument <paramref name="acceptDriveLetterColon "/> is true.
    /// This is because in command shell, "d:" will switch to current directory of drive d:, not to the root.
    /// In this aspect, the behaviour is different from Path.IsPathRooted, which just returns true for such path.<br/>
    /// 
    /// The method accepts input path with length longer than or equal to Win32.MAX_PATH.
    /// </remarks>
    ///
    /// <param name="path"> Absolute or relative path. </param>
    /// <param name="acceptDriveLetterColon"> Indicates whether the method should accept a string consisting
    /// of just drive letter and colon, like "d:", as a rooted path. By default, value of this argument is false. 
    /// </param>
    ///
    /// <returns> True if path rooted only, false if not. </returns>
    /// 
    /// <seealso cref="FilePathHelper.IsPathRooted"/>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/system.io.path.ispathrooted(v=vs.110).aspx">
    /// Path.IsPathRooted Method
    /// </seealso>
    public static bool IsPathRootOnly(string path, bool acceptDriveLetterColon = false)
    {
        int nLastIndex;
        char chLast;
        bool bRes = false;

        if ((path != null) && (0 <= (nLastIndex = path.Length - 1)))
        {
            if (IsDirectorySeparator(chLast = path[nLastIndex]))
            {
                if (nLastIndex == 0)
                    bRes = true;
                else if ((nLastIndex == 2) && (path[1] == Path.VolumeSeparatorChar) && char.IsLetter(path[0]))
                    bRes = true;
            }
            else if ((chLast == Path.VolumeSeparatorChar) && (nLastIndex == 1) && char.IsLetter(path[0]))
            {
                bRes = acceptDriveLetterColon;
            }
        }

        return bRes;
    }

    /// <summary> Gets a value indicating whether the specified path string contains a root. </summary>
    /// <remarks>
    /// This method is an analogy to
    /// <see href="https://msdn.microsoft.com/en-us/library/system.io.path.ispathrooted%28v=vs.110%29.aspx">
    /// Path.IsPathRooted</see> method. <br/>
    /// However it does not throw an exception if path contains one or more of the invalid characters,
    /// it returns simply false in that case. 
    /// Invalid set of characters in this context include wildcard characters "*?" <br/>.
    /// 
    /// If the path consists of just a drive letter and colon (:), like @"d:", it does NOT return true,
    /// unless the actual value of argument <paramref name="acceptDriveLetterColon "/> is true.
    /// This is because in command shell, "d:" will switch to current directory of drive d:, not to the root.
    /// In this aspect, the behaviour is different from Path.IsPathRooted, which just returns true for such path.<br/>
    /// 
    /// The method accepts input path with length longer than or equal to Win32.MAX_PATH.<br/>
    /// </remarks>
    /// 
    /// <param name="path"> The path to be tested. </param>
    /// <param name="acceptDriveLetterColon"> Indicates whether the method should accept a string consisting
    /// of just drive letter and colon, like "d:", as a rooted path. By default, value of this argument is false. 
    /// </param>
    /// 
    /// <returns>
    /// True if path rooted, false if not. <br/>
    /// In more details, the IsPathRooted method returns true if the first character is a directory
    /// separator character such as "\", or if the path starts with a drive letter, colon (:) and directory
    /// separator. For example, it returns true for path strings such as "\\MyDir\\MyFile.txt", "C:\\MyDir",
    /// or "C:\MyDir". It returns false for path strings such as "MyDir" or "c:MyDir".
    /// </returns>
    /// 
    /// <seealso cref="IsPathRootOnly"/>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/system.io.path.ispathrooted(v=vs.110).aspx">
    /// Path.IsPathRooted Method
    /// </seealso>
    public static bool IsPathRooted(string path, bool acceptDriveLetterColon = false)
    {
        bool bRes = false;

        if ((path != null) && (0 > path.IndexOfAny(_PathInvalidOrWildCharacters)))
        {
            switch (path.Length)
            {
                case 0:
                    break;
                case 1:
                    bRes = IsDirectorySeparator(path[0]);
                    break;
                case 2:
                    if (IsDirectorySeparator(path[0]))
                        bRes = true;
                    else if (char.IsLetter(path[0]) && (path[1] == Path.VolumeSeparatorChar))
                        bRes = acceptDriveLetterColon;
                    break;
                default:
                    if (path[1] == Path.VolumeSeparatorChar)
                        bRes = char.IsLetter(path[0]) && IsDirectorySeparator(path[2]);
                    else
                        bRes = IsPathRooted(path.Substring(0, 2));
                    break;
            }
        }

        return bRes;
    }

    /// <summary>
    /// Checks if provided <paramref name="path"/> could be a valid folder string. <br/>
    /// Throws <see cref="ArgumentNullException"/> in case the supplied string is null. <br/>
    /// Throws <see cref="ArgumentException"/> if the supplied string is empty, or contains any of
    /// <see cref="Path.GetInvalidPathChars()"/>. <br/>
    /// 
    /// Note that unlike similar method <see cref="CheckArgIsValidPath(string, string)"/>, this one permits '*'
    /// and  '?' characters presence.
    /// </summary>
    ///
    /// <remarks>
    /// Actually no path existence in the underlying file system is checked, the provided path is treated just as
    /// a string.<br/>
    /// The method accepts input path with length longer than or equal to Win32.MAX_PATH.<br/>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentException"> Thrown when a supplied <paramref name="path"/> could not
    /// represent a valid folder path. </exception>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="path"/> is null. </exception>
    ///
    /// <param name="path"> Absolute or relative path. </param>
    /// <param name="argName"> Name of argument, to be used in constructed error message. </param>
    /// <seealso cref="CheckArgIsValidPath"/>

    public static void CheckIsValidFolderString(string path, string argName = null)
    {
        DoCheckArgValidPath(path, argName, Path.GetInvalidPathChars());
    }

    /// <summary>
    /// Checks if provided <paramref name="path"/> is a legal path in file system, 
    /// not containing invalid characters. <br/>
    /// Throws <see cref="ArgumentNullException"/> in case the supplied string is null. <br/>
    /// Throws <see cref="ArgumentException"/> if the supplied string is empty, consists only of white-space
    /// characters, or contains any of <see cref="Path.GetInvalidPathChars()"/>. <br/>
    /// 
    /// Note that unlike similar method <see cref="CheckIsValidFolderString(string, string)"/>,
    /// this one prohibits '*' and  '?' characters.
    /// </summary>
    /// 
    /// <remarks>
    /// Actually no path existence in the underlying file system is checked,
    /// the provided path is treated just as a string.<br/>
    /// 
    /// The method accepts input path with length longer than or equal to Win32.MAX_PATH.<br/>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentException"> Thrown when a supplied <paramref name="path"/> could not represent
    /// a valid folder path. </exception>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="path"/> is null. </exception>
    /// 
    /// <param name="path"> Absolute or relative path. </param>
    /// <param name="argName"> Name of argument, to be used in constructed error message. </param>
    /// <seealso cref="CheckIsValidFolderString"/>
    public static void CheckArgIsValidPath(this string path, string argName = null)
    {
        DoCheckArgValidPath(path, argName, PathInvalidOrWildCharacters.ToArray());
    }

    /// <summary>
    /// Returns the path 'one directory above' ( one step closer to the root ) for the given input directory.
    /// </summary>
    /// <remarks>
    /// The method accepts input path with length longer than or equal to Win32.MAX_PATH.<br/>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// <param name="path"> The original path. </param>
    /// <param name="allowEmpty"> Indicates whether returning an empty string is valid result. <br/>
    ///  If there is no more directories above given one, and allowEmpty = true, will return an empty string.<br/>
    ///  If there is no more directories above given one, and allowEmpty = false, will return  the original
    ///  directory.<br/> </param>
    /// <returns> The upper directory. </returns>
    public static string GetUpperFolder(string path, bool allowEmpty)
    {
        ArgumentNullException.ThrowIfNull(path);

        string lastFound = string.Empty;
        string strOrig = TrimEndingDirectorySeparator(path);
        string strRight = strOrig;

        for (bool bContinueSearching = true; bContinueSearching;)
        {
            string strTmp, strToTest;
            int nInd = strRight.IndexOfAny(_directorySeparators);

            if (bContinueSearching = (nInd >= 0))
            {
                strTmp = strRight.Substring(0, ++nInd);
                strRight = strRight.Substring(nInd, strRight.Length - nInd);
            }
            else
            {
                strTmp = strRight;
                strRight = string.Empty;
            }
            strToTest = lastFound + strTmp;

            if (strToTest.Equals(strOrig, StringComparison.Ordinal))
            {
                bContinueSearching = false;
            }
            else
            {
                lastFound = strToTest;
            }
        }

        if (string.IsNullOrEmpty(lastFound) && !allowEmpty)
        {
            lastFound = strOrig;
        }

        return lastFound;
    }

    /// <summary>
    /// Returns the path 'one directory above' ( one step closer to the root ) for the given input directory.
    /// </summary>
    /// <remarks> Calls the overloaded method <see cref="GetUpperFolder(string, bool)"/> with the actual 
    /// argument allowEmpty equal to false.<br/>
    /// The method accepts input path with length longer than or equal to Win32.MAX_PATH.<br/>
    /// </remarks>
    /// 
    /// <param name="strPath">    The original path. </param>
    /// <returns> The upper directory. </returns>
    public static string GetUpperFolder(string strPath)
    {
        return GetUpperFolder(strPath, false);
    }

    /// <summary>
    /// Returns an absolute path derived from given relative path,
    /// assuming the relative path relates to the absolute path 'application directory'
    /// ( the directory containing the file of Assembly.GetEntryAssembly).
    /// </summary>
    /// <param name="relativePath">relative path</param>
    /// <returns>Resulting  absolute path</returns>
    public static string GenerateAbsolute(string relativePath)
    {
        // The method is needed because of behavior of Path.GetFullPath in NETCF 
        // ( does not work as needed on that platform).
        // See more details in comments inside CombineAbsolute
        return CombineAbsolute(Path.GetDirectoryName(GetApplicationFileName()), relativePath);
    }

    /// <summary> Returns an absolute path (with or without a filename) created from given relative
    /// path (with or without a filename), assuming the relative path <paramref name="relativePath"/>
    /// relates to the absolute path <paramref name="basePath"/>. <br/>
    /// In case the argument <paramref name="basePath "/>is null or empty string, it is assumed the
    /// <paramref name="relativePath"/> should be relative to the application directory. </summary>
    ///
    /// <remarks> If the actual value of <paramref name="relativePath"/> is  actually an absolute path,
    /// the same value is returned by this method, and the argument  <paramref name="basePath"/> is just
    /// ignored. </remarks>
    ///
    /// <param name="basePath">     A base path ( either an absolute path or null). </param>
    /// <param name="relativePath"> A relative path. </param>
    ///
    /// <returns> Resulting  input arguments combination - an absolute path. </returns>
    public static string CombineAbsolute(string basePath, string relativePath)
    {
        // The method is needed because of behavior of Path.GetFullPath in NETCF 
        // ( see below ).
        string strResult = relativePath;

        if ((!string.IsNullOrEmpty(relativePath)) && !Path.IsPathRooted(relativePath))
        {
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = Path.GetDirectoryName(GetApplicationFileName());
            }
            else if (!Path.IsPathRooted(basePath))
            {
                basePath = GenerateAbsolute(basePath);
            }

            // Note: Path.GetFullPath cannot be used, because of following:
            // i/ On desktop, the output of Path.GetFullPath is based on your current directory
            // ii/ On NETCF however, Path.GetFullPath just changes the beginning of the path 
            // ( it replaces '.\' by '\') to make the path absolute instead of relative ).
            // This is most likely because of the fact WinCE has no concept of current directory.
            /*
            try
            {
              strResult = Path.GetFullPath(relativePath);
            }
            catch (IOException)
            {
              strResult = string.Empty;
            }
            */

            string strPrevVal, strTmp;
            int nCountUpDirs = 0;

            // adjust the relativePath
            do
            {
                strPrevVal = relativePath;
                // get rid of 'current directory' prefix
                while (relativePath.StartsWith(_PrefixCurDirBackSlash, StringComparison.Ordinal))
                {
                    relativePath = relativePath.Remove(0, _PrefixCurDirBackSlash.Length);
                }
                while (relativePath.StartsWith(_PrefixCurDirForwSlash, StringComparison.Ordinal))
                {
                    relativePath = relativePath.Remove(0, _PrefixCurDirForwSlash.Length);
                }
                // get rid of ".."
                while (relativePath.StartsWith(_PrefixUpperDirBackSlash, StringComparison.Ordinal))
                {
                    nCountUpDirs++;
                    relativePath = relativePath.Remove(0, _PrefixUpperDirBackSlash.Length);
                }
                while (relativePath.StartsWith(_PrefixUpperDirForwSlash, StringComparison.Ordinal))
                {
                    nCountUpDirs++;
                    relativePath = relativePath.Remove(0, _PrefixUpperDirForwSlash.Length);
                }
            } while (!strPrevVal.Equals(relativePath, StringComparison.Ordinal));

            // split the string relativePathName to sub-directories
            string[] arrPathParts = relativePath.Split(_directorySeparators);

            // adjust strBasedOn - go nCountUpDirs up
            for (int ii = 0; ii < nCountUpDirs; ii++)
            {
                if (!basePath.Equals(strTmp = GetUpperFolder(basePath), StringComparison.Ordinal))
                    basePath = strTmp;
                else
                    break;
            }
            basePath = AppendPathSeparator(basePath);

            // combine all back again
            strResult = string.Concat(basePath, string.Join(Path.DirectorySeparatorChar.ToString(), arrPathParts));
        }

        return strResult;
    }

    /// <summary>
    /// Query if <paramref name="childPath"/> is a child folder of <paramref name="parentPath"/>.
    /// </summary>
    /// <remarks>
    /// The method accepts input argument <paramref name="parentPath"/> and <paramref name="childPath"/>
    /// path with length longer than or equal to Win32.MAX_PATH.<br/>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentException"> Thrown when one of supplied string arguments could not represent 
    ///  a valid folder path. </exception>
    /// <exception cref="ArgumentNullException"> Thrown when one of supplied string arguments is null. </exception>
    /// 
    /// <param name="parentPath"> Pathname of the folder who is supposed to be a parent folder. </param>
    /// <param name="childPath"> Pathname of the folder who is supposed to be a child folder. </param>
    /// <param name="allowEqual"> In case both string arguments represent he same folder, the result will be value
    ///  of this <paramref name="allowEqual"/> argument. </param>
    /// <returns>
    /// true if <paramref name="childPath"/> is a child folder of <paramref name="parentPath"/>, otherwise false.
    /// </returns>
    public static bool IsSubfolder(string parentPath, string childPath, bool allowEqual)
    {
        string strMainFolder = NormalizeFolderPath(parentPath);
        string strSubFolder = NormalizeFolderPath(childPath);
        bool bRes = false;

        if (strSubFolder.StartsWith(strMainFolder, StringComparison.OrdinalIgnoreCase))
        {
            if (strMainFolder.Length == strSubFolder.Length)
                bRes = allowEqual;
            else
                bRes = true;
        }

        return bRes;
    }

    /// <summary> This method is opposite to Path.Combine. Assuming that <paramref name="parentPath"/> and
    /// <paramref name="childPath"/> are valid rooted paths, and childPath is a sub-folder of parentPath,
    /// the method returns relative path leading from parentPath to childPath. <br/>
    /// 
    /// For instance, if parentPath	is "C:\Tmp3" and childPath is "C:\Tmp3\xyz\CtrlSourceCode",
    /// the result is "xyz\CtrlSourceCode". 
    /// Note the result dos NOT contain a dot ( '.' ) in the beginning. <br/>
    /// 
    /// In case <paramref name="childPath"/>is not a sub-folder of <paramref name="parentPath"/>,
    /// the result will be null.
    /// </summary>
    /// 
    /// <remarks>
    /// The method accepts input argument <paramref name="parentPath"/> and <paramref name="childPath"/>
    /// path with length longer than or equal to Win32.MAX_PATH.<br/>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentException"> Thrown when one of supplied string arguments could not represent 
    ///  a valid folder path. </exception>
    /// <exception cref="ArgumentNullException"> Thrown when one of supplied string arguments is null. </exception>
    /// 
    /// <param name="parentPath"> Pathname of the folder who is supposed to be a parent folder. </param>
    /// <param name="childPath"> The path either of the folder which is supposed to be a child folder,
    ///  or of the file which is supposed to be located below the <paramref name="parentPath"/>. </param>
    /// <returns> The found relative path, or null. </returns>
    public static string GetRelativePath(string parentPath, string childPath)
    {
        string strMainFolder = NormalizeFolderPath(parentPath);
        string strSubFolder = NormalizeFolderPath(childPath);
        string strRelativePath = null;

        if (strSubFolder.StartsWith(strMainFolder, StringComparison.OrdinalIgnoreCase))
        {
            strRelativePath = strSubFolder.Substring(strMainFolder.Length);
            strRelativePath = strRelativePath.Trim(_directorySeparators);
        }
        return strRelativePath;
    }

    /// <summary> Combines two strings into a path. </summary>
    /// <remarks>
    /// This method is an analogy to
    /// <see href="https://msdn.microsoft.com/en-us/library/system.io.path.combine(v=vs.110).aspx">
    /// Path.Combine</see> method. <br/>
    /// However it does not throw an exception if path1 or path2 contains one or more of invalid characters.
    /// 
    /// The method accepts input argument <paramref name="path1"/> and <paramref name="path1"/>
    /// path with length longer than or equal to Win32.MAX_PATH.<br/>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> Passed when any of required arguments is null. </exception>
    /// <param name="path1"> The first path to combine. </param>
    /// <param name="path2"> The second path to combine. </param>
    /// <returns>
    /// The combined paths. If one of the specified paths is a zero-length string, this method returns the
    /// other path. If path2 contains an absolute path, this method returns path2.
    /// </returns>
    public static string CombineNoChecks(string path1, string path2)
    {
        ArgumentNullException.ThrowIfNull(path1);
        ArgumentNullException.ThrowIfNull(path2);

        if (path2.Length == 0)
        {
            return path1;
        }
        if (path1.Length == 0)
        {
            return path2;
        }
        if (IsPathRooted(path2))
        {
            return path2;
        }
        return AppendPathSeparator(path1) + path2;
    }

    /// <summary>
    /// Returns the directory information for the specified path string. Works as a substitution of
    /// <see href="https://msdn.microsoft.com/en-us/library/system.io.path.getdirectoryname%28v=vs.110%29.aspx">
    /// Path.GetDirectoryName</see> method, which throws <see cref="PathTooLongException"/>
    /// if the path parameter is longer than the 259 characters.
    /// </summary>
    /// <remarks>
    /// The method accepts input path with length longer than or equal to Win32.MAX_PATH.<br/>
    /// </remarks>
    /// 
    /// <exception cref="ArgumentException"> Thrown when the <paramref name="path "/>parameter contains 
    /// invalid characters, is empty, or contains only white spaces. </exception>
    /// <param name="path"> Given path of a file or directory. </param>
    /// <returns>
    /// Directory information for the path, or null if path denotes a root directory or is null. 
    /// Returns null if path does not contain directory information.
    /// ( In these aspects, the method is consistent with Path.GetDirectoryName ).
    /// </returns>
    public static string LongPathTolerantGetDirectoryName(string path)
    {
        string result;

        if (string.IsNullOrEmpty(path))
            throw new ArgumentException("Path can't be null or empty string", nameof(path));
        else if (path.Trim(' ').Length == 0)
            throw new ArgumentException("Path can't be just whitespace string", nameof(path));
        else if (0 <= path.IndexOfAny(_PathInvalidCharacters))
            throw new ArgumentException("Path can't contain invalid characters", nameof(path));

        try
        {
            result = Path.GetDirectoryName(path);
        }
        catch (PathTooLongException)
        {
            result = DoGetDirectoryName(path);
        }

        return result;
    }

    /// <summary> Extracts each individual folder name from a path. </summary>
    /// <remarks>
    /// The method accepts input path with length longer than or equal to Win32.MAX_PATH.<br/>
    /// </remarks>
    /// <param name="strPath"> Given path. </param>
    /// <returns> The path parts. </returns>
    public static IReadOnlyList<string> GetPathParts(string strPath)
    {
        List<string> result;

        if (string.IsNullOrEmpty(strPath))
            result = [];
        else
            result = [.. strPath.Split(_directorySeparators, StringSplitOptions.RemoveEmptyEntries)];

        return result;
    }

    /// <summary> 
    /// Compacts a string representing the path, based on the number of characters you wish to restrict it to. 
    /// Note that this does not take into account font and how wide the string will appear on a screen.
    /// </summary>
    /// <remarks>
    /// The method accepts input path with length longer than or equal to Win32.MAX_PATH.<br/>
    /// </remarks>
    ///
    /// <exception cref="ArgumentException"> Thrown when the supplied argument <paramref name="maxLength"/>
    /// has negative or zero value. </exception>
    ///
    /// <param name="path">   The given path to be compacted. </param>
    /// <param name="maxLength"> The maximum length of resulting string in characters. </param>
    ///
    /// <returns> The compacted path. </returns>
    /// <remarks>
    /// In case the input argument is null or empty string, the resulting is just he same value.
    /// </remarks>
    public static string PathCompact(string path, int maxLength)
    {
        string result = path;

        if (maxLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, "The value of this argument must be positive");

        if (!string.IsNullOrEmpty(path) && (path.Length > maxLength))
        {
            int cchMax = maxLength + 1;
            StringBuilder sbOut = new StringBuilder(cchMax);

            if (Win32.PathCompactPathEx(sbOut, path, cchMax, 0))
            {
                result = sbOut.ToString();
            }
        }
        return result;
    }

    /// <summary>
    /// Generates FileInfo object for file defined by strPathName.
    /// If the file does not exist, then returns null.	
    /// </summary>
    /// <remarks>
    /// The method is useful for case of an invalid argument, or for case
    /// the FileInfo may raise an exception for non-existing file, invalid argument or unauthorized access. <br/>
    /// 
    /// Note: Useful also on NETCF, since on that platform (unlike on desktop NET) 
    /// the FileInfo raises an exception for non-existing file. <br/>
    /// 
    /// The method accepts input path with length longer than or equal to Win32.MAX_PATH.<br/>
    /// </remarks>
    /// 
    /// <param name="strPath">strPathName to the file</param>
    /// <param name="bMakeNullOnNotExisting">do check for existence of file ?</param>
    /// <returns>FileInfo object or null</returns> 
    public static FileInfo SafeGetFileInfo(string strPath, bool bMakeNullOnNotExisting = true)
    {
        FileInfo result = null;
        try
        {
            result = new FileInfo(strPath);
        }
        catch (IOException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (System.Security.SecurityException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (NotSupportedException)
        {
        }

        if (bMakeNullOnNotExisting && (null != result) && !result.Exists)
        {
            result = null;
        }
        return result;
    }

    /// <summary>
    /// Generates DirectoryInfo object for a given input argument - the path. If the specified path does
    /// not exist, returns null.
    /// </summary>
    /// 
    /// <remarks>
    /// The method is useful for case of invalid argument ( when the constructor of DirectoryInfo raises
    /// ArgumentException), for case of IOException, SecurityException etc. <br/>
    /// 
    /// The method accepts input path with length longer than or equal to Win32.MAX_PATH.<br/>
    /// </remarks>
    /// 
    /// <param name="path"> The path. </param>
    /// <param name="bMakeNullOnNotExisting"> do check for existence ? </param>
    /// <returns> DirectoryInfo object or null. </returns>
    public static DirectoryInfo SafeGetDirectoryInfo(string path, bool bMakeNullOnNotExisting = true)
    {
        DirectoryInfo result = null;
        try
        {
            result = new DirectoryInfo(path);
        }
        catch (IOException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (System.Security.SecurityException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (NotSupportedException)
        {
        }

        if (bMakeNullOnNotExisting && (null != result) && !result.Exists)
        {
            result = null;
        }
        return result;
    }

    /// <summary>
    /// Gets the full path of the directory or file, calling the FileSystemInfo.FullName property,
    /// but providing a work-around in case that property throws either <see cref="PathTooLongException"/>
    /// or <see cref="System.Security.SecurityException"/>. (In that case, the direct contents of 
    /// protected field FileSystemInfo.FullPath is retrieved instead.) </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Passed when a supplied <paramref name="fs"/> is null. </exception>
    /// 
    /// <param name="fs"> The examined object representing file system item (either a file or a directory). </param>
    /// <returns> A string consisting of the full path of <paramref name="fs"/>. </returns>
    /// 
    /// <seealso href="https://msdn.microsoft.com/en-us/library/system.io.filesysteminfo.fullname(v=vs.110).aspx">
    /// FileSystemInfo.FullName property </seealso>
    public static string SafeGetFullName(FileSystemInfo fs)
    {
        ArgumentNullException.ThrowIfNull(fs);

        string result = null;

        try
        {
            result = fs.FullName;
        }
        catch (PathTooLongException)
        {
        }
        catch (System.Security.SecurityException)
        {
        }
        result ??= FieldsUtils.GetInstanceFieldValueEx<string>(fs, "FullPath");

        return result;
    }

    /// <summary>
    /// Searches for the longest existing directory path prefix of the specified input path.
    /// </summary>
    /// <param name="inputPath">The full input path (can point to a non-existent file or directory).</param>
    /// <returns>
    /// The longest existing directory path that is a prefix of the input path.
    /// Returns <c>null</c> if no part of the input path corresponds to an existing directory.
    /// </returns>
    /// <remarks>
    /// This method trims the input path iteratively until it finds an existing directory,
    /// using <c>SafeGetDirectoryInfo</c> to ensure exception safety.
    /// </remarks>
    public static string GetLongestExistingDirectory(string inputPath)
    {
        if (string.IsNullOrWhiteSpace(inputPath))
            return null;
        try
        {
            for (string currentPath = inputPath.Trim(); !string.IsNullOrEmpty(currentPath);)
            {
                DirectoryInfo dir = SafeGetDirectoryInfo(currentPath);
                if (dir != null && dir.Exists)
                    return dir.FullName;
                else
                    currentPath = Path.GetDirectoryName(currentPath);
            }
        }
        catch { }  // Intentionally suppressed — the method is designed to be fail-safe

        return null;
    }

    /// <summary> For the given exception <paramref name="ex"/> and full (rooted) path either to file-system 
    /// item (either a file or folder), returns the exception message with following modification: <br/>
    /// If the exception message contains full path of <paramref name="fullPath"/> in quotes,
    /// the return modified message has that replaced by just the name of it in quotes. </summary>
    ///
    /// <param name="ex"> The exception being examined. </param>
    /// <param name="fullPath"> The full path to file system item (either a file or a directory). </param>
    ///
    /// <returns> A string containing either the original or modified exception message. </returns>
    public static string ShortenedExceptionMessage(Exception ex, string fullPath)
    {
        ArgumentNullException.ThrowIfNull(ex);
        ArgumentNullException.ThrowIfNull(fullPath);

        if (!Path.IsPathRooted(fullPath))
        {
            throw new ArgumentException("The argument is not a full path", nameof(fullPath));
        }

        string nameOnly = Path.GetFileName(fullPath);
        string strFullNameInQuotes = $"'{fullPath}'";
        string strNameInQuotes = $"'{nameOnly}'";
        string strExMsg = ex.Message.Replace(strFullNameInQuotes, strNameInQuotes, StringComparison.InvariantCulture);

        return strExMsg;
    }

    /// <summary> For the given exception <paramref name="ex"/> and file-system item <paramref name="fs"/>
    /// returns the exception message with following modification: <br/>
    /// If the exception message contains full path of <paramref name="fs"/> in quotes,
    /// the return modified message has that replaced by just the name of it in quotes. </summary>
    ///
    /// <param name="ex"> The exception being examined. </param>
    /// <param name="fs"> The object representing file system item (either a file or a directory). </param>
    ///
    /// <returns> A string containing either the original or modified exception message. </returns>
    public static string ShortenedExceptionMessage(Exception ex, FileSystemInfo fs)
    {
        ArgumentNullException.ThrowIfNull(ex);
        ArgumentNullException.ThrowIfNull(fs);

        string strFullName = SafeGetFullName(fs);
        return ShortenedExceptionMessage(ex, strFullName);
    }

    /// <summary>
    /// For given assembly returns its full path
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied argument <paramref name="assembly"/> is null. </exception>
    /// <param name="assembly">Any assembly. Must not equal to null.</param>
    /// <returns>The full path to assembly (including filename)</returns>
    public static string GetAssemblyFilePathName(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        string strFileAsm = assembly.Location;
        string strRes = strFileAsm.Replace("file:///", "").Replace("/", @"\");

        return strRes;
    }

    /// <summary>
    /// For given assembly returns a full path of the directory where the assembly is.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied argument <paramref name="assembly"/>
    /// is null. </exception>
    /// <param name="assembly">Any assembly. Must not equal to null.</param>
    /// <returns>The directory containing the assembly</returns>
    public static string GetAssemblyDirectory(Assembly assembly)
    {
        FileInfo fi = new FileInfo(GetAssemblyFilePathName(assembly));
        string strFullPath = GetAssemblyFilePathName(assembly);
        string strPath = strFullPath.Substring(0, strFullPath.LastIndexOf(fi.Name, StringComparison.Ordinal));

        return strPath;
    }

    /// <summary>
    /// Returns the application executable full path ( including the filename).
    /// </summary>
    /// <returns>The full path of the  executable</returns>
    public static string GetApplicationFileName()
    {
        Assembly entryAssembly = Assembly.GetEntryAssembly();
        string strRes = string.Empty;

        if (null != entryAssembly)
        {
            strRes = GetAssemblyFilePathName(entryAssembly);
        }
        return strRes;
    }

    /// <summary> Query if 'c' is one of directory separator characters. </summary>
    /// <param name="c"> The tested character. </param>
    /// <returns> true if char <paramref name="c"/> is one of directory separators, false if not. </returns>
    public static bool IsDirectorySeparator(char c)
    {
        return _directorySeparators.Contains(c);
    }
    #endregion // Public Methods

    #region Private Methods

    /// <summary> Normalizes the folder path, by replacing Path.AltDirectorySeparatorChar
    /// with Path.DirectorySeparatorChar, and by adding Path.DirectorySeparatorChar to the end. </summary>
    /// 
    /// <exception cref="ArgumentException"> Thrown when one of supplied string arguments could not represent 
    ///  a valid folder path. </exception>
    /// <exception cref="ArgumentNullException"> Thrown when one of supplied string arguments is null. </exception>
    /// 
    /// <param name="path"> Given path to the folder. </param>
    /// <returns> A resulting (normalized) path. </returns>
    private static string NormalizeFolderPath(string path)
    {
        string result = path;

        CheckIsValidFolderString(path);
        result = result.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        result = AppendPathSeparator(result);

        return result;
    }

    /// <summary>
    /// Returns the directory information for the specified path string, works even on case the path
    /// parameter is longer than the 259 characters.
    /// </summary>
    /// <remarks>
    /// Implementation helper called by <see cref="LongPathTolerantGetDirectoryName"/>.
    /// </remarks>
    /// 
    /// <param name="path"> Given path of a file or directory. <br/>
    /// It is assumed that the caller has checked the validity of this argument, i.e. this parameter
    /// is not empty, does not contain path-invalid characters, and does not contain only white spaces.
    /// </param>
    /// 
    /// <returns>
    /// Directory information for the <paramref name="path"/>, or null if the path either
    /// denotes a root directory ( like "c:\" ), <br/>
    /// or does not contain directory information ( like "c:" ), <br/>
    /// or it is just null. <br/>
    /// In these aspects, the method is consistent with Path.GetDirectoryName.
    /// </returns>
    private static string DoGetDirectoryName(string path)
    {
        string result = null;

        if (path != null)
        {
            Debug.Assert(!string.IsNullOrEmpty(path.Trim()));
            CheckInvalidPathChars(path);

            if (!IsPathRootOnly(path, true))
            {
                // from now on, use tempPath with primitive normalization
                string tempPath = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                int length = tempPath.Length;
                int rootLength = GetRootLength(tempPath);

                do
                {
                } while ((length > rootLength) && !IsDirectorySeparator(tempPath[--length]));

                result = tempPath.Substring(0, length);
            }
        }

        return result;
    }

    /// <summary> An implementation helper, gets length of root part of the path. </summary>
    /// 
    /// <param name="path"> Given path of a file or directory. <br/>
    ///  It is assumed that the caller has checked the validity of this argument, i.e. this parameter is
    ///  not null, is not empty, does not contain path-invalid characters, and does not contains only
    ///  white spaces. </param>
    /// <returns> The root length. </returns>
    private static int GetRootLength(string path)
    {
        Debug.Assert(path != null);
        Debug.Assert(!string.IsNullOrEmpty(path.Trim()));
        CheckInvalidPathChars(path);

        int rootLength = 0;
        int length = path.Length;

        if (length >= 1 && IsDirectorySeparator(path[0]))
        {
            rootLength = 1;
        }
        else if (length >= 2 && path[1] == System.IO.Path.VolumeSeparatorChar)
        {
            rootLength = 2;
            if (length >= 3 && IsDirectorySeparator(path[2]))
                ++rootLength;
        }
        return rootLength;
    }

    private static void DoCheckArgValidPath(string path, string argName, char[] invalidChars)
    {
        if (string.IsNullOrEmpty(argName))
        {
            argName = nameof(path);
        }

        ArgumentNullException.ThrowIfNull(path, argName);
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Path can't be null or empty string", argName);
        }
        else
        {
            int index = path.IndexOfAny(invalidChars);

            if (index >= 0)
            {
                char ch = path[index];
                string strErr = $"The supplied {argName} '{path}' contains invalid character '{ch}' at index {index}";
                throw new ArgumentException(strErr, nameof(path));
            }
        }
    }

    /// <summary> (Only available in DEBUG builds) checks for invalid path characters, causing an assertion 
    /// if any of invalid characters occurs within <paramref name="path"/>. </summary>
    /// <param name="path"> Absolute or relative path. </param>
    [Conditional("DEBUG")]
    private static void CheckInvalidPathChars(string path)
    {
        for (int i = 0; i < path.Length; i++)
        {
            int num2 = path[i];
            if (((num2 == 0x22) || (num2 == 60)) || (((num2 == 0x3e) || (num2 == 0x7c)) || (num2 < 0x20)))
            {
                Debug.Assert(false);
            }
        }
    }
    #endregion // Private Methods
    #endregion // Methods
}
#pragma warning restore IDE0305
#pragma warning restore IDE0057 // Use range operator
#pragma warning restore IDE0090 // Use 'new(...)'
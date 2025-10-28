// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using PK.PkUtils.Extensions;

#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.PkUtils.IO;

/// <summary> A static class containing extension methods for <see cref="IFileSearch"/> argument. </summary>
public static class FileSearchExtension
{
    /// <summary>
    /// Search for all files matching the given pathname, which could include wildcards, like
    /// like "C:\Windows\t??.exe" <br/>
    /// 
    /// <list type="bullet">
    /// <item><b>C:\Windows\t??.exe</b><br/>  should find all "t??.exe" files under "C:\Windows" </item>
    /// <item><b>C:\Windows\</b><br/> should find all files under "C:\Windows" folder </item>
    /// <item><b>C:\Windows</b><br/> should find all files under "C:\Windows" folder, too</item>
    /// </list>
    /// 
    /// If the path is not rooted, method assumes the relative path is based on the absolute path of
    /// application directory.
    /// </summary>
    /// <remarks>
    /// Delegates the functionality to the abstract overload, which needs to be implemented in derived
    /// classes.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> Thrown when one of arguments <paramref name="fs"/> and 
    /// <paramref name="strPath"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal
    ///  values. </exception>
    /// 
    /// <param name="fs"> The searcher to act on. </param>
    /// <param name="strPath"> The directory or path, and the file name, which can include wildcard
    ///  characters; for instance "c:\Tools\*.exe". </param>
    /// <param name="searchOption"> A value which specifies how the search should be performed. </param>
    /// 
    /// <returns> Resulting sequence of files that can be iterated. </returns>
    public static IEnumerable<FileInfo> SearchFiles(this IFileSearch fs,
        string strPath, SearchOption searchOption)
    {
        ArgumentNullException.ThrowIfNull(fs);
        ArgumentNullException.ThrowIfNull(strPath);

        if (string.IsNullOrEmpty(strPath))
        {
            throw new ArgumentException("Path cannot be empty", nameof(strPath));
        }
        if (!Path.IsPathRooted(strPath))
        {
            strPath = Path.GetFullPath(strPath);
        }

        if ((FilePathHelper.HasWildCard(strPath)) || (searchOption == SearchOption.AllDirectories))
        {
            // Search for multiple files there
            DirectoryInfo dir;
            string strErr, strProbe, strDirPathOnly;
            string strWildFileName = null;
            var dirSeps = FilePathHelper.DirectorySeparators;

            if (dirSeps.Contains(strPath.Last()))
            {
                strDirPathOnly = strPath;
            }
            else
            {
                // Remark:
                // - Approach strDirPathOnly = Path.GetFullPath(strPath) cannot be used,
                //     since it throws an exception for case of wildcards
                // - The call Path.GetDirectoryName(strPath) cannot be used, neither, 
                //     since Path.GetDirectoryName(@"c:\Tmp3") returns just @"c:\"

                if (FilePathHelper.HasWildCard(strProbe = strPath.Split(dirSeps.ToArray()).Last()))
                {
                    strDirPathOnly = Path.GetDirectoryName(strPath); // take 'non-wild' part of it
                    strWildFileName = strProbe;
                }
                else
                {
                    strDirPathOnly = strPath;   // consider the complete string as specified path
                }
            }

            if (null == (dir = FilePathHelper.SafeGetDirectoryInfo(strDirPathOnly)))
            {
                strErr = string.Format(CultureInfo.InvariantCulture, "Invalid path '{0}'", strDirPathOnly);
                throw new ArgumentException(strErr, nameof(strPath));
            }
            strWildFileName ??= "*.*";

            return fs.SearchFiles(dir, strWildFileName, searchOption);
        }
        else
        {   // return just one file
            return EnumerableExtensions.FromSingle(new FileInfo(strPath));
        }
    }
}
#pragma warning restore IDE0305
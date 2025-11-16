///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Microsoft Public License (MS-PL) notice
// 
// This software is a Derivative Work based upon a Code Project article series
// C# does Shell, Part 1 – 3	
// http://www.codeproject.com/Articles/3551/C-does-Shell-Part-1
// http://www.codeproject.com/Articles/3590/C-does-Shell-Part-2	
// http://www.codeproject.com/Articles/3728/C-does-Shell-Part-3
// published under Microsoft Public License.
// 
// The related Microsoft Public License (MS-PL) text is available at
// http://www.opensource.org/licenses/ms-pl.html
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Ignore Spelling: Utils
//
using System;

namespace PK.PkUtils.ShellLib;

/// <summary>
/// A wrapper around  <see cref="ShellApi.SHAddToRecentDocs(uint, string)"/> API call.
/// </summary>
public static class ShellAddRecent
{
    /// <summary>
    /// List enum values that could be used with <see cref="ShellApi.SHAddToRecentDocs(uint, string)"/> API call as
    /// its first argument.
    /// </summary>
    public enum ShellAddRecentDocs
    {
        /// <summary>
        /// The pv parameter of SHAddToRecentDocs points to a null-terminated string with the path and file name of the object.
        /// </summary>
        SHARD_PIDL = 0x00000001,

        /// <summary>
        /// The pv parameter of SHAddToRecentDocs points to a pointer to an item identifier list
        /// (PIDL) that identifies the document's file object.
        /// PIDLs that identify 'nonfile' objects are not allowed.
        /// </summary>
        SHARD_PATHA = 0x00000002,

        /// <summary>
        /// The pv parameter of SHAddToRecentDocs points to a null-terminated Unicode string with the path and file name of the object.
        /// </summary>
        /// <remarks>This is almost the same as SHARD_PATHA, but with unicode string</remarks>
        SHARD_PATHW = 0x00000003,
    }

    /// <summary>
    /// Adds an item <paramref name="path"/> to the system lists of items accessed most recently.
    /// </summary>
    /// <param name="path">A string that contains the path and file name of the item.
    /// </param>
    public static void AddToList(string path)
    {
        ShellApi.SHAddToRecentDocs((uint)ShellAddRecentDocs.SHARD_PATHW, path);
    }

    /// <summary>
    /// Clears the recent documents list.
    /// </summary>
    /// <seealso href="http://stackoverflow.com/questions/4678445/clean-windows-7-start-menu-mru-list">
    /// Stackoverflow about Clean Windows 7 Start Menu MRU List</seealso>
    public static void ClearList()
    {
        ShellApi.SHAddToRecentDocs((uint)ShellAddRecentDocs.SHARD_PIDL, IntPtr.Zero);
    }
}
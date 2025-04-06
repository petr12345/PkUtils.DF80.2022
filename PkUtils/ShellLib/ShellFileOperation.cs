/***************************************************************************************************************
*
* FILE NAME:   .\ShellLib\FilterByExtension.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class FilterByExtension 
*
**************************************************************************************************************/

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

// Ignore Spelling: Utils, Dest
//
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.ShellLib;

/// <summary>
/// A wrapper around <see cref="ShellApi.SHFileOperation"/> API call.
/// </summary>
public class ShellFileOperation
{
    /// <summary>
    /// An enum which indicates which operation will be performed by <see cref="ShellFileOperation"/> class, 
    /// which wraps <see cref="ShellApi.SHFileOperation"/> API call.<br/>
    /// Is used for the <see cref="Operation"/> property.
    /// </summary>
    public enum FileOperations
    {
        /// <summary> An enum constant representing 'Move the files specified in pFrom to the location specified in pTo'. </summary>
        FO_MOVE = 0x0001,

        /// <summary> An enum constant representing 'Copy the files specified in the pFrom member to the location specified'. </summary>
        FO_COPY = 0x0002,

        /// <summary> An enum constant representing 'Delete the files specified in pFrom.'. </summary>
        FO_DELETE = 0x0003,

        /// <summary> An enum constant representing 'Rename the file specified in pFrom. You cannot use this flag to rename multiple files with a single function call. Use FO_MOVE instead.'. </summary>
        FO_RENAME = 0x0004,
    }

    /// <summary>
    /// Flags that control the details of file operation.
    /// </summary>
    [Flags]
    public enum ShellFileOperationFlags
    {
        /// <summary>
        /// The pTo member specifies multiple destination files (one for
        /// each source file) rather than one directory where all source 
        /// files are to be deposited. 
        /// </summary>
        FOF_MULTIDESTFILES = 0x0001,

        /// <summary> Not currently used. </summary>
        FOF_CONFIRMMOUSE = 0x0002,

        /// <summary> Do not display a progress dialog box. </summary>
        FOF_SILENT = 0x0004,

        /// <summary> Give the file being operated on a new name in a move, copy, or rename operation 
        ///           if a file with the target name already exists.  </summary>
        FOF_RENAMEONCOLLISION = 0x0008,

        /// <summary> Respond with "Yes to All" for any dialog box that is displayed.  </summary>
        FOF_NOCONFIRMATION = 0x0010,

        /// <summary> If <see cref="FOF_RENAMEONCOLLISION"/> is specified and any files were renamed,
        /// assign a name mapping object containing their old and new names to the hNameMappings member.
        /// </summary>
        FOF_WANTMAPPINGHANDLE = 0x0020,

        /// <summary> Preserve Undo information, if possible. If pFrom does not
        ///           contain fully qualified path and file names, this flag is ignored. </summary>
        FOF_ALLOWUNDO = 0x0040,

        /// <summary> Perform the operation on files only if a wildcard file name (*.*) is specified. </summary>
        FOF_FILESONLY = 0x0080,

        /// <summary> Display a progress dialog box but do not show the file names.  </summary>
        FOF_SIMPLEPROGRESS = 0x0100,

        /// <summary> Do not confirm the creation of a new directory if the operation requires one to be created. </summary>
        FOF_NOCONFIRMMKDIR = 0x0200,

        /// <summary> Do not display a user interface if an error occurs.  </summary>
        FOF_NOERRORUI = 0x0400,

        /// <summary> Do not copy the security attributes of the file. </summary>
        FOF_NOCOPYSECURITYATTRIBS = 0x0800,

        /// <summary> Only operate in the local directory. Don't operate recursively into subdirectories. </summary>
        FOF_NORECURSION = 0x1000,

        /// <summary> Do not move connected files as a group. Only move the specified files. </summary>
        FOF_NO_CONNECTED_ELEMENTS = 0x2000,

        /// <summary> Send a warning if a file is being destroyed during a delete operation 
        ///           rather than recycled. This flag partially overrides FOF_NOCONFIRMATION. </summary>
        FOF_WANTNUKEWARNING = 0x4000,

        /// <summary> Treat reparse points as objects, not containers. </summary>
        FOF_NORECURSEREPARSE = 0x8000,
    }

    /// <summary>
    ///  Values of this are used as an (first) argument with <see cref="ShellApi.SHChangeNotify"/> API call,
    ///  to describes the event that has occurred.
    /// </summary>
    [Flags]
    public enum ShellChangeNotificationEvents : uint
    {
        /// <summary>
        /// The name of a 'nonfolder' item has changed. SHCNF_IDLIST or
        /// SHCNF_PATH must be specified in uFlags. dwItem1 contains the
        /// previous PIDL or name of the item. dwItem2 contains the new PIDL
        /// or name of the item.
        /// </summary>
        SHCNE_RENAMEITEM = 0x00000001,

        /// <summary>
        /// A 'nonfolder' item has been created. SHCNF_IDLIST or SHCNF_PATH
        /// must be specified in uFlags. dwItem1 contains the item that was
        /// created. dwItem2 is not used and should be NULL.
        /// </summary>
        SHCNE_CREATE = 0x00000002,

        /// <summary>
        /// A 'nonfolder' item has been deleted. SHCNF_IDLIST or SHCNF_PATH
        /// must be specified in uFlags. dwItem1 contains the item that was
        /// deleted. dwItem2 is not used and should be NULL.
        /// </summary>
        SHCNE_DELETE = 0x00000004,

        /// <summary>
        /// A folder has been created. SHCNF_IDLIST or SHCNF_PATH must be
        /// specified in uFlags. dwItem1 contains the folder that was
        /// created. dwItem2 is not used and should be NULL.
        /// </summary>
        SHCNE_MKDIR = 0x00000008,

        /// <summary>
        /// A folder has been removed. SHCNF_IDLIST or SHCNF_PATH must be
        /// specified in uFlags. dwItem1 contains the folder that was
        /// removed. dwItem2 is not used and should be NULL.
        /// </summary>
        SHCNE_RMDIR = 0x00000010,

        /// <summary>
        /// Storage media has been inserted into a drive. SHCNF_IDLIST or
        /// SHCNF_PATH must be specified in uFlags. dwItem1 contains the root
        /// of the drive that contains the new media. dwItem2 is not used
        /// and should be NULL.
        /// </summary>
        SHCNE_MEDIAINSERTED = 0x00000020,

        /// <summary>
        /// Storage media has been removed from a drive. SHCNF_IDLIST or
        /// SHCNF_PATH must be specified in uFlags. dwItem1 contains the root
        /// of the drive from which the media was removed. dwItem2 is not
        /// used and should be NULL.
        /// </summary>
        SHCNE_MEDIAREMOVED = 0x00000040,

        /// <summary>
        /// A drive has been removed. SHCNF_IDLIST or SHCNF_PATH must be
        /// specified in uFlags. dwItem1 contains the root of the drive that
        /// was removed. dwItem2 is not used and should be NULL.
        /// </summary>
        SHCNE_DRIVEREMOVED = 0x00000080,

        /// <summary>
        /// A drive has been added. SHCNF_IDLIST or SHCNF_PATH must be
        /// specified in uFlags. dwItem1 contains the root of the drive that
        /// was added. dwItem2 is not used and should be NULL.
        /// </summary>
        SHCNE_DRIVEADD = 0x00000100,

        /// <summary>
        /// A folder on the local computer is being shared via the network.
        /// SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags. dwItem1
        /// contains the folder that is being shared. dwItem2 is not used and
        /// should be NULL.
        /// </summary>
        SHCNE_NETSHARE = 0x00000200,

        /// <summary>
        /// A folder on the local computer is no longer being shared via the
        /// network. SHCNF_IDLIST or SHCNF_PATH must be specified in uFlags.
        /// dwItem1 contains the folder that is no longer being shared.
        /// dwItem2 is not used and should be NULL.
        /// </summary>
        SHCNE_NETUNSHARE = 0x00000400,

        /// <summary>
        /// The attributes of an item or folder have changed. SHCNF_IDLIST
        /// or SHCNF_PATH must be specified in uFlags. dwItem1 contains the
        /// item or folder that has changed. dwItem2 is not used and should
        /// be NULL.
        /// </summary>
        SHCNE_ATTRIBUTES = 0x00000800,

        /// <summary>
        /// The contents of an existing folder have changed, but the folder
        /// still exists and has not been renamed. SHCNF_IDLIST or SHCNF_PATH
        /// must be specified in uFlags. dwItem1 contains the folder that
        /// has changed. dwItem2 is not used and should be NULL. If a folder
        /// has been created, deleted, or renamed, use SHCNE_MKDIR,
        /// SHCNE_RMDIR, or SHCNE_RENAMEFOLDER, respectively, instead.
        /// </summary>
        SHCNE_UPDATEDIR = 0x00001000,

        /// <summary>
        /// An existing 'nonfolder' item has changed, but the item still exists
        /// and has not been renamed. SHCNF_IDLIST or SHCNF_PATH must be
        /// specified in uFlags. dwItem1 contains the item that has changed.
        /// dwItem2 is not used and should be NULL. If a 'nonfolder' item has
        /// been created, deleted, or renamed, use SHCNE_CREATE,
        /// SHCNE_DELETE, or SHCNE_RENAMEITEM, respectively, instead.
        /// </summary>
        SHCNE_UPDATEITEM = 0x00002000,

        /// <summary>
        /// The computer has disconnected from a server. SHCNF_IDLIST or
        /// SHCNF_PATH must be specified in uFlags. dwItem1 contains the
        /// server from which the computer was disconnected. dwItem2 is not
        /// used and should be NULL.
        /// </summary>
        SHCNE_SERVERDISCONNECT = 0x00004000,

        /// <summary>
        /// An image in the system image list has changed. SHCNF_DWORD must be
        /// specified in uFlags. dwItem1 contains the index in the system image
        /// list that has changed. dwItem2 is not used and should be NULL.
        /// </summary>
        SHCNE_UPDATEIMAGE = 0x00008000,

        /// <summary>
        /// A drive has been added and the Shell should create a new window
        /// for the drive. SHCNF_IDLIST or SHCNF_PATH must be specified in
        /// uFlags. dwItem1 contains the root of the drive that was added.
        /// dwItem2 is not used and should be NULL.
        /// </summary>
        SHCNE_DRIVEADDGUI = 0x00010000,

        /// <summary>
        /// The name of a folder has changed. SHCNF_IDLIST or SHCNF_PATH must
        /// be specified in uFlags. dwItem1 contains the previous pointer to
        /// an item identifier list (PIDL) or name of the folder. dwItem2
        /// contains the new PIDL or name of the folder.
        /// </summary>
        SHCNE_RENAMEFOLDER = 0x00020000,

        /// <summary>
        /// The amount of free space on a drive has changed. SHCNF_IDLIST or
        /// SHCNF_PATH must be specified in uFlags. dwItem1 contains the root
        /// of the drive on which the free space changed. dwItem2 is not used
        /// and should be NULL.
        /// </summary>
        SHCNE_FREESPACE = 0x00040000,

        /// <summary>
        /// Not currently used.
        /// </summary>
        SHCNE_EXTENDED_EVENT = 0x04000000,

        /// <summary>
        /// A file type association has changed. SHCNF_IDLIST must be
        /// specified in the uFlags parameter. dwItem1 and dwItem2 are not
        /// used and must be NULL.
        /// </summary>
        SHCNE_ASSOCCHANGED = 0x08000000,

        /// <summary>
        /// Specifies a combination of all of the disk event identifiers.
        /// </summary>
        SHCNE_DISKEVENTS = 0x0002381F,

        /// <summary>
        /// Specifies a combination of all of the global event identifiers.
        /// </summary>
        SHCNE_GLOBALEVENTS = 0x0C0581E0,

        /// <summary>
        /// All events have occurred.
        /// </summary>
        SHCNE_ALLEVENTS = 0x7FFFFFFF,

        /// <summary>
        /// The specified event occurred as a result of a system interrupt.
        /// As this value modifies other event values, it cannot be used alone.
        /// </summary>
        SHCNE_INTERRUPT = 0x80000000,
    }

    /// <summary>
    /// Flags that are used as an (second) argument with <see cref="ShellApi.SHChangeNotify"/> API call,
    /// thus indicating the meaning of the last two parameters dwItem1 and dwItem2.
    /// </summary>
    public enum ShellChangeNotificationFlags
    {
        /// <summary>
        /// The dwItem1 and dwItem2 are the addresses of ITEMIDLIST structures that
        /// represent the item(s) affected by the change. Each ITEMIDLIST must be
        /// relative to the desktop folder.
        /// </summary>
        SHCNF_IDLIST = 0x0000,

        /// <summary>
        /// The dwItem1 and dwItem2 are the addresses of null-terminated ANSI strings of
        /// maximum length MAX_PATH that contain the full path names of the items
        /// affected by the change.
        /// </summary>
        SHCNF_PATHA = 0x0001,

        /// <summary>
        /// The dwItem1 and dwItem2 are the addresses of null-terminated strings that
        /// represent the friendly names of the printer(s) affected by the change.
        /// </summary>
        SHCNF_PRINTERA = 0x0002,

        /// <summary>
        /// The dwItem1 and dwItem2 parameters are DWORD values.
        /// </summary>
        SHCNF_DWORD = 0x0003,

        /// <summary>
        /// Analogical case like SHCNF_PATHA, but with unicode string
        /// </summary>
        SHCNF_PATHW = 0x0005,

        /// <summary>
        /// Analogical case like SHCNF_PRINTERA, but with unicode string
        /// </summary>
        SHCNF_PRINTERW = 0x0006,

        /// <summary>
        /// SHCNF_TYPE is a bitmask used for AND operation on flag values to get to know what
        /// what dwItem1 And dwItem2 mean.
        /// Don't use.
        /// </summary>
        SHCNF_TYPE = 0x00FF,

        /// <summary>
        /// The function should not return until the notification has been delivered
        /// to all affected components. As this flag modifies other data-type flags,
        /// it cannot by used by itself.
        /// </summary>
        SHCNF_FLUSH = 0x1000,

        /// <summary>
        /// The function should begin delivering notifications to all affected
        /// components but should return as soon as the notification process has
        /// begun. As this flag modifies other data-type flags, it cannot by used
        /// by itself.
        /// </summary>
        SHCNF_FLUSHNOWAIT = 0x2000
    }

    /// <summary>
    /// Specifies which file operation will be performed.
    /// </summary>
    public FileOperations Operation { get; set; }

    /// <summary>
    /// Window handle to the parent of dialog box to display information about the status of the file operation.
    /// </summary>
    public IntPtr OwnerWindow { get; set; }

    /// <summary>
    /// Flags that control the file operation. 
    /// This member can take a combination of <see cref="ShellFileOperationFlags"/> enum values.
    /// </summary>
    public ShellFileOperationFlags OperationFlags { get; set; }

    /// <summary>
    /// Address of a string to use as the title of a progress dialog box. This member is used only if
    /// <see cref="OperationFlags"/> includes the FOF_SIMPLEPROGRESS flag.
    /// </summary>
    public string ProgressTitle { get; set; }

    /// <summary>
    /// A collection containing one or more source file names.<br/>
    /// Standard wildcard characters, such as "*", are permitted only in the file-name position.
    /// Using a wildcard character elsewhere in the string will lead to unpredictable results.
    /// These names should be fully qualified paths to prevent unexpected results.<br/>
    /// </summary>
    public IEnumerable<string> SourceFiles { get; set; }

    /// <summary>
    /// A collection containing one or more destination file names.<br/>
    /// Wildcard characters are not supported.<br/>
    /// Use fully qualified paths. Using relative paths is not prohibited, but can have unpredictable results.<br/>
    /// </summary>
    public IEnumerable<string> DestFiles { get; set; }

    /// <summary>
    /// Default argument-less constructor.
    /// </summary>
    public ShellFileOperation()
    {
        // set default properties
        Operation = FileOperations.FO_COPY;
        OwnerWindow = IntPtr.Zero;
        OperationFlags = ShellFileOperationFlags.FOF_ALLOWUNDO
          | ShellFileOperationFlags.FOF_MULTIDESTFILES
          | ShellFileOperationFlags.FOF_NO_CONNECTED_ELEMENTS
          | ShellFileOperationFlags.FOF_WANTNUKEWARNING;
        ProgressTitle = "";
    }

    /// <summary>
    /// The call of this actually makes the file operation, delegating to <see cref="ShellApi.SHFileOperation"/> API call.
    /// </summary>
    /// <returns>True on success; false on any failure or if any operation has been aborted.</returns>
    public bool DoOperation()
    {
        ShellApi.SHFILEOPSTRUCT fileOpStruct = new()
        {
            hwnd = OwnerWindow,
            wFunc = (uint)Operation
        };

        string multiSource = StringCollectionToMultiString(SourceFiles);
        string multiDest = StringCollectionToMultiString(DestFiles);
        fileOpStruct.pFrom = Marshal.StringToHGlobalUni(multiSource);
        fileOpStruct.pTo = Marshal.StringToHGlobalUni(multiDest);

        fileOpStruct.fFlags = (ushort)OperationFlags;
        fileOpStruct.lpszProgressTitle = ProgressTitle;
        fileOpStruct.fAnyOperationsAborted = 0;
        fileOpStruct.hNameMappings = IntPtr.Zero;

        int RetVal;
        RetVal = ShellApi.SHFileOperation(ref fileOpStruct);

        ShellApi.SHChangeNotify(
          (uint)ShellChangeNotificationEvents.SHCNE_ALLEVENTS,
          (uint)ShellChangeNotificationFlags.SHCNF_DWORD,
          IntPtr.Zero,
          IntPtr.Zero);

        if (RetVal != 0)
        {
            return false;
        }

        if (fileOpStruct.fAnyOperationsAborted != 0)
        {
            return false;
        }

        return true;
    }

    private static string StringCollectionToMultiString(IEnumerable<string> stringCollection)
    {
        string multiString = string.Empty;

        if (stringCollection != null)
        {
            stringCollection.SafeForEach(str => multiString += (str + '\0'));
            multiString += '\0';
        }

        return multiString;
    }
}
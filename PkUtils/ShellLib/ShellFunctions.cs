/***************************************************************************************************************
*
* FILE NAME:   .\ShellLib\ShellFunctions.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class ShellFunctions
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


// Ignore Spelling: Malloc, Utils
//
using System;
using System.Runtime.InteropServices;


namespace PK.PkUtils.ShellLib;

/// <summary>
/// A wrapper around functionality returning managed interfaces like <see cref="IMalloc"/>, 
/// <see cref="IShellFolder"/> etc.
/// </summary>
public static class ShellFunctions
{
    /// <summary>
    /// Returns the system object implementing <see cref="IMalloc"/> interface, 
    /// the return involves calling <see cref="ShellApi.SHGetMalloc"/>.
    /// </summary>
    ///
    /// <returns>
    /// Returns a managed object implementing a <see cref="IMalloc"/> interface, that represents a COM object.
    /// </returns>
    public static IMalloc GetMalloc()
    {
        ShellApi.SHGetMalloc(out nint ptrRet);

        object obj = Marshal.GetTypedObjectForIUnknown(ptrRet, GetMallocType());
        IMalloc imalloc = (IMalloc)obj;

        return imalloc;
    }

    /// <summary>
    /// Retrieves the <see cref="IShellFolder"/> interface for the desktop folder, 
    /// which is the root of the Shell's namespace.<br/>
    /// The return involves calling <see cref="ShellApi.SHGetDesktopFolder"/>.
    /// </summary>
    /// <returns>An IShellFolder interface pointer for the desktop folder.</returns>
    public static IShellFolder GetDesktopFolder()
    {
        ShellApi.SHGetDesktopFolder(out IShellFolder ptrRet);
        /* old code, without calling the other method GetShellFolder
         * 
        System.Type shellFolderType = typeof(IShellFolder);
        Object obj = Marshal.GetTypedObjectForIUnknown(ptrRet, shellFolderType);
        IShellFolder ishellFolder = (IShellFolder)obj;

        return ishellFolder;
        */
        // return GetShellFolder(ptrRet);
        // ##FIX#

        return ptrRet;
    }

    /// <summary>
    /// Retrieves the <see cref="IShellFolder"/> interface for the given 
    /// unmanaged object which implements the IUnknown interface.
    /// </summary>
    /// <param name="ptrShellFolder">Interface to unmanaged object for which a managed wrapper will be returned.</param>
    /// <returns>An object implementing IShellFolder interface.</returns>
    public static IShellFolder GetShellFolder(IntPtr ptrShellFolder)
    {
        System.Type shellFolderType = GetShellFolderType();
        object obj = Marshal.GetTypedObjectForIUnknown(ptrShellFolder, shellFolderType);
        IShellFolder retVal = (IShellFolder)obj;

        return retVal;
    }

    /// <summary>
    /// Returns the type of <see cref="IShellFolder"/> interface.
    /// </summary>
    /// <returns>An instance of System.Type object.</returns>
    public static Type GetShellFolderType()
    {
        System.Type shellFolderType = typeof(IShellFolder);
        return shellFolderType;
    }

    /// <summary>
    /// Returns the type of <see cref="IMalloc"/> interface.
    /// </summary>
    /// <returns>An instance of System.Type object.</returns>
    public static Type GetMallocType()
    {
        System.Type mallocType = typeof(IMalloc);
        return mallocType;
    }

    /// <summary>
    /// Returns the type of <see cref="IFolderFilter"/> interface.
    /// </summary>
    /// <returns>An instance of System.Type object.</returns>
    public static Type GetFolderFilterType()
    {
        System.Type folderFilterType = typeof(IFolderFilter);
        return folderFilterType;
    }

    /// <summary>
    /// Returns the type of <see cref="IFolderFilterSite"/> interface.
    /// </summary>
    /// <returns>An instance of System.Type object.</returns>
    public static Type GetFolderFilterSiteType()
    {
        System.Type folderFilterSiteType = typeof(IFolderFilterSite);
        return folderFilterSiteType;
    }

}
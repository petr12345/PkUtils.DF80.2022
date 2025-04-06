/***************************************************************************************************************
*
* FILE NAME:   .\ShellLib\IFolderFilter.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of IFolderFilter
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


// Ignore Spelling: Utils
//
using System;
using System.Runtime.InteropServices;

namespace PK.PkUtils.ShellLib;

/// <summary>
/// Exposed by a client to specify how to filter the enumeration of a Shell folder by a server application.
/// </summary>
///
/// <remarks>
/// This interface is most often used with <see cref="ShellApi.SHBrowseForFolder"/> to filter the contents of
/// the tree view displayed in a folder selection dialog box. To use IFolderFilter with SHBrowseForFolder, the
/// BIF_NEWDIALOGSTYLE flag must be set.
/// <br/>
/// When your application calls SHBrowseForFolder, you become a client of the folder browser object. The folder
/// browser object communicates with you by sending messages to a callback function, BrowseCallbackProc. The
/// BFFM_IUNKNOWN message handled by that callback function contains a pointer to the folder browser's IUnknown
/// interface. To filter the display of a folder's contents, do the following:<br/>
/// <list type="number">
/// <item>.Use the folder browser's QueryInterface method to request a pointer to the IFolderFilterSite
/// interface.
/// </item>
/// <item>Call IFolderFilterSite::SetFilter, passing it a pointer to your IFolderFilter interface.
/// </item>
/// <item>The folder browser then queries IFolderFilter::GetEnumFlags and IFolderFilter::ShouldShowto determine
/// how to filter the enumeration.
/// </item>
/// </list>
/// </remarks>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("9CC22886-DC8E-11d2-B1D0-00C04F8EEB3E")]
public interface IFolderFilter
{
    /// <summary>
    /// Allows a client to specify which individual items should be enumerated.
    /// Note: The host calls this method for each item in the folder. Return S_OK, to have the item enumerated. 
    /// Return S_FALSE to prevent the item from being enumerated.
    /// </summary>
    /// <param name="psf">A pointer to the folder's IShellFolder interface.</param>
    /// <param name="pidlFolder">The folder's PIDL.</param>
    /// <param name="pidlItem">The item's PIDL.</param>
    /// <returns>
    /// The return value is <see href="http://msdn.microsoft.com/en-us/library/bb446131.aspx"> HRESULT</see>,
    /// the common value of COM functions.<br/>
    /// Returns S_OK if the item should be shown, S_FALSE if it should not be shown, 
    /// or a standard error code if an error is encountered.<br/>
    /// If an error is encountered, the item is not shown.
    /// </returns>
    [PreserveSig]
    int ShouldShow(
      [MarshalAs(UnmanagedType.Interface)] object psf,
      IntPtr pidlFolder,
      IntPtr pidlItem);

    /// <summary>
    /// Allows a client to specify which classes of objects in a Shell folder should be enumerated.
    /// </summary>
    /// <param name="psf">A pointer to the folder's IShellFolder interface.</param>
    /// <param name="pidlFolder">// The folder's PIDL.</param>
    /// <param name="phwnd">// A pointer to the host's window handle.</param>
    /// <param name="pgrfFlags">One or more SHCONTF values that specify which classes of objects to enumerate.</param>
    /// <returns>
    /// If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
    /// </returns>
    [PreserveSig]
    int GetEnumFlags(
      [MarshalAs(UnmanagedType.Interface)] object psf,
      IntPtr pidlFolder,
      IntPtr phwnd,
      out uint pgrfFlags);
};
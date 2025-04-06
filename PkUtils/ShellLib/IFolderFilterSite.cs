/***************************************************************************************************************
*
* FILE NAME:   .\ShellLib\IFolderFilterSite.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of IFolderFilterSite
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
/// An interface exported by a host to allow clients to specify how to filter a Shell folder enumeration.
/// </summary>
///
/// <remarks>
/// The most common use of this interface is when your application calls SHBrowseForFolder. When you call this
/// function, you become a client of the folder browser object. That object communicates with you by sending
/// messages to a callback function, BrowseCallbackProc. The BFFM_IUNKNOWN message contains a pointer to the
/// folder browser's IUnknown interface. <br/>
/// To filter folder enumeration:<br/>
/// <list type="number">
/// <item>Use the IUnknown pointer to call the folder browser's QueryInterface method, and request a pointer to the IFolderFilterSite interface.
/// </item>
/// <item>Call IFolderFilterSite::SetFilter, and pass the folder browser a pointer to your IFolderFilter (IUnknown or IFilterFolder?) interface.
/// </item>
/// <item>The folder browser will then query the two methods of the IFolderFilterSite interface to determine how to filter the enumeration.
/// </item>
/// </list>
/// </remarks>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("C0A651F5-B48B-11d2-B5ED-006097C686F6")]
public interface IFolderFilterSite
{
    /// <summary>
    /// Exposed by a host to allow clients to pass the host their IUnknown interface pointers.
    /// </summary>
    /// <param name="punk">
    /// A pointer to the client's IUnknown interface. To notify the host to terminate 
    /// filtering and stop calling your IFolderFilter interface, set this parameter to null. 
    /// </param>
    /// <returns>
    /// If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
    /// </returns>
    [PreserveSig]
    int SetFilter(
      [MarshalAs(UnmanagedType.Interface)] object punk);
}
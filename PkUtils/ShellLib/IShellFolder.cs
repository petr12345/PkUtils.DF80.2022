/***************************************************************************************************************
*
* FILE NAME:   .\ShellLib\IShellFolder.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of IShellFolder
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
/// Exposed by all Shell namespace folder objects, its methods are used to manage folders.
/// </summary>
/// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb775075(v=vs.85).aspx">
/// MSDN description of IShellFolder</seealso>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("000214E6-0000-0000-C000-000000000046")]
public interface IShellFolder
{
    /// <summary>
    /// Translates a file object's or folder's display name into an item identifier list.
    /// Return value: error code, if any
    /// </summary>
    /// <param name="hwnd">Optional window handle</param>
    /// <param name="pbc">Optional bind context that controls the parsing operation. This parameter is normally set to NULL</param>
    /// <param name="pszDisplayName">Null-terminated UNICODE string with the display name.</param>
    /// <param name="pchEaten">Pointer to a ULONG value that receives the number of characters of the display name that was parsed.</param>
    /// <param name="ppidl">Pointer to an ITEMIDLIST pointer that receives the item identifier list for the object.</param>
    /// <param name="pdwAttributes">Optional parameter that can be used to query for file attributes. this can be values from the SFGAO enum</param>
    /// <returns>error code, if any</returns>
    [PreserveSig]
    int ParseDisplayName(
        IntPtr hwnd,
        IntPtr pbc,
        [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName,
        ref uint pchEaten,
        out IntPtr ppidl,
        ref uint pdwAttributes);

    /// <summary>
    /// Allows a client to determine the contents of a folder by creating an item identifier enumeration object 
    /// and returning its IEnumIDList interface.
    /// </summary>
    /// <param name="hwnd">
    /// If user input is required to perform the enumeration, this window handle 
    /// should be used by the enumeration object as the parent window to take user input.
    /// </param>
    /// <param name="grfFlags">
    /// Flags indicating which items to include in the enumeration. For a list of possible values, see the SHCONTF enum. 
    /// </param>
    /// <param name="ppenumIDList">Address that receives a pointer to the IEnumIDList interface 
    /// of the enumeration object created by this method. </param>
    /// <returns>error code, if any</returns>
    [PreserveSig]
    int EnumObjects(
        IntPtr hwnd,
        int grfFlags,
        out IntPtr ppenumIDList);

    /// <summary>
    /// Retrieves an IShellFolder object for a subfolder.
    /// </summary>
    /// <param name="pidl">Address of an ITEMIDLIST structure (PIDL) that identifies the subfolder.</param>
    /// <param name="pbc">Optional address of an IBindCtx interface on a bind context object to be used during this operation.</param>
    /// <param name="riid">Identifier of the interface to return.</param>
    /// <param name="ppv">Address that receives the interface pointer.</param>
    /// <returns>error code, if any</returns>
    [PreserveSig]
    int BindToObject(
        IntPtr pidl,
        IntPtr pbc,
        Guid riid,
        out IntPtr ppv);

    /// <summary>
    /// Requests a pointer to an object's storage interface. 
    /// </summary>
    /// <param name="pidl">Address of an ITEMIDLIST structure that identifies the subfolder relative to its parent folder.</param>
    /// <param name="pbc">Optional address of an IBindCtx interface on a bind context object to be used during this operation.</param>
    /// <param name="riid">Interface identifier (IID) of the requested storage interface.</param>
    /// <param name="ppv">Address that receives the interface pointer specified by riid.</param>
    /// <returns>error code, if any</returns>
    [PreserveSig]
    int BindToStorage(
        IntPtr pidl,
        IntPtr pbc,
        Guid riid,
        out IntPtr ppv);

    /// <summary>
    /// Determines the relative order of two file objects or folders, given their item identifier lists. 
    /// </summary>
    ///
    /// <param name="lParam"> Value that specifies how the comparison should be performed. The lower sixteen bits
    /// of lParam define the sorting rule. The upper sixteen bits of lParam are used for flags that modify the
    /// sorting rule. values can be from the SHCIDS enum. </param>
    /// <param name="pidl1">  Pointer to the first item's ITEMIDLIST structure. </param>
    /// <param name="pidl2">  Pointer to the second item's ITEMIDLIST structure. </param>
    ///
    /// <returns>
    /// The return value is <see href="http://msdn.microsoft.com/en-us/library/bb446131.aspx"> HRESULT</see>,
    /// the common value of COM functions.<br/>
    /// 
    /// If this method is successful, the CODE field of the HRESULT contains one of the values listed below.<br/>
    /// The code value be retrieved using the helper function <see cref="ShellApi.GetHResultCode"/>.<br/>
    /// If this method is unsuccessful, it returns a COM error code.<br/>
    /// 
    /// <list type="bullet">
    /// <listheader>Possible CODE values returned.</listheader>
    /// <item><b>Negative</b>A negative return value indicates that the first item should precede the second (pidl1
    /// &lt; pidl2).
    /// </item>
    /// <item><b>Positive</b>A positive return value indicates that the first item should follow the second (pidl1
    /// &gt; pidl2).
    /// </item>
    /// <item><b>Zero</b>A return value of zero indicates that the two items are the same (pidl1 = pidl2).
    /// </item>
    /// </list>
    /// </returns>
    /// 
    /// <seealso cref="ShellApi.GetHResultCode"/>
    [PreserveSig]
    int CompareIDs(
        int lParam,
        IntPtr pidl1,
        IntPtr pidl2);

    /// <summary>
    /// Requests an object that can be used to obtain information from or interact with a folder object.
    /// </summary>
    /// <param name="hwndOwner">Handle to the owner window.</param>
    /// <param name="riid">Identifier of the requested interface. </param>
    /// <param name="ppv"> Address of a pointer to the requested interface. </param>
    /// <returns>Return value: error code, if any</returns>
    [PreserveSig]
    int CreateViewObject(
        IntPtr hwndOwner,
        Guid riid,
        out IntPtr ppv);

    /// <summary>
    /// Retrieves the attributes of one or more file objects or subfolders. 
    /// </summary>
    /// <param name="cidl">Number of file objects from which to retrieve attributes. </param>
    /// <param name="apidl">
    /// Address of an array of pointers to ITEMIDLIST structures, each of which 
    /// uniquely identifies a file object relative to the parent folder.
    /// </param>
    /// <param name="rgfInOut">Address of a single ULONG value that, on entry, contains the attributes that 
    /// the caller is requesting. On exit, this value contains the requested 
    /// attributes that are common to all of the specified objects. this value can
    /// be from the SFGAO enum</param>
    /// <returns>Return value: error code, if any</returns>
    [PreserveSig]
    int GetAttributesOf(
      uint cidl,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IntPtr[] apidl,
      ref uint rgfInOut);

    /// <summary>
    /// Retrieves an OLE interface that can be used to carry out actions on the specified file objects or folders.
    /// </summary>
    /// <param name="hwndOwner">Handle to the owner window that the client should specify if it displays 
    /// a dialog box or message box.</param>
    /// <param name="cidl">Number of file objects or subfolders specified in the apidl parameter.</param>
    /// <param name="apidl">Address of an array of pointers to ITEMIDLIST structures, each of which 
    /// uniquely identifies a file object or subfolder relative to the parent folder.</param>
    /// <param name="riid">Identifier of the COM interface object to return.</param>
    /// <param name="rgfReserved">Reserved. </param>
    /// <param name="ppv">Pointer to the requested interface.</param>
    /// <returns>Return value: error code, if any</returns>
    [PreserveSig]
    int GetUIObjectOf(
        IntPtr hwndOwner,
        uint cidl,
        IntPtr[] apidl,
        Guid riid,
        ref uint rgfReserved,
        out IntPtr ppv);

    /// <summary>
    /// Retrieves the display name for the specified file object or subfolder. 
    /// </summary>
    /// <param name="pidl">Address of an ITEMIDLIST structure (PIDL) that uniquely identifies the file 
    /// object or subfolder relative to the parent folder. </param>
    /// <param name="uFlags">Flags used to request the type of display name to return. For a list of 
    /// possible values, see the SHGNO enum. </param>
    /// <param name="pName">Address of a STRRET structure in which to return the display name.</param>
    /// <returns>Return value: error code, if any</returns>
    [PreserveSig]
    int GetDisplayNameOf(
        IntPtr pidl,
        uint uFlags,
        out ShellApi.STRRET pName);

    /// <summary>
    /// Sets the display name of a file object or subfolder, changing the item identifier in the process.
    /// </summary>
    /// <param name="hwnd">Handle to the owner window of any dialog or message boxes that the client displays.</param>
    /// <param name="pidl">Pointer to an ITEMIDLIST structure that uniquely identifies the file object or subfolder relative to the parent folder. </param>
    /// <param name="pszName">Pointer to a null-terminated string that specifies the new display name. </param>
    /// <param name="uFlags">Flags indicating the type of name specified by the lpszName parameter. 
    /// For a list of possible values, see the description of the SHGNO enum.</param>
    /// <param name="ppidlOut">Address of a pointer to an ITEMIDLIST structure which receives the new ITEMIDLIST.</param>
    /// <returns>Return value: error code, if any</returns>
    [PreserveSig]
    int SetNameOf(
        IntPtr hwnd,
        IntPtr pidl,
        [MarshalAs(UnmanagedType.LPWStr)] string pszName,
        uint uFlags,
        out IntPtr ppidlOut);
}
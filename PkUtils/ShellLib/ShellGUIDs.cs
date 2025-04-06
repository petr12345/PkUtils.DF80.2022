/***************************************************************************************************************
*
* FILE NAME:   .\ShellLib\ShellGUIDs.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION:  The file contains definition of class ShellGUIDs
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


// Ignore Spelling: Utils, Malloc, IID
//
using System;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable IDE0090 // Use 'new(...)'

namespace PK.PkUtils.ShellLib;

/// <summary>
/// A structure containing interface identifiers (IID) of <see cref="IMalloc"/>, <see cref="IShellFolder"/>,
/// <see cref="IFolderFilterSite"/> and <see cref="IFolderFilter"/>.
/// </summary>
public static class ShellGUIDs
{
    /// <summary>
    /// The interface identifier (IID) for <see cref="IMalloc"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible",
      Justification = "This public non-readonly declaration is needed because of Marshal.QueryInterface, which needs ref argument")]
    public static Guid IID_IMalloc =
      new Guid("{00000002-0000-0000-C000-000000000046}");

    /// <summary>
    /// The interface identifier (IID) for <see cref="IShellFolder"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible",
      Justification = "This public non-readonly declaration is needed because of Marshal.QueryInterface, which needs ref argument")]
    public static Guid IID_IShellFolder =
      new Guid("{000214E6-0000-0000-C000-000000000046}");

    /// <summary>
    /// The interface identifier (IID) for <see cref="IFolderFilterSite"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible",
      Justification = "This public non-readonly declaration is needed because of Marshal.QueryInterface, which needs ref argument")]
    public static Guid IID_IFolderFilterSite =
      new Guid("{C0A651F5-B48B-11d2-B5ED-006097C686F6}");

    /// <summary>
    /// The interface identifier (IID) for <see cref="IFolderFilter"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible",
      Justification = "This public non-readonly declaration is needed because of Marshal.QueryInterface, which needs ref argument")]
    public static Guid IID_IFolderFilter =
      new Guid("{9CC22886-DC8E-11d2-B1D0-00C04F8EEB3E}");
}

#pragma warning restore IDE0090 // Use 'new(...)'
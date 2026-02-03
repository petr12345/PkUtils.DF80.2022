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


// Ignore Spelling: pidl, phwnd, psf, pgrf, Utils
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable IDE0290   // Use primary constructor
#pragma warning disable CA1806    //  ShouldShow calls StrRetToBSTR but does not use the HRESULT 

namespace PK.PkUtils.ShellLib;

/// <summary>
/// A helper class implementing <see cref="IFolderFilter"/> interface.
/// The implementation is using for filtering an array of valid (not filtered-out) file extensions.
/// </summary>
/// 
/// <example>
/// The following is an example of constructing FilterByExtension filter.
/// <code>
/// FilterByExtension filter = new FilterByExtension();
/// filter.ValidExtension = new string[] { "exe", "dll"};
/// </code>
/// </example>
[ComVisible(true)]
[Guid("3766C955-DA6F-4fbc-AD36-311E342EF180")]
public class FilterByExtension : IFolderFilter
{
    #region Fields

    /// <summary>
    /// A backing field for the <see cref="ValidExtension"/> property.
    /// </summary>
    protected IEnumerable<string> _validExtension;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default argument-less constructor.
    /// </summary>
    public FilterByExtension()
      : this(null)
    { }

    /// <summary> Constructor providing an initial value of <see cref="ValidExtension"/>property. </summary>
    ///
    /// <param name="validExtension"> An initial value for property <see cref="ValidExtension"/>. This argument
    /// could be null. </param>
    public FilterByExtension(IEnumerable<string> validExtension)
    {
        _validExtension = validExtension;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// A collection of string filters representing extensions ( without dot in the beginning ).
    /// </summary>
    public IEnumerable<string> ValidExtension
    {
        get { return _validExtension; }
        set { _validExtension = value; }
    }
    #endregion // Properties

    #region IFolderFilter members

    /// <summary>
    /// Allows a client to specify which individual items should be enumerated. Note: The host calls this method
    /// for each item in the folder. Return S_OK, to have the item enumerated. Return S_FALSE to prevent the item
    /// from being enumerated.
    /// </summary>
    /// <param name="psf"> A pointer to the folder's IShellFolder interface. </param>
    /// <param name="pidlFolder"> The folder's PIDL. </param>
    /// <param name="pidlItem"> The item's PIDL. </param>
    /// <returns>
    /// The return value is <see href="http://msdn.microsoft.com/en-us/library/bb446131.aspx"> HRESULT</see>,
    /// the common value of COM functions.<br/>
    /// Returns S_OK if the item should be shown, S_FALSE if it should not be shown, or a standard error code
    /// if an error is encountered.<br/>
    /// If an error is encountered, the item is not shown.
    /// </returns>
    public int ShouldShow(object psf, IntPtr pidlFolder, IntPtr pidlItem)
    {
        const int S_OK = 0;
        const int S_FALSE = 1;

        // Get display name of item
        IShellFolder isf = (IShellFolder)psf;

        isf.GetDisplayNameOf(pidlItem, (uint)ShellApi.SHGNO.SHGDN_NORMAL | (uint)ShellApi.SHGNO.SHGDN_FORPARSING, out ShellApi.STRRET ptrDisplayName);
        ShellApi.StrRetToBSTR(ref ptrDisplayName, IntPtr.Zero, out string sDisplay);

        // Check if item is file or folder
        IntPtr[] aPidl = [pidlItem];
        uint attrib = (uint)ShellApi.SFGAO.SFGAO_FOLDER;

        isf.GetAttributesOf(1, aPidl, ref attrib);

        // If item is a folder, accept
        if ((attrib & (uint)ShellApi.SFGAO.SFGAO_FOLDER) == (uint)ShellApi.SFGAO.SFGAO_FOLDER)
        {
            return S_OK; // accept by returning S_OK
        }
        else if (null != ValidExtension) // If item is file, check if it has a valid extension
        {
            string strMatch = ValidExtension.FirstOrDefault(strExt => sDisplay.EndsWith("." + strExt, StringComparison.OrdinalIgnoreCase));

            if (null != strMatch)
            { // If match has been found, accept by returning S_OK
                return S_OK;
            }
        }

        return S_FALSE;
    }

    /// <summary>
    /// Allows a client to specify which classes of objects in a Shell folder should be enumerated.
    /// </summary>
    /// <param name="psf">A pointer to the folder's IShellFolder interface.</param>
    /// <param name="pidlFolder">The folder's PIDL.</param>
    /// <param name="phwnd">A pointer to the host's window handle.</param>
    /// <param name="pgrfFlags">One or more SHCONTF values that specify which classes of objects to enumerate.</param>
    /// <returns>
    /// If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
    /// </returns>
    public int GetEnumFlags(object psf, IntPtr pidlFolder, IntPtr phwnd, out uint pgrfFlags)
    {
        pgrfFlags = (uint)ShellApi.SHCONTF.SHCONTF_FOLDERS | (uint)ShellApi.SHCONTF.SHCONTF_NONFOLDERS;
        return 0;
    }
    #endregion // IFolderFilter members
}

#pragma warning restore IDE0290
#pragma warning restore CA1806
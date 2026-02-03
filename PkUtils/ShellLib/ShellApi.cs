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


// Ignore Spelling: Api, App, appbar, Buf, interop, pidl, ppidl, pbc, ppv, psfgao, ppshf, pstr, lbpi, lpfn, Malloc, sfgao, riid, Ret, Utils, APPBARDATA, BROWSEINFO, CSIDL, SHGNO, SHGFP, SHCONTF, SHCIDS, SFGAO, SHGFP_TYPE, SHFILEOPSTRUCT, SHELLEXECUTEINFO, STRRET
//
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PK.PkUtils.ShellLib;

#pragma warning disable IDE0079     // Remove unnecessary suppressions
#pragma warning disable SYSLIB1054  // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
#pragma warning disable CA1069      // Enums values should not be duplicated
#pragma warning disable CA1401      // P/Invoke method should not be visible

/// <summary> A wrapper providing several interop declarations of APIS from shell32.dll. </summary>
public static class ShellApi
{
    #region Typedefs

    /// <summary>
    /// The delegate representing an application-defined function, that the dialog box dialog box
    /// <see cref="ShellBrowseForFolderDialog"/> calls when an event occurs.
    /// </summary>
    ///
    /// <remarks> This function is assigned to <see cref="BROWSEINFO"/>structure. </remarks>
    ///
    /// <param name="hwnd">   The window handle of the browse dialog box. </param>
    /// <param name="uMsg">   The dialog box event that generated the message. <br/>
    /// Possible values are defined by <see cref="ShellBrowseForFolderDialog.BrowseForFolderMessagesFrom"/> enum. </param>
    /// <param name="lParam"> value whose meaning depends on the event specified in <paramref name="uMsg"/>. </param>
    /// <param name="lpData"> An application-defined value that was specified in the lParam member of the
    /// <see cref="ShellApi.BROWSEINFO"/> structure used in the call of <see cref="ShellApi.SHBrowseForFolder"/>. </param>
    ///
    /// <returns>
    /// Should return zero, except in the case of message 
    /// <see cref="ShellBrowseForFolderDialog.BrowseForFolderMessagesFrom.BFFM_VALIDATEFAILEDA"/> and
    /// <see cref="ShellBrowseForFolderDialog.BrowseForFolderMessagesFrom.BFFM_VALIDATEFAILEDW"/>. 
    /// For these cases, returns zero to dismiss the dialog or nonzero to keep the dialog displayed.
    /// </returns>
    /// 
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb762598(v=vs.85).aspx">
    /// BFFCALLBACK function
    /// </seealso>
    public delegate Int32 BrowseCallbackProc(IntPtr hwnd, UInt32 uMsg, IntPtr lParam, IntPtr lpData);

    /// <summary>
    /// Contains parameters for the <see cref="SHBrowseForFolder"/> function and receives information about the folder selected
    /// by the user.
    /// </summary>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb773205(v=vs.85).aspx">
    /// BROWSEINFO structure
    /// </seealso>
    [StructLayout(LayoutKind.Sequential)]
    public struct BROWSEINFO
    {
        /// <summary> Handle to the owner window for the dialog box. </summary>
        public IntPtr hwndOwner;

        /// <summary>
        /// Pointer to an item identifier list (PIDL) specifying the location of the root folder from which to start
        /// browsing.
        /// </summary>
        public IntPtr pidlRoot;

        /// <summary> Address of a buffer to receive the display name of the folder selected by the user. </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public String pszDisplayName;

        /// <summary>
        /// Address of a null-terminated string that is displayed above the tree view control in the dialog box.
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public String lpszTitle;

        /// <summary> Flags specifying the options for the dialog box. </summary>
        public UInt32 ulFlags;

        /// <summary>
        /// Address of an application-defined function that the dialog box <see cref="ShellBrowseForFolderDialog"/> calls when an event occurs.
        /// </summary>
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public BrowseCallbackProc lpfn;

        /// <summary> Application-defined value that the dialog box passes to the callback function. </summary>
        public Int32 lParam;

        /// <summary> Variable to receive the image associated with the selected folder. </summary>
        public Int32 iImage;
    }

    /// <summary>
    /// A structure containing strings returned from the IShellFolder interface methods. <br/>
    /// The MSDN documentation of
    /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb759820(v=vs.85).aspx"> STRRET
    /// structure </see> provides more detailed information.<br/>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct STRRET : IDisposable
    {
        /// <summary> One of the STRRET_* values. </summary>
        public uint uType;

        /// <summary> The OLE string. must be freed eventually.</summary>
        public IntPtr pOleStr;

        /// <summary> The offset. </summary>
        public uint uOffset;

        /// <summary> The string. </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        public char[] cStr;

        /// <summary> Gets the string stored in <see cref="pOleStr"/>. </summary>
        /// <returns> The string. </returns>
        public readonly string GetString()
        {
            if (uType == 0) // STRRET_WSTR
            {
                string result = Marshal.PtrToStringUni(pOleStr);
                return result;
            }
            else // STRRET_CSTR
            {
                return new string(cStr);
            }
        }

        /// <summary>
        /// Performs the cleanup of <see cref="pOleStr"/> member variable.
        /// </summary>
        public void Dispose()
        {
            if (uType == 0 && pOleStr != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(pOleStr);
                pOleStr = IntPtr.Zero;
            }
        }
    }

    /// <summary> Contains information used by <see cref="ShellExecuteEx"/> </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SHELLEXECUTEINFO
    {
        /// <summary> Size of the structure, in bytes. </summary>
        public UInt32 cbSize;

        /// <summary> Array of flags that indicate the content and validity of the other structure members. 
        /// </summary>
        /// <remarks>
        /// The value should be a combination of <see cref="ShellExecute.ShellExecuteFlags"/> enum values.
        /// </remarks>
        public UInt32 fMask;

        /// <summary>
        /// Window handle to any message boxes that the system might produce while executing this function.
        /// </summary>
        public IntPtr hwnd;

        /// <summary> String, referred to as a verb, that specifies the action to be performed. </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public String lpVerb;

        /// <summary>
        /// Address of a null-terminated string that specifies the name of the file or object on which ShellExecuteEx
        /// will perform the action specified by the lpVerb parameter.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public String lpFile;

        /// <summary> Address of a null-terminated string that contains the application parameters. </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public String lpParameters;

        /// <summary> Address of a null-terminated string that specifies the name of the working directory. </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public String lpDirectory;

        /// <summary>
        /// A value that specify how an application is to be shown when it is opened.<br/>
        /// The value should be one of enum values of
        /// <see cref="PK.PkUtils.ShellLib.ShellExecute.ShowWindowCommands"/> enum type.
        /// </summary>
        public Int32 nShow;

        /// <summary> If the function succeeds, it sets this member to a value greater than 32. </summary>
        public IntPtr hInstApp;

        /// <summary>
        /// Address of an ITEMIDLIST structure to contain an item identifier list uniquely identifying the file to
        /// execute.
        /// </summary>
        public IntPtr lpIDList;

        /// <summary>
        /// Address of a null-terminated string that specifies the name of a file class or a globally unique identifier
        /// (GUID).
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public String lpClass;

        /// <summary> Handle to the registry key for the file class. </summary>
        public IntPtr hkeyClass;

        /// <summary> Hot key to associate with the application. </summary>
        public UInt32 dwHotKey;

        /// <summary>
        /// Handle to the icon for the file class. OR Handle to the monitor upon which the document is to be displayed.
        /// </summary>
        public IntPtr hIconMonitor;

        /// <summary> Handle to the newly started application ( Win32 process ). </summary>
        public IntPtr hProcess;
    }

    /// <summary>
    /// Contains information that the <see cref="SHFileOperation"/> function uses to perform file operations.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SHFILEOPSTRUCT
    {
        /// <summary>
        /// Window handle to the dialog box to display information about the status of the file operation.
        /// </summary>
        public IntPtr hwnd;

        /// <summary> Value that indicates which operation to perform. </summary>
        public UInt32 wFunc;

        /// <summary>
        /// Address of a buffer to specify one or more source file names.<br/>
        /// These names must be fully qualified paths. Standard Microsoft® MS-DOS® wild cards, such as "*", are
        /// permitted in the file-name position. Although this member is declared as a null-terminated string, it is
        /// used as a buffer to hold multiple file names. Each file name must be terminated by a single NULL character.
        /// An additional NULL character must be appended to the end of the final name to indicate the end of pFrom.
        /// </summary>
        public IntPtr pFrom;

        /// <summary>
        /// Address of a buffer to contain the name of the destination file or directory. This parameter must be set to
        /// NULL if it is not used. Like pFrom, the pTo member is also a double-null terminated string and is handled
        /// in much the same way.
        /// </summary>
        public IntPtr pTo;

        /// <summary> Flags that control the file operation. <br/>
        /// An actual value should be a bitwise OR combination combination of
        /// <see cref="ShellFileOperation.ShellFileOperationFlags"/> enum values.
        /// </summary>
        public UInt16 fFlags;

        /// <summary>
        /// Value that receives TRUE (non-zero) if the user aborted any file operations before they were completed, or
        /// FALSE (zero) otherwise.
        /// </summary>
        public Int32 fAnyOperationsAborted;

        /// <summary>
        /// A handle to a name mapping object containing the old and new names of the renamed files. This member is
        /// used only if the fFlags member includes the FOF_WANTMAPPINGHANDLE flag.
        /// </summary>
        public IntPtr hNameMappings;

        /// <summary>
        /// Address of a string to use as the title of a progress dialog box. This member is used only if fFlags
        /// includes the <see cref="ShellFileOperation.ShellFileOperationFlags.FOF_SIMPLEPROGRESS"/> flag.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public String lpszProgressTitle;
    }

    /// <summary>
    /// The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        /// <summary> The x-coordinate of the upper-left corner of the rectangle. </summary>
        public Int32 left;

        /// <summary> The y-coordinate of the upper-left corner of the rectangle. </summary>
        public Int32 top;

        /// <summary> The x-coordinate of the lower-right corner of the rectangle. </summary>
        public Int32 right;

        /// <summary> The y-coordinate of the lower-right corner of the rectangle. </summary>
        public Int32 bottom;
    }

    /// <summary>
    /// The enum representing the current Appbar position (which screen edge it is aligned to).
    /// </summary>
    public enum AppBarEdges
    {
        /// <summary> An enum constant representing the left option. </summary>
        Left = 0,
        /// <summary> An enum constant representing the top option. </summary>
        Top = 1,
        /// <summary> An enum constant representing the right option. </summary>
        Right = 2,
        /// <summary> An enum constant representing the bottom option. </summary>
        Bottom = 3,
        /// <summary> An enum constant representing the float option. </summary>
        Float = 4
    }

    /// <summary> Contains information about a system appbar message. </summary>
    ///
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb773184(v=vs.85).aspx">
    /// MSDN documentation of APPBARDATA structure
    /// </seealso>
    [StructLayout(LayoutKind.Sequential)]
    public struct APPBARDATA
    {
        #region Fields

        /// <summary> The size of the structure, in bytes. </summary>
        public UInt32 cbSize;

        /// <summary> The handle to the appbar window. </summary>
        public IntPtr hWnd;

        /// <summary>
        /// An application-defined message identifier. The application uses the specified identifier for notification
        /// messages that it sends to the appbar identified by the hWnd member. This member is used when sending the
        /// ABM_NEW message.
        /// </summary>
        public UInt32 uCallbackMessage;

        /// <summary>
        /// A value that specifies an edge of the screen.<br/>
        /// <br/>
        /// This member can be one of the following values:<br/>
        /// <list type="bullet">
        /// <item><b>ABE_BOTTOM</b><br/>Bottom edge.</item>
        /// <item><b>ABE_LEFT</b><br/>Left edge.</item>
        /// <item><b>ABE_RIGHT</b><br/>Right edge. </item>
        /// <item><b>ABE_TOP</b><br/>Top edge. </item>
        /// <item><b>ABE_FLOAT</b><br/>Floating. </item>
        /// </list>
        /// Related numeric values for these are defined in <see cref="ShellApi.AppBarEdges"/>
        /// 
        /// <br/>
        /// <br/>
        /// 
        /// This member is used when sending one of these messages:<br/>
        /// <list type="bullet">
        /// <item><b>ABM_GETAUTOHIDEBAR</b></item>
        /// <item><b>ABM_SETAUTOHIDEBAR</b></item>
        /// <item><b>ABM_GETAUTOHIDEBAREX</b></item>
        /// <item><b>ABM_SETAUTOHIDEBAREX</b></item>
        /// <item><b>ABM_QUERYPOS</b></item>
        /// <item><b>ABM_SETPOS</b></item>
        /// </list>
        /// 
        /// </summary>
        public UInt32 uEdge;

        /// <summary> A RECT structure whose use varies depending on the message. </summary>
        public RECT rc;

        /// <summary> A message-dependent value. </summary>
        public Int32 lParam;

        #endregion // Fields

        #region Constructor(s)

        /// <summary>
        /// Constructor providing a handle of the appbar window.
        /// </summary>
        /// <param name="hWnd">The handle to the appbar window.</param>
        public APPBARDATA(IntPtr hWnd)
          : this()
        {
            this.cbSize = (UInt32)Marshal.SizeOf<APPBARDATA>();
            this.hWnd = hWnd;
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary>
        /// A value that specifies an edge of the screen.<br/>
        /// For more info, see the <see cref="uEdge"/> field.
        /// </summary>
        public ShellApi.AppBarEdges Edge
        {
            readonly get { return (ShellApi.AppBarEdges)uEdge; }
            set { uEdge = (uint)value; }
        }
        #endregion // Properties
    }

    /// <summary>
    /// CSIDL (constant special item ID list) values provide a unique system-independent way to identify special
    /// folders used frequently by applications, but which may not have the same name or location on any given
    /// system. For example, the system folder may be "C:\Windows" on one system and "C:\Winnt" on another.
    /// </summary>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb762494(v=vs.85).aspx">
    /// MSDN documentation of CSIDL
    /// </seealso>
    public enum CSIDL
    {
        /// <summary>
        /// Version 5.0. Combine this CSIDL with any of the following 
        /// CSIDLs to force the creation of the associated folder. 
        /// </summary>
        CSIDL_FLAG_CREATE = (0x8000),
        /// <summary>
        /// Version 5.0. The file system directory that is used to store 
        /// administrative tools for an individual user. The Microsoft 
        /// Management Console (MMC) will save customized consoles to 
        /// this directory, and it will roam with the user.
        /// </summary>
        CSIDL_ADMINTOOLS = (0x0030),
        /// <summary>
        /// The file system directory that corresponds to the user's nonlocalized Startup program group.
        /// </summary>
        CSIDL_ALTSTARTUP = (0x001d),
        /// <summary>
        /// Version 4.71. The file system directory that serves as a 
        /// common repository for application-specific data. A typical
        /// path is C:\Documents and Settings\username\Application Data. 
        /// This CSIDL is supported by the redistributable Shfolder.dll 
        /// for systems that do not have the Microsoft® Internet 
        /// Explorer 4.0 integrated Shell installed.
        /// </summary>
        CSIDL_APPDATA = (0x001a),
        /// <summary>
        /// The virtual folder containing the objects in the user's Recycle Bin.
        /// </summary>
        CSIDL_BITBUCKET = (0x000a),

        /// <summary>
        /// Version 6.0. The file system directory acting as a staging
        /// area for files waiting to be written to CD. A typical path 
        /// is C:\Documents and Settings\username\Local Settings\
        /// Application Data\Microsoft\CD Burning.
        /// </summary>
        CSIDL_CDBURN_AREA = (0x003b),
        /// <summary>
        /// Version 5.0. The file system directory containing 
        /// administrative tools for all users of the computer.
        /// </summary>
        CSIDL_COMMON_ADMINTOOLS = (0x002f),
        /// <summary>
        /// The file system directory that corresponds to the 
        /// non-localized Startup program group for all users. Valid only 
        /// for Microsoft Windows NT® systems.
        /// </summary>
        CSIDL_COMMON_ALTSTARTUP = (0x001e),
        /// <summary>
        /// Version 5.0. The file system directory containing application 
        /// data for all users. A typical path is C:\Documents and 
        /// Settings\All Users\Application Data.
        /// </summary>
        CSIDL_COMMON_APPDATA = (0x0023),
        /// <summary>
        /// The file system directory that contains files and folders 
        /// that appear on the desktop for all users. A typical path is 
        /// C:\Documents and Settings\All Users\Desktop. Valid only for 
        /// Windows NT systems.
        /// </summary>
        CSIDL_COMMON_DESKTOPDIRECTORY = (0x0019),
        /// <summary>
        /// The file system directory that contains documents that are 
        /// common to all users. A typical paths is C:\Documents and 
        /// Settings\All Users\Documents. Valid for Windows NT systems 
        /// and Microsoft Windows® 95 and Windows 98 systems with 
        /// Shfolder.dll installed.
        /// </summary>
        CSIDL_COMMON_DOCUMENTS = (0x002e),
        /// <summary>
        /// The file system directory that serves as a common repository
        /// for favorite items common to all users. Valid only for 
        /// Windows NT systems.
        /// </summary>
        CSIDL_COMMON_FAVORITES = (0x001f),
        /// <summary>
        /// Version 6.0. The file system directory that serves as a 
        /// repository for music files common to all users. A typical 
        /// path is C:\Documents and Settings\All Users\Documents\My Music.
        /// </summary>
        CSIDL_COMMON_MUSIC = (0x0035),
        /// <summary>
        /// Version 6.0. The file system directory that serves as a 
        /// repository for image files common to all users. A typical 
        /// path is C:\Documents and Settings\All Users\Documents\My Pictures.
        /// </summary>
        CSIDL_COMMON_PICTURES = (0x0036),
        /// <summary>
        /// The file system directory that contains the directories for 
        /// the common program groups that appear on the Start menu for
        /// all users. A typical path is C:\Documents and Settings\
        /// All Users\Start Menu\Programs. Valid only for Windows NT systems.
        /// </summary>
        CSIDL_COMMON_PROGRAMS = (0x0017),
        /// <summary>
        /// The file system directory that contains the programs and 
        /// folders that appear on the Start menu for all users. A 
        /// typical path is C:\Documents and Settings\All Users\
        /// Start Menu. Valid only for Windows NT systems.
        /// </summary>
        CSIDL_COMMON_STARTMENU = (0x0016),
        /// <summary>
        /// The file system directory that contains the programs that 
        /// appear in the Startup folder for all users. A typical path 
        /// is C:\Documents and Settings\All Users\Start Menu\Programs\
        /// Startup. Valid only for Windows NT systems.
        /// </summary>
        CSIDL_COMMON_STARTUP = (0x0018),
        /// <summary>
        /// The file system directory that contains the templates that 
        /// are available to all users. A typical path is C:\Documents 
        /// and Settings\All Users\Templates. Valid only for Windows NT systems.
        /// </summary>
        CSIDL_COMMON_TEMPLATES = (0x002d),
        /// <summary>
        /// Version 6.0. The file system directory that serves as a 
        /// repository for video files common to all users. A typical 
        /// path is C:\Documents and Settings\All Users\Documents\My Videos.
        /// </summary>
        CSIDL_COMMON_VIDEO = (0x0037),
        /// <summary>
        /// The virtual folder containing icons for the Control Panel applications.
        /// </summary>
        CSIDL_CONTROLS = (0x0003),
        /// <summary>
        /// The file system directory that serves as a common repository 
        /// for Internet cookies. A typical path is C:\Documents and Settings\username\Cookies.
        /// </summary>
        CSIDL_COOKIES = (0x0021),
        /// <summary>
        /// The virtual folder representing the Windows desktop, the root 
        /// of the namespace.
        /// </summary>
        CSIDL_DESKTOP = (0x0000),
        /// <summary>
        /// The file system directory used to physically store file 
        /// objects on the desktop (not to be confused with the desktop 
        /// folder itself). A typical path is C:\Documents and 
        /// Settings\username\Desktop.
        /// </summary>
        CSIDL_DESKTOPDIRECTORY = (0x0010),
        /// <summary>
        /// The virtual folder representing My Computer, containing 
        /// everything on the local computer: storage devices, printers,
        /// and Control Panel. The folder may also contain mapped 
        /// network drives.
        /// </summary>
        CSIDL_DRIVES = (0x0011),
        /// <summary>
        /// The file system directory that serves as a common repository 
        /// for the user's favorite items. A typical path is C:\Documents
        /// and Settings\username\Favorites.
        /// </summary>
        CSIDL_FAVORITES = (0x0006),
        /// <summary>
        /// A virtual folder containing fonts. A typical path is C:\Windows\Fonts.
        /// </summary>
        CSIDL_FONTS = (0x0014),
        /// <summary>
        /// The file system directory that serves as a common repositoryfor Internet history items.
        /// </summary>
        CSIDL_HISTORY = (0x0022),
        /// <summary>
        /// A virtual folder representing the Internet.
        /// </summary>
        CSIDL_INTERNET = (0x0001),
        /// <summary>
        /// Version 4.72. The file system directory that serves as a 
        /// common repository for temporary Internet files. A typical 
        /// path is C:\Documents and Settings\username\Local Settings\Temporary Internet Files.
        /// </summary>
        CSIDL_INTERNET_CACHE = (0x0020),
        /// <summary>
        /// Version 5.0. The file system directory that serves as a data
        /// repository for local (nonroaming) applications. A typical 
        /// path is C:\Documents and Settings\username\Local Settings\Application Data.
        /// </summary>
        CSIDL_LOCAL_APPDATA = (0x001c),
        /// <summary>
        /// Version 6.0. The virtual folder representing the My Documents
        /// desktop item. This should not be confused with 
        /// CSIDL_PERSONAL, which represents the file system folder that 
        /// physically stores the documents.
        /// </summary>
        CSIDL_MYDOCUMENTS = (0x000c),
        /// <summary>
        /// The file system directory that serves as a common repository 
        /// for music files. A typical path is C:\Documents and Settings\User\My Documents\My Music.
        /// </summary>
        CSIDL_MYMUSIC = (0x000d),
        /// <summary>
        /// Version 5.0. The file system directory that serves as a 
        /// common repository for image files. A typical path is 
        /// C:\Documents and Settings\username\My Documents\My Pictures.
        /// </summary>
        CSIDL_MYPICTURES = (0x0027),
        /// <summary>
        /// // Version 6.0. The file system directory that serves as a 
        /// common repository for video files. A typical path is 
        /// C:\Documents and Settings\username\My Documents\My Videos.
        /// </summary>
        CSIDL_MYVIDEO = (0x000e),
        /// <summary>
        /// A file system directory containing the link objects that may 
        /// exist in the My Network Places virtual folder. It is not the
        /// same as CSIDL_NETWORK, which represents the network namespace
        /// root. A typical path is C:\Documents and Settings\username\NetHood.
        /// </summary>
        CSIDL_NETHOOD = (0x0013),
        /// <summary>
        /// A virtual folder representing Network Neighborhood, the root
        /// of the network namespace hierarchy.
        /// </summary>
        CSIDL_NETWORK = (0x0012),
        /// <summary>
        /// The file system directory used to physically store a user's
        /// common repository of documents. A typical path is 
        /// C:\Documents and Settings\username\My Documents. This should
        /// be distinguished from the virtual My Documents folder in 
        /// the namespace, identified by CSIDL_MYDOCUMENTS. 
        /// </summary>
        CSIDL_PERSONAL = (0x0005),
        /// <summary>
        /// The virtual folder containing installed printers.
        /// </summary>
        CSIDL_PRINTERS = (0x0004),
        /// <summary>
        /// The file system directory that contains the link objects that
        /// can exist in the Printers virtual folder. A typical path is 
        /// C:\Documents and Settings\username\PrintHood.
        /// </summary>
        CSIDL_PRINTHOOD = (0x001b),
        /// <summary>
        /// Version 5.0. The user's profile folder. A typical path is 
        /// C:\Documents and Settings\username. Applications should not 
        /// create files or folders at this level; they should put their
        /// data under the locations referred to by CSIDL_APPDATA or CSIDL_LOCAL_APPDATA.
        /// </summary>
        CSIDL_PROFILE = (0x0028),

        /// <summary>
        /// Version 6.0. The file system directory containing user profile folders. 
        /// A typical path is C:\Documents and Settings.
        /// </summary>
        CSIDL_PROFILES = (0x003e),
        /// <summary>
        /// Version 5.0. The Program Files folder. A typical path is 
        /// C:\Program Files.
        /// </summary>
        CSIDL_PROGRAM_FILES = (0x0026),
        /// <summary>
        /// Version 5.0. A folder for components that are shared across 
        /// applications. A typical path is C:\Program Files\Common. 
        /// Valid only for Windows NT, Windows 2000, and Windows XP 
        /// systems. Not valid for Windows Millennium Edition (Windows Me).
        /// </summary>
        CSIDL_PROGRAM_FILES_COMMON = (0x002b),
        /// <summary>
        /// The file system directory that contains the user's program 
        /// groups (which are themselves file system directories).
        /// A typical path is C:\Documents and Settings\username\
        /// Start Menu\Programs. 
        /// </summary>
        CSIDL_PROGRAMS = (0x0002),
        /// <summary>
        /// The file system directory that contains shortcuts to the 
        /// user's most recently used documents. A typical path is 
        /// C:\Documents and Settings\username\My Recent Documents. 
        /// To create a shortcut in this folder, use SHAddToRecentDocs.
        /// In addition to creating the shortcut, this function updates
        /// the Shell's list of recent documents and adds the shortcut 
        /// to the My Recent Documents submenu of the Start menu.
        /// </summary>
        CSIDL_RECENT = (0x0008),
        /// <summary>
        /// // The file system directory that contains Send To menu items.
        /// A typical path is C:\Documents and Settings\username\SendTo.
        /// </summary>
        CSIDL_SENDTO = (0x0009),
        /// <summary>
        /// The file system directory containing Start menu items. A 
        /// typical path is C:\Documents and Settings\username\Start Menu.
        /// </summary>
        CSIDL_STARTMENU = (0x000b),
        /// <summary>
        /// The file system directory that corresponds to the user's 
        /// Startup program group. The system starts these programs 
        /// whenever any user logs onto Windows NT or starts Windows 95.
        /// A typical path is C:\Documents and Settings\username\
        /// Start Menu\Programs\Startup.
        /// </summary>
        CSIDL_STARTUP = (0x0007),
        /// <summary>
        /// Version 5.0. The Windows System folder. A typical path is C:\Windows\System32.
        /// </summary>
        CSIDL_SYSTEM = (0x0025),
        /// <summary>
        /// The file system directory that serves as a common repository
        /// for document templates. A typical path is C:\Documents and Settings\username\Templates.
        /// </summary>
        CSIDL_TEMPLATES = (0x0015),
        /// <summary>
        /// Version 5.0. The Windows directory or SYSROOT. This 
        /// corresponds to the %windir% or %SYSTEMROOT% environment 
        /// variables. A typical path is C:\Windows.
        /// </summary>
        CSIDL_WINDOWS = (0x0024),
    }

    /// <summary>
    /// A flag being used as an argument of <see cref="SHGetFolderPath"/> API.<br/>
    /// 
    /// This value is used in cases where the folder associated with a KNOWNFOLDERID (or CSIDL) can be moved,
    /// renamed, redirected, or roamed across languages by a user or administrator.<br/>
    /// 
    /// The known folder system that underlies SHGetFolderPath allows users or administrators to redirect a known
    /// folder to a location that suits their needs. This is achieved by calling IKnownFolderManager::Redirect,
    /// which sets the "current" value of the folder associated with the SHGFP_TYPE_CURRENT flag.<br/>
    /// 
    /// The default value of the folder, which is the location of the folder if a user or administrator had not
    /// redirected it elsewhere, is retrieved by specifying the SHGFP_TYPE_DEFAULT flag. This value can be used to
    /// implement a "restore defaults" feature for a known folder.<br/>
    /// 
    /// For example, the default value (SHGFP_TYPE_DEFAULT) for FOLDERID_Music (CSIDL_MYMUSIC) is "C:\Users\user
    /// name\Music". If the folder was redirected, the current value (SHGFP_TYPE_CURRENT) might be "D:\Music". If
    /// the folder has not been redirected, then SHGFP_TYPE_DEFAULT and SHGFP_TYPE_CURRENT retrieve the same
    /// path.<br/>
    /// </summary>
    /// 
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb762181(v=vs.85).aspx">
    /// MSDN documentation of SHGetFolderPath
    /// </seealso>
    public enum SHGFP_TYPE
    {
        /// <summary>
        /// Retrieve the folder's current path.
        /// </summary>
        SHGFP_TYPE_CURRENT = 0,

        /// <summary>
        /// Retrieve the folder's default path.
        /// </summary>
        SHGFP_TYPE_DEFAULT = 1
    }

    /// <summary>
    /// This enum lists attributes that can be retrieved on an item (file or folder) or set of items by methods
    /// 
    /// <list type="bullet">
    /// <item><b><see cref="IShellFolder.GetAttributesOf"/></b></item>
    /// <item><b><see cref="IShellFolder.ParseDisplayName"/></b></item>
    /// </list>
    /// </summary>
    [Flags]
    public enum SFGAO : uint
    {
        /// <summary> Objects can be copied  </summary>
        SFGAO_CANCOPY = 0x00000001,

        /// <summary> Objects can be moved  </summary>
        SFGAO_CANMOVE = 0x00000002,

        /// <summary> Objects can be linked  </summary>
        SFGAO_CANLINK = 0x00000004,

        /// <summary> supports BindToObject(IID_IStorage) </summary>
        SFGAO_STORAGE = 0x00000008,

        /// <summary> Objects can be renamed </summary>
        SFGAO_CANRENAME = 0x00000010,

        /// <summary> Objects can be deleted </summary>
        SFGAO_CANDELETE = 0x00000020,

        /// <summary> Objects have property sheets </summary>
        SFGAO_HASPROPSHEET = 0x00000040,

        /// <summary> Objects are drop target </summary>
        SFGAO_DROPTARGET = 0x00000100,

        /// <summary> This flag is a mask for the capability flags. </summary>
        SFGAO_CAPABILITYMASK = 0x00000177,

        /// <summary> The specified items are encrypted and might require special presentation, like alt color. </summary>
        SFGAO_ENCRYPTED = 0x00002000,

        /// <summary> Accessing the item (through IStream or other storage interfaces) is expected to be a slow operation. </summary>
        SFGAO_ISSLOW = 0x00004000,

        /// <summary> The specified items are shown as dimmed and unavailable to the user. </summary>
        SFGAO_GHOSTED = 0x00008000,

        /// <summary> The specified items are shortcuts (links). </summary>
        SFGAO_LINK = 0x00010000,

        /// <summary> The specified objects are shared. </summary>
        SFGAO_SHARE = 0x00020000,

        /// <summary> The specified items are read-only. In the case of folders, this means that new items cannot be created in those folders. </summary>
        SFGAO_READONLY = 0x00040000,

        /// <summary>
        /// The item is hidden and should not be displayed unless the Show hidden files and folders option is enabled in Folder Settings.
        /// </summary>
        SFGAO_HIDDEN = 0x00080000,

        /// <summary>
        /// This flag is a mask for the display attributes. Do Not use as argument value.
        /// </summary>
        SFGAO_DISPLAYATTRMASK = 0x000FC000,

        /// <summary>
        /// The specified folders are either file system folders or contain at least one descendant (child, grandchild, or later) that is a file system (SFGAO_FILESYSTEM) folder.
        /// </summary>
        SFGAO_FILESYSANCESTOR = 0x10000000,

        /// <summary> An enum constant representing the sfgao folder option. </summary>
        SFGAO_FOLDER = 0x20000000,   // support BindToObject(IID_IShellFolder)
        /// <summary> An enum constant representing the sfgao filesystem option. </summary>
        SFGAO_FILESYSTEM = 0x40000000,   // is a win32 file system object (file/folder/root)
        /// <summary> An enum constant representing the sfgao hassubfolder option. </summary>
        SFGAO_HASSUBFOLDER = 0x80000000,   // may contain children with SFGAO_FOLDER
        /// <summary> An enum constant representing the sfgao contentsmask option. </summary>
        SFGAO_CONTENTSMASK = 0x80000000, // This flag is a mask for the contents attributes.
        /// <summary> An enum constant representing the sfgao validate option. </summary>
        SFGAO_VALIDATE = 0x01000000,   // invalidate cached information
        /// <summary> An enum constant representing the sfgao removable option. </summary>
        SFGAO_REMOVABLE = 0x02000000,   // is this removeable media?
        /// <summary> An enum constant representing the sfgao compressed option. </summary>
        SFGAO_COMPRESSED = 0x04000000,   // Object is compressed (use alt color)
        /// <summary> An enum constant representing the sfgao browsable option. </summary>
        SFGAO_BROWSABLE = 0x08000000,   // supports IShellFolder, but only implements CreateViewObject() (non-folder view)
        /// <summary> An enum constant representing the sfgao nonenumerated option. </summary>
        SFGAO_NONENUMERATED = 0x00100000,   // is a non-enumerated object
        /// <summary> An enum constant representing the sfgao newcontent option. </summary>
        SFGAO_NEWCONTENT = 0x00200000,   // should show bold in explorer tree
        /// <summary> An enum constant representing the sfgao canmoniker option. </summary>
        SFGAO_CANMONIKER = 0x00400000,   // defunct
        /// <summary> An enum constant representing the sfgao hasstorage option. </summary>
        SFGAO_HASSTORAGE = 0x00400000,   // defunct
        /// <summary> An enum constant representing the sfgao stream option. </summary>
        SFGAO_STREAM = 0x00400000,   // supports BindToObject(IID_IStream)
        /// <summary> An enum constant representing the sfgao storageancestor option. </summary>
        SFGAO_STORAGEANCESTOR = 0x00800000,   // may contain children with SFGAO_STORAGE or SFGAO_STREAM
        /// <summary> An enum constant representing the sfgao storagecapmask option. </summary>
        SFGAO_STORAGECAPMASK = 0x70C50008    // for determining storage capabilities, ie for open/save semantics
    }

    /// <summary> Bitfield of flags for specifying SHCONTF. </summary>
    [Flags]
    public enum SHCONTF
    {
        /// <summary> A binary constant representing the shcontf folders flag. </summary>
        SHCONTF_FOLDERS = 0x0020,   // only want folders enumerated (SFGAO_FOLDER)
        /// <summary> A binary constant representing the shcontf nonfolders flag. </summary>
        SHCONTF_NONFOLDERS = 0x0040,   // include non folders
        /// <summary> A binary constant representing the shcontf includehidden flag. </summary>
        SHCONTF_INCLUDEHIDDEN = 0x0080,   // show items normally hidden
        /// <summary> A binary constant representing the shcontf initialise on first next flag. </summary>
        SHCONTF_INIT_ON_FIRST_NEXT = 0x0100,   // allow EnumObject() to return before validating enum
        /// <summary> A binary constant representing the shcontf netprintersrch flag. </summary>
        SHCONTF_NETPRINTERSRCH = 0x0200,   // hint that client is looking for printers
        /// <summary> A binary constant representing the shcontf shareable flag. </summary>
        SHCONTF_SHAREABLE = 0x0400,   // hint that client is looking sharable resources (remote shares)
        /// <summary> A binary constant representing the shcontf storage flag. </summary>
        SHCONTF_STORAGE = 0x0800,   // include all items with accessible storage and their ancestors
    }

    /// <summary> Values that represent SHCIDS. </summary>
    public enum SHCIDS : uint
    {
        /// <summary> An enum constant representing the shcids allfields option. </summary>
        SHCIDS_ALLFIELDS = 0x80000000, // Compare all the information contained in the ITEMIDLIST 
        /// <summary> An enum constant representing the shcids canonicalonly option. </summary>
        // structure, not just the display names
        SHCIDS_CANONICALONLY = 0x10000000, // When comparing by name, compare the system names but not the 
        /// <summary> An enum constant representing the shcids bitmask option. </summary>
        // display names. 
        SHCIDS_BITMASK = 0xFFFF0000,
        /// <summary> An enum constant representing the shcids columnmask option. </summary>
        SHCIDS_COLUMNMASK = 0x0000FFFF
    }

    /// <summary> Bitfield of flags for specifying SHGNO. </summary>
    [Flags]
    public enum SHGNO
    {
        /// <summary> A binary constant representing the shgdn normal flag. </summary>
        SHGDN_NORMAL = 0x0000,    // default (display purpose)
        /// <summary> A binary constant representing the shgdn infolder flag. </summary>
        SHGDN_INFOLDER = 0x0001,    // displayed under a folder (relative)
        /// <summary> A binary constant representing the shgdn forediting flag. </summary>
        SHGDN_FOREDITING = 0x1000,    // for in-place editing
        /// <summary> A binary constant representing the shgdn foraddressbar flag. </summary>
        SHGDN_FORADDRESSBAR = 0x4000,    // UI friendly parsing name (remove ugly stuff)
        /// <summary> A binary constant representing the shgdn forparsing flag. </summary>
        SHGDN_FORPARSING = 0x8000   // parsing name for ParseDisplayName()
    }

    /// <summary> Values that represent STRRET_TYPE. </summary>
    public enum STRRET_TYPE
    {
        /// <summary> An enum constant representing the strret wstr option. </summary>
        STRRET_WSTR = 0x0000,      // Use STRRET.pOleStr
        /// <summary> An enum constant representing the strret offset option. </summary>
        STRRET_OFFSET = 0x0001,      // Use STRRET.uOffset to Ansi
        /// <summary> An enum constant representing the strret cstr option. </summary>
        STRRET_CSTR = 0x0002     // Use STRRET.cStr
    }

    /// <summary> Values that represent PrinterActions. </summary>
    public enum PrinterActions
    {
        /// <summary> An enum constant representing the printaction open option. </summary>
        PRINTACTION_OPEN = 0,   // The printer specified by the name in lpBuf1 will be opened. 
        /// <summary> An enum constant representing the printaction properties option. </summary>
        // lpBuf2 is ignored. 
        PRINTACTION_PROPERTIES = 1, // The properties for the printer specified by the name in lpBuf1
        /// <summary> An enum constant representing the printaction netinstall option. </summary>
        // will be displayed. lpBuf2 can either be NULL or specify.
        PRINTACTION_NETINSTALL = 2, // The network printer specified by the name in lpBuf1 will be 
        /// <summary> An enum constant representing the printaction netinstalllink option. </summary>
        // installed. lpBuf2 is ignored. 
        PRINTACTION_NETINSTALLLINK = 3, // A shortcut to the network printer specified by the name in lpBuf1
        /// <summary> An enum constant representing the printaction testpage option. </summary>
        // will be created. lpBuf2 specifies the drive and path of the folder 
        // in which the shortcut will be created. The network printer must 
        // have already been installed on the local computer. 
        PRINTACTION_TESTPAGE = 4,   // A test page will be printed on the printer specified by the name
        /// <summary> An enum constant representing the printaction opennetprn option. </summary>
        // in lpBuf1. lpBuf2 is ignored. 
        PRINTACTION_OPENNETPRN = 5, // The network printer specified by the name in lpBuf1 will be
        /// <summary> An enum constant representing the printaction documentdefaults option. </summary>
        // opened. lpBuf2 is ignored. 
        PRINTACTION_DOCUMENTDEFAULTS = 6,   // Microsoft® Windows NT® only. The default document properties for
        /// <summary> An enum constant representing the printaction serverproperties option. </summary>
        // the printer specified by the name in lpBuf1 will be displayed. 
        // lpBuf2 is ignored. 
        PRINTACTION_SERVERPROPERTIES = 7        // Windows NT only. The properties for the server of the printer 
                                                // specified by the name in lpBuf1 will be displayed. lpBuf2 
                                                // is ignored.
    }
    #endregion // Typedefs

    #region Methods

    /// <summary> Retrieves a pointer to the Shell's <see cref="PK.PkUtils.ShellLib.IMalloc"/> interface. </summary>
    ///
    /// <param name="hObject"> [out] Address of a pointer that receives the Shell's IMalloc interface pointer. </param>
    ///
    /// <returns>
    /// If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
    /// </returns>
    [DllImport("shell32.dll")]
    public static extern Int32 SHGetMalloc(
      out IntPtr hObject);

    /// <summary> Retrieves the path of a folder as an ITEMIDLIST structure. </summary>
    ///
    /// <param name="hwndOwner">  Handle to the owner window. </param>
    /// <param name="nFolder">    A CSIDL value that identifies the folder to be located. </param>
    /// <param name="hToken">     Token that can be used to represent a particular user. </param>
    /// <param name="dwReserved"> Reserved. </param>
    /// <param name="ppidl">      [out] Address of a pointer to an item identifier list structure specifying the
    /// folder's location relative to the root of the namespace (the desktop) </param>
    ///
    /// <returns>
    /// If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
    /// </returns>
    [DllImport("shell32.dll")]
    public static extern Int32 SHGetFolderLocation(
      IntPtr hwndOwner,
      Int32 nFolder,
      IntPtr hToken,
      UInt32 dwReserved,
      out IntPtr ppidl);

    /// <summary> Converts an item identifier list to a file system path. </summary>
    ///
    /// <param name="pidl">    Address of an item identifier list that specifies a file or directory location
    /// relative to the root of the namespace (the desktop). </param>
    /// <param name="pszPath">  Address of a buffer to receive the file system path. </param>
    ///
    /// <returns> Returns non-zero ( TRUE ) if successful; otherwise zero ( FALSE ). </returns>
    [DllImport("shell32.dll", BestFitMapping = false, ThrowOnUnmappableChar = true)]
    public static extern Int32 SHGetPathFromIDList(
      IntPtr pidl,
      /* do not - WRONG! [MarshalAs(UnmanagedType.LPWStr)] */
      [MarshalAs(UnmanagedType.LPStr)] StringBuilder pszPath);

    /// <summary> Takes the CSIDL of a folder and returns the pathname. </summary>
    ///
    /// <param name="hwndOwner">  Handle to an owner window. </param>
    /// <param name="nFolder">    A CSIDL value that identifies the folder whose path is to be retrieved. </param>
    /// <param name="hToken">     An access token that can be used to represent a particular user. </param>
    /// <param name="dwFlags">   Flags to specify which path is to be returned. It is used for cases where the
    /// folder associated with a CSIDL may be moved or renamed by the user. </param>
    /// <param name="pszPath">    Pointer to a null-terminated string which will receive the path. </param>
    ///
    /// <returns> 
    ///  If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
    /// </returns>
    [DllImport("shell32.dll", BestFitMapping = false, ThrowOnUnmappableChar = true)]
    public static extern Int32 SHGetFolderPath(
      IntPtr hwndOwner,
      Int32 nFolder,
      IntPtr hToken,
      UInt32 dwFlags,
      /* do not - WRONG! [MarshalAs(UnmanagedType.LPWStr)] */
      [MarshalAs(UnmanagedType.LPStr)] StringBuilder pszPath);

    /// <summary>
    /// Translates a Shell namespace object's display name into an item identifier list and returns the attributes
    /// of the object. This function is the preferred method to convert a string to a pointer to an item identifier
    /// list (PIDL).
    /// </summary>
    ///
    /// <param name="pszName">   Pointer to a zero-terminated wide string that contains the display name to parse. </param>
    /// <param name="pbc">       Optional bind context that controls the parsing operation. This parameter is
    /// normally set to NULL. </param>
    /// <param name="ppidl">     [out] Address of a pointer to a variable of type ITEMIDLIST that receives the
    /// item identifier list for the object. </param>
    /// <param name="sfgaoIn">    ULONG value that specifies the attributes to query. </param>
    /// <param name="psfgaoOut"> [out] Pointer to a ULONG. On return, those attributes that are true for the  
    /// object and were requested in sfgaoIn will be set. </param>
    ///
    /// <returns> 
    /// If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
    /// </returns>
    [DllImport("shell32.dll")]
    public static extern Int32 SHParseDisplayName(
      [MarshalAs(UnmanagedType.LPWStr)]
  String pszName,
      IntPtr pbc,
      out IntPtr ppidl,
      UInt32 sfgaoIn,
      out UInt32 psfgaoOut);

    /// <summary>
    /// Retrieves the IShellFolder interface for the desktop folder, which is the root of the Shell's namespace.
    /// </summary>
    ///
    /// <param name="ppshf"> [out] Address that receives an IShellFolder interface pointer for the desktop folder. </param>
    ///
    /// <returns> 
    /// If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
    /// </returns>
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern Int32 SHGetDesktopFolder(
      out IShellFolder ppshf);

    /// <summary>
    /// This function takes the fully-qualified pointer to an item identifier list (PIDL) of a namespace object,
    /// and returns a specified interface pointer on the parent object.
    /// </summary>
    ///
    /// <param name="pidl">       The item's PIDL. </param>
    /// <param name="riid">       The REFIID of one of the interfaces exposed by the item's parent object. </param>
    /// <param name="ppv">       [out] A pointer to the interface specified by riid. You must release the object
    /// when you are finished. </param>
    /// <param name="ppidlLast"> [in,out] The item's PIDL relative to the parent folder. This PIDL can be used
    /// with many of the methods supported by the parent folder's interfaces. If you set ppidlLast to NULL, the
    /// PIDL will not be returned. </param>
    ///
    /// <returns> 
    /// If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
    /// </returns>
    [DllImport("shell32.dll")]
    public static extern Int32 SHBindToParent(
      IntPtr pidl,
      [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
      out IntPtr ppv,
      ref IntPtr ppidlLast);

    /// <summary>
    /// Accepts a STRRET structure returned by IShellFolder::GetDisplayNameOf that contains or points to a string,
    /// and then returns that string as a BSTR.
    /// </summary>
    ///
    /// <param name="pstr">   [in,out] Pointer to a STRRET structure. </param>
    /// <param name="pidl">  Pointer to an ITEMIDLIST uniquely identifying a file object or subfolder relative to
    /// the parent folder. </param>
    /// <param name="pbstr">  [out] Pointer to a variable of type BSTR that contains the converted string. </param>
    ///
    /// <returns> 
    /// If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
    /// </returns>
    [DllImport("shlwapi.dll")]
    public static extern Int32 StrRetToBSTR(
      ref STRRET pstr,
      IntPtr pidl,
      [MarshalAs(UnmanagedType.BStr)] out String pbstr);

    /// <summary>
    /// Takes a STRRET structure returned by IShellFolder::GetDisplayNameOf, converts it to a string, and places
    /// the result in a buffer.
    /// </summary>
    ///
    /// <param name="pstr">   [in,out] Pointer to the STRRET structure. When the function returns, this pointer
    /// will no longer be valid. </param>
    /// <param name="pidl">   Pointer to the item's ITEMIDLIST structure. </param>
    /// <param name="pszBuf"> Buffer to hold the display name. It will be returned as a null-terminated string. If
    /// cchBuf is too small, the name will be truncated to fit. </param>
    /// <param name="cchBuf"> Size of pszBuf, in characters. If cchBuf is too small, the string will be truncated
    /// to fit. </param>
    ///
    /// <returns> 
    /// If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
    /// </returns>
    [DllImport("shlwapi.dll", BestFitMapping = false, ThrowOnUnmappableChar = true)]
    public static extern Int32 StrRetToBuf(
      ref STRRET pstr,
      IntPtr pidl,
      /* do not - WRONG! [MarshalAs(UnmanagedType.LPWStr)] */
      [MarshalAs(UnmanagedType.LPStr)] StringBuilder pszBuf,
      UInt32 cchBuf);

    /// <summary> Displays a dialog box that enables the user to select a Shell folder. </summary>
    ///
    /// <param name="lbpi"> [in,out] Pointer to a BROWSEINFO structure that contains information used to display
    /// the dialog box. </param>
    ///
    /// <returns>
    /// Returns a PIDL that specifies the location of the selected folder relative to the root of the namespace. If
    /// the user chooses the Cancel button in the dialog box, the return value is zero (IntPtr.Zero). <br/>
    /// 
    /// It is possible that the PIDL returned is that of a folder shortcut rather than a folder. For a full
    /// discussion of this case, see
    /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb762115(v=vs.85).aspx">
    /// MSDN documentation of SHBrowseForFolder
    /// </see>
    /// </returns>
    [DllImport("shell32.dll")]
    public static extern IntPtr SHBrowseForFolder(
      ref BROWSEINFO lbpi);

    /// <summary> Performs an operation on a specified file. </summary>
    ///
    /// <remarks>
    /// For possible error return codes, see values of enum
    /// <see cref="PK.PkUtils.ShellLib.ShellExecute.ShellExecuteReturnCodes"/>.<br/>
    /// For possible verbs used as a second argument, see string constants defined in
    /// <see cref="PK.PkUtils.ShellLib.ShellExecute"/>.<br/>
    /// </remarks>
    ///
    /// <param name="hwnd">         Handle to a parent window. </param>
    /// <param name="lpOperation">  Pointer to a null-terminated string, referred to in this case as a verb that
    /// specifies the action to be performed. </param>
    /// <param name="lpFile">       Pointer to a null-terminated string that specifies the file or object on which
    /// to execute the specified verb. </param>
    /// <param name="lpParameters"> Options for controlling the operation.  If the lpFile parameter specifies an
    /// executable file, lpParameters is a pointer to a null-terminated string that specifies the parameters to be
    /// passed to the application. </param>
    /// <param name="lpDirectory">  Pointer to a null-terminated string that specifies the default directory. </param>
    /// <param name="nShowCmd">     Flags that specify how an application is to be displayed when it is opened. </param>
    ///
    /// <returns>
    /// If the function succeeds, it returns a value greater than 32. If the function fails, it returns an error
    /// value that indicates the cause of the failure.
    /// </returns>
    [DllImport("shell32.dll", BestFitMapping = false, ThrowOnUnmappableChar = true)]
    public static extern IntPtr ShellExecute(
      IntPtr hwnd,
      [MarshalAs(UnmanagedType.LPStr)] String lpOperation,
      [MarshalAs(UnmanagedType.LPStr)] String lpFile,
      [MarshalAs(UnmanagedType.LPStr)] String lpParameters,
      [MarshalAs(UnmanagedType.LPStr)] String lpDirectory,
      Int32 nShowCmd);

    /// <summary> Performs an action on a file. </summary>
    ///
    /// <param name="lpExecInfo"> [in,out] Address of a SHELLEXECUTEINFO structure that contains and receives
    /// information about the application being executed. </param>
    ///
    /// <returns>
    /// Nonzero indicates success. Zero indicates failure. To get extended error information, call
    /// Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("shell32.dll")]
    public static extern Int32 ShellExecuteEx(
      ref SHELLEXECUTEINFO lpExecInfo);

    /// <summary> Copies, moves, renames, or deletes a file system object. </summary>
    ///
    /// <param name="lpFileOp"> [in,out] Address of an SHFILEOPSTRUCT structure that contains information this
    /// function needs to carry out the specified operation. This parameter must contain a valid value that is not
    /// NULL. You are responsible for validating the value. If you do not validate it, you will experience
    /// unexpected results. </param>
    ///
    /// <returns>
    /// Returns zero if successful; otherwise nonzero. Applications normally should simply check for zero or
    /// nonzero.
    /// </returns>
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern Int32 SHFileOperation(
      ref SHFILEOPSTRUCT lpFileOp);

    /// <summary>
    /// Notifies the system of an event that an application has performed. An application should use this function
    /// if it performs an action that may affect the Shell.
    /// </summary>
    ///
    /// <param name="wEventId"> Describes the event that has occurred. The ShellChangeNotificationEvents enum
    /// contains a list of options. </param>
    /// <param name="uFlags">   Flags that indicate the meaning of the dwItem1 and dwItem2 parameters. </param>
    /// <param name="dwItem1">  First event-dependent value. </param>
    /// <param name="dwItem2">  Second event-dependent value. </param>
    [DllImport("shell32.dll")]
    public static extern void SHChangeNotify(
      UInt32 wEventId,
      UInt32 uFlags,
      IntPtr dwItem1,
      IntPtr dwItem2);

    /// <summary>
    /// Adds a document to the Shell's list of recently used documents or clears all documents from the list.
    /// </summary>
    /// <remarks>
    /// The usage statistics gathered through calls to this method are used to determine lists of items accessed
    /// most recently and most frequently. These lists are seen in the Start menu and, in Windows 7 and later, in
    /// an application's Jump List.
    /// </remarks>
    ///
    /// <param name="uFlags"> Flags that indicate the meaning of the of the pv parameter. </param>
    /// <param name="pv">     A pointer to either a null-terminated string with the path and file name of the
    /// document, or a PIDL that identifies the document's file object. Set this parameter to NULL to clear all
    /// documents from the list. </param>
    [DllImport("shell32.dll")]
    public static extern void SHAddToRecentDocs(
      UInt32 uFlags,
      IntPtr pv);

    /// <summary>
    /// Adds a document to the Shell's list of recently used documents or clears all documents from the list.
    /// </summary>
    ///
    /// <param name="uFlags"> Flags that indicate the meaning of the of the pv parameter. </param>
    /// <param name="pv">     A pointer to either a null-terminated string with the path and file name of the
    /// document, or a PIDL that identifies the document's file object. Set this parameter to NULL to clear all
    /// documents from the list. </param>
    [DllImport("shell32.dll")]
    public static extern void SHAddToRecentDocs(
      UInt32 uFlags,
      [MarshalAs(UnmanagedType.LPWStr)] String pv);

    /// <summary> Executes a command on a printer object. </summary>
    ///
    /// <param name="hwnd">    Handle of the window that will be used as the parent of any windows or dialog boxes
    /// that are created during the operation. </param>
    /// <param name="uAction"> A value that determines the type of printer operation that will be performed. </param>
    /// <param name="lpBuf1">  Address of a null_terminated string that contains additional information for the
    /// printer command. </param>
    /// <param name="lpBuf2">  Address of a null-terminated string that contains additional information for the
    /// printer command. </param>
    /// <param name="fModal">  Value that determines whether SHInvokePrinterCommand should return after
    /// initializing the command or wait until the command is completed. </param>
    ///
    /// <returns> Returns non-zero if successful; otherwise zero.. </returns>
    [DllImport("shell32.dll")]
    public static extern Int32 SHInvokePrinterCommand(
      IntPtr hwnd,
      UInt32 uAction,
      [MarshalAs(UnmanagedType.LPWStr)] String lpBuf1,
      [MarshalAs(UnmanagedType.LPWStr)] String lpBuf2,
      Int32 fModal);

    /// <summary> Sends an appbar message to the system. </summary>
    ///
    /// <param name="dwMessage">  Appbar message value to send. </param>
    /// <param name="pData">     [in,out] Address of an APPBARDATA structure. The content of the structure depends
    /// on the value set in the dwMessage parameter. </param>
    ///
    /// <returns>
    /// This function returns a message-dependent value. For more information, see the Windows SDK documentation
    /// for the specific appbar message sent.
    /// </returns>
    [DllImport("shell32.dll")]
    public static extern IntPtr SHAppBarMessage(
      UInt32 dwMessage,
      ref APPBARDATA pData);

    /// <summary>
    /// The RegisterWindowMessage function defines a new window message that is guaranteed to be unique throughout
    /// the system. The message value can be used when sending or posting messages.
    /// </summary>
    ///
    /// <param name="lpString"> The registered message name. </param>
    ///
    /// <returns>
    /// If the message is successfully registered, the return value is a message identifier in the range 0xC000
    /// through 0xFFFF. If the function fails, the return value is zero. To get extended error information, call
    /// Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("user32.dll")]
    public static extern uint RegisterWindowMessage(
      [MarshalAs(UnmanagedType.LPWStr)] string lpString);

    /// <summary> Gets h result code. </summary>
    ///
    /// <param name="hr"> The hr. </param>
    ///
    /// <returns> The h result code. </returns>
    public static Int16 GetHResultCode(Int32 hr)
    {
        hr &= 0x0000ffff;
        return (Int16)hr;
    }
    #endregion // Methods
}

#pragma warning restore CA1401
#pragma warning restore CA1069
#pragma warning restore SYSLIB1054
#pragma warning restore IDE0079
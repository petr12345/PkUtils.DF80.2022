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

namespace PK.PkUtils.ShellLib;

/// <summary>
/// Wrapper around the function <see cref="ShellApi.ShellExecute"/>.
/// </summary>
public class ShellExecute
{
    #region Typedefs

    /// <summary>
    /// Commands that specify how an application window is to be displayed
    /// </summary>
    public enum ShowWindowCommands
    {
        /// <summary>
        /// Hides the window and activates another window.
        /// </summary>
        SW_HIDE = 0,

        /// <summary>
        /// Activates and displays a window. If the window is minimized or maximized, 
        /// the system restores it to its original size and position. 
        /// An application should specify this flag when displaying the window for the first time.
        /// </summary>
        SW_SHOWNORMAL = 1,

        /// <summary>
        /// The same as SW_SHOWNORMAL
        /// </summary>
        SW_NORMAL = 1,

        /// <summary>
        /// Activates the window and displays it as a minimized window.
        /// </summary>
        SW_SHOWMINIMIZED = 2,

        /// <summary>
        /// Maximizes the specified window.
        /// </summary>
        SW_SHOWMAXIMIZED = 3,

        /// <summary>
        /// The same as SW_SHOWMAXIMIZED.
        /// Activates the window and displays it as a maximized window.
        /// </summary>
        SW_MAXIMIZE = 3,

        /// <summary>
        /// Displays a window in its most recent size and position. The active window remains active.
        /// </summary>
        SW_SHOWNOACTIVATE = 4,

        /// <summary>
        /// Activates the window and displays it in its current size and position.
        /// </summary>
        SW_SHOW = 5,

        /// <summary>
        /// Minimizes the specified window and activates the next top-level window in the z-order.
        /// </summary>
        SW_MINIMIZE = 6,

        /// <summary>
        /// Displays the window as a minimized window. The active window remains active.
        /// </summary>
        SW_SHOWMINNOACTIVE = 7,

        /// <summary>
        /// Displays the window in its current state. The active window remains active.
        /// </summary>
        SW_SHOWNA = 8,

        /// <summary>
        /// Activates and displays the window.
        /// </summary>
        SW_RESTORE = 9,

        /// <summary>
        /// Sets the show state based on the information specified in the STARTUPINFO structure 
        /// passed to the CreateProcess function that started the application. 
        /// An application should call ShowWindow with this flag to set the initial visual state of its main window.
        /// </summary>
        SW_SHOWDEFAULT = 10,

        /// <summary> Minimizes a window, even if the thread that owns the window is not responding. 
        /// This flag should only be used when minimizing windows from a different thread. 
        /// </summary>
        SW_FORCEMINIMIZE = 11,
    }

    /// <summary>
    /// If the function <see cref="ShellApi.ShellExecute"/> succeeds, it returns a value greater than 32. 
    /// If the function fails, it returns an error value that indicates the cause of the failure.
    /// This enum defines all such possible error codes.
    /// </summary>
    /// <seealso cref="ShellApi.ShellExecute"/>
    public enum ShellExecuteReturnCodes
    {
        /// <summary>
        /// The operating system is out of memory or resources.
        /// </summary>
        ERROR_OUT_OF_MEMORY = 0,
        /// <summary>
        /// The specified file was not found. 
        /// </summary>
        ERROR_FILE_NOT_FOUND = 2,
        /// <summary>
        /// The specified path was not found. 
        /// </summary>
        ERROR_PATH_NOT_FOUND = 3,
        /// <summary>
        /// The .exe file is invalid (non-Microsoft Win32® .exe or error in .exe image). 
        /// </summary>
        ERROR_BAD_FORMAT = 11,
        /// <summary>
        /// The operating system denied access to the specified file.  
        /// </summary>
        SE_ERR_ACCESSDENIED = 5,
        /// <summary>
        /// The file name association is incomplete or invalid. 
        /// </summary>
        SE_ERR_ASSOCINCOMPLETE = 27,
        /// <summary>
        /// The Dynamic Data Exchange (DDE) transaction could not be completed because other DDE transactions were being processed. 
        /// </summary>
        SE_ERR_DDEBUSY = 30,
        /// <summary>
        /// The DDE transaction failed. 
        /// </summary>
        SE_ERR_DDEFAIL = 29,
        /// <summary>
        /// The DDE transaction could not be completed because the request timed out. 
        /// </summary>
        SE_ERR_DDETIMEOUT = 28,
        /// <summary>
        /// The specified dynamic-link library (DLL) was not found.  
        /// </summary>
        SE_ERR_DLLNOTFOUND = 32,
        /// <summary>
        /// The specified file was not found.  
        /// </summary>
        SE_ERR_FNF = 2,
        /// <summary>
        /// There is no application associated with the given file name extension. This error will also be returned if you attempt to print a file that is not printable. 
        /// </summary>
        SE_ERR_NOASSOC = 31,
        /// <summary>
        /// There was not enough memory to complete the operation. 
        /// </summary>
        SE_ERR_OOM = 8,
        /// <summary>
        /// The specified path was not found. 
        /// </summary>
        SE_ERR_PNF = 3,
        /// <summary>
        /// A sharing violation occurred. 
        /// </summary>
        SE_ERR_SHARE = 26,
    }

    /// <summary>
    /// Flags that indicate the content and validity of the fields of <see cref="ShellApi.SHELLEXECUTEINFO"/>
    /// structure.<br/>
    /// Their value combination should be used to fill-in SHELLEXECUTEINFO.fMask field.
    /// </summary>
    [Flags]
    public enum ShellExecuteFlags
    {
        /// <summary>
        /// Use the class name given by the lpClass member. 
        /// </summary>
        SEE_MASK_CLASSNAME = 0x00000001,
        /// <summary>
        /// Use the class key given by the hkeyClass member.
        /// </summary>
        SEE_MASK_CLASSKEY = 0x00000003,
        /// <summary>
        /// Use the item identifier list given by the lpIDList member. 
        /// The lpIDList member must point to an ITEMIDLIST structure.
        /// </summary>
        SEE_MASK_IDLIST = 0x00000004,
        /// <summary>
        /// Use the IContextMenu interface of the selected item's shortcut menu handler.
        /// </summary>
        SEE_MASK_INVOKEIDLIST = 0x0000000c,
        /// <summary>
        /// Use the icon given by the hIcon member.
        /// </summary>
        SEE_MASK_ICON = 0x00000010,
        /// <summary>
        /// Use the hot key given by the dwHotKey member.
        /// </summary>
        SEE_MASK_HOTKEY = 0x00000020,
        /// <summary>
        /// Use to indicate that the hProcess member receives the process handle. 
        /// </summary>
        SEE_MASK_NOCLOSEPROCESS = 0x00000040,
        /// <summary>
        /// Validate the share and connect to a drive letter.
        /// </summary>
        SEE_MASK_CONNECTNETDRV = 0x00000080,
        /// <summary>
        /// Wait for the Dynamic Data Exchange (DDE) conversation to terminate before returning
        /// </summary>
        SEE_MASK_FLAG_DDEWAIT = 0x00000100,
        /// <summary>
        /// Expand any environment variables specified in the string 
        /// given by the lpDirectory or lpFile member. 
        /// </summary>
        SEE_MASK_DOENVSUBST = 0x00000200,
        /// <summary>
        /// Do not display an error message box if an error occurs. 
        /// </summary>
        SEE_MASK_FLAG_NO_UI = 0x00000400,
        /// <summary>
        /// Use this flag to indicate a Unicode application.
        /// </summary>
        SEE_MASK_UNICODE = 0x00004000,
        /// <summary>
        /// Use to create a console for the new process instead of having it inherit the parent's console.
        /// </summary>
        SEE_MASK_NO_CONSOLE = 0x00008000,
        /// <summary>
        /// The execution can be performed on a background thread and the call should return immediately 
        /// without waiting for the background thread to finish. Note that in certain cases ShellExecuteEx ignores this flag 
        /// and waits for the process to finish before returning.
        /// </summary>
        SEE_MASK_ASYNCOK = 0x00100000,
        /// <summary>
        /// Use this flag when specifying a monitor on multi-monitor systems.
        /// </summary>
        SEE_MASK_HMONITOR = 0x00200000,
        /// <summary>
        /// Not used.
        /// </summary>
        SEE_MASK_NOQUERYCLASSSTORE = 0x01000000,
        /// <summary>
        /// After the new process is created, wait for the process to become idle before returning, 
        /// with a one minute timeout. See WaitForInputIdle for more details.
        /// </summary>
        SEE_MASK_WAITFORINPUTIDLE = 0x02000000,
        /// <summary>
        /// Keep track of the number of times this application has been launched. 
        /// </summary>
        SEE_MASK_FLAG_LOG_USAGE = 0x04000000,
    }
    #endregion // Typedefs

    #region Fields

    #region Common_verbs

    /// <summary>
    /// If used as second argument of <see cref="PK.PkUtils.ShellLib.ShellApi.ShellExecute"/>, 
    /// Opens the file specified by the lpFile parameter. The file can be an executable file, a document file, or a folder.
    /// </summary>
    public const string OpenFile = "open";

    /// <summary>
    /// If used as second argument of <see cref="PK.PkUtils.ShellLib.ShellApi.ShellExecute"/>, 
    /// launches an editor and opens the document for editing.
    /// If lpFile is not a document file, the function will fail.
    /// </summary>
    public const string EditFile = "edit";

    /// <summary>
    /// If used as second argument of <see cref="PK.PkUtils.ShellLib.ShellApi.ShellExecute"/>, 
    /// explores the folder specified by lpFile.
    /// </summary>
    public const string ExploreFolder = "explore";

    /// <summary>
    /// If used as second argument of <see cref="PK.PkUtils.ShellLib.ShellApi.ShellExecute"/>, 
    /// initiates a search starting from the specified directory.
    /// </summary>
    public const string FindInFolder = "find";

    /// <summary>
    /// If used as second argument of <see cref="PK.PkUtils.ShellLib.ShellApi.ShellExecute"/>, 
    /// Prints the document file specified by lpFile. If lpFile is not a document file, the function will fail.
    /// </summary>
    public const string PrintFile = "print";

    #endregion // Common_verbs

    /// <summary>
    /// Handle to the owner (parent) window.
    /// </summary>
    public IntPtr OwnerHandle;

    /// <summary>
    /// The requested operation to make on the file. <br/>
    /// Will be used as a second argument of the function <see cref="ShellApi.ShellExecute"/>.
    /// </summary>
    public string Verb;                 // The requested operation to make on the file

    /// <summary>
    /// String that specifies the file or object on which to execute the specified verb.<br/>
    /// Will be used as third argument of the function <see cref="ShellApi.ShellExecute"/>.
    /// </summary>
    public string Path;

    /// <summary>
    /// String that specifies the parameters to be passed to the application.<br/>
    /// Will be used as fourth argument of the function <see cref="ShellApi.ShellExecute"/>.
    /// </summary>
    public string Parameters;

    /// <summary>
    /// Specifies the default directory.<br/>
    /// Will be used as fifth argument of the function <see cref="ShellApi.ShellExecute"/>.
    /// </summary>
    public string WorkingFolder;

    /// <summary>
    /// Enum value that specifies how the given application window will be be displayed when it is opened.
    /// Will be used as sixth argument of the function <see cref="ShellApi.ShellExecute"/>.
    /// </summary>
    public ShowWindowCommands ShowMode;

    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default argument-less constructor.
    /// </summary>
    public ShellExecute()
      : this(IntPtr.Zero, OpenFile, string.Empty)
    {
    }

    /* commented-out to prevent confusion
    /// <summary>
    /// Constructor providing the values of <see cref="OwnerHandle"/> and <see cref="Path"/> as arguments.
    /// </summary>
    /// <param name="ownerHandle">Handle to the owner (parent) window.</param>
    /// <param name="path">String that specifies the file or object on which to execute the specified verb.</param>
    public ShellExecute(IntPtr ownerHandle, string path)
      : this(ownerHandle, OpenFile, path)
    {
    }
    */

    /// <summary>
    /// Constructor providing the values of <see cref="OwnerHandle"/>, <see cref="Verb"/> and <see cref="Path"/> as
    /// arguments.
    /// </summary>
    ///
    /// <param name="ownerHandle">  Handle to the owner (parent) window. </param>
    /// <param name="verb">         The requested operation to make on the file. </param>
    /// <param name="path">        String that specifies the file or object on which to execute the specified
    /// verb. </param>
    public ShellExecute(IntPtr ownerHandle, string verb, string path)
    {
        OwnerHandle = ownerHandle;
        Verb = verb;
        Path = path;

        Parameters = "";
        WorkingFolder = "";
        ShowMode = ShowWindowCommands.SW_SHOWNORMAL;
    }
    #endregion // Constructor(s)

    #region Methods

    /// <summary>
    /// Executes the function <see cref="ShellApi.ShellExecute"/>.
    /// </summary>
    /// <returns>True on success, false on failure.</returns>
    public bool Execute()
    {
        int iRetVal;
        iRetVal = (int)ShellApi.ShellExecute(
          OwnerHandle,
          Verb,
          Path,
          Parameters,
          WorkingFolder,
          (int)ShowMode);

        return (iRetVal > 32);
    }
    #endregion // Methods
}
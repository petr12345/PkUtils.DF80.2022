/***************************************************************************************************************
*
* FILE NAME:   .\ShellLib\ShellBrowseForFolderDialog.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class ShellBrowseForFolderDialog
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


// Ignore Spelling: pidl, Sel, Utils, editbox
//
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static PK.PkUtils.ShellLib.ShellApi;

#pragma warning disable IDE0290 // Use primary constructor

namespace PK.PkUtils.ShellLib;

/// <summary>
///  A class that wraps a Dialog that is created by <see cref="ShellApi.SHBrowseForFolder"/>.
/// </summary>
/// 
/// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb762115(v=vs.85).aspx">
/// SHBrowseForFolder function
/// </seealso>
/// 
/// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb762598(v=vs.85).aspx">
/// BFFCALLBACK function pointer
/// </seealso>
/// 
public class ShellBrowseForFolderDialog
{
    #region Typedefs

    /// <summary>
    /// Values of enum represent different kinds of the root type of the dialog.<br/>
    /// For more information, see <see cref="ShellBrowseForFolderDialog.RootType"/> property.
    /// </summary>
    public enum RootTypeOptions
    {
        /// <summary>
        /// If this values is used for <see cref="ShellBrowseForFolderDialog.RootType"/> property, 
        /// the property <see cref="ShellBrowseForFolderDialog.RootSpecialFolder"/> applies for the dialog root.<br/>
        /// </summary>
        BySpecialFolder,

        /// <summary>
        /// If this values is used for <see cref="ShellBrowseForFolderDialog.RootType"/> property, 
        /// the property <see cref="ShellBrowseForFolderDialog.RootPath"/> applies for the dialog root.<br/>
        /// </summary>
        ByPath
    }

    /// <summary>
    /// Flags that specify the options for the dialog box. This member can be 0 or a combination of this enum values.
    /// </summary>
    /// <remarks>
    /// Combination of these values is used in ulFlags field of <see cref="ShellApi.BROWSEINFO"/> structure.
    /// </remarks>
    [Flags]
    public enum BrowseInfoFlag // BIF
    {
        /// <summary>
        /// For finding a folder to start document searching
        /// </summary>
        BIF_RETURNONLYFSDIRS = 0x0001,

        /// <summary>
        /// For starting the Find Computer
        /// </summary>
        BIF_DONTGOBELOWDOMAIN = 0x0002,

        /// <summary>
        /// Top of the dialog has 2 lines of text for BROWSEINFO.lpszTitle and one line if this flag is set.<br/>
        /// Passing the message BFFM_SETSTATUSTEXTA to the hwnd can set the rest of the text.  
        /// This is not used with BIF_USENEWUI and BROWSEINFO.lpszTitle gets all three lines of text.
        /// </summary>
        BIF_STATUSTEXT = 0x0004,

        /// <summary>
        /// Only return file system ancestors. An ancestor is a subfolder that is beneath the root folder in the namespace hierarchy. 
        /// If the user selects an ancestor of the root folder that is not part of the file system, the OK button is grayed.
        /// </summary>
        BIF_RETURNFSANCESTORS = 0x0008,

        /// <summary>
        /// Add an editbox to the dialog
        /// </summary>
        BIF_EDITBOX = 0x0010,

        /// <summary>
        /// insist on valid result (or CANCEL)
        /// </summary>
        BIF_VALIDATE = 0x0020,

        /// <summary>
        /// Use the new dialog layout with the ability to resize.
        /// Caller needs to call OleInitialize() before using this API.
        /// </summary>
        BIF_NEWDIALOGSTYLE = 0x0040,

        /// <summary> Use the new user interface, including an edit box. 
        /// This flag is equivalent to BIF_EDITBOX | BIF_NEWDIALOGSTYLE.
        /// </summary>
        BIF_USENEWUI = (BIF_NEWDIALOGSTYLE | BIF_EDITBOX),

        /// <summary>
        /// Allow URLs to be displayed or entered. (Requires BIF_USENEWUI).
        /// </summary>
        BIF_BROWSEINCLUDEURLS = 0x0080,

        /// <summary>
        /// Add a UA hint to the dialog, in place of the edit box. May not be combined with BIF_EDITBOX.
        /// </summary>
        BIF_UAHINT = 0x0100,

        /// <summary>
        /// Do not add the "New Folder" button to the dialog. Only applicable with BIF_NEWDIALOGSTYLE.
        /// </summary>
        BIF_NONEWFOLDERBUTTON = 0x0200,

        /// <summary>
        /// Don't traverse target as shortcut.
        /// </summary>
        BIF_NOTRANSLATETARGETS = 0x0400,

        /// <summary>
        /// Browsing for Computers.
        /// </summary>
        BIF_BROWSEFORCOMPUTER = 0x1000,

        /// <summary>
        /// Browsing for Printers
        /// </summary>
        BIF_BROWSEFORPRINTER = 0x2000,

        /// <summary>
        /// Browsing for Everything
        /// </summary>
        BIF_BROWSEINCLUDEFILES = 0x4000,

        /// <summary>
        /// Sharable resources displayed (remote shares, requires BIF_USENEWUI)
        /// </summary>
        BIF_SHAREABLE = 0x8000,
    }

    /// <summary>
    /// Lists messages from the browser dialog
    /// </summary>
    public enum BrowseForFolderMessagesFrom // BFFM
    {
        /// <summary>
        /// The dialog box has finished initializing.
        /// </summary>
        BFFM_INITIALIZED = 1,

        /// <summary>
        /// The selection has changed in the dialog box.
        /// </summary>
        BFFM_SELCHANGED = 2,

        /// <summary>
        /// The user typed an invalid name into the dialog's edit box. A nonexistent folder is considered an invalid name.
        /// </summary>
        BFFM_VALIDATEFAILEDA = 3,        // lParam:szPath ret:1(cont),0(EndDialog)

        /// <summary>
        /// The user typed an invalid name into the dialog's edit box. A nonexistent folder is considered an invalid name.
        /// </summary>
        BFFM_VALIDATEFAILEDW = 4,        // lParam:wzPath ret:1(cont),0(EndDialog)

        /// <summary>
        /// An IUnknown interface is available to the dialog box.
        /// </summary>
        BFFM_IUNKNOWN = 5,        // provides IUnknown to client. lParam: IUnknown*
    }

    /// <summary>
    /// Lists messages that on can send to the browser dialog
    /// </summary>
    public enum BrowseForFolderMessagesTo // BFFM
    {
        // messages to browser.
        // 0x400 = WM_USER

        /// <summary>
        /// Message for setting the status text. 
        /// Set the BrowseCallbackProc lpData parameter to point to a null-terminated string with the desired text.
        /// </summary>
        BFFM_SETSTATUSTEXTA = (0x0400 + 100),

        /// <summary>
        /// Message for enabling or disabling the dialog box's OK button.
        /// </summary>
        BFFM_ENABLEOK = (0x0400 + 101),

        /// <summary>
        /// Message for specifying the path of a folder to select.
        /// </summary>
        BFFM_SETSELECTIONA = (0x0400 + 102),

        /// <summary>
        /// Message for specifying the path of a folder to select.
        /// </summary>
        BFFM_SETSELECTIONW = (0x0400 + 103),

        /// <summary>
        /// Message for setting the status text. 
        /// Set the BrowseCallbackProc lpData parameter to point to a null-terminated string with the desired text.
        /// </summary>
        BFFM_SETSTATUSTEXTW = (0x0400 + 104),

        /// <summary>
        /// Message for setting the text that is displayed on the dialog box's OK button.
        /// </summary>
        BFFM_SETOKTEXT = (0x0400 + 105), // Unicode only

        /// <summary>
        /// Message for specifying the path to expand in the Browse dialog box.
        /// </summary>
        BFFM_SETEXPANDED = (0x0400 + 106)  // Unicode only
    }

    /// <summary>
    /// The event arguments that are used with <see cref="ShellBrowseForFolderDialog.OnInitialized"/> event.
    /// </summary>
    public class InitializedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor accepting single argument ( handle of the browse dialog ).
        /// </summary>
        /// <param name="hwnd">The window handle of the browse dialog box.</param>
        public InitializedEventArgs(IntPtr hwnd)
        {
            this.hwnd = hwnd;
        }

        /// <summary>
        /// The window handle of the browse dialog box, provided by constructor
        /// </summary>
        public readonly IntPtr hwnd;
    }

    /// <summary>
    /// The event arguments that are used with <see cref="ShellBrowseForFolderDialog.OnIUnknown"/> event.
    /// </summary>
    public class IUnknownEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor accepting two arguments ( handle of the browse dialog, and a value of an IUnknown interface).
        /// </summary>
        ///
        /// <param name="hwnd">     The window handle of the browse dialog box. </param>
        /// <param name="iUnknown"> A value of an
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms680509(v=vs.85).aspx"> IUnknown </see>
        /// interface that became available to the dialog box with
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb762598(v=vs.85).aspx"> BFFM_IUNKNOWN
        /// </see>
        /// notification message. </param>
        public IUnknownEventArgs(IntPtr hwnd, IntPtr iUnknown)
        {
            this.hwnd = hwnd;
            this.iUnknown = iUnknown;
        }

        /// <summary>
        /// The window handle of the browse dialog box, provided by constructor
        /// </summary>
        public readonly IntPtr hwnd;

        /// <summary>
        /// The value of an IUnknown interface, provided by constructor.
        /// </summary>
        public readonly IntPtr iUnknown;
    }

    /// <summary>
    /// The event arguments that are used with <see cref="ShellBrowseForFolderDialog.OnSelChanged"/> event.
    /// </summary>
    public class SelChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor accepting two arguments ( handle of the browse dialog, and a pointer to the
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb773321(v=vs.85).aspx">
        /// ITEMIDLIST </see> structure ).
        /// </summary>
        ///
        /// <param name="hwnd"> The window handle of the browse dialog box. </param>
        /// <param name="pidl"> Pointer to the
        /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb773321(v=vs.85).aspx">
        /// ITEMIDLIST </see> structure (PIDL) for the newly selected item. </param>
        public SelChangedEventArgs(IntPtr hwnd, IntPtr pidl)
        {
            this.hwnd = hwnd;
            this.pidl = pidl;
        }

        /// <summary>
        /// The window handle of the browse dialog box, provided by constructor
        /// </summary>
        public readonly IntPtr hwnd;

        /// <summary>
        /// Pointer to the ITEMIDLIST structure (PIDL) for the newly selected item. Provided by constructor.
        /// </summary>
        public readonly IntPtr pidl;
    }

    /// <summary>
    ///  The event arguments that are used with <see cref="ShellBrowseForFolderDialog.OnValidateFailed"/> event.
    /// </summary>
    public class ValidateFailedEventArgs : EventArgs
    {
        /// <summary> The constructor. </summary>
        ///
        /// <param name="hwnd">  The window handle of the browse dialog box. </param>
        /// <param name="invalidSel"> A string that contains the invalid name (invalid selection). 
        /// An application can use this data in an error dialog
        /// informing the user that the name was not valid. </param>
        public ValidateFailedEventArgs(IntPtr hwnd, string invalidSel)
        {
            this.hwnd = hwnd;
            this.invalidSel = invalidSel;
        }

        /// <summary>
        /// The window handle of the browse dialog box, provided by constructor
        /// </summary>
        public readonly IntPtr hwnd;

        /// <summary>
        /// A string that contains the invalid name (invalid selection).<br/>
        /// An application can use this data in an error dialog
        /// informing the user that the name was not valid.<br/>
        /// Provided by constructor.
        /// </summary>
        public readonly string invalidSel;
    }

    /// <summary>
    /// The underlying delegate type for <see cref="OnInitialized"/> event.
    /// </summary>
    /// <param name="sender">The source of the event. Refers to the dialog that invoked the event.</param>
    /// <param name="args">The event argument holding this-delegate-specific data.</param>
    public delegate void InitializedHandler(ShellBrowseForFolderDialog sender, InitializedEventArgs args);

    /// <summary>
    /// The underlying delegate type for <see cref="OnIUnknown"/> event.
    /// </summary>
    /// <param name="sender">The source of the event. Refers to the dialog that invoked the event.</param>
    /// <param name="args">The event argument holding this-delegate-specific data.</param>
    public delegate void IUnknownHandler(ShellBrowseForFolderDialog sender, IUnknownEventArgs args);

    /// <summary>
    /// The underlying delegate type for <see cref="OnSelChanged"/> event.
    /// </summary>
    /// <param name="sender">The source of the event. Refers to the dialog that invoked the event.</param>
    /// <param name="args">The event argument holding this-delegate-specific data.</param>
    public delegate void SelChangedHandler(ShellBrowseForFolderDialog sender, SelChangedEventArgs args);

    /// <summary> The underlying delegate type for <see cref="OnValidateFailed"/> event. </summary>
    ///
    /// <param name="sender"> The source of the event. Refers to the dialog that invoked the event. </param>
    /// <param name="args">   The event argument holding this-delegate-specific data. </param>
    ///
    /// <returns> Should return zero to dismiss the dialog or nonzero to keep the dialog displayed. </returns>
    public delegate int ValidateFailedHandler(ShellBrowseForFolderDialog sender, ValidateFailedEventArgs args);

    #endregion // Typedefs

    #region Fields

    /// <summary>
    /// The event being raised when the dialog completes its initialization, giving you the chance for custom
    /// initialization.
    /// </summary>
    public event InitializedHandler OnInitialized;

    /// <summary>
    /// The event being raised when the IUknown interface pointer is available.
    /// This gives you a chance for custom filtering in the dialog.
    /// To filter the items, you need query for IFolderFilterSite and use it set a filter,
    /// using as an argument any object implementing <see cref="IFolderFilter"/>.
    /// </summary>
    public event IUnknownHandler OnIUnknown;

    /// <summary>
    /// The event being raised when the selection in the dialog box has changed.
    /// </summary>
    public event SelChangedHandler OnSelChanged;

    /// <summary>
    /// The event being raised when the user typed an invalid name into the dialog's edit box. 
    /// A nonexistent folder is considered an invalid name.
    /// </summary>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb762598(v=vs.85).aspx">
    /// BFFCALLBACK function pointer
    /// </seealso>
    public event ValidateFailedHandler OnValidateFailed;

    /// <summary> 
    /// Backing field for the <see cref="Title"/> property
    /// </summary>
    private string _title = string.Empty;

    private string _displayName = string.Empty;
    private string _fullName = string.Empty;
    private IntPtr _hwndOwner = IntPtr.Zero;
    private IntPtr _userToken = IntPtr.Zero;

    private BrowseInfoFlag _detailsFlags;

    private string _rootPath = string.Empty;
    private ShellApi.CSIDL _rootSpecialFolder;
    private RootTypeOptions _RootType;
    #endregion //Fields

    #region Constructor(s)

    /// <summary>
    /// Default argument-less constructor
    /// </summary>
    public ShellBrowseForFolderDialog()
    {
        RootType = RootTypeOptions.BySpecialFolder;
        RootSpecialFolder = ShellApi.CSIDL.CSIDL_DESKTOP;

        // Default flags values
        DetailsFlags = BrowseInfoFlag.BIF_BROWSEINCLUDEFILES
          | BrowseInfoFlag.BIF_EDITBOX
          | BrowseInfoFlag.BIF_NEWDIALOGSTYLE
          | BrowseInfoFlag.BIF_SHAREABLE
          | BrowseInfoFlag.BIF_STATUSTEXT
          | BrowseInfoFlag.BIF_USENEWUI
          | BrowseInfoFlag.BIF_VALIDATE;
    }
    #endregion //Constructor(s)

    #region Properties

    /// <summary> Handle to the owner window for the dialog box (if any).  </summary>
    public IntPtr HwndOwner
    {
        get { return _hwndOwner; }
        set { _hwndOwner = value; }
    }

    /// <summary>
    /// Select the root type of the dialog.<br/>
    /// If the value is <see cref="RootTypeOptions.BySpecialFolder"/>, the property <see cref="RootSpecialFolder"/>
    /// applies.<br/>
    /// If the value is <see cref="RootTypeOptions.ByPath"/>, the property <see cref="RootPath"/> applies.<br/>
    /// </summary>
    /// <seealso cref="RootSpecialFolder"/>
    /// <seealso cref="RootPath"/>
    public RootTypeOptions RootType
    {
        get { return _RootType; }
        set { _RootType = value; }
    }

    /// <summary>
    /// Provides the root folder to use in the dialog box. <br/>
    /// The user cannot browse higher in the tree than this folder. <br/>
    /// Applies only if <see cref="RootType"/> is <see cref="RootTypeOptions.ByPath"/>
    /// ( otherwise, the value of <see cref="RootSpecialFolder"/> is applied).
    /// </summary>
    ///
    /// <seealso cref="RootSpecialFolder"/>
    public string RootPath
    {
        get { return _rootPath; }
        set { _rootPath = value; }
    }

    /// <summary>
    /// Provides the root special folder to use in the dialog box. <br/>
    /// The user cannot browse higher in the tree than this folder. <br/>
    /// Applies only if <see cref="RootType"/> is <see cref="RootTypeOptions.BySpecialFolder"/>
    /// ( otherwise, the value of <see cref="RootPath"/> is applied).
    /// </summary>
    ///
    /// <seealso cref="RootPath"/>
    public ShellApi.CSIDL RootSpecialFolder
    {
        get { return _rootSpecialFolder; }
        set { _rootSpecialFolder = value; }
    }

    /// <summary> Address of a buffer to receive the display name of the folder selected by the user. </summary>
    public string DisplayName
    {
        get => _displayName;
    }

    /// <summary> 
    /// A null-terminated string that is displayed above the tree view control in the dialog box. 
    /// </summary>
    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    /// <summary>
    /// An access token that can be used to represent a particular user. <br/>
    /// It is usually set to NULL (IntPtr.Zero), but it may be needed when there are multiple users for those
    /// folders that are treated as belonging to a single user. <br/>
    /// The most commonly used folder of this type is "My Documents". The calling application is responsible for
    /// correct impersonation when hToken is non-NULL. It must have appropriate security privileges for the
    /// particular user, and the user's registry hive must be currently mounted. See Access Control for further
    /// discussion of access control issues.<br/>
    /// 
    /// Assigning the hToken parameter a value of -1 indicates the Default User. This allows clients of
    /// <see cref="ShellApi.SHGetFolderLocation"/> API to find folder locations (such as the Desktop folder) for
    /// the Default User. The Default User profile is duplicated when any new user account is created,
    /// and includes special folders such as My Documents and Desktop. Any items added to the Default User folder also
    /// appear in any new user account.
    /// </summary>
    public IntPtr UserToken
    {
        get { return _userToken; }
        set { _userToken = value; }
    }

    /// <summary> Return the result of the dialog </summary>
    public string FullName
    {
        get { return _fullName; }
        set { _fullName = value; }
    }

    /// <summary>
    /// Flags that specify the options for the dialog box. 
    /// For more info, see <see cref="ShellBrowseForFolderDialog.BrowseInfoFlag"/> enum.
    /// </summary>
    public BrowseInfoFlag DetailsFlags
    {
        get { return _detailsFlags; }
        set { _detailsFlags = value; }
    }
    #endregion // Properties

    #region Methods

    #region Externals

    /// <summary>
    /// The SendMessage function sends the specified message to a window or windows. It calls the window procedure for 
    /// the specified window and does not return until the window procedure has processed the message. 
    /// </summary>
    /// <param name="hWnd">handle to destination window</param>
    /// <param name="Msg">message</param>
    /// <param name="wParam">first message parameter</param>
    /// <param name="lParam">second message parameter</param>
    /// <returns>Specific value, meaning based on the actual message and other arguments provided.</returns>
    [DllImport("User32.dll")]
    private static extern int SendMessage(
      IntPtr hWnd,
      uint Msg,
      uint wParam,
      int lParam);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd">handle to destination window</param>
    /// <param name="Msg">message</param>
    /// <param name="wParam">first message parameter</param>
    /// <param name="lParam">second message parameter</param>
    /// <returns>Specific value, meaning based on the actual message and other arguments provided.</returns>
    [DllImport("User32.dll")]
    private static extern int SendMessage(
      IntPtr hWnd,
      uint Msg,
      uint wParam,
      [MarshalAs(UnmanagedType.LPWStr)]
  string lParam);
    #endregion // Externals

    #region Public Methods

    /// <summary>
    /// Displays a dialog box that enables the user to select a Shell folder.
    /// </summary>
    /// <returns>
    /// If the user selects a folder in this dialog, the return value is true. <br/>
    /// If the user chooses the Cancel button in the dialog box, the return value is false.<br/>
    /// </returns>
    public bool ShowDialog()
    {
        return ShowDialog(null);
    }

    /// <summary>
    /// Displays a dialog box that enables the user to select a Shell folder.
    /// </summary>
    /// <param name="owner">The dialog owner. may be null</param>
    /// <returns>
    /// If the user selects a folder in this dialog, the return value is true. <br/>
    /// If the user chooses the Cancel button in the dialog box, the return value is false.<br/>
    /// </returns>
    public bool ShowDialog(IWin32Window owner)
    {
        if (owner != null)
        {
            HwndOwner = owner.Handle;
        }

        // Get shell's memory allocator, it is needed to free some memory later
        IMalloc pMalloc = ShellFunctions.GetMalloc();
        IntPtr pidlRoot;

        if (RootType == RootTypeOptions.BySpecialFolder)
        {
            ShellApi.SHGetFolderLocation(HwndOwner, (int)RootSpecialFolder, UserToken, 0, out pidlRoot);
        }
        else  // _RootType = RootTypeOptions.ByPath
        {
            ShellApi.SHParseDisplayName(RootPath, IntPtr.Zero, out pidlRoot, 0, out uint iAttribute);
        }

        ShellApi.BROWSEINFO bi = new()
        {
            hwndOwner = HwndOwner,
            pidlRoot = pidlRoot,
            pszDisplayName = new string(' ', 256),
            lpszTitle = Title,
            ulFlags = (uint)DetailsFlags,
            lParam = 0,
            lpfn = new ShellApi.BrowseCallbackProc(this.myBrowseCallbackProc)
        };

        // Show dialog
        IntPtr pidlSelected;
        bool IsSelected;

        if ((IsSelected = (pidlSelected = ShellLib.ShellApi.SHBrowseForFolder(ref bi)) != IntPtr.Zero))
        {
            // Save the display name
            _displayName = bi.pszDisplayName.ToString();

            IShellFolder isf = ShellFunctions.GetDesktopFolder();

            isf.GetDisplayNameOf(pidlSelected, (uint)ShellApi.SHGNO.SHGDN_NORMAL | (uint)ShellApi.SHGNO.SHGDN_FORPARSING, out STRRET ptrDisplayName);

            StrRetToBSTR(ref ptrDisplayName, pidlRoot, out string sDisplay);
            _fullName = sDisplay;

            Marshal.ReleaseComObject(isf);
            pMalloc.Free(pidlSelected);
        }

        if (pidlRoot != IntPtr.Zero)
        {
            pMalloc.Free(pidlRoot);
        }

        Marshal.ReleaseComObject(pMalloc);

        return IsSelected;
    }

    /// <summary>
    /// Enables or disables the dialog box's OK button.
    /// </summary>
    /// <param name="hwnd">The window handle of the browse dialog box.</param>
    /// <param name="enabled">To enable, set to true. To disable, set to false.</param>
    /// 
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb762598(v=vs.85).aspx">
    /// BFFCALLBACK function pointer
    /// </seealso>
    public static void EnableOk(IntPtr hwnd, bool enabled)
    {
        SendMessage(hwnd, (uint)BrowseForFolderMessagesTo.BFFM_ENABLEOK, 0, enabled ? 1 : 0);
    }

    /// <summary>
    /// Specifies the path to expand in the Browse dialog box. The path can be specified as a string 
    /// (the possibility of PIDL argument is not supported by this overload).
    /// </summary>
    /// <param name="hwnd">The window handle of the browse dialog box.</param>
    /// <param name="path">The string that specifies the path.</param>
    /// 
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb762598(v=vs.85).aspx">
    /// BFFCALLBACK function pointer
    /// </seealso>
    public static void SetExpanded(IntPtr hwnd, string path)
    {
        SendMessage(hwnd, (uint)BrowseForFolderMessagesTo.BFFM_SETEXPANDED, 1, path);
    }

    /// <summary>
    /// Sets the text that is displayed on the dialog box's OK button.
    /// </summary>
    /// <param name="hwnd">The window handle of the browse dialog box.</param>
    /// <param name="text">A string that contains the desired text.</param>
    /// 
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb762598(v=vs.85).aspx">
    /// BFFCALLBACK function pointer
    /// </seealso>
    public static void SetOkText(IntPtr hwnd, string text)
    {
        SendMessage(hwnd, (uint)BrowseForFolderMessagesTo.BFFM_SETOKTEXT, 0, text);
    }

    /// <summary>
    /// Specifies the path of a folder to select. The path can be specified as a string
    /// (the possibility of PIDL argument is not supported by this overload).
    /// </summary>
    /// <param name="hwnd">The window handle of the browse dialog box.</param>
    /// <param name="path">The string that specifies the path.</param>
    /// 
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb762598(v=vs.85).aspx">
    /// BFFCALLBACK function pointer
    /// </seealso>
    public static void SetSelection(IntPtr hwnd, string path)
    {
        SendMessage(hwnd, (uint)BrowseForFolderMessagesTo.BFFM_SETSELECTIONW, 1, path);
    }

    /// <summary>
    /// Sets the status text of dialog box.
    /// </summary>
    /// <param name="hwnd">The window handle of the browse dialog box.</param>
    /// <param name="text">A string that contains the desired text.</param>
    public static void SetTextStatus(IntPtr hwnd, string text)
    {
        SendMessage(hwnd, (uint)BrowseForFolderMessagesTo.BFFM_SETSTATUSTEXTW, 1, text);
    }
    #endregion // Public Methods

    #region Private Methods

    private int myBrowseCallbackProc(IntPtr hwnd, uint uMsg, IntPtr lParam, IntPtr lpData)
    {
        switch ((BrowseForFolderMessagesFrom)uMsg)
        {
            case BrowseForFolderMessagesFrom.BFFM_INITIALIZED:
                System.Diagnostics.Debug.WriteLine("BFFM_INITIALIZED");
                if (!string.IsNullOrEmpty(_fullName))
                {
                    SetSelection(hwnd, _fullName);
                    SetExpanded(hwnd, _fullName);
                }
                if (OnInitialized != null)
                {
                    InitializedEventArgs args = new(hwnd);
                    OnInitialized(this, args);
                }

                break;

            case BrowseForFolderMessagesFrom.BFFM_IUNKNOWN:
                System.Diagnostics.Debug.WriteLine("BFFM_IUNKNOWN");

                if (OnIUnknown != null)
                {
                    IUnknownEventArgs args = new(hwnd, lParam);
                    OnIUnknown(this, args);
                }

                break;

            case BrowseForFolderMessagesFrom.BFFM_SELCHANGED:
                System.Diagnostics.Debug.WriteLine("BFFM_SELCHANGED");
#if DEBUG
                string selectedPath = GetDisplayNameOf(lParam);
#endif
                if (OnSelChanged != null)
                {
                    SelChangedEventArgs args = new(hwnd, lParam);
                    OnSelChanged(this, args);
                }

                break;

            case BrowseForFolderMessagesFrom.BFFM_VALIDATEFAILEDA:
                System.Diagnostics.Debug.WriteLine("BFFM_VALIDATEFAILEDA");

                if (OnValidateFailed != null)
                {
                    string failedSel = Marshal.PtrToStringAnsi(lParam);
                    ValidateFailedEventArgs args = new(hwnd, failedSel);
                    return OnValidateFailed(this, args);
                }
                break;

            case BrowseForFolderMessagesFrom.BFFM_VALIDATEFAILEDW:
                System.Diagnostics.Debug.WriteLine("BFFM_VALIDATEFAILEDW");

                if (OnValidateFailed != null)
                {
                    string failedSel = Marshal.PtrToStringUni(lParam);
                    ValidateFailedEventArgs args = new(hwnd, failedSel);
                    return OnValidateFailed(this, args);
                }

                break;
        }

        return 0;
    }
    #endregion // Private Methods
    #endregion // Methods



    private string GetDisplayNameOf(IntPtr pidl)
    {
        SHGetDesktopFolder(out IShellFolder desktopFolder);

        desktopFolder.GetDisplayNameOf(pidl, 0, out STRRET strret);

        string displayName;
        if (strret.uType == 0) // STRRET_WSTR
        {
            displayName = Marshal.PtrToStringUni(strret.pOleStr);
            Marshal.FreeCoTaskMem(strret.pOleStr);
        }
        else // STRRET_CSTR
        {
            displayName = new string(strret.cStr);
        }

        return displayName;
    }
}

#pragma warning restore IDE0290 // Use primary constructor
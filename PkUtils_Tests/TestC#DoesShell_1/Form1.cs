using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.ShellLib;
using SBFD = PK.PkUtils.ShellLib.ShellBrowseForFolderDialog;

namespace WinTester
{
    /// <summary> The main for of the program. </summary>
    public partial class Form1 : Form
    {
        private readonly IEnumerable<string> _browseFilesExt = new string[] { "bmp", "jpg", "txt" };

        public Form1()
        {
            InitializeComponent();
            this.Icon = Resource.App;
        }

        #region Event_handlers

        private void BtnGetWindowsFolder_Click(object sender, EventArgs e)
        {
            // Get shell's memory allocator, it is needed to free some memory later

            IMalloc pMalloc = ShellFunctions.GetMalloc();

            // Get PIDL of the WINDOWS special folder
            ShellApi.SHGetFolderLocation(this.Handle, (short)ShellApi.CSIDL.CSIDL_WINDOWS,
              IntPtr.Zero, 0, out IntPtr pidlRoot);

            // Get the path from a PIDL
            StringBuilder path_from_a_PIDL = new(256);
            ShellApi.SHGetPathFromIDList(pidlRoot, path_from_a_PIDL);

            // Get the special folder path directly (without getting its PIDL)
            StringBuilder path_directly = new(256);
            ShellApi.SHGetFolderPath(IntPtr.Zero, (short)ShellApi.CSIDL.CSIDL_WINDOWS,
              IntPtr.Zero, (uint)ShellApi.SHGFP_TYPE.SHGFP_TYPE_CURRENT, path_directly);

            if (pidlRoot != IntPtr.Zero)
                pMalloc.Free(pidlRoot);
            Marshal.ReleaseComObject(pMalloc);

            string[] lines = new string[] {
                $"Result of SHGetPathFromIDList: '{path_from_a_PIDL}'",
                $"Result of SHGetFolderPath: '{path_directly}'"
            };
            MessageBox.Show(lines.Join(Environment.NewLine), $"Test of {nameof(ShellApi.SHGetFolderPath)}");
        }

        private void BtnGetSystemFolder_Click(object sender, EventArgs e)
        {
            IShellFolder pShellFolder = ShellFunctions.GetDesktopFolder();

            ShellApi.SHGetFolderLocation(IntPtr.Zero, (short)ShellApi.CSIDL.CSIDL_SYSTEM, IntPtr.Zero, 0,
                out IntPtr pidlRoot);

            const UInt32 uFlags = (uint)ShellApi.SHGNO.SHGDN_NORMAL | (uint)ShellApi.SHGNO.SHGDN_FORPARSING;
            pShellFolder.GetDisplayNameOf(pidlRoot, uFlags, out ShellApi.STRRET strret);

            string strDisplayName = strret.GetString();
            ShellApi.StrRetToBSTR(ref strret, pidlRoot, out string strDisplayName2);

            Marshal.ReleaseComObject(pShellFolder);
            strret.Dispose();

            string[] lines = new string[] {
                $"Result of STRRET.GetString(): '{strDisplayName}'",
                $"Result of {nameof(ShellApi)}.{nameof(ShellApi.StrRetToBSTR)}: '{strDisplayName2}'"
            };
            MessageBox.Show(lines.Join(Environment.NewLine), $"Test of {nameof(ShellApi.SHGetFolderLocation)}");
        }

        private void BtnGetSHBindToParent_Click(object sender, EventArgs e)
        {
            IMalloc pMalloc = ShellFunctions.GetMalloc();

            ShellApi.SHGetFolderLocation(IntPtr.Zero, (int)ShellApi.CSIDL.CSIDL_SYSTEM, IntPtr.Zero, 0, out nint pidlSystem);

            IntPtr pidlRelative = IntPtr.Zero;
            ShellApi.SHBindToParent(pidlSystem, ShellGUIDs.IID_IShellFolder, out nint ptrParent, ref pidlRelative);

            Type shellFolderType = ShellFunctions.GetShellFolderType();
            Object obj = Marshal.GetTypedObjectForIUnknown(ptrParent, shellFolderType);
            IShellFolder ishellParent = (IShellFolder)obj;

            ishellParent.GetDisplayNameOf(pidlRelative, (uint)ShellApi.SHGNO.SHGDN_NORMAL, out ShellApi.STRRET strret);

            StringBuilder strDisplay = new(256);
            ShellApi.StrRetToBuf(ref strret, pidlSystem, strDisplay, (uint)strDisplay.Capacity);

            Marshal.ReleaseComObject(ishellParent);
            pMalloc.Free(pidlSystem);
            Marshal.ReleaseComObject(pMalloc);
            strret.Dispose();

            MessageBox.Show(strDisplay.ToString(), $"Test of {nameof(ShellApi.SHBindToParent)}");
        }

        private void BtnBrowseForFolder_Click(object sender, EventArgs e)
        {
            IMalloc pMalloc = ShellFunctions.GetMalloc();

            String sPath = @"c:\temp\divx";
            ShellApi.SHParseDisplayName(sPath, IntPtr.Zero, out nint pidlRoot, 0, out uint iAttribute);

            ShellApi.BROWSEINFO bi = new()
            {
                hwndOwner = this.Handle,
                pidlRoot = pidlRoot,
                lpszTitle = "hello"
            };

            IntPtr pidlSelected = ShellApi.SHBrowseForFolder(ref bi);

            if (pidlRoot != IntPtr.Zero)
            {
                pMalloc.Free(pidlRoot);
            }

            if (pidlSelected != IntPtr.Zero)
            {
                pMalloc.Free(pidlSelected);
            }

            Marshal.ReleaseComObject(pMalloc);
        }

        #region Custom_Shell_Browse_For_Folder_Dialog

        private void CustomBrowseForFolderButton_Click(object sender, EventArgs e)
        {
            string[] arrMsgs = new string[] {
                "Hello PkNetUtils readers!",
                "Welcome to my versatile Shell dialog, designed for your convenience.",
                "Please choose a folder or select a JPG, BMP, or TXT file within a folder."
            };
            string title = string.Join(Environment.NewLine, arrMsgs);
            SBFD folderDialog = new() { HwndOwner = this.Handle, Title = title };

            // Scenario A - take the defaults

            // Scenario B - select from a special folder
            //folderDialog.RootType = SBFD.RootTypeOptions.BySpecialFolder;
            //folderDialog.RootSpecialFolder = ShellApi.CSIDL.CSIDL_WINDOWS;

            // Scenario C - select from a specific path
            //folderDialog.RootType = SBFD.RootTypeOptions.ByPath;
            //folderDialog.RootPath = @"c:\temp\divx";

            // register events
            folderDialog.OnInitialized += new SBFD.InitializedHandler(this.InitializedEvent);
            folderDialog.OnIUnknown += new SBFD.IUnknownHandler(this.IUnknownEvent);
            folderDialog.OnSelChanged += new SBFD.SelChangedHandler(this.SelChangedEvent);
            folderDialog.OnValidateFailed += new SBFD.ValidateFailedHandler(this.ValidateFailedEvent);

            if (folderDialog.ShowDialog())
            {
                MessageBox.Show($"Display Name: {folderDialog.DisplayName} {Environment.NewLine}Full Name: {folderDialog.FullName}");
            }
        }

        private void InitializedEvent(SBFD sender, SBFD.InitializedEventArgs args)
        {
            SBFD.SetOkText(args.hwnd, "KABOOM!");
        }

        private void IUnknownEvent(SBFD sender, SBFD.IUnknownEventArgs args)
        {

            if (args.iUnknown == IntPtr.Zero)
                return;

            Marshal.QueryInterface(args.iUnknown, in ShellGUIDs.IID_IFolderFilterSite, out nint iFolderFilterSite);

            Object obj = Marshal.GetTypedObjectForIUnknown(iFolderFilterSite, ShellFunctions.GetFolderFilterSiteType());
            IFolderFilterSite folderFilterSite = (IFolderFilterSite)obj;
            FilterByExtension filter = new(_browseFilesExt);

            folderFilterSite.SetFilter(filter);
        }


        private void SelChangedEvent(SBFD sender, SBFD.SelChangedEventArgs args)
        {
            // Get pidl display name
            IShellFolder isf = ShellFunctions.GetDesktopFolder();

            isf.GetDisplayNameOf(args.pidl, (uint)ShellApi.SHGNO.SHGDN_NORMAL | (uint)ShellApi.SHGNO.SHGDN_FORPARSING, out ShellApi.STRRET ptrDisplayName);
            ShellApi.StrRetToBSTR(ref ptrDisplayName, 0, out string pidlPath);

            // do some stuff
            if (pidlPath.ToUpper() == @"c:\temp".ToUpper())
            {
                // make ok button disabled
                SBFD.EnableOk(args.hwnd, false);

                // expand c:\windows
                //sender.SetExpanded(args.hwnd,@"c:\windows");

                // select c:\windows
                //sender.SetSelection(args.hwnd,@"c:\windows");

            }
        }

        private int ValidateFailedEvent(SBFD sender, SBFD.ValidateFailedEventArgs args)
        {
            string[] arrMessages = new string[]
            {
                $"You have written: '{args.invalidSel}'.",
                "This is not valid, please try again.",
                "You can also click Cancel to dismiss the browse dialog."
            };
            string erroMesage = arrMessages.Join(Environment.NewLine);
            DialogResult dlgRes = MessageBox.Show(erroMesage, null, MessageBoxButtons.OKCancel);
            // 
            // returns zero to dismiss the dialog or nonzero to keep the dialog displayed.
            int nRes = (dlgRes == DialogResult.OK) ? 1 : 0;

            return nRes;
        }
        #endregion // Custom_Shell_Browse_For_Folder_Dialog
        #endregion // Event_handlers
    }
}

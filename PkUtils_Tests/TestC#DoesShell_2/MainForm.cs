using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using PK.PkUtils.Extensions;
using PK.PkUtils.IO;
using PK.PkUtils.SystemEx;
using PK.PkUtils.UI.Dialogs.PSTaskDialog;
using PK.PkUtils.UI.Utils;
using PK.PkUtils.Utils;
using ShellLib = PK.PkUtils.ShellLib;

namespace WinTester2
{
    /// <summary>
    /// Summary description for FrmMain.
    /// </summary>
    public partial class MainForm : System.Windows.Forms.Form
    {
        #region Fields

        protected static readonly string[] _arrExesToTry = new string[]
        {
            "taskmgr.exe",
            "winver.exe",
        };

        protected const int _N_COPY_FILES = 4;
        protected const int _N_MOVE_FILES = 5;
        protected const int _N_DELETE_FILES = 8;

        #endregion // Fields

        #region Constructor(s)

        public MainForm()
        {
            InitializeComponent();
            this.Icon = Resource.App;
        }
        #endregion // Constructor(s)

        #region Methods

        #region Public Methods
        #endregion // Public Methods

        #region Protected Methods

        #region Protected Static Methods

        protected static string FindExeInPredefined()
        {
            Func<string, string> selector = str => FilePathHelper.CombineAbsolute(Environment.SystemDirectory, str);
            IEnumerable<string> finalExes = _arrExesToTry.Select(str => selector(str));
            string strExePath = finalExes.FirstOrDefault(str => (null != FilePathHelper.SafeGetFileInfo(str)));

            return strExePath;
        }

        protected static string FindExeFile()
        {
            string strExePath = SysInfo.GetMsinfo32Path();

            if (string.IsNullOrEmpty(strExePath))
            {
                strExePath = FindExeInPredefined();
            }
            return strExePath;
        }

        protected static string GenerateUniqueTempFileName(DirectoryInfo dirSource)
        {
            string strResult = null;

            for (; strResult == null;)
            {
                TimeSpan ts = DateTime.Now.TimeOfDay;
                string strName = string.Format(CultureInfo.InvariantCulture, "{0:00}-{1:00}-{2:00}.txt", ts.Hours, ts.Minutes, ts.Seconds);
                string strComlete = FilePathHelper.CombineAbsolute(dirSource.ToString(), strName);
                FileInfo fi = FilePathHelper.SafeGetFileInfo(strComlete);

                if (null == fi)
                {
                    strResult = strComlete;
                }
            }

            return strResult;
        }

        protected static FileInfo GenerateUniqueTempFile(DirectoryInfo dirSource)
        {
            string strPath = GenerateUniqueTempFileName(dirSource);
            string strContents = string.Format(CultureInfo.InvariantCulture,
              "This temporary file has been created on {0}", Conversions.DayTimeString(DateTime.Now));
            FileInfo fiResult = null;

            File.WriteAllText(strPath, strContents);
            fiResult = FilePathHelper.SafeGetFileInfo(strPath);

            return fiResult;
        }

        protected static IList<FileInfo> GenerateUniqueTempFiles(DirectoryInfo dirSource, int nFiles)
        {
            FileInfo fi;
            IList<FileInfo> result = [];

            for (int ii = 0; ii < nFiles; ii++)
            {
                if (null != (fi = GenerateUniqueTempFile(dirSource)))
                {
                    result.Add(fi);
                }
                else
                {
                    return null;
                }
            }
            return result;
        }
        #endregion // Protected Static Methods

        #region Protected Non-static Methods

        protected string FindBmpFile()
        {
            string strRes = string.Empty;

            using (WaitCursor wc = new(this))
            {
                DirectoryInfo dirSystem = new(Environment.SystemDirectory);
                FileSearchNonRecursive fs = new();
                IEnumerable<FileInfo> fiTmp = fs.SearchFiles(dirSystem, "*.bmp", SearchOption.AllDirectories);

                strRes = fiTmp.OrderBy(file => file.Length).LastOrDefault().NullSafe(fi => fi.ToString());
            }
            return strRes;
        }

        protected void UnableToFindMsg()
        {
            VistaTaskDialogManager.MessageBox(
              this,
              "Error",
              "Unable to find suitable file.",
              "You may need to modify the code of testing project to fixup this problem",
              TaskDialogButtons.OK,
              SystemIconType.Warning);
        }
        #endregion // Protected Non-static Methods
        #endregion // Protected Methods

        #region Private Methods

        private void btnOpenExe_Click(object sender, System.EventArgs e)
        {
            string strExePath = FindExeFile();

            if (string.IsNullOrEmpty(strExePath))
            {
                UnableToFindMsg();
            }
            else
            {
                ShellLib.ShellExecute shellExecute = new(this.Handle, ShellLib.ShellExecute.OpenFile, strExePath);
                shellExecute.Execute();
            }
        }

        private void btnShowBmp_Click(object sender, System.EventArgs e)
        {
            string strBmpPath = FindBmpFile();

            if (string.IsNullOrEmpty(strBmpPath))
            {
                UnableToFindMsg();
            }
            else
            {
                ShellLib.ShellExecute shellExecute = new(this.Handle, ShellLib.ShellExecute.OpenFile, strBmpPath);
                shellExecute.Execute();
            }
        }

        private void btnEditBmp_Click(object sender, System.EventArgs e)
        {
            string strBmpPath = FindBmpFile();

            if (string.IsNullOrEmpty(strBmpPath))
            {
                UnableToFindMsg();
            }
            else
            {
                ShellLib.ShellExecute shellExecute = new(this.Handle,
                  ShellLib.ShellExecute.EditFile, strBmpPath);
                shellExecute.Execute();
            }
        }

        private void btnFindInFolder_Click(object sender, System.EventArgs e)
        {
            ShellLib.ShellExecute shellExecute = new(this.
              Handle, ShellLib.ShellExecute.FindInFolder, Environment.SystemDirectory);
            shellExecute.Execute();
        }

        private void btnExploreFolder_Click(object sender, System.EventArgs e)
        {
            ShellLib.ShellExecute shellExecute = new(this.Handle,
              ShellLib.ShellExecute.ExploreFolder, Environment.SystemDirectory);

            shellExecute.Execute();
        }

        private void btnCopy_Click(object sender, System.EventArgs e)
        {
            DirectoryInfo dirSource = new(Environment.SystemDirectory);
            string strDirDest = System.IO.Path.GetTempPath();

            FileSearchNonRecursive fs = new();
            IEnumerable<FileInfo> fiSource = fs.SearchFiles(dirSource, "*.exe", SearchOption.AllDirectories);
            IEnumerable<FileInfo> fiSourceN = fiSource.Take(_N_COPY_FILES);

            IEnumerable<string> source = fiSourceN.Select(fi => fi.ToString());
            IEnumerable<string> dest = fiSourceN.Select(fi => FilePathHelper.CombineAbsolute(strDirDest, fi.Name));

            ShellLib.ShellFileOperation fo = new();
            string allSources = source.Aggregate(string.Empty, (suma, next) => suma + Environment.NewLine + next.ToString());
            bool retVal = false;

            using (WaitCursor wc = new(this))
            {
                fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_COPY;
                fo.OwnerWindow = this.Handle;
                fo.SourceFiles = source;
                fo.DestFiles = dest;

                retVal = fo.DoOperation();
            }

            if (retVal)
            {
                VistaTaskDialogManager.MessageBox(
                  this,
                  "Success",
                  "Copying of following files complete without errors:",
                  allSources,
                  TaskDialogButtons.OK,
                  SystemIconType.Information);
            }
            else
            {
                VistaTaskDialogManager.MessageBox(
                  this,
                  "Error",
                  "Copying of following files completed with errors!",
                  allSources,
                  TaskDialogButtons.OK,
                  SystemIconType.Warning);
            }
        }

        private void btnMove_Click(object sender, System.EventArgs e)
        {
            string strDirDest = System.IO.Path.GetTempPath();
            string strDirSrc = FilePathHelper.GetAssemblyDirectory(Assembly.GetExecutingAssembly());
            DirectoryInfo dirSource = new(strDirSrc);
            IEnumerable<FileInfo> fiSourceN = null;

            using (WaitCursor wc = new(this))
            {
                fiSourceN = GenerateUniqueTempFiles(dirSource, _N_MOVE_FILES);
            }

            if (null == fiSourceN)
            {
                UnableToFindMsg();
            }
            else
            {
                IEnumerable<string> source = fiSourceN.Select(fi => fi.ToString());
                IEnumerable<string> dest = fiSourceN.Select(fi => FilePathHelper.CombineAbsolute(strDirDest, fi.Name));

                ShellLib.ShellFileOperation fo = new();
                string allSources = source.Aggregate(string.Empty, (suma, next) => suma + Environment.NewLine + next.ToString());
                bool retVal = false;

                using (WaitCursor wc = new(this))
                {
                    fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_MOVE;
                    fo.OwnerWindow = this.Handle;
                    fo.SourceFiles = source;
                    fo.DestFiles = dest;

                    retVal = fo.DoOperation();
                }

                if (retVal)
                {
                    VistaTaskDialogManager.MessageBox(
                      this,
                      "Success",
                      "Moving of following files complete without errors:",
                      allSources,
                      TaskDialogButtons.OK,
                      SystemIconType.Information);
                }
                else
                {
                    VistaTaskDialogManager.MessageBox(
                      this,
                      "Error",
                      "Moving of following files completed with errors!",
                      allSources,
                      TaskDialogButtons.OK,
                      SystemIconType.Warning);
                }
            }
        }

        private void btnDelete_Click(object sender, System.EventArgs e)
        {
            string strDirSrc = FilePathHelper.GetAssemblyDirectory(Assembly.GetExecutingAssembly());
            DirectoryInfo dirSource = new(strDirSrc);
            IEnumerable<FileInfo> fiSourceN = null;

            using (WaitCursor wc = new(this))
            {
                fiSourceN = GenerateUniqueTempFiles(dirSource, _N_DELETE_FILES);
            }

            if (null == fiSourceN)
            {
                UnableToFindMsg();
            }
            else
            {
                IEnumerable<string> source = fiSourceN.Select(fi => fi.ToString());

                ShellLib.ShellFileOperation fo = new();
                string allSources = source.Aggregate(string.Empty, (suma, next) => suma + Environment.NewLine + next.ToString());
                bool retVal = false;

                using (WaitCursor wc = new(this))
                {
                    fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_DELETE;
                    fo.OwnerWindow = this.Handle;
                    fo.SourceFiles = source;
                    fo.DestFiles = source;

                    retVal = fo.DoOperation();
                }

                if (retVal)
                {
                    VistaTaskDialogManager.MessageBox(
                      this,
                      "Success",
                      "Deleting of following files complete without errors:",
                      allSources,
                      TaskDialogButtons.OK,
                      SystemIconType.Information);
                }
                else
                {
                    VistaTaskDialogManager.MessageBox(
                      this,
                      "Error",
                      "Deleting of following files completed with errors!",
                      allSources,
                      TaskDialogButtons.OK,
                      SystemIconType.Warning);
                }
            }
        }

        private void btnAddFile_Click(object sender, System.EventArgs e)
        {
            string strFile = null;

            using (WaitCursor wc = new(this))
            {
                DirectoryInfo dirSystem = new(Environment.SystemDirectory);
                FileSearchNonRecursive fs = new();
                IEnumerable<FileInfo> fiTmp = fs.SearchFiles(dirSystem, "*.bmp", SearchOption.AllDirectories);

                if (!fiTmp.IsEmpty())
                {
                    var fiSorted = fiTmp.OrderBy(file => file.ToString().Length);
                    strFile = fiSorted.First().ToString();
                }
            }

            if (!string.IsNullOrEmpty(strFile))
            {
                ShellLib.ShellAddRecent.AddToList(strFile);
                VistaTaskDialogManager.MessageBox(
                  this,
                  "Success",
                  "Following file has been added to System Recent List:",
                  strFile,
                  TaskDialogButtons.OK,
                  SystemIconType.Information);
            }
            else
            {
                UnableToFindMsg();
            }
        }

        private void btnClearList_Click(object sender, System.EventArgs e)
        {
            ShellLib.ShellAddRecent.ClearList();
            VistaTaskDialogManager.MessageBox(
              this,
              "Success",
              "The System Recent List has been cleared",
              null,
              TaskDialogButtons.OK,
              SystemIconType.Information);
        }

        private void btnOpenPrinter_Click(object sender, System.EventArgs e)
        {
            // #FIX#
            // 
            int Ret;
            Ret = ShellLib.ShellApi.SHInvokePrinterCommand(
              this.Handle,
              (uint)ShellLib.ShellApi.PrinterActions.PRINTACTION_OPEN,
              "<printer name comes here>",
              "",
              1);
        }

        private void btnShowProperties_Click(object sender, System.EventArgs e)
        {
            // #FIX#
            // 
            int Ret;
            Ret = ShellLib.ShellApi.SHInvokePrinterCommand(
              this.Handle,
              (uint)ShellLib.ShellApi.PrinterActions.PRINTACTION_PROPERTIES,
              "<printer name comes here>",
              "",
              1);
        }

        private void btnTestPage_Click(object sender, System.EventArgs e)
        {
            // #FIX#
            // 
            int Ret;
            Ret = ShellLib.ShellApi.SHInvokePrinterCommand(
              this.Handle,
              (uint)ShellLib.ShellApi.PrinterActions.PRINTACTION_TESTPAGE,
              "<printer name comes here>",
              "",
              1);
        }
        #endregion // Private Methods
        #endregion // Methods
    }
}

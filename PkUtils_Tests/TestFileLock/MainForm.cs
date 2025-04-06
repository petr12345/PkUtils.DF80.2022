using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using PK.PkUtils.IO;
using PK.PkUtils.SystemEx;
using PK.PkUtils.Utils;
using PK.TestFileLock.Properties;

namespace PK.TesFileLock
{
    public partial class MainForm : Form
    {
        #region Fields
        private LockFile _FileLock;
        private LockFolder _FolderLock;

        private string _strFileName;
        private string _strDirectoryName;
        private Lazy<FolderBrowserDialog> _fbd;
        private Lazy<OpenFileDialog> _ofd;
        #endregion // Fields

        #region Constructor(s)

        public MainForm()
        {
            InitializeComponent();
            this.Icon = Resources.Locked;

            InitDialogs();
            InitPathTexts();
            UpdateFileControls();
            UpdateDirectoryControls();

        }
        #endregion // Constructor(s)

        #region Properties

        private bool IsFileLocked
        {
            get { return (_FileLock != null) && _FileLock.IsItemLocked; }
        }

        public bool IsFolderOpenOrLocked
        {
            get { return (_FolderLock != null) && _FolderLock.IsItemOpen; }
        }

        public string TestedFileName
        {
            get { return _strFileName; }
        }

        public string TestedDirectoryName
        {
            get { return _strDirectoryName; }
        }

        private FolderBrowserDialog FolderBrowserDialog
        {
            get { return _fbd.Value; }
        }

        private OpenFileDialog OpenFileDialog
        {
            get { return _ofd.Value; }
        }
        #endregion // Properties

        #region Methods

        #region File_locking

        protected void UpdateFileButtons()
        {
            string strFileName = TestedFileName;
            string strFileBtnTxt = !IsFileLocked ? "Test FileLock" : "File UnLock";
            bool bTestEnabled = !string.IsNullOrEmpty(strFileName) && File.Exists(strFileName);
            bool bBrowseEnabled = !IsFileLocked;

            _btnTestFileLock.Text = strFileBtnTxt;
            _btnTestFileLock.Enabled = bTestEnabled;
            _btnBrowseFile.Enabled = bBrowseEnabled;
        }

        protected void UpdateFileControls()
        {
            this._textBxFileName.Text = TestedFileName;
            UpdateFileButtons();
        }

        protected void LockFile(string strPath)
        {
            FileInfo fi = new(strPath);

            if (fi.Exists)
            {
                _FileLock ??= new LockFile();
                _FileLock.TryLockFile(strPath, 0, (ulong)fi.Length, true, true);
            }
            else
            {
                string strMsg = string.Format(CultureInfo.CurrentCulture,
                  "The file '{0}' does not exist", strPath);
                MessageBox.Show(strMsg, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        protected void CloseFileLock()
        {
            if (_FileLock != null)
            {
                _FileLock.Dispose();
                _FileLock = null;
            }
        }
        #endregion // File_locking

        #region Folder_locking

        protected void UpdateDirectoryButtons()
        {
            bool bOpenOrLocked = IsFolderOpenOrLocked;
            bool bBrowseEnabled = !bOpenOrLocked;
            string strDirName = TestedDirectoryName;
            string strDirectoryBtnTxt = !bOpenOrLocked ? "Test Folder Lock" : "Folder UnLock";
            bool bTestEnabled = !string.IsNullOrEmpty(strDirName) && Directory.Exists(strDirName);

            _btnTestDirectoryLock.Text = strDirectoryBtnTxt;
            _btnTestDirectoryLock.Enabled = bTestEnabled;
            _btnBrowseDirectory.Enabled = bBrowseEnabled;
            _btnTestCreateFileInFolder.Enabled = true; /* bOpenOrLocked; */
        }

        protected void UpdateDirectoryControls()
        {
            this._textBxDirectoryName.Text = TestedDirectoryName;
            UpdateDirectoryButtons();
        }

        protected void LockFolder(string strPath)
        {
            DirectoryInfo di = new(strPath);

            if (di.Exists)
            {
                _FolderLock ??= new LockFolder();
                _FolderLock.TryLockFolder(strPath, true, false);
            }
            else
            {
                string strMsg = string.Format(CultureInfo.CurrentCulture,
                  "The folder '{0}' does not exist", strPath);
                MessageBox.Show(strMsg, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        protected void CloseFolderLock()
        {
            Disposer.SafeDispose(ref _FolderLock);
        }
        #endregion // Folder_locking

        #region General
        protected static string GetProcessInfo(Process p)
        {
            string filename = p.MainModule.FileName;
            string userName = ProcessUtils.GetUserNameForProcess(p);
            string result;

            if (string.IsNullOrEmpty(userName))
            {
                result = filename;
            }
            else
            {
                result = string.Format(CultureInfo.InvariantCulture, "{0} (executed by '{1}')", filename, userName);
            }

            return result;
        }

        protected void InitDialogs()
        {
            _fbd = new Lazy<FolderBrowserDialog>(() =>
            {
                FolderBrowserDialog fbd = new()
                {
                    SelectedPath = FilePathHelper.GetAssemblyDirectory(Assembly.GetEntryAssembly())
                };
                return fbd;
            }
            );

            _ofd = new Lazy<OpenFileDialog>(() =>
            {
                OpenFileDialog ofd = new()
                {
                    Title = "Select an existing file to lock",
                    Filter = "All files (*.*)|*.*"
                };
                return ofd;
            }
            );
        }

        protected void InitPathTexts()
        {
            string strLastFile = Settings.Default.LastSelectedFile;
            string strLastFolder = Settings.Default.LastSelectedFolder;

            if (!string.IsNullOrEmpty(strLastFile))
            {
                _strFileName = strLastFile;
            }
            else
            {
                string strDir = FilePathHelper.GetAssemblyDirectory(Assembly.GetEntryAssembly());
                string strReadme = Path.Combine(strDir, "Readme.txt");

                if (File.Exists(strReadme))
                    _strFileName = strReadme;
                else
                    _strFileName = FilePathHelper.GetApplicationFileName();
            }

            if (!string.IsNullOrEmpty(strLastFolder))
                _strDirectoryName = strLastFolder;
            else
                _strDirectoryName = FilePathHelper.GetAssemblyDirectory(Assembly.GetEntryAssembly());
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var sett = Settings.Default;
            sett.LastSelectedFile = TestedFileName;
            sett.LastSelectedFolder = TestedDirectoryName;
            sett.Save();

            base.OnFormClosing(e);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                Disposer.SafeDispose(ref _fbd);
                Disposer.SafeDispose(ref _ofd);

                CloseFileLock();
                CloseFolderLock();
            }
            base.Dispose(disposing);
        }

        #endregion // General
        #endregion // Methods

        #region Event_handlers

        #region File_locking

        private void OnBtnBrowseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = this.OpenFileDialog;
            DialogResult result = ofd.ShowDialog();

            if (result == DialogResult.OK)
            {
                _strFileName = ofd.FileName;
                UpdateFileControls();
            }
        }

        private void OnButtonTestFileLock_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsFileLocked)
                {
                    LockFile(this.TestedFileName);
                }
                else
                {
                    CloseFileLock();
                }
            }
            catch (Win32Exception ex)
            {
                string strMsg = string.Format(CultureInfo.CurrentCulture,
                  "Could not lock the file '{0}'.{1}{1}{2}",
                  TestedFileName, Environment.NewLine, ex.Message);
                MessageBox.Show(strMsg, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            UpdateFileButtons();
        }

        private void _btnWhoLockedTheFile_Click(object sender, EventArgs e)
        {
            string filename = TestedFileName;

            try
            {
                List<Process> processes = ProcessUtils.WhoIsLocking(filename);

                if (!processes.Any())
                {
                    MessageBox.Show("No locking process found", null, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    StringBuilder sb = new("Found these locking processes: ");

                    sb.Append(Environment.NewLine);
                    foreach (var pair in processes.Select((proc, index) => new { proc, index }))
                    {
                        sb.Append(GetProcessInfo(pair.proc));
                        if (pair.index < processes.Count - 1)
                        {
                            sb.Append(", ");
                        }
                        else
                        {
                            break;
                        }
                    }

                    MessageBox.Show(sb.ToString(), null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion // File_locking

        #region Folder_locking

        private void OnBtnBrowseDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = this.FolderBrowserDialog;
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK)
            {
                _strDirectoryName = fbd.SelectedPath;
                UpdateDirectoryControls();
            }
        }

        private void OnButtonTestDirectoryLock_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsFolderOpenOrLocked)
                {
                    LockFolder(TestedDirectoryName);
                }
                else
                {
                    CloseFolderLock();
                }
            }
            catch (Win32Exception ex)
            {
                string strMsg = string.Format(CultureInfo.CurrentCulture,
                  "Could not lock the directory '{0}'.{1}{1}{2}",
                  TestedDirectoryName, Environment.NewLine, ex.Message);
                MessageBox.Show(strMsg, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            UpdateDirectoryButtons();
        }

        private void OnButtonTestCreateFileInFolder_Click(object sender, EventArgs e)
        {
            string strTitle = string.Empty;
            string strMsg = string.Empty;
            string strFileName = string.Empty;
            string strPathFileName = string.Empty;
            string strPath = TestedDirectoryName;
            bool bCreated = false;

            for (bool bExists = true; bExists;)
            {
                strFileName = Guid.NewGuid().ToString() + ".bin";
                strPathFileName = Path.Combine(strPath, strFileName);
                bExists = File.Exists(strPathFileName);
            }

            try
            {
                using (FileStream stream = File.Open(strPathFileName, FileMode.CreateNew))
                {
                    using (BinaryWriter writer = new(stream))
                    {
                        writer.Write(303);
                        writer.Write(720);
                    }
                }

                strTitle = string.Format(CultureInfo.CurrentCulture,
                  "File '{0}' Creation Succeeded", strFileName);
                strMsg = string.Format(CultureInfo.CurrentCulture,
                  "This process succeeded in creating a new file in the locked folder {0}'{1}'",
                  Environment.NewLine, strPath);
                MessageBox.Show(strMsg, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                bCreated = true;
            }
            catch (IOException ex)
            {
                strMsg = string.Format(CultureInfo.CurrentCulture,
                  "Program could NOT create a new file in its locked folder '{0}'{1}{2}",
                  strPath, Environment.NewLine, ex.Message);
                MessageBox.Show(strMsg, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            if (bCreated)
            {
                File.Delete(strPathFileName);
            }
        }
        #endregion // Folder_locking

        #region The_rest

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion // The_rest

        #endregion // Event_handlers
    }
}

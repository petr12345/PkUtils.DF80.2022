using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using PK.PkUtils.Dump;
using PK.PkUtils.Interfaces;
using PK.PkUtils.SystemEx;
using PK.PkUtils.UI.Dialogs.MsgBoxes;
using PK.PkUtils.UI.Dialogs.PSTaskDialog;
using PK.PkUtils.UI.Utils;
using PK.PkUtils.Utils;
using PK.TestMsgBoxObserver.Properties;

namespace PK.TestMsgBoxObserver
{
    public partial class MainForm : Form, IDumper
    {
        #region Fields

        /// <summary> The message boxes observer. </summary>
        private MsgBoxObserver _observer;

        /// <summary>
        /// The wrapper around the TextBox control, providing the IDumper-behaviour for that control.
        /// </summary>
        private readonly DumperCtrlTextBoxWrapper _wrapper;

        // data for random names
        private readonly Random _random = new();
        private readonly List<string> _firstNames = ["Sergio", "Daniel", "Carolina", "David", "Reina", "Saul", "Bernard", "Danny", "Dimas", "Yuri", "Ivan", "Laura"];
        private readonly List<string> _lastNamesA = ["Tapia", "Gutierrez", "Rueda", "Galviz", "Yuli", "Rivera", "Mamami", "Saucedo", "Dominguez", "Escobar", "Martin", "Crespo"];

        #endregion // Fields

        #region Constructor(s)

        public MainForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // My own initialization
            this.Icon = Resources.App;
            _wrapper = new DumperCtrlTextBoxWrapper(this._textBoxMessages);
            InstallObserver();
            UpdateButtons();
        }
        #endregion // Constructor(s)

        #region Properties
        #endregion // Properties

        #region Methods

        #region Protected Methods

        /// <summary> Implementation helper, installs the MsgBoxObserver. </summary>
        protected void InstallObserver()
        {
            if (null == _observer)
            {
                _observer = new MsgBoxObserver();
                _observer.ObservedAction += new EventHandler<MsgBoxObserver.EventMsgBoxActionEventArgs>(_observer_evMsgBoxAction);
            }
        }

        /// <summary> Implementation helper, un-installs the MsgBoxObserver. </summary>
        protected void UninstallObserver()
        {
            Disposer.SafeDispose(ref _observer);
        }

        protected void GenerateText(out string strMsg, out string strCaption)
        {
            int a = _random.Next(0, _firstNames.Count);
            int b = _random.Next(0, _lastNamesA.Count);

            string strFirst = _firstNames[a];
            string strSecond = _lastNamesA[b];

            strMsg = string.Format(CultureInfo.CurrentCulture,
                "Any message you want, {0} {1}.", strFirst, strSecond);
            strCaption = strFirst;
        }

        #endregion // Protected Methods

        #region Private Methods

        private void UpdateButtons()
        {
        }
        #endregion //  Private Methods
        #endregion // Methods

        #region IDisposable Members

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UninstallObserver();
                Disposer.SafeDispose(ref components);
            }
            base.Dispose(disposing);
        }
        #endregion // IDisposable Members

        #region IDumper Members

        public bool DumpText(string text)
        {
            return _wrapper.DumpText(text);
        }

        public bool DumpWarning(string text)
        {
            return _wrapper.DumpWarning(text);
        }

        public bool DumpError(string text)
        {
            return _wrapper.DumpError(text);
        }

        public bool Reset()
        {
            return _wrapper.Reset();
        }
        #endregion // IDumper Members

        #region Event Handlers

        private void buttonShowMessageBox_Click(object sender, System.EventArgs e)
        {

            GenerateText(out string strMsg, out string strTitle);
            RtlAwareMessageBox.Show(this, strMsg, strTitle, MessageBoxIcon.Information);
        }

        private void btnShowTaskDialog_Click(object sender, EventArgs e)
        {
            string strMsg;
            string strContent;
            WinVer sysVer = SysInfo.GetWindowsVersion();

            if (sysVer < WinVer.WinVista)
            {
                strMsg = string.Format(CultureInfo.InvariantCulture,
                  "Since your system is {0}, you CANNOT display the TaskDialog.", sysVer);
                RtlAwareMessageBox.Show(this, strMsg, null, MessageBoxIcon.Error);
            }
            else
            {
                strContent = string.Format(CultureInfo.InvariantCulture,
                  "Since your system is {0}, you can enjoy the TaskDialog.", sysVer);
                GenerateText(out strMsg, out string strTitle);

                cTaskDialog.MessageBox(
                  this,
                  strTitle,
                  strMsg,
                  strContent,
                  eTaskDialogButtons.OK,
                  eSysIcons.Information);
            }
        }

        private void buttonClear_Click(object sender, System.EventArgs e)
        {
            Reset();
        }

        private void _observer_evMsgBoxAction(object sender, MsgBoxObserver.EventMsgBoxActionEventArgs e)
        {
            string strTxt;
            IntPtr hwnd = e.Hwnd;
            string strWndTitle = e.Info.Title;
            string strWndText = e.Info.Text;

            if (IntPtr.Size == 4)
            { // should call IntPtr.ToInt32()
                strTxt = string.Format(CultureInfo.InvariantCulture,
                  "MessageBox {0}, HWND = 0x{1:x8}, Title={2}, Text={3}",
                  e.CurrentAction, hwnd.ToInt32(), strWndTitle, strWndText);
            }
            else
            { // should call IntPtr.ToInt64()
                strTxt = string.Format(CultureInfo.InvariantCulture,
                  "MessageBox {0}, HWND = 0x{1:x8}, Title={2}, Text={3}",
                  e.CurrentAction, hwnd.ToInt64(), strWndTitle, strWndText);
            }

            this.DumpLine(strTxt);
        }

        private void _buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion // Event Handlers
    }
}

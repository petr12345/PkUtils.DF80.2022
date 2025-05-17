// Ignore Spelling: Utils, frm, Winapi, Prelink
//
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PK.PkUtils.Dump;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.UI.General;
using PK.PkUtils.Utils;
using PK.PkUtils.WinApi;
using PK.TestSystemHooking.Properties;

namespace PK.TestSystemHooking
{
    public partial class MainForm : FormWithLayoutPersistence, IDumper
    {
        #region Fields

        private Tester _tester;
        private Settings _settings;
        private readonly DumperCtrlTextBoxWrapper _wrapper;
        private const int _maxMsgHistoryItems = 1024;
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
            this._wrapper = new DumperCtrlTextBoxWrapper(_textBoxMessages, _maxMsgHistoryItems);
            this._tester = new Tester(this);

            LoadLayout();

            // Now for each class from Winapi namespace (Gdi32, Kernel32, User32, Win32 ),
            // let's call one function of them to make sure that Marshal.PrelinkAll is invoked,
            // this way verifying each DllImport can be found.
            Gdi32.TEXTMETRIC tm = new();
            using (Graphics g = this.CreateGraphics())
            {
                IntPtr dc = g.GetHdc();
                Gdi32.GetTextMetrics(dc, tm);
                g.ReleaseHdc(dc);
            }
            string strMemInfo = Kernel32.GetMemoryInfo();
            Trace.WriteLine(strMemInfo);
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary> Returns the textbox where messages are written to. </summary>
        protected internal TextBox TextBoxDumper
        {
            get { return _textBoxMessages; }
        }

        /// <summary> The tester instance. </summary>
        protected internal Tester Tester
        {
            get { return _tester; }
        }

        /// <summary> Gets the dumper for error logging. </summary>
        protected override IDumper Dumper { get => _wrapper; }

        #endregion // Properties

        #region Methods

        private static string SucceededOrFailedText(bool bOk)
        {
            return bOk ? "Succeeded" : "FAILED";
        }

        private void UpdateButtons()
        {
            bool bInstMouseH = Tester.IsMouseHookInstalled;
            bool bInstKeyboardH = Tester.IsKeyboardHookInstalled;
            bool bInstMouseLLH = Tester.IsMouseLowLevelHookInstalled;
            bool bInstKbLLH = Tester.IsKeyboardLowLevelHookInstalled;
            bool bInstCallWndProcH = Tester.IsCallWndProcHookInstalled;
            bool bInstAny = bInstMouseH || bInstKeyboardH || bInstMouseLLH || bInstKbLLH || bInstCallWndProcH;
            /* bool bCheckedAny = _checkBx_WH_MOUSE.Checked || _checkBx_WH_KEYBOARD.Checked || _checkBx_WH_KEYBOARD_LL.Checked || _checkBx_WH_CALLWNDPROCRET.Checked; */
            bool bCheckedAny = this.AllControls().OfType<CheckBox>().Aggregate(false, (total, next) => total || next.Checked);

            _buttonInstall.Enabled = !bInstAny && bCheckedAny;
            _buttonUninstall.Enabled = bInstAny;

            foreach (CheckBox ch in this.AllControls().OfType<CheckBox>())
            {
                ch.Enabled = !bInstAny;
            }
        }
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
                Disposer.SafeDispose(ref _tester);
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

        private void ButtonInstall_Click(object sender, System.EventArgs e)
        {
            bool bTest_WH_MOUSE = this._checkBx_WH_MOUSE.Checked;
            bool bTest_WH_KEYBOARD = this._checkBx_WH_KEYBOARD.Checked;
            bool bTest_WH_MOUSE_LL = this._checkBx_WH_MOUSE_LL.Checked;
            bool bTest_WH_KEYBOARD_LL = this._checkBx_WH_KEYBOARD_LL.Checked;
            bool bTest_WH_CALLWNDPROCRET = this._checkBx_WH_CALLWNDPROCRET.Checked;

            bool bOk;

            this.DumpLine("Installing following hooks: -----------------------------");

            if (bTest_WH_MOUSE)
            {
                bOk = Tester.SetMouseHook();
                this.DumpLine("Adding mouse hook " + SucceededOrFailedText(bOk));
            }

            if (bTest_WH_KEYBOARD)
            {
                bOk = Tester.SetKeyboardHook();
                this.DumpLine("Adding keyboard hook " + SucceededOrFailedText(bOk));
            }

            if (bTest_WH_MOUSE_LL)
            {
                bOk = Tester.SetLLMouseHook();
                this.DumpLine("Adding low-level mouse hook " + SucceededOrFailedText(bOk));
            }

            if (bTest_WH_KEYBOARD_LL)
            {
                bOk = Tester.SetLLKeyboardHook();
                this.DumpLine("Adding low-level keyboard hook " + SucceededOrFailedText(bOk));
            }

            if (bTest_WH_CALLWNDPROCRET)
            {
                bOk = Tester.SetCallWndProcHook();
                this.DumpLine("Adding CallWndProc hook " + SucceededOrFailedText(bOk));
            }

            this.DumpLine("------------------------------------------------------");

            UpdateButtons();
        }

        private void OnButtonUninstall_Click(object sender, System.EventArgs e)
        {
            this.DumpLine("Removing following hooks: ------------------------------");

            if (Tester.IsMouseHookInstalled)
            {
                Tester.UnhookMouseHook();
                this.DumpLine("Mouse hook removed.");
            }

            if (Tester.IsKeyboardHookInstalled)
            {
                Tester.UnhookKeyboardHook();
                this.DumpLine("Keyboard hook removed.");
            }

            if (Tester.IsMouseLowLevelHookInstalled)
            {
                Tester.UnhookLLMouseHook();
                this.DumpLine("Low Level Mouse hook removed.");
            }

            if (Tester.IsKeyboardLowLevelHookInstalled)
            {
                Tester.UnhookLLKeyboardHook();
                this.DumpLine("Low Level Keyboard hook removed.");
            }

            if (Tester.IsCallWndProcHookInstalled)
            {
                Tester.UnhookCallWndProcHook();
                this.DumpLine("CallWndProc hook removed.");
            }
            this.DumpLine("------------------------------------------------------");

            UpdateButtons();
        }

        private void OnButtonClear_Click(object sender, System.EventArgs e)
        {
            Dumper.Reset();
        }

        private void OnCheckBx_CheckedChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void MainFor_Load(object sender, EventArgs e)
        {
            _settings = new Settings();
            _checkBx_WH_MOUSE.Checked = _settings.Test_WH_MOUSE;
            _checkBx_WH_KEYBOARD.Checked = _settings.Test_WH_KEYBOARD;
            _checkBx_WH_MOUSE_LL.Checked = _settings.Test_WH_MOUSE_LL;
            _checkBx_WH_KEYBOARD_LL.Checked = _settings.Test_WH_KEYBOARD_LL;
            _checkBx_WH_CALLWNDPROCRET.Checked = _settings.Test_WH_CALLWNDPROCRET;

            UpdateButtons();
        }

        private void MainFor_FormClosing(object sender, FormClosingEventArgs e)
        {
            _settings.Test_WH_MOUSE = _checkBx_WH_MOUSE.Checked;
            _settings.Test_WH_KEYBOARD = _checkBx_WH_KEYBOARD.Checked;
            _settings.Test_WH_MOUSE_LL = _checkBx_WH_MOUSE_LL.Checked;
            _settings.Test_WH_KEYBOARD_LL = _checkBx_WH_KEYBOARD_LL.Checked;
            _settings.Test_WH_CALLWNDPROCRET = _checkBx_WH_CALLWNDPROCRET.Checked;
            _settings.Save();
        }

        private void OnButtonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion // Event Handlers
    }
}

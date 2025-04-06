using System;
using System.Globalization;
using System.Windows.Forms;
using PK.PkUtils.Dump;
using PK.PkUtils.Interfaces;
using PK.PkUtils.UI.Utils;
using PK.PkUtils.Utils;
using PK.TestConcurrency.Properties;

namespace PK.TestConcurrency
{
    public partial class MainForm : Form
    {
        #region Typedefs
        #endregion // Typedefs

        #region Fields

        /// <summary>
        /// The wrapper around the TextBox control, providing the IDumper-behaviour 
        /// for that control.
        /// </summary>
        private DumperCtrlTextBoxWrapper _wrapper;

        private Nullable<ConcurrentTest.TestMode> _testMode;

        private readonly Settings _settings = new();
        private const int _maxMsgHistoryItems = 1024;
        #endregion // Fields

        #region Constructor(s)
        public MainForm()
        {
            InitializeComponent();
            this.Icon = Resources.App;

            InitDumpOutput();
            InitFromSettings_TestType();
            Init_UI_Fro_TestType();
            UpdateButtons(false);
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary>
        /// Get currently assigned _testMode enum val.
        /// </summary>
        public ConcurrentTest.TestMode CurrentTestMode
        {
            get { return _testMode.Value; }
        }

        /// <summary>
        /// Returns the textbox where messages are written to
        /// </summary>
        protected internal TextBox TextBoxDumper
        {
            get { return _txtBxDump; }
        }

        private IDumper Dumper
        {
            get { return this._wrapper; }
        }
        #endregion // Properties

        #region Methods

        #region Protected Methods

        #region General Initialization

        /// <summary>
        /// Redirect the dumping output of DumperTextWriterAndTraceListener
        /// to the child textbox of this form.
        /// </summary>
        protected internal void InitDumpOutput()
        {
            if (null == _wrapper)
            {
                _wrapper = new DumperCtrlTextBoxWrapper(this._txtBxDump);
            }
        }

        protected void UpdateButtons(bool bTesting)
        {
            this._btnDoTest.Enabled = !bTesting;
            this._btnCleanLogHistory.Enabled = !bTesting;
            this._btnExit.Enabled = !bTesting;
        }
        #endregion // General Initialization

        #region UI_settings_related

        protected void InitFromSettings_TestType()
        {
            string strVal = Settings.Default.TestType;

            try
            {
                _testMode = (ConcurrentTest.TestMode)Enum.Parse(typeof(ConcurrentTest.TestMode), strVal, false);
            }
            catch (System.ArgumentException)
            {
                _testMode = default(ConcurrentTest.TestMode);
            }
        }

        protected void Init_UI_Fro_TestType()
        {
            ConcurrentTest.TestMode mode = this.CurrentTestMode;
            switch (mode)
            {
                case ConcurrentTest.TestMode.NoneLock:
                    this._radioButtonNone.Checked = true;
                    break;
                case ConcurrentTest.TestMode.InterlockedLock:
                    this._radioButtonInterlocked.Checked = true;
                    break;
                case ConcurrentTest.TestMode.MonitorLock:
                    this._radioButtonMonitorLock.Checked = true;
                    break;
                case ConcurrentTest.TestMode.ReaderWriterLock_Basic:
                    this._radioButtonReaderWriterLock_Basic.Checked = true;
                    break;
                case ConcurrentTest.TestMode.ReaderWriterLock_Wrapper:
                    this._radioButtonReaderWriterLock_Wrapper.Checked = true;
                    break;
                case ConcurrentTest.TestMode.ReaderWriterLockSli_Basic:
                    this._radioButtonReaderWriterLockSli_Basic.Checked = true;
                    break;
                case ConcurrentTest.TestMode.ReaderWriterLockSli_Wrapper:
                    this._radioReaderWriterLockSli_Wrapper.Checked = true;
                    break;
                default:
                    string strErr = string.Format(CultureInfo.InvariantCulture, "Invalid value of mode = '{0}'", mode);
                    throw new ArgumentException(strErr, "mode");
            }
        }

        protected ConcurrentTest.TestMode RequiredTestType()
        {
            ConcurrentTest.TestMode result = ConcurrentTest.TestMode.NoneLock;

            if (_radioButtonNone.Checked)
            {
                result = ConcurrentTest.TestMode.NoneLock;
            }
            else if (_radioButtonInterlocked.Checked)
            {
                result = ConcurrentTest.TestMode.InterlockedLock;
            }
            else if (_radioButtonMonitorLock.Checked)
            {
                result = ConcurrentTest.TestMode.MonitorLock;
            }
            else if (_radioButtonReaderWriterLock_Basic.Checked)
            {
                result = ConcurrentTest.TestMode.ReaderWriterLock_Basic;
            }
            else if (_radioButtonReaderWriterLock_Wrapper.Checked)
            {
                result = ConcurrentTest.TestMode.ReaderWriterLock_Wrapper;
            }
            else if (_radioButtonReaderWriterLockSli_Basic.Checked)
            {
                result = ConcurrentTest.TestMode.ReaderWriterLockSli_Basic;
            }
            else if (_radioReaderWriterLockSli_Wrapper.Checked)
            {
                result = ConcurrentTest.TestMode.ReaderWriterLockSli_Wrapper;
            }

            return result;
        }

        protected ConcurrentTest.TestMode UpdateRequiredTestType()
        {
            ConcurrentTest.TestMode result = RequiredTestType();
            this._testMode = result;
            return result;
        }
        #endregion // UI_settings_related

        #region Cleanup
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposer.SafeDispose(ref _wrapper);
                Disposer.SafeDispose(ref components);
            }
            base.Dispose(disposing);
        }
        #endregion // Cleanup
        #endregion Protected Methods
        #endregion // Methods

        #region Event_handlers

        private void _btnCleanLogHistory_Click(object sender, EventArgs e)
        {
            Dumper.Reset();
        }

        private void _btnDoTest_Click(object sender, EventArgs e)
        {
            ConcurrentTest.TestMode testMode = UpdateRequiredTestType();

            using (WaitCursor wc = new(this))
            {
                UpdateButtons(true);
                ConcurrentTest.DoTest(testMode, this.Dumper);
                UpdateButtons(false);
            }
        }

        private void _btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainFor_FormClosing(object sender, FormClosingEventArgs e)
        {
            UpdateRequiredTestType();
        }

        private void MainFor_FormClosed(object sender, FormClosedEventArgs e)
        {
            _settings.TestType = this.RequiredTestType().ToString();
            _settings.Save();
        }
        #endregion // Event_handlers
    }
}

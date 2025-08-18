using System;
using System.Globalization;
using System.Windows.Forms;
using PK.PkUtils.Dump;
using PK.PkUtils.Interfaces;
using PK.PkUtils.UI.Layout;
using PK.PkUtils.UI.Utils;
using PK.PkUtils.Utils;
using PK.Testmemcmp.Properties;

namespace PK.Testmemcmp
{
    public partial class MainForm : FormWithLayoutPersistence
    {
        #region Typedefs
        #endregion // Typedefs

        #region Fields

        /// <summary>
        /// The wrapper around the TextBox control, providing the IDumper-behavior 
        /// for that control.
        /// </summary>
        private DumperCtrlTextBoxWrapper _wrapper;

        private Nullable<ComparisonTest.TestMode> _testMode;

        private readonly Settings _settings = new();
        private const int _maxMsgHistoryItems = 1024;
        #endregion // Fields

        #region Constructor(s)
        public MainForm()
        {
            InitializeComponent();
            this.Icon = Resources.App;

            InitDumpOutput();
            LoadLayout();
            InitFromSettings();
            Init_UI_Fro_TestType();
            UpdateButtons(false);
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary>
        /// Get currently assigned _testMode enum val.
        /// </summary>
        public ComparisonTest.TestMode CurrentTestMode
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

        /// <summary> Gets the dumper for error logging. </summary>
        protected override IDumper Dumper { get => _wrapper; }

        #endregion // Properties

        #region Methods
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
            Control[] ctrls = new Control[] {
        this._btnDoTest,
        this._btnCleanLogHistory,
        this._btnExit,
        this._radioButtonSequenceEquals,
        this._radioButton_memcmp,
        this._numericArrayLenght,
        this._numericThreads
      };
            foreach (Control c in ctrls) { c.Enabled = !bTesting; }
        }
        #endregion // General Initialization

        #region UI_settings_related

        protected void InitFromSettings()
        {
            string strVal = Settings.Default.TestType;

            try
            {
                _testMode = (ComparisonTest.TestMode)Enum.Parse(typeof(ComparisonTest.TestMode), strVal, false);
            }
            catch (System.ArgumentException)
            {
                _testMode = default(ComparisonTest.TestMode);
            }

            _numericThreads.Value = Settings.Default.ParallelThreads;
            _numericArrayLenght.Value = Settings.Default.ArrayLength;
        }

        protected void Init_UI_Fro_TestType()
        {
            ComparisonTest.TestMode mode = this.CurrentTestMode;
            switch (mode)
            {
                case ComparisonTest.TestMode.Test_memcmp:
                    this._radioButton_memcmp.Checked = true;
                    break;
                case ComparisonTest.TestMode.Test_SequenceEquals:
                    this._radioButtonSequenceEquals.Checked = true;
                    break;
                default:
                    string strErr = string.Format(CultureInfo.InvariantCulture,
                      "Invalid value of mode = '{0}'", mode);
                    throw new ArgumentException(strErr, "mode");
            }
        }

        protected ComparisonTest.TestMode RequiredTestType()
        {
            ComparisonTest.TestMode result = ComparisonTest.TestMode.Test_memcmp;

            if (_radioButton_memcmp.Checked)
            {
                result = ComparisonTest.TestMode.Test_memcmp;
            }
            else if (_radioButtonSequenceEquals.Checked)
            {
                result = ComparisonTest.TestMode.Test_SequenceEquals;
            }
            return result;
        }

        protected ComparisonTest.TestMode UpdateRequiredTestType()
        {
            ComparisonTest.TestMode result = RequiredTestType();
            this._testMode = result;
            return result;
        }
        #endregion // UI_settings_related

        #region Trace_listening

        #endregion // Trace_listening

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
        #endregion // Methods

        #region Event_handlers

        private void _btnCleanLogHistory_Click(object sender, EventArgs e)
        {
            Dumper.Reset();
        }

        private void _btnDoTest_Click(object sender, EventArgs e)
        {
            ComparisonTest.TestMode testMode = UpdateRequiredTestType();

            using (WaitCursor wc = new(this))
            {
                UpdateButtons(true);
                ComparisonTest.DoTest(testMode, this.Dumper, (int)_numericThreads.Value, (int)_numericArrayLenght.Value);
                UpdateButtons(false);
            }
        }

        private void _btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainFor_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveLayout();
            UpdateRequiredTestType();
        }

        private void MainFor_FormClosed(object sender, FormClosedEventArgs e)
        {
            _settings.TestType = this.RequiredTestType().ToString();
            _settings.ParallelThreads = (uint)_numericThreads.Value;
            _settings.ArrayLength = (uint)_numericArrayLenght.Value;

            _settings.Save();
        }
        #endregion // Event_handlers
    }
}

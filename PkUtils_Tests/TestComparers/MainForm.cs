// Ignore Spelling: Comparers
//
// 
using System;
using System.Globalization;
using System.Windows.Forms;
using PK.PkUtils.Dump;
using PK.PkUtils.Interfaces;
using PK.PkUtils.LogTimingStatistic;
using PK.PkUtils.UI.Layout;
using PK.PkUtils.UI.Utils;
using PK.PkUtils.Utils;
using PK.TestComparers.Properties;

namespace PK.TestComparers;

public partial class MainForm : FormWithLayoutPersistence
{
    #region Typedefs
    #endregion // Typedefs

    #region Fields

    /// <summary>
    /// The wrapper around the TextBox control, providing the IDumper-behaviour 
    /// for that control.
    /// </summary>
    private DumperCtrlTextBoxWrapper _wrapper;

    private Nullable<ComparersTest.TestMode> _testMode;

    private readonly Settings _settings = new();
    private const int _maxMsgHistoryItems = 1024;
    #endregion // Fields

    #region Constructor(s)
    public MainForm()
    {
        InitializeComponent();

        this.Icon = Resources.App;
        LoadLayout();

        InitDumpOutput();
        InitFromSettings_TestType();
        Init_UI_From_TestType();
        UpdateButtons(false);
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Get currently assigned _testMode enum val.
    /// </summary>
    public ComparersTest.TestMode CurrentTestMode
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
            _testMode = (ComparersTest.TestMode)Enum.Parse(typeof(ComparersTest.TestMode), strVal, false);
        }
        catch (System.ArgumentException)
        {
            _testMode = default(ComparersTest.TestMode);
        }
    }

    protected void Init_UI_From_TestType()
    {
        ComparersTest.TestMode mode = this.CurrentTestMode;
        switch (mode)
        {
            case ComparersTest.TestMode.TestFuncEqualityComparer:
                this._radioButtonFuncEqualityComparer.Checked = true;
                break;
            case ComparersTest.TestMode.TestKeyEqualityComparer:
                this._radioButtonKeyEqualityComparer.Checked = true;
                break;
            default:
                string strErr = string.Format(CultureInfo.InvariantCulture, "Invalid value of mode = '{0}'", mode);
                throw new ArgumentException(strErr, "mode");
        }
    }

    protected ComparersTest.TestMode RequiredTestType()
    {
        ComparersTest.TestMode result = ComparersTest.TestMode.TestFuncEqualityComparer;

        if (_radioButtonFuncEqualityComparer.Checked)
        {
            result = ComparersTest.TestMode.TestFuncEqualityComparer;
        }
        else if (_radioButtonKeyEqualityComparer.Checked)
        {
            result = ComparersTest.TestMode.TestKeyEqualityComparer;
        }

        return result;
    }

    protected ComparersTest.TestMode UpdateRequiredTestType()
    {
        ComparersTest.TestMode result = RequiredTestType();
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
    #endregion // Methods

    #region Event_handlers

    private void On_btnCleanLogHistory_Click(object sender, EventArgs e)
    {
        Dumper.Reset();
    }

    private void On_btnDoTest_Click(object sender, EventArgs e)
    {
        ComparersTest.TestMode testMode = UpdateRequiredTestType();
        ITimingScope scope = new TimingScope();
        using WaitCursor wc = new(this);

        using (var first = new DisposableStopWatchLoggerEx(this.Dumper, nameof(UpdateButtons), "UpdateButtons(true)", scope))
        {
            UpdateButtons(true);
        }

        using (var second = new DisposableStopWatchLoggerEx(this.Dumper, nameof(ComparersTest), "ComparersTest.DoTest", scope))
        {
            ComparersTest.DoTest(testMode, this.Dumper);
        }

        using (var third = new DisposableStopWatchLoggerEx(this.Dumper, nameof(UpdateButtons), "UpdateButtons(false)", scope))
        {
            UpdateButtons(false);
        }

        scope.LogTimings(_wrapper, true);

    }

    private void On_btnExit_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void MainFor_FormClosing(object sender, FormClosingEventArgs e)
    {
        UpdateRequiredTestType();
        SaveLayout();
    }

    private void MainFor_FormClosed(object sender, FormClosedEventArgs e)
    {
        _settings.TestType = this.RequiredTestType().ToString();
        _settings.Save();
    }
    #endregion // Event_handlers
}

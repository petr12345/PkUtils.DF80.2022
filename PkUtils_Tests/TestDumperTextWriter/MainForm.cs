using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using PK.PkUtils.Dump;
using PK.PkUtils.Interfaces;
using PK.PkUtils.UI.Layout;
using PK.PkUtils.Utils;
using PK.TestDumperTextWriter.Properties;

namespace PK.TestDumperTextWriter;

public partial class MainForm : FormWithLayoutPersistence
{
    #region Fields

    /// <summary>
    /// The object encapsulating TextWriterTraceListener and its DumperTextWriter.
    /// </summary>
    private DumperTextWriterAndTraceListener _dumper;

    /// <summary>
    /// The wrapper around the TextBox control, providing the IDumper-behaviour 
    /// for that control.
    /// </summary>
    private DumperCtrlTextBoxWrapper _wrapper;

    #endregion // Fields

    #region Constructor(s)
    public MainForm()
    {
        InitializeComponent();
        this.Icon = Resources.App;
        LoadLayout();
        InitTraceListening();
        InitTimerTicking();
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Returns the textbox where messages are written to
    /// </summary>
    protected internal TextBox TextBoxDumper
    {
        get { return _txtBxDump; }
    }

    /// <summary>
    /// Give me the DumperTextWriterAndTraceListener that I have created ( if any )
    /// </summary>
    protected DumperTextWriterAndTraceListener MyListenerAndDumper
    {
        get { return _dumper; }
    }

    /// <summary>
    /// Is the trace listener created
    /// </summary>
    protected bool HasTraceListener
    {
        get { return (null != MyListenerAndDumper); }
    }

    /// <summary>
    /// Nomen est omen
    /// </summary>
    protected bool IsTraceListenerListening
    {
        get { return HasTraceListener && MyListenerAndDumper.IsTraceListenerListening; }
    }

    /// <summary> Gets the dumper for error logging. </summary>
    protected override IDumper Dumper { get => _wrapper; }

    #endregion // Properties

    #region Methods
    #region General Initialization

    /// <summary>
    /// Nomen est omen
    /// </summary>
    protected void InitTimerTicking()
    {
        this._timer.Tick += new EventHandler(OnTimer_Tick);
        this._timer.Enabled = true;
    }

    /// <summary>
    /// Create the listener, starts listening and updates related UI
    /// </summary>
    protected void InitTraceListening()
    {
        CreateTraceListener();
        StartTraceListening();
        SetDumpOutput();
        UpdateStartStopListeningUI();
    }

    /// <summary>
    /// Update the status and text of the button start/stop listening
    /// </summary>
    protected void UpdateStartStopListeningUI()
    {
        bool bListening = IsTraceListenerListening;
        string strText = bListening ? "Stop Listening" : "Start Listening";
        FontStyle newStyle = bListening ? FontStyle.Regular : FontStyle.Bold;

        _btnStartStopListening.Enabled = HasTraceListener;
        _btnStartStopListening.Text = strText;
        _btnStartStopListening.Font = new Font(_btnStartStopListening.Font, newStyle);
    }
    #endregion // General Initialization

    #region Trace_listening

    /// <summary>
    /// Create the trace listener, if there is no one yet
    /// </summary>
    protected internal void CreateTraceListener()
    {
        if (!HasTraceListener)
        {
            _dumper = new DumperTextWriterAndTraceListener();
            _dumper.CreateTraceListener();
        }
    }

    /// <summary>
    /// Destroy the trace listener if there is any,
    /// but first calls RequestStopDumper to stop its the worker thread.
    /// ( It is better to stop the writing thread before destroying its destination control....)
    /// </summary>
    /// <param name="bFlush"></param>
    protected internal void DestroyTraceListener(bool bFlush)
    {
        if (HasTraceListener)
        {
            MyListenerAndDumper.RequestStopDumper(bFlush);
            MyListenerAndDumper.StopTraceListening();
            Disposer.SafeDispose(ref _dumper);
        }
    }

    /// <summary>
    /// Start top trace listening, will you?
    /// </summary>
    /// <remarks> Trace listener has to be created already </remarks>
    protected internal void StartTraceListening()
    {
        Debug.Assert(HasTraceListener);
        if (!IsTraceListenerListening)
        {
            MyListenerAndDumper.StartTraceListening();
        }
    }

    /// <summary>
    /// Stop trace listening, will you?
    /// </summary>
    /// <remarks> Trace listener has to be created already </remarks>
    protected internal void StopTraceListening()
    {
        Debug.Assert(HasTraceListener);
        if (IsTraceListenerListening)
        {
            MyListenerAndDumper.StopTraceListening();
        }
    }

    /// <summary>
    /// Redirect the dumping output of DumperTextWriterAndTraceListener to given output
    /// </summary>
    protected internal void SetDumpOutput(IDumper output)
    {
        Debug.Assert(HasTraceListener);
        MyListenerAndDumper.SetDumpOutput(output);
    }

    /// <summary>
    /// Redirect the dumping output of DumperTextWriterAndTraceListener
    /// to the child textbox of this form.
    /// </summary>
    protected internal void SetDumpOutput()
    {
        if (null == _wrapper)
        {
            _wrapper = new DumperCtrlTextBoxWrapper(this._txtBxDump);
        }
        SetDumpOutput(_wrapper);
    }
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
            DestroyTraceListener(false);
            Disposer.SafeDispose(ref _wrapper);
            Disposer.SafeDispose(ref _timer);
            Disposer.SafeDispose(ref components);
        }
        base.Dispose(disposing);
    }
    #endregion // Cleanup
    #endregion // Methods

    #region Event_handlers

    private void OnTimer_Tick(object sender, EventArgs e)
    {
        string strOut = string.Format(CultureInfo.InvariantCulture, "Timer ticked");
        System.Diagnostics.Trace.WriteLine(strOut);
    }

    private void OnBtnStartStopListening_Click(object sender, EventArgs e)
    {
        if (IsTraceListenerListening)
            StopTraceListening();
        else
            StartTraceListening();
        UpdateStartStopListeningUI();
    }

    private void OnBtnCleanLogHistory_Click(object sender, EventArgs e)
    {
        Dumper.Reset();
    }

    private void OnBbtnExit_Click(object sender, EventArgs e)
    {
        this.Close();
    }
    #endregion // Event_handlers
}

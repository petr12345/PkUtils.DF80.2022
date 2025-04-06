using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PK.PkUtils.Dump;
using PK.PkUtils.Interfaces;
using PK.PkUtils.UI.General;
using PK.PkUtils.Utils;
using TestBinding.Properties;
using TestDataDef;

namespace TestBinding
{
    public partial class MyForm : FormWithLayoutPersistence
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

        private const int _maxMsgHistoryItems = 1024;
        #endregion // Fields

        #region Constructor(s)

        public MyForm()
        {
            InitDataOfDataSources();
            InitializeComponent();

            LoadLayout();
            InitTraceListening();
            InitDataSources();

            this.Icon = Resources.TestBinding;
        }
        #endregion // Constructor(s)

        #region Properties
        #region Public Properties

        public List<Person> Data_People { get; set; }
        public List<Salary> Data_Salaries { get; set; }
        public List<Role> Data_Roles { get; set; }
        public List<Package> Data_Packages { get; set; }

        public string Data_String { get; set; }
        #endregion // Public Properties

        #region Protected Properties

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

        /// <summary>   Nomen est omen. </summary>
        /// <value> True if this object is trace listener listening, false if not. </value>
        protected bool IsTraceListenerListening
        {
            get { return HasTraceListener && MyListenerAndDumper.IsTraceListenerListening; }
        }

        /// <summary> Gets the dumper for error logging. </summary>
        protected override IDumper Dumper { get => _wrapper; }

        #endregion // Protected Properties
        #endregion // Properties

        #region Methods

        #region General Initialization

        protected void InitDataOfDataSources()
        {
            Data_People = TestData.People();
            Data_Salaries = TestData.Salaries();
            Data_Roles = TestData.Roles();
            Data_Packages = TestData.Packages();
            Data_String = TestData.Sentence().Aggregate((workingSent, next) => workingSent + " " + next);
        }

        protected void InitDataSources()
        {
            // Change the settings from designer, to bind to concrete data instances.
            // ?? do I have to do it by hand ?
            this._myFormBindingSource.DataSource = this; /* typeof(TestBinding.MyForm); */
            this._programBindingSource.DataSource = Program.Instance; /* typeof(TestBinding.Program); */
        }

        protected void SubscribeDataSources()
        {
            // add event handlers
            BindingSource bs = this._myFormBindingSource;
            /*
            bs.CurrentChanged += new EventHandler(_myTextBindingSource_CurrentChanged);
            bs.CurrentItemChanged += new EventHandler(_myTextBindingSource_CurrentItemChanged);
            */

            bs.AddingNew += (s, ev) => Debug.WriteLine("AddingNew");
            bs.BindingComplete += (s, ev) => Debug.WriteLine("BindingComplete");
            bs.CurrentChanged += (s, ev) => Debug.WriteLine("CurrentChanged");
            bs.CurrentItemChanged += (s, ev) => Debug.WriteLine("CurrentItemChanged");
            bs.DataError += (s, ev) => Debug.WriteLine("DataError");
            bs.DataMemberChanged += (s, ev) => Debug.WriteLine("DataMemberChanged");
            bs.DataSourceChanged += (s, ev) => Debug.WriteLine("DataSourceChanged");
            bs.ListChanged += (s, ev) => Debug.WriteLine("ListChanged");
            bs.PositionChanged += (s, ev) => Debug.WriteLine("PositionChanged");
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
                Disposer.SafeDispose(ref components);
            }
            base.Dispose(disposing);
        }
        #endregion // Cleanup

        #endregion // Methods

        #region Event_handlers

        #region Event_handlers

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

        private void MainFor_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveLayout();
        }

        private void OnBtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion // Event_handlers

        private void MyFor_Load(object sender, EventArgs e)
        {
            SubscribeDataSources();
        }

        private void _myTextBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            MessageBox.Show("CurrentItemChanged");
        }

        private void _myTextBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            MessageBox.Show("CurrentChanged");
        }
        #endregion // Event_handlers
    }
}

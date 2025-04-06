/***************************************************************************************************************
*
* FILE NAME:   .\UI\MainForm.cs
* 
* PROJECT:     Demo project regarding Using Dataflow in a Windows Forms Application,
*              based on MSDN project https://msdn.microsoft.com/en-us/library/hh228605(v=vs.110).aspx
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains ( the main application window )
*
**************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms;
using PK.PkUtils.Dump;
using PK.PkUtils.Interfaces;
using PK.PkUtils.UI.General;
using PK.PkUtils.Utils;
using TestCompositeImages.CustomBlocks;
using TestCompositeImages.Properties;


namespace TestCompositeImages.UI
{
    /// <summary> A main application form. </summary>
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

        /// <summary> Pathname of the last chosen folder. </summary>
        private string _lastChosenFolder;

        /// <summary> true if this object is processing images now. </summary>
        private bool _IsProcessing;

        /// <summary> The head of the dataflow network. </summary>
        private ImageProcessingPipe _blocksPipe;

        /// <summary> The supported image extensions. </summary>
        private readonly IEnumerable<string> _SupportedImageExtensions = new string[] {
            "*.bmp", "*.gif", "*.jpg", "*.png", "*.tif" };
        #endregion // Fields

        #region Constructors

        /// <summary> Default constructor. </summary>
        public MainForm()
        {
            InitializeComponent();
            this.Icon = Resources.imagedit;

            InitPaths();
            LoadLayout();
            InitTraceListening();
            InitTimerTicking();
        }
        #endregion // Constructors

        #region Properties
        #region Public Properties

        /// <summary> Gets or sets a value indicating whether this object is processing. </summary>
        ///
        /// <value> true if this object is processing, false if not. </value>
        public bool IsProcessing
        {
            get { return _IsProcessing; }
        }

        /// <summary> Gets the main image processing pipe. It is assumed the pipe is created already. </summary>
        public ImageProcessingPipe MainPipe
        {
            get { Debug.Assert(_blocksPipe != null); return _blocksPipe; }
        }

        /// <summary> Gets or sets the pathname of the last chosen folder. </summary>
        ///
        /// <value> The pathname of the last chosen folder. </value>
        public string LastChosenFolder
        {
            get { return _lastChosenFolder; }
            protected set { _lastChosenFolder = value; }
        }
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

        /// <summary>
        /// Nomen est omen
        /// </summary>
        protected bool IsTraceListenerListening
        {
            get { return HasTraceListener && MyListenerAndDumper.IsTraceListenerListening; }
        }

        /// <summary> Gets the dumper for error logging. </summary>
        protected override IDumper Dumper { get => _wrapper; }

        #endregion // Protected Properties
        #endregion // Properties

        #region Methods

        #region Public Methods

        /// <summary> Displays a final bitmap described by <paramref name="bmp"/>. </summary>
        ///
        /// <param name="bitmap">   The bitmap. </param>
        public void DisplayFinalBitmap(Bitmap bmp)
        {
            // Display the bitmap.
            _pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            _pictureBox.Image = bmp;

            // Enable the user to select another folder.
            IndicateProcessingState(false);
        }

        /// <summary> Displays an error bitmap. </summary>
        public void DisplayErrorBitmap()
        {
            // Display the error image to indicate that the operation 
            // was canceled.
            _pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            _pictureBox.Image = _pictureBox.ErrorImage;

            // Enable the user to select another folder.
            IndicateProcessingState(false);
        }

        public void IndicateProcessingState(bool bProcessing)
        {
            if (bProcessing != IsProcessing)
            {
                this._IsProcessing = bProcessing;
                DoIndicateProcessingState();
            }
        }

        /// <summary> Indicate UI change for stated of stopped image processing. </summary>
        public void DoIndicateProcessingState()
        {
            bool started = this.IsProcessing;
            bool stopped = !started;

            _textBxImagesFolder.Enabled = stopped;
            _btnBrowseDirectory.Enabled = stopped;
            _btnImageSaveAs.Enabled = stopped;

            if (started)
            {
                Cursor = Cursors.WaitCursor;
                _btnTestImagesProcessing.Text = "Stop Images Processing";
                _btnTestImagesProcessing.Cursor = DefaultCursor;
            }
            else
            {
                Cursor = DefaultCursor;
                _btnTestImagesProcessing.Text = "Start Images Processing";
            }
        }
        #endregion // Public Methods

        #region Protected Methods

        #region General Initialization

        /// <summary>
        /// Nomen est omen
        /// </summary>
        protected void InitTimerTicking()
        {
            this._timer.Tick += new EventHandler(timer_Tick);
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

        #region Images-path-related

        /// <summary> Implementation helper. Returns whether a path is valid. </summary>
        ///
        /// <param name="s">    The string. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        protected static bool PathIsvalid(string s)
        {
            return !string.IsNullOrEmpty(s) && Directory.Exists(s);
        }

        protected bool AskForImagesPath(out string strValidPath)
        {
            string strLast;
            bool bRes = false;

            // initialize output to nothing
            strValidPath = string.Empty;

            // Create a FolderBrowserDialog object to enable the user to select a folder.
            FolderBrowserDialog dlg = new()
            {
                ShowNewFolderButton = false,
            };
            if (Directory.Exists(strLast = this.LastChosenFolder))
            {
                dlg.SelectedPath = strLast;
            }

            // Show the dialog and process the dataflow network. 
            if (FolderBrowserLauncher.ShowFolderBrowser(dlg, this) == DialogResult.OK)
            {
                strValidPath = dlg.SelectedPath;
                bRes = true;
            }

            return bRes;
        }

        /// <summary> Initializes the used paths. </summary>
        protected void InitPaths()
        {
            string strLastFolder = Settings.Default.LastSelectedFolder;

            if (!PathIsvalid(strLastFolder))
            {
                // Set the selected path to the common Sample Pictures folder, if it exists.
                strLastFolder = Path.Combine(
                   Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures),
                   "Sample Pictures");
            }

            if (PathIsvalid(strLastFolder))
            {
                LastChosenFolder = strLastFolder;
            }

            this._textBxImagesFolder.Text = LastChosenFolder;
        }

        /// <summary> Gets or create the main pipe. </summary>
        /// <returns>   The main pipe. </returns>
        protected ImageProcessingPipe CreateMainPipe()
        {
            // Create the image processing pipe if needed. 
            return _blocksPipe ??= new ImageProcessingPipe(this, this._SupportedImageExtensions);
        }

        /// <summary> Executes the start processing operation. </summary>
        ///
        /// <param name="strFolder">    Pathname of the folder. </param>
        protected void DoStartProcessing(string strFolder)
        {
            Debug.Assert(PathIsvalid(strFolder));
            Debug.Assert(!IsProcessing);

            this.LastChosenFolder = strFolder;
            // Create the image processing pipe if needed, and Post the selected path to the network.
            if (CreateMainPipe().HeadBlock.Post(strFolder))
            {
                // update UI
                IndicateProcessingState(true);
            }
            else
            {
                MessageBox.Show("The Head Block has refused to post an input value",
                    null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary> Executes the stop processing operation. </summary>
        protected void DoStopProcessing()
        {
            Debug.Assert(IsProcessing);

            // Signal the request for cancellation. The current component of  
            // the dataflow network will respond to the cancellation request. 
            MainPipe.CancellationTokenSource.Cancel();

            // update UI
            IndicateProcessingState(false);
        }
        #endregion // Images-path-related

        #region Overrides

        /// <summary> Raises the <see cref="E:System.Windows.Forms.Form.FormClosing" /> event. </summary>
        ///
        /// <param name="e"> A <see cref="T:System.Windows.Forms.FormClosingEventArgs" /> that contains the event
        /// data. </param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var sett = Settings.Default;

            if (PathIsvalid(this.LastChosenFolder))
            {
                sett.LastSelectedFolder = this.LastChosenFolder;
                sett.Save();
            }

            base.OnFormClosing(e);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly or indirectly by a user's code. 
        /// Managed and unmanaged resources can be disposed.
        /// If disposing equals false, the method has been called by the runtime from inside the finalizer
        /// and you should not reference other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"> If true, is called by IDisposable.Dispose. 
        /// Otherwise it is called by finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // release managed resources first 
            if (disposing)
            {
                if (disposing)
                {
                    Disposer.SafeDispose(ref _blocksPipe);
                    Disposer.SafeDispose(ref _wrapper);
                    Disposer.SafeDispose(ref _timer);
                    Disposer.SafeDispose(ref components);
                }
            }
            // release unmanaged resources here ... ( nothing to do )
            // and call base
            base.Dispose(disposing);
        }
        #endregion // Overrides
        #endregion // Protected Methods
        #endregion // Methods

        #region Event_handlers

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary> Event handler called by _btnStartStopListening_Click for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        private void btnStartStopListening_Click(object sender, EventArgs e)
        {
            if (IsTraceListenerListening)
                StopTraceListening();
            else
                StartTraceListening();
            UpdateStartStopListeningUI();
        }

        /// <summary> Event handler called by _btnCleanLogHistory for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        private void btnCleanLogHistory_Click(object sender, EventArgs e)
        {
            Dumper.Reset();
        }

        /// <summary> Event handler called by _timer for tick events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        private void timer_Tick(object sender, EventArgs e)
        {
            string strOut = string.Format(CultureInfo.InvariantCulture, "Timer ticked");
            System.Diagnostics.Trace.WriteLine(strOut);
        }

        private void btnImageSaveAs_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To be implemented", null, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary> Event handler called by _btnBrowseDirectory for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        private void btnBrowseDirectory_Click(object sender, EventArgs e)
        {

            // Show the dialog and process the dataflow network. 
            if (AskForImagesPath(out string strFolder))
            {
                this.LastChosenFolder = strFolder;
                this._textBxImagesFolder.Text = strFolder;
            }
        }

        /// <summary> Event handler called by btnTestImagesProcessing for click events. </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        private void btnTestImagesProcessing_Click(object sender, EventArgs e)
        {
            if (!this.IsProcessing)
            {
                string strErr;
                string strFolder = this._textBxImagesFolder.Text;

                if (!PathIsvalid(strFolder))
                {
                    strErr = string.Format(CultureInfo.CurrentCulture,
                        "The folder '{0}' is not valid. Please specify valid folder.", strFolder);
                    MessageBox.Show(strErr);
                }
                else
                {
                    DoStartProcessing(strFolder);
                }
            }
            else
            {
                DoStopProcessing();
            }
        }
        #endregion // Event_handlers
    }
}

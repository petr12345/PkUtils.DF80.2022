/***************************************************************************************************************
*
* FILE NAME:   .\MainForm.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class MainForm
*
**************************************************************************************************************/

using System;
using System.Text;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.SerialPortLib;
using PK.PkUtils.UI.Utils;
using PK.PkUtils.Utils;
using PK.TestSerialPortListener.Properties;

namespace PK.TestSerialPortListener
{
    /// <summary> The main application Form. </summary>
    [CLSCompliant(false)]
    public partial class MainForm : Form
    {
        #region Fields
        #endregion // Fields

        #region Constructor(s)

        /// <summary> Default constructor. </summary>
        public MainForm()
        {
            InitializeComponent();
            this.Icon = Resources.SerialPortListener;

            InitPortManager();
            InitDataSources();
            UpdateStartStopButtons();
            UpdateComboBoxes();
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary> Gets the manager for port. </summary>
        protected static SerialPortManager PortManager
        {
            get { return Program.Instance.PortManager; }
        }

        /// <summary> Gets a value indicating whether this object is listening on serial port. </summary>
        protected static bool IsPortManagerListening
        {
            get { return (PortManager != null) && PortManager.IsListening; }
        }
        #endregion // Properties

        #region Methods
        #region Protected Methods

        /// <summary> Init serial port manager. </summary>
        protected void InitPortManager()
        {
            Program.Instance.InitPortManager();
            Program.Instance.PortManager.NewSerialDataReceived += OnSpManager_NewSerialDataReceived;
        }

        /// <summary> Init data sources of user control _serialSettingsCtrl. </summary>
        protected void InitDataSources()
        {
            _serialSettingsCtrl.InitFromData(PortManager.CurrentSerialSettingsEx);
        }

        /// <summary> Updates the start stop buttons. </summary>
        protected void UpdateStartStopButtons()
        {
            bool bListening = IsPortManagerListening;

            _btnStart.Enabled = !bListening;
            _btnStop.Enabled = bListening;
        }

        /// <summary> Updates combo boxes. </summary>
        protected void UpdateComboBoxes()
        {
            bool bListening = IsPortManagerListening;
            foreach (var cb in this._serialSettingsCtrl.AllControls<ComboBox>(true))
            {
                cb.Enabled = !bListening;
            }
        }

        /// <summary> Overwrites the virtual method of the predecessor, to provide custom processing. </summary>
        /// <param name="args">Provides data for a cancel-able Form closing event. </param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var r = e.CloseReason;
            WaitCursor wc = (r == CloseReason.UserClosing) ? new WaitCursor(this) : null;

            base.OnFormClosing(e);
            if (e.Cancel)
            {
                Disposer.SafeDispose(ref wc);
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (PortManager != null)
                {
                    PortManager.NewSerialDataReceived -= OnSpManager_NewSerialDataReceived;
                }
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
            }
            base.Dispose(disposing);
        }
        #endregion Protected Methods

        #region Private Methods

        private static void SavePortSettings()
        {
            Program.Instance.SavePortSettings();
        }
        #endregion // Private Methods
        #endregion // Methods

        #region Event handlers

        /// <summary>
        /// Handler of PortManager.NewSerialDataReceived event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSpManager_NewSerialDataReceived(object sender, SerialDataEventArgs e)
        {
            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<SerialDataEventArgs>(OnSpManager_NewSerialDataReceived), new object[] { sender, e });
            }
            else
            {
                int maxTextLength = 1000; // maximum text length in text box
                if (_tbData.TextLength > maxTextLength)
                    _tbData.Text = _tbData.Text.Remove(0, _tbData.TextLength - maxTextLength);

                // This application is connected to a GPS sending ASCCI characters, so data is converted to text
                string str = Encoding.ASCII.GetString(e.Data);
                _tbData.AppendText(str);
                _tbData.ScrollToCaret();
            }
        }

        /// <summary>
        /// Handles the "Start Listening"-button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnStart_Click(object sender, EventArgs e)
        {
            using (WaitCursor wc = new(this))
            {
                PortManager.StartListening();
                UpdateStartStopButtons();
                UpdateComboBoxes();
            }
        }

        /// <summary>
        /// Handles the "Stop Listening"-button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnStop_Click(object sender, EventArgs e)
        {
            using (WaitCursor wc = new(this))
            {
                PortManager.StopListening(true);
                UpdateStartStopButtons();
                UpdateComboBoxes();
            }
        }

        /// <summary> Event handler. Called by MainForm for double click events. </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainFor_DoubleClick(object sender, EventArgs e)
        {
            _serialSettingsCtrl.TypeLabelVisible = !_serialSettingsCtrl.TypeLabelVisible;
        }

        /// <summary> Event handler. Called by MainForm for form closing events. </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainFor_FormClosing(object sender, FormClosingEventArgs e)
        {
            SavePortSettings();
        }
        #endregion // Event handlers
    }
}

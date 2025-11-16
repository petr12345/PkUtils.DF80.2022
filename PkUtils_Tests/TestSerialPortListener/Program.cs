#define USE_CUSTOM_PORT_SETTINGS

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using PK.PkUtils.DataStructures;
using PK.PkUtils.SerialPortLib;
using PK.PkUtils.Utils;
using PK.TestSerialPortListener.Properties;

namespace PK.TestSerialPortListener
{
    /// <summary> A program. </summary>
    [CLSCompliant(false)]
    public class Program : Singleton<Program>, IDisposable
    {
        #region Fields
        private SerialPortManager _spManager;
        private Settings _settings;
        #endregion // Fields

        #region Constructor(s)

        /// <summary> Specialized default constructor for use only by derived classes. </summary>
        protected Program()
        {
        }
        #endregion // Constructor(s)

        #region IDisposable Members

        /// <summary>
        /// Implements IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue to prevent finalization code 
            // for this object from executing a second time.
            GC.SuppressFinalize(this);
        }
        #endregion // IDisposable Members

        #region Properties

        /// <summary> Gets the manager for single port. </summary>
        public SerialPortManager PortManager
        {
            get { return _spManager; }
            protected set { _spManager = value; }
        }

        /// <summary> Gets a value indicating whether this object is listening on serial port. </summary>
        public bool IsListening
        {
            get { return (PortManager != null) && PortManager.IsListening; }
        }

        /// <summary> Gets settings. </summary>
        internal Settings MySettings
        {
            get
            {
                if (null == _settings)
                    _settings = Settings.Default;
                return _settings;
            }
        }
        #endregion // Properties

        #region Methods

        #region Public Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        #endregion // Public Methods

        #region Protected Methods

        /// <summary> Loads port settings. </summary>
        /// <returns> The port settings. </returns>
        protected internal SerialPortSettings LoadPortSettings()
        {
            var sett = MySettings;
            SerialPortSettings result = null;

#if USE_CUSTOM_PORT_SETTINGS
            result = sett.CustomPortSettings;
#else
      if (!string.IsNullOrEmpty(sett.PortSettings))
      {
        var adapter = new XMLSerializerAdapter<SerialPortSettings>();
        result = adapter.Deserialize(sett.PortSettings);
      }
#endif // USE_CUSTOM_PORT_SETTINGS
            return result;
        }

        /// <summary> Saves the port settings. </summary>
        protected internal void SavePortSettings()
        {
            // Create new SerialPortSettings instance in order to avoid dealing with derived class
            // ( if such class is used by PortManager.CurrentSerialSettings ).
            var current = new SerialPortSettings(PortManager.CurrentSerialSettingsEx);

#if USE_CUSTOM_PORT_SETTINGS
            MySettings.CustomPortSettings = current;
#else
      var adapter = new XMLSerializerAdapter<SerialPortSettings>();

      MySettings.PortSettings = adapter.StringSerialize(current, true);
#endif // USE_CUSTOM_PORT_SETTINGS
            MySettings.Save();
        }

        /// <summary> Init serial port manager. </summary>
        protected internal void InitPortManager()
        {
            if (null == PortManager)
            {
                InitPortManager(LoadPortSettings());
            }
        }

        /// <summary> Init serial port manager. </summary>
        /// <param name="portSettings"> The port settings. </param>
        protected void InitPortManager(SerialPortSettings portSettings)
        {
            _spManager = new SerialPortManager(portSettings, false);
        }

        /// <summary> Init serial port manager. </summary>
        /// <param name="port"> The port. </param>
        protected void InitPortManager(string port)
        {
            _spManager = new SerialPortManager(port, false);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the method has been
        /// called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed. If
        /// disposing equals false, the method has been called by the runtime from inside the finalizer and you should
        /// not reference other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_spManager",
          Justification = "Field is disposed by method call")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposer.SafeDispose(ref _spManager);
            }
        }
        #endregion // Protected Methods
        #endregion // Methods
    }
}

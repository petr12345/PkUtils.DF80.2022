// Ignore Spelling: Utils
//
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using PK.PkUtils.DataStructures;

#pragma warning disable IDE0290 // Use primary constructor

namespace PK.PkUtils.SerialPortLib;

/// <summary>
/// EventArgs used to send bytes received on serial port
/// </summary>
[CLSCompliant(false)]
public class SerialDataEventArgs : EventArgs
{
    /// <summary> The constructor. </summary>
    /// <param name="dataInByteArray"> Array of data in bytes. </param>
    public SerialDataEventArgs(byte[] dataInByteArray)
    {
        Data = dataInByteArray;
    }

    /// <summary>
    /// Byte array containing data from serial port
    /// </summary>
    public byte[] Data { get; protected set; }
}

/// <summary>
/// Manager for serial port data
/// </summary>
[CLSCompliant(true)]
public class SerialPortManager : Repository<SerialPort>, IDisposable
{
    #region Fields
    /// <summary> Event queue for all listeners interested in NewSerialDataReceived events. </summary>
    [CLSCompliant(false)]
    public event EventHandler<SerialDataEventArgs> NewSerialDataReceived;

    private readonly SerialPortSettingsEx _currentSerialSettingsEx = new();

    private SystemException _LastUpdateFromSelectedPortException;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public SerialPortManager()
      : this(string.Empty, true)
    {
    }

    /// <summary> The constructor. </summary>
    /// <param name="selectedPortName"> The selected port. This argument may be null. </param>
    /// <param name="stickToPortName" type="bool"> Use the value 'true' to let the called method to keep the
    ///  specified port name even if it is not an existing port. If the value 'false' is
    ///  used and the port with such name not exist, the code attempts to use first existing port instead. 
    ///  </param>
    public SerialPortManager(string selectedPortName, bool stickToPortName)
    {
        // Finding installed serial ports on hardware
        CurrentSerialSettingsEx.UpdatePortNameCollection();

        // Subscribing property changed is done inside Initialize
        /* CurrentSerialSettings.PropertyChanged += new PropertyChangedEventHandler(OnCurrentSerialSettings_PropertyChanged); */
        Initialize(selectedPortName, stickToPortName, true);
    }

    /// <summary> The constructor. </summary>
    /// <param name="portSettings"> The selected port settings. This argument may be null</param>
    /// <param name="stickToPortName" type="bool"> Use the value 'true' to let the called method to keep the
    ///  specified port name ( in portSettings.PortName) even if it is not an existing port. If the value 'false' is
    ///  used and the port with such name not exist, the code attempts to use first existing port instead. 
    ///  </param>
    public SerialPortManager(SerialPortSettings portSettings, bool stickToPortName)
    {
        // Finding installed serial ports on hardware
        CurrentSerialSettingsEx.UpdatePortNameCollection();

        // Select the port either from settings, or from PortNameCollection
        if ((portSettings != null) &&
          (stickToPortName || CurrentSerialSettingsEx.PortNameCollection.Contains(portSettings.PortName)))
        {
            Initialize(portSettings.PortName, stickToPortName, true);
            CurrentSerialSettingsEx.Assign(portSettings);
        }
        else
        {
            Initialize(string.Empty, false, true);
        }
    }

    /// <summary> Public constructor. </summary>
    /// <param name="port"> The port. This argument cannot be null.</param>
    /// <param name="owned"> Is true if port should be owned by this new instance, false otherwise. 
    ///                      If owning the port, it will be disposed with this instance disposing. </param>
    public SerialPortManager(SerialPort port, bool owned)
      : base(port, owned)
    {
        ArgumentNullException.ThrowIfNull(port);

        // Finding installed serial ports on hardware
        CurrentSerialSettingsEx.UpdatePortNameCollection();

        // Subscribing property changed is done inside Initialize
        /* CurrentSerialSettings.PropertyChanged += new PropertyChangedEventHandler(OnCurrentSerialSettings_PropertyChanged); */
        Initialize(port.PortName, true, true);
    }

    /// <summary> Public constructor. </summary>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal
    ///  values. </exception>
    /// <param name="port"> The port. This argument cannot be null. </param>
    /// <param name="owned"> Is true if port should be owned by this new instance, false otherwise. If
    ///  owning the port, it will be disposed with this instance disposing. </param>
    /// <param name="portSettings"> The selected port settings. This argument may be null;
    ///  if it is not null, values of port.PortName, portSettings.PortName must match. </param>
    /// <param name="stickToPortName" type="bool"> Use the value 'true' to let the called method to keep the
    ///  specified port name ( in portSettings.PortName) even if it is not an existing port. If the value
    ///  'false' is used and the port with such name not exist, the code attempts to use first existing port
    ///  instead. </param>
    public SerialPortManager(
        SerialPort port,
        bool owned,
        SerialPortSettings portSettings,
        bool stickToPortName)
        : base(port, owned)
    {
        ArgumentNullException.ThrowIfNull(port);

        if ((portSettings != null) && (!string.Equals(port.PortName, portSettings.PortName, StringComparison.Ordinal)))
        {
            throw new ArgumentException("port.PortName and portSettings.PortName do do not match", nameof(portSettings));
        }

        // Finding installed serial ports on hardware
        CurrentSerialSettingsEx.UpdatePortNameCollection();

        // Select the port either from settings, or from PortNameCollection
        if ((portSettings != null) &&
          (stickToPortName || CurrentSerialSettingsEx.PortNameCollection.Contains(portSettings.PortName)))
        {
            Initialize(portSettings.PortName, true, true);
            CurrentSerialSettingsEx.Assign(portSettings);
        }
        else
        {
            Initialize(port.PortName, stickToPortName, true);
            portSettings?.Assign(CurrentSerialSettingsEx);
        }
    }
    #endregion // Constructor(s)

    #region Finalizer

    /// <summary>
    /// The Finalizer. Uses C# destructor syntax for generation of finalizer method code.
    /// The actually generated method (finalizer) will run only if the Dispose method
    /// does not get called.
    /// </summary>
    /// <remarks>
    /// Note: the compiler actually expands the pseudo-destructor syntax here to following code:
    /// <code>
    /// protected override void Finalize()
    /// {
    ///   try
    ///   { // Your cleanup code here
    ///     Dispose(false);
    ///   }
    ///   finally
    ///   {
    ///     base.Finalize();
    ///   }
    /// }
    /// </code>
    /// </remarks>
    /// <seealso href="http://www.devx.com/dotnet/Article/33167/1954">
    /// When and How to Use Dispose and Finalize in C#</seealso>
    ~SerialPortManager()
    {
        Dispose(false);
    }
    #endregion // Finalizer

    #region Properties

    /// <summary> Gets the current serial port settings ex. </summary>
    public SerialPortSettingsEx CurrentSerialSettingsEx
    {
        get { return _currentSerialSettingsEx; }
        /* protected set { _currentSerialSettings = value; } better not to use at all */
    }

    /// <summary> The current serial port (if any) </summary>
    public SerialPort SerialPort
    {
        get { return base.Data; }
    }

    /// <summary>
    /// Gets the exception which occurred during the last update from selected port, if that has failed. 
    /// Returns null if there was no such fail.
    /// </summary>
    /// <value> The last update from selected port exception. </value>
    public SystemException LastUpdateFromSelectedPortException
    {
        get { return _LastUpdateFromSelectedPortException; }
    }

    /// <summary> Gets a value indicating whether the last update from selected port has failed. </summary>
    public bool LastUpdateFromSelectedPortHasFailed
    {
        get { return (null != LastUpdateFromSelectedPortException); }
    }

    /// <summary> Gets a value indicating whether this object is listening. </summary>
    public bool IsListening
    {
        get { return (SerialPort != null && SerialPort.IsOpen); }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Connects to a serial port defined through the current settings
    /// </summary>
    public void StartListening()
    {
        // Closing serial port if it is open
        StopListening(true);

        // Setting serial port 
        /* _serialPort = CurrentSerialSettings.ConfigureSerialPort(); */
        Keep(CurrentSerialSettingsEx.ConfigureSerialPort());

        // Subscribe to event and open serial port for data
        SerialPort.DataReceived += new SerialDataReceivedEventHandler(OnSerialPort_DataReceived);
        SerialPort.Open();
    }

    /// <summary> Closes the serial port and disposes it if <paramref name="disposePort "/> is true. </summary>
    /// <param name="disposePort"> true to dispose port. </param>
    public void StopListening(bool disposePort)
    {
        if (IsListening)
        {
            SerialPort.DataReceived -= new SerialDataReceivedEventHandler(OnSerialPort_DataReceived);
            SerialPort.Close();
        }

        if ((SerialPort != null) && disposePort)
        {
            DisposePortIfOwned();
        }
    }
    /// <summary>
    /// Initializes this object:
    /// - if there is matching port found in PortNameCollection, assigns the CurrentSerialSettings.PortName,  
    /// ( otherwise it will assign CurrentSerialSettings.PortName to first port from PortNameCollection);
    /// - afterward it subscribes CurrentSerialSettings.PropertyChanged to internal handler.
    /// </summary>
    /// <param name="selectedPortName"> Name of the port. </param>
    /// <param name="stickToPortName" type="bool"> Use the value 'true' to let the called method to keep the
    ///  specified port name even if it is not an existing port. If the value 'false' is
    ///  used and the port with such name not exist, the code attempts to use first existing port instead. 
    ///  </param>
    /// <param name="subscribePropChanged">true if should subscribe CurrentSerialSettings.PropertyChanged </param>
    protected virtual void Initialize(
      string selectedPortName,
      bool stickToPortName,
      bool subscribePropChanged)
    {
        string usedPortname = null;

        // Finding installed serial ports on hardware
        CurrentSerialSettingsEx.UpdatePortNameCollectionIfEmpty();
        // Subscribe property changed
        if (subscribePropChanged)
            CurrentSerialSettingsEx.PropertyChanged += new PropertyChangedEventHandler(OnCurrentSerialSettings_PropertyChanged);

        // Select the port either from settings, or from PortNameCollection
        if (!string.IsNullOrEmpty(selectedPortName))
        {
            if (stickToPortName || CurrentSerialSettingsEx.PortNameCollection.Contains(selectedPortName))
                usedPortname = selectedPortName;
        }

        if (!string.IsNullOrEmpty(usedPortname))
        {
            CurrentSerialSettingsEx.PortName = usedPortname;
        }
        else if (CurrentSerialSettingsEx.PortNameCollection.Any())
        {
            // If other serial ports is found, we select the first found
            string strPortSubstitute = CurrentSerialSettingsEx.PortNameCollection.First();

            if (null != SerialPort)
                SerialPort.PortName = strPortSubstitute;
            CurrentSerialSettingsEx.PortName = strPortSubstitute;
        }
    }

    /// <summary>
    /// Retrieves the current selected device's COMMPROP structure, and extracts the dwSettableBaud property
    /// </summary>
    protected void UpdateBaudRateCollection()
    {
        int dwSettableBaud = 0;
        bool bNeedTempPort = false;
        bool bNeedOpenPort = false;

        Debug.Assert(!IsListening);
        _LastUpdateFromSelectedPortException = null;

        try
        {
            if (bNeedTempPort = !this.HasData)
                Keep(new SerialPort(CurrentSerialSettingsEx.PortName));
            else
                Debug.Assert(string.Equals(SerialPort.PortName, CurrentSerialSettingsEx.PortName, StringComparison.Ordinal));
            if (bNeedOpenPort = !SerialPort.IsOpen)
                SerialPort.Open();

            object p = SerialPort.BaseStream.GetType().GetField("commProp",
              BindingFlags.Instance | BindingFlags.NonPublic).GetValue(SerialPort.BaseStream);
            dwSettableBaud = (int)p.GetType().GetField("dwSettableBaud",
              BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(p);
        }
        catch (UnauthorizedAccessException ex)
        {
            _LastUpdateFromSelectedPortException = ex;
        }
        finally
        {
            if (bNeedOpenPort)
                SerialPort.Close();
            if (bNeedTempPort)
                DisposePortIfOwned();
        }

        CurrentSerialSettingsEx.UpdateBaudRateCollection(dwSettableBaud);
    }

    /// <summary> Dispose port if owned; otherwise just assigns that property to null. </summary>
    protected void DisposePortIfOwned()
    {
        base.Forfeit();
    }

    /// <summary>
    /// Implements IDisposable. Do not make this method virtual. A derived class should not be able to override
    /// this method.
    /// </summary>
    /// <param name="disposing"> true to release both managed and unmanaged resources; false to release only
    /// unmanaged resources. </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Releasing managed data
            StopListening(!IsAttached);
        }
        base.Dispose(disposing);
    }
    #endregion // Methods

    #region Event handlers

    private void OnCurrentSerialSettings_PropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        // if serial port is changed, a new baud query is issued
        if (args.PropertyName.Equals(nameof(SerialPortSettingsEx.PortName), StringComparison.Ordinal))
        {
            if ((null != this.SerialPort) && (null != CurrentSerialSettingsEx))
                SerialPort.PortName = CurrentSerialSettingsEx.PortName;
            UpdateBaudRateCollection();
        }
    }

    private void OnSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs args)
    {
        int dataLength = SerialPort.BytesToRead;
        byte[] data = new byte[dataLength];
        int nbrDataRead = SerialPort.Read(data, 0, dataLength);
        if (nbrDataRead == 0)
            return;

        // Send data to whom ever interested
        NewSerialDataReceived?.Invoke(this, new SerialDataEventArgs(data));
    }
    #endregion // Event handlers
}

#pragma warning restore IDE0290 // Use primary constructor
/***************************************************************************************************************
*
* FILE NAME:   .\SerialPortLib\SerialPortSettingsBase.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: Contains the class SerialPortSettingsBase
*
**************************************************************************************************************/


// Ignore Spelling: Utils, Memberwise, rhs, Rts, Dtr
//
using System;
using System.Configuration;
using System.IO.Ports;
using PK.PkUtils.Cloning.Binary;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.SerialPortLib;

/// <summary>
/// Class containing properties related to a serial port 
/// </summary>
[Serializable]
[SettingsSerializeAs(SettingsSerializeAs.Xml)]
[CLSCompliant(true)]
public class SerialPortSettingsBase : MakeCloneableBinary<SerialPortSettingsBase>,
  IDeepCloneable<SerialPortSettingsBase>, IEquatable<SerialPortSettingsBase>
{
    #region Fields

    private string _portName = string.Empty;
    private int _baudRate = 4800;
    private Parity _parity = Parity.None;
    private int _dataBits = 8;
    private StopBits _stopBits = StopBits.One;
    private bool _RtsEnable;
    private bool _DtrEnable;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default argument-less constructor. </summary>
    public SerialPortSettingsBase()
    {
    }

    /// <summary> The copy constructor. </summary>
    ///
    /// <exception cref="ArgumentNullException">  Thrown when the argument <paramref name="rhs"/> is null. </exception>
    ///
    /// <param name="rhs">  The right hand side. </param>
    public SerialPortSettingsBase(SerialPortSettingsBase rhs)
    {
        Assign(rhs);
    }
    #endregion // Constructor(s)

    #region Properties
    /// <summary>
    /// The port to use (for example, COM1).
    /// </summary>
    public virtual string PortName
    {
        get { return _portName; }
        set { _portName = value; }
    }

    /// <summary>
    /// The baud rate.
    /// </summary>
    public virtual int BaudRate
    {
        get { return _baudRate; }
        set { _baudRate = value; }
    }

    /// <summary>
    /// One of the Parity values.
    /// </summary>
    public virtual Parity Parity
    {
        get { return _parity; }
        set { _parity = value; }
    }

    /// <summary>
    /// The data bits value.
    /// </summary>
    public virtual int DataBits
    {
        get { return _dataBits; }
        set { _dataBits = value; }
    }

    /// <summary>
    /// One of the StopBits values.
    /// </summary>
    public virtual StopBits StopBits
    {
        get { return _stopBits; }
        set { _stopBits = value; }
    }

    /// <summary> Gets or sets a value indicating whether is RTS enabled. </summary>
    public virtual bool RtsEnable
    {
        get { return _RtsEnable; }
        set { _RtsEnable = value; }
    }

    /// <summary> Gets or sets a value indicating whether is Dtr enabled. </summary>
    public virtual bool DtrEnable
    {
        get { return _DtrEnable; }
        set { _DtrEnable = value; }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Overwrites (implements) the virtual method of the base class. Delegates the functionality to
    /// IEquatable{SerialPortSettingsBase} method. </summary>
    ///
    /// <param name="obj">The object to compare with the current object. </param>
    ///
    /// <returns> true if the objects are considered equal, false if they are not. </returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as SerialPortSettingsBase);
    }

    /// <summary>
    /// Overwrites (implements) the virtual method of the base class. Returns the hash code of this SerialPortSettingsBase.
    /// </summary>
    ///
    /// <returns> A hash code for this object. </returns>
    public override int GetHashCode()
    {
        int suma = 0;

        suma += (PortName == null) ? 0 : PortName.ToUpperInvariant().GetHashCode();
        suma += BaudRate.GetHashCode();
        suma += Parity.GetHashCode();
        suma += DataBits.GetHashCode();
        suma += StopBits.GetHashCode();
        suma += RtsEnable.GetHashCode();
        suma += DtrEnable.GetHashCode();

        return suma;
    }

    /// <summary> Assigns the given right hand side. </summary>
    ///
    /// <exception cref="ArgumentNullException">  Thrown when the argument <paramref name="rhs"/> is null. </exception>
    ///
    /// <param name="rhs">  The right hand side. </param>
    public void Assign(SerialPortSettingsBase rhs)
    {
        ArgumentNullException.ThrowIfNull(rhs);

        this.PortName = rhs.PortName;
        this.BaudRate = rhs.BaudRate;
        this.Parity = rhs.Parity;
        this.DataBits = rhs.DataBits;
        this.StopBits = rhs.StopBits;
        this.RtsEnable = rhs.RtsEnable;
        this.DtrEnable = rhs.DtrEnable;
    }

    /// <summary>
    /// Create and configure serial port.
    /// Creates the serial port object, assigning to it this.PortName, BaudRate, DataBits, Parity, StopBits.
    /// </summary>
    /// <returns> The new serial port. </returns>
    public virtual SerialPort ConfigureSerialPort()
    {
        SerialPort result = new(this.PortName)
        {
            // configure serial port
            BaudRate = this.BaudRate,
            DataBits = this.DataBits,
            Parity = this.Parity,
            StopBits = this.StopBits,
            RtsEnable = this.RtsEnable,
            DtrEnable = this.DtrEnable
        };

        return result;
    }

    /// <summary> Member-wise compare. </summary>
    ///
    /// <param name="first">  The first compared instance. </param>
    /// <param name="second"> The second compared instance. </param>
    ///
    /// <returns> true objects are found member-wise equal, false if not. </returns>
    public static bool MemberwiseCompare(SerialPortSettingsBase first, SerialPortSettingsBase second)
    {
        bool result;

        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        result = string.Equals(first.PortName, second.PortName, StringComparison.OrdinalIgnoreCase);
        result = result && first.BaudRate.Equals(second.BaudRate);
        result = result && first.Parity.Equals(second.Parity);
        result = result && first.DataBits.Equals(second.DataBits);
        result = result && first.StopBits.Equals(second.StopBits);
        result = result && first.RtsEnable.Equals(second.RtsEnable);
        result = result && first.DtrEnable.Equals(second.DtrEnable);

        return result;
    }
    #endregion // Methods

    #region IEquatable<SerialPortSettingsBase> Members

    /// <summary> Tests if this SerialPortSettingsBase is considered equal to another. </summary>
    ///
    /// <param name="other">  The serial port settings base to compare to this object. </param>
    ///
    /// <returns> true if the objects are considered equal, false if they are not. </returns>
    public bool Equals(SerialPortSettingsBase other)
    {
        bool result = false;

        if (other is null)
        {
            /* result = false; already is */
        }
        else if (object.ReferenceEquals(other, this))
        {
            result = true;
        }
        else if (object.ReferenceEquals(other.GetType(), typeof(SerialPortSettingsBase)))
        {   // the other object type is directly SerialPortSettingsBase, just compare values
            result = MemberwiseCompare(this, other);
        }
        else
        {   // the other object is somehow derived, let him decide
            result = other.Equals((object)this);
        }
        return result;
    }
    #endregion // IEquatable<SerialPortSettingsBase> Members
}

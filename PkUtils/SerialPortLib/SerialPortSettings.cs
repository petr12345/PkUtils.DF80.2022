/***************************************************************************************************************
*
* FILE NAME:   .\SerialPortLib\SerialPortSettings.cs
*
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: Contains the class SerialPortSettings
*
**************************************************************************************************************/

// Ignore Spelling: Utils, rhs, Rts, Dtr
//
using System;
using System.ComponentModel;
using System.Configuration;
using System.IO.Ports;


namespace PK.PkUtils.SerialPortLib;

/// <summary>
/// Class containing properties related to a serial port, and implementing INotifyPropertyChanged
/// </summary>
[Serializable]
[SettingsSerializeAs(SettingsSerializeAs.Xml)]
[CLSCompliant(true)]
public class SerialPortSettings : SerialPortSettingsBase, IEquatable<SerialPortSettings>, INotifyPropertyChanged
{
    #region Constructor(s)

    /// <summary> Default argument-less constructor. </summary>
    public SerialPortSettings()
    {
    }

    /// <summary> The copy constructor. </summary>
    ///
    /// <exception cref="ArgumentNullException">  Thrown when the argument <paramref name="rhs"/> is null. </exception>
    ///
    /// <param name="rhs">  The right hand side. </param>
    public SerialPortSettings(SerialPortSettings rhs)
    {
        Assign(rhs);
    }
    #endregion // Constructor(s)

    #region Properties

    #region Overrides
    /// <summary>
    /// The port to use (for example, COM1).
    /// </summary>
    public override string PortName
    {
        set
        {
            if (!base.PortName.Equals(value, StringComparison.Ordinal))
            {
                base.PortName = value;
                SendPropertyChangedEvent(nameof(PortName));
            }
        }
    }

    /// <summary>
    /// The baud rate.
    /// </summary>
    public override int BaudRate
    {
        set
        {
            if (base.BaudRate != value)
            {
                base.BaudRate = value;
                SendPropertyChangedEvent(nameof(BaudRate));
            }
        }
    }

    /// <summary>
    /// One of the Parity values.
    /// </summary>
    public override Parity Parity
    {
        set
        {
            if (base.Parity != value)
            {
                base.Parity = value;
                SendPropertyChangedEvent(nameof(Parity));
            }
        }
    }

    /// <summary>
    /// The data bits value.
    /// </summary>
    public override int DataBits
    {
        set
        {
            if (base.DataBits != value)
            {
                base.DataBits = value;
                SendPropertyChangedEvent(nameof(DataBits));
            }
        }
    }

    /// <summary>
    /// One of the StopBits values.
    /// </summary>
    public override StopBits StopBits
    {
        set
        {
            if (base.StopBits != value)
            {
                base.StopBits = value;
                SendPropertyChangedEvent(nameof(StopBits));
            }
        }
    }

    /// <summary> Gets or sets a value indicating whether is RTS enabled. </summary>
    public override bool RtsEnable
    {
        set
        {
            if (base.RtsEnable != value)
            {
                base.RtsEnable = value;
                SendPropertyChangedEvent(nameof(RtsEnable));
            }
        }
    }

    /// <summary> Gets or sets a value indicating whether is Dtr enabled. </summary>
    public override bool DtrEnable
    {
        set
        {
            if (base.DtrEnable != value)
            {
                base.DtrEnable = value;
                SendPropertyChangedEvent(nameof(DtrEnable));
            }
        }
    }
    #endregion // Overrides
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Overwrites (implements) the virtual method of the base class. Delegates the functionality to
    /// IEquatable{SerialPortSetting} method. </summary>
    ///
    /// <param name="obj">The object to compare with the current object. </param>
    ///
    /// <returns> true if the objects are considered equal, false if they are not. </returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as SerialPortSettings);
    }

    /// <summary>
    /// Overwrites (implements) the virtual method of the base class. Returns the hash code of this SerialPortSettings.
    /// </summary>
    ///
    /// <returns> A hash code for this object. </returns>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary> Send a PropertyChanged event </summary>
    /// <param name="propertyName">Name of changed property</param>
    protected void SendPropertyChangedEvent(string propertyName)
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion // Methods

    #region INotifyPropertyChanged Members

    /// <summary> Occurs when a property value changes. </summary>
    public event PropertyChangedEventHandler PropertyChanged;
    #endregion // INotifyPropertyChanged Members

    #region IEquatable<SerialPortSettings> Members

    /// <summary> Tests if this SerialPortSettings is considered equal to another. </summary>
    ///
    /// <param name="other">  The serial port settings to compare to this object. </param>
    ///
    /// <returns> true if the objects are considered equal, false if they are not. </returns>
    public bool Equals(SerialPortSettings other)
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
        else if (object.ReferenceEquals(other.GetType(), typeof(SerialPortSettings)))
        {   // the other object type is directly SerialPortSettings, just compare values
            result = MemberwiseCompare(this, other);
        }
        else
        {   // the other object is somehow derived, let him decide
            result = other.Equals((object)this);
        }

        return result;
    }
    #endregion // IEquatable<SerialPortSettings> Members
}

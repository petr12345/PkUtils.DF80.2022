// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO.Ports;
using System.Linq;

#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.PkUtils.SerialPortLib;

/// <summary>
/// Class containing properties related to a serial port 
/// </summary>
[Serializable]
[SettingsSerializeAs(SettingsSerializeAs.Xml)]
[CLSCompliant(true)]
public class SerialPortSettingsEx : SerialPortSettings, IEquatable<SerialPortSettingsEx>
{
    #region Fields

    private IEnumerable<string> _portNameCollection = [];
    private readonly BindingList<int> _baudRateCollection = [];
    private int[] _dataBitsCollection = [5, 6, 7, 8];
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default argument-less constructor. </summary>
    public SerialPortSettingsEx()
    {
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Available ports on the computer. The collection is updated by <see cref="UpdatePortNameCollection"/>
    /// </summary>
    public IEnumerable<string> PortNameCollection
    {
        get { return _portNameCollection; }
        private set { _portNameCollection = value; }
    }

    /// <summary>
    /// Available baud rates for current serial port
    /// </summary>
    public BindingList<int> BaudRateCollection
    {
        get { return _baudRateCollection; }
    }

    /// <summary>
    /// Available databits setting
    /// </summary>
    public int[] DataBitsCollection
    {
        get { return _dataBitsCollection; }
        set { _dataBitsCollection = value; }
    }
    #endregion // Properties

    #region Methods

    /// <summary> Updates the range of possible baud rates for device. </summary>
    /// <param name="possibleBaudRates">dwSettableBaud parameter from the COMMPROP Structure</param>
    /// <returns>An updated list of values</returns>
    public void UpdateBaudRateCollection(int possibleBaudRates)
    {
        const int BAUD_075 = 0x00000001;
        const int BAUD_110 = 0x00000002;
        const int BAUD_150 = 0x00000008;
        const int BAUD_300 = 0x00000010;
        const int BAUD_600 = 0x00000020;
        const int BAUD_1200 = 0x00000040;
        const int BAUD_1800 = 0x00000080;
        const int BAUD_2400 = 0x00000100;
        const int BAUD_4800 = 0x00000200;
        const int BAUD_7200 = 0x00000400;
        const int BAUD_9600 = 0x00000800;
        const int BAUD_14400 = 0x00001000;
        const int BAUD_19200 = 0x00002000;
        const int BAUD_38400 = 0x00004000;
        const int BAUD_56K = 0x00008000;
        const int BAUD_57600 = 0x00040000;
        const int BAUD_115200 = 0x00020000;
        const int BAUD_128K = 0x00010000;

        _baudRateCollection.Clear();

        if ((possibleBaudRates & BAUD_075) > 0)
            _baudRateCollection.Add(75);
        if ((possibleBaudRates & BAUD_110) > 0)
            _baudRateCollection.Add(110);
        if ((possibleBaudRates & BAUD_150) > 0)
            _baudRateCollection.Add(150);
        if ((possibleBaudRates & BAUD_300) > 0)
            _baudRateCollection.Add(300);
        if ((possibleBaudRates & BAUD_600) > 0)
            _baudRateCollection.Add(600);
        if ((possibleBaudRates & BAUD_1200) > 0)
            _baudRateCollection.Add(1200);
        if ((possibleBaudRates & BAUD_1800) > 0)
            _baudRateCollection.Add(1800);
        if ((possibleBaudRates & BAUD_2400) > 0)
            _baudRateCollection.Add(2400);
        if ((possibleBaudRates & BAUD_4800) > 0)
            _baudRateCollection.Add(4800);
        if ((possibleBaudRates & BAUD_7200) > 0)
            _baudRateCollection.Add(7200);
        if ((possibleBaudRates & BAUD_9600) > 0)
            _baudRateCollection.Add(9600);
        if ((possibleBaudRates & BAUD_14400) > 0)
            _baudRateCollection.Add(14400);
        if ((possibleBaudRates & BAUD_19200) > 0)
            _baudRateCollection.Add(19200);
        if ((possibleBaudRates & BAUD_38400) > 0)
            _baudRateCollection.Add(38400);
        if ((possibleBaudRates & BAUD_56K) > 0)
            _baudRateCollection.Add(56000);
        if ((possibleBaudRates & BAUD_57600) > 0)
            _baudRateCollection.Add(57600);
        if ((possibleBaudRates & BAUD_115200) > 0)
            _baudRateCollection.Add(115200);
        if ((possibleBaudRates & BAUD_128K) > 0)
            _baudRateCollection.Add(128000);

        SendPropertyChangedEvent(nameof(BaudRateCollection));
    }

    /// <summary> Updates the port name collection <see cref="PortNameCollection"/>. </summary>
    /// <remarks> Filters-out the result of SerialPort.GetPortNames, in order to get only names 
    /// beginning with 'COM'.  This is needed in case there are some virtual ports are present,
    /// if they were installed by some special software. 
    /// For instance, if the "com0com + com2tcp" utilities are present, the result of call 
    /// SerialPort.GetPortNames() may return ports like  "CNCA0"or "CNCB0".
    /// </remarks>
    /// <seealso href="http://com0com.sourceforge.net/doc/UsingCom0com.pdf">
    /// com0com + com2tcp</seealso>
    public void UpdatePortNameCollection()
    {
        var allNames = SerialPort.GetPortNames();
        List<string> filteredNames = allNames.Where(
          portName => portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase)).ToList();

        /* if needed for debug test
        filteredNames.Clear(); */

        PortNameCollection = filteredNames;
    }

    /// <summary> Updates the port name collection if empty. </summary>
    public void UpdatePortNameCollectionIfEmpty()
    {
        if (!PortNameCollection.Any())
        {
            UpdatePortNameCollection();
        }
    }

    /// <summary>
    /// Overwrites (implements) the virtual method of the base class. Delegates the functionality to
    /// IEquatable{SerialPortSettingsEx} method. </summary>
    ///
    /// <param name="obj">The object to compare with the current object. </param>
    ///
    /// <returns> true if the objects are considered equal, false if they are not. </returns>
    public override bool Equals(object obj)
    {
        return Equals(obj as SerialPortSettingsEx);
    }

    /// <summary>
    /// Overwrites (implements) the virtual method of the base class. Returns the hash code of this
    /// SerialPortSettingsEx.
    /// </summary>
    /// <returns> A hash code for this object. </returns>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion // Methods

    #region IEquatable<SerialPortSettingsEx> Members

    /// <summary> Tests if this SerialPortSettingsEx is considered equal to another. </summary>
    ///
    /// <param name="other">  The serial port settings ex to compare to this object. </param>
    ///
    /// <returns> true if the objects are considered equal, false if they are not. </returns>
    public bool Equals(SerialPortSettingsEx other)
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
        else if (object.ReferenceEquals(other.GetType(), typeof(SerialPortSettingsEx)))
        {   // the other object type is directly SerialPortSettingsEx, just compare values
            result = MemberwiseCompare(this, other);
        }
        else
        {   // the other object is somehow derived, let him decide
            result = other.Equals((object)this);
        }

        return result;
    }
    #endregion // IEquatable<SerialPortSettingsEx> Members
}
#pragma warning restore IDE0305
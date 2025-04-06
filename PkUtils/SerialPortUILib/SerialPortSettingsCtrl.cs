/***************************************************************************************************************
*
* FILE NAME:   .\SerialPortUILib\SerialPortSettingsCtrl.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: Contains the class SerialPortSettings
*
**************************************************************************************************************/


// Ignore Spelling: Utils, Ctrl
//
using System;
using System.Linq;
using PK.PkUtils.SerialPortLib;

namespace PK.PkUtils.SerialPortUILib;

/// <summary> A serial port settings user control. </summary>
[CLSCompliant(true)]
public partial class SerialPortSettingsCtrl : SerialPortSettingsCtrl_Design
{
    #region Fields
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public SerialPortSettingsCtrl()
    {
        InitializeComponent();
    }
    #endregion // Constructor(s)

    #region Properties
    #endregion // Properties

    #region Methods

    /// <summary> Sets the error text for the port combo box. </summary>
    /// <param name="text"> The error text. </param>
    public void SetPortComboError(string text)
    {
        _ErrorProvider.SetError(_portNameComboBox, text);
    }

    /// <summary> Clears any error displaying from the port combo box. </summary>
    public void ClearPortComboError()
    {
        _ErrorProvider.Clear();
    }

    /// <summary> Initializes the data sources. </summary>
    protected override void InitDataSources()
    {
        SerialPortSettingsEx mySerialSettings = this.Data;

        base.InitDataSources();

        if (null != mySerialSettings)
        {
            _portNameComboBox.DataSource = mySerialSettings.PortNameCollection.ToList();
            _baudRateComboBox.DataSource = mySerialSettings.BaudRateCollection;
            _dataBitsComboBox.DataSource = mySerialSettings.DataBitsCollection;
            _parityComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            _stopBitsComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));
            // should assign following .DataSource as last-one, 
            // otherwise there were problems with _portNameComboBox ( as if changing value on itself )
            _serialSettingsBindingSource.DataSource = mySerialSettings;
        }
    }
    #endregion // Methods
}

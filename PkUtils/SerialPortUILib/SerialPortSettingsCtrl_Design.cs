/***************************************************************************************************************
*
* FILE NAME:   .\SerialPortUILib\SerialPortSettingsCtrl_Design.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: Contains the class SerialPortSettingsCtrl_Design
*
**************************************************************************************************************/


// Ignore Spelling: Utils, Ctrl
//
using System;
using PK.PkUtils.SerialPortLib;
using PK.PkUtils.UI.General;

namespace PK.PkUtils.SerialPortUILib;

/// <summary>
/// An auxiliary class that will allow the VS designer work on a derived SerialSettingsCtrl
/// The designer cannot handle controls or forms derived directly from generics.
/// For more info see
/// http://adamhouldsworth.blogspot.co.uk/2010/02/winforms-visual-inheritance-limitations.html
/// </summary>
[CLSCompliant(true)]
public partial class SerialPortSettingsCtrl_Design : BaseDataCtrl<SerialPortSettingsEx>
{
    /// <summary> Default constructor. </summary>
    public SerialPortSettingsCtrl_Design()
    {
        InitializeComponent();
    }
}

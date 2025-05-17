/***************************************************************************************************************
*
* FILE NAME:   .\Dump\DumperStringWrapper.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The class DumperStringWrapper
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//

using System;
using System.Text;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.Dump;

/// <summary>
/// A simple string-writing wrapper, providing IDumper interface implementation.
/// All operations are performed on underlying StringBuilder-based buffer.
/// </summary>
[CLSCompliant(true)]
public class DumperStringWrapper : IDumper
{
    #region Fields
    private StringBuilder _sb = new();
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Constructor
    /// </summary>
    public DumperStringWrapper()
    {
    }
    #endregion // Constructor(s)

    #region Methods

    /// <summary>
    /// Overrides the implementation of the base class; 
    /// returns the underlying StringBuilder as a string.
    /// </summary>
    /// <returns>Returns a string that represents the current DumperStringWrapper object. </returns>
    public override string ToString()
    {
        return _sb.ToString();
    }
    #endregion // Methods

    #region IDumper Members
    /// <summary>	Implementation of IDumper.DumpText. </summary>
    /// <param name="text">	The dumped text. </param>
    /// <returns>	true if it succeeds, false if it fails. </returns>
    public bool DumpText(string text)
    {
        _sb.Append(text);
        return true;
    }

    /// <inheritdoc/>
    public bool DumpWarning(string text)
    {
        return DumpText(text);
    }

    /// <inheritdoc/>
    public bool DumpError(string text)
    {
        return DumpText(text);
    }

    /// <summary>	Implementation of IDumper.Reset.  Cleans any previously dumped contents. </summary>
    /// <returns>	true if it succeeds, false if it fails. </returns>
    public bool Reset()
    {
        _sb = new StringBuilder();
        return true;
    }
    #endregion // IDumper Members
}

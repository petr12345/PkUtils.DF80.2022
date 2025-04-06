/***************************************************************************************************************
*
* FILE NAME:   .\Interfaces\IClipboardable.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of interface IClipboardable
*
**************************************************************************************************************/

// Ignore Spelling: Utils, Clipboardable
//
using System;

namespace PK.PkUtils.Interfaces;

/// <summary>
/// Interface IClipboardable defines functionality that should be supported by any object
/// that communicates with clipboard.
/// </summary>
[CLSCompliant(true)]
public interface IClipboardable
{
    /// <summary>
    /// Is the object in state where it can cut data to clipboard.
    /// For instance, for case of the text editor window, this means is there anything selected?
    /// </summary>
    bool CanCut { get; }
    /// <summary>
    /// Is the object in state where it can copy data to clipboard.
    /// For instance, for case of the text editor window, this means is there anything selected?
    /// </summary>
    bool CanCopy { get; }
    /// <summary>
    /// Is the object in state where it can paste data from clipboard.
    /// For instance, for case of the text editor window, 
    /// is there any supported clipboard format data available on clipboard?
    /// </summary>
    bool CanPaste { get; }

    /// <summary>
    /// Nomen est omen.
    /// </summary>
    void Cut();
    /// <summary>
    /// Nomen est omen.
    /// </summary>
    void Copy();
    /// <summary>
    /// Nomen est omen.
    /// </summary>
    void Paste();
}

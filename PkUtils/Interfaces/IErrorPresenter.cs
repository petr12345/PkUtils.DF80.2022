/***************************************************************************************************************
*
* FILE NAME:   .\Interfaces\IErrorPresenter.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of interface IErrorPresenter
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//
using System;
using System.Windows.Forms;

namespace PK.PkUtils.Interfaces;

/// <summary>
/// Defines the functionality needed for displaying ('presenting') application errors.
/// </summary>
/// <remarks> 
/// </remarks>
[CLSCompliant(true)]
public interface IErrorPresenter
{
    /// <summary> Show error specified by the text (first argument), the rest of the arguments is interpreted
    /// based on their type: <br/>
    /// - the argument of type string is interpreted as dialog title <br/>
    /// - the argument of type MessageBoxButtons affect buttons in the dialog <br/>
    /// - the argument of type MessageBoxIcon affect icons in the dialog <br/>
    /// - the argument of type MessageBoxDefaultButton affect default button in the dialog <br/> </summary>
    ///
    /// <param name="strErr"> The error message. </param>
    /// <param name="arguments"> Additional optional arguments, in arbitrary order, that specify in
    /// more detail how the error message should be presented. </param>
    ///
    /// <returns> The WinForms dialog result. </returns>
    DialogResult ShowError(string strErr, params object[] arguments);

    /// <summary> Show error specified by the exception (first argument), the rest of the arguments is
    /// interpreted based on their type: <br/>
    /// - the argument of type string is interpreted as dialog title <br/>
    /// - the argument of type MessageBoxButtons affect buttons in the dialog <br/>
    /// - the argument of type MessageBoxIcon affect icons in the dialog <br/>
    /// - the argument of type MessageBoxDefaultButton affect default button in the dialog <br/> </summary>
    ///
    /// <param name="ex">   The previously caught exception that represents an error that has occurred. </param>
    /// <param name="arguments"> Additional optional arguments, in arbitrary order, that specify in
    /// more detail how  the error message should be presented. </param>
    ///
    /// <returns> The WinForms dialog result. </returns>
    DialogResult ShowError(Exception ex, params object[] arguments);
}

/// <summary>
/// Extends the IErrorPresenter interface by adding methods that should be used
/// for displaying a severe (fatal) errors.
/// </summary>
[CLSCompliant(true)]
public interface IErrorPresenterEx : IErrorPresenter
{
    /// <summary>
    /// Show the fatal application error specified by the text (first argument), 
    /// the rest of the arguments is interpreted based on their type: <br/>
    /// - the argument of type string is interpreted as dialog title <br/>
    /// - the argument of type MessageBoxButtons affect buttons in the dialog <br/>
    /// - the argument of type MessageBoxIcon affect icons in the dialog <br/>
    /// - the argument of type MessageBoxDefaultButton affect default button in the dialog <br/>
    /// </summary>
    /// <param name="strErr">The error message.</param>
    /// <param name="arguments"> Additional optional arguments, in arbitrary order, that specify in
    /// more detail how  the error message should be presented. </param>
    /// <returns>The WinForms dialog result </returns>
    DialogResult ShowFatalError(string strErr, params object[] arguments);

    /// <summary>
    /// Show the fatal application error specified by the exception (first argument), 
    /// the rest of the arguments is interpreted based on their type: <br/>
    /// - the argument of type string is interpreted as dialog title <br/>
    /// - the argument of type MessageBoxButtons affect buttons in the dialog <br/>
    /// - the argument of type MessageBoxIcon affect icons in the dialog <br/>
    /// - the argument of type MessageBoxDefaultButton affect default button in the dialog <br/>
    /// </summary>
    /// <param name="ex">   The previously caught exception  that represents an error that has occurred. </param>
    /// <param name="arguments"> Additional optional arguments, in arbitrary order, that specify in
    /// more detail how  the error message should be presented. </param>
    /// <returns>The WinForms dialog result </returns>
    DialogResult ShowFatalError(Exception ex, params object[] arguments);
}

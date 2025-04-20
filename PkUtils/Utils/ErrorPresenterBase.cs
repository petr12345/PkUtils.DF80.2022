/***************************************************************************************************************
*
* FILE NAME:   .\Utils\ErrorPresenterBase.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains implementation of the class ErrorPresenterBase
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.Reflection;
using PK.PkUtils.UI.Dialogs.MsgBoxes;

namespace PK.PkUtils.Utils;

/// <summary>
/// A base class implementing IErrorPresenter interface.
/// You may change the behaviour by overwriting any of its virtual method.
/// </summary>
[CLSCompliant(true)]
public class ErrorPresenterBase : IErrorPresenter
{
    #region Typedefs

    /// <summary>
    /// An instance of this class keeps the data related to particular error presentation.
    /// It is created in virtual method ConvertArguments, and used afterwards 
    /// as an argument of virtual method DoShowError.
    /// </summary>
    /// <remarks>
    /// The descendants of ErrorPresenterBase may override the virtual method
    /// ConvertArguments (which can return a descendant of ErrorPresentInfo with additional fields),
    /// and at the same time override the virtual method ConvertArgument, 
    /// which will fill-in those additional fields.
    /// </remarks>
    protected class ErrorPresentInfo
    {
        #region Properties

        /// <summary> The final error message dialog caption. </summary>
        public string MessageCaption { get; set; }

        /// <summary> The final error message dialog buttons. </summary>
        public MessageBoxButtons MessageButtons { get; set; }

        /// <summary> The final error message dialog icon. </summary>
        public MessageBoxIcon MessageIcon { get; set; }

        /// <summary> The final error message dialog  default button. </summary>
        public MessageBoxDefaultButton MessageDefaultButton { get; set; }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Default argument-less constructor
        /// </summary>
        public ErrorPresentInfo()
          : this(MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        {
        }

        /// <summary>
        /// The constructor accepting message dialog buttons and message dialog icon.
        /// </summary>
        /// <param name="buttons">Message dialog buttons.</param>
        /// <param name="icon">Message dialog icon.</param>
        public ErrorPresentInfo(MessageBoxButtons buttons, MessageBoxIcon icon)
          : this(null, buttons, icon)
        {
        }

        /// <summary>
        /// The constructor accepting message dialog title (caption), message dialog buttons 
        /// and message dialog icon.
        /// </summary>
        /// <param name="strCaption">Message dialog caption (the text to display in the title bar of the message box).</param>
        /// <param name="buttons">Message dialog buttons.</param>
        /// <param name="icon">Message dialog icon.</param>
        public ErrorPresentInfo(string strCaption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            MessageCaption = strCaption;
            MessageButtons = buttons;
            MessageIcon = icon;
            MessageDefaultButton = MessageBoxDefaultButton.Button1;
        }
        #endregion // Constructors
    }
    #endregion // Typedefs

    #region Fields

    /// <summary>
    /// The underlying field for the property DefaultErrorIcon.
    /// </summary>
    private MessageBoxIcon _errIcon = MessageBoxIcon.Exclamation;

    /// <summary>
    /// The type of the object that will be created inside the method <see cref="ConvertArguments"/>.
    /// </summary>
    private readonly Type _errorPresentInfoType;

    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default argument-less constructor
    /// </summary>
    public ErrorPresenterBase()
      : this(typeof(ErrorPresentInfo))
    {
    }

    /// <summary> The constructor accepting type of the object that will be created inside the method
    /// <see cref="ConvertArguments"/>. It is assumed that type must be derived from ErrorPresentInfo,
    /// and it cannot be abstract. </summary>
    ///
    /// <exception cref="ArgumentException"> Thrown when the argument <paramref name="errorPresentInfoType"/>
    /// represents abstract type, or the type not derived from <see cref="ErrorPresentInfo"/> </exception>
    ///
    /// <param name="errorPresentInfoType"> The type of the object that will be created inside the
    /// method <see cref="ConvertArguments"/> </param>
    protected ErrorPresenterBase(Type errorPresentInfoType)
    {
        if (null != errorPresentInfoType)
        {
            string strErr;

            if (errorPresentInfoType.IsAbstract)
            {
                strErr = string.Format(CultureInfo.InvariantCulture,
                  "'The type '{0}' is an abstract type",
                  errorPresentInfoType.TypeToReadable());
                throw new ArgumentException(strErr, nameof(errorPresentInfoType));
            }
            if (!typeof(ErrorPresentInfo).IsAssignableFrom(errorPresentInfoType))
            {
                strErr = string.Format(CultureInfo.InvariantCulture,
                  "The type '{0}' is not derived from '{1}'",
                  errorPresentInfoType.TypeToReadable(),
                  typeof(ErrorPresentInfo).TypeToReadable());
                throw new ArgumentException(strErr, nameof(errorPresentInfoType));
            }

            _errorPresentInfoType = errorPresentInfoType;
        }
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// The default MessageBoxIcon that is used by <see cref="ConvertArguments "/>
    /// when creating a new instance of ErrorPresentInfo object.
    /// </summary>
    public MessageBoxIcon DefaultErrorIcon
    {
        get { return _errIcon; }
        set { _errIcon = value; }
    }

    /// <summary>
    /// Returns the value of _errorPresentInfoType field, 
    /// that has been initialized by constructor.
    /// </summary>
    private Type ErrorPresentInfoType
    {
        get
        {
            if (null != _errorPresentInfoType)
            {
                Debug.Assert(typeof(ErrorPresentInfo).IsAssignableFrom(_errorPresentInfoType));
            }
            return _errorPresentInfoType;
        }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Basic implementation of showing of the error, by calling the MessageBox.
    /// </summary>
    /// <param name="strErr">The error text.</param>
    /// <param name="info">
    /// The additional info regarding error presenting, 
    /// previously retrieved by call of <see cref="ConvertArguments"/>
    /// </param>
    /// <returns>The WinForms dialog result </returns>
    /// <seealso cref="ConvertArguments"/>
    protected virtual DialogResult DoShowError(
      string strErr,
      ErrorPresentInfo info)
    {
        ArgumentNullException.ThrowIfNull(strErr);
        ArgumentNullException.ThrowIfNull(info);

        return RtlAwareMessageBox.Show(
          null,
          strErr,
          info.MessageCaption, info.MessageButtons, info.MessageIcon, info.MessageDefaultButton);
    }

    /// <summary>
    /// Basic implementation of showing of the error, 
    /// by converting the exception to its text and call stack, 
    /// and invoking an overloaded method.
    /// </summary>
    /// <param name="ex">The exception that has occurred</param>
    /// <param name="info">The additional info regarding error presenting, 
    /// previously retrieved by call of ConvertArguments</param>
    /// <returns>The WinForms dialog result </returns>
    /// <see cref="ConvertArguments"/>
    protected virtual DialogResult DoShowError(
        Exception ex,
        ErrorPresentInfo info)
    {
        ArgumentNullException.ThrowIfNull(ex);
        ArgumentNullException.ThrowIfNull(info);

        return DoShowError(ex.ExceptionDetails(), info);
    }

    /// <summary>
    /// Converts the array of arguments of any type to specific fields of output ErrorPresentInfo object.
    /// The method calls internally the virtual method ConvertArgument for each of the input arguments.
    /// </summary>
    /// <param name="arguments">A collection of arguments provided originally to <see cref="ShowError(string, object[])"/>,
    /// who calls this ConvertArguments method.</param>
    /// <returns>The new ErrorPresentInfo object.</returns>
    protected virtual ErrorPresentInfo ConvertArguments(
      IEnumerable<object> arguments)
    {
        ErrorPresentInfo result;
        // old way before using the ErrorPresentInfoType property
        if (null == ErrorPresentInfoType)
        {
            result = new ErrorPresentInfo(MessageBoxButtons.OK, DefaultErrorIcon);
        }
        else
        {
            result = ActivatorEx.CreateInstance(ErrorPresentInfoType) as ErrorPresentInfo;
            result.MessageButtons = MessageBoxButtons.OK;
            result.MessageIcon = DefaultErrorIcon;
        }

        arguments.SafeForEach(obj => ConvertArgument(obj, result));
        return result;
    }

    /// <summary>
    /// <para>
    /// Auxiliary method called by ConvertArguments. 
    /// Converts the particular argument of any type to specific field of ErrorPresentInfo object.
    /// </para>
    /// <para>
    /// The arguments are interpreted based on their type: <br/>
    /// - the argument of type string is interpreted as dialog title<br/>
    /// - the argument of type MessageBoxButtons affect buttons in the dialog<br/>
    /// - the argument of type MessageBoxIcon affect icons in the dialog<br/>
    /// - the argument of type MessageBoxDefaultButton affect default button in the dialog<br/>
    /// - the other types of arguments are not converted and the method will return false.
    /// </para>
    /// </summary>
    /// <param name="obj">The converted input argument value. May be null.</param>
    /// <param name="info">
    /// The information presentation, to which the converted value is assigned.
    /// Must not be null.
    /// </param>
    /// <returns>True on success, false on failure.</returns>
    protected virtual bool ConvertArgument(object obj, ErrorPresentInfo info)
    {
        ArgumentNullException.ThrowIfNull(info);

        string strTmp;
        bool bRes = false;

        if (obj != null)
        {
            if (null != (strTmp = (obj as string)))
            {
                info.MessageCaption = strTmp;
            }
            else if (obj is MessageBoxButtons)
            {
                info.MessageButtons = (MessageBoxButtons)obj;
            }
            else if (obj is MessageBoxIcon)
            {
                info.MessageIcon = (MessageBoxIcon)obj;
            }
            else if (obj is MessageBoxDefaultButton)
            {
                info.MessageDefaultButton = (MessageBoxDefaultButton)obj;
            }
            else
            {
                // unknown argument; don't do anything
            }
        }

        return bRes;
    }
    #endregion // Methods

    #region IErrorPresenter Members

    /// <summary>
    /// Implements IErrorPresenter.ShowError
    /// </summary>
    /// <param name="strErr"> The error message. </param>
    /// <param name="arguments"> Additional optional arguments, in arbitrary order, that specify in
    /// more detail how the error message should be presented. </param>
    /// <returns>The WinForms dialog result</returns>
    public virtual DialogResult ShowError(string strErr, params object[] arguments)
    {
        ErrorPresentInfo info = ConvertArguments(arguments);

        return DoShowError(strErr, info);
    }

    /// <summary>
    /// Implements IErrorPresenter.ShowError
    /// </summary>
    /// <param name="ex">   The previously caught exception that represents an error that has occurred. </param>
    /// <param name="arguments"> Additional optional arguments, in arbitrary order, that specify in
    /// more detail how  the error message should be presented. </param>
    /// <returns> The WinForms dialog result. </returns>
    public virtual DialogResult ShowError(Exception ex, params object[] arguments)
    {
        ErrorPresentInfo info = ConvertArguments(arguments);

        return DoShowError(ex, info);
    }
    #endregion // IErrorPresenter Members
}

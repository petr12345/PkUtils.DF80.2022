// Ignore Spelling: Utils, Gsl
// 
using System;
using System.Diagnostics;
using PK.Commands.CommandUtils;
using PK.Commands.Interfaces;
using PK.PkUtils.Consoles;
using ILogger = log4net.ILog;

#pragma warning disable IDE0290     // Use primary constructor

namespace PK.Commands.CommandProcessing;

/// <summary>
/// More specialized <see cref="DualFormatInputProcessor{TCommand}"/>. 
/// This version uses a command register that implements <see cref="ICommandRegisterEx{TCommand, TErrorCode}"/> 
/// for more advanced handling of commands and error codes.
/// </summary>
/// 
/// <typeparam name="TCommand">     Type of the command. </typeparam>
/// <typeparam name="TErrorCode">   Type of the error code. </typeparam>
[CLSCompliant(true)]
public class DualFormatInputProcessorEx<TCommand, TErrorCode> : DualFormatInputProcessor<TCommand, TErrorCode>,
    ICommandsInputProcessorEx<TErrorCode>
    where TCommand : class, ICommandEx<TErrorCode>
{
    #region Constructor(s)

    /// <summary> The only public constructor. </summary>
    ///
    /// <param name="logger">   The logger interface. Can't be null. </param>
    /// <param name="display"> A callback interface to display progress. May be null. </param>
    /// <param name="commandRegister">  (Optional) The command register to be used. </param>
    /// <param name="commandNamePrecedes">  (Optional) Determines how command name should be parsed.
    /// If true, the command name is expected to be first separate string on command line, like "BackupESM -Env:LOCAL".
    /// If false, the command name is expected to be derived from the first argument, like "/Env:BUILD".
    /// </param>
    public DualFormatInputProcessorEx(
        ILogger logger,
        IConsoleDisplay display,
        ICommandRegisterEx<TCommand, TErrorCode> commandRegister = null,
        bool commandNamePrecedes = true)
        : base(logger, display,
              commandRegister ?? new CommandRegisterEx<TCommand, TErrorCode>(),
              commandNamePrecedes)
    { }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>  Gets the extended command register. </summary>
    protected ICommandRegisterEx<TCommand, TErrorCode> CommandRegisterEx
    {
        get
        {
            Debug.Assert(base.CommandRegister != null);
            Debug.Assert(base.CommandRegister is ICommandRegisterEx<TCommand, TErrorCode>);
            return base.CommandRegister as ICommandRegisterEx<TCommand, TErrorCode>;
        }
    }
    #endregion // Properties

    #region ICommandsInputProcessorEx Members

    /// <inheritdoc/>
    public TErrorCode GetLastError() => CommandRegisterEx.GetLastError();

    #endregion // ICommandsInputProcessorEx Members
}
#pragma warning restore IDE0290
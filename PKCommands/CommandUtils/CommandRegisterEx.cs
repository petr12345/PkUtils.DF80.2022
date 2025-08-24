// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PK.Commands.Interfaces;
using PK.PkUtils.Consoles;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using static System.FormattableString;
using ILogger = log4net.ILog;

#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.Commands.CommandUtils;

/// <summary>  Class implementing more specialized <see cref="CommandRegister{TCommand}"/>. 
///            Supports only such commands that implement ICommandEx{TErrorCode}.
///</summary>
///
/// <typeparam name="TCommand">     Type of the command. </typeparam>
/// <typeparam name="TErrorCode">   Type of the error code. </typeparam>
[CLSCompliant(true)]
public class CommandRegisterEx<TCommand, TErrorCode> :
    CommandRegister<TCommand, TErrorCode>,
    ICommandRegisterEx<TCommand, TErrorCode> where TCommand : class, ICommandEx<TErrorCode>
{
    #region Fields

    private TCommand _lastValidated;
    private TCommand _lastExecuted;
    private TErrorCode _lastError;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default argument-less constructor. </summary>
    public CommandRegisterEx()
    { }
    #endregion // Constructor(s)

    #region ICommandRegisterEx Members
    #region ICommandRegister Members

    /// <inheritdoc/>
    public override void RegisterCommands(ILogger logger, Assembly assembly = null)
    {
        ArgumentNullException.ThrowIfNull(logger);

        // Precaution to avoid calling registration twice. 
        // This is needed, since for instance some unit tests for magic reasons occasionally Do call it twice.
        // 
        if (!CommandsHaveBeenRegistered)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            IReadOnlyList<Type> candidates = assembly.GetTypes().Where(tp => !tp.IsAbstract).ToList();
            IReadOnlyList<Type> cmdTypes = candidates.Where(ImplementsICommandEx).ToList();

            DoRegisterCommands(logger, cmdTypes);
        }
    }

    /// <inheritdoc/>
    public override void RegisterCommands(ILogger logger, IEnumerable<Type> cmdTypes)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(cmdTypes);

        foreach (Type tp in cmdTypes)
        {
            CheckImplementsICommandEx(tp);
        }

        if (!CommandsHaveBeenRegistered)
        {
            DoRegisterCommands(logger, cmdTypes);
        }
    }

    /// <inheritdoc/>
    public override IComplexErrorResult<TErrorCode> Execute(
        string cmdName,
        IReadOnlyDictionary<string, string> parsedArgs,
        IConsoleDisplay display)
    {
        ResetLastError();
        return base.Execute(cmdName, parsedArgs, display);
    }
    #endregion // ICommandRegister Members

    /// <inheritdoc/>
    public TCommand LastValidatedCommand
    {
        get { return _lastValidated; }
    }

    /// <inheritdoc/>
    public TCommand LastExecutedCommand
    {
        get { return _lastExecuted; }
    }

    /// <inheritdoc/>
    public TErrorCode GetLastError()
    {
        return _lastError;
    }
    #endregion // ICommandRegisterEx Members

    #region Methods

    /// <summary>   Sets the last error to <paramref name="value"/>. </summary>
    /// <param name="value"> The value to be assigned. </param>
    protected void SetLastError(TErrorCode value)
    {
        _lastError = value;
    }

    /// <summary> Resets the last error. </summary>
    protected void ResetLastError()
    {
        SetLastError(default);
    }

    /// <inheritdoc/>
    protected override IComplexResult ValidateCommand(
        TCommand command,
        IReadOnlyDictionary<string, string> parsedArgs,
        IConsoleDisplay display)
    {
        _lastValidated = command ?? throw new ArgumentNullException(nameof(command));
        IComplexResult result = null;

        try
        {
            result = base.ValidateCommand(command, parsedArgs, display);
        }
        finally
        {
            // The result will be still null if and only if command syntax validation failed by exception
            // SetLastErrorFromCommand(command, result);
            // ##FIX## Set last error not needed, whole  override not needed
        }
        return result;
    }

    /// <inheritdoc/>
    protected override IComplexErrorResult<TErrorCode> ExecuteValidatedCommand(TCommand command)
    {
        _lastExecuted = command ?? throw new ArgumentNullException(nameof(command));

        TErrorCode errorCode;
        IComplexErrorResult<TErrorCode> result = null;

        try
        {
            result = base.ExecuteValidatedCommand(command);
            errorCode = result.Failed() ? command.GetLastError() : default;
            SetLastError(errorCode);
        }
        finally
        {
            SetLastErrorFromCommand(command, result);
            // ##FIX## Sert last error not needed
        }

        return result;
    }

    private static void CheckImplementsICommandEx(Type tp)
    {
        if (!ImplementsICommandEx(tp))
        {
            string errorMessage = Invariant($"The type {tp.TypeToReadable()} does not implement generic {typeof(ICommandEx<>).TypeToReadable()}");
            throw new ArgumentException(errorMessage, nameof(tp));
        }
    }

    /// <summary>
    /// Function detecting if given type implements generic interface ICommandEx{}. See also
    /// https://stackoverflow.com/questions/503263/how-to-determine-if-a-type-implements-a-specific-generic-interface-type.
    /// </summary>
    ///
    /// <param name="tp"> The type being checked. Can't be null. </param>
    /// <returns>   True if it <paramref name="tp"/> implements ICommandEx{}, false if not. </returns>
    private static bool ImplementsICommandEx(Type tp)
    {
        ArgumentNullException.ThrowIfNull(tp);
        bool result = (tp.GetInterfaces().Any(ti => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(ICommandEx<>)));

        return result;
    }

    private void SetLastErrorFromCommand(TCommand command, IComplexErrorResult<TErrorCode> result)
    {
        if ((result == null) || result.Failed())
        {
            // get error code from command
            TErrorCode errorCode = command.GetLastError();

            // if it's an error, and last error not set yet
            if ((errorCode is not null)
                && !errorCode.Equals(default(TErrorCode))
                && this.GetLastError().Equals(default(TErrorCode)))
            {
                SetLastError(errorCode);
            }
        }
    }
    #endregion // Methods
}
#pragma warning restore IDE0305
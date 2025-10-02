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
using ILogger = log4net.ILog;

#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.Commands.CommandUtils;

/// <summary>
/// Class implementing more specialized <see cref="CommandRegister{TCommand, TErrorCode}"/>.
/// Supports only such commands that implement ICommandEx{TErrorCode}.
///</summary>
///
/// <typeparam name="TCommand"> Type of the command. </typeparam>
/// <typeparam name="TErrorCode"> Type of the error code. </typeparam>
public class CommandRegisterEx<TCommand, TErrorCode> :
    CommandRegister<TCommand, TErrorCode>,
    ICommandRegisterEx<TCommand, TErrorCode> where TCommand : class, ICommandEx<TErrorCode>
{
    #region Fields

    private TCommand _lastValidated;
    private TCommand _lastExecuted;
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
    #endregion // ICommandRegister Members

    /// <inheritdoc/>
    public TCommand LastValidatedCommand { get => _lastValidated; }

    /// <inheritdoc/>
    public TCommand LastExecutedCommand { get => _lastExecuted; }
    #endregion // ICommandRegisterEx Members

    #region Methods

    /// <inheritdoc/>
    protected override IComplexResult ValidateCommand(
        TCommand command,
        IReadOnlyDictionary<string, string> parsedArgs,
        IConsoleDisplay display)
    {
        _lastValidated = command ?? throw new ArgumentNullException(nameof(command));
        IComplexResult result = base.ValidateCommand(command, parsedArgs, display);

        return result;
    }

    /// <inheritdoc/>
    protected override IComplexErrorResult<TErrorCode> ExecuteValidatedCommand(TCommand command)
    {
        _lastExecuted = command ?? throw new ArgumentNullException(nameof(command));
        return base.ExecuteValidatedCommand(command);
    }

    private static void CheckImplementsICommandEx(Type tp)
    {
        if (!ImplementsICommandEx(tp))
        {
            string errorMessage = $"The type {tp.TypeToReadable()} does not implement generic {typeof(ICommandEx<>).TypeToReadable()}";
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
    #endregion // Methods
}
#pragma warning restore IDE0305
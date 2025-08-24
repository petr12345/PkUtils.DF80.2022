using System;
using System.Collections.Generic;
using System.Reflection;
using PK.PkUtils.Consoles;
using PK.PkUtils.Interfaces;
using ILogger = log4net.ILog;

namespace PK.Commands.Interfaces;

/// <summary>  A utility class helping with registering commands etc. </summary>
/// 
/// <typeparam name="TCommand"> Generic argument representing type of supported commands. </typeparam>
/// <typeparam name="TErrorCode">Type of error details.</typeparam>
[CLSCompliant(true)]
public interface ICommandRegister<TCommand, TErrorCode> where TCommand : class, ICommand<TErrorCode>
{
    #region Properties

    /// <summary>   Gets a value indicating whether the commands have been registered. </summary>
    /// <seealso cref="CommandsRegistrationSucceeded"/>
    bool CommandsHaveBeenRegistered { get; }

    /// <summary>   Gets a value indicating whether the commands registration succeeded. </summary>
    /// <seealso cref="CommandsHaveBeenRegistered"/>
    bool CommandsRegistrationSucceeded { get; }

    /// <summary>   Gets the command names comparer. </summary>
    StringComparer CommandNamesComparer { get; }

    /// <summary>  Gets all the help command variants. </summary>
    IEnumerable<string> HelpCommandVariants { get; }

    /// <summary>  Gets all the quit command variants. </summary>
    IEnumerable<string> QuitCommandVariants { get; }

    /// <summary> Right now executed command. </summary>
    TCommand CurrentCommand { get; }

    #endregion // Properties

    #region Methods

    /// <summary>   Find and registers all the commands ( command types ). </summary>
    ///
    /// <remarks>   If called the second time, just does nothing. </remarks>
    ///
    /// <param name="logger">   The logger, used for commands initialization. Can't be null. </param>
    /// <param name="assembly"> (Optional) The assembly where command types should be searched for. <br/>
    /// If null, will be used Assembly.GetExecutingAssembly. </param>
    ///
    /// <seealso cref="CommandsHaveBeenRegistered"/>
    void RegisterCommands(ILogger logger, Assembly assembly = null);

    /// <summary>  Registers all the commands ( command types ) in given sequence. </summary>
    ///
    /// <remarks>   If called the second time, just does nothing. </remarks>
    ///
    /// <param name="logger">   The logger, used for commands initialization. Can't be null. </param>
    /// <param name="cmdTypes"> List of types of the commands to be registered. Can't be null. </param>
    void RegisterCommands(ILogger logger, IEnumerable<Type> cmdTypes);

    /// <summary>  Gets a command of given <paramref name="cmdName"/>. </summary>
    /// <remarks> Returns null if <paramref name="cmdName"/> refers to list command ("ls" or "list");
    ///           or if refers to help command ( "?" or "h" or "help"). </remarks>
    ///
    /// <param name="cmdName">  The name of command. Can't be null or empty. </param>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when any of required arguments is null. </exception>
    /// <exception cref="ArgumentException"> 
    /// Thrown when a supplied <paramref name="cmdName"/> is empty, or if such command can't be found. 
    /// </exception>
    ///
    /// <returns>  The found command. </returns>
    TCommand GetCommand(string cmdName);

    /// <summary>   Query if 'cmdName' represents a list command. </summary>
    ///
    /// <param name="cmdName">  The name of command. </param>
    ///
    /// <returns>   True if list command, false if not. </returns>
    bool IsListCommand(string cmdName);

    /// <summary>   Query if 'cmdName' is help command. </summary>
    ///
    /// <param name="cmdName">  The name of command. </param>
    ///
    /// <returns>   True if help command, false if not. </returns>
    bool IsHelpCommand(string cmdName);

    /// <summary>   Query if 'cmdName' is quit command. </summary>
    ///
    /// <param name="cmdName">  The name of command. </param>
    ///
    /// <returns>   True if quit command, false if not. </returns>
    bool IsQuitCommand(string cmdName);

    /// <summary>   Executes the command <paramref name="cmdName"/>. </summary>
    ///
    /// <remarks>
    /// The argument command name <paramref name="cmdName"/> should be in lowercase.
    /// Note the method does not return an error code, but throws exception in case command.Execute throws exception.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when any of required arguments is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when a supplied <paramref name="cmdName"/> is empty,
    ///                                      or if such command can't be found. </exception>
    ///
    /// <param name="cmdName">      The name of command. Can't be null or empty. </param>
    /// <param name="parsedArgs">   The parsed arguments of command. Can't be null. </param>
    /// <param name="display">     The callback interface with displaying capability. May be null. </param>
    ///
    /// <returns>
    /// An IComplexErrorResult, describing success or possible error.
    /// As an exception from usual returning IComplexErrorResult, returned value may be also null,
    /// indicating there was no command execution, but no error as well, only help for the command has been displayed.
    /// </returns>
    IComplexErrorResult<TErrorCode> Execute(
        string cmdName,
        IReadOnlyDictionary<string, string> parsedArgs,
        IConsoleDisplay display);
    #endregion // Methods
}
// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PK.Commands.Interfaces;

namespace PK.Commands.CommandUtils;

/// <summary>  A command registry. Internal utility class, 
///            encapsulated by <see cref="CommandRegister{TCommand, TErrorCode}"/>.
/// </summary>
///
/// <typeparam name="TCommand"> Generic argument representing type of supported commands. </typeparam>
/// <typeparam name="TErrorCode">Type of error details.</typeparam>
internal class CommandRegistry<TCommand, TErrorCode> where TCommand : ICommand<TErrorCode>
{
    #region Fields

    private readonly StringComparer _keysComparer = StringComparer.OrdinalIgnoreCase;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>  Default argument-less constructor. </summary>
    public CommandRegistry()
    {
        CommandsDictionary = new Dictionary<string, TCommand>(KeysComparer);
    }

    /// <summary> Constructor accepting several commands. </summary>
    ///
    /// <param name="commands"> A variable-length parameters list containing commands. </param>
    public CommandRegistry(params TCommand[] commands)
        : this()
    {
        ArgumentNullException.ThrowIfNull(commands);
        foreach (TCommand command in commands)
        {
            CommandsDictionary.Add(command.Name, command);
        }
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>  Gets the register -  a dictionary of commands. </summary>
    protected IDictionary<string, TCommand> CommandsDictionary { get; }

    /// <summary> Gets the string comparer used for comparison of keys (command names) in <see cref="CommandsDictionary"/>. </summary>
    protected internal StringComparer KeysComparer { get => _keysComparer; }

    #endregion // Properties

    #region Methods

    /// <summary>   Gets a command of given name. </summary>
    ///
    /// <exception cref="ArgumentException"> Thrown when given command name not found in command register. </exception>
    /// <exception cref="ArgumentNullException"> Thrown when given command name is null. </exception>
    /// 
    /// <param name="name"> The name. </param>
    /// <returns> The resulting found command. </returns>
    public TCommand GetCommand(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (!CommandsDictionary.TryGetValue(name, out TCommand result))
        {
            string strErr = string.Format(CultureInfo.InvariantCulture, "Command '{0}' not found. ", name);
            throw new ArgumentException(strErr, nameof(name));
        }

        return result;
    }

    /// <summary>  Gets a command of given name, or null if there is no such command. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when given command name is null. </exception>
    /// 
    /// <param name="name"> The name. </param>
    /// <returns> The resulting found command or null. </returns>
    public TCommand TryGetCommand(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        CommandsDictionary.TryGetValue(name, out TCommand result);
        return result;
    }

    /// <summary>   Gets the command names, sorted alphabetically. </summary>
    ///
    /// <returns> An enumerable collection. </returns>
    public IEnumerable<string> GetCommandNames()
    {
        return CommandsDictionary.Keys.OrderBy(key => key, KeysComparer);
    }

    /// <summary>   Registers the command described by <paramref name="command"/>. </summary>
    ///
    /// <param name="command">  The command to be registered. </param>
    /// <exception cref="ArgumentNullException"> Thrown when given <paramref name="command"/> is null. </exception>
    public void RegisterCommand(TCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        CommandsDictionary.Add(command.Name, command);
    }
    #endregion // Methods
}
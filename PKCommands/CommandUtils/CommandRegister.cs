// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using PK.Commands.Interfaces;
using PK.PkUtils.Consoles;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.Reflection;
using static System.FormattableString;
using ILogger = log4net.ILog;

#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.Commands.CommandUtils;

/// <summary>  A utility class helping with registering commands etc. </summary>
/// <remarks> It inherits from <see cref="NotifyPropertyChanged"/> to support INotifyPropertyChanged.
///           However the only property involved now is <see cref="CurrentCommand"/>,
///           changed in <see cref="ExecuteValidatedCommand(TCommand)"/>.
/// </remarks>
/// 
/// <typeparam name="TCommand"> Generic argument representing type of supported commands. </typeparam>
public class CommandRegister<TCommand> : NotifyPropertyChanged, ICommandRegister<TCommand>
    where TCommand : class, ICommand
{
    #region Fields

    private bool _commandsHaveBeenRegistered;
    private bool _commandsRegistrationSucceeded;
    private TCommand _currentCommand;

    private readonly CommandRegistry<TCommand> _commandRegistry = new();
    private static readonly string[] _helpCommandVariants = ["?", "h", "help"];
    private static readonly string[] _quitCommandVariants = ["q", "quit", "exit"];
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default argument-less constructor. </summary>
    public CommandRegister()
    { }
    #endregion // Constructor(s)

    #region ICommandRegister Members

    /// <inheritdoc/>
    public bool CommandsHaveBeenRegistered
    {
        get
        {
            Debug.Assert(!(_commandsRegistrationSucceeded && !_commandsHaveBeenRegistered));
            return _commandsHaveBeenRegistered;
        }
    }

    /// <inheritdoc/>
    public bool CommandsRegistrationSucceeded
    {
        get
        {
            Debug.Assert(!(_commandsRegistrationSucceeded && !_commandsHaveBeenRegistered));
            return _commandsRegistrationSucceeded;
        }
    }

    /// <inheritdoc/>
    public StringComparer CommandNamesComparer { get => CommandRegistry.KeysComparer; }

    /// <inheritdoc/>
    public IEnumerable<string> HelpCommandVariants { get => _helpCommandVariants; }

    /// <inheritdoc/>
    public IEnumerable<string> QuitCommandVariants { get => _quitCommandVariants; }

    /// <inheritdoc/>
    public TCommand CurrentCommand
    {
        get => _currentCommand;
        set { SetField(ref _currentCommand, value, nameof(CurrentCommand)); }
    }

    /// <inheritdoc/>
    public virtual void RegisterCommands(ILogger logger, Assembly commandsAssembly = null)
    {
        ArgumentNullException.ThrowIfNull(logger);

        // Precaution to avoid calling registration twice. 
        // This is needed, since for instance some unit tests for magic reasons occasionally Do call it twice.
        // 
        if (!CommandsHaveBeenRegistered)
        {
            commandsAssembly ??= Assembly.GetExecutingAssembly();

            IReadOnlyList<Type> candidates = commandsAssembly.GetTypes().Where(tp => !tp.IsAbstract).ToList();
            IReadOnlyList<Type> cmdTypes = candidates.Where(tp => ImplementsICommand(tp)).ToList();

            DoRegisterCommands(logger, cmdTypes);
        }
    }

    /// <inheritdoc/>
    public virtual void RegisterCommands(ILogger logger, IEnumerable<Type> cmdTypes)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(cmdTypes);

        foreach (Type tp in cmdTypes)
        {
            CheckImplementsICommand(tp);
        }

        if (!CommandsHaveBeenRegistered)
        {
            DoRegisterCommands(logger, cmdTypes);
        }
    }

    /// <summary>  Gets a command of given <paramref name="cmdName"/>. </summary>
    /// <remarks> Returns null if <paramref name="cmdName"/> refers to list command ("ls" or "list");
    ///           or if refers to help command ( "?" or "h" or "help"). </remarks>
    ///
    /// <param name="cmdName">  The name of command. Can't be null or empty. </param>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when any of required arguments is null. </exception>
    /// <exception cref="ArgumentException">  Thrown when a supplied <paramref name="cmdName"/> is empty,
    /// or if such command can't be found.  </exception>
    ///
    /// <returns>  The found command. </returns>
    public TCommand GetCommand(string cmdName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(cmdName);

        TCommand result;

        if (IsListCommand(cmdName) && IsHelpCommand(cmdName))
            result = null;
        else
            result = CommandRegistry.GetCommand(cmdName);

        return result;
    }

    /// <inheritdoc/>
    public virtual bool IsListCommand(string cmdName)
    {
        ChekIsLowerCase(cmdName);
        return (cmdName != null) && (cmdName.Equals("ls") || cmdName.Equals("list"));
    }

    /// <inheritdoc/>
    public virtual bool IsHelpCommand(string cmdName)
    {
        ChekIsLowerCase(cmdName);
        return (cmdName != null) && (HelpCommandVariants.Contains(cmdName, CommandNamesComparer));
    }

    /// <inheritdoc/>
    public virtual bool IsQuitCommand(string cmdName)
    {
        ChekIsLowerCase(cmdName);
        return (cmdName != null) && (QuitCommandVariants.Contains(cmdName, CommandNamesComparer));
    }

    /// <inheritdoc/>
    public virtual IComplexResult Execute(
        string cmdName,
        IReadOnlyDictionary<string, string> parsedArgs,
        IConsoleDisplay display)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(cmdName);
        ArgumentNullException.ThrowIfNull(parsedArgs);

        string cmdLower = cmdName.ToLowerInvariant();
        IComplexResult result = null;

        if (IsListCommand(cmdLower))
        {
            if (display != null) { DisplayAllCommands(display); }
        }
        else if (IsHelpCommand(cmdLower))
        {
            if (display != null) { DisplayHelp(display); }
        }
        else
        {
            TCommand command = CommandRegistry.GetCommand(cmdName); // throws exception if command not found
            IComplexResult v = ValidateCommand(command, parsedArgs, display);

            if (v == null)
            {
                // no command processing, just help;
            }
            else if (!v.Success)
            {
                result = ComplexResult.CreateFailed(Invariant($"Invalid arguments of command '{command.Name}'. {v.ErrorMessage}"));
            }
            else
            {
                result = ExecuteValidatedCommand(command);
            }
            if (result.NullSafe(x => x.Success))
            {
                command.ShowSuccessfulExecution(display);
            }
        }

        return result;
    }
    #endregion // ICommandRegister Members

    #region Properties

    private CommandRegistry<TCommand> CommandRegistry => _commandRegistry;
    #endregion // Properties

    #region Methods

    #region Protected Methods

    /// <summary>
    /// A utility method called by <see cref="Execute"/>, to validate the command before execution.
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    ///
    /// <param name="command">      The command to be executed. Can't be null. </param>
    /// <param name="parsedArgs">   Parsed arguments of command. Can't be null. </param>
    /// <param name="display">     The callback interface with displaying capability. May be null. </param>
    ///
    /// <returns>   Result of command validation. </returns>
    protected virtual IComplexResult ValidateCommand(
        TCommand command,
        IReadOnlyDictionary<string, string> parsedArgs,
        IConsoleDisplay display)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(parsedArgs);

        IComplexResult v = command.Validate(parsedArgs, display);

        return v;
    }

    /// <summary>
    /// A utility method called by <see cref="Execute"/>, to actually execute found command.
    /// </summary>
    ///
    /// <param name="command"> The command to be executed. Can't be null. </param>
    ///
    /// <returns> An IComplexResult </returns>
    protected virtual IComplexResult ExecuteValidatedCommand(TCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        IComplexResult result = null;

        try
        {
            CurrentCommand = command;
            result = command.Execute();
        }
        finally
        {
            CurrentCommand = default;
        }

        return result;
    }

    /// <summary>   An auxiliary method called from overloads of RegisterCommands. </summary>
    ///
    /// <exception cref="MissingMemberException">   
    /// Thrown when method CreateComamndObject called from here throws it.
    /// </exception>
    ///
    /// <param name="logger">   The logger. Can't be null. </param>
    /// <param name="cmdTypes"> Sequence of types of the commands. Can't be null. </param>
    protected void DoRegisterCommands(ILogger logger, IEnumerable<Type> cmdTypes)
    {
        Debug.Assert(cmdTypes != null);
        Debug.Assert(!CommandsHaveBeenRegistered);

        if ((cmdTypes != null) && !CommandsHaveBeenRegistered)
        {
            try
            {
                foreach (Type tp in cmdTypes)
                {
                    TCommand cmd = CreateComamndObject(tp, logger, out MissingMemberException caugtEx);

                    if (cmd != null)
                    {
                        Debug.Assert(caugtEx == null);
                        CommandRegistry.RegisterCommand(cmd);
                    }
                    else
                    {
                        Debug.Assert(caugtEx != null);
                        logger.Error(Invariant($"Failed to create instance of {tp.TypeToReadable()}"), caugtEx);
                        throw caugtEx;
                    }
                }

                _commandsRegistrationSucceeded = true;
            }
            finally
            {
                _commandsHaveBeenRegistered = true;
            }
        }
    }
    #endregion // Protected Methods

    #region Private Methods

    [Conditional("DEBUG")]
    private static void ChekIsLowerCase(string cmdName)
    {
        if (cmdName != null)
        {
            Debug.Assert(0 == StringComparer.Ordinal.Compare(cmdName, cmdName.ToLowerInvariant()),
                "Argument 'cmdName' must be lowercase");
        }
    }

    private static void CheckImplementsICommand(Type tp)
    {
        if (!ImplementsICommand(tp))
        {
            string errorMessage = Invariant($"The type {tp.TypeToReadable()} does not implement {typeof(ICommand).TypeToReadable()}");
            throw new ArgumentException(errorMessage, nameof(tp));
        }
    }

    /// <summary> Function detecting if given type implements interface ICommand. </summary>
    /// <param name="tp"> The type being checked. Can't be null. </param>
    /// <returns>   True if it <paramref name="tp"/> implements ICommand, false if not. </returns>
    private static bool ImplementsICommand(Type tp)
    {
        ArgumentNullException.ThrowIfNull(tp);

        bool result = (tp.GetInterfaces().Contains(typeof(ICommand)));
        return result;
    }

    private static TCommand CreateComamndObject(Type tp, ILogger logger, out MissingMemberException caugtEx)
    {
        object command = null;
        caugtEx = null;

        // Try to create two times; first try with logger argument, later without
        for (int ii = 0; (ii < 2) && (command == null); ii++)
        {
            try
            {
                command = (ii == 0) ?
                      ActivatorEx.CreateInstance(tp, [typeof(ILogger)], [logger])
                    : Activator.CreateInstance(tp);
            }
            catch (MissingMemberException ex)
            {
                caugtEx = ex;
            }
        }

        return command as TCommand;
    }

    private void DisplayAllCommands(IConsoleDisplay display, bool includingHelp = true, bool includingQuit = true)
    {
        Debug.Assert(display != null);

        List<string> helpVariants = HelpCommandVariants.ToList();
        List<string> quitVariants = QuitCommandVariants.ToList();
        List<string> names = CommandRegistry.GetCommandNames().ToList();

        if (includingHelp && !names.Any(s => helpVariants.Contains(s, CommandNamesComparer)))
        {
            names.Add(helpVariants.OrderBy(s => s.Length).Join());
        }
        if (includingQuit && !names.Any(s => quitVariants.Contains(s, CommandNamesComparer)))
        {
            names.Add(quitVariants.OrderBy(s => s.Length).Join());
        }

        display.WriteLine();
        display.WriteText("Available Commands:");

        foreach (var commandName in names)
        {
            display.WriteText(commandName);
        }

        display.WriteLine();
    }

    private void DisplayHelp(IConsoleDisplay display)
    {
        Debug.Assert(display != null);

        DisplayAllCommands(display);
    }
    #endregion // Private Methods
    #endregion // Methods
}
#pragma warning restore IDE0305
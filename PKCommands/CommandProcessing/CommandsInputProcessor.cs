using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

using PK.Commands.CommandExceptions;
using PK.Commands.CommandUtils;
using PK.Commands.Interfaces;
using PK.PkUtils.Cmd;
using PK.PkUtils.Consoles;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using static System.FormattableString;
using ILogger = log4net.ILog;

#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.Commands.CommandProcessing;

/// <summary>
/// A concrete implementation of <see cref="ICommandsInputProcessor"/> that processes command-line input and executes corresponding commands.
/// This class parses command-line arguments and maps them to registered commands, providing an interface for handling both command input and progress display.
///
/// The generic type <typeparamref name="TCommand"/> represents the type of commands supported by this processor, which must implement <see cref="ICommand"/>.
/// 
/// <example>
/// Example usage:
/// 
/// Consider a command-line input "BackupESM -Env:LOCAL -BackupPath:C:\Backup". This will be parsed as:
/// - Command name: "BackupESM"
/// - Command arguments:
///   - "Env": "LOCAL"
///   - "BackupPath": "C:\Backup"
/// 
/// The processor will then execute the "BackupESM" command using the provided arguments.
/// 
/// If command-name precedence is set to false, the command-line input could be parsed as:
/// "/Env:LOCAL /BackupPath:C:\Backup" where the command is implicitly derived from the first argument.
/// </example>
/// </summary>
/// <typeparam name="TCommand">Type of supported commands.</typeparam>
/// <typeparam name="TErrorCode">Type of error details.</typeparam>
public class CommandsInputProcessor<TCommand, TErrorCode> : ICommandsInputProcessor<TErrorCode>
    where TCommand : class, ICommand<TErrorCode>
{
    #region Fields

    private bool _finished;
    private bool _initialized;

    private readonly ICommandRegister<TCommand, TErrorCode> _commandRegister;
    private readonly IConsoleDisplay _displayProgress;
    private readonly ILogger _logger;
    private readonly bool _commandNamePrecedes;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>   Constructor. </summary>
    ///
    /// <param name="logger">           The logger interface. Can't be null. </param>
    /// <param name="display">         A callback interface to display progress. May be null. </param>
    /// <param name="commandRegister"> (Optional) The command register to be used. May be null;
    /// in that case a "plain' CommandRegister is used. </param>
    /// <param name="commandNamePrecedes">  (Optional) Determines how command name should be parsed.
    /// If true, the command name is expected to be first separate string on command line, like "BackupESM -Env:LOCAL".
    /// If false, the command name is expected to be derived from the first argument, like "/Env:BUILD".
    /// </param>
    public CommandsInputProcessor(
        ILogger logger,
        IConsoleDisplay display,
        ICommandRegister<TCommand, TErrorCode> commandRegister = null,
        bool commandNamePrecedes = true)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _displayProgress = display;
        _logger = logger;
        _commandRegister = commandRegister ?? new CommandRegister<TCommand, TErrorCode>();
        _commandNamePrecedes = commandNamePrecedes;
    }
    #endregion // Constructor(s)

    #region ICommandsInputProcessor Members
    #region IObserver<string> Members

    /// <summary>   Executes the completed action. </summary>
    /// 
    /// <exception cref="InvalidOperationException"> Thrown when OnCompleted() has been called already. </exception>
    /// 
    /// <remarks> Can be called even if this Observer object has NOT been initialized.
    ///           Unlike most of other methods, in such case this method does not throw <see cref="InvalidOperationException"/>.
    ///           However, it throws <see cref="InvalidOperationException"/> if the consumer is in finished state already.
    /// </remarks>
    public void OnCompleted()
    {
        if (CheckConsumerHasNotFinishedYet())
        {
            _finished = true;
        }
    }

    /// <inheritdoc/>
    public void OnError(Exception error)
    {
        LogAndDisplayError(error.Message);
    }

    /// <summary> Provides the observer with new data to be processed. </summary>
    ///
    /// <param name="value">  The data for current notification information. </param>
    /// 
    /// <exception cref="InvalidOperationException"> Thrown when any of <see cref="InitializeCommandContainer(ILogger, Assembly)"/>
    /// or <see cref="InitializeCommandContainer(ILogger, IEnumerable{Type})"/> has not been performed on this object yet. </exception>
    /// 
    /// <remarks> Can't be called until this Observer has been initialized by any of InitializeCommandContainer overloads>. </remarks>
    /// <seealso cref="ProcessNextInput(string, out bool)"/>
    public void OnNext(string value)
    {
        CheckHasBeenInitialized();

        if (CheckConsumerHasNotFinishedYet())
        {
            ProcessNextInput(value, out _);
        }
    }
    #endregion // IObserver<string> Members

    /// <summary> Returns value indicating whether this object has been initialized successfully ( including the _log ). </summary>
    public bool HasBeenInitialized { get => _initialized; }

    /// <summary> Gets a value indicating whether this observer has finished processing commands. 
    ///           Becomes true after <see cref="OnCompleted"/> has been called.
    /// </summary>
    public bool HasFinished
    {
        get { return _finished; }
    }

    /// <inheritdoc/>
    public bool InitializeCommandContainer(ILogger logger, Assembly assembly = null)
    {
        if (!HasBeenInitialized)
        {
            DoInitializeCommandContainerEtc(logger, assembly);
        }

        return HasBeenInitialized;
    }

    /// <inheritdoc/>
    public bool InitializeCommandContainer(ILogger logger, IEnumerable<Type> cmdTypes)
    {
        if (!HasBeenInitialized)
        {
            DoInitializeCommandContainerEtc(logger, cmdTypes);
        }

        return HasBeenInitialized;
    }

    /// <summary>   Process the next input text. </summary>
    /// <remarks>
    /// Can't be called until this Observer has been initialized by any of InitializeCommandContainer overloads>.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="input"/> is null. </exception>
    /// <exception cref="InvalidOperationException"> Thrown when any of <see cref="InitializeCommandContainer(ILogger, Assembly)"/>
    /// or <see cref="InitializeCommandContainer(ILogger, IEnumerable{Type})"/> has not been performed on this
    /// object yet. </exception>
    /// <param name="input"> The input, either from the command line or anywhere else. </param>
    /// <param name="shouldContinue"> [out] True if should continue, false otherwise. </param>
    /// <returns>   An IComplexResult. </returns>
    /// <seealso cref="OnNext"/>
    public virtual IComplexErrorResult<TErrorCode> ProcessNextInput(
        string input,
        out bool shouldContinue)
    {
        ArgumentNullException.ThrowIfNull(input);

        CheckHasBeenInitialized();
        CheckConsumerHasNotFinishedYet();

        IComplexErrorResult<TErrorCode> result;

        if (string.IsNullOrEmpty(input))
        {
            result = ComplexErrorResult<TErrorCode>.OK;
            shouldContinue = false;
        }
        else
        {
            IReadOnlyCollection<string> args = SplitCommandLine(input);
            result = RunCommand(args, out shouldContinue);
        }

        return result;
    }

    /// <summary>   Process the next input of already separated command-line arguments. </summary>
    ///
    /// <param name="args"> The command arguments, including command name as first. Can't be null.</param>
    /// <param name="shouldContinue">   [out] True if should continue, false otherwise. </param>
    ///
    /// <returns>   An IComplexResult. </returns>
    public virtual IComplexErrorResult<TErrorCode> ProcessNextInput(
        IEnumerable<string> args,
        out bool shouldContinue)
    {
        ArgumentNullException.ThrowIfNull(args);

        CheckHasBeenInitialized();
        CheckConsumerHasNotFinishedYet();

        IComplexErrorResult<TErrorCode> result = ComplexErrorResult<TErrorCode>.OK;

        if (args.IsEmpty())
            shouldContinue = false;
        else
            result = RunCommand(args, out shouldContinue);

        return result;
    }
    #endregion // ICommandsInputProcessor Members

    #region Properties

    /// <summary>   Gets the command register. </summary>
    protected ICommandRegister<TCommand, TErrorCode> CommandRegister
    {
        get { Debug.Assert(_commandRegister != null); return _commandRegister; }
    }

    /// <summary>   Gets the log interface. </summary>
    /// 
    /// <exception cref="InvalidOperationException"> Thrown when any of <see cref="InitializeCommandContainer(ILogger, Assembly)"/>
    /// or <see cref="InitializeCommandContainer(ILogger, IEnumerable{Type})"/> has not been performed on this object yet. </exception>
    /// 
    /// <remarks> Can't be called until this Observer has been initialized by any InitializeCommandContainer overloads.
    /// </remarks>
    protected ILogger Logger
    {
        get { CheckHasBeenInitialized(); return _logger; }
    }

    /// <summary>   Gets the display progress callback interface. </summary>
    protected IConsoleDisplay DisplayProgress { get => _displayProgress; }

    /// <summary> Gets a value indicating whether the command name is extra first string. </summary>
    protected bool CommandNamePrecedes { get => _commandNamePrecedes; }

    #endregion // Properties

    #region Methods

    /// <summary> Auxiliary implementation shortcut; checks whether this consumer is not in finished state yet. </summary>
    ///
    /// <returns> In case it is in finished state, calls OnError and returns false. 
    ///           Otherwise, returns true. </returns>
    protected bool CheckConsumerHasNotFinishedYet()
    {
        bool bRes = true;

        if (HasFinished)
        {
            OnError(new InvalidOperationException("This consumer finished its lifecycle"));
            bRes = false;
        }

        return bRes;
    }

    /// <summary> Checks this instance has been initialized; throws <see cref="InvalidOperationException"/> if not. </summary>
    ///
    /// <exception cref="InvalidOperationException"> Thrown when any of <see cref="InitializeCommandContainer(ILogger, Assembly)"/>
    /// or <see cref="InitializeCommandContainer(ILogger, IEnumerable{Type})"/> has not been performed on this object yet. </exception>
    protected void CheckHasBeenInitialized()
    {
        if (!HasBeenInitialized)
        {
            string strErr = string.Format(CultureInfo.InvariantCulture,
                "This '{0}' instance has not been initialized yet. Call '{1}' to initialize.",
                this.GetType(), nameof(InitializeCommandContainer));
            throw new InvalidOperationException(strErr);
        }
    }

    /// <summary> Logs and displays an error described by errorMessage. </summary>
    ///
    /// <param name="errorMessage"> Message describing the error. Can't be null or empty. </param>
    protected virtual void LogAndDisplayError(string errorMessage)
    {
        Debug.Assert(!string.IsNullOrEmpty(errorMessage));

        Logger.Error(errorMessage);
        DisplayProgress?.WriteError(errorMessage);
    }

    /// <summary>
    /// Auxiliary method called by <see cref="RunCommand"/>; parses input arguments <paramref name="cmdLineArgs"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> Thrown when the input <paramref name="cmdLineArgs"/> is empty. </exception>
    /// <exception cref="ArgumentNullException"> Thrown when the input <paramref name="cmdLineArgs"/> is null. </exception>
    /// <param name="cmdLineArgs"> The command line arguments. </param>
    /// <param name="cmdName"> [out] Name of the command, taken from the first argument, in lowercase. </param>
    /// <returns> The rest of arguments, parsed into name-value dictionary. </returns>
    protected internal virtual IReadOnlyDictionary<string, string> ParseInputArgs(
        IEnumerable<string> cmdLineArgs,
        out string cmdName)
    {
        ArgumentNullException.ThrowIfNull(cmdLineArgs);
        if (cmdLineArgs.IsEmpty()) { throw new ArgumentException("Arguments can't be empty", nameof(cmdLineArgs)); }

        string commandName;
        CommandArgsParser parser;

        if (CommandNamePrecedes)
        {
            commandName = cmdLineArgs.First();
            parser = new CommandArgsParser(cmdLineArgs.Skip(1), CommandRegister.CommandNamesComparer);
        }
        else
        {
            parser = new CommandArgsParser(cmdLineArgs, CommandRegister.CommandNamesComparer);
            commandName = parser.Parameters.IsNullOrEmpty() ? string.Empty : parser.OriginalParametersOrder.First();
        }

        cmdName = commandName.ToLowerInvariant();
        return parser.Parameters;
    }

    /// <summary>   Executes the command operation. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when the input <paramref name="args"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when the input <paramref name="args"/> is empty. </exception>
    /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
    ///
    /// <param name="args"> The command arguments, including command name as first. Can't be null or empty.
    ///                     Example: Copy -o CreditRiskData -p1 yyyymmdd  -source \\njcfnap37b\b2prod\app1653_cca_logs_b2prod\margin]
    /// </param>
    /// <param name="shouldContinue">   [out] True if the caller should continue processing. </param>
    ///
    /// <returns>   An IComplexResult. </returns>
    protected virtual IComplexErrorResult<TErrorCode> RunCommand(
        IEnumerable<string> args,
        out bool shouldContinue)
    {
        ArgumentNullException.ThrowIfNull(args);
        if (args.IsEmpty()) { throw new ArgumentException("Arguments can't be empty", nameof(args)); }

        string strDisplayedError = null;
        string strLoggedError = null;
        Exception exToDisplay = null;
        string commandName = args.First();
        string cmdLower = commandName.ToLowerInvariant();
        IComplexErrorResult<TErrorCode> result = ComplexErrorResult<TErrorCode>.OK;

        shouldContinue = true;
        try
        {
            if (CommandRegister.IsQuitCommand(cmdLower))
            {
                shouldContinue = false;
            }
            else
            {
                IReadOnlyDictionary<string, string> parsedArgs = ParseInputArgs(args, out commandName);
                IComplexErrorResult<TErrorCode> cmdResult = CommandRegister.Execute(commandName, parsedArgs, DisplayProgress);

                if (cmdResult == null)
                {
                    // Just help was displayed, don't call DisplayProgress.WriteSuccess; leave default result
                    Debug.Assert(result != null);
                }
                else
                {
                    result = cmdResult;
                    if (result.Success)
                    {
                        // This is now done from inside CommandRegister.Execute, invoking virtual method of command
                        // DisplayProgress?.WriteSuccess(Invariant($"{commandName} command succeeded"));
                    }
                    else if (result.ExceptionCaught() != null)
                    {
                        exToDisplay = result.ExceptionCaught();
                    }
                    else
                    {
                        GenerateCommandProcessingMessages(result.ErrorMessage, commandName, out strLoggedError, out strDisplayedError);
                    }
                }
            }
        }
        catch (InputLineValidationException ex)
        {
            strDisplayedError = ex.Message;
            strLoggedError = ex.ExceptionDetails();
        }
        catch (CommandValidationException ex)
        {
            result = ComplexErrorResult<TErrorCode>.CreateFailed(ex);
            if (ex.AlreadyDisplayed)
            {
                // Do NOT assign exToDisplay here if AlreadyDisplayed, to avoid messages duplicating.
                strLoggedError = ex.Message;
            }
            else
            {
                exToDisplay = ex;
            }
        }
        catch (Exception ex)
        {
            result = ComplexErrorResult<TErrorCode>.CreateFailed(exToDisplay = ex);
        }


        if (exToDisplay is not null)
        {
            GenerateCommandProcessingMessages(exToDisplay, commandName, out strLoggedError, out strDisplayedError);
        }

        if (!string.IsNullOrEmpty(strLoggedError))
        {
            Logger.Error(strLoggedError, exToDisplay);
        }
        if (!string.IsNullOrEmpty(strDisplayedError))
        {
            DisplayProgress?.WriteError(strDisplayedError);
        }

        return result;
    }

    /// <summary>
    /// Executes the initialization of command container etc.; that includes also CommandHandler.RegisterCommands.
    /// Does nothing and returns true if already called before successfully.
    /// </summary>
    ///
    /// <param name="logger"> The logger, used for commands initialization. Can't be null. </param>
    /// <param name="commandsAssembly"> (Optional) The assembly where command types should be searched for. <br/>
    /// If null, will be used Assembly.GetExecutingAssembly. </param>
    ///
    /// <returns>   True if it succeeds, false if it fails. </returns>
    protected virtual bool DoInitializeCommandContainerEtc(ILogger logger, Assembly commandsAssembly = null)
    {
        bool result = HasBeenInitialized;

        if (!result)
        {
            CommandRegister.RegisterCommands(logger, commandsAssembly);

            _initialized = true;
            result = HasBeenInitialized;
        }

        return result;
    }

    /// <summary>
    /// Executes the initialization of command container etc.; that includes also CommandHandler.RegisterCommands.
    /// Does nothing and returns true if already called before successfully.
    /// </summary>
    ///
    /// <param name="logger">   The logger, used for commands initialization. Can't be null. </param>
    /// <param name="cmdTypes"> List of types of the commands to be registered. Can't be null. </param>
    ///
    /// <returns>  True if it succeeds, false if it fails. </returns>
    protected virtual bool DoInitializeCommandContainerEtc(ILogger logger, IEnumerable<Type> cmdTypes)
    {
        bool result = HasBeenInitialized;

        if (!result)
        {
            ArgumentNullException.ThrowIfNull(logger);

            try
            {
                CommandRegister.RegisterCommands(logger, cmdTypes);

                _initialized = true;
                result = HasBeenInitialized;
            }
            catch (SystemException ex)
            {
                logger.Error(nameof(DoInitializeCommandContainerEtc), ex);
                throw;
            }
        }

        return result;
    }

    /// <summary> Split single command-line string into collection of arguments. </summary>
    ///
    /// <param name="consoleInput"> The console input. Can't be null.
    /// An example of such input: "Copy -o CreditRiskData -p1 yyyymmdd  -source \\njcfnap37b\b2prod\app1653_cca_logs_b2prod\margin]"
    /// </param>
    ///
    /// <returns> Collection or individual arguments. </returns>
    protected internal static IReadOnlyCollection<string> SplitCommandLine(string consoleInput)
    {
        ArgumentNullException.ThrowIfNull(consoleInput);

        List<string> args;
        string[] arrSplited = consoleInput.Split('"');

        // Reminder: 
        // The Linq method
        //    IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        // enumerates the input sequence, uses a transform function to map each element to an IEnumerable<TResult>,
        // and then enumerates and yields the elements of each such IEnumerable<TResult> object.
        // That is, for each element of source, selector is invoked and a sequence of values is returned.
        // SelectMany then flattens this two - dimensional collection of collections into a one-dimensional IEnumerable<TResult>
        // and returns it. 

        args = arrSplited.Select((element, index) => (index % 2 == 0)  // If even index, split the item
                                                  ? element.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                                  : element.FromSingle())  // otherwise keep the entire item
                          .SelectMany(element => element).ToList();

        return args;
    }

    private static void GenerateCommandProcessingMessages(
        Exception ex,
        string commandName,
        out string loggedError,
        out string displayError)
    {
        // The string loggedError should not contain all details of ex.Message, since Log.Error has the exception as argument
        loggedError = Invariant($"Error while processing command '{commandName}'. ");

        // The string displayedError should rather contain just the first line of ex.Message
        displayError = loggedError + ex.Message.FirstLine();
    }

    private static void GenerateCommandProcessingMessages(
        string errorDetails,
        string commandName,
        out string loggedError,
        out string displayError)
    {
        // if exception details are not available, both strings should contain error details
        loggedError = Invariant($"Error while processing command '{commandName}'. ");
        loggedError += errorDetails;
        displayError = loggedError;
    }
    #endregion // Methods
}
#pragma warning restore IDE0305

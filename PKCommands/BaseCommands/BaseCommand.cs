using System;
using System.Collections.Generic;
using PK.Commands.CommandExceptions;
using PK.Commands.CommandUtils;
using PK.Commands.Interfaces;
using PK.PkUtils.Consoles;
using PK.PkUtils.Interfaces;
using static System.FormattableString;

#pragma warning disable IDE0090 // Use 'new(...)'

namespace PK.Commands.BaseCommands;

/// <summary>   A base command class, implementing <see cref="ICommand"/>. </summary>
///
/// <typeparam name="TOptions"> Type of the options used. </typeparam>
/// <typeparam name="TErrorCode">Type of error details.</typeparam>
[CLSCompliant(true)]
public abstract class BaseCommand<TOptions, TErrorCode> : ICommand<TErrorCode>
    where TOptions : ICommandOptions, new()
{
    #region Constructor(s)

    /// <summary> Specialized default constructor for use only by derived class. </summary>
    protected BaseCommand()
    { }
    #endregion // Constructor(s)

    #region ICommand Members

    /// <summary>  Get the name of command. </summary>
    public abstract string Name { get; }

    /// <summary>  Get the string describing usage of command. </summary>
    public abstract string Usage { get; }

    /// <inheritdoc/>
    public virtual IReadOnlyCollection<IEnumerable<string>> MandatoryOptions => [];

    /// <summary> Validates given arguments and options. If succeeds, assigns new options. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Passed when one or more required arguments are null. </exception>
    /// <exception cref="CommandValidationException"> Thrown in case command arguments validation error. </exception>
    ///
    /// <param name="parsedArgs">       The parsed arguments of command. Can't be null. </param>
    /// <param name="display">         The callback interface with displaying capability. May be null. </param>
    ///
    /// <returns>
    /// Successful IComplexResult if arguments were validated against command options successfully, and command processing may continue.
    /// Null if there was only one argument, help option "h". Such situation is ok, but processing should not continue.
    /// Failed IComplexResult if arguments validation failed, and command processing should stop.
    /// </returns>
    public virtual IComplexResult Validate(
        IReadOnlyDictionary<string, string> parsedArgs,
        IConsoleDisplay display)
    {
        ArgumentNullException.ThrowIfNull(parsedArgs);

        // Create a new separate instance of TOptions, to let ValidateCommand making any changes just on that
        TOptions options = new TOptions();
        IComplexResult result = CommandHelper.ValidateCommand(this, parsedArgs, options, display);

        if (result == null)
        {   // just help
            ShowHelp(options, display);
        }
        else if (!result.Success)
        {
            ShowHelp(options, display);
            // if something has failed
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                // error is already logged in CommandHelper.ValidateCommand, and displayed if display != null
                throw new CommandValidationException(result.ErrorMessage, (display != null), true);
            }
        }
        else
        {
            // ok, will want to proceed (execute command) with these input arguments, must assign them
            ResetCommandOptions(options);
        }

        return result;
    }

    /// <summary> Gets command options. </summary>
    public abstract ICommandOptions GetCommandOptions();

    /// <summary>   Executes this command. </summary>
    /// <returns>   An IComplexErrorResult, describing success or possible error. </returns>
    public abstract IComplexErrorResult<TErrorCode> Execute();

    /// <summary>   Show help for command. </summary>
    /// 
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception> 
    ///
    /// <param name="commandOption">    The command option. Can't be null. </param>
    /// <param name="display">         The callback interface with displaying capability. May be null. </param>
    public virtual void ShowHelp(ICommandOptions commandOption, IConsoleDisplay display)
    {
        ArgumentNullException.ThrowIfNull(commandOption);

        string description = CommandHelper.PrintDescription(Usage, commandOption);
        display?.WriteText(description);
    }

    /// <inheritdoc/>
    public virtual void ShowSuccessfulExecution(IConsoleDisplay display)
    {
        display?.WriteSuccess(Invariant($"{Name} command succeeded"));

    }
    #endregion // ICommand Members

    #region Methods

    /// <summary>
    /// Resets the command options of this instance, assigning a new value <paramref name="newOptions"/>.
    /// </summary>
    ///
    /// <param name="newOptions"> (Optional) The new value to be assigned. It can be null, in that case
    /// the implementation should just create a new <typeparamref name="TOptions"/> of its own. </param>
    ///
    /// <returns> The original (old) value of command options. </returns>
    protected abstract TOptions ResetCommandOptions(TOptions newOptions = default);
    #endregion // Methods
}
#pragma warning restore IDE0090 // Use 'new(...)'
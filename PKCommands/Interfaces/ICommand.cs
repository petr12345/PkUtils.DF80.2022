using System;
using System.Collections.Generic;
using PK.PkUtils.Consoles;
using PK.PkUtils.Interfaces;

namespace PK.Commands.Interfaces;


/// <summary>
/// Defines a command interface with execution, validation, and help functionality.
/// </summary>
/// <typeparam name="TErrorCode">Type of error details.</typeparam>
[CLSCompliant(true)]
public interface ICommand<out TErrorCode>
{
    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a description of the command's usage.
    /// </summary>
    string Usage { get; }

    /// <summary>
    /// Gets a collection of valid mandatory option sets. Each set is represented as an IEnumerable{string},  
    /// and the options provided for the command must match one of these sets.  
    /// The help option "-h" is not included in these sets.
    /// </summary>
    /// <remarks>
    /// The return type could be IEnumerable{HashSet{string}}, but that would require a predefined string comparer.  
    /// Instead, it is left to the calling code (e.g., <c>CommandHelper.ValidateCommand</c> using <c>CommandHelper.OptionNamesComparer</c>) 
    /// to determine the appropriate comparer.
    /// </remarks>
    IReadOnlyCollection<IEnumerable<string>> MandatoryOptions { get; }

    /// <summary>
    /// Validates the provided arguments and options.
    /// </summary>
    /// <param name="parsedArgs">The parsed arguments. Cannot be <c>null</c>.</param>
    /// <param name="display">An optional display interface for output. Can be <c>null</c>.</param>
    /// <returns>
    /// - A successful <see cref="IComplexResult"/> if validation passes, allowing command execution to proceed.  
    /// - <c>null</c> if the only argument is the help option ("-h"), indicating that execution should not continue.  
    /// - A failed <see cref="IComplexResult"/> if validation fails, preventing further execution.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="parsedArgs"/> is <c>null</c>.</exception>
    IComplexResult Validate(IReadOnlyDictionary<string, string> parsedArgs, IConsoleDisplay display);

    /// <summary>
    /// Retrieves the command options that will be used during execution.
    /// </summary>
    /// <returns>An instance of <see cref="ICommandOptions"/> representing the command options.</returns>
    ICommandOptions GetCommandOptions();

    /// <summary> Executes the command. </summary>
    /// <returns>
    /// An <see cref="IComplexErrorResult"/> indicating success or providing details about a possible error.
    /// </returns>
    IComplexErrorResult<TErrorCode> Execute();

    /// <summary>
    /// Displays help information for the command.
    /// </summary>
    /// <param name="commandOption">The command option. Cannot be <c>null</c>.</param>
    /// <param name="display">An optional display interface for output. Can be <c>null</c>.</param>
    void ShowHelp(ICommandOptions commandOption, IConsoleDisplay display);

    /// <summary> Shows the successful execution. </summary>
    /// <param name="display"> An optional display interface for output. Can be <c>null</c>. </param>
    void ShowSuccessfulExecution(IConsoleDisplay display);
}

// Ignore Spelling: Utils, Gsl
// 
using System;
using System.Collections.Generic;
using System.Linq;
using PK.Commands.Interfaces;
using PK.PkUtils.Cmd;
using PK.PkUtils.Consoles;
using PK.PkUtils.Extensions;
using ILogger = log4net.ILog;

#pragma warning disable IDE0290     // Use primary constructor

namespace PK.Commands.CommandProcessing;

/// Provides command-line argument processing using dual-format syntax, 
/// supporting both key-value pairs and switch-based arguments.
/// This class extends CommandsInputProcessor and overrides argument parsing to use DualFormatCmdParametersProvider.
///
/// Supported two command-line formats:
///   - Key-value pairs: /ParamName1 Value1 /ParamName2 Value2 /ParamName3 Value3
///   - Colon-delimited pairs: /ParamName1:Value1 /ParamName2:Value2 -ParamName3:Value3
///
/// Switches: If AcceptSwitchesSyntax is true, arguments without values (e.g., nologo) are interpreted as boolean switches.
///
/// Command name precedence: If input argument commandNamePrecedes is true, the first argument is treated as the command name.
/// Otherwise, the command name is derived from the first parameter.
///
/// <typeparam name="TCommand">Type of command to process. Must implement ICommand.</typeparam>
/// <typeparam name="TErrorCode">Type representing error details.</typeparam>

public class DualFormatInputProcessor<TCommand, TErrorCode> : CommandsInputProcessor<TCommand, TErrorCode>
    where TCommand : class, ICommand<TErrorCode>
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
    public DualFormatInputProcessor(
        ILogger logger,
        IConsoleDisplay display,
        ICommandRegister<TCommand, TErrorCode> commandRegister = null,
        bool commandNamePrecedes = true)
        : base(logger, display, commandRegister, commandNamePrecedes)
    { }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Gets or initializes a value indicating whether <see cref="ParseInputArgs"/> accepts switches syntax.
    /// </summary>
    /// <seealso cref="ParseInputArgs"/>
    public bool AcceptSwitchesSyntax { get; init; } = true;
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Parses input arguments <paramref name="cmdLineArgs"/>.  
    /// Overrides the implementation of the base class, because of need to use <see cref="DualFormatCmdParametersProvider"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> Thrown when the input <paramref name="cmdLineArgs"/> is empty. </exception>
    /// <exception cref="ArgumentNullException"> Thrown when the input <paramref name="cmdLineArgs"/> is null. </exception>
    /// <param name="cmdLineArgs"> The command line arguments. Can't be null or empty, since we assume the first
    /// argument is the name of the command to process. </param>
    /// <param name="cmdName"> [out] Name of the command, taken from the first argument. </param>
    /// <returns> The rest of arguments, parsed into name-value dictionary. </returns>
    /// <remarks> This implementation uses <see cref="DualFormatCmdParametersProvider"/>,
    /// providing <see cref="AcceptSwitchesSyntax"/> boolean value to supports switch syntax.
    /// It is preferred to keep that value true, for consistency with the base class CommandsInputProcessor.ParseInputArgs.
    /// </remarks>
    protected internal override IReadOnlyDictionary<string, string> ParseInputArgs(
        IEnumerable<string> cmdLineArgs,
        out string cmdName)
    {
        ArgumentNullException.ThrowIfNull(cmdLineArgs);
        if (cmdLineArgs.IsEmpty()) { throw new ArgumentException("Arguments can't be empty", nameof(cmdLineArgs)); }

        string commandName;
        DualFormatCmdParametersProvider provider;
        bool acceptSwitchesSyntax = AcceptSwitchesSyntax;

        if (CommandNamePrecedes)
        {
            commandName = cmdLineArgs.First();
            provider = new DualFormatCmdParametersProvider(cmdLineArgs.Skip(1), acceptSwitchesSyntax);
        }
        else
        {
            provider = new DualFormatCmdParametersProvider(cmdLineArgs, acceptSwitchesSyntax);
            commandName = provider.OriginalArgumentsOrder[0];
        }

        cmdName = commandName;
        return provider.AllParameters;
    }
    #endregion // Methods
}


#pragma warning restore IDE0290     // Use primary constructor/// <summary>


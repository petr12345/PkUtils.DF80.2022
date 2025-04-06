using System;
using PK.Commands.BaseCommands;
using PK.Commands.CommandProcessing;
using PK.Commands.Interfaces;
using PK.Commands.Parameters;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Interfaces;
using ILogger = log4net.ILog;

namespace GZipTest.Commands;

/// <summary> A clear command. </summary>
/// <remarks>   Constructor. </remarks>
///
/// <param name="logger"> The logger. Can't be null. </param>
internal class ClearCommand(ILogger logger) : BaseCommandEx<ClearCommand.ClearOptions, ExitCode>(logger)
{
    #region Typedefs

    public class ClearOptions : ICommandOptions
    {
        /// <summary>   An Option&lt;Boolean&gt; to process. </summary>
        /// <remarks> Bool is just "fictive" value of that option; important is just the command-line presence. </remarks>
        public ParameterOption<Boolean> h = new()
        {
            option = nameof(h),
            description = "Displays Help"
        };
    }
    #endregion // Typedefs

    #region ICommand Members

    /// <inheritdoc/>
    public override string Name { get => "Clear"; }

    /// <inheritdoc/>
    public override string Usage { get => Name; }

    /// <inheritdoc/>
    public override IComplexResult Execute()
    {
        Console.Clear();
        return ComplexResult.OK;
    }
    #endregion // ICommand Members
}

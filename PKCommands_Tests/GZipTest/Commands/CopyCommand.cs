using System;
using System.Collections.Generic;
using System.Linq;
using GZipTest.Infrastructure;
using PK.PkUtils.Extensions;
using static PK.Commands.CommandUtils.CommandHelper;
using ILogger = log4net.ILog;


namespace GZipTest.Commands;

/// <summary> File copying command, creating an exact copy target from source.
///           
/// General usage syntax: 
/// <code> 
/// <![CDATA[
/// Copy -source <input_file> -target <output_file>
/// ]]> 
/// </code>
/// 
/// Examples of command-line:
///    Copy -source d:\_Movies\King.Kong\King_Kong_(1933).avi -target "d:\_Movies\King.Kong\King_Kong_(1933) copy.avi"
///    Copy -source d:\_Movies\King.Kong\King_Kong_(1933).avi -target "King_Kong_(1933) copy.avi"
///    Copy -h
///    Copy ?
/// </summary>
internal class CopyCommand : FileProcessingBaseCommand<CopyCommand.CopyOptions>
{
    #region Typedefs

    /// <summary> An instance of this class is returned as ICommandOptions from CopyCommand.GetCommandOptions(). </summary>
    internal class CopyOptions : FileProcessingOptions
    { }
    #endregion // Typedefs

    #region Constructor(s)

    /// <summary> The only constructor. </summary>
    /// <param name="logger"> The logger. Can't be null. </param>
    internal CopyCommand(ILogger logger) : base(FileBlock.BlockStatus.ReadDone, logger)
    { }
    #endregion // Constructor(s)

    #region IFileProcessingCommand Members
    #region ICommand Members

    /// <inheritdoc/>
    public override string Name { get => "Copy"; }

    /// <inheritdoc/>
    public override string Usage
    {
        // For proper syntax examples, see 
        // https://publib.boulder.ibm.com/tividd/td/tec/SC32-1232-00/en_US/HTML/ecormst15.htm
        // 
        get
        {
            const string sourceExample_1st = @"d:\_Movies\King.Kong\King_Kong_(1933).avi";
            const string destExample_1st = @"""d:\_Movies\King.Kong\King_Kong_(1933) copy.avi""";
            const string sourceExample_2nd = @"""d:\_Movies\King.Kong\King Kong contra Godzilla.mpg""";
            const string destExample_2nd = @"""King Kong contra Godzilla copy.mpg""";
            string examplePadding = new(' ', EXAMPLE_prefix.Length);

            string cmdSourcePart = $"-{nameof(CopyOptions.source)} <input_file>";
            string cmdTargetPart = $"[-{nameof(CopyOptions.target)} <output_file>]";

            string exampleSourcePart_1st = $"-{nameof(CopyOptions.source)} {sourceExample_1st}";
            string exampleTargetPart_1st = $"-{nameof(CopyOptions.target)} {destExample_1st}";
            string exampleSourcePart_2nd = $"-{nameof(CopyOptions.source)} {sourceExample_2nd}";
            string exampleTargetPart_2nd = $"-{nameof(CopyOptions.target)} {destExample_2nd}";

            string[] lines = [
                $"{cmdSourcePart} {cmdTargetPart}",
                $"{UsagePrefixWhitespaced}{HelpOptionsNames.Join(", ")}",
                EXAMPLE_prefix + exampleSourcePart_1st + " " + exampleTargetPart_1st,
                examplePadding + exampleSourcePart_2nd + " " + exampleTargetPart_2nd];

            return lines.Join(Environment.NewLine);
        }
    }

    /// <inheritdoc/>
    public override IReadOnlyCollection<IEnumerable<string>> MandatoryOptions
    {
        get
        {
            IEnumerable<string> firstPossible = [nameof(CopyOptions.source)];
            IEnumerable<string> secondPossible = [nameof(CopyOptions.source), nameof(CopyOptions.target)];

            return [firstPossible, secondPossible];
        }
    }
    #endregion // ICommand Members
    #endregion // IFileProcessingCommand Members

    #region Methods
    protected override void InitExecution()
    {
        base.InitExecution();
        ConsoleDisplay.WriteInfo($"Executing {Name} command, {SourceFilePath.AsNameValue()}, {TargetFilePath.AsNameValue()}");
    }
    #endregion // Methods
}
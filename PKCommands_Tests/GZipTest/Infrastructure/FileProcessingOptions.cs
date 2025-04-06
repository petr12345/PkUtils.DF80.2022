using System;
using PK.Commands.Interfaces;
using PK.Commands.Parameters;

namespace GZipTest.Infrastructure;

/// <summary> An instance of this class is returned as ICommandOptions from CopyCommand.GetCommandOptions(). </summary>
internal class FileProcessingOptions : ICommandOptions
{
    /// <summary> A field that keeps specified imported document path. </summary>
    public readonly ParameterOption<string> source = new()
    {
        option = nameof(FileProcessingOptions.source),
        description = "input source file",
        possibleValues = null
    };

    /// <summary> A field that keeps mode option, with option values <see cref="ImportMode"/>. </summary>
    public readonly ParameterOption<string> target = new()
    {
        option = nameof(FileProcessingOptions.target),
        description = "target destination file",
        possibleValues = null
    };

    /// <summary> A field that keeps "h" option, i.e. help. </summary>
    public readonly ParameterOption<Boolean> h = new()
    {
        option = nameof(FileProcessingOptions.h),
        description = "Displays Help"
    };
}

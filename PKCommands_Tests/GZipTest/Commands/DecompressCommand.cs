using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GZipTest.Infrastructure;
using PK.Commands.CommandProcessing;
using PK.Commands.CommandUtils;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using ILogger = log4net.ILog;


namespace GZipTest.Commands;

/// <summary> File decompressing command, creating decompressed target from source.
/// 
/// Examples of command-line:
///    Decompress -source c:\zipped.zzz -target c:\_Movies\Movies_kung-fu\Enter_the_Dragon(1973).mp4
///    Decompress -h
///    Decompress ?
/// </summary>
internal class DecompressCommand(ILogger logger) :
    FileProcessingBaseCommand<DecompressCommand.DecompressOptions>(FileBlock.BlockStatus.Processed, logger)
{
    #region Typedefs

    /// <summary> An instance of this class is returned as ICommandOptions from DecompressCommand.GetCommandOptions(). </summary>
    internal class DecompressOptions : FileProcessingOptions
    {
    }
    #endregion // Typedefs

    #region IFileProcessingCommand Members
    #region ICommand Members

    /// <inheritdoc/>
    public override string Name { get => "Decompress"; }

    /// <inheritdoc/>
    public override string Usage
    {
        // For proper syntax examples, see 
        // https://publib.boulder.ibm.com/tividd/td/tec/SC32-1232-00/en_US/HTML/ecormst15.htm
        // 
        get
        {
            // Note: There are other ways how to include curly braces in formated string, but all rather stupid. 
            // See more on 
            // https://stackoverflow.com/questions/91362/how-to-escape-braces-curly-brackets-in-a-format-string-in-net
            // 
            // 

            string sourcePathPart = string.Format(CultureInfo.InvariantCulture,
                " -{0} c:\\_Movies\\Movies_kung-fu\\Enter_the_Dragon(1973).mp4", nameof(DecompressOptions.source));
            string targetPathPart = string.Format(CultureInfo.InvariantCulture,
                " -{0} {1}", nameof(DecompressOptions.target), "target_file");

            string firstLine = sourcePathPart + targetPathPart;
            string secondLine = CommandHelper.UsagePrefixWhitespaced + CommandHelper.HelpOptionsNames.Join(",");

            return firstLine + Environment.NewLine + secondLine;
        }
    }

    /// <inheritdoc/>
    public override IReadOnlyCollection<IEnumerable<string>> MandatoryOptions
    {
        get
        {
#if JUST_TESTING_NONSENSE
            IEnumerable<string> firstPossible = new string[] { nameof(DecompressOptions.Env), nameof(DecompressOptions.mode) };
            IEnumerable<string> secondPossible = new string[] { nameof(DecompressOptions.Env), nameof(DecompressOptions.documentPath), "modrajedobra" };
            IEnumerable<string> thirdPossible = new string[] { nameof(DecompressOptions.Env), nameof(DecompressOptions.documentPath), "zelenalepsi", "Mike", "David" };
            var all = new IEnumerable<string>[] { firstPossible, secondPossible, thirdPossible };
            return all;
#else
            IEnumerable<string> firstPossible = [];
            IEnumerable<string> secondPossible = [nameof(DecompressOptions.source), nameof(DecompressOptions.target)];
            return [firstPossible, secondPossible];
#endif // JUST_TESTING_NONSENSE
        }
    }

    /// <inheritdoc/>
    public override IComplexErrorResult<ExitCode> Execute()
    {
        IComplexErrorResult<ExitCode> copyRes = PerformDecompress();
        IComplexErrorResult<ExitCode> result = copyRes;

        return result;
    }
    #endregion // ICommand Members
    #endregion // IFileProcessingCommand Members

    #region Methods

    /// <summary>   An auxiliary method called from <see cref="Execute"/> . </summary>
    ///
    /// <returns>   An ExitCode. </returns>
    protected IComplexErrorResult<ExitCode> PerformDecompress()
    {
        Logger.Info("Running PerformDecompress");
        return ComplexErrorResult<ExitCode>.OK;
    }
    #endregion // Methods
}
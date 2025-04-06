using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using GZipTest.Infrastructure;
using PK.PkUtils.Extensions;
using static PK.Commands.CommandUtils.CommandHelper;
using ILogger = log4net.ILog;

#pragma warning disable IDE0017 // Object initialization can be simplified
#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace GZipTest.Commands;

/// <summary> File compressing command, creating compressed target from source.
///           
/// General usage syntax: 
/// <code> 
/// <![CDATA[
/// Compress  -source <input_file> [-target <output_file>] 
/// ]]> 
/// </code>
/// 
/// Examples of command-line:
///    Compress -source c:\_Movies\Movies_kung-fu\Enter_the_Dragon(1973).mp4 -target c:\_Movies\Movies_kung-fu\Enter_the_Dragon(1973).zzz
///    Compress -h
///    Compress ?
/// </summary>
internal class CompressCommand : FileProcessingBaseCommand<CompressCommand.CompressOptions>
{
    #region Typedefs

    /// <summary> An instance of this class is returned as ICommandOptions from CompressCommand.GetCommandOptions(). </summary>
    internal class CompressOptions : FileProcessingOptions
    { }
    #endregion // Typedefs

    #region Constructor(s)

    /// <summary> The only constructor. </summary>
    /// <param name="logger"> The logger. Can't be null. </param>
    internal CompressCommand(ILogger logger)
        : base(FileBlock.BlockStatus.Processed, logger)
    { }
    #endregion // Constructor(s)

    #region IFileProcessingCommand Members

    /// <inheritdoc/>
    public override string Name { get => "Compress"; }

    /// <inheritdoc/>
    public override string Usage
    {
        // For proper syntax examples, see 
        // https://publib.boulder.ibm.com/tividd/td/tec/SC32-1232-00/en_US/HTML/ecormst15.htm
        // 
        get
        {
            const string sourceExample_1st = @"d:\_Movies\King.Kong\Kong.Skull.Island.2017.mp4";
            const string destExample_1st = @"""d:\_Movies\King.Kong\Kong.Skull.Island.2017.zzz""";
            const string sourceExample_2nd = @"d:\_Movies\King.Kong\Kong.Skull.Island.2017.mp4";
            const string destExample_2nd = @"""Kong.Skull.Island.2017.zzz""";
            string examplePadding = new(' ', EXAMPLE_prefix.Length);

            string cmdSourcePart = $"-{nameof(CompressOptions.source)} <input_file>";
            string cmdTargetPart = $"-{nameof(CompressOptions.target)} <output_file>";

            string exampleSourcePart_1st = $"-{nameof(CompressOptions.source)} {sourceExample_1st}";
            string exampleTargetPart_1st = $"-{nameof(CompressOptions.target)} {destExample_1st}";
            string exampleSourcePart_2nd = $"-{nameof(CompressOptions.source)} {sourceExample_2nd}";
            string exampleTargetPart_2nd = $"-{nameof(CompressOptions.target)} {destExample_2nd}";

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
            IEnumerable<string> singletPossible = [nameof(CompressOptions.source), nameof(CompressOptions.target)];
            return [singletPossible];
        }
    }

    /// <inheritdoc/>
    public override void Enqueue(FileBlock newBlock)
    {
        lock (QueueLock)
        {
            base.Enqueue(newBlock);
            Debug.Assert(ReferenceEquals(Tail, newBlock));

            if (!IsBlockReadyForWriting(newBlock))
            {
                RunCompressingThread(newBlock);
            }
        }
    }
    #endregion // IFileProcessingCommand Members

    #region Methods

    protected override void InitExecution()
    {
        base.InitExecution();
        ConsoleDisplay.WriteInfo($"Executing {Name} command, {SourceFilePath.AsNameValue()}, {TargetFilePath.AsNameValue()}");
    }

    protected void RunCompressingThread(FileBlock newBlock)
    {
        ArgumentNullException.ThrowIfNull(newBlock);

        // Start a new thread for compression
        Thread compressingThread = new(() =>
        {
            try
            {
                CompressBlock(newBlock);

                // Set the status to Processed after compression
                newBlock.ChangeStatus(FileBlock.BlockStatus.Processed);
                // If the block is in the head, signal the event
                CheckIfHeadChanged(newBlock);
            }
            catch (Exception ex)
            {
                Logger.Error("Error in compression thread: " + ex.Message);
            }
        });

        // Start the thread
        compressingThread.IsBackground = true;
        compressingThread.Start();
    }

    /// <summary>
    /// Compresses the data in the given file block. The method compresses the data in the block and resizes the buffer
    /// if necessary to hold the compressed data. It updates the block's actual size to match the compressed data size.
    /// </summary>
    /// <param name="newBlock"> The file block to be compressed. </param>
    private void CompressBlock(FileBlock newBlock)
    {
        // Ensure the block has some data to compress
        if (newBlock.ActualSize == 0)
        {
            Logger.Warn("No data to compress in the block.");
            return;
        }

        // Compress the data in the block buffer
        byte[] compressedData = ZipUtils.CompressBytes(newBlock.Buffer.Take(newBlock.ActualSize).ToArray());

        // Assign the new buffer with compressed data
        newBlock.AssignNewBuffer(compressedData, compressedData.Length);
    }

    private void CheckIfHeadChanged(FileBlock block)
    {
        lock (QueueLock)
        {
            if ((block.Status == FileBlock.BlockStatus.Processed) && (Head == block))
            {
                // If the block is at the head of the queue, signal the event
                EventWritingBlockReady.Set();
            }
        }
    }
    #endregion // Methods
}
#pragma warning restore IDE0305
#pragma warning restore IDE0017
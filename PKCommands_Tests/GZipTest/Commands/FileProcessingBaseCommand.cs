using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using GZipTest.CommandThreads;
using GZipTest.Infrastructure;
using PK.Commands.BaseCommands;
using PK.Commands.CommandExceptions;
using PK.Commands.CommandProcessing;
using PK.PkUtils.Consoles;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.IO;
using PK.PkUtils.Utils;
using PK.PkUtils.WinApi;
using static System.FormattableString;

using ILogger = log4net.ILog;

#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace GZipTest.Commands;

/// <summary> A base class of all commands here. </summary>
/// <typeparam name="TProcessingOptions"> Type of the options used. </typeparam>
internal abstract class FileProcessingBaseCommand<TProcessingOptions> :
    BaseCommandEx<TProcessingOptions, ExitCode>,
    IFileProcessingCommand
    where TProcessingOptions : FileProcessingOptions, new()
{
    #region Fields

    protected FileStream _sourceStream;
    protected FileStream _targetStream;
    protected ReadingThread _readingThread;
    protected WritingThread _writingThread;

    private readonly object _queueLock = new();
    private readonly FileBlock.BlockStatus _blockStatusReady4Writing;
    private readonly int _maxEncodingThreads;
    private readonly int _thresholdMaxBlocksInQueue;

    private Queue<FileBlock> _queueProcessing;
    private long _queueBytesSuma;
    private ManualResetEvent _errorEvent;
    private ManualResetEvent _queueCanConsume;
    private AutoResetEvent _writingBlockReady;
    private CancellationTokenSource _tokenSource;
    private IComplexErrorResult<ExitCode> _executionResult;
    private IConsoleDisplay _consoleDisplay;

    private const int _thresholdMemoryLoadPercent = 90;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>   Specialized constructor for use only by derived class. </summary>
    /// <exception cref="NullReferenceException">   Thrown when <paramref name="logger"/> is null. </exception>
    ///
    /// <param name="logger">   The logger. Can't be null. </param>
    /// <param name="blockStatusReady4Writing"> The desired status of file block, when processing is done. </param>
    protected FileProcessingBaseCommand(FileBlock.BlockStatus blockStatusReady4Writing, ILogger logger)
        : base(logger)
    {
        _maxEncodingThreads = Environment.ProcessorCount;
        _thresholdMaxBlocksInQueue = MaxEncodingThreads * 4;
        _blockStatusReady4Writing = blockStatusReady4Writing;
    }
    #endregion // Constructor(s)

    #region Properties(s)
    protected internal string SourceFilePath { get => Options.source.optionValue; }
    protected internal string TargetFilePath { get => Options.target.optionValue; }

    protected FileStream SourceStream { get => _sourceStream; }
    protected FileStream TargetStream { get => _targetStream; }

    protected ReadingThread ReadingThread { get => _readingThread; }
    protected WritingThread WritingThread { get => _writingThread; }

    protected FileBlock.BlockStatus BlockStatusReady4Writing { get => _blockStatusReady4Writing; }

    protected IConsoleDisplay ConsoleDisplay
    {
        get => _consoleDisplay ??= new ConsoleDisplay();
    }

    protected object QueueLock { get => _queueLock; }

    protected FileBlock Head { get => _queueProcessing.FirstOrDefault(); }
    protected FileBlock Tail { get => _queueProcessing.LastOrDefault(); }

    private int MaxEncodingThreads { get => _maxEncodingThreads; }
    private int ThresholdMaxBlocksInQueue { get => _thresholdMaxBlocksInQueue; }

    private static int TresholMemoryLoadPercent { get => _thresholdMemoryLoadPercent; }
    #endregion // Properties(s)

    #region Methods

    #region Initialization-related

    /// <summary>   Checks if <paramref name="actualPath"/> is a legal path. </summary>
    /// <remarks> Checks juts path "syntax", not actual file existence and accessibility. </remarks>
    ///
    /// <param name="actualPath"> Path of the file. Can't be null or empty. </param>
    /// <param name="argName">. Name of argument, for error message. Can't be null or empty. </param>
    /// <param name="errorMessage"> [out] Message describing the error. </param>
    ///
    /// <returns>   True if it succeeds, false if it fails. </returns>
    protected static bool CheckFilePath(string actualPath, string argName, out string errorMessage)
    {
        errorMessage = null;
        if (string.IsNullOrEmpty(actualPath))
        {
            errorMessage = Invariant($"Argument '{argName}' can't be null or empty string");
        }
        else
        {
            char[] invalidChars = FilePathHelper.PathInvalidOrWildCharacters.ToArray();
            var foundInvalidChars = actualPath.Where(c => invalidChars.Contains(c)).Distinct().ToArray();

            if (foundInvalidChars.Length > 0)
            {
                string invalidCharsStr = string.Join("', '", foundInvalidChars);
                errorMessage = Invariant($"The supplied {argName} '{actualPath}' contains invalid character(s): '{invalidCharsStr}'.");
            }
        }

        return (errorMessage == null);
    }

    /// <summary>   Opens a file stream if not already opened. </summary>
    /// <param name="stream"> [ref] The file stream reference. </param>
    /// <param name="filePath"> The file path. </param>
    /// <param name="mode"> The file mode. </param>
    /// <param name="access"> The file access mode. </param>
    /// <returns>   A complex result indicating success or failure. </returns>
    protected static IComplexErrorResult<ExitCode> OpenStream(
        ref FileStream stream,
        string filePath,
        FileMode mode,
        FileAccess access)
    {
        if (stream != null)
            return ComplexErrorResult<ExitCode>.OK;

        try
        {
            stream = new FileStream(filePath, mode, access);
            return ComplexErrorResult<ExitCode>.OK;
        }
        catch (SystemException ex)
        {
            return ComplexErrorResult<ExitCode>.CreateFailed(ex);
        }
    }

    /// <summary>   Validates the source and target input arguments (just syntax). </summary>
    ///
    /// <param name="display"> Zero-based index of the display. </param>
    /// <param name="errorMessage"> [out] Message describing the error. </param>
    ///
    /// <returns>   True if it succeeds, false if it fails. </returns>
    protected bool ValidateSourceAndTarget(IConsoleDisplay display, out string errorMessage)
    {
        string sourceFile = SourceFilePath;
        string targetFile = TargetFilePath;
        bool result = true;

        if (!CheckFilePath(sourceFile, nameof(FileProcessingOptions.source), out errorMessage))
            result = false;
        else if (!CheckFilePath(targetFile, nameof(FileProcessingOptions.target), out errorMessage))
            result = false;
        else
            errorMessage = null;

        if (!string.IsNullOrEmpty(errorMessage))
        {
            display.WriteError(errorMessage);
        }

        return result;
    }

    /// <summary>   Resets the queue. </summary>
    protected void ResetQueue()
    {
        _queueProcessing = new Queue<FileBlock>();
        _queueBytesSuma = 0;
    }

    /// <summary> Initializes command execution status before processing. </summary>
    protected virtual void InitExecution()
    {
        Debug.Assert(_errorEvent == null);
        Debug.Assert(_tokenSource == null);

        _tokenSource = new CancellationTokenSource();
        _errorEvent = new ManualResetEvent(false);
        _queueCanConsume = new ManualResetEvent(false);
        _writingBlockReady = new AutoResetEvent(false);
        _executionResult = null;

        ResetQueue();
        AdjustQueueConsumingEvent();
    }

    protected IComplexErrorResult<ExitCode> OpenSource() =>
        OpenStream(ref _sourceStream, SourceFilePath, FileMode.Open, FileAccess.Read);

    protected IComplexErrorResult<ExitCode> OpenTarget() =>
        OpenStream(ref _targetStream, TargetFilePath, FileMode.OpenOrCreate, FileAccess.Write);

    /// <summary> Opens source file and target file </summary>
    /// <returns> A ExitCode. </returns>
    protected IComplexErrorResult<ExitCode> OpenSourceAndTarget()
    {
        IComplexErrorResult<ExitCode> result = OpenSource();
        return result.Success ? OpenTarget() : result;
    }
    #endregion // Initialization-related

    #region Processing-related

    /// <summary>   Determines if the processing queue is too large. </summary>
    /// <returns>   True if the queue is too large, false otherwise. </returns>
    protected virtual bool IsQueueTooBig()
    {
        bool result = false;

        if (QueuedBlocks > ThresholdMaxBlocksInQueue)
        {
            result = true;
        }
        else if (QueuedBlocks > MaxEncodingThreads)
        {
            Kernel32.MEMORYSTATUS memStatus = new();
            Kernel32.GlobalMemoryStatus(ref memStatus);

            result = (memStatus.dwMemoryLoad > TresholMemoryLoadPercent);
        }

        return result;
    }

    /// <summary> Adjusts the queue consumption event based on queue size. </summary>
    protected virtual void AdjustQueueConsumingEvent()
    {
        if (IsQueueTooBig())
            EventQueueCanConsume.Reset();
        else
            EventQueueCanConsume.Set();
    }

    /// <summary> Adjusts the event signaling that a block is ready for writing. </summary>
    protected virtual void AdjustHeadReadyForWritingEvent()
    {
        lock (QueueLock)
        {
            if (QueuedBlocks > 0)
            {
                if (IsBlockReadyForWriting(Head))
                {
                    EventWritingBlockReady.Set();
                }
            }
        }
    }

    /// <summary>   Determines whether the given file block is ready for writing. </summary>
    /// <param name="block"> The file block to check. </param>
    /// <returns>   True if the block is ready for writing, fa
    protected virtual bool IsBlockReadyForWriting(FileBlock block)
    {
        bool result = block.CheckArgNotNull(nameof(block)).Status == BlockStatusReady4Writing;
        return result;
    }

    /// <summary>   Sets the execution result if not already set. </summary>
    /// <param name="result"> The result to set. </param>
    /// <exception cref="InvalidOperationException"> Thrown if the execution result is already set. </exception>
    protected void SetExecutionResult(IComplexErrorResult<ExitCode> result)
    {
        result.CheckArgNotNull(nameof(result));

        if (null == ExecutionResult)
        {
            _executionResult = result;
        }
        else
        {
            string erroMessage = Invariant($"The {this.ObjectTypeToReadable()} cannot set execution result twice.");
            Debug.Fail(erroMessage);
            throw new InvalidOperationException(erroMessage);
        }
    }

    /// <summary> Processes the source and target streams. </summary>
    protected virtual void ProcessSourceAndTarget()
    {
        Debug.Assert(_readingThread == null);
        Debug.Assert(_writingThread == null);

        _readingThread = new ReadingThread(this, this.SourceStream);
        _writingThread = new WritingThread(this, this.TargetStream);

        ReadingThread.Start();
        WritingThread.Start();

        ReadingThread.Join();
        WritingThread.Join();
    }
    #endregion // Processing-related

    #region Closing-related

    /// <summary> Implementation helper. Does not set execution result field. </summary>
    /// <exception cref="InvalidOperationException">  Thrown when the requested operation is invalid. </exception>
    /// <param name="stream"> The stream to act on. </param>
    /// <returns>  An IComplexResult </returns>
    protected static IComplexErrorResult<ExitCode> CheckedClose(FileStream stream)
    {
        IComplexErrorResult<ExitCode> result = ComplexErrorResult<ExitCode>.OK;

        if (stream != null)
        {
            try
            {
                stream.Close();  // Flush is included in Close itself
            }
            catch (SystemException ex)
            {
                result = ComplexErrorResult<ExitCode>.CreateFailed(ex);
            }
        }

        return result;
    }

    /// <summary>   Closes the target stream safely. </summary>
    /// <returns>   A complex result indicating success or failure. </returns>
    protected IComplexErrorResult<ExitCode> CheckedCloseTarget()
    {
        return CheckedClose(_targetStream);
    }

    /// <summary> Implementation helper. Does not set execution result field. </summary>
    protected void SilentCloseSourceAndTarget()
    {
        CheckedClose(_sourceStream);
        CheckedClose(_targetStream);
    }

    /// <summary>   Releases all resources used by the object. </summary>
    ///
    /// <param name="disposing"> True to release both managed and unmanaged resources; false to release only
    /// unmanaged resources. </param>
    protected virtual void Dispose(bool disposing)
    {
        // If disposing equals true, dispose both managed resources
        if (disposing)
        {
            Disposer.SafeDispose(ref _sourceStream);
            Disposer.SafeDispose(ref _targetStream);
            Disposer.SafeDispose(ref _readingThread);
            Disposer.SafeDispose(ref _writingThread);

            Disposer.SafeDispose(ref _tokenSource);
            Disposer.SafeDispose(ref _errorEvent);
            Disposer.SafeDispose(ref _queueCanConsume);
            Disposer.SafeDispose(ref _writingBlockReady);

            ResetQueue();
        }
    }
    #endregion // Closing-related
    #endregion // Methods

    #region IFileProcessingCommand Members

    #region ICommand Members

    /// <inheritdoc/>
    public override IComplexResult Validate(
        IReadOnlyDictionary<string, string> parsedArgs,
        IConsoleDisplay display)
    {
        IComplexResult result = base.Validate(parsedArgs, display);

        if ((result != null) && result.Success)
        {
            Debug.Assert(ValidatedArguments != null);

            if (!ValidateSourceAndTarget(display, out string errorMessage))
            {
                ValidatedArguments = null;
                throw new CommandValidationException(errorMessage, (display != null), false);
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public override IComplexErrorResult<ExitCode> Execute()
    {
        IComplexErrorResult<ExitCode> opening;
        IComplexErrorResult<ExitCode> result;

        try
        {
            InitExecution();
            opening = OpenSourceAndTarget();
            if (opening.Failed())
            {
                SetExecutionError(result = opening);
                SetLastError(ExitCode.IOError);
            }
            else
            {
                ProcessSourceAndTarget();

                // In case result is not set yet from any thread, we are still ok
                if (null == (result = ExecutionResult))
                {
                    SetExecutionResult(result = CheckedCloseTarget()); // must check successful close
                }
            }

            if (result.Failed())
            {
                SilentCloseSourceAndTarget(); // close both without reporting error
                if (result.ExceptionCaught() is null)
                {   // otherwise, the exception will be reported by the caller CommandsInputProcessor.RunCommand
                    ConsoleDisplay.WriteError(result.ErrorMessage);
                }
            }
        }
        finally
        {
            // cleanup any data accumulated during execution
            this.Dispose();
        }

        return result;
    }
    #endregion // ICommand Members

    #region IFileProcessingStatus Members

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get => _tokenSource.Token; }

    /// <inheritdoc/>
    public ManualResetEvent EventError { get => _errorEvent; }

    /// <inheritdoc/>
    public ManualResetEvent EventQueueCanConsume { get => _queueCanConsume; }

    /// <inheritdoc/>
    public AutoResetEvent EventWritingBlockReady { get => _writingBlockReady; }

    /// <inheritdoc/>
    public IComplexErrorResult<ExitCode> ExecutionResult { get => _executionResult; }

    /// <inheritdoc/>
    public bool ReadingThreadIsAlive { get => _readingThread.NullSafe(x => x.IsAlive); }

    /// <inheritdoc/>
    public int QueuedBlocks { get => _queueProcessing.Count; }

    /// <inheritdoc/>
    public long QueuedBytes { get => _queueBytesSuma; }

    /// <inheritdoc/>
    public void SetExecutionError(IComplexErrorResult<ExitCode> result)
    {
        SetExecutionResult(result);
        EventError.Set();
    }

    /// <inheritdoc/>
    public void SetExecutionCanceled()
    {
        SetExecutionResult(ComplexErrorResult<ExitCode>.CreateSuccessful());
        _tokenSource.Cancel();
    }

    /// <inheritdoc/>
    public virtual void Enqueue(FileBlock newBlock)
    {
        newBlock.CheckArgNotNull(nameof(newBlock));

        lock (QueueLock)
        {
            _queueProcessing.Enqueue(newBlock);
            _queueBytesSuma += newBlock.BufferLength;
            AdjustQueueConsumingEvent();
            if (QueuedBlocks == 1)
            {
                AdjustHeadReadyForWritingEvent(); // there is non-empty head now
            }
        }
    }

    /// <inheritdoc/>
    public virtual FileBlock Dequeue()
    {
        FileBlock block = null;

        lock (QueueLock)
        {
            if (QueuedBlocks > 0)
            {
                if (IsBlockReadyForWriting(Head))
                {
                    block = _queueProcessing.Dequeue();
                    _queueBytesSuma -= block.BufferLength;

                    AdjustQueueConsumingEvent();
                    AdjustHeadReadyForWritingEvent();  // there is different head now
                }
            }
        }

        return block;
    }
    #endregion // IFileProcessingStatus Members

    #region IDisposable Members

    /// <summary>Releases all resources used by the object.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposable Members
    #endregion // IFileProcessingCommand Members
}
#pragma warning restore IDE0305
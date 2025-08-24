using System.Threading;
using PK.Commands.CommandProcessing;
using PK.PkUtils.Interfaces;

namespace GZipTest.Infrastructure;

/// <summary> Interface for general processing status, shared between threads. </summary>
public interface IFileProcessingQueue
{
    #region Properties

    /// <summary> Used for cancellation of whole stuff. </summary>
    CancellationToken CancellationToken { get; }

    /// <summary> Gets becomes signaled if there was an error. </summary>
    ManualResetEvent EventError { get; }

    /// <summary> Event that becomes signaled when queue of read blocks is short enough to accept other blocks to be queued. </summary>
    ManualResetEvent EventQueueCanConsume { get; }

    /// <summary> Event that becomes signaled when the block in the head of the queue could be written. </summary>
    AutoResetEvent EventWritingBlockReady { get; }

    /// <summary> Gets the current execution result ( if any ). </summary>
    IComplexErrorResult<ExitCode> ExecutionResult { get; }

    /// <summary> Gets a value indicating whether the reading thread is alive. </summary>
    bool ReadingThreadIsAlive { get; }

    /// <summary> Gets the count of blocks queued for processing. </summary>
    int QueuedBlocks { get; }

    /// <summary> Gets the size of the data queued for processing, in bytes. </summary>
    long QueuedBytes { get; }

    #endregion // Properties

    #region Methods

    /// <summary>  Sets execution result. </summary>
    ///
    /// <param name="result"> The result of execution.  Can't be null. </param>
    void SetExecutionError(IComplexErrorResult<ExitCode> result);

    /// <summary>  Sets execution canceled. </summary>
    void SetExecutionCanceled();

    /// <summary>   Adds an object onto the end of processing queue. </summary>
    /// <param name="newBlock"> The new block. Can't be null. </param>
    void Enqueue(FileBlock newBlock);

    /// <summary>   Removes the head block from this queue. </summary>
    /// <returns>   The head object from this queue. </returns>
    FileBlock Dequeue();

    #endregion // Methods
}

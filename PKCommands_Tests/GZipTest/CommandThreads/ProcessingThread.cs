using System;
using System.Threading;
using GZipTest.Infrastructure;
using log4net;
using PK.PkUtils.Extensions;
using PK.PkUtils.Threading;
using ILogger = log4net.ILog;

namespace GZipTest.CommandThreads;

/// <summary> A base class for reading thread and writing thread. </summary>
[CLSCompliant(false)]
public abstract class ProcessingThread : WorkerThread
{
    private readonly IFileProcessingQueue _queue;
    private readonly ILogger _logger;

    /// <summary>   Default constructor. </summary>
    ///
    /// <param name="queue"> The file processing status. Can't be null. </param>
    protected ProcessingThread(IFileProcessingQueue queue)
        : base(ThreadPriority.Normal, attach: false, createWaitExitEvent: false)
    {
        _queue = queue.CheckArgNotNull(nameof(queue));
        _logger = LogManager.GetLogger(this.GetType());
        Name = GetType().Name;
    }

    protected ILogger Logger { get => _logger; }

    protected IFileProcessingQueue ProcessingQueue { get => _queue; }

    protected ManualResetEvent EventError { get => ProcessingQueue.EventError; }

    protected CancellationToken CancelToken { get => ProcessingQueue.CancellationToken; }

}

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using GZipTest.Infrastructure;
using PK.Commands.CommandProcessing;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using static System.FormattableString;

namespace GZipTest.CommandThreads;

/// <summary>   A reading thread. </summary>
[CLSCompliant(false)]
public class ReadingThread : ProcessingThread
{
    #region Typedefs

    private enum ReadingWaitCheck
    {
        Undefined,
        GotError,
        GotCancel,
        GotOK2Read,
        GotTimeout,
    }
    #endregion // Typedefs

    #region Fields

    private long _bytesReadSoFar;
    private readonly long _bytesInStream;
    private readonly FileStream _sourceStream;
    private const string ME = nameof(ReadingThread);
    #endregion // Fields

    #region Constructor(s)

    /// <summary>   Default constructor. </summary>
    /// <param name="queue">    The queue of processed blocks. </param>
    /// <param name="sourceStream"> Stream to read data from. </param>
    public ReadingThread(IFileProcessingQueue queue, FileStream sourceStream)
        : base(queue)
    {
        _sourceStream = sourceStream.CheckArgNotNull(nameof(sourceStream));
        _bytesInStream = _sourceStream.Length;
    }
    #endregion // Constructor(s)

    #region Properties

    protected long BytesInStream { get => _bytesInStream; }

    protected long BytesReadSoFar { get => _bytesReadSoFar; }

    protected long BytesRemaining { get => BytesInStream - BytesReadSoFar; }

    protected FileStream SourceStream { get => _sourceStream; }

    protected ManualResetEvent EventQueueCanConsume { get => ProcessingQueue.EventQueueCanConsume; }
    #endregion // Properties

    #region Methods

    /// <summary> The reading thread working function. </summary>
    protected override void WorkerFunction()
    {
        for (bool endThread = false; !endThread;)
        {
            switch (WaitingCheck(Timeout.Infinite))
            {
                case ReadingWaitCheck.GotError:
                case ReadingWaitCheck.GotCancel:
                    endThread = true;
                    break;

                case ReadingWaitCheck.GotOK2Read:
                    if (BytesReadSoFar < BytesInStream)
                    {
                        IComplexResult<ExitCode> read = ReadNextBlock(out FileBlock dataBlock);

                        if (!read.FullSuccess())
                        {
                            ProcessingQueue.SetExecutionError(read);
                            endThread = true;
                        }
                        else
                        {
                            ProcessingQueue.Enqueue(dataBlock);
                        }
                    }
                    else
                    {
                        endThread = true;
                    }
                    break;

                case ReadingWaitCheck.GotTimeout:
                    Debug.Fail("This case should not happen");
                    break;
            }
        }
    }

    protected IComplexResult<ExitCode> ReadNextBlock(out FileBlock dataBlock)
    {
        string description = $"{ME}.{nameof(ReadNextBlock)}";
        long remaining = BytesRemaining;
        IComplexResult<ExitCode> result = ComplexResult<ExitCode>.CreateSuccessful(ExitCode.Success); // ok so far

        if (remaining <= 0)
        {
            string errorMessage = Invariant($"{description} - don't call if nothing to read");
            Debug.Fail(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }

        int countToRead = (int)Math.Min(remaining, FileBlock.DefaultBlockSize);
        long position = SourceStream.Position;
        int gotNow = 0;
        SystemException gotException = null;
        FileBlock newBlock = new(countToRead) { FilePosition = position };

        dataBlock = null;
        try
        {
            gotNow = SourceStream.Read(newBlock.GetUnderlyingBuffer(), 0, countToRead);
            newBlock.ActualSize = gotNow;
            _bytesReadSoFar += gotNow;
        }
        catch (SystemException ex)
        {
            gotException = ex;
            Logger.Error(Invariant($"{description} encountered an exception"), ex);
        }

        if (gotException != null)
            result = ComplexResult<ExitCode>.CreateFailed(gotException);
        else if (gotNow < countToRead)
            result = new ComplexResult<ExitCode>(ExitCode.IOError);
        else
            dataBlock = newBlock.ChangeStatus(FileBlock.BlockStatus.ReadDone);

        return result;
    }

    private ReadingWaitCheck WaitingCheck(int millisecondsTimeout)
    {
        ReadingWaitCheck result = ReadingWaitCheck.Undefined;

        WaitHandle[] waitHandles = [CancelToken.WaitHandle, EventError, EventQueueCanConsume];
        int indxCancel = waitHandles.IndexOf(CancelToken.WaitHandle);
        int indxError = waitHandles.IndexOf(EventError);
        int indxQueueCanConsume = waitHandles.IndexOf(EventQueueCanConsume);
        int indxFinal = WaitHandle.WaitAny(waitHandles, millisecondsTimeout);

        if (indxFinal == indxCancel)
        {
            result = ReadingWaitCheck.GotCancel;
        }
        else if (indxFinal == indxError)
        {
            result = ReadingWaitCheck.GotError;
        }
        else if (indxFinal == indxQueueCanConsume)
        {
            result = ReadingWaitCheck.GotOK2Read;
        }
        else if (indxFinal == WaitHandle.WaitTimeout)
        {
            result = ReadingWaitCheck.GotTimeout;
        }
        else
        {
            string message = Invariant($"Unexpected value {nameof(indxFinal)}={indxFinal} returned from WaitHandle.WaitAny");
            Logger.Warn(message);
            Debug.Fail(message);
        }

        return result;
    }
    #endregion // Methods
}

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using GZipTest.Infrastructure;
using PK.PkUtils.Consoles;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using static System.FormattableString;

#pragma warning disable IDE0290     // Use primary constructor

namespace GZipTest.CommandThreads;

/// <summary> A writing thread. </summary>
[CLSCompliant(false)]
public class WritingThread : ProcessingThread
{
    #region Typedefs
    private enum WritingWaitCheck
    {
        Undefined,
        GotError,
        GotCancel,
        GotBlockToWrite,
        GotTimeout,
    }
    #endregion // Typedefs

    #region Fields

    private readonly FileStream _targetStream;
    private readonly IConsoleDisplay _consoleDisplay = new ConsoleDisplay();
    private const string ME = nameof(WritingThread);
    #endregion // Fields

    #region Constructor(s)

    /// <summary>   Default constructor. </summary>
    /// <param name="queue">    The queue of processed blocks. </param>
    public WritingThread(IFileProcessingQueue queue, FileStream targetStream)
        : base(queue)
    {
        _targetStream = targetStream.CheckArgNotNull(nameof(targetStream));
    }
    #endregion // Constructor(s)

    #region Properties

    protected FileStream TargetStream { get => _targetStream; }

    protected AutoResetEvent EventWritingBlockReady { get => ProcessingQueue.EventWritingBlockReady; }

    protected IConsoleDisplay ConsoleDisplay => _consoleDisplay;
    #endregion // Properties

    #region Methods

    /// <summary> The writing thread working function. </summary>
    protected override void WorkerFunction()
    {
        const int spinDeltaMilliseconds = 256;
        DateTime spinLastShown = DateTime.Now;
        FileBlock head;

        void ShowNextSpin()
        {
            DateTime now = DateTime.Now;
            TimeSpan delta = now - spinLastShown;
            if (delta.TotalMilliseconds >= spinDeltaMilliseconds)
            {
                ConsoleDisplay.ShowNextSpin();
                spinLastShown = now;
            }
        }

        // set-up console
        bool oldCursorVisible = Console.CursorVisible;
        Console.CursorVisible = false;
        Logger.Info($"Thread {Name} begins work");

        try
        {
            for (bool endThread = false; !endThread;)
            {
                switch (WaitingCheck(millisecondsTimeout: spinDeltaMilliseconds / 2))
                {
                    case WritingWaitCheck.GotError:
                    case WritingWaitCheck.GotCancel:
                        endThread = true;
                        break;

                    case WritingWaitCheck.GotBlockToWrite:
                        if ((head = ProcessingQueue.Dequeue()) is not null)
                            WriteNextBlock(head);
                        else
                            goto case WritingWaitCheck.GotTimeout;
                        break;

                    case WritingWaitCheck.GotTimeout:
                        if ((ProcessingQueue.QueuedBlocks == 0) && !ProcessingQueue.ReadingThreadIsAlive)
                        {
                            endThread = true;
                        }
                        break;
                }
                if (!endThread && ((DateTime.Now - spinLastShown).TotalMilliseconds >= spinDeltaMilliseconds))
                {
                    ShowNextSpin();
                }
            }
        }
        finally
        {
            Console.CursorVisible = oldCursorVisible;
        }
    }

    protected IComplexResult WriteNextBlock(FileBlock dataBlock)
    {
        ArgumentNullException.ThrowIfNull(dataBlock);

        string description = $"{ME}.{nameof(WriteNextBlock)}";
        IComplexResult result = ComplexResult.OK;

        Logger.Info($"{description} writing block ({dataBlock}).");
        try
        {
            int actualSize = dataBlock.ActualSize;
            Span<byte> intBuffer = stackalloc byte[sizeof(int)];

            // Write int to span buffer, in little-endian. This is consistent with BinaryWriter.Write(int)
            BitConverter.TryWriteBytes(intBuffer, actualSize);
            TargetStream.Write(intBuffer);
            TargetStream.Write(dataBlock.GetUnderlyingBuffer(), 0, actualSize);
        }
        catch (SystemException ex)
        {
            Logger.Error(Invariant($"{description} encountered an exception"), ex);
            result = ComplexResult.CreateFailed(ex);
        }

        return result;
    }

    private WritingWaitCheck WaitingCheck(int millisecondsTimeout)
    {
        WaitHandle[] waitHandles = [CancelToken.WaitHandle, EventError, EventWritingBlockReady];
        int indxFinal = WaitHandle.WaitAny(waitHandles, millisecondsTimeout);

        return indxFinal switch
        {
            0 => WritingWaitCheck.GotCancel,
            1 => WritingWaitCheck.GotError,
            2 => WritingWaitCheck.GotBlockToWrite,
            WaitHandle.WaitTimeout => WritingWaitCheck.GotTimeout,
            _ => HandleUnexpectedIndex(indxFinal)
        };
    }

    private WritingWaitCheck HandleUnexpectedIndex(int indxFinal)
    {
        string message = Invariant($"Unexpected value {nameof(indxFinal)}={indxFinal} returned from WaitHandle.WaitAny");
        Logger.Warn(message);
        Debug.Fail(message);
        return WritingWaitCheck.Undefined;
    }
    #endregion // Methods
}
#pragma warning restore IDE0290     // Use primary constructor
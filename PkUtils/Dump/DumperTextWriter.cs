// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.Threading;
using PK.PkUtils.Utils;

namespace PK.PkUtils.Dump;

/// <summary>
/// A <see cref="TextWriter"/> implementation that enqueues written text and
/// asynchronously forwards it to an <see cref="IDumper"/> target.
/// </summary>
/// 
/// <remarks>
/// <para>
/// Use this class when you want non-blocking text output: 
/// calls to <see cref="Write(string)"/> and <see cref="WriteLine(string)"/> enqueue the text 
/// and an internal background worker thread dequeues items and calls <see cref="IDumper.DumpText(string)"/> on the IDumper target.
/// </para>
/// 
/// <para>
/// The writer may be constructed with a null <see cref="IDumper"/>; queued text will be buffered
/// until an <see cref="IDumper"/> is assigned via <see cref="SetOutput(IDumper)"/>.
/// Access to the output and the internal queue is synchronized. Calling <see cref="Flush"/> will wait briefly
/// for the queue to drain if a target is set and the worker thread is still alive.
/// </para>
/// 
/// <para>
/// Disposal stops the worker thread (with a short timeout) and cleans up internal resources.
/// </para>
/// 
/// <para><b>Example</b>:</para>
/// <code>
/// // Typical usage: create the writer and add it as a Trace listener so Trace.Write / Trace.WriteLine
/// // forward text to your IDumper implementation without blocking the caller.
/// Trace.AutoFlush = true;
/// using var dumpWriter = new DumperTextWriter(this); // 'this' implements IDumper
/// Trace.Listeners.Add(new TextWriterTraceListener(dumpWriter));
/// 
/// // Later you can change the output target safely:
/// // dumpWriter.SetOutput(new MyOtherDumper());
/// </code>
/// </remarks>
[CLSCompliant(true)]
public class DumperTextWriter : TextWriter, IDisposableEx
{
    #region Typedefs

    /// <summary>
    /// The thread actually writing to the IDumper
    /// ( after picking-up the contents from the queue buffer ).
    /// </summary>
    [CLSCompliant(true)]
    protected class WriterThread : WorkerThread
    {
        #region Fields
        /// <summary>
        /// The owner of this object, is provided by the constructor.
        /// </summary>
        protected readonly DumperTextWriter _dumper;
        #endregion //Fields

        #region Properties
        /// <summary>
        /// The <see cref="DumperTextWriter"/> who has created and owns this thread.
        /// </summary>
        protected DumperTextWriter Dumper { get { return _dumper; } }
        #endregion //Properties

        #region Constructor(s)
        /// <summary>
        /// Constructs a new WriterThread, providing as an argument a caller who has created it
        /// </summary>
        /// <param name="dumper">An owner of this worker thread, whose input is processed</param>
        /// <remarks>
        /// We call the base constructor with the second argument true, 
        /// to enforce creating the event ManualResetEvent _evWaitExit.
        /// </remarks>
        protected internal WriterThread(DumperTextWriter dumper)
            : base(false, true)
        {
            _dumper = dumper ?? throw new ArgumentNullException(nameof(dumper));
            this.Name = "WriterThread of DumperTextWriter class";
            this.IsBackground = true;
        }
        #endregion // Constructor(s)

        #region Methods

        /// <summary>
        /// The thread working function; is actually delegating all received texts to provided IDumper.
        /// The method eats exceptions ThreadInterruptedException and ThreadAbortException.
        /// </summary>
        protected override void WorkerFunction()
        {
            try
            {
                for (bool bTargetDisposed = false; !this.IsStopRequest && !bTargetDisposed;)
                {
                    if (Dumper._evReady.WaitOne(_waitForWriteMs))
                    {
                        string strNext = null; // null indicates we have not found anything to write
                        Nullable<int> nWaitForOutput = null;
                        IDumper outputToUse = null;

                        // Enter upgradeable read lock to inspect Output and QueueBuffer
                        Dumper.SyncSlim.EnterUpgradeableReadLock();
                        try
                        {
                            // If there's no output, instruct to wait a bit before retry
                            if (Dumper.Output == null)
                            {
                                nWaitForOutput = _waitForWriteMs;
                            }
                            else
                            {
                                // If there is an output but nothing in queue, reset the event and loop
                                if (Dumper.QueueBuffer.Count == 0)
                                {
                                    Dumper._evReady.Reset();
                                }
                                else
                                {
                                    // Capture output and dequeue while holding write lock only briefly
                                    Dumper.SyncSlim.EnterWriteLock();
                                    try
                                    {
                                        if ((Dumper.Output is IDisposableEx iDisp) && iDisp.IsDisposed)
                                        {
                                            bTargetDisposed = true;
                                        }
                                        else
                                        {
                                            outputToUse = Dumper.Output;
                                            strNext = Dumper.QueueBuffer.Dequeue();

                                            // If we've drained the queue, reset the event so worker can sleep
                                            if (Dumper.QueueBuffer.Count == 0)
                                            {
                                                Dumper._evReady.Reset();
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        Dumper.SyncSlim.ExitWriteLock();
                                    }
                                }
                            }
                        }
                        finally
                        {
                            // Release upgradeable read lock BEFORE calling out to DumpText to avoid deadlocks
                            Dumper.SyncSlim.ExitUpgradeableReadLock();
                        }

                        // Call DumpText outside of any Dumper.SyncSlim locks to avoid reentrancy/deadlocks
                        if ((strNext != null) && !bTargetDisposed && (outputToUse != null))
                        {
                            try
                            {
                                outputToUse.DumpText(strNext);
                            }
                            catch (ObjectDisposedException /*ex*/)
                            { // eat the ObjectDisposedException.
                                bTargetDisposed = true;
                            }
                        }

                        if (nWaitForOutput.HasValue && !bTargetDisposed)
                        {
                            Thread.Sleep(nWaitForOutput.Value);
                        }
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
                Dumper.ClearQueueBuffer();
            }
            catch (ThreadAbortException)
            {
                Dumper.ClearQueueBuffer();
            }
        }
        #endregion // Methods
    }
    #endregion // Typedefs

    #region Fields

    #region Protected Fields

    /// <summary> The instance of WriterThread, who delegates all received texts to provided IDumper. The thread
    /// is disposed in DumperTextWriter.Dispose; after that <see cref="IsDisposed"/> always returns
    /// true. </summary>
    protected WorkerThread _writer;

    /// <summary> The value in milliseconds how long the DumperTextWriter.Dispose will wait for the worker thread
    /// proper stop, before aborting that manually. </summary>
    protected internal const int _defaultWaitForJoinMs = 512;
    #endregion // Protected Fields

    #region Private Fields
    /// <summary> The eventual target (output) for messages, initialized by constructor.  </summary>
    /// <remarks> Any access to the buffer queueBuffer and to the field _output must be synchronized through sync object </remarks>
    private IDumper _output;

    /// <summary> The event set when the queue buffer receives a new contents </summary>
    private ManualResetEvent _evReady;

    /// <summary> The encoding provided by constructor.  Is actually irrelevant, as all overwritten TextWriter methods do
    /// not write text to any file, but delegate the output to provided IDumper object. </summary>
    private readonly Encoding _encoding;

    /// <summary> The queue buffer where the overwritten Write and WriteLine puts the contents </summary>
    /// <remarks> Any access to the buffer queueBuffer and to the field _output must be synchronized through sync object </remarks>
    private readonly Queue<string> _queueBuffer = new();

    /// <summary> The sync object around the Queue buffer </summary>
    private ReaderWriterLockSlim _slim = new(LockRecursionPolicy.SupportsRecursion);

    private const int _waitDuringFlushMs = 32;
    private const int _waitForWriteMs = 128;
    #endregion // Private Fields
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Constructs a new instance of DumperTextWriter. Starts the internal thread, which will pick-up new items from the
    /// internal queue and "deliver" them to given IDumper, if that IDumper is not null. </summary>
    ///
    /// <remarks> If <paramref name="output"/> is null,  the Dumper.QueueBuffer just fills-in, till someone assigns a non-null output
    /// by <see cref="SetOutput"/> method. </remarks>
    ///
    /// <param name="output"> The object to which the  writer thread will eventually direct the output. May be null. </param>
    public DumperTextWriter(IDumper output)
      : this(output, System.Text.Encoding.Unicode)
    { }

    /// <summary> Constructs a new instance of DumperTextWriter. Starts the internal thread, which will pick-up new items from the
    /// internal queue and "deliver" them to given IDumper, if that IDumper is not null. </summary>
    /// 
    /// <remarks> If <paramref name="output"/> is null,  the Dumper.QueueBuffer just fills-in, till someone assigns a non-null output
    /// by <see cref="SetOutput"/> method. </remarks>
    ///
    /// <remarks> Since the writing is delegated to provided IDumper output, it is actually irrelevant which IFormatProvider is used
    /// as an argument of the base constructor. </remarks>
    ///
    /// <param name="output"> The object to which the  writer thread will eventually direct the output. May be null. </param>
    /// <param name="encoding"> . </param>
    public DumperTextWriter(IDumper output, Encoding encoding)
      : base(CultureInfo.InvariantCulture)
    {
        _output = output;
        _encoding = encoding;
        _evReady = new ManualResetEvent(false);
        _writer = new WriterThread(this);
        Writer.Start();
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Get the current output ( if there is any ). </summary>
    ///
    /// <remarks> Instead of simple setter, the value can be modified by method <see cref="SetOutput"/>,
    /// providing thread-safety. </remarks>
    public IDumper Output { get => _output; }

    /// <summary>
    /// Overrides the virtual property of the base class
    /// </summary>
    public override Encoding Encoding { get => _encoding; }

    /// <summary>
    /// Returns true if the internal queue buffer is empty; false otherwise.
    /// </summary>
    protected internal bool IsWriterQueueEmpty
    {
        get
        {
            SyncSlim.EnterReadLock();
            try
            {
                return (QueueBuffer.Count == 0);
            }
            finally
            {
                SyncSlim.ExitReadLock();
            }
        }
    }

    /// <summary>
    /// Accessing the instance of WorkerThread which performs writing
    /// </summary>
    protected WorkerThread Writer { get => _writer; }

    /// <summary> Get the synchronization object </summary>
    /// <remarks> It would be better to get here some wrapper objects around the 
    /// acquired reader locks and writer locks, similar way as in class RWLockWrapper.
    /// This is something ToDo </remarks>
    private ReaderWriterLockSlim SyncSlim { get => _slim; }

    /// <summary> Returns the queue buffer. </summary>
    /// <remarks> Any access to the buffer must be synchronized through sync object ( ReaderWriterLockSlim ) </remarks>
    private Queue<string> QueueBuffer { get => _queueBuffer; }
    #endregion // Properties

    #region Methods
    #region Public Methods

    #region General_stuff

    /// <summary> Sets the output, to which the worker thread will "deliver" the contents of internal queue. </summary>
    /// <param name="output"> The  <see cref="PK.PkUtils.Interfaces.IDumper "/> output ( may be null ).</param>
    public void SetOutput(IDumper output)
    {
        SyncSlim.EnterWriteLock();
        try
        {
            _output = output;
        }
        finally
        {
            SyncSlim.ExitWriteLock();
        }
    }

    /// <summary>
    /// Request to stop writing, wait the working thread to join
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait for the thread to terminate. </param>
    public void RequestStop(int millisecondsTimeout)
    {
        if (!this.IsDisposed)
        {
            if (Writer.IsAlive)
            {
                Writer.Priority = ThreadPriority.AboveNormal;
                Writer.Join(millisecondsTimeout); // sets its ManualResetEvent _evWaitExit.
            }
        }
    }

    /// <summary>
    /// Request to stop writing, wait the working thread to join;
    /// and if that fails, writing thread is aborted.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait for the thread to terminate. </param>
    public void RequestStopOrAbort(int millisecondsTimeout)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(millisecondsTimeout);
        RequestStop(millisecondsTimeout);
        if (Writer.IsAlive)
        {
            Writer.Abort();
        }
    }
    #endregion // General_stuff

    #region Overrwritten_TextWriter_methods

    /// <summary>
    /// Overwrites the virtual method of the base class.
    /// Puts the new string into internal queue.
    /// </summary>
    /// <param name="value">The string to write. </param>
    public override void Write(string value)
    {
        // warning CC1032: Method Write(...) overrides 'System.IO.TextWriter.Write(System.String)', thus cannot add Requires.
        // Contract.Requires<ArgumentNullException>(null != value, nameof(value));
        this.CheckNotDisposed();

        SyncSlim.EnterWriteLock();
        try
        {
            if ((!this.IsDisposed) && (!Writer.IsStopRequest))
            {
                QueueBuffer.Enqueue(value);
                _evReady.Set();
            }
        }
        finally
        {
            SyncSlim.ExitWriteLock();
        }
    }

    /// <summary>
    /// Overwrites the virtual method of the base class.
    /// Puts the new string into internal queue, with added newline.
    /// </summary>
    /// <param name="value">The string to write. </param>
    public override void WriteLine(string value)
    {
        Write(value + Environment.NewLine);
    }

    /// <summary>
    /// Clears all buffers for the current writer and causes any buffered data
    /// to be written to the underlying <see cref="IDumper"/> output.
    /// </summary>
    public override void Flush()
    {
        base.Flush();

        while ((Output != null) && !IsWriterQueueEmpty && Writer.IsAlive)
        {
            Thread.Sleep(_waitDuringFlushMs);
        }
    }
    #endregion // Overrwritten_TextWriter_methods
    #endregion // Public Methods

    #region Protected Methods

    /// <summary>
    /// Removes all contents from the internal queue buffer.
    /// </summary>
    protected void ClearQueueBuffer()
    {
        SyncSlim.EnterWriteLock();
        try
        {
            QueueBuffer.Clear();
        }
        finally
        {
            SyncSlim.ExitWriteLock();
        }
    }

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the method has been
    /// called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed. If
    /// disposing equals false, the method has been called by the runtime from inside the finalizer and you
    /// should not reference other objects. Only unmanaged resources can be disposed.
    /// </summary>
    ///
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
    protected override void Dispose(bool disposing)
    {
        // release managed resources first
        if (disposing)
        {
            if (!IsDisposed)
            { // get rid of the worker thread before anything else
                RequestStopOrAbort(_defaultWaitForJoinMs);
                Disposer.SafeDispose(ref _writer);
                Disposer.SafeDispose(ref _slim);
                Disposer.SafeDispose(ref _evReady);
            }
        }
        // No unmanaged resources here. Just call base
        base.Dispose(disposing);
    }
    #endregion // Protected Methods
    #endregion // Methods

    #region IDisposableEx Members

    /// <summary>
    ///  Returns true in case the object has been disposed and no longer should be used.
    /// </summary>
    public bool IsDisposed { get => (_writer == null); }

    #endregion // IDisposableEx Members
}

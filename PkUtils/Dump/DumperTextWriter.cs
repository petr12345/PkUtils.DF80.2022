/***************************************************************************************************************
*
* FILE NAME:   .\Dump\DumperTextWriter.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The class DumperTextWriter
*
**************************************************************************************************************/

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
/// DumperTextWriter works as a general output for TextWriterTraceListener,
/// delegating all received texts to provided IDumper. <br/>
/// Code using DumperTextWriter class may look like following 
/// ( assuming the related class provided as 'this' argument derives from IDumper ):
/// <code>
/// Trace.AutoFlush = true;
/// _dumpWriter = new DumperTextWriter(this);
/// Trace.Listeners.Add(new TextWriterTraceListener(_dumpWriter)); 
/// </code>
/// </summary>
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
                        IDisposableEx iDisp;
                        string strNext = null; // null indicates we have not found anything to write
                        Nullable<int> nWaitForOutput = null;

                        // the event became signaled, which means there is something to write, or at least the event about it has been received
                        Dumper.SyncSlim.EnterUpgradeableReadLock();
                        try
                        {
                            // do anything only if there is currently any output set at all
                            if (null == Dumper.Output)
                            {
                                // Assign following to indicate the thread should sleep, to give more chance the other thread to assign the output.
                                // The Sleep itself should not be here, but only in the finally part after the read lock is released.
                                nWaitForOutput = _waitForWriteMs;
                            }
                            else
                            { // Now figure-out if there is truly anything to write in the queue buffer. 
                              // Do not call IsWriterQueueEmpty, for performance reasons (to avoid another lock acquiring).
                                if (Dumper.QueueBuffer.Count == 0)
                                { // just set the state of the event to non-signaled
                                    Dumper._evReady.Reset();
                                    continue;
                                }

                                // --- Proceed with another item processing. Must acquire writer lock first
                                Dumper.SyncSlim.EnterWriteLock();
                                try
                                {
                                    iDisp = Dumper.Output as IDisposableEx;
                                    if ((null != iDisp) && iDisp.IsDisposed)
                                    {
                                        bTargetDisposed = true;
                                    }
                                    else
                                    {
                                        strNext = Dumper.QueueBuffer.Dequeue();
                                        try
                                        {
                                            Dumper.Output.DumpText(strNext);
                                        }
                                        catch (ObjectDisposedException /*ex*/)
                                        { // eat the ObjectDisposedException.
                                            /* string stackTrace = ex.StackTrace; */
                                            bTargetDisposed = true;
                                        }
                                    }
                                }
                                finally
                                {
                                    Dumper.SyncSlim.ExitWriteLock();
                                }
                            }
                        }
                        finally
                        {
                            Dumper.SyncSlim.ExitUpgradeableReadLock();
                            if (nWaitForOutput.HasValue && !bTargetDisposed)
                            {
                                Thread.Sleep(nWaitForOutput.Value);
                            }
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
    {
    }

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
    public IDumper Output
    {
        get { return _output; }
    }

    /// <summary>
    /// Overrides the virtual property of the base class
    /// </summary>
    public override Encoding Encoding
    {
        get
        {
            return _encoding;
        }
    }

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
    protected WorkerThread Writer
    {
        get { return _writer; }
    }

    /// <summary>
    /// Get the synchronization object
    /// </summary>
    /// <remarks> It would be better to get here some wrapper objects around the 
    /// acquired reader locks and writer locks, similar way as in class RWLockWrapper.
    /// This is something ToDo </remarks>
    private ReaderWriterLockSlim SyncSlim
    {
        get { return _slim; }
    }

    /// <summary> Returns the queue buffer. </summary>
    /// <remarks> Any access to the buffer must be synchronized through sync object ( ReaderWriterLockSlim ) </remarks>
    private Queue<string> QueueBuffer
    {
        get { return _queueBuffer; }
    }
    #endregion // Properties

    #region Methods
    #region Public Methods

    #region General_stuff

    /// <summary>
    /// Sets the output , to which the worker thread will "deliver" the contents of internal queue.
    /// </summary>
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
    public bool IsDisposed
    {
        get { return (_writer == null); }
    }
    #endregion // IDisposableEx Members
}

/***************************************************************************************************************
*
* FILE NAME:   .\Threading\WorkerThread.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class WorkerThread
*
**************************************************************************************************************/

// Ignore Spelling: Utils, Unstarted
//
using System;
using System.Diagnostics;
using System.Threading;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.Threading;

/// <summary>
/// WorkerThread is a wrapper around the System.Threading.Thread,
/// providing some more high-level encapsulation.
/// </summary>
public class WorkerThread : Repository<Thread>, IDisposableEx
{
    #region Fields

    /// <summary>
    /// A request for the thread to stop. 
    /// Created in constructor if required by its arguments, but may be null. 
    /// If used, will be created by the thread who called the constructor, 
    /// and set by the code using the thread ( either manually or by calling Join(int milliseconds) ).
    /// </summary>
    protected ManualResetEvent _evWaitExit;
    #endregion  // Fields

    #region Constructor(s)

    /// <summary>
    /// Default constructor. Creates the new thread with default priority, not attached to any existing
    /// thread, and without the internal ManualResetEvent _evWaitExit. </summary>
    public WorkerThread()
      : this(false, false)
    { }

    /// <summary>
    /// Creates the new thread with given priority, not attached to any existing thread, and without
    /// the internal ManualResetEvent _evWaitExit. </summary>
    /// <param name="priority"> The priority of newly created thread. </param>
    public WorkerThread(ThreadPriority priority)
      : this(priority, false, false)
    { }

    /// <summary>
    /// The constructor with two boolean arguments, that specify attaching to existing thread, and
    /// creating wait exit event. </summary>
    ///
    /// <param name="attach">  If true, the new WorkerThread will be attached to Thread.CurrentThread.
    ///   If false, a new thread will be created. </param>
    /// 
    /// <param name="createWaitExitEvent"> If true, the constructor will create a new
    ///   <see cref="ManualResetEvent"/> object and assign that to field <see cref="_evWaitExit "/>. </param>
    public WorkerThread(bool attach, bool createWaitExitEvent)
      : this(ThreadPriority.Normal, attach, createWaitExitEvent)
    { }

    /// <summary>
    /// The constructor with arguments regarding the thread priority, attaching to existing thread, and
    /// creating wait exit event. For the argument attach == true the first argument ThreadPriority is
    /// ignored. </summary>
    ///
    /// <param name="priority">   The priority of newly created thread. </param>
    /// 
    /// <param name="attach">   If true, the new WorkerThread will be attached to
    ///   System.Threading.Thread.CurrentThread. If false, a new thread will be created. </param>
    /// 
    /// <param name="createWaitExitEvent"> If true, the constructor will create a new
    ///   <see cref="ManualResetEvent"/> object and assign that to field <see cref="_evWaitExit "/>. </param>
    public WorkerThread(ThreadPriority priority, bool attach, bool createWaitExitEvent)
    {
        if (createWaitExitEvent)
        {
            _evWaitExit = new ManualResetEvent(false);
        }

        if (attach)
        {
            Attach(Thread.CurrentThread);
        }
        else
        {
            Thread thread = new(new ThreadStart(WorkerFunction))
            {
                Priority = priority,
                CurrentCulture = Thread.CurrentThread.CurrentCulture,
                CurrentUICulture = Thread.CurrentThread.CurrentUICulture
            };

            Keep(thread);
        }
    }
    #endregion  // Constructor(s)

    #region Properties

    /// <summary> Gets the wrapped managed thread. </summary>
    public Thread ManagedThread
    {
        get { return Data; }
    }

    /// <summary>
    /// Is the current thread this (wrapped) one?
    /// </summary>
    public bool IsActive
    {
        get { return Thread.CurrentThread == ManagedThread; }
    }

    /// <summary>
    /// Returns true if the thread never has been started; false otherwise.
    /// Throws ObjectDisposedException if this object is disposed already.
    /// </summary>
    public bool Unstarted
    {
        get
        {
            this.CheckNotDisposed();

            System.Threading.ThreadState nState = ManagedThread.ThreadState;
            bool bResult = (nState & System.Threading.ThreadState.Unstarted) != 0;

            return bResult;
        }
    }

    /// <summary>
    /// True if the thread is running.
    /// If this object is disposed already, returns simply false.
    /// </summary>
    public bool IsAlive
    {
        get { return ManagedThread != null && ManagedThread.IsAlive; }
    }

    /// <summary> Gets or sets a value indicating whether or not a thread is a background thread. </summary>
    /// <remarks>
    /// A thread is either a background thread or a foreground thread. Background threads are identical to
    /// foreground threads, except that background threads do not prevent a process from terminating. Once all
    /// foreground threads belonging to a process have terminated, the common language runtime ends the process.
    /// Any remaining background threads are stopped and do not complete.
    /// </remarks>
    /// <value> true if this object is background, false if not. </value>
    public bool IsBackground
    {
        get
        {
            this.CheckNotDisposed();
            return ManagedThread.IsBackground;
        }
        set
        {
            this.CheckNotDisposed();
            ManagedThread.IsBackground = value;
        }
    }

    /// <summary>
    /// Property representing the priority of the encapsulated Thread.
    /// Throws ObjectDisposedException if this object is disposed already.
    /// </summary>
    public ThreadPriority Priority
    {
        get
        {
            this.CheckNotDisposed();
            return ManagedThread.Priority;
        }
        set
        {
            this.CheckNotDisposed();
            ManagedThread.Priority = value;
        }
    }

    /// <summary>
    /// Property representing the name of the encapsulated Thread. The value can be null.
    /// Throws ObjectDisposedException if this object is disposed already.
    /// </summary>
    public string Name
    {
        get
        {
            this.CheckNotDisposed();
            return ManagedThread.Name;
        }
        set
        {
            this.CheckNotDisposed();
            ManagedThread.Name = value;
        }
    }

    /// <summary>
    /// Accessing the event ( protected member variable ) _evWaitExit.
    /// </summary>
    public ManualResetEvent EventWaitExit
    {
        get { return _evWaitExit; }
    }

    /// <summary>
    /// Returns true if there is a request to stop the thread, false otherwise.
    /// The method is virtual so the derived classes can overwrite it.
    /// In this implementation, it finds-out whether the ManualResetEvent EventWaitExit is set.
    /// </summary>
    public virtual bool IsStopRequest
    {
        get
        {
            bool result = false;
            if (null != EventWaitExit)
            { // If millisecondsTimeout is zero, the method does not block. 
              // It tests the state of the wait handle and returns immediately. 
                result = EventWaitExit.WaitOne(0);
            }
            return result;
        }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Initiates the execution of the thread. If the thread is already attached, it does nothing.
    /// If the thread is not attached and is currently running, it waits for the previous execution to finish first.
    /// </summary>
    public virtual void Start()
    {
        if (!IsAttached)
        {
            if (IsAlive)
            {
                Join();
            }
            EventWaitExit?.Reset();
            ManagedThread.Start();
            Debug.Assert(IsAlive);
        }
    }

    /// <summary>
    /// Blocks the calling thread until a thread terminates. Delegates the call to overloaded method
    /// WaitForStop(int milliseconds) with value Timeout.Infinite.
    /// </summary>
    /// <returns> True if the thread has terminated; false if the thread has not terminated. </returns>
    public bool Join()
    {
        return Join(Timeout.Infinite);
    }

    /// <summary>
    /// In case the thread is not attached, blocks the calling thread until this worker thread terminates 
    /// or the specified time elapses, while continuing to perform standard COM and SendMessage pumping.
    /// </summary>
    /// <param name="milliseconds">The number of milliseconds to wait for the thread to terminate. </param>
    /// <returns> 
    /// True if the thread has terminated; false if the thread has not terminated 
    /// after the amount of time specified by the <paramref name="milliseconds"/> timeout parameter has elapsed.
    /// </returns>
    /// <remarks>Throws InvalidOperationException if the method has been called by this worker thread itself.</remarks>
    public virtual bool Join(int milliseconds)
    {
        bool result = false;

        if (!IsAttached)
        {
            if (IsActive)
            {
                Debug.Fail("The worker thread cannot call Join for itself");
                throw new InvalidOperationException("The worker thread cannot call Join for itself");
            }

            try
            {
                EventWaitExit?.Set();
                if (IsAlive)
                {
                    result = ManagedThread.Join(milliseconds);
                }
                if (result)
                {
                    Debug.Assert(!IsAlive);
                }
            }
            catch (ObjectDisposedException) // handle the case EventWaitExit became disposed in the meantime
            {
                Forfeit();
            }
        }

        return result;
    }

    /// <summary>
    /// Interrupts worker thread, then it closes the wait event, too, and finally assigns the managed
    /// thread variable to null.
    /// </summary>
    public virtual void Abort()
    {
        if (!IsDisposed)
        {
            /* ManagedThread.Abort(stateInfo); Thread.Abort is obsolete */
            ManagedThread.Interrupt();
            CloseWaitEvent();
            Forfeit();
        }
    }

    /// <summary> The working function. Derived thread will overwrite it. </summary>
    protected virtual void WorkerFunction()
    {
        EventWaitExit?.WaitOne();
    }

    /// <summary>
    /// Closes the _evWaitExit object if not closed already
    /// </summary>
    protected virtual void CloseWaitEvent()
    {
        if (null != EventWaitExit)
        {
            EventWaitExit.Close();
            _evWaitExit = null;
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
        if (disposing && !IsDisposed)
        {
            // Make sure calls join only if it is not me, but some other thread who called Dispose
            if (!IsActive)
            {
                Join();
            }
            CloseWaitEvent();
            base.Dispose(disposing);
        }
        // no unmanaged resources here ...
    }
    #endregion     // Methods

    #region IDisposableEx members


    /// <summary>
    /// Returns true in case this WorkerThread has been disposed and no longer should be used.
    /// </summary>
    public bool IsDisposed
    {
        get { return Data == null; }
    }
    #endregion // IDisposableEx members
}

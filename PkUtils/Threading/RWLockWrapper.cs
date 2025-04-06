/***************************************************************************************************************
*
* FILE NAME:   .\Threading\RWLockWrapper.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains implementation of a wrapper around ReaderWriterLock.
*
**************************************************************************************************************/

// Ignore Spelling: Utils, rwlock
//
using System;
using System.Diagnostics;
using System.Threading;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.Threading;

/// <summary> The class RWLockWrapper is a wrapper around <see cref="ReaderWriterLock"/>.
/// The primary purpose of the wrapper is to return something IDisposable-derived from methods
/// <see cref="AcquireWriterLock(int)"/> and <see cref="AcquireReaderLock(int)"/>. <br/>
/// This way, lock will be safely released again if you create the lock by 'using' statement:
/// <code>
///  local variable initialized by constructing RWLockWrapper; or a member variable
/// RWLockWrapper wrapper;
/// using (IDisposable disp = wraper.AcquireWriterLock())
/// {
///    // executive code with writer lock granted
/// }
/// </code> </summary>
///
/// <remarks> In case the time-out interval expires before the lock request is granted,  methods
/// <see cref="AcquireWriterLock(int)"/> and <see cref="AcquireReaderLock(int)"/> return null. <br/>
/// You may want to create a branch in your code for that case, like following: <br/>
/// <code>
///  var wrapper = new RWLockWrapper();
///  int nTimeOut = 320;
/// 
///  using (IDisposable disp = wrapper.AcquireWriterLock(nTimeOut))
///  {
///    if (null == disp) {
///        // error handling ?
///    }
///    else {
///        // executive code with writer lock granted
///    }
///  }
/// </code> </remarks>
[CLSCompliant(true)]
public class RWLockWrapper
{
    #region Typedefs
    /// <summary>
    /// The base class for MyReadLockHelper and MyWriterLockHelper
    /// </summary>
    /* [CLSCompliant(true)]  not needed for private class */
    private abstract class BaseLockHelper : IDisposableEx
    {
        #region Fields
        /// <summary>
        /// The RWLockWrapper who created this BaseLockHelper instance. 
        /// The field _wrapper is assigned if and only if the lock has been truly acquired.
        /// </summary>
        private RWLockWrapper _wrapper;
        #endregion // Fields

        #region Constructor(s)

        /// <summary> Default constructor. </summary>
        /// <param name="wrapper"></param>
        protected BaseLockHelper(RWLockWrapper wrapper)
        {
            Debug.Assert(wrapper != null);
            if (wrapper.Synchronized)
            {
                _wrapper = wrapper;
            }
        }
        #endregion // Constructor(s)

        #region Properties
        protected RWLockWrapper Wrapper { get { return _wrapper; } }
        #endregion // Properties

        #region Methods
        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"> If true, is called by IDisposable.Dispose. 
        /// Otherwise it is called by finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _wrapper = null;
            }
        }
        #endregion // Methods

        #region IDisposableEx Members
        #region IDisposable Members

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue to prevent finalization code 
            // for this object from executing a second time.
            GC.SuppressFinalize(this);
        }
        #endregion // IDisposable Members

        public bool IsDisposed
        {
            get { return (null == Wrapper); }
        }
        #endregion // IDisposableEx Members
    }

    /// <summary>
    /// The helper class, whose instance will be returned from AcquireReaderLock.
    /// Note: For releasing the lock, this class has to be specially careful,
    /// because of following feature of ReaderWriterLock ( see the documentation ): <br/>
    /// " When an attempt is made to acquire a reader lock on a thread that has a writer lock, 
    ///   ReaderWriterLock does not grant the reader lock 
    ///   but instead increments the lock count on the writer lock."
    /// That's why there is a _bHasWriterLock field assigned.
    /// </summary>
    /* [CLSCompliant(true)]  not needed for private class */
    private sealed class MyReadLockHelper : BaseLockHelper
    {
        #region Fields
        /// <summary>
        /// The boolean _bHasWriterLock says whether _wrapper actually has the writer lock.
        /// </summary>
        private readonly bool _bHasWriterLock;
        #endregion // Fields

        #region Constructor(s)
        /// <summary>
        /// constructor. Acquires the Reader Lock by calling Wrapper.InternalAcquireReaderLock.
        /// </summary>
        /// <param name="wrapper"></param>
        /// <param name="millisecondsTimeout"> The time-out in milliseconds. </param>
        /// <remarks> The method InternalAcquireReaderLock throws ApplicationException
        /// in case the time-out interval expires. This is handled by the caller of 
        /// this MyReadLockHelper constructor.
        /// </remarks>
        internal MyReadLockHelper(RWLockWrapper wrapper, int millisecondsTimeout)
          : base(wrapper)
        {
            if (null != Wrapper)
            {
                Debug.Assert(Wrapper.Synchronized); // otherwise the property Wrapper should return null
                Wrapper.InternalAcquireReaderLock(millisecondsTimeout);
                _bHasWriterLock = Wrapper.IsWriterLockHeld;
            }
        }
        #endregion // Constructor(s)

        #region Methods
        /// <summary> The dispose will release the original lock. </summary>
        /// <remarks>
        /// It has to take into account whether it was the reader lock or the writer lock actually.
        /// </remarks>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                if (!_bHasWriterLock)
                    Wrapper.InternalReleaseReaderLock();
                else
                    Wrapper.InternalReleaseWriterLock();
            }
            base.Dispose(disposing);
        }
        #endregion // Methods
    }

    /// <summary>
    /// The helper class, whose instance will be returned from AcquireWriterLock
    /// </summary>
    /* [CLSCompliant(true)]  not needed for private class */
    private sealed class MyWriterLockHelper : BaseLockHelper
    {
        #region Fields
        #endregion // Fields

        #region Constructor(s)
        /// <summary>
        /// constructor. Acquires the Writer Lock by calling Wrapper.InternalAcquireWriterLock.
        /// </summary>
        /// <param name="wrapper"></param>
        /// <param name="millisecondsTimeout"> The time-out in milliseconds. </param>
        /// <remarks> The method InternalAcquireWriterLock throws ApplicationException
        /// in case the time-out interval expires. This is handled by the caller of 
        /// this MyWriterLockHelper constructor.
        /// </remarks>
        internal MyWriterLockHelper(RWLockWrapper wrapper, int millisecondsTimeout)
          : base(wrapper)
        {
            if (null != Wrapper)
            {
                Debug.Assert(Wrapper.Synchronized); // otherwise the property Wrapper should return null
                Wrapper.InternalAcquireWriterLock(millisecondsTimeout);
            }
        }
        #endregion // Constructor(s)

        #region Methods
        /// <summary> The dispose will release the original lock. </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                Wrapper.InternalReleaseWriterLock();
            }
            base.Dispose(disposing);
        }
        #endregion // Methods
    }
    #endregion // Typedefs

    #region Fields

    /// <summary>
    /// The ReaderWriterLock that is used for synchronizing. Is returned from property
    /// <see cref="RWLock"/>. </summary>
    protected readonly ReaderWriterLock _rwlock;
    #endregion // Fields

    #region Constructor

    /// <summary>
    /// Default argument-less constructor. Delegates to overloaded constructor with the argument bool bSynchronize = true.
    /// </summary>
    public RWLockWrapper()
      : this(true)
    {
    }

    /// <summary>
    /// Single-argument constructor. If <paramref name="bSynchronize"/> is true, creates the ReaderWriterLock that is used for synchronizing.
    /// </summary>
    /// <param name="bSynchronize">True if this wrapper should  support synchronization, false otherwise.</param>
    public RWLockWrapper(bool bSynchronize)
    {
        if (bSynchronize)
        {
            _rwlock = new ReaderWriterLock();
        }
    }
    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Returns true if the instance can perform synchronization, false otherwise.
    /// </summary>
    public bool Synchronized
    {
        get { return (RWLock != null); }
    }

    /// <summary>
    /// The return value indicates whether the current thread holds a reader lock.
    /// </summary>
    public bool IsReaderLockHeld
    {
        get
        {
            return (Synchronized && RWLock.IsReaderLockHeld);
        }
    }

    /// <summary>
    /// The return value indicates whether the current thread holds the writer lock.
    /// </summary>
    public bool IsWriterLockHeld
    {
        get
        {
            return (Synchronized && RWLock.IsWriterLockHeld);
        }
    }

    /// <summary>
    /// Returns the constructed ReaderWriterLock ( if there is any ). 
    /// </summary>
    protected ReaderWriterLock RWLock
    {
        get { return _rwlock; }
    }
    #endregion // Properties

    #region Public Methods

    /// <summary> Acquires a reader lock, using an System.Int32 value for the time-out. </summary>
    ///
    /// <remarks> If the time-out interval expires and the lock request has not been granted, the
    /// method implementation catches the ApplicationException thrown by the MyWriterLockHelper
    /// constructor, and returns null. </remarks>
    ///
    /// <exception cref="InvalidOperationException"> Thrown when the  this RWLockWrapper  has not been
    /// initialized as <see cref="Synchronized"/>. </exception>
    ///
    /// <param name="millisecondsTimeout"> The time-out in milliseconds. </param>
    ///
    /// <returns> A disposable object whose Dispose will release the lock. </returns>
    public virtual IDisposable AcquireReaderLock(int millisecondsTimeout)
    {
        IDisposable result = null;

        if (!this.Synchronized) throw new InvalidOperationException("This RWLockWrapper is not synchronized");
        try
        {
            result = new MyReadLockHelper(this, millisecondsTimeout);
        }
        catch (ApplicationException)
        {
        }
        return result;
    }

    /// <summary>
    /// Acquires a reader lock, using an infinite time-out.
    /// </summary>
    /// <returns> A disposable object whose Dispose will release the lock. </returns>
    public virtual IDisposable AcquireReaderLock()
    {
        return AcquireReaderLock(Timeout.Infinite);
    }

    /// <summary> Acquires a writer lock, using an System.Int32 value for the time-out. </summary>
    ///
    /// <remarks> If the time-out interval expires and the lock request has not been granted, the
    /// method implementation catches the ApplicationException thrown by the MyWriterLockHelper
    /// constructor, and returns null. </remarks>
    ///
    /// <exception cref="InvalidOperationException"> Thrown when the  this RWLockWrapper  has not been
    /// initialized as <see cref="Synchronized"/>. </exception>
    ///
    /// <param name="millisecondsTimeout"> The time-out in milliseconds. </param>
    ///
    /// <returns> A disposable object whose Dispose will release the lock. </returns>
    public virtual IDisposable AcquireWriterLock(int millisecondsTimeout)
    {
        IDisposable result = null;

        if (!this.Synchronized) throw new InvalidOperationException("This RWLockWrapper is not synchronized");
        try
        {
            result = new MyWriterLockHelper(this, millisecondsTimeout);
        }
        catch (ApplicationException)
        {
        }
        return result;
    }

    /// <summary>
    /// Acquires the writer lock, using an infinite time-out.
    /// </summary>
    /// <returns> A disposable object whose Dispose will release the lock. </returns>
    public virtual IDisposable AcquireWriterLock()
    {
        return AcquireWriterLock(Timeout.Infinite);
    }
    #endregion // Public Methods

    #region Protected methods

    /// <summary> Acquires a reader lock on a wrapped ReaderWriterLock, using an Int32 value for the time-out. </summary>
    ///
    /// <remarks> Throws <see cref="ApplicationException "/> in case the time-out interval expires
    /// before the lock request is granted.  </remarks>
    ///
    /// <param name="millisecondsTimeout"> The time-out in milliseconds. </param>
    protected void InternalAcquireReaderLock(int millisecondsTimeout)
    {
        Debug.Assert(Synchronized);
        RWLock.AcquireReaderLock(millisecondsTimeout);
    }

    /// <summary>
    /// Acquires a writer lock on a wrapped ReaderWriterLock, using an Int32 value for the time-out.
    /// </summary>
    /// 
    /// <remarks> Throws <see cref="ApplicationException "/> in case the time-out interval expires
    /// before the lock request is granted.  </remarks>
    /// 
    /// <param name="millisecondsTimeout"> The time-out in milliseconds. </param>
    protected void InternalAcquireWriterLock(int millisecondsTimeout)
    {
        Debug.Assert(Synchronized);
        RWLock.AcquireWriterLock(millisecondsTimeout);
    }

    /// <summary>
    /// Releases a reader lock on a wrapped ReaderWriterLock, if there was any.
    /// </summary>
    protected void InternalReleaseReaderLock()
    {
        if (IsReaderLockHeld)
        {
            RWLock.ReleaseReaderLock();
        }
    }

    /// <summary>
    /// Releases a writer lock on a wrapped ReaderWriterLock, if there was any.
    /// </summary>
    protected void InternalReleaseWriterLock()
    {
        if (IsWriterLockHeld)
        {
            RWLock.ReleaseWriterLock();
        }
    }
    #endregion // Protected methods
}

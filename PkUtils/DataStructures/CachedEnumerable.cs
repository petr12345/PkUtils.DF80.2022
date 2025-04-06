/***************************************************************************************************************
*
* FILE NAME:   .\DataStructures\CachedEnumerable.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains implementation of CachedEnumerable<T> class
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.DataStructures;

/// <summary> A generic class implementing ICachedEnumerable, which atop on generic IEnumerator builds
/// a cache of items parsed so far. An internally used (current) enumerator is created by virtual method
/// <see cref="RestartEnumeration"/>, assuming this call restarts parsing of wrapped data source.
/// </summary>
/// 
/// <remarks>
/// For similar idea how to build a wrapper around IEnumerator see for instance
/// <see href="http://codereview.stackexchange.com/questions/32857/implementing-peek-to-ienumerator-and-ienumeratort/">
/// Implementing Peek to IEnumerator</see>.
/// </remarks>
/// 
/// <typeparam name="T"> Generic type parameter - the type of objects to enumerate. </typeparam>
public class CachedEnumerable<T> : NotifyPropertyChanged, ICachedEnumerable<T>, IDisposableEx
{
    #region Typedefs

    /// <summary>
    /// A possible implementation of enumerator which is returned by <see cref="CachedEnumerable{T}"/>.
    /// 
    /// The idea is, the class <see cref="CachedEnumerable{T}"/> holds data common for all iterators, while
    /// the instance of <see cref="CachedEnumerator"/> holds data specific for particular iterator, and
    /// keeps a back reference to <see cref="CachedEnumerable{T}"/>.
    /// </summary>
    /// <remarks>
    /// For analogical ( wrapper-like ) implementation of IEnumerator, see
    /// <see href="http://www.csharphelp.com/2006/05/building-your-own-c-enumerator-to-use-with-the-foreach-construct/">
    /// Building Your Own C# Enumerator To Use With The foreach Construct</see>.<br/>
    /// </remarks>
    protected class CachedEnumerator : IPeekAbleEnumerator<T>, IDisposableEx
    {
        #region Fields

        /// <summary> Refers to parent class with data commonly shared by all MyCacheEnumerator(s). </summary>
        private CachedEnumerable<T> _parser;

        /// <summary> The parser cache version at the moment this enumerator has been created. </summary>
        private readonly long _parserCacheVersion;

        /// <summary> The current position in the list (cache) of items found so far. </summary>
        private int _currentPosition = _initialSeekPos;

        /// <summary> The initial seeking position. </summary>
        private const int _initialSeekPos = -1;
        #endregion // Fields

        #region Constructor(s)

        /// <summary> Constructor accepting single argument. </summary>
        /// <param name="parser"> Parser which keeps the data commonly shared by all MyCacheEnumerator(s). </param>
        protected internal CachedEnumerator(CachedEnumerable<T> parser)
        {
            ArgumentNullException.ThrowIfNull(parser);
            _parser = parser;
            _parserCacheVersion = parser.CacheVersion;
        }

        /// <summary> Specialized copy-like constructor, for use only by this class and derived class. </summary>
        ///
        /// <param name="en"> The enumerator to copy. </param>
        protected CachedEnumerator(CachedEnumerator en)
            : this(en.Parser)
        {
            _currentPosition = en.CurrentPosition;
        }
        #endregion // Constructor(s)

        #region Properties
        #region Protected Properties

        /// <summary> Returns a reference to CachedEnumerable, who created this object.</summary>
        protected internal CachedEnumerable<T> Parser
        {
            get { return _parser; }
        }

        /// <summary> The parser cache version at the moment this enumerator has been created. </summary>
        protected internal long ParserCacheVersion
        {
            get { return _parserCacheVersion; }
        }

        /// <summary> The current position in the list (cache) of items found so far. </summary>
        protected internal int CurrentPosition
        {
            get { return _currentPosition; }
            set { _currentPosition = value; }
        }

        /// <summary> Gets a value indicating whether this iterator has valid position. </summary>
        /// <remarks>
        /// Here it is just a basic check for positions prior to <see cref="_initialSeekPos"/>; which may be overwritten.
        /// </remarks>
        /// <value> true if this object is valid position, false if not. </value>
        /// <seealso cref="HasMoved"/>
        protected internal bool IsValidPosition
        {
            get { return CurrentPosition >= _initialSeekPos; }
        }

        /// <summary> Gets a value indicating whether this enumerator has moved at all from its initial 
        /// seek position. </summary>
        /// <seealso cref="IsValidPosition"/>
        protected internal bool HasMoved
        {
            get { return CurrentPosition > _initialSeekPos; }
        }
        #endregion //  Protected Properties
        #endregion // Properties

        #region Methods
        #region Protected Methods

        /// <summary> Enlarges the parser buffer for purpose of peek done by this enumerator. <br/>
        /// Implementation helper, called by <see cref="CanPeek"/> and <see cref="Peek"/>.
        /// It is assumed these methods did all needed enumerator checking. </summary>
        /// <returns> true if it succeeds, false if it fails. </returns>
        protected bool EnlargeParserBufferForPeek()
        {
            // For instance, if enumerator on a current physical position 0 would like to peek a position 1, 
            // it means it needs buffer filled at least with 2 items. Tricky, isn't it.
            int nNeed = this.CurrentPosition + 2;
            return (Parser.FillBuffer(nNeed) >= nNeed);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the method
        /// has been called directly or indirectly by a user's code. Managed and unmanaged resources can be
        /// disposed. If disposing equals false, the method has been called by the runtime from inside the
        /// finalizer and you should not reference other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by
        ///  finalizer. </param>
        protected virtual void Dispose(bool disposing)
        {
            _parser = null;
        }
        #endregion // Protected Methods
        #endregion // Methods

        #region IPeekAbleEnumerator<T> Members
        #region IEnumerator<T> Members
        #region IEnumerator Members

        /// <summary> Gets the current element in the enumerated collection. </summary>
        object IEnumerator.Current
        {
            get { return (this as IEnumerator<T>).Current; }
        }

        /// <summary> Advances the enumerator to the next element of the <see cref="Parser"/>. </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="Parser"/> was modified after 
        /// the enumerator was created.</exception>
        /// <exception cref="ObjectDisposedException"> Thrown when this object has been disposed. </exception>
        /// 
        /// <returns> Returns true if we can move next, false if we can't. </returns>
        public bool MoveNext()
        {
            this.CheckNotDisposed();
            return Parser.DoMoveNext(this);
        }

        /// <summary> Sets the enumerator to its initial position, which is before the first element in 
        /// the <see cref="Parser"/>. </summary>
        /// <exception cref="ObjectDisposedException"> Thrown when this object has been disposed. </exception>
        public void Reset()
        {
            this.CheckNotDisposed();
            _currentPosition = _initialSeekPos;
        }
        #endregion // IEnumerator Members

        /// <summary> Returns the current element in the enumerated collection. </summary>
        /// <value> The current element. </value>
        public T Current
        {
            get
            {
                this.CheckNotDisposed();
                return Parser.DoGetCurrent(this);
            }
        }
        #endregion // IEnumerator<T> Members

        /// <inheritdoc/>
        public bool CanPeek
        {
            get
            {
                this.CheckNotDisposed();
                Parser.CheckEnumerator(this);
                return this.EnlargeParserBufferForPeek();
            }
        }

        /// <inheritdoc/>
        object IPeekAbleEnumerator.Peek
        {
            get { return (this as IPeekAbleEnumerator<T>).Peek; }
        }

        /// <inheritdoc/>
        public T Peek
        {
            get
            {
                this.CheckNotDisposed();
                Parser.CheckEnumerator(this);
                if (!this.EnlargeParserBufferForPeek())
                {
                    string errorMessage = $"Cannot peek beyond current position {CurrentPosition}";
                    throw new InvalidOperationException(errorMessage);
                }
                else
                {
                    using CachedEnumerator copy = new(this);
                    copy.MoveNext();
                    return copy.Current;
                }
            }
        }
        #endregion // IPeekAbleEnumerator<T> Members

        #region IDisposableEx Members
        #region IDisposable Members

        /// <summary>Releases all resources used by the object.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion // IDisposable Members

        /// <summary> Returns true if is disposed, false otherwise.</summary>
        public bool IsDisposed
        {
            get { return (_parser == null); }
        }
        #endregion // IDisposableEx Members
    }
    #endregion // Typedefs

    #region Private Fields

    /// <summary> A current status of this object. </summary>
    /// <remarks> Should change thread-safely only inside write lock.</remarks>
    private ParseStatus _status;

    /// <summary>
    /// The cache version, updated to another unique value with each <see cref="DoResetCache"/> call.
    /// Should be changed thread-safely only inside write lock.
    /// </summary>
    private long _cacheVersion;

    /// <summary> The parsed data source, provided by constructor. </summary>
    /// <remarks> The field is used inside <see cref="DataSource"/> virtual property, which is called by 
    /// <see cref="RestartEnumeration"/> virtual method.
    /// It may be null, if a derived class needs more complicated implementation of DataSource or 
    /// RestartEnumeration, and for instance could not use always the same field provided by constructor.
    /// </remarks>
    private IEnumerable<T> _dataSource;

    /// <summary> The enumerator used with current parsing. </summary>
    /// <remarks> Should change thread-safely only inside write lock.</remarks>
    private IEnumerator<T> _parserEnumerator;

    /// <summary> Backing field of property <see cref="CachedList"/>, reset by <see cref="ResetCache"/>. </summary>
    /// <remarks> Should be changed thread-safely only inside write lock.</remarks>
    private IList<T> _cachedList;

    /// <summary> A slim lock, used for locking/unlocking the cache. </summary>
    private readonly ReaderWriterLockSlim _slimLock = new(LockRecursionPolicy.SupportsRecursion);

    /// <summary>
    /// A value which indicates the disposable state. 0 indicates undisposed, 1 indicates disposing or disposed.
    /// </summary>
    private int _disposableState = StateUndisposed;

    /// <summary> The constant representing the not-disposed state. </summary>
    private const int StateUndisposed = 0;

    /// <summary> The constant representing the disposed state. </summary>
    private const int StateDisposed = 1;
    #endregion // Private Fields

    #region Constructor(s)

    /// <summary> Default argument-less constructor. </summary>
    public CachedEnumerable()
        : this(null)
    { }

    /// <summary> One-argument constructor accepting (optional) data source. </summary>
    /// <param name="dataSource"> The provided parsed data source ( if any ). May equal to null. </param>
    public CachedEnumerable(IEnumerable<T> dataSource)
    {
        DataSource = dataSource;
        ResetCacheFields();
        // should call AssertValid rather than ValidateMe (which assumes a lock presence)
        AssertValid();
    }
    #endregion // Constructor(s)

    #region Properties

    #region Public Properties

    /// <summary> Returns true if parsing has completed, and all items from data source are cached. 
    /// The internal buffer is completely filled-in ( till someone calls <see cref="ResetCache"/> ).
    /// </summary>
    /// <seealso cref="ParseHasEnded"/>
    public bool ParseHasCompleted
    {
        get { return (Status == ParseStatus.ParsedOk); }
    }

    /// <summary> Returns true if parsing either has completed, or has ended prematurely. </summary>
    /// <seealso cref="ParseHasCompleted"/>
    public bool ParseHasEnded
    {
        get { return (Status == ParseStatus.ParsedOk) || (Status == ParseStatus.ParsePrematureEnd); }
    }
    #endregion // Public Properties

    #region Protected Properties

    /// <summary> A slim lock, used for locking/unlocking the cache. </summary>
    protected ReaderWriterLockSlim SlimLock
    {
        get { return _slimLock; }
    }

    /// <summary> Gets a value indicating whether the cache buffer is allocated. </summary>
    /// <seealso cref="AllocateCache"/>
    protected bool IsCacheAllocated
    {
        get { return (CachedList != null); }
    }

    /// <summary> Gets the value of cache version, modified (incremented) with each cache resetting. </summary>
    /// <remarks> This getter should remain virtual as long as related field remains private.</remarks>
    /// <seealso cref="UpdateCacheVersion"/>
    protected virtual long CacheVersion
    {
        get { return _cacheVersion; }
    }

    /// <summary> Returns the parsed data source used inside <see cref="RestartEnumeration"/>. </summary>
    /// <remarks>
    /// Can be overwritten in a derived class, in case such class does not want to rely on single field 
    /// value being initialized by constructor and stored in _dataSource.
    /// </remarks>
    protected virtual IEnumerable<T> DataSource
    {
        get { return _dataSource; }
        set { _dataSource = value; }
    }

    /// <summary> The enumerator used with current parsing. </summary>
    /// <remarks>
    /// Can be overwritten in a derived class, in case such class does not want to rely on single field 
    /// value being initialized by AllocateCache and stored in _parserEnumerator private field.
    /// </remarks>
    /// <seealso cref="AllocateCache "/>
    protected virtual IEnumerator<T> ParserEnumerator
    {
        get { return _parserEnumerator; }
        set { _parserEnumerator = value; }
    }

    /// <summary> Cached list of found items. Reset by <see cref="ResetCache"/>. </summary>
    protected virtual IList<T> CachedList
    {
        get { return _cachedList; }
        set { _cachedList = value; }
    }
    #endregion // Protected Properties
    #endregion // Properties

    #region Methods
    #region Public Methods

    /// <summary>
    /// Conditionally-compiled Method validating the object instance. It should NOT be virtual.
    /// </summary>
    /// <exception cref="ObjectDisposedException"> Thrown when this object has been disposed. </exception>
    [Conditional("DEBUG")]
    public void AssertValid()
    {
        // Check before attempting to acquire a lock.
        // In case there is a second thread executing Dispose, but it has not disposed the SlimLock yet, 
        // this check guarantees that disposing thread can just wait for the other ( third, fourth ..) threads
        // already waiting for lock to acquire and release it, and there will be no new upcoming threads waiting.
        this.CheckNotDisposed();

        try
        {
            SlimLock.EnterReadLock();
        }
        catch (ObjectDisposedException)
        {
            // thrown if lock has been disposed in the meantime
            throw new ObjectDisposedException(GetType().FullName);
        }
        try
        {
            // Another necessary check is here (handling the case other thread has called Dispose in the meantime)
            this.CheckNotDisposed();
            ValidateMe();
        }
        finally
        {
            SlimLock.ExitReadLock();
        }
    }
    #endregion // Public Methods

    #region Protected Methods

    /// <summary> Change the current status. </summary>
    /// <param name="newStatus"> The new status. </param>
    /// <seealso cref="Status"/>
    protected virtual void ChangeStatus(ParseStatus newStatus)
    {
        this.Status = newStatus;
    }

    /// <summary> Begins a new enumeration, by creating internally used (current) enumerator. 
    /// Derived class may override that method.
    /// </summary>
    /// 
    /// <remarks> You should not call this method on your own, it is always called automatically 
    /// by <see cref="AllocateCache"/> when needed. </remarks>
    /// 
    /// <returns> An IEnumerator&lt;T&gt; </returns>
    protected virtual IEnumerator<T> RestartEnumeration()
    {
        if (DataSource == null)
        {
            throw new InvalidOperationException("DataSource is null. Initialize it or overwrite RestartEnumeration.");
        }
        return DataSource.GetEnumerator();
    }

    /// <summary> The criteria of premature parse ending. </summary>
    /// <returns> true if should prematurely end parsing, false if not. </returns>
    protected virtual bool PrematureParseEndCriteria()
    {
        return false;
    }

    /// <summary> Gets current item for given enumerator. </summary>
    /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
    /// <param name="en"> The enumerator. </param>
    /// <returns> An item in the enumerated collection. </returns>
    protected virtual T DoGetCurrent(CachedEnumerator en)
    {
        // Check before attempting to acquire a lock (more details in the first comment "Check before ...")
        this.CheckNotDisposed();

        int nPosition = en.CurrentPosition;
        T result = default;

        try
        {
            SlimLock.EnterReadLock();
        }
        catch (ObjectDisposedException)
        {
            // thrown if lock has been disposed in the meantime
            throw new ObjectDisposedException(GetType().FullName);
        }
        try
        {
            // Another necessary check is here (handling the case other thread has called Dispose in the meantime)
            this.CheckNotDisposed();
            ValidateMe();
            CheckEnumerator(en);

            if ((this.Status == ParseStatus.ParseNotInitialized) || !en.HasMoved)
                throw new InvalidOperationException("Enumeration has not started. Call MoveNext first.");
            else if (nPosition >= CachedItemsCount)
                throw new InvalidOperationException("Invalid enumerator trying to seek beyond the end of cache.");
            else
                result = CachedList[nPosition];

            ValidateMe();
        }
        finally
        {
            SlimLock.ExitReadLock();
        }

        return result;
    }

    /// <summary> Advances the given enumerator <paramref name="en"/> to the next element of 
    /// the iterated collection. </summary>
    /// <remarks> If overriding DoMoveNext method in a derived class, you are obliged to call the base class
    /// (this class) implementation. </remarks>
    /// 
    /// <param name="en"> The enumerator. </param>
    /// <returns> True if the enumerator was successfully advanced to the next element; 
    ///  false if the enumerator has passed the end of the collection. 
    /// </returns>
    protected virtual bool DoMoveNext(CachedEnumerator en)
    {
        // Check before attempting to acquire a lock (more details in the first comment "Check before ...")
        this.CheckNotDisposed();

        int nOldPosition = en.CurrentPosition;
        int nNewPosition = nOldPosition + 1;
        bool bRes = false;

        try
        {
            SlimLock.EnterUpgradeableReadLock();
        }
        catch (ObjectDisposedException)
        {
            // thrown if lock has been disposed in the meantime
            throw new ObjectDisposedException(GetType().FullName);
        }
        try
        {
            // Another necessary check is here (handling the case other thread has called Dispose in the meantime)
            this.CheckNotDisposed();
            ValidateMe();
            CheckEnumerator(en);

            if (bRes = (nNewPosition < CachedItemsCount))
            {
                Debug.Assert(this.Status != ParseStatus.ParseNotInitialized);
                en.CurrentPosition = nNewPosition;
            }
            else if (ParseHasEnded)
            {
                /* bRes = false;  already is */
            }
            else if (bRes = DoEnlargeBuffer(nNewPosition + 1))
            {
                en.CurrentPosition = nNewPosition;
            }

            ValidateMe();
        }
        finally
        {
            SlimLock.ExitUpgradeableReadLock();
        }

        return bRes;
    }

    /// <summary> Allocates the internal cache buffer and assigns the status to ParseStatus.Parsing,
    /// if both of it has not happened yet. 
    /// </summary>
    /// 
    /// <remarks> 
    ///  Do not call this method on your own. <br/>
    ///  While it is protected for possibility to override it, it is always called automatically in a work-flow
    ///  MyCacheEnumerator.MoveNext() => CachedEnumerable.DoMoveNext() => 
    ///  CachedEnumerable.DoEnlargeBuffer() => CachedEnumerable.AllocateCache().
    ///  <br/>
    ///  If overriding in derived class, either you have to call base ( CachedEnumerable ) implementation
    ///  too, or to initialize related fields ( _TCachedList, _parserEnumerator, Status property )
    ///  on your own.
    /// </remarks>
    /// <seealso cref="IsCacheAllocated"/>
    /// <seealso cref="DoResetCache"/>
    protected virtual void AllocateCache()
    {
        // Check before attempting to acquire a lock (more details in the first comment "Check before ...")
        this.CheckNotDisposed();

        try
        {
            SlimLock.EnterWriteLock();
        }
        catch (ObjectDisposedException)
        {
            // thrown if lock has been disposed in the meantime
            throw new ObjectDisposedException(GetType().FullName);
        }
        try
        {
            // Another necessary check is here (handling the case other thread has called Dispose in the meantime)
            this.CheckNotDisposed();
            ValidateMe();
            if (!IsCacheAllocated)
            {
                CachedList = [];
                // It is too late to modify cache version here. 
                // It must be done inside ResetCache, since the first enumerator ( with cache version stored )
                // is returned even before the buffer gets allocated.
                /* UpdateCacheVersion(); */
                ParserEnumerator = RestartEnumeration();
                ChangeStatus(ParseStatus.Parsing);
                ValidateMe();
            }
        }
        finally
        {
            SlimLock.ExitWriteLock();
        }
    }


    /// <summary> Releases both the managed and unmanaged resources hold by this instance. </summary>
    ///
    /// <remarks>
    /// This method is thread safe. Anyway, you should dispose this object from one thread only
    /// ( preferably from the thread who has created it ).
    /// </remarks>
    ///
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>

    protected virtual void Dispose(bool disposing)
    {
        // Attempt to move the disposable state from StateUndisposed to StateDisposed. 
        // If successful, we can be assured that this thread is the first thread to do so,
        // and can safely dispose of the object.
        // 
        if (Interlocked.CompareExchange(ref _disposableState, StateDisposed, StateUndisposed) == StateUndisposed)
        {
            if (disposing)
            {
                SlimLock.EnterWriteLock();
                try
                {
                    // Reset cache fields when no-one uses them
                    ResetCacheFields();
                    // This thread cannot simply call SlimLock.Dispose() from here, because possibly other threads 
                    // are waiting for that. In such scenario, the Dispose here just raise SynchronizationLockException, 
                    // without actually disposing anything.
                }
                finally
                {
                    SlimLock.ExitWriteLock();
                }
                // Now, after releasing SlimLock write lock here, other thread(s) could complete their waiting for it.
                // They must check the IsDisposed property after they acquire that lock, 
                // and throw ObjectDisposedException if that's true.

                // Complete by actual disposing the SlimLock, till there are no threads waiting for SlimLock.
                // It is guaranteed there will be no new threads getting into that waiting state, 
                // as they find-out this._DisposableState == StateDisposed before attempting to wait for SlimLock.
                // 
                bool bDone = false;
                do
                {
                    try
                    {
                        SlimLock.Dispose();
                        bDone = true;
                    }
                    catch (SynchronizationLockException)
                    {
                        // This exception is raised if either SlimLock has
                        // (this.IsReadLockHeld || this.IsUpgradeableReadLockHeld) || this.IsWriteLockHeld)
                        // or if
                        // (this.WaitingReadCount > 0) || (this.WaitingUpgradeCount > 0)) || (this.WaitingWriteCount > 0))
                        // 
                        // Whatever is the case, call Sleep(1) to force the context switch. Do NOT call Sleep(0),
                        // since loop that calls just Sleep(0) burns 100% CPU cycles - see more on
                        // http://stackoverflow.com/questions/3257708/thread-sleep0-what-is-the-normal-behavior
                        Thread.Sleep(1);
                    }
                } while (!bDone);
            }
        }
    }

    /// <summary>
    /// A method performing the actual cache reset, and updating cache version #. 
    /// It is called by <see cref="ResetCache"/>
    /// </summary>
    /// <seealso cref="UpdateCacheVersion"/>
    protected virtual void DoResetCache()
    {
        // Check before attempting to acquire a lock (more details in the first comment "Check before ...")
        this.CheckNotDisposed();

        try
        {
            SlimLock.EnterWriteLock();
        }
        catch (ObjectDisposedException)
        {
            // thrown if lock has been disposed in the meantime
            throw new ObjectDisposedException(GetType().FullName);
        }
        try
        {
            // Another necessary check is here (handling the case other thread has called Dispose in the meantime)
            this.CheckNotDisposed();
            ResetCacheFields();
            // generate cache version that remains valid till another resetting
            UpdateCacheVersion();
        }
        finally
        {
            SlimLock.ExitWriteLock();
        }
    }

    /// <summary> Auxiliary method called by <see cref="DoResetCache"/>
    /// Generates new cache version ( could be interpreted as cache id), that remains valid till 
    /// another cache resetting call. </summary>
    /// <seealso cref="DoResetCache"/>
    protected virtual void UpdateCacheVersion()
    {
        Debug.Assert(SlimLock.IsWriteLockHeld);
        Interlocked.Increment(ref _cacheVersion);
    }

    /// <summary>
    /// Check of enumerator <paramref name="en"/>, throws exception in case of problem.
    /// Calls <see cref="BasicCheckEnumerator"/> first, and if that check succeeds, 
    /// verifies additionally this.CacheVersion against ParserCacheVersion in <paramref name="en"/>.
    /// </summary>
    /// 
    /// <remarks> Note that all enumerators with obsolete cache version are considered invalid,
    /// regardless what their buffer position is.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="en"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when <paramref name="en"/> has unsupported or illegal
    ///  value. </exception>
    /// <param name="en"> The enumerator to be checked. </param>
    protected void CheckEnumerator(CachedEnumerator en)
    {
        BasicCheckEnumerator(en);
        if (this.CacheVersion != en.ParserCacheVersion)
        {
            StringBuilder sbErr = new("The cache has been reset since enumerator creation. ");

            sbErr.AppendFormat(CultureInfo.InvariantCulture,
              "The current cache version '{0}' cannot support enumerator with ParserCacheVersion value '{1}'",
              this.CacheVersion, en.ParserCacheVersion);
            throw new ArgumentException(sbErr.ToString(), nameof(en));
        }
    }

    /// <summary>
    /// A very basic check of enumerator <paramref name="en"/>, throws exception in case of problem.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="en"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when <paramref name="en"/> has unsupported or illegal
    ///  value. </exception>
    /// <param name="en"> The enumerator. </param>
    protected static void BasicCheckEnumerator(CachedEnumerator en)
    {
        ArgumentNullException.ThrowIfNull(en);

        if (!en.IsValidPosition)
        {
            string errorMessage = string.Format(CultureInfo.InvariantCulture, "Invalid enumerator '{0}'", en);
            throw new ArgumentException(errorMessage, nameof(en));
        }
    }

    /// <summary>
    /// Method implementing validation of an instance of this type.
    /// It is assumed the caller of it has acquired at least the reader lock (it could be a writer lock, too).
    /// An exception of that rule is the constructor.
    /// </summary>
    [Conditional("DEBUG")]
    protected void ValidateMe()
    {
        Debug.Assert(!IsDisposed);
        Debug.Assert(Enum.IsDefined(typeof(ParseStatus), this.Status));
        Debug.Assert(SlimLock.IsReadLockHeld || SlimLock.IsUpgradeableReadLockHeld || SlimLock.IsWriteLockHeld);

        switch (this.Status)
        {
            case ParseStatus.ParseNotInitialized:
                Debug.Assert(ParserEnumerator == null);
                Debug.Assert(!IsCacheAllocated);
                break;

            // complete individual switch is too much code, now there is need to handle more distinctively...
            default:
                Debug.Assert(ParserEnumerator != null);
                Debug.Assert(IsCacheAllocated);
                break;
        }
    }
    #endregion // Protected Methods

    #region Private Methods

    /// <summary>
    /// Resets all cache-related variables (except updating cache version). Auxiliary method called by
    /// constructor, <see cref="DoResetCache"/> and <see cref="Dispose(bool)"/>.
    /// </summary>
    private void ResetCacheFields()
    {
        CachedList = null;
        ParserEnumerator = null;
        ChangeStatus(ParseStatus.ParseNotInitialized);
    }

    /// <summary>
    /// Attempts to enlarge the internal buffer <see cref="CachedList"/>, by seeking with
    /// <see cref="ParserEnumerator"/> and collecting encountered items.
    /// </summary>
    /// <remarks>
    /// Note this method is the only one that does NOT guard ObjectDisposedException by putting try /catch around 
    /// the call SlimLock.Enter'Something'. It is due to the fact the method is private and the responsibility 
    /// of the caller is to do needed checking, including acquiring Upgradeable Read Lock.
    /// ( To be exact, other similar case is <see cref="Dispose(bool)"/>, handling this issue differently).
    /// </remarks>
    /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
    /// <param name="nNewLength"> Required new length of the internal buffer. </param>
    /// <returns>
    /// true if it succeeded and could enlarge internal buffer to given size, false if it fails.
    /// </returns>
    private bool DoEnlargeBuffer(int nNewLength)
    {
        string errorMessage;
        bool bResult = false;

        Debug.Assert(nNewLength >= 0);
        Debug.Assert(SlimLock.IsUpgradeableReadLockHeld);

        SlimLock.EnterWriteLock();
        try
        {
            /* CheckNotDisposed(); - not needed, the caller is assumed to do that after acquiring its read lock */
            ValidateMe();

            switch (this.Status)
            {
                case ParseStatus.ParseNotInitialized:
                    AllocateCache();
                    Debug.Assert(this.Status == ParseStatus.Parsing);
                    goto case ParseStatus.Parsing;

                case ParseStatus.Parsing:
                    if (!(bResult = (CachedItemsCount >= nNewLength)))
                    {
                        while ((this.Status == ParseStatus.Parsing) && !bResult)
                        {
                            if (PrematureParseEndCriteria())
                            {
                                ChangeStatus(ParseStatus.ParsePrematureEnd);
                            }
                            else if (ParserEnumerator.MoveNext())
                            {
                                CachedList.Add(ParserEnumerator.Current);
                                bResult = (CachedItemsCount >= nNewLength);
                            }
                            else
                            {
                                ChangeStatus(ParseStatus.ParsedOk);
                            }
                        }
                    }
                    break;

                case ParseStatus.ParsedOk:
                    bResult = (CachedItemsCount >= nNewLength);
                    break;

                default:
                    errorMessage = string.Format(CultureInfo.InvariantCulture,
                      "Cannot be called with current status '{0}'.", Status);
                    throw new InvalidOperationException(errorMessage);
            }

            ValidateMe();
        }
        finally
        {
            SlimLock.ExitWriteLock();
        }

        return bResult;
    }
    #endregion // Private Methods
    #endregion // Methods

    #region ICachedEnumerable<T> Members
    #region IPeekAbleEnumerable<T> Members
    #region IEnumerable<T> Members
    #region IEnumerable Members

    /// <summary> Gets the enumerator. </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return (this as IEnumerable<T>).GetEnumerator();
    }
    #endregion // IEnumerable Members

    /// <summary> Gets the enumerator. </summary>
    /// <returns> An IEnumerator&lt;T&gt; </returns>
    public IEnumerator<T> GetEnumerator()
    {
        return GetPeekAbleEnumerator();
    }
    #endregion // IEnumerable<T> Members
    #region IPeekAbleEnumerable Members

    /// <summary> Gets the non-generic peek-able enumerator. </summary>
    /// <returns> The peek-able enumerator. </returns>
    IPeekAbleEnumerator IPeekAbleEnumerable.GetPeekAbleEnumerator()
    {
        return (this as IPeekAbleEnumerable<T>).GetPeekAbleEnumerator();
    }
    #endregion // IPeekAbleEnumerable Members

    /// <summary> Gets the peek-able enumerator. </summary>
    /// <returns> The peek-able enumerator. </returns>
    public virtual IPeekAbleEnumerator<T> GetPeekAbleEnumerator()
    {
        this.CheckNotDisposed();
        return new CachedEnumerator(this);
    }
    #endregion // IPeekAbleEnumerable<T> Members

    /// <summary> Gets or sets the parse status. </summary>
    /// <remarks>
    /// For changing the status in derived class code, the virtual method <see cref="ChangeStatus"/>
    /// should be called.
    /// </remarks>
    /// <seealso cref="ChangeStatus"/>
    public ParseStatus Status
    {
        get { return _status; }
        private set { SetField(ref _status, value, nameof(Status)); }
    }

    /// <summary> Gets the number of currently cached items. </summary>
    public int CachedItemsCount
    {
        get { return IsCacheAllocated ? CachedList.Count : 0; }
    }

    /// <summary> Resets the cache of found items. </summary>
    public void ResetCache()
    {
        DoResetCache();
    }

    /// <summary> Attempts to fill the internal buffer to make it increase size to <paramref name="newLength"/>.
    /// If the buffer already has such size or even bigger, nothing is changed.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="newLength"/> is negative. 
    /// </exception>
    ///
    /// <param name="newLength"> Required new length of the internal buffer. 
    /// The default value is int.MaxValue</param>
    /// <returns> An new size of the buffer. </returns>
    public int FillBuffer(int newLength = int.MaxValue)
    {
        int nResult = 0;

        if (newLength < 0)
        {
            string errorMessage = string.Format(CultureInfo.InvariantCulture,
              "Argument value cannot be negative, current value is {0}.", newLength);
            throw new ArgumentOutOfRangeException(nameof(newLength), errorMessage);
        }

        // Check before attempting to acquire a lock (more details in the first comment "Check before ...")
        this.CheckNotDisposed();
        try
        {
            SlimLock.EnterUpgradeableReadLock();
        }
        catch (ObjectDisposedException)
        {
            // thrown if lock has been disposed in the meantime
            throw new ObjectDisposedException(GetType().FullName);
        }
        try
        {
            // Another necessary check is here (handling the case other thread has called Dispose in the meantime)
            this.CheckNotDisposed();

            if ((nResult = this.CachedItemsCount) < newLength)
            {
                DoEnlargeBuffer(newLength);
                nResult = this.CachedItemsCount;
            }
        }
        finally
        {
            SlimLock.ExitUpgradeableReadLock();
        }

        return nResult;
    }

    /// <summary>
    /// Resumes parsing for case the current <see cref="Status"/> is ParseStatus.ParsePrematureEnd.<br/>
    /// </summary>
    /// <remarks>
    /// It is assumed that parsing could be resumed if since the time the cache ended-up in premature end
    /// state, some internal data related to <see cref="PrematureParseEndCriteria"/> has changed, and now
    /// that method could return false, thus permitting further parsing. <br/>
    /// 
    /// The method simply returns true for case the current status is ParseStatus.ParseNotInitialized or
    /// ParseStatus.Parsing. 
    /// For case ParseStatus.ParsedOk it simply returns false.
    /// </remarks>
    /// <returns> true if it succeeds, false if it fails. </returns>
    public bool ResumeParsing()
    {
        bool bRes = false;

        // Check before attempting to acquire a lock (more details in the first comment "Check before ...")
        this.CheckNotDisposed();
        try
        {
            SlimLock.EnterWriteLock();
        }
        catch (ObjectDisposedException)
        {
            // thrown if lock has been disposed in the meantime
            throw new ObjectDisposedException(GetType().FullName);
        }
        try
        {
            // Another necessary check is here (handling the case other thread has called Dispose in the meantime)
            this.CheckNotDisposed();

            switch (this.Status)
            {
                case ParseStatus.ParseNotInitialized:
                case ParseStatus.Parsing:
                    bRes = true;
                    break;

                case ParseStatus.ParsePrematureEnd:
                    if (!PrematureParseEndCriteria())
                    {
                        ChangeStatus(ParseStatus.Parsing);
                        bRes = true;
                    }
                    break;
            }
        }
        finally
        {
            SlimLock.ExitWriteLock();
        }

        return bRes;
    }
    #endregion // ICachedEnumerable<T> Members

    #region IDisposableEx Members
    #region IDisposable Members

    /// <summary>Releases all resources used by the object.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposable Members

    /// <summary> Returns true if is disposed, false otherwise.</summary>
    public bool IsDisposed
    {
        get
        {
            return (Thread.VolatileRead(ref this._disposableState) == StateDisposed);
        }
    }
    #endregion // IDisposableEx Members
}
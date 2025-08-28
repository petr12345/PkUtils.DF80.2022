// Ignore Spelling: Utc

// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using static System.FormattableString;

#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.PkUtils.DataStructures;

/// <summary> A size-limited cache. </summary>
///
/// <typeparam name="TKey"> Type of the key. </typeparam>
/// <typeparam name="TValue">   Type of the value. </typeparam>
public class SizeLimitedCache<TKey, TValue> : ISizeLimitedCache<TKey, TValue>
{
    #region Typedefs

    /// <summary> An entry that is used as value in internal dictionary. </summary>
    protected sealed class CachedEntry
    {
        private DateTime _utcAccessStamp;
        private readonly TValue _cachedValue;

        /// <summary> The only constructor. </summary>
        /// <param name="cachedValue">  The cached value. </param>
        public CachedEntry(TValue cachedValue)
        {
            _cachedValue = cachedValue;
            MarkAccess();
        }

        /// <summary> Gets the URC Date/Time of last access stamp. </summary>
        public DateTime UtcAccessStamp { get => _utcAccessStamp; }

        /// <summary> Gets the cached value. </summary>
        public TValue CachedValue { get => _cachedValue; }

        /// <summary> Mark the time of lass access. </summary>
        /// <returns> This CachedEntry. </returns>
        public CachedEntry MarkAccess() { _utcAccessStamp = DateTime.UtcNow; return this; }
    }
    #endregion // Typedefs

    #region Fields

    private int _maxSize;
    private int _disposableState = StateUndisposed;
    private bool _shouldDisposeEvictedValues = typeof(IDisposable).IsAssignableFrom(typeof(TValue));
    private readonly Dictionary<TKey, CachedEntry> _dictionary;
    private readonly ReaderWriterLockSlim _readerWriterLock = new(LockRecursionPolicy.NoRecursion);

    /// <summary> The constant representing the not-disposed state. </summary>
    private const int StateUndisposed = 0;
    /// <summary> The constant representing the disposed state. </summary>
    private const int StateDisposed = 1;

    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public SizeLimitedCache() : this(int.MaxValue)
    { }

    /// <summary> The constructor accepting maxSize and key comparer. </summary>
    ///
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="maxSize"/> is zero or
    /// negative. </exception>
    ///
    /// <param name="maxSize">  The maximum size of the cache. Must be positive. </param>
    /// <param name="comparer"> (Optional) The key comparer. If null, default comparer will be used. </param>
    public SizeLimitedCache(int maxSize, IEqualityComparer<TKey> comparer = null)
    {
        _dictionary = new Dictionary<TKey, CachedEntry>(comparer ?? EqualityComparer<TKey>.Default);
        this.MaxSize = CheckMaxSizePositive(maxSize, nameof(maxSize));
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether the dispose evicted values, thus modifying the initialization
    /// from constructor.
    /// </summary>
    ///
    /// <remarks>
    /// It does not make much sense to modify this value from false to true, 
    /// if TValue is not disposable ( while doing so will not cause an error).
    /// But for disposable TValue, you may want to specify false in special cases, 
    /// if values are actually owned and disposed by 'someone else'.
    /// </remarks>
    public bool DisposesEvictedValues
    {
        get { return _shouldDisposeEvictedValues; }
        protected set { _shouldDisposeEvictedValues = value; }
    }

    /// <summary>
    /// Gets the current size, without locking.
    /// The caller of this property should guarantee that _readerWriterLock holds either read or write lock.
    /// </summary>
    protected int CurrentSizeWithoutLock
    {
        get
        {
            Debug.Assert(_readerWriterLock.IsReadLockHeld || _readerWriterLock.IsWriteLockHeld);
            return _dictionary.Count;
        }
    }
    #endregion // Properties

    #region Methods

    /// <summary> Checks that maximum size <paramref name="argValue"/> is positive. </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="argValue"/> is zero or negative. </exception>
    ///
    /// <param name="argValue"> The argument value to be checked. </param>
    /// <param name="argName">  Name of the argument. </param>
    ///
    /// <returns> An original value of <paramref name="argValue"/>. </returns>
    protected static int CheckMaxSizePositive(int argValue, string argName)
    {
        if (argValue <= 0) throw new ArgumentOutOfRangeException(argName, argValue, "The maximum size must be positive");
        return argValue;
    }

    /// <summary> Dispose the value going to be evicted from the cache. </summary>
    /// <param name="value"> The value to be disposed. Could be null. </param>
    protected virtual void DisposeEvictedValue(TValue value)
    {
        Debug.Assert(this.DisposesEvictedValues);
        (value as IDisposable)?.Dispose();
    }

    /// <summary> Change maximum size of cache. </summary>
    /// <param name="maxSize">  The new maximum size of the cache. </param>
    protected virtual void ChangeMaxSize(int maxSize)
    {
        int newLimit = CheckMaxSizePositive(maxSize, nameof(maxSize));
        if (MaxSize != newLimit)
        {
            _readerWriterLock.EnterWriteLock();

            try
            {
                int oldLimit = this.MaxSize;
                int nowCached = CurrentSizeWithoutLock;

                if (newLimit != oldLimit)
                {
                    this._maxSize = newLimit;
                    if (newLimit > oldLimit)
                    {
                        Debug.Assert(nowCached <= oldLimit);
                    }
                    else if (nowCached > newLimit)
                    {
                        ShrinkToMaxSize();
                    }
                }
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
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
    protected virtual void Dispose(bool disposing)
    {
        // Attempt to move the disposable state from StateUndisposed to StateDisposed. 
        // If successful, we can be assured that this thread is the first thread to do so,
        // and can safely dispose of the object.
        // 
        if (Interlocked.CompareExchange(ref _disposableState, StateDisposed, StateUndisposed) == StateUndisposed)
        {
            // If disposing equals true, dispose managed resources first.
            if (disposing)
            {
                // to dispose cached values
                Clear();
                _readerWriterLock.Dispose();
            }
            // Now release unmanaged resources. Actually nothing to do here.
        }
    }

    /// <summary> Shrinks currently cached size to maximum size. 
    /// Has no effect if currently cached size does not exceed maximum size.
    /// </summary>
    /// <exception cref="InvalidOperationException"> Thrown when _readerWriterLock does not hold read lock. </exception>
    ///
    /// <returns> True if there was any change, false if not. </returns>
    protected virtual bool ShrinkToMaxSize()
    {
        this.CheckNotDisposed();
        CheckWriteLockIsHeld(nameof(ShrinkToMaxSize));

        int countToBeRemoved = this.CurrentSizeWithoutLock - this.MaxSize;
        bool result;

        if (result = (countToBeRemoved > 0))
        {
            if (this.DisposesEvictedValues)
            {
                // From performance perspective, it's better to materialize evicted items to list immediately,
                // to prevent iterating over the same collection several times during eviction.
                //
                var orderedPairs = _dictionary.OrderBy(pair => pair.Value.UtcAccessStamp)
#pragma warning disable IDE0037 // Use inferred member name
                    .Select(pair => new { Key = pair.Key, Value = pair.Value.CachedValue }).ToList();
#pragma warning restore IDE0037 // Use inferred member name
                // Now, could involve 'Take' to avoid creating another list
                foreach (var pair in orderedPairs.Take(countToBeRemoved))
                {
                    DisposeEvictedValue(pair.Value);
                    _dictionary.Remove(pair.Key);
                }
            }
            else
            {
                // One could be faster here, not considering evicted values.
                // Also, 'for' loops on List are faster than 'foreach' loops on List
                List<TKey> orderedKeys = _dictionary.OrderBy(pair => pair.Value.UtcAccessStamp)
                    .Select(pair => pair.Key).ToList();
                for (int ii = 0; ii < countToBeRemoved; ii++)
                {
                    _dictionary.Remove(orderedKeys[ii]);
                }
            }
            Debug.Assert(this.CurrentSizeWithoutLock <= this.MaxSize);
        }

        return result;
    }

    /// <summary> Check that write lock is held. </summary>
    /// <exception cref="InvalidOperationException">  Thrown the caller did not acquire write lock. </exception>
    private void CheckWriteLockIsHeld(string caller)
    {
        if (!_readerWriterLock.IsWriteLockHeld)
        {
            Debug.Fail(Invariant($"{caller} needs write lock"));
            throw new InvalidOperationException(Invariant($"{caller} assumes write lock is hold."));
        }
    }
    #endregion // Methods

    #region ISizeLimitedCache<TKey, TValue> Members
    #region IDisposableEx Members

    /// <inheritdoc/>
    public bool IsDisposed
    {
        get { return (Thread.VolatileRead(ref this._disposableState) == StateDisposed); }
    }

    /// <summary>
    /// Implements IDisposable. Do not make this method virtual. A derived class should not be able to override it.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposableEx Members

    /// <inheritdoc/>
    public int CurrentSize
    {
        get
        {
            _readerWriterLock.EnterReadLock();
            try
            {
                return _dictionary.Count;
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }
    }

    /// <inheritdoc/>
    public int MaxSize
    {
        get => _maxSize;
        set { ChangeMaxSize(CheckMaxSizePositive(value, nameof(value))); }
    }

    /// <inheritdoc/>
    public TValue this[TKey key]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(key);

            _readerWriterLock.EnterReadLock();
            try
            {
                return _dictionary[key].MarkAccess().CachedValue; // indexer may throw KeyNotFoundException
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }
        set
        {
            AddOrModify(key, value);
        }
    }


    /// <inheritdoc/>
    public bool TryGetValue(TKey key, out TValue value)
    {
        ArgumentNullException.ThrowIfNull(key);

        _readerWriterLock.EnterReadLock();
        try
        {
            if (_dictionary.TryGetValue(key, out CachedEntry entry))
            {
                value = entry.MarkAccess().CachedValue;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        finally
        {
            _readerWriterLock.ExitReadLock();
        }
    }

    /// <inheritdoc/>
    public void AddOrModify(TKey key, TValue value)
    {
        ArgumentNullException.ThrowIfNull(key);

        _readerWriterLock.EnterWriteLock();

        try
        {
            if (_dictionary.TryGetValue(key, out CachedEntry entry))
            {
                if ((entry.CachedValue != null) && entry.CachedValue.Equals(value))
                {
                    entry.MarkAccess();
                }
                else
                {
                    if (this.DisposesEvictedValues)
                    {
                        this.DisposeEvictedValue(entry.CachedValue);
                    }
                    _dictionary[key] = new CachedEntry(value);
                }
            }
            else
            {
                _dictionary.Add(key, new CachedEntry(value));
                if (CurrentSizeWithoutLock > MaxSize)
                {
                    ShrinkToMaxSize();
                }
            }
        }
        finally
        {
            _readerWriterLock.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);

        _readerWriterLock.EnterWriteLock();
        try
        {
            if (this.DisposesEvictedValues)
            {
                if (_dictionary.TryGetValue(key, out CachedEntry entry))
                    DisposeEvictedValue(entry.CachedValue);
                else
                    return false;
            }
            return _dictionary.Remove(key);
        }
        finally
        {
            _readerWriterLock.ExitWriteLock();
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _readerWriterLock.EnterWriteLock();
        try
        {
            if (this.DisposesEvictedValues)
            {
                foreach (CachedEntry entry in _dictionary.Values)
                {
                    DisposeEvictedValue(entry.CachedValue);
                }
            }
            _dictionary.Clear();
        }
        finally
        {
            _readerWriterLock.ExitWriteLock();
        }
    }
    #endregion // ISizeLimitedCache<TKey, TValue> Members
}
#pragma warning restore IDE0305
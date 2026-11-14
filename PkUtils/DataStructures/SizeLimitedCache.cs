// Ignore Spelling: Typedefs, Utc, Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using static System.FormattableString;

#pragma warning disable IDE0079 // Remove unnecessary suppression 
#pragma warning disable IDE0028 // Simplify collection initialization
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
        // Cache readers can update the same entry concurrently; access ticks must be atomic.
        private long _utcAccessTicks;
        private readonly TValue _cachedValue;

        /// <summary> The only constructor. </summary>
        /// <param name="cachedValue">  The cached value. </param>
        public CachedEntry(TValue cachedValue)
        {
            _cachedValue = cachedValue;
            MarkAccess();
        }

        /// <summary> Gets the UTC Date/Time of last access stamp. </summary>
        public DateTime UtcAccessStamp
        {
            get => new(Interlocked.Read(ref _utcAccessTicks), DateTimeKind.Utc);
        }

        /// <summary> Gets the cached value. </summary>
        public TValue CachedValue { get => _cachedValue; }

        /// <summary> Mark the time of last access. </summary>
        /// <returns> This CachedEntry. </returns>
        public CachedEntry MarkAccess()
        {
            Interlocked.Exchange(ref _utcAccessTicks, DateTime.UtcNow.Ticks);
            return this;
        }
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
            List<TValue> evictedValues = null;
            _readerWriterLock.EnterWriteLock();

            try
            {
                // Reject size changes that reached the cache after disposal began.
                ThrowIfDisposedInsideLock();
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
                        evictedValues = ShrinkToMaxSize();
                    }
                }
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }

            // Evicted values must be disposed only after releasing the cache lock.
            DisposeDetachedEvictedValues(evictedValues);
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
            // Remove entries directly under the write lock: the public Clear() method is not suitable after the disposed transition.
            if (disposing)
            {
                List<TValue> valuesToDispose = null;
                _readerWriterLock.EnterWriteLock();
                try
                {
                    if (DisposesEvictedValues)
                        valuesToDispose = _dictionary.Values.Select(entry => entry.CachedValue).ToList();
                    _dictionary.Clear();
                }
                finally
                {
                    _readerWriterLock.ExitWriteLock();
                }

                // Finish disposing all cache-owned values even when an individual Dispose() throws.
                Exception firstError = null;
                if (valuesToDispose is not null)
                {
                    foreach (TValue value in valuesToDispose)
                    {
                        try { DisposeEvictedValue(value); }
                        catch (Exception ex) { firstError ??= ex; }
                    }
                }

                // Keep the managed lock alive to avoid racing with threads already waiting to enter it during disposal.
                // Calls made after disposal must reject access before reading cache state.
                if (firstError is not null)
                    throw new InvalidOperationException("One or more cached values could not be disposed.", firstError);
            }
            // Now release unmanaged resources. Actually nothing to do here.
        }
    }

    /// <summary>
    /// Removes the least recently accessed cache entries until the cached size does not exceed the maximum size.
    /// Has no effect if the current size is already within the limit.
    /// </summary>
    /// <remarks>
    /// The caller must hold the write lock. This method only detaches entries from the cache; it never
    /// disposes their values. The caller must dispose the returned values after releasing the write lock.
    /// When <see cref="DisposesEvictedValues"/> is false, removed values are not returned for disposal.
    /// </remarks>
    /// <exception cref="InvalidOperationException"> Thrown when the caller does not hold the write lock. </exception>
    /// <returns>
    /// The values requiring disposal after the write lock is released; an empty list if no values should
    /// be disposed.
    /// </returns>
    protected virtual List<TValue> ShrinkToMaxSize()
    {
        this.CheckNotDisposed();
        CheckWriteLockIsHeld(nameof(ShrinkToMaxSize));

        List<TValue> evictedValues = [];
        int countToBeRemoved = this.CurrentSizeWithoutLock - this.MaxSize;

        if (countToBeRemoved > 0)
        {
            if (this.DisposesEvictedValues)
            {
                // Materialize candidates before removing entries from the dictionary.
                // Values with the earliest UtcAccessStamp will be removed.
                List<(TKey Key, TValue Value)> orderedPairs = _dictionary
                    .OrderBy(pair => pair.Value.UtcAccessStamp)
                    .Take(countToBeRemoved)
                    .Select(pair => (pair.Key, Value: pair.Value.CachedValue))
                    .ToList();

                foreach ((TKey Key, TValue Value) in orderedPairs)
                {
                    _dictionary.Remove(Key);
                    evictedValues.Add(Value);
                }
            }
            else
            {
                // Avoid collecting values when the cache does not own them.
                List<TKey> orderedKeys = _dictionary
                    .OrderBy(pair => pair.Value.UtcAccessStamp)
                    .Take(countToBeRemoved)
                    .Select(pair => pair.Key)
                    .ToList();

                foreach (TKey key in orderedKeys)
                {
                    _dictionary.Remove(key);
                }
            }

            Debug.Assert(this.CurrentSizeWithoutLock <= this.MaxSize);
        }

        return evictedValues;
    }

    /// <summary>
    /// Disposes values detached by <see cref="ShrinkToMaxSize"/> after the caller has released the write lock.
    /// Continues disposing the remaining values if an individual disposal fails.
    /// </summary>
    /// <param name="evictedValues"> Values to dispose, or null if no eviction occurred. </param>
    /// <exception cref="InvalidOperationException"> Thrown if one or more values could not be disposed. </exception>
    private void DisposeDetachedEvictedValues(List<TValue> evictedValues)
    {
        if (evictedValues is null)
            return;

        Exception firstError = null;
        foreach (TValue value in evictedValues)
        {
            try
            {
                DisposeEvictedValue(value);
            }
            catch (Exception ex)
            {
                firstError ??= ex;
            }
        }

        if (firstError is not null)
        {
            throw new InvalidOperationException(
                "One or more evicted values could not be disposed.", firstError);
        }
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

    // Prevent operations queued on the lock from accessing a cache after disposal has begun.
    private void ThrowIfDisposedInsideLock()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
    }

    #region ISizeLimitedCache<TKey, TValue> Members
    #region IDisposableEx Members

    /// <inheritdoc/>
    public bool IsDisposed
    {
        get { return (Volatile.Read(ref this._disposableState) == StateDisposed); }
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
                // Reject operations that were already waiting when the cache entered its disposed state.
                ThrowIfDisposedInsideLock();
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
                // Reject operations that were already waiting when the cache entered its disposed state.
                ThrowIfDisposedInsideLock();
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
            // Reject operations that were already waiting when the cache entered its disposed state.
            ThrowIfDisposedInsideLock();
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

        TValue valueToDispose = default;
        bool shouldDispose = false;
        List<TValue> evictedValues = null;

        _readerWriterLock.EnterWriteLock();

        try
        {
            // Reject writes that were waiting when disposal began.
            ThrowIfDisposedInsideLock();
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
                        valueToDispose = entry.CachedValue;
                        shouldDispose = true;
                    }
                    _dictionary[key] = new CachedEntry(value);
                }
            }
            else
            {
                _dictionary.Add(key, new CachedEntry(value));
                if (CurrentSizeWithoutLock > MaxSize)
                {
                    evictedValues = ShrinkToMaxSize();
                }
            }
        }
        finally
        {
            _readerWriterLock.ExitWriteLock();
        }

        // Dispose detached values only after releasing the cache lock.
        if (shouldDispose)
        {
            DisposeEvictedValue(valueToDispose);
        }
        DisposeDetachedEvictedValues(evictedValues);
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);

        TValue valueToDispose = default;
        bool shouldDispose = false;
        bool removed;

        _readerWriterLock.EnterWriteLock();
        try
        {
            // Reject operations that were already waiting when the cache entered its disposed state.
            ThrowIfDisposedInsideLock();

            if (!_dictionary.TryGetValue(key, out CachedEntry entry))
                return false;

            removed = _dictionary.Remove(key);
            if (removed && this.DisposesEvictedValues)
            {
                valueToDispose = entry.CachedValue;
                shouldDispose = true;
            }
        }
        finally
        {
            _readerWriterLock.ExitWriteLock();
        }

        // Dispose the detached value outside the cache lock.
        if (shouldDispose)
        {
            DisposeEvictedValue(valueToDispose);
        }

        return removed;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        List<TValue> valuesToDispose = null;

        _readerWriterLock.EnterWriteLock();
        try
        {
            // Reject operations that were already waiting when the cache entered its disposed state.
            ThrowIfDisposedInsideLock();

            if (this.DisposesEvictedValues)
            {
                valuesToDispose = _dictionary.Values
                    .Select(entry => entry.CachedValue).ToList();
            }

            _dictionary.Clear();
        }
        finally
        {
            _readerWriterLock.ExitWriteLock();
        }

        // Dispose detached values outside the lock, continuing after individual failures.
        Exception firstError = null;
        if (valuesToDispose is not null)
        {
            foreach (TValue value in valuesToDispose)
            {
                try
                {
                    DisposeEvictedValue(value);
                }
                catch (Exception ex)
                {
                    firstError ??= ex;
                }
            }
        }

        if (firstError is not null)
        {
            string message = Invariant($"One or more cached values could not be disposed, {firstError.GetType().Name} has occurred.");
            throw new InvalidOperationException(message, firstError);
        }
    }
    #endregion // ISizeLimitedCache<TKey, TValue> Members
}
#pragma warning restore IDE0305
#pragma warning restore IDE0028
#pragma warning restore IDE0079
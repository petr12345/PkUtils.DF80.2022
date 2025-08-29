// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using PK.PkUtils.Interfaces;
using PK.PkUtils.Utils;

#pragma warning disable IDE0290 // Use primary constructor

namespace PK.PkUtils.Threading;

/// <summary> A concurrent locker, supporting key-based async locking. </summary>
/// <typeparam name="TKey"> Type of the key. </typeparam>
public class ConcurrentLocker<TKey> : IConcurrentLocker<TKey>
{
    #region Typedefs

    /// <summary> 
    /// Instance of SemaphoreWrapper is used as value in ConcurrentLocker internal dictionary.
    /// The object returned from LockAsync is UsageMonitor, referring to this SemaphoreWrapper.
    /// </summary>
    protected class SemaphoreWrapper(ConcurrentLocker<TKey> owner, TKey relatedKey) : UsageCounter, IDisposableEx
    {
        private SemaphoreSlim _semaphore = new(1, 1);
        private readonly ConcurrentLocker<TKey> _owner = owner;
        private readonly TKey _relatedKey = relatedKey;

        /// <summary> Gets the underlying semaphore. </summary>
        public SemaphoreSlim SemaphoreSlim { get { return _semaphore; } }

        /// <summary> Gets the related key. </summary>
        public TKey RelatedKey { get => _relatedKey; }

        /// <summary> Returns true in case the object has been disposed and no longer should be used. </summary>
        public bool IsDisposed { get => (_semaphore == null); }

        /// <summary> Releasing, all managed and unmanaged resources. </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary> Unlock that please, releasing semaphore first. </summary>
        /// <returns>   The amount of remaining references. </returns>
        public override int Release()
        {
            SemaphoreSlim.Release();
            return base.Release();
        }

        /// <summary> Calls base release, without releasing the semaphore. </summary>
        /// <returns> The amount of remaining references. </returns>
        public int BaseRelease()
        {
            return base.Release();
        }

        /// <summary> The method invoked when the last lock has been released. </summary>
        protected override void OnLastRelease()
        {
            if (_owner.RemoveKey(RelatedKey))
                Dispose();
            else
                throw new InvalidOperationException($"The {_owner.GetType().Name} should contain key '{RelatedKey}'");
        }

        /// <summary> Releasing all managed and unmanaged resources. </summary>
        /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                    Disposer.SafeDispose(ref _semaphore);
                else
                    _semaphore = null;
            }
        }
    }
    #endregion // Typedefs

    #region Fields

    private int _disposableState = StateUndisposed;
    private readonly Func<TKey, string> _fnErrorMessage;
    private readonly Dictionary<TKey, SemaphoreWrapper> _dictionary;
    private readonly ReaderWriterLockSlim _readerWriterLock = new(LockRecursionPolicy.SupportsRecursion);

    /// <summary> The constant representing the not-disposed state. </summary>
    private const int StateUndisposed = 0;
    /// <summary> The constant representing the disposed state. </summary>
    private const int StateDisposed = 1;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> The primary constructor. </summary>
    /// <param name="fnErrorMessage"> The callback to be called to create an error message for specific key. </param>
    /// <param name="comparer"> (Optional) The key comparer. If null, default comparer will be used. </param>
    public ConcurrentLocker(Func<TKey, string> fnErrorMessage, IEqualityComparer<TKey> comparer = null)
    {
        _fnErrorMessage = fnErrorMessage ?? throw new ArgumentNullException(nameof(fnErrorMessage));
        _dictionary = new Dictionary<TKey, SemaphoreWrapper>(comparer ?? EqualityComparer<TKey>.Default);
    }

    /// <summary> Constructor accepting a constant error message and key comparer. </summary>
    /// <param name="errorMessage"> Message describing the error. </param>
    /// <param name="comparer"> (Optional) The key comparer. If null, default comparer will be used. </param>
    public ConcurrentLocker(string errorMessage, IEqualityComparer<TKey> comparer = null)
        : this((x) => errorMessage, comparer)
    { }

    /// <summary> Constructor accepting just key comparer. </summary>
    /// <param name="comparer"> (Optional) The key comparer. If null, default comparer will be used. </param>
    public ConcurrentLocker(IEqualityComparer<TKey> comparer = null)
        : this((x) => $"An error has occurred when creating lock for key '{x}'", comparer)
    { }

    #endregion // Constructor(s)

    #region IConcurrentLocker members

    /// <inheritdoc/>
    public int CurrentSize
    {
        get
        {
            if (this.IsDisposed) return 0;
            using (new SlimLockReaderGuard(_readerWriterLock))
            {
                return _dictionary.Count;
            }
        }
    }

    /// <inheritdoc/>
    public Task<IDisposable> LockAsync(TKey key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        using (new SlimLockWriterGuard(_readerWriterLock))
        {
            if (_dictionary.TryGetValue(key, out SemaphoreWrapper wrapper))
                Debug.Assert(wrapper.RelatedKey.Equals(key));
            else
                _dictionary.Add(key, wrapper = new SemaphoreWrapper(this, key));

            return new AwaitableDisposable<IDisposable>(WaitForSemaphoreAsync((wrapper)));
        }
    }

    /// <inheritdoc/>
    public Task<IDisposable> LockAsync(TKey key, int millisecondsTimeout)
    {
        return LockAsync(key, millisecondsTimeout, CancellationToken.None);
    }

    /// <inheritdoc/>
    public Task<IDisposable> LockAsync(TKey key, int millisecondsTimeout, CancellationToken cancellationToken)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (millisecondsTimeout < -1) throw new ArgumentOutOfRangeException(
            nameof(millisecondsTimeout), millisecondsTimeout, $"{nameof(millisecondsTimeout)} cannot have negative value less than -1");

        using (new SlimLockWriterGuard(_readerWriterLock))
        {
            if (!_dictionary.TryGetValue(key, out SemaphoreWrapper wrapper))
            {
                _dictionary.Add(key, wrapper = new SemaphoreWrapper(this, key));
            }
            else if (millisecondsTimeout == 0)
            {
                throw new ConcurrencyConflictException(_fnErrorMessage(key));  // don't bother waiting
            }

            // Note that call below still may throw OptimisticConcurrencyException exception of its own
            return new AwaitableDisposable<IDisposable>(
                WaitForSemaphoreAsync(wrapper, millisecondsTimeout, cancellationToken));
        }
    }
    #endregion // IConcurrentLocker members

    #region IDisposableEx Members

    /// <inheritdoc/>
    public bool IsDisposed
    {
        get { return (Thread.VolatileRead(ref this._disposableState) == StateDisposed); }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposableEx Members

    #region Methods

    /// <summary> Removes the key from internal dictionary. </summary>
    /// <param name="key">  The identifier. </param>
    /// <returns>   True if it succeeds, false if it fails. </returns>
    protected internal bool RemoveKey(TKey key)
    {
        using (new SlimLockWriterGuard(_readerWriterLock))
        {
            return _dictionary.Remove(key);
        }
    }

    /// <summary> Clears the locks. </summary>
    protected void ClearLocks()
    {
        using (new SlimLockWriterGuard(_readerWriterLock))
        {
            foreach (KeyValuePair<TKey, SemaphoreWrapper> pair in _dictionary)
            {
                pair.Value.Dispose();
            }
            _dictionary.Clear();
        }
    }

    /// <summary> Releasing all managed and unmanaged resources. </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer.</param>
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
                ClearLocks();
                _readerWriterLock.Dispose();
            }
        }
    }

    /// <summary> Wait for semaphore asynchronously. </summary>
    /// <param name="semaphoreWrapper"> The semaphore wrapper. </param>
    /// <returns>   The disposable lock object. </returns>
    protected async Task<IDisposable> WaitForSemaphoreAsync(SemaphoreWrapper semaphoreWrapper)
    {
        // Note, the returned UsageMonitor will on release calls SemaphoreWrapper.Release(),
        // which in turn calls SemaphoreSlim.Release();
        // 
        UsageMonitor usageWrapper = new(semaphoreWrapper, firstTimeUseOnly: false);

        await semaphoreWrapper.SemaphoreSlim.WaitAsync();
        return usageWrapper;
    }

    /// <summary> Wait for semaphore asynchronously. </summary>
    ///
    /// <exception cref="ConcurrencyConflictException"> Thrown when error condition occurs. </exception>
    /// <param name="semaphoreWrapper"> The semaphore wrapper. </param>
    /// <param name="millisecondsTimeout"> The milliseconds timeout. </param>
    /// <param name="cancellationToken"> A token that allows processing to be canceled. </param>
    /// <returns> The task waiting for semaphore. </returns>
    protected async Task<IDisposable> WaitForSemaphoreAsync(
        SemaphoreWrapper semaphoreWrapper,
        int millisecondsTimeout,
        CancellationToken cancellationToken)
    {
        UsageMonitor usageWrapper = new(semaphoreWrapper, firstTimeUseOnly: false);
        bool succeeded;

        try
        {
            succeeded = await semaphoreWrapper.SemaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            succeeded = false;
        }
        catch (OperationCanceledException)
        {
            succeeded = false;
        }
        catch (Exception ex)
        {   // something not expected happened
            Debug.Fail(ex.GetType().ToString() + ex.Message);
            succeeded = false;
        }

        if (succeeded)
        {
            return usageWrapper;
        }
        else
        {
            semaphoreWrapper.BaseRelease();  // 'manually' release the reference acquired by new UsageMonitor
            throw new ConcurrencyConflictException(_fnErrorMessage(semaphoreWrapper.RelatedKey));
        }
    }
    #endregion // Methods
}

#pragma warning restore IDE0290 // Use primary constructor
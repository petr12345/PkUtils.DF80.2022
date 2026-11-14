// Ignore Spelling: Utils
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.UnitTests.DataStructuresTest;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable IDE0305   // Collection initialization can be simplified
#pragma warning disable CA1806  // result of some calls is not used
#pragma warning disable CA1859  // Change type of variable 'cache' from ..

/// <summary> MSTest tests of generic class SizeLimitedCache. </summary>
[TestClass()]
public class SizeLimitedCacheTests
{
    #region Typedefs_Test_helpers

    /// <summary>
    /// A cache derived for tests of the virtual shrinking contract, including control over
    /// which detached values are returned for disposal by the base class's callers.
    /// </summary>
    private sealed class TrackingCache : SizeLimitedCache<int, TrackedDisposable>
    {
        /// <summary>Initializes a tracking cache with the specified ownership policy.</summary>
        /// <param name="maxSize">The maximum number of retained entries.</param>
        /// <param name="disposesEvictedValues">Whether detached values are owned by the cache.</param>
        public TrackingCache(int maxSize, bool disposesEvictedValues = true) : base(maxSize)
        {
            DisposesEvictedValues = disposesEvictedValues;
        }

        /// <summary>Gets the number of calls dispatched to the override.</summary>
        public int ShrinkCallCount { get; private set; }

        /// <summary>Gets the values detached during the most recent shrink.</summary>
        public IReadOnlyList<TrackedDisposable> LastDetachedValues { get; private set; } = [];

        /// <summary>
        /// Gets or sets whether the override intentionally returns an empty list after detaching
        /// values. Used to verify that callers honor the overridden return value.
        /// 
        /// In practice, returning a different list of evicted objects could allow a derived class
        /// to implement alternative ownership handling. For example, the overridden method could
        /// call the base implementation, transfer some evicted objects to an object pool for reuse,
        /// and return only the remaining objects for disposal by the caller.
        /// Such an override must assume responsibility for every object it excludes from the
        /// returned list; otherwise, the objects might never be disposed.
        /// This test deliberately returns an empty list without transferring ownership,
        /// making it somewhat artificial but useful for verifying the virtual method contract.
        /// </summary>
        public bool OmitDetachedValuesFromReturn { get; set; }

        /// <summary>
        /// Records and optionally filters the values that callers are required to dispose
        /// after releasing the cache's write lock.
        /// </summary>
        /// <returns>The detached values that the caller must dispose.</returns>
        protected override List<TrackedDisposable> ShrinkToMaxSize()
        {
            ShrinkCallCount++;
            List<TrackedDisposable> detached = base.ShrinkToMaxSize();
            LastDetachedValues = detached.ToArray();

            return OmitDetachedValuesFromReturn ? [] : detached;
        }
    }

    /// <summary>Tracks disposal attempts and can raise a callback or a test exception.</summary>
    private sealed class TrackedDisposable : IDisposable
    {
        /// <summary>Gets how many times Dispose was invoked.</summary>
        public int DisposeCount { get; private set; }

        /// <summary>Gets or sets a callback invoked during Dispose.</summary>
        public Action? OnDispose { get; set; }

        /// <summary>Gets or sets whether Dispose should throw after recording its invocation.</summary>
        public bool ThrowOnDispose { get; set; }

        /// <summary>Records the disposal attempt and optionally executes test behavior.</summary>
        public void Dispose()
        {
            DisposeCount++;
            OnDispose?.Invoke();
            if (ThrowOnDispose)
            {
                throw new InvalidOperationException("Expected test disposal failure.");
            }
        }
    }
    #endregion // Typedefs_Test_helpers

    #region Fields

    private const int _addDelayMs = 24; // delay needed so the cache will distinguish timestamps
    #endregion // Fields

    #region Tests

    #region Tests_constructors

    [TestMethod]
    [Description("The constructor rejects a negative maximum cache size.")]
    public void SizeLimitedCache_Constructor_01()
    {
        // ACT + ASSERT
        Assert.ThrowsExactly<ArgumentOutOfRangeException>((Action)(() => new SizeLimitedCache<int, string>(-12)));
    }

    [TestMethod]
    [Description("A new cache starts empty.")]
    public void SizeLimitedCache_Constructor_02()
    {
        // ACT
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(12);
        // ASSERT
        Assert.AreEqual(0, cache.CurrentSize);
    }
    #endregion // Tests_constructors

    #region Tests_disposing_cache

    [TestMethod]
    [Description("Dispose marks the cache as disposed after adding values.")]
    public void SizeLimitedCache_Disposing_01()
    {
        // ARRANGE
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(12);

        // ACT
        foreach (int x in Enumerable.Range(0, 12))
        {
            cache.AddOrModify(x, x.ToString());
        }
        cache.Dispose();

        // ASSERT
        Assert.IsTrue(cache.IsDisposed);
    }

    [TestMethod]
    [Description("Concurrent population, removals and disposal leave all owned mutexes disposed.")]
    public void SizeLimitedCache_Disposing_02()
    {
        // ARRANGE
        // 
        const int nMaxThreads = 16;
        const int nMaxValues = 32;
        ParallelOptions options = new() { MaxDegreeOfParallelism = nMaxThreads };

        static bool IsMutexDisposed(Mutex mutex)
        {
            try
            {
                // A successful wait takes ownership; always release it before returning.
                if (mutex.WaitOne(0))
                {
                    mutex.ReleaseMutex();
                }
                return false;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }
        }

        // 1. Prepare populating mutexes
        // 
        IEnumerable<int> range = Enumerable.Range(0, nMaxValues);
        var populateSource = range.Select(x => new { Index = x, SynchroMutex = new Mutex() }).ToList();
        ISizeLimitedCache<int, Mutex> mutexCache = new SizeLimitedCache<int, Mutex>();

        Assert.IsTrue(mutexCache.DisposesEvictedValues, nameof(mutexCache.DisposesEvictedValues));

        // ACT
        // 
        // 2. Populate the cache the with an amount of mutexes. test doing it in parallel
        // 
        Parallel.For(0, nMaxValues, options, i =>
        {
            foreach (var x in populateSource)
            {
                mutexCache.AddOrModify(x.Index, x.SynchroMutex);
            }
        });


        // ASSERT
        // 
        // 3. Test all mutexes are present in the cache, and not disposed
        // 
        for (int jj = populateSource.Count - 1; jj >= 0; jj--)
        {
            Assert.IsTrue(mutexCache.TryGetValue(jj, out Mutex m));
            Assert.IsFalse(IsMutexDisposed(m), "Mutex should not be disposed now");
        }

        // 4. Attempt to remove a few items from cache, assert mutex was disposed by that
        //
        for (int jj = 6; jj < 8; jj++)
        {
            Assert.IsTrue(mutexCache.Remove(jj));
            Assert.IsTrue(IsMutexDisposed(populateSource[jj].SynchroMutex));
        }

        // 5. Call dispose on the cache from several threads
        //
        Parallel.For(0, nMaxThreads, options, i =>
        {
            mutexCache.Dispose();
        });

        // 6. Check the cache is disposed ok
        //
        Assert.IsTrue(mutexCache.IsDisposed);

        // 7. Check that now all populated mutexes are disposed as well
        //
        foreach (var x in populateSource)
        {
            Assert.IsTrue(IsMutexDisposed(x.SynchroMutex), "Mutex should be disposed now");
        }
    }
    #endregion // Tests_disposing_cache

    #region Tests_MaxSize

    [TestMethod]
    [Description("The MaxSize setter rejects zero and negative limits.")]
    [DataRow(0)]
    [DataRow(-2)]
    [DataRow(-22)]
    public void SizeLimitedCache_MaxSize_01(int invalidSize)
    {
        // ARRANGE
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(12);

        // ACT + ASSERT
        Assert.ThrowsExactly<ArgumentOutOfRangeException>((Action)(() => cache.MaxSize = invalidSize));
    }

    [TestMethod]
    [Description("Reducing MaxSize evicts the least recently accessed entries.")]
    [DataRow(12, 6)]
    [DataRow(12, 1)]
    public void SizeLimitedCache_MaxSize_02(int initialMaxSize, int laterMaxSize)
    {
        // ARRANGE
        // 
        int removedItems = initialMaxSize - laterMaxSize;
        IEnumerable<int> range = Enumerable.Range(0, initialMaxSize);
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(initialMaxSize);

        foreach (int x in range)
        {
            cache.AddOrModify(x, x.ToString());
            Thread.Sleep(_addDelayMs);
        }

        for (int jj = 0; jj < initialMaxSize; jj++)
        {
            Assert.IsTrue(cache.TryGetValue(jj, out _));
        }

        // ACT
        // 
        cache.MaxSize = laterMaxSize;

        // ASSERT
        // 
        for (int ii = 0; ii < removedItems; ii++)
        {
            Assert.IsFalse(cache.TryGetValue(ii, out _));
        }
        for (int jj = removedItems; jj < initialMaxSize; jj++)
        {
            Assert.IsTrue(cache.TryGetValue(jj, out _));
        }
    }
    #endregion // Tests_MaxSize

    #region Tests_CurrentSize

    [TestMethod]
    [Description("CurrentSize tracks insertions and changes to MaxSize.")]
    [DataRow(12, 7)]
    public void SizeLimitedCache_Indexer(int initialMaxSize, int laterMaxSize)
    {
        // ARRANGE
        // 
        IEnumerable<int> range = Enumerable.Range(0, initialMaxSize);
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(initialMaxSize);

        // ACT + ASSERT
        // 
        foreach (int x in range)
        {
            cache.AddOrModify(x, x.ToString());
            Thread.Sleep(_addDelayMs);
            Assert.AreEqual(x + 1, cache.CurrentSize);
        }

        cache.MaxSize = laterMaxSize;
        Assert.AreEqual(laterMaxSize, cache.CurrentSize);
    }
    #endregion // Tests_CurrentSize

    #region Tests_Indexer

    [TestMethod]
    [Description("Indexer insertion enforces capacity and retains the latest values.")]
    [DataRow(12, 3, 2)]
    [DataRow(200, 230, 4)]
    public void SizeLimitedCache_Indexer_ReturnsExpected(int firstValue, int addedItems, int maxCacheSize)
    {
        // ARRANGE
        int removedItemsCount = addedItems - maxCacheSize;
        IReadOnlyList<int> range = Enumerable.Range(firstValue, addedItems).ToList();
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(maxCacheSize);

        // ACT
        foreach (int x in range)
        {
            cache[x] = x.ToString();
            Thread.Sleep(_addDelayMs);
        }

        // ASSERT
        for (int ii = 0; ii < removedItemsCount; ii++)
        {
            Assert.IsFalse(cache.ContainsKey(range[ii]), $"value {range[ii]} should not be there");
        }

        for (int jj = removedItemsCount; jj < addedItems; jj++)
        {
            int key = range[jj];
            Assert.AreEqual(key.ToString(), cache[key]);
        }
    }

    [TestMethod]
    [Description("Indexer access to a missing key throws KeyNotFoundException.")]
    [DataRow(12, 3)]
    [DataRow(200, 230)]
    public void SizeLimitedCache_Indexer_ThrowsKeyNotFoundException(int firstValue, int addedItems)
    {
        // ARRANGE
        int notPresentKey = firstValue + addedItems;
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(addedItems);

        foreach (int x in Enumerable.Range(firstValue, addedItems))
        {
            cache[x] = x.ToString();
        }

        // ACT + ASSERT
        string v;
        Assert.ThrowsExactly<KeyNotFoundException>((Action)(() => v = cache[notPresentKey]));
    }
    #endregion // Tests_Indexer

    #region Tests_ContainsKey

    [TestMethod]
    [Description("ContainsKey distinguishes retained from evicted entries.")]
    [DataRow(12, 3, 2)]
    [DataRow(200, 230, 4)]
    public void SizeLimitedCache_ContainsKey(int firstValue, int addedItems, int maxCacheSize)
    {
        // ARRANGE
        int removedItemsCount = addedItems - maxCacheSize;
        IReadOnlyList<int> range = Enumerable.Range(firstValue, addedItems).ToList();
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(maxCacheSize);

        // ACT
        foreach (int x in range)
        {
            cache.AddOrModify(x, x.ToString());
            Thread.Sleep(_addDelayMs);
        }

        // ASSERT
        for (int ii = 0; ii < removedItemsCount; ii++)
        {
            Assert.IsFalse(cache.ContainsKey(range[ii]), $"value {range[ii]} should not be there");
        }

        for (int jj = removedItemsCount; jj < addedItems; jj++)
        {
            Assert.IsTrue(cache.ContainsKey(range[jj]));
        }
    }
    #endregion // Tests_ContainsKey

    #region Tests_TryGetValue

    [TestMethod]
    [Description("TryGetValue returns false for evicted entries and true for retained entries.")]
    [DataRow(-100, 3, 2)]
    [DataRow(200, 23, 4)]
    public void SizeLimitedCache_TryGetValue_01(int firstValue, int addedItems, int maxCacheSize)
    {
        // ARRANGE
        int removedItemsCount = addedItems - maxCacheSize;
        IReadOnlyList<int> range = Enumerable.Range(firstValue, addedItems).ToList();
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(maxCacheSize);

        // ACT
        foreach (int x in range)
        {
            cache.AddOrModify(x, x.ToString());
            Thread.Sleep(_addDelayMs);
        }

        // ASSERT
        for (int ii = 0; ii < removedItemsCount; ii++)
        {
            Assert.IsFalse(cache.TryGetValue(range[ii], out _), $"value {range[ii]} should not be there");
        }

        for (int jj = removedItemsCount; jj < addedItems; jj++)
        {
            Assert.IsTrue(cache.TryGetValue(range[jj], out _));
        }
    }
    #endregion // Tests_TryGetValue

    #region Tests_AddOrModify

    [TestMethod]
    [Description("AddOrModify enforces the configured maximum cache size.")]
    [DataRow(3, 2)]
    [DataRow(23, 4)]
    public void SizeLimitedCache_AddOrModify_01(int addedItems, int maxCacheSize)
    {
        // ARRANGE
        IEnumerable<int> range = Enumerable.Range(0, addedItems);
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(maxCacheSize);

        // ACT
        foreach (int x in range)
        {
            cache.AddOrModify(x, x.ToString());
            Thread.Sleep(_addDelayMs);
        }

        // ASSERT
        Assert.AreEqual(maxCacheSize, cache.CurrentSize);

        for (int jj = addedItems - maxCacheSize; jj < addedItems; jj++)
        {
            Assert.IsTrue(cache.TryGetValue(jj, out _), $"Item with key {jj} not found in the cache.");
        }
    }
    #endregion // Tests_AddOrModify

    #region Tests_Remove

    [TestMethod]
    [Description("Remove deletes entries and reports whether each key existed.")]
    [DataRow(10)]
    public void SizeLimitedCache_Remove_01(int addedItems)
    {
        // ARRANGE
        IEnumerable<int> range = Enumerable.Range(0, addedItems);
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(24);

        // ACT
        foreach (int x in range)
        {
            cache.AddOrModify(x, x.ToString());
            Thread.Sleep(_addDelayMs);
        }

        // ASSERT
        foreach (int x in range) Assert.IsTrue(cache.Remove(x));
        foreach (int x in range) Assert.IsFalse(cache.Remove(x));

        Assert.AreEqual(0, cache.CurrentSize);
    }
    #endregion // Tests_Remove

    #region Tests_Clear

    [TestMethod]
    [Description("Clear removes all cached entries.")]
    [DataRow(10)]
    public void SizeLimitedCache_Clear_01(int addedItems)
    {
        // ARRANGE
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(24);
        foreach (int x in Enumerable.Range(0, addedItems)) cache.AddOrModify(x, x.ToString());
        Assert.AreEqual(addedItems, cache.CurrentSize);

        // ACT
        cache.Clear();

        // ASSERT
        Assert.AreEqual(0, cache.CurrentSize);
    }
    #endregion // Tests_Clear

    #region Tests_ShrinkToMaxSize

    /// <summary>Verifies that both shrinking entry points invoke the virtual override.</summary>
    [TestMethod]
    [Description("Both AddOrModify and MaxSize changes dispatch to the overridden ShrinkToMaxSize.")]
    [DataRow(false)]
    [DataRow(true)]
    public void ShrinkToMaxSize_OverrideIsCalled(bool shrinkByChangingMaxSize)
    {
        // ARRANGE
        TrackingCache cache = new(shrinkByChangingMaxSize ? 2 : 1);
        TrackedDisposable first = new();
        TrackedDisposable second = new();
        cache.AddOrModify(1, first);
        if (shrinkByChangingMaxSize)
        {
            cache.AddOrModify(2, second);
        }
        Thread.Sleep(_addDelayMs);

        // ACT
        if (shrinkByChangingMaxSize)
        {
            cache.MaxSize = 1;
        }
        else
        {
            cache.AddOrModify(2, second);
        }

        // ASSERT
        Assert.AreEqual(1, cache.ShrinkCallCount);
        Assert.HasCount(1, cache.LastDetachedValues);
        Assert.Contains(first, cache.LastDetachedValues);
        Assert.AreEqual(1, first.DisposeCount);
        Assert.AreEqual(0, second.DisposeCount);
        Assert.AreEqual(1, cache.CurrentSize);
    }

    /// <summary>Verifies that disposal callbacks can reenter the cache from either shrinking caller.</summary>
    [TestMethod]
    [Description("Evicted-object disposal runs after releasing the write lock, allowing reentrant reads.")]
    [DataRow(false)]
    [DataRow(true)]
    public void ShrinkToMaxSize_DisposesOutsideWriteLock(bool shrinkByChangingMaxSize)
    {
        // ARRANGE
        TrackingCache cache = new(shrinkByChangingMaxSize ? 2 : 1);
        int sizeObservedDuringDispose = -1;
        TrackedDisposable first = new()
        {
            OnDispose = () => sizeObservedDuringDispose = cache.CurrentSize
        };
        TrackedDisposable second = new();
        cache.AddOrModify(1, first);
        Thread.Sleep(_addDelayMs);
        if (shrinkByChangingMaxSize)
        {
            cache.AddOrModify(2, second);
        }

        // ACT
        if (shrinkByChangingMaxSize)
        {
            cache.MaxSize = 1;
        }
        else
        {
            cache.AddOrModify(2, second);
        }

        // ASSERT
        Assert.AreEqual(1, sizeObservedDuringDispose);
        Assert.AreEqual(1, first.DisposeCount);
        Assert.AreEqual(0, second.DisposeCount);
        Assert.IsFalse(cache.ContainsKey(1));
        Assert.IsTrue(cache.ContainsKey(2));
    }

    /// <summary>Verifies deferred disposal for a multi-entry reduction of MaxSize.</summary>
    [TestMethod]
    [Description("MaxSize disposes all values returned by the virtual shrinking method, after detaching them.")]
    public void ShrinkToMaxSize_ReturnedValuesAreDisposed()
    {
        // ARRANGE
        TrackingCache cache = new(maxSize: 3);
        TrackedDisposable first = new();
        TrackedDisposable second = new();
        TrackedDisposable third = new();

        cache.AddOrModify(1, first);
        Thread.Sleep(_addDelayMs);
        cache.AddOrModify(2, second);
        Thread.Sleep(_addDelayMs);
        cache.AddOrModify(3, third);

        // ACT
        cache.MaxSize = 1;

        // ASSERT
        Assert.AreEqual(1, cache.ShrinkCallCount);
        Assert.AreSequenceEqual(new[] { first, second }, cache.LastDetachedValues.ToArray(), SequenceOrder.InAnyOrder);
        Assert.AreEqual(1, first.DisposeCount);
        Assert.AreEqual(1, second.DisposeCount);
        Assert.AreEqual(0, third.DisposeCount);
        Assert.AreEqual(1, cache.CurrentSize);
        Assert.IsTrue(cache.ContainsKey(3));
    }

    /// <summary>Verifies that the caller obeys a derived override's returned disposal list.</summary>
    [TestMethod]
    [Description("An overridden ShrinkToMaxSize controls which detached values are returned for disposal.")]
    public void ShrinkToMaxSize_OverrideControlsDisposalList()
    {
        // ARRANGE
        using TrackedDisposable first = new();
        using TrackingCache cache = new(1) { OmitDetachedValuesFromReturn = true };
        TrackedDisposable second = new(); // At the end of the test, this value remains in the cache, so should not be disposed.
        cache.AddOrModify(1, first);
        Thread.Sleep(_addDelayMs);

        // ACT
        cache.AddOrModify(2, second);

        // ASSERT
        Assert.AreEqual(1, cache.ShrinkCallCount);
        Assert.Contains(first, cache.LastDetachedValues);
        Assert.AreEqual(0, first.DisposeCount, "Only values returned from the override should be disposed.");
        Assert.IsFalse(cache.ContainsKey(1));
        Assert.IsTrue(cache.ContainsKey(2));
    }

    /// <summary>Verifies that one failing disposal does not prevent disposing other evicted values.</summary>
    [TestMethod]
    [Description("Shrink eviction removes all selected entries and attempts every disposal even if one throws.")]
    public void ShrinkToMaxSize_DisposalException()
    {
        // ARRANGE
        TrackingCache cache = new(maxSize: 3);
        TrackedDisposable first = new() { ThrowOnDispose = true };
        TrackedDisposable second = new();
        TrackedDisposable third = new();

        cache.AddOrModify(1, first);
        Thread.Sleep(_addDelayMs);
        cache.AddOrModify(2, second);
        Thread.Sleep(_addDelayMs);
        cache.AddOrModify(3, third);

        // ACT
        InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(() => cache.MaxSize = 1);

        // ASSERT
        Assert.AreEqual(typeof(InvalidOperationException), exception.InnerException?.GetType());
        Assert.AreEqual(1, cache.ShrinkCallCount);
        Assert.AreEqual(1, first.DisposeCount);
        Assert.AreEqual(1, second.DisposeCount);
        Assert.AreEqual(0, third.DisposeCount);
        Assert.AreEqual(1, cache.CurrentSize);
        Assert.IsFalse(cache.ContainsKey(1));
        Assert.IsFalse(cache.ContainsKey(2));
        Assert.IsTrue(cache.ContainsKey(3));
    }

    /// <summary>Verifies eviction when the cache does not own disposable values.</summary>
    [TestMethod]
    [Description("When automatic disposal is disabled, shrinking evicts entries without disposing their values.")]
    public void ShrinkToMaxSize_DisposalDisabled()
    {
        // ARRANGE
        TrackingCache cache = new(1, false);
        TrackedDisposable first = new();
        TrackedDisposable second = new();
        cache.AddOrModify(1, first);
        Thread.Sleep(_addDelayMs);

        // ACT
        cache.AddOrModify(2, second);

        // ASSERT
        Assert.IsFalse(cache.DisposesEvictedValues);
        Assert.AreEqual(1, cache.ShrinkCallCount);
        Assert.IsEmpty(cache.LastDetachedValues);
        Assert.AreEqual(0, first.DisposeCount);
        Assert.AreEqual(0, second.DisposeCount);
        Assert.AreEqual(1, cache.CurrentSize);
        Assert.IsFalse(cache.ContainsKey(1));
        Assert.IsTrue(cache.ContainsKey(2));

        // The caller retains ownership when automatic disposal is disabled.
        first.Dispose();
        second.Dispose();
    }

    /// <summary>Verifies that eviction honors the latest access, not insertion order.</summary>
    [TestMethod]
    [Description("Shrinking evicts the least recently accessed entry and retains an older entry accessed recently.")]
    public void ShrinkToMaxSize_LeastRecentlyAccessed()
    {
        // ARRANGE
        TrackingCache cache = new(3);
        TrackedDisposable first = new();
        TrackedDisposable second = new();
        TrackedDisposable third = new();
        TrackedDisposable fourth = new();

        cache.AddOrModify(1, first);
        Thread.Sleep(_addDelayMs);
        cache.AddOrModify(2, second);
        Thread.Sleep(_addDelayMs);
        cache.AddOrModify(3, third);
        Thread.Sleep(_addDelayMs);
        Assert.IsTrue(cache.TryGetValue(1, out _));
        Thread.Sleep(_addDelayMs);

        // ACT
        cache.AddOrModify(4, fourth);

        // ASSERT
        Assert.AreEqual(0, first.DisposeCount);
        Assert.AreEqual(1, second.DisposeCount);
        Assert.AreEqual(0, third.DisposeCount);
        Assert.AreEqual(0, fourth.DisposeCount);
        Assert.IsTrue(cache.ContainsKey(1));
        Assert.IsFalse(cache.ContainsKey(2));
        Assert.IsTrue(cache.ContainsKey(3));
        Assert.IsTrue(cache.ContainsKey(4));
    }
    #endregion // Tests_ShrinkToMaxSize

    #region Tests_disposal_exceptions_and_contracts

    /// <summary>Verifies that replacing an existing value transfers ownership correctly.</summary>
    [TestMethod]
    [Description("Replacing a cached value disposes the old object exactly once and retains the replacement.")]
    public void SizeLimitedCache_AddOrModify_ReplacementDisposesOldValue()
    {
        SizeLimitedCache<int, TrackedDisposable> cache = new(maxSize: 2);
        TrackedDisposable oldValue = new();
        TrackedDisposable newValue = new();

        // ARRANGE
        cache.AddOrModify(1, oldValue);

        // ACT
        cache.AddOrModify(1, newValue);

        // ASSERT
        Assert.AreEqual(1, oldValue.DisposeCount);
        Assert.AreEqual(0, newValue.DisposeCount);
        Assert.AreEqual(1, cache.CurrentSize);
        Assert.IsTrue(cache.TryGetValue(1, out TrackedDisposable retrieved));
        Assert.AreSame(newValue, retrieved);
    }

    /// <summary>Verifies cleanup when one cached value throws during Clear.</summary>
    [TestMethod]
    [Description("Clear attempts to dispose all values and empties the cache even if a value's Dispose throws.")]
    public void SizeLimitedCache_Clear_DisposalException()
    {
        // ARRANGE
        SizeLimitedCache<int, TrackedDisposable> cache = new(maxSize: 2);
        TrackedDisposable first = new() { ThrowOnDispose = true };
        TrackedDisposable second = new();
        cache.AddOrModify(1, first);
        cache.AddOrModify(2, second);

        // ACT
        InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(() => cache.Clear());

        // ASSERT
        Assert.AreEqual(typeof(InvalidOperationException), exception.InnerException?.GetType());
        Assert.AreEqual(1, first.DisposeCount);
        Assert.AreEqual(1, second.DisposeCount);
        Assert.AreEqual(0, cache.CurrentSize);
        Assert.IsFalse(cache.ContainsKey(1));
        Assert.IsFalse(cache.ContainsKey(2));
    }

    /// <summary>Verifies that Remove detaches an entry before calling its Dispose.</summary>
    [TestMethod]
    [Description("Remove completes dictionary removal even if the removed value throws during disposal.")]
    public void SizeLimitedCache_Remove_DisposalException()
    {
        // ARRANGE
        SizeLimitedCache<int, TrackedDisposable> cache = new(maxSize: 2);
        TrackedDisposable value = new() { ThrowOnDispose = true };
        cache.AddOrModify(1, value);

        // ACT
        InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(() => cache.Remove(1));

        // ASSERT
        Assert.Contains("Expected test disposal failure", exception.Message);
        Assert.AreEqual(1, value.DisposeCount);
        Assert.AreEqual(0, cache.CurrentSize);
        Assert.IsFalse(cache.ContainsKey(1));
    }

    /// <summary>Verifies that operations cannot use the cache after disposal.</summary>
    [TestMethod]
    [Description("Cache read and write operations throw ObjectDisposedException after disposal.")]
    public void SizeLimitedCache_OperationsAfterDispose_Throw()
    {
        // ARRANGE
        SizeLimitedCache<int, string> cache = new(maxSize: 1);
        cache.AddOrModify(1, "first");
        cache.Dispose();

        // ACT + ASSERT
        Assert.ThrowsExactly<ObjectDisposedException>(() => { _ = cache.CurrentSize; });
        Assert.ThrowsExactly<ObjectDisposedException>(() => { _ = cache[1]; });
        Assert.ThrowsExactly<ObjectDisposedException>(() => cache.TryGetValue(1, out _));
        Assert.ThrowsExactly<ObjectDisposedException>(() => cache.AddOrModify(2, "second"));
        Assert.ThrowsExactly<ObjectDisposedException>(() => cache.Remove(1));
        Assert.ThrowsExactly<ObjectDisposedException>(() => cache.Clear());
        Assert.ThrowsExactly<ObjectDisposedException>(() => cache.MaxSize = 2);
    }

    /// <summary>Verifies that repeated disposal does not dispose owned values twice.</summary>
    [TestMethod]
    [Description("Repeated calls to Dispose leave the cache disposed and dispose each owned value once.")]
    public void SizeLimitedCache_Disposing_RepeatedCallsAreIdempotent()
    {
        // ARRANGE
        SizeLimitedCache<int, TrackedDisposable> cache = new(2);
        TrackedDisposable first = new();
        TrackedDisposable second = new();
        cache.AddOrModify(1, first);
        cache.AddOrModify(2, second);

        // ACT
        cache.Dispose();
        cache.Dispose();

        // ASSERT
        Assert.IsTrue(cache.IsDisposed);
        Assert.AreEqual(1, first.DisposeCount);
        Assert.AreEqual(1, second.DisposeCount);
    }
    #endregion // Tests_disposal_exceptions_and_contracts

    #region Tests_key_contracts

    /// <summary>Verifies that a caller-supplied comparer determines key equality.</summary>
    [TestMethod]
    [Description("A custom case-insensitive key comparer treats differently cased keys as equal.")]
    public void SizeLimitedCache_CustomKeyComparer()
    {
        // ARRANGE
        SizeLimitedCache<string, string> cache = new(maxSize: 2, comparer: StringComparer.OrdinalIgnoreCase);
        cache.AddOrModify("Alpha", "original");

        // ACT
        cache.AddOrModify("ALPHA", "replacement");

        // ASSERT
        Assert.AreEqual(1, cache.CurrentSize);
        Assert.IsTrue(cache.ContainsKey("alpha"));
        Assert.IsTrue(cache.TryGetValue("aLpHa", out string value));
        Assert.AreEqual("replacement", value);
    }

    /// <summary>Verifies argument validation for null reference-type keys.</summary>
    [TestMethod]
    [Description("Operations accepting a key reject null with ArgumentNullException.")]
    public void SizeLimitedCache_NullKey_ThrowsArgumentNullException()
    {
        // ARRANGE
        SizeLimitedCache<string, string> cache = new(maxSize: 2);

        // ACT + ASSERT
        Assert.ThrowsExactly<ArgumentNullException>(() => cache.AddOrModify(null!, "value"));
        Assert.ThrowsExactly<ArgumentNullException>(() => cache.Remove(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => cache.TryGetValue(null!, out _));
        Assert.ThrowsExactly<ArgumentNullException>(() => { _ = cache[null!]; });
        Assert.ThrowsExactly<ArgumentNullException>(() => cache[null!] = "value");
    }
    #endregion // Tests_key_contracts
    #endregion // Tests
}

#pragma warning restore CA1859
#pragma warning restore CA1806
#pragma warning restore IDE0305
#pragma warning restore IDE0079

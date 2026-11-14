// Ignore Spelling: PkUtils, Utils
// 
using PK.PkUtils.DataStructures;
using PK.PkUtils.Interfaces;


namespace PK.PkUtils.NUnitTests.DataStructuresTest;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable CA1859    // Change type of variable ...
#pragma warning disable IDE0305 // Collection initialization can be simplified

/// <summary> NUnit tests of generic class SizeLimitedCache. </summary>
[TestFixture()]
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
        /// values. Used only to verify that callers honor the overridden return value.
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

    [Test]
    [Description("The constructor rejects a negative maximum cache size.")]
    public void SizeLimitedCache_Constructor_01()
    {
        // ACT + ASSERT
        Assert.Throws<ArgumentOutOfRangeException>((Action)(() => new SizeLimitedCache<int, string>(-12)));
    }

    [Test]
    [Description("A new cache starts empty.")]
    public void SizeLimitedCache_Constructor_02()
    {
        // ACT
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(12);
        // ASSERT
        Assert.That(cache.CurrentSize, Is.Zero);
    }
    #endregion // Tests_constructors

    #region Tests_disposing_cache

    [Test]
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
        Assert.That(cache.IsDisposed, Is.True);
    }

    [Test]
    [Description("Concurrent population, removals and disposal leave all owned mutexes disposed.")]
    public void SizeLimitedCache_Disposing_02()
    {
        // ARRANGE
        // 
        const int nMaxThreads = 16;
        const int nMaxValues = 32;
        ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = nMaxThreads };

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

        Assert.That(mutexCache.DisposesEvictedValues, Is.True, nameof(mutexCache.DisposesEvictedValues));

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
            using (Assert.EnterMultipleScope())
            {
                Assert.That(mutexCache.TryGetValue(jj, out Mutex m), Is.True);
                Assert.That(IsMutexDisposed(m), Is.False, "Mutex should not be disposed now");
            }
        }

        // 4. Attempt to remove a few items from cache, assert mutex was disposed by that
        //
        for (int jj = 6; jj < 8; jj++)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(mutexCache.Remove(jj), Is.True);
                Assert.That(IsMutexDisposed(populateSource[jj].SynchroMutex), Is.True);
            }
        }

        // 5. Call dispose on the cache from several threads
        //
        Parallel.For(0, nMaxThreads, options, i =>
        {
            mutexCache.Dispose();
        });

        // 6. Check the cache is disposed ok
        //
        Assert.That(mutexCache.IsDisposed, Is.True);

        // 7. Check that now all populated mutexes are disposed as well
        //
        foreach (var x in populateSource)
        {
            Assert.That(IsMutexDisposed(x.SynchroMutex), Is.True, "Mutex should be disposed now");
        }
    }
    #endregion // Tests_disposing_cache

    #region Tests_MaxSize

    [Test]
    [Description("The MaxSize setter rejects zero and negative limits.")]
    [TestCase(0)]
    [TestCase(-2)]
    [TestCase(-22)]
    public void SizeLimitedCache_MaxSize_01(int invalidSize)
    {
        // ARRANGE
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(12);

        // ACT + ASSERT
        Assert.Throws<ArgumentOutOfRangeException>((Action)(() => cache.MaxSize = invalidSize));
    }

    [Test]
    [Description("Reducing MaxSize evicts the least recently accessed entries.")]
    [TestCase(12, 6)]
    [TestCase(12, 1)]
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
            Assert.That(cache.TryGetValue(jj, out _), Is.True);
        }

        // ACT
        // 
        cache.MaxSize = laterMaxSize;

        // ASSERT
        // 
        for (int ii = 0; ii < removedItems; ii++)
        {
            Assert.That(cache.TryGetValue(ii, out _), Is.False);
        }
        for (int jj = removedItems; jj < initialMaxSize; jj++)
        {
            Assert.That(cache.TryGetValue(jj, out _), Is.True);
        }
    }
    #endregion // Tests_MaxSize

    #region Tests_CurrentSize

    [Test]
    [Description("CurrentSize tracks insertions and changes to MaxSize.")]
    [TestCase(12, 7)]
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
            Assert.That(cache.CurrentSize, Is.EqualTo(x + 1));
        }

        cache.MaxSize = laterMaxSize;
        Assert.That(cache.CurrentSize, Is.EqualTo(laterMaxSize));
    }
    #endregion // Tests_CurrentSize

    #region Tests_Indexer

    [Test]
    [Description("Indexer insertion enforces capacity and retains the latest values.")]
    [TestCase(12, 3, 2)]
    [TestCase(200, 230, 4)]
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
            Assert.That(cache.ContainsKey(range[ii]), Is.False, $"value {range[ii]} should not be there");
        }

        for (int jj = removedItemsCount; jj < addedItems; jj++)
        {
            int key = range[jj];
            Assert.That(cache[key], Is.EqualTo(key.ToString()));
        }
    }

    [Test]
    [Description("Indexer access to a missing key throws KeyNotFoundException.")]
    [TestCase(12, 3)]
    [TestCase(200, 230)]
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
        Assert.Throws<KeyNotFoundException>((Action)(() => v = cache[notPresentKey]));
    }
    #endregion // Tests_Indexer

    #region Tests_ContainsKey

    [Test]
    [Description("ContainsKey distinguishes retained from evicted entries.")]
    [TestCase(12, 3, 2)]
    [TestCase(200, 230, 4)]
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
            Assert.That(cache.ContainsKey(range[ii]), Is.False, $"value {range[ii]} should not be there");
        }

        for (int jj = removedItemsCount; jj < addedItems; jj++)
        {
            Assert.That(cache.ContainsKey(range[jj]), Is.True);
        }
    }
    #endregion // Tests_ContainsKey

    #region Tests_TryGetValue

    [Test]
    [Description("TryGetValue returns false for evicted entries and true for retained entries.")]
    [TestCase(-100, 3, 2)]
    [TestCase(200, 23, 4)]
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
            Assert.That(cache.TryGetValue(range[ii], out _), Is.False, $"value {range[ii]} should not be there");
        }

        for (int jj = removedItemsCount; jj < addedItems; jj++)
        {
            Assert.That(cache.TryGetValue(range[jj], out _), Is.True);
        }
    }
    #endregion // Tests_TryGetValue

    #region Tests_AddOrModify

    [Test]
    [Description("AddOrModify enforces the configured maximum cache size.")]
    [TestCase(3, 2)]
    [TestCase(23, 4)]
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
        Assert.That(cache.CurrentSize, Is.EqualTo(maxCacheSize));

        for (int jj = addedItems - maxCacheSize; jj < addedItems; jj++)
        {
            Assert.That(cache.TryGetValue(jj, out _), Is.True, $"Item with key {jj} not found in the cache.");
        }
    }
    #endregion // Tests_AddOrModify

    #region Tests_Remove

    [Test]
    [Description("Remove deletes entries and reports whether each key existed.")]
    [TestCase(10)]
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
        foreach (int x in range) Assert.That(cache.Remove(x), Is.True);
        foreach (int x in range) Assert.That(cache.Remove(x), Is.False);

        Assert.That(cache.CurrentSize, Is.Zero);
    }
    #endregion // Tests_Remove

    #region Tests_Clear

    [Test]
    [Description("Clear removes all cached entries.")]
    [TestCase(10)]
    public void SizeLimitedCache_Clear_01(int addedItems)
    {
        // ARRANGE
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(24);
        foreach (int x in Enumerable.Range(0, addedItems)) cache.AddOrModify(x, x.ToString());
        Assert.That(cache.CurrentSize, Is.EqualTo(addedItems));

        // ACT
        cache.Clear();

        // ASSERT
        Assert.That(cache.CurrentSize, Is.Zero);
    }
    #endregion // Tests_Clear

    #region Tests_ShrinkToMaxSize

    /// <summary>Verifies that both shrinking entry points invoke the virtual override.</summary>
    [Test]
    [Description("Both AddOrModify and MaxSize changes dispatch to the overridden ShrinkToMaxSize.")]
    [TestCase(false)]
    [TestCase(true)]
    public void ShrinkToMaxSize_OverrideIsCalled(bool shrinkByChangingMaxSize)
    {
        // ARRANGE
        TrackingCache cache = new TrackingCache(shrinkByChangingMaxSize ? 2 : 1);
        TrackedDisposable first = new TrackedDisposable();
        TrackedDisposable second = new TrackedDisposable();
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(cache.ShrinkCallCount, Is.EqualTo(1));
            Assert.That(cache.LastDetachedValues, Has.Count.EqualTo(1));
            Assert.That(cache.LastDetachedValues, Does.Contain(first));
            Assert.That(first.DisposeCount, Is.EqualTo(1));
            Assert.That(second.DisposeCount, Is.Zero);
            Assert.That(cache.CurrentSize, Is.EqualTo(1));
        }
    }

    /// <summary>Verifies that disposal callbacks can reenter the cache from either shrinking caller.</summary>
    [Test]
    [Description("Evicted-object disposal runs after releasing the write lock, allowing reentrant reads.")]
    [TestCase(false)]
    [TestCase(true)]
    public void ShrinkToMaxSize_DisposesOutsideWriteLock(bool shrinkByChangingMaxSize)
    {
        // ARRANGE
        TrackingCache cache = new TrackingCache(shrinkByChangingMaxSize ? 2 : 1);
        int sizeObservedDuringDispose = -1;
        TrackedDisposable first = new TrackedDisposable
        {
            OnDispose = () => sizeObservedDuringDispose = cache.CurrentSize
        };
        TrackedDisposable second = new TrackedDisposable();
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sizeObservedDuringDispose, Is.EqualTo(1));
            Assert.That(first.DisposeCount, Is.EqualTo(1));
            Assert.That(second.DisposeCount, Is.Zero);
            Assert.That(cache.ContainsKey(1), Is.False);
            Assert.That(cache.ContainsKey(2), Is.True);
        }
    }

    /// <summary>Verifies deferred disposal for a multi-entry reduction of MaxSize.</summary>
    [Test]
    [Description("MaxSize disposes all values returned by the virtual shrinking method, after detaching them.")]
    public void ShrinkToMaxSize_ReturnedValuesAreDisposed()
    {
        // ARRANGE
        TrackingCache cache = new TrackingCache(3);
        TrackedDisposable first = new TrackedDisposable();
        TrackedDisposable second = new TrackedDisposable();
        TrackedDisposable third = new TrackedDisposable();
        cache.AddOrModify(1, first);
        Thread.Sleep(_addDelayMs);
        cache.AddOrModify(2, second);
        Thread.Sleep(_addDelayMs);
        cache.AddOrModify(3, third);

        // ACT
        cache.MaxSize = 1;

        // ASSERT
        using (Assert.EnterMultipleScope())
        {
            Assert.That(cache.ShrinkCallCount, Is.EqualTo(1));
            Assert.That(cache.LastDetachedValues, Is.EquivalentTo([first, second]));
            Assert.That(first.DisposeCount, Is.EqualTo(1));
            Assert.That(second.DisposeCount, Is.EqualTo(1));
            Assert.That(third.DisposeCount, Is.Zero);
            Assert.That(cache.CurrentSize, Is.EqualTo(1));
            Assert.That(cache.ContainsKey(3), Is.True);
        }
    }

    /// <summary>Verifies that the caller obeys a derived override's returned disposal list.</summary>
    [Test]
    [Description("An overridden ShrinkToMaxSize controls which detached values are returned for disposal.")]
    public void ShrinkToMaxSize_OverrideControlsDisposalList()
    {
        // ARRANGE
        TrackingCache cache = new TrackingCache(1)
        {
            OmitDetachedValuesFromReturn = true
        };
        TrackedDisposable first = new TrackedDisposable();
        TrackedDisposable second = new TrackedDisposable();
        cache.AddOrModify(1, first);
        Thread.Sleep(_addDelayMs);

        // ACT
        cache.AddOrModify(2, second);

        // ASSERT
        using (Assert.EnterMultipleScope())
        {
            Assert.That(cache.ShrinkCallCount, Is.EqualTo(1));
            Assert.That(cache.LastDetachedValues, Does.Contain(first));
            Assert.That(first.DisposeCount, Is.Zero, "Only values returned from the override should be disposed.");
            Assert.That(cache.ContainsKey(1), Is.False);
            Assert.That(cache.ContainsKey(2), Is.True);
        }

        // The test intentionally withholds a detached owned object from the disposal list.
        first.Dispose();
    }

    /// <summary>Verifies that one failing disposal does not prevent disposing other evicted values.</summary>
    [Test]
    [Description("Shrink eviction removes all selected entries and attempts every disposal even if one throws.")]
    public void ShrinkToMaxSize_DisposalException()
    {
        // ARRANGE
        TrackingCache cache = new TrackingCache(3);
        TrackedDisposable first = new TrackedDisposable { ThrowOnDispose = true };
        TrackedDisposable second = new TrackedDisposable();
        TrackedDisposable third = new TrackedDisposable();
        cache.AddOrModify(1, first);
        Thread.Sleep(_addDelayMs);
        cache.AddOrModify(2, second);
        Thread.Sleep(_addDelayMs);
        cache.AddOrModify(3, third);

        // ACT
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => cache.MaxSize = 1);

        // ASSERT
        using (Assert.EnterMultipleScope())
        {
            Assert.That(exception.InnerException, Is.TypeOf<InvalidOperationException>());
            Assert.That(cache.ShrinkCallCount, Is.EqualTo(1));
            Assert.That(first.DisposeCount, Is.EqualTo(1));
            Assert.That(second.DisposeCount, Is.EqualTo(1));
            Assert.That(third.DisposeCount, Is.Zero);
            Assert.That(cache.CurrentSize, Is.EqualTo(1));
            Assert.That(cache.ContainsKey(1), Is.False);
            Assert.That(cache.ContainsKey(2), Is.False);
            Assert.That(cache.ContainsKey(3), Is.True);
        }
    }

    /// <summary>Verifies eviction when the cache does not own disposable values.</summary>
    [Test]
    [Description("When automatic disposal is disabled, shrinking evicts entries without disposing their values.")]
    public void ShrinkToMaxSize_DisposalDisabled()
    {
        // ARRANGE
        TrackingCache cache = new TrackingCache(1, false);
        TrackedDisposable first = new TrackedDisposable();
        TrackedDisposable second = new TrackedDisposable();
        cache.AddOrModify(1, first);
        Thread.Sleep(_addDelayMs);

        // ACT
        cache.AddOrModify(2, second);

        // ASSERT
        using (Assert.EnterMultipleScope())
        {
            Assert.That(cache.DisposesEvictedValues, Is.False);
            Assert.That(cache.ShrinkCallCount, Is.EqualTo(1));
            Assert.That(cache.LastDetachedValues, Is.Empty);
            Assert.That(first.DisposeCount, Is.Zero);
            Assert.That(second.DisposeCount, Is.Zero);
            Assert.That(cache.CurrentSize, Is.EqualTo(1));
            Assert.That(cache.ContainsKey(1), Is.False);
            Assert.That(cache.ContainsKey(2), Is.True);
        }

        // The caller retains ownership when automatic disposal is disabled.
        first.Dispose();
        second.Dispose();
    }

    /// <summary>Verifies that eviction honors the latest access, not insertion order.</summary>
    [Test]
    [Description("Shrinking evicts the least recently accessed entry and retains an older entry accessed recently.")]
    public void ShrinkToMaxSize_LeastRecentlyAccessed()
    {
        // ARRANGE
        TrackingCache cache = new TrackingCache(3);
        TrackedDisposable first = new TrackedDisposable();
        TrackedDisposable second = new TrackedDisposable();
        TrackedDisposable third = new TrackedDisposable();
        TrackedDisposable fourth = new TrackedDisposable();
        cache.AddOrModify(1, first);
        Thread.Sleep(_addDelayMs);
        cache.AddOrModify(2, second);
        Thread.Sleep(_addDelayMs);
        cache.AddOrModify(3, third);
        Thread.Sleep(_addDelayMs);
        Assert.That(cache.TryGetValue(1, out _), Is.True);
        Thread.Sleep(_addDelayMs);

        // ACT
        cache.AddOrModify(4, fourth);

        // ASSERT
        using (Assert.EnterMultipleScope())
        {
            Assert.That(first.DisposeCount, Is.Zero);
            Assert.That(second.DisposeCount, Is.EqualTo(1));
            Assert.That(third.DisposeCount, Is.Zero);
            Assert.That(fourth.DisposeCount, Is.Zero);
            Assert.That(cache.ContainsKey(1), Is.True);
            Assert.That(cache.ContainsKey(2), Is.False);
            Assert.That(cache.ContainsKey(3), Is.True);
            Assert.That(cache.ContainsKey(4), Is.True);
        }
    }
    #endregion // Tests_ShrinkToMaxSize

    #region Tests_disposal_exceptions_and_contracts

    /// <summary>Verifies that replacing an existing value transfers ownership correctly.</summary>
    [Test]
    [Description("Replacing a cached value disposes the old object exactly once and retains the replacement.")]
    public void SizeLimitedCache_AddOrModify_ReplacementDisposesOldValue()
    {
        // ARRANGE
        SizeLimitedCache<int, TrackedDisposable> cache = new SizeLimitedCache<int, TrackedDisposable>(2);
        TrackedDisposable oldValue = new TrackedDisposable();
        TrackedDisposable newValue = new TrackedDisposable();
        cache.AddOrModify(1, oldValue);

        // ACT
        cache.AddOrModify(1, newValue);

        // ASSERT
        using (Assert.EnterMultipleScope())
        {
            Assert.That(oldValue.DisposeCount, Is.EqualTo(1));
            Assert.That(newValue.DisposeCount, Is.Zero);
            Assert.That(cache.CurrentSize, Is.EqualTo(1));
            Assert.That(cache.TryGetValue(1, out TrackedDisposable retrieved), Is.True);
            Assert.That(retrieved, Is.SameAs(newValue));
        }
    }

    /// <summary>Verifies cleanup when one cached value throws during Clear.</summary>
    [Test]
    [Description("Clear attempts to dispose all values and empties the cache even if a value's Dispose throws.")]
    public void SizeLimitedCache_Clear_DisposalException()
    {
        // ARRANGE
        SizeLimitedCache<int, TrackedDisposable> cache = new SizeLimitedCache<int, TrackedDisposable>(2);
        TrackedDisposable first = new TrackedDisposable { ThrowOnDispose = true };
        TrackedDisposable second = new TrackedDisposable();
        cache.AddOrModify(1, first);
        cache.AddOrModify(2, second);

        // ACT
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => cache.Clear());

        // ASSERT
        using (Assert.EnterMultipleScope())
        {
            Assert.That(exception.InnerException, Is.TypeOf<InvalidOperationException>());
            Assert.That(first.DisposeCount, Is.EqualTo(1));
            Assert.That(second.DisposeCount, Is.EqualTo(1));
            Assert.That(cache.CurrentSize, Is.Zero);
            Assert.That(cache.ContainsKey(1), Is.False);
            Assert.That(cache.ContainsKey(2), Is.False);
        }
    }

    /// <summary>Verifies that Remove detaches an entry before calling its Dispose.</summary>
    [Test]
    [Description("Remove completes dictionary removal even if the removed value throws during disposal.")]
    public void SizeLimitedCache_Remove_DisposalException()
    {
        // ARRANGE
        SizeLimitedCache<int, TrackedDisposable> cache = new SizeLimitedCache<int, TrackedDisposable>(2);
        TrackedDisposable value = new TrackedDisposable { ThrowOnDispose = true };
        cache.AddOrModify(1, value);

        // ACT
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => cache.Remove(1));

        // ASSERT
        using (Assert.EnterMultipleScope())
        {
            Assert.That(exception.Message, Does.Contain("Expected test disposal failure"));
            Assert.That(value.DisposeCount, Is.EqualTo(1));
            Assert.That(cache.CurrentSize, Is.Zero);
            Assert.That(cache.ContainsKey(1), Is.False);
        }
    }

    /// <summary>Verifies that operations cannot use the cache after disposal.</summary>
    [Test]
    [Description("Cache read and write operations throw ObjectDisposedException after disposal.")]
    public void SizeLimitedCache_OperationsAfterDispose_Throw()
    {
        // ARRANGE
        SizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(1);
        cache.AddOrModify(1, "first");
        cache.Dispose();

        // ACT + ASSERT
        using (Assert.EnterMultipleScope())
        {
            Assert.Throws<ObjectDisposedException>(() => { _ = cache.CurrentSize; });
            Assert.Throws<ObjectDisposedException>(() => { _ = cache[1]; });
            Assert.Throws<ObjectDisposedException>(() => cache.TryGetValue(1, out _));
            Assert.Throws<ObjectDisposedException>(() => cache.AddOrModify(2, "second"));
            Assert.Throws<ObjectDisposedException>(() => cache.Remove(1));
            Assert.Throws<ObjectDisposedException>(() => cache.Clear());
            Assert.Throws<ObjectDisposedException>(() => cache.MaxSize = 2);
        }
    }

    /// <summary>Verifies that repeated disposal does not dispose owned values twice.</summary>
    [Test]
    [Description("Repeated calls to Dispose leave the cache disposed and dispose each owned value once.")]
    public void SizeLimitedCache_Disposing_RepeatedCallsAreIdempotent()
    {
        // ARRANGE
        SizeLimitedCache<int, TrackedDisposable> cache = new SizeLimitedCache<int, TrackedDisposable>(2);
        TrackedDisposable first = new TrackedDisposable();
        TrackedDisposable second = new TrackedDisposable();
        cache.AddOrModify(1, first);
        cache.AddOrModify(2, second);

        // ACT
        cache.Dispose();
        cache.Dispose();

        // ASSERT
        using (Assert.EnterMultipleScope())
        {
            Assert.That(cache.IsDisposed, Is.True);
            Assert.That(first.DisposeCount, Is.EqualTo(1));
            Assert.That(second.DisposeCount, Is.EqualTo(1));
        }
    }
    #endregion // Tests_disposal_exceptions_and_contracts

    #region Tests_key_contracts

    /// <summary>Verifies that a caller-supplied comparer determines key equality.</summary>
    [Test]
    [Description("A custom case-insensitive key comparer treats differently cased keys as equal.")]
    public void SizeLimitedCache_CustomKeyComparer()
    {
        // ARRANGE
        SizeLimitedCache<string, string> cache = new SizeLimitedCache<string, string>(2, StringComparer.OrdinalIgnoreCase);
        cache.AddOrModify("Alpha", "original");

        // ACT
        cache.AddOrModify("ALPHA", "replacement");

        // ASSERT
        using (Assert.EnterMultipleScope())
        {
            Assert.That(cache.CurrentSize, Is.EqualTo(1));
            Assert.That(cache.ContainsKey("alpha"), Is.True);
            Assert.That(cache.TryGetValue("aLpHa", out string value), Is.True);
            Assert.That(value, Is.EqualTo("replacement"));
        }
    }

    /// <summary>Verifies argument validation for null reference-type keys.</summary>
    [Test]
    [Description("Operations accepting a key reject null with ArgumentNullException.")]
    public void SizeLimitedCache_NullKey_ThrowsArgumentNullException()
    {
        // ARRANGE
        SizeLimitedCache<string, string> cache = new SizeLimitedCache<string, string>(2);

        // ACT + ASSERT
        using (Assert.EnterMultipleScope())
        {
            Assert.Throws<ArgumentNullException>(() => cache.AddOrModify(null!, "value"));
            Assert.Throws<ArgumentNullException>(() => cache.Remove(null!));
            Assert.Throws<ArgumentNullException>(() => cache.TryGetValue(null!, out _));
            Assert.Throws<ArgumentNullException>(() => { _ = cache[null!]; });
            Assert.Throws<ArgumentNullException>(() => cache[null!] = "value");
        }
    }
    #endregion // Tests_key_contracts
    #endregion // Tests
}
#pragma warning restore IDE0305
#pragma warning restore CA1859    // Change type of variable ...
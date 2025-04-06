// Ignore Spelling: PkUtils, Utils
// 
using PK.PkUtils.DataStructures;
using PK.PkUtils.Interfaces;


namespace PK.PkUtils.NUnitTests.DataStructuresTest;


/// <summary> NUnit tests of generic class SizeLimitedCache. </summary>
[TestFixture()]
public class SizeLimitedCacheTests
{
    #region Fields

    private const int _addDelayMs = 24; // delay needed so the cache will distinguish timestamps
    #endregion // Fields

    #region Tests_constructors

    [Test]
    public void SizeLimitedCache_Constructor_01()
    {
        // ACT + ASSERT
        Assert.Throws<ArgumentOutOfRangeException>(() => new SizeLimitedCache<int, string>(-12));
    }

    [Test]
    public void SizeLimitedCache_Constructor_02()
    {
        // ACT
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(12);
        // ASSERT
        Assert.That(cache.CurrentSize, Is.EqualTo(0));
    }
    #endregion // Tests_constructors

    #region Tests_disposing_cache

    [Test]
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
    public void SizeLimitedCache_Disposing_02()
    {
        // ARRANGE
        // 
        const int nMaxThreads = 16;
        const int nMaxValues = 32;
        var options = new ParallelOptions { MaxDegreeOfParallelism = nMaxThreads };

        static bool IsMutextIsDisposed(Mutex m)
        {
            bool result = false;

            try { m.WaitOne(); }
            catch (ObjectDisposedException) { result = true; }

            return result;
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
            Assert.That(mutexCache.TryGetValue(jj, out Mutex m), Is.True);
            Assert.That(IsMutextIsDisposed(m), Is.False, "Mutex should not be disposed now");
        }

        // 4. Attempt to remove a few items from cache, assert mutex was disposed by that
        //
        for (int jj = 6; jj < 8; jj++)
        {
            Assert.That(mutexCache.Remove(jj), Is.True);
            Assert.That(IsMutextIsDisposed(populateSource[jj].SynchroMutex), Is.True);
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
            Assert.That(IsMutextIsDisposed(x.SynchroMutex), Is.True, "Mutex should be disposed now");
        }
    }
    #endregion // Tests_disposing_cache

    #region Tests_MaxSize

    [Test]
    [TestCase(0)]
    [TestCase(-2)]
    [TestCase(-22)]
    public void SizeLimitedCache_MaxSize_01(int invalidSize)
    {
        // ARRANGE
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(12);

        // ACT + ASSERT
        Assert.Throws<ArgumentOutOfRangeException>(() => cache.MaxSize = invalidSize);
    }

    [Test]
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
    [TestCase(12, 3, 2)]
    [TestCase(200, 230, 4)]
    public void SizeLimitedCache_Indexer_ReturnsExpected(int firstValue, int addedItems, int maxCacheSize)
    {
        int removedItemsCount = addedItems - maxCacheSize;
        IReadOnlyList<int> range = Enumerable.Range(firstValue, addedItems).ToList();
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(maxCacheSize);

        foreach (int x in range)
        {
            cache[x] = x.ToString();
            Thread.Sleep(_addDelayMs);
        }

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
    [TestCase(12, 3)]
    [TestCase(200, 230)]
    public void SizeLimitedCache_Indexer_ThrowsKeyNotFoundException(int firstValue, int addedItems)
    {
        int notPresentKey = firstValue + addedItems;
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(addedItems);

        foreach (int x in Enumerable.Range(firstValue, addedItems))
        {
            cache[x] = x.ToString();
        }

        string v;
        Assert.Throws<KeyNotFoundException>(() => v = cache[notPresentKey]);
    }
    #endregion // Tests_Indexer

    #region Tests_ContainsKey

    [Test]
    [TestCase(12, 3, 2)]
    [TestCase(200, 230, 4)]
    public void SizeLimitedCache_ContainsKey(int firstValue, int addedItems, int maxCacheSize)
    {
        int removedItemsCount = addedItems - maxCacheSize;
        IReadOnlyList<int> range = Enumerable.Range(firstValue, addedItems).ToList();
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(maxCacheSize);

        foreach (int x in range)
        {
            cache.AddOrModify(x, x.ToString());
            Thread.Sleep(_addDelayMs);
        }

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
    [TestCase(-100, 3, 2)]
    [TestCase(200, 23, 4)]
    public void SizeLimitedCache_TryGetValue_01(int firstValue, int addedItems, int maxCacheSize)
    {
        int removedItemsCount = addedItems - maxCacheSize;
        IReadOnlyList<int> range = Enumerable.Range(firstValue, addedItems).ToList();
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(maxCacheSize);

        foreach (int x in range)
        {
            cache.AddOrModify(x, x.ToString());
            Thread.Sleep(_addDelayMs);
        }

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
    [TestCase(3, 2)]
    [TestCase(23, 4)]
    public void SizeLimitedCache_AddOrModify_01(int addedItems, int maxCacheSize)
    {
        IEnumerable<int> range = Enumerable.Range(0, addedItems);
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(maxCacheSize);

        foreach (int x in range)
        {
            cache.AddOrModify(x, x.ToString());
            Thread.Sleep(_addDelayMs);
        }
        Assert.That(cache.CurrentSize, Is.EqualTo(maxCacheSize));

        for (int jj = addedItems - maxCacheSize; jj < addedItems; jj++)
        {
            Assert.That(cache.TryGetValue(jj, out _), Is.True, $"Item with key {jj} not found in the cache.");
        }
    }
    #endregion // Tests_AddOrModify

    #region Tests_Remove

    [Test]
    [TestCase(10)]
    public void SizeLimitedCache_Remove_01(int addedItems)
    {
        IEnumerable<int> range = Enumerable.Range(0, addedItems);
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(24);

        foreach (int x in range)
        {
            cache.AddOrModify(x, x.ToString());
            Thread.Sleep(_addDelayMs);
        }
        foreach (int x in range) Assert.That(cache.Remove(x), Is.True);
        foreach (int x in range) Assert.That(cache.Remove(x), Is.False);

        Assert.That(cache.CurrentSize, Is.EqualTo(0));
    }
    #endregion // Tests_Remove

    #region Tests_Clear

    [Test]
    [TestCase(10)]
    public void SizeLimitedCache_Clear_01(int addedItems)
    {
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(24);
        foreach (int x in Enumerable.Range(0, addedItems)) cache.AddOrModify(x, x.ToString());
        Assert.That(cache.CurrentSize, Is.EqualTo(addedItems));

        cache.Clear();
        Assert.That(cache.CurrentSize, Is.EqualTo(0));
    }
    #endregion // Tests_Clear
}

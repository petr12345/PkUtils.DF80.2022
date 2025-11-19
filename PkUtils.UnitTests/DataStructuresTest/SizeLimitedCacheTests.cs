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

/// <summary> Unit Test of generic class SizeLimitedCache. </summary>
[TestClass()]
public class SizeLimitedCacheTests
{
    #region Fields

    private const int _addDelayMs = 24; // delay needed so the cache will distinguish timestamps
    [TestMethod]
    public void SizeLimitedCache_Constructor_01()
    {
        // ACT + ASSERT
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
        {
            new SizeLimitedCache<int, string>(-12);
        });
    }

    [TestMethod]
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

    [TestMethod()]
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
            Assert.IsFalse(IsMutextIsDisposed(m), "Mutex should not be disposed now");
        }

        // 4. Attempt to remove a few items from cache, assert mutex was disposed by that
        //
        for (int jj = 6; jj < 8; jj++)
        {
            Assert.IsTrue(mutexCache.Remove(jj));
            Assert.IsTrue(IsMutextIsDisposed(populateSource[jj].SynchroMutex));
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
            Assert.IsTrue(IsMutextIsDisposed(x.SynchroMutex), "Mutex should be disposed now");
        }
    }
    [TestMethod]
    [DataRow(0)]
    [DataRow(-2)]
    [DataRow(-22)]
    public void SizeLimitedCache_MaxSize_01(int invalidSize)
    {
        // ARRANGE
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(12);
        // ACT + ASSERT
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
        {
            cache.MaxSize = invalidSize;
        });
    }

    [TestMethod]
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
    [DataRow(12, 3, 2)]
    [DataRow(200, 230, 4)]
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
            Assert.IsFalse(cache.ContainsKey(range[ii]), $"value {range[ii]} should not be there");
        }
        for (int jj = removedItemsCount; jj < addedItems; jj++)
        {
            int key = range[jj];
            Assert.AreEqual(key.ToString(), cache[key]);
        }
    }

    [TestMethod]
    [DataRow(12, 3)]
    [DataRow(200, 230)]
    public void SizeLimitedCache_Indexer_ThrowsKeyNotFoundException(int firstValue, int addedItems)
    {
        int notPresentKey = firstValue + addedItems;
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(addedItems);

        foreach (int x in Enumerable.Range(firstValue, addedItems))
        {
            cache[x] = x.ToString();
        }

        Assert.ThrowsExactly<KeyNotFoundException>(() =>
        {
            var v = cache[notPresentKey];
        });
    }
    #endregion // Tests_Indexer

    #region Tests_ContainsKey

    [TestMethod]
    [DataRow(12, 3, 2)]
    [DataRow(200, 230, 4)]
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
    [DataRow(-100, 3, 2)]
    [DataRow(200, 23, 4)]
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
    [DataRow(3, 2)]
    [DataRow(23, 4)]
    public void SizeLimitedCache_AddOrModify_01(int addedItems, int maxCacheSize)
    {
        IEnumerable<int> range = Enumerable.Range(0, addedItems);
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(maxCacheSize);

        foreach (int x in range)
        {
            cache.AddOrModify(x, x.ToString());
            Thread.Sleep(_addDelayMs);
        }
        Assert.AreEqual(maxCacheSize, cache.CurrentSize);

        for (int jj = addedItems - maxCacheSize; jj < addedItems; jj++)
        {
            Assert.IsTrue(cache.TryGetValue(jj, out _));
        }
    }
    #endregion // Tests_AddOrModify

    #region Tests_Remove

    [TestMethod]
    [DataRow(10)]
    public void SizeLimitedCache_Remove_01(int addedItems)
    {
        IEnumerable<int> range = Enumerable.Range(0, addedItems);
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(24);

        foreach (int x in range)
        {
            cache.AddOrModify(x, x.ToString());
            Thread.Sleep(_addDelayMs);
        }
        foreach (int x in range) Assert.IsTrue(cache.Remove(x));
        foreach (int x in range) Assert.IsFalse(cache.Remove(x));

        Assert.AreEqual(0, cache.CurrentSize);
    }
    #endregion // Tests_Remove

    #region Tests_Clear

    [TestMethod]
    [DataRow(10)]
    public void SizeLimitedCache_Clear_01(int addedItems)
    {
        ISizeLimitedCache<int, string> cache = new SizeLimitedCache<int, string>(24);
        foreach (int x in Enumerable.Range(0, addedItems)) cache.AddOrModify(x, x.ToString());
        Assert.AreEqual(addedItems, cache.CurrentSize);

        cache.Clear();
        Assert.AreEqual(0, cache.CurrentSize);
    }
    #endregion // Tests_Clear
}

#pragma warning restore CA1859
#pragma warning restore CA1806
#pragma warning restore IDE0305
#pragma warning restore IDE0079
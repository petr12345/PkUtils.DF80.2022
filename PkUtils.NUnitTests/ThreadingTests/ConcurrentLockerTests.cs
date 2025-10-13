// Ignore Spelling: Utils

using System.Data;
using PK.PkUtils.Interfaces;
using PK.PkUtils.Threading;
using PK.PkUtils.Utils;


namespace PK.PkUtils.NUnitTests.ThreadingTests;


#pragma warning disable IDE0079    // Remove unnecessary suppressions
#pragma warning disable CA1859     // Change type of variable ...
#pragma warning disable VSTHRD200  // Use "Async" suffix for async methods

/// <summary> NUnit tests of generic class ConcurrentLocker. </summary>
[TestFixture()]
public class ConcurrentLockerTests
{
    #region Fields

    private const int _constLockId = 777;
    private const int _dayTimeout = 24 * 3600 * 1000;
    #endregion // Fields

    #region Tests_constructors

    [Test]
    public void Constructor_1st_Succeeds()
    {
        using (Assert.EnterMultipleScope())
        {
            // ACT
            IConcurrentLocker<string> locker = new ConcurrentLocker<string>();

            // ASSERT
            Assert.That(locker.CurrentSize, Is.Zero);
            Assert.That(locker.IsDisposed, Is.False);
        }
    }

    [Test]
    public void Constructor_1st_ThrowsExpectedException()
    {
        using (Assert.EnterMultipleScope())
        {
            // ARRANGE
            Func<string, string> fnErrorMessage = null!;

            // ACT + ASSERT
            Assert.Throws<ArgumentNullException>(() => new ConcurrentLocker<string>(fnErrorMessage));
        }
    }

    [Test]
    public void Constructor_2nd_Succeeds()
    {
        using (Assert.EnterMultipleScope())
        {
            // ACT
            IConcurrentLocker<string> locker = new ConcurrentLocker<string>(StringComparer.OrdinalIgnoreCase);

            // ASSERT
            Assert.That(locker.CurrentSize, Is.Zero);
            Assert.That(locker.IsDisposed, Is.False);
        }
    }

    [Test]
    public void Constructor_3rd_Succeeds()
    {
        using (Assert.EnterMultipleScope())
        {
            // ACT
            IConcurrentLocker<string> locker = new ConcurrentLocker<string>(StringComparer.OrdinalIgnoreCase);

            // ASSERT
            Assert.That(locker.CurrentSize, Is.Zero);
            Assert.That(locker.IsDisposed, Is.False);
        }
    }
    #endregion // Tests_constructors

    #region Tests_disposing

    [Test]
    [TestCase(1)]
    [TestCase(3)]
    public void Disposing_01(int numDispose)
    {
        using (Assert.EnterMultipleScope())
        {
            // ARRANGE
            IConcurrentLocker<string> locker = new ConcurrentLocker<string>();

            // ACT + ASSERT
            for (int ii = 0; ii < numDispose; ii++)
            {
                locker.Dispose();
                Assert.That(locker.IsDisposed, Is.True);
            }
        }
    }

    [Test]
    [TestCase(1)]
    [TestCase(16)]
    [TestCase(32)]
    public async Task Disposing_02(int numLocks)
    {
        using (Assert.EnterMultipleScope())
        {
            // ARRANGE
            IEnumerable<int> range = Enumerable.Range(0, numLocks);
            IConcurrentLocker<int> locker = new ConcurrentLocker<int>();

            // ACT
            var tasks = range.Select(x => locker.LockAsync(x)).ToList();
            IDisposable[] locks = await Task.WhenAll(tasks);

            Assert.That(locker.CurrentSize, Is.EqualTo(numLocks));
            locker.Dispose();

            // ASSERT
            Assert.That(locker.IsDisposed, Is.True);
            Assert.That(locker.CurrentSize, Is.Zero);
        }
    }
    #endregion // Tests_disposing

    #region Tests_LockAsync_WithOnlyIdArgument

    [TestCase(1)]
    [TestCase(555)]
    public async Task LockAsync_WithOnlyIdArgument_SingleLock(int itemId)
    {
        using (Assert.EnterMultipleScope())
        {
            // ARRANGE
            IConcurrentLocker<string> locker = new ConcurrentLocker<string>();

            // ACT + ASSERT
            IDisposable myLock = await locker.LockAsync(itemId.ToString());
            Assert.That(locker.CurrentSize, Is.EqualTo(1));

            myLock.Dispose();
            Assert.That(myLock, Is.InstanceOf<IDisposableEx>().Or.Null);
            Assert.That((myLock as IDisposableEx)?.IsDisposed, Is.True);
            Assert.That(locker.CurrentSize, Is.Zero);
        }
    }

    [Test]
    [TestCase(1)]
    [TestCase(3)]
    [TestCase(128)]
    public async Task LockAsync_WithOnlyIdArgument_SeveralParallelLocks_ShouldWork(int numLocks)
    {
        using (Assert.EnterMultipleScope())
        {
            // ARRANGE
            IEnumerable<int> range = Enumerable.Range(0, numLocks);
            IConcurrentLocker<int> locker = new ConcurrentLocker<int>();

            // ACT
            var tasks = range.Select(x => locker.LockAsync(x)).ToList();
            IDisposable[] locks = await Task.WhenAll(tasks);

            Assert.That(locker.CurrentSize, Is.EqualTo(numLocks));

            locks.AsParallel().ForAll(x => x.Dispose());

            // ASSERT
            Assert.That(locks.All(x => x is IDisposableEx { IsDisposed: true }));
            Assert.That(locker.CurrentSize, Is.Zero);
        }
    }

    [Test]
    public void LockAsync_WithOnlyIdArgument_SequentialCallShouldWork()
    {
        using (Assert.EnterMultipleScope())
        {
            // ARRANGE
            IUsageCounter counter = new UsageCounter();
            using IConcurrentLocker<int> locker = new ConcurrentLocker<int>();
            // ACT
            Assert.DoesNotThrowAsync(async () =>
            {
                await RunLockWithInfiniteTimeoutAndFiniteSleep(locker, _constLockId, counter);
                await RunLockWithInfiniteTimeoutAndFiniteSleep(locker, _constLockId, counter);
                await RunLockWithInfiniteTimeoutAndFiniteSleep(locker, _constLockId, counter);
            });

            // ASSERT
            Assert.That(locker.CurrentSize, Is.Zero);
            Assert.That(locker.IsDisposed, Is.False);
            Assert.That(counter.AddReference(), Is.EqualTo(4));
        }
    }
    #endregion // Tests_LockAsync_WithOnlyIdArgument

    #region Tests_LockAsync_Timeout

    [Test]
    [TestCase(-2)]
    [TestCase(-22)]
    public void LockAsync_FiniteTimeout_InvalidValueThrowsException(int msTimeout)
    {
        using (Assert.EnterMultipleScope())
        {
            // ARRANGE
            IUsageCounter counter = new UsageCounter();
            const int sleepMs = 2;

            using IConcurrentLocker<int> locker = new ConcurrentLocker<int>();
            // ACT + ASSERT
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await RunLockWithFiniteTimeoutAndFiniteSleep(locker, _constLockId, counter, msTimeout, sleepMs);
            });
            Assert.That(counter.AddReference(), Is.EqualTo(1));
        }
    }

    [Test]
    [TestCase(0)]
    [TestCase(10)]
    [TestCase(64)]
    [TestCase(256)]
    [Description("Makes sure the first thread who acquired the lock keeps it long enough to prevent the other thread acquiring that")]
    public void LockAsync_FiniteTimeout_SeveralParallelLocks_ShouldThrowException(int msTimeout)
    {
        using (Assert.EnterMultipleScope())
        {
            // ARRANGE
            IUsageCounter counter = new UsageCounter();
            int sleepMs = Math.Max(2 * msTimeout, 32);
            using IConcurrentLocker<int> locker = new ConcurrentLocker<int>();

            // ACT + ASSERT
            Assert.ThrowsAsync<ConcurrencyConflictException>(async () =>
            {
                await Task.WhenAll(
                    RunLockWithFiniteTimeoutAndFiniteSleep(locker, _constLockId, counter, msTimeout, sleepMs),
                    RunLockWithFiniteTimeoutAndFiniteSleep(locker, _constLockId, counter, msTimeout, sleepMs));
            });
            Assert.That(counter.AddReference(), Is.EqualTo(2));
        }
    }

    [Test]
    [TestCase(1, 1)]
    [TestCase(1, 12)]
    [TestCase(12, 1)]
    [TestCase(12, 12)]
    public void LockAsync_FiniteTimeoutZero_SequentialLocks_ShouldWork(int sleepMs, int locks)
    {
        using (Assert.EnterMultipleScope())
        {
            // ARRANGE
            IUsageCounter counter = new UsageCounter();
            const int msTimeout = 0;
            using IConcurrentLocker<int> locker = new ConcurrentLocker<int>();

            // ACT
            Assert.DoesNotThrowAsync(async () =>
            {
                for (int ii = 0; ii < locks; ii++)
                {
                    await RunLockWithFiniteTimeoutAndFiniteSleep(locker, _constLockId, counter, msTimeout, sleepMs);
                }
            });

            // ASSERT
            Assert.That(locker.CurrentSize, Is.Zero);
            Assert.That(locker.IsDisposed, Is.False);
            Assert.That(counter.AddReference(), Is.EqualTo(locks + 1));
        }
    }
    #endregion // Tests_LockAsync_Timeout

    #region Tests_LockAsync_Token_and_Timeout

    [Test]
    [TestCase(0, 1)]
    [TestCase(10, 2)]
    [TestCase(64, 3)]
    [TestCase(0, 1)]
    [TestCase(10, 2)]
    [TestCase(64, 3)]
    public void LockAsync_ConcurrentLockWithToken_TimeOutInfinite_ShouldWork(int sleepMs, int numTasks)
    {
        using (Assert.EnterMultipleScope())
        {
            // ARRANGE
            const int waitTimeout = _dayTimeout;
            const int tokenTimeout = _dayTimeout;
            IEnumerable<int> range = Enumerable.Range(0, numTasks);
            var token = new CancellationTokenSource(TimeSpan.FromMilliseconds(tokenTimeout)).Token;
            using IConcurrentLocker<int> locker = new ConcurrentLocker<int>();

            // ACT + ASSERT
            Task[] tasks = [.. range.Select(x => RunLockWithFiniteTimeoutAndTokenAndSleep(
                locker, _constLockId, waitTimeout, sleepMs, token))];

            Assert.DoesNotThrowAsync(async () => await Task.WhenAll(tasks));
            Assert.That(locker.CurrentSize, Is.Zero);
        }
    }

    [Test]
    [TestCase(0)]
    [TestCase(10)]
    [TestCase(64)]
    [TestCase(300)]
    [Description("Makes sure the first thread who acquired the lock keeps it long enough to prevent the other thread acquiring that")]
    public void LockAsync_ConcurrentLockWithToken_TimeOutFinite_ShouldThrowException(int msTimeout)
    {
        using (Assert.EnterMultipleScope())
        {
            // ARRANGE
            int sleepMs = Math.Max(2 * msTimeout, 64);
            var token = new CancellationTokenSource(TimeSpan.FromMilliseconds(_dayTimeout)).Token;
            using IConcurrentLocker<int> locker = new ConcurrentLocker<int>();

            // ACT + ASSERT
            Assert.ThrowsAsync<ConcurrencyConflictException>(async () =>
            {
                await Task.WhenAll(
                    RunLockWithFiniteTimeoutAndTokenAndSleep(locker, _constLockId, msTimeout, sleepMs, token),
                    RunLockWithFiniteTimeoutAndTokenAndSleep(locker, _constLockId, msTimeout, sleepMs, token));
            });
        }
    }

    [Test]
    [TestCase(10)]
    [TestCase(64)]
    [TestCase(128)]
    [Description("Makes sure the first thread who acquired the lock keeps it long enough to prevent the other thread acquiring that")]
    public void LockAsync_ConcurrentLockWithToken_TokenTimeOutFinite_ShouldThrowException(int msTokenTimeout)
    {
        using (Assert.EnterMultipleScope())
        {
            // ARRANGE
            int sleepMs = Math.Max(4 * msTokenTimeout, 64);
            var token = new CancellationTokenSource(TimeSpan.FromMilliseconds(msTokenTimeout)).Token;
            using IConcurrentLocker<int> locker = new ConcurrentLocker<int>();

            // ACT + ASSERT
            Assert.ThrowsAsync<ConcurrencyConflictException>(async () =>
            {
                await Task.WhenAll(
                    RunLockWithFiniteTimeoutAndTokenAndSleep(locker, _constLockId, _dayTimeout, sleepMs, token),
                    RunLockWithFiniteTimeoutAndTokenAndSleep(locker, _constLockId, _dayTimeout, sleepMs, token));
            });
        }
    }
    #endregion // Tests_LockAsync_Token_and_Timeout

    #region Auxiliary_Methods

    private static Task RunLockWithInfiniteTimeoutAndFiniteSleep(
        IConcurrentLocker<int> locker,
        int lockId,
        IUsageCounter executionCounter = null!,
        int sleepMs = 32)
    {
        return Task.Run(async () =>
        {
            using (await locker.LockAsync(lockId))
            {
                Thread.Sleep(sleepMs);
                executionCounter?.AddReference();
            }
        });
    }

    private static Task RunLockWithFiniteTimeoutAndFiniteSleep(
        IConcurrentLocker<int> locker,
        int lockId,
        IUsageCounter executionCounter,
        int msTimeOut,
        int sleepMs)
    {
        return Task.Run(async () =>
        {
            using (await locker.LockAsync(lockId, msTimeOut))
            {
                Thread.Sleep(sleepMs);
                executionCounter?.AddReference();
            }
        });
    }

    private static Task RunLockWithFiniteTimeoutAndTokenAndSleep(
        IConcurrentLocker<int> locker,
        int lockId,
        int msTimeOut,
        int sleepMs,
        CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            using (await locker.LockAsync(lockId, msTimeOut, cancellationToken))
            {
                Thread.Sleep(sleepMs);
            }
        }, cancellationToken); // <-- Forward the cancellationToken here
    }
    #endregion // Auxiliary_Methods
}

#pragma warning restore VSTHRD200
#pragma warning restore CA1859
#pragma warning restore IDE0079

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.Threading;
using PK.PkUtils.Utils;

namespace PK.TestConcurrency
{
    /// <summary>
    /// The class actually doing the test
    /// </summary>
    public static class ConcurrentTest
    {
        #region Typedefs

        public enum TestMode
        {
            NoneLock = 0,
            InterlockedLock = 1,
            MonitorLock = 2,
            ReaderWriterLock_Basic = 3,
            ReaderWriterLock_Wrapper = 4,
            ReaderWriterLockSli_Basic = 5,
            ReaderWriterLockSli_Wrapper = 6,
        }
        #endregion // Typedefs

        #region Fields
        public const int CONCURRENT_THREADS_COUNT = 100;
        public const int LOOP_COUNT = 100000;
        #endregion // Fields

        #region Methods

        public static void DoTest(TestMode mode, IDumper dumper)
        {
            dumper.CheckArgNotNull(nameof(dumper));

            if (!Enum.IsDefined(typeof(TestMode), mode))
            {
                string strErr = string.Format(CultureInfo.InvariantCulture, "Invalid value of mode = '{0}'", mode);
                throw new ArgumentException(strErr, "mode");
            }
            else
            {
                dumper.DumpLine(string.Format(CultureInfo.InvariantCulture, "---- Testing '{0}'...", mode));
            }

            int expected = CONCURRENT_THREADS_COUNT * LOOP_COUNT;
            int counter = 0;
            ThreadStart ts = null;

            switch (mode)
            {
                case TestMode.NoneLock:
                    {
                        ts = new ThreadStart(delegate
                        {
                            for (int j = 0; j < LOOP_COUNT; j++)
                            {
                                counter++;
                            }
                        });
                    }
                    break;

                case TestMode.MonitorLock:
                    {
                        object o = new();
                        ts = new ThreadStart(delegate
                        {
                            for (int j = 0; j < LOOP_COUNT; j++)
                            {
                                lock (o) { counter++; }
                            }
                        });
                    }
                    break;

                case TestMode.InterlockedLock:
                    {
                        ts = new ThreadStart(delegate
                        {
                            for (int j = 0; j < LOOP_COUNT; j++)
                            {
                                Interlocked.Increment(ref counter);
                            }
                        });
                    }
                    break;

                case TestMode.ReaderWriterLock_Basic:
                    {
                        ReaderWriterLock rwLock = new();
                        ts = new ThreadStart(delegate
                        {
                            for (int j = 0; j < LOOP_COUNT; j++)
                            {
                                try
                                {
                                    rwLock.AcquireWriterLock(Timeout.Infinite);
                                    counter++;
                                }
                                finally
                                {
                                    rwLock.ReleaseWriterLock();
                                }
                            }
                        });
                    }
                    break;

                case TestMode.ReaderWriterLock_Wrapper:
                    {
                        var wrapper = new PK.PkUtils.Threading.RWLockWrapper();
                        ts = new ThreadStart(delegate
                        {
                            for (int j = 0; j < LOOP_COUNT; j++)
                            {
                                using (IDisposable disp = wrapper.AcquireWriterLock())
                                {
                                    counter++;
                                }
                            }
                        });
                    }
                    break;

                case TestMode.ReaderWriterLockSli_Basic:
                    {
                        ReaderWriterLockSlim slim = new(LockRecursionPolicy.SupportsRecursion);
                        ts = new ThreadStart(delegate
                        {
                            for (int j = 0; j < LOOP_COUNT; j++)
                            {
                                try
                                {
                                    slim.EnterWriteLock();
                                    counter++;
                                }
                                finally
                                {
                                    slim.ExitWriteLock();
                                }
                            }
                        });
                    }
                    break;

                case TestMode.ReaderWriterLockSli_Wrapper:
                    {
                        ReaderWriterLockSlim slim = new(LockRecursionPolicy.SupportsRecursion);
                        ts = new ThreadStart(delegate
                        {
                            for (int j = 0; j < LOOP_COUNT; j++)
                            {
                                using (new SlimLockWriterGuard(slim))
                                {
                                    counter++;
                                }
                            }
                        });
                    }
                    break;
            }

            Thread[] threads = new Thread[CONCURRENT_THREADS_COUNT];
            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i] = new Thread(ts);
            }
            Stopwatch stopWatch = new();
            stopWatch.Start();
            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i].Start();
            }
            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i].Join();
            }
            stopWatch.Stop();
            dumper.DumpLine(string.Format(CultureInfo.InvariantCulture, "counter value: {0:#,##0}", counter));
            dumper.DumpLine(string.Format(CultureInfo.InvariantCulture, "expected value: {0:#,##0}", expected));
            dumper.DumpLine(string.Format(CultureInfo.InvariantCulture, "time passed: {0}(ms)",
              Conversions.LongToReadable(stopWatch.ElapsedMilliseconds)));
        }
        #endregion // Methods
    }
}

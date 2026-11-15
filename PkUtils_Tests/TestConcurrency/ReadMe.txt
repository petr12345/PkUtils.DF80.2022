TestConcurrency compares several approaches to synchronizing concurrent
increments of a shared counter. It runs multiple threads and reports the final
counter value, the expected value, and elapsed time.

The available test modes in ConcurrentTest.TestMode are:
  a) NoneLock                  - no synchronization (a deliberately unsafe case)
  b) InterlockedLock           - Interlocked.Increment
  c) MonitorLock               - lock on an ordinary object (Monitor)
  d) ReaderWriterLock_Basic    - ReaderWriterLock
  e) ReaderWriterLock_Wrapper  - RWLockWrapper
  f) ReaderWriterLockSli_Basic - ReaderWriterLockSlim
  g) ReaderWriterLockSli_Wrapper - SlimLockWriterGuard around ReaderWriterLockSlim

The purpose is to illustrate correctness and relative costs. Timing results
are machine-, runtime-, and workload-dependent; they should not be treated
as universal performance measurements. The MonitorLock mode intentionally
uses object/Monitor rather than System.Threading.Lock.

For details, see ConcurrentTest.DoTest() and MainForm.cs.

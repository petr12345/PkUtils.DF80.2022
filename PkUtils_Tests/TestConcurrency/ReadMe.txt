The project TestConcurrency compares thread synchronization techniques
with various synchronization objects.

Currently used synchronization approaches:
a/ none synchronization ( produces unexpected results )
b/ Interlocked.Increment
c/ ReaderWriterLock
d/ ReaderWriterLock wrapped in the class RWLockWrapper
e/ ReaderWriterLockSlim

Notice the huge difference of time consumed by ReaderWriterLock ( c/ and d/ ),
comparing with the other synch primitives. 
The ReaderWriterLockSlim ( e/ ) is not very fast, but still looks good enough.

For more details see the code of static class ConcurrentTest.DoTest
  
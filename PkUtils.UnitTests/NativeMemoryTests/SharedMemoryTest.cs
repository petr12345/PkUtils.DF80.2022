// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.NativeMemory;
using PK.PkUtils.Threading;

#nullable enable

namespace PK.PkUtils.UnitTests.NativeMemoryTests;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable IDE0290 // Use primary constructor
#pragma warning disable CA1806   // suppress "result of '... call is not used"
#pragma warning disable MSTEST0051 // Assert.Throws should contain only a single statement
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods

/// <summary>
/// Contains tests for shared memory usage with <see cref="Segment"/> class.
/// </summary>
[TestClass()]
public class SharedMemoryTest
{
    #region Typedefs

    /// <summary> Thread reading the person data from shared memory. </summary>
    internal sealed class SegmentReadThread : WorkerThread
    {
        private readonly string _mappingName;
        private PersonData? _resultData;

        public SegmentReadThread(string mappingName)
            : base(attach: false, createWaitExitEvent: false)
        {
            _mappingName = mappingName;
        }

        public PersonData? ResultData { get => _resultData; }

        protected override void WorkerFunction()
        {
            using Segment segmentAttached = new(_mappingName, true);
            _resultData = (PersonData)segmentAttached.GetData();
        }
    }
    #endregion // Typedefs

    #region Methods

    private static void WaitAll(IEnumerable<Thread> threads)
    {
        if (threads != null)
        {
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }
    }
    #region Tests_constructors

    /// <summary>
    /// A test for Segment constructor, which should fail with ArgumentNullException
    /// </summary>
    [TestMethod()]
    public void Segment_Constructor_01()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            new Segment(null, SharedMemoryCreationFlag.Create, 20, true);
        });
    }

    /// <summary>
    /// A test for Segment constructor, which should fail with ArgumentException
    /// </summary>
    [TestMethod()]
    public void Segment_Constructor_02()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            new Segment(string.Empty, SharedMemoryCreationFlag.Create, 20, true);
        });
    }

    /// <summary>
    /// A test for Segment constructor, which should succeed for all names 
    /// with length up to <see cref="Segment.MaxMappingNameLength"/>.
    /// </summary>
    [TestMethod()]
    public void Segment_Constructor_03()
    {
        int maxMappingNameLength = Segment.MaxMappingNameLength;

        for (int ii = 1; ii <= maxMappingNameLength; ii++)
        {
            string mappingName = new('x', ii);
            using Segment tempSegment = new(mappingName, SharedMemoryCreationFlag.Create, 20, true);
            Assert.IsTrue(tempSegment.IsSynchronized);
        }
    }

    /// <summary>
    /// A test for Segment constructor, which should raise ArgumentExceptio for mapping name 
    /// longer than <see cref="Segment.MaxMappingNameLength"/>.
    /// </summary>
    [TestMethod()]
    public void Segment_Constructor_04()
    {
        int maxMappingNameLength = Segment.MaxMappingNameLength;
        string mappingName = new('x', maxMappingNameLength + 1);
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            new Segment(mappingName, SharedMemoryCreationFlag.Create, 20, true);
        });
    }

    /// <summary>
    /// A test for Segment constructor, which should fail with ArgumentException
    /// </summary>
    [TestMethod()]
    public void Segment_Constructor_05()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
        {
            new Segment("abc", SharedMemoryCreationFlag.Create, -3, true);
        });
    }
    #endregion // Tests_constructors

    #region Tests_write_read_singlethread

    /// <summary>
    /// A test for Segment writing and reading back.
    /// Should succeed with no exception thrown.
    /// </summary>
    [TestMethod()]
    public void Segment_WriteRead_SingleThread_01()
    {
        string mappingName = Guid.NewGuid().ToString();
        PersonData p1 = new(37, "Tronald Dump");
        PersonData p2;

        using (Segment s1 = new(mappingName, p1, true))
        {
            using Segment s2 = new(mappingName, true);
            p2 = (PersonData)s2.GetData();
        }

        Assert.IsTrue(p2.Equals(p1));
    }

    /// <summary>
    /// A test for Segment writing and reading back, with several lock applied in both segments.
    /// Should succeed with no exception thrown.
    /// </summary>
    ///
    /// <remarks>
    /// The.NET System.Threading.Mutex class uses a Win32 kernel mutex object which is re-entrant.
    /// The following note from the documentation explains correct usage: 
    /// "The thread that owns a mutex can request the same mutex in repeated calls to WaitOne
    /// without blocking its execution.
    /// However, the thread must call the ReleaseMutex method the same number of times to
    /// release ownership of the mutex.".
    /// </remarks>
    [TestMethod()]
    public void Segment_WriteRead_SingleThread_02()
    {
        string mappingName = Guid.NewGuid().ToString();
        string personName = "Liška podšitá";
        PersonData pA = new(222, personName);
        PersonData pB;

        using (Segment sA = new(mappingName, pA, true))
        {
            using IDisposable lockA1 = sA.AcquireLock();
            using IDisposable lockA2 = sA.AcquireLock();
            using Segment sB = new(mappingName, true);
            using IDisposable lockB1 = sB.AcquireLock();
            using IDisposable lockB2 = sB.AcquireLock();
            pB = (PersonData)sB.GetData();
        }

        Assert.IsTrue(pB.Equals(pA));
    }

    /// <summary>
    /// A test for Segment s1 writing and s2 reading back after the original segment is disposed, 
    /// which should succeed ( because s2 is constructed before s1 disposal ).
    /// </summary>
    [TestMethod()]
    public void Segment_WriteRead_SingleThread_03()
    {
        string mappingName = Guid.NewGuid().ToString();
        PersonData p1 = new(22, "John Smith");
        PersonData p2;

        using (Segment s1 = new(mappingName, p1, true))
        {
            using Segment s2 = new(mappingName, true);
            s1.Dispose();

            using IDisposable lockB2 = s2.AcquireLock();
            p2 = (PersonData)s2.GetData();
        }

        Assert.IsTrue(p2.Equals(p1));
    }

    /// <summary>
    /// A test for Segment writing and reading, which should throw SharedMemoryException,
    /// because attaching to non-existing file mapping.
    /// </summary>
    [TestMethod()]
    public void Segment_WriteRead_SingleThread_04()
    {
        // Following should throw SharedMemoryException from the constructor
        // s2 = new Segment(mappingName2
        // because it requires attaching to non-existing file mapping "PersonDataSegment2"
        //

        string mappingName1 = Guid.NewGuid().ToString();
        string mappingName2 = Guid.NewGuid().ToString();
        PersonData p1 = new(73, "XX YY");
        PersonData p2;

        using Segment s1 = new(mappingName1, p1, true);
        Assert.ThrowsExactly<SharedMemoryException>(() =>
        {
            using Segment s2 = new(mappingName2, true);
            p2 = (PersonData)s2.GetData();
        });
    }
    #endregion // Tests_write_read_singlethread

    #region Tests_write_read_multithread

    /// <summary>
    /// A test for Segment writing and reading back in a separate thread, without additional locking.
    /// Should succeed with no exception thrown.
    /// </summary>
    [TestMethod()]

    public void Segment_WriteRead_MultiThread_01()
    {
        string mappingName = Guid.NewGuid().ToString();
        PersonData personWritten = new(37, "Oliver Twist");
        PersonData? personRead = null;

        using (Segment segmentWritten = new(mappingName, personWritten, true))
        {
            using SegmentReadThread thread = new(mappingName);
            thread.Start();
            thread.Join();
            personRead = thread.ResultData;
        }

        Assert.IsTrue(personWritten.Equals(personRead));
    }

    /// <summary>
    /// A test for Segment writing and reading back in a separate thread, with additional locking.
    /// Should succeed with no exception thrown.
    /// </summary>
    [TestMethod()]
    public void Segment_WriteRead_MultiThread_02()
    {
        string mappingName = Guid.NewGuid().ToString();
        PersonData personWritten = new(63, "Sir Arthur Conan Doyle");
        ManualResetEvent acquiredLockOnce;
        Segment segmentWritten;
        PersonData? personRead = null;

        // Local method locking and unlocking the written segment
        void ThreadLockAndUnlockUnlockSegment()
        {
            using IDisposable temporaryLock = segmentWritten.AcquireLock();
            acquiredLockOnce.Set();
            Thread.Sleep(256);
        }

        // Local method reading the person data from shared memory, creating attached segment
        void ThreadReadPersonData()
        {
            // To fulfill the purpose of the test, make sure the other thread locks the segmentWritten
            acquiredLockOnce.WaitOne();

            // only after that try reading
            using Segment segmentAttached = new(mappingName, true);
            personRead = (PersonData)segmentAttached.GetData();
        }

        // the main body
        using (acquiredLockOnce = new ManualResetEvent(false))
        {
            using (segmentWritten = new Segment(mappingName, personWritten, true))
            {
                Thread threadLockUnlock = new(new ThreadStart(ThreadLockAndUnlockUnlockSegment));
                Thread threadRead = new(new ThreadStart(ThreadReadPersonData));
                Thread[] threads = [threadRead, threadLockUnlock];

                threadLockUnlock.Start();
                threadRead.Start();
                WaitAll(threads);
            }
        }

        Assert.IsNotNull(personRead);
        Assert.IsTrue(personWritten.Equals(personRead));
    }

    /// <summary>
    /// A test for Segment writing and reading back in a separate thread, with additional locking.
    /// Should throw AbandonedMutexException.
    /// </summary>
    [TestMethod()]
    public void Segment_WriteRead_MultiThread_03()
    {
        string mappingName = Guid.NewGuid().ToString();
        PersonData personWritten = new(int.MaxValue, "Mark Twain");
        ManualResetEvent acquiredLockOnce;
        Segment segmentWritten;
        PersonData? personRead = null;

        // local method locking and unlocking the written segment
        void ThreadLockAndUnlockUnlockSegment()
        {
            IDisposable temporaryLock = segmentWritten.AcquireLock();
            acquiredLockOnce.Set();

            // Just exit the thread, without disposing temporaryLock.
            // This will cause that the wrapped mutex acquired by this thread is never released by ReleaseMutex();
            // hence the constructor of another segment in the thread below will experience AbandonedMutexException,
            // thrown from constructor of base class BaseSegment
            // 
            // 
            // Note: One could do also following to enforce the thread immediately on Win32 level;
            // but that seems to cause instability of the unit testing engine.
            // 
            // IntPtr hThread = WinApi.Kernel32.GetCurrentThread();
            // WinApi.Kernel32.TerminateThread(hThread, 3333);
        }

        // the main body
        using (acquiredLockOnce = new ManualResetEvent(false))
        {
            using (segmentWritten = new Segment(mappingName, personWritten, true))
            {
                Thread threadLockUnlock = new(new ThreadStart(ThreadLockAndUnlockUnlockSegment));
                threadLockUnlock.Start();

                // make sure the other thread locks the segmentWritten
                acquiredLockOnce.WaitOne();

                threadLockUnlock.Join();

                // perform reading the person data from shared memory, creating attached segment
                Assert.ThrowsExactly<AbandonedMutexException>(() =>
                {
                    using Segment segmentAttached = new(mappingName, true);
                    personRead = (PersonData)segmentAttached.GetData();
                });
            }
        }
    }
    #endregion // Tests_write_read_multithread
    #endregion // Tests
}

#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
#pragma warning restore MSTEST0051 // Assert.Throws should contain only a single statement
#pragma warning restore CA1806   // restore "result of '... call is not used"
#pragma warning restore IDE0290 // Use primary constructor
#pragma warning restore IDE0079   // Remove unnecessary suppressions
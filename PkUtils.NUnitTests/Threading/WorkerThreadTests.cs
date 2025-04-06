// Ignore Spelling: Utils
//
using PK.PkUtils.Threading;


namespace PK.PkUtils.NUnitTests.Threading;

/// <summary>  Unit Tests of class <see cref="WorkerThread"/>. </summary>
[TestFixture()]
[CLSCompliant(false)]
public class WorkerThreadTests
{
    [Test, Description("Test of argument-less constructor.")]
    public void WorkerThreadConstructorArgumentLessTest()
    {
        // Arrange + Act
        WorkerThread wt = new();

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(wt.ManagedThread, Is.Not.Null);
            Assert.That(wt.Unstarted, Is.True);
            Assert.That(wt.IsActive, Is.False);
            Assert.That(wt.IsAlive, Is.False);
            Assert.That(wt.IsBackground, Is.False);
            Assert.That(wt.IsStopRequest, Is.False);
            Assert.That(wt.IsDisposed, Is.False);

            Assert.That(wt.Priority, Is.EqualTo(ThreadPriority.Normal));
            Assert.That(wt.EventWaitExit, Is.Null);
            Assert.That(wt.Name, Is.Null);
        });
    }

    [Test, Description("Test of constructor using ThreadPriority.")]
    [TestCase(ThreadPriority.Lowest)]
    [TestCase(ThreadPriority.BelowNormal)]
    [TestCase(ThreadPriority.Normal)]
    [TestCase(ThreadPriority.AboveNormal)]
    [TestCase(ThreadPriority.Highest)]
    public void WorkerThreadConstructorWithThreadPriorityTest(ThreadPriority priority)
    {
        // Arrange + Act
        WorkerThread wt = new(priority);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(wt.ManagedThread, Is.Not.Null);
            Assert.That(wt.Priority, Is.EqualTo(priority));
        });
    }

    [Test, Description("Test of constructor utilizing two bool arguments.")]
    [TestCase(false, false)]
    [TestCase(false, false)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void WorkerThreadConstructorAttachingTest(bool willAttach, bool willCreateWaitExitEvent)
    {
        // Act
        WorkerThread wt = new(attach: willAttach, createWaitExitEvent: willCreateWaitExitEvent);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(wt.ManagedThread, Is.Not.Null);
            Assert.That(wt.Priority, Is.EqualTo(ThreadPriority.Normal));

            Assert.That(!wt.Unstarted, Is.EqualTo(willAttach));
            Assert.That(wt.IsActive, Is.EqualTo(willAttach));
            Assert.That(wt.IsAlive, Is.EqualTo(willAttach));
            Assert.That(wt.IsBackground, Is.False);
            Assert.That(wt.IsStopRequest, Is.False);
            Assert.That(wt.EventWaitExit != null, Is.EqualTo(willCreateWaitExitEvent));

            Assert.That(wt.IsDisposed, Is.False);
        });
    }

    [Test, Description("Test of Start followed by waiting for exit.")]
    public void StartAndJoinTest()
    {
        // Arrange 
        WorkerThread wt = new(attach: false, createWaitExitEvent: true);

        // Act
        wt.Start();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(wt.Unstarted, Is.False);
            Assert.That(wt.IsAlive, Is.True);
            Assert.That(wt.IsDisposed, Is.False);
        });

        // Act
        wt.Join();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(wt.Unstarted, Is.False);
            Assert.That(wt.IsAlive, Is.False);
            Assert.That(wt.IsDisposed, Is.False);
            Assert.That(wt.ManagedThread, Is.Not.Null);
        });
    }

    [Test, Description("Test of Start followed by Abort.")]
    public void StartAndAbortTest()
    {
        // Arrange 
        WorkerThread wt = new(attach: false, createWaitExitEvent: true);

        // Act
        wt.Start();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(wt.Unstarted, Is.False);
            Assert.That(wt.IsAlive, Is.True);
            Assert.That(wt.Name, Is.Null);
            Assert.That(wt.IsDisposed, Is.False);
        });

        // Act
        wt.Abort();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(wt.IsDisposed, Is.True);
            Assert.That(wt.IsAlive, Is.False);
            Assert.That(wt.ManagedThread, Is.Null);
        });
    }

    [Test, Description("Test of Start followed by Dispose.")]
    [TestCase(null!)]
    [TestCase("")]
    [TestCase("E = mc^2")]
    [TestCase("E = mc²")]
    [TestCase("WINDOWS 11 FOR SENIORS: Learn To Use Windows 11 With Ease With Simple, Illustrated Instructions Tailored To The Needs And Comfort Of Seniors And Beginners Paperback – January 15, 2025, $13.99")]
    public void StartAndDisposeTest(string threadName)
    {
        // Arrange
        WorkerThread wt = new(attach: false, createWaitExitEvent: true)
        {
            Name = threadName
        };

        // Act
        wt.Start();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(wt.Unstarted, Is.False);
            Assert.That(wt.IsAlive, Is.True);
            Assert.That(wt.IsDisposed, Is.False);
            Assert.That(wt.Name, Is.EqualTo(threadName));
        });

        // Act
        wt.Dispose();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(wt.IsDisposed, Is.True);
            Assert.That(wt.IsAlive, Is.False);
            Assert.That(wt.ManagedThread, Is.Null);
        });
    }
}
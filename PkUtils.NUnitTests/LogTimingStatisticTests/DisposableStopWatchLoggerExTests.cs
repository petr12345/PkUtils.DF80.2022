// Ignore Spelling: Utils
// 
using PK.PkUtils.Dump;
using PK.PkUtils.Interfaces;
using PK.PkUtils.LogTimingStatistic;

namespace PK.PkUtils.NUnitTests.LogTimingStatisticTests;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable CA1859    // Change type of variable ...

[TestFixture()]
public class DisposableStopWatchLoggerExTests
{
    private const string _actionName = "actionName";
    private const string _actionDetails = "actionDetails";

    [Test()]
    public void DisposableStopWatchLoggerExConstructorTestShouldSucceed()
    {
        // Arrange
        IDumper dumper = new DumperStringWrapper();
        ITimingScope scope = new TimingScope();

        // Act
        var logger = new DisposableStopWatchLoggerEx(dumper, _actionName, _actionDetails, scope);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(logger.IsDisposed, Is.False);
            Assert.That(logger.ActionName, Is.EqualTo(_actionName));
            Assert.That(logger.ActionDetails, Is.EqualTo(_actionDetails));
        }
    }

    [Test()]
    public void DisposableStopWatchLoggerExConstructorTestShouldFail01()
    {
        // Arrange
        IDumper dumper = null!;
        ITimingScope scope = new TimingScope();

        // Act + Assert
        Assert.Throws<ArgumentNullException>(() => new DisposableStopWatchLoggerEx(dumper, _actionName, _actionDetails, scope));
    }

    [Test()]
    public void DisposableStopWatchLoggerExConstructorTestShouldFail02()
    {
        // Arrange
        IDumper dumper = new DumperStringWrapper();
        ITimingScope scope = new TimingScope();

        // Act + Assert
        Assert.Throws<ArgumentNullException>(() => new DisposableStopWatchLoggerEx(dumper, null, _actionDetails, scope));
    }

    [Test()]
    public void DisposableStopWatchLoggerExConstructorTestShouldFail03()
    {
        // Arrange
        IDumper dumper = new DumperStringWrapper();
        ITimingScope scope = new TimingScope();

        // Act + Assert
        Assert.Throws<ArgumentNullException>(() => new DisposableStopWatchLoggerEx(dumper, _actionName, null, scope));
    }

    [Test()]
    public void DisposableStopWatchLoggerExConstructorTestShouldFail04()
    {
        // Arrange
        IDumper dumper = new DumperStringWrapper();

        // Act + Assert
        Assert.Throws<ArgumentNullException>(() => new DisposableStopWatchLoggerEx(dumper, _actionName, _actionDetails, null));
    }

    [Test()]
    public void DisposeTest()
    {
        // Arrange
        IDumper dumper = new DumperStringWrapper();
        ITimingScope scope = new TimingScope();

        // Act
        var logger = new DisposableStopWatchLoggerEx(dumper, _actionName, _actionDetails, scope);
        logger.Dispose();

        // Assert
        string output = dumper.ToString()!;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(logger.IsDisposed, Is.True);
            Assert.That(output, Is.Not.Null);
        }
    }
}

#pragma warning restore CA1859    // Change type of variable ...
#pragma warning restore IDE0079   // Remove unnecessary suppressions
// Ignore Spelling: Utils
//

using PK.PkUtils.Dump;
using PK.PkUtils.Interfaces;
using PK.PkUtils.LogTimingStatistic;


namespace PK.PkUtils.NUnitTests.LogTimingStatisticTests;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable CA1859    // Change type of variable ...

[TestFixture()]
public class DisposableStopWatchLoggerTests
{
    private const string _actionName = "actionName";
    private const string _actionDetails = "actionDetails";

    [Test()]
    public void DisposableStopWatchLoggerConstructorTestShouldSucceed()
    {
        // Arrange
        IDumper dumper = new DumperStringWrapper();

        // Act
        var logger = new DisposableStopWatchLogger(dumper, _actionName, _actionDetails);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(logger.IsDisposed, Is.False);
            Assert.That(logger.ActionName, Is.EqualTo(_actionName));
            Assert.That(logger.ActionDetails, Is.EqualTo(_actionDetails));
        }
    }

    [Test()]
    public void DisposableStopWatchLoggerConstructorTestShouldFail01()
    {
        // Arrange
        IDumper dumper = null!;

        // Act + Assert
        Assert.Throws<ArgumentNullException>(() => new DisposableStopWatchLogger(dumper, _actionName, _actionDetails));
    }

    [Test()]
    public void DisposableStopWatchLoggerConstructorTestShouldFail02()
    {
        // Arrange
        IDumper dumper = new DumperStringWrapper();

        // Act + Assert
        Assert.Throws<ArgumentNullException>(() => new DisposableStopWatchLogger(dumper, null, _actionDetails));
    }

    [Test()]
    public void DisposableStopWatchLoggerConstructorTestShouldFail03()
    {
        // Arrange
        IDumper dumper = new DumperStringWrapper();

        // Act + Assert
        Assert.Throws<ArgumentNullException>(() => new DisposableStopWatchLogger(dumper, _actionName, null));
    }

    [Test()]
    public void DisposeTest()
    {
        // Arrange
        IDumper dumper = new DumperStringWrapper();

        // Act
        var logger = new DisposableStopWatchLogger(dumper, _actionName, _actionDetails);
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
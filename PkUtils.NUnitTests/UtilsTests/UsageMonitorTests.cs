// Ignore Spelling: Utils
//
using PK.PkUtils.Utils;


namespace PK.PkUtils.NUnitTests.UtilsTests;

[TestFixture()]
public class UsageMonitorTests
{
    [Test()]
    public void UsageMonitor_Constructor_Test()
    {
        // Arrange
        UsageCounter counter = new UsageCounter();

        // Act
        UsageMonitor usageWrapper = new UsageMonitor(counter);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(counter.IsUsed, Is.True);
            Assert.That(counter.AddReference(), Is.EqualTo(2));
            Assert.That(usageWrapper.IsDisposed, Is.False);
        });
    }

    [Test()]
    public void UsageMonitor_Dispose_Test()
    {
        // Arrange
        var counter = new UsageCounter();

        // Act
        using (UsageMonitor usageWrapper = new UsageMonitor(counter))
        {
            // Assert
            Assert.That(counter.IsUsed, Is.True);
        }
        Assert.That(counter.IsUsed, Is.False);
    }

}
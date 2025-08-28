// Ignore Spelling: Utils
//
using PK.PkUtils.Utils;

namespace PK.PkUtils.NUnitTests.UtilsTests;

/// <summary>  Unit Test of <see cref="UsageCounter"/>. </summary>
[TestFixture()]
public class UsageMonitorTests
{
    [Test()]
    public void UsageMonitor_Constructor_Test()
    {
        // Arrange
        UsageCounter counter = new();

        // Act
        UsageMonitor usageWrapper = new(counter);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(counter.IsUsed, Is.True);
            Assert.That(counter.AddReference(), Is.EqualTo(2));
            Assert.That(usageWrapper.IsDisposed, Is.False);
        }
    }

    [Test()]
    public void UsageMonitor_Dispose_Test()
    {
        // Arrange
        var counter = new UsageCounter();

        // Act
        using (UsageMonitor usageWrapper = new(counter))
        {
            // Assert
            Assert.That(counter.IsUsed, Is.True);
        }
        Assert.That(counter.IsUsed, Is.False);
    }

}
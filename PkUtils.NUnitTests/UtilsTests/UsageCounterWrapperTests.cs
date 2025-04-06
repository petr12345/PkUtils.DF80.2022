// Ignore Spelling: Utils
//
using PK.PkUtils.Utils;


namespace PK.PkUtils.NUnitTests.UtilsTests;

[TestFixture()]
public class UsageCounterWrapperTests
{
    [Test()]
    public void UsageCounterWrapper_Constructor_Test()
    {
        // Arrange
        UsageCounter counter = new UsageCounter();

        // Act
        UsageCounterWrapper usageWrapper = new UsageCounterWrapper(counter);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(counter.IsUsed, Is.True);
            Assert.That(counter.AddReference(), Is.EqualTo(2));
            Assert.That(usageWrapper.IsDisposed, Is.False);
        });
    }

    [Test()]
    public void UsageCounterWrapper_Dispose_Test()
    {
        // Arrange
        var counter = new UsageCounter();

        // Act
        using (UsageCounterWrapper usageWrapper = new UsageCounterWrapper(counter))
        {
            // Assert
            Assert.That(counter.IsUsed, Is.True);
        }
        Assert.That(counter.IsUsed, Is.False);
    }

}
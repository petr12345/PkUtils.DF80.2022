// Ignore Spelling: Utils
//

using PK.PkUtils.Utils;

namespace PK.PkUtils.NUnitTests.UtilsTests;

[TestFixture()]
public class UsageCounterTests
{
    [Test()]
    public void UsageCounterConstructorTest()
    {
        // ACT
        UsageCounter counter = new UsageCounter();

        // ASSERT
        Assert.That(counter.IsUsed, Is.False);
        Assert.DoesNotThrow(() => counter.AssertValid());
    }

    [Test()]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(22)]
    public void AddReferenceTest(int references)
    {
        // ARRANGE
        UsageCounter counter = new UsageCounter();

        // ACT
        Parallel.For(0, references, n => counter.AddReference());

        // ASSERT
        Assert.That(counter.AddReference(), Is.EqualTo(references + 1));
    }

    [Test()]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(22)]
    [TestCase(256)]
    public void ReleaseTest(int references)
    {
        // ARRANGE
        UsageCounter counter = new UsageCounter();

        // ACT
        Parallel.For(0, references, n => counter.AddReference());
        Parallel.For(0, references - 1, n => counter.Release());

        // ASSERT
        // Assert.AreEqual(0, counter.Release());
        Assert.That(counter.Release(), Is.EqualTo(0));
    }
}
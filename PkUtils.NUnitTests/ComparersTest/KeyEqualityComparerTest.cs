// Ignore Spelling: Utils, Comparers
// 
using PK.PkUtils.Comparers;

#pragma warning disable NUnit2045 // Use Assert.Multiple

namespace PK.PkUtils.NUnitTests.ComparersTest;

public class KeyEqualityComparerTest
{
    #region Tests

    /// <summary>
    /// A test for KeyEqualityComparer constructor, which should succeed
    /// </summary>
    [Test()]
    public void KeyEqualityComparer_Constructor_01()
    {
        Assert.Throws<ArgumentNullException>(() => new KeyEqualityComparer<int, int>(null));
    }

    /// <summary>
    /// A test for KeyEqualityComparer constructor, which should throw ArgumentNullException
    /// </summary>
    [Test()]
    public void KeyEqualityComparer_Constructor_02()
    {
        Assert.Throws<ArgumentNullException>(() => new KeyEqualityComparer<string, int>(null, null));
    }

    /// <summary>
    /// A test for KeyEqualityComparer constructor, which should throw ArgumentNullException
    /// </summary>
    [Test()]
    public void KeyEqualityComparer_Constructor_03()
    {
        var comparer = new KeyEqualityComparer<string, int>(s => s.Length);
    }

    /// <summary>
    /// A test for KeyEqualityComparer.Equals, which should succeed
    /// </summary>
    [Test()]
    public void KeyEqualityComparer_EqualsTest_01()
    {
        var comparer = new KeyEqualityComparer<int, int>(x => Math.Abs(x));

        foreach (var x in Enumerable.Range(-5, 11).ToList())
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(comparer.Equals(x, x), Is.True);
                Assert.That(comparer.Equals(x, -x), Is.True);
            }
        }
    }

    /// <summary>
    /// A test for KeyEqualityComparer.Equals, which should succeed
    /// </summary>
    [Test()]
    public void KeyEqualityComparer_EqualsTest_02()
    {
        var comparer = new KeyEqualityComparer<string, int>(s => s.Length);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(comparer.Equals("hiker", "HIKER"), Is.True);
            Assert.That(comparer.Equals("trace", "tRaCe"), Is.True);
            Assert.That(comparer.Equals("lemons", "lemon"), Is.False);
        }
    }
    #endregion // Tests
}

// Ignore Spelling: Utils, Comparers
// 
using PK.PkUtils.Comparers;


namespace PK.PkUtils.NUnitTests.ComparersTest;

/// <summary>
/// This is a test class for KeyComparer generic
///</summary>
[TestFixture()]
public class KeyComparerTest
{
    #region Tests

    [Test()]
    [Description("A test for KeyComparer constructor, which should throw ArgumentNullException")]
    public void KeyComparer_Constructor_01()
    {
        Assert.Throws<ArgumentNullException>(() => new KeyComparer<int, int>(null));
    }

    [Test()]
    [Description("A test for KeyComparer constructor, which should throw ArgumentNullException")]
    public void KeyComparer_Constructor_02()
    {
        Assert.Throws<ArgumentNullException>(() => new KeyComparer<string, int>(null, null));
    }

    [Test()]
    [Description("A test for KeyComparer constructor, which should succeed")]
    public void KeyComparer_Constructor_03()
    {
        new KeyComparer<string, int>(s => s.Length);
    }

    [Test()]
    [Description("A test for KeyComparer.Compare, which should succeed")]
    public void KeyComparer_EqualsTest_01()
    {
        var comparer = new KeyComparer<int, int>(x => Math.Abs(x));
        var listInts = Enumerable.Range(0, 11).ToList();

        foreach (var x in listInts)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(comparer.Compare(x, x), Is.Zero);
                Assert.That(comparer.Compare(x, x + 1), Is.LessThan(0));
            }
        }
    }

    [Test()]
    [Description("A test for KeyComparer.Compare, which should succeed")]
    public void KeyComparer_EqualsTest_02()
    {
        var comparer = new KeyComparer<string, int>(s => s.Length);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(comparer.Compare("aaa", "AAA"), Is.Zero);
            Assert.That(comparer.Compare("aaaa", "wXyZ"), Is.Zero);
            Assert.That(comparer.Compare("aaa", "w"), Is.GreaterThan(0));
            Assert.That(comparer.Compare("a", "www"), Is.LessThan(0));
        }
    }

    [Test()]
    [Description("A test for KeyComparer.Create, which should succeed")]
    public void KeyComparer_CreateTest()
    {
        KeyComparer.Create<string, int>(s => s.Length);
    }
    #endregion // Tests
}

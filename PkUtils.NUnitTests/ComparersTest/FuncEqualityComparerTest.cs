using PK.PkUtils.Comparers;

namespace PK.PkUtils.NUnitTests.ComparersTest;



/// <summary>
/// This is a test class for FunctionalEqualityComparer generic
///</summary>
[TestFixture()]
public class FuncEqualityComparerTest
{
    #region Tests

    /// <summary>   A test for FunctionalEqualityComparer constructor. </summary>
    [Test()]
    public void FuncEqualityComparer_Constructor_01()
    {
        Assert.Throws<ArgumentNullException>(() => new FunctionalEqualityComparer<int>(null));
    }

    /// <summary>   A test for FunctionalEqualityComparer constructor. </summary>
    [Test()]
    public void FuncEqualityComparer_Constructor_02()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new FunctionalEqualityComparer<int>((x, y) => x == y, null));
    }

    /// <summary>   A test for FunctionalEqualityComparer constructor. </summary>
    [Test()]
    public void FuncEqualityComparer_Constructor_03()
    {
        var comparer = new FunctionalEqualityComparer<int>((x, y) => x == y, x => x);
    }

    /// <summary>
    /// A test for FunctionalEqualityComparer.Equals
    /// </summary>
    [Test()]
    public void FuncEqualityComparer_CompareTest_01()
    {
        var comparer = new FunctionalEqualityComparer<int>((x, y) => Math.Abs(x) == Math.Abs(y), x => Math.Abs(x));

        using (Assert.EnterMultipleScope())
        {
            foreach (var x in Enumerable.Range(2, 7).ToList())
            {
                Assert.That(comparer.Equals(x, x), Is.True);
                Assert.That(comparer.Equals(x, -x), Is.True);
            }
        }
    }

    /// <summary>
    /// A test for FunctionalEqualityComparer.Equals
    /// </summary>
    [Test()]
    public void FuncEqualityComparer_CompareTest_02()
    {
        bool F(string x, string y) => (x.Length == y.Length);
        int H(string s) => s.Length;
        var comparer = new FunctionalEqualityComparer<string>(F, H);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(comparer.Equals("aaa", "AAA"), Is.True);
            Assert.That(comparer.Equals("aaaa", "wXyZ"), Is.True);
            Assert.That(comparer.Equals("aaa", "w"), Is.False);
        }
    }

    /// <summary>
    /// A test for FunctionalEqualityComparer.CreateNullSafeComparer
    /// </summary>
    [Test()]
    public void FuncEqualityComparer_CreateNullSafeComparerTest_01()
    {
        Func<string, string, bool> f = null!;
        Func<string, int> h = null!;
        Assert.Throws<ArgumentNullException>(
            () => FunctionalEqualityComparer.CreateNullSafeComparer<string>(f, h));
    }

    /// <summary>
    /// A test for FunctionalEqualityComparer.CreateNullSafeComparer
    /// </summary>
    [Test()]
    public void FuncEqualityComparer_CreateNullSafeComparerTest_02()
    {
        bool F(string x, string y) => (x.Length == y.Length);
        int H(string s) => s.Length;
        FunctionalEqualityComparer<string> comparer = FunctionalEqualityComparer.CreateNullSafeComparer<string>(F, H);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(comparer.Equals("aaa", "AAA"), Is.True);
            Assert.That(comparer.Equals("aaa", "XYZ"), Is.True);

            Assert.That(comparer.Equals(null!, "pqr"), Is.False);
            Assert.That(comparer.Equals("pqr", null!), Is.False);

            Assert.That(comparer.Equals(null!, null!), Is.True);
            Assert.That(comparer.Equals(null!, string.Empty), Is.False);
            Assert.That(comparer.Equals(string.Empty!, null!), Is.False);
        }
    }
    #endregion // Tests
}

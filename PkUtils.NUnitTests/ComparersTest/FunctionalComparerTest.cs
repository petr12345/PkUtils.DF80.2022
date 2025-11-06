using PK.PkUtils.Comparers;

namespace PK.PkUtils.NUnitTests.ComparersTest;

/// <summary>
/// This is a test class for FunctionalComparer generic
///</summary>
[TestFixture()]
public class FunctionalComparerTest
{
    #region Tests

    /// <summary>
    /// A test for FunctionalComparer constructor
    /// </summary>
    [Test()]
    public void FunctionalComparer_Constructor_01()
    {
        Assert.Throws<ArgumentNullException>(() => new FunctionalComparer<int>(null));
    }

    /// <summary>
    /// A test for FunctionalComparer.Compare
    /// </summary>
    [Test()]
    public void FunctionalComparer_CompareTest_01()
    {
        static int f(string x, string y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        FunctionalComparer<string> comparer = new(f);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(comparer.Compare("aaa", "AAA"), Is.Zero);
            Assert.That(comparer.Compare("aaa", "bbb"), Is.Not.Zero);
        }
    }

    /// <summary>
    /// A test for FunctionalComparer.CreateNullSafeComparer
    /// </summary>
    [Test()]
    public void FunctionalComparer_CreateNullSafeComparerTest_01()
    {
        Comparison<string> f = null!;
        Assert.Throws<ArgumentNullException>(() => FunctionalComparer.CreateNullSafeComparer(f));
    }

    /// <summary>
    /// A test for FunctionalComparer.CreateNullSafeComparer
    /// </summary>
    [Test()]
    public void FunctionalComparer_CreateNullSafeComparerTest_02()
    {
        static int f(string x, string y) => (x.Length - y.Length);
        FunctionalComparer<string> comparer = FunctionalComparer.CreateNullSafeComparer((Comparison<string>)f);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(comparer.Compare("aaa", "AAA"), Is.Zero);
            Assert.That(comparer.Compare("aaa", "XYZ"), Is.Zero);

            Assert.That(comparer.Compare(null!, "pqr"), Is.LessThan(0));
            Assert.That(comparer.Compare("pqr", null!), Is.GreaterThan(0));

            Assert.That(comparer.Compare(null!, null!), Is.Zero);
            Assert.That(comparer.Compare(null!, string.Empty), Is.LessThan(0));
            Assert.That(comparer.Compare(string.Empty, null!), Is.GreaterThan(0));
        }
    }
    #endregion // Tests
}


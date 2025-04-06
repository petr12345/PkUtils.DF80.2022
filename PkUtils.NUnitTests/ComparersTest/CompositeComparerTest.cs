// Ignore Spelling: Utils, Comparers
//
using PK.PkUtils.Comparers;

namespace PK.PkUtils.NUnitTests.ComparersTest;

/// <summary>
/// Unit tests for <see cref="CompositeComparer{T}"/>.
/// </summary>
[TestFixture, CLSCompliant(false)]
public class CompositeComparerTest
{
    private IComparer<string> _firstComparer;
    private IComparer<string> _secondComparer;

    [SetUp]
    public void SetUp()
    {
        _firstComparer = StringComparer.OrdinalIgnoreCase;
        _secondComparer = Comparer<string>.Create((x, y) => (x?.Length ?? 0).CompareTo(y?.Length ?? 0));
    }

    #region Tests
    #region Constructor Tests

    [Test, Description("Throws ArgumentNullException if first comparer is null.")]
    public void Constructor_FirstComparerNull_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new CompositeComparer<string>(null, _secondComparer));
        Assert.That(ex.ParamName, Is.EqualTo("firstComparer"));
    }

    [Test, Description("Throws ArgumentNullException if second comparer is null.")]
    public void Constructor_SecondComparerNull_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new CompositeComparer<string>(_firstComparer, null));
        Assert.That(ex.ParamName, Is.EqualTo("secondComparer"));
    }

    [Test, Description("Successfully creates instance when both comparers are provided.")]
    public void Constructor_ValidComparers_CreatesInstance()
    {
        var comparer = new CompositeComparer<string>(_firstComparer, _secondComparer);
        Assert.That(comparer, Is.Not.Null);
    }

    #endregion // Constructor Tests

    #region Compare Method Tests

    [TestCase("apple", "Banana", ExpectedResult = -1, Description = "First comparer determines order: 'apple' < 'Banana' (case-insensitive).")]
    [TestCase("Banana", "apple", ExpectedResult = 1, Description = "First comparer determines order: 'Banana' > 'apple' (case-insensitive).")]
    [TestCase("apple", "APPLE", ExpectedResult = 0, Description = "First comparer considers values equal (case-insensitive).")]
    public int Compare_FirstComparerDeterminesOrder_ReturnsCorrectResult(string x, string y)
    {
        var comparer = new CompositeComparer<string>(_firstComparer, _secondComparer);
        return Math.Sign(comparer.Compare(x, y)); // Normalize to -1, 0, 1
    }

    [TestCase("apple", "apple", ExpectedResult = 0, Description = "Both strings are identical.")]
    [TestCase("apple", "APPLE", ExpectedResult = 0, Description = "Case-insensitive match with same length.")]
    [TestCase("apple", "apples", ExpectedResult = -1, Description = "Second comparer used: 'apple'.Length < 'apples'.Length.")]
    [TestCase("apples", "apple", ExpectedResult = 1, Description = "Second comparer used: 'apples'.Length > 'apple'.Length.")]
    public int Compare_UsesSecondComparerWhenFirstReturnsZero(string x, string y)
    {
        var comparer = new CompositeComparer<string>(_firstComparer, _secondComparer);
        return Math.Sign(comparer.Compare(x, y));
    }

    [TestCase(null!, "apple", ExpectedResult = -1, Description = "Null is less than non-null.")]
    [TestCase("apple", null!, ExpectedResult = 1, Description = "Non-null is greater than null.")]
    [TestCase(null!, null!, ExpectedResult = 0, Description = "Both values are null.")]
    public int Compare_NullValues_HandlesCorrectly(string x, string y)
    {
        var comparer = new CompositeComparer<string>(_firstComparer, _secondComparer);
        return Math.Sign(comparer.Compare(x, y));
    }

    [TestCase("cat", "dog", ExpectedResult = 0, Description = "Both strings have the same length.")]
    [TestCase("cat", "bird", ExpectedResult = -1, Description = "'cat'.Length < 'bird'.Length.")]
    [TestCase("elephant", "dog", ExpectedResult = 1, Description = "'elephant'.Length > 'dog'.Length.")]
    public int Compare_UsesSecondComparerWhenFirstComparerAlwaysReturnsZero(string x, string y)
    {
        var firstComparer = Comparer<string>.Create((a, b) => 0); // Always returns 0
        var comparer = new CompositeComparer<string>(firstComparer, _secondComparer);

        return Math.Sign(comparer.Compare(x, y));
    }

    #endregion // Compare Method Tests
    #endregion // Tests
}

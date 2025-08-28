// Ignore Spelling: CCA, Utils
// 
using PK.PkUtils.Extensions;

namespace PK.PkUtils.NUnitTests.ExtensionsTests;

#pragma warning disable CA1859    // Change type of variable ...

/// <summary>   (Unit Test Fixture) of a class <see cref="DictionaryExtensions"/>. </summary>
[TestFixture()]
public class DictionaryExtensionsTests
{
    #region Auxiliary_methods

    /// <summary> A helper method for test of GetValueOrDefault </summary>
    private static void ValueOrDefaultTestHelper<TKey, TValue>(
        IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue expected)
    {
        // Arrange
        bool before = dictionary.ContainsKey(key);
        // Act
        TValue actual = dictionary.ValueOrDefault<TKey, TValue>(key);
        bool after = dictionary.ContainsKey(key);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(after, Is.EqualTo(before));
        }
    }

    /// <summary> A helper method for test of GetValueOrNew </summary>
    private static void GetValueOrNewTestHelper<TKey, TValue>(
        IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue expected) where TValue : new()
    {
        // Act
        TValue actual = dictionary.GetValueOrNew<TKey, TValue>(key);
        bool after = dictionary.ContainsKey(key);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(after, Is.True);
        }
    }


    /// <summary> A helper method for test of AddNew.</summary>
    private static void AddNewTestHelper<TKey, TValue>(
        IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        // Act
        dictionary.AddNew<TKey, TValue>(key, value);

        // Assert
        Assert.That(dictionary.ContainsKey(key), Is.True);
    }

    /// <summary>
    /// A helper method for test of RemoveExisting
    /// </summary>
    private static void RemoveExistingTestHelper<TKey, TValue>(
        IDictionary<TKey, TValue> dictionary, TKey key)
    {
        // Act
        DictionaryExtensions.RemoveExisting<TKey, TValue>(dictionary, key);

        // Assert
        Assert.That(dictionary.ContainsKey(key), Is.False);
    }

    /// <summary>
    /// A helper for test for TryRemove
    /// </summary>
    private static void TryRemoveTestHelper<TKey, TValue>(
        IDictionary<TKey, TValue> dictionary, TKey key)
    {
        // Act
        bool expected = dictionary.ContainsKey(key);
        bool actual = DictionaryExtensions.TryRemove<TKey, TValue>(dictionary, key, out _);

        // Assert

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(dictionary.ContainsKey(key), Is.False);
        }
    }
    #endregion // Auxiliary_methods

    #region Tests_IDictionary_extensions

    [Test]
    public void DictionaryExtension_GetValueOrDefaultTest_01()
    {
        // Arrange
        IDictionary<int, int> dict = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => dict.ValueOrDefault(5));
    }

    [Test]
    public void DictionaryExtension_GetValueOrDefault_Test_02()
    {
        // Arrange
        const int count = 6;
        IDictionary<int, int> dict = Enumerable.Repeat(0, count).Select((n, i) => i).ToDictionary(k => k);

        // Act & Assert
        ValueOrDefaultTestHelper<int, int>(dict, 1, 1);
        ValueOrDefaultTestHelper<int, int>(dict, 3, 3);
        ValueOrDefaultTestHelper<int, int>(dict, 33, 0);
    }

    [Test]
    public void DictionaryExtension_GetValueOrNew_Test_01()
    {
        // Arrange
        IDictionary<int, int> dict = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => dict.GetValueOrNew(5));
    }

    [Test]
    public void DictionaryExtension_GetValueOrNew_Test_02()
    {
        // Arrange
        const int count = 6;
        IDictionary<int, int> dict = Enumerable.Repeat(0, count).Select((n, i) => i).ToDictionary(k => k);

        // Act & Assert
        GetValueOrNewTestHelper<int, int>(dict, 1, 1);
        GetValueOrNewTestHelper<int, int>(dict, 3, 3);
        GetValueOrNewTestHelper<int, int>(dict, 44, 0);
    }

    [Test]
    public void DictionaryExtension_AddNewTest_01()
    {
        // Arrange
        IDictionary<int, int> dict = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => dict.AddNew(1, 1));
    }

    [Test]
    public void DictionaryExtension_AddNewTest_02()
    {
        // Arrange
        var couples = Enumerable.Repeat(0, 5).Select((n, index) => new { index, n }).ToList();
        IDictionary<int, int> dict = new Dictionary<int, int>();

        // Act & Assert
        couples.SafeForEach(couple => AddNewTestHelper(dict, couple.index, couple.n));
    }

    [Test]
    [TestCase(0)]
    [TestCase(7)]
    public void DictionaryExtension_AddNewTest_03(int duplicatedKey)
    {
        // Arrange
        var couples = Enumerable.Repeat(duplicatedKey, 6).Select((n, index) => new { index, n }).ToList();
        IDictionary<int, int> dict = new Dictionary<int, int>();

        // Act & Assert
        // throws because the same key is repeated several times
        Assert.Throws<ArgumentException>(() =>
            couples.SafeForEach(couple => AddNewTestHelper(dict, key: couple.n, value: couple.index)));
    }

    [Test]
    public void DictionaryExtension_AddRange_Test_01()
    {
        // Arrange
        IDictionary<int, string> dict_a = new Dictionary<int, string>();
        IDictionary<int, string> dict_b = null!;

        Assert.Throws<ArgumentNullException>(() => dict_a.AddRange(dict_b));
    }

    [Test]
    public void DictionaryExtension_AddRange_Test_02()
    {
        // Arrange
        IDictionary<int, string> dict_a = new Dictionary<int, string>();
        IDictionary<int, string> dict_b = new Dictionary<int, string> { { 4, "a" }, { 5, "b" }, { 6, "c" } };

        // Act
        dict_a.AddRange(dict_b);

        // Assert
        Assert.That(dict_a.MemberwiseEqual(dict_b), Is.True);
    }

    /// <summary>
    /// A test for RemoveExisting
    /// </summary>
    [Test()]
    public void DictionaryExtension_RemoveExistingTest_01()
    {
        // Arrange
        const int count = 6;
        IDictionary<int, int> dict = new Dictionary<int, int>();
        var listInts = Enumerable.Repeat(0, count).Select((n, i) => i).ToList();
        listInts.ForEach(n => dict.Add(n, n));

        // Act & Assert
        // test removing
        listInts.ForEach(n => RemoveExistingTestHelper<int, int>(dict, n));
    }

    /// <summary> A test for RemoveExisting. </summary>
    [Test()]
    public void DictionaryExtension_RemoveExistingTest_02()
    {
        // Arrange
        const int count = 6;
        IDictionary<int, int> dict = new Dictionary<int, int>();

        // fill dictionary
        Enumerable.Repeat(0, count).Select((n, i) => i).ToList().ForEach(k => dict.Add(k, k));

        // Assert
        // test removing
        Assert.Throws<ArgumentException>(() =>
            RemoveExistingTestHelper<int, int>(dict, 100));
    }

    /// <summary>
    /// A test for TryRemove
    /// </summary>
    [Test()]
    public void DictionaryExtension_TryRemoveTest_01()
    {
        // Arrange
        // fill dictionary
        const int count = 6;
        IDictionary<int, int> dict = new Dictionary<int, int>();
        var listInts = Enumerable.Repeat(0, count).Select((n, i) => i).ToList();
        listInts.ForEach(n => dict.Add(n, n));

        // Assert
        // test TryRemove
        listInts.ForEach(n => TryRemoveTestHelper<int, int>(dict, n));
    }

    /// <summary>
    /// A test for TryRemove
    /// </summary>
    [Test()]
    public void DictionaryExtension_TryRemoveTest_02()
    {
        // Arrange
        // fill dictionary
        const int count = 6;
        IDictionary<int, int> dict = new Dictionary<int, int>();
        var listIntsA = Enumerable.Repeat(0, count).Select((n, i) => i).ToList();
        var listIntsB = Enumerable.Repeat(0, count).Select((n, i) => count + i).ToList();

        listIntsA.ForEach(n => dict.Add(n, n));

        // Assert
        // test TryRemove
        listIntsB.ForEach(n => TryRemoveTestHelper<int, int>(dict, n));
        listIntsA.ForEach(n => TryRemoveTestHelper<int, int>(dict, n));
    }

    [Test]
    public void DictionaryExtension_ToStringEx_Test_01()
    {
        // Arrange
        IDictionary<int, int> dict = null!;

        // Assert
        Assert.Throws<ArgumentNullException>(() => dict.ToStringEx());
    }

    [Test]
    public void DictionaryExtension_ToStringEx_Test_02()
    {
        // Arrange
        IDictionary<int, string> dict = new Dictionary<int, string> { { 4, "a" }, { 5, "b" } };
        string expected = "{4=a,5=b}";
        string actual = dict.ToStringEx();

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void DictionaryExtension_CompareDictionary_Test_01()
    {
        // Arrange
        IDictionary<int, int> dict = null!;

        // Assert
        Assert.Throws<ArgumentNullException>(() =>
            dict.MemberwiseEqual(new Dictionary<int, int>()));
    }

    [Test]
    public void DictionaryExtension_CompareDictionary_Test_02()
    {
        // Arrange
        IDictionary<int, string> dict_0th = null!;
        IDictionary<int, string> dict_1st = new Dictionary<int, string> { { 4, "a" }, { 5, "b" } };
        IDictionary<int, string> dict_2nd = new Dictionary<int, string> { { 4, "A" }, { 5, "B" } };
        IDictionary<int, string> dict_3rd = new Dictionary<int, string> { { 4, "a" }, { 5, "b" }, { 6, "c" } };

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dict_1st.MemberwiseEqual(dict_0th), Is.False);
            Assert.That(dict_1st.MemberwiseEqual(dict_2nd), Is.False);
            Assert.That(dict_1st.MemberwiseEqual(dict_2nd, StringComparer.InvariantCultureIgnoreCase), Is.True);
            Assert.That(dict_1st.MemberwiseEqual(dict_3rd, StringComparer.InvariantCultureIgnoreCase), Is.False);
        }
    }

    [Test]
    public void DictionaryExtension_DictionaryHashCode_Test_01()
    {
        // Arrange
        IDictionary<int, int> dict = null!;

        // Assert
        Assert.Throws<ArgumentNullException>(() => dict.DictionaryHashCode());
    }

    [Test]
    public void DictionaryExtension_DictionaryHashCode_Test_02()
    {
        // Arrange
        IDictionary<int, string> dict_1st = new Dictionary<int, string> { { 4, "a" }, { 5, "b" } };
        IDictionary<int, string> dict_2nd = new Dictionary<int, string> { { 5, "b" }, { 4, "a" } };
        IDictionary<int, string> dict_3rd = new Dictionary<int, string> { { 4, null! }, { 5, null! }, { 6, null! } };
        IDictionary<int, string> dict_4th = new Dictionary<int, string> { { 6, null! }, { 5, null! }, { 4, null! } };

        int hash1st = dict_1st.DictionaryHashCode();
        int hash2nd = dict_2nd.DictionaryHashCode();
        int hash3rd = dict_3rd.DictionaryHashCode();
        int hash4th = dict_4th.DictionaryHashCode();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(hash1st, Is.EqualTo(hash2nd));
            Assert.That(hash3rd, Is.EqualTo(hash4th));
        }
    }
    #endregion // Tests_IDictionary_extensions

    #region Tests_IReadOnlyDictionary_extensions

    // #FIX# - To Do

    #endregion // Tests_IReadOnlyDictionary_extensions
}
#pragma warning restore CA1859    // Change type of variable ...
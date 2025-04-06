// Ignore Spelling: PkUtils, Utils, Concat
// 
using PK.PkUtils.Extensions;

#pragma warning disable NUnit2045 // Use Assert.Multiple
#pragma warning disable IDE0039	// Use local function


namespace PK.PkUtils.NUnitTests.ExtensionsTests;

/// <summary>
/// This is a test class for static class EnumerableExtensions.
/// </summary>
[TestFixture()]
public class EnumerableExtensionTest
{
    #region Auxiliary_methods

    /// <summary>
    /// A helper for test of IsNullorEmpty
    /// </summary>
    private static void IsNullOrEmptyTestHelper<T>(IEnumerable<T> source, bool expected)
    {
        bool actual = EnumerableExtensions.IsNullOrEmpty<T>(source);
        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A helper for test of IsEmpty
    /// </summary>
    private static void IsEmptyTestHelper<T>(IEnumerable<T> source, bool expected)
    {
        bool actual = EnumerableExtensions.IsEmpty<T>(source);
        Assert.That(actual, Is.EqualTo(expected));
    }
    #endregion // Auxiliary_methods

    #region Tests

    #region Tests_IsEmpty

    /// <summary>   A test for EnumerableExtensions.IsEmpty&lt;T&gt; </summary>
    [Test()]
    public void EnumerableExtension_IsEmpty_Test_01()
    {
        int[] arrInt1 = [1, 2, 3];
        IsEmptyTestHelper<int>(arrInt1, false);
    }

    /// <summary>   A test for EnumerableExtensions.IsEmpty&lt;T&gt; </summary>
    [Test()]
    public void EnumerableExtension_IsEmpty_Test_02()
    {
        int[] arrInt2 = [];
        IsEmptyTestHelper<int>(arrInt2, true);
    }

    /// <summary>   A test for EnumerableExtensions.IsEmpty&lt;T&gt; </summary>
    [Test()]
    public void EnumerableExtension_IsEmpty_Test_03()
    {
        Assert.Throws<ArgumentNullException>(() => IsEmptyTestHelper<int>(null!, true));
    }
    #endregion // Tests_IsEmpty

    #region Tests_IsNullOrEmpty

    [Test()]
    public void EnumerableExtension_IsNullOrEmpty_Test_01()
    {
        int[] arrInt = [11, 12];
        IsEmptyTestHelper<int>(arrInt, false);
    }

    [Test()]
    public void EnumerableExtension_IsNullOrEmpty_Test_02()
    {
        IsNullOrEmptyTestHelper<int>(null!, true);
    }
    #endregion // Tests_IsNullOrEmpty

    #region Tests_ContainsNull
    /// <summary>
    /// Tests that the method returns true when the collection contains at least one null element.
    /// </summary>
    [Test]
    public void ContainsNull_WithNullElements_ReturnsTrue()
    {
        // Arrange
        List<string> listWithNull = ["Hello", null!, "World"];

        // Act
        bool result = listWithNull.ContainsNull();

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that the method returns false when the collection does not contain any null elements.
    /// </summary>
    [Test]
    public void ContainsNull_WithoutNullElements_ReturnsFalse()
    {
        // Arrange
        List<string> listWithoutNull = ["Hello", "World"];

        // Act
        bool result = listWithoutNull.ContainsNull();

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that the method returns false when the collection itself is null.
    /// </summary>
    [Test]
    public void ContainsNull_WithNullSource_ReturnsFalse()
    {
        // Arrange
        List<string> nullList = null!;

        // Act
        bool result = nullList.ContainsNull();

        // Assert
        Assert.That(result, Is.False);
    }
    #endregion // Tests_ContainsNull

    #region Tests_IsSorted

    /// <summary>
    /// Tests that the method returns true for a sorted list of integers.
    /// </summary>
    [Test]
    public void EnumerableExtension_IsSorted_TestWithSortedIntegers_ReturnsTrue()
    {
        // Arrange
        List<int> sortedList = [1, 2, 3, 4, 5];

        // Act
        bool result = sortedList.IsSorted();

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that the method returns false for an unsorted list of integers.
    /// </summary>
    [Test]
    public void EnumerableExtension_IsSorted_TestWithUnsortedIntegers_ReturnsFalse()
    {
        // Arrange
        List<int> unsortedList = [5, 3, 1, 4, 2];

        // Act
        bool result = unsortedList.IsSorted();

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that the method returns true for a sorted list of strings.
    /// </summary>
    [Test]
    public void EnumerableExtension_IsSorted_TestWithSortedStrings_ReturnsTrue()
    {
        // Arrange
        List<string> sortedList = ["Apple", "Banana", "Cherry"];

        // Act
        bool result = sortedList.IsSorted();

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that the method throws an ArgumentNullException when the input is null.
    /// </summary>
    [Test]
    public void EnumerableExtension_IsSorted_TestWithNullInput_ThrowsArgumentNullException()
    {
        // Arrange
        List<int> nullList = null!;

        // Act & Assert
        Assert.That(() => nullList.IsSorted(), Throws.TypeOf<ArgumentNullException>());
    }

    /// <summary>
    /// Tests that the method returns true for an empty list.
    /// </summary>
    [Test]
    public void EnumerableExtension_IsSorted_TestWithEmptyList_ReturnsTrue()
    {
        // Arrange
        List<int> emptyList = [];

        // Act
        bool result = emptyList.IsSorted();

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that the method returns true for a single-element list.
    /// </summary>
    [Test]
    public void EnumerableExtension_IsSorted_TestWithSingleElementList_ReturnsTrue()
    {
        // Arrange
        List<int> singleElementList = [42];

        // Act
        bool result = singleElementList.IsSorted();

        // Assert
        Assert.That(result, Is.True);
    }


    #endregion // Tests_IsSorted

    #region Tests_StartsWith

    /// <summary>
    /// Tests that the method returns true when the list starts with the specified integer.
    /// </summary>
    [Test]
    public void EnumerableExtension_StartsWith_TestWithStartingInteger_ReturnsTrue()
    {
        // Arrange
        List<int> list = [1, 2, 3];

        // Act
        bool result = list.StartsWith(1);

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that the method returns false when the list does not start with the specified integer.
    /// </summary>
    [Test]
    public void EnumerableExtension_StartsWith_TestWithNonStartingInteger_ReturnsFalse()
    {
        // Arrange
        List<int> list = [1, 2, 3];

        // Act
        bool result = list.StartsWith(2);

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that the method returns false when the list is empty.
    /// </summary>
    [Test]
    public void EnumerableExtension_StartsWith_TestWithEmptyList_ReturnsFalse()
    {
        // Arrange
        List<int> emptyList = [];

        // Act
        bool result = emptyList.StartsWith(1);

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that the method returns false when the list is null.
    /// </summary>
    [Test]
    public void EnumerableExtension_StartsWith_TestWithNullList_ReturnsFalse()
    {
        // Arrange
        List<int> nullList = null!;

        // Act
        bool result = nullList.StartsWith(1);

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that the method returns true when the list starts with the specified string.
    /// </summary>
    [Test]
    public void EnumerableExtension_StartsWith_TestWithStartingString_ReturnsTrue()
    {
        // Arrange
        List<string> list = ["Hello", "World"];

        // Act
        bool result = list.StartsWith("Hello");

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that the method returns false when the list does not start with the specified string.
    /// </summary>
    [Test]
    public void EnumerableExtension_StartsWith_TestWithNonStartingString_ReturnsFalse()
    {
        // Arrange
        List<string> list = ["Hello", "World"];

        // Act
        bool result = list.StartsWith("World");

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that the method returns true when the list starts with the specified complex object.
    /// </summary>
    [Test]
    public void EnumerableExtension_StartsWith_TestWithStartingComplexObject_ReturnsTrue()
    {
        // Arrange
        var complexObject = new { Id = 1, Name = "Test" };
        List<object> list = [complexObject, new { Id = 2, Name = "Test2" }];

        // Act
        bool result = list.StartsWith(complexObject);

        // Assert
        Assert.That(result, Is.True);
    }
    #endregion // Tests_StartsWith

    #region Tests_EndsWith
    /// <summary>
    /// Tests that the method returns true when the list ends with the specified integer.
    /// </summary>
    [Test]
    public void EnumerableExtension_EndsWith_TestWithEndingInteger_ReturnsTrue()
    {
        // Arrange
        List<int> list = [1, 2, 3];

        // Act
        bool result = list.EndsWith(3);

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that the method returns false when the list does not end with the specified integer.
    /// </summary>
    [Test]
    public void EnumerableExtension_EndsWith_TestWithNonEndingInteger_ReturnsFalse()
    {
        // Arrange
        List<int> list = [1, 2, 3];

        // Act
        bool result = list.EndsWith(2);

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that the method returns false when the list is empty.
    /// </summary>
    [Test]
    public void EnumerableExtension_EndsWith_TestWithEmptyList_ReturnsFalse()
    {
        // Arrange
        List<int> emptyList = [];

        // Act
        bool result = emptyList.EndsWith(1);

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that the method returns false when the list is null.
    /// </summary>
    [Test]
    public void EnumerableExtension_EndsWith_TestWithNullList_ReturnsFalse()
    {
        // Arrange
        List<int> nullList = null!;

        // Act
        bool result = nullList.EndsWith(1);

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that the method returns true when the list ends with the specified string.
    /// </summary>
    [Test]
    public void EnumerableExtension_EndsWith_TestWithEndingString_ReturnsTrue()
    {
        // Arrange
        List<string> list = ["Hello", "World"];

        // Act
        bool result = list.EndsWith("World");

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that the method returns false when the list does not end with the specified string.
    /// </summary>
    [Test]
    public void EnumerableExtension_EndsWith_TestWithNonEndingString_ReturnsFalse()
    {
        // Arrange
        List<string> list = ["Hello", "World"];

        // Act
        bool result = list.EndsWith("Hello");

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that the method returns true when the list ends with the specified complex object.
    /// </summary>
    [Test]
    public void EnumerableExtension_EndsWith_TestWithEndingComplexObject_ReturnsTrue()
    {
        // Arrange
        var complexObject = new { Id = 2, Name = "Test2" };
        List<object> list = [new { Id = 1, Name = "Test" }, complexObject];

        // Act
        bool result = list.EndsWith(complexObject);

        // Assert
        Assert.That(result, Is.True);
    }
    #endregion // Tests_EndsWith

    #region Tests_SequenceEqualOrNull

    /// <summary>
    /// Tests that the method returns true when both sequences are the same instance.
    /// </summary>
    [Test]
    public void EnumerableExtension_SequenceEqualOrNull_TestSameInstance_ReturnsTrue()
    {
        // Arrange
        List<int> sequence = [1, 2, 3];

        // Act
        bool result = sequence.SequenceEqualOrNull(sequence);

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that the method returns true when both sequences are null.
    /// </summary>
    [Test]
    public void EnumerableExtension_SequenceEqualOrNull_TestBothNull_ReturnsTrue()
    {
        // Arrange
        List<int> sequence1 = null!;
        List<int> sequence2 = null!;

        // Act
        bool result = sequence1.SequenceEqualOrNull(sequence2);

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that the method returns false when one sequence is null and the other is not.
    /// </summary>
    [Test]
    public void EnumerableExtension_SequenceEqualOrNull_TestOneNull_ReturnsFalse()
    {
        // Arrange
        List<int> sequence1 = [1, 2, 3];
        List<int> sequence2 = null!;

        // Act
        bool result = sequence1.SequenceEqualOrNull(sequence2);

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that the method returns true when both sequences contain the same elements.
    /// </summary>
    [Test]
    public void EnumerableExtension_SequenceEqualOrNull_TestEqualSequences_ReturnsTrue()
    {
        // Arrange
        List<int> sequence1 = [1, 2, 3];
        List<int> sequence2 = [1, 2, 3];

        // Act
        bool result = sequence1.SequenceEqualOrNull(sequence2);

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Tests that the method returns false when both sequences contain different elements.
    /// </summary>
    [Test]
    public void EnumerableExtension_SequenceEqualOrNull_TestNonEqualSequences_ReturnsFalse()
    {
        // Arrange
        List<int> sequence1 = [1, 2, 3];
        List<int> sequence2 = [3, 2, 1];

        // Act
        bool result = sequence1.SequenceEqualOrNull(sequence2);

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Tests that the method returns true when both sequences contain the same elements with a custom comparer.
    /// </summary>
    [Test]
    public void EnumerableExtension_SequenceEqualOrNull_TestEqualSequencesWithComparer_ReturnsTrue()
    {
        // Arrange
        List<string> sequence1 = ["hello", "world"];
        List<string> sequence2 = ["HELLO", "WORLD"];

        // Act
        bool result = sequence1.SequenceEqualOrNull(sequence2, StringComparer.OrdinalIgnoreCase);

        // Assert
        Assert.That(result, Is.True);
    }
    #endregion // Tests_SequenceEqualOrNull

    #region Tests_IndexOf_with_Comparer

    /// <summary>
    /// Tests that IndexOf with comparer returns the correct index when the element is found.
    /// </summary>
    [Test]
    public void EnumerableExtension_IndexOf_WithComparer_ElementFound_ReturnsCorrectIndex()
    {
        // Arrange
        List<string> list = ["apple", "banana", "orange"];

        // Act
        int index = list.IndexOf("banana", StringComparer.OrdinalIgnoreCase);

        // Assert
        Assert.That(index, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that IndexOf with comparer returns -1 when the element is not found.
    /// </summary>
    [Test]
    public void EnumerableExtension_IndexOf_WithComparer_ElementNotFound_ReturnsMinusOne()
    {
        // Arrange
        List<string> list = ["apple", "banana", "orange"];

        // Act
        int index = list.IndexOf("grape", StringComparer.OrdinalIgnoreCase);

        // Assert
        Assert.That(index, Is.EqualTo(-1));
    }

    /// <summary>
    /// Tests that IndexOf with comparer returns -1 when the source collection is empty.
    /// </summary>
    [Test]
    public void EnumerableExtension_IndexOf_WithComparer_EmptySource_ReturnsMinusOne()
    {
        // Arrange
        List<string> list = [];

        // Act
        int index = list.IndexOf("apple", StringComparer.OrdinalIgnoreCase);

        // Assert
        Assert.That(index, Is.EqualTo(-1));
    }

    /// <summary>
    /// Tests that IndexOf with comparer throws ArgumentNullException when source is null.
    /// </summary>
    [Test]
    public void EnumerableExtension_IndexOf_WithComparer_NullSource_ThrowsArgumentNullException()
    {
        // Arrange
        List<string> list = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => list.IndexOf("apple", StringComparer.OrdinalIgnoreCase));
    }
    #endregion // Tests_IndexOf_with_Comparer

    #region Tests_IndexOf_with_Predicate

    /// <summary>
    /// Tests that IndexOf with predicate returns the correct index when the element is found.
    /// </summary>
    [Test]
    public void EnumerableExtension_IndexOf_WithPredicate_ElementFound_ReturnsCorrectIndex()
    {
        // Arrange
        List<int> list = [1, 2, 3, 4, 5];

        // Act
        int index = list.IndexOf(x => x == 3);

        // Assert
        Assert.That(index, Is.EqualTo(2));
    }

    /// <summary>
    /// Tests that IndexOf with predicate returns -1 when the element is not found.
    /// </summary>
    [Test]
    public void EnumerableExtension_IndexOf_WithPredicate_ElementNotFound_ReturnsMinusOne()
    {
        // Arrange
        List<int> list = [1, 2, 3, 4, 5];

        // Act
        int index = list.IndexOf(x => x == 6);

        // Assert
        Assert.That(index, Is.EqualTo(-1));
    }

    /// <summary>
    /// Tests that IndexOf with predicate returns -1 when the source collection is empty.
    /// </summary>
    [Test]
    public void EnumerableExtension_IndexOf_WithPredicate_EmptySource_ReturnsMinusOne()
    {
        // Arrange
        List<int> list = [];

        // Act
        int index = list.IndexOf(x => x == 1);

        // Assert
        Assert.That(index, Is.EqualTo(-1));
    }

    /// <summary>
    /// Tests that IndexOf with predicate throws ArgumentNullException when source is null.
    /// </summary>
    [Test]
    public void EnumerableExtension_IndexOf_WithPredicate_NullSource_ThrowsArgumentNullException()
    {
        // Arrange
        List<int> list = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => list.IndexOf(x => x == 1));
    }

    /// <summary>
    /// Tests that IndexOf with predicate throws ArgumentNullException when predicate is null.
    /// </summary>
    [Test]
    public void EnumerableExtension_IndexOf_WithPredicate_NullPredicate_ThrowsArgumentNullException()
    {
        // Arrange
        List<int> list = [];

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => list.IndexOf(null));
    }

    #endregion // Tests_IndexOf_with_Predicate

    #region Tests_SafeForEach

    [Test()]
    public void EnumerableExtension_SafeForEachTest()
    {
        const int count = 5;
        var couples = Enumerable.Repeat(0, count).Select((n, i) => new { i, n });
        Dictionary<int, int> dict = [];

        couples.SafeForEach(couple => dict.Add(couple.i, couple.n));
        Assert.That(dict, Has.Count.EqualTo(count));
    }
    #endregion // Tests_SafeForEach

    #region Tests_ForEachWithIndex

    // Tests that action is executed for each element with its index
    [Test]
    public void EnumerableExtension_ActionIsExecutedForEachElementWithIndex()
    {
        // Arrange
        List<int> list = [1, 2, 3];
        List<int> indexes = [];

        // Act
        EnumerableExtensions.ForEachWithIndex(list, (item, index) => indexes.Add(index));

        // Assert
        Assert.That(indexes, Is.EqualTo(new List<int> { 0, 1, 2 }));
    }

    // Tests that no action is executed when source collection is empty
    [Test]
    public void EnumerableExtension_EmptySource_NoActionIsExecuted()
    {
        // Arrange
        List<int> emptyList = [];
        bool actionExecuted = false;

        // Act
        EnumerableExtensions.ForEachWithIndex(emptyList, (item, index) => actionExecuted = true);

        // Assert
        Assert.That(actionExecuted, Is.False);
    }

    // Tests that ArgumentNullException is thrown when source collection is null
    [Test]
    public void EnumerableExtension_NullSource_ThrowsArgumentNullException()
    {
        // Arrange
        List<int> nullList = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.ForEachWithIndex(nullList, (item, index) => { }));
    }

    // Tests that ArgumentNullException is thrown when action is null
    [Test]
    public void EnumerableExtension_NullAction_ThrowsArgumentNullException()
    {
        // Arrange
        List<int> list = [];

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.ForEachWithIndex(list, null));
    }
    #endregion // Tests_ForEachWithIndex

    #region Tests_WithIndex

    [Test]
    public void EnumerableExtension_WithIndex_ReturnsExpectedIndexValuePairs()
    {
        // Arrange
        var input = new[] { "a", "b", "c" };

        // Act
        var result = input.WithIndex().ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(3));

            Assert.That(result[0], Is.EqualTo((0, "a")));
            Assert.That(result[1], Is.EqualTo((1, "b")));
            Assert.That(result[2], Is.EqualTo((2, "c")));
        });
    }

    [Test]
    public void EnumerableExtension_WithIndex_WithEmptyCollection_ReturnsEmptyList()
    {
        // Arrange
        var input = Array.Empty<string>();

        // Act
        var result = input.WithIndex().ToList();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void EnumerableExtension_WithIndex_WithNullSource_ThrowsArgumentNullException()
    {
        // Arrange
        IEnumerable<string> input = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => input.WithIndex().ToList());
    }

    [Test]
    public void EnumerableExtension_WithIndex_WithReferenceType()
    {
        // Arrange
        var input = new List<string> { "apple", "banana", "cherry" };

        // Act
        var result = input.WithIndex().ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result[0], Is.EqualTo((0, "apple")));
            Assert.That(result[1], Is.EqualTo((1, "banana")));
            Assert.That(result[2], Is.EqualTo((2, "cherry")));
        });
    }

    [Test]
    public void EnumerableExtension_WithIndex_WithValueType()
    {
        // Arrange
        var input = new[] { 10, 20, 30 };

        // Act
        var result = input.WithIndex().ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result[0], Is.EqualTo((0, 10)));
            Assert.That(result[1], Is.EqualTo((1, 20)));
            Assert.That(result[2], Is.EqualTo((2, 30)));
        });
    }
    #endregion // Tests_WithIndex

    #region Tests_FromSingle

    /// <summary>
    /// Tests the behavior of FromSingle extension method with a string input.
    /// </summary>
    [Test]
    public void EnumerableExtension_FromSingle_ReturnsSingleElement()
    {
        // Arrange
        var input = "test";

        // Act
        var result = input.FromSingle();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First(), Is.EqualTo("test"));
        });
    }
    #endregion // Tests_FromSingle

    #region Tests_Concat

    /// <summary>
    /// Tests the Concat extension method with two non-empty collections.
    /// </summary>
    [Test]
    public void EnumerableExtension_Concat_TwoNonEmptyCollections_ReturnsCombinedCollection()
    {
        // Arrange
        var first = new[] { 1, 2, 3 };
        var second = new[] { 4, 5 };
        var expected = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = first.Concat(second);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests the Concat extension method with multiple non-empty collections.
    /// </summary>
    [Test]
    public void EnumerableExtension_Concat_MultipleNonEmptyCollections_ReturnsCombinedCollection()
    {
        // Arrange
        int[] first = [1, 2];
        int[] second = [3, 4];
        int[][] additionalItems = [[5], [6, 7]];
        int[] expected = [1, 2, 3, 4, 5, 6, 7];

        // Act
        var result = first.Concat(second, additionalItems);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests the Concat extension method with empty collections.
    /// </summary>
    [Test]
    public void EnumerableExtension_Concat_EmptyCollections_ReturnsEmptyCollection()
    {
        // Arrange
        var first = Array.Empty<int>();
        var second = Array.Empty<int>();

        // Act
        var result = first.Concat(second);

        // Assert
        Assert.That(result, Is.Empty);
    }

    /// <summary>
    /// Tests the Concat extension method with null input for the first collection.
    /// </summary>
    [Test]
    public void EnumerableExtension_Concat_FirstCollectionNull_ThrowsArgumentNullException()
    {
        // Arrange
        int[] first = null!;
        var second = new[] { 1 };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => first.Concat(second).ToList());
    }

    /// <summary>
    /// Tests the Concat extension method with null input for the second collection.
    /// </summary>
    [Test]
    public void EnumerableExtension_Concat_SecondCollectionNull_ThrowsArgumentNullException()
    {
        // Arrange
        var first = new[] { 1 };
        int[] second = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => first.Concat(second).ToList());
    }

    /// <summary>
    /// Tests the Concat extension method with null input for the additional items array.
    /// </summary>
    [Test]
    public void EnumerableExtension_Concat_AdditionalItemsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var first = new[] { 1 };
        var second = new[] { 2 };
        IEnumerable<int>[] additionalItems = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => first.Concat(second, additionalItems).ToList());
    }
    #endregion // Tests_Concat

    #region Tests_Slice

    /// <summary> A test for Slice. </summary>
    [Test()]
    public void EnumerableExtension_SliceTest_01()
    {
        int[] source = null!;

        Assert.Throws<ArgumentNullException>(() => source.Slice(2, 3));
    }

    /// <summary>   A test for Slice. </summary>
    [Test()]
    public void EnumerableExtension_SliceTest_02()
    {
        int[] source = [1, 2, 3, 4, 5, 6];
        int startIndex = -1;
        int size = 3;

        Assert.Throws<ArgumentOutOfRangeException>(() => source.Slice(startIndex, size));
    }

    /// <summary> A test for Slice which should throw ArgumentOutOfRangeException. </summary>
    [Test()]
    public void EnumerableExtension_SliceTest_03()
    {
        int[] source = [1, 2, 3, 4, 5, 6];
        int startIndex = 1;
        int size = -1;

        Assert.Throws<ArgumentOutOfRangeException>(() => source.Slice(startIndex, size));
    }

    /// <summary> A test for Slice which should return expected result. </summary>
    [Test()]
    public void EnumerableExtension_SliceTest_04()
    {
        int[] source = [1, 2, 3, 4, 5, 6];
        int startIndex = 2;
        int size = 2;
        IEnumerable<int> actual = source.Slice(startIndex, size);
        IEnumerable<int> expected = [3, 4];

        Assert.That(actual, Is.EqualTo(expected));
    }
    #endregion // Tests_Slice

    #region Tests_JoinSeparatorAndNullSubstitute

    /// <summary>
    /// Tests the Join method with separator and null substitute, ensuring that a collection is properly joined into a string.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinSeparatorAndNullSubstitute_CollectionIsProperlyJoined_Test()
    {
        // Arrange
        var items = new List<string> { "a", "b", "c" };
        var separator = ", ";
        var expected = "a, b, c";

        // Act
        var result = items.Join(separator);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests the Join method with separator and null substitute, ensuring that null items are replaced with the null substitute.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinSeparatorAndNullSubstitute_NullItemsAreReplaced_Test()
    {
        // Arrange
        var items = new List<string> { "a", null!, "c" };
        var separator = ", ";
        var nullSubstitute = "null";
        var expected = "a, null, c";

        // Act
        var result = items.Join(separator, nullSubstitute);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests the Join method with separator and null substitute, ensuring that null collection throws an ArgumentNullException.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinSeparatorAndNullSubstitute_NullCollectionThrowsException_Test()
    {
        // Arrange
        List<string> items = null!;
        var separator = ", ";

        // Act & Assert
        Assert.That(() => items.Join(separator), Throws.ArgumentNullException);
    }
    #endregion // Tests_JoinSeparatorAndNullSubstitute

    #region Tests_JoinSeparatorAndConversion

    /// <summary>
    /// Tests the Join method with separator and conversion, ensuring that a collection is properly joined into a string.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinSeparatorAndConversion_CollectionIsProperlyJoined_Test()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3 };
        var separator = "-";
        static string conversion(int x) => x.ToString();
        var expected = "1-2-3";

        // Act
        var result = items.Join(separator, conversion);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests the Join method with separator and conversion, ensuring that null separator throws an ArgumentNullException.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinSeparatorAndConversion_NullSeparatorThrowsException_Test()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3 };
        string separator = null!;
        static string conversion(int x) => x.ToString();

        // Act & Assert
        Assert.That(() => items.Join(separator, conversion), Throws.ArgumentNullException);
    }

    /// <summary>
    /// Tests the Join method with separator and conversion, ensuring that null conversion throws an ArgumentNullException.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinSeparatorAndConversion_NullConversionThrowsException_Test()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3 };
        var separator = "-";
        Func<int, string> conversion = null!;

        // Act & Assert
        Assert.That(() => items.Join(separator, conversion), Throws.ArgumentNullException);
    }
    #endregion // Tests_JoinSeparatorAndConversion

    #region Tests_JoinSeparatorConversionLimitAndTermination

    /// <summary>
    /// Tests the Join method with separator, conversion, limit, and termination, ensuring that a limited collection is properly joined into a string with termination.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinSeparatorConversionLimitAndTermination_LimitedCollectionIsProperlyJoined_Test()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3, 4 };
        var separator = "-";
        static string conversion(int x) => x.ToString();
        var listLimit = 3;
        var termination = "...";
        var expected = "1-2-3-...";

        // Act
        var result = items.Join(separator, conversion, listLimit, termination);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests the Join method with separator, conversion, limit, and termination, ensuring that a collection within the limit is properly joined into a string without termination.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinSeparatorConversionLimitAndTermination_CollectionWithinLimitIsProperlyJoined_Test()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3 };
        var separator = "-";
        static string conversion(int x) => x.ToString();
        var listLimit = 3;
        var termination = "...";
        var expected = "1-2-3";

        // Act
        var result = items.Join(separator, conversion, listLimit, termination);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests the Join method with separator, conversion, limit, and termination, ensuring that negative list limit throws an ArgumentOutOfRangeException.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinSeparatorConversionLimitAndTermination_NegativeListLimitThrowsException_Test()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3 };
        var separator = "-";
        static string conversion(int x) => x.ToString();
        var listLimit = -1;
        var termination = "...";

        // Act & Assert
        Assert.That(() => items.Join(separator, conversion, listLimit, termination), Throws.TypeOf<ArgumentOutOfRangeException>());
    }
    #endregion // Tests_JoinSeparatorConversionLimitAndTermination

    #region Tests_JoinWithLastSeparator

    /// <summary>
    /// Tests the JoinWithLastSeparator method, ensuring that a collection is properly joined into a string with a different last separator.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinWithLastSeparator_CollectionIsProperlyJoined_Test()
    {
        // Arrange
        var items = new List<string> { "a", "b", "c" };
        var separator = ", ";
        var lastSeparator = " and ";
        var expected = "a, b and c";

        // Act
        var result = items.JoinWithLastSeparator(separator, lastSeparator);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests the JoinWithLastSeparator method, ensuring that a single-item collection is properly joined into a string.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinWithLastSeparator_SingleItemCollectionIsProperlyJoined_Test()
    {
        // Arrange
        var items = new List<string> { "a" };
        var separator = ", ";
        var lastSeparator = " and ";
        var expected = "a";

        // Act
        var result = items.JoinWithLastSeparator(separator, lastSeparator);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests the JoinWithLastSeparator method, ensuring that a two-item collection is properly joined into a string with the last separator.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinWithLastSeparator_TwoItemCollectionIsProperlyJoined_Test()
    {
        // Arrange
        var items = new List<string> { "a", "b" };
        var separator = ", ";
        var lastSeparator = " AND ";
        var expected = "a AND b";

        // Act
        var result = items.JoinWithLastSeparator(separator, lastSeparator);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests the JoinWithLastSeparator method, ensuring that null collection throws an ArgumentNullException.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinWithLastSeparator_NullCollectionThrowsException_Test()
    {
        // Arrange
        List<string> items = null!;

        // Act & Assert
        Assert.That(() => items.JoinWithLastSeparator(), Throws.ArgumentNullException);
    }

    /// <summary>
    /// Tests the JoinWithLastSeparator method, ensuring that null separator throws an ArgumentNullException.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinWithLastSeparator_NullSeparatorThrowsException_Test()
    {
        // Arrange
        var items = new List<string> { "a", "b", "c" };
        string separator = null!;

        // Act & Assert
        Assert.That(() => items.JoinWithLastSeparator(separator), Throws.ArgumentNullException);
    }

    /// <summary>
    /// Tests the JoinWithLastSeparator method, ensuring that null last separator throws an ArgumentNullException.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinWithLastSeparator_NullLastSeparatorThrowsException_Test()
    {
        // Arrange
        var items = new List<string> { "a", "b", "c" };
        var separator = ", ";
        string lastSeparator = null!;

        // Act & Assert
        Assert.That(() => items.JoinWithLastSeparator(separator, lastSeparator), Throws.ArgumentNullException);
    }

    /// <summary>
    /// Tests the JoinWithLastSeparator method with a conversion function, ensuring that the collection is properly joined into a string with a different last separator.
    /// </summary>
    [Test]
    public void EnumerableExtension_JoinWithLastSeparator_WithConversionFunction_Test()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3 };
        var separator = ", ";
        var expected = "1, 2 and 3";

        // Act
        var result = items.JoinWithLastSeparator(separator);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
    #endregion // Tests_JoinWithLastSeparator

    #region Tests_FindDuplicities

    /// <summary> A test for FindDuplicities, which should throw ArgumentNullExtension. </summary>
    [Test()]
    public void EnumerableExtension_FindDuplicitiesTest_01()
    {
        int[] source = null!;

        Assert.Throws<ArgumentNullException>(() => source.FindDuplicities());
    }

    /// <summary> A test for FindDuplicities, which should succeed. </summary>
    [Test()]
    public void EnumerableExtension_FindDuplicitiesTest_02()
    {
        int[] source = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        int[] actual_duplicities = source.FindDuplicities().ToArray();

        Assert.That(actual_duplicities, Is.Empty);
    }

    /// <summary> A test for FindDuplicities, which should succeed. </summary>
    [Test()]
    public void EnumerableExtension_FindDuplicitiesTest_03()
    {
        int[] source = [1, 2, 3, 4, 2, 5, 6, 4, 7, 2];
        int[] expected_duplicities = [2, 4];
        int[] actual_duplicities = source.FindDuplicities().ToArray();

        Assert.That(actual_duplicities, Is.EqualTo(expected_duplicities));
    }
    #endregion // Tests_FindDuplicities

    #region Tests_CheckNotDuplicated

    /// <summary> A test for FindDuplicities, which should succeed. </summary>
    [Test()]
    public void EnumerableExtension_CheckNotDuplicated_01()
    {
        int[] source = [1, 2, 3, 4, 5, 6, 7, 100];

        Assert.That(source.CheckNotDuplicated(nameof(source)), Is.EqualTo(source));
    }

    /// <summary> A test for FindDuplicities, which should throw ArgumentOutOfRangeException. </summary>
    [Test()]
    public void EnumerableExtension_CheckNotDuplicated_02()
    {
        int[] source = [1, 2, 3, 4, 2, 5, 6, 4, 7, 2];

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            source.CheckNotDuplicated(nameof(source), null, -1));
    }

    /// <summary> A test for FindDuplicities, which should throw ArgumentException. </summary>
    [Test()]
    public void EnumerableExtension_CheckNotDuplicated_03()
    {
        int[] source = [1, 1, 3, 3, 5, 5, 2, 2, 4, 4,];

        Assert.Throws<ArgumentException>(() =>
            source.CheckNotDuplicated(nameof(source), null, 3));
    }
    #endregion // Tests_CheckNotDuplicated

    // region Tests_TakeLast - not needed, Extension TakeLast is present in .NET 6
    // 
    // region Tests_SkipLast - not needed, Extension SkipLast is present in .NET 6
    // 
    #endregion // Tests
}

#pragma warning restore IDE0039    // Use local function                      

// Ignore Spelling: PkUtils, Utils, Impl
// 
using PK.PkUtils.Extensions;
using PK.PkUtils.MathEx;

namespace PK.PkUtils.NUnitTests.MathExTests;

#pragma warning disable CA1859    // Change type of variable ...


/// <summary> This is a test class for <see cref="QuickSortImpl"/> </summary>
[TestFixture()]
[CLSCompliant(false)]
public class QuickSortImplTests
{
    private IQuickSorter? _sorter;

    #region Methods

    private static IReadOnlyList<IReadOnlyList<int>> GenerateAllPermutations(int n)
    {
        IReadOnlyList<int> availableNumbers = Enumerable.Range(1, n).ToList();
        var result = GenerateAllPermutations(n, availableNumbers);

        return result;
    }

    private static IReadOnlyList<IReadOnlyList<int>> GenerateAllPermutations(
        int n,
        IReadOnlyList<int> availableNumbers)
    {
        // Create a list for creation actual result
        List<List<int>> result = [];

        if (n == 1)
        {
            // basic case for one-item list 
            foreach (int number in availableNumbers)
            {
                List<int> singleList = [number];
                result.Add(singleList);
            }
        }
        else
        {
            // general case, approach with recursion
            for (int i = 0; i < availableNumbers.Count; i++)
            {
                int chosen = availableNumbers[i];
                List<int> remainingNumbers = [.. availableNumbers];
                remainingNumbers.RemoveAt(i);
                IReadOnlyList<IReadOnlyList<int>> subResult = GenerateAllPermutations(n - 1, remainingNumbers);

                foreach (IReadOnlyList<int> subList in subResult)
                {
                    List<int> newList = [chosen, .. subList];
                    result.Add(newList);
                }
            }
        }

        return result;
    }
    #endregion // Methods

    #region Tests

    [SetUp]
    public void Setup()
    {
        _sorter = new QuickSortImpl();
    }

    [Test()]
    public void SortArray_0_ElementsTest()
    {
        int[] arr = Array.Empty<int>();

        arr = _sorter.SortArray(arr);
        Assert.That(arr.IsSorted(), Is.True);
    }


    [TestCase(3)]
    public void SortArray_1_ElementTest(int i)
    {
        int[] arr = new int[] { i };

        arr = _sorter.SortArray(arr);
        Assert.That(arr.IsSorted(), Is.True);
    }


    [TestCase(1, 0)]
    [TestCase(0, 1)]
    public void SortArray_2_ElementsTest(int i, int j)
    {
        int[] arr = new int[] { i, j };

        arr = _sorter.SortArray(arr);
        Assert.That(arr.IsSorted(), Is.True);
    }

    [TestCase(new int[] { 2, 4, 6 })]
    [TestCase(new int[] { 2, 6, 4 })]
    [TestCase(new int[] { 4, 2, 6 })]
    [TestCase(new int[] { 4, 6, 2 })]
    [TestCase(new int[] { 6, 2, 4 })]
    [TestCase(new int[] { 6, 4, 2 })]
    public void SortArray_3_ElementsTest(int[] numbers)
    {
        int[] arr = _sorter.SortArray(numbers);
        Assert.That(arr.IsSorted(), Is.True);
    }


    [Test]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(8)]
    public void SortArray_N_ElementsTest(int elements)
    {
        IReadOnlyList<IReadOnlyList<int>> all = GenerateAllPermutations(elements);

        foreach (IReadOnlyList<int> current in all)
        {
            int[] arr = current.ToArray();

            arr = _sorter.SortArray(arr);
            Assert.That(arr.IsSorted(), Is.True);
        }
    }

    #endregion // Tests
}

#pragma warning restore CA1859    // Change type of variable ...
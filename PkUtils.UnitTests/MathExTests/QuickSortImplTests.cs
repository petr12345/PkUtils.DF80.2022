// Ignore Spelling: PkUtils, Utils, Impl
// 
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.MathEx;

#pragma warning disable IDE0300   // Simplify collection initialization

namespace PK.PkUtils.NUnitTests.MathExTests;

[TestClass()]
public class QuickSortImplTests
{
    private IQuickSorter? _sorter;

    #region Methods

    private static bool IsSorted<T>(T[] array) where T : IComparable<T>
    {
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i].CompareTo(array[i - 1]) < 0)
            {
                return false;
            }
        }
        return true;
    }


    private static IReadOnlyList<IReadOnlyList<int>> GenerateAllPermutations(int n)
    {
        IReadOnlyList<int> availableNumbers = Enumerable.Range(1, n).ToList();
        var result = GenerateAllPermutations(n, availableNumbers);

        return result;
    }

    private static List<List<int>> GenerateAllPermutations(
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

    [TestInitialize]
    public void Setup()
    {
        _sorter = new QuickSortImpl();
    }

    [TestMethod()]
    public void SortArray_0_ElementsTest()
    {
        int[] arr = Array.Empty<int>();

        arr = _sorter.SortArray(arr);
        Assert.IsTrue(IsSorted(arr));
    }


    [TestMethod()]
    [DataRow(3)]
    public void SortArray_1_ElementTest(int i)
    {
        int[] arr = new int[] { i };

        arr = _sorter.SortArray(arr);
        Assert.IsTrue(IsSorted(arr));
    }


    [TestMethod()]
    [DataRow(1, 0)]
    [DataRow(0, 1)]
    public void SortArray_2_ElementsTest(int i, int j)
    {
        int[] arr = new int[] { i, j };

        arr = _sorter.SortArray(arr);
        Assert.IsTrue(IsSorted(arr));
    }

    [TestMethod()]
    [DataRow(new int[] { 2, 4, 6 })]
    [DataRow(new int[] { 2, 6, 4 })]
    [DataRow(new int[] { 4, 2, 6 })]
    [DataRow(new int[] { 4, 6, 2 })]
    [DataRow(new int[] { 6, 2, 4 })]
    [DataRow(new int[] { 6, 4, 2 })]
    public void SortArray_3_ElementsTest(int[] numbers)
    {
        int[] arr = _sorter.SortArray(numbers);
        Assert.IsTrue(IsSorted(arr));
    }


    [TestMethod]
    [DataRow(4)]
    [DataRow(5)]
    [DataRow(8)]
    public void SortArray_N_ElementsTest(int elements)
    {
        IReadOnlyList<IReadOnlyList<int>> all = GenerateAllPermutations(elements);

        foreach (IReadOnlyList<int> current in all)
        {
            int[] arr = current.ToArray();

            arr = _sorter.SortArray(arr);
            Assert.IsTrue(IsSorted(arr));
        }
    }

    #endregion // Tests
}
#pragma warning restore IDE0300
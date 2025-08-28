// Ignore Spelling: Utils
//
using System;

namespace PK.PkUtils.MathEx;

/// <summary> Interface for sort array. </summary>
public interface IQuickSorter
{
    /// <summary> Sort array. </summary>
    ///
    /// <param name="array">    The array. </param>
    /// <param name="leftIndex">    Zero-based index of the left. </param>
    /// <param name="rightIndex">   Zero-based index of the right. </param>
    ///
    /// <returns> The sorted array. </returns>
    int[] SortArray(int[] array, int leftIndex, int rightIndex);
}

/// <summary> A quick sorter class extensions. </summary>
public static class QuickSorterExtensions
{
    /// <summary> An IQuickSorter extension method that sorts array. </summary>
    ///
    /// <exception cref="ArgumentNullException">   Thrown when one or more required arguments are null. </exception>
    ///
    /// <param name="sorter">   The sorter to be used. </param>
    /// <param name="array">    The array to act on. </param>
    /// <returns> The sorted array. </returns>
    public static int[] SortArray(this IQuickSorter sorter, int[] array)
    {
        ArgumentNullException.ThrowIfNull(sorter);
        ArgumentNullException.ThrowIfNull(array);

        return sorter.SortArray(array, 0, array.Length - 1);
    }
}

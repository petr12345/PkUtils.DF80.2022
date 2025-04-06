// Ignore Spelling: Utils, Impl

namespace PK.PkUtils.MathEx;

#pragma warning disable IDE0180 // Use tuple to swap values

/// <summary> A quick sort implementation. </summary>
public class QuickSortImpl : IQuickSorter
{
    /// <inheritdoc/>
    public int[] SortArray(int[] array, int leftIndex, int rightIndex)
    {
        if (leftIndex < rightIndex)
        {
            var i = leftIndex;
            var j = rightIndex;
            var pivot = array[leftIndex];

            while (i <= j)
            {
                while (array[i] < pivot)
                {
                    i++;
                }

                while (array[j] > pivot)
                {
                    j--;
                }

                if (i <= j)
                {
                    if (i < j)
                    {
                        int temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                    i++;
                    j--;
                }
            }

            if (leftIndex < j)
                SortArray(array, leftIndex, j);
            if (i < rightIndex)
                SortArray(array, i, rightIndex);
        }
        return array;
    }
}
#pragma warning restore IDE0180 // Use tuple to swap values


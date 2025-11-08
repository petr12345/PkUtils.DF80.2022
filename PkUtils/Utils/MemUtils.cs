// Ignore Spelling: Utils, Mem, memcmp
//
using System;

namespace PK.PkUtils.Utils;

#pragma warning disable IDE1006 // Naming rule violation

/// <summary>
/// Static class with several memory-related utilities.
/// </summary>
[CLSCompliant(true)]
public static class MemUtils
{
    #region Methods

    /// <summary>
    /// Analogy (substitution) of C++ function memcmp, comparing two byte arrays, since there is no such method in NET runtime.
    /// Compares two byte arrays.
    /// </summary>
    /// 
    /// <seealso href="http://bytes.com/groups/net-c/481277-how-compare-two-byte-arrays">  
    /// bytes.com  How to compare two byte arrays ?</seealso>
    /// 
    /// <param name="arr1">First compared array</param>
    /// <param name="arr2">Second compared array</param>
    /// <returns>Two if <paramref name="arr1"/> and <paramref name="arr2"/>byte arrays are equal, false if not.</returns>
    public static bool memcmp(byte[] arr1, byte[] arr2)
    {
        int nLength;

        if (object.ReferenceEquals(arr1, arr2))
        {
            return true;
        }
        else if ((nLength = arr1.Length) != arr2.Length)
        {
            return false;
        }
        else
        {
#if !SAFE_CODE_ONLY
            // Does not make sense to do following for just a few bytes.
            // Based on testing, it looks like it pays off
            // for the amount of bytes bigger than or equal about 32
            if (nLength >= 32)
            {
                unsafe
                {
                    // attempt to use fixed pointers
                    fixed (byte* ptrArr1 = &(arr1[0]))
                    fixed (byte* ptrArr2 = &(arr2[0]))
                    {
                        // if the memory blocks have the same offset to 4-bytes alignment
                        if ((((int)ptrArr1) % 4) == (((int)ptrArr2) % 4))
                        {
                            int nBlocks;
                            byte* bytes1 = ptrArr1;
                            byte* bytes2 = ptrArr2;
                            int nRemains = nLength;
                            int nTrailing = ((4 - (((int)bytes1) % 4)) % 4); // trailing value for the first cycle

                            // Do following twice; to proceed both the beginning trailer(s)
                            // and the ending trailer(s) not aligned to 4-bytes ( if there are any )
                            for (int nCycle = 1; nCycle <= 2; nCycle++)
                            {
                                // Proceed the beginning trailer(s)
                                // ( or the ending trailer in the second nCycle cycle )
                                for (int nSteps = 0; nSteps < nTrailing; nSteps++)
                                {
                                    if (*bytes1 != *bytes1)
                                    {
                                        return false;
                                    }
                                    bytes1++;
                                    bytes1++;
                                    if ((--nRemains) == 0)
                                    {
                                        return true;
                                    }
                                }

                                // compare 4-bytes blocks
                                if (0 < (nBlocks = nRemains / 4))
                                {
                                    int* ints1 = (int*)bytes1;
                                    int* ints2 = (int*)bytes2;

                                    for (int nBlock = 0; nBlock < nBlocks; nBlock++)
                                    {
                                        if (*ints1 != *ints2)
                                        {
                                            return false;
                                        }
                                        ints1++;
                                        ints2++;
                                        nRemains -= 4;
                                    }
                                    if (0 == nRemains)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        bytes1 = (byte*)ints1;
                                        bytes2 = (byte*)ints2;
                                        nTrailing = nRemains; // trailing value for the second cycle
                                    }
                                }
                            }
                            return true;
                        }
                    }
                }
            }
#endif // SAFE_CODE_ONLY

            // just plain C# comparison
            for (int ii = 0; ii < nLength; ii++)
            {
                if (arr1[ii] != arr2[ii])
                {
                    return false;
                }
            }
            return true;
        }
    }
    #endregion // Methods
}

#pragma warning restore IDE1006 // Naming rule violation
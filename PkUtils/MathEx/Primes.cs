// Ignore Spelling: PkUtils, Utils, Cloneable
// 
// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace PK.PkUtils.MathEx;
#pragma warning disable IDE0301 // Simplify collection initialization

/// <summary>
/// Static class containing primes-related methods
/// </summary>
public static class Primes
{
    /// <summary> Finds all primes lower or equal to <paramref name="maxValue"/>. </summary>
    ///
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when argument <paramref name="maxValue"/> is not
    /// positive number. </exception>
    ///
    /// <param name="maxValue"> The maximum value. Has to be positive. </param>
    ///
    /// <returns>   Resulting sequence of primes. </returns>
    public static IEnumerable<int> GeneratePrimesInRange(int maxValue)
    {
        if (maxValue < 1) throw new ArgumentOutOfRangeException(
            nameof(maxValue), maxValue, "Input argument must be positive");

        IEnumerable<int> result;

        if (maxValue < 2)
        {
            result = Enumerable.Empty<int>();
        }
        else
        {
            result = Enumerable.Range(start: 2, count: maxValue - 1)
                       .Where(n => !Enumerable.Range(2, n - 1)
                           .TakeWhile(q => q * q <= n)
                           .Where(r => n % r == 0)
                           .Any());
        }

        return result;
    }

    /// <summary>
    /// Generates a sequence of prime numbers within the specified range.
    /// </summary>
    /// <param name="minValue">The minimum value of the range (inclusive). Must be a positive integer.</param>
    /// <param name="maxValue">The maximum value of the range (inclusive). Must be a positive integer.</param>
    /// <returns>An IEnumerable containing prime numbers within the specified range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="minValue"/> or <paramref name="maxValue"/> is less than 1.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
    /// </exception>
    public static IEnumerable<int> GeneratePrimesInRange(int minValue, int maxValue)
    {
        if (minValue < 1) throw new ArgumentOutOfRangeException(
            nameof(minValue), minValue, "Input argument must be positive");

        if (maxValue < 1) throw new ArgumentOutOfRangeException(
            nameof(maxValue), maxValue, "Input argument must be positive");

        if (minValue > maxValue) throw new ArgumentException(
            "minValue must be less than or equal to maxValue");

        if (maxValue < 2) return Enumerable.Empty<int>();

        minValue = Math.Max(minValue, 2);

        return Enumerable.Range(minValue, maxValue - minValue + 1)
            .Where(n => !Enumerable.Range(2, (int)Math.Sqrt(n) - 1)
                .Where(r => r > 1)
                .Any(r => n % r == 0));
    }
}
#pragma warning restore IDE0301
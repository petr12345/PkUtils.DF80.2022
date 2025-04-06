using System;
using System.Collections.Generic;

namespace PK.PkUtils.Comparers;

#pragma warning disable IDE0290     // Use primary constructor

/// <summary>
/// Combines two <see cref="IComparer{T}"/> instances into one.
/// The first comparer has priority, and if it returns 0 (equal), the second comparer is used.
/// </summary>
/// <typeparam name="T">The type of objects to compare.</typeparam>
[Serializable]
public class CompositeComparer<T> : IComparer<T>
{
    private readonly IComparer<T> _firstComparer;
    private readonly IComparer<T> _secondComparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeComparer{T}"/> class.
    /// </summary>
    /// <param name="firstComparer">The primary comparer used for the initial comparison.</param>
    /// <param name="secondComparer">The secondary comparer used if the primary comparison results in equality.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="firstComparer"/> or <paramref name="secondComparer"/> is null.</exception>
    public CompositeComparer(IComparer<T> firstComparer, IComparer<T> secondComparer)
    {
        this._firstComparer = firstComparer ?? throw new ArgumentNullException(nameof(firstComparer));
        this._secondComparer = secondComparer ?? throw new ArgumentNullException(nameof(secondComparer));
    }

    #region IComparer<T> Members

    /// <summary>
    /// Compares two objects using the primary comparer first, followed by the secondary comparer if needed.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>
    /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>:
    /// less than zero if <paramref name="x"/> is less than <paramref name="y"/>,
    /// zero if they are equal, and greater than zero if <paramref name="x"/> is greater than <paramref name="y"/>.
    /// </returns>
    public int Compare(T x, T y)
    {
        int firstComparison = _firstComparer.Compare(x, y);

        return firstComparison != 0 ? firstComparison : _secondComparer.Compare(x, y);
    }
    #endregion // IComparer<T> Members
}

/// <summary>
/// Provides factory methods for creating instances of <see cref="CompositeComparer{T}"/>.
/// </summary>
public static class CompositeComparer
{
    /// <summary>
    /// Creates a new instance of <see cref="CompositeComparer{T}"/> by combining two comparers.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    /// <param name="firstComparer">The primary comparer used for the initial comparison.</param>
    /// <param name="secondComparer">The secondary comparer used if the primary comparison results in equality.</param>
    /// <returns>A new instance of <see cref="CompositeComparer{T}"/> that combines both comparers.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="firstComparer"/> or <paramref name="secondComparer"/> is null.</exception>
    public static CompositeComparer<T> Create<T>(IComparer<T> firstComparer, IComparer<T> secondComparer)
    {
        return new CompositeComparer<T>(firstComparer, secondComparer);
    }

    /// <summary> Creates null safe comparer. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="firstComparer"> The primary comparer used for the initial comparison. </param>
    /// <param name="secondComparer"> The secondary comparer used if the primary comparison results in equality. </param>
    /// <returns>   The new null safe comparer. </returns>
    public static CompositeComparer<T> CreateNullSafeComparer<T>(
        IComparer<T> firstComparer,
        IComparer<T> secondComparer) where T : class
    {
        ArgumentNullException.ThrowIfNull(firstComparer);
        ArgumentNullException.ThrowIfNull(secondComparer);

        // Wrap the first comparer with null-safe logic
        var nullSafeFirstComparer = Comparer<T>.Create((x, y) =>
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            return firstComparer.Compare(x, y);
        });

        // Wrap the second comparer similarly
        var nullSafeSecondComparer = Comparer<T>.Create((x, y) =>
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            return secondComparer.Compare(x, y);
        });

        return new CompositeComparer<T>(nullSafeFirstComparer, nullSafeSecondComparer);
    }
}
#pragma warning restore IDE0290     // Use primary constructor
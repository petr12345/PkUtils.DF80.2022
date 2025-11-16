// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Globalization;


namespace PK.PkUtils.Extensions;

/// <summary>
/// Static class containing methods extending the HashSet generic
/// </summary>
[CLSCompliant(true)]
public static class HashSetExtensions
{
    /// <summary> A HashSet extension method that modifies the <paramref name="target"/> to contain only elements 
    ///             that are present in that object and in the specified collection <paramref name="other"/>,
    ///             and returns the resulting HashSet. 
    /// </summary>
    /// <remarks> The purpose of the method is to call IntersectWith AND return the reference to result. 
    ///           That could be useful for instance in Linq expressions. 
    ///           For some reason, HashSet.IntersectWith does not do that.
    /// </remarks>
    ///
    /// <typeparam name="T">    Generic type parameter - type of the items in HashSet. Can't be null. </typeparam>
    /// <param name="target">   The target HashSet to act on. </param>
    /// <param name="other">    The collection to compare to the current HashSet{T} object. </param>
    ///
    /// <returns> A HashSet after IntersectWith. </returns>
    public static HashSet<T> IntersectWithRef<T>(this HashSet<T> target, IEnumerable<T> other)
    {
        target.IntersectWith(other);

        return target;
    }

    /// <summary> A HashSet extension method that modifies the <paramref name="target"/> 
    ///             by removal all elements in the specified collection <paramref name="other"/>,
    ///             and returns the resulting HashSet. 
    /// </summary>
    /// <remarks> The purpose of the method is to call ExceptWith AND return the reference to result.
    ///           That could be useful for instance in Linq expressions.
    ///           For some reason, HashSet.ExceptWith does not do that.
    /// </remarks>
    ///
    /// <typeparam name="T">    Generic type parameter - type of the items in HashSet. Can't be null. </typeparam>
    /// <param name="target">   The target HashSet to act on. </param>
    /// <param name="other">    The collection to compare to the current HashSet{T} object. </param>
    ///
    /// <returns> A HashSet after IntersectWith. </returns>
    public static HashSet<T> ExceptWithRef<T>(this HashSet<T> target, IEnumerable<T> other)
    {
        target.ExceptWith(other);

        return target;
    }

    /// <summary>
    /// Adding a new item to the HashSet. Throws ArgumentException if the item is present already.
    /// </summary>
    /// <typeparam name="T">The type of elements in the hash set.</typeparam>
    /// <param name="hashSet">The hash set that is subject of the operation.</param>
    /// <param name="item">The item being inserted into <paramref name="hashSet"/>.</param>
    public static void AddNew<T>(this HashSet<T> hashSet, T item)
    {
        ArgumentNullException.ThrowIfNull(hashSet);

        if (!hashSet.Add(item))
        {
            string strErr = string.Format(CultureInfo.InvariantCulture,
              "The item '{0}' is already present in HashSet", item);
            throw new ArgumentException(strErr, nameof(item));
        }
    }

    /// <summary>
    /// Removing an existing item from the HashSet. 
    /// Throws ArgumentException if the item is not present in the hashSet.
    /// </summary>
    /// <typeparam name="T">The type of elements in the hash set.</typeparam>
    /// <param name="hashSet">The hash set that is subject of the operation.</param>
    /// <param name="item">The item being removed from <paramref name="hashSet"/>.</param>
    public static void RemoveExisting<T>(this HashSet<T> hashSet, T item)
    {
        ArgumentNullException.ThrowIfNull(hashSet);

        if (!hashSet.Remove(item))
        {
            string strErr = string.Format(CultureInfo.InvariantCulture,
              "The item '{0}' is not present in HashSet", item);
            throw new ArgumentException(strErr, nameof(item));
        }
    }
}
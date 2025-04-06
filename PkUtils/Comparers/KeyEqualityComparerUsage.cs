/***************************************************************************************************************
*
* FILE NAME:   .\Comparers\UsageKeyEqualityComparer.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION:  Contains the static class UsageKeyEqualityComparer
*
**************************************************************************************************************/


// Ignore Spelling: Comparers, Utils
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace PK.PkUtils.Comparers;

/// <summary>
/// Static class containing various extension methods for the support of usage of
/// <see cref="KeyEqualityComparer{T, TKey}"/> </summary>
[CLSCompliant(true)]
public static class KeyEqualityComparerUsage
{
    /// <summary>
    /// Produces distinct elements from a sequence <paramref name="source"/>, by using a KeyEqualityComparer 
    /// constructed from given <paramref name="keyExtractor "/> argument. </summary>
    ///
    /// <typeparam name="TSource">  The type of enumerated objects. </typeparam>
    /// <typeparam name="TKey"> The type of keys to which the <typeparamref name="TSource"/>objects 
    /// are eventually converted for comparison purpose. </typeparam>
    /// <param name="source">  The sequence to remove duplicate elements from. </param>
    /// <param name="keyExtractor"> Method converting the compared values to compared keys. 
    ///  Must not equal to null. </param>
    /// <param name="keyComparer"> (Optional) Key comparer. </param>
    ///
    /// <returns>
    /// Returns distinct elements from a sequence <paramref name="source"/> by using a KeyEqualityComparer
    /// constructed from given <paramref name="keyExtractor"/> argument.
    /// </returns>
    public static IEnumerable<TSource> Distinct<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keyExtractor,
        IEqualityComparer<TKey> keyComparer = null)
    {
        return source.Distinct(KeyEqualityComparer.Create(keyExtractor, keyComparer));
    }

    /// <summary>
    /// Determines whether a sequence contains a specified element, by using a KeyEqualityComparer
    /// constructed from given <paramref name="keyExtractor"/> argument. </summary>
    ///
    /// <typeparam name="TSource">  The type of enumerated objects. </typeparam>
    /// <typeparam name="TKey">    The type of keys to which the <typeparamref name="TSource"/>objects
    ///   are eventually converted for comparison purpose. </typeparam>
    /// <param name="source">       A sequence in which to locate an element <paramref name="item"/> </param>
    /// <param name="item">         Element to be located in the sequence. </param>
    /// <param name="keyExtractor"> Method converting the compared values to compared keys. Must not
    ///   equal to null. </param>
    /// <param name="keyComparer"> (Optional) Key comparer. </param>
    ///
    /// <returns> true if the source sequence contains an element that has the specified 
    /// <paramref name="item"/>; otherwise, false. </returns>
    public static bool Contains<TSource, TKey>(
        this IEnumerable<TSource> source,
        TSource item,
        Func<TSource, TKey> keyExtractor,
        IEqualityComparer<TKey> keyComparer = null)
    {
        return source.Contains(item, KeyEqualityComparer.Create(keyExtractor, keyComparer));
    }

    /// <summary>
    /// Produces the set difference of two sequences, by using a KeyEqualityComparer constructed from given
    /// <paramref name="keyExtractor "/> argument. </summary>
    ///
    /// <typeparam name="TSource">  The type of enumerated objects. </typeparam>
    /// <typeparam name="TKey">    The type of keys to which the <typeparamref name="TSource"/>objects
    ///   are eventually converted for comparison purpose. </typeparam>
    /// <param name="first">        The first sequence. </param>
    /// <param name="second">       The second sequence. </param>
    /// <param name="keyExtractor"> Method converting the compared values to compared keys. Must not
    ///   equal to null. </param>
    /// <param name="keyComparer"> (Optional) Key comparer. </param>
    ///
    /// <returns>
    /// A sequence that contains the set difference of the elements of the <paramref name="first"/> and
    /// <paramref name="second"/> sequence. </returns>
    public static IEnumerable<TSource> Except<TSource, TKey>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        Func<TSource, TKey> keyExtractor,
        IEqualityComparer<TKey> keyComparer = null)
    {
        return first.Except(second, KeyEqualityComparer.Create(keyExtractor, keyComparer));
    }

    /// <summary>
    /// Produces the set intersection of two sequences, by using a KeyEqualityComparer constructed from
    /// given <paramref name="keyExtractor "/> argument. </summary>
    ///
    /// <typeparam name="TSource">  The type of enumerated objects. </typeparam>
    /// <typeparam name="TKey">    The type of keys to which the <typeparamref name="TSource"/>objects
    ///  are eventually converted for comparison purpose. </typeparam>
    /// <param name="first"> An <see cref="IEnumerable{TSource}"/> sequence whose distinct elements that
    ///  also appear in second sequence will be returned. </param>
    /// <param name="second">  An <see cref="IEnumerable{TSource}"/> sequence whose distinct elements that
    ///  also appear in first sequence will be returned. </param>
    /// <param name="keyExtractor"> Method converting the compared values to compared keys. Must not
    ///  equal to null. </param>
    /// <param name="keyComparer"> (Optional) Key comparer. </param>
    ///
    /// <returns>
    /// Returns the set intersection of the <paramref name="first"/> and <paramref name="second"/>
    /// sequences. </returns>
    public static IEnumerable<TSource> Intersect<TSource, TKey>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        Func<TSource, TKey> keyExtractor,
        IEqualityComparer<TKey> keyComparer = null)
    {
        return first.Intersect(second, KeyEqualityComparer.Create(keyExtractor, keyComparer));
    }

    /// <summary>
    /// Determines whether two sequences are equal by comparing their elements, by using a KeyEqualityComparer
    /// constructed from given <paramref name="keyExtractor "/> argument. </summary>
    ///
    /// <typeparam name="TSource">  The type of enumerated objects. </typeparam>
    /// <typeparam name="TKey">    The type of keys to which the <typeparamref name="TSource"/>objects
    ///  are eventually converted for comparison purpose. </typeparam>
    /// 
    /// <param name="first"> A <see cref="IEnumerable{TSource}"/> sequence to be compared with 
    ///   <paramref name="second"/> </param>
    /// <param name="second"> A <see cref="IEnumerable{TSource}"/> sequence to be compared with
    ///   <paramref name="first"/> </param>
    /// <param name="keyExtractor"> A delegate converting the compared values to compared keys. Must not
    ///   equal to null. </param>
    /// <param name="keyComparer"> (Optional) Key comparer. </param>
    ///
    /// <returns>
    /// true if the two source sequences are of equal length and their corresponding elements are equal
    /// according to the equality comparer KeyEqualityComparer created from
    /// <paramref name="keyExtractor"/>; otherwise, false. </returns>
    public static bool SequenceEqual<TSource, TKey>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        Func<TSource, TKey> keyExtractor,
        IEqualityComparer<TKey> keyComparer = null)
    {
        return first.SequenceEqual(second, KeyEqualityComparer.Create(keyExtractor, keyComparer));
    }

    /// <summary>
    /// Produces the set union of two sequences, by using a <see cref="KeyEqualityComparer{TSource, TKey}"/>
    /// constructed from given <paramref name="keyExtractor "/> argument. </summary>
    ///
    /// <typeparam name="TSource">  The type of enumerated objects. </typeparam>
    /// <typeparam name="TKey">    The type of keys to which the <typeparamref name="TSource"/>objects
    ///  are eventually converted for comparison purpose. </typeparam>
    /// <param name="first"> An <see cref="IEnumerable{TSource}"/> collection whose distinct elements form
    ///   the first set for the union. </param>
    /// <param name="second"> An <see cref="IEnumerable{TSource}"/> collection whose distinct elements form
    ///   the second set for the union. </param>
    /// <param name="keyExtractor"> A delegate converting the compared values to compared keys. Must not
    ///   equal to null. </param>
    /// <param name="keyComparer"> (Optional) Key comparer. </param>
    /// 
    ///
    /// <returns> An <see cref="IEnumerable{TSource}"/> that contains the elements from both input sequences,
    /// excluding duplicates. </returns>
    public static IEnumerable<TSource> Union<TSource, TKey>(
        this IEnumerable<TSource> first,
        IEnumerable<TSource> second,
        Func<TSource, TKey> keyExtractor,
        IEqualityComparer<TKey> keyComparer = null)
    {
        return first.Union(second, KeyEqualityComparer.Create(keyExtractor, keyComparer));
    }

    /// <summary> Sorts the elements of a sequence in ascending order according to a key, by using a
    /// Comparer constructed from given <paramref name="keyComparerFn"/> argument.
    /// </summary>
    /// 
    /// <typeparam name="TSource"> The type of enumerated objects. </typeparam>
    /// <typeparam name="TKey"> The type of keys to which the <typeparamref name="TSource"/>objects
    ///  are eventually converted for comparison purpose. </typeparam>
    /// 
    /// <param name="source"> A sequence of values to order. </param>
    /// <param name="keySelectorFn"> The key selector function. </param>
    /// <param name="keyComparerFn"> The key comparer function. </param>
    /// <returns> An IOrderedEnumerable&lt;TSource&gt; whose elements are sorted according to a key.
    /// </returns>
    /// <seealso cref="OrderByDescending"/>
    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelectorFn,
        Comparison<TKey> keyComparerFn)
    {
        IComparer<TKey> keyComparer = Comparer<TKey>.Create(keyComparerFn);

        return source.OrderBy<TSource, TKey>(keySelectorFn, keyComparer);
    }

    /// <summary> Sorts the elements of a sequence in descending order according to a key, by using a
    /// Comparer constructed from given <paramref name="keyComparerFn"/> argument.
    /// </summary>
    /// 
    /// <typeparam name="TSource"> The type of enumerated objects. </typeparam>
    /// <typeparam name="TKey"> The type of keys to which the <typeparamref name="TSource"/>objects
    ///  are eventually converted for comparison purpose. </typeparam>
    /// 
    /// <param name="source"> A sequence of values to order. </param>
    /// <param name="keySelectorFn"> The key selector function. </param>
    /// <param name="keyComparerFn"> The key comparer function. </param>
    /// <returns> An IOrderedEnumerable&lt;TSource&gt; whose elements are sorted according to a key.
    /// </returns>
    /// <seealso cref="OrderBy"/>
    public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelectorFn,
        Comparison<TKey> keyComparerFn)
    {
        int ComparisonFn(TKey x, TKey y) => -keyComparerFn(x, y);
        IComparer<TKey> keyComparer = Comparer<TKey>.Create(ComparisonFn);

        return source.OrderBy<TSource, TKey>(keySelectorFn, keyComparer);
    }
}

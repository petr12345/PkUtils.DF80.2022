/***************************************************************************************************************
*
* FILE NAME:   .\Extensions\Extensions.Enmerable.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION:
*   The file contains extension-methods classes EnumerableExtensions
*
**************************************************************************************************************/

// Ignore Spelling: Utils, Concat

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.FormattableString;

#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.PkUtils.Extensions;

/// <summary>
/// Static class containing various IEnumerable extension methods.
/// </summary>
[CLSCompliant(true)]
public static class EnumerableExtensions
{
    #region Public Methods

    /// <summary>
    /// Returns true if the given collection does not contain any element, false otherwise.
    /// Throws System.ArgumentNullException if the source is null ( because of the call source.Any ).
    /// If you need safe check for null source, use IsNullOrEmpty
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">A collection being checked.</param>
    /// <returns>true if the <paramref name="source"/> source does not contain any elements; otherwise, false.
    /// </returns>
    /// <seealso href="http://blogs.teamb.com/craigstuntz/2010/04/21/38598/">
    /// In LINQ, don’t use Count() when You mean Any().</seealso>
    public static bool IsEmpty<T>(this IEnumerable<T> source)
    {
        return !source.Any<T>();
    }

    /// <summary> 'Safe' invoking the IsEmpty method. For the null argument does not throw an exception but
    /// simply returns true. </summary>
    ///
    /// <typeparam name="T"> The type of the elements of source. </typeparam>
    /// <param name="source"> A collection being checked. </param>
    ///
    /// <returns> true if the <paramref name="source"/> source is null or does not contain any
    /// elements; otherwise, false. </returns>
    ///
    /// <seealso href="http://blogs.teamb.com/craigstuntz/2010/04/21/38598/">
    /// In LINQ, don’t use Count() when you mean Any().</seealso>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
        return (source == null) || (source.IsEmpty());
    }

    /// <summary> An extension method that query if 'source' contains null element. </summary>
    /// <param name="source">   A collection being checked. may be null ( in such case, return value is false). </param>
    /// <typeparam name="T"> The type of the elements of source. </typeparam>
    /// <returns> True if there is any null element, false if not. </returns>
    public static bool ContainsNull<T>(this IEnumerable<T> source) where T : class
    {
        return (source != null) && (source.Any(x => x == null));
    }

    /// <summary>
    /// Checks if the elements in the specified sequence are sorted in non-decreasing order using the provided comparer.
    /// </summary>
    /// <typeparam name="T"> The type of elements in the sequence. </typeparam>
    /// <param name="input"> The sequence to be checked for sorted order. </param>
    /// <param name="comparer"> The comparer used to determine the order of elements. If <c>null</c>, the default
    /// comparer for the type <typeparamref name="T"/> will be used. </param>
    /// <returns>
    /// true if the elements in the sequence are sorted in non-decreasing order; otherwise, false.
    /// </returns>
    public static bool IsSorted<T>(this IEnumerable<T> input, IComparer<T> comparer = null)
    {
        ArgumentNullException.ThrowIfNull(input);
        using IEnumerator<T> enumerator = input.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            return true; // Empty sequence is considered sorted
        }

        comparer ??= Comparer<T>.Default;
        for (T previous = enumerator.Current; enumerator.MoveNext();)
        {
            if (comparer.Compare(enumerator.Current, previous) < 0)
            {
                return false;
            }
            previous = enumerator.Current;
        }

        return true;
    }

    /// <summary> An extension method that query if 'source' starts with given element. </summary>
    ///
    /// <param name="source"> A collection being checked. It may be null or empty. </param>
    /// <param name="item"> The single item being compared with first element. </param>
    /// <typeparam name="T"> The type of the elements of source. </typeparam>
    /// <returns> True if source starts with given item, false if not. </returns>
    public static bool StartsWith<T>(this IEnumerable<T> source, T item)
    {
        bool result;
        if (source is null)
        {
            result = false;
        }
        else
        {
            using IEnumerator<T> enumerator = source.GetEnumerator();
            result = enumerator.MoveNext() && EqualityComparer<T>.Default.Equals(enumerator.Current, item);
        }

        return result;
    }

    /// <summary> An extension method that query if 'source' end with given element. </summary>
    ///
    /// <param name="source"> A collection being checked. It may be null or empty.  </param>
    /// <param name="item"> The single item being compared with last element.  </param>
    /// <typeparam name="T"> The type of the elements of source. </typeparam>
    /// <returns> True if source ends with given item, false if not. </returns>
    public static bool EndsWith<T>(this IEnumerable<T> source, T item)
    {
        bool result;

        if (source.IsNullOrEmpty())
            result = false;
        else
            result = source.Last().Equals(item);

        return result;
    }


    /// <summary> Compares two sequences, using the given <paramref name="comparer"/>comparer of items.
    /// If both sequences are null, this method considers them to be equal.
    /// Note the method does NOT consider null source to be equal empty source.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter, expressing the type of elements in sequences. </typeparam>
    /// <param name="sequence1"> The first source to act on. </param>
    /// <param name="sequence2"> The second source to act on. </param>
    /// <param name="comparer"> A comparer used for comparison of values in <paramref name="sequence1"/>
    /// with <paramref name="sequence2"/>. This argument could be null; in that case the EqualityComparer{T}.Default
    /// will be used. </param>
    /// <returns> returns true if sequences are equal or both are null, false if not. </returns>
    public static bool SequenceEqualOrNull<T>(this IEnumerable<T> sequence1, IEnumerable<T> sequence2,
          IEqualityComparer<T> comparer = null)
    {
        if (ReferenceEquals(sequence1, sequence2))
            return true;

        if (sequence1 == null || sequence2 == null)
            return false;

        return sequence1.SequenceEqual(sequence2, comparer ?? EqualityComparer<T>.Default);
    }

    /// <summary> Computes the has code of <paramref name="source"/>, involving each element of the source. </summary>
    /// <typeparam name="T"> Generic type parameter, expressing the type of elements in source. </typeparam>
    /// 
    /// <param name="source"> The source for which the hash is computed. May be null. </param>
    /// <returns> Resulting hash code. </returns>
    public static int SequenceHashCode<T>(this IEnumerable<T> source)
    {
        int result = 0;

        if (source != null)
        {
            unchecked
            {
                const int seedValue = 0x2D2816FE;
                const int primeNumber = 397;

                result = source.Aggregate(seedValue, (current, item) =>
                    (current * primeNumber) + (Equals(item, default(T)) ? 0 : item.GetHashCode()));
            }
        }

        return result;
    }

    /// <summary>
    /// Gets the index of an element in an IEnumerable, using for comparison the provided comparer.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="source"/> argument is null. 
    /// </exception>
    /// <typeparam name="T"> The type of the elements of source. </typeparam>
    /// <param name="source"> A collection where the index of value <paramref name="val"/> is searched.</param>
    /// <param name="val">The value to be located in <paramref name="source"/></param>
    /// <param name="comparer">A comparer used for comparison of values in <paramref name="source"/> 
    /// with <paramref name="val"/>.
    /// This argument could be null; in that case the EqualityComparer{T}.Default will be used.
    /// </param>
    /// <returns>Found index of <paramref name="val"/>, or -1 if that value is not found.</returns>
    public static int IndexOf<T>(this IEnumerable<T> source, T val, IEqualityComparer<T> comparer = null)
    {
        ArgumentNullException.ThrowIfNull(source);

        comparer ??= EqualityComparer<T>.Default;
        var found = source.Select((a, i) => new { a, i }).FirstOrDefault(x => comparer.Equals(x.a, val));
        return found == null ? -1 : found.i;
    }

    /// <summary>
    /// Gets the index of an element in an IEnumerable, matching the predicate <paramref name="predicate"/>.
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    ///
    /// <typeparam name="T"> The type of the elements of source. </typeparam>
    /// <param name="source">   A collection where matching element is searched. Can't be null. </param>
    /// <param name="predicate">  The predicate used. Can't be null. </param>
    ///
    /// <returns> Found index of matching element, or -1 if that value is not found. </returns>
    public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        var found = source.Select((a, i) => new { a, i }).FirstOrDefault(x => predicate(x.a));
        return found == null ? -1 : found.i;
    }

    /// <summary>
    /// 'Safe' invoking the ForEach method ( accepts a null this argument without throwing an exception). <br/>
    /// </summary>
    /// <typeparam name="T"> The type of the elements of <paramref name="target"/>. </typeparam>
    /// <param name="target"> A collection whose elements are subject of <paramref name="action"/>. </param>
    /// <param name="action"> The delegate to perform on each element of the <paramref name="target"/>.
    /// </param>
    /// <remarks>The reason for need of this overload is that there is no ForEach defined on IEnumerable.
    /// And the reason for that is because ForEach(Action) existed before IEnumerable{T} existed.
    /// Since it was not added with the other extension methods, one can assume that the C# designers 
    /// felt it was a bad design and prefer the foreach construct.<br/>
    /// 
    /// For more info see
    /// <see href="http://stackoverflow.com/questions/800151/why-is-foreach-on-ilistt-and-not-on-ienumerablet">
    ///  Why is .ForEach() on IList{T} and not on IEnumerable{T} ?</see>
    /// </remarks>
    public static void SafeForEach<T>(this IEnumerable<T> target, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        // Previous version of the code ( from the reference in remarks ), requiring costly ToList() conversion.
        /* if (target != null) target.ToList().ForEach(action); */

        // following is better, isn't it
        if (target != null)
        {
            foreach (var it in target)
            {
                action(it);
            }
        }
    }

    /// <summary>
    /// An IEnumerable&lt;T&gt; extension method that applies an operation to all items in this
    /// collection.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// <typeparam name="T"> The type of the elements of <paramref name="source"/>. </typeparam>
    /// <param name="source"> A collection being enumerated. </param>
    /// <param name="action"> The delegate to perform on each element of the <paramref name="source"/>. </param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);

        foreach (T local in source)
        {
            action(local);
        }
    }

    /// <summary> An IEnumerable&lt;T&gt; extension method that applies an operation to all items in this
    /// collection, having the argument the item and its index.</summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// <typeparam name="T"> The type of the elements of <paramref name="source"/>. </typeparam>
    /// <param name="source"> A collection being enumerated. </param>
    /// <param name="action"> The delegate to perform on each element of the <paramref name="source"/>. </param>
    public static void ForEachWithIndex<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(action);

        source.WithIndex<T>().ForEach(pair => action(pair.Value, pair.Index));
    }

    /// <summary> Form the given <paramref name="source"/> collection creates a new collection 
    ///  of Tuple(s), with first item of the tuple the index of the original element,
    ///  and the second item the original element itself. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="source"/> argument is null. </exception>
    /// <typeparam name="T"> The type of elements in <paramref name="source"/> collection. </typeparam>
    /// <param name="source"> A source collection being enumerated. </param>
    /// 
    ///  <returns> Resulting source of Tuple(s) that can be iterated. </returns>
    public static IEnumerable<(int Index, T Value)> WithIndex<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        int position = 0;
        return (from value in source select (position++, value));
    }

    /// <summary> This method 'converts' a single <paramref name="item"/> to an enumerable collection,
    ///           thus allowing to pass the result to a LINQ method which expects an IEnumerable. 
    /// </summary>
    /// <typeparam name="T"> The type of the item. There are no restrictions on this type.</typeparam>
    /// <param name="item"> The single item needed to 'convert' to IEnumerable. </param>
    /// 
    /// <returns> A source of elements, containing just one element <paramref name="item"/>. </returns>
    public static IEnumerable<T> FromSingle<T>(this T item)
    {
        yield return item;
    }

    /// <summary>
    /// Returns null if the specified collection is null or empty; otherwise, returns the original collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">A collection being processed. Can be null.</param>
    /// <returns>A null value if the collection is null or empty; otherwise, the original collection.</returns>
    public static IEnumerable<T> NullIfEmpty<T>(this IEnumerable<T> source)
    {
        return ((source is null) || source.IsEmpty()) ? null : source;
    }

    /// <summary>
    /// Returns an empty collection if the specified collection is null; otherwise, returns the original collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">A collection being processed. Can be null.</param>
    /// <returns>An empty collection if the specified collection is null; otherwise, the original collection.</returns>
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
    {
#pragma warning disable IDE0301 // Simplify collection initialization
        return source ?? Enumerable.Empty<T>();
#pragma warning restore IDE0301 // Simplify collection initialization
    }

    /// <summary> Concatenates all given sequences. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more arguments are null. </exception>
    /// <typeparam name="T"> The type of the items in sequences. There are no restrictions on this type.</typeparam>
    /// <param name="first"> The first source to act on. </param>
    /// <param name="second"> The second source to act on. </param>
    /// <param name="additionalItems"> A variable-length parameters list containing 
    ///  additional sequences. </param>
    /// 
    /// <returns> An collection that contains the concatenated elements of all input sequences. </returns>
    public static IEnumerable<T> Concat<T>(
        this IEnumerable<T> first,
        IEnumerable<T> second,
        params IEnumerable<T>[] additionalItems)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(additionalItems);

        IEnumerable<T> result = Enumerable.Concat<T>(first, second);
        foreach (IEnumerable<T> items in additionalItems)
        {
            result = result.Concat(items);
        }
        return result;
    }

    /// <summary> Returns the sub-collection of the <paramref name="source"/>,
    /// skipping the beginning of source and returning the <paramref name="size"/> elements. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="source"/>is null. </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the
    ///  required range. </exception>
    /// <typeparam name="T"> Generic type parameter. The type of items in the collection.</typeparam>
    /// <param name="source"> A collection being processed. Cannot be null. </param>
    /// <param name="startIndex" type="int"> The start index. </param>
    /// <param name="size" type="int"> The size of the result. </param>
    /// 
    /// <returns> Resulting source that can be iterated. </returns>
    public static IEnumerable<T> Slice<T>(this IEnumerable<T> source, int startIndex, int size)
    {
        ArgumentNullException.ThrowIfNull(source);

        int num = source.Count<T>();
        if ((startIndex < 0) || (num < startIndex))
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }
        if ((size < 0) || ((startIndex + size) > num))
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }
        return source.Skip<T>(startIndex).Take<T>(size);
    }

    /// <summary>
    /// An extension method that joins collection elements into single string, given a separator.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter. The type of items in the collection.</typeparam>
    /// <param name="source"> A collection being processed. Cannot be null. </param>
    /// <param name="separator"> The separator of items. Cannot be null.</param>
    /// <param name="nullSubstitute"> (Optional) The value to be returned from AsString when object is null.
    /// If null, {null} will be used. </param>
    ///
    /// <returns> A resulting string. </returns>
    public static string Join<T>(this IEnumerable<T> source, string separator = ", ", string nullSubstitute = null)
    {
        return source.Join<T>(separator, item => item.AsString(nullSubstitute));
    }

    /// <summary>
    /// An extension method that joins collection elements into single string, given a separator.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// <typeparam name="T"> Generic type parameter. The type of items in the collection.</typeparam>
    /// <param name="source"> A collection being processed. Cannot be null. </param>
    /// <param name="separator"> The separator of items. Cannot be null.</param>
    /// <param name="conversion"> The conversion method. Cannot be null.</param>
    /// <returns> A resulting string. </returns>
    public static string Join<T>(this IEnumerable<T> source, string separator, Func<T, string> conversion)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(separator);
        ArgumentNullException.ThrowIfNull(conversion);

        return string.Join(separator, source.Select<T, string>(conversion));
    }

    /// <summary>
    /// An extension method that joins collection elements into single string, given a separator.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// <typeparam name="T"> Generic type parameter. The type of items in the sequence.</typeparam>
    /// <param name="source"> A collection being processed. Cannot be null. </param>
    /// <param name="separator"> The separator of items. Cannot be null.</param>
    /// <param name="conversion"> The conversion method. Cannot be null.</param>
    /// <param name="listLimit"> The limit of items listed. If source contains more items, the rest will be replace by <paramref name="termination"/>. </param>
    /// <param name="termination"> The termination line used in case there is too many lines. </param> 
    /// <returns> A resulting string. </returns>
    public static string Join<T>(
        this IEnumerable<T> source,
        string separator,
        Func<T, string> conversion,
        int listLimit,
        string termination = "...")
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(separator);
        ArgumentNullException.ThrowIfNull(conversion);

        if (listLimit <= 0) throw new ArgumentOutOfRangeException(
            nameof(listLimit), listLimit, Invariant($"Value of {nameof(listLimit)} must be positive."));
        if (listLimit == int.MaxValue)
            listLimit = int.MaxValue - 1; // prevent listLimit + 1 becoming negative
        List<string> finalItems = source.Take(listLimit + 1).Select(x => conversion(x)).ToList();
        if (finalItems.Count > listLimit)
        {
            finalItems[listLimit] = termination;
        }
        return finalItems.Join(separator);
    }

    /// <summary>
    /// An extension method that joins collection elements into single string, given a separator and a different last separator.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter. The type of items in the collection.</typeparam>
    /// <param name="source"> A collection being processed. Cannot be null. </param>
    /// <param name="separator"> The separator of items. Cannot be null.</param>
    /// <param name="lastSeparator"> The separator for the last item. Cannot be null.</param>
    /// <param name="conversion"> (Optional) The conversion method. If null, the item's ToString method will be used.</param>
    /// <returns> A resulting string. </returns>
    public static string JoinWithLastSeparator<T>(
        this IEnumerable<T> source,
        string separator = ", ",
        string lastSeparator = " and ",
        Func<T, string> conversion = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(separator);
        ArgumentNullException.ThrowIfNull(lastSeparator);

        List<string> items = source.Select(conversion ?? (x => x.AsString(null))).ToList();
        int lastIndex = items.Count - 1;
        string result = (lastIndex < 1)
            ? string.Join(separator, items)
            : string.Join(separator, items.Take(lastIndex)) + lastSeparator + items[lastIndex];

        return result;
    }

    /// <summary>
    /// An IEnumerable&lt;T&gt; extension method that converts to a detail string.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter. The type of items in the sequence.</typeparam>
    ///
    /// <param name="source">   A collection being processed. Can be null. </param>
    /// <param name="separator">    (Optional) The separator of individual items. Cannot be null. </param>
    ///
    /// <returns> The given data converted to a string. </returns>
    public static string ToDetailString<T>(this IEnumerable<T> source, string separator = ",")
    {
        string result;

        if (source == null)
        {
            result = source.AsString();
        }
        else
        {
            StringBuilder builder = new();
            result = builder.Append('[').Append(source.Join(separator)).Append(']').ToString();
        }

        return result;
    }

    /// <summary> Finds all elements that represent duplicities in given collection <paramref name="source"/>. 
    ///            For instance, if source = { 1,2,3,4,2,5,6,4,7,2}, the result is { 2, 4} .
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="source"/> argument is null. 
    /// </exception>
    ///
    /// <typeparam name="T"> Generic type parameter. The type of items in the collection.</typeparam>
    /// <param name="source">   A collection being checked for duplicities. Can't be null. </param>
    /// <param name="comparer"> (Optional) A comparer used for comparison of items in <paramref name="source"/>
    /// This argument could be null, in that case EqualityComparer{T}.Default will be used. </param>
    ///
    /// <returns>
    /// If no exception is thrown, returns the sequence of all elements that are duplicated in <paramref name="source"/>.
    /// </returns>
    public static IEnumerable<T> FindDuplicities<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null)
    {
        ArgumentNullException.ThrowIfNull(source);

        comparer ??= EqualityComparer<T>.Default;
        return source.GroupBy(x => x, y => y, comparer).Where(g => g.Count() > 1).Select(y => y.Key);
    }

    /// <summary> Throws <see cref="ArgumentException"/> in case the <paramref name="source"/> contains duplicities. </summary>
    ///
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="listLimit"/> is not positive. </exception>
    /// <exception cref="ArgumentException"> Thrown when <paramref name="source"/> contains duplicities. </exception>
    /// 
    /// <typeparam name="T"> Generic type parameter. The type of items in the collection.</typeparam>
    ///
    /// <param name="source">   The source to act on. It may be null. </param>
    /// <param name="argName">  (Optional) The name the source argument has in calling code. </param>
    /// <param name="comparer"> (Optional) A comparer used for comparison of items in <paramref name="source"/>
    /// This argument could be null, in that case EqualityComparer{T}.Default will be used. </param>
    /// <param name="listLimit">  (Optional) The maximal length of list of duplicated items 
    ///                               in the thrown exception text. Must be positive.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process check not duplicated in this collection.
    /// </returns>
    public static IEnumerable<T> CheckNotDuplicated<T>(
        this IEnumerable<T> source,
        string argName = null,
        IEqualityComparer<T> comparer = null,
        int listLimit = int.MaxValue)
    {
        IEnumerable<T> duplicities = source.FindDuplicities<T>(comparer);
        string listed = duplicities.Join(" and ", x => Invariant($"'{x}'"), listLimit);

        if (!string.IsNullOrEmpty(listed))
        {
            string sequenceName = string.IsNullOrEmpty(argName) ? string.Empty : Invariant($" '{argName}'");
            string sequenceForException = string.IsNullOrEmpty(argName) ? nameof(source) : Invariant($"'{argName}'");
            string message1 = Invariant($"The {sequenceName} cannot contain duplicate {typeof(T).Name},");
            string message2 = Invariant($" but there are duplicated values {listed}");

            throw new ArgumentException(message1 + message2, sequenceForException);
        }

        return source;
    }

    /// <summary> Take last <paramref name="count"/> elements in source <paramref name="source"/>. </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="count"/> has negative value. </exception>
    ///
    /// <typeparam name="T">    Generic type parameter - type of items in source. </typeparam>
    /// <param name="source">   A source collection. Can't be null. </param>
    /// <param name="count">    Number of last items to be returned. </param>
    ///
    /// <returns>
    /// A resulting source containing last <paramref name="count"/> items, or less if there were not enough items.
    /// </returns>
    public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (count < 0) throw new ArgumentOutOfRangeException(
            nameof(count), count, Invariant($"Value of {nameof(count)} can't be negative."));

        if (count == 0)
        {
            yield break;
        }
        else
        {
            List<T> buffer = new(count);
            int pos = 0;

            foreach (T item in source)
            {
                if (buffer.Count < count)
                {
                    // first phase
                    buffer.Add(item);
                }
                else
                {
                    // second phase
                    buffer[pos] = item;
                    pos = (pos + 1) % count;
                }
            }

            for (int ii = 0; ii < buffer.Count; ii++)
            {
                yield return buffer[pos];
                pos = (pos + 1) % count;
            }
        }
    }

    /// <summary> Skip last <paramref name="count"/> elements in source <paramref name="source"/>. </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="count"/> has negative value. </exception>
    ///
    /// <typeparam name="T">    Generic type parameter - type of items in source. </typeparam>
    /// <param name="source">   A source collection. Can't be null. </param>
    /// <param name="count">    Number of last items to be skipped. </param>
    ///
    /// <returns>
    /// A resulting source containing remaining items after skipping last <paramref name="count"/>.
    /// </returns>
    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (count < 0) throw new ArgumentOutOfRangeException(
            nameof(count), count, Invariant($"Value of {nameof(count)} can't be negative."));

        List<T> buffer = [];
        int pos = 0;

        foreach (T item in source)
        {
            if (count == 0)
            {
                yield return item;
            }
            else if (buffer.Count < count)
            {
                // first phase
                buffer.Add(item);
            }
            else
            {
                // second phase
                yield return buffer[pos];
                buffer[pos] = item;
                pos = (pos + 1) % count;
            }
        }
    }
    #endregion // Public Methods
}
#pragma warning restore IDE0305
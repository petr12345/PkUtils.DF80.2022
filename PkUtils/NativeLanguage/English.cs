// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;

#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.PkUtils.NativeLanguage;

/// <summary> A static class containing utilities related to English language. </summary>
public static class English
{
    /// <summary> Gets English—language ordinal suffix for numeric value, usable in text output. </summary>
    /// <typeparam name="N"> The concrete type of numeric value. </typeparam>
    /// <param name="num"> The input number. It is assumed is greater than zero. </param>
    /// <returns> A string like "nd", "rd", "th". For negative <paramref name= "num" /> just an empty string. </returns>
    public static string OrdinalSuffix<N>(this N num) where N : IBinaryInteger<N>
    {
        long numValue = Convert.ToInt64(num, CultureInfo.InvariantCulture);
        string result = num < default(N) ? string.Empty : (numValue % 100) switch
        {
            11 or 12 or 13 => "th",
            _ => (numValue % 10) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            }
        };

        if (num < default(N))
        {
            Trace.TraceWarning("No English ordinal for negative value");
        }

        return result;
    }

    /// <summary> Formats <paramref name="num"/> to string, adding separator and ordinal suffix, like "rd", "th" etc. </summary>
    /// <typeparam name="N"> The concrete type of numeric value. </typeparam>
    /// <param name="num"> The input number. </param>
    /// <param name="separator"> The separator. </param>
    /// <param name="provider"> (Optional) Formating provider.
    /// May be null, in that case CultureInfo.InvariantCulture will be used.
    /// </param>
    /// <returns> Resulting formatted string. </returns>
    public static string ToOrdinal<N>(
        this N num,
        string separator = null,
        IFormatProvider provider = null) where N : IBinaryInteger<N>
    {
        string ext, result;

        provider ??= CultureInfo.InvariantCulture;
        if (string.IsNullOrEmpty(ext = num.OrdinalSuffix())) // will happen for negative value
            result = string.Format(provider, "{0}", num);
        else
            result = string.Format(provider, "{0}{1}{2}", num, separator, ext);

        return result;
    }

    /// <summary> An int extension method that plurals. </summary>
    /// <typeparam name="T"> The concrete type of numeric value. </typeparam>
    /// <param name="count">    The count to act on. </param>
    /// <param name="extension"> (Optional) The extension. By default "s", but if a word ends in -s. -sh, -ch, -x
    /// or -z, you add -es. </param>
    ///
    /// <returns>   Either an extension( "s" or "es" ), or an empty string. </returns>
    public static string Plural<T>(this T count, string extension = "s")
    {
        return Convert.ToInt64(count, CultureInfo.InvariantCulture) >= 2 ? extension : string.Empty;
    }

    /// <summary> Returns plural addition ( -s ) if the given collection has more than one element. </summary>
    /// <typeparam name="T"> The concrete type of numeric value. </typeparam>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="collection"/> is null. </exception>
    /// <param name="collection">   The collection to act on. Can't be null. </param>
    /// <param name="extension"> (Optional) The extension. By default "s", but if a word ends in -s. -sh, -ch, -x
    /// or -z, you add -es. </param>
    ///
    /// <returns> Either an extension( "s" or "es" ), or an empty string. </returns>
    public static string Plural<T>(
        this IReadOnlyCollection<T> collection,
        string extension = "s")
    {
        ArgumentNullException.ThrowIfNull(collection);
        return collection.Count.Plural(extension);
    }

    /// <summary> Returns plural addition ( -s ) if the given sequence has more than one element. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="source"/> is null. </exception>
    ///
    /// <typeparam name="T"> Generic type parameter, type of elements in sequence. </typeparam>
    /// <param name="source"> The sequence to act on. Can't be null. </param>
    /// <param name="extension"> (Optional) The extension. By default "s", but if a word ends in -s. -sh, -ch, -x
    /// or -z, you add -es. </param>
    ///
    /// <returns> Either an extension( "s" or "es" ), or an empty string. </returns>
    public static string Plural<T>(
        this IEnumerable<T> source,
        string extension = "s")
    {
        ArgumentNullException.ThrowIfNull(source);
        string result = (source as IReadOnlyCollection<T> ?? source.Take(2).ToList()).Plural(extension);
        return result;
    }
}
#pragma warning restore IDE0305
/***************************************************************************************************************
*
* FILE NAME:   .\Extensions\Extensions.String.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION:
*   The file contains extension-methods class StringExtension
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

#pragma warning disable IDE0057 // Use range operator

namespace PK.PkUtils.Extensions;

/// <summary>
/// Implements extension methods for strings
/// </summary>
public static class StringExtension
{
    #region Extension Methods

    #region Conversions
    #region Conversions_string_string

    /// <summary> A string extension method that returns null if <paramref name="s"/> is null or empty. 
    ///           Otherwise, returns the original string.
    /// </summary>
    /// <param name="s"> The original string. </param>
    /// <returns> Null or <paramref name="s"/>. </returns>
    public static string NullIfEmpty(this string s)
    {
        return string.IsNullOrEmpty(s) ? null : s;
    }

    /// <summary> A string extension method that returns null if <paramref name="s"/> is null or whitespace.
    ///           Otherwise, returns the original string.
    /// </summary>
    /// <param name="s"> The original string. </param>
    /// <returns> Null or <paramref name="s"/>. </returns>
    public static string NullIfWhitespace(this string s)
    {
        return string.IsNullOrWhiteSpace(s) ? null : s;
    }

    /// <summary> A string extension method that returns string.Empty if <paramref name="s"/> is null. 
    ///           Otherwise, returns the original string.
    /// </summary>
    /// <param name="s"> The original string. </param>
    /// <returns> Returns string.Empty if <paramref name="s"/> is null; otherwise returns s. </returns>
    public static string EmptyIfNull(this string s)
    {
        return s ?? string.Empty;
    }
    #endregion // Conversions_string_string

    #region Conversions_string_enum

    /// <summary>
    /// A string extension method that converts a <paramref name="value"/> to an enum <typeparamref name="T"/>.
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when the input <paramref name="value"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when the input <paramref name="value"/> is not null but can't be
    /// converted to <typeparamref name="T"/>. </exception>
    ///
    /// <typeparam name="T">    The enum type parameter. </typeparam>
    /// <param name="value">  The string to be converted. </param>
    /// <param name="ignoreCase"> (Optional) True to ignore case. </param>
    /// <remarks> By default, ignoreCase = false, because an enum can contain values which differ only in case. </remarks>
    ///
    /// <returns>   If succeeded, returns converted value of <paramref name="value"/> to T. </returns>
    public static T Parse<T>(this string value, bool ignoreCase = false)
        where T : Enum
    {
        return EnumExtension.Parse<T>(value, ignoreCase);
    }

    /// <summary>
    /// A string extension method that converts a <paramref name="value"/> to an enum
    /// <typeparamref name="T"/>.
    /// </summary>
    ///
    /// <remarks>
    /// By default, ignoreCase = false, because an enum can contain values which differ only in case.
    /// </remarks>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    /// <param name="value">    The string to be converted. </param>
    /// <param name="fallBackValue">    The fall-back value. </param>
    /// <param name="ignoreCase">   (Optional) True to ignore case. </param>
    ///
    /// <returns>   If succeeded, returns converted value of <paramref name="value"/> to T. </returns>
    public static T ToEnum<T>(this string value, T fallBackValue, bool ignoreCase = false)
        where T : struct, Enum
    {
        return EnumExtension.ToEnum<T>(value, fallBackValue, ignoreCase);
    }
    #endregion // Conversions_string_enum

    #region Conversions_string_type

    /// <summary> A string extension method that converts a str to a type. </summary>
    ///
    /// <param name="str">  The string to be converted. </param>
    ///
    /// <returns>   Str as a T. </returns>
    public static T ToType<T>(this string str)
    {
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        T value = (T)converter.ConvertFromString(str);
        return value;
    }

    /// <summary> A string extension method that converts a str to a type. </summary>
    ///
    /// <param name="str">              The string to be converted. </param>
    /// <param name="fallBackValue">    The fallback value. </param>
    ///
    /// <returns>   Str as a T. </returns>
    public static T ToType<T>(this string str, T fallBackValue)
    {
        try
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            T value = (T)converter.ConvertFromString(str);
            return value;
        }
        catch { return fallBackValue; }
    }

    /// <summary>
    /// A string extension method that attempts to to type a T from the given string.
    /// </summary>
    ///
    /// <param name="str">      The examined string. </param>
    /// <param name="value">    [out] The value. </param>
    ///
    /// <returns>   True if it succeeds, false if it fails. </returns>
    public static bool TryToType<T>(this string str, out T value)
    {
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        if (converter.CanConvertFrom(typeof(string)))
        {
            try
            {
                value = (T)converter.ConvertFromString(str);
                return true;
            }
            catch { /* Go through to default value */ }
        }

        value = default;
        return false;
    }
    #endregion // Conversions_string_type
    #endregion // Conversions

    #region Contents_Examined

    /// <summary> A string extension method that query if <paramref name="str"/> is null or just whitespaces. </summary>
    ///
    /// <param name="str">  The examined string. </param>
    ///
    /// <returns>   True if null or whitespace, false if not. </returns>
    public static bool IsNullOrWhitespace(this string str)
    {
        return (str == null) || string.IsNullOrWhiteSpace(str);
    }

    /// <summary> Query if <paramref name="str"/> has just alpha numeric or underscore characters. </summary>
    ///
    /// <remarks> For null or empty string returns true. </remarks>
    /// <param name="str">    The examined string. </param>
    ///
    /// <returns>   True if just alpha numeric or underscore, false if not. </returns>
    public static bool IsAlphaNumericOrUnderscore(this string str)
    {
        return (string.IsNullOrEmpty(str) || str.All(ch => ch.IsAlphaNumericOrUnderscore()));
    }

    /// <summary> Throws an exception in case the <paramref name="str"/> is null or empty. </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="str"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when a supplied <paramref name="str"/> is empty. </exception>
    /// 
    /// <param name="str"> The string being checked. </param>
    /// <param name="argName"> The name of the object (for instance the name of formal argument in the calling code). </param>
    /// <returns> The original string. </returns>
    public static string CheckArgNotNullOrEmpty(this string str, string argName)
    {
        if (string.IsNullOrEmpty(argName))
        {
            argName = "string";
        }

        if (null == str)
        {
            throw new ArgumentNullException(argName, $"The argument '{argName}' is null");
        }
        if (string.IsNullOrEmpty(str))
        {
            throw new ArgumentException(
                $"The argument '{argName}' is an empty string", argName);
        }

        return str;
    }
    #endregion // Contents_Examined

    #region String_Manipulations

    /// <summary> Removes all characters <paramref name="characterList"/> from input string <paramref name="str"/>. 
    ///            In case either <paramref name="str"/> or <paramref name="characterList"/> are null or empty.
    ///            just returns the original <paramref name="str"/>.
    /// </summary>
    ///
    /// <param name="str">              The string to be converted. Can be null or empty. </param>
    /// <param name="characterList">    Sequence of characters to be removed. Can be null or empty. </param>
    ///
    /// <returns>   A string. </returns>
    public static string RemoveCharacters(this string str, IEnumerable<char> characterList)
    {
        string result;

        if (string.IsNullOrEmpty(str) || characterList.IsNullOrEmpty())
        {
            result = str;
        }
        else
        {
            StringBuilder builder = new();
            for (int i = 0; i < str.Length; i++)
            {
                char character = str[i];
                if (!characterList.Contains(character))
                {
                    builder.Append(character);
                }
            }

            result = builder.ToString();
        }

        return result;
    }

    /// <summary> Return the substring containing 'count' leftmost characters. </summary>
    /// 
    /// <param name="s"> The original string. </param>
    /// <param name="count"> The amount of characters to be returned.</param>
    /// <returns> The resulting substring.</returns>
    /// <exception cref="ArgumentNullException"> Thrown when the input argument <paramref name="s"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when the input argument <paramref name="count"/>
    /// is negative, or longer than the length of string <paramref name="s"/>.</exception>
    public static string Left(this string s, int count)
    {
        ArgumentNullException.ThrowIfNull(s);
        if ((count < 0) || (count > s.Length)) { throw new ArgumentOutOfRangeException(nameof(count)); }
        return s.Substring(0, count);
    }

    /// <summary> Return the substring containing maximally 'count' leftmost characters. </summary>
    /// 
    /// <param name="s"> The original string. </param>
    /// <param name="count"> The amount of characters to be returned.</param>
    /// <returns> The resulting substring.</returns>
    /// <exception cref="ArgumentNullException"> Thrown when the input argument <paramref name="s"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when the input argument <paramref name="count"/> is negative.</exception>
    public static string LeftMax(this string s, int count)
    {
        ArgumentNullException.ThrowIfNull(s);
        if (count < 0) { throw new ArgumentOutOfRangeException(nameof(count), count, $"{nameof(count)} cannot be negative"); }

        return s.Substring(0, Math.Min(s.Length, count));
    }

    /// <summary> Return the substring containing 'count' rightmost characters. </summary>
    /// 
    /// <param name="s">The original string</param>
    /// <param name="count">The amount of rightmost characters to be returned.</param>
    /// <returns>The resulting substring.</returns>
    /// <exception cref="ArgumentNullException"> Thrown when the input argument <paramref name="s"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when the input argument <paramref name="count"/>
    /// is negative, or longer than the length of string <paramref name="s"/>.</exception>
    public static string Right(this string s, int count)
    {
        ArgumentNullException.ThrowIfNull(s);
        if ((count < 0) || (count > s.Length)) { throw new ArgumentOutOfRangeException(nameof(count)); }
        return s.Substring(s.Length - count, count);
    }

    /// <summary> Return the substring containing maximally 'count' rightmost characters. </summary>
    /// 
    /// <param name="s">The original string</param>
    /// <param name="count">The amount of rightmost characters to be returned.</param>
    /// <returns>The resulting substring.</returns>
    /// <exception cref="ArgumentNullException"> Thrown when the input argument <paramref name="s"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when the input argument <paramref name="count"/> is negative.</exception>
    public static string RightMax(this string s, int count)
    {
        ArgumentNullException.ThrowIfNull(s);
        if (count < 0) { throw new ArgumentOutOfRangeException(nameof(count), count, $"{nameof(count)} cannot be negative"); }

        count = Math.Min(s.Length, count);
        return s.Substring(s.Length - count, count);
    }

    /// <summary> Return the substring starting on the 'index' position and containing 'count' characters. </summary>
    /// 
    /// <param name="s">The original string</param>
    /// <param name="index">The zero-based starting character position of a substring in this instance.</param>
    /// <param name="count">The amount of characters to be returned.</param>
    /// <returns>The resulting substring.</returns>
    /// <exception cref="ArgumentNullException"> Thrown when the input argument <paramref name="s"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when the input argument <paramref name="count"/> 
    /// is negative, or if index plus count indicates a position not within this instance
    /// index plus count indicates a position not within this instance. </exception>
    public static string Mid(this string s, int index, int count)
    {
        ArgumentNullException.ThrowIfNull(s);
        return s.Substring(index, count);
    }

    /// <summary>
    /// Inserts the substring strTextIns into 'this' string, on the 'index' position
    /// </summary>
    /// <param name="s">The original string. </param>
    /// <param name="index">The zero-based character position where <paramref name="strTextIns"/> should be 
    ///                     inserted.</param>
    /// <param name="strTextIns">The string to be inserted.</param>
    /// <returns>The resulting string after the insertion.</returns>
    public static string Insert(this string s, int index, string strTextIns)
    {
        string strRes = s;
        if ((0 <= index) && (index <= s.Length))
        {
            string strLeft = s.Left(index);
            string strRight = s.Right(s.Length - index);
            strRes = strLeft + strTextIns + strRight;
        }

        return strRes;
    }

    /// <summary> A string extension method that replace first occurrence of substring in a string. </summary>
    ///
    /// <param name="src">          The original string. </param>
    /// <param name="oldValue">     The substring to be replaced. </param>
    /// <param name="newValue">     The string to replace all occurrences of oldValue. </param>
    /// <param name="comparison">   The string comparison to be used. </param>
    ///
    /// <returns>   A new string after replace. </returns>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when any of input string arguments is null.</exception>
    /// <exception cref="ArgumentException"> Thrown when the argument <paramref name="comparison"/> has invalid value.</exception>/// 
    /// <seealso cref="ReplaceMaxOccurrences"/>
    public static string ReplaceFirstOccurrence(this string src, string oldValue, string newValue, StringComparison comparison)
    {
        // called ReplaceMaxOccurrences checks all arguments
        return ReplaceMaxOccurrences(src, oldValue, newValue, 1, comparison);
    }

    /// <summary> Returns a first line of the given string. </summary>
    ///
    /// <param name="src"> The original string. </param>
    ///
    /// <returns> A first line, or null if <paramref name="src"/> is null. </returns>
    public static string FirstLine(this string src)
    {
        int index;
        string firstLine = src;

        if (src != null)
        {
            index = src.IndexOfAny(['\r', '\n']);
            if (index >= 0)
            {
                firstLine = src.Left(index);
            }
        }
        return firstLine;
    }

    /// <summary>
    /// Replaces whitespaces between words with single space and removes leading and trailing whitespaces from a string.
    /// <para>In case of all whitespace chars returns empty string.</para>
    /// <para>In case of null returns null.</para>
    /// <para>F.e. from "  John    Smith.  " produces "John Smith."</para>
    /// </summary>
    /// 
    /// <param name="src">The string to normalize. It may be null or empty. </param>
    /// 
    /// <returns>Normalized string</returns>
    public static string NormalizeWhiteSpaces(this string src)
    {
        return src?.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Join(" ");
    }
    #endregion // String_Manipulations
    #endregion // Extension Methods

    #region Private Methods


    private static string ReplaceMaxOccurrences(
        this string src,
        string oldValue,
        string newValue,
        int nMaxOccurrences,
        StringComparison comparison)
    {
        ArgumentNullException.ThrowIfNull(src);
        ArgumentNullException.ThrowIfNull(oldValue);
        ArgumentNullException.ThrowIfNull(newValue);
        comparison.CheckIsDefinedValue();

        // Skip the loop entirely if oldValue and newValue are the same 
        if (string.Equals(oldValue, newValue, comparison))
        {
            return src;
        }

        // I am not sure how this will work in terms of cultural comparison,  
        // this is a hack to avoid the bug reported here 
        // https://stackoverflow.com/questions/244531/is-there-an-alternative-to-string-replace-that-is-case-insensitive/13847351#comment31063745_244933 
        if (oldValue.Length > src.Length)
        {
            return src;
        }

        StringBuilder sb = new();
        int previousIndex = 0;
        int nReplacementsDone = 0;

        for (int index;
            (nReplacementsDone < nMaxOccurrences) && (0 <= (index = src.IndexOf(oldValue, previousIndex, comparison)));)
        {
            sb.Append(src.AsSpan(previousIndex, index - previousIndex));
            sb.Append(newValue);
            previousIndex = index + oldValue.Length;
            nReplacementsDone++;
        }
        sb.Append(src.AsSpan(previousIndex));

        return sb.ToString();
    }
    #endregion // Private Methods
}

#pragma warning restore IDE0057 // Use range operator

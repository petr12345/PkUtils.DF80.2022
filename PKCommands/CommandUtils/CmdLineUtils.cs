// Ignore Spelling: Utils, Unenclose, Descape
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PK.PkUtils.Extensions;
using static System.FormattableString;

#pragma warning disable IDE0057 // Substring can be simplified
#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.Commands.CommandUtils;

/// <summary> Few command-line-related utilities. </summary>
[CLSCompliant(true)]
public static class CmdLineUtils
{
    #region Fields

    private static readonly char[] _newLineChars = Environment.NewLine.ToCharArray();
    private const char _chDoubleQuote = '"';
    private const string _strDoubleQuote = "\"";
    private const string _strSlashAndQuote = "\\\"";
    #endregion // Fields

    #region Public Methods

    #region Arguments_sequence_related

    /// <summary> Filter-out newlines from incoming command-line arguments. 
    ///           This handles the weird case a newline comes from Visual Studio project properties 
    ///           ( the sub-item 'Debug / Command Line Arguments')
    /// </summary>
    /// <param name="args"> The command-line arguments. Can't be null, but could be an empty sequence. </param>
    /// <returns> Filtered sequence no longer containing newlines. </returns>
    public static IEnumerable<string> FilterOutNewlines(this IEnumerable<string> args)
    {
        args.CheckNoNulls();

#if DEBUG
        if (args.Any(s => s.IndexOfAny(_newLineChars) >= 0))
        {
            Trace.WriteLine("input contains newlines"); // just a placeholder for breakpoint
        }
#endif // DEBUG

        var result = args.SelectMany(s => s.Split(_newLineChars, StringSplitOptions.RemoveEmptyEntries));

        result = result.DebugToList();
        Debug.Assert(result.All(s => !string.IsNullOrEmpty(s)),
            "With StringSplitOptions.RemoveEmptyEntries, only non-empty strings should remain.");

        return result;
    }

    /// <summary>
    /// An extension method that checks if the sequence is not null, and each value within it is not null.
    /// </summary>
    /// <param name="args"> The command-line arguments. Can't be null, and can't contain nulls, but could be an empty sequence. </param>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="args"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when one or more items in <paramref name="args"/> is null. </exception>
    public static void CheckNoNulls(this IEnumerable<string> args)
    {
        ArgumentNullException.ThrowIfNull(args);
        if (args.Any(s => s == null))
        {
            throw new ArgumentException(
                Invariant($"Sequence '{nameof(args)}' cannot contain null items"), nameof(args));
        }
    }

    /// <summary>
    /// An extension method that joins a sequence of arguments into a final command line.
    /// </summary>
    /// <param name="args"> The command-line arguments. Can't be null, but could be an empty sequence. </param>
    /// <returns> A final command-line string. </returns>
    /// <seealso cref="SplitFromCommandLine"/>
    public static string JoinToCommandLine(this IEnumerable<string> args)
    {
        args.CheckNoNulls();

        var modified_1st = args.Where(x => !string.IsNullOrWhiteSpace(x)).DebugToList();
        var modified_2nd = modified_1st.Select(s => s.PutInDoubleQuotesIfSpaceAndNotQuoted()).DebugToList();
        var result = modified_2nd.Join(" ");

        Debug.Assert(result == result.Trim());

        return result;
    }

    /// <summary> Split string containing command-line parameters. </summary>
    /// <param name="commandLine"> The string to act on. Can't be null. </param>
    /// <param name="options"> Specifies whether applicable Split method overloads include or omit empty substrings from the return value. </param>
    /// <returns> Resulting sequence of individual arguments. </returns>
    /// <seealso cref="JoinToCommandLine"/>
    public static IEnumerable<string> SplitFromCommandLine(
        this string commandLine,
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
    {
        ArgumentNullException.ThrowIfNull(commandLine);

        bool inQuotes = false;

        bool FnController(char c)
        {
            if (c == '\"')
            {
                inQuotes = !inQuotes;
            }
            return !inQuotes && c == ' ';
        }

        IEnumerable<string> result = commandLine.SplitEx(FnController).Select(arg => arg.Trim().UnencloseDoubleQuotes());

        result = result.DebugToList();
        if (options == StringSplitOptions.RemoveEmptyEntries)
        {
            result = result.Where(arg => !string.IsNullOrEmpty(arg));
            result = result.DebugToList();
        }

        return result;
    }

    /// <summary>
    /// Specific overload of Split, that takes a function to decide whether the specified
    /// character should split the string.
    /// </summary>
    /// <param name="str"> The input string to act on. Can't be null. </param>
    /// <param name="controller"> The controller function callback. Can't be null. </param>
    /// <returns> Resulting sequence of individual substrings. </returns>
    public static IEnumerable<string> SplitEx(this string str, Func<char, bool> controller)
    {
        ArgumentNullException.ThrowIfNull(str);
        ArgumentNullException.ThrowIfNull(controller);

        int nextPiece = 0;

        for (int ii = 0; ii < str.Length; ii++)
        {
            if (controller(str[ii]))
            {
                yield return str.Substring(nextPiece, ii - nextPiece);
                nextPiece = ii + 1;
            }
        }

        yield return str.Substring(nextPiece);
    }

    /// <summary>
    /// An extension method that converts the sequence to a list in debug mode,
    /// and does nothing in non-debug mode.
    /// </summary>
    /// <param name="source"> The input sequence. Can't be null. </param>
    public static IEnumerable<string> DebugToList(this IEnumerable<string> source)
    {
#if DEBUG
        ArgumentNullException.ThrowIfNull(source);
        return source.ToList();
#else
        return source;
#endif // DEBUG
    }
    #endregion // Arguments_sequence_related

    #region Argument_value_related

    /// <summary> Encode <paramref name="argValues"/> to a "single safe" argument value. </summary>
    /// <remarks>
    /// The original string <paramref name="argValues"/> is the complete command-line of the executed program.
    /// This method packs it into a single string that can be handed over to another program as a single named argument value.
    /// Reverting method, executed in that other program, will get the original string using <see cref="DecodeJoinedArgumentsFromSingleArgValue"/>.
    /// </remarks>
    /// <param name="argValues"> The input argument value. Can't be null, but could be an empty string. </param>
    /// <returns> A string. </returns>
    /// <seealso cref="DecodeJoinedArgumentsFromSingleArgValue"/>
    public static string EncodeJoinedArgumentsToSingleArgValue(this string argValues)
    {
        ArgumentNullException.ThrowIfNull(argValues);

        string result = argValues;

        if (!string.IsNullOrEmpty(result))
        {
            result = result.EscapeDoubleQuotes();
            result = result.PutInDoubleQuotesIfSpaceAndNotQuoted();
        }

        return result;
    }

    /// <summary>
    /// Decodes the string value created previously by <see cref="EncodeJoinedArgumentsToSingleArgValue"/>
    /// back to the original.
    /// </summary>
    /// <param name="cmdLineArgValue"> The string to be converted. Originally created by <see cref="EncodeJoinedArgumentsToSingleArgValue"/>. </param>
    /// <returns> A string. </returns>
    /// <seealso cref="EncodeJoinedArgumentsToSingleArgValue"/>
    public static string DecodeJoinedArgumentsFromSingleArgValue(string cmdLineArgValue)
    {
        ArgumentNullException.ThrowIfNull(cmdLineArgValue);

        string result = cmdLineArgValue;

        if (!string.IsNullOrEmpty(result))
        {
            result = UnencloseDoubleQuotes(cmdLineArgValue);
            result = result.DescapeDoubleQuotes();
        }

        return result;
    }
    #endregion // Argument_value_related

    #region Simple_string_oriented_getters

    /// <summary> A string extension method that queries if <paramref name="argValue"/> contains a space character. </summary>
    /// <param name="argValue"> The string to act on. May be null or empty. </param>
    /// <returns> True if contains space character, false if not. </returns>
    public static bool ContainsSpace(this string argValue)
    {
        return !string.IsNullOrEmpty(argValue) && argValue.Contains(' ');
    }

    /// <summary> A string extension method that queries if <paramref name="argValue"/> 
    /// begins and ends with double-quote character. </summary>
    /// <param name="argValue"> The string to act on. May be null or empty. </param>
    /// <returns> True if contains space character, false if not. </returns>
    public static bool IsInDoubleQuotes(this string argValue)
    {
        return argValue.IsEnclosedBy(_chDoubleQuote);
    }
    #endregion // Simple_string_oriented_getters

    #region Simple_string_oriented_modifiers

    /// <summary> 'Encodes' every double-quote character in <paramref name="argValue"/> as two characters \". </summary>
    /// <param name="argValue"> The string to act on. May be null or empty. </param>
    /// <returns>   A modified string. </returns>
    public static string EscapeDoubleQuotes(this string argValue)
    {
        string result = argValue;

        if (!string.IsNullOrEmpty(argValue))
        {
            result = argValue.Replace(_strDoubleQuote, _strSlashAndQuote, StringComparison.Ordinal);
        }

        return result;
    }

    /// <summary> A reverse operation to <see cref="EscapeDoubleQuotes(string)"/>. 
    ///           Replaces sequence of two characters \"  by just double quote character.
    /// </summary>
    /// <param name="argValue"> The string to act on. May be null or empty. </param>
    /// <returns>   A modified string. </returns>
    public static string DescapeDoubleQuotes(this string argValue)
    {
        string result = argValue;

        if (!string.IsNullOrEmpty(argValue))
        {
            result = argValue.Replace(_strSlashAndQuote, _strDoubleQuote, StringComparison.Ordinal);
        }

        return result;
    }

    /// <summary> Puts double-quotes around the string, if there are not any if the text contains space. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="text"/> is null. </exception>
    /// <param name="text"> Text to be quoted. Can't be null. </param>
    /// <returns> Quoted text. </returns>
    public static string PutInDoubleQuotesIfSpaceAndNotQuoted(this string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        string result = text;

        if (text.ContainsSpace())
        {
            result = EncloseInDoubleQuotesIfNotEnclosed(result);
        }

        return result;
    }

    /// <summary> Puts double-quotes around the string, if there are not any. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="text"/> is null. </exception>
    /// <param name="text"> Text to be quoted. Can't be null. </param>
    /// <returns> Quoted text. </returns>
    public static string EncloseInDoubleQuotesIfNotEnclosed(this string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return text.EncloseIfNotEnclosed(_chDoubleQuote);
    }

    /// <summary> Removes double-quotes around the string, if there are any. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="text"/> is null. </exception>
    /// <param name="text"> Text to be unquoted. Can't be null. </param>
    /// <returns> Unquoted text. </returns>
    public static string UnencloseDoubleQuotes(this string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        string result = text.UnencloseIfEnclosed(_chDoubleQuote);
        return result;
    }
    #endregion // Simple_string_oriented_modifiers
    #endregion // Public Methods

    #region Private Methods

    private static bool IsEnclosedBy(this string argValue, char ch)
    {
        int length;
        bool result = false;

        if ((argValue != null) && ((length = argValue.Length) >= 2))
        {
            result = ((argValue[0] == ch) && (argValue[length - 1] == ch));
        }

        return result;
    }

    private static string EncloseIfNotEnclosed(this string text, char ch)
    {
        ArgumentNullException.ThrowIfNull(text);
        string result = text;

        if (!text.IsEnclosedBy(ch))
        {
            result = ch + text + ch;
        }

        return result;
    }

    private static string UnencloseIfEnclosed(this string text, char ch)
    {
        ArgumentNullException.ThrowIfNull(text);

        string result = text.IsEnclosedBy(ch) ? text.Substring(1, text.Length - 2) : text;
        return result;
    }

    #endregion // Private Methods
}
#pragma warning restore IDE0305
#pragma warning restore IDE0057
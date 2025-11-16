// Ignore Spelling: Utils
//
using System;
using System.Linq;
using PK.PkUtils.NativeLanguage;
using static System.Globalization.CultureInfo;

#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.PkUtils.Extensions;

/// <summary> Static class implementing a TimeSpan extension methods. </summary>
public static class TimeSpanExtensions
{
    /// <summary> A TimeSpan extension method that converts a span to a  readable string. </summary>
    ///
    /// <param name="span"> The span to act on. </param>
    /// <param name="includeMilliseconds">  (Optional) True to include, false to exclude the milliseconds. </param>
    ///
    /// <returns>   Span as a human-readable string. </returns>
    public static string ToReadableString(this TimeSpan span, bool includeMilliseconds = false)
    {
        TimeSpan d = span.Duration();
        string[] partsRaw = [
            d.Days > 0 ? string.Format(InvariantCulture, "{0:0} day{1}", span.Days, d.Days.Plural()) : string.Empty,
            d.Hours > 0 ? string.Format(InvariantCulture, "{0:0} hour{1}", span.Hours, d.Hours.Plural()) : string.Empty,
            d.Minutes >  0 ?  string.Format(InvariantCulture,"{0:0} minute{1}", span.Minutes, d.Minutes.Plural()) : string.Empty,
            d.Seconds >  0 ?  string.Format(InvariantCulture, "{0:0} second{1}", span.Seconds, d.Seconds.Plural()) : string.Empty,
            includeMilliseconds && (d.Milliseconds >  0)
                ?  string.Format(InvariantCulture, "{0:0} millisecond{1}", span.Milliseconds, d.Milliseconds.Plural())
                : string.Empty
        ];
        string[] partsFinal = partsRaw.Where(s => !string.IsNullOrEmpty(s)).ToArray();

        string result = (partsFinal.Length > 0) ? partsFinal.Join(",") : "0 seconds";
        return result;
    }
}
#pragma warning restore IDE0305
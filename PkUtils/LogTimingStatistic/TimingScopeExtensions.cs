// Ignore Spelling: Utils
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;


namespace PK.PkUtils.LogTimingStatistic;

/// <summary> Implements extensions of <see cref="ITimingScope"/>. </summary>
public static class TimingScopeExtensions
{
    #region Typedefs

    /// <summary> Values that represent the way of topics sorting. </summary>
    public enum TopicsSorting
    {
        /// <summary> An enum constant representing none sorting. </summary>
        None,
        /// <summary> An enum constant representing sorting by total calls. </summary>
        ByTotalCalls,
        /// <summary> An enum constant representing sorting by total time. </summary>
        ByTotalTime,
        /// <summary> An enum constant representing sorting by average time. </summary>
        ByAverageTime,
    }
    #endregion // Typedefs

    #region Fields

    private const int _headerLength = 64;
    private const char _slash = '/';
    private const char _lineChar = '-';
    #endregion // Fields

    #region Public Methods
    /// <summary> An ITimingScope extension method that generates the timings report for log. </summary>
    /// <param name="scope"> The scope to act on. </param>
    /// <param name="includeTotalMilliseconds"> True to include, false to exclude the total milliseconds. </param>
    /// <param name="sorting"> (Optional) The topics sorting method. </param>
    /// <returns>   The timings report. </returns>
    public static IReadOnlyList<string> GenerateTimingsReport(
        this ITimingScope scope,
        bool includeTotalMilliseconds,
        TopicsSorting sorting = TopicsSorting.ByTotalTime)
    {
        ArgumentNullException.ThrowIfNull(scope);

        IReadOnlyList<ITimingTopic> topics = scope.GetAllTopics();
        List<string> lines = [LogHeader()];

        if (topics.IsEmpty())
        {
            lines.Add("No timing measuring topic present");
        }
        else
        {
            int maxCallDigits = topics.Select(t => t.TotalCallsCount).Max().ToString(CultureInfo.InvariantCulture).Length;
            topics = SortTopics(topics, sorting);
            lines.AddRange(topics.Select(t => LogTopicInfo(t, maxCallDigits, includeTotalMilliseconds)));
        }
        lines.Add(LogFooter());

        return lines;
    }

    /// <summary> An ITimingScope extension method that logs the timings. </summary>
    /// <param name="scope"> The scope to act on. Can't be null. </param>
    /// <param name="logger"> The logger. Can't be null. </param>
    /// <param name="includeTotalMilliseconds"> True to include, false to exclude the total milliseconds. </param>
    /// <param name="sorting"> (Optional) The topics sorting method. </param>
    public static void LogTimings(
        this ITimingScope scope,
        IDumper logger,
        bool includeTotalMilliseconds,
        TopicsSorting sorting = TopicsSorting.ByTotalTime)
    {
        ArgumentNullException.ThrowIfNull(scope);
        ArgumentNullException.ThrowIfNull(logger);

        IReadOnlyList<string> lines = GenerateTimingsReport(scope, includeTotalMilliseconds, sorting);
        logger.Dump(Environment.NewLine + lines.Join(Environment.NewLine));
    }
    #endregion // Public Methods

    #region Private Methods

#pragma warning disable IDE0305 // Collection initialization can be simplified

    private static IReadOnlyList<ITimingTopic> SortTopics(
        IReadOnlyList<ITimingTopic> topics,
        TopicsSorting sorting)
    {
        IReadOnlyList<ITimingTopic> result = topics;

        if ((sorting != TopicsSorting.None) && (topics.Count > 0))
        {
            switch (sorting)
            {
                case TopicsSorting.ByTotalCalls:
                    result = topics.OrderByDescending(t => t.TotalCallsCount).ToList();
                    break;

                case TopicsSorting.ByTotalTime:
                    result = topics.OrderByDescending(t => t.TotalCallsTime).ToList();
                    break;

                case TopicsSorting.ByAverageTime:
                    result = topics.OrderByDescending(t => t.GetAverageTime()).ToList();
                    break;
            }
        }

        return result;
    }
#pragma warning restore IDE0305

    private static string LogHeader()
    {
        string line = _slash + new string(_lineChar, 3) + "Timing Statistics ";
        line += new string(_lineChar, Math.Max(_headerLength - line.Length, 0));

        return line;
    }

    private static string LogFooter()
    {
        return new string(_lineChar, _headerLength) + _slash;
    }

    private static string LogTopicInfo(
        ITimingTopic topic,
        int maxCallDigits,
        bool includeTotalMilliseconds)
    {
        string sAverageTime = string.Empty;
        string sTotalTime = $"total time = {topic.TotalCallsTime.ToReadableString(includeMilliseconds: includeTotalMilliseconds)}";
        string sNumCalls = $"{topic.TotalCallsCount}";
        string sTotalCalls = $"Calls = {sNumCalls.PadLeft(maxCallDigits)}";

        if (topic.TotalCallsCount > 0)
        {
            TimeSpan average = topic.GetAverageTime();
            sAverageTime = "avg = " + average.ToString("hh\\:mm\\:ss\\.fff", CultureInfo.InvariantCulture);
        }
        string line = $"{_slash} {topic.TopicName}: {sTotalCalls}; {sAverageTime}; {sTotalTime}";
        return line;
    }

    #endregion // Private Methods
}

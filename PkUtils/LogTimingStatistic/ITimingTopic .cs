// Ignore Spelling: Utils
//
using System;

namespace PK.PkUtils.LogTimingStatistic;

/// <summary> Defines behaviour of specific topic (specific calls counter). </summary>
public interface ITimingTopic
{
    /// <summary> Gets the name of the topic; for instance a specific method being called. </summary>
    string TopicName { get; }

    /// <summary> Gets the number of totals occurrences of topic. </summary>
    int TotalCallsCount { get; }

    /// <summary> Gets the total timing of all calls. </summary>
    TimeSpan TotalCallsTime { get; }

    /// <summary> Adds an occurrence of the topic to the whole statistic. </summary>
    /// <param name="callSpan"> The call span. </param>
    /// <returns>   A TimingTopic. </returns>
    ITimingTopic AddOccurrence(TimeSpan callSpan);
}

/// <summary> A timing topic extensions methods. </summary>
public static class TimingTopicExtensions
{
    /// <summary> An ITimingTopic extension method that gets average time. </summary>
    /// <param name="topic"> The topic to act on. Can't be null. </param>
    /// <returns>   The average time of call. </returns>
    public static TimeSpan GetAverageTime(this ITimingTopic topic)
    {
        ArgumentNullException.ThrowIfNull(topic);
        return (topic.TotalCallsCount > 0)
            ? new TimeSpan(topic.TotalCallsTime.Ticks / topic.TotalCallsCount)
            : TimeSpan.Zero;
    }
}

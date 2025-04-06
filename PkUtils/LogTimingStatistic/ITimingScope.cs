// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;

namespace PK.PkUtils.LogTimingStatistic;

/// <summary> Defines a timing scope, that contains several timing topics. </summary>
public interface ITimingScope
{
    /// <summary> Attempts to get a topic, returning either already existing topic or null. </summary>
    ITimingTopic TryGetTopic(string topicName);

    /// <summary> Adds one timing operation to a topic with matching names, returns that topic.
    ///            Creates a new topic, if a topic for <paramref name="topicName"/> does not exist. 
    /// </summary>
    ITimingTopic AddOccurrence(string topicName, TimeSpan timing);

    /// <summary> Gets all topics created so far. </summary>
    IReadOnlyList<ITimingTopic> GetAllTopics();

    /// <summary> Removes all topics. </summary>
    void Clear();
}

// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.LogTimingStatistic;

/// <summary> A simple implementation of <see cref="ITimingScope"/>. </summary>
public class TimingScope : ITimingScope
{
    private readonly Dictionary<string, ITimingTopic> _topics = [];
    private readonly object _locker = new();

    /// <summary> Default argument-less constructor. </summary>
    public TimingScope()
    { }

    #region  ITimingScope Members

    /// <inheritdoc/>
    public ITimingTopic TryGetTopic(string topicName)
    {
        topicName.CheckArgNotNullOrEmpty(nameof(topicName));
        lock (_locker) { return _topics.ValueOrDefault(topicName); }
    }

    /// <inheritdoc/>
    public ITimingTopic AddOccurrence(string topicName, TimeSpan callSpan)
    {
        topicName.CheckArgNotNullOrEmpty(nameof(topicName));

        lock (_locker)
        {
            if (!_topics.TryGetValue(topicName, out ITimingTopic result))
            {
                _topics.Add(topicName, result = new TimingTopic(topicName));
            }

            return result.AddOccurrence(callSpan);
        }
    }

    /// <inheritdoc/>
    public IReadOnlyList<ITimingTopic> GetAllTopics()
    {
        lock (_locker) { return [.. _topics.Values]; }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        lock (_locker) { _topics.Clear(); }
    }
    #endregion  // ITimingScope Members
}

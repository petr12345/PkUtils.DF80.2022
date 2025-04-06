// Ignore Spelling: Utils
//

using System;
using PK.PkUtils.Extensions;
#pragma warning disable IDE0290     // Use primary constructor

namespace PK.PkUtils.LogTimingStatistic;


internal class TimingTopic : ITimingTopic
{
    private readonly string _topicName;
    private int _totalCallsCount;
    private TimeSpan _totalSpan = TimeSpan.Zero;

    public TimingTopic(string topicName)
    {
        _topicName = topicName.CheckArgNotNullOrEmpty(nameof(topicName));
    }

    #region ITimingTopic Members

    /// <inheritdoc/>
    public string TopicName { get => _topicName; }

    /// <inheritdoc/>
    public int TotalCallsCount { get => _totalCallsCount; }

    /// <inheritdoc/>
    public TimeSpan TotalCallsTime { get => _totalSpan; }

    /// <inheritdoc/>
    public ITimingTopic AddOccurrence(TimeSpan callSpan)
    {
        _totalSpan += callSpan;
        _totalCallsCount++;

        return this;
    }
    #endregion // ITimingTopic Members

    public override string ToString() { return this.PropertyList(); }
}

#pragma warning restore IDE0290     // Use primary constructor
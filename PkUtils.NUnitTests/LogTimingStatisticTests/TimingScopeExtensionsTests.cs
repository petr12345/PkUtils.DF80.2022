// Ignore Spelling: PkUtils, Utils
// 
using PK.PkUtils.Dump;
using PK.PkUtils.Interfaces;
using PK.PkUtils.LogTimingStatistic;

namespace PK.PkUtils.NUnitTests.LogTimingStatisticTests;

[TestFixture()]
public class TimingScopeExtensionsTests
{
    [Test()]
    public void LogTimingsTest()
    {
        // Arrange
        ITimingScope scope = new TimingScope();
        IDumper dumper = new DumperStringWrapper();

        // Act
        for (int ii = 0; ii < 10;)
        {
            ii++;
            scope.AddOccurrence($"TopicName{ii}", TimeSpan.FromSeconds(ii));
        }

        // Act
        scope.LogTimings(dumper, false, TimingScopeExtensions.TopicsSorting.ByTotalTime);

        // Assert
        Assert.That(dumper.ToString(), Is.Not.Null);
    }
}
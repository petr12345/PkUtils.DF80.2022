// Ignore Spelling: PkUtils, Utils
// 
using PK.PkUtils.Dump;
using PK.PkUtils.Interfaces;
using PK.PkUtils.LogTimingStatistic;

namespace PK.PkUtils.NUnitTests.LogTimingStatisticTests;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable CA1859    // Change type of variable ...

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
#pragma warning restore CA1859    // Change type of variable ...
#pragma warning restore IDE0079   // Remove unnecessary suppressions
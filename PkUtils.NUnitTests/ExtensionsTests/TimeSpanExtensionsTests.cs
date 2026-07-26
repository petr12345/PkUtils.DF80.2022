using PK.PkUtils.Extensions;

namespace PK.PkUtils.NUnitTests.ExtensionsTests;


[TestFixture]
public class TimeSpanExtensionsTests
{
    public static IEnumerable<TestCaseData> ToReadableString_TestCases()
    {
        // --- zero ---
        yield return new TestCaseData(TimeSpan.Zero, false, "0 seconds")
            .SetName("Zero_NoMilliseconds");

        // --- seconds ---
        yield return new TestCaseData(TimeSpan.FromSeconds(1), false, "1 second")
            .SetName("SingleSecond");

        yield return new TestCaseData(TimeSpan.FromSeconds(2), false, "2 seconds")
            .SetName("PluralSeconds");

        // --- minutes ---
        yield return new TestCaseData(TimeSpan.FromMinutes(1), false, "1 minute")
            .SetName("SingleMinute");

        yield return new TestCaseData(TimeSpan.FromMinutes(2), false, "2 minutes")
            .SetName("PluralMinutes");

        // --- hours ---
        yield return new TestCaseData(TimeSpan.FromHours(1), false, "1 hour")
            .SetName("SingleHour");

        yield return new TestCaseData(TimeSpan.FromHours(5), false, "5 hours")
            .SetName("PluralHours");

        // --- days ---
        yield return new TestCaseData(TimeSpan.FromDays(1), false, "1 day")
            .SetName("SingleDay");

        yield return new TestCaseData(TimeSpan.FromDays(3), false, "3 days")
            .SetName("PluralDays");

        // --- mixed components ---
        yield return new TestCaseData(
            new TimeSpan(1, 2, 3, 4),
            false,
            "1 day, 2 hours, 3 minutes, 4 seconds")
            .SetName("Mixed_NoMilliseconds");

        // --- milliseconds excluded ---
        yield return new TestCaseData(
            new TimeSpan(0, 0, 0, 1, 123),
            false,
            "1 second")
            .SetName("Milliseconds_Excluded");

        // --- milliseconds included ---
        yield return new TestCaseData(
            new TimeSpan(0, 0, 0, 1, 123),
            true,
            "1 second, 123 milliseconds")
            .SetName("Milliseconds_Included");

        // --- milliseconds only ---
        yield return new TestCaseData(
            new TimeSpan(0, 0, 0, 0, 5),
            true,
            "5 milliseconds")
            .SetName("OnlyMilliseconds_Included");

        yield return new TestCaseData(
            new TimeSpan(0, 0, 0, 0, 5),
            false,
            "0 seconds")
            .SetName("OnlyMilliseconds_Excluded");

        // --- negative timespan (Duration behavior) ---
        yield return new TestCaseData(
            TimeSpan.FromSeconds(-5),
            false,
            "-5 seconds")
            .SetName("NegativeSpan");

        // --- mixed + milliseconds ---
        yield return new TestCaseData(
            new TimeSpan(0, 1, 2, 3, 4),
            true,
            "1 hour, 2 minutes, 3 seconds, 4 milliseconds")
            .SetName("Mixed_WithMilliseconds");
    }

    #region Tests

    [TestCaseSource(nameof(ToReadableString_TestCases))]
    public void ToReadableString_ShouldReturnExpected(
        TimeSpan input,
        bool includeMilliseconds,
        string expected)
    {
        // Act
        string result = input.ToReadableString(includeMilliseconds);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
    #endregion // Tests
}

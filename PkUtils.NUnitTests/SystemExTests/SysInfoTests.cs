// Ignore Spelling: Utils, Ver
//
using PK.PkUtils.SystemEx;

namespace PK.PkUtils.NUnitTests.SystemExTests;

/// <summary>  This is a test class for <see cref="SysInfo"/> </summary>
[TestFixture()]
public class SysInfoTests
{
    /// <summary>
    /// Test that ensures GetVer() does not throw any exceptions and returns a valid WinVer value.
    /// </summary>
    [Test, Description("Test to verify that GetVer does not fail and returns a valid value.")]
    public void GetVer_ReturnsValidResult()
    {
        // Act
        WinVer result = SysInfo.GetWindowsVersion();

        // Assert
        Assert.That(result, Is.Not.EqualTo(WinVer.Unknown), "The returned value should not be WinVer.Unknown");
    }
}

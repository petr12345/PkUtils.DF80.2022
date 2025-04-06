// Ignore Spelling: PkUtils, Utils
// 
using PK.PkUtils.Extensions;

namespace PK.PkUtils.NUnitTests.ExtensionsTests;

/// <summary> Unit Tests of class <see cref="ExceptionExtension"/>. </summary>
[CLSCompliant(false)]
[TestFixture]
public class ExceptionExtensionTest
{
    #region Tests
    #region AllInnerExceptions Tests

    [Test, Description("Tests retrieving all inner exceptions including the main exception.")]
    public void AllInnerExceptions_IncludesMainException()
    {
        // Arrange
        Exception innerMost = new Exception("Inner most");
        Exception middle = new Exception("Middle", innerMost);
        Exception outer = new Exception("Outer", middle);

        // Act
        var result = outer.AllInnerExceptions(includeThisOne: true).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EquivalentTo(new[] { outer, middle, innerMost }));
        });
    }

    [Test, Description("Tests retrieving only inner exceptions without the main exception.")]
    public void AllInnerExceptions_ExcludesMainException()
    {
        // Arrange
        Exception innerMost = new Exception("Inner most");
        Exception middle = new Exception("Middle", innerMost);
        Exception outer = new Exception("Outer", middle);

        // Act
        var result = outer.AllInnerExceptions(false).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EquivalentTo(new[] { middle, innerMost }));
        });
    }

    [Test, Description("Tests retrieving inner exceptions from a null exception.")]
    public void AllInnerExceptions_NullException_ReturnsEmpty()
    {
        // Arrange
        Exception nullEx = null!;

        // Act
        var result = nullEx.AllInnerExceptions();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Empty);
        });
    }
    #endregion // AllInnerExceptions Tests

    #region MostInnerException Tests

    [Test, Description("Tests retrieving the most inner exception.")]
    public void MostInnerException_ReturnsInnermost()
    {
        // Arrange
        Exception innerMost = new Exception("Inner most");
        Exception middle = new Exception("Middle", innerMost);
        Exception outer = new Exception("Outer", middle);

        // Act
        var result = outer.MostInnerException();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.SameAs(innerMost));
        });
    }

    [Test, Description("Tests retrieving the most inner exception when no inner exception exists.")]
    public void MostInnerException_NoInnerException_ReturnsSelf()
    {
        // Arrange
        Exception ex = new Exception("Standalone");

        // Act
        var result = ex.MostInnerException();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.SameAs(ex));
        });
    }

    [Test, Description("Tests retrieving the most inner exception from a null exception.")]
    public void MostInnerException_NullException_ReturnsNull()
    {
        // Arrange
        Exception nullEx = null!;

        // Act
        var result = nullEx.MostInnerException();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Null);
        });
    }
    #endregion // MostInnerException Tests

    #region ExceptionDetails Tests

    [Test, Description("Tests retrieving exception details including stack trace.")]
    public void ExceptionDetails_IncludesStackTrace()
    {
        // Arrange
        Exception ex = new Exception("Test message");

        // Act
        var result = ex.ExceptionDetails(true);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Does.Contain("Exception Type"));
            Assert.That(result, Does.Contain("Exception Message: Test message"));
            Assert.That(result, Does.Contain("StackTrace"));
        });
    }

    [Test, Description("Tests retrieving exception details without stack trace.")]
    public void ExceptionDetails_ExcludesStackTrace()
    {
        // Arrange
        Exception ex = new Exception("Test message");

        // Act
        var result = ex.ExceptionDetails(false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Does.Contain("Exception Type"));
            Assert.That(result, Does.Contain("Exception Message: Test message"));
            Assert.That(result, Does.Not.Contain("StackTrace"));
        });
    }

    [Test, Description("Tests retrieving exception details for a null exception.")]
    public void ExceptionDetails_NullException_ReturnsEmpty()
    {
        // Arrange
        Exception nullEx = null!;

        // Act
        var result = nullEx.ExceptionDetails(true);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Empty);
        });
    }
    #endregion // ExceptionDetails Tests
    #endregion // Tests
}

// Ignore Spelling: Utils
// 
using PK.PkUtils.Extensions;

namespace PK.PkUtils.NUnitTests.ExtensionsTests;

/// <summary> Unit Tests of class <see cref="ExceptionExtensions"/>. </summary>
[TestFixture]
public class ExceptionExtensionsTest
{
    #region Tests
    #region AllInnerExceptions Tests

    [Test, Description("Tests retrieving all inner exceptions including the main exception.")]
    public void AllInnerExceptions_IncludesMainException()
    {
        // Arrange
        Exception innerMost = new("Inner most");
        Exception middle = new("Middle", innerMost);
        Exception outer = new("Outer", middle);

        // Act
        List<Exception> result = [.. outer.AllInnerExceptions(includeThisOne: true)];

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EquivalentTo([outer, middle, innerMost]));
        }
    }

    [Test, Description("Tests retrieving only inner exceptions without the main exception.")]
    public void AllInnerExceptions_ExcludesMainException()
    {
        // Arrange
        Exception innerMost = new("Inner most");
        Exception middle = new("Middle", innerMost);
        Exception outer = new("Outer", middle);

        // Act
        List<Exception> result = [.. outer.AllInnerExceptions(false)];

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EquivalentTo([middle, innerMost]));
        }
    }

    [Test, Description("Tests retrieving inner exceptions from a null exception.")]
    public void AllInnerExceptions_NullException_ReturnsEmpty()
    {
        // Arrange
        Exception nullEx = null!;

        // Act
        IEnumerable<Exception> result = nullEx.AllInnerExceptions();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Empty);
        }
    }
    #endregion // AllInnerExceptions Tests

    #region MostInnerException Tests

    [Test, Description("Tests retrieving the most inner exception.")]
    public void MostInnerException_ReturnsInnermost()
    {
        // Arrange
        Exception innerMost = new("Inner most");
        Exception middle = new("Middle", innerMost);
        Exception outer = new("Outer", middle);

        // Act
        Exception result = outer.MostInnerException();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.SameAs(innerMost));
        }
    }

    [Test, Description("Tests retrieving the most inner exception when no inner exception exists.")]
    public void MostInnerException_NoInnerException_ReturnsSelf()
    {
        // Arrange
        Exception ex = new("Standalone");

        // Act
        Exception result = ex.MostInnerException();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.SameAs(ex));
        }
    }

    [Test, Description("Tests retrieving the most inner exception from a null exception.")]
    public void MostInnerException_NullException_ReturnsNull()
    {
        // Arrange
        Exception nullEx = null!;

        // Act
        Exception result = nullEx.MostInnerException();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Null);
        }
    }
    #endregion // MostInnerException Tests

    #region ExceptionDetails Tests

    [Test, Description("Tests retrieving exception details including stack trace.")]
    public void ExceptionDetails_IncludesStackTrace()
    {
        // Arrange
        Exception ex = new("Test message");

        // Act
        string result = ex.ExceptionDetails(true);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Does.Contain("Exception Type"));
            Assert.That(result, Does.Contain("Exception Message: Test message"));
            Assert.That(result, Does.Contain("StackTrace"));
        }
    }

    [Test, Description("Tests retrieving exception details without stack trace.")]
    public void ExceptionDetails_ExcludesStackTrace()
    {
        // Arrange
        Exception ex = new("Test message");

        // Act
        string result = ex.ExceptionDetails(false);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Does.Contain("Exception Type"));
            Assert.That(result, Does.Contain("Exception Message: Test message"));
            Assert.That(result, Does.Not.Contain("StackTrace"));
        }
    }

    [Test, Description("Tests retrieving exception details for a null exception.")]
    public void ExceptionDetails_NullException_ReturnsEmpty()
    {
        // Arrange
        Exception nullEx = null!;

        // Act
        string result = nullEx.ExceptionDetails(true);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Empty);
        }
    }
    #endregion // ExceptionDetails Tests
    #endregion // Tests
}

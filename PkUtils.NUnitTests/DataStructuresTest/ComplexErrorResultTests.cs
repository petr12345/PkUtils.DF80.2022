using PK.PkUtils.DataStructures;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.NUnitTests.DataStructuresTest;

/// <summary>
/// Unit Test of class ComplexErrorResult{TError}.
/// </summary>
[TestFixture]
internal class ComplexErrorResultTests
{
    [Test]
    [Description("CreateSuccessful returns a successful result.")]
    public void CreateSuccessful_ReturnsSuccess()
    {
        // Arrange & Act
        IComplexErrorResult<string> result = ComplexErrorResult<string>.CreateSuccessful();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
            Assert.That(result.ErrorDetails, Is.Null);
        }
    }

    [Test]
    [Description("CreateFailed returns a failed result with error message and details.")]
    public void CreateFailed_ReturnsFailedWithMessageAndDetails()
    {
        // Arrange & Act
        IComplexErrorResult<string> result = ComplexErrorResult<string>.CreateFailed("Error occurred", "Details");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Error occurred"));
            Assert.That(result.ErrorDetails, Is.EqualTo("Details"));
        }
    }

    [Test]
    [Description("CreateFailed(Exception) returns a failed result with exception message.")]
    public void CreateFailed_WithException_ReturnsFailedWithExceptionMessage()
    {
        // Arrange
        InvalidOperationException ex = new InvalidOperationException("Invalid op");

        // Act
        IComplexErrorResult<Exception> result = ComplexErrorResult<Exception>.CreateFailed(ex);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid op"));
            Assert.That(result.ErrorDetails, Is.EqualTo(ex));
        }
    }

    [Test]
    [Description("ToString returns correct string for success and failure.")]
    public void ToString_ReturnsExpectedString()
    {
        // Arrange & Act
        IComplexErrorResult<string> success = ComplexErrorResult<string>.CreateSuccessful();
        IComplexErrorResult<string> failed = ComplexErrorResult<string>.CreateFailed("Fail", "Details");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(success.ToString(), Is.EqualTo("Success"));
            Assert.That(failed.ToString(), Is.EqualTo("Error: Fail, Details: Details"));
        }
    }

    [Test]
    [Description("CreateFailed(IComplexErrorResult) throws if input is not failed.")]
    public void CreateFailed_FromSuccessResult_Throws()
    {
        // Arrange
        IComplexErrorResult<string> success = ComplexErrorResult<string>.CreateSuccessful();

        // Act & Assert
        Assert.That(() => ComplexErrorResult<string>.CreateFailed(success), Throws.ArgumentException);
    }

    [Test]
    [Description("CreateFailed(IComplexErrorResult) copies error message and details.")]
    public void CreateFailed_FromFailedResult_CopiesError()
    {
        // Arrange
        IComplexErrorResult<string> failed = ComplexErrorResult<string>.CreateFailed("Fail", "Details");

        // Act
        IComplexErrorResult<string> copy = ComplexErrorResult<string>.CreateFailed(failed);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(copy.Success, Is.False);
            Assert.That(copy.ErrorMessage, Is.EqualTo("Fail"));
            Assert.That(copy.ErrorDetails, Is.EqualTo("Details"));
        }
    }
}



/// <summary>
/// Unit Test of generic class ComplexErrorResult{T, TError}.
/// </summary>
[TestFixture]
internal class ComplexErrorResultGenericTests
{
    [Test]
    [Description("CreateSuccessful returns a successful result with content.")]
    public void CreateSuccessful_ReturnsSuccessWithContent()
    {
        // Arrange & Act
        IComplexErrorResult<int, string> result = ComplexErrorResult<int, string>.CreateSuccessful(42);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.Content, Is.EqualTo(42));
            Assert.That(result.ErrorMessage, Is.Null);
            Assert.That(result.ErrorDetails, Is.Null);
        }
    }

    [Test]
    [Description("CreateFailed returns a failed result with error message and details.")]
    public void CreateFailed_ReturnsFailedWithMessageAndDetails()
    {
        // Arrange & Act
        IComplexErrorResult<int, string> result = ComplexErrorResult<int, string>.CreateFailed("Error occurred", "Details");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Error occurred"));
            Assert.That(result.ErrorDetails, Is.EqualTo("Details"));
        }
    }

    [Test]
    [Description("CreateFailed(Exception) returns a failed result with exception message.")]
    public void CreateFailed_WithException_ReturnsFailedWithExceptionMessage()
    {
        // Arrange
        InvalidOperationException ex = new InvalidOperationException("Invalid op");

        // Act
        IComplexErrorResult<int, Exception> result = ComplexErrorResult<int, Exception>.CreateFailed(ex);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid op"));
            Assert.That(result.ErrorDetails, Is.EqualTo(ex));
        }
    }

    [Test]
    [Description("ToString returns correct string for success and failure.")]
    public void ToString_ReturnsExpectedString()
    {
        // Arrange & Act
        IComplexErrorResult<int, string> success = ComplexErrorResult<int, string>.CreateSuccessful(99);
        IComplexErrorResult<int, string> failed = ComplexErrorResult<int, string>.CreateFailed("Fail", "Details");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(success.ToString(), Is.EqualTo("Success: 99"));
            Assert.That(failed.ToString(), Is.EqualTo("Error: Fail, Details: Details"));
        }
    }

    [Test]
    [Description("Constructor from IComplexErrorResult<T, TError> copies content if successful.")]
    public void Constructor_FromOtherResult_CopiesContent()
    {
        // Arrange
        IComplexErrorResult<int, string> original = ComplexErrorResult<int, string>.CreateSuccessful(123);

        // Act
        ComplexErrorResult<int, string> copy = new ComplexErrorResult<int, string>(original);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(copy.Success, Is.True);
            Assert.That(copy.Content, Is.EqualTo(123));
        }
    }

    [Test]
    [Description("Constructor from IComplexErrorResult<TError> throws if not failed.")]
    public void Constructor_FromSuccessErrorResult_Throws()
    {
        // Arrange
        IComplexErrorResult<string> success = ComplexErrorResult<string>.CreateSuccessful();

        // Act & Assert
        Assert.That(() => new ComplexErrorResult<int, string>(success), Throws.ArgumentException);
    }

    [Test]
    [Description("Constructor from IComplexErrorResult<TError> copies error if failed.")]
    public void Constructor_FromFailedErrorResult_CopiesError()
    {
        // Arrange
        IComplexErrorResult<string> failed = ComplexErrorResult<string>.CreateFailed("Fail", "Details");

        // Act
        ComplexErrorResult<int, string> copy = new ComplexErrorResult<int, string>(failed);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(copy.Success, Is.False);
            Assert.That(copy.ErrorMessage, Is.EqualTo("Fail"));
            Assert.That(copy.ErrorDetails, Is.EqualTo("Details"));
        }
    }
}


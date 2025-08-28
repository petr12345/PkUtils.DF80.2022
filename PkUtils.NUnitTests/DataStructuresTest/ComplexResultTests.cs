// Ignore Spelling: Utils
//

using PK.PkUtils.DataStructures;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.NUnitTests.DataStructuresTest;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable CA1859    // Change type of variable ...

[TestFixture]
internal class ComplexResultTests
{
    #region Tests
    #region Constructor Tests

    [Test, Description("Tests default constructor initializes to success state")]
    public void Constructor_Default_SuccessTrue()
    {
        // Act
        var result = new ComplexResult();

        // Assert
        Assert.That(result.Success, Is.True);
    }

    [Test, Description("Tests constructor with error message initializes to failure state")]
    public void Constructor_ErrorMessage_SetsFailure()
    {
        // Act
        var result = new ComplexResult("Error occurred");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Error occurred"));
        }
    }

    [Test, Description("Tests constructor with exception initializes to failure state")]
    public void Constructor_Exception_SetsFailure()
    {
        // Arrange
        var ex = new InvalidOperationException("Invalid operation");

        // Act
        var result = new ComplexResult(ex);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorDetails, Is.EqualTo(ex));
        }
    }

    [Test, Description("Tests constructor copying from another IComplexResult")]
    public void Constructor_CopyFromOther_SetsCorrectState()
    {
        // Arrange
        var original = new ComplexResult("Original error");

        // Act
        var copy = new ComplexResult(original);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(copy.Success, Is.False);
            Assert.That(copy.ErrorMessage, Is.EqualTo("Original error"));
        }
    }

    #endregion // Constructor Tests

    #region Factory Methods Tests

    [Test, Description("Tests Create method with empty error message returns success result")]
    public void Create_EmptyMessage_ReturnsSuccess()
    {
        // Act
        var result = ComplexResult.Create("");

        // Assert
        Assert.That(result.Success, Is.True);
    }

    [Test, Description("Tests CreateFailed method returns failed result")]
    public void CreateFailed_ErrorMessage_ReturnsFailure()
    {
        // Arrange
        const string originalFailure = "Il mio hovercraft è pieno di anguille";

        // Act
        var result = ComplexResult.CreateFailed(originalFailure);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo(originalFailure));
        }
    }

    [Test, Description("Tests CreateFailed method with exception initializes correctly")]
    public void CreateFailed_Exception_ReturnsFailure()
    {
        // Arrange
        var ex = new ArgumentNullException("param");

        // Act
        var result = ComplexResult.CreateFailed(ex);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorDetails, Is.EqualTo(ex));
        }
    }

    [Test, Description("Tests CreateFailed method with another failed result copies failure")]
    public void CreateFailed_CopyFromFailedResult_ReturnsFailure()
    {
        // Arrange
        const string originalFailure = "Ilmatyynyalukseni on täynnä ankeriaita";
        var failedResult = ComplexResult.CreateFailed(originalFailure);

        // Act
        var newFailed = ComplexResult.CreateFailed(failedResult);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(newFailed.Success, Is.False);
            Assert.That(newFailed.ErrorMessage, Is.EqualTo(originalFailure));
        }
    }
    #endregion // Factory Methods Tests

    #region ToString Tests

    [Test, Description("Tests ToString returns 'Success' for successful result")]
    public void ToString_Success_ReturnsSuccess()
    {
        // Arrange
        var result = new ComplexResult();

        // Act
        var resultString = result.ToString();

        // Assert
        Assert.That(resultString, Is.EqualTo("Success"));
    }

    [Test, Description("Tests ToString returns error message for failed result")]
    public void ToString_Failure_ReturnsErrorMessage()
    {
        // Arrange
        IComplexResult result = ComplexResult.CreateFailed("An error occurred");

        // Act
        var resultString = result.ToString();

        // Assert
        Assert.That(resultString, Is.EqualTo("Error: An error occurred"));
    }

    [Test, Description("Tests ToString handles null error details correctly")]
    public void ToString_FailureWithNullDetails_ReturnsErrorMessageWithoutDetails()
    {
        // Arrange
        ComplexResult result = new("An error occurred", null);

        // Act
        var resultString = result.ToString();

        // Assert
        Assert.That(resultString, Is.EqualTo("Error: An error occurred"));
    }

    [TestCase("An error occurred", "Additional details", "Error: An error occurred, Details: Additional details")]
    [TestCase("Something went wrong", "Oops!", "Error: Something went wrong, Details: Oops!")]
    [TestCase("Failure", "", "Error: Failure")] // Empty details should not be included
    [TestCase("Critical failure", null!, "Error: Critical failure")] // Null details should be ignored
    [TestCase(" ", "Details only", "Error:  , Details: Details only")] // Space-only error
    [TestCase("Undefined error", "42", "Error: Undefined error, Details: 42")] // Hitchhiker's Guide reference
    [TestCase("💥 Boom!", "🔥 Fire on second floor!", "Error: 💥 Boom!, Details: 🔥 Fire on second floor!")] // Unicode characters
    [Test, Description("Tests ToString returns error message with details for failed result with details")]
    public void ToString_FailureWithDetails_ReturnsErrorMessageWithDetails(
        string errorMessage,
        string errorDetails,
        string expectedOutput)
    {
        // Arrange
        var result = new ComplexResult(errorMessage, errorDetails);

        // Act
        var resultString = result.ToString();

        // Assert
        Assert.That(resultString, Is.EqualTo(expectedOutput));
    }
    #endregion // ToString Tests

    #region Exception Wrapping Tests

    [Test, Description("Tests that ExceptionCaught returns null if error details is not an exception")]
    public void ExceptionCaught_NonException_ReturnsNull()
    {
        var result = new ComplexResult("Some error", "Not an exception");

        // Assert
        Assert.That(result.ExceptionCaught(), Is.Null);
    }

    [Test, Description("Tests that ExceptionCaught returns the correct exception if error details is an exception")]
    public void ExceptionCaught_Exception_ReturnsException()
    {
        // Arrange
        var ex = new InvalidOperationException("Something went wrong");

        // Act
        var result = new ComplexResult(ex);

        // Assert
        Assert.That(result.ExceptionCaught(), Is.EqualTo(ex));
    }

    #endregion // Exception Wrapping Tests
    #endregion // Tests
}
#pragma warning restore CA1859    // Change type of variable ...
#pragma warning restore IDE0079   // Remove unnecessary suppressions
// Ignore Spelling: Utils
//
using PK.PkUtils.DataStructures;
using PK.PkUtils.Interfaces;


namespace PK.PkUtils.NUnitTests.DataStructuresTest;

/// <summary> Unit Test of generic class ComplexResult{T}. </summary>
[TestFixture()]
internal class ComplexResultGenericTests
{
    #region Fields
    private const string _errorMessage = "#$%^ what the hell ?!";
    private const string _stringRegularValue = "kobylamamalybok";
    private static readonly NullReferenceException _exception = new("Don't worry, be happy");
    #endregion // Fields

    #region Tests

    #region Tests_constructors

    [Test, Description("Testing generic ComplexResult constructor for succeeded state.")]
    public void Result_Constructor_01()
    {
        // Arrange
        const int nValue = 1234567;

        // Act
        ComplexResult<int> iRes = new(nValue);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(iRes.Success, Is.True);
            Assert.That(iRes.Content, Is.EqualTo(nValue));
            Assert.That(iRes.ErrorMessage, Is.Null);
        }
    }

    [Test, Description("Testing generic ComplexResult constructor for failed state with an error message.")]
    public void Result_Constructor_02()
    {
        // Arrange

        // Act
        ComplexResult<int> iRes = new(_errorMessage);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(iRes.Success, Is.False);
            Assert.That(iRes.ErrorMessage, Is.EqualTo(_errorMessage));
        }
    }

    [Test, Description("Testing generic ComplexResult constructor for failed state with an error message and exception.")]
    public void Result_Constructor_03()
    {
        // Arrange

        // Act
        ComplexResult<int> iRes = new(_errorMessage, _exception);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(iRes.Success, Is.False);
            Assert.That(iRes.ErrorMessage, Is.EqualTo(_errorMessage));
        }
    }

    [Test, Description("Testing generic ComplexResult constructor for failed state with just exception.")]
    public void Result_Constructor_04()
    {
        // Arrange

        // Act
        ComplexResult<int> iRes = new(_exception);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(iRes.Success, Is.False);
            Assert.That(iRes.ErrorMessage, Is.EqualTo(_exception.Message));
        }
    }

    [Test, Description("Testing generic ComplexResult copy-constructor on succeeded state.")]
    public void Result_Constructor_05()
    {
        // Arrange
        const int nValue = 12345678;
        IComplexResult<int> iTmp = new ComplexResult<int>(nValue);

        // Act
        ComplexResult<int> iRes = new(iTmp);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(iRes.Success, Is.True);
            Assert.That(iRes.Content, Is.EqualTo(nValue));
            Assert.That(iRes.ErrorMessage, Is.Null);
        }
    }

    [Test, Description("Testing generic ComplexResult copy-constructor on failed state.")]
    public void Result_Constructor_06()
    {
        // Arrange
        ComplexResult<int> iTmp = new(_errorMessage, _exception);

        // Act
        ComplexResult<int> iRes = new(iTmp);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(iRes.Success, Is.False);
            Assert.That(iRes.ErrorMessage, Is.EqualTo(_errorMessage));
        }
    }

    #endregion // Tests_constructors

    #region Tests_ToString

    [Test, Description("Testing ToString() for successful state.")]
    public void Result_ToString_Success()
    {
        // Arrange
        const string successContent = "Success content";
        ComplexResult<string> iRes = new(successContent);

        // Act
        string? result = iRes.ToString();

        // Assert
        Assert.That(result, Is.EqualTo($"Success: {successContent}"));
    }

    [Test, Description("Testing ToString() for failed state with error message.")]
    public void Result_ToString_Failed_ErrorMessage()
    {
        // Arrange
        IComplexResult<int> iRes = ComplexResult<int>.CreateFailed(_errorMessage);

        // Act
        string? result = iRes.ToString();

        // Assert
        Assert.That(result, Is.EqualTo($"Error: {_errorMessage}"));
    }

    [Test, Description("Testing ToString() for failed state with error message and details.")]
    public void Result_ToString_Failed_ErrorDetails()
    {
        // Arrange
        const string errorDetails = "Error details here";
        IComplexResult<int> iRes = ComplexResult<int>.CreateFailed(_errorMessage, errorDetails);

        // Act
        string? result = iRes?.ToString();

        // Assert
        Assert.That(result, Is.EqualTo($"Error: {_errorMessage}, Details: {errorDetails}"));
    }

    [Test, Description("Testing ToString() for failed state with an exception.")]
    public void Result_ToString_Failed_Exception()
    {
        // Arrange
        IComplexResult<object> iRes = ComplexResult<object>.CreateFailed(_exception);

        // Act
        string? result = iRes.ToString();

        // Assert
        Assert.That(result, Is.EqualTo($"Error: {_exception.Message}, Details: {_exception}"));
    }

    #endregion // Tests_ToString

    #region Tests_others

    [Test, Description("Testing generic ComplexResult Create method, for succeeded state.")]
    public void Result_Make_01()
    {
        // Act
        IComplexResult<string> iRes = ComplexResult<string>.CreateSuccessful(_stringRegularValue);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(iRes.Success, Is.True);
            Assert.That(iRes.Content, Is.EqualTo(_stringRegularValue));
            Assert.That(iRes.ErrorMessage, Is.Null);
        }
    }

    [Test, Description("Testing generic ComplexResult Create method, for failed state.")]
    public void Result_Make_02()
    {
        // Act
        IComplexResult<string> iRes = ComplexResult<string>.CreateFailed(_errorMessage);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(iRes.Success, Is.False);
            Assert.That(iRes.ErrorMessage, Is.EqualTo(_errorMessage));
        }
    }
    #endregion // Tests_others
    #endregion // Tests
}

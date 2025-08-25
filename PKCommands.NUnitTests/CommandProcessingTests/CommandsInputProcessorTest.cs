using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using NUnit.Framework;
using PK.Commands.CommandProcessing;
using PK.Commands.Interfaces;
using PK.PkUtils.Consoles;
using ILogger = log4net.ILog;


namespace PK.Commands.NUnitTests.CommandProcessingTests;

/// <summary> Unit Test Fixture implementing tests of CommandsInputProcessor. </summary>
/// <summary>   (Unit Test Fixture) implementing tests of CommandsInputProcessor. </summary>
[TestFixture]
public class CommandsInputProcessorTest
{
    private Mock<ILogger> _mockLogger;
    private Mock<IConsoleDisplay> _mockDisplay;
    private Mock<ICommandRegister<ICommand<int>, int>> _mockCommandRegister;
    private CommandsInputProcessor<ICommand<int>, int> _processor;

    /// <summary>
    /// Setup method to initialize mocks and the tested object before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger>();
        _mockDisplay = new Mock<IConsoleDisplay>();
        _mockCommandRegister = new Mock<ICommandRegister<ICommand<int>, int>>();
        _processor = new CommandsInputProcessor<ICommand<int>, int>(_mockLogger.Object, _mockDisplay.Object, _mockCommandRegister.Object);
    }

    #region Tests
    #region Constructor Tests

    /// <summary>
    /// Verifies that constructor throws ArgumentNullException when logger is null.
    /// </summary>
    [Test, Description("Constructor throws ArgumentNullException if logger is null.")]
    public void Constructor_ThrowsArgumentNullException_IfLoggerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new CommandsInputProcessor<ICommand<int>, int>(null, _mockDisplay.Object));
    }

    /// <summary>
    /// Verifies that constructor initializes correctly with valid parameters.
    /// </summary>
    [Test, Description("Constructor initializes correctly with valid parameters.")]
    public void Constructor_InitializesCorrectly()
    {
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_processor, Is.Not.Null);
            Assert.That(_processor.HasBeenInitialized, Is.False);
        });
    }
    #endregion // Constructor Tests

    #region Initialization Tests

    /// <summary>
    /// Verifies that InitializeCommandContainer returns true on successful initialization.
    /// </summary>
    [Test, Description("InitializeCommandContainer returns true on successful initialization.")]
    public void InitializeCommandContainer_ReturnsTrue_OnSuccess()
    {
        // Act
        bool result = _processor.InitializeCommandContainer(_mockLogger.Object, Assembly.GetExecutingAssembly());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(_processor.HasBeenInitialized, Is.True);
        });
    }
    #endregion // Initialization Tests

    #region ProcessNextInput Tests

    /// <summary>
    /// Verifies that ProcessNextInput throws ArgumentNullException when input is null.
    /// </summary>
    [Test, Description("ProcessNextInput throws ArgumentNullException if input is null.")]
    public void ProcessNextInput_ThrowsArgumentNullException_IfInputIsNull()
    {
        // Act
        _processor.InitializeCommandContainer(_mockLogger.Object, Assembly.GetExecutingAssembly());

        // Assert
        Assert.Throws<ArgumentNullException>(() => _processor.ProcessNextInput((string)null, out _));
    }

    /// <summary>
    /// Verifies that ProcessNextInput returns OK result for empty input.
    /// </summary>
    [Test, Description("ProcessNextInput returns OK result for empty input.")]
    public void ProcessNextInput_ReturnsOK_ForEmptyInput()
    {
        // Arrange
        _processor.InitializeCommandContainer(_mockLogger.Object, Assembly.GetExecutingAssembly());

        // Act
        var result = _processor.ProcessNextInput(string.Empty, out bool shouldContinue);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Success, Is.True);
            Assert.That(shouldContinue, Is.False);
        });
    }
    #endregion // ProcessNextInput Tests

    #region OnCompleted and OnError Tests

    /// <summary>
    /// Verifies that OnCompleted sets HasFinished to true.
    /// </summary>
    [Test, Description("OnCompleted sets HasFinished to true.")]
    public void OnCompleted_SetsHasFinishedToTrue()
    {
        // Act
        _processor.OnCompleted();

        // Assert
        Assert.That(_processor.HasFinished, Is.True);
    }

    /// <summary>
    /// Verifies that OnError logs the error message.
    /// </summary>
    [Test, Description("OnError logs the error message.")]
    public void OnError_LogsErrorMessage()
    {
        // Arrange
        const string errorMessage = "Test error";

        _processor.InitializeCommandContainer(_mockLogger.Object, Assembly.GetExecutingAssembly());

        // Act
        _processor.OnError(new Exception(errorMessage));

        // Assert
        _mockLogger.Verify(log => log.Error(It.Is<string>(s => s.Contains(errorMessage))), Times.Once);
    }
    #endregion // OnCompleted and OnError Tests

    #region SplitCommandLine Tests
    #region SplitCommandLine_Valid_Inputs

    /// <summary>
    /// Tests that splits a simple command line with space-separated arguments.
    /// </summary>
    [Test, Description("Splits a simple command line with space-separated arguments.")]
    public void SplitCommandLine_SimpleArguments_ReturnsExpectedList()
    {
        // Arrange
        string input = "arg1 arg2 arg3";
        IReadOnlyCollection<string> expected = ["arg1", "arg2", "arg3"];

        // Act
        IReadOnlyCollection<string> result = CommandsInputProcessor<ICommand<int>, int>.SplitCommandLine(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that handles quoted arguments as single tokens.
    /// </summary>
    [Test, Description("Handles quoted arguments as single tokens.")]
    public void SplitCommandLine_QuotedArgument_ReturnsExpectedList()
    {
        // Arrange
        string input = "arg1 \"arg2 arg3\" arg4";
        IReadOnlyCollection<string> expected = ["arg1", "arg2 arg3", "arg4"];

        // Act
        IReadOnlyCollection<string> result = CommandsInputProcessor<ICommand<int>, int>.SplitCommandLine(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that handles multiple quoted arguments.
    /// </summary>
    [Test, Description("Handles multiple quoted arguments.")]
    public void SplitCommandLine_MultipleQuotedArguments_ReturnsExpectedList()
    {
        // Arrange
        string input = "\"arg1 arg2\" \"arg3 arg4\"";
        IReadOnlyCollection<string> expected = ["arg1 arg2", "arg3 arg4"];

        // Act
        IReadOnlyCollection<string> result = CommandsInputProcessor<ICommand<int>, int>.SplitCommandLine(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that handles mixed quoted and unquoted arguments.
    /// </summary>
    [Test, Description("Handles mixed quoted and unquoted arguments.")]
    public void SplitCommandLine_MixedQuotedAndUnquotedArguments_ReturnsExpectedList()
    {
        // Arrange
        string input = "arg1 \"arg2 arg3\" arg4 \"arg5 arg6\"";
        IReadOnlyCollection<string> expected = ["arg1", "arg2 arg3", "arg4", "arg5 arg6"];

        // Act
        IReadOnlyCollection<string> result = CommandsInputProcessor<ICommand<int>, int>.SplitCommandLine(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that handles extra spaces between arguments.
    /// </summary>
    [Test, Description("Handles extra spaces between arguments.")]
    public void SplitCommandLine_ExtraSpaces_IgnoresExtraSpaces()
    {
        // Arrange
        string input = "  arg1   arg2    \"arg3  arg4\"   ";
        IReadOnlyCollection<string> expected = ["arg1", "arg2", "arg3  arg4"];

        // Act
        IReadOnlyCollection<string> result = CommandsInputProcessor<ICommand<int>, int>.SplitCommandLine(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
    #endregion // SplitCommandLine_Valid_Inputs

    #region SplitCommandLine_Edge_Cases

    /// <summary>
    /// Tests that returns an empty list for an empty input string.
    /// </summary>
    [Test, Description("Returns an empty list for an empty input string.")]
    public void SplitCommandLine_EmptyString_ReturnsEmptyList()
    {
        // Arrange
        string input = "";
        IReadOnlyCollection<string> expected = [];

        // Act
        IReadOnlyCollection<string> result = CommandsInputProcessor<ICommand<int>, int>.SplitCommandLine(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that handles input with only spaces.
    /// </summary>
    [Test, Description("Handles input with only spaces.")]
    public void SplitCommandLine_OnlySpaces_ReturnsEmptyList()
    {
        // Arrange
        string input = "      ";
        IReadOnlyCollection<string> expected = [];

        // Act
        IReadOnlyCollection<string> result = CommandsInputProcessor<ICommand<int>, int>.SplitCommandLine(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that handles an input containing only a quoted string.
    /// </summary>
    [Test, Description("Handles an input containing only a quoted string.")]
    public void SplitCommandLine_OnlyQuotedString_ReturnsSingleElement()
    {
        // Arrange
        string input = "\"arg1 arg2\"";
        IReadOnlyCollection<string> expected = ["arg1 arg2"];

        // Act
        IReadOnlyCollection<string> result = CommandsInputProcessor<ICommand<int>, int>.SplitCommandLine(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that handles an input with an unclosed quote.
    /// </summary>
    [Test, Description("Handles an input with an unclosed quote.")]
    public void SplitCommandLine_UnclosedQuote_ReturnsIncompleteParsing()
    {
        // Arrange
        string input = "arg1 \"arg2 arg3";
        IReadOnlyCollection<string> expected = ["arg1", "arg2 arg3"]; // Assumes implicit closing

        // Act
        IReadOnlyCollection<string> result = CommandsInputProcessor<ICommand<int>, int>.SplitCommandLine(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that handles input with consecutive quotes.
    /// </summary>
    [Test, Description("Handles input with consecutive quotes.")]
    public void SplitCommandLine_ConsecutiveQuotes_ReturnsEmptyStrings()
    {
        // Arrange
        string input = "\"\" \"arg1 arg2\" \"\"";
        IReadOnlyCollection<string> expected = ["", "arg1 arg2", ""];

        // Act
        IReadOnlyCollection<string> result = CommandsInputProcessor<ICommand<int>, int>.SplitCommandLine(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
    #endregion // SplitCommandLine_Edge_Cases

    #region SplitCommandLine_Error_Cases

    /// <summary>
    /// Tests that throws an exception for null input.
    /// </summary>
    [Test, Description("Throws an exception for null input.")]
    public void SplitCommandLine_NullInput_ThrowsException()
    {
        // Act + Assert
        Assert.Throws<ArgumentNullException>(() => CommandsInputProcessor<ICommand<int>, int>.SplitCommandLine(null));
    }
    #endregion // SplitCommandLine_Error_Cases
    #endregion // SplitCommandLine Tests
    #endregion // Tests
}

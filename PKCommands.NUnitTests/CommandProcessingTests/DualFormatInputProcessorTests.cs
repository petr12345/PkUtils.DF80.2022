using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PK.Commands.CommandProcessing;
using PK.Commands.Interfaces;
using PK.PkUtils.Cmd;
using PK.PkUtils.Consoles;
using ILogger = log4net.ILog;

namespace PK.Commands.NUnitTests.CommandProcessingTests;


/// <summary>   (Unit Test Fixture) a of class DualFormatInputProcessor. </summary>
[TestFixture]
public class DualFormatInputProcessorTests
{
    private CommandsInputProcessor<ICommand> _processor;
    private Mock<ILogger> _mockLogger;
    private Mock<IConsoleDisplay> _mockDisplay;

    /// <summary>   Sets up this unit test fixture. </summary>
    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger>();
        _mockDisplay = new Mock<IConsoleDisplay>();
        _processor = new DualFormatInputProcessor<ICommand>(_mockLogger.Object, _mockDisplay.Object);
    }

    #region ParseInputArgs Tests

    /// <summary>   Tests CommandsInputProcessor.ParseInputArgs. </summary>
    /// <param name="inputArgs"> The input arguments. </param>
    /// <param name="expectedCommand"> The expected command. </param>
    /// <param name="param1Key"> The parameter 1 key. </param>
    /// <param name="param1Value"> The parameter 1 value. </param>
    /// <param name="param2Key"> The parameter 2 key. </param>
    /// <param name="param2Value"> The parameter 2 value. </param>
    [Test, Description("ParseInputArgs correctly extracts command name and parameters for different input formats.")]
    [TestCase(new string[] { "Command", "/Param1", "Value1", "/Param2", "Value2" }, "Command", "Param1", "Value1", "Param2", "Value2")]
    [TestCase(new string[] { "Command", "/Param1:Value1", "/Param2:Value2" }, "Command", "Param1", "Value1", "Param2", "Value2")]
    public void ParseInputArgs_CorrectlyExtractsCommandNameAndParameters(
        string[] inputArgs,
        string expectedCommand,
        string param1Key, string param1Value,
        string param2Key, string param2Value)
    {
        // Act
        IReadOnlyDictionary<string, string> parsedArgs = _processor.ParseInputArgs(inputArgs, out string cmdName);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(cmdName, Is.EqualTo(expectedCommand), "Command name mismatch.");
            Assert.That(parsedArgs.ContainsKey(param1Key), Is.True, "Missing first parameter key.");
            Assert.That(parsedArgs[param1Key], Is.EqualTo(param1Value), "First parameter value mismatch.");
            Assert.That(parsedArgs.ContainsKey(param2Key), Is.True, "Missing second parameter key.");
            Assert.That(parsedArgs[param2Key], Is.EqualTo(param2Value), "Second parameter value mismatch.");
        });
    }

    [Test, Description("ParseInputArgs should throw ArgumentNullException on null argument.")]
    public void ParseInputArgs_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _processor.ParseInputArgs(null, out _));
    }

    [Test, Description("ParseInputArgs should throw ArgumentException on empty argument.")]
    public void ParseInputArgs_ThrowsArgumentException()
    {
        // Arrange
        IEnumerable<string> input = [];

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _processor.ParseInputArgs(input, out _));
    }

#pragma warning disable IDE0060  // Remove unused parameter 'comment' if it is not part of a shipped public API

    [Test, Description("ParseInputArgs should throw InputLineValidationException on specific input cases.")]
    [TestCase(new string[] { "convert", "-inputLibrary", "value1", "-inputLibrary", "value2" }, "command with duplicated arguments")]
    [TestCase(new string[] { "convert", "-", "value1" }, "case of empty argument name")]
    public void ParseInputArgs_ThrowsInputLineValidationException(string[] inputArgs, string comment)
    {
        // Act & Assert
        Assert.Throws<InputLineValidationException>(() => _processor.ParseInputArgs(inputArgs, out _));
    }
#pragma warning restore IDE0060

    #endregion // ParseInputArgs Tests
}

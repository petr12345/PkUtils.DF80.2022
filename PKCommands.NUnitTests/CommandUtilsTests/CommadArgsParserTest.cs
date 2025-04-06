using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PK.Commands.CommandUtils;

namespace PK.Commands.NUnitTests.CommandUtilsTests;

[TestFixture(Description = "Tests of class CommandArgsParser")]
public class CommandArgsParserTest
{
    #region Tests

    #region Test: Single Parameter Without Value

    [Test, Description("Tests parsing of single parameter without value")]
    public void TestSingleParameterWithoutValue()
    {
        var args = new List<string> { "-param" };
        var parser = new CommandArgsParser(args, StringComparer.OrdinalIgnoreCase);

        Assert.That(parser.Parameters, Has.Count.EqualTo(1));
        Assert.That(parser.Parameters["param"], Is.EqualTo("true"));
        Assert.That(parser.OriginalParametersOrder, Has.Count.EqualTo(1));
        Assert.That(parser.OriginalParametersOrder.First(), Is.EqualTo("param"));
    }
    #endregion // Test: Single Parameter Without Value

    #region Test: Parameter With Value

    [Test, Description("Tests parsing of parameter with value")]
    public void TestParameterWithValue()
    {
        var args = new List<string> { "-param=value" };
        var parser = new CommandArgsParser(args, StringComparer.OrdinalIgnoreCase);

        Assert.That(parser.Parameters, Has.Count.EqualTo(1));
        Assert.That(parser.Parameters["param"], Is.EqualTo("value"));
        Assert.That(parser.OriginalParametersOrder, Has.Count.EqualTo(1));
        Assert.That(parser.OriginalParametersOrder.First(), Is.EqualTo("param"));
    }
    #endregion // Test: Parameter With Value

    #region Test: Multiple Parameters

    [Test, Description("Tests parsing of multiple parameters")]
    public void TestMultipleParameters()
    {
        var args = new List<string> { "-param1=value1", "-param2=value2" };
        var parser = new CommandArgsParser(args, StringComparer.OrdinalIgnoreCase);
        string[] parsed = parser.OriginalParametersOrder.ToArray();

        Assert.That(parser.Parameters, Has.Count.EqualTo(2));
        Assert.That(parser.Parameters["param1"], Is.EqualTo("value1"));
        Assert.That(parser.Parameters["param2"], Is.EqualTo("value2"));
        Assert.That(parser.OriginalParametersOrder, Has.Count.EqualTo(2));
        Assert.That(parsed[0], Is.EqualTo("param1"));
        Assert.That(parsed[1], Is.EqualTo("param2"));
    }
    #endregion // Test: Multiple Parameters

    #region Test: Parameter With Enclosed Value

    [Test, Description("Tests parsing of parameter with enclosed quotes")]
    public void TestParameterWithEnclosedValue()
    {
        var args = new List<string> { "-param=\"value\"" };
        var parser = new CommandArgsParser(args, StringComparer.OrdinalIgnoreCase);

        Assert.That(parser.Parameters, Has.Count.EqualTo(1));
        Assert.That(parser.Parameters["param"], Is.EqualTo("value"));
        Assert.That(parser.OriginalParametersOrder, Has.Count.EqualTo(1));
        Assert.That(parser.OriginalParametersOrder.First(), Is.EqualTo("param"));
    }
    #endregion // Test: Parameter With Enclosed Value

    #region Test: Parameter Without Value

    [Test, Description("Tests handling of parameter without value")]
    public void TestParameterWithoutValue()
    {
        var args = new List<string> { "-param" };
        var parser = new CommandArgsParser(args, StringComparer.OrdinalIgnoreCase);

        Assert.That(parser.Parameters, Has.Count.EqualTo(1));
        Assert.That(parser.Parameters["param"], Is.EqualTo("true"));
        Assert.That(parser.OriginalParametersOrder, Has.Count.EqualTo(1));
        Assert.That(parser.OriginalParametersOrder.First(), Is.EqualTo("param"));
    }
    #endregion // Test: Parameter Without Value

    #region Test: Parameter With Special Characters

    [Test, Description("Tests parsing of parameter with special characters")]
    public void TestParameterWithSpecialCharacters()
    {
        var args = new List<string> { "-param=value!@#$%" };
        var parser = new CommandArgsParser(args, StringComparer.OrdinalIgnoreCase);

        Assert.That(parser.Parameters, Has.Count.EqualTo(1));
        Assert.That(parser.Parameters["param"], Is.EqualTo("value!@#$%"));
        Assert.That(parser.OriginalParametersOrder, Has.Count.EqualTo(1));
        Assert.That(parser.OriginalParametersOrder.First(), Is.EqualTo("param"));
    }
    #endregion // Test: Parameter With Special Characters

    #region Test: Parameter With Equal Sign In Value

    [Test, Description("Tests parsing of parameters with equal signs in values")]
    public void TestParameterWithEqualSignInValue()
    {
        var args = new List<string> { "-param=value=123" };
        var parser = new CommandArgsParser(args, StringComparer.OrdinalIgnoreCase);

        Assert.That(parser.Parameters, Has.Count.EqualTo(1));
        Assert.That(parser.Parameters["param"], Is.EqualTo("value=123"));
        Assert.That(parser.OriginalParametersOrder, Has.Count.EqualTo(1));
        Assert.That(parser.OriginalParametersOrder.First(), Is.EqualTo("param"));
    }
    #endregion // Test: Parameter With Equal Sign In Value

    #region Test: Case Insensitivity of Parameter Keys

    [Test, Description("Tests case-insensitivity of parameter keys")]
    public void TestCaseInsensitivityOfParameterKeys()
    {
        var args = new List<string> { "-Param=value" };
        var parser = new CommandArgsParser(args, StringComparer.OrdinalIgnoreCase);

        Assert.That(parser.Parameters, Has.Count.EqualTo(1));
        Assert.That(parser.Parameters["param"], Is.EqualTo("value"));
        Assert.That(parser.OriginalParametersOrder, Has.Count.EqualTo(1));
        Assert.That(parser.OriginalParametersOrder.First(), Is.EqualTo("Param"));
    }
    #endregion // Test: Case Insensitivity of Parameter Keys

    #region Test: Empty Arguments

    [Test, Description("Tests handling of empty arguments list")]
    public void TestEmptyArguments()
    {
        var args = new List<string>();
        var parser = new CommandArgsParser(args, StringComparer.OrdinalIgnoreCase);

        Assert.That(parser.Parameters, Has.Count.EqualTo(0));
        Assert.That(parser.OriginalParametersOrder, Has.Count.EqualTo(0));
    }
    #endregion // Test: Empty Arguments

    #region Test: Argument With Spaces in Value

    [Test, Description("Tests parsing of argument with a value containing spaces")]
    public void TestArgumentWithSpacesInValue()
    {
        var args = new List<string> { "-param=Value with spaces" };
        var parser = new CommandArgsParser(args, StringComparer.OrdinalIgnoreCase);

        Assert.That(parser.Parameters, Has.Count.EqualTo(1));
        Assert.That(parser.Parameters["param"], Is.EqualTo("Value with spaces"));
        Assert.That(parser.OriginalParametersOrder, Has.Count.EqualTo(1));
        Assert.That(parser.OriginalParametersOrder.First(), Is.EqualTo("param"));
    }
    #endregion // Test: Argument With Spaces in Value

    #endregion // Tests
}

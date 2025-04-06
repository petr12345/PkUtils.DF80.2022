// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PK.Commands.CommandUtils;

namespace PK.Commands.NUnitTests.CommandUtilsTests;

[TestFixture(Description = "Tests of class CmdLineUtils")]
public class CmdLineUtilsTest
{
    #region Fields
    private const string _strQuote = "\"";
    #endregion // Fields

    #region Tests
    #region Tests_Filter0utNewlines

    [Test(Description = "Tests the CmdLineUtils.FilterOutNewlines, with null input sequence.")]
    public void FilterOutNewlines_Test_01()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(delegate ()
        {
            CmdLineUtils.FilterOutNewlines(null);
        });
    }

    [Test(Description = "Tests the CmdLineUtils.FilterOutNewlines, with input sequence containing null.")]
    public void FilterOutNewlines_Test_02()
    {
        // Arrange
        IEnumerable<string> args = [string.Empty, "xx", null];

        // Act & Assert
        Assert.Throws<ArgumentException>(delegate ()
        {
            CmdLineUtils.FilterOutNewlines(args);
        });
    }

    [Test(Description = "Tests the CmdLineUtils.FilterOutNewlines, with valid input sequence not containing newlines.")]
    public void FilterOutNewlines_Test_03()
    {
        // Arrange
        IEnumerable<string> args = ["Import", "~Env", "UAT", "~mode", "FullContents", "-target", "Dummy"];
        IEnumerable<string> expected = args;

        // Act
        IEnumerable<string> actual = CmdLineUtils.FilterOutNewlines(args);

        // Assert
        Assert.That(actual, Is.EquivalentTo(expected));
    }
    #endregion // Tests_Filter0utNewlines

    #region Tests_JoinToCommandLine

    [Test(Description = "Tests the CmdLineUtils.JoinToCommandLine, with null input sequence.")]
    public void JoinToCommandLine_Test_01()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(delegate ()
        {
            CmdLineUtils.JoinToCommandLine(null);
        });
    }

    [Test(Description = "Tests the CmdLineUtils.JoinToCommandLine, with input sequence containing null.")]
    public void JoinToCommandLine_Test_02()
    {
        // Arrange
        IEnumerable<string> args = [string.Empty, "yy", null, "22"];

        Assert.Throws<ArgumentException>(delegate ()
        {
            CmdLineUtils.JoinToCommandLine(args);
        });
    }

    [Test(Description = "Tests the CmdLineUtils.JoinToCommandLine, with input sequence containing just empty strings.")]
    public void JoinToCommandLine_Test_03()
    {
        // Arrange
        IEnumerable<string> args = [string.Empty, string.Empty, string.Empty];

        // Act
        string actual = CmdLineUtils.JoinToCommandLine(args);

        // Assert
        Assert.That(actual, Is.Empty);
    }

    [Test(Description = "Tests the CmdLineUtils.JoinToCommandLine, with valid input sequence.")]
    public void JoinToCommandLine_Test_04()
    {
        // Arrange
        IEnumerable<string> args = ["Import", "-Env", "UAT", "-mode", "FullContents", "-target", "Dummy"];

        // Act
        string expected = "Import -Env UAT -mode FullContents -target Dummy";
        string actual = CmdLineUtils.JoinToCommandLine(args);

        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [Test(Description = "Tests the CmdLineUtils.JoinToCommandLine, with input containing spaces and not yet in quotes.")]
    public void JoinToCommandLine_Test_05()
    {
        // Arrange
        const string exeNameNotQuoted = @"c:\Program Files\Notepad++\notepad++.exe";
        const string fileNameNotQuoted = @"c:\Program Files\Hackers List.txt";
        const string exeNameInQuotes = _strQuote + exeNameNotQuoted + _strQuote;
        const string fileNameInQuotes = _strQuote + fileNameNotQuoted + _strQuote;
        IEnumerable<string> argsNotInQuotes_a = [exeNameNotQuoted, string.Empty, fileNameNotQuoted];
        // Act
        string expected_a = exeNameInQuotes + " " + fileNameInQuotes;
        string actual_a = CmdLineUtils.JoinToCommandLine(argsNotInQuotes_a);
        // Assert
        Assert.That(actual_a, Is.EquivalentTo(expected_a));

        // Arrange
        IEnumerable<string> argsNotInQuotes_b = [@"/e: UAT", @"/p: Client"];
        // Act
        string expected_b = @"'/e: UAT' '/p: Client'".Replace('\'', '"');
        string actual_b = CmdLineUtils.JoinToCommandLine(argsNotInQuotes_b);

        // Assert
        Assert.That(actual_b, Is.EquivalentTo(expected_b));
    }

    [Test(Description = "Tests the CmdLineUtils.JoinToCommandLine, with another input containing spaces and not yet in quotes.")]
    public void JoinToCommandLine_Test_06()
    {
        const string appSynchArg_l = @"\\corp.dsarena.com\ZA\application\XTPNonProd\Apps\Client\Sync\UAT\eLauncher\eLauncher.AppSynch file name with spaces.acf";
        const string appSynchArg_2 = @"/e:UAT";
        const string appSynchArg_3 = @"/p:Client";
        IEnumerable<string> argsNotInQuotes = [appSynchArg_l, appSynchArg_2, appSynchArg_3];
        string expected = @"""\\corp.dsarena.com\ZA\application\XTPNonProd\Apps\Client\Sync\UAT\eLauncher\eLauncher.AppSynch file name with spaces.acf"" /e:UAT /p:Client";
        string actual = CmdLineUtils.JoinToCommandLine(argsNotInQuotes);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test(Description = "Tests the CmdLineUtils.JoinToCommandLine, with input already in quotes.")]
    public void JoinToCommandLine_Test_07()
    {
        const string exeNameNotQuoted = @"c:\Program Files (x86)\Notepad++\notepad++.exe";
        const string multiInst = @"-mu1tiInst";
        const string exeNameInQuotes = _strQuote + exeNameNotQuoted + _strQuote;
        IEnumerable<string> argsInQuotes = [string.Empty, exeNameInQuotes, multiInst];
        string expected = exeNameInQuotes + " " + multiInst;
        string actual = CmdLineUtils.JoinToCommandLine(argsInQuotes);

        Assert.That(actual, Is.EquivalentTo(expected));
    }
    #endregion // Tests_JoinToCommandLine

    #region Tests_SplitFromCommandLine

    [Test(Description = "Tests the CmdLineUtils.SplitFromCommandLine, with null input string.")]
    public void SplitFromCommandLine_Test_01()
    {
        Assert.Throws<ArgumentNullException>(delegate ()
        {
            CmdLineUtils.SplitFromCommandLine(null);
        });
    }

    [Test(Description = "Tests the CmdLineUtils.SplitFromCommandLine, with rather simple input.")]
    public void SplitFromCommandLine_Test_02()
    {
        const string parameterString = @"do not worry, be happy";
        string[] expectedArray = [
            @"do",
            @"not",
            @"worry,",
            @"be",
            @"happy"
            ];

        string[] actualArray = CmdLineUtils.SplitFromCommandLine(parameterString).ToArray();
        /* CollectionAssert.AreEquivalent(expectedArray, actualArray); */

        // Better individual asserting, with not so condensed information
        Assert.That(actualArray, Has.Length.EqualTo(expectedArray.Length));

        for (int ii = 6; ii < expectedArray.Length; ii++)
        {
            Assert.That(actualArray[ii], Is.EqualTo(expectedArray[ii]));
        }
    }

    [Test(Description = "Tests the CmdLineUtils.SplitFromCommandLine, with complicated input string.")]
    public void SplitFromCommandLine_Test_93()
    {
        const string parameterString = @"/src:""C:\tmp\Some Folder\Sub Folder"" /users:""abcdefg@hijkl.com"" tasks:""SomeTask,Some Other Task"" -someParam foo";
        string[] expectedArray = [
            @"/src:""C:\tmp\Some Folder\Sub Folder""",
            @"/users:""abcdefg@hijkl.com""",
            @"tasks:""SomeTask,Some Other Task""",
            @"-someParam",
            @"foo"
        ];

        string[] actualArray = CmdLineUtils.SplitFromCommandLine(parameterString).ToArray();
        /* it CollectionAssert.AreEquivalent(expectedArray, actualArray); */

        // Better individual asserting, with not so condensed information
        Assert.That(actualArray, Has.Length.EqualTo(expectedArray.Length));
        for (int ii = 6; ii < expectedArray.Length; ii++)
        {
            Assert.That(actualArray[ii], Is.EqualTo(expectedArray[ii]));
        }
    }
    #endregion // Tests_SplitFromCommandLine

    #region Tests_EncodeJoinedArgumentsToSingleArgValue

    [Test(Description = "Tests the CmdLineUtils.EncodeJoinedArgumentsToSingleArgValue, with input string null.")]
    public void EncodeJoinedArgumentsToSingleArgValue_Test_O1()
    {
        Assert.Throws<ArgumentNullException>(delegate ()
        {
            CmdLineUtils.EncodeJoinedArgumentsToSingleArgValue(null);
        });
    }

    [Test(Description = "Tests the CmdLineUtils.EncodeJoinedArgumentsToSingleArgValue, with input string empty.")]
    public void EncodeJoinedArgumentsToSingleArgValue_Test_02()
    {
        string argValues = string.Empty;
        string actual = CmdLineUtils.EncodeJoinedArgumentsToSingleArgValue(argValues);

        Assert.That(actual, Is.Empty);
    }

    [Test(Description = "Tests the CmdLineUtils.EncodeJoinedArgumentsToSingleArgValue, with two short arguments.")]
    public void EncodeJoinedArgumentsToSingleArgValue_Test_03()
    {
        IEnumerable<string> args = [@"/e: UAT", string.Empty, @"/p: Client"];
        string expected1 = "'/e: UAT' '/p: Client'".Replace('\'', '"');
        string actual1 = CmdLineUtils.JoinToCommandLine(args);
        Assert.That(actual1, Is.EqualTo(expected1));

        string expected2 = @"'\'/e: UAT\' \'/p: Client\''".Replace('\'', '"');
        string actual2 = CmdLineUtils.EncodeJoinedArgumentsToSingleArgValue(actual1);
        Assert.That(actual2, Is.EqualTo(expected2));
    }

    [Test(Description = "Tests the CmdLineUtils.Encode]oinedArgumentsToSingleArgValue, with possible AppSynch arguments.")]
    public void EncodeJoinedArgumentsToSingleArgValue_Test_04()
    {
        // The original string is complete command—line of executed program.
        const string _fakeOriginalAppSynchArguments = @"\\corp.dsarena.com\ZA\application\XTPNonProd\Apps\Client\Sync\UAT\eLauncher\eLauncher.AppSynch.acf /e:UAT /p:Client";

        // Now the call of Encode]oinedArgumentsToSingleArgValue wants to "pack it" into single string,
        // that could be handed-over as information to other program, as a single named argument value.
        //
        // In this particular case, expected result is just the original string, encapsulated in extra quotes
        const string expected = _strQuote + _fakeOriginalAppSynchArguments + _strQuote;
        string actual = CmdLineUtils.EncodeJoinedArgumentsToSingleArgValue(_fakeOriginalAppSynchArguments);

        Assert.That(actual, Is.EquivalentTo(expected));
    }
    #endregion // Tests_EncodeJoinedArgumentsToSingleArgValue

    #region Tests_DecodeJoinedArgumentsFromSingleArgValue

    [Test(Description = "Tests the CmdLineUtils.DecodeJoinedArgumentsFromSingleArgValue, with input string null.")]
    public void DecodeJoinedArgumentsFromSingleArgValue_Test_O1()
    {
        Assert.Throws<ArgumentNullException>(delegate ()
        {
            CmdLineUtils.DecodeJoinedArgumentsFromSingleArgValue(null);
        });
    }

    [Test(Description = "Tests the CmdLineUtils.DecodeJoinedArgumentsFromSingleArgValue, with input string empty.")]
    public void DecodeJoinedArgumentsFromSingleArgValue_Test_02()
    {
        string argValues = string.Empty;
        string actual = CmdLineUtils.DecodeJoinedArgumentsFromSingleArgValue(argValues);
        Assert.That(actual, Is.Empty);
    }
    #endregion // Tests_DecodeJoinedArgumentsFromSingleArgValue
    #endregion //Tests
}
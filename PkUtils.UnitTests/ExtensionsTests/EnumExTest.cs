// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.UnitTests.ExtensionsTests;

#pragma warning disable IDE0300   // Simplify collection initialization

[TestClass()]
public class EnumExTest
{
    #region Auxiliary_types

    [Flags]
    private enum MultiHue : short
    {
        None = 0,
        Black = 1,
        Red = 2,
        Green = 4,
        Blue = 8
    };

    private struct TestPairMsgBoxEnum
    {
        public string StringVal { get; set; }
        public MessageBoxButtons ConvertedVal { get; set; }
        public TestPairMsgBoxEnum(string strVal, MessageBoxButtons buttons) : this()
        {
            StringVal = strVal;
            ConvertedVal = buttons;
        }
    }

    private struct TestPairHue
    {
        public string StringVal { get; set; }
        public MultiHue ConvertedVal { get; set; }
        public TestPairHue(string strVal, MultiHue hue) : this()
        {
            StringVal = strVal;
            ConvertedVal = hue;
        }
    }
    [TestMethod]
    public void EnumEx_CheckIsDefinedValue_throws()
    {
        MessageBoxButtons invalid = (MessageBoxButtons)666;
        Assert.ThrowsExactly<ArgumentException>(() => invalid.CheckIsDefinedValue());
    }

    [TestMethod()]
    public void EnumEx_IsValidFlagsCombinations()
    {
        MultiHue invalid = (MultiHue)666;
        bool actual = invalid.IsValidFlagsCombination();
        Assert.IsFalse(actual);
    }


    [TestMethod()]
    public void EnumEx_CheckIsValidFlagsCombination_throws()
    {
        MultiHue invalid = (MultiHue)666;
        Assert.ThrowsExactly<ArgumentException>(() => invalid.CheckIsValidFlagsCombination());
    }

    [TestMethod()]
    public void EnumEx_CheckIsValidEnum_throws()
    {
        MessageBoxButtons invalid = (MessageBoxButtons)123;
        Assert.ThrowsExactly<ArgumentException>(() => invalid.CheckIsValidEnum());
    }
    #endregion // Checking_values

    #region Tests
    #region Conversions

    #region Tests_GetValues_SimpleEnum

    [TestMethod()]
    public void EnumEx_GetValues_Test()
    {
        IEnumerable<MessageBoxButtons> expected = new MessageBoxButtons[] {
            MessageBoxButtons.OK,
            MessageBoxButtons.OKCancel,
            MessageBoxButtons.AbortRetryIgnore,
            MessageBoxButtons.YesNoCancel,
            MessageBoxButtons.YesNo,
            MessageBoxButtons.RetryCancel,
            MessageBoxButtons.CancelTryContinue,
        };
        IEnumerable<MessageBoxButtons> actual = Enum.GetValues<MessageBoxButtons>();

        CollectionAssert.AreEquivalent(expected.ToList(), actual.ToList());
    }
    #endregion // Tests_GetValues_SimpleEnum

    #region Tests_Parse_SimpleEnum

    [TestMethod()]
    public void EnumEx_ParseTestCaseSensitive_Test_01()
    {
        // for specific strings should return specific values
        IReadOnlyList<TestPairMsgBoxEnum> testPairs = new ReadOnlyCollection<TestPairMsgBoxEnum>(new[]
        {
            new TestPairMsgBoxEnum( "OK", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "OKCancel", MessageBoxButtons.OKCancel ),
            new TestPairMsgBoxEnum( "AbortRetryIgnore", MessageBoxButtons.AbortRetryIgnore ),
            new TestPairMsgBoxEnum( "YesNoCancel", MessageBoxButtons.YesNoCancel),
            new TestPairMsgBoxEnum( "YesNo", MessageBoxButtons.YesNo ),
            new TestPairMsgBoxEnum( "RetryCancel", MessageBoxButtons.RetryCancel),
        });

        foreach (var p in testPairs)
        {
            MessageBoxButtons expected = p.ConvertedVal;
            MessageBoxButtons actual = EnumExtensions.Parse<MessageBoxButtons>(p.StringVal, ignoreCase: false);

            Assert.AreEqual(expected, actual);
        }
    }

    [TestMethod()]
    public void EnumEx_ParseCaseSensitive_Test_02()
    {
        // for strings with wrong casing should return always the default value
        IReadOnlyList<TestPairMsgBoxEnum> testPairs = new ReadOnlyCollection<TestPairMsgBoxEnum>(new[]
        {
            new TestPairMsgBoxEnum( "OK", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "OKCANCEL", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "ABORTRETRYIGNORE", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "YESNOCANCEL", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "YESNO", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "RETRYCANCEL", MessageBoxButtons.OK),
        });

        foreach (var p in testPairs)
        {
            MessageBoxButtons expected = p.ConvertedVal;
            MessageBoxButtons actual = EnumExtensions.ToEnum<MessageBoxButtons>(p.StringVal);

            Assert.AreEqual(expected, actual);
        }
    }

    [TestMethod()]
    public void EnumEx_ParseCaseSensitive_Test_03()
    {
        // for invalid strings should return always the default value
        IReadOnlyList<TestPairMsgBoxEnum> testPairs = new ReadOnlyCollection<TestPairMsgBoxEnum>(new[]
        {
            new TestPairMsgBoxEnum( null!, MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "kobylamamalybok", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "100", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "-100", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "3.14", MessageBoxButtons.OK),
        });

        foreach (var p in testPairs)
        {
            MessageBoxButtons expected = p.ConvertedVal;
            MessageBoxButtons actual = EnumExtensions.ToEnum<MessageBoxButtons>(p.StringVal);

            Assert.AreEqual(expected, actual);
        }
    }

    [TestMethod()]
    public void EnumEx_ParseCaseInsensitive_Test_01()
    {
        // for specific strings with just wrong casing should return specific values
        IReadOnlyList<TestPairMsgBoxEnum> testPairs = new ReadOnlyCollection<TestPairMsgBoxEnum>(new[]
        {
            new TestPairMsgBoxEnum( "ok", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "OKcANCel", MessageBoxButtons.OKCancel ),
            new TestPairMsgBoxEnum( "AbortRETRYignore", MessageBoxButtons.AbortRetryIgnore ),
            new TestPairMsgBoxEnum( "YesNoCancel", MessageBoxButtons.YesNoCancel),
            new TestPairMsgBoxEnum( "yeSnO", MessageBoxButtons.YesNo ),
            new TestPairMsgBoxEnum( "ReTrYCaNceL", MessageBoxButtons.RetryCancel),
        });

        foreach (var p in testPairs)
        {
            MessageBoxButtons expected = p.ConvertedVal;
            MessageBoxButtons actual = EnumExtensions.Parse<MessageBoxButtons>(
              p.StringVal, ignoreCase: true);

            Assert.AreEqual(expected, actual);
        }
    }
    #endregion // Tests_Parse_SimpleEnum

    #region Tests_GetValues_FlagsEnum

    [TestMethod()]
    public void EnumEx_GetValues_FlagsTest()
    {
        IEnumerable<MultiHue> expected = new MultiHue[] {
            MultiHue.None, MultiHue.Black, MultiHue.Red, MultiHue.Green, MultiHue.Blue
        };
        IEnumerable<MultiHue> actual = Enum.GetValues<MultiHue>();

        CollectionAssert.AreEquivalent(expected.ToList(), actual.ToList());
    }
    #endregion // Tests_GetValues_FlagsEnum

    #region Tests_Parse_FlagsEnum

    [TestMethod()]
    public void EnumEx_ParseTestCaseSensitive_FlagsTest_01()
    {
        // for specific strings should return specific values
        IReadOnlyList<TestPairHue> testPairs = new ReadOnlyCollection<TestPairHue>(new[]
        {
            new TestPairHue( "None", MultiHue.None),
            new TestPairHue( "Black", MultiHue.Black),
            new TestPairHue( "Red", MultiHue.Red),
            new TestPairHue( "Green", MultiHue.Green),
            new TestPairHue( "Blue", MultiHue.Blue),
        });

        foreach (var p in testPairs)
        {
            MultiHue expected = p.ConvertedVal;
            MultiHue actual = EnumExtensions.Parse<MultiHue>(p.StringVal, ignoreCase: false);

            Assert.AreEqual(expected, actual);
        }
    }

    [TestMethod()]
    public void EnumEx_ParseCaseSensitive_FlagsTest_02()
    {
        // for strings with wrong casing should return always the default value
        IReadOnlyList<TestPairHue> testPairs = new ReadOnlyCollection<TestPairHue>(new[]
        {
            new TestPairHue( "NONE", MultiHue.None),
            new TestPairHue( "BLACK", MultiHue.None),
            new TestPairHue( "RED", MultiHue.None),
            new TestPairHue( "GREEN", MultiHue.None),
            new TestPairHue( "BLUE", MultiHue.None),
        });

        foreach (var p in testPairs)
        {
            MultiHue expected = p.ConvertedVal;
            MultiHue actual = EnumExtensions.ToEnum<MultiHue>(p.StringVal);

            Assert.AreEqual(expected, actual);
        }
    }

    [TestMethod()]
    public void EnumEx_ParseCaseSensitive_FlagsTest_03()
    {
        // for invalid strings should return always the default value
        IReadOnlyList<TestPairHue> testPairs = new ReadOnlyCollection<TestPairHue>(new[]
        {
            new TestPairHue("None", MultiHue.None),
            new TestPairHue("Black", MultiHue.Black),
            new TestPairHue("Red", MultiHue.Red),
            new TestPairHue("Black, Red", MultiHue.Black | MultiHue.Red),
            new TestPairHue("Green", MultiHue.Green),
            new TestPairHue("Black, Green", MultiHue.Black | MultiHue.Green),
            new TestPairHue("Red, Green", MultiHue.Red | MultiHue.Green),
            new TestPairHue("Black, Red, Green", MultiHue.Black | MultiHue.Red | MultiHue.Green),
            new TestPairHue("Blue", MultiHue.Blue),
            new TestPairHue("Black, Blue", MultiHue.Black | MultiHue.Blue),
            new TestPairHue("Red, Blue", MultiHue.Red | MultiHue.Blue),
            new TestPairHue("Black, Red, Blue", MultiHue.Black | MultiHue.Red | MultiHue.Blue),
            new TestPairHue("Green, Blue", MultiHue.Green | MultiHue.Blue),
            new TestPairHue("Black, Green, Blue", MultiHue.Black | MultiHue.Green | MultiHue.Blue),
            new TestPairHue("Red, Green, Blue", MultiHue.Red | MultiHue.Green | MultiHue.Blue),
            new TestPairHue("Black, Red, Green, Blue", MultiHue.Black | MultiHue.Red | MultiHue.Green | MultiHue.Blue),
            new TestPairHue("XXX", MultiHue.None),
            new TestPairHue("Black, XXX", MultiHue.None),
        });

        foreach (var p in testPairs)
        {
            MultiHue expected = p.ConvertedVal;
            MultiHue actual = EnumExtensions.ToEnum<MultiHue>(p.StringVal);

            Assert.AreEqual(expected, actual);
        }
    }


    [TestMethod()]
    public void EnumEx_ParseCaseInsensitive_FlagsTest_01()
    {
        // for specific strings with just wrong casing should return specific values
        IReadOnlyList<TestPairHue> testPairs = new ReadOnlyCollection<TestPairHue>(new[]
        {
            new TestPairHue("None", MultiHue.None),
            new TestPairHue("BLACK", MultiHue.Black),
            new TestPairHue("RED", MultiHue.Red),
            new TestPairHue("black, red", MultiHue.Black | MultiHue.Red),
            new TestPairHue("green", MultiHue.Green),
            new TestPairHue("BLACK, green", MultiHue.Black | MultiHue.Green),
            new TestPairHue("Red, Green", MultiHue.Red | MultiHue.Green),
            new TestPairHue("black, red, green", MultiHue.Black | MultiHue.Red | MultiHue.Green),
            new TestPairHue("blue", MultiHue.Blue),
            new TestPairHue("Black, BLUE", MultiHue.Black | MultiHue.Blue),
            new TestPairHue("red, BLUE", MultiHue.Red | MultiHue.Blue),
            new TestPairHue("Black, Red, Blue", MultiHue.Black | MultiHue.Red | MultiHue.Blue),
            new TestPairHue("Green, Blue", MultiHue.Green | MultiHue.Blue),
            new TestPairHue("Black, Green, Blue", MultiHue.Black | MultiHue.Green | MultiHue.Blue),
            new TestPairHue("Red, gREEN, Blue", MultiHue.Red | MultiHue.Green | MultiHue.Blue),
            new TestPairHue("Black, RED, Green, Blue", MultiHue.Black | MultiHue.Red | MultiHue.Green | MultiHue.Blue),
        });

        foreach (var p in testPairs)
        {
            MultiHue expected = p.ConvertedVal;
            MultiHue actual = EnumExtensions.Parse<MultiHue>(p.StringVal, ignoreCase: true);

            Assert.AreEqual(expected, actual);
        }
    }
    #endregion // Tests_Parse_FlagsEnum
    #endregion // Conversions

    #endregion // Tests
}
#pragma warning restore IDE0300
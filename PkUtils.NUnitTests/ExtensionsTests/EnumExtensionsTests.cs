// Ignore Spelling: PkUtils, Utils
// 
using System.Collections.ObjectModel;
using System.Windows.Forms;
using PK.PkUtils.Extensions;

#pragma warning disable NUnit2005  // warning NUnit2005: Consider using the constraint model, Assert.That(actual, Is.EqualTo(expected)), instead of the classic model

namespace PK.PkUtils.NUnitTests.UtilsTests;

/// <summary> (Unit Test Fixture) class testing <see cref="EnumExtension"/>. </summary>
[TestFixture()]
public class EnumExtensionsTests
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
    #endregion // Auxiliary_types

    #region Tests

    #region Getting_values
    // ToDo
    #endregion // Getting_values

    #region Checking_values

    [Test]
    [Description("Ensures that CheckIsDefinedValue throws an exception for an undefined enum value.")]
    public void EnumEx_CheckIsDefinedValue_throws()
    {
        MessageBoxButtons invalid = (MessageBoxButtons)666;
        Assert.Throws<ArgumentException>(() => invalid.CheckIsDefinedValue());
    }

    [Test()]
    [Description("Verifies that IsValidFlagsCombination returns false for an invalid flags combination.")]
    public void EnumEx_IsValidFlagsCombinations()
    {
        MultiHue invalid = (MultiHue)666;
        bool actual = invalid.IsValidFlagsCombination();
        bool expected = false;
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test()]
    [Description("Ensures that CheckIsValidFlagsCombination throws an exception for an invalid flags combination.")]
    public void EnumEx_CheckIsValidFlagsCombination_throws()
    {
        MultiHue invalid = (MultiHue)666;
        Assert.Throws<ArgumentException>(() => invalid.CheckIsValidFlagsCombination());
    }

    [Test()]
    [Description("Ensures that CheckIsValidEnum throws an exception for an invalid enum value.")]
    public void EnumEx_CheckIsValidEnum_throws()
    {
        MessageBoxButtons invalid = (MessageBoxButtons)123;
        Assert.Throws<ArgumentException>(() => invalid.CheckIsValidEnum());
    }
    #endregion // Checking_values

    #region Conversions
    #region Tests_Parse_SimpleEnum

    [Test()]
    [Description("Ensures that Parse correctly converts specific case-sensitive strings to enum values.")]
    public void EnumEx_ParseTestCaseSensitive_Test_01()
    {
        IReadOnlyList<TestPairMsgBoxEnum> testPairs = new ReadOnlyCollection<TestPairMsgBoxEnum>([
            new TestPairMsgBoxEnum( "OK", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "OKCancel", MessageBoxButtons.OKCancel ),
            new TestPairMsgBoxEnum( "AbortRetryIgnore", MessageBoxButtons.AbortRetryIgnore ),
            new TestPairMsgBoxEnum( "YesNoCancel", MessageBoxButtons.YesNoCancel),
            new TestPairMsgBoxEnum( "YesNo", MessageBoxButtons.YesNo ),
            new TestPairMsgBoxEnum( "RetryCancel", MessageBoxButtons.RetryCancel),
        ]);

        foreach (var p in testPairs)
        {
            MessageBoxButtons expected = p.ConvertedVal;
            MessageBoxButtons actual = EnumExtension.Parse<MessageBoxButtons>(p.StringVal, ignoreCase: false);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    [Test()]
    [Description("Ensures that Parse returns the default value when the string casing is incorrect.")]
    public void EnumEx_ParseCaseSensitive_Test_02()
    {
        IReadOnlyList<TestPairMsgBoxEnum> testPairs = new ReadOnlyCollection<TestPairMsgBoxEnum>([
            new TestPairMsgBoxEnum( "OK", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "OKCANCEL", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "ABORTRETRYIGNORE", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "YESNOCANCEL", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "YESNO", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "RETRYCANCEL", MessageBoxButtons.OK),
        ]);

        foreach (var p in testPairs)
        {
            MessageBoxButtons expected = p.ConvertedVal;
            MessageBoxButtons actual = EnumExtension.ToEnum<MessageBoxButtons>(p.StringVal);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    [Test()]
    [Description("Ensures that Parse returns the default value for invalid strings.")]
    public void EnumEx_ParseCaseSensitive_Test_03()
    {
        IReadOnlyList<TestPairMsgBoxEnum> testPairs = new ReadOnlyCollection<TestPairMsgBoxEnum>([
            new TestPairMsgBoxEnum( null!, MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "kobylamamalybok", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "100", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "-100", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "3.14", MessageBoxButtons.OK),
        ]);

        foreach (var p in testPairs)
        {
            MessageBoxButtons expected = p.ConvertedVal;
            MessageBoxButtons actual = EnumExtension.ToEnum<MessageBoxButtons>(p.StringVal);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    [Test()]
    [Description("Ensures that Parse correctly converts case-insensitive strings to enum values.")]
    public void EnumEx_ParseCaseInsensitive_Test_01()
    {
        IReadOnlyList<TestPairMsgBoxEnum> testPairs = new ReadOnlyCollection<TestPairMsgBoxEnum>([
            new TestPairMsgBoxEnum( "ok", MessageBoxButtons.OK),
            new TestPairMsgBoxEnum( "OKcANCel", MessageBoxButtons.OKCancel ),
            new TestPairMsgBoxEnum( "AbortRETRYignore", MessageBoxButtons.AbortRetryIgnore ),
            new TestPairMsgBoxEnum( "YesNoCancel", MessageBoxButtons.YesNoCancel),
            new TestPairMsgBoxEnum( "yeSnO", MessageBoxButtons.YesNo ),
            new TestPairMsgBoxEnum( "ReTrYCaNceL", MessageBoxButtons.RetryCancel),
        ]);

        foreach (var p in testPairs)
        {
            MessageBoxButtons expected = p.ConvertedVal;
            MessageBoxButtons actual = EnumExtension.Parse<MessageBoxButtons>(
              p.StringVal, ignoreCase: true);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
    #endregion // Tests_Parse_SimpleEnum

    #region Tests_GetValues_FlagsEnum

    [Test()]
    [Description("Verifies that GetValues returns all values of a flags enum.")]
    public void EnumEx_GetValues_FlagsTest()
    {
        IEnumerable<MultiHue> expected = new MultiHue[] {
            MultiHue.None, MultiHue.Black, MultiHue.Red, MultiHue.Green, MultiHue.Blue
        };
        IEnumerable<MultiHue> actual = Enum.GetValues<MultiHue>();

        Assert.That(actual.ToList(), Is.EqualTo(expected.ToList()));
    }
    #endregion // Tests_GetValues_FlagsEnum

    #region Tests_Parse_FlagsEnum

    [Test()]
    [Description("Ensures that Parse correctly converts specific case-sensitive strings to flags enum values.")]
    public void EnumEx_ParseTestCaseSensitive_FlagsTest_01()
    {
        // Test specific case-sensitive strings to ensure correct conversion to enum values.
        IReadOnlyList<TestPairHue> testPairs = new ReadOnlyCollection<TestPairHue>([
            new TestPairHue( "None", MultiHue.None),
            new TestPairHue( "Black", MultiHue.Black),
            new TestPairHue( "Red", MultiHue.Red),
            new TestPairHue( "Green", MultiHue.Green),
            new TestPairHue( "Blue", MultiHue.Blue),
        ]);

        foreach (var p in testPairs)
        {
            MultiHue expected = p.ConvertedVal;
            MultiHue actual = EnumExtension.Parse<MultiHue>(p.StringVal, ignoreCase: false);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    [Test()]
    [Description("Ensures that Parse returns the default value for case-sensitive strings with incorrect casing.")]
    public void EnumEx_ParseCaseSensitive_FlagsTest_02()
    {
        // Test strings with incorrect casing to ensure Parse returns the default value.
        IReadOnlyList<TestPairHue> testPairs = new ReadOnlyCollection<TestPairHue>([
            new TestPairHue( "NONE", MultiHue.None),
            new TestPairHue( "BLACK", MultiHue.None),
            new TestPairHue( "RED", MultiHue.None),
            new TestPairHue( "GREEN", MultiHue.None),
            new TestPairHue( "BLUE", MultiHue.None),
        ]);

        foreach (var p in testPairs)
        {
            MultiHue expected = p.ConvertedVal;
            MultiHue actual = EnumExtension.ToEnum<MultiHue>(p.StringVal);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    [Test()]
    [Description("Ensures that Parse returns the default value for invalid case-sensitive strings.")]
    public void EnumEx_ParseCaseSensitive_FlagsTest_03()
    {
        // Test invalid strings to ensure Parse returns the default value.
        IReadOnlyList<TestPairHue> testPairs = new ReadOnlyCollection<TestPairHue>([
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
        ]);

        foreach (var p in testPairs)
        {
            MultiHue expected = p.ConvertedVal;
            MultiHue actual = EnumExtension.ToEnum<MultiHue>(p.StringVal);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    [Test()]
    [Description("Ensures that Parse correctly converts case-insensitive strings to flags enum values.")]
    public void EnumEx_ParseCaseInsensitive_FlagsTest_01()
    {
        // Test case-insensitive strings to ensure correct conversion to enum values.
        IReadOnlyList<TestPairHue> testPairs = new ReadOnlyCollection<TestPairHue>([
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
        ]);

        foreach (var p in testPairs)
        {
            MultiHue expected = p.ConvertedVal;
            MultiHue actual = EnumExtension.Parse<MultiHue>(p.StringVal, ignoreCase: true);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
    #endregion // Tests_Parse_FlagsEnum
    #endregion // Conversions

    #endregion // Tests
}

// Ignore Spelling: Utils
// 

using PK.PkUtils.Dump;

namespace PK.PkUtils.NUnitTests.DumpTests;

/// <summary> This is a test class for <see cref="ObjectDumper"/> </summary>
[TestFixture()]
[CLSCompliant(false)]
public class ObjectDumperTest
{
    #region Tests

    #region DumpHexTests

    /// <summary> A test for DumpHex which should succeed. </summary>
    [Test, Description("Testing ObjectDumper.DumpHex with various input.")]
    [TestCase(null!, "")]
    [TestCase(new byte[] { }, "")]
    [TestCase(new byte[] { 0x01, 0x02, 0x03 }, "01 02 03")]
    [TestCase(new byte[] { 0xFF, 0xAA, 0x00, 0x13 }, "FF AA 00 13")]
    public void ObjectDumper_DumpHex_Test(byte[] inputArray, string expectedResult)
    {
        // Act
        string strActual = ObjectDumper.DumpHex(inputArray);

        // Assert
        Assert.That(strActual, Is.EqualTo(expectedResult));
    }
    #endregion // DumpHexTests

    #region Dump2Text Tests

    /// <summary> A test for DumpHex which should succeed. </summary>
    [Test, Description("Testing ObjectDumper.Dump2Text with null input.")]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("  ")]
    public void ObjectDumper_Dump2Text_ForNull(string lineSeparator)
    {
        // Arrange
        // Fo null or empty separator, Dump2Text uses its default ( newline )
        string actualSeparator = !string.IsNullOrEmpty(lineSeparator) ? lineSeparator : Environment.NewLine;

        // Act
        string expected = "null" + actualSeparator;
        string strActual = ObjectDumper.Dump2Text(null, actualSeparator);

        // Assert
        Assert.That(strActual, Is.EqualTo(expected));
    }

    /// <summary> A test for DumpHex which should succeed. </summary>
    [Test, Description("Testing ObjectDumper.Dump2Text with non-null input.")]
    public void ObjectDumper_Dump2Text_NonNull()
    {
        // Arrange
        string input = "Using Yield to Implement BinaryTree";
        string lineSeparator = Environment.NewLine;

        // Act
        string expected = input + lineSeparator;
        string strActual = ObjectDumper.Dump2Text(input, lineSeparator);

        // Assert
        Assert.That(strActual, Is.EqualTo(expected));
    }
    #endregion // Dump2TextTests
    #endregion // Tests
}

// Ignore Spelling: Utils
// 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Dump;


namespace PK.PkUtils.UnitTests.DumpTests;

/// <summary>
/// This is a test class for FilePathHelper and is intended
/// to contain all FilePathHelperTest Unit Tests
/// </summary>
[TestClass()]
public class ObjectDumperTest
{
    #region Tests

    #region DumpHexTest

    /// <summary> A test for DumpHex . </summary>
    [TestMethod]
    [DataRow(null, "")]
    [DataRow(new byte[] { }, "")]
    [DataRow(new byte[] { 0x01, 0x02, 0x03 }, "01 02 03")]
    [DataRow(new byte[] { 0xFF, 0xAA, 0x00, 0x13 }, "FF AA 00 13")]
    public void ObjectDumper_DumpHex_Test(byte[] inputArray, string expectedResult)
    {
        // Act
        string strActual = ObjectDumper.DumpHex(inputArray);

        // Assert
        Assert.AreEqual(expectedResult, strActual);
    }
    #endregion // DumpHexTest
    #endregion // Tests
}

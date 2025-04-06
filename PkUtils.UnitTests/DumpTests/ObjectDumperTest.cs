// Ignore Spelling: Utils
// 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Dump;


namespace PK.PkUtils.UnitTests.DumpTests
{
    /// <summary>
    /// This is a test class for FilePathHelper and is intended
    /// to contain all FilePathHelperTest Unit Tests
    /// </summary>
    [TestClass()]
    public class ObjectDumperTest
    {
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        #region Tests

        #region DumpHexTest

        /// <summary> A test for DumpHex . </summary>
        [TestClass]
        public class ObjectDumperTests
        {
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
        }
        #endregion // DumpHexTest
        #endregion // Tests
    }
}

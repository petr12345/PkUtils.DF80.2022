// Ignore Spelling: Utils
//
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.UnitTests.ExtensionsTests
{
    /// <summary>
    /// This is a test class for TypeExtension and is intended
    /// to contain all TypeExtensionTest Unit Tests
    /// </summary>
    [TestClass()]
    public class StringExtensionTest
    {
        #region Tests

        #region Tests_Left

        /// <summary> A test for Left that should throw ArgumentNullException. </summary>
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod()]
        public void StringExtension_Left_01()
        {
            string src = null!;
            src.Left(2);
        }

        /// <summary> A test for Left that should throw ArgumentOutOfRangeException. </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StringExtension_Left_02()
        {
            string src = "abc";
            src.Left(-2);
        }

        /// <summary> A test for Left that should throw ArgumentOutOfRangeException. </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StringExtension_Left_03()
        {
            const string src = "abcd";
            src.Left(100);
        }

        /// <summary> A test for Left that should succeed. </summary>
        [TestMethod()]
        public void StringExtension_Left_04()
        {
            const string src = "abcd";
            string expected, actual;

            expected = string.Empty;
            actual = src.Left(0);
            Assert.AreEqual(expected, actual);

            expected = "a";
            actual = src.Left(1);
            Assert.AreEqual(expected, actual);
        }
        #endregion // Tests_Left

        #region Tests_LeftMax

        /// <summary> A test for LeftMax that should throw ArgumentNullException. </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StringExtension_LeftMax_01()
        {
            string src = null!;
            src.LeftMax(2);
        }

        /// <summary> A test for LeftMax that should throw ArgumentOutOfRangeException. </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StringExtension_LeftMax_02()
        {
            string src = "abc";
            src.LeftMax(-2);
        }

        /// <summary> A test for LeftMax that should succeed. </summary>
        [TestMethod()]
        public void StringExtension_LeftMax_03()
        {
            const string src = "abcd";
            string expected, actual;

            expected = string.Empty;
            actual = src.LeftMax(0);
            Assert.AreEqual(expected, actual);

            expected = "a";
            actual = src.LeftMax(1);
            Assert.AreEqual(expected, actual);

            expected = src;
            actual = src.LeftMax(100);
            Assert.AreEqual(expected, actual);
        }
        #endregion // Tests_LeftMax

        #region Tests_Right

        /// <summary> A test for Right that should throw ArgumentNullException. </summary>
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod()]
        public void StringExtension_Right_01()
        {
            string src = null!;
            src.Right(2);
        }

        /// <summary> A test for Right that should throw ArgumentOutOfRangeException. </summary>
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod()]
        public void StringExtension_Right_02()
        {
            string src = "abc";
            src.Right(-2);
        }

        /// <summary> A test for Right that should throw ArgumentOutOfRangeException. </summary>
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod()]
        public void StringExtension_Right_03()
        {
            const string src = "abcd";
            src.Right(100);
        }

        /// <summary> A test for Right that should succeed. </summary>
        [TestMethod()]
        public void StringExtension_Right_04()
        {
            const string src = "abcd";
            string expected, actual;

            expected = string.Empty;
            actual = src.Right(0);
            Assert.AreEqual(expected, actual);

            expected = "d";
            actual = src.Right(1);
            Assert.AreEqual(expected, actual);
        }
        #endregion // Tests_Right

        #region Tests_RightMax

        /// <summary> A test for RightMax that should throw ArgumentNullException. </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StringExtension_RightMax_01()
        {
            string src = null!;
            src.RightMax(2);
        }

        /// <summary> A test for RightMax that should throw ArgumentOutOfRangeException. </summary>
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod()]
        public void StringExtension_RightMax_02()
        {
            string src = "abc";
            src.RightMax(-2);
        }

        /// <summary> A test for RightMax that should succeed. </summary>
        [TestMethod()]
        public void StringExtension_RightMax_03()
        {
            const string src = "abcd";
            string expected, actual;

            expected = string.Empty;
            actual = src.RightMax(0);
            Assert.AreEqual(expected, actual);

            expected = "d";
            actual = src.RightMax(1);
            Assert.AreEqual(expected, actual);

            expected = src;
            actual = src.RightMax(100);
            Assert.AreEqual(expected, actual);
        }
        #endregion // Tests_RightMax

        #endregion // Tests
    }
}

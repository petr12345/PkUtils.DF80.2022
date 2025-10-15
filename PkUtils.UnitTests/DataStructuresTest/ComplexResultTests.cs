// Ignore Spelling: Utils
//
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.UnitTests.DataStructuresTest
{
    /// <summary> Unit Test of generic class Result{T}. </summary>
    [TestClass()]
    public class ComplexResultTests
    {
        #region Fields
        private const string _errorMessage = "#$%^ what the hell ?!";
        private const string _stringRegularValue = "kobylamamalybok";
        private static readonly Exception _exception = new NullReferenceException("Don't worry, be happy");
        #endregion // Fields

        #region Tests

        #region Tests_constructors

        [TestMethod]
        public void Result_Constructor_01()
        {
            const int nValue = 1234567;
            IComplexResult<int> iRes = new ComplexResult<int>(nValue);

            Assert.IsTrue(iRes.Success);
            Assert.AreEqual(nValue, iRes.Content);
            Assert.IsNull(iRes.ErrorMessage);
        }

        [TestMethod]
        public void Result_Constructor_02()
        {
            IComplexResult<int> iRes = new ComplexResult<int>(_errorMessage);

            Assert.IsFalse(iRes.Success);
            Assert.AreEqual(_errorMessage, iRes.ErrorMessage);
        }

        [TestMethod]
        public void Result_Constructor_03()
        {
            IComplexResult<int> iRes = new ComplexResult<int>(_errorMessage, _exception);

            Assert.IsFalse(iRes.Success);
            Assert.AreEqual(_errorMessage, iRes.ErrorMessage);
        }

        [TestMethod]
        public void Result_Constructor_04()
        {
            IComplexResult<int> iRes = new ComplexResult<int>(_exception);

            Assert.IsFalse(iRes.Success);
            Assert.AreEqual(_exception.Message, iRes.ErrorMessage);
        }

        [TestMethod]
        public void Result_Constructor_05()
        {
            const int nValue = 12345678;
            IComplexResult<int> iTmp = new ComplexResult<int>(nValue);
            IComplexResult<int> iRes = new ComplexResult<int>(iTmp);

            Assert.IsTrue(iRes.Success);
            Assert.AreEqual(nValue, iRes.Content);
            Assert.IsNull(iRes.ErrorMessage);
        }

        [TestMethod]
        public void Result_Constructor_06()
        {
            IComplexResult<int> iTmp = new ComplexResult<int>(_errorMessage, _exception);
            IComplexResult<int> iRes = new ComplexResult<int>(iTmp);

            Assert.IsFalse(iRes.Success);
            Assert.AreEqual(_errorMessage, iRes.ErrorMessage);
        }
        #endregion // Tests_constructors

        #region Tests_others

        [TestMethod]
        public void Result_Make_01()
        {
            IComplexResult<string> iRes = ComplexResult<string>.CreateSuccessful(_stringRegularValue);

            Assert.IsTrue(iRes.Success);
            Assert.AreEqual(_stringRegularValue, iRes.Content);
            Assert.IsNull(iRes.ErrorMessage);
        }

        [TestMethod]
        public void Result_Make_02()
        {
            IComplexResult<string> iRes = ComplexResult<string>.CreateFailed(_errorMessage);

            Assert.IsFalse(iRes.Success);
            Assert.AreEqual(_errorMessage, iRes.ErrorMessage);
        }
        #endregion // Tests_others
        #endregion // Tests
    }
}
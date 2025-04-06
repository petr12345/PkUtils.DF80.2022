// Ignore Spelling: Utils
//
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.DataStructures;

namespace PK.PkUtils.UnitTests.DataStructuresTest
{
    /// <summary> Unit Test of generic class Repository. </summary>
    [TestClass()]
    public class RepositoryTests
    {
        #region Tests_constructors

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_Constructor_01()
        {
            new Repository<string>(null!);
        }

        [TestMethod]
        public void Repository_Constructor_02()
        {
            Repository<string> rep = new();

            Assert.IsFalse(rep.HasData);
            Assert.IsFalse(rep.IsAttached);
            Assert.AreEqual(null, rep.Data);
        }

        [TestMethod]
        public void Repository_Constructor_03()
        {
            const string inputPalindrom = "jelenovipivonelej";
            Repository<string> rep = new(inputPalindrom, false);

            Assert.IsTrue(rep.HasData);
            Assert.IsTrue(rep.IsAttached);
            Assert.AreEqual(inputPalindrom, rep.Data);
        }

        [TestMethod]
        public void Repository_Constructor_04()
        {
            const string input = "不要倒鹿啤酒";
            Repository<string> rep = new(input);

            Assert.IsTrue(rep.HasData);
            Assert.IsFalse(rep.IsAttached);
            Assert.AreEqual(input, rep.Data);
        }

        [TestMethod]
        public void Repository_Constructor_05()
        {
            Repository<int> rep = new();

            Assert.IsTrue(rep.HasData);
            Assert.IsFalse(rep.IsAttached);
            Assert.AreEqual(default, rep.Data);
        }

        #endregion // Tests_constructors

        #region Tests_Equals

        [TestMethod]
        public void Repository_Equals_01()
        {
            Repository<string> rep1 = new();
            Repository<string> rep2 = new();

            Assert.IsTrue(rep1.Equals(rep2));
        }

        [TestMethod]
        public void Repository_Equals_02()
        {
            const string input = "荣耀于海盗政党";
            Repository<string> rep1 = new(input);
            Repository<string> rep2 = new(input);
            Repository<string> rep3 = new(input, false);

            Assert.IsTrue(rep1.Equals(rep2));
            Assert.IsFalse(rep1.Equals(rep3));
            Assert.IsFalse(rep2.Equals(rep3));
        }
        #endregion // Tests_Equals

        #region Tests_ToString

        [TestMethod]
        public void Repository_ToString_01()
        {
            const string inputPalindrom = "Báře jede jeřáb";
            Repository<string> rep1 = new();
            Repository<string> rep2 = new(inputPalindrom);
            Repository<string> rep3 = new(inputPalindrom, false);

            string s1 = rep1.ToString();
            string s2 = rep2.ToString();
            string s3 = rep3.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(s1));
            Assert.IsTrue(!string.IsNullOrEmpty(s2));
            Assert.IsTrue(!string.IsNullOrEmpty(s3));
        }
        #endregion // Tests_ToString
    }
}

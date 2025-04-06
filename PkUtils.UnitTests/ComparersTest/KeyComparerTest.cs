// Ignore Spelling: Utils, Comparers
//
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Comparers;


namespace PK.PkUtils.UnitTests.ComparersTest
{
    /// <summary>
    /// This is a test class for KeyComparer generic
    ///</summary>
    [TestClass()]
    public class KeyComparerTest
    {
        #region Tests

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("A test for KeyComparer constructor, which should throw ArgumentNullException")]
        public void KeyComparer_Constructor_01()
        {
            new KeyComparer<int, int>(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("A test for KeyComparer constructor, which should throw ArgumentNullException")]
        public void KeyComparer_Constructor_02()
        {
            new KeyComparer<string, int>(null, null);
        }

        [TestMethod()]
        [Description("A test for KeyComparer constructor, which should succeed")]
        public void KeyComparer_Constructor_03()
        {
            new KeyComparer<string, int>(s => s.Length);
        }

        [TestMethod()]
        [Description("A test for KeyComparer.Compare, which should succeed")]
        public void KeyComparer_EqualsTest_01()
        {
            var comparer = new KeyComparer<int, int>(x => Math.Abs(x));
            var listInts = Enumerable.Range(0, 11).ToList();

            foreach (var x in listInts)
            {
                Assert.AreEqual(0, comparer.Compare(x, x));
                Assert.IsTrue(0 > comparer.Compare(x, x + 1));
            }
        }

        [TestMethod()]
        [Description("A test for KeyComparer.Compare, which should succeed")]
        public void KeyComparer_EqualsTest_02()
        {
            var comparer = new KeyComparer<string, int>(s => s.Length);

            Assert.AreEqual(0, comparer.Compare("aaa", "AAA"));
            Assert.AreEqual(0, comparer.Compare("aaaa", "wXyZ"));
            Assert.IsTrue(0 < comparer.Compare("aaa", "w"));
            Assert.IsTrue(0 > comparer.Compare("a", "www"));
        }

        [TestMethod()]
        [Description("A test for KeyComparer.Create, which should succeed")]
        public void KeyComparer_CreateTest()
        {
            KeyComparer.Create<string, int>(s => s.Length);
        }
        #endregion // Tests
    }
}

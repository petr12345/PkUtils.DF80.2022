// Ignore Spelling: Utils, Comparers
//
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Comparers;


namespace PK.PkUtils.UnitTests.ComparersTest
{
    /// <summary>
    /// This is a test class for KeyEqualityComparer generic
    ///</summary>
    [TestClass()]
    public class KeyEqualityComparerTest
    {
        #region Tests

        /// <summary>
        /// A test for KeyEqualityComparer constructor, which should throw ArgumentNullException
        /// </summary>
        [TestMethod()]
        public void KeyEqualityComparer_Constructor_01()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                var comparer = new KeyEqualityComparer<int, int>(null);
            });
        }

        /// <summary>
        /// A test for KeyEqualityComparer constructor, which should throw ArgumentNullException
        /// </summary>
        [TestMethod()]
        public void KeyEqualityComparer_Constructor_02()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            {
                var comparer = new KeyEqualityComparer<string, int>(null, null);
            });
        }

        /// <summary>
        /// A test for KeyEqualityComparer constructor, which should throw ArgumentNullException
        /// </summary>
        [TestMethod()]
        public void KeyEqualityComparer_Constructor_03()
        {
            var comparer = new KeyEqualityComparer<string, int>(s => s.Length);
        }

        /// <summary>
        /// A test for KeyEqualityComparer.Equals, which should succeed
        /// </summary>
        [TestMethod()]
        public void KeyEqualityComparer_EqualsTest_01()
        {
            var comparer = new KeyEqualityComparer<int, int>(x => Math.Abs(x));

            foreach (var x in Enumerable.Range(-5, 11).ToList())
            {
                Assert.IsTrue(comparer.Equals(x, x));
                Assert.IsTrue(comparer.Equals(x, -x));
            }
        }

        /// <summary>
        /// A test for KeyEqualityComparer.Equals, which should succeed
        /// </summary>
        [TestMethod()]
        public void KeyEqualityComparer_EqualsTest_02()
        {
            var comparer = new KeyEqualityComparer<string, int>(s => s.Length);

            Assert.IsTrue(comparer.Equals("aaa", "AAA"));
            Assert.IsTrue(comparer.Equals("aaaa", "wXyZ"));
            Assert.IsFalse(comparer.Equals("aaa", "w"));
        }
        #endregion // Tests
    }
}

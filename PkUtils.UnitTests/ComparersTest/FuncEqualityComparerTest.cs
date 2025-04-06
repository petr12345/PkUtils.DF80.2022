// Ignore Spelling: Utils, Comparers
//
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Comparers;


namespace PK.PkUtils.UnitTests.ComparersTest
{
#pragma warning disable IDE0039  // suppress "use local functions" warning

    /// <summary>
    /// This is a test class for FunctionalEqualityComparer generic
    ///</summary>
    [TestClass()]
    public class FuncEqualityComparerTest
    {
        #region Tests

        /// <summary>
        /// A test for FunctionalEqualityComparer constructor
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FuncEqualityComparer_Constructor_01()
        {
            new FunctionalEqualityComparer<int>(null);
        }

        /// <summary>
        /// A test for FunctionalEqualityComparer constructor
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FuncEqualityComparer_Constructor_02()
        {
            new FunctionalEqualityComparer<int>((x, y) => x == y, null);
        }

        /// <summary>
        /// A test for FunctionalEqualityComparer constructor
        /// </summary>
        [TestMethod()]
        public void FuncEqualityComparer_Constructor_03()
        {
            var comparer = new FunctionalEqualityComparer<int>((x, y) => x == y, x => x);
        }

        /// <summary>
        /// A test for FunctionalEqualityComparer.Equals
        /// </summary>
        [TestMethod()]
        public void FuncEqualityComparer_CompareTest_01()
        {
            var comparer = new FunctionalEqualityComparer<int>((x, y) => Math.Abs(x) == Math.Abs(y), x => Math.Abs(x));

            foreach (var x in Enumerable.Range(2, 7).ToList())
            {
                Assert.AreEqual(true, comparer.Equals(x, x));
                Assert.AreEqual(true, comparer.Equals(x, -x));
            }
        }

        /// <summary>
        /// A test for FunctionalEqualityComparer.Equals
        /// </summary>
        [TestMethod()]
        public void FuncEqualityComparer_CompareTest_02()
        {
            Func<string, string, bool> f = (x, y) => (x.Length == y.Length);
            Func<string, int> h = (s => s.Length);
            var comparer = new FunctionalEqualityComparer<string>(f, h);

            Assert.AreEqual(true, comparer.Equals("aaa", "AAA"));
            Assert.AreEqual(true, comparer.Equals("aaaa", "wXyZ"));
            Assert.AreEqual(false, comparer.Equals("aaa", "w"));
        }

        /// <summary>
        /// A test for FunctionalEqualityComparer.CreateNullSafeComparer
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FuncEqualityComparer_CreateNullSafeComparerTest_01()
        {
            Func<string, string, bool> f = null!;
            Func<string, int> h = null!;
            FunctionalEqualityComparer.CreateNullSafeComparer<string>(f, h);
        }

        /// <summary>
        /// A test for FunctionalEqualityComparer.CreateNullSafeComparer
        /// </summary>
        [TestMethod()]
        public void FuncEqualityComparer_CreateNullSafeComparerTest_02()
        {
            Func<string, string, bool> f = (x, y) => (x.Length == y.Length);
            Func<string, int> h = x => x.Length;
            var comparer = FunctionalEqualityComparer.CreateNullSafeComparer<string>(f, h);

            Assert.AreEqual(true, comparer.Equals("aaa", "AAA"));
            Assert.AreEqual(true, comparer.Equals("aaa", "XYZ"));

            Assert.IsFalse(comparer.Equals(null!, "pqr"));
            Assert.IsFalse(comparer.Equals("pqr", null!));

            Assert.AreEqual(true, comparer.Equals(null!, null!));
            Assert.IsFalse(comparer.Equals(null!, string.Empty));
            Assert.IsFalse(comparer.Equals(string.Empty, null!));
        }
        #endregion // Tests
    }
#pragma warning restore IDE0039
}

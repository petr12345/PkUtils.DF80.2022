// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.UnitTests.ExtensionsTests
{
    /// <summary>
    /// This is a test class for HashSetExtensions and is intended
    /// to contain all HashSetExtensionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HashSetExtensionTest
    {
        #region Tests

        /// <summary>
        ///A test for RemoveExisting
        ///</summary>
        [TestMethod()]
        public void HashSetExtension_RemoveExistingTest_01()
        {
            HashSet<int> hashSet = [];
            var listInts = Enumerable.Repeat(0, 5).Select((n, i) => i).ToList();

            listInts.ForEach(n => hashSet.Add(n));
            listInts.ForEach(n => hashSet.RemoveExisting(n));
        }

        [TestMethod()]
        public void HashSetExtension_RemoveExistingTest_02()
        {
            HashSet<int> hashSet = [];
            var listInts = Enumerable.Repeat(0, 5).Select((n, i) => i).ToList();

            listInts.ForEach(n => hashSet.Add(n));
            Assert.ThrowsExactly<ArgumentException>(() => hashSet.RemoveExisting(122));
        }

        [TestMethod()]
        public void HashSetExtension_AddNewTest_01()
        {
            HashSet<int> hashSet = [];
            var listInts = Enumerable.Repeat(0, 5).Select((n, i) => i).ToList();

            listInts.ForEach(n => hashSet.AddNew(n));
        }

        [TestMethod()]
        public void HashSetExtension_AddNewTest_02()
        {
            HashSet<int> hashSet = [];
            var listInts = Enumerable.Repeat(0, 5).ToList();

            Assert.ThrowsExactly<ArgumentException>(() =>
            {
                listInts.ForEach(n => hashSet.AddNew(n));
            });
        }
        #endregion // Tests

    }
}

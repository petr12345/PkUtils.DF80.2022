// Ignore Spelling: Utils
//
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.UnitTests.ExtensionsTests
{
    /// <summary>
    /// This is a test class for KeyedCollectionExtensions and is intended
    /// to contain all KeyedCollectionExtensionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class KeyedCollectionExtensionTest
    {
        #region Auxiliary_types

        public class MyIntKeyedCollection : KeyedCollection<int, int>
        {
            protected override int GetKeyForItem(int item)
            {
                return item + 77;
            }
        }
        #endregion // Auxiliary_types

        #region Test_Helpers

        /// <summary>
        /// A helper method for test of AddNew
        ///</summary>
        internal void AddNewTestHelper<TKey, TValue>(
          KeyedCollection<TKey, TValue> keyedCollection,
          TValue item) where TKey : notnull
        {
            KeyedCollectionExtensions.AddNew<TKey, TValue>(keyedCollection, item);
        }
        #endregion // Test_Helpers

        #region Tests

        /// <summary>
        /// A test for AddNew
        /// </summary>
        [TestMethod()]
        public void KeyedCollection_AddNewTest_01()
        {
            MyIntKeyedCollection coll = [];
            var listInts = Enumerable.Repeat(0, 5).Select((n, i) => i).ToList();

            listInts.ForEach(k => AddNewTestHelper(coll, k));
        }

        /// <summary>
        /// A test for AddNew
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void KeyedCollection_AddNewTest_02()
        {
            MyIntKeyedCollection coll = [];
            Enumerable.Repeat(0, 5).ToList().ForEach(k => AddNewTestHelper(coll, k));
        }
        #endregion // Tests

    }
}

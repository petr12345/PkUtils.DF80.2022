// Ignore Spelling: Utils
//

using System.Collections.ObjectModel;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.NUnitTests.ExtensionsTests
{
    /// <summary>
    /// This is a test class for <see cref="KeyedCollectionExtensions"/>
    ///</summary>
    [TestFixture()]
    public class KeyedCollectionExtensionsTest
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
        private static void AddNewTestHelper<TKey, TValue>(
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
        [Test()]
        public void KeyedCollection_AddNewTest_01()
        {
            MyIntKeyedCollection coll = [];
            var listInts = Enumerable.Repeat(0, 5).Select((n, i) => i).ToList();

            listInts.ForEach(k => AddNewTestHelper(coll, k));
        }

        /// <summary> A test for AddNew. </summary>
        [Test()]
        public void KeyedCollection_AddNewTest_02()
        {
            MyIntKeyedCollection coll = [];
            Assert.Throws<ArgumentException>(() =>
                Enumerable.Repeat(0, 5).ToList().ForEach(k => AddNewTestHelper(coll, k)));
        }
        #endregion // Tests
    }
}

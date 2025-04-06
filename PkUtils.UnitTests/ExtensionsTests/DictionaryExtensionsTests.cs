// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.UnitTests.ExtensionsTests
{
    [TestClass()]
    public class DictionaryExtensionsTests
    {
        #region Auxiliary_methods

        /// <summary> A helper method for test of GetValueOrDefault </summary>
        internal void ValueOrDefaultTestHelper<TKey, TValue>(
            IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue expected)
        {
            bool before = dictionary.ContainsKey(key);
            TValue actual = dictionary.ValueOrDefault<TKey, TValue>(key);
            bool after = dictionary.ContainsKey(key);

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(before, after);
        }

        /// <summary> A helper method for test of GetValueOrNew </summary>
        internal void GetValueOrNewTestHelper<TKey, TValue>(
            IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue expected) where TValue : new()
        {
            TValue actual = dictionary.GetValueOrNew<TKey, TValue>(key);
            bool after = dictionary.ContainsKey(key);

            Assert.AreEqual(expected, actual);
            Assert.IsTrue(after);
        }


        /// <summary> A helper method for test of AddNew.</summary>
        internal void AddNewTestHelper<TKey, TValue>(
          IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            dictionary.AddNew<TKey, TValue>(key, value);
            Assert.IsTrue(dictionary.ContainsKey(key));
        }

        /// <summary>
        /// A helper method for test of RemoveExisting
        /// </summary>
        internal void RemoveExistingTestHelper<TKey, TValue>(
          IDictionary<TKey, TValue> dictionary, TKey key)
        {
            DictionaryExtensions.RemoveExisting<TKey, TValue>(dictionary, key);
            Assert.IsFalse(dictionary.ContainsKey(key));
        }

        /// <summary>
        /// A helper for test for TryRemove
        /// </summary>
        internal static void TryRemoveTestHelper<TKey, TValue>(
          IDictionary<TKey, TValue> dictionary, TKey key)
        {
            bool expected = dictionary.ContainsKey(key);
            bool actual = DictionaryExtensions.TryRemove<TKey, TValue>(dictionary, key, out _);

            Assert.AreEqual(expected, actual);
            Assert.IsFalse(dictionary.ContainsKey(key));
        }
        #endregion // Auxiliary_methods

        #region Tests_IDictionary_extensions

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DictionaryExtension_GetValueOrDefaultTest_01()
        {
            IDictionary<int, int> dict = null!;

            dict.ValueOrDefault(5);
        }

        [TestMethod]
        public void DictionaryExtension_GetValueOrDefault_Test_02()
        {
            const int count = 6;
            IDictionary<int, int> dict = Enumerable.Repeat(0, count).Select((n, i) => i).ToDictionary(k => k);

            ValueOrDefaultTestHelper<int, int>(dict, 1, 1);
            ValueOrDefaultTestHelper<int, int>(dict, 3, 3);
            ValueOrDefaultTestHelper<int, int>(dict, 33, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DictionaryExtension_GetValueOrNew_Test_01()
        {
            IDictionary<int, int> dict = null!;

            dict.GetValueOrNew(5);
        }

        [TestMethod]
        public void DictionaryExtension_GetValueOrNew_Test_02()
        {
            const int count = 6;
            IDictionary<int, int> dict = Enumerable.Repeat(0, count).Select((n, i) => i).ToDictionary(k => k);

            GetValueOrNewTestHelper<int, int>(dict, 1, 1);
            GetValueOrNewTestHelper<int, int>(dict, 3, 3);
            GetValueOrNewTestHelper<int, int>(dict, 44, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DictionaryExtension_AddNewTest_01()
        {
            IDictionary<int, int> dict = null!;

            dict.AddNew(1, 1);
        }

        [TestMethod]
        public void DictionaryExtension_AddNewTest_02()
        {
            var couples = Enumerable.Repeat(0, 5).Select((n, index) => new { index, n }).ToList();
            IDictionary<int, int> dict = new Dictionary<int, int>();

            couples.SafeForEach(couple => AddNewTestHelper(dict, couple.index, couple.n));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DictionaryExtension_AddNewTest_03()
        {
            var couples = Enumerable.Repeat(0, 6).Select((n, index) => new { index, n }).ToList();
            IDictionary<int, int> dict = new Dictionary<int, int>();

            couples.SafeForEach(couple => AddNewTestHelper(dict, couple.n, couple.index));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DictionaryExtension_AddRange_Test_01()
        {
            IDictionary<int, string> dict_a = new Dictionary<int, string>();
            IDictionary<int, string> dict_b = null!;

            dict_a.AddRange(dict_b);
        }

        [TestMethod]
        public void DictionaryExtension_AddRange_Test_02()
        {
            IDictionary<int, string> dict_a = new Dictionary<int, string>();
            IDictionary<int, string> dict_b = new Dictionary<int, string> { { 4, "a" }, { 5, "b" }, { 6, "c" } };

            dict_a.AddRange(dict_b);
            Assert.IsTrue(dict_a.MemberwiseEqual(dict_b));
        }

        /// <summary>
        /// A test for RemoveExisting
        /// </summary>
        [TestMethod()]
        public void DictionaryExtension_RemoveExistingTest_01()
        {
            // fill dictionary
            const int count = 6;
            IDictionary<int, int> dict = new Dictionary<int, int>();
            var listInts = Enumerable.Repeat(0, count).Select((n, i) => i).ToList();
            listInts.ForEach(n => dict.Add(n, n));
            // test removing
            listInts.ForEach(n => RemoveExistingTestHelper<int, int>(dict, n));
        }

        /// <summary>
        /// A test for RemoveExisting
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void DictionaryExtension_RemoveExistingTest_02()
        {
            const int count = 6;
            IDictionary<int, int> dict = new Dictionary<int, int>();
            // fill dictionary
            Enumerable.Repeat(0, count).Select((n, i) => i).ToList().ForEach(k => dict.Add(k, k));
            // test removing
            RemoveExistingTestHelper<int, int>(dict, 100);
        }

        /// <summary>
        /// A test for TryRemove
        /// </summary>
        [TestMethod()]
        public void DictionaryExtension_TryRemoveTest_01()
        {
            // fill dictionary
            const int count = 6;
            IDictionary<int, int> dict = new Dictionary<int, int>();
            var listInts = Enumerable.Repeat(0, count).Select((n, i) => i).ToList();
            listInts.ForEach(n => dict.Add(n, n));
            // test TryRemove
            listInts.ForEach(n => TryRemoveTestHelper<int, int>(dict, n));
        }

        /// <summary>
        /// A test for TryRemove
        /// </summary>
        [TestMethod()]
        public void DictionaryExtension_TryRemoveTest_02()
        {
            // fill dictionary
            const int count = 6;
            IDictionary<int, int> dict = new Dictionary<int, int>();
            var listIntsA = Enumerable.Repeat(0, count).Select((n, i) => i).ToList();
            var listIntsB = Enumerable.Repeat(0, count).Select((n, i) => count + i).ToList();

            listIntsA.ForEach(n => dict.Add(n, n));
            // test TryRemove
            listIntsB.ForEach(n => TryRemoveTestHelper<int, int>(dict, n));
            listIntsA.ForEach(n => TryRemoveTestHelper<int, int>(dict, n));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DictionaryExtension_ToStringEx_Test_01()
        {
            IDictionary<int, int> dict = null!;

            dict.ToStringEx();
        }

        [TestMethod]
        public void DictionaryExtension_ToStringEx_Test_02()
        {
            IDictionary<int, string> dict = new Dictionary<int, string> { { 4, "a" }, { 5, "b" } };
            string expected = "{4=a,5=b}";
            string actual = dict.ToStringEx();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DictionaryExtension_CompareDictionary_Test_01()
        {
            IDictionary<int, int> dict = null!;

            dict.MemberwiseEqual(new Dictionary<int, int>());
        }

        [TestMethod]
        public void DictionaryExtension_CompareDictionary_Test_02()
        {
            IDictionary<int, string> dict_0th = null!;
            IDictionary<int, string> dict_1st = new Dictionary<int, string> { { 4, "a" }, { 5, "b" } };
            IDictionary<int, string> dict_2nd = new Dictionary<int, string> { { 4, "A" }, { 5, "B" } };
            IDictionary<int, string> dict_3rd = new Dictionary<int, string> { { 4, "a" }, { 5, "b" }, { 6, "c" } };

            Assert.IsFalse(dict_1st.MemberwiseEqual(dict_0th));
            Assert.IsFalse(dict_1st.MemberwiseEqual(dict_2nd));
            Assert.IsTrue(dict_1st.MemberwiseEqual(dict_2nd, StringComparer.InvariantCultureIgnoreCase));
            Assert.IsFalse(dict_1st.MemberwiseEqual(dict_3rd, StringComparer.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DictionaryExtension_DictionaryHashCode_Test_01()
        {
            IDictionary<int, int> dict = null!;

            dict.DictionaryHashCode();
        }

        [TestMethod]
        public void DictionaryExtension_DictionaryHashCode_Test_02()
        {
            IDictionary<int, string> dict_1st = new Dictionary<int, string> { { 4, "a" }, { 5, "b" } };
            IDictionary<int, string> dict_2nd = new Dictionary<int, string> { { 5, "b" }, { 4, "a" } };
            IDictionary<int, string> dict_3rd = new Dictionary<int, string> { { 4, null! }, { 5, null! }, { 6, null! } };
            IDictionary<int, string> dict_4th = new Dictionary<int, string> { { 6, null! }, { 5, null! }, { 4, null! } };

            int hash1st = dict_1st.DictionaryHashCode();
            int hash2nd = dict_2nd.DictionaryHashCode();
            int hash3rd = dict_3rd.DictionaryHashCode();
            int hash4th = dict_4th.DictionaryHashCode();

            Assert.IsTrue(hash1st == hash2nd);
            Assert.IsTrue(hash3rd == hash4th);
        }
        #endregion // Tests_IDictionary_extensions

        #region Tests_IReadOnlyDictionary_extensions

        // #FIX# - To Do

        #endregion // Tests_IReadOnlyDictionary_extensions
    }
}
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
        /// This is a test class for EnumerableExtensions and is intended
        /// to contain all EnumerableExtensionTest Unit Tests
        /// </summary>
        [TestClass()]
        public class EnumerableExtensionTest
        {
            #region Auxiliary_methods

            /// <summary>
            /// A helper for test of IsNullorEmpty
            /// </summary>
            public void IsNullOrEmptyTestHelper<T>(IEnumerable<T> source, bool expected)
            {
                bool actual = EnumerableExtensions.IsNullOrEmpty<T>(source);
                Assert.AreEqual(expected, actual);
            }

            /// <summary>
            /// A helper for test of IsEmpty
            /// </summary>
            internal void IsEmptyTestHelper<T>(IEnumerable<T> source, bool expected)
            {
                bool actual = EnumerableExtensions.IsEmpty<T>(source);
                Assert.AreEqual(expected, actual);
            }

            /// <summary>
            /// A helper for test of IndexOf
            /// </summary>
            internal void IndexOfTest1Helper<T>(IEnumerable<T> source, T val, int expected)
            {
                int actual = EnumerableExtensions.IndexOf<T>(source, val);
                Assert.AreEqual(expected, actual);
            }

            /// <summary>
            /// A helper for test of IndexOf
            /// </summary>
            internal void IndexOfTest2Helper<T>(IEnumerable<T> source, T val, IEqualityComparer<T> comparer, int expected)
            {
                int actual = EnumerableExtensions.IndexOf<T>(source, val, comparer);
                Assert.AreEqual(expected, actual);
            }
            #endregion // Auxiliary_methods

            #region Tests

            #region Tests_IsEmpty

            [TestMethod()]
            public void EnumerableExtension_IsEmpty_Test()
            {
                List<string> source = [];
                Assert.IsTrue(source.IsEmpty());
            }

            /// <summary>   A test for EnumerableExtensions.IsEmpty&lt;T&gt; </summary>
            [TestMethod()]
            public void EnumerableExtension_IsEmptyTest_01()
            {
                int[] arrInt1 = new int[] { 1, 2, 3 };
                IsEmptyTestHelper<int>(arrInt1, false);
            }

            /// <summary>   A test for EnumerableExtensions.IsEmpty&lt;T&gt; </summary>
            [TestMethod()]
            public void EnumerableExtension_IsEmptyTest_02()
            {
                int[] arrInt2 = new int[] { };
                IsEmptyTestHelper<int>(arrInt2, true);
            }

        /// <summary>   A test for EnumerableExtensions.IsEmpty&lt;T&gt; </summary>
        [TestMethod()]
        public void EnumerableExtension_IsEmptyTest_03()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => IsEmptyTestHelper<int>(null!, true));
        }
            #endregion // Tests_IsEmpty

            #region Tests_IsNullOrEmpty

            [TestMethod()]
            public void EnumerableExtension_IsNullOrEmptyTest_01()
            {
                int[] arrInt = new int[] { 11, 12 };
                IsEmptyTestHelper<int>(arrInt, false);
            }

            [TestMethod()]
            public void EnumerableExtension_IsNullOrEmptyTest_02()
            {
                IsNullOrEmptyTestHelper<int>(null!, true);
            }
            #endregion // Tests_IsNullOrEmpty

            #region Tests_IndexOf

            /// <summary>
            /// A test for IndexOf
            /// </summary>
            [TestMethod()]
            public void EnumerableExtension_IndexOfTest_01()
            {
                var listInts = Enumerable.Repeat(0, 5).Select((n, i) => i).ToList();
                IndexOfTest1Helper<int>(listInts, 1, 1);
            }

            /// <summary>
            /// A test for IndexOf
            /// </summary>
            [TestMethod()]
            public void EnumerableExtension_IndexOfTest_02()
            {
                var listInts = Enumerable.Repeat(0, 5).Select((n, i) => i).ToList();
                IndexOfTest2Helper<int>(listInts, 1, EqualityComparer<int>.Default, 1);
            }
            #endregion // Tests_IndexOf

            #region Tests_SafeForEach

            [TestMethod()]
            public void EnumerableExtension_SafeForEachTest()
            {
                const int count = 5;
                var couples = Enumerable.Repeat(0, count).Select((n, i) => new { i, n });
                IDictionary<int, int> dict = new Dictionary<int, int>();

                couples.SafeForEach(couple => dict.Add(couple.i, couple.n));
                Assert.IsTrue(dict.Count == count);
            }
            #endregion // Tests_SafeForEach

            #region Tests_Slice

            [TestMethod()]
        public void EnumerableExtension_SliceTest_01()
        {
            int[] source = null!;
            int startIndex = 2;
            int size = 3;
            Assert.ThrowsExactly<ArgumentNullException>(() => source.Slice(startIndex, size));
        }

        /// <summary>
        /// A test for Slice
        /// </summary>
        [TestMethod()]
        public void EnumerableExtension_SliceTest_02()
        {
            int[] source = { 1, 2, 3, 4, 5, 6 };
            int startIndex = -1;
            int size = 3;
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => source.Slice(startIndex, size));
        }

        /// <summary>
        /// A test for Slice
        /// </summary>
        [TestMethod()]
        public void EnumerableExtension_SliceTest_03()
        {
            int[] source = { 1, 2, 3, 4, 5, 6 };
            int startIndex = 1;
            int size = -1;
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => source.Slice(startIndex, size));
        }

            /// <summary>
            /// A test for Slice
            /// </summary>
            [TestMethod()]
            public void EnumerableExtension_SliceTest_04()
            {
                int[] source = { 1, 2, 3, 4, 5, 6 };
                int startIndex = 2;
                int size = 2;
                IEnumerable<int> actual = source.Slice(startIndex, size);
                IEnumerable<int> expected = new int[] { 3, 4 };

                Assert.IsTrue(expected.SequenceEqual(actual));
            }
        #endregion // Tests_Slice

        #region Tests_FindDuplicities

        /// <summary> A te        [TestMethod()]
        public void EnumerableExtension_FindDuplicitiesTest_01()
        {
            int[] source = null!;
            Assert.ThrowsExactly<ArgumentNullException>(() => source.FindDuplicities());
        }

            /// <summary> A test for FindDuplicities, which should succeed. </summary>
            [TestMethod()]
            public void EnumerableExtension_FindDuplicitiesTest_02()
            {
                int[] source = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                int[] actual_duplicities = source.FindDuplicities().ToArray();

                Assert.IsTrue(!actual_duplicities.Any());
            }

            /// <summary> A test for FindDuplicities, which should succeed. </summary>
            [TestMethod()]
            public void EnumerableExtension_FindDuplicitiesTest_03()
            {
                int[] source = { 1, 2, 3, 4, 2, 5, 6, 4, 7, 2 };
                int[] expected_duplicities = { 2, 4 };
                int[] actual_duplicities = source.FindDuplicities().ToArray();

                Assert.IsTrue(expected_duplicities.SequenceEqual(actual_duplicities));
            }
            #endregion // Tests_FindDuplicities

            #region Tests_CheckNotDuplicated

            /// <summary> A test for FindDuplicities, which should succeed. </summary>
            [TestMethod()]
            public void EnumerableExtension_CheckNotDuplicated_01()
            {
                int[] source = { 1, 2, 3, 4, 5, 6, 7, 100 };

                Assert.IsTrue(source.CheckNotDuplicated(nameof(source)).SequenceEqual(source));
            }

        /// <summary> A test for FindDuplicities, which should throw ArgumentOutOfRangeException. </summary>
        [TestMethod()]
        public void EnumerableExtension_CheckNotDuplicated_02()
        {
            int[] source = { 1, 2, 3, 4, 2, 5, 6, 4, 7, 2 };
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => source.CheckNotDuplicated(nameof(source), null, -1));
        }

        /// <summary> A test for FindDuplicities, which should throw ArgumentException. </summary>
        [TestMethod()]
        public void EnumerableExtension_CheckNotDuplicated_03()
        {
            int[] source = { 1, 1, 3, 3, 5, 5, 2, 2, 4, 4, };
            Assert.ThrowsExactly<ArgumentException>(() => source.CheckNotDuplicated(nameof(source), null, 3));
        }
            #endregion // Tests_CheckNotDuplicated

            // region Tests_TakeLast - not needed, Extension TakeLast is present in .NET 6
            // 
            // region Tests_SkipLast - not needed, Extension SkipLast is present in .NET 6
            // 

            #endregion // Tests
        }
    }

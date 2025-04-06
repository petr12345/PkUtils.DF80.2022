// Ignore Spelling: Utils, Comparers
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Comparers;
using PK.PkUtils.IO;

namespace PK.PkUtils.UnitTests.ComparersTest
{
#pragma warning disable IDE0039  // suppress "use local functions" warning

    /// <summary>
    /// This is a test class for FunctionalComparer generic
    ///</summary>
    [TestClass()]
    public class FunctionalComparerTest
    {
        #region Tests

        /// <summary>
        /// A test for FunctionalComparer constructor
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FunctionalComparer_Constructor_01()
        {
            new FunctionalComparer<int>(null);
        }

        /// <summary>
        /// A test for FunctionalComparer.Compare
        /// </summary>
        [TestMethod()]
        public void FunctionalComparer_CompareTest_01()
        {
            Comparison<string> f = (x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
            FunctionalComparer<string> comparer = new(f);

            Assert.AreEqual(0, comparer.Compare("aaa", "AAA"));
            Assert.AreNotEqual(0, comparer.Compare("aaa", "bbb"));
        }

        /// <summary>
        /// A test for FunctionalComparer.Compare
        /// </summary>
        [TestMethod()]
        public void FunctionalComparer_CompareTest_03()
        {
            Comparison<FileSystemInfo> fsComparer =
               (x, y) => string.Compare(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);

            Comparison<FileInfo> fileComparer =
              (x, y) => string.Compare(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);

            IComparer<FileSystemInfo> fsComp = new FunctionalComparer<FileSystemInfo>(fsComparer);
            IComparer<FileInfo> fiComp = new FunctionalComparer<FileInfo>(fileComparer);

            // The type argument of IComparer is contravariant.
            // Since FileSystemInfo is less derived than FileInfo (which inherits from it), 
            // one can assign:
            fiComp = fsComp;
            // but one cannot assign following:
            /* fsComp = fiComp;  */

            // assign fiComp again and test files sort
            fiComp = new FunctionalComparer<FileInfo>(fileComparer);

            var fsNonRecursive = new FileSearchNonRecursive();
            string strFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            DirectoryInfo folder = new(strFolder);
            var filesAll = fsNonRecursive.SearchFiles(folder, "*.exe", SearchOption.AllDirectories).ToList();

            var fsSorted = filesAll.OrderBy<FileSystemInfo, FileSystemInfo>(fs => fs, fsComp);
            var fiSorted = filesAll.OrderBy<FileInfo, FileInfo>(fi => fi, fiComp);

            Assert.AreEqual(true, fsSorted.SequenceEqual(fiSorted));
        }

        /// <summary>
        /// A test for FunctionalComparer.CreateNullSafeComparer
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FunctionalComparer_CreateNullSafeComparerTest_01()
        {
            Comparison<string> f = null!;
            FunctionalComparer.CreateNullSafeComparer(f);
        }

        /// <summary>
        /// A test for FunctionalComparer.CreateNullSafeComparer
        /// </summary>
        [TestMethod()]
        public void FunctionalComparer_CreateNullSafeComparerTest_02()
        {
            Comparison<string> f = (x, y) => (x.Length - y.Length);
            FunctionalComparer<string> comparer = FunctionalComparer.CreateNullSafeComparer(f);

            Assert.AreEqual(0, comparer.Compare("aaa", "AAA"));
            Assert.AreEqual(0, comparer.Compare("aaa", "XYZ"));

            Assert.IsTrue(0 > comparer.Compare(null!, "pqr"));
            Assert.IsTrue(0 < comparer.Compare("pqr", null!));

            Assert.AreEqual(0, comparer.Compare(null!, null!));
            Assert.IsTrue(0 > comparer.Compare(null!, string.Empty));
            Assert.IsTrue(0 < comparer.Compare(string.Empty, null!));
        }
        #endregion // Tests
    }
#pragma warning restore IDE0039
}

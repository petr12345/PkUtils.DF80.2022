
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using PK.PkUtils.Extensions;
using PK.PkUtils.IO;
using ParseErrorEventArgs = PK.PkUtils.IO.FileSearchBase.ParseErrorEventArgs;

namespace PK.TestGetFiles
{
    public class SearchTester
    {
        #region Fields

        /// <summary> The dump function, initialized by constructor. </summary>
        private readonly Action<string> _dumpFn;

        /// <summary> The list of non-accessible directories when searching non-recursively. </summary>
        private List<DirectoryInfo> _notAccessibleDirs_NonRec;

        /// <summary> The list of non-accessible directories when searching recursively. </summary>
        private List<DirectoryInfo> _notAccessibleDirs_Rec;
        #endregion // Fields

        #region Constructor(s)

        /// <summary> Constructor with one argument. </summary>
        /// <param name="dumpFn"> The function the will be used for dumping. </param>
        public SearchTester(Action<string> dumpFn)
        {
            dumpFn.CheckArgNotNull(nameof(dumpFn));
            _dumpFn = dumpFn;
        }
        #endregion // Constructor(s)

        #region Methods

        #region Dumping

        internal void Dump(string str)
        {
            _dumpFn(str);
        }

        internal void Dump(string format, object arg0)
        {
            Dump(string.Format(format, arg0));
        }

        internal void Dump(string format, object arg0, object arg1)
        {
            Dump(string.Format(format, arg0, arg1));
        }

        internal void Dump(string format, object arg0, object arg1, object arg2)
        {
            Dump(string.Format(format, arg0, arg1, arg2));
        }

        internal void Dump<T>(IEnumerable<T> seq, Func<T, string> fn)
        {
            foreach (var item in seq)
            {
                Dump(fn(item));
            }
        }

        internal void Dump<T>(IEnumerable<T> seq)
        {
            Dump<T>(seq, item => item.ToString());
        }

        /// <summary> Dump result comparison. </summary>
        /// <param name="itemsA"> The files a. </param>
        /// <param name="itemsB"> The files b. </param>
        /// <param name="strDescrA"> The description a. </param>
        /// <param name="strDescrB"> The description b. </param>
        internal bool SearchDumpResultComparison(
          IEnumerable<FileSystemInfo> itemsA,
          IEnumerable<FileSystemInfo> itemsB,
          string strDescrA,
          string strDescrB,
          string strItemName)
        {
            int countFilesA = itemsA.Count();
            int countFilesB = itemsB.Count();
            bool bOk = true;

            IEnumerable<string> sortedFilesA = itemsA.OrderBy(fi => fi.FullName).Select(fi => fi.FullName);
            IEnumerable<string> sortedFilesB = itemsB.OrderBy(fi => fi.FullName).Select(fi => fi.FullName);

            Dump("  Info: {0} {1} found: {2}", strDescrA, strItemName, countFilesA);
            Dump("  Info: {0} {1} found: {2}", strDescrB, strItemName, countFilesB);

            if (sortedFilesA.SequenceEqual(sortedFilesB))
            {
                Dump("------- OK, {0} and {1} search results DO match.", strDescrA, strDescrB);
            }
            else
            {
                Dump("------- ERROR, {0} and {1} results DO NOT match!", strDescrA, strDescrB);
                bOk = false;

                List<string> seqAminusB = sortedFilesA.Except(sortedFilesB).ToList();
                List<string> seqBminusA = sortedFilesB.Except(sortedFilesA).ToList();
                const int nFragmentCount = 5;

                if (seqAminusB.Count() == 0)
                { // A is subset of B
                    Dump("All results of {0} are contained in {1} results", strDescrA, strDescrB);
                }
                else if (seqBminusA.Count() == 0)
                { // B is subset of A
                    Dump("All results of {0} are contained in {1} results", strDescrB, strDescrA);
                }
                else
                { // neither
                    Dump("Some search results of {0} are not found in {1} and vice versa",
                      strDescrA, strDescrB);
                }

                if (seqAminusB.Count() > 0)
                {
                    string strMsg = string.Format(CultureInfo.InvariantCulture,
                      "Several items found by {0} but NOT present in {1}:", strDescrA, strDescrB);
                    var fragment = seqAminusB.Take(nFragmentCount).ToList();

                    Dump(strMsg);
                    Dump(fragment);
                }

                if (seqBminusA.Count() > 0)
                {
                    string strMsg = string.Format(CultureInfo.InvariantCulture,
                      "Several items NOT found by {0} but present in {1}:", strDescrA, strDescrB);
                    var fragment = seqBminusA.Take(nFragmentCount).ToList();

                    Dump(strMsg);
                    Dump(fragment);
                }
            }

            return bOk;
        }
        #endregion // Dumping

        #region Testing_search

        private void OnNonRecursiveParseError(
          object sender,
          FileSearchBase.ParseErrorEventArgs e)
        {
            string dirFullPath = e.DirInfo.FullName;
            string strInfo = string.Format(CultureInfo.InvariantCulture,
              "Problem reading directory '{0}' has occurred", dirFullPath);
            Dump(strInfo);

            // add to list
            _notAccessibleDirs_NonRec ??= [];
            _notAccessibleDirs_NonRec.Add(e.DirInfo);

            // e.Cancel = true; // un-comment to to try if test if cancellation works
        }

        private void OnRecursiveParseError(
          object sender,
          FileSearchBase.ParseErrorEventArgs e)
        {
            string dirFullPath = e.DirInfo.FullName;
            string strInfo = string.Format(CultureInfo.InvariantCulture,
              "Problem reading directory '{0}' has occurred", dirFullPath);
            Dump(strInfo);

            // add to list
            _notAccessibleDirs_Rec ??= [];
            _notAccessibleDirs_Rec.Add(e.DirInfo);

            // e.Cancel = true; // un-comment to to try if test if cancellation works
        }

        internal bool PerformFilesSearch(
          string strPath,
          string searchPattern,
          bool bDumpAccesError)
        {
            string strMethodA = "'Non-recursively'";
            string strMethodB = string.Empty;
            string strInfo = string.Empty;
            FileSearchBase fsNonRecursive = new FileSearchNonRecursive();
            FileSearchBase fsMethodB = null;
            DirectoryInfo diRoot = new(strPath);
            IEnumerable<FileInfo> filesA = null;
            IEnumerable<FileInfo> filesB = null;
            bool bResult;

            try
            {
                // 1. initialize remaining fields
                fsMethodB = new FileSearchRecursive();
                strMethodB = "'Recursively'";

                if (bDumpAccesError)
                {
                    fsNonRecursive.ParseErrorEvent += new EventHandler<ParseErrorEventArgs>(OnNonRecursiveParseError);
                    fsMethodB.ParseErrorEvent += new EventHandler<ParseErrorEventArgs>(OnRecursiveParseError);
                }
                // reset two output lists
                _notAccessibleDirs_NonRec = null;
                _notAccessibleDirs_Rec = null;

                // 2. search for files now

                // A/ Search for files non-recursively. Write to console found items
                strInfo = string.Format("--- A/ Searching for files {0}, dumping all found items --- ", strMethodA);
                Dump(strInfo);

                filesA = fsNonRecursive.SearchFiles(diRoot, searchPattern, SearchOption.AllDirectories);
                filesA = filesA.ToList();
                Dump(filesA, fi => fi.FullName);

                if (null != _notAccessibleDirs_NonRec)
                {
                    Dump("Not accessible directories encountered:");
                    Dump(_notAccessibleDirs_NonRec, di => di.FullName);
                }
                Dump("...");

                // B/ Search for files with the other method chosen. Don't write to console found items
                strInfo = string.Format("--- B/ Searching for files {0}, dumping found items --- ", strMethodB);
                Dump(strInfo);

                filesB = fsMethodB.SearchFiles(diRoot, searchPattern, SearchOption.AllDirectories);
                filesB = filesB.ToList();
                Dump(filesB, fi => fi.FullName);
                Dump("...");

                // C/ sort the results and compare what has been found
                // SearchTester tester = new SearchTester(str => Dump(str));
                bResult = this.SearchDumpResultComparison(
                  filesA.Cast<FileSystemInfo>(),
                  filesB.Cast<FileSystemInfo>(),
                  strMethodA,
                  strMethodB,
                  "files");
            }
            finally
            {
                // Final cleanup
                _notAccessibleDirs_NonRec = null;
                _notAccessibleDirs_Rec = null;
            }


            return bResult;
        }

        internal bool PerformFoldersSearch(
          string strPath,
          bool bDumpAccesError)
        {
            string strMethodA = "'Non-recursively'";
            string strMethodB = "'Recursively'";
            string strInfo = string.Empty;
            FileSearchBase fsNonRecursive = new FileSearchNonRecursive();
            FileSearchBase fsMethodB = new FileSearchRecursive();
            DirectoryInfo diRoot = new(strPath);
            IEnumerable<DirectoryInfo> dirsA = null;
            IEnumerable<DirectoryInfo> dirsB = null;
            bool bResult;

            try
            {
                if (bDumpAccesError)
                {
                    fsNonRecursive.ParseErrorEvent += new EventHandler<ParseErrorEventArgs>(OnNonRecursiveParseError);
                    fsMethodB.ParseErrorEvent += new EventHandler<ParseErrorEventArgs>(OnNonRecursiveParseError);
                }
                // reset two output lists
                _notAccessibleDirs_NonRec = null;
                _notAccessibleDirs_Rec = null;

                // 2. search for directories now

                // C/ Search for directories non-recursively. Write to console found items
                Dump("--- C/ Searching for directories {0}, dumping all found items --- ", strMethodA);

                dirsA = fsNonRecursive.SearchDirectories(diRoot, SearchOption.AllDirectories, true);
                dirsA = dirsA.ToList();
                Dump(dirsA, di => di.FullName);

                if (null != _notAccessibleDirs_NonRec)
                {
                    Dump("Not accessible directories encountered:");
                    Dump(_notAccessibleDirs_NonRec, di => di.FullName);
                }
                Dump("...");

                // D/ Search for directories with the other method chosen. Don't write to console found items
                Dump("--- D/ Searching for directories {0}, dumping found items --- ", strMethodB);

                dirsB = fsMethodB.SearchDirectories(diRoot, SearchOption.AllDirectories, true);
                dirsB = dirsB.ToList();
                Dump(dirsB, di => di.FullName);
                Dump("...");

                // C/ sort the results and compare what has been found
                bResult = this.SearchDumpResultComparison(
                  dirsA.Cast<FileSystemInfo>(),
                  dirsB.Cast<FileSystemInfo>(),
                  strMethodA,
                  strMethodB,
                  "directories");
            }
            finally
            {
                // Final cleanup
                _notAccessibleDirs_NonRec = null;
                _notAccessibleDirs_Rec = null;
            }

            return bResult;
        }
        #endregion // Testing_search

        #endregion // Methods
    }
}
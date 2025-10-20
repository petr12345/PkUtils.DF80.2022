// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.IO;

#nullable enable

namespace PK.PkUtils.UnitTests.IOTests;

/// <summary>
/// This is a test class for FileSearchNonRecursive and FileSearchNonRecursive.
/// </summary>
[TestClass()]
public class FileSearchTest
{
    /// <summary> The dump function (if any). </summary>
    private Action<string>? _dumpFn;

    #region Auxiliary_code_for_tests
    #region Dump

    internal void Dump(string str)
    {
        _dumpFn?.Invoke(str);
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
            Dump(fn!(item)!);
        }
    }

    internal void Dump<T>(IEnumerable<T> seq)
    {
        Dump<T>(seq, item => item!.ToString()!);
    }

    /// <summary> Dump result comparison. </summary>
    /// <param name="itemsA"> The files a. </param>
    /// <param name="itemsB"> The files b. </param>
    /// <param name="strDescrA"> The description a. </param>
    /// <param name="strDescrB"> The description b. </param>
    /// <param name="strItemName"> The item name. </param>
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

        IEnumerable<string> sortedFilesA = itemsA.Select(fi => fi.FullName).OrderBy(s => s);
        IEnumerable<string> sortedFilesB = itemsB.Select(fi => fi.FullName).OrderBy(s => s);

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

            if (seqAminusB.Count == 0)
            { // A is subset of B
                Dump("All results of {0} are contained in {1} results", strDescrA, strDescrB);
            }
            else if (seqBminusA.Count == 0)
            { // B is subset of A
                Dump("All results of {0} are contained in {1} results", strDescrB, strDescrA);
            }
            else
            { // neither
                Dump("Some search results of {0} are not found in {1} and vice versa",
                  strDescrA, strDescrB);
            }

            if (seqAminusB.Count > 0)
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
    #endregion // Dump

    #region Perfor_search

    /// <summary> Auxiliary method called from tests. Performs the files search action. </summary>
    /// <param name="strPathA"> The path a. </param>
    /// <param name="searchPatternA"> The search pattern a. </param>
    /// <param name="strPathB"> The path b. </param>
    /// <param name="searchPatternB"> The search pattern b. </param>
    /// <param name="searchOption"> The search option. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    internal bool PerformFilesSearch(
        string strPathA,
        string? searchPatternA,
        string strPathB,
        string? searchPatternB,
        SearchOption searchOption)
    {
        string strMethodA = "'Non-recursively'";
        string strMethodB = string.Empty;
        string strInfo = string.Empty;
        FileSearchBase fsNonRecursive = new FileSearchNonRecursive();
        FileSearchBase fsMethodB;
        DirectoryInfo diRootA = new(strPathA);
        DirectoryInfo diRootB = new(strPathB);
        IEnumerable<FileInfo> filesA;
        IEnumerable<FileInfo> filesB;
        bool bResult;

        fsMethodB = new FileSearchRecursive();
        strMethodB = "'Recursively'";

        // A/ Search for files non-recursively. Dump found items
        strInfo = string.Format("--- A/ Searching for files {0}, dumping all found items --- ", strMethodA);
        Dump(strInfo);

        filesA = fsNonRecursive.SearchFiles(diRootA, searchPatternA, searchOption);
        filesA = filesA.ToList();
        Dump(filesA, fi => fi.FullName);
        Dump("...");

        // B/ Search for files with the other method chosen. Don't dump found items
        strInfo = string.Format("--- B/ Searching for files {0}, dumping found items --- ", strMethodB);
        Dump(strInfo);

        filesB = fsMethodB.SearchFiles(diRootB, searchPatternB, searchOption);
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

        return bResult;
    }

    /// <summary> Auxiliary method called from tests. Performs the folders search action. </summary>
    /// <param name="strPath"> Full pathname of the search root folder. </param>
    /// <param name="searchOption"> The search option. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    internal bool PerformFoldersSearch(
        string strPath,
        SearchOption searchOption)
    {
        string strMethodA = "'Non-recursively'";
        string strMethodB = strMethodB = "'Recursively'";
        string strInfo = string.Empty;
        FileSearchBase fsNonRecursive = new FileSearchNonRecursive();
        FileSearchBase fsMethodB = new FileSearchRecursive();
        DirectoryInfo diRoot = new(strPath);
        IEnumerable<DirectoryInfo> dirsA;
        IEnumerable<DirectoryInfo> dirsB;
        bool bResult;

        // 1. initialize remaining fields
        /*
        if (bDumpAccesError)
        {
          fsNonRecursive.evParseError += new EventHandler<FileSearchBase.ParseErrorEventArgs>(OnParseError);
          if ((fsTempB = fsMethodB as FileSearchBase) != null)
            fsTempB.evParseError += new EventHandler<FileSearchBase.ParseErrorEventArgs>(OnParseError);
        }
        */

        // 2. search for directories now
        // a/ Search for directories non-recursively. Dump found items
        Dump("--- a/ Searching for directories {0}, dumping all found items --- ", strMethodA);

        dirsA = fsNonRecursive.SearchDirectories(diRoot, searchOption, true);
        dirsA = dirsA.ToList();
        Dump(dirsA, di => di.FullName);
        Dump("...");

        // b/ Search for directories with the other method chosen. Don't dump found items
        Dump("--- b/ Searching for directories {0}, dumping found items --- ", strMethodB);

        dirsB = fsMethodB.SearchDirectories(diRoot, searchOption, true);
        dirsB = dirsB.ToList();
        Dump(dirsB, di => di.FullName);
        Dump("...");

        // c/ sort the results and compare what has been found
        bResult = this.SearchDumpResultComparison(
          dirsA.Cast<FileSystemInfo>(),
          dirsB.Cast<FileSystemInfo>(),
          strMethodA,
          strMethodB,
          "directories");

        return bResult;
    }

    // Auxiliary method called from tests. Performs the files search action.
    internal bool PerformFilesSearchEx(
      Action<string> dumpFn,
      string strPathA,
      string? searchPatternA,
      string strPathB,
      string? searchPatternB,
      SearchOption searchOption)
    {
        bool bRes = false;

        try
        {
            _dumpFn = dumpFn;
            bRes = PerformFilesSearch(strPathA, searchPatternA, strPathB, searchPatternB, searchOption);
        }
        finally
        {
            _dumpFn = null;
        }
        return bRes;
    }

    // Auxiliary method called from tests. Performs the folders search action.
    internal bool PerformFoldersSearchEx(
      Action<string> dumpFn,
      string strPath,
      SearchOption searchOption)
    {
        bool bRes = false;

        try
        {
            _dumpFn = dumpFn;
            bRes = PerformFoldersSearch(strPath, searchOption);
        }
        finally
        {
            _dumpFn = null;
        }
        return bRes;
    }
    #endregion // Perfor_search
    #endregion // Auxiliary_code_for_tests

    #region Tests

    /// <summary> Unit Test Method for FileSearchNonRecursive.SearchFiles and FileSearchRecursive.SearchFiles. 
    /// </summary>
    [TestMethod()]
    public void FileSearch_SearchFilesTest_01()
    {
        Action<string> dumpFn = str => Debug.WriteLine(str);
        string strPathA = Environment.GetFolderPath(Environment.SpecialFolder.System);
        string searchPatternA = "*.dll";
        string strPathB = strPathA;
        string searchPatternB = searchPatternA;

        Assert.IsTrue(PerformFilesSearchEx(dumpFn, strPathA, searchPatternA, strPathB, searchPatternB,
          SearchOption.AllDirectories));
    }

    /// <summary> Unit Test Method for FileSearchNonRecursive.SearchFiles and FileSearchRecursive.SearchFiles. 
    /// </summary>
    [TestMethod()]
    public void FileSearch_SearchFilesTest_02()
    {
        Action<string> dumpFn = str => Debug.WriteLine(str);
        string strPathA = Environment.GetFolderPath(Environment.SpecialFolder.System);
        string searchPatternA = "*.*";
        string strPathB = FilePathHelper.AppendPathSeparator(strPathA);
        string? searchPatternB = null;

        Assert.IsTrue(PerformFilesSearchEx(dumpFn, strPathA, searchPatternA, strPathB, searchPatternB,
          SearchOption.AllDirectories));
    }

    /// <summary> Unit Test Method for FileSearchNonRecursive.Directories and FileSearchRecursive.Directories.
    /// </summary>
    [TestMethod()]
    public void FileSearch_SearchDirectoriesTest_01()
    {
        Action<string> dumpFn = str => Debug.WriteLine(str);
        string strPath = Environment.GetFolderPath(Environment.SpecialFolder.System);

        Assert.IsTrue(PerformFoldersSearchEx(dumpFn, strPath, SearchOption.AllDirectories));
    }

    /// <summary> Unit Test Method for FileSearchNonRecursive.Directories and FileSearchRecursive.Directories.
    /// </summary>
    [TestMethod()]
    public void FileSearch_SearchDirectoriesTest_02()
    {
        Action<string> dumpFn = str => Debug.WriteLine(str);
        string strPath = Environment.GetFolderPath(Environment.SpecialFolder.System);

        Assert.IsTrue(PerformFoldersSearchEx(dumpFn, strPath, SearchOption.TopDirectoryOnly));
    }
    #endregion // Tests
}

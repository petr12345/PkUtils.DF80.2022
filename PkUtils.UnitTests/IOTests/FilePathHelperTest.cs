// Ignore Spelling: Utils, Subfolder
// 
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.IO;

#pragma warning disable IDE0057 // Use range operator

namespace PK.PkUtils.UnitTests.IOTests;

/// <summary>
/// This is a test class for FilePathHelper and is intended
/// to contain all FilePathHelperTest Unit Tests
/// </summary>
[TestClass()]
public class FilePathHelperTest
{
    private const string _320CharsAbsolutePath = @"c:\Tools\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\M602\uvwxyz";
    private const string _317CharsRelativePath = @"Tools\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\M602\uvwxyz";

    #region Tests

    #region HasWildCardTest

    /// <summary>
    /// A test for HasWildCard which should succeed
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_HasWildCardTest_01()
    {
        Assert.AreEqual(false, FilePathHelper.HasWildCard(_320CharsAbsolutePath));
    }

    /// <summary>
    /// A test for HasWildCard which should succeed
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_HasWildCardTest_02()
    {
        Assert.AreEqual(false, FilePathHelper.HasWildCard(null));
        Assert.AreEqual(false, FilePathHelper.HasWildCard(""));
        Assert.AreEqual(false, FilePathHelper.HasWildCard("abc"));
        Assert.AreEqual(false, FilePathHelper.HasWildCard("\\//"));
        Assert.AreEqual(true, FilePathHelper.HasWildCard("?"));
        Assert.AreEqual(true, FilePathHelper.HasWildCard("aa*c"));
        Assert.AreEqual(true, FilePathHelper.HasWildCard("aa?c"));
    }
    #endregion // HasWildCardTest

    #region AppendPathSeparatorTest

    /// <summary>
    /// A test for AppendPathSeparator which should succeed
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_AppendPathSeparatorTest_01()
    {
        Assert.AreEqual(Path.DirectorySeparatorChar, FilePathHelper.AppendPathSeparator(_320CharsAbsolutePath).Last());
    }

    /// <summary>
    /// A test for AppendPathSeparator which should succeed
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_AppendPathSeparatorTest_02()
    {
        string[,] arrPath_and_Expected = new string[,] {
    { @"",                 @""},
    { @"\",                @"\"},
    { @"/",                @"/"},
    { @":",                @":"},
    { @"c:",               @"c:"},
    { @"c:\",              @"c:\"},
    { @"c:\Tools\M602",    @"c:\Tools\M602\"},
    { @"c:\Tools\M602\",   @"c:\Tools\M602\"},
    { @"c:\Tools\M602/",   @"c:\Tools\M602/"},
  };

        int nDim0 = arrPath_and_Expected.GetLength(0);
        for (int ii = 0; ii < nDim0; ii++)
        {
            string strPath = arrPath_and_Expected[ii, 0];
            string expected = arrPath_and_Expected[ii, 1];
            string actual;

            actual = FilePathHelper.AppendPathSeparator(strPath);
            Assert.AreEqual(expected, actual);
        }
    }
    #endregion // AppendPathSeparatorTest

    #region RemovePathSeparatorTest

    /// <summary>
    /// A test for TrimEndingDirectorySeparator which should succeed
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_RemovePathSeparatorTest_01()
    {
        string expected = _320CharsAbsolutePath;
        string actual = FilePathHelper.TrimEndingDirectorySeparator(FilePathHelper.AppendPathSeparator(_320CharsAbsolutePath));
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// A test for TrimEndingDirectorySeparator which should succeed
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_RemovePathSeparatorTest_02()
    {
        string[,] arrPath_and_Expected = new string[,] {
    { @"",                 @""},
    { @"\",                @"\"},
    { @"/",                @"/"},
    { @":",                @":"},
    { @"c:",               @"c:"},
    { @"c:\",              @"c:\"},
    { @"c:\Tools\M602",    @"c:\Tools\M602"},
    { @"c:\Tools\M602\",   @"c:\Tools\M602"},
    { @"c:\Tools\M602/",   @"c:\Tools\M602"},
  };

        int nDim0 = arrPath_and_Expected.GetLength(0);
        for (int ii = 0; ii < nDim0; ii++)
        {
            string strPath = arrPath_and_Expected[ii, 0];
            string expected = arrPath_and_Expected[ii, 1];
            string actual;

            actual = FilePathHelper.TrimEndingDirectorySeparator(strPath);
            Assert.AreEqual(expected, actual);
        }
    }
    #endregion // RemovePathSeparatorTest

    #region TrimPathDoubleQuotesTest

    /// <summary>
    /// A test for TrimPathDoubleQuotes which should succeed
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_TrimPathDoubleQuotesTest_01()
    {
        string strPath = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", _320CharsAbsolutePath);
        string expected = _320CharsAbsolutePath;
        string actual = FilePathHelper.TrimPathDoubleQuotes(strPath, false);

        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// A test for TrimPathDoubleQuotes for argument value trimSpaces = false
    /// </summary>
    [TestMethod()]
    [DataRow("e:\\Tools\\", "e:\\Tools\\")]
    [DataRow("\"e:\\Tools\\", "\"e:\\Tools\\")]
    [DataRow("\"e:\\Tools\\\"", "e:\\Tools\\")]
    [DataRow("\"e:\\Tools\"", "e:\\Tools")]
    [DataRow("\"e:\"", "e:")]
    [DataRow("\"e:\\Fox dummy\"", "e:\\Fox dummy")]
    [DataRow("  \"e:\\Tools\"  ", "  \"e:\\Tools\"  ")]
    [DataRow("  \"e:\"  ", "  \"e:\"  ")]
    [DataRow("\"\"", "")]
    [DataRow("\"\"\"", "\"")]
    [DataRow("abc", "abc")]
    [DataRow("\"", "\"")]
    [DataRow("", "")]
    [DataRow(null, null)] // Null case
    public void FilePathHelper_TrimPathDoubleQuotesTest_02(string strPathOriginal, string resultExpected)
    {
        const bool trimTrailingSpacesFalse = false;
        string resultActual = FilePathHelper.TrimPathDoubleQuotes(strPathOriginal, trimTrailingSpacesFalse);
        Assert.AreEqual(resultExpected, resultActual);
    }

    /// <summary>
    /// A test for TrimPathDoubleQuotes for argument value trimSpaces = true
    /// </summary>
    [TestMethod()]
    [DataRow("d:\\Tools\\", "d:\\Tools\\")]
    [DataRow("\"d:\\Tools\\", "\"d:\\Tools\\")]
    [DataRow("\"d:\\Tools\\\"", "d:\\Tools\\")]
    [DataRow("\"d:\\Tools\"", "d:\\Tools")]
    [DataRow("\"d:\"", "d:")]
    [DataRow("\"d:\\Fox dummy\"", "d:\\Fox dummy")]
    [DataRow("  \"d:\\Tools\"  ", "d:\\Tools")]
    [DataRow("  \"d:\"  ", "d:")]
    [DataRow("  d:      ", "  d:      ")]
    [DataRow("\"\"", "")]
    [DataRow("\"\"\"", "\"")]
    [DataRow("abc", "abc")]
    [DataRow("\"", "\"")]
    [DataRow("", "")]
    [DataRow(null, null)]
    public void FilePathHelper_TrimPathDoubleQuotesTest03(string strPathOriginal, string resultExpected)
    {
        const bool trimTrailingSpacesTrue = true;
        string resultActual = FilePathHelper.TrimPathDoubleQuotes(strPathOriginal, trimTrailingSpacesTrue);
        Assert.AreEqual(resultExpected, resultActual);
    }

    #endregion // TrimPathDoubleQuotesTest

    #region IsPathRootOnlyTest

    /// <summary>
    /// A test for IsPathRootOnly which should succeed.
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_IsPathRootOnlyTest_01()
    {
        Assert.AreEqual(false, FilePathHelper.IsPathRootOnly(_320CharsAbsolutePath, false));
    }

    /// <summary>
    /// A test for IsPathRootOnly which should succeed.
    /// Is tested always with the second argument bool acceptDriveLetterColon = false.
    /// </summary>
    [TestMethod]
    [DataRow(@"\", true)]
    [DataRow(@"/", true)]
    [DataRow(@"c:\", true)]
    [DataRow(@"c:/", true)]
    [DataRow(null, false)]
    [DataRow(@"", false)]
    [DataRow(@":", false)]
    [DataRow(@":\", false)]
    [DataRow(@":/", false)]
    [DataRow(@"c:", false)]
    [DataRow(@"c:\Tools\M602", false)]
    [DataRow(@"c:\Tools\M602\", false)]
    [DataRow(@"c:\Tools\M602/", false)]
    public void FilePathHelper_IsPathRootOnlyTest_02(string strPath, bool expected)
    {
        bool actual = FilePathHelper.IsPathRootOnly(strPath, false);

        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// A test for IsPathRootOnly which should succeed.
    /// Is tested always with the second argument bool acceptDriveLetterColon = true.
    /// </summary>
    [TestMethod]
    [DataRow(@"c:", true)]
    [DataRow(@"D:", true)]
    [DataRow(@"?:", false)]
    [DataRow(@"::", false)]
    [DataRow(@":", false)]
    public void FilePathHelper_IsPathRootOnlyTest_03(string strPath, bool expected)
    {
        bool actual = FilePathHelper.IsPathRootOnly(strPath, true);

        Assert.AreEqual(expected, actual);
    }
    #endregion // IsPathRootOnlyTest

    #region IsPathRootedTest

    /// <summary>
    /// A test for IsPathRooted which should succeed.
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_IsPathRootedTest_01()
    {
        bool expected = true;
        bool actual = FilePathHelper.IsPathRooted(_320CharsAbsolutePath, false);

        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// A test for IsPathRooted which should succeed.
    /// Is tested always with the second argument bool acceptDriveLetterColon = false.
    /// </summary>
    [TestMethod()]
    [DataRow(null, false)]
    [DataRow(@"\", true)]
    [DataRow(@"/", true)]
    [DataRow(@"\\", true)]
    [DataRow(@"\/", true)]
    [DataRow(@"//", true)]
    [DataRow(@"/\", true)]
    [DataRow(@"\T", true)]
    [DataRow(@"/T", true)]
    [DataRow(@"\ ", true)]
    [DataRow(@"\ \", true)]
    [DataRow(@"\1", true)]
    [DataRow(@"\1\", true)]
    [DataRow(@"\~", true)]
    [DataRow(@"\~\", true)]
    [DataRow(@"\š", true)]
    [DataRow(@"\š\", true)]
    [DataRow(@"\šašek šišlavý", true)]
    [DataRow(@"\šašek šišlavý\", true)]
    [DataRow(@"\TEMP\t.txt", true)]
    [DataRow(@"/TEMP/t.txt", true)]
    [DataRow(@"c:\", true)]
    [DataRow(@"c:/", true)]
    [DataRow(@"c:\Tools\M602", true)]
    [DataRow(@"c:\Tools\M602\", true)]
    [DataRow(@"c:\Tools\M602/", true)]
    [DataRow(@"\\\\", true)]
    [DataRow(@"", false)]
    [DataRow(@":", false)]
    [DataRow(@":\", false)]
    [DataRow(@":/", false)]
    [DataRow(@"c:", false)]
    [DataRow(@"\<", false)]
    [DataRow(@"\>", false)]
    [DataRow(@"\|", false)]
    public void FilePathHelper_IsPathRootedTest_02(string strPath, bool expected)
    {
        string strMsg;
        bool actual = FilePathHelper.IsPathRooted(strPath, false);

        // 1. check that expected and actual value match
        strMsg = string.Format(CultureInfo.InvariantCulture,
            "For the case of path '{0}', expected value is: {1}, actual value is: {2}",
            strPath, expected, actual);
        Assert.AreEqual(expected, actual, strMsg);

        // 2. check that except the tested value @"c:", 
        //    the result either match the value returned by System.IO.Path.IsPathRooted,
        //    or is false in case System.IO.Path.IsPathRooted throws ArgumentEception
        if ((!string.IsNullOrEmpty(strPath)) &&
            (!string.Equals(strPath, @"c:", StringComparison.OrdinalIgnoreCase)) &&
            (0 > strPath.IndexOfAny(FilePathHelper.PathInvalidOrWildCharacters.ToArray())))
        {
            bool bActual;
            bool bExpected = false;
            bool bException = false;

            try
            {
                bExpected = System.IO.Path.IsPathRooted(strPath);
            }
            catch (ArgumentException)
            {
                bException = true;
            }
            bActual = FilePathHelper.IsPathRooted(strPath, false);

            if (bException)
            {
                strMsg = string.Format(CultureInfo.InvariantCulture,
                    "For the case of path '{0}', value expected from FilePathHelper.IsPathRooted: {1}, actual value from FilePathHelper.IsPathRooted: {2}",
                    strPath, bExpected, bActual);
            }
            else
            {
                strMsg = string.Format(CultureInfo.InvariantCulture,
                    "For the case of path '{0}', value returned by Path.IsPathRooted: {1}, actual value from FilePathHelper.IsPathRooted: {2}",
                    strPath, bExpected, bActual);
            }
            Assert.AreEqual(bExpected, bActual, strMsg);
        }
    }

    /// <summary>
    /// A test for IsPathRooted which should succeed.
    /// Is tested always with the second argument bool acceptDriveLetterColon = true.
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_IsPathRootedTest_03()
    {
        string[,] arrPath_and_Expected = new string[,] {
            { @"c:",               "true"},
            { @"D:",               "true"},
            { @"?:",               "false"},
            { @"::",               "false"},
            { @":",                "false"},
        };

        int nDim0 = arrPath_and_Expected.GetLength(0);
        for (int ii = 0; ii < nDim0; ii++)
        {
            string strPath = arrPath_and_Expected[ii, 0];
            string expected = arrPath_and_Expected[ii, 1];
            string actual = FilePathHelper.IsPathRooted(strPath, true).ToString();

            Assert.IsTrue(string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase));
        }
    }
    #endregion // IsPathRootedTest

    #region CheckIsValidFolderStringTest

    /// <summary>
    /// A test for CheckIsValidFolderString which should succeed.
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_CheckIsValidFolderStringTest_01()
    {
        FilePathHelper.CheckIsValidFolderString(_320CharsAbsolutePath);
    }

    /// <summary>
    /// A test for CheckIsValidFolderString which should succeed.
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_CheckIsValidFolderStringTest_02()
    {
        string[] arrPath = new string[] {
         @"\",
        @"/",
        @"c:\",
        @"c:/",
        @":\",
        @":/",
        @"c:",
        @"c:\Tools\M602",
        @"c:\Tools\M602\",
        @"c:\Tools\M602/",
        };

        int nDim0 = arrPath.GetLength(0);
        for (int ii = 0; ii < nDim0; ii++)
        {
            string strPath = arrPath[ii];
            FilePathHelper.CheckIsValidFolderString(strPath);
        }
    }

    /// <summary>
    /// A test for CheckIsValidFolderString which should fail with exception ArgumentNullException.
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_CheckIsValidFolderStringTest_03()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => FilePathHelper.CheckIsValidFolderString(null));
    }

    /// <summary>
    /// A test for CheckIsValidFolderString which should fail with ArgumentException
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_CheckIsValidFolderStringTest_04()
    {
        Assert.ThrowsExactly<ArgumentException>(() => FilePathHelper.CheckIsValidFolderString(string.Empty));
    }


    /// <summary>
    /// A test for CheckIsValidFolderString which should fail with ArgumentException
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_CheckIsValidFolderStringTest_05()
    {
        string strArg = string.Join(" ", Path.GetInvalidPathChars());
        Assert.ThrowsExactly<ArgumentException>(() => FilePathHelper.CheckIsValidFolderString(strArg));
    }

    /// <summary>
    /// A test for CheckIsValidFolderString which should succeed
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_CheckIsValidFolderStringTest_06()
    {
        FilePathHelper.CheckIsValidFolderString(Environment.GetFolderPath(Environment.SpecialFolder.System));
        FilePathHelper.CheckIsValidFolderString(Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms));
    }
    #endregion // CheckIsValidFolderStringTest

    #region GetUpperFolderTest

    /// <summary>
    /// A test for GetUpperFolder which should succeed
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_GetUpperFolderTest_01()
    {
        FilePathHelper.GetUpperFolder(_320CharsAbsolutePath, false);
    }

    /// <summary>
    /// A test for GetUpperFolder which should succeed
    /// </summary>
    [TestMethod()]
    [DataRow(@"e:\Tools\LinkRank\", @"e:\Tools\", false)]
    [DataRow(@"e:\Tools\LinkRank", @"e:\Tools\", false)]
    [DataRow(@"e:\Tools\", @"e:\", false)]
    [DataRow(@"e:\", @"e:\", false)]
    [DataRow(@"\Tools\", @"\", false)]
    [DataRow(@"\", @"\", false)]
    [DataRow(@"", @"", false)]
    // pairs of { original path, expected result } for case allowEmpty = true
    [DataRow(@"c:\Tools\LinkRank\", @"c:\Tools\", true)]
    [DataRow(@"c:\Tools\LinkRank", @"c:\Tools\", true)]
    [DataRow(@"c:\Tools\", @"c:\", true)]
    [DataRow(@"c:\", @"", true)]
    [DataRow(@"\Tools\", @"\", true)]
    [DataRow(@"\", @"", true)]
    [DataRow(@"", @"", true)]
    public void FilePathHelper_GetUpperFolderWorks_Test(string strPathOriginal, string resultExpected, bool allowEmpty)
    {
        string resultActual = FilePathHelper.GetUpperFolder(strPathOriginal, allowEmpty);
        Assert.AreEqual(resultExpected, resultActual);
    }
    #endregion // GetUpperFolderTest

    #region GenerateAbsoluteTest

    [TestMethod()]
    public void FilePathHelper_GenerateAbsolute_Succeeds_Test()
    {
        string basePath = _317CharsRelativePath;
        string resultActual = FilePathHelper.GenerateAbsolute(basePath);

        Assert.IsTrue(resultActual.EndsWith(basePath));
    }
    #endregion // GenerateAbsoluteTest

    #region CombineAbsoluteTest

    /// <summary>
    /// A test for CombineAbsolute which should succeed
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_CombineAbsoluteTest_01()
    {
        string basePath = _320CharsAbsolutePath;
        string relativePath = @"123456789";
        string expected = FilePathHelper.AppendPathSeparator(_320CharsAbsolutePath) + "123456789";
        string actual = FilePathHelper.CombineAbsolute(basePath, relativePath);
        Assert.AreEqual(expected, actual);

        basePath = @"c:\";
        relativePath = _320CharsAbsolutePath.Substring(basePath.Length);
        expected = _320CharsAbsolutePath;
        actual = FilePathHelper.CombineAbsolute(basePath, relativePath);
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// A test for CombineAbsolute which should succeed
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_CombineAbsoluteTest_02()
    {
        string[,] arrBased_Relative_Expected = new string[,] {

    { @"c:\Tools\M602",      @"config",      @"c:\Tools\M602\config" },
    { @"c:\Tools\M602\",     @"config",      @"c:\Tools\M602\config" },
    { @"c:\Tools\M602\",     @"config/",     @"c:\Tools\M602\config\" },

    { @"c:\Tools\M602\",     @"config",      @"c:\Tools\M602\config" },
    { @"c:\Tools\M602\",     @"config\",     @"c:\Tools\M602\config\" },
    { @"c:\Tools\M602\",     @".\config\",   @"c:\Tools\M602\config\" },

    { @"c:\Tools\M602",     @"../config",    @"c:\Tools\config" },
    { @"c:\Tools\M602\",    @"../config",    @"c:\Tools\config" },
    { @"c:\Tools\M602\",    @".\..\config",  @"c:\Tools\config" },

    { @"c:\Tools\M602",      @"config\X",    @"c:\Tools\M602\config\X" },
    { @"c:\Tools\M602\",     @"config\X",    @"c:\Tools\M602\config\X" },
    { @"c:\Tools\M602\",     @"config\X\",   @"c:\Tools\M602\config\X\"},

    { @"c:\Tools\M602\",     @"\config\X",   @"\config\X" }, 
    /* wrong
         { @"c:\Tools\M602\",     @"config\X\..", @"c:\Tools\config" }, 
     */
  };

        int nDim0 = arrBased_Relative_Expected.GetLength(0);
        for (int ii = 0; ii < nDim0; ii++)
        {
            string basePath = arrBased_Relative_Expected[ii, 0];
            string relativePath = arrBased_Relative_Expected[ii, 1];
            string expected = arrBased_Relative_Expected[ii, 2];
            string actual;

            actual = FilePathHelper.CombineAbsolute(basePath, relativePath);
            Assert.AreEqual(expected, actual);
        }
    }

    /// <summary>
    /// A test for CombineAbsolute compared with Path.Combine.
    /// It demonstrates that Path.Combine(@"c:\Tools\M602\", @"config/") returns @"c:\Tools\M602\config/",
    /// while FilePathHelper.CombineAbsolute does a better job.
    /// </summary>
    [TestMethod()]
    public void CombineAbsoluteComparedWithPathCombineTest()
    {
        string[,] arrBased_Relative_Expected = new string[,] {
            { @"c:\Tools\M602\",     @"config/",     @"c:\Tools\M602\config\" },
        };

        int nDim0 = arrBased_Relative_Expected.GetLength(0);
        for (int ii = 0; ii < nDim0; ii++)
        {
            string basePath = arrBased_Relative_Expected[ii, 0];
            string relativePath = arrBased_Relative_Expected[ii, 1];
            string expected = arrBased_Relative_Expected[ii, 2];
            string actualCombineAbsolute, actualPathCombine;

            actualCombineAbsolute = FilePathHelper.CombineAbsolute(basePath, relativePath);
            Assert.AreEqual(expected, actualCombineAbsolute, false, CultureInfo.InvariantCulture,
              "expected compared with actual returned from CombineAbsolute");
            actualPathCombine = Path.Combine(basePath, relativePath);
            Assert.AreNotEqual(expected, actualPathCombine, false, CultureInfo.InvariantCulture,
              "expected compared with actual returned from Path.Combine");
        }
    }
    #endregion // CombineAbsoluteTest

    #region IsSubfolderTest

    /// <summary>
    /// A test for IsSubfolder, with the last argument allowEqual is true, which should succeed.
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_IsSubfolderTest_01()
    {
        string[,] arrParent_and_Child_true = new string[,] {
    { _320CharsAbsolutePath,   _320CharsAbsolutePath},
    { _320CharsAbsolutePath,   FilePathHelper.AppendPathSeparator(_320CharsAbsolutePath) + "123456789"},
  };

        int nDim0 = arrParent_and_Child_true.GetLength(0);
        for (int ii = 0; ii < nDim0; ii++)
        {
            string parentPath = arrParent_and_Child_true[ii, 0];
            string childPath = arrParent_and_Child_true[ii, 1];
            bool expected = true;
            bool actual = FilePathHelper.IsSubfolder(parentPath, childPath, true);

            Assert.AreEqual(expected, actual);
        }
    }

    /// <summary>
    /// A test for IsSubfolder, with the last argument allowEqual is true, and expected result true,
    /// which should succeed
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_IsSubfolderTest_02()
    {
        string[,] arrParent_and_Child_true = new string[,] {
    { @"c:",               @"c:"},
    { @"c:\",              @"c:\"},
    { @"c:\Tools\M602",    @"c:/Tools/M602"},
    { @"c:\Tools\M602\",   @"c:\Tools/M602\"},
    { @"c:\Tools\M602/",   @"c:\Tools\M602/"},
    { @"c:\Tools\M602",    @"c:/Tools/M602/xyz"},
    { @"c:\Tools\M602\",   @"c:\Tools/M602\xyz"},
    { @"c:\Tools\M602",    @"c:/Tools/M602/xyz/"},
    { @"c:\Tools\M602\",   @"c:\Tools/M602\xyz\"},
  };

        int nDim0 = arrParent_and_Child_true.GetLength(0);
        for (int ii = 0; ii < nDim0; ii++)
        {
            string parentPath = arrParent_and_Child_true[ii, 0];
            string childPath = arrParent_and_Child_true[ii, 1];
            bool expected = true;
            bool actual = FilePathHelper.IsSubfolder(parentPath, childPath, true);

            Assert.AreEqual(expected, actual);
        }
    }

    /// <summary>
    /// A test for IsSubfolder, with the last argument allowEqual is false, and expected result false,
    /// which should succeed
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_IsSubfolderTest_03()
    {
        string[,] arrParent_and_Child_true = new string[,] {
    { @"c:",               @"c:"},
    { @"c:\",              @"c:\"},
    { @"c:\Tools\M602",    @"c:/Tools/M602"},
    { @"c:\Tools\M602\",   @"c:\Tools/M602\"},
    { @"c:\Tools\M602/",   @"c:\Tools\M602/"},
    { @"c:\Tools\M602",    @"c:/Tools/M602xyz"},
    { @"c:\Tools\M602\",   @"c:\Tools/M602xyz"},
    { @"c:\Tools\M602",    @"c:/Tools/M602_abc/xyz/"},
    { @"c:\Tools\M602\",   @"c:\Tools/M602_abc\xyz\"},
    { @"c:\Tools\M602",    @"F:/Tools/M602/xyz"},
  };

        int nDim0 = arrParent_and_Child_true.GetLength(0);
        for (int ii = 0; ii < nDim0; ii++)
        {
            string parentPath = arrParent_and_Child_true[ii, 0];
            string childPath = arrParent_and_Child_true[ii, 1];
            bool expected = false;
            bool actual = FilePathHelper.IsSubfolder(parentPath, childPath, false);

            Assert.AreEqual(expected, actual);
        }
    }
    #endregion // IsSubfolderTest

    #region GetRelativePathTest

    /// <summary>
    /// A test for GetRelativePath which should succeed
    /// It tests that the method accepts input paths longer than Win32.MAX_PATH without exception.
    /// </summary>
    [TestMethod]
    [DataRow(_320CharsAbsolutePath, _320CharsAbsolutePath + @"\xyz\Readme.txt", @"xyz\Readme.txt")]
    [DataRow(_320CharsAbsolutePath, _320CharsAbsolutePath, "")]
    [DataRow(_320CharsAbsolutePath, @"C:\Tmp3\xyz\CtrlSourceCode", null)]
    public void FilePathHelper_GetRelativePath_01(string parentPath, string childPath, string expected)
    {
        // Act
        string actual = FilePathHelper.GetRelativePath(parentPath, childPath);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// A test for GetRelativePath which should succeed
    /// </summary>
    [TestMethod()]
    [DataRow(@"C:\Tmp3", @"C:\Tmp3\xyz\CtrlSourceCode", "xyz\\CtrlSourceCode")]
    [DataRow(@"C:\Tmp3\", @"C:\Tmp3\xyz\CtrlSourceCode", "xyz\\CtrlSourceCode")]
    [DataRow(@"C:\Tmp3", @"C:\Tmp3\xyz\CtrlSourceCode\", "xyz\\CtrlSourceCode")]
    [DataRow(@"C:\Tmp3\", @"C:\Tmp3\xyz\CtrlSourceCode\", "xyz\\CtrlSourceCode")]
    [DataRow(@"C:\Tmp3\", @"C:\Tmp3\xyz\Readme.txt", "xyz\\Readme.txt")]
    [DataRow(@"C:\", @"C:\Tmp3\xyz\Readme.txt", "Tmp3\\xyz\\Readme.txt")]
    [DataRow(@"C:\Tmp3\", @"C:\Tmp3\", "")]
    [DataRow(@"C:\Tmp3\abc", @"C:\Tmp3\xyz\CtrlSourceCode", null)]
    public void FilePathHelper_GetRelativePath_02(
        string parentPath,
        string childPath,
        string expectedRelative)
    {
        // Act
        string actual = FilePathHelper.GetRelativePath(parentPath, childPath);

        // Assert
        Assert.AreEqual(expectedRelative, actual);
    }

    #endregion // CheckIsValidFolderStringTest

    #region GetRelativePathTest

    /// <summary>
    /// A test for GetRelativePath which should fail with ArgumentException
    ///</summary>
    [TestMethod()]
    public void FilePathHelper_GetRelativePath_03()
    {
        Assert.ThrowsExactly<ArgumentException>(() => FilePathHelper.GetRelativePath(string.Empty, @"C:\Tmp3\"));
    }

    /// <summary>
    /// A test for GetRelativePath which should fail with ArgumentNullException
    ///</summary>
    [TestMethod()]
    public void FilePathHelper_GetRelativePath_04()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => FilePathHelper.GetRelativePath(null, @"C:\Tmp3\"));
    }
    #endregion // GetRelativePathTest

    #region CombineNoChecksTest

    /// <summary>
    /// A test for CombineNoChecks which should succeed
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_CombineNoChecksTest_01()
    {
        string basePath = _320CharsAbsolutePath;
        string relativePath = @"123456789";
        string expected = FilePathHelper.AppendPathSeparator(_320CharsAbsolutePath) + "123456789";
        string actual = FilePathHelper.CombineNoChecks(basePath, relativePath);
        Assert.AreEqual(expected, actual);

        basePath = @"c:\";
        relativePath = _320CharsAbsolutePath.Substring(basePath.Length);
        expected = _320CharsAbsolutePath;
        actual = FilePathHelper.CombineNoChecks(basePath, relativePath);
        Assert.AreEqual(expected, actual);
    }

    #endregion // GetRelativePathTest

    #region CombineNoChecksTest

    /// <summary>
    /// A test for CombineNoChecks which should fail with ArgumentNullException
    ///</summary>
    [TestMethod()]
    public void FilePathHelper_CombineNoChecks_02()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => FilePathHelper.CombineNoChecks(null, @"C:\Tmp3\"));
    }

    /// <summary>
    /// A test for CombineNoChecks which should fail with ArgumentNullException
    ///</summary>
    [TestMethod()]
    public void FilePathHelper_CombineNoChecks_03()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => FilePathHelper.CombineNoChecks(@"C:\Tmp3\", null));
    }

    /// <summary>
    /// A test for CombineNoChecks which should succeed
    ///</summary>
    [TestMethod()]
    public void FilePathHelper_CombineNoChecks_04()
    {
        string[,] p1_p2_result = new string[,] {
    { @"C:\Tmp3",      @"xyz\CtrlSourceCode",     @"C:\Tmp3\xyz\CtrlSourceCode"    },
    { @"C:\Tmp3\",     @"xyz\CtrlSourceCode",     @"C:\Tmp3\xyz\CtrlSourceCode",   },
    { @"C:\Tmp3",      @"xyz\CtrlSourceCode\",    @"C:\Tmp3\xyz\CtrlSourceCode\",  },
    { @"C:\Tmp3\",     @"xyz\CtrlSourceCode\",    @"C:\Tmp3\xyz\CtrlSourceCode\",  },
    { @"C:\Tmp3\",     @"xyz\Readme.txt",         @"C:\Tmp3\xyz\Readme.txt",       },
    { @"C:\",          @"Tmp3\xyz\Readme.txt",    @"C:\Tmp3\xyz\Readme.txt",       },
    { @"C:\Tmp3\",     string.Empty,              @"C:\Tmp3\",                     },
    { string.Empty,    @"C:\Tmp3\",               @"C:\Tmp3\",                     },
    { @"G:\XYZ\",      @"C:\Tmp3\",               @"C:\Tmp3\",                     },
  };

        int nDim0 = p1_p2_result.GetLength(0);
        for (int ii = 0; ii < nDim0; ii++)
        {
            string p1 = p1_p2_result[ii, 0];
            string p2 = p1_p2_result[ii, 1];
            string expected = p1_p2_result[ii, 2];
            string actual = FilePathHelper.CombineNoChecks(p1, p2);

            Assert.AreEqual(expected, actual);
        }
    }
    #endregion // CombineNoChecksTest

    #region LongPathTolerantGetDirectoryNameTest

    /// <summary>
    /// A test for LongPathTolerantGetDirectoryName which should fail with ArgumentException
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_LongPathTolerantGetDirectoryNameTest_01()
    {
        string s = "\x1A_XYZ";
        Assert.ThrowsExactly<ArgumentException>(() => FilePathHelper.LongPathTolerantGetDirectoryName(s));
    }

    /// <summary>
    /// A test for LongPathTolerantGetDirectoryName which should fail with ArgumentException
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_LongPathTolerantGetDirectoryNameTest_02()
    {
        Assert.ThrowsExactly<ArgumentException>(() => FilePathHelper.LongPathTolerantGetDirectoryName(string.Empty));
    }

    /// <summary>
    /// A test for LongPathTolerantGetDirectoryName which should fail with ArgumentException
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_LongPathTolerantGetDirectoryNameTest_03()
    {
        string s = "   ";
        Assert.ThrowsExactly<ArgumentException>(() => FilePathHelper.LongPathTolerantGetDirectoryName(s));
    }

    /// <summary>
    /// A test for LongPathTolerantGetDirectoryName which should succeed.
    /// The test tries just absolute paths, with drive letter
    /// </summary>
    [TestMethod()]
    [DataRow(@"c:", null)]
    [DataRow(@"c:\", null)]
    [DataRow(@"c:\Tools\M602", @"c:\Tools")]
    [DataRow(@"c:/Tools/M602", @"c:\Tools")]
    [DataRow(@"c:\Tools\M602\", @"c:\Tools\M602")]
    [DataRow(@"c:\Tools\M602/", @"c:\Tools\M602")]
    [DataRow(@"c:/Tools/M602/x", @"c:\Tools\M602")]
    [DataRow(@"c:/Tools/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/M602",
      @"c:\Tools\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")]
    [DataRow(@"c:/Tools/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/M602/",
      @"c:\Tools\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\M602")]
    [DataRow(@"c:/Tools/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/M602/x",
      @"c:\Tools\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\M602")]
    public void FilePathHelper_LongPathTolerantGetDirectoryNameTest_04(string strPath, string expected)
    {
        string actual = FilePathHelper.LongPathTolerantGetDirectoryName(strPath);
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// A test for LongPathTolerantGetDirectoryName which should succeed.
    /// The test tries just absolute paths, without drive letter
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_LongPathTolerantGetDirectoryNameTest_05()
    {
        string[,] arrPath_and_Expected = new string[,] {
            { @"\",                null!},
            { @"/",                null!},
            { @"\Tools\M602",    @"\Tools"},
            { @"/Tools/M602",    @"\Tools"},
            { @"\Tools\M602\",   @"\Tools\M602"},
            { @"\Tools\M602/",   @"\Tools\M602"},
            { @"/Tools/M602/x",  @"\Tools\M602"},

            { @"/Tools/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/M602",
                @"\Tools\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"},

            { @"/Tools/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/M602/",
                @"\Tools\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\M602"},

            { @"/Tools/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/M602/x",
                @"\Tools\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\M602"},
        };

        int nDim0 = arrPath_and_Expected.GetLength(0);
        for (int ii = 0; ii < nDim0; ii++)
        {
            string strPath = arrPath_and_Expected[ii, 0];
            string expected = arrPath_and_Expected[ii, 1];
            string actual;

            actual = FilePathHelper.LongPathTolerantGetDirectoryName(strPath);
            Assert.AreEqual(expected, actual);
        }
    }

    /// <summary>
    /// A test for LongPathTolerantGetDirectoryName which should succeed
    /// The test tries just relative paths
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_LongPathTolerantGetDirectoryNameTest_06()
    {
        string[,] arrPath_and_Expected = new string[,] {

    { @"Tools\M602",    @"Tools"},
    { @"Tools/M602",    @"Tools"},
    { @"Tools\M602\",   @"Tools\M602"},
    { @"Tools\M602/",   @"Tools\M602"},
    { @"Tools/M602/x",  @"Tools\M602"},

    { @"Tools/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/M602",
      @"Tools\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"},

    { @"Tools/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/M602/",
      @"Tools\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\M602"},

    { @"Tools/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/M602/x",
      @"Tools\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\M602"},
  };

        int nDim0 = arrPath_and_Expected.GetLength(0);
        for (int ii = 0; ii < nDim0; ii++)
        {
            string strPath = arrPath_and_Expected[ii, 0];
            string expected = arrPath_and_Expected[ii, 1];
            string actual;

            actual = FilePathHelper.LongPathTolerantGetDirectoryName(strPath);
            Assert.AreEqual(expected, actual);
        }
    }
    #endregion // LongPathTolerantGetDirectoryNameTest

    #region GetPathPartsTest

    /// <summary>
    /// A test for SafeGetDirectoryInfo which should succeed.
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_GetPathPartsTest_01()
    {
        var collection = FilePathHelper.GetPathParts(_320CharsAbsolutePath);
        Assert.IsTrue(collection.Count > 0);
    }

    /// <summary>
    /// A test for GetPathParts which should succeed
    ///</summary>
    [TestMethod()]
    public void FilePathHelper_GetPathPartsTest_02()
    {
        var collection = FilePathHelper.GetPathParts(null);

        Assert.AreEqual(0, collection.Count);
    }

    /// <summary>
    /// A test for GetPathParts which should succeed
    ///</summary>
    [TestMethod()]
    public void FilePathHelper_GetPathPartsTest_03()
    {
        string parentPath = @"C:\Tmp3\";
        string childPath = @"C:\Tmp3\xyz\CtrlSourceCode\";
        var collection = FilePathHelper.GetPathParts(FilePathHelper.GetRelativePath(parentPath, childPath));

        Assert.AreEqual(2, collection.Count);
    }

    /// <summary>
    /// A test for GetPathParts which should succeed
    ///</summary>
    [TestMethod()]
    public void FilePathHelper_GetPathPartsTest_04()
    {
        string parentPath = @"C:\Tmp3\abc";
        string childPath = @"C:\Tmp3\xyz\CtrlSourceCode";
        var collection = FilePathHelper.GetPathParts(FilePathHelper.GetRelativePath(parentPath, childPath));

        Assert.AreEqual(0, collection.Count);
    }
    #endregion // GetPathPartsTest

    #region PathCompactTest

    /// <summary>
    /// A test for PathCompact which should succeed
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [TestMethod()]
    [DataRow(48)]
    [DataRow(8)]
    public void FilePathHelper_PathCompactTest_01(int shortenTo)
    {
        string path = _320CharsAbsolutePath;
        string actual = FilePathHelper.PathCompact(path, shortenTo);
        Assert.IsTrue(actual.Length <= shortenTo);
    }

    /// <summary>
    /// A test for PathCompact which should succeed
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_PathCompactTest_02()
    {
        string[,] arrPath_and_Expected = new string[,] {
    { null!,                null!},
    { @"",                 @""},
    { @"\",                @"\"},
    { @"/",                @"/"},
    { @"c:",               @"c:"},
    { @"c:\",              @"c:\"},
    { @"c:\Tools\M602",    @"...\M602"},
  };

        int nDim0 = arrPath_and_Expected.GetLength(0);
        for (int ii = 0; ii < nDim0; ii++)
        {
            string strPath = arrPath_and_Expected[ii, 0];
            string expected = arrPath_and_Expected[ii, 1];
            string actual;

            actual = FilePathHelper.PathCompact(strPath, 8);
            Assert.AreEqual(expected, actual);
        }
    }
    #endregion // PathCompactTest

    #region SafeGetDirectoryInfoTest

    /// <summary>
    /// A test for SafeGetDirectoryInfo which should succeed.
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_SafeGetDirectoryInfoTest_01()
    {
        string strLongtDir = _320CharsAbsolutePath;
        string strNonExistDir;
        for (int ii = 0; ;)
        {
            strNonExistDir = string.Format(CultureInfo.InvariantCulture, "{0}{1}", strLongtDir, ii);
            if (!Directory.Exists(strNonExistDir)) break;
        }

        string path = strNonExistDir;
        DirectoryInfo expected = null!;
        DirectoryInfo actual = FilePathHelper.SafeGetDirectoryInfo(path);
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// A test for SafeGetDirectoryInfo which should succeed.
    /// It test that for non-existing directory the resulting value is null
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_SafeGetDirectoryInfoTest_02()
    {
        // Initialize path to point to non-existing directory
        string strCurrentDir = Environment.CurrentDirectory;
        string strNonExistDir;
        for (int ii = 0; ;)
        {
            strNonExistDir = string.Format(CultureInfo.InvariantCulture, "{0}{1}", strCurrentDir, ii);
            if (!Directory.Exists(strNonExistDir)) break;
        }

        string path = strNonExistDir;
        DirectoryInfo expected = null!;
        DirectoryInfo actual = FilePathHelper.SafeGetDirectoryInfo(path);
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// A test for SafeGetDirectoryInfo which should succeed.
    /// It test that for existing directory the resulting value is not null
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_SafeGetDirectoryInfoTest_03()
    {
        string strCurrentDir = Environment.CurrentDirectory;

        DirectoryInfo current = FilePathHelper.SafeGetDirectoryInfo(strCurrentDir);
        Assert.IsNotNull(current);
    }
    #endregion // SafeGetDirectoryInfoTest

    #region SafeGetFileInfoTest

    /// <summary>
    /// A test for SafeGetFileInfo which should succeed.
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_SafeGetFileInfoTest_01()
    {
        string strTmp;
        string strLongtDir = _320CharsAbsolutePath;
        string strNonExistingFile;

        for (int ii = 0; ;)
        {
            strTmp = string.Format(CultureInfo.InvariantCulture, "aaa_{0}.txt", ii);
            strNonExistingFile = FilePathHelper.CombineNoChecks(strLongtDir, strTmp);
            if (!File.Exists(strNonExistingFile)) break;
        }

        FileInfo expected = null!;
        FileInfo actual = FilePathHelper.SafeGetFileInfo(strNonExistingFile);
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// A test for SafeGetFileInfo which should succeed
    /// </summary>
    [TestMethod()]
    public void FilePathHelper_SafeGetFileInfoTest_02()
    {
        // Initialize path to point to non-existing directory
        string strCurrentDir = Environment.CurrentDirectory;
        string strNonExistFile;
        for (int ii = 0; ;)
        {
            strNonExistFile = string.Format(CultureInfo.InvariantCulture, "{0}{1}.txt", strCurrentDir, ii);
            if (!File.Exists(strNonExistFile)) break;
        }

        string strPath = strNonExistFile;
        bool bMakeNullOnNotExisting = true;
        FileInfo expected = null!;
        FileInfo actual = FilePathHelper.SafeGetFileInfo(strPath, bMakeNullOnNotExisting);
        Assert.AreEqual(expected, actual);
    }
    #endregion // SafeGetFileInfoTest
    #endregion // Tests
}

#pragma warning restore IDE0057 // Use range operator
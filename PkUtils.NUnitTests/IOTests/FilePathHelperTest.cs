// Ignore Spelling: Utils, Subfolder
//
using System.Globalization;
using PK.PkUtils.IO;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable VSSpell001
#pragma warning disable IDE0057 // Use range operator
#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.PkUtils.NUnitTests.IOTests;

/// <summary> This is a test class for <see cref="FilePathHelper"/>. </summary>
[TestFixture()]
[CLSCompliant(false)]
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
    [Test()]
    public void FilePathHelper_HasWildCardTest_01()
    {
        Assert.That(FilePathHelper.HasWildCard(_320CharsAbsolutePath), Is.False);
    }

    /// <summary>
    /// A test for HasWildCard which should succeed
    /// </summary>
    [Test()]
    public void FilePathHelper_HasWildCardTest_02()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(FilePathHelper.HasWildCard(null), Is.False);
            Assert.That(FilePathHelper.HasWildCard(""), Is.False);
            Assert.That(FilePathHelper.HasWildCard("abc"), Is.False);
            Assert.That(FilePathHelper.HasWildCard("\\//"), Is.False);

            Assert.That(FilePathHelper.HasWildCard("?"), Is.True);
            Assert.That(FilePathHelper.HasWildCard("aa*c"), Is.True);
            Assert.That(FilePathHelper.HasWildCard("aa?c"), Is.True);
        }
    }
    #endregion // HasWildCardTest

    #region AppendPathSeparatorTest

    /// <summary>
    /// A test for AppendPathSeparator which should succeed
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [Test()]
    public void FilePathHelper_AppendPathSeparatorTest_01()
    {
        var expected = Path.DirectorySeparatorChar;
        var actual = FilePathHelper.AppendPathSeparator(_320CharsAbsolutePath).Last();

        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A test for AppendPathSeparator which should succeed
    /// </summary>
    [Test()]
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
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
    #endregion // AppendPathSeparatorTest

    #region TrimEndingDirectorySeparatorTest

    /// <summary>
    /// A test for TrimEndingDirectorySeparator which should succeed
    /// </summary>
    [Test()]
    public void FilePathHelper_TrimEndingDirectorySeparatorTest_01()
    {
        string expected = _320CharsAbsolutePath;
        string actual = FilePathHelper.TrimEndingDirectorySeparator(FilePathHelper.AppendPathSeparator(_320CharsAbsolutePath));
        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A test for TrimEndingDirectorySeparator which should succeed
    /// </summary>
    [Test()]
    public void FilePathHelper_TrimEndingDirectorySeparatorTest_02()
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
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
    #endregion // TrimEndingDirectorySeparatorTest

    #region TrimPathDoubleQuotesTest

    /// <summary>
    /// A test for TrimPathDoubleQuotes which should succeed
    /// </summary>
    [Test()]
    public void FilePathHelper_TrimPathDoubleQuotesTest_01()
    {
        string strPath = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", _320CharsAbsolutePath);
        string expected = _320CharsAbsolutePath;
        string actual = FilePathHelper.TrimPathDoubleQuotes(strPath, false);

        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A test for TrimPathDoubleQuotes for argument value trimSpaces = false
    /// </summary>
    [TestCase("e:\\Tools\\", "e:\\Tools\\")]
    [TestCase("\"e:\\Tools\\", "\"e:\\Tools\\")]
    [TestCase("\"e:\\Tools\\\"", "e:\\Tools\\")]
    [TestCase("\"e:\\Tools\"", "e:\\Tools")]
    [TestCase("\"e:\"", "e:")]
    [TestCase("\"e:\\Fox dummy\"", "e:\\Fox dummy")]
    [TestCase("  \"e:\\Tools\"  ", "  \"e:\\Tools\"  ")]
    [TestCase("  \"e:\"  ", "  \"e:\"  ")]
    [TestCase("\"\"", "")]
    [TestCase("\"\"\"", "\"")]
    [TestCase("abc", "abc")]
    [TestCase("\"", "\"")]
    [TestCase("", "")]
    [TestCase(null!, null!)]
    public void FilePathHelper_TrimPathDoubleQuotesTest_02(string strPathOriginal, string resultExpected)
    {
        const bool trimTrailingSpacesFalse = false;
        string resultActual = FilePathHelper.TrimPathDoubleQuotes(strPathOriginal, trimTrailingSpacesFalse);
        Assert.That(resultActual, Is.EqualTo(resultExpected));
    }

    /// <summary>
    /// A test for TrimPathDoubleQuotes for argument value trimSpaces = true
    /// </summary>
    [TestCase("d:\\Tools\\", "d:\\Tools\\")]
    [TestCase("\"d:\\Tools\\", "\"d:\\Tools\\")]
    [TestCase("\"d:\\Tools\\\"", "d:\\Tools\\")]
    [TestCase("\"d:\\Tools\"", "d:\\Tools")]
    [TestCase("\"d:\"", "d:")]
    [TestCase("\"d:\\Fox dummy\"", "d:\\Fox dummy")]
    [TestCase("  \"d:\\Tools\"  ", "d:\\Tools")]
    [TestCase("  \"d:\"  ", "d:")]
    [TestCase("  d:      ", "  d:      ")]
    [TestCase("\"\"", "")]
    [TestCase("\"\"\"", "\"")]
    [TestCase("abc", "abc")]
    [TestCase("\"", "\"")]
    public void FilePathHelper_TrimPathDoubleQuotesTest03(string strPathOriginal, string resultExpected)
    {
        const bool trimTrailingSpacesTrue = true;
        string resultActual = FilePathHelper.TrimPathDoubleQuotes(strPathOriginal, trimTrailingSpacesTrue);
        Assert.That(resultActual, Is.EqualTo(resultExpected));
    }
    #endregion // TrimPathDoubleQuotesTest

    #region IsPathRootOnlyTest

    /// <summary>
    /// A test for IsPathRootOnly which should succeed.
    /// It tests that the method accepts input longer than Win32.MAX_PATH without exception.
    /// </summary>
    [Test()]
    public void FilePathHelper_IsPathRootOnlyTest_01()
    {
        bool actual = FilePathHelper.IsPathRootOnly(_320CharsAbsolutePath, false);
        Assert.That(actual, Is.False);
    }

    /// <summary>
    /// A test for IsPathRootOnly which should succeed.
    /// Is tested always with the second argument bool acceptDriveLetterColon = false.
    /// </summary>
    [Test()]
    [TestCase(@"\", true)]
    [TestCase(@"\", true)]
    [TestCase(@"/", true)]
    [TestCase(@"c:\", true)]
    [TestCase(@"c:/", true)]
    [TestCase(null!, false)]
    [TestCase(@"", false)]
    [TestCase(@":", false)]
    [TestCase(@":\", false)]
    [TestCase(@":/", false)]
    [TestCase(@"c:", false)]
    [TestCase(@"c:\Tools\M602", false)]
    [TestCase(@"c:\Tools\M602\", false)]
    [TestCase(@"c:\Tools\M602/", false)]
    public void FilePathHelper_IsPathRootOnlyTest_02(string strPath, bool expected)
    {
        bool actual = FilePathHelper.IsPathRootOnly(strPath, false);

        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A test for IsPathRootOnly which should succeed.
    /// Is tested always with the second argument bool acceptDriveLetterColon = true.
    /// </summary>
    [Test]
    [TestCase(@"c:", true)]
    [TestCase(@"D:", true)]
    [TestCase(@"?:", false)]
    [TestCase(@"::", false)]
    [TestCase(@":", false)]
    public void FilePathHelper_IsPathRootOnlyTest_03(string strPath, bool expected)
    {
        bool actual = FilePathHelper.IsPathRootOnly(strPath, true);

        Assert.That(actual, Is.EqualTo(expected));
    }
    #endregion // IsPathRootOnlyTest

    #region IsPathRootedTest

    /// <summary>
    /// A test for IsPathRooted which should succeed.
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [Test()]
    public void FilePathHelper_IsPathRootedTest_01()
    {
        bool expected = true;
        bool actual = FilePathHelper.IsPathRooted(_320CharsAbsolutePath, false);

        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A test for IsPathRooted which should succeed.
    /// Is tested always with the second argument bool acceptDriveLetterColon = false.
    /// </summary>
    [Test]
    [TestCase(null!, false)]
    [TestCase(@"\", true)]
    [TestCase(@"/", true)]
    [TestCase(@"\\", true)]
    [TestCase(@"\/", true)]
    [TestCase(@"//", true)]
    [TestCase(@"/\", true)]
    [TestCase(@"\T", true)]
    [TestCase(@"/T", true)]
    [TestCase(@"\ ", true)]
    [TestCase(@"\ \", true)]
    [TestCase(@"\1", true)]
    [TestCase(@"\1\", true)]
    [TestCase(@"\~", true)]
    [TestCase(@"\~\", true)]
    [TestCase(@"\š", true)]
    [TestCase(@"\š\", true)]
    [TestCase(@"\šašek šišlavý", true)]
    [TestCase(@"\šašek šišlavý\", true)]
    [TestCase(@"\TEMP\t.txt", true)]
    [TestCase(@"/TEMP/t.txt", true)]
    [TestCase(@"c:\", true)]
    [TestCase(@"c:/", true)]
    [TestCase(@"c:\Tools\M602", true)]
    [TestCase(@"c:\Tools\M602\", true)]
    [TestCase(@"c:\Tools\M602/", true)]
    [TestCase(@"\\\\", true)]
    [TestCase(@"", false)]
    [TestCase(@":", false)]
    [TestCase(@":\", false)]
    [TestCase(@":/", false)]
    [TestCase(@"c:", false)]
    [TestCase(@"\<", false)]
    [TestCase(@"\>", false)]
    [TestCase(@"\|", false)]
    public void FilePathHelper_IsPathRootedTest_02(string strPath, bool expected)
    {
        string strMsg;
        bool actual = FilePathHelper.IsPathRooted(strPath, false);

        // 1. check that expected and actual value match
        strMsg = string.Format(CultureInfo.InvariantCulture,
            "For the case of path '{0}', expected value is: {1}, actual value is: {2}",
            strPath, expected, actual);
        Assert.That(actual, Is.EqualTo(expected), strMsg);

        // 2. check that except the tested value @"c:", 
        //    the result either match the value returned by Path.IsPathRooted,
        //    or is false in case Path.IsPathRooted throws ArgumentEception
        if ((!string.IsNullOrEmpty(strPath)) &&
            (!string.Equals(strPath, @"c:", StringComparison.OrdinalIgnoreCase)) &&
            (0 > strPath.IndexOfAny(FilePathHelper.PathInvalidOrWildCharacters.ToArray())))
        {
            bool bActual;
            bool bExpected = false;
            bool bException = false;

            try
            {
                bExpected = Path.IsPathRooted(strPath);
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
            Assert.That(bActual, Is.EqualTo(bExpected), strMsg);
        }
    }

    /// <summary>
    /// A test for IsPathRooted which should succeed.
    /// Is tested always with the second argument bool acceptDriveLetterColon = true.
    /// </summary>
    [Test()]
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

            Assert.That(string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase), Is.True);
        }
    }
    #endregion // IsPathRootedTest

    #region CheckIsValidFolderStringTest

    /// <summary>
    /// A test for CheckIsValidFolderString which should succeed.
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [Test()]
    public void FilePathHelper_CheckIsValidFolderStringTest_01()
    {
        FilePathHelper.CheckIsValidFolderString(_320CharsAbsolutePath);
    }

    /// <summary>
    /// A test for CheckIsValidFolderString which should succeed.
    /// </summary>
    [Test()]
    public void FilePathHelper_CheckIsValidFolderStringTest_02()
    {
        string[] arrPath = [
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
        ];

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
    [Test()]
    public void FilePathHelper_CheckIsValidFolderStringTest_03()
    {
        Assert.Throws<ArgumentNullException>(() =>
            FilePathHelper.CheckIsValidFolderString(null));
    }

    /// <summary>
    /// A test for CheckIsValidFolderString which should fail with ArgumentException
    /// </summary>
    [Test()]
    public void FilePathHelper_CheckIsValidFolderStringTest_04()
    {
        Assert.Throws<ArgumentException>(() =>
            FilePathHelper.CheckIsValidFolderString(string.Empty));
    }


    /// <summary>
    /// A test for CheckIsValidFolderString which should fail with ArgumentException
    /// </summary>
    [Test()]
    public void FilePathHelper_CheckIsValidFolderStringTest_05()
    {
        string strArg = string.Join(" ", Path.GetInvalidPathChars());
        Assert.Throws<ArgumentException>(() =>
            FilePathHelper.CheckIsValidFolderString(strArg));
    }

    /// <summary>
    /// A test for CheckIsValidFolderString which should succeed
    /// </summary>
    [Test()]
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
    [Test()]
    public void FilePathHelper_AcceptsLongPath_Test()
    {
        FilePathHelper.GetUpperFolder(_320CharsAbsolutePath, false);
    }

    /// <summary>
    /// A test for GetUpperFolder which should succeed
    /// </summary>
    [Test()]
    [TestCase(@"e:\Tools\LinkRank\", @"e:\Tools\", false)]
    [TestCase(@"e:\Tools\LinkRank", @"e:\Tools\", false)]
    [TestCase(@"e:\Tools\", @"e:\", false)]
    [TestCase(@"e:\", @"e:\", false)]
    [TestCase(@"\Tools\", @"\", false)]
    [TestCase(@"\", @"\", false)]
    [TestCase(@"", @"", false)]
    // pairs of { original path, expected result } for case allowEmpty = true
    [TestCase(@"c:\Tools\LinkRank\", @"c:\Tools\", true)]
    [TestCase(@"c:\Tools\LinkRank", @"c:\Tools\", true)]
    [TestCase(@"c:\Tools\", @"c:\", true)]
    [TestCase(@"c:\", @"", true)]
    [TestCase(@"\Tools\", @"\", true)]
    [TestCase(@"\", @"", true)]
    [TestCase(@"", @"", true)]
    public void FilePathHelper_GetUpperFolderWorks_Test(string strPathOriginal, string resultExpected, bool allowEmpty)
    {
        string resultActual = FilePathHelper.GetUpperFolder(strPathOriginal, allowEmpty);
        Assert.That(resultActual, Is.EqualTo(resultExpected));
    }
    #endregion // GetUpperFolderTest

    #region GenerateAbsoluteTest

    [Test()]
    public void FilePathHelper_GenerateAbsolute_Succeeds_Test()
    {
        string basePath = _317CharsRelativePath;
        string resultActual = FilePathHelper.GenerateAbsolute(basePath);

        Assert.That(resultActual, Does.EndWith(basePath));
    }
    #endregion // GenerateAbsoluteTest

    #region CombineAbsoluteTest

    /// <summary>
    /// A test for CombineAbsolute which should succeed
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [Test()]
    public void FilePathHelper_CombineAbsoluteTest_01()
    {
        string basePath = _320CharsAbsolutePath;
        string relativePath = @"123456789";
        string expected = FilePathHelper.AppendPathSeparator(_320CharsAbsolutePath) + "123456789";
        string actual = FilePathHelper.CombineAbsolute(basePath, relativePath);
        Assert.That(actual, Is.EqualTo(expected));

        basePath = @"c:\";
        relativePath = _320CharsAbsolutePath.Substring(basePath.Length);
        expected = _320CharsAbsolutePath;
        actual = FilePathHelper.CombineAbsolute(basePath, relativePath);
        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A test for CombineAbsolute which should succeed
    /// </summary>
    [Test()]
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
            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    /// <summary>
    /// A test for CombineAbsolute compared with Path.Combine.
    /// It demonstrates that Path.Combine(@"c:\Tools\M602\", @"config/") returns @"c:\Tools\M602\config/",
    /// while FilePathHelper.CombineAbsolute does a better job.
    /// </summary>
    [Test()]
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
            Assert.That(actualCombineAbsolute, Is.EqualTo(expected),
                      "expected compared with actual returned from CombineAbsolute");
            actualPathCombine = Path.Combine(basePath, relativePath);
            Assert.That(actualPathCombine, Is.Not.EqualTo(expected),
                "expected compared with actual returned from Path.Combine");
        }
    }
    #endregion // CombineAbsoluteTest

    #region IsSubfolderTest

    /// <summary>
    /// A test for IsSubfolder, with the last argument allowEqual is true, which should succeed.
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [Test()]
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

            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    /// <summary>
    /// A test for IsSubfolder, with the last argument allowEqual is true, and expected result true,
    /// which should succeed
    /// </summary>
    [Test()]
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

            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    /// <summary>
    /// A test for IsSubfolder, with the last argument allowEqual is false, and expected result false,
    /// which should succeed
    /// </summary>
    [Test()]
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

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
    #endregion // IsSubfolderTest

    #region GetRelativePathTest

    /// <summary>
    /// A test for GetRelativePath which should succeed
    /// It tests that the method accepts input paths longer than Win32.MAX_PATH without exception.
    /// </summary>
    [TestCase(_320CharsAbsolutePath, _320CharsAbsolutePath + @"\xyz\Readme.txt", @"xyz\Readme.txt")]
    [TestCase(_320CharsAbsolutePath, _320CharsAbsolutePath, "")]
    [TestCase(_320CharsAbsolutePath, @"C:\Tmp3\xyz\CtrlSourceCode", null!)]
    public void FilePathHelper_GetRelativePath_01(string parentPath, string childPath, string expected)
    {
        // Act
        string actual = FilePathHelper.GetRelativePath(parentPath, childPath);

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A test for GetRelativePath which should succeed
    /// </summary>
    [TestCase(@"C:\Tmp3", @"C:\Tmp3\xyz\CtrlSourceCode", "xyz\\CtrlSourceCode")]
    [TestCase(@"C:\Tmp3\", @"C:\Tmp3\xyz\CtrlSourceCode", "xyz\\CtrlSourceCode")]
    [TestCase(@"C:\Tmp3", @"C:\Tmp3\xyz\CtrlSourceCode\", "xyz\\CtrlSourceCode")]
    [TestCase(@"C:\Tmp3\", @"C:\Tmp3\xyz\CtrlSourceCode\", "xyz\\CtrlSourceCode")]
    [TestCase(@"C:\Tmp3\", @"C:\Tmp3\xyz\Readme.txt", "xyz\\Readme.txt")]
    [TestCase(@"C:\", @"C:\Tmp3\xyz\Readme.txt", "Tmp3\\xyz\\Readme.txt")]
    [TestCase(@"C:\Tmp3\", @"C:\Tmp3\", "")]
    [TestCase(@"C:\Tmp3\abc", @"C:\Tmp3\xyz\CtrlSourceCode", null!)]
    public void FilePathHelper_GetRelativePath_02(
        string parentPath,
        string childPath,
        string expectedRelative)
    {
        // Act
        string actual = FilePathHelper.GetRelativePath(parentPath, childPath);

        // Assert
        Assert.That(actual, Is.EqualTo(expectedRelative));
    }

    /// <summary>
    /// A test for GetRelativePath which should fail with ArgumentException
    ///</summary>
    [Test()]
    public void FilePathHelper_GetRelativePath_03()
    {
        Assert.Throws<ArgumentException>(() =>
            FilePathHelper.GetRelativePath(string.Empty, @"C:\Tmp3\"));
    }

    /// <summary>
    /// A test for GetRelativePath which should fail with ArgumentNullException
    ///</summary>
    [Test()]
    public void FilePathHelper_GetRelativePath_04()
    {
        Assert.Throws<ArgumentNullException>(() =>
            FilePathHelper.GetRelativePath(null, @"C:\Tmp3\"));
    }
    #endregion // GetRelativePathTest

    #region CombineNoChecksTest

    /// <summary>
    /// A test for CombineNoChecks which should succeed
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [Test()]
    public void FilePathHelper_CombineNoChecksTest_01()
    {
        string basePath = _320CharsAbsolutePath;
        string relativePath = @"123456789";
        string expected = FilePathHelper.AppendPathSeparator(_320CharsAbsolutePath) + "123456789";
        string actual = FilePathHelper.CombineNoChecks(basePath, relativePath);
        Assert.That(actual, Is.EqualTo(expected));

        basePath = @"c:\";
        relativePath = _320CharsAbsolutePath.Substring(basePath.Length);
        expected = _320CharsAbsolutePath;
        actual = FilePathHelper.CombineNoChecks(basePath, relativePath);
        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A test for CombineNoChecks which should fail with ArgumentNullException
    ///</summary>
    [Test()]
    public void FilePathHelper_CombineNoChecks_02()
    {
        Assert.Throws<ArgumentNullException>(() => FilePathHelper.CombineNoChecks(null, @"C:\Tmp3\"));
    }

    /// <summary>
    /// A test for CombineNoChecks which should fail with ArgumentNullException
    ///</summary>
    [Test()]
    public void FilePathHelper_CombineNoChecks_03()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            string result = FilePathHelper.CombineNoChecks(@"C:\Tmp3\", null);
        });
    }

    /// <summary>
    /// A test for CombineNoChecks which should succeed
    ///</summary>
    [Test()]
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

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
    #endregion // CombineNoChecksTest

    #region LongPathTolerantGetDirectoryNameTest

    /// <summary>
    /// A test for LongPathTolerantGetDirectoryName which should fail with ArgumentException
    /// </summary>
    [Test()]
    public void FilePathHelper_LongPathTolerantGetDirectoryNameTest_01()
    {
        // the case of path with invalid character or characters
        string s = "\x1A_XYZ";
        Assert.Throws<ArgumentException>(() =>
            FilePathHelper.LongPathTolerantGetDirectoryName(s));
    }

    /// <summary>
    /// A test for LongPathTolerantGetDirectoryName which should fail with ArgumentException
    /// </summary>
    [Test()]
    public void FilePathHelper_LongPathTolerantGetDirectoryNameTest_02()
    {
        // the case of path which is empty
        Assert.Throws<ArgumentException>(() =>
            FilePathHelper.LongPathTolerantGetDirectoryName(string.Empty));
    }

    /// <summary>
    /// A test for LongPathTolerantGetDirectoryName which should fail with ArgumentException
    /// </summary>
    [Test()]
    public void FilePathHelper_LongPathTolerantGetDirectoryNameTest_03()
    {
        // the case of path which contains only white spaces
        string s = "   ";
        Assert.Throws<ArgumentException>(
            () => FilePathHelper.LongPathTolerantGetDirectoryName(s));
    }

    /// <summary>
    /// A test for LongPathTolerantGetDirectoryName which should succeed.
    /// The test tries just absolute paths, with drive letter
    /// </summary>
    [Test()]
    [TestCase(@"c:", null!)]
    [TestCase(@"c:\", null!)]
    [TestCase(@"c:\Tools\M602", @"c:\Tools")]
    [TestCase(@"c:/Tools/M602", @"c:\Tools")]
    [TestCase(@"c:\Tools\M602\", @"c:\Tools\M602")]
    [TestCase(@"c:\Tools\M602/", @"c:\Tools\M602")]
    [TestCase(@"c:/Tools/M602/x", @"c:\Tools\M602")]
    [TestCase(@"c:/Tools/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/M602",
      @"c:\Tools\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")]
    [TestCase(@"c:/Tools/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/M602/",
      @"c:\Tools\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\M602")]
    [TestCase(@"c:/Tools/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000/M602/x",
      @"c:\Tools\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\M602")]
    public void FilePathHelper_LongPathTolerantGetDirectoryNameTest_04(string strPath, string expected)
    {
        string actual = FilePathHelper.LongPathTolerantGetDirectoryName(strPath);
        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A test for LongPathTolerantGetDirectoryName which should succeed.
    /// The test tries just absolute paths, without drive letter
    /// </summary>
    [Test()]
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
            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    /// <summary>
    /// A test for LongPathTolerantGetDirectoryName which should succeed
    /// The test tries just relative paths
    /// </summary>
    [Test()]
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
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
    #endregion // LongPathTolerantGetDirectoryNameTest

    #region GetPathPartsTest

    /// <summary>
    /// A test for SafeGetDirectoryInfo which should succeed.
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [Test()]
    public void FilePathHelper_GetPathPartsTest_01()
    {
        var collection = FilePathHelper.GetPathParts(_320CharsAbsolutePath);
        Assert.That(collection, Has.Count.GreaterThan(0));
    }

    /// <summary>
    /// A test for GetPathParts which should succeed
    ///</summary>
    [Test()]
    public void FilePathHelper_GetPathPartsTest_02()
    {
        var collection = FilePathHelper.GetPathParts(null);

        Assert.That(collection, Has.Count.EqualTo(0));

    }

    /// <summary>
    /// A test for GetPathParts which should succeed
    ///</summary>
    [Test()]
    public void FilePathHelper_GetPathPartsTest_03()
    {
        string parentPath = @"C:\Tmp3\";
        string childPath = @"C:\Tmp3\xyz\CtrlSourceCode\";
        var collection = FilePathHelper.GetPathParts(FilePathHelper.GetRelativePath(parentPath, childPath));

        Assert.That(collection, Has.Count.EqualTo(2));
    }

    /// <summary>
    /// A test for GetPathParts which should succeed
    ///</summary>
    [Test()]
    public void FilePathHelper_GetPathPartsTest_04()
    {
        string parentPath = @"C:\Tmp3\abc";
        string childPath = @"C:\Tmp3\xyz\CtrlSourceCode";
        var collection = FilePathHelper.GetPathParts(FilePathHelper.GetRelativePath(parentPath, childPath));

        Assert.That(collection, Has.Count.EqualTo(0));
    }
    #endregion // GetPathPartsTest

    #region PathCompactTest

    /// <summary>
    /// A test for PathCompact which should succeed
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [Test()]
    [TestCase(48)]
    [TestCase(8)]
    public void FilePathHelper_PathCompactTest_01(int shortenTo)
    {
        string path = _320CharsAbsolutePath;
        string actual = FilePathHelper.PathCompact(path, shortenTo);
        Assert.That(actual, Has.Length.LessThanOrEqualTo(shortenTo));
    }

    /// <summary>
    /// A test for PathCompact which should succeed
    /// </summary>
    [Test()]
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
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
    #endregion // PathCompactTest

    #region SafeGetFileInfoTest

    /// <summary>
    /// A test for SafeGetFileInfo which should succeed.
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [Test()]
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

        FileInfo? expected = null;
        FileInfo actual = FilePathHelper.SafeGetFileInfo(strNonExistingFile);
        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A test for SafeGetFileInfo which should succeed
    /// </summary>
    [Test()]
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
        Assert.That(actual, Is.EqualTo(expected));
    }
    #endregion // SafeGetFileInfoTest

    #region SafeGetDirectoryInfoTest

    /// <summary>
    /// A test for SafeGetDirectoryInfo which should succeed.
    /// It tests that the method accepts input path longer than Win32.MAX_PATH without exception.
    /// </summary>
    [Test()]
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
        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A test for SafeGetDirectoryInfo which should succeed.
    /// It test that for non-existing directory the resulting value is null
    /// </summary>
    [Test()]
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
        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A test for SafeGetDirectoryInfo which should succeed.
    /// It test that for existing directory the resulting value is not null
    /// </summary>
    [Test()]
    public void FilePathHelper_SafeGetDirectoryInfoTest_03()
    {
        string strCurrentDir = Environment.CurrentDirectory;

        DirectoryInfo current = FilePathHelper.SafeGetDirectoryInfo(strCurrentDir);
        Assert.That(current, Is.Not.Null);
    }
    #endregion // SafeGetDirectoryInfoTest

    #region SafeGetFullName_Test

    /// <summary> A test for SafeGetFullName which should succeed. </summary>
    [Test()]
    public void FilePathHelper_SafeGetFullName_Test_01()
    {
        string strTmp = "aaa_999.txt";
        string strNonExistingFile = FilePathHelper.CombineNoChecks(_320CharsAbsolutePath, strTmp);

        FileInfo? info = new(strNonExistingFile);
        string fullName = FilePathHelper.SafeGetFullName(info);

        Assert.That(fullName, Is.EqualTo(strNonExistingFile));
    }
    #endregion // SafeGetFullName_Test

    #region GetLongestExistingDirectory_Test

    [Test, Description("A test for GetLongestExistingDirectory which checks if it returns null as expected.")]
    [TestCase(null!)]
    [TestCase("")]
    [TestCase("  ")]
    public void FilePathHelper_GetLongestExistingDirectory_Test_ReturnsNull(string inputPath)
    {
        // Act
        string actual = FilePathHelper.GetLongestExistingDirectory(inputPath);

        // Assert
        Assert.That(actual, Is.Null);
    }

    [Test, Description("A test for GetLongestExistingDirectory which checks if it returns expected value.")]
    public void FilePathHelper_GetLongestExistingDirectory_Test_ReturnsExpected()
    {
        // Arrange
        string systemDirectory = Environment.SystemDirectory;
        string inputPath1 = systemDirectory;
        string inputPath2 = Path.Combine(systemDirectory, Guid.NewGuid().ToString());

        // Act
        string actual1 = FilePathHelper.GetLongestExistingDirectory(inputPath1);
        string actual2 = FilePathHelper.GetLongestExistingDirectory(inputPath2);

        // Assert

        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual1, Is.EqualTo(systemDirectory));
            Assert.That(actual2, Is.EqualTo(systemDirectory));
        }
    }
    #endregion // GetLonestExistingDirectory_Test
    #endregion // Tests
}
#pragma warning restore IDE0305
#pragma warning restore IDE0057
#pragma warning restore VSSpell001
#pragma warning restore gVSSpell001
// Ignore Spelling: Utils, Comparers
//
using PK.PkUtils.Comparers;
using PK.PkUtils.IO;

#pragma warning disable IDE0039  // suppress "use local functions" warning

namespace PK.PkUtils.NUnitTests.ComparersTest;

/// <summary>
/// This is a test class for FunctionalComparer generic
///</summary>
[TestFixture()]
public class FunctionalComparerTest
{
    #region Tests

    /// <summary>
    /// A test for FunctionalComparer constructor
    /// </summary>
    [Test()]
    public void FunctionalComparer_Constructor_01()
    {
        Assert.Throws<ArgumentNullException>(() => new FunctionalComparer<int>(null));
    }

    /// <summary>
    /// A test for FunctionalComparer.Compare
    /// </summary>
    [Test()]
    public void FunctionalComparer_CompareTest_01()
    {
        Comparison<string> f = (x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        FunctionalComparer<string> comparer = new FunctionalComparer<string>(f);

        Assert.That(comparer.Compare("aaa", "AAA"), Is.EqualTo(0));
        Assert.That(comparer.Compare("aaa", "bbb"), Is.Not.EqualTo(0));
    }

    /// <summary>
    /// A test for FunctionalComparer.Compare
    /// </summary>
    [Test()]
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
        DirectoryInfo folder = new DirectoryInfo(strFolder);
        var filesAll = fsNonRecursive.SearchFiles(folder, "*.exe", SearchOption.AllDirectories).ToList();

        var fsSorted = filesAll.OrderBy<FileSystemInfo, FileSystemInfo>(fs => fs, fsComp);
        var fiSorted = filesAll.OrderBy<FileInfo, FileInfo>(fi => fi, fiComp);

        Assert.That(fsSorted.SequenceEqual(fiSorted), Is.True);
    }

    /// <summary>
    /// A test for FunctionalComparer.CreateNullSafeComparer
    /// </summary>
    [Test()]
    public void FunctionalComparer_CreateNullSafeComparerTest_01()
    {
        Comparison<string> f = null!;
        Assert.Throws<ArgumentNullException>(() => FunctionalComparer.CreateNullSafeComparer(f));
    }

    /// <summary>
    /// A test for FunctionalComparer.CreateNullSafeComparer
    /// </summary>
    [Test()]
    public void FunctionalComparer_CreateNullSafeComparerTest_02()
    {
        Comparison<string> f = (x, y) => (x.Length - y.Length);
        FunctionalComparer<string> comparer = FunctionalComparer.CreateNullSafeComparer(f);

        Assert.That(comparer.Compare("aaa", "AAA"), Is.EqualTo(0));
        Assert.That(comparer.Compare("aaa", "XYZ"), Is.EqualTo(0));

        Assert.That(comparer.Compare(null!, "pqr"), Is.LessThan(0));
        Assert.That(comparer.Compare("pqr", null!), Is.GreaterThan(0));

        Assert.That(comparer.Compare(null!, null!), Is.EqualTo(0));
        Assert.That(comparer.Compare(null!, string.Empty), Is.LessThan(0));
        Assert.That(comparer.Compare(string.Empty, null!), Is.GreaterThan(0));
    }
    #endregion // Tests
}

#pragma warning restore IDE0039

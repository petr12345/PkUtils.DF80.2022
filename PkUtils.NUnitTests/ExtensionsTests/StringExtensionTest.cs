// Ignore Spelling: PkUtils, Utils, Cloneable
// 
using PK.PkUtils.Extensions;


namespace PK.PkUtils.NUnitTests.ExtensionsTests;

/// <summary>
/// This is a test class for <see cref="StringExtension"/>.
/// </summary>
[TestFixture()]
public class StringExtensionTest
{
    #region Tests

    #region Tests_Left

    /// <summary> A test for Left that should throw ArgumentNullException. </summary>
    [Test()]
    public void StringExtension_Left_01()
    {
        string src = null!;

        Assert.Throws<ArgumentNullException>(() => src.Left(2));
    }

    /// <summary> A test for Left that should throw ArgumentOutOfRangeException. </summary>
    [Test()]
    public void StringExtension_Left_02()
    {
        string src = "abc";
        Assert.Throws<ArgumentOutOfRangeException>(() => src.Left(-2));
    }

    /// <summary> A test for Left that should throw ArgumentOutOfRangeException. </summary>
    [Test()]
    public void StringExtension_Left_03()
    {
        const string src = "abcd";
        Assert.Throws<ArgumentOutOfRangeException>(() => src.Left(100));
    }

    /// <summary> A test for Left that should succeed. </summary>
    [Test()]
    public void StringExtension_Left_04()
    {
        const string src = "abcd";
        string expected, actual;

        expected = string.Empty;
        actual = src.Left(0);
        Assert.That(actual, Is.EqualTo(expected));

        expected = "a";
        actual = src.Left(1);
        Assert.That(actual, Is.EqualTo(expected));
    }
    #endregion // Tests_Left

    #region Tests_LeftMax

    /// <summary> A test for LeftMax that should throw ArgumentNullException. </summary>
    [Test()]
    public void StringExtension_LeftMax_01()
    {
        string src = null!;
        Assert.Throws<ArgumentNullException>(() => src.LeftMax(2));
    }

    /// <summary> A test for LeftMax that should throw ArgumentOutOfRangeException. </summary>
    [Test()]
    public void StringExtension_LeftMax_02()
    {
        string src = "abc";

        Assert.Throws<ArgumentOutOfRangeException>(() => src.LeftMax(-2));
    }

    /// <summary> A test for LeftMax that should succeed. </summary>
    [Test()]
    public void StringExtension_LeftMax_03()
    {
        const string src = "abcd";
        string expected, actual;

        expected = string.Empty;
        actual = src.LeftMax(0);
        Assert.That(actual, Is.EqualTo(expected));

        expected = "a";
        actual = src.LeftMax(1);
        Assert.That(actual, Is.EqualTo(expected));

        expected = src;
        actual = src.LeftMax(100);
        Assert.That(actual, Is.EqualTo(expected));
    }
    #endregion // Tests_LeftMax

    #region Tests_Right

    /// <summary> A test for Right that should throw ArgumentNullException. </summary>
    [Test()]
    public void StringExtension_Right_01()
    {
        string src = null!;

        Assert.Throws<ArgumentNullException>(() => src.Right(2));
    }

    /// <summary> A test for Right that should throw ArgumentOutOfRangeException. </summary>
    [Test()]
    public void StringExtension_Right_02()
    {
        string src = "abc";
        Assert.Throws<ArgumentOutOfRangeException>(() => src.Right(-2));
    }

    /// <summary> A test for Right that should throw ArgumentOutOfRangeException. </summary>
    [Test()]
    public void StringExtension_Right_03()
    {
        const string src = "abcd";
        Assert.Throws<ArgumentOutOfRangeException>(() => src.Right(100));
    }

    /// <summary> A test for Right that should succeed. </summary>
    [Test()]
    public void StringExtension_Right_04()
    {
        const string src = "abcd";
        string expected, actual;

        expected = string.Empty;
        actual = src.Right(0);
        Assert.That(actual, Is.EqualTo(expected));

        expected = "d";
        actual = src.Right(1);
        Assert.That(actual, Is.EqualTo(expected));
    }
    #endregion // Tests_Right

    #region Tests_RightMax

    /// <summary> A test for RightMax that should throw ArgumentNullException. </summary>
    [Test()]
    public void StringExtension_RightMax_01()
    {
        string src = null!;
        Assert.Throws<ArgumentNullException>(() => src.RightMax(2));
    }

    /// <summary> A test for RightMax that should throw ArgumentOutOfRangeException. </summary>
    [Test()]
    public void StringExtension_RightMax_02()
    {
        string src = "abc";
        Assert.Throws<ArgumentOutOfRangeException>(() => src.RightMax(-2));
    }

    /// <summary> A test for RightMax that should succeed. </summary>
    [Test()]
    public void StringExtension_RightMax_03()
    {
        const string src = "abcd";
        string expected, actual;

        expected = string.Empty;
        actual = src.RightMax(0);
        Assert.That(actual, Is.EqualTo(expected));

        expected = "d";
        actual = src.RightMax(1);
        Assert.That(actual, Is.EqualTo(expected));

        expected = src;
        actual = src.RightMax(100);
        Assert.That(actual, Is.EqualTo(expected));
    }
    #endregion // Tests_RightMax

    #endregion // Tests
}

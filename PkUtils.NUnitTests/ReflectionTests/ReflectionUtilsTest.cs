// Ignore Spelling: PkUtils, Utils
// 
using PK.PkUtils.Reflection;


namespace PK.PkUtils.NUnitTests.ReflectionTests;

/// <summary>
/// This is a test class for ReflectionUtils and is intended
/// to contain all ReflectionUtils Unit Tests
/// </summary>
[TestFixture()]
public class ReflectionUtilsTest
{
    #region Auxiliary_code_for_tests

    [System.ComponentModel.Description("My description of my enum")]
    public enum ErrorStatus
    {
        Critical,
        Fatal,
        Medium,
        Low,
    }
    #endregion // Auxiliary_code_for_tests

    #region ReflectionUtils_tests

    /// <summary>
    /// A test for GetDescriptionForType
    /// </summary>
    [Test()]
    public void ReflectionUtils_GetDescriptionForTypeTest()
    {
        Type t;
        System.ComponentModel.DescriptionAttribute expected, actual;

        t = typeof(ErrorStatus);
        expected = new System.ComponentModel.DescriptionAttribute("My description of my enum");
        actual = ReflectionUtils.GetDescriptionForType(t);
        Assert.That(actual, Is.EqualTo(expected));

        t = typeof(int);
        expected = null!;
        actual = ReflectionUtils.GetDescriptionForType(t);
        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A test for SafeGetType
    /// </summary>
    [Test()]
    public void ReflectionUtils_SafeGetTypeTest()
    {
        object obj = null!;
        Type expected = null!;
        Type actual = ReflectionUtils.SafeGetType(obj);

        Assert.That(actual, Is.EqualTo(expected));

        obj = new OutOfMemoryException();
        expected = typeof(OutOfMemoryException);
        actual = ReflectionUtils.SafeGetType(obj);
        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A test for SafeGetTypeName
    /// </summary>
    [Test()]
    public void ReflectionUtils_SafeGetTypeNameTest()
    {
        object obj;
        string expected, actual;

        obj = null!;
        expected = "<null>";
        actual = ReflectionUtils.SafeGetTypeName(obj);
        Assert.That(actual, Is.EqualTo(expected));

        obj = new System.Windows.Forms.Form();
        expected = "Form";
        actual = ReflectionUtils.SafeGetTypeName(obj);
        Assert.That(actual, Is.EqualTo(expected));
    }
    #endregion // ReflectionUtils_tests
}

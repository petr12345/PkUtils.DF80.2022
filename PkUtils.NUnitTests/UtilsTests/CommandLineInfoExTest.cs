// Ignore Spelling: Utils
// 

using PK.PkUtils.Interfaces;
using PK.PkUtils.Utils;


namespace PK.PkUtils.NUnitTests.UtilsTests;

/// <summary>
/// This is a test class for CommandLineInfoEx and is intended
/// to contain all CommandLineInfoEx Unit Tests
///</summary>
[TestFixture()]
public class CommandLineInfoExTest
{
    #region Tests

    /// <summary>
    /// A test for CommandLineInfoEx constructor, which should fail with ArgumentNullException
    /// </summary>
    [Test()]
    public void CommandLineInfoEx_Constructor_01()
    {
        Assert.Throws<ArgumentNullException>(() => new CommandLineInfoEx(null));
    }

    /// <summary>
    /// A test for CommandLineInfoEx constructor, which should fail with ArgumentException
    /// </summary>
    [Test()]
    public void CommandLineInfoEx_Constructor_02()
    {
        CommandLineInfoEx info;
        var args = new string[] { "doc1.rtf", null!, "doc2.rtf" };

        Assert.Throws<ArgumentException>(() => info = new CommandLineInfoEx(args));
    }

    /// <summary>
    /// A test for CommandLineInfoEx constructor, which should succeed
    /// </summary>
    [Test()]
    public void CommandLineInfoEx_Constructor_03()
    {
        CommandLineInfoEx info = new CommandLineInfoEx([]);
        Assert.That(info.IsEmpty, Is.True);
    }

    /// <summary>
    /// A test for CommandLineInfoEx constructor, which should succeed
    /// </summary>
    [Test()]
    public void CommandLineInfoEx_Constructor_04()
    {
        var args = new string[] { "/InputFile", "doc1.rtf", "nologo" };
        CommandLineInfoEx info = new CommandLineInfoEx(args);
        Assert.That(info.IsEmpty, Is.False);
    }

    /// <summary>
    /// A test for CommandLineInfoEx.ParseCommandLine that is case-sensitive, and should succeed
    /// </summary>
    [Test()]
    public void CommandLineInfoEx_ParseCommandLine_01()
    {
        var args = new string[] { "/InputFile", "doc1.rtf", "nologo" };
        CommandLineInfoEx info = new CommandLineInfoEx();

        info.ParseCommandLine(args);
        Assert.Multiple(() =>
        {
            Assert.That(info.GetSwitch("nologo"), Is.True);
            Assert.That(info.GetSwitch("NoLogo"), Is.False);

            Assert.That(info.GetOption("InputFile", out string strVal), Is.True);
            Assert.That(strVal, Is.EqualTo("doc1.rtf"));
        });
    }

    /// <summary>
    /// A test for CommandLineInfoEx.ParseCommandLine that is case insensitive, and should succeed
    /// </summary>
    [Test()]
    public void CommandLineInfoEx_ParseCommandLine_02()
    {
        var args = new string[] { "/InputFile", "doc1.rtf", "nologo" };
        CommandLineInfoEx info = new CommandLineInfoEx(false, string.Empty);

        info.ParseCommandLine(args);
        Assert.Multiple(() =>
        {
            Assert.That(info.GetSwitch("nologo"), Is.True);
            Assert.That(info.GetSwitch("NoLogO"), Is.True);

            Assert.That(info.GetOption("iNpUtFiLe", out string strVal), Is.True);
            Assert.That(strVal, Is.EqualTo("DOC1.RTF").IgnoreCase);
        });
    }

    /// <summary> A test for CommandLineInfoEx.DeepClone. </summary>
    [Test()]
    public void CommandLineInfoEx_DeepClone_01()
    {
        var args = new string[] { "/InputFile", "doc1.rtf", "nologo" };
        CommandLineInfoEx info1 = new CommandLineInfoEx(args, true, string.Empty);
        CommandLineInfoEx info2 = info1.DeepClone();

        Assert.Multiple(() =>
        {
            Assert.That(info1.GetSwitch("nologo"), Is.True);
            Assert.That(info2.GetSwitch("nologo"), Is.True);
        });
    }

    /// <summary> A test for covariance of IDeepCloneable ( compilation of assignment ). </summary>
    [Test()]
    public void CommandLineInfoEx_InterfaceCovariance()
    {
        IDeepCloneable<CommandLineInfoEx> x = new CommandLineInfoEx();
        // makes sense, as CommandLineInfoEx derived from object, but covariance is needed to let compiler allow that
        IDeepCloneable<object> y = x;
        Assert.That(y, Is.Not.Null);
    }
    #endregion // Tests
}

// Ignore Spelling: Utils
//
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Interfaces;
using PK.PkUtils.Utils;

namespace PK.PkUtils.UnitTests.UtilsTests;

/// <summary>
/// This is a test class for CommandLineInfoEx and is intended
/// to contain all CommandLineInfoEx Unit Tests
///</summary>
[TestClass()]
public class CommandLineInfoExTest
{
    #region Tests

    /// <summary>
    /// A test for CommandLineInfoEx constructor, which should fail with ArgumentNullException
    /// </summary>
    [TestMethod()]
    public void CommandLineInfoEx_Constructor_01()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            CommandLineInfoEx info = new(null);
        });
    }

    /// <summary>
    /// A test for CommandLineInfoEx constructor, which should fail with ArgumentException
    /// </summary>
    [TestMethod()]
    public void CommandLineInfoEx_Constructor_02()
    {
        var args = new string[] { "doc1.rtf", null!, "doc2.rtf" };
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            CommandLineInfoEx info = new(args);
        });
    }

    /// <summary>
    /// A test for CommandLineInfoEx constructor, which should succeed
    /// </summary>
    [TestMethod()]
    public void CommandLineInfoEx_Constructor_03()
    {
        CommandLineInfoEx info = new(Enumerable.Empty<string>());
        Assert.IsTrue(info.IsEmpty);
    }

    /// <summary>
    /// A test for CommandLineInfoEx constructor, which should succeed
    /// </summary>
    [TestMethod()]
    public void CommandLineInfoEx_Constructor_04()
    {
        var args = new string[] { "/InputFile", "doc1.rtf", "nologo" };
        CommandLineInfoEx info = new(args);
        Assert.IsFalse(info.IsEmpty);
    }

    /// <summary>
    /// A test for CommandLineInfoEx.ParseCommandLine that is case-sensitive, and should succeed
    /// </summary>
    [TestMethod()]
    public void CommandLineInfoEx_ParseCommandLine_01()
    {
        var args = new string[] { "/InputFile", "doc1.rtf", "nologo" };
        CommandLineInfoEx info = new();

        info.ParseCommandLine(args);
        Assert.IsTrue(info.GetSwitch("nologo"));
        Assert.IsFalse(info.GetSwitch("NoLogo"));

        Assert.IsTrue(info.GetOption("InputFile", out string strVal));
        Assert.AreEqual("doc1.rtf", strVal);
    }

    /// <summary>
    /// A test for CommandLineInfoEx.ParseCommandLine that is case insensitive, and should succeed
    /// </summary>
    [TestMethod()]
    public void CommandLineInfoEx_ParseCommandLine_02()
    {
        var args = new string[] { "/InputFile", "doc1.rtf", "nologo" };
        CommandLineInfoEx info = new(false, string.Empty);

        info.ParseCommandLine(args);
        Assert.IsTrue(info.GetSwitch("nologo"));
        Assert.IsTrue(info.GetSwitch("NoLogO"));

        Assert.IsTrue(info.GetOption("iNpUtFiLe", out string strVal));
        Assert.AreEqual("DOC1.RTF", strVal);
    }

    /// <summary> A test for CommandLineInfoEx.DeepClone. </summary>
    [TestMethod()]
    public void CommandLineInfoEx_DeepClone_01()
    {
        var args = new string[] { "/InputFile", "doc1.rtf", "nologo" };
        CommandLineInfoEx info1 = new(args, true, string.Empty);
        CommandLineInfoEx info2 = info1.DeepClone();

        Assert.IsTrue(info1.GetSwitch("nologo"));
        Assert.IsTrue(info2.GetSwitch("nologo"));
    }

    /// <summary> A test for covariance of IDeepCloneable ( compilation of assignment ). </summary>
    [TestMethod()]
    public void CommandLineInfoEx_InterfaceCovariance()
    {
        IDeepCloneable<CommandLineInfoEx> x = new CommandLineInfoEx();
        // makes sense, as CommandLineInfoEx derived from object, but covariance is needed to let compiler allow that
        IDeepCloneable<object> y = x;
    }
    #endregion // Tests
}

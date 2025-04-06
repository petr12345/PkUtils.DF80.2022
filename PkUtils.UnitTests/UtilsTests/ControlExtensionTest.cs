// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.UnitTests.UtilsTests;

/// <summary>
/// This is a test class for ControlExtension and is intended
/// to contain all ControlExtension Unit Tests
///</summary>
[TestClass()]
public class ControlExtensionTest
{
    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion

    #region Tests

    /// <summary>
    /// A test for AllControls
    /// </summary>
    [TestMethod()]
    public void ControlExtension_AllControlsTest()
    {
        Control parent = new Form();
        IEnumerable<Control> expected = [];
        /* IEnumerable<Control> actual = ControlExtension.AllControls(parent); */
        IEnumerable<Control> actual = parent.AllControls();

        // If you want to ensure to collections have exactly the same set of members regardless of order, 
        // you can use:
        var areEquivalent_1 = (expected.Count() == actual.Count()) && !expected.Except(actual).Any();
        Assert.IsTrue(areEquivalent_1);

        // If you want to ensure two collections have the same distinct set of members regardless of order,
        // (where duplicates in either are ignored), you can use:
        // check that [(A-B) Union (B-A)] is empty
        var areEquivalent_2 = !expected.Except(actual).Union(actual.Except(expected)).Any();
        Assert.IsTrue(areEquivalent_2);
    }

    /// <summary>
    /// A test for ControlExtension.CheckNotDisposed
    /// </summary>
    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ControlExtension_CheckNotDisposedTest_01()
    {
        Control ctrl = null!;

        ctrl.CheckNotDisposed(nameof(ctrl));
    }

    /// <summary>
    /// A test for ControlExtension.CheckNotDisposed
    ///</summary>
    [TestMethod()]
    [ExpectedException(typeof(System.ObjectDisposedException))]
    public void ControlExtension_CheckNotDisposedTest_02()
    {
        Control ctrl = new Form();

        ctrl.Dispose();
        ctrl.CheckNotDisposed("ctrl");
    }

    /// <summary>
    /// A test for ControlExtension.CheckNotDisposed
    /// </summary>
    [TestMethod()]
    public void ControlExtension_CheckNotDisposedTest_03()
    {
        Control ctrl = new Form();

        ctrl.CheckNotDisposed("ctrl");
    }
    #endregion // Tests
}

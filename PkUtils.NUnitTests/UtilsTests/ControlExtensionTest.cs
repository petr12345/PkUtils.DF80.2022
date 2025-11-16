// Ignore Spelling: PkUtils, Utils, Cloneable
// 
using System.Windows.Forms;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.NUnitTests.UtilsTests;

/// <summary>
/// This is a test class for ControlExtensions and is intended
/// to contain all ControlExtensions Unit Tests
///</summary>
[TestFixture()]
public class ControlExtensionTest
{
    #region Tests

    /// <summary>
    /// A test for AllControls
    /// </summary>
    [Test()]
    public void ControlExtension_AllControlsTest()
    {
        // Arrange
        Control parent = new Form();
        IEnumerable<Control> expected = [];
        /* IEnumerable<Control> actual = ControlExtensions.AllControls(parent); */
        IEnumerable<Control> actual = parent.AllControls();

        // Act
        // If you want to ensure to collections have exactly the same set of members regardless of order, 
        // you can use:
        var areEquivalent_1 = (expected.Count() == actual.Count()) && !expected.Except(actual).Any();
        // Assert
        Assert.That(areEquivalent_1, Is.True);

        // Act
        // If you want to ensure two collections have the same distinct set of members regardless of order,
        // (where duplicates in either are ignored), you can use:
        // check that [(A-B) Union (B-A)] is empty
        var areEquivalent_2 = !expected.Except(actual).Union(actual.Except(expected)).Any();
        // Assert
        Assert.That(areEquivalent_2, Is.True);
    }

    /// <summary>   A test for ControlExtensions.CheckNotDisposed. </summary>
    [Test()]
    public void ControlExtension_CheckNotDisposedTest_01()
    {
        Control ctrl = null!;

        Assert.Throws<ArgumentNullException>(() => ctrl.CheckNotDisposed(nameof(ctrl)));
    }

    /// <summary>
    /// A test for ControlExtensions.CheckNotDisposed
    ///</summary>
    [Test()]
    public void ControlExtension_CheckNotDisposedTest_02()
    {
        Control ctrl = new Form();

        ctrl.Dispose();
        Assert.Throws<ObjectDisposedException>(() => ctrl.CheckNotDisposed(nameof(ctrl)));
    }

    /// <summary>
    /// A test for ControlExtensions.CheckNotDisposed
    /// </summary>
    [Test()]
    public void ControlExtension_CheckNotDisposedTest_03()
    {
        Control ctrl = new Form();

        ctrl.CheckNotDisposed();
    }
    #endregion // Tests
}

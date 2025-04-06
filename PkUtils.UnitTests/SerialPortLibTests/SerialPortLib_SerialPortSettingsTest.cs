// Ignore Spelling: Utils
//

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PK.PkUtils.SerialPortLib;

namespace PK.PkUtils.UnitTests.SerialPortLibTests;

/// <summary>
///This is a test class for EventsUtils and is intended
///to contain all EventsUtilsTest Unit Tests
///</summary>
[TestClass()]
public class SerialPortLib_SerialPortSettingsTest
{
    #region Tests

    /// <summary>
    /// A basic test for SerialPortSettingsBase, which should succeed
    ///</summary>
    [TestMethod()]
    public void SerialPortSettingsBase_DeepCloneTest_01()
    {
        SerialPortSettingsBase b1 = new();
        object b2 = b1.DeepClone();

        Assert.AreEqual(b1.GetType(), b2.GetType());
        Assert.IsTrue(b1.Equals(b2));
        Assert.IsTrue(b1.GetHashCode().Equals(b2.GetHashCode()));
    }

    /// <summary>
    /// A basic test for SerialPortSettings, which should succeed
    ///</summary>
    [TestMethod()]
    public void SerialPortSettings_DeepCloneTest_01()
    {
        SerialPortSettings s1 = new();
        object s2 = s1.DeepClone();

        Assert.AreEqual(s1.GetType(), s2.GetType());
        Assert.IsTrue(s1.Equals(s2));
        Assert.IsTrue(s1.GetHashCode().Equals(s2.GetHashCode()));
    }

    /// <summary>
    /// A basic test for SerialPortSettingsEx, which should succeed
    ///</summary>
    public void SerialPortSettingsEx_DeepCloneTest_01()
    {
        SerialPortSettingsEx sex1 = new();
        object sex2 = sex1.DeepClone();

        Assert.AreEqual(sex1.GetType(), sex2.GetType());
        Assert.IsTrue(sex1.Equals(sex2));
        Assert.IsTrue(sex1.GetHashCode().Equals(sex2.GetHashCode()));
    }
    #endregion // Tests
}

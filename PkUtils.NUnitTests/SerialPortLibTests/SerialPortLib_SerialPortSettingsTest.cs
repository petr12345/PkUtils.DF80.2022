// Ignore Spelling: Utils
// 

using PK.PkUtils.SerialPortLib;

namespace PK.PkUtils.NUnitTests.SerialPortLibTests;

/// <summary>
///This is a test class for EventsUtils and is intended
///to contain all EventsUtilsTest Unit Tests
///</summary>
[TestFixture()]
public class SerialPortLib_SerialPortSettingsTest
{
    #region Tests

    /// <summary>
    /// A basic test for SerialPortSettingsBase, which should succeed
    ///</summary>
    [Test()]
    public void SerialPortSettingsBase_DeepCloneTest_01()
    {
        SerialPortSettingsBase b1 = new SerialPortSettingsBase();
        object b2 = b1.DeepClone();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(b2, Is.InstanceOf<SerialPortSettingsBase>());
            Assert.That(b1.Equals(b2));
            Assert.That(b1.GetHashCode(), Is.EqualTo(b2.GetHashCode()));
        }
    }

    /// <summary>
    /// A basic test for SerialPortSettings, which should succeed
    ///</summary>
    [Test()]
    public void SerialPortSettings_DeepCloneTest_01()
    {
        SerialPortSettings s1 = new SerialPortSettings();
        object s2 = s1.DeepClone();

        Assert.That(s1.GetType(), Is.EqualTo(s2.GetType()));
        Assert.That(s1, Is.EqualTo(s2));
        Assert.That(s1.GetHashCode(), Is.EqualTo(s2.GetHashCode()));
    }

    /// <summary>
    /// A basic test for SerialPortSettingsEx, which should succeed
    ///</summary>
    public static void SerialPortSettingsEx_DeepCloneTest_01()
    {
        SerialPortSettingsEx sex1 = new SerialPortSettingsEx();
        object sex2 = sex1.DeepClone();

        Assert.That(sex2.GetType(), Is.EqualTo(sex1.GetType()));
        Assert.That(sex1.Equals(sex2), Is.True);
        Assert.That(sex1.GetHashCode(), Is.EqualTo(sex2.GetHashCode()));
    }
    #endregion // Tests
}

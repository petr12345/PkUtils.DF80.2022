// Ignore Spelling: Utils
// 
using System.Reflection;
using System.Windows.Forms;
using PK.PkUtils.Reflection;

namespace PK.PkUtils.NUnitTests.ReflectionTests;

/// <summary>
/// This is a test class for EventsUtils and is intended
/// to contain all EventsUtils Unit Tests
///</summary>
[TestFixture()]
public class EventsUtilsTest
{
    #region Events_Utils_Tests

    /// <summary>
    /// An auxiliary class for test of RemoveEventHandler
    ///</summary>
    public class TestSubscriber
    {
        private bool _bHandlerCalled;

        public bool HandlerCalled { get => _bHandlerCalled; set => _bHandlerCalled = value; }

        public void OnSelectedIndexChanged(object? sender, EventArgs e)
        {
            _bHandlerCalled = true;
        }
    }

    /// <summary>
    /// A test of RemoveEventHandler
    ///</summary>
    [Test()]
    public void EventsUtils_RemoveEventHandlerTest()
    {
        ComboBox cb = new System.Windows.Forms.ComboBox();
        cb.Items.AddRange(new object[] { "aaa", "bbb", "ccc", "ddd" });
        TestSubscriber ts = new TestSubscriber();

        // subscribe 
        cb.SelectedIndexChanged += new EventHandler(ts.OnSelectedIndexChanged);
        // check the effect of it
        cb.SelectedIndex = 1;
        Assert.That(cb.SelectedIndex, Is.EqualTo(1));
        Assert.That(ts.HandlerCalled, Is.True);

        // unsubscribe 
        MethodInfo mInfo = ts.GetType().GetInstanceMethod(nameof(ts.OnSelectedIndexChanged));
        cb.RemoveEventHandler<EventArgs>(nameof(cb.SelectedIndexChanged), ts, mInfo);

        // check the effect of it
        ts.HandlerCalled = false;
        cb.SelectedIndex = 3;
        Assert.That(cb.SelectedIndex, Is.EqualTo(3));
        Assert.That(ts.HandlerCalled, Is.False);
    }
    #endregion // Events_Utils_Tests
}

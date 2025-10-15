// Ignore Spelling: Utils
// 
using System;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Reflection;

namespace PK.PkUtils.UnitTests.ReflectionTests
{
    /// <summary>
    /// This is a test class for EventsUtils and is intended
    /// to contain all EventsUtils Unit Tests
    ///</summary>
    [TestClass()]
    public class EventsUtilsTest
    {
        #region Events_Utils_Tests

        /// <summary>
        /// An auxiliary class for test of RemoveEventHandler
        ///</summary>
        public class TestSubscriber
        {
            public bool _bHandlerCalled;

            public void OnSelectedIndexChanged(object? sender, EventArgs e)
            {
                _bHandlerCalled = true;
            }
        }

        /// <summary>
        /// A test of RemoveEventHandler
        ///</summary>
        [TestMethod()]
        public void EventsUtils_RemoveEventHandlerTest()
        {
            ComboBox cb = new();
            cb.Items.AddRange(new object[] { "aaa", "bbb", "ccc", "ddd" });
            TestSubscriber ts = new();

            // subscribe 
            cb.SelectedIndexChanged += new EventHandler(ts.OnSelectedIndexChanged);
            // check the effect of it
            ts._bHandlerCalled = false;
            cb.SelectedIndex = 1;
            Assert.AreEqual(1, cb.SelectedIndex);
            Assert.IsTrue(ts._bHandlerCalled);

            // unsubscribe 
            MethodInfo mInfo = ts.GetType().GetInstanceMethod(nameof(ts.OnSelectedIndexChanged));
            cb.RemoveEventHandler<EventArgs>(nameof(cb.SelectedIndexChanged), ts, mInfo);

            // check the effect of it
            ts._bHandlerCalled = false;
            cb.SelectedIndex = 3;
            Assert.AreEqual(3, cb.SelectedIndex);
            Assert.IsFalse(ts._bHandlerCalled);
        }
        #endregion // Events_Utils_Tests
    }
}

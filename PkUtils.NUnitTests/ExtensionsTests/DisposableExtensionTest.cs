// Ignore Spelling: PkUtils, Utils
// 
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.NUnitTests.ExtensionsTests
{
    /// <summary>
    /// This is a test class for <see cref="DisposableExtension"/>.
    /// </summary>
    [TestFixture()]
    public class DisposableExtensionTest
    {
        #region Auxiliary_types

        /// <summary>
        /// An example of Form-derived class implementing IDisposableEx
        /// </summary>
        public class MySpecificForm : Form, IDisposableEx
        {
            #region IDisposableEx Members
            bool IDisposableEx.IsDisposed
            {
                get { return this.IsDisposed; }
            }
            #endregion
        }
        #endregion // Auxiliary_types

        #region Tests

        /// <summary> A test for DisposableExtension.CheckNotDisposed. </summary>
        [Test()]
        public void DisposableExtension_CheckNotDisposedTest_01()
        {
            IDisposableEx obj = null!;

            Assert.Throws<ArgumentNullException>(() => obj.CheckNotDisposed(nameof(obj)));
        }

        /// <summary> A test for DisposableExtension.CheckNotDisposed. </summary>
        [Test()]
        public void DisposableExtension_CheckNotDisposedTest_02()
        {
            IDisposableEx form = new MySpecificForm();

            form.Dispose();

            Assert.Throws<ObjectDisposedException>(() => form.CheckNotDisposed(nameof(form)));
        }

        /// <summary> A test for DisposableExtension.CheckNotDisposed. </summary>
        [Test()]
        public void DisposableExtension_CheckNotDisposedTest_03()
        {
            IDisposableEx obj = new MySpecificForm();

            obj.CheckNotDisposed(nameof(obj));
        }
        #endregion // Tests
    }
}

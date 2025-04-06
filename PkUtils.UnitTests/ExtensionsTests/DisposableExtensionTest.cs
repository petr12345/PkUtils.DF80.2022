﻿// Ignore Spelling: Utils
//
using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.UnitTests.ExtensionsTests
{
    /// <summary>
    /// This is a test class for DisposableExtension and is intended
    /// to contain all DisposableExtensionTest Unit Tests
    /// </summary>
    [TestClass()]
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

        /// <summary>
        /// A test for DisposableExtension.CheckNotDisposed
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DisposableExtension_CheckNotDisposedTest_01()
        {
            IDisposableEx obj = null!;

            obj.CheckNotDisposed(nameof(obj));
        }

        /// <summary>
        /// A test for DisposableExtension.CheckNotDisposed
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(System.ObjectDisposedException))]
        public void DisposableExtension_CheckNotDisposedTest_02()
        {
            IDisposableEx form = new MySpecificForm();

            form.Dispose();
            form.CheckNotDisposed("form");
        }

        /// <summary>
        /// A test for DisposableExtension.CheckNotDisposed
        /// </summary>
        [TestMethod()]
        public void DisposableExtension_CheckNotDisposedTest_03()
        {
            IDisposableEx obj = new MySpecificForm();

            obj.CheckNotDisposed(nameof(obj));
        }
        #endregion // Tests
    }
}

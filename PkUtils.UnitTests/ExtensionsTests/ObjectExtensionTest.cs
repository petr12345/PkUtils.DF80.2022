// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.UnitTests.ExtensionsTests
{

    /// <summary>
    /// This is a test class for ObjectExtension and is intended
    /// to contain all ObjectExtensionTest Unit Tests
    /// </summary>
    [TestClass()]
    public class ObjectExtensionTest
    {
        #region Auxiliary_types

        private class Person
        {
            private readonly int _age;
            private readonly string _name;

            public Person(int age, string name)
            {
                ArgumentNullException.ThrowIfNull(name);
                this._age = age;
                this._name = name;
            }

            public int Age => _age;

            public string Name => _name;

            public override string ToString()
            {
                return $"{Name} of age {Age}";
            }
        }

        /// <summary>
        /// An example of class implementing IDisposableEx
        /// </summary>
        public class MyDisposableClass : IDisposableEx
        {
            private bool _bDisposed;

            public bool IsDisposed
            {
                get { return _bDisposed; }
            }

            public void Dispose()
            {
                if (!IsDisposed)
                {
                    GC.SuppressFinalize(this);
                    _bDisposed = true;
                }
            }
        }

        /// <summary>
        /// An example of structure implementing IDisposableEx
        /// </summary>
        public struct MyDisposableStruct : IDisposableEx
        {
            private bool _bDisposed;

            public bool IsDisposed
            {
                get { return _bDisposed; }
            }

            public void Dispose()
            {
                if (!IsDisposed)
                {
                    GC.SuppressFinalize(this);
                    _bDisposed = true;
                }
            }
        }
        #endregion // Auxiliary_types

        #region Tests

        [TestMethod()]
        public void AsStringTest()
        {
            Person p1 = null!;
            Person p2 = new(32, "Paul");

            string s1 = p1.AsString();
            string s2 = p2.AsString();

            Assert.AreEqual("<null>", s1);
            Assert.AreEqual("Paul of age 32", s2);
        }

        [TestMethod()]
        public void PropertyListTest()
        {
            Person p1 = null!;
            Person p2 = new(32, "Paul");

            string s1 = p1.PropertyList();
            string s2 = p2.PropertyList();

            Assert.AreEqual("<null>", s1);
            Assert.AreEqual("Age: 32, Name: Paul", s2);
        }


        /// <summary>
        ///A test for ObjectExtension.CheckNotDisposed
        /// </summary>
        [TestMethod()]
        public void CheckNotDisposedTest_21()
        {
            object obj = null!;
            Assert.ThrowsExactly<ArgumentNullException>(() => obj.CheckNotDisposed(nameof(obj)));
        }

        /// <summary>
        ///A test for ObjectExtension.CheckNotDisposed
        /// </summary>
        [TestMethod()]
        public void CheckNotDisposedTest_22()
        {
            object obj = new MyDisposableClass();
            (obj as IDisposable)!.Dispose();
            Assert.ThrowsExactly<ObjectDisposedException>(() => obj.CheckNotDisposed(nameof(obj)));
        }

        /// <summary>
        ///A test for ObjectExtension.CheckNotDisposed
        /// </summary>
        [TestMethod()]
        public void CheckNotDisposedTest_23()
        {
            object obj = new MyDisposableClass();
            obj.CheckNotDisposed(nameof(obj));
        }

        /// <summary>
        ///A test for ObjectExtension.CheckNotDisposed
        /// </summary>
        [TestMethod()]
        public void CheckNotDisposedTest_24()
        {
            object obj = new MyDisposableStruct();
            obj.CheckNotDisposed(nameof(obj));
        }

        /// <summary>
        ///A test for ObjectExtension.CheckArgNotNull
        /// </summary>
        [TestMethod()]
        public void ObjectExtension_CheckArgNotNullTest_01()
        {
            System.Windows.Forms.Form f = null!;
            Assert.ThrowsExactly<ArgumentNullException>(() => f.CheckArgNotNull("f"));
        }

        /// <summary>
        /// A test for ObjectExtension.CheckArgNotNull
        /// </summary>
        [TestMethod()]
        public void ObjectExtension_CheckArgNotNullTest_02()
        {
            Form f = new();
            f.CheckArgNotNull();
        }

        /*
         * Should produce a compilation error:
         * 'The type 'System.Drawing.Size' must be a reference type in order to use it as parameter 'T'
         * in the generic type or method.
         * 
        [TestMethod()]
        public void CheckArgNotNullTest_03()
        {
          System.Drawing.Size s = new System.Drawing.Size();

          ObjectExtension.CheckArgNotNull<System.Drawing.Size>(s, "s");
        }
        */

        [TestMethod()]
        public void ObjectExtension_CheckNotNullTest_01()
        {
            Form f = null!;
            Assert.ThrowsExactly<InvalidOperationException>(() => f.CheckNotNull(nameof(f)));
        }

        /// <summary>
        /// A test for ObjectExtension.CheckNotNull
        /// </summary>
        [TestMethod()]
        public void ObjectExtension_CheckNotNullTest_02()
        {
            Form f = new();
            f.CheckNotNull("f");
        }

#nullable disable // Suppress   "warning CS8602: Dereference of a possibly null reference." ?
        /// <summary>
        /// A test for
        /// TResult NullSafe<T, TResult>(this T target, Func<T, TResult> func) where T : class
        /// </summary>
        [TestMethod()]
        public void ObjectExtension_NullSafeTest_01()
        {
            List<Component> list = [
                new OpenFileDialog(), null, new Form(), null, new Control() ];
            StringBuilder sb = new();

            foreach (Component comp in list)
            {
                sb.Append((comp as Form).NullSafe(f => f.Text));
            }
        }

        /// <summary>
        /// A test for
        /// TResult NullSafe<T, TResult>(this T target, Func<T, TResult> func) where T : class
        /// </summary>
        [TestMethod()]
        public void ObjectExtension_NullSafeTest_02()
        {
            List<Component> list = [
                new OpenFileDialog(), null, new Form(), null, new Control() ];

            IEnumerable<Component> subList = list.Where(
              comp => !string.IsNullOrEmpty((comp as Control).NullSafe(c => c.Text)));

            Assert.IsFalse(subList.Any());
        }
#nullable restore

        /// <summary>
        /// A test for
        /// void NullSafe<T>(this T target, Action<T> action) where T : class
        /// </summary>
        [TestMethod()]
        public void ObjectExtension_NullSafeTest_03()
        {
            List<Component> list = [
                new OpenFileDialog(), null!, new Form(), null!, new Control() ];

            list.ForEach(comp => (comp as Control).NullSafe(c => c!.Dispose()));

            // The other way how to do that, without NullSafe:

            list.ForEach(comp =>
              {
                  var c = (comp as Control);
                  if (c != null) c.Dispose();
              }
              );
        }
        #endregion // Tests
    }
}

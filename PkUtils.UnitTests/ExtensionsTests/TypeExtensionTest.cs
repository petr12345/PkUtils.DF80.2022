// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.UnitTests.ExtensionsTests
{
    /// <summary>
    /// This is a test class for TypeExtension and is intended
    /// to contain all TypeExtensionTest Unit Tests
    /// </summary>
    [TestClass()]
    public class TypeExtensionTest
    {
        #region Auxiliary_code_for_tests

        public interface IStackedForm { }
        public class StackedFormWrapper<TForm> : IStackedForm, IDisposable where TForm : Form
        {
            public void Dispose() { }
        }
        public class FormWrapperPlainDerived<TForm> : StackedFormWrapper<TForm> where TForm : Form { };
        public class MainStackedFormWrapper<TForm> : StackedFormWrapper<TForm> where TForm : Form { }
        public class FormWrapperMainDerived<TForm> : MainStackedFormWrapper<TForm> where TForm : Form { }
        #endregion // Auxiliary_code_for_tests

        #region Tests
        #region Tests_GetGenericTypeName

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetGenericTypeName_Test_01()
        {
            Type t = null!;
            t.GetGenericTypeName();
        }

        [TestMethod()]
        public void GetGenericTypeName_Test_02()
        {
            Type t = new List<string>().GetType();
            const string expected = "List<String>";
            string actual = t.GetGenericTypeName();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetGenericTypeName_Test_03()
        {
            Type t = new Dictionary<List<string>, HashSet<HashSet<int>>>().GetType();
            const string expected = "Dictionary<List`1,HashSet`1>";
            string actual = t.GetGenericTypeName();

            Assert.AreEqual(expected, actual);
        }
        #endregion // Tests_GetGenericTypeName

        /// <summary>
        /// A test for HasTheInterface
        /// </summary>
        [TestMethod()]
        public void TypeExtension_HasTheInterfaceTest()
        {
            bool actual;
            Type t = typeof(System.Windows.Forms.Form);
            Type interfaceType = typeof(IDisposable);

            actual = TypeExtension.HasTheInterface(t, interfaceType);
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// A test for IsSubclassOfRawGeneric
        /// </summary>
        [TestMethod()]
        public void TypeExtension_IsSubclassOfRawGenericTest()
        {
            Type genericT, checkedT1, checkedT2;
            bool actual1, actual2;

            genericT = typeof(StackedFormWrapper<>);
            checkedT1 = typeof(FormWrapperPlainDerived<>);
            checkedT2 = typeof(FormWrapperPlainDerived<Form>);
            actual1 = checkedT1.IsSubclassOfRawGeneric(genericT);
            actual2 = checkedT2.IsSubclassOfRawGeneric(genericT);
            Assert.IsTrue(actual1);
            Assert.IsTrue(actual2);

            genericT = typeof(MainStackedFormWrapper<>);
            checkedT1 = typeof(FormWrapperMainDerived<>);
            checkedT2 = typeof(FormWrapperMainDerived<Form>);
            actual1 = checkedT1.IsSubclassOfRawGeneric(genericT);
            actual2 = checkedT2.IsSubclassOfRawGeneric(genericT);
            Assert.IsTrue(actual1);
            Assert.IsTrue(actual2);

            genericT = typeof(MainStackedFormWrapper<>);
            checkedT1 = typeof(FormWrapperMainDerived<>);
            checkedT2 = typeof(FormWrapperMainDerived<Form>);
            actual1 = checkedT1.IsSubclassOfRawGeneric(genericT);
            actual2 = checkedT2.IsSubclassOfRawGeneric(genericT);
            Assert.IsTrue(actual1);
            Assert.IsTrue(actual2);

            genericT = typeof(StackedFormWrapper<>);
            checkedT1 = typeof(FormWrapperMainDerived<>);
            checkedT2 = typeof(FormWrapperMainDerived<Form>);
            actual1 = checkedT1.IsSubclassOfRawGeneric(genericT);
            actual2 = checkedT2.IsSubclassOfRawGeneric(genericT);
            Assert.IsTrue(actual1);
            Assert.IsTrue(actual2);

            Assert.IsTrue(checkedT2.IsSubclassOfRawGeneric(checkedT1));
        }

        /// <summary>
        /// A test for TypeToReadable
        /// </summary>
        [TestMethod()]
        public void TypeExtension_TypeToReadableTest()
        {
            Type t = typeof(System.Drawing.Point);
            string actual = TypeExtension.TypeToReadable(t);
            Assert.AreEqual("Point", actual);

            t = typeof(System.Collections.Generic.List<System.Drawing.Rectangle>);
            actual = TypeExtension.TypeToReadable(t);
            Assert.AreEqual("List`1[Rectangle]", actual);
        }
        #endregion // Tests
    }
}

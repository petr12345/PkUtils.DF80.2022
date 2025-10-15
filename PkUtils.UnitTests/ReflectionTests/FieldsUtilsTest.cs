// Ignore Spelling: Utils
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Reflection;

#nullable enable

namespace PK.PkUtils.UnitTests.ReflectionTests
{
    /// <summary>
    /// This is a test class for FieldsUtils and is intended
    /// to contain all FieldsUtils Unit Tests
    /// </summary>
    [TestClass()]
    public class FieldsUtilsTest
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

        #region Auxiliary_classes_for_test_purpose

        /// <summary>
        /// An example of base class with static fields.
        /// </summary>
        public class A
        {
            public static int _a_public;
            protected static double _a_protected;
            private static string? _a_private;

            public static void SetFieldsA(int a_public, double a_protected, string a_private)
            {
                _a_public = a_public;
                _a_protected = a_protected;
                _a_private = a_private;
            }

            public static void GetFieldsA(out int a_public, out double a_protected, out string? a_private)
            {
                a_public = _a_public;
                a_protected = _a_protected;
                a_private = _a_private;
            }
        }

        /// <summary>
        /// An example of derived class with static fields.
        /// </summary>
        public class B : A
        {
            public static int _b_public;
            protected static double _b_protected;
            private static string? _b_private;

            public static void SetFieldsB(int b_public, double b_protected, string b_private)
            {
                _b_public = b_public;
                _b_protected = b_protected;
                _b_private = b_private;
            }

            public static void GetFieldsB(out int b_public, out double b_protected, out string? b_private)
            {
                b_public = _b_public;
                b_protected = _b_protected;
                b_private = _b_private;
            }
        }

        /// <summary>
        /// An example of base class with non-static fields.
        /// </summary>
        public class C
        {
            public int _c_public;
            protected double _c_protected;
            private string? _c_private;

            public void SetFieldsC(int c_public, double c_protected, string c_private)
            {
                _c_public = c_public;
                _c_protected = c_protected;
                _c_private = c_private;
            }

            public void GetFieldsC(out int c_public, out double c_protected, out string? c_private)
            {
                c_public = _c_public;
                c_protected = _c_protected;
                c_private = _c_private;
            }
        }

        /// <summary>
        /// An example of derived class with non-static fields.
        /// </summary>
        public class D : C
        {
            public int _d_public;
            protected double _d_protected;
            private string? _d_private;

            public void SetFieldsD(int d_public, double d_protected, string d_private)
            {
                _d_public = d_public;
                _d_protected = d_protected;
                _d_private = d_private;
            }

            public void GetFieldsD(out int d_public, out double d_protected, out string? d_private)
            {
                d_public = _d_public;
                d_protected = _d_protected;
                d_private = _d_private;
            }
        }

        /* not needed
        private readonly string[] _arr_A_StaticFieldNames = new string[] {
            "_a_public",
            "_a_protected",
            "_a_private",
        };
        */

        private readonly string[] _arr_B_StaticFieldNames = new string[] {
            "_a_public",
            "_a_protected",
            "_a_private",
            "_b_public",
            "_b_protected",
            "_b_private",
        };

        private readonly string[] _arr_C_NonStaticFieldNames = new string[] {
            "_c_public",
            "_c_protected",
            "_c_private",
        };

        private readonly string[] _arr_D_NonStaticFieldNames = new string[] {
            "_c_public",
            "_c_protected",
            "_c_private",
            "_d_public",
            "_d_protected",
            "_d_private",
        };
        private readonly string[] _arrInvalidFieldNames = new string[] {
            "_xyz",
            "?@#$%",
        };
        #endregion // Auxiliary_classes_for_test_purpose

        #region Accessing_static_field_value_Shallow_Scope_tests

        /// <summary>
        /// A test for GetStaticFieldValue
        /// </summary>
        [TestMethod()]
        public void FieldsUtils_GetStaticFieldValueTest_01st()
        {
            Type t;
            object actual;
            InvalidOperationException? receivedEx;

            A.SetFieldsA(123, 3.14, "AAAaaa");
            B.SetFieldsB(987654, 2.718281828, "BBBbbb");

            // -- i/ Demonstrates that via typeof(A) you can access all fields of type A
            t = typeof(A);
            actual = t.GetStaticFieldValue("_a_public");
            Assert.AreEqual(123, actual);
            actual = t.GetStaticFieldValue("_a_protected");
            Assert.AreEqual(3.14, actual);
            actual = t.GetStaticFieldValue("_a_private");
            Assert.AreEqual("AAAaaa", actual);

            // -- ii/ Demonstrates that via typeof(B) you cannot simply access its fields of type B declared in base A
            t = typeof(B);
            receivedEx = null;
            try { actual = t.GetStaticFieldValue("_a_public"); }
            catch (InvalidOperationException ex) { receivedEx = ex; }
            Assert.IsNotNull(receivedEx);

            receivedEx = null;
            try { actual = t.GetStaticFieldValue("_a_protected"); }
            catch (InvalidOperationException ex) { receivedEx = ex; }
            Assert.IsNotNull(receivedEx);

            receivedEx = null;
            try { actual = t.GetStaticFieldValue("_a_private"); }
            catch (InvalidOperationException ex) { receivedEx = ex; }
            Assert.IsNotNull(receivedEx);

            // -- iii/ Demonstrates that via typeof(B), with the usage of BindingFlags.FlattenHierarchy,
            // you can access public and protected fields of type B declared in base A, but NOT private fields
            t = typeof(B);
            actual = t.GetStaticFieldValue("_a_public", true);
            Assert.AreEqual(123, actual);

            actual = t.GetStaticFieldValue("_a_protected", true);
            Assert.AreEqual(3.14, actual);

            receivedEx = null;
            try { actual = t.GetStaticFieldValue("_a_private", true); }
            catch (InvalidOperationException ex) { receivedEx = ex; }
            Assert.IsNotNull(receivedEx);

            // -- iv/ Demonstrates that via typeof(B) you can access all fields of type B declared in B
            t = typeof(B);
            actual = t.GetStaticFieldValue("_b_public");
            Assert.AreEqual(987654, actual);
            actual = t.GetStaticFieldValue("_b_protected");
            Assert.AreEqual(2.718281828, actual);
            actual = t.GetStaticFieldValue("_b_private");
            Assert.AreEqual("BBBbbb", actual);
        }

        /// <summary>
        /// A test for SetStaticFieldValue
        /// </summary>
        [TestMethod()]
        public void FieldsUtils_SetStaticFieldValueTest_01()
        {
            Type t;
            bool res_public, res_protected, res_private;

            A.SetFieldsA(123, 3.14, "AAAaaa");
            B.SetFieldsB(987654, 2.718281828, "BBBbbb");

            // -- i/ Demonstrates that via typeof(A) you can set all fields of type A
            t = typeof(A);
            res_public = t.SetStaticFieldValue("_a_public", 1492);
            res_protected = t.SetStaticFieldValue("_a_protected", 3.1415926535);
            res_private = t.SetStaticFieldValue("_a_private", "pyramid");
            A.GetFieldsA(out int a_public, out double a_protected, out string? a_private);

            Assert.IsTrue(res_public);
            Assert.IsTrue(res_protected);
            Assert.IsTrue(res_private);
            Assert.AreEqual(1492, a_public);
            Assert.AreEqual(3.1415926535, a_protected);
            Assert.AreEqual("pyramid", a_private);

            // -- ii/ Demonstrates that via typeof(B) you cannot simply set fields of type B declared in base A
            t = typeof(B);
            res_public = t.SetStaticFieldValue("_a_public", 2006);
            res_protected = t.SetStaticFieldValue("_a_protected", 9.876);
            res_private = t.SetStaticFieldValue("_a_private", "hole in the wall");
            A.GetFieldsA(out a_public, out a_protected, out a_private);

            Assert.IsFalse(res_public);
            Assert.AreNotEqual(2006, a_public);

            Assert.IsFalse(res_protected);
            Assert.AreNotEqual(9.876, a_protected);

            Assert.IsFalse(res_private);
            Assert.AreNotEqual("hole in the wall", a_private);

            // -- iii/ Demonstrates that via typeof(B), with the usage of BindingFlags.FlattenHierarchy,
            // you can set public and protected fields of type B declared in base A, but NOT private fields
            t = typeof(B);
            res_public = t.SetStaticFieldValue("_a_public", 223344, true);
            res_protected = t.SetStaticFieldValue("_a_protected", 55.66, true);
            res_private = t.SetStaticFieldValue("_a_private", "cannot be set", true);
            A.GetFieldsA(out a_public, out a_protected, out a_private);

            Assert.IsTrue(res_public);
            Assert.AreEqual(223344, a_public);

            Assert.IsTrue(res_protected);
            Assert.AreEqual(55.66, a_protected);

            Assert.IsFalse(res_private);
            Assert.AreNotEqual("cannot be set", a_private);

            // -- iv/ Demonstrates that via typeof(B) you can set all fields of type B declared in B
            t = typeof(B);
            res_public = t.SetStaticFieldValue("_b_public", 89, true);
            res_protected = t.SetStaticFieldValue("_b_protected", 101.505, true);
            res_private = t.SetStaticFieldValue("_b_private", "that can be set", true);
            B.GetFieldsB(out int b_public, out double b_protected, out string? b_private);

            Assert.IsTrue(res_public);
            Assert.AreEqual(89, b_public);

            Assert.IsTrue(res_protected);
            Assert.AreEqual(101.505, b_protected);

            Assert.IsTrue(res_private);
            Assert.AreEqual("that can be set", b_private);
        }
        #endregion // Accessing_static_field_value_Shallow_Scope_tests

        #region Accessing_static_field_value_Full_Scope_test

        /// <summary>
        /// A test for GetStaticFieldValueEx
        /// </summary>
        [TestMethod()]
        public void FieldsUtils_GetStaticFieldValueExTest_01st()
        {
            Type t;
            object actual;

            A.SetFieldsA(123, 3.14, "AAAaaa");

            // -- Demonstrates that via typeof(B) you can access its fields of type B declared in base A
            //   through call GetStaticFieldValueEx
            t = typeof(B);
            actual = t.GetStaticFieldValueEx<int>("_a_public");
            Assert.AreEqual(123, actual);
            actual = t.GetStaticFieldValueEx<double>("_a_protected");
            Assert.AreEqual(3.14, actual);
            actual = t.GetStaticFieldValueEx<string>("_a_private");
            Assert.AreEqual("AAAaaa", actual);
        }

        /// <summary>
        /// A test for SetStaticFieldValueEx
        /// </summary>
        [TestMethod()]
        public void FieldsUtils_SetStaticFieldValueExTest_01st()
        {
            Type t;

            A.SetFieldsA(123, 3.14, "AAAaaa");
            B.SetFieldsB(987654, 2.718281828, "BBBbbb");

            // -- i/ Demonstrates that via typeof(A), the call of SetStaticFieldValueEx can
            // set all fields of type A
            t = typeof(A);
            t.SetStaticFieldValueEx("_a_public", 1492);
            t.SetStaticFieldValueEx("_a_protected", 3.1415926535);
            t.SetStaticFieldValueEx("_a_private", "pyramid");
            A.GetFieldsA(out int a_public, out double a_protected, out string? a_private);

            Assert.AreEqual(1492, a_public);
            Assert.AreEqual(3.1415926535, a_protected);
            Assert.AreEqual("pyramid", a_private);

            // -- ii/ Demonstrates that via typeof(B), the call of SetStaticFieldValueEx can
            // set public, protected and private fields of type B declared in base A
            t = typeof(B);
            t.SetStaticFieldValueEx("_a_public", 223344);
            t.SetStaticFieldValueEx("_a_protected", 55.66);
            t.SetStaticFieldValueEx("_a_private", "that can be set in A");
            A.GetFieldsA(out a_public, out a_protected, out a_private);

            Assert.AreEqual(223344, a_public);
            Assert.AreEqual(55.66, a_protected);
            Assert.AreEqual("that can be set in A", a_private);

            // -- iii/ Demonstrates that via typeof(B) you can set all fields of type B declared in B
            t = typeof(B);
            t.SetStaticFieldValueEx("_b_public", 89);
            t.SetStaticFieldValueEx("_b_protected", 101.505);
            t.SetStaticFieldValueEx("_b_private", "that can be set in B");
            B.GetFieldsB(out int b_public, out double b_protected, out string? b_private);

            Assert.AreEqual(89, b_public);
            Assert.AreEqual(101.505, b_protected);
            Assert.AreEqual("that can be set in B", b_private);
        }
        #endregion // Accessing_static_field_value_Full_Scope_test

        #region Accessing_Instance_Field_Value_Shallow_Scope_tests

        /// <summary>
        /// A generic helper for the GetInstanceFieldValueTest
        /// </summary>
        protected static void GetInstanceFieldValueTestHelper<T, V>(T obj, string strFieldName, V expected)
        {
            object actual = obj.GetInstanceFieldValue(strFieldName);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GetInstanceFieldValue
        /// </summary>
        [TestMethod()]
        public void FieldsUtils_GetInstanceFieldValueTest()
        {
            InvalidOperationException? receivedEx;
            C c = new();
            D d = new();

            c.SetFieldsC(123, 3.14, "CCCccc");
            d.SetFieldsC(123, 3.14, "CCCccc");
            d.SetFieldsD(987654, 2.718281828, "DDDddd");

            // -- i/ Demonstrates that having C instance, you can access all non-static fields of type C
            GetInstanceFieldValueTestHelper<C, int>(c, "_c_public", 123);
            GetInstanceFieldValueTestHelper<C, double>(c, "_c_protected", 3.14);
            GetInstanceFieldValueTestHelper<C, string>(c, "_c_private", "CCCccc");

            // -- ii/ Demonstrates that having D instance, you can access all non-static public and protected fields
            // of instance D declared in base C, but NOT private fields declared in base C
            GetInstanceFieldValueTestHelper<D, int>(d, "_c_public", 123);
            GetInstanceFieldValueTestHelper<D, double>(d, "_c_protected", 3.14);
            receivedEx = null;
            try { GetInstanceFieldValueTestHelper<D, string>(d, "_c_private", "CCCccc"); }
            catch (InvalidOperationException ex) { receivedEx = ex; }
            Assert.IsNotNull(receivedEx);

            // -- iii/ Demonstrates that having D instance, you can access all fields of type D declared in D
            GetInstanceFieldValueTestHelper<D, int>(d, "_d_public", 987654);
            GetInstanceFieldValueTestHelper<D, double>(d, "_d_protected", 2.718281828);
            GetInstanceFieldValueTestHelper<D, string>(d, "_d_private", "DDDddd");
        }

        /// <summary>
        /// A test for SetInstanceFieldValue
        /// </summary>
        [TestMethod()]
        public void FieldsUtils_SetInstanceFieldValueTest()
        {
            bool res_public, res_protected, res_private;

            C c = new();
            D d = new();

            // -- i/ Demonstrates that having C instance, you can set all fields of type C
            res_public = c.SetInstanceFieldValue("_c_public", 1492);
            res_protected = c.SetInstanceFieldValue("_c_protected", 3.1415926535);
            res_private = c.SetInstanceFieldValue("_c_private", "pyramid");
            c.GetFieldsC(out int c_public, out double c_protected, out string? c_private);

            Assert.IsTrue(res_public);
            Assert.IsTrue(res_protected);
            Assert.IsTrue(res_private);
            Assert.AreEqual(1492, c_public);
            Assert.AreEqual(3.1415926535, c_protected);
            Assert.AreEqual("pyramid", c_private);

            // -- ii/ Demonstrates that having D instance, you can access all non-static public and protected fields
            // of instance D declared in base C, but NOT private fields
            res_public = d.SetInstanceFieldValue("_c_public", 223344);
            res_protected = d.SetInstanceFieldValue("_c_protected", 55.66);
            res_private = d.SetInstanceFieldValue("_c_private", "cannot be set");
            d.GetFieldsC(out c_public, out c_protected, out c_private);

            Assert.IsTrue(res_public);
            Assert.AreEqual(223344, c_public);

            Assert.IsTrue(res_protected);
            Assert.AreEqual(55.66, c_protected);

            Assert.IsFalse(res_private);
            Assert.AreNotEqual("cannot be set", c_private);

            // -- iii/ Demonstrates that having D instance, you can set all fields of type D declared in D
            res_public = d.SetInstanceFieldValue("_d_public", 89);
            res_protected = d.SetInstanceFieldValue("_d_protected", 101.505);
            res_private = d.SetInstanceFieldValue("_d_private", "that can be set");
            d.GetFieldsD(out int d_public, out double d_protected, out string? d_private);

            Assert.IsTrue(res_public);
            Assert.AreEqual(89, d_public);

            Assert.IsTrue(res_protected);
            Assert.AreEqual(101.505, d_protected);

            Assert.IsTrue(res_private);
            Assert.AreEqual("that can be set", d_private);
        }
        #endregion // Accessing_Instance_Field_Value_Shallow_Scope_tests

        #region Accessing_Instance_Field_Value_Full_Scope_tests

        /// <summary>
        /// A generic helper for the GetInstanceFieldValueExTest
        /// </summary>
        internal static void GetInstanceFieldValueExTestHelper<T, V>(T obj, string strFieldName, V expected)
        {
            V actual = obj.GetInstanceFieldValueEx<V>(strFieldName);
            Assert.AreEqual<V>(expected, actual);
        }

        /// <summary> A test for GetInstanceFieldValueEx. </summary>
        [TestMethod()]
        public void FieldsUtils_GetInstanceFieldValueExTest()
        {
            C c = new();
            D d = new();

            c.SetFieldsC(123, 3.14, "CCCccc");
            d.SetFieldsC(123, 3.14, "CCCccc");
            d.SetFieldsD(987654, 2.718281828, "DDDddd");

            // -- i/ Demonstrates that having C instance, method GetInstanceFieldValueEx can access all non-static fields
            GetInstanceFieldValueExTestHelper<C, int>(c, "_c_public", 123);
            GetInstanceFieldValueExTestHelper<C, double>(c, "_c_protected", 3.14);
            GetInstanceFieldValueExTestHelper<C, string>(c, "_c_private", "CCCccc");

            // -- ii/ Demonstrates that having D instance, method GetInstanceFieldValueEx can access all non-static fields
            GetInstanceFieldValueExTestHelper<D, int>(d, "_c_public", 123);
            GetInstanceFieldValueExTestHelper<D, double>(d, "_c_protected", 3.14);
            GetInstanceFieldValueExTestHelper<D, string>(d, "_c_private", "CCCccc");
            GetInstanceFieldValueExTestHelper<D, int>(d, "_d_public", 987654);
            GetInstanceFieldValueExTestHelper<D, double>(d, "_d_protected", 2.718281828);
            GetInstanceFieldValueExTestHelper<D, string>(d, "_d_private", "DDDddd");
        }

        /// <summary>
        /// A generic helper for the SetInstanceFieldValueExTest_00
        /// </summary>
        internal static void SetInstanceFieldValueExTestHelper<T, V>(T obj, string strFieldName, V val)
        {
            obj.SetInstanceFieldValueEx<V>(strFieldName, val);
            V actual = obj.GetInstanceFieldValueEx<V>(strFieldName);
            Assert.AreEqual<V>(val, actual);
        }

        /// <summary> A test for SetInstanceFieldValueEx, which should succeed. </summary>
        [TestMethod()]
        public void FieldsUtils_SetInstanceFieldValueExTest_00()
        {
            C c = new();
            D d = new();

            // -- i/ Demonstrates that having C instance, SetInstanceFieldValueEx can set all fields of type C
            SetInstanceFieldValueExTestHelper<C, int>(c, "_c_public", 1492);
            SetInstanceFieldValueExTestHelper<C, double>(c, "_c_protected", 3.1415926535);
            SetInstanceFieldValueExTestHelper<C, string>(c, "_c_private", "pyramid");

            // -- ii/ Demonstrates that having D instance, SetInstanceFieldValueEx can set all fields derived from C
            SetInstanceFieldValueExTestHelper<D, int>(d, "_c_public", 223344);
            SetInstanceFieldValueExTestHelper<D, double>(d, "_c_protected", 55.66);
            SetInstanceFieldValueExTestHelper<D, string>(d, "_c_private", "even the private field in base class can be set");

            // -- iii/ Demonstrates that having D instance, you can set all fields of type D declared in D
            SetInstanceFieldValueExTestHelper<D, int>(d, "_d_public", 89);
            SetInstanceFieldValueExTestHelper<D, double>(d, "_d_protected", 101.505);
            SetInstanceFieldValueExTestHelper<D, string>(d, "_d_private", "that can be set");
        }

        /// <summary> A test for SetInstanceFieldValueEx, which should fail. 
        /// The test demonstrates that SetInstanceFieldValueEx cannot be used for assigning
        /// integer expression to the field of type double ( despite the fact that in C#
        /// one could assign integer to double ).
        /// 
        /// The reason is, that it is C# is providing the implicit conversion from int to double. 
        /// That's a language decision, not something which .NET will do for you... 
        /// so from the .NET point of view, double isn't assignable from int.
        /// 
        /// For more information, see for instance 
        /// <see href="http://stackoverflow.com/questions/6275764/why-does-isassignablefrom-not-work-for-int-and-double">
        /// Why does IsAssignableFrom() not work for int and double?</see>
        /// </summary>
        [TestMethod()]
        public void FieldsUtils_SetInstanceFieldValueExTest_01()
        {
            D d = new();

            string strFieldName = "_d_protected";
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance; ;
            FieldInfo f = typeof(D).GetAllFields(strFieldName, flags).Single();

            Assert.AreEqual(typeof(double), f.FieldType);
            Assert.IsFalse(f.FieldType.IsAssignableFrom(typeof(int)));
            Assert.IsFalse(typeof(double).IsAssignableFrom(typeof(int)));

            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                d.SetInstanceFieldValueEx(strFieldName, 101);
            });
        }

        /// <summary> A test for SetInstanceFieldValueEx, which should succeed. 
        /// 
        /// The test demonstrates that SetInstanceFieldValueEx could be used for assigning
        /// integer expression to the field of type double, by a work-around 
        /// (casting the integer expression to double).
        /// 
        /// For more information, see for instance "Why does IsAssignableFrom() not work for int and double?" on
        /// http://stackoverflow.com/questions/6275764/why-does-isassignablefrom-not-work-for-int-and-double
        /// </summary>   
        [TestMethod()]
        public void FieldsUtils_SetInstanceFieldValueExTest_02()
        {
            D d = new();

            string strFieldName = "_d_protected";
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance; ;
            FieldInfo f = typeof(D).GetAllFields(strFieldName, flags).Single();

            Assert.AreEqual(typeof(double), f.FieldType);
            Assert.IsFalse(f.FieldType.IsAssignableFrom(typeof(int)));
            Assert.IsFalse(typeof(double).IsAssignableFrom(typeof(int)));

            d.SetInstanceFieldValueEx(strFieldName, (double)101);
            Assert.AreEqual(101, d.GetInstanceFieldValueEx<double>(strFieldName));
        }


        internal class A_VeryBase { public A_VeryBase() { } };
        internal class B_Derived : A_VeryBase { public B_Derived() { } };
        internal class C_MoreDerived : B_Derived { public C_MoreDerived() { } };
        internal class Owner
        {
            protected B_Derived? _b;
            public Owner() { }
            public Owner(B_Derived b) { _b = b; }
        };

        /// <summary> A test for SetInstanceFieldValueEx, which should fail. </summary>
        [TestMethod()]
        public void FieldsUtils_SetInstanceFieldValueExTest_03()
        {
            Owner owner = new();
            // should fail, since A_VeryBase is neither of type B_Derived, nor derived from it
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                owner.SetInstanceFieldValueEx("_b", new A_VeryBase());
            });
        }

        /// <summary> A test for SetInstanceFieldValueEx, which should succeed. </summary>
        [TestMethod()]
        public void FieldsUtils_SetInstanceFieldValueExTest_04()
        {
            Owner owner = new();
            B_Derived b = new();

            owner.SetInstanceFieldValueEx("_b", b);
            B_Derived actual = owner.GetInstanceFieldValueEx<B_Derived>("_b");
            Assert.AreEqual(b, actual);
        }

        /// <summary> A test for SetInstanceFieldValueEx, which should fail. </summary>
        [TestMethod()]
        public void FieldsUtils_SetInstanceFieldValueExTest_05()
        {
            Owner owner = new();
            C_MoreDerived c = new();

            // should fail, since Owner does not contain any field of type C_MoreDerived
            Assert.ThrowsExactly<InvalidOperationException>(() =>
            {
                owner.SetInstanceFieldValueEx("_b", c);
                C_MoreDerived actual = owner.GetInstanceFieldValueEx<C_MoreDerived>("_b");
                Assert.AreEqual(c, actual);
            });
        }

        /// <summary> A test for SetInstanceFieldValueEx, which should succeed. </summary>
        [TestMethod()]
        public void FieldsUtils_SetInstanceFieldValueExTest_06()
        {
            Owner owner = new();
            C_MoreDerived c = new();

            owner.SetInstanceFieldValueEx<C_MoreDerived>("_b", c);
            C_MoreDerived? actual = owner.GetInstanceFieldValueEx<B_Derived>("_b") as C_MoreDerived;
            Assert.AreEqual(c, actual);
        }
        #endregion // Accessing_Instance_Field_Value_Full_Scope_tests

        #region Accessing_FieldInfo_Full_Scope_tests

        /// <summary>
        /// A test for GetAllFields
        /// </summary>
        [TestMethod()]
        public void FieldsUtils_GetAllFieldsTest_01()
        {
            IEnumerable<FieldInfo> actual;
            IEnumerable<FieldInfo> listActual;
            BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (string strFieldName in _arr_B_StaticFieldNames)
            {
                actual = FieldsUtils.GetAllFields(typeof(B), strFieldName, flags);
                listActual = actual.ToList();
                Assert.AreEqual(1, listActual.Count());
                Assert.AreEqual(listActual.First().Name, strFieldName);
            }

            foreach (string strFieldName in _arrInvalidFieldNames)
            {
                actual = FieldsUtils.GetAllFields(typeof(B), strFieldName, flags);
                Assert.IsFalse(actual.Any());
            }
        }

        /// <summary>
        /// A test for GetAllFields
        /// </summary>
        [TestMethod()]
        public void FieldsUtils_GetAllFieldsTest_02()
        {
            Type t = typeof(B);
            IEnumerable<string> expectedNames = _arr_B_StaticFieldNames;

            BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            IEnumerable<FieldInfo> actualFields = FieldsUtils.GetAllFields(t, flags, null);
            IEnumerable<string> actualNames = actualFields.Select(field => field.Name);

            expectedNames = expectedNames.OrderBy(name => name, Comparer<string>.Default);
            actualNames = actualNames.OrderBy(name => name, Comparer<string>.Default);

            Assert.IsTrue(expectedNames.SequenceEqual(actualNames));
        }

        /// <summary>
        /// A test for GetAllInstanceFields
        /// </summary>
        [TestMethod()]
        public void FieldsUtils_GetAllInstanceFieldsTest()
        {
            IEnumerable<FieldInfo> actual;
            List<FieldInfo> listActual;

            foreach (string strFieldName in _arr_C_NonStaticFieldNames)
            {
                actual = FieldsUtils.GetAllInstanceFields(typeof(C), strFieldName);
                listActual = actual.ToList();
                Assert.HasCount(1, listActual);
                Assert.AreEqual(strFieldName, listActual.First().Name);
            }

            foreach (string strFieldName in _arr_D_NonStaticFieldNames)
            {
                actual = FieldsUtils.GetAllInstanceFields(typeof(D), strFieldName);
                listActual = actual.ToList();
                Assert.HasCount(1, listActual);
                Assert.AreEqual(listActual.First().Name, strFieldName);
            }

            foreach (string strFieldName in _arrInvalidFieldNames)
            {
                actual = FieldsUtils.GetAllInstanceFields(typeof(D), strFieldName);
                Assert.IsFalse(actual.Any());
            }
        }

        /// <summary>
        /// A test for GetInstanceField
        /// </summary>
        [TestMethod()]
        public void FieldsUtils_GetInstanceFieldTest()
        {
            FieldInfo actual;

            foreach (string strFieldName in _arr_D_NonStaticFieldNames)
            {
                actual = FieldsUtils.GetInstanceField(typeof(D), strFieldName);
                Assert.IsNotNull(actual);
                Assert.AreEqual(strFieldName, actual.Name);
            }
        }
        #endregion // Accessing_FieldInfo_Full_Scope_tests
    }
}

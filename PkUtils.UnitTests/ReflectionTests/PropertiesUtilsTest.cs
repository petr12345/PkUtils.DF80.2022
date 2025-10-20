// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Reflection;

#nullable enable

namespace PK.PkUtils.UnitTests.ReflectionTests;

/// <summary>
/// This is a test class for PropertiesUtils and is intended
/// to contain all PropertiesUtils Unit Tests
/// </summary>
[TestClass()]
[DoNotParallelize]
public class PropertiesUtilsTest
{
    #region Auxiliary_classes_for_tests

    /// <summary>
    /// An example of base class with static properties.
    /// </summary>
    public class A
    {
        public static int P_a_public { get; set; }
        protected static double P_a_protected { get; set; }
        private static string? P_a_private { get; set; }

        public static void SetPropertiesA(int a_public, double a_protected, string a_private)
        {
            P_a_public = a_public;
            P_a_protected = a_protected;
            P_a_private = a_private;
        }

        public static void GetPropertiesA(out int a_public, out double a_protected, out string? a_private)
        {
            a_public = P_a_public;
            a_protected = P_a_protected;
            a_private = P_a_private;
        }
    }

    /// <summary>
    /// An example of derived class with static properties.
    /// </summary>
    public class B : A
    {
        public static int P_b_public { get; set; }
        protected static double P_b_protected { get; set; }
        private static string? P_b_private { get; set; }

        public static void SetPropertiesB(int b_public, double b_protected, string b_private)
        {
            P_b_public = b_public;
            P_b_protected = b_protected;
            P_b_private = b_private;
        }

        public static void GetPropertiesB(out int b_public, out double b_protected, out string? b_private)
        {
            b_public = P_b_public;
            b_protected = P_b_protected;
            b_private = P_b_private;
        }
    }

    /// <summary>
    /// An example of base class with non-static properties.
    /// </summary>
    public class C
    {
        public int P_c_public { get; set; }
        protected double P_c_protected { get; set; }
        private string? P_c_private { get; set; }

        public void SetPropertiesC(int c_public, double c_protected, string? c_private)
        {
            P_c_public = c_public;
            P_c_protected = c_protected;
            P_c_private = c_private;
        }

        public void GetPropertiesC(out int c_public, out double c_protected, out string? c_private)
        {
            c_public = P_c_public;
            c_protected = P_c_protected;
            c_private = P_c_private;
        }
    }

    /// <summary>
    /// An example of derived class with non-static properties.
    /// </summary>
    public class D : C
    {
        public int P_d_public { get; set; }
        protected double P_d_protected { get; set; }
        private string? P_d_private { get; set; }

        public void SetPropertiesD(int d_public, double d_protected, string d_private)
        {
            P_d_public = d_public;
            P_d_protected = d_protected;
            P_d_private = d_private;
        }

        public void GetPropertiesD(out int d_public, out double d_protected, out string? d_private)
        {
            d_public = P_d_public;
            d_protected = P_d_protected;
            d_private = P_d_private;
        }
    }

    private readonly string[] P_arr_A_StaticPropertisNames = new string[] {
        "P_a_public",
        "P_a_protected",
        "P_a_private",
    };

    private readonly string[] P_arr_B_StaticPropertisNames = new string[] {
        "P_a_public",
        "P_a_protected",
        "P_a_private",
        "P_b_public",
        "P_b_protected",
        "P_b_private",
    };

    private readonly string[] P_arr_C_NonStaticPropertisNames = new string[] {
        "P_c_public",
        "P_c_protected",
        "P_c_private",
    };

    private readonly string[] P_arr_D_NonStaticPropertisNames = new string[] {
        "P_c_public",
        "P_c_protected",
        "P_c_private",
        "P_d_public",
        "P_d_protected",
        "P_d_private",
    };

    private readonly string[] P_arrInvalidPropertisNames = new string[] {
        "P_xyz",
        "?@#$%",
    };
    #endregion // Auxiliary_classes_for_tests

    #region Auxiliary_code_for_tests

    /// <summary>
    /// A generic helper for the GetInstancePropertyValueTest
    /// </summary>
    internal static void GetInstancePropertyValueTestHelper<T, V>(T obj, string strPropertyName, V? expected)
    {
        object? actual = obj!.GetInstancePropertyValue(strPropertyName);
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// A generic helper for the GetInstancePropertyValueExTest
    /// </summary>
    internal static void GetInstancePropertyValueExTestHelper<T, V>(T obj, string strPropertyName, V? expected)
    {
        V? actual = obj!.GetInstancePropertyValueEx<V>(strPropertyName);
        Assert.AreEqual<V>(expected, actual);
    }

    /// <summary>
    /// A generic helper for the SetInstancePropertyValueExTest
    /// </summary>
    internal static void SetInstancePropertyValueExTestHelper<T, V>(T obj, V? value, string strPropertyName)
    {
        obj!.SetInstancePropertyValueEx<V>(value, strPropertyName);
        V? actual = obj!.GetInstancePropertyValueEx<V>(strPropertyName);
        Assert.AreEqual<V>(value, actual);
    }
    #endregion // Auxiliary_code_for_tests

    #region Accessing_static_property_value_Shallow_Scope_tests

    /// <summary>
    /// A test for GetStaticPropertyValue
    /// </summary>
    [TestMethod()]
    public void PropertiesUtils_GetStaticPropertyValueTest()
    {
        Type t;
        object? actual;

        A.SetPropertiesA(123, 3.14, "AAAaaa");
        B.SetPropertiesB(987654, 2.718281828, "BBBbbb");

        // -- i/ Demonstrates that via typeof(A) you can access all properties of type A
        t = typeof(A);
        actual = t.GetStaticPropertyValue("P_a_public");
        Assert.AreEqual(123, actual);
        actual = t.GetStaticPropertyValue("P_a_protected");
        Assert.AreEqual(3.14, actual);
        actual = t.GetStaticPropertyValue("P_a_private");
        Assert.AreEqual("AAAaaa", actual);

        // -- ii/ Demonstrates that via typeof(B) you cannot simply access its properties of type B declared in base A
        t = typeof(B);
        actual = t.GetStaticPropertyValue("P_a_public");
        Assert.IsNull(actual);
        actual = t.GetStaticPropertyValue("P_a_protected");
        Assert.IsNull(actual);
        actual = t.GetStaticPropertyValue("P_a_private");
        Assert.IsNull(actual);

        // -- iii/ Demonstrates that via typeof(B), with the usage of BindingFlags.FlattenHierarchy,
        // you can access public and protected properties of type B declared in base A, but NOT private properties
        t = typeof(B);
        actual = t.GetStaticPropertyValue("P_a_public", true);
        Assert.AreEqual(123, actual);
        actual = t.GetStaticPropertyValue("P_a_protected", true);
        Assert.AreEqual(3.14, actual);
        actual = t.GetStaticPropertyValue("P_a_private", true);
        Assert.IsNull(actual);

        // -- iv/ Demonstrates that via typeof(B) you can access all properties of type B declared in B
        t = typeof(B);
        actual = t.GetStaticPropertyValue("P_b_public");
        Assert.AreEqual(987654, actual);
        actual = t.GetStaticPropertyValue("P_b_protected");
        Assert.AreEqual(2.718281828, actual);
        actual = t.GetStaticPropertyValue("P_b_private");
        Assert.AreEqual("BBBbbb", actual);
    }

    /// <summary>
    /// A test for SetStaticPropertyValue
    /// </summary>
    [TestMethod()]
    public void PropertiesUtils_SetStaticPropertyValueTest()
    {
        Type t;
        bool res_public, res_protected, res_private;

        A.SetPropertiesA(123, 3.14, "AAAaaa");
        B.SetPropertiesB(987654, 2.718281828, "BBBbbb");

        // -- i/ Demonstrates that via typeof(A) you can set all properties of type A
        t = typeof(A);
        res_public = t.SetStaticPropertyValue(1492, "P_a_public");
        res_protected = t.SetStaticPropertyValue(3.1415926535, "P_a_protected");
        res_private = t.SetStaticPropertyValue("pyramid", "P_a_private");
        A.GetPropertiesA(out int a_public, out double a_protected, out string? a_private);

        Assert.IsTrue(res_public);
        Assert.IsTrue(res_protected);
        Assert.IsTrue(res_private);
        Assert.AreEqual(1492, a_public);
        Assert.AreEqual(3.1415926535, a_protected);
        Assert.AreEqual("pyramid", a_private);

        // -- ii/ Demonstrates that via typeof(B) you cannot simply set properties of type B declared in base A
        t = typeof(B);
        res_public = t.SetStaticPropertyValue(2006, "P_a_public");
        res_protected = t.SetStaticPropertyValue(9.876, "P_a_protected");
        res_private = t.SetStaticPropertyValue("hole in the wall", "P_a_private");
        A.GetPropertiesA(out a_public, out a_protected, out a_private);

        Assert.IsFalse(res_public);
        Assert.AreNotEqual(2006, a_public);

        Assert.IsFalse(res_protected);
        Assert.AreNotEqual(9.876, a_public);

        Assert.IsFalse(res_private);
        Assert.AreNotEqual("hole in the wall", a_private);

        // -- iii/ Demonstrates that via typeof(B), with the usage of BindingFlags.FlattenHierarchy,
        // you can set public and protected properties of type B declared in base A, but NOT private properties
        t = typeof(B);
        res_public = t.SetStaticPropertyValue(223344, "P_a_public", true);
        res_protected = t.SetStaticPropertyValue(55.66, "P_a_protected", true);
        res_private = t.SetStaticPropertyValue("cannot be set", "P_a_private", true);
        A.GetPropertiesA(out a_public, out a_protected, out a_private);

        Assert.IsTrue(res_public);
        Assert.AreEqual(223344, a_public);

        Assert.IsTrue(res_protected);
        Assert.AreEqual(55.66, a_protected);

        Assert.IsFalse(res_private);
        Assert.AreNotEqual("cannot be set", a_private);

        // -- iv/ Demonstrates that via typeof(B) you can set all properties of type B declared in B
        t = typeof(B);
        res_public = t.SetStaticPropertyValue(89, "P_b_public", true);
        res_protected = t.SetStaticPropertyValue(101.505, "P_b_protected", true);
        res_private = t.SetStaticPropertyValue("that can be set", "P_b_private", true);
        B.GetPropertiesB(out int b_public, out double b_protected, out string? b_private);

        Assert.IsTrue(res_public);
        Assert.AreEqual(89, b_public);

        Assert.IsTrue(res_protected);
        Assert.AreEqual(101.505, b_protected);

        Assert.IsTrue(res_private);
        Assert.AreEqual("that can be set", b_private);
    }
    #endregion // Accessing_static_property_value_Shallow_Scope_tests

    #region Accessing_instance_property_value_Shallow_Scope_tests

    /// <summary>
    /// A test for GetInstancePropertyValue
    /// </summary>
    [TestMethod()]
    public void PropertiesUtils_GetInstancePropertyValueTest()
    {
        C c = new();
        D d = new();

        c.SetPropertiesC(123, 3.14, "CCCccc");
        d.SetPropertiesC(123, 3.14, "CCCccc");
        d.SetPropertiesD(987654, 2.718281828, "DDDddd");

        // -- i/ Demonstrates that having C instance, you can access all non-static properties of type C
        GetInstancePropertyValueTestHelper<C, int>(c, "P_c_public", 123);
        GetInstancePropertyValueTestHelper<C, double>(c, "P_c_protected", 3.14);
        GetInstancePropertyValueTestHelper<C, string>(c, "P_c_private", "CCCccc");

        // -- ii/ Demonstrates that having D instance, you can access all non-static public and protected properties
        // of instance D declared in base C, but NOT private properties
        GetInstancePropertyValueTestHelper<D, int>(d, "P_c_public", 123);
        GetInstancePropertyValueTestHelper<D, double>(d, "P_c_protected", 3.14);
        GetInstancePropertyValueTestHelper<D, string>(d, "P_c_private", null);

        // -- iii/ Demonstrates that having D instance, you can access all properties of type D declared in D
        GetInstancePropertyValueTestHelper<D, int>(d, "P_d_public", 987654);
        GetInstancePropertyValueTestHelper<D, double>(d, "P_d_protected", 2.718281828);
        GetInstancePropertyValueTestHelper<D, string>(d, "P_d_private", "DDDddd");
    }

    /// <summary>
    /// A test for SetInstancePropertyValue
    /// </summary>
    [TestMethod()]
    public void PropertiesUtils_SetInstancePropertyValueTest()
    {
        bool res_public, res_protected, res_private;

        C c = new();
        D d = new();

        // -- i/ Demonstrates that having C instance, you can set all properties of type C
        res_public = c.SetInstancePropertyValue(1492, "P_c_public");
        res_protected = c.SetInstancePropertyValue(3.1415926535, "P_c_protected");
        res_private = c.SetInstancePropertyValue("pyramid", "P_c_private");
        c.GetPropertiesC(out int c_public, out double c_protected, out string? c_private);

        Assert.IsTrue(res_public);
        Assert.IsTrue(res_protected);
        Assert.IsTrue(res_private);
        Assert.AreEqual(1492, c_public);
        Assert.AreEqual(3.1415926535, c_protected);
        Assert.AreEqual("pyramid", c_private);

        // -- ii/ Demonstrates that having D instance, you can access all non-static public and protected properties
        // of instance D declared in base C, but NOT private properties
        res_public = d.SetInstancePropertyValue(223344, "P_c_public");
        res_protected = d.SetInstancePropertyValue(55.66, "P_c_protected");
        res_private = d.SetInstancePropertyValue("cannot be set", "P_c_private");
        d.GetPropertiesC(out c_public, out c_protected, out c_private);

        Assert.IsTrue(res_public);
        Assert.AreEqual(223344, c_public);

        Assert.IsTrue(res_protected);
        Assert.AreEqual(55.66, c_protected);

        Assert.IsFalse(res_private);
        Assert.AreNotEqual("cannot be set", c_private);

        // -- iii/ Demonstrates that having D instance, you can set all properties of type D declared in D
        res_public = d.SetInstancePropertyValue(89, "P_d_public");
        res_protected = d.SetInstancePropertyValue(101.505, "P_d_protected");
        res_private = d.SetInstancePropertyValue("that can be set", "P_d_private");
        d.GetPropertiesD(out int d_public, out double d_protected, out string? d_private);

        Assert.IsTrue(res_public);
        Assert.AreEqual(89, d_public);

        Assert.IsTrue(res_protected);
        Assert.AreEqual(101.505, d_protected);

        Assert.IsTrue(res_private);
        Assert.AreEqual("that can be set", d_private);
    }
    #endregion // Accessing_instance_property_value_Shallow_Scope_tests

    #region Accessing_instance_property_value_Full_Scope_tests

    [TestMethod()]
    public void PropertiesUtils_GetInstancePropertyValueExTest()
    {
        C c = new();
        D d = new();

        c.SetPropertiesC(123, 3.14, "CCCccc");
        d.SetPropertiesC(123, 3.14, "CCCccc");
        d.SetPropertiesD(987654, 2.718281828, "DDDddd");

        // -- i/ Demonstrates that having C instance, method GetInstancePropertyValueEx can access all non-static properties
        GetInstancePropertyValueExTestHelper<C, int>(c, "P_c_public", 123);
        GetInstancePropertyValueExTestHelper<C, double>(c, "P_c_protected", 3.14);
        GetInstancePropertyValueExTestHelper<C, string>(c, "P_c_private", "CCCccc");

        // -- ii/ Demonstrates that having D instance, method GetInstancePropertyValueEx can access all non-static properties
        GetInstancePropertyValueExTestHelper<D, int>(d, "P_c_public", 123);
        GetInstancePropertyValueExTestHelper<D, double>(d, "P_c_protected", 3.14);
        GetInstancePropertyValueExTestHelper<D, string>(d, "P_c_private", "CCCccc");
        GetInstancePropertyValueExTestHelper<D, int>(d, "P_d_public", 987654);
        GetInstancePropertyValueExTestHelper<D, double>(d, "P_d_protected", 2.718281828);
        GetInstancePropertyValueExTestHelper<D, string>(d, "P_d_private", "DDDddd");
    }

    [TestMethod()]
    public void PropertiesUtils_SetInstancePropertyValueExTest()
    {
        C c = new();
        D d = new();

        // -- i/ Demonstrates that having C instance, SetInstancePropertyValueEx can set all properties of type C
        SetInstancePropertyValueExTestHelper<C, int>(c, 1492, "P_c_public");
        SetInstancePropertyValueExTestHelper<C, double>(c, 3.1415926535, "P_c_protected");
        SetInstancePropertyValueExTestHelper<C, string>(c, "pyramid", "P_c_private");

        // -- ii/ Demonstrates that having D instance, SetInstancePropertyValueEx can set all properties derived from C
        SetInstancePropertyValueExTestHelper<D, int>(d, 223344, "P_c_public");
        SetInstancePropertyValueExTestHelper<D, double>(d, 55.66, "P_c_protected");
        SetInstancePropertyValueExTestHelper<D, string>(d, "even the private field in base class can be set", "P_c_private");

        // -- iii/ Demonstrates that having D instance, you can set all properties of type D declared in D

        SetInstancePropertyValueExTestHelper<D, int>(d, 89, "P_d_public");
        SetInstancePropertyValueExTestHelper<D, double>(d, 101.505, "P_d_protected");
        SetInstancePropertyValueExTestHelper<D, string>(d, "that can be set", "P_d_private");
    }
    #endregion // Accessing_instance_property_value_Full_Scope_tests

    #region Accessing_PropertyInfo_Full_Scope

    /// <summary>
    /// A test for GetAllProperties
    /// </summary>
    [TestMethod()]
    public void PropertiesUtils_GetAllPropertiesTest_01()
    {
        IEnumerable<PropertyInfo> actual;
        IEnumerable<PropertyInfo> listActual;
        BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        foreach (string strPropertyName in P_arr_B_StaticPropertisNames)
        {
            actual = PropertiesUtils.GetAllProperties(typeof(B), strPropertyName, flags);
            listActual = actual.ToList();
            Assert.AreEqual(1, listActual.Count());
            Assert.AreEqual(listActual.First().Name, strPropertyName);
        }

        foreach (string strPropertyName in P_arrInvalidPropertisNames)
        {
            actual = PropertiesUtils.GetAllProperties(typeof(B), strPropertyName, flags);
            Assert.IsFalse(actual.Any());
        }
    }

    /// <summary>
    /// A test for GetAllProperties
    /// </summary>
    [TestMethod()]
    public void PropertiesUtils_GetAllPropertiesTest_02()
    {
        Type t = typeof(B);
        IEnumerable<string> expectedNames = P_arr_B_StaticPropertisNames;

        BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        IEnumerable<PropertyInfo> actualProperties = PropertiesUtils.GetAllProperties(t, flags);
        IEnumerable<string> actualNames = actualProperties.Select(field => field.Name);

        expectedNames = expectedNames.OrderBy(name => name, Comparer<string>.Default);
        actualNames = actualNames.OrderBy(name => name, Comparer<string>.Default);

        Assert.IsTrue(expectedNames.SequenceEqual(actualNames));
    }

    /// <summary>
    /// A test for GetAllInstanceProperties
    /// </summary>
    [TestMethod()]
    public void PropertiesUtils_GetAllInstancePropertiesTest()
    {
        IEnumerable<PropertyInfo> actual;
        IEnumerable<PropertyInfo> listActual;

        foreach (string strPropertyName in P_arr_C_NonStaticPropertisNames)
        {
            actual = PropertiesUtils.GetAllInstanceProperties(typeof(C), strPropertyName);
            listActual = actual.ToList();
            Assert.AreEqual(1, listActual.Count());
            Assert.AreEqual(listActual.First().Name, strPropertyName);
        }

        foreach (string strPropertyName in P_arr_D_NonStaticPropertisNames)
        {
            actual = PropertiesUtils.GetAllInstanceProperties(typeof(D), strPropertyName);
            listActual = actual.ToList();
            Assert.AreEqual(1, listActual.Count());
            Assert.AreEqual(listActual.First().Name, strPropertyName);
        }

        foreach (string strPropertyName in P_arrInvalidPropertisNames)
        {
            actual = PropertiesUtils.GetAllInstanceProperties(typeof(D), strPropertyName);
            Assert.IsFalse(actual.Any());
        }
    }

    /// <summary>
    /// A test for GetInstanceProperty
    /// </summary>
    [TestMethod()]
    public void PropertiesUtils_GetInstancePropertyTest()
    {
        PropertyInfo actual;

        foreach (string strPropertyName in P_arr_D_NonStaticPropertisNames)
        {
            actual = PropertiesUtils.GetInstanceProperty(typeof(D), strPropertyName);
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Name, strPropertyName);
        }
    }
    #endregion // Accessing_PropertyInfo_Full_Scope
}

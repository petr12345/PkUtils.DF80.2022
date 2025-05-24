// Ignore Spelling: PkUtils, Utils, Inline
// 
using System.Reflection;
using PK.PkUtils.Reflection;

#pragma warning disable IDE0079    // Remove unnecessary suppressions
#pragma warning disable NUnit2045 // Use Assert.Multiple
#pragma warning disable CA2211 // Non-constant fields should not be visible
#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable IDE0018 // Inline variable declaration

namespace PK.PkUtils.NUnitTests.ReflectionTests;

/// <summary> This is a test class for class FieldsUtils </summary>
[TestFixture()]
[CLSCompliant(false)]
public class FieldsUtilsTest
{
    #region Auxiliary_classes_for_test_purpose

    /// <summary>
    /// An example of base class with static fields.
    /// </summary>
    [CLSCompliant(false)]
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
    [CLSCompliant(false)]
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
    [CLSCompliant(false)]
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
    [CLSCompliant(false)]
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

    /// <summary> A test for GetStaticFieldValue checking if it succeeds. </summary>
    [Test()]
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
        Assert.That(actual, Is.EqualTo(123));
        actual = t.GetStaticFieldValue("_a_protected");
        Assert.That(actual, Is.EqualTo(3.14));
        actual = t.GetStaticFieldValue("_a_private");
        Assert.That(actual, Is.EqualTo("AAAaaa"));

        // -- ii/ Demonstrates that via typeof(B) you cannot simply access its fields of type B declared in base A
        t = typeof(B);
        receivedEx = Assert.Throws<InvalidOperationException>(() => t.GetStaticFieldValue("_a_public"));
        Assert.That(receivedEx, Is.Not.Null);

        receivedEx = Assert.Throws<InvalidOperationException>(() => t.GetStaticFieldValue("_a_protected"));
        Assert.That(receivedEx, Is.Not.Null);

        receivedEx = Assert.Throws<InvalidOperationException>(() => t.GetStaticFieldValue("_a_private"));
        Assert.That(receivedEx, Is.Not.Null);

        // -- iii/ Demonstrates that via typeof(B), with the usage of BindingFlags.FlattenHierarchy,
        // you can access public and protected fields of type B declared in base A, but NOT private fields
        t = typeof(B);
        actual = t.GetStaticFieldValue("_a_public", true);
        Assert.That(actual, Is.EqualTo(123));

        actual = t.GetStaticFieldValue("_a_protected", true);
        Assert.That(actual, Is.EqualTo(3.14));

        receivedEx = Assert.Throws<InvalidOperationException>(() => t.GetStaticFieldValue("_a_private", true));
        Assert.That(receivedEx, Is.Not.Null);

        // -- iv/ Demonstrates that via typeof(B) you can access all fields of type B declared in B
        t = typeof(B);
        actual = t.GetStaticFieldValue("_b_public");
        Assert.That(actual, Is.EqualTo(987654));
        actual = t.GetStaticFieldValue("_b_protected");
        Assert.That(actual, Is.EqualTo(2.718281828));
        actual = t.GetStaticFieldValue("_b_private");
        Assert.That(actual, Is.EqualTo("BBBbbb"));
    }

    /// <summary> A test for SetStaticFieldValue checking if it succeeds. </summary>
    [Test()]
    public void FieldsUtils_SetStaticFieldValueTest_01()
    {
        Type t;
        int a_public; double a_protected; string? a_private;
        int b_public; double b_protected; string? b_private;
        bool res_public, res_protected, res_private;

        A.SetFieldsA(123, 3.14, "AAAaaa");
        B.SetFieldsB(987654, 2.718281828, "BBBbbb");

        // -- i/ Demonstrates that via typeof(A) you can set all fields of type A
        t = typeof(A);
        t.SetStaticFieldValue("_a_public", 1492);
        t.SetStaticFieldValue("_a_protected", 3.1415926535);
        t.SetStaticFieldValue("_a_private", "pyramid");
        A.GetFieldsA(out a_public, out a_protected, out a_private);

        Assert.That(a_public, Is.EqualTo(1492));
        Assert.That(a_protected, Is.EqualTo(3.1415926535));
        Assert.That(a_private, Is.EqualTo("pyramid"));

        // -- ii/ Demonstrates that via typeof(B) you cannot simply set fields of type B declared in base A
        t = typeof(B);
        res_public = t.SetStaticFieldValue("_a_public", 2006);
        res_protected = t.SetStaticFieldValue("_a_protected", 9.876);
        res_private = t.SetStaticFieldValue("_a_private", "hole in the wall");
        A.GetFieldsA(out a_public, out a_protected, out a_private);

        Assert.That(res_public, Is.False);
        Assert.That(a_public, Is.Not.EqualTo(2006));

        Assert.That(res_protected, Is.False);
        Assert.That(a_protected, Is.Not.EqualTo(9.876));

        Assert.That(res_private, Is.False);
        Assert.That(a_private, Is.Not.EqualTo("hole in the wall"));

        // -- iii/ Demonstrates that via typeof(B), with the usage of BindingFlags.FlattenHierarchy,
        // you can set public and protected fields of type B declared in base A, but NOT private fields
        t = typeof(B);
        t.SetStaticFieldValue("_a_public", 223344, true);
        t.SetStaticFieldValue("_a_protected", 55.66, true);
        t.SetStaticFieldValue("_a_private", "cannot be set", true);
        A.GetFieldsA(out a_public, out a_protected, out a_private);

        Assert.That(a_public, Is.EqualTo(223344));
        Assert.That(a_protected, Is.EqualTo(55.66));
        Assert.That(a_private, Is.Not.EqualTo("cannot be set"));

        // -- iv/ Demonstrates that via typeof(B) you can set all fields of type B declared in B
        t = typeof(B);
        t.SetStaticFieldValue("_b_public", 89, true);
        t.SetStaticFieldValue("_b_protected", 101.505, true);
        t.SetStaticFieldValue("_b_private", "that can be set", true);
        B.GetFieldsB(out b_public, out b_protected, out b_private);

        Assert.That(b_public, Is.EqualTo(89));
        Assert.That(b_protected, Is.EqualTo(101.505));
        Assert.That(b_private, Is.EqualTo("that can be set"));
    }

    [Test, Description("Throws ArgumentNullException when 't' argument is null")]
    public void SetStaticFieldValue_NullType_ThrowsArgumentNullException()
    {
        Type nullType = null!;
        string fieldName = "SomeField";
        object value = new object();
        bool flattenHierarchy = false;

        Assert.That(() => nullType.SetStaticFieldValue(fieldName, value, flattenHierarchy),
            Throws.ArgumentNullException);
    }

    [TestCase(null)]
    [TestCase("")]
    [Description("Throws ArgumentNullException or ArgumentException when 'fieldName' is null or empty")]
    public void SetStaticFieldValue_NullOrEmptyFieldName_ThrowsException(string? invalidFieldName)
    {
        Type type = typeof(object);
        object value = new object();
        bool flattenHierarchy = false;

        if (invalidFieldName is null)
        {
            Assert.That(() => type.SetStaticFieldValue(invalidFieldName!, value, flattenHierarchy),
                Throws.ArgumentNullException);
        }
        else
        {
            Assert.That(() => type.SetStaticFieldValue(invalidFieldName, value, flattenHierarchy),
                Throws.ArgumentException);
        }
    }
    #endregion // Accessing_static_field_value_Shallow_Scope_tests

    #region Accessing_static_field_value_Full_Scope_test

    /// <summary>
    /// A test for GetStaticFieldValueEx
    /// </summary>
    [Test()]
    public void FieldsUtils_GetStaticFieldValueExTest_01st()
    {
        Type t;
        object actual;

        A.SetFieldsA(123, 3.14, "AAAaaa");

        // -- Demonstrates that via typeof(B) you can access its fields of type B declared in base A
        //   through call GetStaticFieldValueEx
        t = typeof(B);
        actual = t.GetStaticFieldValueEx<int>("_a_public");
        Assert.That(actual, Is.EqualTo(123));
        actual = t.GetStaticFieldValueEx<double>("_a_protected");
        Assert.That(actual, Is.EqualTo(3.14));
        actual = t.GetStaticFieldValueEx<string>("_a_private");
        Assert.That(actual, Is.EqualTo("AAAaaa"));
    }

    /// <summary>
    /// A test for SetStaticFieldValueEx
    /// </summary>
    [Test()]
    public void FieldsUtils_SetStaticFieldValueExTest_01st()
    {
        Type t;
        int a_public; double a_protected; string? a_private;
        int b_public; double b_protected; string? b_private;

        A.SetFieldsA(123, 3.14, "AAAaaa");
        B.SetFieldsB(987654, 2.718281828, "BBBbbb");

        // -- i/ Demonstrates that via typeof(A), the call of SetStaticFieldValueEx can
        // set all fields of type A
        t = typeof(A);
        t.SetStaticFieldValueEx("_a_public", 1492);
        t.SetStaticFieldValueEx("_a_protected", 3.1415926535);
        t.SetStaticFieldValueEx("_a_private", "pyramid");
        A.GetFieldsA(out a_public, out a_protected, out a_private);

        Assert.That(a_public, Is.EqualTo(1492));
        Assert.That(a_protected, Is.EqualTo(3.1415926535));
        Assert.That(a_private, Is.EqualTo("pyramid"));

        // -- ii/ Demonstrates that via typeof(B), the call of SetStaticFieldValueEx can
        // set public, protected and private fields of type B declared in base A
        t = typeof(B);
        t.SetStaticFieldValueEx("_a_public", 223344);
        t.SetStaticFieldValueEx("_a_protected", 55.66);
        t.SetStaticFieldValueEx("_a_private", "that can be set in A");
        A.GetFieldsA(out a_public, out a_protected, out a_private);

        Assert.That(a_public, Is.EqualTo(223344));
        Assert.That(a_protected, Is.EqualTo(55.66));
        Assert.That(a_private, Is.EqualTo("that can be set in A"));

        // -- iii/ Demonstrates that via typeof(B) you can set all fields of type B declared in B
        t = typeof(B);
        t.SetStaticFieldValueEx("_b_public", 89);
        t.SetStaticFieldValueEx("_b_protected", 101.505);
        t.SetStaticFieldValueEx("_b_private", "that can be set in B");
        B.GetFieldsB(out b_public, out b_protected, out b_private);

        Assert.That(b_public, Is.EqualTo(89));
        Assert.That(b_protected, Is.EqualTo(101.505));
        Assert.That(b_private, Is.EqualTo("that can be set in B"));
    }
    #endregion // Accessing_static_field_value_Full_Scope_test

    #region Accessing_Instance_Field_Value_Shallow_Scope_tests

    /// <summary>
    /// A generic helper for the GetInstanceFieldValueTest
    /// </summary>
    protected static void GetInstanceFieldValueTestHelper<T, V>(T obj, string strFieldName, V expected)
    {
        object actual = obj.GetInstanceFieldValue(strFieldName);
        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// A test for GetInstanceFieldValue
    /// </summary>
    [Test()]
    public void FieldsUtils_GetInstanceFieldValueTest()
    {
        InvalidOperationException? receivedEx;
        C c = new C();
        D d = new D();

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
        Assert.That(receivedEx, Is.Not.Null);

        // -- iii/ Demonstrates that having D instance, you can access all fields of type D declared in D
        GetInstanceFieldValueTestHelper<D, int>(d, "_d_public", 987654);
        GetInstanceFieldValueTestHelper<D, double>(d, "_d_protected", 2.718281828);
        GetInstanceFieldValueTestHelper<D, string>(d, "_d_private", "DDDddd");
    }

    /// <summary>
    /// A test for SetInstanceFieldValue
    /// </summary>
    [Test()]
    public void FieldsUtils_SetInstanceFieldValueTest()
    {
        int c_public; double c_protected; string? c_private;
        int d_public; double d_protected; string? d_private;
        bool res_public, res_protected, res_private;

        C c = new C();
        D d = new D();

        // -- i/ Demonstrates that having C instance, you can set all fields of type C
        res_public = c.SetInstanceFieldValue("_c_public", 1492);
        res_protected = c.SetInstanceFieldValue("_c_protected", 3.1415926535);
        res_private = c.SetInstanceFieldValue("_c_private", "pyramid");
        c.GetFieldsC(out c_public, out c_protected, out c_private);

        Assert.That(res_public, Is.True);
        Assert.That(res_protected, Is.True);
        Assert.That(res_private, Is.True);
        Assert.That(c_public, Is.EqualTo(1492));
        Assert.That(c_protected, Is.EqualTo(3.1415926535));
        Assert.That(c_private, Is.EqualTo("pyramid"));

        // -- ii/ Demonstrates that having D instance, you can access all non-static public and protected fields
        // of instance D declared in base C, but NOT private fields
        res_public = d.SetInstanceFieldValue("_c_public", 223344);
        res_protected = d.SetInstanceFieldValue("_c_protected", 55.66);
        res_private = d.SetInstanceFieldValue("_c_private", "cannot be set");
        d.GetFieldsC(out c_public, out c_protected, out c_private);

        Assert.That(res_public, Is.True);
        Assert.That(c_public, Is.EqualTo(223344));

        Assert.That(res_protected, Is.True);
        Assert.That(c_protected, Is.EqualTo(55.66));

        Assert.That(res_private, Is.False);
        Assert.That(c_private, Is.Not.EqualTo("cannot be set"));

        // -- iii/ Demonstrates that having D instance, you can set all fields of type D declared in D
        res_public = d.SetInstanceFieldValue("_d_public", 89);
        res_protected = d.SetInstanceFieldValue("_d_protected", 101.505);
        res_private = d.SetInstanceFieldValue("_d_private", "that can be set");
        d.GetFieldsD(out d_public, out d_protected, out d_private);

        Assert.That(res_public, Is.True);
        Assert.That(d_public, Is.EqualTo(89));

        Assert.That(res_protected, Is.True);
        Assert.That(d_protected, Is.EqualTo(101.505));

        Assert.That(res_private, Is.True);
        Assert.That(d_private, Is.EqualTo("that can be set"));
    }
    #endregion // Accessing_Instance_Field_Value_Shallow_Scope_tests

    #region Accessing_Instance_Field_Value_Full_Scope_tests

    /// <summary>
    /// A generic helper for the GetInstanceFieldValueExTest
    /// </summary>
    private static void GetInstanceFieldValueExTestHelper<T, V>(T obj, string strFieldName, V expected)
    {
        V actual = obj.GetInstanceFieldValueEx<V>(strFieldName);
        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary> A test for GetInstanceFieldValueEx. </summary>
    [Test()]
    public void FieldsUtils_GetInstanceFieldValueExTest()
    {
        C c = new C();
        D d = new D();

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
    private static void SetInstanceFieldValueExTestHelper<T, V>(T obj, string strFieldName, V val)
    {
        obj.SetInstanceFieldValueEx<V>(strFieldName, val);
        V actual = obj.GetInstanceFieldValueEx<V>(strFieldName);
        Assert.That(actual, Is.EqualTo(val));
    }

    /// <summary> A test for SetInstanceFieldValueEx, which should succeed. </summary>
    [Test()]
    public void FieldsUtils_SetInstanceFieldValueExTest_00()
    {
        C c = new C();
        D d = new D();

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
    [Test()]
    public void FieldsUtils_SetInstanceFieldValueExTest_01()
    {
        D d = new D();

        string strFieldName = "_d_protected";
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance; ;
        FieldInfo f = typeof(D).GetAllFields(strFieldName, flags).Single();

        Assert.That(f.FieldType, Is.EqualTo(typeof(double)));
        Assert.That(f.FieldType.IsAssignableFrom(typeof(int)), Is.False);
        Assert.That(typeof(double).IsAssignableFrom(typeof(int)), Is.False);

        Assert.Throws<InvalidOperationException>(() =>
            d.SetInstanceFieldValueEx(strFieldName, 101));
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
    [Test()]
    public void FieldsUtils_SetInstanceFieldValueExTest_02()
    {
        D d = new D();

        string strFieldName = "_d_protected";
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        FieldInfo f = typeof(D).GetAllFields(strFieldName, flags).Single();

        Assert.That(f.FieldType, Is.EqualTo(typeof(double)));
        Assert.That(f.FieldType.IsAssignableFrom(typeof(int)), Is.False);
        Assert.That(typeof(double).IsAssignableFrom(typeof(int)), Is.False);

        d.SetInstanceFieldValueEx(strFieldName, (double)101);
        Assert.That(d.GetInstanceFieldValueEx<double>(strFieldName), Is.EqualTo(101));
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
    [Test()]
    public void FieldsUtils_SetInstanceFieldValueExTest_03()
    {
        Owner owner = new Owner();

        // should fail, since A_VeryBase is neither of type B_Derived, nor derived from it
        Assert.Throws<InvalidOperationException>(() => owner.SetInstanceFieldValueEx("_b", new A_VeryBase()));
    }

    /// <summary> A test for SetInstanceFieldValueEx, which should succeed. </summary>
    [Test()]
    public void FieldsUtils_SetInstanceFieldValueExTest_04()
    {
        Owner owner = new Owner();
        B_Derived b = new B_Derived();

        owner.SetInstanceFieldValueEx("_b", b);
        B_Derived actual = owner.GetInstanceFieldValueEx<B_Derived>("_b");
        Assert.That(actual, Is.EqualTo(b));
    }

    /// <summary> A test for SetInstanceFieldValueEx, which should fail. </summary>
    [Test()]
    public void FieldsUtils_SetInstanceFieldValueExTest_05()
    {
        Owner owner = new Owner();
        C_MoreDerived c = new C_MoreDerived();
        C_MoreDerived actual;

        // should fail, since Owner does not contain any field of type C_MoreDerived
        owner.SetInstanceFieldValueEx("_b", c);
        Assert.Throws<InvalidOperationException>(() => actual = owner.GetInstanceFieldValueEx<C_MoreDerived>("_b"));
    }

    /// <summary> A test for SetInstanceFieldValueEx, which should succeed. </summary>
    [Test()]
    public void FieldsUtils_SetInstanceFieldValueExTest_06()
    {
        Owner owner = new Owner();
        C_MoreDerived c = new C_MoreDerived();

        owner.SetInstanceFieldValueEx<C_MoreDerived>("_b", c);
        C_MoreDerived? actual = owner.GetInstanceFieldValueEx<B_Derived>("_b") as C_MoreDerived;
        Assert.That(actual, Is.EqualTo(c));
    }
    #endregion // Accessing_Instance_Field_Value_Full_Scope_tests

    #region Accessing_FieldInfo_Full_Scope_tests

    /// <summary>
    /// A test for GetAllFields
    /// </summary>
    [Test()]
    public void FieldsUtils_GetAllFieldsTest_01()
    {
        IEnumerable<FieldInfo> actual;
        IEnumerable<FieldInfo> listActual;
        BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        foreach (string strFieldName in _arr_B_StaticFieldNames)
        {
            actual = FieldsUtils.GetAllFields(typeof(B), strFieldName, flags);
            listActual = actual.ToList();

            Assert.That(listActual.Count(), Is.EqualTo(1));
            Assert.That(listActual.First().Name, Is.EqualTo(strFieldName));
        }

        foreach (string strFieldName in _arrInvalidFieldNames)
        {
            actual = FieldsUtils.GetAllFields(typeof(B), strFieldName, flags);
            Assert.That(actual, Is.Empty);
        }
    }

    /// <summary>
    /// A test for GetAllFields
    /// </summary>
    [Test()]
    public void FieldsUtils_GetAllFieldsTest_02()
    {
        Type t = typeof(B);
        IEnumerable<string> expectedNames = _arr_B_StaticFieldNames;

        BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        IEnumerable<FieldInfo> actualFields = FieldsUtils.GetAllFields(t, flags, null);
        IEnumerable<string> actualNames = actualFields.Select(field => field.Name);

        expectedNames = expectedNames.OrderBy(name => name, Comparer<string>.Default);
        actualNames = actualNames.OrderBy(name => name, Comparer<string>.Default);

        Assert.That(expectedNames, Is.EqualTo(actualNames).AsCollection);
    }

    /// <summary>
    /// A test for GetAllInstanceFields
    /// </summary>
    [Test()]
    public void FieldsUtils_GetAllInstanceFieldsTest()
    {
        IEnumerable<FieldInfo> actual;
        List<FieldInfo> listActual;

        foreach (string strFieldName in _arr_C_NonStaticFieldNames)
        {
            actual = FieldsUtils.GetAllInstanceFields(typeof(C), strFieldName);
            listActual = actual.ToList();

            Assert.That(listActual, Has.Count.EqualTo(1));
            Assert.That(listActual.First().Name, Is.EqualTo(strFieldName));
        }

        foreach (string strFieldName in _arr_D_NonStaticFieldNames)
        {
            actual = FieldsUtils.GetAllInstanceFields(typeof(D), strFieldName);
            listActual = actual.ToList();

            Assert.That(listActual, Has.Count.EqualTo(1));
            Assert.That(listActual.First().Name, Is.EqualTo(strFieldName));
        }

        foreach (string strFieldName in _arrInvalidFieldNames)
        {
            actual = FieldsUtils.GetAllInstanceFields(typeof(D), strFieldName);
            Assert.That(actual, Is.Empty);
        }
    }


    /// <summary>
    /// A test for GetInstanceField
    /// </summary>
    [Test()]
    public void FieldsUtils_GetInstanceFieldTest()
    {
        FieldInfo actual;

        foreach (string strFieldName in _arr_D_NonStaticFieldNames)
        {
            actual = FieldsUtils.GetInstanceField(typeof(D), strFieldName);
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Name, Is.EqualTo(strFieldName));
        }
    }
    #endregion // Accessing_FieldInfo_Full_Scope_tests
}

#pragma warning restore IDE0018 // Inline variable declaration
#pragma warning restore IDE0300 // Simplify collection initialization
#pragma warning restore CA2211 // Non-constant fields should not be visible
#pragma warning restore NUnit2045 // Use Assert.Multiple
#pragma warning restore IDE0079
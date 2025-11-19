// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Reflection;

namespace PK.PkUtils.UnitTests.ReflectionTests;

#pragma warning disable IDE0079     // Remove unnecessary suppressions
#pragma warning disable IDE0059  // Avoid unnecessary value assignments
#pragma warning disable IDE0060  // Remove unused parameter
#pragma warning disable IDE0290  // Use primary constructor
#pragma warning disable IDE0305 // Collection initialization can be simplified
#pragma warning disable CA1822   // Member .. does not access instance data and can be marked as static

/// <summary>
/// This is a test class for MethodsUtils and is intended
/// to contain all MethodsUtils Unit Tests
/// </summary>
[TestClass()]
public class MethodsUtilsTest
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

    #region Methods_execution_Shallow_Scope_tests

    #region Auxiliary_classes_for_static_methods_tests

    /// <summary>
    /// An example of base class with static methods
    /// </summary>
    public class A
    {
        public static int A_Public(int p1, int p2)
        {
            return p1 + p2;
        }
        protected static decimal A_Protected(decimal p1, decimal p2)
        {
            return p1 * p2;
        }
        private static string A_Private(string p1, string p2)
        {
            return p1 + p2;
        }
    }

    /// <summary>
    /// An example of derived class with static methods.
    /// </summary>
    public class B : A
    {
        public static int B_Public(int p1, int p2)
        {
            return p1 - p2;
        }
        protected static decimal B_Protected(decimal p1, decimal p2)
        {
            return p1 / p2;
        }

        private static string B_Private(string p1, string p2)
        {
            return string.Format(CultureInfo.InvariantCulture, p1, p2);
        }
    }

    /// <summary>
    /// An example of class with static overloaded methods.
    /// </summary>
    public class C : B
    {
        private static int StatOverload(int x)
        {
            return x;
        }
        private static int StatOverload(int x, int y)
        {
            return 17;
        }
        private static int StatOverload(string x, string y)
        {
            return 19;
        }
    }
    #endregion // Auxiliary_classes_for_static_methods_tests

    #region Auxiliary_code_for_static_method_tests
    private static readonly string[] _arr_A_StaticMethodNames = [
        "A_Public",
        "A_Protected",
        "A_Private",
    ];

    private static readonly string[] _arr_B_StaticMethodNames = [
        "B_Public",
        "B_Protected",
        "B_Private",
    ];

    private static readonly string[] _arr_C_StaticMethodNames = [
        "StatOverload",
        "StatOverload",
        "StatOverload",
    ];

    private static IEnumerable<string>? _allStaticMethodNames;

    private static IEnumerable<string> AllStaticMethodNames
    {
        get
        {
            _allStaticMethodNames ??= _arr_A_StaticMethodNames.
                  Concat(_arr_B_StaticMethodNames).
                  Concat(_arr_C_StaticMethodNames);
            return _allStaticMethodNames;
        }
    }

    /// <summary>
    /// A generic helper for the RunStaticMethodTest, not supporting argument types
    /// </summary>
    internal static void RunStaticMethodTestHelper<V>(Type t, string methodName, object[] args, V expected)
    {
        V actual = (V)MethodsUtils.RunStaticMethod(t, methodName, args)!;
        Assert.AreEqual(expected, actual);
    }

    /// <summary>
    /// A generic helper for the RunStaticMethodTest, supporting argument types
    /// </summary>
    internal static void RunStaticMethodTestHelper<V>(Type t, string methodName, Type[] argTypes, object[] args, V expected)
    {
        V actual = (V)MethodsUtils.RunStaticMethod(t, methodName, argTypes, args)!;
        Assert.AreEqual(expected, actual);
    }
    #endregion // Auxiliary_code_for_static_method_tests

    #region Running_static_methods_tests

    /// <summary>
    /// A test for RunStaticMethod
    /// </summary>
    [TestMethod()]
    public void MethodsUtils_RunStaticMethodTest()
    {
        // -- i/ Demonstrates that via typeof(A) you can access all methods of type A
        RunStaticMethodTestHelper<int>(typeof(A), "A_Public", [3, 4], 7);
        RunStaticMethodTestHelper<decimal>(typeof(A), "A_Protected", [(decimal)-6, (decimal)-3], 18);
        RunStaticMethodTestHelper<string>(typeof(A), "A_Private", ["xx", "yy"], "xxyy");

        // -- ii/ Demonstrates that via typeof(B) you can access all methods of type B
        RunStaticMethodTestHelper<int>(typeof(B), "B_Public",
          [3, 4], -1);
        RunStaticMethodTestHelper<decimal>(typeof(B), "B_Protected",
          [(decimal)-6, (decimal)-3], 2);
        RunStaticMethodTestHelper<string>(typeof(B), "B_Private",
          ["x{0}x", "yy"], "xyyx");
    }

    /// <summary>
    /// A test for RunStaticMethod that tries to invoke one of several overloads without using type arguments,
    /// hence failing with System.Reflection.AmbiguousMatchException
    /// </summary>
    [TestMethod()]
    public void MethodsUtils_RunStaticMethodTestOverloads_01()
    {
        // -- Demonstrates that without using additional type arguments, 
        // calling overloaded method will cause AmbiguousMatchException
        Assert.ThrowsExactly<System.Reflection.AmbiguousMatchException>(() =>
        {
            RunStaticMethodTestHelper<int>(typeof(C), "StatOverload", [15], 15);
        });
    }

    /// <summary>
    /// A test for RunStaticMethod that can invoke one of several overloads.
    /// without getting System.Reflection.AmbiguousMatchException.
    /// This is achieved by providing (specifying) searched method argument types.
    /// </summary>
    [TestMethod()]
    public void MethodsUtils_RunStaticMethodTestOverloads_02()
    {
        // -- Demonstrates that using additional type arguments, you can access and distinguish static overloads
        RunStaticMethodTestHelper<int>(typeof(C), "StatOverload", [typeof(int)], [15], 15);
        RunStaticMethodTestHelper<int>(typeof(C), "StatOverload", [typeof(int), typeof(int)], [15, 15], 17);
        RunStaticMethodTestHelper<int>(typeof(C), "StatOverload", [typeof(string), typeof(string)], ["x", "y"], 19);
    }
    #endregion // Running_static_methods_tests

    #region Auxiliary_classes_for_nonstatic_methods_tests

    /// <summary>
    /// An example of base class with non-static methods.
    /// </summary>
    public class X
    {
        public string DumpIntegers(int x, int y)
        {
            return string.Format(CultureInfo.InvariantCulture, "DumpIntegers by X: {0} {1}", x, y);
        }
        protected virtual string DumpDecimals(decimal x, decimal y)
        {
            return string.Format(CultureInfo.InvariantCulture, "DumpDecimals by X: {0} {1}", x, y);
        }
        private string DumpStrings(string x, string y)
        {
            return string.Format(CultureInfo.InvariantCulture, "DumpStrings by X: {0} {1}", x, y);
        }
        private string DumpStrings_X_specific(string x, string y)
        {
            return string.Format(CultureInfo.InvariantCulture, "DumpStrings_X_specific by X: {0} {1}", x, y);
        }
    }

    /// <summary>
    /// An example of derived class with non-static methods.
    /// </summary>
    public class Y : X
    {
        public new string DumpIntegers(int x, int y)
        {
            return string.Format(CultureInfo.InvariantCulture, "DumpIntegers by Y: {0} {1}", x, y);
        }
        protected override string DumpDecimals(decimal x, decimal y)
        {
            return string.Format(CultureInfo.InvariantCulture, "DumpDecimals by Y: {0} {1}", x, y);
        }
        private string DumpStrings(string x, string y)
        {
            return string.Format(CultureInfo.InvariantCulture, "DumpStrings by Y: {0} {1}", x, y);
        }
        private string DumpStrings_Y_specific(string x, string y)
        {
            return string.Format(CultureInfo.InvariantCulture, "DumpStrings_Y_specific by Y: {0} {1}", x, y);
        }
    }

    /// <summary>
    /// An example of derived class with non-static methods.
    /// </summary>
    public class Z : Y
    {
        public new string DumpIntegers(int x, int y)
        {
            return string.Format(CultureInfo.InvariantCulture, "DumpIntegers by Z: {0} {1}", x, y);
        }
        protected override string DumpDecimals(decimal x, decimal y)
        {
            return string.Format(CultureInfo.InvariantCulture, "DumpDecimals by Z: {0} {1}", x, y);
        }
        private string DumpStrings(string x, string y)
        {
            return string.Format(CultureInfo.InvariantCulture, "DumpStrings by Z: {0} {1}", x, y);
        }
        private string DumpStrings_Z_specific(string x, string y)
        {
            return string.Format(CultureInfo.InvariantCulture, "DumpStrings_Z_specific by Z: {0} {1}", x, y);
        }
    }

    /// <summary>
    /// An example of class with non-static overloaded methods.
    /// </summary>
    public class ZZ : Z
    {
        private readonly int _data;

        public ZZ(int data)
        {
            _data = data;
        }

        private int Suma(int x)
        {
            return x + _data;
        }
        private int Suma(int x, int y)
        {
            return x + y;
        }
        private int Suma(string x, string y)
        {
            return _data;
        }
    }
    #endregion // Auxiliary_classes_for_nonstatic_methods_tests

    #region Auxiliary_code_for_nonstatic_method_tests

    private static readonly string[] _arr_X_NonStaticMethodNames = [
        "DumpIntegers",
        "DumpDecimals",
        "DumpStrings",
        "DumpStrings_X_specific",
    ];

    private static readonly string[] _arr_Y_NonStaticMethodNames = [
        "DumpIntegers",
        "DumpDecimals",
        "DumpStrings",
        "DumpStrings_Y_specific",
    ];

    private static readonly string[] _arr_Z_NonStaticMethodNames = [
        "DumpIntegers",
        "DumpDecimals",
        "DumpStrings",
        "DumpStrings_Z_specific",
    ];

    private static IEnumerable<string>? _allNonStaticMethodNames;

    private static IEnumerable<string> AllNonStaticMethodNames
    {
        get
        {
            _allNonStaticMethodNames ??= _arr_X_NonStaticMethodNames.
                  Concat(_arr_Y_NonStaticMethodNames).
                  Concat(_arr_Z_NonStaticMethodNames);
            return _allNonStaticMethodNames;
        }
    }

    /// <summary>
    /// A generic helper for the RunStaticMethodTest, not supporting argument types
    /// </summary>
    internal void RunInstanceMethodTestHelper<T, V>(T obj, string methodName, object[] args, V expected)
         where T : notnull
    {
        V actual = (V)MethodsUtils.RunInstanceMethod(obj, methodName, args)!;
        Assert.AreEqual<V>(expected, actual);
    }

    /// <summary>
    /// A generic helper for the RunStaticMethodTest, supporting argument types
    /// </summary>
    internal void RunInstanceMethodTestHelper<T, V>(
        T obj,
        string methodName,
        Type[]? argTypes,
        object[] args,
        V expected) where T : notnull
    {
        V actual = (V)MethodsUtils.RunInstanceMethod(obj, methodName, argTypes, args)!;
        Assert.AreEqual<V>(expected, actual);
    }

    /// <summary>
    /// A generic helper for the RunStaticMethodTest, supporting argument types
    /// </summary>
    internal static void RunInstanceMethodExTestHelper<T, V>(
        T obj,
        string methodName,
        Type[]? argTypes,
        object[] args,
        V expected)
        where T : notnull
    {
        V actual = (V)MethodsUtils.RunInstanceMethodEx(obj, methodName, argTypes, args)!;
        Assert.AreEqual<V>(expected, actual);
    }
    #endregion // Auxiliary_code_for_nonstatic_method_tests

    #region Running_nonstatic_methods_tests

    /// <summary>
    /// A test for RunInstanceMethod, which should succeed.
    /// </summary>
    [TestMethod()]
    public void MethodsUtils_RunInstanceMethodTest()
    {
        X x = new();
        Y y = new();
        Z z = new();

        // Demonstrate invoking non-virtual public method
        RunInstanceMethodTestHelper<X, string>(x, "DumpIntegers", [13, 14], "DumpIntegers by X: 13 14");
        RunInstanceMethodTestHelper<Y, string>(y, "DumpIntegers", [23, 24], "DumpIntegers by Y: 23 24");
        RunInstanceMethodTestHelper<Z, string>(z, "DumpIntegers", [33, 34], "DumpIntegers by Z: 33 34");

        // Demonstrate invoking virtual protected method
        RunInstanceMethodTestHelper<X, string>(x, "DumpDecimals", [(decimal)15, (decimal)16],
          "DumpDecimals by X: 15 16");
        RunInstanceMethodTestHelper<Y, string>(y, "DumpDecimals", [(decimal)25, (decimal)26],
          "DumpDecimals by Y: 25 26");
        RunInstanceMethodTestHelper<Z, string>(z, "DumpDecimals", [(decimal)35, (decimal)36],
          "DumpDecimals by Z: 35 36");

        // Demonstrate invoking non-virtual private method
        RunInstanceMethodTestHelper<X, string>(x, "DumpStrings", ["aa", "bb"],
          "DumpStrings by X: aa bb");
        RunInstanceMethodTestHelper<Y, string>(y, "DumpStrings", ["cc", "dd"],
          "DumpStrings by Y: cc dd");
        RunInstanceMethodTestHelper<Z, string>(z, "DumpStrings", ["ee", "ff"],
          "DumpStrings by Z: ee ff");
    }

    /// <summary>
    /// A test for RunInstanceMethod that can invoke one of several overloads, which should succeed.
    /// </summary>
    [TestMethod()]
    public void MethodsUtils_RunInstanceMethodTestOverloads_01()
    {
        ZZ zz = new(100);
        // -- Demonstrates that using additional type arguments, you can access and distinguish static overloads
        RunInstanceMethodTestHelper<Z, int>(zz, "Suma", [typeof(int)],
          [5], 105);
        RunInstanceMethodTestHelper<Z, int>(zz, "Suma", [typeof(int), typeof(int)],
          [25, 15], 40);
        RunInstanceMethodTestHelper<Z, int>(zz, "Suma", [typeof(string), typeof(string)],
          ["x", "y"], 100);
    }

    /// <summary>
    /// A test for RunStaticMethod that tries to invoke one of several overloads without using type arguments,
    /// hence failing with System.Reflection.AmbiguousMatchException
    /// </summary>
    [TestMethod()]
    public void MethodsUtils_RunInstanceMethodTestOverloads_02()
    {
        ZZ zz = new(100);

        Assert.ThrowsExactly<System.Reflection.AmbiguousMatchException>(() =>
        {
            RunInstanceMethodTestHelper<Z, int>(zz, "Suma", null, [25, 15], 40);
        });
    }

    protected delegate string DumpDelegate(decimal x, decimal y);

    /// <summary> A test for CallBaseBase. </summary>
    [TestMethod()]
    public void MethodsUtils_CallBaseBaseTest()
    {
        Z z = new();
        X x = z;

        object[] args = [(decimal)45, (decimal)46];
        string expectedX = "DumpDecimals by X: 45 46";
        string expectedZ = "DumpDecimals by Z: 45 46";
        string? actual;

        // i/ Calling a virtual method implemented in Z, through instance of Z, in variable of type Z
        // Note that Z-implementation is called.
        actual = (string)z.RunInstanceMethod("DumpDecimals", args)!;
        Assert.AreEqual<string>(expectedZ, actual);

        // ii/ calling a virtual method implemented in Z, through instance of Z, in variable of type X
        // Note that regardless the declaration of x, always the Z-implementation is called.
        actual = (string)x.RunInstanceMethod("DumpDecimals", args)!;
        Assert.AreEqual<string>(expectedZ, actual);

        // iii/ following will call the implementation in X (base base of Z)
        actual = z.CallBaseBase<DumpDelegate, string>("DumpDecimals", args);
        Assert.AreEqual<string>(expectedX, actual);
    }
    #endregion // Running_nonstatic_methods_tests
    #endregion // Methods_execution_Shallow_Scope_tests

    #region Accessing_MethodInfo_Full_Scope_tests

    /// <summary>
    /// A test for GetAllMethods, given the Type and BindingFlags, which should succeed.
    /// </summary>
    [TestMethod()]
    public void MethodsUtils_GetAllMethodsTest()
    {
        Type t = typeof(Z);
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        List<string> expectedNames = AllNonStaticMethodNames.ToList();

        var objMethods = typeof(object).GetMethods(flags | BindingFlags.DeclaredOnly);
        IEnumerable<MethodInfo> actualMethods = MethodsUtils.GetAllMethods(t, flags);
        IEnumerable<string> actualNames = actualMethods.Select(method => method.Name);

        expectedNames.AddRange(objMethods.Select(method => method.Name));
        expectedNames = expectedNames.OrderBy(name => name, Comparer<string>.Default).ToList();
        actualNames = actualNames.OrderBy(name => name, Comparer<string>.Default).ToList();

        Assert.IsTrue(expectedNames.SequenceEqual(actualNames));
    }

    /// <summary>
    /// A test for GetAllMethods, given the Type, BindingFlags and method name, which should succeed.
    /// </summary>
    [TestMethod()]
    public void MethodsUtils_GetAllMethodsTest_01()
    {
        string strMethodName = "DumpIntegers";
        bool nameMatchPredicate(string name) => (0 == string.CompareOrdinal(name, strMethodName));
        Type t = typeof(Z);
        IEnumerable<string> expectedNames = AllNonStaticMethodNames.Where(nameMatchPredicate);

        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        IEnumerable<MethodInfo> actualMethods = MethodsUtils.GetAllMethods(t, strMethodName, flags);
        IEnumerable<string> actualNames = actualMethods.Select(method => method.Name);

        expectedNames = expectedNames.OrderBy(name => name, Comparer<string>.Default).ToList();
        actualNames = actualNames.OrderBy(name => name, Comparer<string>.Default).ToList();

        Assert.IsTrue(expectedNames.SequenceEqual(actualNames));
    }

    /// <summary>
    /// A test for GetAllMethods, given the Type, BindingFlags and method name, 
    /// which should fail with ArgumentNullException
    /// </summary>
    [TestMethod()]
    public void MethodsUtils_GetAllMethodsTest_02()
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            IEnumerable<MethodInfo> actualMethods = MethodsUtils.GetAllMethods(null!, "DumpIntegers", flags);
        });
    }

    /// <summary>
    /// A test for GetAllStaticMethods, given the Type and method name, which should succeed.
    /// </summary>
    [TestMethod()]
    public void MethodsUtils_GetAllStaticMethodsTest()
    {
        string strMethodName = "B_Protected";
        bool nameMatchPredicate(string name) => (0 == string.CompareOrdinal(name, strMethodName));
        Type t = typeof(C);
        IEnumerable<string> expectedNames = AllStaticMethodNames.Where(nameMatchPredicate);

        IEnumerable<MethodInfo> actualMethods = MethodsUtils.GetAllStaticMethods(t, strMethodName);
        IEnumerable<string> actualNames = actualMethods.Select(method => method.Name);

        expectedNames = expectedNames.OrderBy(name => name, Comparer<string>.Default).ToList();
        actualNames = actualNames.OrderBy(name => name, Comparer<string>.Default).ToList();

        Assert.IsTrue(expectedNames.SequenceEqual(actualNames));
    }

    /// <summary>
    /// A test for GetStaticMethod, given the Type and method name, which should succeed.
    /// </summary>
    [TestMethod()]
    public void MethodsUtils_GetStaticMethodTest()
    {
        Type t = typeof(B);
        string strMethodName = "B_Protected";
        string strActualName = string.Empty;
        MethodInfo actualMethod = MethodsUtils.GetStaticMethod(t, strMethodName);

        Assert.IsNotNull(actualMethod);
        Assert.AreEqual(0, string.CompareOrdinal(actualMethod.Name, strMethodName));
    }

    /// <summary>
    /// A test for GetAllInstanceMethods, given the Type and method name, which should succeed.
    /// </summary>
    [TestMethod()]
    public void MethodsUtils_GetAllInstanceMethodsTest()
    {
        string strMethodName = "DumpDecimals";
        bool nameMatchPredicate(string name) => (0 == string.CompareOrdinal(name, strMethodName));
        Type t = typeof(Z);
        IEnumerable<string> expectedNames = AllNonStaticMethodNames.Where(nameMatchPredicate);

        IEnumerable<MethodInfo> actualMethods = MethodsUtils.GetAllInstanceMethods(t, strMethodName);
        IEnumerable<string> actualNames = actualMethods.Select(method => method.Name);

        expectedNames = expectedNames.OrderBy(name => name, Comparer<string>.Default).ToList();
        actualNames = actualNames.OrderBy(name => name, Comparer<string>.Default).ToList();

        Assert.IsTrue(expectedNames.SequenceEqual(actualNames));
    }

    /// <summary>
    /// A test for GetInstanceMethod, given the Type and method name, which should succeed.
    /// </summary>
    [TestMethod()]
    public void MethodsUtils_GetInstanceMethodTest()
    {
        Type t = typeof(X);
        string strMethodName = "DumpDecimals";
        string strActualName = string.Empty;
        MethodInfo actualMethod = MethodsUtils.GetInstanceMethod(t, strMethodName);

        Assert.IsNotNull(actualMethod);
        Assert.AreEqual(0, string.CompareOrdinal(actualMethod.Name, strMethodName));
    }

    /// <summary>
    /// A test for RunInstanceMethodEx, which should succeed.
    /// </summary>
    [TestMethod()]
    public void MethodsUtils_RunInstanceMethodExTest()
    {
        X x = new();
        Y y = new();
        Z z = new();

        // Demonstrate invoking non-virtual private method, that is declared in the base type
        RunInstanceMethodExTestHelper<Z, string>(
            z,
            "DumpStrings_X_specific",
            [typeof(string), typeof(string)],
            ["gg", "hh"],
            "DumpStrings_X_specific by X: gg hh");

        RunInstanceMethodExTestHelper<Z, string>(
            z,
            "DumpStrings_Y_specific",
            null,
            ["ii", "jj"],
            "DumpStrings_Y_specific by Y: ii jj");

        RunInstanceMethodExTestHelper<Z, string>(
            z,
            "DumpStrings_Z_specific",
            [typeof(string), typeof(string)],
            ["kk", "ll"],
            "DumpStrings_Z_specific by Z: kk ll");
    }
    #endregion // Accessing_MethodInfo_Full_Scope_tests
}

#pragma warning restore CA1822   // Member .. does not access instance data and can be marked as static
#pragma warning restore IDE0305 // Collection initialization can be simplified
#pragma warning restore IDE0290 // Use primary constructor
#pragma warning restore IDE0060   // Remove unused parameter
#pragma warning restore IDE0059   // Unnecessary assignment of a value
#pragma warning restore IDE0079     // Remove unnecessary suppressions
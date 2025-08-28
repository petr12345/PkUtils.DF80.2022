// Ignore Spelling: Utils
//
using System.Windows.Forms;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.NUnitTests.ExtensionsTests;

/// <summary>   This is a test class for  <see cref="TypeExtension"/> </summary>
[TestFixture()]
public class TypeExtensionTest
{
    #region Auxiliary_code_for_tests

    public interface IStackedForm { }
    public class StackedFormWrapper<TForm> : IStackedForm, IDisposable where TForm : Form
    {
        public void Dispose() { GC.SuppressFinalize(this); }
    }
    public class FormWrapperPlainDerived<TForm> : StackedFormWrapper<TForm> where TForm : Form { };
    public class MainStackedFormWrapper<TForm> : StackedFormWrapper<TForm> where TForm : Form { }
    public class FormWrapperMainDerived<TForm> : MainStackedFormWrapper<TForm> where TForm : Form { }
    #endregion // Auxiliary_code_for_tests

    #region Tests
    #region Tests_GetGenericTypeName

    [Test()]
    public void GetGenericTypeName_Test_01()
    {
        Type t = null!;
        Assert.Throws<ArgumentNullException>(() => t.GetGenericTypeName());
    }

    [Test()]
    public void GetGenericTypeName_Test_02()
    {
        Type t = new List<string>().GetType();
        const string expected = "List<String>";
        string actual = t.GetGenericTypeName();

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test()]
    public void GetGenericTypeName_Test_03()
    {
        Type t = new Dictionary<List<string>, HashSet<HashSet<int>>>().GetType();
        const string expected = "Dictionary<List`1,HashSet`1>";
        string actual = t.GetGenericTypeName();

        Assert.That(actual, Is.EqualTo(expected));
    }
    #endregion // Tests_GetGenericTypeName

    /// <summary>
    /// A test for HasTheInterface
    /// </summary>
    [Test()]
    public void TypeExtension_HasTheInterfaceTest()
    {
        bool actual;
        Type t = typeof(System.Windows.Forms.Form);
        Type interfaceType = typeof(IDisposable);

        actual = TypeExtension.HasTheInterface(t, interfaceType);
        Assert.That(actual, Is.True);
    }

    /// <summary>
    /// A test for IsSubclassOfRawGeneric
    /// </summary>
    [Test()]
    public void TypeExtension_IsSubclassOfRawGenericTest()
    {
        Type genericT, checkedT1, checkedT2;
        bool actual1, actual2;

        using (Assert.EnterMultipleScope())
        {
            genericT = typeof(StackedFormWrapper<>);
            checkedT1 = typeof(FormWrapperPlainDerived<>);
            checkedT2 = typeof(FormWrapperPlainDerived<Form>);
            actual1 = checkedT1.IsSubclassOfRawGeneric(genericT);
            actual2 = checkedT2.IsSubclassOfRawGeneric(genericT);
            Assert.That(actual1, Is.True);
            Assert.That(actual2, Is.True);

            genericT = typeof(MainStackedFormWrapper<>);
            checkedT1 = typeof(FormWrapperMainDerived<>);
            checkedT2 = typeof(FormWrapperMainDerived<Form>);
            actual1 = checkedT1.IsSubclassOfRawGeneric(genericT);
            actual2 = checkedT2.IsSubclassOfRawGeneric(genericT);
            Assert.That(actual1, Is.True);
            Assert.That(actual2, Is.True);

            genericT = typeof(MainStackedFormWrapper<>);
            checkedT1 = typeof(FormWrapperMainDerived<>);
            checkedT2 = typeof(FormWrapperMainDerived<Form>);
            actual1 = checkedT1.IsSubclassOfRawGeneric(genericT);
            actual2 = checkedT2.IsSubclassOfRawGeneric(genericT);
            Assert.That(actual1, Is.True);
            Assert.That(actual2, Is.True);

            genericT = typeof(StackedFormWrapper<>);
            checkedT1 = typeof(FormWrapperMainDerived<>);
            checkedT2 = typeof(FormWrapperMainDerived<Form>);
            actual1 = checkedT1.IsSubclassOfRawGeneric(genericT);
            actual2 = checkedT2.IsSubclassOfRawGeneric(genericT);

            Assert.That(actual1, Is.True);
            Assert.That(actual2, Is.True);
            Assert.That(checkedT2.IsSubclassOfRawGeneric(checkedT1), Is.True);
        }
    }

    /// <summary>   A test for TypeToReadable. </summary>
    [Test()]
    public void TypeExtension_TypeToReadableTest()
    {
        Type t = typeof(System.Drawing.Point);
        string actual = TypeExtension.TypeToReadable(t);
        Assert.That(actual, Is.EqualTo("Point"));

        t = typeof(List<System.Drawing.Rectangle>);
        actual = TypeExtension.TypeToReadable(t);
        Assert.That(actual, Is.EqualTo("List`1[Rectangle]"));
    }
    #endregion // Tests
}

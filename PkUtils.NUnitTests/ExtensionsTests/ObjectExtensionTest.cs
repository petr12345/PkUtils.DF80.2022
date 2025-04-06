// Ignore Spelling: Utils
//

using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

#pragma warning disable NUnit2005  // warning NUnit2005: Consider using the constraint model, Assert.That(actual, Is.EqualTo(expected)), instead of the classic model

namespace PK.PkUtils.NUnitTests.ExtensionsTests;


/// <summary>
/// This is a test class for <see cref="ObjectExtension"/>.
/// </summary>
[TestFixture()]
public class ObjectExtensionTest
{
    #region Auxiliary_types

    private class Person
    {
        private readonly int _age;
        private readonly string _name;

        public Person(int age, string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
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

    public class MyDisposableClass : IDisposableEx
    {
        private bool _bDisposed;

        public bool IsDisposed => _bDisposed;

        public void Dispose()
        {
            if (!IsDisposed)
            {
                GC.SuppressFinalize(this);
                _bDisposed = true;
            }
        }
    }

    public struct MyDisposableStruct : IDisposableEx
    {
        private bool _bDisposed;

        public bool IsDisposed => _bDisposed;

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
    #region AsString tests

    [Test(Description = "Ensures AsString handles a null object and returns '<null>'.")]
    public void AsStringTest_NullObject_ReturnsCorrectString()
    {
        Person p1 = null!;
        Person p2 = new Person(32, "Paul");

        string s1 = p1.AsString();
        string s2 = p2.AsString();

        Assert.That(s1, Is.EqualTo("<null>"));
        Assert.That(s2, Is.EqualTo("Paul of age 32"));
    }

    #endregion // AsString tests

    #region AsNameValue tests

    [Test(Description = "Ensures AsNameValue handles a null object and returns a correct string.")]
    public void AsNameValueTest_NullObject_ReturnsCorrectString()
    {
        object obj = null!;
        string result = obj.AsNameValue();
        Assert.That(result, Is.EqualTo("obj = <null>"));
    }

    [Test(Description = "Validates AsNameValue generates correct string for a non-null object.")]
    public void AsNameValueTest_NonNullObject_ReturnsCorrectString()
    {
        Person person = new Person(25, "Alice");
        string result = person.AsNameValue();
        Assert.That(result, Is.EqualTo("person = Alice of age 25"));
    }

    [Test(Description = "Checks AsNameValue with a custom object name and verifies correct string output.")]
    public void AsNameValueTest_CustomObjectName_ReturnsCorrectString()
    {
        Person person = new Person(30, "Bob");
        string result = person.AsNameValue("CustomName");
        Assert.That(result, Is.EqualTo("CustomName = Bob of age 30"));
    }

    [Test(Description = "Ensures AsNameValue works correctly with a struct object.")]
    public void AsNameValueTest_StructObject_ReturnsCorrectString()
    {
        MyDisposableStruct myStruct = new MyDisposableStruct();
        string result = myStruct.AsNameValue();
        Assert.That(result, Is.EqualTo("myStruct = PK.PkUtils.NUnitTests.ExtensionsTests.ObjectExtensionTest+MyDisposableStruct"));
    }

    [Test(Description = "Validates AsNameValue generates correct string for a primitive type.")]
    public void AsNameValueTest_PrimitiveType_ReturnsCorrectString()
    {
        int number = 42;
        string result = number.AsNameValue();
        Assert.That(result, Is.EqualTo("number = 42"));
    }

    #endregion // AsNameValue tests

    #region PropertyList tests

    [Test(Description = "Validates PropertyList handles null object and generates correct output.")]
    public void PropertyListTest_NullObject_ReturnsCorrectString()
    {
        Person p1 = null!;
        Person p2 = new Person(32, "Paul");

        string s1 = p1.PropertyList();
        string s2 = p2.PropertyList();

        Assert.That(s1, Is.EqualTo("<null>"));
        Assert.That(s2, Is.EqualTo("Age: 32, Name: Paul"));
    }
    #endregion // PropertyList tests

    #region CheckNotDisposed tests

    [Test(Description = "Validates CheckNotDisposed throws ArgumentNullException for a null object.")]
    public void CheckNotDisposedTest_NullObject_ThrowsException()
    {
        object obj = null!;
        Assert.Throws<ArgumentNullException>(() => obj.CheckNotDisposed(nameof(obj)));
    }

    [Test(Description = "Ensures CheckNotDisposed throws ObjectDisposedException for a disposed object.")]
    public void CheckNotDisposedTest_DisposedObject_ThrowsException()
    {
        object obj = new MyDisposableClass();
        (obj as IDisposable)!.Dispose();
        Assert.Throws<ObjectDisposedException>(() => obj.CheckNotDisposed(nameof(obj)));
    }

    [Test(Description = "Verifies CheckNotDisposed does not throw for a valid object.")]
    public void CheckNotDisposedTest_ValidObject_NoException()
    {
        object obj = new MyDisposableClass();
        obj.CheckNotDisposed(nameof(obj));
    }

    [Test(Description = "Verifies CheckNotDisposed does not throw for a valid struct.")]
    public void CheckNotDisposedTest_ValidStruct_NoException()
    {
        object obj = new MyDisposableStruct();
        obj.CheckNotDisposed(nameof(obj));
    }
    #endregion // CheckNotDisposed tests

    #region CheckArgNotNull tests

    [Test(Description = "Ensures CheckArgNotNull throws ArgumentNullException for a null object.")]
    public void CheckArgNotNullTest_NullObject_ThrowsException()
    {
        Form f = null!;
        Assert.Throws<ArgumentNullException>(() => f.CheckArgNotNull(nameof(f)));
    }

    [Test(Description = "Validates CheckArgNotNull does not throw for a valid object.")]
    public void CheckArgNotNullTest_ValidObject_NoException()
    {
        Form f = new Form();
        f.CheckArgNotNull();
    }
    #endregion // CheckArgNotNull tests

    #region CheckNotNull tests

    [Test(Description = "Ensures CheckNotNull throws InvalidOperationException for a null object.")]
    public void CheckNotNullTest_NullObject_ThrowsException()
    {
        Form f = null!;
        Assert.Throws<InvalidOperationException>(() => f.CheckNotNull(nameof(f)));
    }

    [Test(Description = "Validates CheckNotNull does not throw for a valid object.")]
    public void CheckNotNullTest_ValidObject_NoException()
    {
        Form f = new Form();
        f.CheckNotNull();
    }
    #endregion // CheckNotNull tests

    #region NullSafe tests

#nullable disable
    [Test(Description = "Verifies NullSafe allows safely accessing a property on potentially null values.")]
    public void NullSafeTest_FunctionWithNullValues()
    {
        List<Component> list = [
            new OpenFileDialog(), null!, new Form(), null!, new Control() ];
        StringBuilder sb = new StringBuilder();

        foreach (Component comp in list)
        {
            sb.Append((comp as Form).NullSafe(f => f.Text));
        }
    }

    [Test(Description = "Ensures NullSafe filters out objects with null or empty property values.")]
    public void NullSafeTest_FilterNonEmptyText()
    {
        List<Component> list = [
            new OpenFileDialog(), null!, new Form(), null!, new Control() ];

        IEnumerable<Component> subList = list.Where(
          comp => !string.IsNullOrEmpty((comp as Control).NullSafe(c => c.Text)));

        Assert.That(subList, Is.Empty);
    }

    [Test(Description = "Validates NullSafe allows safely executing an action on potentially null objects.")]
    public void NullSafeTest_ActionWithNullValues()
    {
        List<Component> list = [
            new OpenFileDialog(), null!, new Form(), null!, new Control() ];

        list.ForEach(comp => (comp as Control).NullSafe(c => c!.Dispose()));
    }
#nullable restore

    #endregion // NullSafe tests
    #endregion // Tests
}

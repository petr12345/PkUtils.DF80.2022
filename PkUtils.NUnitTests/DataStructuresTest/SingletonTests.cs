// Ignore Spelling: Utils
//
using System.Text;
using PK.PkUtils.DataStructures;

#pragma warning disable IDE0059   // Avoid unnecessary value assignments
#pragma warning disable NUnit2045 // Use Assert.Multiple

namespace PK.PkUtils.NUnitTests.DataStructuresTest;

/// <summary>
/// Tests for the Singleton<T> class.
/// </summary>
[TestFixture]
public class SingletonTests
{
    #region Typedefs
    private class TestSingleton
    {
        private TestSingleton()
        {
            Value = string.Empty;
        }

        public string Value { get; set; }
    }

    private class DisposableSingleton : IDisposable
    {
        private DisposableSingleton() { }

        public void Dispose()
        {
            // Disposal logic
        }
    }

    private class TestClassWithPublicConstructor
    {
        public TestClassWithPublicConstructor() { }
    }
    #endregion // Typedefs

    #region Tests

    /// <summary>
    /// Tests that the Instance property returns the same instance.
    /// </summary>
    [Test]
    public void Singleton_Instance_ReturnsSameInstance_Test()
    {
        // Act
        var instance1 = Singleton<TestSingleton>.Instance;
        var instance2 = Singleton<TestSingleton>.Instance;

        // Assert
        Assert.That(instance1, Is.SameAs(instance2));
    }

    /// <summary>
    /// Tests that the HasInstance property returns true after the instance is created.
    /// </summary>
    [Test]
    public void Singleton_HasInstance_ReturnsTrueAfterInstanceIsCreated_Test()
    {
        // Act
        var instance = Singleton<TestSingleton>.Instance;

        // Assert
        Assert.That(Singleton<TestSingleton>.HasInstance, Is.True);
    }

    /// <summary>
    /// Tests that the HasInstance property returns false before the instance is created.
    /// </summary>
    [Test]
    public void Singleton_HasInstance_ReturnsFalseBeforeInstanceIsCreated_Test()
    {
        // Assert
        Assert.That(Singleton<string>.HasInstance, Is.False);
    }

    /// <summary>
    /// Tests that the PeekInstance property returns the instance if it exists.
    /// </summary>
    [Test]
    public void Singleton_PeekInstance_ReturnsInstanceIfExists_Test()
    {
        // Act
        var instance = Singleton<TestSingleton>.Instance;

        // Assert
        Assert.That(Singleton<TestSingleton>.PeekInstance, Is.SameAs(instance));
    }

    /// <summary>
    /// Tests that the PeekInstance property returns null if the instance does not exist.
    /// </summary>
    [Test]
    public void Singleton_PeekInstance_ReturnsNullIfInstanceDoesNotExist_Test()
    {
        // Assert
        Assert.That(Singleton<StringBuilder>.PeekInstance, Is.Null);
    }

    /// <summary>
    /// Tests that the DisposeInstance method disposes the instance if it implements IDisposable.
    /// </summary>
    [Test]
    public void Singleton_DisposeInstance_DisposesInstanceIfDisposable_Test()
    {
        // Act
        var instance = Singleton<DisposableSingleton>.Instance;
        var disposed = Singleton<DisposableSingleton>.DisposeInstance();

        // Assert
        Assert.That(disposed, Is.True);
        Assert.That(Singleton<DisposableSingleton>.HasInstance, Is.False);
    }

    /// <summary>
    /// Tests that the DisposeInstance method returns false if the instance does not implement IDisposable.
    /// </summary>
    [Test]
    public void Singleton_DisposeInstance_ReturnsFalseIfNotDisposable_Test()
    {
        // Act
        var instance = Singleton<TestSingleton>.Instance;
        var disposed = Singleton<TestSingleton>.DisposeInstance();

        // Assert
        Assert.That(disposed, Is.False);
    }

    /// <summary>
    /// Tests that the CreateInstance method throws an InvalidOperationException if there is a public constructor.
    /// </summary>
    [Test]
    public void Singleton_CreateInstance_ThrowsExceptionIfPublicConstructorExists_Test()
    {
        // Arrange
        Type typeWithPublicConstructor = typeof(TestClassWithPublicConstructor);

        // Act & Assert
        Assert.That(() => Singleton<TestClassWithPublicConstructor>.Instance, Throws.InvalidOperationException);
    }
    #endregion // Tests
}
#pragma warning restore NUnit2045 // Use Assert.Multiple
#pragma warning restore IDE0059 // Avoid unnecessary value assignments
// Ignore Spelling: CCA, Roundtrip
// 
using PK.PkUtils.Extensions;
using PK.PkUtils.IO;

namespace PK.PkUtils.NUnitTests.IOTests;

/// <summary>
/// Tests for <see cref="ApplicationStorage{T}"/>.
/// </summary>
[TestFixture()]
public sealed class ApplicationStorageTests
{
    #region Tests

    /// <summary>
    /// A test for ApplicationStorage constructor, which should succeed.
    /// </summary>
    [Test()]
    public void ApplicationStorage_Constructor_01()
    {
        ApplicationStorage<string> storage = new(
            ApplicationStorage<string>.DefaultStorageScope, true, string.Empty);

        Assert.That(storage.IsEmpty(), Is.True);
    }

    /// <summary>
    /// A test for ApplicationStorage constructor, which should fail with ArgumentNullException
    /// </summary>
    [Test()]
    public void ApplicationStorage_Constructor_02()
    {
        // Arrange
        string fileNameSuffix = null!;

        // Act
        ArgumentNullException? ex = Assert.Throws<ArgumentNullException>(() =>
            new ApplicationStorage<string>(ApplicationStorage<string>.DefaultStorageScope, true, fileNameSuffix));

        // Assert
        Assert.That(ex?.ParamName, Is.EqualTo("fileNameSuffix"));
    }

    /// <summary>
    /// A test for ApplicationStorage constructor, which should fail with ArgumentNullException
    /// </summary>
    [Test()]
    public void ApplicationStorage_Constructor_03()
    {
        const string fileNameSuffix = "abc";
        Dictionary<string, string> data = null!;

        Assert.Throws<ArgumentNullException>(() => new ApplicationStorage<string>(
            ApplicationStorage<string>.DefaultStorageScope, data, fileNameSuffix));
    }

    /// <summary>
    /// A test for ApplicationStorage storing and loading again, which should succeed.
    /// </summary>
    [Test()]
    public void ApplicationStorage_Roundtrip_01()
    {
        ApplicationStorage<DayOfWeek> storage1st, storage2nd;

        const string fileNameSuffix = "xyz";
        IDictionary<string, DayOfWeek> data =
            Enumerable.Range(2018, 10)
            .Select(y => new DateTime(y, 12, 25))
            .ToDictionary(d => d.ToString(), d => d.DayOfWeek);

        storage1st = new ApplicationStorage<DayOfWeek>(ApplicationStorage<DayOfWeek>.DefaultStorageScope, data, fileNameSuffix);
        storage1st.Save();
        storage2nd = new ApplicationStorage<DayOfWeek>(ApplicationStorage<DayOfWeek>.DefaultStorageScope, false, fileNameSuffix);

        Assert.That(storage1st.MemberwiseEqual(storage2nd), Is.True);
    }
    #endregion // Tests
}

// Ignore Spelling: Utils
// 

using PK.PkUtils.Extensions;

namespace PK.PkUtils.NUnitTests.ExtensionsTests;

/// <summary>
/// This is a test class for <see cref="HashSetExtensions"/> 
///</summary>
[TestFixture()]
public class HashSetExtensionsTest
{
    #region Tests

    /// <summary>
    ///A test for RemoveExisting
    ///</summary>
    [Test()]
    public void HashSetExtension_RemoveExistingTest_01()
    {
        HashSet<int> hashSet = [];
        var listInts = Enumerable.Repeat(0, 5).Select((n, i) => i).ToList();

        listInts.ForEach(n => hashSet.Add(n));
        listInts.ForEach(n => hashSet.RemoveExisting(n));
    }

    [Test()]
    public void HashSetExtension_RemoveExistingTest_02()
    {
        HashSet<int> hashSet = [];
        var listInts = Enumerable.Repeat(0, 5).Select((n, i) => i).ToList();

        listInts.ForEach(n => hashSet.Add(n));
        Assert.Throws<ArgumentException>(() => hashSet.RemoveExisting(122));
    }

    [Test()]
    public void HashSetExtension_AddNewTest_01()
    {
        HashSet<int> hashSet = [];
        var listInts = Enumerable.Repeat(0, 5).Select((n, i) => i).ToList();

        listInts.ForEach(n => hashSet.AddNew(n));
    }

    [Test()]
    public void HashSetExtension_AddNewTest_02()
    {
        HashSet<int> hashSet = [];
        var listInts = Enumerable.Repeat(0, 5).ToList();

        Assert.Throws<ArgumentException>(() =>
            listInts.ForEach(n => hashSet.AddNew(n)));
    }
    #endregion // Tests
}

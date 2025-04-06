// Ignore Spelling: Utils, Roundtrip
// 
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Extensions;
using PK.PkUtils.IO;


namespace PK.PkUtils.UnitTests.IOTests;

/// <summary>
/// Tests for <see cref="ApplicationStorage{T}"/>.
/// </summary>
[TestClass()]
public sealed class ApplicationStorageTests
{
    #region Tests

    /// <summary>
    /// A test for ApplicationStorage constructor, which should succeed.
    /// </summary>
    [TestMethod()]
    public void ApplicationStorage_Constructor_01()
    {
        ApplicationStorage<string> storage = new(
            ApplicationStorage<string>.DefaultStorageScope, true, string.Empty);

        Assert.IsTrue(storage.IsEmpty());
    }

    /// <summary>
    /// A test for ApplicationStorage constructor, which should fail with ArgumentNullException
    /// </summary>
    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ApplicationStorage_Constructor_02()
    {
        ApplicationStorage<string> storage = new(
            ApplicationStorage<string>.DefaultStorageScope, true, null);
    }

    /// <summary>
    /// A test for ApplicationStorage constructor, which should fail with ArgumentNullException
    /// </summary>
    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ApplicationStorage_Constructor_03()
    {
        const string fileNameSuffix = "abc";
        Dictionary<string, string> data = null!;

        ApplicationStorage<string> storage = new(
            ApplicationStorage<string>.DefaultStorageScope, data, fileNameSuffix);
    }

    /// <summary>
    /// A test for ApplicationStorage storing and loading again, which should succeed.
    /// </summary>
    [TestMethod()]
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

        Assert.IsTrue(storage1st.MemberwiseEqual(storage2nd));
    }
    #endregion // Tests
}
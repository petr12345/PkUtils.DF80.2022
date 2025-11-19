// Ignore Spelling: Utils, Cloneable, BSID
// 

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PK.PkUtils.Cloning.Binary;
using PK.PkUtils.DataStructures;

#pragma warning disable 1718
#pragma warning disable IDE0059   // Avoid unnecessary value assignments


namespace PK.PkUtils.UnitTests.DataStructuresTest;

/// <summary> Unit Test of class BSID. </summary>
[TestClass()]
public class BSIDTests
{
    #region Fields

    private const string _sDummy = "kobylamamalybok";
    private const string _sLower = "0xb16b00b5";
    private readonly string _sUpper = _sLower.ToUpperInvariant();
    #endregion // Fields

    #region Tests
    #region Tests_constructors

    [TestMethod]
    public void BSID_Constructor_01()
    {
        string nullString = null!;
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            var id = new BSID(nullString);
        });
    }

    [TestMethod]
    public void BSID_Constructor_02()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            var id = new BSID(string.Empty);
        });
    }

    [TestMethod]
    public void BSID_Constructor_03()
    {
        BSID nullID = null!;
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            var id = new BSID(nullID);
        });
    }

    [TestMethod]
    public void BSID_Constructor_04()
    {
        var id1st = new BSID(_sLower);
        var id2nd = new BSID(id1st);
    }
    #endregion // Tests_constructors

    #region Tests_Properties

    [TestMethod]
    public void BSID_ID_01()
    {
        var id = new BSID(_sLower);
        bool equal = StringComparer.InvariantCultureIgnoreCase.Equals(id.ID, _sUpper);

        Assert.IsTrue(equal);
    }
    #endregion // Tests_Properties

    #region Tests_Methods

    [TestMethod]
    public void BSID_ToString_01()
    {
        object id_1st = new BSID(_sLower);
        bool equal_1st = StringComparer.InvariantCultureIgnoreCase.Equals(id_1st.ToString(), _sUpper);

        Assert.IsTrue(equal_1st);
    }

    [TestMethod]
    public void BSID_ToString_02()
    {
        object id_2nd = new BSID(_sUpper);
        bool equal_2nd = StringComparer.InvariantCulture.Equals(id_2nd.ToString(), _sUpper);

        Assert.IsTrue(equal_2nd);
    }

    [TestMethod]
    public void BSID_GetHashCode()
    {
        object id_1st = new BSID(_sLower);
        object id_2nd = new BSID(_sUpper);

        Assert.AreEqual(id_1st.GetHashCode(), id_2nd.GetHashCode());
    }

    [TestMethod]
    public void BSID_Equals_Obj()
    {
        object nullObj = null!;
        object id_dummy = new BSID(_sDummy);
        object id_1st = new BSID(_sLower);
        object id_2nd = new BSID(_sUpper);

        Assert.IsFalse(id_1st.Equals(nullObj));
        Assert.IsFalse(id_1st.Equals(id_dummy));
        Assert.IsTrue(id_1st.Equals(id_1st));
        Assert.IsTrue(id_1st.Equals(id_2nd));

        Assert.IsFalse(id_2nd.Equals(nullObj));
        Assert.IsFalse(id_2nd.Equals(id_dummy));
        Assert.IsTrue(id_2nd.Equals(id_2nd));
        Assert.IsTrue(id_2nd.Equals(id_1st));
    }
    #endregion // Tests_Methods

    #region Tests_Serialization

    [TestMethod]
    public void BSID_Serialization()
    {
        BSID id_1st = new(_sLower);
        BSID id_2nd = CloneHelperBinary.DeepClone(id_1st);  // involves binary serialization

        Assert.IsTrue(id_1st.Equals(id_2nd));
    }
    [TestMethod]
    public void BSID_operator_FromStringToId_01()
    {
        string nullStr = null!;
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        {
            var id = new BSID(nullStr);
        });
    }

    [TestMethod]
    public void BSID_operator_FromStringToId_02()
    {
        var id = (BSID)_sLower;
        bool equal = StringComparer.InvariantCultureIgnoreCase.Equals(id.ID, _sUpper);

        Assert.IsTrue(equal);
    }

    [TestMethod]
    public void BSID_operator_FromIdToString_01()
    {
        var id = new BSID(_sLower);
        string str = (string)id;
        bool equal = StringComparer.InvariantCultureIgnoreCase.Equals(str, _sUpper);

        Assert.IsTrue(equal);
    }
    #endregion // Tests_Static_Operators_Conversions

    #region  Tests_Static_Operators
    #region Tests_Static_Operators_Comparisons

    [TestMethod]
    public void BSID_operator_Equals()
    {
        BSID? id_null = null;
        BSID id_dummy = new(_sDummy);
        BSID id_1st = new(_sLower);
        BSID id_2nd = new(_sUpper);

        Assert.IsFalse(id_1st == id_null);
        Assert.IsFalse(id_1st == id_dummy);

        Assert.IsTrue(id_1st == id_1st);
        Assert.IsTrue(id_1st == id_2nd);

        Assert.IsFalse(id_2nd == id_null);
        Assert.IsFalse(id_2nd == id_dummy);

        Assert.IsTrue(id_2nd == id_2nd);
        Assert.IsTrue(id_2nd == id_1st);
    }

    [TestMethod]
    public void BSID_operator_less_bigger_01()
    {
        BSID? id_null = null;
        BSID id_dummy = new(_sDummy);

        Assert.IsFalse(id_null < id_null);
        Assert.IsFalse(id_null > id_null);
        Assert.IsFalse(id_dummy < id_dummy);
        Assert.IsFalse(id_dummy > id_dummy);

        Assert.IsTrue(id_null < id_dummy);
        Assert.IsTrue(id_dummy > id_null);
    }

    [TestMethod]
    public void BSID_operator_less_bigger_02()
    {
        BSID id_dummy = new(_sDummy);
        BSID id_1st = new(_sLower);
        BSID id_2nd = new(_sUpper);

        Assert.IsTrue(id_1st == id_1st);
        Assert.IsTrue(id_1st == id_2nd);
        Assert.IsTrue(id_2nd == id_1st);

        Assert.IsLessThan(0, _sLower.CompareTo(_sDummy));
        Assert.IsTrue(id_1st < id_dummy);

        Assert.IsGreaterThan(0, _sDummy.CompareTo(_sLower));
        Assert.IsTrue(id_dummy > id_1st);
    }
    #endregion // Tests_Static_Operators_Comparisons
    #endregion // Tests_Static_Operators

    #region Tests_IEquatable_BSID

    [TestMethod]
    public void BSID_Equals_IEquatable()
    {
        BSID id_null = null!;
        BSID id_dummy = new(_sDummy);
        BSID id_1st = new(_sLower);
        BSID id_2nd = new(_sUpper);

        Assert.IsFalse(id_1st.Equals(id_null));
        Assert.IsFalse(id_1st.Equals(id_dummy));

        Assert.IsTrue(id_1st.Equals(id_1st));
        Assert.IsTrue(id_1st.Equals(id_2nd));

        Assert.IsFalse(id_2nd.Equals(id_null));
        Assert.IsFalse(id_2nd.Equals(id_dummy));

        Assert.IsTrue(id_2nd.Equals(id_2nd));
        Assert.IsTrue(id_2nd.Equals(id_1st));
    }
    [TestMethod]
    public void BSID_IComparable_00()
    {
        BSID id_dummy = new(_sDummy);
        object objGuid = Guid.NewGuid();

        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            (id_dummy as IComparable).CompareTo(objGuid);
        });
    }

    [TestMethod]
    public void BSID_IComparable_01()
    {
        BSID id_null = null!;
        BSID id_dummy = new(_sDummy);

        Assert.AreEqual(0, BSID.Compare(id_null, id_null));
        Assert.AreEqual(0, BSID.Compare(id_dummy, id_dummy));

        Assert.AreEqual(-1, BSID.Compare(id_null, id_dummy));
        Assert.AreEqual(1, BSID.Compare(id_dummy, id_null));
    }

    [TestMethod]
    public void BSID_IComparable_02()
    {
        BSID id_dummy = new(_sDummy);
        BSID id_1st = new(_sLower);
        BSID id_2nd = new(_sUpper);

        Assert.AreEqual(0, id_dummy.CompareTo(id_dummy));
        Assert.AreEqual(0, id_1st.CompareTo(id_1st));
        Assert.AreEqual(0, id_1st.CompareTo(id_2nd));
        Assert.AreEqual(0, id_2nd.CompareTo(id_1st));

        Assert.IsLessThan(0, _sLower.CompareTo(_sDummy));
        Assert.IsLessThan(0, id_1st.CompareTo(id_dummy));

        Assert.IsGreaterThan(0, _sDummy.CompareTo(_sLower));
        Assert.IsGreaterThan(0, id_dummy.CompareTo(id_1st));
    }
    #endregion // Tests_IComparable_BSID
    #endregion // Tests
}
#pragma warning restore IDE0059   // Avoid unnecessary value assignments
#pragma warning restore 1718

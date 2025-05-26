// Ignore Spelling: Utils, BSID
//
using PK.PkUtils.Cloning.Binary;
using PK.PkUtils.DataStructures;

#pragma warning disable 1718   // Suppress locally the warning CS1718: Comparison made to same variable; did you mean to compare something else?


namespace PK.PkUtils.NUnitTests.DataStructuresTest;

/// <summary> Unit Test of class BSID. </summary>
[TestFixture()]
public class BSIDTests
{
    #region Fields

    private const string _sDummy = "kobylamamalybok";
    private const string _sLower = "0xb16b00b5";
    private readonly string _sUpper = _sLower.ToUpperInvariant();
    #endregion // Fields

    #region Tests

    #region Tests_constructors

    [Test]
    public void BSID_Constructor_01()
    {
        string nullString = null!;
        Assert.Throws<ArgumentNullException>(() => new BSID(nullString));
    }

    [Test]
    public void BSID_Constructor_02()
    {
        Assert.Throws<ArgumentException>(() => new BSID(string.Empty));
    }

    [Test]
    public void BSID_Constructor_03()
    {
        BSID nullID = null!;
        Assert.Throws<ArgumentNullException>(() => new BSID(nullID));
    }

    [Test]
    public void BSID_Constructor_04()
    {
        var id1st = new BSID(_sLower);
        var id2nd = new BSID(id1st);
    }
    #endregion // Tests_constructors

    #region Tests_Properties

    [Test]
    public void BSID_ID_01()
    {
        var id = new BSID(_sLower);
        bool equal = StringComparer.InvariantCultureIgnoreCase.Equals(id.ID, _sUpper);

        Assert.That(equal, Is.True);
    }
    #endregion // Tests_Properties

    #region Tests_Methods

    [Test]
    public void BSID_ToString_01()
    {
        object id_1st = new BSID(_sLower);
        bool equal_1st = StringComparer.InvariantCultureIgnoreCase.Equals(id_1st.ToString(), _sUpper);

        Assert.That(equal_1st, Is.EqualTo(true));
    }

    [Test]
    public void BSID_ToString_02()
    {
        object id_2nd = new BSID(_sUpper);
        bool equal_2nd = StringComparer.InvariantCulture.Equals(id_2nd.ToString(), _sUpper);

        Assert.That(equal_2nd, Is.EqualTo(true));
    }

    [Test]
    public void BSID_GetHashCode()
    {
        object id_1st = new BSID(_sLower);
        object id_2nd = new BSID(_sUpper);

        Assert.That(id_1st.GetHashCode(), Is.EqualTo(id_2nd.GetHashCode()));
    }

    [Test]
    public void BSID_Equals_Obj()
    {
        object? nullObj = null;
        object id_dummy = new BSID(_sDummy);
        object id_1st = new BSID(_sLower);
        object id_2nd = new BSID(_sUpper);

        Assert.That(id_1st.Equals(nullObj), Is.False);
        Assert.That(id_1st.Equals(id_dummy), Is.False);
        Assert.That(id_1st.Equals(id_1st), Is.True);
        Assert.That(id_1st.Equals(id_2nd), Is.True);

        Assert.That(id_2nd.Equals(nullObj), Is.False);
        Assert.That(id_2nd.Equals(id_dummy), Is.False);
        Assert.That(id_2nd.Equals(id_2nd), Is.True);
        Assert.That(id_2nd.Equals(id_1st), Is.True);
    }
    #endregion // Tests_Methods

    #region Tests_Serialization

    [Test]
    public void BSID_Serialization()
    {
        BSID id_1st = new BSID(_sLower);
        BSID id_2nd = CloneHelperBinary.DeepClone(id_1st);  // involves binary serialization

        Assert.That(id_2nd, Is.EqualTo(id_1st));
    }
    #endregion // Tests_Serialization

    #region Tests_Static_Operators
    #region Tests_Static_Operators_Conversions

    [Test]
    public void BSID_operator_FromStringToId_01()
    {
        string? nullStr = null;
        BSID id;
        Assert.Throws<ArgumentNullException>(() => id = (BSID)(nullStr));
    }

    [Test]
    public void BSID_operator_FromStringToId_02()
    {
        var id = (BSID)_sLower;
        bool equal = StringComparer.InvariantCultureIgnoreCase.Equals(id.ID, _sUpper);

        Assert.That(equal, Is.EqualTo(true));
    }

    [Test]
    public void BSID_operator_FromIdToString_01()
    {
        var id = new BSID(_sLower);
        string str = (string)id;
        bool equal = StringComparer.InvariantCultureIgnoreCase.Equals(str, _sUpper);

        Assert.That(equal, Is.EqualTo(true));
    }
    #endregion // Tests_Static_Operators_Conversions

    #region Tests_Static_Operators_Comparisons

    [Test]
    public void BSID_operator_Equals()
    {
        BSID id_null = null!;
        BSID id_dummy = new BSID(_sDummy);
        BSID id_1st = new BSID(_sLower);
        BSID id_2nd = new BSID(_sUpper);

        Assert.That(id_1st == id_null, Is.False);
        Assert.That(id_1st == id_dummy, Is.False);

        Assert.That(id_1st == id_1st, Is.True);
        Assert.That(id_1st == id_2nd, Is.True);

        Assert.That(id_2nd == id_null, Is.False);
        Assert.That(id_2nd == id_dummy, Is.False);

        Assert.That(id_2nd == id_2nd, Is.True);
        Assert.That(id_2nd == id_1st, Is.True);
    }

    [Test]
    public void BSID_operator_less_bigger_01()
    {
        BSID id_null = null!;
        BSID id_dummy = new BSID(_sDummy);

        Assert.That(id_null < id_null, Is.False);
        Assert.That(id_null > id_null, Is.False);
        Assert.That(id_dummy < id_dummy, Is.False);
        Assert.That(id_dummy > id_dummy, Is.False);

        Assert.That(id_null < id_dummy, Is.True);
        Assert.That(id_dummy > id_null, Is.True);
    }

    [Test]
    public void BSID_operator_less_bigger_02()
    {
        BSID id_dummy = new BSID(_sDummy);
        BSID id_1st = new BSID(_sLower);
        BSID id_2nd = new BSID(_sUpper);

        Assert.That(id_1st == id_1st, Is.True);
        Assert.That(id_1st == id_2nd, Is.True);
        Assert.That(id_2nd == id_1st, Is.True);

        Assert.That(_sLower.CompareTo(_sDummy) < 0, Is.True);
        Assert.That(id_1st < id_dummy, Is.True);

        Assert.That(_sDummy.CompareTo(_sLower) > 0, Is.True);
        Assert.That(id_dummy > id_1st, Is.True);
    }
    #endregion // Tests_Static_Operators_Comparisons
    #endregion // Tests_Static_Operators

    #region Tests_IEquatable_BSID

    [Test]
    public void BSID_Equals_IEquatable()
    {
        BSID id_null = null!;
        BSID id_dummy = new BSID(_sDummy);
        BSID id_1st = new BSID(_sLower);
        BSID id_2nd = new BSID(_sUpper);

        Assert.That(id_1st.Equals(id_null), Is.False);
        Assert.That(id_1st.Equals(id_dummy), Is.False);

        Assert.That(id_1st.Equals(id_1st), Is.True);
        Assert.That(id_1st.Equals(id_2nd), Is.True);

        Assert.That(id_2nd.Equals(id_null), Is.False);
        Assert.That(id_2nd.Equals(id_dummy), Is.False);

        Assert.That(id_2nd.Equals(id_2nd), Is.True);
        Assert.That(id_2nd.Equals(id_1st), Is.True);
    }
    #endregion // Tests_IEquatable_BSID

    #region Tests_IComparable_BSID

    [Test]
    public void BSID_IComparable_00()
    {
        BSID id_dummy = new BSID(_sDummy);
        object objGuid = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => (id_dummy as IComparable).CompareTo(objGuid));
    }

    [Test]
    public void BSID_IComparable_01()
    {
        BSID? id_null = null;
        BSID id_dummy = new BSID(_sDummy);

        Assert.That(BSID.Compare(id_null, id_null), Is.Zero);
        Assert.That(BSID.Compare(id_dummy, id_dummy), Is.Zero);

        Assert.That(BSID.Compare(id_null, id_dummy), Is.EqualTo(-1));
        Assert.That(BSID.Compare(id_dummy, id_null), Is.EqualTo(1));
    }

    [Test]
    public void BSID_IComparable_02()
    {
        BSID id_dummy = new BSID(_sDummy);
        BSID id_1st = new BSID(_sLower);
        BSID id_2nd = new BSID(_sUpper);

        Assert.That(id_dummy.CompareTo(id_dummy), Is.Zero);
        Assert.That(id_1st.CompareTo(id_1st), Is.Zero);
        Assert.That(id_1st.CompareTo(id_2nd), Is.Zero);
        Assert.That(id_2nd.CompareTo(id_1st), Is.Zero);

        Assert.That(string.Compare(_sLower, _sDummy), Is.LessThan(0));
        Assert.That(id_1st.CompareTo(id_dummy), Is.LessThan(0));

        Assert.That(string.Compare(_sDummy, _sLower), Is.GreaterThan(0));
        Assert.That(id_dummy.CompareTo(id_1st), Is.GreaterThan(0));
    }
    #endregion // Tests_IComparable_BSID
    #endregion // Tests
}
#pragma warning restore 1718

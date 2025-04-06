// Ignore Spelling: Utils
//
using System;
using System.Diagnostics;
using System.Linq;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.UnitTests.NativeMemoryTests;

/// <summary>  Serializable simple person data class, to tests serialization within shared memory.. </summary>
[Serializable]
public class PersonData : IEquatable<PersonData>
{
    #region Fields

    private readonly int _age;
    private readonly string _name;

    // Dummy data, just for the purpose to make the object fairly large
    private readonly int[] _dummyArray = new int[2000];

    #endregion // Fields

    #region Constructor(s)

    public PersonData(int age, string name)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));

        this._age = age;
        this._name = name;

        for (int i = 0; i < 2000; i++)
        {
            this._dummyArray[i] = i;
        }
    }
    #endregion // Constructor(s)

    #region Properties

    public int Age
    {
        get { return _age; }
    }

    public string Name
    {
        get { return _name; }
    }

    public int[] DummyArray
    {
        get { return _dummyArray; }
    }
    #endregion // Properties

    #region Methods

    /// <summary> Determines whether the specified object is equal to the current object.</summary>
    /// <param name="obj"> The object to compare with the current object. May be null. </param>
    /// <returns> True if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return (this as IEquatable<PersonData>).Equals(obj as PersonData);
    }

    /// <summary> Serves as the default hash function.</summary>
    /// <returns> A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        int result = this.Age.GetHashCode();

        result ^= StringComparer.Ordinal.GetHashCode(this.Name);
        result ^= this.DummyArray.SequenceHashCode();

        return result;
    }

    private bool MemberwiseCompare(PersonData other)
    {
        Debug.Assert(other != null);
        bool result;

        if (this.Age != other.Age)
            result = false;
        else if (!StringComparer.Ordinal.Equals(this.Name, other.Name))
            result = false;
        else if ((other.DummyArray == null) || !this.DummyArray.SequenceEqual(other.DummyArray))
            result = false;
        else
            result = true;

        return result;
    }
    #endregion // Methods

    #region IEquatable<InternalKey> Members

    /// <summary> Indicates whether the current object is equal to another.</summary>
    /// <param name="other"> A PersonData to compare with this object. It may be equal to null. </param>
    /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
    public bool Equals(PersonData? other)
    {
        bool result;

        if (null == other)
            result = false;
        else if (object.ReferenceEquals(this, other))
            result = true;
        else if (this.GetType() == other.GetType())
            result = this.MemberwiseCompare(other);
        else
            result = false;  // the other guy should decide what to return

        return result;
    }
    #endregion // IEquatable<InternalKey> Members

}

// Ignore Spelling: Memberwise, rhs, Utils
//
using System;
using System.Globalization;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.DataStructures;

/// <summary> A base generic class for creating your own specific ID class.</summary>
/// 
/// <remarks>In your code, you will derive a specific ID class like following
/// <code>
/// <![CDATA[
/// public sealed class BSID : CaseInsensitiveId<BSID>
/// ]]>
/// </code>
///
/// While you cannot derive this generic from the other class given as type argument,
/// at least you can inherit from this generic base class, with self as type parameter.
/// This approach is called
/// <see href="https://ericlippert.com/2011/02/02/curiouser-and-curiouser/">
/// "Curiously recurring templates/generics pattern"</see>.
/// </remarks>
///
/// <typeparam name="T"> The ID class that you want to create, deriving from CaseInsensitiveId. </typeparam>
[Serializable]
public class CaseInsensitiveId<T> :
    IEquatable<T>,
    IEquatable<CaseInsensitiveId<T>>,
    IComparable<T>,
    IComparable<CaseInsensitiveId<T>>,
    IComparable
    where T : class
{
    #region Fields

    private readonly string _id;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Constructor accepting string representation of ID.</summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="id"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when a supplied <paramref name="id"/> is empty. </exception>
    ///
    /// <param name="id"> The identifier. Can't be null or empty</param>
    public CaseInsensitiveId(string id)
    {
        id.CheckArgNotNullOrEmpty(nameof(id));

        // Does not need to convert to upper now, if GetHashCode() involves ValueComparer
        _id = id;
    }

    /// <summary> Copy-like constructor, accepting another id.</summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="rhs"/> is null. </exception>
    ///
    /// <param name="rhs"> The right hand side Id. Can't be null. </param>
    protected CaseInsensitiveId(CaseInsensitiveId<T> rhs)
    {
        ArgumentNullException.ThrowIfNull(rhs);
        _id = rhs.ID;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Gets the string representation of identifier, as provided by constructor.</summary>
    public string ID { get { return _id; } }

    /// <summary> Gets the used comparer of string values.</summary>
    protected static StringComparer ValueComparer { get { return StringComparer.OrdinalIgnoreCase; } }
    #endregion // Properties

    #region Static Operators

    /// <summary> Converts a string to a CaseInsensitiveId. </summary>
    ///
    /// <param name="id"> The string to be converted to CaseInsensitiveId. Can't be null or empty. </param>
    /// <returns> A new CaseInsensitiveId constructed with <paramref name="id"/> argument.</returns>
    public static explicit operator CaseInsensitiveId<T>(string id)
    {
        return new CaseInsensitiveId<T>(id);
    }

    /// <summary> Converts a CaseInsensitiveId to a string. </summary>
    ///
    /// <param name="id"> The original <see cref="CaseInsensitiveId{T}"/> identifier. It may be null. </param>
    ///
    /// <returns> For a non-null <paramref name="id"/> returns a id.ID; otherwise null. </returns>
    public static explicit operator string(CaseInsensitiveId<T> id)
    {
        string result = id?.ID;
        return result;
    }
    #endregion // Static Operators

    #region Methods

    /// <summary> Determines whether the specified object is equal to the current object.</summary>
    ///
    /// <param name="obj"> The object to compare with the current object. May be null. </param>
    ///
    /// <returns> True if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        return (this as IEquatable<CaseInsensitiveId<T>>).Equals(obj as CaseInsensitiveId<T>);
    }

    /// <summary> Serves as the default hash function.</summary>
    /// <returns> A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return ValueComparer.GetHashCode(ID);
    }

    /// <summary> Convert this object into a string representation.</summary>
    /// <returns> A string that represents this object.</returns>
    public override string ToString()
    {
        return ID;
    }


    /// <summary> Member-wise compare of two IDs, considering just ID property.</summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    ///
    /// <param name="first"> The first id. Can't be null. </param>
    /// <param name="second"> The second id. Can't be null. </param>
    ///
    /// <returns> True if considered equal, false if not.</returns>
    protected static bool MemberwiseEquals(CaseInsensitiveId<T> first, CaseInsensitiveId<T> second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        return ValueComparer.Equals(first.ID, second.ID);
    }

    /// <summary> Compares two objects to determine their relative ordering. </summary>
    ///
    /// <remarks> In part delegates the functionality to IComparable.CompareTo, but that's just for non-null values,
    /// to prevent infinite recursion. For more information regarding null arguments, see for instance
    /// <see href="https://stackoverflow.com/questions/8642080/icomparable-behaviour-for-null-arguments">
    /// IComparable behaviour for null arguments</see>
    /// </remarks>
    ///
    /// <param name="left"> The 'left' instance to be compared. May equal to null. </param>
    /// <param name="right"> The 'right' to be compared. May equal to null. </param>
    ///
    /// <returns> Negative if 'left' is less than 'right', 0 if they are equal, or positive if it is greater.</returns>
    protected static int Compare(CaseInsensitiveId<T> left, CaseInsensitiveId<T> right)
    {
        int result;

        if (ReferenceEquals(left, right))
            result = 0;
        else if (left == null)
            result = -1;
        else if (right == null)
            result = 1;
        else
            result = left.CompareTo(right);

        return result;
    }
    #endregion // Methods

    #region IEquatable<T> Members

    /// <summary> Indicates whether the current object is equal to another object of type <typeparamref name="T"/>.</summary>
    ///
    /// <param name="other"> An object to compare with this object. It may be equal to null. </param>
    ///
    /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
    public bool Equals(T other)
    {
        return (this as object).Equals(other);
    }
    #endregion // IEquatable<T> Members

    #region IEquatable<CaseInsensitiveId<T>> Members

    /// <summary> Tests if this instance is considered equal to another.</summary>
    ///
    /// <param name="other"> An object to compare with this object. </param>
    ///
    /// <returns> true if the objects are considered equal, false if they are not.</returns>
    public bool Equals(CaseInsensitiveId<T> other)
    {
        bool result = false;

        if (other == null)
        {
            /* result = false; already is */
        }
        else if (object.ReferenceEquals(other, this))
        {
            result = true;
        }
        else if (object.ReferenceEquals(other.GetType(), this.GetType()))
        {   // the other object type is directly of my type, just compare values
            result = MemberwiseEquals(this, other);
        }
        else
        {   // the other object is somehow derived, let him decide
            result = other.Equals((object)this);
        }

        return result;
    }
    #endregion // IEquatable<CaseInsensitiveId<T>> Members

    #region IComparable Members

    /// <summary> Compares the current instance with another object <paramref name="obj"/> and returns an
    /// indication whether the current instance precedes, follows, or occurs in the same position in the sort
    /// order as the other object.</summary>
    ///
    /// <exception cref="ArgumentException"> Thrown when <paramref name="obj"/> is not null and it's not
    /// CaseInsensitiveId. </exception>
    ///
    /// <param name="obj"> An object to compare with this instance. </param>
    ///
    /// <returns> A value that indicates the relative order of the objects being compared. 
    /// Value less than zero means that this instance is less than <paramref name="obj" />.
    /// Zero value means that this instance is equal to <paramref name="obj" />.
    /// Value greater than zero means this instance is greater than <paramref name="obj" />.
    /// </returns>
    public int CompareTo(object obj)
    {
        int result;

        if (obj == null)
        {
            result = Compare(this, null);
        }
        else if (obj is CaseInsensitiveId<T> other)
        {
            result = (this as IComparable<CaseInsensitiveId<T>>).CompareTo(other);
        }
        else
        {
            string errorMessage = string.Format(CultureInfo.InvariantCulture,
                "The specified object is an instance of '{0}' while only instances of '{1}' are allowed for this method.",
                obj.GetType(), this.GetType());
            throw new ArgumentException(errorMessage, nameof(obj));
        }

        return result;
    }

    #endregion // IComparable Members

    #region IComparable<T> Members

    /// <summary> Compares the current object with another object of the same type.</summary>
    ///
    /// <param name="other"> An object to compare with this object. </param>
    ///
    /// <returns> Negative if this object is less than the other, 0 if they are equal, or positive if this is greater.</returns>
    public int CompareTo(T other)
    {
        int result;

        if (other == null)
            result = Compare(this, null);
        else
            result = (this as IComparable<CaseInsensitiveId<T>>).CompareTo(other as CaseInsensitiveId<T>);

        return result;
    }
    #endregion // IComparable<T> Members

    #region IComparable<CaseInsensitiveId<T>> Members

    /// <summary> Compares this object to another to determine their relative ordering.</summary>
    ///
    /// <param name="other"> Another instance to compare. It may equal to null. </param>
    ///
    /// <returns> Negative if this object is less than the other, 0 if they are equal, or positive if this is greater.</returns>
    public int CompareTo(CaseInsensitiveId<T> other)
    {
        int result;

        if (other == null)
            result = Compare(this, null);
        else
            result = ValueComparer.Compare(this.ID, other.ID);

        return result;
    }
    #endregion // IComparable<CaseInsensitiveId<T>> Members
}

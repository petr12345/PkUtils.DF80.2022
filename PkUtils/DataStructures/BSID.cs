// Ignore Spelling: BSID
// 
// Ignore Spelling: Utils
//
using System;


namespace PK.PkUtils.DataStructures;

// Suppress locally the warning CS0660:
// 'BSID' defines operator == or operator != but does not override Object.Equals(object o)
// One can do that, since base.Equals implementable is sufficient here.
//
#pragma warning disable 660


// Suppress locally the warning CS0661:
// 'BSID' defines operator == or operator != but does not override Object.GetHashCode()
// One can do that, since base.GetHashCode implementable is sufficient here.
#pragma warning disable 661

/// <summary> A wrapper class around Security ID.</summary>
[Serializable]
public class BSID : CaseInsensitiveId<BSID>
{
    #region Constructor(s)

    /// <summary> Constructor accepting string representation of ID.</summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="id"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when a supplied <paramref name="id"/> is empty. </exception>
    ///
    /// <param name="id"> The identifier. Can't be null or empty</param>
    public BSID(string id)
        : base(id)
    { }

    /// <summary> Copy-like constructor, accepting another BSDI.</summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="id"/> is null. </exception>
    ///
    /// <param name="id"> The identifier. Can't be null. </param>
    public BSID(BSID id)
        : base(id)
    { }
    #endregion // Constructor(s)

    #region Static Operators

    /// <summary> Converts a string to a BSID. </summary>
    /// <param name="id"> The id to process. </param>
    /// <returns> A new BSID constructed with <paramref name="id"/> argument.</returns>
    public static explicit operator BSID(string id)
    {
        return new BSID(id);
    }

    /// <summary> Converts a BSID to a string. </summary>
    /// <param name="id"> The <see cref="BSID"/> identifier. </param>
    /// <returns> For a non-null <paramref name="id"/> returns a id.ID; otherwise null. </returns>
    public static explicit operator string(BSID id)
    {
        string result;

        if (id == null)
            result = null;
        else
            result = id.ID;

        return result;
    }

    /// <summary> The equality operator.</summary>
    ///
    /// <param name="first"> The first operand. </param>
    /// <param name="second"> The second operand. </param>
    ///
    /// <returns> Returns true if both operands have the same value.</returns>
    public static bool operator ==(BSID first, BSID second)
    {
        if (ReferenceEquals(first, second)) return true;
        if ((first is null) || (second is null)) return false;

        return first.Equals(second);
    }

    /// <summary> The non-equality operator.</summary>
    ///
    /// <param name="first"> The first operand. </param>
    /// <param name="second"> The second operand. </param>
    ///
    /// <returns> Returns true if both operands do not have the same value.</returns>
    public static bool operator !=(BSID first, BSID second)
    {
        return !(first == second);
    }

    /// <summary> Less-than comparison operator.</summary>
    ///
    /// <param name="left"> The first instance to compare. </param>
    /// <param name="right"> The second instance to compare. </param>
    ///
    /// <returns> The result of the comparison.</returns>
    public static bool operator <(BSID left, BSID right)
    {
        return (Compare(left, right) < 0);
    }

    /// <summary> Greater-than comparison operator.</summary>
    ///
    /// <param name="left"> The first instance to compare. </param>
    /// <param name="right"> The second instance to compare. </param>
    ///
    /// <returns> The result of the comparison.</returns>
    public static bool operator >(BSID left, BSID right)
    {
        return (Compare(left, right) > 0);
    }
    #endregion // Static Operators

    #region Methods

    /// <summary> Compares two BSID objects to determine their relative ordering.</summary>
    ///
    /// <param name="left"> The 'left' instance to be compared. May equal to null. </param>
    /// <param name="right"> The 'right' to be compared. May equal to null. </param>
    ///
    /// <returns> Negative if 'left' is less than 'right', 0 if they are equal, or positive if it is greater.</returns>
    public static int Compare(BSID left, BSID right)
    {
        return CaseInsensitiveId<BSID>.Compare(left, right);
    }
    #endregion // Methods
}

// Ignore Spelling: Comparers, Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace PK.PkUtils.Comparers;

/// <summary>
/// This comparer provides a code simplification for scenario when checking objects equality by
/// their primary key comparisons; for example, by the CustomerID property of some Customer class. <br/>
/// 
/// Assuming we have a object to key conversion delegate, this comparer works as its wrapper
/// implementing IEqualityComparer, that could be further used in several Linq methods. This is similar
/// approach as used in <see cref="FunctionalEqualityComparer{T}"/>. 
/// Code example: <br/>
/// 
/// <code>
/// <![CDATA[
/// var x = new Customer(1);
/// var y = new Customer(2);
/// var list = new List<Customer>(){ new Customer(1), new Customer(2)};
/// list.Contains(new Customer(1), z => z.CustomerID);
/// ]]>
/// </code>
/// 
/// or the other code example
/// <code>
/// <![CDATA[
///  var foo = new List<string> { "abc", "de", "DE" };
///  // case-insensitive distinct constructing KeyEqualityComparer directly
///  var distinct1 = foo.Distinct<string>(new KeyEqualityComparer<string, string>( x => x.ToLower() ) );
///  // case-insensitive distinct constructing KeyEqualityComparer indirectly inside extension method of UsageKeyEqualityComparer
///  var distinct2 = foo.Distinct<string, string>(x => x.ToLower());
/// ]]>
/// </code> 
/// </summary>
///
/// <typeparam name="T">    The type of compared objects. </typeparam>
/// <typeparam name="TKey"> The type of keys to which the compared objects are eventually converted 
///                         for comparison purpose. </typeparam>
///
/// <seealso href="http://blog.lavablast.com/post/2010/05/05/Lambda-IEqualityComparer3cT3e.aspx">
/// Lambda IEqualityComparer{T}</seealso>
/// <seealso cref="FunctionalEqualityComparer{T}"/>
public class KeyEqualityComparer<T, TKey> : IEqualityComparer<T>
{
    #region Fields
    /// <summary> A delegate converting the compared values to compared keys.</summary>
    private readonly Func<T, TKey> _keyExtractorFn;
    /// <summary> The key comparer, provided by constructor (if any). </summary>
    private readonly IEqualityComparer<TKey> _keyComparer;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Constructor with one delegate argument. <br/>
    /// Allows us to simply specify the key to compare with, like: y =&gt; y.CustomerID.
    /// </summary>
    ///
    /// <param name="keyExtractor"> A delegate converting the compared values to compared keys.  
    /// Can't be null. </param>
    public KeyEqualityComparer(Func<T, TKey> keyExtractor)
      : this(keyExtractor, null)
    { }

    /// <summary> Constructor with provided key extractor and key comparer arguments. <br/> </summary>
    /// <param name="keyExtractor"> Method converting the compared values to compared keys. Must not equal
    ///  to null. </param>
    /// <param name="keyComparer"> Key comparer. May be null, in that case just "regular" methods
    ///  GetHashCode for getting key hash code and Equals for keys comparison will be used. </param>
    public KeyEqualityComparer(Func<T, TKey> keyExtractor, IEqualityComparer<TKey> keyComparer)
    {
        ArgumentNullException.ThrowIfNull(keyExtractor);

        _keyExtractorFn = keyExtractor;
        _keyComparer = keyComparer;
        ValidateMe();
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Return the key extractor delegate, initialized by constructor.
    /// </summary>
    protected Func<T, TKey> KeyExtractorFn
    {
        get => _keyExtractorFn;
    }

    /// <summary>
    /// Return the IEqualityComparer for <typeparamref name="TKey"/>, initialized by constructor.
    /// </summary>
    protected IEqualityComparer<TKey> KeyComparer
    {
        get => _keyComparer;
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Non-virtual public method validating the object instance.
    /// </summary>
    [Conditional("DEBUG")]
    public void AssertValid()
    {
        ValidateMe();
    }

    /// <summary>
    /// Non-virtual protected method validating an instance of this type. 
    /// </summary>
    [Conditional("DEBUG")]
    protected void ValidateMe()
    {
        Debug.Assert((this._keyExtractorFn != null));
    }
    #endregion // Methods

    #region IEqualityComparer<T> Members

    /// <inheritdoc/>
    public bool Equals(T x, T y)
    {
        TKey keyX = KeyExtractorFn(x);
        TKey keyY = KeyExtractorFn(y);
        bool result;

        if (null != KeyComparer)
        {
            result = KeyComparer.Equals(keyX, keyY);
        }
        else
        {
            IEnumerable<object> iEnY;

            // Determine the special case where we pass a list of keys
            if (keyX is IEnumerable<object> iEnX)
            {
                iEnY = keyY as IEnumerable<object>;
                Debug.Assert(null != iEnY);
                result = iEnX.SequenceEqual(iEnY);
            }
            else
            {
                result = keyX.Equals(keyY);
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public int GetHashCode(T obj)
    {
        TKey key = KeyExtractorFn(obj);
        int nRes = 0;

        if (null != KeyComparer)
        {
            nRes = KeyComparer.GetHashCode(key);
        }
        else if (key is IEnumerable<object> iEn) // The special case where we pass a list of keys
        {
            nRes = (int)(iEn).Aggregate((x, y) => x.GetHashCode() ^ y.GetHashCode());
        }
        else
        {
            nRes = key.GetHashCode();
        }

        return nRes;
    }
    #endregion // IEqualityComparer<T> Members
}

/// <summary> A static class working as a factory of generic <see cref="KeyEqualityComparer{T, TKey}"/>. </summary>
public static class KeyEqualityComparer
{
    /// <summary> Creates a new KeyEqualityComparer instance. </summary>
    ///
    /// <typeparam name="T">    Generic type parameter - type of compared ob objects. </typeparam>
    /// <typeparam name="TKey"> Type of the key. </typeparam>
    /// <param name="keyExtractor"> Method converting the compared values to compared keys. 
    /// Must not equal to null. </param>
    /// <param name="keyComparer"> (Optional) Key comparer. 
    /// May be null, in that case just "regular" methods GetHashCode for getting key hash code
    /// and Equals for keys comparison will be used. </param>
    /// <returns>   A new <see cref="KeyEqualityComparer{T, TKey}"/> instance. </returns>
    public static KeyEqualityComparer<T, TKey> Create<T, TKey>(
        Func<T, TKey> keyExtractor,
        IEqualityComparer<TKey> keyComparer = null)
    {
        return new KeyEqualityComparer<T, TKey>(keyExtractor, keyComparer);
    }
}

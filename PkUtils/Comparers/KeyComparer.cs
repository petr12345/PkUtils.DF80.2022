using System;
using System.Collections.Generic;
using System.Diagnostics;

// Ignore Spelling: Utils, Comparers
// 

namespace PK.PkUtils.Comparers;

/// <summary>
/// This comparer provides a code simplification for scenario when comparing objects
/// by their primary key comparisons; for example, by the CustomerName property of some Customer class. <br/>
/// 
/// Assuming we have a object -&gt; key conversion delegate, and optional key comparer,
/// this comparer works as their wrapper, implementing IComparer, that could be further used in several Linq methods.
/// This is similar approach as used in <see cref="FunctionalEqualityComparer{T}"/>.
/// </summary>
///
/// <typeparam name="T">    The type of compared objects. </typeparam>
/// <typeparam name="TKey"> The type of keys to which the compared objects are eventually converted 
///                         for comparison purpose. </typeparam>
/// <seealso cref="FunctionalEqualityComparer{T}"/>
[CLSCompliant(true)]
public class KeyComparer<T, TKey> : IComparer<T>
{
    #region Fields

    /// <summary> A delegate converting the compared values to compared keys.</summary>
    private readonly Func<T, TKey> _keyExtractorFn;

    /// <summary> The key comparer, provided by constructor (if any). </summary>
    private readonly IComparer<TKey> _keyComparer;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Constructor with one delegate argument. <br/>
    /// Allows us to simply the extraction of key to compare, like: y => y.CustomerName
    /// </summary>
    /// <param name="keyExtractor">A delegate converting the compared values to compared keys.</param>
    public KeyComparer(Func<T, TKey> keyExtractor)
      : this(keyExtractor, null)
    { }

    /// <summary> Constructor with provided key extractor and key comparer arguments. <br/> </summary>
    /// <param name="keyExtractor"> Method converting the compared values to compared keys. Must not equal to null. </param>
    /// <param name="keyComparer"> Key comparer. May be null, in that case just default Comparer will be used. </param>
    public KeyComparer(Func<T, TKey> keyExtractor, IComparer<TKey> keyComparer)
    {
        ArgumentNullException.ThrowIfNull(keyExtractor);

        this._keyExtractorFn = keyExtractor;
        this._keyComparer = keyComparer;
        ValidateMe();
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Return the key extractor delegate, initialized by constructor.
    /// </summary>
    protected Func<T, TKey> KeyExtractorFn
    {
        get { return _keyExtractorFn; }
    }

    /// <summary>
    /// Return the IComparer for <typeparamref name="TKey"/>, initialized by constructor.
    /// </summary>
    protected IComparer<TKey> Comparer
    {
        get { return _keyComparer; }
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

    #region IComparer<T> Members

    /// <inheritdoc/>
    public int Compare(T x, T y)
    {
        TKey keyX = KeyExtractorFn(x);
        TKey keyY = KeyExtractorFn(y);
        IComparer<TKey> comparer = Comparer ?? Comparer<TKey>.Default;
        int nRes = comparer.Compare(keyX, keyY);

        return nRes;
    }
    #endregion // IComparer<T> Members
}

/// <summary> A static class working as a factory of generic <see cref="KeyComparer{T, TKey}"/>. </summary>
public static class KeyComparer
{
    /// <summary> Creates a new KeyComparer instance. </summary>
    ///
    /// <typeparam name="T">  Generic type parameter - type of compared ob objects. </typeparam>
    /// <typeparam name="TKey"> Type of the key. </typeparam>
    /// <param name="keyExtractor"> Method converting the compared values to compared keys. 
    ///                             Must not equal to null. </param>
    /// <param name="keyComparer"> Key comparer. 
    ///                            May be null, in that case just default Comparer will be used. </param>
    /// <returns>   A new <see cref="KeyComparer{T, TKey}"/> instance. </returns>
    public static KeyComparer<T, TKey> Create<T, TKey>(
        Func<T, TKey> keyExtractor,
        IComparer<TKey> keyComparer = null)
    {
        return new KeyComparer<T, TKey>(keyExtractor, keyComparer);
    }
}

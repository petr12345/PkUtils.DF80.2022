/***************************************************************************************************************
*
* FILE NAME:   .\Comparers\FunctionalEqualityComparer.cs
*
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: Contains the generic class FunctionalEqualityComparer<T> implementing IEqualityComparer<T>,
*              and static class FunctionalEqualityComparer
*
**************************************************************************************************************/


// Ignore Spelling: Utils, Comparers
//
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace PK.PkUtils.Comparers;

/// <summary>
/// A convenient comparison wrapper, that wraps a method <![CDATA[Func<T, T, bool>]]> to implement
/// <see cref="IEqualityComparer{T}"/>. <br/>
/// This way, you can call Linq methods like Distinct or Except, which require comparer object,
/// while using a lambda expression as argument of constructor of the comparer.<br/>
/// </summary>
/// 
/// <example>
/// <code language="xml" title="A simple example">
/// <![CDATA[
///   Func<int, int, bool> f = (x, y) => x == y;
///   var comparer = new FunctionalEqualityComparer<int>(f);
///   Console.WriteLine(comparer.Equals(1, 1));
///   Console.WriteLine(comparer.Equals(1, 2));
/// ]]>
/// </code>
/// </example>
///
/// <remarks>
/// While one of the constructors requires both the <![CDATA[Func<T, T, bool>]]> comparer 
/// and the <![CDATA[Func<T, int>]]> hash, the other requires just the comparer <![CDATA[Func<T, T, bool>]]>,
/// and delegates the call to the first constructor, with the default hash implementation. <br/>
///
/// Be aware that except the most trivial cases, you should provide explicitly both arguments (both the
/// comparer and the hash functions). Otherwise, you will easily end-up with invalid behaviour -
/// - a comparer that considers two objects as equal in terms of Equals method comparison,
/// but returns different hash codes for them. <br/>
/// Such behaviour is breaking basic concepts, since nearly all of related Linq methods that involve
/// IEqualityComparer rely on hash codes to work properly, because they utilize hash tables internally for
/// efficiency. <br/>
/// Take Distinct, for example. Consider the implications of this extension method if all it utilized were an
/// Equals method. How do you determine whether an item's already been scanned in a sequence if you only have
/// Equals? You enumerate over the entire collection of values you've already looked at and check for a match.
/// This would result in Distinct using a worst-case O(N2) algorithm instead of an O(N) one! <br/>
/// 
/// Fortunately, this isn't the case. Distinct doesn't just use Equals; it uses GetHashCode as well.
/// In fact, it absolutely does not work properly without an <![CDATA[IEqualityComparer<T>]]>
/// that supplies a proper GetHashCode. If you think about it, it would make sense for Distinct to use a
/// HashSet{T} (or equivalent) internally, and for GroupBy to use something like 
/// a <![CDATA[Dictionary<TKey, List<T>>]]> internally. And that's what actually happens.
/// </remarks>
///
/// <typeparam name="T"> The type of objects to compare. <br/>
/// This type parameter is contravariant. That is, in the assignment to variable of <![CDATA[IEqualityComparer<T>]]>
/// you can use an instance based on generic type which is less derived.
/// <example>
/// <code language="xml" title="A simple example">
/// <![CDATA[
///  Func<FileSystemInfo, FileSystemInfo, bool> fsComparer = 
///     (x, y) => string.Equals(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
///  Func<FileSystemInfo, int> fsHash = 
///    (x) => x.FullName.ToUpperInvariant().GetHashCode();
/// 
///  Func<FileInfo, FileInfo, bool> fileComparer = 
///    (x, y) => string.Equals(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
///  Func<FileInfo, int> fileHash = 
///    (x) => x.FullName.ToUpperInvariant().GetHashCode();
/// 
///  IEqualityComparer<FileSystemInfo> fsComp = new FunctionalEqualityComparer<FileSystemInfo>(fsComparer, fsHash);
///  IEqualityComparer<FileInfo> fiComp = new FunctionalEqualityComparer<FileInfo>(fileComparer, fileHash);
/// 
///  // Since FileSystemInfo is less derived than FileInfo (which inherits from it), 
///  // one can assign:
///  fiComp = fsComp;
///  // but one cannot assign following:
///  fsComp = fiComp;
/// ]]>
/// </code>
/// </example>
/// 
/// For more information about covariance and contravariance, please have a look at
/// <a href="http://msdn.microsoft.com/en-us/library/dd799517">Covariance and Contravariance in Generics.</a>. 
///  </typeparam>
/// 
/// <seealso href="http://msdn.microsoft.com/en-us/magazine/ff796223.aspx">
/// New C# Features in the .NET Framework 4
/// </seealso>
/// 
/// <seealso href="http://stackoverflow.com/questions/98033/wrap-a-delegate-in-an-iequalitycomparer">
/// StackOverflow: Wrap a delegate in an IEqualityComparer
/// </seealso>
/// <seealso cref="KeyEqualityComparer{T, TKey}"/>
[CLSCompliant(true)]
public class FunctionalEqualityComparer<T> : IEqualityComparer<T>
{
    #region Fields
    /// <summary> The comparison function initialized by constructor. </summary>
    protected readonly Func<T, T, bool> _comparerFn;
    /// <summary> The hash function initialized by constructor. </summary>
    protected readonly Func<T, int> _hashFn;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Constructor accepting both the comparer function and the has function.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when any of input arguments is null.</exception>
    /// <param name="comparerFn">The comparison function that will be used for <typeparamref name="T"/> 
    /// instances comparison. Must not be equal to null.</param>
    /// <param name="hashFn">The hash function that will be used for computing a hash code 
    /// for the specified <typeparamref name="T"/> instance. Must not be equal to null.</param>
    public FunctionalEqualityComparer(Func<T, T, bool> comparerFn, Func<T, int> hashFn)
    {
        ArgumentNullException.ThrowIfNull(comparerFn, nameof(comparerFn));
        ArgumentNullException.ThrowIfNull(hashFn, nameof(hashFn));

        _comparerFn = comparerFn;
        _hashFn = hashFn;
        this.ValidateMe();
    }

    /// <summary>
    /// Constructor accepting the comparer function only; delegates the call to overloaded constructor,
    /// using as a hash function the default T.GetHashCode() implementation. Be aware this might not be
    /// correct - see the remarks in the class description for details.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when the input argument
    /// <paramref name="comparerFn"/>is null. </exception>
    /// <param name="comparerFn"> The comparison function that will be used for comparison of 
    /// <typeparamref name="T"/> instances. </param>
    public FunctionalEqualityComparer(Func<T, T, bool> comparerFn)
      : this(comparerFn, t => t.GetHashCode())
    { }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Return the comparison function initialized by constructor
    /// </summary>
    protected Func<T, T, bool> ComparerFn
    {
        get { return _comparerFn; }
    }

    /// <summary>
    /// Return the hashing function initialized by constructor
    /// </summary>
    protected Func<T, int> HashFn
    {
        get { return _hashFn; }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Virtual method validating the object instance.
    /// </summary>
    [Conditional("DEBUG")]
    public virtual void AssertValid()
    {
        ValidateMe();
    }

    /// <summary>
    /// Non-virtual method validating an instance of this type. 
    /// The reason of existence of this method is to avoid calling virtual method from constructor.
    /// </summary>
    [Conditional("DEBUG")]
    protected void ValidateMe()
    {
        Debug.Assert((this._comparerFn != null) && (this._hashFn != null));
    }
    #endregion // Methods

    #region IEqualityComparer<T> Members

    /// <summary>
    /// Implements IEqualityComparer.Equals
    /// </summary>
    /// <param name="x">The first object of type T to compare</param>
    /// <param name="y">The second object of type T to compare</param>
    /// <returns>true if the specified objects are equal; otherwise, false</returns>
    public bool Equals(T x, T y)
    {
        if (ComparerFn(x, y))
        {
            Debug.Assert(GetHashCode(x) == GetHashCode(y));
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Implements IEqualityComparer.GetHashCode
    /// </summary>
    /// <param name="obj">The Object for which a hash code is to be returned.</param>
    /// <returns>A hash code for the specified object.</returns>
    public int GetHashCode(T obj)
    {
        return HashFn(obj);
    }
    #endregion // IEqualityComparer<T> Members
}

/// <summary> A static class working as a factory of generic <see cref="FunctionalEqualityComparer{T}"/>. </summary>
public static class FunctionalEqualityComparer
{
    /// <summary> Creates a new instance of <see cref="FunctionalEqualityComparer{T}"/>. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Passed by called constructor when any of input
    ///  arguments is null. </exception>
    /// <typeparam name="T"> The type of objects to compare. </typeparam>
    /// 
    /// <param name="comparerFn"> The comparison function that will be used for <typeparamref name="T"/>
    ///  instances comparison. </param>
    /// <param name="hashFn"> The hash function that will be used for computing a hash code for the
    ///  specified <typeparamref name="T"/> instance. Must not be equal to null. </param>
    /// <returns> A new <see cref="FunctionalEqualityComparer{T}"/> instance. </returns>
    public static FunctionalEqualityComparer<T> Create<T>(
        Func<T, T, bool> comparerFn,
        Func<T, int> hashFn)
    {
        return new FunctionalEqualityComparer<T>(comparerFn, hashFn);
    }

    /// <summary> Creates a new instance of <see cref="FunctionalEqualityComparer{T}"/>. </summary>
    ///
    /// <exception cref="System.ArgumentNullException"> Passed by called constructor when the input argument
    ///  <paramref name="comparerFn"/>is null. </exception>
    /// <typeparam name="T"> The type of objects to compare. </typeparam>
    /// 
    /// <param name="comparerFn"> The comparison function that will be used for <typeparamref name="T"/>
    /// instances comparison. </param>
    /// <returns> A new <see cref="FunctionalEqualityComparer{T}"/> instance. </returns>
    public static FunctionalEqualityComparer<T> Create<T>(Func<T, T, bool> comparerFn)
    {
        return Create<T>(comparerFn, t => t.GetHashCode());
    }

    /// <summary>
    /// Creates a new instance of <see cref="FunctionalEqualityComparer{T}"/>, which is "safe" (does not throw
    /// a NullReferenceException) even if one or both of two argument of comparison are null,
    /// regardless whether <paramref name="comparerFn"/> handles that well or not. 
    /// It also computes without exception a hash of a null value, regardless whether the original delegate
    /// <paramref name="hashFn"/> handles that well or not<br/>
    /// 
    /// This is achieved by substituting a <paramref name="comparerFn"/> and <paramref name="hashFn"/>
    /// by a 'null-safe' wrappers, which delegates to original functions only if arguments are not null.
    /// </summary>
    ///
    /// <typeparam name="T"> The type of objects to compare. </typeparam>
    /// <param name="comparerFn"> The comparison function that will be used for <typeparamref name="T"/>
    /// instances comparison, if none of input arguments is null. </param>
    /// <param name="hashFn"> The hash function that will be used for computing a hash code for the
    ///  specified <typeparamref name="T"/> instance, if input argument is not null. </param>
    ///
    /// <returns> A new <see cref="FunctionalEqualityComparer{T}"/> instance.</returns>
    public static FunctionalEqualityComparer<T> CreateNullSafeComparer<T>(
        Func<T, T, bool> comparerFn,
        Func<T, int> hashFn)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(comparerFn);
        ArgumentNullException.ThrowIfNull(hashFn);

        bool fsEqualWrapper(T x, T y) => object.ReferenceEquals(x, y) ||
            (
              (x != null) && (y != null) && comparerFn(x, y)
            );
        int fsHashWrapper(T x) => (x == null) ? 0 : hashFn(x);

        return new FunctionalEqualityComparer<T>(fsEqualWrapper, fsHashWrapper);
    }
}

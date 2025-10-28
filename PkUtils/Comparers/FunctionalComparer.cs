// Ignore Spelling: Utils, Comparers
// 
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace PK.PkUtils.Comparers;

/// <summary>
/// A convenient comparison wrapper, that wraps a method <![CDATA[Func<T, T, int>]]> to implement
/// <see cref="IComparer{T}"/>. <br/>
/// This way, you can call Linq methods like OrderBy or ThenBy, which require comparer object,
/// while using a lambda expression as argument of constructor of the comparer.<br/>
/// </summary>
/// 
/// <example>
/// <code language="xml" title="A simple example">
/// <![CDATA[
///  Comparison<string> f = (x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
///  var comparer = new FunctionalComparer<string>(f);
/// 
///  Console.WriteLine(comparer.Compare("aaa", "AAA"));
///  Console.WriteLine(comparer.Compare("aaa", "bbb"));
/// ]]>
/// </code>
/// </example>
///
/// <remarks>
/// </remarks>
///
/// <typeparam name="T"> The type of objects to compare. <br/>
/// This type parameter is contravariant. That is, in the assignment to variable 
/// of <![CDATA[IComparer<T>]]> you can use an instance based on generic type which is less derived.
/// <example>
/// <code language="xml" title="A simple example">
/// <![CDATA[
///   Comparison<FileSystemInfo> fsComparer =
///      (x, y) => string.Compare(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
/// 
///   Comparison<FileInfo> fileComparer =
///     (x, y) => string.Compare(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
///     
///   IComparer<FileSystemInfo> fsComp = new FunctionalComparer<FileSystemInfo>(fsComparer);
///   IComparer<FileInfo> fiComp = new FunctionalComparer<FileInfo>(fileComparer);
///     
///   // The type argument of IComparer is contravariant.
///   // Since FileSystemInfo is less derived than FileInfo (which inherits from it), 
///   // one can assign:
///   fiComp = fsComp;
///   // but one cannot assign following:
///   fsComp = fiComp;
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
[CLSCompliant(true)]
public class FunctionalComparer<T> : IComparer<T>
{
    #region Fields
    /// <summary> The comparison function initialized by constructor. </summary>
    private readonly Comparison<T> _comparerFn;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Constructor accepting the comparer function. </summary>
    /// <param name="comparerFn"> The comparison function that will be used for <typeparamref name="T"/>
    ///  instances comparison. </param>
    public FunctionalComparer(Comparison<T> comparerFn)
    {
        ArgumentNullException.ThrowIfNull(comparerFn);

        _comparerFn = comparerFn;
        this.ValidateMe();
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Return the comparison function initialized by constructor
    /// </summary>
    protected Comparison<T> ComparerFn
    {
        get { return _comparerFn; }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Method validating the object instance.
    /// </summary>
    [Conditional("DEBUG")]
    public void AssertValid()
    {
        ValidateMe();
    }

    /// <summary>
    /// Protected method actually validating an instance of this type. 
    /// </summary>
    [Conditional("DEBUG")]
    protected void ValidateMe()
    {
        Debug.Assert(this._comparerFn != null);
    }
    #endregion // Methods

    #region IComparer<T> Members

    /// <summary> Implements IComparer.Compare. </summary>
    /// <param name="x"> The first object of type T to compare. </param>
    /// <param name="y"> The second object of type T to compare. </param>
    /// <returns>
    /// Negative if 'x' is less than 'y', 0 if they are equal, or positive if it is greater.
    /// </returns>
    public int Compare(T x, T y)
    {
        AssertValid();
        return ComparerFn(x, y);
    }
    #endregion // IComparer<T> Members
}

/// <summary> A static class working as a factory of generic <see cref="FunctionalComparer{T}"/>. </summary>
public static class FunctionalComparer
{
    /// <summary> Creates a new instance of <see cref="FunctionalComparer{T}"/>. </summary>
    ///
    /// <typeparam name="T"> The type of objects to compare. </typeparam>
    /// <param name="comparerFn"> The comparison function that will be used for <typeparamref name="T"/>
    /// instances comparison. </param>
    ///
    /// <returns> A new <see cref="FunctionalComparer{T}"/> instance.</returns>
    public static FunctionalComparer<T> Create<T>(Comparison<T> comparerFn)
    {
        return new FunctionalComparer<T>(comparerFn);
    }

    /// <summary>
    /// Creates a new instance of <see cref="FunctionalComparer{T}"/>, which is "safe" (does not throw 
    /// a NullReferenceException) even if one or both of two argument of comparison are null,
    /// regardless whether <paramref name="comparerFn"/> handles that well or not. <br/>
    /// 
    /// This is achieved by substituting a <paramref name="comparerFn"/> by a 'null-safe' wrapper, 
    /// which delegates to <paramref name="comparerFn"/> only if both input arguments are not null.
    /// </summary>
    ///
    /// <typeparam name="T"> The type of objects to compare. </typeparam>
    /// <param name="comparerFn"> The comparison function that will be used for <typeparamref name="T"/>
    /// instances comparison, if none of input arguments is null. </param>
    ///
    /// <returns> A new <see cref="FunctionalComparer{T}"/> instance.</returns>
    public static FunctionalComparer<T> CreateNullSafeComparer<T>(Comparison<T> comparerFn) where T : class
    {
        ArgumentNullException.ThrowIfNull(comparerFn);

        int ComparisonFnWrapper(T x, T y) => ReferenceEquals(x, y) ? 0 :
              (
                (x is null) ? -1 : ((y is null) ? 1 : comparerFn(x, y))
              );

        return new FunctionalComparer<T>(ComparisonFnWrapper);
    }
}



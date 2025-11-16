// Ignore Spelling: Utils
//
using System;
using System.Globalization;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.DataStructures;

/// <summary>
/// <para>
/// A base generic class, that allows to implement easily any countable ( ICountable - interface supporting ) class.
/// For instance, assuming you new class name is WaitCursor, in declaration of such class one has to type just
/// <example>
/// <code language="xml" title="CountableGeneric usage example">
/// <![CDATA[
///   public class WaitCursor : CountableGeneric<WaitCursor>
///   {
///     // your specific stuff here ...
///   }
/// ]]>
/// </code>
/// </example>
/// </para>
/// 
/// <para>
/// Any class supporting <see cref="PK.PkUtils.Interfaces.ICountable"/> keeps track of
/// count of all "living" instances of the same type. The object is considered living, if it has
/// not been disposed explicitly by Dispose or by the finalizer. <br/>
/// </para>
/// 
/// <para>
/// Using the generics approach, you can make just any class countable without the request to
/// derive it from a particular base class ( which is not possible if the class needs to derive
/// from other base class, since multiple inheritance is not supported by C# or rather by .NET
/// ).<br/>
/// </para> </summary>
/// 
/// <typeparam name="T"> The class that you want to make countable. </typeparam>
/// 
/// <remarks>
/// Besides the multiple inheritance issue, unfortunately there is another problem with generics
/// approach, since in C# (unlike in C++) one cannot specify the base class the generic could
/// derive from. So, the code like following produces compilation error <i>"Cannot derive from 'R'
/// because it is a type parameter"</i> <br/>
/// <code>
/// <![CDATA[
///   public class CountableGeneric<T, R> : R
/// ]]>
/// </code>
/// Therefore, you cannot derive your generic from the other class provided as type argument;
/// but at least you could inherit from generic base class, with self as type parameter.
/// This approach is called "Curiously recurring templates/generics pattern". <br/>
/// Code example:
/// <code>
/// <![CDATA[
///   public class GGG : CountableGeneric<GGG>
///   {
///     ...
///   }
/// ]]>
/// </code>
/// </remarks>
///
/// <seealso href="http://social.msdn.microsoft.com/Forums/is/csharplanguage/thread/a92428ec-0ea0-49cb-b942-124f36661e98">
/// MSDN forum on Inheriting from generic base class, with self as type parameter</seealso>
/// <seealso href="http://blogs.msdn.com/b/oldnewthing/archive/2009/08/14/9869049.aspx">
/// MSDN blog on Why can't I declare a type that derives from a generic type parameter?</seealso>
[CLSCompliant(true)]
public class CountableGeneric<T> : ICountable, IDisposableEx
{
    #region Fields

    /// <summary>
    /// The object order, which has been initialized at the moment of this instance creation ( the actual value of
    /// order index, that the implementation of <see cref="PK.PkUtils.DataStructures.CountMap"/>
    /// maintains for this specific type).
    /// </summary>
    /// <remarks> The order is zero-based.</remarks>
    protected readonly int _order;

    /// <summary>
    /// Has been the object disposed?
    /// </summary>
    protected bool _disposed;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Argument-less constructor.
    /// </summary>
    public CountableGeneric()
    {
        CountMap.Instance.IncrementCount(this.GetType(), out _order);
    }
    #endregion // Constructor(s)

    #region Finalizer
    /// <summary>
    /// The Finalizer. Uses C# destructor syntax for generation of finalizer method code.
    /// The actually generated method (finalizer) will run only if the Dispose method
    /// does not get called.
    /// </summary>
    /// <remarks>
    /// Note: the compiler actually expands the pseudo-destructor syntax here to following code:
    /// <code>
    /// protected override void Finalize()
    /// {
    ///   try
    ///   { // Your cleanup code here
    ///     Dispose(false);
    ///   }
    ///   finally
    ///   {
    ///     base.Finalize();
    ///   }
    /// }
    /// </code>
    /// </remarks>
    /// <seealso href="http://www.devx.com/dotnet/Article/33167/1954">
    /// When and How to Use Dispose and Finalize in C#</seealso>
    ~CountableGeneric()
    {
        Dispose(false);
    }
    #endregion // Finalizer

    #region Methods

    #region Public Methods

    /// <summary> Returns a string that represents the current object. </summary>
    /// <returns> A string that represents the current object. </returns>
    public override string ToString()
    {
        int nIncludingDescendants = CountIncludingDescendants;
        int nDescendants = CountDescendants;
        string strType = TypeExtensions.TypeToReadable(this.GetType());
        string strRes = string.Format(CultureInfo.InvariantCulture,
          "Instance of '{0}', created in order as {1}. In total there is {2} living instances including {3} descendants.",
          strType, Order, nIncludingDescendants, nDescendants);

        return strRes;
    }
    #endregion // Public Methods

    #region Protected Methods

    /// <summary>
    /// Disposing of countable object. The implementation is thread-safe.
    /// To make it thread-safe, the implementation uses double-checked locking.
    /// </summary>
    /// <param name="disposing"> 
    /// If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        // Use double-checked locking 
        if (!this.IsDisposed)
        {
            CountMap.Instance.SlimLock.EnterWriteLock();
            try
            {
                if (!this.IsDisposed)
                {
                    // If disposing equals true, dispose both managed and unmanaged resources.
                    if (disposing)
                    {
                        // Dispose managed resources here ( if there are any )
                    }
                    // Now release unmanaged resources. If disposing is false, only that code is executed.
                    // Actually nothing to do here for this particular class

                    // The epilogue code, decrements the object counter and sets _disposed to true.
                    // Note IT IS THREAD-SAFE.

                    _disposed = true;
                    CountMap.Instance.DecrementCount(this.GetType());
                }
            }
            finally
            {
                CountMap.Instance.SlimLock.ExitWriteLock();
            }
        }
    }
    #endregion // Protected Methods
    #endregion // Methods

    #region ICountable Members

    /// <summary>
    /// The current count of objects of the same type in 'living state'.
    /// Counts only objects of exactly the same type.
    /// </summary>
    /// <returns></returns>
    public int CountExact
    {
        get { return CountMapExt.GetCountExact(this.GetType()); }
    }

    /// <summary>
    /// The current count of this type descendants in 'living state'.
    /// Counts only descendants, NOT objects of exactly the same type.
    /// </summary>
    /// <returns></returns>
    public int CountDescendants
    {
        get { return CountMapExt.GetCountDescendants(this.GetType()); }
    }

    /// <summary>
    /// The current count of all objects of the same type AND derived types.
    /// </summary>
    public int CountIncludingDescendants
    {
        get { return CountMapExt.GetCountIncludingDescendants(this.GetType()); }
    }

    /// <summary>
    /// The object order, which has been initialized as an index of creation
    /// (specific for this object type).
    /// </summary>
    /// <remarks> The order is zero-based.</remarks>
    public int Order
    {
        get { return _order; }
    }
    #endregion // ICountable Members

    #region IDisposableEx Members
    #region IDisposable Members

    /// <summary>
    /// Implements IDisposable.
    /// Do not make this method virtual.
    /// A derived class should not be able to override this method.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        // Take yourself off the Finalization queue to prevent finalization code 
        // for this object from executing a second time.
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposable Members

    /// <summary>
    /// Returns true in case the object has been disposed and no longer should be used.
    /// As with IDisposable.Dispose() method, do NOT make this property virtual.
    /// </summary>
    public bool IsDisposed
    {
        get { return _disposed; }
    }
    #endregion // IDisposableEx Members
}

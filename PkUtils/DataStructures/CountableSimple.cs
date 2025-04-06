/***************************************************************************************************************
*
* FILE NAME:   .\DataStructures\CountableSimple.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains implementation of Countable class
*
**************************************************************************************************************/

// The CountableSimple could be implemented either using CountableGeneric, or quite from the scratch.
#define USE_COUNTABLE_GENERICS_APPROACH

// Ignore Spelling: Utils
//
using System;

namespace PK.PkUtils.DataStructures;

#if USE_COUNTABLE_GENERICS_APPROACH
// With CountableGeneric, everything is quite simple, isn't it

/// <summary>
/// Countable object keeps track of count of all "living" instances of the same type.
/// The object is considered living, if it has not been disposed explicitly by Dispose
/// or by the Finalizer.
/// CountableSimple is a base class that your countable class could derive from.
/// </summary>
[CLSCompliant(true)]
public class CountableSimple : CountableGeneric<CountableSimple>
{
    /// <summary>
    /// Argument-less constructor.
    /// </summary>
    public CountableSimple()
    { }
}

#else // USE_COUNTABLE_GENERICS_APPROACH

// Without usage of CountableGeneric, one just has to code all the countable 
// functionality from the scratch.

/// <summary>
/// Countable object keeps track of count of all "living" instances of the same type.
/// The object is considered living, if it has not been disposed explicitly by Dispose
/// or by the Finalizer.
/// CountableSimple is a base class that your countable class could derive from.
/// </summary>
[CLSCompliant(true)]
public class CountableSimple : ICountable, IDisposableEx
{
#region Fields

/// <summary>
/// The object order, which has been initialized at the moment of this instance creation ( the actual value of
/// order index that the implementation maintains for this specific type).
/// </summary>
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
public CountableSimple()
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
~CountableSimple()
{
  Dispose(false);
}
#endregion // Finalizer

#region Methods
#region Public Methods

/// <summary>
/// The current count of type t objects in 'living state'.
/// Counts only objects of exactly the same type.
/// </summary>
/// <param name="t"></param>
/// <returns></returns>
public static int GetCountExact(Type t)
{
  return CountMap.Instance.GetCountExact(t);
}

/// <summary>
/// The current count of type t descendants in 'living state'.
/// Counts only descendants, NOT objects of exactly the same type.
/// </summary>
/// <param name="t"></param>
/// <returns></returns>
public static int GetCountDescendants(Type t)
{
  return CountMap.Instance.GetCountDescendants(t);
}

/// <summary>
/// Get count of all instances of given type and instances of derived types.
/// </summary>
/// <param name="t"></param>
/// <returns></returns>
public static int GetCountIncludingDescendants(Type t)
{
  return CountMap.Instance.GetCountIncludingDescendants(t);
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
/// </summary>
public bool IsDisposed
{
  get { return _disposed; }
}
#endregion // IDisposableEx Members

#region ICountable Members

/// <summary>
/// The current count of objects of the same type in 'living state'.
/// Counts only objects of exactly the same type.
/// </summary>
/// <returns></returns>
public int CountExact
{
  get { return GetCountExact(this.GetType()); }
}

/// <summary>
/// The current count of this type descendants in 'living state'.
/// Counts only descendants, NOT objects of exactly the same type.
/// </summary>
/// <returns></returns>
public int CountDescendants
{
  get { return GetCountDescendants(this.GetType()); }
}

/// <summary>
/// The current count of all objects of the same type AND derived types.
/// </summary>
public int CountIncludingDescendants
{
  get { return GetCountIncludingDescendants(this.GetType()); }
}

/// <summary>
/// The object order, which has been initialized as an index of creation
/// (specific for this object type).
/// </summary>
public int Order
{
  get { return _order; }
}
#endregion // ICountable Members
}
#endif // USE_COUNTABLE_GENERICS_APPROACH

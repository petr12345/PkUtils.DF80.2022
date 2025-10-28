// Ignore Spelling: Utils, Dict
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using PK.PkUtils.Extensions;
using PK.PkUtils.Utils;

namespace PK.PkUtils.DataStructures;

/// <summary>
/// Auxiliary class, working as a map (dictionary), mapping the type and its 
/// <see cref="CountMap.TypeCountInfo"/> structure. </summary>
///
/// <remarks> For consistency, all public methods are thread-safe. </remarks>
[CLSCompliant(true)]
public class CountMap : Singleton<CountMap>, IDisposable
{
    #region Typedefs

    /// <summary>
    /// Auxiliary structure used in internal dictionary which maps the type and TypeCountInfo. </summary>
    protected struct TypeCountInfo
    {
        /// <summary> Current count of objects of exactly this (related) type. </summary>
        public int _count;

        /// <summary>
        /// An index which increments with each creation of a new instance of related type. </summary>
        public int _OrderIndex;

        /// <summary> The constructor. </summary>
        ///
        /// <param name="nCount"> The initial value of <see cref="Count"/>. </param>
        /// <param name="nOrderIndex"> The initial value of <see cref="OrderIndex"/>. </param>
        internal TypeCountInfo(int nCount, int nOrderIndex)
        {
            _count = nCount;
            _OrderIndex = nOrderIndex;
        }

        /// <summary> Gets the count of objects of exactly this (related) type. </summary>
        public readonly int Count
        {
            get { return _count; }
        }

        /// <summary> Gets an index which increments with each creation of a new instance of related type. </summary>
        public readonly int OrderIndex
        {
            get { return _OrderIndex; }
        }
    }
    #endregion // Typedefs

    #region Fields

    /// <summary> An actual dictionary, mapping the type and its TypeCountInfo. </summary>
    private readonly Dictionary<Type, TypeCountInfo> _Dict = [];

    /// <summary> A slim lock, used for locking/unlocking the dictionary. </summary>
    private ReaderWriterLockSlim _slimLock = new(LockRecursionPolicy.SupportsRecursion);
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Private constructor. Argument-less constructor must be private or protected because of how
    /// Singleton generic requirements. </summary>
    private CountMap()
    {
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// A slim lock, used for locking/unlocking the dictionary of TypeCountInfo values. </summary>
    ///
    /// <remarks>
    /// Declares protected internal access, to make it available from Countable.Dispose, which uses
    /// double-checked locking. ( Is there a better way to do this ?) </remarks>
    ///
    /// <value> The slim lock. </value>
    protected internal ReaderWriterLockSlim SlimLock
    {
        get { return _slimLock; }
    }

    /// <summary>
    /// Property returning the <see cref="IDictionary{Type, TypeCountInfo}"/>mapping the type and its
    /// <see cref="TypeCountInfo"/>.
    /// </summary>
    protected IDictionary<Type, TypeCountInfo> DictCounts
    {
        get { return _Dict; }
    }
    #endregion // Properties

    #region Methods

    #region Public Methods

    /// <summary>
    /// Returns count of "living" instances of given type ( not including the descendants ). </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when the argument <paramref name="t"/> is
    ///   null. </exception>
    ///
    /// <param name="t">  The type for which the information is required. </param>
    ///
    /// <returns> Count of instances of <paramref name="t"/>. </returns>
    public int CountExact(Type t)
    {
        ArgumentNullException.ThrowIfNull(t);

        int nResult = 0;

        SlimLock.EnterReadLock();
        try
        {
            if (DictCounts.TryGetValue(t, out TypeCountInfo info))
            {
                nResult = info.Count;
            }
            else
            {
                TestTypeIsCountable(t);
            }
        }
        finally
        {
            SlimLock.ExitReadLock();
        }
        return nResult;
    }

    /// <summary>
    /// Returns count of "living" instances of descendants of given type ( not including instances of
    /// this type itself ). </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when the argument <paramref name="t"/> is
    ///   null. </exception>
    ///
    /// <param name="t">  The type for which the information is required. </param>
    ///
    /// <returns> Count of instances of descendants of given type <paramref name="t"/>. </returns>
    public int CountDescendants(Type t)
    {
        ArgumentNullException.ThrowIfNull(t);
        int suma = 0;

        SlimLock.EnterReadLock();
        TestTypeIsCountable(t);
        try
        {
            foreach (KeyValuePair<Type, TypeCountInfo> pair in DictCounts)
            {
                if (pair.Key.IsSubclassOf(t))
                {
                    suma += pair.Value.Count;
                }
            }
        }
        finally
        {
            SlimLock.ExitReadLock();
        }
        return suma;
    }

    /// <summary> Get count of all instances of given type and instances of derived types. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when the argument <paramref name="t"/> is
    ///   null. </exception>
    ///
    /// <param name="t">  The type for which the information is required. </param>
    ///
    /// <returns>
    /// Count of instances of given type <paramref name="t"/>, including descendants. </returns>
    ///
    /// <seealso cref="CountExact"/>
    /// <seealso cref="CountDescendants"/>
    public int CountIncludingDescendants(Type t)
    {
        ArgumentNullException.ThrowIfNull(t);
        int result = 0;

        SlimLock.EnterReadLock();
        /* following is not needed here, as it is called both by CountExact and CountDescendants
         * TestTypeIsCountable(t);
         */
        try
        {
            result += CountExact(t);
            if (!t.IsSealed)
            {
                result += CountDescendants(t);
            }
        }
        finally
        {
            SlimLock.ExitReadLock();
        }

        return result;
    }

    /// <summary>
    /// Upon adding a new instance of type t, increments corresponding internal object counter and corresponding
    /// creation index. If it is the first time the object of type t is created, new corresponding <see cref="TypeCountInfo"/>
    /// structure is added to the dictionary.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when the argument <paramref name="t"/> is null. </exception>
    /// <exception cref="InvalidOperationException"> Thrown if the given type <paramref name="t"/>
    /// does not support the interface <see cref="PK.PkUtils.Interfaces.ICountable"/> </exception>
    /// <param name="t"> The type for which the information is modified. </param>
    /// <param name="orderIndex"> [out] Resulting zero-based index (creation index) of a new instance. </param>
    /// <returns>   Resulting count of instances of given type. </returns>
    public int IncrementCount(Type t, out int orderIndex)
    {
        ArgumentNullException.ThrowIfNull(t);
        TypeCountInfo info;

        SlimLock.EnterWriteLock();
        try
        {
            if (DictCounts.TryGetValue(t, out info))
            {
                ++info._count;
                ++info._OrderIndex;
                DictCounts[t] = info;
            }
            else
            {
                TestTypeIsCountable(t);
                info = new TypeCountInfo(1, 0);
                DictCounts.Add(t, info);
            }
        }
        finally
        {
            SlimLock.ExitWriteLock();
        }

        orderIndex = info.OrderIndex;
        return info.Count;
    }

    /// <summary>
    /// Decrements the usage count of given type t. Unlike similar method
    /// <see cref="SafeDecrementCount"/>, this method throws InvalidOperationException if there is no
    /// such type with existing positive usage counter, or the usage counter has reached zero already. </summary>
    ///
    /// <param name="t">  The type for which the information is modified. </param>
    ///
    /// <returns> The new usage counter. </returns>
    ///
    /// <seealso cref="SafeDecrementCount"/>
    public int DecrementCount(Type t)
    { // locking is done in DoDecrementCount, no need to do that here
        return DoDecrementCount(t, false);
    }

    /// <summary>
    /// Decrements the usage count of given type t. Unlike similar method <see cref="DecrementCount"/>,
    /// it does NOT throw InvalidOperationException if there is no such type with existing positive
    /// usage counter, or the usage counter has reached zero already. </summary>
    ///
    /// <param name="t">  The type for which the information is modified. </param>
    ///
    /// <returns> The new usage counter. </returns>
    ///
    /// <seealso cref="DecrementCount"/>
    public int SafeDecrementCount(Type t)
    { // locking is done in DoDecrementCount, no need to do that here
        return DoDecrementCount(t, true);
    }
    #endregion // Public Methods

    #region Protected Methods

    /// <summary>
    /// The implementation helper, called by <see cref="DecrementCount"/> and
    /// <see cref="SafeDecrementCount"/>. <br/>
    /// If an error occurs ( if there is no such type with existing positive usage counter in the usage map)
    /// and if the argument <paramref name="bSafe "/>is false, throws <see cref="InvalidOperationException"/>. </summary>
    ///
    /// <exception cref="ArgumentNullException">     Thrown when the argument <paramref name="t"/> is
    ///   null. </exception>
    /// <exception cref="InvalidOperationException"> Thrown if the given type <paramref name="t"/>
    ///   does not support the interface <see cref="PK.PkUtils.Interfaces.ICountable"/> </exception>
    ///
    /// <param name="t">      The type for which the information is required. </param>
    /// <param name="bSafe">  If the value is false and there is no such type with existing positive usage counter, throws <see cref="InvalidOperationException"/></param>
    ///
    /// <returns> The resulting usage counter for the type. </returns>
    protected int DoDecrementCount(Type t, bool bSafe)
    {
        ArgumentNullException.ThrowIfNull(t);

        bool bErr = false;
        int result = 0;

        SlimLock.EnterWriteLock();
        try
        {
            if (DictCounts.TryGetValue(t, out TypeCountInfo info))
            {
                if (0 < info.Count)
                {
                    // commented-out following code for case when the dictionary entries 
                    // should be removed upon the count reaching zero
                    /*
                    if (--result > 0)
                    {
                      info._count = result;
                      _countDict[t] = info;
                    }
                    else
                    {
                      _countDict.Remove(t);
                    }
                    */
                    result = --info._count;
                    DictCounts[t] = info;
                    Debug.Assert(DictCounts[t].Count == result);
                }
                else
                {
                    bErr = !bSafe;
                }
            }
            else
            {
                TestTypeIsCountable(t);
                bErr = true;
            }
        }
        finally
        {
            SlimLock.ExitWriteLock();
        }

        if (bErr)
        {
            string strErr = string.Format(CultureInfo.InvariantCulture,
              "The type '{0}' has reached negative usage counter", t.ToString());
            throw new InvalidOperationException(strErr);
        }

        return result;
    }

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the
    /// method has been called directly or indirectly by a user's code. Managed and unmanaged resources
    /// can be disposed. If disposing equals false, the method has been called by the runtime from
    /// inside the finalizer and you should not reference other objects. Only unmanaged resources can
    /// be disposed. </summary>
    ///
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by
    ///   finalizer. </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Disposer.SafeDispose(ref _slimLock);
        }
    }
    #endregion // Protected Methods

    #region Private Methods

    /// <summary>
    /// Tests whether the argument is countable type. If it is in the DictCounts as a key, it passed
    /// this test already;
    /// otherwise the code checks whether the type implements ICountable. Throws
    /// InvalidOperationException if the type is not countable. </summary>
    ///
    /// <exception cref="ArgumentNullException">     Thrown if the given type <paramref name="t"/> is
    ///   null. </exception>
    /// <exception cref="InvalidOperationException"> Thrown if the given type <paramref name="t"/>
    ///   does not support the interface <see cref="PK.PkUtils.Interfaces.ICountable"/> </exception>
    ///
    /// <param name="t">  The type for which the information is required. </param>
    [Conditional("DEBUG")]
    private void TestTypeIsCountable(Type t)
    {
        ArgumentNullException.ThrowIfNull(t);

        if (SlimLock.IsReadLockHeld || SlimLock.IsWriteLockHeld)
        {
            if (DictCounts.ContainsKey(t))
            {
                // ok, got into the dictionary already
                // check the exceptions occurring if uncommented
                // System.Windows.Forms.MessageBox.Show("hi");
            }
            else
            {
                Type tic = typeof(Interfaces.ICountable);
                Type[] arrInterfaces = t.GetInterfaces();

                if (!arrInterfaces.Contains(tic))
                {
                    string strErr = string.Format(CultureInfo.InvariantCulture,
                      "Invalid type used as an argument. The type '{0}' does not implement the interface '{1}'",
                      t.TypeToReadable(), tic.TypeToReadable());
                    throw new InvalidOperationException(strErr);
                }
            }
        }
        else
        {
            throw new InvalidOperationException("This method assumes the lock has been acquired already");
        }
    }
    #endregion // Private Methods
    #endregion // Methods

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
    /// resources. </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposable Members
};

/// <summary>
/// An auxiliary class providing extension methods for counting instances of given type. Delegates its
/// functionality to the <see cref="CountMap"/> singleton instance.
/// </summary>
///
/// <remarks>
/// Unfortunately, there has to be a separate class for this. Extension methods cannot be declared in a non-
/// static class, or a generic class.
/// </remarks>
public static class CountMapExt
{
    #region Methods

    #region Public Extension Methods

    /// <summary>
    /// The extension object returning the current count of type t objects in 'living state'. Counts
    /// only objects (instances) of exactly the same type. </summary>
    ///
    /// <param name="t">  The type for which the information is required. </param>
    ///
    /// <returns> Count of instances of <paramref name="t"/>. </returns>
    public static int GetCountExact(this Type t)
    {
        return CountMap.Instance.CountExact(t);
    }

    /// <summary>
    /// The extension object returning the current count of type t descendants in 'living state'.
    /// Counts only objects (instances) descendants, NOT objects of exactly the same type. </summary>
    ///
    /// <param name="t">  The type for which the information is required. </param>
    ///
    /// <returns> Count of instances of descendants of given type <paramref name="t"/>. </returns>
    public static int GetCountDescendants(this Type t)
    {
        return CountMap.Instance.CountDescendants(t);
    }

    /// <summary>
    /// The extension object returning the count of all instances of given type and instances of
    /// derived types. </summary>
    ///
    /// <param name="t">  The type for which the information is required. </param>
    ///
    /// <returns>
    /// Count of instances of given type <paramref name="t"/>, including descendants. </returns>
    public static int GetCountIncludingDescendants(this Type t)
    {
        return CountMap.Instance.CountIncludingDescendants(t);
    }
    #endregion // Public Extension Methods
    #endregion // Methods
}

/***************************************************************************************************************
*
* FILE NAME:   .\Utils\UsageCounterWrapper.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains code of classes UsageCounter and UsageCounterWrapper
*
**************************************************************************************************************/


// Ignore Spelling: Utils

using System;
using System.Diagnostics;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.Utils;

/// <summary> A helper class, providing easier usage of IUsageCounter>-implementing class. </summary>
///
/// <example> Usage example: <br/>
/// Assume you have declared as a member variable in your class CAnyClass UsageCounter _counter;
/// and in the code of the method of the class you will use locally the instance of UsageCounterWrapper:
/// <code>
/// void CAnyClass.SomeMythod()
/// {
///   if (_counter.IsUsed)
///   {
///     return;
///   }
///   else using(var wrapper = new UsageCounterWrapper(_counter))
///   {
///     // Do something.
///     // The 'using' statement makes sure that dispose of wrapper is called;
///     // which in turn will release the _counter instance, 
///     // regardless any exception thrown.
///   }
/// }
/// </code></example>
[CLSCompliant(true)]
public class UsageCounterWrapper : IDisposableEx
{
    #region Fields
    /// <summary> The wrapped usage counter. </summary>
    protected IUsageCounter _counter;
    private bool _disposed;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// The constructor which increments the given counter, if it is not acquired already by someone else.
    /// If the counter is used already, the usage count does NOT increment.
    /// </summary>
    /// 
    /// <exception cref="System.ArgumentNullException">Thrown when the input argument 
    /// <paramref name="counter"/> is null.</exception>
    /// 
    /// <param name="counter">The object implementing <see cref="IUsageCounter"/> that is going to be locked.</param>
    public UsageCounterWrapper(IUsageCounter counter)
        : this(counter, firstTimeUseOnly: true)
    { }

    /// <summary>
    /// The constructor which increments the given counter, if it is not acquired already by someone else. If the
    /// counter is used already, the usage count does NOT increment.
    /// </summary>
    ///
    /// <exception cref="System.ArgumentNullException"> Thrown when the input argument
    /// <paramref name="counter"/> is null. </exception>
    ///
    /// <param name="counter"> The object implementing <see cref="IUsageCounter"/> that is going to be locked. </param>
    /// <param name="firstTimeUseOnly"> True to add reference for first time use only. </param>
    public UsageCounterWrapper(IUsageCounter counter, bool firstTimeUseOnly)
    {
        ArgumentNullException.ThrowIfNull(counter);
        AcquireUse(counter, firstTimeUseOnly);
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Returns true if counter is acquired; false otherwise. </summary>
    public bool IsCounterAcquired
    {
        get { return (_counter != null); }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the method
    /// has been called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed.
    /// If disposing equals false, the method has been called by the runtime from inside the finalizer
    /// and you should not reference other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.IsDisposed)
        {
            if (disposing)
            {
                // Dispose managed resources here ( if there are any )
                ReleaseCounter();
            }
        }
    }

    /// <summary> Implementation helper called by constructor. Locks given lock counter, if not locked already. </summary>
    /// <param name="aCounter"> The utilized usage counter. </param>
    /// <returns> True if it succeeds, false if it fails. </returns>
    protected bool AcquireFirstTimeUse(IUsageCounter aCounter)
    {
        return AcquireUse(aCounter, firstTimeOnly: true);
    }

    /// <summary> Implementation helper called by constructor. Locks given lock counter, if not locked already. </summary>
    /// <param name="aCounter"> The utilized usage counter. </param>
    /// <param name="firstTimeOnly">    True to first time only. </param>
    /// <returns>   True if it succeeds, false if it fails. </returns>
    protected bool AcquireUse(IUsageCounter aCounter, bool firstTimeOnly)
    {
        Debug.Assert(null == _counter);
        Debug.Assert(null != aCounter);

        if (firstTimeOnly)
        {
            if (!aCounter.IsUsed)  // looks good, try to proceed to atomic locking
            {
                if ((1 == aCounter.AddReference()))
                {  // indeed made it
                    _counter = aCounter;
                }
                else
                {  // someone was faster, ad wanted firstTimeOnly, so revert back
                    aCounter.Release();
                }
            }
        }
        else
        {
            aCounter.AddReference();
            _counter = aCounter;
        }

        return IsCounterAcquired;
    }

    /// <summary>
    /// Implementation helper called by disposing code.
    /// Reverts the lock that has been done by DoLock (if any)
    /// </summary>
    private void ReleaseFirstTimeUse()
    {
        if (!IsDisposed)
        {
            ReleaseCounter();
        }
    }

    /// <summary>
    /// Internal helper that releases the counter and marks disposed.
    /// </summary>
    private void ReleaseCounter()
    {
        if (IsCounterAcquired)
        {
            if (_counter.IsUsed)
            {
                _counter.Release();
            }
            _counter = null;
        }
        _disposed = true;
    }
    #endregion // Methods

    #region IDisposableEx Members
    #region IDisposable Members

    /// <summary>
    /// Implement IDisposable. Do not make this method virtual. 
    /// A derived class should not be able to override this method.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposable Members

    /// <summary>
    /// Returns true in case this UsageCounterWrapper has been disposed and no longer should be used.
    /// </summary>
    public bool IsDisposed
    {
        get { return _disposed; }
    }
    #endregion // IDisposableEx Members
}

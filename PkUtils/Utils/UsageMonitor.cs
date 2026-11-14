// Ignore Spelling: Utils

using System;
using System.Threading;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.Utils;

/// <summary> A helper class, providing easier usage of IUsageCounter-implementing class. </summary>
///
/// <example> Usage example: <br/>
/// Assume you have declared as a member variable in your class CAnyClass UsageCounter _counter;
/// and in the code of the method of the class you will use locally the instance of UsageMonitor:
/// <code>
/// void CAnyClass.SomeMethod()
/// {
///   if (_counter.IsUsed)
///   {
///     return;
///   }
///   else using(var monitor = new UsageMonitor(_counter))
///   {
///     // The 'íf' condition below should be checked just in potentially multi-threading scenarios;
///     // not when the purpose of '_counter' is some kind of re-entrance check (locking, etc.) in a single-threaded UI
///     //
///     if (monitor.IsCounterAcquired)
///     {
///          // Do something.
///          // The 'using' statement makes sure that dispose of wrapper is called;
///          // which in turn will release the _counter instance, 
///          // regardless any exception thrown.
///     }
///   }
/// }
/// </code></example>
public class UsageMonitor : IDisposableEx
{
    #region Fields
    /// <summary> The wrapped usage counter. </summary>
    protected IUsageCounter _counter;
    private volatile bool _disposed;
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
    public UsageMonitor(IUsageCounter counter)
        : this(counter, firstTimeUseOnly: true)
    { }

    /// <summary>
    /// UM-05: With firstTimeUseOnly true, acquires only if this is the first user.
    /// With firstTimeUseOnly false, always adds one reference to the counter.
    /// </summary>
    ///
    /// <exception cref="System.ArgumentNullException"> Thrown when the input argument
    /// <paramref name="counter"/> is null. </exception>
    ///
    /// <param name="counter"> The object implementing <see cref="IUsageCounter"/> that is going to be locked. </param>
    /// <param name="firstTimeUseOnly"> If true, acquire only when the count was zero;
    /// otherwise add a reference even when already in use. </param>
    public UsageMonitor(IUsageCounter counter, bool firstTimeUseOnly)
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
        // UM-03: Enforce ownership and argument preconditions in Release as well as Debug.
        ArgumentNullException.ThrowIfNull(aCounter);
        ObjectDisposedException.ThrowIf(IsDisposed, GetType().FullName);
        if (_counter is not null)
            throw new InvalidOperationException("This monitor has already acquired a counter.");

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
        // UM-01: Exactly one concurrent disposer takes ownership of the counter.
        IUsageCounter counter = Interlocked.Exchange(ref _counter, null);
        try
        {
            // UM-02: The monitor releases its own acquired reference, regardless
            // of the counter's global IsUsed state (which is not an ownership test).
            counter?.Release();
        }
        finally
        {
            // UM-04: A throwing Release/callback cannot make us release twice.
            _disposed = true;
        }
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
    /// Returns true in case this UsageMonitor has been disposed and no longer should be used.
    /// </summary>
    public bool IsDisposed
    {
        get { return _disposed; }
    }
    #endregion // IDisposableEx Members
}

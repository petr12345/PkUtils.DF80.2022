using System;
using System.Threading;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.DataStructures;

#pragma warning disable IDE0290     // Use primary constructor

/// <summary>
/// A "disposable action" implements a wrapper that executes an action in its dispose method.
/// This way, you could alternatively guarantee  that action is performed in some finally statement.  
/// Applying the 'using' tends to contain fewer lines of code than try - finally.
/// </summary>
public class DisposableAction : IDisposableEx
{
    #region Fields

    private readonly Action _action;
    private int _disposed;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> The only constructor. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="action"/> is null. </exception>
    /// <param name="action"> The action. </param>
    public DisposableAction(Action action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }
    #endregion // Constructor(s)

    #region Methods

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the method has been
    /// called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed. If
    /// disposing equals false, the method has been called by the runtime from inside the finalizer and you
    /// should not reference other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <remarks> The action is attempted at most once. If it throws, disposal is not retried. </remarks>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
    protected virtual void Dispose(bool disposing)
    {
        // DA-01 / DA-02: Claim disposal atomically before invoking user code.
        // The action is attempted at most once, including when it throws.
        if (disposing && Interlocked.Exchange(ref _disposed, 1) == 0)
        {
            _action();
        }
    }
    #endregion // Methods

    #region IDisposableEx members

    /// <inheritdoc/>
    public bool IsDisposed { get => Volatile.Read(ref _disposed) != 0; }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposableEx members
}
#pragma warning restore IDE0290     // Use primary constructor
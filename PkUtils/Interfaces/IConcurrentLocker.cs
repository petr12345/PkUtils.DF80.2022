// Ignore Spelling: Utils
// 
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PK.PkUtils.Interfaces;

/// <summary> Interface for concurrent locker. </summary>
/// <typeparam name="TKey"> Type of the key. </typeparam>
public interface IConcurrentLocker<in TKey> : IDisposableEx
{
    /// <summary> Gets the current amount of locks. </summary>
    int CurrentSize { get; }

    /// <summary> Acquire a lock that is specific for <paramref name="key"/>. </summary>
    /// <param name="key"> The key to get lock for. Can't be null. </param>
    /// <returns> The acquired lock. </returns>
    Task<IDisposable> LockAsync(TKey key);

    /// <summary> Acquire a lock that is specific for <paramref name="key"/>. </summary>
    ///
    /// <param name="key">  The key to get lock for. Can't be null. </param>
    /// <param name="millisecondsTimeout"> The number of milliseconds to wait, Infinite (-1) to wait indefinitely,
    /// or zero to test the state of the wait handle and return immediately. </param>
    ///
    /// <returns> The acquired lock. </returns>
    Task<IDisposable> LockAsync(TKey key, int millisecondsTimeout);

    /// <summary> Acquire a lock that is specific for <paramref name="key"/>. </summary>
    ///
    /// <param name="key">  The key to get lock for. Can't be null. </param>
    /// <param name="millisecondsTimeout"> The number of milliseconds to wait, Infinite (-1) to wait indefinitely,
    /// or zero to test the state of the wait handle and return immediately. </param>
    /// <param name="cancellationToken">    A token that allows processing to be canceled. </param>
    ///
    /// <returns> The acquired lock. </returns>
    Task<IDisposable> LockAsync(TKey key, int millisecondsTimeout, CancellationToken cancellationToken);
}

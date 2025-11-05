using System;
using System.Runtime.CompilerServices;
using PK.PkUtils.Interfaces;


namespace PK.PkUtils.Extensions;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable CA1513    // Use ObjectDisposedException throw helper

/// <summary>
/// Wrapper class around IDisposableEx extension methods.
/// </summary>
public static class DisposableExtensions
{
    /// <summary>
    /// Checks whether the specified <paramref name="obj"/> is not <c>null</c> and has not been disposed.
    /// <para>
    /// Throws an <see cref="ArgumentNullException"/> if <paramref name="obj"/> is <c>null</c>.
    /// Throws an <see cref="ObjectDisposedException"/> if <paramref name="obj"/> is not <c>null</c> but its <see cref="IDisposableEx.IsDisposed"/> property returns <c>true</c>.
    /// </para>
    /// </summary>
    /// <param name="obj">The object to check for <c>null</c> and disposal state.</param>
    /// <param name="objectName">
    /// The name of the object (for example, the name of the formal argument in the calling code).
    /// This name will appear in the exception message if an exception is thrown.
    /// </param>
    /// <returns>The original <paramref name="obj"/> if it is not <c>null</c> and not disposed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is <c>null</c>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when <paramref name="obj"/> has been disposed.</exception>
    /// <returns> The original value of <paramref name="obj"/>. </returns>
    public static IDisposableEx CheckNotDisposed(
        this IDisposableEx obj,
        [CallerArgumentExpression(nameof(obj))] string objectName = null)
    {
        ArgumentNullException.ThrowIfNull(obj, objectName);
        if (obj.IsDisposed)
        {
            throw new ObjectDisposedException(string.IsNullOrEmpty(objectName) ?
              obj.GetType().FullName : objectName);
        }

        return obj;
    }
}

#pragma warning restore CA1513
#pragma warning restore IDE0079
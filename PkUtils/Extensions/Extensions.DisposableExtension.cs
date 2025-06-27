using System;
using System.Runtime.CompilerServices;
using PK.PkUtils.Interfaces;


namespace PK.PkUtils.Extensions;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable CA1513    // Use ObjectDisposedException throw helper

/// <summary>
/// Wrapper class around IDisposableEx extension methods.
/// </summary>
public static class DisposableExtension
{
    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> in case the object is null.
    /// Throws <see cref="ObjectDisposedException"/> in case the object is not null, 
    /// but its IDisposableEx.IsDisposed property returns true.
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when a supplied object is null. </exception>
    /// <exception cref="ObjectDisposedException"> Thrown when a supplied object has been disposed. </exception>
    /// 
    /// <param name="obj"> The object being checked. </param>
    /// <param name="objectName"> The name of the object ( for instance the name 
    /// of formal argument in the calling code).
    /// This name will appear in resulting exception text. </param>
    public static void CheckNotDisposed(
        this IDisposableEx obj,
        [CallerArgumentExpression(nameof(obj))] string objectName = null)
    {
        ArgumentNullException.ThrowIfNull(obj, objectName);
        if (obj.IsDisposed)
        {
            throw new ObjectDisposedException(string.IsNullOrEmpty(objectName) ?
              obj.GetType().FullName : objectName);
        }
    }
}

#pragma warning restore CA1513
#pragma warning restore IDE0079
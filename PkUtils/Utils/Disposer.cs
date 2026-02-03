// Ignore Spelling: Utils
//

using System;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable IDE0031 // Null check can be simplified

namespace PK.PkUtils.Utils;

/// <summary>
/// A generic dispose utility, allowing to type just 
/// <code>
/// Disposer.SafeDispose(ref x);
/// </code>
/// instead of <code>
/// <![CDATA[
/// if (x  != null)
/// {
///  x.Dispose();
///  x = null;
/// }
/// ]]>
/// </code>
/// </summary>
public static class Disposer
{
    /// <summary>
    /// Dispose any variable containing disposable object, and assigns that variable to null.
    /// </summary>
    /// <typeparam name="T">The type of variable being disposed.</typeparam>
    /// <param name="t">The variable being disposed.</param>
    public static void SafeDispose<T>(ref T t) where T : class, IDisposable
    {
        if (null != t)
        {
            t.Dispose();
            t = null;
        }
    }

    /// <summary>
    /// Dispose any variable containing nullable disposable structure, and assigns that variable to null.
    /// </summary>
    /// <typeparam name="T">The type of structure being disposed.</typeparam>
    /// <param name="t">The structure being disposed.</param>
    public static void SafeDispose<T>(ref Nullable<T> t) where T : struct, IDisposable
    {
        if (null != t)
        {
            t.Value.Dispose();
            t = null;
        }
    }

    /// <summary>
    /// Dispose any variable containing Lazy-created disposable object, and assigns that variable to null.
    /// </summary>
    ///
    /// <typeparam name="T">The type of lazy variable being disposed.</typeparam>
    /// <param name="t">  [in,out] The variable being disposed. </param>
    /// 
    /// <remarks>
    /// Note that the whole field Lazy is set to null, not just its value created. The reason behind is,
    /// System.Lazy can't be reset once it has been created. For more info see
    /// <see href="http://stackoverflow.com/questions/5961252/reset-system-lazy/">
    /// StackOverflow - Reset System.Lazy</see>.<br/>
    /// </remarks>
    public static void SafeDispose<T>(ref Lazy<T> t) where T : IDisposable
    {
        if (null != t)
        {
            if (t.IsValueCreated)
            {
                t.Value.Dispose();
            }
            t = null;
        }
    }
}

#pragma warning restore IDE0031 // Null check can be simplified
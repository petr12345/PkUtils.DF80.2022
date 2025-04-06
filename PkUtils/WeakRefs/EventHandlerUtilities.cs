// Ignore Spelling: Utils
//
using System;
using System.Diagnostics;

namespace PK.PkUtils.WeakRefs;

/// <summary> An event handler utilities. </summary>
public static class EventHandlerUtilities
{
    /// <summary>
    /// Provides a way to 'cleanup' invocation list of given event handler, by removing specific (or all) contained delegates.
    /// The condition which delegate(s) should be removed is specified by the <paramref name="predicate"/> argument.<br/>
    /// If the predicate is null, all delegates are removed.<br/>
    /// Note:<br/>
    /// You may utilize this method to cleanup application resources,
    /// when some objects are reachable and kept in memory only by event handler(s)
    /// ( if the delegates in subscribers list have as target these objects, 
    /// and this is the only reference to such observers ).<br/>
    /// Note:<br/>
    /// This is a non-generic version of the method (while the similar overload is generic).
    /// </summary>
    /// <param name="handler">The event handler that is subject of cleanup.</param>
    /// <param name="predicate">
    /// A predicate working as a criteria which delegates should be removed.
    /// If null, all delegates are removed.</param>
    /// <returns> true if any change has been done, false otherwise</returns>
    public static bool InvocationListClear(
        ref EventHandler handler,
        Func<Delegate, bool> predicate)
    {
        bool bRes = false;

        if (null != handler)
        {
            MulticastDelegate delegTemp = handler;
            Delegate[] invokes = delegTemp.GetInvocationList();

            // loop backward, so the indexes adjustment does not slow-down or mess-up
            for (int nDex = invokes.Length - 1; nDex >= 0; --nDex)
            {
                Delegate deleg = invokes[nDex];
                if ((null == predicate) || predicate(deleg))
                {
                    handler -= (EventHandler)deleg;
                    bRes = true;
                }
            }
        }
        return bRes;
    }

    /// <summary>
    /// Provides a way to cleanup of invocation list of given event handler.
    /// Has quite the same functionality as similar non-generic overload.
    /// </summary>
    /// <param name="handler">The event handler that is subject of cleanup.</param>
    /// <param name="predicate">
    /// A predicate working as a criteria which delegates should be removed.
    /// If null, all delegates are removed.</param>
    /// <returns> true if any change has been done, false otherwise</returns>
    public static bool InvocationListClear<TEventArgs>(
        ref EventHandler<TEventArgs> handler,
        Func<Delegate, bool> predicate) where TEventArgs : EventArgs
    {
        bool bRes = false;

        if (null != handler)
        {
            MulticastDelegate delegTemp = handler;
            Delegate[] invokes = delegTemp.GetInvocationList();
            // loop backward, so the indexes adjustment does not slow-down or mess-up
            for (int nDex = invokes.Length - 1; nDex >= 0; --nDex)
            {
                Delegate deleg = invokes[nDex];
                if ((null == predicate) || predicate(deleg))
                {
                    handler -= (EventHandler<TEventArgs>)deleg;
                    bRes = true;
                }
            }
        }
        return bRes;
    }

    /// <summary>
    /// Complete cleanup of invocation list.<br/>
    /// Note:<br/>
    /// This is a non-generic version of the method (while the similar overload is generic).
    /// </summary>
    /// <param name="handler">The event handler that is subject of cleanup.</param>
    /// <returns> true if any change has been done, false otherwise</returns>
    public static bool InvocationListDestroy(ref EventHandler handler)
    {
        bool bRes = false;

        if (null != handler)
        {
            bRes = InvocationListClear(ref handler, null);
            Debug.Assert(null == handler as MulticastDelegate);
        }
        return bRes;
    }

    /// <summary>
    /// Complete cleanup of invocation list.<br/>
    /// Note:<br/>
    /// This is a generic version of the method (while the similar overload is non-generic).
    /// </summary>
    /// <param name="handler">The event handler that is subject of cleanup.</param>
    /// <returns> true if any change has been done, false otherwise</returns>
    public static bool InvocationListDestroy<TEventArgs>(
        ref EventHandler<TEventArgs> handler) where TEventArgs : EventArgs
    {
        bool bRes = false;

        if (null != handler)
        {
            bRes = InvocationListClear(ref handler, null);
            Debug.Assert(null == handler as MulticastDelegate);
        }
        return bRes;
    }
}

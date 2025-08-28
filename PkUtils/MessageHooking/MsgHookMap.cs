///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// MSJ Copyright notice
// 
// This software is a Derivative Work based upon a MSJ article
// "More Fun With MFC: DIBs, Palettes, Subclassing and a Gamut of Goodies, Part II"
// from the March 1997 issue of Microsoft Systems Journal
// https://web.archive.org/web/20040614000754/http://www.microsoft.com/msj/0397/mfcp2/mfcp2.aspx
// by Paul DiLascia
// https://en.wikipedia.org/wiki/Paul_DiLascia
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using PK.PkUtils.DataStructures;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.MessageHooking;

/// <summary>
/// MsgHookMap is an auxiliary class used for WindowMessageHook implementation. 
/// Maps HWND to ChainedNativeWindow. The class is singleton.
/// </summary>
internal sealed class MsgHookMap : Singleton<MsgHookMap>
{
    #region Fields
    private readonly Dictionary<IntPtr, ChainedNativeWindow> _mapHwndToHook;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Private argument-less constructor.
    /// </summary>
    /// <remarks>
    /// Argument-less constructor must be private or protected 
    /// because of Singleton generic requirements.
    /// </remarks>
    private MsgHookMap()
    {
        _mapHwndToHook = [];
    }
    #endregion // Constructor(s)

    #region Methods

    /// <summary> Internal implementation helper; does the 'real' hookup of window. </summary>
    ///
    /// <param name="hwnd"> The handle of the window that is subject of adding. </param>
    /// <param name="msgHook"> The hook to be added. </param>
    internal void Add(IntPtr hwnd, WindowMessageHook msgHook)
    {
        Debug.Assert(User32.IsWindow(hwnd));
        Debug.Assert(msgHook != null);
        // find out if the window hooked yet or not
        ChainedNativeWindow nativeWindow = LookupNativeHook(hwnd);

        if (nativeWindow == null)
        {
            // Not hooked yet. Since this is the first hook added, 
            // must create native hook, and subclass the window
            // 
            nativeWindow = new ChainedNativeWindow();
            _mapHwndToHook[hwnd] = nativeWindow;
            msgHook.SubclassWindow(hwnd, nativeWindow);
        }
        else
        {   // window is hooked already, just add another hook to the chain
            msgHook.ChainHook(hwnd, nativeWindow);
        }
    }

    /// <summary>
    /// Internal implementation helper; removes 'real' hookup <paramref name="msgHook"/> from set of hooks for
    /// its hooked window.
    /// </summary>
    /// <param name="msgHook"> The hook to be removed. </param>
    internal void Remove(WindowMessageHook msgHook)
    {
        Debug.Assert(msgHook != null);
        IntPtr hwnd = msgHook.HookedHWND;
        ChainedNativeWindow nativeWindow;
        /* Don't! Actually, the original window *might* be destroyed already
         * Debug.Assert(User32.IsWindow(hwnd));
         */
        if (null != (nativeWindow = LookupNativeHook(hwnd)))
        {
            Debug.Assert(nativeWindow.FirstMsgHook != null);
            if (nativeWindow.FirstMsgHook == msgHook)
            {   // hook is the first one: replace w/next
                if (msgHook.NextHook != null)
                {   // But was not quite the last one hook
                    nativeWindow.FirstMsgHook = msgHook.NextHook;
                    msgHook.UnChainHook();
                }
                else
                {   // This was the last hook for this window: 
                    // restore the window-proc and remove list from map
                    nativeWindow.FirstMsgHook = null;
                    msgHook.UnSubclassWindow();
                    _mapHwndToHook.Remove(hwnd);
                }
            }
            else
            {   // Hook to remove is in the middle: just remove from linked list
                WindowMessageHook tempHook = nativeWindow.FirstMsgHook;

                while ((tempHook != null) && (tempHook.NextHook != msgHook))
                {
                    tempHook = tempHook.NextHook;
                }
                Debug.Assert(tempHook != null);
                Debug.Assert(tempHook.NextHook == msgHook);
                tempHook.SetNextHook(msgHook.NextHook);
                msgHook.UnChainHook();
            }
        }
    }

    /// <summary> Remove all the hooks for a window. </summary>
    /// <param name="hwnd"> The handle of the window that is subject of removal. </param>
    internal void RemoveAll(IntPtr hwnd)
    {
        WindowMessageHook msgHook;

        while ((msgHook = Lookup(hwnd)) != null)
        {
            msgHook.HookWindow(IntPtr.Zero);   // (unhook)
        }
    }

    /// <summary> Find native hook associated with window handle. </summary>
    /// <param name="hwnd"> The handle of the window. </param>
    /// <returns> The found ChainedNativeWindowp; or null if nothing found. </returns>
    internal ChainedNativeWindow LookupNativeHook(IntPtr hwnd)
    {
        ChainedNativeWindow result = null;

        if ((hwnd != IntPtr.Zero) && _mapHwndToHook.TryGetValue(hwnd, out result))
        {
            Debug.Assert(result != null);
        }
        return result;
    }

    /// <summary> Find first hook associated with window. </summary>
    /// <param name="hwnd"> The handle of window. </param>
    /// <returns> A WindowMessageHook, mapped to given <paramref name="hwnd"/>. Will be null if none found. </returns>
    internal WindowMessageHook Lookup(IntPtr hwnd)
    {
        ChainedNativeWindow nativeWindow;
        WindowMessageHook msgHook = null;

        if (null != (nativeWindow = LookupNativeHook(hwnd)))
        {
            msgHook = nativeWindow.FirstMsgHook;
            Debug.Assert(msgHook != null);
        }

        return msgHook;
    }
    #endregion // Methods
}

/***************************************************************************************************************
*
* FILE NAME:   .\MessageHooking\MsgHookMap.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: Contains the class MsgHookMap.
*
**************************************************************************************************************/
///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// MSJ Copyright notice
// 
// This software is a Derivative Work based upon a MSJ article
// "More Fun With MFC: DIBs, Palettes, Subclassing and a Gamut of Goodies, Part II"
// from the March 1997 issue of Microsoft Systems Journal, by Paul DiLascia
// http://www.microsoft.com/msj/0397/mfcp2/mfcp2.aspx
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
    /// <param name="pMsgHook"> The hook to be added. </param>
    internal void Add(IntPtr hwnd, WindowMessageHook pMsgHook)
    {
        Debug.Assert(User32.IsWindow(hwnd));
        Debug.Assert(pMsgHook != null);
        // find out if the window hooked yet or not
        ChainedNativeWindow pNative = LookupNativeHook(hwnd);

        if (pNative == null)
        {
            // Not hooked yet. Since this is the first hook added, 
            // must create native hook, and subclass the window
            // 
            pNative = new ChainedNativeWindow();
            _mapHwndToHook[hwnd] = pNative;
            pMsgHook.SubclassWindow(hwnd, pNative);
        }
        else
        {   // window is hooked already, just add another hook to the chain
            pMsgHook.ChainHook(hwnd, pNative);
        }
    }

    /// <summary>
    /// Internal implementation helper; removes 'real' hookup <paramref name="pMsgHook"/> from set of hooks for
    /// its hooked window.
    /// </summary>
    /// <param name="pMsgHook"> The hook to be removed. </param>
    internal void Remove(WindowMessageHook pMsgHook)
    {
        Debug.Assert(pMsgHook != null);
        IntPtr hwnd = pMsgHook.HookedHWND;
        ChainedNativeWindow pNative;
        /* Don't! Actually, the original window *might* be destroyed already
         * Debug.Assert(User32.IsWindow(hwnd));
         */
        if (null != (pNative = LookupNativeHook(hwnd)))
        {
            Debug.Assert(pNative.FirstMsgHook != null);
            if (pNative.FirstMsgHook == pMsgHook)
            {   // hook is the first one: replace w/next
                if (pMsgHook.NextHook != null)
                {   // But was not quite the last one hook
                    pNative.FirstMsgHook = pMsgHook.NextHook;
                    pMsgHook.UnChainHook();
                }
                else
                {   // This was the last hook for this window: 
                    // restore the window-proc and remove list from map
                    pNative.FirstMsgHook = null;
                    pMsgHook.UnSubclassWindow();
                    _mapHwndToHook.Remove(hwnd);
                }
            }
            else
            {   // Hook to remove is in the middle: just remove from linked list
                WindowMessageHook tempHook = pNative.FirstMsgHook;

                while ((tempHook != null) && (tempHook.NextHook != pMsgHook))
                {
                    tempHook = tempHook.NextHook;
                }
                Debug.Assert(tempHook != null);
                Debug.Assert(tempHook.NextHook == pMsgHook);
                tempHook.SetNextHook(pMsgHook.NextHook);
                pMsgHook.UnChainHook();
            }
        }
    }

    /// <summary> Remove all the hooks for a window. </summary>
    /// <param name="hwnd"> The handle of the window that is subject of removal. </param>
    internal void RemoveAll(IntPtr hwnd)
    {
        WindowMessageHook pMsgHook;

        while ((pMsgHook = Lookup(hwnd)) != null)
        {
            pMsgHook.HookWindow(IntPtr.Zero);   // (unhook)
        }
    }

    /// <summary> Find native hook associated with window handle. </summary>
    /// <param name="hwnd"> The handle of the window. </param>
    /// <returns>  The found ChainedNativeWindowp; or null if nothing found. </returns>
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
    /// <param name="hwnd"> The handle of te window. </param>
    /// <returns>  A WindowMessageHook, mapped to given <paramref name="hwnd"/>. Will be null if none found. </returns>
    internal WindowMessageHook Lookup(IntPtr hwnd)
    {
        ChainedNativeWindow pNative;
        WindowMessageHook pResult = null;

        if (null != (pNative = LookupNativeHook(hwnd)))
        {
            pResult = pNative.FirstMsgHook;
            Debug.Assert(pResult != null);
        }

        return pResult;
    }
    #endregion // Methods
}

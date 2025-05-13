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
using System.Windows.Forms;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.MessageHooking;

/// <summary>
/// Auxiliary class used for <see cref="WindowMessageHook"/> implementation.
/// Inherits the functionality of NativeWindow, thus hooking the Win32 window,
/// but extends that functionality to permit calling multiple hooks in the chain.
/// The variable <see cref="_firstMsgHook"/> is used for that purpose inside <see cref="ChainedNativeWindow.WndProc"/>.
/// </summary>
internal sealed class ChainedNativeWindow : NativeWindow
{
    #region Fields

    /// <summary> The first hook in the chain of hooks. </summary>
    private WindowMessageHook _firstMsgHook;
    /// <summary>
    /// Internal flag set to true when the hook is being destroyed. 
    /// The corresponding property is used in the code of WindowMessageHook 
    /// </summary>
    private bool _doPreserveHandle;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Argument-less constructor.
    /// </summary>
    internal ChainedNativeWindow()
    {
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Gets/sets the first hook in the chain of hooks.
    /// </summary>
    internal WindowMessageHook FirstMsgHook
    {
        get { return _firstMsgHook; }
        set { _firstMsgHook = value; }
    }

    /// <summary>
    /// The value of internal flag which is set to true when the hook is being destroyed. 
    /// The method WindowMessageHook.UnSubclassWindow() needs that info.
    /// </summary>
    /// <remarks> The value is set to true in protected override void WndProc(ref Message m)</remarks>
    internal bool PreserveHandle
    {
        get { return _doPreserveHandle; }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Overrides the virtual method of the base class NativeWindow, to adjust the message processing.
    /// </summary>
    /// <param name="m"></param>
    protected override void WndProc(ref Message m)
    {
        if (FirstMsgHook == null)
        {
            // If first hook is null, the object is not quite hooked yet, or already not hooked
            base.WndProc(ref m);
        }
        else
        {
            if (m.Msg != (int)Win32.WM.WM_NCDESTROY)
            {
                // Now call the first item in the chain,
                // that is responsible for calling the next item, etc.;
                // and the last item in the chain is responsible for calling DefWndProc
                FirstMsgHook.CallHookWindowProc(ref m);
            }
            else
            {
                // Window is being destroyed: unhook all hooks (for this window)
                // and pass the message to original window proc
                MsgHookMap pMap = MsgHookMap.Instance;

                // Set preserveHandle to true, 
                // so ReleaseHandle is not called by UnSubclassWindow(),
                // and I can call the Default
                this._doPreserveHandle = true;
                pMap.RemoveAll(FirstMsgHook.HookedHWND);
                this.DefWndProc(ref m);
                this.ReleaseHandle();
            }
        }
    }
    #endregion // Methods
}

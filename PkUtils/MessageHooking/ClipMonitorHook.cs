// Ignore Spelling: ctrl, Unregister, Utils, 
//
using System;
using System.Diagnostics;
using System.Windows.Forms;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.MessageHooking;


/// <summary> ClipMonitorHook is a message-hook-derived class, which monitors clipboard changes 
/// and notifies its subscribers. </summary>
[CLSCompliant(true)]
public class ClipMonitorHook : ControlMessageHook
{
    #region Fields

    /// <summary> The event being fired when anything on the system clipboard has changed.. </summary>
    public event EventHandler EventClipboardChanged;

    /// <summary> A backing field for the property <see cref="NextClipViewer"/>. </summary>
    protected IntPtr _viewerNext;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default argument-less constructor. </summary>
    public ClipMonitorHook()
    {
    }

    /// <summary> Constructor accepting the control whose messages should be monitored. </summary>
    ///
    /// <param name="ctrlToWatch">  The control to "watch". </param>
    public ClipMonitorHook(Control ctrlToWatch)
      : base(ctrlToWatch)
    {
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Gets a value indicating whether the clipboard viewer is registered. </summary>
    ///
    /// <seealso cref="RegisterClipboardViewer"/>
    public bool IsClipViewerRegistered
    {
        get { return (IntPtr.Zero != NextClipViewer); }
    }

    /// <summary> Gets the Win32 handle of next clipboard viewer in the chain. </summary>
    ///
    /// <value> The next clipboard viewer in the chain. </value>
    protected IntPtr NextClipViewer
    {
        get { return _viewerNext; }
    }
    #endregion // Properties

    #region Methods

    #region Protected methods

    /// <summary>
    /// Overrides the auxiliary helper called from HookWindow, in order to register ClipboardViewer.
    /// </summary>
    ///
    /// <param name="pExtraInfo"> The object providing additional information; possibly provided by the derived
    /// class in case it overrides the caller <see cref="WindowMessageHook.HookWindow(IntPtr)"/> </param>
    protected override void OnHookup(object pExtraInfo)
    {
        base.OnHookup(pExtraInfo);
        RegisterClipboardViewer();
    }

    /// <summary>
    /// Overrides the auxiliary helper called from HookWindow, in order to unregister ClipboardViewer.
    /// </summary>
    ///
    /// <param name="pExtraInfo"> The object providing additional information; possibly provided by the derived
    /// class in case it overrides the caller <see cref="WindowMessageHook.HookWindow(IntPtr)"/> </param>
    protected override void OnUnhook(object pExtraInfo)
    {
        UnregisterClipboardViewer();
        base.OnUnhook(pExtraInfo);
    }

#if DEBUG
    /// <summary>
    /// Overwrites the virtual method called from the OnctrlToHook_HandleCreated.
    /// provides additional verification ( in debug mode only ).
    /// </summary>
    ///
    /// <param name="args"> The event arguments of the original Control.HandleCreated Event. </param>
    protected override void OnHandleCreated(EventArgs args)
    {
        Debug.Assert(!IsClipViewerRegistered);
        base.OnHandleCreated(args);
    }
#endif // DEBUG

    /// <summary> Registers the clipboard viewer, if it has not been registered yet. </summary>
    ///
    /// <returns>   true if it succeeds, false if it fails. </returns>
    /// <seealso cref="UnregisterClipboardViewer"/>
    protected bool RegisterClipboardViewer()
    {
        bool result = false;

        if (!IsClipViewerRegistered)
        {
            _viewerNext = User32.SetClipboardViewer(this.HookedHWND);
            result = IsClipViewerRegistered;
        }
        return result;
    }

    /// <summary> Unregisters the clipboard viewer if it has been registered. </summary>
    ///
    /// <returns>   true if it succeeds, false if it fails. </returns>
    /// <seealso cref="RegisterClipboardViewer"/>
    protected bool UnregisterClipboardViewer()
    {
        bool result = false;

        if (IsClipViewerRegistered)
        {
            User32.ChangeClipboardChain(HookedHWND, NextClipViewer);
            _viewerNext = IntPtr.Zero;
            result = true;
        }
        return result;
    }


    /// <summary> Fires the event <see cref="EventClipboardChanged"/>. </summary>
    protected void FireClipboardChanged()
    {
        EventClipboardChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Overwrites the virtual method of the base class, to provide specific handling of
    /// messages WM_DRAWCLIPBOARD and WM_CHANGECBCHAIN.
    /// </summary>
    ///
    /// <param name="m">  [in,out] The Message structure which wraps Win32 messages that Windows sends. </param>
    protected override void HookWindowProc(ref Message m)
    {
        // process the message
        base.HookWindowProc(ref m);

        // post-process the message
        switch (m.Msg)
        {
            // The WM_DRAWCLIPBOARD message is sent to the first window in the clipboard viewer chain 
            // when the content of the clipboard changes.
            //
            // Each window that receives the WM_DRAWCLIPBOARD message 
            // must call the SendMessage function to pass the message 
            // on to the next window in the clipboard viewer chain.
            case (int)Win32.WM.WM_DRAWCLIPBOARD:
                FireClipboardChanged();
                User32.SendMessage(NextClipViewer, m.Msg, m.WParam, m.LParam);
                break;

            // The WM_CHANGECBCHAIN message is sent to the first window 
            // in the clipboard viewer chain when a window is being 
            // removed from the chain. 
            //
            // wParam is the Handle to the window being removed from 
            // the clipboard viewer chain 
            // lParam is the Handle to the next window in the chain 
            // following the window being removed. 
            //
            case (int)Win32.WM.WM_CHANGECBCHAIN:

                // When a clipboard viewer window receives the WM_CHANGECBCHAIN message, 
                // it should call the SendMessage function to pass the message to the 
                // next window in the chain, unless the next window is the window 
                // being removed. In this case, the clipboard viewer should save 
                // the handle specified by the lParam parameter as the next window in the chain. 
                if (m.WParam == NextClipViewer)
                {
                    //
                    // If wParam is the next clipboard viewer, then it is being removed,
                    // so update pointer to the next window in the clipboard chain
                    //
                    this._viewerNext = m.LParam;
                }
                else
                {
                    User32.SendMessage(NextClipViewer, m.Msg, m.WParam, m.LParam);
                }
                break;
        }
    }
    #endregion // Protected methods
    #endregion // Methods
}

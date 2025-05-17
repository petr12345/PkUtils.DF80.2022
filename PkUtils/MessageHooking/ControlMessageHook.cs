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


// Ignore Spelling: Utils, ctrl
//
using System;
using System.Diagnostics;
using System.Windows.Forms;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.MessageHooking;

/// <summary>
/// The <c>ControlMessageHook</c> class is a subtype of <see cref="WindowMessageHook"/> tailored for hooking
/// up objects derived from the <see cref="Control"/> class. It automatically subscribes to the <c>
/// Control.HandleCreated</c> event, ensuring automatic re-hookup upon handle recreation. It shares the
/// fundamental functionality of <see cref="WindowMessageHook"/>.
/// </summary>
/// <seealso cref="Win32WindowHook"/>
[CLSCompliant(true)]
public class ControlMessageHook : WindowMessageHook
{
    #region Fields

    /// <summary> Currently hooked control (if any). </summary>
    protected Control _ctrlHooked;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Argument-less constructor. You can create ControlMessageHook first without 
    /// providing any argument and hook it only later. 
    /// In that case, you should call the method ControlMessageHook.HookControl
    /// instead of the base class method WindowMessageHook.HookWindow(IntPtr hwnd),
    /// because the latter method does not subscribe to HandleCreated event.
    /// </summary>
    public ControlMessageHook()
    { }

    /// <summary> Constructor which directly hooks the given control. </summary>
    /// <param name="ctrlToHook"> The control being hooked. Must NOT be null or disposed. </param>
    public ControlMessageHook(Control ctrlToHook)
    {
        HookControl(ctrlToHook);
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> The 'watched' (hooked) control. </summary>
    protected Control ControlHooked { get => _ctrlHooked; }
    #endregion // Properties

    #region Public Methods

    /// <summary>
    /// Hooks the control and subscribes to HandleCreated event.
    /// </summary>
    /// <param name="ctrlToHook">The control being hooked. Must NOT be null or disposed.</param>
    /// <returns>True on success, false on failure.</returns>
    public bool HookControl(Control ctrlToHook)
    {
        bool bRes;

        ctrlToHook.CheckNotDisposed(nameof(ctrlToHook));
        if (bRes = HookWindow(ctrlToHook.Handle))
        {
            // Subscribe to the event HandleCreated, to handle properly the case when NET 
            // in the background recreates the window handle.
            SubscribeToControl(ctrlToHook);
        }
        return bRes;
    }
    #endregion // Public Methods

    #region Protected methods

    /// <summary>
    /// The virtual method called from the OnctrlToHook_HandleCreated.
    /// This base implementation just hooks the new window handle.
    /// </summary>
    /// <param name="args"> The event arguments of the original Control.HandleCreated Event </param>
    protected virtual void OnHandleCreated(EventArgs args)
    {
        HookWindow(ControlHooked.Handle);
    }

    /// <summary> Cleanup any resources being used. </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. 
    /// Otherwise it is called by finalizer.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            UnsubscribeFromControl();
        }
        base.Dispose(disposing);
    }

    /// <summary>
    /// Handler of the event HandleCreated.
    /// For case when the control has 'silently' destroyed its window handle 
    /// and recreates it again with the new handler value, it hooks that window again.
    /// Note: If the control's handle is destroyed and recreated, the base class handles cleanup
    /// on WM_NCDESTROY. Therefore, explicit Unhook() is not needed before re-hooking.
    /// </summary>
    /// <param name="sender">The source of the original Control.HandleCreated Event </param>
    /// <param name="args"> The event arguments of the original Control.HandleCreated Event </param>
    protected void OnCtrlToHook_HandleCreated(object sender, EventArgs args)
    {
        Debug.Assert(!this.IsHooked);
        OnHandleCreated(args);
    }
    #endregion // Protected methods

    #region Private Methods

    /// <summary>
    /// Assign the _ctrlToHook to provided control,  and subscribe to event HandleCreated.
    /// This is needed  to handle properly the case when NET in the background recreates the window handle.
    /// </summary>
    /// <param name="ctrl">provided WinForms control. </param>
    private void SubscribeToControl(Control ctrl)
    {
        Debug.Assert(null == _ctrlHooked);
        Debug.Assert(null != ctrl);

        _ctrlHooked = ctrl;
        // Subscribe to the event HandleCreated, to handle properly the case when NET 
        // in the background recreates the window handle.
        ctrl.HandleCreated += new EventHandler(OnCtrlToHook_HandleCreated);
    }

    /// <summary>
    /// Unsubscribe from the event HandleCreated and assign _ctrlToHook to null.
    /// </summary>
    private void UnsubscribeFromControl()
    {
        if (null != ControlHooked)
        {   // unsubscribe from the event HandleCreated 
            ControlHooked.HandleCreated -= OnCtrlToHook_HandleCreated;
            _ctrlHooked = null;
        }
    }
    #endregion // Private Methods
}
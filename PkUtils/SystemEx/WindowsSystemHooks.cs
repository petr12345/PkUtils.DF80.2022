/***************************************************************************************************************
*
* FILE NAME:   .\SystemEx\WindowsSystemHooks.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of several classed derived from WindowsSystemHookBase:
*   WindowsSystemHookMouse     ( with Win32.HookType.WH_MOUSE )
*   WindowsSystemHookKeyboard  ( with Win32.HookType.WH_KEYBOARD )
*   WindowsSystemHookMouseLL    ( with Win32.HookType.WH_MOUSE_LL )
*   WindowsSystemHookKbLL      ( with Win32.HookType.WH_KEYBOARD_LL )
*   WindowsSystemHookCallWndProcRet ( with Win32.HookType.WH_CALLWNDPROCRET )
*
**************************************************************************************************************/


// Ignore Spelling: Utils, Meth, Ret, mss, kbs
//
using System;
using System.Runtime.InteropServices;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.SystemEx;

#region WindowsSystemHookMouse

/// <summary>
/// Derives from WindowsSystemHookBase to simplify system hooking with Win32.HookType.WH_MOUSE.
/// To use this hook type, either create an instance with your own User32.HookProc 
/// provided as an argument, or derive from WindowsSystemHookMouse and override the MouseHookMeth method.
/// </summary>
/// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644959(v=vs.85).aspx">
/// Windows Hooks Overview </seealso>
[CLSCompliant(false)]
public class WindowsSystemHookMouse : WindowsSystemHookBase
{
    #region Constructor(s)

    /// <summary>
    /// The argument-less constructor, hooks using CoreMouseHookProc method.
    /// </summary>
    public WindowsSystemHookMouse()
      : this(null)
    { }

    /// <summary>
    /// The constructor providing a hook method argument.
    /// If null is provided, uses the delegate created from this.CoreMouseHookProc
    /// </summary>
    /// <param name="func">A delegate that will be used  for system hooking, by User32.SetWindowsHookEx</param>
    public WindowsSystemHookMouse(User32.HookProc func)
      : base(WinApi.Win32.HookType.WH_MOUSE)
    {
        _filterFunc = func ?? new User32.HookProc(this.CoreMouseHookProc);
    }
    #endregion // Constructor(s)

    #region Methods

    /// <summary>
    /// A virtual method called by a delegate CoreMouseHookProc. In a derived class you will overwrite this
    /// method. </summary>
    ///
    /// <param name="m"> A MOUSEHOOKSTRUCT structure, containing information about a mouse event passed to a
    ///   WH_MOUSE hook procedure. </param>
    /// <returns> True if the message has been completely handled by this hook instance, 
    ///            and the caller should NOT proceed passing it to other hooks in the chain; 
    ///            false otherwise. Use 'true' value with care! </returns>
    protected virtual bool MouseHookMeth(WinApi.Win32.MOUSEHOOKSTRUCT m)
    {
        return false;
    }

    /// <summary>
    /// A delegate to handle the mouse events for the WH_MOUSE hook
    /// </summary>
    /// <param name="code">A code the hook procedure uses to determine how to process the message. 
    /// If nCode is less than zero, the hook procedure must pass the message to the CallNextHookEx function 
    /// without further processing and should return the value returned by CallNextHookEx. 
    /// </param>
    /// <param name="wParam">The identifier of the mouse message. </param>
    /// <param name="lParam">A pointer to a Win32.MOUSEHOOKSTRUCT structure</param>
    /// <returns>A value returned by the next hook procedure in the chain. </returns>
    protected IntPtr CoreMouseHookProc(int code, IntPtr wParam, IntPtr lParam)
    {
        IntPtr result;

        if ((code < 0) || (code == WinApi.Win32.HC_NOREMOVE))
        {  // for these values, must limit itself to just calling next hook in chain
            result = CallNextHook(code, wParam, lParam);
        }
        else
        {
            WinApi.Win32.MOUSEHOOKSTRUCT m = (WinApi.Win32.MOUSEHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinApi.Win32.MOUSEHOOKSTRUCT));

            if (MouseHookMeth(m))
                result = IntPtr.Zero;
            else
                result = CallNextHook(code, wParam, lParam);
        }

        return result;
    }
    #endregion // Methods
}
#endregion // WindowsSystemHookMouse

#region WindowsSystemHookKeyboard

/// <summary>
/// Derives from WindowsSystemHookBase to simplify system hooking with Win32.HookType.WH_KEYBOARD.
/// To use this hook type, either create an instance with your own User32.HookProc 
/// provided as an argument, or derive from WindowsSystemHookKeyboard and override the KeyboardHookMeth method.
/// </summary>
/// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644959(v=vs.85).aspx">
/// Windows Hooks Overview </seealso>
[CLSCompliant(false)]
public class WindowsSystemHookKeyboard : WindowsSystemHookBase
{
    #region Constructor(s)

    /// <summary>
    /// The argument-less constructor, hooks using CoreKeyboardHookProc method.
    /// </summary>
    public WindowsSystemHookKeyboard()
      : this(null)
    { }

    /// <summary>
    /// The constructor providing a hook delegate argument.
    /// If null is provided, uses the delegate created from this.CoreKeyboardHookProc
    /// </summary>
    /// <param name="func">A delegate that will be used  for system hooking, by User32.SetWindowsHookEx</param>
    public WindowsSystemHookKeyboard(User32.HookProc func)
      : base(WinApi.Win32.HookType.WH_KEYBOARD)
    {
        _filterFunc = func ?? new User32.HookProc(this.CoreKeyboardHookProc);
    }
    #endregion // Constructor(s)

    #region Methods

    /// <summary> A virtual method called by a delegate CoreKeyboardHookProc. In a derived class you will
    /// overwrite this method. </summary>
    ///
    /// <param name="wParam"> The virtual-key code of the key that generated the keystroke message. </param>
    /// <param name="lParam"> The repeat count, scan code, extended-key flag, context code, previous key-state flag, and
    /// transition-state flag. For more information about the lParam parameter, see Keystroke Message
    /// Flags reference. </param>
    ///
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms646267(v=vs.85).aspx#_win32_Keystroke_Message_Flags">
    /// Keystroke Message Flags
    /// </seealso>
    /// <returns> True if the message has been completely handled by this hook instance, 
    ///            and the caller should NOT proceed passing it to other hooks in the chain; 
    ///            false otherwise. Use 'true' value with care! </returns>
    protected virtual bool KeyboardHookMeth(IntPtr wParam, IntPtr lParam)
    {
        return false;
    }

    /// <summary> A delegate to handle the mouse events for the WH_KEYBOARD hook. </summary>
    ///
    /// <param name="code">   A code the hook procedure uses to determine how to process the message. If nCode is
    ///   less than zero, the hook procedure must pass the message to the CallNextHookEx function without further
    ///   processing and should return the value returned by CallNextHookEx. </param>
    /// <param name="wParam"> The wParam value passed to the current hook procedure.  The virtual-key code of the
    ///   key that generated the keystroke message. </param>
    /// <param name="lParam"> The repeat count, scan code, extended-key flag, context code, previous key-state
    ///   flag, and transition-state flag. For more information about the lParam parameter, see Win32
    ///   documentation. </param>
    ///
    /// <returns> A value returned by the next hook procedure in the chain. </returns>
    protected IntPtr CoreKeyboardHookProc(int code, IntPtr wParam, IntPtr lParam)
    {
        IntPtr result;

        if ((code < 0) || (code == WinApi.Win32.HC_NOREMOVE))
        {  // for these values, must limit itself to just calling next hook in chain
            result = CallNextHook(code, wParam, lParam);
        }
        else
        {
            if (KeyboardHookMeth(wParam, lParam))
                result = IntPtr.Zero;
            else
                result = CallNextHook(code, wParam, lParam);
        }

        return result;
    }
    #endregion // Methods
}
#endregion // WindowsSystemHookKeyboard

#region WindowsSystemHookMouseLL

/// <summary>
/// Derives from WindowsSystemHookBase to simplify system hooking with Win32.HookType.WH_MOUSE_LL.
/// To use this hook type, either create an instance with your own User32.HookProc provided as an argument,
/// or derive from WindowsSystemHookMouseLL and override the <see cref="MouseLLHookMeth"/> method.
/// </summary>
/// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644959(v=vs.85).aspx">
/// Windows Hooks Overview </seealso>
[CLSCompliant(false)]
public class WindowsSystemHookMouseLL : WindowsSystemHookBase
{
    #region Constructor(s)

    /// <summary>
    /// The argument-less constructor, hooks using CoreKeyboardLLProc method.
    /// </summary>
    public WindowsSystemHookMouseLL()
      : this(null)
    { }

    /// <summary>
    /// The constructor providing a hook delegate. 
    /// If null is provided, uses the delegate created from <see cref="CoreMouseLLProc"/>.
    /// </summary>
    /// <param name="func">A delegate that will be used  for system hooking, by User32.SetWindowsHookEx</param>
    public WindowsSystemHookMouseLL(User32.HookProc func)
      : base(WinApi.Win32.HookType.WH_MOUSE_LL)
    {
        _filterFunc = func ?? new User32.HookProc(this.CoreMouseLLProc);
    }
    #endregion // Constructor(s)

    #region Methods

    /// <summary> Installs the hook. Overrides the implementation in the base class. </summary>
    /// <remarks>
    /// While the base installs the hook on the current thread by default,
    /// the hooks WH_KEYBOARD_LL and WH_MOUSE_LL (unlike the other hooks) must be global;
    /// that is, the threadID parameter must be 0.
    /// Otherwise, the SetWindowsHookEx API fails, setting the GetLastError value to
    /// error code 1429 ( ERROR_GLOBAL_ONLY_HOOK ).
    /// </remarks>
    public override void Install()
    {
        Install(0);
    }

    /// <summary>
    /// A virtual method called by a delegate <see cref="CoreMouseLLProc"/>.
    /// In a derived class you may overwrite this method.
    /// </summary>
    /// <param name="wParam"> The identifier of the mouse message. </param>
    /// <param name="mss">A structure containing information about a low-level mouse input event.</param>
    /// <returns> True if the message has been completely handled by this hook instance, 
    ///            and the caller should NOT proceed passing it to other hooks in the chain; 
    ///            false otherwise. Use 'true' value with care! </returns>
    protected virtual bool MouseLLHookMeth(IntPtr wParam, Win32.MSLLHOOKSTRUCT mss)
    {
        return false;
    }

    /// <summary>
    /// A delegate to handle the mouse events for the low level mouse hook
    /// </summary>
    /// <param name="code">A code the hook procedure uses to determine how to process the message. 
    /// If nCode is less than zero, the hook procedure must pass the message to the CallNextHookEx function 
    /// without further processing and should return the value returned by CallNextHookEx. 
    /// </param>
    /// <param name="wParam"> The identifier of the mouse message. </param>
    /// <param name="lParam">A pointer to a <see cref="WinApi.Win32.MSLLHOOKSTRUCT"/> structure</param>
    /// <returns>A value returned by the next hook procedure in the chain. </returns>
    protected IntPtr CoreMouseLLProc(int code, IntPtr wParam, IntPtr lParam)
    {
        IntPtr result;

        if ((code < 0) || (code == WinApi.Win32.HC_NOREMOVE))
        {  // for these values, must limit itself to just calling next hook in chain
            result = CallNextHook(code, wParam, lParam);
        }
        else
        {
            WinApi.Win32.MSLLHOOKSTRUCT mss = (WinApi.Win32.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinApi.Win32.MSLLHOOKSTRUCT));

            if (MouseLLHookMeth(wParam, mss))
                result = IntPtr.Zero;
            else
                result = CallNextHook(code, wParam, lParam);
        }

        return result;
    }
    #endregion // Methods
}
#endregion // WindowsSystemHookMouseLL

#region WindowsSystemHookKbLL

/// <summary>
/// Derives from WindowsSystemHookBase to simplify system hooking with Win32.HookType.WH_KEYBOARD_LL.
/// To use this hook type, either create an instance with your own User32.HookProc 
/// provided as an argument, or derive from WindowsSystemHookMouse and override the KeyboardLLHookMeth method.
/// </summary>
/// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644959(v=vs.85).aspx">
/// Windows Hooks Overview </seealso>
[CLSCompliant(false)]
public class WindowsSystemHookKbLL : WindowsSystemHookBase
{
    #region Constructor(s)

    /// <summary>
    /// The argument-less constructor, hooks using CoreKeyboardLLProc method.
    /// </summary>
    public WindowsSystemHookKbLL()
      : this(null)
    { }

    /// <summary>
    /// The constructor providing a hook delegate. 
    /// If null is provided, uses the delegate created from this.CoreKeyboardLLProc.
    /// </summary>
    /// <param name="func">A delegate that will be used  for system hooking, by User32.SetWindowsHookEx</param>
    public WindowsSystemHookKbLL(User32.HookProc func)
      : base(WinApi.Win32.HookType.WH_KEYBOARD_LL)
    {
        _filterFunc = func ?? new User32.HookProc(this.CoreKeyboardLLProc);
    }
    #endregion // Constructor(s)

    #region Methods

    /// <summary>
    /// Installs the hook. Overrides the implementation in the base class.
    /// </summary>
    /// <remarks>
    /// While the base installs the hook on the current thread by default,
    /// the hooks WH_KEYBOARD_LL and WH_MOUSE_LL (unlike the other hooks) must be global;
    /// that is, the threadID parameter must be 0.
    /// Otherwise, the SetWindowsHookEx API fails, setting the GetLastError value to
    /// error code 1429 ( ERROR_GLOBAL_ONLY_HOOK ).
    /// </remarks>
    public override void Install()
    {
        Install(0);
    }

    /// <summary>
    /// A virtual method called by a delegate CoreKeyboardLLProc. 
    /// In a derived class you will overwrite this method.
    /// </summary>
    /// <param name="kbs">A structure containing information about a low-level keyboard input event.</param>
    /// <returns> True if the message has been completely handled by this hook instance, 
    ///            and the caller should NOT proceed passing it to other hooks in the chain; 
    ///            false otherwise. Use 'true' value with care! </returns>
    protected virtual bool KeyboardLLHookMeth(WinApi.Win32.KBDLLHOOKSTRUCT kbs)
    {
        return false;
    }

    /// <summary>
    /// A delegate to handle the keyboard events for the low level keyboard hook
    /// </summary>
    /// <param name="code">A code the hook procedure uses to determine how to process the message. 
    /// If nCode is less than zero, the hook procedure must pass the message to the CallNextHookEx function 
    /// without further processing and should return the value returned by CallNextHookEx. 
    /// </param>
    /// <param name="wParam"> The identifier of the keyboard message. 
    /// This parameter can be one of the following messages: WM_KEYDOWN, WM_KEYUP, WM_SYSKEYDOWN, or WM_SYSKEYUP.
    /// </param>
    /// <param name="lParam">A pointer to a KBDLLHOOKSTRUCT structure</param>
    /// <returns>A value returned by the next hook procedure in the chain. </returns>
    protected IntPtr CoreKeyboardLLProc(int code, IntPtr wParam, IntPtr lParam)
    {
        IntPtr result;

        if ((code < 0) || (code == WinApi.Win32.HC_NOREMOVE))
        {  // for these values, must limit itself to just calling next hook in chain
            result = CallNextHook(code, wParam, lParam);
        }
        else
        {
            WinApi.Win32.KBDLLHOOKSTRUCT kbs = (WinApi.Win32.KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinApi.Win32.KBDLLHOOKSTRUCT));

            if (KeyboardLLHookMeth(kbs))
                result = IntPtr.Zero;
            else
                result = CallNextHook(code, wParam, lParam);
        }

        return result;
    }
    #endregion // Methods
}
#endregion // WindowsSystemHookKbLL

#region WindowsSystemHookCallWndProcRet

/// <summary>
/// Derives from WindowsSystemHookBase to simplify system hooking with Win32.HookType.WH_CALLWNDPROCRET.
/// To use this hook type, either create an instance with your own User32.HookProc 
/// provided as an argument, or derive from WindowsSystemHookCallWndProcRet and override the WndProcRetHookMeth method.
/// </summary>
/// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644959(v=vs.85).aspx">
/// Windows Hooks Overview </seealso>
[CLSCompliant(false)]
public class WindowsSystemHookCallWndProcRet : WindowsSystemHookBase
{
    #region Constructor(s)

    /// <summary>
    /// The argument-less constructor, hooks using <see cref="CoreWndProcRetHookProc"/>method.
    /// </summary>
    public WindowsSystemHookCallWndProcRet()
      : this(null)
    { }

    /// <summary>
    /// The constructor providing a hook delegate argument.
    /// If null is provided, uses the delegate created from <see cref="CoreWndProcRetHookProc"/>
    /// </summary>
    /// <param name="func">A delegate that will be used  for system hooking, by User32.SetWindowsHookEx</param>
    public WindowsSystemHookCallWndProcRet(User32.HookProc func)
      : base(WinApi.Win32.HookType.WH_CALLWNDPROCRET)
    {
        _filterFunc = func ?? new User32.HookProc(this.CoreWndProcRetHookProc);
    }
    #endregion // Constructor(s)

    #region Methods

    /// <summary> A virtual method called by a delegate <see cref="CoreWndProcRetHookProc"/>.
    /// In a derived class you will overwrite this method. </summary>
    /// <param name="msg"> Message parameters passed to a WH_CALLWNDPROCRET hook procedure. </param>
    /// <returns> True if the message has been completely handled by this hook instance, 
    ///            and the caller should NOT proceed passing it to other hooks in the chain; 
    ///            false otherwise. Use 'true' value with care! </returns>
    /// <remarks> For more details, see
    /// <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644963(v=vs.85).aspx">
    /// CWPRETSTRUCT structure </a> description. </remarks>
    protected virtual bool WndProcRetHookMeth(WinApi.Win32.CWPRETSTRUCT msg)
    {
        return false;
    }

    /// <summary> A delegate which can inspect the Win32.CWPRETSTRUCT structure for the
    /// WH_CALLWNDPROCRET hook. The system calls the WH_CALLWNDPROCRET hook function after the
    /// SendMessage function is called. The hook procedure can examine the message; it cannot modify
    /// it. </summary>
    ///
    /// <param name="code">   A code the hook procedure uses to determine how to process the message.
    /// If nCode is less than zero, the hook procedure must pass the message to the CallNextHookEx
    /// function without further processing and should return the value returned by CallNextHookEx. </param>
    /// <param name="wParam"> Specifies whether the message is sent by the current process. If the
    /// message is sent by the current process, it is nonzero; otherwise, it is NULL. </param>
    /// <param name="lParam"> A pointer to a
    /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644963(v=vs.85).aspx">
    /// CWPRETSTRUCT structure </see> </param>
    ///
    /// <returns> This value is returned by the next hook procedure in the chain. </returns>
    protected IntPtr CoreWndProcRetHookProc(int code, IntPtr wParam, IntPtr lParam)
    {
        IntPtr result;

        if ((code < 0) || (code == WinApi.Win32.HC_NOREMOVE))
        {  // for these values, must limit itself to just calling next hook in chain
            result = CallNextHook(code, wParam, lParam);
        }
        else
        {
            WinApi.Win32.CWPRETSTRUCT msg = (WinApi.Win32.CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(WinApi.Win32.CWPRETSTRUCT));

            if (WndProcRetHookMeth(msg))
                result = IntPtr.Zero;
            else
                result = CallNextHook(code, wParam, lParam);
        }

        return result;
    }
    #endregion // Methods
}
#endregion // WindowsSystemHookCallWndProcRet

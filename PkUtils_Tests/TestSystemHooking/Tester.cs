using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.SystemEx;
using PK.PkUtils.Utils;
using PK.PkUtils.WinApi;

namespace PK.TestSystemHooking
{
    /// <summary>
    /// The actual testing class, containing four Windows System Hooks, creating & disposing them.
    /// </summary>
    public class Tester : IDisposable
    {
        #region Fields

        protected readonly IDumper _dumper;

        /// <summary>
        ///  The system hook of type WH_MOUSE
        /// </summary>
        private WindowsSystemHookBase _mouse_hook;

        /// <summary>
        ///  The system hook of type WH_KEYBOARD
        /// </summary>
        private WindowsSystemHookBase _keyboard_hook;

        /// <summary>
        ///  The system hook of type WH_MOUSE_LL
        /// </summary>
        private WindowsSystemHookBase _mouse_ll_hook;

        /// <summary>
        ///  The system hook of type WH_KEYBOARD_LL
        /// </summary>
        private WindowsSystemHookBase _keyboard_ll_hook;

        /// <summary>
        ///  The system hook of type WH_CALLWNDPROCRET
        /// </summary>
        private WindowsSystemHookBase _callWndProc_hook;

        /// <summary> The reentrancy lock, used in FnCallWndProcHook. </summary>
        private readonly UsageCounter _reentrancyLock = new();

        #endregion // Fields

        #region Constructor(s)

        public Tester(IDumper dumper)
        {
            dumper.CheckArgNotNull(nameof(dumper));
            _dumper = dumper;
        }
        #endregion // Constructor(s)

        #region Properties

        public bool IsMouseHookInstalled
        {
            get { return ((_mouse_hook != null) && _mouse_hook.IsInstalled); }
        }

        public bool IsKeyboardHookInstalled
        {
            get { return ((_keyboard_hook != null) && _keyboard_hook.IsInstalled); }
        }

        public bool IsMouseLowLevelHookInstalled
        {
            get { return ((_mouse_ll_hook != null) && _mouse_ll_hook.IsInstalled); }
        }

        public bool IsKeyboardLowLevelHookInstalled
        {
            get { return ((_keyboard_ll_hook != null) && _keyboard_ll_hook.IsInstalled); }
        }

        public bool IsCallWndProcHookInstalled
        {
            get { return ((_callWndProc_hook != null) && _callWndProc_hook.IsInstalled); }
        }

        protected IDumper Dumper
        {
            get { return _dumper; }
        }
        #endregion // Properties

        #region Methods

        #region mouse-hook functionality

        /// <summary>
        /// sets the mouse hook
        /// </summary>
        /// <returns></returns>
        public bool SetMouseHook()
        {
            bool bRes;

            _mouse_hook ??= new WindowsSystemHookMouse(new User32.HookProc(MouseHookMessageProc));
            if (!(bRes = _mouse_hook.IsInstalled))
            {
                _mouse_hook.Install();
                bRes = _mouse_hook.IsInstalled;
            }

            return bRes;
        }

        /// <summary>
        /// uninstalls the mouse hook
        /// </summary>
        public void UnhookMouseHook()
        {
            Disposer.SafeDispose(ref _mouse_hook);
        }

        /// <summary>
        /// A delegate called for the mouse messages that are about to be pumped to the application.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        protected IntPtr MouseHookMessageProc(int code, IntPtr wParam, IntPtr lParam)
        {
            IntPtr nResult;

            if ((code < 0) || (code == Win32.HC_NOREMOVE))
            {  // for these values, must limit itself to just calling next hook in chain
                nResult = _mouse_hook.CallNextHook(code, wParam, lParam);
            }
            else
            {
                Win32.MOUSEHOOKSTRUCT m = (Win32.MOUSEHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(Win32.MOUSEHOOKSTRUCT));
                Point ptScreen = new(m.pt.x, m.pt.y);
                string strMsg = string.Format(CultureInfo.InvariantCulture,
                  "WH_MOUSE - Mouse event screen coordinates = ({0},{1}).",
                  ptScreen.X, ptScreen.Y);

                Dumper.DumpLine(strMsg);
                nResult = _mouse_hook.CallNextHook(code, wParam, lParam);
            }

            return nResult;
        }
        #endregion // mouse-hook functionality

        #region keyboard-hook functionality

        /// <summary>
        /// installs the low level keyboard hook
        /// </summary>
        /// <returns></returns>
        public bool SetKeyboardHook()
        {
            bool bRes;

            _keyboard_hook ??= new WindowsSystemHookKeyboard(new User32.HookProc(KeyboardProc));

            if (!(bRes = _keyboard_hook.IsInstalled))
            {
                _keyboard_hook.Install();
                bRes = _keyboard_hook.IsInstalled;
            }

            return bRes;
        }

        /// <summary>
        /// uninstalls the low-level keyboard hook
        /// </summary>
        public void UnhookKeyboardHook()
        {
            Disposer.SafeDispose(ref _keyboard_hook);
        }

        /// <summary>
        /// A delegate to handle the keyboard events for the low-level hook
        /// </summary>
        /// <param name="code"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        protected IntPtr KeyboardProc(int code, IntPtr wParam, IntPtr lParam)
        {
            IntPtr nResult = IntPtr.Zero;

            if ((code < 0) || (code == Win32.HC_NOREMOVE))
            {  // for these values, must limit itself to just calling next hook in chain
                nResult = _keyboard_hook.CallNextHook(code, wParam, lParam);
            }
            else
            {
                int vkCode = unchecked((int)wParam);
                int lPar = 0;
                bool bPressed = false;

                // Rather than IntPtr.ToInt32(), for the case of x64 platform the following code needs to perform work-around cast, 
                // to avoid OverflowException being thrown on x64 platform even for values that fit in Int32.
                // The reason of that behaviour is the implementation of IntPtr.ToInt32(). See more on
                // http://stackoverflow.com/questions/11192523/why-intptr-toint32-throws-overflowexception-in-64-bit-mode-and-explicitintptr-t
                if (IntPtr.Size == 4)
                {
                    lPar = unchecked((int)lParam);
                    bPressed = (lPar >= 0);
                }
                else
                {
                    lPar = (int)lParam.ToInt64();
                    bPressed = (lPar >= 0);
                }

                string strAdd = bPressed ? "Pressed" : "Released";
                string strMsg = string.Format(CultureInfo.InvariantCulture,
                  "WH_KEYBOARD - Keyboard event: vkCode = {0}, {1}",
                  wParam, strAdd);

                Dumper.DumpLine(strMsg);
                nResult = _keyboard_hook.CallNextHook(code, wParam, lParam);
            }

            return nResult;
        }
        #endregion // keyboard-hook functionality

        #region low-level mouse-hook functionality

        /// <summary>
        /// installs the low level mouse hook
        /// </summary>
        /// <returns></returns>
        public bool SetLLMouseHook()
        {
            bool bRes;

            _mouse_ll_hook ??= new WindowsSystemHookMouseLL(new User32.HookProc(MouseLLProc));
            if (!(bRes = _mouse_ll_hook.IsInstalled))
            {
                _mouse_ll_hook.Install();
                bRes = _mouse_ll_hook.IsInstalled;
            }

            return bRes;
        }

        /// <summary>
        /// uninstalls the low-level mouse hook
        /// </summary>
        public void UnhookLLMouseHook()
        {
            Disposer.SafeDispose(ref _mouse_ll_hook);
        }

        /// <summary>
        /// A delegate to handle the mouse events for the low level mouse hook
        /// </summary>
        /// <param name="code"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        protected IntPtr MouseLLProc(int code, IntPtr wParam, IntPtr lParam)
        {
            IntPtr nResult = IntPtr.Zero;

            if ((code < 0) || (code == Win32.HC_NOREMOVE))
            {  // for these values, must limit itself to just calling next hook in chain
                nResult = _mouse_ll_hook.CallNextHook(code, wParam, lParam);
            }
            else
            {
                string strWPar;

                switch ((int)wParam)
                {
                    case (int)Win32.WM.WM_MOUSEMOVE:
                        strWPar = "WM_MOUSEMOVE";
                        break;

                    case (int)Win32.WM.WM_LBUTTONDOWN:
                        strWPar = "WM_LBUTTONDOWN";
                        break;

                    case (int)Win32.WM.WM_LBUTTONUP:
                        strWPar = "WM_LBUTTONUP";
                        break;

                    case (int)Win32.WM.WM_LBUTTONDBLCLK:
                        strWPar = "WM_LBUTTONDBLCLK";
                        break;

                    case (int)Win32.WM.WM_RBUTTONDOWN:
                        strWPar = "WM_RBUTTONDOWN";
                        break;

                    case (int)Win32.WM.WM_RBUTTONUP:
                        strWPar = "WM_RBUTTONUP";
                        break;

                    default:
                        strWPar = wParam.ToString("X");
                        break;
                }

                if (!string.IsNullOrEmpty(strWPar))
                {
                    Win32.MSLLHOOKSTRUCT mss = (Win32.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(Win32.MSLLHOOKSTRUCT));
                    string strMsg = string.Format(CultureInfo.InvariantCulture,
                      "WH_MOUSE_LL - MouseLLProc called with {0}, mss.pt = {1}", strWPar, mss.pt);

                    Dumper.DumpLine(strMsg);
                    nResult = _mouse_ll_hook.CallNextHook(code, wParam, lParam);
                }
            }

            return nResult;
        }

        #endregion // low-level mouse-hook functionality

        #region low-level keyboard-hook functionality

        /// <summary>
        /// installs the low level keyboard hook
        /// </summary>
        /// <returns></returns>
        public bool SetLLKeyboardHook()
        {
            bool bRes;

            _keyboard_ll_hook ??= new WindowsSystemHookKbLL(new User32.HookProc(KeyboardLLProc));
            if (!(bRes = _keyboard_ll_hook.IsInstalled))
            {
                _keyboard_ll_hook.Install();
                bRes = _keyboard_ll_hook.IsInstalled;
            }

            return bRes;
        }

        /// <summary>
        /// uninstalls the low-level keyboard hook
        /// </summary>
        public void UnhookLLKeyboardHook()
        {
            Disposer.SafeDispose(ref _keyboard_ll_hook);
        }

        /// <summary>
        /// A delegate to handle the keyboard events for the low level keyboard hook
        /// </summary>
        /// <param name="code"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        protected IntPtr KeyboardLLProc(int code, IntPtr wParam, IntPtr lParam)
        {
            IntPtr nResult = IntPtr.Zero;

            if ((code < 0) || (code == Win32.HC_NOREMOVE))
            {  // for these values, must limit itself to just calling next hook in chain
                nResult = _keyboard_ll_hook.CallNextHook(code, wParam, lParam);
            }
            else
            {
                string strWPar = null;

                switch ((int)wParam)
                { // the WM_KEYDOWN and WM_KEYUP are still useful, since for the "Alt" key itself comes all WM_SYSKEYDOWN, WM_SYSKEYUP, WM_KEYDOWN, WM_KEYUP
                    case (int)Win32.WM.WM_SYSKEYDOWN:
                        strWPar = "WM_SYSKEYDOWN";
                        break;
                    case (int)Win32.WM.WM_SYSKEYUP:
                        strWPar = "WM_SYSKEYUP";
                        break;
                    case (int)Win32.WM.WM_KEYDOWN:
                        strWPar = "WM_KEYDOWN";
                        break;
                    case (int)Win32.WM.WM_KEYUP:
                        strWPar = "WM_KEYUP";
                        break;
                }

                if (!string.IsNullOrEmpty(strWPar))
                {
                    Win32.KBDLLHOOKSTRUCT kbs = (Win32.KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(Win32.KBDLLHOOKSTRUCT));
                    string strMsg = string.Format(CultureInfo.InvariantCulture,
                      "WH_KEYBOARD_LL - KeyboardLLProc called with {0}, kbs.vkCode = {1}",
                      strWPar, kbs.vkCode);

                    Dumper.DumpLine(strMsg);
                    nResult = _keyboard_ll_hook.CallNextHook(code, wParam, lParam);
                }
            }

            return nResult;
        }
        #endregion // low-level keyboard-hook functionality

        #region WH_CALLWNDPROCRET hook functionality

        /// <summary>
        /// installs the WH_CALLWNDPROCRET hook
        /// </summary>
        /// <returns></returns>
        public bool SetCallWndProcHook()
        {
            bool bRes;

            _callWndProc_hook ??= new WindowsSystemHookBase(Win32.HookType.WH_CALLWNDPROCRET, new User32.HookProc(FnCallWndProcHook));

            if (!(bRes = _callWndProc_hook.IsInstalled))
            {
                _callWndProc_hook.Install();
                bRes = _callWndProc_hook.IsInstalled;
            }

            return bRes;
        }

        /// <summary>
        /// uninstalls the WH_CALLWNDPROCRET
        /// </summary>
        public void UnhookCallWndProcHook()
        {
            Disposer.SafeDispose(ref _callWndProc_hook);
        }

        /// <summary>
        /// A delegate called by WH_CALLWNDPROCRET hook
        /// </summary>
        /// <param name="code"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        protected IntPtr FnCallWndProcHook(int code, IntPtr wParam, IntPtr lParam)
        {
            IntPtr nResult = IntPtr.Zero;

            if ((code < 0) || (code == Win32.HC_NOREMOVE))
            {  // for these values, must limit itself to just calling next hook in chain
                nResult = _callWndProc_hook.CallNextHook(code, wParam, lParam);
            }
            else if (_reentrancyLock.IsUsed)
            {
                nResult = _callWndProc_hook.CallNextHook(code, wParam, lParam);
            }
            else
            { // to prevent hanging maintains a reentrancy lock 
                using (var lockUser = new UsageCounterWrapper(_reentrancyLock))
                {
                    Win32.CWPRETSTRUCT msg = (Win32.CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(Win32.CWPRETSTRUCT));
                    string strMsg = string.Format(CultureInfo.InvariantCulture,
                      "WH_CALLWNDPROCRET - called with hwnd={0}, message={1}, wParam={2}, lParam={3}, lResult={4}",
                      msg.hwnd, msg.message, msg.wParam, msg.lParam, msg.lResult);

                    Dumper.DumpLine(strMsg);

                    nResult = _callWndProc_hook.CallNextHook(code, wParam, lParam);
                }
            }

            return nResult;
        }
        #endregion // WH_CALLWNDPROCRET hook functionality
        #endregion // Methods

        #region IDisposable Members

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnhookMouseHook();
                UnhookKeyboardHook();
                UnhookLLMouseHook();
                UnhookLLKeyboardHook();
                UnhookCallWndProcHook();
            }
        }

        /// <summary>
        /// Implements IDisposable. Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion // IDisposable Members
    }
}

// Ignore Spelling: Utils, Dict
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using PK.PkUtils.SystemEx;
using PK.PkUtils.WinApi;

#pragma warning disable IDE0290 // Use primary constructor

namespace PK.PkUtils.UI.Utils;

/// <summary>
/// This class "spies" the system for any creation of system MessageBox, 
/// and consequent destroying of it, keeping internally the list of created message boxes 
/// ( their window handles ).
/// Note the code handles the TaskDialog(s) (new in Vista, Win 7 etc.) too, 
/// since TaskDialog and MessageBox has the same window class name "#32770". <br/>
/// 
/// Unfortunately, there is no functionality in WinForms for this 
/// ( there are methods Application.OpenForms, Form.ActiveForm and Control.FromHandle,
/// but nothing of this relates to plain system MessageBox. <br/>
/// 
/// One can check whether there is any MessageBox currently displayed
/// by checking the property IsAnyMsgBox.<br/>
/// </summary>
/// 
/// <remarks>
/// The implementation uses the functionality of the base class WindowsSystemHookBase,
/// creating and installing a specific hook WH_CALLWNDPROCRET, 
/// in the hook callback method watching for messages WM_CREATE and WM_DESTROY.
/// </remarks>
[CLSCompliant(false)]
public class MsgBoxObserver : SystemEventObserver<MsgBoxObserver.EventMsgBoxActionEventArgs>
{
    #region Typedefs

    /// <summary>
    /// Auxiliary class keeping information about message box. </summary>
    /// <remarks>On purpose it is not structure - to be able to derive from that in descendants of MsgBoxObserver</remarks>
    public class MsgBoxInfo
    {
        /// <summary> The MessageBox handle. </summary>
        protected readonly IntPtr _hwnd;

        /// <summary>
        /// The MessageBox title. </summary>
        protected readonly string _title;

        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="hwnd">The MessageBox handle. </param>
        /// <param name="title">The MessageBox title. </param>
        internal MsgBoxInfo(IntPtr hwnd, string title)
          : this(hwnd, title, null)
        {
            _hwnd = hwnd;
            _title = title;
        }

        /// <summary>
        ///  A constructor
        /// </summary>
        /// <param name="hwnd">The MessageBox handle. </param>
        /// <param name="title">The MessageBox title. </param>
        /// <param name="text">The MessageBox text. </param>
        internal MsgBoxInfo(IntPtr hwnd, string title, string text)
        {
            _hwnd = hwnd;
            _title = title;
            Text = text;
        }

        /// <summary> Returns the MessageBox handle. </summary>
        public IntPtr Hwnd
        {
            get { return _hwnd; }
        }

        /// <summary> Returns the MessageBox title. </summary>
        public string Title
        {
            get { return _title; }
        }

        /// <summary> The text of MessageBox </summary>
        public string Text { get; protected internal set; }
    }

    /// <summary>
    /// The event arguments for event that is raised when the MsgBoxObserver "observes something"
    /// What really happened ( MessageBox creating, activation or destroying)
    /// is specified by EventMsgBoxActionEventArgs.CurrentAction property value.
    /// </summary>
    public class EventMsgBoxActionEventArgs : EventArgs
    {
        /// <summary>
        /// The enum is used as a property of EventMsgBoxActionEventArgs
        /// expressing what is "going on".
        /// </summary>
        public enum MsgBoxAction
        {
            /// <summary>
            /// Used when MessageBox is being created
            /// </summary>
            Creating = 0,

            /// <summary>
            /// Used when MessageBox is being activated
            /// </summary>
            Activating = 1,

            /// <summary>
            /// Used when MessageBox is being destroyed
            /// </summary>
            Destroying = 2,
        }

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="action"> A value describing in more detail the current action of the system.</param>
        /// <param name="info"> An information about related MessageBox</param>
        public EventMsgBoxActionEventArgs(MsgBoxAction action, MsgBoxInfo info)
          : base()
        {
            this.CurrentAction = action;
            this.Info = info;
        }
        /// <summary>
        /// The current action as set by the constructor
        /// </summary>
        public MsgBoxAction CurrentAction { get; protected set; }

        /// <summary>
        /// The MessageBox information, as set by the constructor
        /// </summary>
        public MsgBoxInfo Info { get; protected set; }

        /// <summary> Returns the MessageBox handle. </summary>
        public IntPtr Hwnd { get { return Info.Hwnd; } }
    }
    #endregion // Typedefs

    #region Fields

    /// <summary>
    /// The Dictionary of currently created message boxes ( their window handles mapped to MsgBoxInfo )
    /// </summary>
    private readonly Dictionary<IntPtr, MsgBoxInfo> _dictMsgBoxes = [];

    /// <summary>
    /// Thread safety locker object
    /// </summary>
    private readonly object _locker = new();
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default constructor. Installs a system hook WH_CALLWNDPROCRET, using specific filter function.
    /// </summary>
    /// <param name="install">  (Optional) True to install the hook immediately. </param>
    public MsgBoxObserver(bool install = true)
        : base(Win32.HookType.WH_CALLWNDPROCRET)
    {
        _filterFunc = new User32.HookProc(this.FnCallWndProcHook);
        if (install)
            Install();
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Get count of currently created message boxes 
    /// </summary>
    public int CountOfCreatedMsgBoxes
    {
        get { lock (Locker) { return DictOfMsgBoxes.Count; } }
    }

    /// <summary>
    /// Is there any message box created ?
    /// </summary>
    public bool IsAnyMsgBox
    {
        get { return CountOfCreatedMsgBoxes > 0; }
    }

    /// <summary>
    /// Get the list of currently created message boxes ( their window handles )
    /// </summary>
    protected IDictionary<IntPtr, MsgBoxInfo> DictOfMsgBoxes
    {
        get { return _dictMsgBoxes; }
    }

    /// <summary> Sync object. </summary>
    protected object Locker { get => _locker; }
    #endregion // Properties

    #region Protected Methods

    /// <summary> Adds a new MessageBox to internal dictionary, creating its MsgBoxInfo value. </summary>
    ///
    /// <remarks>
    /// Derived class could overwrite this method in case it wants as as a value in the dictionary use a class
    /// derived from MsgBoxInfo.
    /// </remarks>
    ///
    /// <param name="hWnd"> The MessageBox handle. </param>
    ///
    /// <returns> True if new key has been added, false otherwise. </returns>
    protected virtual bool AddNewMessageBox(IntPtr hWnd)
    {
        bool result;

        lock (Locker)
        {
#if DEBUG
            string strClassName = User32.GetClassName(hWnd);
            Debug.Assert(0 == string.CompareOrdinal(strClassName, Win32.MessageBoxClass), "Must be a dialog");
#endif
            // does not contain that key yet, some new MessageBox is being created
            if (result = !DictOfMsgBoxes.ContainsKey(hWnd))
            {
                string strWndText = User32.GetWindowText(hWnd);
                DictOfMsgBoxes.Add(hWnd, new MsgBoxInfo(hWnd, strWndText));
            }
        }
        return result;
    }

    /// <summary>
    /// Updates the MsgBoxInfo related to given messageBox handle with the text of MessageBox.<br/>
    /// This could be done only when handling its Win32.WM.WM_ACTIVATE message.
    /// </summary>
    /// <param name="hWnd"> The MessageBox handle. </param>
    /// <returns> True if valid handle from the dictionary has been provided, false otherwise. </returns>
    /// <seealso href="http://stackoverflow.com/questions/5978879/how-do-i-read-MessageBox-text-using-winapi">
    /// Stackoverflow: How do I read MessageBox text using WinAPI</seealso>
    protected virtual bool UpdateMessageBoxText(IntPtr hWnd)
    {
        bool result;

        lock (Locker)
        {
#if DEBUG
            string strClassName = User32.GetClassName(hWnd);
            Debug.Assert(0 == string.CompareOrdinal(strClassName,
              Win32.MessageBoxClass), "Must be a dialog");
#endif
            if (result = DictOfMsgBoxes.TryGetValue(hWnd, out MsgBoxInfo info))
            {
                IntPtr hChildChild;
                string strText = string.Empty;
                IntPtr hChild = User32.GetDlgItem(hWnd, 0xFFFF);
                IntPtr hTaskDlgChild = User32.GetDlgItem(hWnd, 0x0000);

                if (hChild != IntPtr.Zero)
                {   // appears a simple MessageBox
                    strText = User32.GetWindowText(hChild);
                }
                else if (hTaskDlgChild != IntPtr.Zero)
                {
                    // appears as TaskDialog
                    hChildChild = User32.RecursiveFind(hTaskDlgChild, HasText);
                    if (hChildChild != IntPtr.Zero)
                    {
                        strText = User32.GetWindowText(hChildChild);
                    }
                }

                info.Text = strText;
            }
        }
        return result;
    }

    /// <summary>
    /// Making an opposite action to <see cref="AddNewMessageBox"/> - removing a MessageBox handle from internal
    /// dictionary,.
    /// </summary>
    ///
    /// <remarks> Derived class could overwrite this method in case any additional functionality needed. </remarks>
    ///
    /// <param name="hWnd"> The MessageBox handle. </param>
    ///
    /// <returns> True if valid handle from the dictionary has been provided, false otherwise. </returns>
    protected virtual bool RemoveMessageBox(IntPtr hWnd)
    {
        lock (Locker)
        {
            return DictOfMsgBoxes.Remove(hWnd);
        }
    }

    /// <summary>
    /// Raises the event evMsgBoxAction with given action value. Delegates the call to the overloaded method.
    /// </summary>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
    ///
    /// <param name="action">   A value describing in more detail the current action of the system. </param>
    /// <param name="hwnd"> A window handle of related MessageBox. </param>
    protected void RaiseEventMsgBoxAction(EventMsgBoxActionEventArgs.MsgBoxAction action, IntPtr hwnd)
    {
        lock (Locker)
        {
            if (DictOfMsgBoxes.TryGetValue(hwnd, out MsgBoxInfo info))
            {
                RaiseObservedEvent(new EventMsgBoxActionEventArgs(action, info));
            }
            else
            {
                string errorMessage = string.Format(CultureInfo.InvariantCulture, "Invalid handle = 0x{0:x8}", hwnd);
                throw new ArgumentException(errorMessage, nameof(hwnd));
            }
        }
    }
    #endregion // Protected Methods

    #region Private Methods

    /// <summary> Implementation helper determining if the window has text, and is not Button. </summary>
    /// <param name="hWnd"> A given window handle. </param>
    /// <returns>   True if yes, false if no. </returns>
    private static bool HasText(IntPtr hWnd)
    {
        string strText;
        string strClassName = User32.GetClassName(hWnd);
        bool result = false;

        if (0 != string.CompareOrdinal(strClassName, Win32.ButtonClass))
        {
            strText = User32.GetWindowText(hWnd);
            if (!string.IsNullOrEmpty(strText))
            {
                // To do - figure out how to get text from SysLink control. See for instance
                // http://msdn.microsoft.com/en-us/library/windows/desktop/bb760724(v=vs.85).aspx
                result = true;
            }
        }

        return result;
    }

    /// <summary> The callback for the system hook WH_CALLWNDPROCRET. </summary>
    ///
    /// <param name="code"> If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROCRET hooks will not receive hook notifications and may behave incorrectly as a result. If the hook procedure does not call CallNextHookEx, the return value should be zero.. </param>
    /// <param name="wParam"> Specifies whether the message is sent by the current process. If the message is sent
    /// by the current process, it is nonzero; otherwise, it is zero. </param>
    /// <param name="lParam">  A pointer to a CWPRETSTRUCT structure that contains details about the message. </param>
    ///
    /// <returns> A result of User32.CallNextHookEx. </returns>
    private IntPtr FnCallWndProcHook(int code, IntPtr wParam, IntPtr lParam)
    {
        IntPtr nResult;
        WindowsSystemHookBase callWndProc_hook = this;

        if ((code < 0) || (code == Win32.HC_NOREMOVE))
        {
            // For these values, must limit itself to just calling next hook in chain
            nResult = callWndProc_hook.CallNextHook(code, wParam, lParam);
        }
        else
        {
            IntPtr hWnd;
            Win32.CWPRETSTRUCT prestruct;

            nResult = callWndProc_hook.CallNextHook(code, wParam, lParam);
            prestruct = (Win32.CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(Win32.CWPRETSTRUCT));
            hWnd = prestruct.hwnd;

            switch (prestruct.message)
            {
                case (int)Win32.WM.WM_CREATE:
                    {
                        string strClassName = User32.GetClassName(hWnd);
                        if (0 == string.CompareOrdinal(strClassName, Win32.MessageBoxClass))
                        {
                            lock (Locker)
                            {
                                if (AddNewMessageBox(hWnd))
                                {
                                    RaiseEventMsgBoxAction(EventMsgBoxActionEventArgs.MsgBoxAction.Creating, hWnd);
                                }
                            }
                        }
                    }
                    break;

                case (int)Win32.WM.WM_ACTIVATE:
                    {
                        // yup, someone is being activated
                        string strClassName = User32.GetClassName(hWnd);
                        if (0 == string.CompareOrdinal(strClassName, Win32.MessageBoxClass))
                        {
                            /* not needed for now
                            IntPtr hWndParent = User32.GetParent(hWnd);
                            if ((hWndParent == IntPtr.Zero) || (hWndParent == User32.GetDesktopWindow()))
                            {   
                            }
                            */
                            lock (Locker)
                            {
                                if (UpdateMessageBoxText(hWnd))
                                {
                                    RaiseEventMsgBoxAction(EventMsgBoxActionEventArgs.MsgBoxAction.Activating, hWnd);
                                }
                            }
                        }
                    }
                    break;

                case (int)Win32.WM.WM_DESTROY:
                    {
                        lock (Locker)
                        {
                            if (DictOfMsgBoxes.ContainsKey(hWnd))
                            {
                                RaiseEventMsgBoxAction(EventMsgBoxActionEventArgs.MsgBoxAction.Destroying, hWnd);
                                RemoveMessageBox(hWnd);
                            }
                        }
                    }
                    break;
            }
        }

        return nResult;
    }
    #endregion Private Methods
}

#pragma warning restore IDE0290 // Use primary constructor
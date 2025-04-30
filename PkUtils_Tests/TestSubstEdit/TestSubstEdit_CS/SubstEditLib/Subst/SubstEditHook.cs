using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using PK.PkUtils.Interfaces;
using PK.PkUtils.MessageHooking;
using PK.PkUtils.UI.Utils;
using PK.PkUtils.Undo;
using PK.PkUtils.Utils;
using PK.PkUtils.WinApi;
using LPARAM = System.IntPtr;
using LRESULT = System.IntPtr;
using tPhysPos = System.Int32;
using WPARAM = System.IntPtr;

// Note: There is an other way to "derive" type from integer - 
// - one could use structures with implicit casts;
// see more info on http://www.codeguru.com/forum/showthread.php?p=1817937

namespace PK.SubstEditLib.Subst
{
    /// <summary>
    /// Selection change event arguments
    /// </summary>
    [CLSCompliant(true)]
    public class SelChagedEventArgs : EventArgs
    {
        protected readonly TextBoxSelInfo _selInfo;

        public SelChagedEventArgs(TextBoxSelInfo selInfo)
        {
            _selInfo = selInfo;
        }

        public TextBoxSelInfo SelectionInfo
        {
            get { return _selInfo; }
        }
    }

    /// <summary>
    /// The hook of the text control or RichText control.
    /// </summary>
    [CLSCompliant(true)]
    public class SubstEditHook<TFIELDID> : WindowMessageHook, IModified, IUndoable, IClipboardable
    {
        #region Typedefs

        /// <summary>
        /// The argument for FindPosOutsidePhys
        /// </summary>
        public enum eFindDirection
        {
            eFindCloser,
            eFindBackward,
            eFindForward,
        };

        /// <summary>
        /// "data storage" maintaining edit control status. Used for undo/redo.
        /// </summary>
        protected class StatusRecord
        {
            protected readonly TextBoxSelInfo _selInfo;
            protected readonly SubstLogData<TFIELDID> _logData;

            protected internal StatusRecord(TextBoxSelInfo selInfo, SubstLogData<TFIELDID> logData)
            {
                _selInfo = selInfo;
                _logData = logData;
            }
            protected internal TextBoxSelInfo SelInfo
            {
                get { return _selInfo; }
            }
            protected internal SubstLogData<TFIELDID> LogData
            {
                get { return _logData; }
            }
#if DEBUG
            public virtual string Say
            {
                get
                {
                    return string.Format(CultureInfo.InvariantCulture,
                        "StatusRecord: (_selInfo={0}, _logData={1})",
                        (_selInfo == null) ? "null" : _selInfo.Say,
                        (_logData == null) ? "null" : _logData.Say);
                }
            }
#endif
        }

        /// <summary>
        /// Specific UndoableEdit used in undo/redo of SubstEditHook.
        /// </summary>
        protected class SubstUndoableEdit : UndoableAbstractEdit
        {
            #region Fields
            protected readonly SubstEditHook<TFIELDID> _hook;
            protected readonly StatusRecord _before;
            protected readonly StatusRecord _after;
            #endregion // Fields

            #region Constructors
            protected internal SubstUndoableEdit(SubstEditHook<TFIELDID> hook)
            {
                _hook = hook;
            }
            protected internal SubstUndoableEdit(SubstEditHook<TFIELDID> hook, StatusRecord before, StatusRecord after)
              : this(hook)
            {
                _before = before;
                _after = after;
            }
            #endregion // Constructors

            #region Properties
            protected internal TextBoxSelInfo SelBefore
            {
                get { return (_before != null) ? _before.SelInfo : null; }
            }

            protected internal TextBoxSelInfo SelAfter
            {
                get { return (_after != null) ? _after.SelInfo : null; }
            }
            #endregion // Properties

            #region Methods
            public override void Undo()
            {
                if (null != _before.LogData)
                {
                    _hook.Assign(_before.LogData);
                }
                if (null != _before.SelInfo)
                {
                    _hook.SetSelInfo(_before.SelInfo);
                }
                base.Undo();
            }
            public override void Redo()
            {
                if (null != _after.LogData)
                {
                    _hook.Assign(_after.LogData);
                }
                if (null != _after.SelInfo)
                {
                    _hook.SetSelInfo(_after.SelInfo);
                }
                base.Redo();
            }
            public override void EmptyUndoBuffer()
            {
            }
#if DEBUG
            public override string Say
            {
                get
                {
                    string strRes = string.Format(
                        CultureInfo.InvariantCulture,
                        "SubstUndoableEdit: (_before={0}, _after={1})",
                        _before.Say,
                        _after.Say);

                    return strRes;
                }
            }
#endif
            #endregion // Methods
        }
        #endregion // Typedefs

        #region Fields

        /// <summary>
        /// The hooked control ( like TextBox, RichTextBox etc.)
        /// Note: The hook SubstEditHook is NOT an owner of that control.
        /// </summary>
        protected TextBoxBase _textBx;

        /// <summary>
        /// The runtime data ( in the beginning constructed from stored SubstLogData )
        /// </summary>
        protected SubstPhysData<TFIELDID> _physData = new();

        /// <summary> The redraw-lock object. </summary>
        private LockRedraw _rLock;

        private UndoManager _undoMgr;
        private StatusRecord _lastStatus;

        /// <summary>
        /// If nonzero, the hook fn just delegates to original functionality.
        /// See IsLockedOrigFn.
        /// </summary>
        private int _nLockHookLevel;

        /// <summary>
        /// Becomes nonzero during CallOrigProc. See also OrigCallLevel.
        /// </summary>
        private int _nOrigCallLevel;

        /// <summary>
        /// The event raised when textbox becomes modified ("dirty")
        /// </summary>
        private ModifiedEventHandler _evModified;

        private EventHandler<SelChagedEventArgs> _evSelChaged;

        // change notification lock
        private int _nChangeNotifyLock;

        // change modification counter
        private int _nChangeModifyTempCount;

        // flag indicating the modification ('dirty') state
        private bool _bModified;

        private int _undoCounter;
        private int _redoCounter;
        #endregion // Fields

        #region Constructor(s)

        public SubstEditHook(TextBoxBase textBx)
          : this(textBx, null)
        {
        }

        public SubstEditHook(TextBoxBase textBx, SubstLogData<TFIELDID> logData)
        {
            _textBx = textBx;
            _undoMgr = new UndoManager();
            base.HookWindow(_textBx.Handle);
            if (null != logData)
            {
                _physData.Assign(logData);
            }
            // subscribe to HandleCreated event, for case if NET recreates the control's handle 
            _textBx.HandleCreated += new EventHandler(OnTextBx_HandleCreated);
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary>
        /// The event raised if selection has been changed. This one is missing in "classic" text box.
        /// </summary>
        public event EventHandler<SelChagedEventArgs> evSelChaged
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add { this._evSelChaged += value; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove { this._evSelChaged -= value; }
        }

        public SubstLogData<TFIELDID> LogData
        {
            get { return _physData; }
        }

        public SubstPhysData<TFIELDID> PhysData
        {
            get { return _physData; }
        }

        protected UndoManager UndoMgr
        {
            get { return _undoMgr; }
        }

        protected StatusRecord LastStatus
        {
            get { return _lastStatus; }
        }

        protected bool IsLockedOrigFn
        {
            get { return _nLockHookLevel > 0; }
        }

        protected int OrigCallLevel
        {
            get { return _nOrigCallLevel; }
        }

        protected int GetUndoCounter
        {
            get { return _undoCounter; }
        }

        protected int GetRedoCounter
        {
            get { return _redoCounter; }
        }
        #endregion // Properties

        #region Methods
        #region Public Methods

        /// <summary>
        /// Nomen est omen
        /// </summary>
        /// <param name="substMap"></param>
        public void AssignSubstMap(IEnumerable<ISubstDescr<TFIELDID>> substMap)
        {
            _physData.AssignSubstMap(substMap);
        }

        /// <summary>
        /// Getting the selection info
        /// </summary>
        /// <returns></returns>
        public TextBoxSelInfo GetSelInfo()
        {
            return TextBoxHelper.GetSelInfo(_textBx);
        }

        /// <summary>
        /// Setting the selection
        /// </summary>
        /// <param name="info"></param>
        public void SetSelInfo(TextBoxSelInfo info)
        {
            if (info.IsAllSelection || !info.IsCaretLast)
            {
                this.CallOrigProc((int)Win32.EM.EM_SETSEL, new IntPtr(info.StartChar), new IntPtr(info.EndChar));
            }
            else
            {
                this.CallOrigProc((int)Win32.EM.EM_SETSEL, new IntPtr(info.EndChar), new IntPtr(info.StartChar));
            }
        }

        /// <summary>
        /// Setting the focus, while preserving the selection info.
        /// Needed since just seting the focus itself sometims selects the whole text.
        /// </summary>
        public void RestoreFocus()
        {
            // store old sel. info
            TextBoxSelInfo info = GetSelInfo();

            // Calling NotifyFixPrologue will prevent saving unnecessary undo info,
            // that would be saved otherwise because of selection changes.
            // At the moment of calling NotifyFixEpilogue here, the selection will be the same as before.
            this.NotifyFixPrologue();
            // focus, sometimes breaks selection
            this._textBx.Focus();
            // restore old sel. info
            this.SetSelInfo(info);
            // unlock again
            this.NotifyFixEpilogue();
        }

        /// <summary>
        /// Initialize the text to physical string in _data.GetPhysStr()
        /// </summary>
        public void InitializeText()
        {
            SetPhysText(PhysData.GetPhysStr);
        }

        /// <summary>
        /// Initialize from contents of rhs
        /// </summary>
        /// <param name="rhs"></param>
        public void Assign(SubstLogData<TFIELDID> rhs)
        {
            PhysData.Assign(rhs);
            InitializeText();
        }

        /// <summary>
        /// Initialize from plain text
        /// </summary>
        /// <param name="strPlain"></param>
        public void AssignPlainText(string strPlain)
        {
            PhysData.AssignPlainText(strPlain);
            InitializeText();
        }

        /// <summary>
        /// Nomen est omen
        /// </summary>
        /// <returns></returns>
        public string GetPlainText()
        {
            return PhysData.GetPlainText();
        }

        public void DeleteContents()
        {
            PhysData.DeleteContents();
            InitializeText();
        }

        /// <summary>
        /// From given line and column determine the physical position ( char index )
        /// </summary>
        /// <param name="line"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public int LineCol2CharPos(int line, int col)
        {
            int suma;

            LockHookFn();
            suma = _textBx.GetFirstCharIndexFromLine(line);
            suma += col;
            UnlockHookFn();

            return suma;
        }

        public tPhysPos FindPosOutsidePhys(tPhysPos iorig, eFindDirection direction)
        {
            PhysInfo<TFIELDID> lpPhys;
            int res = iorig;

            if (null != (lpPhys = PhysData.FindPhysInfoPosIsIn(iorig)))
            {
                int delta_a = iorig - lpPhys.GetStart;
                int delta_b = lpPhys.GetEnd - iorig;

                Debug.Assert((delta_a > 0) && (delta_b > 0));
                if ((direction == eFindDirection.eFindBackward) || (direction == eFindDirection.eFindCloser && (delta_a <= delta_b)))
                    res = lpPhys.GetStart;
                else
                    res = lpPhys.GetEnd;
            }
            return res;
        }

        /// <summary>
        /// Insert new field on the caret position
        /// </summary>
        /// <param name="what"></param>
        /// <returns></returns>
        public bool InsertNewInfo(TFIELDID what)
        {
            Debug.Assert(this.IsHooked);

            int phpos;
            PhysInfo<TFIELDID> lpPh;
            bool bUndoMgrListening = (_undoMgr.Status == UndoManager.MgrStatus.Listening);
            TextBoxSelInfo selInf = GetSelInfo();
            bool res = false;

            NotifyFixPrologue();
            if (bUndoMgrListening && (1 == ChangeNotifyLock))
            {
                OpenUndoInfo();
            }

            using (var user = new UsageMonitor(_rLock))
            {
                if (null != (lpPh = PhysData.InsertNewInfo(selInf.CaretChar, what)))
                {
                    phpos = lpPh.GetEnd;
                    SetPhysText(PhysData.GetPhysStr);
                    SetSelInfo(new TextBoxSelInfo(phpos));
                    CallOrigProc((int)Win32.EM.EM_SCROLLCARET, IntPtr.Zero, IntPtr.Zero);
                    ChangeModifyTempCountIncrement(); //  make sure EN_CHANGE is send by NotifyFixEpilogue()
                    res = true;
                }
            }

            if (bUndoMgrListening && (1 == ChangeNotifyLock))
            {
                CloseUndoInfo();
            }
            NotifyFixEpilogue();

            return res;
        }

#if DEBUG
        public virtual string Say
        {
            get
            {
                StringBuilder sb = new();

                sb.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "SubstEditHook: (_undoMgr={0})",
                    (null == this._undoMgr) ? "null" : _undoMgr.Say);
                return sb.ToString();
            }
        }
#endif
        #endregion // Public Methods

        #region Protected Methods

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"> If true, is called by IDisposable.Dispose. 
        /// Otherwise it is called by finalizer.</param>
        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_undoMgr")]
        protected override void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.IsDisposed)
            {
                // If disposing equals true, dispose both managed and unmanaged resources.
                if (disposing)
                {
                    _textBx.HandleCreated -= OnTextBx_HandleCreated;
                    Disposer.SafeDispose(ref _undoMgr);
                }
                // Now release unmanaged resources. Actually nothing to do here.

                // Call the base class implementation
                base.Dispose(disposing);
            }
        }
        #region General helpers

        /// <summary>
        /// Get the hooked control
        /// </summary>
        /// <returns></returns>
        protected internal TextBoxBase GetTextBox()
        {
            return _textBx;
        }

        protected void LockHookFn()
        {
            _nLockHookLevel++;
        }

        protected void UnlockHookFn()
        {
            _nLockHookLevel--;
        }

        protected override IntPtr CallOrigProc(ref Message m)
        {
            IntPtr result;
            Debug.Assert(OrigCallLevel >= 0);
            _nOrigCallLevel++;
            result = base.CallOrigProc(ref m);
            _nOrigCallLevel--;
            Debug.Assert(OrigCallLevel >= 0);
            return result;
        }

        protected void SetPhysText(string strTxt)
        {
            LockHookFn();
            this._textBx.Text = strTxt;
            UnlockHookFn();
        }

        [Conditional("Debug")]
        protected void AssertSelValidity(TextBoxSelInfo sel)
        {
            Debug.Assert(null == PhysData.FindPhysInfoPosIsIn(sel.StartChar));
            if (sel.IsSel)
            {
                if (sel.IsAllSelection)
                { // anything to test?
                }
                else
                {
                    Debug.Assert(null == PhysData.FindPhysInfoPosIsIn(sel.EndChar));
                }
            }
        }
        #endregion // General helpers

        #region Undo_redo helpers

        /// <summary>
        /// Getting the data and selection info ( as a new created object )
        /// </summary>
        /// <returns></returns>
        protected StatusRecord GetStatusRecord()
        {
            SubstLogData<TFIELDID> temp = new();
            this._physData.ExportLogAll(temp);
            return new StatusRecord(this.GetSelInfo(), temp);
        }

        /// <summary>
        /// Setting the data and selection info
        /// </summary>
        /// <param name="info"></param>
        protected void SetStatusRecord(StatusRecord record)
        {
            if (null != record.LogData)
            {
                this.Assign(record.LogData);
            }
            if (null != record.SelInfo)
            {
                this.SetSelInfo(record.SelInfo);
            }
        }

        protected void EmptyEditCtrlUndoBuffer()
        {
            LockHookFn();
            this.CallOrigProc((int)Win32.EM.EM_EMPTYUNDOBUFFER, IntPtr.Zero, IntPtr.Zero);
            UnlockHookFn();
        }

        protected void OpenUndoInfo()
        {
            if (null == LastStatus)
            {
                _lastStatus = this.GetStatusRecord();
            }
            else
            {   // nothing to do so far
            }
        }

        protected bool CloseUndoInfo()
        {
            bool bRes = false;

            if (null != LastStatus)
            {
                StatusRecord newStatus = this.GetStatusRecord();
                bool bSelEqual = LastStatus.SelInfo.Equals(newStatus.SelInfo);
                bool bDataEqual = LastStatus.LogData.Equals(newStatus.LogData);

                if (bSelEqual && bDataEqual)
                {
                    // so far nothing to do
                }
                else
                {
                    StatusRecord before = new(
                        bSelEqual ? null : LastStatus.SelInfo,
                        bDataEqual ? null : LastStatus.LogData);
                    StatusRecord after = new(
                        bSelEqual ? null : newStatus.SelInfo,
                        bDataEqual ? null : newStatus.LogData);
                    SubstUndoableEdit edit = new(this, before, after);
                    // add the edit to undo buffer
                    _undoMgr.AddEdit(edit);
                    // cleanup the last status to indicate it should be recorded again
                    _lastStatus = null;
                    bRes = true;
                }
            }
            return bRes;
        }

        protected void IncUndoCounter()
        {
            _undoCounter = (_undoCounter < int.MaxValue) ? (_undoCounter + 1) : 0;
            /* more simple if the type of it is uint ( not cls-compliant)
            unchecked { _undoCounter++; } */
        }
        protected void IncRedoCounter()
        {
            _redoCounter = (_redoCounter < int.MaxValue) ? (_redoCounter + 1) : 0;
            /* more simple if the type of it is uint ( not cls-compliant)
            unchecked { _redoCounter++; } */
        }
        #endregion // Undo_redo helpers

        #region Hooking Fn

        /// <summary>
        /// Auxiliary helper called from HookWindow. 
        /// </summary>
        /// <param name="pExtraInfo"></param>
        protected override void OnHookup(Object pExtraInfo)
        {
            base.OnHookup(pExtraInfo);
            _textBx.TextChanged += new EventHandler(_textBx_TextChanged);
            _rLock = new LockRedraw(this.HookedHWND, false);
        }

        /// <summary>
        /// Auxiliary helper called from HookWindow. 
        /// </summary>
        /// <param name="pExtraInfo"></param>
        protected override void OnUnhook(Object pExtraInfo)
        {
            _textBx.TextChanged -= _textBx_TextChanged;
            _rLock = null;
            base.OnUnhook(pExtraInfo);
        }

        /// <summary>
        /// internal hooking wndproc fn
        /// </summary>
        /// <param name="m"></param>
        protected override void HookWindowProc(ref Message m)
        {
            TextBoxSelInfo selChanged = null;
            SubstUndoableEdit subst;
            bool bHasUndoRecord = false;

            // 1. prologue
            NotifyFixPrologue();

            // 2. message processing
            if (IsLockedOrigFn)
            {   //just process the message 
                base.HookWindowProc(ref m);
            }
            else
            {
                IntPtr lRes = IntPtr.Zero;
                bool bUndoRedoCmd = false;
                bool bUndoCmd = ((m.Msg == (int)Win32.WM.WM_UNDO) || (m.Msg == (int)Win32.EM.EM_UNDO));
                bool bRedoCmd = (m.Msg == (int)Win32.RichEm.EM_REDO);
                bool bUndoMgrListening = (_undoMgr.Status == UndoManager.MgrStatus.Listening);
                bool bHandled = true;

                bUndoRedoCmd = bUndoCmd || bRedoCmd;
                if (!bUndoRedoCmd && bUndoMgrListening && (1 == ChangeNotifyLock))
                {
                    OpenUndoInfo();
                }

                switch (m.Msg)
                {
                    case (int)Win32.WM.WM_KEYDOWN:
                        {
                            switch ((Win32.VK)m.WParam.ToInt32())
                            {
                                case Win32.VK.VK_LEFT:
                                case Win32.VK.VK_RIGHT:
                                case Win32.VK.VK_HOME:
                                case Win32.VK.VK_END:
                                    lRes = this.MyMoveCaretHorizontal(m.WParam, m.LParam);
                                    break;

                                case Win32.VK.VK_UP:
                                case Win32.VK.VK_DOWN:
                                    lRes = this.MyMoveCaretVertical(m.WParam, m.LParam);
                                    break;

                                case Win32.VK.VK_DELETE:
                                    lRes = this.MyOnVk_Delete(m.LParam);
                                    break;
                                /* Following is now done through ProcessCmdKey in SubstEditTextBoxCtrl
                                case 0x59: // Redo ( Ctrl + Y )
                                    if (Control.ModifierKeys == Keys.Control)
                                        User32.PostMessage(this.HookedHWND, Win32.EM_REDO, IntPtr.Zero, IntPtr.Zero);
                                    else
                                        bHandled = false;
                                    break;
                                */
                                default:
                                    bHandled = false;
                                    break;
                            }
                        }
                        break;

                    case (int)Win32.WM.WM_CHAR:
                        lRes = this.MyOnWmChar(m.WParam, m.LParam, ref bHandled);
                        break;

                    case (int)Win32.WM.WM_LBUTTONDOWN:
                        lRes = this.MyOnWmLButtonDown(m.WParam, m.LParam);
                        break;

                    case (int)Win32.WM.WM_MOUSEMOVE:
                        lRes = this.MyOnWmMouseMove(m.WParam, m.LParam);
                        break;

                    case (int)Win32.WM.WM_LBUTTONDBLCLK:
                        /* lRes = 0; already is */
                        break;

                    case (int)Win32.WM.WM_CUT:
                        lRes = this.MyCopy(true);
                        break;

                    case (int)Win32.WM.WM_COPY:
                        lRes = this.MyCopy(false);
                        break;

                    case (int)Win32.WM.WM_PASTE:
                        lRes = this.MyOnPaste();
                        break;

                    case (int)Win32.WM.WM_UNDO:
                    case (int)Win32.EM.EM_UNDO:
                        if (this.CanUndo)
                        {
                            this.Undo();
                        }
                        break;

                    case (int)Win32.RichEm.EM_REDO:
                        if (this.CanRedo)
                        {
                            this.Redo();
                        }
                        break;

                    default:
                        bHandled = false;
                        break;
                }

                // process the message 
                if (bHandled)
                {
                    m.Result = lRes;
                }
                else
                {
                    base.HookWindowProc(ref m);
                }

                if (!bUndoRedoCmd && bUndoMgrListening && (1 == ChangeNotifyLock))
                {
                    bHasUndoRecord = CloseUndoInfo();
                    if (bHasUndoRecord && (null != (subst = UndoMgr.LastEdit as SubstUndoableEdit)))
                    {
                        selChanged = subst.SelAfter;
                    }
                }
                if (bUndoRedoCmd)
                {   // for undo or redo get selChanged always, for simplicity
                    selChanged = this.GetSelInfo();
                }
            }

            // 3. epilogue
            // Ensures that EN_CHANGE is sent only after all changes are truly completed
            NotifyFixEpilogue();
            // send the selection change event
            if (null != selChanged)
            {
                this.RaiseSelChaged(selChanged);
            }
        }
        #endregion // Hooking Fn

        #region Text Changed

        /// <summary>
        /// The lock depth
        /// </summary>
        protected int ChangeNotifyLock
        {
            get
            {
                Debug.Assert(_nChangeNotifyLock >= 0);
                return _nChangeNotifyLock;
            }
        }

        protected bool IsChangeNotifyLocked()
        {
            return (0 < ChangeNotifyLock);
        }

        protected void NotifyFixPrologue()
        {
            Debug.Assert(_nChangeNotifyLock >= 0);
            if (0 == _nChangeNotifyLock++)
            {
                ChangeModifyTempCountReset();
            }
        }

        protected void NotifyFixEpilogue()
        {
            if (0 == --_nChangeNotifyLock)
            {
                if (0 < _nChangeModifyTempCount)
                {
                    ChangeModifyTempCountReset();
                    OnModified();
                }
            }
            Debug.Assert(_nChangeNotifyLock >= 0);
        }

        private void ChangeModifyTempCountReset()
        {
            this._nChangeModifyTempCount = 0;
        }

        private void ChangeModifyTempCountIncrement()
        {
            this._nChangeModifyTempCount++;
        }

        protected void _textBx_TextChanged(object sender, EventArgs e)
        {
            if (IsChangeNotifyLocked())
            {
                ChangeModifyTempCountIncrement();
            }
            else
            {
                OnModified();
            }
        }

        protected void RaiseSelChaged(TextBoxSelInfo sel)
        {
            if ((null != _evSelChaged) && (null != sel))
            {
                _evSelChaged(this, new SelChagedEventArgs(sel));
            }
        }

        protected void RaiseModified()
        {
            if (null != _evModified)
            {
                _evModified(this, new EventArgs());
            }
        }

        protected virtual void OnModified()
        {
            this.SetModified();
        }
        #endregion // Text Changed
        #endregion // Protected Methods

        #region Private Methods
        #region Special handlers

        /// <summary>
        /// The handler of HandleCreated event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextBx_HandleCreated(object sender, EventArgs e)
        {
            Debug.Assert(null != _textBx);
            if (!IsHooked)
            {
                base.HookWindow(_textBx.Handle);
            }
        }

        /// <summary>
        /// Helper method called by HookWindowProc.
        /// </summary>
        /// <param name="selInf"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private IntPtr DeleteSel_WmCharBack(
            TextBoxSelInfo selInf,
            LPARAM lParam)
        {
            LRESULT lRes;

            Debug.Assert(selInf.IsSel);
            PhysData.DeleteAllBetween(selInf.StartChar, selInf.EndChar);
            lRes = CallOrigProc((int)Win32.WM.WM_CHAR, (IntPtr)Win32.VK.VK_BACK, lParam);
            EmptyEditCtrlUndoBuffer();
            return lRes;
        }

        /// <summary>
        /// Helper method called by HookWindowProc.
        /// </summary>
        /// <param name="selInf"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private LRESULT DeleteSel_VKDelete(
            TextBoxSelInfo selInf,
            LPARAM lParam)
        {
            LRESULT lRes;

            Debug.Assert(selInf.IsSel);
            PhysData.DeleteAllBetween(selInf.StartChar, selInf.EndChar);
            lRes = CallOrigProc((int)Win32.WM.WM_KEYDOWN, (IntPtr)Win32.VK.VK_DELETE, lParam);
            EmptyEditCtrlUndoBuffer();
            return lRes;
        }

        /// <summary>
        /// Helper method called by HookWindowProc.
        /// </summary>
        /// <param name="selInf"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private LRESULT DeleteSel_WmCharStrange(
            TextBoxSelInfo selInf,
            WPARAM wParam,
            LPARAM lParam)
        {
            string strOld, strNew;
            LRESULT lRes;

            Debug.Assert(selInf.IsSel);
            strOld = this._textBx.Text;
            lRes = CallOrigProc((int)Win32.WM.WM_CHAR, wParam, lParam);
            strNew = this._textBx.Text;
            if (strOld != strNew)
            {
                PhysData.DeleteAllBetween(selInf.StartChar, selInf.EndChar);
                EmptyEditCtrlUndoBuffer();
                Debug.Assert(strNew == PhysData.GetPhysStr);
            }
            return lRes;
        }

        /// <summary>
        /// Helper method called by HookWindowProc.
        /// </summary>
        /// <param name="selInf"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private LRESULT BackspaceDeleteNotSel(
            TextBoxSelInfo selInf,
            LPARAM lParam)
        {
            PhysInfo<TFIELDID> lpPhys;
            int iCaret, iStart;
            LRESULT lRes = IntPtr.Zero;

            Debug.Assert(!selInf.IsSel);
            if ((iCaret = selInf.CaretChar) > 0)
            {
                if ((null != (lpPhys = PhysData.FindPhysInfoBefore(iCaret))) && (lpPhys.GetEnd == iCaret))
                {
                    iStart = lpPhys.GetStart;
                }
                else
                {
                    string strLeft = PhysData.GetPhysStr.Substring(0, iCaret);

                    if ((strLeft.Length >= 2) && (0 == string.Compare(strLeft.Substring(strLeft.Length - 2, 2), "\r\n", StringComparison.Ordinal)))
                        iStart = iCaret - 2;
                    else
                        iStart = iCaret - 1;
                }
                PhysData.DeleteAllBetween(iStart, iCaret);
                for (; ; )
                {
                    lRes = CallOrigProc((int)Win32.WM.WM_CHAR, (IntPtr)Win32.VK.VK_BACK, lParam);
                    if (GetSelInfo().StartChar == iStart)
                        break;
                }
                EmptyEditCtrlUndoBuffer();
            }
            else
            {
                lRes = CallOrigProc((int)Win32.WM.WM_CHAR, (IntPtr)Win32.VK.VK_BACK, lParam);
                EmptyEditCtrlUndoBuffer();
            }
#if DEBUG
            string strTmp = this._textBx.Text;
            Debug.Assert(strTmp == PhysData.GetPhysStr);
#endif

            return lRes;
        }

        /// <summary>
        /// Helper method called by HookWindowProc.
        /// </summary>
        /// <param name="selInf"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private LRESULT VkDeleteNotSel(
            TextBoxSelInfo selInf,
            LPARAM lParam)
        {
            byte[] pbKeyState = new byte[256];
            string strTmp, strRight;
            PhysInfo<TFIELDID> lpPhys;
            int iCaret, iEnd, nDelLimit;
            LRESULT lRes = IntPtr.Zero;

            Debug.Assert(!selInf.IsSel);
            strTmp = this._textBx.Text;
            Debug.Assert(strTmp == PhysData.GetPhysStr);
            if ((iCaret = selInf.CaretChar) < strTmp.Length)
            {
                if ((null != (lpPhys = PhysData.FindPhysInfoAfter(iCaret))) && (lpPhys.GetStart == iCaret))
                {
                    nDelLimit = iEnd = lpPhys.GetEnd;
                }
                else
                {
                    strRight = strTmp.Substring(iCaret, strTmp.Length - iCaret);
                    if ((strRight.Length >= 2) && (0 == string.Compare(strRight.Substring(0, 2), "\r\n", StringComparison.Ordinal)))
                        iEnd = iCaret + 2;
                    else
                        iEnd = iCaret + 1;
                    nDelLimit = iCaret + 1;
                }
                PhysData.DeleteAllBetween(iCaret, iEnd);

                User32.GetKeyboardState(pbKeyState);
                pbKeyState[(int)Win32.VK.VK_CONTROL] &= 0x7F;
                User32.SetKeyboardState(pbKeyState);
                for (int ii = iCaret; ii < nDelLimit; ii++)
                {
                    lRes = CallOrigProc((int)Win32.WM.WM_KEYDOWN, (IntPtr)Win32.VK.VK_DELETE, lParam);
                }
                EmptyEditCtrlUndoBuffer();
#if DEBUG
                strTmp = this._textBx.Text;
                Debug.Assert(strTmp == PhysData.GetPhysStr);
#endif
            }
            else
            {
                lRes = CallOrigProc((int)Win32.WM.WM_KEYDOWN, (IntPtr)Win32.VK.VK_DELETE, lParam);
#if DEBUG
                strTmp = this._textBx.Text;
                Debug.Assert(strTmp == PhysData.GetPhysStr);
#endif
            }

            return lRes;
        }

        /// <summary>
        /// Helper method called by HookWindowProc.
        /// </summary>
        /// <param name="selInf"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private LRESULT WmCharDoInsetChar(
            TextBoxSelInfo selInf,
            WPARAM wParam,
            LPARAM lParam)
        {
            string strNew, strOld;
            int iCaret = selInf.CaretChar;
            int oldUndoCounter = this.GetUndoCounter;
            int oldRedoCounter = this.GetRedoCounter;
            LRESULT lRes = IntPtr.Zero;

#if DEBUG
            Debug.Assert(!selInf.IsSel);
            strOld = this._textBx.Text;
            Debug.Assert(strOld == PhysData.GetPhysStr);
#else
      strOld = PhysData.GetPhysStr;
#endif
            lRes = CallOrigProc((int)Win32.WM.WM_CHAR, wParam, lParam);

            if (oldUndoCounter != this.GetUndoCounter || oldRedoCounter != this.GetRedoCounter)
            {   // it was not "regular" char insertion at all, but a shortcut to undo or redo
                EmptyEditCtrlUndoBuffer();
            }
            else
            {
                strNew = this._textBx.Text;
                if (ModifyDataOnInsertion(iCaret, strOld, strNew))
                {
                    EmptyEditCtrlUndoBuffer();
                }
            }

            return lRes;
        }

        /// <summary>
        /// Helper method called by HookWindowProc.
        /// </summary>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private LRESULT MyMoveCaretHorizontal(WPARAM wParam, LPARAM lParam)
        {
            byte[] pbKeyState = new byte[256];
            LRESULT lRes = IntPtr.Zero;

            Debug.Assert((wParam.ToInt32() == (int)Win32.VK.VK_LEFT) ||
                (wParam.ToInt32() == (int)Win32.VK.VK_RIGHT) ||
                (wParam.ToInt32() == (int)Win32.VK.VK_HOME) ||
                (wParam.ToInt32() == (int)Win32.VK.VK_END));
            User32.GetKeyboardState(pbKeyState);
            pbKeyState[(int)Win32.VK.VK_CONTROL] &= 0x7F;
            User32.SetKeyboardState(pbKeyState);

            lRes = CallOrigProc((int)Win32.WM.WM_KEYDOWN, wParam, lParam);
            while (null != PhysData.FindPhysInfoPosIsIn(GetSelInfo().CaretChar))
            {   // move caret this way to keep shift-selecting if shift is pressed
                CallOrigProc((int)Win32.WM.WM_KEYDOWN, wParam, lParam);
            }

            return lRes;
        }

        private LRESULT MyMoveCaretVertical(WPARAM wParam, LPARAM lParam)
        {
            byte[] pbKeyState = new byte[256];
            tPhysPos tPosCurrent;
            LRESULT lRes = IntPtr.Zero;

            Debug.Assert((wParam.ToInt32() == (int)Win32.VK.VK_UP) ||
                (wParam.ToInt32() == (int)Win32.VK.VK_DOWN));
            User32.GetKeyboardState(pbKeyState);
            pbKeyState[(int)Win32.VK.VK_CONTROL] &= 0x7F;
            User32.SetKeyboardState(pbKeyState);

            lRes = CallOrigProc((int)Win32.WM.WM_KEYDOWN, wParam, lParam);
            if (null != PhysData.FindPhysInfoPosIsIn(tPosCurrent = GetSelInfo().CaretChar))
            {
                eFindDirection direction = (wParam.ToInt32() == (int)Win32.VK.VK_UP) ? eFindDirection.eFindBackward : eFindDirection.eFindForward;
                WPARAM wkSubstitute = (wParam.ToInt32() == (int)Win32.VK.VK_UP) ? (IntPtr)Win32.VK.VK_LEFT : (IntPtr)Win32.VK.VK_RIGHT;
                tPhysPos tPosGoal = FindPosOutsidePhys(tPosCurrent, direction);

                while (tPosCurrent != tPosGoal)
                {   // move caret this way to keep shift-selecting if shift is pressed
                    CallOrigProc((int)Win32.WM.WM_KEYDOWN, wkSubstitute, lParam);
                    tPosCurrent = GetSelInfo().CaretChar;
                }
            }

            return lRes;
        }

        /// <summary>
        /// Helper method called by HookWindowProc.
        /// </summary>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="bHandled"></param>
        /// <returns></returns>
        private LRESULT MyOnWmChar(WPARAM wParam, LPARAM lParam, ref bool bHandled)
        {
            TextBoxSelInfo newSel;
            TextBoxSelInfo oldSel = GetSelInfo();
            LRESULT lRes = IntPtr.Zero;

            AssertSelValidity(oldSel);
            bHandled = false;
            switch ((Win32.VK)wParam.ToInt32())
            {
                case Win32.VK.VK_FINAL: // Ctrl+X; let the default processing to transform it to command Win32.WM.WM_CUT
                case Win32.VK.VK_CANCEL: // Ctrl+C; let the default processing to transform it to command Win32.WM.WM_COPY
                    /* bHandled = false; already is */
                    break;

                case Win32.VK.VK_LBUTTON: // Ctrl+A
                    SetSelInfo(TextBoxSelInfo.AllSelection);
                    bHandled = true;
                    break;

                case Win32.VK.VK_BACK: // VK_DELETE is not called as WM_CHAR at all
                    if (oldSel.IsSel)
                        lRes = DeleteSel_WmCharBack(oldSel, lParam);
                    else
                        lRes = BackspaceDeleteNotSel(oldSel, lParam);
                    bHandled = true;
                    break;

                default:
                    if (oldSel.IsSel)
                    {
                        if ((wParam.ToInt32() < (int)Win32.VK.VK_SPACE) && (wParam.ToInt32() != (int)Win32.VK.VK_RETURN))
                        {
                            lRes = DeleteSel_WmCharStrange(oldSel, wParam, lParam);
                        }
                        else
                        {
                            lRes = DeleteSel_WmCharBack(oldSel, lParam);
                            newSel = GetSelInfo();
                            lRes = WmCharDoInsetChar(newSel, wParam, lParam);
                        }
                    }
                    else
                    {
                        lRes = WmCharDoInsetChar(oldSel, wParam, lParam);
                    }
                    bHandled = true;
                    break;
            }

            return lRes;
        }

        /// <summary>
        /// Helper method called by HookWindowProc.
        /// </summary>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private LRESULT MyOnVk_Delete(LPARAM lParam)
        {
            TextBoxSelInfo oldSel = GetSelInfo();
            LRESULT lRes = IntPtr.Zero;

            AssertSelValidity(oldSel);
            if (oldSel.IsSel)
                lRes = DeleteSel_VKDelete(oldSel, lParam);
            else
                lRes = VkDeleteNotSel(oldSel, lParam);

            return lRes;
        }

        /// <summary>
        /// Helper method called by HookWindowProc.
        /// </summary>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private LRESULT MyOnWmLButtonDown(WPARAM wParam, LPARAM lParam)
        {
            /* uint fwKeys = wParam; */
            Point pt = new(Win32.LOWORD(lParam.ToInt32()), Win32.HIWORD(lParam.ToInt32()));
            int iround;
            Point pttmp;
            int nAllLength = PhysData.GetPhysStr.Length;
            int charPos = _textBx.GetCharIndexFromPositionFix(pt);
            int istrPos = LineCol2CharPos(Win32.HIWORD(charPos), Win32.LOWORD(charPos));
            LRESULT lRes = IntPtr.Zero;

            if ((0 <= istrPos) && (istrPos <= nAllLength))
            {
                iround = FindPosOutsidePhys(istrPos, eFindDirection.eFindCloser);
                if (iround != istrPos)
                {
                    pttmp = this._textBx.GetPositionFromCharIndexFix(iround);
                    lParam = Win32.MAKELPARAM((ushort)pttmp.X, (ushort)pttmp.Y);
                }
                lRes = CallOrigProc((int)Win32.WM.WM_LBUTTONDOWN, wParam, lParam);
            }

            return lRes;
        }

        /// <summary>
        /// Helper method called by HookWindowProc.
        /// </summary>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private LRESULT MyOnWmMouseMove(WPARAM wParam, LPARAM lParam)
        {
            uint fwKeys = unchecked((uint)wParam.ToInt32());
            Point pt = new(Win32.LOWORD(lParam.ToInt32()), Win32.HIWORD(lParam.ToInt32()));
            LRESULT lRes = IntPtr.Zero;

            if (0 != (fwKeys & Win32.MK_LBUTTON))
            {
                Point pttmp;
                int iround;
                int istrPos;
                int charPos = _textBx.GetCharIndexFromPositionFix(pt);

                if (0 > (istrPos = LineCol2CharPos(Win32.HIWORD(charPos), Win32.LOWORD(charPos))))
                {
                    return IntPtr.Zero;
                }
                if ((iround = FindPosOutsidePhys(istrPos, eFindDirection.eFindCloser)) != istrPos)
                {
                    pttmp = this._textBx.GetPositionFromCharIndexFix(iround);
                    lParam = Win32.MAKELPARAM((ushort)pttmp.X, (ushort)pttmp.Y);
                }
            }
            lRes = CallOrigProc((int)Win32.WM.WM_MOUSEMOVE, wParam, lParam);

            return lRes;
        }

        /// <summary>
        /// Helper method called by HookWindowProc.
        /// </summary>
        /// <param name="iPos"></param>
        /// <param name="szOldText"></param>
        /// <param name="szNewText"></param>
        /// <returns></returns>
        private bool ModifyDataOnInsertion(
            int iPos,
            string strOldText,
            string strNewText)
        {
            int ioldLen, irold;
            string strTmp;
            int delta = 0;

            if (strOldText != strNewText)
            {
                delta = strNewText.Length - (ioldLen = strOldText.Length);
                Debug.Assert(delta > 0);
                irold = ioldLen - iPos;
                Debug.Assert(strOldText.Substring(0, iPos) == strNewText.Substring(0, iPos));
                /*
                Debug.Assert(strOldText.Right(irold) == strNewText.Right(irold));
                */
                strTmp = strNewText.Substring(iPos, delta);
                PhysData.InsertText(iPos, strTmp);
                Debug.Assert(PhysData.GetPhysStr == strNewText);
            }
            return (delta != 0);
        }

        /// <summary>
        /// Exports the selected contents as a plain text
        /// and puts the resulting plain text on the clipboard.
        /// </summary>
        /// <param name="bCut"></param>
        /// <returns></returns>
        private LRESULT MyCopy(bool bCut)
        {
            TextBoxSelInfo selInf = this.GetSelInfo();
            LRESULT lRes = IntPtr.Zero;

            if (selInf.IsSel)
            {
                SubstLogData<TFIELDID> tempData = new();
                PhysData.ExportLogSel(selInf, tempData);
                Clipboard.SetText(tempData.GetPlainText());
                if (bCut)
                {
                    this.MyOnVk_Delete(1);
                }
                lRes = new IntPtr(1);
            }
            return lRes;
        }

        /// <summary>
        /// Pastes the clipboard contents as a plain text, 
        /// converts the plain text to field list and logical string,
        /// and inserts the result on current selection position.
        /// </summary>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private LRESULT MyOnPaste()
        {
            string strPaste = Clipboard.GetText();

            if (!string.IsNullOrEmpty(strPaste))
            {
                var tempData = new SubstLogData<TFIELDID>(this.PhysData.GetSubstMap);
                TextBoxSelInfo currentSel = GetSelInfo();
                AssertSelValidity(currentSel);

                // 1. parse pasted text; result is in tempData
                tempData.AssignPlainText(strPaste);
                // 2. delete selection if there is any
                if (currentSel.IsSel)
                {
                    DeleteSel_WmCharBack(currentSel, 0x000e0001);
                    currentSel = GetSelInfo();
                    Debug.Assert(!currentSel.IsSel);
                }
                currentSel.Offset(PhysData.InsertData(currentSel.StartChar, tempData));
                InitializeText();
                SetSelInfo(currentSel);
                EmptyEditCtrlUndoBuffer();
            }

            return IntPtr.Zero;
        }
        #endregion // Special handlers
        #endregion // Private Methods
        #endregion // Methods

        #region IModified Members

        public event ModifiedEventHandler evModified
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add { _evModified += value; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove { _evModified -= value; }
        }

        public bool IsModified
        {
            get
            {
                return _bModified;
            }
        }

        /// <summary>
        /// Nomen est omen
        /// </summary>
        /// <param name="bValue"></param>
        public void SetModified(bool bValue)
        {
            if (this._bModified = bValue)
            {
                RaiseModified();
            }
        }
        #endregion // IModified Members

        #region IUndoable Members

        public bool CanUndo
        {
            get { return ((null != _undoMgr) && _undoMgr.CanUndo); }
        }

        public bool CanRedo
        {
            get { return ((null != _undoMgr) && _undoMgr.CanRedo); }
        }

        public void Undo()
        {
            if (!CanUndo)
            {
                throw new InvalidOperationException();
            }
            _undoMgr.Undo();
            this._lastStatus = null;
            IncUndoCounter();
        }

        public void Redo()
        {
            if (!CanRedo)
            {
                throw new InvalidOperationException();
            }
            _undoMgr.Redo();
            this._lastStatus = null;
            IncRedoCounter();
        }

        /// <summary>
        /// Nomen est omen
        /// </summary>
        public void EmptyUndoBuffer()
        {
            this._undoMgr.EmptyUndoBuffer();
            this._lastStatus = null;
        }
        #endregion // IUndoable Members

        #region IClipboardable Members

        public bool CanCut
        {
            get { return IsHooked && GetSelInfo().IsSel; }
        }

        public bool CanCopy
        {
            get { return IsHooked && GetSelInfo().IsSel; }
        }

        public bool CanPaste
        {
            get { return IsHooked && Clipboard.ContainsText(); }
        }

        public void Cut()
        {
            this._textBx.Cut();
        }

        public void Copy()
        {
            this._textBx.Copy();
        }

        public void Paste()
        {
            this._textBx.Paste();
        }
        #endregion // IClipboardable Members
    }
}

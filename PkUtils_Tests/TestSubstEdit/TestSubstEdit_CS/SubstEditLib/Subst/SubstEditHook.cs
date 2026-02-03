// Ignore Spelling: keydown lbutton dblclk mousemove subst phys log fn txt bx sel info mgr epilogue prologue usr phpos lpph txtboxbaselogdata txtboxselinfo undoableedit emscrollcaret emsetsel ememptyundobuffer vk end home ctrl substedittextboxctrl
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
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


namespace PK.SubstEditLib.Subst;

#pragma warning disable IDE0057  // Use range operator
#pragma warning disable IDE0290  // Use primary constructor

/// <summary>
/// Selection change event arguments containing selection information.
/// </summary>
[CLSCompliant(true)]
public class SelChangedEventArgs : EventArgs
{
    /// <summary>
    /// Backing field for selection info.
    /// </summary>
    protected readonly TextBoxSelInfo _selInfo;

    /// <summary>
    /// Initializes a new instance of <see cref="SelChangedEventArgs"/> with the provided selection info.
    /// </summary>
    /// <param name="selInfo">Selection information to include in the event.</param>
    public SelChangedEventArgs(TextBoxSelInfo selInfo)
    {
        _selInfo = selInfo;
    }

    /// <summary>
    /// Gets the selection information carried by this event.
    /// </summary>
    public TextBoxSelInfo SelectionInfo
    {
        get { return _selInfo; }
    }
}

/// <summary>
/// The hook implementation for a text control (TextBoxBase / RichTextBox etc.) that manages
/// substitution-aware editing, undo/redo and clipboard operations.
/// </summary>
[CLSCompliant(true)]
public class SubstEditHook<TFIELDID> : WindowMessageHook, IModified, IUndoable, IClipboardable
{
    #region Typedefs

    /// <summary>
    /// Direction used by FindPosOutsidePhys to determine a closest outside-of-field position.
    /// </summary>
    protected enum FindDirection
    {
        /// <summary>
        /// Finds the closest position outside of a field.
        /// </summary>
        eFindCloser,

        /// <summary>
        /// Finds the nearest position in the backward direction outside of a field.
        /// </summary>
        eFindBackward,

        /// <summary>
        /// Finds the nearest position in the forward direction outside of a field.
        /// </summary>
        eFindForward,
    };

    /// <summary>
    /// "Data storage" maintaining edit control status. Used for undo/redo.
    /// </summary>
    protected class StatusRecord
    {
        /// <summary>
        /// Selection info snapshot.
        /// </summary>
        protected readonly TextBoxSelInfo _selInfo;
        /// <summary>
        /// Logical substitution data snapshot.
        /// </summary>
        protected readonly SubstLogData<TFIELDID> _logData;

        /// <summary>
        /// Initializes a new instance of <see cref="StatusRecord"/> with selection info and log data.
        /// </summary>
        /// <param name="selInfo">Selection information snapshot.</param>
        /// <param name="logData">Logical substitution data snapshot.</param>
        protected internal StatusRecord(TextBoxSelInfo selInfo, SubstLogData<TFIELDID> logData)
        {
            _selInfo = selInfo;
            _logData = logData;
        }

        /// <summary>
        /// Returns a diagnostic string describing the record.
        /// </summary>
        public virtual string Say
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture,
                    "StatusRecord: (_selInfo={0}, _logData={1})",
                    (_selInfo == null) ? "null" : _selInfo.ToString(),
                    (_logData == null) ? "null" : _logData.Say);
            }
        }

        /// <summary>
        /// Gets the stored selection information.
        /// </summary>
        protected internal TextBoxSelInfo SelInfo
        {
            get { return _selInfo; }
        }

        /// <summary>
        /// Gets the stored logical substitution data.
        /// </summary>
        protected internal SubstLogData<TFIELDID> LogData
        {
            get { return _logData; }
        }
    }

    /// <summary>
    /// Specific UndoableEdit used in undo/redo of SubstEditHook.
    /// </summary>
    protected class SubstUndoableEdit : UndoableAbstractEdit
    {
        #region Fields
        /// <summary>
        /// Reference to the hook that created this edit.
        /// </summary>
        protected readonly SubstEditHook<TFIELDID> _hook;
        /// <summary>
        /// Snapshot before the operation.
        /// </summary>
        protected readonly StatusRecord _before;
        /// <summary>
        /// Snapshot after the operation.
        /// </summary>
        protected readonly StatusRecord _after;
        #endregion // Fields

        #region Constructors
        /// <summary>
        /// Initializes a new instance of <see cref="SubstUndoableEdit"/> bound to the given hook.
        /// </summary>
        /// <param name="hook">Owning hook instance.</param>
        protected internal SubstUndoableEdit(SubstEditHook<TFIELDID> hook)
        {
            _hook = hook;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SubstUndoableEdit"/> with before/after snapshots.
        /// </summary>
        /// <param name="hook">Owning hook instance.</param>
        /// <param name="before">Snapshot before change.</param>
        /// <param name="after">Snapshot after change.</param>
        protected internal SubstUndoableEdit(SubstEditHook<TFIELDID> hook, StatusRecord before, StatusRecord after)
          : this(hook)
        {
            _before = before;
            _after = after;
        }
        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Diagnostic representation of the edit.
        /// </summary>
        public override string Say
        {
            get
            {
                string strRes = $"SubstUndoableEdit: (_before={_before.Say}, _after={_after.Say})";
                return strRes;
            }
        }


        /// <summary>
        /// Gets the selection info before the edit (may be null).
        /// </summary>
        protected internal TextBoxSelInfo SelBefore
        {
            get { return _before?.SelInfo; }
        }

        /// <summary>
        /// Gets the selection info after the edit (may be null).
        /// </summary>
        protected internal TextBoxSelInfo SelAfter
        {
            get { return _after?.SelInfo; }
        }
        #endregion // Properties

        #region Methods
        /// <summary>
        /// Performs the undo operation by restoring the 'before' snapshot.
        /// </summary>
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

        /// <summary>
        /// Performs the redo operation by restoring the 'after' snapshot.
        /// </summary>
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

        /// <summary>
        /// Clears any sub-undo buffers held by this edit. Not used in this implementation.
        /// </summary>
        public override void EmptyUndoBuffer()
        {
        }
        #endregion // Methods
    }
    #endregion // Typedefs

    #region Fields

    /// <summary>
    /// The hooked control (like TextBox, RichTextBox etc.). This hook does not own the control.
    /// </summary>
    protected TextBoxBase _textBx;

    /// <summary>
    /// The runtime physical data constructed from stored SubstLogData.
    /// </summary>
    protected SubstPhysData<TFIELDID> _physData = new();

    /// <summary> The redraw-lock object used while updating the control. </summary>
    private LockRedraw _rLock;

    private UndoManager _undoMgr;
    private StatusRecord _lastStatus;

    /// <summary>
    /// Lock counter: when nonzero the hook delegates to original functionality.
    /// </summary>
    private int _nLockHookLevel;

    /// <summary>
    /// Call level counter used during CallOrigProc.
    /// </summary>
    private int _nOrigCallLevel;

    /// <summary>
    /// The event raised when textbox becomes modified ("dirty").
    /// </summary>
    private ModifiedEventHandler _evModified;

    private EventHandler<SelChangedEventArgs> _evSelChanged;

    // change notification lock
    private int _nChangeNotifyLock;

    // change modification counter
    private int _nChangeModifyTempCount;

    // flag indicating the modification ('dirty') state
    private bool _isModified;

    private int _undoCounter;
    private int _redoCounter;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Initializes a new instance of <see cref="SubstEditHook{TFIELDID}"/> for the given text box.
    /// </summary>
    /// <param name="textBx">The TextBoxBase to hook.</param>
    public SubstEditHook(TextBoxBase textBx)
      : this(textBx, null)
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="SubstEditHook{TFIELDID}"/> for the given text box and initial log data.
    /// </summary>
    /// <param name="textBx">The TextBoxBase to hook.</param>
    /// <param name="logData">Optional initial logical substitution data.</param>
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
    /// Event raised when the selection changes. Subscribers receive <see cref="SelChangedEventArgs"/>.
    /// </summary>
    public event EventHandler<SelChangedEventArgs> EventSelChanged
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        add { this._evSelChanged += value; }
        [MethodImpl(MethodImplOptions.Synchronized)]
        remove { this._evSelChanged -= value; }
    }

    /// <summary>
    /// Gets the logical substitution data representation (log form).
    /// </summary>
    public SubstLogData<TFIELDID> LogData
    {
        get { return _physData; }
    }

    /// <summary>
    /// Gets the physical substitution data used for editing.
    /// </summary>
    public SubstPhysData<TFIELDID> PhysData
    {
        get { return _physData; }
    }

    /// <summary>
    /// Diagnostic string describing this hook instance.
    /// </summary>
    public virtual string Say
    {
        get
        {
            string managerSay = (null == this._undoMgr) ? "null" : _undoMgr.Say;
            string physDataSay = (null == this._physData) ? "null" : _physData.Say;
            string result = string.Format(
                CultureInfo.InvariantCulture,
                "SubstEditHook: (_undoMgr={0}, _physData={1})",
                managerSay,
                physDataSay);

            return result;
        }
    }

    /// <summary>
    /// Gets the underlying UndoManager (protected access).
    /// </summary>
    protected UndoManager UndoMgr { get => _undoMgr; }

    /// <summary>
    /// Gets the last status record currently stored (protected access).
    /// </summary>
    protected StatusRecord LastStatus
    {
        get => _lastStatus;
    }

    /// <summary>
    /// Indicates whether the hook is locked to call original functions.
    /// </summary>
    protected bool IsLockedOrigFn
    {
        get => _nLockHookLevel > 0;
    }

    /// <summary>
    /// Gets the current orig-call nesting level (protected access).
    /// </summary>
    protected int OrigCallLevel
    {
        get => _nOrigCallLevel;
    }

    /// <summary>
    /// Gets the internal undo counter (protected access).
    /// </summary>
    protected int UndoCounter
    {
        get => _undoCounter;
    }

    /// <summary>
    /// Gets the internal redo counter (protected access).
    /// </summary>
    protected int RedoCounter
    {
        get => _redoCounter;
    }

    /// <summary>
    /// Assigns a substitution map to the physical data.
    /// </summary>
    /// <param name="substMap">The substitution descriptors to set.</param>
    public void SetSubstitutionMap(IEnumerable<ISubstitutionDescriptor<TFIELDID>> substMap)
    {
        _physData.SetSubstitutionMap(substMap);
    }

    /// <summary>
    /// Gets the selection information from the text box.
    /// </summary>
    /// <returns>The current selection information.</returns>
    public TextBoxSelInfo GetSelInfo()
    {
        return TextBoxExtensions.GetSelInfo(_textBx);
    }

    /// <summary>
    /// Sets the selection in the hooked text box using given selection info.
    /// </summary>
    /// <param name="info">Selection information to apply.</param>
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
    /// Restores focus to the text box while preserving selection information.
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
    /// Initializes the control text to the physical representation from <see cref="PhysData"/>.
    /// </summary>
    public void InitializeText()
    {
        SetPhysText(PhysData.GetPhysStr);
    }

    /// <summary>
    /// Assigns the contents from another <see cref="SubstLogData{TFIELDID}"/> instance.
    /// </summary>
    /// <param name="rhs">Source logical data to assign.</param>
    public void Assign(SubstLogData<TFIELDID> rhs)
    {
        PhysData.Assign(rhs);
        InitializeText();
    }

    /// <summary>
    /// Assigns the contents from plain text.
    /// </summary>
    /// <param name="strPlain">Plain text to parse and assign.</param>
    public void AssignPlainText(string strPlain)
    {
        PhysData.AssignPlainText(strPlain);
        InitializeText();
    }

    /// <summary>
    /// Gets the plain text representation (logical text without fields).
    /// </summary>
    /// <returns>The plain/logical text.</returns>
    public string GetPlainText()
    {
        return PhysData.GetPlainText();
    }

    /// <summary>
    /// Deletes all contents of the physical data and updates the control text.
    /// </summary>
    public void DeleteContents()
    {
        PhysData.DeleteContents();
        InitializeText();
    }

    /// <summary>
    /// Converts a line and column pair to a character position in the control.
    /// </summary>
    /// <param name="line">Zero-based line index.</param>
    /// <param name="col">Zero-based column index.</param>
    /// <returns>Character index corresponding to line and column.</returns>
    public int LineCol2CharPos(int line, int col)
    {
        int suma;

        LockHookFn();
        suma = _textBx.GetFirstCharIndexFromLine(line);
        suma += col;
        UnlockHookFn();

        return suma;
    }

    /// <summary>
    /// Finds the nearest position outside a physical field for a given physical position.
    /// </summary>
    /// <param name="iorig">Original physical position.</param>
    /// <param name="direction">Direction hint to pick left/right boundary.</param>
    /// <returns>Position outside of any field close to input.</returns>
    protected tPhysPos FindPosOutsidePhys(tPhysPos iorig, FindDirection direction)
    {
        PhysInfo<TFIELDID> lpPhys;
        int res = iorig;

        if (null != (lpPhys = PhysData.FindPhysInfoPosIsIn(iorig)))
        {
            int delta_a = iorig - lpPhys.GetStart;
            int delta_b = lpPhys.GetEnd - iorig;

            Debug.Assert((delta_a > 0) && (delta_b > 0));
            if ((direction == FindDirection.eFindBackward) || (direction == FindDirection.eFindCloser && (delta_a <= delta_b)))
                res = lpPhys.GetStart;
            else
                res = lpPhys.GetEnd;
        }
        return res;
    }

    /// <summary>
    /// Inserts a new substitution field at the current caret position.
    /// </summary>
    /// <param name="what">Field identifier to insert.</param>
    /// <returns>True if insertion occurred; otherwise false.</returns>
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
    #endregion // Public Methods

    #region Methods
    #region Protected Methods

    /// <summary>
    /// Releases resources used by this hook.
    /// </summary>
    /// <param name="disposing">True when called from Dispose, false when called from the finalizer.</param>
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
    /// Gets the hooked TextBoxBase control (protected access).
    /// </summary>
    /// <returns>The hooked TextBoxBase.</returns>
    protected internal TextBoxBase GetTextBox()
    {
        return _textBx;
    }

    /// <summary>
    /// Increments hook lock counter to delegate handling to original window procedure.
    /// </summary>
    protected void LockHookFn()
    {
        _nLockHookLevel++;
    }

    /// <summary>
    /// Decrements hook lock counter to restore hook processing.
    /// </summary>
    protected void UnlockHookFn()
    {
        _nLockHookLevel--;
    }

    /// <summary>
    /// Calls the original window procedure while tracking original call level.
    /// </summary>
    /// <param name="m">Windows message to forward.</param>
    /// <returns>Result returned by the original procedure.</returns>
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

    /// <summary>
    /// Sets the control text to the provided physical string while preventing reentrant hook processing.
    /// </summary>
    /// <param name="strTxt">The physical string to set in the control.</param>
    protected void SetPhysText(string strTxt)
    {
        LockHookFn();
        this._textBx.Text = strTxt;
        UnlockHookFn();
    }

    /// <summary>
    /// Asserts that selection boundaries are not within physical fields (debug only).
    /// </summary>
    /// <param name="sel">Selection information to validate.</param>
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
    /// Creates and returns a snapshot of current selection and data for undo/redo.
    /// </summary>
    /// <returns>A new <see cref="StatusRecord"/> representing current state.</returns>
    protected StatusRecord GetStatusRecord()
    {
        SubstLogData<TFIELDID> temp = new();
        this._physData.ExportLogAll(temp);
        return new StatusRecord(this.GetSelInfo(), temp);
    }

    /// <summary>
    /// Restores state from a provided status record (data and selection).
    /// </summary>
    /// <param name="record">The status record to apply.</param>
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

    /// <summary>
    /// Empties the internal edit control undo buffer by forwarding EM_EMPTYUNDOBUFFER to the control.
    /// </summary>
    protected void EmptyEditCtrlUndoBuffer()
    {
        LockHookFn();
        this.CallOrigProc((int)Win32.EM.EM_EMPTYUNDOBUFFER, IntPtr.Zero, IntPtr.Zero);
        UnlockHookFn();
    }

    /// <summary>
    /// Starts collecting an undo snapshot if none recorded yet.
    /// </summary>
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

    /// <summary>
    /// Closes an undo snapshot and adds an undoable edit if something changed.
    /// </summary>
    /// <returns>True if an undo record was created; otherwise false.</returns>
    protected bool CloseUndoInfo()
    {
        bool result = false;

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
                result = true;
            }
        }
        return result;
    }

    /// <summary>
    /// Increments the internal undo counter (protected access).
    /// </summary>
    protected void IncUndoCounter()
    {
        _undoCounter = (_undoCounter < int.MaxValue) ? (_undoCounter + 1) : 0;
        /* more simple if the type of it is uint ( not cls-compliant)
        unchecked { _undoCounter++; } */
    }

    /// <summary>
    /// Increments the internal redo counter (protected access).
    /// </summary>
    protected void IncRedoCounter()
    {
        _redoCounter = (_redoCounter < int.MaxValue) ? (_redoCounter + 1) : 0;
        /* more simple if the type of it is uint ( not cls-compliant)
        unchecked { _redoCounter++; } */
    }
    #endregion // Undo_redo helpers

    #region Hooking Fn

    /// <summary>
    /// Called when the hook is attached to a window. Subscribes to TextChanged and prepares redraw lock.
    /// </summary>
    /// <param name="pExtraInfo">Optional extra info passed from HookWindow.</param>
    protected override void OnHookup(Object pExtraInfo)
    {
        base.OnHookup(pExtraInfo);
        _textBx.TextChanged += new EventHandler(OnTextBx_TextChanged);
        _rLock = new LockRedraw(this.HookedHWND, false);
    }

    /// <summary>
    /// Called when the hook is detached. Unsubscribes events and clears redraw lock.
    /// </summary>
    /// <param name="pExtraInfo">Optional extra info passed from Unhook.</param>
    protected override void OnUnhook(Object pExtraInfo)
    {
        _textBx.TextChanged -= OnTextBx_TextChanged;
        _rLock = null;
        base.OnUnhook(pExtraInfo);
    }

    /// <summary>
    /// Processes window messages for the hooked control. This is the core message handler.
    /// </summary>
    /// <param name="m">Reference to the Windows message to process.</param>
    protected override void HookWindowProc(ref Message m)
    {
        TextBoxSelInfo selChanged = null;
        bool bHasUndoRecord;

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
            bool bUndoCmd = ((m.Msg == (int)Win32.WM.WM_UNDO) || (m.Msg == (int)Win32.EM.EM_UNDO));
            bool bRedoCmd = (m.Msg == (int)Win32.RichEm.EM_REDO);
            bool bUndoRedoCmd = bUndoCmd || bRedoCmd;
            bool bUndoMgrListening = (_undoMgr.Status == UndoManager.MgrStatus.Listening);
            bool bHandled = true;

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
                if (bHasUndoRecord && (UndoMgr.LastEdit is SubstUndoableEdit subst))
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
            this.RaiseSelChanged(selChanged);
        }
    }
    #endregion // Hooking Fn

    #region Text Changed

    /// <summary>
    /// Gets the current change notification lock depth (protected access).
    /// </summary>
    protected int ChangeNotifyLock
    {
        get
        {
            Debug.Assert(_nChangeNotifyLock >= 0);
            return _nChangeNotifyLock;
        }
    }

    /// <summary>
    /// Indicates whether change notifications are currently locked.
    /// </summary>
    /// <returns>True when locked; otherwise false.</returns>
    protected bool IsChangeNotifyLocked()
    {
        return (0 < ChangeNotifyLock);
    }

    /// <summary>
    /// Prologue called before a sequence of changes to suppress immediate change events.
    /// </summary>
    protected void NotifyFixPrologue()
    {
        Debug.Assert(_nChangeNotifyLock >= 0);
        if (0 == _nChangeNotifyLock++)
        {
            ChangeModifyTempCountReset();
        }
    }

    /// <summary>
    /// Epilogue called after a sequence of changes to re-enable change notifications and fire modified events if needed.
    /// </summary>
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

    /// <summary>
    /// Increments the temporary modification counter used to decide whether to raise modified events.
    /// </summary>
    protected void ChangeModifyTempCountIncrement()
    {
        this._nChangeModifyTempCount++;
    }

    /// <summary>
    /// Handles control TextChanged event: either counts modifications or raises modified immediately.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="args">Event arguments.</param>
    protected void OnTextBx_TextChanged(object sender, EventArgs args)
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

    /// <summary>
    /// Raises the selection changed event with provided selection.
    /// </summary>
    /// <param name="sel">Selection info to include in event.</param>
    protected void RaiseSelChanged(TextBoxSelInfo sel)
    {
        if ((null != _evSelChanged) && (null != sel))
        {
            _evSelChanged(this, new SelChangedEventArgs(sel));
        }
    }

    /// <summary>
    /// Raises the modified event to subscribers.
    /// </summary>
    protected void RaiseModified()
    {
        _evModified?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when the control becomes modified. Default implementation sets IsModified flag.
    /// </summary>
    protected virtual void OnModified()
    {
        this.SetModified();
    }
    #endregion // Text Changed
    #endregion // Protected Methods

    #region Private Methods
    #region Special handlers

    private static void SuppressControlKey()
    {
        byte[] state = new byte[256];
        User32.GetKeyboardState(state);

        // Unset CTRL key
        state[(int)Win32.VK.VK_CONTROL] &= 0x7F;

        User32.SetKeyboardState(state);
    }

    /// <summary>
    /// Handler for TextBox.HandleCreated: reattaches the hook if needed.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="args">Event arguments.</param>
    private void OnTextBx_HandleCreated(object sender, EventArgs args)
    {
        Debug.Assert(null != _textBx);
        if (!IsHooked)
        {
            base.HookWindow(_textBx.Handle);
        }
    }

    /// <summary>
    /// Deletes the currently selected region responding to WM_CHAR backspace semantics.
    /// </summary>
    /// <param name="selInf">Selection information describing what to delete.</param>
    /// <param name="lParam">Original lParam for the WM_CHAR call.</param>
    /// <returns>Result of forwarded WM_CHAR backspace call.</returns>
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
    /// Deletes the currently selected region responding to VK_DELETE semantics.
    /// </summary>
    /// <param name="selInf">Selection information describing what to delete.</param>
    /// <param name="lParam">Original lParam for the VK call.</param>
    /// <returns>Result of forwarded VK_DELETE keydown call.</returns>
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
    /// Handles WM_CHAR input that is neither backspace nor printable by forwarding it to the default window proc 
    /// and syncing PhysData if the control text changes.
    /// </summary>
    /// <param name="selInf">Selection info.</param>
    /// <param name="wParam">Character code.</param>
    /// <param name="lParam">lParam from message.</param>
    /// <returns>Result of forwarded WM_CHAR.</returns>
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
    /// Handles backspace when there is no selection, with special handling for fields and CRLF.
    /// </summary>
    /// <param name="selInf">Current selection info (caret position).</param>
    /// <param name="lParam">Original lParam value.</param>
    /// <returns>Result of forwarded WM_CHAR backspace operations.</returns>
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
    /// Handles VK_DELETE when there is no selection, with special handling for fields and CRLF.
    /// </summary>
    /// <param name="selInf">Current selection info.</param>
    /// <param name="lParam">Original lParam value.</param>
    /// <returns>Result of forwarded keydown delete calls.</returns>
    private LRESULT VkDeleteNotSel(
        TextBoxSelInfo selInf,
        LPARAM lParam)
    {
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
            SuppressControlKey();
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
    /// Handles character insertion (WM_CHAR) and updates the physical data accordingly.
    /// </summary>
    /// <param name="selInf">Selection/caret position before insertion.</param>
    /// <param name="wParam">Character code.</param>
    /// <param name="lParam">Original lParam value.</param>
    /// <returns>Result of forwarded WM_CHAR message.</returns>
    private LRESULT WmCharDoInsetChar(
        TextBoxSelInfo selInf,
        WPARAM wParam,
        LPARAM lParam)
    {
        string strNew, strOld;
        int iCaret = selInf.CaretChar;
        int oldUndoCounter = this.UndoCounter;
        int oldRedoCounter = this.RedoCounter;
        LRESULT lRes = IntPtr.Zero;

#if DEBUG
        Debug.Assert(!selInf.IsSel);
        strOld = this._textBx.Text;
        Debug.Assert(strOld == PhysData.GetPhysStr);
#else
        strOld = PhysData.GetPhysStr;
#endif
        lRes = CallOrigProc((int)Win32.WM.WM_CHAR, wParam, lParam);

        if (oldUndoCounter != this.UndoCounter || oldRedoCounter != this.RedoCounter)
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
    /// Moves caret horizontally while skipping internal field ranges.
    /// </summary>
    /// <param name="wParam">Virtual key parameter.</param>
    /// <param name="lParam">Original lParam.</param>
    /// <returns>Result of forwarded keydown.</returns>
    private LRESULT MyMoveCaretHorizontal(WPARAM wParam, LPARAM lParam)
    {
        Debug.Assert((wParam.ToInt32() == (int)Win32.VK.VK_LEFT) ||
            (wParam.ToInt32() == (int)Win32.VK.VK_RIGHT) ||
            (wParam.ToInt32() == (int)Win32.VK.VK_HOME) ||
            (wParam.ToInt32() == (int)Win32.VK.VK_END));
        SuppressControlKey();

        LRESULT lRes = CallOrigProc((int)Win32.WM.WM_KEYDOWN, wParam, lParam);
        while (null != PhysData.FindPhysInfoPosIsIn(GetSelInfo().CaretChar))
        {   // move caret this way to keep shift-selecting if shift is pressed
            CallOrigProc((int)Win32.WM.WM_KEYDOWN, wParam, lParam);
        }

        return lRes;
    }

    /// <summary>
    /// Moves caret vertically while skipping internal field ranges.
    /// </summary>
    /// <param name="wParam">Virtual key parameter.</param>
    /// <param name="lParam">Original lParam.</param>
    /// <returns>Result of forwarded keydown.</returns>
    private LRESULT MyMoveCaretVertical(WPARAM wParam, LPARAM lParam)
    {
        tPhysPos tPosCurrent;
        LRESULT lRes;

        Debug.Assert((wParam.ToInt32() == (int)Win32.VK.VK_UP) ||
            (wParam.ToInt32() == (int)Win32.VK.VK_DOWN));
        SuppressControlKey();

        lRes = CallOrigProc((int)Win32.WM.WM_KEYDOWN, wParam, lParam);
        if (null != PhysData.FindPhysInfoPosIsIn(tPosCurrent = GetSelInfo().CaretChar))
        {
            FindDirection direction = (wParam.ToInt32() == (int)Win32.VK.VK_UP) ? FindDirection.eFindBackward : FindDirection.eFindForward;
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
    /// Handles WM_CHAR message processing for the hook, including clipboard/selection shortcuts and backspace handling.
    /// </summary>
    /// <param name="wParam">Character code passed in the message.</param>
    /// <param name="lParam">Original lParam from the message.</param>
    /// <param name="bHandled">Reference parameter to indicate whether message was handled.</param>
    /// <returns>Result to be placed into message.Result if handled.</returns>
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
                        DeleteSel_WmCharBack(oldSel, lParam);
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
    /// Handles VK_DELETE (key down) behavior for the hook.
    /// </summary>
    /// <param name="lParam">Original lParam value passed with the key event.</param>
    /// <returns>Result of deletion handling.</returns>
    private LRESULT MyOnVk_Delete(LPARAM lParam)
    {
        TextBoxSelInfo oldSel = GetSelInfo();
        LRESULT lRes;

        AssertSelValidity(oldSel);
        if (oldSel.IsSel)
            lRes = DeleteSel_VKDelete(oldSel, lParam);
        else
            lRes = VkDeleteNotSel(oldSel, lParam);

        return lRes;
    }

    /// <summary>
    /// Handles WM_LBUTTONDOWN by snapping clicks outside of fields to nearest allowed position.
    /// </summary>
    /// <param name="wParam">wParam from message.</param>
    /// <param name="lParam">lParam from message.</param>
    /// <returns>Result of forwarded WM_LBUTTONDOWN processing.</returns>
    private LRESULT MyOnWmLButtonDown(WPARAM wParam, LPARAM lParam)
    {
        Point pt = Win32.GetPointFromLParam(lParam.ToInt32());
        int iround;
        Point pttmp;
        int nAllLength = PhysData.GetPhysStr.Length;
        int charPos = _textBx.GetCharIndexFromPositionFix(pt);
        int istrPos = LineCol2CharPos(Win32.HIWORD(charPos), Win32.LOWORD(charPos));
        LRESULT lRes = IntPtr.Zero;

        if ((0 <= istrPos) && (istrPos <= nAllLength))
        {
            iround = FindPosOutsidePhys(istrPos, FindDirection.eFindCloser);
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
    /// Handles WM_MOUSEMOVE during selection to keep selection outside fields.
    /// </summary>
    /// <param name="wParam">wParam from message.</param>
    /// <param name="lParam">lParam from message.</param>
    /// <returns>Result of forwarded WM_MOUSEMOVE processing.</returns>
    private LRESULT MyOnWmMouseMove(WPARAM wParam, LPARAM lParam)
    {
        uint fwKeys = unchecked((uint)wParam.ToInt32());
        Point pt = new(Win32.LOWORD(lParam.ToInt32()), Win32.HIWORD(lParam.ToInt32()));
        LRESULT lRes;

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
            if ((iround = FindPosOutsidePhys(istrPos, FindDirection.eFindCloser)) != istrPos)
            {
                pttmp = this._textBx.GetPositionFromCharIndexFix(iround);
                lParam = Win32.MAKELPARAM((ushort)pttmp.X, (ushort)pttmp.Y);
            }
        }
        lRes = CallOrigProc((int)Win32.WM.WM_MOUSEMOVE, wParam, lParam);

        return lRes;
    }

    /// <summary>
    /// Adjusts internal substitution data after a text insertion.
    /// </summary>
    /// <param name="iPos">Position where insertion occurred.</param>
    /// <param name="strOldText">Old text before insertion.</param>
    /// <param name="strNewText">New text after insertion.</param>
    /// <returns>True if the physical data was modified.</returns>
    private bool ModifyDataOnInsertion(
        int iPos,
        string strOldText,
        string strNewText)
    {
        string strTmp;
        int delta = 0;

        if (strOldText != strNewText)
        {
            int oldLength = strOldText.Length;

            delta = strNewText.Length - oldLength;
            Debug.Assert(delta > 0);
            Debug.Assert(strOldText.Substring(0, iPos) == strNewText.Substring(0, iPos));

            strTmp = strNewText.Substring(iPos, delta);
            PhysData.InsertText(iPos, strTmp);
            Debug.Assert(PhysData.GetPhysStr == strNewText);
        }

        return (delta != 0);
    }

    /// <summary>
    /// Exports the selected contents as plain text and places it on the clipboard, optionally cutting.
    /// </summary>
    /// <param name="bCut">True to cut (remove) selection after copying; false to copy only.</param>
    /// <returns>Non-zero IntPtr on success; zero otherwise.</returns>
    private LRESULT MyCopy(bool bCut)
    {
        TextBoxSelInfo selInf = this.GetSelInfo();
        LRESULT lRes = IntPtr.Zero;

        if (selInf.IsSel)
        {
            SubstLogData<TFIELDID> tempData = new();
            PhysData.ExportLogSel(selInf, tempData);
            string plainText = tempData.GetPlainText();

            Clipboard.SetText(plainText);
            if (bCut)
            {
                this.MyOnVk_Delete(1);
            }
            lRes = new IntPtr(1);
        }
        return lRes;
    }

    /// <summary>
    /// Pastes plain text from the clipboard, parses it into substitution data and inserts it into the model.
    /// </summary>
    /// <returns>Zero IntPtr (unused).</returns>
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
            currentSel = currentSel.WithOffset(PhysData.InsertData(currentSel.StartChar, tempData));
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

    /// <summary>
    /// Event triggered when the modified state changes.
    /// </summary>
    public event ModifiedEventHandler EventModified
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        add { _evModified += value; }
        [MethodImpl(MethodImplOptions.Synchronized)]
        remove { _evModified -= value; }
    }

    /// <summary>
    /// Gets a value indicating whether the content has been modified.
    /// </summary>
    public bool IsModified { get => _isModified; }

    /// <summary>
    /// Sets the modified flag and raises Modified event when true.
    /// </summary>
    /// <param name="bValue">New modified value.</param>
    public void SetModified(bool bValue)
    {
        if (this._isModified = bValue)
        {
            RaiseModified();
        }
    }
    /// <summary>
    /// Gets a value indicating whether an undo operation can be performed.
    /// </summary>
    public bool CanUndo
    {
        get { return ((null != _undoMgr) && _undoMgr.CanUndo); }
    }

    /// <summary>
    /// Gets a value indicating whether a redo operation can be performed.
    /// </summary>
    public bool CanRedo
    {
        get { return ((null != _undoMgr) && _undoMgr.CanRedo); }
    }

    /// <summary>
    /// Performs an undo operation via the internal undo manager.
    /// </summary>
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

    /// <summary>
    /// Performs a redo operation via the internal undo manager.
    /// </summary>
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
    /// Empties the undo buffer and clears last status snapshot.
    /// </summary>
    public void EmptyUndoBuffer()
    {
        this._undoMgr.EmptyUndoBuffer();
        this._lastStatus = null;
    }
    /// <summary>
    /// Gets a value indicating whether a cut operation can be performed.
    /// </summary>
    public bool CanCut
    {
        get { return IsHooked && GetSelInfo().IsSel; }
    }

    /// <summary>
    /// Gets a value indicating whether a copy operation can be performed.
    /// </summary>
    public bool CanCopy
    {
        get { return IsHooked && GetSelInfo().IsSel; }
    }

    /// <summary>
    /// Gets a value indicating whether a paste operation can be performed.
    /// </summary>
    public bool CanPaste
    {
        get { return IsHooked && Clipboard.ContainsText(); }
    }

    /// <summary>
    /// Cuts the selected text to the clipboard using the control's Cut.
    /// </summary>
    public void Cut()
    {
        this._textBx.Cut();
    }

    /// <summary>
    /// Copies the selected text to the clipboard using the control's Copy.
    /// </summary>
    public void Copy()
    {
        this._textBx.Copy();
    }

    /// <summary>
    /// Pastes clipboard contents into the text box using the control's Paste.
    /// </summary>
    public void Paste()
    {
        this._textBx.Paste();
    }
    #endregion // IClipboardable Members
}
#pragma warning restore IDE0290
#pragma warning restore IDE0057
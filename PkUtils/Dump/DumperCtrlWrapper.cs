// Ignore Spelling: CCA, CTRL, Preprocess, Utils
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.Dump;

/// <summary> The wrapper around a general WinForms control, providing the IDumper-behavior for that control.
/// Internally, it keeps the buffer ( Queue ) of recently added items, so the control could display
/// a recent history. The queue maximum length is provided as an input argument of constructor. </summary>
///
/// <typeparam name="CTRL"> Type of the WinForms control. </typeparam>
[CLSCompliant(true)]
public class DumperCtrlWrapper<CTRL> : IDumperEx, IDisposableEx where CTRL : System.Windows.Forms.Control
{
    #region Typedefs

    /// <summary>   Argument-less delegate that is used in implementation of <see cref="Reset"/>. </summary>
    /// <returns>   true if it succeeds, false if it fails. </returns>
    protected delegate bool ResetMethodInvoker();

    /// <summary>	A delegate  that is used in implementation of <see cref="DumpText"/>. </summary>
    /// <param name="strAdd">	The new added text item. </param>
    /// <returns>	true if it succeeds, false if it fails. </returns>
    protected delegate bool MethodAddStringInvoker(string strAdd);

    /// <summary>
    /// The enum indicating what type of change has been done in virtual bool AddText(string strAdd)
    /// </summary>
    public enum AddTextResult
    {
        /// <summary>
        /// Nothing has been added ( for instance the control has no window handle created yet )
        /// </summary>
        AddNone = 0,

        /// <summary>
        /// The text has been appended to the previous text in the control, with preserving all previous text
        /// </summary>
        AppendedOnly = 1,

        /// <summary>
        /// The text has been appended to the previous text in the control, with removing some part of previous text
        /// </summary>
        RemovedAndAppended = 2,
    }

    /// <summary>   Values that represent message levels. </summary>
    protected enum MessageLevel
    {
        /// <summary> An enum constant representing the Information option. </summary>
        Info,
        /// <summary> An enum constant representing the warning option. </summary>
        Warning,
        /// <summary> An enum constant representing the error option. </summary>
        Error
    }

    /// <summary>   (Immutable) record representing log message. </summary>
    protected record LogEntry
    {
        /// <summary>   Gets the textual content of the log entry. </summary>
        public string Text { get; }

        /// <summary>   Gets the severity level of the log message. </summary>
        public MessageLevel Level { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> record with the specified text and message level.
        /// </summary>
        /// <param name="text">The textual content of the log entry. Cannot be <c>null</c>.</param>
        /// <param name="level">The severity level of the log message.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is <c>null</c>.</exception>
        public LogEntry(string text, MessageLevel level)
        {
            this.Text = text ?? throw new ArgumentNullException(nameof(text));
            this.Level = level;
        }
    }
    #endregion // Typedefs

    #region Fields

    /// <summary> A default value of field  _maxMsgHistoryItems </summary>
    protected const int _defaultMsgHistoryItems = 1024;

    /// <summary> A default value of field  _shouldPreprocessItems </summary>
    protected const bool _defaultShouldPreprocessItems = true;

    /// <summary>
    /// An indicator that is true if has added text before, false if not.
    /// With the help of that, When the control becomes valid the first time, it not only adds the new text,
    /// but also flushes the existing _msgHistory content.
    /// </summary>
    protected bool _hasAddedTextBefore;

    /// <summary> A queue of inserted text items</summary>
    protected Queue<LogEntry> _msgHistory = new();

    /// <summary> The lock around queue</summary>
    protected readonly object _lockHistory = new();

    /// <summary> Maximum length of the internal queue buffer </summary>
    private int _maxMsgHistoryItems = _defaultMsgHistoryItems;

    /// <summary> If true,  the method PreprocessAddedText will be called upon adding the new item </summary>
    private readonly bool _shouldPreprocessItems = _defaultShouldPreprocessItems;

    /// <summary> A weak reference to target wrapped control. Used by the public property WrappedControl </summary>
    private WeakReference<CTRL> _targetReference;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Constructor accepting as a single input argument the wrapped control. </summary>
    ///
    /// <param name="ctrl"> The wrapped WinForms control. </param>
    public DumperCtrlWrapper(CTRL ctrl)
      : this(ctrl, _defaultMsgHistoryItems)
    { }

    /// <summary> Constructor accepting two input arguments. </summary>
    ///
    /// <param name="ctrl">  The wrapped WinForms control. </param>
    /// <param name="maxMsgHistoryItems"> The maximum length of internal queue of recently added text items. </param>
    public DumperCtrlWrapper(CTRL ctrl, int maxMsgHistoryItems)
      : this(ctrl, maxMsgHistoryItems, _defaultShouldPreprocessItems)
    { }

    /// <summary> Constructor accepting three input arguments. </summary>
    ///
    /// <param name="ctrl">                  The wrapped WinForms control. </param>
    /// <param name="maxMsgHistoryItems">    The maximum length of internal queue of recently added text items. </param>
    /// <param name="shouldPreprocessItems"> Initializes the value of property ShouldPreprocessItems.  
    /// If true,  the method <see cref="PreprocessAddedText "/>will be called upon adding the new text item. </param>
    public DumperCtrlWrapper(CTRL ctrl, int maxMsgHistoryItems, bool shouldPreprocessItems)
    {
        ctrl.CheckNotDisposed(nameof(ctrl));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxMsgHistoryItems, 0);

        _targetReference = new WeakReference<CTRL>(ctrl);
        _maxMsgHistoryItems = maxMsgHistoryItems;
        _shouldPreprocessItems = shouldPreprocessItems;

        ctrl.HandleCreated += Ctrl_HandleCreated;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Return the wrapped control ( if there is any ).
    /// </summary>
    public CTRL WrappedControl
    {
        get
        {
            this.CheckNotDisposed();
            return (TargetReference.TryGetTarget(out CTRL ctrl) ? ctrl : null);
        }
    }

    /// <summary>
    /// Gets an indication whether the wrapped control referenced by the current WeakReference object 
    /// has been garbage collected.
    /// Returns true if that control has not been garbage collected, false otherwise.
    /// </summary>
    public bool IsAlive
    {
        get { return (!this.IsDisposed) && TargetReference.TryGetTarget(out CTRL _); }
    }

    /// <summary>
    /// Returns the property of InvokeRequired value for the wrapped control ( if there is any )
    /// </summary>
    public bool InvokeRequired
    {
        get
        {
            CTRL ctrlTarget;
            bool bRes = false;

            if ((!this.IsDisposed) && (null != (ctrlTarget = WrappedControl)))
            {
                bRes = ctrlTarget.InvokeRequired;
            }
            return bRes;
        }
    }

    /// <summary>   Returns the WeakReference created by the constructor. </summary>
    /// <remarks>   Will be null if this DumperCtrlWrapper has been disposed. </remarks>
    protected WeakReference<CTRL> TargetReference { get => _targetReference; }

    /// <summary>
    /// Getter for the value of the boolean flag _shouldPreprocessItems.
    /// If true,  the method PreprocessAddedText will be called upon adding the new item.
    /// </summary>
    protected bool ShouldPreprocessItems { get => _shouldPreprocessItems; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Returns the string representing current time of the day, given the DateTime.
    /// </summary>
    /// <param name="now">Represents an instant in time.</param>
    /// <returns>A string like "14:04:09"</returns>
    protected static string DayTimeString(DateTime now)
    {
        TimeSpan ts = now.TimeOfDay;
        string strResult = string.Format(CultureInfo.InvariantCulture,
          "{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
        return strResult;
    }

    /// <summary> Provides preprocessing of the added text.; 
    /// is called individually by <see cref="AddText"/> for each new added text item.
    /// Is implemented as a virtual method that may be overwritten in derived class;
    /// the implementation in this base class adds a time of the day as a prefix. </summary>
    ///
    /// <param name="strAdd"> The new added text item. </param>
    ///
    /// <returns> The resulting string. </returns>
    protected virtual string PreprocessAddedText(string strAdd)
    {
        string strAddText = string.Format(CultureInfo.InvariantCulture,
          "{0} {1}", DayTimeString(DateTime.Now), strAdd);
        return strAddText;
    }

    /// <summary>
    /// Adds the text to the internal message buffer (queue)
    /// and to the wrapped control ( if there is any ).
    /// </summary>
    /// <remarks>
    /// If the history contains less than <see cref="HistoryLimit"/>items, simply adds a new string to control
    /// text. Otherwise, remove the first string from history list, add a new string to queue, and construct a
    /// new text from history queue.
    /// </remarks>
    /// <param name="entry"> The log entry containing the text and severity level. </param>
    /// <returns>
    /// An enum <see cref="AddTextResult  "/> value indicating what type of change has been done .
    /// </returns>
    protected virtual AddTextResult AddText(LogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (ShouldPreprocessItems)
        {
            entry = new LogEntry(PreprocessAddedText(entry.Text), entry.Level);
        }

        string strAdd = entry.Text;
        StringBuilder sbCompletelyNewContents = null;
        CTRL ctrl = WrappedControl;
        bool isControlOk = (null != ctrl);
        bool historyFull = false;
        AddTextResult res = AddTextResult.AddNone;

        lock (_lockHistory)
        {
            isControlOk = isControlOk && ctrl.IsHandleCreated && !ctrl.InvokeRequired;
            if (historyFull = _msgHistory.Count >= this.HistoryLimit)
            {
                _msgHistory.Dequeue();
            }
            _msgHistory.Enqueue(entry);

            if (isControlOk)
            {
                // Either first time or history rollover → rebuild full content
                if (!_hasAddedTextBefore || historyFull)
                {
                    sbCompletelyNewContents = RebuildHistoryText();
                }
                _hasAddedTextBefore = true;
            }
        }

        if (isControlOk)
        {
            if (sbCompletelyNewContents is null)
            {
                ctrl.Text += strAdd;
                res = AddTextResult.AppendedOnly;
            }
            else
            {
                ctrl.Text = sbCompletelyNewContents.ToString();
                res = historyFull ? AddTextResult.RemovedAndAppended : AddTextResult.AppendedOnly;
            }
        }

        return res;
    }

    /// <summary>
    /// Rebuilds the full concatenated text from the current message history.
    /// </summary>
    /// <returns>The concatenated text of all log entries in the message history.</returns>
    protected StringBuilder RebuildHistoryText()
    {
        StringBuilder sb = null;

        if (_msgHistory.Count > 0)
        {
            sb = new StringBuilder();
            foreach (LogEntry item in _msgHistory)
            {
                sb.Append(item.Text);
            }
        }

        return sb;
    }

    /// <summary>
    /// Cleans any previously dumped contents, both in the internal message buffer (queue)
    /// and in the wrapped control ( if there is any ).
    /// </summary>
    protected virtual void ClearTextHistory()
    {
        Debug.Assert(!InvokeRequired);
        CTRL ctrl = WrappedControl;

        lock (_lockHistory)
        {
            _msgHistory.Clear();
        }
        if (null != ctrl)
        {
            ctrl.Text = string.Empty;
        }
    }

    /// <summary>
    /// Writes the specified <see cref="LogEntry"/> to the underlying control,
    /// either directly or via thread invocation, depending on context.
    /// </summary>
    /// <param name="entry">The log entry containing the text and severity level.</param>
    /// <returns><c>true</c> if the entry was successfully added; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="entry"/> is <c>null</c>.</exception>
    protected virtual bool DumpEntry(LogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        bool bRes = false;
        CTRL ctrl = WrappedControl;

        if (null != ctrl)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.BeginInvoke(DumpEntry, entry);
                bRes = true;
            }
            else
            {
                AddTextResult addRes = this.AddText(entry);
                bRes = (addRes != AddTextResult.AddNone);
            }
        }

        return bRes;
    }

    /// <summary>  Flushes the history to control. </summary>
    protected virtual void FlushHistoryToControl()
    {
        CTRL ctrl = WrappedControl;

        if (ctrl != null && ctrl.IsHandleCreated)
        {
            lock (_lockHistory)
            {
                StringBuilder rebuilt = RebuildHistoryText();
                if (rebuilt is not null)
                {
                    ctrl.Text = rebuilt.ToString();
                    _hasAddedTextBefore = true;
                }
            }
        }
    }

    private void Ctrl_HandleCreated(object sender, EventArgs args)
    {
        FlushHistoryToControl();
    }
    #endregion // Methods

    #region IDisposableEx Members
    #region IDisposable Members

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the method has been
    /// called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed. If
    /// disposing equals false, the method has been called by the runtime from inside the finalizer and you
    /// should not reference other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
    protected virtual void Dispose(bool disposing)
    {
        // release managed resources first
        if (disposing)
        {
            if (!IsDisposed)
            {
                lock (_lockHistory)
                {
                    _msgHistory = null;
                }
                _targetReference = null;
            }
        }
        // no unmanaged resources here ...
    }

    /// <summary>
    /// Implementing IDisposable. Do not make this method virtual. 
    /// The derived class should not overwrite this method, 
    /// but the virtual method Dispose(bool disposing)
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposable Members

    /// <summary>
    /// Returns true in case the object has been disposed and no longer should be used.
    /// </summary>
    public bool IsDisposed { get => (null == TargetReference); }

    #endregion // IDisposableEx Members

    #region IDumperEx Members
    #region IDumper Members

    /// <summary>	Implementation of IDumper.DumpText. </summary>
    /// <param name="text">	The dumped text. </param>
    /// <returns>	true if it succeeds, false if it fails. </returns>
    public virtual bool DumpText(string text)
    {
        this.CheckNotDisposed();
        return DumpEntry(new LogEntry(text, MessageLevel.Info));
    }

    /// <inheritdoc/>
    public virtual bool DumpWarning(string text)
    {
        this.CheckNotDisposed();
        return DumpEntry(new LogEntry(text, MessageLevel.Warning));
    }

    /// <inheritdoc/>
    public virtual bool DumpError(string text)
    {
        this.CheckNotDisposed();
        return DumpEntry(new LogEntry(text, MessageLevel.Error));
    }

    /// <summary>	Implementation of IDumper.Reset.  Cleans any previously dumped contents. </summary>
    /// <returns>	true if it succeeds, false if it fails. </returns>
    public bool Reset()
    {
        CTRL ctrl;
        bool bRes;

        this.CheckNotDisposed();
        if (bRes = ((null != (ctrl = WrappedControl)) && ctrl.IsHandleCreated))
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.Invoke(new ResetMethodInvoker((this as IDumper).Reset));
            }
            else
            {
                this.ClearTextHistory();
            }
        }
        return bRes;
    }
    #endregion // IDumper Members

    /// <inheritdoc/>
    public bool SupportsHistory { get => true; }

    /// <inheritdoc/>
    public int HistoryLimit { get => _maxMsgHistoryItems; }

    /// <inheritdoc/>
    public bool SetHistoryLimit(int nNewLimit)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nNewLimit, 0);
        _maxMsgHistoryItems = nNewLimit;

        lock (_lockHistory)
        {
            while (_msgHistory.Count > HistoryLimit)
            {
                _msgHistory.Dequeue();
            }
        }
        return true;
    }
    #endregion // IDumperEx Members
}

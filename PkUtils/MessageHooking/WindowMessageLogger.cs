// Ignore Spelling: Utils
//
using System;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.Utils;

namespace PK.PkUtils.MessageHooking;

/// <summary>
/// Logs incoming Win32 messages on hooked window into a log file.
/// </summary>
public class WindowMessageLogger : WindowMessageHook
{
    #region Fields

    private int _callDepth;
    private const int _defaultIndent = 2;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public WindowMessageLogger() : this(null)
    { }

    /// <summary> The constructor accepting callback to log. </summary>
    /// <param name="fnLogInfo"> A callback function logging the information. May be null. </param>
    public WindowMessageLogger(Action<string> fnLogInfo)
        : this(_defaultIndent, fnLogInfo, null)
    { }

    /// <summary> The constructor accepting callback to log. </summary>
    /// <param name="indent"> The indent. </param>
    /// <param name="fnLogInfo"> A callback function logging the information. May be null. </param>
    /// <param name="messageFilter"> (Optional) Represents the filter of messages that should be logged. If null,
    /// all messages logged. </param>
    /// <param name="beginPrefix"> (Optional) The begin prefix. </param>
    /// <param name="endPrefix"> (Optional) The end prefix. </param>
    public WindowMessageLogger(
        int indent,
        Action<string> fnLogInfo,
        Func<int, bool> messageFilter = null,
        string beginPrefix = "now handling ",
        string endPrefix = "end handling ")
        : base()
    {
        ArgumentOutOfRangeException.ThrowIfNegative(indent);
        Indent = indent;
        FnLogInfo = fnLogInfo;
        MessageFilter = messageFilter;
        BeginPrefix = beginPrefix.EmptyIfNull();
        EndPrefix = endPrefix.EmptyIfNull();
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Represents the callback function logging the information. If null, there is no logging</summary>
    protected Action<string> FnLogInfo { get; init; }

    /// <summary> Represents the filter of messages that should be logged. If null, all messages logged.</summary>
    protected Func<int, bool> MessageFilter { get; init; }

    /// <summary> Gets the indent. </summary>
    protected int Indent { get; }

    /// <summary> Gets the begin prefix. </summary>
    protected string BeginPrefix { get; init; }

    /// <summary> Gets the end prefix. </summary>
    protected string EndPrefix { get; init; }

    /// <summary> Gets the current depth of processing. </summary>
    protected int CallDepth { get => _callDepth; }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Overwrites the implementation of the base class in order to perform different message processing.
    /// </summary>
    /// <param name="m"> The Message structure which wraps Win32 messages that Windows sends. </param>
    protected override void HookWindowProc(ref Message m)
    {
        bool logNow = false;
        string offset = null;
        string infoBase = null;

        _callDepth++;

        try
        {
            if (logNow = ((FnLogInfo != null) && ((MessageFilter == null) || MessageFilter(m.Msg))))
            {
                offset = new string(' ', CallDepth * Indent);
                infoBase = DumpWin32Message.ToText(m);
                FnLogInfo(offset + BeginPrefix + infoBase);
            }
            base.HookWindowProc(ref m);
        }
        finally
        {
            _callDepth--;
            if (logNow) FnLogInfo(offset + EndPrefix + infoBase);
        }
    }
}
#endregion // Methods

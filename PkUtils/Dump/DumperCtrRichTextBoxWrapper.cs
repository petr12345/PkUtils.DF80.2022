// Ignore Spelling: PK, Ctrl, Preprocess, sel, Utils
//

using System;
using System.Drawing;
using System.Windows.Forms;

#pragma warning disable IDE0290 // Use primary constructor

namespace PK.PkUtils.Dump;

/// <summary>
/// The wrapper around a RichTextBox control, providing the IDumper-behavior to it.
/// </summary>
public class DumperCtrRichTextBoxWrapper : DumperCtrlTextBoxBaseWrapper<RichTextBox>
{
    #region Constructor(s)

    /// <summary> Constructor accepting as a single input argument the wrapped control. </summary>
    ///
    /// <param name="ctrl"> The wrapped WinForms control. </param>
    public DumperCtrRichTextBoxWrapper(RichTextBox ctrl)
      : this(ctrl, _defaultMsgHistoryItems)
    { }

    /// <summary> Constructor accepting two input arguments. </summary>
    ///
    /// <param name="ctrl">  The wrapped WinForms control. </param>
    /// <param name="maxMsgHistoryItems"> The maximum length of internal queue of recently added text items. </param>
    public DumperCtrRichTextBoxWrapper(RichTextBox ctrl, int maxMsgHistoryItems)
      : base(ctrl, maxMsgHistoryItems)
    { }
    #endregion // Constructor(s)

    #region Methods

    /// <summary>   Gets color for log entry. </summary>
    /// <param name="entry"> The log entry. Can't be null. </param>
    /// <returns>   The color for entry. </returns>
    protected virtual Color? GetColorForEntry(LogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        return entry.Level switch
        {
            MessageLevel.Error => Color.Red,
            MessageLevel.Warning => Color.Blue,
            _ => null,
        };
    }

    /// <summary> Appends an entry contents. </summary>
    /// <param name="entry"> The text entry. Can't be null. </param>
    protected override void AppendEntry(LogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        RichTextBox rtb = this.WrappedControl;
        CheckInvokeNotRequired(rtb);

        Color? color = GetColorForEntry(entry);
        Color foreColor = rtb.ForeColor;

        rtb.SelectionStart = rtb.TextLength;
        rtb.SelectionLength = 0;
        rtb.SelectionColor = color ?? foreColor;

        rtb.AppendText(FinalEntryText(entry));
        rtb.SelectionColor = foreColor;

        _hasAddedTextBefore = true;
    }

    /// <summary>  Flushes the history to control. </summary>
    protected override void FlushHistoryToControl()
    {
        RichTextBox rtb = WrappedControl;
        CheckInvokeNotRequired(rtb);

        lock (_lockHistory)
        {
            bool wasReadOnly = rtb.ReadOnly;
            rtb.ReadOnly = false;
            rtb.Clear();

            foreach (LogEntry entry in _msgHistory)
            {
                AppendEntry(entry);
            }

            rtb.ReadOnly = wasReadOnly;
            _hasAddedTextBefore = true;
        }
    }
    #endregion // Methods
}
#pragma warning restore IDE0290     // Use primary constructor
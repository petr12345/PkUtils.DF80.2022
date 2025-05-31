// Ignore Spelling: CCA, Ctrl, Preprocess, sel, Utils
//

using System;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.UI.Utils;

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
      : this(ctrl, maxMsgHistoryItems, _defaultShouldPreprocessItems)
    { }

    /// <summary> Constructor accepting three input arguments. </summary>
    ///
    /// <param name="ctrl">                  The wrapped WinForms control. </param>
    /// <param name="maxMsgHistoryItems">    The maximum length of internal queue of recently added text items. </param>
    /// <param name="shouldPreprocessItems"> Initializes the value of property ShouldPreprocessItems.  
    /// If true,  the method <see cref="DumperCtrlWrapper{CTRL}.PreprocessAddedText "/>will be called upon adding the new text item. </param>
    public DumperCtrRichTextBoxWrapper(RichTextBox ctrl, int maxMsgHistoryItems, bool shouldPreprocessItems)
      : base(ctrl, maxMsgHistoryItems, shouldPreprocessItems)
    { }
    #endregion // Constructor(s)

    #region Methods

    /// <summary>   Removes text from the start of the control. </summary>
    /// <param name="rtb"> The rtb control. </param>
    /// <param name="length"> Number of characters to remove. </param>
    protected static void RemoveTextFromStart(RichTextBox rtb, int length)
    {
        if (length <= 0 || length > rtb.TextLength)
            return;

        rtb.Select(0, length);
        rtb.SelectedText = string.Empty;
    }

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

    /// <summary>   Appends an entry with color. </summary>
    /// <param name="rtb"> The rtb control. </param>
    /// <param name="entry"> The log entry. Can't be null. </param>
    protected void AppendEntryWithColor(RichTextBox rtb, LogEntry entry)
    {
        Color? color = GetColorForEntry(entry);
        rtb.SelectionStart = rtb.TextLength;
        rtb.SelectionLength = 0;

        if (color.HasValue)
            rtb.SelectionColor = color.Value;
        else
            rtb.SelectionColor = rtb.ForeColor;

        rtb.AppendText(entry.Text);
        rtb.SelectionColor = rtb.ForeColor;

        _hasAddedTextBefore = true;
    }

    /// <summary>   Flushes the history to control. </summary>
    protected override void FlushHistoryToControl()
    {
        RichTextBox rtb = this.WrappedControl;
        if (rtb == null || !rtb.IsHandleCreated)
            return;

        if (rtb.InvokeRequired)
        {
            rtb.Invoke(new Action(FlushHistoryToControl));
            return;
        }

        bool wasReadOnly = rtb.ReadOnly;
        rtb.ReadOnly = false;
        rtb.Clear();

        foreach (LogEntry entry in _msgHistory)
        {
            AppendEntryWithColor(rtb, entry);
        }

        rtb.ReadOnly = wasReadOnly;
        _hasAddedTextBefore = true;
    }

    /// <summary> Overrides the virtual method of the base class, in order to add scrolling to the actual end of
    /// control text. </summary>
    ///
    /// <remarks> In case there was any selection before text adding, and if the text is just appended
    /// ( AddTextResult.AppendedOnly ), the selection is eventually  restored. </remarks>
    ///
    /// <returns> An enum <see cref="DumperCtrlWrapper{CTRL}.AddTextResult"/> value indicating what
    /// type of change has been done. </returns>
    protected override AddTextResult AddText(LogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        RichTextBox rtb = this.WrappedControl;
        AddTextResult result = AddTextResult.AddNone;

        if (rtb == null || !rtb.IsHandleCreated || rtb.InvokeRequired)
            return result;

        if (ShouldPreprocessItems)
        {
            entry = new LogEntry(PreprocessAddedText(entry.Text), entry.Level);
        }

        TextBoxSelInfo selInfo = rtb.GetSelInfo();
        bool historyFull = false;

        lock (_lockHistory)
        {
            while (_msgHistory.Count >= HistoryLimit)
            {
                _msgHistory.Dequeue();
                historyFull = true;
            }
            _msgHistory.Enqueue(entry);

            if (!HasAddedTextBefore)
            {
                // On first add, flush everything
                FlushHistoryToControl();
                result = AddTextResult.RemovedAndAppended;
            }
            else if (historyFull)
            {
                rtb.SuspendLayout();
                RemoveTextFromStart(rtb, entry.Text.Length);
                AppendEntryWithColor(rtb, entry);
                rtb.ResumeLayout();
                result = AddTextResult.RemovedAndAppended;
            }
            else
            {
                AppendEntryWithColor(rtb, entry);
                result = AddTextResult.AppendedOnly;
            }
        }

        RestoreSelectionOrScrollToEnd(rtb, selInfo);
        return result;
    }

    #endregion // Methods
}

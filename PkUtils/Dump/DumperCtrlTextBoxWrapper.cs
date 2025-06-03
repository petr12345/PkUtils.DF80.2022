// Ignore Spelling: CCA, Ctrl, Preprocess, sel, Utils
//

using System;
using System.Windows.Forms;
using PK.PkUtils.UI.Utils;

#pragma warning disable IDE0057   // Use range operator
#pragma warning disable IDE0290   // Use primary constructor

namespace PK.PkUtils.Dump;

/// <summary>
/// The wrapper around a TextBox control. 
/// Basically, this is more specialized <see cref="DumperCtrlTextBoxBaseWrapper{T}"/>.
/// </summary>
public class DumperCtrlTextBoxWrapper : DumperCtrlTextBoxBaseWrapper<TextBox>
{
    #region Constructor(s)

    /// <summary> Constructor accepting as a single input argument the wrapped control. </summary>
    /// <param name="textBox"> The wrapped TextBox. </param>
    public DumperCtrlTextBoxWrapper(TextBox textBox)
      : this(textBox, _defaultMsgHistoryItems)
    { }

    /// <summary> Constructor accepting two input arguments. </summary>
    /// <param name="textBox"> The wrapped TextBox. </param>
    /// <param name="maxMsgHistoryItems"> The maximum length of internal queue of recently added text items. </param>
    public DumperCtrlTextBoxWrapper(TextBox textBox, int maxMsgHistoryItems)
      : this(textBox, maxMsgHistoryItems, _defaultShouldPreprocessItems)
    { }

    /// <summary> Constructor accepting three input arguments. </summary>
    ///
    /// <param name="textBox"> The wrapped WinForms control. </param>
    /// <param name="maxMsgHistoryItems"> The maximum length of internal queue of recently added text items. </param>
    /// <param name="shouldPreprocessItems"> Initializes the value of property ShouldPreprocessItems.  
    /// If true,  the method <see cref="DumperCtrlWrapper{CTRL}.PreprocessAddedText "/>will be called upon adding the new text item. </param>
    public DumperCtrlTextBoxWrapper(TextBox textBox, int maxMsgHistoryItems, bool shouldPreprocessItems)
      : base(textBox, maxMsgHistoryItems, shouldPreprocessItems)
    { }
    #endregion // Constructor(s)

    #region Methods

    /// <summary>
    /// Removes characters from the beginning of the TextBox.
    /// </summary>
    /// <param name="tb"> The TextBox control. </param>
    /// <param name="length"> Number of characters to remove. </param>
    protected static void RemoveTextFromStart(TextBox tb, int length)
    {
        ArgumentNullException.ThrowIfNull(tb);
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        if (length > 0 && length <= tb.TextLength)
        {
            tb.Text = tb.Text.Substring(length);
        }
    }

    /// <summary>
    /// Flushes the history to control.
    /// </summary>
    protected override void FlushHistoryToControl()
    {
        TextBox tb = this.WrappedControl;
        if (tb == null || !tb.IsHandleCreated)
            return;

        if (tb.InvokeRequired)
        {
            tb.Invoke(new Action(FlushHistoryToControl));
        }
        else
        {
            bool wasReadOnly = tb.ReadOnly;
            tb.ReadOnly = false;

            tb.Clear();

            foreach (LogEntry entry in _msgHistory)
            {
                tb.AppendText(entry.Text);
            }

            tb.ReadOnly = wasReadOnly;
            _hasAddedTextBefore = true;
        }
    }

    /// <summary>
    /// Adds a text entry to the TextBox with trimming if needed.
    /// </summary>
    /// 
    /// <param name="entry"> The text entry. Can't be null. </param>
    /// <returns> An enum <see cref="DumperCtrlWrapper{CTRL}.AddTextResult"/> value indicating what
    /// type of change has been done. </returns>
    protected override AddTextResult AddText(LogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        TextBox tb = this.WrappedControl;
        AddTextResult result = AddTextResult.AddNone;

        if (tb == null || !tb.IsHandleCreated || tb.InvokeRequired)
            return result;

        if (ShouldPreprocessItems)
        {
            entry = new LogEntry(PreprocessAddedText(entry.Text), entry.Level);
        }

        TextBoxSelInfo selInfo = tb.GetSelInfo();
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
                FlushHistoryToControl();
                result = AddTextResult.AppendedOnly;
            }
            else if (historyFull)
            {
                tb.SuspendLayout();
                RemoveTextFromStart(tb, entry.Text.Length);
                tb.AppendText(entry.Text);
                tb.ResumeLayout();
                result = AddTextResult.RemovedAndAppended;
            }
            else
            {
                tb.AppendText(entry.Text);
                result = AddTextResult.AppendedOnly;
            }
        }

        RestoreSelectionOrScrollToEnd(tb, selInfo);
        return result;
    }

    #endregion // Methods
}
#pragma warning restore IDE0290  // Use primary constructor
#pragma warning restore IDE0057  // Use range operator
// Ignore Spelling: CCA, Ctrl, Preprocess, sel, Utils
//

using System;
using System.Windows.Forms;
using PK.PkUtils.UI.Utils;

#pragma warning disable IDE0290     // Use primary constructor

namespace PK.PkUtils.Dump;

/// <summary>
/// The wrapper around a TextBoxBase control, providing the IDumper-behavior to it.
/// Derives from <see cref="DumperCtrlWrapper{T}"/> , but overwrites the method <see cref="AddText"/>,
/// in order to add scrolling to caret.
/// </summary>
/// <typeparam name="T"> Type of the WinForms control. </typeparam>
public class DumperCtrlTextBoxBaseWrapper<T> : DumperCtrlWrapper<T> where T : TextBoxBase
{
    #region Constructor(s)

    /// <summary> Constructor accepting as a single input argument the wrapped control. </summary>
    ///
    /// <param name="textBox"> The wrapped WinForms control. </param>
    public DumperCtrlTextBoxBaseWrapper(T textBox)
      : this(textBox, _defaultMsgHistoryItems)
    { }

    /// <summary> Constructor accepting two input arguments. </summary>
    ///
    /// <param name="textBox">  The wrapped WinForms control. </param>
    /// <param name="maxMsgHistoryItems"> The maximum length of internal queue of recently added text items. </param>
    public DumperCtrlTextBoxBaseWrapper(T textBox, int maxMsgHistoryItems)
      : this(textBox, maxMsgHistoryItems, _defaultShouldPreprocessItems)
    { }

    /// <summary> Constructor accepting three input arguments. </summary>
    ///
    /// <param name="textBox"> The wrapped WinForms control. </param>
    /// <param name="maxMsgHistoryItems">    The maximum length of internal queue of recently added text items. </param>
    /// <param name="shouldPreprocessItems"> Initializes the value of property ShouldPreprocessItems.  
    /// If true,  the method <see cref="DumperCtrlWrapper{T}.PreprocessAddedText "/>will be called upon adding the new text item. </param>
    public DumperCtrlTextBoxBaseWrapper(T textBox, int maxMsgHistoryItems, bool shouldPreprocessItems)
      : base(textBox, maxMsgHistoryItems, shouldPreprocessItems)
    { }
    #endregion // Constructor(s)

    #region Methods

    /// <summary>   Removes text from the start of the control. </summary>
    /// <param name="txtBx"> The text box to be affected. </param>
    /// <param name="length"> Number of characters to remove. </param>
    protected static void RemoveTextFromStart(TextBoxBase txtBx, int length)
    {
        ArgumentNullException.ThrowIfNull(txtBx);
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        CheckInvokeNotRequired(txtBx);

        if (length > 0)
        {
            bool wasReadOnly = txtBx.ReadOnly;

            txtBx.ReadOnly = false;
            txtBx.Select(0, Math.Min(length, txtBx.TextLength));
            txtBx.SelectedText = string.Empty;
            txtBx.ReadOnly = wasReadOnly;
        }
    }

    /// <summary>  Scroll to the end. </summary>
    protected override void ScrollToTheEnd()
    {
        TextBoxBase textBox = WrappedControl;
        CheckInvokeNotRequired(textBox);

        textBox.Select(textBox.Text.Length, 0);
        textBox.ScrollToCaret();
    }

    /// <summary>
    /// Restores the selection in the RichTextBox, or scrolls to end if selection conflicts with latest text
    /// visibility.
    /// </summary>
    /// <param name="selInfo"> Information describing the selected. </param>
    protected virtual void RestoreSelectionOrScrollToEnd(TextBoxSelInfo selInfo)
    {
        ArgumentNullException.ThrowIfNull(selInfo);
        CheckInvokeNotRequired(WrappedControl);
        TextBoxBase textBox = WrappedControl;

        if (!selInfo.IsSel)
        {
            // No selection => move caret to end and scroll
            ScrollToTheEnd();
        }
        else
        {
            // Restore the original selection
            textBox.SetSelInfo(selInfo);

            // Calculate how many lines are visible
            int visibleLines = textBox.ClientSize.Height / textBox.Font.Height;

            // Determine the last visible line index after selection
            int selEndLine = textBox.GetLineFromCharIndex(selInfo.EndChar);
            int lastLine = textBox.GetLineFromCharIndex(textBox.TextLength);

            // If last line is not visible due to selection, scroll to it
            if (lastLine > selEndLine + visibleLines - 1)
            {
                textBox.Select(textBox.TextLength, 0);
                textBox.ScrollToCaret();
            }
        }
    }

    /// <summary> Appends an entry contents. </summary>
    /// <param name="entry"> The text entry. Can't be null. </param>
    protected virtual void AppendEntry(LogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        TextBoxBase txtBx = this.WrappedControl;

        CheckInvokeNotRequired(txtBx);
        txtBx.AppendText(entry.Text);
        _hasAddedTextBefore = true;
    }

    /// <summary> Overrides the virtual method of the base class, in order to add scrolling to the actual end of
    /// control text. </summary>
    ///
    /// <remarks> In case there was any selection before text adding, and if the text is just appended
    /// ( AddTextResult.AppendedOnly ), the selection is eventually  restored. </remarks>
    ///
    /// <param name="entry"> The log entry containing the text and severity level. </param>
    /// <returns> An enum <see cref="DumperCtrlWrapper{T}.AddTextResult"/> value indicating what
    /// type of change has been done. </returns>
    protected override AddTextResult AddText(LogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        TextBoxBase txtBx = WrappedControl;
        CheckInvokeNotRequired(txtBx);

        if (ShouldPreprocessItems)
        {
            entry = new LogEntry(PreprocessAddedText(entry.Text), entry.Level);
        }

        bool historyFull = false;
        bool isControlOk = txtBx.IsHandleCreated;
        int sumaRemovedLength = 0;
        TextBoxSelInfo selInfo = isControlOk ? txtBx.GetSelInfo() : null;
        AddTextResult result = AddTextResult.AddNone;

        lock (_lockHistory)
        {
            while (_msgHistory.Count >= HistoryLimit)
            {
                sumaRemovedLength += _msgHistory.Dequeue().Text.Length;
                historyFull = true;
            }
            _msgHistory.Enqueue(entry);

            if (isControlOk)
            {
                if (!HasAddedTextBefore)
                {
                    // On first add, flush everything
                    FlushHistoryToControl();
                    result = AddTextResult.AppendedOnly;
                }
                else if (historyFull)
                {
                    RemoveTextFromStart(txtBx, sumaRemovedLength);
                    AppendEntry(entry);
                    result = AddTextResult.RemovedAndAppended;
                    // get rid of no longer valid previous selection info
                    selInfo = new TextBoxSelInfo(txtBx.Text.Length);
                }
                else
                {
                    AppendEntry(entry);
                    result = AddTextResult.AppendedOnly;
                }
            }
        }

        if (isControlOk) RestoreSelectionOrScrollToEnd(selInfo);

        return result;
    }
    #endregion // Methods
}
#pragma warning restore IDE0290     // Use primary constructor
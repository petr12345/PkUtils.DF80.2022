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

    /// <summary>
    /// Restores the selection in the RichTextBox, or scrolls to end if selection conflicts with latest text
    /// visibility.
    /// </summary>
    /// <param name="textBox"> The text box control. </param>
    /// <param name="selInfo"> Information describing the selected. </param>
    protected static void RestoreSelectionOrScrollToEnd(
        TextBoxBase textBox,
        TextBoxSelInfo selInfo)
    {
        ArgumentNullException.ThrowIfNull(textBox);
        ArgumentNullException.ThrowIfNull(selInfo);

        if (!selInfo.IsSel)
        {
            // No selection => move caret to end and scroll
            textBox.Select(textBox.Text.Length, 0);
            textBox.ScrollToCaret();
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
        TextBoxBase txtBx = WrappedControl;
        TextBoxSelInfo selInfo = null;
        AddTextResult result;

        if ((null != txtBx) && txtBx.IsHandleCreated && !txtBx.InvokeRequired)
        {
            // Get old selection info. It will also indicate there is reasonable control to operate with
            selInfo = txtBx.GetSelInfo();
        }

        if (AddTextResult.AddNone != (result = base.AddText(entry)))
        {
            if (selInfo != null)
            {
                if (!selInfo.IsSel)
                {
                    // Scroll to end only if there was no selection.
                    txtBx.Select(txtBx.Text.Length, 0);
                    txtBx.ScrollToCaret();
                }
                else if (result == AddTextResult.AppendedOnly)
                {
                    // Restore selection if text was just appended.
                    txtBx.SetSelInfo(selInfo);
                }
            }
        }

        return result;
    }
    #endregion // Methods
}
#pragma warning restore IDE0290     // Use primary constructor
// Ignore Spelling: CCA, Ctrl, Preprocess, sel, Utils
//

using System.Windows.Forms;

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

}

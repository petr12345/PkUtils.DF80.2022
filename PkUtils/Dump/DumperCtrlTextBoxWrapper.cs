/***************************************************************************************************************
*
* FILE NAME:   .\Dump\DumperCtrlWrapper.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains implementation of DumperCtrlTextBoxWrapper
*
**************************************************************************************************************/


// Ignore Spelling: Utils, Preprocess, Ctrl
//

using System;
using System.Windows.Forms;
using PK.PkUtils.UI.Utils;

namespace PK.PkUtils.Dump;

/// <summary>
/// The wrapper around a TextBox control, providing the IDumper-behavior to it.
/// Derives from DumperCtrlWrapper, but overwrites the method AddText,
/// in order to add scrolling to caret.
/// </summary>
public class DumperCtrlTextBoxWrapper : DumperCtrlWrapper<TextBoxBase>
{
    #region Constructor(s)

    /// <summary> Constructor getting as a single input argument the wrapped control. </summary>
    ///
    /// <param name="ctrl"> The wrapped WinForms control. </param>
    public DumperCtrlTextBoxWrapper(TextBoxBase ctrl)
      : this(ctrl, _defaultMsgHistoryItems)
    {
    }

    /// <summary> Constructor getting two input arguments. </summary>
    ///
    /// <param name="ctrl">  The wrapped WinForms control. </param>
    /// <param name="maxMsgHistoryItems"> The maximum length of internal  queue of recently added text items. </param>
    public DumperCtrlTextBoxWrapper(TextBoxBase ctrl, int maxMsgHistoryItems)
      : this(ctrl, maxMsgHistoryItems, _defaultShouldPreprocessItems)
    {
    }

    /// <summary> Constructor getting three input arguments. </summary>
    ///
    /// <param name="ctrl">                  The wrapped WinForms control. </param>
    /// <param name="maxMsgHistoryItems">    The maximum length of internal  queue of recently added text items. </param>
    /// <param name="shouldPreprocessItems"> Initializes the value of property ShouldPreprocessItems.  
    /// If true,  the method <see cref="DumperCtrlWrapper{CTRL}.PreprocessAddedText "/>will be called upon adding the new text item. </param>
    public DumperCtrlTextBoxWrapper(TextBoxBase ctrl, int maxMsgHistoryItems, bool shouldPreprocessItems)
      : base(ctrl, maxMsgHistoryItems, shouldPreprocessItems)
    {
        WrappedControl.VisibleChanged += new EventHandler(WrappedTextBox_VisibleChanged);
    }
    #endregion // Constructor(s)

    #region Properties
    #endregion // Properties

    #region Methods

    /// <summary> Overrides the virtual method of the base class, in order to add scrolling to the actual end of
    /// control text. </summary>
    ///
    /// <remarks> In case there was any selection before text adding, and if the text is just appended
    /// ( AddTextResult.AppendedOnly ), the selection is eventually  restored. </remarks>
    ///
    /// <param name="strAdd"> The new added text item. </param>
    ///
    /// <returns> An enum <see cref="DumperCtrlWrapper{CTRL}.AddTextResult"/> value indicating what
    /// type of change has been done . </returns>
    protected override AddTextResult AddText(string strAdd)
    {
        TextBoxBase txtBx = WrappedControl;
        TextBoxSelInfo selInfo = null;
        AddTextResult result;

        if ((null != txtBx) && txtBx.IsHandleCreated && !txtBx.InvokeRequired)
        {
            // Get old selection info. It will also indicate there is reasonable control to operate with
            selInfo = txtBx.GetSelInfo();
        }

        if (AddTextResult.AddNone != (result = base.AddText(strAdd)))
        {
            if (null != selInfo)
            {
                txtBx.Select(txtBx.Text.Length, 0);
                txtBx.ScrollToCaret();

                if ((AddTextResult.AppendedOnly == result) && selInfo.IsSel)
                {
                    txtBx.SetSelInfo(selInfo);
                }
            }
        }

        return result;
    }

    /// <summary> This event handler subscribes to WrappedControl.VisibleChanged and should guarantee the text contents is
    /// scrolled to the end after the text box control is displayed. This is handful for instance if the text box is in a
    /// tabbed dialog, and the related tab is not initially visible. </summary>
    ///
    /// <param name="sender"> The sender of the event. </param>
    /// <param name="args">   The event arguments. </param>
    private void WrappedTextBox_VisibleChanged(object sender, EventArgs args)
    {
        TextBoxSelInfo selInfo;
        TextBoxBase txtBx = WrappedControl;

        if ((null != txtBx) && txtBx.IsHandleCreated && txtBx.Visible)
        {
            selInfo = txtBx.GetSelInfo();

            txtBx.Select(txtBx.Text.Length, 0);
            txtBx.ScrollToCaret();
            if ((null != selInfo) && selInfo.IsSel)
            {
                txtBx.SetSelInfo(selInfo);
            }
        }
    }
    #endregion // Methods
}

// Ignore Spelling: Utils, Rtl
//
using System;
using System.Globalization;
using System.Windows.Forms;

namespace PK.PkUtils.UI.Dialogs.MsgBoxes;

/// <summary> Auxiliary class for proper message box displaying.  Displays a message box that has
/// options that are appropriate for the reading order of the culture. <br/>
/// 
/// To display a message box correctly for cultures that use a right-to-left reading order, the
/// RightAlign and RtlReading members of the MessageBoxOptions enumeration must be passed to the
/// Show method. For more details, see
/// <a href="http://msdn.microsoft.com/library/ms182191(VS.100).aspx">
/// CA1300: Specify MessageBoxOptions
/// </a>. </summary>
[CLSCompliant(true)]
public static class RtlAwareMessageBox
{

    /// <summary>
    /// A method that displays a message box that has options that are appropriate for the reading
    /// order of the culture.
    /// </summary>
    ///
    /// <param name="owner">    An object implementing the IWin32Window interface, that will serve as
    ///                         the dialog box's top-level window and owner. </param>
    /// <param name="text">     The text to display in the message box. </param>
    /// <param name="caption">  The text to display in the title of message box. </param>
    /// <returns> One of the <see cref="DialogResult"/> values. </returns>
    public static DialogResult Show(IWin32Window owner, string text, string caption)
    {
        return Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
    }

    /// <summary>
    /// A method that displays a message box that has options that are appropriate for the reading
    /// order of the culture.
    /// </summary>
    ///
    /// <param name="owner">    An object implementing the IWin32Window interface, that will serve as
    ///                         the dialog box's top-level window and owner. </param>
    /// <param name="text">     The text to display in the message box. </param>
    /// <param name="caption">  The text to display in the title of message box. </param>
    /// <param name="icon">     One of the <see cref="MessageBoxIcon"/> values,
    ///                         that specify which icon will be displayed in MessageBox. </param>
    ///
    /// <returns> One of the <see cref="DialogResult"/> values. </returns>
    public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxIcon icon)
    {
        return Show(owner, text, caption, MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1);
    }

    /// <summary>
    /// A method that displays a message box that has options that are appropriate for the reading
    /// order of the culture.
    /// </summary>
    ///
    /// <param name="owner">    An object implementing the IWin32Window interface, that will serve as
    ///                         the dialog box's top-level window and owner. </param>
    /// <param name="text">     The text to display in the message box. </param>
    /// <param name="caption">  The text to display in the title of message box. </param>
    /// <param name="buttons">  Specifies which buttons to display in MessageBox. </param>
    /// <param name="icon">     One of the <see cref="MessageBoxIcon"/> values,
    ///                         that specify which icon will be displayed in MessageBox. </param>
    ///
    /// <returns> One of the <see cref="DialogResult"/> values. </returns>
    public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        return Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
    }

    /// <summary>
    /// A method that displays a message box that has options that are appropriate for the reading
    /// order of the culture.
    /// </summary>
    ///
    /// <param name="owner">          An object implementing the IWin32Window interface, that will
    ///                               serve as the dialog box's top-level window and owner. </param>
    /// <param name="text">           The text to display in the message box. </param>
    /// <param name="caption">        The text to display in the title of message box. </param>
    /// <param name="buttons">        One of the <see cref="MessageBoxButtons "/>
    ///                               values, that specify which buttons will be displayed in MessageBox.
    ///                               </param>
    /// <param name="icon">           One of the <see cref="MessageBoxIcon"/>
    ///                               values, that specify which icon will be displayed in MessageBox.
    ///                               </param>
    /// <param name="defaultButton">  Value defining a default button of MessageBox. </param>
    ///
    /// <returns> One of the <see cref="DialogResult"/> values. </returns>
    public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
    {
        return Show(owner, text, caption, buttons, icon, defaultButton, 0);
    }

    /// <summary>
    /// A method that displays a message box that has options that are appropriate for the reading
    /// order of the culture.
    /// </summary>
    ///
    /// <param name="owner">          An object implementing the IWin32Window interface, that will
    ///                               serve as the dialog box's top-level window and owner. </param>
    /// <param name="text">           The text to display in the message box. </param>
    /// <param name="caption">        The text to display in the title of message box. </param>
    /// <param name="buttons">        One of the <see cref="MessageBoxButtons "/>
    ///                               values, that specify which buttons will be displayed in MessageBox.
    ///                               </param>
    /// <param name="icon">           One of the <see cref="MessageBoxIcon"/>
    ///                               values, that specify which icon will be displayed in MessageBox.
    ///                               </param>
    /// <param name="defaultButton">  Value defining a default button of MessageBox. </param>
    /// <param name="options">        Specifies additional options of a message box. </param>
    ///
    /// <returns> One of the <see cref="DialogResult"/> values. </returns>
    public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
    {
        if (IsRightToLeft(owner))
        {
            options |= MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign;
        }

        return MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, options);
    }

    /// <summary> What is the reading order of the window - is it right to left ? </summary>
    ///
    /// <param name="owner">  An object implementing the IWin32Window interface, that will serve as the
    ///                       dialog box's top-level window and owner. </param>
    ///
    /// <returns> true if right to left, false if not. </returns>
    public static bool IsRightToLeft(IWin32Window owner)
    {
        bool bRes;

        if (owner is Control control)
        {
            bRes = control.RightToLeft == RightToLeft.Yes;
        }
        else
        {
            // If no parent control is available, ask the CurrentUICulture
            // if we are running under right-to-left.
            bRes = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;
        }

        return bRes;
    }
}

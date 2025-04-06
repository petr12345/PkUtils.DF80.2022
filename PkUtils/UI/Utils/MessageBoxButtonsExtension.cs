// Ignore Spelling: Utils, Dict
//

using System.Windows.Forms;


namespace PK.PkUtils.UI.Utils;

/// <summary> A message box buttons extension class. </summary>
internal static class MessageBoxButtonsExtension
{
    /// <summary>
    /// The MessageBoxButtons extension method that converts MessageBoxButtons to a dialog result.
    /// </summary>
    /// <param name="buttons"> The buttons to act on. </param>
    /// <param name="defaultResult"> The default result. </param>
    /// <returns>   The given data converted to a DialogResult. </returns>
    public static DialogResult ToDialogResult(this MessageBoxButtons buttons, DialogResult defaultResult)
    {
        switch (buttons)
        {
            case MessageBoxButtons.OK:
                return DialogResult.OK;

            case MessageBoxButtons.OKCancel:
                if (defaultResult == DialogResult.Cancel)
                {
                    break;
                }
                return DialogResult.OK;

            case MessageBoxButtons.YesNo:
                if (defaultResult == DialogResult.No)
                {
                    break;
                }
                return DialogResult.Yes;

            case MessageBoxButtons.YesNoCancel:
                if (defaultResult == DialogResult.No || defaultResult == DialogResult.Cancel)
                {
                    break;
                }
                return DialogResult.Yes;

            case MessageBoxButtons.RetryCancel:
                if (defaultResult == DialogResult.Retry)
                {
                    break;
                }
                return DialogResult.Cancel;

            case MessageBoxButtons.AbortRetryIgnore:
                if (defaultResult == DialogResult.Abort || defaultResult == DialogResult.Retry)
                {
                    break;
                }
                return DialogResult.Ignore;
        }

        return defaultResult;
    }

    /// <summary>
    /// A DialogResult extension method that converts DialogResult to a dialog button identifier.
    /// </summary>
    /// <param name="result"> The result to act on. </param>
    /// <param name="buttons"> The buttons to act on. </param>
    /// <returns>   The given data converted to an int. </returns>
    public static int ToDialogButtonId(this DialogResult result, MessageBoxButtons buttons)
    {
        if (buttons == MessageBoxButtons.OK)
        {
            return 2;
        }

        return (int)result;
    }
}

// Ignore Spelling: Utils, Dict
//

using System.Windows.Forms;


namespace PK.PkUtils.UI.Dialogs.MsgBoxes;

/// <summary>
/// Provides extension methods for working with <see cref="MessageBoxButtons"/> and <see cref="DialogResult"/>.
/// </summary>
internal static class MessageBoxButtonsExtension
{
    /// <summary>
    /// Infers the most likely <see cref="DialogResult"/> the user would choose based on the provided <see cref="MessageBoxButtons"/>
    /// and an optional default result.
    /// </summary>
    /// <param name="buttons">The set of message box buttons.</param>
    /// <param name="defaultResult">The default result to fall back to when inference is ambiguous.</param>
    /// <returns>
    /// A <see cref="DialogResult"/> inferred from the button configuration and the provided default.
    /// </returns>
    public static DialogResult ToDialogResult(this MessageBoxButtons buttons, DialogResult defaultResult)
    {
        return buttons switch
        {
            MessageBoxButtons.OK => DialogResult.OK,
            MessageBoxButtons.OKCancel => defaultResult == DialogResult.Cancel
                                ? defaultResult
                                : DialogResult.OK,
            MessageBoxButtons.YesNo => defaultResult == DialogResult.No
                                ? defaultResult
                                : DialogResult.Yes,
            MessageBoxButtons.YesNoCancel => defaultResult == DialogResult.No || defaultResult == DialogResult.Cancel
                                ? defaultResult
                                : DialogResult.Yes,
            MessageBoxButtons.RetryCancel => defaultResult == DialogResult.Retry
                                ? defaultResult
                                : DialogResult.Cancel,
            MessageBoxButtons.AbortRetryIgnore => defaultResult == DialogResult.Abort || defaultResult == DialogResult.Retry
                                ? defaultResult
                                : DialogResult.Ignore,
            _ => defaultResult,
        };
    }

    /// <summary>
    /// Maps a <see cref="DialogResult"/> to a button index for use with custom dialog logic.
    /// </summary>
    /// <param name="result">The dialog result to convert.</param>
    /// <param name="buttons">The message box buttons used.</param>
    /// <returns>
    /// A numeric identifier representing the button index, where 2 represents the sole "OK" button,
    /// or the raw <see cref="DialogResult"/> value otherwise.
    /// </returns>
    public static int ToDialogButtonId(this DialogResult result, MessageBoxButtons buttons)
    {
        return buttons == MessageBoxButtons.OK ? 2 : (int)result;
    }
}

// Ignore Spelling: Utils, autoclosing, autoclose
//
using System;
using System.Threading;
using System.Windows.Forms;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.UI.Dialogs.MsgBoxes;

/// <summary> An automatic closing message box. </summary>
public class AutoClosingMessageBox
{
    #region Typedefs

    /// <summary> Interface for automatic closing message box. </summary>
    public interface IAutoClosingMessageBox
    {
        /// <summary> Shows autoclosing message box. </summary>
        /// <param name="timeout"> The timeout. </param>
        /// <param name="buttons"> (Optional) The buttons. </param>
        /// <param name="defaultResult"> (Optional) The default result. </param>
        /// <returns> A DialogResult. </returns>
        DialogResult Show(TimeSpan timeout, MessageBoxButtons buttons = MessageBoxButtons.OK, DialogResult defaultResult = DialogResult.None);
    }

    private sealed class Impl : IAutoClosingMessageBox
    {
        private readonly Func<TimeSpan, MessageBoxButtons, DialogResult, DialogResult> getResult;

        internal Impl(Func<string, MessageBoxButtons, DialogResult> showMethod, string caption)
        {
            getResult = (timeout, buttons, defaultResult) =>
                new AutoClosingMessageBox(caption, timeout, showMethod, buttons, defaultResult).Result;
        }

        DialogResult IAutoClosingMessageBox.Show(TimeSpan timeout, MessageBoxButtons buttons, DialogResult defaultResult)
        {
            return getResult(timeout, buttons, defaultResult);
        }
    }
    #endregion // Typedefs

    #region Fields

    private readonly string _caption;
    private readonly DialogResult _result;
    #endregion // Fields

    #region Properties

    /// <summary> Gets the result. </summary>
    protected DialogResult Result => _result;

    /// <summary> Gets the caption. </summary>
    protected string Caption => _caption;
    #endregion // Properties

    #region Methods

    /// <summary> Shows the autoclose MessageBox. </summary>
    /// <param name="text"> The text. </param>
    /// <param name="caption"> (Optional) The caption. </param>
    /// <param name="timeout"> (Optional) The timeout. </param>
    /// <param name="buttons"> (Optional) The buttons. </param>
    /// <param name="icon"> (Optional) The icon. </param>
    /// <param name="defaultResult"> (Optional) The default result. </param>
    /// <returns> A DialogResult. </returns>
    public static DialogResult Show(
        string text,
        string caption,
        TimeSpan timeout,
        MessageBoxButtons buttons = MessageBoxButtons.OK,
        MessageBoxIcon icon = MessageBoxIcon.None,
        DialogResult defaultResult = DialogResult.None)
    {
        return new AutoClosingMessageBox(
            caption,
            timeout,
            (capt, btns) => MessageBox.Show(text, capt, btns, icon), buttons, defaultResult).Result;
    }

    /// <summary> Shows the autoclose MessageBox. </summary>
    /// <param name="owner"> The owner. </param>
    /// <param name="text"> The text. </param>
    /// <param name="caption"> (Optional) The caption. </param>
    /// <param name="timeout"> (Optional) The timeout. </param>
    /// <param name="buttons"> (Optional) The buttons. </param>
    /// <param name="icon"> (Optional) The icon. </param>
    /// <param name="defaultResult"> (Optional) The default result. </param>
    /// <returns> A DialogResult. </returns>
    public static DialogResult Show(
        IWin32Window owner,
        string text,
        string caption,
        TimeSpan timeout,
        MessageBoxButtons buttons = MessageBoxButtons.OK,
        MessageBoxIcon icon = MessageBoxIcon.None,
        DialogResult defaultResult = DialogResult.None)
    {
        return new AutoClosingMessageBox(
            caption,
            timeout,
            (capt, btns) => MessageBox.Show(owner, text, capt, btns, icon), buttons, defaultResult).Result;
    }

    /// <summary> Creates <see cref="IAutoClosingMessageBox"/>. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// <param name="showMethod"> The show method. </param>
    /// <param name="caption"> (Optional) The caption. </param>
    /// <returns> An IAutoClosingMessageBox. </returns>
    public static IAutoClosingMessageBox Factory(Func<string, MessageBoxButtons, DialogResult> showMethod, string caption = null)
    {
        ArgumentNullException.ThrowIfNull(showMethod);
        return new Impl(showMethod, caption);
    }

    private AutoClosingMessageBox(
        string caption,
        TimeSpan dueTime,
        Func<string, MessageBoxButtons, DialogResult> showMethod,
        MessageBoxButtons buttons = MessageBoxButtons.OK,
        DialogResult defaultResult = DialogResult.None)
    {
        _caption = caption ?? string.Empty;
        _result = buttons.ToDialogResult(defaultResult);

        using (new System.Threading.Timer(
            callback: OnTimerElapsed,
            state: _result.ToDialogButtonId(buttons),
            dueTime: dueTime,
            period: Timeout.InfiniteTimeSpan)) // specify period -1 to disable periodic signaling.
        {
            _result = showMethod(Caption, buttons);
        }
    }

    private void OnTimerElapsed(object state)
    {
        CloseMessageBoxWindow((int)state);
    }

    private void CloseMessageBoxWindow(int dlgButtonId)
    {
        nint hWnd = User32.FindMessageBox(Caption);
        User32.SendCommandToDlgButton(hWnd, dlgButtonId);
    }
    #endregion // Methods
}
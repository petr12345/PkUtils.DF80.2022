// Ignore Spelling: CCA, Checkbox, fallback, Utils
//

using System;
using System.Linq;
using System.Windows.Forms;

namespace PK.PkUtils.UI.Dialogs.TaskDialogs;

/// <summary>
/// Shows a structured exception dialog using <see cref="TaskDialog"/>, summarizing the error,
/// its impact, suggested actions, and optional technical details. Replaces basic <see cref="MessageBox"/>
/// with a clearer, more informative UI for error reporting.
/// </summary>
public static class ExceptionDialogWrapper
{
    #region Public Methods

    /// <summary>
    /// Displays a detailed exception dialog using <see cref="TaskDialog"/> that summarizes the error, its impact,
    /// suggested user actions, and optionally shows technical details such as exception stack trace.
    /// </summary>
    /// <param name="owner">The parent window for the dialog.</param>
    /// <param name="caption">The window caption (shown in the title bar).</param>
    /// <param name="heading">The main heading displayed at the top of the dialog.</param>
    /// <param name="errorMessage">A brief description of the error ("What happened").</param>
    /// <param name="scope">A message describing the impact of the error ("How this will affect you").</param>
    /// <param name="action">Suggested steps the user can take ("What you can do about it").</param>
    /// <param name="moreDetails">
    /// Optional technical details (e.g., full exception stack trace), displayed in a collapsible section labeled
    /// "Show technical details".
    /// </param>
    /// <param name="buttons">The button set to display (e.g., OK, OKCancel, YesNo).</param>
    /// <param name="icon">An optional icon indicating the severity or nature of the issue. </param>
    /// <param name="defaultButton">
    /// The button that will be selected by default. Must match one of the buttons implied by <paramref name="buttons"/>. 
    /// Defaults to <see cref="DialogResult.None"/> which uses the first button as default.
    /// </param>
    /// <returns> The DialogResult corresponding to the button the user clicked (e.g., OK, Cancel, Yes, No).</returns>
    /// 
    /// <example>
    /// <code>
    /// try
    /// {
    ///     // Some operation that may fail
    ///     PerformDangerousAction();
    /// }
    /// catch (Exception ex)
    /// {
    ///     var result = ExceptionDialogHelper.ShowExceptionDialog(
    ///         owner: this,
    ///         caption: "MyApp Error",
    ///         heading: "Something went wrong",
    ///         errorMessage: ex.Message,
    ///         scope: "This operation was interrupted and may not have completed.",
    ///         action: "Please try again or contact support.",
    ///         moreDetails: ex.ToString(),
    ///         buttons: MessageBoxButtons.OKCancel,
    ///         icon: MessageBoxIcon.Error
    ///     );
    ///
    ///     if (result == DialogResult.OK)
    ///     {
    ///         // Retry or continue
    ///     }
    ///     else
    ///     {
    ///         // Cancel or exit
    ///     }
    /// }
    /// </code>
    /// </example>
    public static DialogResult ShowExceptionDialog(
        IWin32Window owner,
        string caption,
        string heading,
        string errorMessage,
        string scope,
        string action,
        string moreDetails = null,
        MessageBoxButtons buttons = MessageBoxButtons.OK,
        MessageBoxIcon icon = MessageBoxIcon.Error,
        DialogResult defaultButton = DialogResult.None)
    {
        // Map icon and generate buttons
        TaskDialogIcon taskIcon = MapIcon(icon);
        (TaskDialogButtonCollection taskButtons, DialogResult defaultResult) = TaskDialogWrapper.CreateButtons(buttons, defaultButton);

        // Construct main text content
        string[] contentParts = [
            $"► What happened:{Environment.NewLine}{errorMessage}",
            $"► How this will affect you:{Environment.NewLine}{scope}",
            $"► What you can do about it:{Environment.NewLine}{action}"];
        string fullContent = string.Join(Environment.NewLine + Environment.NewLine, contentParts);

        // Create TaskDialogPage
        TaskDialogPage page = new()
        {
            Caption = caption,
            Heading = heading,
            Text = fullContent,
            Icon = taskIcon,
            Buttons = taskButtons,
            DefaultButton = taskButtons.FirstOrDefault(b => Equals(b.Tag, defaultResult)),
            SizeToContent = true
        };

        // Add expandable section if applicable
        if (!string.IsNullOrWhiteSpace(moreDetails))
        {
            page.Expander = new TaskDialogExpander
            {
                Text = moreDetails.Trim(),
                ExpandedButtonText = "Hide technical details",
                CollapsedButtonText = "Show technical detail"
            };
        }

        // Show dialog and determine result
        TaskDialogButton resultButton = TaskDialog.ShowDialog(owner, page);
        DialogResult result = resultButton?.Tag is DialogResult dr ? dr : defaultResult;

        return result;
    }
    #endregion // Public Methods

    #region Internal Methods

    internal static TaskDialogIcon MapIcon(MessageBoxIcon icon)
    {
        return icon switch
        {
            MessageBoxIcon.Error => TaskDialogIcon.Error,
            MessageBoxIcon.Information => TaskDialogIcon.Information,
            MessageBoxIcon.Warning => TaskDialogIcon.Warning,
            MessageBoxIcon.Question => TaskDialogWrapper.QuestionIcon,
            _ => TaskDialogIcon.None
        };
    }
    #endregion // Internal Methods
}

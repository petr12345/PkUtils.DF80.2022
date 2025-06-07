// Ignore Spelling: Utils, checkbox, fallback
//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PK.PkUtils.UI.Utils;

namespace PK.PkUtils.UI.Dialogs.TaskDialogs;

/// <summary> Several utilities as example of usage of <see cref="TaskDialog"/> </summary>
/// <remarks>
/// For the actual value of 'owner' argument in each method, you should use a Form, 
/// Control or instance of <see cref="Win32Window"/>, created from window handle.
/// </remarks>
public static class TaskDialogWrapper
{
    #region Fields
    private static TaskDialogIcon _questionIcon;
    #endregion // Fields

    #region Properties

    /// <summary> Gets the question icon. </summary>
    /// <remarks> Uses dirty trick with SystemIcons.Question, since there is no TaskDialogIcon.Question </remarks>
    public static TaskDialogIcon QuestionIcon
    {
        get => _questionIcon ??= new TaskDialogIcon(SystemIcons.Question.Handle);
    }
    #endregion // Properties

    #region Public Methods
    #region Question_methods

    /// <summary> Invoke TaskDialog to ask a question. </summary>
    /// <param name="owner">control or form</param>
    /// <param name="caption">text for dialog caption</param>
    /// <param name="heading">text for dialog heading</param>
    /// <param name="yesText">text for yes button</param>
    /// <param name="noText">text for no button</param>
    /// <param name="defaultButton">specifies the default button for this dialog</param>
    /// <returns>true for yes button, false for no button</returns>
    public static bool Question(
        IWin32Window owner,
        string caption,
        string heading,
        string yesText,
        string noText,
        DialogResult defaultButton = DialogResult.Yes)
    {
        TaskDialogButtonCollection buttons = (defaultButton == DialogResult.Yes)
            ? CreateButtons((yesText, DialogResult.Yes), (noText, DialogResult.No))
            : CreateButtons((noText, DialogResult.No), (yesText, DialogResult.Yes));

        TaskDialogButton buttonResult = ShowDialogWithButtons(owner, caption, heading, null, QuestionIcon, buttons, defaultButton);
        bool result = buttonResult.Tag is DialogResult dr && dr == DialogResult.Yes;

        return result;
    }

    /// <summary> Invoke TaskDialog to ask a question. </summary>
    /// <param name="owner"> control or form. </param>
    /// <param name="caption"> text for dialog caption. </param>
    /// <param name="heading"> text for dialog heading. </param>
    /// <param name="text"> The text. </param>
    /// <param name="icon"> Icon to display. </param>
    /// <param name="defaultButton"> (Optional) Button to focus. </param>
    /// <returns>   true for yes button, false for no button. </returns>
    public static bool Question(
        IWin32Window owner,
        string caption,
        string heading,
        string text,
        Icon icon,
        DialogResult defaultButton = DialogResult.Yes)
    {
        TaskDialogButtonCollection buttons = (defaultButton == DialogResult.Yes)
            ? CreateButtons(("Yes", DialogResult.Yes), ("No", DialogResult.No))
            : CreateButtons(("No", DialogResult.No), ("Yes", DialogResult.Yes));
        TaskDialogIcon tdIcon = new(icon);
        TaskDialogButton buttonResult = ShowDialogWithButtons(owner, caption, heading, text, tdIcon, buttons, defaultButton);
        bool result = buttonResult.Tag is DialogResult dr && dr == DialogResult.Yes;

        return result;
    }

    /// <summary> Invoke Task Dialog to ask a question. </summary>
    /// <remarks>
    /// Last two parameters are the actions to perform.
    /// </remarks>
    /// <param name="owner"> control or form. </param>
    /// <param name="caption"> text for dialog caption. </param>
    /// <param name="heading"> text for dialog heading. </param>
    /// <param name="text"> The text. </param>
    /// <param name="yesAction"> The action to be executed on Yes. Can be null. </param>
    /// <param name="noAction"> The action to be executed on No. Can be null. </param>
    /// <param name="icon"> (Optional) Icon to display. If null, QuestionIcon will be used. </param>
    ///
    /// <returns>  true for yes button, false for no button. </returns>
    public static bool Question(
        IWin32Window owner,
        string caption,
        string heading,
        string text,
        Action yesAction,
        Action noAction,
        TaskDialogIcon icon = null)
    {
        TaskDialogButtonCollection buttons = CreateButtons(("Yes", DialogResult.Yes), ("No", DialogResult.No));
        TaskDialogIcon tdIcon = icon ?? QuestionIcon;
        TaskDialogButton buttonResult = ShowDialogWithButtons(owner, caption, heading, text, tdIcon, buttons, DialogResult.Yes);
        bool result = buttonResult.Tag is DialogResult dr && dr == DialogResult.Yes;

        if (result)
        {
            yesAction?.Invoke();
        }
        else
        {
            noAction?.Invoke();
        }

        return result;
    }

    /// <summary> Invoke Task Dialog to ask a question. </summary>
    /// <param name="owner"> control or form. </param>
    /// <param name="caption"> text for dialog caption. </param>
    /// <param name="heading"> text for dialog heading. </param>
    /// <param name="text"> The text. </param>
    /// <param name="defaultButton"> (Optional) Button to focus. </param>
    /// <returns> A DialogResult.Yes or DialogResult.No or DialogResult.Cancel. </returns>
    public static DialogResult QuestionYesNoCancel(
        IWin32Window owner,
        string caption,
        string heading,
        string text,
        DialogResult defaultButton = DialogResult.Yes)
    {
        TaskDialogButtonCollection buttons = CreateButtons(MessageBoxButtons.YesNoCancel);
        TaskDialogButton buttonResult = ShowDialogWithButtons(owner, caption, heading, text, QuestionIcon, buttons, defaultButton, allowCancel: true);
        DialogResult result = buttonResult.Tag is DialogResult dr ? dr : defaultButton;

        return result;
    }

    /// <summary> Invoke TaskDialog to ask a question with Abort, Retry, and Ignore options. </summary>
    /// <param name="owner"> control or form. </param>
    /// <param name="caption"> text for dialog caption. </param>
    /// <param name="heading"> text for dialog heading. </param>
    /// <param name="text"> The text. </param>
    /// <param name="icon"> (Optional) Icon to display. If null, TaskDialogIcon.Information will be used. </param>
    /// <param name="defaultButton"> (Optional) Button to focus. </param>
    /// <returns>   A DialogResult.Abort, DialogResult.Retry, or DialogResult.Ignore. </returns>
    public static DialogResult QuestionAbortRetryIgnore(
        IWin32Window owner,
        string caption,
        string heading,
        string text,
        TaskDialogIcon icon = null,
        DialogResult defaultButton = DialogResult.Retry)
    {
        TaskDialogButtonCollection buttons = CreateButtons(MessageBoxButtons.AbortRetryIgnore);
        TaskDialogButton buttonResult = ShowDialogWithButtons(owner, caption, heading, text, icon ?? QuestionIcon, buttons, defaultButton);
        DialogResult result = buttonResult.Tag is DialogResult dr ? dr : defaultButton;

        return result;
    }

    /// <summary>
    /// Invoke TaskDialog to ask a question with Abort, Retry, Ignore, and Ignore All options.
    /// </summary>
    /// <param name="owner"> control or form. </param>
    /// <param name="caption"> text for dialog caption. </param>
    /// <param name="heading"> text for dialog heading. </param>
    /// <param name="text"> The text. </param>
    /// <param name="icon"> (Optional) Icon to display. If null, TaskDialogIcon.Information will be used. </param>
    /// <param name="defaultButton"> (Optional) Button to focus. </param>
    /// <returns>
    /// A tuple where Item1 is a DialogResult: Abort, Retry, or Ignore (used for both Ignore and Ignore All),
    /// and Item2 is a boolean indicating whether the user selected Ignore All (true) or not (false).
    /// </returns>
    public static (DialogResult Result, bool IsIgnoreAll) QuestionAbortRetryIgnoreIgnoreAll(
        IWin32Window owner,
        string caption,
        string heading,
        string text,
        TaskDialogIcon icon = null,
        DialogResult defaultButton = DialogResult.Retry)
    {
        // Custom button for Ignore All (reuses DialogResult.Ignore tag)
        TaskDialogButtonCollection buttons = CreateButtons(
            ("Abort", DialogResult.Abort),
            ("Retry", DialogResult.Retry),
            ("Ignore", DialogResult.Ignore),
            ("Ignore All", DialogResult.Ignore));
        TaskDialogButton ignoreAllButton = buttons[3];
        TaskDialogButton buttonResult = ShowDialogWithButtons(owner, caption, heading, text, icon ?? QuestionIcon, buttons, defaultButton);
        bool isIgnoreAll = ReferenceEquals(buttonResult, ignoreAllButton);
        DialogResult result = buttonResult.Tag is DialogResult dr ? dr : defaultButton;

        return (result, isIgnoreAll);
    }

    /// <summary>
    /// Displays a dialog with Abort, Retry, Ignore buttons and a customizable checkbox.
    /// </summary>
    /// <param name="owner">The window that owns this dialog.</param>
    /// <param name="caption">The window caption.</param>
    /// <param name="heading">The heading text of the dialog.</param>
    /// <param name="text">The main message body text.</param>
    /// <param name="checkboxText">Text to display next to the checkbox.</param>
    /// <param name="initiallyChecked">Whether the checkbox should be initially checked.</param>
    /// <param name="icon">Optional icon for the dialog (defaults to QuestionIcon).</param>
    /// <param name="defaultButton">Default selected button (defaults to Retry).</param>
    /// <returns>
    /// A tuple containing the <see cref="DialogResult"/> of the button pressed and a <see cref="bool"/>
    /// indicating whether the checkbox was checked when the dialog was closed.
    /// </returns>
    public static (DialogResult Result, bool ApplyIgnoreToAll) QuestionAbortRetryIgnoreWithCheckbox(
        IWin32Window owner,
        string caption,
        string heading,
        string text,
        string checkboxText = "Ignore all subsequent errors",
        bool initiallyChecked = false,
        TaskDialogIcon icon = null,
        DialogResult defaultButton = DialogResult.Retry)
    {
        TaskDialogButtonCollection buttons = CreateButtons(MessageBoxButtons.AbortRetryIgnore);
        TaskDialogVerificationCheckBox verificationCheckBox = new(checkboxText)
        {
            Checked = initiallyChecked
        };
        TaskDialogButton buttonResult = ShowDialogWithButtons(
            owner,
            caption,
            heading,
            text,
            icon ?? QuestionIcon,
            buttons,
            defaultButton,
            allowCancel: false,
            verification: verificationCheckBox);
        bool applyIgnoreToAll = verificationCheckBox.Checked;
        DialogResult result = buttonResult.Tag is DialogResult dr ? dr : defaultButton;

        return (result, applyIgnoreToAll);
    }
    #endregion // Question_methods

    #region Information_methods

    /// <summary> TaskDialog displays a message with option to assign button text. </summary>
    /// <param name="owner"> control or form. </param>
    /// <param name="caption"> text for dialog caption. </param>
    /// <param name="heading"> . </param>
    /// <param name="text"> The text. </param>
    /// <param name="buttonText"> (Optional). If null, "OK" will be used. </param>
    /// <param name="icon"> (Optional) Icon to display. If null, TaskDialogIcon.Information will be used. </param>
    public static void Information(
        IWin32Window owner,
        string caption,
        string heading,
        string text,
        string buttonText = null,
        TaskDialogIcon icon = null)
    {
        TaskDialogButtonCollection buttons = CreateButtons((buttonText ?? "OK", DialogResult.OK));
        TaskDialogIcon tdIcon = icon ?? TaskDialogIcon.Information;

        ShowDialogWithButtons(owner, caption ?? "Information", heading, text, tdIcon, buttons, defaultButton: DialogResult.OK);
    }
    #endregion // Information_methods
    #endregion // Public Methods

    #region Internal Methods

    /// <summary>
    /// Creates a <see cref="TaskDialogButtonCollection"/> from the specified button definitions.
    /// </summary>
    /// <param name="buttonDefs"> An array of tuples where each tuple contains the button text and the
    /// corresponding <see cref="DialogResult"/> tag. </param>
    /// <returns> A collection populated with buttons configured with the given text and associated tag. </returns>
    internal static TaskDialogButtonCollection CreateButtons(
        params (string Text, DialogResult Tag)[] buttonDefs)
    {
        TaskDialogButtonCollection buttons = [];

        foreach (var (Text, Tag) in buttonDefs)
        {
            buttons.Add(new TaskDialogButton(Text) { Tag = Tag });
        }

        return buttons;
    }

    /// <summary> Creates a <see cref="TaskDialogButtonCollection"/> based on the specified <see cref="MessageBoxButtons"/>. </summary>
    /// <param name="buttons"> The standard MessageBoxButtons value to translate into task dialog buttons. </param>
    /// <returns> A <see cref="TaskDialogButtonCollection"/> representing the specified button configuration. </returns>
    internal static TaskDialogButtonCollection CreateButtons(MessageBoxButtons buttons)
    {
        return CreateButtons(buttons, DialogResult.None).Buttons;
    }

    /// <summary>
    /// Creates a collection of <see cref="TaskDialogButton"/>s based on the specified <see cref="MessageBoxButtons"/>
    /// value, and determines the appropriate default <see cref="DialogResult"/> for the dialog.
    /// </summary>
    /// <exception cref="ArgumentException"> Thrown if <paramref name="requestedDefault"/> does not match
    /// any of the valid results implied by <paramref name="buttons"/>. </exception>
    /// 
    /// <param name="buttons"> Specifies the combination of buttons to include in the dialog. </param>
    /// <param name="requestedDefault"> The requested default button to be preselected. If set to <see cref="DialogResult.None"/>,
    /// the first button in the collection will be used as default. </param>
    /// <returns>
    /// A tuple containing:
    /// - <see cref="TaskDialogButtonCollection"/>: the constructed list of buttons for the dialog,
    /// - <see cref="DialogResult"/>: the validated default result to use in <see cref="TaskDialogPage.DefaultButton"/>.
    /// </returns>
    internal static (TaskDialogButtonCollection Buttons, DialogResult Default) CreateButtons(
        MessageBoxButtons buttons,
        DialogResult requestedDefault)
    {
        DialogResult firstButton = DialogResult.None;
        List<DialogResult> validResults = [];
        TaskDialogButtonCollection taskButtons = [];

        void AddButton(string text, DialogResult buttonResult)
        {
            taskButtons.Add(new TaskDialogButton(text) { Tag = buttonResult });
            if (firstButton == DialogResult.None)
                firstButton = buttonResult;
            validResults.Add(buttonResult);
        }

        switch (buttons)
        {
            case MessageBoxButtons.OK:
                AddButton("OK", DialogResult.OK);
                break;
            case MessageBoxButtons.OKCancel:
                AddButton("OK", DialogResult.OK);
                AddButton("Cancel", DialogResult.Cancel);
                break;
            case MessageBoxButtons.YesNo:
                AddButton("Yes", DialogResult.Yes);
                AddButton("No", DialogResult.No);
                break;
            case MessageBoxButtons.YesNoCancel:
                AddButton("Yes", DialogResult.Yes);
                AddButton("No", DialogResult.No);
                AddButton("Cancel", DialogResult.Cancel);
                break;
            case MessageBoxButtons.RetryCancel:
                AddButton("Retry", DialogResult.Retry);
                AddButton("Cancel", DialogResult.Cancel);
                break;
            case MessageBoxButtons.AbortRetryIgnore:
                AddButton("Abort", DialogResult.Abort);
                AddButton("Retry", DialogResult.Retry);
                AddButton("Ignore", DialogResult.Ignore);
                break;
        }

        // Validate or fallback
        DialogResult effectiveDefault;
        if (requestedDefault == DialogResult.None)
        {
            effectiveDefault = firstButton;
        }
        else if (!validResults.Contains(requestedDefault))
        {
            throw new ArgumentException(
                $"The specified default button '{requestedDefault}' is not valid for MessageBoxButtons '{buttons}'.",
                nameof(requestedDefault));
        }
        else
        {
            effectiveDefault = requestedDefault;
        }

        return (taskButtons, effectiveDefault);
    }
    #endregion // Internal Methods

    #region Private Methods

    private static TaskDialogButton FindButton(
        IEnumerable<TaskDialogButton> buttons,
        DialogResult buttonTag)
    {
        return buttons.FirstOrDefault(x => x.Tag is DialogResult dr && dr == buttonTag);
    }

    private static TaskDialogButton ShowDialogWithButtons(
        IWin32Window owner,
        string caption,
        string heading,
        string text,
        TaskDialogIcon icon,
        TaskDialogButtonCollection buttons,
        DialogResult defaultButton,
        bool allowCancel = false,
        TaskDialogVerificationCheckBox verification = null)
    {
        TaskDialogPage page = new()
        {
            Caption = caption,
            SizeToContent = true,
            Heading = heading,
            Text = text,
            Icon = icon,
            Buttons = buttons,
            DefaultButton = FindButton(buttons, defaultButton),
            AllowCancel = allowCancel,
            Verification = verification
        };

        return TaskDialog.ShowDialog(owner, page);
    }
    #endregion // Private Methods
}

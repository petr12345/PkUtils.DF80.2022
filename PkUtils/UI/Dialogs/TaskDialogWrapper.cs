// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PK.PkUtils.UI.Utils;

namespace PK.PkUtils.UI.Dialogs;

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

        TaskDialogButton yesButton = new(yesText) { Tag = DialogResult.Yes };
        TaskDialogButton noButton = new(noText) { Tag = DialogResult.No };

        var buttons = new TaskDialogButtonCollection();

        if (defaultButton == DialogResult.Yes)
        {
            buttons.Add(yesButton);
            buttons.Add(noButton);
        }
        else
        {
            buttons.Add(noButton);
            buttons.Add(yesButton);
        }

        TaskDialogPage page = new()
        {
            Caption = caption,
            SizeToContent = true,
            Heading = heading,
            Icon = QuestionIcon,
            Buttons = buttons,
            DefaultButton = FindButton(buttons, defaultButton)
        };

        TaskDialogButton buttonResult = TaskDialog.ShowDialog(owner, page);
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
        TaskDialogButton yesButton = new("Yes") { Tag = DialogResult.Yes };
        TaskDialogButton noButton = new("No") { Tag = DialogResult.No };

        var buttons = new TaskDialogButtonCollection();

        if (defaultButton == DialogResult.Yes)
        {
            buttons.Add(yesButton);
            buttons.Add(noButton);
        }
        else
        {
            buttons.Add(noButton);
            buttons.Add(yesButton);
        }

        TaskDialogPage page = new()
        {
            Caption = caption,
            SizeToContent = true,
            Heading = heading,
            Text = text,
            Icon = new TaskDialogIcon(icon),
            Buttons = buttons,
            DefaultButton = FindButton(buttons, defaultButton)
        };

        TaskDialogButton buttonResult = TaskDialog.ShowDialog(owner, page);
        bool result = buttonResult.Tag is DialogResult dr && dr == DialogResult.Yes;

        return result;
    }

    /// <summary> Invoke Task Dialog to ask a question. </summary>
    /// <remarks>
    /// Dialogs.Question(this, "Ask something", YesMethod, NoMethod);
    /// 
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
        TaskDialogButton yesButton = new("Yes") { Tag = DialogResult.Yes };
        TaskDialogButton noButton = new("No") { Tag = DialogResult.No };
        var buttons = new TaskDialogButtonCollection
        {
            yesButton,
            noButton
        };

        TaskDialogPage page = new()
        {
            Caption = caption,
            SizeToContent = true,
            Heading = heading,
            Text = text,
            Icon = icon ?? QuestionIcon,
            Buttons = buttons
        };

        TaskDialogButton buttonResult = TaskDialog.ShowDialog(owner, page);
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
        TaskDialogButton yesButton = new("Yes") { Tag = DialogResult.Yes };
        TaskDialogButton noButton = new("No") { Tag = DialogResult.No };
        TaskDialogButton cancelButton = new("Cancel") { Tag = DialogResult.Cancel };
        TaskDialogButtonCollection buttons = [yesButton, noButton, cancelButton];

        TaskDialogPage page = new()
        {
            Caption = caption,
            SizeToContent = true,
            Heading = heading,
            Text = text,
            Icon = QuestionIcon,
            Buttons = buttons,
            DefaultButton = FindButton(buttons, defaultButton),
            AllowCancel = true,
        };

        TaskDialogButton buttonResult = TaskDialog.ShowDialog(owner, page);
        DialogResult result = buttonResult.Tag is DialogResult dr ? dr : DialogResult.Cancel;

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
        TaskDialogButton abortButton = new("Abort") { Tag = DialogResult.Abort };
        TaskDialogButton retryButton = new("Retry") { Tag = DialogResult.Retry };
        TaskDialogButton ignoreButton = new("Ignore") { Tag = DialogResult.Ignore };
        TaskDialogButtonCollection buttons = [abortButton, retryButton, ignoreButton];

        TaskDialogPage page = new()
        {
            Caption = caption,
            SizeToContent = true,
            Heading = heading,
            Text = text,
            Icon = icon ?? QuestionIcon,
            Buttons = buttons,
            DefaultButton = FindButton(buttons, defaultButton),
            AllowCancel = false,
        };

        TaskDialogButton buttonResult = TaskDialog.ShowDialog(owner, page);
        return buttonResult.Tag is DialogResult dr ? dr : DialogResult.Ignore;
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
        TaskDialogButton okayButton = new(buttonText ?? "OK");

        TaskDialogPage page = new()
        {
            Caption = caption ?? "Information",
            SizeToContent = true,
            Heading = heading,
            Text = text,
            Icon = icon ?? TaskDialogIcon.Information,
            Buttons = [okayButton]
        };

        TaskDialog.ShowDialog(owner, page);
    }
    #endregion // Information_methods
    #endregion // Public Methods

    #region Private Methods

    private static TaskDialogButton FindButton(IEnumerable<TaskDialogButton> buttons, DialogResult buttonTag)
    {
        return buttons.FirstOrDefault(x => x.Tag is DialogResult dr && dr == buttonTag);
    }
    #endregion // Private Methods
}

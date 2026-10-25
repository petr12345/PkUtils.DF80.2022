using System;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.UI.Dialogs.TaskDialogs;

namespace PK.TestTaskDialog;

/// <summary>Demonstrates the modern TaskDialogWrapper API.</summary>
public partial class TestForm : Form
{
    #region Constructor(s)
    public TestForm()
    {
        InitializeComponent();
    }
    #endregion // Constructor(s)

    #region Event handlers
    private void OnInformationClick(object? sender, EventArgs e)
    {
        TaskDialogWrapper.Information(this, "Information", "Information dialog",
            "This is a standard information dialog.");
        lblResult.Text = "Information dialog closed.";
    }

    private void OnQuestionClick(object? sender, EventArgs e)
    {
        bool accepted = TaskDialogWrapper.Question(this, "Question",
            "Do you want to continue?", "Demonstrates the standard question dialog.", SystemIcons.Question);
        lblResult.Text = $"Question result: {accepted}";
    }

    private void OnAutoCloseNotificationClick(object? sender, EventArgs e)
    {
        TaskDialogWrapper.ShowAutoCloseNotification(this, SystemIcons.Information,
            "This notification will close automatically after 5 seconds.", 5);
        lblResult.Text = "Notification closed (manually or after timeout).";
    }

    private void OnAutoCloseConfirmationClick(object? sender, EventArgs e)
    {
        bool continueOperation = TaskDialogWrapper.ShowAutoCloseConfirmation(this,
            "Continue processing?", 10,
            "Click Continue or Stop. After 10 seconds, Continue is selected automatically.");
        lblResult.Text = $"Continue processing: {continueOperation}";
    }
    #endregion // Event handlers
}

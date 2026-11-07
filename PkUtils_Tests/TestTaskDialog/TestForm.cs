using System;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.UI.Dialogs.TaskDialogs;

namespace PK.TestTaskDialog;

/// <summary>Demonstrates the modern TaskDialogWrapper API.</summary>
public partial class TestForm : Form
{
    private readonly MeasuredSpacesExperiment _measuredExperiment;
    #region Constructor(s)
    public TestForm()
    {
        InitializeComponent();
        _measuredExperiment = new MeasuredSpacesExperiment(
            this, lblResult, cboMeasuredDataset, nudMeasuredGap, nudMeasuredMaxWidth,
            cboMeasuredTruncation, txtMeasuredOutput, txtMeasuredDiagnostics);
        _measuredExperiment.RebuildPreview();
    }
    #endregion // Constructor(s)

    #region Measured-space experiment event handlers
    // Keep WinForms event handlers on the Form; all experiment logic lives in the plain helper class.
    private void OnMeasuredOptionsChanged(object? sender, EventArgs e) => _measuredExperiment?.RebuildPreview();

    private void OnShowMeasuredClick(object? sender, EventArgs e) => _measuredExperiment.ShowDialog();

    private void OnCopyMeasuredClick(object? sender, EventArgs e) => _measuredExperiment.CopyTextAndDiagnostics();
    #endregion // Measured-space experiment event handlers

    #region Event handlers
    private void OnInformationClick(object sender, EventArgs e)
    {
        TaskDialogWrapper.Information(this, "Information", "Information dialog",
            "This is a standard information dialog.");
        lblResult.Text = "Information dialog closed.";
    }

    private void OnQuestionClick(object sender, EventArgs e)
    {
        bool accepted = TaskDialogWrapper.Question(this, "Question",
            "Do you want to continue?", "Demonstrates the standard question dialog.", SystemIcons.Question);
        lblResult.Text = $"Question result: {accepted}";
    }
    /// <summary>
    /// Tests the existing QuestionYesNoCancel implementation without modifying the wrapper.
    /// In particular, closing the dialog using X or Esc should return Cancel.
    /// </summary>
    private void OnQuestionYesNoCancelClick(object? sender, EventArgs e)
    {
        DialogResult result = TaskDialogWrapper.QuestionYesNoCancel(
            this,
            "QuestionYesNoCancel test",
            "Which result will be returned?",
            "Choose Yes, No, or Cancel; alternatively, close this dialog with X or Esc. " +
            "Both X and Esc should return DialogResult.Cancel.",
            defaultButton: DialogResult.Yes);

        lblResult.Text = $"QuestionYesNoCancel returned: {result} ({(int)result})";
        System.Diagnostics.Debug.WriteLine(
            $"QuestionYesNoCancel returned: {result} ({(int)result})");
    }

    private void OnAutoCloseNotificationClick(object? sender, EventArgs e)
    {
        string GetRemainingTimeText(int seconds)
        {
            return $"Closing automatically in {seconds} seconds.";
        }
        TaskDialogWrapper.ShowAutoCloseNotification(
            this,
            SystemIcons.Information,
            "This notification will close automatically.",
            5,
            remainingTimeText: GetRemainingTimeText,
            countdownPlacement: TaskDialogWrapper.AutoCloseCountdownPlacement.Text);

        lblResult.Text = "Notification closed (manually or after timeout).";
    }

    private void OnAutoCloseConfirmationClick(object? sender, EventArgs e)
    {
        string GetRemainingTimeText(int seconds)
        {
            return $"Continue processing? ({seconds} seconds remaining)";
        }

        bool continueOperation = TaskDialogWrapper.ShowAutoCloseConfirmation(
            this,
            "Continue processing?",
            10,
            "Click Continue or Stop. If the timeout expires, Continue is selected automatically.",
            remainingTimeText: GetRemainingTimeText,
            countdownPlacement: TaskDialogWrapper.AutoCloseCountdownPlacement.Heading);

        lblResult.Text = $"Continue processing: {continueOperation}";
    }
    #endregion // Event handlers
}

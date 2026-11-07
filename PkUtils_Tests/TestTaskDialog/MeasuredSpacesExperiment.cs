using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PK.PkUtils.UI.Dialogs.TaskDialogs;

namespace PK.TestTaskDialog;

/// <summary>Coordinates the measured-space TaskDialog experiment, including its preview, test datasets, and diagnostics.</summary>
internal sealed class MeasuredSpacesExperiment
{
    #region Fields_GUI

    private readonly Form _owner;
    private readonly Label _resultLabel;
    private readonly ComboBox cboMeasuredDataset;
    private readonly NumericUpDown nudMeasuredGap;
    private readonly NumericUpDown nudMeasuredMaxWidth;
    private readonly ComboBox cboMeasuredTruncation;
    private readonly TextBox txtMeasuredOutput;
    private readonly TextBox txtMeasuredDiagnostics;
    #endregion // Fields_GUI

    #region Fields_PresentedData

    private string _measuredGeneratedText = string.Empty;
    private string _measuredGeneratedDiagnostics = string.Empty;
    private string _measuredOriginalValues = string.Empty;
    #endregion // Fields_PresentedData

    #region Constructor(s)

    /// <summary>Initializes the experiment with the owning form and controls used to configure and display its results.</summary>
    /// <param name="owner">Form that owns the TaskDialog and provides the drawing context for text measurements.</param>
    /// <param name="resultLabel">Label in which to display the selected dialog result.</param>
    /// <param name="dataset">Selector for the comparison data to display.</param>
    /// <param name="columnGap">Minimum gap between the measured text columns, in pixels.</param>
    /// <param name="maxWidth">Maximum target width of the formatted comparison, in pixels.</param>
    /// <param name="truncation">Selector for the strategy used to shorten values that do not fit.</param>
    /// <param name="preview">Text box displaying the formatted comparison before opening the TaskDialog.</param>
    /// <param name="diagnostics">Text box displaying measurements, truncation details, and the dialog result.</param>
    internal MeasuredSpacesExperiment(
        Form owner,
        Label resultLabel,
        ComboBox dataset,
        NumericUpDown columnGap,
        NumericUpDown maxWidth,
        ComboBox truncation,
        TextBox preview,
        TextBox diagnostics)
    {
        _owner = owner;
        _resultLabel = resultLabel;
        cboMeasuredDataset = dataset;
        nudMeasuredGap = columnGap;
        nudMeasuredMaxWidth = maxWidth;
        cboMeasuredTruncation = truncation;
        txtMeasuredOutput = preview;
        txtMeasuredDiagnostics = diagnostics;
    }
    #endregion // Constructor(s)

    #region Methods

    #region Public_Methods

    /// <summary>
    /// Rebuilds the aligned text from the currently selected dataset and formatting options,
    /// updates the preview, and reports measurement errors in the diagnostics control.
    /// </summary>
    internal void RebuildPreview()
    {
        try
        {
            var rows = GetMeasuredRows(cboMeasuredDataset.SelectedIndex);
            // System dialog font is the closest available approximation to TaskDialog's DirectUI font.
            using Graphics graphics = _owner.CreateGraphics();
            MeasuredSpaceTableFormatter.FormatResult result = MeasuredSpaceTableFormatter.Format(
                graphics, SystemFonts.MessageBoxFont, rows,
                "Setting", "Ribbon", "Project", (int)nudMeasuredGap.Value,
                (int)nudMeasuredMaxWidth.Value,
                (MeasuredTextTruncationMode)Math.Max(0, cboMeasuredTruncation.SelectedIndex));

            _measuredGeneratedText = result.Text;
            _measuredOriginalValues = result.FullValues;
            _measuredGeneratedDiagnostics =
                $"Font: {SystemFonts.MessageBoxFont.Name}, {SystemFonts.MessageBoxFont.SizeInPoints:0.##} pt; " +
                $"DPI: {graphics.DpiX:0.#} x {graphics.DpiY:0.#}\r\n" + result.Diagnostics;
            txtMeasuredOutput.Text = _measuredGeneratedText;
            txtMeasuredDiagnostics.Text = _measuredGeneratedDiagnostics;
        }
        catch (Exception ex)
        {
            _measuredGeneratedText = string.Empty;
            _measuredOriginalValues = string.Empty;
            _measuredGeneratedDiagnostics = ex.ToString();
            txtMeasuredOutput.Text = string.Empty;
            txtMeasuredDiagnostics.Text = _measuredGeneratedDiagnostics;
        }
    }

    /// <summary>
    /// Displays the current comparison in the expanded section of a TaskDialog
    /// and records which dialog button the user selected.
    /// </summary>
    internal void ShowDialog()
    {
        RebuildPreview();
        if (string.IsNullOrEmpty(_measuredGeneratedText))
            return;

        DialogResult result = TaskDialogWrapper.QuestionYesNoCancel(
            _owner,
            "TaskDialog measured ordinary spaces",
            "Choose Ribbon or Project settings",
            "Compare the alignment of the columns in the expanded information.",
            moreDetails: _measuredGeneratedText,
            defaultButton: DialogResult.Yes);

        _resultLabel.Text = $"MeasuredSpaces result: {result}";
        txtMeasuredDiagnostics.AppendText($"\r\nDialog result: {result}");
    }

    /// <summary>
    /// Copies the formatted preview, full untruncated values, and measurement diagnostics
    /// to the clipboard for inspection or comparison.
    /// </summary>
    internal void CopyTextAndDiagnostics()
    {
        RebuildPreview();

        string[] lines = [
            _measuredGeneratedText,
            string.Empty,
            "Full original values:",
            _measuredOriginalValues,
            string.Empty,
            _measuredGeneratedDiagnostics
        ];
        string clipboardText = string.Join(Environment.NewLine, lines);

        Clipboard.SetText(clipboardText);
    }
    #endregion Public_Methods

    #region Private_Methods

    /// <summary>Returns the selected dataset for evaluating column alignment and truncation.</summary>
    /// <param name="dataset">Selected dataset index; unrecognized values use the standard ACM comparison.</param>
    /// <returns>Setting names with corresponding Ribbon and Project values.</returns>
    private static IReadOnlyList<(string Setting, string Ribbon, string Project)> GetMeasuredRows(int dataset)
    {
        switch (dataset)
        {
            case 1:
                return new (string, string, string)[]
                {
                    ("III / WWW", "iiiii", "WWWWW"),
                    ("Minimum", "iiiiiiii", "MMMMMMMM"),
                    ("Wide labels WMWM", "111111", "888888"),
                    ("Punctuation !.,", "YES", "No")
                };
            case 2:
                return new (string, string, string)[]
                {
                    ("Project name", "Projekt-Žluťoučký-Příliš-Dlouhý-Název", "Project"),
                    ("Output folder", @"D:\Projects\Automation\Generated\LongFolderName\Results", @"C:\Tmp\Výstup"),
                    ("Create at least a script", "On", "Off"),
                    ("Safe cancel", "On", "Off")
                };
            case 3:
                return new (string, string, string)[]
                {
                    ("Output folder", @"C:\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa12", @"C:\aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa34"),
                    ("Project name", "CompletelyDifferentRibbonProjectName-Long", "AnotherIndependentProjectName-Long"),
                    ("Create at least a script", "On", "Off")
                };
            default:
                return new (string, string, string)[]
                {
                    ("Project name", "Project1", "Project"),
                    ("Output folder", @"D:\Tmp222", @"D:\Tmp2"),
                    ("Create at least a script", "On", "Off"),
                    ("Safe cancel", "On", "Off")
                };
        }
    }
    #endregion // Private_Methods
    #endregion // Methods
}

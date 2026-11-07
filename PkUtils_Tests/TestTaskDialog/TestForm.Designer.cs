using System.Drawing;
using System.Windows.Forms;

namespace PK.TestTaskDialog;

partial class TestForm
{
    #region Windows Form Designer generated code
    private Button btnInformation = null!;
    private Button btnQuestion = null!;
    private Button btnAutoCloseNotification = null!;
    private Button btnAutoCloseConfirmation = null!;
    private Button btnQuestionYesNoCancel = null!;
    private Label lblResult = null!;
    private GroupBox grpMeasured = null!;
    private ComboBox cboMeasuredDataset = null!;
    private NumericUpDown nudMeasuredGap = null!;
    private NumericUpDown nudMeasuredMaxWidth = null!;
    private ComboBox cboMeasuredTruncation = null!;
    private Button btnShowMeasured = null!;
    private Button btnCopyMeasured = null!;
    private TextBox txtMeasuredOutput = null!;
    private TextBox txtMeasuredDiagnostics = null!;

    private void InitializeComponent()
    {
        btnInformation = new Button();
        btnQuestion = new Button();
        btnAutoCloseNotification = new Button();
        btnAutoCloseConfirmation = new Button();
        btnQuestionYesNoCancel = new Button();
        lblResult = new Label();
        grpMeasured = new GroupBox();
        cboMeasuredDataset = new ComboBox();
        nudMeasuredGap = new NumericUpDown();
        nudMeasuredMaxWidth = new NumericUpDown();
        cboMeasuredTruncation = new ComboBox();
        btnShowMeasured = new Button();
        btnCopyMeasured = new Button();
        txtMeasuredOutput = new TextBox();
        txtMeasuredDiagnostics = new TextBox();
        ((System.ComponentModel.ISupportInitialize)nudMeasuredGap).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nudMeasuredMaxWidth).BeginInit();
        grpMeasured.SuspendLayout();
        SuspendLayout();
        // Standard TaskDialog examples
        btnInformation.Location = new Point(24, 24);
        btnInformation.Size = new Size(240, 36);
        btnInformation.Text = "Information";
        btnInformation.Click += OnInformationClick;
        btnQuestion.Location = new Point(280, 24);
        btnQuestion.Size = new Size(240, 36);
        btnQuestion.Text = "Question";
        btnQuestion.Click += OnQuestionClick;
        // Automatic closing examples
        btnAutoCloseNotification.Location = new Point(24, 76);
        btnAutoCloseNotification.Size = new Size(240, 36);
        btnAutoCloseNotification.Text = "Auto-close notification (5 s)";
        btnAutoCloseNotification.Click += OnAutoCloseNotificationClick;
        btnAutoCloseConfirmation.Location = new Point(280, 76);
        btnAutoCloseConfirmation.Size = new Size(240, 36);
        btnAutoCloseConfirmation.Text = "Auto-close confirmation (10 s)";
        btnAutoCloseConfirmation.Click += OnAutoCloseConfirmationClick;
        // Yes / No / Cancel result test (including system Close button and Esc)
        btnQuestionYesNoCancel.Location = new Point(24, 128);
        btnQuestionYesNoCancel.Size = new Size(496, 36);
        btnQuestionYesNoCancel.Text = "QuestionYesNoCancel — test returned DialogResult";
        btnQuestionYesNoCancel.Click += OnQuestionYesNoCancelClick;
        lblResult.Location = new Point(24, 183);
        lblResult.Size = new Size(496, 65);
        lblResult.Text = "Choose a demonstration above.";
        lblResult.AutoSize = false;
        lblResult.BorderStyle = BorderStyle.FixedSingle;
        lblResult.Padding = new Padding(8);
        // Measured-space column alignment experiment (no TaskDialog font manipulation).
        grpMeasured.Text = "TaskDialog pixel-measured ordinary spaces";
        grpMeasured.Location = new Point(544, 18);
        grpMeasured.Size = new Size(550, 520);
        grpMeasured.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        Label datasetLabel = new() { Text = "Dataset:", Location = new Point(16, 34), AutoSize = true };
        cboMeasuredDataset.Location = new Point(105, 30);
        cboMeasuredDataset.Size = new Size(423, 26);
        cboMeasuredDataset.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMeasuredDataset.Items.AddRange(new object[]
        {
            "ACM settings (4 differences)",
            "Variable-width characters",
            "Long paths and Unicode",
            "Shared prefix vs different values"
        });
        cboMeasuredDataset.SelectedIndex = 0;
        cboMeasuredDataset.SelectedIndexChanged += OnMeasuredOptionsChanged;
        Label gapLabel = new() { Text = "Column gap (px):", Location = new Point(16, 70), AutoSize = true };
        nudMeasuredGap.Location = new Point(134, 66);
        nudMeasuredGap.Size = new Size(88, 26);
        nudMeasuredGap.Minimum = 4;
        nudMeasuredGap.Maximum = 80;
        nudMeasuredGap.Value = 20;
        nudMeasuredGap.ValueChanged += OnMeasuredOptionsChanged;
        Label maxWidthLabel = new() { Text = "Max table width (px):", Location = new Point(240, 70), AutoSize = true };
        nudMeasuredMaxWidth.Location = new Point(384, 66);
        nudMeasuredMaxWidth.Size = new Size(144, 26);
        nudMeasuredMaxWidth.Minimum = 200;
        nudMeasuredMaxWidth.Maximum = 1200;
        nudMeasuredMaxWidth.Increment = 10;
        nudMeasuredMaxWidth.Value = 420;
        nudMeasuredMaxWidth.ValueChanged += OnMeasuredOptionsChanged;
        Label truncationLabel = new() { Text = "Truncation:", Location = new Point(16, 109), AutoSize = true };
        cboMeasuredTruncation.Location = new Point(105, 105);
        cboMeasuredTruncation.Size = new Size(423, 26);
        cboMeasuredTruncation.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMeasuredTruncation.Items.AddRange(new object[]
        {
            "None — preserve full values",
            "TruncateEnd — cut the ends",
            "Smart — keep differing endings if values share a prefix"
        });
        cboMeasuredTruncation.SelectedIndex = 2;
        cboMeasuredTruncation.SelectedIndexChanged += OnMeasuredOptionsChanged;
        btnShowMeasured.Text = "Show in TaskDialog";
        btnShowMeasured.Location = new Point(16, 147);
        btnShowMeasured.Size = new Size(245, 36);
        btnShowMeasured.Click += OnShowMeasuredClick;
        btnCopyMeasured.Text = "Copy text and diagnostics";
        btnCopyMeasured.Location = new Point(283, 147);
        btnCopyMeasured.Size = new Size(245, 36);
        btnCopyMeasured.Click += OnCopyMeasuredClick;
        Label previewLabel = new() { Text = "Generated text (monospaced preview only):", Location = new Point(16, 198), AutoSize = true };
        txtMeasuredOutput.Location = new Point(16, 222);
        txtMeasuredOutput.Size = new Size(512, 134);
        txtMeasuredOutput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        txtMeasuredOutput.ReadOnly = true;
        txtMeasuredOutput.Multiline = true;
        txtMeasuredOutput.ScrollBars = ScrollBars.Both;
        txtMeasuredOutput.WordWrap = false;
        txtMeasuredOutput.Font = new Font("Consolas", 9F);
        Label diagnosticsLabel = new() { Text = "Measured positions / diagnostics:", Location = new Point(16, 369), AutoSize = true };
        txtMeasuredDiagnostics.Location = new Point(16, 393);
        txtMeasuredDiagnostics.Size = new Size(512, 110);
        txtMeasuredDiagnostics.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
        txtMeasuredDiagnostics.Multiline = true;
        txtMeasuredDiagnostics.ReadOnly = true;
        txtMeasuredDiagnostics.ScrollBars = ScrollBars.Both;
        txtMeasuredDiagnostics.WordWrap = false;
        txtMeasuredDiagnostics.Font = new Font("Consolas", 9F);
        grpMeasured.Controls.AddRange(new Control[]
        {
            datasetLabel, cboMeasuredDataset, gapLabel, nudMeasuredGap, maxWidthLabel, nudMeasuredMaxWidth,
            truncationLabel, cboMeasuredTruncation,
            btnShowMeasured, btnCopyMeasured, previewLabel, txtMeasuredOutput,
            diagnosticsLabel, txtMeasuredDiagnostics
        });
        // Form
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1114, 555);
        Controls.Add(btnInformation);
        Controls.Add(btnQuestion);
        Controls.Add(btnAutoCloseNotification);
        Controls.Add(btnAutoCloseConfirmation);
        Controls.Add(btnQuestionYesNoCancel);
        Controls.Add(lblResult);
        Controls.Add(grpMeasured);
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(1130, 594);
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "TaskDialogWrapper demo";
        grpMeasured.ResumeLayout(false);
        grpMeasured.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)nudMeasuredGap).EndInit();
        ((System.ComponentModel.ISupportInitialize)nudMeasuredMaxWidth).EndInit();
        ResumeLayout(false);
    }
    #endregion // Windows Form Designer generated code
}

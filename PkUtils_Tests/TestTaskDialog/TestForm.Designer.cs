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
    private Label lblResult = null!;

    private void InitializeComponent()
    {
        btnInformation = new Button();
        btnQuestion = new Button();
        btnAutoCloseNotification = new Button();
        btnAutoCloseConfirmation = new Button();
        lblResult = new Label();
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
        lblResult.Location = new Point(24, 139);
        lblResult.Size = new Size(496, 65);
        lblResult.Text = "Choose a demonstration above.";
        lblResult.AutoSize = false;
        lblResult.BorderStyle = BorderStyle.FixedSingle;
        lblResult.Padding = new Padding(8);
        // Form
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(544, 225);
        Controls.Add(btnInformation);
        Controls.Add(btnQuestion);
        Controls.Add(btnAutoCloseNotification);
        Controls.Add(btnAutoCloseConfirmation);
        Controls.Add(lblResult);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "TaskDialogWrapper demo";
        ResumeLayout(false);
    }
    #endregion // Windows Form Designer generated code
}

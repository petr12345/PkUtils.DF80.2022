namespace TestMultiSelectTreeView;

partial class AboutBox
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.Label _labelAssemblyTitle;
    private System.Windows.Forms.Label _labelVersion;
    private System.Windows.Forms.Label _labelCopyright;
    private System.Windows.Forms.TextBox _textBoxDescription;
    private System.Windows.Forms.Button _buttonOK;
    private System.Windows.Forms.Button _buttonMore;
    private System.Windows.Forms.LinkLabel _linkLabelWebsite;

    /// <summary>
    /// Disposes of the resources (other than memory) used by the <see cref="T:System.Windows.Forms.Form" />.
    /// </summary>
    /// <param name="disposing"> <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" />
    /// to release only unmanaged resources. </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        _labelAssemblyTitle = new Label();
        _labelVersion = new Label();
        _labelCopyright = new Label();
        _linkLabelWebsite = new LinkLabel();
        _textBoxDescription = new TextBox();
        _buttonOK = new Button();
        _buttonMore = new Button();
        SuspendLayout();
        // 
        // _labelAssemblyTitle
        // 
        _labelAssemblyTitle.AutoSize = true;
        _labelAssemblyTitle.Location = new Point(12, 9);
        _labelAssemblyTitle.Name = "_labelAssemblyTitle";
        _labelAssemblyTitle.Size = new Size(83, 15);
        _labelAssemblyTitle.TabIndex = 0;
        _labelAssemblyTitle.Text = "Assembly Title";
        // 
        // _labelVersion
        // 
        _labelVersion.AutoSize = true;
        _labelVersion.Location = new Point(12, 35);
        _labelVersion.Name = "_labelVersion";
        _labelVersion.Size = new Size(45, 15);
        _labelVersion.TabIndex = 1;
        _labelVersion.Text = "Version";
        // 
        // _labelCopyright
        // 
        _labelCopyright.AutoSize = true;
        _labelCopyright.Location = new Point(12, 61);
        _labelCopyright.Name = "_labelCopyright";
        _labelCopyright.Size = new Size(60, 15);
        _labelCopyright.TabIndex = 2;
        _labelCopyright.Text = "Copyright";
        // 
        // _linkLabelWebsite
        // 
        _linkLabelWebsite.AutoSize = true;
        _linkLabelWebsite.Location = new Point(12, 87);
        _linkLabelWebsite.Name = "_linkLabelWebsite";
        _linkLabelWebsite.Size = new Size(49, 15);
        _linkLabelWebsite.TabIndex = 3;
        _linkLabelWebsite.TabStop = true;
        _linkLabelWebsite.Text = "Website";
        // 
        // _textBoxDescription
        // 
        _textBoxDescription.Location = new Point(15, 127);
        _textBoxDescription.Multiline = true;
        _textBoxDescription.Name = "_textBoxDescription";
        _textBoxDescription.ReadOnly = true;
        _textBoxDescription.ScrollBars = ScrollBars.Vertical;
        _textBoxDescription.Size = new Size(488, 454);
        _textBoxDescription.TabIndex = 4;
        _textBoxDescription.Visible = false;
        // 
        // _buttonOK
        // 
        _buttonOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _buttonOK.Location = new Point(397, 47);
        _buttonOK.Name = "_buttonOK";
        _buttonOK.Size = new Size(106, 26);
        _buttonOK.TabIndex = 5;
        _buttonOK.Text = "OK";
        _buttonOK.UseVisualStyleBackColor = true;
        _buttonOK.Click += OnButtonOK_Click;
        // 
        // _buttonMore
        // 
        _buttonMore.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _buttonMore.Location = new Point(397, 12);
        _buttonMore.Name = "_buttonMore";
        _buttonMore.Size = new Size(106, 26);
        _buttonMore.TabIndex = 6;
        _buttonMore.Text = "More >>";
        _buttonMore.UseVisualStyleBackColor = true;
        _buttonMore.Click += OnButtonMore_Click;
        // 
        // AboutBox
        // 
        ClientSize = new Size(522, 593);
        Controls.Add(_buttonMore);
        Controls.Add(_buttonOK);
        Controls.Add(_textBoxDescription);
        Controls.Add(_linkLabelWebsite);
        Controls.Add(_labelCopyright);
        Controls.Add(_labelVersion);
        Controls.Add(_labelAssemblyTitle);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AboutBox";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Load += AboutBox_Load;
        ResumeLayout(false);
        PerformLayout();
    }
}

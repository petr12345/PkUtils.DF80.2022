// Ignore Spelling: Utils
// 

namespace PK.PkUtils.UI.Splash;

partial class SplashForm
{
    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    public void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this._timer = new System.Windows.Forms.Timer(this.components);
        this._smoothProgressBar = new PK.PkUtils.UI.Controls.SmoothProgressBar();
        this._lblTimeRemaining = new System.Windows.Forms.Label();
        this._lblStatus = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // _timer
        // 
        this._timer.Tick += new System.EventHandler(this.OnTimer_Tick);
        // 
        // _smoothProgressBar
        // 
        this._smoothProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this._smoothProgressBar.BackColor = System.Drawing.Color.Gray;
        this._smoothProgressBar.BarColorLeft = System.Drawing.Color.Blue;
        this._smoothProgressBar.BarColorText = System.Drawing.SystemColors.ActiveCaptionText;
        this._smoothProgressBar.BarText = "Progress text";
        this._smoothProgressBar.Location = new System.Drawing.Point(12, 228);
        this._smoothProgressBar.Maximum = 100;
        this._smoothProgressBar.Minimum = 0;
        this._smoothProgressBar.Name = "_smoothProgressBar";
        this._smoothProgressBar.Size = new System.Drawing.Size(453, 26);
        this._smoothProgressBar.TabIndex = 3;
        this._smoothProgressBar.Value = 50;
        // 
        // _lblTimeRemaining
        // 
        this._lblTimeRemaining.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this._lblTimeRemaining.BackColor = System.Drawing.Color.Transparent;
        this._lblTimeRemaining.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
        this._lblTimeRemaining.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
        this._lblTimeRemaining.ImeMode = System.Windows.Forms.ImeMode.NoControl;
        this._lblTimeRemaining.Location = new System.Drawing.Point(12, 259);
        this._lblTimeRemaining.Name = "_lblTimeRemaining";
        this._lblTimeRemaining.Size = new System.Drawing.Size(279, 16);
        this._lblTimeRemaining.TabIndex = 2;
        this._lblTimeRemaining.Text = "Time remaining";
        this._lblTimeRemaining.DoubleClick += new System.EventHandler(this.SplashScreen_DoubleClick);
        // 
        // _lblStatus
        // 
        this._lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this._lblStatus.BackColor = System.Drawing.Color.Transparent;
        this._lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
        this._lblStatus.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
        this._lblStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
        this._lblStatus.Location = new System.Drawing.Point(12, 210);
        this._lblStatus.Name = "_lblStatus";
        this._lblStatus.Size = new System.Drawing.Size(279, 14);
        this._lblStatus.TabIndex = 0;
        this._lblStatus.Text = "Status text";
        this._lblStatus.DoubleClick += new System.EventHandler(this.SplashScreen_DoubleClick);
        // 
        // SplashForm
        // 
        this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
        this.BackColor = System.Drawing.Color.LightGray;
        this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        this.ClientSize = new System.Drawing.Size(477, 316);
        this.Controls.Add(this._smoothProgressBar);
        this.Controls.Add(this._lblTimeRemaining);
        this.Controls.Add(this._lblStatus);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        this.Name = "SplashForm";
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Splash Screen";
        this.DoubleClick += new System.EventHandler(this.SplashScreen_DoubleClick);
        this.ResumeLayout(false);

    }
    #endregion

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.Timer _timer;
    private PK.PkUtils.UI.Controls.SmoothProgressBar _smoothProgressBar;
    private System.Windows.Forms.Label _lblTimeRemaining;
    private System.Windows.Forms.Label _lblStatus;
}
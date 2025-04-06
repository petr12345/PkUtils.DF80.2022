namespace PK.TestDumperTextWriter
{
  partial class MainForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this._txtBxDump = new System.Windows.Forms.TextBox();
      this._groupBxServerLog = new System.Windows.Forms.GroupBox();
      this._btnExit = new System.Windows.Forms.Button();
      this._btnStartStopListening = new System.Windows.Forms.Button();
      this._btnCleanLogHistory = new System.Windows.Forms.Button();
      this._timer = new System.Windows.Forms.Timer(this.components);
      this._groupBxServerLog.SuspendLayout();
      this.SuspendLayout();
      // 
      // _txtBxDump
      // 
      this._txtBxDump.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._txtBxDump.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this._txtBxDump.Location = new System.Drawing.Point(8, 16);
      this._txtBxDump.Multiline = true;
      this._txtBxDump.Name = "_txtBxDump";
      this._txtBxDump.ReadOnly = true;
      this._txtBxDump.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this._txtBxDump.Size = new System.Drawing.Size(343, 216);
      this._txtBxDump.TabIndex = 0;
      // 
      // _groupBxServerLog
      // 
      this._groupBxServerLog.Controls.Add(this._btnExit);
      this._groupBxServerLog.Controls.Add(this._btnStartStopListening);
      this._groupBxServerLog.Controls.Add(this._btnCleanLogHistory);
      this._groupBxServerLog.Controls.Add(this._txtBxDump);
      this._groupBxServerLog.Dock = System.Windows.Forms.DockStyle.Fill;
      this._groupBxServerLog.Location = new System.Drawing.Point(0, 0);
      this._groupBxServerLog.Name = "_groupBxServerLog";
      this._groupBxServerLog.Size = new System.Drawing.Size(364, 245);
      this._groupBxServerLog.TabIndex = 0;
      this._groupBxServerLog.TabStop = false;
      this._groupBxServerLog.Text = "Outgoing Trace Messages";
      // 
      // _btnExit
      // 
      this._btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnExit.Location = new System.Drawing.Point(237, 200);
      this._btnExit.Name = "_btnExit";
      this._btnExit.Size = new System.Drawing.Size(84, 23);
      this._btnExit.TabIndex = 3;
      this._btnExit.Text = "Exit";
      this._btnExit.UseVisualStyleBackColor = true;
      this._btnExit.Click += new System.EventHandler(this.OnBbtnExit_Click);
      // 
      // _btnStartStopListening
      // 
      this._btnStartStopListening.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnStartStopListening.Location = new System.Drawing.Point(51, 200);
      this._btnStartStopListening.Name = "_btnStartStopListening";
      this._btnStartStopListening.Size = new System.Drawing.Size(84, 23);
      this._btnStartStopListening.TabIndex = 1;
      this._btnStartStopListening.Text = "Stop Listening";
      this._btnStartStopListening.UseVisualStyleBackColor = true;
      this._btnStartStopListening.Click += new System.EventHandler(this.OnBtnStartStopListening_Click);
      // 
      // _btnCleanLogHistory
      // 
      this._btnCleanLogHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnCleanLogHistory.Location = new System.Drawing.Point(144, 200);
      this._btnCleanLogHistory.Name = "_btnCleanLogHistory";
      this._btnCleanLogHistory.Size = new System.Drawing.Size(84, 23);
      this._btnCleanLogHistory.TabIndex = 2;
      this._btnCleanLogHistory.Text = "Clean";
      this._btnCleanLogHistory.UseVisualStyleBackColor = true;
      this._btnCleanLogHistory.Click += new System.EventHandler(this.OnBtnCleanLogHistory_Click);
      // 
      // _timer
      // 
      this._timer.Interval = 500;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(364, 245);
      this.Controls.Add(this._groupBxServerLog);
      this.MinimumSize = new System.Drawing.Size(380, 280);
      this.Name = "MainForm";
      this.Text = "TestDumperTextWriter";
      this._groupBxServerLog.ResumeLayout(false);
      this._groupBxServerLog.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox _txtBxDump;
    private System.Windows.Forms.GroupBox _groupBxServerLog;
    private System.Windows.Forms.Button _btnExit;
    private System.Windows.Forms.Button _btnStartStopListening;
    private System.Windows.Forms.Button _btnCleanLogHistory;
    private System.Windows.Forms.Timer _timer;

  }
}


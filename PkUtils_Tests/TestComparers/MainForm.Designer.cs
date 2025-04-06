namespace PK.TestComparers
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
      this._txtBxDump = new System.Windows.Forms.TextBox();
      this._groupBxServerLog = new System.Windows.Forms.GroupBox();
      this._groupBxSubscribeMethod = new System.Windows.Forms.GroupBox();
      this._radioButtonKeyEqualityComparer = new System.Windows.Forms.RadioButton();
      this._radioButtonFuncEqualityComparer = new System.Windows.Forms.RadioButton();
      this._btnDoTest = new System.Windows.Forms.Button();
      this._btnExit = new System.Windows.Forms.Button();
      this._btnCleanLogHistory = new System.Windows.Forms.Button();
      this._groupBxServerLog.SuspendLayout();
      this._groupBxSubscribeMethod.SuspendLayout();
      this.SuspendLayout();
      // 
      // _txtBxDump
      // 
      this._txtBxDump.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._txtBxDump.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this._txtBxDump.Location = new System.Drawing.Point(8, 19);
      this._txtBxDump.Multiline = true;
      this._txtBxDump.Name = "_txtBxDump";
      this._txtBxDump.ReadOnly = true;
      this._txtBxDump.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this._txtBxDump.Size = new System.Drawing.Size(464, 273);
      this._txtBxDump.TabIndex = 0;
      // 
      // _groupBxServerLog
      // 
      this._groupBxServerLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._groupBxServerLog.Controls.Add(this._groupBxSubscribeMethod);
      this._groupBxServerLog.Controls.Add(this._btnDoTest);
      this._groupBxServerLog.Controls.Add(this._btnExit);
      this._groupBxServerLog.Controls.Add(this._btnCleanLogHistory);
      this._groupBxServerLog.Controls.Add(this._txtBxDump);
      this._groupBxServerLog.Location = new System.Drawing.Point(0, 6);
      this._groupBxServerLog.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
      this._groupBxServerLog.MinimumSize = new System.Drawing.Size(464, 262);
      this._groupBxServerLog.Name = "_groupBxServerLog";
      this._groupBxServerLog.Padding = new System.Windows.Forms.Padding(3, 6, 3, 3);
      this._groupBxServerLog.Size = new System.Drawing.Size(487, 386);
      this._groupBxServerLog.TabIndex = 0;
      this._groupBxServerLog.TabStop = false;
      this._groupBxServerLog.Text = "Recent test results";
      // 
      // _groupBxSubscribeMethod
      // 
      this._groupBxSubscribeMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._groupBxSubscribeMethod.Controls.Add(this._radioButtonKeyEqualityComparer);
      this._groupBxSubscribeMethod.Controls.Add(this._radioButtonFuncEqualityComparer);
      this._groupBxSubscribeMethod.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this._groupBxSubscribeMethod.Location = new System.Drawing.Point(11, 301);
      this._groupBxSubscribeMethod.Name = "_groupBxSubscribeMethod";
      this._groupBxSubscribeMethod.Size = new System.Drawing.Size(189, 68);
      this._groupBxSubscribeMethod.TabIndex = 5;
      this._groupBxSubscribeMethod.TabStop = false;
      this._groupBxSubscribeMethod.Text = "Test Type";
      this._groupBxSubscribeMethod.UseCompatibleTextRendering = true;
      // 
      // _radioButtonKeyEqualityComparer
      // 
      this._radioButtonKeyEqualityComparer.AutoSize = true;
      this._radioButtonKeyEqualityComparer.Location = new System.Drawing.Point(16, 39);
      this._radioButtonKeyEqualityComparer.Name = "_radioButtonKeyEqualityComparer";
      this._radioButtonKeyEqualityComparer.Size = new System.Drawing.Size(125, 17);
      this._radioButtonKeyEqualityComparer.TabIndex = 1;
      this._radioButtonKeyEqualityComparer.TabStop = true;
      this._radioButtonKeyEqualityComparer.Text = "KeyEqualityComparer";
      this._radioButtonKeyEqualityComparer.UseVisualStyleBackColor = true;
      // 
      // _radioButtonFuncEqualityComparer
      // 
      this._radioButtonFuncEqualityComparer.AutoSize = true;
      this._radioButtonFuncEqualityComparer.Location = new System.Drawing.Point(16, 19);
      this._radioButtonFuncEqualityComparer.Name = "_radioButtonFuncEqualityComparer";
      this._radioButtonFuncEqualityComparer.Size = new System.Drawing.Size(131, 17);
      this._radioButtonFuncEqualityComparer.TabIndex = 0;
      this._radioButtonFuncEqualityComparer.TabStop = true;
      this._radioButtonFuncEqualityComparer.Text = "FunctionalEqualityComparer";
      this._radioButtonFuncEqualityComparer.UseVisualStyleBackColor = true;
      // 
      // _btnDoTest
      // 
      this._btnDoTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnDoTest.Location = new System.Drawing.Point(215, 349);
      this._btnDoTest.Name = "_btnDoTest";
      this._btnDoTest.Size = new System.Drawing.Size(77, 23);
      this._btnDoTest.TabIndex = 2;
      this._btnDoTest.Text = " Test !";
      this._btnDoTest.UseVisualStyleBackColor = true;
      this._btnDoTest.Click += new System.EventHandler(this.On_btnDoTest_Click);
      // 
      // _btnExit
      // 
      this._btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnExit.Location = new System.Drawing.Point(383, 349);
      this._btnExit.Name = "_btnExit";
      this._btnExit.Size = new System.Drawing.Size(77, 23);
      this._btnExit.TabIndex = 4;
      this._btnExit.Text = "Exit";
      this._btnExit.UseVisualStyleBackColor = true;
      this._btnExit.Click += new System.EventHandler(this.On_btnExit_Click);
      // 
      // _btnCleanLogHistory
      // 
      this._btnCleanLogHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnCleanLogHistory.Location = new System.Drawing.Point(299, 349);
      this._btnCleanLogHistory.Name = "_btnCleanLogHistory";
      this._btnCleanLogHistory.Size = new System.Drawing.Size(77, 23);
      this._btnCleanLogHistory.TabIndex = 3;
      this._btnCleanLogHistory.Text = "Clean";
      this._btnCleanLogHistory.UseVisualStyleBackColor = true;
      this._btnCleanLogHistory.Click += new System.EventHandler(this.On_btnCleanLogHistory_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(484, 392);
      this.Controls.Add(this._groupBxServerLog);
      this.MinimumSize = new System.Drawing.Size(500, 335);
      this.Name = "MainForm";
      this.Text = "TestComparers";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFor_FormClosed);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFor_FormClosing);
      this._groupBxServerLog.ResumeLayout(false);
      this._groupBxServerLog.PerformLayout();
      this._groupBxSubscribeMethod.ResumeLayout(false);
      this._groupBxSubscribeMethod.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox _txtBxDump;
    private System.Windows.Forms.GroupBox _groupBxServerLog;
    private System.Windows.Forms.Button _btnExit;
    private System.Windows.Forms.Button _btnCleanLogHistory;
    private System.Windows.Forms.Button _btnDoTest;
    private System.Windows.Forms.GroupBox _groupBxSubscribeMethod;
    private System.Windows.Forms.RadioButton _radioButtonKeyEqualityComparer;
    private System.Windows.Forms.RadioButton _radioButtonFuncEqualityComparer;

  }
}


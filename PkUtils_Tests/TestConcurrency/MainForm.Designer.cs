namespace PK.TestConcurrency
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
      this._radioButtonReaderWriterLockSli_Basic = new System.Windows.Forms.RadioButton();
      this._radioButtonReaderWriterLock_Wrapper = new System.Windows.Forms.RadioButton();
      this._radioButtonReaderWriterLock_Basic = new System.Windows.Forms.RadioButton();
      this._radioButtonMonitorLock = new System.Windows.Forms.RadioButton();
      this._radioButtonInterlocked = new System.Windows.Forms.RadioButton();
      this._radioButtonNone = new System.Windows.Forms.RadioButton();
      this._btnDoTest = new System.Windows.Forms.Button();
      this._btnExit = new System.Windows.Forms.Button();
      this._btnCleanLogHistory = new System.Windows.Forms.Button();
      this._radioReaderWriterLockSli_Wrapper = new System.Windows.Forms.RadioButton();
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
      this._txtBxDump.Size = new System.Drawing.Size(464, 225);
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
      this._groupBxServerLog.Size = new System.Drawing.Size(487, 431);
      this._groupBxServerLog.TabIndex = 0;
      this._groupBxServerLog.TabStop = false;
      this._groupBxServerLog.Text = "Recent test results";
      // 
      // _groupBxSubscribeMethod
      // 
      this._groupBxSubscribeMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this._groupBxSubscribeMethod.Controls.Add(this._radioReaderWriterLockSli_Wrapper);
      this._groupBxSubscribeMethod.Controls.Add(this._radioButtonReaderWriterLockSli_Basic);
      this._groupBxSubscribeMethod.Controls.Add(this._radioButtonReaderWriterLock_Wrapper);
      this._groupBxSubscribeMethod.Controls.Add(this._radioButtonReaderWriterLock_Basic);
      this._groupBxSubscribeMethod.Controls.Add(this._radioButtonMonitorLock);
      this._groupBxSubscribeMethod.Controls.Add(this._radioButtonInterlocked);
      this._groupBxSubscribeMethod.Controls.Add(this._radioButtonNone);
      this._groupBxSubscribeMethod.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this._groupBxSubscribeMethod.Location = new System.Drawing.Point(11, 250);
      this._groupBxSubscribeMethod.Name = "_groupBxSubscribeMethod";
      this._groupBxSubscribeMethod.Size = new System.Drawing.Size(189, 167);
      this._groupBxSubscribeMethod.TabIndex = 1;
      this._groupBxSubscribeMethod.TabStop = false;
      this._groupBxSubscribeMethod.Text = "Tested Lock Type:";
      this._groupBxSubscribeMethod.UseCompatibleTextRendering = true;
      // 
      // _radioButtonReaderWriterLockSli_Basic
      // 
      this._radioButtonReaderWriterLockSli_Basic.AutoSize = true;
      this._radioButtonReaderWriterLockSli_Basic.Location = new System.Drawing.Point(16, 120);
      this._radioButtonReaderWriterLockSli_Basic.Name = "_radioButtonReaderWriterLockSli_Basic";
      this._radioButtonReaderWriterLockSli_Basic.Size = new System.Drawing.Size(160, 17);
      this._radioButtonReaderWriterLockSli_Basic.TabIndex = 5;
      this._radioButtonReaderWriterLockSli_Basic.TabStop = true;
      this._radioButtonReaderWriterLockSli_Basic.Text = "ReaderWriterLockSlim Basic";
      this._radioButtonReaderWriterLockSli_Basic.UseVisualStyleBackColor = true;
      // 
      // _radioButtonReaderWriterLock_Wrapper
      // 
      this._radioButtonReaderWriterLock_Wrapper.AutoSize = true;
      this._radioButtonReaderWriterLock_Wrapper.Location = new System.Drawing.Point(16, 99);
      this._radioButtonReaderWriterLock_Wrapper.Name = "_radioButtonReaderWriterLock_Wrapper";
      this._radioButtonReaderWriterLock_Wrapper.Size = new System.Drawing.Size(156, 17);
      this._radioButtonReaderWriterLock_Wrapper.TabIndex = 4;
      this._radioButtonReaderWriterLock_Wrapper.TabStop = true;
      this._radioButtonReaderWriterLock_Wrapper.Text = "ReaderWriterLock Wrapper";
      this._radioButtonReaderWriterLock_Wrapper.UseVisualStyleBackColor = true;
      // 
      // _radioButtonReaderWriterLock_Basic
      // 
      this._radioButtonReaderWriterLock_Basic.AutoSize = true;
      this._radioButtonReaderWriterLock_Basic.Location = new System.Drawing.Point(16, 79);
      this._radioButtonReaderWriterLock_Basic.Name = "_radioButtonReaderWriterLock_Basic";
      this._radioButtonReaderWriterLock_Basic.Size = new System.Drawing.Size(141, 17);
      this._radioButtonReaderWriterLock_Basic.TabIndex = 3;
      this._radioButtonReaderWriterLock_Basic.TabStop = true;
      this._radioButtonReaderWriterLock_Basic.Text = "ReaderWriterLock Basic";
      this._radioButtonReaderWriterLock_Basic.UseVisualStyleBackColor = true;
      // 
      // _radioButtonMonitorLock
      // 
      this._radioButtonMonitorLock.AutoSize = true;
      this._radioButtonMonitorLock.Location = new System.Drawing.Point(16, 59);
      this._radioButtonMonitorLock.Name = "_radioButtonMonitorLock";
      this._radioButtonMonitorLock.Size = new System.Drawing.Size(84, 17);
      this._radioButtonMonitorLock.TabIndex = 2;
      this._radioButtonMonitorLock.TabStop = true;
      this._radioButtonMonitorLock.Text = "MonitorLock";
      this._radioButtonMonitorLock.UseVisualStyleBackColor = true;
      // 
      // _radioButtonInterlocked
      // 
      this._radioButtonInterlocked.AutoSize = true;
      this._radioButtonInterlocked.Location = new System.Drawing.Point(16, 39);
      this._radioButtonInterlocked.Name = "_radioButtonInterlocked";
      this._radioButtonInterlocked.Size = new System.Drawing.Size(78, 17);
      this._radioButtonInterlocked.TabIndex = 1;
      this._radioButtonInterlocked.TabStop = true;
      this._radioButtonInterlocked.Text = "Interlocked";
      this._radioButtonInterlocked.UseVisualStyleBackColor = true;
      // 
      // _radioButtonNone
      // 
      this._radioButtonNone.AutoSize = true;
      this._radioButtonNone.Location = new System.Drawing.Point(16, 19);
      this._radioButtonNone.Name = "_radioButtonNone";
      this._radioButtonNone.Size = new System.Drawing.Size(51, 17);
      this._radioButtonNone.TabIndex = 0;
      this._radioButtonNone.TabStop = true;
      this._radioButtonNone.Text = "None";
      this._radioButtonNone.UseVisualStyleBackColor = true;
      // 
      // _btnDoTest
      // 
      this._btnDoTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnDoTest.Location = new System.Drawing.Point(215, 394);
      this._btnDoTest.Name = "_btnDoTest";
      this._btnDoTest.Size = new System.Drawing.Size(77, 23);
      this._btnDoTest.TabIndex = 2;
      this._btnDoTest.Text = " Test !";
      this._btnDoTest.UseVisualStyleBackColor = true;
      this._btnDoTest.Click += new System.EventHandler(this._btnDoTest_Click);
      // 
      // _btnExit
      // 
      this._btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnExit.Location = new System.Drawing.Point(383, 394);
      this._btnExit.Name = "_btnExit";
      this._btnExit.Size = new System.Drawing.Size(77, 23);
      this._btnExit.TabIndex = 4;
      this._btnExit.Text = "Exit";
      this._btnExit.UseVisualStyleBackColor = true;
      this._btnExit.Click += new System.EventHandler(this._btnExit_Click);
      // 
      // _btnCleanLogHistory
      // 
      this._btnCleanLogHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnCleanLogHistory.Location = new System.Drawing.Point(299, 394);
      this._btnCleanLogHistory.Name = "_btnCleanLogHistory";
      this._btnCleanLogHistory.Size = new System.Drawing.Size(77, 23);
      this._btnCleanLogHistory.TabIndex = 3;
      this._btnCleanLogHistory.Text = "Clean";
      this._btnCleanLogHistory.UseVisualStyleBackColor = true;
      this._btnCleanLogHistory.Click += new System.EventHandler(this._btnCleanLogHistory_Click);
      // 
      // _radioReaderWriterLockSli_Wrapper
      // 
      this._radioReaderWriterLockSli_Wrapper.AutoSize = true;
      this._radioReaderWriterLockSli_Wrapper.Location = new System.Drawing.Point(14, 142);
      this._radioReaderWriterLockSli_Wrapper.Name = "_radioReaderWriterLockSli_Wrapper";
      this._radioReaderWriterLockSli_Wrapper.Size = new System.Drawing.Size(175, 17);
      this._radioReaderWriterLockSli_Wrapper.TabIndex = 6;
      this._radioReaderWriterLockSli_Wrapper.TabStop = true;
      this._radioReaderWriterLockSli_Wrapper.Text = "ReaderWriterLockSlim Wrapper";
      this._radioReaderWriterLockSli_Wrapper.UseVisualStyleBackColor = true;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(484, 437);
      this.Controls.Add(this._groupBxServerLog);
      this.MinimumSize = new System.Drawing.Size(500, 475);
      this.Name = "MainForm";
      this.Text = "TestConcurrency";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFor_FormClosing);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFor_FormClosed);
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
    private System.Windows.Forms.RadioButton _radioButtonReaderWriterLock_Wrapper;
    private System.Windows.Forms.RadioButton _radioButtonReaderWriterLock_Basic;
    private System.Windows.Forms.RadioButton _radioButtonMonitorLock;
    private System.Windows.Forms.RadioButton _radioButtonInterlocked;
    private System.Windows.Forms.RadioButton _radioButtonNone;
    private System.Windows.Forms.RadioButton _radioButtonReaderWriterLockSli_Basic;
    private System.Windows.Forms.RadioButton _radioReaderWriterLockSli_Wrapper;

  }
}


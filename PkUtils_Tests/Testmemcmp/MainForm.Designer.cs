namespace PK.Testmemcmp
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
      this.label2 = new System.Windows.Forms.Label();
      this._numericArrayLenght = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this._numericThreads = new System.Windows.Forms.NumericUpDown();
      this._groupBxTestedCompareType = new System.Windows.Forms.GroupBox();
      this._radioButtonSequenceEquals = new System.Windows.Forms.RadioButton();
      this._radioButton_memcmp = new System.Windows.Forms.RadioButton();
      this._btnDoTest = new System.Windows.Forms.Button();
      this._btnExit = new System.Windows.Forms.Button();
      this._btnCleanLogHistory = new System.Windows.Forms.Button();
      this._groupBxServerLog.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this._numericArrayLenght)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this._numericThreads)).BeginInit();
      this._groupBxTestedCompareType.SuspendLayout();
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
      this._txtBxDump.Size = new System.Drawing.Size(524, 165);
      this._txtBxDump.TabIndex = 0;
      // 
      // _groupBxServerLog
      // 
      this._groupBxServerLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._groupBxServerLog.Controls.Add(this.label2);
      this._groupBxServerLog.Controls.Add(this._numericArrayLenght);
      this._groupBxServerLog.Controls.Add(this.label1);
      this._groupBxServerLog.Controls.Add(this._numericThreads);
      this._groupBxServerLog.Controls.Add(this._groupBxTestedCompareType);
      this._groupBxServerLog.Controls.Add(this._btnDoTest);
      this._groupBxServerLog.Controls.Add(this._btnExit);
      this._groupBxServerLog.Controls.Add(this._btnCleanLogHistory);
      this._groupBxServerLog.Controls.Add(this._txtBxDump);
      this._groupBxServerLog.Location = new System.Drawing.Point(0, 15);
      this._groupBxServerLog.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
      this._groupBxServerLog.MinimumSize = new System.Drawing.Size(464, 262);
      this._groupBxServerLog.Name = "_groupBxServerLog";
      this._groupBxServerLog.Padding = new System.Windows.Forms.Padding(3, 6, 3, 3);
      this._groupBxServerLog.Size = new System.Drawing.Size(547, 303);
      this._groupBxServerLog.TabIndex = 0;
      this._groupBxServerLog.TabStop = false;
      this._groupBxServerLog.Text = "Recent test results";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(239, 232);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(66, 13);
      this.label2.TabIndex = 10;
      this.label2.Text = "Array length:";
      // 
      // _numericArrayLenght
      // 
      this._numericArrayLenght.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._numericArrayLenght.Location = new System.Drawing.Point(339, 227);
      this._numericArrayLenght.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
      this._numericArrayLenght.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this._numericArrayLenght.Name = "_numericArrayLenght";
      this._numericArrayLenght.Size = new System.Drawing.Size(73, 20);
      this._numericArrayLenght.TabIndex = 9;
      this._numericArrayLenght.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(239, 205);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(86, 13);
      this.label1.TabIndex = 8;
      this.label1.Text = "Parallel Threads:";
      // 
      // _numericThreads
      // 
      this._numericThreads.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._numericThreads.Location = new System.Drawing.Point(339, 200);
      this._numericThreads.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this._numericThreads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this._numericThreads.Name = "_numericThreads";
      this._numericThreads.Size = new System.Drawing.Size(73, 20);
      this._numericThreads.TabIndex = 6;
      this._numericThreads.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // _groupBxTestedCompareType
      // 
      this._groupBxTestedCompareType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._groupBxTestedCompareType.Controls.Add(this._radioButtonSequenceEquals);
      this._groupBxTestedCompareType.Controls.Add(this._radioButton_memcmp);
      this._groupBxTestedCompareType.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this._groupBxTestedCompareType.Location = new System.Drawing.Point(11, 200);
      this._groupBxTestedCompareType.Name = "_groupBxTestedCompareType";
      this._groupBxTestedCompareType.Size = new System.Drawing.Size(214, 89);
      this._groupBxTestedCompareType.TabIndex = 1;
      this._groupBxTestedCompareType.TabStop = false;
      this._groupBxTestedCompareType.Text = "Tested Compare Type:";
      this._groupBxTestedCompareType.UseCompatibleTextRendering = true;
      // 
      // _radioButtonSequenceEquals
      // 
      this._radioButtonSequenceEquals.AutoSize = true;
      this._radioButtonSequenceEquals.Location = new System.Drawing.Point(16, 39);
      this._radioButtonSequenceEquals.Name = "_radioButtonSequenceEquals";
      this._radioButtonSequenceEquals.Size = new System.Drawing.Size(106, 17);
      this._radioButtonSequenceEquals.TabIndex = 1;
      this._radioButtonSequenceEquals.TabStop = true;
      this._radioButtonSequenceEquals.Text = "SequenceEquals";
      this._radioButtonSequenceEquals.UseVisualStyleBackColor = true;
      // 
      // _radioButton_memcmp
      // 
      this._radioButton_memcmp.AutoSize = true;
      this._radioButton_memcmp.Location = new System.Drawing.Point(16, 19);
      this._radioButton_memcmp.Name = "_radioButton_memcmp";
      this._radioButton_memcmp.Size = new System.Drawing.Size(67, 17);
      this._radioButton_memcmp.TabIndex = 0;
      this._radioButton_memcmp.TabStop = true;
      this._radioButton_memcmp.Text = "memcmp";
      this._radioButton_memcmp.UseVisualStyleBackColor = true;
      // 
      // _btnDoTest
      // 
      this._btnDoTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnDoTest.Location = new System.Drawing.Point(275, 266);
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
      this._btnExit.Location = new System.Drawing.Point(443, 266);
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
      this._btnCleanLogHistory.Location = new System.Drawing.Point(359, 266);
      this._btnCleanLogHistory.Name = "_btnCleanLogHistory";
      this._btnCleanLogHistory.Size = new System.Drawing.Size(77, 23);
      this._btnCleanLogHistory.TabIndex = 3;
      this._btnCleanLogHistory.Text = "Clean";
      this._btnCleanLogHistory.UseVisualStyleBackColor = true;
      this._btnCleanLogHistory.Click += new System.EventHandler(this._btnCleanLogHistory_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(544, 322);
      this.Controls.Add(this._groupBxServerLog);
      this.MinimumSize = new System.Drawing.Size(560, 360);
      this.Name = "MainForm";
      this.Text = "Test memcmp";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFor_FormClosed);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFor_FormClosing);
      this._groupBxServerLog.ResumeLayout(false);
      this._groupBxServerLog.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this._numericArrayLenght)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this._numericThreads)).EndInit();
      this._groupBxTestedCompareType.ResumeLayout(false);
      this._groupBxTestedCompareType.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox _txtBxDump;
    private System.Windows.Forms.GroupBox _groupBxServerLog;
    private System.Windows.Forms.Button _btnExit;
    private System.Windows.Forms.Button _btnCleanLogHistory;
    private System.Windows.Forms.Button _btnDoTest;
    private System.Windows.Forms.GroupBox _groupBxTestedCompareType;
    private System.Windows.Forms.RadioButton _radioButtonSequenceEquals;
    private System.Windows.Forms.RadioButton _radioButton_memcmp;
    private System.Windows.Forms.NumericUpDown _numericThreads;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown _numericArrayLenght;

  }
}


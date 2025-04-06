namespace TestBinding
{
  partial class MyForm
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
      this._buttonOk = new System.Windows.Forms.Button();
      this._buttonCance = new System.Windows.Forms.Button();
      this._textBx1st = new System.Windows.Forms.TextBox();
      this._myFormBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this._dataGridView1st = new System.Windows.Forms.DataGridView();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.textBx2nd = new System.Windows.Forms.TextBox();
      this._programBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this._lblGridView = new System.Windows.Forms.Label();
      this._btnStartStopListening = new System.Windows.Forms.Button();
      this._btnCleanLogHistory = new System.Windows.Forms.Button();
      this._groupBxServerLog = new System.Windows.Forms.GroupBox();
      this._txtBxDump = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this._myFormBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this._dataGridView1st)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this._programBindingSource)).BeginInit();
      this._groupBxServerLog.SuspendLayout();
      this.SuspendLayout();
      // 
      // _buttonOk
      // 
      this._buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
      this._buttonOk.Location = new System.Drawing.Point(417, 356);
      this._buttonOk.Name = "_buttonOk";
      this._buttonOk.Size = new System.Drawing.Size(75, 23);
      this._buttonOk.TabIndex = 9;
      this._buttonOk.Text = "OK";
      this._buttonOk.UseVisualStyleBackColor = true;
      // 
      // _buttonCance
      // 
      this._buttonCance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._buttonCance.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this._buttonCance.Location = new System.Drawing.Point(417, 385);
      this._buttonCance.Name = "_buttonCance";
      this._buttonCance.Size = new System.Drawing.Size(75, 23);
      this._buttonCance.TabIndex = 10;
      this._buttonCance.Text = "Cancel";
      this._buttonCance.UseVisualStyleBackColor = true;
      // 
      // _textBx1st
      // 
      this._textBx1st.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._textBx1st.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._myFormBindingSource, "Data_String", true));
      this._textBx1st.Location = new System.Drawing.Point(165, 172);
      this._textBx1st.Name = "_textBx1st";
      this._textBx1st.Size = new System.Drawing.Size(327, 20);
      this._textBx1st.TabIndex = 3;
      // 
      // _myFormBindingSource
      // 
      this._myFormBindingSource.DataSource = typeof(TestBinding.MyForm);
      // 
      // _dataGridView1st
      // 
      this._dataGridView1st.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._dataGridView1st.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this._dataGridView1st.Location = new System.Drawing.Point(18, 29);
      this._dataGridView1st.Name = "_dataGridView1st";
      this._dataGridView1st.Size = new System.Drawing.Size(474, 133);
      this._dataGridView1st.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(18, 176);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(121, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "1st string, binds to Form:";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(18, 201);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(141, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "2nd string, binds to Program:";
      // 
      // textBx2nd
      // 
      this.textBx2nd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.textBx2nd.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._programBindingSource, "Data_p_String", true));
      this.textBx2nd.Location = new System.Drawing.Point(165, 197);
      this.textBx2nd.Name = "textBx2nd";
      this.textBx2nd.Size = new System.Drawing.Size(327, 20);
      this.textBx2nd.TabIndex = 5;
      // 
      // _programBindingSource
      // 
      this._programBindingSource.DataSource = typeof(TestBinding.Program);
      // 
      // _lblGridView
      // 
      this._lblGridView.AutoSize = true;
      this._lblGridView.Location = new System.Drawing.Point(18, 9);
      this._lblGridView.Name = "_lblGridView";
      this._lblGridView.Size = new System.Drawing.Size(190, 13);
      this._lblGridView.TabIndex = 0;
      this._lblGridView.Text = "DataGridView, will bind to something ...";
      // 
      // _btnStartStopListening
      // 
      this._btnStartStopListening.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this._btnStartStopListening.Location = new System.Drawing.Point(417, 253);
      this._btnStartStopListening.Name = "_btnStartStopListening";
      this._btnStartStopListening.Size = new System.Drawing.Size(75, 23);
      this._btnStartStopListening.TabIndex = 7;
      this._btnStartStopListening.Text = "Stop Listening";
      this._btnStartStopListening.UseVisualStyleBackColor = true;
      this._btnStartStopListening.Click += new System.EventHandler(this.OnBtnStartStopListening_Click);
      // 
      // _btnCleanLogHistory
      // 
      this._btnCleanLogHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this._btnCleanLogHistory.Location = new System.Drawing.Point(417, 282);
      this._btnCleanLogHistory.Name = "_btnCleanLogHistory";
      this._btnCleanLogHistory.Size = new System.Drawing.Size(75, 23);
      this._btnCleanLogHistory.TabIndex = 8;
      this._btnCleanLogHistory.Text = "Clean";
      this._btnCleanLogHistory.UseVisualStyleBackColor = true;
      this._btnCleanLogHistory.Click += new System.EventHandler(this.OnBtnCleanLogHistory_Click);
      // 
      // _groupBxServerLog
      // 
      this._groupBxServerLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._groupBxServerLog.Controls.Add(this._txtBxDump);
      this._groupBxServerLog.Location = new System.Drawing.Point(8, 237);
      this._groupBxServerLog.Name = "_groupBxServerLog";
      this._groupBxServerLog.Size = new System.Drawing.Size(403, 180);
      this._groupBxServerLog.TabIndex = 6;
      this._groupBxServerLog.TabStop = false;
      this._groupBxServerLog.Text = "Outgoing Trace Messages";
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
      this._txtBxDump.Size = new System.Drawing.Size(382, 151);
      this._txtBxDump.TabIndex = 0;
      // 
      // MyForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(504, 420);
      this.Controls.Add(this._groupBxServerLog);
      this.Controls.Add(this._btnStartStopListening);
      this.Controls.Add(this._btnCleanLogHistory);
      this.Controls.Add(this._lblGridView);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.textBx2nd);
      this.Controls.Add(this.label1);
      this.Controls.Add(this._dataGridView1st);
      this.Controls.Add(this._textBx1st);
      this.Controls.Add(this._buttonCance);
      this.Controls.Add(this._buttonOk);
      this.MinimumSize = new System.Drawing.Size(520, 455);
      this.Name = "MyForm";
      this.Text = "TestBinding";
      this.Load += new System.EventHandler(this.MyFor_Load);
      ((System.ComponentModel.ISupportInitialize)(this._myFormBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this._dataGridView1st)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this._programBindingSource)).EndInit();
      this._groupBxServerLog.ResumeLayout(false);
      this._groupBxServerLog.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button _buttonOk;
    private System.Windows.Forms.Button _buttonCance;
    private System.Windows.Forms.TextBox _textBx1st;
    private System.Windows.Forms.DataGridView _dataGridView1st;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBx2nd;
    private System.Windows.Forms.BindingSource _myFormBindingSource;
    private System.Windows.Forms.BindingSource _programBindingSource;
    private System.Windows.Forms.Label _lblGridView;
    private System.Windows.Forms.Button _btnStartStopListening;
    private System.Windows.Forms.Button _btnCleanLogHistory;
    private System.Windows.Forms.GroupBox _groupBxServerLog;
    private System.Windows.Forms.TextBox _txtBxDump;
  }
}


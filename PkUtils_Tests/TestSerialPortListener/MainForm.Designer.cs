namespace PK.TestSerialPortListener
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
      this._btnStart = new System.Windows.Forms.Button();
      this._tbData = new System.Windows.Forms.TextBox();
      this._btnStop = new System.Windows.Forms.Button();
      this._serialSettingsCtrl = new PK.PkUtils.SerialPortUILib.SerialPortSettingsCtrl();
      this._serialSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this._serialSettingsBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // _btnStart
      // 
      this._btnStart.Location = new System.Drawing.Point(12, 201);
      this._btnStart.Name = "_btnStart";
      this._btnStart.Size = new System.Drawing.Size(114, 23);
      this._btnStart.TabIndex = 1;
      this._btnStart.Text = "Start port listening";
      this._btnStart.UseVisualStyleBackColor = true;
      this._btnStart.Click += new System.EventHandler(this.OnBtnStart_Click);
      // 
      // _tbData
      // 
      this._tbData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this._tbData.Location = new System.Drawing.Point(12, 260);
      this._tbData.Multiline = true;
      this._tbData.Name = "_tbData";
      this._tbData.ReadOnly = true;
      this._tbData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this._tbData.Size = new System.Drawing.Size(435, 161);
      this._tbData.TabIndex = 3;
      // 
      // _btnStop
      // 
      this._btnStop.Location = new System.Drawing.Point(139, 201);
      this._btnStop.Name = "_btnStop";
      this._btnStop.Size = new System.Drawing.Size(114, 23);
      this._btnStop.TabIndex = 2;
      this._btnStop.Text = "Stop port listening";
      this._btnStop.UseVisualStyleBackColor = true;
      this._btnStop.Click += new System.EventHandler(this.OnBtnStop_Click);
      // 
      // _serialSettingsCtrl
      // 
      this._serialSettingsCtrl.Location = new System.Drawing.Point(6, 6);
      this._serialSettingsCtrl.MinimumSize = new System.Drawing.Size(220, 170);
      this._serialSettingsCtrl.Name = "_serialSettingsCtrl";
      this._serialSettingsCtrl.Size = new System.Drawing.Size(220, 170);
      this._serialSettingsCtrl.TabIndex = 0;
      this._serialSettingsCtrl.TypeLabelVisible = false;
      // 
      // _serialSettingsBindingSource
      // 
      this._serialSettingsBindingSource.DataSource = typeof(PK.PkUtils.SerialPortLib.SerialPortSettingsEx);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.ClientSize = new System.Drawing.Size(459, 437);
      this.Controls.Add(this._serialSettingsCtrl);
      this.Controls.Add(this._tbData);
      this.Controls.Add(this._btnStop);
      this.Controls.Add(this._btnStart);
      this.MinimumSize = new System.Drawing.Size(475, 475);
      this.Name = "MainForm";
      this.Text = "Serial Port Listener Test";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFor_FormClosing);
      this.DoubleClick += new System.EventHandler(this.MainFor_DoubleClick);
      ((System.ComponentModel.ISupportInitialize)(this._serialSettingsBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.BindingSource _serialSettingsBindingSource;
    private System.Windows.Forms.Button _btnStart;
    private System.Windows.Forms.TextBox _tbData;
    private System.Windows.Forms.Button _btnStop;
    private PK.PkUtils.SerialPortUILib.SerialPortSettingsCtrl _serialSettingsCtrl;
  }
}


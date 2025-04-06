// Ignore Spelling: Utils
// 

namespace PK.PkUtils.SerialPortUILib
{
  partial class SerialPortSettingsCtrl
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.Label baudRateLabel;
      System.Windows.Forms.Label dataBitsLabel;
      System.Windows.Forms.Label parityLabel;
      System.Windows.Forms.Label portNameLabel;
      System.Windows.Forms.Label stopBitsLabel;
      this._baudRateComboBox = new System.Windows.Forms.ComboBox();
      this._serialSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this._dataBitsComboBox = new System.Windows.Forms.ComboBox();
      this._parityComboBox = new System.Windows.Forms.ComboBox();
      this._portNameComboBox = new System.Windows.Forms.ComboBox();
      this._stopBitsComboBox = new System.Windows.Forms.ComboBox();
      this._groupBox = new System.Windows.Forms.GroupBox();
      this._ErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      baudRateLabel = new System.Windows.Forms.Label();
      dataBitsLabel = new System.Windows.Forms.Label();
      parityLabel = new System.Windows.Forms.Label();
      portNameLabel = new System.Windows.Forms.Label();
      stopBitsLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this._serialSettingsBindingSource)).BeginInit();
      this._groupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this._ErrorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // _lblTypeText
      // 
      this._lblTypeText.Location = new System.Drawing.Point(73, 0);
      this._lblTypeText.Size = new System.Drawing.Size(147, 15);
      this._lblTypeText.Text = "SerialSettingsCtrl_Design";
      // 
      // baudRateLabel
      // 
      baudRateLabel.AutoSize = true;
      baudRateLabel.Location = new System.Drawing.Point(10, 59);
      baudRateLabel.Name = "baudRateLabel";
      baudRateLabel.Size = new System.Drawing.Size(61, 13);
      baudRateLabel.TabIndex = 2;
      baudRateLabel.Text = "Baud Rate:";
      // 
      // dataBitsLabel
      // 
      dataBitsLabel.AutoSize = true;
      dataBitsLabel.Location = new System.Drawing.Point(10, 86);
      dataBitsLabel.Name = "dataBitsLabel";
      dataBitsLabel.Size = new System.Drawing.Size(53, 13);
      dataBitsLabel.TabIndex = 4;
      dataBitsLabel.Text = "Data Bits:";
      // 
      // parityLabel
      // 
      parityLabel.AutoSize = true;
      parityLabel.Location = new System.Drawing.Point(10, 113);
      parityLabel.Name = "parityLabel";
      parityLabel.Size = new System.Drawing.Size(36, 13);
      parityLabel.TabIndex = 6;
      parityLabel.Text = "Parity:";
      // 
      // portNameLabel
      // 
      portNameLabel.AutoSize = true;
      portNameLabel.Location = new System.Drawing.Point(10, 32);
      portNameLabel.Name = "portNameLabel";
      portNameLabel.Size = new System.Drawing.Size(60, 13);
      portNameLabel.TabIndex = 0;
      portNameLabel.Text = "Port Name:";
      // 
      // stopBitsLabel
      // 
      stopBitsLabel.AutoSize = true;
      stopBitsLabel.Location = new System.Drawing.Point(10, 140);
      stopBitsLabel.Name = "stopBitsLabel";
      stopBitsLabel.Size = new System.Drawing.Size(52, 13);
      stopBitsLabel.TabIndex = 8;
      stopBitsLabel.Text = "Stop Bits:";
      // 
      // _baudRateComboBox
      // 
      this._baudRateComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._serialSettingsBindingSource, "BaudRate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this._baudRateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._baudRateComboBox.FormattingEnabled = true;
      this._baudRateComboBox.Location = new System.Drawing.Point(77, 56);
      this._baudRateComboBox.Name = "_baudRateComboBox";
      this._baudRateComboBox.Size = new System.Drawing.Size(121, 21);
      this._baudRateComboBox.TabIndex = 3;
      // 
      // _serialSettingsBindingSource
      // 
      this._serialSettingsBindingSource.DataSource = typeof(PK.PkUtils.SerialPortLib.SerialPortSettings);
      // 
      // _dataBitsComboBox
      // 
      this._dataBitsComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._serialSettingsBindingSource, "DataBits", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this._dataBitsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._dataBitsComboBox.FormattingEnabled = true;
      this._dataBitsComboBox.Location = new System.Drawing.Point(77, 83);
      this._dataBitsComboBox.Name = "_dataBitsComboBox";
      this._dataBitsComboBox.Size = new System.Drawing.Size(121, 21);
      this._dataBitsComboBox.TabIndex = 5;
      // 
      // _parityComboBox
      // 
      this._parityComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._serialSettingsBindingSource, "Parity", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this._parityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._parityComboBox.FormattingEnabled = true;
      this._parityComboBox.Location = new System.Drawing.Point(77, 110);
      this._parityComboBox.Name = "_parityComboBox";
      this._parityComboBox.Size = new System.Drawing.Size(121, 21);
      this._parityComboBox.TabIndex = 7;
      // 
      // _portNameComboBox
      // 
      this._portNameComboBox.AllowDrop = true;
      this._portNameComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._serialSettingsBindingSource, "PortName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this._portNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._portNameComboBox.FormattingEnabled = true;
      this._portNameComboBox.Location = new System.Drawing.Point(77, 29);
      this._portNameComboBox.Name = "_portNameComboBox";
      this._portNameComboBox.Size = new System.Drawing.Size(121, 21);
      this._portNameComboBox.TabIndex = 1;
      // 
      // _stopBitsComboBox
      // 
      this._stopBitsComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._serialSettingsBindingSource, "StopBits", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this._stopBitsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this._stopBitsComboBox.FormattingEnabled = true;
      this._stopBitsComboBox.Location = new System.Drawing.Point(77, 137);
      this._stopBitsComboBox.Name = "_stopBitsComboBox";
      this._stopBitsComboBox.Size = new System.Drawing.Size(121, 21);
      this._stopBitsComboBox.TabIndex = 9;
      // 
      // _groupBox
      // 
      this._groupBox.Controls.Add(this._baudRateComboBox);
      this._groupBox.Controls.Add(baudRateLabel);
      this._groupBox.Controls.Add(this._stopBitsComboBox);
      this._groupBox.Controls.Add(stopBitsLabel);
      this._groupBox.Controls.Add(dataBitsLabel);
      this._groupBox.Controls.Add(this._portNameComboBox);
      this._groupBox.Controls.Add(this._dataBitsComboBox);
      this._groupBox.Controls.Add(portNameLabel);
      this._groupBox.Controls.Add(parityLabel);
      this._groupBox.Controls.Add(this._parityComboBox);
      this._groupBox.Location = new System.Drawing.Point(0, 0);
      this._groupBox.Name = "_groupBox";
      this._groupBox.Size = new System.Drawing.Size(220, 170);
      this._groupBox.TabIndex = 0;
      this._groupBox.TabStop = false;
      this._groupBox.Text = "Serial Port Settings";
      // 
      // _ErrorProvider
      // 
      this._ErrorProvider.ContainerControl = this;
      // 
      // SerialSettingsCtrl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._groupBox);
      this.MinimumSize = new System.Drawing.Size(220, 170);
      this.Name = "SerialSettingsCtrl";
      this.Size = new System.Drawing.Size(220, 170);
      this.TypeLabelVisible = true;
      this.Controls.SetChildIndex(this._groupBox, 0);
      this.Controls.SetChildIndex(this._lblTypeText, 0);
      ((System.ComponentModel.ISupportInitialize)(this._serialSettingsBindingSource)).EndInit();
      this._groupBox.ResumeLayout(false);
      this._groupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this._ErrorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.BindingSource _serialSettingsBindingSource;
    private System.Windows.Forms.ComboBox _baudRateComboBox;
    private System.Windows.Forms.ComboBox _dataBitsComboBox;
    private System.Windows.Forms.ComboBox _parityComboBox;
    private System.Windows.Forms.ComboBox _portNameComboBox;
    private System.Windows.Forms.ComboBox _stopBitsComboBox;
    private System.Windows.Forms.GroupBox _groupBox;
    private System.Windows.Forms.ErrorProvider _ErrorProvider;
  }
}


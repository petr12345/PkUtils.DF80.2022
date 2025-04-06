namespace PK.TestSplash
{
  partial class TestForm
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
      this._labelInfo = new System.Windows.Forms.Label();
      this._timerClose = new System.Windows.Forms.Timer(this.components);
      this.SuspendLayout();
      // 
      // _labelInfo
      // 
      this._labelInfo.AutoSize = true;
      this._labelInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._labelInfo.Location = new System.Drawing.Point(12, 47);
      this._labelInfo.Name = "_labelInfo";
      this._labelInfo.Size = new System.Drawing.Size(426, 25);
      this._labelInfo.TabIndex = 1;
      this._labelInfo.Text = "The form will close itself in a few seconds...";
      // 
      // _timerClose
      // 
      this._timerClose.Tick += new System.EventHandler(this._timerClose_Tick);
      // 
      // TestForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(492, 208);
      this.Controls.Add(this._labelInfo);
      this.Name = "TestForm";
      this.Text = "TestForm";
      this.Layout += new System.Windows.Forms.LayoutEventHandler(this.TestFor_Layout);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label _labelInfo;
    private System.Windows.Forms.Timer _timerClose;
  }
}
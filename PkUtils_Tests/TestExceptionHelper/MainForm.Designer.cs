namespace PK.TestExceptionHelper
{
  partial class MainForm
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
      this._btnExit = new System.Windows.Forms.Button();
      this._btnTestWrong = new System.Windows.Forms.Button();
      this._btnTestGood = new System.Windows.Forms.Button();
      this._textBxOut = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // _btnExit
      // 
      this._btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnExit.DialogResult = System.Windows.Forms.DialogResult.OK;
      this._btnExit.Location = new System.Drawing.Point(850, 436);
      this._btnExit.Name = "_btnExit";
      this._btnExit.Size = new System.Drawing.Size(75, 27);
      this._btnExit.TabIndex = 2;
      this._btnExit.Text = "Exit";
      this._btnExit.UseVisualStyleBackColor = true;
      this._btnExit.Click += new System.EventHandler(this._btnExit_Click);
      // 
      // _btnTestWrong
      // 
      this._btnTestWrong.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnTestWrong.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this._btnTestWrong.Location = new System.Drawing.Point(722, 436);
      this._btnTestWrong.Name = "_btnTestWrong";
      this._btnTestWrong.Size = new System.Drawing.Size(112, 27);
      this._btnTestWrong.TabIndex = 1;
      this._btnTestWrong.Text = "Test Wrong Handling";
      this._btnTestWrong.Click += new System.EventHandler(this._btnTestWrong_Click);
      // 
      // _btnTestGood
      // 
      this._btnTestGood.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnTestGood.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this._btnTestGood.Location = new System.Drawing.Point(594, 438);
      this._btnTestGood.Name = "_btnTestGood";
      this._btnTestGood.Size = new System.Drawing.Size(112, 27);
      this._btnTestGood.TabIndex = 0;
      this._btnTestGood.Text = "Test Good Handling";
      this._btnTestGood.Click += new System.EventHandler(this._btnTestGood_Click);
      // 
      // _textBxOut
      // 
      this._textBxOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this._textBxOut.ForeColor = System.Drawing.SystemColors.ControlText;
      this._textBxOut.Location = new System.Drawing.Point(10, 26);
      this._textBxOut.Multiline = true;
      this._textBxOut.Name = "_textBxOut";
      this._textBxOut.ReadOnly = true;
      this._textBxOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this._textBxOut.Size = new System.Drawing.Size(922, 390);
      this._textBxOut.TabIndex = 14;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(944, 482);
      this.Controls.Add(this._textBxOut);
      this.Controls.Add(this._btnExit);
      this.Controls.Add(this._btnTestWrong);
      this.Controls.Add(this._btnTestGood);
      this.MinimumSize = new System.Drawing.Size(960, 520);
      this.Name = "MainForm";
      this.Text = "TestExceptionHelper";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button _btnExit;
    private System.Windows.Forms.Button _btnTestWrong;
    private System.Windows.Forms.Button _btnTestGood;
    private System.Windows.Forms.TextBox _textBxOut;
  }
}


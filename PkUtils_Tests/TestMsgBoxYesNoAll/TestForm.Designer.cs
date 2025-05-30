namespace PK.TestMsgBoxYesNoAll
{
  partial class TestForm
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
            this._btnTest = new System.Windows.Forms.Button();
            this._lblInfo = new System.Windows.Forms.Label();
            this._btnClose = new System.Windows.Forms.Button();
            this._checkBxTestLayoutPanel = new System.Windows.Forms.CheckBox();
            this._checkBxTestCustomBtnsTexts = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _btnTest
            // 
            this._btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnTest.Location = new System.Drawing.Point(244, 100);
            this._btnTest.Name = "_btnTest";
            this._btnTest.Size = new System.Drawing.Size(75, 23);
            this._btnTest.TabIndex = 0;
            this._btnTest.Text = "Test";
            this._btnTest.UseVisualStyleBackColor = true;
            this._btnTest.Click += new System.EventHandler(this.OnTestButton_Click);
            // 
            // _lblInfo
            // 
            this._lblInfo.AutoSize = true;
            this._lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblInfo.Location = new System.Drawing.Point(12, 24);
            this._lblInfo.Name = "_lblInfo";
            this._lblInfo.Size = new System.Drawing.Size(368, 20);
            this._lblInfo.TabIndex = 2;
            this._lblInfo.Text = "Click the \"Test\" button to test the MsgBoxYesNoAll";
            // 
            // _btnClose
            // 
            this._btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnClose.Location = new System.Drawing.Point(336, 101);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(75, 23);
            this._btnClose.TabIndex = 1;
            this._btnClose.Text = "Close";
            this._btnClose.UseVisualStyleBackColor = true;
            this._btnClose.Click += new System.EventHandler(this.OnBtnClose_Click);
            // 
            // _checkBxTestLayoutPanel
            // 
            this._checkBxTestLayoutPanel.AutoSize = true;
            this._checkBxTestLayoutPanel.Location = new System.Drawing.Point(16, 58);
            this._checkBxTestLayoutPanel.Name = "_checkBxTestLayoutPanel";
            this._checkBxTestLayoutPanel.Size = new System.Drawing.Size(231, 17);
            this._checkBxTestLayoutPanel.TabIndex = 3;
            this._checkBxTestLayoutPanel.Text = "Test implementtaion with TableLayoutPanel";
            this._checkBxTestLayoutPanel.UseVisualStyleBackColor = true;
            // 
            // _checkBxTestCustomBtnsTexts
            // 
            this._checkBxTestCustomBtnsTexts.AutoSize = true;
            this._checkBxTestCustomBtnsTexts.Location = new System.Drawing.Point(15, 78);
            this._checkBxTestCustomBtnsTexts.Name = "_checkBxTestCustomBtnsTexts";
            this._checkBxTestCustomBtnsTexts.Size = new System.Drawing.Size(159, 17);
            this._checkBxTestCustomBtnsTexts.TabIndex = 4;
            this._checkBxTestCustomBtnsTexts.Text = "Test custom texts of buttons";
            this._checkBxTestCustomBtnsTexts.UseVisualStyleBackColor = true;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 131);
            this.Controls.Add(this._checkBxTestCustomBtnsTexts);
            this.Controls.Add(this._checkBxTestLayoutPanel);
            this.Controls.Add(this._btnClose);
            this.Controls.Add(this._lblInfo);
            this.Controls.Add(this._btnTest);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(430, 170);
            this.MinimumSize = new System.Drawing.Size(430, 170);
            this.Name = "TestForm";
            this.Text = "Test Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestFor_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label _lblInfo;
    private System.Windows.Forms.Button _btnTest;
    private System.Windows.Forms.Button _btnClose;
    private System.Windows.Forms.CheckBox _checkBxTestLayoutPanel;
	private System.Windows.Forms.CheckBox _checkBxTestCustomBtnsTexts;
  }
}


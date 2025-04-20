// Ignore Spelling: Utils
// 

namespace PK.PkUtils.UI.General
{
  partial class MsgBoxYesNoAll
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
      this._pbxIcon = new System.Windows.Forms.PictureBox();
      this._btnYes = new System.Windows.Forms.Button();
      this._btnYesToAll = new System.Windows.Forms.Button();
      this._btnNo = new System.Windows.Forms.Button();
      this._btnNoToAll = new System.Windows.Forms.Button();
      this._btnCancel = new System.Windows.Forms.Button();
      this._lblContent = new System.Windows.Forms.Label();
      this._lblMainInstruction = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this._pbxIcon)).BeginInit();
      this.SuspendLayout();
      // 
      // _pbxIcon
      // 
      this._pbxIcon.Location = new System.Drawing.Point(12, 12);
      this._pbxIcon.Name = "_pbxIcon";
      this._pbxIcon.Size = new System.Drawing.Size(64, 64);
      this._pbxIcon.TabIndex = 0;
      this._pbxIcon.TabStop = false;
      // 
      // _btnYes
      // 
      this._btnYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
      this._btnYes.Location = new System.Drawing.Point(23, 117);
      this._btnYes.Name = "_btnYes";
      this._btnYes.Size = new System.Drawing.Size(75, 23);
      this._btnYes.TabIndex = 0;
      this._btnYes.Text = "&Yes";
      this._btnYes.UseVisualStyleBackColor = true;
      this._btnYes.Click += new System.EventHandler(this.OnBtn_Click);
      // 
      // _btnYesToAll
      // 
      this._btnYesToAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnYesToAll.DialogResult = System.Windows.Forms.DialogResult.Yes;
      this._btnYesToAll.Location = new System.Drawing.Point(104, 117);
      this._btnYesToAll.Name = "_btnYesToAll";
      this._btnYesToAll.Size = new System.Drawing.Size(75, 23);
      this._btnYesToAll.TabIndex = 1;
      this._btnYesToAll.Text = "Yes to All";
      this._btnYesToAll.UseVisualStyleBackColor = true;
      this._btnYesToAll.Click += new System.EventHandler(this.OnBtn_Click);
      // 
      // _btnNo
      // 
      this._btnNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
      this._btnNo.Location = new System.Drawing.Point(185, 117);
      this._btnNo.Name = "_btnNo";
      this._btnNo.Size = new System.Drawing.Size(75, 23);
      this._btnNo.TabIndex = 2;
      this._btnNo.Text = "&No";
      this._btnNo.UseVisualStyleBackColor = true;
      this._btnNo.Click += new System.EventHandler(this.OnBtn_Click);
      // 
      // _btnNoToAll
      // 
      this._btnNoToAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnNoToAll.DialogResult = System.Windows.Forms.DialogResult.No;
      this._btnNoToAll.Location = new System.Drawing.Point(266, 117);
      this._btnNoToAll.Name = "_btnNoToAll";
      this._btnNoToAll.Size = new System.Drawing.Size(75, 23);
      this._btnNoToAll.TabIndex = 3;
      this._btnNoToAll.Text = "Not to All";
      this._btnNoToAll.UseVisualStyleBackColor = true;
      this._btnNoToAll.Click += new System.EventHandler(this.OnBtn_Click);
      // 
      // _btnCancel
      // 
      this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this._btnCancel.Location = new System.Drawing.Point(347, 117);
      this._btnCancel.Name = "_btnCancel";
      this._btnCancel.Size = new System.Drawing.Size(75, 23);
      this._btnCancel.TabIndex = 4;
      this._btnCancel.Text = "Cancel";
      this._btnCancel.UseVisualStyleBackColor = true;
      this._btnCancel.Click += new System.EventHandler(this.OnBtn_Click);
      // 
      // _lblContent
      // 
      this._lblContent.AutoSize = true;
      this._lblContent.Location = new System.Drawing.Point(118, 45);
      this._lblContent.Name = "_lblContent";
      this._lblContent.Size = new System.Drawing.Size(171, 13);
      this._lblContent.TabIndex = 6;
      this._lblContent.Text = "The content text. Can be multi-line.";
      this._lblContent.TextChanged += new System.EventHandler(this.AnyLabel_TextChanged);
      // 
      // _lblMainInstruction
      // 
      this._lblMainInstruction.AutoSize = true;
      this._lblMainInstruction.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._lblMainInstruction.Location = new System.Drawing.Point(118, 12);
      this._lblMainInstruction.Name = "_lblMainInstruction";
      this._lblMainInstruction.Size = new System.Drawing.Size(189, 20);
      this._lblMainInstruction.TabIndex = 5;
      this._lblMainInstruction.Text = "The main instruction text. ";
      this._lblMainInstruction.TextChanged += new System.EventHandler(this.AnyLabel_TextChanged);
      // 
      // MsgBoxYesNoAll
      // 
      this.AcceptButton = this._btnYes;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this._btnCancel;
      this.ClientSize = new System.Drawing.Size(434, 152);
      this.Controls.Add(this._lblMainInstruction);
      this.Controls.Add(this._lblContent);
      this.Controls.Add(this._btnCancel);
      this.Controls.Add(this._btnNoToAll);
      this.Controls.Add(this._btnNo);
      this.Controls.Add(this._btnYesToAll);
      this.Controls.Add(this._btnYes);
      this.Controls.Add(this._pbxIcon);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(440, 180);
      this.Name = "MsgBoxYesNoAll";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "MsgBoxYesNoAll";
      ((System.ComponentModel.ISupportInitialize)(this._pbxIcon)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox _pbxIcon;
    private System.Windows.Forms.Button _btnYes;
    private System.Windows.Forms.Button _btnYesToAll;
    private System.Windows.Forms.Button _btnNo;
    private System.Windows.Forms.Button _btnNoToAll;
    private System.Windows.Forms.Button _btnCancel;
    private System.Windows.Forms.Label _lblContent;
    private System.Windows.Forms.Label _lblMainInstruction;
  }
}
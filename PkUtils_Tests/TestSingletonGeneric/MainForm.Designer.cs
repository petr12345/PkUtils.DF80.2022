namespace PK.TestSingletonGeneric
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
          this._btnClose = new System.Windows.Forms.Button();
          this._btnTest = new System.Windows.Forms.Button();
          this._label = new System.Windows.Forms.Label();
          this.SuspendLayout();
          // 
          // _btnClose
          // 
          this._btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
          this._btnClose.Location = new System.Drawing.Point(372, 99);
          this._btnClose.Name = "_btnClose";
          this._btnClose.Size = new System.Drawing.Size(75, 23);
          this._btnClose.TabIndex = 0;
          this._btnClose.Text = "Close";
          this._btnClose.UseVisualStyleBackColor = true;
          this._btnClose.Click += new System.EventHandler(this.Close_Click);
          // 
          // _btnTest
          // 
          this._btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
          this._btnTest.Location = new System.Drawing.Point(280, 99);
          this._btnTest.Name = "_btnTest";
          this._btnTest.Size = new System.Drawing.Size(75, 23);
          this._btnTest.TabIndex = 1;
          this._btnTest.Text = "Test!";
          this._btnTest.UseVisualStyleBackColor = true;
          this._btnTest.Click += new System.EventHandler(this.buttonTest_Click);
          // 
          // _label
          // 
          this._label.AutoSize = true;
          this._label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this._label.ForeColor = System.Drawing.Color.MediumBlue;
          this._label.Location = new System.Drawing.Point(13, 26);
          this._label.Name = "_label";
          this._label.Size = new System.Drawing.Size(430, 20);
          this._label.TabIndex = 2;
          this._label.Text = "Hit the \'Test!\' button to construct singleton object instance...";
          // 
          // MainForm
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(459, 140);
          this.Controls.Add(this._label);
          this.Controls.Add(this._btnTest);
          this.Controls.Add(this._btnClose);
          this.MinimumSize = new System.Drawing.Size(475, 175);
          this.Name = "MainForm";
          this.Text = "Singleton Test";
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.Button _btnTest;
        private System.Windows.Forms.Label _label;
    }
}


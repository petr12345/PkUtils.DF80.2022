namespace TestTgSchema
{
    partial class TaggingSchema_LinesEnumBasedUserCtrl
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
            this._labelLines = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _SchemaTextBxCtrl
            // 
            this._SchemaTextBxCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._SchemaTextBxCtrl.Font = new System.Drawing.Font("Moire", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._SchemaTextBxCtrl.Multiline = true;
            this._SchemaTextBxCtrl.Size = new System.Drawing.Size(244, 46);
            // 
            // _labelLines
            // 
            this._labelLines.AutoSize = true;
            this._labelLines.Location = new System.Drawing.Point(21, 21);
            this._labelLines.Name = "_labelLines";
            this._labelLines.Size = new System.Drawing.Size(162, 13);
            this._labelLines.TabIndex = 3;
            this._labelLines.Text = "Specify tagging schema for  lines";
            // 
            // TaggingSchema_LinesEnumBasedUserCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this._labelLines);
            this.Name = "TaggingSchema_LinesEnumBasedUserCtrl";
            this.Controls.SetChildIndex(this._SchemaTextBxCtrl, 0);
            this.Controls.SetChildIndex(this._labelLines, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _labelLines;
    }
}

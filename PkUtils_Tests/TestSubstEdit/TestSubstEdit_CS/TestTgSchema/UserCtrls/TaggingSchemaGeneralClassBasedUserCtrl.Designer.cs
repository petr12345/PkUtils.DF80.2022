using PK.TestTgSchema.TextBoxCtrls;

namespace PK.TestTgSchema.UserCtrls
{
    partial class TaggingSchemaGeneralClassBasedUserCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._SchemaTextBxCtrl = new TaggingSchemaTextBoxClassBasedCtrl();
            this.SuspendLayout();
            // 
            // _SchemaTextBxCtrl
            // 
            this._SchemaTextBxCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._SchemaTextBxCtrl.Location = new System.Drawing.Point(20, 52);
            this._SchemaTextBxCtrl.Name = "_SchemaTextBoxCtrl";
            this._SchemaTextBxCtrl.Size = new System.Drawing.Size(244, 20);
            this._SchemaTextBxCtrl.TabIndex = 0;
            // 
            // TaggingSchemaGeneralClassBasedUserCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._SchemaTextBxCtrl);
            this.Name = "TaggingSchemaGeneralClassBasedUserCtrl";
            this.Size = new System.Drawing.Size(267, 112);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected internal TaggingSchemaTextBoxClassBasedCtrl _SchemaTextBxCtrl;
    }
}

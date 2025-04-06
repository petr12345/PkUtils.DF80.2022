namespace TestTgSchema
{
    partial class TaggingSchema_ComponentslUserCtrl
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
            this._labelComponents = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _SchemaTextBxCtrl
            // 
            this._SchemaTextBxCtrl.Size = new System.Drawing.Size(228, 20);
            // 
            // _labelComponents
            // 
            this._labelComponents.AutoSize = true;
            this._labelComponents.Location = new System.Drawing.Point(21, 32);
            this._labelComponents.Name = "_labelComponents";
            this._labelComponents.Size = new System.Drawing.Size(196, 13);
            this._labelComponents.TabIndex = 2;
            this._labelComponents.Text = "Specify tagging schema for components";
            // 
            // TaggingSchema_ComponentslUserCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this._labelComponents);
            this.Name = "TaggingSchema_ComponentslUserCtrl";
            this.Size = new System.Drawing.Size(267, 93);
            this.Controls.SetChildIndex(this._SchemaTextBxCtrl, 0);
            this.Controls.SetChildIndex(this._labelComponents, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _labelComponents;

    }
}

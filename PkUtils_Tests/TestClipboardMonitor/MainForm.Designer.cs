namespace PK.TestClipMon
{
    public partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            _ctlClipboardText = new System.Windows.Forms.RichTextBox();
            _pictureBox = new System.Windows.Forms.PictureBox();
            _menuMain = new System.Windows.Forms.MenuStrip();
            _menuItemCurrentFormats = new System.Windows.Forms.ToolStripMenuItem();
            _menuItemSupportedFormats = new System.Windows.Forms.ToolStripMenuItem();

            ((System.ComponentModel.ISupportInitialize)_pictureBox).BeginInit();
            _menuMain.SuspendLayout();
            SuspendLayout();
            // 
            // 
            // _ctlClipboardText
            // 
            _ctlClipboardText.DetectUrls = false;
            _ctlClipboardText.Dock = System.Windows.Forms.DockStyle.Fill;
            _ctlClipboardText.Location = new System.Drawing.Point(0, 24);
            _ctlClipboardText.Name = "_ctlClipboardText";
            _ctlClipboardText.ReadOnly = true;
            _ctlClipboardText.Size = new System.Drawing.Size(364, 277);
            _ctlClipboardText.TabIndex = 0;
            _ctlClipboardText.Text = "";
            _ctlClipboardText.WordWrap = false;
            // 
            // _pictureBox
            // 
            _pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            _pictureBox.Location = new System.Drawing.Point(0, 24);
            _pictureBox.Name = "_pictureBox";
            _pictureBox.Size = new System.Drawing.Size(364, 277);
            _pictureBox.TabIndex = 1;
            _pictureBox.TabStop = false;
            // 
            // _menuMain
            // 
            _menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { _menuItemCurrentFormats, _menuItemSupportedFormats });
            _menuMain.Location = new System.Drawing.Point(0, 0);
            _menuMain.Name = "_menuMain";
            _menuMain.Size = new System.Drawing.Size(364, 24);
            _menuMain.TabIndex = 2;
            _menuMain.Text = "Main menu";
            // 
            // _menuItemCurrentFormats
            // 
            _menuItemCurrentFormats.Name = "_menuItemCurrentFormats";
            _menuItemCurrentFormats.Size = new System.Drawing.Size(105, 20);
            _menuItemCurrentFormats.Text = "Current Formats";
            // 
            // _menuItemSupportedFormats
            // 
            _menuItemSupportedFormats.Name = "_menuItemSupportedFormats";
            _menuItemSupportedFormats.Size = new System.Drawing.Size(120, 20);
            _menuItemSupportedFormats.Text = "Supported Formats";
            // 
            // MainForm
            // 
            AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            ClientSize = new System.Drawing.Size(364, 301);
            Controls.Add(_pictureBox);
            Controls.Add(_ctlClipboardText);
            Controls.Add(_menuMain);
            Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Location = new System.Drawing.Point(100, 100);
            MainMenuStrip = _menuMain;
            MinimumSize = new System.Drawing.Size(380, 340);
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "Clipboard Monitor Example";
            Load += frmMain_Load;

            ((System.ComponentModel.ISupportInitialize)_pictureBox).EndInit();
            _menuMain.ResumeLayout(false);
            _menuMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        #region Fields
        private System.Windows.Forms.MenuStrip _menuMain;
        private System.Windows.Forms.ToolStripMenuItem _menuItemCurrentFormats;
        private System.Windows.Forms.ToolStripMenuItem _menuItemSupportedFormats;
        // private System.Windows.Forms.ToolStripMenuItem mnuSupported;
        private System.Windows.Forms.PictureBox _pictureBox;
        private System.Windows.Forms.RichTextBox _ctlClipboardText;
        #endregion
    }

}


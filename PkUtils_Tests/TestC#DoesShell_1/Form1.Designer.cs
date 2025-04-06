
namespace WinTester
{
    public partial class Form1
    {
        #region Windows Form Designer generated code
        private System.Windows.Forms.Button _btnGetWindowsFolder;
private System.Windows.Forms.Button _btnGetSystemFolder;
private System.Windows.Forms.Button _btnGetSHBindToParent;
private System.Windows.Forms.Button _btnBrowseForFolder;
private System.Windows.Forms.Button _btnCustomBrowseForFolder;
private System.Windows.Forms.Label label1;
private System.ComponentModel.Container components = null;

        private void InitializeComponent()
        {
            this._btnGetWindowsFolder = new System.Windows.Forms.Button();
            this._btnGetSystemFolder = new System.Windows.Forms.Button();
            this._btnGetSHBindToParent = new System.Windows.Forms.Button();
            this._btnBrowseForFolder = new System.Windows.Forms.Button();
            this._btnCustomBrowseForFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this._btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _btnGetWindowsFolder
            // 
            this._btnGetWindowsFolder.Location = new System.Drawing.Point(8, 57);
            this._btnGetWindowsFolder.Name = "_btnGetWindowsFolder";
            this._btnGetWindowsFolder.Size = new System.Drawing.Size(154, 25);
            this._btnGetWindowsFolder.TabIndex = 2;
            this._btnGetWindowsFolder.Text = "Get Windows Folder";
            this._btnGetWindowsFolder.Click += new System.EventHandler(this.BtnGetWindowsFolder_Click);
            // 
            // _btnGetSystemFolder
            // 
            this._btnGetSystemFolder.Location = new System.Drawing.Point(8, 86);
            this._btnGetSystemFolder.Name = "_btnGetSystemFolder";
            this._btnGetSystemFolder.Size = new System.Drawing.Size(154, 25);
            this._btnGetSystemFolder.TabIndex = 3;
            this._btnGetSystemFolder.Text = "Get System Folder";
            this._btnGetSystemFolder.Click += new System.EventHandler(this.BtnGetSystemFolder_Click);
            // 
            // _btnGetSHBindToParent
            // 
            this._btnGetSHBindToParent.Location = new System.Drawing.Point(8, 115);
            this._btnGetSHBindToParent.Name = "_btnGetSHBindToParent";
            this._btnGetSHBindToParent.Size = new System.Drawing.Size(154, 25);
            this._btnGetSHBindToParent.TabIndex = 4;
            this._btnGetSHBindToParent.Text = "Get SHBindToParent";
            this._btnGetSHBindToParent.Click += new System.EventHandler(this.BtnGetSHBindToParent_Click);
            // 
            // _btnBrowseForFolder
            // 
            this._btnBrowseForFolder.Location = new System.Drawing.Point(8, 144);
            this._btnBrowseForFolder.Name = "_btnBrowseForFolder";
            this._btnBrowseForFolder.Size = new System.Drawing.Size(154, 25);
            this._btnBrowseForFolder.TabIndex = 5;
            this._btnBrowseForFolder.Text = "Browse for Folder...";
            this._btnBrowseForFolder.Click += new System.EventHandler(this.BtnBrowseForFolder_Click);
            // 
            // _btnCustomBrowseForFolder
            // 
            this._btnCustomBrowseForFolder.Location = new System.Drawing.Point(8, 173);
            this._btnCustomBrowseForFolder.Name = "_btnCustomBrowseForFolder";
            this._btnCustomBrowseForFolder.Size = new System.Drawing.Size(154, 25);
            this._btnCustomBrowseForFolder.TabIndex = 6;
            this._btnCustomBrowseForFolder.Text = "Custom Browse for Folder...";
            this._btnCustomBrowseForFolder.Click += new System.EventHandler(this.CustomBrowseForFolderButton_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(362, 38);
            this.label1.TabIndex = 1;
            this.label1.Text = "Some sample code for the first section. Trace them if you like, \r\nthey demonstare" +
    "s using some shell functions\r\n\r\n";
            // 
            // _btnClose
            // 
            this._btnClose.Location = new System.Drawing.Point(11, 228);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(154, 25);
            this._btnClose.TabIndex = 0;
            this._btnClose.Text = "Close";
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(380, 262);
            this.Controls.Add(this._btnClose);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._btnCustomBrowseForFolder);
            this.Controls.Add(this._btnBrowseForFolder);
            this.Controls.Add(this._btnGetSHBindToParent);
            this.Controls.Add(this._btnGetSystemFolder);
            this.Controls.Add(this._btnGetWindowsFolder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Shell Basics sample";
            this.ResumeLayout(false);

        }
        #endregion

        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        private System.Windows.Forms.Button _btnClose;
    }
}

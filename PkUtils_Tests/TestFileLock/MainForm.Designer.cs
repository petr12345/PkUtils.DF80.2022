namespace PK.TesFileLock
{
  partial class MainForm
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
            this._btnClose = new System.Windows.Forms.Button();
            this._btnTestFileLock = new System.Windows.Forms.Button();
            this._label = new System.Windows.Forms.Label();
            this._btnTestDirectoryLock = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this._lblLockedFile = new System.Windows.Forms.Label();
            this._textBxFileName = new System.Windows.Forms.TextBox();
            this._btnBrowseFile = new System.Windows.Forms.Button();
            this._groupBxFile = new System.Windows.Forms.GroupBox();
            this._btnWhoLockedTheFile = new System.Windows.Forms.Button();
            this._groupBxDirectory = new System.Windows.Forms.GroupBox();
            this._btnTestCreateFileInFolder = new System.Windows.Forms.Button();
            this._btnBrowseDirectory = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this._textBxDirectoryName = new System.Windows.Forms.TextBox();
            this._groupBxFile.SuspendLayout();
            this._groupBxDirectory.SuspendLayout();
            this.SuspendLayout();
            // 
            // _btnClose
            // 
            this._btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnClose.Location = new System.Drawing.Point(458, 303);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(86, 25);
            this._btnClose.TabIndex = 4;
            this._btnClose.Text = "Close";
            this._btnClose.UseVisualStyleBackColor = true;
            this._btnClose.Click += new System.EventHandler(this.Close_Click);
            // 
            // _btnTestFileLock
            // 
            this._btnTestFileLock.Location = new System.Drawing.Point(6, 66);
            this._btnTestFileLock.Name = "_btnTestFileLock";
            this._btnTestFileLock.Size = new System.Drawing.Size(93, 25);
            this._btnTestFileLock.TabIndex = 3;
            this._btnTestFileLock.Text = "Test FileLock";
            this._btnTestFileLock.UseVisualStyleBackColor = true;
            this._btnTestFileLock.Click += new System.EventHandler(this.OnButtonTestFileLock_Click);
            // 
            // _label
            // 
            this._label.AutoSize = true;
            this._label.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._label.ForeColor = System.Drawing.Color.MediumBlue;
            this._label.Location = new System.Drawing.Point(13, 17);
            this._label.Name = "_label";
            this._label.Size = new System.Drawing.Size(399, 18);
            this._label.TabIndex = 0;
            this._label.Text = "Hit the \'Test...\' button to test either a file or directory locking.";
            // 
            // _btnTestDirectoryLock
            // 
            this._btnTestDirectoryLock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._btnTestDirectoryLock.Location = new System.Drawing.Point(6, 62);
            this._btnTestDirectoryLock.Name = "_btnTestDirectoryLock";
            this._btnTestDirectoryLock.Size = new System.Drawing.Size(105, 25);
            this._btnTestDirectoryLock.TabIndex = 3;
            this._btnTestDirectoryLock.Text = "Test FoldeLock";
            this._btnTestDirectoryLock.UseVisualStyleBackColor = true;
            this._btnTestDirectoryLock.Click += new System.EventHandler(this.OnButtonTestDirectoryLock_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.MediumBlue;
            this.label1.Location = new System.Drawing.Point(13, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(304, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "Hit the same button again to release the lock.";
            // 
            // _lblLockedFile
            // 
            this._lblLockedFile.AutoSize = true;
            this._lblLockedFile.Location = new System.Drawing.Point(7, 28);
            this._lblLockedFile.Name = "_lblLockedFile";
            this._lblLockedFile.Size = new System.Drawing.Size(62, 13);
            this._lblLockedFile.TabIndex = 0;
            this._lblLockedFile.Text = "Locked file:";
            // 
            // _textBxFileName
            // 
            this._textBxFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textBxFileName.Location = new System.Drawing.Point(94, 25);
            this._textBxFileName.Name = "_textBxFileName";
            this._textBxFileName.ReadOnly = true;
            this._textBxFileName.Size = new System.Drawing.Size(376, 20);
            this._textBxFileName.TabIndex = 1;
            // 
            // _btnBrowseFile
            // 
            this._btnBrowseFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnBrowseFile.Location = new System.Drawing.Point(475, 25);
            this._btnBrowseFile.Name = "_btnBrowseFile";
            this._btnBrowseFile.Size = new System.Drawing.Size(29, 23);
            this._btnBrowseFile.TabIndex = 2;
            this._btnBrowseFile.Text = "...";
            this._btnBrowseFile.UseVisualStyleBackColor = true;
            this._btnBrowseFile.Click += new System.EventHandler(this.OnBtnBrowseFile_Click);
            // 
            // _groupBxFile
            // 
            this._groupBxFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._groupBxFile.Controls.Add(this._btnWhoLockedTheFile);
            this._groupBxFile.Controls.Add(this._btnTestFileLock);
            this._groupBxFile.Controls.Add(this._btnBrowseFile);
            this._groupBxFile.Controls.Add(this._lblLockedFile);
            this._groupBxFile.Controls.Add(this._textBxFileName);
            this._groupBxFile.Location = new System.Drawing.Point(16, 67);
            this._groupBxFile.Name = "_groupBxFile";
            this._groupBxFile.Size = new System.Drawing.Size(530, 100);
            this._groupBxFile.TabIndex = 2;
            this._groupBxFile.TabStop = false;
            this._groupBxFile.Text = "File Locking Test";
            // 
            // _btnWhoLockedTheFile
            // 
            this._btnWhoLockedTheFile.Location = new System.Drawing.Point(115, 66);
            this._btnWhoLockedTheFile.Name = "_btnWhoLockedTheFile";
            this._btnWhoLockedTheFile.Size = new System.Drawing.Size(139, 25);
            this._btnWhoLockedTheFile.TabIndex = 4;
            this._btnWhoLockedTheFile.Text = "See who locked the file";
            this._btnWhoLockedTheFile.UseVisualStyleBackColor = true;
            this._btnWhoLockedTheFile.Click += new System.EventHandler(this._btnWhoLockedTheFile_Click);
            // 
            // _groupBxDirectory
            // 
            this._groupBxDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._groupBxDirectory.Controls.Add(this._btnTestCreateFileInFolder);
            this._groupBxDirectory.Controls.Add(this._btnBrowseDirectory);
            this._groupBxDirectory.Controls.Add(this.label2);
            this._groupBxDirectory.Controls.Add(this._textBxDirectoryName);
            this._groupBxDirectory.Controls.Add(this._btnTestDirectoryLock);
            this._groupBxDirectory.Location = new System.Drawing.Point(14, 185);
            this._groupBxDirectory.Name = "_groupBxDirectory";
            this._groupBxDirectory.Size = new System.Drawing.Size(530, 100);
            this._groupBxDirectory.TabIndex = 3;
            this._groupBxDirectory.TabStop = false;
            this._groupBxDirectory.Text = "Directory Locking Test";
            // 
            // _btnTestCreateFileInFolder
            // 
            this._btnTestCreateFileInFolder.Location = new System.Drawing.Point(117, 62);
            this._btnTestCreateFileInFolder.Name = "_btnTestCreateFileInFolder";
            this._btnTestCreateFileInFolder.Size = new System.Drawing.Size(233, 25);
            this._btnTestCreateFileInFolder.TabIndex = 4;
            this._btnTestCreateFileInFolder.Text = "Test Creating New File In This Folder";
            this._btnTestCreateFileInFolder.UseVisualStyleBackColor = true;
            this._btnTestCreateFileInFolder.Click += new System.EventHandler(this.OnButtonTestCreateFileInFolder_Click);
            // 
            // _btnBrowseDirectory
            // 
            this._btnBrowseDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnBrowseDirectory.Location = new System.Drawing.Point(475, 25);
            this._btnBrowseDirectory.Name = "_btnBrowseDirectory";
            this._btnBrowseDirectory.Size = new System.Drawing.Size(29, 23);
            this._btnBrowseDirectory.TabIndex = 2;
            this._btnBrowseDirectory.Text = "...";
            this._btnBrowseDirectory.UseVisualStyleBackColor = true;
            this._btnBrowseDirectory.Click += new System.EventHandler(this.OnBtnBrowseDirectory_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Locked directory:";
            // 
            // _textBxDirectoryName
            // 
            this._textBxDirectoryName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textBxDirectoryName.Location = new System.Drawing.Point(102, 25);
            this._textBxDirectoryName.Name = "_textBxDirectoryName";
            this._textBxDirectoryName.ReadOnly = true;
            this._textBxDirectoryName.Size = new System.Drawing.Size(368, 20);
            this._textBxDirectoryName.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 340);
            this.Controls.Add(this._groupBxDirectory);
            this.Controls.Add(this._groupBxFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._label);
            this.Controls.Add(this._btnClose);
            this.MinimumSize = new System.Drawing.Size(575, 375);
            this.Name = "MainForm";
            this.Text = "Test FileLock and FolderLock";
            this._groupBxFile.ResumeLayout(false);
            this._groupBxFile.PerformLayout();
            this._groupBxDirectory.ResumeLayout(false);
            this._groupBxDirectory.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button _btnClose;
    private System.Windows.Forms.Button _btnTestFileLock;
    private System.Windows.Forms.Label _label;
    private System.Windows.Forms.Button _btnTestDirectoryLock;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label _lblLockedFile;
    private System.Windows.Forms.TextBox _textBxFileName;
    private System.Windows.Forms.Button _btnBrowseFile;
    private System.Windows.Forms.GroupBox _groupBxFile;
    private System.Windows.Forms.GroupBox _groupBxDirectory;
    private System.Windows.Forms.Button _btnBrowseDirectory;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox _textBxDirectoryName;
    private System.Windows.Forms.Button _btnTestCreateFileInFolder;
        private System.Windows.Forms.Button _btnWhoLockedTheFile;
    }
}


namespace TestCompositeImages.UI
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
            this.components = new System.ComponentModel.Container();
            this._pnlBottom = new System.Windows.Forms.Panel();
            this._groupBxProcessingOptions = new System.Windows.Forms.GroupBox();
            this._btnBrowseDirectory = new System.Windows.Forms.Button();
            this._labelImagesFolder = new System.Windows.Forms.Label();
            this._textBxImagesFolder = new System.Windows.Forms.TextBox();
            this._btnExit = new System.Windows.Forms.Button();
            this._btnTestImagesProcessing = new System.Windows.Forms.Button();
            this._splitContainer = new System.Windows.Forms.SplitContainer();
            this._groupBxResultingImage = new System.Windows.Forms.GroupBox();
            this._btnImageSaveAs = new System.Windows.Forms.Button();
            this._pictureBox = new System.Windows.Forms.PictureBox();
            this._labelImagePreview = new System.Windows.Forms.Label();
            this._groupBxServerLog = new System.Windows.Forms.GroupBox();
            this._btnStartStopListening = new System.Windows.Forms.Button();
            this._btnCleanLogHistory = new System.Windows.Forms.Button();
            this._txtBxDump = new System.Windows.Forms.TextBox();
            this._timer = new System.Windows.Forms.Timer(this.components);
            this._pnlBottom.SuspendLayout();
            this._groupBxProcessingOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._splitContainer)).BeginInit();
            this._splitContainer.Panel1.SuspendLayout();
            this._splitContainer.Panel2.SuspendLayout();
            this._splitContainer.SuspendLayout();
            this._groupBxResultingImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
            this._groupBxServerLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // _pnlBottom
            // 
            this._pnlBottom.BackColor = System.Drawing.SystemColors.Control;
            this._pnlBottom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._pnlBottom.Controls.Add(this._groupBxProcessingOptions);
            this._pnlBottom.Controls.Add(this._btnExit);
            this._pnlBottom.Controls.Add(this._btnTestImagesProcessing);
            this._pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._pnlBottom.Location = new System.Drawing.Point(0, 302);
            this._pnlBottom.Name = "_pnlBottom";
            this._pnlBottom.Size = new System.Drawing.Size(664, 179);
            this._pnlBottom.TabIndex = 2;
            // 
            // _groupBxProcessingOptions
            // 
            this._groupBxProcessingOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._groupBxProcessingOptions.Controls.Add(this._btnBrowseDirectory);
            this._groupBxProcessingOptions.Controls.Add(this._labelImagesFolder);
            this._groupBxProcessingOptions.Controls.Add(this._textBxImagesFolder);
            this._groupBxProcessingOptions.Location = new System.Drawing.Point(2, 3);
            this._groupBxProcessingOptions.Name = "_groupBxProcessingOptions";
            this._groupBxProcessingOptions.Size = new System.Drawing.Size(648, 133);
            this._groupBxProcessingOptions.TabIndex = 4;
            this._groupBxProcessingOptions.TabStop = false;
            this._groupBxProcessingOptions.Text = "Images Processing Options";
            // 
            // _btnBrowseDirectory
            // 
            this._btnBrowseDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnBrowseDirectory.Location = new System.Drawing.Point(593, 25);
            this._btnBrowseDirectory.Name = "_btnBrowseDirectory";
            this._btnBrowseDirectory.Size = new System.Drawing.Size(29, 23);
            this._btnBrowseDirectory.TabIndex = 2;
            this._btnBrowseDirectory.Text = "...";
            this._btnBrowseDirectory.UseVisualStyleBackColor = true;
            this._btnBrowseDirectory.Click += new System.EventHandler(this.btnBrowseDirectory_Click);
            // 
            // _labelImagesFolder
            // 
            this._labelImagesFolder.AutoSize = true;
            this._labelImagesFolder.Location = new System.Drawing.Point(7, 28);
            this._labelImagesFolder.Name = "_labelImagesFolder";
            this._labelImagesFolder.Size = new System.Drawing.Size(76, 13);
            this._labelImagesFolder.TabIndex = 0;
            this._labelImagesFolder.Text = "Source Folder:";
            // 
            // _textBxImagesFolder
            // 
            this._textBxImagesFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textBxImagesFolder.Location = new System.Drawing.Point(102, 25);
            this._textBxImagesFolder.Name = "_textBxImagesFolder";
            this._textBxImagesFolder.Size = new System.Drawing.Size(486, 20);
            this._textBxImagesFolder.TabIndex = 1;
            // 
            // _btnExit
            // 
            this._btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnExit.Location = new System.Drawing.Point(565, 142);
            this._btnExit.Name = "_btnExit";
            this._btnExit.Size = new System.Drawing.Size(84, 23);
            this._btnExit.TabIndex = 3;
            this._btnExit.Text = "Exit";
            this._btnExit.UseVisualStyleBackColor = true;
            this._btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // _btnTestImagesProcessing
            // 
            this._btnTestImagesProcessing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnTestImagesProcessing.Location = new System.Drawing.Point(398, 142);
            this._btnTestImagesProcessing.Name = "_btnTestImagesProcessing";
            this._btnTestImagesProcessing.Size = new System.Drawing.Size(151, 25);
            this._btnTestImagesProcessing.TabIndex = 3;
            this._btnTestImagesProcessing.Text = "Start Images Processing";
            this._btnTestImagesProcessing.UseVisualStyleBackColor = true;
            this._btnTestImagesProcessing.Click += new System.EventHandler(this.btnTestImagesProcessing_Click);
            // 
            // _splitContainer
            // 
            this._splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitContainer.Location = new System.Drawing.Point(0, 0);
            this._splitContainer.Name = "_splitContainer";
            // 
            // _splitContainer.Panel1
            // 
            this._splitContainer.Panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this._splitContainer.Panel1.Controls.Add(this._groupBxResultingImage);
            this._splitContainer.Panel1.Controls.Add(this._labelImagePreview);
            this._splitContainer.Panel1MinSize = 180;
            // 
            // _splitContainer.Panel2
            // 
            this._splitContainer.Panel2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this._splitContainer.Panel2.Controls.Add(this._groupBxServerLog);
            this._splitContainer.Panel2MinSize = 220;
            this._splitContainer.Size = new System.Drawing.Size(664, 302);
            this._splitContainer.SplitterDistance = 206;
            this._splitContainer.TabIndex = 3;
            // 
            // _groupBxResultingImage
            // 
            this._groupBxResultingImage.Controls.Add(this._btnImageSaveAs);
            this._groupBxResultingImage.Controls.Add(this._pictureBox);
            this._groupBxResultingImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this._groupBxResultingImage.Location = new System.Drawing.Point(0, 0);
            this._groupBxResultingImage.Name = "_groupBxResultingImage";
            this._groupBxResultingImage.Size = new System.Drawing.Size(206, 302);
            this._groupBxResultingImage.TabIndex = 3;
            this._groupBxResultingImage.TabStop = false;
            this._groupBxResultingImage.Text = "Resulting Image Preview";
            // 
            // _btnImageSaveAs
            // 
            this._btnImageSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnImageSaveAs.Location = new System.Drawing.Point(108, 273);
            this._btnImageSaveAs.Name = "_btnImageSaveAs";
            this._btnImageSaveAs.Size = new System.Drawing.Size(84, 23);
            this._btnImageSaveAs.TabIndex = 2;
            this._btnImageSaveAs.Text = "Save As...";
            this._btnImageSaveAs.UseVisualStyleBackColor = true;
            this._btnImageSaveAs.Click += new System.EventHandler(this.btnImageSaveAs_Click);
            // 
            // _pictureBox
            // 
            this._pictureBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this._pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pictureBox.Image = global::TestCompositeImages.Properties.Resources.C_sharp;
            this._pictureBox.Location = new System.Drawing.Point(3, 16);
            this._pictureBox.Name = "_pictureBox";
            this._pictureBox.Size = new System.Drawing.Size(200, 283);
            this._pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._pictureBox.TabIndex = 1;
            this._pictureBox.TabStop = false;
            // 
            // _labelImagePreview
            // 
            this._labelImagePreview.AutoSize = true;
            this._labelImagePreview.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this._labelImagePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._labelImagePreview.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelImagePreview.Location = new System.Drawing.Point(4, 4);
            this._labelImagePreview.Margin = new System.Windows.Forms.Padding(3);
            this._labelImagePreview.Name = "_labelImagePreview";
            this._labelImagePreview.Size = new System.Drawing.Size(172, 15);
            this._labelImagePreview.TabIndex = 2;
            this._labelImagePreview.Text = "The resulting image preview:";
            // 
            // _groupBxServerLog
            // 
            this._groupBxServerLog.Controls.Add(this._btnStartStopListening);
            this._groupBxServerLog.Controls.Add(this._btnCleanLogHistory);
            this._groupBxServerLog.Controls.Add(this._txtBxDump);
            this._groupBxServerLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this._groupBxServerLog.Location = new System.Drawing.Point(0, 0);
            this._groupBxServerLog.Name = "_groupBxServerLog";
            this._groupBxServerLog.Size = new System.Drawing.Size(454, 302);
            this._groupBxServerLog.TabIndex = 1;
            this._groupBxServerLog.TabStop = false;
            this._groupBxServerLog.Text = "Outgoing Trace Messages";
            // 
            // _btnStartStopListening
            // 
            this._btnStartStopListening.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnStartStopListening.Location = new System.Drawing.Point(234, 273);
            this._btnStartStopListening.Name = "_btnStartStopListening";
            this._btnStartStopListening.Size = new System.Drawing.Size(105, 23);
            this._btnStartStopListening.TabIndex = 1;
            this._btnStartStopListening.Text = "Stop Listening";
            this._btnStartStopListening.UseVisualStyleBackColor = true;
            this._btnStartStopListening.Click += new System.EventHandler(this.btnStartStopListening_Click);
            // 
            // _btnCleanLogHistory
            // 
            this._btnCleanLogHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnCleanLogHistory.Location = new System.Drawing.Point(356, 273);
            this._btnCleanLogHistory.Name = "_btnCleanLogHistory";
            this._btnCleanLogHistory.Size = new System.Drawing.Size(84, 23);
            this._btnCleanLogHistory.TabIndex = 2;
            this._btnCleanLogHistory.Text = "Clean";
            this._btnCleanLogHistory.UseVisualStyleBackColor = true;
            this._btnCleanLogHistory.Click += new System.EventHandler(this.btnCleanLogHistory_Click);
            // 
            // _txtBxDump
            // 
            this._txtBxDump.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._txtBxDump.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this._txtBxDump.Location = new System.Drawing.Point(8, 16);
            this._txtBxDump.Multiline = true;
            this._txtBxDump.Name = "_txtBxDump";
            this._txtBxDump.ReadOnly = true;
            this._txtBxDump.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._txtBxDump.Size = new System.Drawing.Size(433, 251);
            this._txtBxDump.TabIndex = 0;
            // 
            // _timer
            // 
            this._timer.Interval = 1500;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 481);
            this.Controls.Add(this._splitContainer);
            this.Controls.Add(this._pnlBottom);
            this.MinimumSize = new System.Drawing.Size(420, 280);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Composite Images Dataflow Test";
            this._pnlBottom.ResumeLayout(false);
            this._groupBxProcessingOptions.ResumeLayout(false);
            this._groupBxProcessingOptions.PerformLayout();
            this._splitContainer.Panel1.ResumeLayout(false);
            this._splitContainer.Panel1.PerformLayout();
            this._splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitContainer)).EndInit();
            this._splitContainer.ResumeLayout(false);
            this._groupBxResultingImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
            this._groupBxServerLog.ResumeLayout(false);
            this._groupBxServerLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _pnlBottom;
        private System.Windows.Forms.SplitContainer _splitContainer;
        private System.Windows.Forms.Label _labelImagePreview;
        private System.Windows.Forms.GroupBox _groupBxServerLog;
        private System.Windows.Forms.Button _btnExit;
        private System.Windows.Forms.Button _btnStartStopListening;
        private System.Windows.Forms.Button _btnCleanLogHistory;
        private System.Windows.Forms.TextBox _txtBxDump;
        private System.Windows.Forms.GroupBox _groupBxResultingImage;
        private System.Windows.Forms.Button _btnImageSaveAs;
        private System.Windows.Forms.PictureBox _pictureBox;
        private System.Windows.Forms.Timer _timer;
        private System.Windows.Forms.GroupBox _groupBxProcessingOptions;
        private System.Windows.Forms.Button _btnBrowseDirectory;
        private System.Windows.Forms.Label _labelImagesFolder;
        private System.Windows.Forms.TextBox _textBxImagesFolder;
        private System.Windows.Forms.Button _btnTestImagesProcessing;
    }
}


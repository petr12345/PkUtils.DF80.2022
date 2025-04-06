namespace PK.TestPropertyGridCustomCollectionEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            btnAssign = new System.Windows.Forms.Button();
            _propertyGrid = new System.Windows.Forms.PropertyGrid();
            _textBoxMessages = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            _btnClose = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // btnAssign
            // 
            btnAssign.Location = new System.Drawing.Point(14, 14);
            btnAssign.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnAssign.Name = "btnAssign";
            btnAssign.Size = new System.Drawing.Size(350, 27);
            btnAssign.TabIndex = 0;
            btnAssign.Text = "Click to assign class instance and edit the member collection";
            btnAssign.UseVisualStyleBackColor = true;
            btnAssign.Click += OnBtnAssign_Click;
            // 
            // _propertyGrid
            // 
            _propertyGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _propertyGrid.Location = new System.Drawing.Point(14, 52);
            _propertyGrid.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            _propertyGrid.Name = "_propertyGrid";
            _propertyGrid.Size = new System.Drawing.Size(430, 219);
            _propertyGrid.TabIndex = 1;
            // 
            // _textBoxMessages
            // 
            _textBoxMessages.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            _textBoxMessages.Location = new System.Drawing.Point(14, 301);
            _textBoxMessages.Multiline = true;
            _textBoxMessages.Name = "_textBoxMessages";
            _textBoxMessages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            _textBoxMessages.Size = new System.Drawing.Size(430, 110);
            _textBoxMessages.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(14, 283);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(112, 15);
            label1.TabIndex = 3;
            label1.Text = "Notification history:";
            // 
            // _btnClose
            // 
            _btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            _btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            _btnClose.Location = new System.Drawing.Point(354, 426);
            _btnClose.Name = "_btnClose";
            _btnClose.Size = new System.Drawing.Size(90, 27);
            _btnClose.TabIndex = 4;
            _btnClose.Text = "Close";
            _btnClose.UseVisualStyleBackColor = true;
            _btnClose.Click += BtnClose_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(464, 461);
            Controls.Add(_btnClose);
            Controls.Add(label1);
            Controls.Add(_textBoxMessages);
            Controls.Add(_propertyGrid);
            Controls.Add(btnAssign);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MinimumSize = new System.Drawing.Size(480, 480);
            Name = "MainForm";
            Text = "Demo for Custom Collection Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnAssign;
        private System.Windows.Forms.PropertyGrid _propertyGrid;
        private System.Windows.Forms.TextBox _textBoxMessages;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _btnClose;
    }
}


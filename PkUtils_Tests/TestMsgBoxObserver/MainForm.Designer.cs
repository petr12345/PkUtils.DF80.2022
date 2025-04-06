namespace PK.TestMsgBoxObserver
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
            this._btnShowMsgBox = new System.Windows.Forms.Button();
            this._textBoxMessages = new System.Windows.Forms.TextBox();
            this._buttonClear = new System.Windows.Forms.Button();
            this._buttonExit = new System.Windows.Forms.Button();
            this._label = new System.Windows.Forms.Label();
            this._btnShowTaskDialog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _btnShowMsgBox
            // 
            this._btnShowMsgBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._btnShowMsgBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._btnShowMsgBox.Location = new System.Drawing.Point(17, 202);
            this._btnShowMsgBox.Name = "_btnShowMsgBox";
            this._btnShowMsgBox.Size = new System.Drawing.Size(112, 27);
            this._btnShowMsgBox.TabIndex = 0;
            this._btnShowMsgBox.Text = "Invoke MessageBox";
            this._btnShowMsgBox.Click += new System.EventHandler(this.buttonShowMessageBox_Click);
            // 
            // _textBoxMessages
            // 
            this._textBoxMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxMessages.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this._textBoxMessages.Location = new System.Drawing.Point(12, 43);
            this._textBoxMessages.Multiline = true;
            this._textBoxMessages.Name = "_textBoxMessages";
            this._textBoxMessages.ReadOnly = true;
            this._textBoxMessages.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._textBoxMessages.Size = new System.Drawing.Size(455, 141);
            this._textBoxMessages.TabIndex = 5;
            this._textBoxMessages.TabStop = false;
            // 
            // _buttonClear
            // 
            this._buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._buttonClear.Location = new System.Drawing.Point(304, 202);
            this._buttonClear.Name = "_buttonClear";
            this._buttonClear.Size = new System.Drawing.Size(75, 27);
            this._buttonClear.TabIndex = 2;
            this._buttonClear.Text = "Clear";
            this._buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // _buttonExit
            // 
            this._buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonExit.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._buttonExit.Location = new System.Drawing.Point(392, 202);
            this._buttonExit.Name = "_buttonExit";
            this._buttonExit.Size = new System.Drawing.Size(75, 27);
            this._buttonExit.TabIndex = 3;
            this._buttonExit.Text = "Exit";
            this._buttonExit.UseVisualStyleBackColor = true;
            this._buttonExit.Click += new System.EventHandler(this._buttonExit_Click);
            // 
            // _label
            // 
            this._label.AutoSize = true;
            this._label.Location = new System.Drawing.Point(12, 20);
            this._label.Name = "_label";
            this._label.Size = new System.Drawing.Size(170, 13);
            this._label.TabIndex = 4;
            this._label.Text = "History of MsgBoxObserver events";
            // 
            // _btnShowTaskDialog
            // 
            this._btnShowTaskDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._btnShowTaskDialog.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._btnShowTaskDialog.Location = new System.Drawing.Point(142, 202);
            this._btnShowTaskDialog.Name = "_btnShowTaskDialog";
            this._btnShowTaskDialog.Size = new System.Drawing.Size(112, 27);
            this._btnShowTaskDialog.TabIndex = 1;
            this._btnShowTaskDialog.Text = "Invoke TaskDialog";
            this._btnShowTaskDialog.Click += new System.EventHandler(this.btnShowTaskDialog_Click);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(479, 237);
            this.Controls.Add(this._btnShowTaskDialog);
            this.Controls.Add(this._label);
            this.Controls.Add(this._buttonExit);
            this.Controls.Add(this._buttonClear);
            this.Controls.Add(this._textBoxMessages);
            this.Controls.Add(this._btnShowMsgBox);
            this.MinimumSize = new System.Drawing.Size(495, 275);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MsgBoxObserver Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #region Fields

        private System.Windows.Forms.Button _btnShowMsgBox;
        private System.Windows.Forms.TextBox _textBoxMessages;
        private System.Windows.Forms.Button _buttonClear;
        private System.Windows.Forms.Button _buttonExit;
        private System.Windows.Forms.Label _label;
        private System.Windows.Forms.Button _btnShowTaskDialog;
        #endregion // Fields
    }
}


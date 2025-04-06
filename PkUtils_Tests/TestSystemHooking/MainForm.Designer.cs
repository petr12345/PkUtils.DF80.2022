namespace PK.TestSystemHooking
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
            this._buttonInstall = new System.Windows.Forms.Button();
            this._buttonUninstall = new System.Windows.Forms.Button();
            this._textBoxMessages = new System.Windows.Forms.TextBox();
            this._buttonClear = new System.Windows.Forms.Button();
            this._groupBoxChecks = new System.Windows.Forms.GroupBox();
            this._checkBx_WH_MOUSE_LL = new System.Windows.Forms.CheckBox();
            this._checkBx_WH_CALLWNDPROCRET = new System.Windows.Forms.CheckBox();
            this._checkBx_WH_KEYBOARD_LL = new System.Windows.Forms.CheckBox();
            this._checkBx_WH_KEYBOARD = new System.Windows.Forms.CheckBox();
            this._checkBx_WH_MOUSE = new System.Windows.Forms.CheckBox();
            this._buttonExit = new System.Windows.Forms.Button();
            this._groupBoxChecks.SuspendLayout();
            this.SuspendLayout();
            // 
            // _buttonInstall
            // 
            this._buttonInstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonInstall.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._buttonInstall.Location = new System.Drawing.Point(457, 38);
            this._buttonInstall.Name = "_buttonInstall";
            this._buttonInstall.Size = new System.Drawing.Size(106, 29);
            this._buttonInstall.TabIndex = 1;
            this._buttonInstall.Text = "&Install Hooks";
            this._buttonInstall.Click += new System.EventHandler(this.ButtonInstall_Click);
            // 
            // _buttonUninstall
            // 
            this._buttonUninstall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonUninstall.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._buttonUninstall.Location = new System.Drawing.Point(457, 74);
            this._buttonUninstall.Name = "_buttonUninstall";
            this._buttonUninstall.Size = new System.Drawing.Size(106, 28);
            this._buttonUninstall.TabIndex = 2;
            this._buttonUninstall.Text = "&Uninstall Hooks";
            this._buttonUninstall.Click += new System.EventHandler(this.OnButtonUninstall_Click);
            // 
            // _textBoxMessages
            // 
            this._textBoxMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._textBoxMessages.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this._textBoxMessages.Location = new System.Drawing.Point(0, 198);
            this._textBoxMessages.Multiline = true;
            this._textBoxMessages.Name = "_textBoxMessages";
            this._textBoxMessages.ReadOnly = true;
            this._textBoxMessages.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._textBoxMessages.Size = new System.Drawing.Size(578, 192);
            this._textBoxMessages.TabIndex = 5;
            this._textBoxMessages.TabStop = false;
            // 
            // _buttonClear
            // 
            this._buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._buttonClear.Location = new System.Drawing.Point(457, 109);
            this._buttonClear.Name = "_buttonClear";
            this._buttonClear.Size = new System.Drawing.Size(106, 29);
            this._buttonClear.TabIndex = 3;
            this._buttonClear.Text = "Clear";
            this._buttonClear.Click += new System.EventHandler(this.OnButtonClear_Click);
            // 
            // _groupBoxChecks
            // 
            this._groupBoxChecks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._groupBoxChecks.Controls.Add(this._checkBx_WH_MOUSE_LL);
            this._groupBoxChecks.Controls.Add(this._checkBx_WH_CALLWNDPROCRET);
            this._groupBoxChecks.Controls.Add(this._checkBx_WH_KEYBOARD_LL);
            this._groupBoxChecks.Controls.Add(this._checkBx_WH_KEYBOARD);
            this._groupBoxChecks.Controls.Add(this._checkBx_WH_MOUSE);
            this._groupBoxChecks.Location = new System.Drawing.Point(16, 14);
            this._groupBoxChecks.Name = "_groupBoxChecks";
            this._groupBoxChecks.Size = new System.Drawing.Size(390, 164);
            this._groupBoxChecks.TabIndex = 0;
            this._groupBoxChecks.TabStop = false;
            this._groupBoxChecks.Text = "Hook Types Tested";
            // 
            // _checkBx_WH_MOUSE_LL
            // 
            this._checkBx_WH_MOUSE_LL.AutoSize = true;
            this._checkBx_WH_MOUSE_LL.Location = new System.Drawing.Point(19, 79);
            this._checkBx_WH_MOUSE_LL.Name = "_checkBx_WH_MOUSE_LL";
            this._checkBx_WH_MOUSE_LL.Size = new System.Drawing.Size(135, 21);
            this._checkBx_WH_MOUSE_LL.TabIndex = 2;
            this._checkBx_WH_MOUSE_LL.Text = "WH_MOUSE_LL";
            this._checkBx_WH_MOUSE_LL.UseVisualStyleBackColor = true;
            this._checkBx_WH_MOUSE_LL.CheckedChanged += new System.EventHandler(this.OnCheckBx_CheckedChanged);
            // 
            // _checkBx_WH_CALLWNDPROCRET
            // 
            this._checkBx_WH_CALLWNDPROCRET.AutoSize = true;
            this._checkBx_WH_CALLWNDPROCRET.Location = new System.Drawing.Point(19, 135);
            this._checkBx_WH_CALLWNDPROCRET.Name = "_checkBx_WH_CALLWNDPROCRET";
            this._checkBx_WH_CALLWNDPROCRET.Size = new System.Drawing.Size(195, 21);
            this._checkBx_WH_CALLWNDPROCRET.TabIndex = 4;
            this._checkBx_WH_CALLWNDPROCRET.Text = "WH_CALLWNDPROCRET";
            this._checkBx_WH_CALLWNDPROCRET.UseVisualStyleBackColor = true;
            this._checkBx_WH_CALLWNDPROCRET.CheckedChanged += new System.EventHandler(this.OnCheckBx_CheckedChanged);
            // 
            // _checkBx_WH_KEYBOARD_LL
            // 
            this._checkBx_WH_KEYBOARD_LL.AutoSize = true;
            this._checkBx_WH_KEYBOARD_LL.Location = new System.Drawing.Point(19, 107);
            this._checkBx_WH_KEYBOARD_LL.Name = "_checkBx_WH_KEYBOARD_LL";
            this._checkBx_WH_KEYBOARD_LL.Size = new System.Drawing.Size(161, 21);
            this._checkBx_WH_KEYBOARD_LL.TabIndex = 3;
            this._checkBx_WH_KEYBOARD_LL.Text = "WH_KEYBOARD_LL";
            this._checkBx_WH_KEYBOARD_LL.UseVisualStyleBackColor = true;
            this._checkBx_WH_KEYBOARD_LL.CheckedChanged += new System.EventHandler(this.OnCheckBx_CheckedChanged);
            // 
            // _checkBx_WH_KEYBOARD
            // 
            this._checkBx_WH_KEYBOARD.AutoSize = true;
            this._checkBx_WH_KEYBOARD.Location = new System.Drawing.Point(19, 51);
            this._checkBx_WH_KEYBOARD.Name = "_checkBx_WH_KEYBOARD";
            this._checkBx_WH_KEYBOARD.Size = new System.Drawing.Size(137, 21);
            this._checkBx_WH_KEYBOARD.TabIndex = 1;
            this._checkBx_WH_KEYBOARD.Text = "WH_KEYBOARD";
            this._checkBx_WH_KEYBOARD.UseVisualStyleBackColor = true;
            this._checkBx_WH_KEYBOARD.CheckedChanged += new System.EventHandler(this.OnCheckBx_CheckedChanged);
            // 
            // _checkBx_WH_MOUSE
            // 
            this._checkBx_WH_MOUSE.AutoSize = true;
            this._checkBx_WH_MOUSE.Location = new System.Drawing.Point(19, 23);
            this._checkBx_WH_MOUSE.Name = "_checkBx_WH_MOUSE";
            this._checkBx_WH_MOUSE.Size = new System.Drawing.Size(111, 21);
            this._checkBx_WH_MOUSE.TabIndex = 0;
            this._checkBx_WH_MOUSE.Text = "WH_MOUSE";
            this._checkBx_WH_MOUSE.UseVisualStyleBackColor = true;
            this._checkBx_WH_MOUSE.CheckedChanged += new System.EventHandler(this.OnCheckBx_CheckedChanged);
            // 
            // _buttonExit
            // 
            this._buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonExit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._buttonExit.Location = new System.Drawing.Point(457, 145);
            this._buttonExit.Name = "_buttonExit";
            this._buttonExit.Size = new System.Drawing.Size(106, 29);
            this._buttonExit.TabIndex = 4;
            this._buttonExit.Text = "Exit";
            this._buttonExit.Click += new System.EventHandler(this.OnButtonExit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(578, 414);
            this.Controls.Add(this._buttonExit);
            this.Controls.Add(this._groupBoxChecks);
            this.Controls.Add(this._buttonClear);
            this.Controls.Add(this._textBoxMessages);
            this.Controls.Add(this._buttonUninstall);
            this.Controls.Add(this._buttonInstall);
            this.MinimumSize = new System.Drawing.Size(540, 404);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "System Hook Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFor_FormClosing);
            this.Load += new System.EventHandler(this.MainFor_Load);
            this._groupBoxChecks.ResumeLayout(false);
            this._groupBoxChecks.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #region Fields

        private System.Windows.Forms.Button _buttonInstall;
        private System.Windows.Forms.Button _buttonUninstall;
        private System.Windows.Forms.TextBox _textBoxMessages;
        private System.Windows.Forms.Button _buttonClear;
        #endregion // Fields
        private System.Windows.Forms.Button _buttonExit;
        private System.Windows.Forms.GroupBox _groupBoxChecks;

        private System.Windows.Forms.CheckBox _checkBx_WH_MOUSE;
        private System.Windows.Forms.CheckBox _checkBx_WH_KEYBOARD;
        private System.Windows.Forms.CheckBox _checkBx_WH_MOUSE_LL;
        private System.Windows.Forms.CheckBox _checkBx_WH_KEYBOARD_LL;
        private System.Windows.Forms.CheckBox _checkBx_WH_CALLWNDPROCRET;
    }
}


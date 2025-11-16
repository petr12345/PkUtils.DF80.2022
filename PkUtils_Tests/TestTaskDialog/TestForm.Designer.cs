///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// The Code Project Open License Notice
// 
// This software is a Derivative Work based upon a Code Project article
// Vista TaskDialog Wrapper and Emulator
// http://www.codeproject.com/Articles/21276/Vista-TaskDialog-Wrapper-and-Emulator
//
// The Code Project Open License (CPOL) text is available at
// http://www.codeproject.com/info/cpol10.aspx
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////


namespace PK.TestTaskDialog
{
    partial class TestForm
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
            this.buttonFullExample = new System.Windows.Forms.Button();
            this.lbResult = new System.Windows.Forms.Label();
            this.buttonMessageBoxExample = new System.Windows.Forms.Button();
            this.buttonSimpleMessageBoxExample = new System.Windows.Forms.Button();
            this.buttonRadioBoxExample = new System.Windows.Forms.Button();
            this.buttonCommandBoxExample = new System.Windows.Forms.Button();
            this.checkBoxForceEmulation = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.edWidth = new System.Windows.Forms.TextBox();
            this.btAsterisk = new System.Windows.Forms.Button();
            this.btQuestion = new System.Windows.Forms.Button();
            this.btHand = new System.Windows.Forms.Button();
            this.btExclamation = new System.Windows.Forms.Button();
            this.btBeep = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbResult
            // 
            this.lbResult.Location = new System.Drawing.Point(224, 12);
            this.lbResult.Name = "lbResult";
            this.lbResult.Size = new System.Drawing.Size(158, 61);
            this.lbResult.TabIndex = 1;
            this.lbResult.Text = "lbResult";
            // 
            // buttonFullExample
            // 
            this.buttonFullExample.Location = new System.Drawing.Point(12, 12);
            this.buttonFullExample.Name = "buttonFullExample";
            this.buttonFullExample.Size = new System.Drawing.Size(158, 45);
            this.buttonFullExample.TabIndex = 0;
            this.buttonFullExample.Text = "Full Example";
            this.buttonFullExample.UseVisualStyleBackColor = true;
            this.buttonFullExample.Click += new System.EventHandler(this.OnButtonFullExample_Click);
            // 
            // buttonMessageBoxExample
            // 
            this.buttonMessageBoxExample.Location = new System.Drawing.Point(12, 63);
            this.buttonMessageBoxExample.Name = "buttonMessageBoxExample";
            this.buttonMessageBoxExample.Size = new System.Drawing.Size(158, 45);
            this.buttonMessageBoxExample.TabIndex = 1;
            this.buttonMessageBoxExample.Text = "MessageBox Example";
            this.buttonMessageBoxExample.UseVisualStyleBackColor = true;
            this.buttonMessageBoxExample.Click += new System.EventHandler(this.OnButtonMessageBoxExample_Click);
            // 
            // buttonSimpleMessageBoxExample
            // 
            this.buttonSimpleMessageBoxExample.Location = new System.Drawing.Point(12, 114);
            this.buttonSimpleMessageBoxExample.Name = "buttonSimpleMessageBoxExample";
            this.buttonSimpleMessageBoxExample.Size = new System.Drawing.Size(158, 45);
            this.buttonSimpleMessageBoxExample.TabIndex = 2;
            this.buttonSimpleMessageBoxExample.Text = "Simple MessageBox Example";
            this.buttonSimpleMessageBoxExample.UseVisualStyleBackColor = true;
            this.buttonSimpleMessageBoxExample.Click += new System.EventHandler(this.OnButtonSimpleMessageBoxExample_Click);
            // 
            // buttonRadioBoxExample
            // 
            this.buttonRadioBoxExample.Location = new System.Drawing.Point(12, 165);
            this.buttonRadioBoxExample.Name = "buttonRadioBoxExample";
            this.buttonRadioBoxExample.Size = new System.Drawing.Size(158, 45);
            this.buttonRadioBoxExample.TabIndex = 3;
            this.buttonRadioBoxExample.Text = "RadioBox Example";
            this.buttonRadioBoxExample.UseVisualStyleBackColor = true;
            this.buttonRadioBoxExample.Click += new System.EventHandler(this.OnButtonRadioBoxExample_Click);
            // 
            // buttonCommandBoxExample
            // 
            this.buttonCommandBoxExample.Location = new System.Drawing.Point(12, 216);
            this.buttonCommandBoxExample.Name = "buttonCommandBoxExample";
            this.buttonCommandBoxExample.Size = new System.Drawing.Size(158, 45);
            this.buttonCommandBoxExample.TabIndex = 4;
            this.buttonCommandBoxExample.Text = "CommandBox Example";
            this.buttonCommandBoxExample.UseVisualStyleBackColor = true;
            this.buttonCommandBoxExample.Click += new System.EventHandler(this.OnButtonCommandBoxExample_Click);
            // 
            // checkBoxForceEmulation
            // 
            this.checkBoxForceEmulation.AutoSize = true;
            this.checkBoxForceEmulation.Location = new System.Drawing.Point(12, 287);
            this.checkBoxForceEmulation.Name = "checkBoxForceEmulation";
            this.checkBoxForceEmulation.Size = new System.Drawing.Size(131, 17);
            this.checkBoxForceEmulation.TabIndex = 5;
            this.checkBoxForceEmulation.Text = "Force Emulation Mode";
            this.checkBoxForceEmulation.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(162, 288);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Emulated Form Width";
            // 
            // edWidth
            // 
            this.edWidth.Location = new System.Drawing.Point(277, 285);
            this.edWidth.Name = "edWidth";
            this.edWidth.Size = new System.Drawing.Size(74, 21);
            this.edWidth.TabIndex = 11;
            this.edWidth.Text = "450";
            // 
            // btAsterisk
            // 
            this.btAsterisk.Location = new System.Drawing.Point(276, 125);
            this.btAsterisk.Name = "btAsterisk";
            this.btAsterisk.Size = new System.Drawing.Size(75, 23);
            this.btAsterisk.TabIndex = 6;
            this.btAsterisk.Text = "Asterisk";
            this.btAsterisk.UseVisualStyleBackColor = true;
            this.btAsterisk.Click += new System.EventHandler(this.OnBtAsterisk_Click);
            // 
            // btQuestion
            // 
            this.btQuestion.Location = new System.Drawing.Point(276, 238);
            this.btQuestion.Name = "btQuestion";
            this.btQuestion.Size = new System.Drawing.Size(75, 23);
            this.btQuestion.TabIndex = 10;
            this.btQuestion.Text = "Question";
            this.btQuestion.UseVisualStyleBackColor = true;
            this.btQuestion.Click += new System.EventHandler(this.OnBtQuestion_Click);
            // 
            // btHand
            // 
            this.btHand.Location = new System.Drawing.Point(276, 212);
            this.btHand.Name = "btHand";
            this.btHand.Size = new System.Drawing.Size(75, 23);
            this.btHand.TabIndex = 9;
            this.btHand.Text = "Hand";
            this.btHand.UseVisualStyleBackColor = true;
            this.btHand.Click += new System.EventHandler(this.OnBtHand_Click);
            // 
            // btExclamation
            // 
            this.btExclamation.Location = new System.Drawing.Point(276, 183);
            this.btExclamation.Name = "btExclamation";
            this.btExclamation.Size = new System.Drawing.Size(75, 23);
            this.btExclamation.TabIndex = 8;
            this.btExclamation.Text = "Exclamation";
            this.btExclamation.UseVisualStyleBackColor = true;
            this.btExclamation.Click += new System.EventHandler(this.OnBtExclamation_Click);
            // 
            // btBeep
            // 
            this.btBeep.Location = new System.Drawing.Point(276, 154);
            this.btBeep.Name = "btBeep";
            this.btBeep.Size = new System.Drawing.Size(75, 23);
            this.btBeep.TabIndex = 7;
            this.btBeep.Text = "Beep";
            this.btBeep.UseVisualStyleBackColor = true;
            this.btBeep.Click += new System.EventHandler(this.OnBtBeep_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(274, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "System Sounds";
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 316);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btBeep);
            this.Controls.Add(this.btExclamation);
            this.Controls.Add(this.btHand);
            this.Controls.Add(this.btQuestion);
            this.Controls.Add(this.btAsterisk);
            this.Controls.Add(this.edWidth);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxForceEmulation);
            this.Controls.Add(this.buttonCommandBoxExample);
            this.Controls.Add(this.buttonRadioBoxExample);
            this.Controls.Add(this.buttonSimpleMessageBoxExample);
            this.Controls.Add(this.buttonMessageBoxExample);
            this.Controls.Add(this.lbResult);
            this.Controls.Add(this.buttonFullExample);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test TaskDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonFullExample;
        private System.Windows.Forms.Button buttonMessageBoxExample;
        private System.Windows.Forms.Button buttonSimpleMessageBoxExample;
        private System.Windows.Forms.Button buttonRadioBoxExample;
        private System.Windows.Forms.Button buttonCommandBoxExample;
        private System.Windows.Forms.CheckBox checkBoxForceEmulation;
        private System.Windows.Forms.Label lbResult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox edWidth;
        private System.Windows.Forms.Button btAsterisk;
        private System.Windows.Forms.Button btQuestion;
        private System.Windows.Forms.Button btHand;
        private System.Windows.Forms.Button btExclamation;
        private System.Windows.Forms.Button btBeep;
        private System.Windows.Forms.Label label2;

    }
}


namespace PK.TestErrorDisplay
{
  partial class MainForm
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
      this._btnTestClassA = new System.Windows.Forms.Button();
      this._btnTestClassB = new System.Windows.Forms.Button();
      this._btnTestClassC = new System.Windows.Forms.Button();
      this._chkBxTestCastToInterface = new System.Windows.Forms.CheckBox();
      this.labelA = new System.Windows.Forms.Label();
      this.labelB = new System.Windows.Forms.Label();
      this.labelC = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // _btnTestClassA
      // 
      this._btnTestClassA.Location = new System.Drawing.Point(24, 58);
      this._btnTestClassA.Name = "_btnTestClassA";
      this._btnTestClassA.Size = new System.Drawing.Size(80, 23);
      this._btnTestClassA.TabIndex = 0;
      this._btnTestClassA.Text = "Test DisplayA";
      this._btnTestClassA.UseVisualStyleBackColor = true;
      this._btnTestClassA.Click += new System.EventHandler(this._btnTestClassA_Click);
      // 
      // _btnTestClassB
      // 
      this._btnTestClassB.Location = new System.Drawing.Point(24, 96);
      this._btnTestClassB.Name = "_btnTestClassB";
      this._btnTestClassB.Size = new System.Drawing.Size(80, 23);
      this._btnTestClassB.TabIndex = 1;
      this._btnTestClassB.Text = "Test DisplayB";
      this._btnTestClassB.UseVisualStyleBackColor = true;
      this._btnTestClassB.Click += new System.EventHandler(this._btnTestClassB_Click);
      // 
      // _btnTestClassC
      // 
      this._btnTestClassC.Location = new System.Drawing.Point(24, 134);
      this._btnTestClassC.Name = "_btnTestClassC";
      this._btnTestClassC.Size = new System.Drawing.Size(80, 23);
      this._btnTestClassC.TabIndex = 2;
      this._btnTestClassC.Text = "Test DisplayC";
      this._btnTestClassC.UseVisualStyleBackColor = true;
      this._btnTestClassC.Click += new System.EventHandler(this._btnTestClassC_Click);
      // 
      // _chkBxTestCastToInterface
      // 
      this._chkBxTestCastToInterface.AutoSize = true;
      this._chkBxTestCastToInterface.Location = new System.Drawing.Point(24, 18);
      this._chkBxTestCastToInterface.Name = "_chkBxTestCastToInterface";
      this._chkBxTestCastToInterface.Size = new System.Drawing.Size(159, 17);
      this._chkBxTestCastToInterface.TabIndex = 3;
      this._chkBxTestCastToInterface.Text = "Test calling cast to interface";
      this._chkBxTestCastToInterface.UseVisualStyleBackColor = true;
      // 
      // labelA
      // 
      this.labelA.AutoSize = true;
      this.labelA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelA.Location = new System.Drawing.Point(111, 63);
      this.labelA.Name = "labelA";
      this.labelA.Size = new System.Drawing.Size(447, 13);
      this.labelA.TabIndex = 4;
      this.labelA.Text = "DisplayA implements IErrorPresenter with the same implicit arguments as in interf" +
    "ace definition.";
      // 
      // labelB
      // 
      this.labelB.AutoSize = true;
      this.labelB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelB.Location = new System.Drawing.Point(111, 101);
      this.labelB.Name = "labelB";
      this.labelB.Size = new System.Drawing.Size(328, 13);
      this.labelB.TabIndex = 5;
      this.labelB.Text = "DisplayB implements IErrorPresenter with different implicit arguments.";
      // 
      // labelC
      // 
      this.labelC.AutoSize = true;
      this.labelC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelC.Location = new System.Drawing.Point(111, 139);
      this.labelC.Name = "labelC";
      this.labelC.Size = new System.Drawing.Size(355, 13);
      this.labelC.TabIndex = 6;
      this.labelC.Text = "DisplayC implements IErrorPresenter completely without implicit arguments.";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(567, 183);
      this.Controls.Add(this.labelC);
      this.Controls.Add(this.labelB);
      this.Controls.Add(this.labelA);
      this.Controls.Add(this._chkBxTestCastToInterface);
      this.Controls.Add(this._btnTestClassC);
      this.Controls.Add(this._btnTestClassB);
      this.Controls.Add(this._btnTestClassA);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MainForm";
      this.Text = "Test ErrorDisplay";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button _btnTestClassA;
    private System.Windows.Forms.Button _btnTestClassB;
    private System.Windows.Forms.Button _btnTestClassC;
    private System.Windows.Forms.CheckBox _chkBxTestCastToInterface;
    private System.Windows.Forms.Label labelA;
    private System.Windows.Forms.Label labelB;
    private System.Windows.Forms.Label labelC;
  }
}


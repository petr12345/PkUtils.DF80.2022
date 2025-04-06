using System.Windows.Forms;

namespace WinTester3
{
  public partial class frmMain
  {
    #region Windows Form Designer generated code
    private System.Windows.Forms.GroupBox grpEdge;
    private System.Windows.Forms.RadioButton rdoFloat;
    private System.Windows.Forms.RadioButton rdoRight;
    private System.Windows.Forms.RadioButton rdoLeft;
    private System.Windows.Forms.RadioButton rdoBottom;
    private System.Windows.Forms.RadioButton rdoTop;
    private System.ComponentModel.Container components = null;

    private void InitializeComponent()
    {
      this.grpEdge = new System.Windows.Forms.GroupBox();
      this._checkBxOnTop = new System.Windows.Forms.CheckBox();
      this.rdoFloat = new System.Windows.Forms.RadioButton();
      this.rdoRight = new System.Windows.Forms.RadioButton();
      this.rdoLeft = new System.Windows.Forms.RadioButton();
      this.rdoBottom = new System.Windows.Forms.RadioButton();
      this.rdoTop = new System.Windows.Forms.RadioButton();
      this.grpEdge.SuspendLayout();
      this.SuspendLayout();
      // 
      // grpEdge
      // 
      this.grpEdge.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.grpEdge.Controls.Add(this._checkBxOnTop);
      this.grpEdge.Location = new System.Drawing.Point(8, 8);
      this.grpEdge.Name = "grpEdge";
      this.grpEdge.Size = new System.Drawing.Size(72, 134);
      this.grpEdge.TabIndex = 3;
      this.grpEdge.TabStop = false;
      this.grpEdge.Text = "Edge";
      // 
      // _checkBxOnTop
      // 
      this._checkBxOnTop.AutoSize = true;
      this._checkBxOnTop.Location = new System.Drawing.Point(5, 112);
      this._checkBxOnTop.Name = "_checkBxOnTop";
      this._checkBxOnTop.Size = new System.Drawing.Size(67, 17);
      this._checkBxOnTop.TabIndex = 0;
      this._checkBxOnTop.Text = "Topmost";
      this._checkBxOnTop.UseVisualStyleBackColor = true;
      this._checkBxOnTop.CheckedChanged += new System.EventHandler(this._checkBxOnTop_CheckedChanged);
      // 
      // rdoFloat
      // 
      this.rdoFloat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.rdoFloat.Location = new System.Drawing.Point(12, 88);
      this.rdoFloat.Name = "rdoFloat";
      this.rdoFloat.Size = new System.Drawing.Size(64, 16);
      this.rdoFloat.TabIndex = 9;
      this.rdoFloat.Text = "Float";
      this.rdoFloat.CheckedChanged += new System.EventHandler(this.rdo_CheckedChanged);
      // 
      // rdoRight
      // 
      this.rdoRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.rdoRight.Location = new System.Drawing.Point(12, 72);
      this.rdoRight.Name = "rdoRight";
      this.rdoRight.Size = new System.Drawing.Size(64, 16);
      this.rdoRight.TabIndex = 8;
      this.rdoRight.Text = "Right";
      this.rdoRight.CheckedChanged += new System.EventHandler(this.rdo_CheckedChanged);
      // 
      // rdoLeft
      // 
      this.rdoLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.rdoLeft.Location = new System.Drawing.Point(12, 56);
      this.rdoLeft.Name = "rdoLeft";
      this.rdoLeft.Size = new System.Drawing.Size(64, 16);
      this.rdoLeft.TabIndex = 7;
      this.rdoLeft.Text = "Left";
      this.rdoLeft.CheckedChanged += new System.EventHandler(this.rdo_CheckedChanged);
      // 
      // rdoBottom
      // 
      this.rdoBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.rdoBottom.Location = new System.Drawing.Point(12, 40);
      this.rdoBottom.Name = "rdoBottom";
      this.rdoBottom.Size = new System.Drawing.Size(64, 16);
      this.rdoBottom.TabIndex = 6;
      this.rdoBottom.Text = "Bottom";
      this.rdoBottom.CheckedChanged += new System.EventHandler(this.rdo_CheckedChanged);
      // 
      // rdoTop
      // 
      this.rdoTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.rdoTop.Location = new System.Drawing.Point(12, 24);
      this.rdoTop.Name = "rdoTop";
      this.rdoTop.Size = new System.Drawing.Size(64, 16);
      this.rdoTop.TabIndex = 5;
      this.rdoTop.Text = "Top";
      this.rdoTop.CheckedChanged += new System.EventHandler(this.rdo_CheckedChanged);
      // 
      // frmMain
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(88, 150);
      this.Controls.Add(this.rdoFloat);
      this.Controls.Add(this.rdoRight);
      this.Controls.Add(this.rdoLeft);
      this.Controls.Add(this.rdoBottom);
      this.Controls.Add(this.rdoTop);
      this.Controls.Add(this.grpEdge);
      this.Name = "frmMain";
      this.Text = "WinTester for Part 3";
      this.Load += new System.EventHandler(this.frmMain_Load);
      this.grpEdge.ResumeLayout(false);
      this.grpEdge.PerformLayout();
      this.ResumeLayout(false);

    }
    #endregion

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    private CheckBox _checkBxOnTop;
  }
}

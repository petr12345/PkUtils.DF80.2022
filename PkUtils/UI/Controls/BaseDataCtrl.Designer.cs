// Ignore Spelling: Utils
// 

using System.Windows.Forms;
namespace PK.PkUtils.UI.Controls;

partial class BaseDataCtrl<D> : UserControl where D : class
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this._lblTypeText = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // _lblTypeText
        // 
        this._lblTypeText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this._lblTypeText.AutoSize = true;
        this._lblTypeText.BackColor = System.Drawing.Color.Khaki;
        this._lblTypeText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._lblTypeText.Location = new System.Drawing.Point(421, 0);
        this._lblTypeText.Name = "_lblTypeText";
        this._lblTypeText.Size = new System.Drawing.Size(103, 15);
        this._lblTypeText.TabIndex = 1;
        this._lblTypeText.Text = "UC_BaseDataCtrl";
        this._lblTypeText.Visible = false;
        // 
        // BaseDataCtrl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this._lblTypeText);
        this.Name = "BaseDataCtrl";
        this.Size = new System.Drawing.Size(562, 281);
        this.Load += new System.EventHandler(this.BaseDataCtrl_Load);
        this.VisibleChanged += new System.EventHandler(this.UC_BaseCtrl_VisibleChanged);
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    /// <summary> The label type text. </summary>
    protected System.Windows.Forms.Label _lblTypeText;
}

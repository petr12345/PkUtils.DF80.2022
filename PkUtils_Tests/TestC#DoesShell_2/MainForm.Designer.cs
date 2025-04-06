
namespace WinTester2
{
	public partial class MainForm
	{
		#region Windows Form Designer generated code
		private System.Windows.Forms.GroupBox grpLaunchApp;
private System.Windows.Forms.Button btnExploreFolder;
private System.Windows.Forms.Button btnFindInFolder;
private System.Windows.Forms.Button btnEditBmp;
private System.Windows.Forms.Button btnShowBmp;
private System.Windows.Forms.Button btnOpenExe;
private System.Windows.Forms.GroupBox grpFileOp;
private System.Windows.Forms.Button btnCopy;
private System.Windows.Forms.Button btnMove;
private System.Windows.Forms.Button btnDelete;
private System.Windows.Forms.Button btnClearList;
private System.Windows.Forms.Button btnAddFile;
private System.Windows.Forms.GroupBox grpRecentDocs;
private System.Windows.Forms.GroupBox grpPrinter;
private System.Windows.Forms.Button btnOpenPrinter;
private System.Windows.Forms.Button btnShowProperties;
private System.Windows.Forms.Button btnTestPage;
private System.ComponentModel.Container components = null;

		private void InitializeComponent()
		{
      this.grpLaunchApp = new System.Windows.Forms.GroupBox();
      this.btnExploreFolder = new System.Windows.Forms.Button();
      this.btnFindInFolder = new System.Windows.Forms.Button();
      this.btnEditBmp = new System.Windows.Forms.Button();
      this.btnShowBmp = new System.Windows.Forms.Button();
      this.btnOpenExe = new System.Windows.Forms.Button();
      this.grpFileOp = new System.Windows.Forms.GroupBox();
      this.btnDelete = new System.Windows.Forms.Button();
      this.btnMove = new System.Windows.Forms.Button();
      this.btnCopy = new System.Windows.Forms.Button();
      this.grpRecentDocs = new System.Windows.Forms.GroupBox();
      this.btnClearList = new System.Windows.Forms.Button();
      this.btnAddFile = new System.Windows.Forms.Button();
      this.grpPrinter = new System.Windows.Forms.GroupBox();
      this.btnShowProperties = new System.Windows.Forms.Button();
      this.btnOpenPrinter = new System.Windows.Forms.Button();
      this.btnTestPage = new System.Windows.Forms.Button();
      this.grpLaunchApp.SuspendLayout();
      this.grpFileOp.SuspendLayout();
      this.grpRecentDocs.SuspendLayout();
      this.grpPrinter.SuspendLayout();
      this.SuspendLayout();
      // 
      // grpLaunchApp
      // 
      this.grpLaunchApp.Controls.Add(this.btnExploreFolder);
      this.grpLaunchApp.Controls.Add(this.btnFindInFolder);
      this.grpLaunchApp.Controls.Add(this.btnEditBmp);
      this.grpLaunchApp.Controls.Add(this.btnShowBmp);
      this.grpLaunchApp.Controls.Add(this.btnOpenExe);
      this.grpLaunchApp.Location = new System.Drawing.Point(8, 8);
      this.grpLaunchApp.Name = "grpLaunchApp";
      this.grpLaunchApp.Size = new System.Drawing.Size(128, 192);
      this.grpLaunchApp.TabIndex = 8;
      this.grpLaunchApp.TabStop = false;
      this.grpLaunchApp.Text = "Launch Applications:";
      // 
      // btnExploreFolder
      // 
      this.btnExploreFolder.Location = new System.Drawing.Point(16, 152);
      this.btnExploreFolder.Name = "btnExploreFolder";
      this.btnExploreFolder.Size = new System.Drawing.Size(96, 24);
      this.btnExploreFolder.TabIndex = 11;
      this.btnExploreFolder.Text = "Explore Folder";
      this.btnExploreFolder.Click += new System.EventHandler(this.btnExploreFolder_Click);
      // 
      // btnFindInFolder
      // 
      this.btnFindInFolder.Location = new System.Drawing.Point(16, 120);
      this.btnFindInFolder.Name = "btnFindInFolder";
      this.btnFindInFolder.Size = new System.Drawing.Size(96, 24);
      this.btnFindInFolder.TabIndex = 10;
      this.btnFindInFolder.Text = "Find in Folder";
      this.btnFindInFolder.Click += new System.EventHandler(this.btnFindInFolder_Click);
      // 
      // btnEditBmp
      // 
      this.btnEditBmp.Location = new System.Drawing.Point(16, 88);
      this.btnEditBmp.Name = "btnEditBmp";
      this.btnEditBmp.Size = new System.Drawing.Size(96, 24);
      this.btnEditBmp.TabIndex = 9;
      this.btnEditBmp.Text = "Edit BMP";
      this.btnEditBmp.Click += new System.EventHandler(this.btnEditBmp_Click);
      // 
      // btnShowBmp
      // 
      this.btnShowBmp.Location = new System.Drawing.Point(16, 56);
      this.btnShowBmp.Name = "btnShowBmp";
      this.btnShowBmp.Size = new System.Drawing.Size(96, 24);
      this.btnShowBmp.TabIndex = 8;
      this.btnShowBmp.Text = "Show BMP";
      this.btnShowBmp.Click += new System.EventHandler(this.btnShowBmp_Click);
      // 
      // btnOpenExe
      // 
      this.btnOpenExe.Location = new System.Drawing.Point(16, 24);
      this.btnOpenExe.Name = "btnOpenExe";
      this.btnOpenExe.Size = new System.Drawing.Size(96, 24);
      this.btnOpenExe.TabIndex = 7;
      this.btnOpenExe.Text = "Open an EXE";
      this.btnOpenExe.Click += new System.EventHandler(this.btnOpenExe_Click);
      // 
      // grpFileOp
      // 
      this.grpFileOp.Controls.Add(this.btnDelete);
      this.grpFileOp.Controls.Add(this.btnMove);
      this.grpFileOp.Controls.Add(this.btnCopy);
      this.grpFileOp.Location = new System.Drawing.Point(144, 8);
      this.grpFileOp.Name = "grpFileOp";
      this.grpFileOp.Size = new System.Drawing.Size(128, 128);
      this.grpFileOp.TabIndex = 9;
      this.grpFileOp.TabStop = false;
      this.grpFileOp.Text = "File Operations";
      // 
      // btnDelete
      // 
      this.btnDelete.Location = new System.Drawing.Point(16, 88);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new System.Drawing.Size(96, 24);
      this.btnDelete.TabIndex = 2;
      this.btnDelete.Text = "Delete Files";
      this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
      // 
      // btnMove
      // 
      this.btnMove.Location = new System.Drawing.Point(16, 56);
      this.btnMove.Name = "btnMove";
      this.btnMove.Size = new System.Drawing.Size(96, 24);
      this.btnMove.TabIndex = 1;
      this.btnMove.Text = "Move Files";
      this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
      // 
      // btnCopy
      // 
      this.btnCopy.Location = new System.Drawing.Point(16, 24);
      this.btnCopy.Name = "btnCopy";
      this.btnCopy.Size = new System.Drawing.Size(96, 24);
      this.btnCopy.TabIndex = 0;
      this.btnCopy.Text = "Copy Files";
      this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
      // 
      // grpRecentDocs
      // 
      this.grpRecentDocs.Controls.Add(this.btnClearList);
      this.grpRecentDocs.Controls.Add(this.btnAddFile);
      this.grpRecentDocs.Location = new System.Drawing.Point(144, 144);
      this.grpRecentDocs.Name = "grpRecentDocs";
      this.grpRecentDocs.Size = new System.Drawing.Size(128, 96);
      this.grpRecentDocs.TabIndex = 10;
      this.grpRecentDocs.TabStop = false;
      this.grpRecentDocs.Text = "Recent Documents";
      // 
      // btnClearList
      // 
      this.btnClearList.Location = new System.Drawing.Point(16, 56);
      this.btnClearList.Name = "btnClearList";
      this.btnClearList.Size = new System.Drawing.Size(96, 23);
      this.btnClearList.TabIndex = 1;
      this.btnClearList.Text = "Clear List";
      this.btnClearList.Click += new System.EventHandler(this.btnClearList_Click);
      // 
      // btnAddFile
      // 
      this.btnAddFile.Location = new System.Drawing.Point(16, 24);
      this.btnAddFile.Name = "btnAddFile";
      this.btnAddFile.Size = new System.Drawing.Size(96, 23);
      this.btnAddFile.TabIndex = 0;
      this.btnAddFile.Text = "Add file to List";
      this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click);
      // 
      // grpPrinter
      // 
      this.grpPrinter.Controls.Add(this.btnShowProperties);
      this.grpPrinter.Controls.Add(this.btnOpenPrinter);
      this.grpPrinter.Controls.Add(this.btnTestPage);
      this.grpPrinter.Location = new System.Drawing.Point(280, 8);
      this.grpPrinter.Name = "grpPrinter";
      this.grpPrinter.Size = new System.Drawing.Size(128, 128);
      this.grpPrinter.TabIndex = 11;
      this.grpPrinter.TabStop = false;
      this.grpPrinter.Text = "Printer Managment";
      // 
      // btnShowProperties
      // 
      this.btnShowProperties.Location = new System.Drawing.Point(16, 56);
      this.btnShowProperties.Name = "btnShowProperties";
      this.btnShowProperties.Size = new System.Drawing.Size(96, 24);
      this.btnShowProperties.TabIndex = 1;
      this.btnShowProperties.Text = "Show Properties";
      this.btnShowProperties.Click += new System.EventHandler(this.btnShowProperties_Click);
      // 
      // btnOpenPrinter
      // 
      this.btnOpenPrinter.Location = new System.Drawing.Point(16, 24);
      this.btnOpenPrinter.Name = "btnOpenPrinter";
      this.btnOpenPrinter.Size = new System.Drawing.Size(96, 24);
      this.btnOpenPrinter.TabIndex = 0;
      this.btnOpenPrinter.Text = "Open Printer";
      this.btnOpenPrinter.Click += new System.EventHandler(this.btnOpenPrinter_Click);
      // 
      // btnTestPage
      // 
      this.btnTestPage.Location = new System.Drawing.Point(16, 88);
      this.btnTestPage.Name = "btnTestPage";
      this.btnTestPage.Size = new System.Drawing.Size(96, 24);
      this.btnTestPage.TabIndex = 3;
      this.btnTestPage.Text = "Print Test Page";
      this.btnTestPage.Click += new System.EventHandler(this.btnTestPage_Click);
      // 
      // FrmMain
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(416, 245);
      this.Controls.Add(this.grpPrinter);
      this.Controls.Add(this.grpRecentDocs);
      this.Controls.Add(this.grpFileOp);
      this.Controls.Add(this.grpLaunchApp);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(432, 280);
      this.Name = "FrmMain";
      this.Text = "WinTester for Part 2";
      this.grpLaunchApp.ResumeLayout(false);
      this.grpFileOp.ResumeLayout(false);
      this.grpRecentDocs.ResumeLayout(false);
      this.grpPrinter.ResumeLayout(false);
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
	}
}

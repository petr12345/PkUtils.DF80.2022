using System;
using System.Windows.Forms;

namespace PK.TestSingletonGeneric
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            Highlander.Instance.Fight();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

using System;
using System.Windows.Forms;


namespace PK.TestErrorDisplay
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void _btnTestClassA_Click(object sender, EventArgs e)
        {
            DisplayA presA = new();
            IErrorDisplay ipresA = presA;
            bool castToInterface = _chkBxTestCastToInterface.Checked;

            if (!castToInterface)
            {
                // Following uses default argument values as they are in interface definition
                presA.ShowError("Calling Instance of PresenterA", "Error");
            }
            else
            {
                ipresA.ShowError("Calling IErrorPresenter of PresenterA", "Error");
            }
        }

        private void _btnTestClassB_Click(object sender, EventArgs e)
        {
            DisplayB presB = new();
            IErrorDisplay ipresB = presB;
            bool castToInterface = _chkBxTestCastToInterface.Checked;

            if (!castToInterface)
            {
                // uses default argument values as they are in THE BLASS definition
                presB.ShowError("Calling Instance of PresenterB. \nNotice the difference in dialog buttons with/without casting to interface.", "Error");
            }
            else
            {
                ipresB.ShowError("Calling IErrorPresenter of PresenterB. \nNotice the difference in dialog buttons with/without casting to interface.", "Error");
            }
        }

        private void _btnTestClassC_Click(object sender, EventArgs e)
        {
            DisplayC presC = new();
            IErrorDisplay ipresC = presC;
            bool castToInterface = _chkBxTestCastToInterface.Checked;

            if (!castToInterface)
            {
                // Following cannot be compiled -
                // Error1	No overload for method 'ShowError' takes 2 arguments	e:\PK.Projects\TestInterfaces\Form1.cs	31	7	TestInterfaces.DF40.VS0
                //
                /* presC.ShowError("Calling Instance of PresenterC", "Error"); */
                presC.ShowError("Calling Instance of PresenterC", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                ipresC.ShowError("Calling IErrorPresenter of PresenterC", "Error");
            }
        }
    }
}

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

using System;
using System.Text;
using System.Windows.Forms;
using PK.PkUtils.UI.Dialogs.PSTaskDialog;

namespace TestTaskDialog
{
    public partial class TestForm : Form
    {
        #region Constructor(s)

        public TestForm()
        {
            InitializeComponent();
            this.Icon = PK.TestTaskDialog.Properties.Resources.App;
        }
        #endregion // Constructor(s)

        #region Methods

        private void UpdateResult(DialogResult res)
        {
            StringBuilder sb = new();

            sb.Append("Result : " + Enum.GetName(typeof(DialogResult), res) + Environment.NewLine);
            sb.Append("RadioButtonIndex : " + cTaskDialog.RadioButtonResult.ToString() + Environment.NewLine);
            sb.Append("CommandButtonIndex : " + cTaskDialog.CommandButtonResult.ToString() + Environment.NewLine);
            sb.Append("Verify CheckBox : " + (cTaskDialog.VerificationChecked ? "true" : "false"));

            lbResult.Text = sb.ToString();
        }
        #endregion // Methods

        #region Event handlers

        private void button1_Click(object sender, EventArgs e)
        {
            cTaskDialog.ForceEmulationMode = checkBox1.Checked;
            try { cTaskDialog.EmulatedFormWidth = Convert.ToInt32(edWidth.Text); }
            catch (Exception) { cTaskDialog.EmulatedFormWidth = 450; }

            DialogResult res = cTaskDialog.ShowTaskDialogBox(
              this,
             "Task Dialog Title",
             "The main instruction text for the TaskDialog goes here.",
             "The content text for the task dialog is shown here and the text will automatically wrap as needed.",
             "Any expanded content text for the task dialog is shown here and the text will automatically wrap as needed.",
             "Optional footer text with an icon can be included",
             "Don't show me this message again",
             "Radio Option 1|Radio Option 2|Radio Option 3",
             "Command &Button 1|Command Button 2\nLine 2\nLine 3|Command Button 3",
             eTaskDialogButtons.OKCancel,
             eSysIcons.Information,
             eSysIcons.Warning);
            UpdateResult(res);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cTaskDialog.ForceEmulationMode = checkBox1.Checked;
            try { cTaskDialog.EmulatedFormWidth = Convert.ToInt32(edWidth.Text); }
            catch (Exception) { cTaskDialog.EmulatedFormWidth = 450; }

            DialogResult res = cTaskDialog.MessageBox(
              this,
              "MessageBox Title",
              "The main instruction text for the message box is shown here.",
              "The content text for the message box is shown here and the text will automatically wrap as needed.",
              "Any expanded content text for the message box is shown here and the text will automatically wrap as needed.",
              "Optional footer text with an icon can be included",
              "ARRGHH! Don't show me this again!!!!",
              eTaskDialogButtons.YesNoCancel,
              eSysIcons.Information,
              eSysIcons.Error);

            /*
            DialogResult res = PSTaskDialog.cTaskDialog.MessageBox(
              this,
              "OnRAMP Design Suite",
              "The model is not configured properly.",
              "The local structures have not been successfully updated with the OnRAMP Model.",
              "Any expanded content text for the message box is shown here and the text will automatically wrap as needed.",
              null,
              "ARRGHH! Don't show me this again!!!!",
              PSTaskDialog.eTaskDialogButtons.OK,
              PSTaskDialog.eSysIcons.Warning,
              PSTaskDialog.eSysIcons.Error);
            */

            UpdateResult(res);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cTaskDialog.ForceEmulationMode = checkBox1.Checked;
            try { cTaskDialog.EmulatedFormWidth = Convert.ToInt32(edWidth.Text); }
            catch (Exception) { cTaskDialog.EmulatedFormWidth = 450; }

            DialogResult res = cTaskDialog.MessageBox(
              this,
              "MessageBox Title",
              "The main instruction text for the message box is shown here.",
              "The content text for the message box is shown here and the text will automatically wrap as needed.",
              eTaskDialogButtons.OK,
              eSysIcons.Warning);
            UpdateResult(res);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cTaskDialog.ForceEmulationMode = checkBox1.Checked;
            try { cTaskDialog.EmulatedFormWidth = Convert.ToInt32(edWidth.Text); }
            catch (Exception) { cTaskDialog.EmulatedFormWidth = 450; }

            int idx = cTaskDialog.ShowRadioBox(
              this,
              "RadioBox Title",
              "The main instruction text for the radiobox is shown here.",
              "The content text for the radiobox is shown here and the text will automatically wrap as needed.",
              "Any expanded content text for the radiobox is shown here and the text will automatically wrap as needed.",
              "Optional footer text with an icon can be included",
              "Don't show this message again",
              "Radio Option 1|Radio Option 2|Radio Option 3|Radio Option 4|Radio Option 5",
              eSysIcons.Information,
              eSysIcons.Warning);

            lbResult.Text = "ShowRadioBox return value : " + idx.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            cTaskDialog.ForceEmulationMode = checkBox1.Checked;
            try { cTaskDialog.EmulatedFormWidth = Convert.ToInt32(edWidth.Text); }
            catch (Exception) { cTaskDialog.EmulatedFormWidth = 450; }

            int idx = cTaskDialog.ShowCommandBox(
              this,
              "CommandBox Title",
              "The main instruction text for the commandbox is shown here.",
              "The content text for the commandbox is shown here and the text will automatically wrap as needed.",
              "Any expanded content text for the commandbox is shown here and the text will automatically wrap as needed.",
              "Optional footer text with an icon can be included",
              "Don't show this message again",
              "Command Button 1|Command Button 2|Command Button 3|Command Button 4",
              true,
              eSysIcons.Information,
              eSysIcons.Warning);

            lbResult.Text = "ShowCommandBox return value : " + idx.ToString();
        }

        private void btAsterisk_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void btQuestion_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Question.Play();
        }

        private void btHand_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Hand.Play();
        }

        private void btExclamation_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Exclamation.Play();
        }

        private void btBeep_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Beep.Play();
        }
        #endregion // Event handlers
    }
}
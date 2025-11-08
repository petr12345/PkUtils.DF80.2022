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
using PK.TestTaskDialog.Properties;

namespace PK.TestTaskDialog
{
    public partial class TestForm : Form
    {
        #region Constructor(s)

        public TestForm()
        {
            InitializeComponent();
            this.Icon = Resources.App;
        }
        #endregion // Constructor(s)

        #region Methods

        private void UpdateResult(DialogResult res)
        {
            StringBuilder sb = new();

            sb.Append("Result : " + Enum.GetName(typeof(DialogResult), res) + Environment.NewLine);
            sb.Append("RadioButtonIndex : " + VistaTaskDialogManager.RadioButtonResult.ToString() + Environment.NewLine);
            sb.Append("CommandButtonIndex : " + VistaTaskDialogManager.CommandButtonResult.ToString() + Environment.NewLine);
            sb.Append("Verify CheckBox : " + (VistaTaskDialogManager.VerificationChecked ? "true" : "false"));

            lbResult.Text = sb.ToString();
        }
        #endregion // Methods

        #region Event handlers

        private void OnButtonFullExample_Click(object sender, EventArgs e)
        {
            VistaTaskDialogManager.ForceEmulationMode = checkBoxForceEmulation.Checked;
            try { VistaTaskDialogManager.EmulatedFormWidth = Convert.ToInt32(edWidth.Text); }
            catch (Exception) { VistaTaskDialogManager.EmulatedFormWidth = 450; }

            DialogResult res = VistaTaskDialogManager.ShowTaskDialogBox(
              this,
             "Task Dialog Title",
             "The main instruction text for the TaskDialog goes here.",
             "The content text for the task dialog is shown here and the text will automatically wrap as needed.",
             "Any expanded content text for the task dialog is shown here and the text will automatically wrap as needed.",
             "Optional footer text with an icon can be included",
             "Don't show me this message again",
             "Radio Option 1|Radio Option 2|Radio Option 3",
             "Command &Button 1|Command Button 2\nLine 2\nLine 3|Command Button 3",
             TaskDialogButtons.OKCancel,
             SystemIconType.Information,
             SystemIconType.Warning);
            UpdateResult(res);
        }

        private void OnButtonMessageBoxExample_Click(object sender, EventArgs e)
        {
            VistaTaskDialogManager.ForceEmulationMode = checkBoxForceEmulation.Checked;
            try { VistaTaskDialogManager.EmulatedFormWidth = Convert.ToInt32(edWidth.Text); }
            catch (Exception) { VistaTaskDialogManager.EmulatedFormWidth = 450; }

            DialogResult res = VistaTaskDialogManager.MessageBox(
              this,
              "MessageBox Title",
              "The main instruction text for the message box is shown here.",
              "The content text for the message box is shown here and the text will automatically wrap as needed.",
              "Any expanded content text for the message box is shown here and the text will automatically wrap as needed.",
              "Optional footer text with an icon can be included",
              "ARRGHH! Don't show me this again!!!!",
              TaskDialogButtons.YesNoCancel,
              SystemIconType.Information,
              SystemIconType.Error);

            /*
            DialogResult res = PSTaskDialog.VistaTaskDialogManager.MessageBox(
              this,
              "XYZ Design Suite",
              "The model is not configured properly.",
              "The local structures have not been successfully updated with the XYZ Design Suite Model.",
              "Any expanded content text for the message box is shown here and the text will automatically wrap as needed.",
              null,
              "ARRGHH! Don't show me this again!!!!",
              PSTaskDialog.TaskDialogButtons.OK,
              PSTaskDialog.SystemIconType.Warning,
              PSTaskDialog.SystemIconType.Error);
            */

            UpdateResult(res);
        }

        private void OnButtonSimpleMessageBoxExample_Click(object sender, EventArgs e)
        {
            VistaTaskDialogManager.ForceEmulationMode = checkBoxForceEmulation.Checked;
            try { VistaTaskDialogManager.EmulatedFormWidth = Convert.ToInt32(edWidth.Text); }
            catch (Exception) { VistaTaskDialogManager.EmulatedFormWidth = 450; }

            DialogResult res = VistaTaskDialogManager.MessageBox(
              this,
              "MessageBox Title",
              "The main instruction text for the message box is shown here.",
              "The content text for the message box is shown here and the text will automatically wrap as needed.",
              TaskDialogButtons.OK,
              SystemIconType.Warning);
            UpdateResult(res);
        }

        private void OnButtonRadioBoxExample_Click(object sender, EventArgs e)
        {
            VistaTaskDialogManager.ForceEmulationMode = checkBoxForceEmulation.Checked;
            try { VistaTaskDialogManager.EmulatedFormWidth = Convert.ToInt32(edWidth.Text); }
            catch (Exception) { VistaTaskDialogManager.EmulatedFormWidth = 450; }

            int idx = VistaTaskDialogManager.ShowRadioBox(
              this,
              "RadioBox Title",
              "The main instruction text for the radiobox is shown here.",
              "The content text for the radiobox is shown here and the text will automatically wrap as needed.",
              "Any expanded content text for the radiobox is shown here and the text will automatically wrap as needed.",
              "Optional footer text with an icon can be included",
              "Don't show this message again",
              "Radio Option 1|Radio Option 2|Radio Option 3|Radio Option 4|Radio Option 5",
              SystemIconType.Information,
              SystemIconType.Warning);

            lbResult.Text = "ShowRadioBox return value : " + idx.ToString();
        }

        private void OnButtonCommandBoxExample_Click(object sender, EventArgs e)
        {
            VistaTaskDialogManager.ForceEmulationMode = checkBoxForceEmulation.Checked;
            try { VistaTaskDialogManager.EmulatedFormWidth = Convert.ToInt32(edWidth.Text); }
            catch (Exception) { VistaTaskDialogManager.EmulatedFormWidth = 450; }

            int idx = VistaTaskDialogManager.ShowCommandBox(
              this,
              "CommandBox Title",
              "The main instruction text for the commandbox is shown here.",
              "The content text for the commandbox is shown here and the text will automatically wrap as needed.",
              "Any expanded content text for the commandbox is shown here and the text will automatically wrap as needed.",
              "Optional footer text with an icon can be included",
              "Don't show this message again",
              "Command Button 1|Command Button 2|Command Button 3|Command Button 4",
              true,
              SystemIconType.Information,
              SystemIconType.Warning);

            lbResult.Text = "ShowCommandBox return value : " + idx.ToString();
        }

        private void OnBtAsterisk_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void OnBtQuestion_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Question.Play();
        }

        private void OnBtHand_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Hand.Play();
        }

        private void OnBtExclamation_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Exclamation.Play();
        }

        private void OnBtBeep_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Beep.Play();
        }
        #endregion // Event handlers
    }
}
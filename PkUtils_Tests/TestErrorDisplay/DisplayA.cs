using System;
using System.Windows.Forms;

namespace PK.TestErrorDisplay
{
    /// <summary>
    /// Implements IErrorPresenter with the same implicit arguments as in interface definition.
    /// </summary>
    internal class DisplayA : IErrorDisplay
    {
        public object ShowError(string text)
        {
            return MessageBox.Show(text);
        }

        public object ShowError(
          string text,
          string caption,
          MessageBoxButtons buttons = MessageBoxButtons.OK,
          MessageBoxIcon icon = MessageBoxIcon.Exclamation)
        {
            return MessageBox.Show(text, caption, buttons, icon);
        }

        public object ShowError(Exception ex)
        {
            return ShowError(ex.ToString());
        }

        public object ShowError(
          Exception ex,
          string caption,
          MessageBoxButtons buttons = MessageBoxButtons.OK,
          MessageBoxIcon icon = MessageBoxIcon.Exclamation)
        {
            return ShowError(ex.ToString(), caption, buttons, icon);
        }
    }
}

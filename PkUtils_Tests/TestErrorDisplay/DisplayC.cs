using System;
using System.Windows.Forms;

namespace PK.TestErrorDisplay
{
    /// <summary>
    /// Implements IErrorPresenter completely without implicit arguments definition.
    /// </summary>
    internal class DisplayC : IErrorDisplay
    {
        public object ShowError(string text)
        {
            return MessageBox.Show(text);
        }

        public object ShowError(
          string text,
          string caption,
          MessageBoxButtons buttons,
          MessageBoxIcon icon)
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
          MessageBoxButtons buttons,
          MessageBoxIcon icon)
        {
            return ShowError(ex.ToString(), caption, buttons, icon);
        }
    }
}

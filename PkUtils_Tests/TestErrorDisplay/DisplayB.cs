using System;
using System.Windows.Forms;

namespace PK.TestErrorDisplay
{
    /// <summary>
    /// Implements IErrorPresenter with DIFFERENT implicit arguments ( different from the interface definition).
    /// In the last overload, also the number of implicit arguments is different from the interface definition.
    /// </summary>
    internal class DisplayB : IErrorDisplay
    {
        public virtual object ShowError(string text)
        {
            return MessageBox.Show(text);
        }

        public virtual object ShowError(
          string text,
          string caption,
          MessageBoxButtons buttons = MessageBoxButtons.YesNo,
          MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            return MessageBox.Show(text, caption, buttons, icon);
        }

        public virtual object ShowError(Exception ex)
        {
            return ShowError(ex.ToString());
        }

        public virtual object ShowError(
          Exception ex,
          string caption,
          MessageBoxButtons buttons,
          MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            return ShowError(ex.ToString(), caption, buttons, icon);
        }
    }
}

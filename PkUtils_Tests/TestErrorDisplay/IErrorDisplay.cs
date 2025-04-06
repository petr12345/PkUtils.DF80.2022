using System;
using System.Windows.Forms;

namespace PK.TestErrorDisplay
{
    public interface IErrorDisplay
    {
        object ShowError(string text);
        object ShowError(
          string text,
          string caption,
          MessageBoxButtons buttons = MessageBoxButtons.OK,
          MessageBoxIcon icon = MessageBoxIcon.Exclamation);

        object ShowError(Exception ex);
        object ShowError(
          Exception ex,
          string caption,
          MessageBoxButtons buttons = MessageBoxButtons.OK,
          MessageBoxIcon icon = MessageBoxIcon.Exclamation);
    }
}

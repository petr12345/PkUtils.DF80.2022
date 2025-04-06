using System;
using System.Globalization;
using System.Windows.Forms;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;
using PK.PkUtils.UI.General;
using PK.PkUtils.UI.Utils;

namespace PK.TestSingletonGeneric
{
    public class Highlander : Singleton<Highlander>
    {
        private Highlander()
        {
            string strTp = this.GetType().TypeToReadable();
            RtlAwareMessageBox.Show(
              null,
              string.Format(CultureInfo.InvariantCulture, "{0} is born.{1}There can be only one...", strTp, Environment.NewLine),
              strTp,
              MessageBoxIcon.Information);
        }

        public void Fight()
        {
            using (WaitCursor wc = new(null))
            {
                RtlAwareMessageBox.Show(
                  null,
                  "Fihting...",
                  this.GetType().TypeToReadable(),
                  MessageBoxIcon.Warning);
            }
        }
    }
}

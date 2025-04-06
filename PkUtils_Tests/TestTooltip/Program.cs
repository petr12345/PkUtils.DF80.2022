using System;
using System.Windows.Forms;
using PK.TestTooltip.Properties;

namespace PK.TestTooltip
{
    internal static class Program
    {
        #region Fields
        private static Settings _settings;
        #endregion // Fields

        #region Properties
        public static Settings Settings
        {
            get { return _settings; }
        }
        #endregion // Properties

        #region Methods

        private static void LoadSettings()
        {
            _settings = new Settings();
        }

        private static void SaveSettings()
        {
            _settings.Save();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            LoadSettings();

            // In previous version of ComboBoxTipHandler, the tooltips for combo boxes did not work quite well 
            // if EnableVisualStyles() was called in the program initialization.
            // In more detail, tooltips for the very bottom items of ComboBox were not displayed.
            // For more info, see the code of ComboBoxTipHandler.ItemFromPoint
            if (Settings.UseVisualStyles)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                // For more info about SetCompatibleTextRenderingDefault see
                // http://blogs.msdn.com/b/jfoscoding/archive/2005/10/13/480632.aspx
            }
            Application.Run(new MainForm());

            SaveSettings();
        }
        #endregion // Methods
    }
}

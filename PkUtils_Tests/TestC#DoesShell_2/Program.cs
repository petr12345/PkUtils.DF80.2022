using System;
using System.Windows.Forms;


namespace WinTester2
{
    internal static class Program
    {
        #region Fields
#if SETTINGS
    private static Settings _settings;
#endif // SETTINGS
        #endregion // Fields

        #region Properties
#if SETTINGS
    public static Settings Settings
    {
      get { return _settings; }
    }
#endif // SETTINGS
        #endregion // Properties

        #region Methods

        private static void LoadSettings()
        {
#if SETTINGS
      _settings = new Settings();
#endif // SETTINGS
        }

        private static void SaveSettings()
        {
#if SETTINGS
      _settings.Save();
#endif // SETTINGS
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            LoadSettings();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            SaveSettings();
        }

        #endregion // Methods
    }
}
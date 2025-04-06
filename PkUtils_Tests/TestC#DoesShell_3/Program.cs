using System;
using System.Diagnostics;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.ShellLib;
using WinTester3.Properties;

namespace WinTester3
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

        internal static ShellApi.AppBarEdges SettingsEdge
        {
            get
            {
                Debug.Assert(null != _settings);

                string strVal = _settings.MainFormEdge;
                var result = EnumExtension.ToEnum<ShellApi.AppBarEdges>(strVal);

                return result;
            }
            set
            {
                Debug.Assert(null != _settings);

                string strVal = value.ToString();
                _settings.MainFormEdge = strVal;
            }
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
        private static void Main()
        {
            LoadSettings();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());

            SaveSettings();
        }

        #endregion // Methods
    }
}
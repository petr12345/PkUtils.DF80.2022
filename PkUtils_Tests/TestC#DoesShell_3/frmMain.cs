// Ignore Spelling: frm, edg
//
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.ShellLib;
using PK.PkUtils.Utils;
using WinTester3.Properties;
using ShellLib = PK.PkUtils.ShellLib;

namespace WinTester3
{
    /// <summary>
    /// The main application form
    /// </summary>
    public partial class frmMain : ShellLib.ApplicationDesktopToolbar
    {
        #region Fields

        /// <summary> Checks for reentrancy around SetEdge method ( to prevent rdo_CheckedChanged doing changes second time)</summary>
        private readonly UsageCounter _settingEdgeLock = new();

        #endregion // Fields

        #region Constructor(s)

        public frmMain()
        {
            InitializeComponent();
            this.Icon = Resource.App;
        }
        #endregion // Constructor(s)

        #region Properties

        protected IUsageCounter SettingEdgeLock
        {
            get { Debug.Assert(null != _settingEdgeLock); return _settingEdgeLock; }
        }
        #endregion // Properties

        #region Methods
        #region Public methods

        #endregion // Public methods

        #region Protected methods

        /// <summary>
        /// Returns the radio button that should be checked for given <see cref="AppBarEdges"/> value of <paramref name="edg"/>.
        /// </summary>
        /// <param name="edg"></param>
        /// <returns>Found radio button or null.</returns>
        [CLSCompliant(false)]
        protected RadioButton RadioForEdge(ShellApi.AppBarEdges edg)
        {
            RadioButton rb = null;

            switch (edg)
            {
                case ShellApi.AppBarEdges.Left:
                    rb = this.rdoLeft;
                    break;

                case ShellApi.AppBarEdges.Top:
                    rb = this.rdoTop;
                    break;

                case ShellApi.AppBarEdges.Right:
                    rb = this.rdoRight;
                    break;

                case ShellApi.AppBarEdges.Bottom:
                    rb = this.rdoBottom;
                    break;

                case ShellApi.AppBarEdges.Float:
                    rb = this.rdoFloat;
                    break;
            }

            return rb;
        }

        /// <summary>
        /// Returns the enum value representing how should be the edge set, according to current radio buttons status.
        /// </summary>
        ///
        /// <returns> AppBarEdges value. </returns>
        [CLSCompliant(false)]
        protected Nullable<ShellApi.AppBarEdges> RequiredEdge()
        {
            RadioButton rbChecked = this.AllControls().OfType<RadioButton>().Where(rb => rb.Checked).FirstOrDefault();
            Nullable<ShellApi.AppBarEdges> result = null;

            if (null != rbChecked)
            {
                result = Enum.GetValues<ShellApi.AppBarEdges>().Where(edg => (RadioForEdge(edg) == rbChecked)).Single();
            }
            return result;
        }

        [CLSCompliant(false)]
        protected void SetEdge(ShellApi.AppBarEdges edg, bool bEnforce)
        {
            RadioButton rbCheck;

            if (bEnforce || (Edge != edg))
            {
                if (SettingEdgeLock.IsUsed)
                {
                    return;  // nothing to do
                }
                using (var usageWrapper = new UsageMonitor(SettingEdgeLock))
                {
                    rbCheck = RadioForEdge(edg);
                    this.Edge = edg;
                    if (null != rbCheck)
                    {
                        rbCheck.Checked = true;
                    }
                }
            }
        }
        #endregion // Protected methods

        #region Private methods

        private void frmMain_Load(object sender, System.EventArgs e)
        {
            ShellApi.AppBarEdges edg = Program.SettingsEdge;

            SetEdge(edg, true);
        }

        private void rdo_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!SettingEdgeLock.IsUsed)
            {
                RadioButton rdo = sender as RadioButton;
                Nullable<ShellApi.AppBarEdges> edg = null;

                if (rdo.Checked)
                {
                    edg = RequiredEdge();
                    if (edg.HasValue)
                    {
                        this.Edge = edg.Value;
                        Program.SettingsEdge = edg.Value;
                    }
                }
            }
        }

        private void _checkBxOnTop_CheckedChanged(object sender, EventArgs e)
        {
            AppBarStates oldState = AppbarGetTaskbarState();
            AppBarStates newState = oldState;

            bool bOldTopMost = this.TopMost;
            bool bNewTopMost = this.TopMost;

            if (this._checkBxOnTop.Checked)
            {
                newState |= AppBarStates.AlwaysOnTop;
                bNewTopMost = true;
            }
            else
            {
                newState &= ~AppBarStates.AlwaysOnTop;
                bNewTopMost = false;
            }

            // Must compare both values, 
            // since sometimes the AppbarSetTaskbarState juts does not succeeeds in chaging the value...
            if ((newState != oldState) || (bOldTopMost != bNewTopMost))
            {
                AppbarSetTaskbarState(newState);
            }
        }
        #endregion // Private methods
        #endregion // Methods
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PK.PkUtils.UI.General;
using PK.TestMsgBoxYesNoAll.Properties;
using MsgBoxResult = PK.PkUtils.UI.General.MsgBoxYesNoAll.MsgBoxResult;
using MsgBoxResultLP = PK.PkUtils.UI.General.MsgBoxYesNoAllPanelBased.MsgBoxResult;

namespace PK.TestMsgBoxYesNoAll
{
    public partial class TestForm : Form
    {
        #region Fields
        private const int _nMaxTestLoopCount = 12;

        private readonly string _strOriginalInfo;
        private readonly string _strTestTitle = "Testing MsgBoxYesNoAll";
        private readonly string _strTestTitleLP = "Testing MsgBoxYesNoAllPanelBased";

        private readonly MessageBoxIcon[] _TestIcons = [
            MessageBoxIcon.None,
            MessageBoxIcon.Error,
            MessageBoxIcon.Question,
            MessageBoxIcon.Warning,
            MessageBoxIcon.Information,
        ];

        private readonly IReadOnlyDictionary<MsgBoxYesNoAll.MsgBoxResult, string> _btnsModifiedTexts = new Dictionary<MsgBoxYesNoAll.MsgBoxResult, string>()
        {
            { MsgBoxYesNoAll.MsgBoxResult.Yes, "Yes!"},
            { MsgBoxYesNoAll.MsgBoxResult.YesToAll, "Yes to All"},
            { MsgBoxYesNoAll.MsgBoxResult.No, "Oh No"},
            { MsgBoxYesNoAll.MsgBoxResult.NoToAll, ""},
            { MsgBoxYesNoAll.MsgBoxResult.Cancel, "Cancel it"},
        };

        private readonly IReadOnlyDictionary<MsgBoxYesNoAllPanelBased.MsgBoxResult, string> _btnsModifiedTextsLP = new Dictionary<MsgBoxYesNoAllPanelBased.MsgBoxResult, string>()
        {
            { MsgBoxYesNoAllPanelBased.MsgBoxResult.Yes, "Yes!"},
            { MsgBoxYesNoAllPanelBased.MsgBoxResult.YesToAll, "Yes to All"},
            { MsgBoxYesNoAllPanelBased.MsgBoxResult.No, "Oh No"},
            { MsgBoxYesNoAllPanelBased.MsgBoxResult.NoToAll, ""},
            { MsgBoxYesNoAllPanelBased.MsgBoxResult.Cancel, "Cancel it"},
        };
        #endregion // Fields

        #region Constructor(s)

        public TestForm()
        {
            InitializeComponent();

            this.Icon = Resources.TestMsgBoxYesNoAll;
            _strOriginalInfo = _lblInfo.Text;

            InitFromSettings();
        }
        #endregion // Constructor(s)

        #region Methods

        private void InitFromSettings()
        {
            Settings sett = Settings.Default;
            this._checkBxTestLayoutPanel.Checked = sett.TestTableLayoutPanel;
            this._checkBxTestCustomBtnsTexts.Checked = sett.UseCustomTexts;
        }

        private void SaveSettings()
        {
            Settings sett = Settings.Default;
            sett.TestTableLayoutPanel = this._checkBxTestLayoutPanel.Checked;
            sett.UseCustomTexts = this._checkBxTestCustomBtnsTexts.Checked;
            sett.Save();
        }

        /// <summary>   Test of class MsgBoxYesNoAll. </summary>
        /// <param name="bCustomTexts"> True to custom texts. </param>
        private void Test_MsgBoxYesNoAll(bool bCustomTexts)
        {
            // prepare the dialog and its text
            string strMainInstructionBase = "This is a good question, isn't it?";
            string strRandomContents = " Now this is just dummy sentence to make it all longer.";
            string strContentTextBase = string.Format(CultureInfo.CurrentCulture,
                "This test demonstrates the behaviour of MsgBoxYesNoAll class.{0}You will be asked maximally {1} times a Yes or No, and given a running total.",
                Environment.NewLine, _nMaxTestLoopCount);

            // make some other preparation
            bool bStop = false;
            int nYes_Count = 0;
            int n_No_Count = 0;

            // reset the info text, to handle the case if this is not the first click
            this._lblInfo.Text = _strOriginalInfo;

            //  loop and count the yes and no replies
            for (int ii = 0; !bStop && (ii < _nMaxTestLoopCount); ii++)
            {
                MsgBoxYesNoAll msgBx = new(bCustomTexts ? _btnsModifiedTexts : null);
                string strContentTemp = ((ii % 2) == 0) ? strContentTextBase : (strContentTextBase + strRandomContents);
                IEnumerable<string> moreLines = Enumerable.Range(0, ii).Select(
                  n => string.Format(CultureInfo.CurrentCulture, "[Another test line {0}]", n + 3));
                string strMainInstruction = strMainInstructionBase;
                string strContentText = moreLines.Aggregate(strContentTemp,
                  (workingSent, next) => workingSent + Environment.NewLine + next);


                int nIconIndex = ii % _TestIcons.Length;
                MessageBoxIcon icon = _TestIcons[nIconIndex];
                MsgBoxResult result = msgBx.ShowDialogEx(this, strMainInstruction, strContentText, _strTestTitle, icon);

                switch (result)
                {
                    case MsgBoxResult.Cancel:
                        bStop = true;
                        break;

                    case MsgBoxResult.Yes:
                    case MsgBoxResult.YesToAll:
                        nYes_Count++;
                        break;

                    case MsgBoxResult.No:
                    case MsgBoxResult.NoToAll:
                        n_No_Count++;
                        break;
                }
                UpdateLabel(nYes_Count, n_No_Count);
            }
        }

        /// <summary>
        /// Test of class MsgBoxYesNoAllPanelBased
        /// </summary>
        private void Test_MsgBoxYesNoAllLP(bool bCustomTextx)
        {
            // prepare the dialog and its text
            string strMainInstructionBase = "This is a good question, isn't it?";
            string strRandomContents = " Now this is just dummy sentence to make it all longer.";
            string strContentTextBase = string.Format(CultureInfo.CurrentCulture,
                "This test demonstrates the behaviour of MsgBoxYesNoAllPanelBased class.{0}You will be asked maximally {1} times a Yes or No, and given a running total.",
                Environment.NewLine, _nMaxTestLoopCount);

            // make some other preparation
            bool bStop = false;
            int nYes_Count = 0;
            int n_No_Count = 0;

            // reset the info text, to handle the case if this is not the first click
            this._lblInfo.Text = _strOriginalInfo;

            //  loop and count the yes and no replies
            for (int ii = 0; !bStop && (ii < _nMaxTestLoopCount); ii++)
            {
                MsgBoxYesNoAllPanelBased msgBx = new(bCustomTextx ? _btnsModifiedTextsLP : null);
                StringBuilder sbContentTemp = new();

                sbContentTemp.Append(strContentTextBase);
                if ((ii % 2) > 0)
                    sbContentTemp.Append(strRandomContents);
                sbContentTemp.AppendLine();
                sbContentTemp.Append("Double-click the bottom of the dialog to turn cell emphasizing on/off.");

                IEnumerable<string> moreLines = Enumerable.Range(0, ii).Select(
                  n => string.Format(CultureInfo.CurrentCulture, "[Another test line {0}]", n + 3));
                string strMainInstruction = strMainInstructionBase;
                string strContentText = moreLines.Aggregate(sbContentTemp.ToString(),
                  (workingSent, next) => workingSent + Environment.NewLine + next);

                int nIconIndex = ii % _TestIcons.Length;
                MessageBoxIcon icon = _TestIcons[nIconIndex];
                MsgBoxResultLP result = msgBx.ShowDialogEx(this, strMainInstruction, strContentText, _strTestTitleLP, icon);

                switch (result)
                {
                    case MsgBoxResultLP.Cancel:
                        bStop = true;
                        break;

                    case MsgBoxResultLP.Yes:
                    case MsgBoxResultLP.YesToAll:
                        nYes_Count++;
                        break;

                    case MsgBoxResultLP.No:
                    case MsgBoxResultLP.NoToAll:
                        n_No_Count++;
                        break;
                }
                UpdateLabel(nYes_Count, n_No_Count);
            }
        }

        private void UpdateLabel(int yesCount, int noCount)
        {
            _lblInfo.Text = $"Yes: {yesCount} No: {noCount}";
        }
        #endregion // Methods

        #region Event Handlers

        private void OnTestButton_Click(object sender, EventArgs e)
        {
            bool bCustomTexts = _checkBxTestCustomBtnsTexts.Checked;

            if (!_checkBxTestLayoutPanel.Checked)
                Test_MsgBoxYesNoAll(bCustomTexts);
            else
                Test_MsgBoxYesNoAllLP(bCustomTexts);
        }

        private void TestFor_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveSettings();
        }

        private void OnBtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion // Event Handlers
    }
}

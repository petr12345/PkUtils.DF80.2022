using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using PK.PkUtils.XmlSerialization;
using PK.SubstEditLib.Subst;

#pragma warning disable SYSLIB0011 // BinaryFormatter serialization is obsolete and should not be used.

namespace PK.TestTgSchema.TextBoxCtrls
{
    /// <summary>
    /// Generic control extending the base SubstEditTextBoxCtrl ( adds IO-functionality ).
    /// </summary>
    /// <typeparam name="TFIELDID"></typeparam>
    public class TaggingSchemaTextBoxGenericCtrl<TFIELDID> : SubstEditTextBoxCtrl<TFIELDID>
    {
        #region Typedefs
        #endregion // Typedefs

        #region Fields

        protected const string _strOpenFailed = "File reading has failed";
        protected const string _strSaveFailed = "File saving has failed";
        #endregion // Fields

        #region Constructor(s)

        public TaggingSchemaTextBoxGenericCtrl()
        {
        }
        #endregion // Constructor(s)

        #region Properties
        #endregion // Properties

        #region Methods
        /// <summary>
        /// Open the given file and assigns its contents
        /// </summary>
        /// <param name="strOpenFile"></param>
        /// <param name="fmt"></param>
        /// <returns></returns>
        public bool DoOpen(string strOpenFile, FileFormatType fmt)
        {
            string strPlain;
            SubstLogData<TFIELDID> logData;
            bool bOk = false;

            try
            {
                switch (fmt)
                {
                    case FileFormatType.fmtBinary:
                        using (Stream stream = new FileStream(strOpenFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            BinaryFormatter formatter = new();
                            logData = formatter.Deserialize(stream) as SubstLogData<TFIELDID>;
                        }
                        TheHook.Assign(logData);
                        bOk = true;
                        break;

                    case FileFormatType.fmtPlainText:
                        using (FileStream fs = new(strOpenFile, FileMode.Open, FileAccess.Read))
                        {
                            using StreamReader sr = new(fs);
                            strPlain = sr.ReadToEnd();
                            this.AssignPlainText(strPlain);
                        }
                        bOk = true;
                        break;

                    case FileFormatType.fmtXml:
                        {
                            var serializer = new XMLSerializerAdapter<SubstLogData<TFIELDID>>();
                            logData = serializer.ReadFile(strOpenFile);
                            this.Assign(logData);
                            bOk = true;
                        }
                        break;
                }
            }
            catch (IOException ex)
            {
                ShowException(ex, _strOpenFailed);
            }
            catch (System.UnauthorizedAccessException ex)
            {
                ShowException(ex, _strOpenFailed);
            }
            catch (System.InvalidOperationException ex)
            {
                ShowException(ex, _strOpenFailed);
            }

            if (bOk)
            {
                this.EmptyUndoBuffer();
            }
            return bOk;
        }

        /// <summary>
        /// Save to a given file
        /// </summary>
        /// <param name="strSaveFile"></param>
        /// <param name="fmt"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public bool DoSaveAs(string strSaveFile, FileFormatType fmt, Encoding encoding)
        {
            SubstLogData<TFIELDID> logData;
            bool bOk = false;

            try
            {
                switch (fmt)
                {
                    case FileFormatType.fmtBinary:
                        TheHook.PhysData.ExportLogAll(logData = new SubstLogData<TFIELDID>());
                        using (FileStream fs = new(strSaveFile, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            BinaryFormatter formatter = new();
                            formatter.Serialize(fs, logData);
                            bOk = true;
                        }
                        break;

                    case FileFormatType.fmtPlainText:
                        using (FileStream fs = new(strSaveFile, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            using StreamWriter sw = new(fs, Encoding.Unicode);
                            sw.Write(TheHook.GetPlainText());
                        }
                        bOk = true;
                        break;

                    case FileFormatType.fmtXml:
                        TheHook.PhysData.ExportLogAll(logData = new SubstLogData<TFIELDID>());
                        var serializer = new XMLSerializerAdapter<SubstLogData<TFIELDID>>();
                        /* writing without xml declaration line
                        serializer.WriteFile(strSaveFile, formatProvider, encoding, logData);
                        */
                        bOk = serializer.WriteXmlFile(strSaveFile, encoding, logData);
                        break;
                }
            }
            catch (IOException ex)
            {
                ShowException(ex, _strSaveFailed);
            }
            catch (System.UnauthorizedAccessException ex)
            {
                ShowException(ex, _strSaveFailed);
            }
            catch (System.InvalidOperationException ex)
            {
                ShowException(ex, _strOpenFailed);
            }

            return bOk;
        }

        /// <summary>
        /// Auxiliary helper that should gaurantee the modified contents is saved.
        /// Returns true if it is safe to continue and discard the edit control contents; 
        /// false if the control contents should be preserved.
        /// </summary>
        /// <param name="saveDlg"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public bool SaveModified(SaveFileDialog saveDlg, Encoding encoding)
        {
            bool bOk = true;

            if (this.IsModified)
            {
                DialogResult dlgRes;
                string strClassName = ShortClassName();
                string strMsg = string.Format(
                    CultureInfo.InvariantCulture,
                    "The contents of {0} has been modified.{1}Save ?", strClassName, Environment.NewLine);

                dlgRes = MessageBox.Show(
                    this,
                    strMsg,
                    "Save modifications",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (dlgRes)
                {
                    case DialogResult.Yes:
                        if (saveDlg.ShowDialog() == DialogResult.Cancel)
                        {
                            bOk = false;
                        }
                        else
                        {
                            bOk = DoSaveAs(saveDlg.FileName, (FileFormatType)saveDlg.FilterIndex, encoding);
                        }
                        break;

                    case DialogResult.No:
                        bOk = true;
                        break;

                    case DialogResult.Cancel:
                        bOk = false;
                        break;
                }

            }
            return bOk;
        }

        /// <summary>
        /// Helpwer method, called on IO-error
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="strMsg"></param>
        protected void ShowException(Exception ex, string strMsg)
        {
            string strErr = string.IsNullOrEmpty(strMsg) ? "Error" : strMsg;
            StringBuilder sb = new(ex.Message);

            sb.Append(Environment.NewLine);
            sb.Append("Call stack: ");
            sb.Append(ex.StackTrace);
            System.Windows.Forms.MessageBox.Show(
                sb.ToString(),
                strErr,
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Exclamation,
                System.Windows.Forms.MessageBoxDefaultButton.Button1);
        }

        /// <summary>
        /// Implementation helper
        /// </summary>
        /// <returns></returns>
        protected string ShortClassName()
        {
            int nDex;
            string strClassName = this.GetType().ToString();

            if (0 < (nDex = strClassName.LastIndexOf('.')))
            {
                strClassName = strClassName[(nDex + 1)..];
            }

            return strClassName;
        }
        #endregion // Methods
    }

    /// <summary>
    /// File format enum; the order of valid values ( fmtBinary through fmtXml ) matches to 
    /// the order of items in FileDialog FilterIndex.
    /// </summary>
    public enum FileFormatType
    {
        fmtNone = 0,
        fmtXml,
        fmtBinary,
        fmtPlainText,
    }

}
#pragma warning restore SYSLIB0011
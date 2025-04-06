// define following if you want to see MassageBox-base output
// #define USE_MSG_BOX_OUTPUT

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using PK.PkUtils.Dump;
using PK.PkUtils.Interfaces;
using PK.PkUtils.Utils;
using PK.TestExceptionHelper.Properties;

namespace PK.TestExceptionHelper
{
    public partial class MainForm : Form
    {
        #region Fields
        /// <summary>
        /// The wrapper around the TextBox control, providing the IDumper-behaviour 
        /// for that control.
        /// </summary>
        private DumperCtrlTextBoxWrapper _wrapper;
        private const int _maxMsgHistoryItems = 512;

        private const string _strThickLine = "=======";
        private const string _strThinLine = "--------";
        private const string _strNoticePreserved = " Note the call stack is preserved.";
        private const string _strNoticeNotPreserved = " Note the call stack is NOT preserved !!";
#if USE_MSG_BOX_OUTPUT
    private readonly string _strThinSeparator = _strThinLine;
#else
        private readonly string _strThinSeparator = string.Empty;
#endif

        #endregion // Fields

        #region Constructor(s)
        public MainForm()
        {
            InitializeComponent();
            InitializeControls();
            this.Icon = Resources.App;
        }
        #endregion // Constructor(s)

        #region Properties

        protected IDumper Dumper
        {
            get { return this._wrapper; }
        }
        #endregion // Properties

        #region Methods

        #region Protected Methods

        protected void InitializeControls()
        {
            this._wrapper = new DumperCtrlTextBoxWrapper(this._textBxOut, _maxMsgHistoryItems);
        }
        #endregion // Protected Methods

        #region Private Methods

        private static void TESTED_INVOKED_FUNC()
        {
            throw new InvalidOperationException("some funny exception thrown during TESTED_INVOKED_FUNC");
        }

        private static string GiveMeMessageAndCallStack(Exception ex, string strLine)
        {
            string strMsg, strCallStack, strRes;

            strMsg = string.Format(CultureInfo.InvariantCulture,
              "{0}The message:{1}{2}",
              strLine, Environment.NewLine, ex.Message);

            strCallStack = string.Format(CultureInfo.InvariantCulture,
              "{0}The call stack:{1}{2}{3}",
              strLine, Environment.NewLine, ex.StackTrace.ToString(), Environment.NewLine);

            strRes = string.Format(CultureInfo.InvariantCulture,
              "{0}{1}{2}{3}",
              strMsg, Environment.NewLine, strCallStack, Environment.NewLine);

            return strRes;
        }

        private void DisplayTheException(Exception e, string strTitleFormat, List<string> listOut)
        {
            string strType = e.GetType().ToString();
            string strTitle = string.Format(CultureInfo.InvariantCulture, strTitleFormat, strType);
            string strMsg = GiveMeMessageAndCallStack(e, _strThinSeparator);

#if USE_MSG_BOX_OUTPUT
          RtlAwareMessageBox.Show(
            null,
            strMsg,
            strTitle,
            System.Windows.Forms.MessageBoxIcon.Exclamation);
#else // USE_MSG_BOX_OUTPUT
            listOut.Add(_strThickLine + strTitle + _strThickLine);
            listOut.AddRange(strMsg.Split(new string[] { Environment.NewLine },
              int.MaxValue, StringSplitOptions.RemoveEmptyEntries));
#endif // USE_MSG_BOX_OUTPUT
        }

        private void TestHandleInvokedException(bool bUseExceptionHelper)
        {
            List<string> listOut = [];

            try
            {
                try
                {
                    this.GetType().InvokeMember(nameof(TESTED_INVOKED_FUNC),
                      BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic, null, null, null);
                }
                catch (Exception e)
                {
                    // Despite the fact that function TESTED_INVOKED_FUNC raised InvalidOperationException,
                    // the exception 'e' which comes here is TargetInvocationException;
                    // the original exception ( InvalidOperationException in this case) is wrapped as its inner exception.
                    DisplayTheException(e, " THE FIRST OCCURENCE of {0} has been caught ", listOut);
                    if (e is TargetInvocationException)
                    {
                        DisplayTheException(e.InnerException, " Note the inner exception is ", listOut);
                    }
                    listOut.AddRange(new string[] { string.Empty, string.Empty });

                    if (!bUseExceptionHelper)
                    {
                        // An example of wrong handling.
                        // This kind of throw damages the original call stack information;
                        // and the code who will handle that has no way to figure out that TESTED_INVOKED_FUNC() has been called.
                        throw (e.InnerException != null) ? e.InnerException : e;
                    }
                    else
                    {
                        // An example of good handling.
                        // For case of TargetInvocationException, let's throw the actual cause of problem ( e.InnerException ).
                        // To preserve the original stack somewhere, the PrepareRethrow will insert 
                        // a new exception of type RethrownException, who keeps the original call stack.

                        if (e is TargetInvocationException)
                            throw ExceptionHelper.PrepareRethrow(e.InnerException);
                        else
                            throw;
                    }
                }
            }
            catch (Exception ex)
            {
                string strNotice = _strThickLine + (bUseExceptionHelper ? _strNoticePreserved : _strNoticeNotPreserved);
                RethrownException exRethrownInner = ex.InnerException as RethrownException;
                Exception exDisplay = (null != exRethrownInner) ? exRethrownInner : ex;

                DisplayTheException(exDisplay, " RETHROWN {0} has been caught ", listOut);
                listOut.Add(strNotice);
            }
#if !USE_MSG_BOX_OUTPUT
            foreach (string str in listOut) this.Dumper.DumpLine(str);
#endif // !USE_MSG_BOX_OUTPUT
        }
        #endregion // Private Methods

        #region Event_handlers

        private void _btnTestGood_Click(object sender, EventArgs e)
        {
            Dumper.Reset();
            TestHandleInvokedException(true);
        }

        private void _btnTestWrong_Click(object sender, EventArgs e)
        {
            Dumper.Reset();
            TestHandleInvokedException(false);
        }

        private void _btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion // Event_handlers
        #endregion // Methods
    }
}

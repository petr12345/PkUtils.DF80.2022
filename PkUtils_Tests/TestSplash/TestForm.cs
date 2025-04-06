using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using PK.PkUtils.UI.Splash;
using PK.PkUtils.Utils;
using PK.PkUtils.WinApi;

namespace PK.TestSplash
{
    /// <summary>
    /// Summary description for Form
    /// </summary>
    public partial class TestForm : System.Windows.Forms.Form
    {
        #region Fields
        private ISplashFactory _sf;
        private bool _bLayoutCalled;
        private DateTime _dtStarted;
        private const int _ShowDurationSecs = 4;
        #endregion // Fields

        #region Constructor(s)

        public TestForm()
        {
            LongInitialization();
            InitializeComponent();
            this.Icon = TestSplash.Properties.Resources.App;
        }
        #endregion // Constructor(s)

        #region Properties

        protected ISplashFactory SplashFactory
        {
            get
            {
                if (null == _sf)
                {
                    _sf = new SplashFactory();
                }
                return _sf;
            }
        }
        #endregion // Properties

        #region Methods

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Disposer.SafeDispose(ref components);
                Disposer.SafeDispose(ref _sf);
            }
            base.Dispose(disposing);
        }

        protected void LongInitialization()
        {
            ISplashWindow isplash;
            ISplashFactory sf = SplashFactory;

            // Create splash. With Timeout.Infinite will wait till the splash thread has a chance to create and show splash window.
            // This is not mandatory for the splash screen work, we just want to see all following texts actually displayed.
            // Without that, the splash screen may not make it, and pop-up only with second or third text being displayed. 
            sf.CreateSplash(Timeout.Infinite, ThreadPriority.Normal, "PK.TestSplash.splash.jpg", Assembly.GetExecutingAssembly());
            Debug.Assert(sf.IsSplashVisible);
            isplash = sf.SplashWindow;

            isplash.SetTextsAndReference("In this example the status text is constant", "Loading module 1");
            System.Threading.Thread.Sleep(500);
            isplash.SetTextProgressBar("Loading module 2");
            System.Threading.Thread.Sleep(300);
            isplash.SetTextProgressBar("Loading module 3");
            System.Threading.Thread.Sleep(900);
            isplash.SetTextProgressBar("Loading module 4");
            System.Threading.Thread.Sleep(100);
            isplash.SetTextProgressBar("Loading module 5");
            System.Threading.Thread.Sleep(400);
            isplash.SetTextProgressBar("Loading module 6");
            System.Threading.Thread.Sleep(50);
            isplash.SetTextProgressBar("Loading module 7");
            System.Threading.Thread.Sleep(240);
            isplash.SetTextProgressBar("Loading module 8");
            System.Threading.Thread.Sleep(900);
            isplash.SetTextProgressBar("Loading module 9");
            System.Threading.Thread.Sleep(240);
            isplash.SetTextProgressBar("Loading module 10");
            System.Threading.Thread.Sleep(90);
            isplash.SetTextProgressBar("Loading module 11");
            System.Threading.Thread.Sleep(2000);
            isplash.SetTextProgressBar("Loading module 12");
            System.Threading.Thread.Sleep(100);
            isplash.SetTextProgressBar("Loading module 13");
            System.Threading.Thread.Sleep(500);
            isplash.SetTexts(null, "Loading module 14", false);
            System.Threading.Thread.Sleep(1000);
            isplash.SetTexts(null, "Loading module 14a", false);
            System.Threading.Thread.Sleep(1000);
            isplash.SetTexts(null, "Loading module 14b", false);
            System.Threading.Thread.Sleep(1000);
            isplash.SetTexts(null, "Loading module 14c", false);
            System.Threading.Thread.Sleep(1000);
            isplash.SetTextProgressBar("Loading module 15");
            System.Threading.Thread.Sleep(20);
            isplash.SetTextProgressBar("Loading module 16");
            System.Threading.Thread.Sleep(450);
            isplash.SetTextProgressBar("Loading module 17");
            System.Threading.Thread.Sleep(240);
            isplash.SetTextProgressBar("Loading module 18");
            System.Threading.Thread.Sleep(90);

            // better do that closing later, in TestFor_Layout event handler
            /* sf.CloseSplash(true); */
        }

        public static void ShowMe()
        {
            TestForm frm1 = new();
            Application.Run(frm1);
        }
        #endregion // Methods

        #region Event handlers

        private void TestFor_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if (_bLayoutCalled == false)
            {
                _bLayoutCalled = true;
                _dtStarted = DateTime.Now;

                // Get rid of splash
                SplashFactory.CloseSplash(true);

                // and activate me.
                // For some weird reason, just this.Activate() is not sufficient, must call the ActivateWnd hack
                /* this.Activate(); */
                User32.ActivateWnd(this);
                _timerClose.Start();
            }
        }

        private void _timerClose_Tick(object sender, System.EventArgs e)
        {
            TimeSpan tsPassed = DateTime.Now.Subtract(_dtStarted);
            int nSecsPassed = (int)tsPassed.TotalSeconds;

            if (nSecsPassed >= _ShowDurationSecs)
            {
                this.Close();
            }
            else
            {
                int remains = _ShowDurationSecs - nSecsPassed;
                string strInfo = string.Format(CultureInfo.CurrentCulture,
                  "The form will close itself in a {0} seconds...", remains);
                _labelInfo.Text = strInfo;
            }
        }
        #endregion // Event handlers
    }
}

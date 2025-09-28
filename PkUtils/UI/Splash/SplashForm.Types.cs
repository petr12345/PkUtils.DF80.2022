///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// The Code Project Open License Notice
// 
// This software is a Derivative Work based upon a Code Project article
// A Pretty Good Splash Screen in C#
// http://www.codeproject.com/KB/cs/prettygoodsplashscreen.aspx
//
// The Code Project Open License (CPOL) text is available at
// http://www.codeproject.com/info/cpol10.aspx
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////


#define SUPPORT_FADE

// Ignore Spelling: Utils
//
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.Threading;
using PK.PkUtils.Utils;

namespace PK.PkUtils.UI.Splash;

public partial class SplashForm : Form, ISplashWindow
{
    #region Typedefs

    /// <summary>
    /// Class SplashInitData keeps initialization data for the new SplashScreenThread, 
    /// which is created by SplashFactory.
    /// The actual splash form is created and destroyed inside the worker thread.
    /// </summary>
    public class SplashInitData : IDisposable
    {
        #region Fields

        /// <summary>
        /// A splash screen background image. Should be declared as read-only,
        /// but in that case I cannot get rid of that in dispose.
        /// </summary>
        private Image _bgImage;
        private readonly bool _isImageOwner;
        private readonly ThreadPriority _priority;
        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Constructor initializing ThreadPriority, background image and the IsImageOwner property value.
        /// </summary>
        /// <param name="priority">Priority of the worker thread that will be created</param>
        /// <param name="backgroundImage">The background image that should be used for a splash form</param>
        /// <param name="isImageOwner">True if this object should become an owner of <paramref name="backgroundImage"/> and should dispose it.</param>
        public SplashInitData(
          ThreadPriority priority,
          Image backgroundImage,
          bool isImageOwner)
        {
            _priority = priority;
            _bgImage = backgroundImage;
            _isImageOwner = isImageOwner;
        }
        #endregion // Constructors

        #region Properties

        /// <summary>
        /// The priority that will be used for SplashScreenThread . Initialized by constructor.
        /// </summary>
        public ThreadPriority Priority
        {
            get { return _priority; }
        }

        /// <summary>
        /// The background image that will be used for the splash form. Initialized by constructor.
        /// </summary>
        public Image BackgroundImage
        {
            get { return _bgImage; }
        }

        /// <summary>
        /// An indicator whether this object is an owner of the background image _bgImage and should dispose it.
        /// </summary>
        public bool IsImageOwner
        {
            get { return _isImageOwner; }
        }
        #endregion // Properties

        #region Methods

        /// <summary>
        /// Cleanup any resources being used.
        /// </summary>
        /// <param name="disposing"> If true, is called by IDisposable.Dispose. 
        /// Otherwise it is called by finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (IsImageOwner)
                { // as an owner must dispose that image
                    Disposer.SafeDispose(ref _bgImage);
                }
                else
                { // just detach the image
                    _bgImage = null;
                }
            }
        }
        #endregion // Methods

        #region IDisposable Members

        /// <summary>
        /// Implements IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion // IDisposable Members
    }

    /// <summary>
    /// The thread which creates, presents and disposes the splash form.
    /// </summary>
    public class SplashScreenThread : WorkerThread
    {
        #region Fields

        /// <summary> A back reference to the factory who created this thread </summary>
        protected ISplashFactory _factory;

        /// <summary> A splash window ( if there is any ) </summary>
        private ISplashWindow _frmSplash;

        /// <summary> A data provided to constructor for initialization </summary>
        private SplashInitData _initData;

        #endregion // Fields

        #region Constructor(s)

        /// <summary>
        /// Creates the splash thread, with given argument SplashInitData initialization data,
        /// and ISplashFactory which is back reference to the thread creator.
        /// The thread becomes an owner of the SplashInitData.
        /// </summary>
        /// <param name="data">  The initial data that will be used for Splash form creation in <see cref="WorkerFunction"/></param>
        /// <param name="factory">The splash factory that will be used for a new splash form creation </param>
        protected internal SplashScreenThread(SplashInitData data, ISplashFactory factory)
          : base(data.Priority)
        {
            ArgumentNullException.ThrowIfNull(data);
            factory.CheckNotDisposed();

            _initData = data;
            _factory = factory;
            this.Name = this.GetType().TypeToReadable();
            this.IsBackground = true;
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary> A splash window ( if there is any ) </summary>
        public ISplashWindow SplashWindow
        {
            get { return _frmSplash; }
        }

        /// <summary> A data provided to constructor for initialization </summary>
        protected SplashInitData SplashInitData
        {
            get { return _initData; }
        }
        #endregion // Properties

        #region Methods

        /// <summary>
        /// Overrides the method of the base class to define this thread specific behavior.
        /// </summary>
        protected override void WorkerFunction()
        {
            SplashForm splash;

            _frmSplash = splash = new SplashForm(SplashInitData.BackgroundImage);

#if SIMULATE_APP_RUN
    // alternative way, if for some reason the Application.Run does not work
    splash.Show();
    while (_factory.IsSplashVisible)
    {
      Application.DoEvents();
      // Have some rest. Without that, the thread occasionally gets stuck for some weird reason.
      Thread.Sleep(16);
    }
#else
            Application.Run(splash);
#endif
            // must dispose the form from the same thread it was created in
            Disposer.SafeDispose(ref _frmSplash);
        }

        /// <summary>
        /// Overrides the method of the base class to perform resources cleanup.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            // call the base disposing FIRST, to let the thread finish nicely.
            base.Dispose(disposing);

            if (disposing)
            {
                _factory = null;
                Debug.Assert(_frmSplash == null, "Must be disposed already");
                Disposer.SafeDispose(ref _initData);
            }
        }
        #endregion // Methods
    }
    #endregion // Typedefs
}
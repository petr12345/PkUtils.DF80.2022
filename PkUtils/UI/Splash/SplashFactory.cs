/***************************************************************************************************************
*
* FILE NAME:   .\UI\Splash\SplashFactory.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class SplashFactory
*
**************************************************************************************************************/


// Ignore Spelling: Utils
//
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.Utils;

namespace PK.PkUtils.UI.Splash;

/// <summary>
/// A factory implementing ISplashFactory interface, thus creating ISplashWindow objects.
/// In this case the factory creates a SplashForm instance.
/// To modify the behavior in a derived class, one should overwrite protected virtual bool DoCreateSplash
/// </summary>
public class SplashFactory : ISplashFactory
{
    #region Fields
    /// <summary>
    /// A worker thread creating new SplashForm
    /// </summary>
    protected SplashForm.SplashScreenThread _splashThread;
    private bool _bDisposed;
    private readonly object _syncRoot = new();
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default argument-less constructor.
    /// </summary>
    public SplashFactory()
    {
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Get the synchronization object that is used for instance-creation lock
    /// </summary>
    protected object SyncRoot
    {
        get { return _syncRoot; }
    }
    #endregion // Properties

    #region Methods

    #region Public Methods

    /// <summary>
    /// Load the image, specified by the arguments strResourceImage and resourceAssembly.
    /// For null or empty argument string it doesn't do anything.
    /// If the resourceAssembly argument is null, the code tries to load the resource
    /// from Assembly.GetExecutingAssembly instead.
    /// </summary>
    /// <param name="strResourceImage">The name of resource stream containing the image.</param>
    /// <param name="resourceAssembly">The assembly containing specified resource</param>
    /// <returns>True on success, false on failure</returns>
    public static Image LoadImage(string strResourceImage, Assembly resourceAssembly)
    {
        Image backgroundImage = null;

        if (!string.IsNullOrEmpty(strResourceImage))
        {
            Assembly asm = resourceAssembly ?? Assembly.GetExecutingAssembly();
            using (Stream strm = asm.GetManifestResourceStream(strResourceImage))
            {
                if (null != strm)
                {
                    backgroundImage = Image.FromStream(strm);
                }
                else
                { /* rather the caller should do that
        string strErr = string.Format(CultureInfo.InvariantCulture,
          "Failed to load resource stream {0}", strResourceImage);
        throw new InvalidOperationException(strErr);
         */
                }
            }
        }
        return backgroundImage;
    }
    #endregion // Public Methods

    #region Protected Methods

    /// <summary>
    /// Implementation helper, actually creating the ISplashWindow object,
    /// in this case an instance of SplashForm.
    /// It is creating the auxiliary object SplashForm.SplashInitData first,
    /// then the SplashForm.SplashScreenThread which becomes an owner of these auxiliary data.
    /// The thread itself creates the SplashForm form.
    /// </summary>
    /// 
    /// <param name="millisecondsTimeout">The time-out in milliseconds</param>
    /// <param name="priority">Priority of the worker thread that will be created</param>
    /// <param name="backgroundImage">The background image that should be used for a splash form</param>
    /// <param name="isImageOwner">True if the splash window should become an owner of <paramref name="backgroundImage"/> and should dispose it.</param>
    /// <returns>True on success, false on failure</returns>
    protected virtual bool DoCreateSplash(int millisecondsTimeout, ThreadPriority priority, Image backgroundImage, bool isImageOwner)
    {
        bool bRes = false;

        if ((millisecondsTimeout < 0) && (millisecondsTimeout != Timeout.Infinite))
        {
            throw new ArgumentOutOfRangeException(
              nameof(millisecondsTimeout),
              millisecondsTimeout,
              "The value of millisecondsTimeout is negative, but it is not equal to Timeout.Infinite, which is the only negative value allowed.");
        }
        // Make sure it's only launched once.
        if (null == SplashWindow)
        {
            lock (SyncRoot)
            {
                if (null == SplashWindow)
                {
                    var dtStarted = DateTime.Now;
                    var iniData = new SplashForm.SplashInitData(priority, backgroundImage, isImageOwner);
                    _splashThread = new SplashForm.SplashScreenThread(iniData, this);
                    _splashThread.Start();
                    bRes = true;

                    if (0 != millisecondsTimeout)
                    {
                        while (!IsSplashVisible)
                        {
                            // DoEvents is not necessary, just Sleep is sufficient
                            /* Application.DoEvents(); */
                            Thread.Sleep(32);
                            if ((millisecondsTimeout != Timeout.Infinite) && (millisecondsTimeout <= DateTime.Now.Subtract(dtStarted).TotalMilliseconds))
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
        return bRes;
    }

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios.
    /// If disposing equals true, the method has been called directly or indirectly 
    /// by a user's code. Managed and unmanaged resources can be disposed.
    /// If disposing equals false, the method has been called by the 
    /// runtime from inside the finalizer and you should not reference 
    /// other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. 
    /// Otherwise it is called by finalizer.
    /// </param>
    /// <remarks> This implementation is thread-safe, as it uses double-checked locking.</remarks>
    [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_splashThread")]
    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (!this.IsDisposed)
        {
            lock (this.SyncRoot)
            {
                if (!this.IsDisposed)
                {
                    // If disposing equals true, dispose both managed and unmanaged resources.
                    if (disposing)
                    {
                        Disposer.SafeDispose(ref _splashThread);
                    }
                    // Now release unmanaged resources. If disposing is false, only that code is executed. 
                    // Actually nothing to do here for this particular class
                    _bDisposed = true;
                }
            }
        }
    }
    #endregion // Protected Methods
    #endregion // Methods

    #region ISplashFactory Members
    #region ISplash Members
    #region IDisposableEx Members
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

    /// <summary>
    ///  Returns true in case the object has been disposed and no longer should be used.
    /// </summary>
    public bool IsDisposed
    {
        get { return _bDisposed; }
    }
    #endregion // IDisposableEx Members

    /// <inheritdoc/>
    public bool IsSplashVisible
    {
        get
        {
            lock (SyncRoot)
            {
                return IsSplashExisting && SplashWindow.IsSplashVisible;
            }
        }
    }

    /// <summary>
    /// Set (change) the status label text, and set new reference point of existing splash window.
    /// </summary>
    /// <param name="newStatusText">New status text.</param>
    /// <remarks>The null argument has a special meaning - indicates that current text should not change.</remarks>
    public void SetTextStatus(string newStatusText)
    {
        if (IsSplashExisting)
        {
            SplashWindow.SetTextStatus(newStatusText);
        }
    }

    /// <summary>
    /// Set (change) the progress bar text, and set new reference point of existing splash window.
    /// </summary>
    /// <param name="newProgressBarText">New progress bar text.</param>
    /// <remarks>The null argument has a special meaning - indicates that current text should not change.</remarks>
    public void SetTextProgressBar(string newProgressBarText)
    {
        if (IsSplashExisting)
        {
            SplashWindow.SetTextProgressBar(newProgressBarText);
        }
    }

    /// <inheritdoc/>
    public void SetTexts(string newStatusText, string newProgressBarText, bool setReference)
    {
        if (IsSplashExisting)
        {
            SplashWindow.SetTexts(newStatusText, newProgressBarText, setReference);
        }
    }

    /// <inheritdoc/>
    public void SetReferencePoint()
    {
        if (IsSplashExisting)
        {
            SplashWindow.SetReferencePoint();
        }
    }

    /// <summary>
    /// Close the existing splash window ( if there is any ).
    /// </summary>
    /// <param name="bStoreIncrements">
    /// If true, closing should store the time increments that will be used on the next splash presentation for the actual progress processing.
    /// You will specify false if closing splash prematurely ( in the case of program initialization error etc.) </param>
    public void CloseSplash(bool bStoreIncrements /*, bool bWaitForClosing */)
    {
        try
        {
            if (IsSplashExisting)
            {
                SplashWindow.CloseSplash(bStoreIncrements /*, bWaitForClosing */);
            }
        }
        catch (NullReferenceException)
        {
        }
        finally
        {
            // we don't need that any more
            Disposer.SafeDispose(ref _splashThread);
        }
    }
    #endregion // ISplash Members

    /// <summary>
    ///  Get the current ISplashWindow ( if there is any )
    /// </summary>
    public ISplashWindow SplashWindow
    {
        get
        {
            lock (SyncRoot)
            {
                ISplashWindow result = null;

                if ((_splashThread != null) && _splashThread.IsAlive)
                {
                    result = _splashThread.SplashWindow;
                }
                return result;
            }
        }
    }

    /// <inheritdoc/>
    public bool IsSplashExisting
    {
        get
        {
            lock (SyncRoot)
            {
                ISplashWindow splash;
                bool bRes = false;

                if (null != (splash = SplashWindow))
                {
                    bRes = !(splash as Form).NullSafe(f => f.IsDisposed);
                }
                return bRes;
            }
        }
    }

    /// <inheritdoc/>
    public bool CreateSplash(int millisecondsTimeout)
    {
        return CreateSplash(millisecondsTimeout, ThreadPriority.Normal, null);
    }

    /// <inheritdoc/>
    public bool CreateSplash(int millisecondsTimeout, ThreadPriority priority, Image backgroundImage)
    {
        return DoCreateSplash(millisecondsTimeout, priority, backgroundImage, false);
    }

    /// <inheritdoc/>
    public bool CreateSplash(int millisecondsTimeout, ThreadPriority priority, string strResourceStreamName, Assembly resourceAssembly)
    {
        Image backgroundImage;

        if (null == (backgroundImage = LoadImage(strResourceStreamName, resourceAssembly)))
        {
            string strErr = string.Format(CultureInfo.InvariantCulture,
              "Failed to load resource stream {0}", strResourceStreamName);
            throw new InvalidOperationException(strErr);
        }
        return DoCreateSplash(millisecondsTimeout, priority, backgroundImage, true);
    }

    #endregion // ISplashFactory Members
}

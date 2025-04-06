/***************************************************************************************************************
*
* FILE NAME:   .\UI\Splash\SplashForm.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class SplashForm
*
**************************************************************************************************************/

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

// Ignore Spelling: Utils, Hiddenlbl
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using PK.PkUtils.SystemEx;

namespace PK.PkUtils.UI.Splash;

//
// Changes from the previous (original) CodeProject code:
//
//  - replacement of the panel working as progress bar (pnlStatus ) by SmoothProgressBar control
//  - definition of interfaces ISplashWindow and ISplashFactory;
//    together with separating their functionality to corresponding implementation classes
//    ( SplashForm and SplashFactory )
//  - new properties IsSplashExisting, IsSplashVisible
//  - using the thread class SplashScreenThread : WorkerThread
//  - thread safety on accessing _alActualTimes
//  - original code of RegistryAccess replaced by a new one using Application.UserAppDataRegistry
//  - thread synchronization with sync. object _sync
//  - added catch (NullReferenceException) in void CloseForm(),
//  - CloseForm renamed to CloseSplash, which has additional boolean argument bStoreIncrements
//  - adding arguments for ThreadPriority
//  - usage of generic List<double> instead of previous ArrayList 
//  - fields _dblOpacityIncrement and _dblOpacityDecrement are now constants,
//    and there is a new variable _dblOpacityStep 

/// <summary>
/// The SplashScreen form that could be displayed on program startup.
/// The source code has the origin in the CodeProject article
/// http://www.codeproject.com/KB/cs/prettygoodsplashscreen.aspx
/// See the file header for the history of changes done.
/// </summary>
public partial class SplashForm : Form, ISplashWindow
{
    #region Typedefs
    // see the SplashForm.Types.cs for nested types definitions
    #endregion // Typedefs

    #region Fields

    #region Protected Fields

    /// <summary>
    /// User interface string destined for the label _lblTimeRemaining.  Should be in resource for localization
    /// </summary>
    protected const string _StrSecondRemains = "1 second remaining";
    /// <summary>
    /// User interface string destined for the label _lblTimeRemaining.  Should be in resource for localization
    /// </summary>
    protected const string _StrSecondsRemain234 = "{0} seconds remaining";
    /// <summary>
    /// User interface string destined for the label _lblTimeRemaining.  Should be in resource for localization
    /// </summary>
    protected const string _StrSecondsRemain = "{0} seconds remaining";
    #endregion // Protected Fields

    #region Private Fields

#if SUPPORT_FADE
    // Fade in and out.
    private const double _dblOpacityIncrement = .05;
    private const double _dblOpacityDecrement = .08;
    private double _dblOpacityStep = _dblOpacityIncrement;
#else
private bool _bShouldClose;
#endif // SUPPORT_FADE
    private const int TIMER_INTERVAL = 50;

    // Status and progress bar
    private string _strStatusText;
    private string _strProgressBarText;
    private bool _bIsHiddenlblStatus;

    private double _dblCompletionFraction;

    // Progress smoothing
    private double _dblLastCompletionFraction;
    private double _dblPBIncrementPerTimerInterval = .015;

    // Self-calibration support
    private bool _bFirstLaunch;
    private DateTime _dtStart;

    // indicates whether internal references were set
    private bool _bDTSet;
    private bool _bIncrementsStored;
    private bool _bDoNotStoreIncerments;
    private int _iIndex = 1;
    private int _nActualTicks;
    private List<double> _alPreviousCompletionFraction;
    private readonly List<double> _alActualTimes = [];
    private readonly object _sync = new();

    // private const string REG_KEY_INITIALIZATION = "Initialization";
    private const string REGVALUE_PB_MILISECOND_INCREMENT = "Increment";
    private const string REGVALUE_PB_PERCENTS = "Percents";
    #endregion // Private Fields
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Argument-less constructor
    /// </summary>
    /// 
    public SplashForm()
      : this(null)
    {
    }

    /// <summary>
    /// Constructor with provided background image
    /// </summary>
    /// <param name="img"></param>
    internal SplashForm(Image img)
      : base()
    {
        InitializeComponent();
        // some texts should be initially empty
        _lblStatus.Text = string.Empty;
        _lblTimeRemaining.Text = string.Empty;
#if SUPPORT_FADE
        this.Opacity = .00;
#endif // SUPPORT_FADE

        // rest of initialization
        if (null != img)
        {
            this.SetBackgroundImage(img);
        }
        _timer.Interval = TIMER_INTERVAL;
        _timer.Start();
    }
    #endregion // Constructor(s)

    #region Properties

    private object SyncObject
    {
        get { return _sync; }
    }
    #endregion // Properties

    #region Methods
    #region Public Methods
    #endregion // Public Methods

    #region Protected methods

    /// <summary>
    /// Returns true if this is the first launching. The criteria is that no self-calibration support data have been found in the registry.
    /// </summary>
    /// <returns>true if this is the first launch of the window; false if not</returns>
    protected bool IsFirstLaunch()
    {
        return _bFirstLaunch;
    }

    /// <summary>
    /// Indicator if visibility of _lblStatus; the value is initialized by auxiliary method UpdateControlsStatus.
    /// </summary>
    /// <returns> Returns true if the label _lblStatus is hidden; false otherwise. </returns>
    protected bool IsHiddenlblStatus()
    {
        return _bIsHiddenlblStatus;
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            lock (SyncObject)
            {
                if (!this.IsDisposed)
                {
                    if (components != null)
                    {
                        components.Dispose();
                        components = null;
                    }
                }
            }
        }
        base.Dispose(disposing);
    }
    #endregion // Protected methods

    #region Private Methods

    /// <summary>
    /// A utility method setting the text of Label _lblTimeRemaining to empty
    /// </summary>
    private void CleanTimeRemainingText()
    {
        if (this.InvokeRequired)
            this.Invoke(new MethodInvoker(CleanTimeRemainingText));
        else
            _lblTimeRemaining.Text = string.Empty;
    }

    /// <summary>
    /// Internal method for setting reference points.
    /// </summary>
    private void SetReferenceInternal()
    {
        lock (SyncObject)
        {
            if (_bDTSet == false)
            {
                _bDTSet = true;
                _dtStart = DateTime.Now;
                ReadIncrements();
            }
            double dblMilliseconds = ElapsedMilliSeconds();
            lock (_alActualTimes)
            {
                _alActualTimes.Add(dblMilliseconds);
                _dblLastCompletionFraction = _dblCompletionFraction;
            }
            if (_alPreviousCompletionFraction != null && _iIndex < _alPreviousCompletionFraction.Count)
                _dblCompletionFraction = _alPreviousCompletionFraction[_iIndex++];
            else
                _dblCompletionFraction = (_iIndex > 0) ? 1 : 0;
        }
    }

    /// <summary>
    /// Utility function to return elapsed mMilliseconds since 
    /// the FormSplash was launched.
    /// </summary>
    /// <returns></returns>
    private double ElapsedMilliSeconds()
    {
        TimeSpan ts = DateTime.Now - _dtStart;
        return ts.TotalMilliseconds;
    }

    /// <summary>
    /// Function to read the checkpoint intervals from the previous invocation of the
    /// splash screen from the registry.
    /// </summary>
    private void ReadIncrements()
    {
        string sPBIncrementPerTimerInterval = RegistryAccess.GetStringRegistryValue(REGVALUE_PB_MILISECOND_INCREMENT, "0.0015");
        bool bParsedOk = false;

        if (double.TryParse(sPBIncrementPerTimerInterval, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double dblResult))
        {
            bParsedOk = (!double.IsInfinity(dblResult) && (dblResult > 0));
        }
        if (bParsedOk)
            _dblPBIncrementPerTimerInterval = dblResult;
        else
            _dblPBIncrementPerTimerInterval = .0015;

        string sPBPreviousPctComplete = RegistryAccess.GetStringRegistryValue(REGVALUE_PB_PERCENTS, string.Empty);
        sPBPreviousPctComplete = sPBPreviousPctComplete.Trim();
        _bFirstLaunch = string.IsNullOrEmpty(sPBPreviousPctComplete);

        if (!this.IsFirstLaunch())
        {
            string[] aTimes = sPBPreviousPctComplete.Split(null);
            _alPreviousCompletionFraction = [];

            for (int i = 0; i < aTimes.Length; i++)
            {
                if (double.TryParse(aTimes[i], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double dblVal))
                    _alPreviousCompletionFraction.Add(dblVal);
                else
                    _alPreviousCompletionFraction.Add(1.0);
            }
        }
        else
        {
            CleanTimeRemainingText();
        }
    }

    /// <summary>
    /// Method to store the intervals (in percent complete) from the current invocation of
    /// the splash screen to the registry.
    /// </summary>
    private void StoreIncrements()
    {
        if ((!_bIncrementsStored) && (!_bDoNotStoreIncerments))
        {
            _bIncrementsStored = DoStoreIncrements();
        }
    }

    /// <summary>
    /// Implementation helper actually storing the increments data
    /// </summary>
    private bool DoStoreIncrements()
    {
        bool bRes;

        lock (SyncObject)
        {
            int nCount;
            StringBuilder sbPercent = new();
            double dblElapsedMilliseconds = ElapsedMilliSeconds();

            lock (_alActualTimes)
            {
                if (bRes = (0 < (nCount = _alActualTimes.Count)))
                {
                    for (int ii = 0; ;)
                    {
                        double dPercent = ((double)_alActualTimes[ii] / dblElapsedMilliseconds);
                        string strAddPercent = dPercent.ToString("0.####", NumberFormatInfo.InvariantInfo);
                        sbPercent.Append(strAddPercent);
                        if (++ii < nCount)
                            sbPercent.Append(' ');
                        else
                            break;
                    }
                }
            }

            if (bRes)
            {
                RegistryAccess.SetStringRegistryValue(REGVALUE_PB_PERCENTS, sbPercent.ToString());
                _dblPBIncrementPerTimerInterval = 1.0 / _nActualTicks;
                RegistryAccess.SetStringRegistryValue(REGVALUE_PB_MILISECOND_INCREMENT, _dblPBIncrementPerTimerInterval.ToString("#.000000", NumberFormatInfo.InvariantInfo));
            }
        }
        return bRes;
    }

    /// <summary>
    /// Thread-safe form shutdown
    /// </summary>
    private void CloseMe()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new MethodInvoker(CloseMe));
        }
        else
        {
            this._timer.Stop();
            this.StoreIncrements();
            this.Close();
        }
    }

    /// <summary>
    /// In case of the first launch, hides the progress bar 
    /// and label _lblTimeRemaining (for time remaining).
    /// </summary>
    private void UpdateControlsStatus()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new MethodInvoker(UpdateControlsStatus));
        }
        else
        {
            _bIsHiddenlblStatus = !_lblStatus.Visible;

            if (IsFirstLaunch())
            {
                _smoothProgressBar.Hide();
                _lblTimeRemaining.Hide();
            }
        }
    }

    private bool SetBackgroundImage(Image backgroundImage)
    {
        bool bRes = false;
        if (null != backgroundImage)
        {
            this.BackgroundImage = backgroundImage;
            this.ClientSize = backgroundImage.Size;
            bRes = true;
        }
        return bRes;
    }

    private void UpdateLblStatusText()
    {
        if (this._bIsHiddenlblStatus)
        {
            _lblStatus.Text = string.Empty;
        }
        else if (null != _strStatusText)
        {
            _lblStatus.Text = _strStatusText;
        }
    }

    private void UpdateProgressBarText()
    {
        if (!_smoothProgressBar.Visible)
        {
            _smoothProgressBar.BarText = string.Empty;
        }
        else if (null != _strProgressBarText)
        {
            _smoothProgressBar.BarText = _strProgressBarText;
        }
    }

    private void UpdateTimeRemainingText(string strVal)
    {
        Debug.Assert(!InvokeRequired);
        if (!_lblTimeRemaining.Visible)
            _lblTimeRemaining.Text = string.Empty;
        else
            _lblTimeRemaining.Text = strVal;
    }
    #endregion // Private Methods
    #endregion // Methods

    #region Event handlers

    // Tick Event handler for the Timer control.  Handle fade in and fade out.  Also
    // handle the smoothed progress bar.
    private void OnTimer_Tick(object sender, EventArgs e)
    {
        try
        {
            UpdateLblStatusText();
            UpdateProgressBarText();

#if SUPPORT_FADE
            if (_dblOpacityStep > 0)
            {
                _nActualTicks++;
                if (this.Opacity < 1)
                {
                    this.Opacity += _dblOpacityStep;
                }
            }
            else
            {
                if (this.Opacity > 0)
                {
                    this.Opacity += _dblOpacityStep;
                    if (this.IsFirstLaunch())
                    {
                        // in that case store increments asap - for some reason the form is occasionally closed 
                        // before CloseMe has a chance to be called
                        this.StoreIncrements();
                    }
                }
                else
                {
                    CloseMe();
                    return;
                }
            }
#else // SUPPORT_FADE
            if (!_bShouldClose)
            {
              _nActualTicks++;
            }
            else
            {
              StoreIncrements();
              CloseMe();
            }
#endif // SUPPORT_FADE

            if (IsFirstLaunch())
            {
                UpdateControlsStatus();
            }
            else if (_dblLastCompletionFraction < _dblCompletionFraction)
            {
                _dblLastCompletionFraction += _dblPBIncrementPerTimerInterval;

                if (_smoothProgressBar.Handle != IntPtr.Zero)
                {
                    _smoothProgressBar.Value = (int)(100 * _dblLastCompletionFraction);
                }
                if (_lblTimeRemaining.Handle != IntPtr.Zero)
                {
                    string strVal = string.Empty;

                    if (_lblTimeRemaining.Visible)
                    {
                        double dInterval = (TIMER_INTERVAL * ((1.0 - _dblLastCompletionFraction) / _dblPBIncrementPerTimerInterval)) / 1000;
                        double dInter = Math.Ceiling(dInterval + 0.25); // add 0.25 sec for sure
                        int iSecondsLeft = (int)dInter;

                        strVal = iSecondsLeft switch
                        {
                            1 => string.Format(CultureInfo.CurrentCulture, _StrSecondRemains),
                            2 or 3 or 4 => string.Format(CultureInfo.CurrentCulture, _StrSecondsRemain234, iSecondsLeft),
                            _ => string.Format(CultureInfo.CurrentCulture, _StrSecondsRemain, iSecondsLeft)
                        };
                    }
                    UpdateTimeRemainingText(strVal);
                }
            }
        }
        catch (ObjectDisposedException /* ex */)
        {  // Silly, but i couldn't find better way to reliably get rid of this exception...
        }
    }

    /// <summary>
    /// The double-click handler. Close the form if they double click on it.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SplashScreen_DoubleClick(object sender, System.EventArgs e)
    {
        CloseSplash(false);
    }
    #endregion // Event handlers

    #region IDisposableEx Members
    #region IDisposable Members
    /*  void Dispose(); - not needed, is implemented by the Form itself
    */
    #endregion // IDisposable Members
    /* bool IsDisposed { get; } - not needed implemented by the base class
    */
    #endregion // IDisposableEx Members

    #region ISplashWindow Members

    /// <summary>
    /// Implements ISplashWindow.IsSplashVisible. Returns true if the window is visible.
    /// </summary>
    public bool IsSplashVisible
    {
        get { return this.Visible; }
    }

    /// <summary>
    /// Set the status text and update the reference point.
    /// The null argument has a special meaning - indicates that current text should not change.
    /// </summary>
    /// <param name="newStatusText">The new text that will be displayed by _lblStatus.</param>
    public void SetTextStatus(string newStatusText)
    {
        SetTexts(newStatusText, null, true);
    }

    /// <summary>
    /// Sets the progress bar text, and updates the reference point
    /// The null argument has a special meaning - indicates that current text should not change.
    /// </summary>
    /// <param name="newProgressBarText">The new text that will be displayed by _smoothProgressBar</param>
    public void SetTextProgressBar(string newProgressBarText)
    {
        SetTexts(null, newProgressBarText, true);
    }

    /// <summary>
    /// Sets the status text and progress bar text, optionally updating the reference point list
    /// ( based on the value of setReference argument ).
    /// This boolean argument is useful if you are in a section of code that has a variable set 
    /// (variable amount) of status string updates, depending on the actual runtime flow.
    /// In that case, don't set the reference point for such status update.
    /// </summary>
    /// <param name="newStatusText">The new text that will be displayed by _lblStatus.</param>
    /// <param name="newProgressBarText">The new text that will be displayed by _smoothProgressBar</param>
    /// <param name="setReference">If true, the internal method method for setting new reference point will be called.</param>
    /// <remarks> The null argument(s) value of newStatusText or newProgressBarText 
    /// has a special meaning - indicates that current text should not change.
    /// Regarding the exact sense of reference point creation, see the remarks for SetReferencePoint method.
    /// </remarks>
    public void SetTexts(string newStatusText, string newProgressBarText, bool setReference)
    {
        _strStatusText = newStatusText;
        _strProgressBarText = newProgressBarText;
        if (setReference)
        {
            this.SetReferenceInternal();
        }
    }

    /// <summary>
    /// A method called several times from the initializing application 
    /// to give the splash screen reference points.
    /// </summary>
    /// <remarks>
    /// SetStatus() and SetReferencePoint() both call SetReferenceInternal(),
    /// which records the time of the first call and adds the elapsed time of each subsequent call 
    /// to the value list for later processing. 
    /// It sets the progress bar values by referencing previous recorded values for the progress bar. 
    /// For example, if we're processing the 3rd SetReferencePoint() call, we use the actual percentage 
    /// of the overall load time that occurred between the 3rd and 4th calls during the previous invocation.
    /// </remarks>
    public void SetReferencePoint()
    {
        this.SetReferenceInternal();
    }

    /// <summary>
    /// Close an existing Splash window ( form ). <br/>
    /// If <paramref name="bStoreIncrements"/> is true, this code will cal private method StoreIncrements
    ///  that stores to the registry the intervals (in percent complete) from the current invocation of the splash screen.
    /// </summary>
    /// <param name="bStoreIncrements"> If true, will be called StoreIncrements</param>
    public void CloseSplash(bool bStoreIncrements /*, bool bWaitForClosing */)
    {
        if (bStoreIncrements)
            this.StoreIncrements();
        else
            this._bDoNotStoreIncerments = true;
#if SUPPORT_FADE
        // Make it start going away.
        this._dblOpacityStep = -_dblOpacityDecrement;
#else
  this._bShouldClose = true;
#endif
    }
    #endregion // ISplashWindow Members
}
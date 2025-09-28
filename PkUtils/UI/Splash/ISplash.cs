/***************************************************************************************************************
*
* FILE NAME:   .\UI\Splash\ISplash.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of interface ISplash, ISplashWindow and ISplashFactory 
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//
using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.UI.Splash;

/// <summary>
/// Interface containing the basic splash functionality, common for both ISplashWindow and ISplashFactory.
/// </summary>
[CLSCompliant(true)]
public interface ISplash : IDisposableEx
{
    /// <summary>
    /// Is there visible splash window?
    /// </summary>
    bool IsSplashVisible { get; }

    /// <summary>
    /// Set (change) the status label text, and set new reference point.
    /// </summary>
    /// <param name="newStatusText">New status text.</param>
    /// <remarks>The null argument has a special meaning - indicates that current text should not change.</remarks>
    void SetTextStatus(string newStatusText);

    /// <summary>
    /// Set (change) the progress bar text, and set new reference point.
    /// </summary>
    /// <param name="newProgressBarText">New progress bar text.</param>
    /// <remarks>The null argument has a special meaning - indicates that current text should not change.</remarks>
    void SetTextProgressBar(string newProgressBarText);

    /// <summary>
    /// Sets the status text and progress bar text, optionally updating the reference point list
    /// ( based on the value of setReference argument ).
    /// This boolean argument is useful if you are in a section of code that has a variable set 
    /// (variable amount) of status string updates, depending on the actual runtime flow.
    /// In that case, don't set the reference point for such status update.
    /// </summary>
    /// <param name="newStatusText">New status text.</param>
    /// <param name="newProgressBarText">New progress bar text.</param>
    /// <param name="setReference">If true, the internal method method for setting new reference point will be called.</param>
    /// <remarks>
    /// For both newStatusText and newProgressBarText, the null argument has a special meaning - 
    /// - it indicates that current text should not change.
    /// </remarks>
    void SetTexts(string newStatusText, string newProgressBarText, bool setReference);

    /// <summary>
    /// Set a new reference point. Array of reference points will be stored on closing 
    /// and reloaded later on the next splash presentation.
    /// </summary>
    void SetReferencePoint();

    /// <summary>
    /// Close the existing splash window ( if there is any ).
    /// </summary>
    /// <param name="bStoreIncrements">
    /// If true, closing should store the time increments that will be used on the next splash presentation for the actual progress processing.
    /// You will specify false if closing splash prematurely ( in the case of program initialization error etc.) </param>
    void CloseSplash(bool bStoreIncrements /* , bool bWaitForClosing */);
}

/// <summary>
/// The actual splash window interface, deriving from ISplash and IWin32Window
/// </summary>
[CLSCompliant(true)]
public interface ISplashWindow : ISplash, IWin32Window
{
}

/// <summary>
/// Contains functionality needed for ISplashWindow creation and managing.
/// </summary>
[CLSCompliant(true)]
public interface ISplashFactory : ISplash
{
    #region Properties

    /// <summary> 
    /// Get the current ISplashWindow ( if there is any ) 
    /// </summary>
    ISplashWindow SplashWindow { get; }

    /// <summary> 
    /// Returns true if there is any non-disposed splash window managed by this factory, false otherwise
    /// </summary>
    bool IsSplashExisting { get; }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// The splash creation with minimal amount of arguments.
    /// Creates the splash window on a new UI thread, with default thread priority and no background image.
    /// If non-zero timeout is specified, the method blocks till either the splash widow is visible
    /// or the timeout expires, whichever occurs first. 
    /// If the value Timeout.Infinite is used, the method just waits till the splash widow is visible.
    /// </summary>
    /// <param name="millisecondsTimeout">The time-out in milliseconds</param>
    /// <returns>Returns true if there was no splash and the method has created a new one, false otherwise.</returns>
    bool CreateSplash(int millisecondsTimeout);

    /// <summary>
    /// Create the splash window on a new UI thread, given a thread priority and window background image.
    /// </summary>
    /// <param name="millisecondsTimeout">The time-out in milliseconds</param>
    /// <param name="priority">Priority of the worker thread that will be created</param>
    /// <param name="backgroundImage">The background image that should be used for a splash form</param>
    /// <returns>Returns true if there was no splash and the method has created a new one, false otherwise.</returns>
    bool CreateSplash(int millisecondsTimeout, ThreadPriority priority, Image backgroundImage);

    /// <summary>
    /// Create the splash window on a new UI thread, given a thread priority 
    /// and the location of background image resource ( specified by a resource name and the assembly ).
    /// </summary>
    /// <param name="millisecondsTimeout">The time-out in milliseconds</param>
    /// <param name="priority">Priority of the worker thread that will be created</param>
    /// <param name="strResourceStreamName">The name of resource stream containing the image.</param>
    /// <param name="resourceAssembly">The assembly containing specified resource</param>
    /// <returns>Returns true if there was no splash and the method has created a new one, false otherwise.</returns>
    bool CreateSplash(int millisecondsTimeout, ThreadPriority priority, string strResourceStreamName, Assembly resourceAssembly);

    #endregion // Methods
}

/// <summary>
/// A class containing extension methods for ISplash, which I do not want to include in ISplash itself.
/// </summary>
public static class SplashExtension
{
    /// <summary>
    /// Sets the status text and progress bar text, at the same time updating the reference point list.
    /// </summary>
    /// <param name="s">Any object supporting <see cref="ISplash"/> interface.</param>
    /// <param name="newStatusText">New status text.</param>
    /// <param name="newProgressBarText">New progress bar text.</param>
    public static void SetTextsAndReference(this ISplash s, string newStatusText, string newProgressBarText)
    {
        s.CheckNotDisposed();
        s.SetTexts(newStatusText, newProgressBarText, true);
    }

    /// <summary>
    /// Sets the status text and progress bar text, without updating the reference point list.
    /// </summary>
    /// <param name="s">Any object supporting <see cref="ISplash"/> interface.</param>
    /// <param name="newStatusText">New status text.</param>
    /// <param name="newProgressBarText">New progress bar text.</param>
    public static void SetTextsNoReference(this ISplash s, string newStatusText, string newProgressBarText)
    {
        s.CheckNotDisposed();
        s.SetTexts(newStatusText, newProgressBarText, false);
    }
}

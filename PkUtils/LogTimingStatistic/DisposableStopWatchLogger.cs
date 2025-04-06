// Ignore Spelling: Utils
//

using System;
using System.Diagnostics;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.LogTimingStatistic;

/// <summary> A disposable stop watch logger. </summary>
public class DisposableStopWatchLogger : IDisposableEx
{
    private Stopwatch _sw = Stopwatch.StartNew();
    private readonly IDumper _dumper;
    private readonly string _actionName;
    private readonly string _actionDetails;

    /// <summary> Constructor. </summary>
    ///
    /// <param name="dumper"> The dumper. Can't be null. </param>
    /// <param name="actionName"> Name of the action. Can't be null or empty. </param>
    /// <param name="actionDetails">  The action details. Can't be null or empty. </param>
    public DisposableStopWatchLogger(IDumper dumper, string actionName, string actionDetails)
    {
        _dumper = dumper ?? throw new ArgumentNullException(nameof(dumper));
        _actionName = actionName.CheckArgNotNullOrEmpty(nameof(actionName));
        _actionDetails = actionDetails.CheckArgNotNullOrEmpty(nameof(actionDetails));

        DumpTimingBeginning();
    }

    #region Public Methods

    /// <summary> Gets the name of the action. </summary>
    public string ActionName { get => _actionName; }

    /// <summary> Gets the action details. </summary>
    public string ActionDetails { get => _actionDetails; }

    /// <summary> Gets a value indicating whether the stopwatch running. </summary>
    public bool IsRunning { get => (!IsDisposed) && _sw.IsRunning; }

    /// <inheritdoc/>
    public bool IsDisposed { get => _sw == null; }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion // Public Methods

    #region Protected Methods

    /// <summary> Gets the time elapsed so far. </summary>
    protected TimeSpan Elapsed
    {
        get { this.CheckNotDisposed(); return _sw.Elapsed; }
    }

    /// <summary>
    /// Stops the stopwatch if running. If this logger inherits, Stop should be called from inherited class 
    /// Dispose, so the time does not run between Inherited.Dispose and base.Dispose,
    /// thus returning different 'Elapsed' values.
    /// </summary>
    protected void Stop()
    {
        if (IsRunning) _sw.Stop();
    }


    /// <summary> Returns time elapsed converted to string. </summary>
    protected virtual string ElapsedMessagePart()
    {
        // For real usage, make back to:
        // string result = Elapsed.ToString("hh\\:mm\\:ss\\:fff");

        string result = Elapsed.ToReadableString(includeMilliseconds: true);
        return result;
    }

    /// <summary> Dumps the timing beginning. </summary>
    protected virtual void DumpTimingBeginning()
    {
        // It's better having the beginning and ending part aligned in the log;
        // so use the words of the same length ( like now  'start' and 'ended' ).
        _dumper.Dump($"{ActionName}: start {ActionDetails}");
    }

    /// <summary> Dumps the timing beginning. </summary>
    protected virtual void DumpTimingEnd()
    {
        _dumper.Dump($"{ActionName}: ended {ActionDetails}, took {ElapsedMessagePart()}");
    }

    /// <summary> Cleanup any resources being used. </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
                Stop();
                DumpTimingEnd();
            }
            _sw = null;
        }
    }
    #endregion // Protected Methods
}

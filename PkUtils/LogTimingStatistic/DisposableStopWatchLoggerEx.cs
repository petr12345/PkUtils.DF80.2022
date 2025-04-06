// Ignore Spelling: Utils
// 
using System;
using System.Diagnostics;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.LogTimingStatistic;

/// <summary> A disposable stop watch logger. </summary>
public class DisposableStopWatchLoggerEx : DisposableStopWatchLogger
{
    private readonly ITimingScope _scope;

    /// <summary> Constructor. </summary>
    ///
    /// <param name="dumper"> The dumper. Can't be null. </param>
    /// <param name="actionName"> Name of the action. Can't be null or empty. </param>
    /// <param name="actionDetails"> The action details. Can't be null or empty. </param>
    /// <param name="scope"> The scope. </param>
    public DisposableStopWatchLoggerEx(
        IDumper dumper,
        string actionName,
        string actionDetails,
        ITimingScope scope) : base(dumper, actionName, actionDetails)
    {

        ArgumentNullException.ThrowIfNull(scope);
        _scope = scope;
    }

    /// <summary> Cleanup any resources being used. </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
    protected override void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
                Stop();
                _scope.AddOccurrence(ActionName, Elapsed);
            }

            base.Dispose(disposing);
            Debug.Assert(this.IsDisposed, "should be disposed now");
        }
    }

}

using System.Diagnostics;

namespace PK.PkUtils.Diagnostics;

/// <summary>   A stack trace wrapper, used for diagnostics purposes. </summary>
public static class StackTraceWrapper
{
    /// <summary>
    /// Returns a string representation of the current stack trace, skipping the specified number of frames.
    /// </summary>
    /// <param name="skipFrames">The number of stack frames to skip from the top of the stack trace.</param>
    /// <returns>A string representation of the stack trace.</returns>
    /// <remarks>
    /// To exclude the call to <c>GetStackTraceString</c> itself from the stack trace, set <paramref name="skipFrames"/> to at least 1.
    /// If you want to skip additional frames (such as helper methods), increase the value accordingly.
    /// </remarks>
    public static string GetStackTraceString(int skipFrames = 1)
    {
        StackTrace stackTrace = new(skipFrames, fNeedFileInfo: true);
        string result = stackTrace.ToString();

        return result;
    }
}

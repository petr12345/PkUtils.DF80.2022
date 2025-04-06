using System;
using PK.Commands.CommandProcessing;
using PK.PkUtils.Interfaces;

namespace GZipTest.Infrastructure;

/// <summary> A utility class implementing extensions of IComplexResult{ExitCode}. </summary>
public static class CommandResultExtensions
{
    /// <summary>
    /// An extension method which checks Check if <paramref name="result"/> represents success, and wrapped
    /// result is ExitCode.Success.
    /// </summary>
    ///
    /// <param name="result">   The checked result. Can't be null. </param>
    ///
    /// <returns>   True if it quite succeeds, false if it fails. </returns>

    public static bool FullSuccess(this IComplexResult<ExitCode> result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.Success && result.Content.IsOK();
    }
}

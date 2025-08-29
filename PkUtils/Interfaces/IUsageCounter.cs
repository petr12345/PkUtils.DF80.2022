// Ignore Spelling: Utils, reentrancy

using System;

namespace PK.PkUtils.Interfaces;

/// <summary>
/// An interface representing basic usage counter. 
/// Implementation of usage counter changes must be atomic (thread - safe ).
/// </summary>
/// <remaks>
/// Rather than addressing thread-safety (concurrency) and thread locks, 
/// the code using a class implementing IUsageCounter might intend to protect itself against reentrancy issues.
/// After the first usage is acquired, it is assumed any subsequent usages on any thread 
/// still succeed as well. Hence, it is the duty of the calling code to avoid such reentrancy.
/// </remaks>
[CLSCompliant(true)]
public interface IUsageCounter
{
    /// <summary> Is used at all (once or more times)? </summary>
    bool IsUsed { get; }

    /// <summary> Add new usage please. </summary>
    /// <returns> The amount of total usages acquired. </returns>
    int AddReference();

    /// <summary> Release usage please. </summary>
    /// <returns>   The amount of remaining references. </returns>
    int Release();
}

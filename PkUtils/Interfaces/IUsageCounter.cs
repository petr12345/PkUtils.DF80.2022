/***************************************************************************************************************
*
* FILE NAME:   .\Interfaces\IUsageCounter.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of interface IUsageCounter
*
**************************************************************************************************************/


// Ignore Spelling: Utils

using System;

namespace PK.PkUtils.Interfaces;

/// <summary>
/// An interface representing basic usage counter.
/// Rather than addressing thread-safety (concurrency) and thread locks, the class implementing IUsageCounter
/// intends to protect against reentrancy issues. 
/// After the first usage is acquired, it is assumed any subsequent usages on any thread 
/// still succeed as well. Hence, it is the duty of the calling code to avoid reentrancy.
/// </summary>
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

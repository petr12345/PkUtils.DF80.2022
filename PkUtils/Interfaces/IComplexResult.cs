// Ignore Spelling: Utils
//
using System;

namespace PK.PkUtils.Interfaces;

/// <summary> Encapsulates a result of client method call. </summary>
[CLSCompliant(true)]
public interface IComplexResult
{
    /// <summary> Gets a value indicating whether there was success or failure. </summary>
    bool Success { get; }

    /// <summary> Gets a message describing the error ( if any ). </summary>
    string ErrorMessage { get; }

    /// <summary> Gets details of the error ( if any ). </summary>
    object ErrorDetails { get; }
}

/// <summary> Interface for result of client method having a content of <typeparamref name="T"/>. </summary>
///
/// <typeparam name="T">  Type of the content. </typeparam>
/// 
/// <remarks>
/// The interface is declared covariant, to preserve assignment compatibility,
/// i.e. to permit compilation of following assignment:
/// <code>
/// <![CDATA[
///   IComplexResult<string> expr1 = SomeMethod();
///   // logically possible as string derives from object, but the compiler needs covariance to permit it:
///   IComplexResult<object> expr2 = expr1;
/// ]]>
/// </code>
/// </remarks>
[CLSCompliant(true)]
public interface IComplexResult<out T> : IComplexResult
{
    /// <summary> Gets the content of response. </summary>
    T Content { get; }
}

/// <summary> Static class that implements extension methods if IComplexResult. </summary>
[CLSCompliant(true)]
public static class ResultExtension
{
    /// <summary> An IComplexResult extension method that query if 'iResult' has failed. </summary>
    /// <param name="iResult"> The IComplexResult to act on. Can't be null. </param>
    /// <returns>   True if iResult.Success is false, false if iResult.Success is true. </returns>
    public static bool Failed(this IComplexResult iResult)
    {
        return !iResult.Success;
    }

    /// <summary> An IComplexResult extension method that returns the exception caught ( if any). </summary>
    /// <param name="iResult">  The IComplexResult to act on. Can't be null. </param>
    /// <returns> An Exception. </returns>
    /// <remarks> Note, ExceptionCaught is intentionally designed to return null 
    ///           when ErrorDetails is not null but it is not an Exception.</remarks>
    public static Exception ExceptionCaught(this IComplexResult iResult)
    {
        return iResult.ErrorDetails as Exception;
    }

}

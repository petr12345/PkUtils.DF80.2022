// Ignore Spelling: Utils
//
using System;


namespace PK.PkUtils.Interfaces;

/// <summary> 
/// Encapsulates a result of client method call, with customizable error type. 
/// </summary>
/// <typeparam name="TError">Type of error details.</typeparam>
/// <remarks>
/// The interface is declared covariant for <c>TError</c>, to preserve assignment compatibility
/// for derived error types. For example:
/// <code>
/// <![CDATA[
///   IComplexErrorResult<string> errorResult1 = SomeMethod();
///   // Because string derives from object and TError is covariant, this assignment is valid:
///   IComplexErrorResult<object> errorResult2 = errorResult1;
/// ]]>
/// </code>
/// </remarks>
[CLSCompliant(true)]
public interface IComplexErrorResult<out TError>
{
    /// <summary> Gets a value indicating whether there was success or failure. </summary>
    bool Success { get; }

    /// <summary> Gets a message describing the error (if any). </summary>
    string ErrorMessage { get; }

    /// <summary> Gets details of the error (if any). </summary>
    TError ErrorDetails { get; }
}

/// <summary> Encapsulates a result with customizable content and error type. </summary>
/// <typeparam name="T">Type of the content.</typeparam>
/// <typeparam name="TError">Type of error details.</typeparam>
/// <remarks>
/// The interface is declared covariant for <c>T</c>, allowing assignment compatibility for derived types.
/// For example:
/// <code>
/// <![CDATA[
///   IComplexErrorResult<string, Exception> result1 = SomeMethod();
///   // Because string derives from object and T is covariant, this assignment is valid:
///   IComplexErrorResult<object, Exception> result2 = result1;
/// ]]>
/// </code>
/// </remarks>
public interface IComplexErrorResult<out T, out TError> : IComplexErrorResult<TError>
{
    /// <summary> Gets the content. </summary>
    T Content { get; }
}

/// <summary> Static class that implements extension methods for IComplexErrorResult. </summary>
public static class ComplexErrorResultExtensions
{
    /// <summary> Extension method that queries if 'result' has failed. </summary>
    /// <typeparam name="TError">Type of error details.</typeparam>
    /// <param name="result">The instance to act on. Can't be null.</param>
    /// <returns>True if result.Success is false, otherwise false.</returns>
    public static bool Failed<TError>(this IComplexErrorResult<TError> result)
    {
        return !result.Success;
    }

    /// <summary> Extension method that returns the exception caught (if any). </summary>
    /// <typeparam name="TError">Type of error details.</typeparam>
    /// <param name="result">The instance to act on. Can't be null.</param>
    /// <returns> The Exception instance if ErrorDetails is an Exception; otherwise, null. </returns>
    /// <remarks>
    /// ExceptionCaught is intentionally designed to return null 
    /// when ErrorDetails is not null but it is not an Exception.
    /// </remarks>
    public static Exception ExceptionCaught<TError>(this IComplexErrorResult<TError> result)
    {
        return result.ErrorDetails as Exception;
    }
}

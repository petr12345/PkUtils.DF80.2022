// Ignore Spelling: Utils, rhs, Ctrl
//
using System;
using System.Diagnostics;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.DataStructures;

/// <summary>
/// Generic ComplexResult is an immutable object representing either a valid returned value, or a failed
/// state of the call. Implements a <see cref="IComplexResult"/> with a content.
/// </summary>
/// <typeparam name="T"> Type of the content. </typeparam>
[CLSCompliant(true)]
public class ComplexResult<T> : ComplexResult, IComplexResult<T>
{
    #region Fields

    private readonly T _content;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Constructor that initializes this instance to failure state, with provided error message. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="errorMessage"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when a supplied <paramref name="errorMessage"/> is empty. </exception>
    ///
    /// <param name="errorMessage"> Message describing the error. Can't be null or empty. </param>
    /// <param name="errorDetails"> (Optional) The error details, if available. </param>
    public ComplexResult(string errorMessage, object errorDetails = null)
        : base(errorMessage, errorDetails)
    { }

    /// <summary> Constructor that initializes this instance to failure state, with provided exception. </summary>
    ///
    /// <exception cref="ArgumentNullException"> 
    /// Thrown when required argument <paramref name="exceptionCaught"/> is null. 
    /// </exception>
    ///
    /// <param name="exceptionCaught"> The exception caught by caller's code. Can't be null. </param>
    public ComplexResult(Exception exceptionCaught)
        : base(exceptionCaught)
    { }

    /// <summary> Constructor that initializes this instance to failure state, with provided non-generic <paramref name="rhs"/>. </summary>
    /// <remarks>
    /// Note we assume that <paramref name="rhs"/> is in failure state, since we don't provide any content-argument here.
    /// </remarks>
    /// 
    /// <exception cref="ArgumentException"> Thrown when argument <paramref name="rhs"/> is not in failed state. </exception>
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="rhs"/>
    /// is null. </exception>
    ///
    /// <param name="rhs"> The other (non-generic) client result. Can't be null. </param>
    public ComplexResult(IComplexResult rhs)
        : base(rhs)
    {
        CheckIsFailed(rhs);
        Debug.Assert(!Success);
    }

    /// <summary> Constructor that initializes this instance to state of <paramref name="rhs"/>, including content. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="rhs"/> is null. </exception>
    /// <param name="rhs"> The other (non-generic) client result. Can't be null. </param>
    public ComplexResult(IComplexResult<T> rhs)
        : base(rhs)
    {
        if (rhs.Success)
        {
            _content = rhs.Content;
        }
    }

    /// <summary> Constructor that initializes this instance to successful state, with no error message. </summary>
    /// <remarks>
    /// Note that any value of T ( including null value ) is considered a valid value here.
    /// If you require a different behaviour, you should derive your own class for that.
    /// </remarks>
    ///
    /// <param name="content"> The content. </param>
    public ComplexResult(T content)
    {
        _content = content;
    }
    #endregion // Constructor(s)

    #region IComplexResult<T> members

    /// <inheritdoc/>
    public T Content => _content;
    #endregion // IComplexResult<T> members

    #region Methods

    /// <summary>   Returns a string that represents the current object. </summary>
    /// <returns>   A string that represents the current object. </returns>
    public override string ToString()
    {
        return Success ? $"Success: {Content}" : base.ToString();
    }

    /// <summary> Static factory-like method, that creates a "successful" instance of ComplexResult{T}. </summary>
    /// <remarks> May be needed in case the typeof(T) is string, to prevent ambiguity which of constructors
    ///           ComplexResult(string errorMessage, ...) and ComplexResult(T content) is actually called.
    /// </remarks>
    ///
    /// <param name="content"> The content to be assigned. </param>
    ///
    /// <returns> The new instance. </returns>
    public static IComplexResult<T> CreateSuccessful(T content)
    {
        ComplexResult<T> result = new(content);

        Debug.Assert(result.Success);
        return result;
    }

    /// <summary> Static factory-like method, that creates a "failed" instance of ComplexResult{T}. </summary>
    ///
    /// <remarks>
    /// May be needed in case the typeof(T) is string, to prevent ambiguity which of constructors
    /// ComplexResult(string errorMessage, ...) and ComplexResult(T content) is actually called.
    /// </remarks>
    ///
    /// <param name="errorMessage"> Message describing the error. Can't be null or empty. </param>
    /// <param name="errorDetails"> (Optional) The error details, if available. </param>
    ///
    /// <returns>   The new "failed" instance. </returns>
    public static new IComplexResult<T> CreateFailed(string errorMessage, object errorDetails = null)
    {
        ComplexResult<T> result = new(errorMessage, errorDetails);

        Debug.Assert(!result.Success);
        return result;
    }

    /// <summary> Static factory-like method, creates a "failed" instance of ComplexResult{T}. </summary>
    ///
    /// <param name="exceptionCaught"> The exception caught by caller's code. Can't be null. </param>
    /// <returns> The new "failed" instance. </returns>
    public static new IComplexResult<T> CreateFailed(Exception exceptionCaught)
    {
        ComplexResult<T> result = new(exceptionCaught);

        Debug.Assert(!result.Success);
        return result;
    }

    /// <summary>
    /// Static factory-like method, creates a "failed" instance of ComplexResult{T}, based on provided other
    /// <paramref name="rhs"/> which failed originally.
    /// </summary>
    /// <exception cref="ArgumentException"> Thrown when argument <paramref name="rhs"/> is not in failed state. </exception>
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="rhs"/>
    /// is null. </exception>
    /// <param name="rhs"> The other client result. Can't be null. </param>
    /// <returns>   The new failed ComplexResult instance. </returns>
    public static new IComplexResult<T> CreateFailed(IComplexResult rhs)
    {
        CheckIsFailed(rhs);

        ComplexResult<T> result = new(rhs);
        Debug.Assert(result.Failed());

        return result;
    }
    #endregion // Methods
}

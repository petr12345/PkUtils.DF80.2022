// Ignore Spelling: Utils, rhs
//
using System;
using System.Diagnostics;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;


namespace PK.PkUtils.DataStructures;

/// <summary>
/// Implements <see cref="IComplexResult"/>, representing a 'void call result'.
/// </summary>
/// <remarks>
/// ComplexResult is an immutable object representing either a successful or a failed state.
///
/// - When created in a successful state, the instance contains no error message and no error details.
/// - When created in a failed state, you can initialize it in several ways:
///   - With a raw error message and optional error details.
///   - With an exception, which will be used to compose the error message.
///   - By copying another failed <see cref="IComplexResult"/> instance.
///
/// Factory methods are provided to simplify creation of both success and failure instances.
/// </remarks>
public class ComplexResult : ComplexErrorResult<object>, IComplexResult
{
    #region Fields

    private static readonly Lazy<IComplexResult> _ok = new(() => new ComplexResult());
    #endregion // Fields

    #region Constructors

    /// <summary> Default constructor. Initializes to success state, with no error message. </summary>
    public ComplexResult() : base() { }

    /// <summary> Constructor that initializes to failure state, with provided error message. </summary>
    /// <param name="errorMessage"> Message describing the error. Can't be null or empty. </param>
    /// <param name="errorDetails"> (Optional) Error details, if available. </param>
    public ComplexResult(string errorMessage, object errorDetails = null)
        : base(errorMessage, errorDetails)
    { }

    /// <summary> Constructor that initializes to failure state, with provided exception. </summary>
    /// <param name="exceptionCaught"> The exception caught by caller's code. Can't be null. </param>
    public ComplexResult(Exception exceptionCaught) : base(exceptionCaught)
    { }

    /// <summary> Constructor that initializes by copying the state from another <see cref="IComplexResult"/>. </summary>
    /// <param name="rhs"> The other client result. Can't be null. </param>
    public ComplexResult(IComplexResult rhs) : base(rhs)
    { }

    /// <summary> Constructor that initializes by copying the state from another <see cref="IComplexErrorResult{TError}"/>. </summary>
    /// <param name="rhs"> The other client result. Can't be null. </param>
    public ComplexResult(IComplexErrorResult<object> rhs) : base(rhs)
    { }
    #endregion // Constructors

    #region Properties

    /// <summary> Prepared standard representation of successful result. </summary>
    public static new IComplexResult OK { get => _ok.Value; }
    #endregion // Properties

    #region Methods

    #region Static_Factory_methods

    /// <summary>
    /// Static factory-like method, creates either a "failed" or "successful" instance of ComplexResult,
    /// depending on <paramref name="errorMessage"/> argument.
    /// </summary>
    ///
    /// <param name="errorMessage"> Message describing the error. It may be null or empty; in that case a
    /// "successful" ComplexResult is created; otherwise, failed result is created. </param>
    ///
    /// <returns> The new ComplexResult instance. </returns>
    public static IComplexResult Create(string errorMessage)
    {
        ComplexResult result;

        if (string.IsNullOrEmpty(errorMessage))
            result = new ComplexResult();
        else
            result = new ComplexResult(errorMessage);

        return result;
    }

    /// <summary> Static factory-like method, creates a "failed" instance of ComplexResult. </summary>
    /// <param name="errorMessage"> Message describing the error. Can't be null or empty. </param>
    /// <param name="errorDetails"> (Optional) Error details, if available. </param>
    ///
    /// <returns> The new failed ComplexResult instance. </returns>
    public static new IComplexResult CreateFailed(string errorMessage, object errorDetails = null)
    {
        ComplexResult result = new(errorMessage, errorDetails);

        return result;
    }

    /// <summary> Static factory-like method, creates a "failed" instance of ComplexResult, with provided exception. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="exceptionCaught"/>
    /// is null. </exception>
    ///
    /// <param name="exceptionCaught"> The exception caught by caller's code. Can't be null. </param>
    ///
    /// <returns> The new failed ComplexResult instance. </returns>
    public static new IComplexResult CreateFailed(Exception exceptionCaught)
    {
        return new ComplexResult(exceptionCaught);
    }

    /// <summary>
    /// Static factory-like method, creates a "failed" instance of ComplexResult, based on provided other
    /// <paramref name="rhs"/> which failed originally.
    /// </summary>
    ///
    /// <exception cref="ArgumentException"> Thrown when argument <paramref name="rhs"/> is not in failed state. </exception>
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="rhs"/>
    /// is null. </exception>
    ///
    /// <param name="rhs"> The other client result. Can't be null. </param>
    ///
    /// <returns> The new failed ComplexResult instance. </returns>
    public static IComplexResult CreateFailed(IComplexResult rhs)
    {
        CheckIsFailed(rhs);

        ComplexResult result = new(rhs);
        Debug.Assert(result.Failed());

        return result;
    }

    /// <summary> Checks that <paramref name="rhs"/> is in failed state. </summary>
    /// <exception cref="ArgumentException"> Thrown when argument <paramref name="rhs"/> is not in failed state. </exception>
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="rhs"/>
    /// is null. </exception>
    /// <param name="rhs"> The other client result. Can't be null. </param>
    protected static void CheckIsFailed(IComplexResult rhs)
    {
        ArgumentNullException.ThrowIfNull(rhs);
        if (rhs.Success)
        {
            string errorMessage = $"The state of {nameof(rhs)} is not 'failed', as it is supposed to be.";
            throw new ArgumentException(errorMessage, nameof(rhs));
        }
    }
    #endregion // Static_Factory_methods

    /// <summary> Returns a string that represents the current object. </summary>
    /// <returns> A string that represents the current object. </returns>
    public override string ToString()
    {
        string result;

        if (Success)
        {
            result = "Success";
        }
        else
        {
            string errorDetails = ErrorDetails.NullSafe(x => x.ToString()).NullIfEmpty().NullSafe(x => $", Details: {x}");
            result = $"Error: {ErrorMessage}{errorDetails}";
        }
        return result;
    }
    #endregion // Methods
}

/// <summary>
/// Generic ComplexResult is an immutable object representing either a valid returned value,
/// or a failed state of the call. Implements <see cref="IComplexResult{T}"/> with a content.
/// </summary>
/// <typeparam name="T"> Type of the content. </typeparam>
public class ComplexResult<T> : ComplexErrorResult<T, object>, IComplexResult<T>
{
    #region Constructors

    /// <summary> Constructor that initializes this instance to successful state, with content. </summary>
    /// <param name="content"> The content. </param>
    public ComplexResult(T content) : base(content)
    { }

    /// <summary> Constructor that initializes this instance to failure state, with provided error message. </summary>
    /// <param name="errorMessage"> Message describing the error. Can't be null or empty. </param>
    /// <param name="errorDetails"> (Optional) The error details, if available. </param>
    public ComplexResult(string errorMessage, object errorDetails = null)
        : base(errorMessage, errorDetails)
    { }

    /// <summary> Constructor that initializes this instance to failure state, with provided exception. </summary>
    /// <param name="exceptionCaught"> The exception caught by caller's code. Can't be null. </param>
    public ComplexResult(Exception exceptionCaught) : base(exceptionCaught)
    { }

    /// <summary> Constructor that initializes this instance from another failed <see cref="IComplexResult"/>. </summary>
    /// <param name="rhs"> The other client result. Must be failed. </param>
    public ComplexResult(IComplexResult rhs) : base(rhs)
    { }

    /// <summary> Constructor that initializes this instance from another <see cref="IComplexErrorResult{T, TError}"/>,
    ///           including content if successful. </summary>
    /// <param name="rhs"> The other client result. Can't be null. </param>
    public ComplexResult(IComplexResult<T> rhs) : base(rhs)
    { }

    #endregion // Constructors

    #region Methods
    #region Static_Factory_methods

    /// <summary> Static factory-like method, that creates a "successful" instance of ComplexResult{T}. </summary>
    /// <remarks> May be needed in case the typeof(T) is string, to prevent ambiguity which of constructors
    ///           ComplexResult(string errorMessage, ...) and ComplexResult(T content) is actually called.
    /// </remarks>
    ///
    /// <param name="content"> The content to be assigned. </param>
    ///
    /// <returns> The new instance. </returns>
    public static new IComplexResult<T> CreateSuccessful(T content)
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
    /// <returns> The new "failed" instance. </returns>
    public static new IComplexResult<T> CreateFailed(string errorMessage, object errorDetails = null)
    {
        ComplexResult<T> result = new(errorMessage, errorDetails);

        Debug.Assert(!result.Success);
        return result;
    }

    /// <summary> Static factory-like method, creates a "failed" instance of ComplexResult{T}. </summary>
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
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="rhs"/> is null. </exception>
    /// <param name="rhs"> The other complex result. Can't be null. </param>
    /// <returns> The new failed ComplexResult instance. </returns>
    public static IComplexResult<T> CreateFailed(IComplexResult rhs)
    {
        CheckIsFailed(rhs);

        ComplexResult<T> result = new(rhs);
        Debug.Assert(result.Failed());

        return result;
    }
    #endregion // Static_Factory_methods
    #endregion // Methods
}

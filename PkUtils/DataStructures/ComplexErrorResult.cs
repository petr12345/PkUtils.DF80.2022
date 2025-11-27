// Ignore Spelling: Utils, rhs
//
using System;
using System.Diagnostics;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.DataStructures;

/// <summary>
/// Typed-error base implementation of <see cref="IComplexErrorResult{TError}"/>.
/// Mirrors the behavior of <see cref="ComplexResult"/> but with a strongly-typed <c>ErrorDetails</c>.
/// </summary>
public class ComplexErrorResult<TError> : IComplexErrorResult<TError>
{
    #region Fields
    private readonly bool _success;
    private readonly string _errorMessage;
    private readonly TError _errorDetails;
    private static readonly Lazy<IComplexErrorResult<TError>> _ok = new(() => new ComplexErrorResult<TError>());
    #endregion // Fields

    #region Constructor

    /// <summary> Default success constructor. </summary>
    public ComplexErrorResult()
        : this(success: true, errorMessage: null, errorDetails: default)
    { }

    /// <summary> Failure constructor with message and typed error details. </summary>
    /// <param name="errorMessage">Message describing the error. Can't be null or empty.</param>
    /// <param name="errorDetails">(Optional) The typed error details, if available.</param>
    public ComplexErrorResult(string errorMessage, TError errorDetails = default)
        : this(
              success: false,
              errorMessage: errorMessage ?? throw new ArgumentNullException(nameof(errorMessage)),
              errorDetails: errorDetails)
    { }

    /// <summary> Constructor that initializes to failure state, with provided exception. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="exceptionCaught"/>
    /// is null. </exception>
    ///
    /// <param name="exceptionCaught"> The exception caught by caller's code. Can't be null. </param>
    public ComplexErrorResult(Exception exceptionCaught)
    {
        ArgumentNullException.ThrowIfNull(exceptionCaught);

        if (typeof(TError).IsAssignableFrom(exceptionCaught.GetType()))
        {
            this._errorDetails = (TError)(object)exceptionCaught;
        }
        else
        {
            // If TError is not Exception or object, we can't assign _errorDetails.
            // Just use the exception's message.
            this._errorMessage = exceptionCaught.Message;
        }
    }

    /// <param name="rhs">The other (typed) client result. Can't be null.</param>
    public ComplexErrorResult(IComplexErrorResult<TError> rhs)
        : this(
            success: (rhs ?? throw new ArgumentNullException(nameof(rhs))).Success,
            errorMessage: rhs.Success ? null : rhs.ErrorMessage,
            errorDetails: rhs.Success ? default : rhs.ErrorDetails)
    { }

    /// <summary> Core constructor used by other overloads. </summary>
    /// <param name="success">Indicates success or failure.</param>
    /// <param name="errorMessage">Message describing the error.</param>
    /// <param name="errorDetails">The typed error details.</param>
    private ComplexErrorResult(bool success, string errorMessage, TError errorDetails)
    {
        if (success && !string.IsNullOrEmpty(errorMessage))
        {
            throw new ArgumentException($"Inconsistent state: a successful result cannot have a non-empty error message '{errorMessage}'.", nameof(errorMessage));
        }
        _success = success;
        _errorMessage = errorMessage;
        _errorDetails = errorDetails;
    }
    #endregion // Constructor

    #region Properties
    /// <summary> Prepared standard representation of successful result. </summary>
    public static IComplexErrorResult<TError> OK { get => _ok.Value; }

    /// <summary>
    /// Gets the "raw" message, as initialized by constructor. Note the difference from <see cref="ErrorMessage"/>.
    /// </summary>
    /// <seealso cref="ErrorMessage"/>
    protected string RawErrorMessage { get => _errorMessage; }
    #endregion // Properties

    #region IComplexErrorResult<TError> members

    /// <summary>  Gets a value indicating success / failure. </summary>
    public bool Success { get => _success; }

    /// <summary>  Gets the error details. </summary>
    public TError ErrorDetails { get => _errorDetails; }

    /// <summary> Gets a message describing the error. </summary>
    /// <seealso cref="RawErrorMessage"/>
    public virtual string ErrorMessage
    {
        get
        {
            string result;

            if (Success)
            {
                result = null;
            }
            else if (string.IsNullOrEmpty(result = RawErrorMessage)) // Prefer the explicitly set raw message
            {
                // If no raw message, and if the typed error is an Exception, mimic legacy formatting
                result = TryGetException(out Exception ex) ? GetDetailedExceptionMessage(ex) : string.Empty;
            }
            return result;
        }
    }
    #endregion // IComplexErrorResult<TError> members

    #region Methods

    #region Static_factory_methods

    /// <summary> Static factory-like method, that creates a "successful" instance of ComplexErrorResult&lt;TError&gt;. </summary>
    /// <returns>A successful <see cref="IComplexErrorResult{TError}"/> instance.</returns>
    public static IComplexErrorResult<TError> CreateSuccessful()
    {
        ComplexErrorResult<TError> result = new();
        Debug.Assert(result.Success);
        return result;
    }

    /// <summary> Static factory-like method, that creates a "failed" instance of ComplexErrorResult&lt;TError&gt;. </summary>
    /// <param name="errorMessage">Message describing the error. Can't be null or empty.</param>
    /// <param name="errorDetails">(Optional) The typed error details, if available.</param>
    /// <returns>A failed <see cref="IComplexErrorResult{TError}"/> instance.</returns>
    public static IComplexErrorResult<TError> CreateFailed(
        string errorMessage,
        TError errorDetails = default)
    {
        ComplexErrorResult<TError> result = new(errorMessage, errorDetails);
        Debug.Assert(!result.Success);
        return result;
    }

    /// <summary> Static factory-like method, that creates a "failed" instance of ComplexErrorResult. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="exceptionCaught"/>
    /// is null. </exception>
    /// 
    /// <returns>A failed <see cref="IComplexErrorResult{T, TError}"/> instance.</returns>
    public static IComplexErrorResult<TError> CreateFailed(Exception exceptionCaught)
    {
        ComplexErrorResult<TError> result = new(exceptionCaught);
        Debug.Assert(!result.Success);
        return result;
    }

    /// <summary> Static factory-like method, that creates a "failed" instance of ComplexErrorResult&lt;TError&gt; from another result. </summary>
    /// <param name="rhs">The other (typed) client result. Can't be null.</param>
    /// <returns>A failed <see cref="IComplexErrorResult{TError}"/> instance.</returns>
    public static IComplexErrorResult<TError> CreateFailed(IComplexErrorResult<TError> rhs)
    {
        CheckIsFailed(rhs);
        ComplexErrorResult<TError> result = new(rhs);
        Debug.Assert(!result.Success);
        return result;
    }

    /// <summary> Checks that <paramref name="rhs"/> is in failed state. </summary>
    /// <param name="rhs">The other client result. Can't be null.</param>
    /// <exception cref="ArgumentException">Thrown when argument <paramref name="rhs"/> is not in failed state.</exception>
    /// <exception cref="ArgumentNullException">Thrown when required argument <paramref name="rhs"/> is null.</exception>
    protected static void CheckIsFailed(IComplexErrorResult<TError> rhs)
    {
        ArgumentNullException.ThrowIfNull(rhs);
        if (rhs.Success)
        {
            string errorMessage = $"The provided result instance ('{nameof(rhs)}') is expected to represent a failed state, but it indicates success.";
            throw new ArgumentException(errorMessage, nameof(rhs));
        }
    }
    #endregion // Static_factory_methods

    #region Non_static_methods

    /// <summary>   Convert this object into a string representation. </summary>
    /// <returns>   A string that represents this object. </returns>
    public override string ToString()
    {
        if (Success) return "Success";
        string details = _errorDetails?.ToString();
        return string.IsNullOrEmpty(details) ? $"Error: {ErrorMessage}" : $"Error: {ErrorMessage}, Details: {details}";
    }

    /// <summary>   Attempts to get exception. </summary>
    /// <param name="exception"> [out] The exception. </param>
    /// <returns>   True if it succeeds, false if it fails. </returns>
    protected bool TryGetException(out Exception exception)
    {
        exception = _errorDetails as Exception;
        return exception != null;
    }

    /// <summary>
    /// Matches the legacy detailed exception formatting.
    /// </summary>
    /// <param name="exception">The examined exception. Can't be null.</param>
    /// <returns>The detailed exception message.</returns>
    protected virtual string GetDetailedExceptionMessage(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        string result;

        if (exception.InnerException != null)
            result = exception.ExceptionDetails(includeStackTrace: false);
        else
            result = exception.Message;

        return result;
    }
    #endregion // Non_static_methods
    #endregion // Methods
}

/// <summary>
/// Dual-generic ComplexErrorResult is an immutable object representing either a valid returned value,
/// or a failed state of the call. Implements <see cref="IComplexErrorResult{T, TError}"/>.
/// </summary>
/// <typeparam name="T">Type of the content.</typeparam>
/// <typeparam name="TError">Type of error details.</typeparam>
public class ComplexErrorResult<T, TError> : ComplexErrorResult<TError>, IComplexErrorResult<T, TError>
{
    #region Fields
    private readonly T _content;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Constructor that initializes this instance to successful state, with content. </summary>
    /// <remarks>
    /// Note that any value of T (including null) is considered a valid value here.
    /// If you require a different behaviour, derive your own class.
    /// </remarks>
    /// <param name="content"> The content. </param>
    public ComplexErrorResult(T content)
        : base()
    {
        _content = content;
    }

    /// <summary> 
    /// Constructor that initializes this instance to failure state, with provided error message and typed details. 
    /// </summary>
    /// <param name="errorMessage"> Message describing the error. Can't be null or empty. </param>
    /// <param name="errorDetails"> (Optional) The typed error details, if available. </param>
    public ComplexErrorResult(string errorMessage, TError errorDetails = default)
        : base(errorMessage, errorDetails)
    { }

    /// <summary> Constructor that initializes to failure state, with provided exception. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="exceptionCaught"/>
    /// is null. </exception>
    ///
    /// <param name="exceptionCaught"> The exception caught by caller's code. Can't be null. </param>
    public ComplexErrorResult(Exception exceptionCaught) : base(exceptionCaught)
    { }

    /// <summary> 
    /// Constructor that initializes this instance to state of <paramref name="rhs"/>, including content if successful. 
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="rhs"/> is null. </exception>
    /// <param name="rhs"> The other (typed) client result. Can't be null. </param>
    public ComplexErrorResult(IComplexErrorResult<T, TError> rhs)
        : base(rhs)
    {
        if ((rhs ?? throw new ArgumentNullException(nameof(rhs))).Success)
        {
            _content = rhs.Content;
        }
    }

    /// <summary>
    /// Failure constructor.
    /// Ensures <paramref name="rhs"/> is failed and attempts to convert ErrorDetails to <typeparamref name="TError"/>.
    /// </summary>
    /// <param name="rhs">The other (legacy) client result. Must be in failed state and can't be null.</param>
    public ComplexErrorResult(IComplexErrorResult<TError> rhs)
        : base(rhs)
    {
        CheckIsFailed(rhs);
        Debug.Assert(!Success);
    }
    #endregion // Constructor(s)

    #region IComplexErrorResult{T, TError} members

    /// <inheritdoc/>
    public T Content { get => _content; }
    #endregion // IComplexErrorResult{T, TError} members

    #region Static_factory_methods

    /// <summary> Static factory-like method, that creates a "successful" instance of ComplexResult&lt;T, TError&gt;. </summary>
    /// <param name="content">The content to return in the successful result.</param>
    /// <returns>A successful <see cref="IComplexErrorResult{T, TError}"/> instance.</returns>
    public static IComplexErrorResult<T, TError> CreateSuccessful(T content)
    {
        ComplexErrorResult<T, TError> result = new(content);
        Debug.Assert(result.Success);
        return result;
    }

    /// <summary> Static factory-like method, that creates a "failed" instance of ComplexResult&lt;T, TError&gt;. </summary>
    /// <param name="errorMessage">Message describing the error. Can't be null or empty.</param>
    /// <param name="errorDetails">(Optional) The typed error details, if available.</param>
    /// <returns>A failed <see cref="IComplexErrorResult{T, TError}"/> instance.</returns>
    public static new IComplexErrorResult<T, TError> CreateFailed(
        string errorMessage,
        TError errorDetails = default)
    {
        var result = new ComplexErrorResult<T, TError>(errorMessage, errorDetails);
        Debug.Assert(!result.Success);
        return result;
    }

    /// <summary> Static factory-like method, that creates a "failed" instance of ComplexErrorResult. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="exceptionCaught"/>
    /// is null. </exception>
    /// 
    /// <returns>A failed <see cref="IComplexErrorResult{T, TError}"/> instance.</returns>
    public static new IComplexErrorResult<T, TError> CreateFailed(Exception exceptionCaught)
    {
        var result = new ComplexErrorResult<T, TError>(exceptionCaught);
        Debug.Assert(!result.Success);
        return result;
    }
    #endregion // Static_factory_methods

    #region NonStatic_Methods

    /// <summary> Returns a string that represents the current object. </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return Success ? $"Success: {Content}" : base.ToString();
    }
    #endregion // NonStatic_Methods
}

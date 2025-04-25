// Ignore Spelling: Utils, rhs
//
using System;
using System.Diagnostics;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.DataStructures;

/// <summary> Implements a <see cref="IComplexResult"/>, representing a 'void call result'. </summary>
/// <remarks>
/// <para>
/// ComplexResult is an immutable object representing either a successful or a failed state.
/// </para>
/// <para>
/// When created in a successful state, the instance contains no error message and no error details.
/// When created in a failed state, there are multiple options for initialization:
/// </para>
/// <list type="bullet">
///   <item>
///     <term>With a raw error message</term>
///     <description>
///     A new instance can be created with a plain error message and optional error details.
///     In this case, the error message returned by the instance will always match the originally provided message.
///     </description>
///   </item>
///   <item>
///     <term>With an exception</term>
///     <description>
///     A new instance can be created using an exception.
///     The resulting error message will be composed of the exception's message, its type,
///     and details from any inner exceptions, if present.
///     </description>
///   </item>
///   <item>
///     <term>By copying another failed instance</term>
///     <description>
///     A new instance can be created by copying an existing failed <see cref="IComplexResult"/> instance,
///     ensuring that the error message and details are preserved.
///     </description>
///   </item>
/// </list>
/// <para>
/// Additionally, ComplexResult provides factory methods to simplify the creation of instances
/// in both success and failure states.
/// </para>
/// </remarks>

[CLSCompliant(true)]
public class ComplexResult : IComplexResult
{
    #region Fields

    private readonly bool _success;
    private readonly string _errorMessage;
    private readonly object _errorDetails;
    private static readonly IComplexResult _ok = new ComplexResult();
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default constructor. Initializes to success state, with no error message. </summary>
    public ComplexResult()
    {
        _success = true;
    }

    /// <summary> Constructor that initializes to failure state, with provided error message. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="errorMessage"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when a supplied <paramref name="errorMessage"/> is empty. </exception>
    ///
    /// <param name="errorMessage"> Message describing the error. Can't be null or empty. </param>
    /// <param name="errorDetails"> (Optional) Error details, if available. </param>
    public ComplexResult(string errorMessage, object errorDetails = null)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(errorMessage);

        /* this._success = false;  already is by default */
        this._errorMessage = errorMessage;
        this._errorDetails = errorDetails;
    }

    /// <summary> Constructor that initializes to failure state, with provided exception. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="exceptionCaught"/>
    /// is null. </exception>
    ///
    /// <param name="exceptionCaught"> The exception caught by caller's code. Can't be null. </param>
    public ComplexResult(Exception exceptionCaught)
    {
        ArgumentNullException.ThrowIfNull(exceptionCaught);

        /* this._success = false;  already is by default */
        this._errorDetails = exceptionCaught;
    }

    /// <summary> Constructor that initializes by copying the state from <paramref name="rhs"/>. </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="rhs"/> is null. </exception>
    ///
    /// <param name="rhs"> The other client result. Can't be null. </param>
    public ComplexResult(IComplexResult rhs)
    {
        ArgumentNullException.ThrowIfNull(rhs);

        if (rhs.Success)
        {
            this._success = true;
        }
        else
        {
            // try to cast to be able to access protected property
            this._errorMessage = (rhs is ComplexResult temp) ? temp.RawErrorMessage : rhs.ErrorMessage;
            this._errorDetails = rhs.ErrorDetails;
        }
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Prepared standard representation of successful result. </summary>
    public static IComplexResult OK { get => _ok; }

    /// <summary>
    /// Gets the "raw" message, as initialized by constructor. Note the difference from <see cref="ErrorMessage"/>.
    /// </summary>
    protected string RawErrorMessage
    {
        get { return _errorMessage; }
    }
    #endregion // Properties

    #region IComplexResult members

    /// <inheritdoc/>
    public bool Success => _success;

    /// <inheritdoc/>
    public object ErrorDetails => _errorDetails;

    /// <inheritdoc/>
    public virtual string ErrorMessage
    {
        get
        {
            string result = null;

            if (!this.Success)
            {
                // use primarily "raw" error message if there is any
                if (!string.IsNullOrEmpty(this.RawErrorMessage))
                {
                    result = this.RawErrorMessage;
                }
                else if (this.ExceptionCaught() != null)
                {
                    result = GetDetailedExceptionMessage(this.ExceptionCaught());
                }
                else
                {
                    result = string.Empty; // something went wrong
                }
            }

            return result;
        }
    }
    #endregion // IComplexResult members

    #region Methods

    /// <summary> Returns a string that represents the current object. </summary>
    /// <returns>   A string that represents the current object. </returns>
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

    /// <summary>
    /// Static factory-like method, creates either a "failed" or "successful"  instance of ComplexResult,
    /// depending on <paramref name="errorMessage"/> argument.
    /// </summary>
    ///
    /// <param name="errorMessage"> Message describing the error. It may be null or empty; in that case a
    /// successful"  ComplexResult is created; otherwise, failed result is created. </param>
    ///
    /// <returns>   The new ComplexResult instance. </returns>
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
    public static IComplexResult CreateFailed(string errorMessage, object errorDetails = null)
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
    public static IComplexResult CreateFailed(Exception exceptionCaught)
    {
        ComplexResult result = new(exceptionCaught);

        return result;
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
    /// <returns>   The new failed ComplexResult instance. </returns>
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

    /// <summary>
    /// Gets detailed exception message. 
    /// In case of exceptions nesting, returns the inner exception(s) details too.
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="exception"/> is null. </exception>
    ///
    /// <param name="exception"> The examined exception. Can't be null. </param>
    ///
    /// <returns>   The detailed exception message. </returns>
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
    #endregion // Methods
}
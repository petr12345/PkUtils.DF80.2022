/***************************************************************************************************************
*
* FILE NAME:   .\Utils\ExceptionHelper.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The class ExceptionHelper
*
**************************************************************************************************************/


// Ignore Spelling: Utils, Rethrow, Rethrown
//
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace PK.PkUtils.Utils;

/// <summary>
/// The auxiliary class, that will work as an exception that is inserted 
/// as an inner exception of the original exception, which is 'prepared' for rethrow.
/// Contains the original call stack of given exception.
/// This is needed in case someone wants to rethrow an exception, 
/// and to keep the original exception stack somewhere.
/// See the usage of class ExceptionHelper.
/// </summary>
/// 
/// <seealso href="https://blogs.msdn.microsoft.com/agileer/2013/05/17/the-correct-way-to-code-a-custom-exception-class/">
/// The CORRECT Way to Code a Custom Exception Class.
/// </seealso>
[Serializable]
[CLSCompliant(true)]
public class RethrownException : Exception
{
    #region Fields

    private readonly string _stackTrace;
    private const string _strEndOfInnerExceptionStack = "--- End of inner exception stack trace ---";
    private const string _strRETHROWN_STACK = "<RETHROWN-STACK>";
    private const string _strRETHROWN_ORIGINAL_STACK = "<RETHROWN-ORIGINAL STACK>";
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default argument-less constructor.
    /// </summary>
    public RethrownException()
    {
    }

    /// <summary>
    /// Constructor which specifies the error message.
    /// </summary>
    /// <param name="strMsg">The error message that explains the reason for the exception.</param>
    public RethrownException(string strMsg)
        : this(strMsg, null)
    {
    }

    /// <summary>
    /// Constructor which specifies the error message and original exception fro which the inner exception will be taken from.
    /// </summary>
    /// <param name="strMsg">The error message that explains the reason for the exception.</param>
    /// <param name="original">The original exception from which the inner exception will be retrieved.</param>
    public RethrownException(string strMsg, Exception original)
        : this(strMsg, original, 0)
    {
    }

    /// <summary>
    /// Constructor which specifies the original exception and the amount of omitted (skipped) inner exceptions
    /// when retrieving the inner exception from that.<br/>
    /// See the method <see cref="GetInnerException"/>.
    /// </summary>
    /// <param name="original">The original exception from which the inner exception will be retrieved.</param>
    /// <param name="skip">How many inner exceptions should be skipped</param>
    public RethrownException(Exception original, int skip)
        : this(_strRETHROWN_ORIGINAL_STACK, original, skip)
    {
    }

    /// <summary>
    /// Constructor; specifies the error message, the original exception 
    /// and the amount of omitted (skipped) inner exceptions 
    /// when retrieving the inner exception from that.<br/>
    /// See the method <see cref="GetInnerException"/>.
    /// </summary>
    /// <param name="strMsg">The error message that explains the reason for the exception.</param>
    /// <param name="original">The original exception from which the inner exception will be retrieved.</param>
    /// <param name="skip">How many inner exceptions should be skipped</param>
    public RethrownException(string strMsg, Exception original, int skip)
        : base(strMsg, GetInnerException(original, skip))
    {
        this._stackTrace = original.StackTrace;
        /* this.original = original; */
        /* this.skip = skip; */
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="original">The original exception from which the inner exception will be retrieved.</param>
    public RethrownException(Exception original)
        : this(original, 0)
    {
    }

    /// <summary>
    /// This constructor is required for deserializing our class from persistent storage.
    /// </summary>
    /// <param name="info">    The SerializationInfo that holds the serialized object data about the
    /// exception being thrown. </param>
    /// <param name="context"> The StreamingContext that contains contextual information about the
    /// source or destination. </param>
    [Obsolete("Making obsolete because of StreamingContex")]
    protected RethrownException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <summary> Implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface and
    /// returns the data needed to serialize this instance.. </summary>
    ///
    /// <param name="info">    An object that contains on the output the information required to
    /// serialize this instance. . </param>
    /// <param name="context"> A structure that contains the source and destination of the serialized
    /// stream associated current serialization of this instance. </param>
    [Obsolete("Making obsolete because of StreamingContex")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
    #endregion // Constructor

    #region Properties

    /// <summary>
    /// Overrides the implementation of the Exception.StackTrace property,
    /// which gets a string representation of the frames on the call stack at the time the current exception was thrown.<br/>
    /// This implementation returns a stack trace of the original exception, 
    /// that has been provided to constructor, and from which the inner exception will be retrieved.
    /// </summary>
    /// <seealso cref="Exception.InnerException"/>
    public override string StackTrace
    {
        get { return _stackTrace; }
    }
    #endregion // Properties

    #region Methods

    /// <summary> Overrides ( implements ) the virtual method of the base class. </summary>
    ///
    /// <returns> Returns the text of inner exception ( if there is any ), 
    ///           together with the stack trace ( if there is any ). </returns>
    public override string ToString()
    {
        string text2 = _strRETHROWN_STACK;

        if (this.InnerException != null)
        {
            string[] textArray1 = new string[6] {
                text2,
                " ---> ",
                this.InnerException.ToString(),
                Environment.NewLine,
                "   ",
                _strEndOfInnerExceptionStack
            };
            text2 = string.Concat(textArray1);
        }
        if (this.StackTrace != null)
        {
            text2 = text2 + Environment.NewLine + this.StackTrace;
        }
        return text2;
    }

    /// <summary>
    /// Get the inner exception, omitting (skipping) <paramref name="nSkip "/> inner exceptions in the inner exceptions chain hierarchy.
    /// For <paramref name="nSkip "/> equal to zero will return the very first inner exception (does not omit anything).
    /// </summary>
    /// <param name="ex">The original exception from which the inner exception is taken from.</param>
    /// <param name="nSkip">How many inner exceptions should be skipped</param>
    /// <returns>Resulting inner exception, or null if there is no inner exception "deep enough" in the inner exceptions chain hierarchy.</returns>
    public static Exception GetInnerException(Exception ex, int nSkip)
    {
        ArgumentNullException.ThrowIfNull(ex);
        if (nSkip < 0)
        {
            throw new ArgumentException("The value of this argument cannot be negative", nameof(nSkip));
        }

        for (; (ex != null) && (nSkip >= 0); nSkip--)
        {
            ex = ex.InnerException;
        }

        return ex;
    }
    #endregion // Methods
}

/// <summary>
/// The helper class, used to prepare the exception for rethrow. 
/// It will create quite new exception ( RethrownException newInnerEx ),
/// which keeps (saves) the stack trace of original argument 'innerEx'.
/// The return value of this method is the original argument ( innerEx ).
/// 
/// The usage of ExceptionHelper in your code should look like following:
/// <code>
///   try
///   {
///     ...
///   }
///   catch (Exception e)
///   // For case of TargetInvocationException, let's throw the actual cause of problem ( e.InnerException ).
///   // To preserve the original stack somewhere, the PrepareRethrow will create
///   // a new exception of type RethrownException, who keeps the original call stack,
///   // and insert that RethrownException as an inner exception of innerEx.
///   if (e is TargetInvocationException)
///       throw ExceptionHelper.PrepareRethrow(e.InnerException);
///    else
///       throw;
/// </code>
/// </summary>
[CLSCompliant(true)]
public static class ExceptionHelper
{
    #region Methods

    /// <summary>
    /// Prepares a new exception RethrownException, which stores the original call stack
    /// taken from innerEx, and sets the RethrownException as inner in the innerEx.
    /// Assuming the caller will throw the result of PrepareRethrow,
    /// ten the code that will catch that could get the original call stack from the inner RethrownException.
    /// </summary>
    /// <param name="innerEx">The modified exception, into which the new exception is inserted.</param>
    /// <returns>Returns the original input argument <paramref name="innerEx"/>.</returns>
    public static Exception PrepareRethrow(this Exception innerEx)
    {
        RethrownException newInnerEx = new(innerEx);
        // assign exception.InnerException = newInnerEx 

        typeof(Exception).InvokeMember(
            "_innerException",
            BindingFlags.SetField | BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            innerEx,
            [newInnerEx],
            CultureInfo.InvariantCulture);

        return innerEx;
    }


    #endregion // Methods
}

namespace PK.PkUtils.Threading;

using System;
using System.Runtime.Serialization;


#pragma warning disable SYSLIB0051


/// <summary>
/// Exception that is thrown when a concurrency conflict is detected,
/// typically in a scenario where an operation cannot proceed because of 
/// conflicting access or state.
/// </summary>
[Serializable]
public class ConcurrencyConflictException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrencyConflictException"/> class.
    /// </summary>
    public ConcurrencyConflictException()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrencyConflictException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ConcurrencyConflictException(string message)
        : base(message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrencyConflictException"/> class
    /// with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ConcurrencyConflictException(string message, Exception innerException)
        : base(message, innerException)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrencyConflictException"/> class
    /// with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    protected ConcurrencyConflictException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
}

#pragma warning restore SYSLIB0051
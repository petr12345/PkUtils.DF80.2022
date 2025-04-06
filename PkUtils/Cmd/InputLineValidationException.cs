using System;
using System.Runtime.Serialization;

namespace PK.PkUtils.Cmd;

/// <summary>
/// A custom exception that could be thrown from parsing an input line,
/// if encountering an input that is completely wrong, regardless which command it relates to.
/// An example of such case is some argument duplication.
/// </summary>
/// <seealso href="https://blogs.msdn.microsoft.com/agileer/2013/05/17/the-correct-way-to-code-a-custom-exception-class/">
/// The CORRECT Way to Code a Custom Exception Class
/// </seealso>
[Serializable]
public class InputLineValidationException : Exception
{
    /// <summary> Default constructor. </summary>
    public InputLineValidationException() : base()
    { }

    /// <summary> Single-argument constructor. </summary>
    ///
    /// <param name="message"> The error message. </param>
    public InputLineValidationException(string message)
        : base(message)
    { }

    /// <summary> Two-arguments constructor. </summary>
    ///
    /// <param name="message"> The error message. </param>
    /// <param name="innerException">   The inner exception. </param>
    public InputLineValidationException(string message, Exception innerException)
        : base(message, innerException)
    { }

    /// <summary>  Specialized constructor for to support of serialization. </summary>
    ///
    /// <param name="info">     The serialization information. </param>
    /// <param name="context">  The serialization context. </param>
    [Obsolete("Making obsolete because of StreamingContex")]
    protected InputLineValidationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
}

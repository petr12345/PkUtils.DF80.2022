using System;
using System.Runtime.Serialization;

namespace PK.Commands.CommandExceptions;

/// <summary>  Custom Exception that could be thrown generally from command execution, 
///            thus indication something went quite wrong. 
///            A derived exception is <see cref="CommandValidationException"/>
/// </summary>
/// 
/// <seealso href="https://blogs.msdn.microsoft.com/agileer/2013/05/17/the-correct-way-to-code-a-custom-exception-class/">
/// The CORRECT Way to Code a Custom Exception Class</seealso>
[Serializable]
public class CommandProcessingErrorException : Exception
{
    /// <summary>  Default constructor. </summary>
    public CommandProcessingErrorException() : base()
    { }

    /// <summary>   Constructor. </summary>
    ///
    /// <param name="message">  The message. </param>
    public CommandProcessingErrorException(string message)
        : base(message)
    { }

    /// <summary>   Constructor. </summary>
    ///
    /// <param name="message">          The message. </param>
    /// <param name="innerException">   The inner exception. </param>
    public CommandProcessingErrorException(string message, Exception innerException)
        : base(message, innerException)
    { }

    /// <summary>   Specialized constructor for to support of serialization. </summary>
    ///
    /// <param name="info">     The serialization information. </param>
    /// <param name="context">  The serialization context. </param>
    [Obsolete("Making obsolete because of StreamingContex")]
    protected CommandProcessingErrorException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
}

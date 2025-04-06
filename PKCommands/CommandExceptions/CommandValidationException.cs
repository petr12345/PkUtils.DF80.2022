using System;
using System.Runtime.Serialization;

namespace PK.Commands.CommandExceptions;

/// <summary>  Custom Exception that could be thrown from BaseCommand.Validate. </summary>
/// 
/// <seealso href="https://blogs.msdn.microsoft.com/agileer/2013/05/17/the-correct-way-to-code-a-custom-exception-class/">
/// The CORRECT Way to Code a Custom Exception Class</seealso>
[Serializable]
public class CommandValidationException : CommandProcessingErrorException
{
    #region Fields
    private readonly bool _alreadyDisplayed;
    private readonly bool _alreadyLogged;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>  Default constructor. </summary>
    public CommandValidationException()
        : base()
    { }

    /// <summary>   Constructor. </summary>
    ///
    /// <param name="message">          The message. </param>
    public CommandValidationException(string message)
        : this(message, false, false)
    { }

    /// <summary>   Constructor. </summary>
    ///
    /// <param name="message">          The message. </param>
    /// <param name="alreadyDisplayed"> True if error already displayed. </param>
    /// <param name="alreadyLogged"> True if error already logged. </param>
    public CommandValidationException(string message, bool alreadyDisplayed, bool alreadyLogged)
        : base(message)
    {
        _alreadyDisplayed = alreadyDisplayed;
        _alreadyLogged = alreadyLogged;
    }

    /// <summary>   Constructor. </summary>
    ///
    /// <param name="message">          The message. </param>
    /// <param name="innerException">   The inner exception. </param>
    public CommandValidationException(string message, Exception innerException)
        : base(message, innerException)
    { }

    /// <summary>   Constructor. </summary>
    ///
    /// <param name="message">          The message. </param>
    /// <param name="innerException">   The inner exception. </param>
    /// <param name="alreadyDisplayed"> True if error already displayed. </param>
    /// <param name="alreadyLogged"> True if error already logged. </param>
    public CommandValidationException(string message,
        Exception innerException,
        bool alreadyDisplayed,
        bool alreadyLogged)
        : this(message, innerException)
    {
        _alreadyDisplayed = alreadyDisplayed;
        _alreadyLogged = alreadyLogged;
    }

    /// <summary>   Specialized constructor for to support of serialization. </summary>
    ///
    /// <param name="info">     The serialization information. </param>
    /// <param name="context">  The serialization context. </param>
    [Obsolete("Making obsolete because of StreamingContex")]
    protected CommandValidationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        _alreadyDisplayed = info.GetBoolean(nameof(_alreadyDisplayed));
        _alreadyLogged = info.GetBoolean(nameof(_alreadyLogged));
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>   Gets a value indicating whether the error is already displayed. </summary>
    public bool AlreadyDisplayed { get => _alreadyDisplayed; }

    /// <summary>   Gets a value indicating whether the error is already logged. </summary>
    public bool AlreadyLogged { get => _alreadyLogged; }

    #endregion // Properties

    #region Methods

    /// <summary>
    /// Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and returns the data
    /// needed to serialize the instance.
    /// </summary>
    ///
    /// <param name="info"> A SerializationInfo object that contains the information required to serialize
    /// the instance. </param>
    /// <param name="context"> A StreamingContext /> structure that contains the source and destination of the
    /// serialized stream associated with the instance. </param>
    [Obsolete("Making obsolete because of StreamingContex")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(_alreadyDisplayed), _alreadyDisplayed);
        info.AddValue(nameof(_alreadyLogged), _alreadyLogged);
    }
    #endregion // Methods
}

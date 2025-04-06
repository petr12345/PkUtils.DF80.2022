/***************************************************************************************************************
*
* FILE NAME:   .\NativeMemory\SharedMemoryException.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class SharedMemoryException
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//
using System;
using System.Globalization;

namespace PK.PkUtils.NativeMemory;

/// <summary>
/// The exception thrown by <see cref="BaseSegment"/> code. Derives from Exception.
/// </summary>
/// 
/// <seealso href="https://blogs.msdn.microsoft.com/agileer/2013/05/17/the-correct-way-to-code-a-custom-exception-class/">
/// The CORRECT Way to Code a Custom Exception Class.
/// </seealso>
[Serializable]
[CLSCompliant(true)]
public class SharedMemoryException : Exception
{
    /// <summary> Default argument-less constructor. </summary>
    public SharedMemoryException()
    { }

    /// <summary> Initializes a new instance of the Exception class with a specified error message. </summary>
    ///
    /// <param name="msg"> The message that describes the error. </param>
    public SharedMemoryException(string msg)
      : base(msg)
    { }

    /// <summary> Initializes a new instance of the Exception class with a specified error message and
    /// a reference to the inner exception that is the cause of this exception. </summary>
    ///
    /// <param name="msg">            The message that describes the error. </param>
    /// <param name="innerException"> The exception that is the cause of the current exception, or a
    /// null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
    public SharedMemoryException(string msg, Exception innerException)
      : base(msg, innerException)
    { }

    /// <summary> Initializes a new instance of the Exception class with a specified error message and an error
    /// code . The message and error code are  afterwards combined into juts one message. </summary>
    ///
    /// <param name="msg">        The message that describes the error. </param>
    /// <param name="nLastError"> The error code related to the last Win32 API call.  This code could
    /// be retrieved by <see cref="System.Runtime.InteropServices.Marshal.GetLastWin32Error"/> </param>
    public SharedMemoryException(string msg, int nLastError)
      : base(string.Format(CultureInfo.InvariantCulture, "{0}, GetLastError = {1}", msg, nLastError))
    { }

    /// <summary> Initializes a new instance of the Exception class with serialized data. </summary>
    ///
    /// <remarks> This constructor is called during deserialization to reconstitute the exception object
    /// transmitted over a stream. For more information see the topic
    /// <a href="http://msdn.microsoft.com/en-us/library/72hyey7b(v=vs.90).aspx">Binary Serialization
    /// .</a>. </remarks>
    ///
    /// <param name="info">    The SerializationInfo that holds the serialized object data about the
    /// exception being thrown. </param>
    /// <param name="context"> The StreamingContext that contains contextual information about the
    /// source or destination. </param>
    [Obsolete("Still want to support SerializationInfo-related constructor")]
    protected SharedMemoryException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    { }
}

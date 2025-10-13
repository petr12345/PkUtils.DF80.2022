// Ignore Spelling: Utils
//
#pragma warning disable SYSLIB0011 // BinaryFormatter serialization is obsolete and should not be used.
#pragma warning disable IDE0090 // Use 'new(...)'

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using static System.FormattableString;


namespace PK.PkUtils.NativeMemory;

/// <summary> This class derives from <see cref="BaseSegment"/>, which wraps Win32 shared memory. <br/>
/// The Segment extends the functionality of the base with the object ( or the objects graph )
/// serialization. You can store any serializable type or object graph in shared memory and have it
/// retrieved from another process on the same machine. Data is stored via the SetData method and
/// retrieved via the GetData method. Access to the shared memory segment can be synchronized using
/// the Lock and Unlock methods, which lock a named mutex. </summary>
[CLSCompliant(true)]
public class Segment : BaseSegment
{
    #region Constructor(s)

    /// <summary>
    /// Constructs the shared memory segment through calling the API for FileMapping and the ViewOfFile.
    /// </summary>
    /// <param name="fileMappingName"> The name given to file mapping object. Can't be null or empty. </param>
    /// <param name="creationFlag"> The creation flag, either 'Create' or 'Attach' </param>
    /// <param name="nBufferEffectiveSize"> The required effective size of the buffer</param>
    /// <param name="synchronized"> True if the access to shared memory should be synchronized. </param>
    public Segment(
        string fileMappingName,
        SharedMemoryCreationFlag creationFlag,
        int nBufferEffectiveSize,
        bool synchronized = true)
      : base(fileMappingName, creationFlag, nBufferEffectiveSize, synchronized)
    { }

    /// <summary>
    /// Constructs the shared memory segment through calling the API for FileMapping and the ViewOfFile.
    /// Just shortcut to constructor with argument SharedMemoryCreationFlag.Attach.
    /// </summary>
    /// <param name="fileMappingName"> The name given to file mapping object. Can't be null or empty. </param>
    /// <param name="synchronized"> True if the access to shared memory should be synchronized. </param>
    public Segment(string fileMappingName, bool synchronized = true)
      : base(fileMappingName, SharedMemoryCreationFlag.Attach, 0, synchronized)
    { }

    /// <summary> Constructs the shared memory segment through calling the API for FileMapping and the ViewOfFile,
    /// and copies the contents of <paramref name="dataStream"/> to shared memory segment.<br/>
    /// The segment created by this constructor has always its memory 'created', not 'attached' ( the
    /// constructor calls the overloaded constructor with argument SharedMemoryCreationFlag.Create ).
    /// The effective size of buffer of created segment is determined by the
    /// <paramref name="dataStream"/>length. </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="dataStream"/> is null. </exception>
    /// <exception cref="ObjectDisposedException"> Thrown when a supplied <paramref name="dataStream"/> has been disposed. </exception>
    ///
    /// <param name="fileMappingName"> The name given to file mapping object. Can't be null or empty. </param>
    /// <param name="dataStream">  <see cref="Stream"/> containing  data to be copied to shared memory. </param>
    /// <param name="synchronized"> True if the access to shared memory should be synchronized. </param>
    public Segment(string fileMappingName, Stream dataStream, bool synchronized = true)
      : base(fileMappingName, dataStream, synchronized)
    { }

    /// <summary>
    /// Constructs the shared memory segment through calling the API for FileMapping and the ViewOfFile,
    /// and copies byte array data to shared memory segment.
    /// The segment created by this constructor has always its memory 'created', not 'attached'
    /// ( the method calls overloaded constructor with argument SharedMemoryCreationFlag.Create ).
    /// The effective size of buffer of created segment is determined by the array length.
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="data"/> is null. </exception>
    /// 
    /// <param name="fileMappingName"> The name given to file mapping object. Can't be null or empty. </param>
    /// <param name="data"> The input array of bytes</param>
    /// <param name="synchronized"> True if the access to shared memory should be synchronized. </param>
    public Segment(string fileMappingName, byte[] data, bool synchronized = true)
      : base(fileMappingName, data, synchronized)
    { }

    /// <summary>
    /// Constructs the shared memory segment through calling the API for FileMapping and the ViewOfFile,
    /// and copies the data of object obj to its buffer.
    /// The effective size of buffer of created segment is determined by needed length for serialization.
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="obj"/> is null. </exception>
    /// <exception cref="ObjectDisposedException"> Thrown when a supplied <paramref name="obj"/> has been disposed. </exception>
    /// <exception cref="SharedMemoryException"> Thrown when the type of a supplied <paramref name="obj"/> is not serializable . </exception>
    /// 
    /// <param name="fileMappingName"> The name given to file mapping object. Can't be null or empty. </param>
    /// <param name="obj"> The object  being stored  to this shared memory segment. Can't be null. </param>
    /// <param name="synchronized"> True if the access to shared memory should be synchronized. </param>
    public Segment(string fileMappingName, object obj, bool synchronized = true)
      : this(fileMappingName, ObjectToStream(obj), synchronized)
    { }
    #endregion // Constructor(s)

    #region Methods
    #region Public Methods

    /// <summary> Returns the object graph stored in the shared memory segment. </summary>
    /// <returns>System.Object - root of object graph</returns>
    /// <exception cref="SerializationException"> Thrown if there was a problem during objects graph deserialization, 
    ///  for instance because of various object types versioning. </exception>
    public virtual object GetData()
    {
        using MemoryStream ms = new MemoryStream();
        BinaryFormatter bf = new BinaryFormatter();

        CopySharedMemoryToStream(ms);
        return bf.Deserialize(ms);
    }

    /// <summary>
    /// Stores serializable object graph in shared memory
    /// </summary>
    /// <param name="obj">System.Object root of object graph to be stored in shared memory</param>
    public virtual void SetData(object obj)
    {
        // Calculate size of serialized object graph
        using MemoryStream stream = ObjectToStream(obj);
        long requiredSaveSize = stream.Length;

        CheckBufferEffectiveSize(requiredSaveSize);
        CopyStreamToSharedMemory(stream);
    }
    #endregion // Public Methods

    #region Protected methods

    /// <summary>
    /// Implementation helper, converts the serializable object to the stream of bytes
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="obj"/> is null. </exception>
    /// <exception cref="ObjectDisposedException"> Thrown when a supplied <paramref name="obj"/> has been disposed. </exception>
    /// <exception cref="SharedMemoryException"> Thrown when the type of a supplied <paramref name="obj"/> is not serializable . </exception>
    /// 
    /// <param name="obj"> The object  being stored to MemoryStream </param>
    /// <returns>The memory stream containing data of serialized object <paramref name="obj"/></returns>
    protected static MemoryStream ObjectToStream(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        // Ensure root object is serializable
        /* if (!obj.GetType().IsSerializable) produces Error	SYSLIB0050	'Type.IsSerializable' is obsolete */
        if (!Attribute.IsDefined(obj.GetType(), typeof(SerializableAttribute)))
        {
            string errorMessage = Invariant($"Stored objects must be serializable, but {obj.GetType()} is not.");
            throw new SharedMemoryException(errorMessage);
        }

        // Calculate size of serialized object graph
        MemoryStream stream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();

        formatter.Serialize(stream, obj);
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }
    #endregion // Protected methods
    #endregion // Methods
}
#pragma warning restore IDE0090 // Use 'new(...)'
#pragma warning restore SYSLIB0011

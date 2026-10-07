// Ignore Spelling: discriminator, serializer, Utils
//
#pragma warning disable IDE0090 // Use 'new(...)'

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using PK.PkUtils.Serialization;

namespace PK.PkUtils.NativeMemory;

/// <summary>
/// Extends <see cref="BaseSegment"/> with safe, versioned JSON object transfer across processes.
/// Runtime types must be explicitly registered using a stable discriminator before use.
/// </summary>
/// <remarks>
/// The same discriminator must be registered for JSON-compatible runtime types in every process.
/// The JSON payload never activates a type by an assembly-qualified name. Access may be
/// synchronized using the inherited lock operations.
/// </remarks>
public class Segment : BaseSegment
{
    #region Fields

    private const string JsonFormatName = "PK.PkUtils.SharedMemory.Json";
    private const int JsonFormatVersion = 1;
    private static readonly JsonObjectTypeRegistry _objectTypeRegistry = new();
    private static readonly JsonSerializerOptions _serializerOptions = CreateSerializerOptions();

    #endregion // Fields

    #region Constructor(s)

    /// <summary>Creates or attaches to a named shared-memory segment.</summary>
    /// <param name="fileMappingName">The non-empty file-mapping name.</param>
    /// <param name="creationFlag">Specifies whether the mapping is created or attached.</param>
    /// <param name="nBufferEffectiveSize">The effective payload capacity.</param>
    /// <param name="synchronized">Whether access is synchronized by a named mutex.</param>
    public Segment(string fileMappingName, SharedMemoryCreationFlag creationFlag,
        int nBufferEffectiveSize, bool synchronized = true)
        : base(fileMappingName, creationFlag, nBufferEffectiveSize, synchronized)
    { }

    /// <summary>Attaches to an existing named shared-memory segment.</summary>
    /// <param name="fileMappingName">The non-empty file-mapping name.</param>
    /// <param name="synchronized">Whether access is synchronized by a named mutex.</param>
    public Segment(string fileMappingName, bool synchronized = true)
        : base(fileMappingName, SharedMemoryCreationFlag.Attach, 0, synchronized)
    { }

    /// <summary>Creates a segment and copies the supplied stream into it.</summary>
    /// <param name="fileMappingName">The non-empty file-mapping name.</param>
    /// <param name="dataStream">The readable payload stream.</param>
    /// <param name="synchronized">Whether access is synchronized by a named mutex.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataStream"/> is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when <paramref name="dataStream"/> is disposed.</exception>
    public Segment(string fileMappingName, Stream dataStream, bool synchronized = true)
        : base(fileMappingName, dataStream, synchronized)
    { }

    /// <summary>Creates a segment and copies the supplied bytes into it.</summary>
    /// <param name="fileMappingName">The non-empty file-mapping name.</param>
    /// <param name="data">The payload bytes.</param>
    /// <param name="synchronized">Whether access is synchronized by a named mutex.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    public Segment(string fileMappingName, byte[] data, bool synchronized = true)
        : base(fileMappingName, data, synchronized)
    { }

    /// <summary>Creates a segment containing a registered object's versioned JSON representation.</summary>
    /// <param name="fileMappingName">The non-empty file-mapping name.</param>
    /// <param name="obj">The non-null registered object to store.</param>
    /// <param name="synchronized">Whether access is synchronized by a named mutex.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    /// <exception cref="SharedMemoryException">Thrown when JSON serialization fails.</exception>
    public Segment(string fileMappingName, object obj, bool synchronized = true)
        : this(fileMappingName, ObjectToStream(obj), synchronized)
    { }

    #endregion // Constructor(s)

    #region Methods
    #region Public Methods

    /// <summary>Registers a JSON-serializable runtime type using a stable protocol discriminator.</summary>
    /// <typeparam name="T">The runtime type to register.</typeparam>
    /// <param name="discriminator">The stable, non-empty discriminator.</param>
    public static void RegisterType<T>(string discriminator)
    {
        _objectTypeRegistry.Register<T>(discriminator);
    }

    /// <summary>Registers a JSON-serializable runtime type using a stable protocol discriminator.</summary>
    /// <param name="type">The runtime type to register.</param>
    /// <param name="discriminator">The stable, non-empty discriminator.</param>
    public static void RegisterType(Type type, string discriminator)
    {
        _objectTypeRegistry.Register(type, discriminator);
    }

    /// <summary>Returns the registered object stored in this segment.</summary>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="SharedMemoryException">Thrown when the JSON payload cannot be deserialized.</exception>
    public virtual object GetData()
    {
        using MemoryStream stream = new MemoryStream();
        CopySharedMemoryToStream(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return ObjectFromStream(stream);
    }

    /// <summary>Stores a registered object's versioned JSON representation in this segment.</summary>
    /// <param name="obj">The non-null registered object to store.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    /// <exception cref="SharedMemoryException">Thrown when serialization fails or capacity is insufficient.</exception>
    public virtual void SetData(object obj)
    {
        using MemoryStream stream = ObjectToStream(obj);
        CheckBufferEffectiveSize(stream.Length);
        CopyStreamToSharedMemory(stream);
    }

    #endregion // Public Methods

    #region Protected Methods

    /// <summary>Reads and validates a registered object's versioned UTF-8 JSON payload.</summary>
    /// <param name="stream">The non-null readable payload stream.</param>
    /// <returns>The registered, non-null deserialized object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the stream is null.</exception>
    /// <exception cref="SharedMemoryException">Thrown when the JSON contract or protocol is invalid.</exception>
    protected static object ObjectFromStream(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        try
        {
            SegmentJsonEnvelope envelope = JsonSerializer.Deserialize<SegmentJsonEnvelope>(stream, _serializerOptions)
                ?? throw new JsonException("The shared-memory JSON payload was null.");
            ValidateEnvelope(envelope);
            return envelope.Data
                ?? throw new JsonException("The shared-memory JSON payload contained null data.");
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            throw new SharedMemoryException("The shared-memory JSON payload could not be deserialized.", ex);
        }
    }

    /// <summary>Converts a registered object into a versioned UTF-8 JSON payload stream.</summary>
    /// <param name="obj">The non-null registered object.</param>
    /// <returns>A readable stream positioned at the start of its JSON payload.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
    /// <exception cref="SharedMemoryException">Thrown when JSON serialization fails.</exception>
    protected static MemoryStream ObjectToStream(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        try
        {
            SegmentJsonEnvelope envelope = new SegmentJsonEnvelope
            {
                Format = JsonFormatName,
                Version = JsonFormatVersion,
                Data = obj,
            };
            MemoryStream stream = new MemoryStream();
            JsonSerializer.Serialize(stream, envelope, _serializerOptions);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            throw new SharedMemoryException(
                $"The object of type '{obj.GetType().FullName}' could not be serialized to shared-memory JSON.", ex);
        }
    }

    #endregion // Protected Methods

    #region Private Methods

    /// <summary>Creates the immutable serializer configuration used by all segments.</summary>
    private static JsonSerializerOptions CreateSerializerOptions()
    {
        JsonSerializerOptions options = new();
        options.Converters.Add(new RegisteredObjectJsonConverter(
            _objectTypeRegistry, "shared-memory segment", nameof(Segment)));
        return options;
    }

    /// <summary>Validates the shared-memory JSON protocol identifier and version.</summary>
    private static void ValidateEnvelope(SegmentJsonEnvelope envelope)
    {
        if (!StringComparer.Ordinal.Equals(envelope.Format, JsonFormatName))
        {
            throw new JsonException($"Unsupported shared-memory JSON format '{envelope.Format}'.");
        }

        if (envelope.Version != JsonFormatVersion)
        {
            throw new JsonException($"Unsupported shared-memory JSON protocol version '{envelope.Version}'.");
        }
    }

    #endregion // Private Methods
    #endregion // Methods

    #region Nested Types

    /// <summary>Defines the self-identifying shared-memory JSON protocol envelope.</summary>
    private sealed class SegmentJsonEnvelope
    {
        /// <summary>Gets or sets the protocol format identifier.</summary>
        [JsonPropertyName("$format")]
        public string Format { get; set; }

        /// <summary>Gets or sets the protocol version.</summary>
        [JsonPropertyName("$version")]
        public int Version { get; set; }

        /// <summary>Gets or sets the registered polymorphic payload.</summary>
        [JsonPropertyName("$data")]
        public object Data { get; set; }
    }

    #endregion // Nested Types
}

#pragma warning restore IDE0090 // Use 'new(...)'

// Ignore Spelling: CCA, discriminator, Json
//
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using PK.PkUtils.Serialization;
namespace PK.PkUtils.IO;

/// <summary>
/// Preserves registered runtime types stored through <see cref="ApplicationObjStorage"/>.
/// </summary>
internal sealed class ApplicationObjStorageJsonConverter : JsonConverter<object>
{
    #region Fields
    private readonly RegisteredObjectJsonConverter _converter;
    #endregion // Fields

    #region Constructors

    /// <summary> Initializes a new instance of the <see cref="ApplicationObjStorageJsonConverter"/> class. </summary>
    /// <param name="registry">The registry of runtime types supported by <see cref="ApplicationObjStorage"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <see langword="null"/>.</exception>
    public ApplicationObjStorageJsonConverter(ApplicationObjStorageTypeRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);
        _converter = new RegisteredObjectJsonConverter(
            registry,
            "application object storage",
            nameof(ApplicationObjStorage));
    }
    #endregion // Constructors

    #region Methods

    /// <inheritdoc />
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return _converter.Read(ref reader, typeToConvert, options);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        _converter.Write(writer, value, options);
    }
    #endregion // Methods
}


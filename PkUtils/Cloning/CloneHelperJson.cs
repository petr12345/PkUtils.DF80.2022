// Ignore Spelling: Utils, Json
//
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PK.PkUtils.Cloning.Json;

/// <summary>Creates deep copies using explicit JSON contracts.</summary>
/// <remarks>
/// Public properties and public fields form the default contract. Reference preservation supports
/// cycles and repeated references in mutable contracts. Private fields and formatter callbacks are
/// not copied. Configure nested polymorphism with JSON attributes or the options overload.
/// The root runtime type comes from the caller's object, never from a serialized CLR type name.
/// Immutable types require a JSON-compatible constructor or a custom converter.
/// </remarks>
public static class CloneHelperJson
{
    /// <summary>Creates a deep copy using the source object's runtime type.</summary>
    /// <param name="original">The non-null object to copy.</param>
    /// <returns>A distinct JSON round trip of the source object.</returns>
    public static object DeepClone(this object original)
    {
        return DeepClone(original, CreateSerializerOptions());
    }

    /// <summary>Creates a deep copy using caller-configured JSON contracts.</summary>
    /// <param name="original">The non-null object to copy.</param>
    /// <param name="options">The serializer configuration, including any required converters.</param>
    /// <returns>The deserialized copy.</returns>
    /// <exception cref="ArgumentNullException">An argument is null.</exception>
    /// <exception cref="JsonException">The object cannot be round-tripped.</exception>
    /// <exception cref="NotSupportedException">The JSON contract is unsupported.</exception>
    public static object DeepClone(object original, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(original);
        ArgumentNullException.ThrowIfNull(options);
        Type runtimeType = original.GetType();
        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(original, runtimeType, options);
        object result = JsonSerializer.Deserialize(payload, runtimeType, options)
            ?? throw new JsonException("The cloned JSON object was null.");
        return result;
    }

    /// <summary>Creates a typed deep copy while preserving the root runtime type.</summary>
    /// <typeparam name="T">The source's declared type.</typeparam>
    /// <param name="original">The non-null source object.</param>
    /// <returns>The copied object.</returns>
    public static T DeepClone<T>(this T original)
    {
        return (T)DeepClone((object)original);
    }

    /// <summary>Serializes an object as UTF-8 JSON bytes.</summary>
    /// <param name="original">The source object, or null.</param>
    /// <returns>The JSON bytes, or null when the source is null.</returns>
    /// <remarks>The result is not compatible with previous binary serialization bytes.</remarks>
    public static byte[] ToByteArray(this object original)
    {
        byte[] result = null;
        if (original != null)
        {
            result = JsonSerializer.SerializeToUtf8Bytes(original, original.GetType(), CreateSerializerOptions());
        }
        return result;
    }

    /// <summary>Creates a fresh configuration that preserves mutable object graph references.</summary>
    /// <returns>The configurable serializer options.</returns>
    public static JsonSerializerOptions CreateSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            IncludeFields = true,
            ReferenceHandler = ReferenceHandler.Preserve,
        };
    }
}

// Ignore Spelling: discriminator
//
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PK.PkUtils.Serialization;

/// <summary>
/// Serializes an <see cref="object"/> using a stable discriminator resolved exclusively through
/// an explicit <see cref="JsonObjectTypeRegistry"/> allow-list.
/// </summary>
internal sealed class RegisteredObjectJsonConverter : JsonConverter<object>
{
    #region Fields

    private const string TypePropertyName = "$type";
    private const string ValuePropertyName = "$value";
    private readonly JsonObjectTypeRegistry _registry;
    private readonly string _contextName;
    private readonly string _registrationOwnerName;

    #endregion // Fields

    #region Constructor(s)

    /// <summary>Initializes a converter for the supplied registry and diagnostic context.</summary>
    internal RegisteredObjectJsonConverter(
        JsonObjectTypeRegistry registry,
        string contextName,
        string registrationOwnerName)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _contextName = contextName ?? throw new ArgumentNullException(nameof(contextName));
        _registrationOwnerName = registrationOwnerName ?? throw new ArgumentNullException(nameof(registrationOwnerName));
    }

    #endregion // Constructor(s)

    #region Methods

    /// <inheritdoc />
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        JsonElement root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new JsonException($"A {_contextName} value must be represented by a JSON object.");
        }

        if (!root.TryGetProperty(TypePropertyName, out JsonElement typeElement) ||
            typeElement.ValueKind != JsonValueKind.String)
        {
            throw new JsonException($"The {_contextName} type discriminator is missing.");
        }

        string discriminator = typeElement.GetString();
        if (string.IsNullOrWhiteSpace(discriminator) || !_registry.TryGetType(discriminator, out Type runtimeType))
        {
            throw new JsonException($"Unsupported {_contextName} type '{discriminator}'.");
        }

        if (!root.TryGetProperty(ValuePropertyName, out JsonElement value))
        {
            throw new JsonException($"The {_contextName} value is missing.");
        }

        return value.Deserialize(runtimeType, options)
            ?? throw new JsonException($"The {_contextName} value for '{discriminator}' was null.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);

        Type runtimeType = value.GetType();
        if (!_registry.TryGetDiscriminator(runtimeType, out string discriminator))
        {
            throw new NotSupportedException(
                $"{_registrationOwnerName} does not support values of type '{runtimeType.FullName}'. " +
                $"Register the type through {_registrationOwnerName}.RegisterType before use.");
        }

        writer.WriteStartObject();
        writer.WriteString(TypePropertyName, discriminator);
        writer.WritePropertyName(ValuePropertyName);
        JsonSerializer.Serialize(writer, value, runtimeType, options);
        writer.WriteEndObject();
    }

    #endregion // Methods
}

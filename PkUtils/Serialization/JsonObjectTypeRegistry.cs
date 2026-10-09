// Ignore Spelling: discriminator, JSon
//
using System;
using System.Collections.Generic;

namespace PK.PkUtils.Serialization;

/// <summary>
/// Maintains an explicit, thread-safe mapping between stable JSON type discriminators and
/// runtime types that are permitted to participate in polymorphic serialization.
/// </summary>
/// <remarks>
/// The registry deliberately does not resolve assembly-qualified names supplied by serialized
/// data. A writer and reader may use different CLR assembly versions provided that both register
/// JSON-compatible types with the same stable discriminator.
/// </remarks>
public class JsonObjectTypeRegistry
{
    #region Fields

    private readonly Dictionary<string, Type> _typesByDiscriminator = new(StringComparer.Ordinal);
    private readonly Dictionary<Type, string> _discriminatorsByType = [];
    private readonly object _syncRoot = new();

    #endregion // Fields

    #region Constructor(s)

    /// <summary>Initializes an empty JSON object type registry.</summary>
    public JsonObjectTypeRegistry()
    { }

    #endregion // Constructor(s)

    #region Methods

    /// <summary>Registers a runtime type using a stable JSON discriminator.</summary>
    /// <typeparam name="T">The JSON-serializable runtime type.</typeparam>
    /// <param name="discriminator">The stable, non-empty discriminator.</param>
    public void Register<T>(string discriminator)
    {
        Register(typeof(T), discriminator);
    }

    /// <summary>Registers a runtime type using a stable JSON discriminator.</summary>
    /// <param name="type">The JSON-serializable runtime type.</param>
    /// <param name="discriminator">The stable, non-empty discriminator.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the type is not concrete, the discriminator is empty, or either value is already mapped differently.
    /// </exception>
    public void Register(Type type, string discriminator)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(discriminator);
        if (type == typeof(object) || type.IsAbstract || type.IsInterface || type.ContainsGenericParameters ||
            type.IsPointer || type.IsByRef || type == typeof(void))
        {
            throw new ArgumentException("Register a concrete JSON payload type, not System.Object or an open type.", nameof(type));
        }

        lock (_syncRoot)
        {
            if (_typesByDiscriminator.TryGetValue(discriminator, out Type existingType))
            {
                if (existingType == type)
                {
                    return;
                }

                throw new ArgumentException(
                    $"The discriminator '{discriminator}' is already registered for '{existingType.FullName}' and cannot be reassigned to '{type.FullName}'.",
                    nameof(discriminator));
            }

            if (_discriminatorsByType.TryGetValue(type, out string existingDiscriminator))
            {
                if (StringComparer.Ordinal.Equals(existingDiscriminator, discriminator))
                {
                    return;
                }

                throw new ArgumentException(
                    $"The type '{type.FullName}' is already registered as '{existingDiscriminator}' and cannot be reassigned to '{discriminator}'.",
                    nameof(type));
            }

            _typesByDiscriminator.Add(discriminator, type);
            _discriminatorsByType.Add(type, discriminator);
        }
    }

    /// <summary>Attempts to resolve the discriminator registered for a runtime type.</summary>
    /// <param name="type">The runtime type to resolve.</param>
    /// <param name="discriminator">Receives the discriminator when the type is registered.</param>
    /// <returns>True when the type is registered; otherwise, false.</returns>
    internal bool TryGetDiscriminator(Type type, out string discriminator)
    {
        ArgumentNullException.ThrowIfNull(type);
        lock (_syncRoot)
        {
            return _discriminatorsByType.TryGetValue(type, out discriminator);
        }
    }

    /// <summary>Attempts to resolve the permitted runtime type for a discriminator.</summary>
    /// <param name="discriminator">The discriminator to resolve.</param>
    /// <param name="type">Receives the permitted runtime type when registered.</param>
    /// <returns>True when the discriminator is registered; otherwise, false.</returns>
    internal bool TryGetType(string discriminator, out Type type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(discriminator);
        lock (_syncRoot)
        {
            return _typesByDiscriminator.TryGetValue(discriminator, out type);
        }
    }

    #endregion // Methods
}

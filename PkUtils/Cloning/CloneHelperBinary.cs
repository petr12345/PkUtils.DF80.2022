// Ignore Spelling: Utils
//
#pragma warning disable IDE0130 // Namespace does not match folder structure
using PK.PkUtils.Cloning.Json;

namespace PK.PkUtils.Cloning.Binary;

/// <summary>Retains the original cloning API while delegating to JSON serialization.</summary>
/// <remarks>
/// The name is retained for source compatibility only. No binary formatter is used.
/// Existing formatter bytes, private-field serialization and formatter callbacks are not supported.
/// New code should use <see cref="CloneHelperJson"/> and explicit JSON contracts.
/// </remarks>
public static class CloneHelperBinary
{
    /// <summary>Creates a JSON deep copy using the source runtime type.</summary>
    /// <param name="original">The non-null source object.</param>
    /// <returns>The copied object.</returns>
    public static object DeepClone(this object original)
    {
        return CloneHelperJson.DeepClone(original);
    }

    /// <summary>Creates a typed JSON deep copy.</summary>
    /// <typeparam name="T">The source's declared type.</typeparam>
    /// <param name="original">The non-null source object.</param>
    /// <returns>The copied object.</returns>
    public static T DeepClone<T>(this T original)
    {
        return CloneHelperJson.DeepClone<T>(original);
    }

    /// <summary>Serializes an object as UTF-8 JSON, retaining the original method name.</summary>
    /// <param name="original">The source object, or null.</param>
    /// <returns>JSON bytes, or null for a null source. These are not legacy formatter bytes.</returns>
    public static byte[] ToByteArray(this object original)
    {
        return CloneHelperJson.ToByteArray(original);
    }
}
#pragma warning restore IDE0130

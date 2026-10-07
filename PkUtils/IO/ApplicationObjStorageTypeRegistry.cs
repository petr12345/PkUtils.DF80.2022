// Ignore Spelling: CCA, discriminator
//
using System.Drawing;
using PK.PkUtils.Serialization;
namespace PK.PkUtils.IO;

/// <summary>
/// Maps stable JSON type discriminators to the runtime types permitted in
/// <see cref="ApplicationObjStorage"/>.
/// </summary>
/// <remarks>
/// The registry avoids resolving arbitrary assembly-qualified type names from persisted data.
/// Applications may register additional JSON-serializable types before saving or loading them.
/// Registrations are process-wide and thread-safe.
/// </remarks>
public sealed class ApplicationObjStorageTypeRegistry : JsonObjectTypeRegistry
{
    /// <summary>Creates a registry containing the types required by existing consumers.</summary>
    public ApplicationObjStorageTypeRegistry()
    {
        Register<int>("Int32");
        Register<Point>("Point");
        Register<Size>("Size");
        Register<Rectangle>("Rectangle");
    }

}

// Ignore Spelling: CCA, serializer, discriminator, Utils
//
using System;
using System.IO.IsolatedStorage;
using System.Text.Json;
namespace PK.PkUtils.IO;

/// <summary>
/// Provides persistent isolated-storage for arbitrary JSON-serializable
/// runtime objects, using stable type discriminators to safely round-trip
/// polymorphic values without exposing assembly-qualified type activation.
/// </summary>
/// 
/// <remarks>
/// The class derives from <see cref="ApplicationStorage{T}"/> and uses stable
/// type discriminators.
/// The registry is pre-populated with registrations for
/// <see cref="System.Int32"/>,
/// <see cref="System.Drawing.Point"/>, and
/// <see cref="System.Drawing.Size"/>.
///
/// Applications may register additional JSON-serializable runtime types by
/// calling <see cref="RegisterType{T}(string)"/> before saving or loading
/// them.
///
/// Persisted data never supplies an assembly-qualified type name for
/// unrestricted CLR type activation.
/// </remarks>
public class ApplicationObjStorage : ApplicationStorage<object>
{
    #region Private Fields
    private JsonSerializerOptions _serializerOptions;
    private static readonly ApplicationObjStorageTypeRegistry _objectTypeRegistry = new();
    #endregion // Private Fields

    #region Constructors(s)

    /// <summary>
    /// The default data-loading constructor. Creates a new <see cref="ApplicationObjStorage"/> instance.
    /// It also loads persisted data by calling the appropriate base-class constructor.
    /// Any exceptions raised during loading may be propagated from the underlying
    /// <see cref="ApplicationStorage{T}"/> implementation.
    /// </summary>
    public ApplicationObjStorage()
        : base()
    { }

    /// <summary>
    /// A "data loading" constructor, delegating the call to the corresponding
    /// <see cref="ApplicationStorage{T}"/> constructor and controlling whether
    /// loading should be performed through the regular or safe-loading path.
    /// </summary>
    /// <param name="safeLoad">
    /// If this value is false, the constructor performs a regular load.
    /// Otherwise it uses the safe-load mechanism provided by the base class.
    /// </param>
    public ApplicationObjStorage(bool safeLoad)
        : base(safeLoad)
    { }

    /// <summary>
    /// A "data loading" constructor that allows the caller to specify the
    /// underlying isolated-storage scope. The constructor delegates loading
    /// behavior to the corresponding <see cref="ApplicationStorage{T}"/> constructor.
    /// </summary>
    /// <param name="scope">Scope of the underlying isolated storage.</param>
    /// <param name="safeLoad">
    /// If this value is false, the constructor performs a regular load.
    /// Otherwise it uses the safe-load mechanism provided by the base class.
    /// </param>
    public ApplicationObjStorage(IsolatedStorageScope scope, bool safeLoad)
        : base(scope, safeLoad)
    { }

    /// <summary>
    /// A "data loading" constructor that allows the caller to specify both the
    /// underlying isolated-storage scope and a file name suffix used when resolving
    /// the storage file name.
    ///
    /// Depending on the value of <paramref name="safeLoad"/>, the constructor
    /// performs either a regular load or a safe-load operation through the
    /// underlying <see cref="ApplicationStorage{T}"/> implementation.
    /// </summary>
    /// <param name="scope">Scope of the underlying isolated storage.</param>
    /// <param name="safeLoad">
    /// If this value is false, the constructor performs a regular load.
    /// Otherwise it uses the safe-load mechanism provided by the base class.
    /// </param>
    /// <param name="fileNameSuffix">
    /// The file name suffix that will be used as part of the generated storage file name.
    /// </param>
    public ApplicationObjStorage(IsolatedStorageScope scope, bool safeLoad, string fileNameSuffix)
        : base(scope, safeLoad, fileNameSuffix)
    { }
    #endregion // Constructors(s)

    #region Methods

    /// <summary>
    /// Registers an additional runtime type using a stable discriminator.
    /// Registration must occur before the type is saved or loaded.
    /// </summary>
    public static void RegisterType<T>(string discriminator)
    {
        _objectTypeRegistry.Register<T>(discriminator);
    }

    /// <summary>
    /// Registers an additional runtime type using a stable discriminator.
    /// Registration must occur before the type is saved or loaded.
    /// </summary>
    public static void RegisterType(Type type, string discriminator)
    {
        _objectTypeRegistry.Register(type, discriminator);
    }

    /// <summary>
    /// Gets the <see cref="JsonSerializerOptions"/> used by this storage instance,
    /// configured with the <see cref="ApplicationObjStorageJsonConverter"/> that
    /// applies the registered stable type discriminators.
    /// </summary>
    protected override JsonSerializerOptions SerializerOptions
    {
        get { return _serializerOptions ??= CreateSerializerOptions(); }
    }

    /// <summary> Allows derived classes to customize the <see cref="JsonSerializerOptions"/> instance
    /// used for serialization. </summary>
    /// <param name="options"> The serializer options to configure. Can't be null. </param>
    protected virtual void ConfigureSerializerOptions(JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }

    /// <summary>
    /// Creates and configures the shared <see cref="JsonSerializerOptions"/>
    /// instance, registering the <see cref="ApplicationObjStorageJsonConverter"/>
    /// bound to the static type registry.
    /// </summary>
    /// <returns>The configured <see cref="JsonSerializerOptions"/>.</returns>
    private JsonSerializerOptions CreateSerializerOptions()
    {
        JsonSerializerOptions options = new();

        options.Converters.Add(new ApplicationObjStorageJsonConverter(_objectTypeRegistry));
        ConfigureSerializerOptions(options);
        return options;
    }
    #endregion // Methods
}

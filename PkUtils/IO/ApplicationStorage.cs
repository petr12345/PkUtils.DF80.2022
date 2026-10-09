// Ignore Spelling: CCA, Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text.Json;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using static System.FormattableString;

namespace PK.PkUtils.IO;

/// <summary>
/// Provides a dictionary-based persistent application store backed by
/// <see cref="IsolatedStorageFile"/> and serialized as JSON.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="ApplicationStorage{T}"/> is the current persistence implementation. It does not use
/// <c>BinaryFormatter</c>. Existing binary <c>.dtt</c> files are left untouched and are not loaded automatically.
/// </para>
/// <para>
/// The class derives from <see cref="Dictionary{TKey,TValue}"/> and stores values under string keys.
/// Unless a constructor intended only to initialize values is used, construction loads previously
/// persisted data by calling <see cref="LoadData"/> or <see cref="SafeLoadData"/>. Calling
/// <see cref="Save"/> serializes the dictionary to a JSON file in the selected isolated store.
/// </para>
/// <para>
/// JSON serialization is performed using <see cref="System.Text.Json.JsonSerializer"/>. Types stored as
/// <typeparamref name="T"/> must therefore be supported by System.Text.Json. In particular, applications
/// that require polymorphic serialization should configure it explicitly in the involved types or use a
/// specialized storage abstraction. Specialized polymorphic object storage is provided by
/// <see cref="ApplicationObjStorage"/> using an explicit allow-list of supported runtime types. Legacy binary object storage is not used by this implementation.
/// </para>
/// <para>
/// Isolated storage is an organization and isolation mechanism, not encryption or tamper protection.
/// Persisted data should not be treated as secret or trusted solely because it resides in isolated storage.
/// </para>
/// <para>
/// This class inherits the thread-safety characteristics of <see cref="Dictionary{TKey,TValue}"/>.
/// Callers must synchronize concurrent access when the collection can be modified.
/// </para>
/// </remarks>
/// <typeparam name="T">The type of values stored in the persistent dictionary.</typeparam>
public class ApplicationStorage<T> : Dictionary<string, T>
{
    #region Fields

    /// <summary> The default isolated-storage scope used by the parameterless constructor. </summary>
    public const IsolatedStorageScope DefaultStorageScope = IsolatedStorageScope.User | IsolatedStorageScope.Assembly;

    /// <summary> Result of the most recent call to <see cref="SafeLoadData"/>. </summary>
    private IComplexResult _lastSafeLoadResult;

    /// <summary> Indicates whether the most recent load found the expected storage file. </summary>
    private bool _lastLoadFoundAnyStorage;

    /// <summary> Scope of the isolated storage, as provided by the constructor. </summary>
    private readonly IsolatedStorageScope _scope;

    /// <summary> File name within isolated storage. </summary>
    private readonly string _settingsFileName;

    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Creates a new instance and loads persisted data by calling <see cref="LoadData"/>.
    /// </summary>
    public ApplicationStorage()
        : this(DefaultStorageScope, false)
    { }

    /// <summary>
    /// Creates a new instance and loads persisted data either normally or through <see cref="SafeLoadData"/>.
    /// </summary>
    /// <param name="safeLoad">
    /// If false, calls <see cref="LoadData"/>; otherwise calls <see cref="SafeLoadData"/>.
    /// </param>
    public ApplicationStorage(bool safeLoad)
        : this(DefaultStorageScope, safeLoad)
    { }

    /// <summary>
    /// Creates a new instance for the specified isolated-storage scope and loads persisted data.
    /// </summary>
    /// <param name="scope">Scope of the underlying isolated storage.</param>
    /// <param name="safeLoad">
    /// If false, calls <see cref="LoadData"/>; otherwise calls <see cref="SafeLoadData"/>.
    /// </param>
    public ApplicationStorage(IsolatedStorageScope scope, bool safeLoad)
        : this(scope, safeLoad, string.Empty)
    { }

    /// <summary>
    /// Creates a new instance for the specified isolated-storage scope and file-name suffix and loads
    /// persisted data.
    /// </summary>
    /// <param name="scope">Scope of the underlying isolated storage.</param>
    /// <param name="safeLoad">
    /// If false, calls <see cref="LoadData"/>; otherwise calls <see cref="SafeLoadData"/>.
    /// </param>
    /// <param name="fileNameSuffix">
    /// File-name suffix passed to <see cref="GenerateSettingsFileName"/>. It may be empty but not null.
    /// </param>
    public ApplicationStorage(IsolatedStorageScope scope, bool safeLoad, string fileNameSuffix)
    {
        _scope = scope;
        _settingsFileName = GenerateSettingsFileName(fileNameSuffix);

        if (safeLoad)
        {
            SafeLoadData();
        }
        else
        {
            LoadData();
        }
    }

    /// <summary>
    /// Creates a new instance initialized from <paramref name="dictionary"/>. This constructor does not
    /// load persisted data and does not save automatically.
    /// </summary>
    /// <param name="scope">Scope of the underlying isolated storage.</param>
    /// <param name="dictionary">Dictionary providing the initial contents. Cannot be null.</param>
    /// <param name="fileNameSuffix">
    /// File-name suffix passed to <see cref="GenerateSettingsFileName"/>. It may be empty but not null.
    /// </param>
    public ApplicationStorage(IsolatedStorageScope scope, IDictionary<string, T> dictionary, string fileNameSuffix)
        : base(dictionary ?? throw new ArgumentNullException(nameof(dictionary)))
    {
        _scope = scope;
        _settingsFileName = GenerateSettingsFileName(fileNameSuffix);
    }

    #endregion // Constructor(s)

    #region Properties

    /// <summary> Gets the isolated-storage scope supplied to the constructor. </summary>
    public IsolatedStorageScope Scope
    {
        get { return _scope; }
    }

    /// <summary>
    /// Gets the result of the most recent safe-load operation. The value is reset to null by
    /// <see cref="ResetContents"/>.
    /// </summary>
    public IComplexResult LastSafeLoadResult
    {
        get { return _lastSafeLoadResult; }
    }

    /// <summary> Gets whether the most recent load found the expected storage file. </summary>
    public bool LastLoadFoundAnyStorage
    {
        get { return _lastLoadFoundAnyStorage; }
    }

    /// <summary> Gets the file name of the JSON settings file. </summary>
    protected virtual string SettingsFileName
    {
        get { return _settingsFileName; }
    }

    /// <summary>
    /// Gets the JSON serializer options used by <see cref="Save"/> and <see cref="LoadData"/>.
    /// Derived classes may override this property when specialized JSON behavior is required.
    /// </summary>
    protected virtual JsonSerializerOptions SerializerOptions
    {
        get { return JsonSerializerOptions.Default; }
    }

    #endregion // Properties

    #region Methods

    #region Public methods

    /// <summary> Reloads the data from persistent storage. </summary>
    public void ReLoad()
    {
        LoadData();
    }

    /// <summary> Saves the dictionary to persistent storage as JSON. </summary>
    public void Save()
    {
        // Validate the complete JSON contract before opening and truncating the existing file.
        byte[] payload = JsonSerializer.SerializeToUtf8Bytes((Dictionary<string, T>)this, SerializerOptions);
        using IsolatedStorageFile isoStore = GetStorageFile();
        using IsolatedStorageFileStream stream = new(SettingsFileName, FileMode.Create, isoStore);
        stream.Write(payload);
    }

    /// <summary> Clears the dictionary and resets <see cref="LastSafeLoadResult"/>. </summary>
    public void ResetContents()
    {
        Clear();
        _lastSafeLoadResult = null;
    }

    #endregion // Public methods

    #region Protected methods

    /// <summary>
    /// Loads the dictionary from the underlying JSON file in <see cref="IsolatedStorageFile"/>.
    /// </summary>
    protected virtual void LoadData()
    {
        _lastLoadFoundAnyStorage = false;

        using IsolatedStorageFile isoStore = GetStorageFile();
        string fileName = SettingsFileName;

        if (isoStore.GetFileNames(fileName).Length == 0)
        {
            return;
        }

        _lastLoadFoundAnyStorage = true;

        using Stream stream = new IsolatedStorageFileStream(fileName, FileMode.Open, isoStore);
        Dictionary<string, T> appData = JsonSerializer.Deserialize<Dictionary<string, T>>(stream, SerializerOptions)
            ?? throw new JsonException("The application storage JSON contained a null dictionary.");

        foreach (KeyValuePair<string, T> item in appData)
        {
            this[item.Key] = item.Value;
        }
    }

    /// <summary>
    /// Safely loads data, converting supported persistence and JSON deserialization failures into
    /// <see cref="LastSafeLoadResult"/> rather than propagating them.
    /// </summary>
    protected void SafeLoadData()
    {
        Exception exCaught = null;

        try
        {
            LoadData();
            _lastSafeLoadResult = ComplexResult.OK;
        }
        catch (IsolatedStorageException ex)
        {
            exCaught = ex;
        }
        catch (JsonException ex)
        {
            exCaught = ex;
        }
        catch (NotSupportedException ex)
        {
            exCaught = ex;
        }

        if (exCaught != null)
        {
            string errorMessage = Invariant($"{nameof(SafeLoadData)} failed by {exCaught.GetType().Name} [{exCaught.Message}].");
            _lastSafeLoadResult = ComplexResult.CreateFailed(errorMessage, exCaught);
            Debug.WriteLine(errorMessage);
        }
    }

    /// <summary>
    /// Generates the JSON settings file name from the executing assembly name and optional suffix.
    /// </summary>
    /// <param name="fileNameSuffix">Optional file-name suffix. May be empty but not null.</param>
    /// <returns>The generated file name, for example <c>CCA.TestHooking.json</c>.</returns>
    protected virtual string GenerateSettingsFileName(string fileNameSuffix)
    {
        ArgumentNullException.ThrowIfNull(fileNameSuffix);

        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly()
            .GetRootOrCurrentAssembly();
        string asmName = assembly.GetName().Name;

        return $"{asmName}{fileNameSuffix}.json";
    }

    /// <summary> Creates the isolated-storage file store used for reading and writing. </summary>
    /// <returns>The isolated-storage file store.</returns>
    protected virtual IsolatedStorageFile GetStorageFile()
    {
        return IsolatedStorageFile.GetStore(Scope, null, null);
    }

    #endregion // Protected methods

    #endregion // Methods
}

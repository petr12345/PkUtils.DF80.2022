/***************************************************************************************************************
*
* FILE NAME:   .\IO\ApplicationStorage.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of generic class ApplicationStorage
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using static System.FormattableString;

namespace PK.PkUtils.IO;

#pragma warning disable SYSLIB0011 // BinaryFormatter serialization is obsolete and should not be used.

/// <summary>
/// A wrapper around <see cref="System.IO.IsolatedStorage.IsolatedStorageFile"/> functionality.
/// </summary>
/// 
/// <remarks>
/// <para>
/// <b>Introduction</b><br/>
/// 
/// The .NET Framework provides the concept of 'application configuration file' to store application settings.
/// The configuration file is a regular XML file and need to be shipped with the application. Since this is a simple XML file,
/// users can make changes to the configuration data using any simple text editor, without the need 
/// to recompile the application.
/// </para>
/// <para>
/// <b>Limitations of Application Configuration File</b><br/>
/// Configuration files are very handy and easy to use. But there are a few limitations:
/// - This is a read only mechanism. You cannot use the .NET ConfigurationSettings class to update
/// the app.configuration file.
/// - Since the configuration file is a simple XML file, you can write your own class to update the data from your application.
/// But in Windows environment, until you restart the application, the changes to the configuration file
/// will not be recognized by the application. In web applications, if you update the web.configuration,
/// the ASP.NET worker thread will be recycled.
/// - Security threat - The configuration file is accessible to everyone who has access to the file system.
/// It is possible that the user may edit the configuration file manually and mess up with the content or provide
/// invalid settings.
/// </para>
/// <para>
/// <b>Alternate approaches:</b><br/>
/// <list type="bullet">
/// <item><b>Registry</b></item>
/// <item><b>File</b></item>
/// <item><b>Serialize Custom classes</b></item>
/// <item><b>Isolated Storage</b></item>
/// </list>
/// 
/// Each of the above approaches have advantages and disadvantages. Writing into registry may need more
/// permissions than otherwise required by the application. Also, it is possible that any other application
/// can accidentally or deliberately overwrite the registry data. Writing into File also requires specific
/// permissions. Also, the question of where to store the file, how to secure it etc. raises.
/// </para>
/// 
/// <para>
/// <b>What is Isolated Storage?</b><br/>
/// 
/// .NET introduces a concept called Isolated Storage. Isolated Storage is a kind of Virtual Folder.
/// Users never need to know where exactly the file is stored. All you do is, tell the .NET framework to store your
/// file in Isolated Storage. The physical location of Isolated Storage varies for each Operating System. 
/// But your application simply uses the .NET classes to create and access files, without bothering where it is
/// physically located. And you can have Isolated Storage specific to each Assembly or each Windows user.
/// Since Isolated Storage can be specific to each user or assembly, you don't need to worry about other
/// users accidentally changing your files.
/// </para>
/// <para>
/// <b>ApplicationStorage - Overview</b><br/>
/// 
/// This code implements custom class that can be used to easily store and retrieve application data.
/// This custom class uses various features including Dictionary, serialization and isolated storage to
/// manipulate and store application data. The application data is stored in a dictionary as Key-Value pairs.
/// Dictionary supports inserting any kind of objects into it. So this class, which is derived from Dictionary,
/// will support saving any object into the persistent storage. When the Save() method is called,
/// the data will be serialized into Isolated Storage. In the constructor of the class, it will load data
/// by deserializing itself from the data stored in Isolated Storage.
/// </para>
/// 
/// <para>
/// <b>Disadvantages</b><br/>
/// Our class uses Isolated Storage and it has a few limitations.
/// - First thing is, Administrator can set quota per user and per assembly.
/// This will be a limitation and our utility will fail if it exceeds the limit.
/// - Also, smart users can find the location of Isolated Storage. Even though it is a hidden
/// location, it is possible that someone can locate it and remove or corrupt our files. So, it doesn't give
/// very high security for data.
/// </para>
///
/// <para>
/// <b>Remarks</b><br/>
/// A base class of this has been changed in the past from Hash table to a Dictionary{string, object}.
/// Be aware the change is not just formal, there are two differences in Hash table vs. Dictionary:
/// </para>
/// <para>
/// i/ With Hash table, you could safely look up any item (any key) without throwing an exception. With the
/// generic Dictionary, this is no longer an option. If you try to look-up an item with a Key that does not exist,
/// instead of returning null, a Dictionary will throw a KeyNotFound Exception
/// </para>
/// <para>
/// ii/ A subtle but important difference is that Hash table supports multiple reader threads with a single
/// writer thread, while Dictionary offers no thread safety. If you need thread safety with a generic
/// dictionary, you must implement your own synchronization or (in .NET 4.0) use ConcurrentDictionary{TKey, TValue}.
/// </para>
/// 
/// <para>
/// For further reference, see:
/// Types of Isolation
/// https://docs.microsoft.com/en-us/dotnet/standard/io/types-of-isolation <br/>
/// Isolated​Storage​Scope Enum
/// https://docs.microsoft.com/en-us/dotnet/api/system.io.isolatedstorage.isolatedstoragescope <br/>
/// "Unable to determine application identity of the caller"
/// https://social.msdn.microsoft.com/Forums/windows/en-US/90780a13-7830-46d0-bc7f-1f256eeebde7/unable-to-determine-application-identity-of-the-caller?forum=winformssetup <br/>
/// </para>
/// </remarks>
///
/// <typeparam name="T">  The type o values stored in the serialized dictionary. </typeparam>
[Serializable]
[CLSCompliant(true)]
public class ApplicationStorage<T> : Dictionary<string, T>
{
    #region Fields

    /// <summary> The default Isolated Storage scope, used by argument-less constructor. </summary>
    [NonSerialized]
    public const IsolatedStorageScope DefaultStorageScope = IsolatedStorageScope.User | IsolatedStorageScope.Assembly;

    /// <summary> Error indicator set by method <see cref="SafeLoadData"/> </summary>
    [NonSerialized]
    private IComplexResult _lastSafeLoadResult;

    /// <summary> Flag set by method <see cref="LoadData"/> </summary>
    [NonSerialized]
    private bool _lastLoadFoundAnyStorage;

    /// <summary> Scope of the isolated storage, as provided by constructor.</summary>
    /// <remarks> Usually, will be either IsolatedStorageScope.User or  IsolatedStorageScope.Assembly or both. </remarks>
    [NonSerialized]
    private readonly IsolatedStorageScope _scope;

    /// <summary>
    /// File name within Isolated Storage. Let us use the entry assembly name with .dat as the extension.
    /// </summary>
    private readonly string _settingsFileName;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// The default "data loading" constructor, constructs new ApplicationStorage object. 
    /// Note it also loads the data by calling <see cref="LoadData"/>. 
    /// It may throw <see cref="IsolatedStorageException"/>, <see cref="SerializationException"/>
    /// or <see cref="TargetInvocationException"/>, in case called method LoadData() throws that exception.
    /// </summary>
    /// 
    /// <exception cref="SerializationException"> Thrown if called LoadData throws that exception. </exception>
    public ApplicationStorage()
        : this(DefaultStorageScope, false)
    { }

    /// <summary>
    /// A "data loading" constructor, delegating the call to overloaded constructor <see cref="ApplicationStorage{T}(IsolatedStorageScope, bool, string)"/>
    /// which loads the data ( passing to it the <paramref name="safeLoad"/>argument ).
    /// </summary>
    /// <exception cref="IsolatedStorageException"> Thrown if called LoadData throws that exception. </exception>
    /// <exception cref="SerializationException"> Thrown if called LoadData throws that exception. </exception>
    /// <exception cref="TargetInvocationException"> Thrown if called LoadData throws that exception. </exception>
    /// 
    /// <param name="safeLoad"> If this value is false, the constructor calls <see cref="LoadData"/>, otherwise
    /// it calls<see cref="SafeLoadData"/>. </param>
    public ApplicationStorage(bool safeLoad)
      : this(DefaultStorageScope, safeLoad)
    { }

    /// <summary>
    /// A "data loading" constructor, delegating the call to overloaded constructor <see cref="ApplicationStorage{T}(IsolatedStorageScope, bool, string)"/>
    /// which loads the data ( passing to it the <paramref name="safeLoad"/>argument ).
    /// </summary>
    /// <exception cref="IsolatedStorageException"> Thrown if called LoadData throws that exception. </exception>
    /// <exception cref="SerializationException"> Thrown if called LoadData throws that exception. </exception>
    /// <exception cref="TargetInvocationException"> Thrown if called LoadData throws that exception. </exception>
    /// 
    /// <param name="scope"> Scope of underlying isolated storage. </param>
    /// <param name="safeLoad"> If this value is false, the constructor calls <see cref="LoadData"/>, otherwise
    /// it calls<see cref="SafeLoadData"/>. </param>
    public ApplicationStorage(IsolatedStorageScope scope, bool safeLoad)
      : this(scope, safeLoad, string.Empty)
    { }

    /// <summary>
    /// A "data loading" constructor, accepting a file name suffix to be used by the called <see cref="GenerateSettingsFileName"/>method.
    /// Depending on the argument <paramref name="safeLoad"/> value, the constructor calls either
    /// <see cref="LoadData"/> or <see cref="SafeLoadData"/>. <br/>
    /// The method SafeLoadData in case of error should NOT throw exception, but should set <see cref="LastSafeLoadResult"/>.
    ///
    /// </summary>
    /// 
    /// <exception cref="IsolatedStorageException"> Thrown if called LoadData throws that exception. </exception>
    /// <exception cref="SerializationException"> Thrown if called LoadData throws that exception. </exception>
    /// <exception cref="TargetInvocationException"> Thrown if called LoadData throws that exception. </exception>
    ///
    /// <param name="scope"> Scope of underlying isolated storage. </param>
    /// <param name="safeLoad"> If this value is false, the constructor calls <see cref="LoadData"/>,
    /// otherwise  it calls<see cref="SafeLoadData"/>. </param>
    /// 
    /// <param name="fileNameSuffix"> The file name suffix that will be used as an argument for
    /// <see cref="GenerateSettingsFileName"/> </param>
    public ApplicationStorage(IsolatedStorageScope scope, bool safeLoad, string fileNameSuffix)
    {
        _scope = scope;
        _settingsFileName = GenerateSettingsFileName(fileNameSuffix);

        if (!safeLoad)
        {
            LoadData();
        }
        else
        {
            SafeLoadData();
        }
    }

    /// <summary>
    /// A constructor that initializes a new instance of ApplicationStorage, that contains elements 
    /// copied from the specified IDictionary <paramref name="dictionary"/>.
    /// 
    /// The created instance could be later use to store the data, calling <see cref="Save()"/>.
    /// Note this constructor does NOT call that method by itself. 
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown if required argument <paramref name="dictionary"/> is null.
    /// </exception>
    /// 
    /// <param name="scope"> Scope of underlying isolated storage. </param>
    /// <param name="dictionary"> The dictionary providing initial contents of ApplicationStorage, to be saved later. 
    ///                           Can't be null.
    /// </param>
    /// <param name="fileNameSuffix"> The file name suffix that will be used as an argument for
    /// <see cref="GenerateSettingsFileName"/> </param>
    public ApplicationStorage(IsolatedStorageScope scope, IDictionary<string, T> dictionary, string fileNameSuffix)
        : base(dictionary)
    {
        _scope = scope;
        _settingsFileName = GenerateSettingsFileName(fileNameSuffix);
    }

    /// <summary>
    /// This protected constructor is required for deserializing our class from persistent storage,
    /// when the object is binary-serialized via [Serializable] attribute.
    /// </summary>
    /// 
    /// <param name="info">  The SerializationInfo that holds the serialized object data about the
    /// exception being thrown. </param>
    /// <param name="context"> The StreamingContext that contains contextual information about the
    /// source or destination. </param>
    [Obsolete("Making obsolete because of StreamingContex")]
    protected ApplicationStorage(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        _settingsFileName = GenerateSettingsFileName(string.Empty);
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Returns underlying isolated storage scope, as provided by constructor. </summary>        
    public IsolatedStorageScope Scope
    {
        get { return _scope; }
    }

    /// <summary>
    /// Has the last call of "safe load" failed?
    /// The value is set by <see cref="SafeLoadData"/>, and reset to null by <see cref="ResetContents"/>.
    /// </summary>
    public IComplexResult LastSafeLoadResult
    {
        get { return _lastSafeLoadResult; }
    }

    /// <summary> Gets a value indicating whether the last load found any storage isolated storage to load from. </summary>
    public bool LastLoadFoundAnyStorage
    {
        get { return _lastLoadFoundAnyStorage; }
    }

    /// <summary> Gets the filename of the settings file. </summary>
    protected virtual string SettingsFileName
    {
        get { return _settingsFileName; }
    }
    #endregion // Properties

    #region Methods

    #region Public Methods

    /// <summary> Loads the data again from persistent storage. </summary>
    public void ReLoad()
    {
        LoadData();
    }

    /// <summary>
    /// Saves the data to the persistent storage.
    /// </summary>
    public void Save()
    {
        // Open the stream from the IsolatedStorage.
        IsolatedStorageFile isoStore = GetStorageFile();
        using Stream stream = new IsolatedStorageFileStream(SettingsFileName, FileMode.Create, isoStore);

        // Serialize the contents into the IsolatedStorage.
        BinaryFormatter formatter = new();
        formatter.Serialize(stream, this);
    }

    /// <summary>
    /// Cleans the dictionary contents, and assigns _bLastSafeLoadHasFailed = false
    /// </summary>
    public void ResetContents()
    {
        Clear();
        _lastSafeLoadResult = null;
    }

    /// <summary> Implements the <see cref="ISerializable"/> interface and returns the data needed to serialize 
    ///  this instance this instance, when the object is binary-serialized via [Serializable] attribute. </summary>
    ///
    /// <param name="info">  An object that contains on the output the information required to
    /// serialize this instance. </param>
    /// 
    /// <param name="context"> A structure that contains the source and destination of the serialized
    /// stream associated current serialization of this instance. </param>
    [Obsolete("Making obsolete because of StreamingContex")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
    #endregion // Public Methods

    #region Protected methods

    /// <summary>
    /// Loads the data from underlying <see cref="IsolatedStorageFile"/> storage.
    /// The storage file name is determined by <see cref="_settingsFileName"/> field value.
    /// </summary>
    protected virtual void LoadData()
    {
        IsolatedStorageFile isoStore;
        string fileName = SettingsFileName;

        _lastLoadFoundAnyStorage = false;
        isoStore = GetStorageFile();

        if (isoStore.GetFileNames(fileName).Length == 0)
        {
            // File does not exist. Let us NOT try to DeSerialize it.
            return;
        }
        else
        {
            _lastLoadFoundAnyStorage = true;

            // Read the stream from Isolated Storage.
            using Stream stream = new IsolatedStorageFileStream(fileName, FileMode.OpenOrCreate, isoStore);

            // DeSerialize the contents from stream.
            BinaryFormatter formatter = new();
            Dictionary<string, T> appData = (Dictionary<string, T>)formatter.Deserialize(stream);

            // Enumerate through the collection and load our base dictionary.
            Dictionary<string, T>.Enumerator enumerator = appData.GetEnumerator();
            while (enumerator.MoveNext())
            {
                this[enumerator.Current.Key] = enumerator.Current.Value;
            }
        }
    }

    /// <summary>
    /// This is the "safe" version of <see cref="LoadData"/>.
    /// Does not throw SerializationException, but sets the value of property <see cref="LastSafeLoadResult"/>.
    /// </summary>
    protected void SafeLoadData()
    {
        Exception exCaught = null;

        try
        {
            LoadData();
            _lastSafeLoadResult = ComplexResult.OK;
        }
        catch (IsolatedStorageException ex) { exCaught = ex; }
        catch (SerializationException ex) { exCaught = ex; }
        catch (TargetInvocationException ex) when (ex.AllInnerExceptions().Any(x => x is SerializationException))
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

    /// <summary> Implementation helper, generates the settings filename.
    /// The result is used for <see cref="_settingsFileName"/> initialization.<br/> </summary>
    ///
    /// <remarks> You may override this method in a derived class. </remarks>
    ///
    /// <param name="fileNameSuffix"> A string that should be used as part of the file name (a suffix).
    /// May be empty, but not null. </param>
    ///
    /// <returns> Resulting value file name. </returns>
    protected virtual string GenerateSettingsFileName(string fileNameSuffix)
    {
        ArgumentNullException.ThrowIfNull(fileNameSuffix);

        // Better following than Assembly.GetEntryAssembly(), which may return null
        Assembly assembly = Assembly.GetExecutingAssembly().GetRootOrCurrentAssembly();
        string asmName = assembly.GetName().Name;
        string strResult = $"{asmName}{fileNameSuffix}.dtt";

        return strResult;
    }

    /// <summary> A virtual method that creates underlying IsolatedStorageFile for either reading or writing. </summary>
    ///
    /// <returns>  The created IsolatedStorageFile file. </returns>
    protected virtual IsolatedStorageFile GetStorageFile()
    {
        IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(this.Scope, null, null);

        return isoStore;
    }
    #endregion // Protected methods
    #endregion // Methods
}
#pragma warning restore SYSLIB0011
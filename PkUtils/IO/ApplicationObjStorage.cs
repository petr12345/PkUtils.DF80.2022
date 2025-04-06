/***************************************************************************************************************
*
* FILE NAME:   .\IO\ApplicationObjStorage.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class ApplicationObjStorage
*
**************************************************************************************************************/


// Ignore Spelling: Utils
//
using System;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Runtime.Serialization;


namespace PK.PkUtils.IO;

/// <summary>
/// A specialized ApplicationStorage for objects
/// </summary>
[Serializable]
public class ApplicationObjStorage : ApplicationStorage<object>
{
    /// <summary>
    /// The default constructor, constructs new ApplicationObjStorage object.
    /// Note it also loads the data by calling <see cref="ApplicationStorage{T}.LoadData"/>.
    /// It may throw <see cref="IsolatedStorageException"/>, <see cref="SerializationException"/>
    /// or <see cref="TargetInvocationException"/>, in case called method LoadData() throws that exception.
    /// </summary>
    public ApplicationObjStorage()
        : base()
    { }

    /// <summary>
    /// A constructor, delegating the call to base constructor and passing the safeLoad argument. </summary>
    ///
    /// <param name="safeLoad"> If this value is false, the constructor calls <see cref="ApplicationStorage{T}.LoadData()"/>,
    /// otherwise  it calls <see cref="ApplicationStorage{T}.SafeLoadData()"/>. </param>
    public ApplicationObjStorage(bool safeLoad)
        : base(safeLoad)
    { }

    /// <summary>
    /// A constructor, delegating the call to base constructor and passing the safeLoad argument. </summary>
    ///
    /// <param name="scope"> Scope of underlying isolated storage. </param>
    /// <param name="safeLoad"> If this value is false, the constructor calls <see cref="ApplicationStorage{T}.LoadData()"/>,
    /// otherwise  it calls <see cref="ApplicationStorage{T}.SafeLoadData()"/>. </param>
    public ApplicationObjStorage(IsolatedStorageScope scope, bool safeLoad)
        : base(scope, safeLoad)
    { }

    /// <summary>
    /// A constructor accepting a file name suffix to be used by the called GenerateSettingsFileName method. The
    /// constructor calls either <see cref="ApplicationStorage{T}.LoadData()"/> or
    /// <see cref="ApplicationStorage{T}.SafeLoadData()"/>, depending on the argument <paramref name="safeLoad "/>
    /// value. <br/>
    /// The method SafeLoadData does NOT throw SerializationException, but sets the property
    /// <see cref="ApplicationStorage{T}.LastSafeLoadResult"/>.
    /// </summary>
    ///
    /// <param name="scope"> Scope of underlying isolated storage. </param>
    /// <param name="safeLoad"> If this value is false, the constructor calls <see cref="ApplicationStorage{T}.LoadData()"/>,
    /// otherwise  it calls <see cref="ApplicationStorage{T}.SafeLoadData()"/>. </param>
    /// <param name="fileNameSuffix"> The file name suffix that will be used as an argument for
    /// <see cref="ApplicationStorage{T}.GenerateSettingsFileName(string)"/> </param>
    public ApplicationObjStorage(IsolatedStorageScope scope, bool safeLoad, string fileNameSuffix)
        : base(scope, safeLoad, fileNameSuffix)
    { }

    /// <summary>
    /// This constructor is required for deserializing our class from streaming context.
    /// </summary>
    /// <param name="info">    The SerializationInfo that holds the data needed to deserialize.
    /// </param>
    /// <param name="context"> The StreamingContext that contains contextual information about the
    /// source or destination. </param>
    [Obsolete("Marked obsolete because of StreamingContext")]
    protected ApplicationObjStorage(SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
}

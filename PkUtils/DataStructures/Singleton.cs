// Ignore Spelling: ctors, Utils
//
using System;
using System.Reflection;

#pragma warning disable IDE0301 // Simplify collection initialization

namespace PK.PkUtils.DataStructures;

/// <summary>
/// Singleton generic pattern support. 
/// </summary>
/// 
/// <typeparam name="T">The class that you want to make singleton.</typeparam>
/// 
/// <remarks>
/// <para>
/// Usage scenarios of this generic class are determined by C# limitations. 
/// Pitifully, in C# (unlike in C++ with templates) one cannot specify the base class the generic could derive from.
/// Code like following produces compilation error "Cannot derive from 'R' because it is a type parameter"
/// <code>
/// <![CDATA[
/// public class Singleton<T, R> : R
/// ]]>
/// </code>
/// </para>
/// <para>
/// Therefore, you cannot derive your generic from the other class; but at least
/// you could inherit from generic base class, with self as type parameter. This approach is called
/// <see href="https://ericlippert.com/2011/02/02/curiouser-and-curiouser/">
/// "Curiously recurring templates/generics pattern"</see>.
/// <example>
/// <code>
/// <![CDATA[
/// public class CountMap : Singleton<CountMap>, IDisposable
/// ]]>
/// </code>
/// </example>
/// </para>
/// </remarks>
/// 
/// <seealso href="https://ericlippert.com/2011/02/02/curiouser-and-curiouser/">
/// Curiously recurring templates/generics pattern</seealso>
/// <seealso href="http://social.msdn.microsoft.com/Forums/is/csharplanguage/thread/a92428ec-0ea0-49cb-b942-124f36661e98">
/// MSDN forum on Inheriting from generic base class, with self as type parameter</seealso>
/// <seealso href="http://blogs.msdn.com/b/oldnewthing/archive/2009/08/14/9869049.aspx">
/// MSDN blog on Why can't I declare a type that derives from a generic type parameter?</seealso>
/// <seealso href="http://andyclymer.blogspot.com/2008/02/true-generic-singleton.html">
/// .NET Mutterings about Singleton</seealso>
/// <seealso href="http://social.msdn.microsoft.com/Forums/en-US/netfxcompact/thread/20289c10-4f95-42af-b993-f6a01da60297/">
/// MSDN forum on How To Invoke A Non-Public Constructor On Compact Framework?</seealso>
[CLSCompliant(true)]
public class Singleton<T> where T : class
{
    #region Fields

    /// <summary>
    /// The variable for instance being created (if any). <br/>
    /// The variable is declared to be volatile, to ensure that assignment to the instance variable completes 
    /// before the instance variable can be accessed.
    /// </summary>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/ff650316.aspx">
    /// Implementing Singleton in C#
    /// </seealso>
    private static volatile T _instance;
    private static readonly object _syncRoot = new();
    #endregion // Fields

    #region Properties

    /// <summary>
    /// Get the instance of class; create a new one if does not exist yet. 
    /// Thread-safe version.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance is null)
            {
                lock (SyncRoot)
                {
                    _instance ??= CreateInstance();
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Returns true if the instance of T already has been created; false otherwise.
    /// </summary>
    /// <remarks>
    /// Remark: While the original singleton pattern does not consider such property,
    /// you will find it helpful in case your singleton class implements IDisposable.
    /// In that case you can "shut down" the existing instance by checking its existence first;
    /// otherwise ( without HasInstance property ) one would have to create the instance
    /// even if has not been created yet, just to guarantee its proper disposal.
    /// </remarks>
    public static bool HasInstance
    {
        get => (PeekInstance is not null);
    }

    /// <summary> Gets the existing instance, if there is any, or null. </summary>
    public static T PeekInstance
    {
        get { lock (SyncRoot) { return _instance; } }
    }

    /// <summary>
    /// Get the synchronization object that is used for instance-creation lock
    /// </summary>
    protected static object SyncRoot
    {
        get { return _syncRoot; }
    }
    #endregion // Properties

    #region Methods

    /// <summary> Attempts to dispose the instance of T, if it exists and is disposable. </summary>
    /// <remarks> Upon success, assigns instance to null; otherwise does not change anything. </remarks>
    /// <returns>   True if it succeeds, false if it fails. </returns>
    public static bool DisposeInstance()
    {
        lock (SyncRoot)
        {
            if (_instance is IDisposable disposable)
            {
                disposable.Dispose();
                _instance = null;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Private implementation helper; creates an instance via the private constructor.
    /// Is not thread-safe; it is up to the caller to guarantee thread safety.
    /// </summary>
    /// <returns>An instance of T</returns>
    /// <remarks>Throws InvalidOperationException if the contains any public T constructor</remarks>
    private static T CreateInstance()
    {
#if DEBUG
        // Ensure there are no public constructors...
        Type t = typeof(T);
        ConstructorInfo[] ctors = t.GetConstructors();
        if (ctors.Length > 0)
        {
            throw new InvalidOperationException(
              $"{t.Name} has at least one accessible constructor, making it impossible to enforce singleton behaviour");
        }
#endif
        // On DESKTOP, following commented-out code works just fine
        /* return (T)Activator.CreateInstance(t, true); */
        // But for case of NETCF, one has to go through more elaborate process, 
        // since Activator.CreateInstance has no such method overload.
        // 

        ConstructorInfo nonPublicConstructorInfo = typeof(T).GetConstructor(
          BindingFlags.NonPublic | BindingFlags.Instance, null, Array.Empty<Type>(), null);
        return (T)nonPublicConstructorInfo.Invoke(Array.Empty<object>());
    }
    #endregion // Methods
}

#pragma warning restore IDE0301 // Simplify collection initialization
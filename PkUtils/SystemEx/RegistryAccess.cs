/***************************************************************************************************************
* 
* FILE NAME:   .\SystemEx\RegistryAccess.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class RegistryAccess
*
**************************************************************************************************************/

// Ignore Spelling: Utils
// 
using System.Windows.Forms;

namespace PK.PkUtils.SystemEx;

/// <summary>
/// A class for managing registry access, delegating to <see cref="System.Windows.Forms.Application"/> methods.
/// </summary>
/// <remarks>
///  <para>
/// .NET has a quick way of getting / setting application specific registry values.
/// Application.UserAppDataRegistry returns a RegistryKey object pointing to the path 
/// HKEY_CURRENT_USER\Software\Company\Product\Version\. 
/// It ensures this path exists, etc.  <br/>
/// For the values of "Company", "Product" and "Version" the Framework turns to the AssemblyInfo.cs file.
/// <br/></para>
/// 
/// <para>
/// If you don't want a new Registry Tree every time you build you application, 
/// you will want to manual set the version number by replacing AssemblyVersion("1.0.*") with AssemblyVersion("1.0.0")
///
/// Using Application.UserAppDataRegistry the registry code boils down to:
/// <code>
/// public static string GetStringRegistryValue(string key, string defaultValue)
/// {	return Application.UserAppDataRegistry.GetValue(key, defaultValue) as string;}
/// public static void SetStringRegistryValue(string key, string stringValue)
/// {	Application.UserAppDataRegistry.SetValue(key, stringValue); }
/// Or you could replace you registry object all together.
/// </code>
/// 
/// This nice thing about this approach is that your code is generalized,
/// and can be used in a DLL or as source code and used without modification. 
/// .NET will always use the information from AssemblyInfo file of the Top-Level Application.
///  <br/></para>
/// </remarks>
public static class RegistryAccess
{
    /// <summary> 
    /// Getting the string value from the registry for given <paramref name="key"/>. <br/>
    /// For instance, if the Assembly.GetEntryAssembly attributes in AssemblyInfo.cs are implemented as
    /// <code>
    /// [assembly: AssemblyCompany("PK")]
    /// [assembly: AssemblyProduct("PK.TestSplash")]
    /// [assembly: AssemblyVersion("1.0.0.0")]
    /// </code>
    /// and the value of <paramref name="key"/> is "xyz", it will return a value of the key "xyz" under
    /// [HKEY_CURRENT_USER\Software\PK\PK.TestSplash\1.0.0.0]
    /// </summary>
    /// <param name="key">The key being returned from the registry.</param>
    /// <param name="defaultValue"> Default value that will be returned if the value in the registry registry is
    /// not found. </param>
    /// <returns>
    /// Existing string value in the registry, or the <paramref name="defaultValue"/> if the value in the
    /// registry does not exist. 
    /// </returns>
    public static string GetStringRegistryValue(string key, string defaultValue)
    {
        return Application.UserAppDataRegistry.GetValue(key, defaultValue) as string;
    }

    /// <summary>
    /// Writing the string value to the registry for given <paramref name="key"/>. <br/>
    /// For instance, if the Assembly.GetEntryAssembly attributes in AssemblyInfo.cs are implemented as
    /// <code>
    /// [assembly: AssemblyCompany("PK")]
    /// [assembly: AssemblyProduct("PK.TestSplash")]
    /// [assembly: AssemblyVersion("1.0.0.0")]
    /// </code>
    /// and the value of <paramref name="key"/> is "xyz", it will write a value for the key "xyz" under
    /// [HKEY_CURRENT_USER\Software\PK\PK.TestSplash\1.0.0.0]
    /// </summary>
    /// <param name="key">The key being modified in the registry.</param>
    /// <param name="stringValue">The value being written.</param>
    public static void SetStringRegistryValue(string key, string stringValue)
    {
        Application.UserAppDataRegistry.SetValue(key, stringValue);
    }
}

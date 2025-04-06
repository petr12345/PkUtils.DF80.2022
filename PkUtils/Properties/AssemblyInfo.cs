/***************************************************************************************************************
*
* FILE NAME:   .\Properties\AssemblyInfo.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains Assembly-related attributes.
*
**************************************************************************************************************/


using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("PK Utilities")]
[assembly: AssemblyDescription("PK Utilities Library for .NET 6.0")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("PK")]
[assembly: AssemblyProduct("PkUtils")]
[assembly: AssemblyCopyright("Copyright © 2023 PK")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: SupportedOSPlatform("windows")]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1bc39410-63d0-4f0b-af87-33ae6d5768a7")]

// The Common Language Specification (CLS) defines naming restrictions, data types, 
// and rules to which assemblies must conform if they are to be used across programming languages. 
// Good design dictates that all assemblies explicitly indicate CLS compliance with CLSCompliantAttribute.
[assembly: System.CLSCompliant(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.12.16.0")]
[assembly: AssemblyFileVersion("1.12.16.0")]
[assembly: NeutralResourcesLanguage("en")]

[assembly: InternalsVisibleTo("PK.PkUtils.NUnitTests, PublicKey=00240000048000009400000006020000002400005253413100040000010001001d207074f270143bf82e9ad20697aa4aff70d6c30cddcc01fa2ac0f2cdec47acfd3ebc3bd1e585e7a9c150af363f8df72a64ea60f3ab7ade9ca7e49bb3848c0550690b7ca219889e554ca366eab256083367ec747a30f46ad5110907cc5055296af89a99943660920cba6eaa4656eef36357d05de2aaf9c019d183989dee8eba")]
[assembly: InternalsVisibleTo("PK.Commands.NUnitTests, PublicKey=00240000048000009400000006020000002400005253413100040000010001001d207074f270143bf82e9ad20697aa4aff70d6c30cddcc01fa2ac0f2cdec47acfd3ebc3bd1e585e7a9c150af363f8df72a64ea60f3ab7ade9ca7e49bb3848c0550690b7ca219889e554ca366eab256083367ec747a30f46ad5110907cc5055296af89a99943660920cba6eaa4656eef36357d05de2aaf9c019d183989dee8eba")]
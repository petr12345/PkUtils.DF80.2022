// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PK.PkUtils.Extensions;

/// <summary>
/// Implements extension methods for <see cref="Assembly"/>.
/// </summary>
public static class AssemblyExtensions
{
    #region Extension Methods

    /// <summary>
    /// An Assembly extension method that for given <paramref name="assembly"/> returns the initial "root"
    /// Assembly that has referenced your current Assembly. This is useful in situation like web application,
    /// when there is no Assembly.GetEntryAssembly().
    /// Note, the result will be null if there is no "more root assembly than the input <paramref name="assembly"/>.
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when the input argument <paramref name="assembly"/> is
    /// null. </exception>
    ///
    /// <param name="assembly"> The assembly to act on. Can't be null. </param>
    ///
    /// <returns> The root assembly or null. </returns>
    public static Assembly GetRootAssembly(this Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        // The commented-out code below was observed to throw NullReferenceException in some cases, 
        // for reasons not entirely clear. It is replaced with a more robust code more below.
        // IEnumerable<Assembly> callerAssemblies = new StackTrace().GetFrames()
        //    .Select(x => x.GetMethod().ReflectedType.Assembly).Distinct()
        //    .Where(x => x.GetReferencedAssemblies().Any(y => y.FullName == theFullName));

        string targetFullName = assembly.FullName;
        StackFrame[] frames = new StackTrace().GetFrames();
        if (frames == null)
            return null;

        // Use HashSet for uniqueness
        HashSet<Assembly> callerAssemblies = [];
        foreach (StackFrame frame in frames)
        {
            Assembly asm = frame.GetMethod()?.ReflectedType?.Assembly;
            if (asm != null)
                callerAssemblies.Add(asm);
        }

        // Find assemblies that reference the target assembly; return the last one found
        Assembly rootAssembly = null;
        foreach (Assembly asm in callerAssemblies)
        {
            if (asm.GetReferencedAssemblies().Any(r => r?.FullName == targetFullName))
                rootAssembly = asm;
        }

        return rootAssembly;
    }

    /// <summary> An Assembly extension method that gets either root assembly of <paramref name="assembly"/>
    /// or that assembly itself. </summary>
    /// <param name="assembly"> The current assembly to act on. Can't be null. </param>
    /// <returns> The root or current assembly. </returns>
    public static Assembly GetRootOrCurrentAssembly(this Assembly assembly)
    {
        return assembly.GetRootAssembly() ?? assembly;
    }

    /// <summary> An Assembly extension method that gets assembly unique identifier,  
    ///           retrieved from its GuidAttribute. </summary>
    /// <exception cref="ArgumentNullException">Thrown when the input argument <paramref name="assembly"/> is null.
    /// </exception>
    ///
    /// <param name="assembly"> The assembly to act on. </param>
    /// <returns> The assembly unique identifier. </returns>
    public static string GetAssemblyGuid(this Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        return assembly.GetAssemblyAttribute<GuidAttribute>().NullSafe(attribute => attribute.Value);
    }

    /// <summary> An Assembly extension method that gets assembly product,  
    ///           retrieved from its AssemblyProductAttribute. </summary>
    /// <exception cref="ArgumentNullException">Thrown when the input argument <paramref name="assembly"/> is null.
    /// </exception>
    ///
    /// <param name="assembly"> The assembly to act on. </param>
    /// <returns> The assembly unique identifier. </returns>
    public static string GetAssemblyProduct(this Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        return assembly.GetAssemblyAttribute<AssemblyProductAttribute>().NullSafe(attribute => attribute.Product);
    }

    /// <summary> An Assembly extension method that gets assembly version. </summary>
    /// <exception cref="ArgumentNullException">Thrown when the input argument <paramref name="assembly"/> is null.
    /// </exception>
    ///
    /// <remarks>
    /// Note this is the proper way to get assembly version. One can't just get AssemblyVersion
    /// attribute by calling Asssembly.GetCustomAttributes and enumerating the result. See also
    /// <see href="https://stackoverflow.com/questions/14866768/why-cant-i-load-the-assemblyversion-attribute-using-reflection/">
    /// Why can't I load the AssemblyVersion attribute using reflection?</see>.
    /// </remarks>
    ///
    /// <param name="assembly"> The assembly to act on. </param>
    ///
    /// <returns> The assembly Version. </returns>
    public static Version GetAssemblyVersion(this Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        return assembly.GetName().Version;
    }

    /// <summary>
    /// Find the custom attribute of the given type t of the given assembly.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when the input argument <paramref name="assembly"/> is null.
    /// </exception>
    /// 
    /// <param name="assembly">The examined assembly. Must not equal to null.</param>
    /// <param name="t">The type of attribute which is searched in assembly custom attributes.</param>
    /// <returns>First Attribute of matching type, or null.</returns>
    public static Attribute GetAssemblyAttribute(this Assembly assembly, Type t)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(t);

        Attribute result = null;
        object[] atts = assembly.GetCustomAttributes(t, false);

        if (atts != null)
        {
            result = atts.FirstOrDefault(obj => t.IsInstanceOfType(obj)) as Attribute;
        }
        return result;
    }

    /// <summary>
    /// Find the custom attribute of the type T of the given assembly
    /// </summary>
    /// 
    /// <typeparam name="T">The type of attribute which is searched in assembly custom attributes.</typeparam>
    /// 
    /// <exception cref="ArgumentNullException">Thrown when the input argument <paramref name="assembly"/> is null.
    /// </exception>
    /// 
    /// <param name="assembly">The examined assembly. Must not equal to null.</param>
    /// <returns>First Attribute of matching type, or null.</returns>
    public static T GetAssemblyAttribute<T>(this Assembly assembly) where T : Attribute
    {
        return GetAssemblyAttribute(assembly, typeof(T)) as T;
    }
    #endregion // Extension Methods
}
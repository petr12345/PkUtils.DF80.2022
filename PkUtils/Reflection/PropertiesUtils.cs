// Ignore Spelling: Utils, bitmask, endregion
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.PkUtils.Reflection;

/// <summary>
/// Contains useful reflection-implemented utilities accessing properties.
/// </summary>
/// <seealso cref="EventsUtils"/>
/// <seealso cref="FieldsUtils"/>
/// <seealso cref="MethodsUtils"/>
/// <seealso cref="ReflectionUtils"/>
public static class PropertiesUtils
{
    #region Private Fields

    /// <summary> A bit-flag combination used when searching for non-static properties.</summary>
    private const BindingFlags AnyInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary> A bit-flag combination used when searching for static properties.</summary>
    private const BindingFlags AnyStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    #endregion // Private Fields

    #region Public Methods

    #region Accessing_static_properties_Shallow_Scope

    /// <summary>
    /// Gets the value of a static property declared directly on the specified <paramref name="t"/> type.
    /// </summary>
    /// <remarks>
    /// This method does not consider static properties declared in base classes. 
    /// To include base class properties, use the overload <see cref="GetStaticPropertyValue(Type, string, bool)"/> with <c>flattenHierarchy = true</c>.
    /// </remarks>
    /// <param name="t">The type that declares the static property.</param>
    /// <param name="propertyName">The name of the static property.</param>
    /// <returns>The value of the found static property, or <c>null</c> if not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="t"/> or <paramref name="propertyName"/> is <c>null</c>.</exception>
    public static object? GetStaticPropertyValue(this Type t, string propertyName)
    {
        return GetStaticPropertyValue(t, propertyName, false);
    }

    /// <summary>
    /// Gets the value of a static property on the specified <paramref name="t"/> type, optionally including properties declared in base classes.
    /// </summary>
    /// <remarks>
    /// If <paramref name="flattenHierarchy"/> is <c>true</c>, the method searches public and protected static properties in the inheritance hierarchy. 
    /// Private static properties declared in base classes are never returned by this method. To access all such properties, use <see cref="GetAllProperties(Type, BindingFlags)"/>.
    /// </remarks>
    /// <param name="t">The type that declares the static property.</param>
    /// <param name="propertyName">The name of the static property.</param>
    /// <param name="flattenHierarchy">
    /// When <c>true</c>, searches public and protected static properties up the inheritance chain; otherwise, only properties declared on <paramref name="t"/> are considered.
    /// </param>
    /// <returns>The value of the found static property, or <c>null</c> if not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="t"/> or <paramref name="propertyName"/> is <c>null</c>.</exception>
    public static object? GetStaticPropertyValue(this Type t, string propertyName, bool flattenHierarchy)
    {
        ArgumentNullException.ThrowIfNull(t);
        ArgumentNullException.ThrowIfNullOrEmpty(propertyName);

        BindingFlags flags = flattenHierarchy ? (AnyStatic | BindingFlags.FlattenHierarchy) : AnyStatic;
        return GetPropertyValue(t, null, propertyName, flags);
    }

    /// <summary>
    /// Sets the value of a static property declared directly on the specified <paramref name="t"/> type.
    /// </summary>
    /// <remarks>
    /// This method does not consider static properties declared in base classes. 
    /// To include base class properties, use the overload <see cref="SetStaticPropertyValue(Type, object, string, bool)"/> with <c>flattenHierarchy = true</c>.
    /// </remarks>
    /// <param name="t">The type that declares the static property.</param>
    /// <param name="value">The value to assign to the property.</param>
    /// <param name="propertyName">The name of the static property.</param>
    /// <returns><c>true</c> if the property was set successfully; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="t"/> or <paramref name="propertyName"/> is <c>null</c>.</exception>
    public static bool SetStaticPropertyValue(this Type t, object value, string propertyName)
    {
        return SetStaticPropertyValue(t, value, propertyName, false);
    }

    /// <summary>
    /// Sets the value of a static property on the specified <paramref name="t"/> type, optionally including properties declared in base classes.
    /// </summary>
    /// <remarks>
    /// If <paramref name="flattenHierarchy"/> is <c>true</c>, the method attempts to set public and protected static properties in the inheritance hierarchy. 
    /// Private static properties declared in base classes are never accessed by this method. To access all such properties, use <see cref="GetAllProperties(Type, BindingFlags)"/>.
    /// </remarks>
    /// <param name="t">The type that declares the static property.</param>
    /// <param name="value">The value to assign to the property.</param>
    /// <param name="propertyName">The name of the static property.</param>
    /// <param name="flattenHierarchy">
    /// When <c>true</c>, searches public and protected static properties up the inheritance chain; otherwise, only properties declared on <paramref name="t"/> are considered.
    /// </param>
    /// <returns><c>true</c> if the property was set successfully; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="t"/> or <paramref name="propertyName"/> is <c>null</c>.</exception>
    public static bool SetStaticPropertyValue(this Type t, object value, string propertyName, bool flattenHierarchy)
    {
        ArgumentNullException.ThrowIfNull(t);
        ArgumentNullException.ThrowIfNullOrEmpty(propertyName);

        BindingFlags flags = flattenHierarchy ? (AnyStatic | BindingFlags.FlattenHierarchy) : AnyStatic;
        return SetPropertyValue(t, null, value, propertyName, flags);
    }

    #endregion // Accessing_static_properties_Shallow_Scope

    #region Accessing_instance_property_value_Shallow_Scope

    /// <summary>
    /// Gets the value of an instance property from the specified <paramref name="obj"/>.
    /// </summary>
    /// <remarks>
    /// Unlike static property accessors, this method automatically considers public and protected properties declared in base classes without requiring <c>BindingFlags.FlattenHierarchy</c>. 
    /// It does not return private properties declared in base classes; for those, use <see cref="GetAllProperties(Type, BindingFlags)"/>.
    /// </remarks>
    /// <param name="obj">The object instance whose property value is retrieved.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The value of the instance property, or <c>null</c> if not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> or <paramref name="propertyName"/> is <c>null</c>.</exception>
    public static object? GetInstancePropertyValue(this object obj, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNullOrEmpty(propertyName);

        return GetPropertyValue(obj.GetType(), obj, propertyName, AnyInstance);
    }

    /// <summary>
    /// Sets the value of an instance property on the specified <paramref name="obj"/>.
    /// </summary>
    /// <remarks>
    /// This method considers public and protected instance properties declared in base classes automatically, without requiring <c>BindingFlags.FlattenHierarchy</c>. 
    /// Private properties declared in base classes are not accessed; use <see cref="GetAllProperties(Type, BindingFlags)"/> to access those.
    /// </remarks>
    /// <param name="obj">The object instance on which to set the property value.</param>
    /// <param name="value">The value to assign to the property.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns><c>true</c> if the property was set successfully; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> or <paramref name="propertyName"/> is <c>null</c>.</exception>
    public static bool SetInstancePropertyValue(this object obj, object value, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNullOrEmpty(propertyName);

        return SetPropertyValue(obj.GetType(), obj, value, propertyName, AnyInstance);
    }
    #endregion // Accessing_instance_property_value_Shallow_Scope

    #region Accessing_instance_property_value_Full_Scope

    /// <summary>
    /// Gets the value of an instance property from the specified <paramref name="obj"/> by name, searching the entire inheritance hierarchy including private properties.
    /// Throws <see cref="InvalidOperationException"/> if the property is not found or multiple properties with the same name exist.
    /// </summary>
    /// <typeparam name="T">The expected type of the property value.</typeparam>
    /// <param name="obj">The object instance whose property value is retrieved.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The value of the property cast to <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> or <paramref name="propertyName"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if no matching property is found or multiple matches exist.</exception>
    public static T? GetInstancePropertyValueEx<T>(this object obj, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj);
        PropertyInfo info = GetInstancePropertyEx<T>(obj.GetType(), propertyName);
        object? result = info.GetValue(obj, null);

        return (T?)result;
    }

    /// <summary>Sets the value of property in given object obj. Throws InvalidOperationException if there is no
    /// such property, or if there are more than one such properties.</summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    ///
    /// <typeparam name="T"> The assumed type of the property. </typeparam>
    /// <param name="obj"> The object whose property value is being set. </param>
    /// <param name="value"> The value being set. </param>
    /// <param name="propertyName"> The property name. </param>
    public static void SetInstancePropertyValueEx<T>(this object obj, T? value, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNullOrEmpty(propertyName);

        PropertyInfo info = GetInstancePropertyEx<T>(obj.GetType(), propertyName);
        info.SetValue(obj, value, null);
    }
    #endregion // Accessing_instance_property_value_Full_Scope

    #region Accessing_PropertyInfo_Full_Scope

    /// <summary>Get all properties of a given type, including type predecessors, and including their private
    /// properties.</summary>
    ///
    /// <remarks>To get private properties declared in the base class, one cannot just simply call
    /// type.GetProperties, even with BindingFlags.FlattenHierarchy, that's why this method exists. <br/></remarks>
    ///
    /// <param name="t"> The type whose properties will be enumerated. </param>
    /// <param name="flags"> A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the
    /// search is conducted. </param>
    ///
    /// <returns> Resulting sequence of PropertyInfo that can be iterated.</returns>
    public static IEnumerable<PropertyInfo> GetAllProperties(this Type t, BindingFlags flags)
    {
        List<PropertyInfo> result;

        if (t is null)
        {
            throw new ArgumentNullException(nameof(t));
        }
        else if (t == typeof(object))
        {
            result = [];
        }
        else
        {
            result = t!.BaseType!.GetAllProperties(flags).ToList();

            // To avoid duplicate properties from base types, 
            // restrict to those declared only on this type using BindingFlags.DeclaredOnly
            result.AddRange(t.GetProperties(flags | BindingFlags.DeclaredOnly));
        }

        return result;
    }

    /// <summary>Get all properties of a given type, given a property name propertyName, include type predecessors,
    /// and including their private properties, that have the given property name.</summary>
    ///
    /// <param name="t"> The type whose properties will be enumerated. </param>
    /// <param name="propertyName"> The property name the returned properties must match to. </param>
    /// <param name="flags"> A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the
    /// search is conducted. </param>
    ///
    /// <returns> Resulting sequence of PropertyInfo that can be iterated.</returns>
    public static IEnumerable<PropertyInfo> GetAllProperties(
        this Type t,
        string propertyName,
        BindingFlags flags)
    {
        IEnumerable<PropertyInfo> temp = GetAllProperties(t, flags);
        IEnumerable<PropertyInfo> result = (from f in temp where (0 == string.CompareOrdinal(propertyName, f.Name)) select f);
#if DEBUG
        // for possible debugging purpose
        result = result.ToList();
#endif // DEBUG

        return result;
    }


    /// <summary>Get all instance properties of a given type, include type predecessors, and including their
    /// private properties, that have the given property name.</summary>
    ///
    /// <param name="t"> The type whose properties will be enumerated. </param>
    /// <param name="propertyName"> The property name the returned properties should match to. </param>
    ///
    /// <returns> Resulting sequence of PropertyInfo that can be iterated.</returns>
    public static IEnumerable<PropertyInfo> GetAllInstanceProperties(this Type t, string propertyName)
    {
        return GetAllProperties(t, propertyName, AnyInstance);
    }

    /// <summary>Get all instance properties of a given type, include type predecessors, and including their
    /// private properties, that have the given property name, and the type of the property is either equal to
    /// type specified by <typeparamref name="T"/>, or is derived from it.</summary>
    ///
    /// <typeparam name="T"> The type of the property. </typeparam>
    /// <param name="t"> The type whose properties will be enumerated. </param>
    /// <param name="propertyName"> The property name the returned properties should match to. </param>
    ///
    /// <returns> Resulting sequence of PropertyInfo that can be iterated.</returns>
    public static IEnumerable<PropertyInfo> GetAllInstancePropertiesEx<T>(this Type t, string propertyName)
    {
        IEnumerable<PropertyInfo> temp = GetAllInstanceProperties(t, propertyName);
        IEnumerable<PropertyInfo> result = temp.Where(p => typeof(T).IsAssignableFrom(p.PropertyType));
#if DEBUG
        // for possible debugging purpose
        result = result.ToList();
#endif // DEBUG

        return result;
    }


    /// <summary>Returns a single property of a given type, include type predecessors, and including their private
    /// properties, that has the given property name. Throws InvalidOperationException if there is no such
    /// property, or if there are more than one such properties.</summary>
    ///
    /// <param name="t"> The type whose properties will be enumerated. </param>
    /// <param name="propertyName"> The property name the returned property name must match. </param>
    ///
    /// <returns> The <see cref="PropertyInfo"/> describing the specified property.</returns>
    public static PropertyInfo GetInstanceProperty(this Type t, string propertyName)
    {
        return GetAllInstanceProperties(t, propertyName).Single();
    }

    /// <summary>Returns a single property of a given type, include type predecessors, and including their private
    /// properties, that has the given property name, and the type of the property is either equal to type
    /// specified by <typeparamref name="T"/>, or is derived from it. Throws InvalidOperationException if there
    /// is no such property, or if there are more than one such properties.</summary>
    ///
    /// <typeparam name="T"> The type of the property. </typeparam>
    /// <param name="t"> The type whose properties will be enumerated. </param>
    /// <param name="propertyName"> The property name the returned property name must match. </param>
    ///
    /// <returns> The <see cref="PropertyInfo"/> describing the specified property.</returns>
    public static PropertyInfo GetInstancePropertyEx<T>(this Type t, string propertyName)
    {
        return GetAllInstancePropertiesEx<T>(t, propertyName).Single();
    }
    #endregion // Accessing_PropertyInfo_Full_Scope
    #endregion // Public Methods

    #region Private Methods

    /// <summary>Get the value of property in given object obj, or of the static property if the object obj is
    /// null. If the object obj is non-null, the type of the object must be t.</summary>
    ///
    /// <remarks> Implementation helper, called by GetInstancePropertyValue and GetStaticPropertyValue.</remarks>
    ///
    /// <param name="t"> . </param>
    /// <param name="obj"> . </param>
    /// <param name="propertyName"> . </param>
    /// <param name="bindingAttr"> . </param>
    ///
    /// <returns> The property value.</returns>
    private static object? GetPropertyValue(
        Type t,
        object? obj,
        string propertyName,
        BindingFlags bindingAttr)
    {
        PropertyInfo? info;
        object? result = null;

        if (null != (info = t.GetProperty(propertyName, bindingAttr)))
        {
            result = info.GetValue(obj, null);
        }

        return result;
    }

    /// <summary>Set the value of property in given object obj, or of the static property if the object obj is
    /// null. If the object obj is non-null, the type of the object must be <paramref name="t"/>.</summary>
    ///
    /// <param name="t"> . </param>
    /// <param name="obj"> . </param>
    /// <param name="value"> . </param>
    /// <param name="propertyName"> . </param>
    /// <param name="bindingAttr"> . </param>
    ///
    /// <returns> True on success, false on failure.</returns>
    private static bool SetPropertyValue(
        Type t,
        object? obj,
        object value,
        string propertyName,
        BindingFlags bindingAttr)
    {
        PropertyInfo? info;
        bool result = false;

        if (null != (info = t.GetProperty(propertyName, bindingAttr)))
        {
            info.SetValue(obj, value, null);
            result = true;
        }

        return result;
    }
    #endregion // Private Methods
}
#pragma warning restore IDE0305
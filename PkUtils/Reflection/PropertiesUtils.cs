/***************************************************************************************************************
*
* FILE NAME:   .\Reflection\PropertiesUtils.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The class PropertiesUtils
*
**************************************************************************************************************/

// Ignore Spelling: Utils
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
    ///
    /// <value> The non public.</value>
    private const BindingFlags AnyInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;


    /// <summary> A bit-flag combination used when searching for static properties.</summary>
    ///
    /// <value> The non public.</value>
    private const BindingFlags AnyStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    #endregion // Private Fields

    #region Public Methods

    #region Accessing_static_properties_Shallow_Scope

    /// <summary> Get the value of static property in given class of type <paramref name="t"/>.</summary>
    ///
    /// <remarks>This method does not access any properties declared in a base class. To achieve that, use the
    /// overloaded
    /// <see cref="GetStaticPropertyValue(Type, string, bool)"/> extension method.</remarks>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    ///
    /// <param name="t"> The type the required static property belongs to. </param>
    /// <param name="propertyName"> The name of the static property. </param>
    ///
    /// <returns> The value of found static property.</returns>
    public static object? GetStaticPropertyValue(this Type t, string propertyName)
    {
        return GetStaticPropertyValue(t, propertyName, false);
    }


    /// <summary> Get the value of static property in given class of type <paramref name="t"/>.</summary>
    ///
    /// <remarks>Depending on the argument <paramref name="flattenHierarchy"/> value, the method may access non-
    /// private properties of the base type. Note this will NEVER return private properties declared in the base
    /// class. To achieve that, one must to use the
    /// <see cref="GetAllProperties(Type, System.Reflection.BindingFlags)"/> extension method.<br/></remarks>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    ///
    /// <param name="t"> The type the required static property belongs to. </param>
    /// <param name="propertyName"> The name of the static property. </param>
    /// <param name="flattenHierarchy"> If true, specifies that public and protected static properties of base
    /// classes up the hierarchy should be returned. Otherwise, properties declared in base classes are not
    /// returned. </param>
    ///
    /// <returns> The value of found static property.</returns>
    public static object? GetStaticPropertyValue(this Type t, string propertyName, bool flattenHierarchy)
    {
        ArgumentNullException.ThrowIfNull(t);
        ArgumentNullException.ThrowIfNull(propertyName);

        BindingFlags eFlags = AnyStatic;
        if (flattenHierarchy)
        {
            eFlags |= BindingFlags.FlattenHierarchy;
        }

        return GetPropertyValue(t, null, propertyName, eFlags);
    }


    /// <summary> Set the value of static property in given class of type <paramref name="t"/>.</summary>
    ///
    /// <remarks>This method does not access any properties declared in a base class. To achieve that, use the
    /// overloaded
    /// <see cref="SetStaticPropertyValue(Type, object, string, bool)"/> extension method.</remarks>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when either the <paramref name="t"/>
    /// or the <paramref name="propertyName"/> parameter is null. </exception>
    ///
    /// <param name="t"> The type the given static property belongs to. </param>
    /// <param name="value"> The value being assigned to the property. </param>
    /// <param name="propertyName"> Then static property name. </param>
    ///
    /// <returns> True on success, false on failure.</returns>
    public static bool SetStaticPropertyValue(this Type t, object value, string propertyName)
    {
        return SetStaticPropertyValue(t, value, propertyName, false);
    }


    /// <summary> Set the value of static property in given class of type <paramref name="t"/>.</summary>
    ///
    /// <remarks>Depending on the argument <paramref name="flattenHierarchy"/> value, the method may access non-
    /// private properties of the base type. Note this will NEVER access private properties declared in the base
    /// class. To achieve that, one must to use the
    /// <see cref="GetAllProperties(Type, System.Reflection.BindingFlags)"/> extension method.<br/></remarks>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when either the <paramref name="t"/>
    /// or the <paramref name="propertyName"/> parameter is null. </exception>
    ///
    /// <param name="t"> The type the given static property belongs to. </param>
    /// <param name="value"> The value being assigned to the property. </param>
    /// <param name="propertyName"> Then static property name. </param>
    /// <param name="flattenHierarchy"> If true, specifies that public and protected static properties of base
    /// classes up the hierarchy should be accessed. Otherwise, properties declared in base classes are not
    /// accessed. </param>
    ///
    /// <returns> True on success, false on failure.</returns>
    public static bool SetStaticPropertyValue(
        this Type t,
        object value,
        string propertyName,
        bool flattenHierarchy)
    {
        ArgumentNullException.ThrowIfNull(t);
        ArgumentNullException.ThrowIfNull(propertyName);

        BindingFlags eFlags = AnyStatic;
        if (flattenHierarchy)
        {
            eFlags |= BindingFlags.FlattenHierarchy;
        }

        return SetPropertyValue(t, null, value, propertyName, eFlags);
    }
    #endregion // Accessing_static_properties_Shallow_Scope

    #region Accessing_instance_property_value_Shallow_Scope
    /// <summary> Get the value of property in given object <paramref name="obj"/>.</summary>
    ///
    /// <remarks>Unlike with the analogical method <see cref="GetStaticPropertyValue(Type, string)"/>, this
    /// method can access public and protected properties declared in a base class, and there is no need
    /// specifying BindingFlags.FlattenHierarchy. <br/>
    /// 
    /// Note this will NOT return private properties declared in the base class, and one must to use the
    /// <see cref="GetAllProperties(Type, System.Reflection.BindingFlags)"/> extension method to do
    /// that.<br/></remarks>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when either the <paramref name="obj"/>
    /// or the <paramref name="propertyName"/> parameter is null. </exception>
    ///
    /// <param name="obj"> The object whose property value is retrieved. </param>
    /// <param name="propertyName"> The property name. </param>
    ///
    /// <returns> The instance property value.</returns>
    public static object? GetInstancePropertyValue(this object obj, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(propertyName);

        return GetPropertyValue(obj.GetType(), obj, propertyName, AnyInstance);
    }

    /// <summary> Sets the value of property in given object <paramref name="obj"/>.</summary>
    ///
    /// <remarks>Unlike with the analogical method
    /// <see cref="SetStaticPropertyValue(Type, object, string)"/>, this method can access public and
    /// protected properties declared in a base class, and there is no need specifying
    /// BindingFlags.FlattenHierarchy. The reason for that is that static and non-static properties are handled
    /// differently by the
    /// <a href="http://msdn.microsoft.com/en-us/library/system.type.getproperty(v=vs.110).aspx">
    /// Type.GetProperty Method</a> <br/>
    /// 
    /// Note this will NOT access private properties declared in the base class, and one must to use the
    /// <see cref="GetAllProperties(Type, System.Reflection.BindingFlags)"/>
    /// extension method to do that.<br/></remarks>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    ///
    /// <param name="obj"> The object the required property belongs to. </param>
    /// <param name="value"> The value being assigned to the property. </param>
    /// <param name="propertyName"> Then property name. </param>
    ///
    /// <returns> True on success, false on failure.</returns>
    public static bool SetInstancePropertyValue(this object obj, object value, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(propertyName);

        return SetPropertyValue(obj.GetType(), obj, value, propertyName, AnyInstance);
    }
    #endregion // Accessing_instance_property_value_Shallow_Scope

    #region Accessing_instance_property_value_Full_Scope
    /// <summary>Get the value of property in given object obj, specified by property name. Is searching all
    /// properties including the object type predecessors, and including their private properties. Throws
    /// InvalidOperationException if there is no such property, or if there are more than one such properties.</summary>
    ///
    /// <remarks>Throws an exception if there is no such property, or if there are more than one such properties.</remarks>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    ///
    /// <typeparam name="T"> The assumed type of the property. </typeparam>
    /// <param name="obj"> The object whose property value is retrieved. </param>
    /// <param name="propertyName"> The property name. </param>
    ///
    /// <returns> The property value.</returns>
    public static T? GetInstancePropertyValueEx<T>(this object obj, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(obj);
        PropertyInfo info = GetInstancePropertyEx<T>(obj.GetType(), propertyName);
        object? result = info.GetValue(obj, null);

        return (result != null) ? (T)result : default;
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
        ArgumentNullException.ThrowIfNull(propertyName);

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
        return GetAllProperties(t, propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
        bool bRes = false;

        if (null != (info = t.GetProperty(propertyName, bindingAttr)))
        {
            info.SetValue(obj, value, null);
            bRes = true;
        }

        return bRes;
    }
    #endregion // Private Methods
}
#pragma warning restore IDE0305
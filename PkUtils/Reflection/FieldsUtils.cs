/***************************************************************************************************************
*
* FILE NAME:   .\Reflection\FieldsUtils.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The class FieldsUtils
*
**************************************************************************************************************/

// Ignore Spelling: bitmask, Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace PK.PkUtils.Reflection;

/// <summary>
/// Contains useful reflection-implemented utilities accessing fields.
/// </summary>
/// <seealso cref="EventsUtils"/>
/// <seealso cref="MethodsUtils"/>
/// <seealso cref="PropertiesUtils"/>
/// <seealso cref="ReflectionUtils"/>
[CLSCompliant(true)]
public static class FieldsUtils
{
    #region Fields

    /// <summary> A bit-flag combination used when searching for non-static fields.</summary>
    public const BindingFlags AnyInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary> A bit-flag combination used when searching for static fields.</summary>
    public const BindingFlags AnyStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    #endregion // Fields

    #region Public Methods

    #region Accessing_static_field_value_limited_depth

    /// <summary>
    /// Retrieves the value of a static field in the specified type.
    /// </summary>
    /// <remarks>
    /// This method does not access fields declared in a base class.  
    /// To include fields from base classes, use the overloaded  
    /// <see cref="GetStaticFieldValue(System.Type, string, bool)"/> method.
    /// </remarks>
    /// <param name="t">The type that declares the static field.</param>
    /// <param name="fieldName">The name of the static field.</param>
    /// <returns>The value of the static field.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="t"/> or <paramref name="fieldName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the specified field does not exist.
    /// </exception>
    public static object GetStaticFieldValue(this Type t, string fieldName)
    {
        return GetStaticFieldValue(t, fieldName, false);
    }

    /// <summary>
    /// Retrieves the value of a static field in the specified type.
    /// </summary>
    /// <remarks>
    /// Depending on <paramref name="flattenHierarchy"/>, this method may  
    /// access non-private static fields of base classes.  
    /// However, private fields declared in base classes are never accessed.  
    /// To access private fields, consider using <see cref="GetStaticFieldValueEx{T}"/>.  
    /// <br/>
    /// For further details, see:  
    /// <a href="http://stackoverflow.com/questions/1155529/c-gettype-getfields-problem">
    /// Stack Overflow: C# GetType().GetFields problem</a>.
    /// </remarks>
    /// <param name="t">The type that declares the static field.</param>
    /// <param name="fieldName">The name of the static field.</param>
    /// <param name="flattenHierarchy">
    /// If <see langword="true"/>, allows access to public and protected static  
    /// fields from base classes; otherwise, only fields declared in <paramref name="t"/>  
    /// are considered.
    /// </param>
    /// <returns>The value of the static field.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="t"/> or <paramref name="fieldName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException"> Thrown if the specified field does not exist.
    /// </exception>
    public static object GetStaticFieldValue(this Type t, string fieldName, bool flattenHierarchy)
    {
        ArgumentNullException.ThrowIfNull(t);
        ArgumentNullException.ThrowIfNull(fieldName);

        BindingFlags eFlags = AnyStatic;
        if (flattenHierarchy)
        {
            eFlags |= BindingFlags.FlattenHierarchy;
        }
        return GetFieldValue(t, null, fieldName, eFlags);
    }

    /// <summary>
    /// Sets the value of a static field in the specified type.
    /// </summary>
    /// <remarks> This method does not access fields declared in a base class.  
    /// To include fields from base classes, use the overloaded  
    /// <see cref="SetStaticFieldValue(System.Type, string, object, bool)"/> method. </remarks>
    /// <param name="t">The type that declares the static field.</param>
    /// <param name="fieldName">The name of the static field.</param>
    /// <param name="value">The value to assign to the field.</param>
    /// <returns>
    /// <see langword="true"/> if the field was successfully set; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="t"/> or <paramref name="fieldName"/> is <see langword="null"/>.
    /// </exception>
    public static bool SetStaticFieldValue(this Type t, string fieldName, object value)
    {
        return SetStaticFieldValue(t, fieldName, value, false);
    }

    /// <summary>
    /// Sets the value of a static field in the specified type.
    /// </summary>
    /// <remarks>
    /// Depending on <paramref name="flattenHierarchy"/>, this method may  
    /// access non-private static fields of base classes.  
    /// However, it never modifies private fields declared in base classes.  
    /// To access private static fields, consider using <see cref="SetStaticFieldValueEx{T}"/>.  
    /// <br/>
    /// For further details, see:  
    /// <a href="http://stackoverflow.com/questions/1155529/c-gettype-getfields-problem">
    /// Stack Overflow: C# GetType().GetFields problem</a>.
    /// </remarks>
    /// <param name="t">The type that declares the static field.</param>
    /// <param name="fieldName">The name of the static field.</param>
    /// <param name="value">The value to assign to the field.</param>
    /// <param name="flattenHierarchy">
    /// If <see langword="true"/>, allows access to public and protected static  
    /// fields from base classes; otherwise, only fields declared in <paramref name="t"/>  
    /// are considered.
    /// </param>
    /// <returns> true if the field was successfully set; otherwise, false. </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="t"/> or <paramref name="fieldName"/> is <see langword="null"/>.
    /// </exception>
    public static bool SetStaticFieldValue(
        this Type t,
        string fieldName,
        object value,
        bool flattenHierarchy)
    {
        ArgumentNullException.ThrowIfNull(t);
        ArgumentNullException.ThrowIfNull(fieldName);

        return SetFieldValue(
            t,
            null,
            fieldName,
            value,
            flattenHierarchy ? AnyStatic | BindingFlags.FlattenHierarchy : AnyStatic);
    }
    #endregion // Accessing_static_field_value_limited_depth

    #region Accessing_Static_Field_Value_Whole_Depth

    /// <summary>
    /// Retrieves the value of a static field from the specified type <paramref name="t"/>, 
    /// including fields declared in its base classes, even private ones.
    /// </summary>
    /// <typeparam name="T">The expected type of the field.</typeparam>
    /// <param name="t">The type containing the static field or a derived type.</param>
    /// <param name="fieldName">The name of the static field.</param>
    /// <returns>The value of the static field.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="t"/> or <paramref name="fieldName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the field does not exist or if multiple matching fields are found.
    /// </exception>
    public static T GetStaticFieldValueEx<T>(this Type t, string fieldName)
    {
        ArgumentNullException.ThrowIfNull(t);
        ArgumentNullException.ThrowIfNull(fieldName);

        FieldInfo fieldInfo = GetAllFieldsAssignableTo(t, typeof(T), AnyStatic, fieldName).Single();
        return GetFieldInfoValue<T>(fieldInfo, null);
    }

    /// <summary>
    /// Sets the value of a static field in the specified type <paramref name="t"/>.
    /// Searches all static fields in the type and its base classes.
    /// </summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="t">The type containing the static field or a derived type. Cannot be <see langword="null"/>.</param>
    /// <param name="fieldName">The name of the static field.</param>
    /// <param name="value">The value to assign to the field.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="t"/> or <paramref name="fieldName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the field does not exist or if multiple matching fields are found.
    /// </exception>
    /// <remarks>
    /// The method utilizes <see cref="GetAllFieldsAssignableFrom"/> to locate a field whose type is either
    /// exactly <typeparamref name="T"/> or a base type of <typeparamref name="T"/>. However, implicit 
    /// conversions (e.g., assigning an integer expression to a field of type <c>double</c>) are not supported. 
    /// This limitation is similar to the one in <see cref="SetInstanceFieldValueEx"/>.
    /// </remarks>
    public static void SetStaticFieldValueEx<T>(this Type t, string fieldName, T value)
    {
        ArgumentNullException.ThrowIfNull(t);
        ArgumentNullException.ThrowIfNull(fieldName);

        FieldInfo fieldInfo = GetAllFieldsAssignableFrom(t, typeof(T), AnyStatic, fieldName).Single();
        fieldInfo.SetValue(null, value);
    }
    #endregion // Accessing_Static_Field_Value_Whole_Depth

    #region Accessing_Instance_Field_Value_Limited_Depth

    /// <summary>
    /// Retrieves the value of an instance field from the specified <paramref name="obj"/>.
    /// </summary>
    /// <remarks>
    /// Unlike the method <see cref="GetStaticFieldValue(System.Type, string)"/>, which deals with static fields, 
    /// this method can access public and protected fields declared in base classes without the need for 
    /// <see cref="BindingFlags.FlattenHierarchy"/>. This difference arises because static and instance fields are 
    /// handled differently by the <a href="http://msdn.microsoft.com/en-us/library/4ek9c21e(v=vs.110).aspx">Type.GetField</a> method.
    /// <br />
    /// Note that this method does not retrieve private fields declared in the base class. To access private fields, 
    /// use the <see cref="GetAllFields(System.Type, System.Reflection.BindingFlags, Predicate{FieldInfo})"/> extension method.
    /// <br />
    /// For more details, refer to <a href="http://stackoverflow.com/questions/1155529/c-gettype-getfields-problem">
    /// StackOverflow: C# GetType().GetFields problem</a>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="obj"/> or <paramref name="fieldName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the specified field cannot be found.
    /// </exception>
    /// <param name="obj">The object whose field value is to be retrieved.</param>
    /// <param name="fieldName">The name of the field to retrieve.</param>
    /// <returns>The value of the instance field.</returns>
    public static object GetInstanceFieldValue(this object obj, string fieldName)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(fieldName);

        return GetFieldValue(obj.GetType(), obj, fieldName, AnyInstance);
    }

    /// <summary>
    /// Sets the value of an instance field in the specified <paramref name="obj"/>.
    /// </summary>
    /// <remarks>
    /// Unlike the method <see cref="SetStaticFieldValue(System.Type, string, object)"/>, which deals with static fields, 
    /// this method can access public and protected fields declared in base classes without the need for 
    /// <see cref="BindingFlags.FlattenHierarchy"/>. This distinction arises because static and instance fields are 
    /// handled differently by the <a href="http://msdn.microsoft.com/en-us/library/4ek9c21e(v=vs.110).aspx">Type.GetField</a> method.
    /// <br />
    /// Note that this method does not allow access to private fields declared in the base class. To modify private fields, 
    /// use the <see cref="GetAllFields(System.Type, System.Reflection.BindingFlags, Predicate{FieldInfo})"/> extension method.
    /// <br />
    /// For more information, refer to <a href="http://stackoverflow.com/questions/1155529/c-gettype-getfields-problem">
    /// StackOverflow: C# GetType().GetFields problem</a>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="obj"/> or <paramref name="fieldName"/> is <see langword="null"/>.
    /// </exception>
    /// <param name="obj">The object whose field value is to be set.</param>
    /// <param name="fieldName">The name of the field to set.</param>
    /// <param name="value">The value to assign to the field.</param>
    /// <returns><see langword="true"/> if the value was successfully set; otherwise, <see langword="false"/>.</returns>
    public static bool SetInstanceFieldValue(this object obj, string fieldName, object value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(fieldName);

        return SetFieldValue(obj.GetType(), obj, fieldName, value, AnyInstance);
    }
    #endregion // Accessing_Instance_Field_Value_Limited_Depth

    #region Accessing_instance_field_value_whole_depth
    /// <summary>Get the value of field in given object <paramref name="obj"/>, specified by field name. Is
    /// searching all fields including the object type predecessors, and including their private fields. Throws
    /// InvalidOperationException if there is no such field, or if there are more than one such fields.</summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    ///
    /// <typeparam name="T"> The assumed type of the field. The actual FieldType property must match that type, or
    /// derive from it. </typeparam>
    /// <param name="obj"> The object whose field value is retrieved. </param>
    /// <param name="fieldName"> The field name. </param>
    ///
    /// <returns> The field value.</returns>
    public static T GetInstanceFieldValueEx<T>(this object obj, string fieldName)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(fieldName);

        FieldInfo info = GetAllFieldsAssignableTo(obj.GetType(), typeof(T), AnyInstance, fieldName).Single();

        return GetFieldInfoValue<T>(info, obj);
    }

    /// <summary> Sets the value of field in given object <paramref name="obj"/>.</summary>
    ///
    /// <remarks>The method uses <see cref="GetAllFieldsAssignableFrom"/>, to search for fields with type either
    /// equal to type specified by with type either equal to type specified by <typeparamref name="T"/>, or is a
    /// base of it. However, this method cannot be used for assigning integer expression to the field of type
    /// double ( despite the fact that in C# one could assign integer to double ). The reason is, that it is C#
    /// is providing the implicit conversion from int to double. That's a language decision, not something which
    /// .NET will do for you, so from the .NET perspective, double isn't assignable from int. For more
    /// information, see for instance
    /// <see href="http://stackoverflow.com/questions/6275764/why-does-isassignablefrom-not-work-for-int-and-double">
    /// Why does IsAssignableFrom() not work for int and double?</see></remarks>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="obj"/> is null. </exception>
    /// <exception cref="InvalidOperationException"> Thrown if there is no such field, or if there are more than
    /// one such fields. </exception>
    ///
    /// <typeparam name="T"> The actual type of the <paramref name="value"/>. </typeparam>
    /// <param name="obj"> The object whose field value is being set. </param>
    /// <param name="fieldName"> The field name. </param>
    /// <param name="value"> The value being set. </param>
    public static void SetInstanceFieldValueEx<T>(this object obj, string fieldName, T value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(fieldName);

        FieldInfo info = GetAllFieldsAssignableFrom(obj.GetType(), typeof(T), AnyInstance, fieldName).Single();
        info.SetValue(obj, value);
    }
    #endregion // Accessing_instance_field_value_whole_depth

    #region Accessing_FieldInfo_whole_depth

    #region Accessing_FieldInfo_whole_depth_common_general

    /// <summary>Get all fields of a given type, including type predecessors, and including their private fields.</summary>
    ///
    /// <remarks>To get private fields declared in the base class, one cannot just simply call type.GetFields,
    /// even with BindingFlags.FlattenHierarchy, that's why this method exists. <br/>
    /// For the reasons behind, see
    /// <a href="http://stackoverflow.com/questions/1155529/c-gettype-getfields-problem">
    /// Stackoverflow: C# GetType().GetFields problem</a></remarks>
    ///
    /// <param name="t"> The type whose fields will be enumerated. Can't be null. </param>
    /// <param name="flags"> A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the
    /// search is conducted. </param>
    /// <param name="match"> Specifies the predicate found fields must match. May be null. </param>
    ///
    /// <returns> Resulting sequence of FieldInfo that can be iterated.</returns>
    public static IEnumerable<FieldInfo> GetAllFields(this Type t, BindingFlags flags, Predicate<FieldInfo> match)
    {
        ArgumentNullException.ThrowIfNull(t);

        IEnumerable<FieldInfo> result;

        if (t == typeof(object))
        {
            result = [];
        }
        else
        {
            result = t.BaseType.GetAllFields(flags, match);
            // in order to avoid duplicates, force BindingFlags.DeclaredOnly
            IEnumerable<FieldInfo> temp = t.GetFields(flags | BindingFlags.DeclaredOnly);
            if (match != null)
                temp = temp.Where(f => match(f));
            result = result.Concat(temp);
        }
        return result;
    }

    /// <summary>Get all fields of a given type, given a field name fieldName, include type predecessors, and
    /// including their private fields, that have the given field name.</summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="fieldName"/> is null. </exception>
    ///
    /// <param name="t"> The type whose fields will be enumerated. Can't be null. </param>
    /// <param name="fieldName"> The field name the returned fields must match to. </param>
    /// <param name="flags"> A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the
    /// search is conducted. </param>
    /// <param name="match"> (Optional) Specifies the predicate found fields must match. May be null. </param>
    ///
    /// <returns> Resulting sequence of FieldInfo that can be iterated.</returns>
    public static IEnumerable<FieldInfo> GetAllFields(
        this Type t,
        string fieldName,
        BindingFlags flags,
        Predicate<FieldInfo> match = null)
    {
        ArgumentNullException.ThrowIfNull(t);
        ArgumentNullException.ThrowIfNull(fieldName);

        // Predicate<FieldInfo> nameMatch = f => (0 == string.CompareOrdinal(fieldName, f.Name));
        bool NameMatch(FieldInfo f) => (0 == string.CompareOrdinal(fieldName, f.Name));
        IEnumerable<FieldInfo> result = GetAllFields(t, flags, NameMatch);
#if DEBUG
        // for possible debugging purpose
        List<FieldInfo> tempList = [.. result];
        Debug.Write(string.Format(CultureInfo.InvariantCulture, "GetAllFields returns {0} fields", tempList.Count));
#endif // DEBUG

        if (match != null)
        {
            result = result.Where(f => match(f));
#if DEBUG
            // for possible debugging purpose
            List<FieldInfo> resList = [.. result];
            Debug.Write(string.Format(CultureInfo.InvariantCulture, "GetAllFields returns {0} fields", resList.Count));
#endif // DEBUG
        }
        return result;
    }

    /// <summary>Get all fields of a given type, include type predecessors, and including their private fields,
    /// that have the given field name, and the type of the field is either equal to type specified by
    /// <paramref name="valType"/>, or is derived from it.
    /// Hence, the field value can be assigned to any variable of type <paramref name="valType"/></summary>
    ///
    /// <param name="t"> The type whose fields will be enumerated. </param>
    /// <param name="valType"> The type of the field. The FieldType property must match that type, or derive
    /// from it. </param>
    /// <param name="flags"> A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the
    /// search is conducted. </param>
    /// <param name="fieldName"> The field name the returned fields should match to. </param>
    ///
    /// <returns> Resulting sequence of FieldInfo that can be iterated.</returns>
    public static IEnumerable<FieldInfo> GetAllFieldsAssignableTo(this Type t, Type valType,
        BindingFlags flags, string fieldName)
    {
        return GetAllFields(t, fieldName, flags, f => valType.IsAssignableFrom(f.FieldType));
    }


    /// <summary>Get all fields of a given type, include type predecessors, and including their private fields,
    /// that have the given field name, and the type of the field is either equal to type specified by
    /// <paramref name="valType"/>, or is a base of it.
    /// Hence, any variable of type <paramref name="valType"/> can be assigned to such field value.</summary>
    ///
    /// <param name="t"> The type whose fields will be enumerated. </param>
    /// <param name="valType"> The type of the field. The FieldType property must match that type, or be a base
    /// of it. </param>
    /// <param name="flags"> A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the
    /// search is conducted. </param>
    /// <param name="fieldName"> The field name the returned fields should match to. </param>
    ///
    /// <returns> Resulting sequence of FieldInfo that can be iterated.</returns>
    public static IEnumerable<FieldInfo> GetAllFieldsAssignableFrom(this Type t, Type valType,
      BindingFlags flags, string fieldName)
    {
        return GetAllFields(t, fieldName, flags, f => f.FieldType.IsAssignableFrom(valType));
    }
    #endregion // Accessing_FieldInfo_whole_depth_common_general

    #region Accessing_FieldInfo_whole_depth_instance_fields
    /// <summary>Get all instance fields of a given type, include type predecessors, and including their private
    /// fields, that have the given field name.</summary>
    ///
    /// <param name="t"> The type whose fields will be enumerated. </param>
    /// <param name="fieldName"> The field name the returned fields should match to. </param>
    ///
    /// <returns> Resulting sequence of FieldInfo that can be iterated.</returns>
    public static IEnumerable<FieldInfo> GetAllInstanceFields(this Type t, string fieldName)
    {
        return GetAllFields(t, fieldName, AnyInstance);
    }

    /// <summary>Get all instance fields of a given type, include type predecessors, and including their private
    /// fields, that have the given field name, and the type of the field is either equal to type specified by
    /// <typeparamref name="T"/>, or is derived from it.</summary>
    ///
    /// <typeparam name="T"> The type of the field. The FieldType property must match that type, or derive from
    /// it. </typeparam>
    /// <param name="t"> The type whose fields will be enumerated. </param>
    /// <param name="fieldName"> The field name the returned fields should match to. </param>
    ///
    /// <returns> Resulting sequence of FieldInfo that can be iterated.</returns>
    public static IEnumerable<FieldInfo> GetAllInstanceFieldsEx<T>(this Type t, string fieldName)
    {
        IEnumerable<FieldInfo> temp = GetAllFields(t, fieldName, AnyInstance, f => typeof(T).IsAssignableFrom(f.FieldType));
        IEnumerable<FieldInfo> result = temp.Where(f => typeof(T).IsAssignableFrom(f.FieldType));
#if DEBUG
        // for possible debugging purpose
        var tempList = temp.ToList();
        var resList = result.ToList();

        Debug.Write($"Local variable tempList contains {tempList.Count} fields");
        Debug.Write(string.Format(CultureInfo.InvariantCulture, "GetAllInstanceFieldsEx returns {0} fields",
          resList.Count));
#endif // DEBUG

        return result;
    }

    /// <summary>Returns a single instance field of a given type, include type predecessors, and including their
    /// private fields, that has the given field name. Throws InvalidOperationException if there is no such field,
    /// or if there are more than one such fields.</summary>
    ///
    /// <param name="t"> The type whose fields will be enumerated. </param>
    /// <param name="fieldName"> The field name the returned field name must match. </param>
    ///
    /// <returns> The <see cref="FieldInfo "/> describing the specified field.</returns>
    public static FieldInfo GetInstanceField(this Type t, string fieldName)
    {
        return GetAllInstanceFields(t, fieldName).Single();
    }
    #endregion // Accessing_FieldInfo_whole_depth_instance_fields

    #region Accessing_FieldInfo_whole_depth_static_fields
    /// <summary>Get all static fields of a given type, include type predecessors, and including their private
    /// fields, that have the given field name.</summary>
    ///
    /// <param name="t"> The type whose fields will be enumerated. </param>
    /// <param name="fieldName"> The field name the returned fields should match to. </param>
    ///
    /// <returns> Resulting sequence of FieldInfo that can be iterated.</returns>
    public static IEnumerable<FieldInfo> GetAllStaticFields(this Type t, string fieldName)
    {
        return GetAllFields(t, fieldName, AnyStatic);
    }

    /// <summary>Get all static fields of a given type, include type predecessors, and including their private
    /// fields, that have the given field name, and the type of the field is either equal to type specified by
    /// <typeparamref name="T"/>, or is derived from it.</summary>
    ///
    /// <typeparam name="T"> The type of the field. The FieldType property must match that type, or derive from
    /// it. </typeparam>
    /// <param name="t"> The type whose fields will be enumerated. </param>
    /// <param name="fieldName"> The field name the returned fields should match to. </param>
    ///
    /// <returns> Resulting sequence of FieldInfo that can be iterated.</returns>
    public static IEnumerable<FieldInfo> GetAllStaticFieldsEx<T>(this Type t, string fieldName)
    {
        IEnumerable<FieldInfo> temp = GetAllStaticFields(t, fieldName);
        IEnumerable<FieldInfo> result = temp.Where(f => typeof(T).IsAssignableFrom(f.FieldType));
#if DEBUG
        List<FieldInfo> resList = [.. result];
        Debug.Write(string.Format(CultureInfo.InvariantCulture, "GetAllStaticFieldsEx returns {0} fields",
          resList.Count));
#endif // DEBUG

        return result;
    }

    /// <summary>Returns a single static field of a given type, include type predecessors, and including their
    /// private fields, that has the given field name. Throws InvalidOperationException if there is no such field,
    /// or if there are more than one such fields.</summary>
    ///
    /// <param name="t"> The type whose fields will be enumerated. </param>
    /// <param name="fieldName"> The field name the returned field name must match. </param>
    ///
    /// <returns> The <see cref="FieldInfo "/> describing the specified field.</returns>
    public static FieldInfo GetStaticField(this Type t, string fieldName)
    {
        return GetAllStaticFields(t, fieldName).Single();
    }
    #endregion // Accessing_FieldInfo_whole_depth_static_fields
    #endregion // Accessing_FieldInfo_whole_depth

    #region Auxiliary_Methods
    /// <summary>
    /// Extension method for <see cref="FieldInfo"/> that retrieves the value of a specified field from an object instance
    /// and casts it to the specified type <typeparamref name="T"/>. If the field is static, <paramref name="obj"/> can be null.
    /// </summary>
    /// <typeparam name="T">The type to which the field value will be cast.</typeparam>
    /// <param name="f">The <see cref="FieldInfo"/> representing the field to retrieve.</param>
    /// <param name="obj">The object instance whose field value is to be retrieved. If the field is static, this can be null.</param>
    /// <returns>The value of the field, cast to type <typeparamref name="T"/>, or the default value of <typeparamref name="T"/> if not found.</returns>
    public static T GetFieldInfoValue<T>(this FieldInfo f, object obj)
    {
        return f?.GetValue(obj) is T value ? value : default;
    }
    #endregion // Auxiliary_Methods

    #region Private Methods

    /// <summary>
    /// Retrieves the value of a field in the given object <paramref name="obj"/> or a static field if <paramref name="obj"/> is null.
    /// If <paramref name="obj"/> is non-null, its type must match <paramref name="t"/>.
    /// </summary>
    /// <remarks> This is a helper method used by GetInstanceFieldValue and GetStaticFieldValue. </remarks>
    /// <exception cref="InvalidOperationException">Thrown if the field with the specified name does not exist in the type.</exception>
    /// <param name="t">The <see cref="Type"/> of the object from which to retrieve the field.</param>
    /// <param name="obj">The object from which to retrieve the field value, or null for static fields.</param>
    /// <param name="fieldName">The name of the field to retrieve.</param>
    /// <param name="bindingAttr">Binding flags to control the search for the field.</param>
    /// <returns>The value of the field.</returns>
    private static object GetFieldValue(Type t, object obj, string fieldName, BindingFlags bindingAttr)
    {
        FieldInfo field = t.GetField(fieldName, bindingAttr);
        if (field != null)
        {
            return GetFieldInfoValue<object>(field, obj);
        }
        else
        {
            throw new InvalidOperationException($"No field '{fieldName}' found in type '{t}'.");
        }
    }

    /// <summary>
    /// Sets the value of a field in the given object <paramref name="obj"/>, or a static field if <paramref name="obj"/> is null.
    /// If <paramref name="obj"/> is non-null, its type must match <paramref name="t"/>.
    /// </summary>
    /// <param name="t">The <see cref="Type"/> of the object containing the field.</param>
    /// <param name="obj">The object whose field value will be set, or null for static fields.</param>
    /// <param name="fieldName">The name of the field to set.</param>
    /// <param name="value">The value to set the field to.</param>
    /// <param name="bindingAttr">Binding flags to control the search for the field.</param>
    /// <returns>True if the field value was successfully set, otherwise false.</returns>
    private static bool SetFieldValue(Type t, object obj, string fieldName, object value, BindingFlags bindingAttr)
    {
        FieldInfo field = t.GetField(fieldName, bindingAttr);
        if (field != null)
        {
            field.SetValue(obj, value);
            return true;
        }
        return false;
    }
    #endregion // Private Methods
    #endregion // Methods
}

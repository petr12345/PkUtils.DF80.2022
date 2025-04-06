/***************************************************************************************************************
*
* FILE NAME:   .\Reflection\ReflectionUtils.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The class ReflectionUtils
*
**************************************************************************************************************/

// Ignore Spelling: fallback, Utils
//
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace PK.PkUtils.Reflection;

/// <summary>
/// Contains useful reflection-implemented utilities which does not fit into other class in this namespace.
/// </summary>
/// <seealso cref="EventsUtils"/>
/// <seealso cref="FieldsUtils"/>
/// <seealso cref="MethodsUtils"/>
/// <seealso cref="PropertiesUtils"/>
[CLSCompliant(true)]
public static class ReflectionUtils
{
    #region Public Methods

    /// <summary>
    /// Find the description for given type, if there is any. 
    /// This description on the type is represented by System.ComponentModel.DescriptionAttribute.
    /// </summary>
    /// <param name="t">The type whose attributes will be enumerated. Must not equal to null.</param>
    /// <returns>First found DescriptionAttribute, or null if there is none.</returns>
    public static DescriptionAttribute GetDescriptionForType(Type t)
    {
        ArgumentNullException.ThrowIfNull(t);

        object[] attributes = t.GetCustomAttributes(false);
        var describs = attributes.OfType<DescriptionAttribute>();
        return describs.FirstOrDefault();
    }

    /// <summary> Finds description of enum  value, from value of a description attribute. </summary>
    ///
    /// <typeparam name="T">  Generic type parameter. </typeparam>
    /// <param name="enumVal">  The enum value to act on. </param>
    /// <param name="fallbackToValue">  (Optional) True to fall-back to value of enum. </param>
    ///
    /// <returns>   The description. </returns>
    public static string GetDescription<T>(
        this T enumVal,
        bool fallbackToValue = true) where T : Enum
    {
        string asString = enumVal.ToString();
        FieldInfo fi = enumVal.GetType().GetField(asString);
        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        string result = null;

        if ((attributes != null) && (attributes.Length > 0))
            result = attributes[0].Description;
        else if (fallbackToValue)
            result = asString;

        return result;
    }

    /// <summary>
    /// A 'safe' way of getting a type of given object. For a null argument, 
    /// just returns null instead of throwing an exception.
    /// </summary>
    /// <param name="obj">The object whose type should be retrieved.</param>
    /// <returns>The type of given object, or null  if the argument <paramref name="obj"/> is null.</returns>
    /// <seealso cref="SafeGetTypeName"/>
    public static Type SafeGetType(this object obj)
    {
        return obj?.GetType();
    }

    /// <summary> A 'safe' way of getting a type name of given object. For a null argument,  just returns a
    /// string "&lt;null&gt;" instead of throwing an exception. </summary>
    ///
    /// <param name="obj"> The object whose type name should be retrieved. </param>
    ///
    /// <returns>  The name of type of given object. </returns>
    ///
    /// <seealso cref="SafeGetType"/>
    public static string SafeGetTypeName(this object obj)
    {
        return (obj == null) ? "<null>" : obj.GetType().Name;
    }
    #endregion // Public Methods
}

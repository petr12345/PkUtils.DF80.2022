// Ignore Spelling: Utils
//
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PK.PkUtils.Extensions;

/// <summary> Implements extension methods for types. </summary>
public static class TypeExtension
{
    #region Fields
    private const string _splitDott = ".";
    #endregion // Fields

    #region Public Methods

    /// <summary>
    /// A Type extension method that gets generic type name Works best for simple generics types, like List{string}.
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="type"/> is null. </exception>
    ///
    /// <param name="type"> The type to act on. Can‘t be null. </param>
    ///
    /// <returns> The generic type name. </returns>
    public static string GetGenericTypeName(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        string genericTypeName = type.Name;
        string result;

        if (type.IsGenericType)
        {
            int backtickIndex = genericTypeName.IndexOf('`');
            result = $"{genericTypeName[..backtickIndex]}<{type.GetGenericArguments().Select(arg => arg.Name).Join(",")}>";
        }
        else
        {
            result = genericTypeName;
        }
        return result;
    }

    /// <summary>
    /// A Type extension method that gets generic type full name. For non-generic type return its plain full name.
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when required argument<paramre1c name="type"/> is null. </exception>
    ///
    /// <param name="type"> The type to act on. Can't be null. </param>
    ///
    /// <returns> The generic type name. </returns>
    public static string GetGenericTypeFullName(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        string typeName = type.FullName;
        string result;

        if (type.IsGenericType)
        {
            int backtickIndex = typeName.IndexOf('`');
            result = $"{typeName[..backtickIndex]}<{type.GetGenericArguments().Select(arg => arg.Name).Join(",")}>";
        }
        else
        {
            result = typeName;
        }

        return result;
    }


    /// <summary> A Type exten51on method that gets default value. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when required argument<paramref name="type"/> is null. </exception >
    /// <param name="type"> The type to act on. Can't be null. </param>
    /// <returns> The default value. </returns>
    public static object GetDefaultValue(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        object result = null;

        if (type.IsValueType)
        {
            if (type.ContainsGenericParameters)
            {
                throw new ArgumentException(
                    $"The default value of this type cannot be determined, as the type '{type}' contains open generic parameters",
                nameof(type));
            }
            else
            {
                try
                {
                    result = Activator.CreateInstance(type);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(
                        $"The default value of this type cannot be determined, since Activator failed to create an instance",
                        nameof(type),
                        ex);
                }
            }

        }

        return result;
    }


    /// <summary> Checks if a class is derived from a generic class. </summary>
    ///
    /// <remarks> If generic classes are involved, one has to use this method 
    /// instead of simple methods like <see cref="Type.IsAssignableFrom"/> etc.,
    /// since they do not work in this case. For more info see the MSDN article
    /// <a href="http://msdn.microsoft.com/en-us/library/b8ytshk6.aspx">
    /// How to: Examine and Instantiate Generic Types with Reflection.</a> </remarks>
    ///
    /// <exception cref="ArgumentNullException">
    /// Thrown when at least one of supplied arguments is null.
    /// </exception>
    /// <exception cref="ArgumentException"> 
    /// Thrown when the argument <paramref name="genericT"/> is not a generic type. 
    /// </exception>
    ///
    /// <param name="checkedT"> The type being checked. </param>
    /// <param name="genericT"> The Type to compare with the checked Type 
    /// <paramref name="checkedT"/>. </param>
    ///
    /// <returns> Returns true If the <paramref name="checkedT"/> is a subclass 
    /// of a generic type <paramref name="checkedT"/>;  
    /// returns false otherwise. </returns>
    ///
    /// <seealso href="http://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class"> 
    /// Check if a class is derived from a generic class</seealso>
    public static bool IsSubclassOfRawGeneric(this Type checkedT, Type genericT)
    {
        ArgumentNullException.ThrowIfNull(checkedT);
        ArgumentNullException.ThrowIfNull(genericT);

        if (!genericT.IsGenericType)
        {
            string strErr = string.Format(CultureInfo.InvariantCulture,
              "The type '{0}' is not a generic type", genericT);
            throw new ArgumentException(strErr, nameof(genericT));
        }

        for (Type current = checkedT; current != typeof(object);)
        {
            Type comparedT = current.IsGenericType ? current.GetGenericTypeDefinition() : current;
            if (comparedT == genericT)
            {
                return true;
            }
            current = current.BaseType;
        }
        return false;
    }

    /// <summary>
    /// Figure-out whether the given type has the given interface 
    /// </summary>
    /// <param name="t"> The type being examined. Must not equal to null.</param>
    /// <param name="interfaceType">The type of the interface we are interested in.</param>
    /// <returns>True if the type <paramref name="t"/> supports the interface 
    /// <paramref name="interfaceType"/>, false otherwise.</returns>
    public static bool HasTheInterface(this Type t, Type interfaceType)
    {
        ArgumentNullException.ThrowIfNull(t);

        bool result = false;

        if (t.IsClass && !t.IsAbstract)
        {
            TypeFilter filter = new(delegate (Type ttp, object filterCriteria)
            {
                return (ttp == filterCriteria as Type);
            });

            Type[] itsInterfaces = t.FindInterfaces(filter, interfaceType);
            if (itsInterfaces.Length > 0)
            {
                result = true;
            }
        }
        return result;
    }

    /// <summary>
    /// For a given type, returns "human-readable" type name ( without namespace ).
    /// </summary>
    /// <param name="t">The type being converted to string.</param>
    /// <returns>For simple types, like System.Drawing.Point or System.Drawing.Rectangle, 
    /// returns their name without namespaces, like "Point" or "Rectangle". <br/>
    /// For more complex types, see the description of <see cref="TypeNameToReadable"/>,
    /// which is called by this method.
    /// </returns>
    public static string TypeToReadable(this Type t)
    {
        ArgumentNullException.ThrowIfNull(t);


        return TypeNameToReadable(t.ToString());
    }

    /// <summary> An object extension method that returns either object type 
    ///             or "null" for null value of <paramref name="obj"/>. 
    /// </summary>
    ///
    /// <param name="obj"> The object to act on. </param>
    ///
    /// <returns> A string containing "readable" object type. </returns>
    public static string ObjectTypeToReadable(this object obj)
    {
        return (obj != null) ? TypeToReadable(obj.GetType()) : "null";
    }

    /// <summary>
    /// A recursive utility method, called by <see cref="TypeToReadable"/>,
    /// which returns "human-readable" type name ( without namespace ).
    /// All actual string processing is done here.
    /// </summary>
    /// <param name="strTypeName"> A type name, that has been retrieved by the call t.ToString()</param>
    /// <returns>A type name with namespaces stripped-off.</returns>
    /// <remarks> Examples of input and output values:<br/>
    /// <list type="number">
    /// <item> The type <b>Point</b>: <br/>
    ///    a/ type name: "System.Drawing.Point" <br/>
    ///    b/ returned value: "Point" <br/>
    /// </item>
    /// <item> The type <b>List{Rectangle}</b>: <br/>
    ///    a/ type name: "System.Collections.Generic.List`1[System.Drawing.Rectangle]" <br/>
    ///    b/ returned value: "List`1[Rectangle]" <br/>
    /// </item>
    /// <item> The type <b>Singleton{DataContractSerializerAdapter{Rectangle}}</b>: <br/>
    ///    a/ type name: "PK.PkUtils.DataStructures.Singleton`1[PK.PkUtils.XmlSerialization.DataContractSerializerAdapter`1[System.Drawing.Rectangle]]" <br/>
    ///    b/ returned value: "Singleton`1[DataContractSerializerAdapter`1[Rectangle]]" <br/>
    /// </item>
    /// </list>
    /// </remarks>
    public static string TypeNameToReadable(string strTypeName)
    {
        ArgumentNullException.ThrowIfNull(strTypeName);

        string strRet = strTypeName;

        if (strRet.Contains(_splitDott))
        {
            // Be aware of the case of generics, that now contain their type argument 
            // in brackets, like
            // "PK.PkUtilsUI.Stack.MainStackedFormWrapper`1[PK.YourProject.ServerL1.UI.DemoServerFormEx]"
            // This is resolved by involving FindBracketGroup
            string strTemp = strRet;
            Group grp = FindBracketGroup(strTemp);

            if (null == grp)
            {
                // just remove namespace if there is any
                strRet = RemoveDottedNamespace(strTemp);
            }
            else
            { // remove namespace both outside the brackets and inside the brackets
                Debug.Assert(grp.Index > 0);
#pragma warning disable IDE0057 // Use range operator
                string strBeforeBrackets = strTemp.Substring(0, grp.Index - 1);
#pragma warning restore IDE0057 // Use range operator
                string strInBrackets = grp.Value;

                strBeforeBrackets = RemoveDottedNamespace(strBeforeBrackets);
                if (!strInBrackets.Contains('['))
                {
                    strInBrackets = RemoveDottedNamespace(strInBrackets);
                }
                else
                {
                    strInBrackets += "]";
                    strInBrackets = TypeNameToReadable(strInBrackets);
                }
                strRet = $"{strBeforeBrackets}[{strInBrackets}]";
            }
        }

        return strRet;
    }
    #endregion // Public Methods

    #region Private Methods

    /// <summary>
    /// Implementation helper; 
    /// from a string like "ABC.Foo.Anything.YouWant" returns "YouWant"
    /// </summary>
    /// <param name="strArg"></param>
    /// <returns></returns>
    private static string RemoveDottedNamespace(string strArg)
    {
        string[] arr = strArg.Split(_splitDott.ToCharArray());

        return arr.LastOrDefault();
    }

    /// <summary> Implementation helper; finds bracket pair inside a string. </summary>
    /// <param name="strArg"> A string that may contain nested brackets. </param>
    /// <returns> The found bracket group, or null. </returns>
    /// <seealso href="http://codeasp.net/blogs/raghav_khunger/microsoft-net/1888/c-regex-extract-the-text-between-square-brackets-without-returning-the-brackets-themselves"> 
    /// C# Regex: Extract the text between square brackets, without returning the brackets themselves.</seealso>
    private static Group FindBracketGroup(string strArg)
    {
        string pattern = @"(?<=\[)(.*?)(?=\])";
        Match match = Regex.Match(strArg, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        Group grp = null;

        if (match.Groups.Count > 1)
        {
            grp = match.Groups[1];
        }
        return grp;
    }
    #endregion // Private Methods
}
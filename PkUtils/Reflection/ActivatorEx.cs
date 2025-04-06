/***************************************************************************************************************
*
* FILE NAME:   .\Reflection\ActivatorEx.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class ActivatorEx
*
**************************************************************************************************************/


// Ignore Spelling: Utils
//
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using PK.PkUtils.Dump;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.Reflection;

/// <summary>
/// A replacement of the System.Activator functionality, with methods that 
/// either the Compact Framework does not include,
/// or in the Desktop Framework do not work sufficiently...
/// </summary>
/// 
/// <seealso href="http://social.msdn.microsoft.com/Forums/en/netfxcompact/thread/90a8918d-a5b0-4796-820f-93e251d09093">
/// Why is Activator.CreateInstance(Type, Object[]) missing from compact framework? </seealso>
public static class ActivatorEx
{
    /// <summary>
    /// Creates an instance of the type designated by the specified generic type parameter, using the constructor
    /// that best matches the specified parameters. The argument args may be null; in that case it simply
    /// delegates the functionality to Activator.CreateInstance(t); </summary>
    ///
    /// <typeparam name="T">  The type of object being created. </typeparam>
    /// <param name="args"> Arguments that are used for the found constructor of type <typeparamref name="T"/>
    ///   </param>
    ///
    /// <returns> New instance of <typeparamref name="T"/> </returns>
    public static T CreateInstance<T>(params object[] args) where T : class
    {
        return CreateInstance(typeof(T), args) as T;
    }

    /// <summary>
    /// Creates an instance of the type designated by the specified generic type parameter,
    /// using the constructor that best matches the specified parameters, including their types.
    /// This is overloaded method of CreateInstance. Besides the constructor argument values given by the 'args' argument,
    /// this overload allows you to specify the argument types as well. This is useful in case that one or more argument
    /// values are null, hence the exact argument type cannot be determined from the value itself.
    /// </summary>
    /// <typeparam name="T">The type of object being created</typeparam>
    /// <param name="argTypes"> An array of Type objects representing the number, order, and type of the
    ///   parameters for the constructor to get. </param>
    /// <param name="args"> Arguments that are used for the found constructor of type <typeparamref name="T"/>
    ///   </param>
    /// <returns>New instance of <typeparamref name="T"/></returns>
    public static T CreateInstance<T>(Type[] argTypes, object[] args) where T : class
    {
        return CreateInstance(typeof(T), argTypes, args) as T;
    }

    /// <summary>
    /// Creates an instance of the specified type using the constructor that best matches the specified
    /// parameters. The argument args may be null; in that case it simply delegates the functionality to
    /// Activator.CreateInstance(t); </summary>
    ///
    /// <remarks>
    /// The method delegates the call to overloaded CreateInstance after retrieving the argument types from their
    /// values. In case that one or more argument values are null, hence the exact argument type cannot be
    /// determined from the value itself, you should use the overloaded method with additional argument Type[]
    /// for argument types. </remarks>
    ///
    /// <param name="t">    The type of object being created. </param>
    /// <param name="args"> Arguments that are used for the found constructor of type
    ///   <paramref name="t"/> </param>
    ///
    /// <returns> New instance of object having type <paramref name="t"/> </returns>
    public static object CreateInstance(Type t, params object[] args)
    {
        object result;

        if (args != null)
        {
            var argTypes = args.Select(x => x?.GetType()).ToArray();
            result = CreateInstance(t, argTypes, args);
        }
        else
        {
            result = Activator.CreateInstance(t);
        }
        return result;
    }

    /// <summary>
    /// Creates an instance of the specified type using the constructor that best matches the specified
    /// parameters, including their types.  
    /// This is overloaded method of CreateInstance. Besides the constructor argument values given by the 'args'
    /// argument, this overload allows you to specify the argument types as well. This is useful in case that one
    /// or more argument values are null, hence the exact argument type cannot be determined from the value
    /// itself. </summary>
    ///
    /// <remarks>
    /// Unlike the more simple overload, the method assumes both arrays <paramref name="argTypes "/>and
    /// <paramref name="args "/>are not null, and their count match; otherwise it throws ArgumentNullException or
    /// ArgumentException. In case the method cannot find appropriate constructor, it throws
    /// MissingMethodException. </remarks>
    ///
    /// <param name="t">        The type of object being created. </param>
    /// <param name="argTypes"> An array of Type objects representing the number, order, and type of the
    ///   parameters for the constructor to get. </param>
    /// <param name="args">     Arguments that are used for the found constructor of type
    ///   <paramref name="t"/> </param>
    ///
    /// <returns> New instance of object having type <paramref name="t"/> </returns>
    public static object CreateInstance(Type t, Type[] argTypes, object[] args)
    {
        return CreateInstance(t, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, argTypes, args);
    }

    /// <summary>
    /// Creates an instance of the specified type using the constructor that best matches the specified
    /// parameters, including their types. </summary>
    ///
    /// <remarks>
    /// Throws ArgumentNullException when one of arguments
    /// <paramref name="t"/>
    /// or <paramref name="argTypes "/>or <paramref name="args"/>is null. <br/>
    /// Throws ArgumentException if  size of arrays <paramref name="argTypes "/> and
    /// <paramref name="args"/> does not match. </remarks>
    ///
    /// <exception cref="ArgumentException">      Thrown when one or more arguments have unsupported or illegal
    ///   values. </exception>
    /// <exception cref="MissingMethodException"> Thrown when a Missing Method error condition occurs. </exception>
    ///
    /// <param name="t">            The type of object being created. </param>
    /// <param name="bindingAttr"> A bitmask comprised of one or more BindingFlags that specify how the search
    ///   is conducted. </param>
    /// <param name="argTypes">    An array of Type objects representing the number, order, and type of the
    ///   parameters for the constructor to get. </param>
    /// <param name="args">        Arguments that are used for the found constructor of type
    ///   <paramref name="t"/> </param>
    ///
    /// <returns> New instance of object having type <paramref name="t"/> </returns>
    public static object CreateInstance(Type t, BindingFlags bindingAttr, Type[] argTypes, object[] args)
    {
        ArgumentNullException.ThrowIfNull(t);
        ArgumentNullException.ThrowIfNull(argTypes);
        ArgumentNullException.ThrowIfNull(args);

        if (argTypes.Length != args.Length)
            throw new ArgumentException("The count of arrays argTypes and args does not match", nameof(args));

        ConstructorInfo ci = t.GetConstructor(bindingAttr, null, argTypes, null);

        // error handling if no appropriate constructor found
        if (ci == null)
        {
            ConstructorInfo[] arrCtors = t.GetConstructors(bindingAttr);
            int nCount = arrCtors.Length;
            StringBuilder sbErr = new();

            sbErr.AppendFormat(CultureInfo.InvariantCulture,
              "Could not find appropriate constructor in type {0} with BindingFlags = <{1}>. ",
              t.TypeToReadable(), Enum.Format(typeof(BindingFlags), bindingAttr, "G"));


#if DEBUG
            if (nCount == 0)
            {
                sbErr.AppendFormat(CultureInfo.InvariantCulture, "This type has none such constructor.");
            }
            else
            {
                sbErr.AppendFormat(CultureInfo.InvariantCulture, "This type has {0} such constructors:{1}.", nCount, Environment.NewLine);
                foreach (ConstructorInfo c in arrCtors)
                {
                    sbErr.AppendFormat(CultureInfo.InvariantCulture, "----{0}{1}", ObjectDumper.Dump2Text(c, 1), Environment.NewLine);

                    ParameterInfo[] pList = c.GetParameters();
                    int nParamCount = pList.Length;
                    string strParInfo = ObjectDumper.Dump2Text(pList, 2);
                    sbErr.AppendFormat(CultureInfo.InvariantCulture, "{0} Parameters: {1}{2}", nParamCount, strParInfo, Environment.NewLine);
                }
            }
#endif // DEBUG

            throw new MissingMethodException(sbErr.ToString());
        }

        return ci.Invoke(args);
    }
}

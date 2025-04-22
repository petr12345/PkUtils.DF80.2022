// Ignore Spelling: bitmask, Stackoverflow, suma, Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;


#nullable enable

namespace PK.PkUtils.Reflection;

/// <summary>
/// Contains useful reflection-implemented utilities accessing methods.
/// </summary>
/// <seealso cref="EventsUtils"/>
/// <seealso cref="FieldsUtils"/>
/// <seealso cref="PropertiesUtils"/>
/// <seealso cref="ReflectionUtils"/>
public static class MethodsUtils
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
    #region Methods_execution_limited_depth
    #region Running_static_methods
    /// <summary> Run static method for given object type, method name and arguments.</summary>
    ///
    /// <exception cref="ArgumentNullException"> Passed when a supplied type <paramref name="t"/> is null. </exception>
    /// <exception cref="ArgumentException"> Passed when string <paramref name="methodName"/> is null or empty. </exception>
    ///
    /// <param name="t"> The type that implements the static method <paramref name="methodName"/>. </param>
    /// <param name="methodName"> Method name. </param>
    /// <param name="parameters"> Arguments of the method. </param>
    ///
    /// <returns> Result of method execution.</returns>
    public static object? RunStaticMethod(this Type t, string methodName, object[] parameters)
    {
        return RunMethod(null, t, methodName, null, parameters, AnyStatic);
    }


    /// <summary>Run static method for given object type <paramref name="t"/>, with method name
    /// <paramref name="methodName"/>, argument types <paramref name="argTypes"/>
    /// and arguments <paramref name="parameters"/>.</summary>
    ///
    /// <remarks>Argument types are needed if there are overloaded versions of the same method ( to avoid the
    /// exception AmbiguousMatchException )</remarks>
    ///
    /// <exception cref="ArgumentNullException"> Passed when a supplied type <paramref name="t"/> is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when string <paramref name="methodName"/> is null or empty. </exception>
    ///
    /// <param name="t"> The type that implements the static method <paramref name="methodName"/>. </param>
    /// <param name="methodName"> Name of the called method. </param>
    /// <param name="argTypes"> Types of formal arguments in the method declaration. </param>
    /// <param name="parameters"> Supported arguments for calling the method. </param>
    ///
    /// <returns> Result of method execution.</returns>
    public static object? RunStaticMethod(
        this Type t,
        string methodName,
        Type[] argTypes,
        object[] parameters)
    {
        return RunMethod(null, t, methodName, argTypes, parameters, AnyStatic);
    }
    #endregion // Running_static_methods

    #region Running_nonstatic_methods
    /// <summary>Run non-static method for given object instance <paramref name="objInstance"/>, method name
    /// <paramref name="methodName"/> and arguments <paramref name="parameters"/>.</summary>
    ///
    /// <remarks>Note this will NOT access private methods declared in the base class. One must either <br/>
    /// a/use some of the overloads GetAllMethods / GetAllInstanceMethods / GetInstanceMethod and use the
    /// retrieved MethodInfo to be able to access such private method.<br/>
    /// b/ call <see cref="RunInstanceMethodEx"/></remarks>
    ///
    /// <exception cref="ArgumentNullException"> Passed when the argument <paramref name="objInstance"/> is null. </exception>
    /// <exception cref="ArgumentException"> Passed when string <paramref name="methodName"/> is null or empty. </exception>
    ///
    /// <param name="objInstance"> The instance of object whose method will be called. </param>
    /// <param name="methodName"> Name of the called method. </param>
    /// <param name="parameters"> Supported arguments for calling the method. </param>
    ///
    /// <returns> Result of method execution.</returns>
    public static object? RunInstanceMethod(this object objInstance, string methodName, object[] parameters)
    {
        return RunInstanceMethod(objInstance, methodName, null, parameters);
    }


    /// <summary>Run non-static method for given object instance, method name, argument types and arguments.
    /// Argument types are needed if there are overloaded versions of the same method.</summary>
    ///
    /// <remarks>Note this will NOT access private methods declared in the base class. One must either <br/>
    /// a/use some of the overloads GetAllMethods / GetAllInstanceMethods / GetInstanceMethod and use the
    /// retrieved MethodInfo to be able to access such private method.<br/>
    /// b/ call <see cref="RunInstanceMethodEx"/></remarks>
    ///
    /// <exception cref="ArgumentNullException"> Passed when the argument <paramref name="objInstance"/> is null. </exception>
    /// <exception cref="ArgumentException"> Passed when string <paramref name="methodName"/> is null or empty. </exception>
    ///
    /// <param name="objInstance"> The instance of object whose method will be called. </param>
    /// <param name="methodName"> Name of the called method. </param>
    /// <param name="argTypes"> Types of formal arguments in the method declaration. </param>
    /// <param name="parameters"> Supported arguments for calling the method. If the called method has no arguments,
    /// the value of <paramref name="parameters"/> can be simply null. </param>
    ///
    /// <returns> Result of method execution.</returns>
    public static object? RunInstanceMethod(
        this object objInstance,
        string methodName,
        Type[]? argTypes,
        object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(objInstance);

        return RunMethod(objInstance, objInstance.GetType(), methodName, argTypes, parameters, AnyInstance);
    }


    /// <summary>Run non-static method for given object instance, method name, argument types and arguments.
    /// Argument types are needed if there are overloaded versions of the same method.</summary>
    ///
    /// <remarks>Note that unlike <see cref="RunInstanceMethod(object, string, Type[], object[])"/>, this method
    /// CAN access private methods declared in the base class.</remarks>
    ///
    /// <exception cref="ArgumentNullException"> Passed when the argument <paramref name="objInstance"/>
    /// is null. </exception>
    /// <exception cref="ArgumentException"> Passed when string <paramref name="methodName"/> is null or empty. </exception>
    ///
    /// <param name="objInstance"> The instance of object whose method will be called. </param>
    /// <param name="methodName"> Name of the called method. </param>
    /// <param name="argTypes"> Types of formal arguments in the method declaration. </param>
    /// <param name="parameters"> Supported arguments for calling the method. If the called method has no arguments,
    /// the value of <paramref name="parameters"/> can be simply null. </param>
    ///
    /// <returns> Result of method execution.</returns>
    public static object? RunInstanceMethodEx(
        this object objInstance,
        string methodName,
        Type[]? argTypes,
        object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(objInstance);

        return RunMethodEx(objInstance, objInstance.GetType(), methodName, argTypes, parameters, AnyInstance);
    }
    #endregion // Running_nonstatic_methods

    #region Running_specific_nonstatic_methods
    /// <summary> Performs the (syntactically impossible) call of base.base.MethodName() </summary>
    /// <remarks>   In C# 7.3. it is now possible to use delegate in constraints ('where TD : Delegate') </remarks>
    /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// <typeparam name="TD"> The type of delegate (type of method) that you want to call.
    ///                       In reality, your calling code may look like following
    /// <code>
    /// <![CDATA[
    /// public delegate void TArgumentLessDelegate();
    /// ...
    /// DUMMY.CallBaseBase<TArgumentLessDelegate>(this, "SelectNode");
    /// ]]>
    /// </code> </typeparam>
    /// <param name="obj"> The real object on which you want to make the call. Can't be null. </param>
    /// <param name="methodName"> The name of the method. </param>
    /// <param name="args"> An array of objects that are the arguments to pass to the method represented, or null,
    /// if the method represented does not require arguments. </param>
    /// <returns>   Returns the result of method invocation. </returns>
    public static object? CallBaseBase<TD>(
        this object obj,
        string methodName,
        params object[] args) where TD : Delegate
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentException.ThrowIfNullOrEmpty(methodName);

        // get the base base
        Type tBaseBase = obj!.GetType()!.BaseType!.BaseType!;

        // temporary make a, which is the base base class you want to call the method
        object? a = Activator.CreateInstance(tBaseBase);

        // Create your delegate using the method name with the instance 'a' of the base class         
        Delegate d = Delegate.CreateDelegate(typeof(TD), a!, methodName);

        /* Following commented code is the original one from the http://stackoverflow..., 
         * but it does not compile for general delegate
        TD am = d as TD;
        // Get the target of the delegate and set it to your object (this in most case)         
        am.GetType().BaseType.BaseType.GetField("_target", 
           BindingFlags.Instance | BindingFlags.NonPublic).SetValue(am, obj);
        // call the method using the delegate.         
        am();
        */
        FieldInfo? fi = d.GetType().BaseType!.BaseType!.GetField("_target",
          BindingFlags.Instance | BindingFlags.NonPublic);
        fi!.SetValue(d, obj);

        // call the method using the delegate.         
        return d.DynamicInvoke(args);
    }

    /// <summary> Performs the (syntactically impossible) call of base.base.MethodName() </summary>
    /// <remarks>   In C# 7.3. it is now possible to use delegate in constraints ('where TD : Delegate') </remarks>
    /// <typeparam name="TD"> The type of delegate (type of method) that you want to call. In reality, your calling
    /// code may look like following
    /// <code>
    /// <![CDATA[
    /// public delegate void TArgumentLessDelegate();
    /// ...
    /// DUMMY.CallBaseBase<TArgumentLessDelegate>(this, "SelectNode");
    /// ]]>
    /// </code> </typeparam>
    /// <typeparam name="TResult"> The type of the result value. </typeparam>
    /// <param name="obj"> The real object on which you want to make the call. </param>
    /// <param name="methodName"> The name of the method. </param>
    /// <param name="args"> An array of objects that are the arguments to pass to the method represented, or null,
    /// if the method represented does not require arguments. </param>
    /// <returns>   Returns the result of method invocation. </returns>
    public static TResult CallBaseBase<TD, TResult>(
        this object obj,
        string methodName,
        params object[] args) where TD : Delegate
    {
        return (TResult)CallBaseBase<TD>(obj, methodName, args)!;
    }
    #endregion // Running_specific_nonstatic_methods
    #endregion // Methods_execution_limited_depth

    #region Accessing_MethodInfo_whole_depth

    #region Accessing_static_methods
    /// <summary>Get all static methods of a given type, include type predecessors, and including their private
    /// methods, that have the given method name.</summary>
    ///
    /// <exception cref="ArgumentNullException"> Passed when argument <paramref name="t"/> is null. </exception>
    /// <exception cref="ArgumentException"> Passed when argument <paramref name="methodName"/> is null or empty. </exception>
    /// <exception cref="InvalidOperationException"> Passed from Enumerable.Single when there is more than one
    /// method matching the given criteria, or when there is no such method. </exception>
    ///
    /// <param name="t"> The type whose methods will be enumerated. </param>
    /// <param name="methodName"> The name that returned methods must match. </param>
    ///
    /// <returns> Resulting sequence of MethodInfo that can be iterated.</returns>
    public static IEnumerable<MethodInfo> GetAllStaticMethods(this Type t, string methodName)
    {
        return GetAllMethods(t, methodName, AnyStatic);
    }


    /// <summary>Returns a single static method of a given type, include type predecessors, and including their
    /// private methods, that has the given method name. Throws InvalidOperationException if there is no such
    /// method, or if there are more than one such methods.</summary>
    ///
    /// <exception cref="ArgumentNullException"> Passed when argument <paramref name="t"/> is null. </exception>
    /// <exception cref="ArgumentException"> Passed when argument <paramref name="methodName"/> is null or empty. </exception>
    /// <exception cref="InvalidOperationException"> Passed from Enumerable.Single when there is more than one
    /// method matching the given criteria, or when there is no such method. </exception>
    ///
    /// <param name="t"> The type whose methods will be enumerated. </param>
    /// <param name="methodName"> The name that returned method must match. </param>
    ///
    /// <returns> Resulting single MethodInfo.</returns>
    public static MethodInfo GetStaticMethod(this Type t, string methodName)
    {
        return GetAllStaticMethods(t, methodName).Single();
    }
    #endregion // Accessing_static_methods

    #region Accessing_instance_methods
    /// <summary>Get all instance methods of a given type, include type predecessors, and including their private
    /// methods, that have the given method name.</summary>
    ///
    /// <exception cref="ArgumentNullException"> Passed when argument <paramref name="t"/> is null. </exception>
    /// <exception cref="ArgumentException"> Passed when argument <paramref name="methodName"/> is null or empty. </exception>
    ///
    /// <param name="t"> The type whose methods will be enumerated. </param>
    /// <param name="methodName"> The name that returned methods must match. </param>
    ///
    /// <returns> Resulting sequence of MethodInfo that can be iterated.</returns>
    public static IEnumerable<MethodInfo> GetAllInstanceMethods(this Type t, string methodName)
    {
        return GetAllMethods(t, methodName, AnyInstance);
    }


    /// <summary>Returns a single instance method of a given type, including type predecessors, and including
    /// their private methods, that have the given method name. Throws InvalidOperationException if there is no
    /// such method, or if there are more than one such methods.</summary>
    ///
    /// <exception cref="ArgumentNullException"> Passed when argument <paramref name="t"/> is null. </exception>
    /// <exception cref="ArgumentException"> Passed when argument <paramref name="methodName"/> is null or empty. </exception>
    /// <exception cref="InvalidOperationException"> Passed from Enumerable.Single when there is more than one
    /// method matching the given criteria, or when there is no such method. </exception>
    ///
    /// <param name="t"> The type whose methods will be enumerated. </param>
    /// <param name="methodName"> The name that returned method must match. </param>
    ///
    /// <returns> Resulting single MethodInfo.</returns>
    public static MethodInfo GetInstanceMethod(this Type t, string methodName)
    {
        return GetAllInstanceMethods(t, methodName).Single();
    }
    #endregion // Accessing_instance_methods

    #region Accessing_just_any_methods
    /// <summary>Get all MethodInfo of a given type, including type predecessors, and including their private
    /// methods.</summary>
    ///
    /// <remarks>To get private method declared in the base class, one cannot just simply call type.GetMethod,
    /// even with BindingFlags.FlattenHierarchy, that's why this method exists.
    /// 
    /// For the reasons behind, see
    /// <a href="http://stackoverflow.com/questions/1155529/c-gettype-getfields-problem">
    /// Stackoverflow: C# GetType().GetFields problem</a></remarks>
    ///
    /// <exception cref="ArgumentNullException"> Passed when argument <paramref name="t"/> is null. </exception>
    ///
    /// <param name="t"> The type whose methods will be enumerated. Can't be null. </param>
    /// <param name="flags"> A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the
    /// search is conducted. </param>
    ///
    /// <returns> Resulting sequence of MethodInfo that can be iterated.</returns>
    public static IEnumerable<MethodInfo> GetAllMethods(this Type t, BindingFlags flags)
    {
        ArgumentNullException.ThrowIfNull(t);

        // in order to avoid duplicates, force BindingFlags.DeclaredOnly
        IEnumerable<MethodInfo> mtp = t.GetMethods(flags | BindingFlags.DeclaredOnly);
        IEnumerable<MethodInfo> result;

        if (t == typeof(object))
        {
            result = mtp;
        }
        else
        {
            result = t.BaseType!.GetAllMethods(flags);
            result = result.Concat(mtp);
        }

        return result;
    }


    /// <summary>Get all MethodInfo of a given type <paramref name="t"/>
    /// that have given name <paramref name="methodName"/>, including type predecessors, and including their
    /// private methods.</summary>
    ///
    /// <remarks>To get private method declared in the base class, one cannot just simply call type.GetMethod,
    /// even with BindingFlags.FlattenHierarchy, that's why this method exists.
    /// 
    /// For the reasons behind, see
    /// <a href="http://stackoverflow.com/questions/1155529/c-gettype-getfields-problem">
    /// Stackoverflow: C# GetType().GetFields problem</a></remarks>
    ///
    /// <exception cref="ArgumentException"> Passed when argument <paramref name="methodName"/> is null or empty. </exception>
    /// <exception cref="ArgumentNullException"> Passed when argument <paramref name="t"/> is null. </exception>
    ///
    /// <param name="t"> The type whose methods will be enumerated. Can't be null. </param>
    /// <param name="methodName"> . </param>
    /// <param name="flags"> A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the
    /// search is conducted. </param>
    ///
    /// <returns> Resulting sequence of MethodInfo that can be iterated.</returns>
    public static IEnumerable<MethodInfo> GetAllMethods(this Type t, string methodName, BindingFlags flags)
    {
        if (string.IsNullOrEmpty(methodName))
            throw new ArgumentException("This argument value cannot be null or an empty string", nameof(methodName));
        // checking of 't' argument is performed by called overload
        IEnumerable<MethodInfo> temp = GetAllMethods(t, flags);
#if DEBUG
        IEnumerable<MethodInfo> result;

        temp = temp.ToList();
        result = temp.Where(m => 0 == string.CompareOrdinal(methodName, m.Name));
        result = result.ToList();

        return result;
#else
        return temp.Where(m => 0 == string.CompareOrdinal(methodName, m.Name));
#endif // DEBUG
    }


    /// <summary>Get method of a given type, method name, argument types. Searches complete type hierarchy of base
    /// types their private methods, that have the given name.</summary>
    ///
    /// <exception cref="ArgumentException"> Thrown when string <paramref name="methodName"/> is null or empty. </exception>
    /// <exception cref="ArgumentNullException"> Passed when a supplied type <paramref name="t"/> is null. </exception>
    /// <exception cref="AmbiguousMatchException"> Passed when more than one method is found with the specified
    /// name and specified parameters. </exception>
    ///
    /// <param name="t"> The type whose methods will be enumerated. </param>
    /// <param name="methodName"> The name that returned methods must match. </param>
    /// <param name="argTypes"> Types of formal arguments in the method declaration. </param>
    /// <param name="flags"> A bit mask comprised of one or more <see cref="BindingFlags"/>
    /// that specify how the search is conducted. </param>
    ///
    /// <returns> MethodInfo of found method, or null if none such method found.</returns>
    public static MethodInfo? GetMethodEx(
        this Type t,
        string methodName,
        Type[] argTypes,
        BindingFlags flags)
    {
        ArgumentNullException.ThrowIfNull(t);

        if (string.IsNullOrEmpty(methodName))
        {
            throw new ArgumentException("Method name cannot be null or empty", nameof(methodName));
        }

        MethodInfo? result;

        if ((t == typeof(object)) || (null == (result = t!.BaseType!.GetMethodEx(methodName, argTypes, flags))))
        {
            result = t.GetMethod(methodName, flags | BindingFlags.DeclaredOnly, null, CallingConventions.Any,
              argTypes, null);
        }

        return result;
    }
    #endregion // Accessing_just_any_methods
    #endregion // Accessing_MethodInfo_whole_depth
    #endregion // Public Methods

    #region Private Methods
    #region Running_method_limited_depth
    /// <summary>Implementation helper called by public methods. Runs given method, finding the method by calling
    /// Type.GetMethod.<br/>
    /// 
    /// Note this method will NOT access private methods declared in the base class, and will throw
    /// <see cref="ArgumentException"/> instead.</summary>
    ///
    /// <exception cref="ArgumentException"> Thrown when string <paramref name="methodName"/> is null or empty. </exception>
    /// <exception cref="ArgumentNullException"> Passed when a supplied type <paramref name="t"/> is null. </exception>
    /// <exception cref="AmbiguousMatchException"> Passed when more than one method is found with the specified
    /// name and specified parameters. </exception>
    ///
    /// <param name="objInstance"> The object which implements the given method, or null for case of static
    /// method. </param>
    /// <param name="t"> For case of static methods this is their implementing type. For non-static methods this
    /// is the type of the object objInstance. </param>
    /// <param name="methodName"> The method name. Can't be null or empty. </param>
    /// <param name="argTypes"> The types of all arguments of the method. This argument may be null, if it is not
    /// null the method searches for the specified method whose parameters match the specified argument types.
    /// This helps to determine which one of several overloads should be used. </param>
    /// <param name="parameters"> The actual parameters. </param>
    /// <param name="eFlags"> A bit mask comprised of one or more <see cref="BindingFlags"/> that specify how the
    /// search for the method is conducted. </param>
    ///
    /// <returns> Result of the method execution.</returns>
    private static object? RunMethod(
        this object? objInstance,
        Type t,
        string methodName,
        Type[]? argTypes,
        object[] parameters,
        BindingFlags eFlags)
    {
        ArgumentNullException.ThrowIfNull(t);

        if (string.IsNullOrEmpty(methodName))
            throw new ArgumentException("Method name cannot be null or empty", nameof(methodName));
        Debug.Assert((objInstance == null) || objInstance.GetType() == t);

        string strErr, strTypesDump;
        MethodInfo? m;

        if (argTypes == null)
            m = t.GetMethod(methodName, eFlags);
        else
            m = t.GetMethod(methodName, eFlags, null, argTypes, null);

        if (m == null)
        {
            if (argTypes == null)
            {
                strErr = string.Format(CultureInfo.InvariantCulture, "There is no method '{0}' in the type '{1}'.",
                  methodName, t);
            }
            else
            {
                strTypesDump = argTypes.Select(tp => tp.ToString()).Aggregate((suma, next) => suma + ", " + next);
                strErr = string.Format(CultureInfo.InvariantCulture,
                  "There is no method '{0}' with argument types '({1})' in the type '{2}'.",
                  methodName, strTypesDump, t);
            }
            throw new ArgumentException(strErr);
        }

        object? objRet = m.Invoke(objInstance, parameters);
        return objRet;
    }
    #endregion // Running_method_limited_depth

    #region Running_method_whole_depth
    /// <summary>Run non-static method for given object instance, method name, argument types and arguments. Note
    /// this CAN access private methods declared in the base class.</summary>
    ///
    /// <exception cref="ArgumentException"> Thrown when string <paramref name="methodName"/> is null or empty. </exception>
    /// <exception cref="ArgumentNullException"> Passed when a supplied type <paramref name="t"/> is null. </exception>
    /// <exception cref="InvalidOperationException"> Passed from Enumerable.Single when there is more than one
    /// method matching the given criteria, or when there is no such method. </exception>
    /// <exception cref="AmbiguousMatchException"> Passed when more than one method is found with the specified
    /// name and specified parameters. </exception>
    ///
    /// <param name="objInstance"> The instance of object whose method will be called. </param>
    /// <param name="t"> The type that implements the static method <paramref name="methodName"/>. </param>
    /// <param name="methodName"> The name of the method. </param>
    /// <param name="argTypes"> Types of formal arguments in the method declaration. </param>
    /// <param name="parameters"> Arguments of the method. </param>
    /// <param name="flags"> A bit mask comprised of one or more <see cref="BindingFlags"/> that specify how the
    /// search for the method is conducted. </param>
    ///
    /// <returns> An object which is a result of the method execution.</returns>
    private static object? RunMethodEx(
        this object objInstance,
        Type t,
        string methodName,
        Type[]? argTypes,
        object[] parameters,
        BindingFlags flags)
    {
        ArgumentNullException.ThrowIfNull(t, nameof(t));

        if (string.IsNullOrEmpty(methodName))
        {
            throw new ArgumentException("Method name cannot be null or empty", nameof(methodName));
        }
        Debug.Assert((objInstance == null) || objInstance.GetType() == t);

        string strErr, strTypesDump;
        IEnumerable<MethodInfo> methods;
        MethodInfo? m = null;
        object? objRet = null;

        if (argTypes != null)
        {
            m = GetMethodEx(t, methodName, argTypes, flags);
        }
        else
        {
            methods = GetAllMethods(t, methodName, flags);
            m = methods.Single();
        }

        if (m == null)
        {
            if (null == argTypes)
            {
                strErr = string.Format(CultureInfo.InvariantCulture, "There is no method '{0}' in the type '{1}'.",
                  methodName, t);
            }
            else
            {
                strTypesDump = argTypes.Select(tp => tp.ToString()).Aggregate((suma, next) => suma + ", " + next);
                strErr = string.Format(CultureInfo.InvariantCulture,
                  "There is no method '{0}' with argument types '({1})' in the type '{2}'.",
                  methodName, strTypesDump, t);
            }
            throw new ArgumentException(strErr);
        }

        objRet = m.Invoke(objInstance, parameters);
        return objRet;
    }
    #endregion // Running_method_whole_depth
    #endregion // Private Methods
}

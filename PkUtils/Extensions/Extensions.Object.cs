using System;
using System.Linq;
using System.Runtime.CompilerServices;
using PK.PkUtils.Interfaces;

// Ignore Spelling: Utils

namespace PK.PkUtils.Extensions;

/// <summary>
/// Wrapper class around various extension methods.
/// Extension methods must be defined in a static class.
/// </summary>
public static class ObjectExtension
{
    #region Fields
    private const string _nullLiteral = "<null>";
    #endregion // Fields

    #region Methods

    /// <summary>
    /// This extension method helps to avoid nasty null checks in your code.<br/>
    /// Rather than writing following code, which is quite verbose and evaluates 
    /// the same properties over and over again
    /// <code> 
    /// <![CDATA[
    /// if (Contract != Null &&
    ///     Contract.Parties != null &&
    ///     Contract.Parties.Client != null &&
    ///     Contract.Parties.Client.Adress != null) 
    ///     { do something with the address }
    /// ]]>
    /// </code>
    /// now you could write more simply
    /// <code>
    /// if (Contract
    ///  .NullSafe(c => c.Parties)
    ///  .NullSafe(p => p.Client)
    ///  .NullSafe(c => c.Address) != null)  
    ///  { do something with the address }
    /// </code>
    /// </summary>
    /// <typeparam name="T">The type of target object that is being checked for null.
    /// </typeparam>
    /// <typeparam name="TResult">The type of the expression result.</typeparam>
    /// <param name="target">The target object that is being checked for null.</param>
    /// <param name="func">The callback function which is called for a non-null <paramref name="target"/>.
    /// In a real usage, the actual value of this argument is usually represented 
    /// by a lambda-expression.</param>
    /// <returns>For a non-null <paramref name="target"/> returns func(target); 
    /// otherwise returns default(TResult)
    /// </returns>
    public static TResult NullSafe<T, TResult>(this T target, Func<T, TResult> func) where T : class
    {
        if (target != null)
            return func(target);
        else
            return default;
    }

    /// <summary>
    /// Performs given <paramref name="action"/> on <paramref name="target"/>,
    /// if that target is not null.
    /// </summary>
    /// <typeparam name="T">The type of target object that is being checked for null.</typeparam>
    /// <param name="target">The target object that is being checked for null.</param>
    /// <param name="action">Encapsulates a method that takes a single parameter 
    /// <paramref name="target"/> and does not return a value.
    /// </param>
    public static void NullSafe<T>(this T target, Action<T> action) where T : class
    {
        if (target != null)
        {
            action(target);
        }
    }

    /// <summary>
    /// An extension method that converts <paramref name="obj"/> object to a string.
    /// </summary>
    ///
    /// <typeparam name="T"> Generic type parameter, type of the object. </typeparam>
    /// <param name="obj">  The object being converted. May be null. </param>
    /// <param name="nullSubstitute"> (Optional) The value to be returned when <paramref name="obj"/> is null.
    /// If null, {null} will be used. </param>
    ///
    /// <returns> A string. </returns>
    public static string AsString<T>(this T obj, string nullSubstitute = null)
    {
        string result;

        if (obj == null)
            result = nullSubstitute ?? _nullLiteral;
        else
            result = obj.ToString();

        return result;
    }

    /// <summary> A T extension method that converts this object to string containing a name = value pair. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="obj"> The object to act on. Can be null. </param>
    /// <param name="objName"> (Optional) Name of the object. </param>
    /// <returns> A string. </returns>
    public static string AsNameValue<T>(this T obj, [CallerArgumentExpression(nameof(obj))] string objName = null)
    {
        return $"{objName} = {obj.AsString()}";
    }

    /// <summary>
    /// An extension method that converts <paramref name="obj"/> object to a string,.
    /// </summary>
    ///
    /// <typeparam name="T">    Generic type parameter, type of the object. </typeparam>
    /// <param name="obj">  The object being converted. May be null. </param>
    /// <param name="sortAlphabetically">   (Optional) True to sort properties alphabetically. </param>
    /// <param name="nullSubstitute"> (Optional) The value to be returned when <paramref name="obj"/> is null. 
    ///                               If null, {null} will be used. </param>
    /// <returns> A list of properties with values, like "Age: 32, Name: Paul". </returns>
    public static string PropertyList<T>(
        this T obj,
        bool sortAlphabetically = false,
        string nullSubstitute = null)
    {
        string result;

        if (obj == null)
        {
            result = nullSubstitute ?? _nullLiteral;
        }
        else
        {
            var props = obj.GetType().GetProperties()
                .Select(info => (info.Name, Value: info.GetValue(obj, null) ?? _nullLiteral));
            if (sortAlphabetically)
                props = props.OrderBy(pair => pair.Name);

            result = props.Select(pair => $"{pair.Name}: {pair.Value}").Join();
        }

        return result;
    }

    /// <summary> Throws <see cref="ArgumentNullException"/> in case the object is null. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied object is null. </exception>
    /// 
    /// <typeparam name="T">The type of target object that is being checked for null.</typeparam>
    /// <param name="obj"> The object being checked. </param>
    /// <param name="objectName"> The name of the object ( for instance the name 
    /// of formal argument in the calling code). </param>
    /// 
    /// <returns> The original value of <paramref name="obj"/>, if that's not null.</returns>
    /// 
    /// <seealso cref="CheckNotNull"/>
    public static T CheckArgNotNull<T>(
        this T obj,
        [CallerArgumentExpression(nameof(obj))] string objectName = "obj") where T : class
    {
        if (null == obj)
        {
            if (string.IsNullOrEmpty(objectName))
                objectName = "obj";

            throw new ArgumentNullException(objectName, $"The object '{objectName}' is null");
        }

        return obj;
    }

    /// <summary> Throws <see cref="InvalidOperationException"/> in case the object is null. </summary>
    /// <exception cref="InvalidOperationException"> Thrown when a supplied object is null. </exception>
    /// 
    /// <typeparam name="T">The type of target object that is being checked for null.</typeparam>
    /// <param name="obj"> The object being checked. </param>
    /// <param name="objectName"> The name of the object ( for instance the name 
    /// of formal argument in the calling code). </param>
    /// <returns> The original value of <paramref name="obj"/>, if that's not null.</returns>
    /// <seealso cref="CheckArgNotNull"/>

    public static T CheckNotNull<T>(
        this T obj,
        [CallerArgumentExpression(nameof(obj))] string objectName = "obj") where T : class
    {
        if (null == obj)
        {
            if (string.IsNullOrEmpty(objectName))
                objectName = "obj";

            throw new InvalidOperationException($"The object '{objectName}' is null");
        }
        return obj;
    }

    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> in case the object is null.
    /// Throws <see cref="ObjectDisposedException"/> in case the object is not null, 
    /// but supports <see cref="PK.PkUtils.Interfaces.IDisposableEx"/>
    /// and IDisposableEx.IsDisposed property returns true. 
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied object is null. </exception>
    /// <exception cref="ObjectDisposedException"> Thrown when a supplied object has been disposed. </exception>
    /// <param name="obj"> The object being checked. </param>
    /// <returns> The original value of <paramref name="obj"/>, if that's not null or disposed.</returns>
    public static object CheckNotDisposed(this object obj)
    {
        return CheckNotDisposed(obj, null);
    }

    /// <summary> 
    /// Throws <see cref="ArgumentNullException"/> in case the object is null.
    /// Throws <see cref="ObjectDisposedException"/> in case the object is not null, 
    /// but supports <see cref="PK.PkUtils.Interfaces.IDisposableEx"/>
    /// and IDisposableEx.IsDisposed property returns true. 
    /// </summary>
    ///
    /// <remarks> The argument objectName is used as a exception argument. </remarks>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when a supplied object is null. </exception>
    /// <exception cref="ObjectDisposedException"> Thrown when a supplied object has been disposed. </exception>
    ///
    /// <param name="obj">        The object being checked. </param>
    /// <param name="objectName"> The object name (usually the argument or 
    /// variable name the object has in the calling code).
    /// Will be used for the exception message creation. </param>
    public static object CheckNotDisposed(
        this object obj,
        [CallerArgumentExpression(nameof(obj))] string objectName = null)
    {
        CheckArgNotNull(obj, objectName);
        if (obj is IDisposableEx iDisp)
        {
            DisposableExtension.CheckNotDisposed(iDisp, objectName);
        }
        return obj;
    }
    #endregion // Methods
}

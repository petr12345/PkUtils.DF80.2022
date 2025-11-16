// Ignore Spelling: Utils, Cloneable
//
using System;

namespace PK.PkUtils.Interfaces;

/// <summary>
/// IDeepCloneable is a replacement of  <see cref="ICloneable"/> interface design mistake.
/// The call of DeepClone produces a deep clone of the object.
/// </summary>
///
/// <seealso href="https://docs.microsoft.com/en-us/archive/blogs/brada/should-we-obsolete-icloneable-the-slar-on-system-icloneable"> Brad
/// Abrams Blog: Should we Obsolete ICloneable</seealso>
/// <seealso href="http://stackoverflow.com/questions/3345389/copy-constructor-versus-clone"> Stackoverflow:
/// Copy constructor versus Clone()</seealso>
/// <seealso href="https://codinghelmet.com/articles/implement-icloneable-or-not"> Stackoverflow:
/// The ICloneable Controversy: Should a Class Implement ICloneable or Not?</seealso>
public interface IDeepCloneable
{
    /// <summary> Returns a deep clone of 'this' object. </summary>
    /// <returns> Resulting deep clone object. </returns>
    object DeepClone();
};

/// <summary>
/// Generic version of non-generic interface IDeepCloneable
/// </summary>
/// 
/// <remarks>
/// <para>
/// The interface derives from IDeepCloneable on purpose.
/// This way, if you have an object implementing generic version of the interface, 
/// you can make a deep copy of that without knowing which type T was used as a generic argument.
/// </para>
/// 
/// <para>
/// Note the <typeparamref name="T"/> argument is covariant; hence assignment compatibility is preserved.
/// You can assign object that is instantiated with a more derived type argument
/// to an object instantiated with a less derived type argument like following.
/// <code>
/// <![CDATA[
///  IDeepCloneable<string> x = SomeMethod();
///  // makes sense, as string derived from object, but covariance is needed to let compiler allow that
///  IDeepCloneable<object> y = x;
/// ]]>
/// </code>
/// </para>
/// 
/// </remarks>
/// <typeparam name="T"> The type of object that is being cloned. </typeparam>
public interface IDeepCloneable<out T> : IDeepCloneable
{
    /// <summary> The type-safe version of <see cref="IDeepCloneable.DeepClone" /> </summary>
    /// <returns> Resulting deep clone object. </returns>
    new T DeepClone();
};

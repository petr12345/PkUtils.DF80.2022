// Ignore Spelling: Utils, Cloneable
//
using System;

namespace PK.PkUtils.Interfaces;

/// <summary>
/// IShallowCloneable is a replacement of  <see cref="ICloneable"/> interface design mistake.
/// The call of ShallowClone produces a shallow clone of the object.
/// </summary>
/// <seealso href="https://docs.microsoft.com/en-us/archive/blogs/brada/should-we-obsolete-icloneable-the-slar-on-system-icloneable">
/// Brad Abrams Blog: Should we Obsolete ICloneable </seealso>
/// <seealso href="http://stackoverflow.com/questions/3345389/copy-constructor-versus-clone">
/// Stackoverflow: Copy constructor versus Clone() </seealso>
[CLSCompliant(true)]
public interface IShallowCloneable
{
    /// <summary>
    /// Returns a shallow clone of 'this' object.
    /// </summary>
    /// <returns>Resulting shallow  clone object.</returns>
    object ShallowClone();
};

/// <summary>
/// Generic version of non-generic interface IShallowCloneable
/// </summary>
/// 
/// <remarks>
/// <para>
/// The interface derives from IShallowCloneable on purpose.
/// This way, if you have an object implementing generic version of the interface, 
/// you can make a shallow copy of that without knowing which type T was used as a generic argument.
/// </para>
/// 
/// <para>
/// Note the <typeparamref name="T"/> argument is covariant; hence assignment compatibility is preserved.
/// You can assign object that is instantiated with a more derived type argument
/// to an object instantiated with a less derived type argument like following.
/// <code>
/// <![CDATA[
///  IShallowCloneable<string> x = SomeMethod();
///  // makes sense, as string derived from object, but covariance is needed to let compiler allow that
///  IShallowCloneable<object> y = x;
/// ]]>
/// </code>
/// </para>
/// 
/// </remarks>
/// <typeparam name="T">The type of object that is being cloned.</typeparam>
[CLSCompliant(true)]
public interface IShallowCloneable<out T> : IShallowCloneable
{
    /// <summary>
    /// The type-safe version of <see cref="IShallowCloneable.ShallowClone" />
    /// </summary>
    /// <returns>Resulting shallow clone object.</returns>
    new T ShallowClone();
}

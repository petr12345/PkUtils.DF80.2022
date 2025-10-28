// Ignore Spelling: Utils, Cloneable
// 

using System;
using PK.PkUtils.Interfaces;
#pragma warning disable IDE0130 // Namespace "..." does not match folder structure

namespace PK.PkUtils.Cloning.Binary;

/// <summary>
/// Applying this generic helper class will implement the IDeepCloneable-generic 
/// interface for you, like in following example:
/// <code>
/// <![CDATA[
/// [Serializable]
/// class Cat : MakeCloneableBinary<Cat>, IDeepCloneable<Cat>
/// {
///   public string Name { get; set; }
/// 
///   public Cat()
///     : this(string.Empty)
///   {
///   }
///   public Cat(string strName)
///   {
///     Name = strName;
///   }
/// }
/// ]]>
/// </code>
/// </summary>
/// <typeparam name="T">The type of object that is being cloned.</typeparam>
/// <remarks>
/// Note that in C# (unlike in C++) one cannot specify the base class the generic could derive from;
/// such code produces a compilation error "Cannot derive from 'R' because it is a type parameter".
/// But at least you could inherit from generic base class, with self as type parameter.
/// This approach is called "Curiously recurring templates/generics pattern".
/// </remarks>
/// <seealso href="http://social.msdn.microsoft.com/Forums/is/csharplanguage/thread/a92428ec-0ea0-49cb-b942-124f36661e98">
/// MSDN forum on Inheriting from generic base class, with self as type parameter</seealso>
/// <seealso href="http://blogs.msdn.com/b/oldnewthing/archive/2009/08/14/9869049.aspx">
/// MSDN blog on Why can't I declare a type that derives from a generic type parameter?</seealso>
[CLSCompliant(true)]
[Serializable]
public class MakeCloneableBinary<T> : IDeepCloneable<T>
{
    #region Constructor(s)

    /// <summary> Specialized default constructor for use only by derived class. </summary>
    protected MakeCloneableBinary()
    { }

    #endregion // Constructor(s)

    #region IDeepCloneable<T> Members

    /// <summary>
    /// An implementation of IDeepCloneable.DeepClone() method.
    /// </summary>
    /// <returns>Resulting deep clone object.</returns>
    /// <remarks>
    /// One has to use an explicit interface implementation on one of DeepClone() overloads,
    /// otherwise the compiler produces an error message
    /// "Error	Type .... already defines a member called 'DeepClone'  with the same parameter types".
    /// Assuming this is the choice, it is better for usability if the generic overload is made public
    /// and the non-generic overload non-public.
    /// </remarks>
    object IDeepCloneable.DeepClone()
    {
        return (this as IDeepCloneable<T>).DeepClone();
    }

    /// <summary>
    /// Just a "normal" implementation of generic IDeepCloneable<typeparamref name="T"/>.DeepClone() method.
    /// </summary>
    /// <returns>Resulting deep clone object.</returns>
    public virtual T DeepClone()
    {
        return (T)CloneHelperBinary.DeepClone((object)this);
    }
    #endregion // IDeepCloneable<T> Members
}

#pragma warning restore IDE0130
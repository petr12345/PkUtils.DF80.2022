// Ignore Spelling: Utils
//
using System;
using System.IO;
using System.Runtime.Serialization;

#pragma warning disable IDE0130 // Namespace "..." does not match folder structure

namespace PK.PkUtils.Cloning.ByContract;

/// <summary>
/// The helper class that you will use if you want to create a deep copy of an object.
/// It works a similar way as the class CloneHelperBinary, but it should be used for cloning classes
/// which do not support binary serialization ( hence the class CloneHelperBinary could not be used),
/// but which DO support Data Contract Serialization.
/// </summary>
/// 
/// <example>
/// The following is an example of using a CloneHelperByContract extension method:
/// <code>
/// [DataContract]
/// class Foo
/// {
///   [DataMember]
///   public Bar Bar { get; set; }
/// }
/// [DataContract]
/// class Bar
/// {
///   [DataMember]
///   public Foo Foo { get; set; }
/// }
/// static void Main()
/// {
///   Foo foo = new Foo();
///   Bar bar = new Bar();
///   foo.Bar = bar;
///   bar.Foo = foo; // nice cyclic graph
/// 
///   // The code line below has the the same sense as: 
///   // Foo clone = CloneHelperByContract.DeepClone(foo);
///   //   or
///   // Foo clone = CloneHelperByContract.DeepClone{Foo}(foo);
/// 
///   Foo clone = foo.DeepClone();
///   Console.WriteLine(foo != clone); //true - new object
///   Console.WriteLine(clone.Bar.Foo == clone); // true; copied graph
/// }
/// </code>
/// </example>
/// 
/// <remarks>
/// Note that CloneHelperBinary and CloneHelperByContract are in different nested namespaces.
/// This should prevents ambiguous calls, with compilation errors like
/// <i>
///   Error 1 The call is ambiguous between the following methods or properties: 
///   'CloneHelperBinary.DeepClone{PK.TestCloning.Foo}(PK.TestCloning.Foo)' and 
///   'CloneHelperByContract.DeepClone{PK.TestCloning.Foo}(PK.TestCloning.Foo)'
/// </i>
/// </remarks>
[CLSCompliant(true)]
public static class CloneHelperByContract
{
    /// <summary>
    /// The deep copy copies the object, and all objects it refers to 
    /// (and all objects they refer to in turn, and so forth, at least if it's really deep). 
    /// </summary>
    /// <param name="original">The object that will be copied by deep cloning.</param>
    /// <returns>The deep clone of the <paramref name="original"/> instance</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input argument <paramref name="original"/> is null.</exception>
    /// 
    /// <seealso href="http://stackoverflow.com/questions/2417023/clone-whole-object-graph">
    /// Stackoverflow.com : Clone Whole Object Graph</seealso>
    public static object DeepClone(this object original)
    {
        ArgumentNullException.ThrowIfNull(original);
        var serializer = new DataContractSerializer(type: original.GetType(), knownTypes: null);
        using var ms = new MemoryStream();

        serializer.WriteObject(ms, original);
        ms.Position = 0;
        return serializer.ReadObject(ms);
    }

    /// <summary> Type-safe overload of DeepClone </summary>
    /// <typeparam name="T">The type of the input argument <paramref name="original"/></typeparam>
    /// <param name="original">The object that will be copied by deep cloning.</param>
    /// <returns>The deep clone of the <paramref name="original"/> instance</returns>
    /// <exception cref="System.ArgumentNullException"> Thrown when the input argument <paramref name="original"/> is null.</exception>
    public static T DeepClone<T>(this T original)
    {
        return (T)CloneHelperByContract.DeepClone((object)original);
    }
}

#pragma warning restore IDE0130
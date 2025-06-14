// Ignore Spelling: Utils, Cat, Dog, miaow
//
#pragma warning disable SYSLIB0011 // BinaryFormatter serialization is obsolete and should not be used.
#pragma warning disable IDE0130 // Namespace "..." does not match folder structure

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using PK.PkUtils.Cloning.ByContract;

namespace PK.PkUtils.Cloning.Binary;

/// <summary>
/// The helper class that you will use if you want to implement <see cref="Interfaces.IDeepCloneable"/>.
/// It works a similar way as the class <see cref="CloneHelperByContract"/>, 
/// but it should be used for cloning classes which do not support data contract serialization 
/// ( hence the class CloneHelperByContract could not be used), 
/// but which DO support binary Serialization.
/// </summary>
/// 
/// <example>
/// Code example:
/// The following is an example of using a CloneHelperBinary extension method:
/// <code>
/// [Serializable]
/// class Cat : Animal
/// {
///   public Dog Dog { get; set; }
///   public Cat() { }
/// }
/// [Serializable]
/// class Dog : Animal
/// {
///   public Cat Cat { get; set; }
///   public Dog() { }
/// }
/// static void Main()
/// {
///   Cat miaow = new Cat();
///   Dog bowwow = new Dog();
///   miaow.Dog = bowwow;
///   bowwow.Cat = miaow; // nice cyclic graph
///   
///   // The code line below has the same sense as: 
///   // Cat clone = CloneHelperBinary.DeepClone(miaow);
///   //   or
///   // Cat clone = CloneHelperBinary.DeepClone{Cat}(miaow);
///   
///   Cat clone = miaow.DeepClone(); 
///   Console.WriteLine("Check Cat clone is not original: {0}", miaow != clone); //true - new object
///   Console.WriteLine("Check Cat copied graph: {0}", clone.Dog.Cat == clone); // true; copied graph
/// }
/// </code>
/// </example>
/// 
/// <remarks>
/// Note that CloneHelperBinary and CloneHelperByContract are in different nested namespaces.
/// This should prevents ambiguous calls, with compilation errors like <br/>
/// <i>
///   Error 1 The call is ambiguous between the following methods or properties: 
///   'CloneHelperBinary.DeepClone{PK.TestCloning.Foo}(PK.TestCloning.Foo)' and 
///   'CloneHelperByContract.DeepClone{PK.TestCloning.Foo}(PK.TestCloning.Foo)'
/// </i>
/// </remarks>
[CLSCompliant(true)]
public static class CloneHelperBinary
{
    /// <summary>
    /// The deep copy copies the object, and all objects it refers to 
    /// (and all objects they refer to in turn, and so forth, at least if it's really deep). 
    /// </summary>
    /// <remarks>
    /// <para>
    /// The code origin is MSDN magazine article Run-time Serialization, Part 1 and 2.<br/>
    /// Unlike with the original code, the method is now declared as an extension method.<br/>
    /// Here is the fragment of the article regarding Streaming Contexts:
    /// </para>
    /// <para>
    /// Streaming Contexts <br/>
    /// There are many destinations for a serialized set of objects: same process, 
    /// different process on the same machine, different process on a different machine, 
    /// and so on. In some rare situations, an object might want to know 
    /// where it is going to be deserialized so that it can emit its state differently. 
    /// For example, an object that wraps a Windows® semaphore object might decide to serialize 
    /// its kernel handle if the object knows that it will be deserialized into the same process. 
    /// The object might decide to serialize the semaphore's string name if it knows 
    /// that the object will be deserialized on the same machine. 
    /// Finally, the object might decide to throw an exception if it knows 
    /// that it will be deserialized within a process running on a different machine.
    /// A number of the methods mentioned earlier, such as GetObjectData and SetObjectData, 
    /// have a StreamingContext as one of their parameters. 
    /// </para>
    /// <para>
    /// A StreamingContext structure is a very simple value type, 
    /// offering just two public read-only properties.
    /// A method that receives a StreamingContext structure can examine the State property's bit flags 
    /// to determine the source or destination of the objects being serialized/deserialized. 
    /// </para>
    /// <para>
    /// Now that you know how to get this information, I'll show you how to set it. <br/>
    /// The IFormatter interface (implemented by both the BinaryFormatter and the SoapFormatter types) 
    /// defines a read/write StreamingContext property called Context. 
    /// When you construct a formatter, the formatter initializes its Context property 
    /// so that StreamingContextStates is set to All and the reference to the additional state object is set to null.
    /// After the formatter is constructed, you can construct a StreamingContext structure 
    /// using any of the StreamingContextState bit flags and you can optionally pass a reference 
    /// to an object containing any additional context information you need. 
    /// Now, all you need to do is set the formatter's Context property with this new StreamingContext object 
    /// before calling the formatter's Serialize or Deserialize methods. 
    /// </para>
    /// </remarks>
    /// <param name="original">The object that will be copied by deep cloning.</param>
    /// <returns>The deep clone of the <paramref name="original"/> instance</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when the input argument 
    /// <paramref name="original"/> is null.</exception>
    /// 
    /// <seealso href="https://docs.microsoft.com/en-us/archive/msdn-magazine/2002/april/net-run-time-serialization">
    /// .NET Run-time Serialization Part 1</seealso>
    ///
    /// <seealso href="https://docs.microsoft.com/en-us/archive/msdn-magazine/2002/july/net-run-time-serialization-part-2">
    /// .NET Run-time Serialization Part 2</seealso>
    /// 
    /// <seealso href="https://docs.microsoft.com/en-us/archive/msdn-magazine/2002/september/net-column-run-time-serialization-part-3">
    /// .NET Run-time Serialization Part 3 </seealso>
    public static object DeepClone(this object original)
    {
        ArgumentNullException.ThrowIfNull(original);

        // Construct a temporary memory stream
        using MemoryStream stream = new();

        // Construct a serialization formatter that does all the hard work
        BinaryFormatter formatter = new();

        // Serialize the object graph into the memory stream
        formatter.Serialize(stream, original);

        // Seek back to the start of the memory stream before deserializing
        stream.Position = 0;

        // Deserialize the graph into a new set of objects
        // and return the root of the graph (deep copy) to the caller
        return formatter.Deserialize(stream);

    }

    /// <summary>
    /// Type-safe overload of DeepClone
    /// </summary>
    /// <typeparam name="T">The type of the input argument <paramref name="original"/></typeparam>
    /// <param name="original">The object that will be copied by deep cloning.</param>
    /// <returns>The deep clone of the <paramref name="original"/> instance</returns>
    /// <exception cref="System.ArgumentNullException"> Thrown when the input argument 
    /// <paramref name="original"/> is null.</exception>
    /// <remarks>
    /// If deriving a new class that should support type-safe variant of IDeepCloneable, 
    /// you can do just following:
    /// <code>
    /// 
    /// [Serializable]
    /// class DeepCloneableBase : IDeepCloneable
    /// {
    ///   // Just assume this class implements somehow the non-generic IDeepCloneable
    /// }
    /// 
    /// [Serializable]
    /// class Derived : DeepCloneableBase, IDeepCloneable{Derived}
    /// {
    ///   // Some specific fields will go here...
    ///  
    ///   public new Derived DeepClone()
    ///   {
    ///     return CloneHelperBinary.DeepClone{Derived}(this);
    ///   }
    /// }
    /// </code>
    /// </remarks>
    public static T DeepClone<T>(this T original)
    {
        return (T)CloneHelperBinary.DeepClone((object)original);
    }

    /// <summary>
    /// Conversion of serializable object to byte array
    /// </summary>
    /// <param name="original">The object being converted.</param>
    /// <returns>The resulting array of bytes. 
    /// Returns null if the input argument <paramref name="original"/> is null.
    /// </returns>
    /// <remarks> Note: unlike in the original code, I have declared this as an extension method. </remarks>
    public static byte[] ToByteArray(this object original)
    {
        byte[] result = null;

        if (null != original)
        {
            using MemoryStream stream = new();
            BinaryFormatter formatter = new();

            formatter.Serialize(stream, original);
            stream.Position = 0;
            result = stream.ToArray();
        }
        return result;
    }
}

#pragma warning restore IDE0130
#pragma warning restore SYSLIB0011
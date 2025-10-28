// Ignore Spelling: Utils, Serializer
//
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace PK.PkUtils.XmlSerialization;

/// <summary>
/// Extensions methods for XMLSerializerAdapter>.
/// </summary>
[CLSCompliant(true)]
public static class XmlSerializerExtensions
{
    /// <summary> Gets the default encoding, used in methods Serialize and SerializeFragment. </summary>
    public static Encoding DefaultEncoding { get; } = new UnicodeEncoding(false, false);

    /// <summary>
    /// Deserializes the XML document contained in the specified <see cref="string"/>. The xml
    /// <see cref="string"/> must not have a byte order mask in order for this method to work.
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException">  Thrown when one or more required arguments are null. </exception>
    /// <exception cref="InvalidOperationException">
    /// An error occurred during deserialization. The original exception is available
    /// using the <see cref="Exception.InnerException"/> property.
    /// </exception>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>
    /// <param name="instance"> The instance which performs serialization. </param>
    /// <param name="xml">      The <see cref="string"/> that contains the XML document to deserialize. </param>
    ///
    /// <returns>   The deserialized object. </returns>
    public static T Deserialize<T>(this XMLSerializerAdapter<T> instance, string xml) where T : new()
    {
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(xml);

        T result;
        using (var stringReader = new StringReader(xml))
        {
            result = instance.Deserialize(stringReader);
        }

        return result;
    }

    /// <summary>
    /// Serializes the specified object into a <see cref="string"/> using <see cref="P:DefaultEncoding"/>.
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
    ///
    /// <typeparam name="T">    Generic type parameter - type of the object being serialized. </typeparam>
    /// <param name="instance"> The instance which performs serialization. </param>
    /// <param name="object">   The object to serialize. </param>
    ///
    /// <returns> A string - result of serialization. </returns>
    public static string Serialize<T>(this XMLSerializerAdapter<T> instance, T @object) where T : new()
    {
        ArgumentNullException.ThrowIfNull(instance);

        string result = instance.Serialize(@object, DefaultEncoding);

        return result;
    }

    /// <summary> Serializes the specified object into a <see cref="string"/>. </summary>
    ///
    /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are null. </exception>
    ///
    /// <typeparam name="T">    Generic type parameter - type of the object being serialized. </typeparam>
    /// <param name="instance"> The instance which performs serialization. </param>
    /// <param name="object">   The object to serialize. </param>
    /// <param name="encoding"> The encoding to use for serialization. Can't be null. </param>
    ///
    /// <returns> A string - result of serialization. </returns>
    public static string Serialize<T>(this XMLSerializerAdapter<T> instance, T @object, Encoding encoding) where T : new()
    {
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(encoding);

        string result = Serialize(instance, @object, encoding, false);

        return result;
    }

    /// <summary> Serializes the specified object into a <see cref="string"/>. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    ///
    /// <typeparam name="T">    Generic type parameter - type of the object being serialized. </typeparam>
    /// <param name="instance"> The instance which performs serialization. </param>
    /// <param name="object">   The object to serialize. </param>
    ///
    /// <returns> A string - result of serialization. </returns>
    public static string SerializeFragment<T>(this XMLSerializerAdapter<T> instance, T @object) where T : new()
    {
        ArgumentNullException.ThrowIfNull(instance);
        string result = Serialize(instance, @object, DefaultEncoding, true);

        return result;
    }

    /// <summary>
    /// Serializes the specified object into a <see cref="string"/>.
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// 
    /// <param name="instance">The involved serializer. Can't be null.</param>
    /// <param name="object">The object to serialize. Can't be null.</param>
    /// <param name="encoding">The encoding to use for serialization. Optional argument, that may be null.</param>
    /// <param name="omitXmlDeclaration"> A value indicating whether to write an XML declaration.</param>
    /// 
    /// <remarks>
    /// Note that indentation is considered rather as Serializer-bound property, and initialized in BaseSerializerAdapter. 
    /// method MakeXmlWriterSettings, where is this determined from public property Formatting WriteFormatting.
    /// </remarks>
    private static string Serialize<T>(
        this XMLSerializerAdapter<T> instance,
        T @object,
        Encoding encoding,
        bool omitXmlDeclaration) where T : new()
    {
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(@object);

        string result;
        using (var memoryStream = new MemoryStream())
        {
            var xmlWriterSettings = instance.MakeXmlWriterSettings(omitXmlDeclaration, encoding);
            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
            {
                instance.Serialize(xmlWriter, @object);
            }

            result = encoding.GetString(memoryStream.ToArray());
        }

        return result;
    }
}
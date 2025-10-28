///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// The Code Project Open License Notice
// 
// This software is a Derivative Work based upon a Code Project article
// "Using the XmlSerializer Attributes"
// http://www.codeproject.com/Articles/14064/Using-the-XmlSerializer-Attributes
//
// The Code Project Open License (CPOL) text is available at
// http://www.codeproject.com/info/cpol10.aspx
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Ignore Spelling: Utils, Serializer, namespaces
//
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PK.PkUtils.XmlSerialization;

/// <summary>
/// Serializes and deserializes objects into and from XML documents, utilizing <see cref="XmlSerializer"/>.
/// </summary>
/// <typeparam name="T"> Generic type parameter. </typeparam>
[CLSCompliant(true)]
public class XMLSerializerAdapter<T> : BaseSerializerAdapter<T>
{
    #region Fields

    private readonly XmlSerializer _serializer;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlSerializer"/> class
    /// that can serialize objects of the specified type into XML documents, and deserialize
    /// XML documents into objects of the specified type.
    /// </summary>
    public XMLSerializerAdapter()
    {
        _serializer = new XmlSerializer(typeof(T));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlSerializer"/> class
    /// that can serialize objects of the specified type into XML documents, and deserialize
    /// an XML document into object of the specified type. It also specifies the class
    /// to use as the XML root element.
    /// </summary>
    /// <param name="root">An <see cref="XmlRootAttribute"/> that represents the XML root element.</param>
    public XMLSerializerAdapter(XmlRootAttribute root)
    {
        _serializer = new XmlSerializer(typeof(T), root);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlSerializer"/> class
    /// that can serialize objects of the specified type into XML documents, and deserialize
    /// XML documents into object of a specified type. If a property or field returns
    /// an array, the extraTypes parameter specifies objects that can be inserted into
    /// the array.
    /// </summary>
    /// <param name="extraTypes">A <see cref="Type"/> array of additional object types to serialize.</param>
    public XMLSerializerAdapter(Type[] extraTypes)
    {
        _serializer = new XmlSerializer(typeof(T), extraTypes);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlSerializer"/> class
    /// that can serialize objects of the specified type into XML documents, and deserialize
    /// XML documents into objects of the specified type. Each object to be serialized
    /// can itself contain instances of classes, which this overload can override with
    /// other classes.
    /// </summary>
    /// <param name="overrides">An <see cref="XmlAttributeOverrides"/>.</param>
    public XMLSerializerAdapter(XmlAttributeOverrides overrides)
    {
        _serializer = new XmlSerializer(typeof(T), overrides);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlSerializer"/> class
    /// that can serialize objects of the specified type into XML documents, and deserialize
    /// XML documents into objects of the specified type. Specifies the default namespace
    /// for all the XML elements.
    /// </summary>
    /// <param name="defaultNamespace">The default namespace to use for all the XML elements.</param>
    public XMLSerializerAdapter(string defaultNamespace)
    {
        _serializer = new XmlSerializer(typeof(T), defaultNamespace);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlSerializer"/> class
    /// that can serialize objects of type System.Object into XML document instances,
    /// and deserialize XML document instances into objects of type System.Object. Each
    /// object to be serialized can itself contain instances of classes, which this overload
    /// overrides with other classes. This overload also specifies the default namespace
    /// for all the XML elements and the class to use as the XML root element.
    /// </summary>
    /// <param name="overrides">
    /// An <see cref="XmlAttributeOverrides"/> that extends or overrides the behavior of
    /// the class specified in the type parameter.
    /// </param>
    /// <param name="extraTypes">A <see cref="Type"/> array of additional object types to serialize.</param>
    /// <param name="root">An <see cref="XmlRootAttribute"/> that defines the XML root element properties.</param>
    /// <param name="defaultNamespace">The default namespace of all XML elements in the XML document.</param>
    public XMLSerializerAdapter(XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace)
    {
        _serializer = new XmlSerializer(typeof(T), overrides, extraTypes, root, defaultNamespace);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlSerializer"/> class
    /// that can serialize objects of type System.Object into XML document instances,
    /// and deserialize XML document instances into objects of type System.Object. Each
    /// object to be serialized can itself contain instances of classes, which this overload
    /// overrides with other classes. This overload also specifies the default namespace
    /// for all the XML elements and the class to use as the XML root element.
    /// </summary>
    /// <param name="overrides">
    /// An <see cref="XmlAttributeOverrides"/> that extends or overrides the behavior of
    /// the class specified in the type parameter.
    /// </param>
    /// <param name="extraTypes">A <see cref="Type"/> array of additional object types to serialize.</param>
    /// <param name="root">An <see cref="XmlRootAttribute"/> that defines the XML root element properties.</param>
    /// <param name="defaultNamespace">The default namespace of all XML elements in the XML document.</param>
    /// <param name="location">The location of the types.</param>
    public XMLSerializerAdapter(XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace, string location)
    {
        _serializer = new XmlSerializer(typeof(T), overrides, extraTypes, root, defaultNamespace, location);
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Return the used XmlSerializer </summary>
    protected XmlSerializer Serializer
    {
        get { return _serializer; }
    }
    #endregion // Properties

    #region Methods

    #region Public_Not_Virtual

    /// <summary>
    /// Gets a value that indicates whether this <see cref="XmlSerializer"/>
    /// can deserialize a specified XML document.
    /// </summary>
    /// <param name="xmlReader">An <see cref="XmlReader"/> that points to the document to deserialize.</param>
    /// <returns>
    /// <c>true</c> if this <see cref="XmlSerializer"/> can deserialize the object
    /// that the <see cref="XmlReader"/> points to; otherwise, false.
    /// </returns>
    public bool CanDeserialize(XmlReader xmlReader)
    {
        bool result = Serializer.CanDeserialize(xmlReader);
        return result;
    }

    /// <summary>
    /// Deserializes the XML document contained by the specified <see cref="XmlReader"/>.
    /// </summary>
    /// <param name="xmlReader">The <see cref="XmlReader"/>. that contains the XML document to deserialize.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="InvalidOperationException">
    /// An error occurred during deserialization. The original exception is available
    /// using the <see cref="Exception.InnerException"/> property.
    /// </exception>
    public T Deserialize(XmlReader xmlReader)
    {
        var result = (T)Serializer.Deserialize(xmlReader);
        return result;
    }

    /// <summary>
    /// Deserializes the XML document contained by the specified <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> that contains the XML document to deserialize.</param>
    /// <returns>The deserialized object.</returns>
    public T Deserialize(Stream stream)
    {
        var result = (T)Serializer.Deserialize(stream);
        return result;
    }

    /// <summary>
    /// Deserializes the XML document contained by the specified <see cref="XmlReader"/>
    /// and encoding style.
    /// </summary>
    /// <param name="xmlReader">The <see cref="XmlReader"/> that contains the XML document to deserialize.</param>
    /// <param name="encodingStyle">The encoding style of the serialized XML.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="InvalidOperationException">
    /// An error occurred during deserialization. The original exception is available
    /// using the <see cref="Exception.InnerException"/> property.
    /// </exception>
    public T Deserialize(XmlReader xmlReader, string encodingStyle)
    {
        var result = (T)Serializer.Deserialize(xmlReader, encodingStyle);
        return result;
    }

    /// <summary>
    /// Deserializes an XML document contained by the specified <see cref="XmlReader"/>
    /// and allows the overriding of events that occur during deserialization.
    /// </summary>
    /// <param name="xmlReader">The <see cref="XmlReader"/> that contains the document to deserialize.</param>
    /// <param name="events">An instance of the <see cref="XmlDeserializationEvents"/> class.</param>
    /// <returns>The deserialized object.</returns>
    public T Deserialize(XmlReader xmlReader, XmlDeserializationEvents events)
    {
        var result = (T)Serializer.Deserialize(xmlReader, events);
        return result;
    }

    /// <summary>
    /// Deserializes the object using the data contained by the specified <see cref="XmlReader"/>.
    /// </summary>
    /// <param name="xmlReader">An instance of the <see cref="XmlReader"/> class used to read the document.</param>
    /// <param name="encodingStyle">The encoding used.</param>
    /// <param name="events">An instance of the <see cref="XmlDeserializationEvents"/> class.</param>
    /// <returns>The deserialized object.</returns>
    public T Deserialize(XmlReader xmlReader, string encodingStyle, XmlDeserializationEvents events)
    {
        var result = (T)Serializer.Deserialize(xmlReader, encodingStyle, events);
        return result;
    }

    /// <summary>
    /// Serializes the specified object and writes the XML document to a file
    /// using the specified <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="textWriter">The <see cref="TextWriter"/> used to write the XML document.</param>
    /// <param name="object">The object to serialize.</param>
    public void Serialize(TextWriter textWriter, T @object)
    {
        Serializer.Serialize(textWriter, @object);
    }

    /// <summary>
    /// Serializes the specified object and writes the XML document to a file
    /// using the specified <see cref="XmlWriter"/>.
    /// </summary>
    /// <param name="xmlWriter">The <see cref="XmlWriter"/> used to write the XML document.</param>
    /// <param name="object">The object to serialize.</param>
    /// <exception cref="InvalidOperationException">
    /// An error occurred during serialization. The original exception is available
    /// using the <see cref="P:Exception.InnerException"/> property.
    /// </exception>
    public void Serialize(XmlWriter xmlWriter, T @object)
    {
        Serializer.Serialize(xmlWriter, @object);
    }

    /// <summary>
    /// Serializes the specified object and writes the XML document to a file
    /// using the specified <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> used to write the XML document.</param>
    /// <param name="object">The object to serialize.</param>
    /// <exception cref="InvalidOperationException">
    /// An error occurred during serialization. The original exception is available
    /// using the <see cref="Exception.InnerException"/> property.
    /// </exception>
    public void Serialize(Stream stream, T @object)
    {
        Serializer.Serialize(stream, @object);
    }

    /// <summary>
    /// Serializes the specified object and writes the XML document to a file
    /// using the specified <see cref="Stream"/> that references the specified namespaces.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> used to write the XML document.</param>
    /// <param name="object">The object to serialize.</param>
    /// <param name="namespaces">The <see cref="XmlSerializerNamespaces"/> referenced by the object.</param>
    /// <exception cref="InvalidOperationException">
    /// An error occurred during serialization. The original exception is available
    /// using the <see cref="Exception.InnerException"/> property.
    /// </exception>
    public void Serialize(Stream stream, T @object, XmlSerializerNamespaces namespaces)
    {
        Serializer.Serialize(stream, @object, namespaces);
    }

    /// <summary>
    /// Serializes the specified object and writes the XML document to a file
    /// using the specified <see cref="TextWriter"/> and references the specified namespaces.
    /// </summary>
    /// <param name="textWriter">The <see cref="TextWriter"/> used to write the XML document.</param>
    /// <param name="object">The object to serialize.</param>
    /// <param name="namespaces">
    /// The <see cref="XmlSerializerNamespaces"/> that contains namespaces
    /// for the generated XML document.
    /// </param>
    public void Serialize(TextWriter textWriter, T @object, XmlSerializerNamespaces namespaces)
    {
        Serializer.Serialize(textWriter, @object, namespaces);
    }

    /// <summary>
    /// Serializes the specified object and writes the XML document to a file 
    /// using the specified <see cref="XmlWriter"/> and references the specified namespaces.
    /// </summary>
    /// <param name="xmlWriter">The <see cref="XmlWriter"/> used to write the XML document.</param>
    /// <param name="object">The object to serialize.</param>
    /// <param name="namespaces">
    /// The <see cref="XmlSerializerNamespaces"/> that contains namespaces
    /// for the generated XML document.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// An error occurred during serialization. The original exception is available
    /// using the <see cref="P:Exception.InnerException"/> property.
    /// </exception>
    public void Serialize(XmlWriter xmlWriter, T @object, XmlSerializerNamespaces namespaces)
    {
        Serializer.Serialize(xmlWriter, @object, namespaces);
    }

    /// <summary>
    /// Serializes the specified object and writes the XML document to a file using the
    /// specified <see cref="XmlWriter"/> and references the specified namespaces and encoding
    /// style.
    /// </summary>
    /// <param name="xmlWriter">The <see cref="XmlWriter"/> used to write the XML document.</param>
    /// <param name="object">The object to serialize.</param>
    /// <param name="namespaces">The <see cref="XmlSerializerNamespaces"/> referenced by the object.</param>
    /// <param name="encodingStyle">The encoding style of the serialized XML.</param>
    public void Serialize(XmlWriter xmlWriter, T @object, XmlSerializerNamespaces namespaces, string encodingStyle)
    {
        Serializer.Serialize(xmlWriter, @object, namespaces, encodingStyle);
    }

    /// <summary>
    /// Serializes the specified object and writes the XML document to a file
    /// using the specified System.Xml.XmlWriter, XML namespaces, and encoding.
    /// </summary>
    /// <param name="xmlWriter">The <see cref="XmlWriter"/> used to write the XML document.</param>
    /// <param name="object">The object to serialize.</param>
    /// <param name="namespaces">
    /// An instance of the <see cref="XmlSerializerNamespaces"/> that contains namespaces and prefixes
    /// to use.
    /// </param>
    /// <param name="encodingStyle">The encoding used in the document.</param>
    /// <param name="id">For SOAP encoded messages, the base used to generate id attributes.</param>
    public void Serialize(XmlWriter xmlWriter, T @object, XmlSerializerNamespaces namespaces, string encodingStyle, string id)
    {
        Serializer.Serialize(xmlWriter, @object, namespaces, encodingStyle, id);
    }
    #endregion // Public_Not_Virtual

    #region Base_class_overrides

    /// <inheritdoc/>
    public override void Serialize(T obj, TextWriter tw)
    {
        ArgumentNullException.ThrowIfNull(tw);
        Serializer.Serialize(tw, obj);
        tw.Flush();
    }

    /// <inheritdoc/>
    public override void Serialize(T obj, XmlWriter xw)
    {
        ArgumentNullException.ThrowIfNull(xw);
        Serializer.Serialize(xw, obj);
        xw.Flush();
    }

    /// <summary>
    /// Deserializes the XML document contained by the specified <see cref="TextReader"/>.
    /// </summary>
    /// <param name="textReader">The <see cref="TextReader"/> that contains the XML document to deserialize.</param>
    /// <returns>The deserialized object.</returns>
    /// 
    /// <exception cref="InvalidOperationException">
    /// An error occurred during deserialization. The original exception is available
    /// using the <see cref="Exception.InnerException"/> property.
    /// </exception>
    /// <exception cref="ArgumentNullException"> Thrown when the argument <paramref name="textReader"/> is null. </exception>
    public override T Deserialize(TextReader textReader)
    {
        ArgumentNullException.ThrowIfNull(textReader);

        T result = (T)Serializer.Deserialize(textReader);
        return result;
    }
    #endregion // Base_class_overrides

    #endregion // Methods
}
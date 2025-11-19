///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// The Code Project Open License Notice
// 
// This software is a Derivative Work based upon a Code Project article
// XmlSerializer and 'not expected' Inherited Types
// http://www.codeproject.com/Articles/8644/XmlSerializer-and-not-expected-Inherited-Types
//
// The Code Project Open License (CPOL) text is available at
// http://www.codeproject.com/info/cpol10.aspx
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Ignore Spelling: Codeproject, deserialization, deserialized, enums, Serializer, Utils
//
using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PK.PkUtils.XmlSerialization;

/// <summary>
/// <para>
/// The purpose of this class is to assist with XML serialization, specifically to overcome the "not expected" error
/// that occurs when serializing derived classes using <see cref="XmlSerializer"/>. <br/>
/// The <see cref="XmlSerializer"/> generates an on-the-fly assembly for serialization and deserialization of a given type.
/// Therefore, the exact (concrete) type of the object and its public properties must be known at compile time.
/// If a public member of a base type is set to a derived type at runtime, a <see cref="System.InvalidOperationException"/> is thrown.
/// </para>
/// <para>
/// Workarounds include custom XML serialization (which can be complex) or using <see cref="XmlIncludeAttribute"/> (which is limited to known types).
/// </para>
/// <para>
/// Simon Hewitt's solution uses <see cref="IXmlSerializable"/> to mitigate this by-design issue. <br/>
/// See: <a href="http://www.codeproject.com/KB/XML/xmlserializerforunknown.aspx">
/// Codeproject: XmlSerializer and 'not expected' Inherited Types
/// </a>
/// </para>
/// <para>
/// This generic class encapsulates the logic, so you only need to decorate your public members of base type(s)
/// (that may be substituted with derived types at runtime) with an attribute.
/// </para>
/// <para>
/// Example usage: <br/>
/// <code>
/// <![CDATA[
/// class SerializedClass
/// {
///   [XmlElement("Developer", Type = typeof(NodeSerializer<Company>))]
///   public Company Developer { get; set; }
///
///   [XmlArrayItem("Code", typeof(NodeSerializer<Cheat>))]
///   [XmlArray("CheatCodes")]
///   public Cheat[] CheatCodes { get; set; }
/// }
/// ]]>
/// </code>
/// </para>
/// <para>
/// You can also use <see cref="NodeSerializer{T}"/> directly in generic code where the type may not support <see cref="IXmlSerializable"/>.
/// </para>
/// </summary>
/// <remarks>
/// See also:
/// <list type="bullet">
/// <item><see href="http://www.codeproject.com/KB/XML/xmlserializerforunknown.aspx">Codeproject: XmlSerializer and 'not expected' Inherited Types</see></item>
/// <item><see href="http://www.softwarerockstar.com/2006/12/using-ixmlserializable-to-overcome-not-expected-error-on-derived-classes/">softwarerockstar.com: Using IXmlSerializable To Overcome “not expected” Error On Derived Classes.</see></item>
/// </list>
/// </remarks>
/// <typeparam name="T">The base type of the element whose instances (or instances of derived types) will be serialized.</typeparam>
[Serializable]
[CLSCompliant(true)]
public class NodeSerializer<T> : IXmlSerializable
{
    #region Fields

    /// <summary> 
    /// Holds the object that this serializer operates on. 
    /// </summary>
    private T _node;

    /// <summary>
    /// Indicates whether type information should be omitted in <see cref="ReadXml"/> and <see cref="WriteXml"/> methods.
    /// If true, the code assumes that all serialized data are always of type T, and no instance has a type derived from T.
    /// This is useful for types that cannot be derived from (e.g., enums or sealed classes).
    /// </summary>
    [NonSerialized]
    private readonly bool _omitTypeInfo;

    /// <summary>
    /// The last exception that occurred in <see cref="ReadXml(XmlReader)"/>, if any.
    /// </summary>
    private static Exception _readEx = null;

    // XML attribute names for type information
    private const string _strAttrType = "type";
    private const string _strAttrRuntimeType = "RuntimeType";
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Standard parameterless constructor.
    /// </summary>
    public NodeSerializer()
    { }

    /// <summary> 
    /// Constructor providing the serialized node. 
    /// </summary>
    /// <param name="node">The value of the serialized node.</param>
    public NodeSerializer(T node)
      : this(node, false)
    { }

    /// <summary>
    /// Constructor providing the serialized node and initializing <see cref="_omitTypeInfo"/>.
    /// </summary>
    /// <param name="node">The value of the serialized node.</param>
    /// <param name="bOmitTypeInfo">If true, type information is omitted in XML.</param>
    public NodeSerializer(T node, bool bOmitTypeInfo)
    {
        this._node = node;
        this._omitTypeInfo = bOmitTypeInfo;
    }

    /// <summary>
    /// Constructor initializing <see cref="_omitTypeInfo"/>.
    /// </summary>
    /// <param name="omitTypeInfo">If true, type information is omitted in XML.</param>
    public NodeSerializer(bool omitTypeInfo)
    {
        this._omitTypeInfo = omitTypeInfo;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Gets the last exception that occurred in <see cref="ReadXml(XmlReader)"/>, if any.
    /// </summary>
    public static Exception LastTypeReadError { get => _readEx; }

    /// <summary> 
    /// Gets the serialized node.
    /// </summary>
    public T Node { get => this._node; }

    /// <summary>
    /// Gets the value of <see cref="_omitTypeInfo"/>, as set by the constructor.
    /// </summary>
    protected bool OmitTypeInfo { get => _omitTypeInfo; }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Converts the actual type to a string for writing into the XML attribute.
    /// Derived classes can override this (e.g., to use <see cref="Type.AssemblyQualifiedName"/>).
    /// </summary>
    /// <param name="t">The <see cref="Type"/> to process.</param>
    /// <returns>The string representation of <paramref name="t"/>.</returns>
    /// <remarks>This method is called by <see cref="IXmlSerializable.WriteXml"/>.</remarks>
    protected virtual string Type2String(Type t)
    {
        ArgumentNullException.ThrowIfNull(t);
#if DEBUG
        string strAsmName = t.Assembly.GetName().Name;
        string result = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", t.FullName, strAsmName);
        return result;
#else
        return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", t.FullName, t.Assembly.GetName().Name);
#endif // DEBUG
    }

    /// <summary>
    /// Implicit operator casting the serialized node <typeparamref name="T"/> to <see cref="NodeSerializer{T}"/>.
    /// </summary>
    /// <remarks>
    /// Called by the .NET serialization framework when storing a property of type T, if decorated with the appropriate Xml attribute.
    /// </remarks>
    /// <param name="node">The value of the serialized node; may be null.</param>
    /// <returns>A new <see cref="NodeSerializer{T}"/> instance, or null if <paramref name="node"/> is null.</returns>
    public static implicit operator NodeSerializer<T>(T node)
    {
        return node == null ? null : new NodeSerializer<T>(node);
    }

    /// <summary>
    /// Implicit operator casting the <see cref="NodeSerializer{T}"/> to the T object.
    /// </summary>
    /// <remarks>
    /// Called by the .NET serialization framework when reading a property of type T, if decorated with the appropriate Xml attribute.
    /// </remarks>
    /// <param name="serializer">The serializer instance.</param>
    /// <returns>The deserialized value of type T, or default if <paramref name="serializer"/> is null.</returns>
    public static implicit operator T(NodeSerializer<T> serializer)
    {
        return (serializer == null) ? default : serializer.Node;
    }
    #endregion // Methods

    #region implementation of IXmlSerializable

    /// <summary>
    /// Returns the XML schema for the serialized object.
    /// </summary>
    /// <returns>Always returns null (no schema provided).</returns>
    public XmlSchema GetSchema()
    {
        return null;
    }

    /// <summary>
    /// Generates an object from its XML representation.
    /// The actual type of the serialized object is restored from the XML attribute "type",
    /// then a new <see cref="XmlSerializer"/> is generated for its deserialization.
    /// If the serialized object is itself a <see cref="Type"/>, the fully-qualified name is read from the "RuntimeType" attribute.
    /// </summary>
    /// <param name="reader">The <see cref="XmlReader"/> stream from which the object is deserialized.</param>
    public void ReadXml(XmlReader reader)
    {
        string strRuntimeType = reader.GetAttribute(_strAttrRuntimeType);

        _readEx = null;

        if (!string.IsNullOrEmpty(strRuntimeType))
        {  // Handle the situation when the value of this.Node is itself a Type
            this._node = (T)(object)Type.GetType(strRuntimeType);
        }
        else if (OmitTypeInfo)
        {
            // Omit type info: always deserialize as T
            reader.ReadStartElement();
            this._node = (T)new XmlSerializer(typeof(T)).Deserialize(reader);
            reader.ReadEndElement();
        }
        else
        {
            // Read the type from the XML attribute and deserialize accordingly
            Type type;
            string strType = reader.GetAttribute(_strAttrType);

            try
            {
                type = Type.GetType(strType, true);
            }
            catch (Exception ex)
            {
                _readEx = ex;
                // Use 'throw' without an argument, in order to preserve the stack location 
                // where the exception was initially raised
                throw;
            }

            if (type != null)
            {
                _readEx = null;
                reader.ReadStartElement();
                this._node = (T)new XmlSerializer(type).Deserialize(reader);
                reader.ReadEndElement();
            }
        }
    }

    /// <summary>
    /// Writes an object into its XML representation.
    /// The actual type of the object is stored in the XML attribute "type",
    /// then a new <see cref="XmlSerializer"/> is generated for its serialization.
    /// If the object itself is a <see cref="Type"/>, the fully-qualified name is written into the "RuntimeType" attribute.
    /// </summary>
    /// <param name="writer">The <see cref="XmlWriter"/> stream to which the object is serialized.</param>
    public void WriteXml(XmlWriter writer)
    {
        Type t = this.Node.GetType();
        string strVal = Type2String(t);

        // Handle the situation when the value of this.Node is itself a Type
        if (t.Name.Equals(_strAttrRuntimeType, StringComparison.Ordinal))
        {   // see http://stackoverflow.com/questions/12306/can-i-serialize-a-c-type-object
            string strRuntimeType = Type2String((Node as Type));
            writer.WriteAttributeString(_strAttrRuntimeType, strRuntimeType);
        }
        else if (OmitTypeInfo)
        {
            // Omit type info: always serialize as T
            new XmlSerializer(typeof(T)).Serialize(writer, this.Node);
        }
        else
        {
            // Write the type info and serialize using the actual type
            writer.WriteAttributeString(_strAttrType, strVal);
            new XmlSerializer(t).Serialize(writer, this.Node);
        }
    }
    #endregion // implementation of IXmlSerializable
}

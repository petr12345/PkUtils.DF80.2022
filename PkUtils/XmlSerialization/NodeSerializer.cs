/***************************************************************************************************************
*
* FILE NAME:   .\XmlSerialization\NodeSerializer.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The class NodeSerializer, related to xml serializing
*
**************************************************************************************************************/
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


// Ignore Spelling: Utils, Serializer
//
using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PK.PkUtils.XmlSerialization
{
    /// <summary>
    /// <para>
    /// The purpose of this class is to assist with XMLSerialization - to help to overcome "not expected" error on
    /// derived classes. <br/>
    /// The problem with XmlSerializer is that it works by generating an on-the-fly assembly behind the scenes,
    /// that has logic for serialization and deserialization of a given type.  <br/>
    /// For this reason, the exact (concrete) type of the object and it's public properties must be known to the
    /// compiler at the time of that assembly code generation and compilation. If we try to serialize an object
    /// that, for example has a public member of a given base type, and if the public member was set to a derived
    /// type, then at run-time we receive a <see cref="System.InvalidOperationException"/>.
    /// </para>
    /// <para>
    /// The workaround to above problem has generally been resorting to custom XML serialization, which can get
    /// pretty complicated at times. 
    /// The other option ( usage of XmlIncludeAttribute ) is limited again to previously know types.
    /// </para>
    /// <para>
    /// An ingenious solutions was discovered by Simon Hewitt using
    /// <see cref="System.Xml.Serialization.IXmlSerializable"/> interface that allows us to mitigate the by-design
    /// issue of XmlSerializer object. <br/>
    /// For more info see
    /// <a href="http://www.codeproject.com/KB/XML/xmlserializerforunknown.aspx">
    /// Codeproject: XmlSerializer and 'not expected' Inherited Types
    /// </a>
    /// </para>
    /// 
    /// <para>
    /// While that solution works very well, it requires that a new class is created for each base class that we
    /// need serialization support for. Using C# generics instead eliminates the need for such new classes, and is
    /// encapsulating the grunt work into just one class. <br/>
    /// 
    /// Once this class has been added to our project (or referenced from another assembly), all we really need to
    /// do is to decorate any of our public members of base type(s)
    /// (that may be substituted with derived types at runtime) with an attribute.
    /// </para>
    /// Code example:  <br/>
    /// <code>
    /// <![CDATA[
    /// class SerializedClass
    /// {
    ///   ...
    ///   [XmlElement("Developer", Type = typeof(PK.PkUtils.XmlSerialization.NodeSerializer<Company>))]
    ///   public Company Developer
    ///   {
    ///     get { return element_developer; }
    ///     set { element_developer = value; }
    ///   }
    ///   ...
    ///   
    /// // or for an XML array serialization:
    /// 
    ///   [XmlArrayItem("Code", typeof(NodeSerializer<Cheat>)]
    ///		[XmlArray("CheatCodes")]
    ///		public Cheat[] CheatCodes 
    ///   {
    ///      get { ... }
    ///      set { ... }
    ///   }
    ///   ...
    /// }
    /// ]]>
    /// </code>
    /// </summary>
    /// 
    /// <remarks>
    /// The other way how to use the NodeSerializer is involving it directly in XML serialization code, for
    /// instance in case of code of generic class, which wants to serialize the member field which has the type
    /// given by generic argument; hence you are not sure whether that type will support IXmlSerializable or not,
    /// and you don't want to specify constraints on that. <br/>
    /// Code example: <br/>
    /// <code>
    /// <![CDATA[
    /// public void ReadXml(System.Xml.XmlReader reader)
    /// {
    ///     if (reader.IsStartElement())
    ///     {
    ///       // 1. read position
    ///       reader.ReadStartElement();
    ///       Pos = reader.ReadElementContentAsInt(_strAttrPosition, string.Empty);
    ///       // 2. read TFIELDID
    ///       if (reader.IsStartElement(_strAttrFieldType))
    ///       {
    ///          // if TFIELDID is a class, must create an instance now
    ///          if (typeof(TFIELDID).IsClass)
    ///          {
    ///             // Note:
    ///             // If there is a 'new()' constraint applied on TFIELDID,
    ///             // one could create its instance by new.
    ///             // However, TFIELDID does not have to be a class
    ///             // ( as I want to make it quite general, so I avoid applying 'class' constraint ),
    ///             // hence I do not want to apply the 'new()' constraint as well.
    ///             //
    ///             // So, I have to go through more elaborate process with reflection,
    ///             // to find the constructor and invoke it.
    /// 
    ///             /* _what = new TFIELDID(); */
    ///             ConstructorInfo constructorInfo = typeof(TFIELDID).GetConstructor(
    ///                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null);
    ///             _what = (TFIELDID)constructorInfo.Invoke(new object[] { });
    ///          }
    ///          IXmlSerializable iSer;
    ///          if (null != (iSer = What as IXmlSerializable))
    ///          {
    ///              iSer.ReadXml(reader);
    ///          }
    ///          else
    ///          {
    ///              var serializer = new NodeSerializer<TFIELDID>(typeof(TFIELDID).IsEnum);
    ///              serz.ReadXml(reader);
    ///              _what = (TFIELDID)serializer;
    ///          }
    ///       }
    ///       reader.Skip();
    ///       reader.ReadEndElement();
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </remarks>
    /// <typeparam name="T"> The base of element whose instances - or instances of derived  types - will be
    ///  serialized. </typeparam>
    /// <seealso href="http://www.codeproject.com/KB/XML/xmlserializerforunknown.aspx"> Codeproject: XmlSerializer
    ///  and 'not expected' Inherited Types</seealso>
    /// <seealso href="http://www.softwarerockstar.com/2006/12/using-ixmlserializable-to-overcome-not-expected-error-on-derived-classes/">
    ///  softwarerockstar.com: Using IXmlSerializable To Overcome “not expected” Error On Derived Classes. 
    ///  </seealso>
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
        /// You will set _bOmitTypeInfo to true, if the note type should be omitted in ReadXml and  WriteXml methods,
        /// for the purpose of simplicity.  In that case, the code assumes that actual serialized data are always of type T,
        /// and no actual instance has type derived form T.
        /// This simplification be done for instance in situation if the concrete type actually cannot be derived from ( like enum type,
        /// or sealed class).
        /// </summary>
        [NonSerialized]
        private readonly bool _bOmitTypeInfo;

        /// <summary>
        /// The last exception that occurred in public void ReadXml(XmlReader reader) (if any)
        /// </summary>
        private static Exception _readEx = null;

        // various attribute names
        private const string _strAttrType = "type";
        private const string _strAttrRuntimeType = "RuntimeType";
        #endregion // Fields

        #region Constructor(s)

        /// <summary>
        /// Standard argument-less constructor.
        /// </summary>
        public NodeSerializer()
        { }

        /// <summary> 
        /// The constructor providing serialized node. 
        /// </summary>
        /// <param name="node"> The value of the serialized node.</param>
        public NodeSerializer(T node)
          : this(node, false)
        { }

        /// <summary>
        /// The constructor providing serialized node and initializing this._bOmitTypeInfo.
        /// </summary>
        /// <param name="node"> The value of the serialized node.</param>
        /// <param name="bOmitTypeInfo"> Initial value of <see cref="OmitTypeInfo"/> property. <br/>
        ///  You will set bOmitTypeInfo to true, if the node type should be omitted in ReadXml and  WriteXml methods,
        /// for the purpose of simplicity.  In that case, the code assumes that actual serialized data are always of type T,
        /// and no actual instance has a type derived form T. <br/>
        /// This simplification be done for instance in situation if the concrete type actually cannot be derived from ( like enum type,
        /// or sealed class).
        /// </param>
        public NodeSerializer(T node, bool bOmitTypeInfo)
        {
            this._node = node;
            this._bOmitTypeInfo = bOmitTypeInfo;
        }

        /// <summary>
        /// The constructor and initializing this._bOmitTypeInfo.
        /// </summary>
        /// <param name="bOmitTypeInfo"> Initial value of <see cref="OmitTypeInfo"/> property. <br/>
        ///  You will set bOmitTypeInfo to true, if the node type should be omitted in ReadXml and  WriteXml methods,
        /// for the purpose of simplicity.  In that case, the code assumes that actual serialized data are always of type T,
        /// and no actual instance has a type derived form T. <br/>
        /// This simplification be done for instance in situation if the concrete type actually cannot be derived from ( like enum type,
        /// or sealed class).
        /// </param>
        public NodeSerializer(bool bOmitTypeInfo)
        {
            this._bOmitTypeInfo = bOmitTypeInfo;
        }
        #endregion // Constructor(s)

        #region Properties

        /// <summary> 
        /// Property returning the serialized node.
        /// </summary>
        public T Node
        {
            get => this._node;
        }

        /// <summary>
        /// Get the last exception that occurred in public void ReadXml(XmlReader reader) (if any)
        /// </summary>
        public static Exception LastTypeReadError
        {
            get => _readEx;
        }

        /// <summary>
        /// Return the value of read-only field _bOmitTypeInfo, that has been set by constructor.
        /// </summary>
        protected bool OmitTypeInfo
        {
            get { return _bOmitTypeInfo; }
        }
        #endregion // Properties

        #region Methods

        /// <summary> Virtual method for converting the actual type to the string information written into xml
        /// attribute. Derived class can overwrite that ( for instance use t.AssemblyQualifiedName ) </summary>
        ///
        /// <param name="t"> The Type to process. </param>
        ///
        /// <returns> The string representation of  type  <paramref name="t"/> </returns>
        /// <remarks> This method is called by <see cref="IXmlSerializable.WriteXml"/> method."/></remarks>
        protected virtual string Type2String(Type t)
        {
#if DEBUG
            string strAsmName = t.Assembly.GetName().Name;
            string result = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", t.FullName, strAsmName);
            return result;
#else
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", t.FullName, t.Assembly.GetName().Name);
#endif // DEBUG
        }

        /// <summary> Implicit operator casting the serialized node T to the NodeSerializer. </summary>
        ///
        /// <remarks> Will be called by NET serialization framework when storing a property T, thanks to the Xml
        /// attribute that you used for decoration of that property. </remarks>
        ///
        /// <param name="node"> The value of the serialized node; may be null.</param>
        /// <returns> A new NodeSerializer instance, or null if the <paramref name="node"/> is null.</returns>
        public static implicit operator NodeSerializer<T>(T node)
        {
            return node == null ? null : new NodeSerializer<T>(node);
        }

        /// <summary>
        /// Implicit operator casting the NodeSerializer to the T object.<br/>
        /// Will be called by .NET serialization framework when reading a property T,
        /// thanks to the Xml attribute that you used for decoration of that property.
        /// </summary>
        /// <param name="serializer"> The serializer </param>
        /// <returns> A new instance of T.</returns>
        public static implicit operator T(NodeSerializer<T> serializer)
        {
            return (serializer == null) ? default : serializer.Node;
        }
        #endregion // Methods

        #region implementation of IXmlSerializable

        /// <summary>
        /// Overwrites IXmlSerializable.GetSchema. 
        /// Should return an XmlSchema that describes the XML representation of the object 
        /// that is produced by the WriteXml method and consumed by the ReadXml method.
        /// </summary>
        /// <returns> Actually does not return any specific schema, always returns null. </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Overwrites IXmlSerializable.ReadXml. Generates an object from its XML representation. 
        /// The actual type of the serialized object is restored here from the XML attribute "type",
        /// then a new XmlSerializer is generated for its serialization.
        /// Note: The exception is the case if the serialized object is itself a type.
        /// In that case, we read fully-qualified name from XML attribute "RuntimeType".
        /// </summary>
        /// 
        /// <remarks>
        /// WriteXml should NOT write the wrapper element. For more info see
        /// <a href="http://social.msdn.microsoft.com/Forums/en-US/xmlandnetfx/thread/27e77baa-67d0-4e15-a345-a6c314e924de">
        /// IXmlSerializable ReadXml question</a>
        /// </remarks>
        /// 
        /// <param name="reader">The XmlReader stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
            string strRuntimeType = reader.GetAttribute(_strAttrRuntimeType);

            _readEx = null;

            if (!string.IsNullOrEmpty(strRuntimeType))
            {  // handling the situation when the value of this.Node is itself System.Type
                this._node = (T)(object)Type.GetType(strRuntimeType);
            }
            else if (OmitTypeInfo)
            {
                reader.ReadStartElement();
                this._node = (T)new XmlSerializer(typeof(T)).Deserialize(reader);
                reader.ReadEndElement();
            }
            else
            {
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
        /// Overwrites <see cref="IXmlSerializable.WriteXml"/>. Writes an object into its XML representation.
        /// The actual type of the object is stored here into the XML attribute "type",
        /// then a new XmlSerializer is generated for its serialization.
        /// The only exception is the case when the object itself is System.Type;
        /// in that case we write fully-qualified name into the XML attribute "RuntimeType"
        /// </summary>
        /// 
        /// <remarks>
        /// WriteXml should NOT write the wrapper element. For more info see
        /// <a href="http://social.msdn.microsoft.com/Forums/en-US/xmlandnetfx/thread/27e77baa-67d0-4e15-a345-a6c314e924de">
        /// IXmlSerializable ReadXml question</a>
        /// </remarks>
        /// 
        /// <param name="writer">The XmlWriter stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            Type t = this.Node.GetType();
            string strVal = Type2String(t);

            // handling the situation when the value of this.Node is itself System.Type
            if (t.Name.Equals(_strAttrRuntimeType, StringComparison.Ordinal))
            {   // see http://stackoverflow.com/questions/12306/can-i-serialize-a-c-type-object
                string strRuntimeType = Type2String((Node as Type));
                writer.WriteAttributeString(_strAttrRuntimeType, strRuntimeType);
            }
            else if (OmitTypeInfo)
            {
                new XmlSerializer(typeof(T)).Serialize(writer, this.Node);
            }
            else
            {
                // just an ordinary type, go the original way
                writer.WriteAttributeString(_strAttrType, strVal);
                new XmlSerializer(t).Serialize(writer, this.Node);
            }
        }
        #endregion // implementation of IXmlSerializable
    }
}

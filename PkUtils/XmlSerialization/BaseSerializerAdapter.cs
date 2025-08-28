/***************************************************************************************************************
*
* FILE NAME:   .\XmlSerialization\BaseSerializerAdapter.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The class BaseSerializerAdapter, related to xml serializing
*
**************************************************************************************************************/
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


// Ignore Spelling: Serializer, Utils
//
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;


namespace PK.PkUtils.XmlSerialization
{
    /// <summary>
    /// <para>
    /// The general Xml serialization utility. It provides common access to similar functionality of
    /// <see href="http://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx">
    /// XmlSerializer </see> and
    /// <see href="http://social.msdn.microsoft.com/Search/en-US?query=DataContractSerializer&amp;emptyWatermark=true&amp;ac=4">
    /// DataContractSerializer </see>
    /// thus implementing an <see href="http://en.wikipedia.org/wiki/Adapter_pattern"> Adapter</see> design pattern.
    /// </para>
    /// 
    /// <para>
    /// The class is abstract, and only the derived class will contain particular implementation of abstract methods.
    /// This will be done for instance by using a XmlSerializer or a DataContractSerializer in derived classes.
    /// </para>
    /// 
    /// <para>
    /// Note: Some of methods writing to a file do write the XML declaration which specifies the version of XML
    /// being used (the part of the prologue in first line, in the form like 
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-8"?>
    /// ]]>
    /// , while the others do not. Each relevant method has a note regarding this.
    /// </para>
    /// </summary>
    ///
    /// <typeparam name="T">  The type of object being serialized. </typeparam>
    ///
    /// <seealso href="http://en.wikipedia.org/wiki/Adapter_pattern"> Wikipedia about Adapter pattern</seealso>
    /// <seealso href="http://www.comptechdoc.org/independent/web/xml/guide/xmlstructure.html"> The XML Document Structure. </seealso>
    [CLSCompliant(true)]
    public abstract class BaseSerializerAdapter<T>
    {
        #region Fields

        /// <summary>
        /// An underlying field for property <see cref="BaseSerializerAdapter{T}.WriteFormatting"/>.
        /// </summary>
        private Formatting _writeFormatting = Formatting.Indented;
        #endregion // Fields

        #region Constructors

        /// <summary> Default argument-less constructor.</summary>
        protected BaseSerializerAdapter()
        {
        }

        /// <summary>
        /// The constructor accepting <see cref="System.Xml.Formatting"/>.
        /// </summary>
        /// 
        /// <param name="formatting"> The xml result formatting; the value is used as initialization of 
        /// property <see cref="WriteFormatting"/>. </param>
        protected BaseSerializerAdapter(Formatting formatting)
        {
            this.WriteFormatting = formatting;
        }
        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Should be the output of Write operations Indented or not?
        /// The default value is System.Xml.Formatting.Indented.
        /// </summary>
        public Formatting WriteFormatting
        {
            get { return _writeFormatting; }
            set { _writeFormatting = value; }
        }

        /// <summary>
        /// The cached typeof(T) value
        /// </summary>
        public Type ArgumentType
        {
            get { return typeof(T); }
        }
        #endregion // Properties

        #region Methods

        #region Public Methods

        #region Non-abstract Public Methods

        /// <summary>
        /// Writes object data to a xml file. Note: This method does NOT write the XML declaration line.
        /// </summary>
        ///
        /// <exception cref="IOException">      Thrown when an IO failure occurred. </exception>
        /// <exception cref="SystemException">  Thrown when a System error condition occurs. </exception>
        ///
        /// <param name="strFileName">  The file name, that will be used by writing StreamWriter. </param>
        /// <param name="encoding">     The resulting file encoding. </param>
        /// <param name="obj">          The written object. </param>
        ///
        /// <returns> True on success, false on failure. </returns>

        public bool WriteFile(string strFileName, Encoding encoding, T obj)
        {
            string xml = this.Serialize(obj, false).OuterXml;
            using StreamWriter writer = new(strFileName, false, encoding);
            writer.Write(xml);
            writer.Flush();

            return true;
        }

        /// <summary> Write the object data to a xml file, including the XML declaration line. This method DOES write
        /// the XML declaration line. </summary>
        ///
        /// <remarks> One does NOT need to include IFormatProvider argument here, since XmlSerialization
        /// always uses the <see cref="System.Xml.XmlConvert"/>, which serializes with CultureInfo.InvariantCulture.
        /// </remarks>
        ///
        /// <exception cref="IOException">  Thrown when an IO failure occurred. </exception>
        /// <exception cref="SystemException"> Thrown when a System error condition occurs. </exception>
        ///
        /// <param name="strFileName"> The file name, that will be used by XmlTextWriter when creating writing XmlWriter. </param>
        /// <param name="encoding">   The encoding of created file. May be null, in that case the default
        /// encoding will be derived from default encoding of new <see cref="XmlWriterSettings "/> instance. </param>
        /// 
        /// <param name="obj">         The object being serialized. </param>
        ///
        /// <returns> True on success, false on failure. </returns>
        public bool WriteXmlFile(string strFileName, Encoding encoding, T obj)
        {
            XmlDocument doc = this.Serialize(obj, true);
            bool ok = WriteXmlDocument(strFileName, encoding, doc);

            return ok;
        }

        /// <summary> Writes given XmlDocument  to a xml file, including the XML declaration line. This method DOES write
        /// the XML declaration line. </summary>
        ///
        /// <remarks> One does NOT need to include IFormatProvider argument here, since XmlSerialization
        /// always uses the <see cref="System.Xml.XmlConvert"/>, which serializes with CultureInfo.InvariantCulture.
        /// </remarks>
        ///
        /// <exception cref="IOException">  Thrown when an IO failure occurred. </exception>
        /// <exception cref="SystemException"> Thrown when a System error condition occurs. </exception>
        /// 
        /// <param name="strFileName"> The file name, that will be used by XmlTextWriter when creating writing XmlWriter. </param>
        /// 
        /// <param name="encoding">    The encoding of created file. May be null, in that case the default
        /// encoding will be derived from default encoding of new
        /// <see cref="XmlWriterSettings "/>instance. </param>
        /// <param name="doc">The serialized document.  Must not equal to null, otherwise ArgumentNullException is thrown.</param>
        /// 
        /// <returns> True on success, false on failure. </returns>
        public bool WriteXmlDocument(string strFileName, Encoding encoding, XmlDocument doc)
        {
            ArgumentNullException.ThrowIfNull(doc);

            using XmlWriter writer = XmlTextWriter.Create(strFileName, MakeXmlWriterSettings(false, encoding));
            doc.Save(writer);

            return true;
        }

        /// <summary> Serializes to an XmlDocument. </summary>
        ///
        /// <remarks>
        /// One does NOT need to include IFormatProvider argument here, since XmlSerialization always uses
        /// the System.Xml.XmlCovert which serializes with CultureInfo.InvariantCulture. </remarks>
        ///
        /// <param name="obj">                Object or object graph to serialize. </param>
        /// <param name="preserveXmlHeader"> If true, the serialization will preserve the XML declaration
        ///  in first line, in the form like.
        /// <![CDATA[
        /// <?xml version="1.0" encoding="utf-8"?>
        /// ]]>
        /// If false, the declaration is not preserved. </param>
        ///
        /// <returns> XmlDocument instance. </returns>
        public XmlDocument Serialize(T obj, bool preserveXmlHeader)
        {
            string xml = StringSerialize(obj, preserveXmlHeader);
            var doc = new XmlDocument
            {
                PreserveWhitespace = (this.WriteFormatting == Formatting.Indented)
            };
            doc.LoadXml(xml);
            doc = Clean(doc);
            return doc;
        }

        /// <summary>
        /// Serializes the object to xml string, internally using a TextWriter
        /// </summary>
        /// 
        /// <remarks>
        /// One does NOT need to include IFormatProvider argument here, since XmlSerialization always 
        /// uses the System.Xml.XmlCovert which serializes with CultureInfo.InvariantCulture
        /// </remarks>
        /// 
        /// <param name="obj">The object being serialized.</param>
        /// <param name="preserveXmlHeader"> If true, the serialization will preserve the XML declaration
        ///  in first line, in the form like.
        /// <![CDATA[
        /// <?xml version="1.0" encoding="utf-8"?>
        /// ]]>
        /// If false, the declaration is not preserved. </param>
        /// 
        /// <returns>The xml string representation of object serialized.</returns>
        public string StringSerialize(T obj, bool preserveXmlHeader)
        {
            string xml;

            using (var w = new StringWriter(CultureInfo.InvariantCulture))
            {
                using XmlWriter xw = XmlTextWriter.Create(w, MakeXmlWriterSettings(!preserveXmlHeader, null));
                Serialize(obj, xw);
                xw.Flush();
                xml = w.ToString();
            }

            return xml;
        }

        /// <summary>Reads object data from the file.</summary>
        /// <param name="strFileName">File path and name.</param>
        /// <returns>The resulting T instance.</returns>
        /// <remarks>>Calls the overloaded method ReadFile, with the argument makeNewOnError = false</remarks>
        public T ReadFile(string strFileName)
        {
            T result;
            string xml = string.Empty;

            // StreamReader defaults to UTF-8 encoding unless specified otherwise
            using (StreamReader reader = new(strFileName))
            {
                xml = reader.ReadToEnd();
            }
            result = this.Deserialize(xml);
            return result;
        }

        /// <summary>Deserialize the XML document represented by the string <paramref name="xml"/> to an instance of T.</summary>
        /// <param name="xml">String containing an xml document.</param>
        /// <returns>The resulting T instance.</returns>
        public T Deserialize(string xml)
        {
            using TextReader reader = new StringReader(xml);
            return Deserialize(reader);
        }

        /// <summary>Deserialize given <see cref="XmlDocument "/> to an instance of T.</summary>
        /// <param name="doc">XmlDocument instance.</param>
        /// <returns>The resulting T instance.</returns>
        /// <exception cref="System.ArgumentNullException"> Thrown when the input argument <paramref name="doc"/> is null.</exception>
        public T Deserialize(XmlDocument doc)
        {
            ArgumentNullException.ThrowIfNull(doc);

            using TextReader reader = new StringReader(doc.OuterXml);
            return Deserialize(reader);
        }
        #endregion // Non-abstract Public Methods

        #region Abstract Public Methods
        /// <summary>
        /// Serializes the object to the xml format into the given TextWriter
        /// </summary>
        /// <remarks>
        /// One does NOT need to include IFormatProvider argument here, since XmlSerialization always 
        /// uses the System.Xml.XmlCovert which serializes with CultureInfo.InvariantCulture.
        /// </remarks>
        /// <param name="obj">The object being serialized.</param>
        /// <param name="tw"> The <see cref="TextWriter "/> used to write the XML document.</param>
        public abstract void Serialize(T obj, TextWriter tw);

        /// <summary>
        /// Serializes the object to the xml format into the given XmlTextWriter
        /// </summary>
        /// <param name="obj">The object being serialized.</param>
        /// <param name="xw"> The <see cref="XmlWriter"/>used to write the XML document.</param>
        public abstract void Serialize(T obj, XmlWriter xw);

        /// <summary>Deserialize the XML document contained by the specified <see cref="TextReader "/> to an instance of T.</summary>
        /// <param name="textReader">TextReader instance.</param>
        /// <returns>The resulting T instance.</returns>
        public abstract T Deserialize(TextReader textReader);

        #endregion // Abstract Public Methods
        #endregion // Public Methods

        #region Protected Methods

        /// <summary>
        /// Returns the XmlWriterSettings that will be used for the XmlTextWriter, created by the calling code.
        /// </summary>
        ///
        /// <remarks>
        /// Any code overwriting this method should create new XmlWriterSettings with NewLineHandling.None,
        /// or to call this base class implementation to reuse. It should be used to deal with the
        /// newline(s) problem in an empty xml elements. See more detailed description on
        ///  http://stackoverflow.com/questions/2479871/xmltextwriter-writefullendelement-tags-on-the-same-line
        /// </remarks>
        ///
        /// <param name="omitXmlDeclaration">A value indicating whether to write an XML declaration.</param>
        /// <param name="encoding"> The optional argument (may be null). If it is not null, the encoding of
        ///   created XmlWriterSettings will be assigned to encoding.Value. </param>
        ///
        /// <returns> New XmlWriterSettings that will be used by an involved <see cref="XmlWriter"/>. </returns>
        ///
        /// <seealso href="http://stackoverflow.com/questions/2479871/xmltextwriter-writefullendelement-tags-on-the-same-line">
        /// Stackoverflow - XmlTextWriter.WriteFullEndElement tags on the same line</seealso>
        protected internal virtual XmlWriterSettings MakeXmlWriterSettings(bool omitXmlDeclaration, Encoding encoding)
        {
            Formatting formatting = this.WriteFormatting;
            var writerSettings = new XmlWriterSettings
            {
                Indent = (formatting == System.Xml.Formatting.Indented),
                NewLineHandling = NewLineHandling.None,
                OmitXmlDeclaration = omitXmlDeclaration,
                CloseOutput = false
            };

            if (null != encoding)
            {
                writerSettings.Encoding = encoding;
            }
            return writerSettings;
        }

        /// <summary>
        /// "Cleanup" of the xml document; in more detail this means removal of namespace declarations 
        /// "xmlns:xsd" and "xmlns:xsi", that are on some circumstances added by Microsoft's XML serializer.
        /// For more info, see
        /// http://stackoverflow.com/questions/5027265/whats-xmlnsmstns-in-a-xsd
        /// http://www.velocityreviews.com/forums/t683532-what-does-xmlns-xsi-and-xmlns-xsd-attributes-mean.html
        /// </summary>
        /// <param name="doc">The document being processed.</param>
        /// <returns>Returns the original document.</returns>
        protected static XmlDocument Clean(XmlDocument doc)
        {
            ArgumentNullException.ThrowIfNull(doc);

            XmlNode first = doc.FirstChild;
            // if the first child is not XmlDeclaration, the document is kind of strange, just forget about it...
            if ((first != null) && (first.NodeType == XmlNodeType.XmlDeclaration))
            {
                doc.RemoveChild(first);
                first = doc.FirstChild;
                foreach (XmlNode n in doc.ChildNodes)
                {
                    if (n.NodeType == XmlNodeType.Element)
                    {
                        first = n;
                        break;
                    }
                }
                if ((first != null) && (first.Attributes != null))
                {
                    XmlAttribute a = first.Attributes["xmlns:xsd"];
                    if (a != null) { first.Attributes.Remove(a); }
                    a = first.Attributes["xmlns:xsi"];
                    if (a != null) { first.Attributes.Remove(a); }
                }
            }
            return doc;
        }
        #endregion // Protected Methods
        #endregion // Methods
    }
}

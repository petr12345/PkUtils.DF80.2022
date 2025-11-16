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


// Ignore Spelling: Utils, Serializer
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace PK.PkUtils.XmlSerialization
{
    /// <summary>
    /// The general Xml serialization utility, internally using DataContractSerializer generated from typeof(T).
    /// 
    /// For more info see the base class BaseSerializerAdapter. </summary>
    ///
    /// <typeparam name="T">  The type of object being serialized. </typeparam>
    ///
    /// <seealso href="http://www.comptechdoc.org/independent/web/xml/guide/xmlstructure.html">
    /// The XML Document Structure</seealso>
    [CLSCompliant(true)]
    public class DataContractSerializerAdapter<T> : BaseSerializerAdapter<T>
    {
        #region Fields
        /// <summary>
        /// The <see cref="DataContractSerializer"/> that actually performs the serialization.
        /// </summary>
        private readonly DataContractSerializer _serializer;
        #endregion // Fields

        #region Constructors

        /// <summary>Default argument-less constructor.</summary>
        public DataContractSerializerAdapter()
            : this(null)
        {
        }

        /// <summary> The constructor accepting a collection of known types passed to DataContractSerializer. </summary>
        ///
        /// <param name="knownTypes"> An enumeration of types that may be present in the serialized object graph.
        /// Is subsequently used for <see cref="DataContractSerializer"/> creation. </param>
        public DataContractSerializerAdapter(IEnumerable<Type> knownTypes)
            : base()
        {
            this._serializer = new DataContractSerializer(this.ArgumentType, knownTypes);
        }

        /// <summary> The constructor. </summary>
        ///
        /// <param name="formatting"> The xml result formatting; the value is used as initialization of property
        /// WriteFormatting/>. </param>
        public DataContractSerializerAdapter(System.Xml.Formatting formatting)
            : base(formatting)
        {
            this._serializer = new DataContractSerializer(this.ArgumentType);
        }
        #endregion // Constructors

        #region Properties

        /// <summary> Returns the used DataContractSerializer </summary>
        protected DataContractSerializer Serializer
        {
            get { return _serializer; }
        }
        #endregion // Properties

        #region Methods

        #region Public Methods

        /// <summary>
        /// Serializes the object to the xml format into the given TextWriter
        /// </summary>
        /// 
        /// <remarks>
        /// One does NOT need to include IFormatProvider argument here, since XmlSerialization always 
        /// uses the System.Xml.XmlCovert which serializes with CultureInfo.InvariantCulture
        /// </remarks>
        /// 
        /// <param name="obj">The object being serialized.</param>
        /// <param name="tw"> The <see cref="TextWriter "/> used to write the XML document.</param>
        public override void Serialize(T obj, TextWriter tw)
        {
            /* looks like that settings is not needed
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.NewLineOnAttributes = true;
            using (XmlWriter xwr = XmlWriter.Create(w, settings))
            */
            using (XmlWriter xwr = XmlWriter.Create(tw))
            {
                Serializer.WriteObject(xwr, obj);
                xwr.Flush();
            }
            tw.Flush();
        }

        /// <summary>
        /// Serializes the object to the xml format into the given XmlTextWriter 
        /// </summary>
        /// <param name="obj">The object being serialized.</param>
        /// <param name="xw"> The <see cref="XmlWriter"/>used to write the XML document.</param>
        public override void Serialize(T obj, XmlWriter xw)
        {
            Serializer.WriteObject(xw, obj);
            xw.Flush();
        }

        /// <summary> Deserialize the XML document contained by the specified <see cref="TextReader "/> to an instance of T.</summary>
        /// <param name="textReader">TextReader instance.</param>
        /// <returns> The resulting T instance.</returns>
        public override T Deserialize(TextReader textReader)
        {
            /* looks like that settings is not needed
            XmlReaderSettings settings = new XmlReaderSettings();
            using (XmlReader xrd = XmlReader.Create(textReader, settings))
            */
            using XmlReader xrd = XmlReader.Create(textReader);
            return (T)Serializer.ReadObject(xrd);
        }
        #endregion // Public Methods

        #endregion // Methods
    }
}

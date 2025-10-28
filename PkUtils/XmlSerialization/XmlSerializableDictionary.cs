// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace PK.PkUtils.XmlSerialization;

/// <summary>
/// A custom dictionary whose purpose is to overcome inconvenience of XmlSerializer, that does not support
/// plain dictionary. For more information, see for instance
/// <see href="https://stackoverflow.com/questions/2911514/why-doesnt-xmlserializer-support-dictionary">
/// Why doesn't XmlSerializer support Dictionary?</see>
/// </summary>
///
/// <typeparam name="TKey">     Type of the key. </typeparam>
/// <typeparam name="TValue">   Type of the value. </typeparam>
[XmlRoot("Dictionary")]
[Serializable]
public class XmlSerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
{
    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public XmlSerializableDictionary()
    {
    }

    /// <summary> Constructor. </summary>
    ///
    /// <param name="comparer"> The comparer that should be used to compare keys. </param>
    public XmlSerializableDictionary(IEqualityComparer<TKey> comparer)
        : base(comparer)
    {
    }

    /// <summary> Constructor. </summary>
    ///
    /// <param name="dictionary"> The dictionary whose elements are copied to the new dictionary. </param>
    public XmlSerializableDictionary(IDictionary<TKey, TValue> dictionary)
        : base(dictionary)
    {
    }

    /// <summary> Constructor. </summary>
    ///
    /// <param name="dictionary">   The dictionary whose elements are copied to the new dictionary. </param>
    /// <param name="comparer">     The comparer that should be used to compare keys. </param>
    public XmlSerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        : base(dictionary, comparer)
    {
    }

    /// <summary> Specialized constructor for use by binary serialization. </summary>
    ///
    /// <param name="info">     The serialization information. </param>
    /// <param name="context">  The serialization context. </param>
    [Obsolete("Making obsolete because of StreamingContex")]
    protected XmlSerializableDictionary(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
    #endregion // Constructor(s)

    #region IXmlSerializable Members

    /// <inheritdoc/>
    public System.Xml.Schema.XmlSchema GetSchema()
    {
        return null;
    }

    /// <inheritdoc/>
    public void ReadXml(System.Xml.XmlReader reader)
    {
        var keySerializer = new XmlSerializer(typeof(TKey));
        var valueSerializer = new XmlSerializer(typeof(TValue));

        bool wasEmpty = reader.IsEmptyElement;
        reader.Read();

        if (wasEmpty)
            return;

        while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
        {
            reader.ReadStartElement("item");

            reader.ReadStartElement("key");
            var key = (TKey)keySerializer.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("value");
            var value = (TValue)valueSerializer.Deserialize(reader);
            reader.ReadEndElement();

            this.Add(key, value);

            reader.ReadEndElement();
            reader.MoveToContent();
        }
        reader.ReadEndElement();
    }

    /// <inheritdoc/>
    public void WriteXml(System.Xml.XmlWriter writer)
    {
        var keySerializer = new XmlSerializer(typeof(TKey));
        var valueSerializer = new XmlSerializer(typeof(TValue));

        foreach (TKey key in this.Keys)
        {
            writer.WriteStartElement("item");

            writer.WriteStartElement("key");
            keySerializer.Serialize(writer, key);
            writer.WriteEndElement();

            writer.WriteStartElement("value");
            TValue value = this[key];
            valueSerializer.Serialize(writer, value);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
    #endregion // IXmlSerializable Members

    #region Methods

    /// <summary>
    /// Implements the <see cref="System.Runtime.Serialization.ISerializable" /> interface and returns the data
    /// needed to serialize the generic Dictionary instance.
    /// </summary>
    ///
    /// <param name="info">    A SerializationInfo object that contains the information required to serialize
    /// the instance. </param>
    /// <param name="context"> A <see cref="StreamingContext" /> structure that
    /// contains the source and destination of the serialized stream associated with
    /// generic Dictionary instance. </param>
    [Obsolete("Making obsolete because of StreamingContex")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }
    #endregion // Methods
}

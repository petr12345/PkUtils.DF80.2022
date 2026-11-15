TestXMLSerializer is a console application demonstrating
XMLSerializerAdapter, a wrapper around System.Xml.Serialization.XmlSerializer.

Without the -d switch, it serializes sample PaymentMethod and Employee objects
into payment.xml and employee.xml in the current working directory.
With -d, it deserializes those existing files instead; run serialization first.

The source also contains an OLD_MSDN_MAGAZINE_CODE compile-time option for
comparing the adapter with the earlier MSDN example. See
XmlSerializerSamples.cs, XMLSerializerAdapter, and BaseSerializerAdapter.

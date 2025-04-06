The project TestXmlSerializer is a console application demonstrating
the usage of the class XMLSerializerAdapter.
The XMLSerializerAdapter internally encapsulates the class System.Xml.Serialization.XmlSerializer.

Usage:
If you run the application with no argument or the other argument than -d,
the code will serialize some stuff to files "payment.xml" and "employee.xml".

If you run the application with the argument -d, 
the code will deserialize the previously serialized files "payment.xml" and "employee.xml".

Remarks:
For more details see the code of XMLSerializerAdapter and its base class BaseSerializerAdapter.
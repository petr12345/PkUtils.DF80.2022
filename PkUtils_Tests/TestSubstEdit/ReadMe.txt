The TestSubstEdit project illustrates the implementation of "atomic" (indivisible) field editing 
within the edit control. 
This is achieved by subclassing the control and nearly completely replacing its original functionality. 
The project consists of two subdirectories:

1. TestSubstEdit_cpp:
   This is a C++ solution featuring a reusable static library, SubstLib, and a testing project named TestSubst.

   Key Features:
   i/ Template-based solution; any serializable class can function as a fieldId.
   ii/ Serialization to binary and plain text formats.
   iii/ Clipboard support for plain text, where special XML characters that are not part of field itself 
        are converted to XML entity representations such as "&" or "<".

2. TestSubstEdit_CS:
   This is a C# solution comprising a reusable library (assembly) SubstEditLib, and a testing project named TestTgSchema.

   Key Features:
   i/ Generic-based solution; any binary-serializable and XML-serializable class can serve as a fieldId.
   ii/ Serialization to binary, plain text, and XML. The latter is achieved by implementing IXmlSerializable in generic classes:
     - public class LogInfo<TFIELDID>
     - public class SubstLogData<TFIELDID>
   iii/ Clipboard support, with special XML characters that are not part of field itself being converted 
      to XML entity representations like "&" or "<".
  iv/ Multiple undo/redo functionality.
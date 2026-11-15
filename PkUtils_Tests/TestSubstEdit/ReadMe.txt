TestSubstEdit explores editing text containing embedded fields that must be
treated as indivisible units rather than ordinary sequences of characters.
It uses specialized edit controls and includes both C++ and C# versions.

1. TestSubstEdit_cpp
   A native C++ implementation containing a reusable static library,
   SubstLib, and a sample application, TestSubst.

   Features:
   - A template-based approach supporting serializable field identifiers.
   - Binary and plain-text serialization.
   - Clipboard support, including escaping XML-sensitive characters in text.

2. TestSubstEdit_CS
   A managed C# implementation containing the reusable SubstEditLib
   assembly and the TestTgSchema sample application.

   Features:
   - Generic field identifiers with binary and XML serialization support.
   - Binary, plain-text, and XML serialization, including IXmlSerializable
     implementations in LogInfo<TFIELDID> and SubstLogData<TFIELDID>.
   - Clipboard support with XML entity escaping, including &amp; for & and
     &lt; for <, when serializing text outside a field.
   - Multiple undo/redo operations.

See the source code in each subdirectory for the implementation details.

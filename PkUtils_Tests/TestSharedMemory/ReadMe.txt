TestSharedMemory demonstrates exchanging a serializable data object between
processes through a Windows shared-memory segment using PK.PkUtils.NativeMemory.Segment.

The folder contains three projects:

  TestSharedMemXferObj
    A shared assembly containing the PersonData type exchanged by the processes.

  TestSharedMemServer
    A console application that creates the shared-memory segment named
    "PersonDataSegment" and serializes a PersonData instance into it.

  TestSharedMemClient
    A console application that attaches to the existing segment named
    "PersonDataSegment", deserializes PersonData, and prints selected values.

Run the server before the client so that the named segment exists when the
client attempts to attach. The sample is Windows-specific.

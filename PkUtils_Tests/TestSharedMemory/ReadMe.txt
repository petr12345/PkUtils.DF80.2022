TestSharedMemory folder contains code testing the usage of the class Segment,
which wraps Win32 shared memory.

The folder contains three projects in three directories:

TestSharedMemXferObj - an assembly containing definition of the auxiliary class PersonData,
                       which is serialized to the shared segment.

TestSharedMemServer - an executable that creates a shared memory segment named "PersonDataSegment",
                    ( delegating to the other constructor with argument SharedMemoryCreationFlag.Create)
                     and then serializes an object of type PersonData into created segment.

TestSharedMemClient - an executable creating a shared memory segment object, 
                     by delegating to the other constructor with argument SharedMemoryCreationFlag.Attach,
                     thus actually attaching to shared memory segment named "PersonDataSegment" created by the server.
                     Upon success, it de-serializes an object of type PersonData stored previously
                     by the server application, and writes some of its values to console.

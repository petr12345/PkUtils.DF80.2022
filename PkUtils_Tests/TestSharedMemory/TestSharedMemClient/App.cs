using System;
using PK.PkUtils.NativeMemory;

internal class App
{
    /// <summary>
    /// Main client application entry point.
    /// 
    /// Creates a shared memory segment object,
    /// by delegating to the other constructor with argument SharedMemoryCreationFlag.Attach,
    /// thus actually attaching to shared memory segment named "PersonDataSegment" created by the server.
    /// 
    /// Upon success, it de-serializes an object of type PersonData stored previously
    /// by the server application, and writes some of its values to console.
    /// Waits for termination till any key is pressed.
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        Segment s = new("PersonDataSegment", true);
        Console.WriteLine("The client has successfully attached to shared memory segment.");
        Console.WriteLine("Reading the PersonData object stored in the segment...");

        PersonData p = (PersonData)s.GetData();

        Console.WriteLine("{0} is {1}", p.Name, p.Age);
        Console.WriteLine("Arr[1570] = {0}", (p.Arr[1570]));
    }
}

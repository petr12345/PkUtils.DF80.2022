using System;
using PK.PkUtils.NativeMemory;

internal class App
{
    /// <summary>
    /// Main server application entry point.
    /// 
    /// Creates a shared memory segment named "PersonDataSegment",
    /// ( delegating to the other constructor with argument SharedMemoryCreationFlag.Create)
    /// and then serializes an object of type PersonData into created segment.
    /// Waits for termination till any key is pressed.
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        PersonData p = new(37, "Rich");
        Segment s = new("PersonDataSegment", p, true);

        Console.WriteLine("The server has created and initialized a shared memory segment.");
        Console.WriteLine("Press any key to terminate server application...");
        Console.ReadLine();

        s.Dispose();
    }
}



Testmemcmp compares two byte-array comparison approaches and measures
their execution times:

  a) MemUtils.memcmp(byte[] arr1, byte[] arr2)
  b) Enumerable.SequenceEqual

The original experiment observed substantial speed differences for long arrays
in Release builds, but performance depends on the runtime, hardware, input
size, and test conditions. Rerun the sample before drawing conclusions about
current .NET performance.

See ComparisonTest.cs and MainForm.cs.

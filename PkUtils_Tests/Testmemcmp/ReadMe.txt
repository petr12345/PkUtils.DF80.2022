The project Testmemcmp compares various approaches for byte array comparison,
concentrating on the runtime speed.

Currently there are just two approaches:
a/ public static bool MemUtils.memcmp(byte[] arr1, byte[] arr2) 
b/ Enumerable.SequenceEqual

Notice the significant difference of the speed.
In the Release build and for long arrays ( > 10000 bytes) memcmp seems about 60-times faster.
  
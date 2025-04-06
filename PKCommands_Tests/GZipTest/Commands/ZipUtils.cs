using System.IO;
using System.IO.Compression;

namespace GZipTest.Commands;

internal static class ZipUtils
{
    /// <summary> Compresses the given byte array using GZip. </summary>
    /// <param name="bytes"> The byte array to compress. Can't be null. </param>
    /// <returns> The compressed byte array. </returns>
    internal static byte[] CompressBytes(byte[] bytes)
    {
        using MemoryStream memoryStream = new();
        using GZipStream gzipStream = new(memoryStream, CompressionMode.Compress, leaveOpen: true);

        gzipStream.Write(bytes, 0, bytes.Length);
        gzipStream.Flush();

        return memoryStream.ToArray();
    }

    /// <summary> Decompresses the given compressed byte array using GZip. </summary>
    /// <param name="compressedBytes"> The compressed byte array. Can't be null. </param>
    /// <returns> The decompressed byte array. </returns>
    internal static byte[] DecompressBytes(byte[] compressedBytes)
    {
        using MemoryStream memoryStream = new(compressedBytes);
        using GZipStream gzipStream = new(memoryStream, CompressionMode.Decompress);
        using MemoryStream decompressedStream = new();

        gzipStream.CopyTo(decompressedStream);

        return decompressedStream.ToArray();
    }
}

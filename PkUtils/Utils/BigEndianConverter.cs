// Ignore Spelling: Utils, Endian
//
using System;
using System.Collections.Generic;
using System.Linq;


namespace PK.PkUtils.Utils;

/// <summary>
/// A static class containing various conversions of base data types to an array of bytes, and an array
/// of bytes to base data types. <br/>
/// When converting the value to the array of bytes, the input value is current-platform-dependent,
/// and the output array is in BigEndian order. <br/>
/// When converting from the array of bytes to value, the input array, is in BigEndian order,
/// and the output value is current-platform-dependent. <br/>
/// </summary>
/// 
/// <seealso href="http://msdn.microsoft.com/en-us/library/system.bitconverter(v=vs.110).aspx">
/// BitConverter Class
/// </seealso>
[CLSCompliant(true)]
public static class BigEndianConverter
{
    /// <summary>
    /// This property returns <c>true</c> if the system is little-endian; otherwise, <c>false</c>.
    /// <para>
    /// In a little-endian system, the least significant byte is stored first. 
    /// In contrast, a big-endian system stores the most significant byte first.
    /// </para>
    /// <para>
    /// Most modern desktop and server architectures, such as x86, x86-64 (Intel/AMD), 
    /// and ARM (in most operating modes), are little-endian.
    /// Some older or specialized systems use big-endian, such as:
    /// - Motorola 68k series (used in classic Mac computers)
    /// - IBM mainframes (e.g., System/360, System/390, z/Architecture)
    /// - SPARC (used in some Sun/Oracle systems)
    /// - PowerPC (can be big-endian or little-endian depending on configuration)
    /// </para>
    /// </summary>
    public static bool IsLittleEndian => BitConverter.IsLittleEndian;

    #region Public Methods

    #region Conversions_to_byte_array

    /// <summary>
    /// Returns the specified 16-bit signed integer value as a big-endian byte array.
    /// </summary>
    public static byte[] GetBigEndianBytes(short value)
        => ConvertToBigEndian(BitConverter.GetBytes(value));

    /// <summary>
    /// Returns the specified 32-bit signed integer value as a big-endian byte array.
    /// </summary>
    public static byte[] GetBigEndianBytes(int value)
        => ConvertToBigEndian(BitConverter.GetBytes(value));

    /// <summary>
    /// Returns the specified 64-bit signed integer value as a big-endian byte array.
    /// </summary>
    public static byte[] GetBigEndianBytes(long value)
        => ConvertToBigEndian(BitConverter.GetBytes(value));

    /// <summary>
    /// Returns the specified single-precision floating point value as a big-endian byte array.
    /// </summary>
    public static byte[] GetBigEndianBytes(float value)
        => ConvertToBigEndian(BitConverter.GetBytes(value));

    /// <summary>
    /// Returns the specified double-precision floating point value as a big-endian byte array.
    /// </summary>
    public static byte[] GetBigEndianBytes(double value)
        => ConvertToBigEndian(BitConverter.GetBytes(value));

    /// <summary>
    /// Returns the specified 16-bit unsigned integer value as a big-endian byte array.
    /// </summary>
    [CLSCompliant(false)]
    public static byte[] GetBigEndianBytes(ushort value)
        => GetBigEndianBytes((short)value);

    /// <summary>
    /// Returns the specified 32-bit unsigned integer value as a big-endian byte array.
    /// </summary>
    [CLSCompliant(false)]
    public static byte[] GetBigEndianBytes(uint value)
        => GetBigEndianBytes((int)value);

    /// <summary>
    /// Returns the specified 64-bit unsigned integer value as a big-endian byte array.
    /// </summary>
    [CLSCompliant(false)]
    public static byte[] GetBigEndianBytes(ulong value)
        => GetBigEndianBytes((long)value);

    #endregion // Conversions_to_byte_array

    #region Conversions_from_byte_array

    /// <summary>
    /// Converts the given source byte sequence to little-endian ordering if necessary.
    /// </summary>
    public static byte[] ToLowEndian(IEnumerable<byte> source, bool isSourceBigEndian)
    {
        ArgumentNullException.ThrowIfNull(source);
        return [.. (IsLittleEndian == isSourceBigEndian ? source.Reverse() : source)];
    }

    /// <summary>
    /// Converts a big-endian byte array to a 16-bit signed integer.
    /// </summary>
    public static short ToInt16(byte[] arrBytes, bool isInputBigEndian)
        => BitConverter.ToInt16(ToLowEndian(arrBytes, isInputBigEndian), 0);

    /// <summary>
    /// Converts a big-endian byte array to a 32-bit signed integer.
    /// </summary>
    public static int ToInt32(byte[] arrBytes, bool isInputBigEndian)
        => BitConverter.ToInt32(ToLowEndian(arrBytes, isInputBigEndian), 0);

    /// <summary>
    /// Converts a big-endian byte array to a 64-bit signed integer.
    /// </summary>
    public static long ToInt64(byte[] arrBytes, bool isInputBigEndian)
        => BitConverter.ToInt64(ToLowEndian(arrBytes, isInputBigEndian), 0);

    /// <summary>
    /// Converts a big-endian byte array to a single-precision floating point value.
    /// </summary>
    public static float ToSingle(byte[] arrBytes, bool isInputBigEndian)
        => BitConverter.ToSingle(ToLowEndian(arrBytes, isInputBigEndian), 0);

    /// <summary>
    /// Converts a big-endian byte array to a double-precision floating point value.
    /// </summary>
    public static double ToDouble(byte[] arrBytes, bool isInputBigEndian)
        => BitConverter.ToDouble(ToLowEndian(arrBytes, isInputBigEndian), 0);

    /// <summary>
    /// Converts a big-endian byte array to a 16-bit unsigned integer.
    /// </summary>
    [CLSCompliant(false)]
    public static ushort ToUInt16(byte[] arrBytes, bool isInputBigEndian)
        => BitConverter.ToUInt16(ToLowEndian(arrBytes, isInputBigEndian), 0);

    /// <summary>
    /// Converts a big-endian byte array to a 32-bit unsigned integer.
    /// </summary>
    [CLSCompliant(false)]
    public static uint ToUInt32(byte[] arrBytes, bool isInputBigEndian)
        => BitConverter.ToUInt32(ToLowEndian(arrBytes, isInputBigEndian), 0);

    /// <summary>
    /// Converts a big-endian byte array to a 64-bit unsigned integer.
    /// </summary>
    [CLSCompliant(false)]
    public static ulong ToUInt64(byte[] arrBytes, bool isInputBigEndian)
        => BitConverter.ToUInt64(ToLowEndian(arrBytes, isInputBigEndian), 0);

    #endregion // Conversions_from_byte_array
    #endregion // Public Methods

    #region Private Helpers

    /// <summary>
    /// Helper method that returns a byte array in big-endian order regardless of platform endianness.
    /// </summary>
    private static byte[] ConvertToBigEndian(byte[] data)
        => IsLittleEndian ? [.. data.Reverse()] : data;

    #endregion // Private Helpers
}

/***************************************************************************************************************
*
* FILE NAME:   .\Utils\BigEndianConverter.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The class BigEndianConverter
*
**************************************************************************************************************/


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
    #region Properties

    /// <summary>
    /// This value is true if the architecture is little-endian; false if it is big-endian.
    /// </summary>
    public static bool IsLittleEndian
    {
        get { return BitConverter.IsLittleEndian; }
    }
    #endregion // Properties

    #region Methods

    #region Public Methods

    #region conversions_to_byte_array

    /// <summary>Returns the specified 16-bit signed integer value as an array of bytes. </summary>
    /// <param name="value" type="short">The input value. </param>
    /// <returns> An array of byte. </returns>
    public static byte[] GetBigEndianBytes(short value)
    {
        byte[] result = BitConverter.GetBytes(value);
        if (IsLittleEndian)
            result = result.Reverse().ToArray();
        return result;
    }

    /// <summary> Returns the specified 32-bit signed integer value as an array of bytes. </summary>
    /// <param name="value" type="int">The input value. </param>
    /// <returns> An array of byte. </returns>
    public static byte[] GetBigEndianBytes(int value)
    {
        byte[] result = BitConverter.GetBytes(value);
        if (IsLittleEndian)
            result = result.Reverse().ToArray();
        return result;
    }

    /// <summary> Returns the specified 64-bit signed integer value as an array of bytes. </summary>
    /// <param name="value" type="long">The input value. </param>
    /// <returns> An array of byte. </returns>
    public static byte[] GetBigEndianBytes(long value)
    {
        byte[] result = BitConverter.GetBytes(value);
        if (IsLittleEndian)
            result = result.Reverse().ToArray();
        return result;
    }

    /// <summary> Returns the specified single-precision floating point value as an array of bytes. </summary>
    /// <param name="value" type="float">The input value. </param>
    /// <returns> An array of byte. </returns>
    public static byte[] GetBigEndianBytes(float value)
    {
        byte[] result = BitConverter.GetBytes(value);
        if (IsLittleEndian)
            result = result.Reverse().ToArray();
        return result;
    }

    /// <summary> Returns the specified double-precision floating point value as an array of bytes. </summary>
    /// <param name="value" type="double"> The input value. </param>
    /// <returns> An array of byte. </returns>
    public static byte[] GetBigEndianBytes(double value)
    {
        byte[] result = BitConverter.GetBytes(value);
        if (IsLittleEndian)
            result = result.Reverse().ToArray();
        return result;
    }

    /// <summary> Returns the specified 16-bit unsigned integer value as an array of bytes. </summary>
    /// <param name="value" type="ushort">The input value. </param>
    /// <returns> An array of byte. </returns>
    [CLSCompliant(false)]
    public static byte[] GetBigEndianBytes(ushort value)
    {
        return GetBigEndianBytes((short)value);
    }

    /// <summary> Returns the specified 32-bit unsigned integer value as an array of bytes. </summary>
    /// <param name="value" type="uint">The input value. </param>
    /// <returns> An array of byte. </returns>
    [CLSCompliant(false)]
    public static byte[] GetBigEndianBytes(uint value)
    {
        return GetBigEndianBytes((int)value);
    }

    /// <summary> Returns the specified 64-bit unsigned integer value as an array of bytes. </summary>
    /// <param name="value" type="ulong">The input value. </param>
    /// <returns> An array of byte. </returns>
    [CLSCompliant(false)]
    public static byte[] GetBigEndianBytes(ulong value)
    {
        return GetBigEndianBytes((long)value);
    }
    #endregion // conversions_to_byte_array

    #region conversions_fro_byte_array

    /// <summary> Converts the byte input into low-endian collection in case it is not. </summary>
    /// <param name="source"> Source of the data. </param>
    /// <param name="isSourceBigEndian" type="bool"> true if this byte source is big-endian-based, 
    ///  false if not. </param>
    /// <returns> The given data source converted to a  low-endian order. </returns>
    public static byte[] ToLowEndian(IEnumerable<byte> source, bool isSourceBigEndian)
    {
        ArgumentNullException.ThrowIfNull(source);

        var tmp = (IsLittleEndian == isSourceBigEndian) ? source.Reverse() : source;
        return tmp.ToArray();
    }

    /// <summary> Returns a 16-bit signed integer converted from first two bytes of arrBytes. </summary>
    /// <param name="arrBytes" type="byte[]"> The array of bytes. </param>
    /// <param name="isInputBigEndian" type="bool"> true if the input array is in big endian. </param>
    /// <returns> The given data converted to a short. </returns>
    public static short ToInt16(byte[] arrBytes, bool isInputBigEndian)
    {
        return BitConverter.ToInt16(ToLowEndian(arrBytes, isInputBigEndian), 0);
    }

    /// <summary> Returns a 32-bit signed integer converted from first four bytes of arrBytes. </summary>
    /// <param name="arrBytes" type="byte[]"> The array of bytes. </param>
    /// <param name="isInputBigEndian" type="bool"> true if the input array is in big endian. </param>
    /// <returns> The given data converted to an int. </returns>
    public static int ToInt32(byte[] arrBytes, bool isInputBigEndian)
    {
        return BitConverter.ToInt32(ToLowEndian(arrBytes, isInputBigEndian), 0);
    }

    /// <summary> Returns a 64-bit signed long converted from first eight bytes of arrBytes. </summary>
    /// <param name="arrBytes" type="byte[]"> The array of bytes. </param>
    /// <param name="isInputBigEndian" type="bool"> true if the input array is in big endian. </param>
    /// <returns> The given data converted to long. </returns>
    public static long ToInt64(byte[] arrBytes, bool isInputBigEndian)
    {
        return BitConverter.ToInt64(ToLowEndian(arrBytes, isInputBigEndian), 0);
    }

    /// <summary>
    /// Returns a single-precision floating point number converted from first four bytes of arrBytes.
    /// </summary>
    /// <param name="arrBytes" type="byte[]"> The array of bytes. </param>
    /// <param name="isInputBigEndian" type="bool"> true if the input array is in big endian. </param>
    /// <returns> The given data converted to float. </returns>
    public static float ToSingle(byte[] arrBytes, bool isInputBigEndian)
    {
        return BitConverter.ToSingle(ToLowEndian(arrBytes, isInputBigEndian), 0);
    }

    /// <summary>
    /// Returns a double-precision floating point number converted from first eight bytes of arrBytes.
    /// </summary>
    /// <param name="arrBytes" type="byte[]"> The array of bytes. </param>
    /// <param name="isInputBigEndian" type="bool"> true if the input array is in big endian. </param>
    /// <returns> The given data converted to double. </returns>
    public static double ToDouble(byte[] arrBytes, bool isInputBigEndian)
    {
        return BitConverter.ToDouble(ToLowEndian(arrBytes, isInputBigEndian), 0);
    }

    /// <summary> Returns a 16-bit unsigned integer converted from first two bytes of arrBytes. </summary>
    /// <param name="arrBytes" type="byte[]"> The array of bytes. </param>
    /// <param name="isInputBigEndian" type="bool"> true if the input array is in big endian. </param>
    /// <returns> The given data converted to an unsigned short. </returns>
    [CLSCompliant(false)]
    public static ushort ToUInt16(byte[] arrBytes, bool isInputBigEndian)
    {
        return BitConverter.ToUInt16(ToLowEndian(arrBytes, isInputBigEndian), 0);
    }

    /// <summary> Returns a 32-bit unsigned integer converted from first four bytes of arrBytes. </summary>
    /// <param name="arrBytes" type="byte[]"> The array of bytes. </param>
    /// <param name="isInputBigEndian" type="bool"> true if the input array is in big endian. </param>
    /// <returns> The given data converted to an unsigned int. </returns>
    [CLSCompliant(false)]
    public static uint ToUInt32(byte[] arrBytes, bool isInputBigEndian)
    {
        return BitConverter.ToUInt32(ToLowEndian(arrBytes, isInputBigEndian), 0);
    }

    /// <summary> Returns a 64-bit unsigned long converted from first eight bytes of arrBytes. </summary>
    /// <param name="arrBytes" type="byte[]"> The array of bytes. </param>
    /// <param name="isInputBigEndian" type="bool"> true if the input array is in big endian. </param>
    /// <returns> The given data converted to unsigned long. </returns>
    [CLSCompliant(false)]
    public static ulong ToUInt64(byte[] arrBytes, bool isInputBigEndian)
    {
        return BitConverter.ToUInt64(ToLowEndian(arrBytes, isInputBigEndian), 0);
    }
    #endregion // conversions_fro_byte_array
    #endregion // Public Methods

    #region Private Methods
    #endregion // Private Methods
    #endregion // Methods
}

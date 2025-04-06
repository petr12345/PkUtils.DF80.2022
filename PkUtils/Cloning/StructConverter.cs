/***************************************************************************************************************
*
* FILE NAME:   .\Cloning\StructConverter.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains implementation of StructConverter class
*
**************************************************************************************************************/

// Ignore Spelling: Utils
//
using System;
using System.Runtime.InteropServices;
using PK.PkUtils.NativeMemory;

namespace PK.PkUtils.Cloning;

/// <summary>
/// StructConverter is a helper class, implementing for any structure conversions 
/// to and from byte array, and a ShallowClone.
/// </summary>
/// <typeparam name="S">The type of structure that is being converted.</typeparam>
[CLSCompliant(true)]
public static class StructConverter<S> where S : struct
{
    /// <summary>
    /// The size of byte array required to keep the structure S contents.
    /// </summary>
    /// <seealso cref="ToByteArray"/>
    public static int OccupiedBytes
    {
        get { return Marshal.SizeOf(typeof(S)); }
    }

    /// <summary>
    /// Conversion of given structure S to byte array.
    /// </summary>
    /// <param name="s">The structure being converted.</param>
    /// <returns>The resulting array of bytes. </returns>
    /// <seealso cref="FromByteArray"/> 
    public static byte[] ToByteArray(S s)
    {
        int nLength;
        byte[] byteArr = new byte[nLength = OccupiedBytes];
#if NOTDEF // previous version of code without usage of class UnmanagedPtr
        IntPtr p = Marshal.AllocHGlobal(nLength);

        Marshal.StructureToPtr(s, p, false);
        Marshal.Copy((IntPtr)p, byteArr, 0, nLength);
        Marshal.FreeHGlobal(p);
#endif // NOTDEF
        using (UnmanagedPtr p = new(nLength))
        {
            Marshal.StructureToPtr(s, p.PtrToUnmanagedMemory, false);
            Marshal.Copy(p.PtrToUnmanagedMemory, byteArr, 0, nLength);
        }

        return byteArr;
    }

    /// <summary>
    /// Conversion from byte array to structure
    /// </summary>
    /// <param name="arr">The input array of bytes (might be created previously by ToByteArray)</param>
    /// <returns>The converted structure</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when the input argument <paramref name="arr"/> is null.</exception>
    /// <seealso cref="ToByteArray"/>
    public static S FromByteArray(byte[] arr)
    {
        int nLength;
        S result;

        ArgumentNullException.ThrowIfNull(arr);
#if NOTDEF // previous version of code without usage of class UnmanagedPtr
        IntPtr p = Marshal.AllocHGlobal(nLength = OccupiedBytes);

        Marshal.Copy(arr, 0, p, nLength);
        result = (S)Marshal.PtrToStructure(p, typeof(S));
        Marshal.FreeHGlobal(p);
#endif // NOTDEF
        using (UnmanagedPtr p = new(nLength = OccupiedBytes))
        {
            Marshal.Copy(arr, 0, p.PtrToUnmanagedMemory, nLength);
            result = (S)Marshal.PtrToStructure(p.PtrToUnmanagedMemory, typeof(S));
        }

        return result;
    }

    /// <summary>
    /// Creates a shallow copy of a given structure, by calling <see cref="ToByteArray"/> method
    /// and afterwards calling <see cref="FromByteArray"/> on the temporary byte array.
    /// </summary>
    /// <param name="original">The structure that will be copied by shallow cloning.</param>
    /// <returns>The shallow clone of the <paramref name="original"/> instance</returns>
    public static S ShallowClone(S original)
    {
        byte[] arrTemp = ToByteArray(original);
        return FromByteArray(arrTemp);
    }
}

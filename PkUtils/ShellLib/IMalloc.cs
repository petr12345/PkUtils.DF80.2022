///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Microsoft Public License (MS-PL) notice
// 
// This software is a Derivative Work based upon a Code Project article series
// C# does Shell, Part 1 – 3	
// http://www.codeproject.com/Articles/3551/C-does-Shell-Part-1
// http://www.codeproject.com/Articles/3590/C-does-Shell-Part-2	
// http://www.codeproject.com/Articles/3728/C-does-Shell-Part-3
// published under Microsoft Public License.
// 
// The related Microsoft Public License (MS-PL) text is available at
// http://www.opensource.org/licenses/ms-pl.html
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Ignore Spelling: Malloc, Utils
//
using System;
using System.Runtime.InteropServices;


#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable SYSLIB1096 // Mark the type 'IMalloc' with 'GeneratedComInterfaceAttribute' instead of 'ComImportAttribute' to generate COM marshalling code at compile time

namespace PK.PkUtils.ShellLib;

/// <summary>
/// Defines an interface that allocates, frees, and manages memory. <br/>
/// In the context of ShellLib, you will gen an instance if this interface by calling
/// <see cref="ShellApi.SHGetMalloc"/>, or on more 'high-level' by calling
/// <see cref="ShellFunctions.GetMalloc()"/> that calls SHGetMalloc internally.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("00000002-0000-0000-C000-000000000046")]
public interface IMalloc
{
    /// <summary> Allocates a block of memory. Return value: a pointer to the allocated memory block. </summary>
    ///
    /// <param name="cb"> Size, in bytes, of the memory block to be allocated. </param>
    ///
    /// <returns>
    /// If the method succeeds, the return value is a pointer to the allocated block of memory. Otherwise, it is
    /// IntPtr.Zero (NULL).
    /// </returns>
    [PreserveSig]
    IntPtr Alloc(
      int cb);

    /// <summary>
    /// Changes the size of a previously allocated memory block.
    /// Return value:  Reallocated memory block 
    /// </summary>
    /// <param name="pv">Pointer to the memory block to be reallocated.</param>
    /// <param name="cb">Size of the memory block (in bytes) to be reallocated.</param>
    /// <returns>
    /// If the method succeeds, the return value is a pointer to the allocated block of memory. Otherwise, it is
    /// IntPtr.Zero (NULL).
    /// </returns>
    [PreserveSig]
    IntPtr Realloc(
      IntPtr pv,
      int cb);

    /// <summary>
    /// Frees a previously allocated block of memory.
    /// </summary>
    /// <param name="pv">Pointer to the memory block to be freed.</param>
    [PreserveSig]
    void Free(
      IntPtr pv);

    /// <summary>
    /// This method returns the size (in bytes) of a memory block previously allocated with 
    /// <see cref="IMalloc.Alloc"/> or <see cref="IMalloc.Realloc"/>.
    /// </summary>
    /// <param name="pv">Pointer to the memory block for which the size is requested.</param>
    /// <returns>The size of the allocated memory block in bytes</returns>
    [PreserveSig]
    int GetSize(
      IntPtr pv);

    /// <summary>
    /// This method determines whether this allocator was used to allocate the specified block of memory,
    /// represented by <paramref name="pv"/> argument.
    /// </summary>
    ///
    /// <param name="pv"> Pointer to the memory block. </param>
    ///
    /// <returns>
    /// A value representing relationship to this <paramref name="pv"/> memory block.
    /// 
    /// <list type="bullet">
    /// <item><b>1</b><br/>The block <paramref name="pv"/> has been allocated by this IMalloc instance</item>
    /// <item><b>0</b><br/>The block <paramref name="pv"/> has not been allocated by this IMalloc instance</item>
    /// <item><b>-1</b><br/>Cannot be determined.</item>
    /// </list>
    /// 
    /// </returns>
    [PreserveSig]
    int DidAlloc(
      IntPtr pv);

    /// <summary>
    /// This method minimizes the heap as much as possible by releasing unused memory to the operating system, 
    /// coalescing adjacent free blocks and committing free pages.
    /// </summary>
    [PreserveSig]
    void HeapMinimize();
}

#pragma warning restore SYSLIB1096 // Mark the type 'IMalloc' with 'GeneratedComInterfaceAttribute'
#pragma warning restore IDE0079   // Remove unnecessary suppressions
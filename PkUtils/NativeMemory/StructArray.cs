// Ignore Spelling: Utils, Realloc
//
using System;
using System.Runtime.InteropServices;

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

namespace PK.PkUtils.NativeMemory;

/// <summary>
/// Generics for implementation of array of structures allocated in native memory,
/// which is retrieved by the base <see cref="UnmanagedPtr"/> through
/// <see cref="System.Runtime.InteropServices.Marshal.AllocHGlobal(int)"/>.  <br/>
/// </summary>
/// <typeparam name="S">The type of the structure this is all about.</typeparam>
[CLSCompliant(true)]
public unsafe class StructArray<S> : UnmanagedPtr where S : struct
{
    #region Fields

    private int _size;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Constructor pre-allocating the memory for given amount of structures S".
    /// </summary>
    /// <param name="nStructuresToAllocate">An initial amount of structures for which  the memory block will be  allocated.</param>
    public StructArray(int nStructuresToAllocate)
      : base(unchecked(nStructuresToAllocate * Marshal.SizeOf(typeof(S))))
    {
        _size = nStructuresToAllocate;
    }

    /// <summary> Initializes the array of structures wrapper around a given block of memory. <br/>
    /// If the argument  <paramref name="bAttach"/> is false, this object is considered as an owner of that
    /// memory, and its Dispose or Finalize will call <see cref="Marshal.FreeHGlobal"/>. <br/>
    /// 
    /// If the argument  <paramref name="bAttach"/> is true, this object is NOT considered as an owner, and
    /// its Dispose or Finalize will NOT call <see cref="Marshal.FreeHGlobal"/>. </summary>
    ///
    /// <param name="ptrToMemory"> Pointer to unmanaged memory,  previously retrieved by
    /// <see cref="Marshal.AllocHGlobal(int)"/> </param>
    /// <param name="bAttach">     If true, this object will become an owner of  the memory
    /// represented by   <paramref name="ptrToMemory"/> . </param>
    public StructArray(IntPtr ptrToMemory, bool bAttach)
      : base(ptrToMemory, bAttach)
    {
    }
    #endregion // Constructor(s)

    /// <summary> Gets the current size ( amount of structures the buffer keeps). </summary>
    public int Size
    {
        get { return _size; }
    }

    /// <summary> Get the pointer to n-th ( nDex-th ) structure. Accompanies the indexer. </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="nDex"/> has invalid value. </exception>
    /// <param name="nDex"> An array index. </param>
    /// <returns>   Pointer to nDex-th  structure. </returns>
    public IntPtr GetStructurePtr(int nDex)
    {
        if ((nDex < 0) || (nDex >= Size))
        {
            throw new ArgumentOutOfRangeException(
                nameof(nDex), nDex, $"Value of {nameof(nDex)} can't be negative and must be less than current size {Size}.");
        }

        return (IntPtr)(((byte*)base.PtrToUnmanagedMemory) + nDex * Marshal.SizeOf(typeof(S)));
    }

    /// <summary> Indexer to get items within this collection using array index syntax. </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="nDex"/> has invalid value. </exception>
    /// <param name="nDex"> The index. </param>
    /// <returns>   The indexed item. </returns>
    public S* this[int nDex]
    {
        get { return (S*)GetStructurePtr(nDex); }
    }

    /// <summary> Re-allocates the memory buffer so it could contain new given amount of structures. </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="nStructuresToAllocate"/> has
    /// negative value. </exception>
    /// <exception cref="OutOfMemoryException"> Thrown when there is insufficient memory to satisfy the request. </exception>
    /// <param name="nStructuresToAllocate"> A new amount of structures this wrapper should keep. </param>
    /// <returns>   A new pointer to the buffer beginning. </returns>
    public IntPtr ReallocStructures(int nStructuresToAllocate)
    {
        if (nStructuresToAllocate < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(nStructuresToAllocate), nStructuresToAllocate, $"Value of {nameof(nStructuresToAllocate)} can't be negative.");
        }

        IntPtr result = base.Realloc(unchecked(nStructuresToAllocate * Marshal.SizeOf(typeof(S))));
        // OK to assign new size, no exception happened
        _size = nStructuresToAllocate;

        return result;
    }
}

#pragma warning restore CS8500
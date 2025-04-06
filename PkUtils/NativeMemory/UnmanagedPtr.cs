/***************************************************************************************************************
*
* FILE NAME:   .\NativeMemory\UnmanagedPtr.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class UnmanagedPtr
*
**************************************************************************************************************/

// Ignore Spelling: Realloc, Utils
//
using System;
using System.Runtime.InteropServices;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.NativeMemory;

/// <summary> The class UnmanagedPtr is a wrapper around a pointer to native memory block,
/// retrieved through
/// <see cref="System.Runtime.InteropServices.Marshal.AllocHGlobal(int)"/>.  <br/>
/// If you use such memory in your code, you have to free that memory properly, involving
/// <see cref="System.Runtime.InteropServices.Marshal.FreeHGlobal"/>. So, in order to wrap
/// that memory 'safely' and avoid forgetting to call Marshal.FreeHGlobal, you can use class
/// UnmanagedPtr, which implements both IDisposable and the Finalizer ( for case someone forgets to
/// call IDisposable.Dispose ). </summary>
///
/// <remarks> <see cref="Marshal.AllocHGlobal(int)"/>is one of two memory allocation API methods in the
/// <see cref="Marshal"/>class.   (While <see cref="Marshal.AllocCoTaskMem"/> is the other.)
/// This method exposes the Win32 LocalAlloc function from Kernel32.dll.  <br/>
/// 
/// When AllocHGlobal calls LocalAlloc, it passes a LMEM_FIXED flag, which causes the allocated
/// memory to be locked in place. Also, the allocated memory is not zero-filled. </remarks>
[CLSCompliant(true)]
public class UnmanagedPtr : IDisposableEx
{
    #region Fields
    private IntPtr _ptrToMemory;
    private bool _bAttached;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Construct the UnmanagedPtr wrapper, allocating its memory. </summary>
    ///
    /// <param name="nBytesToAllocate"> The number of bytes in memory required. </param>
    public UnmanagedPtr(int nBytesToAllocate)
    {
        Realloc(nBytesToAllocate);
    }

    /// <summary> Construct the UnmanagedPtr wrapper. <br/>
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
    public UnmanagedPtr(IntPtr ptrToMemory, bool bAttach)
    {
        if (bAttach)
            Attach(ptrToMemory);
        else
            _ptrToMemory = ptrToMemory;
    }
    #endregion // Constructor(s)

    #region Finalizer

    /// <summary>
    /// Finalizer
    /// </summary>
    /// <remarks>
    /// Note: the compiler actually expands the pseudo-destructor syntax here to following code:
    /// <code>
    /// protected override void Finalize()
    /// {
    ///   try
    ///   { // Your cleanup code here
    ///     Dispose(false);
    ///   }
    ///   finally
    ///   {
    ///     base.Finalize();
    ///   }
    /// }
    /// </code>
    /// </remarks>
    /// <seealso href="http://www.devx.com/dotnet/Article/33167/1954">
    /// When and How to Use Dispose and Finalize in C#</seealso>
    ~UnmanagedPtr()
    {
        Dispose(false);
    }
    #endregion // Finalizer

    #region Properties

    /// <summary>
    /// Returns pointer to unmanaged memory
    /// </summary>
    public IntPtr PtrToUnmanagedMemory
    {
        get { return _ptrToMemory; }
    }

    /// <summary>
    /// Has been attached to previously existing memory?
    /// </summary>
    protected bool IsAttached
    {
        get { return _bAttached; }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// A user-defined type conversion operator that is invoked with a cast to IntPtr.
    /// Implementation delegates the call to UnmanagedPtr.PtrToUnmanagedMemory
    /// </summary>
    /// <param name="ptr">An  UnmanagedPtr  instance that is casted (being converted)</param>
    /// <returns>A pointer to unmanaged memory</returns>
    public static explicit operator IntPtr(UnmanagedPtr ptr)
    {
        return (ptr != null) ? ptr.PtrToUnmanagedMemory : IntPtr.Zero;
    }

    /// <summary>
    /// Attach the memory. If attached, this object is NOT considered as an owner,
    /// and its dispose and finalize will NOT call Marshal.FreeHGlobal.
    /// </summary>
    /// <param name="ptrToMemory"> Pointer to unmanaged memory,  previously retrieved by
    /// <see cref="Marshal.AllocHGlobal(int)"/> </param>
    public void Attach(IntPtr ptrToMemory)
    {
        FreeMemory();
        this._ptrToMemory = ptrToMemory;
        this._bAttached = true;
    }

    /// <summary>
    /// Detach from previously attached memory, and return the pointer to that memory.
    /// If the object has not been attached, no action is done and the result is IntPtr.Zero.
    /// </summary>
    /// <returns>Pointer to previously attached memory - if there were any attached, or IntPtr.Zero</returns>
    public IntPtr Detach()
    {
        IntPtr result = IntPtr.Zero;

        if (this.IsAttached)
        {
            result = this.PtrToUnmanagedMemory;
            this._ptrToMemory = IntPtr.Zero;
            this._bAttached = false;
        }
        return result;
    }

    /// <summary>
    /// Reallocate new memory buffer with new size; 
    /// if there was an old buffer makes a copy of old original buffer contents.
    /// </summary>
    /// 
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when <paramref name="nBytesToAllocate"/> has negative value. </exception>
    /// <exception cref="OutOfMemoryException"> 
    /// Thrown when there is insufficient memory to satisfy the request. </exception>
    /// 
    /// <param name="nBytesToAllocate">New size of the buffer</param>
    /// <returns>  An IntPtr to the newly allocated memory buffer. </returns>
    public IntPtr Realloc(int nBytesToAllocate)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(nBytesToAllocate);

        if (nBytesToAllocate > 0)
        {
            if (IntPtr.Zero != PtrToUnmanagedMemory)
                _ptrToMemory = Marshal.ReAllocHGlobal(PtrToUnmanagedMemory, nBytesToAllocate);
            else
                _ptrToMemory = Marshal.AllocHGlobal(unchecked(nBytesToAllocate));
        }
        else
        {
            FreeMemory();
        }
        return PtrToUnmanagedMemory;
    }

    /// <summary>
    /// Free-up the allocated _ptrToMemory
    /// ( if I am the owner of that call Marshal.FreeHGlobal; otherwise Detach ).
    /// </summary>
    public void FreeMemory()
    {
        if (IntPtr.Zero != PtrToUnmanagedMemory)
        {
            if (IsAttached)
            {
                Detach();
            }
            else
            {
                Marshal.FreeHGlobal(PtrToUnmanagedMemory);
                _ptrToMemory = IntPtr.Zero;
            }
        }
    }
    #endregion // Methods

    #region IDisposableEx members
    #region IDisposable members

    /// <summary>
    /// Implement IDisposable.
    /// Do not make this method virtual.
    /// A derived class should not be able to override this method.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the UnmanagedPtr and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (!this.IsDisposed)
        {
            // If disposing equals true, should dispose managed resources first.
            // Actually, no managed resources for disposal in this particular case...

            // Now release unmanaged resources. If disposing is false, only that code is executed.
            FreeMemory();
        }
    }
    #endregion // Implementation of IDisposable

    /// <summary>
    /// Returns true in case the object has been disposed and no longer should be used.
    /// </summary>
    public bool IsDisposed
    {
        get { return (PtrToUnmanagedMemory == IntPtr.Zero); }
    }
    #endregion // Implementation of IDisposableEx
}

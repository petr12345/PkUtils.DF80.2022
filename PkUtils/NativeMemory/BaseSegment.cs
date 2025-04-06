/***************************************************************************************************************
*
* FILE NAME:   .\NativeMemory\BaseSegment.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The file contains definition of class BaseSegment
*
**************************************************************************************************************/

// Ignore Spelling: Utils
// 

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using PK.PkUtils.Utils;
using PK.PkUtils.WinApi;
using static System.FormattableString;

namespace PK.PkUtils.NativeMemory;

/// <summary>
/// Enum for specifying whether a new shared memory segment should be created
/// or just attached to an existing one.
/// </summary>
[CLSCompliant(true)]
public enum SharedMemoryCreationFlag
{
    /// <summary>
    /// Indicates that the constructor must create a new FileMapping object. 
    /// Fails if there is existing one of that name
    /// </summary>
    Create,

    /// <summary>
    /// Indicates that must attach to existing new FileMapping object. 
    /// Fails if there is no such existing mapping object of that name
    /// </summary>
    Attach,

    /// <summary>
    /// Indicates that should attach if there is existing object of the given name, 
    /// or create a new one if there is not.
    /// </summary>
    CreateOrAttach,
}

/// <summary> This class wraps Win32 shared memory. <br/>
/// You can store any byte array in shared memory and have it retrieved from another process on the
/// same machine. Data is stored via the <see cref="SetByteArrayData"/> method and retrieved via
/// the <see cref="GetByteArrayData"/> method.  <br/>
/// Access to the shared memory segment can be synchronized for instance using the
/// <see cref="AcquireLock"/>, which locks a named mutex.
/// </summary>
/// <seealso href="http://msdn2.microsoft.com/en-us/library/ms810428.aspx">
/// A Quick and Versatile Synchronization Object </seealso>
[CLSCompliant(true)]
public class BaseSegment : IDisposable
{
    #region Typedefs

    /// <summary> The class MutexWaitWrapper is a wrapper around <see cref="Mutex"/>.
    /// The primary purpose of the wrapper is to wrap lock-acquiring into "something IDisposable-derived".
    /// This way, the traditional try-finally code, with releasing the lock in the finally part
    /// could be replaced by more simple 
    /// <code>
    /// <![CDATA[
    /// public PersonData Read(string mappingName)
    /// {
    ///   using (Segment s = new Segment(mappingName, true))
    ///   {
    ///     using (IDisposable segmnentLock = s.AcquireLock())
    ///     {
    ///        return (PersonData)s.GetData();  
    ///     }
    ///   }
    /// }
    /// ]]>
    /// </code>
    /// </summary>
    protected class MutexLockWrapper : IDisposable
    {
        private Mutex _acquiredMutex;

        /// <summary> Specialized constructor for use only by <see cref="BaseSegment"/> and classed derived from it. 
        ///           The caller provides a mutex that should be released when disposing this lock.
        /// </summary>
        /// <exception cref="ArgumentNullException">    Thrown when <paramref name="acquiredMutex"/> is null. </exception>
        /// <param name="acquiredMutex"> The mutex acquired by calling code. </param>
        protected internal MutexLockWrapper(Mutex acquiredMutex)
        {
            ArgumentNullException.ThrowIfNull(acquiredMutex);
            _acquiredMutex = acquiredMutex;
        }

        /// <inheritdoc/>
        public bool IsDisposed { get => (_acquiredMutex == null); }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose executes in two distinct scenarios. If disposing equals true, the method has been
        /// called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed.
        /// If disposing equals false, the method has been called by the runtime from inside
        /// the finalizer and you should not reference other objects. Only unmanaged resources can be disposed.
        /// </summary>
        ///
        /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                _acquiredMutex.ReleaseMutex();
                _acquiredMutex = null;
            }
        }
    }
    #endregion // Typedefs

    #region Fields

    /// <summary> The handle returned by API CreateFileMapping/OpenFileMapping.  <br/>
    /// 
    /// The purpose of SafeFileHandle usage here is to make sure that system memory is properly
    /// released regardless of any asynchronous thread aborts. <br/>
    /// For more details see <a href="https://docs.microsoft.com/en-us/archive/msdn-magazine/2005/october/using-the-reliability-features-of-the-net-framework">
    /// Keep Your Code Running with the Reliability Features of the .NET Framework</a>
    /// by Stephen Toub, MSDN Magazine Oct 2005. </summary>
    private SafeFileHandle _nativeMappingHandle;

    /// <summary>
    /// The result of MapViewOfFile
    /// </summary>
    private IntPtr _nativeDataPtr;

    /// <summary>
    /// A named mutex, that allows to synchronize the access to the shared memory instance, 
    /// using the <see cref="AcquireLock"/>.
    /// </summary>
    private Mutex _guardMutex;

    /// <summary> The complete size of created buffer ( memory block pointed by _nativeDataPtr ).
    /// First two longs in the buffer are occupied by auxiliary header:
    /// - the first long in the buffer that keeps the size of the buffer itself
    /// - the second long stores the size of following data.  
    /// See also the related code <see cref="CopyByteArrayToSharedMemory"/> </summary>
    private readonly int _bufferSize;

    /// <summary>
    /// Is true if the constructor actually attached to an existing file mapping; false otherwise.
    /// </summary>
    private readonly bool _isAttached;

    /// <summary>
    /// Is true if the access to shared memory should be synchronized. Determined by constructor argument.
    /// </summary>
    private readonly bool _isSynchronized;

    /* Here I could store the file mapping object name.
     * But there is no purpose to keep that name once the file mapping has been constructed 
     * 
     * protected string _segmentName = string.Empty;
     */

    /// <summary>
    /// The maximum length of the mutex name. Constructor of mutex throws ArgumentException in case
    /// provided name is longer than 260 characters.
    /// </summary>
    private const int _maxMutexNameLength = 260;

    private const string _guardMutexNamePrefix = @"Global\";
    private const string _guardMutexNameSuffix = "Mutex";
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Constructs the shared memory segment through calling the API for FileMapping and the ViewOfFile.
    /// </summary>
    /// <param name="fileMappingName"> The name given to file mapping object. Can't be null or empty. </param>
    /// <param name="creationFlag"> The creation flag, either 'Create', 'Attach' or 'CreateOrAttach'</param>
    /// <param name="nBufferEffectiveSize"> The required effective size of the buffer.
    /// Has no effect if the mapping is attached to an existing object; otherwise the value must be positive. </param>
    /// <param name="synchronized"> True if the access to shared memory should be synchronized. </param>
    /// 
    /// <exception cref="ArgumentException"> Thrown if any of input arguments has illegal value. </exception>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown if <paramref name="creationFlag"/> is SharedMemoryCreationFlag.Create,
    ///                                                and the value <paramref name="nBufferEffectiveSize"/> is not positive. </exception>
    /// <exception cref="SharedMemoryException"> Thrown if <see cref="Kernel32.CreateFileMapping"/> has failed. </exception>
    public BaseSegment(
        string fileMappingName,
        SharedMemoryCreationFlag creationFlag,
        int nBufferEffectiveSize,
        bool synchronized)
    {
        int nBufferComplete, nLength;
        int lastErr;
        bool bAttached;

        if (fileMappingName == null)
        {
            throw new ArgumentNullException(nameof(fileMappingName));
        }
        else if (string.IsNullOrEmpty(fileMappingName))
        {
            throw new ArgumentException("You must supply mapping name", nameof(fileMappingName));
        }
        else if ((nLength = fileMappingName.Length) > MaxMappingNameLength)
        {
            throw new ArgumentException(
                Invariant($"The length of the {nameof(fileMappingName)} argument can be up to {MaxMappingNameLength} characters, but provided argument contains {nLength} characters."),
                nameof(fileMappingName));
        }
        else if ((nBufferEffectiveSize <= 0) && (creationFlag == SharedMemoryCreationFlag.Create))
        {
            throw new ArgumentOutOfRangeException(nameof(nBufferEffectiveSize),
                Invariant($"Value of {nameof(nBufferEffectiveSize)} must be positive, but {nBufferEffectiveSize} is not."));
        }
        // Determine buffer complete size and mutex name.  See also the comment regarding _nBufferSize 
        nBufferComplete = nBufferEffectiveSize + 2 * Marshal.SizeOf(typeof(long));

        _isSynchronized = synchronized;

        // Create named mutex, with name derived from fileMappingName
        // The name must be prefixed by "Global\", to specify the mutex being created in the Win32 global or session namespace.
        // See https://msdn.microsoft.com/en-us/library/windows/desktop/ms682411(v=vs.85).aspx
        // see also https://msdn.microsoft.com/en-us/library/system.threading.mutex.aspx
        //
        string strMutexName = _guardMutexNamePrefix + fileMappingName + _guardMutexNameSuffix;
        Exception initException = null;

        _guardMutex = new Mutex(false, strMutexName);

        // If synchronizing shared memory access, all following needs to be guarded by mutex, 
        // until contents of memory shared memory block is completely initialized.
        // 
        // The lock will work either as a read lock or write lock, depending on the segment is crated or attached.
        // 
        using (IDisposable creationLock = AcquireLock())
        {
            bool bFailed = false;
            string errorMessage = string.Empty;

            // Now Create or attach to shared memory segment
            if (creationFlag == SharedMemoryCreationFlag.Create)
            {
                // Create the shared segment
                _nativeMappingHandle = Kernel32.CreateFileMapping(
                  Kernel32.INVALID_HANDLE_VALUE_INTPTR,
                  IntPtr.Zero,
                  (uint)Kernel32.PageAccess.PAGE_READWRITE,
                  0,
                  (uint)nBufferComplete,
                  fileMappingName);
                bAttached = ((lastErr = Marshal.GetLastWin32Error()) == Kernel32.ERROR_ALREADY_EXISTS);

                // In case when creation is required, consider 'bAttached' case as an error, 
                // even if valid _nativeMappingHandle has been returned.
                bFailed = _nativeMappingHandle.IsInvalid || bAttached;
            }
            else
            {
                /* _nativeMappingHandle = Kernel32.OpenFileMapping(Kernel32.FILE_MAP_ALL_ACCESS, true, fileMappingName);
                 * lastErr = Marshal.GetLastWin32Error();
                 */

                // Could be simple as that, but there is no OpenFileMapping function on WinCE.
                // However, the loss of this API on Windows CE is not as bad as it may sound.
                // CreateFileMapping can create respective kernel objects as well as open them 
                // if given a name that refers to a preexisting object.
                // Sea also "A Quick and Versatile Synchronization Object" at
                // http://msdn2.microsoft.com/en-us/library/ms810428.aspx"
                //
                // Note: If the object exists before the function call, 
                // the function returns a handle to the existing object 
                // (with its current size, not the specified size), 
                // and GetLastError returns ERROR_ALREADY_EXISTS. 

                _nativeMappingHandle = Kernel32.CreateFileMapping(
                    Kernel32.INVALID_HANDLE_VALUE_INTPTR,
                    IntPtr.Zero,
                    (uint)Kernel32.PageAccess.PAGE_READWRITE,
                    0,
                    (uint)nBufferComplete,
                    fileMappingName);
                bAttached = ((lastErr = Marshal.GetLastWin32Error()) == Kernel32.ERROR_ALREADY_EXISTS);

                if (creationFlag == SharedMemoryCreationFlag.Attach)
                {
                    bFailed = (_nativeMappingHandle.IsInvalid || !bAttached);
                }
                else if (creationFlag == SharedMemoryCreationFlag.CreateOrAttach)
                {
                    bFailed = _nativeMappingHandle.IsInvalid;
                }
                else
                {
                    errorMessage = Invariant($"Invalid argument {nameof(creationFlag)} value: 0x{creationFlag:X}");
                    initException = new ArgumentException(errorMessage, nameof(creationFlag));
                }
            }

            if (bFailed && (initException == null))
            {
                switch (creationFlag)
                {
                    case SharedMemoryCreationFlag.Create:
                        errorMessage = Invariant($"Unable to create shared memory segment.");
                        if (lastErr == Kernel32.ERROR_ALREADY_EXISTS)
                        {
                            errorMessage += Invariant($" Shared memory segment '{fileMappingName}' already in use.");
                        }
                        break;

                    case SharedMemoryCreationFlag.Attach:
                        errorMessage = Invariant($"Unable to attach to shared memory segment with {nameof(fileMappingName)} = {fileMappingName}.");
                        break;

                    case SharedMemoryCreationFlag.CreateOrAttach:
                        errorMessage = Invariant($"Unable to create or attach to shared memory segment with {nameof(fileMappingName)} = {fileMappingName}.");
                        break;
                }

                initException = new SharedMemoryException(errorMessage, lastErr);
            }
            else
            {
                // Get pointer to shared memory segment
                _nativeDataPtr = Kernel32.MapViewOfFile(_nativeMappingHandle.DangerousGetHandle(), Kernel32.FILE_MAP_ALL_ACCESS, 0, 0, UIntPtr.Zero);
                lastErr = Marshal.GetLastWin32Error();

                if (NativeDataAsIntPtr == IntPtr.Zero)
                {
                    initException = new SharedMemoryException("Unable to map shared memory segment.", lastErr);
                }
                else
                {
                    // Complete the initialization
                    if (_isAttached = bAttached)
                        _bufferSize = ReadBufferCompleteSize();
                    else
                        WriteBufferCompleteSize(_bufferSize = nBufferComplete);
                }
            }
        }

        // It is more convenient an manageable to call the cleanup method and throw exception from the same one place,
        // and that should happen only after creationLock.Dispose() has been called by the end of 'using' scope.
        // This is because creationLock.Dispose() calls ReleaseMutex(), and that mutex is disposed by NonVirtualDispose.
        // If NonVirtualDispose is called first, there is later System.ObjectDisposedException caused by ReleaseMutex() 
        // in creationLock.Dispose().

        if (initException != null)
        {
            NonVirtualDispose(true); // call non-virtual method, preventing the call of derived class Dispose
            throw initException;
        }
    }

    /// <summary>
    /// Constructs the shared memory segment through calling the API for FileMapping and the ViewOfFile.
    /// Just shortcut to constructor with argument SharedMemoryCreationFlag.Attach.
    /// </summary>
    /// <param name="fileMappingName"> The name given to file mapping object. Can't be null or empty. </param>
    /// <param name="synchronized"> True if the access to shared memory should be synchronized. </param>
    public BaseSegment(
        string fileMappingName,
        bool synchronized)
      : this(fileMappingName, SharedMemoryCreationFlag.Attach, 0, synchronized)
    { }

    /// <summary> Constructs the shared memory segment through calling the API for FileMapping and the ViewOfFile,
    /// and copies the contents of  <paramref name="dataStream"/> to shared memory segment.<br/>
    /// The segment created by this constructor has always its memory 'created', not 'attached' ( the
    /// constructor calls the overloaded constructor with argument SharedMemoryCreationFlag.Create ).
    /// The effective size of buffer of created segment is determined by the
    /// <paramref name="dataStream"/> length. </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when a supplied  <paramref name="dataStream"/>  is null. </exception>
    /// <exception cref="ObjectDisposedException"> Thrown when a supplied  <paramref name="dataStream"/>  has been disposed. </exception>
    ///
    /// <param name="fileMappingName"> The name given to file mapping object. Can't be null or empty. </param>
    /// <param name="dataStream">  <see cref="System.IO.Stream"/> containing  data to be copied to shared memory. </param>
    /// <param name="synchronized"> True if the access to shared memory should be synchronized. </param>
    public BaseSegment(
      string fileMappingName,
      Stream dataStream,
      bool synchronized)
      : this(fileMappingName, SharedMemoryCreationFlag.Create, GetLengthOfDataToCopy(dataStream), synchronized)
    {
        CopyStreamToSharedMemory(dataStream);
    }

    /// <summary>
    /// Constructs the shared memory segment through calling the API for FileMapping and the ViewOfFile,
    /// and copies byte array data to shared memory segment.
    /// The segment created by this constructor has always its memory 'created', not 'attached'
    /// ( the method calls overloaded constructor with argument SharedMemoryCreationFlag.Create ).
    /// The effective size of buffer of created segment is determined by the array length.
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when a supplied <paramref name="data"/> is null. </exception>
    /// 
    /// <param name="fileMappingName"> The name given to file mapping object. Can't be null or empty. </param>
    /// <param name="data"> The input array of bytes. </param>
    /// <param name="synchronized"> True if the access to shared memory should be synchronized. </param>
    public BaseSegment(
        string fileMappingName,
        byte[] data,
        bool synchronized)
      : this(fileMappingName, SharedMemoryCreationFlag.Create, data.Length, synchronized)
    {
        CopyByteArrayToSharedMemory(data);
    }
    #endregion // Constructor(s)

    #region Finalizer

    /// <summary>
    /// Finalizer to free up shared memory segment native handle
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
    ~BaseSegment()
    {
        Dispose(false);
    }

    #endregion // Finalizer

    #region Properties

    /// <summary> Gets the maximum length of the mapping name provided in constructor. </summary>
    public static int MaxMappingNameLength
    {
        get
        {
            int result = _maxMutexNameLength - (_guardMutexNamePrefix.Length + _guardMutexNameSuffix.Length);
            Debug.Assert(result > 0);
            return result;
        }
    }

    /// <summary>
    /// Get the current complete size of created buffer 
    /// </summary>
    public int BufferCompleteSize
    {
        get { return _bufferSize; }
    }

    /// <summary>
    /// Get the size of the usable part of the buffer.
    /// The buffer has two long(s) in its header, so the resulting size here is
    /// BufferCompleteSize - 2*Marshal.SizeOf(typeof(long)) )
    /// </summary>
    public int BufferEffectiveSize
    {
        get
        {
            int nTemp = BufferCompleteSize;
            int nSubstract = 2 * Marshal.SizeOf(typeof(long));
            int nResult = 0;

            if (nTemp >= nSubstract)
            {
                nResult = (nTemp - nSubstract);
            }
            return nResult;
        }
    }

    /// <summary>
    /// Returns true if the constructor actually attached to an existing file mapping; false otherwise.
    /// </summary>
    public bool IsAttached
    {
        get { return _isAttached; }
    }

    /// <summary>
    /// Returns true if the access to shared memory should be synchronized.
    /// </summary>
    public bool IsSynchronized
    {
        get { return _isSynchronized; }
    }

    /// <summary>
    /// Provides access to the cross-process wait handle
    /// </summary>
    public WaitHandle WaitHandle
    {
        get { return _guardMutex; }
    }

    /// <summary>
    /// Get the result of MapViewOfFile as IntPtr
    /// </summary>
    protected IntPtr NativeDataAsIntPtr
    {
        get { return _nativeDataPtr; }
    }

    /// <summary>
    /// Get the result of MapViewOfFile as pointer void*
    /// </summary>
    private unsafe void* NativeDataAsVoidPtr
    {
        get { return NativeDataAsIntPtr.ToPointer(); }
    }
    #endregion // Properties

    #region Methods
    #region Public Methods

    /// <summary> Acquires the lock object, that - until released - guarantees the calling thread an exclusive access
    ///            to shared memory through methods that use mutex-synchronization mechanism. </summary>
    /// <param name="millisecondsTimeout"> The number of milliseconds to wait, or Infinite (-1) to wait indefinitely.. </param>
    ///
    /// <returns> If locking succeeds, the result is IDisposable-object holding the mutex 
    ///           that should be released when unlocking. If locking fails, the result will be just null.
    /// </returns>
    public IDisposable AcquireLock(int millisecondsTimeout = Timeout.Infinite)
    {
        IDisposable result = null;

        if (IsSynchronized && _guardMutex.WaitOne(millisecondsTimeout))
        {
            result = new MutexLockWrapper(_guardMutex);
        }

        return result;
    }

    /// <summary>
    /// Returns the stored data of this BaseSegment as byte array
    /// </summary>
    /// <returns> Byte Array</returns>
    public byte[] GetByteArrayData()
    {
        return CopySharedMemoryToByteArray();
    }

    /// <summary>
    /// Stores Byte array in shared memory of this BaseSegment
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied  <paramref name="data"/> is null. </exception>
    /// <param name="data"> The input array of bytes </param>
    public void SetByteArrayData(byte[] data)
    {
        CopyByteArrayToSharedMemory(data);
    }
    #endregion // Public Methods

    #region Protected methods

    /// <summary>
    /// Throws SharedMemoryException with a specific text. Just implementation helper called by other methods.
    /// </summary>
    /// <exception cref="SharedMemoryException"> Thrown when a Shared Memory error condition occurs. </exception>
    ///
    /// <param name="nLength"> The length to be used for formatting text. </param>
    protected static void ThrowTooLargeDataException(long nLength)
    {
        string errorMessage = Invariant($"The data length {nLength} to be stored is too large for the segment");
        throw new SharedMemoryException(errorMessage);
    }

    /// <summary>
    /// Checks if the available (effective) buffer size is large enough to keep data 
    /// of length nRequiredSize; throws an exception <see cref="SharedMemoryException"/> if not.
    /// </summary>
    /// <param name="neededSize">Required size of buffer in bytes.</param>
    protected void CheckBufferEffectiveSize(long neededSize)
    {
        if (BufferEffectiveSize < neededSize)
        {
            ThrowTooLargeDataException(neededSize);
        }
    }

    /// <summary>
    /// This is the SAVING method.
    /// Copies a Byte array to shared memory segment using unsafe pointers.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied  <paramref name="data"/> is null. </exception>
    /// <param name="data"> The input array of bytes</param>
    /// <seealso cref="CopySharedMemoryToByteArray"/>
    protected unsafe void CopyByteArrayToSharedMemory(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        long dataLen = data.Length;

        // CheckBufferEffectiveSize could be done outside the lock
        CheckBufferEffectiveSize(dataLen);

        using IDisposable writeLock = AcquireLock();
        byte* dest = (byte*)NativeDataAsVoidPtr;

        dest += Marshal.SizeOf(typeof(long)); // skip first long in the buffer that keeps the size of buffer itself
        *(long*)dest = dataLen;               // store the size of following data
        dest += Marshal.SizeOf(typeof(long)); // increment pointer again
        Marshal.Copy(data, 0, (IntPtr)dest, (int)dataLen);  // now store the data
    }

    /// <summary>
    /// This is READING method.
    /// Copies the shared memory data to output byte array.
    /// </summary>
    /// <returns>The resulting array of bytes. </returns>
    /// <seealso cref="CopyByteArrayToSharedMemory"/>
    protected unsafe byte[] CopySharedMemoryToByteArray()
    {
        using IDisposable readLock = AcquireLock();
        long dataLen;
        byte* source = (byte*)NativeDataAsVoidPtr;

        source += Marshal.SizeOf(typeof(long)); // skip first long in the buffer that keeps the size of buffer itself
        dataLen = *(long*)source;               // get the stored data Length
        source += Marshal.SizeOf(typeof(long)); // set the source data pointer to start of serialized object graph

        // Create a byte array to hold the serialized data
        byte[] data = new byte[dataLen];
        // Copy the shared memory data to byte array
        Marshal.Copy((IntPtr)source, data, 0, (int)dataLen);

        return data;
    }

    /// <summary>
    /// This is SAVING method.
    /// Copies the contents of input stream to shared memory segment, using temporary byte array.
    /// </summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied  <paramref name="stream"/> is null. </exception>
    /// <exception cref="ObjectDisposedException"> Thrown when a supplied  <paramref name="stream"/>  has been disposed. </exception>
    /// 
    /// <param name="stream"> System.IO.Stream - data to be copied to shared memory. Can't be null. </param>
    protected void CopyStreamToSharedMemory(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        // Note no write lock is needed here, existing lock inside CopyByteArrayToSharedMemory is sufficient.
        // 
        // Read stream data into byte array
        byte[] data;
        int nLength = GetLengthOfDataToCopy(stream);

        using (BinaryReader reader = new(stream, Encoding.Default, leaveOpen: true))
        {
            data = reader.ReadBytes(nLength);
        }
        Debug.Assert(nLength == GetLengthOfDataToCopy(stream), "assure reader closing did not dispose the stream");

        // Copy the byte array to shared memory
        CopyByteArrayToSharedMemory(data);
    }

    /// <summary>
    /// This is READING method.
    /// Copies shared memory data to passed stream, using temporary byte array.
    /// </summary>
    /// <param name="stream">System.IO.Stream - stream to receive data. Can't be null. </param>
    protected void CopySharedMemoryToStream(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        // Note no read lock is needed here, existing lock inside CopySharedMemoryToByteArray is sufficient
        // 
        // Copy the shared memory data to byte array
        byte[] data = CopySharedMemoryToByteArray();

        // Write the byte array to the stream. 
        // Must use specific constructor of BinaryWriter, to dispose it properly without closing used stream
        using (BinaryWriter writer = new(stream, Encoding.Default, leaveOpen: true))
        {
            writer.Write(data);
        }
        Debug.Assert(0 <= GetLengthOfDataToCopy(stream), "assure writer closing did not dispose the stream");

        // Reset stream to start
        stream.Seek(0, SeekOrigin.Begin);
    }

    /// <summary>
    /// Dispose executes in two distinct scenarios. If disposing equals true, the method has been
    /// called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed.
    /// If disposing equals false, the method has been called by the runtime from inside
    /// the finalizer and you should not reference other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by finalizer. </param>
    protected virtual void Dispose(bool disposing)
    {   // Specific implementation - call an extra method to actually do the job.
        // See the comment for NonVirtualDispose why this is needed
        NonVirtualDispose(disposing);
    }
    #endregion // Protected methods

    #region Private methods

    /// <summary>
    /// Returns the integer representing the Length of data that should be copied. 
    /// Throws an exception if the stream is too long ( out of integer range )
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    private static int GetLengthOfDataToCopy(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        long nLength = stream.Length;
        if (nLength > int.MaxValue)
        {
            ThrowTooLargeDataException(nLength);
        }
        return (int)nLength;
    }

    /// <summary>
    /// Auxiliary method called by constructor, 
    /// reads the complete size of attached buffer from its header
    /// </summary>
    private unsafe int ReadBufferCompleteSize()
    {
        long* source = (long*)NativeDataAsVoidPtr;
        long nResult = *source;

        return (int)nResult;
    }

    /// <summary>
    /// Auxiliary method called by constructor, 
    /// writes the complete size of attached buffer to its header
    /// </summary>
    private unsafe void WriteBufferCompleteSize(int nBufferComplete)
    {
        long* dest = (long*)NativeDataAsVoidPtr;
        *dest = nBufferComplete;
    }
    #endregion // Private methods
    #endregion // Methods

    #region IDisposable members

    /// <summary>
    /// IDisposable.Dispose allow timely clean up and removed the need for finalization
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// The cleanup method, called by Dispose and on some scenarios by constructor.
    /// The reason of existence of this method is that I need non-virtual method 
    /// callable by the constructor, having the same functionality as Dispose, 
    /// but which does not call derived class dispose.
    /// Note:
    /// In C#, calling virtual method always involved calling the method of derived class 
    /// who overloaded it, even if the call is from constructor 
    /// ( and the derived class is not fully constructed yet ).
    /// Also, I cannot use some syntax to enforce 'exactly this' method calling, like
    /// <code>
    /// Segment.Dispose(true);  or this.BaseSegment.Dispose(true);
    /// </code>
    /// since both does not compile in C#.
    /// </summary>
    /// <param name="disposing"></param>
    private void NonVirtualDispose(bool disposing)
    {
        // 1. if being called from IDisposable.Dispose, clean up managed resources first
        if (disposing)
        {
            // Note it is safe to close the mutex here even if constructor did not created the "real" Win32 mutex as new,
            // because "real" Win32 mutex is destroyed when last handle to it is closed.
            Disposer.SafeDispose(ref _guardMutex);
        }

        // 2. Now clean up unmanaged resources.
        // i/ First get rid of the result of MapViewOfFile
        if (NativeDataAsIntPtr != IntPtr.Zero)
        {
            Kernel32.UnmapViewOfFile(_nativeDataPtr);
            _nativeDataPtr = IntPtr.Zero;
        }
        // ii/ Now get rid of file mapping itself
        Disposer.SafeDispose(ref _nativeMappingHandle);
    }
    #endregion // IDisposable members
}

// Ignore Spelling: Utils
//
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using PK.PkUtils.Utils;
using PK.PkUtils.WinApi;


namespace PK.PkUtils.IO;

/// <summary> An abstract file system item locker. 
///           Works as common base class for FolderLock and FileLock. </summary>
[CLSCompliant(false)]
public abstract class LockFileSystemItem : IDisposable
{
    #region Fields

    /// <summary> SafeFileHandle value of previously open file system item. </summary>
    private SafeFileHandle _itemHandle;

    /// <summary> Value indicating whether this object has locked given file system item. </summary>
    private bool _itemLocked;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Specialized default constructor for use only by derived classes. </summary>
    protected LockFileSystemItem()
    { }
    #endregion // Constructor(s)

    /// <summary>
    /// The Finalizer. Uses C# destructor syntax for generation of finalizer method code.
    /// The actually generated method (finalizer) will run only if the Dispose method
    /// does not get called.
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
    ~LockFileSystemItem()
    {
        Dispose(false);
    }
    #region Properties

    /// <summary> Gets a value indicating whether this object has open given file system item. </summary>
    public bool IsItemOpen
    {
        get { return (ItemHandle != null) && (!ItemHandle.IsInvalid); }
    }

    /// <summary> Gets a value indicating whether this object has locked given file system item. </summary>
    public bool IsItemLocked
    {
        get { return IsItemOpen && _itemLocked; }
    }

    /// <summary> Gets a SafeFileHandle value of previously open file system item. </summary>
    public SafeFileHandle ItemHandle
    {
        get { return _itemHandle; }
        protected set { _itemHandle = value; }
    }
    #endregion // Properties

    #region Methods

    /// <summary> Closes this object, i.e. unlocks and closes the item handle. </summary>
    public void Close()
    {
        UnlockItemHandle();
        CloseItemHandle();
    }

    /// <summary> Opens item handle. </summary>
    ///
    /// <exception cref="Win32Exception"> Thrown when item handle could not be open and
    /// <paramref name="throwOnError"/> is true. </exception>
    ///
    /// <param name="item">         The item. </param>
    /// <param name="fileShare">    The file share. </param>
    /// <param name="throwOnError"> true to throw Win32Exception when encountering an error. </param>
    protected void OpenItemHandle(FileSystemInfo item, FileShare fileShare, bool throwOnError)
    {
        int nLastError;

        Debug.Assert(!IsItemOpen);
        Debug.Assert(item != null);

#if CREATEFILE_RETURNS_SIMPLE_HANDLE
        IntPtr handle = Kernel32.CreateFile(
          item.FullName,
          Kernel32.EFileAccess.GenericWrite,
          fileShare,
          IntPtr.Zero,
          FileMode.Open,
          (uint)Kernel32.CreateFileFlags.FILE_FLAG_BACKUP_SEMANTICS,
          IntPtr.Zero);
        nLastError = Marshal.GetLastWin32Error();
        _itemHandle = new SafeFileHandle(handle, true);
#else
        _itemHandle = Kernel32.CreateFile(
          item.FullName,
          Kernel32.EFileAccess.GenericWrite,
          fileShare,
          IntPtr.Zero,
          FileMode.Open,
          (uint)Kernel32.CreateFileFlags.FILE_FLAG_BACKUP_SEMANTICS,
          IntPtr.Zero);
        nLastError = Marshal.GetLastWin32Error();
#endif // CREATEFILE_RETURNS_SIMPLE_HANDLE

        if (ItemHandle.IsInvalid)
        {
            _itemHandle = null;
            if (throwOnError)
            {
                throw new Win32Exception(nLastError);
            }
        }
    }

    /// <summary> Dispose item handle. </summary>
    protected void CloseItemHandle()
    {
        Disposer.SafeDispose(ref _itemHandle);
    }

    /// <summary> Locks the item handle. </summary>
    ///
    /// <exception cref="Win32Exception"> Thrown when item handle could not be locked
    /// and <paramref name="throwOnError"/> is true. </exception>
    /// <param name="throwOnError"> true to throw Win32Exception when encountering an error. </param>
    protected void LockItemHandle(bool throwOnError)
    {
        LockItemHandle(throwOnError, 0, 0, 0, 0);
    }

    /// <summary> Locks the item handle. </summary>
    /// <exception cref="Win32Exception"> Thrown when item handle could not be locked and
    ///  <paramref name="throwOnError"/> is true. </exception>
    /// <param name="throwOnError"> true to throw Win32Exception when encountering an error. </param>
    /// <param name="dwFileOffsetLow"> The file offset low. </param>
    /// <param name="dwFileOffsetHigh"> The file offset high. </param>
    /// <param name="nNumberOfBytesToLockLow"> Number of bytes to lock lows. </param>
    /// <param name="nNumberOfBytesToLockHigh"> Number of bytes to lock highs. </param>
    protected void LockItemHandle(
        bool throwOnError,
        uint dwFileOffsetLow,
        uint dwFileOffsetHigh,
        uint nNumberOfBytesToLockLow,
        uint nNumberOfBytesToLockHigh)
    {
        int nLastError;

        if (IsItemOpen)
        {
            Debug.Assert(!_itemLocked);
            _itemLocked = Kernel32.LockFile(ItemHandle.DangerousGetHandle(),
              dwFileOffsetLow, dwFileOffsetHigh, nNumberOfBytesToLockLow, nNumberOfBytesToLockHigh);
            nLastError = Marshal.GetLastWin32Error();

            if (!IsItemLocked && throwOnError)
            {
                throw new Win32Exception(nLastError);
            }
        }
    }

    /// <summary> Unlocks the item handle. </summary>
    protected void UnlockItemHandle()
    {
        if (IsItemLocked)
        {
            Debug.Assert(ItemHandle != null);
            Debug.Assert(!ItemHandle.IsInvalid);
            Kernel32.UnlockFile(ItemHandle.DangerousGetHandle(), 0, 0, 0, 0);
            _itemLocked = false;
        }
    }

    /// <summary> Opens and Locks the given <paramref name="item"/>. </summary>
    ///
    /// <exception cref="ArgumentNullException"> Passed when argument <paramref name="item"/> is null. </exception>
    /// <exception cref="ArgumentException"> Passed when the argument <paramref name="item"/> does not
    ///  exist in the file system. </exception>
    /// <exception cref="Win32Exception"> Passed either when item handle could not be open and
    ///  <paramref name="throwOnOpenError"/> is true, or when item handle could not be locked and
    ///  <paramref name="throwOnLockError"/> is true. </exception>
    /// 
    /// <param name="item"> The item in the file system. </param>
    /// <param name="fileShare"> The file share used for opening the item. </param>
    /// <param name="throwOnOpenError"> true to throw an exception on item open error. </param>
    /// <param name="throwOnLockError"> true to throw an exception on item lock error. </param>
    protected virtual void OpenAndLockItem(
        FileSystemInfo item,
        FileShare fileShare,
        bool throwOnOpenError,
        bool throwOnLockError)
    {
        OpenAndLockItem(item, fileShare, throwOnOpenError, throwOnLockError, 0, 0, 0, 0);
    }

    /// <summary> Opens and Locks the given <paramref name="item"/>. </summary>
    /// <exception cref="ArgumentNullException"> Passed when the required argument <paramref name="item"/>
    ///  is null. </exception>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal
    ///  values. </exception>
    /// <exception cref="Win32Exception"> Passed either when item handle could not be open and
    ///  <paramref name="throwOnOpenError"/> is true, or when item handle could not be locked and
    ///  <paramref name="throwOnLockError"/> is true. </exception>
    /// 
    /// <param name="item"> The item in the file system. </param>
    /// <param name="fileShare"> The file share used for opening the item. </param>
    /// <param name="throwOnOpenError"> true to throw an exception on item open error. </param>
    /// <param name="throwOnLockError"> true to throw an exception on item lock error. </param>
    /// <param name="dwFileOffsetLow"> The file offset low. </param>
    /// <param name="dwFileOffsetHigh"> The file offset high. </param>
    /// <param name="nNumberOfBytesToLockLow"> Number of bytes to lock lows. </param>
    /// <param name="nNumberOfBytesToLockHigh"> Number of bytes to lock highs. </param>
    protected virtual void OpenAndLockItem(
        FileSystemInfo item,
        FileShare fileShare,
        bool throwOnOpenError,
        bool throwOnLockError,
        uint dwFileOffsetLow,
        uint dwFileOffsetHigh,
        uint nNumberOfBytesToLockLow,
        uint nNumberOfBytesToLockHigh)
    {
        // 1. check arguments
        ArgumentNullException.ThrowIfNull(item);
        item.Refresh();

        if (!item.Exists)
        {
            string strMsg = string.Format(CultureInfo.InvariantCulture,
              "The item '{0}' does not exist", item.FullName);
            throw new ArgumentException(strMsg, nameof(item));
        }

        // 2. close if previously open
        Close();

        // 3. open item handle
        OpenItemHandle(item, fileShare, throwOnOpenError);

        // 4. lock item handle
        if (IsItemOpen)
        {
            LockItemHandle(throwOnLockError,
              dwFileOffsetLow, dwFileOffsetHigh, nNumberOfBytesToLockLow, nNumberOfBytesToLockHigh);
        }
    }

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, the method
    /// has been called directly or indirectly by a user's code. Managed and unmanaged resources can be
    /// disposed. If disposing equals false, the method has been called by the runtime from inside the
    /// finalizer and you should not reference other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <param name="disposing"> If true, is called by IDisposable.Dispose. Otherwise it is called by
    ///  finalizer. </param>
    protected virtual void Dispose(bool disposing)
    {
        // Releasing of managed resources not needed, no managed resources here;
        // release just unmanaged resources
        Close();
    }
    #endregion // Methods

    #region IDisposable Members

    /// <summary>
    /// Implementing IDisposable. Do not make this method virtual. 
    /// The derived class should not overwrite this method, 
    /// but the virtual method Dispose(bool disposing)
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion // IDisposable Members
}

/// <summary> A class performing a folder lock. </summary>
/// <remarks>
/// Note that a lock performed by this class does not quite prevent other process to modify deployed folder 
/// contents,  if such process know or guesses the exact file path, without need to browse the root folder.
/// </remarks>
[CLSCompliant(false)]
public class LockFolder : LockFileSystemItem
{
    #region Constructor(s)

    /// <summary> Default argument-less constructor. </summary>
    public LockFolder() : base()
    { }
    #endregion // Constructor(s)

    #region Methods

    /// <summary>
    /// Locks the folder. Throws one of listed exceptions if <paramref name="strFolderPath"/> is invalid, 
    /// or if folder handle could not be open and <paramref name="throwOnOpenError"/> is true,
    /// or the lock could not be acquired and <paramref name="throwOnLockError"/> is true.
    /// </summary>
    /// <remarks>
    /// Note that the argument <paramref name="throwOnLockError "/> is by default false. It's because for
    /// the purpose of preventing to browse the folder contents, it seems sufficient just to open folder
    /// handle. ( Afterwards, the LockFile for such folder handle usually returns false, with
    /// Marshal.GetLastWin32Error() returning 87 (0x57, i.e. ERROR_INVALID_PARAMETER).
    /// </remarks>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="strFolderPath "/> argument is
    ///  null. </exception>
    /// <exception cref="ArgumentException"> Thrown when the directory <paramref name="strFolderPath "/>
    ///  does not exist. </exception>
    /// <exception cref="Win32Exception"> Thrown in case of folder handle opening error, 
    ///  if <paramref name="throwOnOpenError"/> is true,
    ///  or in case of folder handle locking error if <paramref name="throwOnLockError"/> is true.
    ///  </exception>
    /// <param name="strFolderPath"> Full path of the folder. </param>
    /// <param name="throwOnOpenError"> true to throw Win32Exception upon folder handle opening error. </param>
    /// <param name="throwOnLockError"> true to throw Win32Exception upon folder handle locking error. </param>
    public void TryLockFolder(string strFolderPath, bool throwOnOpenError, bool throwOnLockError = false)
    {
        ArgumentNullException.ThrowIfNull(strFolderPath);

        if (!Directory.Exists(strFolderPath))
        {
            string errorMessage = $"The folder '{strFolderPath}' does not exist";
            throw new ArgumentException(errorMessage, nameof(strFolderPath));
        }

        Close();
        OpenAndLockItem(new DirectoryInfo(strFolderPath), FileShare.None, throwOnOpenError, throwOnLockError);
    }
    #endregion // Methods
}

/// <summary> A class performing a file lock via Kernel32.LockFile. </summary>
/// <seealso href="http://stackoverflow.com/questions/1784195/using-lockfileex-in-c-sharp">
/// StackOverflow - Using LockFileEx in C#</seealso>
[CLSCompliant(false)]
public class LockFile : LockFileSystemItem
{
    #region Constructor(s)

    /// <summary> Default argument-less constructor. </summary>
    public LockFile() : base()
    { }
    #endregion // Constructor(s)

    #region Methods

    /// <summary> Locks the file. Throws one of listed exceptions if the lock could not be acquired. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="strFilePath"/> argument is null. </exception>
    /// <exception cref="ArgumentException">Thrown when the file <paramref name="strFilePath"/> does not exist. </exception>
    /// <param name="strFilePath"> Full path of the file. </param>
    /// <param name="offset"> The offset in the file. </param>
    /// <param name="count"> Number of bytes to lock. </param>
    /// <param name="throwOnOpenError"> true to throw Win32Exception upon directory handle opening error. </param>
    /// <param name="throwOnLockError"> true to throw Win32Exception upon directory handle locking error. </param>
    public void TryLockFile(
        string strFilePath,
        ulong offset,
        ulong count,
        bool throwOnOpenError,
        bool throwOnLockError)
    {
        ArgumentNullException.ThrowIfNull(strFilePath);

        if (!File.Exists(strFilePath))
        {
            string errorMessage = $"The file '{strFilePath}' does not exist";
            throw new ArgumentException(errorMessage, nameof(strFilePath)); ;
        }

        uint offsetLow = (uint)offset;
        uint offsetHigh = (uint)(offset >> 32);
        uint countLow = (uint)count;
        uint countHigh = (uint)(count >> 32);

        Close();
        OpenAndLockItem(new FileInfo(strFilePath), FileShare.None, throwOnOpenError, throwOnLockError,
                  offsetLow, offsetHigh, countLow, countHigh);
    }
    #endregion // Methods
}

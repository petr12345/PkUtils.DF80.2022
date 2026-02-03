// Ignore Spelling: Utils, Api, Winapi, Ctrl, Unmap, memset, ull, bufsize, FNAME, LCID, DAC, MEMORYSTATUS, MEMORYSTATUSEX, OSVERSIONINFOEX, LOCKFILE, EXCED, INTPTR, programmatically
// 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace PK.PkUtils.WinApi;

#pragma warning disable IDE0079  // Remove unnecessary suppression
#pragma warning disable IDE0057     // Substring can be simplified
#pragma warning disable IDE0251     // Member can be made 'readonly'
#pragma warning disable CA1401      // P/Invoke method should not be visible
#pragma warning disable CA1419      // Provide a parameterless constructor that is as visible as the containing type for concrete types derived from 'System.Runtime.InteropServices.SafeHandle'
#pragma warning disable CA1806      // The HRESULT of some API is not used
#pragma warning disable SYSLIB1054  // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

/// <summary>
/// Helper class containing Kernel32 API functions
/// </summary>
public static class Kernel32
{
    #region Constants

    /// <summary> The maximum character length of a path, as normally supported by Win32. </summary>
    /// <remarks> This number 260 actually means 259 characters plus a terminating null character, 
    /// hence the maximal path normally supported by .NET runtime has 259 characters.</remarks>
    public const int MAX_PATH = 260;

    /// <summary>
    /// Maximum long path.
    /// While Windows allows larger paths up to a maximum of 32767 characters, because this is only
    /// an approximation and can vary across systems and OS versions, we choose a limit well under so that we
    /// can give a consistent behavior.
    /// </summary>
    public const int MAX_LONG_PATH = 32000;

    /// <summary> The maximal length of drive component of the full path. </summary>
    public const int MAX_DRIVE = 3;

    /// <summary> The maximal length of path component of the full path. </summary>
    public const int MAX_DIR = 256;

    /// <summary> The maximal length of file name component of the full path. </summary>
    public const int MAX_FNAME = 256;

    /// <summary> The maximal length of file extension component of the full path. </summary>
    public const int MAX_EXT = 256;

    /// <summary> The maximum length of an alternative name for the file, including the termination null,
    /// as used in <see cref="WIN32_FIND_DATA"/> structure.</summary>
    public const int MAX_ALTERNATE = 14;

    /// <summary>
    /// The flag to be used as a second argument of <see cref="LockFileEx"/>. If applied, the function
    /// returns immediately if it is unable to acquire the requested lock. Otherwise, it waits.
    /// </summary>
    public const uint LOCKFILE_FAIL_IMMEDIATELY = 0x00000001;

    /// <summary>
    /// The flag to be used as a second argument of <see cref="LockFileEx"/>. If applied, the function
    /// requests an exclusive lock. Otherwise, it requests a shared lock.
    /// </summary>
    public const uint LOCKFILE_EXCLUSIVE_LOCK = 0x00000002;

    /// <summary>
    /// The invalid file attributes, a possible result of <see cref="GetFileAttributes"/>.
    /// </summary>
    public const int INVALID_FILE_ATTRIBUTES = -1;

    /// <summary> Constant for invalid handle value, returned by FindFirstFile and so on. </summary>
    public const uint INVALID_HANDLE_VALUE = unchecked((uint)-1);

    /// <summary> Constant for invalid handle value, casted to IntPtr. </summary>
    public static readonly IntPtr INVALID_HANDLE_VALUE_INTPTR = new(-1);
    #region Marshal_GetLastWin32Error_codes

    /// <summary> The system cannot find the file specified. </summary>
    public const int ERROR_FILE_NOT_FOUND = 0x2;

    /// <summary> The system cannot find the path specified. </summary>
    public const int ERROR_PATH_NOT_FOUND = 0x3;

    /// <summary> Access is denied. </summary>
    public const int ERROR_ACCESS_DENIED = 0x5;

    /// <summary> The handle is invalid.</summary>
    public const int ERROR_INVALID_HANDLE = 0x6;

    /// <summary> The system cannot find the drive specified. </summary>
    public const int ERROR_INVALID_DRIVE = 0xf;

    /// <summary> There are no more files. </summary>
    public const int ERROR_NO_MORE_FILES = 0x12;

    /// <summary> The filename, directory name, or volume label syntax is incorrect. </summary>
    public const int ERROR_INVALID_NAME = 0x7B; // 123;

    /// <summary> Cannot create a file when that file already exists. </summary>
    public const int ERROR_ALREADY_EXISTS = 0xB7;

    /// <summary> The filename or extension is too long. </summary>
    public const int ERROR_FILENAME_EXCED_RANGE = 0xCE;

    /// <summary> The directory name is invalid. </summary>
    public const int ERROR_DIRECTORY = 0x10B;

    /// <summary> Indicates that the operation was canceled. </summary>
    public const int ERROR_OPERATION_ABORTED = 0x3e3;

    #endregion // Marshal_GetLastWin32Error_codes

    #region Constants_for_MapViewOfFile_desired_access

    /// <summary>
    /// A constant used for specifying desired access argument for <see cref="Kernel32.MapViewOfFile"/>. <br/>
    /// 
    /// A copy-on-write view of the file is mapped. The file mapping object must have been created with
    /// PAGE_READONLY, PAGE_READ_EXECUTE, PAGE_WRITECOPY, PAGE_EXECUTE_WRITECOPY, PAGE_READWRITE, or
    /// PAGE_EXECUTE_READWRITE protection.<br/>
    /// 
    /// When a process writes to a copy-on-write page, the system copies the original page to a new page
    /// that is private to the process. The new page is backed by the paging file. The protection of the
    /// new page changes from copy-on-write to read/write.<br/>
    /// 
    /// When copy-on-write access is specified, the system and process commit charge taken is for the
    /// entire view because the calling process can potentially write to every page in the view, making all
    /// pages private. The contents of the new page are never written back to the original file and are
    /// lost when the view is unmapped.<br/>
    /// </summary>
    public const uint FILE_MAP_COPY = 0x0001;

    /// <summary>
    /// A constant used for specifying desired access argument for <see cref="Kernel32.MapViewOfFile"/>. <br/>
    /// 
    /// A read/write view of the file is mapped. The file mapping object must have been created with
    /// PAGE_READWRITE or PAGE_EXECUTE_READWRITE protection.<br/>
    /// 
    /// When used with MapViewOfFile, (FILE_MAP_WRITE | FILE_MAP_READ) and FILE_MAP_ALL_ACCESS are
    /// equivalent to FILE_MAP_WRITE.<br/>
    /// </summary>
    public const uint FILE_MAP_WRITE = 0x0002;

    /// <summary>
    /// A read-only view of the file is mapped. An attempt to write to the file view results in an access
    /// violation.<br/>
    /// 
    /// The file mapping object must have been created with PAGE_READONLY, PAGE_READWRITE,
    /// PAGE_EXECUTE_READ, or PAGE_EXECUTE_READWRITE protection.<br/>
    /// </summary>
    public const uint FILE_MAP_READ = 0x0004;

    /// <summary>
    /// A constant used for specifying desired access argument for <see cref="Kernel32.MapViewOfFile"/>. <br/>
    /// 
    /// An executable view of the file is mapped (mapped memory can be run as code). The file mapping
    /// object must have been created with PAGE_EXECUTE_READ, PAGE_EXECUTE_WRITECOPY, or
    /// PAGE_EXECUTE_READWRITE protection.<br/>
    /// </summary>
    public const uint FILE_MAP_EXECUTE = 0x0020;

    /// <summary>
    /// A constant used for specifying desired access argument for <see cref="Kernel32.MapViewOfFile"/>. <br/>
    /// 
    /// A read/write view of the file is mapped. The file mapping object must have been created with
    /// PAGE_READWRITE or PAGE_EXECUTE_READWRITE protection.<br/>
    /// 
    /// When used with the MapViewOfFile function, FILE_MAP_ALL_ACCESS is equivalent to FILE_MAP_WRITE.<br/>
    /// </summary>
    public const uint FILE_MAP_ALL_ACCESS = 0x000f001f;
    #endregion // Constants_for_MapViewOfFile_desired_access

    #region Constants_for_EnumLocales

    /// <summary> This constant is used as in input argument for <see cref="Kernel32.EnumSystemLocales"/>
    ///  and <see cref="Kernel32.EnumLocales"/>. <br/>
    ///  Value of LCID_INSTALLED means 'Enumerate only installed locale identifiers'.
    ///  </summary>
    /// <seealso cref="LCID_SUPPORTED"/>
    public const uint LCID_INSTALLED = 1;

    /// <summary> This constant is used as in input argument for <see cref="Kernel32.EnumSystemLocales"/>
    ///  and <see cref="Kernel32.EnumLocales"/>. <br/>
    ///  Value of LCID_SUPPORTED means 'Enumerate all supported locale identifiers.'.
    ///  </summary>
    /// <seealso cref="LCID_INSTALLED"/>
    public const uint LCID_SUPPORTED = 2;
    #endregion // Constants_for_EnumLocales

    #region Waiting_times

    /// <summary> Specifies an infinite wait period used as input argument for <see cref="WaitForSingleObject"/> 
    /// or <see cref="WaitForMultipleObjects"/>.</summary>
    public const Int32 INFINITE = -1;
    #endregion // Waiting_times

    #region Waiting_results

    /// <summary>
    /// Indentifies a possible the result of <see cref="WaitForSingleObject"/> or
    /// <see cref="WaitForMultipleObjects"/> for case the state of the specified object 
    ///  or objects became signaled.
    /// </summary>
    public const Int32 WAIT_OBJECT_0 = 0x00000000;

    /// <summary>
    /// Identifies a possible the result of <see cref="WaitForSingleObject"/> or
    /// <see cref="WaitForMultipleObjects"/> for case the time-out interval elapsed,
    /// and the object's state is non-signaled.
    /// </summary>
    public const Int32 WAIT_TIMEOUT = 0x102;

    /// <summary> Identifies a possible the result of <see cref="WaitForSingleObject"/> or 
    /// <see cref="WaitForMultipleObjects"/> for case the specified object is a mutex object 
    /// that was not released by the thread that owned the mutex object before the owning thread terminated. 
    /// Ownership of the mutex object is granted to the calling thread and the mutex state is set to non-signaled.
    /// </summary>
    public const Int32 WAIT_ABANDONED = 0x00000080;

    /// <summary> Identifies a possible the result of <see cref="WaitForSingleObject"/> or 
    /// <see cref="WaitForMultipleObjects"/> for case waiting has failed. </summary>
    public const Int32 WAIT_FAILED = -1;

    #endregion //Waiting_results

    #region Access_rights
    // The Windows security model enables you to control access to event, mutex, semaphore, and waitable 
    // timer objects. (Timer queues, interlocked variables, and critical section objects are not securable.)
    // 
    // The following are masks for the predefined standard access types
    // taken from winnt.h

    /// <summary> Access right required to delete the object.. </summary>
    public const UInt32 DELETE = (0x00010000);

    /// <summary> Required to read information in the security descriptor for the object, not including 
    /// the information in the SACL. 
    /// To read or write the SACL, you must request the ACCESS_SYSTEM_SECURITY access right.
    /// </summary>
    public const UInt32 READ_CONTROL = (0x00020000);

    /// <summary> Required to modify the DACL in the security descriptor for the object. </summary>
    public const UInt32 WRITE_DAC = (0x00040000);

    /// <summary> Required to change the owner in the security descriptor for the object. </summary>
    public const UInt32 WRITE_OWNER = (0x00080000);

    /// <summary>
    /// The right to use the object for synchronization. This enables a thread to wait until the object is in
    /// the signaled state.
    /// </summary>
    public const UInt32 SYNCHRONIZE = (0x00100000);

    /// <summary>
    /// The STANDARD_RIGHTS_REQUIRED mask is meant to be used when defining access masks for object types.
    /// I'm guessing it's called STANDARD_RIGHTS_REQUIRED because it's the set of access masks that all
    /// securable objects must support.<br/>
    /// Notice that STANDARD_RIGHTS_REQUIRED is just an abbreviation for the union of the four access bits
    /// DELETE | READ_CONTROL | WRITE_DAC | WRITE_OWNER.
    /// </summary>
    /// <remarks> For more info see for instance
    /// <see href="http://blogs.msdn.com/b/oldnewthing/archive/2008/02/27/7912126.aspx">
    /// If you ask for STANDARD_RIGHTS_REQUIRED, you may as well ask for the moon</see>
    /// </remarks>
    public const UInt32 STANDARD_RIGHTS_REQUIRED = (0x000F0000);

    /// <summary>
    /// Modify state access, which is required for the SetEvent, ResetEvent and PulseEvent functions.
    /// </summary>
    public const UInt32 EVENT_MODIFY_STATE = 0x0002;

    /// <summary> All possible access rights for an event object. </summary>
    public const UInt32 EVENT_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x3);

    #endregion // Access_rights

    /// <summary> 
    /// The constant for argument of <see cref="AttachConsole"/> method.
    /// For more info see for instance 
    /// <see href="http://www.csharp411.com/console-output-from-winforms-application/">
    /// Console Output from a WinForms Application</see>
    /// </summary>
    public const uint ATTACH_PARENT_PROCESS = unchecked((uint)-1);

    #endregion // Constants

    #region Nested types

    /// <summary>
    /// Defines flags for specifying file access argument that is passed to methods
    /// <see cref="CreateFile"/>, DuplicateHandle etc.
    /// </summary>
    /// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa446632(v=vs.85).aspx">
    /// Generic Access Rights</seealso>
    [Flags]
    public enum EFileAccess : uint
    {
        /// <summary> A binary constant that is used to represent a value
        /// of a dwDesiredAccess desired access argument that is passed to methods <see cref="CreateFile"/>,
        /// DuplicateHandle etc. This particular value represents read access.
        /// </summary>
        GenericRead = 0x80000000,

        /// <summary> A binary constant that is used to represent a value
        /// of a dwDesiredAccess desired access argument that is passed to methods <see cref="CreateFile"/>,
        /// DuplicateHandle etc. This particular value represents write access.
        /// </summary>
        GenericWrite = 0x40000000,

        /// <summary> A binary constant representing the generic Execute access right. </summary>
        GenericExecute = 0x20000000,

        /// <summary> A binary constant representing all possible access rights. </summary>
        GenericAll = 0x10000000,
    }

    /// <summary>
    /// Defines flags for specifying the last but one ( dwFlagsAndAttributes ) argument of
    /// <see cref="CreateFile"/> function.
    /// </summary>
    /// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa363858(v=vs.85).aspx">
    /// CreateFile function</seealso>
    [Flags]
    public enum CreateFileFlags : uint
    {
        /// <summary>
        /// The file is being opened or created for a backup or restore operation. The system ensures that the
        /// calling process overrides file security checks when the process has SE_BACKUP_NAME and
        /// SE_RESTORE_NAME privileges. For more information, see Changing Privileges in a Token.
        /// </summary>
        FILE_FLAG_BACKUP_SEMANTICS = 0x02000000,

        /// <summary>
        /// The file is to be deleted immediately after all of its handles are closed, which includes the
        /// specified handle and any other open or duplicated handles.
        /// </summary>
        FILE_FLAG_DELETE_ON_CLOSE = 0x04000000,

        /// <summary>
        /// If you attempt to create multiple instances of a pipe with this flag, creation of the first instance
        /// succeeds, but creation of the next instance fails with ERROR_ACCESS_DENIED.
        /// </summary>
        FILE_FLAG_FIRST_PIPE_INSTANCE = 0x00080000,

        /// <summary>
        /// The file or device is being opened with no system caching for data reads and writes. This flag does
        /// not affect hard disk caching or memory mapped files.
        /// </summary>
        FILE_FLAG_NO_BUFFERING = 0x20000000,

        /// <summary>
        /// The file data is requested, but it should continue to be located in remote storage. It should not be
        /// transported back to local storage. This flag is for use by remote storage systems.
        /// </summary>
        FILE_FLAG_OPEN_NO_RECALL = 0x00100000,

        /// <summary>
        /// Normal reparse point processing will not occur; CreateFile will attempt to open the reparse point.
        /// When a file is opened, a file handle is returned, whether or not the filter that controls the reparse
        /// point is operational.<br/>
        /// This flag cannot be used with the CREATE_ALWAYS flag.<br/>
        /// If the file is not a reparse point, then this flag is ignored.<br/>
        /// </summary>
        FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000,

        /// <summary> The file or device is being opened or created for asynchronous I/O. </summary>
        FILE_FLAG_OVERLAPPED = 0x40000000,

        /// <summary>
        /// Access will occur according to POSIX rules. This includes allowing multiple files with names,
        /// differing only in case, for file systems that support that naming. Use care when using this option,
        /// because files created with this flag may not be accessible by applications that are written for MS-
        /// DOS or 16-bit Windows.
        /// </summary>
        FILE_FLAG_POSIX_SEMANTICS = 0x01000000,

        /// <summary>
        /// Access is intended to be random. The system can use this as a hint to optimize file caching.
        /// </summary>
        FILE_FLAG_RANDOM_ACCESS = 0x10000000,

        /// <summary>
        /// Access is intended to be sequential from beginning to end. The system can use this as a hint to
        /// optimize file caching.
        /// </summary>
        FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000,

        /// <summary> 
        /// The file or device is being opened with session awareness. If this flag is not specified, 
        /// then per-session devices (such as a redirected USB device) cannot be opened by processes 
        /// running in session 0. This flag has no effect for callers not in session 0. 
        /// This flag is supported only on server editions of Windows. 
        /// </summary>
        FILE_FLAG_SESSION_AWARE = 0x00800000,

        /// <summary> Write operations will not go through any intermediate cache, they will go directly to disk.
        /// </summary>
        FILE_FLAG_WRITE_THROUGH = 0x80000000,
    };

    /// <summary>
    /// This structure contains information used in asynchronous (or overlapped) input and output (I/O).
    /// Is used for instance as an argument of <see cref="LockFileEx"/>. </summary>
    ///  <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms684342(v=vs.85).aspx">
    /// MSDN help for OVERLAPPED</seealso>
    [StructLayout(LayoutKind.Sequential)]
    public struct OVERLAPPED
    {
        /// <summary> The status code for the I/O request. </summary>
        public uint internalLow;

        /// <summary>
        /// The number of bytes transferred for the I/O request. The system sets this member if the request is
        /// completed without errors.
        /// </summary>
        public uint internalHigh;

        /// <summary> The low-order portion of the file position at which to start the I/O request, 
        /// as specified by the user. . </summary>
        public uint offsetLow;

        /// <summary>
        /// The high-order portion of the file position at which to start the I/O request, as specified
        /// by the user. </summary>
        public uint offsetHigh;

        /// <summary>
        /// A handle to the event that will be set to a signaled state by the system when the operation has
        /// completed. </summary>
        public IntPtr hEvent;
    }
    /// <summary>
    /// Defines bit-fields for specifying format messages flags argument of <see cref="FormatMessage"/> API.
    /// </summary>
    [Flags]
    public enum FormatMsg
    {
        /// <summary>
        /// The function allocates a buffer large enough to hold the formatted message, and places a pointer to
        /// the allocated buffer at the address specified by lpBuffer. The lpBuffer parameter is a pointer to
        /// an LPTSTR; you must cast the pointer to an LPTSTR (for example, (LPTSTR)&amp;lpBuffer). The nSize
        /// parameter specifies the minimum number of TCHARs to allocate for an output message buffer. The
        /// caller should use the LocalFree function to free the buffer when it is no longer needed. If the
        /// length of the formatted message exceeds 128K bytes, then FormatMessage will fail and a subsequent
        /// call to GetLastError will return ERROR_MORE_DATA. This value is not available for use when
        /// compiling Windows Store apps.
        /// </summary>
        FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100,

        /// <summary> The lpSource parameter is a module handle containing the message-table resource(s) to search. 
        /// If this lpSource handle is NULL, the current process's application image file will be searched. 
        /// This flag cannot be used with FORMAT_MESSAGE_FROM_STRING.
        /// If the module has no message table resource, the function fails with ERROR_RESOURCE_TYPE_NOT_FOUND. 
        /// </summary>
        FORMAT_MESSAGE_FROM_HMODULE = 0x00000800,

        /// <summary> The lpSource parameter is a pointer to a null-terminated string that contains 
        /// a message definition. The message definition may contain insert sequences, just as the message text 
        /// in a message table resource may. 
        /// This flag cannot be used with FORMAT_MESSAGE_FROM_HMODULE or FORMAT_MESSAGE_FROM_SYSTEM. </summary>
        FORMAT_MESSAGE_FROM_STRING = 0x00000400,

        /// <summary> The function should search the system message-table resource(s) for the requested message. 
        /// If this flag is specified with FORMAT_MESSAGE_FROM_HMODULE, the function searches the system message 
        /// table if the message is not found in the module specified by lpSource. 
        /// This flag cannot be used with FORMAT_MESSAGE_FROM_STRING.
        /// If this flag is specified, an application can pass the result of the GetLastError function 
        /// to retrieve the message text for a system-defined error. </summary>
        FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000,

        /// <summary> Insert sequences in the message definition are to be ignored and passed through 
        /// to the output buffer unchanged. This flag is useful for fetching a message for later formatting. 
        /// If this flag is set, the Arguments parameter is ignored. </summary>
        FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200,

        /// <summary>
        /// The Arguments parameter is not a va_list structure, but is a pointer to an array of values that
        /// represent the arguments. This flag cannot be used with 64-bit integer values. If you are using a 64-
        /// bit integer, you must use the va_list structure.
        /// </summary>
        FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000,
    }

    /// <summary> CreationDisposition values represent individual values that could be used 
    /// as a value of a dwCreationDisposition argument of CreateFile( . ..., dwCreationDisposition, ...)
    /// </summary>
    public enum CreationDisposition : int
    {
        /// <summary>
        /// This particular value represents an option "Creates a new file, only if it does not already exist.".
        /// </summary>
        CREATE_NEW = 1,

        /// <summary>
        /// This particular value represents an option "Creates a new file, always."<br/>
        /// If the specified file exists and is writable, the function overwrites the file, the function
        /// succeeds, and last-error code is set to ERROR_ALREADY_EXISTS (183).<br/>
        /// If the specified file does not exist and is a valid path, a new file is created, the function
        /// succeeds, and the last-error code is set to zero."<br/>
        /// </summary>
        CREATE_ALWAYS = 2,

        /// <summary>
        /// This particular value represents an option "Opens a file or device, only if it exists.".
        /// </summary>
        OPEN_EXISTING = 3,

        /// <summary>
        /// This particular value represents an option "Opens a file, always."<br/>
        /// If the specified file exists, the function succeeds and the last-error code is set to
        /// ERROR_ALREADY_EXISTS (183).<br/> 
        /// If the specified file does not exist and is a valid path to a writable location,
        /// the function creates a file and the last-error code is set to zero.<br/>
        /// </summary>
        OPEN_ALWAYS = 4,

        /// <summary>
        /// This particular value represents an option 
        /// "Opens a file and truncates it so that its size is zero bytes, only if it exists."
        /// </summary>
        TRUNCATE_EXISTING = 5,
    };

    /// <summary> This enum lists Bit-fields of flags for specifying PageAccess memory-protection options,
    /// when allocating or protecting a page in memory.
    /// This is used for instance with <see cref="CreateFileMapping"/> as its 3rd argument,
    /// or <see cref="OpenFileMapping"/> as its 1st argument.
    /// </summary>
    /// <remarks>
    /// Protection attributes cannot be assigned to a portion of a page; they can only be assigned to a whole page.
    /// </remarks>
    [Flags]
    public enum PageAccess : uint
    {
        /// <summary>
        /// Disables all access to the committed region of pages. An attempt to read from, write to, or execute
        /// the committed region results in an access violation.
        /// </summary>
        PAGE_NOACCESS = 0x01,

        /// <summary>
        /// Enables read-only access to the committed region of pages. An attempt to write to the committed
        /// region results in an access violation. If Data Execution Prevention is enabled, an attempt to execute
        /// code in the committed region results in an access violation.
        /// </summary>
        PAGE_READONLY = 0x02,

        /// <summary>
        /// Enables read-only or read/write access to the committed region of pages. If Data Execution Prevention
        /// is enabled, attempting to execute code in the committed region results in an access violation.
        /// </summary>
        PAGE_READWRITE = 0x04,

        /// <summary>
        /// Enables read-only or copy-on-write access to a mapped view of a file mapping object. An attempt to
        /// write to a committed copy-on-write page results in a private copy of the page being made for the
        /// process. The private page is marked as PAGE_READWRITE, and the change is written to the new page. If
        /// Data Execution Prevention is enabled, attempting to execute code in the committed region results in
        /// an access violation.
        /// </summary>
        PAGE_WRITECOPY = 0x08,

        /// <summary>
        /// Enables execute access to the committed region of pages. An attempt to write to the committed region
        /// results in an access violation.
        /// This flag is not supported by the CreateFileMapping function.
        /// </summary>
        PAGE_EXECUTE = 0x10,

        /// <summary>
        /// Enables execute or read-only access to the committed region of pages. An attempt to write to the
        /// committed region results in an access violation.
        /// </summary>
        PAGE_EXECUTE_READ = 0x20,

        /// <summary> Enables execute, read-only, or read/write access to the committed region of pages.
        /// </summary>
        PAGE_EXECUTE_READWRITE = 0x40,

        /// <summary>
        /// Enables execute, read-only, or copy-on-write access to a mapped view of a file mapping object. An
        /// attempt to write to a committed copy-on-write page results in a private copy of the page being made
        /// for the process. The private page is marked as PAGE_EXECUTE_READWRITE, and the change is written to
        /// the new page.
        /// </summary>
        PAGE_EXECUTE_WRITECOPY = 0x80,
    };

    /// <summary>
    /// Structure used to retrieve system info
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEM_INFO
    {
        /// <summary> The processor architecture of the installed operating system. </summary>
        public ushort wProcessorArchitecture;

        /// <summary> This member is reserved for future use.</summary>
        public ushort wReserved;

        /// <summary> The page size and the granularity of page protection and commitment. 
        /// This is the page size used by the VirtualAlloc function.
        /// </summary>
        public uint dwPageSize;

        /// <summary>
        /// A pointer to the lowest memory address accessible to applications and dynamic-link libraries (DLLs).
        /// </summary>
        public IntPtr lpMinimumApplicationAddress;

        /// <summary> A pointer to the highest memory address accessible to applications and DLLs.
        /// </summary>
        public IntPtr lpMaximumApplicationAddress;

        /// <summary>
        /// A mask representing the set of processors configured into the system. Bit 0 is processor 0; bit 31 is
        /// processor 31.
        /// </summary>
        public uint dwActiveProcessorMask;

        /// <summary> The number of logical processors in the current group. </summary>
        public uint dwNumberOfProcessors;

        /// <summary>
        /// An obsolete member that is retained for compatibility. Use the wProcessorArchitecture,
        /// wProcessorLevel, and wProcessorRevision members to determine the type of processor.
        /// </summary>
        public uint dwProcessorType;

        /// <summary> The granularity for the starting address at which virtual memory can be allocated. </summary>
        public uint dwAllocationGranularity;

        /// <summary>
        /// The architecture-dependent processor level. It should be used only for display purposes. To determine
        /// the feature set of a processor, use the IsProcessorFeaturePresent function.
        /// </summary>
        public ushort dwProcessorLevel;

        /// <summary> The architecture-dependent processor revision. </summary>
        public ushort dwProcessorRevision;
    }

    /// <summary> Structure used to retrieve memory status. </summary>
    /// <remarks>
    /// One does not have to initialize dwLength field; the function GlobalMemoryStatus does that itself.
    /// Note that on computers with more than 4 GB of memory, the MEMORYSTATUS structure can return incorrect
    /// information, reporting a value of -1 to indicate an overflow.
    /// </remarks>
    /// <seealso href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa366772(v=vs.85).aspx">
    /// MEMORYSTATUS structure on MSDN
    /// </seealso>
    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORYSTATUS
    {
        /// <summary>
        /// The size of the MEMORYSTATUS data structure, in bytes. You do not need to set this member before
        /// calling the GlobalMemoryStatus function; the function sets it.
        /// </summary>
        public uint dwLength;

        /// <summary>
        /// A number between 0 and 100 that specifies the approximate percentage of physical memory that is in
        /// use (0 indicates no memory use and 100 indicates full memory use).
        /// </summary>
        public uint dwMemoryLoad;

        /// <summary> The amount of actual physical memory, in bytes.
        /// </summary>
        public ulong ullTotalPhys;

        /// <summary>
        /// The amount of physical memory currently available, in bytes. This is the amount of physical memory
        /// that can be immediately reused without having to write its contents to disk first. It is the sum of
        /// the size of the standby, free, and zero lists.
        /// </summary>
        public ulong ullAvailPhys;

        /// <summary>
        /// The current size of the committed memory limit, in bytes. This is physical memory plus the size of
        /// the page file, minus a small overhead.
        /// </summary>
        public ulong ullTotalPageFile;

        /// <summary>
        /// The maximum amount of memory the current process can commit, in bytes. This value should be smaller
        /// than the system-wide available commit. To calculate this value, call GetPerformanceInfo and subtract
        /// the value of CommitTotal from CommitLimit.
        /// </summary>
        public ulong ullAvailPageFile;

        /// <summary>
        /// The size of the user-mode portion of the virtual address space of the calling process, in bytes. This
        /// value depends on the type of process, the type of processor, and the configuration of the operating
        /// system. For example, this value is approximately 2 GB for most 32-bit processes on an x86 processor
        /// and approximately 3 GB for 32-bit processes that are large address aware running on a system
        /// with 4 GT RAM Tuning enabled.
        /// </summary>
        public ulong ullTotalVirtual;

        /// <summary>
        /// The amount of unreserved and uncommitted memory currently in the user-mode portion of the virtual
        /// address space of the calling process, in bytes.
        /// </summary>
        public ulong ullAvailVirtual;
    }

    /// <summary> Structure used to retrieve memory status 'ex'. </summary>
    /// <remarks>
    /// Unlike with <see cref="MEMORYSTATUS"/> one HAS to initialize dwLength field;
    /// the function GlobalMemoryStatusEx requires the caller to do it.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORYSTATUSEX
    {
        /// <summary>
        /// The size of the structure, in bytes. You must set this member before calling GlobalMemoryStatusEx.
        /// </summary>
        public uint dwLength;
        /// <summary>
        /// A number between 0 and 100 that specifies the approximate percentage of physical memory that is in
        /// use (0 indicates no memory use and 100 indicates full memory use).
        /// </summary>
        public uint dwMemoryLoad;

        /// <summary> The amount of actual physical memory, in bytes. </summary>
        public ulong ullTotalPhys;

        /// <summary>
        /// The amount of physical memory currently available, in bytes. This is the amount of physical memory
        /// that can be immediately reused without having to write its contents to disk first. It is the sum of
        /// the size of the standby, free, and zero lists.
        /// </summary>
        public ulong ullAvailPhys;

        /// <summary>
        /// The current committed memory limit for the system or the current process, whichever is smaller, in
        /// bytes. To get the system-wide committed memory limit, call GetPerformanceInfo.
        /// </summary>
        public ulong ullTotalPageFile;

        /// <summary>
        /// The maximum amount of memory the current process can commit, in bytes. This value is equal to or
        /// smaller than the system-wide available commit value. To calculate the system-wide available commit
        /// value, call GetPerformanceInfo and subtract the value of CommitTotal from the value of CommitLimit.
        /// </summary>
        public ulong ullAvailPageFile;

        /// <summary>
        /// The size of the user-mode portion of the virtual address space of the calling process, in bytes. This
        /// value depends on the type of process, the type of processor, and the configuration of the operating
        /// system. For example, this value is approximately 2 GB for most 32-bit processes on an x86 processor
        /// and approximately 3 GB for 32-bit processes that are large address aware running on a system with 4-
        /// gigabyte tuning enabled.
        /// </summary>
        public ulong ullTotalVirtual;

        /// <summary>
        /// The amount of unreserved and uncommitted memory currently in the user-mode portion of the virtual
        /// address space of the calling process, in bytes.
        /// </summary>
        public ulong ullAvailVirtual;

        /// <summary>
        /// Reserved. This value is always 0.
        /// </summary>
        public ulong ullAvailExtendedVirtual;

        /// <summary> Initializes this object. </summary>
        public void Init()
        {
            dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(this);
        }

        /// <summary> Initializes this object and gets global memory status. </summary>
        /// <returns> true if it succeeds, false if it fails. 
        ///            To get extended error information, call Marshal.GetLastWin32Error().</returns>
        public bool GetStatus()
        {
            Init();
            return Kernel32.GlobalMemoryStatusEx(ref this);
        }

        /// <summary> The amount of actual physical memory, in bytes. </summary>
        /// <returns> The total physical memory. </returns>
        public ulong GetTotalPhys()
        {
            return ullTotalPhys;
        }

        /// <summary> The amount of actual physical memory, in KBytes. </summary>
        /// <returns> The total physical memory in KBytes. </returns>
        public uint GetTotalPhysKB()
        {
            uint result = (uint)(GetTotalPhys() / 1024);
            return result;
        }

        /// <summary>
        /// Return occupied space (total - available) in bytes.
        /// </summary>
        /// <returns>Occupied space in bytes.</returns>
        public ulong GetOccupied()
        {
            ulong result = ullTotalPhys - this.ullAvailPhys;
            return result;
        }

        /// <summary>
        /// Return occupied space (total - available) in KBytes.
        /// </summary>
        /// <returns>Occupied space in KBytes.</returns>
        public uint GetOccupiedKB()
        {
            ulong occupied = GetOccupied();
            uint result = (uint)(occupied / 1024);
            return result;
        }

        /// <summary>
        /// Return available (free) space in bytes.
        /// </summary>
        /// <returns>Available (free) space in bytes.</returns>
        public ulong GetAvailable()
        {
            return ullAvailPhys;
        }

        /// <summary>
        /// Return available (free) space in KBytes.
        /// </summary>
        /// <returns>Available (free) space in KBytes.</returns>
        public ulong GetAvailableKB()
        {
            uint result = (uint)(GetAvailable() / 1024);
            return result;
        }
    }

    /// <summary>
    /// Structure used to retrieve OS Version Info.
    /// Note: one HAS to initialize dwOSVersionInfoSize field.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct OSVERSIONINFOEX
    {
        #region Fields

        /// <summary> The size of this data structure, in bytes. </summary>
        public int dwOSVersionInfoSize;

        /// <summary> The major version number of the operating system. </summary>
        public int dwMajorVersion;

        /// <summary> The minor version number of the operating system. </summary>
        public int dwMinorVersion;

        /// <summary> The build number of the operating system.</summary>
        public int dwBuildNumber;

        /// <summary> The operating system platform. This member can be VER_PLATFORM_WIN32_NT. </summary>
        public int dwPlatformId;

        /// <summary>
        /// A null-terminated string, such as "Service Pack 3", that indicates the latest Service Pack installed
        /// on the system. If no Service Pack has been installed, the string is empty.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szCSDVersion;

        /// <summary>
        /// The major version number of the latest Service Pack installed on the system. For example, for Service
        /// Pack 3, the major version number is 3. If no Service Pack has been installed, the value is zero.
        /// </summary>
        public short wServicePackMajor;

        /// <summary>
        /// The minor version number of the latest Service Pack installed on the system. For example, for Service
        /// Pack 3, the minor version number is 0.
        /// </summary>
        public short wServicePackMinor;

        /// <summary> A bit mask that identifies the product suites available on the system. </summary>
        public short wSuiteMask;

        /// <summary> Any additional information about the system. </summary>
        public byte wProductType;

        /// <summary> Reserved for future use. </summary>
        public byte wReserved;
        #endregion // Fields

        #region Methods

        /// <summary>
        /// Initializes dwOSVersionInfoSize field
        /// </summary>
        public void Init()
        {
            dwOSVersionInfoSize = Marshal.SizeOf<OSVERSIONINFOEX>();
        }

        /// <summary>
        /// Initialize and call Kernel32.GetVersionEx for itself
        /// </summary>
        /// <returns> true if it succeeds, false if it fails. 
        ///            To get extended error information, call Marshal.GetLastWin32Error().</returns>
        public bool GetVersionEx()
        {
            Init();
            return Kernel32.GetVersionEx(ref this);
        }
        #endregion // Methods
    }

    /// <summary>
    /// Structure used by FindFirstFile, FindNextFile. For details see
    /// http://msdn.microsoft.com/msdnmag/issues/05/10/Reliability/default.aspx
    /// </summary>
    /* [Serializable] - don't make serializable. 
     * Since FILETIME is not serializable, WIN32_FIND_DATA can't be serializable 
     * unless FILETIME information is left-out, or we resort to custom serialization.
     * In both cases, there is no feasible reason to do that...
     */
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    [BestFitMapping(false)]
    public class WIN32_FIND_DATA
    {
        /// <summary> The file attributes of a file. </summary>
        public FileAttributes dwFileAttributes;

        /// <summary> A FILETIME structure that specifies when a file or directory was created. </summary>
        public ComTypes.FILETIME ftCreationTime;

        /// <summary>
        /// For a file, the structure specifies when the file was last read from, written to, or for executable
        /// files, run. For a directory, the structure specifies when the directory is created.
        /// </summary>
        public ComTypes.FILETIME ftLastAccessTime;

        /// <summary>
        /// For a file, the structure specifies when the file was last written to, truncated, or overwritten, for
        /// example, when WriteFile or SetEndOfFile are used. The date and time are not updated when file
        /// attributes or security descriptors are changed.
        /// 
        /// For a directory, the structure specifies when the directory is created. If the underlying file system
        /// does not support last write time, this member is zero.
        /// </summary>
        public ComTypes.FILETIME ftLastWriteTime;

        /// <summary> The high-order DWORD value of the file size, in bytes.
        /// This value is zero unless the file size is greater than MAXDWORD.
        /// </summary>
        public int nFileSizeHigh;

        /// <summary> The low-order DWORD value of the file size, in bytes. </summary>
        public int nFileSizeLow;

        /// <summary>
        /// If the dwFileAttributes member includes the FILE_ATTRIBUTE_REPARSE_POINT attribute, this member
        /// specifies the reparse point tag.
        /// Otherwise, this value is undefined and should not be used.
        /// </summary>
        public int dwReserved0;

        /// <summary> Reserved for future use. </summary>
        public int dwReserved1;

        /// <summary> The name of the file. </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
        public string cFileName;

        /// <summary> An alternative name for the file. This name is in the classic 8.3 file name format.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ALTERNATE)]
        public string cAlternateFileName;
    }

    /// <summary>
    /// SafeFindHandle is a custom type that derives from SafeHandle. 
    /// It guarantees that related windows handle will be safely closed,
    /// thus avoiding resource leak. 
    /// The mechanism relies on these facts:
    /// 1/ The class overwrites SafeHandleZeroOrMinusOneIsInvalid.ReleaseHandle
    ///   and calls FindClose inside that method.
    /// 2/ When the runtime invokes a call to FindFirstFile, it first creates an instance 
    /// of SafeFindHandle. When FindFirstFile returns, the runtime stores 
    /// the resulting IntPtr into the already created SafeFindHandle. 
    /// The runtime guarantees that this operation is atomic, 
    /// meaning that if the P/Invoke method successfully returns, 
    /// the IntPtr will be stored safely inside the SafeHandle. 
    /// Once inside the SafeHandle, even if an asynchronous exception occurs 
    /// and prevents FindFirstFile's SafeFindHandle return value from being stored, 
    /// the relevant IntPtr is already stored within a managed object 
    /// whose finalizer will ensure its proper release.
    /// 
    /// For more info, see MSDN magazine October 2005, Stephen Toub:
    /// Keep Your Code Running with the Reliability Features of the .NET Framework,
    /// https://docs.microsoft.com/en-us/archive/msdn-magazine/2005/october/using-the-reliability-features-of-the-net-framework
    /// </summary>
    [ComVisible(false)]
    public sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeFindHandle()
            : base(true)
        { }

        /// <summary>
        /// Executes the code required to free the handle.
        /// </summary>
        /// <returns>
        /// true if the handle is released successfully; otherwise, in the event of a catastrophic failure,
        /// false. In this case, it generates a releaseHandleFailed MDA Managed Debugging Assistant.
        /// </returns>
        protected override bool ReleaseHandle()
        {
            return FindClose(this.handle);
        }
    }

    /// <summary>
    /// The delegate used for EnumSystemLocales argument ( callback method ) type.
    /// Note: The actual C prototype of callback function called by API EnumSystemLocales is
    /// <code> 
    /// BOOL CALLBACK EnumLocalesProc(LPWSTR lpLocaleString); 
    /// </code> 
    /// Here (in C#), the method EnumLocales which calls EnumSystemLocales 
    /// is using the fact that Unicode string can be marshaled to StringBuilder.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    /// <see cref="EnumSystemLocales"/> 
    /// <see cref="EnumLocales"/> 
    public delegate int SystemLocalesDelegate(StringBuilder text);

    /// <summary> The type of control signal received by the handler <see cref="HandlerRoutine"/>. </summary>
    public enum CtrlTypes
    {
        /// <summary> A CTRL+C signal was received, either from keyboard input or from a signal generated by 
        ///           the GenerateConsoleCtrlEvent function. </summary>
        CTRL_C_EVENT = 0,

        /// <summary> A CTRL+BREAK signal was received, either from keyboard input or from a signal generated by 
        ///             GenerateConsoleCtrlEvent.
        /// </summary>
        CTRL_BREAK_EVENT = 1,

        /// <summary> A signal that the system sends to all processes attached to a console when the user 
        ///            closes the console (either by clicking Close on the console window's window menu, 
        ///            or by clicking the End Task button command from Task Manager).
        /// </summary>
        CTRL_CLOSE_EVENT = 2,

        /// <summary>
        /// A signal that the system sends to all console processes when a user is logging off. This signal does not
        /// indicate which user is logging off, so no assumptions can be made.
        /// 
        /// Note that this signal is received only by services. Interactive applications are terminated at logoff, 
        /// so they are not present when the system sends this signal.
        /// </summary>
        CTRL_LOGOFF_EVENT = 5,

        /// <summary> A signal that the system sends when the system is shutting down.
        ///            Interactive applications are not present by the time the system sends this signal, 
        ///            therefore it can be received only be services in this situation. 
        ///            Services also have their own notification mechanism for shutdown events. 
        /// </summary>
        CTRL_SHUTDOWN_EVENT = 6,
    }


    /// <summary> An application-defined function used with the <see cref="SetConsoleCtrlHandler"/> function. 
    /// A console process uses this function to handle control signals received by the process. 
    /// When the signal is received, the system creates a new thread in the process to execute the function. </summary>
    ///
    /// <param name="CtrlType"> The type of control signal received by the handler. </param>
    ///
    /// <returns> If the function handles the control signal, it should return TRUE. 
    /// If it returns FALSE, the next handler function in the list of handlers for this process is used. </returns>
    public delegate bool HandlerRoutine(CtrlTypes CtrlType);

    #endregion // Nested types

    #region External functions

    /// <summary> Retrieves the full path and file name of the specified file. </summary>
    /// <param name="lpFileName"> The name of the file or directory. This parameter can be a short (the 8.3
    ///  form) or long file name. This string can also be a share or volume name. </param>
    /// 
    /// <param name="nBufferLength"> Length of the buffer, in characters (in TCHARs). </param>
    /// <param name="lpBuffer"> The buffer that receives the null-terminated string for the drive and path.
    /// This parameter cannot be null.</param>
    /// <param name="mustBeZero"> This argument must be IntPtr.Zero for proper usage from C# code. For the 
    /// original meaning of this argument in native code, see the MSDN documentation. </param>
    /// 
    /// <returns>
    /// If the function succeeds, the return value is the length, in TCHARs, of the string copied to lpBuffer, 
    /// not including the terminating null character.
    /// If the lpBuffer buffer is too small to contain the path, the return value is the size, in TCHARs, 
    /// of the buffer that is required to hold the path and the terminating null character.<br/>
    /// If the function fails for any other reason, the return value is zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// 
    /// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa364963(v=vs.85).aspx">
    /// MSDN documentation of GetFullPathName</seealso>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern uint GetFullPathName(string lpFileName, uint nBufferLength,
       StringBuilder lpBuffer, IntPtr mustBeZero);

    /// <summary> Copies an existing file to a new file. </summary>
    /// <param name="src"> The name of an existing file ( the source file ). </param>
    /// <param name="dst"> The name of the destination file.. </param>
    /// <param name="failIfExists"> If this parameter is true and the new file specified by <paramref name="dst"/> 
    /// already exists, the function fails. If this parameter is false and the new file already exists, 
    /// the function overwrites the existing file and succeeds. </param>
    /// <returns> true if it succeeds, false if it fails. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CopyFile(
      string src,
      string dst,
      [MarshalAs(UnmanagedType.Bool)] bool failIfExists);

    /// <summary> Moves an existing file or a directory, including its children. </summary>
    /// 
    /// <param name="lpPathNameFrom"> The current name of the file or directory on the local computer. </param>
    /// <param name="lpPathNameTo"> The new name for the file or directory. The new name must not already
    ///  exist. A new file may be on a different file system or drive. A new directory must be on the same
    ///  drive. </param>
    /// 
    /// <returns>
    /// true if it succeeds, false if it fails. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool MoveFile(string lpPathNameFrom, string lpPathNameTo);

    /// <summary> Deletes the file described by lpFileName. </summary>
    /// <remarks> Normally, the length of the argument <paramref name="lpFileName"/> must be less then
    /// 260 characters (the well-known MAX_PATH constant in the native code).
    /// However, this Windows APIs supports file paths that are longer than MAX_PATH in length if the file path 
    /// is eventually normalized, i.e. it contains \\?\ prefix. To achieve that, utilize the method
    /// Microsoft.Experimental.IO.LongPathCommon.NormalizeLongPath.
    /// The length of such path is limited to <see cref="Kernel32.MAX_LONG_PATH"/>.
    /// </remarks>
    /// 
    /// <param name="lpFileName"> The name of the file to be deleted. </param>
    /// <returns> true if it succeeds, false if it fails.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteFile(string lpFileName);

    /// <summary>
    /// Formats a message string. The function requires a message definition as input. The message
    /// definition can come from a buffer passed into the function. It can come from a message table
    /// resource in an already-loaded module. Or the caller can ask the function to search the system's
    /// message table resource(s) for the message definition. The function finds the message definition in
    /// a message table resource based on a message identifier and a language identifier. The function
    /// copies the formatted message text to an output buffer, processing any embedded insert sequences if
    /// requested.
    /// </summary>
    /// 
    /// <param name="dwFlags"> [in] The formatting options, and how to interpret the lpSource parameter.
    ///  The low-order byte of dwFlags specifies how the function handles line breaks in the output
    ///  buffer. The low-order byte can also specify the maximum width of a formatted output line. </param>
    /// 
    /// <param name="lpSource"> [in, optional] The location of the message definition. The type of this
    ///  parameter depends upon the settings in the dwFlags parameter. </param>
    /// 
    /// <param name="dwMessageId"> [in] The message identifier for the requested message. This parameter
    ///  is ignored if dwFlags includes FORMAT_MESSAGE_FROM_STRING. </param>
    /// 
    /// <param name="dwLanguageId"> [in] The language identifier for the requested message. This parameter
    ///  is ignored if dwFlags includes FORMAT_MESSAGE_FROM_STRING. </param>
    /// 
    /// <param name="lpBuffer"> [out] A buffer that receives the null-terminated string that specifies the
    ///  formatted message. If dwFlags includes FORMAT_MESSAGE_ALLOCATE_BUFFER, the function allocates a
    ///  buffer using the LocalAlloc function, and places the pointer to the buffer at the address
    ///  specified in lpBuffer. This buffer cannot be larger than 64K bytes. </param>
    /// 
    /// <param name="nSize"> If the FORMAT_MESSAGE_ALLOCATE_BUFFER flag is not set, this parameter
    ///  specifies the size of the output buffer, in TCHARs. If FORMAT_MESSAGE_ALLOCATE_BUFFER is set,
    ///  this parameter specifies the minimum number of TCHARs to allocate for an output buffer. The
    ///  output buffer cannot be larger than 64K bytes. </param>
    /// 
    /// <param name="va_list_arguments"> The variable arguments list arguments; i.e. nn array of values
    ///  that are used as insert values in the formatted message. A %1 in the format string indicates the
    ///  first value in the Arguments array; a %2 indicates the second argument; and so on. </param>
    /// 
    /// <returns>
    /// If the function succeeds, the return value is the number of TCHARs stored in the output buffer,
    /// excluding the terminating null character. If the function fails, the return value is zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern int FormatMessage(
      [MarshalAs(UnmanagedType.U4)] FormatMsg dwFlags,
      IntPtr lpSource,
      int dwMessageId,
      int dwLanguageId,
      StringBuilder lpBuffer,
      int nSize,
      IntPtr va_list_arguments);

    /// <summary> Gets system information, filling-in the structure <see cref="SYSTEM_INFO"/>. </summary>
    /// <param name="pSI" type="ref SYSTEM_INFO"> [in,out] The System Information. </param>
    [DllImport("kernel32")]
    public static extern void GetSystemInfo(ref SYSTEM_INFO pSI);

    /// <summary> Gets Memory status, filling-in the structure <see cref="MEMORYSTATUS"/>. </summary>
    /// <param name="buffer" type="ref MEMORYSTATUS"> [in,out] The resulting info about memory status. </param>
    /// <remarks>
    /// One does not have to initialize dwLength field; the function GlobalMemoryStatus does that itself.
    /// </remarks>
    [DllImport("kernel32")]
    public static extern void GlobalMemoryStatus(ref MEMORYSTATUS buffer);

    /// <summary>
    /// Retrieves information about the system's current usage of both physical and virtual memory.
    /// </summary>
    /// <param name="buffer" type="ref MEMORYSTATUSEX"> [in,out] The resulting info about memory status. </param>
    /// <returns> true if it succeeds, false if it fails. 
    ///            To get extended error information, call Marshal.GetLastWin32Error().</returns>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX buffer);

    /// <summary>
    /// Identifying the current operating system, filling-in the <see cref="OSVERSIONINFOEX"/> structure.
    /// Before calling the GetVersionEx function, set the OSVERSIONINFOEX.dwOSVersionInfoSize member of the
    /// structure as appropriate.
    /// </summary>
    /// <remarks>
    /// GetVersionEx may be altered or unavailable for releases after Windows 8.1. For more info see
    /// <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms724451(v=vs.85).aspx">
    /// GetVersionEx function on MSDN
    /// </see>
    /// </remarks>
    /// <param name="osVersionInfo" type="ref OSVERSIONINFOEX"> [in,out] Information describing the operating
    ///  system version. </param>
    /// <returns> true if it succeeds, false if it fails. 
    ///            To get extended error information, call Marshal.GetLastWin32Error().</returns>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetVersionEx(ref OSVERSIONINFOEX osVersionInfo);

    /// <summary> Allocates a new console for the calling process. </summary>
    /// <remarks>
    /// A process can be associated with only one console, so the AllocConsole function fails if the calling
    /// process already has a console. A process can use the FreeConsole function to detach itself from its
    /// current console, then it can call AllocConsole to create a new console or AttachConsole to attach to
    /// another console.
    /// </remarks>
    /// <returns> true if it succeeds, false if it fails. 
    ///            To get extended error information, call Marshal.GetLastWin32Error().</returns>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AllocConsole();

    /// <summary> Attaches the calling process to the console of the specified process.
    /// </summary>
    /// <param name="dwProcessId" type="uint"> Identifier for the process. </param>
    /// <returns> true if it succeeds, false if it fails. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AttachConsole(uint dwProcessId);

    /// <summary> Gets current thread identifier. </summary>
    /// <returns> The return value is the thread identifier of the calling thread.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true)]
    public static extern uint GetCurrentThreadId();

    /// <summary>
    /// Gets current thread handle. The returned value is actually a pseudo handle, a special
    /// constant that is interpreted as the current thread handle. The calling thread can use this
    /// handle to specify itself whenever a thread handle is required. Pseudo handles are not
    /// inherited by child processes.
    /// </summary>
    ///
    /// <returns>   The current thread. </returns>
    [DllImport("kernel32", SetLastError = true)]
    public static extern IntPtr GetCurrentThread();

    /// <summary> Retrieves the termination status of the specified thread. </summary>
    /// <param name="hThread" type="IntPtr"> A handle to the thread. </param>
    /// <param name="lpExitCode" type="ref int"> [out] A pointer to a variable to receive the thread
    ///  termination status. </param>
    /// <returns> True if it succeeds, false if it fails. 
    ///           To get extended error information, call Marshal.GetLastWin32Error().</returns>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetExitCodeThread(
        IntPtr hThread,
        ref int lpExitCode);

    /// <summary> Terminate given thread. cause a thread to exit. 
    ///             Cause a thread to exit immediately, with no chance to execute any user-mode code. 
    ///             DLLs attached to the thread are not notified that the thread is terminating. 
    ///             The system frees the thread's initial stack.
    /// </summary>
    ///
    /// <param name="hThread">      A handle to the thread to be terminated.. </param>
    /// <param name="dwExitCode">   The exit code for the thread. 
    ///                             Use the GetExitCodeThread function to retrieve a thread's exit value. </param>
    ///
    /// <returns>   True if it succeeds, false if it fails. </returns>
    [DllImport("kernel32", SetLastError = true)]
    public static extern bool TerminateThread(
        IntPtr hThread,
        uint dwExitCode);

    /// <summary> Ends the calling process and all its threads. </summary>
    ///
    /// <param name="uExitCode">  The exit code for the process and all threads. </param>
    /// <remarks> Use the GetExitCodeProcess function to retrieve the process's exit value. </remarks>
    /// 
    /// <seealso href="https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-exitprocess">
    /// MSDN about ExitProcess function.</seealso>
    /// <seealso cref="TerminateProcess"/>
    [DllImport("kernel32", SetLastError = true)]
    public static extern void ExitProcess(uint uExitCode);

    /// <summary> Terminates the specified process and all of its threads. </summary>
    ///
    /// <param name="hProcess"> A handle to the process to be terminated. </param>
    /// <param name="uExitCode"> The exit code for the process and all threads. </param>
    ///
    /// <returns>   True if it succeeds, false if it fails. </returns>
    ///
    /// <seealso href="https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-terminateprocess">
    /// MSDN about TerminateProcess function.
    /// </seealso>
    /// <seealso cref="ExitProcess"/>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

    /// <summary>
    /// Searches a directory for a file or subdirectory with a name that matches a specific name (or
    /// partial name if wildcards are used).
    /// </summary>
    /// <param name="lpFileName"> The directory or path, and the file name, which can include wildcard
    ///  characters, for example, an asterisk (*) or a question mark (?). </param>
    /// <param name="lpFindFileData"> [out] A reference to the WIN32_FIND_DATA structure that receives
    ///  information about a found file or directory. </param>
    /// <returns>
    /// If the function succeeds, the return value is a search handle used in a subsequent call to 
    /// <see cref="FindNextFile"/> or <see cref="FindClose"/>, and the <paramref name="lpFindFileData"/> parameter 
    /// contains information about the first file or directory found.<br/>
    /// If the function fails or fails to locate files from the search string in the lpFileName parameter, 
    /// the return value is <see cref="INVALID_HANDLE_VALUE"/>.<br/>
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// 
    /// <seealso cref="FindNextFile"/>
    /// <seealso cref="FindClose"/>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern SafeFindHandle FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

    /// <summary> Continues a file search from a previous call to the FindFirstFile, FindFirstFileEx, 
    /// or FindFirstFileTransacted functions. </summary>
    /// <param name="hFindFile"> The search handle returned by a previous call to the FindFirstFile 
    ///                            or FindFirstFileEx function. </param>
    /// <param name="lpFindFileData"> A <see cref="WIN32_FIND_DATA"/> structure that receives information 
    ///                               about the found file or subdirectory. </param>
    /// <returns> If it succeeds, the returned value is true and lpFindFileData parameter contains information 
    ///           about the next file or directory found. <br/>
    /// If it fails, the returned value is false and the contents of lpFindFileData is indeterminate.
    /// To get extended error information, call Marshal.GetLastWin32Error()..
    /// </returns>
    /// 
    /// <seealso cref="FindFirstFile"/>
    /// <seealso cref="FindClose"/>
    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FindNextFile(
        SafeFindHandle hFindFile,
        [In, Out, MarshalAs(UnmanagedType.LPStruct)] WIN32_FIND_DATA lpFindFileData);

    /// <summary> Closes a file search handle opened by the FindFirstFile, FindFirstFileEx, FindFirstFileNameW, 
    /// FindFirstFileNameTransactedW, FindFirstFileTransacted, FindFirstStreamTransactedW, 
    /// or FindFirstStreamW functions. </summary>
    /// <param name="hFindFile"> The file search handle. </param>
    /// <returns> true if it succeeds, false if it fails. <br/>
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// 
    /// <seealso cref="FindFirstFile"/>
    /// <seealso cref="FindNextFile"/>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FindClose(IntPtr hFindFile);

    /// <summary>
    /// Creates or opens a file or I/O device. The most commonly used I/O devices are as follows: file,
    /// file stream, directory, physical disk, volume, console buffer, tape drive, communications resource,
    /// mailslot, and pipe. The function returns a <see cref="SafeFileHandle"/> that can be used to access
    /// the file or device for various types of I/O depending on the file or device and the flags and
    /// attributes specified.
    /// </summary>
    /// <param name="lpFileName"> The name of the file or device to be created or opened. </param>
    /// <param name="dwDesiredAccess"> The requested access to the file or device, which can be summarized
    ///  as read, write, both or neither zero). The most commonly used values are
    ///  <see cref="Kernel32.EFileAccess.GenericRead"/>, <see cref="Kernel32.EFileAccess.GenericWrite"/>
    ///  or both. </param>
    /// <param name="dwShareMode"> The requested sharing mode of the file or device, which can be read,
    ///  write, both, delete, all of these, or none. </param>
    /// <param name="securityAttributes"> A pointer to a SECURITY_ATTRIBUTES structure that contains two
    ///  separate but related data members: an optional security descriptor, and a Boolean value that
    ///  determines whether the returned handle can be inherited by child processes. This parameter can be
    ///  null. </param>
    /// <param name="dwCreationDisposition"> An action to take on a file or device that exists or does not
    ///  exist. For devices other than files, this parameter is usually set to OPEN_EXISTING. </param>
    /// <param name="dwFlagsAndAttributes"> The file or device attributes and flags, created as combination
    ///  of <see cref="Kernel32.CreateFileFlags"/> enum values with <see cref="System.IO.FileAttributes"/>
    ///  enumeration, with <b>FileAttributes.Normal</b> being the most common default value for files.
    ///  This parameter can include any combination of the Kernel32.CreateFileFlags and 
    ///  System.IO.FileAttributes. Note that all other file attributes override FileAttributes.Normal.
    ///  </param>
    /// <param name="hTemplateFile"> A valid handle to a template file with the GENERIC_READ access right.
    ///  The template file supplies file attributes and extended attributes for the file that is being
    ///  created. This parameter can be IntPtr.Zero. </param>
    /// <returns> If the function succeeds, the return value is an open handle to the specified file, 
    ///  device, named pipe, or mail slot.
    ///  If the function fails, the return value is <see cref="INVALID_HANDLE_VALUE"/>.
    ///  To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern SafeFileHandle CreateFile(
      [MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
      EFileAccess dwDesiredAccess,
      FileShare dwShareMode,
      IntPtr securityAttributes,
      FileMode dwCreationDisposition,
      uint dwFlagsAndAttributes,
      IntPtr hTemplateFile);

    /// <summary>
    /// Creates a new directory. If the underlying file system supports security on files and directories,
    /// the function applies a specified security descriptor to the new directory.
    /// </summary>
    /// 
    /// <param name="lpPathName"> The path of the directory to be created. There is a default string size
    ///  limit for paths of 248 characters. This limit is related to how the CreateDirectory function
    ///  parses paths. To extend this limit to 32,767 wide characters, call the Unicode version of the
    ///  function and prepend "\\?\". </param>
    /// 
    /// <param name="lpSecurityAttributes"> A reference to a SECURITY_ATTRIBUTES structure. 
    /// The lpSecurityDescriptor member of the structure specifies a security descriptor for the new directory. 
    /// If lpSecurityAttributes is NULL, the directory gets a default security descriptor. </param>
    /// 
    /// <returns> true if it succeeds, false if it fails.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CreateDirectory(string lpPathName,
       IntPtr lpSecurityAttributes);

    /// <summary> Deletes an existing empty directory. </summary>
    /// <remarks> Normally, the length of the argument <paramref name="lpPathName"/> must be less then
    /// 260 characters (the well-known MAX_PATH constant in the native code).
    /// However, this Windows APIs supports file paths that are longer than MAX_PATH in length if the file path 
    /// is eventually normalized, i.e. it contains \\?\ prefix. To achieve that, utilize the method
    /// Microsoft.Experimental.IO.LongPathCommon.NormalizeLongPath.
    /// The length of such path is limited to <see cref="Kernel32.MAX_LONG_PATH"/>.
    /// </remarks>
    /// 
    /// <param name="lpPathName"> The path of the directory to be removed. This path must specify an empty 
    /// directory, and the calling process must have delete access to the directory. </param>
    /// <returns> true if it succeeds, false if it fails.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool RemoveDirectory(string lpPathName);

    /// <summary> Opens file mapping. </summary>
    /// <param name="dwDesiredAccess"> The access to the file mapping object. This access is checked against
    ///  any security descriptor on the target file mapping object. For a list of values, see File Mapping
    ///  Security and Access Rights. </param>
    /// <param name="bInheritHandle"> If this parameter is true, a process created by the CreateProcess
    ///  function can inherit the handle; otherwise, the handle cannot be inherited. </param>
    /// <param name="lpName"> The name of the file mapping object to be opened. If there is an open handle to
    ///  a file mapping object by this name and the security descriptor on the mapping object does not
    ///  conflict with the dwDesiredAccess parameter, the open operation succeeds. The name can have a
    ///  "Global\" or "Local\" prefix to explicitly open an object in the global or session namespace. The
    ///  remainder of the name can contain any character except the backslash character (\). For more
    ///  information, see Kernel Object Namespaces. Fast user switching is implemented using Terminal
    ///  Services sessions. The first user to log on uses session 0, the next user to log on uses session 1,
    ///  and so on. Kernel object names must follow the guidelines outlined for Terminal Services so that
    ///  applications can support multiple users. </param>
    /// <returns>
    /// If the function succeeds, the return value is an open handle to the specified file mapping object.
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern SafeFileHandle OpenFileMapping(
      uint dwDesiredAccess,
      [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
      [MarshalAs(UnmanagedType.LPWStr)] string lpName);

    /// <summary> Gets file size. </summary>
    /// <remarks> It is recommended that you use GetFileSizeEx instead. </remarks>
    /// <param name="hFile"> A handle to the file. </param>
    /// <param name="lpFileSizeHigh"> [out] The variable where the high-order double-word of the file size is
    ///  returned. </param>
    /// <returns>
    /// If the function succeeds, the return value is the low-order double-word of the file size, and the
    /// function puts the high-order double-word of the file size into the variable
    /// <paramref name="lpFileSizeHigh"/>.
    /// If the function fails, the return value is INVALID_FILE_SIZE.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", EntryPoint = "GetFileSize", SetLastError = true)]
    public static extern uint GetFileSize(
      SafeFileHandle hFile,
      out uint lpFileSizeHigh);

    /// <summary> Retrieves file system attributes for a specified file or directory. </summary>
    /// <param name="lpFileName"> The name of the file or directory. </param>
    /// <returns>
    /// If the function succeeds, the return value contains the attributes of the specified file
    /// or directory.  
    /// If the function fails, the return value is <see cref="INVALID_FILE_ATTRIBUTES"/>. <br/>
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern FileAttributes GetFileAttributes(string lpFileName);

    /// <summary> Sets the attributes for a file or directory. </summary>
    /// <param name="lpFileName"> The name of the file or directory. </param>
    /// <param name="dwFileAttributes"> The file attributes. </param>
    /// <returns> true if it succeeds, false if it fails. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    ///  </returns>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetFileAttributes(
      string lpFileName,
      [MarshalAs(UnmanagedType.U4)] FileAttributes dwFileAttributes);

    /// <summary> Calls the corresponding CreateFileMapping API. </summary>
    /// <remarks>
    /// The purpose of result declaration as SafeFileHandle is to make sure that system memory is properly
    /// released regardless of any asynchronous thread aborts. For more details see
    /// <a href="https://docs.microsoft.com/en-us/archive/msdn-magazine/2005/october/using-the-reliability-features-of-the-net-framework">
    /// Keep Your Code Running with the Reliability Features of the .NET Framework</a>
    /// by Stephen Toub, MSDN Magazine Oct 2005.
    /// </remarks>
    /// <param name="hFile"> A handle to the file from which to create a file mapping object. </param>
    /// <param name="lpAttributes"> A pointer to a SECURITY_ATTRIBUTES structure that determines whether a
    ///  returned handle can be inherited by child processes. The lpSecurityDescriptor member of the
    ///  SECURITY_ATTRIBUTES structure specifies a security descriptor for a new file mapping object.
    ///  
    ///  If lpAttributes is null, the handle cannot be inherited and the file mapping object gets a default
    ///  security descriptor. </param>
    /// 
    /// <param name="flProtect"> Specifies the page protection of the file mapping object. All mapped views
    ///  of the object must be compatible with this protection. </param>
    /// 
    /// <param name="dwMaximumSizeHigh"> The high-order DWORD of the maximum size of the file mapping
    ///  object. </param>
    /// 
    /// <param name="dwMaximumSizeLow"> The low-order DWORD of the maximum size of the file mapping
    ///  object.<br/>
    ///  If this parameter and dwMaximumSizeHigh are 0 (zero), the maximum size of the file mapping object
    ///  is equal to the current size of the file that hFile identifies.
    ///  An attempt to map a file with a length of 0 (zero) fails with an error code of ERROR_FILE_INVALID.
    ///  Applications should test for files with a length of 0 (zero) and reject those files. </param>
    /// <param name="lpName"> . </param>
    /// <returns> If the function succeeds, the return value is a handle to the newly created file mapping.
    /// If the function fails, the return value is IntPtr.Zero.  
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern SafeFileHandle CreateFileMapping(
      IntPtr hFile,
      IntPtr lpAttributes,
      uint flProtect,
      uint dwMaximumSizeHigh,
      uint dwMaximumSizeLow,
      [MarshalAs(UnmanagedType.LPWStr)] string lpName);

    /// <summary> Maps a view of a file mapping into the address space of a calling process. </summary>
    /// 
    /// <param name="hFileMappingObject" type="IntPtr"> A handle to a file mapping object. The
    ///  CreateFileMapping and OpenFileMapping functions return this handle. </param>
    /// <param name="dwDesiredAccess"> The type of access to a file mapping object, which determines the
    ///  protection of the pages,. </param>
    /// <param name="dwFileOffsetHigh" type="uint"> A high-order DWORD of the file offset where the view
    ///  begins. </param>
    /// <param name="dwFileOffsetLow" type="uint"> A low-order DWORD of the file offset where the view is to
    ///  begin. The combination of the high and low offsets must specify an offset within the file mapping.
    ///  They must also match the memory allocation granularity of the system. That is, the offset must be a
    ///  multiple of the allocation granularity. To obtain the memory allocation granularity of the system,
    ///  use the GetSystemInfo function, which fills in the members of a SYSTEM_INFO structure. </param>
    /// <param name="dwNumberOfBytesToMap" type="UIntPtr"> The number of bytes of a file mapping to map to
    ///  the view. All bytes must be within the maximum size specified by CreateFileMapping. If this
    ///  parameter is 0 (zero), the mapping extends from the specified offset to the end of the file
    ///  mapping. </param>
    /// <returns> If the function succeeds, the return value is the starting address of the mapped view.
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32")]
    public static extern IntPtr MapViewOfFile(
      IntPtr hFileMappingObject,
      uint dwDesiredAccess,
      uint dwFileOffsetHigh,
      uint dwFileOffsetLow,
      UIntPtr dwNumberOfBytesToMap);

    /// <summary> Unmaps a mapped view of a file from the calling process's address space. </summary>
    /// 
    /// <param name="lpBaseAddress" type="IntPtr"> A pointer to the base address of the mapped view of a
    ///  file that is to be unmapped. This value must be identical to the value returned by a previous call
    ///  to the MapViewOfFile or MapViewOfFileEx function. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("kernel32", EntryPoint = "UnmapViewOfFile", SetLastError = true, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

    // only if compiling with /unsafe
#if UNSAFE
    [DllImport("kernel32", SetLastError = true)]
    public static extern unsafe int ReadFile(
        SafeFileHandle handle, 
        byte* bytes, 
        int numBytesToRead, 
        int* numBytesRead,
        void* overlapped);
#endif

    /// <summary>
    /// Reads data from the specified file or input/output (I/O) device. Reads occur at the position
    /// specified by the file pointer if supported by the device.
    /// </summary>
    /// 
    /// <param name="handle" type="SafeFileHandle"> A handle to the device (for example, a file, file stream,
    ///  physical disk, volume, console buffer, tape drive, socket, communications resource, mailslot, or
    ///  pipe). </param>
    /// <param name="bytes" type="Byte[]"> The buffer that receives the data read from a file or device. </param>
    /// <param name="numBytesToRead" type="int"> The maximum number of bytes to reads. </param>
    /// <param name="numBytesRead" type="ref int"> [out] the variable that receives the number of bytes read
    ///  when using a synchronous hFile parameter. </param>
    /// <param name="lpOverlapped" type="ref System.Threading.Overlapped"> [in,out] A pointer to an
    ///  OVERLAPPED structure is required if the hFile parameter was opened with FILE_FLAG_OVERLAPPED,
    ///  otherwise it can be IntPtr.Zero. </param>
    /// <returns> If the function succeeds, the return value is nonzero.
    /// If the function fails, or is completing asynchronously, the return value is zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true)]
    public static extern int ReadFile(
      SafeFileHandle handle,
      Byte[] bytes,
      int numBytesToRead,
      ref int numBytesRead,
      System.Threading.Overlapped lpOverlapped);

    /// <summary> Locks the specified file for exclusive access by the calling process. </summary>
    /// <param name="hFile"> A handle to the file. The file handle must have been created with the
    ///  <see cref="Kernel32.EFileAccess.GenericRead"/> or
    ///  <see cref="Kernel32.EFileAccess.GenericWrite"/>
    ///  access right. </param>
    /// <param name="dwFileOffsetLow"> The low-order 32 bits of the starting byte offset in the file where
    ///  the lock should begin. </param>
    /// <param name="dwFileOffsetHigh"> The high-order 32 bits of the starting byte offset in the file
    ///  where the lock should begin. </param>
    /// <param name="nNumberOfBytesToLockLow"> The low-order 32 bits of the length of the byte range to be
    ///  locked. </param>
    /// <param name="nNumberOfBytesToLockHigh"> The high-order 32 bits of the length of the byte range to
    ///  be locked. </param>
    /// 
    /// <returns> true if it succeeds, false if it fails.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// <seealso cref="LockFileEx"/>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool LockFile(
      IntPtr hFile,
      uint dwFileOffsetLow,
      uint dwFileOffsetHigh,
      uint nNumberOfBytesToLockLow,
      uint nNumberOfBytesToLockHigh);

    /// <summary>
    /// Locks the specified file for exclusive access by the calling process. This function can operate
    /// either synchronously or asynchronously and can request either an exclusive or a shared lock.
    /// </summary>
    /// <param name="hFile"> A handle to the file. The file handle must have been created with the
    ///  <see cref="Kernel32.EFileAccess.GenericRead"/> or
    ///  <see cref="Kernel32.EFileAccess.GenericWrite"/> access right. </param>
    /// <param name="dwFlags"> This parameter may be one or more of the following values:
    ///  <list type="bullet">
    ///  <item><b>LOCKFILE_EXCLUSIVE_LOCK</b>0x00000002<br/>The function requests an exclusive lock.
    ///  Otherwise, it requests a shared lock.</item>
    ///  <item><b>LOCKFILE_FAIL_IMMEDIATELY</b>0x00000001<br/>The function returns immediately if it is
    ///  unable to acquire the requested lock. Otherwise, it waits.</item>
    ///  </list> 
    /// </param>
    /// <param name="dwReserved"> Reserved parameter; must be set to zero. </param>
    /// <param name="nNumberOfBytesToLockLow"> The low-order 32 bits of the length of the byte range to be
    ///  locked. </param>
    /// <param name="nNumberOfBytesToLockHigh"> The high-order 32 bits of the length of the byte range to
    ///  be locked. </param>
    /// 
    /// <param name="lpOverlapped"> The overlapped structure that the function uses with the locking
    ///  request. This structure, which is required, contains the file offset of the beginning of the lock
    ///  range. You must initialize the hEvent member to a valid handle or zero. </param>
    /// 
    /// <returns> true if it succeeds, false if it fails.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// <seealso cref="LockFile"/>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool LockFileEx(
      IntPtr hFile,
      uint dwFlags,
      uint dwReserved,
      uint nNumberOfBytesToLockLow,
      uint nNumberOfBytesToLockHigh,
      ref OVERLAPPED lpOverlapped);

    /// <summary>
    /// Unlocks a region in an open file. Unlocking a region enables other processes to access the region.
    /// </summary>
    /// <param name="hFile"> A handle to the file that contains a region locked with LockFile. The file
    ///  handle must have been created with either the GENERIC_READ or GENERIC_WRITE access right. </param>
    /// <param name="dwFileOffsetLow" type="uint"> The low-order 32 bits of the starting byte offset in the
    ///  file where the locked region begins. </param>
    /// <param name="dwFileOffsetHigh" type="uint"> The high-order 32 bits of the starting byte offset in the
    ///  file where the locked region begins. </param>
    /// <param name="nNumberOfBytesToUnlockLow" type="uint"> The low-order word of the length of the byte
    ///  range to be unlocked. </param>
    /// <param name="nNumberOfBytesToUnlockHigh" type="uint"> The high-order word of the length of the byte
    ///  range to be unlocked. </param>
    /// <returns>
    /// true if it succeeds, false if it fails. In such case you should call Marshal.GetLastWin32Error() to
    /// get more information.
    /// </returns>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnlockFile(
      IntPtr hFile,
      uint dwFileOffsetLow,
      uint dwFileOffsetHigh,
      uint nNumberOfBytesToUnlockLow,
      uint nNumberOfBytesToUnlockHigh);

    /// <summary> Retrieves a pseudo handle for the current process. </summary>
    /// <remarks>
    /// A pseudo handle is a special constant, currently (HANDLE)-1, that is interpreted as the current
    /// process handle. For compatibility with future operating systems, it is best to call GetCurrentProcess
    /// instead of hard-coding this constant value. The calling process can use a pseudo handle to specify
    /// its own process whenever a process handle is required. Pseudo handles are not inherited by child
    /// processes.
    /// </remarks>
    /// <returns> The return value is a pseudo handle to the current process. </returns>
    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr GetCurrentProcess();

    /// <summary> Retrieves the process identifier of the calling process. </summary>
    /// <returns>   The current process identifier. </returns>
    [DllImport("kernel32", SetLastError = true)]
    private static extern uint GetCurrentProcessId();

    /// <summary> Closes an open object handle. </summary>
    /// <param name="hHandle" type="IntPtr"> A valid handle to an open object. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("kernel32", EntryPoint = "CloseHandle", SetLastError = true, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(IntPtr hHandle);

    /// <summary> Duplicates an object handle. </summary>
    /// <param name="hSourceProcessHandle"> A handle to the process with the handle to be duplicated. </param>
    /// <param name="hSourceHandle"> The handle to be duplicated. This is an open object handle that is
    ///  valid in the context of the source process. </param>
    /// <param name="hTargetProcessHandle" type="IntPtr"> A handle to the process that is to receive the
    ///  duplicated handle. </param>
    /// <param name="lpTargetHandle" type="ref IntPtr"> [out] to a variable that receives the duplicate
    ///  handle. This handle value is valid in the context of the target process. </param>
    /// <param name="dwDesiredAccess"> The dwDesiredAccess parameter specifies the new handle's access
    ///  rights. All objects support the standard access rights <see cref="Kernel32.EFileAccess.GenericRead"/> and
    ///  and <see cref="Kernel32.EFileAccess.GenericWrite"/>. 
    ///  Objects may also support additional access rights depending on the object type.
    ///  
    ///  For more info see
    ///  <see href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms724251(v=vs.85).aspx">
    ///  DuplicateHandle function </see> MSDN documentation. </param>
    /// <param name="bInheritHandle"> A an argument that indicates whether the resulting handle is
    ///  inheritable. If this parameter is true, the duplicate handle can be inherited by new processes
    ///  created by the target process. If false, the new handle cannot be inherited. </param>
    /// <param name="dwOptions" type="uint"> Optional actions. This parameter can be zero, or any
    ///  combination of the following values.
    ///  <list type="bullet">
    ///  <item><b>DUPLICATE_CLOSE_SOURCE</b>0x00000001<br/>
    ///  Closes the source handle. This occurs regardless of any error status returned.
    ///  </item>
    ///  <item><b>DUPLICATE_SAME_ACCESS</b>0x00000002<br/>
    ///  Ignores the dwDesiredAccess parameter. The duplicate handle has the same access as the source
    ///  handle.
    ///  </item>
    ///  </list> </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DuplicateHandle(
      IntPtr hSourceProcessHandle,
      IntPtr hSourceHandle,
      IntPtr hTargetProcessHandle,
      ref IntPtr lpTargetHandle,
      int dwDesiredAccess,
      [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
      uint dwOptions);

    /// <summary>
    /// Retrieves a module handle for the specified module. The module must have been loaded by the calling
    /// process.
    /// </summary>
    /// <param name="lpModuleName" type="string"> The name of the loaded module (either a .dll or .exe file).
    ///  If the file name extension is omitted, the default library extension .dll is appended. The file
    ///  name string can include a trailing point character (.) to indicate that the module name has no
    ///  extension. The string does not have to specify a path. When specifying a path, be sure to use
    ///  backslashes (\), not forward slashes (/). </param>
    /// <returns> The module handle. </returns>
    [DllImport("kernel32", SetLastError = true)]
    public static extern IntPtr GetModuleHandle(
      [MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

    // Note: The declaration of GetProcAddress must have CharSet = CharSet.Ansi,
    // and the argument procName must be marshaled as UnmanagedType.LPStr,
    // to match the actual API declaration
    //   WINAPI GetProcAddress(__in  HMODULE hModule, __in  LPCSTR lpProcName);
    // To satisfy the FxCop rule SpecifyMarshalingForPInvokeStringArguments
    // ( see http://msdn.microsoft.com/cs-cz/library/ms182319(en-us).aspx )
    // we also apply BestFitMapping = false
    [DllImport("kernel32", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true,
      ExactSpelling = true, SetLastError = true)]
    internal static extern IntPtr GetProcAddress(
      IntPtr hModule,
      [MarshalAs(UnmanagedType.LPStr)] string procName);

    /// <summary>
    /// Retrieves the fully qualified path for the file that contains the specified module. The module must
    /// have been loaded by the current process.
    /// </summary>
    /// <remarks>
    /// To locate the file for a module that was loaded by another process, use the GetModuleFileNameEx
    /// function.
    /// </remarks>
    /// <param name="hModule"> A handle to the loaded module whose path is being requested. If this
    ///  parameter is IntPtr.Zero, GetModuleFileName retrieves the path of the executable file of the
    ///  current process. </param>
    /// <param name="lpBuffer"> A buffer that receives the fully qualified path of the module. </param>
    /// <param name="nSize"> The size of the lpFilename buffer, in TCHARs (Unicode characters). </param>
    /// <returns>
    /// If the function succeeds, the return value is the length of the string that is copied to the buffer,
    /// in characters, not including the terminating null character. If the buffer is too small to hold the
    /// module name, the string is truncated to nSize characters including the terminating null character,
    /// the function returns nSize, and the function sets the last error to ERROR_INSUFFICIENT_BUFFER.
    /// </returns>
    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    [PreserveSig]
    public static extern uint GetModuleFileName(
      IntPtr hModule,
      [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpBuffer,
      int nSize);

    /* When any other PInvoke(d) function is marked with SetLastError=true,
     * this makes the CLR call GetLastError immediately after it calls the target unmanaged API 
     * and save the result.
     * Therefore, never define a PInvoke signature for GetLastError from kernel32.dll.  
     * If managed code calls such a method, it will not get reliable results. 
     * You should call Marshal.GetLastWin32Error() instead.
     * 
    [DllImport("kernel32", EntryPoint = "GetLastError", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern uint GetLastError();
    */

    /// <summary>
    /// Retrieves information about the specified disk, including the amount of free space on the disk.
    /// </summary>
    /// 
    /// <param name="lpRootPathName" type="string"> The root directory of the disk for which information is
    ///  to be returned. If this parameter is NULL, the function uses the root of the current disk. If this
    ///  parameter is a UNC name, it must include a trailing backslash (for example, "\\MyServer\MyShare\").
    ///  Furthermore, a drive specification must have a trailing backslash (for example, "C:\"). The calling
    ///  application must have FILE_LIST_DIRECTORY access rights for this directory. </param>
    /// <param name="lpSectorsPerCluster" type="out uint"> [out] A pointer to a variable that receives the
    ///  number of sectors per cluster.
    ///  </param>
    /// 
    /// <param name="lpBytesPerSector" type="out uint"> [out] A pointer to a variable that receives the
    ///  number of bytes per sector. </param>
    /// <param name="lpNumberOfFreeClusters" type="out uint"> [out] A pointer to a variable that receives the
    ///  total number of free clusters on the disk that are available to the user who is associated with the
    ///  calling thread. If per-user disk quotas are in use, this value may be less than the total number of
    ///  free clusters on the disk.
    ///  </param>
    /// 
    /// <param name="lpTotalNumberOfClusters" type="out uint"> [out] A pointer to a variable that receives
    ///  the total number of clusters on the disk that are available to the user who is associated with the
    ///  calling thread.
    ///  If per-user disk quotas are in use, this value may be less than the total number of clusters on the
    ///  disk.
    ///  </param>
    /// 
    /// <returns> true if it succeeds, false if it fails. 
    ///            To get extended error information, call Marshal.GetLastWin32Error().</returns>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetDiskFreeSpace(
    [MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName,
       out uint lpSectorsPerCluster,
       out uint lpBytesPerSector,
       out uint lpNumberOfFreeClusters,
       out uint lpTotalNumberOfClusters);

    /// <summary>
    /// Retrieves information about the amount of space that is available on a disk volume, which is the
    /// total amount of space, the total amount of free space, and the total amount of free space available
    /// to the user that is associated with the calling thread.
    /// </summary>
    /// <param name="lpDirectoryName" type="string"> A directory on the disk. </param>
    /// <param name="lpFreeBytesAvailable" type="out ulong"> [out] A pointer to a variable that receives the
    ///  total number of free bytes on a disk that are available to the user who is associated with the
    ///  calling thread. </param>
    /// <param name="lpTotalNumberOfBytes" type="out ulong"> [out] A pointer to a variable that receives the
    ///  total number of bytes on a disk that are available to the user who is associated with the calling
    ///  thread. </param>
    /// <param name="lpTotalNumberOfFreeBytes" type="out ulong"> [out] A pointer to a variable that receives
    ///  the total number of bytes on a disk that are available to the user who is associated with the
    ///  calling thread. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetDiskFreeSpaceEx(
       [MarshalAs(UnmanagedType.LPWStr)] string lpDirectoryName,
       out ulong lpFreeBytesAvailable,
       out ulong lpTotalNumberOfBytes,
       out ulong lpTotalNumberOfFreeBytes);

    /// <summary> Creates or opens a named or unnamed mutex object. </summary>
    /// <remarks>
    /// If MutexName matches the name of an existing event, semaphore, waitable timer, job, or file-mapping
    /// object, the function fails and the GetLastError function returns ERROR_INVALID_HANDLE. This occurs
    /// because these objects share the same namespace.
    /// 
    /// The name can have a "Global\" or "Local\" prefix to explicitly create the object in the global or
    /// session namespace. The remainder of the name can contain any character except the backslash character
    /// (\). For more information, see Kernel Object Namespaces. Fast user switching is implemented using
    /// Terminal Services sessions. Kernel object names must follow the guidelines outlined for Terminal
    /// Services so that applications can support multiple users.
    /// 
    /// The object can be created in a private namespace.
    /// </remarks>
    /// 
    /// <param name="lpMutexAttributes" type="IntPtr"> A pointer to a SECURITY_ATTRIBUTES structure. If this
    ///  parameter is NULL, the handle cannot be inherited by child processes. </param>
    /// <param name="InitialOwner" type="bool"> If this value is true and the caller created the mutex, the
    ///  calling thread obtains initial ownership of the mutex object. Otherwise, the calling thread does
    ///  not obtain ownership of the mutex. To determine if the caller created the mutex, see the Return
    ///  Values section. </param>
    /// 
    /// <param name="MutexName" type="string"> The name of the mutex object. The name is limited to MAX_PATH
    ///  characters. Name comparison is case sensitive.
    ///  
    ///  If MutexName matches the name of an existing named mutex object, this function requests the
    ///  MUTEX_ALL_ACCESS access right. In this case, the bInitialOwner parameter is ignored because it has
    ///  already been set by the creating process. If the MutexName parameter is not NULL, it determines
    ///  whether the handle can be inherited, but its security-descriptor member is ignored.
    ///  
    ///  If MutexName is NULL, the mutex object is created without a name. </param>
    /// 
    /// <returns>
    /// If the function succeeds, the return value is a handle to the newly created mutex object.
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// 
    /// <remarks>
    /// If the mutex is a named mutex and the object existed before this function call, the return value is a
    /// handle to the existing object, GetLastError returns ERROR_ALREADY_EXISTS, bInitialOwner is ignored,
    /// and the calling thread is not granted ownership. However, if the caller has limited access rights,
    /// the function will fail with ERROR_ACCESS_DENIED and the caller should use the OpenMutex function.
    /// </remarks>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr CreateMutex(
      IntPtr lpMutexAttributes,
      [MarshalAs(UnmanagedType.Bool)] bool InitialOwner,
      [MarshalAs(UnmanagedType.LPWStr)] string MutexName);

    /// <summary> Releases the mutex described by hMutex. </summary>
    /// <param name="hMutex" type="IntPtr"> A handle to the mutex object. The CreateMutex or OpenMutex
    ///  function returns this handle. </param>
    /// <returns>
    /// true if it succeeds, false if it fails. In such case you should call Marshal.GetLastWin32Error() 
    /// to get more information.
    /// </returns>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReleaseMutex(
      IntPtr hMutex);

    /// <summary> Creates or opens a named or unnamed semaphore object. </summary>
    /// <param name="lpSemaphoreAttributes" type="IntPtr"> A pointer to a SECURITY_ATTRIBUTES structure. If
    ///  this parameter is IntPtr.Zero, the handle cannot be inherited by child processes. </param>
    /// 
    /// <param name="initialCount" type="int"> The initial count for the semaphore object. This value must
    ///  be greater than or equal to zero and less than or equal to lMaximumCount. The state of a semaphore
    ///  is signaled when its count is greater than zero and non-signaled when it is zero. The count is
    ///  decreased by one whenever a wait function releases a thread that was waiting for the semaphore. The
    ///  count is increased by a specified amount by calling the ReleaseSemaphore function. </param>
    /// 
    /// <param name="maximumCount" type="int"> The maximum count for the semaphore object. This value must
    ///  be greater than zero. </param>
    /// 
    /// <param name="name" type="string"> The name of the semaphore object. 
    ///  The name is limited to MAX_PATH characters. Name comparison is case sensitive.
    /// </param>
    /// 
    /// <returns> If the function succeeds, the return value is a handle to the semaphore object. 
    /// If the named semaphore object existed before the function call, 
    /// the function returns a handle to the existing object and GetLastError returns ERROR_ALREADY_EXISTS.
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr CreateSemaphore(
      IntPtr lpSemaphoreAttributes,
      int initialCount,
      int maximumCount,
      [MarshalAs(UnmanagedType.LPWStr)] string name);

    /// <summary> Opens an existing named semaphore object. </summary>
    /// 
    /// <param name="desiredAccess"> The access to the semaphore object. The function fails if the security
    ///  descriptor of the specified object does not permit the requested access for the calling process. </param>
    /// <param name="inheritHandle"> If this value is true, processes created by this process will inherit
    ///  the handle. Otherwise, the processes do not inherit this handle. </param>
    /// <param name="name" type="string"> The name of the semaphore object. Name comparisons are case
    ///  sensitive. </param>
    /// 
    /// <returns>
    /// If the function succeeds, the return value is a handle to the semaphore object. 
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr OpenSemaphore(
      int desiredAccess,
      [MarshalAs(UnmanagedType.Bool)] bool inheritHandle,
      [MarshalAs(UnmanagedType.LPWStr)] string name);

    /// <summary>
    /// Releases the semaphore. This function increases the count of the specified semaphore object by a
    /// specified amount.
    /// </summary>
    /// 
    /// <param name="handle" type="IntPtr"> Handle to the semaphore object. The CreateSemaphore function
    ///  returns this handle. </param>
    /// <param name="releaseCount" type="int"> Number of releases. Specifies the amount by which the current
    ///  count of the semaphore object is to be increased. The value must be greater than zero. If the
    ///  specified amount would cause the count of the semaphore to exceed the maximum count that was
    ///  specified when the semaphore was created, the count is not changed and the function returns FALSE. </param>
    /// <param name="previousCount" type="out int"> [out] 32-bit variable to receive the previous count for
    ///  the semaphore. This parameter can be NULL if the previous count is not required. </param>
    /// <returns> 
    /// True indicates success. False indicates failure;
    /// in such case you should call Marshal.GetLastWin32Error() to get more information.
    /// </returns>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ReleaseSemaphore(
      IntPtr handle,
      int releaseCount,
      out int previousCount);

    /// <summary> Creates or opens a named or unnamed event object. </summary>
    /// 
    /// <param name="lpEventAttributes" type="IntPtr"> A pointer to a SECURITY_ATTRIBUTES structure. If this
    ///  parameter is IntPtr.Zero, the handle cannot be inherited by child processes. </param>
    /// 
    /// <param name="bManualReset"> If this parameter is true, the function creates a manual-reset event
    ///  object, which requires the use of the ResetEvent function to set the event state to non-signaled. 
    ///  If this parameter is false, the function creates an auto-reset event object, and system automatically
    ///  resets the event state to non-signaled after a single waiting thread has been released. </param>
    /// 
    /// <param name="bInitialState"> If this parameter is true, the initial state of the event object is
    ///  signaled; otherwise, it is non-signaled. </param>
    /// 
    /// <param name="lpName"> The name of the event object. The name is limited to MAX_PATH characters. Name
    ///  comparison is case sensitive. If lpName is null, the event object is created without a name. </param>
    /// 
    /// <returns> If the function succeeds, the return value is a handle to the event object. 
    /// If the named event object existed before the function call, the function returns a handle 
    /// to the existing object and GetLastError returns ERROR_ALREADY_EXISTS.
    ///
    /// If the function succeeds, the return value is a handle to the semaphore object. 
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true)]
    public static extern IntPtr CreateEvent(
      IntPtr lpEventAttributes,
      [MarshalAs(UnmanagedType.Bool)] bool bManualReset,
      [MarshalAs(UnmanagedType.Bool)] bool bInitialState,
      [MarshalAs(UnmanagedType.LPWStr)] string lpName);

    /// <summary> Opens an existing named event object. </summary>
    /// 
    /// <param name="dwDesiredAccess"> The access to the event object. The function fails if the security
    ///  descriptor of the specified object does not permit the requested access for the calling process. </param>
    /// <param name="bInheritHandle"> If this parameter is true, a process created by the CreateProcess
    ///  function can inherit the handle; otherwise, the handle cannot be inherited. </param>
    /// <param name="lpName"> The name of the event to be opened. Name comparisons are case sensitive. </param>
    /// 
    /// <returns>
    /// If the function succeeds, the return value is a handle to the Event object. 
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true)]
    public static extern IntPtr OpenEvent(
      UInt32 dwDesiredAccess,
      [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
      [MarshalAs(UnmanagedType.LPWStr)] string lpName);

    /// <summary> Sets the specified event object to the signaled state.
    /// </summary>
    /// <param name="handle" type="IntPtr"> A handle to the event object. 
    /// The CreateEvent or OpenEvent function returns this handle. </param>
    /// <returns> 
    /// True indicates success. False indicates failure;
    /// in such case you should call Marshal.GetLastWin32Error() to get more information.
    /// </returns>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetEvent(
      IntPtr handle);

    /// <summary> Sets the specified event object to the non-signaled state. </summary>
    /// <param name="handle" type="IntPtr"> A handle to the event object. The CreateEvent or OpenEvent
    ///  function returns this handle. </param>
    /// <returns>
    /// True indicates success. False indicates failure;
    /// in such case you should call Marshal.GetLastWin32Error() to get more information.
    /// </returns>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ResetEvent(
      IntPtr handle);

    /// <summary>
    /// Sets the specified event object to the signaled state and then resets it to the nonsignaled state
    /// after releasing the appropriate number of waiting threads.
    /// </summary>
    /// <param name="handle" type="IntPtr"> A handle to the event object. 
    /// The CreateEvent or OpenEvent function returns this handle. </param>
    /// <returns> 
    /// True indicates success. False indicates failure;
    /// in such case you should call Marshal.GetLastWin32Error() to get more information.
    /// </returns>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PulseEvent(
      IntPtr handle);

    /// <summary>
    /// Waits until the specified object is in the signaled state or the time-out interval elapses.
    /// </summary>
    /// <param name="hHandle" type="IntPtr"> A valid handle to an open object. For a list of the object
    ///  types whose handles can be specified, see the following Remarks section.
    ///  
    ///  If this handle is closed while the wait is still pending, the function's behavior is undefined.
    ///  
    ///  The handle must have the SYNCHRONIZE access right. </param>
    /// <param name="dwMilliseconds" type="Int32"> The time-out interval, in milliseconds. If a nonzero
    ///  value is specified, the function waits until the object is signaled or the interval elapses. If
    ///  dwMilliseconds is zero, the function does not enter a wait state if the object is not signaled; it
    ///  always returns immediately. If dwMilliseconds is INFINITE, the function will return only when the
    ///  object is signaled. </param>
    /// 
    /// <returns>
    /// If the function succeeds, the return value indicates the event that caused the function to return. It
    /// can be one of the following values.
    /// <list type="bullet">
    /// <item><b>WAIT_ABANDONED</b><br/>The specified object is a mutex object that was not released by the
    /// thread that owned the mutex object before the owning thread terminated. Ownership of the mutex object
    /// is granted to the calling thread and the mutex state is set to non-signaled.</item>
    /// <item><b>WAIT_OBJECT_0</b><br/>The state of the specified object is signaled.</item>
    /// <item><b>WAIT_TIMEOUT</b><br/>The time-out interval elapsed, and the object's state is non-signaled.
    /// </item>
    /// <item><b>WAIT_FAILED</b><br/>The function has failed. To get extended error information, call
    /// Marshal.GetLastWin32Error. </item>
    /// </list>
    /// </returns>
    [DllImport("kernel32", SetLastError = true)]
    public static extern Int32 WaitForSingleObject(
      IntPtr hHandle,
      Int32 dwMilliseconds);

    /// <summary>
    /// Waits until one or all of the specified objects are in the signaled state or the time-out interval
    /// elapses.
    /// </summary>
    /// <param name="nCount" type="int"> The number of object handles in the array pointed to by lpHandles.
    ///  The maximum number of object handles is MAXIMUM_WAIT_OBJECTS. This parameter cannot be zero. </param>
    /// <param name="lpHandles" type="IntPtr[]"> An array of object handles. For a list of the object types
    ///  whose handles can be specified, see the following Remarks section. The array can contain handles to
    ///  objects of different types. It may not contain multiple copies of the same handle. If one of these
    ///  handles is closed while the wait is still pending, the function's behavior is undefined. </param>
    /// <param name="bWaitAll" type="bool"> If this parameter is true, the function returns when the state of
    ///  all objects in the lpHandles array is signaled. <br/>
    ///  If false, the function returns when the state of any one of the objects is set to signaled. In the
    ///  latter case, the return value indicates the object whose state caused the function to return. </param>
    /// <param name="dwMilliseconds"> The time-out interval, in milliseconds. If a nonzero value is specified,
    ///  the function waits until the specified objects are signaled or the interval elapses. If
    ///  dwMilliseconds is zero, the function does not enter a wait state if the specified objects are not
    ///  signaled; it always returns immediately. If dwMilliseconds is INFINITE, the function will return
    ///  only when the specified objects are signaled. </param>
    /// <returns>
    /// If the function succeeds, the return value indicates the event that caused the function to return. It
    /// can be one of the following values. (Note that WAIT_OBJECT_0 is defined as 0 and WAIT_ABANDONED_0 is
    /// defined as 0x00000080L.)
    /// 
    /// <list type="bullet">
    /// <item><b>WAIT_OBJECT_0 to (WAIT_OBJECT_0 + nCount– 1)</b><br/>
    /// If bWaitAll is true, the return value indicates that the state of all specified objects is
    /// signaled.<br/>
    /// If bWaitAll is false, the return value minus WAIT_OBJECT_0 indicates the lpHandles array index of the
    /// object that satisfied the wait. If more than one object became signaled during the call, this is the
    /// array index of the signaled object with the smallest index value of all the signaled objects.
    /// </item>
    /// <item><b>WAIT_ABANDONED_0 to (WAIT_ABANDONED_0 + nCount– 1)</b><br/>
    /// If bWaitAll is TRUE, the return value indicates that the state of all specified objects is signaled
    /// and at least one of the objects is an abandoned mutex object. <br/>
    /// If bWaitAll is FALSE, the return value minus WAIT_ABANDONED_0 indicates the lpHandles array index of
    /// an abandoned mutex object that satisfied the wait. Ownership of the mutex object is granted to the
    /// calling thread, and the mutex is set to non-signaled.<br/>
    /// </item>
    /// <item><b>WAIT_TIMEOUT 0x00000102L</b><br/> The time-out interval elapsed and the conditions specified
    /// by the bWaitAll parameter are not satisfied.
    /// </item>
    /// <item><b>WAIT_FAILED (DWORD)0xFFFFFFFF</b><br/>
    /// The function has failed. To get extended error information, call Marshal.GetLastWin32Error()..
    /// </item>
    /// </list>
    /// </returns>
    [DllImport("kernel32", SetLastError = true)]
    public static extern int WaitForMultipleObjects(
      int nCount,
      IntPtr[] lpHandles,
      [MarshalAs(UnmanagedType.Bool)] bool bWaitAll,
      int dwMilliseconds);

    /// <summary>
    /// Retrieves the path of the system directory. The system directory contains system files such as
    /// dynamic-link libraries and drivers.
    /// </summary>
    /// <remarks>
    /// This function is provided primarily for compatibility. Applications should store code in the Program
    /// Files folder and persistent data in the Application Data folder in the user's profile.
    /// </remarks>
    /// <param name="lpBuffer"> A buffer that receives the fully qualified path of system directory. A
    ///  pointer to the buffer to receive the path. This path does not end with a backslash unless the
    ///  system directory is the root directory. For example, if the system directory is named Windows\
    ///  System32 on drive C, the path of the system directory retrieved by this function is C:\Windows\
    ///  System32. </param>
    /// <param name="uMaxSize" type="uint"> The maximum size of the buffer, in characters. </param>
    /// <returns> The system directory. </returns>
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern uint GetSystemDirectory(
      [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpBuffer,
      uint uMaxSize);

    /// <summary>
    /// Enumerates the locales that are either installed on or supported by an operating system.
    /// </summary>
    /// <param name="lpLocaleEnumProc" type="SystemLocalesDelegate"> An application-defined callback
    ///  function. For more information see <see cref="SystemLocalesDelegate"/>. </param>
    /// <param name="dwFlags"> Flags specifying the locale identifiers to enumerate. The flags can be used
    ///  singly or combined using a binary OR. If the application specifies 0 for this parameter, the
    ///  function behaves as for LCID_SUPPORTED.
    ///  <list type="bullet">
    ///  <item><b>LCID_INSTALLED</b><br/>
    ///  Enumerate only installed locale identifiers. This value cannot be used with LCID_SUPPORTED.
    ///  </item>
    ///  <item><b>LCID_SUPPORTED</b><br/>
    ///  Enumerate all supported locale identifiers. This value cannot be used with LCID_INSTALLED.
    ///  </item>
    ///  <item><b>LCID_ALTERNATE_SORTS</b><br/>
    ///  Enumerate only the alternate sort locale identifiers. If this value is used with either
    ///  LCID_INSTALLED or LCID_SUPPORTED, the installed or supported locales are retrieved, as well as the
    ///  alternate sort locale identifiers.
    ///  </item>
    ///  </list> </param>
    /// <returns>
    /// Returns a nonzero value if successful, or 0 otherwise. 
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", SetLastError = true)]
    public static extern int EnumSystemLocales(
      SystemLocalesDelegate lpLocaleEnumProc,
      uint dwFlags);

    /// <summary>
    /// Sends a control code directly to a specified device driver, causing the corresponding device to
    /// perform the corresponding operation.
    /// </summary>
    /// 
    /// <param name="hDevice"> [in] A handle to the device on which the operation is to be performed.
    ///  The device is typically a volume, directory, file, or stream. To retrieve a device handle,
    ///  use the <see cref="CreateFile"/> function. For more information, see Remarks. </param>
    /// 
    /// <param name="dwIoControlCode"> [in] The control code for the operation. This value identifies
    ///  the specific operation to be performed and the type of device on which to perform it. </param>
    /// 
    /// <param name="lpInBuffer"> [in, optional] A pointer to the input buffer that contains the data
    ///  required to perform the operation. The format of this data depends on the value of the
    ///  dwIoControlCode parameter. This parameter can be IntPtr.Zero if dwIoControlCode specifies an
    ///  operation that does not require input data. </param>
    /// 
    /// <param name="nInBufferSize"> [in] The size of the input buffer, in bytes. </param>
    /// 
    /// <param name="lpOutBuffer"> [out, optional] A pointer to the output buffer that is to receive
    ///  the data returned by the operation. The format of this data depends on the value of the
    ///  dwIoControlCode parameter. This parameter can be IntPtr.Zeroif dwIoControlCode specifies
    ///  an operation that does not return data. </param>
    /// 
    /// <param name="nOutBufferSize"> [in] The size of the output buffer, in bytes. </param>
    /// 
    /// <param name="lpBytesReturned"> [out, optional] A reference to a variable that receives the size
    ///  of the data stored in the output buffer, in bytes.
    ///  If the output buffer is too small to receive any data, the call fails, 
    ///  Marshal.GetLastWin32Error() returns ERROR_INSUFFICIENT_BUFFER, and lpBytesReturned is zero.
    ///
    /// If the output buffer is too small to hold all of the data but can hold some entries, 
    /// some drivers will return as much data as fits. In this case, the call fails, 
    /// Marshal.GetLastWin32Error() returns ERROR_MORE_DATA, and lpBytesReturned indicates the amount 
    /// of data received. Your application should call DeviceIoControl again with the same operation, 
    /// specifying a new starting point.
    ///
    /// If lpOverlapped is IntPtr.Zero, lpBytesReturned cannot be NULL. Even when an operation returns 
    /// no output data and lpOutBuffer is NULL, DeviceIoControl makes use of lpBytesReturned. 
    /// After such an operation, the value of lpBytesReturned is meaningless.
    ///
    /// If lpOverlapped is not IntPtr.Zero, lpBytesReturned can be NULL. 
    /// If this parameter is not NULL and the operation returns data, lpBytesReturned is meaningless 
    /// until the overlapped operation has completed. 
    /// To retrieve the number of bytes returned, call GetOverlappedResult. 
    /// If hDevice is associated with an I/O completion port, you can retrieve the number of bytes returned 
    /// by calling GetQueuedCompletionStatus.
    ///  </param>
    /// 
    /// <param name="lpOverlapped"> [in, out, optional] A pointer to an OVERLAPPED structure.
    ///  
    ///  If hDevice was opened without specifying FILE_FLAG_OVERLAPPED, lpOverlapped is ignored.
    ///  
    ///  If hDevice was opened with the FILE_FLAG_OVERLAPPED flag, the operation is performed as an
    ///  overlapped (asynchronous) operation. In this case, lpOverlapped must point to a valid OVERLAPPED
    ///  structure that contains a handle to an event object. Otherwise, the function fails in
    ///  unpredictable ways.
    ///  
    ///  For overlapped operations, DeviceIoControl returns immediately, and the event object is signaled
    ///  when the operation has been completed. Otherwise, the function does not return until the
    ///  operation has been completed or an error occurs.
    ///  </param>
    /// <returns> If the operation completes successfully, the return value is nonzero.
    /// If the operation fails or is pending, the return value is zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    /// 
    /// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/aa363216(v=vs.85).aspx">
    /// DeviceIoControl function MSDN help
    /// </seealso>
    [DllImport("kernel32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeviceIoControl(
      IntPtr hDevice,
      Int32 dwIoControlCode,
      IntPtr lpInBuffer,
      Int32 nInBufferSize,
      byte[] lpOutBuffer,
      Int32 nOutBufferSize,
      ref Int32 lpBytesReturned,
      IntPtr lpOverlapped);

    /// <summary> Copies the memory. </summary>
    /// <param name="destination" type="IntPtr"> A pointer to the starting address of the copied block's
    ///  destination. </param>
    /// <param name="source" type="IntPtr"> A pointer to the starting address of the block of memory to copy.
    /// </param>
    /// <param name="length" type="uint"> The length of memory block to process, in bytes. </param>
    [DllImport("kernel32", EntryPoint = "RtlMoveMemory", SetLastError = false)]
    public static extern void CopyMemory(
      IntPtr destination,
      IntPtr source,
      uint length);

    /// <summary> The routine fills a block of memory with the specified fill value. </summary>
    /// <param name="destination" type="IntPtr"> A pointer to the block of memory to be filled. </param>
    /// <param name="length" type="uint"> The length of memory block to process. </param>
    /// <param name="fill" type="byte"> The byte used as contents for filling. </param>
    [DllImport("kernel32", EntryPoint = "RtlFillMemory", SetLastError = false)]
    public static extern void memset(
      IntPtr destination,
      uint length,
      byte fill);

    /// <summary> Returns true if native debugger is attached, false otherwise. </summary>
    ///
    /// <remarks> The IsDebuggerPresent call will probably return False if only a managed debugger is attached,
    /// whereas <see cref="System.Diagnostics.Debugger.IsAttached"/>will return True only if a managed
    /// debugger is attached.<br/>
    /// 
    /// For more info about this subject, see for instance
    /// <a href="http://social.msdn.microsoft.com/forums/en-US/vbgeneral/thread/a3380003-2a99-48e9-8eb5-1676d918b57c">
    /// IsDebuggerPresent doesn't work under .NET?</a>.  <br/> </remarks>
    ///
    /// <returns> true if native debugger present, false if not. </returns>
    [DllImport("kernel32", CharSet = CharSet.Auto, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsDebuggerPresent();

    /// <summary> Gets DLL directory, i.e.  retrieves the application-specific portion of the search path
    ///  used to locate DLLs for the application.
    ///  It is assumed this application-specific portion has been set previously by <see cref="SetDllDirectory"/>.
    /// </summary>
    ///
    /// <param name="bufsize">  The size of the buffer. </param>
    /// <param name="lpBuffer"> The buffer. </param>
    ///
    /// <returns> If the function succeeds, the return value is the length of the string copied to lpBuffer, 
    /// in characters, not including the terminating null character. 
    /// If the return value is greater than nBufferLength, it specifies the size of the buffer required for the path.
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int GetDllDirectory(
      int bufsize,
      [MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder lpBuffer);

    /// <summary> Adds a directory to the search path used to locate DLLs for the application. . </summary>
    ///
    /// <param name="lpPathName"> The directory to be added to the search path. </param>
    ///
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetDllDirectory(
      [MarshalAs(UnmanagedType.LPWStr)] string lpPathName);

    /// <summary> Loads the specified module into the address space of the calling process. 
    ///  The specified module may cause other modules to be loaded.</summary>
    ///
    /// <param name="libraryName">  The name of the module. This can be either a library module (a .dll file) 
    ///  or an executable module (an .exe file). 
    ///  The name specified is the file name of the module and is not related to the name stored in the library module 
    ///  itself, as specified by the LIBRARY keyword in the module-definition (.def) file.
    ///  </param>
    ///
    /// <returns> If the function succeeds, the return value is a handle to the module.
    /// If the function fails, the return value is IntPtr.Zero.
    /// To get extended error information, call Marshal.GetLastWin32Error().
    /// </returns>
    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false,
      ThrowOnUnmappableChar = true)]
    public static extern IntPtr LoadLibrary(
      [MarshalAs(UnmanagedType.LPTStr)] string libraryName);

    /// <summary> Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count. 
    ///  When the reference count reaches zero, the module is unloaded from the address space of the calling process 
    ///  and the handle is no longer valid.
    /// </summary>
    ///
    /// <param name="hModule">  A handle to the loaded library module. </param>
    ///
    /// <returns> true if it succeeds, false if it fails. </returns>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FreeLibrary(IntPtr hModule);

    /// <summary>
    /// Adds or removes an application-defined <see cref="HandlerRoutine"/> function from the list of handler
    /// functions for the calling process.
    /// </summary>
    ///
    /// <remarks>
    /// This function provides a similar notification for console application and services that
    /// WM_QUERYENDSESSION provides for graphical applications with a message pump. You could also use this
    /// function from a graphical application, but there is no guarantee it would arrive before the notification
    /// from WM_QUERYENDSESSION.
    /// 
    /// Note unlike when handling CTRL+C or CTRL+BREAK events, your process does not get the opportunity to cancel 
    /// the close, log off, or shutdown.
    /// </remarks>
    ///
    /// <param name="handler"> The handler application-defined HandlerRoutine function to be added or removed. </param>
    /// <param name="add">     If this parameter is true, the handler is added; if it is false, the handler is
    /// removed. </param>
    ///
    /// <returns>   True if it succeeds, false if it fails. </returns>
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetConsoleCtrlHandler(
        HandlerRoutine handler,
        [MarshalAs(UnmanagedType.Bool)] bool add);

    /// <summary>
    /// Retrieves the handle to the console window associated with the calling process.
    /// </summary>
    /// 
    /// <remarks>
    /// This function returns a handle to the console window for the calling process if it exists. The console
    /// window is created when the process starts, and can be accessed for operations like modifying window 
    /// appearance or interacting with the console programmatically. If the process is not associated with a console
    /// window, the return value is IntPtr.Zero.
    /// </remarks>
    /// 
    /// <returns> A handle to the console window, or IntPtr.Zero if the process does not have a console window. </returns>
    [DllImport("kernel32")]
    public static extern IntPtr GetConsoleWindow();

    #endregion //  External functions

    #region Methods

    /// <summary> Enumerates the system locales. </summary>
    /// <exception cref="ArgumentException"> Thrown when the argument <paramref name="flag"/> has
    ///  unsupported value. </exception>
    /// <param name="flag"> Either <see cref="Kernel32.LCID_INSTALLED"/> or
    ///  <see cref="Kernel32.LCID_SUPPORTED"/> </param>
    /// <returns> Collection of found locales, with each value converted to string. </returns>
    /// <seealso cref="SystemLocalesDelegate"/>
    /// <seealso cref="EnumSystemLocales"/>
    public static IEnumerable<string> EnumLocales(uint flag)
    {
        List<string> list = [];

        switch (flag)
        {
            case Kernel32.LCID_INSTALLED:
            case Kernel32.LCID_SUPPORTED:
                break;
            default:
                throw new ArgumentException(
                  string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "Invalid argument flag value ({0})", flag),
                  nameof(flag));
        }

        Kernel32.EnumSystemLocales(
          delegate (StringBuilder text) { list.Add(text.ToString()); return 1; },
          flag);

        return list;
    }

    /// <summary> Gets application file name, including its full path and the file extension. </summary>
    /// <returns> The application file name. </returns>
    public static string GetApplicationFileName()
    {
        StringBuilder sb = new(260);
        string result = string.Empty;

        if (0 != GetModuleFileName(IntPtr.Zero, sb, sb.Capacity))
        {
            result = sb.ToString();
        }
        return result;
    }

    /// <summary> Gets memory information in a form of string. 
    /// Fills-in the auxiliary <see cref="MEMORYSTATUS "/> structure, 
    /// and lists all its fields in the returned string.
    /// </summary>
    /// <returns> The memory information in a textual form. </returns>
    public static string GetMemoryInfo()
    {
        MEMORYSTATUS memStatus = new();
        GlobalMemoryStatus(ref memStatus);

        // Use a StringBuilder for the message box string.
        StringBuilder sbInfo = new();
        CultureInfo cult = CultureInfo.CurrentCulture;

        sbInfo.AppendFormat(cult, "Memory Load: {0} ", memStatus.dwMemoryLoad);
        sbInfo.AppendFormat(cult, "Total Physical: {0} ", memStatus.ullTotalPhys);
        sbInfo.AppendFormat(cult, "Avail Physical: {0} ", memStatus.ullAvailPhys);
        sbInfo.AppendFormat(cult, "Total Page File: {0} ", memStatus.ullTotalPageFile);
        sbInfo.AppendFormat(cult, "Avail Page File: {0} ", memStatus.ullAvailPageFile);
        sbInfo.AppendFormat(cult, "Total Virtual: {0} ", memStatus.ullTotalVirtual);
        sbInfo.AppendFormat(cult, "Avail Virtual: {0} ", memStatus.ullAvailVirtual);

        return sbInfo.ToString();
    }

    /// <summary>
    /// Returns the directory of the module given by hModule handle.
    /// For the hModule == IntPtr.Zero the actual module is the running executable.
    /// </summary>
    /// <param name="hModule">The handle of the module the caller is interested in.</param>
    /// <returns>The string containing the full path of found directory</returns>
    public static string GetModuleDirectory(IntPtr hModule)
    {
        string result = string.Empty;

        // If hModule is IntPtr.Zero, GetModuleFileName retrieves the path of the executable file of the current process.
        /* no ! if (IntPtr.Zero != hModule)  */
        {
            int nDex;
            string strTmp;
            StringBuilder sb = new(260);

            if (0 != GetModuleFileName(hModule, sb, sb.Capacity))
            {
                strTmp = sb.ToString();
                if (0 < (nDex = strTmp.LastIndexOf(Path.DirectorySeparatorChar)))
                {
                    result = strTmp.Substring(0, nDex);
                }
            }
        }

        return result;
    }
    #endregion //  Methods

    #region Constructor(s)

    /// <summary>
    /// Static constructor, performs PrelinkAll check
    /// </summary>
    static Kernel32()
    {
        try
        {
            Marshal.PrelinkAll(typeof(Kernel32));
        }
#if DEBUG
        catch (Exception ex)
        {
            string strMsg = $"PrelinkAll failed for '{typeof(Kernel32)}', with exception: '{ex.Message}', stack trace '{ex.StackTrace}'";
            Debug.Fail(strMsg);
            throw;
        }
#else
        catch (Exception)
        {
            throw;
        }
#endif // DEBUG
    }
    #endregion // Constructor(s)
}


#pragma warning restore SYSLIB1054
#pragma warning restore CA1806
#pragma warning restore CA1401
#pragma warning restore CA1419
#pragma warning restore IDE0057

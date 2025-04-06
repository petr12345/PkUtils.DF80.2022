//     Copyright (c) Microsoft Corporation.  All rights reserved.


// Ignore Spelling: Utils
//
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using PK.PkUtils.WinApi;

namespace Microsoft.Experimental.IO;

internal static class LongPathCommon
{
    #region Internal Interface

    /// <summary>
    /// Normalize the search pattern. Null or an empty string, or string consisting of juts one dot, are
    /// replaced by "*".
    /// </summary>
    /// <param name="searchPattern"> A pattern specifying the file-search. </param>
    /// <returns> A  resulting search patter. </returns>
    internal static string NormalizeSearchPattern(string searchPattern)
    {
        if (string.IsNullOrEmpty(searchPattern) || searchPattern == ".")
            return "*";

        return searchPattern;
    }

    /// <summary> Normalizes path (can be longer than MAX_PATH) and adds \\?\ long path prefix. </summary>
    /// <remarks> Calls overloaded method <see cref="NormalizeLongPath(string, string)"/>.</remarks>
    /// 
    /// <exception cref="ArgumentNullException"> Passed when one or more required arguments are null. </exception>
    /// <exception cref="ArgumentException"> Passed when one or more arguments have unsupported or illegal
    ///  values. </exception>
    ///                                      
    /// <param name="path"> A path to the file or directory. </param>
    /// <returns> A normalized path. </returns>
    /// <see cref="RemoveLongPathPrefix"/>
    internal static string NormalizeLongPath(string path)
    {
        return NormalizeLongPath(path, "path");
    }

    /// <summary> Normalizes path (can be longer than MAX_PATH) and adds \\?\ long path prefix. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal
    ///  values. </exception>
    /// 
    /// <param name="path"> A path to the file or directory. </param>
    /// <param name="parameterName"> Name of the parameter <paramref name="path"/>, that will be used in
    ///  thrown exceptions. </param>
    /// <returns> A normalized path. </returns>
    /// <seealso cref="RemoveLongPathPrefix"/>
    internal static string NormalizeLongPath(string path, string parameterName)
    {
        if (path == null)
        {
            throw new ArgumentNullException(parameterName);
        }
        if (path.Length == 0)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "'{0}' cannot be an empty string.",
              parameterName), parameterName);
        }

        StringBuilder buffer = new(path.Length + 1); // Add 1 for NULL
        uint length = Kernel32.GetFullPathName(path, (uint)buffer.Capacity, buffer, IntPtr.Zero);
        if (length > buffer.Capacity)
        {
            // Resulting path longer than our buffer, so increase it
            buffer.Capacity = (int)length;
            length = Kernel32.GetFullPathName(path, length, buffer, IntPtr.Zero);
        }

        if (length == 0)
        {
            throw GetExceptionFromLastWin32Error(parameterName);
        }

        if (length > Kernel32.MAX_LONG_PATH)
        {
            throw GetExceptionFromWin32Error(Kernel32.ERROR_FILENAME_EXCED_RANGE, parameterName);
        }

        return AddLongPathPrefix(buffer.ToString());
    }

    /// <summary> Removes the long path prefix from <paramref name="normalizedPath"/>. </summary>
    /// <remarks>
    /// It is assumed that the input path <paramref name="normalizedPath"/> begins with long path prefix;
    /// it's up to the caller to guarantee that.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> Thrown when argument <paramref name="normalizedPath"/> is
    ///  null. </exception>
    /// <exception cref="ArgumentException"> Thrown when argument <paramref name="normalizedPath"/>  has
    ///  unsupported or illegal values. </exception>
    /// <param name="normalizedPath"> Full pathname of the normalized file. </param>
    /// <returns> An original path, without long path prefix. </returns>
    /// <seealso cref="NormalizeLongPath(string)"/>
    internal static string RemoveLongPathPrefix(string normalizedPath)
    {
        ArgumentNullException.ThrowIfNull(normalizedPath);
        if (normalizedPath.Length == 0)
            throw new ArgumentException("normalizedPath cannot be an empty string.", nameof(normalizedPath));
        if (!normalizedPath.StartsWith(LongPathPrefix, StringComparison.Ordinal))
            throw new ArgumentException($"normalizedPath does not begin with {LongPathPrefix}", nameof(normalizedPath));

        // return normalizedPath.Substring(LongPathPrefix.Length);
        return normalizedPath[LongPathPrefix.Length..];
    }

    /// <summary> Determines whether object of given path <paramref name="path"/> exists. </summary>
    /// <param name="path"> A path to the file or directory. </param>
    /// <param name="isDirectory"> [out] True if <paramref name="path"/> represents the directory. </param>
    /// <returns> true if such file or directory exists, false if not. </returns>
    internal static bool Exists(string path, out bool isDirectory)
    {
        if (TryNormalizeLongPath(path, out string normalizedPath))
        {
            int errorCode = TryGetFileAttributes(normalizedPath, out FileAttributes attributes);

            if (errorCode == 0)
            {
                isDirectory = LongPathDirectory.IsDirectory(attributes);
                return true;
            }
        }

        isDirectory = false;
        return false;
    }

    /// <summary>
    /// Tries to get attributes of the file or directory represented by <paramref name="normalizedPath"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> Thrown when <paramref name="normalizedPath"/> actually is not
    ///  a normalized path. </exception>
    /// <param name="normalizedPath"> A normalized path of file or directory. </param>
    /// <param name="attributes"> [out] The file or directory attributes. </param>
    /// <returns> Zero if succeeds, or non-zero error code ( Marshal.GetLastWin32Error ) if fails. </returns>
    /// <seealso cref="TryGetDirectoryAttributes"/>
    internal static int TryGetFileAttributes(string normalizedPath, out FileAttributes attributes)
    {
        CheckNormalizedPath(normalizedPath);
        // NOTE: Don't be tempted to use FindFirstFile here, it does not work with root directories

        int result = 0;
        attributes = Kernel32.GetFileAttributes(normalizedPath);
        if ((int)attributes == Kernel32.INVALID_FILE_ATTRIBUTES)
            result = Marshal.GetLastWin32Error();

        return result;
    }

    /// Tries to get attributes of the directory represented by <paramref name="normalizedPath"/>.
    /// <remarks> Returns ERROR_DIRECTORY error code in case the argument <paramref name="normalizedPath"/>
    /// represents a file, not a directory.</remarks>
    /// 
    /// <exception cref="ArgumentException"> Passed when <paramref name="normalizedPath"/> actually is not
    ///  a normalized path. </exception>
    /// <param name="normalizedPath"> A normalized path of a directory. </param>
    /// <param name="attributes"> [out] The attributes of the directory. </param>
    /// <returns> Zero if succeeds, or non-zero error code ( Marshal.GetLastWin32Error ) if fails. </returns>
    /// 
    /// <see cref="LongPathDirectory.IsDirectory"/>
    /// <seealso cref="TryGetFileAttributes"/>
    internal static int TryGetDirectoryAttributes(string normalizedPath, out FileAttributes attributes)
    {
        CheckNormalizedPath(normalizedPath);
        int errorCode = TryGetFileAttributes(normalizedPath, out attributes);
        if ((errorCode == 0) && (!LongPathDirectory.IsDirectory(attributes)))
            errorCode = Kernel32.ERROR_DIRECTORY;

        return errorCode;
    }

    /// <summary>
    /// Attempts to set attributes <paramref name="attributes"/> to the file represented by
    /// <paramref name="normalizedPath"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> Passed when <paramref name="normalizedPath"/> is not
    ///  a normalized path. </exception>
    /// 
    /// <param name="normalizedPath"> A normalized path of file or directory. </param>
    /// <param name="attributes"> [int] The file or directory attributes. </param>
    /// 
    /// <returns> Zero if succeeds, or non-zero error code ( Marshal.GetLastWin32Error ) if fails. </returns>
    internal static int TrySetFileAttributes(string normalizedPath, FileAttributes attributes)
    {
        CheckNormalizedPath(normalizedPath);
        if (!Kernel32.SetFileAttributes(normalizedPath, attributes))
            return Marshal.GetLastWin32Error();

        return 0;
    }

    /// <summary>
    /// Attempts to set attributes <paramref name="attributes"/> to the directory represented by
    /// <paramref name="normalizedPath"/>.
    /// </summary>
    /// <exception cref="ArgumentException"> Passed when <paramref name="normalizedPath"/> is not
    ///  a normalized path. </exception>
    /// 
    /// <param name="normalizedPath"> A normalized path of a directory. </param>
    /// <param name="attributes"> [int] The file or directory attributes. </param>
    /// 
    /// <returns> Zero if succeeds, or non-zero error code ( Marshal.GetLastWin32Error ) if fails. </returns>
    internal static int TrySetDirectoryAttributes(string normalizedPath, FileAttributes attributes)
    {
        CheckNormalizedPath(normalizedPath);
        if (!Kernel32.SetFileAttributes(normalizedPath, attributes))
            return Marshal.GetLastWin32Error();

        return 0;
    }

    /// <summary> Returns a new Exception created from last win32 error ( Marshal.GetLastWin32Error ). 
    /// The actual type of exception is determined by that error code.
    /// </summary>
    /// <returns> The created exception. </returns>
    /// <seealso cref="GetExceptionFromWin32Error(int, string)"/>
    internal static Exception GetExceptionFromLastWin32Error()
    {
        return GetExceptionFromLastWin32Error("path");
    }

    /// <summary>
    /// Returns a new Exception created from last win32 error ( Marshal.GetLastWin32Error ). The actual
    /// type of exception is determined by that error code.
    /// </summary>
    /// <param name="parameterName"> Name of the parameter that will be used in created exceptions. </param>
    /// <returns> The created exception. </returns>
    /// <seealso cref="GetExceptionFromWin32Error(int, string)"/>
    internal static Exception GetExceptionFromLastWin32Error(string parameterName)
    {
        return GetExceptionFromWin32Error(Marshal.GetLastWin32Error(), parameterName);
    }

    /// <summary>
    /// Returns a new Exception created from win32 error specified by <paramref name="errorCode"/>. The
    /// actual type of exception is determined by that error code.
    /// </summary>
    /// <param name="errorCode"> The error code. </param>
    /// <returns> The created exception. </returns>
    /// <seealso cref="GetExceptionFromWin32Error(int, string)"/>
    internal static Exception GetExceptionFromWin32Error(int errorCode)
    {
        return GetExceptionFromWin32Error(errorCode, "path");
    }

    /// <summary>
    /// Returns a new Exception created from win32 error specified by <paramref name="errorCode"/>.
    /// The actual type of exception is determined by that error code.
    /// </summary>
    /// <param name="errorCode"> The error code. </param>
    /// <param name="parameterName"> Name of the parameter that will be used in created exceptions. </param>
    /// <returns> The created exception. </returns>
    internal static Exception GetExceptionFromWin32Error(int errorCode, string parameterName)
    {
        string message = GetMessageFromErrorCode(errorCode);

        return errorCode switch
        {
            Kernel32.ERROR_FILE_NOT_FOUND => new FileNotFoundException(message),
            Kernel32.ERROR_PATH_NOT_FOUND => new DirectoryNotFoundException(message),
            Kernel32.ERROR_ACCESS_DENIED => new UnauthorizedAccessException(message),
            Kernel32.ERROR_FILENAME_EXCED_RANGE => new PathTooLongException(message),
            Kernel32.ERROR_INVALID_DRIVE => new DriveNotFoundException(message),
            Kernel32.ERROR_OPERATION_ABORTED => new OperationCanceledException(message),
            Kernel32.ERROR_INVALID_NAME => new ArgumentException(message, parameterName),
            _ => new IOException(message, PK.PkUtils.WinApi.Win32.MakeHRFromErrorCode(errorCode)),
        };
    }
    #endregion // Internal Interface

    #region Private Members
    #region Private Methods

    /// <summary> Query if <paramref name="normalizedPath"/> is truly normalized path. </summary>
    /// <remarks>Works purely by string-level comparison, does not examine any file or folder existence.</remarks>
    /// 
    /// <param name="normalizedPath"> Path to the file or directory. </param>
    /// <returns> true if <paramref name="normalizedPath"/> is normalized path, false if not. </returns>
    /// <seealso cref="CheckNormalizedPath"/>
    private static bool IsNormalizedPath(string normalizedPath)
    {
        if (string.IsNullOrEmpty(normalizedPath))
            return false;
        if (!normalizedPath.StartsWith(LongPathPrefix, StringComparison.Ordinal))
            return false;

        return true;
    }

    /// <summary> Checks whether the <paramref name="normalizedPath"/> is truly normalized path,
    /// throws ArgumentException in case it is not. </summary>
    /// <exception cref="ArgumentException"> Thrown when the argument <paramref name="normalizedPath"/> is
    ///  not a normalized path. </exception>
    /// <param name="normalizedPath"> Path to the file or directory. </param>
    /// <seealso cref="IsNormalizedPath"/>
    private static void CheckNormalizedPath(string normalizedPath)
    {
        if (!IsNormalizedPath(normalizedPath))
        {
            throw new ArgumentException(
                $"The value '{normalizedPath}' is not normalized path",
                nameof(normalizedPath));
        }
    }

    /// <summary> Attempts to normalize long path specified by <paramref name="path"/> argument,
    ///  handles possible exceptions and returns false in case of any error. </summary>
    /// 
    /// <param name="path"> [in] A path to the file or directory. </param>
    /// <param name="result"> [out] The resulting normalized path, or null if there was an error. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    /// <see cref="NormalizeLongPath(string)"/>
    private static bool TryNormalizeLongPath(string path, out string result)
    {
        bool bRes = false;

        try
        {
            result = NormalizeLongPath(path);
            bRes = true;
        }
        catch (ArgumentException)
        {
            result = null;
        }
        catch (PathTooLongException)
        {
            result = null;
        }

        return bRes;
    }

    /// <summary>
    /// Prepends the <paramref name="path"/> with long path prefix and returns the result.
    /// </summary>
    /// <param name="path"> A path to the file or directory. </param>
    /// <returns> A string. </returns>
    private static string AddLongPathPrefix(string path)
    {
        Debug.Assert(!IsNormalizedPath(path));
        return LongPathPrefix + path;
    }

    /// <summary>
    /// Retrieve the message text for a system-defined error ( Win32 error code )
    /// <paramref name="errorCode"/>.
    /// </summary>
    /// <param name="errorCode"> The Win32 error code. </param>
    /// <returns> The message related to the <paramref name="errorCode"/>. </returns>
    private static string GetMessageFromErrorCode(int errorCode)
    {
        StringBuilder buffer = new(512);
        int bufferLength = Kernel32.FormatMessage(
          Kernel32.FormatMsg.FORMAT_MESSAGE_IGNORE_INSERTS |
          Kernel32.FormatMsg.FORMAT_MESSAGE_FROM_SYSTEM |
          Kernel32.FormatMsg.FORMAT_MESSAGE_ARGUMENT_ARRAY,
          IntPtr.Zero, errorCode, 0, buffer, buffer.Capacity, IntPtr.Zero);

        Debug.Assert(bufferLength != 0);

        return buffer.ToString();
    }
    #endregion // Private Methods

    #region Private Fields

    /// <summary>
    /// The long path prefix.  If you prefix the file name with "\\?\" and call the Unicode versions of the
    /// Windows APIs, then you can use file names up to 32K characters in length.
    /// </summary>
    /// <seealso href="http://blogs.msdn.com/b/bclteam/archive/2007/02/13/long-paths-in-net-part-1-of-3-kim-hamilton.aspx">
    /// Long Paths in .NET [Kim Hamilton]</seealso>
    private const string LongPathPrefix = @"\\?\";
    #endregion // Private Fields
    #endregion // Private Members
}

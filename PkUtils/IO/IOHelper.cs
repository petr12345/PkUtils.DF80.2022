// Ignore Spelling: fi, Dest, Utils
//
using System;
using System.IO;
using Microsoft.Experimental.IO;

namespace PK.PkUtils.IO;

/// <summary>
/// A helper class performing few IO operations.
/// </summary>
[CLSCompliant(true)]
public static class IOHelper
{
    #region Methods

    /// <summary> Returns true if the file or directory <paramref name="fs"/> has read-only attribute. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when a supplied argument is null. </exception>
    /// <param name="fs"> The info about file or directory being examined. </param>
    ///
    /// <returns> true if read only, false if not. </returns>
    public static bool IsReadOnly(FileSystemInfo fs)
    {
        ArgumentNullException.ThrowIfNull(fs);

        bool isReadOnly = ((fs.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
        return isReadOnly;
    }

    /// <summary> Returns true if the file or directory <paramref name="fs"/> 
    ///           exists and has read-only attribute. </summary>
    ///
    /// <param name="fs"> The info about file or directory being examined. </param>
    ///
    /// <returns> true if exists and is read only, false if not. </returns>
    public static bool ExistsAsReadOnly(FileSystemInfo fs)
    {
        ArgumentNullException.ThrowIfNull(fs);

        return fs.Exists && IsReadOnly(fs);
    }

    /// <summary> Attempts to delete the file system item, represented by <paramref name="fs"/>,
    /// which is either an instance of FileInfo or DirectoryInfo.
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException">Passed if the required argument <paramref name="fs"/> is null.
    /// </exception>
    ///
    /// <param name="fs">                 The info about file or directory being deleted. </param>
    /// <param name="forceCleanReadOnly"> true to force clean read only-attribute and delete
    ///                                   the file or folder afterwards.</param>
    /// 
    /// <remarks>
    /// For more details about FileSystemInfo, see MSDN documentation
    /// <a href="http://msdn.microsoft.com/en-us/library/system.io.filesysteminfo.delete(v=vs.110).aspx">
    /// FileSystemInfo.Delete</a>.
    /// 
    /// For more info about exceptions occurrence when deleting files particularly, see MSDN
    /// <a href="http://msdn.microsoft.com/en-us/library/system.io.file.delete(v=vs.110).aspx">
    /// File.Delete Method</a>.
    /// </remarks>
    ///
    /// <returns> A SystemException in case the file or folder could not be deleted; 
    ///           or null if deleting succeeded. </returns>
    ///
    /// <seealso cref="LongPathTolerantAttemptToDelete"/>    
    public static SystemException AttemptToDelete(FileSystemInfo fs, bool forceCleanReadOnly)
    {
        ArgumentNullException.ThrowIfNull(fs);

        bool bDeleted = false;
        bool tryOnceMore = false;
        SystemException resultEx = null;

        do
        {
            bool needChangeAttribute = false;
            SystemException lastSystemEx = null;

            try
            {
                if (fs.Exists)
                {
                    if (needChangeAttribute = forceCleanReadOnly && IsReadOnly(fs))
                    {
                        fs.Attributes &= ~FileAttributes.ReadOnly;
                    }
                    fs.Delete();
                }
                bDeleted = true;
            }
            catch (IOException ex)
            {
                // this may occur if the file is open by someone else
                lastSystemEx = ex;
            }
            catch (UnauthorizedAccessException ex)
            {
                lastSystemEx = ex;
                // This may occur if the file actually is read-only. Give it one more try?
                if (tryOnceMore)
                {
                    tryOnceMore = false; // already tried
                }
                else if (forceCleanReadOnly && !needChangeAttribute)
                {
                    fs.Refresh();
                    tryOnceMore = IOHelper.IsReadOnly(fs);
                }
            }

            if ((!bDeleted) && (!tryOnceMore))
            {
                resultEx = lastSystemEx;
            }
        } while (!bDeleted && (resultEx == null));

        return resultEx;
    }

    /// <summary> Attempt to delete the file or folder represented by <paramref name="fs"/>. </summary>
    /// <remarks>
    /// Unlike the similar method <see cref="AttemptToDelete"/>, this one avoids usage of methods
    /// which throw <see cref="PathTooLongException"/> for case of path longer than 259 characters.
    /// </remarks>
    ///
    /// <exception cref="ArgumentNullException"> Passed when the required argument <paramref name="fs"/>
    ///  is null. </exception>
    /// <exception cref="ArgumentException"> Thrown if the argument required <paramref name="fs"/> is
    ///  neither a FileInfo nor DirectoryInfo. </exception>
    /// 
    /// <param name="fs"> Info about the file or folder being deleted. </param>
    /// <param name="forceCleanReadOnly"> true to force clean read only-attribute and delete the file
    ///  or folder afterwards. </param>
    /// <returns>
    /// A SystemException in case the file or folder could not be deleted; or null if deleting succeeded.
    /// </returns>
    /// 
    /// <seealso cref="AttemptToDelete"/>
    public static SystemException LongPathTolerantAttemptToDelete(FileSystemInfo fs, bool forceCleanReadOnly)
    {
        ArgumentNullException.ThrowIfNull(fs);

        string strFullName = FilePathHelper.SafeGetFullName(fs);
        FileInfo fi = fs as FileInfo;
        DirectoryInfo di = fs as DirectoryInfo;
        bool bDeleted = false;
        bool tryOnceMore = false;
        SystemException resultEx = null;

        if ((fi == null) && (di == null))
        {
            throw new ArgumentException("Not supported argument type " + fs.GetType(), nameof(fs));
        }

        do
        {
            bool needChangeAttribute = false;
            SystemException lastSystemEx = null;

            try
            {
                if (fs.Exists)
                {
                    var oldAttributes = fs.Attributes;
                    var newAttributes = oldAttributes & ~FileAttributes.ReadOnly;

                    needChangeAttribute = forceCleanReadOnly && (newAttributes != oldAttributes);
                    if (fi != null)
                    {
                        if (needChangeAttribute)
                            LongPathFile.SetFileAttributes(strFullName, newAttributes);
                        LongPathFile.Delete(strFullName);
                    }
                    else
                    {
                        if (needChangeAttribute)
                            LongPathDirectory.SetDirectoryAttributes(strFullName, newAttributes);
                        LongPathDirectory.Delete(strFullName);
                    }
                }
                bDeleted = true;
            }
            catch (IOException ex)
            {
                // this may occur if the file is open by someone else
                lastSystemEx = ex;
            }
            catch (UnauthorizedAccessException ex)
            {
                lastSystemEx = ex;
                // This may occur if the file actually is read-only. Give it one more try?
                if (tryOnceMore)
                {
                    tryOnceMore = false; // already tried
                }
                else if (forceCleanReadOnly && !needChangeAttribute)
                {
                    fs.Refresh();
                    tryOnceMore = IOHelper.IsReadOnly(fs);
                }
            }

            if ((!bDeleted) && (!tryOnceMore))
            {
                resultEx = lastSystemEx;
            }
        } while (!bDeleted && (resultEx == null));

        return resultEx;
    }

    /// <summary> Attempt to copy one file to the other. </summary>
    ///
    /// <param name="fiSrc">  The source file info. </param>
    /// <param name="fiDest"> The destination file info. Is refreshed in case of successful copying. </param>
    /// <param name="forceCleanReadOnly"> true to force clean read only-attribute on the destination file.
    /// </param>
    ///
    /// <returns> A <see cref="SystemException"/> in case the operation could not be performed,
    ///           but failed either on <see cref="IOException"/> or <see cref="UnauthorizedAccessException"/>.
    ///           Returns null if succeeded. </returns>
    public static SystemException AttemptToCopy(FileInfo fiSrc, FileInfo fiDest, bool forceCleanReadOnly)
    {
        ArgumentNullException.ThrowIfNull(fiSrc);
        ArgumentNullException.ThrowIfNull(fiDest);

        SystemException resultEx = null;

        try
        {
            if (ExistsAsReadOnly((fiDest)) && forceCleanReadOnly)
            {
                fiDest.Attributes &= ~FileAttributes.ReadOnly;
            }
            File.Copy(FilePathHelper.SafeGetFullName(fiSrc), FilePathHelper.SafeGetFullName(fiDest), true);
            fiDest.Refresh();
        }
        catch (IOException ex)
        {
            resultEx = ex;
        }
        catch (UnauthorizedAccessException ex)
        {
            resultEx = ex;
        }

        return resultEx;
    }
    #endregion // Methods
}

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Kernel32 = PK.PkUtils.WinApi.Kernel32;

namespace PK.PkUtils.IO;

/// <summary>
/// Provides various file opening (launching) actions, such as opening files with Notepad++ or the system shell.
/// </summary>
public static class FileLauncher
{
    #region Methods 
    #region Public methods

    /// <summary>
    /// Opens the specified file using Notepad++ if it is installed.
    /// </summary>
    /// <param name="fileName">The full path of the file to open.</param>
    /// <returns>
    /// <c>true</c> if the Notepad++ process was successfully started; <c>false</c> if
    /// Notepad++ was not found or <see cref="Process.Start()"/> returned <c>false</c>.
    /// Successful process creation does not guarantee that the document was opened.
    /// </returns>
    /// <seealso cref="OpenFileByShell"/>
    public static bool OpenFileByNotepadPlusPlus(string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName);
        string notepad = FindNotepadPlusPlus();

        if (notepad == null)
        {
            return false;
        }
        else
        {
            // Dispose the Process wrapper and pass the filename as one correctly escaped argument.
            using Process process = new();
            process.StartInfo.FileName = notepad;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.ArgumentList.Add(fileName);

            // Report whether Windows actually created the process (not whether it opened the file).
            return process.Start();
        }
    }

    /// <summary> Opens the specified file using the system shell's default handler. </summary>
    /// <remarks>
    /// This method is useful when Notepad++ is not available or when the default application is preferred.
    /// </remarks>
    /// <param name="fileName">The full path of the file to open.</param>
    /// <seealso cref="OpenFileByNotepadPlusPlus"/>
    public static void OpenFileByShell(string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        ProcessStartInfo psi = new(fileName) { UseShellExecute = true };
        Process.Start(psi);
    }

    /// <summary> Opens "This PC" (formerly "My Computer") in Windows Explorer. </summary>
    /// <remarks>
    /// The GUID {20D04FE0-3AEA-1069-A2D8-08002B30309D} is the CLSID for "My Computer" (now "This PC").
    /// The <c>/e,::</c> syntax tells Explorer to open the folder specified by the CLSID.
    /// See: https://learn.microsoft.com/en-us/windows/win32/shell/knownfolderid
    /// </remarks>
    public static void OpenMyComputerInExplorer()
    {
        const string myComputerClsid = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
        // Launches Windows Explorer targeting "This PC" using its CLSID.
        ProcessStartInfo psi = new()
        {
            FileName = "explorer.exe",
            Arguments = $"/e,{myComputerClsid}",
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    /// <summary>
    /// Opens the specified file using Notepad++ when possible; otherwise, tries the system shell.
    /// </summary>
    /// <remarks>
    /// <para>Fallback behavior:</para>
    /// <list type="bullet">
    /// <item><description>Notepad++ starts successfully: return normally.</description></item>
    /// <item><description>Notepad++ is not installed: open using the system shell.</description></item>
    /// <item><description>Notepad++ cannot start due to a recognized Windows startup error:
    /// try the system shell. The recognized error codes are file not found (2), path not found (3),
    /// access denied (5), bad executable format (193), and elevation required (740).</description></item>
    /// <item><description>Notepad++ startup fails with a recognized error and the shell also fails:
    /// throw <see cref="AggregateException"/> containing both original exceptions.</description></item>
    /// <item><description>An unexpected Notepad++ exception occurs: propagate it unchanged,
    /// without attempting a shell fallback.</description></item>
    /// </list>
    /// <para>If Notepad++ is missing or returns <c>false</c>, and the shell fails,
    /// the shell exception propagates directly because no Notepad++ startup exception exists
    /// to preserve. A successful process start does not prove that the file was opened.</para>
    /// </remarks>
    /// <param name="fileName">The full path of the file to open.</param>
    /// <exception cref="ArgumentException">The filename is null or empty.</exception>
    /// <exception cref="AggregateException">Both the recognized Notepad++ startup attempt
    /// and the shell fallback failed; the inner exceptions contain both failures.</exception>
    public static void OpenFileByNotepadPlusPlusOrShell(string fileName)
    {
        ArgumentException.ThrowIfNullOrEmpty(fileName);

        try
        {
            if (OpenFileByNotepadPlusPlus(fileName))
                return;
        }
        catch (Win32Exception notepadException) when (ShouldFallbackToShell(notepadException))
        {
            try
            {
                OpenFileByShell(fileName);
                return;
            }
            catch (Exception shellException)
            {
                // Preserve both failures when the alternative shell launch also fails.
                throw new AggregateException(
                    "Both Notepad++ and the system shell failed to open the file.",
                    notepadException,
                    shellException);
            }
        }

        // Notepad++ was not found, or its process start returned false.
        OpenFileByShell(fileName);
    }
    #endregion // Public methods

    #region Private methods

    /// <summary>
    /// Returns whether a Windows process-creation failure should trigger the shell fallback.
    /// Only known Notepad++ launch failures qualify; other exceptions propagate unchanged.
    /// </summary>
    /// <param name="exception">The Windows process-start exception.</param>
    /// <returns><c>true</c> for Windows errors 2, 3, 5, 193, or 740.</returns>
    private static bool ShouldFallbackToShell(Win32Exception exception)
    {
        return exception.NativeErrorCode is
            Kernel32.ERROR_FILE_NOT_FOUND   // ERROR_FILE_NOT_FOUND
            or Kernel32.ERROR_PATH_NOT_FOUND   // ERROR_PATH_NOT_FOUND
            or Kernel32.ERROR_ACCESS_DENIED   // ERROR_ACCESS_DENIED
            or Kernel32.ERROR_BAD_EXE_FORMAT // ERROR_BAD_EXE_FORMAT
            or Kernel32.ERROR_ELEVATION_REQUIRED; // ERROR_ELEVATION_REQUIRED
    }

    /// <summary> Attempts to locate the Notepad++ executable on the system. </summary>
    /// <returns> The full path to notepad++.exe if found; otherwise, <c>null</c>. </returns>
    private static string FindNotepadPlusPlus()
    {
        const string childPath = "Notepad++\\notepad++.exe";

        string strProgramsNative = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        string strPathNative = Path.Combine(strProgramsNative, childPath);
        string result = null;

        if (File.Exists(strPathNative))
        {
            result = strPathNative;
        }
        else
        {
            string strProgramsX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string strPathX86 = Path.Combine(strProgramsX86, childPath);

            if (strProgramsX86 != strProgramsNative && File.Exists(strPathX86))
                result = strPathX86;
        }

        return result;
    }
    #endregion // Private methods
    #endregion // Methods 
}

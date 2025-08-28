// Ignore Spelling: Utils, App
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace PK.PkUtils.SystemEx;

#pragma warning disable IDE0079   // Remove unnecessary suppressions
#pragma warning disable IDE0018
#pragma warning disable SYSLIB1054  // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

/// <summary> A locking process finder. </summary>
public static class ProcessUtils
{
    #region Typedefs

    [StructLayout(LayoutKind.Sequential)]
    private struct RM_UNIQUE_PROCESS
    {
        public int dwProcessId;
        public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
    }

    private enum RM_APP_TYPE
    {
        RmUnknownApp = 0,
        RmMainWindow = 1,
        RmOtherWindow = 2,
        RmService = 3,
        RmExplorer = 4,
        RmConsole = 5,
        RmCritical = 1000
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct RM_PROCESS_INFO
    {
        public RM_UNIQUE_PROCESS Process;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)]
        public string strAppName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)]
        public string strServiceShortName;

        public RM_APP_TYPE ApplicationType;
        public uint AppStatus;
        public uint TSSessionId;
        [MarshalAs(UnmanagedType.Bool)]
        public bool bRestartable;
    }
    #endregion // Typedefs

    #region Fields

    internal const int RmRebootReasonNone = 0;
    internal const int CCH_RM_MAX_APP_NAME = 255;
    internal const int CCH_RM_MAX_SVC_NAME = 63;

    // just use the current TS server context.
    internal static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

    // the User Name is the info we want returned by the query.
    internal static int WTS_UserName = 5;

    #endregion // Fields

    #region External functions


    // Registers resources to a Restart Manager session.
    // For more info, see
    // https://msdn.microsoft.com/en-us/library/windows/desktop/aa373663(v=vs.85).aspx
    // http://community.bartdesmet.net/blogs/bart/archive/2006/11/12/Exploring-Windows-Vista_2700_s-Restart-Manager-in-C_2300_.aspx
    [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
    private static extern int RmRegisterResources(
        uint pSessionHandle,
        UInt32 nFiles,
        string[] rgsFilenames,
        UInt32 nApplications,
        [In] RM_UNIQUE_PROCESS[] rgApplications,
        UInt32 nServices,
        string[] rgsServiceNames);

    // Starts a new Restart Manager session.
    // For more info, see
    // https://msdn.microsoft.com/en-us/library/windows/desktop/aa373668(v=vs.85).aspx
    // http://community.bartdesmet.net/blogs/bart/archive/2006/11/12/Exploring-Windows-Vista_2700_s-Restart-Manager-in-C_2300_.aspx
    [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
    private static extern int RmStartSession(
        out uint pSessionHandle,
        int dwSessionFlags,
        string strSessionKey);

    // Ends the Restart Manager session.
    // For more info, see
    // https://msdn.microsoft.com/en-us/library/windows/desktop/aa373659(v=vs.85).aspx
    // http://community.bartdesmet.net/blogs/bart/archive/2006/11/12/Exploring-Windows-Vista_2700_s-Restart-Manager-in-C_2300_.aspx
    [DllImport("rstrtmgr.dll")]
    private static extern int RmEndSession(uint pSessionHandle);

    // Gets a list of all applications and services that are currently using resources that have been registered with the Restart Manager session.
    // For more info, see
    // https://msdn.microsoft.com/en-us/library/windows/desktop/aa373661(v=vs.85).aspx
    // http://community.bartdesmet.net/blogs/bart/archive/2006/11/12/Exploring-Windows-Vista_2700_s-Restart-Manager-in-C_2300_.aspx
    [DllImport("rstrtmgr.dll")]
    private static extern int RmGetList(
        uint dwSessionHandle,
        out uint pnProcInfoNeeded,
        ref uint pnProcInfo,
        [In, Out] RM_PROCESS_INFO[] rgAffectedApps,
        ref uint lpdwRebootReasons);

    /// <summary>
    /// Retrieves session information for the specified session on the specified Remote Desktop
    /// Session Host (RD Session Host) server. It can be used to query session information on local
    /// and remote RD Session Host servers.
    /// </summary>
    /// 
    /// <remarks>
    /// For more info, see https://msdn.microsoft.com/en-us/library/aa383838(v=vs.85).aspx
    /// </remarks>
    ///
    /// <param name="hServer">          The server. </param>
    /// <param name="SessionId">        Identifier for the session. </param>
    /// <param name="WTSInfoClass">     The wts information class. </param>
    /// <param name="ppBuffer">         [out] The buffer. </param>
    /// <param name="pBytesReturned">   [out] The bytes returned. </param>
    ///
    /// <returns> True if it succeeds, false if it fails. </returns>
    [DllImport("Wtsapi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool WTSQuerySessionInformationW(
        IntPtr hServer,
        int SessionId,
        int WTSInfoClass,
        out IntPtr ppBuffer,
        out IntPtr pBytesReturned);
    #endregion // External functions

    #region Methods

    /// <summary>
    /// Find out what process(es) have a lock on the specified file.
    /// </summary>
    /// <param name="path">Path of the file. Can't be null or empty. </param>
    /// <returns> List of processes locking the file. </returns>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="path"/> is null or empty. </exception>
    /// 
    /// <remarks>See also:
    /// https://stackoverflow.com/questions/317071/how-do-i-find-out-which-process-is-
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa373661(v=vs.85).aspx
    /// http://wyupdate.googlecode.com/svn-history/r401/trunk/frmFilesInUse.cs (no copyright in code at time of viewing)
    /// </remarks>
    public static List<Process> WhoIsLocking(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        string key = Guid.NewGuid().ToString();
        List<Process> processes = [];
        int nErrorCode = RmStartSession(out uint handle, 0, key);

        if (nErrorCode != 0)
        {
            string strErr = string.Format(CultureInfo.InvariantCulture,
                "Could not begin restart session. Unable to determine file locker ( {0} ).", ErrorCodeToString(nErrorCode));
            throw new Win32Exception(nErrorCode, strErr);
        }

        try
        {
            const int ERROR_MORE_DATA = 234;
            uint pnProcInfoNeeded = 0;
            uint pnProcInfo = 0;
            uint lpdwRebootReasons = RmRebootReasonNone;

            string[] resources = [path]; // Just checking on one resource.

            nErrorCode = RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null);

            if (nErrorCode != 0)
            {
                string strErr = string.Format(CultureInfo.InvariantCulture,
                    "Could not register resource ( {0} ).", ErrorCodeToString(nErrorCode));
                throw new Win32Exception(nErrorCode, strErr);
            }

            //Note: there's a race condition here -- the first call to RmGetList() returns
            //      the total number of process. However, when we call RmGetList() again to get
            //      the actual processes this number may have increased.
            nErrorCode = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons);

            if (nErrorCode == ERROR_MORE_DATA)
            {
                // Create an array to store the process results
                RM_PROCESS_INFO[] processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
                pnProcInfo = pnProcInfoNeeded;

                // Get the list
                nErrorCode = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, ref lpdwRebootReasons);
                if (nErrorCode == 0)
                {
                    processes = new List<Process>((int)pnProcInfo);

                    // Enumerate all of the results and add them to the 
                    // list to be returned
                    for (int i = 0; i < pnProcInfo; i++)
                    {
                        try
                        {
                            processes.Add(Process.GetProcessById(processInfo[i].Process.dwProcessId));
                        }
                        // catch the error -- in case the process is no longer running
                        catch (ArgumentException) { }
                    }
                }
                else
                {
                    string strErr = string.Format(CultureInfo.InvariantCulture,
                        "Could not list processes locking resource ( {0} ).", ErrorCodeToString(nErrorCode));
                    throw new Win32Exception(nErrorCode, strErr);
                }
            }
            else if (nErrorCode != 0)
            {
                string strErr = string.Format(CultureInfo.InvariantCulture,
                    "Could not list processes locking resource. Failed to get size of result ( {0} ).", ErrorCodeToString(nErrorCode));
                throw new Win32Exception(nErrorCode, strErr);
            }
        }
        finally
        {
            RmEndSession(handle);
        }

        return processes;
    }


    /// <summary> Gets user name for process, by the means of Terminal Services Server. </summary>
    ///
    /// <param name="proc"> The process to process. Can't be null. </param>
    /// <returns> The user name for process, or an empty string in case of error. </returns>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when required argument <paramref name="proc"/> is null. </exception>
    /// 
    /// <seealso href="https://social.msdn.microsoft.com/Forums/vstudio/en-US/aeff7e41-a4ba-4bf0-8677-81162040984d/retrieving-username-of-a-running-process?forum=netfxbcl">
    /// Retrieving user name of a running process</seealso>
    public static string GetUserNameForProcess(Process proc)
    {
        string result = string.Empty;

        ArgumentNullException.ThrowIfNull(proc);

        if (WTSQuerySessionInformationW(WTS_CURRENT_SERVER_HANDLE,
                                        proc.SessionId,
                                        WTS_UserName,
                                        out IntPtr answerBytes,
                                        out _))
        {
            result = Marshal.PtrToStringUni(answerBytes);
        }
        else
        {
            Trace.WriteLine("Could not access Terminal Services Server.");
        }

        return result;
    }

    private static string ErrorCodeToString(int nErrorCode)
    {
        Win32Exception ex = new(nErrorCode);
        string result = ex.Message;

        return result;
    }
    #endregion // Methods
}

#pragma warning restore SYSLIB1054
#pragma warning restore IDE0018
#pragma warning restore IDE0079
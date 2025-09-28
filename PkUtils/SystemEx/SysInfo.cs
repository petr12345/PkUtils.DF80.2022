// Ignore Spelling: Admin, Fallback, Msinfo, Sys, Utils, Ver
//
using System;
using System.IO;
using System.Security.Principal;
using Microsoft.Win32;
using PK.PkUtils.WinApi;


namespace PK.PkUtils.SystemEx;

/// <summary> A static class providing various system-related info. </summary>
public static class SysInfo
{
    #region Fields
    /// <summary>
    /// Cached value of last call of GetVer()
    /// </summary>
    private static Nullable<WinVer> _ver;

    private const int VER_NT_WORKSTATION = 0x0000001;
    // private const int VER_NT_DOMAIN_CONTROLLER = 0x0000002;
    private const int VER_NT_SERVER = 0x0000003;

    #endregion // Fields

    #region Methods

    /// <summary> Get the service pack, using Kernel32.GetVersionEx. </summary>
    ///
    /// <returns> Value of current service pack, like "Service Pack 1".</returns>
    public static string GetServicePack()
    {
        Kernel32.OSVERSIONINFOEX osVersionInfo = new();
        string servicePack = string.Empty;

        if (osVersionInfo.GetVersionEx())
        {
            servicePack = osVersionInfo.szCSDVersion;
        }

        return servicePack;
    }

    /// <summary> Get operating system version information. </summary>
    /// <returns>The detected operating system version.</returns>
    /// <seealso href="http://stackoverflow.com/questions/2819934/detect-windows-7-in-net">
    /// Stackoverflow: Detect Windows 7 in .net</seealso>
    /// <seealso href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms724834(v=vs.85).aspx">
    /// OSVERSIONINFO structure</seealso>
    public static WinVer GetWindowsVersion()
    {
        if (_ver.HasValue)
            return _ver.Value; // Cached result

        OperatingSystem os = Environment.OSVersion;
        Kernel32.OSVERSIONINFOEX osVersionInfo = new();
        WinVer result = WinVer.Unknown;

        if (osVersionInfo.GetVersionEx())
        {
            int majorVersion = os.Version.Major;
            int minorVersion = os.Version.Minor;
            byte productType = osVersionInfo.wProductType;

            switch (os.Platform)
            {
                case PlatformID.Win32S:
                    result = WinVer.Win31;
                    break;

                case PlatformID.WinCE:
                    result = WinVer.WinCE;
                    break;

                case PlatformID.Win32Windows:
                    if (majorVersion == 4)
                    {
                        switch (minorVersion)
                        {
                            case 0:
                                result = osVersionInfo.szCSDVersion == "B" || osVersionInfo.szCSDVersion == "C"
                                    ? WinVer.Win95_OSR2
                                    : WinVer.Win95;
                                break;
                            case 10:
                                result = osVersionInfo.szCSDVersion == "A"
                                    ? WinVer.Win98_SE
                                    : WinVer.Win98;
                                break;
                            case 90:
                                result = WinVer.WinME;
                                break;
                        }
                    }
                    break;

                case PlatformID.Win32NT:
                    switch (majorVersion)
                    {
                        case 3:
                            result = WinVer.WinNT351;
                            break;

                        case 4:
                            result = productType == VER_NT_WORKSTATION ? WinVer.WinNT4 : WinVer.WinNT4_Server;
                            break;

                        case 5:
                            switch (minorVersion)
                            {
                                case 0:
                                    result = WinVer.Win2000;
                                    break;
                                case 1:
                                    result = WinVer.WinXP;
                                    break;
                                case 2:
                                    result = User32.GetSystemMetrics(Win32.SM.SM_SERVERR2) == 0
                                        ? WinVer.WinServer2003
                                        : WinVer.WinServer2003_R2;
                                    break;
                            }
                            break;

                        case 6:
                            switch (minorVersion)
                            {
                                case 0:
                                    result = productType == VER_NT_WORKSTATION ? WinVer.WinVista : WinVer.WinServer2008;
                                    break;
                                case 1:
                                    result = productType == VER_NT_WORKSTATION ? WinVer.Win7 : WinVer.WinServer2008_R2;
                                    break;
                                case 2:
                                    result = productType == VER_NT_WORKSTATION
                                        ? WinVer.Win8
                                        : WinVer.WinServer2012;
                                    break;
                                case 3:
                                    result = productType == VER_NT_WORKSTATION
                                        ? WinVer.Win8_1
                                        : WinVer.WinServer2012_R2;
                                    break;
                            }
                            break;

                        // Microsoft did not bump the major version from 10 to 11 in the kernel.
                        // Only the marketing name changed to Windows 11.
                        // Detection based on version numbers alone can’t distinguish Win10 vs Win11.
                        // The reliable way is to use the build number.
                        case 10:
                            if (productType == VER_NT_WORKSTATION)
                            {
                                if (os.Version.Build >= 22000)
                                    result = WinVer.Win11;
                                else
                                    result = WinVer.Win10;
                            }
                            else
                            {
                                result = WinVer.WinServer2016;
                            }
                            break;
                    }
                    break;
            }
        }

        _ver = result;
        return result;
    }

    /// <summary> Find the full path to "MSInfo32.exe". </summary>
    /// <returns> Returns non-empty string on success, empty string on failure. </returns>
    public static string GetMsinfo32Path()
    {
        string strTempPath = string.Empty;
        RegistryKey regKey = Registry.LocalMachine;

        if (regKey != null)
        {
            FileInfo fi;
            object objTmp = null;

            regKey = regKey.OpenSubKey("Software\\Microsoft\\Shared Tools\\MSInfo");
            if (regKey != null)
            {
                objTmp = regKey.GetValue("Path");
            }

            if (objTmp == null)
            {
                regKey = regKey.OpenSubKey("Software\\Microsoft\\Shared Tools Location");
                if (regKey != null)
                {
                    objTmp = regKey.GetValue("MSInfo");
                    if (objTmp != null)
                    {
                        strTempPath = Path.Combine(objTmp.ToString(), "MSInfo32.exe");
                    }
                }
            }
            else
            {
                strTempPath = objTmp.ToString();
            }

            try
            {
                fi = new FileInfo(strTempPath);
                if (!fi.Exists)
                {
                    strTempPath = string.Empty;
                }
            }
            catch (ArgumentException)
            {
                strTempPath = string.Empty;
            }
        }

        return strTempPath;
    }

    /// <summary>
    /// Checks if you are running as an administrator. For Windows Vista and later, if User Account Control (UAC)
    /// is enabled, this means whether the application currently runs with Administrator privileges, ( NOT just
    /// whether you belong to the Administrators group).
    /// </summary>
    ///
    /// <returns> true if currently running as an administrator, false if not. </returns>
    ///
    /// <seealso href="http://stackoverflow.com/questions/2819934/detect-windows-7-in-net">       dotnet thoughts:
    /// How to check you are running as administrator using C#</seealso>
    /// <seealso href="http://www.dotnetthoughts.net/create-uac-compatible-applications-in-net/"> dotnet thoughts:
    /// Create UAC Compatible applications in .NET. </seealso>
    public static bool IsCurrentlyRunningAsAdmin()
    {
        bool isAdmin = false;

        using (WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent())
        {
            if (currentIdentity != null)
            {
                WindowsPrincipal principal = new(currentIdentity);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
        return isAdmin;
    }

    #endregion // Methods
}

// Ignore Spelling: Utils, Ver
//

namespace PK.PkUtils.SystemEx;

/// <summary>
/// Desktop Windows versions known so far
/// </summary>
public enum WinVer : int
{
    /// <summary> Unknown version </summary>
    Unknown = 0,

    /// <summary> Windows CE </summary>
    WinCE,

    /// <summary> Windows 3.1 </summary>
    Win31,

    /// <summary> Windows 95 </summary>
    Win95,

    /// <summary> Windows 95 OSR2 </summary>
    Win95_OSR2,

    /// <summary> Windows 98 </summary>
    Win98,

    /// <summary> Windows 98 Second Edition </summary>
    Win98_SE,

    /// <summary> Windows Millennium Edition (Windows Me) </summary>
    WinME,

    /// <summary> Windows NT 3.51 </summary>
    WinNT351,

    /// <summary> Windows NT 4.0 </summary>
    WinNT4,

    /// <summary> Windows NT 4.0 Server </summary>
    WinNT4_Server,

    /// <summary> Windows 2000 </summary>
    Win2000,

    /// <summary> Windows XP </summary>
    WinXP,

    /// <summary> Windows Server 2003 </summary>
    WinServer2003,

    /// <summary> Windows Server 2003 R2 </summary>
    WinServer2003_R2,

    /// <summary> Windows Vista </summary>
    WinVista,

    /// <summary> Windows Server 2008. Successor to Windows Server 2003, built from the same code base as Windows Vista. </summary>
    /// <seealso href="http://en.wikipedia.org/wiki/Windows_Server_2008">Windows Server 2008</seealso>
    WinServer2008,

    /// <summary> Windows Server 2008 R2. Introduced as the server variant of Windows 7. </summary>
    /// <seealso href="http://en.wikipedia.org/wiki/Windows_Server_2008_R2">Windows Server 2008 R2</seealso>
    WinServer2008_R2,

    /// <summary> Windows 7 </summary>
    Win7,

    /// <summary> Windows Server 2012 </summary>
    WinServer2012,

    /// <summary> Windows Server 2012 R2 </summary>
    WinServer2012_R2,

    /// <summary> Windows 8 </summary>
    Win8,

    /// <summary> Windows 8.1 </summary>
    Win8_1,

    /// <summary> Windows Server 2016 </summary>
    WinServer2016,

    /// <summary> Windows 10 </summary>
    Win10,

    /// <summary> Windows Server 2019 </summary>
    WinServer2019,

    /// <summary> Windows 11 </summary>
    Win11,

    /// <summary> Windows Server 2022 </summary>
    WinServer2022,

    /// <summary> Placeholder for future Windows versions </summary>
    UnknownFuture = int.MaxValue,
}
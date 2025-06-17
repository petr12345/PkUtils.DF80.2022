// Ignore Spelling: Utils, Admin
//
using System;
using PK.PkUtils.Interfaces;
using PK.PkUtils.SystemEx;
using PK.PkUtils.WinApi;

namespace PK.PkUtils.Dump;

/// <summary>
/// The class implementing <see cref="PK.PkUtils.Interfaces.IDumper"/> interface by writing to the console.<br/>
/// 
/// For more info about this subject, see for instance
/// <a href="http://www.csharp411.com/console-output-from-winforms-application/">”C# 411 : Console
/// Output from a WinForms Application</a>.  <br/>
/// See also on Stackoverflow.com:
/// <a href="http://stackoverflow.com/questions/8047741/attachconsole-1-but-console-writeline-wont-output-to-parent-command-prompt">
/// ”AttachConsole(-1), but Console.WriteLine won't output to parent command prompt?</a>. 
/// 
/// </summary>
[CLSCompliant(true)]
public class DumperConsoleWrapper : IDumper
{
    #region Fields
    private bool _bAttached;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default argument-less  constructor. Delegates to overloaded constructor, with value bAttachConsole = true
    /// </summary>
    public DumperConsoleWrapper()
      : this(false)
    {
    }

    /// <summary> One-argument constructor. If bAttachConsole is true, the program attempts to attach to console of
    /// the parent process, to be able to write to it. <br/>
    /// Delegates the call to overloaded constructor with two boolean arguments; the second argument
    /// value is determined by property <see cref="ShouldCheckAdminPrivileges"/>  <br/>  </summary>
    ///
    /// <param name="bAttachConsole"> If true, will attempt to attach to console of the parent process. </param>
    public DumperConsoleWrapper(bool bAttachConsole)
      : this(bAttachConsole, ShouldCheckAdminPrivileges)
    {
    }

    /// <summary> Two-arguments constructor. If bAttachConsole is true, the program attempts to attach to console of
    /// the parent process, to be able to write to it.  
    /// This will be done done by calling <see cref="AttachConsole"/>, using a boolean argument
    /// <paramref name="bCheckAdminPrivileges"/> </summary>
    ///
    /// <param name="bAttachConsole">        If true, will attempt to attach to console of the parent
    /// process. </param>
    /// <param name="bCheckAdminPrivileges"> Is used as an input argument  in the call  of
    /// <see cref="AttachConsole"/> </param>
    public DumperConsoleWrapper(bool bAttachConsole, bool bCheckAdminPrivileges)
    {
        if (bAttachConsole)
        {
            AttachConsole(bCheckAdminPrivileges);
        }
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Has the call of AttachConsole() succeeded ?
    /// </summary>
    public bool IsConsoleAttached
    {
        get { return _bAttached; }
    }

    /// <summary>
    /// This property returns true in case the code of AttachConsole should verify admin rights before doing anything.<br/>
    /// Currently returns true if running on the system Vista or later.  <br/>
    /// See more related comments in <see cref="AttachConsole"/>method.
    /// </summary>
    protected static bool ShouldCheckAdminPrivileges
    {
        get
        {
            WinVer winVer = SysInfo.GetWindowsVersion();
            bool result = (winVer >= WinVer.WinVista);

            return result;
        }
    }
    #endregion // Properties

    #region Methods

    /// <summary> Attach to the console of parent process, if has not been attached already. If
    /// <paramref name="bCheckAdminPrivileges"/> is true, the class attempts to call Kernel32.AttachConsole only if the
    /// code is currently running as admin. </summary>
    ///
    /// <remarks> For operating systems like Vista or higher, that introduced UAC, you should check if
    /// the code is currently running as admin. For those OSs, API Kernel32.AttachConsole may return
    /// true but if you are not admin you still cannot write to console. </remarks>
    ///
    /// <param name="bCheckAdminPrivileges"> Should the method check admin privileges for case of Vista or higher.</param>
    ///
    /// <returns> If the console was not attached already, returns true on success, false on failure.
    /// Otherwise,  just returns the <see cref="IsConsoleAttached"/> property. </returns>
    public bool AttachConsole(bool bCheckAdminPrivileges)
    {
        if (!IsConsoleAttached)
        {
            if (bCheckAdminPrivileges && !PK.PkUtils.SystemEx.SysInfo.IsCurrentlyRunningAsAdmin())
            { // don't do anything.  perhaps trace a warning
            }
            else
            {
                _bAttached = Kernel32.AttachConsole(Kernel32.ATTACH_PARENT_PROCESS);
            }
        }
        return IsConsoleAttached;
    }
    #endregion // Methods

    #region IDumper Members

    /// <summary>	Implementation of IDumper.DumpText. </summary>
    /// <param name="text">	The dumped text. </param>
    /// <returns>	true if it succeeds, false if it fails. </returns>
    public bool DumpText(string text)
    {
        Console.Write(text);
        return true;
    }

    /// <inheritdoc/>
    public bool DumpWarning(string text)
    {
        return DumpText(text);
    }

    /// <inheritdoc/>
    public bool DumpError(string text)
    {
        return DumpText(text);
    }

    /// <summary>	Implementation of IDumper.Reset.  Cleans any previously dumped contents. </summary>
    /// <returns>	true if it succeeds, false if it fails. </returns>
    public bool Reset()
    {
        Console.Clear();
        return true;
    }
    #endregion // IDumper Members
}

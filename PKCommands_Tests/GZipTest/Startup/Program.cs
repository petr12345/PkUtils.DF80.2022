// Ignore Spelling: Ctrl
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using GZipTest.Infrastructure;
using log4net;
using PK.Commands.CommandProcessing;
using PK.Commands.CommandUtils;
using PK.Commands.Interfaces;
using PK.PkUtils.Consoles;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using PK.PkUtils.Utils;
using PK.PkUtils.WinApi;
using static System.FormattableString;
using ExitCode = PK.Commands.CommandProcessing.ExitCode;
using IGZipTestCommand = PK.Commands.Interfaces.ICommandEx<PK.Commands.CommandProcessing.ExitCode>;
using ILogger = log4net.ILog;


namespace GZipTest.Startup;

using GZipTestCommandsRegister = CommandRegisterEx<IGZipTestCommand, ExitCode>;
using IGZipTestCommandsRegister = ICommandRegisterEx<IGZipTestCommand, ExitCode>;

/// <summary> A program containing the main entry point for the application. </summary>
/// <remarks> The class is not static and implements IDisposable to properly releases resources on shutdown.</remarks>
public class Program : Singleton<Program>, IDisposableEx
{
    #region Fields

    private static bool _restartRequested;
    private static bool _ctrlBreakPressed;
    /// <summary>
    /// A static delegate instance for handling console exit events. This prevents the delegate from being
    /// garbage collected, ensuring that the handler remains registered throughout the application's lifetime.
    /// </summary>
    private static Kernel32.HandlerRoutine _consoleHandler;

    private readonly ILogger _logger;
    private readonly IConsoleDisplay _consoleDisplay;
    private readonly Lazy<ICommandsInputProcessor<ExitCode>> _lazyInputProcessor;
    private IGZipTestCommandsRegister _cmndsRegister;
    private CancellationTokenSource _cancellationTokenSource;
    private string _readPrompt;
    private bool _disposed;

    private const string _programName = "GZipTest";
    private const int _msSleepBeforeErrorExit = 8 * 1024;
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Protected argument-less constructor, that prevents a default instance of this class from being created. </summary>
    protected Program()
    {
        // set console display
        _consoleDisplay = new ConsoleDisplay();

        // set logger
        _logger = CreateLogger();
        // golden source for cancellation
        _cancellationTokenSource = new CancellationTokenSource();

        // lazy initialize input processor
        _lazyInputProcessor = new Lazy<ICommandsInputProcessor<ExitCode>>(
            () => new DualFormatInputProcessor<IGZipTestCommand, ExitCode>(
                this.Logger,
                this.ConsoleDisplay,
                this.CommandsRegister));

        // default read prompt, please
        UpdateReadPrompt(null);
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>  Gets the name of the program. </summary>
    public static string ProgramName => _programName;
    /// <summary>  Gets the name + assembly version of program. </summary>
    public static string ProgramInfo => $"{ProgramName} {Assembly.GetExecutingAssembly().GetAssemblyVersion()}";

    protected internal static bool IsRestartRequested { get => _restartRequested; }
    protected internal static bool CtrlBreakPressed { get => _ctrlBreakPressed; }

    /// <summary>  Gets the logger for this instance. </summary>
    protected internal ILogger Logger
    {
        get { Debug.Assert(_logger != null); return _logger; }
    }

    /// <summary> Getter to get cancellation means. </summary>
    protected internal CancellationTokenSource CancellationTokenSource { get => _cancellationTokenSource; }

    protected internal bool IsCancellationRequested { get => CancellationTokenSource.IsCancellationRequested; }

    /// <summary> Gets the cancellation token. </summary>
    protected internal CancellationToken CancellationToken
    {
        get { return (CancellationTokenSource != null) ? CancellationTokenSource.Token : CancellationToken.None; }
    }

    private ICommandsInputProcessor<ExitCode> InputProcessor
    {
        get { /* Debug.Assert(_lazyInputProcessor.IsValueCreated); */ return _lazyInputProcessor.Value; }
    }

    private ICommandRegisterEx<IGZipTestCommand, ExitCode> CommandsRegister
    {
        get => _cmndsRegister ??= new GZipTestCommandsRegister();
    }

    private IConsoleDisplay ConsoleDisplay { get => _consoleDisplay; }

    private bool RunningCommandsFromConsole { get; set; }
    #endregion // Properties

    #region IDisposableEx Members

    /// <summary> Implements IDisposable; releases all resources used by the object.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary> Gets a value indicating whether this object is disposed. </summary>
    public bool IsDisposed { get => _disposed; }

    #endregion // IDisposableEx Members

    #region Methods
    #region Public Methods

    /// <summary> The main entry point for the application. </summary>
    /// <param name="args"> An array of command-line argument strings. </param>
    /// <returns> Exit-code for the process - 0 for success, otherwise an error code. </returns>
    [STAThread]
    public static int Main(string[] args)
    {
        // Add "unhandled exception handler". Kind of Catch XXII, isn't it?
        // Note: Don't do that for Application.ThreadException event too ( that's only for WinForms application)
        AppDomain.CurrentDomain.UnhandledException += (sender, e) => ProcessUnhandledException(e.ExceptionObject);

        // Install Console Exit handler. For more info, see
        // https://social.msdn.microsoft.com/Forums/vstudio/en-US/707e9ae1-a53f-4918-8ac4-62a1eddb3c4a/detecting-console-application-exit-in-c?forum=csharpgeneral
        Kernel32.SetConsoleCtrlHandler(_consoleHandler = new(ConsoleExitHandler), true);

        try
        {
            return Instance.InitializeAndRun(args).Code;
        }
        finally
        {
            if (HasInstance)
            {
                Program.Instance.SayGoodByeIfCtrlC();
                Program.Instance.Dispose();
            }

            if (IsRestartRequested)
            {
                RestartMe();
            }
            else
            {
                // Nothing to do
            }
        }
    }
    #endregion // Public Methods

    #region Protected Methods 

    /// <summary>   Initializes the application and run. </summary>
    /// 
    /// <param name="args"> An array of command-line argument strings. </param>
    /// <returns>   An ExitCode. </returns>
    protected ExitCode InitializeAndRun(string[] args)
    {
        string exitMsg;
        ExitCode result = ExitCode.Success;

        Logger.InfoFormat("Starting the {0}", ProgramName);

        // In case of need just ones instance permitted running at a time
        // if (!IsFirstProcess) result = ExitCode.RunningSecondProgramInstance;

        Console.CancelKeyPress += Console_CancelKeyPress;
        Console.TreatControlCAsInput = false;

        if (!InputProcessor.InitializeCommandContainer(this.Logger, Assembly.GetExecutingAssembly()))
        {
            // Don't bother to continue. Note that InputProcessor does logging for this failure
            result = ExitCode.FailedToRegisterCommands;
        }
        else
        {
            SetIcons(Resources.gears);
            result = ProcessIncomingCommands(args);
        }

        exitMsg = Invariant($"Stopping {ProgramName} with result '{result}'");
        if (result.IsOK() || result.HasBeenCanceled())
            Logger.Info(exitMsg);
        else
            Logger.Warn(exitMsg);
        Logger.Info(Invariant($"Stopped{Environment.NewLine}{new string('-', 64)}"));

        return result;
    }

    /// <summary>   A "main" commands processing method. </summary>
    /// <param name="args"> Sequence of command-line argument strings. </param>
    /// <returns>   An ExitCode . </returns>
    private ExitCode ProcessIncomingCommands(IEnumerable<string> args)
    {
        string consoleInput;
        bool shouldContinue;
        IComplexErrorResult<ExitCode> processsed;
        ExitCode result = ExitCode.Success; //  ok so far

        // Local static method for error handling
        static ExitCode HandleErrorResult(IComplexErrorResult<ExitCode> processed)
        {
            var res = processed.ErrorDetails;
            return res.IsOK() ? ExitCode.UnknownError : res;
        }

        try
        {
            // see what command is there, if any
            if (args.Any())
            {
                // Run command directly
                // 
                // Handle the weird case a newline comes from Visual Studio project properties ( Debug / Command Line Arguments )
                /* consoleInput = args.Join(" "); */
                consoleInput = args.FilterOutNewlines().JoinToCommandLine();
                processsed = InputProcessor.ProcessNextInput(consoleInput, out shouldContinue);

                if (!processsed.Success)
                {
                    result = HandleErrorResult(processsed);
                }
                if (!result.IsOK())
                {
                    Thread.Sleep(_msSleepBeforeErrorExit);
                }
            }
            else
            {
                this.RunningCommandsFromConsole = true;
                Console.Clear();
                DisplayWaitMessage(DefaultWaitForCommandMessage());

                // read any next command, and run till the end
                // 
                for (shouldContinue = true; shouldContinue;)
                {
                    if (null != (consoleInput = ReadFromConsole()))
                    {
                        consoleInput = consoleInput.Trim();
                    }
                    if (string.IsNullOrEmpty(consoleInput))
                    {
                        result = ExitCode.Success;
                        shouldContinue = false;
                    }
                    else
                    {
                        Logger.Info("Processing input: " + consoleInput);
                        processsed = InputProcessor.ProcessNextInput(consoleInput, out shouldContinue);

                        if (!processsed.Success)
                        {
                            result = HandleErrorResult(processsed);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            result = ExitCode.UnknownError;
            Logger.Fatal("Error while executing command.", ex);
        }
        finally
        {
            InputProcessor.OnCompleted();
        }

        return result;
    }


    /// <summary>   Implements IDisposable; releases all resources used by the object. </summary>
    /// <param name="disposing"> True to release both managed and unmanaged resources; false to release only
    /// unmanaged resources. </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed && disposing)
        {
            Disposer.SafeDispose(ref _cancellationTokenSource);
        }
        _disposed = true;
    }

    protected static void RestartMe()
    {
        // Must use MainModule.FileName to be assigned to ProcessStartInfo.FileName, 
        // instead of Assembly.GetEntryAssembly().Location, since that .Location value
        // may return a path ending with .dll instead of .exe in .NET Core and later versions.
        // 
        // Note, though, that even args[0] may end with .dll instead of .exe,
        // so it should be compared against both the .exe path and the assembly location.
        // 
        string assemblyLocation = Assembly.GetEntryAssembly().Location;
        // string exeFileName = Process.GetCurrentProcess().MainModule.FileName;
        string exeFileName = Environment.ProcessPath;
        string workingDirectory = Path.GetDirectoryName(exeFileName);
        string[] args = Environment.GetCommandLineArgs();
        string inputCommandLine = null;

        if (args.Length > 0)
        {
            // Compare args[0] against both the .exe and .dll paths
            if (string.Equals(args[0], exeFileName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(args[0], assemblyLocation, StringComparison.OrdinalIgnoreCase))
            {
                args = [.. args.Skip(1)];
            }
            inputCommandLine = string.Join(" ", args);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = exeFileName,               // Full path to the executable
            Arguments = inputCommandLine,         // Pass original arguments
            UseShellExecute = true,               // Needed for .NET 8 restart
            WorkingDirectory = workingDirectory,  // Explicitly set working dir
            CreateNoWindow = true                 // Avoids creating a new console window
        };

        Process.Start(startInfo);
        Environment.Exit(0);
    }
    #endregion // Protected Methods

    #region Private Methods 

    private static ILogger CreateLogger()
    {
        return LogManager.GetLogger(typeof(Program));
    }

    private static ILogger GetExistingOrCreateNewLogger()
    {
        ILogger result;

        if (Program.HasInstance)
            result = Instance.Logger;
        else
            result = CreateLogger();

        return result;
    }
    private static string DefaultWaitConsoleMessage()
    {
        return Invariant($"{ProgramName} started, Press <Ctrl + Break> to terminate or <Ctrl + Shift + Break> to restart");
    }

    private static string DefaultWaitForCommandMessage()
    {
        return Invariant($"{ProgramName} started. Enter valid command, or press <Ctrl + Break> to terminate");
    }

    private string WaitConsoleMessage()
    {
        return RunningCommandsFromConsole ? string.Empty : DefaultWaitConsoleMessage();
    }

    private void DisplayWaitMessage(string message)
    {
        string toDisplayMsg = message ?? WaitConsoleMessage();

        if (!string.IsNullOrEmpty(toDisplayMsg))
        {
            ConsoleDisplay.WriteSuccess(toDisplayMsg);
        }
    }

    /// <summary> Reads from console.</summary>
    ///
    /// <param name="promptMessage"> (Optional) Message appended to the prompt. </param>
    ///
    /// <returns> String that was read from the console. Note it will be null in case of Ctrl + Break.</returns>
    private string ReadFromConsole(string promptMessage = null)
    {
        // Show a prompt, and get input. ( Don't utilize DisplayProgress.DisplayText, because of newline )
        Console.Write(_readPrompt + promptMessage);
        return Console.ReadLine();
    }

    private void UpdateReadPrompt(string environment)
    {
        if (string.IsNullOrEmpty(environment))
            _readPrompt = string.Format(CultureInfo.InvariantCulture, "{0}> ", ProgramName);
        else
            _readPrompt = string.Format(CultureInfo.InvariantCulture, "{0}-{1}> ", ProgramName, environment);
    }

    private void SayGoodByeIfCtrlC()
    {
        // Let the thread where Console_CancelKeyPress is processed to do its job.
        // For strange reason, there seems no better way ...
        Thread.Sleep(256);

        if (CtrlBreakPressed)
        {
            if (!IsRestartRequested)
            {
                ConsoleDisplay.WriteInfo(
                    Invariant($"{ProgramName} - Cancellation has been requested per Ctrl + Break, wait ..."));
            }
            else
            {
                ConsoleDisplay.WriteInfo(
                    Invariant($"{ProgramName} - Restart has been requested per Ctrl + Shift + Break, wait ..."));
            }
            Thread.Sleep(2 * 1024);
        }
    }
    private static void SetIcons(byte[] resourceData)
    {
        Icon icon = ConsoleIconManager.CreateIcon(resourceData);
        ConsoleIconManager.SetConsoleWindowIcon(icon);
        ConsoleIconManager.SetTaskbarIcon(icon);
    }
    #endregion // Private Methods 
    #endregion // Methods

    #region Event_handlers

    /// <summary>  Handles the unhandled exception described by <paramref name="exceptionObject"/>. </summary>
    /// <param name="exceptionObject">  The exception object. </param>
    private static void ProcessUnhandledException(object exceptionObject)
    {
        if (exceptionObject is not Exception ex)
        {
            string exInfo = exceptionObject.ObjectTypeToReadable();
            ex = new NotSupportedException("Unhandled exception doesn't derive from System.Exception: " + exInfo);
        }
        string errorMessage = "Unhandled exception has occurred. " + ex.ExceptionDetails();
        ILogger logger = GetExistingOrCreateNewLogger();

        if (logger != null)
        {
            logger.Fatal(errorMessage, ex);
        }
        else
        {
            // just in case for any reason is logger not available
            ConsoleDisplay consoleDisplay = new();
            consoleDisplay.WriteError(errorMessage);
        }
    }

    /// <summary>
    /// Handles console exit events, such as close, log-off, and shutdown events, to ensure proper cleanup.
    /// </summary>
    /// <remarks>
    /// This method ensures that when certain control events occur (e.g., when the console is closed, logged off,
    /// or shut down), the application performs necessary cleanup, such as logging a warning and disposing of
    /// resources, if an instance exists. 
    /// </remarks>
    /// <param name="ctrlType"> The type of control event that triggered the handler (e.g., close, log-off, shutdown, etc.). </param>
    /// <returns> Returns true if the exit event is handled successfully, indicating that the event has been processed.
    /// Otherwise, returns false. </returns>
    private static bool ConsoleExitHandler(Kernel32.CtrlTypes ctrlType)
    {
        bool result = false;

        switch (ctrlType)
        {
            case Kernel32.CtrlTypes.CTRL_CLOSE_EVENT:
            case Kernel32.CtrlTypes.CTRL_LOGOFF_EVENT:
            case Kernel32.CtrlTypes.CTRL_SHUTDOWN_EVENT:
                if (HasInstance)
                {
                    Instance.Logger.Warn(Invariant($"{ProgramInfo} terminated irregular way, ctrlType = '{ctrlType}'"));
                    Instance.Dispose();
                    result = true;
                }
                break;

            // CTRL_BREAK_EVENT represents a control event that occurs when the user presses Ctrl + Break on the keyboard
            case Kernel32.CtrlTypes.CTRL_BREAK_EVENT:
                result = true;
                break;
        }

        return result;
    }

#pragma warning disable IDE0019    // Use pattern matching

    private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        IFileProcessingCommand iFileCmd;
        bool bShiftPressed = (0 != (User32.GetKeyState((int)Win32.VK.VK_SHIFT) & 0x8000));

        // Adjust arguments
        // The value ConsoleCancelEventArgs.Cancel false means that Ctrl+Break will terminate the current process immediately
        e.Cancel = true;

        // note what happened

        Program._ctrlBreakPressed = true;
        Program._restartRequested = bShiftPressed;

        // cancel current command
        if (null != (iFileCmd = CommandsRegister?.CurrentCommand as IFileProcessingCommand))
        {
            iFileCmd.SetExecutionCanceled();
        }
    }
#pragma warning restore IDE0019    // Use pattern matching
}
#endregion // Event_handlers

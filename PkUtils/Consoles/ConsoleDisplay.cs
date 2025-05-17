// Ignore Spelling: fg, bg
// 
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PK.PkUtils.Consoles;

/// <summary> A part of program code related to text input and output. </summary>
[CLSCompliant(true)]
public class ConsoleDisplay : IConsoleDisplay
{
    #region Fields

    private static SpinLock _spinLock = new();

    private int _spinnerIndex;
    private readonly char[] _spinner = ['/', '-', '\\', '|'];
    // optionally: private readonly char[] _spinner = ['<', '^', '>', 'v'];
    private readonly ConsoleColor _initialForeground = Console.ForegroundColor;
    private readonly ConsoleColor _initialBackground = Console.BackgroundColor;
    private const string _consoleErrorMessagePrefix = "ERROR: ";
    #endregion // Fields

    #region Constructor(s)

    /// <summary> Default constructor. </summary>
    public ConsoleDisplay()
    { }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Gets the initial foreground color. </summary>
    public ConsoleColor InitialForeground { get => _initialForeground; }

    /// <summary> Gets the initial background color. </summary>
    public ConsoleColor InitialBackground { get => _initialBackground; }
    #endregion // Properties

    #region Methods

    /// <summary> The very low-level actual writing, without synchronization. 
    ///           It is the caller who must do the synchronization. </summary>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void RawWriteText(string message, bool newLine)
    {
        if (newLine)
            Console.WriteLine(message);
        else
            Console.Write(message);
    }

    /// <summary> Executes thread-synchronized writing. </summary>
    /// <remars> Thread synchronization is needed to prevent colors of one writing to affect the other. </remars>
    private static void DoWriteText(ConsoleColor fgColor, ConsoleColor bgColor, string message, bool newLine = true)
    {
        bool lockTaken = false;

        try
        {
            _spinLock.Enter(ref lockTaken);
            ConsoleColor oldFg = Console.ForegroundColor;
            ConsoleColor oldBg = Console.BackgroundColor;

            Console.ForegroundColor = fgColor;
            Console.BackgroundColor = bgColor;

            RawWriteText(message, newLine);

            Console.ForegroundColor = oldFg;
            Console.BackgroundColor = oldBg;
        }
        finally
        {
            if (lockTaken) _spinLock.Exit();
        }
    }

    /// <summary> Executes thread-synchronized writing. </summary>
    /// <remars> Thread synchronization is needed to prevent colors of one writing to affect the other. </remars>
    private void DoWriteText(string message, bool newLine = true)
    {
        bool lockTaken = false;

        try
        {
            _spinLock.Enter(ref lockTaken);

            Console.ForegroundColor = InitialForeground;
            Console.BackgroundColor = InitialBackground;
            RawWriteText(message, newLine);
        }
        finally
        {
            if (lockTaken) _spinLock.Exit();
        }
    }

    private void DoWriteNextSpin()
    {
        bool lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);
            Console.Write(_spinner[_spinnerIndex]);
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop); // Move cursor back
            _spinnerIndex = (_spinnerIndex + 1) % _spinner.Length;
        }
        finally
        {
            if (lockTaken) _spinLock.Exit();
        }
    }
    #endregion // Methods

    #region IConsoleDisplay Members
    #region IDumper Members

    /// <inheritdoc/>
    public bool DumpText(string text)
    {
        WriteText(text, newLine: true);
        return true;
    }

    /// <inheritdoc/>
    public bool DumpWarning(string text)
    {
        WriteWarning(text, newLine: true);
        return true;
    }

    /// <inheritdoc/>
    public bool DumpError(string text)
    {
        WriteError(text, newLine: true);
        return true;
    }

    /// <inheritdoc/>
    public bool Reset()
    {
        Console.Clear();
        return true;
    }
    #endregion // IDumper Members

    /// <inheritdoc/>
    public void WriteLine()
    {
        DoWriteText(string.Empty, true);
    }

    /// <inheritdoc/>
    public void WriteText(string message, bool newLine = true)
    {
        DoWriteText(message, newLine);
    }

    /// <inheritdoc/>
    public void WriteText(ConsoleColor fgColor, string message, bool newLine = true)
    {
        ConsoleColor oldFg = Console.ForegroundColor;

        Console.ForegroundColor = fgColor;
        DoWriteText(fgColor, Console.BackgroundColor, message, newLine);
        Console.ForegroundColor = oldFg;
    }

    /// <inheritdoc/>
    public void WriteText(ConsoleColor fgColor, ConsoleColor bgColor, string message, bool newLine = true)
    {
        DoWriteText(fgColor, bgColor, message, newLine);
    }

    /// <inheritdoc/>
    public void WriteInfo(string message, bool newLine = true)
    {
        WriteText(Console.ForegroundColor, Console.BackgroundColor, message, newLine);
    }

    /// <inheritdoc/>
    public void WriteWarning(string message, bool newLine = true)
    {
        WriteText(ConsoleColor.Yellow, message, newLine);
    }

    /// <inheritdoc/>
    public void WriteSuccess(string message, bool newLine = true)
    {
        WriteText(ConsoleColor.Green, message, newLine);
    }

    /// <inheritdoc/>
    public void WriteError(string errorMessage, bool newLine = true)
    {
        // Should avoid writing newline with red background color
        WriteText(ConsoleColor.White, ConsoleColor.Red, _consoleErrorMessagePrefix + errorMessage, false);
        if (newLine)
        {
            WriteLine();
        }
    }

    /// <inheritdoc/>
    public void ShowNextSpin()
    {
        DoWriteNextSpin();
    }
    #endregion // IConsoleDisplay Members
}

// Ignore Spelling: Utils
//
using System;
using PK.PkUtils.Interfaces;

namespace PK.PkUtils.Consoles;


/// <summary> Interface for writing to console. </summary>
[CLSCompliant(true)]
public interface IConsoleDisplay : IDumper
{
    /// <summary> Gets the initial foreground color. </summary>
    ConsoleColor InitialForeground { get; }

    /// <summary> Gets the initial background color. </summary>
    ConsoleColor InitialBackground { get; }

    /// <summary> Write an newline. </summary>
    void WriteLine();

    /// <summary> Displays a text on console with specified background color and foreground color. </summary>
    ///
    /// <param name="message">  The message. </param>
    /// <param name="newLine">  (Optional) Should write newline. </param>
    void WriteText(string message, bool newLine = true);

    /// <summary> Displays a text on console with specified background color and foreground color. </summary>
    ///
    /// <param name="fgColor">  The foreground color. </param>
    /// <param name="message">  The message. </param>
    /// <param name="newLine">  (Optional) Should write newline. </param>
    void WriteText(ConsoleColor fgColor, string message, bool newLine = true);

    /// <summary> Displays a text on console with specified background color and foreground color. </summary>
    ///
    /// <param name="fgColor">  The foreground color. </param>
    /// <param name="bgColor">  The background color. </param>
    /// <param name="message">  The message. </param>
    /// <param name="newLine">  (Optional) Should write newline. </param>
    void WriteText(ConsoleColor fgColor, ConsoleColor bgColor, string message, bool newLine = true);

    /// <summary> Displays an information described by message. </summary>
    ///
    /// <param name="message">  The message. Can't be null or empty. </param>
    /// <param name="newLine">  (Optional) Should write newline. </param>
    void WriteInfo(string message, bool newLine = true);

    /// <summary> Displays a warning described by message. </summary>
    /// 
    /// <param name="message">  The message. Can't be null or empty. </param>
    /// <param name="newLine">  (Optional) Should write newline. </param>
    void WriteWarning(string message, bool newLine = true);

    /// <summary> Displays the success described by message. </summary>
    ///
    /// <param name="message"> The message. Can't be null or empty. </param>
    /// <param name="newLine"> (Optional) Should write newline. </param>
    void WriteSuccess(string message, bool newLine = true);

    /// <summary> Displays an error described by errorMessage. </summary>
    ///
    /// <param name="errorMessage"> Message describing the error. Can't be null or empty. </param>
    /// <param name="newLine">      (Optional) Should write newline. </param>
    void WriteError(string errorMessage, bool newLine = true);

    /// <summary> Move to next spin and show. </summary>
    void ShowNextSpin();
}

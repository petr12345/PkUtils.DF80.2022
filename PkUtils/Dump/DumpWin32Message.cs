// Ignore Spelling: Utils, Hexa
//
using System;
using System.Globalization;
using System.Windows.Forms;
using PK.PkUtils.Extensions;
using PK.PkUtils.WinApi;
using static System.FormattableString;

namespace PK.PkUtils.Dump;

/// <summary> A static utility to dump window32 messages to text. </summary>
public static class DumpWin32Message
{
    /// <summary> Converts a window message number to a human-readable hexa string. </summary>
    /// <param name="message"> The message. </param>
    /// <returns> Message as a string. </returns>
    public static string ToHexa(int message)
    {
        return string.Format(CultureInfo.InvariantCulture, "0x{0:X4}", message);
    }

    /// <summary> Converts a window message to a human-readable hexa string. </summary>
    /// <param name="message"> The message. </param>
    /// <returns> Message as a string. </returns>
    public static string ToHexa(Win32.WM message)
    {
        string result = ToHexa((int)message);
        return result;
    }

    /// <summary> Converts a Win32 message to a human-readable text. </summary>
    /// <param name="message"> The message. </param>
    /// <returns> Message as a string. </returns>
    public static string ToText(Win32.WM message)
    {
        string result = message.CheckIsDefinedValue().ToString();
        return result;
    }

    /// <summary> Converts a Win32 message to a human-readable text. </summary>
    /// <param name="message"> The message. </param>
    /// <returns> Message as a string. </returns>
    public static string ToText(int message)
    {
        string result;

        if (!Enum.IsDefined(typeof(Win32.WM), message))
            result = ToHexa(message);
        else
            result = ToText((Win32.WM)message);

        return result;
    }

    /// <summary> Converts a Win32 message to a human-readable text. </summary>
    /// <param name="message"> The message. </param>
    /// <param name="wParam"> Additional information about the message. </param>
    /// <param name="lParam"> Additional information about the message. </param>
    /// <returns> Message as a string. </returns>
    public static string ToText(int message, IntPtr wParam, IntPtr lParam)
    {
        string msgText = ToText(message);
        string wParamText = Invariant($"wParam = {wParam:X4}");
        string lParamText = Invariant($"lParam = {lParam:X4}");
        string result = Invariant($"{msgText}, {wParamText}, {lParamText}");

        return result;
    }

    /// <summary> Converts a Win32 message to a text. </summary>
    /// <param name="m"> A Message to process. </param>
    /// <returns> Message as a human-readable string. </returns>
    public static string ToText(this Message m)
    {
        string result = ToText(m.Msg, m.WParam, m.LParam);
        return result;
    }
}
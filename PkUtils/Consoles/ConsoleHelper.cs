// Ignore Spelling: Utils, Dict, Ctrl
//
using System;
using System.Collections.Generic;
using System.Linq;
using PK.PkUtils.Extensions;

namespace PK.PkUtils.Consoles;

/// <summary> Static class containing console-related utilities. </summary>
[CLSCompliant(true)]
public static class ConsoleHelper
{
    #region Typedefs

    /// <summary> Defines values that represent the return value of a dialog with the user. </summary>
    ///
    /// <remarks>
    /// Replaces the usage of DialogResult from WinForms, so the related components do not have to refer System.WinForms,
    /// but could communicate over console as well.
    /// </remarks>
    public enum ConsoleDialogResult
    {
        /// <summary> An enum constant representing the none option. </summary>
        None,
        /// <summary> An enum constant representing the ok option. </summary>
        OK,
        /// <summary> An enum constant representing the cancel option. </summary>
        Cancel,
        /// <summary> An enum constant representing the abort option. </summary>
        Abort,
        /// <summary> An enum constant representing the retry option. </summary>
        Retry,
        /// <summary> An enum constant representing the ignore option. </summary>
        Ignore,
        /// <summary> An enum constant representing the yes option. </summary>
        Yes,
        /// <summary> An enum constant representing the no option. </summary>
        No,
    };
    #endregion // Typedefs

    #region Fields

    private static IReadOnlyDictionary<ConsoleKey, ConsoleDialogResult> _keyToDialogResultDict;
    #endregion // Fields

    #region Properties

    /// <summary> Lazy initialization of dictionary mapping console keys to ConsoleDialogResult. </summary>
    public static IReadOnlyDictionary<ConsoleKey, ConsoleDialogResult> KeyToDialogResultDict
    {
        get
        {
            _keyToDialogResultDict ??= new Dictionary<ConsoleKey, ConsoleDialogResult>
                {
                    { ConsoleKey.O, ConsoleDialogResult.OK     },
                    { ConsoleKey.C, ConsoleDialogResult.Cancel },
                    { ConsoleKey.A, ConsoleDialogResult.Abort  },
                    { ConsoleKey.R, ConsoleDialogResult.Retry  },
                    { ConsoleKey.I, ConsoleDialogResult.Ignore },
                    { ConsoleKey.Y, ConsoleDialogResult.Yes    },
                    { ConsoleKey.N, ConsoleDialogResult.No     },
                };
            return _keyToDialogResultDict;
        }
    }
    #endregion // Properties

    #region Methods

    /// <summary> Wait until control c is pressed. </summary>
    public static void WaitUntilCtrlC()
    {
        Console.CancelKeyPress += ConsoleOnCancelKeyPress;

        while (Console.ReadLine() != null) { } // Ctrl+C -> null
    }

    /// <summary> Empty console buffer. </summary>
    public static void EmptyConsoleBuffer()
    {
        while (Console.KeyAvailable)
        {
            Console.ReadKey(false);
        }
    }

    /// <summary> Wait for one of keys, returns the ConsoleKey eventually pressed. </summary>
    ///
    /// <param name="keys"> The collection of possible keys. Can't be null or empty. </param>
    /// <param name="keyChar">  [out] The resulting key character, corresponds to returned value. </param>
    ///
    /// <returns> The pressed ConsoleKey. </returns>
    public static ConsoleKey WaitForOneOfKeys(IEnumerable<ConsoleKey> keys, out char keyChar)
    {
        if (keys.IsNullOrEmpty()) { throw new ArgumentNullException(nameof(keys)); }

        Nullable<ConsoleKey> result = null;

        keyChar = (char)0;
        do
        {
            ConsoleKeyInfo input = Console.ReadKey(true);
            if (keys.Contains(input.Key))
            {
                result = input.Key;
                keyChar = input.KeyChar;
            }
        } while (!result.HasValue);

        return result.Value;
    }

    /// <summary> Executes the console pseudo-dialog operation. </summary>
    ///
    /// <param name="consoleDisplay">   The <see cref="IConsoleDisplay"/> implementing object. Can't be null.</param>
    /// <param name="warnings">         The sequence of warnings. May be null or empty. </param>
    /// <param name="instructions">     The sequence of instructions. May be null or empty. </param>
    /// <param name="keysPossible">     The set of keys possible to end the dialog. </param>
    ///
    /// <returns> A ConsoleDialogResult. </returns>
    public static ConsoleDialogResult DoConsoleDialog(
        IConsoleDisplay consoleDisplay,
        IEnumerable<string> warnings,
        IEnumerable<string> instructions,
        IEnumerable<ConsoleKey> keysPossible)
    {
        ArgumentNullException.ThrowIfNull(consoleDisplay);

        warnings.SafeForEach(str => consoleDisplay.WriteWarning(str));
        instructions.SafeForEach(str => consoleDisplay.WriteInfo(str));

        ConsoleKey key = ConsoleHelper.WaitForOneOfKeys(keysPossible, out char keyChar);
        ConsoleDialogResult result = KeyToDialogResultDict.GetValueOrDefault(key);

        consoleDisplay.WriteInfo(keyChar.ToString().ToUpperInvariant());
        return result;
    }

    private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs args)
    {
        args.Cancel = true;
    }
    #endregion // Methods
}
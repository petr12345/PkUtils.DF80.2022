// Ignore Spelling: Gsl, Utils
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using PK.PkUtils.Extensions;


#pragma warning disable IDE0057 // Use range operator
#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.PkUtils.Cmd;

/// <summary> A complete implementation of <see cref="ICmdParametersProvider"/>. </summary>
/// 
/// <remarks>
/// Parses command-line arguments supporting both formats:
/// 1. Key-value pairs with colon delimiter: "/ParamName1:Value1 /ParamName2:Value2 -ParamName3:Value3"
/// 2. Key followed by space-delimited value: "/ParamName1 Value1 /ParamName2 Value2 /ParamName3 Value3"
/// Produces a dictionary (parameter name -> parameter value).
/// </remarks>
public class DualFormatCmdParametersProvider : BaseCmdParametersProvider, ICmdParametersProviderEx
{
    #region Fields

    private readonly IReadOnlyDictionary<string, string> _name2Value;
    private readonly List<string> _originalArgumentsOrder = [];
    private readonly StringComparer _argsNamesComparer = StringComparer.OrdinalIgnoreCase;

    private const char PREFIX_SLASH = '/';
    private const char PREFIX_HYPHEN = '-';
    private const char KEY_VALUE_DELIMITER = ':';

    /// <summary> The (fictive) switch value assigned to each switch, to distinguish options and switches.
    /// Using the fact that none incoming command-line string could equal to null.</summary>
    private const string _fictiveSwitchValue = null;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Initializes a new instance of the <see cref="DualFormatCmdParametersProvider"/> class with default
    /// settings. Parses command-line arguments retrieved from
    /// <see cref="Environment.GetCommandLineArgs"/>, excluding the executable file name.
    /// Switch syntax is enabled by default.
    /// </summary>
    public DualFormatCmdParametersProvider() : this(acceptSwitchesSyntax: true)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DualFormatCmdParametersProvider"/> class.
    /// Parses command-line arguments retrieved from <see cref="Environment.GetCommandLineArgs"/>, 
    /// excluding the executable file name.
    /// </summary>
    /// <param name="acceptSwitchesSyntax">
    /// Specifies whether to include switch syntax during parsing. If <c>true</c>, named arguments 
    /// without values (switches) are recognized as boolean flags.
    /// </param>
    /// <remarks>
    /// A switch is a named argument without a value, not preceded by '/' or '-'. Its presence 
    /// serves as a boolean indicator. 
    /// For example, if switch syntax is enabled, the command line:
    /// <code>/InputFile doc1.rtf nologo</code>
    /// will interpret 'nologo' as a boolean switch.
    /// </remarks>
    public DualFormatCmdParametersProvider(bool acceptSwitchesSyntax)
    {
        _name2Value = ParseArgs(Environment.GetCommandLineArgs(), acceptSwitchesSyntax, true);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DualFormatCmdParametersProvider"/> class using a specified
    /// sequence of command-line arguments, as passed to the program entry point.
    /// </summary>
    /// <remarks>
    /// A switch is a named argument without a value, not preceded by '/' or '-'. Its presence serves as a
    /// boolean indicator.  For example, in the command line:
    /// <code>/InputFile doc1.rtf nologo</code>
    /// 'nologo' is interpreted as a boolean switch.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> Thrown when the input argument <paramref name="args"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"> Thrown by <see cref="ParseArgs"/> when any string in <paramref name="args"/>
    /// is empty. </exception>
    /// <exception cref="InputLineValidationException"> Thrown by <see cref="ParseArgs"/> when a semantic error
    /// occurs (e.g., argument duplication). </exception>
    /// <param name="args"> A collection containing command-line arguments, typically received from the <c>Main</c>
    /// method. </param>
    /// <param name="acceptSwitchesSyntax"> Specifies whether to include switch syntax during parsing. If <c>true</c>,
    /// named arguments without values (switches) are recognized as boolean flags. </param>
    public DualFormatCmdParametersProvider(IEnumerable<string> args, bool acceptSwitchesSyntax)
    {
        _name2Value = ParseArgs(args, acceptSwitchesSyntax, false);
    }
    #endregion // Constructor(s)

    #region ICmdParametersProviderEx Members

    /// <summary> Gets all keys representing options (pairs /name value or -name value). </summary>
    public IEnumerable<string> Options
    {
        get
        {
            AssertValid();
            return AllParameters.Where(pair => pair.Value != _fictiveSwitchValue).Select(p => p.Key);
        }
    }

    /// <summary> Gets all keys representing switches ("values by just themselves"). </summary>
    public IEnumerable<string> Switches
    {
        get
        {
            AssertValid();
            return AllParameters.Where(pair => pair.Value == _fictiveSwitchValue).Select(p => p.Key);
        }
    }

    /// <summary> Gets all keys representing both Options and switches, in the same order as they were on the command line. </summary>
    public IEnumerable<string> OriginalArgumentsOrder
    {
        get
        {
            AssertValid();
            return _originalArgumentsOrder;
        }
    }

    /// <summary> return the dictionary of pairs arg -> value. </summary>
    public IReadOnlyDictionary<string, string> AllParameters
    {
        get
        {
            AssertValid();
            return _name2Value;
        }
    }

    /// <inheritdoc/>
    public StringComparer ArgumentNamesComparer => _argsNamesComparer;

    #endregion // ICmdParametersProviderEx Members

    #region Methods

    /// <summary>
    /// Get the boolean-like switch. Returns true if the switch has been present, false otherwise.
    /// </summary>
    /// <param name="switchName">The name of retrieved switch. Can't be null or empty. </param>
    /// <returns>true if the switch has been present, false otherwise.</returns>
    public bool GetSwitch(string switchName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(switchName);
        AssertValid();
        return Switches.Contains(switchName, ArgumentNamesComparer);
    }

    /// <summary>
    /// Virtual method validating the object instance.
    /// </summary>
    [Conditional("DEBUG")]
    public virtual void AssertValid()
    {
        ValidateMe();
    }

    /// <summary>
    /// Non-virtual method validating an instance of this type. 
    /// The reason of existence of this method is to avoid calling virtual method from constructor.
    /// </summary>
    [Conditional("DEBUG")]
    protected void ValidateMe()
    {
        Debug.Assert(_name2Value != null);
        Debug.Assert(_originalArgumentsOrder != null);
        Debug.Assert(_name2Value.Count <= _originalArgumentsOrder.Count);
        foreach (string key in _name2Value.Keys)
        {
            Debug.Assert(_originalArgumentsOrder.Contains(key, ArgumentNamesComparer));
        }
    }


    /// <summary> Normalizes the key. With current implementation, just returns the original key.
    ///            Derived class could override this, and for instance convert key to uppercase or lowercase.
    ///</summary>
    /// 
    /// <param name="key"> The key. Can't be null or empty. </param>
    /// <returns> A normalized value of <paramref name="key"/>. </returns>
    protected virtual string NormalizeKey(string key)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(key);
        return key;
    }

    /// <inheritdoc/>
    protected override string GetStringParameter(string name)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name);
        string unifiedName = NormalizeKey(name);

        return AllParameters.GetValueOrDefault(unifiedName);
    }

    private static bool StartsWithAnyArgPrefix(string arg)
    {
        return (!string.IsNullOrEmpty(arg)) && ((arg[0] == PREFIX_SLASH) || (arg[0] == PREFIX_HYPHEN));
    }

    private Dictionary<string, string> ParseArgs(
        IEnumerable<string> args,
        bool includeSwitches,
        bool skipFirstArg)
    {
        ArgumentNullException.ThrowIfNull(args);
        // Convert to list now, to prevent possible multiple enumerations
        List<string> listArgs = args.ToList();

        if (listArgs.Any(s => s == null))
        {
            throw new ArgumentException("Input argument cannot equal to null", nameof(args));
        }
        if (listArgs.Any(string.IsNullOrEmpty))
        {
            throw new ArgumentException("Input argument cannot be empty", nameof(args));
        }

        if (skipFirstArg)
        {
            // skip first arg - the exe file name
            string exeFileName = Assembly.GetEntryAssembly().Location;

            if (listArgs.IsEmpty())
            {
                string strErr = $"There is no input argument representing '{exeFileName}' to be skipped";
                throw new ArgumentException(strErr, nameof(args));
            }
            else
            {
#if DEBUG
                // should compare just filenames, since args.First() may have just relative path
                string argFileName = Path.GetFileName(listArgs[0]);

                exeFileName = Path.GetFileName(exeFileName);
                Debug.Assert(string.Equals(exeFileName, argFileName, StringComparison.OrdinalIgnoreCase));
#endif // DEBUG
                listArgs = listArgs.Skip(1).ToList();
            }
        }

        Dictionary<string, string> dict = new(ArgumentNamesComparer);

        for (int ii = 0, nCount = listArgs.Count; ii < nCount;)
        {
            string name, value;
            string arg = listArgs[ii++];
            string argNext = (ii < nCount) ? listArgs[ii] : null;

            if (StartsWithAnyArgPrefix(arg))
            {
                int delimiterIndex = arg.IndexOf(KEY_VALUE_DELIMITER, StringComparison.InvariantCulture);

                if (delimiterIndex >= 0)
                {
                    Debug.Assert(delimiterIndex >= 1); // since arg.StartsWith the prefix

                    if (delimiterIndex == 1)
                    {
                        // Handles the case of invalid argument like /:kuknastrejdu
                        string strErr = string.Format(CultureInfo.InvariantCulture,
                            "Invalid input argument '{0}' - no argument name precedes the prefix '{1}'",
                            arg, KEY_VALUE_DELIMITER);
                        throw new ArgumentException(strErr, nameof(args));
                    }
                    else
                    {
                        name = arg.Substring(1, delimiterIndex - 1);
                        value = arg.Substring(delimiterIndex + 1);
                    }
                }
                else if (StartsWithAnyArgPrefix(argNext))
                {
                    name = arg.Substring(1);
                    value = _fictiveSwitchValue;
                }
                else
                {
                    // May consider the argNext as a value for current argument.
                    // Hence, increment the index
                    ii++;
                    name = arg.Substring(1);
                    value = argNext;
                }
            }
            else
            {
                if (!includeSwitches) continue;
                name = arg;
                value = _fictiveSwitchValue;
            }

            if (string.IsNullOrEmpty(name))
            {
                // Now, the 'name' is an empty string, and arg is "-".
                // This handles the case where the quotes around the file or directory name have been omitted, 
                // and the folder or file name contains a hyphen; like
                // ConvertLibrary -inputLibrary C:\RAPIDLaunch\ProjectA_ACMScriptBundleTesting - Copy\Testing_ControlModule_Test_Libr2
                // 
                string error = $"Invalid empty input argument '{arg}'. Did you accidentally insert a space where it shouldn't be, or forget to enclose an argument value with a space in quotes?";
                throw new InputLineValidationException(error);
            }
            else
            {
                _originalArgumentsOrder.Add(name);
                name = NormalizeKey(name);
                if (!dict.TryAdd(name, value))
                {   // rather than ArgumentException, prefer specialized InputLineValidationException
                    throw new InputLineValidationException($"The argument '{name}' is duplicated");
                }
            }
        }

        return dict;
    }
    #endregion // Methods
}
#pragma warning restore IDE0305
#pragma warning restore IDE0057
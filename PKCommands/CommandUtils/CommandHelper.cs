// Ignore Spelling: Utils, Gsl, Whitespaced
// 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using PK.Commands.Interfaces;
using PK.PkUtils.Consoles;
using PK.PkUtils.DataStructures;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using static System.FormattableString;
using ILogger = log4net.ILog;

#pragma warning disable IDE0305 // Collection initialization can be simplified

namespace PK.Commands.CommandUtils;

/// <summary>   A helper class, performing various operations on command. </summary>
[CLSCompliant(true)]
public static class CommandHelper
{
    #region Fields

    /// <summary>   The usage prefix, used in <see cref="PrintDescription"/>. </summary>
    public const string USAGE_prefix = "USAGE:  ";

    /// <summary>   (Immutable) the example prefix. </summary>
    public const string EXAMPLE_prefix = "Example: ";

    private static ILogger _logger;

    private static readonly StringComparer _optionNamesComparer = StringComparer.OrdinalIgnoreCase;
    private const string _parameterPropertyName = nameof(IParameterBase.option);
    private const string _parameterDescriptionName = nameof(IParameterBase.description);
    private const string _optionValueName = nameof(IParameterOption<int>.optionValue);
    private const string _optionPossibleValuesName = nameof(IParameterOption<int>.possibleValues);
    private static readonly IEnumerable<string> _helpOptionsKeys = ["h", "?"];
    #endregion // Fields

    #region Properties

    /// <summary> Gets the 'usage' prefix. </summary>
    public static string UsagePrefix { get => USAGE_prefix; }

    /// <summary>   Gets the usage prefix white-spaced. </summary>
    public static string UsagePrefixWhitespaced { get => new(' ', USAGE_prefix.Length); }

    /// <summary>   Gets a list of names of the help options. </summary>
    public static IEnumerable<string> HelpOptionsNames { get => _helpOptionsKeys; }

    /// <summary> Gets the string comparer used for comparison of option names. </summary>
    /// <remarks>
    /// It would be much more better if instead of this we could refer to the string comparer that is used internally in
    /// argument of method <see cref="ValidateCommand"/> ( the input IReadOnlyDictionary parsedArgs).
    /// But there seem no easy way doing so.
    /// </remarks>
    private static StringComparer OptionNamesComparer { get => _optionNamesComparer; }

    private static ILogger Logger
    {
        get { return (_logger ??= CreateLogger()); }
    }
    #endregion // Properties

    #region Methods
    #region Public Methods

    /// <summary>
    /// Validates command parsed arguments <paramref name="parsedArgs"/> against command options
    /// <paramref name="commandOption"/>. In case of success, returns true ( and sets values to commandOption first ).
    /// In case of failure, writes error to log and to <paramref name="display"/>.
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException"> Passed when any of required arguments is null. </exception>
    ///
    /// <param name="cmd">              The command. Can't be null. </param>
    /// <param name="parsedArgs">       The parsed arguments of command. Can't be null. </param>
    /// <param name="commandOption">    The command option. Can't be null. </param>
    /// <param name="display">         The callback interface with displaying capability. May be null. </param>
    ///
    /// <returns>
    /// Successful IComplexResult if arguments were validated against command options successfully, and command processing may continue.
    /// Null if there was only one argument, help option "h". Such arguments are ok, but processing should not continue.
    /// Failed IComplexResult if arguments validation failed, and command processing should stop.
    /// </returns>
    public static IComplexResult ValidateCommand<TErrorCode>(
        ICommand<TErrorCode> cmd,
        IReadOnlyDictionary<string, string> parsedArgs,
        ICommandOptions commandOption,
        IConsoleDisplay display)
    {
        ArgumentNullException.ThrowIfNull(cmd);
        ArgumentNullException.ThrowIfNull(parsedArgs);
        ArgumentNullException.ThrowIfNull(commandOption);

        string cmdName = cmd.Name;
        Type optionsType = commandOption.GetType();
        IReadOnlyList<FieldInfo> fields = [.. optionsType.GetFields()];
        IReadOnlyList<string> validKeys = fields.Select(fi => fi.Name).ToList();
        IReadOnlyList<string> parsedArgsKeys = parsedArgs.Keys.ToList();

        // if no arguments passed on command line at all 
        if (HasNoArguments(parsedArgs))
        {
            // if no arguments given, and possible arguments list contains 'h' or '?'
            if (validKeys.IsNullOrEmpty() || HasHelpOptionOnly(validKeys))
            {
                // Use case for commands like clear and ls commands :  no arguments; only valid is 'h'. Execute the command.
                return ComplexResult.OK;
            }
            else if (HasHelpOption(validKeys))
            {
                return null;
            }
            else
            {
                string errorMessage = $"Command '{cmdName}' requires at least one command option";
                LogAndDisplayValidationError(display, errorMessage);
                return ComplexResult.CreateFailed(errorMessage);
            }
        }

        // only argument is 'h' help option - show help
        if (HasHelpOptionOnly(parsedArgsKeys))
        {
            return null;
        }

        // check only valid keys are passed
        List<string> difference = parsedArgsKeys.Except(validKeys, OptionNamesComparer).ToList();

        if (difference.Count > 0)
        {
            List<string> listErrors = difference.Select(optionName =>
                Invariant($"Unrecognized option '-{optionName}', not defined in command '{cmdName}'.")).ToList();
            listErrors.ForEach(s => LogAndDisplayValidationError(display, s));
            string errorMessage = listErrors.Join(" ");  // to let the message to contain all of them

            return ComplexResult.CreateFailed(errorMessage);
        }

        // Check if values passed are valid and set the value. Also, check if all 'mandatory' were present
        HashSet<string> presentFields = new(OptionNamesComparer);
        IEnumerable<IEnumerable<string>> mandOpts = cmd.MandatoryOptions;
        IReadOnlyList<IReadOnlyList<string>> mandsList = mandOpts.Select(x => x.ToList()).ToList();
        IReadOnlyList<HashSet<string>> mandsSets = mandsList.Select(s => new HashSet<string>(s, OptionNamesComparer)).ToList();

        foreach (FieldInfo optionField in fields.Where(f => parsedArgs.ContainsKey(f.Name)))
        {
            string validationError = null;
            object optionObj = optionField.GetValue(commandOption);
            string fieldName = optionField.Name;
            string inputValue = parsedArgs[fieldName];

            if (optionObj is IParameterSwitch paramSwitch)
            {
                // Try parsing the value of the switch if it is provided, 
                // or use the value "true" due to the presence of the switch

                if (string.IsNullOrEmpty(inputValue))
                    paramSwitch.switchValue = true;
                else if (bool.TryParse(inputValue, out bool boolValue))
                    paramSwitch.switchValue = boolValue;
                else
                    validationError = $"Switch '{fieldName}' has incorrect value '{inputValue}'.";
            }
            else if (ImplementsGenericInterface(optionObj, typeof(IParameterOption<>)))
            {
                // Everything else should be generic IParameterOption{T}
                if (IsValidOptionValue(optionObj, inputValue))
                {
                    SetOptionValue(optionObj, inputValue);
                }
                else
                {
                    Type optionObjType = optionObj.GetType();
                    object optionNameObj = optionObjType.GetProperty(_parameterPropertyName).GetValue(optionObj, null);
                    string optionName = optionNameObj.NullSafe(o => o.ToString());

                    validationError = $"Not a valid option '{optionName}' value : '{inputValue}'";
                }
            }
            else
            {
                Logger.Warn($"Parameter '{fieldName}' has unexpected representing type '{optionObj.GetType()}'");
                continue;
            }

            if (validationError is null)
            {
                // For all mandatory sets: remove from the set of arguments the current one ( if present )
                mandsSets.ForEach(s => s.Remove(fieldName));
                presentFields.Add(fieldName);
            }
            else
            {
                LogAndDisplayValidationError(display, validationError);
                return ComplexResult.CreateFailed(validationError);
            }
        }

        // If there are any mandatory sets, and all of them are not emptied yet
        if (mandsSets.Any() && mandsSets.All(s => !s.IsNullOrEmpty()))
        {
            // For the error message generation, select the arguments set with the fewest missing options.
            // Note: The second idea, to select the set with the most matching options,
            // is not feasible, as some optional options might also be listed in that case)

            HashSet<string> bestMatchMissing = mandsSets.OrderBy(s => s.Count).ThenBy(s => s.Intersect(presentFields).Count()).First();
            IEnumerable<string> namesInQuotes = bestMatchMissing.Select(x => Invariant($"'-{x}'"));
            string opts = (bestMatchMissing.Count > 1) ? "options" : "option";
            string errorMessage = Invariant($"Command '{cmdName}' mandatory {opts} {namesInQuotes.Join(",")} not found in current input arguments.");

            LogAndDisplayValidationError(display, errorMessage);
            return ComplexResult.CreateFailed(errorMessage);
        }

        return ComplexResult.OK;
    }

    /// <summary> Get text descriptionInfo of individual command options <paramref name="commandOption"/>. </summary>
    ///
    /// <param name="usage">  The usage of command. </param>
    /// <param name="commandOption">  The command options. Can't be null.</param>
    ///
    /// <returns>  A string containing complete descriptionInfo, including usage. </returns>
    public static string PrintDescription(string usage, ICommandOptions commandOption)
    {
        ArgumentNullException.ThrowIfNull(usage);
        ArgumentNullException.ThrowIfNull(commandOption);

        Type optionsType = commandOption.GetType();
        FieldInfo[] optionsTypeFields = optionsType.GetFields();
        StringBuilder sb = new(Environment.NewLine + USAGE_prefix + usage);

        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}{0}[Options]{0}{0}", Environment.NewLine);

        // Perform two parsing of fields, in the first cycle just find-out maximal length of paramNameString,
        // in the second use it for alignment.
        //
        for (int maxNameLength = 0, ii = 0; ii < 2; ii++)
        {
            foreach (FieldInfo optionFields in optionsTypeFields)
            {
                object parameterObj = optionFields.GetValue(commandOption);
                Type parameterObjType = parameterObj.GetType();
                object paramNameObj = parameterObjType.GetProperty(_parameterPropertyName).GetValue(parameterObj, null);
                string paramNameString = paramNameObj.NullSafe(o => o.ToString());
                int paramNameLength = paramNameString.Length;

                if (ii == 0)
                {
                    maxNameLength = Math.Max(maxNameLength, paramNameLength);
                }
                else
                {
                    PropertyInfo descriptionInfo = parameterObjType.GetProperty(_parameterDescriptionName);
                    string description = descriptionInfo.NullSafe(d => d.GetValue(parameterObj, null).ToString());
                    string padd = new(' ', maxNameLength - paramNameLength);
                    string basicInfo = $"-{paramNameString}:{padd}  {description}";

                    sb.Append(basicInfo);
                    if (parameterObj is IParameterSwitch)
                    {
                        sb.AppendLine();
                    }
                    else if (ImplementsGenericInterface(parameterObj, typeof(IParameterOption<>)))
                    {
                        Type propertyType = parameterObjType.GetProperty(_optionValueName).PropertyType;
                        string[] possibleValues = GetPossibleOptionValues(parameterObj);

                        if (!possibleValues.IsNullOrEmpty() && (Type.GetTypeCode(propertyType) != TypeCode.Boolean))
                        {
                            sb.AppendFormat(CultureInfo.InvariantCulture, " <Values: {0}>", possibleValues.Join());
                        }
                        sb.AppendLine();
                    }
                    else
                    {
                        Logger.Warn($"Parameter '{paramNameString}' has unexpected representing type '{parameterObjType.GetType()}'");
                    }
                }
            }
        }

        return sb.ToString();
    }
    #endregion // Public Methods

    #region Private Methods

    private static bool IsHelpOptionKey(string key) => _helpOptionsKeys.Contains(key, OptionNamesComparer);

    private static bool HasHelpOption(IReadOnlyCollection<string> optionKeys)
    {
        Debug.Assert(optionKeys != null);
        return optionKeys.Any(IsHelpOptionKey);
    }

    private static bool HasHelpOptionOnly(IReadOnlyCollection<string> optionKeys)
    {
        Debug.Assert(optionKeys != null);
        return (optionKeys.Count == 1) && IsHelpOptionKey(optionKeys.First());
    }

    private static bool HasNoArguments(IReadOnlyDictionary<string, string> parsedArgs)
    {
        return parsedArgs.IsNullOrEmpty();
    }

    private static bool IsValidOptionValue(Object optionObj, string inputValue)
    {
        string[] possibleValues = GetPossibleOptionValues(optionObj);
        bool result = true;  // ok so far

        if (possibleValues != null)
        {
            /* possibleValues = possibleValues.Where(s => s != "None").ToArray(); removal of "None" removal */
            result = possibleValues.Any(x => string.Compare(x, inputValue, StringComparison.OrdinalIgnoreCase) == 0);
        }

        return result;
    }

    private static void SetOptionValue(Object optionObj, string inputValue)
    {
        PropertyInfo optionValue = optionObj.GetType().GetProperty(_optionValueName);
        optionValue.SetValue(optionObj, CoerceArgument(optionValue.PropertyType, inputValue));
    }

    private static string[] GetPossibleOptionValues(Object optionObj)
    {
        Debug.Assert(optionObj != null);
        PropertyInfo possibleValuesInfo = optionObj.GetType().GetProperty(_optionPossibleValuesName);
        string[] result = null;

        if (possibleValuesInfo != null)
        {
            result = (string[])possibleValuesInfo.GetValue(optionObj, null);
        }

        return result;
    }

    private static object CoerceArgument(Type requiredType, string inputValue)
    {
        TypeCode requiredTypeCode = Type.GetTypeCode(requiredType);
        string exceptionMessage = string.Format(CultureInfo.InvariantCulture,
            "Cannot coerce the input argument '{0}' to required type '{1}'",
            inputValue, requiredType.Name);

        object result = null;
        switch (requiredTypeCode)
        {
            case TypeCode.String:
                result = inputValue;
                break;

            case TypeCode.Boolean:
                if (inputValue == null)
                {
                    result = false;
                }
                else if (bool.TryParse(inputValue, out bool trueFalse))
                {
                    result = trueFalse;
                }
                else
                {
                    throw new ArgumentException(exceptionMessage);
                }
                break;

            case TypeCode.DateTime:
                if (DateTime.TryParse(inputValue, out DateTime dateValue))
                {
                    result = dateValue;
                }
                else
                {
                    throw new ArgumentException(exceptionMessage);
                }
                break;

            case TypeCode.Decimal:
                if (Decimal.TryParse(inputValue, out decimal decimalValue))
                {
                    result = decimalValue;
                }
                else
                {
                    throw new ArgumentException(exceptionMessage);
                }
                break;

            case TypeCode.Double:
                if (Double.TryParse(inputValue, out double doubleValue))
                {
                    result = doubleValue;
                }
                else
                {
                    throw new ArgumentException(exceptionMessage);
                }
                break;

            case TypeCode.Single:
                if (Single.TryParse(inputValue, out float singleValue))
                {
                    result = singleValue;
                }
                else
                {
                    throw new ArgumentException(exceptionMessage);
                }
                break;

            case TypeCode.UInt32:
                if (UInt32.TryParse(inputValue, out uint uInt32Value))
                {
                    result = uInt32Value;
                }
                else
                {
                    throw new ArgumentException(exceptionMessage);
                }
                break;

            case TypeCode.Int32:
                var int32Value = Enum.Parse(requiredType, inputValue, true);
                result = int32Value;
                break;

            case TypeCode.Object:
                if (requiredType.IsArray)
                {
                    result = inputValue.Split(',');
                }
                break;

            default:
                throw new ArgumentException(exceptionMessage);
        }

        return result;
    }

    private static bool ImplementsGenericInterface(object obj, Type genericInterfaceType)
    {
        if (obj is null || genericInterfaceType is null || !genericInterfaceType.IsGenericTypeDefinition)
            return false;

        return obj.GetType()
                  .GetInterfaces()
                  .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterfaceType);
    }

    private static void LogAndDisplayValidationError(IConsoleDisplay display, string message)
    {
        Logger.Error(message);
        if (display != null)
        {
            DisplayValidationError(display, message);
        }
    }

    private static void DisplayValidationError(IConsoleDisplay display, string message)
    {
        if (display != null)
        {
            display.WriteLine();
            display.WriteError($"Validation error: {message}");
            display.WriteLine();
        }
    }

    private static ILogger CreateLogger()
    {
        return log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
    #endregion // Private Methods
    #endregion // Methods
}
#pragma warning restore IDE0305
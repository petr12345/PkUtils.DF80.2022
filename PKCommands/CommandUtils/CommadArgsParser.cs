// Ignore Spelling: Utils, Gsl
// 
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PK.Commands.CommandUtils;

#pragma warning disable IDE0079    // Remove unnecessary suppressions
#pragma warning disable SYSLIB1045 // Use 'GeneratedRegexAttribute' to generate the regular expression implementation at compile-time.

/// <summary>
/// A class used to parse command-line arguments into a dictionary of parameters and values.
/// </summary>
internal class CommandArgsParser
{
    #region Fields

    /// <summary>
    /// Dictionary to store parsed command-line arguments and their associated values.
    /// </summary>
    private readonly Dictionary<string, string> _parameters;

    /// <summary>
    /// List to maintain the original order of parameters as they appeared in the input.
    /// </summary>
    private readonly IList<string> _originalParametersOrder = [];

    /// <summary>
    /// Default value assigned to parameters that do not have an explicit value.
    /// </summary>
    private const string _fictiveParameterValue = "true";

    /// <summary>
    /// Regular expression for splitting arguments and values.
    /// Supports "-", "--", "/", and "=" as delimiters.
    /// </summary>
    private static readonly Regex _plitterRegex = new(@"^-{1,2}|^/|=",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Regular expression for removing enclosing quotes from argument values.
    /// Handles both single and double quotes.
    /// </summary>
    private static readonly Regex _removerRegex = new(@"^['""]?(.*?)['""]?$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandArgsParser"/> class.
    /// Parses the provided command-line arguments into key-value pairs.
    /// </summary>
    /// <param name="args">The command-line arguments to parse.</param>
    /// <param name="comparer">The comparer used for dictionary key comparisons. Can be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
    internal CommandArgsParser(IEnumerable<string> args, IEqualityComparer<string> comparer)
    {
        ArgumentNullException.ThrowIfNull(args);

        _parameters = new Dictionary<string, string>(comparer);

        string parameter = null;

        foreach (string txt in args)
        {
            string[] parts = _plitterRegex.Split(txt, 3);

            switch (parts.Length)
            {
                case 1:
                    // Argument is a value assigned to a previously stored parameter
                    AddParameterIfNeeded(parameter, parts[0]);
                    parameter = null;
                    break;

                case 2:
                    // Argument is a parameter without an explicit value
                    AddParameterIfNeeded(parameter, _fictiveParameterValue);
                    parameter = parts[1];
                    break;

                case 3:
                    // Argument is a parameter with an explicit value
                    AddParameterIfNeeded(parameter, _fictiveParameterValue);
                    parameter = parts[1];
                    parts[2] = _removerRegex.Replace(parts[2], "$1");
                    AddParameter(parameter, parts[2]);
                    parameter = null;
                    break;
            }
        }

        // Finalize parsing for any parameter waiting for a value
        if (parameter != null)
        {
            AddParameterIfNeeded(parameter, _fictiveParameterValue);
        }
    }

    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Gets a read-only dictionary of parsed parameters and their associated values.
    /// </summary>
    internal IReadOnlyDictionary<string, string> Parameters => _parameters;

    /// <summary>
    /// Gets the original order in which parameters appeared in the input.
    /// </summary>
    internal IEnumerable<string> OriginalParametersOrder => _originalParametersOrder;

    #endregion // Properties

    #region Private Methods

    /// <summary>
    /// Adds a parameter and its value to the dictionary if the parameter is not null and hasn't been added yet.
    /// Also records the parameter in the original order list.
    /// </summary>
    /// <param name="parameter">The parameter to add.</param>
    /// <param name="value">The value associated with the parameter.</param>
    private void AddParameterIfNeeded(string parameter, string value)
    {
        if (parameter != null && _parameters.TryAdd(parameter, value))
        {
            _originalParametersOrder.Add(parameter);
        }
    }

    /// <summary>
    /// Adds a parameter and its value directly to the dictionary if it hasn't been added yet.
    /// Also records the parameter in the original order list.
    /// </summary>
    /// <param name="parameter">The parameter to add.</param>
    /// <param name="value">The value associated with the parameter.</param>
    private void AddParameter(string parameter, string value)
    {
        if (_parameters.TryAdd(parameter, value))
        {
            _originalParametersOrder.Add(parameter);
        }
    }
    #endregion // Private Methods
}

#pragma warning restore SYSLIB1045
#pragma warning restore IDE0079
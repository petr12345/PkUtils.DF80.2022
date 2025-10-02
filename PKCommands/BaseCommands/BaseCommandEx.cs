using System;
using System.Collections.Generic;
using System.Diagnostics;
using PK.Commands.Interfaces;
using PK.PkUtils.Consoles;
using PK.PkUtils.Extensions;
using PK.PkUtils.Interfaces;
using ILogger = log4net.ILog;

namespace PK.Commands.BaseCommands;

/// <summary>  A generic base command, supplying more functionality to derived concrete commands. </summary>
///
/// <typeparam name="TOptions"> Type of the options used. </typeparam>
/// <typeparam name="TErrorCode"> The command execution error code, that extends error information. </typeparam>
[CLSCompliant(true)]
public abstract class BaseCommandEx<TOptions, TErrorCode> : BaseCommand<TOptions, TErrorCode>, ICommandEx<TErrorCode>
    where TOptions : ICommandOptions, new()
{
    #region Fields

    private TErrorCode _lastError;
    private IReadOnlyDictionary<string, string> _validatedArguments;
    private TOptions _options = new();
    private readonly ILogger _logger;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>   Specialized constructor for use only by derived class. </summary>
    /// <exception cref="NullReferenceException"> Thrown when <paramref name="logger"/> is null. </exception>
    /// <param name="logger"> The logger. Can't be null. </param>
    protected BaseCommandEx(ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }
    #endregion // Constructor(s)

    #region Properties
    /// <summary>   Gets the logger. </summary>
    protected ILogger Logger
    {
        get { Debug.Assert(_logger != null); return _logger; }
    }

    /// <summary>  Gets options for this command. 
    ///            A type-safe non-virtual variant of <see cref="GetCommandOptions"/>
    /// </summary>
    protected TOptions Options
    {
        get { Debug.Assert(_options != null); return _options; }
        set { Debug.Assert(value != null); _options = value; }
    }
    #endregion // Properties

    #region Methods

    /// <inheritdoc/>
    public override ICommandOptions GetCommandOptions()
    {
        return Options;
    }

    /// <inheritdoc/>
    protected override TOptions ResetCommandOptions(TOptions newOptions = default)
    {
        TOptions oldVal = this.Options;

        this.Options = (newOptions ?? new TOptions());
        return oldVal;
    }

    /// <summary>   Sets the last error to <paramref name="value"/>. </summary>
    /// <param name="value"> The value to be assigned. </param>
    protected void SetLastError(TErrorCode value)
    {
        _lastError = value;
    }

    /// <summary> Resets the last error. </summary>
    protected void ResetLastError() => SetLastError(default);

    /// <summary>
    /// An utility method - join values for reading in a single string, optionally including the curly braces.
    /// </summary>
    ///
    /// <typeparam name="T">    Generic type parameter - type of values in input sequence. </typeparam>
    /// <param name="values">   The sequence values. </param>
    /// <param name="separator"> (Optional) The separator character. </param>
    /// <param name="includeCurlyBraces"> (Optional) True to include, false to not include the curly braces. </param>
    ///
    /// <returns>   A resulting string, like "{LOCAL|BUILD|PROD}". </returns>
    protected static string Join4Reading<T>(
        IEnumerable<T> values,
        char separator = '|',
        bool includeCurlyBraces = true)
    {
        ArgumentNullException.ThrowIfNull(values);
        string result = values.Join(separator.ToString());

        if (includeCurlyBraces)
        {
            result = "{" + result + "}";
        }

        return result;
    }
    #endregion // Methods

    #region ICommandEx Members
    #region ICommand Members

    /// <inheritdoc/>
    public override IComplexResult Validate(
        IReadOnlyDictionary<string, string> parsedArgs,
        IConsoleDisplay display)
    {
        IComplexResult result;

        _validatedArguments = null;
        result = base.Validate(parsedArgs, display);

        if ((result == null) || result.Success)
        {
            _validatedArguments = parsedArgs;
        }

        return result;
    }
    #endregion // ICommand Members

    /// <summary> Gets or sets the validated arguments. </summary>
    public IReadOnlyDictionary<string, string> ValidatedArguments
    {
        get { return _validatedArguments; }
        protected set { _validatedArguments = value; }
    }

    /// <inheritdoc/>
    public TErrorCode GetLastError() => _lastError;

    #endregion // ICommandEx Members
}
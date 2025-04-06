/***************************************************************************************************************
*
* FILE NAME:   .\Utils\CommandLineInfoEx.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: The class CommandLineInfoEx 
*
**************************************************************************************************************/

// Ignore Spelling: Utils, dict
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using PK.PkUtils.Interfaces;


namespace PK.PkUtils.Utils;


/// <summary> CommandLineInfoEx parses arbitrary command-line contents, while recognizing options and
/// switches.
/// <para>
/// <b>--- Terminology clarification:  </b><br/>
/// In general, input arguments are of two types, that we call options and switches. <br/>
/// 
/// <list type="bullet">
/// <item><b>options (having values):</b><br/>
/// Such option is named argument with value; its name preceded by / or -</item>
/// 
/// <item><b>switches (without values):</b><br/>
/// Switch is a named argument without value,  its presence  itself is a boolean indicator</item>
/// </list>
/// 
/// For example, if you invoke your application like
/// <code>
///     "myApp.exe -f c:\outputFile.txt nologo"
/// </code>
/// the 'f' here is an option argument with value 'c:\outputFile.txt', and 'nologo' is a boolean
/// switch.
/// </para>
/// <para>
/// This terminology introduced here is not quite standard, primarily since it seems there is
/// just NO quite standard terminology. So, this is introduced to have at least something...
/// </para>
/// 
/// <para>
/// <b>--- CommandLineInfoEx usage:  </b><br/>
/// 
/// 1/ In the application initialization, type the code like<br/>
/// <code>
/// public static int Main(string[] args)
/// {
///   CommandLineInfoEx cmdinfo = new CommandLineInfoEx();
///   cmdinfo.ParseCommandLine(args, false);
/// </code>
/// 
/// 2/ After parsing, to get the value of a string option, type<br/>
/// <code>
///   string filename;
///   if (cmdinfo.GetOption("f", filename)) {
///     // now filename is string following -f option
///   }
/// </code>
/// 
/// 3/ To get the value of boolean-like switch, do the following<br/>
/// <code>
///   if (cmdinfo.GetOption("nologo")) {
///     // handle it
///   }
/// </code> 
/// </para>
/// </summary>
///
/// <remarks> The implementation idea is inspired by a C++ class CommandLineInfoEx, published in
/// <a href="http://www.microsoft.com/msj/1099/c/c1099.aspx">MSJ October 1999  C++ Q&amp;A
/// </a> by Paul DiLascia.  <br/> 
/// </remarks>
///
/// <seealso href="http://www.microsoft.com/msj/1099/c/c1099.aspx"> MSJ October 1999  C++ Q&amp;A</seealso>
/// <seealso href="http://pauldilascia.com/PixieLib.asp.html"> PixieLib -  
/// C++ Class Library for MFC — Copyright 2005 Paul DiLascia</seealso>
[CLSCompliant(true)]
public class CommandLineInfoEx : IDeepCloneable<CommandLineInfoEx>
{
    #region Fields

    /// <summary>
    /// The dictionary storing string pairs (options/value) or (switch/empty string)
    /// ( empty string is the imaginary (fictive, dummy) value for the switch argument).
    /// </summary>
    protected Dictionary<string, string> _dict = [];

    /// <summary> The auxiliary field, keeping last option value, for the purpose of parsing. </summary>
    private string _LastOption;

    /// <summary> The backing field of <see cref="IsCaseSensitive"/> property. </summary>
    private bool _caseSensitive = _defaultCaseSens;

    /// <summary>
    /// The default value assigned for the option value during parsing, till actual value is determined.
    /// </summary>
    private string _defaultOptionValue = _defaultDeaultOptionValue;

    /// <summary> The default value of <see cref="IsCaseSensitive"/> property. </summary>
    private const bool _defaultCaseSens = true;

    /// <summary> The default value of <see cref="DefaultOptionValue"/> property. </summary>
    private const string _defaultDeaultOptionValue = "TRUE";

    /// <summary> The (fictive) switch value assigned to each switch, to distinguish options and switches.
    /// Using the fact that none incoming command-line string could equal to null.</summary>
    private const string _fictiveSwitchValue = null;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default argument-less constructor, initializes the "empty" instance.
    /// Later you can call <see cref="ParseCommandLine(IEnumerable{string}, bool, string)"/>
    /// </summary>
    public CommandLineInfoEx()
    {
    }

    /// <summary>
    /// Constructor which initializes the property <see cref="IsCaseSensitive"/> by
    /// <paramref name="caseSensitive"/>
    /// </summary>
    ///
    /// <param name="caseSensitive">    The value to be assigned to _CaseSensitive field. </param>
    /// <param name="defaultOptionValue"> The value assigned to <see cref="DefaultOptionValue"/>. 
    /// Can't be null. </param>
    public CommandLineInfoEx(bool caseSensitive, string defaultOptionValue)
    {
        ArgumentNullException.ThrowIfNull(defaultOptionValue);

        _caseSensitive = caseSensitive;
        _defaultOptionValue = defaultOptionValue;
    }

    /// <summary>
    /// Constructor which initializes by parsing the array of arguments,
    /// with the case-sensitivity default value true.
    /// ( default value is specified in the field _bDefaultCaseSens = true; )
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Passed by called method ParseCommandLine
    /// when the input argument <paramref name="args"/> is null.</exception>
    /// <exception cref="ArgumentException"> Passed by called method ParseCommandLine when any of strings
    /// in the input <paramref name="args"/> is null.</exception>
    /// 
    /// <param name="args">Collection that contains command-line arguments, which are coming from Main</param>
    public CommandLineInfoEx(IEnumerable<string> args)
        : this()
    {
        ParseCommandLine(args);
    }

    /// <summary>
    /// Constructor which initializes the object by parsing the array of arguments,
    /// with the case-sensitivity specified by <paramref name="caseSensitive"/> argument.
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Passed by called method ParseCommandLine
    /// when the input argument <paramref name="args"/> is null.</exception>
    /// <exception cref="ArgumentException"> Passed by called method ParseCommandLine when any of strings
    /// in the input <paramref name="args"/> is null.</exception>
    /// 
    /// <param name="args"> Collection that contains command-line arguments, which are coming from Main</param>
    /// <param name="caseSensitive">The initial value for the <see cref="IsCaseSensitive"/>property.</param>
    /// <param name="defaultOptionValue"> The value assigned to <see cref="DefaultOptionValue"/>.
    /// Can't be null. </param>
    public CommandLineInfoEx(IEnumerable<string> args, bool caseSensitive, string defaultOptionValue)
        : this(caseSensitive, defaultOptionValue)
    {
        ParseCommandLine(args);
    }

    /// <summary>
    /// Protected constructor, creates the deep copy of given CommandLineInfoEx 
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when the input argument
    /// <paramref name="src"/> is null.</exception>
    /// 
    /// <param name="src">The object which will be copied by this copy-like constructor. 
    /// Must not equal to null. </param>
    protected CommandLineInfoEx(CommandLineInfoEx src)
    {
        ArgumentNullException.ThrowIfNull(src);

        this._dict = new Dictionary<string, string>(src.DictArgs);
        this._caseSensitive = src.IsCaseSensitive;
        this._defaultOptionValue = src.DefaultOptionValue;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary> Returns true if the dictionary of arguments is empty. </summary>
    public bool IsEmpty { get => (DictArgs.Count == 0); }

    /// <summary>
    /// The current case-sensitivity value. It is determined from the value specified either in the constructor,
    /// or later when calling ParseCommandLine overload.
    /// </summary>
    public bool IsCaseSensitive { get => _caseSensitive; }

    /// <summary> Gets the value assigned to <see cref="_defaultOptionValue"/>.
    /// That value will assigned for the option(s) value during parsing, till actual value is determined,
    /// and remains as that if no actual value is specified. 
    /// </summary>
    public string DefaultOptionValue { get => _defaultOptionValue; }

    /// <summary>
    /// return the dictionary of pairs arg -> value. Includes both options and switches.
    /// </summary>
    public IDictionary<string, string> DictArgs
    {
        get { Debug.Assert(_dict != null); return _dict; }
    }

    /// <summary>
    /// return the readonly dictionary of pairs arg -> value. Includes both options and switches.
    /// </summary>
    public IReadOnlyDictionary<string, string> ParsedArgsDictionary
    {
        get { Debug.Assert(_dict != null); return _dict; }
    }

    /// <summary> Gets all keys representing options (pairs /name value or -name value). </summary>
    public IEnumerable<string> Options
    {
        get { return DictArgs.Where(pair => pair.Value != _fictiveSwitchValue).Select(p => p.Key); }
    }

    /// <summary> Gets all keys representing switches ("values by just themselves"). </summary>
    public IEnumerable<string> Switches
    {
        get { return DictArgs.Where(pair => pair.Value == _fictiveSwitchValue).Select(p => p.Key); }
    }
    #endregion // Properties

    #region Methods
    #region Public Methods

    /// <summary>
    /// Clears the dictionary of arguments, and assigns the <see cref="_caseSensitive"/> and
    /// <see cref="_defaultOptionValue"/>fields to the default values.
    /// </summary>
    public virtual void Clear()
    {
        DictArgs.Clear();

        this._LastOption = string.Empty;
        _caseSensitive = _defaultCaseSens;
        _defaultOptionValue = _defaultDeaultOptionValue;
    }

    /// <summary>
    /// Calls the overloaded method, using the current <see cref="IsCaseSensitive"/> property value
    /// as a second argument.
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Passed by called overload when the input argument
    /// <paramref name="args"/> is null.</exception>
    /// <exception cref="ArgumentException"> Passed by called overload when any of strings in the input
    /// <paramref name="args"/> is null.</exception>
    /// 
    /// <param name="args">Parsed collection of command-line arguments. Must not equal to null.</param>
    public void ParseCommandLine(IEnumerable<string> args)
    {
        ParseCommandLine(args, IsCaseSensitive, DefaultOptionValue);
    }

    /// <summary>
    /// Call this member function to parse the command line and send the parameters, one at a time, 
    /// to the <see cref="ParseParam"/>.
    /// </summary>
    /// 
    /// <exception cref="ArgumentNullException"> Thrown when the input argument <paramref name="args"/>
    /// is null.</exception>
    /// <exception cref="ArgumentException"> Thrown when any of strings in the input collection 
    /// <paramref name="args"/> is null.</exception>
    /// 
    /// <remarks> This method is close analogy of MFC CWinApp::ParseCommandLine. </remarks>
    /// 
    /// <param name="args"> Parsed collection of command-line arguments. Must not equal to null. </param>
    /// <param name="caseSensitive"> Sets the <see cref="IsCaseSensitive"/> property which is used during
    ///  parsing. </param>
    /// <param name="defaultOptionValue"> The value assigned to <see cref="DefaultOptionValue"/>. 
    /// Can't be null. </param>
    public virtual void ParseCommandLine(IEnumerable<string> args, bool caseSensitive, string defaultOptionValue)
    {
        ArgumentNullException.ThrowIfNull(args);
        ArgumentNullException.ThrowIfNull(defaultOptionValue);

        if (args.Any(s => s is null))
            throw new ArgumentException("Input argument cannot equal to null");

        Clear();
        this._caseSensitive = caseSensitive;
        this._defaultOptionValue = defaultOptionValue;

        foreach (string s in args)
        {
            bool bFlag;
            string strArg = s;

            if (bFlag = !string.IsNullOrEmpty(strArg) && (strArg[0] == '-' || strArg[0] == '/'))
            {   // remove that flag prefix
                strArg = strArg[1..];
            }
            ParseParam(strArg, bFlag);
        }
    }

    /// <summary> Get the option and its value. </summary>
    /// <remarks>   One may use this method to figure-out a switch presence, too. </remarks>
    /// <param name="strOption">    The name of retrieved option. </param>
    /// <param name="strVal">   [out] The output value. </param>
    ///
    /// <returns>
    /// True on success ( if the option of given name <paramref name="strOption"/> is present), false otherwise.
    /// </returns>
    public bool GetOption(string strOption, out string strVal)
    {
        if (!IsCaseSensitive)
        {
            strOption = strOption.ToUpper(CultureInfo.InvariantCulture);
        }
        return DictArgs.TryGetValue(strOption, out strVal);
    }

    /// <summary>
    /// Get the boolean-like switch. Returns true if the switch has been present, false otherwise.
    /// </summary>
    /// <param name="strSwitch">The name of retrieved switch.</param>
    /// <returns>true if the switch has been present, false otherwise.</returns>
    public bool GetSwitch(string strSwitch)
    {
        return GetOption(strSwitch, out _);
    }
    #endregion // Public Methods

    #region Protected Methods

    /// <summary>
    /// The parser ParseCommandLine calls this function to parse/interpret individual parameters from
    /// the command line. You may overwrite it to modify the functionality.
    /// </summary>
    /// <remarks> This method is analogy of <see href="https://msdn.microsoft.com/en-us/library/zaydx040.aspx">
    /// CCommandLineInfo::ParseCommandLine</see> in MFC </remarks>
    ///  
    /// <param name="strParam"> the currently parsed parameter string value. </param>
    /// <param name="bFlag"> True if currently parsed <paramref name="strParam"/> is a flag,
    ///  i.e. an option (having value); false otherwise. </param>
    protected virtual void ParseParam(string strParam, bool bFlag)
    {
        if (!IsCaseSensitive)
        {
            strParam = strParam.ToUpper(CultureInfo.InvariantCulture);
        }
        if (bFlag)
        {
            // this is a "flag" (begins with / or -)
            DictArgs[strParam] = _defaultOptionValue;  // assign default option value ( "TRUE" )
            _LastOption = strParam;    // save option name in case other value specified
        }
        else
        {
            if (!string.IsNullOrEmpty(_LastOption))
            {
                // previous token was option: set its value instead of default that was set before
                DictArgs[_LastOption] = strParam;
                _LastOption = string.Empty; // clear
            }
            else
            { // we have switch; add to dictionary too, with fictive value null
                DictArgs[strParam] = _fictiveSwitchValue;
            }
        }
    }
    #endregion // Protected Methods
    #endregion // Methods

    #region IDeepCloneable<CommandLineInfoEx> Members
    #region IDeepCloneable Members

    object IDeepCloneable.DeepClone()
    {
        return (this as IDeepCloneable<CommandLineInfoEx>).DeepClone();
    }
    #endregion // IDeepCloneable Members

    /// <summary>
    /// Returns a deep clone of 'this' object.
    /// </summary>
    /// <returns>Resulting deep clone object.</returns>
    public CommandLineInfoEx DeepClone()
    {
        return new CommandLineInfoEx(this);
    }
    #endregion // IDeepCloneable<CommandLineInfoEx> Members
}

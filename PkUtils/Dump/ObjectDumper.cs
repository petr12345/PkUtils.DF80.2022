/***************************************************************************************************************
*
* FILE NAME:   .\Dump\ObjectDumper.cs
*
* AUTHOR:      Petr Kodet
*
* DESCRIPTION: Contains code of the class ObjectDumper
*
**************************************************************************************************************/

///////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// MSDN license agreement notice
// 
// This software is a Derivative Work based upon a MSDN 
// Implementation of ObjectDumper class
// http://msdn.microsoft.com/en-us/library/bb397968(VS.90).aspx
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Ignore Spelling: Utils
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PK.PkUtils.Extensions;

#pragma warning disable IDE0083 // Use pattern matching

namespace PK.PkUtils.Dump;

/// <summary>
/// The Object Dumper is a class that you can add to your project for instance to output the
/// results of LINQ queries for testing. For more info see
/// <see href="http://msdn.microsoft.com/en-us/library/bb397968(VS.90).aspx">
/// MSDN Object Dumper Sample</see> </summary>
///
/// <seealso href="http://msdn.microsoft.com/en-us/library/bb397968(VS.90).aspx">
/// Object Dumper Sample</seealso>
[CLSCompliant(false)]
public class ObjectDumper
{
    #region Fields
    /// <summary> The eventual output, initialized by constructor </summary>
    protected readonly TextWriter _writer;

    /// <summary> The current line position </summary>
    protected int _pos;

    /// <summary> The current level of recursion </summary>
    protected int _level;

    /// <summary> Maximal depth of dump recursion.  Initialized by constructor; in case of constructor without that
    /// argument will be used <see cref="DefaultMaxDepth "/> </summary>
    protected readonly int _maxDepth;

    /// <summary> Default maximal depth of recursion </summary>
    protected internal const int DefaultMaxDepth = int.MaxValue;

    private string _strLineSep = "\r\n";
    private string _strTabSep = _sTabString;

    private const string _sTabString = "\t";
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default argument-less constructor
    /// </summary>
    protected ObjectDumper()
      : this(null)
    {
    }

    /// <summary>
    /// Constructor accepting single argument TextWriter, which becomes an eventual target of the output text.
    /// </summary>
    /// <param name="writer"> A writer that can write a sequential series of characters. <br/>
    ///May equal to null, in that case no text is written anywhere.</param>
    protected ObjectDumper(TextWriter writer)
      : this(writer, DefaultMaxDepth)
    {
    }

    /// <summary>
    /// Constructor accepting the output TextWriter and the max. dump recursion depth
    /// </summary>
    /// <param name="writer"> A writer that can write a sequential series of characters. </param>
    /// <param name="maxDepth">Limit of dump recursion</param>
    protected ObjectDumper(TextWriter writer, int maxDepth)
    {
        this._writer = writer;
        this._maxDepth = maxDepth;
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Current line-separator string
    /// </summary>
    public string LineSeparator
    {
        get { return _strLineSep; }
        set { _strLineSep = value; }
    }

    /// <summary>
    /// Current tab-separator string
    /// </summary>
    public string TabSeparator
    {
        get { return _strTabSep; }
        set { _strTabSep = value; }
    }

    private bool IsLineSeparatorNewLine
    {
        get { return string.Equals(LineSeparator, "\r\n", StringComparison.OrdinalIgnoreCase); }
    }
    #endregion // Properties

    #region Public Methods

    /// <summary>
    /// Dump given object by temporary constructed ObjectDumper, using as TextWriter output the standard Console.Out,
    /// </summary>
    /// <param name="element">The object to be dumped. Can be null. </param>
    public static void Write(object element)
    {
        Write(element, 0);
    }

    /// <summary>
    /// Dump given object by temporary constructed ObjectDumper, using as TextWriter output the standard Console.Out,
    /// </summary>
    /// <param name="element">The object to be dumped. Can be null. </param>
    /// <param name="maxDepth">Max depth of dump recursion</param>
    public static void Write(object element, int maxDepth)
    {
        Write(element, maxDepth, Console.Out);
    }

    /// <summary>
    /// Dump given <paramref name="element"/> object by temporary constructed ObjectDumper, using given TextWriter
    /// </summary>
    /// <param name="element">The object to be dumped. Can be null. </param>
    /// <param name="maxDepth">Max depth of dump recursion</param>
    /// <param name="log"> A writer that can write a sequential series of characters. </param>
    public static void Write(object element, int maxDepth, TextWriter log)
    {
        ObjectDumper dumper = new(log, maxDepth);
        dumper.WriteObject(null, element);
    }

    /// <summary>
    /// Dump given <paramref name="element"/> object by temporary constructed ObjectDumper, resulting in a string output. <br/>
    /// The implementation uses temporary MemoryStream and a StreamWriter under that stream.
    /// </summary>
    /// <param name="element">The object to be dumped. Can be null. </param>
    /// <returns>Resulting textual information about the input <paramref name="element"/></returns>
    public static string Dump2Text(object element)
    {
        return Dump2Text(element, null);
    }

    /// <summary>
    /// Dump given <paramref name="element"/> object by temporary constructed ObjectDumper, resulting in a string output. <br/>
    /// The implementation uses temporary MemoryStream and a StreamWriter under that stream.
    /// </summary>
    /// <param name="element">The object to be dumped. Can be null. </param>
    /// <param name="lineSeparator">Line separator used on the output.</param>
    /// <returns>Resulting textual information about the input <paramref name="element"/></returns>
    public static string Dump2Text(object element, string lineSeparator)
    {
        return Dump2Text(element, lineSeparator, null, DefaultMaxDepth);
    }

    /// <summary>
    /// Dump given <paramref name="element"/> object by temporary constructed ObjectDumper, resulting in a string output. <br/>
    /// The implementation uses temporary MemoryStream and a StreamWriter under that stream.
    /// </summary>
    /// <param name="element">The object to be dumped. Can be null. </param>
    /// <param name="maxDepth">Max depth of dump recursion</param>
    /// <returns>Resulting textual information about the input <paramref name="element"/></returns>
    public static string Dump2Text(object element, int maxDepth)
    {
        return Dump2Text(element, null, null, maxDepth);
    }

    /// <summary>
    /// Dump given <paramref name="element"/> object by temporary constructed ObjectDumper, resulting in a string output. <br/>
    /// The implementation uses temporary MemoryStream and a StreamWriter under that stream.
    /// </summary>
    /// <param name="element">The object to be dumped. Can be null. </param>
    /// <param name="lineSeparator">Line separator used on the output.</param>
    /// <param name="tabSeparator">Tab separator used on the output.</param>
    /// <returns>Resulting textual information about the input <paramref name="element"/></returns>
    public static string Dump2Text(object element, string lineSeparator, string tabSeparator)
    {
        return Dump2Text(element, lineSeparator, tabSeparator, DefaultMaxDepth);
    }

    /// <summary>
    /// Dump given <paramref name="element"/> object by temporary constructed ObjectDumper, resulting in a string output. <br/>
    /// The implementation uses temporary MemoryStream and a StreamWriter under that stream.
    /// </summary>
    /// <param name="element">The object to be dumped. Can be null. </param>
    /// <param name="lineSeparator">Line separator used on the output.</param>
    /// <param name="tabSeparator">Tab separator used on the output.</param>
    /// <param name="maxDepth">Max depth of dump recursion</param>
    /// <returns>Resulting textual information about the input <paramref name="element"/></returns>
    public static string Dump2Text(object element, string lineSeparator, string tabSeparator, int maxDepth)
    {
        string strText = string.Empty;

        using (MemoryStream stream = new())
        {
            StreamWriter sw = new(stream);
            ObjectDumper dumper = new(sw, maxDepth);

            if (!string.IsNullOrEmpty(lineSeparator))
            {
                dumper.LineSeparator = lineSeparator;
            }
            if (!string.IsNullOrEmpty(tabSeparator))
            {
                dumper.TabSeparator = tabSeparator;
            }
            dumper.WriteObject(null, element);
            sw.Flush();
            stream.Position = 0;

            using StreamReader reader = new(stream);
            strText = reader.ReadToEnd();
        }

        return strText;
    }

    /// <summary> Dumps consequence of bytes into string containing their hexadecimal codes.</summary>
    /// <param name="data"> The data to dump. </param>
    /// <returns>   A resulting string, like "01 02 03 ". </returns>
    public static string DumpHex(IEnumerable<byte> data)
    {
        return (data is null) ? string.Empty : data.Join(" ", b => $"{b:X2}");
    }
    #endregion // Public Methods

    #region Private Methods

    private void Write(string s)
    {
        if (s != null)
        {
            _writer.Write(s);
            _pos += s.Length;
        }
    }

    private void WriteIndent()
    {
        for (int i = 0; i < _level; i++)
            _writer.Write("  ");
    }

    private void WriteLineSeparator()
    {
        if (IsLineSeparatorNewLine)
        {
            _writer.WriteLine();
            _pos = 0;
        }
        else
        {
            Write(LineSeparator);
        }
    }

    private void WriteTabSeparator()
    {
        if (TabSeparator == _sTabString)
        {
            Write("  ");
            while (_pos % 8 != 0)
                Write(" ");
        }
        else
        {
            Write(TabSeparator);
        }
    }

    private void WriteObject(string prefix, object element)
    {
        if (element == null || element is ValueType || element is string)
        {
            WriteIndent();
            Write(prefix);
            WriteValue(element);
            WriteLineSeparator();
        }
        else
        {
            if (element is IEnumerable enumerableElement)
            {
                foreach (object item in enumerableElement)
                {
                    if ((item is IEnumerable) && !(item is string))
                    {
                        WriteIndent();
                        Write(prefix);
                        Write("...");
                        WriteLineSeparator();
                        if (_level < _maxDepth)
                        {
                            _level++;
                            WriteObject(prefix, item);
                            _level--;
                        }
                    }
                    else
                    {
                        WriteObject(prefix, item);
                    }
                }
            }
            else
            {
                MemberInfo[] members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
                bool propWritten = false;

                WriteIndent();
                Write(prefix);
                foreach (MemberInfo m in members)
                {
                    FieldInfo f = m as FieldInfo;
                    PropertyInfo p = m as PropertyInfo;

                    if (f != null || p != null)
                    {
                        if (propWritten)
                        {
                            WriteTabSeparator();
                        }
                        else
                        {
                            propWritten = true;
                        }
                        Write(m.Name);
                        Write("=");
                        Type t = f != null ? f.FieldType : p.PropertyType;
                        if (t.IsValueType || t == typeof(string))
                        {
                            // PetrK 04/27/2012: Modified to avoid the exception TargetInvocationException 
                            // with InnerException of type InvalidOperationException,
                            // with text "Method may only be called on a Type for which Type.IsGenericParameter is true"
                            // original code commented-out...
                            /* WriteValue(f != null ? f.GetValue(element) : p.GetValue(element, null)); */
                            WriteValue(SafeGetValue(f, p, element));
                        }
                        else
                        {
                            if (typeof(IEnumerable).IsAssignableFrom(t))
                            {
                                Write("...");
                            }
                            else
                            {
                                Write("{ }");
                            }
                        }
                    }
                }

                // PetrK 09/12/2018: don't write separator after last item if the separator is not newline
                if (propWritten && IsLineSeparatorNewLine)
                {
                    WriteLineSeparator();
                }

                if (_level < _maxDepth)
                {
                    foreach (MemberInfo m in members)
                    {
                        FieldInfo f = m as FieldInfo;
                        PropertyInfo p = m as PropertyInfo;
                        if (f != null || p != null)
                        {
                            Type t = f != null ? f.FieldType : p.PropertyType;
                            if (!(t.IsValueType || t == typeof(string)))
                            {
                                object value = SafeGetValue(f, p, element);
                                if (value != null)
                                {
                                    _level++;
                                    WriteObject(m.Name + ": ", value);
                                    _level--;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void WriteValue(object o)
    {
        if (o == null)
        {
            Write("null");
        }
        else if (o is DateTime dt)
        {
            Write((dt).ToShortDateString());
        }
        else if (o is ValueType || o is string)
        {
            Write(o.ToString());
        }
        else if (o is IEnumerable)
        {
            Write("...");
        }
        else
        {
            Write("{ }");
        }
    }

    // PetrK 04/27/2012: Modified to avoid the exception TargetInvocationException 
    // with InnerException of type InvalidOperationException,
    // with text "Method may only be called on a Type for which Type.IsGenericParameter is true"
    //
    // The original code without the method call usually looked-like following...
    /* WriteValue(f != null ? f.GetValue(element) : p.GetValue(element, null)); */
    // 
    private static object SafeGetValue(FieldInfo f, PropertyInfo p, object element)
    {
        object result;

        if (f != null)
        {
            result = f.GetValue(element);
        }
        else
        {
            try
            {
                result = p.GetValue(element, null);
            }
            catch (TargetInvocationException)
            {
                result = "?";
            }
        }
        return result;
    }
    #endregion // Private Methods
}

#pragma warning restore IDE0083 // Use pattern matching
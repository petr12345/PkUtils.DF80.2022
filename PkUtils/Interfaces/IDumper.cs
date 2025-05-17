// Ignore Spelling: Utils
//
using System;
using PK.PkUtils.Dump;

namespace PK.PkUtils.Interfaces;


/// <summary>
/// Definition of object dumping functionality
/// </summary>
[CLSCompliant(true)]
public interface IDumper
{
    /// <summary>	Dumps the text to abstract output ( implementation-defined). </summary>
    /// <param name="text">	The added outgoing text. </param>
    /// <returns>	true if it succeeds, false if it fails. </returns>
    bool DumpText(string text);

    /// <summary>   Dumps a warning. </summary>
    /// <param name="text"> The added outgoing text. </param>
    /// <returns>   True if it succeeds, false if it fails. </returns>
    bool DumpWarning(string text);

    /// <summary>   Dumps an error. </summary>
    /// <param name="text"> The added outgoing text. </param>
    /// <returns>   True if it succeeds, false if it fails. </returns>
    bool DumpError(string text);

    /// <summary>	Cleans any previously dumped contents. </summary>
    /// <returns>	true if it succeeds, false if it fails. </returns>
    bool Reset();
}

/// <summary> IDumperEx is the base IDumper, with added functionality for managing a history limit. </summary>
[CLSCompliant(true)]
public interface IDumperEx : IDumper
{
    /// <summary>
    /// Returns true if this dumping target currently supports items history; false otherwise.
    /// </summary>
    bool SupportsHistory { get; }

    /// <summary>
    /// The current limit of items kept in history
    /// </summary>
    int HistoryLimit { get; }

    /// <summary>
    /// Sets the new value of limit of items kept in history.
    /// </summary>
    /// <param name="nNewLimit">A new value of <see cref="HistoryLimit"/> property.</param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    bool SetHistoryLimit(int nNewLimit);
}

/// <summary> Implements IDumper extensions. </summary>
public static class DumperExtensions
{
    /// <summary>   An IDumper extension method that dumps given text and newline. </summary>
    /// <param name="this"> The dumper to act on. Can't be null. </param>
    /// <param name="text"> The text to write. Can be null. </param>
    /// <returns>   True if it succeeds, false if it fails. </returns>
    public static bool DumpLine(this IDumper @this, string text)
    {
        ArgumentNullException.ThrowIfNull(@this);
        return @this.DumpText(text + Environment.NewLine);
    }

    /// <summary>   An IDumper extension method that dumps an object. </summary>
    /// <param name="this"> The dumper to act on. Can't be null. </param>
    /// <param name="obj"> The object to dump. Can be null. </param>
    /// <returns>   True if it succeeds, false if it fails. </returns>
    public static bool Dump(this IDumper @this, object obj)
    {
        ArgumentNullException.ThrowIfNull(@this);
        return @this.DumpText(ObjectDumper.Dump2Text(obj));
    }

}

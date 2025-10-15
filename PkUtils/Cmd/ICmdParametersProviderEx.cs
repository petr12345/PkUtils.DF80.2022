// Ignore Spelling: Utils
//
using System;
using System.Collections.Generic;

namespace PK.PkUtils.Cmd;

/// <summary> Interface for command-line parameters provider, extending <see cref="ICmdParametersProvider"/>. </summary>
public interface ICmdParametersProviderEx : ICmdParametersProvider
{
    /// <summary> Gets all keys representing options (pairs /name value or -name value). </summary>
    /// <remarks> Their order is not guaranteed to be the same as originally in the command line. </remarks>
    IEnumerable<string> Options { get; }

    /// <summary> Gets all keys representing switches ("values by just themselves"). </summary>
    /// <remarks> Their order is not guaranteed to be the same as originally in the command line. </remarks>
    IEnumerable<string> Switches { get; }

    /// <summary> Gets all keys representing both Options and switches, in the same order as they were on the command line. </summary>
    /// <remarks> Unlike with <see cref="Options"/> and <see cref="Switches"/>, the order of items here is preserved. </remarks>
    IReadOnlyList<string> OriginalArgumentsOrder { get; }

    /// <summary> Return the dictionary of pairs arg -> value. </summary>
    IReadOnlyDictionary<string, string> AllParameters { get; }

    /// <summary> Gets the used comparer of argument names ( keys in the dictionary <see cref="AllParameters "/>). </summary>
    StringComparer ArgumentNamesComparer { get; }
}
using System;

namespace PK.SubstEditLib.Subst;

/// <summary>
/// The interface mapping the individual field ID to displayed text
/// </summary>
[CLSCompliant(true)]
public interface ISubstitutionDescriptor<TFieldId>
{
    /// <summary>
    /// The identifier of the field, must uniquely determine the field
    /// </summary>
    TFieldId FieldId { get; }

    /// <summary>
    /// The drawn description text of the field, (before the field -> current value) substitution.
    /// Such text could look like {Month}, {DayOfWeek} etc.
    /// Any beginning/end brackets if needed must be included in this text too.
    /// </summary>
    string DisplayText { get; }
}

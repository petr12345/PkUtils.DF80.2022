using System;

namespace PK.SubstEditLib.Subst;

/// <summary>
/// One possible implementation of ISubstitutionDescriptor -
/// the class mapping the field ID to displayed text
/// </summary>
[CLSCompliant(true)]
public class SubstitutionDescriptor<TFieldId> : ISubstitutionDescriptor<TFieldId>
{
    #region Fields

    /// <summary>
    /// The field identifier.
    /// </summary>
    private readonly TFieldId _fieldId;

    /// <summary>
    /// The display text.
    /// </summary>
    private readonly string _displayText;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Initializes a new instance of the <see cref="SubstitutionDescriptor{TFieldId}"/> class.
    /// </summary>
    /// <param name="fieldId">The field identifier.</param>
    /// <param name="displayText">The display text.</param>
    public SubstitutionDescriptor(TFieldId fieldId, string displayText)
    {
        _fieldId = fieldId;
        _displayText = displayText;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubstitutionDescriptor{TFieldId}"/> class from another descriptor.
    /// </summary>
    /// <param name="rhs">The descriptor to copy from.</param>
    public SubstitutionDescriptor(ISubstitutionDescriptor<TFieldId> rhs)
    {
        ArgumentNullException.ThrowIfNull(rhs);
        _fieldId = rhs.FieldId;
        _displayText = rhs.DisplayText;
    }
    #endregion // Constructor(s)

    #region ISubstitutionDescriptor<TFieldId> Members

    /// <inheritdoc/>
    TFieldId ISubstitutionDescriptor<TFieldId>.FieldId
    {
        get { return _fieldId; }
    }

    /// <inheritdoc/>
    string ISubstitutionDescriptor<TFieldId>.DisplayText
    {
        get { return _displayText; }
    }
    #endregion // ISubstitutionDescriptor<TFieldId> Members
}
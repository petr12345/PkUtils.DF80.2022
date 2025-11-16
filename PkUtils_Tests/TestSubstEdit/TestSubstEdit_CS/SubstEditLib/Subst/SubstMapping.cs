using System;
using System.Collections.Generic;
using System.Linq;

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

/// <summary>
/// The interface representing complete "substitution map".
/// With given FieldId helps to find its substitution.
/// </summary>
[CLSCompliant(true)]
public interface ISubstitutionMap<TFieldId> : IEnumerable<ISubstitutionDescriptor<TFieldId>>
{
}

/// <summary>
/// The function converting the descriptor itself to the substitute text.
/// </summary>
/// <typeparam name="TFieldId"></typeparam>
/// <param name="descriptor"></param>
/// <returns></returns>
[CLSCompliant(true)]
public delegate string DescriptorToTextDelegate<TFieldId>(ISubstitutionDescriptor<TFieldId> descriptor);

/// <summary>
/// One possible implementation of ISubstitutionDescriptor -
/// the class mapping the field ID to displayed text
/// </summary>
[CLSCompliant(true)]
public class SubstitutionDescriptor<TFieldId> : ISubstitutionDescriptor<TFieldId>
{
    #region Fields
    private readonly TFieldId _fieldId;
    internal string _displayText;
    #endregion // Fields

    #region Constructor(s)

    public SubstitutionDescriptor(TFieldId fieldId, string displayText)
    {
        _fieldId = fieldId;
        _displayText = displayText;
    }

    public SubstitutionDescriptor(ISubstitutionDescriptor<TFieldId> rhs)
    {
        _fieldId = rhs.FieldId;
        _displayText = rhs.DisplayText;
    }
    #endregion // Constructor(s)

    #region ISubstitutionDescriptor<TFieldId> Members

    TFieldId ISubstitutionDescriptor<TFieldId>.FieldId
    {
        get { return _fieldId; }
    }

    string ISubstitutionDescriptor<TFieldId>.DisplayText
    {
        get { return _displayText; }
    }
    #endregion // ISubstitutionDescriptor<TFieldId> Members
};

[CLSCompliant(true)]
public class SubstitutionMapKeeper<TFieldId>
{
    private static readonly SubstitutionDescriptor<TFieldId>[] s_emptyMap =
        [new SubstitutionDescriptor<TFieldId>(default, null)];

    #region Fields

    // map of (field id) -> (field text)
    protected IEnumerable<ISubstitutionDescriptor<TFieldId>> _substitutionMap;
    #endregion // Fields

    #region Constructor(s)

    public SubstitutionMapKeeper()
      : this(s_emptyMap)
    {
    }

    public SubstitutionMapKeeper(IEnumerable<ISubstitutionDescriptor<TFieldId>> substitutionMap)
    {
        SetSubstitutionMap(substitutionMap);
    }

    public SubstitutionMapKeeper(SubstitutionMapKeeper<TFieldId> rhs)
    {
        SetSubstitutionMap(rhs.SubstitutionMap);
    }
    #endregion // Constructor(s)

    #region Properties

    public IEnumerable<ISubstitutionDescriptor<TFieldId>> SubstitutionMap
    {
        get { return _substitutionMap; }
    }
    #endregion // Properties

    #region Public Methods

    public void SetSubstitutionMap(IEnumerable<ISubstitutionDescriptor<TFieldId>> substitutionMap)
    {
        ArgumentNullException.ThrowIfNull(substitutionMap);
        _substitutionMap = substitutionMap;
    }

    /// <summary>
    /// In given sequence of descriptors finds the descriptor with matching id
    /// </summary>
    /// <param name="substitutionMap"></param>
    /// <param name="fieldId"></param>
    /// <returns></returns>
    public static ISubstitutionDescriptor<TFieldId> FindDescriptor(
        IEnumerable<ISubstitutionDescriptor<TFieldId>> substitutionMap,
        TFieldId fieldId)
    {
        ISubstitutionDescriptor<TFieldId> result = null;

        if (!EqualityComparer<TFieldId>.Default.Equals(fieldId, default) && (null != substitutionMap))
        {
            result = substitutionMap.FirstOrDefault(descr => EqualityComparer<TFieldId>.Default.Equals(descr.FieldId, fieldId));
        }
        return result;
    }

    /// <summary>
    /// Finds the descriptor with matching id in the current map
    /// </summary>
    /// <param name="fieldId"></param>
    /// <returns></returns>
    public ISubstitutionDescriptor<TFieldId> FindDescriptor(TFieldId fieldId)
    {
        return FindDescriptor(SubstitutionMap, fieldId);
    }
    #endregion // Public Methods
};

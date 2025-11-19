using System;
using System.Collections.Generic;
using System.Linq;

namespace PK.SubstEditLib.Subst;

/// <summary>
/// Maintains a map of field IDs to their substitution descriptors.
/// </summary>
/// <typeparam name="TFieldId">The type of the field identifier.</typeparam>
[CLSCompliant(true)]
public class SubstitutionMapping<TFieldId>
{
    /// <summary>
    /// An empty substitution map.
    /// </summary>
    private static readonly SubstitutionDescriptor<TFieldId>[] s_emptyMap =
        [new SubstitutionDescriptor<TFieldId>(default, null)];

    #region Fields

    /// <summary>
    /// The map of (field id) to (field text).
    /// </summary>
    protected IEnumerable<ISubstitutionDescriptor<TFieldId>> _substitutionMap;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Initializes a new instance of the <see cref="SubstitutionMapping{TFieldId}"/> class with an empty map.
    /// </summary>
    public SubstitutionMapping() : this(s_emptyMap)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubstitutionMapping{TFieldId}"/> class with the specified substitution map.
    /// </summary>
    /// <param name="substitutionMap">The substitution map.</param>
    public SubstitutionMapping(IEnumerable<ISubstitutionDescriptor<TFieldId>> substitutionMap)
    {
        SetSubstitutionMap(substitutionMap);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubstitutionMapping{TFieldId}"/> class by copying another instance.
    /// </summary>
    /// <param name="rhs">The instance to copy from.</param>
    public SubstitutionMapping(SubstitutionMapping<TFieldId> rhs)
    {
        ArgumentNullException.ThrowIfNull(rhs);
        SetSubstitutionMap(rhs.SubstitutionMap);
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Gets the current substitution map.
    /// </summary>
    public IEnumerable<ISubstitutionDescriptor<TFieldId>> SubstitutionMap
    {
        get { return _substitutionMap; }
    }
    #endregion // Properties

    #region Public Methods

    /// <summary>
    /// Sets the substitution map.
    /// </summary>
    /// <param name="substitutionMap">The substitution map to set.</param>
    public void SetSubstitutionMap(IEnumerable<ISubstitutionDescriptor<TFieldId>> substitutionMap)
    {
        ArgumentNullException.ThrowIfNull(substitutionMap);
        _substitutionMap = substitutionMap;
    }

    /// <summary>
    /// In given sequence of descriptors finds the descriptor with matching id.
    /// </summary>
    /// <param name="substitutionMap">The sequence of descriptors.</param>
    /// <param name="fieldId">The field identifier to search for.</param>
    /// <returns>The matching descriptor, or null if not found.</returns>
    public static ISubstitutionDescriptor<TFieldId> FindDescriptor(
        IEnumerable<ISubstitutionDescriptor<TFieldId>> substitutionMap,
        TFieldId fieldId)
    {
        ArgumentNullException.ThrowIfNull(substitutionMap);
        ISubstitutionDescriptor<TFieldId> result = null;

        if (!EqualityComparer<TFieldId>.Default.Equals(fieldId, default) && (null != substitutionMap))
        {
            result = substitutionMap.FirstOrDefault(descr => EqualityComparer<TFieldId>.Default.Equals(descr.FieldId, fieldId));
        }
        return result;
    }

    /// <summary>
    /// Finds the descriptor with matching id in the current map.
    /// </summary>
    /// <param name="fieldId">The field identifier to search for.</param>
    /// <returns>The matching descriptor, or null if not found.</returns>
    public ISubstitutionDescriptor<TFieldId> FindDescriptor(TFieldId fieldId)
    {
        return FindDescriptor(SubstitutionMap, fieldId);
    }
    #endregion // Public Methods
};

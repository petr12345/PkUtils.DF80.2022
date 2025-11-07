using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PK.SubstEditLib.Subst;


/// <summary>
/// The interface mapping the individual field ID to displayed text
/// </summary>
[CLSCompliant(true)]
public interface ISubstDescr<TFIELDID>
{
    /// <summary>
    /// The identifier of the field, must uniquely determine the field
    /// </summary>
    TFIELDID FieldId { get; }

    /// <summary>
    /// The drawn description text of the field, ( before the field -> current value ) substitution.
    /// Such text could look like {Month}, {DayOfWeek} etc. 
    /// Any beginning/end brackets if needed must be included in this text too.
    /// </summary>
    string DrawnText { get; }
}

/// <summary>
/// The interface representing complete "substitution map". 
/// With given FieldId helps to find its substitution.
/// </summary>
[CLSCompliant(true)]
public interface ISubstMap<TFIELDID> : IEnumerable<ISubstDescr<TFIELDID>>
{
}

/// <summary>
/// The function converting the filed itself ho the substitute text.
/// </summary>
/// <typeparam name="TFIELDID"></typeparam>
/// <param name="lpDescr"></param>
/// <returns></returns>
[CLSCompliant(true)]
public delegate string DescrToTextDelegate<TFIELDID>(ISubstDescr<TFIELDID> lpDescr);

/// <summary>
/// One possible implementation of ISubstDescr -
/// - the class mapping the field ID to displayed text
/// </summary>
[CLSCompliant(true)]
public class SubstDescr<TFIELDID> : ISubstDescr<TFIELDID>
{
    #region Fields
    private readonly TFIELDID _valId;
    internal string _strTxt;
    #endregion // Fields

    #region Cosntructor(s)

    public SubstDescr(TFIELDID valId, string strTxt)
    {
        _valId = valId;
        _strTxt = strTxt;
    }

    public SubstDescr(ISubstDescr<TFIELDID> rhs)
    {
        _valId = rhs.FieldId;
        _strTxt = rhs.DrawnText;
    }
    #endregion // Cosntructor(s)

    #region ISubstDescr<TFIELDID> Members

    TFIELDID ISubstDescr<TFIELDID>.FieldId
    {
        get { return _valId; }
    }

    string ISubstDescr<TFIELDID>.DrawnText
    {
        get { return _strTxt; }
    }
    #endregion // ISubstDescr<TFIELDID> Members
};

[CLSCompliant(true)]
public class SubstMapKeeper<TFIELDID>
{
    private static readonly SubstDescr<TFIELDID>[] stdEmptyMap = [new(default, null),];

    #region Fields
    // map of (field id) -> (field text)
    protected IEnumerable<ISubstDescr<TFIELDID>> _substMap;
    #endregion // Fields

    #region Constructor(s)

    public SubstMapKeeper()
      : this(stdEmptyMap)
    {
    }

    public SubstMapKeeper(IEnumerable<ISubstDescr<TFIELDID>> substMap)
    {
        AssignSubstMap(substMap);
    }

    public SubstMapKeeper(SubstMapKeeper<TFIELDID> rhs)
    {
        AssignSubstMap(rhs.GetSubstMap);
    }
    #endregion // Constructor(s)

    #region Properties

    public IEnumerable<ISubstDescr<TFIELDID>> GetSubstMap
    {
        get { return _substMap; }
    }
    #endregion // Properties

    #region Public Methods

    public void AssignSubstMap(IEnumerable<ISubstDescr<TFIELDID>> substMap)
    {
        Debug.Assert(substMap != null);
        _substMap = substMap;
    }

    /// <summary>
    /// In given sequence of ISubstDescr finds the ISubstDescr 
    /// with matching Type
    /// </summary>
    /// <param name="substMap"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static ISubstDescr<TFIELDID> FindMapItem(
        IEnumerable<ISubstDescr<TFIELDID>> substMap, TFIELDID item)
    {
        ISubstDescr<TFIELDID> result = null;

        if (!EqualityComparer<TFIELDID>.Default.Equals(default, item) && (null != substMap))
        {
            result = substMap.FirstOrDefault(descr => EqualityComparer<TFIELDID>.Default.Equals(descr.FieldId, item));
        }
        return result;
    }

    /// <summary>
    /// Finds the ISubstDescr with matching Type
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public ISubstDescr<TFIELDID> FindMapItem(TFIELDID item)
    {
        return FindMapItem(GetSubstMap, item);
    }
    #endregion // Public Methods
};

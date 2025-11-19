// Ignore Spelling: Substring
//
// Note: There is another way to "derive" type from integer - 
// - one could use structures with implicit casts;
// see more info on http://www.codeguru.com/forum/showthread.php?p=1817937

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using PK.PkUtils.Cloning.Binary;
using PK.PkUtils.Interfaces;
using PK.PkUtils.Utils;
using PK.PkUtils.XmlSerialization;
using tLogPos = System.Int32;

namespace PK.SubstEditLib.Subst;

#pragma warning disable IDE0079  // Remove unnecessary suppression
#pragma warning disable CA1859   // Change type of variable ...
#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
#pragma warning disable IDE0057  // Use range operator

/// <summary>
/// LogInfo is a logical coordinate of a field.
/// "Logical" coordinates do not include the interior length of fields.
/// </summary>
[Serializable]
[CLSCompliant(true)]
public class LogInfo<TFIELDID> :
    IXmlSerializable, IDeepCloneable<LogInfo<TFIELDID>>, IEquatable<LogInfo<TFIELDID>>
{
    #region Fields

    /// <summary>
    /// The field ID.
    /// </summary>
    protected TFIELDID _what;

    /// <summary>
    /// The logical position in the string; the index is considered without displayed body of fields.
    /// When computing logical position, an ordinary character has length 1,
    /// while the (complete) field has length 0.
    /// Hence, this logical position is actually a logical index of the character
    /// immediately AFTER the field.
    /// </summary>
    protected tLogPos _pos;

    // various attribute names
    private const string _strAttrPosition = "Position";
    private const string _strAttrFieldType = "FieldType";
    #endregion // Fields

    #region Constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public LogInfo() : this(default, 0)
    { }

    /// <summary>
    /// Constructor that sets the field id.
    /// </summary>
    /// <param name="what">Field identifier.</param>
    public LogInfo(TFIELDID what) : this(what, 0)
    { }

    /// <summary>
    /// Constructor that sets the field id and position.
    /// </summary>
    /// <param name="what">Field identifier.</param>
    /// <param name="pos">Logical position after the field.</param>
    public LogInfo(TFIELDID what, int pos)
    {
        SetWhat(what);
        SetPos(pos);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="rhs">Source instance to copy from.</param>
    public LogInfo(LogInfo<TFIELDID> rhs)
    {
        SetWhat(rhs.What);
        SetPos(rhs.Pos);
    }
    #endregion // Constructors

    #region Properties

    /// <summary>
    /// Human-readable representation of this instance for debugging.
    /// </summary>
    public virtual string Say
    {
        get
        {
            string strInfo = string.Format(
                CultureInfo.InvariantCulture,
                "LogInfo: (_what={0}, _pos={1})", _what, _pos);
            return strInfo;
        }
    }

    /// <summary>
    /// Gets or sets the field identifier.
    /// </summary>
    public TFIELDID What
    {
        get { return _what; }
        set { _what = value; }
    }

    /// <summary>
    /// Gets or sets the logical position (index immediately after the field).
    /// </summary>
    public tLogPos Pos
    {
        get { return _pos; }
        set { _pos = value; }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Sets the field identifier.
    /// </summary>
    /// <param name="id">Field identifier.</param>
    public void SetWhat(TFIELDID id)
    {
        _what = id;
    }

    /// <summary>
    /// Sets the logical position.
    /// </summary>
    /// <param name="pos">Logical position value.</param>
    public void SetPos(tLogPos pos)
    {
        _pos = pos;
    }

    /// <summary>
    /// Adds delta to the logical position.
    /// </summary>
    /// <param name="idelta">Delta to add to position (can be negative).</param>
    public void Add2Pos(tLogPos idelta)
    {
        _pos += idelta;
    }

    /// <summary>
    /// Assigns values from another LogInfo instance.
    /// </summary>
    /// <param name="rhs">Source instance.</param>
    public virtual void Assign(LogInfo<TFIELDID> rhs)
    {
        this._what = rhs.What;
        this._pos = rhs.Pos;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    public override int GetHashCode()
    {
        int hash;
        // must avoid calling GetHashCode if TFIELDID is a reference type and _what is null
        if (typeof(TFIELDID).IsClass && _what is null)
            hash = 0;
        else
            hash = _what.GetHashCode();

        hash ^= Pos.GetHashCode();
        return hash;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">Object to compare with.</param>
    public override bool Equals(object obj)
    {
        return (obj is LogInfo<TFIELDID> other) && Equals(other);
    }

    /// <summary>
    /// Asserts that the object is in a valid state (debug only).
    /// </summary>
    [Conditional("Debug")]
    public virtual void AssertValid()
    {
        Debug.Assert(0 <= _pos);
    }
    #endregion // Methods

    #region IXmlSerializable Members

    /// <summary>
    /// Not used. Returns null.
    /// </summary>
    public System.Xml.Schema.XmlSchema GetSchema()
    {
        return null;
    }

    /// <summary>
    /// Reads the LogInfo contents from its XML representation.
    /// </summary>
    /// <param name="reader">XML reader positioned at the start element.</param>
    /// <remarks>  
    /// ReadXml MUST read the wrapper element, including all of its contents.
    /// For more information, see: http://social.msdn.microsoft.com/Forums/en-US/xmlandnetfx/thread/27e77baa-67d0-4e15-a345-a6c314e924de
    /// </remarks>
    public void ReadXml(System.Xml.XmlReader reader)
    {
        if (reader.IsStartElement())
        {
            bool bSkip = true;

            // 1. read position
            reader.ReadStartElement();

            Pos = reader.ReadElementContentAsInt(_strAttrPosition, string.Empty);
            // 2. read TFIELDID
            if (reader.IsStartElement(_strAttrFieldType))
            {
                // if TFIELDID is a class, an instance must be created now
                if (typeof(TFIELDID).IsClass)
                {
                    // Note:
                    // If there is a 'new()' constraint applied on TFIELDID, one could create its instance by new.
                    // However, TFIELDID does not have to be a class
                    // (as I want to make it quite general, so I avoid applying the 'class' constraint),
                    // hence I do not want to apply the 'new()' constraint, neither.
                    // So, I have to go through a more elaborate process with reflection, 
                    // to find the constructor and invoke it.

                    /* _what = new TFIELDID(); */
                    ConstructorInfo constructorInfo = typeof(TFIELDID).GetConstructor(
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, [], null);
                    _what = (TFIELDID)constructorInfo.Invoke([]);
                }

                if (What is IXmlSerializable iSer)
                {
                    iSer.ReadXml(reader);
                }
                else
                {
                    NodeSerializer<TFIELDID> serz = new(typeof(TFIELDID).IsEnum);
                    serz.ReadXml(reader);
                    _what = (TFIELDID)serz;
                    bSkip = false;
                }
            }
            if (bSkip)
            {
                reader.Skip();
            }
            reader.ReadEndElement();
        }
    }

    /// <summary>
    /// Writes the LogInfo contents (elements and child elements) into XML.
    /// </summary>
    /// <param name="writer">XML writer positioned at the start of the object's element (wrapper element is not to be written here).</param>
    /// <remarks>
    /// WriteXml should NOT write the wrapper element.
    /// For more information, see:
    /// https://learn.microsoft.com/en-us/dotnet/api/system.xml.serialization.ixmlserializable.writexml#remarks
    /// </remarks>
    public void WriteXml(System.Xml.XmlWriter writer)
    {
        // 1. write position
        writer.WriteElementString(_strAttrPosition, Pos.ToString(CultureInfo.InvariantCulture));
        // 2. write TFIELDID
        if (What is not IXmlSerializable iSer)
        {
            iSer = new NodeSerializer<TFIELDID>(What, typeof(TFIELDID).IsEnum);
        }
        writer.WriteStartElement(_strAttrFieldType);
        iSer.WriteXml(writer);
        writer.WriteEndElement();

    }
    #endregion // IXmlSerializable Members

    #region IDeepCloneable<LogInfo> Members
    #region IDeepCloneable Members

    /// <summary>
    /// Non-generic deep clone implementation.
    /// </summary>
    object IDeepCloneable.DeepClone()
    {
        return (this as IDeepCloneable<LogInfo<TFIELDID>>).DeepClone();
    }
    #endregion // IDeepCloneable Members

    /// <summary>
    /// Creates a deep clone using copy constructor.
    /// </summary>
    public LogInfo<TFIELDID> DeepClone()
    {
        return new LogInfo<TFIELDID>(this);
        /* the other possibility
        return CloneHelper<LogInfo<TFIELDID>>.DeepClone(this);
         */
    }
    #endregion // IDeepCloneable<LogInfo> Members

    #region IEquatable<LogInfo<TFIELDID>> Members

    /// <summary>
    /// Determines equality with another LogInfo instance.
    /// </summary>
    /// <param name="other">Other instance to compare.</param>
    public bool Equals(LogInfo<TFIELDID> other)
    {
        bool result;

        if (other is null)
            result = false;
        else if (object.ReferenceEquals(this, other))
            result = true;
        else
        {
            byte[] arrThis = CloneHelperBinary.ToByteArray(this);
            byte[] arrThat = CloneHelperBinary.ToByteArray(other);
            result = MemUtils.memcmp(arrThis, arrThat);
        }

        return result;
    }
    #endregion // IEquatable<LogInfo<TFIELDID>> Members
}

/// <summary>
/// SubstLogData keeps "logical substitution data", i.e. logical data
/// needed for serialization and displaying of the contents of a control
/// such as <see cref="SubstEditTextBoxCtr{TFIELDID}"/>.
///
/// It stores a logical string (text without fields) and a list of logical
/// field positions, as well as a substitution map (not serialized) for
/// mapping field IDs to display text.
///
/// This class supports XML serialization and deep cloning, and provides
/// methods for converting between logical and physical representations of
/// the text, as well as for manipulating the logical field list and string.
/// </summary>
[Serializable]
[CLSCompliant(true)]
public class SubstLogData<TFIELDID> : IXmlSerializable, IEquatable<SubstLogData<TFIELDID>>
{
    #region Fields

    /// <summary>
    /// The field substitution map. Rather than deriving SubstLogData from SubstitutionMapping,
    /// it is stored as a member variable, since these data should not be serialized,
    /// while SubstLogData itself is serializable.
    /// </summary>
    [NonSerialized]
    private readonly SubstitutionMapping<TFIELDID> _map = new();

    /// <summary>
    /// The logical string (text without fields).
    /// </summary>
    protected string _logStr = string.Empty;

    /// <summary>
    /// List of logical positions.
    /// </summary>
    protected List<LogInfo<TFIELDID>> _logList = [];

    // various attribute names
    private const string _strLogicalText = "LogicalText";
    private const string _strFieldList = "FieldList";
    private const string _strElementLogInfo = "LogInfo";
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Default constructor.
    /// </summary>
    public SubstLogData() : this(null, string.Empty)
    { }

    /// <summary>
    /// Constructor that accepts a substitution map.
    /// </summary>
    /// <param name="substMap">Substitution map descriptors.</param>
    public SubstLogData(IEnumerable<ISubstitutionDescriptor<TFIELDID>> substMap)
      : this(substMap, string.Empty)
    { }

    /// <summary>
    /// Constructor that accepts a substitution map and initial logical string.
    /// </summary>
    /// <param name="substMap">Substitution map descriptors.</param>
    /// <param name="strLogStr">Initial logical string.</param>
    public SubstLogData(IEnumerable<ISubstitutionDescriptor<TFIELDID>> substMap, string strLogStr)
    {
        if (null != substMap)
        {
            _map.SetSubstitutionMap(substMap);
        }
        SetLogStr(strLogStr);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="rhs">Source instance to copy from.</param>
    public SubstLogData(SubstLogData<TFIELDID> rhs)
    {
        Assign(rhs);
    }
    #endregion // Constructor(s)

    #region Properties

    // apply the attribute if using simple xml serialization, without IXmlSerializable support
    /* [XmlElement("LogicalText")] */
    /// <summary>
    /// Gets or sets the logical text (text without fields).
    /// Setter is protected to control modifications via methods.
    /// </summary>
    public string GetLogStr
    {
        get { return _logStr; }
        protected set { SetLogStr(value); }
    }

    // apply the attribute if using simple xml serialization, without IXmlSerializable support
    /* [XmlArray("FieldList")] */
    /// <summary>
    /// Gets the list of LogInfo entries describing field positions.
    /// Setter is protected to allow controlled assignment.
    /// </summary>
    public List<LogInfo<TFIELDID>> GetLogList
    {
        get { return _logList; }
        protected set { AssignLogList(value); }
    }

    /// <summary>
    /// Human-readable representation of this instance for debugging.
    /// </summary>
    public virtual string Say
    {
        get
        {
            string strRes;
            StringBuilder sbInfo = new();

            foreach (LogInfo<TFIELDID> info in this.GetLogList)
            {
                sbInfo.Append(info.Say);
            }
            strRes = string.Format(
                CultureInfo.InvariantCulture,
                "SubstLogData: (_logStr={0}, _logList={1} )", _logStr, sbInfo);

            return strRes;
        }
    }

    /// <summary>
    /// Protected access to the substitution map keeper.
    /// </summary>
    protected SubstitutionMapping<TFIELDID> MapKeeper { get => _map; }

    /// <summary>
    /// Gets the substitution map descriptors (protected internal).
    /// </summary>
    protected internal IEnumerable<ISubstitutionDescriptor<TFIELDID>> GetSubstMap { get => MapKeeper.SubstitutionMap; }
    #endregion // Properties

    #region Methods
    #region Public Methods

    /// <summary>
    /// Sets the logical string.
    /// </summary>
    /// <param name="szLogStr">The logical string to set.</param>
    public void SetLogStr(string szLogStr)
    {
        _logStr = szLogStr;
    }

    /// <summary>
    /// Clears logical contents: logical string and log list.
    /// </summary>
    public void ClearContentsLogical()
    {
        _logStr = string.Empty;
        _logList.Clear();
    }

    /// <summary>
    /// Deletes all contents (alias for ClearContentsLogical).
    /// </summary>
    public virtual void DeleteContents()
    {
        ClearContentsLogical();
    }

    /// <summary>
    /// Sets the substitution map descriptors.
    /// </summary>
    /// <param name="substMap">Sequence of substitution descriptors.</param>
    public void SetSubstitutionMap(IEnumerable<ISubstitutionDescriptor<TFIELDID>> substMap)
    {
        _map.SetSubstitutionMap(substMap);
    }

    /// <summary>
    /// Assigns serializable data from another instance. Substitution map is not assigned here.
    /// </summary>
    /// <param name="rhs">Source instance.</param>
    public virtual void Assign(SubstLogData<TFIELDID> rhs)
    {
        // Do not assign subst map here, as it is not part of assignment or serialization
        /* SetSubstitutionMap(rhs.GetSubstMap); */
        AssignSerializableData(rhs);
    }

    /// <summary>
    /// Assigns the complete contents based on the given plain text input.
    /// Will parse the text to find all known field specifications and replace them with logical field entries.
    /// </summary>
    /// <param name="strText">Plain text containing possible field display texts.</param>
    public virtual void AssignPlainText(string strText)
    {
        IEnumerable<ISubstitutionDescriptor<TFIELDID>> substMap = MapKeeper.SubstitutionMap;

        ClearContentsLogical();
        // assign the input text as a logical text for now; following code will filter it
        SetLogStr(strText);

        // 1. parse the plain text and remove all known fields specification.
        // Note: Must do this BEFORE calling ReplaceLogXmlPartsBack,
        // since ReplaceLogXmlPartsBack will put back specific characters '<' '>',
        // that otherwise can mess up with fields beginnings
        for (tLogPos nDex = 0; ;)
        {
            string strLog = GetLogStr;
            LogInfo<TFIELDID> lpFound = null;

            if (nDex >= strLog.Length)
            {
                break;
            }
            // find corresponding field;
            // corresponding means that the text at logical position nDex matches
            foreach (ISubstitutionDescriptor<TFIELDID> descr in substMap)
            {
                string strMid;
                string strLocal = descr.DisplayText;
                int nLocalLength = strLocal.Length;

                if (nDex + nLocalLength <= strLog.Length)
                {
                    strMid = strLog.Substring(nDex, strLocal.Length);
                    if (strMid == strLocal)
                    {   // match found; create a new field replacing the text
                        ReplaceLogTextPart(nDex, nLocalLength, string.Empty);
                        lpFound = AppenNewLogInfo(descr.FieldId);
                        lpFound.SetPos(nDex);
                        break;
                    }
                }
            }
            if (null == lpFound)
            {
                nDex++;
            }
        }
        // 2. now perform the XML special characters substitution
        ReplaceLogXmlPartsBack();
    }

    /// <summary>
    /// Gets the "plain text" representation where fields are replaced by their display text.
    /// For illustration, this may look like "&lt;ProjectTitle&gt;".
    /// </summary>
    /// <returns>Plain text with fields replaced by their drawn text.</returns>
    public string GetPlainText()
    {
        // 1. Initialize temporary data
        SubstLogData<TFIELDID> tempData = new(this);
        tempData.SetSubstitutionMap(this.GetSubstMap);
        // 2. Substitute special XML characters in the logical string. 
        // See http://xml.silmaril.ie/authors/specials/
        // Note: this may modify the length of logical string in tempData, 
        // thus causing corresponding shifts of field positions.
        tempData.ReplaceLogXmlCharsThere();
        // 3. Convert current temp logical string to physical string by inserting fields
        return LogStr2PhysStr(tempData);
    }

    /// <summary>
    /// Gets the index of the specified LogInfo object in the internal list using reference equality.
    /// </summary>
    /// <param name="lpPos">LogInfo instance to search for.</param>
    /// <returns>Index of the instance or -1 if not found.</returns>
    public int GetLogInfoIndex(
        LogInfo<TFIELDID> lpPos)
    {
        // Must NOT use _logList.IndexOf(lpPos) !!
        // From MSDN:
        // The IndexOf method determines equality using the default equality comparer EqualityComparer<T>.Default for T, the type of values in the list.
        // The Default property checks whether type T implements the System.IEquatable<T> generic interface 
        // and if so returns an EqualityComparer<T> that uses that implementation. 
        // Otherwise it returns an EqualityComparer<T> that uses the overrides of Object.Equals and Object.GetHashCode provided by T.
        //
        // Since LogInfo implements (overridden) Equals, the returned index would be the first index where the Equals match.
        // However, _logList may contain several LogInfo instances whose fields are exactly the same.
        // Hence, what is needed here is object.ReferenceEquals
        //
        /* return _logList.IndexOf(lpPos); no!! */

        return _logList.FindIndex(item => object.ReferenceEquals(item, lpPos));
    }

    /// <summary>
    /// Appends a new LogInfo entry with the specified field id and returns it.
    /// Note: Method name retains original spelling.
    /// </summary>
    /// <param name="what">Field identifier.</param>
    /// <returns>Newly created LogInfo instance.</returns>
    public LogInfo<TFIELDID> AppenNewLogInfo(
        TFIELDID what)
    {
        LogInfo<TFIELDID> result;
        AppendLogInfo(result = new LogInfo<TFIELDID>(what));
        return result;
    }

    /// <summary>
    /// Adds an element to the internal log list.
    /// </summary>
    /// <param name="lpLogInfo">LogInfo instance to add.</param>
    public void AppendLogInfo(LogInfo<TFIELDID> lpLogInfo)
    {
        GetLogList.Add(lpLogInfo);
    }

    /// <summary>
    /// Inserts a LogInfo entry before a given index or appends if the index is negative.
    /// </summary>
    /// <param name="indexBefore">Index before which to insert; if negative, the item is appended.</param>
    /// <param name="lpLogInfo">LogInfo instance to insert.</param>
    public void InsertLogInfo(
        int indexBefore,
        LogInfo<TFIELDID> lpLogInfo)
    {
        if (0 <= indexBefore)
        {
            GetLogList.Insert(indexBefore, lpLogInfo);
        }
        else
        {
            GetLogList.Add(lpLogInfo);
        }
    }

    /// <summary>
    /// Inserts <paramref name="lpLogInfo"/> before <paramref name="lpLogInfoBefore"/>; appends if <paramref name="lpLogInfoBefore"/> is null.
    /// </summary>
    /// <param name="lpLogInfoBefore">Reference LogInfo before which the new item should be inserted.</param>
    /// <param name="lpLogInfo">LogInfo to insert.</param>
    public void InsertLogInfo(
        LogInfo<TFIELDID> lpLogInfoBefore,
        LogInfo<TFIELDID> lpLogInfo)
    {
        if (null != lpLogInfoBefore)
        {
            InsertLogInfo(GetLogInfoIndex(lpLogInfoBefore), lpLogInfo);
        }
        else
        {
            AppendLogInfo(lpLogInfo);
        }
    }

    /// <summary>
    /// Removes a LogInfo by index.
    /// </summary>
    /// <param name="nIndex">Index to remove.</param>
    public void RemoveLogInfo(int nIndex)
    {
        GetLogList.RemoveAt(nIndex);
    }

    /// <summary>
    /// Removes the specified LogInfo instance.
    /// </summary>
    /// <param name="lpLogInfo">Instance to remove.</param>
    /// <returns>True if removed; otherwise false.</returns>
    public bool RemoveLogInfo(LogInfo<TFIELDID> lpLogInfo)
    {
        return GetLogList.Remove(lpLogInfo);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">Object to compare with.</param>
    public override bool Equals(object obj)
    {
        return (obj is SubstLogData<TFIELDID> other) && Equals(other);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    public override int GetHashCode()
    {
        List<LogInfo<TFIELDID>> listThis = this.GetLogList;
        int nCount = listThis.Count;
        int hash = this.GetLogStr.GetHashCode();

        for (int ii = 0; ii < nCount; ii++)
        {
            hash ^= listThis[ii].GetHashCode();
        }
        return hash;
    }

    /// <summary>
    /// Conversion of logical string to complete physical representation using a mapping function.
    /// </summary>
    /// <param name="logicalData">Source logical data.</param>
    /// <param name="fnDescriptorToText">Delegate that converts a descriptor to display text.</param>
    /// <returns>Physical string with fields inserted.</returns>
    public static string LogStrToPhysStr(
        SubstLogData<TFIELDID> logicalData,
        Func<ISubstitutionDescriptor<TFIELDID>, string> fnDescriptorToText)
    {
        int itmplen;
        LogInfo<TFIELDID> lplogInf;
        ISubstitutionDescriptor<TFIELDID> lpDesc;
        tLogPos iLogCopied = 0;
        IList<LogInfo<TFIELDID>> logList = logicalData.GetLogList;
        string strTmp;
        string strLog = logicalData.GetLogStr;
        SubstitutionMapping<TFIELDID> mapKeeper = logicalData.MapKeeper;
        string strPhys = string.Empty;

        for (int ii = 0, nCount = logList.Count; ii < nCount; ii++)
        {
            tLogPos iLogPos;

            lplogInf = logList[ii];
            if (null != (lpDesc = mapKeeper.FindDescriptor(lplogInf.What)))
            {
                Debug.Assert(lplogInf.Pos <= strLog.Length);
                // add another piece of logical text
                strTmp = strLog.Substring(iLogCopied, (iLogPos = lplogInf.Pos) - iLogCopied);
                strPhys += strTmp;
                // add the text of field
                strPhys += fnDescriptorToText(lpDesc);
                // update the position in logical text
                iLogCopied = iLogPos;
            }
            else
            {
                Debug.Assert(false);
            }
        }
        // if there is remaining logical text not copied so far
        if ((itmplen = strLog.Length - iLogCopied) > 0)
        {
            strTmp = strLog.Substring(iLogCopied, itmplen);
            strPhys += strTmp;
        }

        return strPhys;
    }

    /// <summary>
    /// Conversion of logical string to complete physical representation using the display text from descriptors.
    /// </summary>
    /// <param name="logData">Source logical data.</param>
    /// <returns>Physical string with fields inserted.</returns>
    public static string LogStr2PhysStr(
        SubstLogData<TFIELDID> logData)
    {
        return LogStrToPhysStr(logData, lpDesc => (lpDesc != null) ? lpDesc.DisplayText : string.Empty);
    }

    /// <summary>
    /// Asserts that the object is in a valid state (debug only).
    /// </summary>
    [Conditional("Debug")]
    public virtual void AssertValid()
    {
        foreach (LogInfo<TFIELDID> info in this.GetLogList)
        {
            info.AssertValid();
        }
    }
    #endregion // Public Methods

    #region Protected Methods

    /// <summary>
    /// Assigns all serializable data from another instance.
    /// </summary>
    /// <param name="rhs">Source instance.</param>
    protected void AssignSerializableData(SubstLogData<TFIELDID> rhs)
    {
        _logStr = rhs.GetLogStr;  // assign _logStr
        AssignLogList(rhs.GetLogList);  // duplicate list
    }

    /// <summary>
    /// Assigns the contents of list by making a deep clone of it.
    /// </summary>
    /// <param name="list">List to clone.</param>
    protected void AssignLogList(IList<LogInfo<TFIELDID>> list)
    {
        _logList.Clear();
        foreach (LogInfo<TFIELDID> info in list)
        {
            this._logList.Add(info.DeepClone());
        }
    }

    /// <summary>
    /// Replaces the logical text part specified by startIndex and nReplacedLenght
    /// by a new string strNewText. The new string may be empty or even null.
    /// </summary>
    /// <param name="startIndex">Position of modification.</param>
    /// <param name="nReplacedLenght">The length of the part of original text to be replaced.</param>
    /// <param name="strNewText">Replacement text (may be null or empty).</param>
    /// <see cref="ReplaceLogTextAllThrough"/>
    protected virtual void ReplaceLogTextPart(
        tLogPos startIndex,
        int nReplacedLenght,
        string strNewText)
    {
        int nAddedLength;
        string strOldLog = GetLogStr;
        string strNewLog = strOldLog;

        if ((startIndex < 0) || (startIndex > strOldLog.Length))
        {
            throw new ArgumentException("Argument out of range", nameof(startIndex));
        }
        if ((nReplacedLenght < 0) || (nReplacedLenght > strOldLog.Length - startIndex))
        {
            throw new ArgumentException("Argument out of range", nameof(nReplacedLenght));
        }
        // find all affected fields (fields for which the deletion index in text precedes the field)
        List<LogInfo<TFIELDID>> listAffected = GetLogList.FindAll(info => (startIndex < info.Pos));

        if (nReplacedLenght > 0)
        {
            strNewLog = strNewLog.Remove(startIndex, nReplacedLenght);
            foreach (LogInfo<TFIELDID> info in listAffected)
            {
                info.Add2Pos(-nReplacedLenght);
            }
        }
        if (!string.IsNullOrEmpty(strNewText))
        {
            nAddedLength = strNewText.Length;
            strNewLog = strNewLog.Insert(startIndex, strNewText);
            foreach (LogInfo<TFIELDID> info in listAffected)
            {
                info.Add2Pos(nAddedLength);
            }
        }
        this.SetLogStr(strNewLog);
    }

    /// <summary>
    /// Goes through all current logical text, and replaces occurrences of 
    /// substring strOldPart to substring strNewPart.
    /// Positions of fields are updated appropriately.
    /// </summary>
    /// <param name="strOldPart">Substring to find.</param>
    /// <param name="strNewPart">Substring to replace with.</param>
    /// <see cref="ReplaceLogTextPart"/>
    protected void ReplaceLogTextAllThrough(string strOldPart, string strNewPart)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(strOldPart);

        int nDexFound;
        int nOldPart = strOldPart.Length;
        int nNewPart = string.IsNullOrEmpty(strNewPart) ? 0 : strNewPart.Length;

        for (int nSearPos = 0; ;)
        {
            string strOldLog = GetLogStr;

            if (0 <= (nDexFound = strOldLog.IndexOf(strOldPart, nSearPos, StringComparison.InvariantCulture)))
            {
                ReplaceLogTextPart(nDexFound, nOldPart, strNewPart);
                nSearPos += nNewPart;
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// Substitute special XML characters in the logical string
    /// for their entity representations like "&amp;" or "&lt;".
    /// See http://xml.silmaril.ie/authors/specials/
    /// </summary>
    /// <see cref="ReplaceLogXmlPartsBack"/>
    protected void ReplaceLogXmlCharsThere()
    {   // must replace ampersands first
        ReplaceLogTextAllThrough("&", "&amp;");   // 1.
        ReplaceLogTextAllThrough("<", "&lt;");    // 2.
        ReplaceLogTextAllThrough(">", "&gt;");    // 3.
        ReplaceLogTextAllThrough("\"", "&quot;"); // 4.
        ReplaceLogTextAllThrough("'", "&apos");   // 5.
    }

    /// <summary>
    /// Substitute special XML character entities in the logical string
    /// back to single characters.
    /// </summary>
    /// <see cref="ReplaceLogXmlCharsThere"/>
    protected void ReplaceLogXmlPartsBack()
    {
        // Must do replacements in reverse order compared to ReplaceLogXmlCharsThere();
        // hence must replace ampersands last.
        ReplaceLogTextAllThrough("&apos", "'");   // 5.
        ReplaceLogTextAllThrough("&quot;", "\""); // 4.
        ReplaceLogTextAllThrough("&gt;", ">");    // 3.
        ReplaceLogTextAllThrough("&lt;", "<");    // 2.
        ReplaceLogTextAllThrough("&amp;", "&");   // 1.
    }

    #endregion // Protected Methods
    #endregion // Methods

    #region IXmlSerializable Members

    /// <summary>
    /// Overwrites IXmlSerializable.GetSchema. 
    /// Should return an XmlSchema that describes the XML representation of the object 
    /// that is produced by the WriteXml method and consumed by the ReadXml method.
    /// </summary>
    /// <returns>Actually does not return any specific schema, always returns null.</returns>
    public System.Xml.Schema.XmlSchema GetSchema()
    {
        return null;
    }

    /// <summary>
    /// Reads the object contents from its XML representation using the specified <see cref="System.Xml.XmlReader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="System.Xml.XmlReader"/> from which the object contents will be read.</param>
    /// <remarks>  
    /// ReadXml MUST read the wrapper element, including all of its contents.
    /// For more information, see: http://social.msdn.microsoft.com/Forums/en-US/xmlandnetfx/thread/27e77baa-67d0-4e15-a345-a6c314e924de
    /// </remarks>
    public void ReadXml(System.Xml.XmlReader reader)
    {
        GetLogStr = reader.GetAttribute(_strLogicalText);
        reader.ReadStartElement();
        if (reader.IsStartElement(_strFieldList))
        {
            reader.ReadStartElement();
            while (reader.IsStartElement(_strElementLogInfo, string.Empty))
            {
                LogInfo<TFIELDID> info = new();
                info.ReadXml(reader);
                GetLogList.Add(info);
            }
        }
    }

    /// <summary>
    /// Writes the object contents (attributes, contents, child elements) into XML.
    /// </summary>
    /// <param name="writer">The <see cref="System.Xml.XmlWriter"/> to which the object contents will be written.
    /// This writer is positioned at the start of the element that represents the object, and should be used to write the object's contents
    /// (attributes, elements, etc.), but not the wrapper element itself.</param>
    /// <remarks>
    /// WriteXml should NOT write the wrapper element.
    /// For more information, see:
    /// https://learn.microsoft.com/en-us/dotnet/api/system.xml.serialization.ixmlserializable.writexml#remarks
    /// </remarks>
    public void WriteXml(System.Xml.XmlWriter writer)
    {
        writer.WriteAttributeString(_strLogicalText, GetLogStr);

        if (GetLogList.Count > 0)
        {
            writer.WriteStartElement(_strFieldList);
            foreach (LogInfo<TFIELDID> info in GetLogList)
            {
                writer.WriteStartElement(_strElementLogInfo);
                info.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
    #endregion // IXmlSerializable Members

    #region IEquatable<SubstLogData<TFIELDID>> Members

    /// <summary>
    /// Determines equality with another SubstLogData instance.
    /// </summary>
    /// <param name="other">Other instance to compare.</param>
    public bool Equals(SubstLogData<TFIELDID> other)
    {
        bool result = false;

        if (null != other)
        {
            if (object.ReferenceEquals(this, other))
            {
                result = true;
            }
            else if (result = (0 == string.Compare(this.GetLogStr, other.GetLogStr, StringComparison.Ordinal)))
            {
                List<LogInfo<TFIELDID>> listThis = this.GetLogList;
                List<LogInfo<TFIELDID>> listThat = other.GetLogList;

                result = (listThis.Count == listThat.Count) && listThis.SequenceEqual(listThat);
            }
        }
        return result;
    }
    #endregion // IEquatable<SubstLogData<TFIELDID>> Members
};

#pragma warning restore IDE0057   // Use range operator
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
#pragma warning restore CA1859    // Change type of variable ...
#pragma warning restore IDE0079  // Remove unnecessary suppression

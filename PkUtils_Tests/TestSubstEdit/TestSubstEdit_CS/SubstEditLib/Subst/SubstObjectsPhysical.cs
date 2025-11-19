using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PK.PkUtils.Interfaces;
using PK.PkUtils.UI.Utils;
using tLogPos = System.Int32;
using tPhysPos = System.Int32;

// Note: There is an other way to "derive" type from integer - 
// - one could use structures with implicit casts;
// see more info on http://www.codeguru.com/forum/showthread.php?p=1817937

#pragma warning disable IDE0057 // Use range operator

namespace PK.SubstEditLib.Subst;

/// <summary>
/// Represents a physical coordinate of a field, including the interior text of fields.
/// </summary>
[CLSCompliant(true)]
public class PhysInfo<TFIELDID> : IDeepCloneable<PhysInfo<TFIELDID>>
{
    #region Fields
    /// <summary>
    /// The identifier of the field.
    /// </summary>
    protected TFIELDID _what;

    /// <summary>
    /// The start position of the field in physical coordinates.
    /// </summary>
    protected tPhysPos _start;

    /// <summary>
    /// The end position of the field in physical coordinates.
    /// </summary>
    protected tPhysPos _end;
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Initializes a new instance of the <see cref="PhysInfo{TFIELDID}"/> class with default values.
    /// </summary>
    public PhysInfo() : this(default, 0, 0)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PhysInfo{TFIELDID}"/> class with the specified field identifier.
    /// </summary>
    /// <param name="what">The field identifier.</param>
    public PhysInfo(TFIELDID what) : this(what, 0, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PhysInfo{TFIELDID}"/> class with the specified field identifier, start, and end positions.
    /// </summary>
    /// <param name="what">The field identifier.</param>
    /// <param name="start">The start position in physical coordinates.</param>
    /// <param name="end">The end position in physical coordinates.</param>
    public PhysInfo(TFIELDID what, tPhysPos start, tPhysPos end)
    {
        SetWhat(what);
        SetStart(start);
        SetEnd(end);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PhysInfo{TFIELDID}"/> class by copying another instance.
    /// </summary>
    /// <param name="rhs">The instance to copy.</param>
    public PhysInfo(PhysInfo<TFIELDID> rhs)
      : this(rhs.What, rhs.GetStart, rhs.GetEnd)
    { }
    #endregion // Fields

    #region Properties

    /// <summary>
    /// Gets the field identifier.
    /// </summary>
    public TFIELDID What
    {
        get { return _what; }
    }

    /// <summary>
    /// Gets the start position in physical coordinates.
    /// </summary>
    public tPhysPos GetStart
    {
        get { return _start; }
    }

    /// <summary>
    /// Gets the end position in physical coordinates.
    /// </summary>
    public tPhysPos GetEnd
    {
        get { return _end; }
    }
    #endregion // Properties

    #region Methods

    /// <summary>
    /// Sets the field identifier.
    /// </summary>
    /// <param name="id">The field identifier.</param>
    public void SetWhat(TFIELDID id)
    {
        _what = id;
    }

    /// <summary>
    /// Sets the start position in physical coordinates.
    /// </summary>
    /// <param name="start">The start position.</param>
    public void SetStart(tPhysPos start)
    {
        _start = start;
    }

    /// <summary>
    /// Adds a delta to the start position.
    /// </summary>
    /// <param name="idelta">The delta to add.</param>
    public void Add2Start(int idelta)
    {
        _start += idelta;
    }

    /// <summary>
    /// Sets the end position in physical coordinates.
    /// </summary>
    /// <param name="end">The end position.</param>
    public void SetEnd(tPhysPos end)
    {
        _end = end;
    }

    /// <summary>
    /// Adds a delta to the end position.
    /// </summary>
    /// <param name="idelta">The delta to add.</param>
    public void Add2End(int idelta)
    {
        _end += idelta;
    }

    /// <summary>
    /// Gets the length of the field in physical coordinates.
    /// </summary>
    /// <returns>The length of the field.</returns>
    public int GetLength()
    {
        Debug.Assert(GetStart <= GetEnd);
        return (GetEnd - GetStart);
    }

    /// <summary>
    /// Assigns the values from another <see cref="PhysInfo{TFIELDID}"/> instance.
    /// </summary>
    /// <param name="rhs">The instance to copy from.</param>
    public virtual void Assign(PhysInfo<TFIELDID> rhs)
    {
        _what = rhs.What;
        _start = rhs.GetStart;
        _end = rhs.GetEnd;
    }

    /// <summary>
    /// Asserts the validity of the object in debug builds.
    /// </summary>
    [Conditional("Debug")]
    public virtual void AssertValid()
    {
        Debug.Assert(GetStart <= GetEnd);
    }
    #endregion // Methods

    #region IDeepCloneable<PhysInfo> Members
    #region IDeepCloneable Members

    /// <summary>
    /// Returns a deep clone of this object.
    /// </summary>
    /// <returns>A deep clone of this object.</returns>
    object IDeepCloneable.DeepClone()
    {
        return (this as IDeepCloneable<PhysInfo<TFIELDID>>).DeepClone();
    }
    #endregion // IDeepCloneable Members

    /// <summary>
    /// Returns a deep clone of this object.
    /// </summary>
    /// <returns>A deep clone of this object.</returns>
    public PhysInfo<TFIELDID> DeepClone()
    {
        return new PhysInfo<TFIELDID>(this);
    }
    #endregion // IDeepCloneable<PhysInfo> Members
}

/// <summary>
/// SubstPhysData keeps "substitution physical data", 
/// i.e. an internal data of <see cref="SubstEditTextBoxCtrl{TFIELDID}"/> control used during its editing.
/// Note: SubstPhysData do have to be serialized; 
/// they all are reconstructed from serialized SubstLogData.
/// </summary>
[CLSCompliant(true)]
public class SubstPhysData<TFIELDID> : SubstLogData<TFIELDID>
{
    #region Fields
    /// <summary>
    /// The physical string.
    /// </summary>
    protected string _physStr = string.Empty;
    /// <summary>
    /// List of physical positions.
    /// </summary>
    protected List<PhysInfo<TFIELDID>> _physlist = [];
    #endregion // Fields

    #region Constructor(s)

    /// <summary>
    /// Initializes a new instance of the <see cref="SubstPhysData{TFIELDID}"/> class.
    /// </summary>
    public SubstPhysData()
      : base()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubstPhysData{TFIELDID}"/> class with a substitution map.
    /// </summary>
    /// <param name="substMap">The substitution map.</param>
    public SubstPhysData(IEnumerable<ISubstitutionDescriptor<TFIELDID>> substMap)
      : base(substMap)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubstPhysData{TFIELDID}"/> class from log data.
    /// </summary>
    /// <param name="logData">The log data.</param>
    public SubstPhysData(SubstLogData<TFIELDID> logData)
      : base(logData)
    {    /* not needed - ctor of the base class calls overwriten Assign 
        AssignPhysFromLog(this);
        */
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubstPhysData{TFIELDID}"/> class by copying another instance.
    /// </summary>
    /// <param name="rhs">The instance to copy.</param>
    public SubstPhysData(SubstPhysData<TFIELDID> rhs)
    {
        this.Assign(rhs);
    }
    #endregion // Constructor(s)

    #region Properties

    /// <summary>
    /// Gets the physical string.
    /// </summary>
    public string GetPhysStr
    {
        get { return _physStr; }
    }

    /// <summary>
    /// Gets the list of physical positions.
    /// </summary>
    public List<PhysInfo<TFIELDID>> PhysList
    {
        get { return _physlist; }
    }
    #endregion // Properties

    #region Methods
    #region Public Methods

    #region Assignments & Cleanup
    /// <inheritdoc/>
    public override void Assign(SubstLogData<TFIELDID> rhs)
    {
        base.Assign(rhs);
        AssignPhysFromLog(this);
    }

    /// <inheritdoc/>
    public override void AssignPlainText(string strText)
    {
        base.AssignPlainText(strText);
        AssignPhysFromLog(this);
    }

    /// <summary>
    /// Assigns the contents of another <see cref="SubstPhysData{TFIELDID}"/> instance.
    /// </summary>
    /// <param name="rhs">The instance to copy from.</param>
    public virtual void Assign(SubstPhysData<TFIELDID> rhs)
    {
        base.Assign(rhs);
        AssignPhysStr(rhs);
        AssignPhysList(rhs.PhysList);
    }

    /// <summary>
    /// Clears the physical contents.
    /// </summary>
    public void ClearContentsPhys()
    {
        _physStr = string.Empty;
        PhysList.Clear();
    }

    /// <inheritdoc/>
    public override void DeleteContents()
    {
        base.DeleteContents();
        ClearContentsPhys();
    }
    #endregion // Assignments & Cleanup

    #region Searching

    /// <summary>
    /// For given LogInfo finds corresponding PhysInfo.
    /// </summary>
    /// <param name="logInf">The logical info.</param>
    /// <returns>The matching physical info, or null if not found.</returns>
    public PhysInfo<TFIELDID> FindMatch(LogInfo<TFIELDID> logInf)
    {
        int nDex;
        PhysInfo<TFIELDID> result = null;
        if (0 <= (nDex = GetLogInfoIndex(logInf)))
        {
            result = this.PhysList[nDex];
        }
        return result;
    }

    /// <summary>
    /// For given PhysInfo finds corresponding LogInfo.
    /// </summary>
    /// <param name="physInf">The physical info.</param>
    /// <returns>The matching logical info, or null if not found.</returns>
    public LogInfo<TFIELDID> FindMatch(PhysInfo<TFIELDID> physInf)
    {
        int nDex;
        LogInfo<TFIELDID> result = null;
        if (0 <= (nDex = PhysList.IndexOf(physInf)))
        {
            result = this.GetLogList[nDex];
        }
        return result;
    }

    /// <summary>
    /// Finds the last PhysInfo located before or on the given tPhysPos.
    /// </summary>
    /// <param name="phpos">The physical position.</param>
    /// <returns>The matching PhysInfo, or null if not found.</returns>
    public PhysInfo<TFIELDID> FindPhysInfoBefore(tPhysPos phpos)
    {
        return PhysList.LastOrDefault(phys => phys.GetEnd <= phpos);
    }

    /// <summary>
    /// Finds the first PhysInfo located after or on the given tPhysPos.
    /// </summary>
    /// <param name="phpos">The physical position.</param>
    /// <returns>The matching PhysInfo, or null if not found.</returns>
    public PhysInfo<TFIELDID> FindPhysInfoAfter(tPhysPos phpos)
    {
        return PhysList.FirstOrDefault(phys => phys.GetStart >= phpos);
    }

    /// <summary>
    /// Finds the first PhysInfo located between start and end.
    /// </summary>
    /// <param name="start">The start position.</param>
    /// <param name="end">The end position.</param>
    /// <returns>The matching PhysInfo, or null if not found.</returns>
    public PhysInfo<TFIELDID> FindPhysInfoBetween(tPhysPos start, tPhysPos end)
    {
        return PhysList.FirstOrDefault(phys => (phys.GetStart >= start) && (phys.GetEnd <= end));
    }

    /// <summary>
    /// Finds all PhysInfo located between start and end.
    /// </summary>
    /// <param name="start">The start position.</param>
    /// <param name="end">The end position.</param>
    /// <returns>List of matching PhysInfo.</returns>
    public List<PhysInfo<TFIELDID>> FindPhysInfoAllBetween(tPhysPos start, tPhysPos end)
    {
        return PhysList.FindAll(phys => (phys.GetStart >= start) && (phys.GetEnd <= end));
    }

    /// <summary>
    /// Finds the first PhysInfo containing the given tPhysPos.
    /// </summary>
    /// <param name="phpos">The physical position.</param>
    /// <returns>The matching PhysInfo, or null if not found.</returns>
    public PhysInfo<TFIELDID> FindPhysInfoPosIsIn(tPhysPos phpos)
    {
        return PhysList.FirstOrDefault(phys => (phys.GetStart < phpos) && (phpos < phys.GetEnd));
    }
    #endregion // Searching

    #region Operations on elements - these methods DO correct positions of other items

    /// <summary>
    /// Inserts a new PhysInfo at the specified position.
    /// </summary>
    /// <param name="phpos">The physical position.</param>
    /// <param name="what">The field identifier.</param>
    /// <returns>The inserted PhysInfo.</returns>
    public PhysInfo<TFIELDID> InsertNewInfo(tPhysPos phpos, TFIELDID what)
    {
        tLogPos logpos;

        if (null == MapKeeper.FindDescriptor(what))
        {
            Debug.Assert(false, "Cannot find specified field argument in the substitution map");
            return null;
        }

        logpos = PhysPos2LogPos(phpos);
        return InsertNewInfo(phpos, new LogInfo<TFIELDID>(what, logpos));
    }

    /// <summary>
    /// Inserts a new PhysInfo at the specified position using a LogInfo.
    /// </summary>
    /// <param name="phpos">The physical position.</param>
    /// <param name="logInfo">The logical info.</param>
    /// <returns>The inserted PhysInfo.</returns>
    public PhysInfo<TFIELDID> InsertNewInfo(tPhysPos phpos, LogInfo<TFIELDID> logInfo)
    {
        ArgumentNullException.ThrowIfNull(logInfo);

        string newphysStr;
        ISubstitutionDescriptor<TFIELDID> lpDesc;
        PhysInfo<TFIELDID> lpPhysBefore;
        LogInfo<TFIELDID> lpLogBefore;
        TFIELDID what;

        if (null == (lpDesc = MapKeeper.FindDescriptor(what = logInfo.What)))
        {
            Debug.Assert(false, "Cannot find specified field argument in the substitution map");
            return null;
        }
        int ilen = lpDesc.DisplayText.Length;
        PhysInfo<TFIELDID> physInfo = new(what, phpos, phpos + ilen);

        if (null != (lpPhysBefore = FindPhysInfoAfter(phpos)))
        {
            lpLogBefore = FindMatch(lpPhysBefore);
            MoveAllPhysInfoGreaterEq(phpos, ilen);
            InsertLogInfo(lpLogBefore, logInfo);
            InsertPhysInfo(lpPhysBefore, physInfo);
        }
        else
        {
            AppendLogInfo(logInfo);
            AppendPhysToList(physInfo);
        }

        newphysStr = GetPhysStr.Insert(phpos, lpDesc.DisplayText);
#if DEBUG
        string strTempNew = LogStr2PhysStr(this);
        Debug.Assert(newphysStr == strTempNew);
#endif
        SetPhysStr(newphysStr);

        return physInfo;
    }

    /// <summary>
    /// Removes a PhysInfo and its corresponding LogInfo.
    /// </summary>
    /// <param name="lpInf">The PhysInfo to remove.</param>
    /// <returns>True if removed, otherwise false.</returns>
    public bool DeleteOneInfo(PhysInfo<TFIELDID> lpInf)
    {
        ArgumentNullException.ThrowIfNull(lpInf);

        LogInfo<TFIELDID> lpLog;
        string strTmp;
        bool result = false;

        if (null == lpInf)
        {
            Debug.Assert(false);
        }
        else if (null == (lpLog = FindMatch(lpInf)))
        {
            Debug.Assert(false);
        }
        else
        {
            tPhysPos start = lpInf.GetStart;
            int ilen = lpInf.GetLength();

            RemovPhysInfo(lpInf);
            RemoveLogInfo(lpLog);
            if (ilen > 0)
            {
                MoveAllPhysInfoGreaterEq(start, -ilen);
                strTmp = GetPhysStr.Remove(start, ilen);
                SetPhysStr(strTmp);
            }
            result = true;
        }

        return result;
    }

    /// <summary>
    /// Removes all PhysInfo(s) between start and end, including their corresponding LogInfo(s).
    /// </summary>
    /// <param name="start">The start position.</param>
    /// <param name="end">The end position.</param>
    /// <returns>The number of physical positions deleted.</returns>
    public int DeleteAllBetween(tPhysPos start, tPhysPos end)
    {
        PhysInfo<TFIELDID> lpInf;
        string strLog, strPhys;
        tLogPos nStart;
        int ilen, log_dx;
        int phys_dx = end - start;
        tPhysPos tempEnd = end;

        while (null != (lpInf = FindPhysInfoBetween(start, tempEnd)))
        {
            ilen = lpInf.GetLength();
            DeleteOneInfo(lpInf);
            tempEnd -= ilen;
        }

        if ((log_dx = tempEnd - start) > 0)
        {
            strPhys = GetPhysStr.Remove(start, log_dx);
            SetPhysStr(strPhys);

            nStart = PhysPos2LogPos(start);
            strLog = GetLogStr.Remove(nStart, log_dx);
            SetLogStr(strLog);
            MoveAllInfoIfPhysGreaterEq(start, -log_dx);
#if DEBUG
            string strTmp = LogStr2PhysStr(this);
            Debug.Assert(strTmp == strPhys);
#endif
        }

        return phys_dx;
    }

    /// <summary>
    /// Inserts text at the given physical index.
    /// </summary>
    /// <param name="physIndex">The physical index.</param>
    /// <param name="sztext">The text to insert.</param>
    /// <returns>True if inserted, otherwise false.</returns>
    public bool InsertText(tPhysPos physIndex, string sztext)
    {
        bool res = false;

        if ((physIndex < 0) || (physIndex > GetPhysStr.Length))
        {   // invalid index - out of range
            Debug.Assert(false);
        }
        else if (null != FindPhysInfoPosIsIn(physIndex))
        {   // invalid index - request for insertion in middle of some field ?!
            Debug.Assert(false);
        }
        else
        {
            int ilen;
            tLogPos logIndex = PhysPos2LogPos(physIndex);
            string strText = sztext;

            if ((ilen = strText.Length) > 0)
            {
                string strPhysNew = GetPhysStr.Insert(physIndex, sztext);
                string strLogNew = GetLogStr.Insert(logIndex, sztext);

                SetPhysStr(strPhysNew);
                SetLogStr(strLogNew);
                MoveAllInfoIfPhysGreaterEq(physIndex, ilen);
                Debug.Assert(strLogNew == PhysStr2logStr(this, this.MapKeeper));
            }
            res = true;
        }

        return res;
    }

    /// <summary>
    /// Inserts data from logData at the given physical index.
    /// </summary>
    /// <param name="physIndex">The physical index.</param>
    /// <param name="logData">The log data to insert.</param>
    /// <returns>The number of characters inserted.</returns>
    public int InsertData(tPhysPos physIndex, SubstLogData<TFIELDID> logData)
    {
        ArgumentNullException.ThrowIfNull(logData);
        int nSuma = 0;

        if (this.InsertText(physIndex, logData.GetLogStr))
        {
            nSuma = logData.GetLogStr.Length;

            tLogPos beginLogPos = PhysPos2LogPos(physIndex);
            tPhysPos insertPhysIndex = physIndex;
            List<LogInfo<TFIELDID>> list = logData.GetLogList;

            for (tLogPos lasFieldLogPos = 0; list.Count > 0;)
            {
                int nLen;
                PhysInfo<TFIELDID> phInfo;
                LogInfo<TFIELDID> logInfo = list[0];
                int deltaLogPos = logInfo.Pos - lasFieldLogPos;

                insertPhysIndex += deltaLogPos;
                lasFieldLogPos = logInfo.Pos;
                logInfo.Add2Pos(beginLogPos);

                phInfo = InsertNewInfo(insertPhysIndex, logInfo);
                insertPhysIndex += (nLen = phInfo.GetLength());
                nSuma += nLen;

                list.RemoveAt(0);
            }
        }
        return nSuma;
    }
    #endregion // Operations on elements - these methods DO correct positions of other items

    #region conversions between log and phys positions

    /// <summary>
    /// Converts a physical position to a logical position.
    /// </summary>
    /// <param name="ph">The physical position.</param>
    /// <returns>The logical position.</returns>
    public tLogPos PhysPos2LogPos(tPhysPos ph)
    {
        int nDex, nSize;
        PhysInfo<TFIELDID> lpTmp;
        tLogPos result = ph;

        for (nDex = 0, nSize = PhysList.Count; nDex < nSize; nDex++)
        {
            if ((lpTmp = PhysList[nDex]).GetEnd <= ph)
            {
                result -= lpTmp.GetLength();
            }
        }
        return result;
    }

    /// <summary>
    /// "Exporting" the data in this.LogList to the output logData.LogList 
    /// The method does not modify "this" instance ( could be const ).
    /// </summary>
    /// <param name="logData">The log data to export to.</param>
    protected void ExportLogListAll(SubstLogData<TFIELDID> logData)
    {
        ArgumentNullException.ThrowIfNull(logData);
        ExportLogListSel(null, logData);
    }

    /// <summary>
    /// "Exporting" the data in this.LogList that are selected by input arg. selInf
    /// to the output logData.LogList. The beginnings of exported fields are adjusted 
    /// to match the selection begin.
    /// If the argument selInf is null, complete field list is exported
    /// ( selInf equal to null is interpreted as 'all selected').
    /// </summary> 
    /// <remarks>The method does not modify "this" instance ( could be const ).</remarks>
    /// <param name="selInf">The selection info.</param>
    /// <param name="logData">The log data to export to.</param>
    protected void ExportLogListSel(TextBoxSelInfo selInf, SubstLogData<TFIELDID> logData)
    {
        ArgumentNullException.ThrowIfNull(logData);

        LogInfo<TFIELDID> logInfOld, logInfNew;
        PhysInfo<TFIELDID> physInf;
        ISubstitutionDescriptor<TFIELDID> lpDesc;
        List<PhysInfo<TFIELDID>> listExported;
        tPhysPos suma;

        Debug.Assert(0 == logData.GetLogList.Count);

        if (null == selInf)
        {
            listExported = PhysList;
            suma = 0;
        }
        else
        {
            listExported = FindPhysInfoAllBetween(selInf.StartChar, selInf.EndChar);
            suma = selInf.StartChar;
        }

        /* no, subst. map is not assigned here, but the caller may do it
            logData.SetSubstitutionMap(this.GetSubstMap);
        */
        for (int nDex = 0, nCount = listExported.Count; nDex < nCount; nDex++)
        {
            physInf = listExported[nDex];
            logInfOld = FindMatch(physInf);
            if (null != (lpDesc = MapKeeper.FindDescriptor(logInfOld.What)))
            {
                if (null != (logInfNew = logData.AppenNewLogInfo(physInf.What)))
                {
                    logInfNew.SetPos(physInf.GetStart - suma);
                    suma += lpDesc.DisplayText.Length;
                }
            }
        }
    }

    /// <summary>
    /// "Exporting" all the SubstLogData to the output logData
    /// The method does not modify this object ( could be const )
    /// </summary>
    /// <param name="logData">The log data to export to.</param>
    public void ExportLogAll(SubstLogData<TFIELDID> logData)
    {
        ArgumentNullException.ThrowIfNull(logData);
        logData.ClearContentsLogical();
        ExportLogListAll(logData);
        logData.SetLogStr(PhysStr2logStr(this, this.MapKeeper));
        logData.SetSubstitutionMap(this.GetSubstMap);
    }

    /// <summary>
    /// Exporting the selection represented by input argument selInf to the output logData.
    /// If the argument selInf is null, complete field list is exported
    /// ( selInf equal to null is interpreted as 'all selected').
    /// </summary>
    /// <param name="selInf">The selection info.</param>
    /// <param name="logData">The log data to export to.</param>
    public void ExportLogSel(TextBoxSelInfo selInf, SubstLogData<TFIELDID> logData)
    {
        string strLog;
        int nLogSelBeg, nLogSelEnd;

        logData.ClearContentsLogical();
        if ((null == selInf) || selInf.IsSel)
        {
            ExportLogListSel(selInf, logData);
            strLog = PhysStr2logStr(this, this.MapKeeper);
            if (null != selInf)
            {
                nLogSelBeg = PhysPos2LogPos(selInf.StartChar);
                nLogSelEnd = PhysPos2LogPos(selInf.EndChar);
                strLog = strLog.Substring(nLogSelBeg, nLogSelEnd - nLogSelBeg);
            }
            logData.SetLogStr(strLog);
            logData.SetSubstitutionMap(this.GetSubstMap);
        }
    }
    #endregion // conversions between log and phys positions
    #endregion // Public Methods

    #region Protected Methods

    #region Operations on phys. elements only
    /// <summary>
    /// Moves all PhysInfo items whose start is greater than or equal to the specified value by the given offset.
    /// </summary>
    /// <param name="greaterOrEq">The threshold value.</param>
    /// <param name="nOffset">The offset to apply.</param>
    protected void MoveAllPhysInfoGreaterEq(tPhysPos greaterOrEq, int nOffset)
    {
        PhysList.ForEach(delegate (PhysInfo<TFIELDID> phinf)
        {
            if (phinf.GetStart >= greaterOrEq)
            {
                phinf.Add2Start(nOffset);
                phinf.Add2End(nOffset);
            }
        });
    }

    /// <summary>
    /// Moves all PhysInfo and LogInfo items whose start is greater than or equal to the specified value by the given offset.
    /// </summary>
    /// <param name="greaterOrEq">The threshold value.</param>
    /// <param name="nOffset">The offset to apply.</param>
    protected void MoveAllInfoIfPhysGreaterEq(tPhysPos greaterOrEq, int nOffset)
    {
        int nDex, nSize;
        LogInfo<TFIELDID> lpLogTmp;
        PhysInfo<TFIELDID> lpPhysTmp;

        for (nDex = 0, nSize = GetLogList.Count; nDex < nSize; nDex++)
        {
            lpLogTmp = GetLogList[nDex];
            lpPhysTmp = PhysList[nDex];
            Debug.Assert(EqualityComparer<TFIELDID>.Default.Equals(lpPhysTmp.What, lpLogTmp.What));
            if (lpPhysTmp.GetStart >= greaterOrEq)
            {
                lpPhysTmp.Add2Start(nOffset);
                lpPhysTmp.Add2End(nOffset);
                lpLogTmp.Add2Pos(nOffset);
            }
        }
    }
    #endregion // Operations on phys. elements only

    #region Operations on elements - these methods DO NOT correct positions of other items

    /// <summary>
    /// Appends a PhysInfo to the list.
    /// </summary>
    /// <param name="lpPhysInfo">The PhysInfo to append.</param>
    protected void AppendPhysToList(PhysInfo<TFIELDID> lpPhysInfo)
    {
        PhysList.Add(lpPhysInfo);
    }

    /// <summary>
    /// Adds a new PhysInfo with the specified field identifier.
    /// </summary>
    /// <param name="what">The field identifier.</param>
    /// <returns>The new PhysInfo.</returns>
    protected PhysInfo<TFIELDID> AddNewPhysInfo(TFIELDID what)
    {
        PhysInfo<TFIELDID> lpPhysInfo = new(what);
        AppendPhysToList(lpPhysInfo);
        return lpPhysInfo;
    }

    /// <summary>
    /// Inserts a PhysInfo before the specified index.
    /// </summary>
    /// <param name="indexBefore">The index before which to insert.</param>
    /// <param name="lpPhysInfo">The PhysInfo to insert.</param>
    protected void InsertPhysInfo(tPhysPos indexBefore, PhysInfo<TFIELDID> lpPhysInfo)
    {
        if (0 <= indexBefore)
        {
            PhysList.Insert(indexBefore, lpPhysInfo);
        }
        else
        {
            PhysList.Add(lpPhysInfo);
        }
    }

    /// <summary>
    /// Inserts a PhysInfo before another PhysInfo.
    /// </summary>
    /// <param name="lpPhysInfoBefore">The PhysInfo before which to insert.</param>
    /// <param name="lpPhysInfo">The PhysInfo to insert.</param>
    protected void InsertPhysInfo(PhysInfo<TFIELDID> lpPhysInfoBefore, PhysInfo<TFIELDID> lpPhysInfo)
    {
        if (null != lpPhysInfoBefore)
        {
            InsertPhysInfo(PhysList.IndexOf(lpPhysInfoBefore), lpPhysInfo);
        }
        else
        {
            AppendPhysToList(lpPhysInfo);
        }
    }

    /// <summary>
    /// Removes a PhysInfo at the specified index.
    /// </summary>
    /// <param name="nIndex">The index to remove.</param>
    protected void RemovPhysInfo(int nIndex)
    {
        PhysList.RemoveAt(nIndex);
    }

    /// <summary>
    /// Removes the specified PhysInfo.
    /// </summary>
    /// <param name="lpPhysInfo">The PhysInfo to remove.</param>
    /// <returns>True if removed, otherwise false.</returns>
    protected bool RemovPhysInfo(PhysInfo<TFIELDID> lpPhysInfo)
    {
        int nDex;
        bool result = false;

        if (0 <= (nDex = PhysList.IndexOf(lpPhysInfo)))
        {
            RemovPhysInfo(nDex);
            result = true;
        }
        return result;
    }
    #endregion // Operations on elements - these methods DO NOT correct positions of other items

    /// <summary>
    /// Assigns the physical string.
    /// </summary>
    /// <param name="str">The string to assign.</param>
    protected void SetPhysStr(string str)
    {
        this._physStr = str;
    }

    /// <summary>
    /// Assigns all this physical data (except the list PhysList) to the contents of the input argument rhs.
    /// </summary>
    /// <param name="rhs">The source SubstPhysData.</param>
    protected void AssignPhysStr(SubstPhysData<TFIELDID> rhs)
    {
        SetPhysStr(rhs.GetPhysStr);
    }

    /// <summary>
    /// Assigns the list this._physlist to the contents of the input argument list.
    /// </summary>
    /// <param name="list">The list to assign from.</param>
    protected void AssignPhysList(IList<PhysInfo<TFIELDID>> list)
    {
        this._physlist.Clear();
        foreach (PhysInfo<TFIELDID> info in list)
        {
            this._physlist.Add(info.DeepClone());
        }
    }

    /// <summary>
    /// Assigns its physical data from input argument SubstLogData logData.
    /// (Note: The input arg. does not change)
    /// </summary>
    /// <param name="logData">The log data to assign from.</param>
    protected void AssignPhysFromLog(SubstLogData<TFIELDID> logData)
    {
        // DON'T call DeleteContents or ClearContentsLogical - logical contents must be preserved
        ClearContentsPhys();
        AppendAsPhysInfo(logData);
        SetPhysStr(LogStr2PhysStr(logData));
    }

    /// <summary>
    /// Converts the constant input structure SubstLogData to physical data, 
    /// and appends to this object's physical data.
    /// It is assumed that current Phys.data are empty.
    /// </summary>
    /// <param name="logData">The log data to append as physical info.</param>
    protected void AppendAsPhysInfo(SubstLogData<TFIELDID> logData)
    {
        int ilen;
        int nDex, nCount;
        LogInfo<TFIELDID> lplogInf;
        PhysInfo<TFIELDID> physInf;
        ISubstitutionDescriptor<TFIELDID> lpDesc;
        tPhysPos suma;

        Debug.Assert(0 == PhysList.Count);

        for (suma = 0, nDex = 0, nCount = logData.GetLogList.Count; nDex < nCount; nDex++)
        {
            lplogInf = logData.GetLogList[nDex];
            if (null != (lpDesc = MapKeeper.FindDescriptor(lplogInf.What)))
            {
                ilen = lpDesc.DisplayText.Length;
                if (null != (physInf = AddNewPhysInfo(lplogInf.What)))
                {
                    tPhysPos start = suma + lplogInf.Pos;
                    tPhysPos end = start + ilen;
                    physInf.SetStart(start);
                    physInf.SetEnd(end);
                    suma += ilen;
                }
            }
        }
    }
    #endregion // Protected Methods

    #region Private Methods

    /// <summary>
    /// Converts the physical string to a logical string.
    /// </summary>
    /// <param name="physData">The physical data.</param>
    /// <param name="mapKeeper">The substitution map keeper.</param>
    /// <returns>The logical string.</returns>
    private static string PhysStr2logStr(
        SubstPhysData<TFIELDID> physData,
        SubstitutionMapping<TFIELDID> mapKeeper)
    {
        PhysInfo<TFIELDID> physInf;
        int itmplen, istart, idone;
        int nDex, nSize;
        string strTmp;
        string strPhys = physData.GetPhysStr;
        string strLog = string.Empty;

        Debug.Assert(physData.PhysList.Count == physData.GetLogList.Count);
        if (null == mapKeeper)
        {
            mapKeeper = physData.MapKeeper;
        }
        for (idone = 0, nDex = 0, nSize = physData.PhysList.Count; nDex < nSize; nDex++)
        {
            physInf = physData.PhysList[nDex];
            if (null != mapKeeper.FindDescriptor(physInf.What))
            {
                if (idone < (istart = physInf.GetStart))
                {
                    strTmp = strPhys.Substring(idone, istart - idone);
                    strLog += strTmp;
                }
                idone = physInf.GetEnd;
            }
            else
            {
                Debug.Assert(false);
            }
        }
        if ((itmplen = strPhys.Length - idone) > 0)
        {
            strTmp = strPhys.Substring(idone, itmplen);
            strLog += strTmp;
        }

        return strLog;
    }
    #endregion // Private Methods
    #endregion // Methods
}

#pragma warning restore IDE0057 // Use range operator
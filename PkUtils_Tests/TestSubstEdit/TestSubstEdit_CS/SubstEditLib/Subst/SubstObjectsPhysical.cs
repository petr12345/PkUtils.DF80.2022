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

namespace PK.SubstEditLib.Subst
{
    /// <summary>
    /// PhysInfo is a physical coordinate of field.
    /// Unlike logical coordinates, physical coordinates DO include ( count ) the interior 
    /// text of fields.
    /// </summary>
    [CLSCompliant(true)]
    public class PhysInfo<TFIELDID> : IDeepCloneable<PhysInfo<TFIELDID>>
    {
        #region Fields
        protected TFIELDID _what;
        protected tPhysPos _start;
        protected tPhysPos _end;
        #endregion // Fields

        #region Constructor(s)

        public PhysInfo()
          : this(default, 0, 0)
        {
        }

        public PhysInfo(TFIELDID what)
          : this(what, 0, 0)
        {
        }

        public PhysInfo(TFIELDID what, tPhysPos start, tPhysPos end)
        {
            SetWhat(what);
            SetStart(start);
            SetEnd(end);
        }

        public PhysInfo(PhysInfo<TFIELDID> rhs)
          : this(rhs.What, rhs.GetStart, rhs.GetEnd)
        {
        }
        #endregion // Fields

        #region Properties

        public TFIELDID What
        {
            get { return _what; }
        }

        public tPhysPos GetStart
        {
            get { return _start; }
        }

        public tPhysPos GetEnd
        {
            get { return _end; }
        }
        #endregion // Properties

        #region Methods

        public void SetWhat(TFIELDID id)
        {
            _what = id;
        }

        public void SetStart(tPhysPos start)
        {
            _start = start;
        }

        public void Add2Start(int idelta)
        {
            _start += idelta;
        }

        public void SetEnd(tPhysPos end)
        {
            _end = end;
        }

        public void Add2End(int idelta)
        {
            _end += idelta;
        }

        public int GetLength()
        {
            Debug.Assert(GetStart <= GetEnd);
            return (GetEnd - GetStart);
        }

        public virtual void Assign(PhysInfo<TFIELDID> rhs)
        {
            _what = rhs.What;
            _start = rhs.GetStart;
            _end = rhs.GetEnd;
        }

        [Conditional("Debug")]
        public virtual void AssertValid()
        {
            Debug.Assert(GetStart <= GetEnd);
        }
        #endregion // Methods

        #region IDeepCloneable<PhysInfo> Members
        #region IDeepCloneable Members

        object IDeepCloneable.DeepClone()
        {
            return (this as IDeepCloneable<PhysInfo<TFIELDID>>).DeepClone();
        }
        #endregion // IDeepCloneable Members

        public PhysInfo<TFIELDID> DeepClone()
        {
            return new PhysInfo<TFIELDID>(this);
        }
        #endregion // IDeepCloneable<PhysInfo> Members
    };

    /// <summary>
    /// SubstPhysData  keeps "substitution physical data", 
    /// i.e. an internal data of CSubstEdit control used during its editing.
    /// Note: SubstPhysData do have to be serialized; 
    /// they all are reconstructed from serialied SubstLogData.
    /// </summary>
    [CLSCompliant(true)]
    public class SubstPhysData<TFIELDID> : SubstLogData<TFIELDID>
    {
        #region Fields
        // the physical string
        protected string _physStr = string.Empty;
        // list of phys. positions
        protected List<PhysInfo<TFIELDID>> _physlist = [];
        #endregion // Fields

        #region Constructor(s)

        public SubstPhysData()
          : base()
        {
        }
        public SubstPhysData(IEnumerable<ISubstDescr<TFIELDID>> substMap)
          : base(substMap)
        {
        }

        public SubstPhysData(SubstLogData<TFIELDID> logData)
          : base(logData)
        {    /* not needed - ctor of the base class calls overwriten Assign 
            AssignPhysFromLog(this);
            */
        }

        public SubstPhysData(SubstPhysData<TFIELDID> rhs)
        {
            this.Assign(rhs);
        }
        #endregion // Constructor(s)

        #region Properties

        public string GetPhysStr
        {
            get { return _physStr; }
        }

        public List<PhysInfo<TFIELDID>> PhysList
        {
            get { return _physlist; }
        }
        #endregion // Properties

        #region Methods
        #region Public Methods

        #region Assignments & Cleanup
        public override void Assign(SubstLogData<TFIELDID> rhs)
        {
            base.Assign(rhs);
            AssignPhysFromLog(this);
        }

        public override void AssignPlainText(string strText)
        {
            base.AssignPlainText(strText);
            AssignPhysFromLog(this);
        }

        public virtual void Assign(SubstPhysData<TFIELDID> rhs)
        {
            base.Assign(rhs);
            AssignPhysStr(rhs);
            AssignPhysList(rhs.PhysList);
        }

        public void ClearContentsPhys()
        {
            _physStr = string.Empty;
            PhysList.Clear();
        }
        public override void DeleteContents()
        {
            base.DeleteContents();
            ClearContentsPhys();
        }
        #endregion // Assignments & Cleanup

        #region Searching

        /// <summary>
        /// For given LogInfo finds corresponding PhysInfo
        /// </summary>
        /// <param name="logInf"></param>
        /// <returns></returns>
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
        /// For given PhysInfo finds corresponding LogInfo
        /// </summary>
        /// <param name="physInf"></param>
        /// <returns></returns>
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
        /// Finding last PhysInfo located before or on given tPhysPos 
        /// </summary>
        /// <param name="phpos"></param>
        /// <returns></returns>
        public PhysInfo<TFIELDID> FindPhysInfoBefore(tPhysPos phpos)
        {
            /* usage of anonymous delegate, but the lambda expression is more efficient
            int nDex = this.PhysList.FindLastIndex(
                delegate(PhysInfo phys) { return phys.GetEnd <= phpos; });
            return (0 <= nDex) ? PhysList[nDex] : null;
             */
            return PhysList.LastOrDefault(phys => phys.GetEnd <= phpos);
        }

        /// <summary>
        /// Finding first PhysInfo located after or on given tPhysPos 
        /// </summary>
        /// <param name="phpos"></param>
        /// <returns></returns>
        public PhysInfo<TFIELDID> FindPhysInfoAfter(tPhysPos phpos)
        {
            /* usage of anonymous delegate, but the lambda expression is more efficient
            int nDex = this.PhysList.FindIndex(
                delegate(PhysInfo phys) { return phys.GetStart >= phpos; });
            return (0 <= nDex) ? PhysList[nDex] : null;
             */
            return PhysList.FirstOrDefault(phys => phys.GetStart >= phpos);
        }

        /// <summary>
        /// Finding first PhysInfo located between start and end
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public PhysInfo<TFIELDID> FindPhysInfoBetween(tPhysPos start, tPhysPos end)
        {
            /* usage of anonymous delegate, but the lambda expression is more efficient
            int nDex = this.PhysList.FindIndex(
                delegate(PhysInfo phys) { return (phys.GetStart >= start) && (phys.GetEnd <= end); });
            return (0 <= nDex) ? PhysList[nDex] : null;
             */
            return PhysList.FirstOrDefault(phys => (phys.GetStart >= start) && (phys.GetEnd <= end));
        }

        /// <summary>
        /// Finding all PhysInfo located between start and end
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<PhysInfo<TFIELDID>> FindPhysInfoAllBetween(tPhysPos start, tPhysPos end)
        {
            return PhysList.FindAll(phys => (phys.GetStart >= start) && (phys.GetEnd <= end));
        }

        /// <summary>
        /// Finding first PhysInfo located around (containing) given tPhysPos
        /// </summary>
        /// <param name="phpos"></param>
        /// <returns></returns>
        public PhysInfo<TFIELDID> FindPhysInfoPosIsIn(tPhysPos phpos)
        {
            /* usage of anonymous delegate, but the lambda expression is more efficient
            int nDex = this.PhysList.FindIndex(
                delegate(PhysInfo phys) { return (phys.GetStart < phpos) && (phpos < phys.GetEnd); });
            return (0 <= nDex) ? PhysList[nDex] : null;
             */
            return PhysList.FirstOrDefault(phys => (phys.GetStart < phpos) && (phpos < phys.GetEnd));
        }
        #endregion // Searching

        #region Operations on elements - these methods DO correct positions of other items

        public PhysInfo<TFIELDID> InsertNewInfo(tPhysPos phpos, TFIELDID what)
        {
            tLogPos logpos;

            if (null == MapKeeper.FindMapItem(what))
            {
                Debug.Assert(false, "Cannot find speficied field argument in the substitution map");
                return null;
            }

            logpos = PhysPos2LogPos(phpos);
            return InsertNewInfo(phpos, new LogInfo<TFIELDID>(what, logpos));
        }

        public PhysInfo<TFIELDID> InsertNewInfo(tPhysPos phpos, LogInfo<TFIELDID> logInfo)
        {
            string newphysStr;
            ISubstDescr<TFIELDID> lpDesc;
            PhysInfo<TFIELDID> lpPhysBefore;
            LogInfo<TFIELDID> lpLogBefore;
            TFIELDID what;

            if (null == (lpDesc = MapKeeper.FindMapItem(what = logInfo.What)))
            {
                Debug.Assert(false, "Cannot find speficied field argument in the substitution map");
                return null;
            }
            int ilen = lpDesc.DrawnText.Length;
            //tLogPos logpos = logInfo.Pos;
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

            newphysStr = GetPhysStr.Insert(phpos, lpDesc.DrawnText);
#if DEBUG
            string strTempNew = LogStr2PhysStr(this);
            Debug.Assert(newphysStr == strTempNew);
#endif
            SetPhysStr(newphysStr);

            return physInfo;
        }

        /// <summary>
        /// Removing PhysInfo and its corresponding LogInfo 
        /// </summary>
        /// <param name="lpInf"></param>
        /// <returns></returns>
        public bool DeleteOneInfo(PhysInfo<TFIELDID> lpInf)
        {
            LogInfo<TFIELDID> lpLog;
            string strTmp;
            bool bRes = false;

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
                bRes = true;
            }

            return bRes;
        }

        /// <summary>
        /// Removing all PhysInfo(s) between start and end,
        /// including its corresponding LogInfo(s) 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
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
        /// Inserting the text on given tPhysPos physIndex
        /// </summary>
        /// <param name="physIndex"></param>
        /// <param name="sztext"></param>
        /// <returns></returns>
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

        public int InsertData(tPhysPos physIndex, SubstLogData<TFIELDID> logData)
        {
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
        /// conversion of tPhysPos to tLogPos
        /// </summary>
        /// <param name="ph"></param>
        /// <returns></returns>
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
        /// <see cref="ExportLogAll"/>
        /// <param name="logData"></param>
        protected void ExportLogListAll(SubstLogData<TFIELDID> logData)
        {
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
        /// <see cref="ExportLogList"/>
        /// <param name="selInf"></param>
        /// <param name="logData"></param>
        protected void ExportLogListSel(TextBoxSelInfo selInf, SubstLogData<TFIELDID> logData)
        {
            LogInfo<TFIELDID> logInfOld, logInfNew;
            PhysInfo<TFIELDID> physInf;
            ISubstDescr<TFIELDID> lpDesc;
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
                logData.AssignSubstMap(this.GetSubstMap);
            */
            for (int nDex = 0, nCount = listExported.Count; nDex < nCount; nDex++)
            {
                physInf = listExported[nDex];
                logInfOld = FindMatch(physInf);
                if (null != (lpDesc = MapKeeper.FindMapItem(logInfOld.What)))
                {
                    if (null != (logInfNew = logData.AppenNewLogInfo(physInf.What)))
                    {
                        logInfNew.SetPos(physInf.GetStart - suma);
                        suma += lpDesc.DrawnText.Length;
                    }
                }
            }
        }

        /// <summary>
        /// "Exporting" all the SubstLogData to the output logData
        /// The method does not modify this object ( could be const )
        /// </summary>
        /// <see cref="ExportLogSel"/>
        /// <param name="logData"></param>
        public void ExportLogAll(SubstLogData<TFIELDID> logData)
        {
            logData.ClearContentsLogical();
            ExportLogListAll(logData);
            logData.SetLogStr(PhysStr2logStr(this, this.MapKeeper));
            logData.AssignSubstMap(this.GetSubstMap);
        }

        /// <summary>
        /// Exporting the selection represented by input argument selInf to the output logData.
        /// If the argument selInf is null, complete field list is exported
        /// ( selInf equal to null is interpreted as 'all selected').
        /// </summary>
        /// <see cref="ExportLogAll"/>
        /// <param name="selInf"></param>
        /// <param name="logData"></param>
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
                logData.AssignSubstMap(this.GetSubstMap);
            }
        }
        #endregion // conversions between log and phys positions
        #endregion // Public Methods

        #region Protected Methods

        #region Operations on phys. elements only
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

        protected void AppendPhysToList(PhysInfo<TFIELDID> lpPhysInfo)
        {
            PhysList.Add(lpPhysInfo);
        }

        protected PhysInfo<TFIELDID> AddNewPhysInfo(TFIELDID what)
        {
            PhysInfo<TFIELDID> lpPhysInfo = new(what);
            AppendPhysToList(lpPhysInfo);
            return lpPhysInfo;
        }

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

        protected void RemovPhysInfo(int nIndex)
        {
            PhysList.RemoveAt(nIndex);
        }

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
        /// Assign this._physStr to the input argument
        /// </summary>
        /// <param name="str"></param>
        protected void SetPhysStr(string str)
        {
            this._physStr = str;
        }

        /// <summary>
        /// Assign all this. physical data ( except the list PhysList ) to 
        /// the contents of the input argument rhs
        /// </summary>
        /// <param name="rhs"></param>
        protected void AssignPhysStr(SubstPhysData<TFIELDID> rhs)
        {
            SetPhysStr(rhs.GetPhysStr);
        }

        /// <summary>
        /// Assign the list this._physlist to 
        /// the contents of the input argument rhs
        /// </summary>
        /// <param name="list"></param>
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
        /// <param name="logData"></param>
        protected void AssignPhysFromLog(SubstLogData<TFIELDID> logData)
        {
            // DON'T call DeleteContents or ClearContentsLogical - logical contents must be preserved
            ClearContentsPhys();
            AppendAsPhysInfo(logData);
            SetPhysStr(LogStr2PhysStr(logData));
        }

        /// <summary>
        /// Converting the constant input structure SubstLogData to physical data, 
        /// and appending to this objects physical data.
        /// It is assumed that current Phys.data are empty
        /// </summary>
        /// <param name="logData"></param>
        protected void AppendAsPhysInfo(SubstLogData<TFIELDID> logData)
        {
            int ilen;
            int nDex, nCount;
            LogInfo<TFIELDID> lplogInf;
            PhysInfo<TFIELDID> physInf;
            ISubstDescr<TFIELDID> lpDesc;
            tPhysPos suma;

            Debug.Assert(0 == PhysList.Count);

            for (suma = 0, nDex = 0, nCount = logData.GetLogList.Count; nDex < nCount; nDex++)
            {
                lplogInf = logData.GetLogList[nDex];
                if (null != (lpDesc = MapKeeper.FindMapItem(lplogInf.What)))
                {
                    ilen = lpDesc.DrawnText.Length;
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

        private static string PhysStr2logStr(
            SubstPhysData<TFIELDID> physData,
            SubstMapKeeper<TFIELDID> mapKeeper)
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
                if (null != mapKeeper.FindMapItem(physInf.What))
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
    };
}

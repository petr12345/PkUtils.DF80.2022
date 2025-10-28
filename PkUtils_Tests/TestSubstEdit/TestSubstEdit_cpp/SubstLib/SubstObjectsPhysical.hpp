// SubstObjectsPhysical.hpp : 
// templates CPhysInfo<TFIELDID> and CSubstPhysData<TFIELDID> implementation file
//

/////////////////////////////////////////////////////////////////////////////
// INCLUDE FILES
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
// MANIFESTED CONSTANTS & MACROS
/////////////////////////////////////////////////////////////////////////////

// disabling inappropriate warning C4146: unary minus operator applied to unsigned type, result still unsigned
#pragma warning(disable:4146)

/////////////////////////////////////////////////////////////////////////////
// PRIVATE SYMBOLS
/////////////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////////////
// CLASS DEFINITIONS
/////////////////////////////////////////////////////////////////////////////

IMPLEMENT_DYNCREATE_T(CPhysInfo, TFIELDID, tPhysInfoPredecessor)

template<class TFIELDID> 
CPhysInfo<TFIELDID>::CPhysInfo() : tPhysInfoPredecessor()
{
    SetWhat((TFIELDID)kInvalidSubstElemId);
    SetStart(0);
    SetEnd(0);
}

template<class TFIELDID> 
CPhysInfo<TFIELDID>::CPhysInfo(TFIELDID what) 
    : tPhysInfoPredecessor()
{
    SetWhat(what);
    SetStart(0);
    SetEnd(0);
}

template<class TFIELDID> 
CPhysInfo<TFIELDID>::CPhysInfo(
    TFIELDID  what,
    tPhysPos  start,
    tPhysPos  end) 
    : tPhysInfoPredecessor()
{
    SetWhat(what);
    SetStart(start);
    SetEnd(end);
}

template<class TFIELDID> 
CPhysInfo<TFIELDID>::~CPhysInfo()
{
}

template<class TFIELDID> 
BOOL CPhysInfo<TFIELDID>::Assign(CPhysInfo<TFIELDID>const* lprhs)
{
    if (lprhs->IsKindOf(RUNTIME_CLASS(CPhysInfo)))
    {
        m_what  = lprhs->What();
        m_start = lprhs->GetStart();
        m_end   = lprhs->GetEnd();
        return TRUE;
    }
    else
    {
        ASSERT(FALSE);
        return FALSE;
    }
}

template<class TFIELDID> 
CPhysInfo<TFIELDID>& CPhysInfo<TFIELDID>::operator = (CPhysInfo<TFIELDID> const& rhs)
{
    Assign(&rhs);
    return *this;
}

template<class TFIELDID> 
void CPhysInfo<TFIELDID>::Serialize(CArchive& ar)
{
    tPhysInfoPredecessor::Serialize(ar);

    if (ar.IsLoading())
    {
        // In case the line below does not compile for the particular TFIELDID type,
        // you have to supply for that type an operator
        // CArchive& AFXAPI operator>>(CArchive& ar, TFIELDID &val)
        ar >> m_what;
        ar >> m_start;
        ar >> m_end;
    }
    else
    {
        ar << m_what;
        ar << m_start;
        ar << m_end;
    }
}

#ifdef _DEBUG
template<class TFIELDID> 
void CPhysInfo<TFIELDID>::AssertValid() const
{
    tPhysInfoPredecessor::AssertValid();
}

template<class TFIELDID> 
void CPhysInfo<TFIELDID>::Dump(CDumpContext& dc) const
{
    tPhysInfoPredecessor::Dump(dc);
}
#endif // _DEBUG

////////////////////////////////////////////

IMPLEMENT_DYNCREATE_T(CSubstPhysData, TFIELDID, CSubstLogData<TFIELDID>)

template<class TFIELDID> 
CSubstPhysData<TFIELDID>::CSubstPhysData() : CSubstLogData<TFIELDID>()
{
}

template<class TFIELDID> 
CSubstPhysData<TFIELDID>::CSubstPhysData(SubstDescr<TFIELDID> const* lpMap) 
    : CSubstLogData<TFIELDID>(lpMap)
{
}

template<class TFIELDID> 
CSubstPhysData<TFIELDID>::CSubstPhysData(CSubstLogData<TFIELDID> const & logData)
{
    *this = logData;
}

template<class TFIELDID> 
CSubstPhysData<TFIELDID>::CSubstPhysData(
    CSubstPhysData<TFIELDID> const &pattern) : CSubstLogData()
{
    *this = pattern;
}

template<class TFIELDID> 
CSubstPhysData<TFIELDID>::~CSubstPhysData()
{
    DeleteContents();
}

template<class TFIELDID> 
void CSubstPhysData<TFIELDID>::ClearContentsPhys(void)
{
    PhysList().DeleteAndRemoveAll();
    m_physStr.Empty();
}

template<class TFIELDID> 
void CSubstPhysData<TFIELDID>::DeleteContents(void)
{
    CSubstLogData<TFIELDID>::DeleteContents();
    ClearContentsPhys();
}

template<class TFIELDID> 
CPhysInfo<TFIELDID>* CSubstPhysData<TFIELDID>::AddNewPhysInfo(TFIELDID  what)
{
    CPhysInfo<TFIELDID>* lpPhysInfo = NULL;

    try
    {
        if (lpPhysInfo = new CPhysInfo<TFIELDID>(what))
        {
            AppendPhysToList(lpPhysInfo);
        }
    }
    catch(CException *e)
    {
        e->Delete();
        lpPhysInfo = NULL;
    }

    return lpPhysInfo;
}

template<class TFIELDID> 
void CSubstPhysData<TFIELDID>::MoveAllPhysInfoGreaterEq(tPhysPos greaterOrEq, size_t by)
{
    PhysList().ForEach(FnMoveAllPhysInfoGreaterEq, (WPARAM)greaterOrEq, (LPARAM)by);
}

template<class TFIELDID> 
void CSubstPhysData<TFIELDID>::MoveAllInfoIfPhysGreaterEq(tPhysPos greaterOrEq, size_t by)
{
    INT_PTR nDex, nSize;
    CLogInfo<TFIELDID>*  lpLogTmp;
    CPhysInfo<TFIELDID>* lpPhysTmp;

    for(nDex = 0, nSize = this->LogListC().GetSize(); nDex < nSize; nDex++)
    {
        VERIFY(lpLogTmp = this->LogListC().GetAt(nDex));
        VERIFY(lpPhysTmp = PhysListC().GetAt(nDex));
        ASSERT(lpPhysTmp->What() == lpLogTmp->What());
        if (lpPhysTmp->GetStart() >= greaterOrEq)
        {
            lpPhysTmp->Add2Start(by);
            lpPhysTmp->Add2End(by);
            lpLogTmp->Add2Pos(by);
        }
    }
}

template<class TFIELDID> 
CPhysInfo<TFIELDID>* CSubstPhysData<TFIELDID>::FindMatch(CLogInfo<TFIELDID>* lplogInf) const
{
    INT_PTR nDex;
    CPhysInfo<TFIELDID>* lpPhys = NULL;

    if (0 <= (nDex = GetLogInfoIndex(lplogInf)))
    {
        lpPhys = PhysListC().GetAt(nDex);
    }
    return lpPhys;
}

template<class TFIELDID> 
CLogInfo<TFIELDID>* CSubstPhysData<TFIELDID>::FindMatch(CPhysInfo<TFIELDID>* physInf) const
{
    INT_PTR nDex;
    CLogInfo<TFIELDID>*  lpLog = NULL;

    if (0 <= (nDex = PhysListC().Find(physInf)))
    {
        lpLog = this->LogListC().GetAt(nDex);
    }
    return lpLog;
}

// finding last CPhysInfo<TFIELDID>* located before or on given tPhysPos 
template<class TFIELDID> 
CPhysInfo<TFIELDID>* CSubstPhysData<TFIELDID>::FindPhysInfoBefore(tPhysPos phpos) const
{
    INT_PTR nDex = PhysListC().LastThat(FnPhysInfoPosLowerEq, (WPARAM)phpos);
    return (nDex >= 0) ? PhysListC().GetAt(nDex) : NULL;
}

// finding first CPhysInfo<TFIELDID>* located after or on given tPhysPos 
template<class TFIELDID> 
CPhysInfo<TFIELDID>* CSubstPhysData<TFIELDID>::FindPhysInfoAfter(tPhysPos phpos) const
{
    INT_PTR nDex = PhysListC().FirstThat(FnPhysInfoPosGreaterEq, (WPARAM)phpos);
    return (nDex >= 0) ? PhysListC().GetAt(nDex) : NULL;
}

// finding first CPhysInfo<TFIELDID>* located between start and end
template<class TFIELDID> 
CPhysInfo<TFIELDID>* CSubstPhysData<TFIELDID>::FindPhysInfoBetween(tPhysPos start, tPhysPos end) const
{
    INT_PTR nDex = PhysListC().FirstThat(FnPhysInfoBetween, (WPARAM)start, (LPARAM)end);
    return (nDex >= 0) ? PhysListC().GetAt(nDex) : NULL;
}

template<class TFIELDID> 
INT_PTR CSubstPhysData<TFIELDID>::FindPhysInfoAllBetween(tPhysPos start, tPhysPos end,
    CTypedPtrArray<CObArray, CPhysInfo<TFIELDID>*> &output) const
{
    return PhysListC().FindAllThat(FnPhysInfoBetween, output, (WPARAM)start, (LPARAM)end);
}

// finding first CPhysInfo<TFIELDID>* located around (containing) given tPhysPos
template<class TFIELDID> 
CPhysInfo<TFIELDID>* CSubstPhysData<TFIELDID>::FindPhysInfoPosIsIn(tPhysPos phpos) const
{
    INT_PTR nDex = PhysListC().FirstThat(FnPhysInfoPosIsIn, (WPARAM)phpos);
    return (nDex >= 0) ? PhysListC().GetAt(nDex) : NULL;
}

//// following methods DO NOT correct positions of other items /////////////////////////////////////////////////
template<class TFIELDID> 
INT_PTR CSubstPhysData<TFIELDID>::AppendPhysToList(CPhysInfo<TFIELDID>const* lpPhysInfo)
{
    return PhysList().Add(const_cast<CPhysInfo<TFIELDID>*>(lpPhysInfo));
}

template<class TFIELDID> 
void CSubstPhysData<TFIELDID>::InsertPhysInfo(
    INT_PTR     indexBefore, 
    CPhysInfo<TFIELDID>* lpPhysInfo)
{
    if (0 <= indexBefore)
    {
        PhysList().InsertAt(indexBefore, lpPhysInfo);
    }
    else
    {
        PhysList().Add(lpPhysInfo);
    }
}

template<class TFIELDID> 
void  CSubstPhysData<TFIELDID>::InsertPhysInfo(
    CPhysInfo<TFIELDID>* lpPhysInfoBefore, 
    CPhysInfo<TFIELDID>* lpPhysInfo)
{
    if (lpPhysInfoBefore)
    {
        InsertPhysInfo(PhysListC().Find(lpPhysInfoBefore), lpPhysInfo);
    }
    else
    {
        AppendPhysToList(lpPhysInfo);
    }
}

template<class TFIELDID> 
void CSubstPhysData<TFIELDID>::RemovPhysInfo(
    INT_PTR nIndex)
{
    this->PhysList().DeleteAndRemoveAt(nIndex);
}

template<class TFIELDID> 
BOOL  CSubstPhysData<TFIELDID>::RemovPhysInfo(
    CPhysInfo<TFIELDID>* lpPhysInfo)
{
    INT_PTR nDex;
    BOOL    result = FALSE;

    if (0 <= (nDex = PhysListC().Find(lpPhysInfo)))
    {
        RemovPhysInfo(nDex);
        result = TRUE;
    }
    return result;
}

//// Following methods DO correct positions of other items //////////
template<class TFIELDID> 
CPhysInfo<TFIELDID>*  CSubstPhysData<TFIELDID>::InsertNewInfo(
    tPhysPos  phpos, 
    TFIELDID  what)
{
    tLogPos       logpos;
    SubstDescr<TFIELDID> const* lpDesc;

    if (NULL == (lpDesc = this->FindMapItem(what)))
    {
        ASSERT(FALSE); return NULL;
    }
    logpos = PhysPos2LogPos(phpos);
    return InsertNewInfo(phpos, new CLogInfo<TFIELDID>(what, logpos));
}

template<class TFIELDID> 
CPhysInfo<TFIELDID>*  CSubstPhysData<TFIELDID>::InsertNewInfo(
    tPhysPos      phpos, 
    CLogInfo<TFIELDID> *lpLogInfo)
{
    size_t     ilen;
    CString    strLeft, strRight, oldphysStr, newphysStr;
    LPCTSTR    lpTxt;
    TFIELDID   what;
    SubstDescr<TFIELDID> const* lpDesc;
    CPhysInfo<TFIELDID>*   lpPhysInfo;
    CPhysInfo<TFIELDID>*   lpPhysBefore;
    CLogInfo<TFIELDID>*    lpLogBefore;

    if (NULL == (lpDesc = this->FindMapItem(what = lpLogInfo->What())))
    {
        ASSERT(FALSE); return NULL;
    }
    ilen = _tcslen(lpTxt = lpDesc->lpTxt);

    try
    {
        lpPhysInfo = new CPhysInfo<TFIELDID>(what, phpos, phpos + ilen);
    }
    catch(CException *e)
    {
        e->Delete(); 
        return NULL;
    }

    if (lpPhysBefore = FindPhysInfoAfter(phpos))
    {
        lpLogBefore = FindMatch(lpPhysBefore);
        MoveAllPhysInfoGreaterEq(phpos, ilen);
        this->InsertLogInfo(lpLogBefore, lpLogInfo);
        this->InsertPhysInfo(lpPhysBefore, lpPhysInfo);
    }
    else
    {
        this->AppendLogInfo(lpLogInfo);
        this->AppendPhysToList(lpPhysInfo);
    }

    oldphysStr = StrPhysStr();
    strLeft  = oldphysStr.Left((int)phpos);
    strRight = oldphysStr.Right((int)(oldphysStr.GetLength() - phpos));
    newphysStr = strLeft + lpTxt + strRight;
    ASSERT(newphysStr == this->LogStr2PhysStr(*this));
    SetPhysStr(newphysStr);

    return lpPhysInfo;
}

template<class TFIELDID> 
BOOL CSubstPhysData<TFIELDID>::DeleteOneInfo(
    CPhysInfo<TFIELDID>* lpInf)
{
    size_t     ilen;
    CLogInfo<TFIELDID>* lpLog;
    tPhysPos   start;
    CString    strTmp;
    BOOL       bRes   = FALSE;

    if ((NULL != lpInf) && (NULL != (lpLog = FindMatch(lpInf))))
    {
        start = lpInf->GetStart();
        ilen = lpInf->GetLength();

        this->RemovPhysInfo(lpInf);
        this->RemoveLogInfo(lpLog);
        if (ilen > 0)
        {
            MoveAllPhysInfoGreaterEq(start, -ilen);
            strTmp = extractSubstr(GetPhysStr(), start, ilen);
            SetPhysStr(strTmp);
        }
        bRes = TRUE;
    }
    else
    {
        ASSERT(FALSE);
    }
    return bRes;
}

template<class TFIELDID> 
size_t CSubstPhysData<TFIELDID>::DeleteAllBetween(
    tPhysPos      start, 
    tPhysPos      end)
{
    CPhysInfo<TFIELDID>* lpInf;
    CString     strTmp, strLog, strPhys;
    tLogPos     nStart;
    size_t      ilen, log_dx;
    size_t      phys_dx = end - start;
    tPhysPos    tempEnd   = end;

    while (lpInf = FindPhysInfoBetween(start, tempEnd))
    {
        ilen = lpInf->GetLength();
        DeleteOneInfo(lpInf);
        tempEnd -= ilen;
    }

    if ((log_dx = tempEnd - start) > 0)
    {
        strPhys = extractSubstr(GetPhysStr(), start, log_dx);
        SetPhysStr(strPhys);

        nStart = PhysPos2LogPos(start);
        strLog = extractSubstr(this->GetLogStr(), nStart, log_dx);
        this->SetLogStr(strLog);
        MoveAllInfoIfPhysGreaterEq(start, -log_dx);

#ifdef _DEBUG
        strTmp = this->LogStr2PhysStr(*this);
        ASSERT(strTmp == strPhys);
#endif
    }

    return phys_dx;
}

template<class TFIELDID> 
BOOL CSubstPhysData<TFIELDID>::InsertText(
    tPhysPos  physIndex, 
    LPCTSTR   sztext)
{
    BOOL  res = FALSE;

    if ((physIndex < 0) || (physIndex > (tPhysPos)StrPhysStr().GetLength()))
    {   // invalid index - out of range
        ASSERT(FALSE);
    }
    else if (NULL != FindPhysInfoPosIsIn(physIndex))
    {   // invalid index - request for insertion in middle of some field ?!
        ASSERT(FALSE);
    }
    else
    {
        int      ilen;
        tLogPos  logIndex = PhysPos2LogPos(physIndex);
        CString  strText = sztext;

        if ((ilen = strText.GetLength()) > 0)
        {
            CString  strPhysNew(GetPhysStr());
            CString  strLogNew(this->GetLogStr());

            strPhysNew.Insert((int)physIndex, sztext);
            strLogNew.Insert((int)logIndex, sztext);

            SetPhysStr(strPhysNew);
            this->SetLogStr(strLogNew);
            MoveAllInfoIfPhysGreaterEq(physIndex, ilen);
#ifdef DEBUG
			CString  strTmp = PhysStr2logStr(*this, &this->MapKeeper());
            ASSERT(strLogNew == strTmp);
#endif // DEBUG
        }
        res = TRUE;
    }

    return res;
}

template<class TFIELDID> 
size_t CSubstPhysData<TFIELDID>::InsertData(
    tPhysPos physIndex, CSubstLogData<TFIELDID> &logData)
{
    LPCTSTR szLog;
    size_t nSuma = 0;

    if (this->InsertText(physIndex, szLog = logData.GetLogStr()))
    { 
        nSuma = _tcslen(szLog);

        tLogPos  const beginLogPos = PhysPos2LogPos(physIndex);
        tPhysPos  insertPhysIndex = physIndex;
        CLogInfoList<TFIELDID> &list = logData.LogList();

        for (tLogPos lasFieldLogPos = 0; list.GetCount() > 0; )
        {
            size_t nLen;
            CPhysInfo<TFIELDID>* phInfo;
            CLogInfo<TFIELDID>* logInfo = list.GetAt(0);
            size_t deltaLogPos = logInfo->GetPos() - lasFieldLogPos;

            insertPhysIndex += deltaLogPos;
            lasFieldLogPos = logInfo->GetPos();
            logInfo->Add2Pos(beginLogPos);

            phInfo = InsertNewInfo(insertPhysIndex, logInfo);
            insertPhysIndex += (nLen = phInfo->GetLength());
            nSuma += nLen;

            list.RemoveAt(0);
        }
    }
    return nSuma;
}

template<class TFIELDID> 
tLogPos CSubstPhysData<TFIELDID>::PhysPos2LogPos(tPhysPos ph) const
{
    INT_PTR      nDex, nSize;
    CPhysInfo<TFIELDID>*  lpTmp;
    tLogPos      result = ph;

    for (nDex = 0, nSize = PhysListC().GetCount(); nDex < nSize; nDex++)
    {
        VERIFY(lpTmp = PhysListC().GetAt(nDex));
        if (lpTmp->GetEnd() <= ph)
            result -= lpTmp->GetLength();
    }

    return result;
}

template<class TFIELDID> 
void CSubstPhysData<TFIELDID>::AppendAsPhysInfo(
    CSubstLogData<TFIELDID> const & logData)
{
    CLogInfo<TFIELDID>*    lplogInf;
    CPhysInfo<TFIELDID>*   physInf;
    SubstDescr<TFIELDID> const* lpDesc;
    size_t        ilen;
    INT_PTR       nDex, nCount;    
    tPhysPos      start, end, suma;
    tLogPos       iLogPos = 0;
    LPCTSTR       lpTxt = NULL;

    ASSERT(PhysListC().IsEmpty());

    for (suma = 0, nDex = 0, nCount = logData.LogListC().GetCount(); nDex <nCount; nDex++)
    {
        VERIFY(lplogInf = logData.LogListC().GetAt(nDex));
        if (lpDesc = this->MapKeeper().FindMapItem(lplogInf->What()))
        {
            ilen = _tcslen(lpTxt = lpDesc->lpTxt);
            if (physInf = AddNewPhysInfo(lplogInf->What()))
            {
                start = suma + (iLogPos = lplogInf->GetPos());
                end = start + ilen;
                suma += ilen;
                physInf->SetStart(start);
                physInf->SetEnd(end);
            }
        }
    }
}

// Assigns its physical data from input argument SubstLogData logData.
// (Note: The input arg. does not change)
template<class TFIELDID> 
void CSubstPhysData<TFIELDID>::AssignPhysFromLog(CSubstLogData<TFIELDID> const & logData)
{
    // DON'T call DeleteContents or ClearContentsLogical - logical contents must be preserved
    ClearContentsPhys();
    AppendAsPhysInfo(logData);
    SetPhysStr(this->LogStr2PhysStr(logData));
}

// "Exporting" the data in this.LogList to the output logData.LogList 
// The method does not modify "this" instance ( could be const ).
template<class TFIELDID> 
void CSubstPhysData<TFIELDID>::ExportLogListAll(
    CSubstLogData<TFIELDID> & logData) const
{
    ExportLogListSel(NULL, logData);
}

// "Exporting" the data in this.LogList that are selected by input arg. selInf
// to the output logData.LogList. The beginnings of exported fields are adjusted 
// to match the selection begin.
// If the argument selInf is null, complete field list is exported
// ( selInf equal to null is interpreted as 'all selected').
template<class TFIELDID> 
void CSubstPhysData<TFIELDID>::ExportLogListSel(
    LPCCSelInfo selInf, CSubstLogData<TFIELDID> & logData) const
{
    CLogInfo<TFIELDID>*  logInfOld;
    CLogInfo<TFIELDID>* logInfNew;
    CPhysInfo<TFIELDID>*   physInf;
    SubstDescr<TFIELDID> const* lpDesc;
    // Make followings just CTypedPtrArrayEx. 
    // Since they share contents with PhysListC, the destructor must not delete pointent objects
    CTypedPtrArrayEx<CObArray, CPhysInfo<TFIELDID>*> listSel;
    CTypedPtrArrayEx<CObArray, CPhysInfo<TFIELDID>*> const*listExported;
    tPhysPos      suma;

    ASSERT(logData.LogListC().IsEmpty());
    /* no, subst. map is not assigned here, but the caller may do it
    logData.AssignSubstMap(this->GetSubstMap());
    */

    if (NULL == selInf)
    {   
        listExported = static_cast<CPkTypedPtrArray<CObArray, CPhysInfo<TFIELDID>*> const*>(&PhysListC());
        suma = 0;
    }
    else
    {
        FindPhysInfoAllBetween(selInf->StartChar(), selInf->EndChar(), listSel);
        listExported = &listSel;
        suma = selInf->StartChar();
    }
    ASSERT_VALID(listExported);

    for (INT_PTR nDex = 0, nSize = listExported->GetCount(); nDex < nSize; nDex++)
    {
        VERIFY(physInf = listExported->GetAt(nDex));
        VERIFY(logInfOld = FindMatch(physInf));
        if (lpDesc = this->FindMapItem(logInfOld->What()))
        {
            if (logInfNew = logData.AppenNewLogInfo(physInf->What()))
            {
                ASSERT(lpDesc->lpTxt);
                logInfNew->SetPos(physInf->GetStart() - suma);
                suma += _tcslen(lpDesc->lpTxt);
            }
        }
    }
}

// "Exporting" all the SubstLogData to the output logData
// The method does not modify this object ( could be const )
// See also ExportLogListAll.
template<class TFIELDID> 
void CSubstPhysData<TFIELDID>::ExportLogAll(CSubstLogData<TFIELDID> & logData) const
{
    CString  strLog;

    logData.Clear();
    ExportLogListAll(logData);
    strLog = PhysStr2logStr(*this, NULL);
    logData.SetLogStr(strLog);
    logData.AssignSubstMap(this->GetSubstMap());
}

template<class TFIELDID> 
void CSubstPhysData<TFIELDID>::ExportLogSel(LPCCSelInfo selInf, CSubstLogData<TFIELDID> &logData) const
{
    CString  strLog;
    size_t nLogSelBeg, nLogSelEnd;

    logData.DeleteContents();
    if ((NULL == selInf) || selInf->IsSel())
    {
        ExportLogListSel(selInf, logData);
        strLog = PhysStr2logStr(*this, NULL);

        if (NULL != selInf)
        {
            nLogSelBeg = PhysPos2LogPos(selInf->StartChar());
            nLogSelEnd = PhysPos2LogPos(selInf->EndChar());
            strLog = strLog.Mid((int)nLogSelBeg, (int)(nLogSelEnd - nLogSelBeg));
        }
        logData.SetLogStr(strLog);
        logData.AssignSubstMap(this->GetSubstMap());
    }
}

template<class TFIELDID> 
void CSubstPhysData<TFIELDID>::AssignPhysData(CSubstPhysData<TFIELDID> const& what)
{
    m_physStr  = what.m_physStr;
}

template<class TFIELDID> 
void  CSubstPhysData<TFIELDID>::AssignPhysList(CSubstPhysList<TFIELDID> const &list)
{
    INT_PTR          nDex, nSize;
    CPhysInfo<TFIELDID>*      lpTmp, lpNew;
    CRuntimeClass*   lpRt;
    CObject*         pObj;

    PhysList().DeleteAndRemoveAll();
    for(nDex = 0, nSize = list.GetCount(); nDex < nSize; nDex++)
    {
        VERIFY(lpTmp = list.GetAt(nDex));
        VERIFY(lpRt = lpTmp->GetRuntimeClass());
        if (pObj = lpRt->CreateObject())
        {
            VERIFY(lpNew = dynamic_cast<CPhysInfo *>(pObj));
            lpNew->Assign(lpTmp);
            PhysList().Add(lpNew);
        }
    }
}

template<class TFIELDID> 
CSubstPhysData<TFIELDID>& CSubstPhysData<TFIELDID>::operator = (CSubstLogData<TFIELDID> const & rhs)
{
    CSubstLogData<TFIELDID>::operator = (rhs);
    AssignPhysFromLog(*this);

    return *this;
}

template<class TFIELDID>
CSubstPhysData<TFIELDID>& CSubstPhysData<TFIELDID>::operator = (CSubstPhysData<TFIELDID> const& rhs)
{
    CSubstLogData::operator = (rhs);
    AssignPhysData(rhs);
    AssignPhysList(rhs.PhysListC());

    return *this;
}

template<class TFIELDID> 
CString CSubstPhysData<TFIELDID>::PhysStr2logStr(
    CSubstPhysData<TFIELDID> const& physData,
    SubstMapKeeper<TFIELDID> const* lpMapKeeper)
{
    CPhysInfo<TFIELDID>*   physInf;
    size_t        itmplen, istart, idone;
    INT_PTR       nDex, nSize;
    SubstDescr<TFIELDID> const* lpDesc;
    CString       strTmp, strLog;
    CString       strPhys = physData.GetPhysStr();
    int           physlen = strPhys.GetLength();

    ASSERT(physData.PhysListC().GetCount() == physData.LogListC().GetCount());
    if (NULL == lpMapKeeper)
    {
        lpMapKeeper = &physData.MapKeeper();
    }
    for (idone = 0, nDex = 0, nSize = physData.PhysListC().GetCount(); nDex < nSize; nDex++)
    {
        physInf = physData.PhysListC().GetAt(nDex);
        if (lpDesc = lpMapKeeper->FindMapItem(physInf->What()))
        {
            if (idone < (istart = physInf->GetStart()))
            {
                strTmp = strPhys.Mid((int)idone, (int)(istart - idone));
                strLog += strTmp;
            }
            idone = physInf->GetEnd();
        }
        else
        {
            ASSERT(FALSE);
        }
    }
    if ((itmplen = strPhys.GetLength() - idone) > 0)
    {
        strTmp = strPhys.Mid((int)idone, (int)itmplen);
        strLog += strTmp;
    }

    return strLog;
}

template<class TFIELDID> 
void CSubstPhysData<TFIELDID>::Serialize(CArchive& ar)
{
    CSubstLogData<TFIELDID>::Serialize(ar);

    if (ar.IsLoading())
    {
        ar >> m_physStr;
    }
    else
    {
        ar << m_physStr;
    }

    m_physlist.Serialize(ar);
}

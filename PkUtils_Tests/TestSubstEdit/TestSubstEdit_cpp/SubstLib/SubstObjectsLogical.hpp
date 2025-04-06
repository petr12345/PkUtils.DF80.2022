// SubstObjectsLogical.hpp :
// templates CLogInfo<TFIELDID> and CSubstLogData<TFIELDID> implementation file

/////////////////////////////////////////////////////////////////////////////
// INCLUDE FILES
/////////////////////////////////////////////////////////////////////////////
#include "stdafx.h"
#include "afx.h"

/////////////////////////////////////////////////////////////////////////////
// MANIFESTED CONSTANTS & MACROS
/////////////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////////////
// EXTERNAL SYMBOLS
/////////////////////////////////////////////////////////////////////////////
extern CString WINAPI extractSubstr(
    LPCTSTR szStr, 
    size_t  istart, 
    size_t  ilen);

/////////////////////////////////////////////////////////////////////////////
// STATIC FUNCTIONS
/////////////////////////////////////////////////////////////////////////////

template<class TFIELDID> 
CString CALLBACK getReplacementTextFn(SubstDescr<TFIELDID> const* lpDesc)
{
    CString strRes;
    if (NULL != lpDesc)
    {
        strRes = lpDesc->lpTxt;
    }
    return strRes;
}

/////////////////////////////////////////////////////////////////////////////
// CLASS DEFINITIONS
/////////////////////////////////////////////////////////////////////////////

IMPLEMENT_SERIAL_T(CLogInfo, TFIELDID, tLogInfoPredecessor, LOGINFO_VERSION);

template<class TFIELDID> 
CLogInfo<TFIELDID>::CLogInfo() : tLogInfoPredecessor()
{
    SetWhat((TFIELDID)kInvalidSubstElemId);
    SetPos(0);
}

template<class TFIELDID> 
CLogInfo<TFIELDID>::CLogInfo(TFIELDID  what) : tLogInfoPredecessor()
{
    SetWhat(what);
    SetPos(0);
}

template<class TFIELDID> 
CLogInfo<TFIELDID>::CLogInfo(
    TFIELDID  what,
    tLogPos      pos) : tLogInfoPredecessor()
{
    SetWhat(what);
    SetPos(pos);
}

template<class TFIELDID> 
CLogInfo<TFIELDID>::~CLogInfo()
{
}

template<class TFIELDID> 
BOOL CLogInfo<TFIELDID>::Assign(CLogInfo<TFIELDID> const*lprhs)
{
    if (lprhs->IsKindOf(RUNTIME_CLASS(CLogInfo)))
    {
        m_what  = lprhs->What();
        m_pos = lprhs->GetPos();
        return TRUE;
    }
    else
    {
        ASSERT(FALSE);
        return FALSE;
    }
}

template<class TFIELDID> 
CLogInfo<TFIELDID>& CLogInfo<TFIELDID>::operator = (CLogInfo<TFIELDID> const& rhs)
{
    Assign(&rhs);
    return *this;
}

template<class TFIELDID> 
void CLogInfo<TFIELDID>::Serialize(CArchive& ar)
{
    tLogInfoPredecessor::Serialize(ar);

    if (ar.IsLoading())
    {
        // In case the line below does not compile for the particular TFIELDID type,
        // you have to supply for that type an operator
        // CArchive& AFXAPI operator>>(CArchive& ar, TFIELDID &val)
        ar >> m_what;
        ar >> m_pos;
    }
    else
    {
        ar << m_what;
        ar << m_pos;
    }
}

#ifdef _DEBUG

template<class TFIELDID> 
void CLogInfo<TFIELDID>::AssertValid() const
{
    tLogInfoPredecessor::AssertValid();
}

template<class TFIELDID> 
void CLogInfo<TFIELDID>::Dump(CDumpContext& dc) const
{
    tLogInfoPredecessor::Dump(dc);
}
#endif // _DEBUG

/////////////////////////////////////////////////////////////////////////////

IMPLEMENT_SERIAL_T(CSubstLogData, TFIELDID, tSubstLogDataPredecessor, SUBSTLOGDATA_VERSION );

template<class TFIELDID> 
CSubstLogData<TFIELDID>::CSubstLogData() : tSubstLogDataPredecessor()
{
}

template<class TFIELDID> 
CSubstLogData<TFIELDID>::CSubstLogData(
    SubstDescr<TFIELDID> const* lpMap) : tSubstLogDataPredecessor(), m_map(lpMap)
{
}

template<class TFIELDID> 
CSubstLogData<TFIELDID>::CSubstLogData(
    SubstDescr<TFIELDID> const* lpMap,
    LPCTSTR       szLogStr) : tSubstLogDataPredecessor(), m_map(lpMap)
{
    SetLogStr(szLogStr);
}

template<class TFIELDID> 
CSubstLogData<TFIELDID>::CSubstLogData(
    CLogInfo<TFIELDID> const & rhs) : tSubstLogDataPredecessor()
{
    *this = rhs;
}

template<class TFIELDID> 
CSubstLogData<TFIELDID>::CSubstLogData(CSubstLogData<TFIELDID> const& rhs)
{
    this->Assign(rhs);
}

template<class TFIELDID> 
CSubstLogData<TFIELDID>::~CSubstLogData()
{
    ClearContentsLogical();
}

template<class TFIELDID> 
INT_PTR CSubstLogData<TFIELDID>::GetLogInfoIndex(CLogInfo<TFIELDID> const*lpPos) const
{
    return LogListC().Find(const_cast<CLogInfo<TFIELDID>*>(lpPos));
}

template<class TFIELDID> 
void CSubstLogData<TFIELDID>::DestroyList(void)
{
    m_logList.DeleteAndRemoveAll();
}

template<class TFIELDID> 
void CSubstLogData<TFIELDID>::ClearContentsLogical(void)
{
    DestroyList();
    m_logStr.Empty();
}

template<class TFIELDID> 
void  CSubstLogData<TFIELDID>::DeleteContents()
{
    ClearContentsLogical();
}

template<class TFIELDID> 
SubstDescr<TFIELDID> const* CSubstLogData<TFIELDID>::FindMapItem(
    TFIELDID item) const
{
    return SubstMapKeeper<TFIELDID>::FindMapItem(GetSubstMap(), item);
}

template<class TFIELDID> 
INT_PTR CSubstLogData<TFIELDID>::AppendLogInfo(CLogInfo<TFIELDID> const*lpLogInfo)
{
    return LogList().Add(const_cast<CLogInfo<TFIELDID>*>(lpLogInfo));
}

template<class TFIELDID> 
CLogInfo<TFIELDID>* CSubstLogData<TFIELDID>::AppenNewLogInfo(TFIELDID  what)
{
    CLogInfo<TFIELDID>* lpLogInfo = NULL;

    try
    {
        if (lpLogInfo = new CLogInfo<TFIELDID>(what))
        {
            AppendLogInfo(lpLogInfo);
        }
    }
    catch(CException *e)
    {
        e->Delete();
        lpLogInfo = NULL;
    }

    return lpLogInfo;
}

template<class TFIELDID> 
void CSubstLogData<TFIELDID>::InsertLogInfo(
    INT_PTR     indexBefore, 
    CLogInfo<TFIELDID>* lpLogInfo)
{
    if (0 <= indexBefore)
    {
        m_logList.InsertAt(indexBefore, lpLogInfo);
    }
    else
    {
        m_logList.Add(lpLogInfo);
    }
}

template<class TFIELDID> 
void  CSubstLogData<TFIELDID>::InsertLogInfo(
    CLogInfo<TFIELDID>* lpLogInfoBefore, 
    CLogInfo<TFIELDID>* lpLogInfo)
{
    if (lpLogInfoBefore)
    {
        InsertLogInfo(GetLogInfoIndex(lpLogInfoBefore), lpLogInfo);
    }
    else
    {
        AppendLogInfo(lpLogInfo);
    }
}

template<class TFIELDID> 
void CSubstLogData<TFIELDID>::RemoveLogInfo(INT_PTR  nIndex)
{
    LogList().DeleteAndRemoveAt(nIndex);
}

template<class TFIELDID> 
BOOL  CSubstLogData<TFIELDID>::RemoveLogInfo(CLogInfo<TFIELDID> const* lpLogInfo)
{
    INT_PTR nIndex;
    BOOL  result = FALSE;

    if (0 <= (nIndex = GetLogInfoIndex(lpLogInfo)))
    {
        RemoveLogInfo(nIndex);
        result = TRUE;
    }
    return result;
}

template<class TFIELDID> 
void CSubstLogData<TFIELDID>::AssignSerializableData(CSubstLogData<TFIELDID> const & what)
{
    ClearContentsLogical();
    // No, m_lpMap is NOT serialized, hence it is NOT assigned here
    /* m_lpMap   = what.m_lpMap; */
    m_logStr   = what.GetLogStr();  // assign m_logStr
    AssignLogList(what.LogListC());  // duplicate list
}

template<class TFIELDID> 
void  CSubstLogData<TFIELDID>::AssignLogList(CPkTypedPtrArray<CObArray, CLogInfo<TFIELDID>*> const &list)
{
    CLogInfo<TFIELDID>*  lpTmp;
    CLogInfo<TFIELDID>*  lpNew;
    CRuntimeClass*    lpRt;
    CObject*          pObj;

    DestroyList();
    for(INT_PTR ii = 0, nSize = list.GetCount(); ii < nSize; ii++)
    {
        VERIFY(lpTmp = list[ii]);
        VERIFY(lpRt = lpTmp->GetRuntimeClass());
        if (pObj = lpRt->CreateObject())
        {
            VERIFY(lpNew = dynamic_cast<CLogInfo<TFIELDID>*>(pObj));
            lpNew->Assign(lpTmp);
            m_logList.Add(lpNew);
        }
    }
}

template<class TFIELDID> 
CString CSubstLogData<TFIELDID>::LogStr2PhysStr(
    CSubstLogData<TFIELDID> const & logData)
{
    return LogStrToPhysStr(logData, getReplacementTextFn);
}

template<class TFIELDID> 
CString CSubstLogData<TFIELDID>::LogStrToPhysStr(
    CSubstLogData<TFIELDID> const & logData, 
    fnDescrToText lpFn)
{
    size_t        itmplen;
    CLogInfo<TFIELDID> const*  lplogInf;
    SubstDescr<TFIELDID> const* lpDesc;
    tLogPos       iLogPos = 0;
    tLogPos       iLogCopied = 0;
    LPCTSTR       lpTxt = NULL;
    CString       strPhys, strTmp;
    CString       strLog = logData.GetLogStr();
    SubstMapKeeper<TFIELDID> const& mapKeeper = logData.MapKeeper();

    for(INT_PTR ii = 0, nCount = logData.LogListC().GetCount(); ii < nCount; ii++)
    {
        lplogInf = logData.LogListC().GetAt(ii);
        if (lpDesc = mapKeeper.FindMapItem(lplogInf->What()))
        {
            ASSERT(lplogInf->GetPos() <= (tLogPos)strLog.GetLength());
            // add another piece of logical text
            strTmp = strLog.Mid((int)iLogCopied, (int)((iLogPos = lplogInf->GetPos()) - iLogCopied));
            strPhys += strTmp;
            // add the field text, or generally the replacement
            strPhys += (*lpFn)(lpDesc);
            // update the position in logical text
            iLogCopied = iLogPos;
        }
        else
        {
            ASSERT(FALSE);
        }
    }
    // if there is remaining logical text not copied so far
    if ((itmplen = strLog.GetLength() - iLogCopied) > 0)
    {
        strTmp = strLog.Mid((int)iLogCopied, (int)itmplen);
        strPhys += strTmp;
    }

    return strPhys;
}

template<class TFIELDID> 
void CSubstLogData<TFIELDID>::Assign(CSubstLogData<TFIELDID> const &rhs)
{
    // Do not assign subst map here, as it is not part of assignment or serialization
    /* AssignSubstMap(rhs.GetSubstMap); */
    AssignSerializableData(rhs);
}

/// <summary>
/// Substitute special xml characters in the logical string
/// for their entities representation like "&amp;" or "&lt;"
/// See http://xml.silmaril.ie/authors/specials/
/// </summary>
/// <see cref="ReplaceLogXmlPartsBack"/>
template<class TFIELDID> 
void CSubstLogData<TFIELDID>::ReplaceLogXmlCharsThere()
{   // must replace ampresands first
    ReplaceLogTextAllThrough("&", "&amp;");   // 1.
    ReplaceLogTextAllThrough("<", "&lt;");    // 2.
    ReplaceLogTextAllThrough(">", "&gt;");    // 3.
    ReplaceLogTextAllThrough("\"", "&quot;"); // 4.
    ReplaceLogTextAllThrough("'", "&apos");   // 5.
}

/// <summary>
/// Substitute special xml characters in the logical string
/// from their entities representation back to single character
/// </summary>
/// <see cref="ReplaceLogXmlCharsThere"/>
template<class TFIELDID> 
void CSubstLogData<TFIELDID>::ReplaceLogXmlPartsBack()
{   // Must do replacement in reverse order than ReplaceLogXmlCharsThere();
    // hence must replace ampresands as last
    ReplaceLogTextAllThrough("&apos", "'");   // 5.
    ReplaceLogTextAllThrough("&quot;", "\""); // 4.
    ReplaceLogTextAllThrough("&gt;", ">");    // 3.
    ReplaceLogTextAllThrough("&lt;", "<");    // 2.
    ReplaceLogTextAllThrough("&amp;", "&");   // 1.
}

// Replaces the logical text part specified by startIndex and nReplacedLenght
// by a new string strNewText. The new string may be empty or even null.
// startIndex - Position of modification</param>
// nReplacedLenght - The length of the part of original text to be replaced </param>
// strNewText - new text to be inserted
template<class TFIELDID> 
void CSubstLogData<TFIELDID>::ReplaceLogTextPart(
    tLogPos startIndex, int nReplacedLenght, LPCTSTR szNewText)
{
    int nAddedLength;
    CString strOldLog = GetLogStr();
    CString strNewLog = strOldLog;
    CString strNewText(szNewText);

    if ((startIndex < 0) || (startIndex > (tLogPos)strOldLog.GetLength()))
    {
        /* throw new ArgumentException("startIndex"); */
        ASSERT(FALSE);
        return;
    }
    if ((nReplacedLenght < 0) || (nReplacedLenght > (int)(strOldLog.GetLength() - startIndex)))
    {
        /* throw new ArgumentException("nReplacedLenght"); */
        ASSERT(FALSE);
        return;
    }
    // find all affected fields ( fields for which the deletion index in text preceeds the field )
    CTypedPtrArray<CObArray, CLogInfo<TFIELDID>*> listAffected;
    for(INT_PTR ii = 0, nSize = LogListC().GetCount(); ii < nSize; ii++)
    {
        CLogInfo<TFIELDID>* pInfo;
        if (startIndex < (pInfo = LogListC()[ii])->GetPos())
        {
            listAffected.Add(pInfo);
        }
    }

    if (nReplacedLenght > 0)
    {
        strNewLog = extractSubstr(strNewLog, startIndex, nReplacedLenght);
        for(INT_PTR jj = 0, nSize = listAffected.GetCount(); jj < nSize; jj++)
        {
            listAffected.GetAt(jj)->Add2Pos(-nReplacedLenght);
        }
    }
    if (0 < (nAddedLength = strNewText.GetLength()))
    {
        strNewLog.Insert((int)startIndex, strNewText);
        for(INT_PTR kk = 0, nSize = listAffected.GetCount(); kk < nSize; kk++)
        {
            listAffected.GetAt(kk)->Add2Pos(nAddedLength);
        }
    }
    this->SetLogStr(strNewLog);
}

/// <summary>
/// Go through all current logical text, and replace occurencies of 
/// substring strOldPart to substring strNewPart.
/// Positions of fields are updated approprittaly.
/// </summary>
/// <param name="strOldPart"></param>
/// <param name="strNewPart"></param>
/// <see cref="ReplaceLogTextPart"/>
template<class TFIELDID> 
void CSubstLogData<TFIELDID>::ReplaceLogTextAllThrough(CString strOldPart, CString strNewPart)
{
    if (strOldPart.GetLength() == 0)
    {
        /* throw new ArgumentException("strOldPart"); */
        ASSERT(FALSE);
        return;
    }

    int nDexFound;
    int nOldPart = strOldPart.GetLength();
    int nNewPart = strNewPart.GetLength();

    for (int nSearPos = 0;;)
    {
        CString strOldLog(GetLogStr());

        if (0 <= (nDexFound = strOldLog.Find(strOldPart, nSearPos)))
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

template<class TFIELDID> 
void CSubstLogData<TFIELDID>::AssignPlainText(LPCTSTR szText)
{
    SubstDescr<TFIELDID> const* substMap = GetSubstMap();

    ClearContentsLogical();
    // assing the input text as a logicl text for now; folloing code will filter it
    SetLogStr(szText);

    // 1. parse the plain text and remove all known fields specification.
    // Note: Must do this BEFORE calling ReplaceLogXmlPartsBack,
    // since ReplaceLogXmlPartsBack will put back specific characters '<' '>',
    // that otherwise can mess-up with fields beggings
    for (tLogPos nDex = 0; ; )
    {
        CString strLog(GetLogStr());
        CLogInfo<TFIELDID>* lpFound = NULL;

        if (nDex >= (tLogPos)strLog.GetLength())
        {
            break;
        }

        // enumerate known fields; check if the text on specific position matches
        for(SubstDescr<TFIELDID> const* descr = substMap;;descr++)
        {
            if (kInvalidSubstElemId == descr->valId)
            {
                break;
            }
            else
            {
                CString strMid;
                CString strLocal = descr->lpTxt;
                int nLocalLength = strLocal.GetLength();

                if (nDex + nLocalLength <= (size_t)strLog.GetLength())
                {
                    strMid = strLog.Mid((int)nDex, strLocal.GetLength());
                    if (strMid == strLocal)
                    {	// match found; create a new field replacing the text
                        ReplaceLogTextPart(nDex, nLocalLength, NULL);
                        lpFound = AppenNewLogInfo(descr->valId);
                        lpFound->SetPos(nDex);
                        break;
                    }
                }
            }
        }
        if (NULL == lpFound)
        {
            nDex++;
        }
    }
    // 2. now perform the xml special chars substitution
    ReplaceLogXmlPartsBack();
}

template<class TFIELDID> 
CString CSubstLogData<TFIELDID>::GetPlainText() const
{
    // 1. Initialize temporary data
    CSubstLogData tempData(*this);
    tempData.AssignSubstMap(this->GetSubstMap());
    // 2. Substitute special xml characters in the logical string. 
    // See http://xml.silmaril.ie/authors/specials/
    // Note: this may modify the length of logical string in tempData, 
    // thus causing corresponding shift of fields positions.
    tempData.ReplaceLogXmlCharsThere();
    // 3. Convert current temp. logical string to physival string by inserting fields
    return LogStr2PhysStr(tempData);
}

template<class TFIELDID> 
CSubstLogData<TFIELDID> & CSubstLogData<TFIELDID>::operator = (LPCTSTR szLogStr)
{
    Clear();
    SetLogStr(szLogStr);
    return *this;
}

template<class TFIELDID> 
CSubstLogData<TFIELDID> & CSubstLogData<TFIELDID>::operator = (CSubstLogData<TFIELDID> const &rhs)
{
    Assign(rhs);
    return *this;
}

template<class TFIELDID> 
void CSubstLogData<TFIELDID>::Serialize(CArchive& ar)
{
    tSubstLogDataPredecessor::Serialize(ar);

    if (ar.IsLoading())
    {
        ar >> m_logStr;
    }
    else
    {
        ar << m_logStr;
    }
    m_logList.Serialize(ar);
}


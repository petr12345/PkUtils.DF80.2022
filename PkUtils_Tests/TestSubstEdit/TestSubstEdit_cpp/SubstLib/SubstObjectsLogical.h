/////////////////////////////////////////////////////////////////////////////
// SubstObjectsLogical.h
/////////////////////////////////////////////////////////////////////////////

#ifndef __SUBSTOBJECTSLOGICAL_H__
#define __SUBSTOBJECTSLOGICAL_H__

/////////////////////////////////////////////////////////////////////////////
// INCLUDE FILES
/////////////////////////////////////////////////////////////////////////////
#include "AfxTempl.h"
#include "PkArray.h"
#include "SubstMapping.h"
#include "RuntimeTpt.h"

/////////////////////////////////////////////////////////////////////////////
// MANIFESTED CONSTANTS & MACROS
/////////////////////////////////////////////////////////////////////////////
#define    LOGINFO_VERSION                0
#define    SUBSTLOGDATA_VERSION           0

/////////////////////////////////////////////////////////////////////////////
// TYPES
/////////////////////////////////////////////////////////////////////////////

template<class TFIELDID> class CLogInfo;
template<class TFIELDID> class CSubstLogData;

#define tLogInfoPredecessor       CObject
#define tSubstLogDataPredecessor  CObject
typedef size_t                    tLogPos;

#ifndef kInvalidSubstElemId
#define kInvalidSubstElemId  0
#endif

/////////////////////////////////////////////////////////////////////////////
// CLASES
/////////////////////////////////////////////////////////////////////////////

/** CLogInfo is a logical coordinate of field
  "Logical" coordinates do not include interior length of fields.
*/
template<class TFIELDID> class CLogInfo : public tLogInfoPredecessor
{
	DECLARE_SERIAL_T(CLogInfo, TFIELDID);

protected:
    // The field ID
    TFIELDID m_what;
    // The logical position in the string; the index is considered without displayed body of fields.
    // When computing logical position, the 'ordinary' character has a length 1, 
    // while the (complete) field has a length 0.
    // Hence, this logical position is actually a logical index of character 
    // immediatelly AFTER the field.
    tLogPos  m_pos;

public:
    CLogInfo();
    CLogInfo(TFIELDID what);
    CLogInfo(TFIELDID what, tLogPos pos);
    virtual ~CLogInfo();

    TFIELDID  What(void) const
    { return m_what; }
    void  SetWhat(TFIELDID id) 
    { m_what = id; }

    tLogPos const GetPos(void) const
    { return m_pos; }
    void SetPos(tLogPos pos) 
    { m_pos = pos; }
    void Add2Pos(size_t idelta) 
    { m_pos += idelta; }

    virtual BOOL Assign(CLogInfo<TFIELDID> const* lprhs);
    CLogInfo<TFIELDID>& operator = (CLogInfo<TFIELDID> const& rhs);
    void Serialize(CArchive& ar);
#ifdef _DEBUG
    virtual void AssertValid() const;
    virtual void Dump(CDumpContext& dc) const;
#endif // _DEBUG
};

/** CLogInfoList teplate is actually just a nickname for CPkTypedPtrArray of CLogInfo<TFIELDID>*
    Its purpose is making long syntax more simple.
*/
template<class TFIELDID> class CLogInfoList : public CPkTypedPtrArray<CObArray, CLogInfo<TFIELDID>*>
{
};

/** CSubstLogData keeps "logical substitution data", 
    i.e. logical data needed for serialization and displaying of CSubstEdit contents.
*/
template<class TFIELDID> class CSubstLogData : public tSubstLogDataPredecessor
{
	DECLARE_SERIAL_T(CSubstLogData, TFIELDID);

public:
    typedef CString CALLBACK fnDescrToText(SubstDescr<TFIELDID> const* lpDescr);
    typedef fnDescrToText *lpfnDescrToText;

protected:
    // the logical string ( text without fields )
    CString       m_logStr; 
    // list of log. positions
    CLogInfoList<TFIELDID> m_logList;

private:
    // map of (field id) -> (field text)
    SubstMapKeeper<TFIELDID> m_map;

public:
    CSubstLogData();
    CSubstLogData(SubstDescr<TFIELDID> const* lpMap);
    CSubstLogData(SubstDescr<TFIELDID> const*, LPCTSTR szLogStr);
    CSubstLogData(CLogInfo<TFIELDID> const& rhs);
    CSubstLogData(CSubstLogData<TFIELDID> const& rhs);
    virtual ~CSubstLogData();

    LPCTSTR GetLogStr(void) const
    { return m_logStr; }
    void  SetLogStr(LPCTSTR szLogStr)
    { m_logStr = szLogStr; }

    CLogInfoList<TFIELDID> &LogList()
    { return m_logList; }
    CLogInfoList<TFIELDID> const & LogListC() const
    { return m_logList; }

    SubstDescr<TFIELDID> const* GetSubstMap(void) const
    { 
        return MapKeeper().GetSubstMap(); 
    }
    void AssignSubstMap(SubstDescr<TFIELDID> const* lpMap)
    { 
        ASSERT(lpMap); 
        m_map.AssignSubstMap(lpMap); 
    }

    void ClearContentsLogical(void);
    virtual void  DeleteContents();

    INT_PTR GetLogInfoIndex(CLogInfo<TFIELDID> const* lpPos) const;
    SubstDescr<TFIELDID> const* FindMapItem(TFIELDID item) const;

    CLogInfo<TFIELDID>* AppenNewLogInfo(TFIELDID  what);
    INT_PTR AppendLogInfo(CLogInfo<TFIELDID> const* lpLogInfo);
    void    InsertLogInfo(INT_PTR indexBefore, CLogInfo<TFIELDID>* lpLogInfo);
    void    InsertLogInfo(CLogInfo<TFIELDID>* lpLogInfoBefore, CLogInfo<TFIELDID>* lpLogInfo);

    void   RemoveLogInfo(INT_PTR nIndex);
    BOOL   RemoveLogInfo(CLogInfo<TFIELDID> const*lpLogInfo);

    static CString LogStrToPhysStr(CSubstLogData<TFIELDID> const &logData, lpfnDescrToText lpFn);
    static CString LogStr2PhysStr(CSubstLogData<TFIELDID> const &logData);

    virtual void Assign(CSubstLogData<TFIELDID> const &rhs);
    virtual void AssignPlainText(LPCTSTR szText);

    CString GetPlainText() const;

    CSubstLogData<TFIELDID> & operator = (LPCTSTR szLogStr);
    CSubstLogData<TFIELDID> & operator = (CSubstLogData<TFIELDID> const & rhs);

    void   Serialize(CArchive& ar);

protected:
    SubstMapKeeper<TFIELDID> const& MapKeeper() const
    { return m_map; }

    void  AssignSerializableData(CSubstLogData<TFIELDID> const & what);
    void  AssignLogList(CPkTypedPtrArray<CObArray, CLogInfo<TFIELDID>*>  const &list);
    void  ReplaceLogXmlCharsThere();
    void  ReplaceLogXmlPartsBack();
    virtual void ReplaceLogTextPart(tLogPos startIndex, int nReplacedLenght, LPCTSTR szNewText);
    void ReplaceLogTextAllThrough(CString strOldPart, CString strNewPart);
    /* not needed so far
    LPCLogInfoList DuplicateList() const;
    */
    void  DestroyList(void);
};

#include "SubstObjectsLogical.hpp"

#endif // __SUBSTOBJECTSLOGICAL_H__

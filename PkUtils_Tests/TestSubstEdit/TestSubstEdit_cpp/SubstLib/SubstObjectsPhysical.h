//////////////////////////////////////////////////////////////////////////////////////////////////////////
// SubstObjectsPhysical.h


#ifndef __SUBSTOBJECTSPHYSICAL_H__
#define __SUBSTOBJECTSPHYSICAL_H__

/////////////////////////////////////////////////////////////////////////////
// INCLUDE FILES
/////////////////////////////////////////////////////////////////////////////
#include "AfxTempl.h"
#include "PkArray.h"
#include "SelInfo.h"
#include "SubstObjectsLogical.h"

/////////////////////////////////////////////////////////////////////////////
// MANIFESTED CONSTANTS & MACROS
/////////////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////////////
// TYPES
/////////////////////////////////////////////////////////////////////////////

template<class TFIELDID> class CPhysInfo;
template<class TFIELDID> class CSubstPhysData;

#define tPhysInfoPredecessor      CObject
typedef size_t                    tPhysPos;

/////////////////////////////////////////////////////////////////////////////
// CLASES
/////////////////////////////////////////////////////////////////////////////

/** CPhysInfo is a physical coordinate of field
*/
template<class TFIELDID> class CPhysInfo : public tPhysInfoPredecessor
{
	DECLARE_DYNCREATE_T(CPhysInfo, TFIELDID);
protected:
   TFIELDID  m_what;
   tPhysPos  m_start;
   tPhysPos  m_end;

public:
   CPhysInfo();
   CPhysInfo(TFIELDID what);
   CPhysInfo(TFIELDID what, tPhysPos start, tPhysPos end);
   virtual ~CPhysInfo();

   TFIELDID  What(void) const
   { return m_what; }
   void  SetWhat(TFIELDID id) 
   { m_what = id; }

   tPhysPos const GetStart(void) const
   { return m_start; }
   void SetStart(tPhysPos start) 
   { m_start = start; }
   void Add2Start(size_t idelta)
   { m_start += idelta; }

   tPhysPos const GetEnd(void) const
   { return m_end; }
   void SetEnd(tPhysPos end) 
   { m_end = end; }
   void Add2End(size_t idelta)
   { m_end += idelta; }

   size_t GetLength(void) const
   {
       ASSERT(GetStart() <= GetEnd());
       return (GetEnd() - GetStart());
   }

   virtual BOOL Assign(CPhysInfo<TFIELDID>const* lprhs);
   CPhysInfo<TFIELDID>& operator = (CPhysInfo<TFIELDID> const& rhs);

   void Serialize(CArchive& ar);
#ifdef _DEBUG
    virtual void AssertValid() const;
    virtual void Dump(CDumpContext& dc) const;
#endif
};

///////////////////////////////

/** CSubstPhysList teplate is just a nickname for CPkTypedPtrArray of CPhysInfo<TFIELDID>*
    Its purpose is making long syntax more simple.
*/
template<class TFIELDID> class CSubstPhysList : public CPkTypedPtrArray<CObArray, CPhysInfo<TFIELDID>*>
{
};

/** CSubstPhysData  keeps "substitution physical data", 
    i.e. an internal data of CSubstEdit control used during its editing.
    Note: CSubstPhysData do not have to be serialized; 
    they all will be reconstructed from serialied CSubstLogData.
*/
template<class TFIELDID> class CSubstPhysData  : public CSubstLogData<TFIELDID>
{
   DECLARE_DYNCREATE_T(CSubstPhysData, TFIELDID)

protected:
   CString        m_physStr;
   CSubstPhysList<TFIELDID> m_physlist;  // list of phys. positions
private:

public:
   CSubstPhysData();
   CSubstPhysData(SubstDescr<TFIELDID> const* lpMap);
   CSubstPhysData(CSubstLogData<TFIELDID> const &logData);
   CSubstPhysData(CSubstPhysData<TFIELDID> const &pattern);
   virtual ~CSubstPhysData();

   LPCTSTR GetPhysStr(void) const
    { return m_physStr; }
   CString const &StrPhysStr(void) const
    { return m_physStr; }
   void  SetPhysStr(LPCTSTR szstr)
    { m_physStr = szstr; }

   CSubstPhysList<TFIELDID>& PhysList(void)
     { return m_physlist; }
   CSubstPhysList<TFIELDID> const & PhysListC(void) const
     { return m_physlist; }

   void   ClearContentsPhys(void);
   virtual void   DeleteContents(void);

   CPhysInfo<TFIELDID>* FindMatch(CLogInfo<TFIELDID>* logInf) const;
   CLogInfo<TFIELDID>*  FindMatch(CPhysInfo<TFIELDID>* physInf) const;
   CPhysInfo<TFIELDID>* FindPhysInfoBefore(tPhysPos phpos) const;
   CPhysInfo<TFIELDID>* FindPhysInfoAfter(tPhysPos phpos) const;
   CPhysInfo<TFIELDID>* FindPhysInfoBetween(tPhysPos start, tPhysPos end) const;
   INT_PTR   FindPhysInfoAllBetween(tPhysPos start, tPhysPos end, 
       CTypedPtrArray<CObArray, CPhysInfo<TFIELDID>*> &output) const;
   CPhysInfo<TFIELDID>* FindPhysInfoPosIsIn(tPhysPos phpos) const;

   //// following methods DO correct positions of other items  //////////
   CPhysInfo<TFIELDID>*  InsertNewInfo(tPhysPos phpos, TFIELDID what);
   CPhysInfo<TFIELDID>*  InsertNewInfo(tPhysPos phpos, CLogInfo<TFIELDID> *lpLogInfo);

   BOOL         DeleteOneInfo(CPhysInfo<TFIELDID>* lpInf);
   size_t       DeleteAllBetween(tPhysPos start, tPhysPos end);
   BOOL         InsertText(tPhysPos physIndex, LPCTSTR  sztext);
   size_t       InsertData(tPhysPos physIndex, CSubstLogData<TFIELDID> &logData);

   //// conversions between log and phys /////////////////////////////////
   tLogPos PhysPos2LogPos(tPhysPos ph) const;
   void   AppendAsPhysInfo(CSubstLogData<TFIELDID> const & logData);
   void   AssignPhysFromLog(CSubstLogData<TFIELDID> const & logData);
   void   ExportLogListAll(CSubstLogData<TFIELDID> & logData) const;
   void	  ExportLogListSel(LPCCSelInfo selInf, CSubstLogData<TFIELDID> & logData) const;
   void   ExportLogAll(CSubstLogData<TFIELDID> & logData) const;
   void   ExportLogSel(LPCCSelInfo selInf, CSubstLogData<TFIELDID> & logData) const;

   void   AssignPhysData(CSubstPhysData<TFIELDID> const& what);
   static CString PhysStr2logStr(
       CSubstPhysData<TFIELDID> const& physData, 
       SubstMapKeeper<TFIELDID> const* lpMapKeeper);

   void   Serialize(CArchive& ar);

   CSubstPhysData<TFIELDID>& operator = (CSubstLogData<TFIELDID> const& rhs);
   CSubstPhysData<TFIELDID>& operator = (CSubstPhysData<TFIELDID> const& rhs);

protected:
   void MoveAllPhysInfoGreaterEq(tPhysPos greaterOrEq, size_t by);
   void MoveAllInfoIfPhysGreaterEq(tPhysPos greaterOrEq, size_t by);

   //// following methods DO NOT correct positions of other items //////////
   void    InsertPhysInfo(INT_PTR indexBefore, CPhysInfo<TFIELDID>* lpPhysInfo);
   void    InsertPhysInfo(CPhysInfo<TFIELDID>* lpPhysInfoBefore, CPhysInfo<TFIELDID>* lpPhysInfo);

   CPhysInfo<TFIELDID>*  AddNewPhysInfo(TFIELDID  what);
   INT_PTR AppendPhysToList(CPhysInfo<TFIELDID>const* lpPhysInfo);

   void   RemovPhysInfo(INT_PTR nIndex);
   BOOL   RemovPhysInfo(CPhysInfo<TFIELDID>* lpPhysInfo);

   void  AssignPhysList(CSubstPhysList<TFIELDID> const &list);

private:
   static BOOL CALLBACK FnPhysInfoPosLowerEq(CPhysInfo<TFIELDID>* phinf, WPARAM wParam, LPARAM lParam)
   {
       return (phinf->GetEnd() <= (tPhysPos)wParam);
   }

   static BOOL CALLBACK FnPhysInfoPosGreaterEq(CPhysInfo<TFIELDID>* phinf, WPARAM wParam, LPARAM lParam)
   {
       return (phinf->GetStart() >= (tPhysPos)wParam);
   }

    static BOOL CALLBACK FnPhysInfoBetween(CPhysInfo<TFIELDID>* phinf, WPARAM wParam, LPARAM lParam)
    {
        return (phinf->GetStart() >= (tPhysPos)wParam) && (phinf->GetEnd() <= (tPhysPos)lParam);
    }

    static BOOL CALLBACK FnPhysInfoPosIsIn(CPhysInfo<TFIELDID>* phinf, WPARAM wParam, LPARAM lParam)
    {
        return (phinf->GetStart() < (tPhysPos)wParam) && ((tPhysPos)wParam < phinf->GetEnd());
    }

    static void CALLBACK FnMoveAllPhysInfoGreaterEq(CPhysInfo<TFIELDID>* phinf, WPARAM wParam, LPARAM lParam)
    {
        if (phinf->GetStart() >= (tPhysPos)wParam)
        {
            phinf->Add2Start(lParam);
            phinf->Add2End(lParam);
        }
    }
};

#include "SubstObjectsPhysical.hpp"

#endif // __SUBSTOBJECTSPHYSICAL_H__

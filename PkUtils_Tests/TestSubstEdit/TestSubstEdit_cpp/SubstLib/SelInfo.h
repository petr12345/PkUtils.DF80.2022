#if !defined(AFX_SUBSTEDIT_H__7842B239_ECE7_11D1_807B_0020E480683A__INCLUDED_)
#define AFX_SUBSTEDIT_H__7842B239_ECE7_11D1_807B_0020E480683A__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// SelInfo.h : header file
//

/////////////////////////////////////////////////////////////////////////////
//	INCLUDE FILES
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//	MANIFESTED CONSTANTS & MACROS
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//	TYPES
/////////////////////////////////////////////////////////////////////////////
class CSelInfo;

typedef CSelInfo *LPCSelInfo;
typedef CSelInfo const *LPCCSelInfo;

/////////////////////////////////////////////////////////////////////////////
// CSelInfo 

/** CSelInfo is a general text selection info
*/
class CSelInfo : CObject
{
protected:
    size_t m_nStartChar;
    size_t m_nEndChar;
    BOOL   m_bCaretIsLast;
    static CSelInfo const m_allSelection;

public:
    CSelInfo();
    CSelInfo(int nPos);
    CSelInfo(int nStart, int nEnd, BOOL bLast);
    CSelInfo(CSelInfo const& rhs);
    
    size_t    StartChar(void) const noexcept
    { return m_nStartChar; }
    size_t   EndChar(void) const noexcept
    { return m_nEndChar; }

    size_t CaretChar(void) const;
    
    BOOL  IsCaretLast(void) const noexcept
    { return m_bCaretIsLast; }

    void SetCaretLast(BOOL bLast)
    { m_bCaretIsLast = bLast; }

    BOOL  IsSel(void) const
    { return StartChar() != EndChar(); }

    BOOL IsAllSelection() const;

    void MakeAllSelection();

    static CSelInfo const & AllSelection() noexcept
    { return m_allSelection; }
    
    CSelInfo&  operator = (CSelInfo const& sel);
    CSelInfo&  operator += (size_t nDelta);

#if _DEBUG
    virtual void AssertValid() const;
#endif // DEBUG
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SUBSTEDIT_H__7842B239_ECE7_11D1_807B_0020E480683A__INCLUDED_)

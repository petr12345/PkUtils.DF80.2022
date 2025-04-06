// SelInfo.cpp : implementation file
//

/////////////////////////////////////////////////////////////////////////////
//  INCLUDE FILES
/////////////////////////////////////////////////////////////////////////////
#include "stdafx.h"
#include "SelInfo.h"

/////////////////////////////////////////////////////////////////////////////
//  MANIFESTED CONSTANTS & MACROS
/////////////////////////////////////////////////////////////////////////////
#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif


/////////////////////////////////////////////////////////////////////////////
// CSelInfo 

CSelInfo const CSelInfo::m_allSelection(0, -1, TRUE);

CSelInfo::CSelInfo()
{
    m_nStartChar = m_nEndChar = 0;
    m_bCaretIsLast = FALSE;
}

CSelInfo::CSelInfo(int nPos)
{
    m_nStartChar = nPos;
    m_nEndChar = nPos;
    m_bCaretIsLast = FALSE;
}

CSelInfo::CSelInfo(
   int  nStart, 
   int  nEnd, 
   BOOL bLast)
{
    m_nStartChar = nStart;
    m_nEndChar = nEnd;
    m_bCaretIsLast = bLast;
}

CSelInfo::CSelInfo(CSelInfo const& rhs)
{
    *this = rhs;
}

CSelInfo& CSelInfo::operator = (CSelInfo const& rhs)
{
    m_nStartChar = rhs.m_nStartChar;
    m_nEndChar = rhs.m_nEndChar;
    m_bCaretIsLast = rhs.m_bCaretIsLast;

    return *this;
}

CSelInfo&  CSelInfo::operator += (size_t nDelta)
{
    m_nStartChar += nDelta;
    m_nEndChar += nDelta;
    return *this;
}

size_t CSelInfo::CaretChar(void) const
{
    return (!IsCaretLast()) ? StartChar() : EndChar();
}

BOOL CSelInfo::IsAllSelection() const
{  
    return (StartChar() == 0 && EndChar() == -1 && IsCaretLast()); 
}

void CSelInfo::MakeAllSelection()
{
    m_nStartChar = 0; 
    m_nEndChar = -1; 
    m_bCaretIsLast = true;
}

#if _DEBUG
void CSelInfo::AssertValid() const
{
    ASSERT(IsAllSelection() || (StartChar() <= EndChar()));
}
#endif // DEBUG

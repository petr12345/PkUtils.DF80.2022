// SubstEdit.hpp : template CSubstEdit<TFIELDID> implementation file
//

/////////////////////////////////////////////////////////////////////////////
//  INCLUDE FILES
/////////////////////////////////////////////////////////////////////////////
#include "ClipWrapper.h"

/////////////////////////////////////////////////////////////////////////////
//  MANIFESTED CONSTANTS & MACROS
/////////////////////////////////////////////////////////////////////////////
#ifndef GWL_WNDPROC
#define GWL_WNDPROC         (-4)
#endif

/////////////////////////////////////////////////////////////////////////////
//  PRIVATE VARIABLES
/////////////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////////////
//  FUNCTIONS
/////////////////////////////////////////////////////////////////////////////
#ifdef _DEBUG
extern void WINAPI traceMsg(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
#endif // _DEBUG

/////////////////////////////////////////////////////////////////////////////
// CSubstEdit

BEGIN_TEMPLATE_MESSAGE_MAP(CSubstEdit, TFIELDID, CEdit)
    //{{AFX_MSG_MAP(CSubstEdit)
    ON_CONTROL_REFLECT_EX(EN_CHANGE, OnEnChange)
    //}}AFX_MSG_MAP
END_MESSAGE_MAP()

template<class TFIELDID> 
CSubstEdit<TFIELDID>::CSubstEdit()
{
    m_oldWndProc = NULL;
    m_nOrigCallLevel = 0;
    m_nChangeNotifyLock = 0;
}

template<class TFIELDID> 
CSubstEdit<TFIELDID>::CSubstEdit(CSubstLogData<TFIELDID> const & logData) 
    : m_data(logData)
{
    m_oldWndProc = NULL;
    this->m_bHandlingEnter = false;
    m_nOrigCallLevel = 0;
    m_nChangeNotifyLock = m_nChangeModifyTempCount = m_nLockHookLevel = 0;
}

template<class TFIELDID> 
CSubstEdit<TFIELDID>::~CSubstEdit()
{
    ASSERT(OrigCallLevel() == 0);
}

template<class TFIELDID> 
bool CSubstEdit<TFIELDID>::CheckStyles()
{
    DWORD dwStyle = this->GetStyle();
    bool bRes = true;

    if ( (0 != (dwStyle & ES_MULTILINE)) && (0 == (dwStyle & ES_AUTOVSCROLL)) )
    {   // The control supports ES_MULTILINE only with ES_AUTOVSCROLL
        ASSERT(FALSE);
        bRes = false;
    }
    return bRes;
}

template<class TFIELDID> 
void CSubstEdit<TFIELDID>::PreSubclassWindow()
{
    CWnd* pParent;
    ISubstDescrProvider<TFIELDID>* iprov;

    // call base class
    CEdit::PreSubclassWindow();
    // adjust styles if needed
    if (CheckStyles())
    {
        // subclass SubstEditNewPro
        SetNewWndProc(SubstEditNewPro);
        // find and assign the substitution map
        if (NULL != (pParent = this->GetParent()))
        {
            if (NULL != (iprov = dynamic_cast<ISubstDescrProvider<TFIELDID>*>(pParent)))
            {
                this->PhysData().AssignSubstMap(iprov->GetSubstDescr());
            }
        }
    }
}

// Replacement ( fixup ) of original PosFromChar method
template<class TFIELDID> 
CPoint CSubstEdit<TFIELDID>::MyPosFromChar(UINT nChar) const
{
    CPoint ptRes = PosFromChar(nChar);

    if (ptRes.x < 0)
    {
        ptRes = CPoint(0, 0);
        if (nChar > 0)
        {
            UINT     n_Len;
            CDC     *lpdc;
            CSize    sz;
            CString  strTmp, strText;

            GetWindowText(strText);
            n_Len = strText.GetLength(); 
            ASSERT(n_Len <= nChar);
            if (n_Len > 0)
            {
                ptRes = MyPosFromChar(n_Len - 1);
            }
            strTmp = strText.GetAt(n_Len - 1);
            lpdc = ((CWnd*)this)->GetDC();
            sz = lpdc->GetTextExtent(strTmp);
            ((CWnd*)this)->ReleaseDC(lpdc);
            ptRes.x += sz.cx;
        }
    }
    else
    {
        while ((UINT)GetCharIndexFromPosition(ptRes) > nChar)
        {
            ptRes.x--;
        }
    }

    return ptRes;
}

template<class TFIELDID> 
CSelInfo&  CSubstEdit<TFIELDID>::GetSelInfo(
    CSelInfo&  info) const
{
    int nStartChar, nEndChar;

    const_cast<CSubstEdit<TFIELDID>*>(this)->CallOrigProc(EM_GETSEL, (WPARAM)&nStartChar, (LPARAM)&nEndChar);
    CSelInfo tmpsel(nStartChar, nEndChar, false);

    if (tmpsel.StartChar() != tmpsel.EndChar())
    {
        CPoint   pcaret = GetCaretPos();
        CPoint   pstart = PosFromChar((UINT)tmpsel.StartChar());
        CPoint   pend   = PosFromChar((UINT)tmpsel.EndChar());

        if (pend.x < 0)   // "overflow"
        {
            tmpsel.SetCaretLast((pstart.x != pcaret.x));
        }
        else if (pcaret.x == pend.x)
        {
            tmpsel.SetCaretLast(TRUE);
        }
        else if (pcaret.x == pstart.x)
        {
            /* tmpsel.m_bCaretIsLast = FALSE; already is */ 
        }
        else
        {
            ASSERT(FALSE);
        }
    }

    info = tmpsel;
    return info;
}

template<class TFIELDID> 
void CSubstEdit<TFIELDID>::SetSelInfo(
    CSelInfo const& info)
{
    if (info.IsAllSelection() || !info.IsCaretLast())
    {
        this->CallOrigProc(EM_SETSEL, info.StartChar(), info.EndChar());
    }
    else
    {
        this->CallOrigProc(EM_SETSEL, info.EndChar(), info.StartChar());
    }
}

#ifdef _DEBUG
template<class TFIELDID> 
void CSubstEdit<TFIELDID>::AssertSelValidity(
    CSelInfo const& sel) const
{
    ASSERT(NULL == RFPhysDataC().FindPhysInfoPosIsIn(sel.StartChar()));
    if (sel.IsSel())
    {
        if (sel.IsAllSelection())
        { // is there anything to test?
        }
        else
        {
            ASSERT(NULL == RFPhysDataC().FindPhysInfoPosIsIn(sel.EndChar()));
        }
    }
}
#endif // _DEBUG

template<class TFIELDID> 
int CSubstEdit<TFIELDID>::GetFirstCharIndexFromLine(int line) const
{
  return (int) const_cast<CSubstEdit<TFIELDID>*>(this)->CallOrigProc(EM_LINEINDEX, (WPARAM)line);
}

// Retrieves the zero-based index of the character nearest the specified point.
template<class TFIELDID> 
int CSubstEdit<TFIELDID>::GetCharIndexFromPosition(POINT const &pt, int *pLineIndex /*= NULL*/) const
{   // the zero-based line and character indices of the character nearest the specified point
    int nIndicies = CharFromPos(pt);
    if (NULL != pLineIndex)
    {
        *pLineIndex = HIWORD((UINT)nIndicies);
    }
    return LOWORD((UINT)nIndicies);
}

template<class TFIELDID> 
int CSubstEdit<TFIELDID>::LineCol2CharPos(int line, int col) const
{
    int  suma = 0;

    if ((line == -1) || (col == -1))
    {
        return -1;
    }
    suma = GetFirstCharIndexFromLine(line);
    suma += col;

    return suma;
}

template<class TFIELDID> 
tPhysPos CSubstEdit<TFIELDID>::FindPosOutsidePhys(tPhysPos iorig, eFindDirection direction) const
{
    CPhysInfo<TFIELDID>* lpPhys;
    tPhysPos          res = iorig;

    if (lpPhys = RFPhysDataC().FindPhysInfoPosIsIn(iorig))
    {
        size_t delta_a = iorig - lpPhys->GetStart();
        size_t delta_b = lpPhys->GetEnd() - iorig;

        ASSERT((delta_a > 0) && (delta_b > 0));
        if ( (direction == eFindBackward) || (direction == eFindCloser && (delta_a <= delta_b)) )
            res = lpPhys->GetStart();
        else
            res = lpPhys->GetEnd();
    }

    return res;
}

template<class TFIELDID> 
void CSubstEdit<TFIELDID>::InitializeText()
{
    LockHookFn();
    CallOrigProc(WM_SETTEXT, 0, (LPARAM)(LPVOID)PhysData().GetPhysStr());
    UnlockHookFn();
}

template<class TFIELDID> 
void CSubstEdit<TFIELDID>::ChangeModifyTempCountReset()
{
    this->m_nChangeModifyTempCount = 0;
}

template<class TFIELDID> 
void CSubstEdit<TFIELDID>::ChangeModifyTempCountIncrement()
{
    this->m_nChangeModifyTempCount++;
}

template<class TFIELDID> 
void CSubstEdit<TFIELDID>::NotifyFixPrologue()
{
    if (0 == this->m_nChangeNotifyLock++)
    {
        ChangeModifyTempCountReset();
    }
}

template<class TFIELDID> 
void CSubstEdit<TFIELDID>::NotifyFixEpilogue()
{
    CWnd  *pParent;
    if (0 == --this->m_nChangeNotifyLock)
    {
        if ( 0 < this->m_nChangeModifyTempCount)
        {
            ChangeModifyTempCountReset();
            if (NULL != (pParent = this->GetParent()))
            {
                ::SendMessage(
                    pParent->GetSafeHwnd(), 
                    WM_COMMAND, 
                    MAKEWPARAM(this->GetDlgCtrlID(), EN_CHANGE), 
                    (LPARAM)this->GetSafeHwnd());
            }
        }
    }
}

template<class TFIELDID> 
LRESULT CALLBACK CSubstEdit<TFIELDID>::SubstEditNewPro(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    CSelInfo    selInf;
    CSubstEdit *pEdit;
    LRESULT     lRes = 0;

    if (NULL == (pEdit = (CSubstEdit*)CWnd::FromHandle(hwnd)))
    {
        ASSERT(FALSE); return 0;
    }
    /* _DBG(traceMsg(hwnd, msg, wParam, lParam)); */

    // 1. prologue
    pEdit->NotifyFixPrologue();

    // 2. message processing
    if (pEdit->IsLockedOrigFn())
    {	//just process the message 
        pEdit->CallOrigProc(msg, wParam, lParam);
    }
    else
    {
        bool bHandled = true;

        switch (msg)
        {
            case WM_KEYDOWN:
                switch (wParam)
                {
                    case VK_LEFT :
                    case VK_RIGHT :
                    case VK_HOME:
                    case VK_END:
                        /* TRACE1("\ncaret moved %i ", GetTickCount()); */
                        lRes = pEdit->MoveCaretHorizontal(wParam, lParam);
                        break;

                    case VK_UP:
                    case VK_DOWN:
                        lRes = pEdit->MoveCaretVertical(wParam, lParam);
                        break;

                    case VK_DELETE:
                        lRes = pEdit->MyOnVk_Delete(lParam);
                        break;

                    default:
                        bHandled = false;
                        break;
                }
                break;

            case WM_CHAR:
                lRes = pEdit->MyOnWmChar(wParam, lParam, bHandled);
                break;

            case WM_LBUTTONDOWN:
                lRes = pEdit->MyOnWmLButtonDown(wParam, lParam);
                break;

            case WM_MOUSEMOVE:
                lRes = pEdit->MyOnWmMouseMove(wParam, lParam);
                break;

            case WM_LBUTTONDBLCLK:
                /* lRes = 0; already is */
                break;

            case WM_CUT:
                lRes = pEdit->MyCopy(true);
                break;

            case WM_COPY:
                lRes = pEdit->MyCopy(false);
                break;

            case WM_PASTE:
                lRes = pEdit->MyOnPaste();
                break;

            default:
                bHandled = FALSE;
                break;
        }

        if (!bHandled)
        {
            lRes = pEdit->CallOrigProc(msg, wParam, lParam);
        }
    }

    // 3. epilogue
    // Ensures that EN_CHANGE is sent only after all changes are truly completed
    pEdit->NotifyFixEpilogue();

    return lRes;
}

template<class TFIELDID> 
BOOL CSubstEdit<TFIELDID>::SetNewWndProc(WNDPROC lpWndProc)
{
    HWND  hWnd;

    ASSERT(!IsSubclassed());
    if (::IsWindow(hWnd = (HWND)*this))
    {
        m_oldWndProc = (WNDPROC)::GetWindowLongPtr(hWnd, GWL_WNDPROC);
        ::SetWindowLongPtr(hWnd, GWL_WNDPROC, (LONG_PTR)lpWndProc);
        return TRUE;
    }
    else
    {
        ASSERT(FALSE);
        return FALSE;
    }
}

template<class TFIELDID> 
LRESULT CSubstEdit<TFIELDID>::CallOrigProc(
    UINT   msg, 
    WPARAM wParam, 
    LPARAM lParam)
{
    LRESULT  lRes = 0;

    m_nOrigCallLevel++;
    ASSERT(OldWndProc());

    lRes = CallWindowProc(OldWndProc(), (HWND)*this, msg, wParam, lParam);

    m_nOrigCallLevel--;
    ASSERT(OrigCallLevel() >= 0);

    return lRes;
}

template<class TFIELDID> 
void CSubstEdit<TFIELDID>::EmptyEditCtrlUndoBuffer()
{
    LockHookFn();
    CallOrigProc(EM_EMPTYUNDOBUFFER);
    UnlockHookFn();
}

template<class TFIELDID> 
BOOL CSubstEdit<TFIELDID>::InsertNewInfo(
    TFIELDID  what)
{
    CSelInfo     selInf;
    tPhysPos     phpos;
    CPhysInfo<TFIELDID>*  lpPh;
    BOOL         res = FALSE;

    ASSERT(::IsWindow((HWND)*this));
    ASSERT(this->IsSubclassed());
    if (lpPh = PhysData().InsertNewInfo(GetSelInfo(selInf).CaretChar(), what))
    {
        NotifyFixPrologue();
        LockHookFn();
        phpos = lpPh->GetEnd();
        CallOrigProc(WM_SETTEXT, 0, (LPARAM)(LPVOID)PhysData().GetPhysStr());
        CallOrigProc(EM_SETSEL, (WPARAM)phpos, (LPARAM)phpos);
        CallOrigProc(EM_SCROLLCARET);
        ChangeModifyTempCountIncrement();  // make sure EN_CHANGE is send by NotifyFixEpilogue()
        UnlockHookFn();
        NotifyFixEpilogue();

        res = TRUE;
    }

    return res;
}

template<class TFIELDID> 
size_t CSubstEdit<TFIELDID>::ModifyDataOnInsertion(
    size_t   iPos, 
    LPCTSTR  szOldText, 
    LPCTSTR  szNewText)
{
    size_t   ioldLen, inewLen, irold;
    CString  strTmp;
    CString  strOld = szOldText;
    CString  strNew = szNewText;
    size_t   delta = 0;

    if (strOld != strNew)
    {
        delta = (inewLen = strNew.GetLength()) - (ioldLen = strOld.GetLength());
        ASSERT(delta > 0);
        irold = ioldLen  - iPos;
        ASSERT(strOld.Left((int)iPos) == strNew.Left((int)iPos));
        ASSERT(strOld.Right((int)irold) == strNew.Right((int)irold));
        strTmp = strNew.Mid((int)iPos, (int)delta);
        VERIFY(PhysData().InsertText(iPos, strTmp));
        ASSERT(PhysData().GetPhysStr() == strNew);
    }
    return delta;
}

/////////////////////////////////////////////////////////////////////////////
// CSubstEdit special handlers

template<class TFIELDID> 
LRESULT CSubstEdit<TFIELDID>::DeleteSel_WmCharBack(
    CSelInfo const& selInf, 
    LPARAM      lParam)
{
    LRESULT  lRes;

    ASSERT(selInf.IsSel());
    PhysData().DeleteAllBetween(selInf.StartChar(), selInf.EndChar());
    lRes = CallOrigProc(WM_CHAR, VK_BACK, lParam);
    CEdit::EmptyUndoBuffer();
    return lRes;
}

template<class TFIELDID> 
LRESULT CSubstEdit<TFIELDID>::DeleteSel_VKDelete(
    CSelInfo const& selInf, 
    LPARAM      lParam)
{
    LRESULT  lRes;

    ASSERT(selInf.IsSel());
    PhysData().DeleteAllBetween(selInf.StartChar(), selInf.EndChar());
    lRes = CallOrigProc(WM_KEYDOWN, VK_DELETE, lParam);
    CEdit::EmptyUndoBuffer();
    return lRes;
}

template<class TFIELDID> 
LRESULT CSubstEdit<TFIELDID>::DeleteSel_WmCharStrange(
    CSelInfo const& selInf, 
    WPARAM      wParam, 
    LPARAM      lParam)
{
    CString  strOld, strNew;
    LRESULT  lRes;

    ASSERT(selInf.IsSel());
    GetWindowText(strOld);
    lRes = CallOrigProc(WM_CHAR, wParam, lParam);
    GetWindowText(strNew);
    if (strOld != strNew)
    {
        PhysData().DeleteAllBetween(selInf.StartChar(), selInf.EndChar());
        CEdit::EmptyUndoBuffer();
        ASSERT(strNew == PhysData().GetPhysStr());
    }
    return lRes;
}

template<class TFIELDID> 
LRESULT CSubstEdit<TFIELDID>::BackspaceDeleteNotSel(
    CSelInfo const& selInf, 
    LPARAM      lParam)
{
#ifdef _DEBUG
    CString     strTmp;
#endif
    CSelInfo    sel;
    CPhysInfo<TFIELDID>* lpPhys;
    size_t      iCaret, iStart;
    LRESULT     lRes = 0;

    ASSERT(!selInf.IsSel());
    if ((iCaret = selInf.CaretChar()) > 0)
    {
        if ((lpPhys = PhysData().FindPhysInfoBefore(iCaret)) && (lpPhys->GetEnd() == iCaret))
        {
            iStart = lpPhys->GetStart();
        }
        else
        {
            CString strLeft = PhysData().StrPhysStr().Left((int)iCaret);
            CString lastTwos = strLeft.Right(2);

            if (0 == strLeft.Right(2).Compare(_T("\r\n")))
                iStart = iCaret - 2;
            else
                iStart = iCaret - 1;
        }
        PhysData().DeleteAllBetween(iStart, iCaret);
        for(;;)
        {
            lRes = CallOrigProc(WM_CHAR, VK_BACK, lParam);
            if (GetSelInfo(sel).StartChar() == iStart)
                break;
        }
        CEdit::EmptyUndoBuffer();
#ifdef _DEBUG
        GetWindowText(strTmp); ASSERT(strTmp == PhysData().GetPhysStr());
#endif
    }
    else
    {
        lRes = CallOrigProc(WM_CHAR, VK_BACK, lParam);
        CEdit::EmptyUndoBuffer();
#ifdef _DEBUG
        GetWindowText(strTmp); ASSERT(strTmp == PhysData().GetPhysStr());
#endif
    }

    return lRes;
}

template<class TFIELDID> 
LRESULT CSubstEdit<TFIELDID>::VkDeleteNotSel(
    CSelInfo const& selInf, 
    LPARAM      lParam)
{
    BYTE        pbKeyState[256];
    CString     strTmp, strRight;
    CPhysInfo<TFIELDID>* lpPhys;
    size_t      iCaret, iEnd, nDelLimit;
    LRESULT     lRes = 0;

    ASSERT(!selInf.IsSel());
    GetWindowText(strTmp); 
    ASSERT(strTmp == PhysData().GetPhysStr());
    if ((iCaret = selInf.CaretChar()) < (size_t)strTmp.GetLength())
    {
        if ((lpPhys = PhysData().FindPhysInfoAfter(iCaret)) && (lpPhys->GetStart() == iCaret))
        {
            nDelLimit = iEnd = lpPhys->GetEnd();
        }
        else
        {
            strRight = strTmp.Right((int)(strTmp.GetLength() - iCaret));
            if (0 == strRight.Left(2).Compare(_T("\r\n")))
                iEnd = iCaret + 2;
            else
                iEnd = iCaret + 1;
            nDelLimit = iCaret + 1;
        }
        PhysData().DeleteAllBetween(iCaret, iEnd);

        GetKeyboardState((LPBYTE)&pbKeyState);
        pbKeyState[VK_CONTROL] &= 0x7F;
        SetKeyboardState((LPBYTE)&pbKeyState);
        for(size_t ii = iCaret; ii < nDelLimit; ii++)
        {
            lRes = CallOrigProc(WM_KEYDOWN, VK_DELETE, lParam);
        }
        CEdit::EmptyUndoBuffer();
#ifdef _DEBUG
        GetWindowText(strTmp); ASSERT(strTmp == PhysData().GetPhysStr());
#endif
    }
    else
    {
        lRes = CallOrigProc(WM_KEYDOWN, VK_DELETE, lParam);
#ifdef _DEBUG
        GetWindowText(strTmp); ASSERT(strTmp == PhysData().GetPhysStr());
#endif
    }

    return lRes;
}

template<class TFIELDID> 
LRESULT CSubstEdit<TFIELDID>::WmCharDoInsetChar(
    CSelInfo const& selInf, 
    WPARAM      wParam, 
    LPARAM      lParam)
{
    CString  strNew, strOld;
    size_t   iCaret = selInf.CaretChar();
    LRESULT  lRes = 0;

#ifdef _DEBUG
    ASSERT(!selInf.IsSel());
    GetWindowText(strOld);
    ASSERT(strOld == PhysData().GetPhysStr() );
#else
    strOld = PhysData().GetPhysStr();
#endif
    lRes = CallOrigProc(WM_CHAR, wParam, lParam);
    GetWindowText(strNew);
    if (ModifyDataOnInsertion(iCaret, strOld, strNew) > 0)
    {
        CEdit::EmptyUndoBuffer();
    }

    return lRes;
}

template<class TFIELDID> 
LRESULT CSubstEdit<TFIELDID>::MoveCaretHorizontal(WPARAM wParam, LPARAM lParam)
{
    BYTE        pbKeyState[256];
    CSelInfo    selInf;
    LRESULT     lRes = 0;

    ASSERT((wParam == VK_LEFT) || (wParam == VK_RIGHT) || (wParam == VK_HOME) || (wParam == VK_END));
    GetKeyboardState((LPBYTE)&pbKeyState);
    pbKeyState[VK_CONTROL] &= 0x7F;
    SetKeyboardState((LPBYTE)&pbKeyState);

    lRes = CallOrigProc(WM_KEYDOWN, wParam, lParam);
    while (PhysData().FindPhysInfoPosIsIn(GetSelInfo(selInf).CaretChar()))
    {   // move caret this way to keep shift-selecting if shift is pressed
        CallOrigProc(WM_KEYDOWN, wParam, lParam);
    }

    return lRes;
}

template<class TFIELDID> 
LRESULT CSubstEdit<TFIELDID>::MoveCaretVertical(WPARAM wParam, LPARAM lParam)
{
    BYTE        pbKeyState[256];
    tPhysPos    tPosCurrent;
    CSelInfo    selInf;
    LRESULT     lRes = 0;

    ASSERT((wParam == VK_UP) || (wParam == VK_DOWN));
    GetKeyboardState((LPBYTE)&pbKeyState);
    pbKeyState[VK_CONTROL] &= 0x7F;
    SetKeyboardState((LPBYTE)&pbKeyState);

    lRes = CallOrigProc(WM_KEYDOWN, wParam, lParam);
    if (PhysData().FindPhysInfoPosIsIn(tPosCurrent = GetSelInfo(selInf).CaretChar()))
    {
        eFindDirection direction = (wParam == VK_UP) ? eFindBackward : eFindForward;
        WPARAM wkSubstitute = (wParam == VK_UP) ? VK_LEFT : VK_RIGHT;
        tPhysPos  tPosGoal = FindPosOutsidePhys(tPosCurrent, direction);

        while (tPosCurrent != tPosGoal)
        {   // move caret this way to keep shift-selecting if shift is pressed
            CallOrigProc(WM_KEYDOWN, wkSubstitute, lParam);
            tPosCurrent = GetSelInfo(selInf).CaretChar();
        }
    }

    return lRes;
}

template<class TFIELDID> 
LRESULT CSubstEdit<TFIELDID>::MyOnWmChar(WPARAM wParam, LPARAM lParam, bool &bHandled)
{
    CSelInfo  oldSel;
    LRESULT   lRes = 0;

    GetSelInfo(oldSel);
    _DBG(AssertSelValidity(oldSel));
    /* TRACE1("\n -- WM_CHAR; char = %x", wParam); */

    bHandled = false;
    switch (wParam)
    {
        case VK_FINAL: // Ctrl+X; let the default processing to transform it to command Win32.WM_CUT
        case VK_CANCEL: // Ctrl+C; let the default processing to transform it to command Win32.WM_COPY
            /* bHandled = false; already is */
            break;

        case VK_LBUTTON: // Ctrl+A
            SetSelInfo(CSelInfo::AllSelection());
            bHandled = true;
            break;

        case VK_BACK:
            if (oldSel.IsSel())
                lRes = DeleteSel_WmCharBack(oldSel, lParam);
            else
                lRes = BackspaceDeleteNotSel(oldSel, lParam);
            bHandled = true;
            break;

        default:
            if (oldSel.IsSel())
            {
                if ((wParam < VK_SPACE) && (wParam != VK_RETURN))
                {
                    lRes = DeleteSel_WmCharStrange(oldSel, wParam, lParam);
                }
                else
                {
                    lRes = DeleteSel_WmCharBack(oldSel, lParam);
                    lRes = WmCharDoInsetChar(oldSel, wParam, lParam);
                }
            }
            else
            {
                lRes = WmCharDoInsetChar(oldSel, wParam, lParam);
            }
            bHandled = true;
            break;
    }

    return lRes;
}

template<class TFIELDID> 
LRESULT CSubstEdit<TFIELDID>::MyOnVk_Delete(LPARAM lParam)
{
    CSelInfo  oldSel;
    LRESULT   lRes = 0;

    GetSelInfo(oldSel);
    _DBG(AssertSelValidity(oldSel));
    if (oldSel.IsSel())
        lRes = DeleteSel_VKDelete(oldSel, lParam);
    else 
        lRes = VkDeleteNotSel(oldSel, lParam);

    return lRes;
}

template<class TFIELDID> 
LRESULT CSubstEdit<TFIELDID>::MyOnWmLButtonDown(WPARAM wParam, LPARAM lParam)
{
    UINT        fwKeys  = (UINT)wParam; 
    CPoint      pt(LOWORD(lParam), HIWORD(lParam));
    size_t      iround;
    CPoint      pttmp;
    int         nAllLength = RFPhysDataC().StrPhysStr().GetLength();
    int         charPos = GetCharIndexFromPosition(pt);
    int         istrPos = LineCol2CharPos(HIWORD(charPos), LOWORD(charPos));
    LRESULT     lRes    = 0;

    if ((0 <= istrPos) && (istrPos <= nAllLength))
    {
        iround = FindPosOutsidePhys(istrPos);
        if (iround != istrPos)
        {
            pttmp = MyPosFromChar((UINT)iround);
            lParam = MAKELPARAM(pttmp.x, pttmp.y);
        }
        lRes = CallOrigProc(WM_LBUTTONDOWN, wParam, lParam);
    }

    return lRes;
}

template<class TFIELDID> 
LRESULT CSubstEdit<TFIELDID>::MyOnWmMouseMove(WPARAM wParam, LPARAM lParam)
{
    UINT        fwKeys  = (UINT)wParam; 
    CPoint      pt(LOWORD(lParam), HIWORD(lParam));
    LRESULT     lRes    = 0;

    if (fwKeys & MK_LBUTTON)
    {
        CPoint  pttmp;
        size_t  iround;
        int     istrPos;
        int     charPos = GetCharIndexFromPosition(pt);

        if (0 > (istrPos = LineCol2CharPos(HIWORD(charPos), LOWORD(charPos))))
        {
            return 0;
        }
        if ((iround = FindPosOutsidePhys(istrPos)) != istrPos)
        {
            pttmp = MyPosFromChar((UINT)iround);
            lParam = MAKELPARAM(pttmp.x, pttmp.y);
        }
    }
    lRes = CallOrigProc(WM_MOUSEMOVE, wParam, lParam);

    return lRes;
}

// Exports the selected contents as a plain text, 
// and puts the resulting plain text on the clipboard.
template<class TFIELDID> 
LRESULT CSubstEdit<TFIELDID>::MyCopy(bool bCut)
{
    CSubstLogData<TFIELDID> tempData;
    CString  strPlain;
    CSelInfo selInf;
    LRESULT lRes = 0;

    GetSelInfo(selInf);
    _DBG(AssertSelValidity(selInf));
    if (selInf.IsSel())
    {
        PhysData().ExportLogSel(&selInf, tempData);
        strPlain = tempData.GetPlainText();
        ClipWrapper::SetText(strPlain);
        if (bCut)
        {
            this->MyOnVk_Delete(1);
        }
        lRes = 1;
    }
    return lRes;
}

// Pastes the clipboard contents as a plain text, 
// converts the plain text to field list and logical string,
// and inserts the result on current selection position.
template<class TFIELDID> 
LRESULT CSubstEdit<TFIELDID>::MyOnPaste()
{
    CString   strPaste;
    CComBSTR  bsPaste;
    bool      bPaste = false;

    if (ClipWrapper::GetText(strPaste))
    {
        bPaste = true;
    }
    else if (ClipWrapper::GetUnicodeText(bsPaste))
    {
        strPaste = bsPaste;
        bPaste = true;
    }

    if (bPaste)
    {
        CString   strOld, strNew;
        CSelInfo  currentSel;
        CSubstLogData<TFIELDID> tempData(this->PhysData().GetSubstMap());

        // 1. parse pasted text; result is in tempData
        tempData.AssignPlainText(strPaste);
        // 2. delete selection if there is any
        if (GetSelInfo(currentSel).IsSel())
        {
            _DBG(AssertSelValidity(currentSel));
            DeleteSel_WmCharBack(currentSel, 0x000e0001);
            GetSelInfo(currentSel);
            ASSERT(!currentSel.IsSel());
        }
        currentSel += PhysData().InsertData(currentSel.StartChar(), tempData);
        InitializeText();
        SetSelInfo(currentSel);
        EmptyEditCtrlUndoBuffer();
    }

    return 0;
}

/////////////////////////////////////////////////////////////////////////////
// CSubstEdit message map handlers

// Following helps to avoid sending EN_CHANGE notifications BEFORE i am completely done 
// with the change. Related code is NotifyFixPrologue() and NotifyFixEpilogue().
// For more info, see for instance http://www.flounder.com/avoid_en_change.htm
// See also http://www.codeproject.com/KB/edit/Avoiding_EN_CHANGE.aspx?display=PrintAll
template<class TFIELDID> 
BOOL CSubstEdit<TFIELDID>::OnEnChange()
{
    BOOL bRes = FALSE;
    if (IsChangeNotifyLocked())
    {   // just increment the temporary changes counter
        ChangeModifyTempCountIncrement();
        // return true to indicate EN_CHANGE should NOT propagate
        bRes = TRUE;
    }
    return bRes;
}
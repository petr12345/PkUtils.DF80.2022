
#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// SubstEdit.h : header file
//

/////////////////////////////////////////////////////////////////////////////
//	INCLUDE FILES
/////////////////////////////////////////////////////////////////////////////
#include "SubstObjectsLogical.h"
#include "SubstObjectsPhysical.h"

/////////////////////////////////////////////////////////////////////////////
//	MANIFESTED CONSTANTS & MACROS
/////////////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////////////
// CSubstEdit window

#pragma warning(push)
#pragma warning(disable : 4091) // WarningC4091 'typedef ': ignored on left of 'CSubstEdit<tagMyFields>::eFindDirection' when no variable is declared


template<class TFIELDID> class CSubstEdit : public CEdit
{
public:
    // the argument for FindPosOutsidePhys
    typedef enum eFindDirection
    {
        eFindCloser,
        eFindBackward,
        eFindForward,
    };
    
private:
    CSubstPhysData<TFIELDID> m_data;
    WNDPROC 		m_oldWndProc;
    int 			m_nOrigCallLevel;
    int 			m_nChangeNotifyLock;
    // the temporary changes counter
    int             m_nChangeModifyTempCount;
    // If nonzero, the hook fn just delegates to original functionality.
    int				m_nLockHookLevel;

public:
    CSubstEdit();
    CSubstEdit(CSubstLogData<TFIELDID> const & logData);

public:
    CSubstPhysData<TFIELDID>& PhysData(void)
    { return m_data; }
    CSubstPhysData<TFIELDID> const& RFPhysDataC(void) const
    { return m_data; }

    // Replacement ( fixup ) of original PosFromChar method
    CPoint		MyPosFromChar(UINT nChar) const;
    // Getting the selection info
    CSelInfo& 	GetSelInfo(CSelInfo& info) const;
    // Setting the selection info
    void        SetSelInfo(CSelInfo const& info);

    // Initialize the text to physical string in m_data.GetPhysStr()
    void		InitializeText();
    // Retrieves the index of the first character of a given line.
    int         GetFirstCharIndexFromLine(int line) const;
    // Retrieves the zero-based index of the character nearest the specified point.
    int         GetCharIndexFromPosition(POINT const &pt, int *pLineIndex = NULL) const;
    // From given line and column determine the physical position ( index )
    int 		LineCol2CharPos(int line, int col) const;
    // if tPhysPos is inside any field, return the position outside
    tPhysPos 	FindPosOutsidePhys(tPhysPos iorig, eFindDirection direction = eFindCloser) const;
    // Insert new field
    BOOL		InsertNewInfo(TFIELDID	what);
    // Is the control subclassed already ?
    BOOL IsSubclassed() const
    { return (NULL != m_oldWndProc); }

protected:
    // Overrides
    // ClassWizard generated virtual function overrides
    //{{AFX_VIRTUAL(CSubstEdit)
    //}}AFX_VIRTUAL

public:
    virtual ~CSubstEdit();
    virtual void PreSubclassWindow();

protected:
    WNDPROC 	OldWndProc(void) const
    { return m_oldWndProc; }
    int 		OrigCallLevel(void) const
    { return m_nOrigCallLevel; }

    bool IsChangeNotifyLocked() const
    { return (0 < m_nChangeNotifyLock); }

    bool IsLockedOrigFn() const
    { return m_nLockHookLevel > 0; }

    void LockHookFn()
    { m_nLockHookLevel++; }

    void UnlockHookFn()
    { m_nLockHookLevel--; }

    static LRESULT CALLBACK SubstEditNewPro(HWND hwnd, UINT, WPARAM, LPARAM);
#ifdef _DEBUG
    void	 AssertSelValidity(CSelInfo const& sel) const;
#endif

    void EmptyEditCtrlUndoBuffer();
    LRESULT  CallOrigProc(UINT msg, WPARAM wParam = 0, LPARAM lParam = 0);

    BOOL	SetNewWndProc(WNDPROC NewWndProc);
    size_t 	ModifyDataOnInsertion(size_t iPos, LPCTSTR szOldText, LPCTSTR szNewText);

    void    ChangeModifyTempCountReset();
    void    ChangeModifyTempCountIncrement();
    void	NotifyFixPrologue();
    void	NotifyFixEpilogue();

    LRESULT DeleteSel_WmCharStrange(CSelInfo const& selInf, WPARAM wParam, LPARAM lParam);
    LRESULT DeleteSel_WmCharBack(CSelInfo const& selInf, LPARAM lParam);
    LRESULT DeleteSel_VKDelete(CSelInfo const& selInf, LPARAM lParam);
    LRESULT BackspaceDeleteNotSel(CSelInfo const& selInf, LPARAM lParam);
    LRESULT VkDeleteNotSel(CSelInfo const& selInf, LPARAM lParam);
    LRESULT WmCharDoInsetChar(CSelInfo const& selInf, WPARAM wParam, LPARAM lParam);
    LRESULT MoveCaretHorizontal(WPARAM wParam, LPARAM lParam);
    LRESULT MoveCaretVertical(WPARAM wParam, LPARAM lParam);
    LRESULT MyOnWmChar(WPARAM wParam, LPARAM lParam, bool &bHandled);
    LRESULT MyOnVk_Delete(LPARAM lParam);
    LRESULT MyOnWmLButtonDown(WPARAM wParam, LPARAM lParam);
    LRESULT MyOnWmMouseMove(WPARAM wParam, LPARAM lParam);
    LRESULT MyCopy(bool bCut);
    LRESULT MyOnPaste();
private:
    bool CheckStyles();

    // Generated message map functions
protected:
    afx_msg BOOL OnEnChange();
    //{{AFX_MSG(CSubstEdit)
    //}}AFX_MSG
    DECLARE_MESSAGE_MAP()
};

#pragma warning(pop)

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.


#include "SubstEdit.hpp"

#pragma once
#include "afxwin.h"
#include "SubstEdit.h"


class CTestSubstEditDoc; // forward decl.

class CTestFormView : public CFormView, public ISubstDescrProvider<tagMyFields>
{
    DECLARE_DYNCREATE(CTestFormView)

private:
    CWnd *m_pOldFoc;
    CSubstEdit<tagMyFields> m_editSample;
    CEdit m_editPreview;

protected:
    CTestFormView();           // protected constructor used by dynamic creation
    virtual ~CTestFormView();

public:
    enum { IDD = IDD_TESTFORMVIEW };
#ifdef _DEBUG
    virtual void AssertValid() const;
#ifndef _WIN32_WCE
    virtual void Dump(CDumpContext& dc) const;
#endif
#endif
    // implementation of ISubstDescrProvider
    virtual SubstDescr<tagMyFields> const* GetSubstDescr() const;
    CTestSubstEditDoc* GetSubstDocument();

protected:
    virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
    void RestoreFocus();
    void UpdatePreview();
    CString GetPreviewText(CSubstLogData<tagMyFields> const &logData) const;
    CString GetPreviewText() const;

	//{{AFX_MSG(CTestFormView)
    afx_msg void OnFileSave();
    afx_msg void OnFileSaveAs();
    afx_msg void OnKillFocusEditSingleLn();
    afx_msg void OnEnChangeEditSingleLn();
    afx_msg void OnBnClickedButtonYear();
    afx_msg void OnBnClickedButtonMonth();
    afx_msg void OnBnClickedButtonDayOfWeek();
    afx_msg void OnBnClickedButtonDog();
    afx_msg void OnEditCopy();
    afx_msg void OnUpdateEditCopy(CCmdUI *pCmdUI);
    afx_msg void OnEditCut();
    afx_msg void OnUpdateEditCut(CCmdUI *pCmdUI);
    afx_msg void OnEditPaste();
    afx_msg void OnUpdateEditPaste(CCmdUI *pCmdUI);
    afx_msg void OnEditUndo();
    afx_msg void OnUpdateEditUndo(CCmdUI *pCmdUI);
	//}}AFX_MSG
    DECLARE_MESSAGE_MAP()
};


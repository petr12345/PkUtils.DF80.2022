// TestFormView.cpp : implementation file
//

#include "stdafx.h"
#include "resource.h"
#include "TestFormView.h"
#include "TestSubstEditDoc.h"

// CTestFormView

IMPLEMENT_DYNCREATE(CTestFormView, CFormView)

BEGIN_MESSAGE_MAP(CTestFormView, CFormView)
    ON_COMMAND(ID_EDIT_COPY, &CTestFormView::OnEditCopy)
    ON_UPDATE_COMMAND_UI(ID_EDIT_COPY, &CTestFormView::OnUpdateEditCopy)
    ON_COMMAND(ID_EDIT_CUT, &CTestFormView::OnEditCut)
    ON_UPDATE_COMMAND_UI(ID_EDIT_CUT, &CTestFormView::OnUpdateEditCut)
    ON_COMMAND(ID_EDIT_PASTE, &CTestFormView::OnEditPaste)
    ON_UPDATE_COMMAND_UI(ID_EDIT_PASTE, &CTestFormView::OnUpdateEditPaste)
    ON_COMMAND(ID_EDIT_UNDO, &CTestFormView::OnEditUndo)
    ON_UPDATE_COMMAND_UI(ID_EDIT_UNDO, &CTestFormView::OnUpdateEditUndo)

    ON_EN_KILLFOCUS(IDC_EDIT_SAMPLE, OnKillFocusEditSingleLn)
    ON_EN_CHANGE(IDC_EDIT_SAMPLE, OnEnChangeEditSingleLn)

    ON_BN_CLICKED(IDC_BUTTON_YEAR, &CTestFormView::OnBnClickedButtonYear)
    ON_BN_CLICKED(IDC_BUTTON_MONTH, &CTestFormView::OnBnClickedButtonMonth)
    ON_BN_CLICKED(IDC_BUTTON_DAY_OF_WEEK, &CTestFormView::OnBnClickedButtonDayOfWeek)
    ON_BN_CLICKED(IDC_BUTTON_DOG, &CTestFormView::OnBnClickedButtonDog)

    ON_COMMAND(ID_FILE_SAVE, &CTestFormView::OnFileSave)
    ON_COMMAND(ID_FILE_SAVE_AS, &CTestFormView::OnFileSaveAs)

    ON_WM_CREATE()
END_MESSAGE_MAP()

CTestFormView::CTestFormView()
    : CFormView(CTestFormView::IDD)
{
    m_pOldFoc = NULL;
}

CTestFormView::~CTestFormView()
{
}

// implements ISubstDescrProvider
SubstDescr<tagMyFields> const* CTestFormView::GetSubstDescr() const
{
    return &CTestSubstEditDoc::m_myDesctpts[0];
}

CTestSubstEditDoc* CTestFormView::GetSubstDocument()
{
    CDocument* pDoc = CView::GetDocument();
    ASSERT(pDoc->IsKindOf(RUNTIME_CLASS(CTestSubstEditDoc)));
    return static_cast<CTestSubstEditDoc*>(pDoc);
}

void CTestFormView::DoDataExchange(CDataExchange* pDX)
{
    CTestSubstEditDoc* pDoc = this->GetSubstDocument();

    CFormView::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_EDIT_SAMPLE, m_editSample);
    if (pDX->m_bSaveAndValidate)
    {
        pDoc->Data1st() = this->m_editSample.PhysData();
    }
    else
    {
        this->m_editSample.PhysData() = pDoc->Data1st();
        /* following coukd be called instead of previous assignment operator
        this->m_editSample.PhysData().Assign(pDoc->Data1st());
        */
        this->m_editSample.InitializeText();
        this->UpdatePreview();
    }
    DDX_Control(pDX, IDC_EDIT_PREVIEW, m_editPreview);
}

// CTestFormView diagnostics

#ifdef _DEBUG
void CTestFormView::AssertValid() const
{
    CFormView::AssertValid();
}

#ifndef _WIN32_WCE
void CTestFormView::Dump(CDumpContext& dc) const
{
    CFormView::Dump(dc);
}
#endif
#endif //_DEBUG

void CTestFormView::RestoreFocus()
{
    if (m_pOldFoc)
    {
        m_pOldFoc->SetFocus();
        m_pOldFoc = NULL;
    }
}

void CTestFormView::UpdatePreview()
{
    if (NULL != m_editPreview.GetSafeHwnd())
    {
        CString strText = GetPreviewText();
        m_editPreview.SetWindowText(strText);
    }
}

static CString CALLBACK fieldPreviewVal(SubstDescr<tagMyFields> const* lpDesc)
{
    COleDateTime now = COleDateTime::GetCurrentTime();
    CString strNewPart = lpDesc->lpTxt;

    switch (lpDesc->valId)
    {
        case IdField_Year:
            strNewPart.Format(_T("%i"), now.GetYear());
            break;

        case IdField_Month:
            strNewPart = now.Format(_T("%B"));
            break;

        case IdField_DayoftheWeek:
            strNewPart = now.Format(_T("%A"));
            break;

        case IdField_Dog:
            strNewPart = _T("The Hound of the Baskervilles");
            break;
    }
    return strNewPart;
}

CString CTestFormView::GetPreviewText(CSubstLogData<tagMyFields> const &logData) const
{
    int nDex;
    CString strTmp = CSubstLogData<tagMyFields>::LogStrToPhysStr(logData, fieldPreviewVal);
    if (0 <= (nDex = strTmp.Find('\n')))
        strTmp = strTmp.Left(nDex);
    if (0 <= (nDex = strTmp.Find('\r')))
        strTmp = strTmp.Left(nDex);
    return strTmp;
}

CString CTestFormView::GetPreviewText() const
{
    return GetPreviewText(this->m_editSample.RFPhysDataC());
}

// CTestFormView message handlers

void CTestFormView::OnKillFocusEditSingleLn() 
{
    m_pOldFoc = &m_editSample;	
}

void CTestFormView::OnEnChangeEditSingleLn()
{
    this->GetDocument()->SetModifiedFlag();
    this->UpdatePreview();
}

void CTestFormView::OnFileSave()
{   // Note: Is handling ID_FILE_SAVE here to enforce proper document update.
    // Althoug modifications of CSubstEdit endure 
    if (this->UpdateData())
    {
        GetDocument()->DoFileSave();
    }
}

void CTestFormView::OnFileSaveAs()
{ // Note: Is handling ID_FILE_SAVE_AS here to enforce proper document update.
    if (this->UpdateData())
    {
        GetDocument()->DoSave(NULL);
    }
}

void CTestFormView::OnEditCopy()
{
    m_editSample.Copy();
}

void CTestFormView::OnUpdateEditCopy(CCmdUI *pCmdUI)
{
    CSelInfo sel;
    m_editSample.GetSelInfo(sel);
    pCmdUI->Enable(sel.IsSel());
}

void CTestFormView::OnEditCut()
{
    m_editSample.Cut();
}

void CTestFormView::OnUpdateEditCut(CCmdUI *pCmdUI)
{
    CSelInfo sel;
    m_editSample.GetSelInfo(sel);
    pCmdUI->Enable(sel.IsSel());
}

void CTestFormView::OnEditPaste()
{
    m_editSample.Paste();
}

void CTestFormView::OnUpdateEditPaste(CCmdUI *pCmdUI)
{
    size_t nSize;
    BOOL bEnable = FALSE;

    if (ClipWrapper::GetTextLength(&nSize))
        bEnable = (nSize > 0);
    else if (ClipWrapper::GetUnicodeTextLength(&nSize))
        bEnable = (nSize > 0);
    pCmdUI->Enable(bEnable);
}

void CTestFormView::OnUpdateEditUndo(CCmdUI *pCmdUI)
{   // undo not supported so far
    pCmdUI->Enable(FALSE);
}
void CTestFormView::OnEditUndo()
{ // undo not supported so far
}

void CTestFormView::OnBnClickedButtonYear()
{
    m_editSample.InsertNewInfo(IdField_Year);
    RestoreFocus();
}

void CTestFormView::OnBnClickedButtonMonth()
{
    m_editSample.InsertNewInfo(IdField_Month);
    RestoreFocus();
}

void CTestFormView::OnBnClickedButtonDayOfWeek()
{
    m_editSample.InsertNewInfo(IdField_DayoftheWeek);
    RestoreFocus();
}

void CTestFormView::OnBnClickedButtonDog()
{
    m_editSample.InsertNewInfo(IdField_Dog);
    RestoreFocus();
}


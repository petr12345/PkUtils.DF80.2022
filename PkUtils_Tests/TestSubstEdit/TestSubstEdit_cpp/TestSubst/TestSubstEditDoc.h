// TestSubstEditDoc.h : interface of the CTestSubstEditDoc class
//
/////////////////////////////////////////////////////////////////////////////

#include "SubstObjectsPhysical.h"

#if !defined(AFX_TESTSUBSTEDITDOC_H__ED953471_AED9_404C_8420_E852DC5C7D02__INCLUDED_)
#define AFX_TESTSUBSTEDITDOC_H__ED953471_AED9_404C_8420_E852DC5C7D02__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/////////////////////////////////////////////////////////////////////////////
// CTestSubstEditDoc

class CTestSubstEditDoc : public CDocument
{
    DECLARE_DYNCREATE(CTestSubstEditDoc)

public:

// Attributes
public:
    static SubstDescr<tagMyFields> const m_myDesctpts[];

protected:
    CSubstLogData<tagMyFields> m_data1st;
    CSubstLogData<tagMyFields> m_data2nd;

// Methods
protected: // create from serialization only
    CTestSubstEditDoc();

// Operations
public:
    CSubstLogData<tagMyFields>& Data1st()
    {	return m_data1st; }
// Overrides
    // ClassWizard generated virtual function overrides
    //{{AFX_VIRTUAL(CTestSubstEditDoc)
    public:
    virtual BOOL OnNewDocument();
    virtual void Serialize(CArchive& ar);
    //}}AFX_VIRTUAL

// Implementation
public:
    virtual ~CTestSubstEditDoc();
    virtual void DeleteContents(); // delete doc items etc
#ifdef _DEBUG
    virtual void AssertValid() const;
    virtual void Dump(CDumpContext& dc) const;
#endif

protected:
    virtual CStdioFile* GetStdioFile(LPCTSTR lpszFileName, UINT nOpenFlags, CFileException* pError);
    virtual BOOL OnOpenDocument(LPCTSTR lpszPathName);
    virtual BOOL OnSaveDocument(LPCTSTR lpszPathName);

    BOOL DoOpenTextDocument(LPCTSTR lpszPathName);
    BOOL DoSaveTextDocument(LPCTSTR lpszPathName);

    BOOL SerializeTextFile(BOOL bOpening, CStdioFile* pFile);
    CString GetPlainText() const;
    void AssignPlainText(LPCTSTR szText);
// Generated message map functions
protected:
    //{{AFX_MSG(CTestSubstEditDoc)
        // NOTE - the ClassWizard will add and remove member functions here.
        //    DO NOT EDIT what you see in these blocks of generated code !
    //}}AFX_MSG
    DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TESTSUBSTEDITDOC_H__ED953471_AED9_404C_8420_E852DC5C7D02__INCLUDED_)

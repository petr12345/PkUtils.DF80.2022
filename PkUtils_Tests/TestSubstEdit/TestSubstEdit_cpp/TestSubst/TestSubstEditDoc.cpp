// TestSubstEditDoc.cpp : implementation of the CTestSubstEditDoc class
//

#include "stdafx.h"
#include "TestSubstEdit.h"

#include "TestSubstEditDoc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CTestSubstEditDoc

SubstDescr<tagMyFields> const CTestSubstEditDoc::m_myDesctpts[] = 
{
    { IdField_Year,			_T("<Year>")   },
    { IdField_Month,		_T("<Month>")  },
    { IdField_DayoftheWeek, _T("<WeekDay>")  },
    { IdField_Dog,			_T("<Dog>")  },
    { IdField_NONE,			NULL     },
};

IMPLEMENT_DYNCREATE(CTestSubstEditDoc, CDocument)

BEGIN_MESSAGE_MAP(CTestSubstEditDoc, CDocument)
    //{{AFX_MSG_MAP(CTestSubstEditDoc)
    // NOTE - the ClassWizard will add and remove mapping macros here.
    //    DO NOT EDIT what you see in these blocks of generated code!
    //}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTestSubstEditDoc construction/destruction

CTestSubstEditDoc::CTestSubstEditDoc()
{
    m_data1st.AssignSubstMap((SubstDescr<tagMyFields> const*)m_myDesctpts);
    m_data2nd.AssignSubstMap(m_myDesctpts);
}

CTestSubstEditDoc::~CTestSubstEditDoc()
{
}

BOOL CTestSubstEditDoc::OnNewDocument()
{
    if (!CDocument::OnNewDocument())
        return FALSE;

    // TODO: add reinitialization code here
    // (SDI documents will reuse this document)

    return TRUE;
}

void CTestSubstEditDoc::DeleteContents()
{
    CDocument::DeleteContents();
    m_data1st.DeleteContents();
    m_data2nd.DeleteContents();
}

/////////////////////////////////////////////////////////////////////////////
// CTestSubstEditDoc serialization

CStdioFile* CTestSubstEditDoc::GetStdioFile(LPCTSTR lpszFileName, UINT nOpenFlags,
    CFileException* pError)
{
    CStdioFile* pFile = new CStdioFile;
    ASSERT(pFile != NULL);
    if (!pFile->Open(lpszFileName, nOpenFlags, pError))
    {
        delete pFile;
        pFile = NULL;
    }
    return pFile;
}


BOOL CTestSubstEditDoc::OnOpenDocument(LPCTSTR lpszPathName)
{
    CString strTmp(lpszPathName);
    BOOL bRes = FALSE;

    strTmp.MakeLower();
    strTmp = strTmp.Right(4);
    if (strTmp == _T(".txt"))
    {
        bRes = DoOpenTextDocument(lpszPathName);
    }
    else
    {
        bRes = CDocument::OnOpenDocument(lpszPathName);
    }
    return bRes;
}

BOOL CTestSubstEditDoc::OnSaveDocument(LPCTSTR lpszPathName)
{
    CString strTmp(lpszPathName);
    BOOL bRes = FALSE;

    strTmp.MakeLower();
    strTmp = strTmp.Right(4);
    if (strTmp == _T(".txt"))
    {
        bRes = DoSaveTextDocument(lpszPathName);
    }
    else
    {
        bRes = CDocument::OnSaveDocument(lpszPathName);
    }
    return bRes;
}

BOOL CTestSubstEditDoc::DoOpenTextDocument(LPCTSTR lpszPathName)
{
    CFileException fe;
    CStdioFile* pFile = GetStdioFile(lpszPathName, CFile::modeRead|CFile::shareDenyWrite, &fe);
    if (pFile == NULL)
    {
        ReportSaveLoadException(lpszPathName, &fe, FALSE, AFX_IDP_FAILED_TO_OPEN_DOC);
        return FALSE;
    }

    DeleteContents();
    SetModifiedFlag();  // dirty during de-serialize

    try
    {
        CWaitCursor wait;
        if (pFile->GetLength() != 0)
        {
            SerializeTextFile(TRUE, pFile);  // load me
        }
        ReleaseFile(pFile, FALSE);
    }
    catch (CException* e)
    {
        ReleaseFile(pFile, TRUE);
        DeleteContents();   // remove failed contents
        ReportSaveLoadException(lpszPathName, e, FALSE, AFX_IDP_FAILED_TO_OPEN_DOC);
        e->Delete();
        return FALSE;
    }

    SetModifiedFlag(FALSE);     // start off with unmodified

    return TRUE;
}

BOOL CTestSubstEditDoc::DoSaveTextDocument(LPCTSTR lpszPathName)
{
    CFileException fe;
    CStdioFile* pFile = GetStdioFile(lpszPathName, CFile::modeCreate |
        CFile::modeReadWrite | CFile::shareExclusive, &fe);

    if (pFile == NULL)
    {
        ReportSaveLoadException(lpszPathName, &fe, TRUE, AFX_IDP_INVALID_FILENAME);
        return FALSE;
    }

    try
    {
        CWaitCursor wait;
        SerializeTextFile(FALSE, pFile);  // save me
        ReleaseFile(pFile, FALSE);
    }
    catch (CException* e)
    {
        ReleaseFile(pFile, TRUE);
        DeleteContents();   // remove failed contents
        ReportSaveLoadException(lpszPathName, e, true, AFX_IDP_FAILED_TO_SAVE_DOC);
        e->Delete();
        return FALSE;
    }

    SetModifiedFlag(FALSE);     // back to unmodified

    return TRUE;        // success
}

void CTestSubstEditDoc::Serialize(CArchive& ar)
{
    if (ar.IsStoring())
    {
        m_data1st.Serialize(ar);
        /* m_data2nd.Serialize(ar); */
    }
    else
    {
        m_data1st.Serialize(ar);
        /* m_data2nd.Serialize(ar); */
    }
}

CString CTestSubstEditDoc::GetPlainText() const
{
    return m_data1st.GetPlainText();
}

void CTestSubstEditDoc::AssignPlainText(LPCTSTR szText)
{
    m_data1st.AssignPlainText(szText);
}

BOOL CTestSubstEditDoc::SerializeTextFile(BOOL bOpening, CStdioFile* pFile)
{
    CString strContents, strLine;
    BOOL bRes = TRUE;

    if (bOpening)
    {
        while (pFile->ReadString(strLine))
        {
            strContents += strLine;
        }
        AssignPlainText(strContents);
    }
    else
    {
        strContents = GetPlainText();
        pFile->WriteString(strContents);
    }
    return bRes; 
}

/////////////////////////////////////////////////////////////////////////////
// CTestSubstEditDoc diagnostics

#ifdef _DEBUG
void CTestSubstEditDoc::AssertValid() const
{
    CDocument::AssertValid();
}

void CTestSubstEditDoc::Dump(CDumpContext& dc) const
{
    CDocument::Dump(dc);
}
#endif //_DEBUG

/////////////////////////////////////////////////////////////////////////////
// CTestSubstEditDoc commands

// TestSubstEdit.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "TestSubstEdit.h"

#include "MainFrm.h"
#include "TestSubstEditDoc.h"
#include "TestFormView.h"
#include "DocManagerEx.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CTestSubstEditApp

BEGIN_MESSAGE_MAP(CTestSubstEditApp, CWinApp)
    //{{AFX_MSG_MAP(CTestSubstEditApp)
    // NOTE - the ClassWizard will add and remove mapping macros here.
    //    DO NOT EDIT what you see in these blocks of generated code!
    //}}AFX_MSG_MAP
    // Standard file based document commands
    ON_COMMAND(ID_FILE_NEW, CWinApp::OnFileNew)
    ON_COMMAND(ID_FILE_OPEN, CWinApp::OnFileOpen)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CTestSubstEditApp construction

CTestSubstEditApp::CTestSubstEditApp()
{
    // TODO: add construction code here,
    // Place all significant initialization in InitInstance
}

/////////////////////////////////////////////////////////////////////////////
// The one and only CTestSubstEditApp object

CTestSubstEditApp theApp;

/////////////////////////////////////////////////////////////////////////////
// CTestSubstEditApp initialization

BOOL CTestSubstEditApp::InitInstance()
{
    AfxEnableControlContainer();

    // Change the registry key under which our settings are stored.
    // You should modify this string to be something appropriate
    // such as the name of your company or organization.
    SetRegistryKey(_T("Test Applications"));

    LoadStdProfileSettings();  // Load standard INI file options (including MRU)

    // Register the application's document templates.  Document templates
    //  serve as the connection between documents, frame windows and views.
    CSingleDocTemplate* pDocTemplate;
    pDocTemplate = new CSingleDocTemplate(
        IDR_TESTDOC,
        RUNTIME_CLASS(CTestSubstEditDoc),
        RUNTIME_CLASS(CMainFrame),       // main SDI frame window
        RUNTIME_CLASS(CTestFormView));
    /* AddDocTemplate(pDocTemplate); */

    ASSERT(m_pDocManager == NULL);
    m_pDocManager = new CDocManagerEx();
    m_pDocManager->AddDocTemplate(pDocTemplate); // 

    // Parse command line for standard shell commands, DDE, file open
    CCommandLineInfo cmdInfo;
    ParseCommandLine(cmdInfo);

    // Dispatch commands specified on the command line
    if (!ProcessShellCommand(cmdInfo))
        return FALSE;

    // The one and only window has been initialized, so show and update it.
    m_pMainWnd->ShowWindow(SW_SHOW);
    m_pMainWnd->UpdateWindow();

    return TRUE;
}

/////////////////////////////////////////////////////////////////////////////
// CTestSubstEditApp message handlers


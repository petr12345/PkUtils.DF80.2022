// TestSubstEdit.h : main header file for the TESTSUBSTEDIT application
//

#if !defined(AFX_TESTSUBSTEDIT_H__A75AF2FF_6AE1_4070_B701_8A97A5F3B1BC__INCLUDED_)
#define AFX_TESTSUBSTEDIT_H__A75AF2FF_6AE1_4070_B701_8A97A5F3B1BC__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
    #error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"       // main symbols

/////////////////////////////////////////////////////////////////////////////
// CTestSubstEditApp:
// See TestSubstEdit.cpp for the implementation of this class
//

class CTestSubstEditApp : public CWinApp
{
public:
    CTestSubstEditApp();

// Overrides
    // ClassWizard generated virtual function overrides
    //{{AFX_VIRTUAL(CTestSubstEditApp)
    public:
    virtual BOOL InitInstance();
    //}}AFX_VIRTUAL

// Implementation
    //{{AFX_MSG(CTestSubstEditApp)
        // NOTE - the ClassWizard will add and remove member functions here.
        //    DO NOT EDIT what you see in these blocks of generated code !
    //}}AFX_MSG
    DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_TESTSUBSTEDIT_H__A75AF2FF_6AE1_4070_B701_8A97A5F3B1BC__INCLUDED_)

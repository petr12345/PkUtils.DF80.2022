#ifndef __DOCMANAGER_EXT_H__
#define __DOCMANAGER_EXT_H__

class CDocManagerEx : public CDocManager
{
    DECLARE_DYNAMIC(CDocManagerEx)

// Construction
public:
    CDocManagerEx();

// Attributes
public:

// Operations
public:

// Overrides
    // helper for standard commdlg dialogs
    virtual BOOL DoPromptFileName(CString& fileName, UINT nIDSTitle,
            DWORD lFlags, BOOL bOpenFileDialog, CDocTemplate* pTemplate);

// Implementation
public:
    virtual ~CDocManagerEx();
};


#endif // __DOCMANAGER_EXT_H__
// ClipWrapper.cpp : class ClipWrapper implementation


// ClipWrapper class
// ------------------
// Based on class CClipboard written by David Terracino <davet@lycosemail.com>, 
// published http://www.codeguru.com/cpp/w-p/clipboard/article.php/c3013/
// and modified by Petr Kodet.
// See header file for complete revision history.

////////////////////////////////////////////////////////////////
//  INCLUDE FILES
////////////////////////////////////////////////////////////////
#include "stdafx.h"
#include <comdef.h>			 // basic COM defs for _com_error
#include "ClipWrapper.h"

// #include "UtilFunctions.h"

////////////////////////////////////////////////////////////////
// MANIFESTED CONSTANTS & MACROS
////////////////////////////////////////////////////////////////
#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#pragma warning ( disable : 4706) // get rid of C4706: assignment within conditional expression

////////////////////////////////////////////////////////////////
// STATIC FUNCTIONS
////////////////////////////////////////////////////////////////

static CString WINAPI getErrorMessage(DWORD dwLastError)
{
    CString strRes = _com_error(dwLastError).ErrorMessage();
    return strRes;
}

static CString WINAPI getErrorMessage2(DWORD dwLastError)
{
    CString strTmp = getErrorMessage(dwLastError);
    CString strRes;

    strRes.Format(_T("GetLastError = %i, %s"), (LPCTSTR)strTmp);
    return strRes;
}

////////////////////////////////////////////////////////////////
// CLASS DEFINITIONS
////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////
// ClipWrapper::SetText
// - Places text on the clipboard as CF_TEXT or CF_UNICODETEXT or both
// This method opens and closes the clipboard.
////////////////////////////////////////////////////////////////////
//
// Parameters:
//  lpszBuffer - pointer to an imput string
//	bCF_TEXT   - put it as CF_TEXT
//	bCF_UNICODETEXT - put it as CF_UNICODETEXT
//	hWnd       - window handle to be used by clipboard
//
// Return Values:
//	TRUE       - Text was successfully copied to clipboard
//	FALSE      - Failed
//
////////////////////////////////////////////////////////////////////

BOOL ClipWrapper::SetText(
    LPCTSTR lpszBuffer, 
    BOOL bCF_TEXT /* = TRUE */, 
    BOOL bCF_UNICODETEXT /* = TRUE */,
    HWND hWnd /* = NULL */)
{
    BOOL bRes;

    // First, open the clipboard. OpenClipboard() takes one parameter,
    // the handle of the window that will temporarily be it's owner. 
    // If NULL is passed, we will get the handle of main app. window
    // ( otherwise, with (hWnd == NULL), functions like SetClipboardData sometimes fail, 
    //   with ::GetLastError() == ERROR_CLIPBOARD_NOT_OPEN         1418L )
    if (hWnd != NULL) {
        ASSERT(::IsWindow(hWnd));
    }
    else {
        hWnd = AfxGetMainWnd()->GetSafeHwnd();
    }
    if (bRes = ::OpenClipboard(hWnd))
    {
        EmptyClipboard();
        bRes = SetTextNoOpenClose(lpszBuffer, bCF_TEXT, bCF_UNICODETEXT);
        CloseClipboard();
    }

    return bRes;
}

////////////////////////////////////////////////////////////////////
// ClipWrapper::SetTextNoOpenClose
//   - Places text on the clipboard as CF_TEXT or CF_UNICODETEXT or both.
// This method does not open or close the clipboard, assuming the caller wil take care of it.
// You will use this method if the caller wants to put more additional data ( in more formats ) on clipboard.
////////////////////////////////////////////////////////////////////
//
// Parameters:
//	lpszBuffer - pointer to an imput string
//	bCF_TEXT   - put it as CF_TEXT
//	bCF_UNICODETEXT - put it as CF_UNICODETEXT
//
// Return Values:
//	TRUE       - Text was successfully copied to clipboard
//	FALSE      - Failed
//
////////////////////////////////////////////////////////////////////

BOOL ClipWrapper::SetTextNoOpenClose(
    LPCTSTR lpszBuffer, 
    BOOL bCF_TEXT /* = TRUE */, 
    BOOL bCF_UNICODETEXT /* = TRUE */)
{
    unsigned long nChars;	// amount of characters in lpszBuffer
    BOOL bRes = TRUE;

    // Get the size of the string in the buffer that was passed into the function, 
    // so we know how much global memory to allocate for the string.
    if (0 < (nChars = lstrlen(lpszBuffer)))
    {
        CByteArray arrbuff;
        LPCSTR lptext = NULL;
        unsigned long nTextCharBuff = 0; // needed size of buffer, in characters, including null terminator
        LPCWSTR lpwtext = NULL;
        unsigned long nWCharBuff = 0;  // needed size of buffer, in wchars, including null terminator
        BOOL bOk1 = TRUE;
        BOOL bOk2 = TRUE;
        DWORD dwErr = NO_ERROR;

        // first prepare conversions of input data if needed:
#ifndef _UNICODE
        if (bCF_TEXT)
        {
            nTextCharBuff = nChars + 1;
            lptext = lpszBuffer;
        }
        if (bCF_UNICODETEXT)
        {
            nWCharBuff = MultiByteToWideChar(CP_ACP, 0, lpszBuffer, -1, NULL, 0);
            arrbuff.SetSize(sizeof(WCHAR)*nWCharBuff);
            lpwtext = (LPWSTR)arrbuff.GetData();
            VERIFY(MultiByteToWideChar(CP_ACP, 0, lpszBuffer, -1, const_cast<LPWSTR>(lpwtext), nWCharBuff));
        }
#else
        if (bCF_TEXT)
        {
            nTextCharBuff = WideCharToMultiByte(CP_ACP, 0, lpszBuffer, -1, NULL, 0, NULL, NULL);
            arrbuff.SetSize(sizeof(char)*nTextCharBuff);
            lptext = (LPSTR)arrbuff.GetData();
            WideCharToMultiByte(CP_ACP, 0, lpszBuffer, -1, const_cast<LPSTR>(lptext), nTextCharBuff, NULL, NULL);
        }
        if (bCF_UNICODETEXT)
        {
            nWCharBuff = nChars + 1;
            lpwtext = lpszBuffer;
        }
#endif
        // now make actual clipboard copy
        if (bCF_TEXT)
        {
            LPSTR lpszCF_TEXT;		// Pointer to clipboard data
            HGLOBAL hCF_TEXT = GlobalAlloc(GMEM_ZEROINIT | GMEM_MOVEABLE, nTextCharBuff * sizeof(char));

            bOk1 = FALSE;
            if (hCF_TEXT != NULL)
            {
                if (lpszCF_TEXT = (LPSTR)GlobalLock(hCF_TEXT))
                {
                    // Now copy the text from the buffer into the allocated global memory pointer.
                    memcpy(lpszCF_TEXT, lptext, nTextCharBuff * sizeof(char)); 
                    GlobalUnlock(hCF_TEXT);
                    bOk1 = (NULL != ::SetClipboardData(CF_TEXT, hCF_TEXT));
#ifdef _DEBUG
                    if (!bOk1) {
                        dwErr = ::GetLastError();
                        TRACE1("\n%s", (LPCTSTR)getErrorMessage2(dwErr));
                    }
#endif
                }
                else
                {
                    GlobalFree(hCF_TEXT);
                }
            }
        }

        if (bCF_UNICODETEXT)
        {
            LPWSTR lpszCF_UNICODETEXT;		// Pointer to clipboard data
            HGLOBAL hCF_UNICODETEXT = GlobalAlloc(GMEM_ZEROINIT | GMEM_MOVEABLE, nWCharBuff * sizeof(WCHAR));

            bOk2 = FALSE;
            if (hCF_UNICODETEXT != NULL)
            {
                if (lpszCF_UNICODETEXT = (LPWSTR)GlobalLock(hCF_UNICODETEXT))
                {
                    // Now copy the text from the buffer into the allocated global memory pointer.
                    memcpy(lpszCF_UNICODETEXT, lpwtext, nWCharBuff * sizeof(WCHAR)); 
                    GlobalUnlock(hCF_UNICODETEXT);
                    bOk2 = (NULL != SetClipboardData(CF_UNICODETEXT, hCF_UNICODETEXT));
#ifdef _DEBUG
                    if (!bOk2) {
                        dwErr = ::GetLastError();
                        TRACE1("\n%s", (LPCTSTR)getErrorMessage2(dwErr));
                    }
#endif
                }
                else
                {
                    GlobalFree(hCF_UNICODETEXT);
                }
            }
        }

        bRes = (bOk1 && bOk2);
    }

    return bRes;
}


////////////////////////////////////////////////////////////////////
// ClipWrapper::SetLocaleText
// - Places text as CF_TEXT and specified LCID in CF_LOCALE format on clipboard.
// This method opens and closes the clipboard.
////////////////////////////////////////////////////////////////////
//
// Parameters:
//  lpszBuffer - pointer to an imput string
//  textLocale - locale
//	hWnd       - window handle to be used by clipboard
//
// Return Values:
//	TRUE       - Text was successfully copied to clipboard
//	FALSE      - Failed
//
////////////////////////////////////////////////////////////////////
BOOL ClipWrapper::SetLocaleText(
    LPCSTR lpText, 
    LCID textLocale /* = MAKELANGID(LANG_NEUTRAL, SUBLANG_NEUTRAL) */, 
    HWND hWnd /* = NULL*/)
{
    BOOL bRes;

    // First, open the clipboard. OpenClipboard() takes one parameter,
    // the handle of the window that will temporarily be it's owner. 
    // If NULL is passed, we will get the handle of main app. window
    // ( otherwise, with (hWnd == NULL), functions like SetClipboardData sometimes fail, 
    //   with ::GetLastError() == ERROR_CLIPBOARD_NOT_OPEN         1418L )
    if (hWnd != NULL) {
        ASSERT(::IsWindow(hWnd));
    }
    else {
        hWnd = AfxGetMainWnd()->GetSafeHwnd();
    }
    if (bRes = ::OpenClipboard(hWnd))
    {
        EmptyClipboard();
        bRes = SetLocaleTextNoOpenClose(lpText, textLocale );
        CloseClipboard();
    }

    return bRes;
}

BOOL ClipWrapper::SetLocaleTextNoOpenClose(
    LPCSTR lpText, 
    LCID textLocale /* = MAKELANGID(LANG_NEUTRAL, SUBLANG_NEUTRAL) */)
{
    size_t nChars;	// amount of characters in lpszBuffer
    BOOL bRes = TRUE;

    // Get the size of the string in the buffer that was passed into the function, 
    // so we know how much global memory to allocate for the string.
    if (0 < (nChars = _mbstrlen(lpText)))
    {
        size_t nTextCharBuff = nChars + 1; // needed size of buffer, in characters, including null terminator
        BOOL bOk1 = TRUE;
        BOOL bOk2 = TRUE;
        DWORD dwErr = NO_ERROR;

        if (textLocale != MAKELANGID(LANG_NEUTRAL, SUBLANG_NEUTRAL))
        {
            HGLOBAL hglbLocale = GlobalAlloc(GMEM_MOVEABLE, sizeof(textLocale));
            bOk1 = FALSE;
            if (NULL != hglbLocale) 
            {
                PLCID lpLocale = (PLCID)GlobalLock(hglbLocale);
                if (lpLocale) 
                {
                    memcpy(lpLocale, &textLocale, sizeof(textLocale));
                    GlobalUnlock(hglbLocale);
                    bOk1 = (NULL != ::SetClipboardData(CF_LOCALE, hglbLocale));
#ifdef _DEBUG
                    if (!bOk1) {
                        dwErr = ::GetLastError();
                        TRACE1("\n%s", (LPCTSTR)getErrorMessage2(dwErr));
                    }
#endif
                }
                else
                {
                    GlobalFree(hglbLocale);
                }
            }
        }

        if (bOk1)
        {
            LPSTR lpszCF_TEXT;		// Pointer to clipboard data
            HGLOBAL hCF_TEXT = GlobalAlloc(GMEM_ZEROINIT | GMEM_MOVEABLE, nTextCharBuff * sizeof(char));

            bOk2 = FALSE;
            if (hCF_TEXT != NULL)
            {
                if (lpszCF_TEXT = (LPSTR)GlobalLock(hCF_TEXT))
                {
                    // Now copy the text from the buffer into the allocated global memory pointer.
                    memcpy(lpszCF_TEXT, lpText, nTextCharBuff * sizeof(char)); 
                    GlobalUnlock(hCF_TEXT);
                    bOk2 = (NULL != ::SetClipboardData(CF_TEXT, hCF_TEXT));
#ifdef _DEBUG
                    if (!bOk2) {
                        dwErr = ::GetLastError();
                        TRACE1("\n%s", (LPCTSTR)getErrorMessage2(dwErr));
                    }
#endif
                }
                else
                {
                    GlobalFree(hCF_TEXT);
                }
            }
        }


        bRes = (bOk1 && bOk2);
    }

    return bRes;
}


////////////////////////////////////////////////////////////////////
// ClipWrapper::GetText
// - retrieves text data CF_TEXT from the clipboard
////////////////////////////////////////////////////////////////////
//
// Parameters:
//	strOut     - output string
//	hWnd       - window handle to be used by clipboard
//
// Return Values:
//	TRUE       - Text was successfully copied from clipboard
//	FALSE      - No text CF_TEXT on the clipboard
//
////////////////////////////////////////////////////////////////////

BOOL ClipWrapper::GetText(
    CString &strOut, 
    HWND hWnd /* = NULL*/)
{
    BOOL bRes = FALSE;

    strOut.Empty();

    // First, open the clipboard. OpenClipboard() takes one parameter,
    // the handle of the window that will temporarily be it's owner. 
    // If NULL is passed, we will get the handle of main app. window
    // ( otherwise, with (hWnd == NULL), functions like SetClipboardData sometimes fail, 
    //   with ::GetLastError() == ERROR_CLIPBOARD_NOT_OPEN         1418L )
    if (hWnd != NULL) {
        ASSERT(::IsWindow(hWnd));
    }
    else {
        hWnd = AfxGetMainWnd()->GetSafeHwnd();
    }
    if ( (::IsClipboardFormatAvailable(CF_TEXT)) && ::OpenClipboard(hWnd) )
    {
        HGLOBAL hGlobal;		// Global memory handle
        LPCSTR lpszData;		// Pointer to clipboard data

        // Request handle to the text on the clipboard.
        if (NULL != (hGlobal = GetClipboardData(CF_TEXT)))
        {
            lpszData = (LPCSTR)GlobalLock(hGlobal);
            strOut = lpszData;

            // Now, simply unlock the global memory pointer and close the clipboard.
            // The handle returned by GetClipboardData is still owned by the clipboard, so an application must not free it or leave it locked.
            GlobalUnlock(hGlobal);
            bRes = TRUE;
        }
        CloseClipboard();
    }

    return bRes;
}

////////////////////////////////////////////////////////////////////
// ClipWrapper::GetTextLength
// - Retrieves length of text CF_TEXT on the clipboard
////////////////////////////////////////////////////////////////////
//
// Parameters:
//	pnSize     - pointer to unsigned long that will receive
//               the length of the text on the clipboard in characters.
//               NOTE: Does not include NULL terminator.
//	hWnd       - window handle to be used by clipboard
//
// Return Values:
//	TRUE       - Text length was successfully returned.
//	FALSE      - No text CF_TEXT on the clipboard
//
////////////////////////////////////////////////////////////////////

BOOL ClipWrapper::GetTextLength (size_t *pnSize, HWND hWnd)
{
    size_t lSize = 0;
    BOOL bRes = FALSE;

    // First, open the clipboard. OpenClipboard() takes one parameter,
    // the handle of the window that will temporarily be it's owner. 
    // If NULL is passed, we will get the handle of main app. window
    // ( otherwise, with (hWnd == NULL), functions like SetClipboardData sometimes fail, 
    //   with ::GetLastError() == ERROR_CLIPBOARD_NOT_OPEN         1418L )
    if (hWnd != NULL) {
        ASSERT(::IsWindow(hWnd));
    }
    else {
        hWnd = AfxGetMainWnd()->GetSafeHwnd();
    }
    if ( (::IsClipboardFormatAvailable(CF_TEXT)) && ::OpenClipboard(hWnd) )
    {
        HGLOBAL hGlobal;		// Global memory handle
        LPCSTR lpszData;		// Pointer to clipboard data

        // Request handle to the text on the clipboard.
        if (NULL != (hGlobal = GetClipboardData(CF_TEXT)))
        {
            lpszData = (LPCSTR)GlobalLock(hGlobal);
            lSize = _mbstrlen (lpszData);
            GlobalUnlock(hGlobal);
            bRes = TRUE;
        }
        CloseClipboard();
    }
    if (pnSize)
    {
        *pnSize = lSize;
    }

    return bRes;
}

////////////////////////////////////////////////////////////////////
// ClipWrapper::GetUnicodeText
// - retrieves text data CF_UNICODETEXT from the clipboard
////////////////////////////////////////////////////////////////////
//
// Parameters:
//	strOut     - output string
//	hWnd       - window handle to be used by clipboard
//
// Return Values:
//	TRUE       - Text was successfully copied from clipboard
//	FALSE      - No text CF_UNICODETEXT on the clipboard
//
////////////////////////////////////////////////////////////////////

BOOL ClipWrapper::GetUnicodeText(
    CComBSTR &strOut, 
    HWND hWnd /* = NULL*/)
{
    BOOL bRes = FALSE;

    strOut.Empty();

    // First, open the clipboard. OpenClipboard() takes one parameter,
    // the handle of the window that will temporarily be it's owner. 
    // If NULL is passed, we will get the handle of main app. window
    // ( otherwise, with (hWnd == NULL), functions like SetClipboardData sometimes fail, 
    //   with ::GetLastError() == ERROR_CLIPBOARD_NOT_OPEN         1418L )
    if (hWnd != NULL) {
        ASSERT(::IsWindow(hWnd));
    }
    else {
        hWnd = AfxGetMainWnd()->GetSafeHwnd();
    }
    if ( (::IsClipboardFormatAvailable(CF_UNICODETEXT)) && ::OpenClipboard(hWnd) )
    {
        HGLOBAL hGlobal;		// Global memory handle
        LPCWSTR lpszData;		// Pointer to clipboard data

        // Request handle to the text on the clipboard.
        if (NULL != (hGlobal = GetClipboardData(CF_UNICODETEXT)))
        {
            lpszData = (LPCWSTR)GlobalLock(hGlobal);
            strOut = lpszData;

            // Now, simply unlock the global memory pointer and close the clipboard.
            // The handle returned by GetClipboardData is still owned by the clipboard, so an application must not free it or leave it locked.
            GlobalUnlock(hGlobal);
            bRes = TRUE;
        }
        CloseClipboard();
    }

    return bRes;
}

////////////////////////////////////////////////////////////////////
// ClipWrapper::GetTextLength
// - Retrieves length of text CF_UNICODETEXT on the clipboard
////////////////////////////////////////////////////////////////////
//
// Parameters:
//	pnSize     - pointer to unsigned long that will receive
//               the length of the text on the clipboard in characters.
//               NOTE: Does not include NULL terminator.
//	hWnd       - window handle to be used by clipboard
//
// Return Values:
//	TRUE       - Text length was successfully returned.
//	FALSE      - No text CF_UNICODETEXT on the clipboard
//
////////////////////////////////////////////////////////////////////

BOOL ClipWrapper::GetUnicodeTextLength (size_t *pnSize, HWND hWnd)
{
    size_t lSize = 0;
    BOOL bRes = FALSE;

    // First, open the clipboard. OpenClipboard() takes one parameter,
    // the handle of the window that will temporarily be it's owner. 
    // If NULL is passed, we will get the handle of main app. window
    // ( otherwise, with (hWnd == NULL), functions like SetClipboardData sometimes fail, 
    //   with ::GetLastError() == ERROR_CLIPBOARD_NOT_OPEN         1418L )
    if (hWnd != NULL) {
        ASSERT(::IsWindow(hWnd));
    }
    else {
        hWnd = AfxGetMainWnd()->GetSafeHwnd();
    }
    if ( (::IsClipboardFormatAvailable(CF_UNICODETEXT)) && ::OpenClipboard(hWnd) )
    {
        HGLOBAL hGlobal;		// Global memory handle
        LPCWSTR lpszData;		// Pointer to clipboard data

        // Request handle to the text on the clipboard.
        if (NULL != (hGlobal = GetClipboardData(CF_UNICODETEXT)))
        {
            lpszData = (LPCWSTR)GlobalLock(hGlobal);
            lSize = wcslen(lpszData);
            GlobalUnlock(hGlobal);
            bRes = TRUE;
        }
        CloseClipboard();
    }
    if (pnSize)
    {
        *pnSize = lSize;
    }

    return bRes;
}


////////////////////////////////////////////////////////////////////
// ClipWrapper::SetEnhMetaFile
// - Places CF_ENHMETAFILE on the clipboard.
// This method opens and closes the clipboard.
////////////////////////////////////////////////////////////////////
//
// Parameters:
//  lpszFileName - a string containging filename of enhanced metafile 
//	hWnd       - window handle to be used by clipboard
//
// Return Values:
//	TRUE       - Data successfully copied to clipboard
//	FALSE      - Failed
//
////////////////////////////////////////////////////////////////////

BOOL ClipWrapper::SetEnhMetaFile(
    LPCTSTR lpszFileName, 
    HWND hWnd /* = NULL */)
{
    BOOL bRes;

    // First, open the clipboard. OpenClipboard() takes one parameter,
    // the handle of the window that will temporarily be it's owner. 
    // If NULL is passed, we will get the handle of main app. window
    // ( otherwise, with (hWnd == NULL), functions like SetClipboardData sometimes fail, 
    //   with ::GetLastError() == ERROR_CLIPBOARD_NOT_OPEN         1418L )
    if (hWnd != NULL) {
        ASSERT(::IsWindow(hWnd));
    }
    else {
        hWnd = AfxGetMainWnd()->GetSafeHwnd();
    }
    if (bRes = ::OpenClipboard(hWnd))
    {
        EmptyClipboard();
        bRes = SetEnhMetaFileNoOpenClose(lpszFileName);
        CloseClipboard();
    }

    return bRes;
}

BOOL ClipWrapper::SetEnhMetaFileNoOpenClose(
    LPCTSTR lpszFileName)
{
    HENHMETAFILE  hFileData, hData;
    BOOL bRes = FALSE;

    if (NULL != (hFileData = GetEnhMetaFile(lpszFileName)))
    {
        hData = CopyEnhMetaFile(  hFileData,  NULL);
        // then the hFileData can be released
        DeleteEnhMetaFile(hFileData);

        if (NULL != hData)
        {
            bRes = (NULL != ::SetClipboardData(CF_ENHMETAFILE, hData));
#ifdef _DEBUG
            if (!bRes) 
            {
                DWORD dwErr = ::GetLastError();
                TRACE1("\n%s", (LPCTSTR)getErrorMessage2(dwErr));
            }
#endif
        }
    }
    return bRes;
}

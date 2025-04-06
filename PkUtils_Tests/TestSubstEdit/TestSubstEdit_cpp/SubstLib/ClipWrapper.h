// ClipWrapper.h : class ClipWrapper declaration

/////////////////////////////////////////////////////////////////////////////
// ClipWrapper class
// 
// Based on class CClipboard written by David Terracino <davet@lycosemail.com>, 
// published at http://www.codeguru.com/cpp/w-p/clipboard/article.php/c3013/
//
// REVISIONS: September 10/05/2006 by Petr Kodet:
//   i/ CClipboard renamed to ClipWrapper
//   ii/ code modification to support Unicode arguments
//   iii/ adding methods to support CF_UNICODETEXT text format


/////////////////////////////////////////////////////////////////////////////
//  INCLUDE FILES
/////////////////////////////////////////////////////////////////////////////
#include "StdAfx.h"
#include <atlbase.h>   // for CComBSTR 

/////////////////////////////////////////////////////////////////////////////
// MANIFESTED CONSTANTS & MACROS
/////////////////////////////////////////////////////////////////////////////

#ifndef __CCLIPBOARD_WRAPPER_H__
#define __CCLIPBOARD_WRAPPER_H__

/////////////////////////////////////////////////////////////////////////////
// CLASS DEFINITIONS
/////////////////////////////////////////////////////////////////////////////

class PKMFCEXT_CLASS ClipWrapper
{
public:
    // Set to clipboard. Function working with CF_TEXT or CF_UNICODETEXT or both.
    static BOOL SetText(LPCTSTR lpszBuffer, BOOL bCF_TEXT = TRUE, BOOL bCF_UNICODETEXT = TRUE, HWND hWND = NULL);
    // Set to clipboard, don't open/close. Function working with CF_TEXT or CF_UNICODETEXT or both.
    static BOOL SetTextNoOpenClose(LPCTSTR lpszBuffer, BOOL bCF_TEXT = TRUE, BOOL bCF_UNICODETEXT = TRUE);

    // Set to clipboard. Function setting CF_TEXT and specified LCID in CF_LOCALE format.
    // ( Note: When you close the clipboard, if it contains CF_TEXT data but no CF_LOCALE data, 
    //   the system automatically sets the CF_LOCALE format to the current input locale. 
    //   You can use the CF_LOCALE format to associate a different locale with the clipboard text. )
    static BOOL SetLocaleText(LPCSTR lpText, LCID textLocale = MAKELANGID(LANG_NEUTRAL, SUBLANG_NEUTRAL), HWND hWnd = NULL);
    // Set to clipboard, don't open/close. Function setting CF_TEXT and specified LCID in CF_LOCALE format.
    static BOOL SetLocaleTextNoOpenClose(LPCSTR lpText, LCID textLocale = MAKELANGID(LANG_NEUTRAL, SUBLANG_NEUTRAL));

    // Function working with CF_TEXT - retrieving from clipboard
    static BOOL GetText(CString &strOut, HWND hWnd = NULL);
    static BOOL GetTextLength (size_t *pnSize, HWND hWnd = NULL);

    // Function working with CF_UNICODETEXT - retrieving from clipboard
    static BOOL GetUnicodeText(CComBSTR &strOut, HWND hWnd = NULL);
    static BOOL GetUnicodeTextLength (size_t *pnSize, HWND hWnd = NULL);

    // Set to clipboard the metafile ( CF_ENHMETAFILE ).
    static BOOL SetEnhMetaFile(LPCTSTR lpszFileName, HWND hWnd = NULL);
    // Set to clipboard the metafile ( CF_ENHMETAFILE ), don't open/close.
    static BOOL SetEnhMetaFileNoOpenClose(LPCTSTR lpszFileName);
};

#endif // __CCLIPBOARD_WRAPPER_H__
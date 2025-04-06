// PkMfcExt_Export.h : 
//  
//   definitin of export/import macros for PkMfcExt.dll
//

#if !defined(__PKMFCEXT_EXPORT_H__)
#define __PKMFCEXT_EXPORT_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000


#define PKMFCEXT_CLASS_EXPORT    __declspec(dllexport)
#define PKMFCEXT_API_EXPORT      __declspec(dllexport)
#define PKMFCEXT_DATA_EXPORT     __declspec(dllexport)

#define PKMFCEXT_CLASS_IMPORT    __declspec(dllimport)
#define PKMFCEXT_API_IMPORT      __declspec(dllimport)
#define PKMFCEXT_DATA_IMPORT     __declspec(dllimport)

#define DECLARE_SERIAL_EXPORT(class_name) \
    _DECLARE_DYNCREATE(class_name) \
    PKMFCEXT_API_EXPORT friend CArchive& AFXAPI operator>>(CArchive& ar, class_name* &pOb);

#define DECLARE_SERIAL_IMPORT(class_name) \
    _DECLARE_DYNCREATE(class_name) \
    PKMFCEXT_API_IMPORT friend CArchive& AFXAPI operator>>(CArchive& ar, class_name* &pOb);


/////////////////////////////////////////////////////////////////////////////
// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler.
//
// All files within this DLL and all files using the DLL needs to be compiled with symbol
// PKMFCEXT defined. The purpose is to indicate that we are using dynamic link library
// ( The PKMFCEXT symbol is not defined in case 
//   - the .cpp files are used for compiling just the static version of the library;
//   - or the project using the library and including the headers
//     is using a static version of the library
// 
// Now supposing PKMFCEXT is defined, all files within this DLL are compiled with 
// PKMFCEXT_EXPORTS symbol defined. This symbol should not be defined on any project 
// that uses this DLL. This way any other project whose source files include this file see 
// PKMFCEXT_API functions as being imported from a DLL, wheras this DLL sees symbols
// defined with this macro as being exported.
//
//
// Note : We use our own preprocessor symbols here instead of _AFXEXT preprocessor symbol;
// preprocessor symbol _AFXEXT is intentionally not used. 
// The symbold _AFXEXT is automatically defined in extension dlls created by Visual Studio 
// project-wizard, but such standard extension symbol may be used for extension DLLs only 
// as long as we do not have multiple layers of extension DLLs. 
// If we have extension DLLs which call or derive from classes in other our own extension DLLs, 
// which then derive from the MFC classes, we must use your own preprocessor symbol 
// to avoid ambiguity.

#if defined(PKMFCEXT) && defined(PKMFCEXT_EXPORTS)
    #define PKMFCEXT_CLASS      PKMFCEXT_CLASS_EXPORT
    #define PKMFCEXT_API        PKMFCEXT_API_EXPORT
    #define PKMFCEXT_DATA       PKMFCEXT_DATA_EXPORT
    #define DECLARE_SERIAL_EXT   DECLARE_SERIAL_EXPORT
#elif defined(PKMFCEXT) && !defined(PKMFCEXT_EXPORTS)
    #define PKMFCEXT_CLASS      PKMFCEXT_CLASS_IMPORT
    #define PKMFCEXT_API        PKMFCEXT_API_IMPORT
    #define PKMFCEXT_DATA       PKMFCEXT_DATA_IMPORT
    #define DECLARE_SERIAL_EXT   DECLARE_SERIAL_IMPORT
#else
    #define PKMFCEXT_CLASS
    #define PKMFCEXT_API
    #define PKMFCEXT_DATA
    #define DECLARE_SERIAL_EXT   DECLARE_SERIAL
#endif


#ifndef messageEx
// New pragma messageEx. Usage is similar like with the #pragma message,
// but the preprocessor includes the file & line number into the output.
// #pragma messageEx(blabla) 
#define chSTR(x)         #x
#define chSTR2(x)        chSTR(x)
#define messageEx(desc)  message(__FILE__ "("chSTR2(__LINE__) "):" #desc)
#endif // messageEx

#ifdef AVDBG
#ifdef _DEBUG
#define AVDBG(X)          X
#else // _DEBUG
#define AVDBG(X)
#endif // _DEBUG
#endif // AVDBG


#endif // !defined(__PKMFCEXT_EXPORT_H__)

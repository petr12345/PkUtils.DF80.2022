// RuntimeTpt.h  header file
// Support of dynamic creation & serialization for templates
//

// Macros DECLARE..._T and IMPLEMENT_..._T defined in this header work 
// as a substitution ( replacement ) of standard MFC runtime macros 
// DECLARE... and IMPLEMENT_...
//
// --Motivation: 
// Standard MFC macros DECLARE... and IMPLEMENT_... are used for a 
// CObject-derived classes, falling into three categories:
// 
// i/ DECLARE_DYNAMIC, IMPLEMENT_DYNAMIC
//    Adds the ability to access run-time information about an object's class 
// ii/ DECLARE_DYNCREATE, IMPLEMENT_DYNCREATE
//    Enables instances of CObject-derived classes to be created dynamically at run time.
// iii/ DECLARE_SERIAL, IMPLEMENT_SERIAL
//    Generates the code necessary for a class to be serialized.
//
//  Now, the difficulty with MFC standard macros is that you cannot use it for 
//  template classes. Macros ..._T overcome this difficulty, assumming
//  your template class is CObject-derived and has one template argument.
//  In principe, this is similar to MFC marco BEGIN_TEMPLATE_MESSAGE_MAP.
//
//  --Usage: 
//  Let's say you have template CLogInfo with single argument TARG, like
//    template<class TARG> class CLogInfo : public TLogInfoPredecessor
//  i/ To add of CLogInfo derived-classes run-time information, use
//    	DECLARE_DYNAMIC_T(CLogInfo, TARG)
//    	IMPLEMENT_DYNAMIC_T(CLogInfo, TARG, TLogInfoPredecessor)
//  ii/ To enable instances of CLogInfo derived-classes be created dynamically 
//      at run time, use
//    	DECLARE_DYNCREATE_T(CLogInfo, TARG)
//    	IMPLEMENT_DYNCREATE_T(CLogInfo, TARG, TLogInfoPredecessor)
//  iii/ To add of CLogInfo derived-classes runtime-serialization ability
//    	DECLARE_SERIAL_T(CLogInfo, TARG)
//    	IMPLEMENT_SERIAL_T(CLogInfo, TARG, TLogInfoPredecessor)
//
// --Reference: 
// http://www.keyongtech.com/5322186-how-to-serialize-mfc-template
// http://visual-c.itags.org/visual-c-c++/138043/
// 

/////////////////////////////////////////////////////////////////////////////
//	INCLUDE FILES
/////////////////////////////////////////////////////////////////////////////
#include "afx.h"

/////////////////////////////////////////////////////////////////////////////
//	MANIFESTED CONSTANTS & MACROS
/////////////////////////////////////////////////////////////////////////////
#ifndef __RUNTIME_TPT_H__
#define __RUNTIME_TPT_H__

#pragma once

// Helper macros
#define _RUNTIME_CLASS_T(class_name, templ_arg) ((CRuntimeClass*)(&class_name<templ_arg>::class##class_name##templ_arg))
#ifdef _AFXDLL
#define RUNTIME_CLASS_T(class_name, templ_arg) class_name<templ_arg>::GetThisClass()
#else
#define RUNTIME_CLASS_T(class_name, templ_arg) _RUNTIME_CLASS_T(class_name, templ_arg)
#endif


/////////////////////////////////////////////////////////////////////////////
// Helper macros for declaring CRuntimeClass compatible classes

// -- 1. declarations used inside the class; similar way as DECLARE_DYNAMIC

// Note: When defined, the preprocessor symbol _AFXDLL indicates 
// that the shared version of MFC is being used by the target linked module 
// (either a DLL or an .exe application).
// When not defined, MFC is linked to resulting module "statically".
#ifdef _AFXDLL
#define DECLARE_DYNAMIC_T(class_name, templ_arg) \
protected: \
	static CRuntimeClass* PASCAL _GetBaseClass(); \
public: \
    static const CRuntimeClass class##class_name##templ_arg; \
	static CRuntimeClass* PASCAL GetThisClass(); \
	virtual CRuntimeClass* GetRuntimeClass() const; \

#define _DECLARE_DYNAMIC_T(class_name, templ_arg) \
protected: \
	static CRuntimeClass* PASCAL _GetBaseClass(); \
public: \
	static CRuntimeClass class##class_name##templ_arg; \
	static CRuntimeClass* PASCAL GetThisClass(); \
	virtual CRuntimeClass* GetRuntimeClass() const; \

#else
#define DECLARE_DYNAMIC_T(class_name, templ_arg) \
public: \
	static const CRuntimeClass class##class_name##templ_arg; \
	virtual CRuntimeClass* GetRuntimeClass() const; \

#define _DECLARE_DYNAMIC_T(class_name, templ_arg) \
public: \
	static CRuntimeClass class##class_name##templ_arg; \
	virtual CRuntimeClass* GetRuntimeClass() const; \

#endif //_AFXDLL

// not serializable, but dynamically constructable
#define DECLARE_DYNCREATE_T(class_name, templ_arg) \
	DECLARE_DYNAMIC_T(class_name, templ_arg) \
	static CObject* PASCAL CreateObject();

#define _DECLARE_DYNCREATE_T(class_name, templ_arg) \
	_DECLARE_DYNAMIC_T(class_name, templ_arg) \
	static CObject* PASCAL CreateObject();

#define DECLARE_SERIAL_T(class_name, templ_arg) \
	_DECLARE_DYNCREATE_T(class_name, templ_arg) \
	template<class templ_arg> \
	AFX_API friend CArchive& AFXAPI operator>>(CArchive& ar, class_name<templ_arg>* &pOb); \
    static AFX_CLASSINIT const _init_##class_name##templ_arg;

// -- 2. implementations used outside the class declaration ( similar way as MFC macros IMPLEMENT_... )

#ifdef _AFXDLL
#define IMPLEMENT_RUNTIMECLASS_T(class_name, templ_arg, base_class_name, wSchema, pfnNew, class_init) \
	template<class templ_arg> \
    CRuntimeClass* PASCAL class_name<templ_arg>::_GetBaseClass() \
		{ return RUNTIME_CLASS(base_class_name); } \
    template<class templ_arg> \
	AFX_COMDAT const CRuntimeClass class_name<templ_arg>::class##class_name##templ_arg = { \
        #class_name#templ_arg, \
        sizeof(class_name<templ_arg>), \
        wSchema, \
        pfnNew, \
		&class_name<templ_arg>::_GetBaseClass, NULL, class_init }; \
	template<class templ_arg> \
    CRuntimeClass* PASCAL class_name<templ_arg>::GetThisClass() \
        { return _RUNTIME_CLASS_T(class_name, templ_arg); } \
	template<class templ_arg> \
    CRuntimeClass* class_name<templ_arg>::GetRuntimeClass() const \
		{ return _RUNTIME_CLASS_T(class_name, templ_arg); }

#define _IMPLEMENT_RUNTIMECLASS_T(class_name, templ_arg, base_class_name, wSchema, pfnNew, class_init) \
	template<class templ_arg> CRuntimeClass* PASCAL class_name<templ_arg>::_GetBaseClass() \
		{ return RUNTIME_CLASS(base_class_name); } \
    template<class templ_arg> \
	AFX_COMDAT CRuntimeClass class_name<templ_arg>::class##class_name##templ_arg = { \
        #class_name#templ_arg, \
        sizeof(class_name<templ_arg>), \
        wSchema, \
        pfnNew, \
        &class_name<templ_arg>::_GetBaseClass, NULL, class_init }; \
	template<class templ_arg> CRuntimeClass* PASCAL class_name<templ_arg>::GetThisClass() \
		{ return _RUNTIME_CLASS_T(class_name, templ_arg); } \
	template<class templ_arg> CRuntimeClass* class_name<templ_arg>::GetRuntimeClass() const \
		{ return _RUNTIME_CLASS_T(class_name, templ_arg); }

#else
#define IMPLEMENT_RUNTIMECLASS_T(class_name, templ_arg, base_class_name, wSchema, pfnNew, class_init) \
	template<class templ_arg> \
	CRuntimeClass* class_name<templ_arg>::GetRuntimeClass() const \
		{ return RUNTIME_CLASS_T(class_name, templ_arg); }  \
	template<class templ_arg> \
        AFX_COMDAT const CRuntimeClass class_name<templ_arg>::class##class_name##templ_arg = { \
		#class_name#templ_arg, \
        sizeof(class_name<templ_arg>), \
        wSchema, \
        pfnNew, \
		&class_name<templ_arg>::_GetBaseClass, NULL, class_init };

#define _IMPLEMENT_RUNTIMECLASS_T(class_name, templ_arg, base_class_name, wSchema, pfnNew, class_init) \
	template<class templ_arg> \
	CRuntimeClass* class_name<templ_arg>::GetRuntimeClass() const \
		{ return RUNTIME_CLASS_T(class_name, templ_arg); }  \
	template<class templ_arg> \
	AFX_COMDAT CRuntimeClass class_name<templ_arg>::class##class_name##templ_arg = { \
		#class_name#templ_arg, \
        sizeof(class_name<templ_arg>), \
        wSchema, \
        pfnNew, \
		&class_name<templ_arg>::_GetBaseClass, NULL, class_init };
#endif // _AFXDLL

#define IMPLEMENT_DYNAMIC_T(class_name, templ_arg, base_class_name) \
	IMPLEMENT_RUNTIMECLASS_T(class_name, templ_arg, base_class_name, 0xFFFF, NULL, NULL);

#define IMPLEMENT_DYNCREATE_T(class_name, templ_arg, base_class_name) \
	template<class templ_arg> CObject* PASCAL class_name<templ_arg>::CreateObject() \
		{ return new class_name<templ_arg>(); } \
	IMPLEMENT_RUNTIMECLASS_T(class_name, templ_arg, base_class_name, 0xFFFF, \
		class_name<templ_arg>::CreateObject, NULL)

#define IMPLEMENT_SERIAL_T(class_name, templ_arg, base_class_name, wSchema) \
	template<class templ_arg> CObject* PASCAL class_name<templ_arg>::CreateObject() \
		{ return new class_name<templ_arg>(); } \
	_IMPLEMENT_RUNTIMECLASS_T(class_name, templ_arg, base_class_name, wSchema, class_name<templ_arg>::CreateObject, &_init_##class_name##templ_arg) \
    template<class templ_arg>  \
    AFX_CLASSINIT const class_name<templ_arg>::_init_##class_name##templ_arg(RUNTIME_CLASS_T(class_name, templ_arg)); \
	template<class templ_arg>  \
    CArchive& AFXAPI operator>>(CArchive& ar, class_name<templ_arg>* &pOb) \
	{ pOb = (class_name<templ_arg>*) ar.ReadObject(RUNTIME_CLASS_T(class_name, templ_arg)); \
	return ar; }

#endif // __RUNTIME_TPT_H__

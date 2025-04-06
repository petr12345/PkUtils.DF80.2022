// TaggingFieldTypes.h
//
/////////////////////////////////////////////////////////////////////////////

#pragma once
#if !defined(__TAGGING_FIELDTYPES__H__)
#define __TAGGING_FIELDTYPES__H__

enum tagMyFields
{
    IdField_NONE  = 0,
    IdField_Year  = 1,
    IdField_Month = 2,
    IdField_DayoftheWeek = 3,
    IdField_Dog = 4,
} tMyFields;

inline CArchive& AFXAPI operator>>(CArchive& ar, tagMyFields &val)
{ 
    ar >> (int&)val;
    return ar; 
}


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(__TAGGING_FIELDTYPES__H__)

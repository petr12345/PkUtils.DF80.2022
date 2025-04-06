////////////////////////////////////////////////////////////////////////
// SubstMapping.h
////////////////////////////////////////////////////////////////////////

#ifndef __SUBSTMAPPING_H__
#define __SUBSTMAPPING_H__

///////////////////////////////////////////
// INCLUDE FILES
///////////////////////////////////////////
#include "AfxTempl.h"
#include "PkArray.h"

///////////////////////////////////////////
// MANIFESTED CONSTANTS & MACROS
///////////////////////////////////////////
#define    LOGINFO_VERSION                0
#define    SUBSTLOGDATA_VERSION           0

///////////////////////////////////////////
// TYPES
///////////////////////////////////////////


template<class TFIELDID> struct SubstDescr
{
public:
    TFIELDID    valId;
    LPCTSTR     lpTxt;
};

template<class TFIELDID> interface ISubstDescrProvider
{
    virtual SubstDescr<TFIELDID> const* GetSubstDescr() const = 0;
};

template<class TFIELDID> class SubstMapKeeper
{
protected:
    static SubstDescr<TFIELDID> const m_stdEmptyMap;
    SubstDescr<TFIELDID> const *m_lpMap;    // map of (field id) -> (field text)

public:
    SubstMapKeeper();
    SubstMapKeeper(SubstDescr<TFIELDID> const *lpMap);
    SubstMapKeeper(SubstMapKeeper<TFIELDID> const &rhs);

    SubstDescr<TFIELDID> const* GetSubstMap() const;
    void AssignSubstMap(SubstDescr<TFIELDID> const* lpMap);

   static SubstDescr<TFIELDID> const* FindMapItem(SubstDescr<TFIELDID> const* lpMap, TFIELDID  item);
   SubstDescr<TFIELDID> const* FindMapItem(TFIELDID item) const;
};


#include "SubstMapping.hpp"

#endif // __SUBSTMAPPING_H__

// SubstMapping.hpp - template SubstMapKeeper<TFIELDID> implementation file
//

/////////////////////////////////////////////////////////////////////////////
// INCLUDE FILES
/////////////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////////////
// MANIFESTED CONSTANTS & MACROS
/////////////////////////////////////////////////////////////////////////////
#ifndef kInvalidSubstElemId
#define kInvalidSubstElemId  0
#endif

/////////////////////////////////////////////////////////////////////////////
// PRIVATE SYMBOLS
/////////////////////////////////////////////////////////////////////////////
template<class TFIELDID> 
SubstDescr<TFIELDID> const SubstMapKeeper<TFIELDID>::m_stdEmptyMap = {
    (TFIELDID)kInvalidSubstElemId, 
    NULL };

/////////////////////////////////////////////////////////////////////////////
// CLASS DEFINITIONS
/////////////////////////////////////////////////////////////////////////////

template<class TFIELDID> 
SubstMapKeeper<TFIELDID>::SubstMapKeeper()
{
    AssignSubstMap(&m_stdEmptyMap);
}

template<class TFIELDID> 
SubstMapKeeper<TFIELDID>::SubstMapKeeper(SubstDescr<TFIELDID> const *lpMap)
{
    AssignSubstMap(lpMap);
}

template<class TFIELDID> 
SubstMapKeeper<TFIELDID>::SubstMapKeeper(SubstMapKeeper const &rhs)
{
    AssignSubstMap(rhs.GetSubstMap());
}

template<class TFIELDID> 
SubstDescr<TFIELDID> const* SubstMapKeeper<TFIELDID>::GetSubstMap() const
{
    return m_lpMap;
}

template<class TFIELDID> 
void SubstMapKeeper<TFIELDID>::AssignSubstMap(SubstDescr<TFIELDID> const* lpMap)
{
    ASSERT(lpMap != NULL);
    m_lpMap = lpMap;
}

template<class TFIELDID> 
/*static */ SubstDescr<TFIELDID> const* SubstMapKeeper<TFIELDID>::FindMapItem(
    SubstDescr<TFIELDID> const* lpMap, 
    TFIELDID  item)
{
    TFIELDID  tmp_item;
    SubstDescr<TFIELDID> const* lpTmp;

    if ((kInvalidSubstElemId != item) && (lpTmp = lpMap))
    {
        for(;;lpTmp++)
        {
            if (kInvalidSubstElemId == (tmp_item = lpTmp->valId))
            {
                return NULL;
            }
            if (tmp_item == item)
            {
                ASSERT(lpTmp->lpTxt);
                return lpTmp;
            }
        }
    }
    else
    {
        ASSERT(FALSE);
    }
    return NULL;
}

template<class TFIELDID> 
SubstDescr<TFIELDID> const* SubstMapKeeper<TFIELDID>::FindMapItem(TFIELDID item) const
{
    return FindMapItem(GetSubstMap(), item);
}

// PkArray.h : interface of the classes
//              CPkArrBase, CPkArray, CPkIntrinsicArray, CPkTypedPtrArray 
//
//


/////////////////////////////////////////////////////////////////////////////
// INCLUDE FILES
/////////////////////////////////////////////////////////////////////////////
#include "AfxTempl.h"
#include "StdAfx.h"

/////////////////////////////////////////////////////////////////////////////
// MANIFESTED CONSTANTS & MACROS
/////////////////////////////////////////////////////////////////////////////

#if !defined(AFX_PKARRAY__H__BA3E5C_3F91_427C_973A_A778B722635D__INCLUDED_)
#define AFX_PKARRAY__H__BA3E5C_3F91_427C_973A_A778B722635D__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/////////////////////////////////////////////////////////////////////////////
// FUNCTIONS
/////////////////////////////////////////////////////////////////////////////
/** 
    Copy the object by serializing it to serialization archive and 
    reading the resulting new object from it.
    @param  ptr Pointer to the object to be copied
    @return Pointer to the copied object (or NULL in case the null pointer apssed as argument).
*/
PKMFCEXT_API CObject* WINAPI CopyObject(CObject const *ptr);

/////////////////////////////////////////////////////////////////////////////
//  CLASS DEFINITIONS
/////////////////////////////////////////////////////////////////////////////

/**	CPkArrBase is a classic CArray, supporting the enumeration like forEach and FirstThat
    together with the other operations (Find, ToCList ...).
    It is used as prececessor of both CPkArray and CPkIntrinsicArray.
    @see CPkArray
    @see CPkIntrinsicArray
*/
template<class TYPE, class ARG_TYPE> class CPkArrBase : public CArray < TYPE, ARG_TYPE >
{
protected:
    typedef void CALLBACK tEachItemFn(typename TYPE &item, WPARAM wPar, LPARAM lPar);
    typedef tEachItemFn *lptEachItemFn;

    typedef BOOL CALLBACK tFirstThatItemFn(typename TYPE const &item, WPARAM wPar, LPARAM lPar);
    typedef tFirstThatItemFn *lpFirstThatItemFn;

    typedef int CALLBACK tCompareItemFn(typename TYPE const &itemA, typename TYPE const &itemB, WPARAM wPar, LPARAM lPar);
    typedef tCompareItemFn *lpCompareItemFn;

public:
    /// constructor
    CPkArrBase();
    /// copy constructor
    CPkArrBase(CArray <TYPE, ARG_TYPE> const &rfArray);

    /// convert between CPkArrBase* and CArray*
    operator CArray < TYPE, ARG_TYPE >* () { return this; }
    /// convert between CPkArrBase* and CArray const *
    operator CArray < TYPE, ARG_TYPE > const * () const { return this; }
    /// assignment operator
    CPkArrBase<TYPE, ARG_TYPE>& operator = (CArray < TYPE, ARG_TYPE > const &what );

    /// Performs the action (*lpFn) for each item, where lpFn is a callback function provided by user.
    void ForEach(lptEachItemFn lpFn, WPARAM wPar = 0, LPARAM lPar = 0);
    /// Finds the first item complying the condition (*lpFn), where  lpFn is a callback function provided by user.
    INT_PTR FirstThat(lpFirstThatItemFn lpFn, WPARAM wPar = 0, LPARAM lPar = 0) const;
    /// Finds the last item complying the condition (*lpFn), where  lpFn is a callback function provided by user.
    INT_PTR LastThat(lpFirstThatItemFn lpFn, WPARAM wPar = 0, LPARAM lPar = 0) const;
    /// Finds all the items complying the condition (*lpFn), where  lpFn is a callback function provided by user.
    INT_PTR FindAllThat(lpFirstThatItemFn lpFn, CPkArrBase &output, WPARAM wPar = 0, LPARAM lPar = 0) const;
    /**
        Sorts the array by bublesort method, comparing the items by call of (*lpCompFn), where lpCompFn is a callback function provided by user. 
        Returns TRUE if the array has been changed, FALSE if no change 
    */
    BOOL BubbleSort(lpCompareItemFn lpCompFn, WPARAM wPar = 0, LPARAM lPar = 0, BOOL bReverseOrder = FALSE);
    /**
        Checks if the array is sorted, comparing the items by call of (*lpCompFn), where lpCompFn is a callback function provided by user. 
        Returns TRUE if is sorted, FALSE if is not. Returns TRUE for array with zero ore one element.
    */
    BOOL IsSorted(lpCompareItemFn lpCompFn, WPARAM wPar = 0, LPARAM lPar = 0, BOOL bReverseOrder = FALSE) const;

    /**
        A more object oriented and friendly version of FirstThat(). 
        This requires that TYPE supported an overloaded == (equality) operator, but it should anyway
    */
    INT_PTR Find( TYPE const& item ) const;

    /**
        Transform a CPkArrBase into a CList
        @param v_CList [in]
    */
    void ToCList(CList<TYPE, ARG_TYPE>* v_CList) const;

protected:
};

/**	CPkArray is an array derived from CPkArrBase, intended for usage used as an array
    which is keeping structurized elements (any struct, CStrings, CObject-derived classes...).<br>
    Besides the inherited methods, it defines PkSerializeObject method, 
    which should be used to serialize your own CObject-derived classes.
    @see CPkArrBase
    @see CPkIntrinsicArray
*/
template< class TYPE > class CPkArray : public CPkArrBase < TYPE, TYPE& >
{
public:
    /// constructor
    CPkArray();
    /// copy constructor
    CPkArray(CArray <TYPE, TYPE&> const &rfArray);
    /// assignment operator
    CPkArray<TYPE>& operator = (CArray < TYPE, TYPE&> const &what );
    /**
    This is a little more user-friendly version of Serialize() for CPkArrBase.  
    Use this to serialize your own CObject-derived classes which support 
    serialization.
    @param ar The CArchive with which data will be serialized.
    */
    void PkSerializeObject(CArchive &ar);
};

/**	CPkIntrinsicArray is an array derived from CPkArrBase, intended to be used as
    an array keeping intrinsic (simple ) elements, like integers, pointers, DWORDs ....<br>
    Comparing to CPkArray it uses different tempate-argumet list 
    (not CPkArrBase < TYPE, &TYPE>, but < TYPE, TYPE> ) for two reasons:<br>
    i/ this way we avoid compile error C2664, otherwise caused when using CPkArray with pointers
        and expanding the method AFX_INLINE int CArray<TYPE, ARG_TYPE>::Add(ARG_TYPE newElement)<br>
    &nbsp	<b>Example:</b>
    <pre>
    &nbsp	CMyObject::CMyObject()
    &nbsp	{
    &nbsp		global_array.Add(this);
    &nbsp	}
            error C2664: 'Add' : cannot convert parameter 1 from 'class CMyObject *' to 'class CMyObject *& '
    </pre>
    ii/ using directly the TYPE argument with intrinsic types may run faster 
*/
template< class TYPE > class CPkIntrinsicArray : public CPkArrBase < TYPE, TYPE>
{
public:
    /// constructor
    CPkIntrinsicArray();
    /// copy constructor
    CPkIntrinsicArray(CArray <TYPE, TYPE> const &rfArray);
    /// assignment operator
    CPkIntrinsicArray<TYPE>& operator = (CArray < TYPE, TYPE> const &what );
};

/** CTypedPtrArrayEx is a template derived from MFC CTypedPtrArray.
    Analogically, as CTypedPtrArray, it provides type-safe wrapper 
    for objects of class CPtrArray or CObArray.<br>
    Its purpose is to extend the CTypedPtrArray functionality with some useful 
    methods like Find, ForEach, FirstThat, FindAllThat.
*/
template<class BASE_CLASS, class PTRTYPE>
class CTypedPtrArrayEx : public CTypedPtrArray < BASE_CLASS, PTRTYPE >
{
protected:
    typedef void CALLBACK tEachPtrFn(PTRTYPE ptr, WPARAM wPar, LPARAM lPar);
    typedef tEachPtrFn *lptEachPtrFn;

    typedef BOOL CALLBACK tFirstThatPtrFn(PTRTYPE ptr, WPARAM wPar, LPARAM lPar);
    typedef tFirstThatPtrFn *lpFirstThatPtrFn;

public:
    /// constructor
    CTypedPtrArrayEx();

    /// destructor
    virtual ~CTypedPtrArrayEx();

#ifdef _DEBUG
    void AssertValid() const;
#endif

    /**
        Find the specified pointer in this array of pointers and return its index.
        @param ptrObj The pointer we are searching for.
        @return The zero-based index of pointer, if pointer found in the array ( -1 otherwise).
    */
    INT_PTR Find(PTRTYPE ptrObj) const;

    /// Performs the action (*lpFn) for each pointer, where lpFn is a callback function provided by user.
    void ForEach(lptEachPtrFn lpFn, WPARAM wPar = 0, LPARAM lPar = 0);

    /// Finds the index of first pointer complying the condition (*lpFn), where  lpFn is a callback function provided by user.
    INT_PTR FirstThat(lpFirstThatPtrFn lpFn, WPARAM wPar = 0, LPARAM lPar = 0) const;
    /// Finds the index of last pointer complying the condition (*lpFn), where  lpFn is a callback function provided by user.
    INT_PTR LastThat(lpFirstThatPtrFn lpFn, WPARAM wPar = 0, LPARAM lPar = 0) const;

    /// Finds all the pointers complying the condition (*lpFn), where  lpFn is a callback function provided by user.
    INT_PTR FindAllThat(lpFirstThatPtrFn lpFn, CTypedPtrArray < BASE_CLASS, PTRTYPE > &output, WPARAM wPar = 0, LPARAM lPar = 0) const;
};

/** CPkTypedPtrArray is a template derived from MFC CTypedPtrArrayEx.
    Analogically, as CTypedPtrArrayEx and CTypedPtrArray, it provides type-safe wrapper 
    for objects of class CPtrArray or CObArray.<br>
    Its purpose is to change CTypedPtrArray rules of memory management concerning
    contained objects. With the original CPtrArray or CObArray (and therefore 
    with the CTypedPtrArray), when the array object is deleted, or when its elements 
    are removed, only the CObject pointers are removed, not the objects they refer to.
    Therefore with MFC CTypedPtrArray the user is explicitly responsible for deleting 
    objects contained in CTypedPtrArray.<br>
    On the opposite, the CPkTypedPtrArray is considered to be an owner of all the objects
    in the array, and its destructor calls the method DeleteAndRemoveAll, deleting 
    all of them.<br>
    However the side-effect of this improvement is that two original methods 	
    <pre>
    INT_PTR CTypedPtrArray::Append(const CTypedPtrArray<BASE_CLASS, PTRTYPE>& src);
    void CTypedPtrArray::Copy(const CTypedPtrArray<BASE_CLASS, PTRTYPE>& src);
    </pre>
    needs to be changed. CPkTypedPtrArray overloads those two methods and both its 
    'Append' and 'Copy' now performs the actual copy of all the objects, calling internally
    <pre>
    CObject* WINAPI CopyObject(CObject const *ptr);
    </pre>
    The original CTypedPtrArray::Append, CTypedPtrArray::Copy are no longer safe 
    with this class and should not be used.
    @see CopyObject
    @see CTypedPtrArrayEx 
*/
template<class BASE_CLASS, class PTRTYPE>
class CPkTypedPtrArray : public CTypedPtrArrayEx < BASE_CLASS, PTRTYPE >
{
public:
    // Public 'using' declaration to make accessible both overloded FindAllThat 
    // ( the one defined here, and the other defined in CTypedPtrArrayEx ).
    // Otherwise the CTypedPtrArrayEx::FindAllThat would become unaccessible
    // and compiler could not recognize it without specificication CTypedPtrArrayEx::FindAllThat.
    using CTypedPtrArrayEx < BASE_CLASS, PTRTYPE > :: FindAllThat;

public:
    /// constructor
    CPkTypedPtrArray();

    /// destructor
    virtual ~CPkTypedPtrArray();

#ifdef _DEBUG
    void AssertValid() const;
#endif

    /// assignment operator
    CPkTypedPtrArray <BASE_CLASS, PTRTYPE >& operator = (CTypedPtrArray < BASE_CLASS, PTRTYPE > const &what );

    /** Deletes the elements starting at the given index and sets their pointers to zero. 
        The pointers are just set to zero, are not removed from the array.
        @param nIndex An integer index that is greater than or equal to 0 and less than the value returned by GetSize
        @param nCount The number of elements to delete.
        @see DeleteAndRemoveAt
        @see DeleteAndRemoveAll
    */
    void DeleteAt(INT_PTR nIndex, INT_PTR nCount = 1);

    /** Deletes and removes one or more elements starting at a specified index in an array. 
        In the process, it shifts down all the elements above the removed element(s).
        @param nIndex An integer index that is greater than or equal to 0 and less than the value returned by GetSize
        @param nCount The number of elements to delete and remove.
        @see DeleteAt
        @see DeleteAndRemoveAll
    */
    void DeleteAndRemoveAt(INT_PTR nIndex, INT_PTR nCount = 1);

    /** Delete and remove all the elements. Called internally from the destructor.
        @see DeleteAt
        @see DeleteAndRemoveAt
    */
    void DeleteAndRemoveAll();

    /// Finds all the pointers complying the condition (*lpFn), where  lpFn is a callback function provided by user.
    INT_PTR FindAllThat(lpFirstThatPtrFn lpFn, CPkTypedPtrArray < BASE_CLASS, PTRTYPE > &output, WPARAM wPar = 0, LPARAM lPar = 0) const;

    /**
       Overloaded method of the predecesor. This method adds the contents of another array 
       to the end of the specified array, by copying all the objects the source pointer 
       array points to. This is done internally by calling the function 
       <pre>
            CObject* WINAPI CopyObject(CObject const *ptr);
       </pre>
       @param src Specifies the source of the elements to be appended to an array.
       @return Returns the index of the first appended element.
    */
    INT_PTR Append(const CTypedPtrArray<BASE_CLASS, PTRTYPE>& src);

    /**
       Overloaded method of the predecesor. This method make an copy of another 
       ( src ) array to the specified (this) array, by copying all the objects 
       the source pointer array points to. This is done internally by calling the function 
       <pre>
            CObject* WINAPI CopyObject(CObject const *ptr);
       </pre>
       @param src Specifies the source of the elements to be copied.
    */
    void Copy(const CTypedPtrArray<BASE_CLASS, PTRTYPE>& src);
};

/////////////////////////////////////////////////////////////////////////////
//  CPkArrBase implementation

template<class TYPE, class ARG_TYPE>
CPkArrBase<TYPE, ARG_TYPE>::CPkArrBase()
{
}

template<class TYPE, class ARG_TYPE>
CPkArrBase<TYPE, ARG_TYPE>::CPkArrBase(CArray <TYPE, ARG_TYPE> const &rfArray)
{
    *this = rfArray;
}

template<class TYPE, class ARG_TYPE>
CPkArrBase<TYPE, ARG_TYPE>& CPkArrBase<TYPE, ARG_TYPE>::operator = (CArray <TYPE, ARG_TYPE> const &what)
{
    this->RemoveAll();
    this->Copy(what);
    return *this;
}

template<class TYPE, class ARG_TYPE>
void CPkArrBase<TYPE, ARG_TYPE>::ForEach(lptEachItemFn lpFn, WPARAM wPar, LPARAM lPar)
{
    INT_PTR ii, isz;

    ASSERT(!IsBadCodePtr((FARPROC)lpFn));
    for(ii = 0, isz = GetSize(); ii < isz; ii++)
    {
        (*lpFn)(this->ElementAt(ii), wPar, lPar);
    }
}

template<class TYPE, class ARG_TYPE>
INT_PTR CPkArrBase<TYPE, ARG_TYPE>::FirstThat(lpFirstThatItemFn lpFn, WPARAM wPar, LPARAM lPar) const
{
    INT_PTR ii, isz;

    ASSERT(!IsBadCodePtr((FARPROC)lpFn));
    for(ii = 0, isz = GetSize(); ii < isz; ii++)
    {
        if ((*lpFn)((const_cast<CPkArrBase*>(this))->ElementAt(ii), wPar, lPar))
        {
            return ii;
        }
    }

    return -1;
}

template<class TYPE, class ARG_TYPE>
INT_PTR CPkArrBase<TYPE, ARG_TYPE>::LastThat(lpFirstThatItemFn lpFn, WPARAM wPar, LPARAM lPar) const
{
    INT_PTR ii, isz;
    INT_PTR result = -1;

    ASSERT(!IsBadCodePtr((FARPROC)lpFn));
    for(ii = 0, isz = GetSize(); ii < isz; ii++)
    {
        if ((*lpFn)((const_cast<CPkArrBase*>(this))->ElementAt(ii), wPar, lPar))
        {
            result = ii;
        }
    }

    return result;
}

template<class TYPE, class ARG_TYPE>
INT_PTR CPkArrBase<TYPE, ARG_TYPE>::FindAllThat(lpFirstThatItemFn lpFn, CPkArrBase &output, WPARAM wPar, LPARAM lPar) const
{
    INT_PTR ii, isz;

    ASSERT(!IsBadCodePtr((FARPROC)lpFn));
    output.RemoveAll();
    for(ii = 0, isz = GetSize(); ii < isz; ii++)
    {
        if ((*lpFn)((const_cast<CPkArrBase*>(this))->ElementAt(ii), wPar, lPar))
        {
            output.Add((*this)[ii]);
        }
    }
    return output.GetSize();
}

template<class TYPE, class ARG_TYPE>
INT_PTR CPkArrBase<TYPE, ARG_TYPE>::Find( TYPE const& item) const
{
    INT_PTR ii, isz;

    for(ii = 0, isz = GetSize(); ii < isz; ii++)
    {
        if( ((const_cast<CPkArrBase*>(this))->ElementAt(ii) == item ) )
        {
            return ii;
        }
    }
    return -1;
}

template<class TYPE, class ARG_TYPE>
BOOL CPkArrBase<TYPE, ARG_TYPE>::BubbleSort(lpCompareItemFn lpCompFn, 
    WPARAM wPar, LPARAM lPar, BOOL bReverseOrder)
{
    TYPE*    lpItem;
    TYPE*    lpNxt;
    INT_PTR  ii, lastInd;
    int      cmpRes;
    BOOL     bAnyChange = FALSE;

    ASSERT(!IsBadCodePtr((FARPROC)lpCompFn));
    for(lastInd = GetSize() - 1; lastInd > 0; lastInd--)
    {
        for (ii = 0; ii < lastInd; ii++)
        {
            lpItem = &(this->ElementAt(ii));
            lpNxt = &this->ElementAt(ii+1);
            if ((cmpRes = (*lpCompFn)(*lpItem, *lpNxt, wPar, lPar)) != 0)
            {
                if ((cmpRes > 0) == !bReverseOrder)
                {   // exchange the items
                    TYPE tmp = *lpItem;

                    *lpItem = *lpNxt;
                    *lpNxt = tmp;
                    bAnyChange = TRUE;
                }
            }
        }
    }
    return bAnyChange;
}

template<class TYPE, class ARG_TYPE>
BOOL CPkArrBase<TYPE, ARG_TYPE>::IsSorted(lpCompareItemFn lpCompFn, 
    WPARAM wPar, LPARAM lPar, BOOL bReverseOrder) const
{
    const TYPE* lpNxt;  // TYPE const *lpNxt 
    const TYPE* lpItem;
    INT_PTR  ii, nBound;
    int    cmpRes;
    BOOL   bResult = TRUE;

    ASSERT(!IsBadCodePtr((FARPROC)lpCompFn));
    for(ii = 0, nBound = GetUpperBound(); ii < nBound; )
    {
        lpItem = &(*this)[ii];
        lpNxt = &(*this)[++ii];
        if ((cmpRes = (*lpCompFn)(*lpItem, *lpNxt, wPar, lPar)) != 0)
        {
            if ((cmpRes > 0) == !bReverseOrder)
            {   // items are nor sorted
                bResult = FALSE;
                break;
            }
        }
    }
    return bResult;
}

template<class TYPE, class ARG_TYPE>
void CPkArrBase<TYPE, ARG_TYPE>::ToCList(CList<TYPE, ARG_TYPE>* v_CList) const
{
    INT_PTR ii, isz;

    for(ii = 0, isz = GetSize(); ii < isz; ii++)
    {
        TYPE PkArrayItem = this->GetAt(ii);
        v_CList->AddTail(PkArrayItem);
    }
}

/////////////////////////////////////////////////////////////////////////////
//  CPkArray implementation

template<class TYPE>
CPkArray<TYPE>::CPkArray()
{
}

template<class TYPE>
CPkArray<TYPE>::CPkArray(CArray <TYPE, TYPE&> const &rfArray)
{
    *this = rfArray;
}

template<class TYPE>
CPkArray<TYPE>& CPkArray<TYPE>::operator = (CArray <TYPE, TYPE&> const &what)
{
    return (CPkArray<TYPE>&) CPkArrBase < TYPE, TYPE& >::operator = ( what );
}

template<class TYPE>
void CPkArray<TYPE>::PkSerializeObject(CArchive &ar)
{
    ASSERT_VALID(this);

    CObject::Serialize(ar);
    if (ar.IsStoring())
    {
        ar.WriteCount(m_nSize);
        for (size_t i = 0; i < m_nSize; i++)
        {
            ar << &this->ElementAt(i);
        }
    }
    else
    {
        size_t  nCount = (size_t)ar.ReadCount();
        this->RemoveAll();

        for (; nCount--;)
        {
            TYPE * p_mp = NULL;
            ar >> p_mp;
            if (p_mp)
            {
                this->Add(*p_mp);
            }
            delete p_mp;
            p_mp = NULL;
        }
    }
}

/////////////////////////////////////////////////////////////////////////////
//  CPkIntrinsicArray implementation

template<class TYPE>
CPkIntrinsicArray<TYPE>::CPkIntrinsicArray()
{
}

template<class TYPE>
CPkIntrinsicArray<TYPE>::CPkIntrinsicArray(CArray <TYPE, TYPE> const &rfArray)
{
    *this = rfArray;
}

template<class TYPE>
CPkIntrinsicArray<TYPE>& CPkIntrinsicArray<TYPE>::operator = (CArray <TYPE, TYPE> const &what)
{
    return (CPkIntrinsicArray<TYPE>&) CPkArrBase < TYPE, TYPE >::operator = ( what );
}

/////////////////////////////////////////////////////////////////////////////
//  CTypedPtrArrayEx implementation

template<class BASE_CLASS, class PTRTYPE>
CTypedPtrArrayEx <BASE_CLASS, PTRTYPE>::CTypedPtrArrayEx()
{
}

template<class BASE_CLASS, class PTRTYPE>
CTypedPtrArrayEx <BASE_CLASS, PTRTYPE>::~CTypedPtrArrayEx()
{
}

#pragma warning ( disable : 4706) // get rid of C4706: assignment within conditional expression
#ifdef _DEBUG
template<class BASE_CLASS, class PTRTYPE>
void CTypedPtrArrayEx <BASE_CLASS, PTRTYPE>::AssertValid() const
{
    INT_PTR ii, isz;
    PTRTYPE ptr;

    BASE_CLASS::AssertValid();
    for(ii = 0, isz = GetSize(); ii < isz; ii++)
    {
        if (ptr = (*this)[ii])
        {
            ASSERT_VALID(ptr);
        }
    }
}
#endif // _DEBUG
#pragma warning ( default : 4706)

#pragma warning ( disable : 4706) // get rid of C4706: assignment within conditional expression
template<class BASE_CLASS, class PTRTYPE>
INT_PTR CTypedPtrArrayEx <BASE_CLASS, PTRTYPE>::Find(PTRTYPE ptrObj) const
{
#if defined(NO_ASM) || defined(_WIN64)

    INT_PTR ii, isz;

    for(ii = 0, isz = GetSize(); ii < isz; ii++)
    {
        if (ptrObj == this->GetAt(ii))
            return ii;
    }
    return -1;

#else // NO_ASM

    INT_PTR isz;
    INT_PTR iRes = -1;

    if (isz = GetSize())
    {
        CObject const **ptrBuffer = (CObject const **)BASE_CLASS::GetData();

        _asm 
        {
            push  edi              // save edi
            pushf                  // save flags
            cld                    // it will be forward search operation
            mov   edi, ptrBuffer   // edi = pointer to buffer 
            mov   eax, ptrObj      // eax = pointer we are searching for
            mov   ecx, isz         // ecx = count of items in the buffer
            mov   ebx, ecx         // ebx = ecx
            repne scasd            // search !
            jne   __finish         // jump if not found
            sub    ebx, ecx        // compute the found index
            dec    ebx
            mov    iRes, ebx       // assign result
          __finish:
            popf                   // restore flags
            pop   edi              // restore edi
        }

#ifdef _DEBUG
        if (iRes >= 0)
        {	// assert we found the right thing
            ASSERT(ptrObj == this->GetAt(iRes));
        }
#endif // _DEBUG
    }

    return iRes;

#endif // NO_ASM
}
#pragma warning ( default : 4706)

template<class BASE_CLASS, class PTRTYPE>
void CTypedPtrArrayEx <BASE_CLASS, PTRTYPE>::ForEach(lptEachPtrFn lpFn, WPARAM wPar, LPARAM lPar)
{
    INT_PTR ii, isz;

    ASSERT(!IsBadCodePtr((FARPROC)lpFn));
    for(ii = 0, isz = GetSize(); ii < isz; ii++)
    {
        (*lpFn)(this->ElementAt(ii), wPar, lPar);
    }
}

template<class BASE_CLASS, class PTRTYPE>
INT_PTR CTypedPtrArrayEx <BASE_CLASS, PTRTYPE>::FirstThat(lpFirstThatPtrFn lpFn, WPARAM wPar, LPARAM lPar) const
{
    INT_PTR ii, isz;

    ASSERT(!IsBadCodePtr((FARPROC)lpFn));
    for(ii = 0, isz = GetSize(); ii < isz; ii++)
    {
        if ((*lpFn)((const_cast<CTypedPtrArrayEx*>(this))->ElementAt(ii), wPar, lPar))
        {
            return ii;
        }
    }

    return -1;
}

template<class BASE_CLASS, class PTRTYPE>
INT_PTR CTypedPtrArrayEx <BASE_CLASS, PTRTYPE>::LastThat(lpFirstThatPtrFn lpFn, WPARAM wPar, LPARAM lPar) const
{
    INT_PTR ii, isz;
    INT_PTR result = -1;

    ASSERT(!IsBadCodePtr((FARPROC)lpFn));
    for(ii = 0, isz = GetSize(); ii < isz; ii++)
    {
        if ((*lpFn)((const_cast<CTypedPtrArrayEx*>(this))->ElementAt(ii), wPar, lPar))
        {
            result = ii;
        }
    }

    return result;
}

template<class BASE_CLASS, class PTRTYPE>
INT_PTR CTypedPtrArrayEx <BASE_CLASS, PTRTYPE>::FindAllThat(lpFirstThatPtrFn lpFn, CTypedPtrArray < BASE_CLASS, PTRTYPE > &output, WPARAM wPar, LPARAM lPar) const
{
    INT_PTR ii, isz;

    ASSERT(!IsBadCodePtr((FARPROC)lpFn));
    output.RemoveAll();
    for(ii = 0, isz = GetSize(); ii < isz; ii++)
    {
        PTRTYPE ptr = const_cast<CTypedPtrArrayEx*>(this)->ElementAt(ii);
        if ((*lpFn)(ptr, wPar, lPar))
        {
            output.Add(ptr);
        }
    }
    return output.GetSize();
}

/////////////////////////////////////////////////////////////////////////////
//  CPkTypedPtrArray implementation

template<class BASE_CLASS, class PTRTYPE>
CPkTypedPtrArray <BASE_CLASS, PTRTYPE>::CPkTypedPtrArray()
{
}

template<class BASE_CLASS, class PTRTYPE>
CPkTypedPtrArray <BASE_CLASS, PTRTYPE>::~CPkTypedPtrArray()
{
    DeleteAndRemoveAll();
}

#pragma warning ( disable : 4706) // get rid of C4706: assignment within conditional expression
#ifdef _DEBUG
template<class BASE_CLASS, class PTRTYPE>
void CPkTypedPtrArray <BASE_CLASS, PTRTYPE>::AssertValid() const
{
    INT_PTR ii, isz;
    PTRTYPE ptr;

    BASE_CLASS::AssertValid();
    for(ii = 0, isz = GetSize(); ii < isz; ii++)
    {
        if (ptr = (*this)[ii])
        {
            ASSERT_VALID(ptr);
        }
    }
}
#endif // _DEBUG
#pragma warning ( default : 4706)

template<class BASE_CLASS, class PTRTYPE>
CPkTypedPtrArray <BASE_CLASS, PTRTYPE >& CPkTypedPtrArray <BASE_CLASS, PTRTYPE>::operator = (CTypedPtrArray < BASE_CLASS, PTRTYPE > const &what )
{
    this->DeleteAndRemoveAll();
    this->Copy(what);
    return *this;
}

#pragma warning ( disable : 4706) // get rid of C4706: assignment within conditional expression
template<class BASE_CLASS, class PTRTYPE>
void CPkTypedPtrArray <BASE_CLASS, PTRTYPE>::DeleteAt(INT_PTR nIndex, INT_PTR nCount)
{
    PTRTYPE ptr;

    ASSERT_VALID(this);
    ASSERT(nIndex >= 0);
    ASSERT(nCount >= 0);
    ASSERT(nIndex + nCount <= GetSize());

    INT_PTR ii, nLimit;
    for(ii = nIndex, nLimit = nIndex + nCount; ii < nLimit; ii++)
    {
        if (ptr = (*this)[ii])
        {
            (*this)[ii] = NULL;
            delete ptr;
        }
    }
}
#pragma warning ( default : 4706)

template<class BASE_CLASS, class PTRTYPE>
void CPkTypedPtrArray <BASE_CLASS, PTRTYPE>::DeleteAndRemoveAt(INT_PTR nIndex, INT_PTR nCount)
{
    ASSERT_VALID(this);
    ASSERT(nIndex >= 0);
    ASSERT(nCount >= 0);
    ASSERT(nIndex + nCount <= GetSize());

    DeleteAt(nIndex, nCount);
    BASE_CLASS::RemoveAt(nIndex, nCount);
}

template<class BASE_CLASS, class PTRTYPE>
void CPkTypedPtrArray <BASE_CLASS, PTRTYPE>::DeleteAndRemoveAll()
{
    DeleteAndRemoveAt(0, BASE_CLASS::GetSize());
}

template<class BASE_CLASS, class PTRTYPE>
INT_PTR CPkTypedPtrArray <BASE_CLASS, PTRTYPE>::FindAllThat(lpFirstThatPtrFn lpFn, CPkTypedPtrArray < BASE_CLASS, PTRTYPE > &output, WPARAM wPar, LPARAM lPar) const
{
    CTypedPtrArray < BASE_CLASS, PTRTYPE > temp;

    ASSERT(!IsBadCodePtr((FARPROC)lpFn));
    FindAllThat(lpFn, temp, wPar, lPar);
    output.Copy(temp);
    return output.GetSize();
}

template<class BASE_CLASS, class PTRTYPE>
INT_PTR CPkTypedPtrArray <BASE_CLASS, PTRTYPE>::Append(const CTypedPtrArray<BASE_CLASS, PTRTYPE>& src)
{
    ASSERT_VALID(this);
    ASSERT(this != &src);   // cannot append and copy copy to itself

    INT_PTR ii, isz;
    INT_PTR nOldSize = this->GetSize();

    for(ii = 0, isz = src.GetSize(); ii < isz; ii++)
    {
        this->Add((PTRTYPE)CopyObject(src[ii]));
    }

    return nOldSize;
}

template<class BASE_CLASS, class PTRTYPE>
void CPkTypedPtrArray <BASE_CLASS, PTRTYPE>::Copy(const CTypedPtrArray<BASE_CLASS, PTRTYPE>& src)
{
    ASSERT_VALID(this);
    ASSERT(this != &src);   // cannot append and copy copy to itself

    this->DeleteAndRemoveAll();
    this->Append(src);
}

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PKARRAY__H__BA3E5C_3F91_427C_973A_A778B722635D__INCLUDED_)

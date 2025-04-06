// SubstEdit.cpp
//

/////////////////////////////////////////////////////////////////////////////
//  INCLUDE FILES
/////////////////////////////////////////////////////////////////////////////
#include "stdafx.h"

/////////////////////////////////////////////////////////////////////////////
//  MANIFESTED CONSTANTS & MACROS
/////////////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////////////
//  PRIVATE VARIABLES
/////////////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////////////
//  FUNCTIONS
/////////////////////////////////////////////////////////////////////////////
#ifdef _DEBUG
void WINAPI traceMsg(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    switch (msg)
    {
    case WM_SETCURSOR:
        TRACE0("\nWM_SETCURSOR");
        break;
    case WM_MOUSEACTIVATE:
        TRACE0("\nWM_MOUSEACTIVATE");
        break;
    case WM_NCHITTEST:
        TRACE0("\nWM_NCHITTEST");
        break;
    case WM_GETDLGCODE:
        TRACE0("\nWM_GETDLGCODE");
        break;
    case WM_KEYDOWN :
        TRACE0("\nWM_KEYDOWN");
        break;
    case  WM_KEYUP :
        TRACE0("\nWM_KEYUP");
        break;
    case WM_CHAR :
        TRACE0("\nWM_CHAR");
        break;
    case WM_SYSCHAR :
        TRACE0("\nWM_SYSCHAR");
        break;

    case EM_CANUNDO :
        TRACE0("\nEM_CANUNDO");
        break;
    case EM_UNDO    :
        TRACE0("\nEM_UNDO");
        break;

    case WM_MOUSEMOVE:
        TRACE0("\nWM_MOUSEMOVE");
        break;
    case WM_LBUTTONDOWN :
        TRACE0("\nWM_LBUTTONDOWN");
        break;
    case WM_LBUTTONUP :
        TRACE0("\nWM_LBUTTONUP");
        break;

    case WM_CUT   :
        TRACE0("\nWM_CUT");
        break;
    case WM_COPY :
        TRACE0("\nWM_COPY");
        break;
    case WM_PASTE :
        TRACE0("\nWM_PASTE");
        break;
    case WM_CLEAR :
        TRACE0("\nWM_CLEAR");
        break;
    case WM_UNDO :
        TRACE0("\nWM_UNDO");
        break;
    case WM_SYSCOMMAND:
        TRACE1("\nWM_SYSCOMMAND, wParam = %x", wParam);
        break;

    default:
        TRACE1("\nMessage %x", msg);
        break;
    }
}
#endif // _DEBUG


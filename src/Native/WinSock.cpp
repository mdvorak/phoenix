#include "StdAfx.h"
#include "ComInterop.h"
#include "WinSock.h"


UNKNOWN_BEGIN(WinSock)
	UNKNOWN_QUERYINTERFACE(__uuidof(IWinSock), IWinSock)
UNKNOWN_END()

WinSock::WinSock()
	: UNKNOWN_INIT()
{
}

WinSock::~WinSock()
{
}


HRESULT __stdcall WinSock::recv (/*[in]*/ __int64 s, /*[out]*/ unsigned char * buf,
        /*[in]*/ long len, /*[in]*/ long flags, /*[out,retval]*/ long * pRetVal )
{
	*pRetVal = ::recv((SOCKET)s, (char*)buf, len, flags);
	return S_OK;
}

HRESULT __stdcall WinSock::send (/*[in]*/ __int64 s, /*[in]*/ unsigned char * buf,
        /*[in]*/ long len, /*[in]*/ long flags, /*[out,retval]*/ long * pRetVal )
{
	*pRetVal = ::send((SOCKET)s, (char*)buf, len, flags);
	return S_OK;
}

HRESULT __stdcall WinSock::closesocket (
        /*[in]*/ __int64 s,
        /*[out,retval]*/ long * pRetVal )
{
	*pRetVal = ::closesocket((SOCKET)s);
	return S_OK;
}


void SimulateCursorPos(int x, int y);
void OverrideCursor();
void ReleaseCursorOverride();

HRESULT __stdcall WinSock::SimulateCursorPos(long x, long y, /*[out,retval]*/ long * pRetVal)
{
    ::SimulateCursorPos(x, y);
    return 0;
}

HRESULT __stdcall WinSock::OverrideCursor(/*[out,retval]*/ long * pRetVal)
{
    ::OverrideCursor();
    return 0;
}

HRESULT __stdcall WinSock::ReleaseCursorOverride(/*[out,retval]*/ long * pRetVal)
{
    ::ReleaseCursorOverride();
    return 0;
}

#pragma once


class WinSock : IWinSock
{
	virtual ~WinSock();

public:
	WinSock();
	
	virtual HRESULT __stdcall recv (
        /*[in]*/ __int64 s,
        /*[in]*/ unsigned char * buf,
        /*[in]*/ long len,
        /*[in]*/ long flags,
        /*[out,retval]*/ long * pRetVal );

    virtual HRESULT __stdcall send (
        /*[in]*/ __int64 s,
        /*[in]*/ unsigned char * buf,
        /*[in]*/ long len,
        /*[in]*/ long flags,
        /*[out,retval]*/ long * pRetVal );

	virtual HRESULT __stdcall closesocket (
        /*[in]*/ __int64 s,
        /*[out,retval]*/ long * pRetVal );

    virtual HRESULT __stdcall SimulateCursorPos(long x, long y, long * pRetVal);
    virtual HRESULT __stdcall OverrideCursor(long * pRetVal);
    virtual HRESULT __stdcall ReleaseCursorOverride(long * pRetVal);

	UNKNOWN_INTERFACE()
};

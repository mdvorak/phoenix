#include "StdAfx.h"
#include "ImportHook.h"
#include "SocksHook.h"
#include "ComInterop.h"

extern CIPtr<IComObject> comObject;

/*
	Builds SAFEARRAY from data without copying
*/
SAFEARRAY BuildSafeArray(const void * data, int len);

int WINAPI hook_connect(SOCKET s, const struct sockaddr *name, int namelen)
{
	sockaddr_in * inaddr = (sockaddr_in*)reinterpret_cast<const sockaddr_in*>(name);
	//inaddr->sin_addr.s_addr = inet_addr( "127.0.0.2" ); // 84.244.114.99
	//inaddr->sin_port = ntohs(10091);

	int err = ::connect(s, name, namelen);

	if(err != SOCKET_ERROR )
	{
		comObject->OnConnect(s, inaddr->sin_addr.S_un.S_addr, ntohs(inaddr->sin_port));
	}

    return err;
}

int WINAPI hook_WSAConnect(
  __in   SOCKET s,
  __in   const struct sockaddr *name,
  __in   int namelen,
  __in   LPWSABUF lpCallerData,
  __out  LPWSABUF lpCalleeData,
  __in   LPQOS lpSQOS,
  __in   LPQOS lpGQOS
  )
{
    sockaddr_in * inaddr = (sockaddr_in*)reinterpret_cast<const sockaddr_in*>(name);
    
    int err =  ::WSAConnect(s, name, namelen, lpCallerData, lpCalleeData, lpSQOS, lpGQOS);

    if(err != SOCKET_ERROR )
	{
		comObject->OnConnect(s, inaddr->sin_addr.S_un.S_addr, ntohs(inaddr->sin_port));
	}

    return err;
}

int WINAPI hook_closesocket(SOCKET s)
{
	int err = ::closesocket(s);

	comObject->OnCloseSocket(s);

    return err;
}

int WINAPI hook_recv(SOCKET s, char *buf, int len, int flags)
{
	__int64 managedSocket;
	comObject->ManagedSocket(&managedSocket);

	if(managedSocket == s)
	{
		SAFEARRAY safeBuf = BuildSafeArray(buf, len);
		long retval = 0;
		comObject->OnRecv(s, &safeBuf, len, flags, &retval);
		return retval;
	}

	return ::recv(s, buf, len, flags);
}

int
WINAPI
hook_WSARecv(
    IN SOCKET s,
    __in_ecount(dwBufferCount) __out_data_source(NETWORK) LPWSABUF lpBuffers,
    IN DWORD dwBufferCount,
    __out_opt LPDWORD lpNumberOfBytesRecvd,
    IN OUT LPDWORD lpFlags,
    __in_opt LPWSAOVERLAPPED lpOverlapped,
    __in_opt LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine
    )
{
    __int64 managedSocket;
	comObject->ManagedSocket(&managedSocket);

	if(managedSocket == s)
	{
		SAFEARRAY safeBuf = BuildSafeArray(lpBuffers, dwBufferCount);
		long retval = 0;
		comObject->OnRecv(s, &safeBuf, dwBufferCount, *lpFlags, &retval);
        if (lpNumberOfBytesRecvd != NULL)
            *lpNumberOfBytesRecvd = retval;
        return retval > 0 ? 0 : WSAENETDOWN;
	}

	return ::WSARecv(s, lpBuffers, dwBufferCount, lpNumberOfBytesRecvd, lpFlags, lpOverlapped, lpCompletionRoutine);
}

int WINAPI hook_send(SOCKET s, /*const*/ char *buf, int len, int flags)
{
	__int64 managedSocket;
	comObject->ManagedSocket(&managedSocket);

	if(managedSocket == s)
	{
		SAFEARRAY safeBuf = BuildSafeArray(buf, len);
		long retval = 0;
		comObject->OnSend(s, &safeBuf, len, flags, &retval);
		return retval;
	}

	return ::send(s, buf, len, flags);
}

int WINAPI hook_WSASend(
    IN SOCKET s,
    __in_ecount(dwBufferCount) LPWSABUF lpBuffers,
    IN DWORD dwBufferCount,
    __out_opt LPDWORD lpNumberOfBytesSent,
    IN DWORD dwFlags,
    __in_opt LPWSAOVERLAPPED lpOverlapped,
    __in_opt LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine
    )
{
    __int64 managedSocket;
	comObject->ManagedSocket(&managedSocket);

	if(managedSocket == s)
	{
		SAFEARRAY safeBuf = BuildSafeArray(lpBuffers, dwBufferCount);
		long retval = 0;
		comObject->OnSend(s, &safeBuf, dwBufferCount, dwFlags, &retval);
        
        if (lpNumberOfBytesSent != NULL)
            *lpNumberOfBytesSent = retval;

        return retval > 0 ? 0 : WSAENETDOWN;
	}

	return ::WSASend(s, lpBuffers, dwBufferCount, lpNumberOfBytesSent, dwFlags, lpOverlapped, lpCompletionRoutine);
}

int WINAPI hook_select(int nfds, fd_set * readfds, fd_set * writefds, fd_set * exceptfds, const struct timeval * timeout)
{
    __int64 managedSocket;
	comObject->ManagedSocket(&managedSocket);

    if (managedSocket > 0) {
	    comObject->SendPendingData();
    }

	long ret=::select(nfds, readfds, writefds, exceptfds, timeout); 

	comObject->ManagedSocket(&managedSocket);
	SOCKET s = (SOCKET)managedSocket;

	if(FD_ISSET(s, readfds))
	{
		FD_CLR(s, readfds);
		comObject->ReceiveData(s);
		ret--;
	}

	VARIANT_BOOL b;
	comObject->DataAvailable(s, &b);

	if(b != 0)
	{
		FD_SET(s, readfds);
		ret++;
	}

	return ret;
}

SOCKET WINAPI hook_socket(int af, int type, int protocol)
{
    return ::socket(af, type, protocol);
}

bool HookWinsockFunctions()
{
    HookImportedFunction("WS2_32.dll", "WSAConnect", 33, hook_WSAConnect);
    HookImportedFunction("WS2_32.dll", "WSASend", 76, hook_WSASend);
    HookImportedFunction("WS2_32.dll", "WSARecv", 71, hook_WSARecv);
//WSARecv    Ordinal:  71


	bool a=HookImportedFunction("wsock32.dll", "recv", 16, hook_recv) &&
		   HookImportedFunction("wsock32.dll", "send", 19, hook_send) &&
		   HookImportedFunction("wsock32.dll", "closesocket", 3, hook_closesocket) &&
		   HookImportedFunction("wsock32.dll", "connect", 4, hook_connect) &&
		   HookImportedFunction("wsock32.dll", "select", 18, hook_select) ;
		//   HookImportedFunction("wsock32.dll", "socket", 23, hook_socket);

            HookImportedFunction("WS2_32.dll", "recv", 16, hook_recv);

		   HookImportedFunction("WS2_32.dll", "send", 19, hook_send) ;
		   HookImportedFunction("WS2_32.dll", "closesocket", 3, hook_closesocket) ;
		   HookImportedFunction("WS2_32.dll", "connect", 4, hook_connect) ;
		   HookImportedFunction("WS2_32.dll", "select", 18, hook_select);
		   //HookImportedFunction("WS2_32.dll", "socket", 23, hook_socket);

    return true;
}

/*
	Builds SAFEARRAY from data without copying
*/
SAFEARRAY BuildSafeArray(const void * data, int len)
{
	SAFEARRAY array;
	memset(&array, 0, sizeof(SAFEARRAY));

	array.cDims = 1;
	array.fFeatures = FADF_AUTO|FADF_FIXEDSIZE;
	array.cbElements = 1;
	array.cLocks = 0;
	array.pvData = (void*)data;

	SAFEARRAYBOUND bound;
	bound.cElements = len;
	bound.lLbound = 0;
	array.rgsabound[0] = bound;

	return array;
}

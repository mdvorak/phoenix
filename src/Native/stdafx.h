// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once
#pragma unmanaged

// Modify the following defines if you have to target a platform prior to the ones specified below.
// Refer to MSDN for the latest info on corresponding values for different platforms.
#ifndef WINVER				// Allow use of features specific to Windows 95 and Windows NT 4 or later.
#define WINVER 0x0500		// Change this to the appropriate value to target Windows 98 and Windows 2000 or later.
#endif

#ifndef _WIN32_WINNT		// Allow use of features specific to Windows NT 4 or later.
#define _WIN32_WINNT 0x0500		// Change this to the appropriate value to target Windows 98 and Windows 2000 or later.
#endif						

#ifndef _WIN32_WINDOWS		// Allow use of features specific to Windows 98 or later.
#define _WIN32_WINDOWS 0x0500 // Change this to the appropriate value to target Windows Me or later.
#endif

#ifndef _WIN32_IE			// Allow use of features specific to IE 4.0 or later.
#define _WIN32_IE 0x0400	// Change this to the appropriate value to target IE 5.0 or later.
#endif

#define WIN32_LEAN_AND_MEAN		// Exclude rarely-used stuff from Windows headers
// Windows Header Files:
#include <string.h>
#include <stdlib.h>
#include <windows.h>
#include <objbase.h>
#include <winsock2.h>
#include <mscoree.h>
#include <tchar.h>

#include "ciptr.h"

#pragma warning(push)
#pragma warning(disable: 4278)

#import "Mscorlib.tlb" raw_interfaces_only	
using namespace mscorlib;

#pragma warning(pop)

#define NET_EXPORT __declspec(dllexport) __cdecl

// Some helpful macros
#define UNKNOWN_INTERFACE() 	private: unsigned int m_cRef; public: virtual long __stdcall QueryInterface( REFIID riid, LPVOID* ppvObj ); virtual unsigned long __stdcall AddRef();  virtual unsigned long __stdcall Release();

#define UNKNOWN_IMPLEMENT(class)	unsigned long __stdcall class::AddRef() { return InterlockedIncrement( (long*)&m_cRef ); } \
									unsigned long __stdcall class::Release() { unsigned long cRef = InterlockedDecrement( (long*)&m_cRef ); if( cRef == 0 ) { delete this; return 0; } else return m_cRef; }

#define UNKNOWN_QUERYINTERFACE(iid,interface)	if(riid==iid) { if(ppvObj) { *ppvObj= (interface*)this; AddRef(); } return S_OK; }
#define UNKNOWN_BEGIN_NO_UNKNOWN(class)	UNKNOWN_IMPLEMENT(class) \
								long __stdcall class::QueryInterface(REFIID riid,LPVOID* ppvObj) {
#define UNKNOWN_BEGIN(class)	UNKNOWN_BEGIN_NO_UNKNOWN(class) \
								UNKNOWN_QUERYINTERFACE(IID_IUnknown,IUnknown)
#define UNKNOWN_END() 			return E_NOINTERFACE; }
#define UNKNOWN_INIT() m_cRef(0)
#define UNKNOWN_QUERYUNKNOWN(interface)	if(riid==IID_IUnknown) { if(ppvObj) { *ppvObj= (IUnknown*)(interface*)this; AddRef(); } return S_OK; }


#ifndef NOT
#define NOT(var) ((var)<0)
#endif
#ifndef SAFE_FREE
#define SAFE_FREE(p) free(p); (p)=NULL;
#endif
#ifndef SAFE_DELETE
#define SAFE_DELETE(p) delete (p); (p)=NULL;
#endif
#ifndef SAFE_RELEASE
#define SAFE_RELEASE(p) if(p) (p)->Release(); (p)=NULL;
#endif

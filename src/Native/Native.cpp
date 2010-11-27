//
// This code is mostly copied from UOInjection written by mamaich
//

#pragma warning(disable: 4278)

#include "stdafx.h"
#include "SocksHook.h"
#include "WndProcHook.h"
#include "ComInterop.h"
#include "WinSock.h"
#include "FpsLimiter.h"
#include "UltimaDirHook.h"
#include "ImportHook.h"


static CIPtr<ICorRuntimeHost> spRuntimeHost;
CIPtr<IComObject> comObject;

static char OepBytes[5];	// 5 bytes at client's original entry point overwritten by hook
static DWORD OEP=0;			// client's original entry point address

HINSTANCE DllBase;


extern "C" void __declspec(dllexport) __load();

DECLSPEC_NORETURN VOID WINAPI HookExitProcess(UINT uExitCode);
LONG __stdcall MyUnhandledExceptionFilter(struct _EXCEPTION_POINTERS *ExceptionInfo);

void __declspec(naked) EntryPointHook()
{
// This function is called just before client starts its work,
// but after Windows had finished application initialization.
// So we can safely create all needed windows, hooks, load plugins, etc.
	__load();
	__asm jmp dword ptr [OEP];
}

#pragma warning(disable:4311)
#pragma warning(disable:4312)

BOOL APIENTRY DllMain( HINSTANCE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		DllBase = hModule;
		DisableThreadLibraryCalls(hModule);
		{
			// Windows does not allow to load DLLs & create threads in DllMain.
			// So I hook client's entry point & perform initialization
			// in that hook.

			DWORD image_base = (DWORD)GetModuleHandle(NULL);
			IMAGE_DOS_HEADER *idh = (IMAGE_DOS_HEADER *)image_base;
			IMAGE_FILE_HEADER *ifh = (IMAGE_FILE_HEADER *)(image_base +
				idh->e_lfanew + sizeof(DWORD));
			IMAGE_OPTIONAL_HEADER *ioh = (IMAGE_OPTIONAL_HEADER *)((DWORD)(ifh) +
				sizeof(IMAGE_FILE_HEADER));
			OEP = ioh->AddressOfEntryPoint + image_base;

			memcpy(OepBytes,(void*)OEP,5);	// remember bytes overwritten by my hook

		#include <pshpack1.h>	// select 1-byte structure packing
			struct NewOepBytes
			{
				BYTE  JMP;
				DWORD Addr;
			} Patch = {0xe9, (DWORD)EntryPointHook - (OEP+5)};
		#include <poppack.h>

			WriteProcessMemory(GetCurrentProcess(),(void*)OEP,
				&Patch,sizeof(Patch),0);
		}
		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
		break;

	case DLL_PROCESS_DETACH:
		break;
	}
    return TRUE;
}

/*
#pragma managed

#using "System.Windows.Forms.dll"

using namespace System;
using namespace System::Threading;

ref class Test
{
public:
	Test()
	{
		ThreadPool::QueueUserWorkItem(gcnew System::Threading::WaitCallback(Proc));
	}

	static void Proc(Object^ unused)
	{
		System::Windows::Forms::MessageBox::Show("Test");
	}
};

void RunTest()
{
	Test t;
}

#pragma unmanaged
*/
extern "C" void __declspec(dllexport) __load()
{
	// restore bytes at OEP overwritten by hook
	WriteProcessMemory(GetCurrentProcess(),(void*)OEP,
		&OepBytes,sizeof(OepBytes),0);

	SetUnhandledExceptionFilter(MyUnhandledExceptionFilter);

    WORD FPUcw=0x27f;
    __asm {
		fstcw [FPUcw]
	}

	//RunTest();

	try
	{
		char phoenixDir[1024];

		char* p = strstr(GetCommandLine(), "-phoenixdir:\"") + strlen("-phoenixdir:\"");
		strcpy_s(phoenixDir, p);

		p = strchr(phoenixDir, '\"');
		*p = '\0';

		if(*(p - 1) != '\\')
		{
			*p++ = '\\';
			*p = '\0';
		}

		char phoenixFile[1024];
		strcpy_s(phoenixFile, phoenixDir);
		strcat_s(phoenixFile, "Phoenix.dll");

		char configFileA[1024];
		strcpy_s(configFileA, phoenixDir);
		strcat_s(configFileA, "Phoenix.dll.config");

		wchar_t configFileW[1024];
		mbstowcs_s(NULL, configFileW, configFileA, 1024);

		// First try to load Runtime
		HRESULT hr = CorBindToCurrentRuntime(
										configFileW,
										CLSID_CorRuntimeHost,
										IID_ICorRuntimeHost,
										(void**)&spRuntimeHost
										);

		if (FAILED(hr)) throw "Unable to bind runtime.";

		// Start runtime
		hr = spRuntimeHost->Start();
		if (FAILED(hr)) throw "Unable to start runtime.";

		CIPtr<IUnknown> pUnk;
		//Retrieve the IUnknown for AppDomainSetup
		hr = spRuntimeHost->CreateDomainSetup(&pUnk);
		if (FAILED(hr)) throw "Unable to create AppDomainSetup.";

		CIPtr<IAppDomainSetup> pAppDomainSetup;
		hr = pUnk->QueryInterface(&pAppDomainSetup);
		if (FAILED(hr)) throw "Unable to bind IAppDomainSetup.";
		pUnk.Release();

		pAppDomainSetup->put_ApplicationName(_bstr_t("Phoenix"));
		pAppDomainSetup->put_ApplicationBase(_bstr_t(phoenixDir));

		// Create domain using setup
		hr = spRuntimeHost->CreateDomainEx(L"Phoenix", pAppDomainSetup, NULL, &pUnk);
		if (FAILED(hr)) throw "Unable to create AppDomain.";

		CIPtr<_AppDomain> spAppDomain;

		hr = pUnk->QueryInterface(&spAppDomain);
		if (FAILED(hr)) throw "Unable to bind _AppDomain.";
		pUnk.Release();

		//Creates an instance of the type specified in the Assembly
		CIPtr<_ObjectHandle> spObjectHandle;
		hr = spAppDomain->CreateInstanceFrom(_bstr_t(phoenixFile), _bstr_t("ComInterop.ComObject"), &spObjectHandle);
		if (FAILED(hr)) throw "Unable to create ComObject.";

		VARIANT VntUnwrapped;
		hr = spObjectHandle->Unwrap(&VntUnwrapped);
		if (FAILED(hr)) throw "Unable to unwrap ComObject.";

		if (VntUnwrapped.vt != VT_DISPATCH)	throw "Unwrapped interface is not IDispatch.";

		hr = VntUnwrapped.pdispVal->QueryInterface(__uuidof(IComObject), (void**)&comObject);
		if (FAILED(hr)) throw "Unable to query IComObject interface.";

		// Init winsock wrapper
		WinSock* pWrapper = new WinSock();
		if(!pWrapper) throw "WinSock wrapper creation failed.";

		CIPtr<IWinSock> pIWrapper; 
		hr = pWrapper->QueryInterface(__uuidof(IWinSock), (void**)&pIWrapper);
		if (FAILED(hr)) throw "Unable to query IWinSock interface.";


		// Hook import table
		bool hookSuccess = true;
		hookSuccess &= HookWinsockFunctions();
		hookSuccess &= HookClientWindowProc();
		hookSuccess &= HookUltimaDir();
		hookSuccess &= HookImportedFunction("kernel32.dll", "ExitProcess", 0, HookExitProcess) != NULL;
		hookSuccess &= InitFrameLimiter();

		//if(!hookSuccess) throw "Unable to hook import table.";

		// Init COM object
		hr = comObject->Init(pIWrapper);
		if (FAILED(hr)) throw "ComObject.Init() call failed.";
	}
	catch(const char* msg)
	{
		char Text[1024];

		strcpy_s(Text, "Error in Native.dll during initializing Phoenix.\nMessage: ");
		strcat_s(Text, msg);

		MessageBox(NULL, Text, "Fatal Error", MB_OK|MB_ICONERROR);
		ExitProcess(0xFF);
	}
	catch(...)
	{
		MessageBox(NULL, "Error in Native.dll during initializing Phoenix.", "Fatal Error", MB_OK|MB_ICONERROR);
		ExitProcess(0xFF);
	}

    __asm {
        fldcw [FPUcw]
    }
}

DECLSPEC_NORETURN VOID WINAPI HookExitProcess(UINT uExitCode)
{
	if (comObject != NULL)
	{
		comObject->OnExitProcess(uExitCode);
		comObject.Release();
	}

	if (spRuntimeHost != NULL)
	{
		spRuntimeHost->Stop();
		spRuntimeHost.Release();
	}

	ExitProcess(uExitCode);
}

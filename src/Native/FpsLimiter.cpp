#include "stdafx.h"
#include "Mmsystem.h"
#include "ImportHook.h"
#include "ComInterop.h"
#include "FpsLimiter.h"

DWORD g_FrameLimit = 0;
static DWORD lastFrame = 0;

void FrameLimiter()
{
	DWORD time = timeGetTime();
	DWORD interval = time - lastFrame;

	if(g_FrameLimit >= 10)
	{
		DWORD desiredPause = (1000 / g_FrameLimit);

		if(interval < desiredPause)
		{
			Sleep(desiredPause - interval);
		}
	}

	lastFrame = time;
}

BOOL WINAPI HookPeekMessageA( 
    __out LPMSG lpMsg,
    __in_opt HWND hWnd,
    __in UINT wMsgFilterMin,
    __in UINT wMsgFilterMax,
    __in UINT wRemoveMsg)
{
	FrameLimiter();

	return PeekMessageA(lpMsg, hWnd, wMsgFilterMin, wMsgFilterMax, wRemoveMsg);
}

bool InitFrameLimiter()
{
	lastFrame = timeGetTime();
	return HookImportedFunction("user32.dll", "PeekMessageA", 0, HookPeekMessageA) != NULL;
}

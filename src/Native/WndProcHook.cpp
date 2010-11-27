#include "stdafx.h"
#include "ImportHook.h"
#include "WndProcHook.h"
#include "ComInterop.h"

#pragma warning(disable: 4311)

#define WM_SETFPS 0x3FFF

extern DWORD g_FrameLimit;
static HWND ClientHWND = 0;
static LPARAM keyToIgnore = 0;

static bool overrideCursor = false;
static POINT cursorPosition;


WNDPROC OldWndProc=0;

#define REPEAT_COUNT(lparam) ((UINT)((lparam)&0xFF))
#define PREVIOUS_KEY_STATE(lparam) ((BOOL)((lparam>>30)&0x1))

#define KEYS_Alt (262144)
#define KEYS_Shift (65536)
#define KEYS_Control (131072)

UINT GetModifierKeys()
{
	UINT modifiers = 0;

	if(GetKeyState(VK_MENU) & 0xFF00)
		modifiers |= KEYS_Alt;

	if(GetKeyState(VK_CONTROL) & 0xFF00)
		modifiers |= KEYS_Control;

	if(GetKeyState(VK_SHIFT) & 0xFF00)
		modifiers |= KEYS_Shift;

	return modifiers;
}

bool HookWinsockFunctions();

LRESULT CALLBACK HookWndProc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
{
	ClientHWND=hwnd;
	
	VARIANT_BOOL b;
	static int textChanging = 0;
	UINT mouseCode = 0;
	
	switch(msg)
	{
	case WM_CREATE:
		comObject->OnWindowCreated((long)hwnd);
		break;

	case WM_SYSKEYDOWN:
	case WM_SYSDEADCHAR:
	case WM_KEYDOWN:
		comObject->OnKeyDown(((UINT)wparam) | GetModifierKeys(), REPEAT_COUNT(lparam), PREVIOUS_KEY_STATE(lparam), &b);
		if(b != 0)
		{
			keyToIgnore = lparam;
			return 0;
		}
		break;

	case WM_CHAR:
	case WM_SYSCHAR:
		if(keyToIgnore && keyToIgnore == lparam)
		{
			keyToIgnore = 0;
			return 0;
		}
		break;

	case WM_MBUTTONDOWN:
		switch(wparam) {
			case MK_MBUTTON:
				mouseCode = VK_MBUTTON;
				break;
			case MK_XBUTTON1:
				mouseCode = VK_XBUTTON2;
				break;
			case MK_XBUTTON2:
				mouseCode = VK_XBUTTON2;
				break;
		}
		if (mouseCode > 0) {
			comObject->OnKeyDown(mouseCode | GetModifierKeys(), 0, FALSE, &b);
			if(b != 0)
			{
				return 0;
			}
		}
		break;

	case WM_MOUSEWHEEL:
		comObject->OnMouseWheel((short)GET_WHEEL_DELTA_WPARAM(wparam), &b);
		if(b != 0)
		{
			return 0;
		}
		break;

	case WM_APPCOMMAND:
		mouseCode = GET_APPCOMMAND_LPARAM(lparam);
		comObject->OnAppCommand((USHORT) mouseCode, &b);
		if(b != 0)
		{
			return 0;
		}
		break;

	case WM_SETFOCUS:
		comObject->OnFocusChanged(TRUE);
		break;

	case WM_KILLFOCUS:
		comObject->OnFocusChanged(FALSE);
		break;

	case WM_CLOSE:
		ClientHWND = 0;
		break;

	case WM_SETFPS:
		g_FrameLimit = (DWORD)wparam;
		break;

	case WM_SETTEXT:
		{
			WCHAR* pwText = (WCHAR*)lparam;

			if(pwText != NULL)
			{
				WCHAR Text[1024];
				wcscpy_s(Text, pwText);

				int change = ++textChanging;

				comObject->OnTextChanged(bstr_t(Text));

				if(textChanging > change){
					if(change == 1) // This is first call.
						textChanging = 0;

					return 0;
				}

				if(change == 1) // This is first call.
					textChanging = 0;
			}
		}
		break;
	};
	return OldWndProc(hwnd,msg,wparam,lparam);
}


ATOM WINAPI HookRegisterClassA(
    WNDCLASSA *lpWndClass 	// address of structure with class data
   )
{
	OldWndProc = lpWndClass->lpfnWndProc;
	lpWndClass->lpfnWndProc = HookWndProc;
	return RegisterClassA(lpWndClass);
}

ATOM WINAPI HookRegisterClassW(
    WNDCLASSW *lpWndClass 	// address of structure with class data
   )
{
	OldWndProc = lpWndClass->lpfnWndProc;
	lpWndClass->lpfnWndProc = HookWndProc;
	return RegisterClassW(lpWndClass);
}

BOOL WINAPI Hook_GetCursorPos(LPPOINT lpPoint)
{
    if (!overrideCursor) {
        return GetCursorPos(lpPoint);
    }
    else {
        *lpPoint = cursorPosition;
        ClientToScreen(ClientHWND, lpPoint);
        return true;
    }
}

bool HookClientWindowProc()
{
	bool ret = NULL!=HookImportedFunction("user32.dll","RegisterClassA",0,
		HookRegisterClassA);
	ret |= (NULL!=HookImportedFunction("user32.dll","RegisterClassW",0,
		HookRegisterClassW));
    
    HookImportedFunction("user32.dll", "GetCursorPos", 0, Hook_GetCursorPos);

	return ret;
}

HWND GetClientWindow()
{
	return ClientHWND;
}


void SimulateCursorPos(int x, int y)
{
    cursorPosition.x = x;
    cursorPosition.y = y;
}

void OverrideCursor()
{
    GetCursorPos(&cursorPosition);
    ScreenToClient(ClientHWND, &cursorPosition);
    overrideCursor = true;
}

void ReleaseCursorOverride()
{
    overrideCursor = false;
}

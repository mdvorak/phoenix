#include "stdafx.h"
#include "ImportHook.h"
#include "ComInterop.h"
#include "UltimaDirHook.h"

extern CIPtr<IComObject> comObject;

char g_szClientDir[1024];
bool g_bClientDirRead = false;

LONG WINAPI HookRegQueryValueExA (
    HKEY hKey,
    LPCSTR lpValueName,
    LPDWORD lpReserved,
    LPDWORD lpType,
    LPBYTE lpData,
    LPDWORD lpcbData
    )
{
	if(!g_bClientDirRead)
	{
		char* pDir = strstr(GetCommandLine(), "-ultimadir:\"") + strlen("-ultimadir:\"");
		strcpy_s(g_szClientDir, pDir);

		char* pathEnd = strchr(g_szClientDir, '\"');
		*pathEnd = '\0';

		if(*(pathEnd - 1) != '\\')
		{
			*pathEnd++ = '\\';
			*pathEnd = '\0';
		}

		g_bClientDirRead = true;
	}

	char Value[MAX_PATH];

	if(strcmp(lpValueName, "ExePath") == 0)
	{
		strcpy_s(Value, g_szClientDir);
		strcat_s(Value, "uotd.exe");
	}
	else if(strcmp(lpValueName, "InstCDPath") == 0)
	{
		strcpy_s(Value, g_szClientDir);
	}
	else if(strcmp(lpValueName, "PatchExePath") == 0)
	{
		strcpy_s(Value, g_szClientDir);
		strcat_s(Value, "uopatch.exe");
	}
	else if(strcmp(lpValueName, "StartExePath") == 0)
	{
		strcpy_s(Value, g_szClientDir);
		strcat_s(Value, "uo.exe");
	}
	else
	{
		return RegQueryValueExA(hKey, lpValueName, lpReserved, lpType, lpData, lpcbData);
	}

	size_t len = strlen(Value) + 1;
	
	if(lpData == NULL)
	{
		*lpcbData = (DWORD)len;
		return ERROR_SUCCESS;
	}
	else if(*lpcbData >= len)
	{
		memcpy(lpData, Value, len);
		*lpcbData = (DWORD)len;
		return ERROR_SUCCESS;
	}
	else
	{
		return ERROR_MORE_DATA;
	}
}

HANDLE WINAPI HookCreateFileA(
    LPCSTR lpFileName,
    DWORD dwDesiredAccess,
    DWORD dwShareMode,
    LPSECURITY_ATTRIBUTES lpSecurityAttributes,
    DWORD dwCreationDisposition,
    DWORD dwFlagsAndAttributes,
    HANDLE hTemplateFile
    )
{
	static bool check = true;

	// I tried to avoid desktop corruption under old clients..
	// It needs some more tests to find out if it really works
	if(check && strstr(lpFileName, "Desktop") != NULL)
	{
		VARIANT_BOOL ok;
		comObject->HasBeenCharListSent(&ok);

		if(ok == 0)
		{
			return NULL;
		}
		else
		{
			check = false;
		}
	}

	return CreateFileA(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
}

bool HookUltimaDir()
{
	return HookImportedFunction("kernel32.dll", "CreateFileA", 0, HookCreateFileA) &&
		   HookImportedFunction("advapi32.dll", "RegQueryValueExA", 0, HookRegQueryValueExA);
}



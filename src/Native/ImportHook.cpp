////////////////////////////////////////////////////////////////////////////////
//
// Copyright (C) 2002 mamaich
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
////////////////////////////////////////////////////////////////////////////////
//
//  Module Name:
// 
//     ImportHook.cpp
// 
//  Abstract:
// 
//     Client import hooking primitives
//     
//  Author:
// 
//     mamaich
//
//  Revision history:
//
//  [2/17/2002]	- source code cleanup
//
////////////////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include <windows.h>
#include "ImportHook.h"

#pragma unmanaged
#pragma warning(disable: 4311 4312)

__declspec(dllexport) void* __cdecl HookImportedFunction(const char *Dll, const char *FuncName,
										 int Ordinal, void *Function)
{
    DWORD oldProtect;
	void *PrevValue=0;

	DWORD image_base = (DWORD)GetModuleHandle(NULL);
    IMAGE_DOS_HEADER *idh = (IMAGE_DOS_HEADER *)image_base;
    IMAGE_FILE_HEADER *ifh = (IMAGE_FILE_HEADER *)(image_base +
        idh->e_lfanew + sizeof(DWORD));
    IMAGE_OPTIONAL_HEADER *ioh = (IMAGE_OPTIONAL_HEADER *)((DWORD)(ifh) +
        sizeof(IMAGE_FILE_HEADER));
    IMAGE_IMPORT_DESCRIPTOR *iid = (IMAGE_IMPORT_DESCRIPTOR *)(image_base +
        ioh->DataDirectory[IMAGE_DIRECTORY_ENTRY_IMPORT].VirtualAddress);

    VirtualProtect((LPVOID)(image_base +
        ioh->DataDirectory[IMAGE_DIRECTORY_ENTRY_IAT].VirtualAddress),
        ioh->DataDirectory[IMAGE_DIRECTORY_ENTRY_IAT].Size, PAGE_READWRITE,
        &oldProtect);

    while(iid->Name)
    {
        if(_stricmp(Dll, (char *)(image_base + iid->Name)) == 0)
        {
            //trace_printf("Found descriptor: %s\n", dhook->name);
            IMAGE_THUNK_DATA * pThunk = (IMAGE_THUNK_DATA *)
                ((DWORD)iid->OriginalFirstThunk + image_base);
            IMAGE_THUNK_DATA * pThunk2 = (IMAGE_THUNK_DATA *)
                ((DWORD)iid->FirstThunk + image_base);
            while(pThunk->u1.AddressOfData)
            {
                char * name = 0;
                int ordinal;
                // Imported by ordinal only:
                if(pThunk->u1.Ordinal & 0x80000000)
                    ordinal = pThunk->u1.Ordinal & 0xffff;
                else    // Imported by name, with ordinal hint
                {
                    IMAGE_IMPORT_BY_NAME * pname = (IMAGE_IMPORT_BY_NAME *)
                        ((DWORD)pThunk->u1.AddressOfData + image_base);
                    ordinal = pname->Hint;
                    name = (char *)pname->Name;
                }

                if(name != 0 && FuncName && strcmp(name, FuncName) == 0)
                {
                    //trace_printf("Found entry name: %s\n", ehook->name);
					PrevValue = (void*)pThunk2->u1.Function;
#if _MFC_VER == 0x0600
                    pThunk2->u1.Function = (DWORD*)Function;
#else
                    pThunk2->u1.Function = (DWORD)Function;
#endif
                }
                else if(ordinal == Ordinal)
                {
                    //trace_printf("Found entry ordinal: %s\n", ehook->name);
					PrevValue = (void*)pThunk2->u1.Function;
#if _MFC_VER == 0x0600
                    pThunk2->u1.Function = (DWORD*)Function;
#else
                    pThunk2->u1.Function = (DWORD)Function;
#endif
                }

				pThunk++;
                pThunk2++;
            }
		}
        iid++;
    }
	return PrevValue;
}


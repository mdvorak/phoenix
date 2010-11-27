////////////////////////////////////////////////////////////////////////////////
//
// debug.cpp
//
// Copyright (C) 2001 mamaich
// In 2005 edited by Mikee (I removed some parts i don't need)
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

#include "stdafx.h"
#include "ComInterop.h"

extern CIPtr<IComObject> comObject;

#pragma warning( disable : 4312 )

#define PEEKB(addr) (*(char*)(addr))

void disasm(BYTE* iptr0, DWORD* osizeptr);

//
// This function is a hack. It tries to ignore an unhandled exception (i.e. crash),
// but if there are too many crashes it will force client to terminate...
//
LONG __stdcall MyUnhandledExceptionFilter(
  struct _EXCEPTION_POINTERS *ExceptionInfo)
{
	static int TotalErrorCount=0;
	static int ErrorCount=0;
	static DWORD LastErrorTime=GetTickCount();

	ErrorCount++; TotalErrorCount++;
	if(ExceptionInfo->ExceptionRecord)
	{
		char ErrString[1024];
		sprintf_s(ErrString, "Unhandled exception%3d: 0x%08X at %08X",TotalErrorCount,
		ExceptionInfo->ExceptionRecord->ExceptionCode,
		ExceptionInfo->ExceptionRecord->ExceptionAddress);

		comObject->WriteToLog(_bstr_t(ErrString));

	if(ErrorCount>100 && GetTickCount()-LastErrorTime<5000)
	// crash when too many errors happen too often
	{
		if(MessageBox(0,"UO client performed an unrecoverable invalid operation.\nContinue?",0,MB_ICONSTOP|MB_YESNO)==IDYES)
		 ExitProcess(1);
	}

	if(GetTickCount()-LastErrorTime>5000) // reset error count every 5 sec
	{
		ErrorCount=0;
		LastErrorTime=GetTickCount();
	}

	int B=255&PEEKB(ExceptionInfo->ContextRecord->Eip);	// this can cause another crash...

    if(B>=0xD9 && B<=0xDF)	// For some hell float exceptions are unmasked
    {	// - ignore all floating point exceptions and clear coprocessor err state
		// these constants are coprocessor opcodes
		ExceptionInfo->ContextRecord->FloatSave.ControlWord=0x27f;
        ExceptionInfo->ContextRecord->FloatSave.StatusWord&=~0xff;
        return EXCEPTION_CONTINUE_EXECUTION;
    }

// Determine the length of a faulting instruction and step over it
	DWORD Len=0;
	disasm((BYTE*)ExceptionInfo->ContextRecord->Eip,&Len);

	ExceptionInfo->ContextRecord->Eip+=Len;

	if(Len==0)
	    return EXCEPTION_CONTINUE_SEARCH;

	}
    return EXCEPTION_CONTINUE_EXECUTION;
}

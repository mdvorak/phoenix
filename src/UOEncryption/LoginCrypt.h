/******************************************************************************\
* 
* 
*  Modified: 2004 Daniel 'Necr0Potenc3' Cavalcanti
*
*  Copyright (C) 1999 Bruno 'Beosil' Heidelberger
* 
* 
*  This program is free software; you can redistribute it and/or modify
*  it under the terms of the GNU General Public License as published by
*  the Free Software Foundation; either version 2 of the License, or
*  (at your option) any later version.
* 
*  This program is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU General Public License for more details.
* 
* 	n0p3:
* 	Aug 8th, 2004  -- Code clean up. Part of this code comes from Sphere's
*   crypto code. Some from Injection. I think it originally comes from Beosil...
* 
\******************************************************************************/


#ifndef _LCRYPT_H_INCLUDED
#define _LCRYPT_H_INCLUDED

typedef struct tagLoginCryptInfo
{
	/* public, must be declared */
    unsigned int pseed;
	unsigned int k1, k2;
	/* private, dynamic vars used by the crypt code, each encrypted byte changes them */
	unsigned int m_key[2]; /* 0 - m_CryptMaskLo 1 - m_CryptMaskHi */
	unsigned int m_k1, m_k2; /* 1 - m_MasterHi  2 - m_MasterLo */
}LoginCryptObj;


/* functions, Init should always be called first */
void LoginCryptInit(LoginCryptObj *obj);
void LoginCryptEncrypt(LoginCryptObj *obj, unsigned char *in, unsigned char *out, int len);

#endif

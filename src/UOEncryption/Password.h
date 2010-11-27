/******************************************************************************\
* 
* 
*  Modified: 2003, 2005 Daniel 'Necr0Potenc3' Cavalcanti
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
* 	This was actually written in 27/07/2003, but today (19/02/2005) I'm adding
*  it to the UOEncryption library, because this is in fact a part of UO's crypt
* 
\******************************************************************************/


#ifndef _PWCRYPT_H_INCLUDED
#define _PWCRYPT_H_INCLUDED

void PWEncrypt(unsigned char *in, unsigned char *out, int len);
void PWDecrypt(unsigned char *in, unsigned char *out, int len);

#endif

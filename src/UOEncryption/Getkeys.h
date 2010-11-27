/******************************************************************************\
* 
* 
*  Modified: 2004 Daniel 'Necr0Potenc3' Cavalcanti
*
*  Copyright (C) 2002 mamaich
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
* 	Somewhere in 2004  -- got the code from UOInjection, cleaned it up.
* 
\******************************************************************************/


#ifndef _GETKEYS_H_INCLUDED
#define _GETKEYS_H_INCLUDED

int CalculateKeys(unsigned char *Plaintext, unsigned char *Ciphertext, unsigned int *Seed, unsigned int *Key1, unsigned int *Key2);

#endif

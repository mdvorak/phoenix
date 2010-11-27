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

#include "LoginCrypt.h"

/*
later to be discovered: MD5 ;)
2.0.4 - 1e4a60b0
1st building function (parameter= seed)
00425DFA

16byte array building function (parm1 == class's 16byte array, parm2 == byte array from 1st function)
00425E2F

ps: the seeds are passed in little endian

seed: 0100007F
05 92 66 23 67 14 e3 62 dc 60 8c d6 fe 7c 25 69

seed: FFFFFFFF
a9 d5 7d a4 3e 0c 22 da de 15 e9 92 dd 99 98 4d

seed: 00000000
d4 a9 9f 4c 3c 1b 55 94 df a3 73 52 e4 54 d3 67

seed: 00000001
7d 07 d7 1e 17 c1 bb c7 bb d1 00 ff dc 34 e0 da

seed: 00000002
7c 3c 8b 55 74 d1 3f a5 11 e0 7d e0 6e 75 51 48

seed: 00000003
19 19 a1 d7 77 db c8 79 48 97 d2 d1 b1 f7 ce 0a

seed: 10000000
69 54 83 e1 b4 45 eb ad 37 dd 1d f8 12 10 6c 01

seed: 20000000
30 fc 3b 42 38 51 2d bd bb 41 49 54 f2 05 d1 6c

seed: 30000000
d5 55 8d 24 77 34 29 84 d7 bc 41 53 3e 12 01 68
*/

/* load long from C into LL with bytes swapped */
#define N2L(C, LL) \
    LL  = ((C&0xff000000))>>24,\
    LL |= ((C&0x00ff0000))>>8,\
    LL |= ((C&0x0000ff00))<<8,\
    LL |= ((C&0x000000ff)<<24)


void LoginCryptInit(LoginCryptObj *obj)
{
    unsigned int seed;
	N2L(obj->pseed, seed);

    obj->m_key[0] =
            (((~seed) ^ 0x00001357) << 16)
        |   ((seed ^ 0xffffaaaa) & 0x0000ffff);
    obj->m_key[1] =
            ((seed ^ 0x43210000) >> 16)
        |   (((~seed) ^ 0xabcdffff) & 0xffff0000);

    obj->m_k1 = obj->k1;
    obj->m_k2 = obj->k2;
    
    return;
}


void LoginCryptEncrypt(LoginCryptObj *obj, unsigned char *in, unsigned char *out, int len)
{
	int i = 0;
	unsigned int table0 = 0, table1 = 0;

    for(i = 0; i < len; i++)
    {
        out[i] = in[i] ^ (unsigned char)(obj->m_key[0]);

        table0 = obj->m_key[0];
        table1 = obj->m_key[1];

    	/* table1 is updated instead of being directly added (like in infidel's code) */
    	obj->m_key[0] = ((table0 >> 1) | (table1 << 31)) ^ obj->m_k2;
    	table1 =		(((table1 >> 1) | (table0 << 31)) ^ obj->m_k1);
    	obj->m_key[1] = ((table1 >> 1) | (table0 << 31)) ^ obj->m_k1;
    }
    
    return;
}

/* version == 1.25.36 */
void RareLoginCryptEncrypt(LoginCryptObj *obj, unsigned char *in, unsigned char *out, int len)
{
	int i = 0;
	unsigned int table0 = 0, table1 = 0;

    for(i = 0; i < len; i++)
    {
        out[i] = in[i] ^ (unsigned char)(obj->m_key[0]);

        table0 = obj->m_key[0];
        table1 = obj->m_key[1];

		/*  ps: could it be any uglier? */
		obj->m_key[1] =
    					(obj->m_k1 >> ((5 * table1 * table1) & 0xff))
						+ (table1 * obj->k1)
    					+ (table0 * table0 * 0x35ce9581)
    					+ 0x07afcc37;
    	obj->m_key[0] =
    					(obj->m_k2 >> ((3 * table0 * table0) & 0xff))
    					+ (table0 * obj->m_k2)
    					- (obj->m_key[1] * obj->m_key[1] * 0x4c3a1353)
    					+ 0x16ef783f;
    }
    
    return;
}

/* version <= 1.25.35 */
void OldLoginCryptEncrypt(LoginCryptObj *obj, unsigned char *in, unsigned char *out, int len)
{
	int i = 0;
	unsigned int table0 = 0, table1 = 0;

    for(i = 0; i < len; i++)
    {
        out[i] = in[i] ^ (unsigned char)(obj->m_key[0]);

        table0 = obj->m_key[0];
        table1 = obj->m_key[1];

		/* simple rotary crypt */
		obj->m_key[0] = ((table0 >> 1) | (table1 << 31)) ^ obj->m_k2;
    	obj->m_key[1] = ((table1 >> 1) | (table0 << 31)) ^ obj->m_k1;
    }
    
    return;
}

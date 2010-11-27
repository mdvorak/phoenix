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


/* thanks to RElf (spam4relf@chat.ru) */
#include <stdio.h>
#include <string.h>
#include "Getkeys.h"


/* Load long from address C into LL with bytes swapped. */
#define N2L(C, LL) \
    LL  = ((unsigned int)(*((C)++))) << 24, \
    LL |= ((unsigned int)(*((C)++))) << 16, \
    LL |= ((unsigned int)(*((C)++))) << 8, \
    LL |= ((unsigned int)(*((C)++)))

/*
* we need less than 32 bytes to get k0, but we need at least 40 bytes to get k1...
* n0p3: size is defined as 61 because both login packets (0x80 and 0xCF)
* have this structure in common:
* byte[1] (CMD), byte[30] login, byte[30] pwd
* everything after the pwd changes
*/
#define SIZE 61

static unsigned int key0[SIZE+1];
static unsigned int key1[SIZE+1];
static unsigned int k0=0;
static unsigned int k1=0;
static unsigned int k30=0;	/* would be (L_{31}(k0) shr 1) ^ L_{30}(k0). */

static void init(unsigned char * pseed)
{
    unsigned int seed;
    N2L(pseed, seed);

    key0[0] =
            (((~seed) ^ 0x00001357) << 16)
        |   ((seed ^ 0xffffaaaa) & 0x0000ffff);
    key1[0] =
            ((seed ^ 0x43210000) >> 16)
        |   (((~seed) ^ 0xabcdffff) & 0xffff0000);
}

static void crypt(unsigned char *buff, int len)
{
	int i = 0;
	int m_key0 = key0[0];
	int m_key1 = key1[0];
	unsigned int table0 = 0, table1 = 0;

    for(i = 0; i < len; i++)
    {
        buff[i] = buff[i] ^ (unsigned char)m_key0;

        table0 = m_key0;
        table1 = m_key1;

		m_key0 = ((table0 >> 1) | (table1 << 31)) ^ k1;
    	table1 = ((table1 >> 1) | (table0 << 31));
    	m_key1 = ((table1 >> 1) | (table0 << 31)) ^ k30;
    }

	return;
}

/* low a bits of b */
#define L(a,b) ((b) & (((__int64)1<<((a)+1))-1))
/* a-st bit of b */
#define B(a,b) (((b) >> (a)) & 1)


/*
* everything goes from these formulas:
* key1[n+1] = ((( ((key1[n]>>1) ^ (key0[n]<<31)) ^ k0)>>1 )) ^ (key0[n]<<31) ) ^ k0;
* key0[n+1] = ((key0[n]>>1) ^ (key1[n]<<31)) ^ k1;
*/
int CalculateKeys(unsigned char *Plaintext, unsigned char *Ciphertext, unsigned int *Seed, unsigned int *Key1, unsigned int *Key2)
{
	int i = 0, t = 0, n = 0;

	/* clean the vars */
	memset(key0, 0, sizeof(key0));
	memset(key1, 0, sizeof(key1));
	k0 = k1 = k30 = 0;

	/* fill key0[] array with known bits: */
	for(i = 0; i < SIZE; i++)
		key0[i] = Plaintext[i] ^ Ciphertext[i];

	/* calculate key0[0] & key1[0] */
	init((unsigned char*)Seed);

	/* Get first 8 bits of k1: */
	k1 = B(8, (key0[1] ^ ((key0[0]>>1) | (key1[0]<<31))));

	/*
	* use formula to calculate remaining bits of k1/key0[n]:
	* L_{t+1}(key0[n]) = ((L_t(key0[n+1])^L_t(k1)) shl 1) | B_0(key0[n]);
	*/
	for(t = 7; t < 32; t++)
	{
		for(n = 1; n < SIZE; n++)	
			key0[n] = L(t+1, ( ((L(t,key0[n+1]) ^ L(t,k1))<<1) ) | (B(0,key0[n])) );

		k1 = L(t+1, (key0[1] ^ ((key0[0]>>1) | (key1[0]<<31))) );
	}
	*Key2 = k1;

	/* CALCULATE k30 */
	/*
	* Calculate 1st bits of key1[n]:
	* B_0(key1[n]) = (key0[n+1]^k1) shr 31;
	*/
	k30 = 0;
	for(n = 1; n < SIZE; n++)
		key1[n] = B(31, key0[n+1] ^ k1 );

	/*
	* calculate remaining bits of key1:
	* B_t(key1[n+1]) = B_{t+2}(key1[n]) ^ B_t(k30)
	* B_{t+2}(key1[n]) = B_t(key1[n+1]) ^ B_t(k30)
	* B_t(k30) = B_{t+2}(key1[n]) ^ B_t(key1[n+1])
	*/
	k30 = 0;
	for(t = 0; t < 30; t += 2)	
	{
		k30 |= (B(t+2, key1[0]) ^ B(t, key1[1])) << t;

		/* calculate t+2 bits */
		for(n = 1; n < SIZE; n++)
			key1[n] |= (B(t, key1[n+1]) ^ B(t, k30)) << (t+2);
	}

	/*
	* B_31(key1[n+1])=B_30(key1[n+1]) 
	* key1[1]|=B(30,key1[1])<<31;
	*/

	/* get 31st bit of k. By the way, it is == bit 30 */
	k30 |= (B(30, key1[1]) ^ L(1,key0[0])) << 31;
	*Key1 = k30;

	/* encrypt and see if it worked */
	crypt(Plaintext, SIZE);
	if(memcmp(Ciphertext, Plaintext, SIZE) != 0)
		return -1; /* if not, oops */

	return 1;
}

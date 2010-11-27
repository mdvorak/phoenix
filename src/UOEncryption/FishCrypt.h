/******************************************************************************\
*
*
*  Modified: 2004 Daniel 'Necr0Potenc3' Cavalcanti
*  Modified: 2004 Roman 'Fallout' Ilichev
*  Modified: 2001 Luke 'Infidel' Dunstan
*
*  Old Info:
*  Author:  Beosil
*  E-Mail:  beosil@swileys.com
*  Version: 1.26.4
*  Date:    27. Jan. 2000
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
*   Jun 15th, 2004 -- this module is based on Infidel's and Mamaich's crypt.cpp
*   from Injection http://injection.sourceforge.net. Code clean up by Fallout.
*   C++ -> C conversion and more clean ups by Necr0Potenc3.
*
*   Greets to Beosil for being the first to break UO's encryption. Greets to
*   Infidel for keeping it updated. Greets to Mamaich for the Getkeys code.
*
\******************************************************************************/


#ifndef _FISHCRYPT_H_INCLUDED
#define _FISHCRYPT_H_INCLUDED


/* defines for twofish */
#define	p8(N)	P8x8[P_##N]
#define	RS_rem(x)		\
	{ unsigned char  b  = (unsigned char) (x >> 24);											 \
	  unsigned int g2 = ((b << 1) ^ ((b & 0x80) ? 0x14D : 0 )) & 0xFF;		 \
	  unsigned int g3 = ((b >> 1) & 0x7F) ^ ((b & 1) ? 0x14D >> 1 : 0 ) ^ g2 ; \
	  x = (x << 8) ^ (g3 << 24) ^ (g2 << 16) ^ (g3 << 8) ^ b;				 \
	}

#define	LFSR1(x) (((x)>>1)^(((x)&0x01)?0x169/2 : 0))
#define	LFSR2(x) (((x)>>2)^(((x)&0x02)?0x169/2:0)^(((x) & 0x01)?0x169/4:0))
#define	Mx_1(x) ((unsigned int)  (x))
#define	Mx_X(x) ((unsigned int) ((x)^LFSR2(x)))
#define	Mx_Y(x) ((unsigned int) ((x)^LFSR1(x)^LFSR2(x)))
#define	M00		Mul_1
#define	M01		Mul_Y
#define	M02		Mul_X
#define	M03		Mul_X
#define	M10		Mul_X
#define	M11		Mul_Y
#define	M12		Mul_Y
#define	M13		Mul_1
#define	M20		Mul_Y
#define	M21		Mul_X
#define	M22		Mul_1
#define	M23		Mul_Y
#define	M30		Mul_Y
#define	M31		Mul_1
#define	M32		Mul_Y
#define	M33		Mul_X
#define	Mul_1	Mx_1
#define	Mul_X	Mx_X
#define	Mul_Y	Mx_Y
#define	P_00	1
#define	P_01	0
#define	P_02	0
#define	P_03	(P_01^1)
#define	P_04	1
#define	P_10	0
#define	P_11	0
#define	P_12	1
#define	P_13	(P_11^1)
#define	P_14	0
#define	P_20	1
#define	P_21	1
#define	P_22	0
#define	P_23	(P_21^1)
#define	P_24	0
#define	P_30	0
#define	P_31	1
#define	P_32	1
#define	P_33	(P_31^1)
#define	P_34	1
#define	ROL(x,n) (((x) << ((n) & 0x1F)) | ((x) >> (32-((n) & 0x1F))))
#define	ROR(x,n) (((x) >> ((n) & 0x1F)) | ((x) << (32-((n) & 0x1F))))
#define		Bswap(x)			(x)
#define	_b(x,N)	(((unsigned char *)&x)[((N) & 3)^0])

#define N2L(C, LL)	LL  = ((unsigned int)(*((C)++))) << 24, \
			LL |= ((unsigned int)(*((C)++))) << 16, \
			LL |= ((unsigned int)(*((C)++))) << 8,	\
			LL |= ((unsigned int)(*((C)++)))

/* defines used by blowfish */
#define ROUND(LL, R, S, P) LL=(LL)^(P)^((S[(R)>>24]+S[0x0100+(((R)>>16)&0xff)])^S[0x0200+(((R)>>8)&0xff)])+S[0x0300+((R)&0xff)]

#define L2N(LL, C) *((C)++) = 	(unsigned char)(((LL) >> 24) & 0xff), *((C)++) = \
				(unsigned char)(((LL) >> 16) & 0xff), *((C)++) = \
				(unsigned char)(((LL) >> 8) & 0xff), *((C)++) = \
				(unsigned char)(((LL)) & 0xff)

/* general defines */
#define CRYPT_GAMEKEY_LENGTH    6
#define CRYPT_GAMEKEY_COUNT     25
#define CRYPT_GAMESEED_LENGTH   8
#define CRYPT_GAMESEED_COUNT    25
#define CRYPT_GAMETABLE_START   1
#define CRYPT_GAMETABLE_STEP    3
#define CRYPT_GAMETABLE_MODULO  11
#define CRYPT_GAMETABLE_TRIGGER 21036


/* BLOWFISH */
typedef struct tagBlowfishObject
{
	unsigned char seed[CRYPT_GAMESEED_LENGTH];
	int table_index; /* init: 0 */
	int stream_pos; /* init: 0 */
	int block_pos; /* init: 0 */
}BlowfishObj;

/* private/internal functions */
void BlowfishRawEncrypt(unsigned int *values, int table);
void BlowfishInitTables(BlowfishObj *Obj);

/* public/exported functions */
void BlowfishInit(BlowfishObj *Obj);
void BlowfishEncrypt(BlowfishObj *Obj, unsigned char *in, unsigned char *out, int len);
void BlowfishDecrypt(BlowfishObj *Obj, unsigned char *in, unsigned char *out, int len);


/* TWOFISH */
typedef struct tagkeyInstance
{
	unsigned char direction;
	int  keyLen;
	int numRounds;
	char keyMaterial[68];
	unsigned int keySig;
	unsigned int key32[8];
	unsigned int sboxKeys[4];
	unsigned int subKeys[40];
}keyInstance;

typedef struct tagcipherInstance
{
	unsigned char  mode;
	unsigned char  IV[16];
	unsigned int cipherSig;
	unsigned int iv32[4];
}cipherInstance;

typedef struct tagTwofishObject
{
	keyInstance ki;
	cipherInstance ci;
	unsigned char tabUsed[256];
	unsigned char subData3[256];
	unsigned int IP; /* init: by user before calling TwofishInit() */
	unsigned int dwIndex; /* init: 0 */
	int	tabEnable; /* init: 0 */
	int pos; /* init: 0 */
	int	numRounds[4]; /* init {0x00, 0x10, 0x10, 0x10} */
}TwofishObj;

/* private/internal functions */
unsigned int TwofishRS_MDS_Encode(unsigned int k0,unsigned int k1);
unsigned int TwofishF32(unsigned int x, unsigned int *k32,int keyLen);
void TwofishReKey(keyInstance *key);
void TwofishCipherInit(cipherInstance *cipher, unsigned char mode, char *IV);
void TwofishMakeKey(TwofishObj *Obj, keyInstance *key, unsigned char direction, int keyLen, char *keyMaterial);
void TwofishBlockEncrypt(cipherInstance *cipher, keyInstance *key, unsigned char *input, int inputLen, unsigned char *outBuffer);

/* public/exported functions */
void TwofishEncrypt(TwofishObj *Obj, unsigned char *in, unsigned char *out, int len);
void TwofishInit(TwofishObj *Obj);


#endif

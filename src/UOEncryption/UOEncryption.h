/******************************************************************************\
* 
* 
*  Copyright (C) 2004 Daniel 'Necr0Potenc3' Cavalcanti
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
* 	Jun 09th, 2004 -- IRW is done. started the encryption code
* 
\******************************************************************************/

#ifndef _UOENCRYPTION_H_INCLUDED
#define _UOENCRYPTION_H_INCLUDED

#ifdef _cplusplus
extern "C" {
#endif


/* Huffman adapted for network usage */
typedef struct tagHuffmanObj
{
	int has_incomplete;
	int incomplete_node;
	char incomplete_byte;
}HuffmanObj;

/* declares the minimal size of a buffer to decompress n bytes */
#define MIN_DECBUF_SIZE(in) ((in * 4) + 4)

void Compress(char *dest, const char *src, int *dest_size, int *src_size);
void Decompress(char *dest, const char *src, int *dest_size, int *src_size, HuffmanObj *obj);
void DecompressClean(HuffmanObj *obj);

/* Login crypt */
#define GETKEYS_MIN_SIZE 61

typedef struct tagLoginCryptInfo
{
	/* public, must be declared */
    unsigned int pseed;
	unsigned int k1, k2;
	/* private, dynamic vars used by the crypt code, each encrypted byte changes them */
	unsigned int m_key[2]; /* [0] - m_CryptMaskLo [1] - m_CryptMaskHi */
	unsigned int m_k1, m_k2; /* [1] - m_MasterHi  [2] - m_MasterLo */
}LoginCryptObj;

void LoginCryptInit(LoginCryptObj *obj);
void LoginCryptEncrypt(LoginCryptObj *obj, unsigned char * in, unsigned char * out, int len);
int CalculateKeys(unsigned char *Plaintext, unsigned char *Ciphertext, unsigned int *Seed, unsigned int *Key1, unsigned int *Key2);


/* Game crypt */
#define CRYPT_GAMEKEY_COUNT     25
#define CRYPT_GAMESEED_LENGTH   8

/* BLOWFISH */
typedef struct tagBlowfishObject
{
	unsigned char seed[CRYPT_GAMESEED_LENGTH];
	int table_index; /* init: 0 */
	int stream_pos; /* init: 0 */
	int block_pos; /* init: 0 */
}BlowfishObj;

void BlowfishInit(BlowfishObj *Obj);
void BlowfishEncrypt(BlowfishObj *Obj, unsigned char * in, unsigned char * out, int len);
void BlowfishDecrypt(BlowfishObj *Obj, unsigned char * in, unsigned char * out, int len);


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

/* TwofishEncrypt is for decryption as well */
void TwofishInit(TwofishObj *Obj);
void TwofishEncrypt(TwofishObj *Obj, unsigned char * in, unsigned char * out, int len);

/* MD5 (server->client encryption and decryption) */
typedef struct tagMD5Obj
{
	unsigned int TableIdx;
	unsigned char Digest[16];
}MD5Obj;

/* data should be subData3 and Size 256 */
void MD5Init(MD5Obj *Obj, unsigned char *Data, unsigned int Size);
void MD5Encrypt(MD5Obj *Obj, unsigned char *in, unsigned char *out, int len);

/* UO.cfg Password crypt */
void PWEncrypt(unsigned char *in, unsigned char *out, int len);
void PWDecrypt(unsigned char *in, unsigned char *out, int len);

#if _cplusplus
}
#endif

#endif

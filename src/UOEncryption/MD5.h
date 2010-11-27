/******************************************************************************\
* 
* 
*  Copyright (C) 2004 Daniel 'Necr0Potenc3' Cavalcanti
*  Copyright (C) 1999, 2000, 2002 Aladdin Enterprises.  All rights reserved.
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
*   Sept 14th, 2004  -- YES!! FINALLY I BROKE THE CODE MWAHAHA WAIT UP OSI!!
*   I mean... This is the last piece of UO's encryption puzzle. This module is
*   responsable by the server->client encryption/decryption on gamesocket.
*   It's only used with Twofish alone. Blowfish+Twofish doesn't use this.
* 
\******************************************************************************/


#ifndef _MD5_H_INCLUDED
#define _MD5_H_INCLUDED


/* md5 struct */
typedef struct tagmd5_state
{
    unsigned int count[2]; /* message length in bits, lsw first */
    unsigned int abcd[4]; /* digest buffer */
    unsigned char buf[64]; /* accumulate block */
}md5_state;

typedef struct tagMD5Obj
{
	unsigned int TableIdx;
	unsigned char Digest[16];
}MD5Obj;

/* private functions */
static void md5_process(md5_state *pms, const unsigned char *data /*[64]*/);
static void md5_start(md5_state *pms);
static void md5_append(md5_state *pms, const unsigned char *data, int nbytes);
static void md5_finish(md5_state *pms, unsigned char digest[16]);

/* public/exported functions */
void MD5Init(MD5Obj *Obj, unsigned char *Data, unsigned int Size);
void MD5Encrypt(MD5Obj *Obj, unsigned char *in, unsigned char *out, int len);


#endif

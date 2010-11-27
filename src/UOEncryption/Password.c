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


#include "Password.h"


/*
* a little history first...
* UO uses the RememberAcctPW=true and AcctPassword=pwd in uo.cfg to remember
* the password. AcctID to remember the login.
* the pwd is encrypted in the uo.cfg, these functions will encrypt/decrypt it.
*
* I first based from mamaich's encryption routine which was coded for Injection,
* from that function I wrote a decryption function, which is quite ugly :)
*
* thinking things were not right, one beautiful day (27/07/2003) I found the
* decryption routine in the client's assembly on accident, when searching for
* something else with folko, I don't even remember what.
* I wrote the PWDecrypt function from the assembly (reverse engineered) and after
* I wrote the PWEncrypt function based off from the PWDecrypt function. PWEncrypt
* turned out to be 100% equal to the password encryption function in UO's code.
*/

/*
* client 2.0.0g:
* 0047E820  /$ 56             PUSH ESI
* 0047E821  |. 8B7424 08      MOV ESI,DWORD PTR SS:[ESP+8]
* 0047E825  |. 57             PUSH EDI
* 0047E826  |. 8BFE           MOV EDI,ESI
* 0047E828  |. 83C9 FF        OR ECX,FFFFFFFF
* 0047E82B  |. 33C0           XOR EAX,EAX
* 0047E82D  |. F2:AE          REPNE SCAS BYTE PTR ES:[EDI]
* 0047E82F  |. F7D1           NOT ECX
* 0047E831  |. 49             DEC ECX
* 0047E832  |. 85C9           TEST ECX,ECX
* 0047E834  |. 7E 19          JLE SHORT Client.0047E84F
* 0047E836  |> 8A1430         /MOV DL,BYTE PTR DS:[EAX+ESI]
* 0047E839  |. 80C2 0D        |ADD DL,0D
* 0047E83C  |. 80FA 7F        |CMP DL,7F
* 0047E83F  |. 881430         |MOV BYTE PTR DS:[EAX+ESI],DL
* 0047E842  |. 76 06          |JBE SHORT Client.0047E84A
* 0047E844  |. 80EA 5F        |SUB DL,5F
* 0047E847  |. 881430         |MOV BYTE PTR DS:[EAX+ESI],DL
* 0047E84A  |> 40             |INC EAX
* 0047E84B  |. 3BC1           |CMP EAX,ECX
* 0047E84D  |.^7C E7          \JL SHORT Client.0047E836
* 0047E84F  |> 5F             POP EDI
* 0047E850  |. 5E             POP ESI
* 0047E851  \. C3             RETN
*/
void PWEncrypt(unsigned char *in, unsigned char *out, int len)
{
	int i = 0;
	unsigned char Tmp = 0;

	for(i = 0; i < len; i++)
	{
		Tmp = in[i];

		Tmp += 0x0D; /* 0x100 - 0xf3 */
		if(Tmp > 0x7F) /* 0x5f + 0x20 */
			Tmp -= 0x5F;

		out[i] = Tmp;
	}

	return;
}

/*
* client 2.0.0g
* 0047E860  /$ 56             PUSH ESI
* 0047E861  |. 8B7424 08      MOV ESI,DWORD PTR SS:[ESP+8]
* 0047E865  |. 57             PUSH EDI
* 0047E866  |. 8BFE           MOV EDI,ESI
* 0047E868  |. 83C9 FF        OR ECX,FFFFFFFF
* 0047E86B  |. 33C0           XOR EAX,EAX
* 0047E86D  |. F2:AE          REPNE SCAS BYTE PTR ES:[EDI]
* 0047E86F  |. F7D1           NOT ECX
* 0047E871  |. 49             DEC ECX
* 0047E872  |. 85C9           TEST ECX,ECX
* 0047E874  |. 7E 19          JLE SHORT Client.0047E88F
* 0047E876  |> 8A1430         /MOV DL,BYTE PTR DS:[EAX+ESI]
* 0047E879  |. 80C2 F3        |ADD DL,0F3
* 0047E87C  |. 80FA 20        |CMP DL,20
* 0047E87F  |. 881430         |MOV BYTE PTR DS:[EAX+ESI],DL
* 0047E882  |. 73 06          |JNB SHORT Client.0047E88A
* 0047E884  |. 80C2 5F        |ADD DL,5F
* 0047E887  |. 881430         |MOV BYTE PTR DS:[EAX+ESI],DL
* 0047E88A  |> 40             |INC EAX
* 0047E88B  |. 3BC1           |CMP EAX,ECX
* 0047E88D  |.^7C E7          \JL SHORT Client.0047E876
* 0047E88F  |> 5F             POP EDI
* 0047E890  |. 5E             POP ESI
* 0047E891  \. C3             RETN
*/
void PWDecrypt(unsigned char *in, unsigned char *out, int len)
{
	int i = 0;
	unsigned char Tmp = 0;

	for(i = 0; i < len; i++)
	{
		Tmp = in[i];

		Tmp += 0xF3;
		if(Tmp < 0x20) /* jnb */
			Tmp += 0x5F;

		out[i] = Tmp;
	}

	return;
}


/* THE CODE BELOW IS 100% ORIGINAL, THUS ITS NOT ANSI C, COMMENTED OUT :( */

/* mamaich's encryption routine */
/*
bool mamaichcrypt_pw(BYTE *in, BYTE *out, int len)
{
	//UO pwd can't be bigger than 30 chars
	//but fuck it, you know what you do
	if(len > 30)
		NumOut("Are you sure that password is ok? It's too long if you ask me...");

	for(int i=0; i<len; i++)
	{
		int c = in[i];
		//if(c < 31 && c > 127) //32 to 126 are the supported vallues
		//{
		//	strcpy((char*)out, "That password can't be encrypted");
		//	NumOut("Failed: Hex: 0x.2%X Character: %c", c, c);
		//	return 0;
		//}
		c += 13;
		if (c > 126) c -= 95;
		if(c == 32) c = 127;
		//just to debug
		//fprintf(fp, "%.2X\t%.2X\t%.3d\t%c\n", c,i,i, isprint(i) ? i : '.');
		out[i] = c;
	}

	return 1;
}
*/


/*
//the other method I invented, quited fucked up I should say
//shouldnt be used btw, works with mamaich's encryption
bool olddecrypt_pw(BYTE *in, BYTE *out, int len)
{
	if(len > 30)
		NumOut("Are you sure that password is ok? It's too long if you ask me...");

	//could use this one too, it's just cuter and better :)
	for(int i=0; i<len; i++)
	{
		int c = in[i];
		//there's no need to check to see if the encrypted byte
		//is in the encrypted bytes range (0x21 to 0x7f)
		//cause I'll check (at the end of the code) to see
		//if it's in the not-encrypted range
		if(c > 0x20 && c < 0x2D) //between 0x21 and 0x2c
		{
			if(c + 95 > 126) c +=95; //or just xor c, 0x52
			c -= 13;
		}
		else if(c > 0x2c && c < 0x80) //between 0x2d and 0x7f
		{
			if(c == 127) c == 32 + 13 + 13; //0x20
			c -= 13; //could just do xor c, 0x0d
		}
		//check to see if it's in the non-encrypted range
		if(c < 31 && c > 127)
		{
			strcpy((char*)out, "That password can't be broken");
			return 0;
		}
		out[i] = c;
	}

	return 1;
}
*/

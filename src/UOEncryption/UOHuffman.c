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
* 	April 4th, 2004 	-- Thinking about my future in uni, I decided to
* 	write this (yeah right, I just wanted to see how Huffman worked =))
* 	April 6th, 2004 	-- Finished it up, changed the vars's names to
* 	match Injection's, updated the decompress tree, now it's like a Huffman
* 	tree is supposed to be. Added explanations.
* 	April 7th, 2004		-- The end is an illusion :o. I thought about it the
* 	whole day in school and added support for incomplete codewords. It's almost
* 	impossible that a codeword comes broken in two bytes, but just in case...
* 
* 	This file is an exception to the rest of the UOEncryption and IRW coding
*   style. It is HIGHLY based in Injection and Ultimate Melange, and I only
*   coded the Huffman compression and decompression, the tables from the above
*   projects so it is more than fair that this code follows the style of those
*   projects. The original GPL agreement of Injection is in "UOHuffman.h"
* 
* 	Used the unix pure C coding style (var_foo)
* 
\*******************************************************************************/


#include <stdio.h>
#include "UOHuffman.h"

static int GetBit(int buf, int bit);
static int PutBit(unsigned char *byte, int bit);
static int RemoveBit(unsigned char *byte, int bit);


/**			HUFFMAN COMPRESSION AND DECOMPRESSION CODE						**/
/*
Compression and decompression explanations are above the respective trees
Huffman is based on byte occurance probabilities
the most frequent bytes get less bits in decompression, the less frequent
get the highest amount of bits
each sequence of bits that represents a de/compressed byte, is called codeword
The compression tree is built like that, then the decompression tree is based on
the compression tree
OSI built the compression tree after running the server for a few hours withouth
having compression enabled so they could get the byte frequency, so it's static
*/



/*
* compression tree
* the usage is: comp_tree[BYTE], BYTE being the byte you want to compress
* [BYTE][0] gives you how many bits the codeword has
* [BYTE][1] gives you the codeword
* then you build the bytes (bit by bit =o)
* and at the end of the buffer you add the flush codeword (or halt codeword)
* and thats it
* the most frequent byte is 0x00, it's codeword is 0x00 and it takes 2 bits
*/
static unsigned int comp_tree[257][2] =
{
/* 0x00 */
    { 0x02, 0x00 }, 	{ 0x05, 0x1F }, 	{ 0x06, 0x22 }, 	{ 0x07, 0x34 },
/* 0x04 */
    { 0x07, 0x75 }, 	{ 0x06, 0x28 }, 	{ 0x06, 0x3B }, 	{ 0x07, 0x32 },
/* 0x08 */
    { 0x08, 0xE0 }, 	{ 0x08, 0x62 }, 	{ 0x07, 0x56 }, 	{ 0x08, 0x79 },
/* 0x0c */
    { 0x09, 0x19D }, 	{ 0x08, 0x97 }, 	{ 0x06, 0x2A }, 	{ 0x07, 0x57 },
/* 0x10 */
    { 0x08, 0x71 }, 	{ 0x08, 0x5B }, 	{ 0x09, 0x1CC }, 	{ 0x08, 0xA7 },
    { 0x07, 0x25 }, 	{ 0x07, 0x4F }, 	{ 0x08, 0x66 }, 	{ 0x08, 0x7D },
    { 0x09, 0x191 }, 	{ 0x09, 0x1CE }, 	{ 0x07, 0x3F }, 	{ 0x09, 0x90 },
    { 0x08, 0x59 }, 	{ 0x08, 0x7B }, 	{ 0x08, 0x91 }, 	{ 0x08, 0xC6 },
/* 0x20 */
    { 0x06, 0x2D }, 	{ 0x09, 0x186 }, 	{ 0x08, 0x6F }, 	{ 0x09, 0x93 },
    { 0x0A, 0x1CC }, 	{ 0x08, 0x5A }, 	{ 0x0A, 0x1AE }, 	{ 0x0A, 0x1C0 },
    { 0x09, 0x148 }, 	{ 0x09, 0x14A }, 	{ 0x09, 0x82 }, 	{ 0x0A, 0x19F },
    { 0x09, 0x171 }, 	{ 0x09, 0x120 }, 	{ 0x09, 0xE7 }, 	{ 0x0A, 0x1F3 },
/* 0x30 */
    { 0x09, 0x14B }, 	{ 0x09, 0x100 }, 	{ 0x09, 0x190 }, 	{ 0x06, 0x13 },
    { 0x09, 0x161 }, 	{ 0x09, 0x125 }, 	{ 0x09, 0x133 }, 	{ 0x09, 0x195 },
    { 0x09, 0x173 }, 	{ 0x09, 0x1CA }, 	{ 0x09, 0x86 }, 	{ 0x09, 0x1E9 },
    { 0x09, 0xDB }, 	{ 0x09, 0x1EC }, 	{ 0x09, 0x8B }, 	{ 0x09, 0x85 },
/* 0x40 */
    { 0x05, 0x0A }, 	{ 0x08, 0x96 }, 	{ 0x08, 0x9C }, 	{ 0x09, 0x1C3 },
    { 0x09, 0x19C }, 	{ 0x09, 0x8F }, 	{ 0x09, 0x18F }, 	{ 0x09, 0x91 },
    { 0x09, 0x87 }, 	{ 0x09, 0xC6 }, 	{ 0x09, 0x177 }, 	{ 0x09, 0x89 },
    { 0x09, 0xD6 }, 	{ 0x09, 0x8C }, 	{ 0x09, 0x1EE }, 	{ 0x09, 0x1EB },
/* 0x50 */
    { 0x09, 0x84 }, 	{ 0x09, 0x164 }, 	{ 0x09, 0x175 }, 	{ 0x09, 0x1CD },
    { 0x08, 0x5E }, 	{ 0x09, 0x88 }, 	{ 0x09, 0x12B }, 	{ 0x09, 0x172 },
    { 0x09, 0x10A }, 	{ 0x09, 0x8D }, 	{ 0x09, 0x13A }, 	{ 0x09, 0x11C },
    { 0x0A, 0x1E1 }, 	{ 0x0A, 0x1E0 }, 	{ 0x09, 0x187 }, 	{ 0x0A, 0x1DC },
/* 0x60 */
    { 0x0A, 0x1DF }, 	{ 0x07, 0x74 }, 	{ 0x09, 0x19F }, 	{ 0x08, 0x8D },
    { 0x08, 0xE4 }, 	{ 0x07, 0x79 }, 	{ 0x09, 0xEA }, 	{ 0x09, 0xE1 },
    { 0x08, 0x40 }, 	{ 0x07, 0x41 }, 	{ 0x09, 0x10B }, 	{ 0x09, 0xB0 },
    { 0x08, 0x6A }, 	{ 0x08, 0xC1 }, 	{ 0x07, 0x71 }, 	{ 0x07, 0x78 },
/* 0x70 */
    { 0x08, 0xB1 }, 	{ 0x09, 0x14C }, 	{ 0x07, 0x43 }, 	{ 0x08, 0x76 },
    { 0x07, 0x66 }, 	{ 0x07, 0x4D }, 	{ 0x09, 0x8A }, 	{ 0x06, 0x2F },
    { 0x08, 0xC9 }, 	{ 0x09, 0xCE }, 	{ 0x09, 0x149 }, 	{ 0x09, 0x160 },
    { 0x0A, 0x1BA }, 	{ 0x0A, 0x19E }, 	{ 0x0A, 0x39F }, 	{ 0x09, 0xE5 },
/* 0x80 */
    { 0x09, 0x194 }, 	{ 0x09, 0x184 }, 	{ 0x09, 0x126 }, 	{ 0x07, 0x30 },
    { 0x08, 0x6C }, 	{ 0x09, 0x121 }, 	{ 0x09, 0x1E8 }, 	{ 0x0A, 0x1C1 },
    { 0x0A, 0x11D }, 	{ 0x0A, 0x163 }, 	{ 0x0A, 0x385 }, 	{ 0x0A, 0x3DB },
    { 0x0A, 0x17D }, 	{ 0x0A, 0x106 }, 	{ 0x0A, 0x397 }, 	{ 0x0A, 0x24E },
/* 0x90 */
    { 0x07, 0x2E }, 	{ 0x08, 0x98 }, 	{ 0x0A, 0x33C }, 	{ 0x0A, 0x32E },
    { 0x0A, 0x1E9 }, 	{ 0x09, 0xBF }, 	{ 0x0A, 0x3DF }, 	{ 0x0A, 0x1DD },
    { 0x0A, 0x32D }, 	{ 0x0A, 0x2ED }, 	{ 0x0A, 0x30B }, 	{ 0x0A, 0x107 },
    { 0x0A, 0x2E8 }, 	{ 0x0A, 0x3DE }, 	{ 0x0A, 0x125 }, 	{ 0x0A, 0x1E8 },
/* 0xA0 */
    { 0x09, 0xE9 }, 	{ 0x0A, 0x1CD }, 	{ 0x0A, 0x1B5 }, 	{ 0x09, 0x165 },
    { 0x0A, 0x232 }, 	{ 0x0A, 0x2E1 }, 	{ 0x0B, 0x3AE }, 	{ 0x0B, 0x3C6 },
    { 0x0B, 0x3E2 }, 	{ 0x0A, 0x205 }, 	{ 0x0A, 0x29A }, 	{ 0x0A, 0x248 },
    { 0x0A, 0x2CD }, 	{ 0x0A, 0x23B }, 	{ 0x0B, 0x3C5 }, 	{ 0x0A, 0x251 },
/* 0xB0 */
    { 0x0A, 0x2E9 }, 	{ 0x0A, 0x252 }, 	{ 0x09, 0x1EA }, 	{ 0x0B, 0x3A0 },
    { 0x0B, 0x391 }, 	{ 0x0A, 0x23C }, 	{ 0x0B, 0x392 }, 	{ 0x0B, 0x3D5 },
    { 0x0A, 0x233 }, 	{ 0x0A, 0x2CC }, 	{ 0x0B, 0x390 }, 	{ 0x0A, 0x1BB },
    { 0x0B, 0x3A1 }, 	{ 0x0B, 0x3C4 }, 	{ 0x0A, 0x211 }, 	{ 0x0A, 0x203 },
/* 0xC0 */
    { 0x09, 0x12A }, 	{ 0x0A, 0x231 }, 	{ 0x0B, 0x3E0 }, 	{ 0x0A, 0x29B },
    { 0x0B, 0x3D7 }, 	{ 0x0A, 0x202 }, 	{ 0x0B, 0x3AD }, 	{ 0x0A, 0x213 },
    { 0x0A, 0x253 }, 	{ 0x0A, 0x32C }, 	{ 0x0A, 0x23D }, 	{ 0x0A, 0x23F },
    { 0x0A, 0x32F }, 	{ 0x0A, 0x11C }, 	{ 0x0A, 0x384 }, 	{ 0x0A, 0x31C },
/* 0xD0 */
    { 0x0A, 0x17C }, 	{ 0x0A, 0x30A }, 	{ 0x0A, 0x2E0 }, 	{ 0x0A, 0x276 },
    { 0x0A, 0x250 }, 	{ 0x0B, 0x3E3 }, 	{ 0x0A, 0x396 }, 	{ 0x0A, 0x18F },
    { 0x0A, 0x204 }, 	{ 0x0A, 0x206 }, 	{ 0x0A, 0x230 }, 	{ 0x0A, 0x265 },
    { 0x0A, 0x212 }, 	{ 0x0A, 0x23E }, 	{ 0x0B, 0x3AC }, 	{ 0x0B, 0x393 },
/* 0xE0 */
    { 0x0B, 0x3E1 }, 	{ 0x0A, 0x1DE }, 	{ 0x0B, 0x3D6 }, 	{ 0x0A, 0x31D },
    { 0x0B, 0x3E5 }, 	{ 0x0B, 0x3E4 }, 	{ 0x0A, 0x207 }, 	{ 0x0B, 0x3C7 },
    { 0x0A, 0x277 }, 	{ 0x0B, 0x3D4 }, 	{ 0x08, 0xC0 }, 	{ 0x0A, 0x162 },
    { 0x0A, 0x3DA }, 	{ 0x0A, 0x124 }, 	{ 0x0A, 0x1B4 }, 	{ 0x0A, 0x264 },
/* 0xF0 */
    { 0x0A, 0x33D }, 	{ 0x0A, 0x1D1 }, 	{ 0x0A, 0x1AF }, 	{ 0x0A, 0x39E },
    { 0x0A, 0x24F }, 	{ 0x0B, 0x373 }, 	{ 0x0A, 0x249 }, 	{ 0x0B, 0x372 },
    { 0x09, 0x167 }, 	{ 0x0A, 0x210 }, 	{ 0x0A, 0x23A }, 	{ 0x0A, 0x1B8 },
    { 0x0B, 0x3AF }, 	{ 0x0A, 0x18E }, 	{ 0x0A, 0x2EC }, 	{ 0x07, 0x62 },
/* 0x100 - FLUSH CODE */
    { 0x04, 0x0D }
};


/*
* decompression tree - bit0 = walk left, bit1 = walk right
* always start at node0 and keep following it
* let's say you have the bytes: B2 E8
* binary look is: 10110010.11101000
* if we follow it in the tree, we get the path:
* 1, 4, 8, 15, 29, 53, 89, 144, -163
* we halt on values bellow 0 because thats a codeword, in this case -163
* which is multiplied by -1, then we get 163, in hex that's 0xA3
* thats the decompressed value
* so far we had: 10110010.1 to deal with
* now there's 1101
* the sequence is:
* 1, 3, 7, -256
* we halt (negative value) and check it, thats the flush codeword (or halt codeword)
* means the same as "there's nothing else to check in this byte"
* so if there is a next byte, we check it too, if not, end the function
*/
static int dec_tree[256][2] =
{
	/*node*/ /*leaf0  leaf1*/
	/*   0*/ {   2,    1},
	/*   1*/ {   4,    3},
	/*   2*/ {   0,    5},
	/*   3*/ {   7,    6},
	/*   4*/ {   9,    8},
	/*   5*/ {  11,   10},
	/*   6*/ {  13,   12},
	/*   7*/ {  14, -256},
	/*   8*/ {  16,   15},
	/*   9*/ {  18,   17},
	/*  10*/ {  20,   19},
	/*  11*/ {  22,   21},
	/*  12*/ {  23,   -1},
	/*  13*/ {  25,   24},
	/*  14*/ {  27,   26},
	/*  15*/ {  29,   28},
	/*  16*/ {  31,   30},
	/*  17*/ {  33,   32},
	/*  18*/ {  35,   34},
	/*  19*/ {  37,   36},
	/*  20*/ {  39,   38},
	/*  21*/ { -64,   40},
	/*  22*/ {  42,   41},
	/*  23*/ {  44,   43},
	/*  24*/ {  45,   -6},
	/*  25*/ {  47,   46},
	/*  26*/ {  49,   48},
	/*  27*/ {  51,   50},
	/*  28*/ {  52, -119},
	/*  29*/ {  53,  -32},
	/*  30*/ { -14,   54},
	/*  31*/ {  -5,   55},
	/*  32*/ {  57,   56},
	/*  33*/ {  59,   58},
	/*  34*/ {  -2,   60},
	/*  35*/ {  62,   61},
	/*  36*/ {  64,   63},
	/*  37*/ {  66,   65},
	/*  38*/ {  68,   67},
	/*  39*/ {  70,   69},
	/*  40*/ {  72,   71},
	/*  41*/ {  73,  -51},
	/*  42*/ {  75,   74},
	/*  43*/ {  77,   76},
	/*  44*/ {-111, -101},
	/*  45*/ { -97,   -4},
	/*  46*/ {  79,   78},
	/*  47*/ {  80, -110},
	/*  48*/ {-116,   81},
	/*  49*/ {  83,   82},
	/*  50*/ {-255,   84},
	/*  51*/ {  86,   85},
	/*  52*/ {  88,   87},
	/*  53*/ {  90,   89},
	/*  54*/ { -10,  -15},
	/*  55*/ {  92,   91},
	/*  56*/ {  93,  -21},
	/*  57*/ {  94, -117},
	/*  58*/ {  96,   95},
	/*  59*/ {  98,   97},
	/*  60*/ { 100,   99},
	/*  61*/ { 101, -114},
	/*  62*/ { 102, -105},
	/*  63*/ { 103,  -26},
	/*  64*/ { 105,  104},
	/*  65*/ { 107,  106},
	/*  66*/ { 109,  108},
	/*  67*/ { 111,  110},
	/*  68*/ {  -3,  112},
	/*  69*/ {  -7,  113},
	/*  70*/ {-131,  114},
	/*  71*/ {-144,  115},
	/*  72*/ { 117,  116},
	/*  73*/ { 118,  -20},
	/*  74*/ { 120,  119},
	/*  75*/ { 122,  121},
	/*  76*/ { 124,  123},
	/*  77*/ { 126,  125},
	/*  78*/ { 128,  127},
	/*  79*/ {-100,  129},
	/*  80*/ {  -8,  130},
	/*  81*/ { 132,  131},
	/*  82*/ { 134,  133},
	/*  83*/ { 135, -120},
	/*  84*/ { -31,  136},
	/*  85*/ { 138,  137},
	/*  86*/ {-234, -109},
	/*  87*/ { 140,  139},
	/*  88*/ { 142,  141},
	/*  89*/ { 144,  143},
	/*  90*/ { 145, -112},
	/*  91*/ { 146,  -19},
	/*  92*/ { 148,  147},
	/*  93*/ { -66,  149},
	/*  94*/ {-145,  150},
	/*  95*/ { -65,  -13},
	/*  96*/ { 152,  151},
	/*  97*/ { 154,  153},
	/*  98*/ { 155,  -30},
	/*  99*/ { 157,  156},
	/* 100*/ { 158,  -99},
	/* 101*/ { 160,  159},
	/* 102*/ { 162,  161},
	/* 103*/ { 163,  -23},
	/* 104*/ { 164,  -29},
	/* 105*/ { 165,  -11},
	/* 106*/ {-115,  166},
	/* 107*/ { 168,  167},
	/* 108*/ { 170,  169},
	/* 109*/ { 171,  -16},
	/* 110*/ { 172,  -34},
	/* 111*/ {-132,  173},
	/* 112*/ {-108,  174},
	/* 113*/ { -22,  175},
	/* 114*/ {  -9,  176},
	/* 115*/ { -84,  177},
	/* 116*/ { -37,  -17},
	/* 117*/ { 178,  -28},
	/* 118*/ { 180,  179},
	/* 119*/ { 182,  181},
	/* 120*/ { 184,  183},
	/* 121*/ { 186,  185},
	/* 122*/ {-104,  187},
	/* 123*/ { -78,  188},
	/* 124*/ { -61,  189},
	/* 125*/ {-178,  -79},
	/* 126*/ {-134,  -59},
	/* 127*/ { -25,  190},
	/* 128*/ { -18,  -83},
	/* 129*/ { -57,  191},
	/* 130*/ { 192,  -67},
	/* 131*/ { 193,  -98},
	/* 132*/ { -68,  -12},
	/* 133*/ { 195,  194},
	/* 134*/ {-128,  -55},
	/* 135*/ { -50,  -24},
	/* 136*/ { 196,  -70},
	/* 137*/ { -33,  -94},
	/* 138*/ {-129,  197},
	/* 139*/ { 198,  -74},
	/* 140*/ { 199,  -82},
	/* 141*/ { -87,  -56},
	/* 142*/ { 200,  -44},
	/* 143*/ { 201, -248},
	/* 144*/ { -81, -163},
	/* 145*/ {-123,  -52},
	/* 146*/ {-113,  202},
	/* 147*/ { -41,  -48},
	/* 148*/ { -40, -122},
	/* 149*/ { -90,  203},
	/* 150*/ { 204,  -54},
	/* 151*/ {-192,  -86},
	/* 152*/ { 206,  205},
	/* 153*/ {-130,  207},
	/* 154*/ { 208,  -53},
	/* 155*/ { -45, -133},
	/* 156*/ { 210,  209},
	/* 157*/ { -91,  211},
	/* 158*/ { 213,  212},
	/* 159*/ { -88, -106},
	/* 160*/ { 215,  214},
	/* 161*/ { 217,  216},
	/* 162*/ { -49,  218},
	/* 163*/ { 220,  219},
	/* 164*/ { 222,  221},
	/* 165*/ { 224,  223},
	/* 166*/ { 226,  225},
	/* 167*/ {-102,  227},
	/* 168*/ { 228, -160},
	/* 169*/ { 229,  -46},
	/* 170*/ { 230, -127},
	/* 171*/ { 231, -103},
	/* 172*/ { 233,  232},
	/* 173*/ { 234,  -60},
	/* 174*/ { -76,  235},
	/* 175*/ {-121,  236},
	/* 176*/ { -73,  237},
	/* 177*/ { 238, -149},
	/* 178*/ {-107,  239},
	/* 179*/ { 240,  -35},
	/* 180*/ { -27,  -71},
	/* 181*/ { 241,  -69},
	/* 182*/ { -77,  -89},
	/* 183*/ {-118,  -62},
	/* 184*/ { -85,  -75},
	/* 185*/ { -58,  -72},
	/* 186*/ { -80,  -63},
	/* 187*/ { -42,  242},
	/* 188*/ {-157, -150},
	/* 189*/ {-236, -139},
	/* 190*/ {-243, -126},
	/* 191*/ {-214, -142},
	/* 192*/ {-206, -138},
	/* 193*/ {-146, -240},
	/* 194*/ {-147, -204},
	/* 195*/ {-201, -152},
	/* 196*/ {-207, -227},
	/* 197*/ {-209, -154},
	/* 198*/ {-254, -153},
	/* 199*/ {-156, -176},
	/* 200*/ {-210, -165},
	/* 201*/ {-185, -172},
	/* 202*/ {-170, -195},
	/* 203*/ {-211, -232},
	/* 204*/ {-239, -219},
	/* 205*/ {-177, -200},
	/* 206*/ {-212, -175},
	/* 207*/ {-143, -244},
	/* 208*/ {-171, -246},
	/* 209*/ {-221, -203},
	/* 210*/ {-181, -202},
	/* 211*/ {-250, -173},
	/* 212*/ {-164, -184},
	/* 213*/ {-218, -193},
	/* 214*/ {-220, -199},
	/* 215*/ {-249, -190},
	/* 216*/ {-217, -230},
	/* 217*/ {-216, -169},
	/* 218*/ {-197, -191},
	/* 219*/ { 243,  -47},
	/* 220*/ { 245,  244},
	/* 221*/ { 247,  246},
	/* 222*/ {-159, -148},
	/* 223*/ { 249,  248},
	/* 224*/ { -93,  -92},
	/* 225*/ {-225,  -96},
	/* 226*/ { -95, -151},
	/* 227*/ { 251,  250},
	/* 228*/ { 252, -241},
	/* 229*/ { -36, -161},
	/* 230*/ { 254,  253},
	/* 231*/ { -39, -135},
	/* 232*/ {-124, -187},
	/* 233*/ {-251,  255},
	/* 234*/ {-238, -162},
	/* 235*/ { -38, -242},
	/* 236*/ {-125,  -43},
	/* 237*/ {-253, -215},
	/* 238*/ {-208, -140},
	/* 239*/ {-235, -137},
	/* 240*/ {-237, -158},
	/* 241*/ {-205, -136},
	/* 242*/ {-141, -155},
	/* 243*/ {-229, -228},
	/* 244*/ {-168, -213},
	/* 245*/ {-194, -224},
	/* 246*/ {-226, -196},
	/* 247*/ {-233, -183},
	/* 248*/ {-167, -231},
	/* 249*/ {-189, -174},
	/* 250*/ {-166, -252},
	/* 251*/ {-222, -198},
	/* 252*/ {-179, -188},
	/* 253*/ {-182, -223},
	/* 254*/ {-186, -180},
	/* 255*/ {-247, -245},
};

/* 87654321 */
/* 11010101 */
static int GetBit(int buf, int bit)
{
	return (buf >> (bit-1)) & 1;
}

static int PutBit(unsigned char *byte, int bit)
{
	int bit_num = 1 << (bit-1);
	*byte |= bit_num;
	return 1;
}

static int RemoveBit(unsigned char *byte, int bit)
{
	int bit_num=0;

	if(GetBit(*byte, bit))
	{
		bit_num = 1 << (bit-1);
		*byte ^= bit_num;
	}
	return 1;
}

void Compress(char *dest, const char *src, int *dest_size, int *src_size)
{
	int src_pos=0, dest_pos=0, tree_bits=0, tree_byte=0, bit_num=0, bit=0;

	while(src_pos <= *src_size)
	{
		if(src_pos != *src_size)
		{
			tree_bits = comp_tree[src[src_pos]&0xff][0];
			tree_byte = comp_tree[src[src_pos]&0xff][1];
		}
		else /* if the buffer is over, add the flush code */
		{
			tree_bits = comp_tree[256][0];
			tree_byte = comp_tree[256][1];
		}

		while(tree_bits)
		{
			/* proceed to next byte if we are done with this one */
			if(!bit_num) { dest_pos++; bit_num=8; };

			bit = GetBit(tree_byte, tree_bits);
			if(bit)
				PutBit((unsigned char*)&dest[dest_pos-1], bit_num);
			else
				RemoveBit((unsigned char*)&dest[dest_pos-1], bit_num);

			bit_num--; tree_bits--;
		}

		src_pos++;
	}

	*dest_size = dest_pos;
	return;
}


/*
* not sure if this happens, just a test
* test bytes: 10110010.10000001.11110000
*                   b2       81       f0
* if a regullar huffman code receives: b2 81 it will end
* up "forgetting" the first bit of the "11111" codeword
* so unless something fortunate happens, the decompressor
* will be broken. You might ask why a compressed packet
* wont be received in it's entirely but routers break
* a 100 bytes packet into chunks of 10 (more or less)
* the incomplete codeword support code saves the last node (if there was one)
* so the decompressor will remember the last position for the current codeword
* decompressed: a3 00 00 00 01 00 00
*/
void Decompress(char *dest, const char *src, int *dest_size, int *src_size, HuffmanObj *obj)
{
	int node=0, leaf=0, leaf_value=0, dest_pos=0, bit_num=8, src_pos=0;

	while(src_pos < *src_size)
	{
		/* if the last codeword was not full */
		if(obj != NULL && obj->has_incomplete)
		{
			src_pos = -1; bit_num=-1;
			leaf_value = obj->incomplete_node;
			obj->has_incomplete = 0;

			/* if this is the incomplete codeword, we already processed it */
			/* LogPrint("Processed the incomplete byte: %X\r\n", incomplete_byte); */
		}
		else
		{
			leaf = GetBit(src[src_pos], bit_num);
			leaf_value = dec_tree[node][leaf];
			/* LogPrint("Leaf: %d Value: %d CurNode: %d\r\n", leaf, leaf_value, node); */
			if(obj!= NULL && bit_num == 1)
				obj->incomplete_node = leaf_value;
		}

		/*
		* all numbers bellow 1 (0..-256) are codewords
		* if the halt codeword has been found, skip this byte
		*/
		if(leaf_value == -256)
			{ bit_num=8; node=0; src_pos++; continue; }
		else if(leaf_value < 1)
		{
			dest[dest_pos] = -leaf_value;
			leaf_value=0; dest_pos++;
		}

		bit_num--; node=leaf_value;
		/* if its the end of the byte, go to the next byte */
		if(bit_num < 1)
			{ bit_num=8; src_pos++; }

		/* check to see if the current codeword has no end
		if not, make it an incomplete byte */
		if(obj != NULL && src_pos == *src_size && node)
		{
			obj->incomplete_byte = src[src_pos-1];
			obj->has_incomplete = 1;
			/* LogPrint("Leaving incomplete byte: %X node: %d\r\n", incomplete_byte, incomplete_node); */
		}
	}

	*dest_size = dest_pos;
	return;
}


void DecompressClean(HuffmanObj *obj)
{
	obj->incomplete_byte = 0;
	obj->has_incomplete = 0;
	obj->incomplete_node = 0;

	return;
}


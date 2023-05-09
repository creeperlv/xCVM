#pragma once
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
typedef int bool;
typedef char byte;
#if defined(__MSDOS__)
	typedef long long int64;
#elif defined(_MSVC_LANG)
	typedef __int64 int64;
#elif defined(_WIN32)
	typedef __int64 int64;
#else
	typedef int64_t int64;
#endif
#define LIST_BLOCK 16
#define true 1
#define True 1
#define TRUE 1
#define false 0
#define False 0
#define FALSE 0
typedef struct __e{
	int ID;
	//Att - Attachment
	void* Att;
} Error;
typedef struct __r{
	void* Data;
	bool HasError;
	struct __e error;
}* Result;

typedef struct _str{
	char* HEAD;
	int Size;
	int Count;
}* string;

Error NewError();
Result NewResult();
string NewString();
bool AddChar(string str, char c);
bool CStrEqualsCStr(char* L,char* R);
void DestoryResult(Result r);
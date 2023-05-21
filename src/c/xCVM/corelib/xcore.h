#pragma once
#include "errors.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#ifndef __cplusplus
#define true 1
#define True 1
#define TRUE 1
#define false 0
#define False 0
#define FALSE 0
typedef int bool;
#endif
typedef char byte;
#if defined(__MSDOS__)
typedef long long Int64;
#elif defined(_MSVC_LANG)
typedef __int64 Int64;
#elif defined(_WIN32)
typedef __int64 Int64;
#else
typedef int64_t Int64;
#endif
typedef int Int32;
#define panic Panic
#define PANIC Panic
#define LIST_BLOCK 16
typedef struct __e {
    int ID;
    // Att - Attachment
    void* Att;
} Error;
typedef struct __r {
    void* Data;
    bool HasError;
    struct __e error;
}* Result;

typedef struct _str {
    char* HEAD;
    int Size;
    int Count;
}* string;
typedef struct _BL {
    int Size;
    int Count;
    byte* HEAD;
}* ByteList;
typedef struct _LI {
    struct _LI* Next;
    void* Att;
}* LinkedList;
typedef struct _DE {
    int ID;
    void* Att;
}* DictElem;
typedef struct _GL {
    int ListSize;
    int ItemSize;
    int Count;
    void** HEAD;
}* GenericList;
Error NewError();
Error NewErrorWID(int ID);
Result NewResult();
/**
 * New Result With Panic
 **/
Result NewResultWP();
Result NewString();
Result NewByteList();
Result StripByteList(ByteList List);
int ToInt32(ByteList List, int offset);
Int64 ToInt64(ByteList List, int offset);
Result AppendByteList(ByteList List, byte b);
Result NewLinkedListItem();
Result GetItem_LI(LinkedList HEAD, int Count);
Result NewDictionaryElement();
Result GetValueInDictionary(LinkedList HEAD, int Count);
Result NewGenericLsit(size_t ItemSize);
bool AddChar(string str, char c);
Result ToCStr(string str);
bool ByteArrayEqualsCStr(byte* L, char* R, size_t LLen);
bool CStrEqualsCStr(char* L, char* R);
void DestoryResult(Result r);
void SetPanicHandler(void (*func)(int));
void Panic();
void realpanic(int ID);

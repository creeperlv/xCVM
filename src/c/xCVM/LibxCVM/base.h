#pragma once
#define GL_SIZE 10
#include <stdlib.h>
#include <stdio.h>
#include "Exceptions.h"
typedef char bool;
#define true 1
#define True 1
#define TRUE 1
#define false 0
#define False 0
#define FALSE 0
typedef struct seg {
	struct seg* Prev;
	struct seg* Next;
	char* content;
	int Line;
}*Segment;
typedef struct GL {
	void** HEAD;
	int Size;
	size_t ItemSize;
	int Count;
}*GenericList;
typedef struct exc {
	int ID;
	char* Description;
	void* Content;
	int ContentType;
}*Exception;
typedef struct ER {
	Exception Exception;
	void* Content;
}*ExceptableResult;
Exception NewException(int ID,char* Descrption);
bool HasException(ExceptableResult ER);
ExceptableResult NewER();
ExceptableResult NewGL(size_t ItemSize);
ExceptableResult AddGL(GenericList List, void* item);
ExceptableResult DestoryGLWithoutDestoryChildren(GenericList List);
ExceptableResult DestoryGLWithDestoryChildren(GenericList List);
ExceptableResult GLToCStr(GenericList List);
ExceptableResult NewSegment();
void DestoryException(Exception exception);
void DestoryExceptionWithoutDestoryingContent(Exception exception);
void DestoryER(ExceptableResult Result);
void DestoryERWithoutDestoryingContent(ExceptableResult result);

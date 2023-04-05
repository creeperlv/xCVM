#pragma once
#include "base.h"
typedef struct CStr{
    char* HEAD;
    int Size;
    int Count;
}* String;
String NewString();
String NewStringFromCStr(char* cstr);
bool StringEqualsCStr(char* cstr);
bool StringEqualsCStr(String string);

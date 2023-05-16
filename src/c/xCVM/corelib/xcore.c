#include "xcore.h"
Error NewError()
{
    struct __e e;
    e.Att = NULL;
    e.ID = 0;
    return e;
}
Error NewErrorWID(int ID)
{
    struct __e e;
    e.Att = NULL;
    e.ID = ID;
    return e;
}
Result NewResult()
{
    Result r = (Result)malloc(sizeof(struct __r));
    if (r == NULL)
        return NULL;
    r->HasError = false;
    r->Data = NULL;
    return r;
}
Result NewString()
{
    Result r = NewResult();
    if (r == NULL)
        Panic();
    string str = (string)malloc(sizeof(struct _str));
    if (str == NULL) {
        DestoryResult(r);
        return NULL;
    }
    str->HEAD = (char*)malloc(sizeof(char) * LIST_BLOCK);
    if (str == NULL) {
        DestoryResult(r);
        free(str);
        return NULL;
    }
    str->Count = 0;
    str->Size = LIST_BLOCK;
    r->Data = str;
    return r;
}
bool AddChar(string str, char c)
{
    if (str == NULL)
        return false;
    if (str->Count >= str->Size) {
        int NewSize = str->Size + LIST_BLOCK;
        void* __new = realloc(str->HEAD, sizeof(char) * NewSize);
        if (__new == NULL) {
            return false;
        }
        str->HEAD = __new;
        str->Size = NewSize;
        return AddChar(str, c);
    }
    str->HEAD[str->Count] = c;
    str->Count++;
    return true;
}
Result ToCStr(string str)
{
    Result r = NewResult();
    if (r == NULL)
        Panic();
    char* cstr = (char*)malloc(sizeof(char) * (str->Count + 1));
    for (int i = 0; i < str->Count; i++) {
        cstr[i] = str->HEAD[i];
    }
    cstr[str->Count] = '\0';
    r->Data = cstr;
    return r;
}
bool ByteArrayEqualsCStr(byte* L, char* R, size_t LLen)
{
    if (L == NULL) {
        if (R == NULL)
            return true;
        return false;
    }
    if (R == NULL)
        return false;
    int LC = LLen;
    int RC = strlen(R);
    if (LC != RC)
        return false;
    for (int i = 0; i < LC; i++) {
        if (L[i] != R[i])
            return false;
    }
    return true;
}
bool CStrEqualsCStr(char* L, char* R)
{
    if (L == NULL) {
        if (R == NULL)
            return true;
        return false;
    }
    if (R == NULL)
        return false;
    int LC = strlen(L);
    int RC = strlen(R);
    if (LC != RC)
        return false;
    for (int i = 0; i < LC; i++) {
        if (L[i] != R[i])
            return false;
    }
    return true;
}
void DestoryResult(Result r)
{
    r->Data = NULL;
    free(r);
}
void Panic()
{
    puts("Runtime panic!");
    puts("Usually due to malloc cannot alloc desired memory.");
    exit(1);
}

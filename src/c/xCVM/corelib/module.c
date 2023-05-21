#include "module.h"
Result NewMetadata()
{
    Result r = NewResult();
    if (r == NULL) {
        Panic();
        return NULL;
    }
    ModuleMetadata m = (ModuleMetadata)malloc(sizeof(struct _mi));
    if (m == NULL) {
        DestoryResult(r);
        return NULL;
    }
    r->Data = m;
    return r;
}
Int32 ReadInt32(byte* Buffer, FILE* f)
{
    Buffer[0] = fgetc(f);
    Buffer[1] = fgetc(f);
    Buffer[2] = fgetc(f);
    Buffer[3] = fgetc(f);
    return ((Int32*)Buffer)[0];
}
Int64 ReadInt64(byte* Buffer, FILE* f)
{
    Buffer[0] = fgetc(f);
    Buffer[1] = fgetc(f);
    Buffer[2] = fgetc(f);
    Buffer[3] = fgetc(f);
    Buffer[4] = fgetc(f);
    Buffer[5] = fgetc(f);
    Buffer[6] = fgetc(f);
    Buffer[7] = fgetc(f);
    return ((Int32*)Buffer)[0];
}
bool LoadModuleMeta(ModuleMetadata meta, FILE* f)
{
    byte Byte4Buffer[4];
    {
        Int32 DataLen = ReadInt32(Byte4Buffer, f);
        char* Data = (char*)malloc((DataLen + 1) * sizeof(char));
        for (int i = 0; i < DataLen; i++) {
            Data[i] = fgetc(f);
            if (Data[i] == EOF)
                return false;
        }
        Data[DataLen] = '\0';
        meta->Name = Data;
        meta->name_len = DataLen;
    }
    {
        Int32 DataLen = ReadInt32(Byte4Buffer, f);
        char* Data = (char*)malloc((DataLen + 1) * sizeof(char));
        for (int i = 0; i < DataLen; i++) {
            Data[i] = fgetc(f);
            if (Data[i] == EOF)
                return false;
        }
        Data[DataLen] = '\0';
        meta->Author = Data;
        meta->author_len = DataLen;
    }
    return true;
}
Result NewInst()
{
    Result r = NewResult();
    if (r == NULL) {
        Panic();
        return NULL;
    }
    Inst inst = (Inst)malloc(sizeof(struct __inst));
    if (inst == NULL)
        return NULL;
    r->Data = inst;
    return r;
}
bool LoadInst(Inst inst, FILE* f)
{

    return true;
}
Result NewModule()
{
    Result r = NewResult();
    if (r == NULL) {
        Panic();
        return NULL;
    }
    Module m = (Module)malloc(sizeof(struct _m));
    if (m == NULL) {
        DestoryResult(r);
        return NULL;
    }
    r->Data = m;
    return r;
}
Result LoadModule(FILE* f)
{
    Result r = NewResult();
    if (r == NULL) {
        Panic();
        return NULL;
    }
    Result mr = NewModule();
    if (mr->HasError) {
        r->Data = NULL;
        r->error = mr->error;
        return r;
    }
    Module m = mr->Data;
    DestoryResult(mr);
    // HEADER CHECK
    {
        byte header[4]; //=new byte[4];
        header[0] = fgetc(f);
        header[1] = fgetc(f);
        header[2] = fgetc(f);
        header[3] = fgetc(f);
        if (!ByteArrayEqualsCStr(header, "xCVM", 4)) {
            r->HasError = true;
            r->error = NewErrorWID(ERROR_CORE_DATA_MODULE_HEADER_MISMATCH);
            return r;
        }
    }
    //
    {
        Result metar = NewMetadata();
        if (metar == NULL) {
            Panic();
            return NULL;
        }
        ModuleMetadata meta = metar->Data;
        DestoryResult(metar);
        LoadModuleMeta(meta, f);
    }
    while (true) {

        char c = fgetc(f);
        if (c == EOF) {
            break;
        }
    }
    return r;
}

#pragma once
#include "./xcore.h"
typedef struct __inst {
    int inst;
    short op0_len;
    short op1_len;
    short op2_len;
    byte* op0;
    byte* op1;
    byte* op2;
}* Inst;
typedef struct intText {
    int ID;
    int Length;
    char* HEAD;
}* IntText;
typedef struct ver {
    int Major;
    int Minor;
    int Build;
    int Patch;
}* Version;
typedef struct _mi {
    int name_len;
    char* Name;
    int author_len;
    char* Author;
    int cpyr_len;
    char* Copyright;
    int* desc_len;
    char* Description;
    Version ModuleVersion;
    Version TargetVersion;
}* ModuleMetadata;
typedef struct _m {
    ModuleMetadata Metadata;
    int TextCount;
    IntText* Texts;
    int IDCount;
    IntText* IDs;
    int InstCount;
    Inst* HEAD;
}* Module;
typedef struct reg {
    byte* HEAD;
    int Size;
    int WordLen;
} xCVMRegister;
typedef struct mem {
    DictElem Mem;
} xCVMMemory;
typedef struct callframe {
    int ModuleID;
    int PC;
} CallFrame;
typedef struct _CS {
    int Count;
    int Size;
    CallFrame* HEAD;
}* CallStack;
typedef struct xminst {
    xCVMRegister xmreg;
    xCVMMemory vmmem;
    DictElem LoadedModules;
    Module CurrentModule;
    int PC;
    CallStack Callstack;
} xCVMInstance;

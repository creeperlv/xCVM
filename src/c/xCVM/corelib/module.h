#pragma once
#include "xcore.h"
#include "datas.h"
#include "errors.h"
Result NewMetadata();
bool LoadModuleMeta(ModuleMetadata meta,FILE*f);
Result NewInst();
bool LoadInst(Inst inst,FILE* f);
Result NewModule();
Result LoadModule(FILE* f);

Result NewCallStack();
bool PushCallStack(CallFrame frame);
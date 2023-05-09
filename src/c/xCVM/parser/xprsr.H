#pragma once
#include "../corelib/xcore.h"
typedef struct seg{
	struct seg* Prev;
	struct seg* Next;
	void* Data;
	int ID;
	int Index;
}* Segment;

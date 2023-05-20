#include "rtdata.h"
Result NewCallStack()
{
	Result r = NewResultWP();
	CallFrame* HEAD = (CallFrame*)malloc(sizeof(CallFrame) * LIST_BLOCK);
	if (HEAD = NULL) {

		DestoryResult(r);
		return NULL;
	}
	CallStack _cs = (CallStack)malloc(sizeof(struct  _CS));
	if (_cs == NULL) {
		DestoryResult(r);
		return NULL;
	}
	_cs->HEAD = HEAD;
	_cs->Count = 0;
	_cs->Size = LIST_BLOCK;
	return r;
}

bool PushCallStack(CallFrame frame)
{
	return false;
}

#include "base.h"

Exception NewException(int ID, char* Descrption)
{
	Exception e = (Exception)malloc(sizeof(struct exc));
	if (e == NULL)return e;
	e->ID = ID;
	e->Description = Descrption;
	e->Content = NULL;
	e->ContentType = -1;
	return e;
}

ExceptableResult NewER()
{
	ExceptableResult _ER = (ExceptableResult)malloc(sizeof(struct ER));
	if (_ER == NULL)return NULL;
	_ER->Exception = NULL;
	_ER->Content = NULL;
	return _ER;
}

ExceptableResult NewGL(size_t ItemSize)
{
	ExceptableResult result = NewER();
	GenericList list = (GenericList)malloc(sizeof(struct GL));
	if (list == NULL) {
		result->Exception = NewException(GL_MALLOC_FAILED_ID, GL_MALLOC_FAILED_DESC);
		result->Content = NULL;
		return result;
	}
	list->HEAD = (void**)malloc(ItemSize * GL_SIZE);
	if (list->HEAD == NULL) {
		free(list);
		result->Exception = NewException(GL_HEAD_MALLOC_FAILED_ID, GL_HEAD_MALLOC_FAILED_DESC);
		result->Content = NULL;
		return result;
	}
	result->Content = list;
	list->Size = GL_SIZE;
	list->Count = 0;
	list->ItemSize = ItemSize;
	return result;
}

ExceptableResult AddGL(GenericList List, void* item)
{
	if (List->Count >= List->Size) {
		void** temp = (void*)realloc(List->HEAD, List->ItemSize * (List->Size + GL_SIZE));
		if (temp == NULL) {
			ExceptableResult result = NewER();
			result->Exception = NewException(GL_REALLOC_FAILED_ID, GL_REALLOC_FAILED_DESC);
			return result;
		}
		List->HEAD = (void*)temp;
		List->Size = List->Size + GL_SIZE;
		return AddGL(List, item);
	}
	List->HEAD[List->Count] = item;
	List->Count++;
	return NULL;
}

ExceptableResult DestoryGLWithoutDestoryChildren(GenericList List)
{
	for (size_t i = 0; i < List->Count; i++)
	{
		List->HEAD[i] = NULL;
	}
	free(List);
	return NULL;
}

ExceptableResult DestoryGLWithDestoryChildren(GenericList List)
{
	free(List);
	return NULL;
}

ExceptableResult GLToCStr(GenericList List)
{
	ExceptableResult er = NewER();
	size_t count = (List->Count);
	char* str = malloc(count * sizeof(char));
	if (str == NULL) {
		er->Exception = NewException(CSTR_MALLOC_FAILED_ID, CSTR_MALLOC_FAILED_DESC);
		return er;
	}
	for (size_t i = 0; i < count; i++)
	{
		str[i] = ((char*)List->HEAD[i])[0];
	}
	str[count] = '\0';
	er->Content = str;
	return er;
}

ExceptableResult NewSegment()
{
	ExceptableResult er = NewER();
	Segment seg = (Segment)malloc(sizeof(struct seg));
	if (seg == NULL) {
		er->Exception = NewException(MALLOC_FAILED_ID, MALLOC_FAILED_DESC);
		return er;
	}
	seg->content = NULL;
	seg->Line = -1;
	seg->Next = NULL;
	seg->Prev = NULL;
	er->Content = seg;
	return er;
}
bool HasException(ExceptableResult ER) {
	return ER->Exception == NULL;
}
void DestoryException(Exception exception)
{
	free(exception);
}

void DestoryExceptionWithoutDestoryingContent(Exception exception)
{
	exception->Content = NULL;
	exception->Description = NULL;
	free(exception);
}

void DestoryER(ExceptableResult Result)
{
	DestoryException(Result->Exception);
	Result->Exception = NULL;
	free(Result);
}

void DestoryERWithoutDestoryingContent(ExceptableResult result)
{
	DestoryException(result->Exception);
	result->Exception = NULL;
	result->Content = NULL;
	free(result);
}

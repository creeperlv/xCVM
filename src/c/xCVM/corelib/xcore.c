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
Result NewResultWP()
{
	Result r = (Result)malloc(sizeof(struct __r));
	if (r == NULL) {
		Panic();
		return NULL;
	}
	r->HasError = false;
	r->Data = NULL;
	return r;
}
Result NewString()
{
	Result r = NewResult();
	if (r == NULL) {
		Panic();
		return NULL;
	}
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
Result NewByteList()
{
	Result r = NewResultWP();
	byte* head = (byte*)malloc(LIST_BLOCK * sizeof(byte));
	if (head == NULL) {
		if (head == NULL) {
			DestoryResult(r);
			return NULL;
		}
	}
	ByteList BL = (ByteList)malloc(sizeof(struct _BL));
	if (BL == NULL) {
		DestoryResult(r);
		return NULL;
	}
	BL->HEAD = head;
	BL->Count = 0;
	BL->Size = LIST_BLOCK;
	r->Data = BL;
	return r;
}
Result StripByteList(ByteList List)
{
	Result r = NewResultWP();
	int Size = List->Count;
	byte* head = (byte*)malloc(Size * sizeof(byte));
	if (head == NULL) {
		DestoryResult(r);
		return NULL;
	}
	for (int i = 0; i < Size; i++) {
		head[i] = List->HEAD[i];
	}
	r->Data = head;
	return r;
}
int ToInt32(ByteList List, int offset)
{
	byte* newHead = List->HEAD + offset;
	int* intHead = (int*)newHead;
	return intHead[0];
}
Int64 ToInt64(ByteList List, int offset)
{
	byte* newHead = List->HEAD + offset;
	Int64* Int64Head = (Int64*)newHead;
	return Int64Head[0];
}
Result AppendByteList(ByteList List, byte b)
{
	Result r = NewResultWP();
	if (List->Count >= List->Size) {
		byte* newhead = (byte*)realloc(List->HEAD, sizeof(byte) * (List->Size + LIST_BLOCK));
		if (newhead == NULL) {
			r->HasError = true;
			r->error = NewErrorWID(ERROR_REALLOC_FAILED);
			return r;
		}
		List->Size += LIST_BLOCK;
		List->HEAD = newhead;
		DestoryResult(r);
		return AppendByteList(List, b);
	}
	List->HEAD[List->Count] = b;
	List->Count++;
	return r;
}
Result GetItem_LI(LinkedList HEAD, int Count)
{
	Result r = NewResultWP();
	LinkedList CUR = HEAD;
	for (int i = 0; i <= Count; i++) {
		if (CUR->Next == NULL) {
			r->HasError = true;
			Error e = NewErrorWID(ERROR_LIST_INDEX_OUT_OF_RANGE);
			r->error = e;
			return r;
		}
		CUR = CUR->Next;
	}
	r->Data = CUR->Att;
	return r;
}
Result NewDictionaryElement()
{
	Result r = NewResultWP();
	DictElem de = (DictElem)malloc(sizeof(struct _DE));
	if (de == NULL) {
		DestoryResult(r);
		return NULL;
	}
	de->ID = 0;
	de->Att = NULL;
	r->Data = de;
	return r;
}
Result GetValueInDictionary(LinkedList HEAD, int Count)
{
	Result r = NewResultWP();
	Result LI_R = GetItem_LI(HEAD, Count);
	if (LI_R->HasError) {
		r->HasError = true;
		r->error = LI_R->error;
		DestoryResult(LI_R);
		return r;
	}
	DictElem de = LI_R->Data;
	r->Data = de;
	return r;
}
Result NewGenericList(size_t ItemSize)
{
	Result r = NewResult();
	if (r == NULL) {
		Panic();
		return NULL;
	}
	void** HEAD = (void**)malloc(ItemSize * LIST_BLOCK);
	if (HEAD == NULL) {
		DestoryResult(r);
		return NULL;
	}
	GenericList list = (GenericList)malloc(sizeof(struct _GL));
	if (list == NULL) {
		free(HEAD);
		DestoryResult(r);
		return NULL;
	}
	list->HEAD = HEAD;
	list->Count = 0;
	list->ItemSize = ItemSize;
	list->ListSize = LIST_BLOCK;
	r->Data = list;
	return r;
}
Result NewLinkedListItem()
{
	Result r = NewResult(); 
	if (r == NULL) {
		Panic();
		return NULL;
	}
	LinkedList ll = (LinkedList)malloc(sizeof(struct _LI));
	if (ll == NULL) {
		DestoryResult(r);
		return NULL;
	}
	r->Data = ll;
	ll->Next = NULL;
	ll->Att = NULL;
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
	if (r == NULL) {
		Panic();
		return NULL;
	}
	char* cstr = (char*)malloc(sizeof(char) * (str->Count + 1));
	if (cstr == NULL) {
		DestoryResult(r);
		return NULL;
	}
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
	size_t LC = LLen;
	size_t RC = strlen(R);
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
	size_t LC = strlen(L);
	size_t RC = strlen(R);
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
void (*panic_handler)(int) = NULL;
void SetPanicHandler(void (*func)(int)) { panic_handler = func; }
void Panic() { realpanic(0); }

void realpanic(int ID)
{
	if (panic_handler == NULL) {
		puts("Runtime panic!");
		puts("Usually due to malloc cannot alloc desired memory.");
		printf("Panic Reason:%d", ID);
		exit(1);
		return;
	}
	else {
		panic_handler(ID);
	}
}

#include "xcore.h"
Error NewError(){
	struct __e e;
	e.Att=NULL;
	e.ID=0;
	return e;
}
Result NewResult(){
	Result r=(Result)malloc(sizeof(struct __r));
	if(r==NULL)return NULL;
	r->HasError=false;
	r->Data=NULL;
	return r;
}
bool CStrEqualsCStr(char* L, char* R){
	if(L==NULL){
		if(R==NULL)return true;
		return false;
	}
	if(R==NULL)return false;
	int LC=strlen(L);
	int RC=strlen(R);
	if(LC!=RC)return false;
	for(int i=0;i<LC;i++){
		if(L[i]!=R[i])return false;
	}
	return true;
}
void DestoryResult(Result r){
	r->Data=NULL;
	free(r);
}
void Panic(){
	puts("Runtime panic!");
	puts("Usually due to malloc cannot alloc desired memory.");
	exit(1);
}

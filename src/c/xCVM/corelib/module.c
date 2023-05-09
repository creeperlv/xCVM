#include "module.h"
Result NewModule(){
	Result r=NewResult();
	if(r==NULL)Panic();
	Module m=(Module)malloc(sizeof(struct _m));
	if(m==NULL)return NULL;
	r->Data=m;
	return r;
}
Result LoadModule(FILE* f){
	Result r=NewResult();
	if(r==NULL){
		Panic();		
	}
	Result mr=NewModule();
	if(mr->HasError){
		r->Data=NULL;
		r->error=mr->error;
		return r;
	}
	Module m=mr->Data;
	DestoryResult(mr);
	while(true){
		char c=fgetc(f);
		if(c==EOF){
			break;
		}
		
	}
	return r;
}

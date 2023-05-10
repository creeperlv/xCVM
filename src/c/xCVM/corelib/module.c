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
	//HEADER CHECK
	{
		byte header[4];//=new byte[4];
		header[0]=fgetc(f);
		header[1]=fgetc(f);
		header[2]=fgetc(f);
		header[3]=fgetc(f);
		if(!ByteArrayEqualsCStr(&header, "xCVM",4)){
			r->HasError=true;
			r->error=NewErrorWID(ERROR_CORE_DATA_MODULE_HEADER_MISMATCH);
			return r;
		}
	}
	//
	while(true){
		char c=fgetc(f);
		if(c==EOF){
			break;
		}
		
	}
	return r;
}
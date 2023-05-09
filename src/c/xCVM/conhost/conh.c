#include "conh.h"
#define CEC(L,R) CStrEqualsCStr(L,R)
bool AppendInput(args arg,char* str){
	if(arg->InC>=arg->InS){
		if(arg->input==NULL){
			arg->input=
(char**)malloc(sizeof(char*)*LIST_BLOCK);
			return AppendInput(arg,str);
		}else{
			int SIZE=sizeof(char*)*(arg->InS+LIST_BLOCK);
			void* _n=realloc(arg->input,SIZE);
			if(_n==NULL)return false;
			arg->InS+=LIST_BLOCK;
			arg->input=(char**)_n;
			return AppendInput(arg,str);
		}
	}else{
		arg->input[arg->InC]=str;
		arg->InC++;
		return true;
	}
}
Result ParseArg(int argc,char** argv){
	Result r=NewResult();
	if(r==NULL)return NULL;
	args arg=(args)malloc(sizeof(struct __a));
	if(arg==NULL){
		return NULL;
	}
	r->Data=arg;
	arg->InC=0;
	arg->input=NULL;
	arg->InS=0;
	arg->ShowVer=false;
	arg->ShowHelp=false;
	arg->output=NULL;
	for(int i=1;i<argc;i++){
		char* item=argv[i];
		if(CStrEqualsCStr(item,"-v")){
			arg->ShowVer=true;
		}else if(CEC(item,"--version")){
			arg->ShowVer=true;
		}else if(CEC(item,"-o")){
			i++;
			item=argv[i];
			arg->output=item;
		}else{
			AppendInput(arg,item);			
		}
	}
	return r;
}

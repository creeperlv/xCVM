#include "../../parser/xprsr.h"
#include "../../corelib/ter.h"
#include "../../conhost/conh.h"
int main(int argc,char** argv){
	Result r=ParseArg(argc,argv);
	if(r==NULL){
		WriteLine("KERNEL PANIC!!MALLOC FAILED!");
	}
	args arg=r->Data;
	if(arg->ShowVer){
		WriteLine("C eXtended Compiler");
		WriteLine("Version: 0.0.1.0-dev");
		WriteLine("Copyright(C)2023 Creeper Lv");
		WriteLine("This program is licensed under the MIT License.");	
		return 0;
	}
	if(arg->InC==0){
		WriteLine("ERROR: NO INPUT FILE.");
	}else{
		WriteLine("Inputs:");
		for(int i=0;i<arg->InC;i++){
			WriteLine(arg->input[i]);
		}
	}
	return 0;
}

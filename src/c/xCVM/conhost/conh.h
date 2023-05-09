#pragma once
#include "../corelib/xcore.h"
typedef struct __a{
	char** input;
	int InC;
	int InS;
	char* output;
	bool ShowVer;
	bool ShowHelp;
}* args;
Result ParseArg(int argc, char** argv);

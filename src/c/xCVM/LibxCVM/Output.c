#include "Output.h"
bool Quiet = false;
void SetForeground(ConsoleColor color)
{
	if (Quiet)return;
	switch (color)
	{
	case red:
	{
		printf("\033[91m");
	}
	break;
	case green:
	{
		printf("\033[92m");
	}break;
	case blue:
	{
		printf("\033[34m");
	}break;
	default:
		break;
	}
}
void ResetColor()
{
	if (Quiet)return;
	printf("\033[m");
}
void WriteLineCStr
(char* str)
{
	if (Quiet)return;
	printf("%s\n", str);
}

void WriteLineGL(GenericList str)
{
	ExceptableResult ER = GLToCStr(str);
	if (ER->Exception != NULL) {
		printf("PANIC:%s\n", ER->Exception->Description);
		DestoryER(ER);
	}
	else {
		WriteLineCStr(ER->Content);
		free(ER);
	}
}

void WriteLine()
{
	if (Quiet)return;
	printf("\n");
}

void WriteGL(GenericList str)
{
	ExceptableResult ER = GLToCStr(str);
	if (ER->Exception != NULL) {
		printf("PANIC:%s\n", ER->Exception->Description);
		DestoryER(ER);
	}
	else {
		WriteCStr(ER->Content);
		free(ER);
	}
}

void WriteCStr(char* str)
{
	if (Quiet)return;
	printf("%s", str);
}

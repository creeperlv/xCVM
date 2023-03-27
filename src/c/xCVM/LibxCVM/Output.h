#pragma once
#include "base.h"
extern bool Quiet;
typedef enum ConsoleColor
{
	red, green, blue
}ConsoleColor;
void SetForeground(ConsoleColor color);
void ResetColor();
void WriteLineCStr(char* str);
void WriteLineGL(GenericList str);
void WriteLine();
void WriteGL(GenericList str);
void WriteCStr(char* str);
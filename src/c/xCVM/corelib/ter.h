#pragma once
#include "xcore.h"
#define ANSI_Black 0
#define ANSI_Red 1
#define ANSI_Green 2
#define ANSI_Yellow 3
#define ANSI_Blue 4
#define ANSI_Magneta 5
#define ANSI_Cyan 6
#define ANSI_White 7
#define ANSI_Bright 10
void Write(char* str);
void WriteLine(char* str);
void SetFG(int code);
void SetBG(int code);
void ResetColors();

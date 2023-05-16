#include "ter.h"
void Write(char* str){
	printf("%s",str);
}
void WriteLine(char* str){
	printf("%s\n",str);
}

#if defined(__MSDOS__)

void SetFG(int code)
{
        printf("\x1b[3%dm", code % 10);
}
void SetBG(int code)
{
        printf("\x1b[4%dm", code % 10);
}
#else
void SetFG(int code)
{
        if (code < 10) {
            printf("\x1b[3%dm", code);
        } else {
            printf("\x1b[9%dm", code % 10);
        }
}
void SetBG(int code)
{
        if (code < 10) {
            printf("\x1b[4%dm", code);
        } else {
            printf("\x1b[10%dm", code % 10);
        }
}
#endif
void ResetColors()
{
        printf("\x1b[0m");
}

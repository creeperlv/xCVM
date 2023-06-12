#include "../corelib/module.h"
#include "../corelib/ter.h"
#include <stdio.h>
int main(int argc, char** argv)
{
    if (argc == 1) {
        SetFG(ANSI_Red + ANSI_Bright);
        Write("error:");
        ResetColors();
        WriteLine("no input file.");
        return 0;
    }
    char* fn = argv[1];
    FILE* f = fopen(fn, "r");
    if (f == NULL) {
        SetFG(ANSI_Red + ANSI_Bright);
        Write("error:");
        ResetColors();
        WriteLine("file not found.");
        return 0;
    }
    Result r = LoadModule(f);
    fclose(f);
    if (r->HasError) {
        Write("Cannot load module: ");
        WriteLine(GetErrorMessage(r->error.ID));
    }
    return 0;
}

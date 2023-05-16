#include "../corelib/ter.h"
#include "../corelib/module.h"
int main(int argc, char** argv){
	if(argc==1){
            SetFG(ANSI_Red + ANSI_Bright);
            Write("error:");
            ResetColors();
            WriteLine("no input file.");
            return 0;
        }
        char* fn = argv[1];
        FILE* f = fopen(fn, "r");
        Result r=LoadModule(f);
	if(r->HasError){
		Write("Cannot load module: ");
		WriteLine(GetErrorMessage(r->error.ID));
	}
	return 0;
}

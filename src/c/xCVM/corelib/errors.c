#include "errors.h"
char* GetErrorMessage(int ID){
	switch(ID){
		case ERROR_CORE_DATA_MODULE_HEADER_MISMATCH:
			return "Module Header Mismatch";
		default:
			return "Undefined Message";
	}
}

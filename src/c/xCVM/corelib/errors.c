#include "errors.h"
char* GetErrorMessage(int ID){
	switch(ID){
		case ERROR_CORE_DATA_MODULE_HEADER_MISMATCH:
			return "Module Header Mismatch";
                        break;
                case ERROR_LIST_INDEX_OUT_OF_RANGE:
                        return "Index out of range";
                case ERROR_REALLOC_FAILED:
                        return "Failed on realloc()!";
                default:
                        return "Undefined Message";
                }
}

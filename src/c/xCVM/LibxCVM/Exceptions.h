#pragma once
#define MALLOC_FAILED_ID 0x0001
#define MALLOC_FAILED_DESC "malloc() failed."
#define REALLOC_FAILED_ID 0x0002
#define REALLOC_FAILED_DESC "realloc() failed."
#define GL_MALLOC_FAILED_ID 0x0003
#define GL_MALLOC_FAILED_DESC "malloc() failed on initializing GenericList."
#define GL_HEAD_MALLOC_FAILED_ID 0x0004
#define GL_HEAD_MALLOC_FAILED_DESC "malloc() failed on initializing the HEAD of GenericList."
#define GL_REALLOC_FAILED_ID 0x0005
#define GL_REALLOC_FAILED_DESC "realloc() failed on adding item to GenericList."

#define CSTR_MALLOC_FAILED_ID 0x0006
#define CSTR_MALLOC_FAILED_DESC "malloc() failed on converting GenericList to CStr."
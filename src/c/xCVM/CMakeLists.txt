cmake_minimum_required (VERSION 3.0)
project("xCVM" C)
add_executable ("xCVM.VM" xcvm_vm/main.c corelib/xcore.c corelib/ter.c corelib/module.c corelib/errors.c "corelib/rtdata.c" "corelib/rtdata.h")

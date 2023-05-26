#!/bin/sh
if [ -z $CC ]
then
	export CC=gcc
	echo "Defaults CC to $CC"
else
	echo "CC=$CC"
fi
if [ ! -d "./bin/c/" ]
then
	mkdir ./bin/c
fi
$CC ./src/c/xCVM/xcvm_vm/*.c ./src/c/xCVM/corelib/*.c -o ./bin/c/xcvm_vm.exe

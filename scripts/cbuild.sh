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
OutputFile=./bin/c/xcvm_vm
Sys=$(uname)
if [[ Sys == *"Win"* ]]
then
	OutputFile=./bin/c/xcvm_vm.exe
fi
$CC ./src/c/xCVM/xcvm_vm/*.c ./src/c/xCVM/corelib/*.c -o $OutputFile

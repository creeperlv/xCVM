#!/bin/sh
dotnetinfo=$(type dotnet)
dotnetinfo="EXPAND$dotnetinfo"
if [ ! -z "${dotnetinfo##*"is"*}" ]
then
	echo ".NET not installed!"
	exit
fi
alias dotbuild="dotnet build --nologo -c Debug -v q -o ../../../bin/dotnet-debug/"
alias buildcli="dotnet build --nologo -c Debug -v q -o ../../../../../bin/dotnet-debug/"
if [ $# -gt 0 ]
then
	alias dotbuild="dotnet publish --nologo -c Debug -v q -r $1 -o ../../../bin/dotnet-debug/"
	alias buildcli="dotnet publish --nologo -c Debug -v q -r $1 -o ../../../../../bin/dotnet-debug/"
	echo "Build with publish, NativeAOT, DEBUG, targeting $1"
fi
cd ./src/dotNet/
cd xCVMc
dotbuild
cd ../xCVM.VM
dotbuild
cd ./../../
echo "Build Done."

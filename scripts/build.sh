#!/bin/sh
dotnetinfo=$(type dotnet)
dotnetinfo="EXPAND$dotnetinfo"
if [ ! -z "${dotnetinfo##*"is"*}" ]
then
	echo ".NET not installed!"
	exit
fi
alias dotbuild="dotnet build --nologo -c Release -v q -o ../../../bin/.net/"
alias buildcli="dotnet build --nologo -c Release -v q -o ../../../../../bin/.net/"
if [ $# -gt 0 ]
then
	alias dotbuild="dotnet publish --nologo -c Release -v q -r $1 -o ../../../bin/.net/"
	alias buildcli="dotnet publish --nologo -c Release -v q -r $1 -o ../../../../../bin/.net/"
	echo "Build with publish, NativeAOT, targeting $1"
fi
cd ./src/dotNet/
cd xCVMc
dotbuild
cd ../xCVM.VM
dotbuild
cd ../Cx/cli-tools
cd cxp
buildcli
cd ../cxhlc/
buildcli
cd ./../../../
echo "Build Done."
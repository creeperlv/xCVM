#!/bin/sh
alias dotbuild="dotnet build -c Release -o ../../../bin/"
alias buildcli="dotnet build -c Release -o ../../../../../bin/"
if [ $# -gt 0 ]
then
alias dotbuild="dotnet publish -c Release -r $1 -o ../../../bin/"
alias buildcli="dotnet publish -c Release -r $1 -o ../../../../../bin"
fi
cd ../src/dotNet/
cd xCVMc
dotbuild
cd ../xCVM.VM
dotbuild
cd ../Cx/cli-tools
cd cxp
buildcli
cd ../../../../
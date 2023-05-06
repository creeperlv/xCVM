#!/bin/sh
cd src/dotNet
cd xCVMc/
dotnet publish -r win-x64 -c Release -o ../../../bin/
cd ../xCVM.VM
dotnet publish -r win-x64 -c Release -o ../../../bin/
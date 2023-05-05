#!/bin/sh
cd src/dotNet
cd xCVMc/
dotnet publish -r linux-arm64 -c Release -o ../../../bin/
cd ../xCVM.VM
dotnet publish -r linux-arm64 -c Release -o ../../../bin/
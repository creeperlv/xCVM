# Build xCVM

## .NET Version

To build .Net version of xCVM and Cx.

In the project root run:
```
sh ./script/build.sh (rid)
```

`rid` is `Runtime ID` can be found in [.NET RID Catalog](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog). This parameter is optional, without it, script will use `dotnet build -c Release` instead of `dotnet publish -c Release -r $1`, resulting output managed dlls. When this parameter is specified, script will try to build NativeAOT version of all executables.

Note: NativeAOT has limited capability of cross-compiling.

Note: Try `busybox-w32` on windows to run the scripts.

### Projects will be build in .NET:

- xCVM.VM
- xCVMc
- cxp
- cxhlc

## C Version

To build C version of xCVM.

In the project root, run:

```
sh ./scripts/cbuild.sh
```

The C version should be compatible with gcc from DJGPP.

The build script uses $CC in Environment Variable as c compiler.

Thus, you may use:

```
export CC=x86_64-w64-mingw32-gcc
sh ./scripts/cbuild.sh
```

to use gcc from mingw32 to build the C version.
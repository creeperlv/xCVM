# Build xCVM

## .NET Version

To build .Net version of xCVM and Cx.

In `scripts` run:
```
./build.sh (rid)
```

`rid` is `Runtime ID` can be found in [.NET RID Catalog](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog). This parameter is optional, without it, script will use `dotnet build -c Release` instead of `dotnet publish -c Release -r $1`, resulting output managed dlls. When this parameter is specified, script will try to build NativeAOT version of all executables.

Note: NativeAOT has limited capability of cross-compiling.

Note: Try `busybox-w32` on windows to run the scripts.

## C Version

To build C version of xCVM.

In the project root, run:

```
sh ./scripts/DOS_BLD.SH
```

The C version should be compatible with gcc from DJGPP.


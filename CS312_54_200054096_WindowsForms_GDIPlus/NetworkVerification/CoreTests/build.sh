#!/bin/bash
# Compiles Network/Core + tests with Roslyn against the real .NET 8 reference pack; runs on the real .NET 8 runtime (real System.Text.Json, real sockets).
set -e
REF=$(ls -d /usr/lib/dotnet/packs/Microsoft.NETCore.App.Ref/8.*/ref/net8.0 | head -1)
CSC=$(ls /usr/lib/dotnet/sdk/8.*/Roslyn/bincore/csc.dll | head -1)
cd /tmp/coretests && mkdir -p out
dotnet $CSC -nologo -nostdlib+ -noconfig -target:exe -langversion:12 -nullable:enable -warn:4 \
   -out:out/CoreTests.dll $(for f in $REF/*.dll; do echo -n "-r:$f "; done) globals.cs /home/claude/InventoryManager/Network/Core/*.cs "$@" 2>&1 | sort -u
echo '{"runtimeOptions":{"tfm":"net8.0","framework":{"name":"Microsoft.NETCore.App","version":"8.0.0"}}}' > out/CoreTests.runtimeconfig.json

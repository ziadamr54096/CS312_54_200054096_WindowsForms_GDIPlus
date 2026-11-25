#!/bin/bash
# usage: build.sh <outname> <main-class> <extra-source-files...>
# Copies the REAL project sources (recursively), applies only .NET8->Framework API shims, compiles with Roslyn against Mono refs.
set -e
OUT=$1; MAIN=$2; shift 2
rm -rf /tmp/vauth/src && mkdir -p /tmp/vauth/src
cd /home/claude/InventoryManager && find . -name "*.cs" | while read f; do mkdir -p /tmp/vauth/src/$(dirname $f); cp $f /tmp/vauth/src/$f; done
cd /tmp/vauth/src
cp /tmp/vauth/h/*.cs . ; for f in "$@"; do cp $f .; done
sed -i 's/Math\.Clamp(/Compat.Clamp(/g' ReportsPanel.cs SettingsPanel.cs
find . -name "*.Designer.cs" -exec sed -i '/PlaceholderText/d' {} \;
sed -i 's/MessageBox\.Show(/Harness.Msg(/g' Ui.cs
sed -i 's/tcp.ConnectAsync(s.Host, s.Port, cts.Token)/tcp.ConnectAsync(s.Host, s.Port)/; s/reader.ReadLineAsync(cts.Token)/reader.ReadLineAsync()/; s/client.SendMailAsync(msg, cts.Token)/client.SendMailAsync(msg)/' AlertService.cs
R=/usr/lib/mono/4.8-api
REFS="-r:$R/mscorlib.dll -r:$R/System.dll -r:$R/System.Core.dll -r:$R/System.Data.dll -r:$R/System.Data.DataSetExtensions.dll -r:$R/System.Drawing.dll -r:$R/System.Windows.Forms.dll -r:$R/System.Xml.dll -r:$R/Mono.Data.Sqlite.dll $(for f in $R/Facades/*.dll; do echo -n "-r:$f "; done)"
dotnet $(ls /usr/lib/dotnet/sdk/8.*/Roslyn/bincore/csc.dll | head -1) -nologo -noconfig -nostdlib+ -target:exe -main:$MAIN -langversion:12 -nullable:enable -warn:4 -out:/tmp/vauth/$OUT $REFS $(find . -name "*.cs") 2>&1 | sort -u
cp /usr/lib/mono/4.5/Mono.Data.Sqlite.dll /tmp/vauth/ 2>/dev/null || true

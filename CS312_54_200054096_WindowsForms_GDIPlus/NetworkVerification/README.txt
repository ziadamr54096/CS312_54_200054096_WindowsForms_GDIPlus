Linux verification harness (NOT part of the Visual Studio project; never add these files to the .csproj).

CoreTests/   147 checks on InventoryManager/Network/Core compiled with Roslyn against the real .NET 8 reference pack and run on the
             real .NET 8 runtime (real System.Text.Json, real TCP sockets over loopback). build.sh CoreTests.cs ; dotnet out/CoreTests.dll

MonoHarness/ the WHOLE project compiled against the .NET Framework WinForms reference assemblies (Mono) with small shims
             (Shim.cs: Microsoft.Data.Sqlite over Mono's SQLite = a real database; Stj.cs: tiny System.Text.Json stand-in) and run under Xvfb:
             NetTests.cs        78 checks: stock-change watcher, DB gateway (UI-thread guarantee), 1 server + 2 clients end to end
             NetUiTests.cs      56 checks: the real Network tab + pop-up, three panels talking over TCP
             RegressionTests.cs 49 checks: existing screens/MainForm unchanged (now 7 tabs)
             AuthTests.cs 62 + AuthUiTests.cs 39 (+13 with arg "flow"): the login system still works with the new tab
Not a substitute for building on Windows with the real SDK, opening the Designer, and testing on two real computers.

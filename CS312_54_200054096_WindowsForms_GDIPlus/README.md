# Inventory Manager — CS312 (Windows Forms & GDI+)

C# / .NET 8 Windows Forms + SQLite inventory system.

## Open & run
1. Open `InventoryManager.sln` in Visual Studio 2022 (with the ".NET desktop development" workload).
2. Build once (NuGet restores `Microsoft.Data.Sqlite`), then press F5.

## This week's components
**1. Windows Forms Controls & Event Handling**
`MainForm` + one UserControl per tab (`DashboardPanel`, `ProductsPanel`, `SuppliersPanel`, `MovementsPanel`,
`ReportsPanel`, `SettingsPanel`, `Network/NetworkPanel`) and the Auth forms. Each has a `.cs` file (event handlers)
and a `.Designer.cs` file (controls/layout).

**2. GDI+ / Graphics / Visualization**
Custom-painted controls using `OnPaint` / `Graphics`: `KpiCard.cs`, `BarChart.cs`, `DonutChart.cs`
(used on the Dashboard), plus `Auth/BrandHeader.cs`.

See `InventoryManager/README.md` for the full requirement checklist.

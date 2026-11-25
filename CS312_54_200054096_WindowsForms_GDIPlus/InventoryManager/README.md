# Inventory Manager (CS 312 project)

C# / .NET 8 Windows Forms + SQLite. The database file is created automatically on first run
(`%LOCALAPPDATA%\InventoryManager\inventory.db`) and filled with sample data.

## Run
* Visual Studio 2022: open the folder / `InventoryManager.csproj`, press F5 (NuGet restores Microsoft.Data.Sqlite).
* Or in a terminal: `dotnet run` (needs the .NET 8 SDK on Windows).

## Requirement checklist
| # | Requirement | Where |
|---|-------------|-------|
| 1-2 | GUI / Windows Forms | `MainForm` + one `UserControl` per tab, each with its own `.Designer.cs` |
| 3 | C# | whole project |
| 4 | Good CRUD | `ProductsPanel`, `SuppliersPanel`, `MovementsPanel` |
| 5 | Database connectivity | `Db.cs` (SQLite, parameterised queries, transactions) |
| 6 | Reports / statistics | `ReportsPanel` (5 reports, total/avg/min/max), `DashboardPanel` KPIs |
| 7 | Exception handling | try/catch in every handler, `Ui.Error` friendly messages, global handlers in `Program.cs` |
| 8 | Validation | SKU regex, price > 0, e-mail/phone regex, stock-out <= stock, SMTP settings; DB CHECK/UNIQUE/FK too |
| 9 | Search / filter | live search + low/out-of-stock filter, column sorting, history search |
| 10 | GDI+ | `KpiCard.cs`, `BarChart.cs`, `DonutChart.cs`: custom `OnPaint` controls (Toolbox items) |
| 11 | Multithreading / async | dashboard loads via `Task.Run`; CSV export with progress; SMTP send/test are `async` |
| 12 | Networking | `AlertService.cs`: TCP connect + SMTP greeting check, SMTP low-stock e-mail |

## The error "Failed to enable constraints..."
This is `System.Data.ConstraintException`. It is raised when a `DataTable`/`DataSet` that has
constraints (primary key, unique, NOT NULL / AllowDBNull = false, foreign key - typically from a typed
DataSet `.xsd`) is filled with rows that violate them. The usual causes are a JOIN query that repeats the
key column, NULL values in a column marked not-null, or duplicate / orphan keys.

How this project avoids it: `Db.Query()` builds a plain `DataTable` with no constraints; integrity is
enforced by SQLite (UNIQUE, NOT NULL, FOREIGN KEY, CHECK) and by form validation, and violations are shown as
readable messages by `Ui.Error()`.

If you keep a DataSet/TableAdapter in another project, fix it with one of:
```csharp
ds.EnforceConstraints = false;      // before Fill()
adapter.Fill(ds.Products);
// find the offending rows instead of guessing:
foreach (DataRow r in ds.Products.GetErrors()) Console.WriteLine(r.RowError);
```
or in the designer: set the table's key / `AllowDBNull` to match the real data, and never mark the key
column of a JOINed table as a primary key.

## E-mail alerts (Settings tab)
Gmail: smtp.gmail.com, port 587, SSL on, and an **App Password** (2-Step Verification must be on).
The password is stored in the local database - fine for coursework, use a secret store for real deployments.

## Visual Studio Designer support
Every screen is a `partial` class split into two files:

| Component | Code (runtime logic) | Designer (layout, editable visually) |
|-----------|----------------------|--------------------------------------|
| Main window | `MainForm.cs` | `MainForm.Designer.cs` |
| Dashboard | `DashboardPanel.cs` | `DashboardPanel.Designer.cs` |
| Products | `ProductsPanel.cs` | `ProductsPanel.Designer.cs` |
| Suppliers | `SuppliersPanel.cs` | `SuppliersPanel.Designer.cs` |
| Stock Movements | `MovementsPanel.cs` | `MovementsPanel.Designer.cs` |
| Reports | `ReportsPanel.cs` | `ReportsPanel.Designer.cs` |
| Settings | `SettingsPanel.cs` | `SettingsPanel.Designer.cs` |
| KPI card / bar chart / donut chart | `KpiCard.cs`, `BarChart.cs`, `DonutChart.cs` | custom controls (Toolbox) |

Rules used: constructors only call `InitializeComponent()`; all database / subscription work happens in `OnLoad`
and is skipped when `Ui.InDesigner(this)` is true, so the Designer never needs a database.
To use the Designer: **Build the project first** (custom controls must be compiled), then double-click a `.cs`
file (or right-click > View Designer). Do not put code inside `InitializeComponent()`.
Not in the Designer by design: the report-name list in `ReportsPanel` (filled from `Defs` so it cannot drift from the SQL),
and grid columns (generated from the query at runtime).

## Sign-in and account management (Auth/)
Start-up flow: `Program.Main` -> `Db.Init()` -> `AuthSchema.Ensure()` -> `StartupFlow.Run()`:
**[first-run admin setup] -> LoginForm -> MainForm -> (Log out) -> LoginForm ...**

* New table `Users` only (`CREATE TABLE IF NOT EXISTS`); existing tables and data are never touched.
* Passwords: salted PBKDF2-HMAC-SHA256 (600,000 iterations, per-user random salt, constant-time compare); no plain text anywhere.
* No self-registration. Accounts are created only by an Admin (**Manage users**). The first Admin is created by the person who
  starts the app for the first time (setup form) - no default account or password exists. Setup is refused once an active Admin exists.
* Inactive accounts cannot sign in. 5 wrong passwords lock the account for 5 minutes.
* Admin-created / reset passwords are temporary: the user must choose their own at the next sign-in.
* Commands live in the main window's **system menu** (click the window icon / right-click the title bar / Alt+Space):
  *Signed in as ...*, *Change my password...*, *Manage users...* (admins only), *Log out*. MainForm itself is unchanged.
* Forms (LoginForm, FirstAdminSetupForm, ChangePasswordForm, UserManagementForm) and the BrandHeader control are all editable in the Designer.
* "Remember me" stores only the username in `%LOCALAPPDATA%\InventoryManager\last-user.txt`.
* Lost the only admin password? Close the app and delete the admin row from `Users` (e.g. with DB Browser for SQLite);
  the next start offers the first-run setup again.

## LAN networking (Network tab)
New **Network** tab: TCP client/server over the LAN (server on one PC, clients connect by IP + port), live device list, messaging,
broadcast, pop-up notifications, stock-update / low-stock alerts, client-to-server stock requests. See **docs/NETWORK_GUIDE.md**
(architecture, two-computer test plan, firewall setup, demonstration script).
All code is in `Network/` (protocol + sockets in `Network/Core`, which has no WinForms/database dependency).

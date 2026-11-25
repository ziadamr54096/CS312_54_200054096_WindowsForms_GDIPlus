using System.Data;
using System.Globalization;
using Microsoft.Data.Sqlite;

namespace InventoryManager;

/// <summary>
/// All database access. Query() returns a plain DataTable with NO primary key / unique / not-null
/// constraints, so the "Failed to enable constraints" ConstraintException can never be raised
/// when loading data (JOINs, NULLs and repeated values are all fine).
/// Integrity is enforced where it belongs: by SQLite itself (see Init) and by form validation.
/// </summary>
public static class Db
{
    static readonly string Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "InventoryManager");

    static readonly string ConnStr =
        new SqliteConnectionStringBuilder { DataSource = Path.Combine(Dir, "inventory.db") }.ToString();

    /// <summary>Raised on the calling (UI) thread after every successful write.</summary>
    public static event Action? DataChanged;

    static SqliteConnection Open()
    {
        var c = new SqliteConnection(ConnStr);
        c.Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = "PRAGMA foreign_keys = ON;";
        cmd.ExecuteNonQuery();
        return c;
    }

    static void Bind(SqliteCommand cmd, (string Name, object? Value)[] p)
    {
        foreach (var (name, value) in p)
            cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
    }

    // ---------- reads ----------

    public static DataTable Query(string sql, params (string Name, object? Value)[] p)
    {
        var rows = new List<object[]>();
        string[] names;
        using (var c = Open())
        using (var cmd = c.CreateCommand())
        {
            cmd.CommandText = sql;
            Bind(cmd, p);
            using var r = cmd.ExecuteReader();
            names = Enumerable.Range(0, r.FieldCount).Select(r.GetName).ToArray();
            while (r.Read())
            {
                var a = new object[r.FieldCount];
                r.GetValues(a);
                rows.Add(a);
            }
        }

        var dt = new DataTable();
        for (int i = 0; i < names.Length; i++)
        {
            var types = rows.Select(x => x[i]).Where(v => v is not DBNull).Select(v => v.GetType()).Distinct().ToList();
            dt.Columns.Add(names[i], PickType(types));
        }
        foreach (var a in rows)
        {
            var vals = new object[a.Length];
            for (int i = 0; i < a.Length; i++)
                vals[i] = a[i] is DBNull ? DBNull.Value : Convert.ChangeType(a[i], dt.Columns[i].DataType, CultureInfo.InvariantCulture);
            dt.Rows.Add(vals);
        }
        return dt;
    }

    static Type PickType(List<Type> t)
    {
        if (t.Count == 0) return typeof(string);
        if (t.Count == 1) return t[0];
        if (t.All(x => x == typeof(long) || x == typeof(double))) return typeof(double);
        return typeof(string);
    }

    public static T Scalar<T>(string sql, params (string Name, object? Value)[] p)
    {
        using var c = Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = sql;
        Bind(cmd, p);
        var o = cmd.ExecuteScalar();
        if (o == null || o is DBNull) return default!;
        return (T)Convert.ChangeType(o, typeof(T), CultureInfo.InvariantCulture);
    }

    // ---------- writes (always inside a transaction) ----------

    public static int Execute(string sql, params (string Name, object? Value)[] p) => Exec(true, sql, p);

    static int Exec(bool notify, string sql, (string Name, object? Value)[] p)
    {
        int n;
        using (var c = Open())
        using (var tx = c.BeginTransaction())
        {
            using var cmd = c.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = sql;
            Bind(cmd, p);
            n = cmd.ExecuteNonQuery();
            tx.Commit();
        }
        if (notify) DataChanged?.Invoke();
        return n;
    }

    // ---------- settings ----------

    public static string GetSetting(string key, string fallback = "")
    {
        var v = Scalar<string>("SELECT SettingValue FROM Settings WHERE SettingKey = @k", ("@k", key));
        return string.IsNullOrEmpty(v) ? fallback : v;
    }

    public static void SetSetting(string key, string value) =>
        Exec(false, @"INSERT INTO Settings(SettingKey, SettingValue) VALUES(@k, @v)
                      ON CONFLICT(SettingKey) DO UPDATE SET SettingValue = excluded.SettingValue",
             new (string, object?)[] { ("@k", key), ("@v", value) });

    public static string Now() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

    // ---------- schema ----------

    public static void Init()
    {
        Directory.CreateDirectory(Dir);

        Execute(@"
CREATE TABLE IF NOT EXISTS Suppliers (
    Id      INTEGER PRIMARY KEY AUTOINCREMENT,
    Name    TEXT NOT NULL UNIQUE,
    Phone   TEXT,
    Email   TEXT,
    Address TEXT
);
CREATE TABLE IF NOT EXISTS Products (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
    Sku          TEXT NOT NULL UNIQUE,
    Name         TEXT NOT NULL,
    Category     TEXT,
    SupplierId   INTEGER REFERENCES Suppliers(Id) ON DELETE SET NULL,
    UnitPrice    REAL NOT NULL DEFAULT 0 CHECK (UnitPrice >= 0),
    Quantity     INTEGER NOT NULL DEFAULT 0 CHECK (Quantity >= 0),
    ReorderLevel INTEGER NOT NULL DEFAULT 5 CHECK (ReorderLevel >= 0)
);
CREATE TABLE IF NOT EXISTS StockMovements (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductId INTEGER NOT NULL REFERENCES Products(Id) ON DELETE CASCADE,
    Delta     INTEGER NOT NULL,
    Reason    TEXT,
    CreatedAt TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS Settings (
    SettingKey   TEXT PRIMARY KEY,
    SettingValue TEXT
);");

        if (Scalar<long>("SELECT COUNT(*) FROM Suppliers") == 0 && Scalar<long>("SELECT COUNT(*) FROM Products") == 0)
            Seed();
    }

    static void Seed()
    {
        Execute(@"
INSERT INTO Suppliers(Name, Phone, Email, Address) VALUES
 ('Delta Electronics',   '+20 3 555 0101', 'sales@delta-electronics.example', 'Alexandria'),
 ('Nile Office Supplies','+20 2 555 0202', 'orders@nile-office.example',     'Cairo'),
 ('Fresh Foods Co.',     '+20 3 555 0303', 'info@freshfoods.example',        'Alexandria');
INSERT INTO Products(Sku, Name, Category, SupplierId, UnitPrice, Quantity, ReorderLevel) VALUES
 ('EL-001','USB-C Cable',          'Electronics',1, 5.50,120,30),
 ('EL-002','Wireless Mouse',       'Electronics',1,14.99,  8,10),
 ('EL-003','Bluetooth Speaker',    'Electronics',1,39.00,  0, 5),
 ('EL-004','Phone Charger 20W',    'Electronics',1,12.75, 45,15),
 ('OF-001','A4 Paper Ream',        'Office',     2, 4.20,200,50),
 ('OF-002','Ballpoint Pens (box)', 'Office',     2, 3.10, 18,20),
 ('OF-003','Stapler',              'Office',     2, 6.80, 35,10),
 ('OF-004','Whiteboard Markers',   'Office',     2, 7.50,  4,10),
 ('FD-001','Olive Oil 1L',         'Food',       3, 9.90, 60,20),
 ('FD-002','Instant Coffee 200g',  'Food',       3, 6.25, 25,15),
 ('FD-003','Green Tea (box)',      'Food',       3, 2.80,  0,10),
 ('FD-004','Dates 500g',           'Food',       3, 3.40, 70,25);
INSERT INTO StockMovements(ProductId, Delta, Reason, CreatedAt)
 SELECT Id, Quantity, 'Initial stock', datetime('now','localtime') FROM Products WHERE Quantity > 0;");
    }
}

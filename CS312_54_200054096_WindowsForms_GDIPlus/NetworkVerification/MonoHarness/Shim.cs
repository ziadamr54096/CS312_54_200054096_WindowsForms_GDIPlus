// HARNESS ONLY. Global usings (ImplicitUsings), .NET8-only API stand-ins, and Microsoft.Data.Sqlite over Mono.Data.Sqlite (REAL SQLite engine).
global using System;
global using System.IO;
global using System.Linq;
global using System.Collections.Generic;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Drawing;
global using System.Windows.Forms;

namespace System.Runtime.CompilerServices { internal static class IsExternalInit { } }
namespace System.Windows.Forms { public static class ApplicationConfiguration { public static void Initialize() { } } }
public static class Compat { public static int Clamp(int v, int lo, int hi) => v < lo ? lo : v > hi ? hi : v; }
public static class Harness
{
    public static bool ForbidDb; public static bool DbTouched; public static List<string> Messages = new();
    public static void Touch() { DbTouched = true; if (ForbidDb) throw new InvalidOperationException("DATABASE ACCESSED AT DESIGN TIME"); }
    public static DialogResult Msg(string text, string caption, MessageBoxButtons b, MessageBoxIcon i) { Messages.Add(i + ": " + text); return DialogResult.Yes; }
}
namespace Microsoft.Data.Sqlite
{
    using M = Mono.Data.Sqlite;
    public class SqliteException : Exception { public int SqliteErrorCode => 0; }
    public class SqliteConnectionStringBuilder
    {
        public string DataSource { get; set; } = "";
        public override string ToString() => "Data Source=" + DataSource + ";Version=3;";
    }
    public class SqliteParameterCollection
    {
        readonly M.SqliteCommand _c; internal SqliteParameterCollection(M.SqliteCommand c) { _c = c; }
        public void AddWithValue(string n, object v) => _c.Parameters.AddWithValue(n, v);
    }
    public class SqliteDataReader : IDisposable
    {
        readonly M.SqliteDataReader _r; internal SqliteDataReader(M.SqliteDataReader r) { _r = r; }
        public int FieldCount => _r.FieldCount; public string GetName(int i) => _r.GetName(i); public bool Read() => _r.Read();
        public int GetValues(object[] a) => _r.GetValues(a); public void Dispose() => _r.Dispose();
    }
    public class SqliteTransaction : IDisposable
    {
        internal readonly M.SqliteTransaction T; internal SqliteTransaction(M.SqliteTransaction t) { T = t; }
        public void Commit() => T.Commit(); public void Dispose() => T.Dispose();
    }
    public class SqliteCommand : IDisposable
    {
        internal readonly M.SqliteCommand C; public SqliteParameterCollection Parameters;
        internal SqliteCommand(M.SqliteCommand c) { C = c; Parameters = new SqliteParameterCollection(c); }
        public string CommandText { get => C.CommandText; set => C.CommandText = value; }
        public SqliteTransaction? Transaction { set => C.Transaction = value?.T; }
        public int ExecuteNonQuery() => C.ExecuteNonQuery(); public object? ExecuteScalar() => C.ExecuteScalar();
        public SqliteDataReader ExecuteReader() => new SqliteDataReader(C.ExecuteReader()); public void Dispose() => C.Dispose();
    }
    public class SqliteConnection : IDisposable
    {
        readonly M.SqliteConnection _c;
        public SqliteConnection(string s) { Harness.Touch(); _c = new M.SqliteConnection(s); }
        public void Open() => _c.Open();
        public SqliteCommand CreateCommand() => new SqliteCommand(_c.CreateCommand());
        public SqliteTransaction BeginTransaction() => new SqliteTransaction(_c.BeginTransaction());
        public void Dispose() => _c.Dispose();
    }
}

namespace InventoryManager.Auth;

/// <summary>
/// Adds the authentication table next to the existing ones. Uses CREATE TABLE IF NOT EXISTS, so it is safe to run on every start
/// and never alters, drops or rewrites Suppliers, Products, StockMovements or Settings.
/// </summary>
public static class AuthSchema
{
    public static void Ensure()
    {
        Db.Execute(@"
CREATE TABLE IF NOT EXISTS Users (
    Id                 INTEGER PRIMARY KEY AUTOINCREMENT,
    Username           TEXT NOT NULL UNIQUE COLLATE NOCASE,
    FullName           TEXT,
    PasswordHash       TEXT NOT NULL,
    Role               TEXT NOT NULL DEFAULT 'User' CHECK (Role IN ('Admin','User')),
    IsActive           INTEGER NOT NULL DEFAULT 1 CHECK (IsActive IN (0,1)),
    MustChangePassword INTEGER NOT NULL DEFAULT 0 CHECK (MustChangePassword IN (0,1)),
    FailedAttempts     INTEGER NOT NULL DEFAULT 0,
    LockedUntil        TEXT,
    CreatedAt          TEXT NOT NULL,
    CreatedBy          TEXT,
    LastLoginAt        TEXT
);");
    }
}

using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using InventoryManager;
using InventoryManager.Auth;

public static class AuthTests
{
    static int pass, fail;
    static void Check(bool ok, string msg) { Console.WriteLine((ok ? "  PASS  " : "  FAIL  ") + msg); if (ok) pass++; else fail++; }
    static string Msg<T>(Action a) where T : Exception { try { a(); return "<no exception>"; } catch (T ex) { return ex.Message; } catch (Exception ex) { return "WRONG TYPE " + ex.GetType().Name + ": " + ex.Message; } }
    static bool Throws<T>(Action a) where T : Exception => !Msg<T>(a).StartsWith("<no") && !Msg<T>(a).StartsWith("WRONG");

    static long Fixture(string user, string password, string role = "User", bool active = true, bool must = false, int iters = 10000)
    {
        Db.Execute(@"INSERT INTO Users(Username,FullName,PasswordHash,Role,IsActive,MustChangePassword,CreatedAt,CreatedBy)
                     VALUES(@u,@u,@h,@r,@a,@m,@now,'fixture')",
            ("@u", user), ("@h", PasswordHasher.Hash(password, iters)), ("@r", role), ("@a", active ? 1 : 0), ("@m", must ? 1 : 0), ("@now", Db.Now()));
        return Db.Scalar<long>("SELECT Id FROM Users WHERE Username=@u", ("@u", user));
    }
    static AuthUser U(long id) => AuthService.GetById(id)!;

    static string Snap(string table)
    {
        var dt = Db.Query($"SELECT * FROM {table} ORDER BY 1");
        var sb = new StringBuilder();
        foreach (DataRow r in dt.Rows) sb.AppendLine(string.Join("|", r.ItemArray.Select(v => Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture))));
        using var sha = SHA256.Create();
        return dt.Rows.Count + " rows, sha256 " + BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()))).Replace("-", "").Substring(0, 16);
    }
    static List<string> SchemaObjects() => Db.Query("SELECT name FROM sqlite_master ORDER BY name").AsEnumerable().Select(r => Convert.ToString(r[0])!).ToList();
    static string SchemaSql(string t) => Convert.ToString(Db.Scalar<string>("SELECT sql FROM sqlite_master WHERE name=@n", ("@n", t)))!;

    public static int Main()
    {
        string dataHome = "/tmp/vauth/data_logic_" + Guid.NewGuid().ToString("N").Substring(0, 6);
        Environment.SetEnvironmentVariable("XDG_DATA_HOME", dataHome);

        Console.WriteLine("== A. Simulate the user's EXISTING database (created by the unchanged Db.Init) and snapshot it ==");
        Db.Init();
        Db.Execute("INSERT INTO Suppliers(Name,Phone) VALUES('Acme Test Co','+20 100')");
        Db.Execute("INSERT INTO Products(Sku,Name,Category,SupplierId,UnitPrice,Quantity,ReorderLevel) VALUES('ZZ-900','Custom item','Custom',4,1.25,7,3)");
        Db.Execute("INSERT INTO StockMovements(ProductId,Delta,Reason,CreatedAt) VALUES(13,-2,'Custom sale','2026-01-01 10:00:00')");
        Db.SetSetting("SmtpHost", "smtp.example.org");
        string[] tables = { "Suppliers", "Products", "StockMovements", "Settings" };
        var before = tables.ToDictionary(t => t, Snap);
        var sqlBefore = tables.ToDictionary(t => t, SchemaSql);
        var objBefore = SchemaObjects();
        Check(before["Suppliers"].StartsWith("4 rows") && before["Products"].StartsWith("13 rows") && before["StockMovements"].StartsWith("11 rows") && before["Settings"].StartsWith("1 rows"),
              "snapshot taken: " + string.Join("; ", before.Select(kv => kv.Key + " " + kv.Value.Split(',')[0])));

        AuthSchema.Ensure(); AuthSchema.Ensure();    // idempotent
        Check(Db.Scalar<long>("SELECT COUNT(*) FROM Users") == 0, "fresh Users table is EMPTY: no built-in/default account exists");
        Check(!AuthService.HasActiveAdmin(), "HasActiveAdmin() is false on a database that has no accounts (-> first-run setup is offered)");

        Console.WriteLine("== B. Password hashing ==");
        var sw = Stopwatch.StartNew(); string h1 = PasswordHasher.Hash("Str0ng!Pass1"); sw.Stop();
        string h2 = PasswordHasher.Hash("Str0ng!Pass1");
        Check(h1.StartsWith("PBKDF2-SHA256$600000$") && h1.Split('$').Length == 4, $"format PBKDF2-SHA256$iterations$salt$hash (hash took {sw.ElapsedMilliseconds} ms on this test machine/Mono)");
        Check(h1 != h2, "same password hashed twice gives different results (random salt)");
        Check(PasswordHasher.Verify("Str0ng!Pass1", h1, out bool rh) && !rh, "correct password verifies");
        Check(!PasswordHasher.Verify("Str0ng!Pass2", h1, out _) && !PasswordHasher.Verify("", h1, out _), "wrong / empty password does not verify");
        bool garbageOk = true;
        foreach (var bad in new[] { "", "x", "a$b$c$d", "PBKDF2-SHA256$1$AAAAAAAAAAAAAAAA$AAAAAAAAAAAAAAAAAAAAAAAA", "PBKDF2-SHA256$600000$!!!$!!!", "PBKDF2-SHA256$99999999999$AA$BB" })
            try { garbageOk &= !PasswordHasher.Verify("x", bad, out _); } catch { garbageOk = false; }
        Check(garbageOk, "corrupted / tampered stored hashes are rejected without throwing");

        Console.WriteLine("== C. Password policy ==");
        Check(PasswordPolicy.Validate("short1", "u") != null && PasswordPolicy.Validate("onlyletterss", "u") != null && PasswordPolicy.Validate("12345678", "u") != null
              && PasswordPolicy.Validate("owner12345", "owner") != null && PasswordPolicy.Validate("Password", "u") != null, "weak passwords rejected (short, no digit, no letter, contains username, common)");
        Check(PasswordPolicy.Validate("Str0ng!Pass1", "owner") == null, "strong password accepted");
        Check(PasswordPolicy.ValidateUsername("ab") != null && PasswordPolicy.ValidateUsername("bad name") != null && PasswordPolicy.ValidateUsername("ok.user-1") == null, "username rules");
        bool allGood = true; var seen = new HashSet<string>();
        for (int i = 0; i < 300; i++) { var g = PasswordPolicy.Generate(); allGood &= PasswordPolicy.Validate(g, "x") == null && g.Length >= 12; seen.Add(g); }
        Check(allGood && seen.Count == 300, "300 generated temporary passwords all satisfy the policy and are unique");

        Console.WriteLine("== D. First-administrator provisioning (no hardcoded password) ==");
        Check(Throws<InvalidOperationException>(() => AuthService.ProvisionFirstAdmin("owner", "", "weak")), "weak password refused during setup");
        Check(AuthService.ProvisionFirstAdmin("owner", "Store Owner", "Str0ng!Pass1"), "first admin created with the password the operator chose");
        Check(!AuthService.ProvisionFirstAdmin("intruder", "", "Str0ng!Pass1"), "setup CANNOT be used again once an active admin exists (no backdoor admin creation)");
        Check(Db.Scalar<long>("SELECT COUNT(*) FROM Users") == 1, "exactly one account exists");
        string stored = Db.Scalar<string>("SELECT PasswordHash FROM Users WHERE Username='owner'");
        Check(stored.StartsWith("PBKDF2-SHA256$") && !stored.Contains("Str0ng"), "stored value is a hash, not the password");
        byte[] dbBytes = File.ReadAllBytes(Path.Combine(dataHome, "InventoryManager", "inventory.db"));
        Check(!Encoding.UTF8.GetString(dbBytes).Contains("Str0ng!Pass1"), "the plain-text password appears nowhere in the database file");
        long ownerId = Db.Scalar<long>("SELECT Id FROM Users WHERE Username='owner'");

        Console.WriteLine("== E. Authentication ==");
        var ok = AuthService.Authenticate("owner", "Str0ng!Pass1");
        Check(ok.Status == LoginStatus.Success && ok.User!.IsAdmin && ok.User.Username == "owner", "valid credentials -> Success (admin)");
        Check(AuthService.Authenticate("OWNER", "Str0ng!Pass1").Status == LoginStatus.Success, "username is case-insensitive");
        Check(Db.Scalar<string>("SELECT LastLoginAt FROM Users WHERE Id=@i", ("@i", ownerId)) != null, "LastLoginAt recorded");
        var wrong = AuthService.Authenticate("owner", "Wrong!Pass9");
        var unknown = AuthService.Authenticate("nobody", "Wrong!Pass9");
        Check(wrong.Status == LoginStatus.InvalidCredentials && wrong.User == null, "wrong password -> InvalidCredentials, no user returned");
        Check(unknown.Status == LoginStatus.InvalidCredentials && unknown.Message == wrong.Message, "unknown user gets the IDENTICAL message (no account enumeration)");
        Check(AuthService.Authenticate("", "x").Status == LoginStatus.InvalidCredentials && AuthService.Authenticate("owner", "").Status == LoginStatus.InvalidCredentials, "empty username/password rejected");
        long usersBefore = Db.Scalar<long>("SELECT COUNT(*) FROM Users");
        Check(AuthService.Authenticate("owner' OR '1'='1", "x").Status == LoginStatus.InvalidCredentials
              && AuthService.Authenticate("owner", "' OR 1=1 --").Status == LoginStatus.InvalidCredentials
              && AuthService.Authenticate("x'; DROP TABLE Users; --", "x").Status == LoginStatus.InvalidCredentials
              && Db.Scalar<long>("SELECT COUNT(*) FROM Users") == usersBefore, "SQL-injection strings are rejected and the Users table is intact");

        long spaced = Fixture("spacey", " Sp4ced pass ");
        Check(AuthService.Authenticate("spacey", " Sp4ced pass ").Status == LoginStatus.Success && AuthService.Authenticate("spacey", "Sp4ced pass").Status == LoginStatus.InvalidCredentials, "passwords are not trimmed (exact match required)");

        Console.WriteLine("== F. Inactive accounts ==");
        long bob = Fixture("bob", "B0b!Secret", active: false);
        var inactive = AuthService.Authenticate("bob", "B0b!Secret");
        Check(inactive.Status == LoginStatus.Inactive && inactive.User == null && inactive.Message.Contains("deactivated"), "inactive account with the CORRECT password cannot log in");
        Check(AuthService.Authenticate("bob", "nope").Message == wrong.Message, "inactive account with a WRONG password gets the generic message (existence not revealed)");
        Check(Db.Scalar<string>("SELECT LastLoginAt FROM Users WHERE Id=@i", ("@i", bob)) == null, "no sign-in is recorded for the inactive account");

        Console.WriteLine("== G. Lock-out after repeated failures ==");
        long lockMe = Fixture("lockme", "L0ck!Me123");
        for (int i = 0; i < AuthService.MaxFailedAttempts; i++) AuthService.Authenticate("lockme", "bad" + i);
        var locked = AuthService.Authenticate("lockme", "L0ck!Me123");
        Check(locked.Status == LoginStatus.Locked && locked.Message.Contains("minute"), "after 5 failures even the CORRECT password is refused while locked");
        Db.Execute("UPDATE Users SET LockedUntil='2000-01-01 00:00:00' WHERE Id=@i", ("@i", lockMe));
        Check(AuthService.Authenticate("lockme", "L0ck!Me123").Status == LoginStatus.Success && Db.Scalar<long>("SELECT FailedAttempts FROM Users WHERE Id=@i", ("@i", lockMe)) == 0, "lock expires; success resets the counters");

        Console.WriteLine("== H. Hash upgrade ==");
        long old = Fixture("oldhash", "0ld!Hash123", iters: 10000);
        Check(AuthService.Authenticate("oldhash", "0ld!Hash123").Status == LoginStatus.Success
              && Db.Scalar<string>("SELECT PasswordHash FROM Users WHERE Id=@i", ("@i", old)).StartsWith("PBKDF2-SHA256$600000$"), "hash with fewer iterations is upgraded transparently at login");

        Console.WriteLine("== I. Only an authorised Admin can manage accounts ==");
        long aliceId = Fixture("alice", "Al1ce!Pass99");
        var admin = U(ownerId); var alice = U(aliceId);
        Check(Throws<UnauthorizedAccessException>(() => UserService.Create(alice, "mallory", "", UserRole.Admin, "Str0ng!Pass2", false)), "regular user CANNOT create accounts");
        Check(Throws<UnauthorizedAccessException>(() => UserService.List(alice)) && Throws<UnauthorizedAccessException>(() => UserService.SetActive(alice, bob, true))
              && Throws<UnauthorizedAccessException>(() => UserService.ResetPassword(alice, ownerId, "Str0ng!Pass2")) && Throws<UnauthorizedAccessException>(() => UserService.SetRole(alice, aliceId, UserRole.Admin)),
              "regular user cannot list / activate / reset / change roles");
        var forged = alice with { Role = UserRole.Admin };
        Check(Throws<UnauthorizedAccessException>(() => UserService.Create(forged, "mallory", "", UserRole.Admin, "Str0ng!Pass2", false)), "a forged in-memory 'Admin' object is rejected (role re-checked in the database)");
        Check(Throws<UnauthorizedAccessException>(() => UserService.Create(null, "mallory", "", UserRole.User, "Str0ng!Pass2", false)), "no signed-in user -> refused");
        Check(Db.Scalar<long>("SELECT COUNT(*) FROM Users WHERE Username='mallory'") == 0, "no unauthorised account was created");

        UserService.Create(admin, "carol", "Carol C", UserRole.User, "T3mp!Pass77", mustChange: true);
        Check(Db.Scalar<long>("SELECT COUNT(*) FROM Users WHERE Username='carol'") == 1, "admin can create an account");
        Check(Msg<InvalidOperationException>(() => UserService.Create(admin, "CAROL", "", UserRole.User, "T3mp!Pass78", false)).Contains("taken"), "duplicate username refused (case-insensitive)");
        Check(Throws<InvalidOperationException>(() => UserService.Create(admin, "dave", "", UserRole.User, "weak", false)) && Throws<InvalidOperationException>(() => UserService.Create(admin, "d", "", UserRole.User, "T3mp!Pass77", false)), "weak password / invalid username refused by the service");
        var carolLogin = AuthService.Authenticate("carol", "T3mp!Pass77");
        Check(carolLogin.Status == LoginStatus.Success && carolLogin.User!.MustChangePassword && !carolLogin.User.IsAdmin, "new account signs in and is flagged 'must change password'");

        var afterChange = UserService.ChangeOwnPassword(carolLogin.User!, "T3mp!Pass77", "My0wn!Secret5");
        Check(!afterChange.MustChangePassword && AuthService.Authenticate("carol", "T3mp!Pass77").Status == LoginStatus.InvalidCredentials && AuthService.Authenticate("carol", "My0wn!Secret5").Status == LoginStatus.Success, "user changes own password; old one stops working");
        Check(Msg<InvalidOperationException>(() => UserService.ChangeOwnPassword(afterChange, "WRONG", "An0ther!Pass1")).Contains("current password"), "wrong current password refused");

        UserService.ResetPassword(admin, aliceId, "Reset!Pass42");
        Check(AuthService.Authenticate("alice", "Al1ce!Pass99").Status == LoginStatus.InvalidCredentials && AuthService.Authenticate("alice", "Reset!Pass42").User!.MustChangePassword, "admin reset: old password dead, new one is temporary (must change)");

        Check(Msg<InvalidOperationException>(() => UserService.SetActive(admin, ownerId, false)).Contains("own account"), "admin cannot deactivate themselves");
        Check(Msg<InvalidOperationException>(() => UserService.SetRole(admin, ownerId, UserRole.User)).Contains("own administrator"), "admin cannot demote themselves");
        UserService.SetActive(admin, aliceId, false);
        Check(AuthService.Authenticate("alice", "Reset!Pass42").Status == LoginStatus.Inactive, "admin deactivates a user -> they can no longer sign in");
        UserService.SetActive(admin, aliceId, true);
        Check(AuthService.Authenticate("alice", "Reset!Pass42").Status == LoginStatus.Success, "admin re-activates the user -> sign-in works again");
        UserService.SetRole(admin, aliceId, UserRole.Admin);
        Check(U(aliceId).IsAdmin, "admin can promote a user");
        UserService.SetActive(U(aliceId), ownerId, false);              // alice (now admin) deactivates owner
        Check(Throws<UnauthorizedAccessException>(() => UserService.Create(admin, "ghost2", "", UserRole.User, "Str0ng!Pass2", false)), "a deactivated admin immediately loses the right to manage accounts (stale session)");
        Check(Msg<InvalidOperationException>(() => UserService.SetActive(U(aliceId), aliceId, false)).Contains("own account"), "the last active admin cannot be deactivated");
        Check(Db.Scalar<long>("SELECT COUNT(*) FROM Users WHERE Role='Admin' AND IsActive=1") >= 1, "at least one active administrator always remains");

        Console.WriteLine("== J. Existing data and schema are untouched ==");
        bool same = true;
        foreach (var t in tables) { bool s = Snap(t) == before[t]; same &= s; Check(s, $"{t}: rows unchanged ({before[t]})"); }
        foreach (var t in tables) Check(SchemaSql(t) == sqlBefore[t], $"{t}: table definition unchanged");
        var added = SchemaObjects().Except(objBefore).ToList();
        Check(added.Count == 2 && added.Contains("Users") && added.Contains("sqlite_autoindex_Users_1"), "the ONLY new database objects are: " + string.Join(", ", added.OrderBy(x => x, StringComparer.Ordinal)));
        Check(objBefore.All(SchemaObjects().Contains), "no existing object was removed");

        Console.WriteLine($"\nRESULT: {pass} passed, {fail} failed");
        return fail == 0 ? 0 : 1;
    }
}

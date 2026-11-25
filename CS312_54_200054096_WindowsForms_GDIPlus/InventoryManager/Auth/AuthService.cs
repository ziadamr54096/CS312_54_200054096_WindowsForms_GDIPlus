using System.Data;
using System.Globalization;

namespace InventoryManager.Auth;

/// <summary>Credential checking and first-administrator provisioning. All SQL is parameterised and goes through the existing Db class.</summary>
public static class AuthService
{
    public const int MaxFailedAttempts = 5;
    public static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(5);

    const string GenericFailure = "Invalid username or password.";
    static readonly Lazy<string> DummyHash = new(() => PasswordHasher.Hash("not-a-real-password"));

    /// <summary>True when at least one ACTIVE administrator exists. When false, the first-run setup is offered.</summary>
    public static bool HasActiveAdmin() =>
        Db.Scalar<long>("SELECT COUNT(*) FROM Users WHERE Role = 'Admin' AND IsActive = 1") > 0;

    public static LoginResult Authenticate(string? username, string? password)
    {
        username = (username ?? "").Trim();
        password ??= "";
        if (username.Length == 0 || password.Length == 0)
            return new LoginResult(LoginStatus.InvalidCredentials, null, GenericFailure);

        DataTable dt = Db.Query(
            @"SELECT Id, Username, FullName, PasswordHash, Role, IsActive, MustChangePassword, FailedAttempts, LockedUntil
              FROM Users WHERE Username = @u", ("@u", username));

        if (dt.Rows.Count == 0)
        {
            PasswordHasher.Verify(password, DummyHash.Value, out _);   // same work as a real check: no timing hint that the user is unknown
            return new LoginResult(LoginStatus.InvalidCredentials, null, GenericFailure);
        }

        DataRow r = dt.Rows[0];
        long id = Convert.ToInt64(r["Id"]);

        if (r["LockedUntil"] is not DBNull && TryParse(Convert.ToString(r["LockedUntil"]), out DateTime until) && until > DateTime.Now)
        {
            int mins = Math.Max(1, (int)Math.Ceiling((until - DateTime.Now).TotalMinutes));
            return new LoginResult(LoginStatus.Locked, null, $"Too many failed attempts. Try again in {mins} minute(s).");
        }

        bool ok = PasswordHasher.Verify(password, Convert.ToString(r["PasswordHash"]) ?? "", out bool rehash);
        if (!ok)
        {
            Db.Execute(@"UPDATE Users
                         SET FailedAttempts = CASE WHEN FailedAttempts + 1 >= @max THEN 0 ELSE FailedAttempts + 1 END,
                             LockedUntil    = CASE WHEN FailedAttempts + 1 >= @max THEN @until ELSE LockedUntil END
                         WHERE Id = @id",
                       ("@max", MaxFailedAttempts), ("@until", (DateTime.Now + LockDuration).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)), ("@id", id));
            return new LoginResult(LoginStatus.InvalidCredentials, null, GenericFailure);
        }

        // The password is correct, but a deactivated account must never get in.
        if (Convert.ToInt64(r["IsActive"]) != 1)
            return new LoginResult(LoginStatus.Inactive, null, "This account has been deactivated. Please contact your administrator.");

        if (rehash)
            Db.Execute("UPDATE Users SET PasswordHash = @h WHERE Id = @id", ("@h", PasswordHasher.Hash(password)), ("@id", id));
        Db.Execute("UPDATE Users SET FailedAttempts = 0, LockedUntil = NULL, LastLoginAt = @now WHERE Id = @id",
                   ("@now", Db.Now()), ("@id", id));

        return new LoginResult(LoginStatus.Success, FromRow(r), "");
    }

    /// <summary>
    /// Creates the very first administrator. Succeeds only while NO active administrator exists (checked atomically in the INSERT),
    /// so it cannot be used later to add accounts. The password is chosen by the person running setup - nothing is hardcoded.
    /// </summary>
    public static bool ProvisionFirstAdmin(string username, string fullName, string password)
    {
        string? err = PasswordPolicy.ValidateUsername(username) ?? PasswordPolicy.Validate(password, username);
        if (err != null) throw new InvalidOperationException(err);

        int n = Db.Execute(@"
            INSERT INTO Users(Username, FullName, PasswordHash, Role, IsActive, MustChangePassword, CreatedAt, CreatedBy)
            SELECT @u, @n, @h, 'Admin', 1, 0, @now, 'first-run setup'
            WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Role = 'Admin' AND IsActive = 1)",
            ("@u", username.Trim()), ("@n", string.IsNullOrWhiteSpace(fullName) ? null : fullName.Trim()),
            ("@h", PasswordHasher.Hash(password)), ("@now", Db.Now()));
        return n == 1;
    }

    public static AuthUser? GetById(long id)
    {
        DataTable dt = Db.Query("SELECT Id, Username, FullName, Role, IsActive, MustChangePassword FROM Users WHERE Id = @id", ("@id", id));
        return dt.Rows.Count == 0 ? null : FromRow(dt.Rows[0]);
    }

    internal static AuthUser FromRow(DataRow r) => new(
        Convert.ToInt64(r["Id"]),
        Convert.ToString(r["Username"]) ?? "",
        r["FullName"] is DBNull ? "" : Convert.ToString(r["FullName"]) ?? "",
        string.Equals(Convert.ToString(r["Role"]), "Admin", StringComparison.OrdinalIgnoreCase) ? UserRole.Admin : UserRole.User,
        Convert.ToInt64(r["IsActive"]) == 1,
        Convert.ToInt64(r["MustChangePassword"]) == 1);

    static bool TryParse(string? s, out DateTime dt) =>
        DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
}

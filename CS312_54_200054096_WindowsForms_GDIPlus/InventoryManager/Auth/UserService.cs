using System.Data;

namespace InventoryManager.Auth;

/// <summary>
/// Account management. EVERY method except ChangeOwnPassword requires an administrator, and that is re-checked against the
/// database on each call (so a stale or tampered session cannot manage accounts). There is deliberately no self-registration API.
/// </summary>
public static class UserService
{
    static void RequireAdmin(AuthUser? actor)
    {
        if (actor == null) throw new UnauthorizedAccessException("You must be signed in.");
        long n = Db.Scalar<long>("SELECT COUNT(*) FROM Users WHERE Id = @id AND Role = 'Admin' AND IsActive = 1", ("@id", actor.Id));
        if (n != 1) throw new UnauthorizedAccessException("Only an active administrator can manage user accounts.");
    }

    public static DataTable List(AuthUser actor)
    {
        RequireAdmin(actor);
        return Db.Query(@"SELECT Id, Username, FullName, Role, IsActive, MustChangePassword, LastLoginAt, CreatedAt, CreatedBy
                          FROM Users ORDER BY Username");
    }

    public static void Create(AuthUser actor, string username, string? fullName, UserRole role, string password, bool mustChange)
    {
        RequireAdmin(actor);
        username = (username ?? "").Trim();
        string? err = PasswordPolicy.ValidateUsername(username) ?? PasswordPolicy.Validate(password, username);
        if (err != null) throw new InvalidOperationException(err);
        if (Db.Scalar<long>("SELECT COUNT(*) FROM Users WHERE Username = @u", ("@u", username)) > 0)
            throw new InvalidOperationException("That username is already taken.");

        Db.Execute(@"INSERT INTO Users(Username, FullName, PasswordHash, Role, IsActive, MustChangePassword, CreatedAt, CreatedBy)
                     VALUES(@u, @n, @h, @r, 1, @m, @now, @by)",
            ("@u", username), ("@n", string.IsNullOrWhiteSpace(fullName) ? null : fullName.Trim()),
            ("@h", PasswordHasher.Hash(password)), ("@r", role == UserRole.Admin ? "Admin" : "User"),
            ("@m", mustChange ? 1 : 0), ("@now", Db.Now()), ("@by", actor.Username));
    }

    public static void SetActive(AuthUser actor, long userId, bool active)
    {
        RequireAdmin(actor);
        if (!active && userId == actor.Id) throw new InvalidOperationException("You cannot deactivate your own account.");
        int n = active
            ? Db.Execute("UPDATE Users SET IsActive = 1, FailedAttempts = 0, LockedUntil = NULL WHERE Id = @id", ("@id", userId))
            : Db.Execute(@"UPDATE Users SET IsActive = 0 WHERE Id = @id
                           AND NOT (Role = 'Admin' AND (SELECT COUNT(*) FROM Users WHERE Role = 'Admin' AND IsActive = 1 AND Id <> @id) = 0)",
                         ("@id", userId));
        if (n == 0) throw new InvalidOperationException(active ? "User not found." : "At least one active administrator must remain.");
    }

    public static void SetRole(AuthUser actor, long userId, UserRole role)
    {
        RequireAdmin(actor);
        if (userId == actor.Id && role != UserRole.Admin) throw new InvalidOperationException("You cannot remove your own administrator role.");
        int n = Db.Execute(@"UPDATE Users SET Role = @r WHERE Id = @id
                             AND NOT (@r <> 'Admin' AND Role = 'Admin' AND IsActive = 1
                                      AND (SELECT COUNT(*) FROM Users WHERE Role = 'Admin' AND IsActive = 1 AND Id <> @id) = 0)",
                           ("@r", role == UserRole.Admin ? "Admin" : "User"), ("@id", userId));
        if (n == 0) throw new InvalidOperationException("At least one active administrator must remain.");
    }

    /// <summary>Admin sets a new (temporary) password; the user must change it at the next sign-in.</summary>
    public static void ResetPassword(AuthUser actor, long userId, string newPassword, bool mustChange = true)
    {
        RequireAdmin(actor);
        DataTable t = Db.Query("SELECT Username FROM Users WHERE Id = @id", ("@id", userId));
        if (t.Rows.Count == 0) throw new InvalidOperationException("User not found.");
        string? err = PasswordPolicy.Validate(newPassword, Convert.ToString(t.Rows[0]["Username"]));
        if (err != null) throw new InvalidOperationException(err);

        Db.Execute("UPDATE Users SET PasswordHash = @h, MustChangePassword = @m, FailedAttempts = 0, LockedUntil = NULL WHERE Id = @id",
                   ("@h", PasswordHasher.Hash(newPassword)), ("@m", mustChange ? 1 : 0), ("@id", userId));
    }

    /// <summary>A signed-in user changes their own password (current password required). Returns the refreshed account.</summary>
    public static AuthUser ChangeOwnPassword(AuthUser user, string currentPassword, string newPassword)
    {
        DataTable t = Db.Query("SELECT Username, PasswordHash, IsActive FROM Users WHERE Id = @id", ("@id", user.Id));
        if (t.Rows.Count == 0 || Convert.ToInt64(t.Rows[0]["IsActive"]) != 1)
            throw new UnauthorizedAccessException("This account is not active.");
        if (!PasswordHasher.Verify(currentPassword ?? "", Convert.ToString(t.Rows[0]["PasswordHash"]) ?? "", out _))
            throw new InvalidOperationException("Your current password is incorrect.");

        string? err = PasswordPolicy.Validate(newPassword, Convert.ToString(t.Rows[0]["Username"]));
        if (err != null) throw new InvalidOperationException(err);
        if (newPassword == currentPassword) throw new InvalidOperationException("The new password must be different from the current one.");

        Db.Execute("UPDATE Users SET PasswordHash = @h, MustChangePassword = 0 WHERE Id = @id",
                   ("@h", PasswordHasher.Hash(newPassword)), ("@id", user.Id));
        return AuthService.GetById(user.Id) ?? user;
    }
}

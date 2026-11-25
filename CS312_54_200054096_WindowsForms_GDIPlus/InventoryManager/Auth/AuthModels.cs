namespace InventoryManager.Auth;

public enum UserRole { User = 0, Admin = 1 }

/// <summary>The signed-in account (never contains the password hash).</summary>
public sealed record AuthUser(long Id, string Username, string FullName, UserRole Role, bool IsActive, bool MustChangePassword)
{
    public bool IsAdmin => Role == UserRole.Admin;
    public string DisplayName => string.IsNullOrWhiteSpace(FullName) ? Username : FullName;
}

public enum LoginStatus { Success, InvalidCredentials, Inactive, Locked }

public sealed record LoginResult(LoginStatus Status, AuthUser? User, string Message);

/// <summary>Who is signed in right now. Cleared on logout.</summary>
public static class Session
{
    public static AuthUser? Current { get; private set; }

    /// <summary>Set by the Log out command; tells the startup loop to show the LoginForm again instead of exiting.</summary>
    public static bool LogoutRequested { get; set; }

    public static void Begin(AuthUser user) { Current = user; LogoutRequested = false; }
    public static void End() => Current = null;
}

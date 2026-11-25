using System.Security.Cryptography;

namespace InventoryManager.Auth;

/// <summary>
/// Salted PBKDF2-HMAC-SHA256. Stored format: PBKDF2-SHA256$iterations$saltBase64$hashBase64
/// Passwords are never stored or logged in clear text. The iteration count is stored with each hash,
/// so it can be raised later and old hashes are upgraded automatically at the next successful login.
/// </summary>
public static class PasswordHasher
{
    public const int CurrentIterations = 600_000;   // OWASP guidance for PBKDF2-HMAC-SHA256
    const int SaltBytes = 16;
    const int HashBytes = 32;
    const string Scheme = "PBKDF2-SHA256";
    const int MinAcceptedIterations = 10_000;       // defensive bounds when parsing stored values
    const int MaxAcceptedIterations = 5_000_000;

    public static string Hash(string password, int iterations = CurrentIterations)
    {
        byte[] salt = RandomBytes(SaltBytes);
        byte[] hash = Derive(password, salt, iterations, HashBytes);
        return $"{Scheme}${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    /// <summary>Constant-time verification. needsRehash is true when the stored hash uses fewer iterations than today's default.</summary>
    public static bool Verify(string password, string stored, out bool needsRehash)
    {
        needsRehash = false;
        var parts = (stored ?? "").Split('$');
        if (parts.Length != 4 || parts[0] != Scheme) return false;
        if (!int.TryParse(parts[1], out int iterations) || iterations < MinAcceptedIterations || iterations > MaxAcceptedIterations) return false;

        byte[] salt, expected;
        try { salt = Convert.FromBase64String(parts[2]); expected = Convert.FromBase64String(parts[3]); }
        catch (FormatException) { return false; }
        if (salt.Length < 8 || expected.Length < 16) return false;

        byte[] actual = Derive(password, salt, iterations, expected.Length);
        bool ok = FixedEquals(actual, expected);
        needsRehash = ok && iterations < CurrentIterations;
        return ok;
    }

    static byte[] Derive(string password, byte[] salt, int iterations, int length)
    {
        using var kdf = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        return kdf.GetBytes(length);
    }

    static bool FixedEquals(byte[] a, byte[] b)
    {
        if (a.Length != b.Length) return false;
        int diff = 0;
        for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
        return diff == 0;
    }

    internal static byte[] RandomBytes(int n)
    {
        var buf = new byte[n];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buf);
        return buf;
    }
}

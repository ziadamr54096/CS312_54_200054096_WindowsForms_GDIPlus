using System.Text.RegularExpressions;

namespace InventoryManager.Auth;

/// <summary>Username and password rules shared by every form and by the services (the rules are enforced server-side too).</summary>
public static class PasswordPolicy
{
    public const int MinLength = 8;
    public const int MaxLength = 128;
    public const string Requirements = "At least 8 characters, including a letter and a digit.";

    static readonly Regex UsernamePattern = new(@"^[A-Za-z0-9][A-Za-z0-9._-]{2,31}$", RegexOptions.Compiled);
    static readonly string[] Common = { "password", "12345678", "123456789", "qwerty123", "admin123", "letmein1", "stockora1", "welcome1" };

    public const string UsernameRequirements = "3-32 characters: letters, digits, '.', '_' or '-' (must start with a letter or digit).";

    public static string? ValidateUsername(string? username) =>
        string.IsNullOrWhiteSpace(username) ? "Enter a username." :
        !UsernamePattern.IsMatch(username.Trim()) ? "Username must be " + UsernameRequirements : null;

    /// <summary>Returns a friendly error message, or null when the password is acceptable.</summary>
    public static string? Validate(string? password, string? username)
    {
        if (string.IsNullOrEmpty(password)) return "Enter a password.";
        if (password.Length < MinLength) return $"Password must be at least {MinLength} characters long.";
        if (password.Length > MaxLength) return $"Password must be at most {MaxLength} characters long.";
        if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit)) return "Password must contain at least one letter and one digit.";
        if (!string.IsNullOrWhiteSpace(username) && username.Trim().Length >= 3 &&
            password.IndexOf(username.Trim(), StringComparison.OrdinalIgnoreCase) >= 0)
            return "Password must not contain the username.";
        if (Common.Contains(password.ToLowerInvariant())) return "That password is too common. Choose another one.";
        return null;
    }

    /// <summary>Cryptographically random temporary password that satisfies the policy (no look-alike characters).</summary>
    public static string Generate(int length = 12)
    {
        const string lower = "abcdefghijkmnopqrstuvwxyz", upper = "ABCDEFGHJKLMNPQRSTUVWXYZ", digits = "23456789", symbols = "!@#$%*?";
        string all = lower + upper + digits + symbols;
        var chars = new List<char> { Pick(lower), Pick(upper), Pick(digits), Pick(symbols) };
        while (chars.Count < Math.Max(length, 8)) chars.Add(Pick(all));
        for (int i = chars.Count - 1; i > 0; i--) { int j = RandomInt(i + 1); (chars[i], chars[j]) = (chars[j], chars[i]); }
        return new string(chars.ToArray());
    }

    static char Pick(string set) => set[RandomInt(set.Length)];

    static int RandomInt(int maxExclusive)   // unbiased (rejection sampling)
    {
        uint limit = uint.MaxValue - (uint.MaxValue % (uint)maxExclusive);
        while (true)
        {
            uint v = BitConverter.ToUInt32(PasswordHasher.RandomBytes(4), 0);
            if (v < limit) return (int)(v % (uint)maxExclusive);
        }
    }
}
